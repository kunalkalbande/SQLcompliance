using System;
using System.Collections.Generic ;
using System.Data.SqlClient;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using Idera.SQLcompliance.Core.Event ;

namespace Idera.SQLcompliance.Core
{
	/// <summary>
	/// Summary description for DatabaseRecord.
	/// </summary>
    [Serializable]
	[DataContract(Name = "Database")]
	public class DatabaseRecord
	{
	   #region Constructor
	   
		public DatabaseRecord()
		{
		  name        = "";
		  description = "";
		  dbId        = -1;
		  srvId       = -1;
          auditPrivUsersList = "";
            
         auditUserExceptions = false;

        // we dont support setting this field for in 2.0 - setting default to OFF
        auditBroker = false;
		}
		
		#endregion
		
		#region Private Properties
		
		private SqlConnection     conn;
		
		private int      dbId;
		private int      srvId;
		private string   srvInstance;
		private int      sqlDatabaseId;
		private string   name;
		private string   description;
		private bool     isEnabled;
		private bool     isSqlSecureDb;
        bool             isAlwaysOn;
        bool             isPrimary;
        private string   replicaServers;
        private string   availGroupName;
		
		private DateTime timeCreated;
		private DateTime timeLastModified;
		private DateTime timeEnabledModified;

      // Audit Settings		
		private bool    auditDDL;
		private bool    auditSecurity;
		private bool    auditAdmin;
		private bool    auditBroker;
		private bool    auditDML;
		private bool    auditSELECT;
		private int     auditAccessCheck ;
		private bool    auditCaptureSQL;
      private bool    auditCaptureTrans;
		private bool    auditExceptions;
      private string  auditUsersList;
      private bool    pci;
      private bool    hipaa;
        private bool disa;
        private bool nerc;
        private bool cis;
        private bool sox;
        private bool ferpa;
        private bool gdpr;
		
		// DML Filtering
		private bool    auditDmlAll;
		private int     auditUserTables;
		private bool    auditSystemTables;
		private bool    auditStoredProcedures;
		private bool    auditDmlOther;
		
		private bool    auditDataChanges;
      private bool    auditSensitiveColumns;
        private int auditSensitiveColumnActivity;
        private int auditSensitiveColumnActivityDataset;
      // Users
      private string auditPrivUsersList;
      private bool auditUserAll;
      private bool auditUserLogins;
	  private bool auditUserLogouts; // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
        private bool auditUserFailedLogins;
      private bool auditUserDDL;
      private bool auditUserSecurity;
      private bool auditUserAdmin;
      private bool auditUserDML;
      private bool auditUserSELECT;
      private int auditUserAccessCheck;
      private bool auditUserCaptureSQL;
      private bool auditUserCaptureTrans;
      private bool auditUserExceptions;
      private bool auditUserUDE;
		
      private bool auditUserCaptureDDL;
      private bool auditCaptureDDL;
		#endregion
		
		#region Public Properties
		
		public SqlConnection     Connection
		{
		   get { return conn;  }
		   set { conn = value; }
		}

        public int AuditSensitiveColumnActivity
        {
            get
            {
                return auditSensitiveColumnActivity;
            }
            set
            {
                auditSensitiveColumnActivity = value;
            }
        }
        public int AuditSensitiveColumnActivityDataset
        {
            get
            {
                return auditSensitiveColumnActivityDataset;
            }
            set
            {
                auditSensitiveColumnActivityDataset = value;
            }
        }

        public int     DbId
		{
		   get { return dbId;  }
		   set { dbId = value; }
		}

		public int     SrvId
		{
		   get { return srvId;  }
		   set { srvId = value; }
		}

		public string  SrvInstance
		{
		   get { return srvInstance;  }
		   set { srvInstance = value; }
		}

		public int     SqlDatabaseId
		{
		   get { return sqlDatabaseId;  }
		   set { sqlDatabaseId = value; }
		}

		public string  Name
		{
		   get { return name;  }
		   set { name = value; }
		}

		public string  Description
		{
		   get { return description;  }
		   set { description = value; }
		}

		public bool    IsEnabled
		{
		   get { return isEnabled;  }
		   set { isEnabled = value; }
		}

		public DateTime    TimeCreated
		{
		   get { return timeCreated;  }
		   set { timeCreated = value; }
		}

		public DateTime    TimeLastModified
		{
		   get { return timeLastModified;  }
		   set { timeLastModified = value; }
		}

		public DateTime    TimeEnabledModified
		{
		   get { return timeEnabledModified;  }
		   set { timeEnabledModified = value; }
		}

		public bool    IsSqlSecureDb
		{
		   get { return isSqlSecureDb;  }
		   set { isSqlSecureDb = value; }
		}

        public bool IsAlwaysOn
        {
            get { return isAlwaysOn; }
            set { isAlwaysOn = value; }
        }

        public bool IsPrimary
        {
            get { return isPrimary; }
            set { isPrimary = value; }
        }

        public string ReplicaServers
        {
            get { return replicaServers; }
            set { replicaServers = value; }
        }
        public string AvailGroupName
        {
            get { return availGroupName; }
            set { availGroupName = value; }
        }

		
      // Audit Settings	
		public bool    AuditDDL
		{
		   get { return auditDDL;  }
		   set { auditDDL = value; }
		}

		public bool    AuditSecurity
		{
		   get { return auditSecurity;  }
		   set { auditSecurity = value; }
		}

		public bool    AuditAdmin
		{
		   get { return auditAdmin;  }
		   set { auditAdmin = value; }
		}

		public bool    AuditBroker
		{
		   get { return auditBroker;  }
		   set { auditBroker = value; }
		}

		public bool    AuditDML
		{
		   get { return auditDML;  }
		   set { auditDML = value; }
		}

		public bool    AuditSELECT
		{
		   get { return auditSELECT;  }
		   set { auditSELECT = value; }
		}

		public AccessCheckFilter    AuditAccessCheck
		{
		   get { return (AccessCheckFilter)auditAccessCheck;  }
		   set { auditAccessCheck = (int)value; }
		}

		public bool    AuditCaptureSQL
		{
		   get { return auditCaptureSQL;  }
		   set { auditCaptureSQL = value; }
		}

      public bool AuditCaptureTrans
      {
         get { return auditCaptureTrans; }
         set { auditCaptureTrans = value; }
      }

		public bool    AuditExceptions
		{
		   get { return auditExceptions;  }
		   set { auditExceptions = value; }
		}

      public string AuditUsersList
      {
         get { return auditUsersList ; }
         set { auditUsersList = value ; }
      }

      public bool PCI
      {
         get { return pci; }
         set { pci = value; }
      }

      public bool HIPAA
      {
         get { return hipaa; }
         set { hipaa = value; }
      }
      public bool DISA
      {
          get { return disa; }
          set { disa = value; }
      }
      public bool NERC
      {
          get { return nerc; }
          set { nerc = value; }
      }
      public bool CIS
      {
          get { return cis; }
          set { cis = value; }
      }
      public bool SOX
      {
          get { return sox; }
          set { sox = value; }
      }
      public bool FERPA
      {
          get { return ferpa; }
          set { ferpa = value; }
      }
        
