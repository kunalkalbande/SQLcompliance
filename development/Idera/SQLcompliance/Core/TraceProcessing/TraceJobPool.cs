using System;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using Idera.SQLcompliance.Core.Rules ;
using Idera.SQLcompliance.Core.Rules.Filters ;
using Microsoft.Win32;
using System.Configuration;

using BlackHen.Threading;

using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Collector;

namespace Idera.SQLcompliance.Core.TraceProcessing
{
   
	/// <summary>
	/// Summary description for TraceJobPool.
	/// </summary>
	internal class TraceJobPool
	{
        #region Properties
        
	    int maxThreads;
	    int waitInterval = 300000; // 5 minutes (normally awoken by signal from
	                                                          //             trace receive msg handler, so this
	                                                          //             is a safety mechanism not normal
	    int maxConcurrentThreads;
	   
      static WorkQueue                     jobPool;
	   static ManualResetEvent              jobAvailableEvent = null;

      private Hashtable _ruleProcessors ; // serverName -> RuleProcessor class (can be null)
      private IList _rules ;
      private object _rulesUpdateLock ;
      private FiltersConfiguration _configuration ;

	   private bool                         stopped;
	   private int                          startupErrors = 0;
	   
	   public  string                       traceDirectory;
	   
	   public int NumberThreads
	   {
	      get { return maxThreads; }
	      set
	      {
	         if ( value != -1 )
	         {
	            maxThreads           = value;
	            maxConcurrentThreads = value;
	         }
	      }
	   }
	   
      private static Object poolStartupLock = new Object();
	   
	   #endregion
	   
	   #region Constructor

	   //-------------------------------------------------------------------------
	   // TraceJobPool constructor
	   //-------------------------------------------------------------------------
		public
		   TraceJobPool(FiltersConfiguration config)
		{

            //If an exception comes then set some default value
            try
            {

                //Handling upgrade scenario
                bool isUpdated = false;
                System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None); // Add an Application Setting.

                if (configuration.AppSettings.Settings[CoreConstants.PARALLEL_PROCESSING_ENABLED] == null)
                {
                    if (configuration.AppSettings.Settings[CoreConstants.MAX_THREADS_KEY] == null)
                    {
                        configuration.AppSettings.Settings.Add(CoreConstants.MAX_THREADS_KEY, CoreConstants.MAX_THREADS_VALUE);
                    }
                    else
                    {
                        configuration.AppSettings.Settings[CoreConstants.MAX_THREADS_KEY].Value = CoreConstants.MAX_THREADS_VALUE;
                    }

                    if (configuration.AppSettings.Settings[CoreConstants.MAX_CONCURRENT_THREADS_KEY] == null)
                    {
                        configuration.AppSettings.Settings.Add(CoreConstants.MAX_CONCURRENT_THREADS_KEY, CoreConstants.MAX_CONCURRENT_THREADS_VALUE);
                    }
                    else
                    {
                        configuration.AppSettings.Settings[CoreConstants.MAX_CONCURRENT_THREADS_KEY].Value = CoreConstants.MAX_CONCURRENT_THREADS_VALUE;
                    }

                    configuration.AppSettings.Settings.Add(CoreConstants.PARALLEL_PROCESSING_ENABLED, "TRUE");
                    isUpdated = true;
                }
                else
                {
                    if (configuration.AppSettings.Settings[CoreConstants.MAX_THREADS_KEY] == null)
                    {
                        configuration.AppSettings.Settings.Add(CoreConstants.MAX_THREADS_KEY, CoreConstants.MAX_THREADS_VALUE);
						isUpdated = true;
                    }

                    if (configuration.AppSettings.Settings[CoreConstants.MAX_CONCURRENT_THREADS_KEY] == null)
                    {
                        configuration.AppSettings.Settings.Add(CoreConstants.MAX_CONCURRENT_THREADS_KEY, CoreConstants.MAX_CONCURRENT_THREADS_VALUE);
						isUpdated = true;
                    }
                }
                if (isUpdated)
                {
                    configuration.Save(ConfigurationSaveMode.Modified);
                }

                maxThreads = int.Parse(configuration.AppSettings.Settings[CoreConstants.MAX_THREADS_KEY].Value.Trim());
                maxConcurrentThreads = int.Parse(configuration.AppSettings.Settings[CoreConstants.MAX_CONCURRENT_THREADS_KEY].Value.Trim());
            }
            catch (Exception ex)
            {
                maxThreads = 25;
                maxConcurrentThreads = 25;
            }

