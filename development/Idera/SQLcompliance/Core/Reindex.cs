using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Win32;
using Idera.SQLcompliance.Core.Collector;

namespace Idera.SQLcompliance.Core
{
   public class Reindex
   {
      //This is just a sanity check.  This should never be needed.
      bool isRunning = false;

      public Reindex()
      {
      }

      public void reindexWorker_DoWork(object sender, DoWorkEventArgs e)
      {
         if (isRunning)
            return;
         isRunning = true;

         List<string> eventDBs = new List<string>();
         Repository repo = new Repository();
         
         try
         {
            repo.OpenConnection(false);

            //Get the schedule
            SQLcomplianceConfiguration config = new SQLcomplianceConfiguration();
            config.Read(repo.connection);

            //The schedule has not been set
            if (config.IndexDuration == TimeSpan.Zero)
            {
               isRunning = false;
               return;
            }

            //According to the current time, it is not time to reindex.
            if ((DateTime.Now.TimeOfDay < config.IndexStartTime.TimeOfDay) ||
                (DateTime.Now.TimeOfDay > (config.IndexStartTime.TimeOfDay + config.IndexDuration)))
            {
               isRunning = false;
               return;
            }

            //Get the list of Event databases.
            eventDBs = GetEventDatabases(repo.connection);

            foreach (string eventDatabase in eventDBs)
            {
               try
               {
                  EventDatabase.UpdateIndexes(repo.connection, eventDatabase);

                  //we are no longer in the schedule window. so quit and do not change the registry flag.
                  if ((DateTime.Now.TimeOfDay < config.IndexStartTime.TimeOfDay) ||
                      (DateTime.Now.TimeOfDay > (config.IndexStartTime.TimeOfDay + config.IndexDuration)))
                  {
                     isRunning = false;
                     return;
                  }

               }
               catch (Exception ex)
               {
                  ErrorLog.Instance.Write(ErrorLog.Level.Default, String.Format("Unable to update the indexes on {0}", eventDatabase), ex.ToString(), ErrorLog.Severity.Warning);
               }
            }
            CollectionServer.Instance.SetReindexFlag(false);
            isRunning = false;
         }
         finally
         {
            repo.CloseConnection();
         }
      }
      
      private List<string> GetEventDatabases(SqlConnection conn)
      {
         List<string> eventDBs = new List<string>();

         SystemDatabaseRecord[] records ;

         records = SystemDatabaseRecord.Read(conn);

         foreach (SystemDatabaseRecord record in records)
         {
            //We want to exclude SQLcompliance and SQLcompliance.Processing.  We only want event and archive databases.
            if (String.Equals(record.DatabaseType, "System") == false)
            {
               eventDBs.Add(record.DatabaseName);
            }
         }
         return eventDBs;
      }
   }
}