      public bool GDPR
      {
          get { return gdpr; }
          set { gdpr = value; }
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

		public bool    AuditViews
		{
		   get { return false;  }
		}

		public bool    AuditIndexes
		{
		   get { return false;  }
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
		public bool AuditDataChanges
		{
		   get
		   {
		      return auditDataChanges;
		   }
		   set
		   {
		      auditDataChanges = value;
		   }
	   }

      public bool AuditSensitiveColumns
      {
         get { return auditSensitiveColumns; }
         set { auditSensitiveColumns = value; }
      }
      
      // Users
      public string AuditPrivUsersList
      {
          get { return auditPrivUsersList; }
          set { auditPrivUsersList = value; }
      }
      public bool AuditUserAll
      {
          get { return auditUserAll; }
          set { auditUserAll = value; }
      }
      public bool AuditUserLogins
      {
          get { return auditUserLogins; }
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
      public bool AuditUserFailedLogins
      {
          get { return auditUserFailedLogins; }
          set { auditUserFailedLogins = value; }
      }
      public bool AuditUserDDL
      {
          get { return auditUserDDL; }
          set { auditUserDDL = value; }
      }
      public bool AuditUserSecurity
      {
          get { return auditUserSecurity; }
          set { auditUserSecurity = value; }
      }
      public bool AuditUserAdmin
      {
          get { return auditUserAdmin; }
          set { auditUserAdmin = value; }
      }
      public bool AuditUserDML
      {
          get { return auditUserDML; }
          set { auditUserDML = value; }
      }
      public bool AuditUserSELECT
      {
          get { return auditUserSELECT; }
          set { auditUserSELECT = value; }
      }
      public AccessCheckFilter AuditUserAccessCheck
      {
          get { return (AccessCheckFilter)auditUserAccessCheck; }
          set { auditUserAccessCheck = (int)value; }
      }
      public bool AuditUserCaptureSQL
      {
          get { return auditUserCaptureSQL; }
          set { auditUserCaptureSQL = value; }
      }
      public bool AuditUserCaptureTrans
      {
          get { return auditUserCaptureTrans; }
          set { auditUserCaptureTrans = value; }
      }
      public bool AuditUserExceptions
      {
          get { return auditUserExceptions; }
          set { auditUserExceptions = value; }
      }
      public bool AuditUserUDE
      {
          get { return auditUserUDE; }
          set { auditUserUDE = value; }
      }
      public bool AuditUserCaptureDDL
      {
          get { return auditUserCaptureDDL;; }
          set { auditUserCaptureDDL = value; }
      }
        
      public bool AuditCaptureDDL
      {
          get { return auditCaptureDDL; }
          set { auditCaptureDDL = value; }
      }
      
      
	   #endregion
		
		#region LastError
		
	   static string           errMsg   = "";
	   static public  string   GetLastError() { return errMsg; } 
	   
	   #endregion
		
      #region Public Methods - Read/Write/Delete

      //-------------------------------------------------------------------
      // Clone
      //-------------------------------------------------------------------
      public DatabaseRecord 
         Clone()
      {
         DatabaseRecord db = (DatabaseRecord)this.MemberwiseClone();
         
         return db;
      }

	   //-----------------------------------------------------------------------------
	   // Read - reads database record given server name and database name
	   //-----------------------------------------------------------------------------
	   public bool
	      Read(
	         int               dbId
         )
	   {
         string where = String.Format( "d.dbId={0}", dbId );
			return InternalRead( where );
      }

	   //-----------------------------------------------------------------------------
	   // Read - reads database record given server name and database name
	   //-----------------------------------------------------------------------------
	   public bool
	      Read(
	         string            instance,
	         string            dbName
         )
	   {
         string where = String.Format( "s.instance={0} AND d.name={1}",
	                                    SQLHelpers.CreateSafeString(instance),
			                              SQLHelpers.CreateSafeString(dbName) );
			return InternalRead( where );
      }

	   //-----------------------------------------------------------------------------
	   // Read - reads database record given server id and database name
	   //-----------------------------------------------------------------------------
	   public bool
	      Read(
	         int               srvId,
	         string            dbName
         )
	   {
	      string where = String.Format( "s.srvId={0} AND d.name={1}",
			                              srvId,
			                              SQLHelpers.CreateSafeString(dbName) );
			return InternalRead( where );
	   }


       //--------------------// SQLCM-5.4 SCM-2174 Start-----------------------------------------
       // GetDatabaseNameSQL - searching with server id  
       //--------------------------------------------------------------
        public IList
	      GetDatabaseList(
            int  serverId
	      )
	   {
   		IList appDbList = null;			   
			try
			{
			   string cmdstr = GetDatabaseNameSQL(serverId);

			   using ( SqlCommand    cmd    = new SqlCommand( cmdstr,conn ) )
			   {
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
                   appDbList = new ArrayList();
				  while ( reader.Read() )
                  {
                      var dbName = reader.GetString(0);
                      appDbList.Add(dbName);
                  }
               }
            }
			}
			catch( Exception ex )
			{
				Debug.Write( String.Format("Error loading list: {0}", ex.Message ) );
			}
			
			return appDbList;
	   }

       private static string
          GetDatabaseNameSQL(
            int srvId)
       {
           //SCM-705
           string tmp = "SELECT d.name as dbName" +
                              " FROM {0} AS d, {1} as s " +
                              " WHERE d.srvId=s.srvId AND s.srvId={2} AND d.name NOT IN('master','model','tempdb','msdb','mssqlsystemresource')";

           return string.Format(tmp,
                                 CoreConstants.RepositoryDatabaseTable,
                                 CoreConstants.RepositoryServerTable,
                                 srvId);
       }

        //--------------------// SQLCM-5.4 SCM-2174 end-----------------------------------------

	   //-------------------------------------------------------------------
	   // Create - Create new database record in repository
	   //-------------------------------------------------------------------
	   public bool
	      Create(
	         SqlTransaction    transaction
	      )
	   {
	      bool retval;
	      
		   this.timeCreated         = DateTime.UtcNow;
		   this.timeLastModified    = this.timeCreated;
		   this.timeEnabledModified = this.timeCreated;
	      
	      retval = InternalWrite( true /* create */,
	                              null,
	                              transaction );
	      return retval;
	   }

       //-------------------------------------------------------------------
       // Create - Create new database record in repository
       //-------------------------------------------------------------------
       public bool
          CopyPrimaryRecord(
           bool isPrimary, 
           int srvId, 
           string srvInstance, 
           string avgName,
           SqlConnection conn
          )
       {
           bool retval;

           retval = InternalCopy(isPrimary, srvId, srvInstance, avgName,
                                   conn);
           return retval;
       }

       public List<DatabaseRecord> 
           GetSecondaryDatabaseRecords(
           int srvId, SqlConnection conn
           )
       {
           List<DatabaseRecord> dbList= new List<DatabaseRecord>();

           try 
           {
               SQLHelpers.CheckConnection(conn);
               StringBuilder sqlCmd = new StringBuilder(""); 

               string cmdstr = GetSecondarySelectSQL(srvId);

               using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
               {
                   using (SqlDataReader reader = cmd.ExecuteReader())
                   {                       
                       while (reader.Read())
                       {
                           DatabaseRecord db = new DatabaseRecord();
                           db.Load(reader);

                           // Add to list               
                           dbList.Add(db);
                       }
                   }
               }

           }

           catch (Exception ex)
           {
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                        "GetSecondaryDatabaseRecords",
                                        ex);
               Debug.Write(String.Format("Error loading list: {0}", ex.Message));
               errMsg = ex.Message;
               
           }

           return dbList;       
       }
   

	   
	   //-------------------------------------------------------------------
	   // Write - Updates existing database record
	   //-------------------------------------------------------------------
	   public bool
	      Write()
	   {
		   this.timeLastModified = DateTime.UtcNow;
		   
	      return InternalWrite( false /* dont create */,
	                            null,
	                            null );
	   }   

	   //-------------------------------------------------------------------
	   // Write - Updates existing database record
	   //-------------------------------------------------------------------
	   public bool
	      Write(
	         DatabaseRecord    oldDbRec
	      )
	   {
		   this.timeLastModified = DateTime.UtcNow;
		   
	      return InternalWrite( false /* dont create */,
	                            oldDbRec,
	                            null );
	   }

        //-------------------------------------------------------------------
        ////v5.6 SQLCM-5373
        //-------------------------------------------------------------------
        public bool
          Update(
             DatabaseRecord oldDbRec,
             SqlTransaction transaction
         )
       {
           this.timeLastModified = DateTime.UtcNow;

           return InternalUpdate(oldDbRec,
                                 transaction);
       }   
	   

	   //-------------------------------------------------------------------
	   // Write - Updates existing database record
	   //-------------------------------------------------------------------
	   public bool
	      Write(
	         DatabaseRecord    oldDbRec,
	         SqlTransaction    transaction
         )
	   {
		   this.timeLastModified = DateTime.UtcNow;
		   
	      return InternalWrite( false /* dont create */,
	                            oldDbRec,
	                            transaction );
	   }   
	   
	   //-------------------------------------------------------------------
	   // Enable - Enable/Disable Auditing of Database
	   //-------------------------------------------------------------------
      public bool
         Enable(
            bool              enableAuditing
         )	   
      {
	      bool retval = true;
	      
	      string sqlCmd = GetEnableSQL( enableAuditing );

         try
         {
		      using ( SqlCommand  cmd = new SqlCommand( sqlCmd,conn ) )
		      {
               int nRows = cmd.ExecuteNonQuery();
               if ( nRows == 1 )
               {
                  // Update Database Record
	               this.IsEnabled = enableAuditing;
		            this.timeEnabledModified = DateTime.UtcNow;
               
                  retval = true;
               }
            }
         }
         catch( Exception ex )
         {
           errMsg = ex.Message;
         }
                             
         return retval;
      }
	   
      //-------------------------------------------------------------------
      // Delete
      //
      // Note: Associated tables are deleted in the GUI logic and not down
      //       here as part of the delete logic - hence the transactiojn 
      //       passed in
      //-------------------------------------------------------------------
      public bool
         Delete(
            SqlTransaction    transaction
         )
      {
	      bool retval = false;
	      
	      string sqlCmd = GetDeleteSQL();

         try
         {
		      using ( SqlCommand  cmd = new SqlCommand( sqlCmd, conn ) )
		      {
               if ( transaction != null )
               {
                  cmd.Transaction = transaction;
               }
               		                                        
               int nRows = cmd.ExecuteNonQuery();
               if ( nRows == 1 )
               {
                  retval = true;
               }
            }
         }
         catch( Exception ex )
         {
           errMsg = ex.Message;
         }
                             
         return retval;
      }


      //-------------------------------------------------------------
      //    SQLCM-5.4 SCM-2174 GetDeleteSQL - Create SQL to delete a database
      //--------------------------------------------------------------
      public void
         GetDeleteSQL(string instanceName, string dbName)
      {
          string cmdStr = String.Format("DELETE FROM {0} where srvInstance = {1} and name = {2}",
                                         CoreConstants.RepositoryDatabaseTable,
                                         SQLHelpers.CreateSafeString(instanceName),
                                          SQLHelpers.CreateSafeString(dbName));

          try
          {
              using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
              {
                    cmd.ExecuteNonQuery();  
              }
          }
          catch (Exception ex)
          {
              errMsg = ex.Message;
          }
          
      }
      //    SQLCM-5.4 SCM-2174 

      public void ResetAuditSettings()
      {
         // Audit Settings		
		   auditDDL = false;
		   auditSecurity = false;
         auditAdmin = false;
         auditBroker = false;
         auditDML = false;
         auditSELECT = false;
         auditAccessCheck = (int)AccessCheckFilter.NoFilter;
         auditCaptureSQL = false;
         auditCaptureTrans = false;
         auditExceptions = false;
         auditDataChanges = false;
         auditSensitiveColumns = false;
         auditUsersList = "";
         pci = false;
         hipaa = false;
         disa = false;
         nerc = false;
         cis = false;
         sox = false;
         ferpa = false;
         gdpr = false;
		   // DML Filtering
         auditDmlAll = false;
         auditUserTables=0;
         auditSystemTables = false;
         auditStoredProcedures = false;
         auditDmlOther = false;
         auditSensitiveColumnActivity = 0;
         auditSensitiveColumnActivityDataset = 0;
      }

      public void ResetPrivilegedUsersAuditSettings()
      {
         auditUserAll = false;
         auditUserLogins = false;
         auditUserLogouts = false;
         auditUserFailedLogins = false;
         auditUserDDL = false;
         // SQLCM-6019 : SQLCM-4718: Import/Export Audit Settings - Capture DDL data is not getting imported to the other database
         auditUserCaptureDDL = false;
         auditUserSecurity = false;
         auditUserAdmin = false;
         auditUserDML = false;
         auditUserSELECT = false;
         auditUserExceptions = false;
         auditUserUDE = false;

          auditPrivUsersList = "";
         auditUserAccessCheck = (int)AccessCheckFilter.NoFilter;
         auditUserCaptureSQL = false;
          auditUserCaptureDDL = false;
         auditUserCaptureTrans = false;
         auditSensitiveColumnActivity = (int)SensitiveColumnActivity.SelectOnly;  // SQLCM-5471 v5.6
         auditSensitiveColumnActivityDataset = (int)SensitiveColumnActivity.SelectOnly;
      }

      public List<AuditCategory> GetAuditCategoryList()
      {
          List<AuditCategory> categoryList = new List<AuditCategory>();

          if (AuditDDL)
          {
              categoryList.Add(AuditCategory.DDL);
          }

          if (AuditSecurity)
          {
              categoryList.Add(AuditCategory.Security);
          }

          if (AuditAdmin)
          {
              categoryList.Add(AuditCategory.Admin);
          }

          if (AuditSELECT)
          {
              categoryList.Add(AuditCategory.SELECT);
          }

          if (AuditDML)
          {
              categoryList.Add(AuditCategory.DML);
          }

          if (AuditDataChanges)
          {
              categoryList.Add(AuditCategory.UDC);
          }

          return categoryList;
      }

      public List<AuditCategory> GetUserAuditCategoryList()
      {
          List<AuditCategory> categoryList = new List<AuditCategory>();

          if (auditUserAll)
          {
              categoryList.Add(AuditCategory.Logins);
              categoryList.Add(AuditCategory.DDL);
              categoryList.Add(AuditCategory.Security);
              categoryList.Add(AuditCategory.Admin);
              categoryList.Add(AuditCategory.FailedLogins);
              categoryList.Add(AuditCategory.SELECT);
              categoryList.Add(AuditCategory.DML);
              categoryList.Add(AuditCategory.UDC);
              categoryList.Add(AuditCategory.Logouts);
          }
            else
          {
          if (AuditUserLogins)
          {
              categoryList.Add(AuditCategory.Logins);
          }

          // Add Logouts Category for Logouts
          if (AuditUserLogouts)
          {
              categoryList.Add(AuditCategory.Logouts);
          }

          if (AuditUserDDL)
          {
              categoryList.Add(AuditCategory.DDL);
          }

          if (AuditUserSecurity)
          {
              categoryList.Add(AuditCategory.Security);
          }

          if (AuditUserAdmin)
          {
              categoryList.Add(AuditCategory.Admin);
          }

          if (AuditUserFailedLogins)
          {
              categoryList.Add(AuditCategory.FailedLogins);
          }

          if (AuditUserSELECT)
          {
              categoryList.Add(AuditCategory.SELECT);
          }

          if (AuditUserDML)
          {
              categoryList.Add(AuditCategory.DML);
          }

          if (AuditUserUDE)
          {
              categoryList.Add(AuditCategory.UDC);
          }
          }

          return categoryList;
      }

	   #endregion
	   
	   #region static Public Routines

	   //-----------------------------------------------------------------------------
	   // GetDatabases - Workhorse routine for all GetDatabase* routines - takes the
	   //                WHERE clause as input to filter results
	   //-----------------------------------------------------------------------------
	   static public ICollection
	      GetDatabases(
	         SqlConnection    conn,
	         string           whereClause
         )
	   {
   		IList dbList     = null;
         
         // Load Databases			   
			try
			{
			   string cmdstr = GetSelectSQL( whereClause,
			                                 "ORDER by s.instance ASC,d.name ASC" );

			   using ( SqlCommand    cmd    = new SqlCommand( cmdstr,conn ) )
			   {
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
				      dbList = new ArrayList();

			         while ( reader.Read() )
                  {
                     DatabaseRecord db = new DatabaseRecord();
                     db.Load( reader );

                     // Add to list               
                     dbList.Add( db );
                  }
               }
            }
			}
			catch( Exception ex )
			{
				Debug.Write( String.Format("Error loading list: {0}", ex.Message ) );
				dbList = new ArrayList();  // return an empty array
			}
			
			return dbList;
	   }

