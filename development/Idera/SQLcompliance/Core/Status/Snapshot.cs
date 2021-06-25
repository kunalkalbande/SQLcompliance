using System ;
using System.Collections ;
using System.Collections.Generic ;
using System.Data.SqlClient ;
using System.Text ;
using Idera.SQLcompliance.Core.Agent ;
using Idera.SQLcompliance.Core.Event;

namespace Idera.SQLcompliance.Core.Status
{
   #region Supporting Classes	
	
	internal class SnapshotDB
	{
	   internal string  Name;
		internal bool    AuditDDL;
      internal bool    AuditSecurity;
		internal bool    AuditAdmin;
		internal bool    AuditDML;
		internal bool    AuditSELECT;
      internal int     AuditAccessCheck;
      internal bool    AuditCaptureSQL;
	  internal bool AuditCaptureDDL;
      internal bool    AuditCaptureTransactions;
		internal bool    AuditDmlAll;
		internal int     AuditUserTables;
		internal bool    AuditSystemTables;
		internal bool    AuditStoredProcedures;
		internal bool    AuditDmlOther;
		internal bool    IsEnabled;
      internal string AuditUsersList;
      internal bool AuditDataChanges;
      internal bool AuditSensitiveColums;
	  internal string AuditPrivUsersList;
      internal bool AuditUserAll;
      internal bool AuditUserLogins;
      internal bool AuditUserLogouts;
      internal bool AuditUserFailedLogins;
      internal bool AuditUserDDL;
      internal bool AuditUserDML;
      internal bool AuditUserSecurity;
      internal bool AuditUserAdmin;
      internal bool AuditUserSELECT;
      internal bool AuditUserUDE;
      internal int AuditUserAccessCheck;
      internal bool AuditUserCaptureSQL;
      internal bool AuditUserCaptureDDL;
      internal bool AuditUserCaptureTrans;
		
