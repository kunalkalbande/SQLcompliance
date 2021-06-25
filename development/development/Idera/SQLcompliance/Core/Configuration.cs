// TODO: Break into two sets of routines: Audit Settings and General


using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;
using System.Text;
using Idera.SQLcompliance.Core.Event ;
using Idera.SQLcompliance.Core.Licensing;

namespace Idera.SQLcompliance.Core
{
    using System.Data;

    using Idera.SQLcompliance.Core.Settings;

    using Microsoft.ApplicationBlocks.Data;

    /// <summary>
	/// Singleton class holding SQLcompliance Configuration data - defaults etc
	/// Summary description for Configuration.
	/// </summary>
	public class SQLcomplianceConfiguration
	{
	    private const string GetRepositoryInfoStoredProcedure = "sp_sqlcm_GetRepositoryInfo";
        private const string ConnectionInfo = "connectionInfo";
        private const string NoConnection = "No Connection";
        private const string InsertRepositoryInfoStoredProcedure = "sp_sqlcm_InsertRepositoryInfo";

        #region Private Properties		

        public  string _repositoryServer ;
      
      // Audit Settings
		private bool   auditLogins;          // Audit login events
		private bool   auditLogouts;          // Audit logout events
		private bool   auditFailedLogins;    // Audit failed login attempts
		private bool   auditDDL;             // Audit DDL operations - Add, Alter, Drop
      private bool   auditSecurity;        // Audit operations that affect permissions
		private bool   auditAdmin;           // Audit administrtive actions
		private bool   auditDML;             // Audit DML operations - Insert, Update, Delete
		private bool   auditSELECT;          // Audit SELECTs
		
		// Filters
		private int    auditAccessCheck;        // Audit failed operations as well as successful
		private bool   auditSystemEvents;    // Audit system and user process events (SQL Server 2005 only)
		
		// DML Filtering
		private bool    auditDmlAll;
		private int     auditUserTables;
		private bool    auditSystemTables;
		private bool    auditStoredProcedures;
		private bool    auditDmlOther;
		
		// Privileged Users
		private bool           auditUserAll;
		private bool           auditUserLogins;
        private bool           auditUserLogouts;
		private bool           auditUserFailedLogins;
		private bool           auditUserDDL;
      private bool           auditUserAdmin;
      private bool           auditUserSecurity;
		private bool           auditUserDML;
		private bool           auditUserSELECT;
		private int            auditUserAccessCheck;
		
		private int            serverPort;  // port server is listening on
		private int            agentPort;   // default port for agents
		private string         server;      // server host (machine name)
		
		private int            sqlComplianceDbSchemaVersion;
		private int            eventsDbSchemaVersion;
		private string         serverVersion;
		private string         serverCoreVersion;
		
		// archive settings
		private int            archiveOn;
		private string         archiveTimeZoneName;
		private int            archiveBias;
		private int            archiveStandardBias;
		private DateTime       archiveStandardDate;
		private int            archiveDaylightBias;
		private DateTime       archiveDaylightDate;
		private int            archiveInterval;
		private int            archiveAge;
		private int            archivePeriod;
		private string         archivePrefix;
		private DateTime       archiveLastTime;
		private DateTime       archiveNextTime;
      private bool           archiveCheckIntegrity ;

        private string              _archiveDatabaseFilesLocation;
	    private ArchiveScheduleType _archiveSchedule;
	    private DateTime            _archiveScheduleTime;
	    private int                 _archiveScheduleRepetition;
	    private bool[]              _archiveScheduleWeekdays = new bool[7];
	    private int                 _archiveScheduleDayOrWeekOfMonth;
		
		// grooming
		private int            groomLogAge;
		private int            groomAlertAge;
		private bool           groomEventAllow;
		private int            groomEventAge;
		
      //  SQLcompliance License		
		private string          licenseKey;
      private BBSProductLicense licenseObject ;
		
		// snapshot
		private int            snapshotInterval;
		private DateTime       snapshotLastTime;
	
	   // server heartbeat	
   	private DateTime       serverLastHeartbeatTime;
   	
   	// database creation
   	private int            recoveryModel;  // 0=model  1=simple

      // login filtering options
      private bool           loginCollapse;
      private int            loginTimespan;
      private int            loginCacheSize;

      private int collectionServerHeartbeatInterval;
      private DateTime indexStartTime;
      private TimeSpan indexDuration;

      private int alwaysOnRoleUpdateInterval;

      // threshold notification mail

      public String smtpServer;
      public String smtpUsername;
      public String smtpPassword;
      public String smtpSenderAddress;
      public int smtpAuthType;
      public int smtpPort;
      public bool smtpSsl = false;
      public String smtpRecieverAddress;
		
		#endregion

        /// <summary>
        /// Automatic archiving schedule type like daily, no schedule, etc.
        /// </summary>
	    public enum ArchiveScheduleType
	    {
	        NoSchedule,
            Daily,
            Weekly,
            MonthlyDateWise,
            MonthlyWeekdayWise
	    }
		
		#region Public Properties

      // General		
		public int    ServerPort
		{
		   get { return serverPort;  }
		   set { serverPort = value; }
		}
		public int    AgentPort
		{
		   get { return agentPort;  }
		   set { agentPort = value; }
		}
		public string    Server
		{
		   get { return server;  }
		   set { server = value; }
		}
		public int  SqlComplianceDbSchemaVersion
		{
		   get { return sqlComplianceDbSchemaVersion;  }
		   set { sqlComplianceDbSchemaVersion = value; }
		}
		public int    EventsDbSchemaVersion
		{
		   get { return eventsDbSchemaVersion;  }
		   set { eventsDbSchemaVersion = value; }
		}
		public string    ServerVersion
		{
		   get { return serverVersion;  }
		   set { serverVersion = value; }
		}
		public string    ServerCoreVersion
		{
		   get { return serverCoreVersion;  }
		   set { serverCoreVersion = value; }
		}

