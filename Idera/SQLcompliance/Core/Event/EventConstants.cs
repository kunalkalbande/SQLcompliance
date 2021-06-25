using System;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Text;

using Idera.SQLcompliance.Core;
using System.ComponentModel;


namespace Idera.SQLcompliance.Core.Event
{
   //------------------------------------------------------------------------------
	// Enumerations
	//------------------------------------------------------------------------------
   // TraceStatus             - state of a job
   //
   // TraceFilterType         - used in creating trace filters
   // TraceFilterLogicalOp    - used in creating trace filters
   // TraceFilterComparisonOp - used in creating trace filters
   //
   // TraceEventId            - event class from trace file
   // TraceColumnId           - column in trace file
   // TracePermissions        - possible values of permissions column in trace	
   // TraceSubclass           - possible values of eventSubClass column in trace
   
   // Note: ObjectType possibilities are in DBObjectType in DBObjectType class 
   //------------------------------------------------------------------------------
   public enum TraceStatus : int
	{
      Unknown = -1,
      Stop  = 0,
      Start = 1,
      Close = 2
   }
   
	public enum TraceFilterType : int
	{
      Unknown = -1,
      integer  = 0,
      nvarchar = 1
      // add more later as needed!
   }

	public enum TraceFilterLogicalOp : int
	{
      Unknown = -1,
	   AND = 0,
	   OR  = 1
	}

	public enum TraceFilterComparisonOp : int
	{
      Unknown            = -1,
	   Equal              = 0,
	   NotEqual           = 1,
	   GreaterThan        = 2,
	   LessThan           = 3,
	   GreaterThanOrEqual = 4,
	   LessThanOrEqual    = 5,
	   Like               = 6,
	   NotLike            = 7
	}

    //5.4 XE
    public class TraceFilterComparisonOpXE
    {
        public const string Equal                   = " = " ;
        public const string NotEqual                = " <> ";
        public const string GreaterThan             = " > ";
        public const string LessThan                = " < ";
        public const string GreaterThanOrEqual      = " >= ";
        public const string LessThanOrEqual         = " <= ";
        public const string Like                    = " LIKE ";
        public const string NotLike                 = " NOT LIKE ";
    }

    public class AuditLogFilterLogicalOp 
    {
        public const string AND = " AND ";
        public const string OR = " OR ";
    }

   public enum TraceEventId : int
   {
      Unknown                       = -1, // For error handling

      EventIdMin                    = 0,
      
      Reserved0                     = 0,
      Reserved1                     = 1,
      Reserved2                     = 2,
      Reserved3                     = 3,
      Reserved4                     = 4,
      Reserved5                     = 5,
      Reserved6                     = 6,
      Reserved7                     = 7,
      Reserved8                     = 8,
      Reserved9                     = 9,

