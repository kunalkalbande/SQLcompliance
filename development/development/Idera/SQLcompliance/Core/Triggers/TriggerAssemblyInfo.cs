using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Idera.SQLcompliance.Core.Agent;

namespace Idera.SQLcompliance.Core.Triggers
{
   public enum AssemblyStatus
   {
      NotExist = 0,
      Exists = 1,
      Unknown = 2
   }
   
   public class TriggerAssemblyInfo
   {
   
      #region data members

      private readonly string _instance;
      private readonly string _database;
      private readonly string _name;
      private string _fullname = null;
      private AssemblyStatus _exist = AssemblyStatus.Unknown;
      private readonly string _table;
      private readonly string _schema;
      private readonly int _rowLimit = 10;
      private DateTime _createDate = DateTime.MinValue;
      private TriggerManager _manager = null;
      private readonly string[] _columns;

      #endregion
      
      #region Constructor
      
      public TriggerAssemblyInfo( 
         string instance,
         string database,
         string schema,
         string table,
         string [] columns,
         int rowLimit )
         : this( instance, database, schema, table, columns, false )
      {
         _rowLimit = rowLimit;
      }
      
      public TriggerAssemblyInfo( 
         string instance,
         string database,
         string schema,
         string table,
         string [] columns,
         bool   getStatus ) 
      {
         _instance = instance;
         _database = database;
         _table = table;
         _schema = schema;
         _columns = columns;
         _name = TriggerHelpers.GetAssemblyName(_schema, _table);
         _fullname = GetFullname();
         
         if( getStatus )
            GetStatus();
      }

      #endregion
      
      #region Properties
      
      public string Name 
      { 
         get
         {
            return _name;
         }
      }
      
      public string Filename
      {
         get
         {
            if ( _fullname == null )
            {
               _fullname = GetFullname();
            }
            return _fullname;
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
      
      public string Instance
      {
         get
         {
            return _instance;
         }
      }
      
      public string Database
      {
         get
         {
            return _database;
         }
      }

      public bool FileExists
      {
         get
         {
            FileInfo fi = new FileInfo( Filename );
            return fi.Exists;
         }
      }
      
      public DateTime FileCreateDate
      {
         get
         {
            FileInfo fi = new FileInfo( Filename );
            return fi.CreationTime;
         }
      }
      
      public DateTime CreateDate
      {
         get
         {
            if( _exist == AssemblyStatus.Unknown )
               GetStatus();
            return _createDate;
         }
      }

      public bool Exists
      {
         get
         {
            if( _exist == AssemblyStatus.Unknown )
               return GetStatus();
            return _exist == AssemblyStatus.Exists;
         }
      }
      
      public TriggerManager Manager
      {
         get
         {
            return _manager;
         }
         set
         {
            _manager = value;
         }
      }

      #endregion
      
      #region Public Methods
      
      private bool GetStatus()
      {
         using( SqlConnection conn = GetConnection(  ) )
         {
            string query = String.Format( "SELECT modify_date FROM sys.assemblies WHERE name = '{0}'", _name );
            using( SqlDataReader reader = TriggerHelpers.ExecuteReader(  conn, query ) )
            {
               if( reader.HasRows )
               {
                  reader.Read();
                  _exist = AssemblyStatus.Exists;
                  _createDate = reader.GetDateTime( 0 );
               }
               else
               {
                  _exist = AssemblyStatus.NotExist;
                  _createDate = DateTime.MinValue;
               }
            }
         }
         
         return _exist == AssemblyStatus.Exists;
      }
      
      public void Create()
      {
         Create( false );
      }

      public void Create( bool recreate )
      {
         CheckPath();
         using (SqlConnection conn = GetConnection())
         {
            if( !FileExists || recreate )
            {
               try
               {
                  List<string> pkCols = GetPrimaryKeyColumns( conn );
                  if (_columns == null || _columns.Length == 0)
                  {
                     TriggerHelpers.GenerateDMLTriggerAssembly(Filename,
                                                                _schema,
                                                                _table,
                                                                pkCols.ToArray(),
                                                                _rowLimit);
                  }
                  else
                  {
                     TriggerHelpers.GenerateDMLTriggerAssembly(Filename,
                                                                _schema,
                                                                _table,
                                                                pkCols.ToArray(),
                                                                _rowLimit,
                                                                _columns);
                  }
               }
               catch( Exception e )
               {
                  ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                           String.Format(
                                              "An error occurred generating CLR assembly code for {0}.{1}.",
                                              _schema,
                                              _table ),
                                           e,
                                           ErrorLog.Severity.Warning,
                                           true );
                  throw;
               }
            }
            
            try
            {
               TriggerHelpers.CreateAssembly(conn,
                                             _name,
                                             _fullname,
                                             recreate );
            }
            catch (Exception e)
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        String.Format(
                                           "An error occurred creating assembly for {0}.{1}.",
                                           _schema,
                                           _table),
                                        e,
                                        ErrorLog.Severity.Warning,
                                        true);
               throw;
            }
         }
      }
      