   // Audit Settings
		public bool AuditLogins
		{
		   get { return auditLogins; }
		   set { auditLogins = value; }
		}
        /// <summary>
        /// SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
        /// </summary>
	    public bool AuditLogouts
	    {
	        get { return auditLogouts; }
	        set { auditLogouts = value; }
	    }
		public bool AuditFailedLogins
		{
		   get { return auditFailedLogins; }
		   set { auditFailedLogins = value; }
		}
		public bool AuditDDL
		{
		   get { return auditDDL; }
		   set { auditDDL = value; }
		}
      public bool AuditSecurity
		{
		   get { return auditSecurity; }
		   set { auditSecurity = value; }
		}
		public bool AuditAdmin
		{
		   get { return auditAdmin; }
		   set { auditAdmin = value; }
		}
		public bool AuditDML
		{
		   get { return auditDML; }
		   set { auditDML = value; }
		}
		public bool AuditSELECT
		{
		   get { return auditSELECT; }
		   set { auditSELECT = value; }
		}
		public AccessCheckFilter AuditAccessCheck
		{
		   get { return (AccessCheckFilter)auditAccessCheck; }
		   set { auditAccessCheck = (int)value; }
		}
      public bool AuditSystemEvents
      {
         get { return auditSystemEvents; }
         set { auditSystemEvents = value; }
      }
		
		// DML Filtering
		public bool    AuditDmlAll
		{
		   get { return auditDmlAll;  }
		   set { auditDmlAll = value; }
		}
		public int    AuditUserTables
		{
		   get { return auditUserTables;  }
		   set { auditUserTables = value; }
		}
		public bool    AuditSystemTables
		{
		   get { return auditSystemTables;  }
		   set { auditSystemTables = value; }
		}
		public bool    AuditStoredProcedures
		{
		   get { return auditStoredProcedures;  }
		   set { auditStoredProcedures = value; }
		}
		public bool    AuditDmlOther
		{
		   get { return auditDmlOther;  }
		   set { auditDmlOther = value; }
		}
		
		public bool    AuditUserAll
		{
		   get { return auditUserAll;  }
		   set { auditUserAll = value; }
		}
		public bool    AuditUserLogins
		{
		   get { return auditUserLogins;  }
		   set { auditUserLogins = value; }
		}
        /// <summary>
        /// SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
        /// </summary>
	    public bool AuditUserLogouts
	    {
	        get { return auditUserLogouts; }
	        set { auditUserLogouts = value; }
	    }
		public bool    AuditUserFailedLogins
		{
		   get { return auditUserFailedLogins;  }
		   set { auditUserFailedLogins = value; }
		}
		public bool    AuditUserDDL
		{
		   get { return auditUserDDL;  }
		   set { auditUserDDL = value; }
		}
      public bool    AuditUserAdmin
      {
         get { return auditUserAdmin;  }
         set { auditUserAdmin = value; }
      }
      public bool    AuditUserSecurity
		{
		   get { return auditUserSecurity;  }
		   set { auditUserSecurity = value; }
		}
		public bool    AuditUserDML
		{
		   get { return auditUserDML;  }
		   set { auditUserDML = value; }
		}
		public bool    AuditUserSELECT
		{
		   get { return auditUserSELECT;  }
		   set { auditUserSELECT = value; }
		}
		public AccessCheckFilter    AuditUserAccessCheck
		{
		   get { return (AccessCheckFilter)auditUserAccessCheck;  }
		   set { auditUserAccessCheck = (int)value; }
		}

      // archive fields		
		public int ArchiveOn
		{
		   get { return archiveOn; }
		   set { archiveOn = value; }
		}
		public string ArchiveTimeZoneName
		{
		   get { return archiveTimeZoneName; }
		   set { archiveTimeZoneName = value; }
		}

		public int ArchiveBias
		{
		   get { return archiveBias; }
		   set { archiveBias = value; }
		}
		public int ArchiveStandardBias
		{
		   get { return archiveStandardBias; }
		   set { archiveStandardBias = value; }
		}
		public DateTime ArchiveStandardDate
		{
		   get { return archiveStandardDate; }
		   set { archiveStandardDate = value; }
		}
		public int ArchiveDaylightBias
		{
		   get { return archiveDaylightBias; }
		   set { archiveDaylightBias = value; }
		}
		public DateTime ArchiveDaylightDate
		{
		   get { return archiveDaylightDate; }
		   set { archiveDaylightDate = value; }
		}

		public int ArchiveInterval
		{
		   get { return archiveInterval; }
		   set { archiveInterval = value; }
		}
		public int ArchiveAge
		{
		   get { return archiveAge; }
		   set { archiveAge = value; }
		}
		public int ArchivePeriod
		{
		   get { return archivePeriod; }
		   set { archivePeriod = value; }
		}
		public string ArchivePrefix
		{
		   get { return archivePrefix; }
		   set { archivePrefix = value; }
		}
		public DateTime ArchiveLastTime
		{
		   get { return archiveLastTime; }
		   set { archiveLastTime = value; }
		}
		public DateTime ArchiveNextTime
		{
		   get { return archiveNextTime; }
		   set { archiveNextTime = value; }
		}

	    public string ArchiveDatabaseFilesLocation
	    {
	        get { return _archiveDatabaseFilesLocation; }
            set { _archiveDatabaseFilesLocation = value; }
        }

	    public ArchiveScheduleType ArchiveSchedule
	    {
	        get { return _archiveSchedule; } 
            set { _archiveSchedule = value; }
	    }

	    public DateTime ArchiveScheduleTime
	    {
	        get { return _archiveScheduleTime; }
            set { _archiveScheduleTime = value; }
	    }

	    public int ArchiveScheduleRepetition
	    {
	        get { return _archiveScheduleRepetition; }
            set { _archiveScheduleRepetition = value; }
	    }

	    public bool[] ArchiveScheduleWeekDays
	    {
	        get { return _archiveScheduleWeekdays; }
            set { _archiveScheduleWeekdays = value; }
	    }

	    public int ArchiveScheduleDayOrWeekOfMonth
	    {
	        get { return _archiveScheduleDayOrWeekOfMonth; }
            set { _archiveScheduleDayOrWeekOfMonth = value; }
	    }