      RpcCompleted                  = 10, // Completed Occurs when a remote procedure call (RPC) has completed. 
      RpcStarted                    = 11, // Starting Occurs when an RPC has started. 
      BatchCompleted                = 12, //  SQL:BatchCompleted Occurs when a Transact-SQL batch has completed. 
      BatchStarting                 = 13, //  SQL:BatchStarting Occurs when a Transact-SQL batch has started. 
      Login                         = 14, //  Login Occurs when a user successfully logs in to SQL Server. 
      Logout                        = 15, //  Logout Occurs when a user logs out of SQL Server. 
      Attention                     = 16, //  Attention Occurs when attention events, such as client-interrupt requests or broken client connections, happen. 
      ExistingConnection            = 17, //  ExistingConnection Detects all activity by users connected to SQL Server before the trace started. 
      ServiceControl                = 18, //  ServiceControl Occurs when the SQL Server service state is modified. 
      DtcTransaction                = 19, //  DTCTransaction Tracks Microsoft Distributed Transaction Coordinator (MS DTC) coordinated transactions between two or more databases. 
      LoginFailed                   = 20, //  Login Failed Indicates that a login attempt to SQL Server from a client failed. 
      EventLog                      = 21, //  EventLog Indicates that events have been logged in the Microsoft Windows NT?application log. 
      ErrorLog                      = 22, //  ErrorLog Indicates that error events have been logged in the SQL Server error log. 
      LockReleased                  = 23, //  Lock:Released Indicates that a lock on a resource, such as a page, has been released. 
      LockAcquired                  = 24, //  Lock:Acquired Indicates acquisition of a lock on a resource, such as a data page. 
      LockDeadlock                  = 25, //  Lock:Deadlock Indicates that two concurrent transactions have deadlocked each other by trying to obtain incompatible locks on resources the other transaction owns.  
      LockCancel                    = 26, //  Lock:Cancel Indicates that the acquisition of a lock on a resource has been canceled (for example, due to a deadlock). 
      LockTimeout                   = 27, //  Lock:Timeout Indicates that a request for a lock on a resource, such as a page, has timed out due to another transaction holding a blocking lock on the required resource. Time-out is determined by the @@LOCK_TIMEOUT function, and can be set with the SET LOCK_TIMEOUT statement.  
      DopEvent                      = 28, // DOP Event Occurs before a SELECT, INSERT, or UPDATE statement is executed.  
                                          //  29-32 Reserved   
      Exception                     = 33, //  Exception Indicates that an exception has occurred in SQL Server.  
      SpCacheMiss                   = 34, //  SP:CacheMiss Indicates when a stored procedure is not found in the procedure cache. 
      SpCacheInsert                 = 35, //  SP:CacheInsert Indicates when an item is inserted into the procedure cache. 
      SpCacheRemove                 = 36, //  SP:CacheRemove Indicates when an item is removed from the procedure cache. 
      SpRecompile                   = 37, //  SP:Recompile Indicates that a stored procedure was recompiled. 
      SpCacheHit                    = 38, //  SP:CacheHit Indicates when a stored procedure is found in the procedure cache. 
      SpExecContextHit              = 39, //  SP:ExecContextHit Indicates when the execution version of a stored procedure has been found in the procedure cache. 
      SqlStmtStarting                = 40, //  SQL:StmtStarting Occurs when the Transact-SQL statement has started. 
      SqlStmtCompleted               = 41, //  SQL:StmtCompleted Occurs when the Transact-SQL statement has completed. 
      SpStarting                    = 42, //  SP:Starting Indicates when the stored procedure has started. 
      SpCompleted                   = 43, //  SP:Completed Indicates when the stored procedure has completed. 
                                          //  44 Reserved
      SpStmtCompleted               = 45, // SP:StmtCompleted event class indicates that a Transact-SQL statement within a stored procedure has completed.
      ObjectCreated                 = 46, //  Object:Created Indicates that an object has been created, such as for CREATE INDEX, CREATE TABLE, and CREATE DATABASE statements. 
      ObjectDeleted                 = 47, //  Object:Deleted Indicates that an object has been deleted, such as in DROP INDEX and DROP TABLE statements. 
                                          //  48,49 Reserved   
      Transaction                   = 50, //  SQL Transaction Tracks Transact-SQL BEGIN, COMMIT, SAVE, and ROLLBACK TRANSACTION statements. 
      ScanStarted                   = 51, //  Scan:Started Indicates when a table or index scan has started. 
      ScanStopped                   = 52, //  Scan:Stopped Indicates when a table or index scan has stopped. 
      CursorOpen                    = 53, //  CursorOpen Indicates when a cursor is opened on a Transact-SQL statement by ODBC, OLE DB, or DB-Library. 
      TransactionLog                = 54, //  Transaction Log Tracks when transactions are written to the transaction log. 
      HashWarning                   = 55, //  Hash Warning Indicates that a hashing operation (for example, hash join, hash aggregate, hash union, and hash distinct) that is not processing on a buffer partition has reverted to an alternate plan. This can occur because of recursion depth, data skew, trace flags, or bit counting. 
                                          //  56-57 Reserved   
      AutoUpdateStats               = 58, //  Auto Update Stats Indicates an automatic updating of index statistics has occurred. 
      LockDeadlockChain             = 59, //  Lock:Deadlock Chain Produced for each of the events leading up to the deadlock. 
      LockEscalation                = 60, //  Lock:Escalation Indicates that a finer-grained lock has been converted to a coarser-grained lock (for example, a row lock escalated or converted to a page lock). 
      OleDbError                    = 61, //  OLE DB Errors Indicates that an OLE DB error has occurred. 
                                          //  62-66 Reserved   
      ExecutionWarning              = 67, //  Execution Warnings Indicates any warnings that occurred during the execution of a SQL Server statement or stored procedure. 
      ExecutionPlan                 = 68, //  Execution Plan Displays the plan tree of the Transact-SQL statement executed. 
      SortWarning                   = 69, //  Sort Warnings Indicates sort operations that do not fit into memory. Does not include sort operations involving the creating of indexes; only sort operations within a query (such as an ORDER BY clause used in a SELECT statement). 
      CursorPrepare                 = 70, //  CursorPrepare Indicates when a cursor on a Transact-SQL statement is prepared for use by ODBC, OLE DB, or DB-Library. 
      Prepare                       = 71, //  Prepare SQL ODBC, OLE DB, or DB-Library has prepared a Transact-SQL statement or statements for use. 
      ExecPrepare                   = 72, //  Exec Prepared SQL ODBC, OLE DB, or DB-Library has executed a prepared Transact-SQL statement or statements. 
      Unprepare                     = 73, //  Unprepare SQL ODBC, OLE DB, or DB-Library has unprepared (deleted) a prepared Transact-SQL statement or statements. 
      CursorExecute                 = 74, //  CursorExecute A cursor previously prepared on a Transact-SQL statement by ODBC, OLE DB, or DB-Library is executed. 
      CursorRecompile               = 75, //  CursorRecompile A cursor opened on a Transact-SQL statement by ODBC or DB-Library has been recompiled either directly or due to a schema change. 
      CursorImplicitConversion      = 76, //  CursorImplicitConversion A cursor on a Transact-SQL statement is converted by SQL Server from one type to another. 
      CursorUnprepare               = 77, //  CursorUnprepare A prepared cursor on a Transact-SQL statement is unprepared (deleted) by ODBC, OLE DB, or DB-Library. 
      CursorClose                   = 78, //  CursorClose A cursor previously opened on a Transact-SQL statement by ODBC, OLE DB, or DB-Library is closed. 
      MissingColumn                 = 79, //  Missing Column Statistics Column statistics that could have been useful for the optimizer are not available. 
      MissingJoin                   = 80, //  Missing Join Predicate Query that has no join predicate is being executed. This could result in a long-running query. 
      ServerMemoryChange            = 81, //  Server Memory Change Microsoft SQL Server memory usage has increased or decreased by either 1 megabyte (MB) or 5 percent of the maximum server memory, whichever is greater. 
      UserEvent0                    = 82, //  82-91 User Configurable (0-9) Event data defined by the user. 
      UserEvent1                    = 83, //  82-91 User Configurable (0-9) Event data defined by the user. 
      UserEvent2                    = 84, //  82-91 User Configurable (0-9) Event data defined by the user. 
      UserEvent3                    = 85, //  82-91 User Configurable (0-9) Event data defined by the user. 
      UserEvent4                    = 86, //  82-91 User Configurable (0-9) Event data defined by the user. 
      UserEvent5                    = 87, //  82-91 User Configurable (0-9) Event data defined by the user. 
      UserEvent6                    = 88, //  82-91 User Configurable (0-9) Event data defined by the user. 
      UserEvent7                    = 89, //  82-91 User Configurable (0-9) Event data defined by the user. 
      UserEvent8                    = 90, //  82-91 User Configurable (0-9) Event data defined by the user. 
      UserEvent9                    = 91, //  82-91 User Configurable (0-9) Event data defined by the user. 
      DataFileAutoGrow              = 92, //  Data File Auto Grow Indicates that a data file was extended automatically by the server. 
      LogFileAutoGrow               = 93, //  Log File Auto Grow Indicates that a data file was extended automatically by the server. 
      DataFileAutoShrink            = 94, //  Data File Auto Shrink Indicates that a data file was shrunk automatically by the server. 
      LogFileAutoShrink             = 95, //  Log File Auto Shrink Indicates that a log file was shrunk automatically by the server. 
      ShowPlanText                  = 96, //  Show Plan Text Displays the query plan tree of the SQL statement from the query optimizer. 
      ShowPlanAll                   = 97, //  Show Plan ALL Displays the query plan with full compile-time details of the SQL statement executed. 
      ShowPlanStatistics            = 98, //  Show Plan Statistics Displays the query plan with full run-time details of the SQL statement executed. 
                                           //  99 Reserved   
      RpcOutputParameter            = 100, //  RPC Output Parameter Produces output values of the parameters for every RPC. 
                                           //  101 Reserved   
      AuditStatementGDR             = 102, //  Audit Statement GDR Occurs every time a GRANT, DENY, REVOKE for a statement permission is issued by any user in SQL Server. 
      AuditObjectGDR                = 103, //  Audit Object GDR Occurs every time a GRANT, DENY, REVOKE for an object permission is issued by any user in SQL Server. 
      AuditAddLogin                 = 104, //  Audit Add/Drop Login Occurs when a SQL Server login is added or removed; for sp_addlogin and sp_droplogin. 
      AuditLoginGDR                 = 105, //  Audit Login GDR Occurs when a Microsoft Windows?login right is added or removed; for sp_grantlogin, sp_revokelogin, and sp_denylogin. 
      AuditLoginChange              = 106, //  Audit Login Change Property Occurs when a property of a login, except passwords, is modified; for sp_defaultdb and sp_defaultlanguage. 
      AuditLoginChangePassword      = 107, //  Audit Login Change Password Occurs when a SQL Server login password is changed. Passwords are not recorded.
      AuditAddLoginToServer         = 108, //  Audit Add Login to Server Role Occurs when a login is added or removed from a fixed server role; for sp_addsrvrolemember, and sp_dropsrvrolemember. 
      AuditAddDbUser                = 109, //  Audit Add DB User Occurs when a login is added or removed as a database user (Windows or SQL Server) to a database; for sp_grantdbaccess, sp_revokedbaccess, sp_adduser, and sp_dropuser. 
      AuditAddMember                = 110, //  Audit Add Member to DB Occurs when a login is added or removed as a database user (fixed or user-defined) to a database; for sp_addrolemember, sp_droprolemember, and sp_changegroup. 
      AuditAddDropRole              = 111, //  Audit Add/Drop Role Occurs when a login is added or removed as a database user to a database; for sp_addrole and sp_droprole. 
      AppRolePassChange             = 112, //  App Role Pass Change Occurs when a password of an application role is changed. 
      AuditStatementPermission      = 113, //  Audit Statement Permission Occurs when a statement permission (such as CREATE TABLE) is used. 
      AuditObjectPermission         = 114, //  Audit Object Permission Occurs when an object permission (such as SELECT) is used, both successfully or unsuccessfully. 
      AuditBackupRestore            = 115, //  Audit Backup/Restore Occurs when a BACKUP or RESTORE command is issued. 
      AuditDbcc                     = 116, //  Audit DBCC Occurs when DBCC commands are issued. 
      AuditChangeAudit              = 117, //  Audit Change Audit Occurs when audit trace modifications are made. 
      AuditObjectDerivedPermission  = 118, // Audit Object Derived Permission Occurs when a CREATE, ALTER, and DROP object commands are issued. 

