using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.Data.SqlClient;

using Idera.SQLcompliance.Core.Event;

namespace Idera.SQLcompliance.Core.Stats
{
   public class InstanceStats : IStats
   {
      #region Private data members

      string name;
      string eventDb;
      bool   audited;
      int    id;
      Dictionary<DateTime, InstanceStatsRecord> statsCache = new Dictionary<DateTime, InstanceStatsRecord>();
      Hashtable dbNameList;
      Hashtable dbIdMap;
      object syncObject = new object();
      DateTime purgeTime = DateTime.Now.AddHours( 2 );

      #endregion
      
      #region Constants
      
      // Columns used to retrieve instance info from Servers table
      internal const string ColIsAudited = "isAuditedServer";
      internal const string ColSrvId = "srvId";
      internal const string ColInstance = "instance";
      internal const string ColEventDb = "eventDatabase";
      
      #endregion
      
      #region Constructors
      
      internal InstanceStats( string instanceName)
      {
         Init(instanceName);
      }
      
      internal InstanceStats( DataRow row )
      {
         if( !Init( row ))
         {
            Init( name );
         }
         
      }
      
      private InstanceStats( string instance, 
                             int instanceId, 
                             string eventDatabase, 
                             bool isAudited, 
                             Hashtable dbList, 
                             Hashtable idMap )
      {
         name = instance;
         id = instanceId;
         eventDb = eventDatabase;
         audited = isAudited;
         dbNameList = dbList;
         dbIdMap = idMap;
      }
         
         
      
      #endregion

      #region Properties

      
      public string Instance
      {
         get { return name; }
         set { name = value; }
      }
      
      public string EventDatabase
      {
         get { return eventDb; }
         set { eventDb = value; }
      }
      
      public int Id
      {  
         get { return id; }
         set { id = value; }
      }
      
      public bool IsAudited
      {
         get { return audited; }
         set { audited = value; }
      }
      
      #endregion

      #region Public Methods
      
      public InstanceStatsRecord [] GetRecords()
      {
         InstanceStatsRecord [] tmpArray= new InstanceStatsRecord [statsCache.Count];
         
         statsCache.Values.CopyTo( tmpArray, 0);
         
         return tmpArray;
      }
      
      public InstanceStatsRecord GetRecord( DateTime date )
      {
         date = GetStatsInterval(date);
         
         InstanceStatsRecord record;
         
         if( statsCache.ContainsKey( date ) )
            record = statsCache[date];
         else
         {
            record = new InstanceStatsRecord( name, date );
            statsCache.Add( date, record );
         }
         
         return record;
      }

      #region Update Stats records in the database
      
      public bool Update()
      {
         bool success = true;
         
         if( !IsValidInstance() )
            return success;
         
         InstanceStatsRecord [] records = GetUpdatedRecords();
         
         if( records == null )
            return true;
         
         try
         {
            ArrayList updateStmts = new ArrayList();
            
            for( int i = 0; i < records.Length; i++ )
            {
               updateStmts.AddRange( records[i].GetUpdateStatements( this ));
            }
         
            if( ExecuteUpdates( updateStmts ) )
            {
               for( int i = 0; i < records.Length; i++ )
               {
                  if( !records[i].SetUpdated() )
                     success = false;
                  
               }
            }
            else
            {
               for (int i = 0; i < records.Length; i++)
               {
                  records[i].Revert();
               }
               success = false;
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                     "An error occurred updating stats records.",
                                     e,
                                     ErrorLog.Severity.Warning,
                                     true );
            throw;
         }
                                    
         
            

         return success;
      }
      
      //
      //
      //
      public bool Update( InstanceStats newStats )
      {
         string connStr = StatsDAL.GetConnectionString( eventDb );
         using( SqlConnection conn = new SqlConnection( connStr ) )
         {
            return Update( conn, null, newStats );
         }
      }