      public bool ArchiveCheckIntegrity
      {
         get { return archiveCheckIntegrity ; }
         set { archiveCheckIntegrity = value ; }
      }
		
		public int GroomLogAge
		{
		   get { return groomLogAge;  }
		   set { groomLogAge = value; }
		}
		public int GroomAlertAge
		{
		   get { return groomAlertAge;  }
		   set { groomAlertAge = value; }
		}
		public bool GroomEventAllow
		{
		   get { return groomEventAllow;  }
		   set { groomEventAllow = value; }
		}
		public int GroomEventAge
		{
		   get { return groomEventAge;  }
		   set { groomEventAge = value; }
		}
		
		public string OldLicenseKey
		{
		   get { return licenseKey; }
		   set
		   {
		      licenseKey= value;
         }
		}
		
		public BBSProductLicense LicenseObject
		{
		   get { return licenseObject; }
		}
		
		public int    SnapshotInterval
		{
		   get { return snapshotInterval;  }
		   set { snapshotInterval = value; }
		}
		public DateTime    SnapshotLastTime
		{
		   get { return snapshotLastTime;  }
		   set { snapshotLastTime = value; }
		}

		public DateTime    ServerLastHeartbeatTime
		{
		   get { return serverLastHeartbeatTime;  }
		   set { serverLastHeartbeatTime = value; }
		}
		
		public int RecoveryModel
		{
		   get { return recoveryModel; }
		   set { recoveryModel = value; }
		}

      public bool LoginCollapse
      {
         get { return loginCollapse; }
         set { loginCollapse = value; }
      }
      public int LoginTimespan
      {
         get { return loginTimespan; }
         set { loginTimespan = value; }
      }
      public int LoginCacheSize
      {
         get { return loginCacheSize; }
         set { loginCacheSize = value; }
      }

      public int CollectionServerHeartbeatInterval
      {
         get { return collectionServerHeartbeatInterval; }
         set { collectionServerHeartbeatInterval = value; }
      }

      public int AlwaysOnRoleUpdateInterval
      {
          get { return alwaysOnRoleUpdateInterval; }
          set { alwaysOnRoleUpdateInterval = value; }
      }

      public DateTime IndexStartTime
      {
         get { return indexStartTime; }
         set { indexStartTime = value; }
      }
      
      public TimeSpan IndexDuration
      {
         get { return indexDuration; }
         set { indexDuration = value; }
      }
	
		#endregion

		#region LastError
		
	   static string           errMsg   = "";
	   static public  string   GetLastError() { return errMsg; } 
	   
	   #endregion
		
		#region Public Methods
		
		//------------------------------------------------------------------
		// Read
		//------------------------------------------------------------------
		public bool Read(SqlConnection conn)
		{
		   bool retval = false;

		   try
		   {
            _repositoryServer = conn.DataSource.ToUpper() ;
			   string cmdstr = GetSelectSQL();

			   using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
			   {
               using (SqlDataReader reader = cmd.ExecuteReader())
               {
			         if (reader.Read())
                  {
                     Load(reader);
                     retval = true;
                  }
               }
            }
            LoadLicenseData(conn) ;
		   }
		   catch (Exception ex)
		   {
		      errMsg = ex.Message;
		   }
		   return retval;
		}

      private void LoadLicenseData(SqlConnection conn)
      {
         string scope = _repositoryServer  ;
         Version v = Assembly.GetExecutingAssembly().GetName().Version;
         string versionString = v.Major + "." + v.Minor ;
         licenseObject = new BBSProductLicense(conn.ConnectionString, scope, CoreConstants.ProductID, versionString) ;
      }

      public void ResetLicense(SqlConnection conn)
      {
         LoadLicenseData(conn) ;
      }

      public bool IsLicenseOk()
      {
         if (licenseObject == null)
         {
            return false;
         }
         else
         {
            return licenseObject.CombinedLicense.licState == BBSProductLicense.LicenseState.Valid;
         }
      }

	   //------------------------------------------------------------------
		// Write
		//------------------------------------------------------------------
		public bool Write(SqlConnection conn)
		{
		   bool retval = false;
		   
		   try
		   {
			   string cmdstr = GetUpdateSQL();

			   using ( SqlCommand cmd = new SqlCommand( cmdstr, conn ) )
			   {
               cmd.ExecuteNonQuery();
            }
            retval = true;
		   }
		   catch ( Exception ex )
		   {
		      errMsg = ex.Message;
		   }
		   return retval;
		}
		
      static public bool IsCompatibleSchema(SqlConnection conn)
      {
         int dbSchema, eventsSchema ;

         if (!ReadSchemaVersions(conn, out dbSchema, out eventsSchema))
         {
            ErrorLog.Instance.Write( CoreConstants.Exception_CantReadRepository, GetLastError(), ErrorLog.Severity.Error );
            return false ;
         }

         if (eventsSchema / 100 != CoreConstants.RepositoryEventsDbSchemaVersion / 100)
         {
            return false;
         }
         else if (dbSchema / 100 != CoreConstants.RepositorySqlComplianceDbSchemaVersion / 100)
         {
            return false;
         }
         else
         {
            return true;
         }
      }

		//------------------------------------------------------------------
		// ReadSchemaVersions
		//------------------------------------------------------------------
		static public bool ReadSchemaVersions(SqlConnection conn, 
                                            out int outSqlComplianceDbSchemaVersion, 
                                            out int outEventsDbSchemaVersion)
		{
		   bool success         = false;
		   
         outSqlComplianceDbSchemaVersion = -1;
         outEventsDbSchemaVersion    = -1;
		   
		   try
		   {
			   string cmdstr = String.Format( "SELECT sqlComplianceDbSchemaVersion,eventsDbSchemaVersion from {0}..{1}",
			                                  CoreConstants.RepositoryDatabase,
			                                  CoreConstants.RepositoryConfigurationTable );
			   using ( SqlCommand cmd = new SqlCommand( cmdstr, conn ) )
			   {
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
                  if ( reader.Read() )
                  {
		               outSqlComplianceDbSchemaVersion = SQLHelpers.GetInt32(reader,0);
		               outEventsDbSchemaVersion        = SQLHelpers.GetInt32(reader,1);
                     success = true;
                  }
                  else
                  {
                     errMsg = "Internal Error - Schema Version read failed";
                  }
               }
            }
		   }
		   catch ( Exception ex )
		   {
		      errMsg = ex.Message;
		   }
		   