      // New events in SQL Server 2005
      AuditDatabaseManagement             = 128, // Occurs when a database is created, altered, or dropped.
      AuditDatabaseObjectManagement       = 129, // Occurs when a CREATE, ALTER, or DROP statement executes on database objects, such as schemas.
      AuditDatabasePrincipalManagement    = 130, // Occurs when principals, such as users, are created, altered, or dropped from a database.
      AuditSchemaObjectManagement         = 131, // Occurs when server objects are created, altered, or dropped.
      AuditServerPrincipalImpersonation   = 132, // Occurs when there is an impersonation within server scope, such as EXECUTE AS LOGIN.
      AuditDatabasePrincipalImpersonation = 133, // Occurs when an impersonation occurs within the database scope, such as EXECUTE AS USER or SETUSER.
      AuditServerObjectTakeOwnership      = 134, // Occurs when the owner is changed for objects in server scope.
      AuditDatabaseObjectTakeOwnership    = 135, // Occurs when a change of owner for objects within database scope occurs.
      AuditChangeDatabaseOwner            = 152, // Occurs when ALTER AUTHORIZATION is used to change the owner of a database and permissions are checked to do that.
      AuditSchemaObjectTakeOwnership      = 153, // Occurs when ALTER AUTHORIZATION is used to assign an owner to an object and permissions are checked to do that. 
      AuditBrokerConversation             = 158, // Reports audit messages related to Service Broker dialog security.
      AuditBrokerLogin                    = 159, // Reports audit messages related to Service Broker transport security.
      AuditServerScopeGDR                 = 170, // Indicates that a grant, deny, or revoke event for permissions in server scope occurred, such as creating a login.
      AuditServerObjectGDR                = 171, // Indicates that a grant, deny, or revoke event for a schema object, such as a table or function, occurred.
      AuditDatabaseObjectGDR              = 172, // Indicates that a grant, deny, or revoke event for database objects, such as assemblies and schemas, occurred.
      AuditServerOperation                = 173, // Occurs when Security Audit operations such as altering settings, resources, external access, or authorization are used.
      AuditServerAlterTrace               = 175, // Occurs when a statement checks for the ALTER TRACE permission.
      AuditServerObjectManagement         = 176, // Occurs when server objects are created, altered, or dropped.
      AuditServerPrincipalManagement      = 177, //  Occurs when server principals are created, altered, or dropped.
      AuditDatabaseOperation              = 178, // Occurs when database operations occur, such as checkpoint or subscribe query notification.
      AuditDatabaseObjectAccess           = 180, // Occurs when database objects, such as schemas, are accessed.
      
