using System;
using System.Collections;

namespace Idera.SQLcompliance.Core.Stats
{
   //-----------------------------------
   // Server level stats record
   //-----------------------------------
   public class InstanceStatsRecord : StatsRecord
   {
      #region Data Members

      static Hashtable validCat;

      string name;
      Hashtable dbRecords = new Hashtable();

      #endregion

      #region Constructors

      static InstanceStatsRecord()
      {
         validCat = new Hashtable();
         validCat.Add(StatsCategory.Alerts, StatsCategory.Alerts);
         validCat.Add(StatsCategory.Logins, StatsCategory.Logins);
         validCat.Add(StatsCategory.Security, StatsCategory.Security);
         validCat.Add(StatsCategory.Admin, StatsCategory.Admin);
         validCat.Add(StatsCategory.DDL, StatsCategory.DDL);
         validCat.Add(StatsCategory.DML, StatsCategory.DML);
         validCat.Add(StatsCategory.Select, StatsCategory.Select);
         validCat.Add(StatsCategory.FailedLogin, StatsCategory.FailedLogin);
         validCat.Add(StatsCategory.UserDefinedEvents, StatsCategory.UserDefinedEvents);
            //start sqlcm 5.6 - 5363
            validCat.Add(StatsCategory.Logout, StatsCategory.Logout);
            //end sqlcm 5.6 - 5363
        }


      public InstanceStatsRecord(string instance, DateTime inDate)
      {
         name = instance;
         date = inDate;
         
      }

      #endregion

      #region Properties

      public string Name
      {
         get { return name; }
      }

      #endregion

      #region Public Methods

      // Returns true if the record is not in the cache.  Otherwise, returns false.
      public bool Add(string dbName, StatsCategory cat, int count, bool newData)
      {
         bool success;

         if (dbName == "" ) //Sever level stats
         {

            return Add(cat, count, newData);
         }

         if (dbName == null)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                     "An error occurred adding unknow database level statistics record: Unknow database.",
                                     ErrorLog.Severity.Warning);
            return false;
         }

         if (dbRecords.Contains(dbName))
         {
            success = ((StatsRecord)dbRecords[dbName]).Add(cat, count, newData);
         }
         else
         {
            DBStatsRecord newRec = AddNewDbRecord(dbName);
            success = newRec.Add(cat, count, newData);
         }

         /* Update server level stats for these categories
         switch( cat )
         {
            case StatsCategory.Delete:
            case StatsCategory.Insert:
            case StatsCategory.Update:
            case StatsCategory.Select:
               Add( cat, count );
               break;
            default:
               break;
         }
         */

         return success;
      }

      //
      // Add
      //
      public bool Add(InstanceStatsRecord newRecord)
      {
         bool success = true;

         if (newRecord == null)
            return true;

         StatsData[] dataArray = newRecord.GetUpdatedData();

         if (dataArray != null)
         {

            foreach (StatsData data in dataArray)
            {
               if (!Add(data.Category, data.Count, true))
               {
                  success = false;
                  break;
               }
            }
         }
         
         lastUpdateTime = DateTime.Now;

         if (!success)
            return success;

         DBStatsRecord[] dbRecs = newRecord.GetDbRecords();

         for (int i = 0; i < dbRecs.Length && success; i++)
         {
            success = Add(dbRecs[i]);
         }

         return success;
      }

      //
      // Add database level stats
      //
      public bool Add(DBStatsRecord newRecord)
      {

         if (newRecord == null)
            return true;

         lastUpdateTime = DateTime.Now;
         DBStatsRecord record = GetDbRecord(newRecord.DbName);
         if (record.Add(newRecord))
         {
            needUpdate = true;
            return true;
         }
         else
            return false;
      }

      //
      // GetDbRecord
      //
      public DBStatsRecord GetDbRecord(string dbName)
      {
         if (dbName == null)
         {
            return null;
         }

         if (dbRecords.Contains(dbName))
            return (DBStatsRecord)dbRecords[dbName];
         else
            return AddNewDbRecord(dbName);
      }

      //
      // GetDbRecords
      //
      public DBStatsRecord[] GetDbRecords()
      {
         ArrayList list = new ArrayList(dbRecords.Count);

         IDictionaryEnumerator enumerator = dbRecords.GetEnumerator();

         while (enumerator.MoveNext())
            list.Add(enumerator.Value);

         return (DBStatsRecord[])list.ToArray(typeof(DBStatsRecord));
      }

      public static bool IsValidCategory(StatsCategory cat)
      {
         return validCat.Contains(cat);
      }

      public string [] GetUpdateStatements(InstanceStats iStats)
      {
         ArrayList statements = new ArrayList();
         StatsData[] dataArray = GetUpdatedData();

         if (dataArray != null)
         {
            for (int i = 0; i < dataArray.Length; i++)
            {
               if (!dataArray[i].inDatabase)
               {
                  int count = StatsDAL.GetStatsCountFromDb(iStats.EventDatabase,
                                                            StatsDAL.SvrStatsDbId,
                                                            dataArray[i].Category,
                                                            date);
                  if (count >= 0)
                  {
                     dataArray[i].Count += count;
                     dataArray[i].inDatabase = true;
                  }
               }

               statements.Add(GetUpdateStatement(iStats.EventDatabase,
                                                   StatsDAL.SvrStatsDbId,
                                                   dataArray[i].Category,
                                                   date,
                                                   dataArray[i].Count,
                                                   dataArray[i].inDatabase));
            }
         }

         DBStatsRecord[] records = GetDbRecords();

         for (int i = 0; i < records.Length; i++)
         {
            if (records[i].NeedUpdate)
            {
               statements.AddRange(records[i].GetUpdateStatements(iStats));
            }
         }

         return (string [])statements.ToArray( typeof(string));
      }
      
      public override bool SetUpdated()
      {
         needUpdate = false;
         bool success = true;
         StatsData data;

         try
         {
            IDictionaryEnumerator enumerator = statsTable.GetEnumerator();

            while (enumerator.MoveNext())
            {
               data = (StatsData)enumerator.Value;
               data.SetUpdated();
            }
            
            
            DBStatsRecord [] dbRecList = GetDbRecords();
            if( dbRecList != null )
            {
               for( int i = 0; i < dbRecList.Length; i++ )
                  dbRecList[i].SetUpdated();
            }
         }
         catch (Exception e)
         {
            success = false;
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                     "SetUpdated: An error occurred setting instance stats record updated flags.",
                                     e.Message,
                                     ErrorLog.Severity.Warning);
         }

         return success;
      }


      #endregion

      #region Private methods

      DBStatsRecord AddNewDbRecord(string dbName)
      {
         DBStatsRecord newRec = new DBStatsRecord(name, dbName, date);
         dbRecords.Add(dbName, newRec);
         return newRec;
      }
      

      #endregion
   }
}
