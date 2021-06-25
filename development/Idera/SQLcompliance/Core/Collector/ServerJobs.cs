using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Win32;
using System.ComponentModel;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Cwf;
using Idera.SQLcompliance.Core.Status;
using Idera.SQLcompliance.Core.Licensing;
using Idera.SQLcompliance.Core.TimeZoneHelper;
using Idera.SQLcompliance.Core.Rules;
using Idera.SQLcompliance.Core.Rules.Alerts;
using Idera.SQLcompliance.Core.Event;
using Alert = PluginCommon.Alert;
using System.Configuration;

namespace Idera.SQLcompliance.Core.Collector
{
	/// <summary>
	/// Summary description for ServerJobs.
	/// </summary>
	internal class ServerJobs 
	{
      private BackgroundWorker _reindexWorker = new BackgroundWorker();
      private Reindex reIndexer = new Reindex();

      // Heartbeat
      private TimerState   heartbeatTimer                   = null;

      // 12:01 - License check
      private TimerState   licenseCheckTimer                = null;
      private int          licenseCheckMinutesAfterMidnight = 1;
      
      // 12:02 - Snapshot job
      private TimerState    snapshotJobTimer                = null;
      private int           snapshotMinutesAfterMidnight    = 2;
      
      // 12:05 - groomLogsAndAlerts job
      private TimerState    groomLogsAndAlertsTimer         = null;
      private int           groomLogsAndAlertsAfterMidnight = 5;
      
      // 12:10 - Data integrity check
      private TimerState    dataIntegrityCheckTimer           = null;
      private int           dataIntegrityMinutesAfterMidnight = 6;
      
      // 12:15 - AutoArchive job
      private TimerState    autoArchiveTimer                = null;
      private int           autoArchiveMinutesAfterMidnight = 15;

      // AlwaysOn Role Updates
      private TimerState alwaysOnRoleUpdateTimer = null;
	
      //---------------------------------------------------------------------------
      /// StartAllTimers
      //---------------------------------------------------------------------------
      public void StartAllTimers(SqlConnection conn)
		{
		   StartHeartbeat(conn);
		   StartLicenseChecking();
		   StartLogAndAlertGrooming();
		   StartSnapshots();
           StartAlwaysOnRoleUpdate(conn);
		}
		
      //---------------------------------------------------------------------------
      /// StopAllTimers
      //---------------------------------------------------------------------------
		public void StopAllTimers()
		{
		   StopHeartbeat();
		   StopLicenseChecking();
		   StopLogAndAlertGrooming();
		   StopSnapshots();
           StopAlwaysOnRoleUpdate();
		}

        #region AlwaysOn Role Update
        
        //---------------------------------------------------------------------------
      /// StartAlwaysOnRoleUpdate
      //---------------------------------------------------------------------------
      public void StartAlwaysOnRoleUpdate(SqlConnection conn)
      {
         try
         {
            if (alwaysOnRoleUpdateTimer == null)
            {
                alwaysOnRoleUpdateTimer = new TimerState();
            }
            else
            {
                alwaysOnRoleUpdateTimer.Reset();
            }
            SQLcomplianceConfiguration config = new SQLcomplianceConfiguration();
            config.Read(conn);
            alwaysOnRoleUpdateTimer.Interval = config.AlwaysOnRoleUpdateInterval * 1000;
            alwaysOnRoleUpdateTimer.EventHandler = new ElapsedEventHandler(AlwaysOnRoleUpdate);
            alwaysOnRoleUpdateTimer.Start();
         }
         catch( Exception e )
         {
             ErrorLog.Instance.Write("An error occurred starting alwaysOnRoleUpdate timer.", e, true);
         }
      }

        //---------------------------------------------------------------------------
        /// StopAlwaysOnRoleUpdate
        //---------------------------------------------------------------------------
      public void StopAlwaysOnRoleUpdate()
        {
            if (alwaysOnRoleUpdateTimer == null)
            {
                return;
            }
            alwaysOnRoleUpdateTimer.Reset();
        }

      //---------------------------------------------------------------------------
      /// AlwaysOnRoleUpdate
      //---------------------------------------------------------------------------
      public void AlwaysOnRoleUpdate(object sender, System.Timers.ElapsedEventArgs args)
      {
          // add this moment don't need to update any always on info.
    }