		   return success;
      }
		
		//------------------------------------------------------------------
		// WriteServer
		//------------------------------------------------------------------
		static public void WriteServer(SqlConnection conn,          
                                     string server,
		                               string serverVersion, 
                                     string serverCoreVersion)
		{
	      string cmdstr = "";

		   try
		   {
			   cmdstr = String.Format( "UPDATE {0}..{1} set server={2},serverVersion='{3}',serverCoreVersion='{4}'",
			                           CoreConstants.RepositoryDatabase,
			                           CoreConstants.RepositoryConfigurationTable,
			                           SQLHelpers.CreateSafeString(server),
			                           serverVersion,
			                           serverCoreVersion );
			   using ( SqlCommand cmd = new SqlCommand( cmdstr, conn ) )
			   {
               cmd.ExecuteNonQuery();
            }
		   }
		   catch ( Exception ex )
		   {
		      errMsg = ex.Message;
		      throw ex;
		   }
		}
		
		//------------------------------------------------------------------
		// GetRecoveryModel
		//------------------------------------------------------------------
		static public int GetRecoveryModel()
		{
		   int model = 0;
		   
         string sql = String.Format( "SELECT recoveryModel from  {0}..{1}",
                                     CoreConstants.RepositoryDatabase,
                                     CoreConstants.RepositoryConfigurationTable );
         Repository rep = new Repository();
         
         try
         {
            rep.OpenConnection();
            
            using ( SqlCommand cmd = new SqlCommand( sql, rep.connection ) )
            {
               object obj = cmd.ExecuteScalar();
               if( obj is System.DBNull )
               {
                  model = 0;
               }
               else
               {
                  model = (int)obj;
               }
            }
         }
         finally
         {
            rep.CloseConnection();
         }            
         return model;
		}
		
		//------------------------------------------------------------------
		// SetRecoveryModel
		//------------------------------------------------------------------
		static public void SetRecoveryModel(int newRecoveryModel)
		{
         string sql = String.Format( "UPDATE {0}..{1} SET timeLastModified = GETUTCDATE(),recoveryModel = {2}",
                                     CoreConstants.RepositoryDatabase,
                                     CoreConstants.RepositoryConfigurationTable,
                                     newRecoveryModel );
         Repository rep = new Repository();
         
         try
         {
            rep.OpenConnection();
            
            using ( SqlCommand cmd = new SqlCommand( sql, rep.connection ) )
            {
               cmd.ExecuteNonQuery();
            }
         }
         finally
         {
            rep.CloseConnection();
         }                                     
		}
		