      EventIdMax                          = 181
   }
   // 5.4_4.1.1_Extended Events Start
   public class TraceEventIdXE
   {      
       public const string AuditChangeAudit = "NA"; 
      
       public const string  Exception = "NA";                   
       
       public const string AuditObjectPermission = "NA";       
       
       public const string AuditDatabaseObjectAccess = "NA";

       public const string SqlStmtStarting = "sql_statement_starting";

       public const string SqlStmtCompleted = "sql_statement_completed";

       public const string SpCompleted = "sp_statement_completed";

       public const string RpcCompleted = "rpc_completed";

       public const string SpStarting = "sp_statement_starting";
   }

   public class TraceColumnIdXE 
   {       
       public const string TextData                 = "sqlserver.sql_text";
       public const string DatabaseID               = "sqlserver.database_id";
       public const string ClientHostName           = "sqlserver.client_hostname";
       public const string ApplicationName          = "sqlserver.client_app_name";
       public const string SQLSecurityLoginName     = "sqlserver.server_principal_name";
       public const string SPID                     = "sqlserver.session_id";
       public const string StartTime                = "package0.collect_system_time";
       public const string DatabaseName             = "sqlserver.database_name";
       public const string LinkedServerName         = "sqlserver.server_instance_name"; 
       public const string EventSequence            = "package0.event_sequence"; 
       public const string IsSystem                 = "sqlserver.is_system"; 
       public const string SessionLoginName         = "sqlserver.session_server_principal_name";
       public const string Statement                = "statement";                                   
   }
   // 5.4_4.1.1_Extended Events End
   public enum TraceColumnId : int
   {
      Unknown              = -1, // For error handling
      Unknown1             = 0,