            _configuration = config ;
         _ruleProcessors = new Hashtable() ;
         _rulesUpdateLock = new object() ;
         LoadEventFilters() ;
		   stopped = false;
        }

        #endregion

        #region Public Methods

        public void LoadEventFilters()
      {
         lock(_rulesUpdateLock)
         {
            _rules = FiltersDal.SelectEventFilters(_configuration.ConnectionString) ;
            _ruleProcessors.Clear() ;
         }
      }

	   //-------------------------------------------------------------------------
	   // Initialize
	   //-------------------------------------------------------------------------
		public void
		   Initialize()
		{
	      ErrorLog.Instance.Write( ErrorLog.Level.Debug, "TraceJobPool::Initialize - Enter" );
	      
		   try
		   {
            ReadTraceDirectory();
            StartupCleanup();
         }
         catch ( Exception ex )
         {
		      ErrorLog.Instance.Write( "TraceJobPool::Initialize",
		                               ex,
		                               true );
         }
         
	      ErrorLog.Instance.Write( ErrorLog.Level.Debug, "TraceJobPool::Initialize - Leave" );
		}

	   //-------------------------------------------------------------------------
	   // Start
	   //-------------------------------------------------------------------------
		public void
		   Start()
		{
		   ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
		                           CoreConstants.Info_EnterJobPool );
		   try
		   {
   		                            
   		
            ReadTraceDirectory();
            
		      // create thread pool
		      jobPool = new WorkQueue();
            jobPool.ConcurrentLimit = maxConcurrentThreads;
            ((WorkThreadPool) jobPool.WorkerPool).MaxThreads  = maxThreads;
         }
         catch ( Exception ex )
         {
		      ErrorLog.Instance.Write( "TraceJobPool::Start",
		                               ex,
		                               true );
         }
         
         stopped = false;
         
		   while ( !stopped )
		   {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug, "TraceJobPool::ProcessingLoop - Enter" );  

