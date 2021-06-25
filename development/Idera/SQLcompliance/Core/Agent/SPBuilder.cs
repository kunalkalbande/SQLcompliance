using System;
using System.IO;
using System.Text;
using System.Collections;

using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Event;



namespace Idera.SQLcompliance.Core.Agent
{
   #region Enumerations
   // Stored procedure commands
   public enum SPCmd : int
   {
      Unknown               = -1,
      Start                 =1,         
      Stop                  =2,         
      GetStatus             =3,         
      GetInfo               =4,         
      GetRunningTraces      =5,
      StopAll               =6
   }

   #endregion


   /// <summary>
   /// Summary description for SPBuilder.
   /// </summary>
   public class SPBuilder
   {
      #region Constants              // Stored procedure builder constants

      public const int               EventListVariableLength        = 400;
      public const int               ColumnListVariableLength       = 300;
      public const string            SQLSECURE_AuditSPParam1        = "@Cmd int = 1,\n"; // Default to start the trace
      public const string            SQLSECURE_AuditSPParam2        = "@Option nvarchar(4000) = null\n";
      public const string            SQLSECURE_AuditSPParam3        = "@FileName nvarchar(256) = 'SQLsecureAgentTrace.config'\n";
      public const string            SPCommand                      = "@Cmd";
      public const string            SPOption                       = "@Option";
      public const string            SPStartTrace                   = "STARTTRACE";
      public const string            SPEndStartTrace                = "ENDSTARTTRACE";

      // Commonly used string formats
      public const string            CreateProc                     = "CREATE PROC {0}\n";
      // SQLCM 5.6- 566/740/4620/5280 (Non-Admin and Non-Sysadmin role) Permissions
      public const string            AlterProc                      = "ALTER PROC {0}\n";
      public const string            SetNoCountOn                   = "SET NOCOUNT ON\n";
      public const string            AssignValue                    = "SET {0}={1}\n";
      public const string            GetDateString                  = "CONVERT(CHAR(24),GETDATE(),121)\n";
      public const string            CastType                       = "CAST( {0} AS {1} )\n";
      public const string            DeclareIntVariable             = "DECLARE {0} int\n";
      public const string            DeclareDateTimeVariable        = "DECLARE {0} datetime\n";
      public const string            DeclareVarcharVariable         = "DECLARE {0} varchar({1})\n";
      public const string            DeclareNvarcharVariable        = "DECLARE {0} nvarchar({1})\n";
      public const string            DeclareBigIntVariable          = "DECLARE {0} bigint\n";
      public const string            DeclareBitVariable             = "DECLARE {0} bit\n";
      public const string            DeclareVariantVariable         = "DECLARE {0} SQL_VARIANT\n";
      public const string            DeclareSysnameVariable         = "DECLARE {0} sysname\n";
      public const string            DeclareCursor                  = "DECLARE {0} CURSOR FOR {1}\n";
      public const string            OpenCursor                     = "OPEN {0}\n";
      public const string            CloseCursor                    = "CLOSE {0}\n";
      public const string            DeallocateCursor               = "DEALLOCATE {0}\n";
      public const string            DeclareUserNameTable           = "DECLARE {0} table ( name nvarchar(128) )\n";
      public const string            DeclareTraceInfoTable          = "DECLARE {0} table ( "
         + "[num] [int], [TraceID] [int], [FileName] [nvarchar] (245), [Options] [int], [MaxSize] [bigint], [StopTime] [DateTime], [Status] [int])\n";
      public const string            BuildFileName                  = "SET {0} = '{1}' + {2}\n";
      public const string            BuildUniqueFileName            = "SET {0}='{1}' + CONVERT(CHAR(8),GETDATE(),112)\n"
         + " + REPLACE(CONVERT(varchar(15),GETDATE(),114),':','')\n";
      public const string            BuildFileDate                  = "SET {0} = CONVERT(CHAR(8),GETDATE(),112)\n"
         + " + REPLACE(CONVERT(varchar(15),GETDATE(),114),':','')\n";
      public const string            CreateTrace                    = "EXEC {0}=sp_trace_create @TraceId={1} OUT," 
         + "@options={2}, "
         + "@tracefile={3}, "
         + "@maxfilesize={4}, "
         + "@stoptime={5}\n";
      public const string            FindServerRoleUsersStatement   = "SELECT DISTINCT l.name FROM master.dbo.sysxlogins l, master.dbo.spt_values s \n"
         + "WHERE ( s.low = 0 AND s.type = 'SRV' AND ( s.number & l.xstatus = s.number ) AND l.srvid IS NULL )\n";
      public const string            CreateTempTable                = "CREATE TABLE {0} ( num INT, id INT, name nvarchar(245), o INT, m BIGINT, st DATETIME, s INT)\n";
      public const string            InsertStartResultToTempTable   = "INSERT {0} EXEC {1}={2} 1\n";

      public const string            SetEventStatement              = "EXEC {0}=sp_trace_setevent @TraceID={1},@EventId={2},@ColumnId={3},@on={4}\n";
      public const string            SetTraceStatusStatement        = "EXEC {0}=sp_trace_setstatus @traceid={1}, @status={2}\n";
      public const string            SetTextFilterStatement         = "EXEC {0}=sp_trace_setfilter @TraceId={1}, @columnid={2}, @logical_operator={3}, @comparison_operator={4}, @value=N'{5}'\n";
      public const string            SetFilterStatement             = "EXEC {0}=sp_trace_setfilter @TraceId={1}, @columnid={2}, @logical_operator={3}, @comparison_operator={4}, @value={5}\n";
      public const string            GetTraceInfoALLStatement       = "SELECT * FROM ::fn_trace_getinfo(0)\n";
      public const string            GetTraceInfoStatement          = "SELECT property, value FROM ::fn_trace_getinfo( {0} )\n";
      public const string            GetTraceStatusStatement        = "SELECT Value FROM ::fn_trace_getinfo( {0} ) WHERE Property = 5\n";
      public const string            IfStatement                    = "IF( {0} {1} {2} ) \n";
      public const string            BlockIfStatement               = "IF( {0} {1} {2} ) \n" + SqlBegin;
      public const string            ElseIfStatement                = "ELSE IF( {0} {1} {2} ) \n";
      public const string            BlockElseIfStatement           = "ELSE IF( {0} {1} {2} ) \n" + SqlBegin;
      public const string            BlockElseStatement             = "ELSE BEGIN\n";
      public const string            WhileStatement                 = "WHILE {0} {1} {2} BEGIN\n";
      public const string            ReturnStatement                = "RETURN {0}\n";
      public const string            ReturnSuccess                  = "RETURN 0\n";
      public const string            GotoStatement                  = "GOTO {0}\n";
      public const string            BinaryExpression               = " {0} {1} {2} ";
      public const string            ElseStatement                  = "ELSE\n";

      // Registry extended stored procedures
      public const string            DeleteRegistryValue            = "EXEC xp_regdeletevalue 'HKEY_LOCAL_MACHINE', '{0}', '{1}'\n";
      public const string            AddToRegistryValue             = "EXEC xp_regaddmultistring 'HKEY_LOCAL_MACHINE', '{0}', '{1}', {2}\n";
      public const string            AddToRegistryValueString       = "EXEC xp_regaddmultistring 'HKEY_LOCAL_MACHINE', '{0}', '{1}', '{2}'\n";

      // Stored procedure variable names
      public const string            SPReturnCode          = "@RetCode";
      public const string            SPTraceId             = "@TraceID";
      public const string            SPTraceNumber         = "@TraceNumber";

      public const string            SPServerTraceID       = "@STraceID";
      public const string            SPServerEvents        = "@SEvents";
      public const string            SPServerColumns       = "@SCols";
      public const string            SPServerFileName      = "@SFileName";
      public const string            SPServerFilePrefix    = "SQLsecureS";

      public const string            SPPrefix              = "@SPPrefix";
      public const string            SPDBTraceID           = "@DTraceID";
      public const string            SPDBEvents            = "@DEvents";
      public const string            SPDBColumns           = "@DCols";

      public const string            SPTableTraceID        = "@TTraceID";
      public const string            SPTableEvents         = "@DEvents";
      public const string            SPTableColumns        = "@DCols";

      public const string            SPObjectTraceID       = "@OTraceID";
      public const string            SPObjectEvents        = "@OEvents";
      public const string            SPObjectColumns       = "@OCols";

      public const string            SPEvent               = "@Event";
      public const string            SPCol                 = "@Col";
      public const string            SPCols                = "@Cols";
      public const string            SPEvents              = "@Events";
      public const string            SPTempEvents          = "@TmpEvents";
      public const string            SPTempCols            = "@TmpCols";
      public const string            SPColStr              = "@ColStr";
      public const string            SPStopTime            = "@StopTime";
      public const string            SPOn                  = "@OnOff";
      public const string            SPMaxSize             = "@MaxSize";
      public const string            SPFileDate            = "@FileDate";
      public const string            SPTraceOptions        = "@TraceOptions";
      public const string            SPStatus              = "@Status";
      public const string            SPFileName            = "@TraceFileName";
      public const string            SPProperty            = "@Property";
      public const string            SPValue               = "@Value";
      public const string            SPUsers               = "@Users";
      public const string            SPUser                = "@User";
      public const string            SPNumUsers            = "@NumUsers";
      public const string            SPPUser               = "@PUser";
      public const string            SPIndex               = "@Idx";
      public const string            SPTempResult          = "##TMPRst";
      public const string            SPRows                = "@Rows";
      public const string            SPPermissions         = "@Permissions";

      public const string            SPTraceInfoTable      = "@TraceInfoTable";
      public const string            SPInfoCursor          = "InfoCursor";
      public const string            SPUserCursor          = "UserCursor";


      public const string            SPSessionIdXE         = "@SPSessionId";
      public const string            SPQueryXE             = "@SPQuery";
      public const string            ExecQueryXE           = "EXEC (@SPQuery)\n";
      public const string            IfExistSessionXE      = "IF EXISTS(SELECT * FROM sys.server_event_sessions WHERE name='''+ cast({0} as nvarchar(max)) +''')\n";
      public const string            DropCastSessionXE     = "DROP EVENT SESSION '+ cast({0} as nvarchar(max)) +' ON SERVER";
      public const string            DropSessionXE         = "DROP EVENT SESSION '+ {0} +' ON SERVER";
      public const string            CreateSessionXE       = "CREATE EVENT SESSION '+ cast({0} as nvarchar(max)) +' " +
                                                             "ON SERVER \n";
	  public const string            AddEventXE            = "ADD EVENT {0}(ACTION(\n{1}\n){2})";
      public const string            AddTargetXE           = "ADD TARGET package0.event_file(SET filename = N'''+ cast({0} as nvarchar(max)) +'.xel'', max_file_size= ({1}), max_rollover_files=(10000))\n " +
                                                             "WITH (MAX_MEMORY={1} MB,EVENT_RETENTION_MODE=NO_EVENT_LOSS,MAX_DISPATCH_LATENCY=30 SECONDS," +
                                                             "MAX_EVENT_SIZE={1} MB,MEMORY_PARTITION_MODE=NONE,TRACK_CAUSALITY=ON,STARTUP_STATE=OFF)\n"+
                                                             "ALTER EVENT SESSION '+ cast({2} as nvarchar(max)) +' ON SERVER state=start";
      public const string            AlterSessionStatusXE  = "ALTER EVENT SESSION '+ {0} +' ON SERVER state={1}\n";
      public const string            GetSessionIdXE        = "SET @SPSessionId = (SELECT event_session_id FROM sys.server_event_sessions WHERE name = ''+ {0} +'')\n";
      public const string            BuildFileNameXE       = "SET {0} = '{1}' + {2}\n";
      public const string            BuildSessionNameXE    = "SET {0} = '{1}' + {2}\n";
      public const string            SPTraceInfoTableXE    = "@TraceInfoTable";
      public const string            SetQueryStringXE      = "SET {0} = '{1}'\n";
      public const string            SessionStartXE        = "start";
      public const string            SessionStopXE         = "stop";
      public const string            SessionNameXE         = "@SessionName";
      public const string            SetSessionNameXE      = "SET @SessionName = (SELECT name FROM sys.server_event_sessions where event_session_id = @TraceID)";
      public const string            EventSessionName      = "@EventSessionName";
      public const string            MaxMemory             = "@MaxMemory";
      public const string            GetTraceInfoQueryXE   = "SELECT name,max_memory from sys.server_event_sessions where event_session_id = {0}\n";
      public const string            FileNameWithPath      = "@FileNameWithPath";
      public const string            GetFileNameWithPath   = "(select  xed.target_data.value('(@name)[1]', 'nvarchar(max)') from\n"+
	                                                          "\t(SELECT CAST(target_data as xml) as targetdata\n" +
	                                                          "\tFROM sys.dm_xe_session_targets xet\n"+
	                                                          "\tJOIN sys.dm_xe_sessions xes ON\n"+
	                                                          "\txes.address = xet.event_session_address\n"+
                                                              "\tWHERE xes.name like @EventSessionName) filePath\n"+
	                                                          "\tCROSS APPLY targetdata.nodes('/EventFileTarget/File') AS xed (target_data))\n";
      public const string            Max                   = "MAX";
      public const string            SessoinToFileName     = "REPLACE(@EventSessionName,'XE_','XE-')";
      public const string            GetPathFromFileName   = @"reverse((SUBSTRING(reverse({0}), charindex('\', reverse({0})),LEN({0}))))";
			                                                

      // T-SQL keywords
      protected const string         SqlAs             = "AS\n";
      protected const string         SqlBegin          = "BEGIN \n";
      protected const string         SqlEnd            = "END\n";
      protected const string         SqlIf             = "IF ";
      protected const string         SqlElse           = "ELSE \n";
      protected const string         SqlElseIf         = "ELSE IF ";
      protected const string         SqlWhile          = "WHILE ";
      protected const string         SqlSet            = "SET ";
      protected const string         SqlPrint          = "PRINT ";
      protected const string         SqlNull           = "NULL";
      protected const string         SqlAnd            = "AND";
      protected const string         SqlOr             = "OR";
      protected const string         SqlIsNot          = "IS NOT";
      protected const string         SqlIs             = "IS";
      protected const string         SqlGreater        = ">";
      protected const string         SqlLess           = "<";
      protected const string         SqlNotEqual       = "<>";
      protected const string         SqlGreaterOrEqual = ">=";
      protected const string         SqlLessOrEqual    = "<=";
      protected const string         SqlEqual          = "=";
      protected const string         SqlIn             = "IN";