      TextData             =  1, // TextData Text value dependent on the event class that is captured in the trace. 
      BinaryData           =  2, // BinaryData Binary value dependent on the event class captured in the trace. 
      DatabaseID           =  3, // DatabaseID ID of the database specified by the USE database statement, or the default database if no USE database statement is issued for a given connection. The value for a database can be determined by using the DB_ID function.
      TransactionID        =  4, // TransactionID System-assigned ID of the transaction. 
      Reserved             =  5, // Reserved   
      NTUserName           =  6, // NTUserName Microsoft Windows NT?user name. 
      NTDomainName         =  7, // NTDomainName Windows NT domain to which the user belongs. 
      ClientHostName       =  8, // ClientHostName Name of the client computer that originated the request.  
      ClientProcessID      =  9, // ClientProcessID ID assigned by the client computer to the process in which the client application is running. 
      ApplicationName      = 10, // ApplicationName Name of the client application that created the connection to an instance of SQL Server. This column is populated with the values passed by the application rather than the displayed name of the program. 
      SQLSecurityLoginName = 11, // SQLSecurityLoginName SQL Server login name of the client. 
      SPID                 = 12, // SPID Server Process ID assigned by SQL Server to the process associated with the client. 
      Duration             = 13, // Duration Amount of elapsed time (in milliseconds) taken by the event. This data column is not populated by the Hash Warning event. 
      StartTime            = 14, // StartTime Time at which the event started, when available. 
      EndTime              = 15, // EndTime Time at which the event ended. This column is not populated for starting event classes, such as SQL:BatchStarting or SP:Starting. It is also not populated by the Hash Warning event. 
      Reads                = 16, // Reads Number of logical disk reads performed by the server on behalf of the event. This column is not populated by the Lock:Released event. 
      Writes               = 17, // Writes Number of physical disk writes performed by the server on behalf of the event. 
      CPU                  = 18, // CPU Amount of CPU time (in milliseconds) used by the event. 
      Permissions          = 19, // Permissions Represents the bitmap of permissions; used by Security Auditing. 
      Severity             = 20, // Severity Severity level of an exception. 
      EventSubClass        = 21, // EventSubClass Type of event subclass. This data column is not populated for all event classes. 
      ObjectID             = 22, // ObjectID System-assigned ID of the object. 
      Success              = 23, // Success Success of the permissions usage attempt; used for auditing. 1 = success; 0 = failure
      IndexID              = 24, // IndexID ID for the index on the object affected by the event. To determine the index ID for an object, use the indid column of the sysindexes system table. 
      IntegerData          = 25, // IntegerData Integer value dependent on the event class captured in the trace. 
      ServerName           = 26, // ServerName Name of the instance of SQL Server (either servername or servername\instancename) being traced. 
      EventClass           = 27, // EventClass Type of event class being recorded. 
      ObjectType           = 28, // ObjectType Type of object (such as table, function, or stored procedure). 
      NestLevel            = 29, // NestLevel The nesting level at which this stored procedure is executing. See @@NESTLEVEL. 
      State                = 30, // State Server state, in case of an error. 
      Error                = 31, // Error Error number. 
      Mode                 = 32, // Mode Lock mode of the lock acquired. This column is not populated by the Lock:Released event. 
      Handle               = 33, // Handle Handle of the object referenced in the event. 
      ObjectName           = 34, // ObjectName Name of object accessed. 
      DatabaseName         = 35, // DatabaseName Name of the database specified in the USE database statement. 
      FileName             = 36, // Filename Logical name of the file name modified. 
      ObjectOwner          = 37, // ObjectOwner Owner ID of the object referenced. 
      TargetRoleName       = 38, // TargetRoleName Name of the database or server-wide role targeted by a statement. 
      TargetUserName       = 39, // TargetUserName User name of the target of some action. 
      DatabaseUserName     = 40, // DatabaseUserName SQL Server database username of the client. 
      LoginSID             = 41, // LoginSID Security identification number (SID) of the logged-in user. 
      TargetLoginName      = 42, // TargetLoginName Login name of the target of some action. 
      TargetLoginSID       = 43, // TargetLoginSID SID of the login that is the target of some action. 
      ColumnPermissionsSet = 44, // ColumnPermissionsSet Column-level permissions status; used by Security Auditing. 
      LinkedServerName     = 45, // Name of the linked server
      ProviderName         = 46, // The authentication provider
      RowCounts            = 48, // Affected Row Counts
      EventSequence        = 51, // Trace event sequence number
      ParentName           = 59, // Name of the schema that the object is within.
      IsSystem             = 60,  // Indicates whether the event occurred on a system process or a user process
      SessionLoginName     = 64,  // Login name of the user who originated the session
      SQLcmTableName       = 9001  // A special column ID used during server side filtering not one of the SQL Server trace columns
   }