      public void Drop()
      {
         if( FileExists )
         {
            File.Delete( _fullname );
         }
         
         using (SqlConnection conn = GetConnection())
         {
            try
            {
               TriggerHelpers.DropAssembly( conn, _name );
            }
            catch ( SqlException se )
            {
               ErrorLog.Instance.Write( ErrorLog.Level.Verbose, 
                                        "An error occurred dropping assembly " + _name,
                                        se,
                                        ErrorLog.Severity.Warning,
                                        true );
               if ( se.Number != 15151 ) // ignore assembly doesn't exist error
                  throw;
            }
         }
      }
      
      public void Recreate()
      {
         Create(true);
      }
      
      public bool IsHealthy()
      {
         return true;
      }

      #endregion
      
      
      
      #region Private Methods
      
      private void CheckPath()
      {
         string dllPath = Path.Combine(GetRootDirectory(), "Assemblies");
         CheckDirectory(dllPath);

         dllPath = Path.Combine(dllPath,
                                 TriggerHelpers.GetSaveInstancename(_instance));
         CheckDirectory(dllPath);
         
         dllPath = Path.Combine(dllPath, _database);
         CheckDirectory(dllPath);

         if (_schema != null && _schema.Length > 0)
         {
            dllPath = Path.Combine(dllPath, _schema);
            CheckDirectory(dllPath);
         }

      }

      private static void CheckDirectory(string path)
      {
         try
         {
            DirectoryInfo di = new DirectoryInfo( path );
            if ( !di.Exists )
               di.Create();
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                     "An error occurred creating assembly directory " +
                                     path + ".",
                                     e ,
                                     ErrorLog.Severity.Warning);
            throw;
         }
      }
      
      private string GetFullname()
      {
         string dllPath = Path.Combine(GetRootDirectory(), "Assemblies");

         dllPath = Path.Combine( dllPath, 
                                 TriggerHelpers.GetSaveInstancename( _instance ) );

         dllPath = Path.Combine( dllPath, _database );

         if ( _schema != null && _schema.Length > 0 )
         {
            dllPath = Path.Combine( dllPath, _schema );
         }

         return Path.Combine( dllPath, _name + ".dll" );
      }
      
      private string GetRootDirectory()
      {
         if (String.IsNullOrEmpty(SQLcomplianceAgent.Instance.AssemblyRootDirectory))
         {
            FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            return fileInfo.DirectoryName;
         }
         else
            return SQLcomplianceAgent.Instance.AssemblyRootDirectory;
      }

      private List<string> GetPrimaryKeyColumns( SqlConnection conn )
      {
         List<string>primaryKeyColumns = new List<string>( );
         string schema = _schema;
         try
         {
            if (schema == null || schema.Length == 0)
               schema = "dbo";
            string query =
               String.Format(
                  "SELECT c.COLUMN_NAME from INFORMATION_SCHEMA.TABLE_CONSTRAINTS pk ," +
                  "INFORMATION_SCHEMA.KEY_COLUMN_USAGE c where pk.TABLE_NAME = '{0}' and pk.TABLE_SCHEMA = '{1}'" +
                  " and pk.CONSTRAINT_TYPE = 'PRIMARY KEY' and c.TABLE_NAME = pk.TABLE_NAME and c.TABLE_SCHEMA = pk.TABLE_SCHEMA" +
                  " and c.CONSTRAINT_NAME = pk.CONSTRAINT_NAME ORDER by ORDINAL_POSITION ASC",
                  _table, schema);

            using (SqlDataReader reader = TriggerHelpers.ExecuteReader( conn, query))
            {
               while (reader.Read())
               {
                  primaryKeyColumns.Add(reader[0].ToString());
               }
            }
         }
         catch (Exception e)
         {
            string msg = String.Format( "An error occurred retrieving primary key columns for assembly {0}.  " +
                                        "Schema: {1}.  Table: {2}.  Error: {3}.",
                                        _name,
                                        schema,
                                        _table,
                                        e.Message );

            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
               "An error occurred retrieving primary key columns for table " + _name + ". ",
               e,
               true);
            throw new Exception( msg );
         }
         return primaryKeyColumns;
      }


      private List<string> GetColumns(SqlConnection conn)
      {
         List<string> columns = new List<string>();
         string schema = _schema;
         try
         {
            if (schema == null || schema.Length == 0)
               schema = "dbo";
            string query =
               String.Format(
                  "SELECT COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS WHERE " +
                  "TABLE_NAME = '{0}' and TABLE_SCHEMA = '{1}' " +
                  "ORDER by ORDINAL_POSITION ASC",
                  _table, schema);


            using (SqlDataReader reader = TriggerHelpers.ExecuteReader(conn, query))
            {
               while (reader.Read())
               {
                  columns.Add(reader[0].ToString());
               }
            }
         }
         catch (Exception e)
         {
            string msg = String.Format("An error occurred retrieving columns for assembly {0}.  " +
                                        "Schema: {1}.  Table: {2}.  Error: {3}.",
                                        _name,
                                        schema,
                                        _table,
                                        e.Message);

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
               "An error occurred retrieving columns for table " + _name + ". ",
               e,
               true);
            throw new Exception(msg);
         }
         return columns;
      }


      private SqlConnection GetConnection()
      {
         if( _manager == null )
            return TriggerHelpers.GetConnection( _instance, _database );
         else
            return _manager.GetConnection(_database);
      }

      private bool RetrieveStoredPrimaryKeyCols()
      {
         Assembly asm = Assembly.LoadFrom(_fullname);
         return true;
      }

      #endregion
   }
}
