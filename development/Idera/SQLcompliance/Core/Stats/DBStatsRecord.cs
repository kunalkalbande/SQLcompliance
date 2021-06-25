using System;
using System.Collections;

namespace Idera.SQLcompliance.Core.Stats
{
   //-----------------------------------
   // Database level stats record
   //-----------------------------------
   public class DBStatsRecord : StatsRecord
   {
      #region Data Members

      internal string instance;
      internal string name;

      #endregion

      #region Properties

      public string Instance
      {
         get { return instance; }
      }

      public string DbName
      {
         get { return name; }
      }

      #endregion

      #region Constructors

      public DBStatsRecord(string instanceName, string dbName, DateTime inDate)
      {
         instance = instanceName;
         name = dbName;
         date = inDate;
      }

      #endregion

      #region Public methods

      public bool Add(DBStatsRecord newRecord)
      {
         bool success = true;

         if (newRecord == null)
            return success;

         return Add(newRecord.GetUpdatedData());
      }

      public string [] GetUpdateStatements(InstanceStats iStats)
      {
         ArrayList statements = new ArrayList();

         StatsData[] dataArray = GetUpdatedData();

         if (dataArray != null)
         {
            for (int i = 0; i < dataArray.Length; i++)
            {
               if( !dataArray[i].inDatabase )
               {
                  int count = StatsDAL.GetStatsCountFromDb( iStats.EventDatabase,
                                                            iStats.GetDbId(name),
                                                            dataArray[i].Category,
                                                            date );
                  if( count >= 0 )
                  {
                     dataArray[i].Count += count;
                     dataArray[i].inDatabase = true;
                  }
               }
               
               statements.Add(GetUpdateStatement(  iStats.EventDatabase,
                                                   iStats.GetDbId(name),
                                                   dataArray[i].Category,
                                                   date,
                                                   dataArray[i].Count,
                                                   dataArray[i].inDatabase));
            }
         }

         return (string [])statements.ToArray( typeof(string) );

      }

      #endregion



   }
}
