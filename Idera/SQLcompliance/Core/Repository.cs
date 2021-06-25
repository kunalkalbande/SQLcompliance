using System;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;

namespace Idera.SQLcompliance.Core
{
	/// <summary>
	/// Summary description for Repository.
	/// </summary>
	public class Repository
	{
	   #region Constructor
	   
		public Repository()
		{
		   connection = null;
		   errMsg     = "";
		}
		
		~Repository()
		{
		   CloseConnection();
		}
		
		#endregion
		
	   #region Properties
	   
	   private static string applicationName = CoreConstants.DefaultSqlApplicationName;
	   public static string ApplicationName
	   {
	      get { return applicationName;  }
	      set { applicationName = value; }
	   }
	   private static string serverInstance  = CoreConstants.RepositoryServerDefault;
	   public static string ServerInstance
	   {
	      get { return serverInstance;  }
	      set { serverInstance = value; }
	   }
	   
	   private int sqlVersion = -1;
	   public int SqlVersion
	   {
	      get{ return sqlVersion; }
	   }
	   
	   public SqlConnection connection = null;
	   public string        errMsg     = "";
	   
	   #endregion
	   
	   #region Connection Management
	   
	   //-----------------------------------------------------------------------------
	   // OpenConnection - open a connection to the SQLsecure configuration database
	   //-----------------------------------------------------------------------------
	   public bool
	      OpenConnection()
	   {
	      return OpenConnection( CoreConstants.RepositoryDatabase, true );
	   }

	   public bool
	      OpenConnection(
	         bool                throwException
	      )
	   {
	      return OpenConnection( CoreConstants.RepositoryDatabase,
	                             throwException);
	   }
	   
	   public bool
	      OpenConnection(
	         string database
         )
	   {
	      return OpenConnection( database, true );
	   }
	   
	   public bool
	      OpenConnection(
	         string              database,
	         bool                throwException
         )
	   {
         string strConn = CreateConnectionString( database );
	      bool   retval = false;
	      
         // handle already open case
	      if ( connection != null )
	      {
	         try
	         {
	            if ( connection.State != ConnectionState.Closed )
	            {
	               connection.Close();
	            }
	         }
	         catch (Exception e)
            {
	            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                        CoreConstants.Exception_CantCloseConnectionToRepository,
	                                     e,
	                                     true );
            }
	      }
	      
	      try
	      {
            connection = new SqlConnection( strConn );
	         connection.Open();
	         sqlVersion = GetSqlVersion(connection.ServerVersion);
	         
	         
            errMsg = "";
	         retval = true;
	      }
	      catch ( SqlException sqlEx )
	      {
	         string sqlExceptionStr = String.Format( "SqlException: Severity: {0} Number: {1}  State: {2} Procedure: {3}",
	                                                 sqlEx.Class,
	                                                 sqlEx.Number,
	                                                 sqlEx.State,
	                                                 sqlEx.Procedure );
	         ErrorLog.Instance.Write( throwException ? ErrorLog.Level.Verbose : ErrorLog.Level.Default,
	                                  strConn,
	                                  sqlExceptionStr,
	                                  sqlEx,
	                                  true );
            errMsg = sqlEx.Message;
	         retval = false;
	         
	         if ( throwException )
	         {
	            throw sqlEx;
	         }
	      }
	      catch ( Exception ex )
	      {
	         ErrorLog.Instance.Write( strConn,
	                                  ex,
	                                  true );
            errMsg = ex.Message;
	         retval = false;
	         
	         if ( throwException )
	         {
	            throw ex;
	         }
	      }

	      return retval;
	   }
	   
	   public static string CreateConnectionString( string database )
	   {
         string dbName = SQLHelpers.CreateSafeDatabaseNameForConnectionString(database);

         return String.Format("server={0}" +
                             "{1}{2};" +
                             "integrated security=SSPI;" +
                             "Connect Timeout=30;" +
                             "Application Name='{3}';",
                             ServerInstance,
                             (dbName != null) && (dbName != "") ? ";database=" : "",
                             (dbName != null) && (dbName != "") ? dbName : "",
                             CoreConstants.DefaultSqlApplicationName);
      }
	   
	   
	   //-----------------------------------------------------------------------------
	   // CloseConnection - close the connection to the SQLsecure configuration database
	   //-----------------------------------------------------------------------------
	   public void
	      CloseConnection()
	   {
         try
         {
            if ( connection != null )
            {
               connection.Dispose();
               connection = null;
            }
         }
         catch{}
	   }
	   
	   //-----------------------------------------------------------------------------
	   // GetLastError
	   //-----------------------------------------------------------------------------
	   public  string   GetLastError()
	   {
	      return errMsg;
      } 
	   
	   static internal int GetSqlVersion( string versionString )
      {
         return int.Parse( versionString.Substring(0, versionString.IndexOf('.')));
      }

	   
	   #endregion

      // 
      // BuildIndexes()
      //
      // This function installs the currently supported indexes on an Events database.  It will
      //  drop these if they currently exist.  Any index upgrade should essentially call this function.
      //
      static public void BuildIndexes(SqlConnection conn)
      {
         string[] stmts = new string[6];
         // 2.1 Indexes
         stmts[0] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Alerts_created') DROP INDEX Alerts.IX_Alerts_created" ;
         stmts[1] = "CREATE  INDEX [IX_Alerts_created] ON [dbo].[Alerts]([created] DESC, [alertId] DESC ) ON [PRIMARY]" ;
         stmts[2] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Alerts_alertLevel') DROP INDEX Alerts.IX_Alerts_alertLevel" ;
         stmts[3] = "CREATE  INDEX [IX_Alerts_alertLevel] ON [dbo].[Alerts]([alertLevel], [alertId] DESC ) ON [PRIMARY]" ;
         stmts[4] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Alerts_eventType') DROP INDEX Alerts.IX_Alerts_eventType" ;
         stmts[5] = "CREATE  INDEX [IX_Alerts_eventType] ON [dbo].[Alerts]([eventType], [alertId] DESC ) ON [PRIMARY]" ;

         int i = 0 ;
         try
         {
            string sDatabase = conn.Database ;
            conn.ChangeDatabase(CoreConstants.RepositoryDatabase) ;
            for(i = 0 ; i < stmts.Length ; i++)
            {
               using(SqlCommand cmd = new SqlCommand( stmts[i], conn ))
               {
                  // Infinite time
                  cmd.CommandTimeout = 0 ;
                  cmd.ExecuteNonQuery();
               }
            }
            string s = String.Format("UPDATE Configuration SET sqlComplianceDbSchemaVersion={0}", 
                                     CoreConstants.RepositorySqlComplianceDbSchemaVersion) ;
            using(SqlCommand cmd = new SqlCommand( s, conn ))
            {
               // Infinite time
               cmd.CommandTimeout = 0 ;
               cmd.ExecuteNonQuery();
            }
            conn.ChangeDatabase(sDatabase) ;
         }
         catch(Exception e)
         {
            throw new Exception(String.Format("BuildIndexes failed on {0}", stmts[i]), e) ;
         }
      }
   }
}