   // two types of permissions in events - which you use depends on the event class
   public enum TracePermissions : int
   {
      Unknown          = -1,
      Unknown1         = 0,
      
      CreateDatabase   = 1,
      CreateTable      = 2,
      CreateProcedure  = 4,
      CreateView       = 8,
      CreateRule       = 16,
      CreateDefault    = 32,
      BackupDatabase   = 64,
      BackupLog        = 128,
      BackupTable      = 256,
      CreateFunction   = 512,
      AlterUserTable = 217, // SQLCM-5471 v5.6 Add Activity to Senstitive columns
      
      SelectAll        = 1,
      UpdateAll        = 2,
      ReferencesAll    = 4,
      Insert           = 8,
      Delete           = 16,
      Execute          = 32,
      SelectAny        = 4096,
      UpdateAny        = 8192,
      ReferencesAny    = 16384
   }
   
   public enum TraceSubclass : int
   {
      Unknown          = -1,
      Unknown1         = 0,
      
      // DDL
      Create   = 1,
      Alter    = 2,
      Drop     = 3,
      Dump     = 4,
      Disable  = 5,                // Disable login - applies to SQL 2005 logins only
      Enable   = 6,                // Enable login - applies to SQL 2005 logins only
      CredentialMappedToLogin = 7, // server object only
      Transfer = 8,                // schema object only
      CredentialMapDropped = 9,    // server objects only
      Open     = 10,
      Load     = 11,
      Access   = 12,
      