      /// <summary>
      /// ProcessAlwaysOnRoleUpdates: Do the AlwaysOn Processing 
      /// </summary>
      private void ProcessAlwaysOnRoleUpdates()
      {
          try
          {
              IList listDatabaseAlwaysOnDetails;
              IList listRawAVGroup;
              Repository repository = new Repository();
              if (!repository.OpenConnection(CoreConstants.RepositoryDatabase))
                  return;

              foreach (ServerRecord server in ServerRecord.GetServers(repository.connection, true, false))
              {
                  if (server.SqlVersion < 11)
                      continue;
                  //Check if the Instance has any AlwaysOn Configured DBs, Get details.
                  listDatabaseAlwaysOnDetails = ServerRecord.GetDatabaseAlwaysOnDetails(repository.connection, server.Instance);
                  if (listDatabaseAlwaysOnDetails != null && listDatabaseAlwaysOnDetails.Count > 0)
                  {
                      //If the instance has AlwaysOn DBs then fetch the Current AVGroup Role details for this instance
                      using (SqlConnection connAgent = SQLcomplianceAgent.GetConnection(server.Instance))
                      {
                          listRawAVGroup = RawSQL.GetAlwaysOnRoles(connAgent);
                          if (listRawAVGroup != null)
                          {
                              foreach (RawAVGroup rawAVGroup in listRawAVGroup)
                              {
                                  List<DatabaseAlwaysOnDetails> dbList = new List<DatabaseAlwaysOnDetails>();
                                  if (rawAVGroup.replicaCount > 1)
                                  {
                                      foreach (DatabaseAlwaysOnDetails databaseAlwaysOnDetails in listDatabaseAlwaysOnDetails)
                                      {
                                          //Check if the current Instance Node was secondary before make it primary now
                                          if (rawAVGroup.avgName.Equals(databaseAlwaysOnDetails.availGroupName))
                                          {
                                              if (
                                                  databaseAlwaysOnDetails.srvInstance.Equals(
                                                      rawAVGroup.primaryServerName))
                                              {
                                                  if (databaseAlwaysOnDetails.isPrimary == 0)
                                          {
                                              databaseAlwaysOnDetails.isPrimary = 1;
                                              dbList.Add(databaseAlwaysOnDetails);
                                          }
                                      }
                                              else
                                              {
                                                  if (databaseAlwaysOnDetails.isPrimary == 1)
                                                  {
                                                      databaseAlwaysOnDetails.isPrimary = 0;
                                                      dbList.Add(databaseAlwaysOnDetails);
                                                  }
                                              }
                                          }
                                      }
                                      if (dbList.Count > 0)
                                          ServerRecord.UpdateAlwaysOnRole(repository.connection, dbList);
                                  }
                              }
                          }
                      }
                  }
              }
          }
          catch (Exception e)
          {
              ErrorLog.Instance.Write("Cannot Process AlwaysOn Role changes.", e, true);
          }
      }



    //---------------------------------------------------------------------------
      /// ResetAlwaysOnTimer
    //---------------------------------------------------------------------------
    private void ResetAlwaysOnTimer(int interval)
    {
        StopAlwaysOnRoleUpdate();
        alwaysOnRoleUpdateTimer.Interval = interval * 1000;
        alwaysOnRoleUpdateTimer.EventHandler = new ElapsedEventHandler(AlwaysOnRoleUpdate);
        alwaysOnRoleUpdateTimer.Start();
    }

    #endregion

      #region Heartbeat

      //---------------------------------------------------------------------------
      /// StartHeartbeat
      //---------------------------------------------------------------------------
      public void StartHeartbeat(SqlConnection conn)
      {
         try
         {
            // do a heartbeat at startup
            WriteServerHeartbeat();

            if (heartbeatTimer == null)
            {
               heartbeatTimer = new TimerState();
            }
            else
            {
               heartbeatTimer.Reset();
            }
            SQLcomplianceConfiguration config = new SQLcomplianceConfiguration();
            config.Read(conn);
            heartbeatTimer.Interval = config.CollectionServerHeartbeatInterval * 60 * 1000;
            heartbeatTimer.EventHandler = new ElapsedEventHandler(ServerHeartbeat);
            heartbeatTimer.Start();
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( "An error occurred starting heartbeat timer.", e, true );
         }
      }
      
      //---------------------------------------------------------------------------
      /// StopHeartbeat
      //---------------------------------------------------------------------------
      public void StopHeartbeat( )
      {
         if (heartbeatTimer == null)
         {
            return;
         }
         heartbeatTimer.Reset();
      }

