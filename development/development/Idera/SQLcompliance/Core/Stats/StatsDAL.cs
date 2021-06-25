using System;
using System.Collections;
using System.Text;
using System.Data.SqlClient;
using System.Data;

using Idera.SQLcompliance.Core.Rules;
using Idera.SQLcompliance.Core.Event;


namespace Idera.SQLcompliance.Core.Stats
{
   public enum SQLErrorSeverity
   {
      Info = 0,            // 0-9
      Status = 1,          // 10
      UserError = 2,       // 11-16
      AdminError = 3,      // 17-19
      SystemError = 4      // 20-25
   }
      
   public class StatsDAL
   {
      #region Static Data Members

      static string statsTableColumns = String.Format(" [{0}], [{1}], [{2} ], [{3}],[{4}] ", ColDBId, ColDate, ColCategory, ColCount, ColLastUpdated);

      static string statsTablePK = String.Format(" [{0}], [{1}], [{2} ] ", ColDBId, ColDate, ColCategory);
      #endregion
      
      #region Constructor
      
      StatsDAL()
      {
      }
      
      #endregion
      
      
      #region SQL
      
      // Stats Categories table
      internal const string SQLCreateStatsCategoriesTable = "CREATE TABLE [{0}]..[{1}] ( [{2}] [int] not null, [{3}] nvarchar(64) not null )";
      internal const string SQLInsertStatsCategoryRow = "INSERT INTO [{0}]..[{1}] VALUES ( {2}, {3} )";

      // Stats table
      internal const string SQLDoesTableExist = "select Count(*) from dbo.sysobjects where id = object_id(N'{0}') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
      internal const string SQLCreateStatsTable = "CREATE TABLE {0}[{1}] ( [{2}] [int] not null default 0, [{3}] [DateTime] not null, [{4}] [int] not null, [{5}] [int] not null default 0, [{6}] [DateTime] NOT NULL, PRIMARY KEY CLUSTERED ( [{2}], [{3}] DESC, [{4}] ) ) on [PRIMARY]";
      
      internal const string SQLGetInstanceRecords = "SELECT {0}, {1}, {2}, {3} FROM {4}..{5}";
      internal const string SQLGetInstanceStatsBASE = "SELECT {0}, {1}, {2}, {3} FROM {4}..{5} {6}"; // The last param is the WHERE clause
      internal const string SQLStatsRecordExists = "SELECT COUNT(*) FROM {0}..{1} WHERE {2} = {3} AND {4} = {5} AND {6} = {7}";
      internal const string SQLStatsRecordCountValue = "SELECT {0} FROM {1}..{2} WHERE {3} = {4} AND {5} = {6} AND {7} = {8}";
      
      internal const string SQLGetDbIdsBase = "SELECT {0}, {1}, {2}, {3} FROM {4}..{5}";
      internal const string SQLGetDbId = "SELECT {0} FROM {1}..{2} WHERE {3} = {4} and {5} = {6}";
      
      // Archive and groom
      internal const string SQLCopyStatsRecords = "INSERT INTO {0}..{1} ( {2} ) SELECT {2} FROM {3}..{1} WHERE {4} >= {5} and {4} < {6}";
      internal const string SQLDeleteStatsInRange = "DELETE FROM {0}..{1} WHERE {2} >= {3} and {2} < {4}";
      internal const string SQLDeleteOldStats = "DELETE FROM {0}..{1} WHERE {2} < {3}";
      internal const string SQLEarliestStatsTime = "SELECT MIN({0}) FROM {1}..{2}";

      // Update and Insert statements
      internal const string SQLInsertStatsRecord = "INSERT INTO {0}..{1} ( {2}, {3}, {4}, {5}, {6} ) VALUES ( {7}, {8}, {9}, {10}, {11} );";
      internal const string SQLUpdateStatsRecord = "UPDATE {0}..{1} SET {2} = {3}, {4} = {5} WHERE {6} = {7} AND {8} = {9} AND {10} = {11};";
      
      // Delete statements
      internal const string SQLTruncateStatsTable = "DELETE FROM {0}..{1}";
      internal const string SQLDeleteStatsForLastNDays = "DELETE FROM {0}..{1} WHERE {2} > {3}";
      
      // Repository events table statements
      internal const string SQLSelectFromEventsTable = "SELECT {0}, {1}, {2}, {3} FROM {4} {5} ORDER BY {1}"; // {5} is the where clause
      
      
      internal const string SQLPrimaryKeyStatsCount = "SELECT COUNT(*) FROM {0}..{2} st INNER JOIN {1}..{2}  ast ON ast.dbId = st.dbId and ast.date = st.date and ast.category = st.category";
      internal const string SQLUpdateStatsData = "UPDATE ast SET {3} = ([st].{3} + [ast].{3}),{4} = st.{4} FROM {0}..{2} ast INNER JOIN {1}..{2} st ON (ast.dbId = st.dbId and ast.date = st.date and ast.category = st.category)";
      internal const string SQLDeleteConflictingStatsData = "DELETE st FROM {0}..{2} st INNER JOIN {1}..{2} ast ON (ast.dbId = st.dbId and ast.date = st.date and ast.category = st.category)";
      #endregion
      