      // Security
      Grant    = 1,
      Revoke   = 2,
      Deny     = 3,
      
      // Change login password
      ChangedSelf  = 1,
      ChangedOther = 2,
      ResetSelf = 3,
      ResetOther = 4,
      PasswordUnlocked = 5,
      PasswordMustChange = 6,
      
      // Change login property
      DefaultDatabase = 1,
      DefaultLanguage = 2,
      
      // Add/drop logins, users, members
      AddLogin  = 1,
      DropLogin = 2,
      
      // Add database user
      AddDatabaseUser      = 1,
      DropDatabaseUser     = 2,
      GrantDatabaseAccess  = 3,
      RevokeDatabaseAccess = 4,
      
      // Backup
      Backup  = 1,
      Restore = 2,
      BackupLog = 3,

      // Server Operations
      AdminBulkOperations   = 1,
      AlterSettings         = 2,
      AlterResource         = 3,
      Authenticate          = 4,
      ExternalAccess        = 5,
      AlterServerState      = 175,

      // Database Operations
      CheckPoint                   = 1,
      SubscribeToQueryNotification = 2,

      // Broker Conversation
      NoSecurityHeader             = 1,
      NoCertificate                = 2,
      InvalidSignature             = 3,
      RunAsTargetFailure           = 4,

      //SQLTransaction
      BeginTran = 0,
      CommitTran = 1,
      RollbackTran = 2,
      SaveTran = 3
   }


   /// <summary>
   /// 5.5 Audit Logs
   /// </summary>
   #region Audit Logs
   public enum AuditLogEventID : int
   {
       SCHEMA_OBJECT_ACCESS_GROUP = 114,  //Assigning Event ID of trace's AuditObjectPermission event class to retain existing architecture of trace auditing.
       TRANSACTION_GROUP = 50
   }

   public class AuditLogColumnId
   {
      public const string TextData                = "statement";  
      public const string TransactionID           = "transaction_id";
      public const string ClientHostName          = "client_ip";
      public const string ApplicationName         = "application_name";
      public const string SQLSecurityLoginName    = "server_principal_name";
      public const string SPID                    = "session_id";           
      public const string Duration                = "duration_milliseconds";
      public const string StartTime               = "event_time"; 
      public const string Permissions             = "permission_bitmask"  ;
      public const string ObjectID                = "object_id";    
      public const string Success                 = "succeeded"  ;  
      public const string ServerName              = "server_instance_name"; 
      public const string ObjectType              = "class_type" ;
      public const string ObjectName              = "object_name";
      public const string DatabaseName            = "database_name";     
      public const string FileName                = "file_name";
      public const string DatabaseUserName        = "database_principal_name";
      public const string LoginSID                = "server_principal_sid";
      public const string TargetLoginName         = "target_server_principal_name";
      public const string TargetLoginSID          = "target_server_principal_sid";
      public const string ColumnPermissionsSet    = "is_column_permission";
      public const string EventSequence           = "sequence_number";
      public const string ParentName              = "schema_name";  
      public const string SessionLoginName        = "session_server_principal_name";
      public const string ActionID                = "action_id";
      public const string IsColumnsPermission     = "is_column_permission";
      public const string RowCounts               = "affected_rows";
    }


   public class AuditLogActionID
   {
       public const string Delete               = "DL";
       public const string Execute              = "EX";  
       public const string Insert               = "IN";  
       public const string Select               = "SL";  
       public const string Update               = "UP";
       public const string References           = "RF";
       public const string TransactionBegin     = "TXBG";
       public const string TransactionCommit    = "TXCM";
       public const string TransactionRollback  = "TXRB";
   }
    #endregion
}
