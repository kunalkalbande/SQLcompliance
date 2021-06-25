using System;
using System.Collections;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Status;
using System.Collections.Generic;

namespace Idera.SQLcompliance.Core.Agent
{
	/// <summary>
	/// Summary description for TamperingDetector.
	/// </summary>
	public class TamperingDetector : ServiceTimer
	{
      #region Data members

      private string instanceName;
      private SQLInstance sqlInstance;
      private bool checking = false;
      private object syncObj = new object();
      private int configFileErrors = 0;

      #endregion

      #region Constructors

		public TamperingDetector( SQLInstance instance ) : base(instance.DetectionInterval * 1000)
		{
         sqlInstance = instance;
         instanceName = instance.Name;
         TimerCallbackHandler = new TimerCallback(Check) ;
		}

      #endregion

      #region Public methods

      //------------------------------------------------------------------
      // Start
      //------------------------------------------------------------------
      /// <summary>
      /// Start trace tampering detection.
      /// </summary>
      public override void 
         Start()
      {
         base.Start ();
         ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
            "Tampering detector for  " + instanceName + " started.");
      }

      //------------------------------------------------------------------
      // Stop
      //------------------------------------------------------------------
      /// <summary>
      /// Stop detection
      /// </summary>
      public override void 
         Stop()
      {
         base.Stop();
         ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
            "Tampering detector for " + instanceName + " stopped." );
      }

      public void 
         Check (
            object obj
         )
      {
         CheckTraceStatus();
      }

      public bool
         CheckTraceStatus()
      {
         return CheckTraceStatus( true );
      }

      public bool
         CheckTraceStatus( bool restartTraces )
      {
         return CheckTraceStatus( restartTraces, true );
      }

      public bool
         CheckTraceStatus( bool restartTraces, bool lockInstance )
      {
         bool ok = true;
         if( !sqlInstance.IsEnabled )
            return ok;
            
         int startTime = Environment.TickCount;

         if( SQLcomplianceAgent.Instance.MaxFolderSizeReached )
            return ok;

         // Check if there is another detection thread running
         lock( syncObj )
         {
            if( checking )
            {
               ErrorLog.Instance.Write( ErrorLog.Level.Verbose, 
                                        "There is already a detector thread running for " + sqlInstance.Name );
               return ok;
            }
            checking = true;
         }
         
         object lockObject;

         // lock the instance object so no other threads change the trace info
         // during tampering detection
         if (lockInstance)
            lockObject = sqlInstance.syncObj;
         else 
            lockObject = new object();

         lock (lockObject)
         {
            try
            {
               ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                        String.Format("Detecting trace tampering for {0}.  Restart traces = {1}.", 
                                                      sqlInstance.Name, restartTraces ) );
               ok = Perform( restartTraces );
               ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                        String.Format("Tampering detection for {0} finished ({1}ms).", 
                                                      sqlInstance.Name, Environment.TickCount - startTime ) );
               if( sqlInstance.TamperingDetectionExceptionFound > 0 )
               {
                  ReportTamperingDetectionResolution();
               }
            }
            catch( ConnectionFailedException cfe )
            {
               // This exception is handled by the sql connection errors.
               if ((sqlInstance.TamperingDetectionExceptionFound++ %
                    sqlInstance.ExceptionReportInterval == 0))
               {
                  ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                     CoreConstants.Exception_TraceTamperingDetectionError,
                     cfe,
                     ErrorLog.Severity.Warning,
                     true);
               }
            }
            catch( Exception e )
            {
               ReportTamperingDetectionError( e );
            }
            finally
            {
               lock( syncObj )
               {
                  checking = false;
               }
            }
         }

         return ok;
      }

      #endregion

      #region Private methods

      //---------------------------------------------------------------------------
      // Perform : check the trace status
      //---------------------------------------------------------------------------
      private bool Perform( bool restartTraces )
      {
         TraceInfo[] currentTraces = null;
         bool ok = true;

         List<TraceInfo>  agentTraces = new List<TraceInfo>();
         agentTraces.AddRange(TraceInfo.GetAgentStartedTraceInfo(sqlInstance.Alias));
         TraceInfo[] agentTracesXE = TraceInfo.GetAgentStartedTraceInfoXE(sqlInstance.Alias);
         TraceInfo[] agentAuditLogs = TraceInfo.GetAgentStartedAuditLogsInfo(sqlInstance.Alias);         
         if(agentAuditLogs != null)
             agentTraces.AddRange(agentAuditLogs);
         using( SqlConnection conn = sqlInstance.GetConnection() )
         {
            int [] runningTraces = SPHelper.GetRunningTraces( conn );
            if( runningTraces != null && runningTraces.Length > 0 )
            {
               StringBuilder idList = new StringBuilder();
               
               for( int i = 0; i < runningTraces.Length; i++ )
                  idList.AppendFormat("{0},", runningTraces[i]);
               
               currentTraces = SPHelper.GetTraceInfo(conn, idList.ToString());
            }
         }
         
         Hashtable traces = new Hashtable();
         
         if( currentTraces != null )
            foreach( TraceInfo trace in currentTraces )
               traces.Add(trace.TraceId, trace);


        
         using (SqlConnection conn = sqlInstance.GetConnection())
         {
             currentTraces = null;
             int[] runningTraces = SPHelper.GetRunningTracesXE(conn);
             if (runningTraces != null && runningTraces.Length > 0)
             {
                 StringBuilder idList = new StringBuilder();

                 for (int i = 0; i < runningTraces.Length; i++)
                     idList.AppendFormat("{0},", runningTraces[i]);

                 currentTraces = SPHelper.GetTraceInfoXE(conn, idList.ToString());
             }
         }


         if (currentTraces != null)
             foreach (TraceInfo trace in currentTraces)
                 traces.Add(trace.TraceId, trace);

         using (SqlConnection conn = sqlInstance.GetConnection())
         {
             currentTraces = null;
             int[] runningTraces = SPHelper.GetRunningAuditLogs(conn);
             if (runningTraces != null && runningTraces.Length > 0)
             {
                 StringBuilder idList = new StringBuilder();

                 for (int i = 0; i < runningTraces.Length; i++)
                     idList.AppendFormat("{0},", runningTraces[i]);

                 currentTraces = SPHelper.GetAuditLogsInfo(conn, idList.ToString());
             }
         }

         if (currentTraces != null)
             foreach (TraceInfo trace in currentTraces)
                 traces.Add(trace.TraceId, trace);

         if( agentTraces != null )
         {
            bool stopped = false;
            bool closed = false;
            bool altered = false;
                  
            for( int i = 0; i < agentTraces.Count; i++ )
            {
               TraceInfo trace = (TraceInfo) traces[agentTraces[i].TraceId];
               if( trace == null )
               {
                  // Trace is closed
                  if( ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Verbose 
                   || !sqlInstance.TamperingDetectionErrorFound )
                     ErrorLog.Instance.Write( String.Format( CoreConstants.Warning_TraceClosed, agentTraces[i]),
                                             ErrorLog.Severity.Warning );
                  closed = true;
               }
               else if( trace.Status != TraceStatus.Start )
               {
                  // Trace is Stopped
                  if (ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Verbose
                   || !sqlInstance.TamperingDetectionErrorFound)
                     ErrorLog.Instance.Write(String.Format(CoreConstants.Warning_TraceStopped, agentTraces[i]),
                                          ErrorLog.Severity.Warning );
                  
                  // close the trace so it can be restarted later
                  using( SqlConnection conn = sqlInstance.GetConnection() )
                  {
                     SPHelper.StopTrace( conn, trace.TraceId );
                  }
                  stopped = true;
               }
               else // Check if the trace is altered.
               {
                  altered = IsTraceAltered( trace );
               }
            }
            
            if( !sqlInstance.TamperingDetectionErrorFound )
            {
               if ( stopped )
                  SQLcomplianceAgent.Instance.SendTraceTamperedMessage( instanceName,
                                                                        AgentStatusMsg.
                                                                           MsgType.
                                                                           TraceStopped );

               if ( closed )
                  SQLcomplianceAgent.Instance.SendTraceTamperedMessage( instanceName,
                                                                        AgentStatusMsg.
                                                                           MsgType.
                                                                           TraceClosed );

               if ( altered )
                  SQLcomplianceAgent.Instance.SendTraceTamperedMessage( instanceName,
                                                                        AgentStatusMsg.
                                                                           MsgType.
                                                                           TraceAltered );
            }

            if( stopped || closed || altered ) 
            {
               //ReportTraceTamperedError();
               ok = false;               
            }
         }
         if (agentTracesXE != null)
         {
             bool stopped = false;
             bool closed = false;
             bool altered = false;

             for (int i = 0; i < agentTracesXE.Length; i++)
             {
                 TraceInfo trace = (TraceInfo)traces[agentTracesXE[i].TraceId];
                 if (trace == null)
                 {
                     // Trace is closed
                     if (ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Verbose
                      || !sqlInstance.TamperingDetectionErrorFound)
                         ErrorLog.Instance.Write(String.Format(CoreConstants.Warning_TraceClosed, agentTracesXE[i]),
                                                 ErrorLog.Severity.Warning);
                     closed = true;
                 }
                 else if (trace.Status != TraceStatus.Start)
                 {
                     // Trace is Stopped
                     if (ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Verbose
                      || !sqlInstance.TamperingDetectionErrorFound)
                         ErrorLog.Instance.Write(String.Format(CoreConstants.Warning_TraceStopped, agentTraces[i]),
                                              ErrorLog.Severity.Warning);

                     // close the trace so it can be restarted later
                     using (SqlConnection conn = sqlInstance.GetConnection())
                     {
                         if (SPHelper.DoesSPExist(conn, CoreConstants.Agent_AuditSPNameXE))
                            SPHelper.StopTraceXE(conn, trace.TraceId);
                     }
                     stopped = true;
                 }
                 else // Check if the trace is altered.
                 {
                     altered = IsTraceAltered(trace);
                 }
             }

             if (!sqlInstance.TamperingDetectionErrorFound)
             {
                 if (stopped)
                     SQLcomplianceAgent.Instance.SendTraceTamperedMessage(instanceName,
                                                                           AgentStatusMsg.
                                                                              MsgType.
                                                                              TraceStopped);

                 if (closed)
                     SQLcomplianceAgent.Instance.SendTraceTamperedMessage(instanceName,
                                                                           AgentStatusMsg.
                                                                              MsgType.
                                                                              TraceClosed);

                 if (altered)
                     SQLcomplianceAgent.Instance.SendTraceTamperedMessage(instanceName,
                                                                           AgentStatusMsg.
                                                                              MsgType.
                                                                              TraceAltered);
             }

             if (stopped || closed || altered)
             {
                 //ReportTraceTamperedError();
                 ok = false;
             }
         }
         if (ok)
         {
             if (restartTraces)
             {
                 ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "Traces tampered.  Restart traces.");
                 sqlInstance.RestartTraces();
             }
             else
             {
                 ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "Traces tampered.");
             }
         }
         return ok;

      }

      //
      // Check if the given trace has been altered unexpectedly.
      //
      private bool IsTraceAltered( TraceInfo trace )
      {
         string traceFile = trace.FileName;
         bool tempAltered = false;
         FileInfo info = new FileInfo( traceFile );
         string binFileName = info.DirectoryName + @"\" + sqlInstance.Alias + ".bin";
         TraceConfiguration config = null ;        
         FileInfo fileInfo = new FileInfo( binFileName );
         
         if ( !fileInfo.Exists )
         {
               Exception error = new Exception( CoreConstants.Exception_MissingAuditConfigurationFile +
                                        binFileName);
               sqlInstance.ReportConfigurationError( error );
               configFileErrors++;
         }
         else
         {
            // Compare trace setting with the original settings
            config = ConfigurationHelper.GetTraceConfiguration(
               binFileName,
               traceFile,
               false,
               sqlInstance.Name,
               sqlInstance.SqlVersion );
         }

         string tmpFileName = Path.GetFileNameWithoutExtension(trace.FileName);
         if (tmpFileName.Contains("XE") || tmpFileName.StartsWith("AL")) // Need to be changed
         {
             return false;
         }
         if( config == null)
         {
            if ( configFileErrors % sqlInstance.ExceptionReportInterval == 0 )
            {
               Exception error = new Exception( String.Format(
                                                      "Trace tampering detection aborted. {0}.  Bin file: {1}.",
                                                      CoreConstants.Exception_ErrorReadingAuditConfiguratioin,
                                                      binFileName ) );
               sqlInstance.ReportConfigurationError(error);
               ErrorLog.Instance.Write(ErrorLog.Level.Default,
                  String.Format(
                     "Trace tampering detection aborted. {0}.  Bin file: {1}.",
                     CoreConstants.Exception_ErrorReadingAuditConfiguratioin,
                     binFileName ) );
            }
            configFileErrors++;
            return false;
         }
         else
         {
            sqlInstance.ReportConfigurationResolution();
         }
         
         TraceColumnId[] setColumns = config.GetTraceColumns();
         TraceEventId[] setEvents = config.GetTraceEvents();
         TraceFilter[] setFilters = config.GetTraceFilters();
         ArrayList events;
         ArrayList columns;
         ArrayList filters;
         
         using ( SqlConnection conn = sqlInstance.GetConnection() )
         {
            SPHelper.GetTraceSettings( conn,
                                       trace.TraceId,
                                       out events,
                                       out columns,
                                       out filters,
                                       sqlInstance.SqlVersion );
         }
         if ( events.Count != setEvents.Length ||
              columns.Count != setColumns.Length ||
              filters.Count != setFilters.Length )
            tempAltered = true;

         for ( int j = 0; j < events.Count && !tempAltered; j++ )
            if ( !events.Contains( setEvents[j] ) )
               tempAltered = true;
         for ( int j = 0; j < columns.Count && !tempAltered; j++ )
            if ( !columns.Contains( setColumns[j] ) )
               tempAltered = true;

         for ( int j = 0; j < filters.Count && !tempAltered; j++ )
         {
            bool found = false;

            for ( int k = 0; k < setFilters.Length && !found; k++ )
               if ( setFilters[k].Equals( (TraceFilter) filters[j] ) )
                  found = true;
            tempAltered = !found;
         }

         if ( tempAltered )
         {
            if (ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Verbose
             || !sqlInstance.TamperingDetectionErrorFound)
               ErrorLog.Instance.Write(
                  String.Format( CoreConstants.Warning_TraceAltered,
                                 trace ),
                  ErrorLog.Severity.Warning );
                  
            using ( SqlConnection conn = sqlInstance.GetConnection() )
            {
               SPHelper.StopTrace( conn, trace.TraceId );
               if (SPHelper.DoesSPExist(conn, CoreConstants.Agent_AuditSPNameXE))
                    SPHelper.StopTraceXE(conn, trace.TraceId);
            }
         }
         else
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     "Trace status checked." );
         return tempAltered;
      }

	   #endregion

      #region System Alerts
      
      //--------------------------------------------------------------------------
      private void
         ReportTraceTamperedError( )
      {
         ErrorLog.Level level = ErrorLog.Level.Debug;
         string details =
            String.Format("Traces for instance {0} are altered unexpectedly.",
                           sqlInstance.Name);

         if (!sqlInstance.TamperingDetectionErrorFound)
         {
            level = ErrorLog.Level.Default;
            sqlInstance.TamperingDetectionErrorFound = true;
            SystemAlert alert =
               new SystemAlert( SystemAlertType.SqlTraceError,
                                DateTime.UtcNow,
                                sqlInstance.Name,
                                details );
            sqlInstance.SubmitSystemAlert( alert );
         }
         ErrorLog.Instance.Write( level,
                                  details,
                                  ErrorLog.Severity.Warning);
      }

      //--------------------------------------------------------------------------
      private void
         ReportTraceTamperedResolution()
      {
         sqlInstance.TamperingDetectionErrorFound = false;
         string details = String.Format(  "Trace tampered condition cleared for {0}.  Traces are recreated.", sqlInstance.Name );
         ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                  details );
         SystemAlert alert =
            new SystemAlert(SystemAlertType.SqlTraceResolution,
                             DateTime.UtcNow,
                             sqlInstance.Name,
                             details);
         sqlInstance.SubmitSystemAlert(alert);
      }

      //--------------------------------------------------------------------------
      private void
         ReportTamperingDetectionError( Exception e)
      {
         string details = String.Format( "An error occurred during trace tampering check for instance {0}.  Error: {1}.",
                                         sqlInstance, e.Message );
         ErrorLog.Level level = ErrorLog.Level.Debug;
         if ((sqlInstance.TamperingDetectionExceptionFound++ %
              sqlInstance.ExceptionReportInterval == 0))
         {
            level = ErrorLog.Level.Default;
            SystemAlert alert =
               new SystemAlert(SystemAlertType.SqlTraceError,
                                DateTime.UtcNow,
                                sqlInstance.Name,
                                details);
            sqlInstance.SubmitSystemAlert(alert);
         }

         ErrorLog.Instance.Write( level,
            CoreConstants.Exception_TraceTamperingDetectionError,
            e,
            ErrorLog.Severity.Warning,
            true);

      }

      //--------------------------------------------------------------------------
      private void
         ReportTamperingDetectionResolution()
      {
         sqlInstance.TamperingDetectionExceptionFound = 0;
         string details = String.Format("Trace tampering detection exception conditions resolved for {0}.", sqlInstance.Name);
         ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                  details);
         SystemAlert alert =
            new SystemAlert(SystemAlertType.SqlTraceResolution,
                             DateTime.UtcNow,
                             sqlInstance.Name,
                             details);
         sqlInstance.SubmitSystemAlert(alert);
      }

      #endregion
	}
}