      #region Constants
      
      internal const int                     MaxSQLLength = 32767;
      internal static readonly Int16         Int16NullValue = Int16.MinValue;
      internal static readonly int           Int32NullValue = Int32.MinValue;
      internal static readonly string        StringNullValue = "";
      internal static readonly bool          BoolNullValue = false;
      internal static readonly long          LongNullValue = long.MinValue;
      internal static readonly DateTime      DateTimeNullValue = DateTime.MinValue;
      internal static readonly byte          ByteNullValue = byte.MinValue;
      
      // Stats table
      internal const string StatsTable = "Stats";
      internal const string StatsCategoryTable = "StatsCategories";
      internal const int    SvrStatsDbId = -1;
      internal const int    NonExistDbId = 0;
      // Stats table column names
      internal const string ColDBId      = "dbId";
      internal const string ColCategory  = "category";
      internal const string ColDate      = "date";
      internal const string ColCount     = "count";
      internal const string ColName      = "name";
      internal const string ColLastUpdated = "lastUpdated" ;
      
      // database table column names
      internal const string ColDTDbId        = "dbId";
      internal const string ColDTName        = "name";
      internal const string ColDTInstance    = "srvInstance";
      internal const string ColDTSrvId       = "srvId";
      internal const string ColDTIsEnabled   = "isEnabled";
      
      // events table column names
      internal const string ColETEventType  = "eventType";
      internal const string ColETTime       = "startTime";
      internal const string ColETDbName     = "databaseName";
      internal const string ColETIsPriv     = "privilegedUser";

      #endregion
      
      #region Properties
      
      public static string StatsTableColumns
      {
         get { return statsTableColumns; }
      }
      
      #endregion
         
      
      #region Public Methods
      
      #region Connection Management on Repository Server
      
      static public SqlConnection GetConnection()
      {
         Repository rep = new Repository();
         
         if( rep.OpenConnection() )
            return rep.connection;
         else 
            return null;
      }
      
      static public SqlConnection GetConnection( string dbName )
      {
         Repository rep = new Repository();
         
         if( rep.OpenConnection( dbName, true ) )
            return rep.connection;
         else 
            return null;
      }
      
      static public string GetConnectionString()
      {
         return Repository.CreateConnectionString( null );
      }
      
      static public string GetConnectionString( string database )
      {
         return Repository.CreateConnectionString( database );
      }
      
      #endregion
      
      #region Command Executions
      
      static public int ExecuteNonQuery( string database,
                                         string commandText )
      {
         int nRows;
         
         string connectionString = GetConnectionString( database );
                
         try
         {
            using( SqlConnection conn = new SqlConnection( connectionString ) )
            {
               nRows = ExecuteNonQuery( conn, commandText );
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     String.Format( "An error occurred while executing non-query statement in database {0}.", 
                                                    database ),
                                     e,
                                     ErrorLog.Severity.Error,
                                     true);
            throw ;
         }
         
         return nRows;
      }

      static public int ExecuteNonQuery( SqlConnection conn,
                                         string commandText)
      {
         int nRows;

         try
         {
            if( conn.State == ConnectionState.Closed )
               conn.Open();

            using (SqlCommand cmd = GetNewCommand(conn, commandText ))
            {
               nRows = cmd.ExecuteNonQuery();
            }
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "An error occurred while executing non-query statement.",
                                     e,
                                     ErrorLog.Severity.Error,
                                     true);
            throw ;
         }

         return nRows;
      }

      static public int ExecuteNonQuery(string commandText)
      {
         return ExecuteNonQuery((string)null, commandText);
      }
      