      //Constants for Audit Logs 
      private const string CreateAuditSpecification    =  "CREATE SERVER AUDIT ['+ {0} +'] \n" +
                                                          "TO FILE \n" +
                                                          "(	 FILEPATH = N''{1}'' \n" +
	                                                      "     ,MAXSIZE = {2} MB \n" +
	                                                      "     ,MAX_ROLLOVER_FILES = 2147483647 \n"+
	                                                      "     ,RESERVE_DISK_SPACE = OFF \n" +
                                                          ") \n"+
                                                          "WITH \n"+
                                                          "(	QUEUE_DELAY = 0 \n"+
	                                                      "    ,ON_FAILURE = CONTINUE \n"+
                                                          ") \n"+
                                                          "WHERE {3}" +
                                                          "ALTER SERVER AUDIT ['+ {0} +'] WITH (STATE = ON) \n" +
                                                          "CREATE SERVER AUDIT SPECIFICATION ['+ {0} +'] \n" +
                                                          "FOR SERVER AUDIT ['+ {0} +'] \n" +
                                                          "{4}"+
                                                          "WITH (STATE = ON)";
      private const string AddAuditLogEvent             = "ADD ({0})\n";
      private const string GetAuditLogDetails           = "SELECT @SPSessionId = audit_id,  @TraceFileName = SUBSTRING(audit_file_path,0,CHARINDEX('.',audit_file_path)) FROM sys.dm_server_audit_status WHERE name = {0}\n";
      private const string ConditionTextForNvarchar     = "[{0}]{1}N''{2}''";
      private const string ConditionTextForInt          = "[{0}]{1}{2}";
      private const string GroupFilterText              = "({0})\n";
      private const string GroupFilters                 = "({0})";
      private const string GetAuditLogSessionName       = "SET @SessionName = (SELECT name FROM sys.server_audits where audit_id = @TraceID)";
      private const string IfSessionIsNotEmpty          = "IF(@SessionName <> '')\n" ;
      private const string DropServerAudit              = "EXEC ('IF EXISTS(SELECT * FROM sys.server_audits WHERE name = N'''+ {0} +''')\n"+
                                                          "BEGIN\n" +
                                                          "ALTER SERVER AUDIT ['+ {0} +'] WITH (STATE = OFF)\n" +
                                                          "DROP SERVER AUDIT ['+ {0} +']\n" +
                                                          "END')\n";

      private const string DropServerAuditSpecification = "EXEC ('IF EXISTS(SELECT * FROM sys.server_audit_specifications WHERE name = N'''+ {0} +''')\n" +
                                                          "BEGIN\n" +
                                                          "ALTER SERVER AUDIT SPECIFICATION ['+ {0} +'] WITH (STATE = OFF)\n" +
                                                          "DROP SERVER AUDIT SPECIFICATION['+ {0} +']\n" +
                                                          "END')\n";
      private const string GetAuditLogInfo              = "SELECT {0} = status,{1} = audit_file_path FROM sys.dm_server_audit_status WHERE audit_id = {2}\n";


      // T-SQL data types
      protected const string         SqlInt            = "int";
      protected const string         SqlBigInt         = "bigint";
      protected const string         SqlDateTime       = "DateTime";
      protected const string         SqlSysname        = "sysname";
      protected const string         SqlNvarchar       = "nvarchar( {0} )";

      // SQL Server Roles
      protected const string         SysAdminRole      = "sysadmin";
      #endregion

      #region Protected Data Members

      protected StringBuilder procedure;
      protected string        agentVersion;
      protected string        traceDirectory;
      protected string        instanceAlias;
      protected int           configVersion;
      protected DateTime      lastModifiedTime = DateTime.MinValue;
      protected DateTime      stopTime = DateTime.MinValue;
      protected int           maxTraceSize = CoreConstants.Agent_Default_MaxTraceSize;
      protected TraceOption   options = (TraceOption)CoreConstants.Agent_Default_TraceOptions;
      protected ArrayList     traceConfigurations;
      protected ArrayList     traceConfigurationsXE;
      protected int           maxUnattendedTime = -1;
      protected int           sqlVersion = 8;

      //5.5 Audit Logs
      protected ArrayList     auditLogConfigurations;

      #endregion

      #region Properties

      // SQLsecure Agent's version number
      public string AgentVersion
      {
         get { return agentVersion; }
         set 
         { 
            if( value == null )
               throw( new ArgumentNullException( "AgentVersion" ) );

            agentVersion = value; 
         }
      }

      public string TraceDirectory
      {
         get { return traceDirectory; }
         set { traceDirectory = value; }
      }

      public int MaxTraceSize
      {
         get { return maxTraceSize; }
         set { maxTraceSize = value; }
      }

      public DateTime StopTime
      {
         get { return stopTime; }
         set { stopTime = value; }
      }

      public int MaxUnattendedTime
      {
         get { return maxUnattendedTime; }
         set { maxUnattendedTime = value; }
      }

      public TraceOption Options
      {
         get { return options; }
         set { options = value; }
      }

      public string InstanceAlias
      {
         get { return instanceAlias; }
         set { instanceAlias = value; }
      }

      public int ConfigurationVersion
      {
         get { return configVersion; }
         set { configVersion = value; }
      }


      // Number of traces in the main audit SP
      public int  TraceCount
      {
         get 
         { 
            if( traceConfigurations == null )
               return 0;
            else
               return traceConfigurations.Count;
         }
      }

      // Number of traces in the main audit SP
      public int TraceCountXE
      {
          get
          {
              if (traceConfigurationsXE == null)
                  return 0;
              else
                  return traceConfigurationsXE.Count;
          }
      }


      public DateTime LastModifiedTime
      {
         get { return lastModifiedTime; }
         set { lastModifiedTime = value; }
      }

      // Array of trace configurations
      public TraceConfiguration [] TraceConfigurations
      {
         get
         {
            return (TraceConfiguration [])traceConfigurations.ToArray( typeof(TraceConfiguration));
         }

         set
         {
            if( value == null )
               throw( new ArgumentNullException( "TraceConfigurations" ) );

            traceConfigurations.Clear();
            for( int i = 0; i < value.Length; i++ )
               traceConfigurations.Add( value[i] );

         }
      }

      // Array of trace configurations 
      public XeTraceConfiguration[] XeTraceConfigurations
      {
          get
          {
              return (XeTraceConfiguration[])traceConfigurationsXE.ToArray(typeof(XeTraceConfiguration));
          }

          set
          {
              if (value == null)
                  throw (new ArgumentNullException("XeTraceConfigurations"));

              traceConfigurationsXE.Clear();
              for (int i = 0; i < value.Length; i++)
                  traceConfigurationsXE.Add(value[i]);

          }
      }


       // End Of 
      #endregion

      #region Constructors

      public SPBuilder()
      {
         procedure = new StringBuilder( 1000 );
         traceConfigurations = new ArrayList();
         traceConfigurationsXE = new ArrayList();
         auditLogConfigurations = new ArrayList();
         configVersion = 0;
         agentVersion = CoreConstants.AgentVersion;
      }

      public SPBuilder( int serverVersion ) : this()
      {
         switch( serverVersion )
         {
            case 8:
            case 9:  // SQL Server 2005
            case 10: // SQL Server 2008
            case 11: // SQL Server 2012
            case 12: // SQL Server 2014
            case 13: // SQL Server 2016   
            case 14: // SQL Server 2017  Support SQL Server 2017
            case 15: // SQL Server 2019
               sqlVersion = serverVersion;
               break;
         }
      }

      #endregion

      #region Public Methods

      //---------------------------------------------------------------------
      // CreateStartUpSP
      //---------------------------------------------------------------------
      /// <summary>
      /// CreateStartUpSP
      /// </summary>
      /// <param name="exists">Set if Startup SP exists</param>
      /// <returns></returns>
      public string
      CreateStartUpSP( string instance, bool exists)
      {
         return CreateStartUpSP( instance, CoreConstants.Agent_AuditSPName, exists);
      }

      //---------------------------------------------------------------------
      // CreateStartUpSP
      //---------------------------------------------------------------------
      /// <summary>
      /// CreateStartUpSP
      /// </summary>
      /// <param name="mainSPName"></param>
      /// <param name="exists">Set if Startup SP exists</param>
      /// <returns></returns>
      public string
         CreateStartUpSP(
            string server,
            string mainSPName,
            bool exists
         )
      {
         string registryKey = String.Format( @"{0}\{1}", SQLcomplianceAgent.AgentRegistryKey, server );
         procedure.Remove( 0, procedure.Length );

         AddCopyright();
         // SQLCM 5.6- 566/740/4620/5280 (Non-Admin and Non-Sysadmin role) Permissions
         // Use Alter proc statement if SP already exists to persists permissions and startup execution of the stored procedure
         this.procedure.AppendFormat(exists ? AlterProc : CreateProc, CoreConstants.Agent_StartUpSPName);
         procedure.Append( SqlAs + SqlBegin + SetNoCountOn );

         // Declare return code variable
         procedure.AppendFormat( DeclareIntVariable, SPReturnCode );
         procedure.AppendFormat( DeclareIntVariable, SPTraceId );
         procedure.AppendFormat( DeclareNvarcharVariable, SPFileName, 245 );
         procedure.AppendFormat( DeclareIntVariable, SPOption );
         procedure.AppendFormat( DeclareBigIntVariable, SPMaxSize );
         procedure.AppendFormat( DeclareDateTimeVariable, SPStopTime );
         procedure.AppendFormat( DeclareIntVariable, SPStatus );
         procedure.AppendFormat( DeclareIntVariable, SPIndex );
         procedure.AppendFormat( DeclareVarcharVariable, SPValue, 30 );
         procedure.AppendFormat( DeclareIntVariable, SPRows );

         procedure.AppendFormat( DeleteRegistryValue, registryKey, CoreConstants.Agent_RegVal_StartupSPTraces );
         procedure.AppendFormat( AssignValue, SPValue, GetDateString );
         procedure.AppendFormat( AddToRegistryValue, registryKey, CoreConstants.Agent_RegVal_StartupSPTraces, SPValue );
         procedure.AppendFormat( AddToRegistryValueString, registryKey, CoreConstants.Agent_RegVal_StartupSPTraces, configVersion );
         procedure.AppendFormat( AddToRegistryValueString, registryKey, CoreConstants.Agent_RegVal_StartupSPTraces, agentVersion );

         procedure.AppendFormat( CreateTempTable, SPTempResult );
         procedure.AppendFormat( InsertStartResultToTempTable, SPTempResult, SPReturnCode, mainSPName );

         procedure.AppendFormat( "SELECT {0} = COUNT(id) From {1}\n", SPRows, SPTempResult );
         procedure.AppendFormat( BlockIfStatement, SPReturnCode, SqlEqual, 0 );

         procedure.AppendFormat( DeclareCursor, SPInfoCursor, String.Format( "SELECT num, id, name FROM {0}\n", SPTempResult ) );
         procedure.AppendFormat( OpenCursor, SPInfoCursor );
         procedure.AppendFormat( "FETCH NEXT FROM {0} INTO {1}, {2}, {3}\n",
            SPInfoCursor, SPIndex, SPTraceId, SPFileName );

         procedure.AppendFormat( "WHILE {0} > 0\n", SPRows );
         procedure.Append( SqlBegin );

         procedure.AppendFormat( BlockIfStatement, SPFileName, SqlIsNot, SqlNull );
         procedure.AppendFormat( AssignValue, SPValue, String.Format( "CONVERT( VARCHAR(10), {0})", SPIndex ));
         procedure.AppendFormat( AddToRegistryValue, registryKey, CoreConstants.Agent_RegVal_StartupSPTraces, SPValue );
         procedure.AppendFormat( AssignValue, SPValue, String.Format( "CONVERT( VARCHAR(10), {0})", SPTraceId ));
         procedure.AppendFormat( AddToRegistryValue, registryKey, CoreConstants.Agent_RegVal_StartupSPTraces, SPValue );
         procedure.AppendFormat( AddToRegistryValue, registryKey, CoreConstants.Agent_RegVal_StartupSPTraces, SPFileName );
         procedure.Append( SqlEnd );

         procedure.AppendFormat( "SET {0} = {0} - 1\n", SPRows );
         procedure.AppendFormat( "FETCH NEXT FROM {0} INTO {1}, {2}, {3}\n",
            SPInfoCursor, SPIndex, SPTraceId, SPFileName );
         procedure.Append( SqlEnd );

         procedure.AppendFormat( CloseCursor, SPInfoCursor );
         procedure.AppendFormat( DeallocateCursor, SPInfoCursor );
         procedure.Append( SqlEnd );

         procedure.AppendFormat( "DROP TABLE {0}\n", SPTempResult );
         procedure.AppendFormat( "Return {0}\n", SPReturnCode );
         procedure.Append( SqlEnd );


         return procedure.ToString();
      }

