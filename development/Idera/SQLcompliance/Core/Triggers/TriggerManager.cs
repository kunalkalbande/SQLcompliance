using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Event;

namespace Idera.SQLcompliance.Core.Triggers
{
   public enum DataChangeComponent
   {
      SharedAssembly = 1,
      SpecialSchema = 2,
      SpecialTable = 3,
      Assembly = 4,
      Trigger = 5,
      CLR = 6
   };
   
   public class TriggerManager
   {   
   #region fields
   
   private SQLInstance _sqlInstance;
   private string _instance;
   private Dictionary<string, Dictionary<string, TableConfiguration>>_dbLists 
      = new Dictionary<string, Dictionary<string, TableConfiguration>>( );
   private bool _supportTrigger;
   
   private readonly object _syncObj = new object();
   private bool _init = true;
   private bool _hasTriggerErrors = false;
   private bool _hasAssemblyErrors = false;
   
   internal bool DataChangeEnabled = false;

   private Dictionary<string, Dictionary<string, string>> _errorTriggers 
      = new Dictionary<string, Dictionary<string, string>>();
   private Dictionary<string, Dictionary<string, string>> _errorAssemblies 
      = new Dictionary<string, Dictionary<string, string>>();
   private int _newErrors = 0;
   #endregion
   
   #region Constructor
   
   public TriggerManager( SQLInstance si, ServerAuditConfiguration config )
   {
      _sqlInstance = si;
      _instance = config.Instance;
      _supportTrigger = config.sqlVersion >= 9;
         
      if( config.AuditDBList == null 
          || config.AuditDBList.Length == 0
          || !_supportTrigger)
      {
         DataChangeEnabled = false;
         return;
      }

      try
      {
         UpdateConfiguration( config );
      }
      catch( Exception e )
      {
         ErrorLog.Instance.Write( ErrorLog.Level.Default,
                                  "An error occurred initializing trigger manager for " +
                                  _instance + ".",
                                  e,
                                  ErrorLog.Severity.Warning,
                                  true );
      }
   }

   #endregion
   
   #region Public Methods

   //----------------------------------------------------------------------
   //
   //
   //----------------------------------------------------------------------
   /*
   public void CreateDMLTriggers(string database,
                                  TableConfiguration[] tables,
                                  DMLTriggerInfo [] existingTriggers )
   {
      List <TableConfiguration> missingOnes = new List<TableConfiguration>( );
      Dictionary<string, DMLTriggerInfo> trgList = new Dictionary<string, DMLTriggerInfo>();
      if( existingTriggers != null )
      {
         foreach ( DMLTriggerInfo info in existingTriggers )
         {
            string key = info.FullTableName;
            if ( !trgList.ContainsKey( key ) )
               trgList.Add( key, info );
         }
      }
      
      if (tables != null)
      {
         foreach (TableConfiguration table in tables)
         {
            if( !trgList.ContainsKey( CoreHelpers.GetTableNameKey( table.Schema, table.Name ) ))
               CreateDMLTrigger(database, table);

         }
      }
   }
    
   //----------------------------------------------------------------------
   //
   //
   //----------------------------------------------------------------------
   public void CreateDMLTriggers( string database,
                                  TableConfiguration [] tables )
   {
      if( tables != null )
      {
         StringBuilder newTriggers = new StringBuilder("Triggers are created on the following tables: \n\r");
         int count = 0;
         foreach (TableConfiguration table in tables)
         {
            try
            {
               CreateDMLTrigger( database, table );
               newTriggers.AppendFormat( "\t[{0}].[{1}]\n\r", table.Schema, table.Name );
               count++;
            }
            catch( Exception e )
            {
               ErrorLog.Instance.Write(
                  String.Format("An error occurred creating trigger on {0}.{1}.",
                                 table.Schema, table.Name),
                  e,
                  ErrorLog.Severity.Warning,
                  true);
            }
         }
         
         if( count > 0 )
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose, newTriggers.ToString() );
      }
   }
    * */

   //----------------------------------------------------------------------
   //
   //
   //----------------------------------------------------------------------
   internal void CreateDMLTrigger(string database, TableConfiguration table)
   {
      DMLTriggerInfo ti = new DMLTriggerInfo(_instance, 
                                             database, 
                                             table.Schema, 
                                             table.Name, 
                                             table.MaxRows,
                                             table.Columns);
      ti.Manager = this;
      if( _init )
         ti.CheckHealth( true );
      else
         ti.Create( false );
   }

