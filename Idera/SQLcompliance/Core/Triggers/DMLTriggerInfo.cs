using System;
using System.Data;
using System.Data.SqlClient;

namespace Idera.SQLcompliance.Core.Triggers
{
   public class DMLTriggerInfo : TriggerInfo
   {
      #region Constants
      
      internal readonly string DMLTriggerClass = "CLRTriggers";
      internal readonly string DMLTriggerMethod = "DMLTrigger";
      
      #endregion
      
      #region Fields
      
      protected int _rowLimit;
      protected bool _retrieved = false;
      protected string _schema = "";
      protected string _table;
      protected string[] _columns;
      protected bool _statusOnly = false;
      
      #endregion
      
      #region Constructor

      public DMLTriggerInfo(
         string instance,
         string database,
         string schema,
         string table,
         int rowLimit,
         string [] columns
         )
         : this(instance, database, schema, table, rowLimit, columns, false)
      {
      }

      // This constructor is for creating or recreating triggers
      public DMLTriggerInfo(
         string instance,
         string database,
         string schema,
         string table,
         int rowLimit,
         string [] columns,
         bool exists
         ) : base( instance, database )
      {
         _table = table;
         _schema = schema;
         _name = TriggerHelpers.GetTriggerName( table );
         _rowLimit = rowLimit;
         _assembly = new TriggerAssemblyInfo( instance, database, schema, table, columns, rowLimit );
         _exists = exists;
      }
      
      // This constructor is for retrieving trigger info from the servers
      internal DMLTriggerInfo( 
         string instance,
         string database,
         DataRow row ) : base( instance, database )
      {
         GetTriggerInfoFromDataRow( row );
      }

      #endregion
      
      #region Properties
      
      public int RowLimit
      {
         get
         {
            return _rowLimit;
         }
      }
      
      public string FullTableName
      {
         get
         {
            return CoreHelpers.GetTableNameKey( _schema, _table );
         }
      }
      
      public string Table
      {
         get
         {
            return _table;
         }
      }

      public string Schema
      {
         get
         {
            return _schema;
         }
      }

      public TriggerAssemblyInfo Assembly
      {
         get
         {
            return _assembly;
         }
      }

      public DateTime CreateDate
      {
         get
         {
            return _dateCreated;
         }
      }
      

      #endregion
      
      #region Public Methods