      //---------------------------------------------------------------------
      // CreateAuditSP
      //---------------------------------------------------------------------
      /// <summary>
      /// CreateAuditSP: This method generates the script to create the main
      /// SQLsecure audit control stored procedure.  A trace is created for
      /// each trace configuration stored in the object.
      /// </summary>
      /// <returns></returns>
      //-----------------------------------------------------------------------
      // This is the method to create the main SQLsecure audit stored proceduce.
      // The stored procedure is takes the following commands,
      //    - Start: starts traces
      //    - Stop:  stops traces
      //    - GetStatus: get the tracing status
      //    - GetInfo: get the audit configuration version, agent version and other
      //               information
      // Note that the SP is regenerated for each new configuration so the
      // configuration can be stored in the SP for later querying.
      //-----------------------------------------------------------------------
      public string
         CreateAuditSP()
      {
         // Clear the string builder
         procedure.Remove( 0, procedure.Length );

         AddCopyright();
         //
         // Create the header part
         //
         procedure.AppendFormat( CreateProc, CoreConstants.Agent_AuditSPName );
         procedure.Append(SQLSECURE_AuditSPParam1 + SQLSECURE_AuditSPParam2);
         procedure.Append( SqlAs + SqlBegin + SetNoCountOn );

         //
         // Declare variables and lists
         //
         procedure.AppendFormat( DeclareIntVariable, SPReturnCode );
         procedure.AppendFormat( DeclareIntVariable, SPTraceNumber );
         procedure.AppendFormat( DeclareIntVariable, SPTraceId );
         if( sqlVersion == 8 ) // SQL Server 2000
            procedure.AppendFormat( DeclareIntVariable, SPPermissions );
         else
            procedure.AppendFormat( DeclareBigIntVariable, SPPermissions );
         // Declare variables for each property
         procedure.AppendFormat( DeclareIntVariable, SPTraceOptions );
         procedure.AppendFormat( DeclareNvarcharVariable, SPFileName, 245 );
         procedure.AppendFormat( DeclareDateTimeVariable, SPStopTime );
         procedure.AppendFormat( DeclareIntVariable, SPStatus );
         procedure.AppendFormat( DeclareBigIntVariable, SPMaxSize );
         procedure.AppendFormat( DeclareIntVariable, SPRows );
         // Declare a table variable to hold the trace info records
         procedure.AppendFormat( DeclareTraceInfoTable, SPTraceInfoTable );
         procedure.AppendFormat( DeclareIntVariable, SPNumUsers );

         procedure.AppendFormat( DeclareIntVariable, "@i" );
         procedure.AppendFormat( DeclareIntVariable, "@j" );
         procedure.AppendFormat(DeclareIntVariable, "@length");

         procedure.AppendFormat( IfStatement, SPCommand, "=", 0 ); // No-op command
         procedure.Append( SqlPrint + "'No op'\n");

         // Generate the start trace script
         CreateStartCommandScript();

         // Generate the stop trace command script
         CreateStopCommandScript();

         // Get trace status command script
         CreateGetStatusCommandScript();

         // Get stored procedure information script
         CreateGetSPInfoCommandScript();

         //
         // Get running traces info
         //
         procedure.AppendFormat( BlockElseIfStatement, SPCommand, "=", SPCmd.GetRunningTraces.ToString("D"));
         CreateGetRunningTracesScript();
         procedure.Append( SqlEnd );

         //
         // Stop all traces
         //
         procedure.AppendFormat( BlockElseIfStatement, SPCommand, "=", SPCmd.StopAll.ToString("D"));
         CreateStopAllTraces( );
         procedure.Append( SqlEnd );
         //
         // Unknown command
         //
         procedure.Append( SqlElse );
         procedure.Append( SqlPrint + "'Unknown Command'\n");

         //
         //
         procedure.Append( ReturnSuccess );
         procedure.Append( SqlEnd );

         return procedure.ToString();
      }

      public string
        CreateAuditSPXE()
      {
          // Clear the string builder
          procedure.Remove(0, procedure.Length);

          AddCopyright();
          //
          // Create the header part
          //
          procedure.AppendFormat(CreateProc, CoreConstants.Agent_AuditSPNameXE);
          procedure.Append(SQLSECURE_AuditSPParam1 + SQLSECURE_AuditSPParam2);
          procedure.Append(SqlAs + SqlBegin + SetNoCountOn);

          //
          // Declare variables and lists
          //
          procedure.AppendFormat(DeclareIntVariable, SPReturnCode);
          procedure.AppendFormat(DeclareIntVariable, SPTraceNumber);
          procedure.AppendFormat(DeclareIntVariable, SPTraceId);
          if (sqlVersion == 8) // SQL Server 2000
              procedure.AppendFormat(DeclareIntVariable, SPPermissions);
          else
              procedure.AppendFormat(DeclareBigIntVariable, SPPermissions);
          // Declare variables for each property
          procedure.AppendFormat(DeclareIntVariable, SPTraceOptions);
          procedure.AppendFormat(DeclareNvarcharVariable, SPFileName, 245);
          procedure.AppendFormat(DeclareDateTimeVariable, SPStopTime);
          procedure.AppendFormat(DeclareIntVariable, SPStatus);
          procedure.AppendFormat(DeclareBigIntVariable, SPMaxSize);
          procedure.AppendFormat(DeclareIntVariable, SPRows);
          // Declare a table variable to hold the trace info records
          procedure.AppendFormat(DeclareTraceInfoTable, SPTraceInfoTableXE);
          procedure.AppendFormat(DeclareNvarcharVariable, SessionNameXE, 100);
          procedure.AppendFormat(DeclareIntVariable, SPNumUsers);

          procedure.AppendFormat(DeclareIntVariable, "@i");
          procedure.AppendFormat(DeclareIntVariable, "@j");
          procedure.AppendFormat(DeclareIntVariable, "@length");

          if (sqlVersion == 8)
              CheckPermissions(SysAdminRole);

          procedure.AppendFormat(IfStatement, SPCommand, "=", 0); // No-op command
          procedure.Append(SqlPrint + "'No op'\n");
          // Generate the start trace script
          CreateStartCommandScriptXE();

          // Generate the stop trace command script
          CreateStopCommandScriptXE();

          // Get trace status command script
          CreateGetStatusCommandScriptXE();

          // Get stored procedure information script
          CreateGetSPInfoCommandScriptXE();

          //
          // Get running traces info
          //
          procedure.AppendFormat(BlockElseIfStatement, SPCommand, "=", SPCmd.GetRunningTraces.ToString("D"));
          CreateGetRunningTracesScriptXE();
          procedure.Append(SqlEnd);

          //
          // Stop all traces
          //
          procedure.AppendFormat(BlockElseIfStatement, SPCommand, "=", SPCmd.StopAll.ToString("D"));
          CreateStopAllTracesXE();
          procedure.Append(SqlEnd);
          //
          // Unknown command
          //
          procedure.Append(SqlElse);
          procedure.Append(SqlPrint + "'Unknown Command'\n");

          //
          //
          procedure.Append(ReturnSuccess);
          procedure.Append(SqlEnd);

          return procedure.ToString();
      }

      //---------------------------------------------------------------------
      // AddCopyright: add copyright message to the stored procedure.
      //---------------------------------------------------------------------
      private void
         AddCopyright()
      {
         procedure.AppendFormat( "-- IDERA SQL compliance Manager Version {0}\n", SQLcomplianceAgent.Instance.AgentVersion );
         procedure.Append( "--\n" );
         procedure.Append( "-- (c) Copyright 2004-2021 IDERA, Inc., All Rights Reserved.\n" );

         procedure.Append( "-- SQL Compliance Manager, IDERA and the IDERA Logo are trademarks or registered trademarks\n" );

         procedure.Append( "-- of IDERA or its subsidiaries in the United States and other jurisdictions.\n\n" );
      }




      //---------------------------------------------------------------------
      // CheckPermissions: Check if the current user is a member of a specified
      //                   server role
      //---------------------------------------------------------------------
      public void
         CheckPermissions(
            string roleName
         )
      {
         string isMember = "@isMember";
         procedure.AppendFormat( "Declare {0} int\n", isMember );
         procedure.AppendFormat( "SELECT {0} = is_srvrolemember('{1}')\n", isMember, roleName );
         procedure.AppendFormat( "IF {0} is null\nbegin\n", isMember );
         procedure.AppendFormat( "raiserror( '{0}', 16, 1 )\n", CoreConstants.Exception_UserDoesNotHavePermissionOnThisSP );
         procedure.AppendFormat( "return (1) \n" );
         procedure.Append( SqlEnd );
         procedure.AppendFormat( "IF {0} = 0\nbegin\n", isMember );
         procedure.AppendFormat( "raiserror( '{0}', 16, 1 )\n", CoreConstants.Exception_UserDoesNotHavePermissionOnThisSP );
         procedure.AppendFormat( "return (1) \n" );
         procedure.Append( SqlEnd );
      }

      //---------------------------------------------------------------------
      // AddTrace
      //---------------------------------------------------------------------
      /// <summary>
      /// AddTrace: add a new trace configuration to the audit stored procedure.
      /// </summary>
      /// <param name="traceConfig"></param>
      public void 
         AddTrace (
         TraceConfiguration traceConfig
         )
      {
         // Treat null configuration as an error to avoid programming
         // error.
         if( traceConfig == null )
            throw( new ArgumentNullException( "traceConfig" ) );

         // Check for duplicates
         if( !traceConfigurations.Contains( traceConfig ) )
            traceConfigurations.Add( traceConfig );
      }

      
      //---------------------------------------------------------------------
      // RemoveTrace
      //---------------------------------------------------------------------
      /// <summary>
      /// RemoveTrace
      /// </summary>
      /// <param name="traceConfig"></param>
      public void
         RemoveTrace(
         TraceConfiguration traceConfig
         )
      {
         // Treat null configuration as an error to avoid programming
         // error.
         if( traceConfig == null )
            throw( new ArgumentNullException( "traceConfig" ) );

         // Verify if it is in the list
         if( traceConfigurations.Contains( traceConfig ) )
         {
            traceConfigurations.Remove( traceConfig );
         }
      }

      //---------------------------------------------------------------------
      // ClearTraceConfigurations
      //---------------------------------------------------------------------
      /// <summary>
      /// ClearTraceConfigurations: remove all the configured traces
      /// </summary>
      public void
         ClearTraceConfigurations()
      {
         if( traceConfigurations != null )
            traceConfigurations.Clear();
      }


      #endregion

      #region Protected Methods

      //-----------------------------------------------------------------------
      //  CreateList() creates a varchar variable and stores an array of integer
      //  values in a string as a comma separated list.  Note that the trailing
      //  comma is added for later processing (i.e. parsing).
      //-----------------------------------------------------------------------
      /// <summary>
      /// 
      /// </summary>
      /// <param name="listVariable"></param>
      /// <param name="list"></param>
      protected virtual void 
         CreateList (
         string listVariable,
         int [] list
         )
      {
         procedure.Append( SqlSet + listVariable + "='" );

         for( int i = 0; i < list.Length ; i++ )
            procedure.AppendFormat( "{0},", list[i] );
         procedure.Append("'\n");
      }

      //---------------------------------------------------------------------
      // CreateStartCommandScript
      //---------------------------------------------------------------------
      /// <summary>
      /// CreateStartCommandScript
      /// </summary>
      protected virtual void
         CreateStartCommandScript ()
      {
         //
         // Start tracing statements
         //
         procedure.AppendFormat( BlockElseIfStatement, SPCommand, "=", SPCmd.Start.ToString("D"));

         // Declare variables used in this command
         procedure.AppendFormat( DeclareVarcharVariable, SPEvents, EventListVariableLength );
         procedure.AppendFormat( DeclareVarcharVariable, SPCols, ColumnListVariableLength );
         procedure.AppendFormat( DeclareVarcharVariable, SPTempEvents, EventListVariableLength );
         procedure.AppendFormat( DeclareVarcharVariable, SPTempCols, ColumnListVariableLength );
         procedure.AppendFormat( DeclareVarcharVariable, SPColStr, ColumnListVariableLength );
         procedure.AppendFormat( DeclareVarcharVariable, SPFileDate, 32 );
         // These three variables are using in CreateTraceStatements();
         procedure.Append( "DECLARE @Event int, @Col int, @OnOff bit\n");
         // The two variables below are for privileged user traces
         procedure.AppendFormat( DeclareUserNameTable, SPUsers );
         procedure.AppendFormat( DeclareNvarcharVariable, SPPUser, 128 );

         // Create file name suffix for the traces
         procedure.AppendFormat( BuildFileDate, SPFileDate );

         // No option specified, start all traces
         procedure.AppendFormat( IfStatement, SPOption, SqlIs, SqlNull );
         procedure.AppendFormat( AssignValue, SPTraceNumber, 0 );

         // Convert the option to trace number
         procedure.Append( SqlElse + "\n" );
         procedure.AppendFormat("SET {0}=CAST({1} AS int)\n", SPTraceNumber, SPOption );

         // Validate trace number 
         procedure.AppendFormat( BlockIfStatement, SPTraceNumber, SqlGreater, traceConfigurations.Count );
         //Invalid trace number, set return code and go to the end of the start trace block
         procedure.AppendFormat( AssignValue, SPReturnCode, -1 );
         procedure.AppendFormat( GotoStatement, SPEndStartTrace );
         procedure.Append( SqlEnd );

                                 
         // Create start trace statements for each trace configuration
         for( int i = 0; i < traceConfigurations.Count; i++ )
         {
            // Starting all traces or this individual trace?
            procedure.AppendFormat( BlockIfStatement, 
               String.Format( BinaryExpression, SPTraceNumber, SqlEqual, 0 ),
               SqlOr,
               String.Format( BinaryExpression, SPTraceNumber, SqlEqual, i+1 ) );

            CreateTraceStatements ( (TraceConfiguration)traceConfigurations[i], i+1 );

            // If the call is to start individual trace, go to the end of
            // start trace block
            procedure.AppendFormat( IfStatement, SPTraceNumber,  SqlNotEqual, 0 );
            procedure.AppendFormat( GotoStatement, SPEndStartTrace );
            procedure.Append( SqlEnd );

         }
         // Create the End of Start Trace label
         procedure.AppendFormat( "{0}:\n", SPEndStartTrace );
         // Create result set
         procedure.AppendFormat( "select * from {0}\n", SPTraceInfoTable );
         procedure.Append( SqlEnd );
      }

