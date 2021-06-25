using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Collector;
using System.Configuration;

namespace Idera.SQLcompliance.Core.Rules.Alerts
{
   public delegate void JobCompleteCallback(string targetInstance) ;

   /// <summary>
   /// Summary description for AlertingJobPool.
   /// </summary>
   public class AlertingJobPool
   {
      #region Properties
        
      private int maxThreads;
      private int waitInterval = 300000 ; // 5 minutes 
      private int maxConcurrentThreads;
      private AlertingConfiguration _configuration ;
      private Hashtable _ruleProcessors ; // serverName -> RuleProcessor class (can be null)
      private Hashtable _dataRuleProcessors;
      private IList _rules ;
      private List<DataAlertRule> _dataRules;
      private object _rulesUpdateLock ;
      private ArrayList _activeJobs ; // Instance-level locking on jobs
      private object _activeJobsLock ;

      static private BlackHen.Threading.WorkQueue jobPool ;
      static private ManualResetEvent jobAvailableEvent = null ;

      static private bool doBADAlertProcessing = false;
      static private string bADAlertInstance;
      static private string bADAlertEventDB;
      static private int bADAlertEventCount;

      private bool stopped ;


      public int NumberThreads
      {
         get { return maxThreads ; }
         set
         {
            if (value != -1)
            {
               maxThreads = value ;
               maxConcurrentThreads = value ;
            }
         }
      }

      private static Object poolStartupLock = new Object() ;

      #endregion

      #region Constructor

      //-------------------------------------------------------------------------
      // AlertingJobPool constructor
      //-------------------------------------------------------------------------
      public AlertingJobPool(AlertingConfiguration configuration)
      {
            //If an exception comes then set some default value
            try
            {

                //Handling upgrade scenario
                bool isUpdated = false;
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None); // Add an Application Setting.

                if (config.AppSettings.Settings[CoreConstants.MAX_THREADS_KEY] == null)
                {
                    config.AppSettings.Settings.Add(CoreConstants.MAX_THREADS_KEY, CoreConstants.MAX_THREADS_VALUE);
                    isUpdated = true;
                }

                if (config.AppSettings.Settings[CoreConstants.MAX_CONCURRENT_THREADS_KEY] == null)
                {
                    config.AppSettings.Settings.Add(CoreConstants.MAX_CONCURRENT_THREADS_KEY, CoreConstants.MAX_CONCURRENT_THREADS_VALUE);
                    isUpdated = true;
                }

                if (config.AppSettings.Settings[CoreConstants.MAX_ACTIVITY_LOGS_AGE] == null)
                {
                    config.AppSettings.Settings.Add(CoreConstants.MAX_ACTIVITY_LOGS_AGE, CoreConstants.MAX_ACTIVITY_LOGS_AGE_VALUE);
                    isUpdated = true;
                }

                if (isUpdated)
                {
                    config.Save(ConfigurationSaveMode.Modified);
                }

                maxThreads = int.Parse(config.AppSettings.Settings[CoreConstants.MAX_THREADS_KEY].Value.Trim());
                maxConcurrentThreads = int.Parse(config.AppSettings.Settings[CoreConstants.MAX_CONCURRENT_THREADS_KEY].Value.Trim());

            }
            catch (Exception ex)
            {
                maxThreads = 100;
                maxConcurrentThreads = 100;
            }


            if (configuration == null)
            throw new ArgumentNullException("configuration") ;
         _configuration = configuration ;
         _activeJobs = new ArrayList();
         _activeJobsLock = new object() ;
         _rulesUpdateLock = new object() ;
         _ruleProcessors = new Hashtable() ;
         _dataRuleProcessors = new Hashtable();
         LoadAlertRules() ;
         stopped = false ;
      }

      #endregion

      #region Public Methods

      static public bool DoBADAlertProcessing
      {
          get { return doBADAlertProcessing; }
          set
          {
              doBADAlertProcessing = value;
          }
      }

      static public void SetBADAlertDetails(string instance, string eventDB, int eventCount)
      {
          bADAlertInstance = instance;
          bADAlertEventDB = eventDB;
          bADAlertEventCount = eventCount;
      }