      //-----------------------------------------------------------------------
      public override bool Create(bool recreate)
      {
         if (_statusOnly)
         {
            // The info is gotten from the server and doesn't have the list of audited columns
            return false;
         }
         bool success = false;
            
         try 
         {
            if ( recreate )
            {
               Drop();
            }
            else if( _exists )
               return true;

            _assembly.Create( recreate );
            using ( SqlConnection conn = GetConnection() )
            {
               try
               {
                  TriggerHelpers.CreateCLRDMLTrigger( conn,
                                                      _schema,
                                                      _table,
                                                      _name,
                                                      GetMethodSpecifier( _assembly.Name,
                                                                           DMLTriggerClass,
                                                                           DMLTriggerMethod ) );
               }
               catch( Exception e )
               {
                  ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                           String.Format(
                                              "An error occurred creating assembly and trigger for {0}.",
                                              _table ),
                                           e,
                                           ErrorLog.Severity.Warning,
                                           true );
                  if( _manager != null )
                     _manager.ReportTriggerError( _instance, _database, _name, e.Message );
                  success = false;
               }
            }
         }
         catch( SqlException se )
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     String.Format(
                                        "An error occurred creating trigger for {0}.",
                                        _table),
                                     se,
                                     ErrorLog.Severity.Warning,
                                     true);
            success = false;
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     String.Format(
                                        "An error occurred creating trigger for {0}.",
                                        _table ),
                                     e,
                                     ErrorLog.Severity.Warning,
                                     true );
            success = false;
         }
            

         return success;
      }

      //-----------------------------------------------------------------------
      public override bool Drop()
      {
         try
         {
            using ( SqlConnection conn = GetConnection() )
            {
               TriggerHelpers.DropTrigger( conn, _schema, _name );
            }
            _assembly.Drop();
         }
         catch( Exception e )
         {
            string msg = String.Format( "An error occurred dropping trigger{0} on {1}.{2}.{3}.",
                                        _name,
                                        _database,
                                        _schema,
                                        _table);
            ErrorLog.Instance.Write( msg, e, ErrorLog.Severity.Warning, true );
            return false;
         }
         return true;
      }

      //-----------------------------------------------------------------------
      public bool CheckHealth()
      {
         bool create = false;
         bool healthy = true;
         try 
         {
            
            using ( SqlConnection conn = GetConnection() )
            {
               if ( !_assembly.Exists )
               {
                  ReportAssemblyError("assembly is missing" );
                  healthy = false;
                  _assembly.Recreate();
                  create = true;
                  ReportAssemblyResolution( );
                  ErrorLog.Instance.Write( String.Format( "Data change assembly {0} for trigger {1}.{2} is missing. " +
                                                          "Assembly is recreated.",
                                                          _assembly.Name, _schema, _name ),
                                           ErrorLog.Severity.Warning );
                                           
               }
               
               if ( create || !TriggerHelpers.DoesTriggerExist( conn, _schema, _table ) )
               {
                  ReportTriggerError( "trigger is missing");
                  healthy = false;
                  
                  TriggerHelpers.CreateCLRDMLTrigger( conn,
                                                      _schema,
                                                      _table,
                                                      _name,
                                                      GetMethodSpecifier(
                                                         _assembly.Name,
                                                         DMLTriggerClass,
                                                         DMLTriggerMethod ) );
                  ReportTriggerResolution();
                  ErrorLog.Instance.Write( String.Format("Data change trigger {1}.{0} for trigger {1}.{2} is missing. " +
                                                          "Trigger is recreated.",
                                                          _name, _schema, _name),
                                           ErrorLog.Severity.Warning);
               }
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Default,
                                     String.Format(
                                        "An error occurred checking trigger health for {0}.{1}.  {2}.",
                                        _schema,
                                        _table,
                                        e.Message ),
                                     ErrorLog.Severity.Warning );
            return false;
         }


         return healthy;
      }

      //-----------------------------------------------------------------------
      public bool CheckHealth(bool recreate)
      {
         bool create = false;
         bool healthy = true;
         try
         {

            using (SqlConnection conn = GetConnection())
            {
               bool disabled;
               bool renamed; // whether the base table is renamed.
               string newName;
               bool exists = TriggerHelpers.GetTriggerStatus(conn, _schema, _table, out disabled, out renamed, out newName);

               if (exists)
               {
                  if ( renamed ) // Trigger exists but the base table is renamed
                  {
                     healthy = false;
                     string msg = String.Format("Base table of trigger {0}.{1} is renamed from {0}.{2} to {0}.{3}",
                                                 _schema, _name, _table, newName ); 
                     ReportTriggerError( msg );
                     ErrorLog.Instance.Write( msg,
                                              ErrorLog.Severity.Warning);
                  }
                  else if ( disabled ) // trigger exists but disabled
                  {
                     ReportTriggerError( "trigger is disabled" );
                     Enable();
                     ReportTriggerResolution();
                     ErrorLog.Instance.Write(String.Format("Data change trigger {1}.{0} for trigger {1}.{2} is disabled. " +
                                                             "Trigger is re-enabled.",
                                                             _name, _schema, _table ),
                                              ErrorLog.Severity.Warning);
                  }
                  return healthy;
               }
               
               // Check if table is still there
               if( !TriggerHelpers.DoesTableExist( conn, _schema, _table ) )
               {
                  if ( _assembly.Exists ) // remove assembly for the dropped table
                  {
                     string msg = String.Format( "Base table {0}.{1} no longer exists.",
                                                 _schema, _table );
                     ReportAssemblyError( msg );
                     _assembly.Drop();
                     ReportAssemblyResolution();
                     ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                              String.Format(
                                                 "Assembly {0} for non-existing table {1}.{2} is dropped.",
                                                 _assembly.Name,
                                                 _schema,
                                                 _table ),
                                               ErrorLog.Severity.Warning);
                  }
                  return true;
               }


               if (!_assembly.Exists)
               {
                  ReportAssemblyError("assembly is missing");
                  healthy = false;
                  if (recreate)
                  {
                     _assembly.Recreate();
                     create = true;
                     ReportAssemblyResolution();
                  }
                  ErrorLog.Instance.Write(String.Format("Data change assembly {0} for trigger {1}.{2} is missing. " +
                                                          "Assembly is {3}recreated.",
                                                          _assembly.Name, _schema, _name, recreate ? "" : "not "),
                                           ErrorLog.Severity.Warning);

               }

               if (create || !TriggerHelpers.DoesTriggerExist(conn, _schema, _table))
               {
                  ReportTriggerError("trigger is missing");
                  healthy = false;

                  if (recreate)
                  {
                     TriggerHelpers.CreateCLRDMLTrigger(conn,
                                                         _schema,
                                                         _table,
                                                         _name,
                                                         GetMethodSpecifier(
                                                            _assembly.Name,
                                                            DMLTriggerClass,
                                                            DMLTriggerMethod));
                     ReportTriggerResolution();
                     healthy = true;
                  }
                  ErrorLog.Instance.Write(String.Format("Data change trigger {1}.{0} for trigger {1}.{2} is missing. " +
                                                          "Trigger is {3}recreated.",
                                                          _name, _schema, _table, recreate ? "" : "not "),
                                           ErrorLog.Severity.Warning);
               }
            }
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                     String.Format(
                                        "An error occurred checking trigger health for {0}.{1}.  {2}.",
                                        _schema,
                                        _table,
                                        e.Message),
                                     ErrorLog.Severity.Warning);
         }


         return healthy;
      }
      
      //-----------------------------------------------------------------------
      public void Enable()
      {
         try
         {
            using ( SqlConnection conn = GetConnection() )
            {
               TriggerHelpers.EnableTrigger( conn, _schema, _name, _table );
            }
         }
         catch( Exception e )
         {
            throw new Exception( String.Format(
                                        "An error occurred enabling trigger {0} on {1}.  Error: {2}",
                                        _name,
                                        _table,
                                        e.Message ) );
         }
      }

      //-----------------------------------------------------------------------
      public void Disable()
      {
         try
         {
            using (SqlConnection conn = GetConnection())
            {
               TriggerHelpers.DisableTrigger(conn, _schema, _name, _table);
            }
         }
         catch (Exception e)
         {
            throw new Exception(String.Format(
                                        "An error occurred diabling trigger {0} on {1}.  Error: {2}",
                                        _name,
                                        _table,
                                        e.Message));
         }
      }

      #endregion
      
      #region Private Methods
      

      private static string GetMethodSpecifier(string assembly, string aClass, string method)
      {
         return String.Format("[{0}].[{1}].[{2}]",
                               assembly,
                               aClass,
                               method);
      }

            
      private void GetTriggerInfoFromDataRow( DataRow row )
      {
         _name = SQLHelpers.GetRowString( row,"name");
         _schema = SQLHelpers.GetRowString( row,"schemaName");
         _table = SQLHelpers.GetRowString( row, "tableName");
         _dateCreated = SQLHelpers.GetRowDateTime(row,"modify_date");
         _disabled = (bool)row["is_disabled"];
         _statusOnly = true;
         _assembly =
            new TriggerAssemblyInfo( _instance, _database, _schema, _table, null, false );
         _exists = true;
      }
      
      private void 
         ReportTriggerError( string error )
      {
         if ( _manager != null )
            _manager.ReportTriggerError( _instance,
                                         _database,
                                         _name,
                                         error );
      }

      private void
         ReportAssemblyError(string error)
      {
         if (_manager != null)
            _manager.ReportAssemblyError(_instance,
                                         _database,
                                         _assembly.Name,
                                         error);
      }

      private void
         ReportAssemblyResolution()
      {
         if (_manager != null)
            _manager.ReportAssemblyResolution( _database, _assembly.Name);
      }

      private void
         ReportTriggerResolution( )
      {
         if (_manager != null)
            _manager.ReportTriggerResolution(_database, _name);
      }

      #endregion
   }
}