       //5.4 XE
      protected virtual void
        CreateStartCommandScriptXE()
      {
          //
          // Start tracing statements
          //
          procedure.AppendFormat(BlockElseIfStatement, SPCommand, "=", SPCmd.Start.ToString("D"));

          // Declare variables used in this command
          procedure.AppendFormat(DeclareVarcharVariable, SPFileDate, 32);
          // Create file name suffix for the traces
          procedure.AppendFormat(BuildFileDate, SPFileDate);
          // The two variables below are for privileged user traces
          procedure.AppendFormat(DeclareUserNameTable, SPUsers);
          procedure.AppendFormat(DeclareNvarcharVariable, SPPUser, 128);
          procedure.AppendFormat(DeclareNvarcharVariable, SPQueryXE, "MAX");
          procedure.AppendFormat(DeclareNvarcharVariable, SPSessionIdXE, 150);
          

          // No option specified, start all traces
          procedure.AppendFormat(IfStatement, SPOption, SqlIs, SqlNull);
          procedure.AppendFormat(AssignValue, SPTraceNumber, 0);

          // Convert the option to trace number
          procedure.Append(SqlElse + "\n");
          procedure.AppendFormat("SET {0}=CAST({1} AS int)\n", SPTraceNumber, SPOption);

          // Validate trace number 
          procedure.AppendFormat(BlockIfStatement, SPTraceNumber, SqlGreater, traceConfigurationsXE.Count);
          //Invalid trace number, set return code and go to the end of the start trace block
          procedure.AppendFormat(AssignValue, SPReturnCode, -1);
          procedure.AppendFormat(GotoStatement, SPEndStartTrace);
          procedure.Append(SqlEnd);


          // Create start trace statements for each trace configuration
          for (int i = 0; i < traceConfigurationsXE.Count; i++)
          {

              procedure.AppendFormat(AssignValue, SPMaxSize, maxTraceSize);
              // Starting all traces or this individual trace?
              procedure.AppendFormat(BlockIfStatement,
                 String.Format(BinaryExpression, SPTraceNumber, SqlEqual, 0),
                 SqlOr,
                 String.Format(BinaryExpression, SPTraceNumber, SqlEqual, i + 1));
              CreateTraceStatementsXE((XeTraceConfiguration)traceConfigurationsXE[i], i + 1);

              // If the call is to start individual trace, go to the end of
              // start trace block
              procedure.AppendFormat(IfStatement, SPTraceNumber, SqlNotEqual, 0);
              procedure.AppendFormat(GotoStatement, SPEndStartTrace);
              procedure.Append(SqlEnd);

          }
          // Create the End of Start Trace label
          procedure.AppendFormat("{0}:\n", SPEndStartTrace);
          // Create result set
          procedure.AppendFormat("select * from {0}\n", SPTraceInfoTable);
          procedure.Append(SqlEnd);
      }


      //---------------------------------------------------------------------
      // CreateStopCommandScript
      //---------------------------------------------------------------------
      /// <summary>
      /// CreateStopCommandScript: generate the scripts for stopping traces.
      /// </summary>
      protected virtual void
         CreateStopCommandScript()
      {
          //
          // Stop tracing statements
          //
          procedure.AppendFormat(BlockElseIfStatement, SPCommand, "=", SPCmd.Stop.ToString("D"));

          // check for null option
          procedure.AppendFormat(IfStatement, SPOption, SqlIs, SqlNull);
          procedure.Append(ReturnSuccess); // TODO: returns an error and error message

          // Trace IDs are passed in from the option as a list
          // Append a comma to the trace ID list
          procedure.AppendFormat("SET @length = LEN({0})\n", SPOption);
          procedure.AppendFormat("IF RIGHT({0},1)<>',' SET {0}={0}+','\n", SPOption/*, SPOption, SPOption*/ );


          // Parse the trace ID list in a while loop and stop each trace
          procedure.AppendFormat("SET @j=CHARINDEX(',',{0})\n", SPOption);
          procedure.Append("WHILE @j<>0 BEGIN\n");
          // Get one trace ID
          procedure.AppendFormat("SET {0}=CAST(LEFT({1},@j-1) AS int)\n", SPTraceId, SPOption);
          // Stop the trace
          procedure.AppendFormat(SetTraceStatusStatement, SPReturnCode, SPTraceId, TraceStatus.Stop.ToString("D"));
          // Close the trace
          procedure.AppendFormat(SetTraceStatusStatement, SPReturnCode, SPTraceId, TraceStatus.Close.ToString("D"));
          // Remove this trace ID
          procedure.AppendFormat("SET {0}=SUBSTRING({1},@j+1,@length)\n", SPOption, SPOption);
          procedure.AppendFormat("SET @j=CHARINDEX(',',{0})\n", SPOption);
          procedure.Append(SqlEnd);

          // End of the outter IF statement
          procedure.Append(SqlEnd);
      }

       //5.4 XE
      protected virtual void
        CreateStopCommandScriptXE()
      {
          //
          // Stop tracing statements
          //
          procedure.AppendFormat(BlockElseIfStatement, SPCommand, "=", SPCmd.Stop.ToString("D"));

          // check for null option
          procedure.AppendFormat(IfStatement, SPOption, SqlIs, SqlNull);
          procedure.Append(ReturnSuccess); // TODO: returns an error and error message

          // Trace IDs are passed in from the option as a list
          // Append a comma to the trace ID list
          procedure.AppendFormat("SET @length = LEN({0})\n", SPOption);
          procedure.AppendFormat("IF RIGHT({0},1)<>',' SET {0}={0}+','\n", SPOption/*, SPOption, SPOption*/ );


          // Parse the trace ID list in a while loop and stop each trace
          procedure.AppendFormat("SET @j=CHARINDEX(',',{0})\n", SPOption);
          procedure.Append("WHILE @j<>0 BEGIN\n");
          // Get one trace ID
          procedure.AppendFormat("SET {0}=CAST(LEFT({1},@j-1) AS int)\n", SPTraceId, SPOption);
          // Stop and Drop session
          procedure.AppendFormat("IF EXISTS(SELECT * FROM sys.server_event_sessions WHERE event_session_id={0})\n", SPTraceId);
          procedure.Append(SqlBegin);
          procedure.AppendFormat(SetSessionNameXE);
          procedure.AppendFormat("\nEXEC ('" + DropSessionXE + "')\n", SessionNameXE);
          procedure.Append(SqlEnd);
          // Remove this trace ID
          procedure.AppendFormat("SET {0}=SUBSTRING({1},@j+1,@length)\n", SPOption, SPOption);
          procedure.AppendFormat("SET @j=CHARINDEX(',',{0})\n", SPOption);
          procedure.Append(SqlEnd);

          // End of the outter IF statement
          procedure.Append(SqlEnd);
      }

      //---------------------------------------------------------------------
      // CreateGetStatusCommandScript
      //---------------------------------------------------------------------
      /// <summary>
      /// CreateGetStatusCommandScript: generates the scripts to gather trace status and information.
      /// </summary>
      protected virtual void
         CreateGetStatusCommandScript()
      {
         //
         // Get status statements
         //
         procedure.AppendFormat( BlockElseIfStatement, SPCommand, "=", SPCmd.GetStatus.ToString("D"));
         // Create get status statements
         procedure.Append( SqlPrint + "'get status'\n");
         // check for null option
         procedure.AppendFormat( IfStatement, SPOption, SqlIs, SqlNull );
         procedure.Append( ReturnSuccess ); // TODO: returns an error and error message

         // Trace IDs are passed in from the option as a list
         // Append a comma to the trace ID list
         procedure.AppendFormat("SET @length = LEN({0})\n", SPOption);
         procedure.AppendFormat("IF RIGHT({0},1)<>',' SET {0}={0}+','\n", SPOption/*, SPOption, SPOption*/ );


         // Parse the trace ID list in a while loop and get trace info for each trace
         procedure.AppendFormat( "SET @j=CHARINDEX(',',{0})\n", SPOption );
         procedure.Append( "WHILE @j<>0 BEGIN\n" );
         // Get one trace ID
         procedure.AppendFormat("SET {0}=CAST(LEFT({1},@j-1) AS int)\n", SPTraceId, SPOption );

         procedure.AppendFormat( DeclareIntVariable, SPProperty );
         procedure.AppendFormat( DeclareVariantVariable, SPValue );

         // Declare a cursor for the result set
         procedure.AppendFormat( DeclareCursor, SPInfoCursor, String.Format( GetTraceInfoStatement, SPTraceId ) );
         procedure.AppendFormat( OpenCursor, SPInfoCursor );
         procedure.AppendFormat( "FETCH NEXT FROM {0} INTO {1}, {2}\n", SPInfoCursor, SPProperty, SPValue );

         // Use a while loop to fetch all the properties
         procedure.AppendFormat( "WHILE @@FETCH_STATUS = 0\n");
         procedure.Append( SqlBegin );

         // Identify each property and save its value into the corresponding varaible
         //   Options     = 1,
         procedure.AppendFormat( IfStatement, SPProperty, SqlEqual, TraceInfoProperty.Options.ToString("D") );
         procedure.AppendFormat( AssignValue, SPTraceOptions, String.Format( CastType, SPValue, SqlInt ) );
         //   FileName    = 2,
         procedure.AppendFormat( ElseIfStatement, SPProperty, SqlEqual, TraceInfoProperty.FileName.ToString("D") );
         procedure.AppendFormat( AssignValue, SPFileName, String.Format( CastType, SPValue, "nvarchar(245)" ) );
         //   MaxSize     = 3,
         procedure.AppendFormat( ElseIfStatement, SPProperty, SqlEqual, TraceInfoProperty.MaxSize.ToString("D") );
         procedure.AppendFormat( AssignValue, SPMaxSize, String.Format( CastType, SPValue, SqlBigInt ) );
         //   StopTime    = 4,
         procedure.AppendFormat( ElseIfStatement, SPProperty, SqlEqual, TraceInfoProperty.StopTime.ToString("D") );
         procedure.AppendFormat( AssignValue, SPStopTime, String.Format( CastType, SPValue, SqlDateTime ) );
         //   Status      = 5
         procedure.AppendFormat( ElseIfStatement, SPProperty, SqlEqual, TraceInfoProperty.Status.ToString("D") );
         procedure.AppendFormat( AssignValue, SPStatus, String.Format( CastType, SPValue, SqlInt ) );

         procedure.AppendFormat( "FETCH NEXT FROM {0} INTO {1}, {2}\n", SPInfoCursor, SPProperty, SPValue );

         procedure.Append( SqlEnd );
         procedure.AppendFormat( CloseCursor, SPInfoCursor );
         procedure.AppendFormat( DeallocateCursor, SPInfoCursor );

         // Insert a trace info record to the result set table
         procedure.AppendFormat( BlockIfStatement, SPFileName, SqlIsNot, SqlNull );
         procedure.AppendFormat( "INSERT {0} VALUES ( 0, {1}, {2}, {3}, {4}, {5}, {6} )\n",
            SPTraceInfoTable, SPTraceId, SPFileName, SPTraceOptions, SPMaxSize, SPStopTime, SPStatus );

         procedure.AppendFormat( AssignValue, SPFileName, SqlNull );
         procedure.Append( SqlEnd );


         // Get property info for the trace
         // Remove this trace ID from the list
         procedure.AppendFormat( "SET {0}=SUBSTRING({1},@j+1,@length)\n", SPOption, SPOption );
         procedure.AppendFormat( "SET @j=CHARINDEX(',',{0})\n", SPOption );
         procedure.Append( SqlEnd );  // End of the WHILE parsing loop

         // Create the result set
         procedure.AppendFormat( "select * from {0}\n", SPTraceInfoTable );
         procedure.Append( SqlEnd );

      }

       //5.4 XE
      protected virtual void
        CreateGetStatusCommandScriptXE()
      {
          string prefix = "XE" + instanceAlias;
          prefix = prefix.Replace("-", "_");
          //
          // Get status statements
          //
          procedure.AppendFormat(BlockElseIfStatement, SPCommand, "=", SPCmd.GetStatus.ToString("D"));
          // Create get status statements
          procedure.Append(SqlPrint + "'get status'\n");
          // check for null option
          procedure.AppendFormat(IfStatement, SPOption, SqlIs, SqlNull);
          procedure.Append(ReturnSuccess); // TODO: returns an error and error message

          // Trace IDs are passed in from the option as a list
          // Append a comma to the trace ID list
          procedure.AppendFormat("SET @length = LEN({0})\n", SPOption);
          procedure.AppendFormat("IF RIGHT({0},1)<>',' SET {0}={0}+','\n", SPOption/*, SPOption, SPOption*/ );


          // Parse the trace ID list in a while loop and get trace info for each trace
          procedure.AppendFormat("SET @j=CHARINDEX(',',{0})\n", SPOption);
          procedure.Append("WHILE @j<>0 BEGIN\n");
          // Get one trace ID
          procedure.AppendFormat("SET {0}=CAST(LEFT({1},@j-1) AS int)\n", SPTraceId, SPOption);

          procedure.AppendFormat(DeclareNvarcharVariable, EventSessionName, 100);
          procedure.AppendFormat(DeclareNvarcharVariable, FileNameWithPath, Max);
          procedure.AppendFormat(DeclareBigIntVariable, MaxMemory);

          // Declare a cursor for the result set
          procedure.AppendFormat(DeclareCursor, SPInfoCursor, String.Format(GetTraceInfoQueryXE, SPTraceId));
          procedure.AppendFormat(OpenCursor, SPInfoCursor);
          procedure.AppendFormat("FETCH NEXT FROM {0} INTO {1}, {2}\n", SPInfoCursor, EventSessionName, MaxMemory);

          // Use a while loop to fetch all the properties
          procedure.AppendFormat("WHILE @@FETCH_STATUS = 0\n");
          procedure.Append(SqlBegin);

          // Identify each property and save its value into the corresponding varaible
          //   Options     = 1,
          procedure.AppendFormat(AssignValue, SPTraceOptions, 2);
          procedure.AppendFormat(AssignValue, FileNameWithPath, GetFileNameWithPath);
          //   FileName    = 2,
          //   Status      = 5
          procedure.AppendFormat(BlockIfStatement, FileNameWithPath, SqlIs, "NULL");
          procedure.AppendFormat(AssignValue, SPFileName, SessoinToFileName);
          procedure.AppendFormat(AssignValue, SPStatus, 0);
          procedure.AppendFormat(SqlEnd);
          procedure.AppendFormat(BlockElseStatement);
          procedure.AppendFormat(AssignValue, SPFileName, String.Format(GetPathFromFileName, FileNameWithPath) + " + " + SessoinToFileName);
          procedure.AppendFormat(AssignValue, SPStatus, 1);
          procedure.AppendFormat(SqlEnd);
          //   MaxSize     = 3,
          procedure.AppendFormat(AssignValue, SPMaxSize, MaxMemory + "/1024");
          //   StopTime    = 4,
          procedure.AppendFormat(AssignValue, SPStopTime, "NULL");
          

          procedure.AppendFormat("FETCH NEXT FROM {0} INTO {1}, {2}\n", SPInfoCursor, EventSessionName, MaxMemory);

          procedure.Append(SqlEnd);
          procedure.AppendFormat(CloseCursor, SPInfoCursor);
          procedure.AppendFormat(DeallocateCursor, SPInfoCursor);

          // Insert a trace info record to the result set table
          procedure.AppendFormat(BlockIfStatement, SPFileName, SqlIsNot, SqlNull);
          procedure.AppendFormat("INSERT {0} VALUES ( 0, {1}, {2}, {3}, {4}, {5}, {6} )\n",
             SPTraceInfoTable, SPTraceId, SPFileName, SPTraceOptions, SPMaxSize, SPStopTime, SPStatus);

          procedure.AppendFormat(AssignValue, SPFileName, SqlNull);
          procedure.Append(SqlEnd);


          // Get property info for the trace
          // Remove this trace ID from the list
          procedure.AppendFormat("SET {0}=SUBSTRING({1},@j+1,@length)\n", SPOption, SPOption);
          procedure.AppendFormat("SET @j=CHARINDEX(',',{0})\n", SPOption);
          procedure.Append(SqlEnd);  // End of the WHILE parsing loop

          // Create the result set
          procedure.AppendFormat("select * from {0}\n", SPTraceInfoTable);
          procedure.Append(SqlEnd);

      }