      public void LoadAlertRules()
      {
         // this lock is for the _ruleProcessor object
         lock(_rulesUpdateLock)
         {
            _rules = AlertingDal.SelectAlertRules(_configuration.ConnectionString) ;
            _ruleProcessors.Clear() ;
            _dataRules = AlertingDal.SelectDataAlertRules(_configuration.ConnectionString);
            _dataRuleProcessors.Clear();
         }
      }

      //-------------------------------------------------------------------------
      // Initialize
      //-------------------------------------------------------------------------
      public void Initialize()
      {
         ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AlertingJobPool::Initialize - Enter") ;


         ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AlertingJobPool::Initialize - Leave") ;
      }

      //-------------------------------------------------------------------------
      // Start
      //-------------------------------------------------------------------------
      public void Start()
      {
         ErrorLog.Instance.Write(ErrorLog.Level.Verbose, CoreConstants.Info_EnterAlertingJobPool) ;

         try
         {
            // create thread pool
            jobPool = new BlackHen.Threading.WorkQueue() ;
            jobPool.ConcurrentLimit = maxConcurrentThreads ;
            ((BlackHen.Threading.WorkThreadPool) jobPool.WorkerPool).MaxThreads = maxThreads ;
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write("AlertingJobPool::Start", ex, true) ;
         }

         stopped = false ;

         while (!stopped)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AlertingJobPool::ProcessingLoop - Enter") ;

            try
            {
               // event to signal job pool manager thread that work has arrived
               jobAvailableEvent = new ManualResetEvent(false) ;

               // loop forever waiting for alerting jobs to come around 
               while (!stopped)
               {
                  // This query lets us know any servers that have data for which alerts have not been generated
                  AlertingJobInfo[] jobs = AlertingDal.SelectAlertableServers(_configuration.ConnectionString) ;

                  if (doBADAlertProcessing)
                  {
                      AlertingJobInfo job = new AlertingJobInfo();
                      job.TargetInstance = bADAlertInstance;
                      job.ServerDbName = bADAlertEventDB;
                      job.EventCountForBAD = bADAlertEventCount;
                      job.DoBADAlertProcessing = doBADAlertProcessing;
                      lock (_activeJobsLock)
                      {
                          if (_activeJobs.Contains(job.TargetInstance))
                              continue;
                          else
                              _activeJobs.Add(job.TargetInstance);
                      }
                      job.Configuration = _configuration;
                      job.Processor = GetRuleProcessor(job.TargetInstance);
                      job.DataProcessor = GetDataRuleProcessor(job.TargetInstance);
                      job.JobCompleteHandler = new JobCompleteCallback(MarkJobComplete);
                      jobPool.Add(new AlertingJobPoolWorkItem(job));
                      ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                               String.Format("Before After Data Alert processing job created for {0}.\n" +
                                                              "Event Count = {1}",
                                                              job.TargetInstance,
                                                              job.EventCountForBAD));

                      doBADAlertProcessing = false;
                      bADAlertInstance = "";
                      bADAlertEventDB = "";
                      bADAlertEventCount = 0;
                  }

                  foreach(AlertingJobInfo job in jobs)
                  {
                     // queue up a worker thread
                     if(!stopped)
                     {
                        lock(_activeJobsLock)
                        {
                           if(_activeJobs.Contains(job.TargetInstance))
                              continue ;
                           else
                              _activeJobs.Add(job.TargetInstance) ;
                        }
                        job.Configuration = _configuration ;
                        job.Processor = GetRuleProcessor(job.TargetInstance) ;
                        job.DataProcessor = GetDataRuleProcessor(job.TargetInstance);
                        job.JobCompleteHandler = new JobCompleteCallback(MarkJobComplete) ;
                        jobPool.Add(new AlertingJobPoolWorkItem(job)) ;
                        ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                                 String.Format( "Alert processing job created for {0}.\n" +
                                                                "Highwatermark = {1}\n" +
                                                                "Alert highwatermark = {2}",
                                                                job.TargetInstance, 
                                                                job.HighWatermark,
                                                                job.AlertHighWatermark ) );
                     }
                  }

                                           

                  if (stopped) break ;

                  // wait for more jobs signal - we will wait a specified number of minutes before
                  // we decide to check again to make sure that nothing is waiting
                  bool signalled = jobAvailableEvent.WaitOne(waitInterval, true) ;
                  if (signalled)
                  {
                     jobAvailableEvent.Reset() ;
                  }
               }
            }
            catch (Exception e)
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AlertingJobPool::ExceptionHandler - Enter") ;
               ErrorLog.Instance.Write("An exception occurred in the AlertingJobPool.", e, true) ;

               // Clear waiting threads
               jobPool.Clear() ;

               // We are not as complicated as the TraceJobPool.
               //  When things go wrong, we go to timeout
               Thread.Sleep(CoreConstants.CollectionService_AlertJobPoolRetryInterval) ;
               ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AlertingJobPool::ExceptionHandler - Leave") ;
            } // catch
         } // while ( ! stopped )
         ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AlertingJobPool::ProcessingLoop - Leave") ;
      }

      private void MarkJobComplete(string targetInstance)
      {
         lock(_activeJobsLock)
         {
            _activeJobs.Remove(targetInstance) ;
         }
      }

      //---------------------------------------------------------------------------
      // Stop
      //---------------------------------------------------------------------------
      public void Stop()
      {
         stopped = true ;

         // stop any waiting jobs from waiting
         if (jobPool != null)
         {
            jobPool.Clear() ;
         }


         // Wake up jobPool if waiting		   
         if (jobAvailableEvent != null)
            jobAvailableEvent.Set() ;

         // Wait for any remaining jobs to shut down - should be fast since
         // aborting flag has been set - main reason for slwo is we are waiting for a
         // timeout in a SQL call or some other lenghty operation - but we dont want to 
         // shut down until all threads are stopped since it causes busy file problems
         if (jobPool != null)
         {
            int waitCount = 0 ;
            bool firstTime = true ;
            while (jobPool.Count != 0)
            {
               if (firstTime)
               {
                  firstTime = false ;
                  ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                          "Waiting for all alert processing threads to finish before stopping service.") ;
               }

               if (! jobPool.WaitAll(new TimeSpan(0, 0, 2, 0, 0)))
               {
                  waitCount ++ ;
                  if ((jobPool.Count != 0) && (waitCount%5 == 0)) // 10 minutes
                  {
                     ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                             String.Format("Waiting for {0} alert processing threads to complete before shutting down.",
                                                           jobPool.Count),
                                             ErrorLog.Severity.Warning) ;
                  }
               }
            }
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                    "Alert processing threads all stopped") ;
         }
      }

      //---------------------------------------------------------------------------
      // SignalNewEventsAvailable
      //---------------------------------------------------------------------------
      static public void SignalNewEventsAvailable()
      {
         // this is called from trace processing.  We do not want
         //  to interrupt trace processing with errors.
         try
         {
            if (CollectionServer.Instance.alertingJobThread == null) return ;
            if (CollectionServer.Instance.aborting) return ;

            // only let one job restart pool
            lock (poolStartupLock)
            {
               if (! CollectionServer.Instance.alertingJobThread.IsAlive)
               {
                  ErrorLog.Instance.Write(CoreConstants.Exception_ErrorAlertingJobPoolThreadDown,
                     ErrorLog.Severity.Warning) ;
                  CollectionServer.Instance.StartAlerting() ;
               }
            }

            jobAvailableEvent.Set() ;
         }
         catch
         {
         }
      }

      #endregion

      private RuleProcessor GetRuleProcessor(string sInstance)
      {
         RuleProcessor retVal ;

         lock(_rulesUpdateLock)
         {
            retVal = new RuleProcessor("Alert Rules", sInstance, _rules) ;
            _ruleProcessors[sInstance] = retVal ;
            return retVal.Clone() ;
         }
      }

      private DataRuleProcessor GetDataRuleProcessor(string sInstance)
      {
         DataRuleProcessor dataRuleProcessor;

         lock (_rulesUpdateLock)
         {
            dataRuleProcessor = (DataRuleProcessor)_dataRuleProcessors[sInstance];
            if (dataRuleProcessor == null)
            {
               dataRuleProcessor = new DataRuleProcessor("Data Alert Rules", sInstance, _dataRules);
               _dataRuleProcessors[sInstance] = dataRuleProcessor;
            }
            return dataRuleProcessor;
         }
      }
   }
}