      //
      // Update the stats both in the cache and in the database using newStats
      //
      public bool Update( SqlConnection conn, SqlTransaction trans, InstanceStats newStats )
      {
         bool success = true;

         if (!IsValidInstance())
            return success;
            
         lock (syncObject)
         {
            try
            {
               // Update the cache
               InstanceStatsRecord[] updatedRecords = Add(newStats);
               
               // Start updating the stats table
               ArrayList updateStmts = new ArrayList();

               for (int i = 0; i < updatedRecords.Length; i++)
               {
                  updateStmts.AddRange(updatedRecords[i].GetUpdateStatements(this));
               }

               if (ExecuteUpdates(conn, trans, updateStmts))
               {
                  for (int i = 0; i < updatedRecords.Length; i++)
                  {
                     if (!updatedRecords[i].SetUpdated())
                        success = false;
                  }
               }
               else
               {
                  for (int i = 0; i < updatedRecords.Length; i++)
                  {
                     updatedRecords[i].Revert();
                  }
                  success = false;
               }
               
               if( success )
                  newStats.Reset();
               
               try
               {
                  Purge();
               }
               catch( Exception e )
               {
                  ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                           "An error occurred purging cached stats records.",
                                           e,
                                           ErrorLog.Severity.Warning,
                                           true);
               }
            }
            catch (Exception e)
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                        "An error occurred updating stats records.",
                                        e,
                                        ErrorLog.Severity.Warning,
                                        true);
               throw;
            }      
         }
         
         return success;
      }
      
      
      
      #endregion
      
      
      #region Add Methods
      
      //
      // Add new stats records to the existing ones
      public InstanceStatsRecord [] Add( InstanceStats iStats )
      {
         
         InstanceStatsRecord [] records = iStats.GetRecords();
         ArrayList updatedRecords = new ArrayList();
         
         if( records != null )
         {
            foreach (InstanceStatsRecord record in records)
            {
               InstanceStatsRecord updatedRec;
               updatedRec = Add( record );
               if( updatedRec != null )
                  updatedRecords.Add( updatedRec );
            }
         }



         return (InstanceStatsRecord [])updatedRecords.ToArray( typeof(InstanceStatsRecord) );
      }
      
      public InstanceStatsRecord Add( InstanceStatsRecord newRecord )
      {
         if (newRecord == null)
            return null;

         InstanceStatsRecord record = GetRecord(newRecord.Date);
         try
         {
            record.Add( newRecord );
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                     "An error occurred adding new stats data.",
                                     e,
                                     ErrorLog.Severity.Warning,
                                     true );
         }
         
         return record;

      }
      
      public bool Add( int level, TraceEventType type, string database, DateTime time, int count )
      {
         TraceLevel tLevel = (TraceLevel)level;
         
         StatsCategory cat = Stats.GetStatsCategory(type);
         InstanceStatsRecord record = GetRecord(time);
         
         switch( tLevel )
         {
            case TraceLevel.Server:
               return record.Add( "", cat, count, true );
               
            case TraceLevel.Database:
               return record.Add( database, cat, count, true );
               
            case TraceLevel.User:
               record.Add( "", StatsCategory.PrivUserEvents, count , true );
               if( TraceConstants.IsDatabaseLevelEvent( type ) )
                  return record.Add( database, cat, count, true );
               else 
                  return record.Add( "", cat, count, true );
            default:
               return record.Add(database, cat, count, true);
         }
      }
      
      public bool Add( string database, StatsCategory cat, DateTime time, int count )
      {
         InstanceStatsRecord record = GetRecord(time);
         return record.Add( database, cat, count, true );
      }
      
      public bool Add( StatsCategory cat, DateTime time, int count )
      {
         return Add( "", cat, time, count );
      }
         
         
      
      #endregion
      
      //
      // Clear all stats data
      public void Reset()
      {
         statsCache.Clear();
         purgeTime = DateTime.Now.AddHours( 2 );
      }
      
      //
      // Purge stats cache
      //
      public void Purge()
      {
         if( DateTime.Now < purgeTime )
            return;

         IDictionaryEnumerator enumerator = statsCache.GetEnumerator();
         ArrayList purgeList = new ArrayList();

         while( enumerator.MoveNext() )
         {
            InstanceStatsRecord record;
            record = (InstanceStatsRecord)enumerator.Value;
            if( record.CanBePurged() )
               purgeList.Add( record.Date );
         }
         for( int i = 0; i < purgeList.Count; i++ )
            statsCache.Remove( (DateTime)purgeList[i] );
      }
      
      
      public InstanceStats GetNewInstanceStats()
      {
         return new InstanceStats( name, id, eventDb, audited, dbNameList, dbIdMap );
      }

      
      public void CreateStatsTable()
      {
         StatsDAL.CreateStatsTable( eventDb );
      }
      
      public int GetDbId( string dbName )
      {
         if( dbIdMap.Contains( dbName ) )
            return (int)dbIdMap[dbName];
         else
         {
            int dbId = StatsDAL.GetDbId( name, dbName );
            if( dbId != StatsDAL.NonExistDbId )
            {
               dbIdMap.Add( dbName, dbId );
               try
               {
                  dbNameList.Add( dbId, dbName );
               }
               catch
               {}
            }
            return dbId;
         }
      }
      
      
      public bool Recalculate()
      {
         bool success = StatsDAL.DeleteStats( eventDb, -1 );
         
         if( !success )
            return success;
         
         
         try
         {
            Reset();
            StatsDAL.CalculateStats( this );
            
         }
         catch( Exception e )
         {
            success = false;
            ErrorLog.Instance.Write( "An error occurred recalculating statistics for " + name,
                                     e,
                                     ErrorLog.Severity.Error );
         }
         
         return success;
      }
      
      public bool Recalculate( string database )
      {
         bool success = StatsDAL.DeleteStats( database, -1 );
         
         if( !success )
            return success;
         
         
         try
         {
            Reset();
            StatsDAL.CalculateStats( this, database );
            
         }
         catch( Exception e )
         {
            success = false;
            ErrorLog.Instance.Write( "An error occurred recalculating statistics for " + name,
                                     e,
                                     ErrorLog.Severity.Error );
         }
         
         return success;
}

            
            

      #endregion

      #region Private Methods
      
      void Init( string instanceName )
      {
         name = instanceName;
         id = -1;
         eventDb = "";
         audited = false;
         dbNameList = new Hashtable();
         dbIdMap = new Hashtable();
      }

      
      bool Init( DataRow row )
      {
         try
         {
            id = StatsDAL.GetIntValue( row, ColSrvId );
            name = StatsDAL.GetStringValue( row, ColInstance );
            eventDb = StatsDAL.GetStringValue( row, ColEventDb );
            audited =  StatsDAL.GetByteAsBool( row, ColIsAudited );
            LoadStats();
            dbNameList = Stats.GetDbList(name);
            dbIdMap = GetDbIdMap();
            return true;
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                     String.Format( "An error occurred initializing InstanceStats for {0}.", name ),
                                     e,
                                     ErrorLog.Severity.Warning,
                                     true );
            return false;
         }
      }

      
      void LoadStats()
      {
         if( !StatsDAL.StatsTableExists( eventDb ))
         {
            StatsDAL.CreateStatsTable(eventDb);
            return;
         }
         
         DataTable table = StatsDAL.GetInstanceStatsForLastNDays( eventDb, Stats.DaysStatsCached );
         
         for( int i = 0; i < table.Rows.Count; i++ )
         {
            AddRecord( table.Rows[i] );
         }
         
      }
      
      // This function is called only when the record is read
      // from the stats table to build stats cache
      void AddRecord( DataRow row )
      {
         StatsRow newRow = new StatsRow( row );
         InstanceStatsRecord record;
         
         if( !statsCache.ContainsKey( newRow.Date ) )
         {
            record = new InstanceStatsRecord( name, newRow.Date );
            statsCache.Add( newRow.Date, record  );
         }
         else 
            record =statsCache[newRow.Date];
            
         string dbName = GetDbName( newRow.DbId );
         
         record.Add( dbName, newRow.Category, newRow.Count, false );
         
      }
      
      Hashtable GetDbIdMap()
      {
         Hashtable map = new Hashtable();
         
         IDictionaryEnumerator enumerator = dbNameList.GetEnumerator();
         
         while( enumerator.MoveNext() )
            map.Add( enumerator.Value, enumerator.Key );
         
         return map;
      }
      
      
      string GetDbName( int dbId )
      {
         if (dbId <= 0)
            return "";
         else if (dbNameList.Contains(dbId))
            return (string)dbNameList[dbId];
         else
         {
            ErrorLog.Instance.Write( "Unknow database ID", ErrorLog.Severity.Warning );
            return null;
         }
      }
      
      
      
      InstanceStatsRecord [] GetUpdatedRecords( )
      {
         ArrayList list = new ArrayList();
         IDictionaryEnumerator enumerator = statsCache.GetEnumerator();
         while( enumerator.MoveNext() )
         {
            if( ((InstanceStatsRecord)enumerator.Value).NeedUpdate )
               list.Add( enumerator.Value );
         }
         
         return (InstanceStatsRecord [])list.ToArray( typeof(InstanceStatsRecord) );
      }

      static bool ExecuteUpdates( ArrayList statements )
      {
         return ExecuteUpdates( null, null, statements );
      }

      static bool ExecuteUpdates( SqlConnection conn, SqlTransaction trans, ArrayList statements )
      {
         int idx = 0;
         int count = 0;
         
         if( statements == null )
            return true;
         
         do
         {
            try
            {
               count = StatsDAL.ExecuteNonQueryBatch( conn,
                                                      trans,
                                                      statements,
                                                      idx );
            }
            catch( Exception e )
            {
               ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                        "An error occurred when batch updating the Stats table.",
                                        e,
                                        true );
            }
            idx += count;
         }while( count > 0 && idx < statements.Count );
         
         return idx >= statements.Count;
         
      }

      static DateTime GetStatsInterval( DateTime time )
      {
         return StatsDAL.CreateStatsIntervalTime(time);
      }
      
      bool IsValidInstance()
      {
         if( id == -1 )
         {
            DataRow row = StatsDAL.GetInstanceInfo(name);
            if( row != null )
               return Init( row );
            else 
               return false;
         }
         else 
            return true;
      }

      #endregion
   }
}