      //---------------------------------------------------------------------
      // CreateGetSPInfoCommandScript
      //---------------------------------------------------------------------
      /// <summary>
      /// CreateGetSPInfoCommandScript: creates the script to return the stored procedure information.
      /// </summary>
      protected virtual void
         CreateGetSPInfoCommandScript()
      {
         //
         // Get trace information statements
         //
         procedure.AppendFormat( BlockElseIfStatement, SPCommand, "=", SPCmd.GetInfo.ToString("D"));
         // Create SP Info table variable
         procedure.Append( "DECLARE @SPInfo table ( [CreateTime] [nvarchar] (40), [Agent Verion] [nvarchar] (40), ");
         procedure.Append( "                       [configVersion] [int], [NumTraces] [int] )\n" );
         procedure.AppendFormat( "INSERT @SPInfo VALUES ( '{0}', '{1}', {2}, {3} )", 
            lastModifiedTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), agentVersion, configVersion, traceConfigurations.Count );
         procedure.Append( "SELECT * FROM @SPInfo\n");
         procedure.Append( SqlEnd );

      }

       //5.4 XE
      protected virtual void
        CreateGetSPInfoCommandScriptXE()
      {
          //
          // Get trace information statements
          //
          procedure.AppendFormat(BlockElseIfStatement, SPCommand, "=", SPCmd.GetInfo.ToString("D"));
          // Create SP Info table variable
          procedure.Append("DECLARE @SPInfo table ( [CreateTime] [nvarchar] (40), [Agent Verion] [nvarchar] (40), ");
          procedure.Append("                       [configVersion] [int], [NumTraces] [int] )\n");
          procedure.AppendFormat("INSERT @SPInfo VALUES ( '{0}', '{1}', {2}, {3} )",
             lastModifiedTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), agentVersion, configVersion, traceConfigurationsXE.Count);
          procedure.Append("SELECT * FROM @SPInfo\n");
          procedure.Append(SqlEnd);

      }


      //---------------------------------------------------------------------
      // CreateTraceStatements
      //---------------------------------------------------------------------
      /// <summary>
      /// 
      /// </summary>
      /// <param name="config"></param>
      /// <param name="traceNumber"></param>
      protected virtual void
         CreateTraceStatements (
            TraceConfiguration config,
            int traceNumber
         )
      {
         if( config == null )
            throw new ArgumentNullException( "config" );

         TraceFilter [] filters = config.GetTraceFilters();

         // Initialize variables
         if( maxUnattendedTime <= 0 )
            procedure.AppendFormat( AssignValue, SPStopTime, SqlNull );
         else
            procedure.AppendFormat( "Set {0} = DATEADD( dd, {1}, GETDATE())", SPStopTime, maxUnattendedTime );
         procedure.AppendFormat( AssignValue, SPMaxSize, maxTraceSize ); 
         CreateList( SPEvents, config.GetTraceEventsAsIntArray() );
         CreateList( SPTempCols, config.GetTraceColumnsAsIntArray() );

         // Create the trace
         procedure.AppendFormat( BuildFileName, SPFileName, config.FileName, SPFileDate );
         procedure.AppendFormat( CreateTrace, 
                                 SPReturnCode, 
                                 SPTraceId, 
                                 (int)options, 
                                 SPFileName, 
                                 SPMaxSize, 
                                 SPStopTime );
         procedure.AppendFormat( BlockIfStatement, SPReturnCode, ">", 0 );
         // handle create trace file error
         procedure.Append( SqlPrint + "'Error creating the tracefile'\n" );
         procedure.AppendFormat( BlockIfStatement, SPReturnCode, "=", 12 );
         string msg = String.Format( "Error creating trace: %s. Error code = %d.\n"
                       + "This may be due to the SQL Server service account having insufficient rights to write to the SQLcompliance Agent trace directory.\n"
                       + "Make sure that the service account either has the right to write to that directory or that it is a member of the local Administrators group." );

         procedure.AppendFormat( "RAISERROR('{0}', 16, 1, {1}, {2}) ", 
                                 msg,
                                 SPFileName, 
                                 SPReturnCode );
         procedure.Append( SqlEnd );
         procedure.Append( "ELSE BEGIN\n" );
         procedure.AppendFormat( "RAISERROR('Error creating trace: %s. Error code = %d.', 16, 1, {0}, {1}) ", 
                                 SPFileName, 
                                 SPReturnCode );
         procedure.Append( SqlEnd );

         procedure.AppendFormat( "Return ({0})", SPReturnCode );

         procedure.Append( SqlEnd );
         procedure.AppendFormat( BlockIfStatement, SPTraceId, "<=", 0 );
         procedure.Append( SqlPrint + "'Invalid trace ID'\n" );
         procedure.AppendFormat( "RAISERROR('Invalid trace ID: %d.', 16, 1, {0}) ", 
                                 SPTraceId );
         procedure.AppendFormat( "Return (1)", SPReturnCode );

         procedure.Append( SqlEnd );

         // Set audit events
         //   -- Set server trace events and columns
         procedure.AppendFormat( "IF {0} IS NOT NULL BEGIN\n", SPEvents );

         procedure.AppendFormat( AssignValue, SPOn, 1 );
          
         procedure.AppendFormat( "SET @i=CHARINDEX(',',{0})\n", SPEvents );
         procedure.Append( "WHILE @i<>0 BEGIN\n" );
         procedure.AppendFormat("SET @Event=CAST(LEFT({0},@i-1) AS int)\n", SPEvents );
         procedure.AppendFormat( AssignValue, SPColStr, SPTempCols );
         procedure.AppendFormat( "SET @j=CHARINDEX(',',{0})\n", SPColStr );
         procedure.Append( "WHILE @j<>0 BEGIN\n" );
         procedure.AppendFormat("SET @Col=CAST(LEFT({0},@j-1) AS int)\n", SPColStr );
         procedure.AppendFormat( SetEventStatement, SPReturnCode, SPTraceId, SPEvent, SPCol, SPOn );
         procedure.AppendFormat( "SET {0}=SUBSTRING(@ColStr,@j+1,300)\n", SPColStr );
         procedure.AppendFormat( "SET @j=CHARINDEX(',',{0})\n", SPColStr );
         procedure.Append( SqlEnd );
         procedure.AppendFormat( "SET {0}=SUBSTRING({1},@i+1,300)\n", SPEvents, SPEvents );
         procedure.AppendFormat( "SET @i=CHARINDEX(',',{0})\n", SPEvents );
         procedure.Append( SqlEnd );
         procedure.Append( SqlEnd );


         if( config.Level == TraceLevel.User )
         {
            procedure.AppendFormat( AssignValue , SPNumUsers, 0 );
            procedure.AppendFormat( "SET {0} = {0} + {1}\n", SPNumUsers, config.PrivUserCount );
            // If there are no audited privileged users, don't start the trace
            procedure.AppendFormat( "IF {0} = 0 BEGIN\n", SPNumUsers );
            procedure.AppendFormat( SetTraceStatusStatement, SPReturnCode, SPTraceId, TraceStatus.Close.ToString("D") );
            procedure.AppendFormat( GotoStatement, SPEndStartTrace );
            procedure.Append( SqlEnd );
         }


         // Set filters
         if( filters != null )
         {
            for( int i = 0; i < filters.Length; i++ )
            {
               if( filters[i].Type == TraceFilterType.nvarchar )
                  procedure.AppendFormat( SetTextFilterStatement, 
                     SPReturnCode, 
                     SPTraceId, 
                     filters[i].ColumnId.ToString("D"), 
                     filters[i].LogicalOp.ToString("D"),
                     filters[i].CompareOp.ToString("D"),
                     SQLHelpers.CreateEscapedString(filters[i].GetTextValue()));
               else if( filters[i].ColumnId == TraceColumnId.Permissions )
               {
                  procedure.AppendFormat( AssignValue, SPPermissions, filters[i].GetIntValue() );
                  procedure.AppendFormat( SetFilterStatement, 
                     SPReturnCode, 
                     SPTraceId, 
                     filters[i].ColumnId.ToString("D"), 
                     filters[i].LogicalOp.ToString("D"),
                     filters[i].CompareOp.ToString("D"),
                     SPPermissions );
               }
               else
                  procedure.AppendFormat( SetFilterStatement, 
                     SPReturnCode, 
                     SPTraceId, 
                     filters[i].ColumnId.ToString("D"), 
                     filters[i].LogicalOp.ToString("D"),
                     filters[i].CompareOp.ToString("D"),
                     filters[i].GetIntValue() );
               procedure.AppendFormat( BlockIfStatement, SPReturnCode, ">", 0 );
               procedure.AppendFormat( "Print  'Error setting trace filter - RETCODE='+CAST({0} AS VARCHAR(10))\n", SPReturnCode );
               procedure.AppendFormat( "Print  '  Column = {0}'\n", filters[i].ColumnId );
               procedure.AppendFormat( "Print  '  Comparison Op = {0}'\n", filters[i].CompareOp );

               if (filters[i].Type == TraceFilterType.nvarchar)
                  procedure.AppendFormat( "Print  '  Value = {0}'\n", SQLHelpers.CreateEscapedString((string)filters[i].GetValue()));
               else
                  procedure.AppendFormat( "Print  '  Value = {0}'\n", filters[i].GetValue());
               
               procedure.AppendFormat( "Print  '  Logical Op = {0}'\n", filters[i].LogicalOp );
               procedure.AppendFormat( "RAISERROR('Error creating trace filter.  Error code = %d', 16, 1, {0} )", SPReturnCode );
               procedure.AppendFormat( "Return ({0})", SPReturnCode );
               procedure.Append(SqlEnd);
            }
         }
                   
         // Start tracing
         procedure.AppendFormat( SetTraceStatusStatement, SPReturnCode, SPTraceId, 1 );  // Turn on the trace
         procedure.AppendFormat( BlockIfStatement, SPReturnCode, ">", 0 );
         // add error handling here
         procedure.Append( SqlPrint + "'Error starting the trace'\n");
         procedure.AppendFormat( ReturnStatement, -1 );
         procedure.Append( SqlEnd );
         // Insert a trace info record into the result set
         procedure.Append( SqlElse );
         procedure.AppendFormat( "INSERT {0} VALUES ( {1}, {2}, {3}, {4}, {5}, {6}, {7} )\n",
            SPTraceInfoTable, traceNumber, SPTraceId, SPFileName, config.Options.ToString("D"), SPMaxSize, SPStopTime, 1 );
                                
      }

       //5.4 XE

      protected virtual void
       CreateTraceStatementsXE(
          XeTraceConfiguration config,
          int traceNumber
       )
      {
          if (config == null)
              throw new ArgumentNullException("config");

          TraceFilter[] filters = config.GetTraceFilters();
                   
          procedure.AppendFormat(BuildFileNameXE, SPFileName, config.FileName, SPFileDate);
          procedure.AppendFormat(BuildSessionNameXE, SessionNameXE, config.XESessionName.Replace("-","_"), SPFileDate);
          StringBuilder XEString =new StringBuilder();
          XEString.AppendFormat(IfExistSessionXE, SessionNameXE);

          XEString.AppendFormat(DropCastSessionXE + '\n', SessionNameXE);
          XEString.AppendFormat(CreateSessionXE, SessionNameXE);
          string filterString = FilterDataXE(filters, (int)config.Level, config.Category); //SQLCM-5471 - v5.6
              
          for (int i = 0; i < config.GetTraceEventsXE().Length; i++)
          {
              string actionName = string.Join("\n,", config.GetTraceColumnsXE());              

              XEString.AppendFormat(AddEventXE, config.GetTraceEventsXE()[i], actionName, filterString);
              if (i + 1 < config.GetTraceEventsXE().Length)
              {
                  XEString.Append(",");
              }
          }
          XEString.AppendFormat(AddTargetXE, SPFileName, config.MaxFileSize,SessionNameXE);
          procedure.AppendFormat(SetQueryStringXE, SPQueryXE, XEString.ToString());
          procedure.AppendFormat(ExecQueryXE);
          procedure.AppendFormat(GetSessionIdXE, SessionNameXE);
          procedure.AppendFormat("INSERT {0} VALUES ( {1}, {2}, {3}, {4}, {5}, {6}, {7} )\n",
          SPTraceInfoTableXE, traceNumber, SPSessionIdXE, SPFileName, config.Options.ToString("D"), SPMaxSize, SPStopTime, 1);          
      }

    
      //---------------------------------------------------------------------
      // CreateServerRoleAuditSettings
      //---------------------------------------------------------------------
      /// <summary>
      /// CreateServerRoleAuditSettings: create script to query for user names in these
      /// server roles and add audit settings to audit these users' activities.
      /// </summary>
      /// <param name="serverRoles"></param>
      protected virtual void
         CreateServerRoleAuditSettings (
         int []    serverRoles,
         string [] excludedUsers
         )
      {
         if( serverRoles == null ||
             serverRoles.Length == 0 )
            return;  //nothing to do

         bool saSelected = false;

         // First parameter is the user name table and the second parameter is the
         // select statement that creates the fixed system role ID result set.
         procedure.AppendFormat( "INSERT {0} {1} ", SPUsers, FindServerRoleUsersStatement );
         // Create the Server Roles ID list to be audited
         procedure.AppendFormat( "AND s.number in ( " );
         for( int i = 0; i < serverRoles.Length; i++ )
         {
            if( serverRoles[i] == 1 ) // sa is selected
               saSelected = true;
            procedure.AppendFormat( "{0}", serverRoles[i] );
            if( i < serverRoles.Length - 1 )
               procedure.Append( ", " );
            else
               procedure.Append( ")\n" );
         }
         // Create the excluded user name list if exists
         if( excludedUsers != null &&
            excludedUsers.Length > 0 )
         {
            procedure.AppendFormat( "AND l.name NOT IN ( " );
            for( int i = 0; i < excludedUsers.Length; i++ )
            {
               procedure.AppendFormat( "'{0}'", excludedUsers[i] );
               if( i < excludedUsers.Length - 1 )
                  procedure.Append( ", " );
               else
                  procedure.Append( ")\n" );
            }
         }

         if( saSelected )
            procedure.Append( " OR l.name = 'sa' \n");

         // Set user name filters to exclude all users in the SPUsers table
         procedure.AppendFormat( "SELECT {0} = COUNT( DISTINCT NAME) FROM {1}\n", SPRows, SPUsers );
         procedure.AppendFormat( "SET {0} = {0} + {1}\n", SPNumUsers, SPRows );
         procedure.AppendFormat( DeclareCursor, SPUserCursor, String.Format( "SELECT DISTINCT name FROM {0}\n", SPUsers ) );
         procedure.AppendFormat( OpenCursor, SPUserCursor );
         procedure.AppendFormat( "FETCH NEXT FROM {0} INTO {1}\n", SPUserCursor, SPPUser );
         procedure.AppendFormat( "WHILE {0} > 0\n", SPRows);
         procedure.Append( SqlBegin );
         procedure.AppendFormat( SetFilterStatement, 
                                 SPReturnCode, 
                                 SPTraceId, 
                                 TraceColumnId.SQLSecurityLoginName.ToString("D"),
                                 TraceFilterLogicalOp.OR.ToString("D"),
                                 TraceFilterComparisonOp.Like.ToString("D"),
                                 SPPUser );
         procedure.AppendFormat( IfStatement, SPReturnCode, SqlGreater, 0 );
         procedure.Append( SqlPrint + "':Error Setting the filter'\n" );
         procedure.AppendFormat( "SET {0} = {0} - 1\n", SPRows );
         procedure.AppendFormat( "FETCH NEXT FROM {0} INTO {1}\n", SPUserCursor, SPPUser );
         procedure.Append( SqlEnd );
         procedure.AppendFormat( CloseCursor, SPUserCursor );
         procedure.AppendFormat( DeallocateCursor, SPUserCursor );
      }

      //---------------------------------------------------------------------
      // CreateGetRunningTracesScript
      //---------------------------------------------------------------------
      private void
         CreateGetRunningTracesScript()
      {
         string prefix = instanceAlias;//Path.Combine(traceDirectory, instanceAlias );
         procedure.AppendFormat( "SELECT traceid FROM ::fn_trace_getinfo(0) where property = 2 and convert( nvarchar(245), value ) like '%{0}%'\n", 
                                 prefix );
      }

       //5.4 XE

      private void
        CreateGetRunningTracesScriptXE()
      {
          string prefix = "XE" + instanceAlias;
          prefix = prefix.Replace("-", "_");
          procedure.AppendFormat("SELECT event_session_id as traceid FROM sys.server_event_sessions WHERE name like '%{0}%'\n",
                                  prefix);
      }

      /// <summary>
      /// Create script to stop all SQLsecure traces
      /// </summary>
      private void 
         CreateStopAllTraces( )
      {
         string prefix = Path.Combine(traceDirectory, instanceAlias );
         procedure.AppendFormat( @"DECLARE InfoCursor CURSOR FOR SELECT traceid FROM ::fn_trace_getinfo(0) where property = 2 and convert( nvarchar(245), value ) like '{0}%'", 
                                  prefix );
         procedure.Append( "\nOPEN InfoCursor\n" );
         procedure.AppendFormat( "FETCH NEXT FROM InfoCursor INTO {0}\n", SPTraceId );
         procedure.Append( "WHILE @@FETCH_STATUS = 0\nBEGIN \n");
         procedure.AppendFormat( SetTraceStatusStatement, SPReturnCode, SPTraceId, 0 );
         procedure.AppendFormat( SetTraceStatusStatement, SPReturnCode, SPTraceId, 2 );
         procedure.AppendFormat( "FETCH NEXT FROM InfoCursor INTO {0}\n", SPTraceId );
         procedure.Append( "END\nCLOSE InfoCursor\nDEALLOCATE InfoCursor\n" );
      }

      private void
        CreateStopAllTracesXE()
      {
          string prefix = "XE" + instanceAlias;
          prefix = prefix.Replace("-", "_");
          procedure.AppendFormat(@"DECLARE InfoCursor CURSOR FOR SELECT name FROM sys.server_event_sessions where name like '{0}%'",
                                  prefix);
          procedure.Append("\nOPEN InfoCursor\n");
          procedure.AppendFormat("FETCH NEXT FROM InfoCursor INTO {0}\n", SessionNameXE);
          procedure.Append("WHILE @@FETCH_STATUS = 0\nBEGIN \n"); 
          procedure.AppendFormat("\nEXEC ('" + DropSessionXE + "')\n",  SessionNameXE );
          procedure.AppendFormat("FETCH NEXT FROM InfoCursor INTO {0}\n", SessionNameXE);
          procedure.Append("END\nCLOSE InfoCursor\nDEALLOCATE InfoCursor\n");
      }

      public string FilterDataXE(TraceFilter[] filters, int level, TraceCategory traceCategory) //SQLCM-5471 - v5.6
      {

          string ApplicationNameFilter      = "";
          string SecurityLoginNameFilter    = "";
          string PermissionFilter           = "";
          string whereCondition             = "";
          string objectNameFilterSC         = "";
          string selectFilter               = "";
          string dbIdFilter                 = "";
          string dataChangeEventFilter      = "";
          if (filters.Length > 0)
          {
              for (int j = 0; j < filters.Length; j++)
              {
                  string comparisonOp = "";
                  switch (filters[j].CompareOp)
                  {
                      case TraceFilterComparisonOp.Equal:
                          comparisonOp = TraceFilterComparisonOpXE.Equal;
                          break;
                      case TraceFilterComparisonOp.GreaterThan:
                          comparisonOp = TraceFilterComparisonOpXE.GreaterThan;
                          break;
                      case TraceFilterComparisonOp.GreaterThanOrEqual:
                          comparisonOp = TraceFilterComparisonOpXE.GreaterThanOrEqual;
                          break;
                      case TraceFilterComparisonOp.LessThan:
                          comparisonOp = TraceFilterComparisonOpXE.LessThan;
                          break;
                      case TraceFilterComparisonOp.LessThanOrEqual:
                          comparisonOp = TraceFilterComparisonOpXE.LessThanOrEqual;
                          break;
                      case TraceFilterComparisonOp.Like:
                          comparisonOp = TraceFilterComparisonOpXE.Like;
                          break;
                      case TraceFilterComparisonOp.NotEqual:
                          comparisonOp = TraceFilterComparisonOpXE.NotEqual;
                          break;
                      case TraceFilterComparisonOp.NotLike:
                          comparisonOp = TraceFilterComparisonOpXE.NotLike;
                          break;
                  }
                  if (filters[j].ColumnId == TraceColumnId.ApplicationName)
                  {
                      if (!ApplicationNameFilter.Equals(""))
                          ApplicationNameFilter += ((filters[j].LogicalOp == 0) ? "\nAND " : "\nOR ") + TraceColumnIdXE.ApplicationName + comparisonOp + "N''" + filters[j].GetTextValue() + "''";
                      else
                          ApplicationNameFilter += TraceColumnIdXE.ApplicationName + comparisonOp + "N''" + filters[j].GetTextValue() + "''";
                  }
                  if (filters[j].ColumnId == TraceColumnId.SQLSecurityLoginName)
                  {
                      if (!SecurityLoginNameFilter.Equals(""))
                          SecurityLoginNameFilter += ((filters[j].LogicalOp == 0) ? "\nAND " : "\nOR ") + TraceColumnIdXE.SQLSecurityLoginName + comparisonOp + "N''" + filters[j].GetTextValue() + "''";
                      else
                          SecurityLoginNameFilter += TraceColumnIdXE.SQLSecurityLoginName + comparisonOp + "N''" + filters[j].GetTextValue() + "''";
                  }
                  if (filters[j].ColumnId == TraceColumnId.DatabaseID)
                  {
                      if (!dbIdFilter.Equals(""))
                          dbIdFilter += ((filters[j].LogicalOp == 0) ? "\nAND " : "\nOR ") + TraceColumnIdXE.DatabaseID + comparisonOp + filters[j].GetIntValue();
                      else
                          dbIdFilter += TraceColumnIdXE.DatabaseID + comparisonOp + filters[j].GetIntValue();
                  }

                  if (filters[j].ColumnId == TraceColumnId.ParentName)
                  {
                      if (!comparisonOp.Equals(TraceFilterComparisonOpXE.Like))
                      {
                          string tempString = "%SQLcompliance_Data_Change%.%SQLcompliance_Changed_Data_Table%";
                          dataChangeEventFilter = TraceColumnIdXE.Statement + comparisonOp + "N''" + tempString + "''";
                      }
                      else
                      {
                          string tempString = "%[SQLcompliance_Data_Change].[SQLcompliance_Changed_Data_Table]%WHERE%0";
                          dataChangeEventFilter = TraceColumnIdXE.Statement + comparisonOp + "N''" + tempString + "''";
                      }
                  }
                  if (level != (int)TraceLevel.Table)
                  {
                      if (filters[j].ColumnId == TraceColumnId.Permissions)
                      {
                          string EventTypeFilter = "";
                          if (comparisonOp.Equals(TraceFilterComparisonOpXE.Equal))
                              comparisonOp = TraceFilterComparisonOpXE.Like;
                          else comparisonOp = TraceFilterComparisonOpXE.NotLike;

                          if (filters[j].GetIntValue() == 2)
                              EventTypeFilter = "UPDATE%";
                          else if (filters[j].GetIntValue() == 8)
                              EventTypeFilter = "INSERT%";
                          else if (filters[j].GetIntValue() == 16)
                              EventTypeFilter = "DELETE%";
                          else if (filters[j].GetIntValue() == 32)
                              EventTypeFilter = "EXEC%";
                          else if (filters[j].GetIntValue() == 1)
                              EventTypeFilter = "SELECT%";
                          if (!EventTypeFilter.Equals(""))
                          {
                              if (!PermissionFilter.Equals(""))
                                  PermissionFilter += ((filters[j].LogicalOp == 0) ? "\nAND " : "\nOR ") + TraceColumnIdXE.Statement + comparisonOp + "N''" + EventTypeFilter + "''";
                              else
                                  PermissionFilter += TraceColumnIdXE.Statement + comparisonOp + "N''" + EventTypeFilter + "''";
                          }
                      }
                  }
                  //SQLCM-5471 v5.6
                  if (level == (int)TraceLevel.Table &&
                      (traceCategory == TraceCategory.SensitiveColumnwithSelect || traceCategory == TraceCategory.SensitiveColumn))
                  {
                      if (filters[j].ColumnId == TraceColumnId.Permissions)
                      {
                          string EventTypeFilter = "";
                          if (comparisonOp.Equals(TraceFilterComparisonOpXE.Equal))
                              comparisonOp = TraceFilterComparisonOpXE.Like;
                          else comparisonOp = TraceFilterComparisonOpXE.NotLike;
                          if (filters[j].GetIntValue() == 2)
                              EventTypeFilter = "UPDATE%";
                          else if (filters[j].GetIntValue() == 8)
                              EventTypeFilter = "INSERT%";
                          else if (filters[j].GetIntValue() == 16)
                              EventTypeFilter = "DELETE%";
                          else if (filters[j].GetIntValue() == 32)
                              EventTypeFilter = "EXEC%";
                          else if (filters[j].GetIntValue() == 1)
                              EventTypeFilter = "SELECT%";
                          else if (filters[j].GetIntValue() == 217)
                              EventTypeFilter = "ALTER%";
                          if (!EventTypeFilter.Equals(""))
                          {
                              
                              if (!PermissionFilter.Equals(""))
                              {
                                  //SQLCM-5674 v5.6
                                  if (EventTypeFilter.Contains("DELETE"))
                                  {
                                      EventTypeFilter = "DELETE%";
                                      PermissionFilter += ((filters[j].LogicalOp == 0) ? "\nAND " : "\nOR ") + TraceColumnIdXE.Statement + comparisonOp + "N''" + EventTypeFilter + "''";
                                      EventTypeFilter = "TRUNCATE%";
                                      PermissionFilter += ((filters[j].LogicalOp == 0) ? "\nAND " : "\nOR ") + TraceColumnIdXE.Statement + comparisonOp + "N''" + EventTypeFilter + "''";
                                  }
                                  else
                                  {
                                      PermissionFilter += ((filters[j].LogicalOp == 0) ? "\nAND " : "\nOR ") + TraceColumnIdXE.Statement + comparisonOp + "N''" + EventTypeFilter + "''";
                                  }
                              }
                              else
                              {
                                  //SQLCM-5674 v5.6
                                  if (EventTypeFilter.Contains("DELETE"))
                                  {
                                      EventTypeFilter = "DELETE%";
                                      PermissionFilter += TraceColumnIdXE.Statement + comparisonOp + "N''" + EventTypeFilter + "''";
                                      EventTypeFilter = "TRUNCATE%";
                                      PermissionFilter += TraceColumnIdXE.Statement + comparisonOp + "N''" + EventTypeFilter + "''";
                                  }
                                  else
                                  {
                                      PermissionFilter += TraceColumnIdXE.Statement + comparisonOp + "N''" + EventTypeFilter + "''";
                                  }
                              }
                              
                          }
                      }
                  }
                  //SQLCM-5471 v5.6 - END
                  else
                  {                      
                      if (filters[j].ColumnId == TraceColumnId.Permissions &&  (filters[j].GetIntValue() == 1))
                      {
                          selectFilter = "SELECT%FROM%";
                      }
                      if (filters[j].ColumnId == TraceColumnId.ObjectName 
                          && !filters[j].GetTextValue().Equals(CoreConstants.Agent_BeforeAfter_TableName))
                      {
                          string tempString = "SELECT%FROM%{0}%";
                          if (!objectNameFilterSC.Equals(""))
                              objectNameFilterSC += ((filters[j].LogicalOp == 0) ? "\nAND " : "\nOR ") + TraceColumnIdXE.Statement + comparisonOp + "N''" + String.Format(tempString, filters[j].GetTextValue()) + "''";
                          else
                              objectNameFilterSC += TraceColumnIdXE.Statement + comparisonOp + "N''" + String.Format(tempString, filters[j].GetTextValue()) + "''";
                      }
                  }
              }
          }
              
            if (!SecurityLoginNameFilter.Equals(""))
            {
                whereCondition += "( " + SecurityLoginNameFilter + ")\n";
            }
            if (!ApplicationNameFilter.Equals(""))
            {
                if (whereCondition.Equals(""))
                    whereCondition += "( " + ApplicationNameFilter + ")\n";
                else
                    whereCondition += "AND ( " + ApplicationNameFilter + ")\n";
            }

            if(!dbIdFilter.Equals(""))
            {
                if (whereCondition.Equals(""))
                    whereCondition += "( " + dbIdFilter + ")\n";
                else
                    whereCondition += "AND ( " + dbIdFilter + ")\n";
            }
            if (!PermissionFilter.Equals(""))
            {
                if (whereCondition.Equals(""))
                    whereCondition += "( " + PermissionFilter + ")\n";
                else
                    whereCondition += "AND ( " + PermissionFilter + ")\n";
            }

            if (!objectNameFilterSC.Equals("") && !selectFilter.Equals(""))
            {
                if (whereCondition.Equals(""))
                    whereCondition += "( " + objectNameFilterSC + ")\n";
                else
                    whereCondition += "AND ( " + objectNameFilterSC + ")\n";
            }

            if (!dataChangeEventFilter.Equals(""))
            {
                if (whereCondition.Equals(""))
                    whereCondition += "( " + dataChangeEventFilter + ")\n";
                else
                    whereCondition += "AND ( " + dataChangeEventFilter + ")\n";
            }
          

            return (whereCondition.Equals("") ? whereCondition : " ' + 'WHERE " + whereCondition);
      }

      #endregion

      #region Audit Logs
      // Array of Audit Log configurations
      public AuditLogConfiguration[] AuditLogConfiguration
      {
          get
          {
              return (AuditLogConfiguration[])auditLogConfigurations.ToArray(typeof(AuditLogConfiguration));
          }

          set
          {
              if (value == null)
                  throw (new ArgumentNullException("AuditLogConfiguration"));
              auditLogConfigurations.Clear();
              for (int i = 0; i < value.Length; i++)
                  auditLogConfigurations .Add(value[i]);

          }
      }

      /// <summary>
      /// CreateAuditLogSP
      /// </summary>
      /// <returns></returns>
      public string
       CreateAuditLogSP()
      {
          // Clear the string builder
          procedure.Remove(0, procedure.Length);

          AddCopyright();
          //
          // Create the header part
          //
          procedure.AppendFormat(CreateProc, CoreConstants.Agent_AuditLogSPName);
          procedure.Append(SQLSECURE_AuditSPParam1 + SQLSECURE_AuditSPParam2);
          procedure.Append(SqlAs + SqlBegin + SetNoCountOn);

          //
          // Declare variables and lists
          //
          procedure.AppendFormat(DeclareIntVariable, SPReturnCode);
          procedure.AppendFormat(DeclareIntVariable, SPTraceNumber);
          procedure.AppendFormat(DeclareIntVariable, SPTraceId);
          procedure.AppendFormat(DeclareBigIntVariable, SPPermissions);
          // Declare variables for each property
          procedure.AppendFormat(DeclareIntVariable, SPTraceOptions);
          procedure.AppendFormat(DeclareNvarcharVariable, SPFileName, 245);
          procedure.AppendFormat(DeclareDateTimeVariable, SPStopTime);
          procedure.AppendFormat(DeclareIntVariable, SPStatus);
          procedure.AppendFormat(DeclareBigIntVariable, SPMaxSize);
          procedure.AppendFormat(DeclareIntVariable, SPRows);
          // Declare a table variable to hold the trace info records
          procedure.AppendFormat(DeclareTraceInfoTable, SPTraceInfoTable);
          procedure.AppendFormat(DeclareNvarcharVariable, SessionNameXE, 100);
          procedure.AppendFormat(DeclareIntVariable, SPNumUsers);

          procedure.AppendFormat(DeclareIntVariable, "@i");
          procedure.AppendFormat(DeclareIntVariable, "@j");
          procedure.AppendFormat(DeclareIntVariable, "@length");

          procedure.AppendFormat(IfStatement, SPCommand, "=", 0); // No-op command
          procedure.Append(SqlPrint + "'No op'\n");
          // Generate the start trace script
          CreateStartCommandScriptAuditLogs();

          // Generate the stop trace command script
          CreateStopCommandScriptAuditLogs();

          // Get trace status command script
          CreateGetStatusCommandScriptAuditLogs();

          // Get stored procedure information script
          CreateGetSPInfoCommandScriptAuditLogs();

          //
          // Get running traces info
          //
          procedure.AppendFormat(BlockElseIfStatement, SPCommand, "=", SPCmd.GetRunningTraces.ToString("D"));
          CreateGetRunningAuditLogsScript();
          procedure.Append(SqlEnd);

          //
          // Stop all traces
          //
          procedure.AppendFormat(BlockElseIfStatement, SPCommand, "=", SPCmd.StopAll.ToString("D"));
          CreateStopAllAuditLogs();
          procedure.Append(SqlEnd);
          //
          // Unknown command
          //
          procedure.Append(SqlElse);
          procedure.Append(SqlPrint + "'Unknown Command'\n");

          //
          //
          procedure.Append(ReturnSuccess);
          procedure.Append(SqlEnd);

          return procedure.ToString();
      }

      //5.5 Audit Logs
      protected virtual void
        CreateStartCommandScriptAuditLogs()
      {
          //
          // Start tracing statements
          //
          procedure.AppendFormat(BlockElseIfStatement, SPCommand, "=", SPCmd.Start.ToString("D"));

          // Declare variables used in this command
          procedure.AppendFormat(DeclareVarcharVariable, SPFileDate, 32);
          // Create file name suffix for the traces
          procedure.AppendFormat(BuildFileDate, SPFileDate);
          // The two variables below are for privileged user traces
          procedure.AppendFormat(DeclareUserNameTable, SPUsers);
          procedure.AppendFormat(DeclareNvarcharVariable, SPPUser, 128);
          procedure.AppendFormat(DeclareNvarcharVariable, SPQueryXE, Max);
          procedure.AppendFormat(DeclareNvarcharVariable, SPSessionIdXE, 150);


          // No option specified, start all traces
          procedure.AppendFormat(IfStatement, SPOption, SqlIs, SqlNull);
          procedure.AppendFormat(AssignValue, SPTraceNumber, 0);

          // Convert the option to trace number
          procedure.Append(SqlElse + "\n");
          procedure.AppendFormat("SET {0}=CAST({1} AS int)\n", SPTraceNumber, SPOption);

          // Validate trace number 
          procedure.AppendFormat(BlockIfStatement, SPTraceNumber, SqlGreater, auditLogConfigurations.Count);
          //Invalid trace number, set return code and go to the end of the start trace block
          procedure.AppendFormat(AssignValue, SPReturnCode, -1);
          procedure.AppendFormat(GotoStatement, SPEndStartTrace);
          procedure.Append(SqlEnd);


          // Create start trace statements for each trace configuration
          for (int i = 0; i < auditLogConfigurations.Count; i++)
          {

              procedure.AppendFormat(AssignValue, SPMaxSize, maxTraceSize);
              // Starting all audit logs or this individual trace?
              procedure.AppendFormat(BlockIfStatement,
                 String.Format(BinaryExpression, SPTraceNumber, SqlEqual, 0),
                 SqlOr,
                 String.Format(BinaryExpression, SPTraceNumber, SqlEqual, i + 1));
              CreateAuditLogStatements((AuditLogConfiguration)auditLogConfigurations[i], i + 1);

              // If the call is to start individual trace, go to the end of
              // start trace block
              procedure.AppendFormat(IfStatement, SPTraceNumber, SqlNotEqual, 0);
              procedure.AppendFormat(GotoStatement, SPEndStartTrace);
              procedure.Append(SqlEnd);

          }
          // Create the End of Start Trace label
          procedure.AppendFormat("{0}:\n", SPEndStartTrace);
          // Create result set
          procedure.AppendFormat("select * from {0}\n", SPTraceInfoTable);
          procedure.Append(SqlEnd);
      }
      /// <summary>
      /// CreateStopCommandScriptAuditLogs
      /// </summary>
      protected virtual void
        CreateStopCommandScriptAuditLogs()
      {
          //
          // Stop tracing statements
          //
          procedure.AppendFormat(BlockElseIfStatement, SPCommand, "=", SPCmd.Stop.ToString("D"));

          // check for null option
          procedure.AppendFormat(IfStatement, SPOption, SqlIs, SqlNull);
          procedure.Append(ReturnSuccess); // TODO: returns an error and error message

          // Trace IDs are passed in from the option as a list
          // Append a comma to the trace ID list
          procedure.AppendFormat("SET @length = LEN({0})\n", SPOption);
          procedure.AppendFormat("IF RIGHT({0},1)<>',' SET {0}={0}+','\n", SPOption );


          // Parse the trace ID list in a while loop and stop each trace
          procedure.AppendFormat("SET @j=CHARINDEX(',',{0})\n", SPOption);
          procedure.Append("WHILE @j<>0 BEGIN\n");
          // Get one trace ID
          procedure.AppendFormat("SET {0}=CAST(LEFT({1},@j-1) AS int)\n", SPTraceId, SPOption);
          // Stop and Drop session
          procedure.Append(GetAuditLogSessionName);
          procedure.Append(IfSessionIsNotEmpty);
          procedure.Append(SqlBegin);
          procedure.AppendFormat(DropServerAuditSpecification, SessionNameXE);
          procedure.AppendFormat(DropServerAudit, SessionNameXE);
          procedure.Append(SqlEnd);
          // Remove this trace ID
          procedure.AppendFormat("SET {0}=SUBSTRING({1},@j+1,@length)\n", SPOption, SPOption);
          procedure.AppendFormat("SET @j=CHARINDEX(',',{0})\n", SPOption);
          procedure.Append(SqlEnd);

          // End of the outter IF statement
          procedure.Append(SqlEnd);
      }
      
      /// <summary>
      /// CreateGetStatusCommandScriptAuditLogs
      /// </summary>
      protected virtual void
       CreateGetStatusCommandScriptAuditLogs()
      {
          //
          // Get status statements
          //
          procedure.AppendFormat(BlockElseIfStatement, SPCommand, "=", SPCmd.GetStatus.ToString("D"));
          // Create get status statements
          procedure.Append(SqlPrint + "'get status'\n");
          // check for null option
          procedure.AppendFormat(IfStatement, SPOption, SqlIs, SqlNull);
          procedure.Append(ReturnSuccess); // TODO: returns an error and error message

          // Trace IDs are passed in from the option as a list
          // Append a comma to the trace ID list
          procedure.AppendFormat("SET @length = LEN({0})\n", SPOption);
          procedure.AppendFormat("IF RIGHT({0},1)<>',' SET {0}={0}+','\n", SPOption/*, SPOption, SPOption*/ );


          // Parse the trace ID list in a while loop and get trace info for each trace
          procedure.AppendFormat("SET @j=CHARINDEX(',',{0})\n", SPOption);
          procedure.Append("WHILE @j<>0 BEGIN\n");
          // Get one trace ID
          procedure.AppendFormat("SET {0}=CAST(LEFT({1},@j-1) AS int)\n", SPTraceId, SPOption);
          procedure.AppendFormat(GetAuditLogInfo, SPStatus, SPFileName, SPTraceId);
          // Insert a trace info record to the result set table
          procedure.AppendFormat(BlockIfStatement, SPFileName, SqlIsNot, SqlNull);
          procedure.AppendFormat("INSERT {0} VALUES ( 0, {1}, {2}, {3}, {4}, {5}, {6} )\n",
             SPTraceInfoTable, SPTraceId, SPFileName, 2, 5, SqlNull, SPStatus);

          procedure.AppendFormat(AssignValue, SPFileName, SqlNull);
          procedure.Append(SqlEnd);


          // Get property info for the trace
          // Remove this trace ID from the list
          procedure.AppendFormat("SET {0}=SUBSTRING({1},@j+1,@length)\n", SPOption, SPOption);
          procedure.AppendFormat("SET @j=CHARINDEX(',',{0})\n", SPOption);
          procedure.Append(SqlEnd);  // End of the WHILE parsing loop

          // Create the result set
          procedure.AppendFormat("select * from {0}\n", SPTraceInfoTable);
          procedure.Append(SqlEnd);

      }

      /// <summary>
      /// CreateGetSPInfoCommandScriptAuditLogs
      /// </summary>
      protected virtual void
        CreateGetSPInfoCommandScriptAuditLogs()
      {
          //
          // Get trace information statements
          //
          procedure.AppendFormat(BlockElseIfStatement, SPCommand, "=", SPCmd.GetInfo.ToString("D"));
          // Create SP Info table variable
          procedure.Append("DECLARE @SPInfo table ( [CreateTime] [nvarchar] (40), [Agent Verion] [nvarchar] (40), ");
          procedure.Append("                       [configVersion] [int], [NumTraces] [int] )\n");
          procedure.AppendFormat("INSERT @SPInfo VALUES ( '{0}', '{1}', {2}, {3} )",
             lastModifiedTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), agentVersion, configVersion, traceConfigurationsXE.Count);
          procedure.Append("SELECT * FROM @SPInfo\n");
          procedure.Append(SqlEnd);

      }

      /// <summary>
      /// CreateGetRunningAuditLogsScript
      /// </summary>
      private void
       CreateGetRunningAuditLogsScript()
      {
          string instanceAliasWithPrefix = "AL" + instanceAlias;
          //Get running audit logs list
          procedure.AppendFormat("SELECT audit_id as traceid FROM sys.server_audits WHERE [name] like '{0}%'\n",
                                  instanceAliasWithPrefix);
      }

      /// <summary>
      /// CreateStopAllAuditLogs
      /// </summary>
      private void
       CreateStopAllAuditLogs()
      {
          string instanceAliasWithPrefix = "AL" + instanceAlias;
          procedure.AppendFormat(@"DECLARE InfoCursor CURSOR FOR SELECT name FROM sys.server_audits WHERE name like '{0}%'",
                                  instanceAliasWithPrefix);
          procedure.Append("\nOPEN InfoCursor\n");
          procedure.AppendFormat("FETCH NEXT FROM InfoCursor INTO {0}\n", SessionNameXE);
          procedure.Append("WHILE @@FETCH_STATUS = 0\nBEGIN \n");
          procedure.AppendFormat(DropServerAudit, SessionNameXE);
          procedure.AppendFormat(DropServerAuditSpecification, SessionNameXE);
          procedure.AppendFormat("FETCH NEXT FROM InfoCursor INTO {0}\n", SessionNameXE);
          procedure.Append("END\nCLOSE InfoCursor\nDEALLOCATE InfoCursor\n");
      }

      protected virtual void
       CreateAuditLogStatements(
          AuditLogConfiguration config,
          int traceNumber
       )
      {
          if (config == null)
              throw new ArgumentNullException("config");

          TraceFilter[] filters = config.GetTraceFilters();

          //procedure.AppendFormat(BuildFileNameXE, SPFileName, config.FileName, SPFileDate);
          procedure.AppendFormat(BuildSessionNameXE, SessionNameXE, config.SessionName, SPFileDate);
          StringBuilder eventsString = new StringBuilder();
          string filterString = FilterAuditLogData(filters, (int)config.Category);

          for (int i = 0; i < config.GetAuditLogEvents().Length; i++)
          {
              eventsString.AppendFormat(AddAuditLogEvent, config.GetAuditLogEvents()[i]);
              if (i + 1 < config.GetAuditLogEvents().Length)
              {
                  eventsString.Append(",");
              }
          }
          procedure.AppendFormat(DropServerAuditSpecification, SessionNameXE);
          procedure.AppendFormat(DropServerAudit, SessionNameXE);
          procedure.AppendFormat(SetQueryStringXE, SPQueryXE, string.Format(CreateAuditSpecification, SessionNameXE, config.TraceDirectory, config.MaxFileSize, filterString, eventsString.ToString()));
          //procedure.AppendLine(AuditLogString.ToString());
          procedure.AppendFormat(ExecQueryXE);
          procedure.AppendFormat(GetAuditLogDetails, SessionNameXE);
          procedure.AppendFormat("INSERT {0} VALUES ( {1}, {2}, {3}, {4}, {5}, {6}, {7} )\n",
          SPTraceInfoTable, traceNumber, SPSessionIdXE, SPFileName, config.Options.ToString("D"), SPMaxSize, SPStopTime, 1);
      }
      /// <summary>
      /// FilterAuditLogData
      /// </summary>
      /// <param name="filters"></param>
      /// <param name="category"></param>FilterAuditLogData
      /// <returns></returns>
      public string FilterAuditLogData(TraceFilter[] filters, int category)
      {
          string applicationNameFilter = string.Empty;
          string securityLoginNameFilter = string.Empty;
          string permissionFilter = string.Empty;
          string dataChangeObjectFilter = string.Empty;
          string dataChangeParentFilter = string.Empty;
          string whereCondition = string.Empty;
          string objectNameFilter = string.Empty;
          string objectIdFilter = string.Empty;
          string dbNameFilter = string.Empty;
          string parentNameFilter = string.Empty;
          string objectTypeFilter = string.Empty;
          if (filters.Length > 0)
          {
              foreach (TraceFilter filter in filters)
              {
                  if (filter.AuditLogColumnId == AuditLogColumnId.ApplicationName)
                  {
                      if (!applicationNameFilter.Equals(string.Empty))
                          applicationNameFilter += filter.AuditLogLogicOperator;
                      applicationNameFilter += string.Format(ConditionTextForNvarchar,
                                                            AuditLogColumnId.ApplicationName, 
                                                            filter.AuditLogCompareOperator,
                                                            filter.GetTextValue());
                      continue;
                  }
                  if (filter.AuditLogColumnId == AuditLogColumnId.SQLSecurityLoginName)
                  {
                      if (!securityLoginNameFilter.Equals(string.Empty))
                          securityLoginNameFilter += filter.AuditLogLogicOperator;
                      securityLoginNameFilter += string.Format(ConditionTextForNvarchar,
                                                                AuditLogColumnId.SQLSecurityLoginName,
                                                                filter.AuditLogCompareOperator, 
                                                                filter.GetTextValue());
                      continue;
                  }
                  if (filter.AuditLogColumnId == AuditLogColumnId.DatabaseName)
                  {
                      if (!dbNameFilter.Equals(string.Empty))
                          dbNameFilter += filter.AuditLogLogicOperator;
                      dbNameFilter += string.Format(ConditionTextForNvarchar, 
                                                    AuditLogColumnId.DatabaseName, 
                                                    filter.AuditLogCompareOperator, 
                                                    filter.GetTextValue()); 
                      continue;
                  }
                  if ((category != (int)TraceCategory.DMLwithDetails
                         || category != (int)TraceCategory.DML)
                         && filter.AuditLogCompareOperator == TraceFilterComparisonOpXE.Equal
                         && filter.AuditLogColumnId == AuditLogColumnId.ParentName
                         && filter.GetTextValue() == CoreConstants.Agent_BeforeAfter_SchemaName)
                  {                      
                      dataChangeParentFilter = string.Format(ConditionTextForNvarchar, 
                                                                    AuditLogColumnId.ParentName, 
                                                                    filter.AuditLogCompareOperator, 
                                                                    filter.GetTextValue());
                      continue;
                  }

                  if ((category != (int)TraceCategory.DMLwithDetails
                        || category != (int)TraceCategory.DML)
                        && filter.AuditLogCompareOperator == TraceFilterComparisonOpXE.Equal
                        && filter.AuditLogColumnId == AuditLogColumnId.ObjectName
                        && filter.GetTextValue() == CoreConstants.Agent_BeforeAfter_TableName)
                  {
                      dataChangeObjectFilter = string.Format(ConditionTextForNvarchar, 
                                                                    AuditLogColumnId.ObjectName, 
                                                                    filter.AuditLogCompareOperator, 
                                                                    filter.GetTextValue());
                      continue;
                  }

                  if ((category == (int)TraceCategory.DataChange
                        || category == (int)TraceCategory.DataChangeWithDetails)
                        && filter.AuditLogColumnId == AuditLogColumnId.ActionID
                        && filter.GetTextValue() == AuditLogActionID.Select)
                  {
                      //dataChangePermissionFilter += string.Format(ConditionTextForInt, 
                      //                                            AuditLogColumnId.ActionID, 
                      //                                            filter.AuditLogCompareOperator,
                      //                                            filter.GetIntValueOfActionId());
                      continue;
                  }

                  if (filter.AuditLogColumnId == AuditLogColumnId.ActionID)
                  {
                      if (!permissionFilter.Equals(string.Empty))
                          permissionFilter += filter.AuditLogLogicOperator;
                      permissionFilter += string.Format(ConditionTextForInt,
                                                        AuditLogColumnId.ActionID,
                                                        filter.AuditLogCompareOperator,
                                                        filter.GetIntValueOfActionId());
                      continue;
                  }

                  if (filter.AuditLogColumnId == AuditLogColumnId.ObjectName)
                  {
                      if (!objectNameFilter.Equals(string.Empty))
                          objectNameFilter += filter.AuditLogLogicOperator;
                      objectNameFilter += string.Format(ConditionTextForNvarchar, 
                                                        AuditLogColumnId.ObjectName, 
                                                        filter.AuditLogCompareOperator, 
                                                        filter.GetTextValue()); 
                      continue;
                  }

                  if (filter.AuditLogColumnId == AuditLogColumnId.ParentName)
                  {
                      if (!parentNameFilter.Equals(string.Empty))
                          parentNameFilter += filter.AuditLogLogicOperator;
                      parentNameFilter += string.Format(ConditionTextForNvarchar, 
                                                        AuditLogColumnId.ParentName, 
                                                        filter.AuditLogCompareOperator, 
                                                        filter.GetTextValue()); 
                      continue;
                  }

                  if (filter.AuditLogColumnId == AuditLogColumnId.ObjectID)
                  {
                      if (!objectIdFilter.Equals(string.Empty))
                          objectIdFilter += filter.AuditLogLogicOperator;
                      objectIdFilter += string.Format(ConditionTextForInt, 
                                                      AuditLogColumnId.ObjectID, 
                                                      filter.AuditLogCompareOperator, 
                                                      filter.GetIntValue()); 
                      continue;
                  }

                  if (filter.AuditLogColumnId == AuditLogColumnId.ObjectType)
                  {
                      if (!objectTypeFilter.Equals(string.Empty))
                          objectTypeFilter += filter.AuditLogLogicOperator;
                      objectTypeFilter += string.Format(ConditionTextForInt,
                                                        AuditLogColumnId.ObjectType,
                                                        filter.AuditLogCompareOperator,
                                                        filter.GetIntValue());
                      continue;
                  }
              }
          }

          if (!securityLoginNameFilter.Equals(string.Empty))
          {
              whereCondition += string.Format(GroupFilterText,securityLoginNameFilter);
          }
          if (!applicationNameFilter.Equals(string.Empty))
          {
              if (!whereCondition.Equals(string.Empty))
                  whereCondition += SqlAnd;
              whereCondition += string.Format(GroupFilterText,applicationNameFilter);
          }

          if (!dbNameFilter.Equals(string.Empty))
          {
              if (!whereCondition.Equals(string.Empty))
                  whereCondition += SqlAnd;
              whereCondition += string.Format(GroupFilterText,dbNameFilter);
          }

          if (!objectTypeFilter.Equals(string.Empty))
          {
              if (!whereCondition.Equals(string.Empty))
                  whereCondition += SqlAnd;
              whereCondition += string.Format(GroupFilterText, objectTypeFilter);
          }

          if (!objectNameFilter.Equals(string.Empty))
          {
              if (!dataChangeObjectFilter.Equals(string.Empty))
                  objectNameFilter += AuditLogFilterLogicalOp.OR + dataChangeObjectFilter;
              if (!whereCondition.Equals(string.Empty))
                  whereCondition += SqlAnd;
              whereCondition += string.Format(GroupFilterText, objectNameFilter);
          }

          if (!parentNameFilter.Equals(string.Empty))
          {
              if (!dataChangeParentFilter.Equals(string.Empty))
                  parentNameFilter += AuditLogFilterLogicalOp.OR + dataChangeParentFilter;
              if (!whereCondition.Equals(string.Empty))
                  whereCondition += SqlAnd;
              whereCondition += string.Format(GroupFilterText, parentNameFilter);
          }

          if (!objectIdFilter.Equals(string.Empty))
          {
              if (!whereCondition.Equals(string.Empty))
                  whereCondition += SqlAnd;
              whereCondition += string.Format(GroupFilterText, objectIdFilter);
          }

          if (!dataChangeObjectFilter.Equals(string.Empty) && !dataChangeParentFilter.Equals(string.Empty))
          {
              if (!permissionFilter.Equals(string.Empty))
              {
                  dataChangeObjectFilter = string.Format(GroupFilterText, string.Format(GroupFilters, permissionFilter +
                                                AuditLogFilterLogicalOp.OR + dataChangeObjectFilter + AuditLogFilterLogicalOp.AND + dataChangeParentFilter));
              }
              if (!whereCondition.Equals(string.Empty))
                  whereCondition += SqlAnd;
              whereCondition += string.Format(GroupFilterText, dataChangeObjectFilter);

          }
          else
          {
              if (!permissionFilter.Equals(string.Empty))
              {
                  if (!whereCondition.Equals(string.Empty))
                      whereCondition += SqlAnd;
                  whereCondition += string.Format(GroupFilterText,permissionFilter);
              }              
          }
          return (whereCondition.Equals(string.Empty) ? "1=1" : whereCondition);
      }

      #endregion
   }
}
