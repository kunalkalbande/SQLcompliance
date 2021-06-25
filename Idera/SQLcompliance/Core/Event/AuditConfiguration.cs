using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Idera.SQLcompliance.Core.Event
{
    /// <summary>
    ///     Audit Category Values
    /// </summary>
    public enum AuditCategory
    {
        Unknown = -1,
        Logins = 1,
        DDL = 2,
        Security = 3,
        DML = 4,
        SELECT = 5,
        Admin = 6,
        Audit = 7,
        FailedLogins = 8,
        DBCC = 9,
        Broker = 10,
        SystemEvents = 11,
        Logouts = 12, // SQLCM-5375-6.1.4.3-Capture Logout Events
        ServerStartStopEvent = 998,

        UDC = 999 // User defined
    }

    [DataContract]
    public enum AccessCheckFilter
    {
        [EnumMember]
        SuccessOnly = 0,

        [EnumMember]
        NoFilter = 1,

        [EnumMember]
        FailureOnly = 2
    }

    [DataContract]
    public enum SensitiveColumnActivity
    {
        [EnumMember]
        SelectOnly = 0,

        [EnumMember]
        SelectAndDML = 1,

        [EnumMember]
        AllActivity = 2
    }
    /// <summary>
    ///     Summary description for AuditConfiguration.  This class handles SQLsecure audit
    ///     settings.
    /// </summary>
    public class AuditConfiguration
    {
        #region Constructors

        public AuditConfiguration()
        {
            auditedCategories = new Hashtable();
            auditServerRoleList = new ArrayList();
            auditDBRoleList = new ArrayList();
            auditObjectList = new ArrayList();
            auditUserList = new ArrayList();
            auditUserGroups = new ArrayList();
            privUserList = new ArrayList();
            auditLevel = 0;
            version = 0;
            auditUserCategories = new Hashtable();
            //v5.6 SQLCM-5373
            auditTrustedUserServerRoleList = new ArrayList();
            auditTrustedUsersServerList = new ArrayList();
        }

        #endregion

        #region Protected data members

        protected Hashtable auditedCategories;
        //v5.6 SQLCM-5373
        protected ArrayList auditTrustedUserServerRoleList;
        protected ArrayList auditTrustedUsersServerList;

        protected ArrayList auditServerRoleList;
        protected ArrayList auditDBRoleList;
        protected ArrayList auditObjectList;
        protected ArrayList auditUserList;
        protected ArrayList auditUserGroups;
        protected ArrayList privUserList;
        protected int auditLevel;
        protected int auditCategory;
        protected int version;
        //protected bool           auditFailures;  // Obsolete since 2.1.  Use auditAccessCheck instead.
        protected bool auditCaptureDetails;

        //5.4_4.1.1_Extended Events
        protected bool auditCaptureDetailsXE;
        protected bool auditCaptureSQLXE;

        protected bool auditCaptureTransactions;
        protected bool auditCaptureDDL;
        protected bool auditExceptions;
        protected AccessCheckFilter auditAccessCheck;

        protected Hashtable auditUserCategories;
        protected bool auditUserCaptureSql;
        protected bool auditUserCaptureTransactions;
        protected bool auditUserCaptureDDL;
        protected bool auditUserExceptions;
        protected AccessCheckFilter auditUserAccessCheck;

        //5.5 Audit Logs
        protected bool isAuditLogsEnabled;
        //v5.6 SQLCM-5471
        private SensitiveColumnActivity auditSensitiveColumnActivity;
        private SensitiveColumnActivity auditSensitiveColumnActivityDataset;

        #endregion

        #region Properties

        public bool AuditDDL
        {
            get { return auditedCategories.ContainsKey(AuditCategory.DDL); }
            set { EnableAuditedCategory(AuditCategory.DDL, value); }
        }

        public bool AuditSecurity
        {
            get { return auditedCategories.ContainsKey(AuditCategory.Security); }
            set { EnableAuditedCategory(AuditCategory.Security, value); }
        }

        public bool AuditDML
        {
            get { return auditedCategories.ContainsKey(AuditCategory.DML); }
            set { EnableAuditedCategory(AuditCategory.DML, value); }
        }

        public bool AuditSELECT
        {
            get { return auditedCategories.ContainsKey(AuditCategory.SELECT); }
            set { EnableAuditedCategory(AuditCategory.SELECT, value); }
        }

        public virtual bool AuditLogins
        {
            get { return auditedCategories.ContainsKey(AuditCategory.Logins); }
            set { EnableAuditedCategory(AuditCategory.Logins, value); }
        }

        /// <summary>
        /// SQLCM-5375-6.1.4.3-Capture Logout Events
        /// </summary>
        public virtual bool AuditLogouts
        {
            get { return auditedCategories.ContainsKey(AuditCategory.Logouts); }
            set { EnableAuditedCategory(AuditCategory.Logouts, value); }
        }

        public virtual bool AuditFailedLogins
        {
            get { return auditedCategories.ContainsKey(AuditCategory.FailedLogins); }
            set { EnableAuditedCategory(AuditCategory.FailedLogins, value); }
        }

        public virtual bool AuditAdmin
        {
            get { return auditedCategories.ContainsKey(AuditCategory.Admin); }
            set { EnableAuditedCategory(AuditCategory.Admin, value); }
        }

        public bool AuditDBCC
        {
            get { return auditedCategories.ContainsKey(AuditCategory.DBCC); }
            set { EnableAuditedCategory(AuditCategory.DBCC, value); }
        }

        public bool AuditBroker
        {
            get { return auditedCategories.ContainsKey(AuditCategory.Broker); }
            set { EnableAuditedCategory(AuditCategory.Broker, value); }
        }

        public bool AuditSystemEvents
        {
            get { return auditedCategories.ContainsKey(AuditCategory.SystemEvents); }
            set { EnableAuditedCategory(AuditCategory.SystemEvents, value); }
        }

        public bool AuditUserEvents
        {
            get { return auditedCategories.ContainsKey(AuditCategory.UDC); }
            set { EnableAuditedCategory(AuditCategory.UDC, value); }
        }

        public AccessCheckFilter AuditAccessCheck
        {
            get { return auditAccessCheck; }
            set { auditAccessCheck = value; }
        }

        public bool AuditCaptureDetails
        {
            get { return auditCaptureDetails; }
            set { auditCaptureDetails = value; }
        }

        public bool AuditCaptureTransactions
        {
            get { return auditCaptureTransactions; }
            set { auditCaptureTransactions = value; }
        }

        public bool AuditCaptureDDL
        {
            get { return auditCaptureDDL; }
            set { auditCaptureDDL = value; }
        }

        public bool AuditExceptions
        {
            get { return auditExceptions; }
            set { auditExceptions = value; }
        }

        //5.4_4.1.1_Extended Events
        public bool AuditCaptureDetailsXE
        {
            get { return auditCaptureDetailsXE; }
            set { auditCaptureDetailsXE = value; }
        }

        //5.4_4.1.1_Extended Events
        public bool AuditCaptureSQLXE
        {
            get { return auditCaptureSQLXE; }
            set { auditCaptureSQLXE = value; }
        }

        public AuditCategory[] AuditedCategories
        {
            get
            {
                if (auditedCategories.Count == 0)
                    return null;

                var categories = new AuditCategory[auditedCategories.Count];
                var enumerator = auditedCategories.GetEnumerator();
                if (enumerator != null)
                {
                    var i = 0;
                    while (enumerator.MoveNext())
                    {
                        categories[i++] = (AuditCategory)enumerator.Value;
                    }
                }
                return categories;
            }
        }

        public int[] AuditedServerRoles
        {
            get { return (int[])auditServerRoleList.ToArray(typeof(int)); }
        }

        public int[] AuditedDBRoles
        {
            get { return (int[])auditDBRoleList.ToArray(typeof(int)); }
        }

        public string[] AuditedPrivUsers
        {
            get { return (string[])privUserList.ToArray(typeof(string)); }
        }

        public string[] AuditedUsers
        {
            get { return (string[])auditUserList.ToArray(typeof(string)); }
            set
            {
                auditUserList.Clear();
                if (value != null)
                {
                    for (var i = 0; i < value.Length; i++)
                        auditUserList.Add(value[i]);
                }
            }
        }
        //v5.6 SQLCM-5373
        public string[] AuditedServerTrustedUsers
        {
            get { return (string[])auditTrustedUsersServerList.ToArray(typeof(string)); }
            set
            {
                auditTrustedUsersServerList.Clear();
                if (value != null)
                {
                    for (var i = 0; i < value.Length; i++)
                        auditTrustedUsersServerList.Add(value[i]);
                }
            }
        }

        public string[] AuditedUserGroups
        {
            get { return (string[])auditUserGroups.ToArray(typeof(string)); }
        }

        public int[] AuditObjectList
        {
            get { return (int[])auditObjectList.ToArray(typeof(int)); }
        }

        public int Version
        {
            get { return version; }
            set { version = value; }
        }

        public bool AuditUserDDL
        {
            get { return auditedCategories.ContainsKey(AuditCategory.DDL); }
            set { EnableAuditedCategory(AuditCategory.DDL, value); }
        }

        public bool AuditUserSecurity
        {
            get { return auditedCategories.ContainsKey(AuditCategory.Security); }
            set { EnableAuditedCategory(AuditCategory.Security, value); }
        }

        public bool AuditUserDML
        {
            get { return auditedCategories.ContainsKey(AuditCategory.DML); }
            set { EnableAuditedCategory(AuditCategory.DML, value); }
        }

        public bool AuditUserSELECT
        {
            get { return auditedCategories.ContainsKey(AuditCategory.SELECT); }
            set { EnableAuditedCategory(AuditCategory.SELECT, value); }
        }

        public virtual bool AuditUserLogins
        {
            get { return auditedCategories.ContainsKey(AuditCategory.Logins); }
            set { EnableAuditedCategory(AuditCategory.Logins, value); }
        }

        /// <summary>
        /// SQLCM-5375-6.1.4.3-Capture Logout Events
        /// </summary>
        public virtual bool AuditUserLogouts
        {
            get { return auditedCategories.ContainsKey(AuditCategory.Logouts); }
            set { EnableAuditedCategory(AuditCategory.Logouts, value); }
        }

        public virtual bool AuditUserFailedLogins
        {
            get { return auditedCategories.ContainsKey(AuditCategory.FailedLogins); }
            set { EnableAuditedCategory(AuditCategory.FailedLogins, value); }
        }

        public virtual bool AuditUserAdmin
        {
            get { return auditedCategories.ContainsKey(AuditCategory.Admin); }
            set { EnableAuditedCategory(AuditCategory.Admin, value); }
        }

        public bool AuditUserDBCC
        {
            get { return auditedCategories.ContainsKey(AuditCategory.DBCC); }
            set { EnableAuditedCategory(AuditCategory.DBCC, value); }
        }

        public bool AuditPrivUserEvents
        {
            get { return auditedCategories.ContainsKey(AuditCategory.UDC); }
            set { EnableAuditedCategory(AuditCategory.UDC, value); }
        }

        public AccessCheckFilter AuditUserAccessCheck
        {
            get { return auditUserAccessCheck; }
            set { auditUserAccessCheck = value; }
        }

        public bool AuditUserCaptureSql
        {
            get { return auditUserCaptureSql; }
            set { auditUserCaptureSql = value; }
        }

        public bool AuditUserCaptureTransactions
        {
            get { return auditUserCaptureTransactions; }
            set { auditUserCaptureTransactions = value; }
        }

        public bool AuditUserCaptureDDL
        {
            get 
            { 
                return auditUserCaptureDDL; 
            }

            set 
            { 
                auditUserCaptureDDL = value; 
            }
        }

        public bool AuditUserExceptions
        {
            get { return auditUserExceptions; }
            set { auditUserExceptions = value; }
        }


        public AuditCategory[] AuditUserCategories
        {
            get
            {
                if (auditUserCategories.Count == 0)
                    return null;

                AuditCategory[] categories = new AuditCategory[auditUserCategories.Count];
                IDictionaryEnumerator enumerator = auditUserCategories.GetEnumerator();
                if (enumerator != null)
                {
                    int i = 0;
                    while (enumerator.MoveNext())
                    {
                        categories[i++] = (AuditCategory)enumerator.Value;
                    }
                }
                return categories;
            }
        }

        //5.5 Audit Logs
        public bool IsAuditLogsEnabled
        {
            get { return isAuditLogsEnabled; }
            set { isAuditLogsEnabled = value; }
        }

        //v5.6 SQLCM-5471
        public SensitiveColumnActivity AuditSensitiveColumnActivity
        {
            get { return auditSensitiveColumnActivity; }
            set { auditSensitiveColumnActivity = value; }
        }
        public SensitiveColumnActivity AuditSensitiveColumnActivityDataset
        {
            get { return auditSensitiveColumnActivityDataset; }
            set { auditSensitiveColumnActivityDataset = value; }
        }
        #endregion

        #region Public methods

        public virtual void
            AddAuditedCategory(
            AuditCategory category
            )
        {
            EnableAuditedCategory(category, true);
        }

        public virtual void
            RemoveAuditedCategory(
            AuditCategory category
            )
        {
            EnableAuditedCategory(category, false);
        }

        public virtual void
            AddAuditedServerRole(
            int serverRoleID
            )
        {
            EnableAuditedServerRole(serverRoleID, true);
        }

        //v5.6 SQLCM-5373
        public virtual void
            AddAuditedServerTrustedUserRole(
            int serverRoleID
            )
        {
            EnableAuditedServerTrustedUserRole(serverRoleID, true);
        }

        public virtual void
            RemoveAuditedServerRole(
            int serverRoleID
            )
        {
            EnableAuditedServerRole(serverRoleID, false);
        }

        public virtual void
            AddAuditedUser(
            string userName
            )
        {
            if (userName != null)
                auditUserList.Add(userName);
        }

        public virtual void
            RemoveAuditedUser(
            string userName
            )
        {
            if (userName == null)
                return;

            if (auditUserList.Contains(userName))
                auditUserList.Remove(userName);
        }

        //v5.6 SQLCM-5373
        public virtual void
            AddAuditedServerTrustedUser(
            string userName
            )
        {
            if (userName != null)
                auditTrustedUsersServerList.Add(userName);
        }

        public virtual void
            RemoveuditedServerTrustedUser(
            string userName
            )
        {
            if (userName == null)
                return;

            if (auditTrustedUsersServerList.Contains(userName))
                auditTrustedUsersServerList.Remove(userName);
        }

        

        public virtual void
            AddAuditedUserGroup(
            string group
            )
        {
            if (group != null)
                auditUserGroups.Add(group);
        }

        public virtual void
            AddPrivUser(
            string privUser
            )
        {
            if (privUser != null)
                privUserList.Add(privUser);
        }

        public virtual void
            AddAuditedDBRole(
            int dbRole
            )
        {
            EnableAuditedDBRole(dbRole, true);
        }

        public virtual void
            RemoveAuditedDBRole(
            int dbRole
            )
        {
            EnableAuditedDBRole(dbRole, false);
        }

        public virtual void
            AddAuditedObject(
            int obj
            )
        {
            EnableAuditedObject(obj, true);
        }

        public virtual void
            RemoveAuditedObject(
            int obj
            )
        {
            EnableAuditedObject(obj, true);
        }

        public virtual void
          AddAuditUserCategory(
          AuditCategory category
          )
        {
            EnableAuditUserCategory(category, true);
        }

        public virtual void
           RemoveAuditUserCategory(
           AuditCategory category
           )
        {
            EnableAuditUserCategory(category, false);
        }

        /// <summary>
        ///     Compares if two audit configurations are equal
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public virtual bool
            Equals(
            AuditConfiguration config
            )
        {
            // Implement this
            return false;
        }

        public virtual void
            ClearAuditedCategories()
        {
            auditedCategories.Clear();
        }

        public virtual void
            ClearAuditedSereredRoles()
        {
            auditServerRoleList.Clear();
        }

        public virtual void
            ClearPrivUserList()
        {
            privUserList.Clear();
        }

        public virtual void
            ClearAuditedDBRoles()
        {
            auditDBRoleList.Clear();
        }

        public virtual void
            ClearAuditedObjects()
        {
            auditObjectList.Clear();
        }

        public virtual void
            ClearAuditedUsers()
        {
            auditUserList.Clear();
        }
        //v5.6 SQLCM-5373s
        public virtual void
            ClearAuditedServerTrustedUsers()
        {
            auditTrustedUsersServerList.Clear();
        }
        public virtual void
            ClearAuditedServerTrustedUsersRolesList()
        {
            auditTrustedUserServerRoleList.Clear();
        }

        public virtual void
            ClearAuditedUserGroups()
        {
            auditUserGroups.Clear();
        }

        public virtual void
          ClearAuditUserCategories()
        {
            auditUserCategories.Clear();
        }


        public virtual void
            CopyTo(
            AuditConfiguration config
            )
        {
            if (config == null)
                throw new ArgumentNullException("config");

            config.ClearAuditedCategories();
            config.ClearAuditedDBRoles();
            config.ClearAuditedObjects();
            config.ClearAuditedSereredRoles();
            config.ClearAuditedUsers();
            //v5.6 SQLCM-5373
            config.ClearAuditedServerTrustedUsers();
            config.ClearAuditedServerTrustedUsersRolesList();
            config.ClearAuditedUserGroups();
            config.ClearPrivUserList();
            config.ClearAuditUserCategories();

            foreach (AuditCategory category in auditedCategories)
                config.AddAuditedCategory(category);

            foreach (int serverRole in auditServerRoleList)
                config.AddAuditedServerRole(serverRole);

            foreach (int dbRole in auditDBRoleList)
                config.AddAuditedDBRole(dbRole);

            foreach (int obj in auditObjectList)
                config.AddAuditedObject(obj);

            config.Version = version;
            config.AuditCaptureDetails = auditCaptureDetails;
            //5.4_4.1.1_Extended Events
            config.AuditCaptureDetailsXE = auditCaptureDetailsXE;
            config.AuditCaptureSQLXE = auditCaptureSQLXE;//5.4_4.1.1_Extended Events
            config.isAuditLogsEnabled = isAuditLogsEnabled; // 5.5 Audit Logs
            config.AuditCaptureTransactions = auditCaptureTransactions;
            config.AuditExceptions = auditExceptions;
            config.auditAccessCheck = auditAccessCheck;
            foreach (AuditCategory category in auditUserCategories)
                config.AddAuditUserCategory(category);

            config.AuditUserCaptureSql = auditUserCaptureSql;
            config.AuditUserCaptureTransactions = auditUserCaptureTransactions;
            config.AuditUserExceptions = auditUserExceptions;
            config.auditUserAccessCheck = auditUserAccessCheck;
            //SQLCM-5471 - v5.6
            config.AuditSensitiveColumnActivity = auditSensitiveColumnActivity;
            config.AuditSensitiveColumnActivityDataset = auditSensitiveColumnActivityDataset;
        }

        //------------------------------------------------------
        // Clear
        //------------------------------------------------------
        /// <summary>
        ///     Clear the configuration.
        /// </summary>
        public virtual void
            Clear()
        {
            //v5.6 SQLCM-5373
            ClearAuditedServerTrustedUsers();
            ClearAuditedServerTrustedUsersRolesList();

            ClearAuditedUsers();
            ClearAuditedObjects();
            ClearAuditedDBRoles();
            ClearAuditedCategories();
            ClearAuditedSereredRoles();
            ClearAuditedUserGroups();
            ClearAuditedDBRoles();
            ClearPrivUserList();
            AuditCaptureDetails = false;
            AuditCaptureDetailsXE = false;//5.4_4.1.1_Extended Events
            AuditCaptureSQLXE = false;//5.4_4.1.1_Extended Events
            IsAuditLogsEnabled = false; // 5.5 Audit Logs
            AuditCaptureTransactions = false;
            AuditExceptions = false;
            auditAccessCheck = AccessCheckFilter.SuccessOnly;
            auditSensitiveColumnActivity = SensitiveColumnActivity.SelectOnly;// SQLCM-5471 v5.6 Add Activity to Senstitive columns
            auditSensitiveColumnActivityDataset = SensitiveColumnActivity.SelectOnly;// SQLCM-5471 v5.6 Add Activity to Senstitive columns

            ClearAuditUserCategories();
            this.AuditUserCaptureSql = false;
            this.AuditUserCaptureTransactions = false;
            this.AuditUserExceptions = false;
            this.AuditUserAccessCheck = AccessCheckFilter.SuccessOnly;
            this.AuditSensitiveColumnActivity = SensitiveColumnActivity.SelectOnly; // SQLCM-5471 v5.6 Add Activity to Senstitive columns
            this.AuditSensitiveColumnActivityDataset = SensitiveColumnActivity.SelectOnly; // SQLCM-5471 v5.6 Add Activity to Senstitive columns
        }

        #endregion

        #region Protected Methods

        protected void
            EnableAuditedCategory(
            AuditCategory category,
            bool enable)
        {
            try
            {
                ValidateCategory(category);
            }
            catch (Exception e) // invalid category.  just ignore it.
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                    e,
                    true);
            }

            try
            {
                if (enable)
                {
                    auditedCategories.Add(category, category);
                }
                else
                {
                    auditedCategories.Remove(category);
                }
            }
            catch (ArgumentException)
            {
                // Ignore it.
            }
        }

        protected void
            ValidateCategory(
            AuditCategory category
            )
        {
            if (category < AuditCategory.Logins ||
                (category > AuditCategory.Logouts && // SQLCM-5375-6.1.4.3-Capture Logout Events
                 category != AuditCategory.UDC))
                throw new CoreException(String.Format(CoreConstants.Exception_Format_InvalidAuditCategory,
                    (int)category));
        }

        /// <summary>
        ///     Enable or disable auditing a server role
        /// </summary>
        /// <param name="serverRole"></param>
        /// <param name="enable"></param>
        /// //v5.6 SQLCM-5373
        protected virtual void
            EnableAuditedServerTrustedUserRole(
            int serverRole,
            bool enable
            )
        {
            if (enable)
            {
                if (!auditTrustedUserServerRoleList.Contains(serverRole))
                    auditTrustedUserServerRoleList.Add(serverRole);
            }
            else // disable
            {
                if (auditTrustedUserServerRoleList.Contains(serverRole))
                    auditTrustedUserServerRoleList.Remove(serverRole);
            }
        }

        /// <summary>
        ///     Enable or disable auditing a server role
        /// </summary>
        /// <param name="serverRole"></param>
        /// <param name="enable"></param>
        protected virtual void
            EnableAuditedServerRole(
            int serverRole,
            bool enable
            )
        {
            if (enable)
            {
                if (!auditServerRoleList.Contains(serverRole))
                    auditServerRoleList.Add(serverRole);
            }
            else // disable
            {
                if (auditServerRoleList.Contains(serverRole))
                    auditServerRoleList.Remove(serverRole);
            }
        }

        protected void
            EnableAuditedObject(
            int obj,
            bool enable
            )
        {
            if (enable)
            {
                if (!auditObjectList.Contains(obj))
                    auditObjectList.Add(obj);
            }
            else // disable
            {
                if (auditObjectList.Contains(obj))
                    auditObjectList.Remove(obj);
            }
        }

        protected void
            EnableAuditedDBRole(
            int dbRole,
            bool enable
            )
        {
            if (enable)
            {
                if (!auditDBRoleList.Contains(dbRole))
                    auditDBRoleList.Add(dbRole);
            }
            else // disable
            {
                if (auditDBRoleList.Contains(dbRole))
                    auditDBRoleList.Remove(dbRole);
            }
        }

        protected void
          EnableAuditUserCategory(
             AuditCategory category,
          bool enable)
        {
            try
            {
                ValidateUserCategory(category);
            }
            catch (Exception e) // invalid category.  just ignore it.
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);

            }

            try
            {
                if (enable)
                {
                    auditUserCategories.Add(category, category);
                }
                else
                {
                    auditUserCategories.Remove(category);
                }
            }
            catch (ArgumentException)
            {
                // Ignore it.
            }
        }

        protected void
           ValidateUserCategory(
           AuditCategory category
           )
        {
            if (category < AuditCategory.Logins ||
               (category > AuditCategory.Logouts && // SQLCM-5375-6.1.4.3-Capture Logout Events
               category != AuditCategory.UDC))
                throw new CoreException(String.Format(CoreConstants.Exception_Format_InvalidAuditCategory, (int)category));
        }

        #endregion
    }
}