		      try
		      {
		         // Clean up fragments left over from aborted or interrupted jobs
		         // last time server was running
		         StartupCleanup();
		      
		         // event to signal job pool manager thread that work has arrived
			      jobAvailableEvent = new ManualResetEvent(false);
			      
			      // Actual work of job pool manager

		         // loop forever waiting for trace file processing jobs to come around 
               while ( !stopped )
               {
                  TraceJobInfo jobInfo ;
                  jobInfo = TraceJobInfo.GetNextJob();
                   
		            while ( jobInfo != null && ! stopped)
		            {
                     jobInfo.eventFilterProcessor = GetRuleProcessor(jobInfo.instance) ;

                     // set temptableprefix for this job
                     jobInfo.UpdateState(TraceJobInfo.State.WaitingToRun);
                        
                     // queue up a worker thread	
                     jobPool.Add( new TraceJobPoolWorkItem( jobInfo ) );

                     // get another job		                                       
                     jobInfo = TraceJobInfo.GetNextJob();
		            }
		            
		            if ( stopped ) break;
      		      
		            // wait for more jobs signal - we will wait a specified number of minutes before
		            // we decide to check again to make sure that nothing is waiting
		            bool signalled = jobAvailableEvent.WaitOne( waitInterval, true);
		            if ( signalled )
		            {
      		         jobAvailableEvent.Reset();
      		      }
      		      
      		      // if SQL Server was bounced while we were waiting, we may have jobs that were aborted
      		      // but couldnt update their job state so we are left with jobs in state 1 or 2 that
      		      // dont really exist - symptom: no active jobs; some states != 0,4 or 5
      		      //
      		      // no new jobs in this bad can be created while we do this since they are all
      		      // created by this routinein the while loop above
      		      if ( !stopped && jobPool.Count == 0 )
      		      {
      		         TraceCleanup.ResetTraceJobs();
      		      }
		         }
            }
            catch( Exception e)
            {
               //if we are here, then the job pool mgr shutdown - this is normally
               // caused by SQL Server being shut out from under us - so we
               // will retry conecting every 15 minutes
               // Once we reestablish the connection we will start the job 
               // pool manager back up
               
               // TODO: Wait until threads are all gone before restarting
               
               // Clear waiting threads
               jobPool.Clear();
               TraceJobInfo.ResetJobQueue();
               ExceptionHandler( e );
               /*
               // wait for jobs already executing to die off
               bool done     = false;
               int  count    = 0;
               int  maxCount = 24;  // 24*5 = 2 hours
               
               Repository  rep         = new Repository();
               bool        bConnecting = true;

               try
               {               
                  // loop waiting for all current jobs to shut down or for sql server to be back up
                  ErrorLog.Instance.Write( ErrorLog.Level.Debug, "TraceJobPool::ExceptionHandler - Enter WaitForCurrentJobs" );  
                  while ( !done && !stopped )
                  {
                     if ( jobPool.Count == 0 )
                        done = true;
                     else
                        done = jobPool.WaitAll( new TimeSpan(0,0,5,0,0) );

                     if ( !done )
                     {
                        // try to connect to SQL Server - if we can connect we are done!
                        if ( rep.OpenConnection( false ) )
                        {
                           bConnecting = false;
                           done = true;
                        }
                     }
                     
                     if ( ! done )
                     {
                        count++;
                        if ( count < maxCount)
                        {
                           if  (count % 3 == 0) // log every 15 minutes
                           {
		                        ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
		                                                String.Format( "TraceJobPool: Waiting for processing threads to die after problem (likely cause SQL Server down) - Total time waited: {0} minutes",
		                                                               count * 5 ) );
		                     }
		                  }
		                  else
		                  {
		                     // sql server down - hung in job pool - just die
		                     ErrorLog.Instance.Write( CoreConstants.Exception_TraceJobHung,
		                                              ErrorLog.Severity.Error );
                           System.Diagnostics.Process p = System.Diagnostics.Process.GetCurrentProcess();
                           p.Kill();
		                  }
                     }
                  }
                  ErrorLog.Instance.Write( ErrorLog.Level.Debug, "TraceJobPool::ExceptionHandler - Leave WaitForCurrentJobs" );  

                  // loop trying to connect waiting for SQL Server to come back up  
                  while (bConnecting && ! stopped )
                  {
                     if ( rep.OpenConnection( false ) )
                     {
                        bConnecting = false;
                     }
                     else
                     {
                        // wait a few minutes between each connection attempt
                        Thread.Sleep( CoreConstants.CollectionService_SqlReconnectInterval );
                     }
                  }
               }
               finally
               {
                  rep.CloseConnection();
               }
               
               ErrorLog.Instance.Write( ErrorLog.Level.Debug, "TraceJobPool::ExceptionHandler - Leave" );  
               */
               
            } // catch
			} // while ( ! stopped )
         ErrorLog.Instance.Write( ErrorLog.Level.Debug, "TraceJobPool::ProcessingLoop - Leave" );  

		   ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
		                            CoreConstants.Info_LeaveJobPool );
		}
		
		//---------------------------------------------------------------------------
		// Stop
		//---------------------------------------------------------------------------
		public void Stop()
		{
		   stopped = true;
		   
		   // stop any waiting jobs from waiting
		   if ( jobPool != null)
		   {
		       jobPool.Clear();
		   }
		   
         // Set aborting flags for all running and waiting jobs.
         TraceJobInfo.SetAbortingAll();

         // Wake up jobPool if waiting		   
		   if (jobAvailableEvent != null )
		      jobAvailableEvent.Set();
		   
		   // Wait for any remaining jobs to shut down - should be fast since
		   // aborting flag has been set - main reason for slwo is we are waiting for a
		   // timeout in a SQL call or some other lenghty operation - but we dont want to 
		   // shut down until all threads are stopped since it causes busy file problems
		   if ( jobPool != null )
		   {
		      int  waitCount = 0;
		      bool firstTime = true;
		      while ( jobPool.Count != 0)
		      {
		         if ( firstTime )
		         {
		            firstTime = false;
                  ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                          "Waiting for all processing threads to finish before stopping service." );
		         }
                                          
			      if( ! jobPool.WaitAll( new TimeSpan(0,0,2,0,0) ) )
			      {
			         waitCount ++;
			         if ( (jobPool.Count != 0) && ( waitCount % 5 == 0 )) // 10 minutes
			         {
                     ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                              String.Format( "Waiting for {0} processing threads to complete before shutting down.",
                                                             jobPool.Count ),
                                              ErrorLog.Severity.Warning);
                  }
               }
            }
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     "Processing threads all stopped" );
         }
	   }
	   
		//---------------------------------------------------------------------------
		// StopInstanceJobs
		//---------------------------------------------------------------------------
		public void
		   StopInstanceJobs(
		      string            instanceName
		   )
		{
		   jobPool.Pause();
		   
         TraceJobInfo.KillFinishedJobs( instanceName );		   
		   
		   // jobs in queue - Existing jobs check the jobs table periodicaly and will die gracefully
		   //                 if their job record has been deleted - this way uninstall doesnt
		   //                 have to wait for jobs to finish before continuing
         TraceJobInfo.SetAbortingForInstanceJobs( instanceName );
         
		   jobPool.Resume();

         // loop until all instance jobs are done
         //    Wait up to 60 seconds - any jobs not done after this are jobs
         //       that havent started that will abort as soon as they startup
         //    Our main goal here is to give jobs that are writing events
         //       database time to finish so events database isnt busy when
         //       we try to delete it
         int startTime = Environment.TickCount;
         while ( TraceJobInfo.GetInstanceJobCount(instanceName) > 0 )
         {
            Thread.Sleep(500);
            if ( Environment.TickCount - startTime > 60000 ) break;
         }
	   }
		
		//---------------------------------------------------------------------------
		// StartupCleanup - Special bootstrap logic done at JobPool startup
		//---------------------------------------------------------------------------
		public void StartupCleanup()
		{
		   try
		   {
		      TraceCleanup.KillFinishedJobs();
		      TraceCleanup.ResetTraceJobs();
		      TraceCleanup.KillOrphanedTraceFiles();
		      TraceCleanup.KillDecompressedFiles( traceDirectory );
		      startupErrors = 0;
		   }
		   catch ( Exception ex )
		   {
		      startupErrors++;
		      // report errors every hour
		      if ( startupErrors % 120 != 1 )
		      {
		         Thread.Sleep( 30000 );
               ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                       "An error occurred during startup clean up",
                                       ex,
                                       ErrorLog.Severity.Warning);
            }
		      else
		         throw;
		   }
		}

	   //---------------------------------------------------------------------------
		// SignalNewTraceAvailable - This is the routine called by the trace file
		//                           received to wake us up if we are idle
		//---------------------------------------------------------------------------
		static public void SignalNewTraceAvailable()
		{
		   if ( CollectionServer.Instance.jobPoolThread == null ) return;
		   if ( CollectionServer.Instance.aborting ) return;

         // only let one job restart pool
 			lock ( poolStartupLock)
			{
		      if ( ! CollectionServer.Instance.jobPoolThread.IsAlive )
		      {
               ErrorLog.Instance.Write( CoreConstants.Exception_ErrorJobPoolThreadDown,
                                       ErrorLog.Severity.Warning );
               CollectionServer.Instance.StartTraceProcessing() ;
		      }
		   }
		    
		   jobAvailableEvent.Set();
		}
		
      //-----------------------------------------------------------------------		
      // ReadTraceDirectory
      //-----------------------------------------------------------------------		
		private void
		   ReadTraceDirectory( )
		{
         RegistryKey rk  = null;
         RegistryKey rks = null;
         
			try
			{
				rk  = Registry.LocalMachine;
				rks = rk.CreateSubKey(CoreConstants.CollectionService_RegKey);
				traceDirectory = (string)rks.GetValue( CoreConstants.CollectionService_RegVal_TraceDir, "" );
			}
			catch (Exception)
			{
			}
			finally
			{
				if ( rks != null )rks.Close();
				if ( rk != null )rk.Close();
			}

         // validate preferences         
         if ( traceDirectory == "" )
         {
            string msg = String.Format( CoreConstants.Alert_InvalidTraceDirectory,
                                        traceDirectory  );
            ErrorLog.Instance.Write( ErrorLog.Level.Default,
                                     msg,
                                     ErrorLog.Severity.Error );
            throw new Exception (msg);
         }	                                                        
		}

      private RuleProcessor GetRuleProcessor(string sInstance)
      {
         lock(_rulesUpdateLock)
         {
            RuleProcessor retVal ;
            retVal = (RuleProcessor)_ruleProcessors[sInstance] ;
            if(retVal == null)
            {
               retVal = new RuleProcessor("Event Filters", sInstance, _rules) ;
               _ruleProcessors[sInstance] = retVal ;
            }
            return retVal.Clone() ;
         }
      }
      
      //
      // ExceptionHandler(): Handles exceptions in ThreadPool::Start().  Checks
      //      SQL Server availability and retries if unavailable.
      // 
      private void ExceptionHandler( Exception e )
      {
         bool done = false;
         int count = 0;
         int maxCount = 24; // 24*5 = 2 hours

         Repository rep = new Repository();
         bool bConnecting = true;

         try
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "TraceJobPool::ExceptionHandler - Enter");
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, e);
            
            // loop waiting for all current jobs to shut down or for sql server to be back up
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     "TraceJobPool::ExceptionHandler - Enter WaitForCurrentJobs" );
            while ( !done && !stopped )
            {
               if ( jobPool.Count == 0 )
                  done = true;
               else
                  done = jobPool.WaitAll( new TimeSpan( 0, 0, 5, 0, 0 ) );

               if ( !done )
               {
                  // try to connect to SQL Server - if we can connect we are done!
                  if ( rep.OpenConnection( false ) )
                  {
                     bConnecting = false;
                     done = true;
                  }
               }

               if ( ! done )
               {
                  count++;
                  if ( count < maxCount )
                  {
                     if ( count % 3 == 0 ) // log every 15 minutes
                     {
                        ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                                 String.Format(
                                                    "TraceJobPool: Waiting for processing threads to die after problem (likely cause SQL Server down) - Total time waited: {0} minutes",
                                                    count * 5 ) );
                     }
                  }
                  else
                  {
                     // sql server down - hung in job pool - just die
                     ErrorLog.Instance.Write( CoreConstants.Exception_TraceJobHung,
                                              ErrorLog.Severity.Error );
                     Process p = Process.GetCurrentProcess();
                     p.Kill();
                  }
               }
            }
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     "TraceJobPool::ExceptionHandler - Leave WaitForCurrentJobs" );

            // loop trying to connect waiting for SQL Server to come back up  
            while ( bConnecting && ! stopped )
            {
               if ( rep.OpenConnection( false ) )
               {
                  bConnecting = false;
               }
               else
               {
                  // wait a few minutes between each connection attempt
                  Thread.Sleep( CoreConstants.CollectionService_SqlReconnectInterval );
               }
            }
         }
         finally
         {
            rep.CloseConnection();
         }

         ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                  "TraceJobPool::ExceptionHandler - Leave" );
      }

	   #endregion
	}
}