      //------------------------------------------------------------------
      // GetLoginFilterOptions
      //------------------------------------------------------------------
      static public void GetLoginFilterOptions( out bool loginCollapse ,
                                                out int  loginTimespan ,
                                                out int  loginCacheSize
         )
      {
         string sql = String.Format( "SELECT loginCollapse,loginTimespan,loginCacheSize from  {0}..{1}",
                                     CoreConstants.RepositoryDatabase,
                                     CoreConstants.RepositoryConfigurationTable );
         Repository rep = new Repository();
         
         try
         {
            rep.OpenConnection();
            
            using ( SqlCommand cmd = new SqlCommand( sql, rep.connection ) )
            {
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
                  if ( reader.Read() )
                  {
                     loginCollapse  = SQLHelpers.ByteToBool(reader,0);
                     loginTimespan  = SQLHelpers.GetInt32(reader,1);
                     loginCacheSize = SQLHelpers.GetInt32(reader,2);
                  }
                  else
                  {
                     throw new Exception("Internal Error - Could not read login filter options");
                  }
               }
            }
         }
         finally
         {
            rep.CloseConnection();
         }            
      }
		
      //------------------------------------------------------------------
      // SetLoginFilterOptions
      //------------------------------------------------------------------
      static public void SetLoginFilterOptions( bool loginCollapse,
                                                int  loginTimespan,
                                                int  loginCacheSize)
      {
         string sql = String.Format( "UPDATE {0}..{1} SET timeLastModified = GETUTCDATE()" +
                                                          ",loginCollapse = {2}" +
                                                          ",loginTimespan = {3}" +
                                                          ",loginCacheSize = {4}",
                                     CoreConstants.RepositoryDatabase,
                                     CoreConstants.RepositoryConfigurationTable,
                                     loginCollapse ? 1 : 0,
                                     loginTimespan,
                                     loginCacheSize );
            
         Repository rep = new Repository();
         
         try
         {
            rep.OpenConnection();
            
            using ( SqlCommand cmd = new SqlCommand( sql, rep.connection ) )
            {
               cmd.ExecuteNonQuery();
            }
         }
         finally
         {
            rep.CloseConnection();
         }                                     
      }

        static public void SetArchiveSchedulerExecutionPlan(DateTime[] executionPlan)
        {
            CultureInfo usCulture = CultureInfo.CreateSpecificCulture("EN-us");

            // create string with each DateTime seperated by pipe '|'
            StringBuilder executionPlanString = new StringBuilder();
            for (int index = 0; index < executionPlan.Length; index += 1)
                executionPlanString.AppendFormat("{0}|", executionPlan[index].ToString("yyyy-MM-dd HH:mm:ss", usCulture));

            string sql = String.Format("UPDATE {0}..{1} SET archiveScheduleExecutionPlan = {2}",
                                        CoreConstants.RepositoryDatabase,
                                        CoreConstants.RepositoryConfigurationTable,
                                        SQLHelpers.CreateSafeString(executionPlanString.ToString()));

            Repository repository = new Repository();
            try
            {
                repository.OpenConnection();

                using (SqlCommand cmd = new SqlCommand(sql, repository.connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                repository.CloseConnection();
            }
        }

        static public DateTime[] GetArchiveSchedulerExecutionPlan()
        {
            CultureInfo usCulture = CultureInfo.CreateSpecificCulture("EN-us");
            List<DateTime> executionPlan = new List<DateTime>();

            string sql = String.Format("SELECT archiveScheduleExecutionPlan FROM {0}..{1}",
                                        CoreConstants.RepositoryDatabase,
                                        CoreConstants.RepositoryConfigurationTable);

            Repository repository = new Repository();
            try
            {
                repository.OpenConnection();

                using (SqlCommand cmd = new SqlCommand(sql, repository.connection))
                {
                    object valueReturned = cmd.ExecuteScalar();
                    if (valueReturned != null)
                    {
                        // DateTime values are seperated by pipes '|'
                        string[] planDateStrings = valueReturned.ToString().Split(new string[]{"|"}, StringSplitOptions.RemoveEmptyEntries);
                        for (int index = 0; index < planDateStrings.Length; index += 1)
                            executionPlan.Add(DateTime.ParseExact(planDateStrings[index], "yyyy-MM-dd HH:mm:ss", usCulture));
                    }
                }
            }
            finally
            {
                repository.CloseConnection();
            }

            return executionPlan.ToArray();
        }

        static public void RemoveArchiveScheduleExecutionPlan()
        {
            string sql = String.Format("UPDATE {0}..{1}  SET archiveScheduleExecutionPlan = {2}",
                 CoreConstants.RepositoryDatabase, CoreConstants.RepositoryConfigurationTable, "NULL");

            Repository repository = new Repository();
            try
            {
                repository.OpenConnection();
                using (SqlCommand cmd = new SqlCommand(sql, repository.connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                repository.CloseConnection();
            }
        }


	    public static SQLcomplianceConfiguration GetConfiguration()
	    {
            SQLcomplianceConfiguration configuration = new SQLcomplianceConfiguration();

	        Repository repository = new Repository();
	        try
	        {
	            repository.OpenConnection();
	            configuration.Read(repository.connection);
	        }
	        finally
	        {
	            repository.CloseConnection();
	        }

	        return configuration;
	    }

		#endregion
		
		#region Private Methods

      //-------------------------------------------------------------
      // Load - Loads DatabaseRecord from SELECT result set
      //--------------------------------------------------------------
      private void Load(SqlDataReader reader)
      {
         int col=0;
         
         // Audit Settings
         auditLogins          = SQLHelpers.ByteToBool( reader, col++);
         auditFailedLogins    = SQLHelpers.ByteToBool( reader, col++);
         auditDDL             = SQLHelpers.ByteToBool( reader, col++);
         auditSecurity        = SQLHelpers.ByteToBool( reader, col++);
         auditAdmin           = SQLHelpers.ByteToBool( reader, col++);
         auditDML             = SQLHelpers.ByteToBool( reader, col++);
         auditSELECT          = SQLHelpers.ByteToBool( reader, col++);
         
         // Filters
         auditAccessCheck        = SQLHelpers.ByteToInt( reader, col++);
         auditSystemEvents    = SQLHelpers.ByteToBool( reader, col++);
         
         // Audit Objects
         auditDmlAll           = SQLHelpers.ByteToBool( reader, col++);
         auditUserTables       = SQLHelpers.ByteToInt(  reader, col++);
         auditSystemTables     = SQLHelpers.ByteToBool( reader, col++);
         auditStoredProcedures = SQLHelpers.ByteToBool( reader, col++);
         auditDmlOther         = SQLHelpers.ByteToBool( reader, col++);

         // Privileged Users         
         auditUserAll          = SQLHelpers.ByteToBool( reader, col++);
         auditUserLogins       = SQLHelpers.ByteToBool( reader, col++);
         auditUserFailedLogins = SQLHelpers.ByteToBool( reader, col++);
         auditUserDDL          = SQLHelpers.ByteToBool( reader, col++);
         auditUserAdmin        = SQLHelpers.ByteToBool( reader, col++);
         auditUserSecurity     = SQLHelpers.ByteToBool( reader, col++);
         auditUserDML          = SQLHelpers.ByteToBool( reader, col++);
         auditUserSELECT       = SQLHelpers.ByteToBool( reader, col++);
         auditUserAccessCheck  = SQLHelpers.ByteToInt( reader, col++);
         
         // license
         licenseKey            = SQLHelpers.GetString(  reader, col++);
//         licenseObject         = new IderaLicense(licenseKey, _repositoryServer);
         
         // general
         agentPort              = SQLHelpers.GetInt32( reader, col++ );
         serverPort             = SQLHelpers.GetInt32( reader, col++ );
         server                 = SQLHelpers.GetString( reader, col++ );
         
         sqlComplianceDbSchemaVersion = SQLHelpers.GetInt32( reader, col++ );
         eventsDbSchemaVersion    = SQLHelpers.GetInt32( reader, col++ );
         serverVersion            = SQLHelpers.GetString( reader, col++ );
         serverCoreVersion        = SQLHelpers.GetString( reader, col++ );
         
         // archive
         archiveOn            = SQLHelpers.GetInt32( reader, col++ );
         archiveTimeZoneName  = SQLHelpers.GetString( reader, col++ );

         archiveBias          = SQLHelpers.GetInt32( reader, col++ );
         archiveStandardBias  = SQLHelpers.GetInt32( reader, col++ );
         archiveStandardDate  = SQLHelpers.GetDateTime( reader, col++ );
         archiveDaylightBias  = SQLHelpers.GetInt32( reader, col++ );
         archiveDaylightDate  = SQLHelpers.GetDateTime( reader, col++ );

         archiveInterval      = SQLHelpers.GetInt32( reader, col++ );
         archiveAge           = SQLHelpers.GetInt32( reader, col++ );
         archivePeriod        = SQLHelpers.GetInt32( reader, col++ );
         archivePrefix        = SQLHelpers.GetString( reader, col++ );
         archiveLastTime      = SQLHelpers.GetDateTime( reader, col++);
         archiveNextTime      = SQLHelpers.GetDateTime( reader, col++);
         archiveCheckIntegrity = SQLHelpers.ByteToBool(reader, col++) ;
         
         // grooming
         groomLogAge     = SQLHelpers.GetInt32( reader, col++ );
         groomAlertAge   = SQLHelpers.GetInt32( reader, col++ );
         groomEventAllow = SQLHelpers.ByteToBool( reader, col++ );
         groomEventAge   = SQLHelpers.GetInt32( reader, col++ );
         
         // snapshot
         snapshotInterval  = SQLHelpers.GetInt32( reader, col++ );
         snapshotLastTime  = SQLHelpers.GetDateTime( reader, col++);
         
         // last heartbeat
         serverLastHeartbeatTime = SQLHelpers.GetDateTime( reader, col++);
         
         // new database recovery model
         recoveryModel  = SQLHelpers.GetInt32( reader, col++ );
         
         // login filter options
         loginCollapse  = SQLHelpers.ByteToBool( reader, col++ );
         loginTimespan  = SQLHelpers.GetInt32( reader, col++ );
         loginCacheSize = SQLHelpers.GetInt32( reader, col++ );

         collectionServerHeartbeatInterval = SQLHelpers.GetInt32(reader, col++);
         alwaysOnRoleUpdateInterval = SQLHelpers.GetInt32(reader, col++);

         if (reader.IsDBNull(col))
         {
            indexStartTime = DateTime.MinValue;
            col++;
         }
         else
            indexStartTime = SQLHelpers.GetDateTime(reader, col++);

         if (reader.IsDBNull(col))
         {
            indexDuration = new TimeSpan();
            col++;
         }
         else
            indexDuration = TimeSpan.FromSeconds(SQLHelpers.GetInt32(reader, col++));

        // archive
        _archiveDatabaseFilesLocation = SQLHelpers.GetString(reader, col++);
        _archiveSchedule = (ArchiveScheduleType) SQLHelpers.GetInt32(reader, col++);
        _archiveScheduleTime = SQLHelpers.GetDateTime(reader, col++);
        _archiveScheduleRepetition = SQLHelpers.GetInt32(reader, col++);

          // if no schedule time set, use 1:30AM
          if (_archiveScheduleTime.Equals(DateTime.MinValue))
              _archiveScheduleTime = new DateTime(2014, 1, 1, 1, 30, 00);

        string weekDay = SQLHelpers.GetString(reader, col++);
       
          if (string.IsNullOrEmpty(weekDay) || weekDay.Length != 7)
            weekDay = "0000000";

        _archiveScheduleWeekdays[0] = weekDay.Substring(0, 1).Equals("1", StringComparison.InvariantCultureIgnoreCase);
        _archiveScheduleWeekdays[1] = weekDay.Substring(1, 1).Equals("1", StringComparison.InvariantCultureIgnoreCase);
        _archiveScheduleWeekdays[2] = weekDay.Substring(2, 1).Equals("1", StringComparison.InvariantCultureIgnoreCase);
        _archiveScheduleWeekdays[3] = weekDay.Substring(3, 1).Equals("1", StringComparison.InvariantCultureIgnoreCase);
        _archiveScheduleWeekdays[4] = weekDay.Substring(4, 1).Equals("1", StringComparison.InvariantCultureIgnoreCase);
        _archiveScheduleWeekdays[5] = weekDay.Substring(5, 1).Equals("1", StringComparison.InvariantCultureIgnoreCase);
        _archiveScheduleWeekdays[6] = weekDay.Substring(6, 1).Equals("1", StringComparison.InvariantCultureIgnoreCase);

        _archiveScheduleDayOrWeekOfMonth = SQLHelpers.GetInt32(reader, col++);
        // Read Audit Logouts events
        auditLogouts = SQLHelpers.ByteToBool(reader, col++);
        auditUserLogouts = SQLHelpers.ByteToBool(reader, col);
      }

        #endregion

        #region SQL

        //-------------------------------------------------------------
        // GetSelectSQL
        //--------------------------------------------------------------
        private static string GetSelectSQL()
      {
          string tmp = "SELECT " + 
                           "auditLogins,auditFailedLogins" +
			                  ",auditDDL,auditSecurity,auditAdmin,auditDML,auditSELECT" + 
			                  ",auditFailures,auditSystemEvents" +
			                  ",auditDmlAll,auditUserTables,auditSystemTables" +
			                  ",auditStoredProcedures,auditDmlOther" +
	                        ",auditUserAll,auditUserLogins,auditUserFailedLogins" +
			                  ",auditUserDDL,auditUserAdmin,auditUserSecurity,auditUserDML,auditUserSELECT" +
			                  ",auditUserFailures" +
			                  ",licenseKey" +
			                  ",agentPort,serverPort,server" +
			                  ",sqlComplianceDbSchemaVersion,eventsDbSchemaVersion" +
			                  ",serverVersion,serverCoreVersion" +
			                  ",archiveOn,archiveTimeZoneName" +
			                  ",archiveBias,archiveStandardBias,archiveStandardDate,archiveDaylightBias,archiveDaylightDate" +
			                  ",archiveInterval,archiveAge,archivePeriod,archivePrefix" +
			                  ",archiveLastTime,archiveNextTime,archiveCheckIntegrity" +
			                  ",groomLogAge,groomAlertAge,groomEventAllow,groomEventAge" +
			                  ",snapshotInterval,snapshotLastTime,serverLastHeartbeatTime" +
			                  ",recoveryModel" +
			                  ",loginCollapse" +
                           ",loginTimespan" +
                           ",loginCacheSize" +
                           ",collectionServerHeartbeatInterval" +
                           ",alwaysOnRoleUpdateInterval" +
                           ",indexStartTime" +
                           ",indexDurationInSeconds" +
                           ",archiveDatabaseFilesLocation" + 
                           ",archiveScheduleType" +
                           ",archiveScheduleTime" + 
                           ",archiveScheduleRepetition" +
                           ",archiveScheduleWeekDay" +
                           ",archiveScheduleDayOrWeekOfMonth" +
                           ",auditLogouts" +  // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
                           ",auditUserLogouts" +
                           " FROM {0} ";
		                     
         return String.Format( tmp, CoreConstants.RepositoryConfigurationTable );
      }
      
      //-----------------------------------------------------------------
      // GetUpdateSQL - Create UPDATE SQL to update configuration record
      //-----------------------------------------------------------------
      private string GetUpdateSQL()
      {
          string weekDays = string.Format("{0}{1}{2}{3}{4}{5}{6}",
                                          _archiveScheduleWeekdays[0] ? 1 : 0,
                                          _archiveScheduleWeekdays[1] ? 1 : 0,
                                          _archiveScheduleWeekdays[2] ? 1 : 0,
                                          _archiveScheduleWeekdays[3] ? 1 : 0,
                                          _archiveScheduleWeekdays[4] ? 1 : 0,
                                          _archiveScheduleWeekdays[5] ? 1 : 0,
                                          _archiveScheduleWeekdays[6] ? 1 : 0);

         StringBuilder tmp = new StringBuilder("");
         
         //--------------------
         // General Properties
         //--------------------
         tmp.AppendFormat( "UPDATE {0} SET ",
                           CoreConstants.RepositoryConfigurationTable );
         tmp.Append(       "timeLastModified = GETUTCDATE()" );

         //-------------------------------------------------------------------------
         // Audit Settings
         //-------------------------------------------------------------------------
         tmp.AppendFormat( ",auditLogins = {0}",        this.auditLogins ? 1 : 0 );
         tmp.AppendFormat( ",auditLogouts = {0}",        this.auditLogouts ? 1 : 0 );
         tmp.AppendFormat( ",auditFailedLogins = {0}",  this.auditFailedLogins ? 1 : 0 );
         tmp.AppendFormat( ",auditDDL = {0}",           this.auditDDL ? 1 : 0 );
         tmp.AppendFormat( ",auditSecurity = {0}",      this.auditSecurity ? 1 : 0 );
         tmp.AppendFormat( ",auditAdmin = {0}",         this.auditAdmin ? 1 : 0 );
         tmp.AppendFormat( ",auditDML = {0}",           this.auditDML ? 1 : 0 );
         tmp.AppendFormat( ",auditSELECT = {0}",        this.auditSELECT ? 1 : 0 );
         tmp.AppendFormat( ",auditFailures = {0}",      this.auditAccessCheck);
         tmp.AppendFormat( ",auditSystemEvents = {0}",  this.auditSystemEvents ? 1 : 0 );

         //---------------
         // Audit Objects
         //---------------
         tmp.AppendFormat( ",auditDmlAll = {0}",           this.auditDmlAll ? 1 : 0 );
         tmp.AppendFormat( ",auditUserTables = {0}",       this.auditUserTables );
         tmp.AppendFormat( ",auditSystemTables = {0}",     this.auditSystemTables ? 1 : 0 );
         tmp.AppendFormat( ",auditStoredProcedures = {0}", this.auditStoredProcedures ? 1 : 0 );
         tmp.AppendFormat( ",auditDmlOther = {0}",         this.auditDmlOther ? 1 : 0 );

         //------------------------
         // Audit Privileged Users
         //------------------------
         tmp.AppendFormat( ",auditUserAll = {0}",          this.auditUserAll ? 1 : 0  );
         tmp.AppendFormat( ",auditUserLogins = {0}",       this.auditUserLogins ? 1 : 0  );
         tmp.AppendFormat( ",auditUserLogouts = {0}",       this.auditUserLogouts ? 1 : 0  );
         tmp.AppendFormat( ",auditUserFailedLogins = {0}", this.auditUserFailedLogins ? 1 : 0  );
         tmp.AppendFormat( ",auditUserDDL = {0}",          this.auditUserDDL ? 1 : 0  );
         tmp.AppendFormat( ",auditUserAdmin = {0}",        this.auditUserAdmin ? 1 : 0  );
         tmp.AppendFormat( ",auditUserSecurity = {0}",     this.auditUserSecurity ? 1 : 0  );
         tmp.AppendFormat( ",auditUserDML = {0}",          this.auditUserDML ? 1 : 0  );
         tmp.AppendFormat( ",auditUserSELECT = {0}",       this.auditUserSELECT ? 1 : 0  );
         tmp.AppendFormat( ",auditUserFailures = {0}",     this.auditUserAccessCheck);

         //------------
         // LicenseKey
         //------------
         tmp.AppendFormat( ",licenseKey = {0}", SQLHelpers.CreateSafeString(this.licenseKey) );
         
         //---------
         // Archive
         //---------
         tmp.AppendFormat( ",archiveOn = {0}",       this.archiveOn );
         tmp.AppendFormat( ",archiveTimeZoneName = {0}", SQLHelpers.CreateSafeString(this.archiveTimeZoneName) );

         tmp.AppendFormat( ",archiveBias = {0}", this.archiveBias );
         tmp.AppendFormat( ",archiveStandardBias = {0}", this.archiveStandardBias );
         tmp.AppendFormat( ",archiveStandardDate = {0}", SQLHelpers.CreateSafeDateTimeString(this.archiveStandardDate) );
         tmp.AppendFormat( ",archiveDaylightBias = {0}", this.archiveDaylightBias );
         tmp.AppendFormat( ",archiveDaylightDate = {0}", SQLHelpers.CreateSafeDateTimeString(this.archiveDaylightDate) );

         tmp.AppendFormat( ",archiveInterval = {0}", this.archiveInterval );
         tmp.AppendFormat( ",archiveAge = {0}",      this.archiveAge);
         tmp.AppendFormat( ",archivePeriod = {0}",   this.archivePeriod);
         tmp.AppendFormat( ",archivePrefix = {0}",   SQLHelpers.CreateSafeString(this.archivePrefix) );
         tmp.AppendFormat( ",archiveLastTime = {0}", SQLHelpers.CreateSafeDateTimeString(this.archiveLastTime));
         tmp.AppendFormat( ",archiveNextTime = {0}", SQLHelpers.CreateSafeDateTimeString(this.archiveNextTime));
         tmp.AppendFormat( ",archiveCheckIntegrity = {0}", this.archiveCheckIntegrity ? 1 : 0  );
         tmp.AppendFormat( ",archiveDatabaseFilesLocation = {0}", SQLHelpers.CreateSafeString(_archiveDatabaseFilesLocation));
         tmp.AppendFormat( ",archiveScheduleType = {0}", (int)_archiveSchedule);
         tmp.AppendFormat( ",archiveScheduleTime = {0}", SQLHelpers.CreateSafeDateTimeString(_archiveScheduleTime));
         tmp.AppendFormat( ",archiveScheduleRepetition = {0}", _archiveScheduleRepetition);
         tmp.AppendFormat( ",archiveScheduleWeekDay = {0}", SQLHelpers.CreateSafeString(weekDays));
         tmp.AppendFormat( ",archiveScheduleDayOrWeekOfMonth = {0}", _archiveScheduleDayOrWeekOfMonth);
         
         //---------
         // Grooming
         //---------
         tmp.AppendFormat( ",groomLogAge = {0}",     this.groomLogAge );
         tmp.AppendFormat( ",groomAlertAge = {0}",   this.groomAlertAge);
         tmp.AppendFormat( ",groomEventAllow = {0}", this.groomEventAllow ? 1 : 0);
         tmp.AppendFormat( ",groomEventAge = {0}",   this.groomEventAge);

         //----------
         // Snapshot
         //----------
         tmp.AppendFormat( ",snapshotInterval = {0}", this.snapshotInterval);
         tmp.AppendFormat( ",snapshotLastTime = {0}", SQLHelpers.CreateSafeDateTimeString(this.snapshotLastTime));

         //----------------
         // Recovery Model
         //----------------
         tmp.AppendFormat( ",recoveryModel = {0}", this.recoveryModel);

         //----------------
         // Login Filters
         //----------------
         tmp.AppendFormat( ",loginCollapse = {0}", this.loginCollapse ? 1 : 0);
         tmp.AppendFormat( ",loginTimespan = {0}", this.loginTimespan);
         tmp.AppendFormat( ",loginCacheSize = {0}", this.loginCacheSize);

         //--------------------------------------------
         //Heartbeat Interval for the collection server
         //--------------------------------------------
         tmp.AppendFormat(",collectionServerHeartbeatInterval = {0}", this.collectionServerHeartbeatInterval);
         tmp.AppendFormat(",alwaysOnRoleUpdateInterval = {0}", this.alwaysOnRoleUpdateInterval);

         //--------------------------------------------
         //Reindexing schedule
         //--------------------------------------------
         if (this.indexStartTime == DateTime.MinValue)
            tmp.AppendFormat(",indexStartTime = null");
         else
            tmp.AppendFormat(",indexStartTime = '{0}'", this.indexStartTime);

         tmp.AppendFormat(",indexDurationInSeconds = {0}", this.indexDuration.TotalSeconds.ToString());		                     
         return tmp.ToString();
      }
	
		#endregion

      /// <summary>
      /// Gets the Repository Information Table Values keyed on the Name
      /// </summary>
      /// <param name="connectionInfo">Input Connection to the repository</param>
      /// <returns></returns>
	  public static Dictionary<string, RepositoryInfo> GetRepositoryInfo(SqlConnection connectionInfo)
	  {
	      var repositoryInfoMap = new Dictionary<string, RepositoryInfo>();

          if (connectionInfo == null)
	      {
	          throw new ArgumentNullException(SQLcomplianceConfiguration.ConnectionInfo);
	      }
	      try
	      {
	          using (var dataReader = SqlHelper.ExecuteReader(
	              connectionInfo,
	              CommandType.StoredProcedure,
	              GetRepositoryInfoStoredProcedure))
	          {
	              while (dataReader.Read())
	              {
                      var repositoryInfo = new RepositoryInfo();
	                  var nameIndex = dataReader.GetOrdinal("Name");
	                  var internalValueIndex = dataReader.GetOrdinal("Internal_Value");
	                  var characterValueIndex = dataReader.GetOrdinal("Character_Value");

	                  if (!dataReader.IsDBNull(nameIndex))
	                  {
	                      repositoryInfo.Name = dataReader.GetString(nameIndex);
	                  }
	                  if (!dataReader.IsDBNull(internalValueIndex))
	                  {
	                      repositoryInfo.InternalValue = dataReader.GetInt32(internalValueIndex);
	                  }
	                  if (!dataReader.IsDBNull(characterValueIndex))
	                  {
	                      repositoryInfo.CharacterValue = dataReader.GetString(characterValueIndex);
	                  }
	                  if (!repositoryInfoMap.ContainsKey(repositoryInfo.Name))
	                  {
	                      repositoryInfoMap.Add(repositoryInfo.Name, repositoryInfo);
	                  }
	              }
	          }
	          return repositoryInfoMap;
            }
            catch (SqlException e)
	      {
	          // Assuming that a valid connection can be established to the SQL Server, 
	          // an invalid call to the version procedure would indicate an invalid database;
	          // all other exceptions will be passed on.
	          //
	          // Error 2812 = cannot find stored procedure in SQL Server 2000 & 2005
	          //
	          if (e.Number == 2812)
	          {
	              return null;
	          }
	          else
	          {
	              throw;
	          }
	      }
	  }

      /// <summary>
      /// Insert the values in the RepositoryInfo table
      /// </summary>
      /// <param name="name">Name column - max 64 size and not null</param>
      /// <param name="internalValue">Internal value indicates - 0 and 1 mostly</param>
      /// <param name="characterValue">Character Value</param>
	  public static void SetRepositoryInfo(string name, int internalValue = 1, string characterValue = null)
	  {
	      Repository rep = new Repository();

	      try
	      {
	          rep.OpenConnection();

	          if (rep.connection == null)
	              throw new ArgumentNullException(NoConnection);

              using (SqlCommand cmd = SqlHelper.CreateCommand(rep.connection, InsertRepositoryInfoStoredProcedure))
	          {
	              SqlHelper.AssignParameterValues(cmd.Parameters, name, internalValue, characterValue);
                  cmd.ExecuteNonQuery();
	          }
	      }
	      finally
	      {
	          rep.CloseConnection();
	      }
      }
	}
}
