using Idera.SQLcompliance.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    /// <summary>
    /// This class encapsulates permissions assigned to a SQLdm user.   
    /// </summary>
    [Serializable]
    public class UserToken
    {
        #region members

        // flag columns
        private const int ColFlagIsSecurityEnabled = 0;
        private const int ColFlagIsSysadmin = 1;
        private const int ColFlagIsSQLdmAdministrator = 2;

        // assigned server columns
        private const int ColSQLServerID = 0;
        private const int ColInstanceName = 1;
        private const int ColActive = 2;
        private const int ColDeleted = 3;
        private const int ColPermissionType = 4;

        private bool m_IsSecurityEnabled = true;
        private bool m_IsSysadmin = true;
        private bool m_IsSQLdmAdministrator = false;
        private byte[] m_SID = null;//SQLdm 10.0 (Gaurav Karwal): The SID to be used in rest api
        private Dictionary<int, ServerPermission> m_ServersDictionaryById = new Dictionary<int, ServerPermission>();
        private Dictionary<string, ServerPermission> m_ServersDictionaryByName = new Dictionary<string, ServerPermission>();
        private List<ServerPermission> m_ActiveServers = new List<ServerPermission>();

        #endregion

        private static void getCredentials(
                string connectionString,
                out bool isIntegratedSecurity,
                out string sqlLogin,
                out byte[] sidByteArray
            )
        {
            // Init return.
            isIntegratedSecurity = false;
            sqlLogin = "";
            sidByteArray = new byte[85];

            // Get credential information from connection string if using SQL login
            // get name of login.   Else get the windows user sid byte array of the
            // currently connected user.
            SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder(connectionString);
            isIntegratedSecurity = scsb.IntegratedSecurity;
            if (isIntegratedSecurity)
            {
                WindowsIdentity.GetCurrent().User.GetBinaryForm(sidByteArray, 0);
            }
            else
            {
                sqlLogin = scsb.UserID;
            }
        }

        #region ctors

        public UserToken()
        {
        }

        #endregion

        #region properties

        public bool IsSecurityEnabled
        {
            get
            {
                bool flag = false;
                lock (this)
                {
                    flag = m_IsSecurityEnabled;
                }
                return flag;
            }
        }

        //[START] SQLdm 10.0 (Gaurav Karwal): getting the SID to be used in rest api
        public string UserSID
        {
            get
            {
                return new SecurityIdentifier(m_SID, 0).Value;
            }
        }
        //[END] SQLdm 10.0 (Gaurav Karwal): getting the SID to be used in rest api

        public bool IsSysadmin
        {
            get
            {
                bool flag = false;
                lock (this)
                {
                    flag = m_IsSysadmin;
                }
                return flag;
            }
        }

        public bool IsSQLdmAdministrator
        {
            get
            {
                bool flag = false;
                lock (this)
                {
                    flag = m_IsSQLdmAdministrator;
                }
                return flag;
            }
        }        

        #endregion

        #region methods

        /// <summary>
        /// Refresh by connecting to the repository and determining permissions.
        /// </summary>
        /// <param name="connectionString"></param>
        public void Refresh(string connectionString)
        {
            lock (this)
            {
                // Clear internal fields.
                m_IsSecurityEnabled = m_IsSysadmin = m_IsSQLdmAdministrator = false;
                m_ServersDictionaryById.Clear();
                m_ServersDictionaryByName.Clear();
                m_ActiveServers.Clear();

                // Get credentials information.
                bool isIntegratedSecurity = false;
                string sqlLogin = "";
                byte[] sidByteArray = null;
                getCredentials(connectionString, out isIntegratedSecurity, out sqlLogin, out sidByteArray);

                m_SID = sidByteArray;//SQLdm 10.0 (Gaurav Karwal): getting the SID to be used in rest api

                // Create parameters for user token SP.
                SqlParameter[] arParms = new SqlParameter[3];
                arParms[0] = new SqlParameter("@IsSQLLogin", SqlDbType.Bit);
                arParms[0].Direction = ParameterDirection.Input;
                arParms[0].Value = !isIntegratedSecurity;

                arParms[1] = new SqlParameter("@SQLLoginName", SqlDbType.NVarChar);
                arParms[1].Direction = ParameterDirection.Input;
                arParms[1].Value = sqlLogin;

                arParms[2] = new SqlParameter("@WindowsSID", SqlDbType.Binary);
                arParms[2].Direction = ParameterDirection.Input;
                arParms[2].Value = sidByteArray;

                // Get user token from repository and fill the fields.
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, "p_GetUserToken", arParms))
                {
                    // Read the security enabled, sysadmin and sqldm admin flags.
                    if (reader.Read())
                    {
                        m_IsSecurityEnabled = reader.IsDBNull(ColFlagIsSecurityEnabled) ? false : reader.GetSqlBoolean(ColFlagIsSecurityEnabled).Value;
                        m_IsSysadmin = reader.IsDBNull(ColFlagIsSysadmin) ? false : reader.GetSqlBoolean(ColFlagIsSysadmin).Value;
                        m_IsSQLdmAdministrator = reader.IsDBNull(ColFlagIsSQLdmAdministrator) ? false : reader.GetSqlBoolean(ColFlagIsSQLdmAdministrator).Value;
                    }

                    // Read assigned servers.
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            // Get server ID and permission type.
                            int sqlServerID = reader.GetSqlInt32(ColSQLServerID).Value;
                            PermissionType permissionType = (PermissionType)reader.GetSqlInt32(ColPermissionType).Value;

                            // If server already in assigned server update its permission,
                            // Else add a new server permission.
                            ServerPermission serverPermission = null;
                            if (m_ServersDictionaryById.TryGetValue(sqlServerID, out serverPermission))
                            {
                                serverPermission.updateToHighestPermission(permissionType);
                            }
                            else
                            {
                                string instanceName = reader.GetSqlString(ColInstanceName).Value;
                                bool active = reader.GetSqlBoolean(ColActive).Value;
                                bool deleted = reader.GetSqlBoolean(ColDeleted).Value;
                                serverPermission = new ServerPermission(sqlServerID, instanceName, active, deleted, permissionType);
                                m_ServersDictionaryById.Add(sqlServerID, serverPermission);
                                m_ServersDictionaryByName.Add(instanceName, serverPermission);
                            }

                            // If server is active, add to active servers list.
                            if (serverPermission.Server.Active)
                            {
                                m_ActiveServers.Add(serverPermission);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the permission assigned for the server.
        /// </summary>
        /// <param name="sqlServerID"></param>
        /// <returns></returns>
        public PermissionType GetServerPermission(int sqlServerID)
        {
            PermissionType pt = PermissionType.None;

            lock (this)
            {
                // If security is enabled get permission assigned to the server
                // else return admin permission.
                if (IsSecurityEnabled)
                {
                    ServerPermission serverPermission = null;
                    if (m_ServersDictionaryById.TryGetValue(sqlServerID, out serverPermission))
                    {
                        pt = serverPermission.PermissionType;
                    }
                }
                else
                {
                    pt = PermissionType.Administrator;
                }
            }

            return pt;
        }       

        #endregion
    }


    /// <summary>
    /// This class encapsulates server permission
    /// </summary>
    [Serializable]
    public class ServerPermission : IComparable
    {
        #region members

        private Server m_Server;
        private PermissionType m_PermissionType;

        #endregion

        #region ctors

        public ServerPermission(int sqlServerID, string instanceName, bool active, bool deleted, PermissionType permissionType)
        {
            m_Server = new Server(sqlServerID, instanceName, active, deleted);
            m_PermissionType = permissionType;
        }

        #endregion

        #region properties

        public Server Server
        {
            get { return m_Server; }
        }

        public PermissionType PermissionType
        {
            get { return m_PermissionType; }
        }

        #endregion

        #region methods

        public void updateToHighestPermission(PermissionType pt)
        {
            // If the existing permission type is View and
            // input is Modify, then set to Modify.....etc.
            if ((int)m_PermissionType < (int)pt)
            {
                m_PermissionType = pt;
            }
        }

        public override bool Equals(object rhs)
        {
            if (rhs == this) { return true; }

            ServerPermission other = rhs as ServerPermission;
            if (other == null) return false;

            return m_Server.Equals(other.m_Server);
        }

        public override int GetHashCode()
        {
            return m_Server.GetHashCode();
        }

        public int CompareTo(object rhs)
        {
            if (this == rhs) return 0;

            ServerPermission other = rhs as ServerPermission;
            if (other == null) return 1;

            return m_Server.CompareTo(other);
        }

        public override string ToString()
        {
            return m_Server.InstanceName + ":" + m_PermissionType;
        }

        #endregion
    }

    /// <summary>
    /// Permissions supported by SQLdm.
    /// </summary>
    public enum PermissionType { View, Modify, Administrator, None };

    /// <summary>
    /// Type of login.
    /// </summary>
    public enum LoginType
    {
        [Description("Unknown")]
        Unknown,
        [Description("Windows Authentication")]
        WindowsUser,
        [Description("Windows Authentication")]
        WindowsGroup,
        [Description("SQL Server Authentication")]
        SQLLogin
    };

    /// <summary>
    /// This class encapsulates basic monitored SQL Server information.
    /// </summary>
    [Serializable]
    public class Server : IComparable
    {
        #region members

        private int m_SQLServerID;
        private string m_InstanceName;
        private bool m_Active;
        private bool m_Deleted;

        #endregion

        #region ctors

        public Server(int sqlServerID, string instanceName, bool active, bool deleted)
        {
            m_SQLServerID = sqlServerID;
            m_InstanceName = instanceName;
            m_Active = active;
            m_Deleted = deleted;
        }

        #endregion

        #region properties

        public int SQLServerID
        {
            get { return m_SQLServerID; }
        }

        public string InstanceName
        {
            get { return m_InstanceName; }
        }

        public bool Active
        {
            get { return m_Active; }
            set { m_Active = value; }
        }

        public bool Deleted
        {
            get { return m_Deleted; }
        }

        #endregion

        #region methods

        public override bool Equals(object rhs)
        {
            if (rhs == this) { return true; }

            Server other = rhs as Server;
            if (other == null) return false;

            return m_InstanceName.Equals(other.m_InstanceName);
        }

        public override int GetHashCode()
        {
            return m_InstanceName.GetHashCode();
        }

        public int CompareTo(object rhs)
        {
            if (this == rhs) return 0;

            Server other = rhs as Server;
            if (other == null) return 1;

            return m_InstanceName.CompareTo(other.m_InstanceName);
        }

        public override string ToString()
        {
            return m_InstanceName;
        }

        #endregion
    }

    /// <summary>
    /// Contains the result of checking a single license key.
    /// </summary>
    [Serializable]
    public class CheckedKey
    {
        public string KeyString;// Original key string passed to SummarizeKeys.

        // KeyObject is the esult of deserializing KeyString.  It returns
        // null if KeyString can't be deserialized.
        // We would store the KeyObject instance if BBSLic were serializable.
        public BBS.License.BBSLic KeyObject
        {
            get
            {
                BBS.License.BBSLic temp = new BBS.License.BBSLic();
                BBS.License.LicErr licErr = temp.LoadKeyString(KeyString);
                if (licErr == BBS.License.LicErr.OK || licErr == BBS.License.LicErr.FutureKey)
                {
                    return temp;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool IsValid;    // False if expired, etc.

        public string Comment;  // Reason why key is not valid.

        public CheckedKey(string key) { KeyString = key; }
    }
}