      //---------------------------------------------------------------------------
      /// ServerHeartbeat
      //---------------------------------------------------------------------------
      public void ServerHeartbeat(object sender, System.Timers.ElapsedEventArgs args)
      {
         try
         {
            heartbeatTimer.Pause();  // no double heartbeats if server is busy
            WriteServerHeartbeat();

            // Update Log Level
            CollectionServer.Instance.ReloadSomePreferences();

            // clear out archives that have been deleted from systemdatabases table
            ArchiveJob.UpdateArchiveList();
            ArchiveJob aj = new ArchiveJob();
            aj.Dispose();

            SynchronizeInstancesAsync();

            //Collection server status alerts
            GenerateStatusAlerts();

            ReindexEventDatabases();

            //This will only be done if an upgrade from 3.5 was just done.
            FixSensitiveColumnHash();

            //Reset the timer if the interval changed
            SQLcomplianceConfiguration config = new SQLcomplianceConfiguration();
            Repository rep = new Repository();

            try
            {
               rep.OpenConnection();
               config.Read(rep.connection);
            }
            catch (Exception e)
            {
               ErrorLog.Instance.Write("An error occurred getting the heartbeat interval.", e);
            }
            finally
            {
               rep.CloseConnection();
            }

            try
            {
               //Some how the interval is getting messed up. 
               //This is logging showing the messed up value. 
               //(This is mainly so QA can see that the bug is fixed.)
               if (config.CollectionServerHeartbeatInterval <= 0)
               {
                  ErrorLog.Instance.Write(ErrorLog.Level.Default, String.Format("Invalid collection server heartbeat interval. {0}", config.CollectionServerHeartbeatInterval), ErrorLog.Severity.Warning);
               }

               if ((config.CollectionServerHeartbeatInterval > 0) && (heartbeatTimer.Interval != config.CollectionServerHeartbeatInterval * 60 * 1000))
                  ResetTimer(config.CollectionServerHeartbeatInterval);
            }
            catch (Exception e)
            {
               ErrorLog.Instance.Write("An error occurred resetting the heartbeat interval.", e);
            }
            try
            {
                rep.OpenConnection();
                List<ServerRecord> servers = ServerRecord.GetServers(rep.connection, false);
                foreach (ServerRecord server in servers)
                {
                    //SQLCM -541/4876/5259 v5.6
                    if (server.AddNewDatabasesAutomatically)
                    {
                        AgentManager.AutomaticallyAuditNewDatabases(rep.connection, server);
                    }
                    //SQLCM -541/4876/5259 v5.6 - END
                    AgentManager.UpdateInstanceDatabasesTable(rep.connection, server);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred while updating the Databases table.", e);
            }
            finally
            {
                rep.CloseConnection();
            }
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write("An unknown error occurred processing the heartbeat.", e);
         }
         finally
         {
            //Debug logging to show that we contiued the heartbeat timer
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Continuing Heartbeat timer.", ErrorLog.Severity.Informational);
            heartbeatTimer.Continue();
         }
      }

      //---------------------------------------------------------------------------
      /// ResetTimer
      //---------------------------------------------------------------------------
      private void ResetTimer(int interval)
      {
         StopHeartbeat();
         heartbeatTimer.Interval = interval * 60 * 1000;
         heartbeatTimer.EventHandler = new ElapsedEventHandler(ServerHeartbeat);
         heartbeatTimer.Start();
      }
      
      //---------------------------------------------------------------------------
      /// WriteServerHeartbeat
      //---------------------------------------------------------------------------
      private void WriteServerHeartbeat()
      {
         Repository rep = new Repository();

         try
         {         
            rep.OpenConnection();
            // write entry to configuration table
            string sql = String.Format( "UPDATE {0}..{1} set serverLastHeartbeatTime=GETUTCDATE()",
                                       CoreConstants.RepositoryDatabase,
                                       CoreConstants.RepositoryConfigurationTable );
            using(SqlCommand cmd = new SqlCommand( sql, rep.connection ))
            {
             cmd.ExecuteNonQuery();                                       
            }
         }
         catch ( Exception ex )
         {
            ErrorLog.Instance.Write( "An error occurred writing Collection Server heartbeat.", ex );
         }
         finally
         {
            rep.CloseConnection();
         }                            
      }

	    private void SynchronizeInstancesAsync()
	    {
            Task.Factory.StartNew(SynchronizeInstances);
	    }

	    private void SynchronizeInstances()
	    {
	        try
	        {
	            CwfHelper.Instance.PushInstancesToDashboard();
	        }
	        catch (Exception ex)
	        {
                ErrorLog.Instance.Write("An error occurred during instances synchronization.", ex);
	        }
	    }

      /// <summary>
      /// 
      /// </summary>
      private void GenerateStatusAlerts()
      {
         StatusRuleProcessor processor = new StatusRuleProcessor();
         List<StatusAlertRule> rules = null;
         List<StatusAlert> alerts = new List<StatusAlert>();
         int state = 0;
         Repository rep = new Repository();
         AlertingConfiguration config = new AlertingConfiguration();

         try
         {
            config.Initialize(CollectionServer.ServerInstance);
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write("Unable to Generate the collection server status alerts", e);
            return;
         }
         ActionProcessor actionProcessor = new ActionProcessor(config);
            
         try
         {
            rep.OpenConnection();
            rules = AlertingDal.SelectStatusAlertRules(rep.connection);

            // No point in proceeding if there are no status alert rules.
            if (rules.Count <= 0)
               return;

            state = 1;
            alerts = processor.GenerateAlerts(rep.connection, rules);
            state = 2;

            if (alerts != null && alerts.Count > 0)
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose, String.Format("Alert: {0} collection server status alerts generated.", alerts.Count));

                var dashboardAlerts = new List<Alert>();

               // Store the alerts
               foreach (StatusAlert alert in alerts)
               {
                  string s = processor.ExpandMacros(alert.MessageTitle, alert);
                  alert.MessageTitle = s;
                  s = processor.ExpandMacros(alert.MessageBody, alert);
                  alert.MessageBody = s;
                  AlertingDal.InsertAlert(alert, rep.connection);

                  // add alert to dashboard alerts list
                  var dashboardAlert = new Alert();
                  dashboardAlert.AlertCategory = alert.AlertType.ToString();
                  dashboardAlert.Database = string.Empty;
                  dashboardAlert.Instance = alert.Instance;
                  dashboardAlert.LastActiveTime = alert.Created;
                  dashboardAlert.Metric = string.Empty;
                  dashboardAlert.ProductId = -1;
                  dashboardAlert.Severity = alert.Level.ToString();
                  dashboardAlert.StartTime = alert.Created;
                  dashboardAlert.Summary = alert.MessageTitle;
                  dashboardAlert.Table = string.Empty;
                  dashboardAlert.Value = alert.RuleName;
                  dashboardAlerts.Add(dashboardAlert);
               }
               state = 3;

               // push new alerts to CWF dashboard
               CwfHelper.Instance.PushAlertsToDashboard(dashboardAlerts);

               dashboardAlerts.Clear();
               
               // Prepare and store the actions
               alerts.Sort(new StatusAlertLevelDescending());
               state = 4;

               // Perform the actions
               actionProcessor.PerformActions(alerts, rep.connection);
            }
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "Alert: collection server alert processing finished.");
         }     
         catch (Exception e)
         {
            string errorString = "Error";

            switch (state)
            {
               case 0:
                  errorString = CoreConstants.Exception_StatusAlertingJobError_0;
                  break;
               case 1:
                  errorString = CoreConstants.Exception_StatusAlertingJobError_1;
                  break;
               case 2:
                  errorString = CoreConstants.Exception_StatusAlertingJobError_2;
                  break;
               case 3:
                  errorString = CoreConstants.Exception_StatusAlertingJobError_3;
                  break;
               case 4:
                  errorString = CoreConstants.Exception_StatusAlertingJobError_4;
                  break;
            }
            ErrorLog.Instance.Write(errorString, e);
         }
         finally
         {
            // Update alerts stats
            try
            {
               //if (alerts != null && alerts.Count > 0)
                  //Stats.Stats.Instance.UpdateAlertCount(_jobInfo.TargetInstance, alerts.Count);
            }
            catch
            {
            }
            rep.CloseConnection();
         }
      }

      private void FixSensitiveColumnHash()
      {
         Repository rep = new Repository();
         byte refactor = 0;

         try
         {
            rep.OpenConnection();

            using (SqlCommand command = new SqlCommand("SELECT refactorTable from SQLcompliance..Configuration", rep.connection))
            {
               object obj = command.ExecuteScalar();
               
               if (obj is DBNull)
                  refactor = 0;
               else
                  refactor = (byte)obj;

               if (refactor == 1)
               {
                  UpdateHashes(rep.connection);

                  command.CommandText = "UPDATE SQLcompliance..Configuration SET refactorTable = 0";
                  command.ExecuteNonQuery();
               }
            }
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write("Error updating the table.", e, ErrorLog.Severity.Error, true);
         }
         finally
         {
            rep.CloseConnection();
         }
      }

      private void UpdateHashes(SqlConnection conn)
      {
         List<SensitiveColumnRecord> scRecords = new List<SensitiveColumnRecord>();
         SensitiveColumnRecord scr;
         List<string> databases = GetRepositoryDatabases(conn);
         DataTable scTable;
         DataRow row;

         foreach (string database in databases)
         {
            scTable = GetSensitiveColumnTable(conn, database);

            for (int i = 0; i < scTable.Rows.Count; i++)
            {
               row = scTable.Rows[i];
               scr = CreateSensitiveColumnRecord(row);
               row["hashcode"] = scr.GetHashCode();
            }   
            UpdateHashCodes(scTable, database, conn);
         }
      }

      private void UpdateHashCodes(DataTable scTable, string database, SqlConnection conn)
      {
         SqlDataAdapter da = new SqlDataAdapter("", conn);

         using (SqlCommand cmd = CreateSCUpdateCommand(database, conn))
         {
            da.UpdateCommand = cmd;
            da.Update(scTable);
         }
      }

      private SqlCommand CreateSCUpdateCommand(string database, SqlConnection conn)
      {
         string stmt = String.Format("UPDATE {0}..{1} SET hashcode = @hashcode where scId = @scId",
                                     SQLHelpers.CreateSafeDatabaseName(database),
                                     CoreConstants.RepositorySensitiveColumnsTable);
         SqlCommand command = new SqlCommand(stmt, conn);

         command.CommandTimeout = CoreConstants.sqlcommandTimeout;
         command.Parameters.Add("@hashcode", SqlDbType.Int, 0, "hashcode");
         command.Parameters.Add("@scId", SqlDbType.Int, 0, "scId");
         return command;
      }
      
      protected SensitiveColumnRecord CreateSensitiveColumnRecord(DataRow row)
      {
         SensitiveColumnRecord scr = new SensitiveColumnRecord();

         scr.startTime = SQLHelpers.GetRowDateTime(row, "startTime");
         scr.eventId = SQLHelpers.GetRowInt32(row, "eventId");
         scr.columnName = SQLHelpers.GetRowString(row, "columnName");
         scr.hashcode = SQLHelpers.GetRowInt32(row, "hashcode");
         scr.tableId = SQLHelpers.GetRowInt32(row, "tableId");
         scr.columnId = SQLHelpers.GetRowInt32(row, "columnId");
         scr.scId = SQLHelpers.GetRowInt32(row, "scId");
         return scr;
      }

      private DataTable GetSensitiveColumnTable(SqlConnection conn, string database)
      {
         SqlDataAdapter da;
         DataTable table;
         string stmt = "";

         stmt = String.Format("SELECT {0} FROM {1}..{2}",
                              SensitiveColumnRecord.SelectColumnList,
                              SQLHelpers.CreateSafeDatabaseName(database),
                              CoreConstants.RepositorySensitiveColumnsTable);

         da = new SqlDataAdapter(stmt, conn);
         da.SelectCommand.CommandTimeout = CoreConstants.sqlcommandTimeout;
         table = new DataTable();
         da.Fill(table);
         return table;
      }

      private List<string> GetRepositoryDatabases(SqlConnection conn)
      {
         List<string> databases = new List<string>();

         string cmdstr = String.Format("SELECT databaseName " +
                                          "FROM SQLcompliance..{0} " +
                                          "WHERE databaseType='Archive' OR databaseType='Event' " +
                                          "ORDER by instance ASC, databaseType DESC",
                                        CoreConstants.RepositorySystemDatabaseTable);

         using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
         {
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
               while (reader.Read())
               {
                  databases.Add(SQLHelpers.GetString(reader, 0));
               }
            }
         }
         return databases;
      }

      private void ReindexEventDatabases()
      {
         RegistryKey rk = null;
         RegistryKey rks = null;

         try
         {
            rk = Registry.LocalMachine;
            rks = rk.CreateSubKey(CoreConstants.CollectionService_RegKey);

            if ((int)rks.GetValue(CoreConstants.CollectionService_RegVal_CheckEventIndexes) == 1)
            {
               //Start the Reindex thread but only is one is not already running
               if (_reindexWorker.IsBusy == false)
               {
                  _reindexWorker.DoWork += new DoWorkEventHandler(reIndexer.reindexWorker_DoWork);
                  //_reindexWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(reIndexer.reindexWorker_RunWorkerCompleted);
                  _reindexWorker.RunWorkerAsync();
               }
            }
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Default, "Unable to get the flag to see if I should start the thread.", ex.Message, ErrorLog.Severity.Warning);
         }
         finally
         {
            if (rks != null)
            {
               rks.Close();
            }

            if (rk != null)
            {
               rk.Close();
            }
         }
      }

      public void reindexWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
      {
      }

      #endregion
		
      #region LicenseChecking
      
      //---------------------------------------------------------------------------
      /// StartLicenseChecking
      //---------------------------------------------------------------------------
      public void StartLicenseChecking()
      {
         try
         {
            if( licenseCheckTimer == null )
               licenseCheckTimer = new TimerState( );
            else
               licenseCheckTimer.Reset();

            licenseCheckTimer.Interval = CalculateNextInterval(licenseCheckMinutesAfterMidnight);
            licenseCheckTimer.EventHandler = new ElapsedEventHandler(CheckLicenseEvent);
            licenseCheckTimer.Start();
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( "An error occurred starting license check timer.", e, true );
         }
      }
      
      //---------------------------------------------------------------------------
      /// StopLicenseChecking
      //---------------------------------------------------------------------------
      public void StopLicenseChecking( )
      {
         if( licenseCheckTimer == null )
            return;

         licenseCheckTimer.Reset();
      }

      //---------------------------------------------------------------------------
      /// CheckLicenseEvent - Delegate that handles elapsed event for licenseCheckTimer
      //---------------------------------------------------------------------------
      public void CheckLicenseEvent(object sender, System.Timers.ElapsedEventArgs   args)
      {
         licenseCheckTimer.Pause();
         
         ErrorLog.Instance.Write( ErrorLog.Level.Verbose, "CheckLicense job starting" );
         Idera.SQLcompliance.Core.Licensing.LicenseManager.CheckLicense();
         
         // restart timer
         licenseCheckTimer.Interval = CalculateNextInterval( licenseCheckMinutesAfterMidnight );
         licenseCheckTimer.Continue();
      }
      
      #endregion
      
      #region DataIntegrityChecking
      
      //---------------------------------------------------------------------------
      /// StartDataIntegrityChecking
      //---------------------------------------------------------------------------
      public void StartDataIntegrityChecking()
      {
         try
         {
            if (dataIntegrityCheckTimer == null)
            {
               dataIntegrityCheckTimer = new TimerState();
            }
            else
            {
               dataIntegrityCheckTimer.Reset();
            }
            dataIntegrityCheckTimer.Interval = CalculateNextInterval(dataIntegrityMinutesAfterMidnight);
            dataIntegrityCheckTimer.EventHandler = new ElapsedEventHandler(CheckDataIntegrityEvent);
            dataIntegrityCheckTimer.Start();
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( "An error occurred starting license check timer.", e, true );
         }
      }
      
      //---------------------------------------------------------------------------
      /// StopDataIntegrityChecking
      //---------------------------------------------------------------------------
      public void StopDataIntegrityChecking( )
      {
         if( dataIntegrityCheckTimer == null )
            return;

         dataIntegrityCheckTimer.Reset();
      }

      //---------------------------------------------------------------------------
      /// CheckDataIntegrityEvent - Delegate that handles elapsed event
      //---------------------------------------------------------------------------
      public void CheckDataIntegrityEvent (object sender, System.Timers.ElapsedEventArgs args)
      {
         ErrorLog.Instance.Write( ErrorLog.Level.Verbose, "Data integrity job starting" );
         IntegrityChecker ic = new IntegrityChecker();
         ic.CheckIntegrity(false);
         ErrorLog.Instance.Write( ErrorLog.Level.Verbose, "Data integrity job complete" );
      }
      
      #endregion
      
      #region AutoArchive
      
      //---------------------------------------------------------------------------
      /// StartAutoArchive
      //---------------------------------------------------------------------------
      public void StartAutoArchive()
      {
         try
         {
            if( autoArchiveTimer == null )
               autoArchiveTimer = new TimerState( );
            else
               autoArchiveTimer.Reset();

            autoArchiveTimer.Interval = CalculateNextInterval( autoArchiveMinutesAfterMidnight);
            ErrorLog.Instance.Write( ErrorLog.Level.Debug, 
                                     String.Format( "Next archive job will run in {0} minutes",
                                                    autoArchiveTimer.Interval / (60*1000) ) );
            autoArchiveTimer.EventHandler = new ElapsedEventHandler(AutoArchive);
            autoArchiveTimer.Start();
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( "An error occurred starting auto archive timer.", e, true );
         }
      }
      
      //---------------------------------------------------------------------------
      /// StopAutoArchive
      //---------------------------------------------------------------------------
      public void StopAutoArchive( )
      {
         if( autoArchiveTimer == null )
            return;

         autoArchiveTimer.Reset();
      }

      //---------------------------------------------------------------------------
      /// AutoArchive - Delegate that handles elapsed event for licenseCheckTimer
      //---------------------------------------------------------------------------
      private void AutoArchive (object sender, System.Timers.ElapsedEventArgs args)
      {
         try
         {
            autoArchiveTimer.Pause();
            ArchiveJob.UpdateArchiveList();
            ArchiveJob aj = new ArchiveJob();
            
            if ( aj.AutoArchiveOn )
            {
            
               // wake up each day and check if number of days has passed            
               if ( aj.IsTimeToArchive() )
               {
                  ErrorLog.Instance.Write( ErrorLog.Level.Verbose, "AutoArchive job started" );
                  LogSystemJob(LogType.AutoArchiveJob);
                  aj.Archive();
                  ErrorLog.Instance.Write( ErrorLog.Level.Verbose, "AutoArchive job complete" );
               }
            }
            else
            {
               ErrorLog.Instance.Write( ErrorLog.Level.Verbose, "AutoArchive skipped - AutoArchive has been set to 'Do not autoArchive'." );
            }
            autoArchiveTimer.Interval = CalculateNextInterval( autoArchiveMinutesAfterMidnight );
            ErrorLog.Instance.Write( ErrorLog.Level.Debug, 
                                     String.Format( "Next archive job will run in {0} minutes",
                                                    autoArchiveTimer.Interval / (60*1000) ) );
            autoArchiveTimer.Continue();
            aj.Dispose();
         }
         catch( Exception e)
         {
            ErrorLog.Instance.Write( "An error occurred during AutoArchive.", e, true );
         }
      }
      
      #endregion

      #region Grooming
      
      //---------------------------------------------------------------------------
      /// StartLogAndAlertGrooming
      //---------------------------------------------------------------------------
      public void StartLogAndAlertGrooming()
      {
         try
         {
            if( groomLogsAndAlertsTimer == null )
               groomLogsAndAlertsTimer = new TimerState( );
            else
               groomLogsAndAlertsTimer.Reset();

            groomLogsAndAlertsTimer.Interval = CalculateNextInterval( groomLogsAndAlertsAfterMidnight );
            groomLogsAndAlertsTimer.EventHandler = new ElapsedEventHandler(GroomLogsAndAlertsEvent);
            groomLogsAndAlertsTimer.Start();
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( "An error occurred starting groomLogsAndAlertsTimer.", e, true );
         }
      }
      
      //---------------------------------------------------------------------------
      /// StopLogAndAlertGrooming
      //---------------------------------------------------------------------------
      public void StopLogAndAlertGrooming( )
      {
         if( groomLogsAndAlertsTimer != null )
         {
            groomLogsAndAlertsTimer.Reset();
         }
      }

      //---------------------------------------------------------------------------
      /// GroomLogsAndAlertsEvent
      //---------------------------------------------------------------------------
      public void GroomLogsAndAlertsEvent (object sender, System.Timers.ElapsedEventArgs args)
      {
         ErrorLog.Instance.Write( ErrorLog.Level.Verbose, "GroomLogsAndAlerts job starting" );
         
         // clean out old events from
         //    AgentLog
         //    Alerts
         try
         {
            groomLogsAndAlertsTimer.Pause();
            Groom();
            groomLogsAndAlertsTimer.Interval = CalculateNextInterval( groomLogsAndAlertsAfterMidnight );
            groomLogsAndAlertsTimer.Continue();
         }
         catch( Exception e)
         {
            ErrorLog.Instance.Write( "An error occurred grooming logs and errors.", e, true );
         }
      }
      
      //---------------------------------------------------------------------------
      /// Groom - Groom alerts and agent events
      //---------------------------------------------------------------------------
      private void Groom()
      {
         int            groomLogsAge = 60;
         int            groomAlertsAge = 60;
         int            groomEventsAge = 60;
         TimeZoneHelper.TimeZoneInfo   timeZoneInfo;
         Repository rep = new Repository();

         try
         {
            rep.OpenConnection();

            // Get grooming settings
            GetGroomSettings( rep.connection,
                              out groomLogsAge,
                              out groomAlertsAge,
                              out groomEventsAge,
                              out timeZoneInfo );
             
             //Getting groomLogsAge value from config file
             System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
             groomLogsAge = int.Parse(config.AppSettings.Settings[CoreConstants.MAX_ACTIVITY_LOGS_AGE].Value);

            // groom agent events
            DateTime groomTime = CalculateGroomTime( groomLogsAge, timeZoneInfo );
            GroomAgentEvents( rep.connection, groomTime );

            // groom alerts
            groomTime = CalculateGroomTime( groomLogsAge, timeZoneInfo );
            GroomAlerts( rep.connection, groomTime );
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
               "An error occurred grooming events and logs.",
               e,
               true );
         }
         finally
         {
            rep.CloseConnection();
         }
      }

      //---------------------------------------------------------------------------
      /// GetGroomSettings - get groom settings from the repository.
      //---------------------------------------------------------------------------
      private void GetGroomSettings(SqlConnection    conn,
                                    out int          groomLogsAge,
                                    out int          groomAlertsAge,
                                    out int          groomEventsAge,
                                    out TimeZoneHelper.TimeZoneInfo timeZoneInfo)
      {
         groomLogsAge = 60;
         groomAlertsAge = 60;
         groomEventsAge = 60;
         
         int      bias         = 0;
         int      standardBias = 0;
         DateTime standardDate = DateTime.MinValue;
         int      daylightBias = 0;
         DateTime daylightDate = DateTime.MinValue;

         try
         {
            string query = String.Format( "SELECT archiveBias, archiveStandardBias, archiveStandardDate, archiveDaylightBias,archiveDaylightDate,"+
                                                 "groomAlertAge, groomLogAge, groomEventAge FROM {0}",
                                          CoreConstants.RepositoryConfigurationTable );
            using ( SqlCommand command = new SqlCommand( query, conn ) )
            {
               using ( SqlDataReader reader = command.ExecuteReader() )
               {
                  if( reader != null && reader.Read() )
                  {
                     int col = 0;
                     
                     bias          = SQLHelpers.GetInt32(reader,col++);
                     standardBias   = SQLHelpers.GetInt32(reader,col++);
                     standardDate   = SQLHelpers.GetDateTime(reader,col++);
                     daylightBias   = SQLHelpers.GetInt32(reader,col++);
                     daylightDate   = SQLHelpers.GetDateTime(reader,col++);
                     groomAlertsAge = SQLHelpers.GetInt32(reader,col++);
                     groomLogsAge   = SQLHelpers.GetInt32(reader,col++);
                     groomEventsAge = SQLHelpers.GetInt32(reader,col++);
                  }
               }
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
               "An error occurred reading groom settings from the repository database.",
               e,
               true );
         }
         
         TimeZoneStruct tzs = new TimeZoneStruct();
			tzs.Bias          = bias;
			tzs.StandardBias  = standardBias;
			tzs.StandardDate  = SystemTime.FromTimeZoneDateTime(standardDate);
			tzs.DaylightBias  = daylightBias;
			tzs.DaylightDate  = SystemTime.FromTimeZoneDateTime(daylightDate);
      	
			timeZoneInfo = new TimeZoneHelper.TimeZoneInfo();
			timeZoneInfo.TimeZoneStruct = tzs;
      }
      
      //---------------------------------------------------------------------------
      /// GroomAgentEvents - 
      //---------------------------------------------------------------------------
      private void GroomAgentEvents(SqlConnection conn, DateTime groomTime)
      {
         try
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose, "Grooming agent events." );
            string stmt = GetGroomAgentEventsStmt( groomTime );
            Execute( conn, stmt );
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( "An error occurred grooming agent events.", e.Message, ErrorLog.Severity.Warning );
         }
      }

      //---------------------------------------------------------------------------
      /// GroomAlerts - 
      //---------------------------------------------------------------------------
      private void GroomAlerts(SqlConnection conn, DateTime groomTime )
      {
         // TODO: waiting for the alerts table to be defined.
      }

      //---------------------------------------------------------------------------
      /// CalculateGroomTime - 
      //---------------------------------------------------------------------------
      private DateTime CalculateGroomTime( int groomAge, TimeZoneHelper.TimeZoneInfo timeZoneInfo)
      {
         DateTime eventTime = DateTime.UtcNow - new TimeSpan( groomAge, 0, 0, 0, 0 );
         // We are going to groom based on the local time-zone; however,
         //  events are stored in UTC, so return the Now - days in UTC form
         return eventTime ;
      }

      //---------------------------------------------------------------------------
      /// CalculateGroomTime - 
      //---------------------------------------------------------------------------
      private string GetGroomAgentEventsStmt(DateTime eventTime)
      {
         return String.Format( "DELETE {0} WHERE eventTime < {1}",
                               CoreConstants.RepositoryAgentEventTable,
                               SQLHelpers.CreateSafeDateTime(eventTime) );
      }
      
      #endregion

      #region Snapshots
      
      //---------------------------------------------------------------------------
      /// StartSnapshots
      //---------------------------------------------------------------------------
      public void StartSnapshots()
      {
         try
         {
            if( snapshotJobTimer == null )
               snapshotJobTimer = new TimerState( );
            else
               snapshotJobTimer.Reset();

            snapshotJobTimer.Interval = CalculateNextInterval( snapshotMinutesAfterMidnight);
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     String.Format( "Next audit snapshot job will run in {0} minutes",
                                                    snapshotJobTimer.Interval / (60*1000) ) );
            snapshotJobTimer.EventHandler = new ElapsedEventHandler(SnapshotsEvent);
            snapshotJobTimer.Start();
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( "An error occurred starting snapshotsJobTimer.", e, true );
         }
      }
      
      //---------------------------------------------------------------------------
      /// StopSnapshots
      //---------------------------------------------------------------------------
      public void StopSnapshots( )
      {
         if( snapshotJobTimer != null )
         {
            snapshotJobTimer.Reset();
         }
      }

      //---------------------------------------------------------------------------
      /// SnapshotsEvent
      //---------------------------------------------------------------------------
      public void SnapshotsEvent (object sender, System.Timers.ElapsedEventArgs args)
      {
         ErrorLog.Instance.Write( ErrorLog.Level.Verbose, "Snapshot job starting" );

         try
         {
            snapshotJobTimer.Pause();
            DoSnapshots();
            snapshotJobTimer.Interval = CalculateNextInterval( snapshotMinutesAfterMidnight);
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     String.Format( "Next audit snapshot job will run in {0} minutes",
                                                    snapshotJobTimer.Interval / (60*1000) ) );
            snapshotJobTimer.Continue();
         }
         catch( Exception e)
         {
            ErrorLog.Instance.Write( "An error occurred taking audit setting snapshots.", e, true );
         }
      }
      
      //---------------------------------------------------------------------------
      /// DoSnapshots
      //---------------------------------------------------------------------------
      private void DoSnapshots()
      {
         int      snapshotInterval = 0;
         DateTime snapshotLastTime = DateTime.MinValue;
         
         Repository rep = new Repository();

         try
         {
            rep.OpenConnection();

            // Get grooming settings
            Snapshot.GetSnapshotSettings( rep.connection,
                                          out snapshotInterval,
                                          out snapshotLastTime );
            // how long has it been?
            DateTime now  = DateTime.Now;
            TimeSpan diff = now - snapshotLastTime;
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                  String.Format( "Snapshot job: Now {0} Last {1} Diff {2} Interval {3}",
                                 now, snapshotLastTime, diff, snapshotInterval ) );

            if ( snapshotInterval > 0 )
            {
               if ( diff.Days >= snapshotInterval-1 )
               {
                  ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                          "Starting snapshots of all registered SQL Servers" );
                  LogSystemJob(LogType.CaptureSnapshotJob);
                  Snapshot.DumpAllServers( rep.connection );
                  Snapshot.UpdateSnapshotLastTime( rep.connection );
               }
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug, "An error occurred taking snapshots", e, true );
         }
         finally
         {
            rep.CloseConnection();
         }
      }
      
      #endregion
      
      #region Utility Routines

      //---------------------------------------------------------------------------
      /// CalculateNextInterval - calculate the interval so the job starts at
      /// midnight plus a few minutes the following day.
      //---------------------------------------------------------------------------
      private static int CalculateNextInterval(int minutesAfterMidnight)
      {
         int oneDay = 24*60*60*1000; // 1 day
         int newInterval = oneDay;

         try
         {
            DateTime now      = DateTime.Now;
            DateTime nextTime = new DateTime( now.Year, now.Month, now.Day, 0, minutesAfterMidnight, 0, 0 );
                                              
            if ( nextTime.CompareTo(now) <= 0 )
            {
               // already happened to day - schedule for tomorrow
               nextTime = nextTime.AddDays(1);
            }
            TimeSpan diff = nextTime - now;
            newInterval = (int)diff.TotalMilliseconds;

            // Note that TimeSpan.TotalMilliseconds may return fractions of a milliseconds and will result in
            // newInterval being 0.
            if (newInterval <= 0)
            {
               newInterval = oneDay;
            }
         }
         catch ( Exception ex )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     "Error in CalculateNextInterval - will run next job in 24 hours",
                                     String.Format( "Now: {0} MinutesAfterMidnight: {1}", DateTime.Now.ToString(), minutesAfterMidnight ),
                                     ex );
         }
         return newInterval;
      }

      //---------------------------------------------------------------------------
      /// Execute - execute a non-query SQL statement.
      //---------------------------------------------------------------------------
      private void Execute(SqlConnection conn, string stmt)
      {
		  using(SqlCommand command = new SqlCommand( stmt, conn ))
		  {
			  command.ExecuteNonQuery();
		  }
      }
      
      //---------------------------------------------------------------
      // LogSystemJob
      //---------------------------------------------------------------
      public void LogSystemJob(LogType jobType)
      {      
         Repository rep = new Repository();
         
         try
         {
            rep.OpenConnection();
            LogRecord.WriteLog( rep.connection,
                                jobType,
                                "",
                                String.Format("Computer: {0}",System.Net.Dns.GetHostName()),
                                CoreConstants.Log_SystemUser );
         }
         catch {}
         finally
         {
            rep.CloseConnection();                             
         }
      }
      
      #endregion
	}
}