      static public int ExecuteNonQuery( SqlConnection conn, SqlTransaction trans, string stmt )
      {
         try
         {
            if (conn.State == ConnectionState.Closed)
               conn.Open();
            
            using ( SqlCommand cmd = GetNewCommand( conn, trans, stmt ) )
            {
               return cmd.ExecuteNonQuery();
            }
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "An error occurred while executing transactional non-query statement.",
                                     e,
                                     ErrorLog.Severity.Error,
                                     true);
            throw ;
         }
      }
      
      // Execute a query and return the result set as a DataTable
      static public DataTable ExecuteDataTable( string database, string query )
      {
         string connString = GetConnectionString( database );
         DataTable table = new DataTable();
         
         try
         {
            using( SqlConnection conn = new SqlConnection( connString ) )
            {
               using (SqlCommand cmd = GetNewCommand(conn, query))
               {
                  SqlDataAdapter da = new SqlDataAdapter( cmd );
                  da.Fill( table );
               }
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     "An error occurred while executing query.",
                                     e,
                                     ErrorLog.Severity.Error,
                                     true );
            throw ;
         }
         
         return table;
      }
            
      static public DataTable ExecuteDataTable( string query )
      {
         return ExecuteDataTable( null, query );
      }
      
      static public int ExecuteNonQueryBatch( string database, ArrayList statements, int offset )
      {
         string connStr = GetConnectionString( database );
         
         using( SqlConnection conn = new SqlConnection( connStr ) )
         {
            return ExecuteNonQueryBatch( conn, null, statements, offset );
         }
      }
      
      static public int ExecuteNonQueryBatch( 
         SqlConnection conn, 
         SqlTransaction trans, 
         ArrayList statements, 
         int offset )
      {
         if( statements == null )
            return 0;
         
         int count = 0;
         bool maxReached = false;
         
         StringBuilder cmdTxt = new StringBuilder();
         
         for( int i = offset; i < statements.Count && !maxReached; i++ )
         {
            string stmt;
            
            stmt = (string)statements[i];
            if( cmdTxt.Length + stmt.Length > MaxSQLLength )
            {
               maxReached = true;
               continue;
            }
            cmdTxt.Append( stmt );
            count++;
            //maxReached = true;
         }
         
         if( count == 0 )
            return 0;
         
         if( cmdTxt.Length == 0 )
            return count;
         
         try
         {
            ExecuteNonQuery( conn,
                             trans,
                             cmdTxt.ToString() );
            return count;
         }
         catch( SqlException se )
         {
            SQLErrorSeverity sev = GetSeverityLevel( se.Class );
            if( sev == SQLErrorSeverity.SystemError )
               return 0;
            else 
               return count;
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     e,
                                     true );
            return count; // Keep the update going if possible
         }
      }
      
      static public Object ExecuteScalar( string database, string query )
      {
         string connString = GetConnectionString( database );
         
         try
         {
            using( SqlConnection conn = new SqlConnection( connString ) )
            {
               conn.Open();
               using (SqlCommand cmd = GetNewCommand(conn, query))
               {
                  return cmd.ExecuteScalar();
               }
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "An error occurred while executing scalar query.",
                                     e,
                                     ErrorLog.Severity.Error,
                                     true);
            throw ;
         }
      }
         
      static public int ExecuteInt( string database, string query )
      {
         return (int)ExecuteScalar( database, query );
      }
      
      static public DateTime ExecuteDateTime( string database, string query )
      {
         return (DateTime)ExecuteScalar( database, query );
      }
      
      
      static public SqlDataReader ExecuteReader( SqlConnection conn, string query )
      {
         try
         {
            if( conn.State == ConnectionState.Closed )
               conn.Open();

            using (SqlCommand cmd = GetNewCommand(conn, query))
            {
               return cmd.ExecuteReader();
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                    "An error occurred while executing data reader query.",
                                    e,
                                    ErrorLog.Severity.Error,
                                    true);
            throw;
         }
      }
      
      static public SqlCommand GetNewCommand( SqlConnection conn, string stmt )
      {
         return GetNewCommand( conn, null, stmt );
      }
      
      static public SqlCommand GetNewCommand( SqlConnection conn, SqlTransaction trans, string stmt )
      {
         if( conn.State == ConnectionState.Closed )
            conn.Open();
            
         SqlCommand cmd;
         if( trans != null )
            cmd = new SqlCommand( stmt, conn, trans );
         else  
            cmd = new SqlCommand( stmt, conn );
            
         cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
         
         return cmd;
      }
      
      #endregion
      
      #region Instance methods

      //
      //
      //
      static public bool CreateStatsTable(string database )
      {
         bool success = true;
         try
         {
            ExecuteNonQuery( database, GetCreateStatsTableStmt(null) );
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                    "An error occurred createing stats table in database "+database,
                                    e,
                                    ErrorLog.Severity.Error,
                                    true);
            success = false;
         }
         
         return success;
      }

      //
      //
      //
      static public bool PopulateStatsCategortiesTable( )
      {
         if( TableExists( null, StatsCategoryTable ) )
            return true;

         if (!CreateStatsCategoriesTable())
            return false;
            
         for( int i = 0; i < (int)StatsCategory.MaxValue; i++ )
         {
            string stmt;
            stmt = CreateInsertStatsCategoryRowStmt(i, (StatsCategory)i);
            ExecuteNonQuery( stmt );
         }
         return true;
         
      }
      //
      //
      //
      static public bool CreateStatsTable( SqlConnection conn, string database)
      {
         bool success = true;
         try
         {
            ExecuteNonQuery( conn, GetCreateStatsTableStmt(database));
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                    "An error occurred createing stats table in database " + database,
                                    e,
                                    ErrorLog.Severity.Error,
                                    true);
            success = false;
         }

         return success;
      }
      
      static public bool CreateStatsCategoriesTable()
      {
         try
         {
            string stmt = String.Format( SQLCreateStatsCategoriesTable,
                                         CoreConstants.RepositoryDatabase,
                                         StatsCategoryTable,
                                         ColCategory, ColName );
            ExecuteNonQuery(stmt);
            return true;
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                    "An error occurred createing StatsCategories table.",
                                    e,
                                    ErrorLog.Severity.Error,
                                    true);
            return false;
         }
      }
         
         
      //
      //
      //
      static public string [] GetAuditedInstances()
      {
         string connString = Repository.CreateConnectionString( null );
         
         return RulesDal.SelectAuditedInstances( connString );
         
      }

      //
      //
      //
      static public string[] GetRegisteredInstances()
      {
         string connString = Repository.CreateConnectionString(null);

         return RulesDal.SelectRegisteredInstances( connString );
      }

      //
      //
      //
      static public DataTable GetInstanceInfo()
      {
         return ExecuteDataTable(CreateGetInstanceRecordsStmt());
      }

      //
      //
      //
      static public DataRow GetInstanceInfo(string name)
      {
         string query = CreateGetInstanceRecordsStmt( name );
         DataTable table = ExecuteDataTable(query);
         
         if( table.Rows.Count > 0 )
            return table.Rows[0];
         else 
            return null;
      }

      //
      //
      //
      static public DataTable GetInstanceStatsForLastNDays(string database, int lastNDays)
      {
         string query = CreateStatsQuery(database,CreateWhereClauseForNDays(lastNDays, SvrStatsDbId));                     
         return ExecuteDataTable( database, query );
      }

      //
      //
      //
      static public DataTable GetInstanceStatsInRange(string database, DateTime startDate, DateTime endDate)
      {
         string query = CreateStatsQuery(database, CreateWhereClause(startDate, endDate, SvrStatsDbId));
         return ExecuteDataTable(database, query);
      }

      //
      //
      //
      static public DataTable GetInstanceStats(string database, DateTime day)
      {
         string query = CreateStatsQuery(database, CreateWhereClause(day, SvrStatsDbId));
         return ExecuteDataTable(database, query);
      }

      //
      //
      //
      static public DataTable GetDbLists()
      {
         return GetDbList(null);
      }

      //
      //
      //
      static public DataTable GetDbList(string instance)
      {
         StringBuilder query = new StringBuilder();
         
         query.AppendFormat( SQLGetDbIdsBase, 
                             ColDTInstance,
                             ColDTSrvId,
                             ColDTName, 
                             ColDTDbId, 
                             CoreConstants.RepositoryDatabase, 
                             CoreConstants.RepositoryDatabaseTable );
                              
         if( instance != null && instance.Length > 0 )
            query.AppendFormat( " WHERE {0} = {1}", ColDTInstance, SQLHelpers.CreateSafeString(instance) );
            
         return ExecuteDataTable( null, query.ToString() );
      }

      //
      //
      //
      static public int GetDbId(string instance, string database)
      {
            
         string query = CreateGetDbIdStmt( instance, database );
         object obj = ExecuteScalar( null, query );
         if( obj == null || obj is DBNull )
         {
            // Check for 2005 system database name
            if (database == CoreConstants.SQL2005SystemDatabase)
               return CoreConstants.SQL2005SystemDatabaseId;
            return NonExistDbId;
         }
         else 
            return (int)obj;
      }

      //
      //
      //
      static public bool StatsTableExists(string database)
      {
         return TableExists( database, StatsTable );
      }
      
      static public bool TableExists( string database, string table )
      {
         string query = String.Format( SQLDoesTableExist, table );
         
         if( ExecuteInt( database, query ) > 0 )
            return true;
         else 
            return false;
      }
      
      //
      //
      //
      static public bool RecordExists( string dbName, int dbId, StatsCategory cat, DateTime date )
      {
         string query = CreateRecordExistsStmt(dbName, dbId, cat, date);
         if( ExecuteInt( dbName, query ) > 0 )
            return true;
         else 
            return false;
      }
      
      //
      //
      //
      static public int GetStatsCountFromDb( string dbName, int dbId, StatsCategory cat, DateTime date )
      {
         string query = CreateGetStatsCountValueStmt(dbName, dbId, cat, date);
         object value = ExecuteScalar( dbName, query );
         if( value == null || value is DBNull )
            return -1;
         else 
            return (int)value;
      }
      
      static public void UpdateStatsRecord( string dbName, int dbId, StatsCategory cat, DateTime date, int count )
      {
         date = CreateStatsIntervalTime( date );
         int current = GetStatsCountFromDb( dbName, dbId, cat, date );
         string stmt;
         if( current >= 0 )
         {
            count += current;
            stmt = CreateUpdateStatsRecordStmt( dbName, dbId, cat, date, count );
         }
         else
         {
            stmt = CreateInsertStatsRecordStmt( dbName, dbId, cat, date, count );
         }
         ExecuteNonQuery( dbName, stmt );
            
      }
      
      public static bool DeleteStats( string eventDb, int lastNDays )
      {
         bool success = true;
         
         try
         {
            ExecuteNonQuery(eventDb, CreateDeleteStatsStmt( eventDb, lastNDays, false));
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                    "An error occurred deleting stats table in database " + eventDb,
                                    e,
                                    ErrorLog.Severity.Error,
                                    true);
            success = false;
         }
         
         return success;
      }
      
      public static void CalculateStats( InstanceStats iStats, string database )
      {
         string connStr = GetConnectionString(database);
         string query = CreateQueryEventsTableStmt(-1);  // Everything
         InstanceStats tmpStats = iStats.GetNewInstanceStats();
         DateTime currentTime = DateTime.UtcNow;

         try
         {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
               SqlDataReader reader = ExecuteReader(conn, query);
               if (reader.HasRows)
               {
                  int count = 0;
                  while (reader.Read())
                  {
                     TraceEventType type;
                     DateTime eventTime;
                     string databaseName;
                     bool isPriv;

                     GetEventStatsData(reader, out type, out databaseName, out eventTime, out isPriv);

                     if ((eventTime.Year != currentTime.Year ||
                         eventTime.Month != currentTime.Month ||
                         eventTime.Day != currentTime.Day)) // Update if event day is changed
                     {
                        if (count > 0)
                           iStats.Update(tmpStats);
                        currentTime = eventTime;
                        count = 0;
                        tmpStats.Reset();
                        iStats.Reset();
                     }

                     if (isPriv)
                        tmpStats.Add(3, type, "", eventTime, 1);

                     if (Stats.IsServerLevelEvent(type))
                        tmpStats.Add(1, type, "", eventTime, 1);
                     else
                        tmpStats.Add(2, type, databaseName, eventTime, 1);

                     count++;
                  }

                  if (count > 0)
                     iStats.Update(tmpStats);
               }
            }
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                    "An error occurred calculating stats for " + iStats.Instance,
                                    e,
                                    ErrorLog.Severity.Error,
                                    true);
            throw;
         }
      }

      public static void CalculateStats( InstanceStats iStats )
      {
         string connStr = GetConnectionString(iStats.EventDatabase);
         string query = CreateQueryEventsTableStmt( -1 );  // Everything
         InstanceStats tmpStats = iStats.GetNewInstanceStats();
         DateTime currentTime = DateTime.UtcNow;
         
         try
         {
            using( SqlConnection conn = new SqlConnection( connStr ) )
            {
               SqlDataReader reader = ExecuteReader(conn, query);
               if( reader.HasRows )
               {
                  int count = 0;
                  while( reader.Read() )
                  {
                     TraceEventType type;
                     DateTime eventTime;
                     string databaseName;
                     bool isPriv;

                     GetEventStatsData(reader, out type, out databaseName, out eventTime, out isPriv);
                     
                     if ((eventTime.Year != currentTime.Year ||
                         eventTime.Month != currentTime.Month ||
                         eventTime.Day != currentTime.Day)) // Update if event day is changed
                     {
                        if( count > 0 )
                           iStats.Update(tmpStats);
                        currentTime = eventTime;
                        count = 0;
                        tmpStats.Reset();
                        iStats.Reset();
                     }
                     
                     if( isPriv )
                        tmpStats.Add( 3, type, "", eventTime, 1 );
                        
                     if( Stats.IsServerLevelEvent( type ) )
                        tmpStats.Add( 1, type, "", eventTime, 1 );
                     else 
                        tmpStats.Add( 2, type, databaseName, eventTime, 1 );
                     
                     count++;
                  }
                  
                  if( count > 0 )
                     iStats.Update(tmpStats);
               }
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                    "An error occurred calculating stats for " + iStats.Instance,
                                    e,
                                    ErrorLog.Severity.Error,
                                    true);
            throw;
         }
      }

      
      #endregion
      
      #region DataRow handling methods


      internal static int GetIntValue(DataRow row, string name)
      {
         return GetInt32Value( row, name );
      }

      internal static Int32 GetInt32Value(DataRow row, string name)
      {
         return row.IsNull(name) ? Int32NullValue : (Int32) row[name];
      }
      
      internal static Int16 GetInt16Value( DataRow row, string name )
      {
         return row.IsNull(name) ? Int16NullValue : (Int16)row[name] ;
      }

      internal static bool GetByteAsBool(DataRow row, string name)
      {
         return row.IsNull(name) ? false : (byte)row[name] > 0;
      }

      internal static byte GetByteValue(DataRow row, string name)
      {
         return row.IsNull(name) ? ByteNullValue : (byte)row[name];
      }
      
      internal static string GetStringValue( DataRow row, string name )
      {
         return row.IsNull(name) ? StringNullValue : (string) row[name];
      }
      
      internal static DateTime GetDateTimeValue( DataRow row, string name )
      {
         return row.IsNull(name) ? DateTimeNullValue : (DateTime)row[name];
      }
      
      internal static void GetEventStatsData( SqlDataReader reader, 
                                              out TraceEventType type, 
                                              out string dbName, 
                                              out DateTime time, 
                                              out bool isPriv )
      {
         type = (TraceEventType)reader.GetInt32( 0 ); // event type
         time = (DateTime)reader.GetSqlDateTime(1); // startTime
         dbName = reader.GetString(2);
         isPriv = reader.GetInt32( 3 ) == 1;
      }
      
      #endregion
      
      #region SQL statement building methods
      
      static string CreateGetInstanceRecordsStmt()
      {
         return CreateGetInstanceRecordsStmt( null );
      }
      
      static string CreateInsertStatsCategoryRowStmt( int i, StatsCategory cat )
      {
         return String.Format( SQLInsertStatsCategoryRow,
                               SQLHelpers.CreateSafeDatabaseName(CoreConstants.RepositoryDatabase),
                               StatsCategoryTable,
                               i,
                               cat );
      }
      
      static string CreateGetInstanceRecordsStmt( string name )
      {
         StringBuilder query = new StringBuilder();
         
         query.AppendFormat( SQLGetInstanceRecords,
                             InstanceStats.ColSrvId,
                             InstanceStats.ColInstance,
                             InstanceStats.ColEventDb,
                             InstanceStats.ColIsAudited,
                             SQLHelpers.CreateSafeDatabaseName(CoreConstants.RepositoryDatabase),
                             CoreConstants.RepositoryServerTable);
         if( name != null && name.Length > 0 )
            query.AppendFormat( " WHERE {0} = {1}", InstanceStats.ColInstance, SQLHelpers.CreateSafeString(name));
            
         return query.ToString();
      }
      
      static string GetCreateStatsTableStmt( string database )
      {
         string dbName;
         if (database == null || database.Length == 0)
            dbName = "";
         else 
            dbName = String.Format( "{0}..", SQLHelpers.CreateSafeDatabaseName(database));
         return String.Format( SQLCreateStatsTable,
                               dbName,
                               StatsTable,
                               ColDBId,
                               ColDate,
                               ColCategory,
                               ColCount,
                               ColLastUpdated);
      }
      
      static string CreateStatsQuery( string database, string whereClause )
      {
         return String.Format(SQLGetInstanceStatsBASE,
                              ColDBId,
                              ColDate,
                              ColCategory,
                              ColCount,
                              SQLHelpers.CreateSafeDatabaseName(database),
                              CoreConstants.RepositoryStatsTable,
                              whereClause);
      }
      
      // Build where clause for a single day
      static string CreateWhereClause( DateTime day, int dbId )
      {
         return CreateWhereClause( day, DateTime.MinValue, dbId );
      }
      
      // Create where clause for the last n days
      static string CreateWhereClauseForNDays(int nDays, int dbId)
      {
         DateTime earliestDay = DateTime.UtcNow.AddDays( 1-nDays );
         return CreateWhereClause( earliestDay, DateTime.UtcNow, dbId );
      }
      
      // Create where clause for a range
      static string CreateWhereClause(DateTime startDate, DateTime endDate, int dbId)
      {
         DateTime start = CreateStatsIntervalTime(startDate);
         DateTime end;
         
         if( endDate == DateTime.MinValue )
            end = CreateStatsIntervalTime( startDate.AddDays( -1 ) );
         else
            end = CreateStatsIntervalTime(endDate);
         StringBuilder query = new StringBuilder( "Where " );
         query.AppendFormat( "{0} >= {1} and {0} <= {2}",
                             ColDate,
                             SQLHelpers.CreateSafeDateTimeString( start ),
                             SQLHelpers.CreateSafeDateTimeString( end ) );
                                
         query.Append( " AND " );
         
         if( dbId > 0 ) // one database
            query.AppendFormat( "{0} = {1}", ColDBId, dbId );
         else if( dbId == 0 ) // all database stats records
            query.AppendFormat( "{0} <> {1}", ColDBId, SvrStatsDbId );
         else 
            query.AppendFormat( "{0} = {1}", ColDBId, SvrStatsDbId );
            
         return query.ToString();
      }
      
      public static string CreateRecordExistsStmt( string dbName, int dbId, StatsCategory cat, DateTime date )
      {
         return String.Format( SQLStatsRecordExists, 
                               SQLHelpers.CreateSafeDatabaseName( dbName ),
                               StatsTable,
                               ColDBId,
                               dbId,
                               ColCategory,
                               (int)cat,
                               ColDate,
                               SQLHelpers.CreateSafeDateTimeString( date ));
      }

      public static string CreateGetStatsCountValueStmt(string dbName, int dbId, StatsCategory cat, DateTime date)
      {
         return String.Format(SQLStatsRecordCountValue,
                               ColCount,
                               SQLHelpers.CreateSafeDatabaseName(dbName),
                               StatsTable,
                               ColDBId,
                               dbId,
                               ColCategory,
                               (int)cat,
                               ColDate,
                               SQLHelpers.CreateSafeDateTimeString(date));
      }
      
      public static string CreateInsertStatsRecordStmt( string dbName, int dbId, StatsCategory cat, DateTime date, int count )
      {
         return String.Format( SQLInsertStatsRecord,
                               SQLHelpers.CreateSafeDatabaseName(dbName),
                               CoreConstants.RepositoryStatsTable,
                               ColDBId,
                               ColDate,
                               ColCategory,
                               ColCount,
                               ColLastUpdated,
                               dbId,
                               SQLHelpers.CreateSafeDateTimeString(date),
                               (int)cat,
                               count,
                               SQLHelpers.CreateSafeDateTime(DateTime.UtcNow));
      }
      
      public static string CreateUpdateStatsRecordStmt( string dbName, int dbId, StatsCategory cat, DateTime date, int count )
      {
         return String.Format( SQLUpdateStatsRecord,
                               SQLHelpers.CreateSafeDatabaseName(dbName),
                               CoreConstants.RepositoryStatsTable,
                               ColCount, count,
                               ColLastUpdated, SQLHelpers.CreateSafeDateTime(DateTime.UtcNow),
                               ColDBId, dbId,
                               ColDate, SQLHelpers.CreateSafeDateTimeString( date ),
                               ColCategory, (int)cat );
      }
      
      public static string CreateGetDbIdStmt( string instance, string database )
      {
         return String.Format( SQLGetDbId, 
                               ColDTDbId, 
                               SQLHelpers.CreateSafeDatabaseName(CoreConstants.RepositoryDatabase), 
                               CoreConstants.RepositoryDatabaseTable,
                               ColDTInstance, 
                               SQLHelpers.CreateSafeString(instance), 
                               ColDTName, 
                               SQLHelpers.CreateSafeString(database) );
      }
             
      //
      //  CreateDeleteStatsStmt: Delete stats records in the Stats table.  If includeProcessingData is false,
      //  EventReceived, EventProcessed and EventFiltered recrods will not be deleted.
      //          
      public static string CreateDeleteStatsStmt( string database, int days, bool includeProcessingData )
      {
         StringBuilder cmdTxt = new StringBuilder( );
         
         if( days <= 0 )
         {
            cmdTxt.AppendFormat( SQLTruncateStatsTable,
                                 SQLHelpers.CreateSafeDatabaseName(database),
                                 CoreConstants.RepositoryStatsTable );
            if( !includeProcessingData )
            {
               cmdTxt.AppendFormat( " WHERE {0} <> {1} AND {0} <> {2} AND {0} <> {3}",
                                    ColCategory,
                                    (int) StatsCategory.EventReceived,
                                    (int) StatsCategory.EventProcessed,
                                    (int) StatsCategory.EventFiltered );
            }
         }
         else
         {
            DateTime endTime = CreateStatsIntervalTime( DateTime.UtcNow.AddDays( -days ) );
            cmdTxt.AppendFormat( SQLDeleteStatsForLastNDays,
                                    SQLHelpers.CreateSafeDatabaseName(database), 
                                    CoreConstants.RepositoryStatsTable,
                                    ColDate,
                                    SQLHelpers.CreateSafeDateTimeString( endTime ) );
            if (!includeProcessingData)
            {
               cmdTxt.AppendFormat(" AND {0} <> {1} AND {0} <> {2} AND {0} <> {3}",
                                    ColCategory,
                                    (int)StatsCategory.EventReceived,
                                    (int)StatsCategory.EventProcessed,
                                    (int)StatsCategory.EventFiltered);
            }
         }
         
         return cmdTxt.ToString();
      }
      
      public static string CreateQueryEventsTableStmt( int days )
      {
         string whereClause = "";
         if( days > 0 )
         {
            DateTime endTime = CreateStatsIntervalTime( DateTime.UtcNow.AddDays( -days ) );
            whereClause = String.Format( " WHERE {0} > {1} ", 
                                         ColDate,
                                         SQLHelpers.CreateSafeDateTimeString( endTime ) );
         }
         
         return String.Format( SQLSelectFromEventsTable,
                               ColETEventType,
                               ColETTime,
                               ColETDbName,
                               ColETIsPriv,
                               CoreConstants.RepositoryEventsTable,
                               whereClause );
      }
      
      public static string CreateArchiveStatsTableStmt( string sourceDb,
                                                        string destinationDb,
                                                        DateTime startTime,
                                                        DateTime endTime )
      {
         StringBuilder stmt = new StringBuilder();
         string source = SQLHelpers.CreateSafeDatabaseName( sourceDb );
         string destination = SQLHelpers.CreateSafeDatabaseName( destinationDb );
         string start = SQLHelpers.CreateSafeDateTimeString(startTime);
         string end = SQLHelpers.CreateSafeDateTimeString( endTime );
         stmt.AppendFormat( SQLCopyStatsRecords, 
                            destination, 
                            StatsTable,
                            statsTableColumns,
                            source,
                            ColDate,
                            start,
                            end );
         stmt.Append(";");
         stmt.AppendFormat( SQLDeleteStatsInRange, 
                            source,
                            StatsTable,
                            ColDate,
                            start,
                            end );
         return stmt.ToString();
      }
      
      public static string CreateDeleteOldStatsStmt( string database, DateTime date )
      {
         return String.Format( SQLDeleteOldStats, 
                               SQLHelpers.CreateSafeDatabaseName( database ),
                               StatsTable,
                               ColDate,
                               SQLHelpers.CreateSafeDateTimeString( date ) );
      }

      public static string CreateEarliestStatsTimeStmt( string database )
      {
         return String.Format( SQLEarliestStatsTime, ColDate, SQLHelpers.CreateSafeDatabaseName(database), StatsTable );
      }
      #endregion
      
      #region Utilities
      
      public static DateTime CreateStatsIntervalTime( DateTime time )
      {
         // Create 15 minute interval
         return new DateTime( time.Year, time.Month, time.Day, time.Hour, (time.Minute /15) * 15, 0 );
      }
      
      public static DateTime GetEarliestStatsTime( string dbName )
      {
         try
         {
            return ExecuteDateTime(dbName, CreateEarliestStatsTimeStmt( dbName) );
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug, e.Message, e, true );
            return DateTime.UtcNow;
         }
      }
      
      #endregion
      
      #region Error Handlings
      
      static public SQLErrorSeverity GetSeverityLevel( byte sqlSeverity )
      {
         if (sqlSeverity < 10) return SQLErrorSeverity.Info;
         else if (sqlSeverity == 10) return SQLErrorSeverity.Status;
         else if (sqlSeverity > 10 && sqlSeverity <= 16) return SQLErrorSeverity.UserError;
         else if (sqlSeverity > 17 && sqlSeverity <= 19) return SQLErrorSeverity.AdminError;
         else  return SQLErrorSeverity.SystemError;
      }

      #endregion
            
      #endregion // public methods
      
      public static string CheckPrimaryKeyConflict(string sourceDb,
                                                        string destinationDb)
      {
          StringBuilder stmt = new StringBuilder();
          string source = SQLHelpers.CreateSafeDatabaseName(sourceDb);
          string destination = SQLHelpers.CreateSafeDatabaseName(destinationDb);
          stmt.AppendFormat(SQLPrimaryKeyStatsCount,
                            source,
                            destination,
                            StatsTable);
          stmt.Append(";");
          return stmt.ToString();
      }
      
      public static string UpdateStatsPrimaryKeyData(string sourceDb,
                                                        string destinationDb)
      {
          StringBuilder stmt = new StringBuilder();
          string source = SQLHelpers.CreateSafeDatabaseName(sourceDb);
          string destination = SQLHelpers.CreateSafeDatabaseName(destinationDb);
          stmt.AppendFormat(SQLUpdateStatsData,
                            destination,
                            source,
                            StatsTable,                            
                            ColCount,
                            ColLastUpdated);
          stmt.Append(";");
          stmt.AppendFormat(SQLDeleteConflictingStatsData,
                           source,
                           destination,
                           StatsTable);
          return stmt.ToString();
   }
   }   
}