        //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
        //Get count of all databases in a server
        public static int GetCountDatabasesWithEnabledAuditDML(SqlConnection conn, int srvId)
        {
            int countDatabases = 0;
            string query = "";

            try
            {
                SQLHelpers.CheckConnection(conn);

                //Count only databases whose are available for auditing and whose audit DML is enabled
                query = GetCountDatabasesSQL(srvId) + " AND (auditDML = 1 OR auditSELECT = 1)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            countDatabases = reader.GetInt32(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "GetCountDatabases", query, ex);
                errMsg = ex.Message;
            }

            return countDatabases;
        }

        //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
        //Update(Increment/Decrement) countDatabasesAuditingAllObjects on the basis of 'actionType'
        //True -> Increment, False -> Decrement
        public static void UpdateCountDatabasesAuditingAllObjects(SqlConnection conn, int srvId, int dbId, bool actionType)
        {
            string query = "";
            bool isDatabaseEnabledForAuditDMLAll = false;

            try
            {
                SQLHelpers.CheckConnection(conn);

                query = GetAuditDMLAllSQL(srvId, dbId);
                
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int temp = reader.GetInt32(0);
                            isDatabaseEnabledForAuditDMLAll = (temp > 0)? true : false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "UpdateCountDatabasesAuditingAllObjects", query, ex);
                errMsg = ex.Message;
            }

            //Update only if auditDMLAll is enabled
            if (isDatabaseEnabledForAuditDMLAll)
            {
                if (actionType)
                {
                    Agent.ServerRecord.IncrementCountDatabasesAuditingAllObjects(conn, srvId);
                }
                else
                {
                    Agent.ServerRecord.DecrementCountDatabasesAuditingAllObjects(conn, srvId);
                    
                }
            }
        }