   //----------------------------------------------------------------------
   //
   //
   //----------------------------------------------------------------------
   internal void RecreateDMLTrigger(string database, TableConfiguration table)
   {
      DMLTriggerInfo ti = new DMLTriggerInfo(_instance,
                                             database,
                                             table.Schema,
                                             table.Name,
                                             table.MaxRows,
                                             table.Columns);
      ti.Manager = this;
      ti.Create(true);
   }


   //----------------------------------------------------------------------
   //
   //
   //----------------------------------------------------------------------
   public void DropDMLTriggers(string database,
                                TableConfiguration [] tables )
   {
      if ( tables != null )
      {
         StringBuilder removed = new StringBuilder( "Triggers on the following tables are dropped:\n\r" );
         int count = 0;
         foreach ( TableConfiguration table in tables )
         {
            DMLTriggerInfo ti = new DMLTriggerInfo(_instance,
                                                   database,
                                                   table.Schema,
                                                   table.Name,
                                                   table.MaxRows,
                                                   table.Columns,
                                                   true );
            ti.Manager = this;
            ti.Drop();
            removed.AppendFormat( "\t[{0}].[{1}]\n\r", table.Schema, table.Name );
            count++;
         }
         
         if( count > 0 )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                     removed.ToString() );
         }
      }
   }

   //----------------------------------------------------------------------
   //
   //
   //----------------------------------------------------------------------
   internal void CheckTriggerHealth(string database, TableConfiguration table)
   {
      DMLTriggerInfo ti = new DMLTriggerInfo(_instance,
                                             database,
                                             table.Schema,
                                             table.Name,
                                             table.MaxRows,
                                             table.Columns);
      ti.Manager = this;
      ti.CheckHealth(true);
   }

   //----------------------------------------------------------------------
   public void CheckSharedSetup()
   {
      if( !_supportTrigger || _init || !DataChangeEnabled )
         return;
      
      lock( _syncObj )
      {
         try
         {
            bool checkCLR = false;
            foreach ( string dbName in _dbLists.Keys )
            {
               Dictionary<string, TableConfiguration> tables = _dbLists[dbName];
               if ( tables != null && tables.Count > 0 )
               {
                  CheckSetup( dbName, _dbLists[dbName] != null );
                  ErrorLog.Instance.Write( ErrorLog.Level.UltraDebug,
                                           "Data change setup checkd - " + dbName );
                  checkCLR = true;
               }
            }

            if ( checkCLR )
            {
               CheckCLRStatus();
               ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                        "Data change shared components checked." );
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                     "An error occurred checking data change setup.",
                                     e.Message,
                                     ErrorLog.Severity.Warning );
         }
      }
   }

   //----------------------------------------------------------------------
   //
   //
   //----------------------------------------------------------------------
   public void UpdateConfiguration(ServerAuditConfiguration config)
   {  
      if( !_supportTrigger )
         return;
      
      lock( _syncObj )
      {
         _newErrors = 0;
         Dictionary<string, Dictionary<string, TableConfiguration>> newList
            = new Dictionary<string, Dictionary<string, TableConfiguration>>();
         bool checkCLR = false;

         if ( config != null && config.AuditDBList != null )
         {
            if ( config.IsEnabled )
            {
               // check each database's data change setup
               foreach ( DBAuditConfiguration dbConfig in config.AuditDBList )
               {
                  try
                  {
                     bool bDataChange = true;

                     // Checks if data change is configurated
                     if ( !dbConfig.AuditDML )
                     {
                        bDataChange = false;
                     }
                     else if ( dbConfig.AuditObjectTypeList != null
                               && dbConfig.AuditObjectTypeList.Length > 0 )
                     {
                        bDataChange = false;
                        foreach ( DBObjectType type in dbConfig.AuditObjectTypeList )
                        {
                           if ( type == DBObjectType.UserTable_2005 )
                           {
                              bDataChange = true;
                              break;
                           }
                        }
                     }
                     // else all object types are audited

                     // Update data change setup
                     TableConfiguration[] newTables = null;
                     if (_dbLists.ContainsKey(dbConfig.Name))
                     {
                        if( bDataChange )
                        {
                           newTables = dbConfig.DataChangeTables;
                           if ( newTables != null && newTables.Length > 0 )
                           {
                              checkCLR = true;
                              newList.Add( dbConfig.Name,
                                           UpdateConfiguration( dbConfig.Name,
                                                                _dbLists[dbConfig.Name],
                                                                newTables ) );
                           }
                           else  // no tables configured, remove the data change components
                           {
                              UpdateConfiguration( dbConfig.Name,
                                                   _dbLists[dbConfig.Name],
                                                   null );
                           }
                        }
                        else // user tables are not audited
                        {
                           UpdateConfiguration( dbConfig.Name,
                                                _dbLists[dbConfig.Name],
                                                null );
                        }
                        _dbLists.Remove( dbConfig.Name );
                     }
                     else if ( bDataChange ) // newly added database
                     {
                        if ( dbConfig.DataChangeTables != null &&
                             dbConfig.DataChangeTables.Length > 0 )
                        {
                           checkCLR = true;
                           newList.Add(dbConfig.Name, GetTableConfigurations(dbConfig));
                           UpdateConfiguration(dbConfig.Name,
                                               null,
                                               dbConfig.DataChangeTables);
                        }
                     }
                                            
                  }
                  catch ( Exception e )
                  {
                     ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                              "An error occurred updating trigger configurations for " +
                                              dbConfig.Name + ".",
                                              e,
                                              ErrorLog.Severity.Warning,
                                              true );
                  }
               }
            }
         }
         
         CleanUpTriggerSetup( _dbLists );
         _dbLists = newList;
         DataChangeEnabled = checkCLR;
         
         if ( checkCLR )
         {
            CheckCLRStatus();
         }

         if( !_init )  // the time is set by the instance during instance initialization
         {
            if( _sqlInstance != null )
               _sqlInstance.LastDCSetupCheckTime = DateTime.Now;
            
            // Reset agent health flags if no new errors  found after configuration update.
            if( _newErrors == 0 && _errorTriggers.Count > 0 )
               ReportTriggerResolutionAll();
         }
         else
         {
            // set the initialization flag to false so trigger checks are on from now on.
            _init = false;
         }
      }
   }

   //----------------------------------------------------------------------
   //
   //
   //----------------------------------------------------------------------
   public Dictionary<string, TableConfiguration> UpdateConfiguration( string database,
                                    Dictionary<string, TableConfiguration> oldTables,
                                    TableConfiguration [] newTables )
   {
      Dictionary<string, TableConfiguration> newList = new Dictionary<string, TableConfiguration>();

      if (!_supportTrigger)
         return newList;

      if (newTables == null || newTables.Length == 0)
      {
         try
         {
            CleanUpTriggerSetup( database ); // Remove everything
         }
         catch( Exception e )
         {
            string msg =
               String.Format(
                  "An error occurred removing trigger setup for database {0}.", database );
            ErrorLog.Instance.Write( msg,
                                     e,
                                     ErrorLog.Severity.Warning );
         }

         return newList;
      }
      
      CheckSetup( database, oldTables != null && oldTables.Count > 0 );  // Check share assembly and data change setup in the database
      
      StringBuilder newTriggers = new StringBuilder("Triggers are created on the following tables: \n\r");
      int count = 0;
      if( oldTables == null ) oldTables = new Dictionary<string, TableConfiguration>( );
      foreach( TableConfiguration table in newTables )
      {
         try
         {
            string key = GetTableNameKey( table );
            if ( oldTables.ContainsKey( key ) )
            {
               if ( table.Changed(oldTables[key]))
               {
                  RecreateDMLTrigger( database, table );
                  newTriggers.AppendFormat( "\t[{0}].[{1}]\n\r", table.Schema, table.Name );
                  count++;
               }
               else
               {
                  CheckTriggerHealth( database, table );
               }
               newList.Add( key, table );
               oldTables.Remove( key );
            }
            else
            {
               CreateDMLTrigger( database, table );
               newTriggers.AppendFormat( "\t[{0}].[{1}]\n\r", table.Schema, table.Name );
               count++;
               newList.Add( key, table );
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( 
               String.Format( "An error occurred updating data change setup for {0}.{1} in database {2}.", 
                              table.Schema, table.Name, database ),
               e,
               ErrorLog.Severity.Warning,
               true  );
         }
      }
      
      if (count > 0)
      {
         if( !_init )
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose, newTriggers.ToString());
      }
      try
      {
         CleanUpTriggerSetup( database, oldTables, false );
      }
      catch( Exception e )
      {
         string msg =
            String.Format(
               "An error occurred removing trigger setup for database {0}.", database );
         ErrorLog.Instance.Write( msg,
                                  e,
                                  ErrorLog.Severity.Warning );
      }
      return newList;
   }


   //----------------------------------------------------------------------
   //
   //
   //----------------------------------------------------------------------
   public static void CleanUpTriggerSetup(string instance, string database)
   {

      // Drop all triggers and their assemblies
      using (SqlConnection conn = TriggerHelpers.GetConnection(instance, database))
      {
         DMLTriggerInfo[] trgs = TriggerHelpers.GetTriggers(conn, database);
         foreach (DMLTriggerInfo trg in trgs)
            trg.Drop();
         RemoveSharedSetup(conn);
      }
   }

   //----------------------------------------------------------------------
   //
   //
   //----------------------------------------------------------------------
   public void RemoveSharedSetup(string instance, string database)
   {
      using (SqlConnection conn = _sqlInstance.GetConnection(database))
      {
         RemoveSharedSetup(conn);
      }
   }
   
   public SqlConnection GetConnection( string database )
   {
      return _sqlInstance.GetConnection( database );
   }

   #endregion
   
   #region Private Methods
   
   private static string GetTableNameKey( TableConfiguration table )
   {
      return CoreHelpers.GetTableNameKey( table.Schema, table.Name );
   }

   //----------------------------------------------------------------------
   private void CheckCLRStatus()
   {
      if( !_supportTrigger )
         return;
         
      using ( SqlConnection conn = _sqlInstance.GetConnection( ) )
      {
         try
         {
            bool config,
                 running;
            RawSQL.GetCLRStatus( conn, out config, out running );
            if ( !config || !running )
            {
               ReportCLRError();
               RawSQL.EnableCLR( conn );
               ReportCLRResolution();
               ErrorLog.Instance.Write(
                  String.Format(
                     "CLR was disabled on {0} and it is re-enabled. ",
                     _instance ),
                  ErrorLog.Severity.Warning );
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                    String.Format( 
                                                   "An error occurred check CLR status on {0}. ",
                                                   _instance ),
                                    e.Message,
                                    ErrorLog.Severity.Warning );
         }
      }
   }

   //----------------------------------------------------------------------
   //
   //
   //----------------------------------------------------------------------
   private void CheckSetup( string database, bool shouldExist )
   {
      if (!_supportTrigger)
         return;
      try
      {
         using ( SqlConnection conn = _sqlInstance.GetConnection( database ) )
         {
            if (
               !TriggerHelpers.DoesAssemblyExist( conn, TriggerHelpers.SharedAssemblyName ) )
            {
               if ( shouldExist )
                  ReportSharedAssemblyError( database );
               CreateSharedAssembly( conn );
               if ( shouldExist )
               {
                  ReportSharedAssemblyResolution( database );
                  ErrorLog.Instance.Write(
                     String.Format(
                        "Data change shared assembly in database {0} on {1} is missing. " +
                        "Shared assembly is recreated.",
                        database,
                        _instance ),
                     ErrorLog.Severity.Warning );
               }
            }

            if ( !TriggerHelpers.DoesSchemaExist( conn,
                                                  CoreConstants.
                                                     Agent_BeforeAfter_SchemaName ) )
            {
               if ( shouldExist )
                  ReportSpecialSchemaError( database );
               CreateSpecialSchema( conn );
               if ( shouldExist )
               {
                  ReportSpecialSchemaResolution( database );
                  ErrorLog.Instance.Write(
                     String.Format(
                        "Data change special schema in database {0} on {1} is missing. " +
                        "Schema is recreated.",
                        database,
                        _instance ),
                     ErrorLog.Severity.Warning );
               }
            }

            if ( !TriggerHelpers.DoesTableExist( conn,
                                                 CoreConstants.
                                                    Agent_BeforeAfter_SchemaName,
                                                 CoreConstants.Agent_BeforeAfter_TableName ) )
            {
               if ( shouldExist )
                  ReportSpecialTableError( database );
               CreateSpecialTable( conn );
               if ( shouldExist )
               {
                  ReportSpecialTableResolution( database );
                  ErrorLog.Instance.Write(
                     String.Format(
                        "Data change special table in database {0} on {1} is missing. " +
                        "Table is recreated.",
                        database,
                        _instance ),
                     ErrorLog.Severity.Warning );
               }
            }
                    else
                    {
                        UpdateSpecialTable(conn);
                    }
         }
      }
      catch( Exception e )
      {
         string msg =
            String.Format(
               "An error occurred checking trigger shared component setup for database {0}.", database );
         ErrorLog.Instance.Write( msg,
                                  e,
                                  ErrorLog.Severity.Warning );
      }
   }

   //----------------------------------------------------------------------
   //
   //
   //----------------------------------------------------------------------
   private static void CreateSharedAssembly(SqlConnection conn)
   {
      TriggerHelpers.CreateAssembly( conn,
                                     TriggerHelpers.SharedAssemblyName,
                                     TriggerHelpers.GetFullShardAssemblyName());
   }

   //----------------------------------------------------------------------
   private static void CreateSpecialSchema(SqlConnection conn)
   {
      string stmt =
         String.Format( "CREATE SCHEMA {0}", CoreConstants.Agent_BeforeAfter_SchemaName );
      Execute(conn, stmt);
   }

   //----------------------------------------------------------------------
   private static void CreateSpecialTable(SqlConnection conn)
   {
      string stmt =
            String.Format("CREATE TABLE {0}.{1} ( col1 tinyint ); GRANT SELECT on [{0}].[{1}] to [public]",
                        CoreConstants.Agent_BeforeAfter_SchemaName,
                        CoreConstants.Agent_BeforeAfter_TableName);
            //String.Format("CREATE TABLE {0}.{1} ( col1 tinyint ); GRANT SELECT, DELETE on [{0}].[{1}] to [public]",
            //               CoreConstants.Agent_BeforeAfter_SchemaName,
            //               CoreConstants.Agent_BeforeAfter_TableName );
            Execute(conn, stmt);
   }
        private static void UpdateSpecialTable(SqlConnection conn)
        {
            string stmt =
                  String.Format("REVOKE DELETE ON [{0}].[{1}] to [public] " + Environment.NewLine + " GRANT SELECT on [{0}].[{1}] to [public]",
                              CoreConstants.Agent_BeforeAfter_SchemaName,
                              CoreConstants.Agent_BeforeAfter_TableName);
            //String.Format("CREATE TABLE {0}.{1} ( col1 tinyint ); GRANT SELECT, DELETE on [{0}].[{1}] to [public]",
            //               CoreConstants.Agent_BeforeAfter_SchemaName,
            //               CoreConstants.Agent_BeforeAfter_TableName );
            Execute(conn, stmt);
        }
        //----------------------------------------------------------------------
        private static void DropSharedAssembly(SqlConnection conn)
   {
      TriggerHelpers.DropAssembly( conn, TriggerHelpers.SharedAssemblyName );
   }

   //----------------------------------------------------------------------
   private static void DropSpecialTable(SqlConnection conn)
   {
      string stmt =
         String.Format( "DROP TABLE {0}.{1}",
                        CoreConstants.Agent_BeforeAfter_SchemaName,
                        CoreConstants.Agent_BeforeAfter_TableName );

      Execute(conn, stmt);
   }

   //----------------------------------------------------------------------
   private static void DropSpecialSchema(SqlConnection conn)
   {
      string stmt =
         String.Format( "DROP SCHEMA {0}", CoreConstants.Agent_BeforeAfter_SchemaName );
      Execute( conn, stmt ) ;
   }


   private static void Execute( SqlConnection conn, string stmt )
   {
      try
      {
         using ( SqlCommand cmd = new SqlCommand( stmt, conn ) )
            cmd.ExecuteNonQuery();
      }
      catch ( Exception e )
      {
         ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                  "Error executing the statement: " + stmt,
                                  e,
                                  ErrorLog.Severity.Warning,
                                  true );
      }
   }

   //----------------------------------------------------------------------
   //
   //
   //----------------------------------------------------------------------
   private static Dictionary<string, TableConfiguration> GetTableConfigurations(DBAuditConfiguration config)
   {
      Dictionary<string, TableConfiguration> list =
         new Dictionary<string, TableConfiguration>( config.DataChangeTables.Length );
      foreach ( TableConfiguration table in config.DataChangeTables )
         list.Add( GetTableNameKey( table), table );
      return list;
   }

   //----------------------------------------------------------------------
   //
   //
   //----------------------------------------------------------------------
   private void CleanUpTriggerSetup(string database)
   {

      try
      {
         // Drop all triggers and their assemblies
         using ( SqlConnection conn = _sqlInstance.GetConnection( database ) )
         {
            DMLTriggerInfo[] trgs = TriggerHelpers.GetTriggers( conn, database );
            foreach ( DMLTriggerInfo trg in trgs )
               trg.Drop();
            RemoveSharedSetup( conn );
         }
      }
      catch( Exception e )
      {
         string msg =
            String.Format(
               "An error occurred removing trigger setup for database {0}.", database );
         ErrorLog.Instance.Write( msg,
                                  e,
                                  ErrorLog.Severity.Warning );
      }
   }

   //----------------------------------------------------------------------
   //
   //
   //----------------------------------------------------------------------
   private void CleanUpTriggerSetup(Dictionary<string, Dictionary<string, TableConfiguration>> dbList)
   {
      foreach ( string database in dbList.Keys )
         CleanUpTriggerSetup( database );
   }

   /*
   //----------------------------------------------------------------------
   //
   //
   //----------------------------------------------------------------------
   private void CleanUpTriggerSetup(string database, Dictionary<string, TableConfiguration> tables)
   {
      CleanUpTriggerSetup( database, tables, true );
   }
    */

   //----------------------------------------------------------------------
   //
   //
   //----------------------------------------------------------------------
   private void 
      CleanUpTriggerSetup(string database, 
                          Dictionary<string, TableConfiguration> tables, 
                          bool removeShared)
   {
      try 
      {
         // Drop all triggers and their assemblies
         TableConfiguration [] tmp = new TableConfiguration[tables.Count];
         tables.Values.CopyTo( tmp, 0 );
         DropDMLTriggers( database, tmp );
         
         if( removeShared )
            using( SqlConnection conn = _sqlInstance.GetConnection( database ) )
               RemoveSharedSetup (conn );
      }
      catch( Exception e )
      {
         string msg =
            String.Format(
               "An error occurred removing trigger setup for database {0}.", database );
         ErrorLog.Instance.Write( msg,
                                  e,
                                  ErrorLog.Severity.Warning );
      }
  }
   
   private static void RemoveSharedSetup( SqlConnection conn )
   {
      try
      {
         DropSpecialTable( conn );
         DropSpecialSchema( conn );
         DropSharedAssembly( conn );
      }
      catch( SqlException se )
      {
         string msg =
            String.Format(
               "An error occurred removing shared data change setup for database {0} on instance {1}.",
               conn.Database,
               conn.DataSource );
         ErrorLog.Instance.Write( msg, se, ErrorLog.Severity.Warning, true );
      }
   }

   #endregion
   #region System Alerts

   //--------------------------------------------------------------------------
   private void ReportCLRError()
   {
      ReportDataChangeSetupError( _instance,
                                  "",
                                  DataChangeComponent.CLR,
                                  "" );
   }

      //--------------------------------------------------------------------------
   private void ReportSharedAssemblyError(string database)
   {
      ReportDataChangeSetupError(_instance,
                                  database,
                                  DataChangeComponent.SharedAssembly,
                                  TriggerHelpers.SharedAssemblyName);
   }

   //--------------------------------------------------------------------------
   private void ReportSpecialSchemaError(string database)
   {
      ReportDataChangeSetupError(_instance,
                                  database,
                                  DataChangeComponent.SpecialSchema,
                                  CoreConstants.Agent_BeforeAfter_SchemaName);
   }

   //--------------------------------------------------------------------------
   private void ReportSpecialTableError(string database)
   {
      ReportDataChangeSetupError(_instance,
                                  database,
                                  DataChangeComponent.SpecialTable,
                                  CoreConstants.Agent_BeforeAfter_TableName);
   }

   //--------------------------------------------------------------------------
   internal void ReportTriggerError(string instance, string database, string name, string error)
   {
      Dictionary <string, string> list;
      _newErrors++;

      if( _errorTriggers.ContainsKey( database ) )
      {
         list = _errorTriggers[database];
         if ( list.ContainsKey( name ) )
         return;
      }
      else
      {
         list = new Dictionary<string, string>();
         list.Add( name, error );
         _errorTriggers.Add( database, list );
      }
      
      ReportDataChangeSetupError(instance,
                                  database,
                                  DataChangeComponent.Trigger,
                                  name,
                                  error);
   }

   //--------------------------------------------------------------------------
   internal void ReportAssemblyError(string instance, string database, string name, string error)
   {
      Dictionary <string, string> list;
      
      if( _errorAssemblies.ContainsKey( database ) )
      {
         list = _errorAssemblies[database];
         if ( list.ContainsKey( name ) )
         return;
      }
      else
      {
         list = new Dictionary<string, string>();
         list.Add( name, error );
         _errorAssemblies.Add( database, list );
      }
      
      ReportDataChangeSetupError( instance,
                                  database,
                                  DataChangeComponent.Assembly,
                                  name,
                                  error );
   }

   //--------------------------------------------------------------------------
   private void ReportCLRResolution()
   {
      ReportDataChangeSetupResolution(_instance,
                                  "",
                                  DataChangeComponent.CLR,
                                  "");
   }

   //--------------------------------------------------------------------------
   private void ReportSharedAssemblyResolution(string database)
   {
      ReportDataChangeSetupResolution(_instance,
                                  database,
                                  DataChangeComponent.SharedAssembly,
                                  TriggerHelpers.SharedAssemblyName);
   }

   //--------------------------------------------------------------------------
   private void ReportSpecialSchemaResolution(string database)
   {
      ReportDataChangeSetupResolution(_instance,
                                  database,
                                  DataChangeComponent.SpecialSchema,
                                  CoreConstants.Agent_BeforeAfter_SchemaName);
   }

   //--------------------------------------------------------------------------
   private void ReportSpecialTableResolution(string database)
   {
      ReportDataChangeSetupResolution(_instance,
                                  database,
                                  DataChangeComponent.SpecialTable,
                                  CoreConstants.Agent_BeforeAfter_TableName);
   }

   //--------------------------------------------------------------------------
   internal void ReportTriggerResolution(string database, string name)
   {
      _newErrors--;
      if( _errorTriggers.ContainsKey( database ) )
      {
         Dictionary<string, string> list = _errorTriggers[database];
         if ( list.ContainsKey( name ) )
         {
            list.Remove( name );
         }
         if ( list.Count == 0 )
            _errorTriggers.Remove( database );
      }
      
      if( _errorTriggers.Count == 0 )
      {
         _hasTriggerErrors = false;
      }
      
      ReportDataChangeSetupResolution( _instance,
                                       database,
                                       DataChangeComponent.Trigger,
                                       name );
   }

   //--------------------------------------------------------------------------
   internal void ReportTriggerResolutionAll()
   {
      _hasTriggerErrors = false;
      _errorTriggers.Clear();
      _newErrors = 0;

      ReportDataChangeSetupResolution(_instance,
                                       null,
                                       DataChangeComponent.Trigger,
                                       null);
   }
   //--------------------------------------------------------------------------
   internal void ReportAssemblyResolution(string database, string name)
   {
      if (_errorAssemblies.ContainsKey(database))
      {
         Dictionary<string, string> list = _errorAssemblies[database];
         if (list.ContainsKey(name))
         {
            list.Remove(name);
         }
         if (list.Count == 0)
            _errorAssemblies.Remove(database);
      }

      if (_errorAssemblies.Count == 0)
      {
         _hasAssemblyErrors = false;
      }

      ReportDataChangeSetupResolution(_instance,
                                       database,
                                       DataChangeComponent.Assembly,
                                       name);
   }

   //--------------------------------------------------------------------------
   private void
      ReportDataChangeSetupError(string instance,
                                  string database,
                                  DataChangeComponent component,
                                  string name )
   {
      ReportDataChangeSetupError( instance, database, component, name, "" );
   }

      //--------------------------------------------------------------------------
   private void
      ReportDataChangeSetupError(string instance,
                                  string database,
                                  DataChangeComponent component,
                                  string name,
                                  string error )
   {
      string details = "";
      
      try
      {
         switch ( component )
         {
            case DataChangeComponent.SharedAssembly:
               details =
                  String.Format( "Shared assembly for {0} in database {1} is missing.",
                                 instance,
                                 database );
               break;
            case DataChangeComponent.SpecialSchema:
               details =
                  String.Format(
                     "Data change special schema for {0} in database {1} is missing.",
                     instance,
                     database );
               break;
            case DataChangeComponent.SpecialTable:
               details =
                  String.Format(
                     "Data change special table for {0} in database {1} is missing.",
                     instance,
                     database );
               break;
            case DataChangeComponent.Trigger:
               _hasTriggerErrors = true;
               details =
                  String.Format(
                     "An error found during data change trigger creation or health check.\n\r" +
                     "\tTrigger: {0}\n\r\tServer: {1}\n\r\tDatabase {2}\n\r\t" +
                     "Error: {3}",
                     name,
                     instance,
                     database,
                     error );
               break;
            case DataChangeComponent.Assembly:
               _hasAssemblyErrors = true;
               details =
                  String.Format(
                     "An error found during data change assembly creation or health check.\n\r" +
                     "\tAssembly: {0}\n\r\tServer: {1}\n\r\tDatabase {2}\n\r\t" +
                     "Error: {3}",
                     name,
                     instance,
                     database,
                     error );
               break;
            case DataChangeComponent.CLR:
               details =
                  String.Format(
                     "CLR is not enabled on {0}.",
                     instance );
               break;
         }

         ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                  details,
                                  ErrorLog.Severity.Warning );
         SystemAlert alert =
            new SystemAlert( SystemAlertType.ClrError,
                             DateTime.UtcNow,
                             instance,
                             details );
         _sqlInstance.SubmitSystemAlert( alert );
      }
      catch (Exception e)
      {
         string msg = String.Format("An error occurred submitting data change error system alert.\n\r" +
                                     "Instance: {0}, database: {1}, type: {2}, name: {3}, alert: {4}",
                                     instance, database, component, name, details );
         ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                  msg,
                                  e,
                                  ErrorLog.Severity.Warning,
                                  true);
      }

   }

   //--------------------------------------------------------------------------
   private void
      ReportDataChangeSetupResolution(string instance,
                                  string database,
                                  DataChangeComponent component,
                                  string name)
   {
      string details = "";
      bool noMoreErrors = true;
      try
      {
         switch ( component )
         {
            case DataChangeComponent.SharedAssembly:
               details =
                  String.Format( "Shared assembly for {0} in database {1} is recreated.",
                                 instance,
                                 database );
               break;
            case DataChangeComponent.SpecialSchema:
               details =
                  String.Format(
                     "Data change special schema for {0} in database {1} is recreated.",
                     instance,
                     database );
               break;
            case DataChangeComponent.SpecialTable:
               details =
                  String.Format(
                     "Data change special table for {0} in database {1} is recreated.",
                     instance,
                     database );
               break;
            case DataChangeComponent.Trigger:
               noMoreErrors = !_hasTriggerErrors;
               if( database == null || name == null )
               {
                  details =
                     String.Format(
                        "All trigger errors are cleared." );
               }
               else 
                  details =
                     String.Format(
                        "Errors on trigger {0} in databse {1} are resolved.",
                        database,
                        name );
               break;
            case DataChangeComponent.Assembly:
               noMoreErrors = !_hasAssemblyErrors;
               details =
                  String.Format(
                     "Errors on assembly {0} in database {1}.",
                     database,
                     name );
               break;
            case DataChangeComponent.CLR:
               details =
                  String.Format(
                     "CLR is enabled on {0}.",
                     instance );
               break;
         }

         ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                  details,
                                  ErrorLog.Severity.Informational );
         SystemAlert alert =
            new SystemAlert( SystemAlertType.ClrResolution,
                             DateTime.UtcNow,
                             instance,
                             details );
         _sqlInstance.SubmitSystemAlert( alert, noMoreErrors );
      }
      catch( Exception e )
      {
         string msg = String.Format( "An error occurred submitting data change resolution system alert.\n\r" +
                                     "Instance: {0}, database: {1}, type: {2}, name: {3}",
                                     instance, database, component, name );
         ErrorLog.Instance.Write( ErrorLog.Level.Default,
                                  msg,
                                  e,
                                  ErrorLog.Severity.Warning,
                                  true );
      }
   }

   #endregion

}
}
