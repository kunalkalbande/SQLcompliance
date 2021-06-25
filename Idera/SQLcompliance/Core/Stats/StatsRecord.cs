using System;
using System.Collections;

namespace Idera.SQLcompliance.Core.Stats
{
   
   public class StatsRecord
   {
      #region Internal data members

      protected Hashtable statsTable;
      protected DateTime date;
      protected int instanceId = 0;
      protected bool needUpdate;
      protected DateTime lastUpdateTime = DateTime.MinValue;

      #endregion
      
      #region Constructors
      
      public StatsRecord()
      {
         statsTable = new Hashtable();
         date = DateTime.MinValue;
         needUpdate = false;
      }
      
      
      #endregion
      
      #region Properties
      
      public DateTime Date
      {
         get { return date; }
         set { date = value; }
      }
      
      public bool NeedUpdate
      {
         get { return needUpdate; }
         set { needUpdate = value; }
      }
      
      #endregion
           
      
      #region Public Methods
          
      // Returns true if it is a new record in the cache.
      public bool Add( StatsCategory cat, int count, bool newData )
      {
         StatsData data;
         bool success = true;
         
         try
         {
            if( statsTable.Contains( cat ) )
            {
               data = (StatsData)statsTable[cat];
               data.Add( count );
            }
            else
            {
               data = new StatsData( cat, count, newData );
               statsTable.Add( cat, data );
            }
            needUpdate = true;
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                     "An error occurred adding new stats data.",
                                     e.Message,
                                     ErrorLog.Severity.Warning );
            success = false;
         }
         
         return success;
      }
      
      // Add a single StatsData
      public bool Add( StatsData data )
      {
         return Add( data.Category, data.Count, true );
      }
      
      // Add an array of StatsData
      public bool Add( StatsData [] dataArray )
      {
         if( dataArray == null )
            return true;
         
         foreach( StatsData data in dataArray )
         {
            if( !Add( data ) )
            {
               return false;
            }
         }
         
         return true;
      }
      
      internal StatsData [] GetUpdatedData()
      {
         ArrayList list = new ArrayList();
         StatsData data;
         IDictionaryEnumerator enumerator = statsTable.GetEnumerator();
         
         while( enumerator.MoveNext() )
         {
            data = (StatsData)enumerator.Value;
            if( data.NeedUpdate || !data.inDatabase )
               list.Add( data );
         }
            
         return (StatsData [])list.ToArray( typeof(StatsData) );
      }
      
      public virtual bool SetUpdated()
      {
         needUpdate = false;
         StatsData data;
         bool success = true;
         
         try
         {
            IDictionaryEnumerator enumerator = statsTable.GetEnumerator();

            while (enumerator.MoveNext())
            {
               data = (StatsData)enumerator.Value;
               data.SetUpdated();
            }
         }
         catch( Exception e )
         {
            success = false;
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                     "SetUpdated: An error occurred setting stats updated flags.",
                                     e.Message,
                                     ErrorLog.Severity.Warning );
         }
         
         return success;
      }
      
      //
      // Revert: throw away any new data collected
      //
      public bool Revert()
      {

         needUpdate = false;
         StatsData data;
         bool success = true;

         try
         {
            IDictionaryEnumerator enumerator = statsTable.GetEnumerator();

            while (enumerator.MoveNext())
            {
               data = (StatsData)enumerator.Value;
               data.Revert();
            }
         }
         catch (Exception e)
         {
            success = false;
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                     "Revert(): An error occurred reverting to saved stats.",
                                     e.Message,
                                     ErrorLog.Severity.Warning);
         }

         return success;
      }
      
      public bool CanBePurged()
      {
         if( (DateTime.Now - date) > new TimeSpan( CoreConstants.DaysStatsCached, 0, 0, 0) &&
             (DateTime.Now.Ticks - lastUpdateTime.Ticks ) > Stats.KeepInCacheTime )
             return true;
            
         return false;
      }
         
      
      public static string GetUpdateStatement( string db, int dbId, StatsCategory cat, DateTime date, int count, bool isNew )
      {
         if( dbId == StatsDAL.NonExistDbId )
            return "";
            
         if (!isNew )
            return StatsDAL.CreateInsertStatsRecordStmt( db, dbId, cat, date, count );
         else
            return StatsDAL.CreateUpdateStatsRecordStmt( db, dbId, cat, date, count );
      }
      
      #endregion
   }
     
}