        //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
        //Update auditDmlAll
        public static void SetAuditDMLAll(SqlConnection conn, int srvId, int dbId, int newValue)
        {
            string query = "";

            try
            {
                SQLHelpers.CheckConnection(conn);

                query = GetUpdateAuditDMLAllSQL(srvId, dbId, newValue);

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "SetAuditDMLAll", query, ex);
                errMsg = ex.Message;
            }
        }
        
        //-----------------------------------------------------------------------------
        // GetDatabases - Workhorse routine for all GetDatabase* routines - takes the
        //                WHERE clause as input to filter results
        //-----------------------------------------------------------------------------
        static public List<DatabaseRecord> GetDatabases(SqlConnection conn, int srvId, SqlTransaction transaction = null)
      {
         List<DatabaseRecord> dbList = new List<DatabaseRecord>();

         // Load Databases			   
         try
         {
            string cmdstr = GetSelectSQL(String.Format("s.srvId={0}",srvId),
                                          "ORDER by s.instance ASC,d.name ASC");

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    if (transaction != null)
                        cmd.Transaction = transaction;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DatabaseRecord db = new DatabaseRecord();
                            db.Load(reader);

                            // Add to list               
                            dbList.Add(db);
                        }
                    }
                }
         }
         catch (Exception ex)
         {
            Debug.Write(String.Format("Error loading list: {0}", ex.Message));
         }

         return dbList;
      }

	   //-----------------------------------------------------------------------------
	   // GetAuditedDatabaseCount
	   //-----------------------------------------------------------------------------
	   static public int
	      GetAuditedDatabaseCount(
	         SqlConnection    conn
         )
	   {
   		int count = 0;
         
			try
			{
			   string cmdstr = "SELECT count(*) from {0} as s,{1} as d " +
			                   "WHERE s.isAuditedServer=1 " +
			                      "AND s.isEnabled=1 " +
			                      "AND d.isEnabled=1 " +
			                      "AND s.srvId=d.srvId";
            cmdstr = String.Format( cmdstr,
                                    CoreConstants.RepositoryServerTable,
                                    CoreConstants.RepositoryDatabaseTable );          

			   using ( SqlCommand    cmd    = new SqlCommand( cmdstr,conn ) )
			   {
				   object obj = cmd.ExecuteScalar();
               if( obj is System.DBNull )
                  count = 0;
               else
                  count = (int)obj;
            }
			}
			catch( Exception ex )
			{
				Debug.Write( String.Format("Error counting dbs: {0}", ex.Message ) );
			}
			
			return count;
	   }
	   
      //-----------------------------------------------------------------------------
      // GetAuditedDatabaseCount
      //-----------------------------------------------------------------------------
      static public int GetAuditedDatabaseCount(SqlConnection conn, int srvId)
      {
         int count = 0;

         try
         {
            string cmdstr = "SELECT count(*) from {0}  " +
                            "WHERE isEnabled=1 " +
                               "AND srvId={1}";
            cmdstr = String.Format(cmdstr,
                                    CoreConstants.RepositoryDatabaseTable,
                                    srvId);

            using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
            {
               object obj = cmd.ExecuteScalar();
               if (obj is System.DBNull)
                  count = 0;
               else
                  count = (int)obj;
            }
         }
         catch (Exception ex)
         {
            Debug.Write(String.Format("Error counting dbs for srvId {0}: {1}", srvId, ex.Message));
         }
	   
         return count;
      }	   
      //-------------------------------------------------------------
      // Match - Decides if two DatabaseRecords match
      //         Used by properties dialog to decide if write is needed
      //--------------------------------------------------------------
	   public static bool
	     Match(
	        DatabaseRecord     db1,
	        DatabaseRecord     db2
	     )
      {
         if ( db1==null || db2== null )
            return false;
            
         // General Properties            
         if ( db2.name != db1.name ) return false;
         if ( db2.description != db1.description ) return false;
         if ( db2.isEnabled != db1.isEnabled ) return false;

         // Audit Settings
         if ( db2.auditDDL != db1.auditDDL) return false;
         if ( db2.auditSecurity != db1.auditSecurity) return false;
         if ( db2.auditAdmin != db1.auditAdmin) return false;
         if ( db2.auditBroker != db1.auditBroker) return false;
         if ( db2.auditDML != db1.auditDML) return false;
         if ( db2.auditSELECT != db1.auditSELECT) return false;
         if ( db2.auditAccessCheck != db1.auditAccessCheck) return false;
         if ( db2.auditCaptureSQL != db1.auditCaptureSQL) return false;
         if (db2.auditCaptureTrans != db1.auditCaptureTrans) return false;
         if ( db2.auditExceptions != db1.auditExceptions) return false;
         if (db2.PCI != db1.PCI) return false;
         if (db2.HIPAA != db1.HIPAA) return false;
         if (db2.DISA != db1.DISA) return false;
         if (db2.NERC != db1.NERC) return false;
         if (db2.CIS != db1.CIS) return false;
         if (db2.SOX != db1.SOX) return false;
         if (db2.FERPA != db1.FERPA) return false;
         if (db2.GDPR != db1.GDPR) return false;

         // Compare this way to propertly detect "" and serialized empty lists.
         if (!UserList.Match(db2.auditUsersList, db1.auditUsersList)) return false ;
         //if (db2.auditUsersList != db1.auditUsersList) return false;

         // DMl Filtering
         if ( db2.auditDmlAll != db1.auditDmlAll ) return false;
         if ( ! db2.AuditDmlAll )
         {
            if ( db2.auditUserTables       != db1.auditUserTables ) return false;
            if ( db2.auditSystemTables     != db1.auditSystemTables ) return false;
            if ( db2.auditStoredProcedures != db1.auditStoredProcedures ) return false;
            if ( db2.auditDmlOther         != db1.auditDmlOther ) return false;
         }
         if( db2.auditDataChanges != db1.auditDataChanges ) return false;
         if (db2.auditSensitiveColumns != db1.auditSensitiveColumns) return false;
         
         // Privileged User Auditing
         if (!UserList.Match(db2.auditPrivUsersList , db1.auditPrivUsersList)) return false;
         if (db2.auditUserAll != db1.auditUserAll) return false;
         if (db2.auditUserLogins != db1.auditUserLogins) return false;
         if (db2.auditUserLogouts != db1.auditUserLogouts) return false;
         if (db2.auditUserFailedLogins != db1.auditUserFailedLogins) return false;
         if (db2.auditUserDDL != db1.auditUserDDL) return false;
         if (db2.auditCaptureDDL != db1.auditCaptureDDL) return false;
         if (db2.auditUserSecurity != db1.auditUserSecurity) return false;
         if (db2.auditUserAdmin != db1.auditUserAdmin) return false;
         if (db2.auditUserDML != db1.auditUserDML) return false;
         if (db2.auditUserSELECT != db1.auditUserSELECT) return false;
         if (db2.auditUserAccessCheck != db1.auditUserAccessCheck) return false;
         if (db2.auditUserCaptureSQL != db1.auditUserCaptureSQL) return false;
         if (db2.auditUserCaptureDDL != db1.auditUserCaptureDDL) return false;
	     if (db2.auditUserCaptureTrans != db1.auditUserCaptureTrans) return false;
	     if (db2.auditUserExceptions != db1.auditUserExceptions) return false;
	     if (db2.auditUserUDE != db1.auditUserUDE) return false;
         if (db2.auditSensitiveColumnActivity != db1.auditSensitiveColumnActivity) return false;
         if (db2.auditSensitiveColumnActivityDataset != db1.auditSensitiveColumnActivityDataset) return false;
         // if we got here nothing changed
         return true;
	   }
	   
      //-------------------------------------------------------------
      // GetDatabaseId
      //--------------------------------------------------------------
	   public static int
	      GetDatabaseId(
            SqlConnection     conn,
            int               srvId,
            string            dbName
         )
      {
         int dbId = -1;
         
			try
			{
			   string cmdstr = GetDatabaseIdSQL( srvId, dbName ); 

			   using ( SqlCommand    cmd    = new SqlCommand( cmdstr,conn ) )
			   {
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
			         if ( reader.Read() )
			         {
			            dbId = reader.GetInt32(0);
			         }
			      }
			   }
			}
			catch( Exception)
			{
			}
         
         return dbId;
      }
      
      //-------------------------------------------------------------
      // GetDatabaseId
      //--------------------------------------------------------------
	   public static int
	      GetDatabaseId(
            SqlConnection     conn,
            string            srvName,
            string            dbName
         )
      {
         int dbId = -1;
         
			try
			{
			   string cmdstr = GetDatabaseIdSQL( srvName, dbName ); 

			   using ( SqlCommand    cmd    = new SqlCommand( cmdstr,conn ) )
			   {
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
			         if ( reader.Read() )
			         {
			            dbId = reader.GetInt32(0);
			         }
			      }
			   }
			}
			catch( Exception)
			{
			}
         
         return dbId;
      }
      
	   
       public static DatabaseRecord GetDatabaseRecord(int databaseId, SqlConnection connection)
       {
           var database = new DatabaseRecord();
           database.Connection = connection;
           database.Read(databaseId);
           return database;
       }

       public static DatabaseRecord GetDatabaseRecord(int databaseId, string connectionString)
       {
           DatabaseRecord database = null;

           using(var connection = new SqlConnection(connectionString))
           {
               connection.Open();
               database = GetDatabaseRecord(databaseId, connection);
           }

           return database;
       }
	   
	   #endregion
     
      #region Private Methods
      
      //-------------------------------------------------------------
      // Load - Loads DatabaseRecord from SELECT result set
      //--------------------------------------------------------------
      private void
         Load(
            SqlDataReader reader
         )
      {
         int col =0 ;
         
         dbId                 = reader.GetInt32(col++);
         srvId                = reader.GetInt32(col++);
         srvInstance          = SQLHelpers.GetString( reader, col++);
         
         // General
         name                 = SQLHelpers.GetString( reader, col++);
         description          = SQLHelpers.GetString( reader, col++);
         isEnabled            = SQLHelpers.ByteToBool( reader, col++);
         timeCreated          = SQLHelpers.GetDateTime( reader, col++);
         timeLastModified     = SQLHelpers.GetDateTime( reader, col++);
         timeEnabledModified  = SQLHelpers.GetDateTime( reader, col++);
         isSqlSecureDb        = SQLHelpers.ByteToBool( reader, col++);
         isAlwaysOn           = SQLHelpers.ByteToBool(reader, col++);
         isPrimary            = SQLHelpers.ByteToBool(reader, col++);
         replicaServers       = SQLHelpers.GetString(reader, col++);
         availGroupName       = SQLHelpers.GetString(reader, col++);
         
         // Audit Settings
         auditDDL             = SQLHelpers.ByteToBool( reader, col++);
         auditSecurity        = SQLHelpers.ByteToBool( reader, col++);
         auditAdmin           = SQLHelpers.ByteToBool( reader, col++);
         auditBroker          = SQLHelpers.ByteToBool( reader, col++);
         auditDML             = SQLHelpers.ByteToBool( reader, col++);
         auditSELECT          = SQLHelpers.ByteToBool( reader, col++);
         auditAccessCheck     = SQLHelpers.ByteToInt( reader, col++);
         auditCaptureSQL      = SQLHelpers.ByteToBool( reader, col++);
         auditExceptions      = SQLHelpers.ByteToBool( reader, col++);
         auditUsersList       = SQLHelpers.GetString(reader, col++);
         pci                  = SQLHelpers.ByteToBool(reader, col++);
         hipaa                = SQLHelpers.ByteToBool(reader, col++);
         disa                 = SQLHelpers.ByteToBool(reader, col++);
         nerc                 = SQLHelpers.ByteToBool(reader, col++);
         cis                  = SQLHelpers.ByteToBool(reader, col++);
         sox                  = SQLHelpers.ByteToBool(reader, col++);
         ferpa                = SQLHelpers.ByteToBool(reader, col++);
         gdpr                 = SQLHelpers.ByteToBool(reader, col++);
         
         // DML Filterin g
         auditDmlAll           = SQLHelpers.ByteToBool( reader, col++);
         auditUserTables       = SQLHelpers.ByteToInt( reader, col++ );
         auditSystemTables     = SQLHelpers.ByteToBool( reader, col++);
         auditStoredProcedures = SQLHelpers.ByteToBool( reader, col++);
         auditDmlOther         = SQLHelpers.ByteToBool( reader, col++);
         
         // 3.1 Data Changes
         auditDataChanges      = SQLHelpers.ByteToBool( reader, col++ );

         //3.5 Sensitive Columns
         auditSensitiveColumns = SQLHelpers.ByteToBool( reader, col++ );
         auditCaptureTrans     = SQLHelpers.ByteToBool( reader, col++ );
         
         sqlDatabaseId        = SQLHelpers.GetInt16( reader, col++);

         // User
         auditPrivUsersList = SQLHelpers.GetString(reader, col++);
         auditUserAll = SQLHelpers.ByteToBool(reader, col++);
         auditUserLogins = SQLHelpers.ByteToBool(reader, col++);
         auditUserFailedLogins = SQLHelpers.ByteToBool(reader, col++);

         auditUserDDL = SQLHelpers.ByteToBool(reader, col++);
         auditUserSecurity = SQLHelpers.ByteToBool(reader, col++);
         auditUserAdmin = SQLHelpers.ByteToBool(reader, col++);
         auditUserDML = SQLHelpers.ByteToBool(reader, col++);
         auditUserSELECT = SQLHelpers.ByteToBool(reader, col++);

         auditUserAccessCheck = SQLHelpers.ByteToInt(reader, col++);
         auditUserCaptureSQL = SQLHelpers.ByteToBool(reader, col++);
         auditUserCaptureTrans = SQLHelpers.ByteToBool(reader, col++);
         auditUserExceptions = SQLHelpers.ByteToBool(reader, col++);
         auditUserUDE = SQLHelpers.ByteToBool(reader, col++);
         auditUserCaptureDDL = SQLHelpers.ByteToBool(reader, col++);
         auditCaptureDDL = SQLHelpers.ByteToBool(reader, col++);
         auditUserLogouts = SQLHelpers.ByteToBool(reader, col++);
         auditSensitiveColumnActivity = SQLHelpers.ByteToInt(reader, col);
         auditSensitiveColumnActivityDataset = SQLHelpers.ByteToInt(reader, col);
        }

        //-----------------------------------------------------------------------------
        // InternalRead
        //-----------------------------------------------------------------------------
        private bool
	      InternalRead(
	         string            where
	      )
	   {
	      bool           retval = false;
	      
	      try
	      {
			   string cmdstr = GetSelectSQL( where, "" );

			   using ( SqlCommand    cmd    = new SqlCommand( cmdstr,conn ) )
			   {
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
			         if ( reader.Read() )
                  {
                     Load( reader );
                     retval = true;
                  }
               }
            }
	      }
	      catch (Exception ex )
	      {
	        errMsg = ex.Message;
			}
	   
	      return retval;
	   }

        //-------------------------------------------------------------------
        // InternalWrite - Writes database record to repository
        //
        // Has to do 4/5  writes: if creating, INSERT database
        //                         UPDATE Database table
        //                         modifyTables table
        //                         UPDATE Config version in Servers table
        //                         INSERT activity log entry
        //-------------------------------------------------------------------
        //v5.6 SQLCM-5373
        private bool
          InternalUpdate(
             DatabaseRecord oldDatabaseRecord,
             SqlTransaction transaction
          )
       {
           bool retval = false;

           //--------------------------------------------------
           // Builds set of SQL commands for write transaction
           // we do this in a transaction to guarantee some
           // consistency between database and tables
           //--------------------------------------------------

           StringBuilder sqlCmd = new StringBuilder("");
           sqlCmd.Append(GetUpdateSQL(oldDatabaseRecord));
           // If sqlCmd is empty, then there is nothing to do!
           if (sqlCmd.Length == 0)
               return true;
           //--------------------------------------------------
           // Execute SQL
           //--------------------------------------------------
           try
           {
               using (SqlCommand cmd = new SqlCommand(sqlCmd.ToString(), conn))
               {
                   if (transaction != null)
                   {
                       cmd.Transaction = transaction;
                   }

                   int nRows = cmd.ExecuteNonQuery();
                   if (nRows == 1)
                   {
                       retval = true;
                   }
               }
           }
           catch (Exception ex)
           {
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                        "DatabaseRecord::InternalWrite::Execute",
                                        sqlCmd.ToString(),
                                        ex);
               errMsg = ex.Message;
           }

           return retval;
       }
	   
      //-------------------------------------------------------------------
      // InternalWrite - Writes database record to repository
      //
      // Has to do 4/5  writes: if creating, INSERT database
      //                         UPDATE Database table
      //                         modifyTables table
      //                         UPDATE Config version in Servers table
      //                         INSERT activity log entry
      //-------------------------------------------------------------------
      private bool
         InternalWrite(
            bool              newDatabase,
            DatabaseRecord    oldDatabaseRecord,
            SqlTransaction    transaction
         ) 
	   {
	      bool retval = false;

         //--------------------------------------------------
         // Builds set of SQL commands for write transaction
         // we do this in a transaction to guarantee some
         // consistency between database and tables
         //--------------------------------------------------
	      
	      StringBuilder sqlCmd = new StringBuilder( "" );

         // Create Database Record
         if ( newDatabase )
         {
            sqlCmd.Append( GetInsertSQL() );
         }
         else
         {
            sqlCmd.Append( GetUpdateSQL( oldDatabaseRecord ) );
         }
         
         
         // User Tables
         //sqlCmd.Append += this.DeleteTablesSQL();
         //sqlCmd += this.CreateTablesSQL();
         
         // If sqlCmd is empty, then there is nothing to do!
         if ( sqlCmd.Length == 0 )
            return true;
            

         //--------------------------------------------------
         // Execute SQL
         //--------------------------------------------------
         try
         {
		      using ( SqlCommand  cmd = new SqlCommand( sqlCmd.ToString(),conn) )
		      {
               if ( transaction != null )
               {
                  cmd.Transaction = transaction;
               }
               		                                        
               int nRows = cmd.ExecuteNonQuery();
               if ( nRows == 1 )
               {
                  if ( newDatabase )
                  {
	                  this.dbId = GetDatabaseId(conn, this.srvId, this.name );
                  }
                  
                  retval = true;
               }
            }
         }
         catch( Exception ex )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                     "DatabaseRecord::InternalWrite::Execute",
                                     sqlCmd.ToString(),
                                     ex );
            errMsg = ex.Message;
         }
                             
         return retval;
	   }

        //-------------------------------------------------------------------
        // UpdatePrivUsers - Updates database privilege users record to repository
        //-------------------------------------------------------------------
        public bool
           UpdatePrivUsers(
              DatabaseRecord oldDatabaseRecord,
              SqlConnection connection
           )
        {
            bool retval = false;

            StringBuilder sqlCmd = new StringBuilder("");
            sqlCmd.Append(GetUpdateSQL(oldDatabaseRecord));

            if (sqlCmd.Length == 0)
                return true;

            //--------------------------------------------------
            // Execute SQL
            //--------------------------------------------------
            try
            {
                using (SqlCommand cmd = new SqlCommand(sqlCmd.ToString(), connection))
                {
                    int nRows = cmd.ExecuteNonQuery();
                    if (nRows == 1)
                    {
                        retval = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "DatabaseRecord::InternalWrite::Execute",
                                         sqlCmd.ToString(),
                                         ex);
                errMsg = ex.Message;
            }

            return retval;
        }

        public bool SaveTrustedandPrivUsers(StringBuilder querySQL)
        {
            bool retval = false;
            try
            {
                int nRows;
                using (SqlCommand cmd = new SqlCommand(querySQL.ToString(), conn))
                {
                    nRows = cmd.ExecuteNonQuery();
                }
                retval = true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return retval;
        }
        public bool SaveDatabaseLevelUsersFromServerSettings(DatabaseRecord currdb, RemoteUserList trustedUserList, RemoteUserList privilegedUserList)
        {
            StringBuilder userSQL = new StringBuilder("");
            userSQL.Append(GetDeleteOldUserForCurrentDatabaseSQL(currdb.DbId));
            if (currdb.AuditUsersList != null && currdb.AuditUsersList != "")
            {
                userSQL.Append(GetTrustedUsersSQLForDatabase(currdb, trustedUserList));
            }
            if (currdb.AuditPrivUsersList != null && currdb.AuditPrivUsersList != "")
            {
                userSQL.Append(GetPrivilegedUsersSQLForDatabase(currdb, privilegedUserList));
            }
            if (SaveTrustedandPrivUsers(userSQL))
            {
                return true;
            }
            return false;
        }
        private string GetDeleteOldUserForCurrentDatabaseSQL(int dbId)
        {
            string deleteSQL = "";
            deleteSQL += GetDeleteSQLForUser(dbId);
            return deleteSQL;
        }
        private string GetTrustedUsersSQLForDatabase(DatabaseRecord dbDetails, RemoteUserList userList)
        {
            string trustedUsersSQL = "";
            foreach (ServerRole tRole in userList.ServerRoles)
            {
                trustedUsersSQL += dbDetails.GetInsertSQLForUser(0, dbDetails.DbId, 1, 0, tRole.Name, null);
            }
            foreach (Login tLogin in userList.Logins)
            {
                trustedUsersSQL += dbDetails.GetInsertSQLForUser(0, dbDetails.DbId, 1, 0, null, tLogin.Name);
            }
            return trustedUsersSQL;
        }
    private string GetPrivilegedUsersSQLForDatabase(DatabaseRecord dbDetails, RemoteUserList userList)
    {
        string privUsersSQL = "";
        foreach (ServerRole tRole in userList.ServerRoles)
        {
            privUsersSQL += dbDetails.GetInsertSQLForUser(0, dbDetails.DbId, 0, 1, tRole.Name, null);
        }
        foreach (Login tLogin in userList.Logins)
        {
            privUsersSQL += dbDetails.GetInsertSQLForUser(0, dbDetails.DbId, 0, 1, null, tLogin.Name);
        }
        return privUsersSQL;
    }
    public bool DeleteUser(string querySQL)
    {
        bool retval = false;
        try
        {
            int nRows;
            using (SqlCommand cmd = new SqlCommand(querySQL, conn))
            {
                nRows = cmd.ExecuteNonQuery();
            }
            if (nRows > 0)
            {
                retval = true;
            }
        }
        catch (Exception ex)
        {
            errMsg = ex.Message;
        }
        return retval;
    }
      //-------------------------------------------------------------------
      // InternalCopy - Copy database record to repository from primary to secondary
      //-------------------------------------------------------------------
      private bool
       InternalCopy(
           bool isPrimary, 
           int srvId, 
           string srvInstance, 
           string avgName,
          SqlConnection     conn
       )
      {

          bool retval = false;

          //--------------------------------------------------
          // Builds set of SQL commands for write transaction
          // we do this in a transaction to guarantee some
          // consistency between database and tables
          //--------------------------------------------------

          StringBuilder sqlCmd = new StringBuilder("");          

          sqlCmd.Append(GetInsertCopySQL(isPrimary, srvId, srvInstance, avgName));

          // If sqlCmd is empty, then there is nothing to do!
          if (sqlCmd.Length == 0)
              return true;

          //--------------------------------------------------
          // Execute SQL
          //--------------------------------------------------
          try
          {
              SQLHelpers.CheckConnection(conn);
              using (SqlCommand cmd = new SqlCommand(sqlCmd.ToString(), conn))
              {
                  int nRows = cmd.ExecuteNonQuery();
                  if (nRows > 1)
                  {
                      retval = true;
                  }
              }
          }
          catch (Exception ex)
          {
              ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                       "DatabaseRecord::InternalCopyWrite::Execute",
                                       sqlCmd.ToString(),
                                       ex);
              errMsg = ex.Message;
          }

          return retval;

      
      }
	   
      
      #endregion
      
      #region SQL Builders
      
      //-------------------------------------------------------------
      // GetDatabaseIdSQL - searching with server id and database name
      //--------------------------------------------------------------
      private static string
         GetDatabaseIdSQL(       
           int                srvId,
           string             dbName )
      {
          string tmp = "SELECT d.dbId" +
		                     " FROM {0} AS d, {1} as s " +
		                     " WHERE d.srvId=s.srvId AND s.srvId={2} AND d.name={3}";
		                     
         return string.Format( tmp,
                               CoreConstants.RepositoryDatabaseTable,
                               CoreConstants.RepositoryServerTable,
                               srvId,
                               SQLHelpers.CreateSafeString(dbName) );
      }

      //-------------------------------------------------------------
      // GetDatabaseIdSQL - searching with server name and database name
      //--------------------------------------------------------------
      private static string
         GetDatabaseIdSQL(       
           string             srvName,
           string             dbName )
      {
          string tmp = "SELECT d.dbId" +
		                     " FROM {0}..{1} AS d, {0}..{2} as s " +
		                     " WHERE d.srvId=s.srvId AND s.instance={3} AND d.name={4}";
		                     
         return string.Format( tmp,
                               CoreConstants.RepositoryDatabase,
                               CoreConstants.RepositoryDatabaseTable,
                               CoreConstants.RepositoryServerTable,
                               SQLHelpers.CreateSafeString(srvName),
                               SQLHelpers.CreateSafeString(dbName) );
      }

        //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
        //Get query for getting count of all databases in a server
        private static string GetCountDatabasesSQL(int srvId)
        {
            string query = "SELECT COUNT(*) FROM {0}..{1} WHERE srvId = {2}";

            return string.Format(query, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDatabaseTable, srvId);
        }

        //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
        //Get query for getting 'auditDmlAll' flag
        private static string GetAuditDMLAllSQL(int srvId, int dbId)
        {
            string query = "SELECT COUNT(*) FROM {0}..{1} WHERE srvId = {2} AND dbId = {3} AND (auditDmlAll = 1 OR (auditDmlAll = 0 AND auditUserTables <> 2)) AND (auditDML = 1 OR auditSELECT = 1)";

            return string.Format(query, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDatabaseTable, srvId, dbId);
        }

        //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
        //Get query for updating 'auditDmlAll' flag
        private static string GetUpdateAuditDMLAllSQL(int srvId, int dbId, int newValue)
        {
            string query = "Update {0}..{1} SET auditDmlAll = {4} WHERE srvId = {2} AND dbId = {3}";

            return string.Format(query, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDatabaseTable, srvId, dbId, newValue);
        }

        //-------------------------------------------------------------
        // GetSelectSQL
        //--------------------------------------------------------------
        private static string
         GetSelectSQL(       
           string             strWhere,
           string             strOrder )
      {
          string tmp = "SELECT d.dbId" +
			                  ",d.srvId" +
			                  ",s.instance" +
			                  ",d.name" +
			                  ",d.description" +
    		                  ",d.isEnabled" +
    		                  ",d.timeCreated" +
    		                  ",d.timeLastModified" +
    		                  ",d.timeEnabledModified" +
    		                  ",d.isSqlSecureDb" +
                              ",d.isAlwaysOn" +
                              ",d.isPrimary" +
                              ",d.replicaServers" +
                              ",d.availGroupName" +
			                  ",d.auditDDL,d.auditSecurity,d.auditAdmin,d.auditBroker,d.auditDML,d.auditSELECT" + 
			                  ",d.auditFailures,d.auditCaptureSQL,d.auditExceptions,d.auditUsersList" +
                           ",d.pci,d.hipaa,d.disa,d.nerc,d.cis,d.sox,d.ferpa, d.gdpr" +
			                  ",d.auditDmlAll,d.auditUserTables,d.auditSystemTables" +
			                  ",d.auditStoredProcedures,d.auditDmlOther" +
			                  ",d.auditDataChanges" +
                           ",d.auditSensitiveColumns" +
                           ",d.auditCaptureTrans" +
			                  ",d.sqlDatabaseId" +
                           ",d.auditPrivUsersList,d.auditUserAll,d.auditUserLogins,d.auditUserFailedLogins" +
                              ",d.auditUserDDL,d.auditUserSecurity,d.auditUserAdmin,d.auditUserDML,d.auditUserSELECT" +
                              ",d.auditUserFailures,d.auditUserCaptureSQL,d.auditUserCaptureTrans,d.auditUserExceptions,d.auditUserUDE,d.auditUserCaptureDDL,d.auditCaptureDDL,d.auditUserLogouts" + // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
                             ",auditSensitiveColumnActivity, auditSensitiveColumnActivityDataset" +
                              " FROM {0} AS d, {1} as s " +
		                     " WHERE d.srvId=s.srvId {2}{3}" + // where
		                     " {4};";  // order
		                     
         return string.Format( tmp,
                               CoreConstants.RepositoryDatabaseTable,
                               CoreConstants.RepositoryServerTable,
                               (strWhere!="") ? "AND " : "",
                               strWhere,
                               strOrder );
      }

      //-------------------------------------------------------------
      // GetSecondarySelectSQL
      //--------------------------------------------------------------
      private static string
         GetSecondarySelectSQL(
           int srvId
           )
      {
          string tmp = "SELECT d.dbId" +
                              ",d.srvId" +
                              ",d.srvInstance" +
                              ",d.name" +
                              ",d.description" +
                              ",d.isEnabled" +
                              ",d.timeCreated" +
                              ",d.timeLastModified" +
                              ",d.timeEnabledModified" +
                              ",d.isSqlSecureDb" +
                              ",d.isAlwaysOn" +
                              ",d.isPrimary" +
                              ",d.replicaServers" +
                              ",d.availGroupName" +
                              ",d.auditDDL,d.auditSecurity,d.auditAdmin,d.auditBroker,d.auditDML,d.auditSELECT" +
                              ",d.auditFailures,d.auditCaptureSQL,d.auditExceptions,d.auditUsersList" +
                           ",d.pci,d.hipaa,d.disa,d.nerc,d.cis,d.sox,d.ferpa, d.gdpr" +
                              ",d.auditDmlAll,d.auditUserTables,d.auditSystemTables" +
                              ",d.auditStoredProcedures,d.auditDmlOther" +
                              ",d.auditDataChanges" +
                           ",d.auditSensitiveColumns" +
                           ",d.auditCaptureTrans" +
                              ",d.sqlDatabaseId" +
                           ",d.auditPrivUsersList,d.auditUserAll,d.auditUserLogins,d.auditUserFailedLogins" +
                              ",d.auditUserDDL,d.auditUserSecurity,d.auditUserAdmin,d.auditUserDML,d.auditUserSELECT" +
                              ",d.auditUserFailures,d.auditUserCaptureSQL,d.auditUserCaptureTrans,d.auditUserExceptions,d.auditUserUDE,d.auditUserCaptureDDL,d.auditCaptureDDL,d.auditUserLogouts," + // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
                              "auditSensitiveColumnActivity, auditSensitiveColumnActivityDataset" +
                             " FROM {0} AS d " +
                             " WHERE d.srvId = {1}" +
                             " AND isPrimary = 0" +
                             " AND isAlwaysOn = 1";

          return string.Format(tmp,
                                CoreConstants.RepositoryDatabaseTable,
                                srvId
                                );
      }
      
      //-------------------------------------------------------------
      // GetUpdateSQL - Create UPDATE SQL to update a database record
      //                Only add those properties that have changed. 
      //                If nothing has changed, eturn empty string
      //--------------------------------------------------------------
      private string
         GetUpdateSQL(
            DatabaseRecord    oldDb
         )
      {
         StringBuilder prop = new StringBuilder("");
         
         //--------------------
         // General Properties
         //--------------------
         if ( oldDb==null || this.name != oldDb.name )
         {
            prop.AppendFormat( "name = {0}", SQLHelpers.CreateSafeString(this.name) );
         }
            
         if ( oldDb==null || this.description != oldDb.description )
         {
            if ( prop.Length > 0 ) prop.Append(",");
            prop.AppendFormat( "description = {0}",
                               SQLHelpers.CreateSafeString(this.description) );
         }
         
         if ( oldDb==null || this.isEnabled != oldDb.isEnabled )
         {
            if ( prop.Length > 0 ) prop.Append(",");
            prop.AppendFormat( "isEnabled = {0}",
                               this.isEnabled ? 1 : 0 );
         }

         //-------------------------------------------------------------------------
         // Audit Settings
         //
         // If just changed from not overridng to overriding server, force write 
         // of all audit settings        
         //-------------------------------------------------------------------------
         if ( oldDb==null || this.auditDDL != oldDb.auditDDL )
         {
            if ( prop.Length > 0 ) prop.Append(",");
            prop.AppendFormat( "auditDDL = {0}", this.auditDDL ? 1 : 0 );
         }
         
         if ( oldDb==null || this.auditSecurity != oldDb.auditSecurity)
         {
            if ( prop.Length > 0 ) prop.Append(",");
            prop.AppendFormat( "auditSecurity = {0}", this.auditSecurity ? 1 : 0 );
         }
         
         if ( oldDb==null || this.auditAdmin != oldDb.auditAdmin)
         {
            if ( prop.Length > 0 ) prop.Append(",");
            prop.AppendFormat( "auditAdmin = {0}", this.auditAdmin ? 1 : 0 );
         }
         
         if ( oldDb==null || this.auditBroker != oldDb.auditBroker)
         {
            if ( prop.Length > 0 ) prop.Append(",");
            prop.AppendFormat( "auditBroker = {0}", this.auditBroker ? 1 : 0 );
         }
         
         if ( oldDb==null || this.auditDML != oldDb.auditDML)
         {
            if ( prop.Length > 0 ) prop.Append(",");
            prop.AppendFormat( "auditDML = {0}", this.auditDML ? 1 : 0 );
         }
         
         if ( oldDb==null || this.auditSELECT != oldDb.auditSELECT)
         {
            if ( prop.Length > 0 ) prop.Append(",");
            prop.AppendFormat( "auditSELECT = {0}",this.auditSELECT ? 1 : 0 );
         }
         
         if ( oldDb==null || this.auditAccessCheck != oldDb.auditAccessCheck)
         {
            if ( prop.Length > 0 ) prop.Append(",");
            prop.AppendFormat( "auditFailures = {0}",this.auditAccessCheck);
         }
         
         if ( oldDb==null || this.auditCaptureSQL != oldDb.auditCaptureSQL || !CoreConstants.AllowCaptureSql)
         {
            if ( prop.Length > 0 ) prop.Append(",");
            if(!CoreConstants.AllowCaptureSql)
               prop.Append( "auditCaptureSQL = 2");
            else
            prop.AppendFormat( "auditCaptureSQL = {0}",this.auditCaptureSQL ? 1 : 0 );
         }
         
         if ( oldDb==null || this.auditExceptions != oldDb.auditExceptions)
         {
            if ( prop.Length > 0 ) prop.Append(",");
            prop.AppendFormat( "auditExceptions = {0}",this.auditExceptions ? 1 : 0 );
         }
         //Trusted Users //v5.6 SQLCM-5373
         if (oldDb == null || this.auditUsersList != oldDb.auditUsersList)
         {
            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditUsersList = {0}", SQLHelpers.CreateSafeString(auditUsersList));
         }

         if (oldDb == null || this.pci != oldDb.pci)
         {
            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("pci = {0}", this.pci ? 1 : 0);
         }

         if (oldDb == null || this.hipaa != oldDb.hipaa)
         {
            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("hipaa = {0}", this.hipaa ? 1 : 0);
         }
         if (oldDb == null || this.disa != oldDb.disa)
         {
             if (prop.Length > 0) prop.Append(",");
             prop.AppendFormat("disa = {0}", this.disa ? 1 : 0);
         }
         if (oldDb == null || this.nerc != oldDb.nerc)
         {
             if (prop.Length > 0) prop.Append(",");
             prop.AppendFormat("nerc = {0}", this.nerc ? 1 : 0);
         }
         if (oldDb == null || this.cis != oldDb.cis)
         {
             if (prop.Length > 0) prop.Append(",");
             prop.AppendFormat("cis = {0}", this.cis ? 1 : 0);
         }
         if (oldDb == null || this.sox != oldDb.sox)
         {
             if (prop.Length > 0) prop.Append(",");
             prop.AppendFormat("sox = {0}", this.sox ? 1 : 0);
         }
         if (oldDb == null || this.ferpa != oldDb.ferpa)
         {
             if (prop.Length > 0) prop.Append(",");
             prop.AppendFormat("ferpa = {0}", this.ferpa ? 1 : 0);
         }
         if (oldDb == null || this.gdpr != oldDb.gdpr)
         {
             if (prop.Length > 0) prop.Append(",");
             prop.AppendFormat("gdpr = {0}", this.gdpr ? 1 : 0);
         }

         //---------------
         // DML Filtering
         //---------------
         if ( oldDb==null || this.auditDmlAll != oldDb.auditDmlAll )
         {
            if ( prop.Length > 0 ) prop.Append(",");
            prop.AppendFormat( "auditDmlAll = {0}", this.auditDmlAll ? 1 : 0 );
         }
         if ( oldDb==null || this.auditUserTables != oldDb.auditUserTables )
         {
            if ( prop.Length > 0 ) prop.Append(",");
            prop.AppendFormat( "auditUserTables = {0}", this.auditUserTables );
         }
         if ( oldDb==null || this.auditSystemTables != oldDb.auditSystemTables )
         {
            if ( prop.Length > 0 ) prop.Append(",");
            prop.AppendFormat( "auditSystemTables = {0}", this.auditSystemTables ? 1 : 0 );
         }
         if ( oldDb==null || this.auditStoredProcedures != oldDb.auditStoredProcedures )
         {
            if ( prop.Length > 0 ) prop.Append(",");
            prop.AppendFormat( "auditStoredProcedures = {0}", this.auditStoredProcedures ? 1 : 0 );
         }
         if ( oldDb==null || this.auditDmlOther != oldDb.auditDmlOther )
         {
            if ( prop.Length > 0 ) prop.Append(",");
            prop.AppendFormat( "auditDmlOther = {0}", this.auditDmlOther ? 1 : 0 );
         }
         if( oldDb == null || this.auditDataChanges != oldDb.auditDataChanges )
         {
            if ( prop.Length > 0 ) prop.Append( "," );
            prop.AppendFormat( "auditDataChanges = {0}", this.auditDataChanges ? 1 : 0 );
         }

         if (oldDb == null || this.auditSensitiveColumns != oldDb.auditSensitiveColumns)
         {
            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditSensitiveColumns = {0}", this.auditSensitiveColumns ? 1 : 0);
         }

         if (oldDb == null || this.auditCaptureTrans != oldDb.auditCaptureTrans)
         {
            if (prop.Length > 0) prop.Append(",");
            prop.AppendFormat("auditCaptureTrans = {0}", this.auditCaptureTrans ? 1 : 0);
         }

         // Users
         if (oldDb == null || this.auditPrivUsersList != oldDb.auditPrivUsersList)
         {
             if (prop.Length > 0) prop.Append(",");
             prop.AppendFormat("auditPrivUsersList = {0}", SQLHelpers.CreateSafeString(auditPrivUsersList));
         }

          if (oldDb == null || this.AuditUserAll != oldDb.AuditUserAll)
          {
              if (prop.Length > 0) prop.Append(",");
              prop.AppendFormat("auditUserAll = {0}", this.auditUserAll ? 1 : 0);
          }

          if (oldDb == null || this.AuditUserLogins != oldDb.AuditUserLogins)
          {
              if (prop.Length > 0) prop.Append(",");
              prop.AppendFormat("auditUserLogins = {0}", this.auditUserLogins ? 1 : 0); 
          }

          // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
          if (oldDb == null || this.AuditUserLogouts != oldDb.AuditUserLogouts)
          {
              if (prop.Length > 0) prop.Append(",");
              prop.AppendFormat("auditUserLogouts = {0}", this.auditUserLogouts ? 1 : 0);
          }

          if (oldDb == null || this.auditUserFailedLogins != oldDb.auditUserFailedLogins)
          {
              if (prop.Length > 0) prop.Append(",");
              prop.AppendFormat("auditUserFailedLogins = {0}", this.auditUserFailedLogins ? 1 : 0); 
          }

          if (oldDb == null || this.auditUserDDL != oldDb.auditUserDDL)
          {
              if (prop.Length > 0) prop.Append(",");
              prop.AppendFormat("auditUserDDL = {0}", this.auditUserDDL ? 1 : 0);
          }

          if (oldDb == null || this.auditUserSecurity != oldDb.auditUserSecurity)
          {
              if (prop.Length > 0) prop.Append(",");
              prop.AppendFormat("auditUserSecurity = {0}", this.auditUserSecurity ? 1 : 0);
          }

          if (oldDb == null || this.auditUserAdmin != oldDb.auditUserAdmin)
          {
              if (prop.Length > 0) prop.Append(",");
              prop.AppendFormat("auditUserAdmin = {0}", this.auditUserAdmin ? 1 : 0);
          }

          if (oldDb == null || this.auditUserDML != oldDb.auditUserDML)
          {
              if (prop.Length > 0) prop.Append(",");
              prop.AppendFormat("auditUserDML = {0}", this.auditUserDML ? 1 : 0); 
          }

          if (oldDb == null || this.auditUserSELECT != oldDb.auditUserSELECT)
          {
              if (prop.Length > 0) prop.Append(",");
              prop.AppendFormat("auditUserSELECT = {0}", this.auditUserSELECT ? 1 : 0); 
          }

          if (oldDb == null || this.auditUserAccessCheck != oldDb.auditUserAccessCheck)
          {
              if (prop.Length > 0) prop.Append(",");
              prop.AppendFormat("auditUserFailures = {0}", this.auditUserAccessCheck); 
          }

          if(oldDb == null || this.auditSensitiveColumnActivity != oldDb.auditSensitiveColumnActivity)
          {
                if (prop.Length > 0) prop.Append(",");
                prop.AppendFormat("auditSensitiveColumnActivity = {0}",this.auditSensitiveColumnActivity);
          }
          if (oldDb == null || this.auditSensitiveColumnActivityDataset != oldDb.auditSensitiveColumnActivityDataset)
          {
              if (prop.Length > 0) prop.Append(",");
              prop.AppendFormat("auditSensitiveColumnActivityDataset = {0}", this.auditSensitiveColumnActivityDataset);
          }
             
         if (oldDb == null || this.auditUserCaptureSQL != oldDb.auditUserCaptureSQL || !CoreConstants.AllowCaptureSql)
         {
             if (CoreConstants.AllowCaptureSql)
             {
                 if (prop.Length > 0) prop.Append(",");
                 prop.AppendFormat("auditUserCaptureSQL = {0}", this.AuditUserCaptureSQL ? 1 : 0);
             }
             else
             {
                 if (prop.Length > 0) prop.Append(",");
                 prop.AppendFormat("auditUserCaptureSQL = {0}", 2);
             }
         }

         if (oldDb == null || this.auditUserCaptureDDL != oldDb.auditUserCaptureDDL || !CoreConstants.AllowCaptureSql)
         {
             if (CoreConstants.AllowCaptureSql)
             {
                 if (prop.Length > 0) prop.Append(",");
                 prop.AppendFormat("auditUserCaptureDDL = {0}", this.auditUserCaptureDDL ? 1 : 0);
             }
             else
             {
                 if (prop.Length > 0) prop.Append(",");
                 prop.AppendFormat("auditUserCaptureDDL = {0}", 2);
             }
         }

         if (oldDb == null || this.auditCaptureDDL != oldDb.auditCaptureDDL || !CoreConstants.AllowCaptureSql)
         {
             if (CoreConstants.AllowCaptureSql)
             {
                 if (prop.Length > 0) prop.Append(",");
                 prop.AppendFormat("auditCaptureDDL = {0}", this.auditCaptureDDL ? 1 : 0);
             }
             else
             {
                 if (prop.Length > 0) prop.Append(",");
                 prop.AppendFormat("auditCaptureDDL = {0}", 2);
             }
         }

          if (oldDb == null || this.auditUserCaptureTrans != oldDb.auditUserCaptureTrans)
          {
              if (prop.Length > 0) prop.Append(",");
              prop.AppendFormat("auditUserCaptureTrans = {0}", this.auditUserCaptureTrans ? 1 : 0);
          }

          if (oldDb == null || this.auditUserExceptions != oldDb.auditUserExceptions)
          {
              if (prop.Length > 0) prop.Append(",");
              prop.AppendFormat("auditUserExceptions = {0}", this.auditUserExceptions ? 1 : 0);
          }

          if (oldDb == null || this.auditUserUDE != oldDb.auditUserUDE)
          {
              if (prop.Length > 0) prop.Append(",");
              prop.AppendFormat("auditUserUDE = {0}", this.auditUserUDE ? 1 : 0); 
          }
         
         //----------------------------------------------------
         // Finish Building SQL if any properties have changed
         //-----------------------------------------------------
         StringBuilder tmp  = new StringBuilder("");
         if ( prop.Length > 0 )
         {                      
            // update last modified
            prop.Append( ",timeLastModified = GETUTCDATE()" );
            
            tmp.AppendFormat( "UPDATE {0} SET ",
                              CoreConstants.RepositoryDatabaseTable );
            tmp.Append( prop.ToString() );
            tmp.AppendFormat( " WHERE dbId={0};", this.dbId );       
         }
		                     
         return tmp.ToString();
      }

      //-------------------------------------------------------------
      // GetEnableSQL - Create SQL to enable/disable 
      //                database auditing
      //--------------------------------------------------------------
      private string
         GetEnableSQL(
            bool              enable
         )
      {
         StringBuilder cmd = new StringBuilder("");
         
         cmd.AppendFormat( "UPDATE {0} SET ",
                           CoreConstants.RepositoryDatabaseTable );
         
         cmd.AppendFormat( "isEnabled = {0}",
                            enable ? 1 : 0 );

         cmd.Append( ",timeEnabledModified=GETUTCDATE()" );

         cmd.AppendFormat( " WHERE dbId={0};", this.dbId );       
		                     
         return cmd.ToString();
      }

        public string GetInsertSQLForUser(int srvId, int dbID, int isTrusted, int isPrivileged, string roleName, string loginName)
        {
            string tmp = "INSERT INTO {0} " +
                           "(" +
                                 "serverId" +
                                 ",databaseId" +
                                 ",isTrusted" +
                                 ",isPrivileged" +
                                 ",roleName" +
                                 ",loginName" +
                              ") VALUES ({1},{2},{3},{4},{5},{6});";
            return string.Format(tmp,
                                  CoreConstants.RepositoryUsersTable,
                                  srvId,
                                  dbID,
                                  isTrusted,
                                  isPrivileged,
                                  SQLHelpers.CreateSafeString(roleName),
                                  SQLHelpers.CreateSafeString(loginName));
        }
        public string GetDeleteSQLForUser(int dbId)
        {
            string deleteSql = String.Format("DELETE FROM {0} where databaseId = {1};", CoreConstants.RepositoryUsersTable, dbId);
            return deleteSql;
        }
      //-------------------------------------------------------------
      // GetInsertSQL
      //--------------------------------------------------------------
      private string
         GetInsertSQL()
      {
         int captureSql = 0 ;

         if(!CoreConstants.AllowCaptureSql)
            captureSql = 2 ;
         else
            captureSql = auditCaptureSQL ? 1 : 0 ;

         if (this.availGroupName == null)
             this.availGroupName = "";
          string tmp = "INSERT INTO {0} "+
                           "(" +
			                     "name" +
			                     ",description" +
    		                     ",isEnabled" +
    		                     ",timeCreated" +
    		                     ",timeLastModified" +
    		                     ",timeEnabledModified" +
    		                     ",isSqlSecureDb" +
                                 ",isAlwaysOn" +
                                 ",isPrimary" +
                                 ",replicaServers" +
                                 ",availGroupName" +
			                     ",auditDDL" +
			                     ",auditSecurity" +
			                     ",auditAdmin" +
			                     ",auditBroker" +
			                     ",auditDML" +
			                     ",auditSELECT" +
			                     ",auditFailures" +
			                     ",auditCaptureSQL" +
			                     ",auditExceptions" +
                              ",auditUsersList" +
                              ",pci" +
                              ",hipaa"+
			                     ",auditDmlAll" +
			                     ",auditUserTables" +
			                     ",auditSystemTables" +
			                     ",auditStoredProcedures" +
			                     ",auditDmlOther" +
			                     ",auditDataChanges" + 
                              ",auditSensitiveColumns" + 
                              ",auditCaptureTrans" + 
			                     ",srvId" +
			                     ",srvInstance" +
			                     ",sqlDatabaseId" +
                                 ",auditPrivUsersList"+
                                 ",auditUserAll"+
                                 ",auditUserLogins"+
                                 ",auditUserFailedLogins"+
                                 ",auditUserDDL"+
                                 ",auditUserSecurity"+
                                 ",auditUserAdmin"+
                                 ",auditUserDML"+
                                 ",auditUserSELECT"+
                                 ",auditUserFailures" +
                                 ",auditUserCaptureSQL"+
                                 ",auditUserCaptureTrans"+
                                 ",auditUserExceptions"+
                                 ",auditUserUDE"+
                                 ",auditUserCaptureDDL" +
                                 ",auditCaptureDDL" +
                                 ",disa" +
                              	 ",nerc" +
                                 ",cis" +
                                 ",sox" +
                                 ",ferpa" +
                                 ",auditUserLogouts" + // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
                                 ",gdpr"+
                                 ",auditSensitiveColumnActivity, auditSensitiveColumnActivityDataset" +
                              ") VALUES ({1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56},{57},{58}, {59});";

            return string.Format(tmp,
                                  CoreConstants.RepositoryDatabaseTable,
                                  SQLHelpers.CreateSafeString(this.name),
                                  SQLHelpers.CreateSafeString(this.description),
                                  this.isEnabled ? 1 : 0,
                                  "GETUTCDATE()",
                                  "GETUTCDATE()",
                                  "GETUTCDATE()",
                                  this.isSqlSecureDb ? 1 : 0,
                                  this.isAlwaysOn ? 1 : 0,
                                  this.isPrimary ? 1 : 0,
                                  SQLHelpers.CreateSafeString(this.replicaServers),
                                  SQLHelpers.CreateSafeString(this.availGroupName),
                                  this.auditDDL ? 1 : 0,
                                  this.auditSecurity ? 1 : 0,
                                  this.auditAdmin ? 1 : 0,
                                  this.auditBroker ? 1 : 0,
                                  this.auditDML ? 1 : 0,
                                  this.auditSELECT ? 1 : 0,
                                  this.auditAccessCheck,
                                  captureSql,
                                  this.auditExceptions ? 1 : 0,
                                  SQLHelpers.CreateSafeString(auditUsersList),
                                  this.pci ? 1 : 0,
                                  this.hipaa ? 1 : 0,
                                  this.auditDmlAll ? 1 : 0,
                                     this.auditUserTables,
                                     this.auditSystemTables ? 1 : 0,
                                     this.auditStoredProcedures ? 1 : 0,
                                  this.auditDmlOther ? 1 : 0,
                                  this.auditDataChanges ? 1 : 0,
                                  this.auditSensitiveColumns ? 1 : 0,
                                  this.auditCaptureTrans ? 1 : 0,
                                  this.srvId,
                                  SQLHelpers.CreateSafeString(this.srvInstance),
                                  this.sqlDatabaseId,
                                  SQLHelpers.CreateSafeString(auditPrivUsersList),
                                  this.auditUserAll ? 1 : 0,
                                  this.auditUserLogins ? 1 : 0,
                                  this.auditUserFailedLogins ? 1 : 0,
                                  this.auditUserDDL ? 1 : 0,
                                  this.auditUserSecurity ? 1 : 0,
                                  this.auditUserAdmin ? 1 : 0,
                                  this.auditUserDML ? 1 : 0,
                                  this.auditUserSELECT ? 1 : 0,
                                  this.auditUserAccessCheck,
                                  this.auditUserCaptureSQL ? 1 : 0,
                                  this.auditUserCaptureTrans ? 1 : 0,
                                  this.auditUserExceptions ? 1 : 0,
                                  this.auditUserUDE ? 1 : 0,
                                  this.auditUserCaptureDDL ? 1 : 0,
                                  this.auditCaptureDDL ? 1 : 0,
                                  this.disa ? 1 : 0,
                                  this.nerc ? 1 : 0,
                                  this.cis ? 1 : 0,
                                  this.sox ? 1 : 0,
                                  this.ferpa ? 1 : 0,
                                  this.auditUserLogouts ? 1 : 0,  // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
                                  this.gdpr ? 1 : 0,
                                  this.auditSensitiveColumnActivity,
                                  this.auditSensitiveColumnActivityDataset);
                                  
      }


      //-------------------------------------------------------------
      // GetInsertCopySQL
      //--------------------------------------------------------------

      private string
          GetInsertCopySQL(           bool isPrimary, 
           int srvId, 
           string srvInstance, 
           string avgName)
      {

          string cmdstr = String.Format("INSERT INTO {0} " +
                               "(" +
                                     "name" +
                                     ",description" +
                                     ",isEnabled" +
                                     ",timeCreated" +
                                     ",timeLastModified" +
                                     ",timeEnabledModified" +
                                     ",isSqlSecureDb" +
                                     ",isAlwaysOn" +
                                     ",isPrimary" +
                                     ",replicaServers" +
                                     ",availGroupName" +
                                     ",auditDDL" +
                                     ",auditSecurity" +
                                     ",auditAdmin" +
                                     ",auditBroker" +
                                     ",auditDML" +
                                     ",auditSELECT" +
                                     ",auditFailures" +
                                     ",auditCaptureSQL" +
                                     ",auditExceptions" +
                                  ",auditUsersList" +
                                  ",pci" +
                                  ",hipaa" +
                                     ",auditDmlAll" +
                                     ",auditUserTables" +
                                     ",auditSystemTables" +
                                     ",auditStoredProcedures" +
                                     ",auditDmlOther" +
                                     ",auditDataChanges" +
                                  ",auditSensitiveColumns" +
                                  ",auditCaptureTrans" +
                                     ",srvId" +
                                     ",srvInstance" +
                                     ",sqlDatabaseId" +
                                     ",auditPrivUsersList" +
                                     ",auditUserAll" +
                                     ",auditUserLogins" +
                                     ",auditUserFailedLogins" +
                                     ",auditUserDDL" +
                                     ",auditUserSecurity" +
                                     ",auditUserAdmin" +
                                     ",auditUserDML" +
                                     ",auditUserSELECT" +
                                     ",auditUserFailures" +
                                     ",auditUserCaptureSQL" +
                                     ",auditUserCaptureTrans" +
                                     ",auditUserExceptions" +
                                     ",auditUserUDE" +
                                     ",auditUserCaptureDDL" +
                                     ",auditCaptureDDL" +
                                     ",disa" +
                                     ",nerc" +
                                     ",cis" +
                                     ",sox" +
                                     ",ferpa" +
                                     ",auditUserLogouts" + // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
                                     ",gdpr" +
                                     ",auditSensitiveColumnActivity, auditSensitiveColumnActivityDataset" +
                                     ")" +              
                      "SELECT " +                      
                                     "name" +
                                     ",description" +
                                     ",isEnabled" +
                                     ",{1}" +
                                     ",{2}" +
                                     ",{3}" +
                                     ",isSqlSecureDb" +
                                     ",isAlwaysOn" +
                                     ",{4}" +
                                     ",replicaServers" +
                                     ",availGroupName" +
                                     ",auditDDL" +
                                     ",auditSecurity" +
                                     ",auditAdmin" +
                                     ",auditBroker" +
                                     ",auditDML" +
                                     ",auditSELECT" +
                                     ",auditFailures" +
                                     ",auditCaptureSQL" +
                                     ",auditExceptions" +
                                  ",auditUsersList" +
                                  ",pci" +
                                  ",hipaa" +
                                     ",auditDmlAll" +
                                     ",auditUserTables" +
                                     ",auditSystemTables" +
                                     ",auditStoredProcedures" +
                                     ",auditDmlOther" +
                                     ",auditDataChanges" +
                                  ",auditSensitiveColumns" +
                                  ",auditCaptureTrans" +
                                     ",{5}" +
                                     ",{6}" +
                                     ",sqlDatabaseId" +
                                     ",auditPrivUsersList" +
                                     ",auditUserAll" +
                                     ",auditUserLogins" +
                                     ",auditUserFailedLogins" +
                                     ",auditUserDDL" +
                                     ",auditUserSecurity" +
                                     ",auditUserAdmin" +
                                     ",auditUserDML" +
                                     ",auditUserSELECT" +
                                     ",auditUserFailures" +
                                     ",auditUserCaptureSQL" +
                                     ",auditUserCaptureTrans" +
                                     ",auditUserExceptions" +
                                     ",auditUserUDE" +
                                     ",auditUserCaptureDDL" +
                                     ",auditCaptureDDL " +
                                     ",disa" +
                                     ",nerc" +
                                     ",cis" +
                                     ",sox" +
                                     ",ferpa" +
                                     ",auditUserLogouts" + // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
                                     ",gdpr" +
                                     "auditSensitiveColumnActivity, auditSensitiveColumnActivityDataset" +
                                  " FROM {0}  t1" +
                                  " WHERE " +
                                  "isPrimary = 1 AND availGroupName = '{7}' " +
                                  " AND" +
                                  " NOT EXISTS(" +
                                  " SELECT t2.srvId" +
                                  " FROM {0}  t2" +
                                  " WHERE" +
                                  " t2.srvId = {5}	AND " +
                                  " t2.name = t1.name AND" +
                                  " t2.isPrimary = {4}" +
                                  ")"
                                  , 
                                  CoreConstants.RepositoryDatabaseTable, 
                                  "GETUTCDATE()", "GETUTCDATE()", "GETUTCDATE()",
                                  isPrimary ? 1 : 0, srvId, SQLHelpers.CreateSafeString(srvInstance), avgName);

          return cmdstr;
                
      }

      //-------------------------------------------------------------
      // GetDeleteSQL - Create SQL to delete a database
      //--------------------------------------------------------------
      private string
         GetDeleteSQL()
      {
         string cmdStr = String.Format( "DELETE FROM {0} where dbId={1}",
                                        CoreConstants.RepositoryDatabaseTable,
                                        this.dbId );
         return cmdStr;
      }


      #endregion
	}
}
