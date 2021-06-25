using System;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core;

namespace SQLcomplianceCwfAddin.Helpers.SQL
{
    /// <summary>
    /// This class was created based on the Idera.SQLcompliance.Application.GUI.SQL and probably should be moved to the SQLcomplianceCore in the future release
    /// </summary>
    public class SqlDirect : IDisposable
    {
        #region Properties

        private string errMsg;
        private bool _ownConnection = false;
        private SqlConnection _conn = null;

        public SqlConnection Connection
        {
            get { return _conn; }
            set { _conn = value; _ownConnection = false; }
        }

        public bool IsConnected
        {
            get { return _ownConnection && _conn != null; }
        }

        #endregion

        #region GetLastError

        public string GetLastError()
        {
            return errMsg;
        }

        #endregion

        #region Connection Management

        //-----------------------------------------------------------------------------
        // OpenConnection - open a connection to a SQL server
        //-----------------------------------------------------------------------------
        public bool OpenConnection(string serverName)
        {
            bool retval;

            try
            {
                string strConn = String.Format("server={0};" +
                                                "integrated security=SSPI;" +
                                               "Connect Timeout=30;" +
                                                "Application Name='{1}';",
                                                serverName,
                                                CoreConstants.ManagementConsoleName);

                // Cleanup any previous connections if necessary
                CloseConnection();

                _conn = new SqlConnection(strConn);
                _conn.Open();

                _ownConnection = true;

                errMsg = "";
                retval = true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                retval = false;
            }

            return retval;
        }

        //-----------------------------------------------------------------------------
        // CloseConnection - close the connection to the SQLsecure configuration database
        //-----------------------------------------------------------------------------
        public void CloseConnection()
        {
            if (IsConnected)
            {
                _conn.Close();
                _conn.Dispose();
                _conn = null;
            }
        }

        public void Dispose()
        {
            CloseConnection();
        }

        public static SqlDirect OpenDirectConnection(string serverName)
        {
            var sqlServer = new SqlDirect();
            sqlServer.OpenConnection(serverName);
            return sqlServer;
        }

        #endregion
    }
}