		// read database settings
		public void
		   Read(
		      SqlConnection     conn,
		      int               dbId
         )
		{
		   string        sql    = "";
		   
		   try
		   {
		      // read
            string tmp = "SELECT " +
			                  "name,auditDDL,auditSecurity,auditAdmin,auditDML,auditSELECT" + 
			                  ",auditFailures,auditCaptureSQL" +
			                  ",auditDmlAll,auditUserTables,auditSystemTables" +
			                  ",auditStoredProcedures,auditDmlOther" +
			                  ",isEnabled,auditUsersList,auditDataChanges,auditSensitiveColumns" +
                           ",auditCaptureTrans,auditCaptureDDL,auditPrivUsersList,auditUserAll" +
                          ",auditUserLogins,auditUserLogouts,auditUserFailedLogins,auditUserDDL,auditUserDML,auditUserSecurity,auditUserAdmin,auditUserSELECT,auditUserUDE,auditUserFailures,auditUserCaptureSQL,auditUserCaptureDDL,auditUserCaptureTrans" +
                           " FROM {0}..{1}" +  
                           " WHERE dbId={2};";
            sql = String.Format( tmp,
                                 CoreConstants.RepositoryDatabase,
                                 CoreConstants.RepositoryDatabaseTable,
                                 dbId );
            using ( SqlCommand cmd = new SqlCommand( sql, conn ) )
            {
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
                  if ( reader.Read() )
                  {
		               int col=0;
                     Name                  = SQLHelpers.GetString(reader, col++);
                     AuditDDL              = SQLHelpers.ByteToBool( reader, col++);
                     AuditSecurity         = SQLHelpers.ByteToBool( reader, col++);
                     AuditAdmin            = SQLHelpers.ByteToBool( reader, col++);
                     AuditDML              = SQLHelpers.ByteToBool( reader, col++);
                     AuditSELECT           = SQLHelpers.ByteToBool( reader, col++);
                     AuditAccessCheck      = SQLHelpers.ByteToInt( reader, col++);
                     AuditCaptureSQL       = SQLHelpers.ByteToBool( reader, col++);
                     AuditDmlAll           = SQLHelpers.ByteToBool( reader, col++);
                     AuditUserTables       = SQLHelpers.ByteToInt( reader, col++ );
                     AuditSystemTables     = SQLHelpers.ByteToBool( reader, col++);
                     AuditStoredProcedures = SQLHelpers.ByteToBool( reader, col++);
                     AuditDmlOther         = SQLHelpers.ByteToBool( reader, col++);
                     IsEnabled             = SQLHelpers.ByteToBool( reader, col++);
                     AuditUsersList = SQLHelpers.GetString(reader, col++);
                     AuditDataChanges = SQLHelpers.ByteToBool(reader, col++);
                     AuditSensitiveColums = SQLHelpers.ByteToBool(reader, col++);
                     AuditCaptureTransactions = SQLHelpers.ByteToBool(reader, col++);
					 AuditCaptureDDL = SQLHelpers.ByteToBool(reader, col++);
                     AuditPrivUsersList = SQLHelpers.GetString(reader, col++);
                     AuditUserAll = SQLHelpers.ByteToBool(reader, col++);
                     AuditUserLogins = SQLHelpers.ByteToBool(reader, col++);
                     AuditUserLogouts = SQLHelpers.ByteToBool(reader, col++);
                     AuditUserFailedLogins = SQLHelpers.ByteToBool(reader, col++);
                     AuditUserDDL = SQLHelpers.ByteToBool(reader, col++);
                     AuditUserDML = SQLHelpers.ByteToBool(reader, col++);
                     AuditUserSecurity =SQLHelpers.ByteToBool(reader, col++);
                     AuditUserAdmin = SQLHelpers.ByteToBool(reader, col++);
                     AuditUserSELECT = SQLHelpers.ByteToBool(reader, col++);
                     AuditUserUDE = SQLHelpers.ByteToBool(reader, col++);
                     AuditUserAccessCheck = SQLHelpers.ByteToInt(reader, col++);
                     AuditUserCaptureSQL = SQLHelpers.ByteToBool(reader, col++);
                     AuditUserCaptureDDL = SQLHelpers.ByteToBool(reader, col++);
                     AuditUserCaptureTrans = SQLHelpers.ByteToBool(reader, col++);
                  }
               }
            }
         }
         catch(Exception ex)
         {
            ErrorLog.Instance.Write( "SnapshotDB->Read",
                                     sql,
                                     ex );
         }
		}
	}
	
	#endregion

	/// <summary>
	/// Summary description for Snapshot.
	/// </summary>
	public class Snapshot
	{
      //-------------------------------------------------------------
      // DumpAllServers - Workhorse for periodic snapshot job that
      //                  runs to capture all audit settings to
      //                  change log
      //-------------------------------------------------------------
		static public void
		   DumpAllServers(
		      SqlConnection     conn
		   )
		{
		   string       snapshot ;
         ICollection serverList = null;
         
         try
         {
            serverList = ServerRecord.GetServers( conn, true );
         }
         catch ( Exception ex)
         {
            ErrorLog.Instance.Write( "GetServers for SnapshotJob",
                                     ex );
         }

			if ( (serverList != null) && (serverList.Count != 0))
			{
		      foreach (ServerRecord srv in serverList )
		      {
               snapshot = ServerSnapshot( conn,
                                          srv,
                                          true );
               LogRecord.WriteLog( conn,
                                 LogType.Snapshot,
                                 srv.Instance,
                                 snapshot );                                   
            }                             
         }
		}
		
      #region ServerRecord snapshots

      //-------------------------------------------------------------
      // ServerSnapshot - String description of ServerRecord
      //--------------------------------------------------------------
	   public static string
	     ServerSnapshot(
           SqlConnection   conn,
	        ServerRecord     srv,
	        bool             includeDatabases
	     )
      {
         if ( srv==null )
         {
            return "";
         }
         
         StringBuilder snapshot = new StringBuilder(1024);
         
         snapshot.AppendFormat( "Audit Settings for SQL Server: {0}\r\n",    srv.Instance);
         snapshot.Append(       "\r\n" );

         snapshot.AppendFormat( "Auditing Status: {0}\r\n",srv.IsEnabled ? "Enabled" : "Disabled" );

         snapshot.AppendFormat( "Default database permissions: {0}\r\n",
                                GetDefaultAccess(srv.DefaultAccess) );
         
         // Server Level Audit Settings
         snapshot.Append(       "Audited Activities\r\n");
         snapshot.AppendFormat( "\tLogins: {0}\r\n",         srv.AuditLogins       ? "ON" : "OFF" );
         snapshot.AppendFormat("\tLogouts: {0}\r\n", srv.AuditLogouts ? "ON" : "OFF");
         snapshot.AppendFormat( "\tFailed Logins: {0}\r\n",  srv.AuditFailedLogins ? "ON" : "OFF" );
         snapshot.AppendFormat( "\tAdmin: {0}\r\n",          srv.AuditAdmin          ? "ON" : "OFF" );
         snapshot.AppendFormat( "\tDDL: {0}\r\n",            srv.AuditDDL          ? "ON" : "OFF" );
         snapshot.AppendFormat( "\tSecurity: {0}\r\n",       srv.AuditSecurity     ? "ON" : "OFF" );
         snapshot.AppendFormat( "\tUser Defined Events: {0}\r\n",       srv.AuditUDE     ? "ON" : "OFF" );
         snapshot.AppendFormat( "\tAccess Check Filter:  {0}\r\n", srv.AuditAccessCheck.ToString()) ;

         // Privileged Users
         snapshot.Append( "Privileged User Auditing\r\n");
         if ( srv.AuditUsersList == "" )
         {
            snapshot.Append( "\tNo audited privileged users\r\n" );
         }
         else if ( ! srv.IsEnabled )
         {
            snapshot.Append( "\tPrivileged user auditing disabled by server level audit status setting\r\n" );
         }
         else
         {
            snapshot.Append( GetUsersList( srv.AuditUsersList, false ) );
            snapshot.Append(          "\tAudit Settings:\r\n");
            snapshot.AppendFormat(    "\t\tAudit All Activity: {0}\r\n", srv.AuditUserAll       ? "ON" : "OFF" );
            if ( ! srv.AuditUserAll )
            {
               snapshot.AppendFormat( "\t\tLogins: {0}\r\n",         srv.AuditUserLogins       ? "ON" : "OFF" );
               snapshot.AppendFormat( "\t\tLogouts: {0}\r\n",         srv.AuditUserLogouts       ? "ON" : "OFF" );
               snapshot.AppendFormat( "\t\tFailed Logins: {0}\r\n",  srv.AuditUserFailedLogins ? "ON" : "OFF" );
               snapshot.AppendFormat( "\t\tDDL: {0}\r\n",            srv.AuditUserDDL          ? "ON" : "OFF" );
               snapshot.AppendFormat( "\t\tSecurity: {0}\r\n",       srv.AuditUserSecurity     ? "ON" : "OFF" );
               snapshot.AppendFormat( "\t\tAdmin: {0}\r\n",          srv.AuditUserAdmin        ? "ON" : "OFF" );
               snapshot.AppendFormat( "\t\tDML: {0}\r\n",            srv.AuditUserDML          ? "ON" : "OFF" );
               snapshot.AppendFormat( "\t\tSELECT: {0}\r\n",         srv.AuditUserSELECT       ? "ON" : "OFF" );
               snapshot.AppendFormat( "\t\tUser Defined Events: {0}\r\n",         srv.AuditUserUDE       ? "ON" : "OFF" );
               snapshot.AppendFormat( "\t\tAccess Check Filter:  {0}\r\n", srv.AuditUserAccessCheck.ToString()) ;
               snapshot.AppendFormat( "\t\tCapture SQL Statements: {0}\r\n",    srv.AuditUserCaptureSQL   ? "ON" : "OFF" );
			   snapshot.AppendFormat("\t\tCapture SQL Statements For DDL: {0}\r\n", srv.AuditUserCaptureDDL ? "ON" : "OFF");
               snapshot.AppendFormat( "\t\tCapture Transactions: {0}\r\n",      srv.AuditUserCaptureTrans ? "ON" : "OFF" );
            }
         }
         snapshot.Append( "\r\n");

        // Trusted Users //v5.6 SQLCM-5373
        snapshot.Append("Trusted Users\r\n");
         if (srv.AuditTrustedUsersList == "")
         {
             snapshot.AppendFormat("\tNo trusted users\r\n");
         }
         else
         {
             snapshot.AppendFormat(GetUsersList(srv.AuditTrustedUsersList, false));
         }

         // Databases         
         if ( includeDatabases )
         {
            if ( ! srv.IsEnabled )
            {
               snapshot.Append( "Database auditing disabled by server level audit status setting" );
            }
            else
            {
               string dbsettings = GetDatabases( conn, srv.SrvId );
               if ( dbsettings == "" )
                  snapshot.Append( "No audited databases associated with this SQL Server.");
               else
                  snapshot.Append( dbsettings );
            }
         }
         
		   return snapshot.ToString();
	   }
	   
      //---------------------------------------------------------------------------
      // GetPrivilegedUserList - loads server roles and users
      //---------------------------------------------------------------------------
		private static string
		   GetUsersList(
		      string auditUsersList,
		      bool   bExtraIndent
		   )
		{
		   bool firstLogin = true;
		   bool firstRole  = true;
		   
		   string extraIndent = (bExtraIndent) ? "\t" : "";
		   
		   StringBuilder users = new StringBuilder(1024);
		   
		   UserList userList = new UserList( auditUsersList );
		   
		   // Add logins
		   foreach ( Login l in userList.Logins )
		   {
		      if ( firstLogin )
		      {
               users.AppendFormat( "{0}\tLogins:\r\n", extraIndent);
               firstLogin = false;
		      }
            users.AppendFormat( "{0}\t\t{1}\r\n", extraIndent, l.Name );
         }

		   // Add server roles
		   foreach ( ServerRole r in userList.ServerRoles )
		   {
		      if ( firstRole )
		      {
               users.AppendFormat( "{0}\tRoles:\r\n", extraIndent);
               firstRole = false;
		      }
            users.AppendFormat( "{0}\t\t{1}\r\n", extraIndent, r.Name );
         }
         
         if ( users.Length == 0 )
         {
            users.AppendFormat( "{0}\tNone:\r\n", extraIndent);
         }
         
         return users.ToString();
		}
		
      //---------------------------------------------------------------------------
      // GetDatabases
      //---------------------------------------------------------------------------
		private static string
		   GetDatabases(
            SqlConnection   conn,
		      int             srvId
		   )
		{
		   StringBuilder db = new StringBuilder(1024);
		   
		   string         sql    = "";
		   
		   ArrayList dbs   = new ArrayList();
		   ArrayList names = new ArrayList();
		   int      count=0;
		   
		   try
		   {
		      string sqlfmt = "SELECT dbId,name from {0}..{1} where srvId={2}";
		      sql = String.Format( sqlfmt,
		                           CoreConstants.RepositoryDatabase,
		                           CoreConstants.RepositoryDatabaseTable,
		                           srvId );
            using ( SqlCommand cmd = new SqlCommand( sql, conn ) )
            {
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
                  while ( reader.Read() )
                  {
                     int    dbId = SQLHelpers.GetInt32(reader, 0);
                     string name = SQLHelpers.GetString(reader, 1);
                     
                     dbs.Add( dbId );
                     names.Add( name );
                     
                     count++;
                  }
               }
            }

            
            for (int i=0; i<count; i++)
            {
               db.Append( DatabaseSnapshot( conn,
                                            (int)dbs[i],
                                            (string)names[i],
                                            false ) );
               db.Append("\r\n");
            }
            
            if (db.Length==0)
            {
               db.Append( "No audited databases" );
               db.Append("\r\n");
            }            
		   }
		   catch (Exception ex )
		   {
            ErrorLog.Instance.Write( "GetDatabases",
                                     sql,
                                     ex );
		   }
         
         return db.ToString();
      }		
      
      #endregion
      
      #region Server ChangeLog

      //-------------------------------------------------------------
      // ServerChangeLog - Difference between two ServerRecords
      //--------------------------------------------------------------
	   public static string
	     ServerChangeLog(
	        ServerRecord     oldSrv,
	        ServerRecord     newSrv
	     )
      {
	      if ( oldSrv==null || newSrv==null ) return "";
         
         StringBuilder snapshot = new StringBuilder(1024);
         
         // Enabled / Disable
         if ( oldSrv.IsEnabled != newSrv.IsEnabled )
         {
            snapshot.AppendFormat( "Auditing Status:\r\n" );
            snapshot.AppendFormat( "\tOld: {0}\r\n\tNew: {1}\r\n",
                                   oldSrv.IsEnabled ? "Enabled" : "Disabled",
                                   newSrv.IsEnabled ? "Enabled" : "Disabled" );
         }
         
         // Default database access
         if ( oldSrv.DefaultAccess != newSrv.DefaultAccess )
         {
            snapshot.AppendFormat( "Default Database Access:\r\n" );
            snapshot.AppendFormat( "\tOld: {0}\r\n", GetDefaultAccess(oldSrv.DefaultAccess) );
            snapshot.AppendFormat( "\tNew: {0}\r\n", GetDefaultAccess(newSrv.DefaultAccess) );
         }
         
         // SQL Length
         if ( oldSrv.MaxSqlLength != newSrv.MaxSqlLength )
         {
            snapshot.AppendFormat( "Maximum SQL Length:\r\n" );
            snapshot.AppendFormat( "\tOld: {0}\r\n", (oldSrv.MaxSqlLength==-1) ? "Unlimited" : oldSrv.MaxSqlLength.ToString());
            snapshot.AppendFormat( "\tNew: {0}\r\n", (newSrv.MaxSqlLength==-1) ? "Unlimited" : newSrv.MaxSqlLength.ToString());
         }
         
         
         // Server Level Audit Settings
         StringBuilder auditSettings = new StringBuilder(1024);
         auditSettings.Append( GetOnOffFlag( "Logins",         oldSrv.AuditLogins, newSrv.AuditLogins ) );
         auditSettings.Append(GetOnOffFlag("Logouts", oldSrv.AuditLogouts, newSrv.AuditLogouts));
         auditSettings.Append( GetOnOffFlag( "Failed Logins",  oldSrv.AuditFailedLogins, newSrv.AuditFailedLogins ) );
         auditSettings.Append( GetOnOffFlag( "Admin",          oldSrv.AuditAdmin, newSrv.AuditAdmin ) );
         auditSettings.Append( GetOnOffFlag( "DDL",            oldSrv.AuditDDL, newSrv.AuditDDL ) );
         auditSettings.Append( GetOnOffFlag( "Security",       oldSrv.AuditSecurity, newSrv.AuditSecurity ) );
         auditSettings.Append( GetOnOffFlag( "User Defined Events",       oldSrv.AuditUDE, newSrv.AuditUDE ) );
         if(newSrv.AuditAccessCheck != oldSrv.AuditAccessCheck)
            auditSettings.AppendFormat("\tAccess Check Filter:\t\tOld: {0}   New: {1}\r\n", oldSrv.AuditAccessCheck.ToString(),   newSrv.AuditAccessCheck.ToString());

         if ( auditSettings.Length != 0 )
         {         
            snapshot.Append( "Audited Activities\r\n");
            snapshot.Append( auditSettings.ToString() ); 
         }

         //v5.6 SQLCM-5373
         StringBuilder trustedUsers = new StringBuilder(1024);
         if (!UserList.Match(oldSrv.AuditTrustedUsersList, newSrv.AuditTrustedUsersList))
         {
             // names
             if (oldSrv.AuditTrustedUsersList != newSrv.AuditTrustedUsersList)
             {
                 trustedUsers.Append("\tOld Trusted User List\r\n");
                 trustedUsers.Append(GetUsersList(oldSrv.AuditTrustedUsersList, true));
                 trustedUsers.Append("\tNew Trusted User List\r\n");
                 trustedUsers.Append(GetUsersList(newSrv.AuditTrustedUsersList, true));
             }
         }
         if (trustedUsers.Length != 0)
         {
             snapshot.Append("Trusted Users\r\n");
             snapshot.Append(trustedUsers.ToString());
         }

         // Privileged Users
         StringBuilder privUsers = new StringBuilder(1024);
         
         if (!UserList.Match(oldSrv.AuditUsersList, newSrv.AuditUsersList))
         {
            // names
            if ( oldSrv.AuditUsersList != newSrv.AuditUsersList )
            {
               privUsers.Append( "\tOld Privileged User List\r\n" );
               privUsers.Append( GetUsersList( oldSrv.AuditUsersList, true ) );
               privUsers.Append( "\tNew Privileged User List\r\n" );
               privUsers.Append( GetUsersList( newSrv.AuditUsersList, true ) );
            }
            
            if ( oldSrv.AuditUserAll && ! newSrv.AuditUserAll )
            {
               privUsers.Append( "\tAudit all privileged user activity turned OFF\r\n" );
               privUsers.Append( "\tNew Values\r\n" );
               privUsers.AppendFormat( "\t\tLogins\t{0}\r\n",          newSrv.AuditUserLogins       ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tLogouts\t{0}\r\n",         newSrv.AuditUserLogouts       ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tFailed Logins\t{0}\r\n",   newSrv.AuditUserFailedLogins ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tDDL\t{0}\r\n",             newSrv.AuditUserDDL ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tSecurity\t{0}\r\n",        newSrv.AuditUserSecurity ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tAdmin\t{0}\r\n",           newSrv.AuditUserAdmin ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tDML\t{0}\r\n",             newSrv.AuditUserDML ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tSELECT\t{0}\r\n",          newSrv.AuditUserSELECT ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tUser Defined Events\t{0}\r\n",          newSrv.AuditUserUDE ? "ON" : "OFF" );
               privUsers.AppendFormat("\t\tAccess Check Filter:\t{0}\r\n", newSrv.AuditUserAccessCheck.ToString());
               privUsers.AppendFormat( "\t\tCapture SQL Statements\t{0}\r\n",    newSrv.AuditUserCaptureSQL ? "ON" : "OFF" );
			   privUsers.AppendFormat("\t\tCapture SQL Statements For DDL\t{0}\r\n", newSrv.AuditUserCaptureDDL ? "ON" : "OFF");
               privUsers.AppendFormat("\t\tCapture Transactions: {0}\r\n", newSrv.AuditUserCaptureTrans ? "ON" : "OFF");

            }
            else if (  ! oldSrv.AuditUserAll && newSrv.AuditUserAll )
            {
               privUsers.Append( "\tAudit all privileged user activity turned ON\r\n" );
               privUsers.Append( "\tOld Values\r\n" );
               privUsers.AppendFormat( "\t\tLogins\t{0}\r\n",          oldSrv.AuditUserLogins       ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tLogouts\t{0}\r\n",         oldSrv.AuditUserLogouts       ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tFailed Logins\t{0}\r\n",   oldSrv.AuditUserFailedLogins ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tDDL\t{0}\r\n",             oldSrv.AuditUserDDL ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tSecurity\t{0}\r\n",        oldSrv.AuditUserSecurity ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tAdmin\t{0}\r\n",           oldSrv.AuditUserAdmin ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tDML\t{0}\r\n",             oldSrv.AuditUserDML ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tSELECT\t{0}\r\n",          oldSrv.AuditUserSELECT ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tUser Defined Events\t{0}\r\n",          oldSrv.AuditUserUDE ? "ON" : "OFF" );
               privUsers.AppendFormat("\t\tAccess Check Filter:\t{0}\r\n", oldSrv.AuditUserAccessCheck.ToString());
               privUsers.AppendFormat( "\t\tCapture SQL Statements\t{0}\r\n",    oldSrv.AuditUserCaptureSQL ? "ON" : "OFF" );
			   privUsers.AppendFormat("\t\tCapture SQL Statements For DDL: {0}\r\n", oldSrv.AuditUserCaptureDDL ? "ON" : "OFF");
               privUsers.AppendFormat("\t\tCapture Transactions: {0}\r\n", oldSrv.AuditUserCaptureTrans ? "ON" : "OFF");
            }
            else if ( oldSrv.AuditUsersList == "")
            {
               privUsers.Append( "\tAudited Privileged User Activity\r\n" );
               privUsers.AppendFormat( "\t\tLogins\t{0}\r\n",          newSrv.AuditUserLogins       ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tLogouts\t{0}\r\n",         newSrv.AuditUserLogouts       ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tFailed Logins\t{0}\r\n",   newSrv.AuditUserFailedLogins ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tDDL\t{0}\r\n",             newSrv.AuditUserDDL ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tSecurity\t{0}\r\n",        newSrv.AuditUserSecurity ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tAdmin\t{0}\r\n",           newSrv.AuditUserAdmin ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tDML\t{0}\r\n",             newSrv.AuditUserDML ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tSELECT\t{0}\r\n",          newSrv.AuditUserSELECT ? "ON" : "OFF" );
               privUsers.AppendFormat( "\t\tUser Defined Events\t{0}\r\n",          newSrv.AuditUserUDE ? "ON" : "OFF" );
               privUsers.AppendFormat("\t\tAccess Check Filter:\t{0}\r\n", newSrv.AuditUserAccessCheck.ToString());
               privUsers.AppendFormat( "\t\tCapture SQL Statements\t{0}\r\n",    newSrv.AuditUserCaptureSQL ? "ON" : "OFF" );
			   privUsers.AppendFormat("\t\tCapture SQL Statements For DDL \t{0}\r\n", newSrv.AuditUserCaptureDDL ? "ON" : "OFF");
               privUsers.AppendFormat("\t\tCapture Transactions: {0}\r\n", newSrv.AuditUserCaptureTrans ? "ON" : "OFF");
            }
            else
            {
               privUsers.Append( GetOnOffFlag( "\tLogins",         oldSrv.AuditUserLogins,       newSrv.AuditUserLogins ) );
               privUsers.Append( GetOnOffFlag( "\tLogouts",        oldSrv.AuditUserLogouts,       newSrv.AuditUserLogouts ) );
               privUsers.Append( GetOnOffFlag( "\tFailed Logins",  oldSrv.AuditUserFailedLogins, newSrv.AuditUserFailedLogins ) );
               privUsers.Append( GetOnOffFlag( "\tDDL",            oldSrv.AuditUserDDL,          newSrv.AuditUserDDL ) );
               privUsers.Append( GetOnOffFlag( "\tSecurity",       oldSrv.AuditUserSecurity,     newSrv.AuditUserSecurity ) );
               privUsers.Append( GetOnOffFlag( "\tAdmin",          oldSrv.AuditUserAdmin,        newSrv.AuditUserAdmin ) );
               privUsers.Append( GetOnOffFlag( "\tDML",            oldSrv.AuditUserDML,          newSrv.AuditUserDML ) );
               privUsers.Append( GetOnOffFlag( "\tSELECT",         oldSrv.AuditUserSELECT,       newSrv.AuditUserSELECT ) );
               privUsers.Append( GetOnOffFlag( "\tUser Defined Events",         oldSrv.AuditUserUDE,       newSrv.AuditUserUDE ) );
               if(oldSrv.AuditUserAccessCheck != newSrv.AuditUserAccessCheck)
                  privUsers.AppendFormat("\t\tAccess Check Filter:\tOld: {0}   New: {1}\r\n", oldSrv.AuditUserAccessCheck.ToString(), newSrv.AuditUserAccessCheck.ToString());
               privUsers.Append( GetOnOffFlag( "\tCapture SQL Statements",    oldSrv.AuditUserCaptureSQL,   newSrv.AuditUserCaptureSQL ) );
			   privUsers.Append(GetOnOffFlag("\tCapture SQL Statements For DDL", oldSrv.AuditUserCaptureDDL, newSrv.AuditUserCaptureDDL));
               privUsers.Append(GetOnOffFlag("\tCapture Transactions", oldSrv.AuditUserCaptureTrans, newSrv.AuditUserCaptureTrans));
            }
         }
         else // check if priv user settings are changed
         {
            if( oldSrv.AuditUserLogins != newSrv.AuditUserLogins )
               privUsers.Append( GetOnOffFlag( "\tLogins",         oldSrv.AuditUserLogins,       newSrv.AuditUserLogins ) );
            if( oldSrv.AuditUserLogouts != newSrv.AuditUserLogouts )
               privUsers.Append( GetOnOffFlag( "\tLogouts",         oldSrv.AuditUserLogouts,       newSrv.AuditUserLogouts) );
            if (oldSrv.AuditUserFailedLogins != newSrv.AuditUserFailedLogins)
               privUsers.Append(GetOnOffFlag("\tFailed Logins", oldSrv.AuditUserFailedLogins, newSrv.AuditUserFailedLogins));
            if (oldSrv.AuditUserDDL != newSrv.AuditUserDDL)
               privUsers.Append(GetOnOffFlag("\tDDL", oldSrv.AuditUserDDL, newSrv.AuditUserDDL));
            if (oldSrv.AuditUserSecurity != newSrv.AuditUserSecurity)
               privUsers.Append(GetOnOffFlag("\tSecurity", oldSrv.AuditUserSecurity, newSrv.AuditUserSecurity));
            if (oldSrv.AuditUserAdmin != newSrv.AuditUserAdmin)
               privUsers.Append(GetOnOffFlag("\tAdmin", oldSrv.AuditUserAdmin, newSrv.AuditUserAdmin));
            if (oldSrv.AuditUserDML != newSrv.AuditUserDML)
               privUsers.Append(GetOnOffFlag("\tDML", oldSrv.AuditUserDML, newSrv.AuditUserDML));
            if (oldSrv.AuditUserSELECT != newSrv.AuditUserSELECT)
               privUsers.Append(GetOnOffFlag("\tSELECT", oldSrv.AuditUserSELECT, newSrv.AuditUserSELECT));
            if (oldSrv.AuditUserUDE != newSrv.AuditUserUDE)
               privUsers.Append(GetOnOffFlag("\tUser Defined Events", oldSrv.AuditUserUDE, newSrv.AuditUserUDE));
            if(oldSrv.AuditUserAccessCheck != newSrv.AuditUserAccessCheck)
               privUsers.AppendFormat("\t\tAccess Check Filter:\tOld: {0}   New: {1}\r\n", oldSrv.AuditUserAccessCheck.ToString(), newSrv.AuditUserAccessCheck.ToString());
            if (oldSrv.AuditUserCaptureSQL != newSrv.AuditUserCaptureSQL)
               privUsers.Append(GetOnOffFlag("\tCapture SQL Statements", oldSrv.AuditUserCaptureSQL, newSrv.AuditUserCaptureSQL));
		   if (oldSrv.AuditUserCaptureDDL != newSrv.AuditUserCaptureDDL)
                    privUsers.Append(GetOnOffFlag("\tCapture SQL Statements For DDL", oldSrv.AuditUserCaptureDDL, newSrv.AuditUserCaptureDDL));
            if (oldSrv.AuditUserCaptureTrans != newSrv.AuditUserCaptureTrans)
               privUsers.Append(GetOnOffFlag("\tCapture Transactions", oldSrv.AuditUserCaptureTrans, newSrv.AuditUserCaptureTrans));
         }

         if ( privUsers.Length != 0 )
         {         
            snapshot.Append( "Privileged User Auditing\r\n");
            snapshot.Append( privUsers.ToString() ); 
         }
         
         if (snapshot.Length == 0 )
         {
            snapshot.Append( "No changes to audit settings.");
         }
         if(oldSrv.Description != newSrv.Description)
         {
            snapshot.AppendFormat( "\r\n\r\nServer Description:\r\n" );
            snapshot.AppendFormat( "\tOld: {0}\r\n", oldSrv.Description);
            snapshot.AppendFormat( "\tNew: {0}\r\n", newSrv.Description) ;
         }
         
         // Add Heading         
         string hdg = String.Format( "Audit Setting Changes for SQL Server: {0}\r\n\r\n",
                                      newSrv.Instance);
         snapshot.Insert(0,hdg);
         
         
		   return snapshot.ToString();
	   }
	   
	   public static string
	      GetDefaultAccess(
	         int defaultAccess
         )
	   {
         string acc;
         if (defaultAccess == 0)
            acc = "Deny read access to events and SQL statements";
         else if (defaultAccess == 1)
            acc = "Grant right to read events only";
         else 
            acc = "Grant right to read events and SQL statements";
            
         return acc;   
	   }

      //---------------------------------------------------------------------------
      // GetOnOffDelta - Utility routine
      //---------------------------------------------------------------------------
      static private string
         GetOnOffFlag(
            string  valueName,
            bool    oldValue,
            bool    newValue
         )
      {
         return GetOnOffFlag( valueName, oldValue, newValue, false );
      }

      static private string
         GetOnOffFlag(
            string  valueName,
            bool    oldValue,
            bool    newValue,
            bool    forceWrite
         )
      {
         string flag = "";
         if ( forceWrite || (oldValue != newValue) )
         {
            flag = String.Format( "\t{0}:\t\tOld: {1}   New: {2}\r\n",
                                  valueName,
                                  oldValue ? "ON" : "OFF",
                                  newValue ? "ON" : "OFF" );
         }
         return flag;
      }
	   
	   
	   #endregion
      
      #region Database snapshots
      
      //-------------------------------------------------------------
      // DatabaseSnapshot
      //--------------------------------------------------------------
      static public string      
         DatabaseSnapshot(
            SqlConnection     conn,
            int               dbId,
            string            databaseName,
            bool              dbSnapshotOnly
         )
      {
         StringBuilder snapshot = new StringBuilder(1024);
         
         SnapshotDB db = new SnapshotDB();
         db.Read( conn, dbId );

         snapshot.AppendFormat( "Audit Settings for Database: {0}\r\n", db.Name);
            
         snapshot.AppendFormat( "Auditing Status: {0}\r\n", db.IsEnabled ? "Enabled" : "Disabled");
         snapshot.Append(       "Audited Activities:\r\n");
         snapshot.AppendFormat( "\tDDL: {0}\r\n",            db.AuditDDL          ? "ON" : "OFF" );
         snapshot.AppendFormat( "\tSecurity: {0}\r\n",       db.AuditSecurity     ? "ON" : "OFF" );
         snapshot.AppendFormat( "\tAdmin: {0}\r\n",          db.AuditAdmin        ? "ON" : "OFF" );
         snapshot.AppendFormat( "\tDML: {0}\r\n",            db.AuditDML          ? "ON" : "OFF" );
         snapshot.AppendFormat( "\tSELECT: {0}\r\n",         db.AuditSELECT       ? "ON" : "OFF" );
         snapshot.AppendFormat("\tAccess Check Filter:  {0}\r\n", ((AccessCheckFilter)db.AuditAccessCheck).ToString());

         if (db.AuditDML)
         {
            snapshot.AppendFormat("\tCapture Transactions: {0}\r\n", db.AuditCaptureTransactions ? "ON" : "OFF");
         }
		 if (db.AuditDML || db.AuditDDL)
            {
                snapshot.AppendFormat("\tCapture SQL Statements For DDL: {0}\r\n", db.AuditCaptureDDL ? "ON" : "OFF");
            }

         if ( db.AuditDML || db.AuditSELECT )
         { 
            snapshot.AppendFormat( "\tCapture SQL Statements: {0}\r\n",    db.AuditCaptureSQL   ? "ON" : "OFF" );
         
            snapshot.Append(       "DML/SELECT Filtering:\r\n");
            
            if ( db.AuditDmlAll )
            {
               snapshot.AppendFormat( "\tDML and SELECT activities on all object types will be captured.\r\n");
            }
            else
            {
               snapshot.AppendFormat( "\tUser Tables: {0}\r\n",
                                    GetUserTableSetting(db.AuditUserTables) );
               if ( db.AuditUserTables == 2 )
               {
                  string tables = GetDatabaseTables( conn, dbId, "\t\t" );
                  if ( tables == "" )
                     snapshot.Append( "\t\tNo user tables specified");
                  else
                     snapshot.Append( tables );
               }
               
               snapshot.AppendFormat( "\tSystem Tables: {0}\r\n",     db.AuditSystemTables     ? "ON" : "OFF" );
               snapshot.AppendFormat( "\tStored Procedures: {0}\r\n", db.AuditStoredProcedures ? "ON" : "OFF" );
               snapshot.AppendFormat( "\tOther Object Types (Indexes, Views, etc): {0}\r\n",             db.AuditDmlOther         ? "ON" : "OFF" );
            }

            // Before-After Data Auditing
            snapshot.AppendFormat("DML Before-After Auditing: {0}\r\n", db.AuditDataChanges ? "ON" : "OFF");
            if (db.AuditDataChanges)
            {
               string tables = GetDataChangeTables(conn, dbId, "\t");
               if (tables == "")
                  snapshot.Append("\tNo before-after tables specified\r\n");
               else
                  snapshot.Append(tables);
            }
         }
         snapshot.AppendFormat("Sensitive Column Auditing: {0}\r\n", db.AuditSensitiveColums ? "ON" : "OFF");

         if (db.AuditSensitiveColums)
         {
            string tables = GetSensitiveColumnTables(conn, dbId, "\t");

            if (tables == "")
               snapshot.Append("\tNo Sensitive Column tables specified\r\n");
            else
               snapshot.Append(tables);
         }


         // Trusted Users
         snapshot.Append("Trusted Users\r\n");
         if (db.AuditUsersList == "")
         {
            snapshot.Append("\tNo trusted users\r\n");
         }
         else
         {
            snapshot.Append(GetUsersList(db.AuditUsersList, false));
         }
		 // Privilged Users
            snapshot.Append("Privilged Users\r\n");
            if (db.AuditPrivUsersList == "")
            {
                snapshot.Append("\tNo Privilged users\r\n");
            }
            else
            {
                snapshot.Append(GetUsersList(db.AuditPrivUsersList, false));                
                snapshot.AppendFormat("\t\tAudit All Activity: {0}\r\n", db.AuditUserAll? "ON" : "OFF");
                if (!db.AuditUserAll)
                {
                 

                    snapshot.AppendFormat("\t\tLogins: {0}\r\n", db.AuditUserLogins ? "ON" : "OFF");
                    snapshot.AppendFormat("\t\tLogouts: {0}\r\n", db.AuditUserLogouts ? "ON" : "OFF");
                    snapshot.AppendFormat("\t\tFailed Logins: {0}\r\n", db.AuditUserFailedLogins ? "ON" : "OFF");
                    snapshot.AppendFormat("\t\tDDL: {0}\r\n", db.AuditUserDDL ? "ON" : "OFF");
                    snapshot.AppendFormat("\t\tDML: {0}\r\n", db.AuditUserDML ? "ON" : "OFF");
                    snapshot.AppendFormat("\t\tSecurity: {0}\r\n", db.AuditUserSecurity ? "ON" : "OFF");
                    snapshot.AppendFormat("\t\tAdmin: {0}\r\n", db.AuditUserAdmin ? "ON" : "OFF");
                    snapshot.AppendFormat("\t\tSELECT: {0}\r\n", db.AuditUserSELECT ? "ON" : "OFF");
                    snapshot.AppendFormat("\t\tUser Defined Events: {0}\r\n", db.AuditUserUDE ? "ON" : "OFF");
                    //snapshot.AppendFormat("\tAccess Check Filter:  {0}\r\n", ((AccessCheckFilter)db.AuditAccessCheck).ToString());
                    snapshot.AppendFormat("\t\tAccess Check Filter:  {0}\r\n", ((AccessCheckFilter)db.AuditUserAccessCheck).ToString());
                    snapshot.AppendFormat("\t\tCapture SQL Statements: {0}\r\n", db.AuditUserCaptureSQL ? "ON" : "OFF");
                    snapshot.AppendFormat("\t\tCapture SQL Statements For DDL: {0}\r\n", db.AuditUserCaptureDDL ? "ON" : "OFF");
                    snapshot.AppendFormat("\t\tCapture Transactions: {0}\r\n", db.AuditUserCaptureTrans ? "ON" : "OFF");
                }
            }

		return snapshot.ToString();
      }

      //-------------------------------------------------------------
      // GetDatabaseTables
      //--------------------------------------------------------------
      static public string
         GetDatabaseTables(
            SqlConnection     conn,
            int               dbId,
            string            prefix
         )
      {
         string         sql    = "";
         StringBuilder  tables = new StringBuilder(1024);
         
         try
         {
            string sqlfmt = "SELECT name,schemaName from {0}..{1} WHERE dbId={2};";
            sql = String.Format( sqlfmt,
                                 CoreConstants.RepositoryDatabase,
                                 CoreConstants.RepositoryDatabaseObjectsTable,
                                 dbId );
            using ( SqlCommand cmd = new SqlCommand( sql, conn ) )
            {
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
                  while ( reader.Read() )
                  {
                     tables.AppendFormat( "{0}{1}.{2}\r\n",
                                          prefix,
                                          SQLHelpers.GetString(reader, 1),
                                          SQLHelpers.GetString( reader, 0 ) );
                  }
               }
            }
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write( "Fetching user tables",
                                     sql,
                                     ex );
         }
         
         return tables.ToString();
      }

      //-------------------------------------------------------------
      // GetDataChangeTables
      //--------------------------------------------------------------
      static public string GetDataChangeTables(SqlConnection conn,int dbId,string prefix)
      {
         string sql = "";
         StringBuilder tablesStr = new StringBuilder(1024);

         try
         {
            List<DataChangeTableRecord> tables = DataChangeTableRecord.GetAuditedTables(conn, dbId);

            foreach(DataChangeTableRecord tbl in tables)
            {
               string rowLimitStr = String.Format("{0} rows", tbl.RowLimit == -1 ? "All" : tbl.RowLimit.ToString());

               tablesStr.AppendFormat("{0}{1} ({2})\r\n{0}{0}{3}\r\n",
                                          prefix,
                                          tbl.FullTableName,
                                          rowLimitStr,
                                          tbl.ColumnNames);
            }
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write("Fetching data change tables",
                                     sql,
                                     ex);
         }

         return tablesStr.ToString();
      }

      static public string GetSensitiveColumnTables(SqlConnection conn, int dbid, string prefix)
      {
         string sql = "";
         StringBuilder tablesStr = new StringBuilder(1024);

         try
         {
            List<SensitiveColumnTableRecord> tables = SensitiveColumnTableRecord.GetAuditedTables(conn, dbid);
            if (tables != null)
            {
                foreach (SensitiveColumnTableRecord tbl in tables)
                {
                    tablesStr.AppendFormat("{0}{1} \r\n{0}{0}{2}\r\n",
                                               prefix,
                                               tbl.FullTableName,
                                               tbl.ColumnNames);
                }
            }
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write("Fetching Sensitive Column tables", sql, ex);
         }
         return tablesStr.ToString();
      }
      
      #endregion
      
      #region Database ChangeLog

      //-------------------------------------------------------------
      // DatabaseChangeLog - Difference between two ServerRecords
      //--------------------------------------------------------------
	   public static string
	     DatabaseChangeLog(
	        SqlConnection      conn,
	        DatabaseRecord     oldDb,
	        DatabaseRecord     newDb,
	        string             oldTables,
           string             oldDCTables,
           string             oldSCTables
	     )
      {
	      string        newTables = "";
         
         if ( oldDb==null || newDb==null ) return "";
         
         StringBuilder snapshot = new StringBuilder(1024);

         // Description
         if ( oldDb.Description != newDb.Description )
         {
            snapshot.AppendFormat( "Database Description:\r\n" );
            snapshot.AppendFormat( "\tOld: {0}\r\n\tNew: {1}\r\n",
                                   oldDb.Description,
                                   newDb.Description);
         }
         
         // Enabled / Disable
         if ( oldDb.IsEnabled != newDb.IsEnabled )
         {
            snapshot.AppendFormat( "Auditing Status:\r\n" );
            snapshot.AppendFormat( "\tOld: {0}   New: {1}\r\n",
                                   oldDb.IsEnabled ? "Enabled" : "Disabled",
                                   newDb.IsEnabled ? "Enabled" : "Disabled" );
         }
         
         // Audit Settings
         StringBuilder auditSettings = new StringBuilder(1024);
         auditSettings.Append( GetOnOffFlag( "DDL",            oldDb.AuditDDL,        newDb.AuditDDL ) );
         auditSettings.Append( GetOnOffFlag( "Security",       oldDb.AuditSecurity,   newDb.AuditSecurity ) );
         auditSettings.Append( GetOnOffFlag( "Admin",          oldDb.AuditAdmin,      newDb.AuditAdmin ) );
         auditSettings.Append( GetOnOffFlag( "DML",            oldDb.AuditDML,        newDb.AuditDML ) );
         auditSettings.Append( GetOnOffFlag( "SELECT",         oldDb.AuditSELECT,     newDb.AuditSELECT ) );
         if(newDb.AuditAccessCheck != oldDb.AuditAccessCheck)
            auditSettings.AppendFormat("\tAccess Check Filter:\t\tOld: {0}   New: {1}\r\n", oldDb.AuditAccessCheck.ToString(),   newDb.AuditAccessCheck.ToString());

         if ( newDb.AuditDML || newDb.AuditSELECT )
         {
            auditSettings.Append( GetOnOffFlag( "Capture SQL Statements",    oldDb.AuditCaptureSQL, newDb.AuditCaptureSQL ) );
         }
		 if (newDb.AuditDDL)
         {
            auditSettings.Append(GetOnOffFlag("Capture SQL Statements For DDL", oldDb.AuditCaptureDDL, newDb.AuditCaptureDDL));
         }

         if (newDb.AuditDML)
         {
            auditSettings.Append(GetOnOffFlag("Capture Transactions", oldDb.AuditCaptureTrans, newDb.AuditCaptureTrans));
         }

         if ( auditSettings.Length != 0 )
         {         
            snapshot.Append( "Audited Activities\r\n");
            snapshot.Append( auditSettings.ToString() ); 
         }
         
         // DML Filtering
         if ( newDb.AuditDML || newDb.AuditSELECT || oldDb.AuditDML || oldDb.AuditSELECT )
         {
            StringBuilder auditedObjects = new StringBuilder(1024);
            
            if ( !oldDb.AuditDML && !oldDb.AuditSELECT )
            {
               // DML/SELECT filtering was just turned on - just dump the new values
               if ( newDb.AuditDmlAll )
               {
                  auditedObjects.Append( "\tDML/SELECT auditing of all objects types: ON\r\n");
               }
               else
               {
                  auditedObjects.Append( "\tDML/SELECT auditing:\r\n");
               
                  // User Tables
                  auditedObjects.AppendFormat( "\tUser Tables: {0}\r\n",
                                             GetUserTableSetting(newDb.AuditUserTables) );
                  if ( newTables != "" )
                  {
                     auditedObjects.Append( newTables );
                  }
                  
                  // other object types   
                  auditedObjects.AppendFormat( "\tSystem Tables: {0}\r\n",     newDb.AuditSystemTables     ? "ON" : "OFF" );
                  auditedObjects.AppendFormat( "\tStored Procedures: {0}\r\n", newDb.AuditStoredProcedures ? "ON" : "OFF" );
                  auditedObjects.AppendFormat( "\tOther Object Types (Indexes, Views, etc): {0}\r\n", newDb.AuditDmlOther         ? "ON" : "OFF" );
               }
            }
            else
            {
               if ( newDb.AuditDmlAll )
               {
                  if ( ! oldDb.AuditDmlAll )
                  {
                     // NEW: ALL  OLD: SELECTED
                     auditedObjects.Append( "\tDML/SELECT auditing of all objects types turned ON\r\n");
                     auditedObjects.Append( "\tOld Values:\r\n");
                     
                     // User Tables
                     auditedObjects.AppendFormat( "\tUser Tables: {0}\r\n",
                                                GetUserTableSetting(oldDb.AuditUserTables) );
                     if (oldTables=="" && oldDb.AuditUserTables==2)
                        oldTables = "\t\tNo user tables specified";
                     if ( oldTables != "" )
                        auditedObjects.Append( oldTables );
                     
                     // other object types   
                     auditedObjects.AppendFormat( "\tSystem Tables: {0}\r\n",     oldDb.AuditSystemTables     ? "ON" : "OFF" );
                     auditedObjects.AppendFormat( "\tStored Procedures: {0}\r\n", oldDb.AuditStoredProcedures ? "ON" : "OFF" );
                     auditedObjects.AppendFormat( "\tOther Object Types (Indexes, Views, etc): {0}\r\n", oldDb.AuditDmlOther         ? "ON" : "OFF" );
                  }
                  //else - no change so nothing to add to changelog snapshot
                  //    NEW: ALL  OLD: ALL
               }
               else
               {
                  if ( newDb.AuditUserTables==2 )
                  {
                     newTables = GetDatabaseTables( conn,
                                                      newDb.DbId,
                                                      "\t\t" );
                     if (newTables=="") newTables = "\t\tNo user tables specified";
                  }
                  else if ( newDb.AuditUserTables==1 )
                  {
                     newTables = "\t\tAll User Tables\r\n";
                  }
               
                  if ( oldDb.AuditDmlAll )
                  {
                     // NEW: SELECTED  OLD: ALL
                     auditedObjects.Append( "\tDML/SELECT auditing of all objects types turned OFF\r\n");
                     auditedObjects.Append( "\tNew Values:\r\n");
                     
                     // User Tables
                     auditedObjects.AppendFormat( "\tUser Tables: {0}\r\n",
                                                GetUserTableSetting(newDb.AuditUserTables) );
                     if ( newTables != "" )
                        auditedObjects.Append( newTables );
                     
                     // other object types   
                     auditedObjects.AppendFormat( "\tSystem Tables: {0}\r\n",     newDb.AuditSystemTables     ? "ON" : "OFF" );
                     auditedObjects.AppendFormat( "\tStored Procedures: {0}\r\n", newDb.AuditStoredProcedures ? "ON" : "OFF" );
                     auditedObjects.AppendFormat( "\tOther Object Types (Indexes, Views, etc): {0}\r\n", newDb.AuditDmlOther         ? "ON" : "OFF" );
                  }
                  else
                  {
                     // NEW: SELECTED  OLD: SELECTED
                     if (    ( oldDb.AuditUserTables != newDb.AuditUserTables )
                        || ( oldDb.AuditUserTables == 2  && (oldTables != newTables) )
                        )
                     {
                        auditedObjects.AppendFormat( "\tUser Tables: Old: {0}  New: {1}\r\n",
                                                   GetUserTableSetting(oldDb.AuditUserTables),
                                                   GetUserTableSetting(newDb.AuditUserTables) );
                        if ( oldDb.AuditUserTables == 0 )
                        {
                           auditedObjects.Append( "\tNew Tables:\r\n" );
                           auditedObjects.Append( newTables );
                        }
                        else if ( newDb.AuditUserTables == 0 )
                        {
                           auditedObjects.Append( "\tOld Tables:\r\n" );
                           auditedObjects.Append( oldTables );
                        }
                        else
                        {
                           if ( oldDb.AuditUserTables == 1 ) oldTables = "\t\tAll User Tables\r\n";
                           if ( newDb.AuditUserTables == 1 ) newTables = "\t\tAll User Tables\r\n";

                           auditedObjects.Append( "\tOld Tables:\r\n" );
                           auditedObjects.Append( oldTables );
                           auditedObjects.Append( "\tNew Tables:\r\n" );
                           auditedObjects.Append( newTables );
                        }
                     }
                  
                     auditedObjects.Append( GetOnOffFlag( "System Tables",     oldDb.AuditSystemTables,     newDb.AuditSystemTables ) );
                     auditedObjects.Append( GetOnOffFlag( "Stored Procedures", oldDb.AuditStoredProcedures, newDb.AuditStoredProcedures ) );
                     auditedObjects.Append( GetOnOffFlag( "Other objects(indexes,views etc):",             oldDb.AuditDmlOther,         newDb.AuditDmlOther ) );
                  }
               }
            }
            if ( auditedObjects.Length != 0 )
            {         
               snapshot.Append( "Audited Objects\r\n");
               snapshot.Append( auditedObjects.ToString() ); 
            }
         }

         string newDCTables = GetDataChangeTables(conn, newDb.DbId, "\t\t");
         if (oldDb.AuditDataChanges != newDb.AuditDataChanges ||
            !newDCTables.Equals(oldDCTables))
         {
            snapshot.Append(GetOnOffFlag("DML Before-After Auditing", oldDb.AuditDataChanges, newDb.AuditDataChanges, true));
            snapshot.Append("\tOld Tables:\r\n");
            if (String.IsNullOrEmpty(oldDCTables))
               snapshot.Append("\t\tNo tables specified\r\n");
            else
               snapshot.Append(oldDCTables);
            snapshot.Append("\tNew Tables:\r\n");
            if(String.IsNullOrEmpty(newDCTables))
               snapshot.Append("\t\tNo tables specified\r\n");
            else
               snapshot.Append(newDCTables);
         }

         string newSCTables = GetSensitiveColumnTables(conn, newDb.DbId, "\t\t");
         if (oldDb.AuditSensitiveColumns != newDb.AuditSensitiveColumns ||
            !newSCTables.Equals(oldSCTables))
         {
            snapshot.Append(GetOnOffFlag("Sensitive Column Auditing", oldDb.AuditSensitiveColumns, newDb.AuditSensitiveColumns, true));
            snapshot.Append("\tOld Tables:\r\n");
            if (String.IsNullOrEmpty(oldSCTables))
               snapshot.Append("\t\tNo tables specified\r\n");
            else
               snapshot.Append(oldSCTables);
            snapshot.Append("\tNew Tables:\r\n");
            if (String.IsNullOrEmpty(newSCTables))
               snapshot.Append("\t\tNo tables specified\r\n");
            else
               snapshot.Append(newSCTables);
         }
         
         StringBuilder trustedUsers = new StringBuilder(1024);
         if (!UserList.Match(oldDb.AuditUsersList, newDb.AuditUsersList))
         {
            // names
            if (oldDb.AuditUsersList != newDb.AuditUsersList)
            {
               trustedUsers.Append("\tOld Trusted User List\r\n");
               trustedUsers.Append(GetUsersList(oldDb.AuditUsersList, true));
               trustedUsers.Append("\tNew Trusted User List\r\n");
               trustedUsers.Append(GetUsersList(newDb.AuditUsersList, true));
            }
         }
         if (trustedUsers.Length != 0)
         {
            snapshot.Append("Trusted Users\r\n");
            snapshot.Append(trustedUsers.ToString());
         }

         // Privileged Users
         StringBuilder privUsers = new StringBuilder(1024);

         if (!UserList.Match(oldDb.AuditPrivUsersList, newDb.AuditPrivUsersList))
         {
             // names
             if (oldDb.AuditPrivUsersList != newDb.AuditPrivUsersList)
             {
                 privUsers.Append("\tOld Privileged User List\r\n");
                 privUsers.Append(GetUsersList(oldDb.AuditPrivUsersList, true));
                 privUsers.Append("\tNew Privileged User List\r\n");
                 privUsers.Append(GetUsersList(newDb.AuditPrivUsersList, true));
             }

             if (oldDb.AuditUserAll && !newDb.AuditUserAll)
             {
                 privUsers.Append("\tAudit all privileged user activity turned OFF\r\n");
                 privUsers.Append("\tNew Values\r\n");
                 privUsers.AppendFormat("\t\tLogins\t{0}\r\n", newDb.AuditUserLogins ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tLogouts\t{0}\r\n", newDb.AuditUserLogouts ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tFailed Logins\t{0}\r\n", newDb.AuditUserFailedLogins ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tDDL\t{0}\r\n", newDb.AuditUserDDL ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tSecurity\t{0}\r\n", newDb.AuditUserSecurity ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tAdmin\t{0}\r\n", newDb.AuditUserAdmin ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tDML\t{0}\r\n", newDb.AuditUserDML ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tSELECT\t{0}\r\n", newDb.AuditUserSELECT ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tUser Defined Events\t{0}\r\n", newDb.AuditUserUDE ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tAccess Check Filter:\t{0}\r\n", newDb.AuditUserAccessCheck.ToString());
                 privUsers.AppendFormat("\t\tCapture SQL Statements\t{0}\r\n", newDb.AuditUserCaptureSQL ? "ON" : "OFF");
				 privUsers.AppendFormat("\t\tCapture SQL Statements For DDL\t{0}\r\n", newDb.AuditUserCaptureDDL ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tCapture Transactions: {0}\r\n", newDb.AuditUserCaptureTrans ? "ON" : "OFF");

             }
             else if (!oldDb.AuditUserAll && newDb.AuditUserAll)
             {
                 privUsers.Append("\tAudit all privileged user activity turned ON\r\n");
                 privUsers.Append("\tOld Values\r\n");
                 privUsers.AppendFormat("\t\tLogins\t{0}\r\n", oldDb.AuditUserLogins ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tLogouts\t{0}\r\n", oldDb.AuditUserLogouts ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tFailed Logins\t{0}\r\n", oldDb.AuditUserFailedLogins ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tDDL\t{0}\r\n", oldDb.AuditUserDDL ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tSecurity\t{0}\r\n", oldDb.AuditUserSecurity ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tAdmin\t{0}\r\n", oldDb.AuditUserAdmin ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tDML\t{0}\r\n", oldDb.AuditUserDML ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tSELECT\t{0}\r\n", oldDb.AuditUserSELECT ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tUser Defined Events\t{0}\r\n", oldDb.AuditUserUDE ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tAccess Check Filter:\t{0}\r\n", oldDb.AuditUserAccessCheck.ToString());
                 privUsers.AppendFormat("\t\tCapture SQL Statements\t{0}\r\n", oldDb.AuditUserCaptureSQL ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tCapture Transactions: {0}\r\n", oldDb.AuditUserCaptureTrans ? "ON" : "OFF");
             }
             else if (oldDb.AuditPrivUsersList == "")
             {
                 privUsers.Append("\tAudited Privileged User Activity\r\n");
                 privUsers.AppendFormat("\t\tLogins\t{0}\r\n", newDb.AuditUserLogins ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tLogouts\t{0}\r\n", newDb.AuditUserLogouts ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tFailed Logins\t{0}\r\n", newDb.AuditUserFailedLogins ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tDDL\t{0}\r\n", newDb.AuditUserDDL ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tSecurity\t{0}\r\n", newDb.AuditUserSecurity ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tAdmin\t{0}\r\n", newDb.AuditUserAdmin ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tDML\t{0}\r\n", newDb.AuditUserDML ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tSELECT\t{0}\r\n", newDb.AuditUserSELECT ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tUser Defined Events\t{0}\r\n", newDb.AuditUserUDE ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tAccess Check Filter:\t{0}\r\n", newDb.AuditUserAccessCheck.ToString());
                 privUsers.AppendFormat("\t\tCapture SQL Statements\t{0}\r\n", newDb.AuditUserCaptureSQL ? "ON" : "OFF");
				 privUsers.AppendFormat("\t\tCapture SQL Statements For DDL\t{0}\r\n", newDb.AuditUserCaptureDDL ? "ON" : "OFF");
                 privUsers.AppendFormat("\t\tCapture Transactions: {0}\r\n", newDb.AuditUserCaptureTrans ? "ON" : "OFF");
             }
             else
             {
                 privUsers.Append(GetOnOffFlag("\tLogins", oldDb.AuditUserLogins, newDb.AuditUserLogins));
                 privUsers.Append(GetOnOffFlag("\tLogouts", oldDb.AuditUserLogouts, newDb.AuditUserLogouts));
                 privUsers.Append(GetOnOffFlag("\tFailed Logins", oldDb.AuditUserFailedLogins, newDb.AuditUserFailedLogins));
                 privUsers.Append(GetOnOffFlag("\tDDL", oldDb.AuditUserDDL, newDb.AuditUserDDL));
                 privUsers.Append(GetOnOffFlag("\tSecurity", oldDb.AuditUserSecurity, newDb.AuditUserSecurity));
                 privUsers.Append(GetOnOffFlag("\tAdmin", oldDb.AuditUserAdmin, newDb.AuditUserAdmin));
                 privUsers.Append(GetOnOffFlag("\tDML", oldDb.AuditUserDML, newDb.AuditUserDML));
                 privUsers.Append(GetOnOffFlag("\tSELECT", oldDb.AuditUserSELECT, newDb.AuditUserSELECT));
                 privUsers.Append(GetOnOffFlag("\tUser Defined Events", oldDb.AuditUserUDE, newDb.AuditUserUDE));
                 if (oldDb.AuditUserAccessCheck != newDb.AuditUserAccessCheck)
                     privUsers.AppendFormat("\t\tAccess Check Filter:\tOld: {0}   New: {1}\r\n", oldDb.AuditUserAccessCheck.ToString(), newDb.AuditUserAccessCheck.ToString());
                 privUsers.Append(GetOnOffFlag("\tCapture SQL Statements", oldDb.AuditUserCaptureSQL, newDb.AuditUserCaptureSQL));
				 privUsers.Append(GetOnOffFlag("\tCapture SQL Statements For DDL", oldDb.AuditUserCaptureDDL, newDb.AuditUserCaptureDDL));
                 privUsers.Append(GetOnOffFlag("\tCapture Transactions", oldDb.AuditUserCaptureTrans, newDb.AuditUserCaptureTrans));
             }
         }
         else // check if priv user settings are changed
         {
             if (oldDb.AuditUserLogins != newDb.AuditUserLogins)
                 privUsers.Append(GetOnOffFlag("\tLogins", oldDb.AuditUserLogins, newDb.AuditUserLogins));
             if (oldDb.AuditUserLogouts != newDb.AuditUserLogouts)
                 privUsers.Append(GetOnOffFlag("\tLogins", oldDb.AuditUserLogouts, newDb.AuditUserLogouts));
             if (oldDb.AuditUserFailedLogins != newDb.AuditUserFailedLogins)
                 privUsers.Append(GetOnOffFlag("\tFailed Logins", oldDb.AuditUserFailedLogins, newDb.AuditUserFailedLogins));
             if (oldDb.AuditUserDDL != newDb.AuditUserDDL)
                 privUsers.Append(GetOnOffFlag("\tDDL", oldDb.AuditUserDDL, newDb.AuditUserDDL));
             if (oldDb.AuditUserSecurity != newDb.AuditUserSecurity)
                 privUsers.Append(GetOnOffFlag("\tSecurity", oldDb.AuditUserSecurity, newDb.AuditUserSecurity));
             if (oldDb.AuditUserAdmin != newDb.AuditUserAdmin)
                 privUsers.Append(GetOnOffFlag("\tAdmin", oldDb.AuditUserAdmin, newDb.AuditUserAdmin));
             if (oldDb.AuditUserDML != newDb.AuditUserDML)
                 privUsers.Append(GetOnOffFlag("\tDML", oldDb.AuditUserDML, newDb.AuditUserDML));
             if (oldDb.AuditUserSELECT != newDb.AuditUserSELECT)
                 privUsers.Append(GetOnOffFlag("\tSELECT", oldDb.AuditUserSELECT, newDb.AuditUserSELECT));
             if (oldDb.AuditUserUDE != newDb.AuditUserUDE)
                 privUsers.Append(GetOnOffFlag("\tUser Defined Events", oldDb.AuditUserUDE, newDb.AuditUserUDE));
             if (oldDb.AuditUserAccessCheck != newDb.AuditUserAccessCheck)
                 privUsers.AppendFormat("\t\tAccess Check Filter:\tOld: {0}   New: {1}\r\n", oldDb.AuditUserAccessCheck.ToString(), newDb.AuditUserAccessCheck.ToString());
             if (oldDb.AuditUserCaptureSQL != newDb.AuditUserCaptureSQL)
                 privUsers.Append(GetOnOffFlag("\tCapture SQL Statements", oldDb.AuditUserCaptureSQL, newDb.AuditUserCaptureSQL));
			 if (oldDb.AuditUserCaptureDDL != newDb.AuditUserCaptureDDL)
                 privUsers.Append(GetOnOffFlag("\tCapture SQL Statements For DDL", oldDb.AuditUserCaptureDDL, newDb.AuditUserCaptureDDL));
             if (oldDb.AuditUserCaptureTrans != newDb.AuditUserCaptureTrans)
                 privUsers.Append(GetOnOffFlag("\tCapture Transactions", oldDb.AuditUserCaptureTrans, newDb.AuditUserCaptureTrans));
         }

         if (privUsers.Length != 0)
         {
             snapshot.Append("Privileged User Auditing\r\n");
             snapshot.Append(privUsers.ToString());
         }         

	      // finish up
         if (snapshot.Length == 0 )
         {
            snapshot.Append( "No changes to audit settings.");
         }

         // Add Heading         
         string hdg = String.Format( "Audit Setting Changes for Database: {0} on SQL Server: {1}\r\n\r\n",
                                     newDb.Name,
                                     newDb.SrvInstance );
         snapshot.Insert(0,hdg);
         
		   return snapshot.ToString();
	   }
	   
      //-------------------------------------------------------------
      // GetUserTableSetting
      //--------------------------------------------------------------
	   static private string
	      GetUserTableSetting(
	         int               flag
	      )
	   {
	      string setting ;

         if ( flag == 0 )
            setting = "OFF";
         else if ( flag == 1 )
            setting = "ON (All users tables)";
         else
            setting = "ON (Selected user tables)";
            
         return setting;
	   }
	   
	   #endregion
	   
	   #region Utility Routines

      //---------------------------------------------------------------------------
      /// GetSnapshotSettings - get snapshot settings from the repository.
      //---------------------------------------------------------------------------
      static public void
         GetSnapshotSettings( 
            SqlConnection     conn,
            out int           snapshotInterval,
            out DateTime      snapshotLastTime
         )
      {
         snapshotInterval = 0;
         snapshotLastTime = DateTime.Now;

         try
         {
            string query = String.Format( "SELECT snapshotInterval, snapshotLastTime FROM {0}",
                                          CoreConstants.RepositoryConfigurationTable );
            using ( SqlCommand command = new SqlCommand( query, conn ) )
            {
               using ( SqlDataReader reader = command.ExecuteReader() )
               {
                  if( reader.Read() )
                  {
                     snapshotInterval = SQLHelpers.GetInt32(reader, 0);
                     snapshotLastTime = SQLHelpers.GetDateTime(reader, 1);
                  }
               }
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
               "An error occurred reading snapshot settings from the repository database.",
               e,
               true );
         }
      }
      
      //---------------------------------------------------------------------------
      /// UpdateSnapshotLastTime
      //---------------------------------------------------------------------------
      static public void
         UpdateSnapshotLastTime( 
            SqlConnection conn
         )
      {
         string        sql = "";
         SqlCommand    cmd ;

         try
         {
            sql = String.Format( "UPDATE {0} SET snapshotLastTime={1};",
                                 CoreConstants.RepositoryConfigurationTable,
                                 SQLHelpers.CreateSafeDateTimeString(DateTime.Now) );
			 using(cmd = new SqlCommand( sql, conn ))
			 {
				 cmd.ExecuteNonQuery();
			 }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
               "UpdateSnapshotLastTime",
               sql,
               e );
         }
      }
      
      //---------------------------------------------------------------------------
      /// UpdateSnapshotInterval
      //---------------------------------------------------------------------------
      static public void
         UpdateSnapshotInterval( 
            SqlConnection     conn,
            int               newInterval
         )
      {
         string        sql = "";
         SqlCommand    cmd = null;

         try
         {
            sql = String.Format( "UPDATE {0} SET snapshotInterval={1};",
                                 CoreConstants.RepositoryConfigurationTable,
                                 newInterval );
			 using(cmd = new SqlCommand( sql, conn ))
			 {
				 cmd.ExecuteNonQuery();
			 }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
               "UpdateSnapshotInterval",
               sql,
               e );
         }
         finally
         {
            if (cmd != null ) cmd.Dispose();
         }
      }
      
	   
	   #endregion
	}
}
