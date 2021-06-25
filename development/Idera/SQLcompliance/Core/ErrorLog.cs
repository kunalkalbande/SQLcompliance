using System;
using System.Text;
using System.Diagnostics;
using Idera.SQLcompliance.Core.Rules.Alerts ;

namespace Idera.SQLcompliance.Core
{
	//-----------------------------------------------------------------------------------------
	// ErrorLog - Logging class
	//
	// * Singleton object
	// * Writes to event log or console depending on settings
	// * Set Level in Write(); only messages with value >= current level are written
	// * Version of Write() without level as a parameter are set to Level.Default
	//
	// Properties
	// ----------
	// ErrorLevel     - level of message to write
	// LogToEventLog  - toggle writing to event log
	// LogToConsole   - toggle writing to console
	// EventLogSource - Change event log source
	//
	// Write Variants
	// --------------
	// Note: All variants have optional first parameter: Level (default=Default)
	//       All variants have optional last parameter:  Severity (default=Informational)
	//       e.g. Write (str,errmsg)
	//            Write( Level.Default, str, errmsg, Severity.Error)
	//
	//  Write( string     errorString );
	//
	//  Write( string     description,
	//	        string     errorString );
	//
	//  Write( Exception  exception );
	//
	//	 Write( Exception  exception,
	//	        bool       showStackTrace );
	//
	//  Write( string     errorMsg,
	//	        Exception  exception );
	//
	//  Write( string     description,
	//         string     errorMsg,
	//	        Exception  exception );
   //
	//  Write( string     errorMsg,
	//	        Exception  exception,
	//         bool       showStackTrace );
	//
	//  Write( string     description,
	//         string     errorMsg,
	//	        Exception  exception,
	//         bool       showStackTrace );
	//
	// Example of how to use
	// ---------------------
	// Initialization
	//		ErrorLog.Instance.LogToEventLog  = true;
	//		ErrorLog.Instance.EventLogSource = CoreConstants.EventLogSource_CollectionService;
	//    ErrorLog.Instance.ErrorLevel     = Level.Default;
	//
	// Usage   
 	//    ErrorLog.Instance.Write( Level.Debug,
 	//                             errmsg,
   //                             ErrorLog.Severity.Informational );
	//-----------------------------------------------------------------------------------------
	public class ErrorLog
	{		
	   #region Enums
	   
		public enum Level : int
		{
			Always     = 0,
			Default    = 1,
			Verbose    = 2,
			Debug      = 3,
			UltraDebug = 4
		}
	
	   // event log severity	
   	public enum Severity
		{
			Informational,
			Warning,
			Error
		}

	   
	   #endregion

      #region Constructor

		// Static constructor to create singleton instance
		static ErrorLog()
		{
			instance = new ErrorLog();
			eventLog = new EventLog( "", /* Default: Application - not specify so it works in other languages */
			                         ".",
			                         CoreConstants.EventLogSource);
		}

		// The singleton instance of this class
		private static ErrorLog instance;

		// Private constructor to prevent code from creating additional instances of singleton
		private ErrorLog() {}
		public static ErrorLog Instance
		{
			get { return instance; }
		}

      #endregion
	   
	   #region Public Properties

		// The Windows event log
		private static EventLog eventLog;

		// The severity of the error log entry
		// Logging Level - errors only written that are >= this level
		private static Level errorLevel = Level.Default;
		public Level  ErrorLevel
		{
		   get { return errorLevel; }
		   set { errorLevel = value; } 
		}

		// Write error log entries to Windows event log?
		// Can be toggled by application</remarks>
		private bool logToEventLog;
		public bool LogToEventLog {
			get {
				return logToEventLog;
			}
			set {
				logToEventLog = value;
			}
		}

		// Change event log source
		// Can be toggled by application</remarks>
		public string EventLogSource {
			get {
				return eventLog.Source;
			}
			set {
            if(value != null && value.Length > 0)
            {
               try
               {
                  // Since we mini-deploy, we need to clean up old EventLog entries.  This
                  //  is normally done by the .NET framework, but our framework can change locations.
                  if(EventLog.SourceExists(value))
                     EventLog.DeleteEventSource(value) ;
               }
               catch
               {
               }
            }
				eventLog.Source = value;
			}
		}
		
		// Write error log entries to console?
		// Can be toggled by application
		private bool logToConsole;
		public bool LogToConsole {
			get {
				return logToConsole;
			}
			set {
				logToConsole = value;
			}
		}
		
		#endregion
		
      #region Write() - ONE STRING + OPTIONAL SEVERITY
		//---------------------------------------------------------------------------
		// Write - Error string; severity set to Informational
		//---------------------------------------------------------------------------
		public void
		   Write(
		      string            errorString
	      )
		{
			Write( Level.Default,
			       errorString,
			       Severity.Informational);
		}
		
		//---------------------------------------------------------------------------
		// Write -  - Error string; severity set to Informational
		//---------------------------------------------------------------------------
		public void
		   Write(
		      Level       level,
		      string            errorString
         )
		{
			Write( level,
			       errorString,
			       Severity.Informational);
		}
		
		//---------------------------------------------------------------------------
		// Write - Error string and severity
		//---------------------------------------------------------------------------
		public void
		   Write(
		      string            errorString,
		      Severity          severity
         )
      {
	      Write( Level.Default,
	             errorString,
	             severity );
      }
      
      #endregion
      
      #region One String BUT ALSO THE REAL UNDERLYING WRITE OF ALL WRITES

		//---------------------------------------------------------------------------
		// Write - Error string and severity
		//---------------------------------------------------------------------------
		public void
		   Write(
		      Level             level,
		      string            errorString,
		      Severity          severity
	      )
		{
		   if ( level > ErrorLevel ) return;
		   
			try
			{
				if (errorString != null && !errorString.Equals("")) 
				{

					if (logToEventLog) 
					{
						if (severity == Severity.Informational) 
						{
							eventLog.WriteEntry( errorString, 
							                     EventLogEntryType.Information,
							                     CoreConstants.EventId_SQLcompliance + (int)level);
						} 
						else if (severity == Severity.Warning) 
						{
							eventLog.WriteEntry( errorString,
							                     EventLogEntryType.Warning,
							                     CoreConstants.EventId_SQLcompliance + (int)level);
						} 
						else if (severity == Severity.Error) 
						{
							eventLog.WriteEntry( errorString,
							                     EventLogEntryType.Error,
							                     CoreConstants.EventId_SQLcompliance + (int)level);
						}
					}

					if (logToConsole) 
					{
						Console.WriteLine(errorString);
					}

				}
			}
			catch
			{
				// We do nothing here, just don't want to shut down the app when the event log is full
			}

		}
		
		#endregion
		
      #region Write() - TWO STRINGS + OPTIONAL SEVERITY
		//---------------------------------------------------------------------------
		// Write - Error string; severity set to Informational
		//---------------------------------------------------------------------------
		public void
		   Write(
		      string            description,
		      string            errorString
	      )
		{
	      Write( Level.Default,
	             description,
	             errorString,
	             Severity.Informational );
		}
		
		//---------------------------------------------------------------------------
		// Write -  - Error string; severity set to Informational
		//---------------------------------------------------------------------------
		public void
		   Write(
		      Level       level,
		      string            description,
		      string            errorString
         )
		{
	      Write( level,
	             description,
	             errorString,
	             Severity.Informational );
		}
		
		//---------------------------------------------------------------------------
		// Write - Error string and severity
		//---------------------------------------------------------------------------
		public void
		   Write(
		      string            description,
		      string            errorString,
		      Severity          severity
         )
      {
	      Write( Level.Default,
	             description,
	             errorString,
	             severity );
      }

		//---------------------------------------------------------------------------
		// Write - Error string and severity
		//---------------------------------------------------------------------------
		public void
		   Write(
		      Level       level,
		      string            description,
		      string            errorString,
		      Severity          severity
	      )
	   {
			string msg;
			
			msg = String.Format( "{0}\n{1}\n{2}",
			                     CoreConstants.ErrorPrefix,
			                     description,
			                     errorString );
			Write( level,
			       msg,
			       severity);
	   }
		
		#endregion
		
		#region Write() - EXCEPTION + OPTIONAL SEVERITY

		//---------------------------------------------------------------------------
		// Write - Write an exception's message and exception string to the error log.
		//         Error severity is defaulted to ErrorLog.Severity.Error.
		//---------------------------------------------------------------------------
		public void
		   Write(
		      Exception         exception
	      )
	   {
			Write( Level.Default,
			       "",
			       exception,
			       Severity.Error);
	   }
	   
		//---------------------------------------------------------------------------
		// Write - Write an exception's message and exception string to the error log.
		//         Error severity is defaulted to ErrorLog.Severity.Error.
		//---------------------------------------------------------------------------
		public void
		   Write(
		      Level        level,
		      Exception          exception
	      )
		{
			Write( level,
			       "",
			       exception,
			       Severity.Error);
		}


		//---------------------------------------------------------------------------
		// Write - Write an exception's message and exception string to the error log
		//---------------------------------------------------------------------------
		public void
		   Write(
		      Exception         exception,
		      Severity          severity
		   )
	   {
         Write( Level.Default,
                "",
                exception,
                severity );
	   }
	   
		//---------------------------------------------------------------------------
		// Write - Write an exception's message and exception string to the error log
		//---------------------------------------------------------------------------
		public void
		   Write(
            Level       level,
		      Exception         exception,
		      Severity          severity
		   )
	   {
	      Write( level,
	             "",
	             exception,
	             severity );
		}
		
		#endregion
		
		#region Write() - EXCEPTION WITH OPTIONAL STACK TRACE

		//---------------------------------------------------------------------------
		// Write - Writes an exception to the error log, optionally displaying
		//         the associated stack trace
		//---------------------------------------------------------------------------
		public void
		   Write(
		      Exception         exception,
		      bool              showStackTrace
	      )
	   {
         Write( Level.Default, exception , showStackTrace );
	   }
	      
		//---------------------------------------------------------------------------
		// Write - Writes an exception to the error log, optionally displaying
		//         the associated stack trace
		//---------------------------------------------------------------------------
		public void
		   Write(
		      Level        level,
		      Exception         exception,
		      bool              showStackTrace
	      )
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("Exception message: "+ exception.Message);
			if (showStackTrace) {
				builder.Append("\nStack Trace:\n"+ exception.StackTrace);
			}
			Write(level, builder.ToString(), Severity.Error);
		}
		
		#endregion
		
		#region Write() - STRING + EXCEPTION + optional SEVERITY

		//---------------------------------------------------------------------------
		// Write - Adds app specific message to exception before writing
		//---------------------------------------------------------------------------
		public void
	      Write(
	         string            errorMsg,
	         Exception         exception
	      )
	   {
			Write( Level.Default,
	             "", 
			       errorMsg,
			       exception,
			       Severity.Error,
			       false);
		}
		
		//---------------------------------------------------------------------------
		// Write - Adds app specific message to exception before writing
		//---------------------------------------------------------------------------
		public void
	      Write(
	         Level       level,
	         string            errorMsg,
	         Exception         exception
	      )
	   {
			Write( level,
	             "", 
			       errorMsg,
			       exception,
			       Severity.Error,
			       false);
		}
		//---------------------------------------------------------------------------
		// Write - Adds app specific message to exception before writing
		//---------------------------------------------------------------------------
		public void
	      Write(
	         string            errorMsg,
	         Exception         exception,
	         Severity          severity
	      )
	   {
			Write( Level.Default,
	             "", 
			       errorMsg,
			       exception,
			       severity,
			       false );
	   }
	   

		//---------------------------------------------------------------------------
		// Write - Adds app specific message to exception before writing
		//---------------------------------------------------------------------------
		public void
	      Write(
	         Level       level,
	         string            errorMsg,
	         Exception         exception,
	         Severity          severity
	      )
	   {
	      Write( level,
	             "", 
	             errorMsg,
	             exception,
	             severity,
	             false );
		}
		
		#endregion
		
		#region Write() - TWO STRING + EXCEPTION + optional SEVERITY

		//---------------------------------------------------------------------------
		// Write - Adds app specific message to exception before writing
		//---------------------------------------------------------------------------
		public void
	      Write(
	         string            description,
	         string            errorMsg,
	         Exception         exception
	      )
	   {
			Write( Level.Default,
			       description,
			       errorMsg,
			       exception,
			       Severity.Error,
			       false);
		}
		
		//---------------------------------------------------------------------------
		// Write - Adds app specific message to exception before writing
		//---------------------------------------------------------------------------
		public void
	      Write(
	         Level             level,
	         string            description,
	         string            errorMsg,
	         Exception         exception
	      )
	   {
			Write( level,
			       description,
		          errorMsg,
		          exception,
		          Severity.Error,
		          false );
		}
		
		//---------------------------------------------------------------------------
		// Write - Adds app specific message to exception before writing
		//---------------------------------------------------------------------------
		public void
	      Write(
	         string            description,
	         string            errorMsg,
	         Exception         exception,
	         Severity          severity
	      )
	   {
			Write( Level.Default,
			       description,
			       errorMsg,
			       exception,
			       severity,
			       false );
	   }
	   
		//---------------------------------------------------------------------------
		// Write - Adds app specific message to exception before writing
		//---------------------------------------------------------------------------
		public void
	      Write(
	         Level             level,
	         string            description,
	         string            errorMsg,
	         Exception         exception,
	         Severity          severity
	      )
	   {
			Write( level,
			       description,
			       errorMsg,
			       exception,
			       severity,
			       false );
	   }
	   
	   #endregion
	   
	   #region one strings exceptions, severity and optional stack trace

		//---------------------------------------------------------------------------
		// Write - Adds app specific message to exception before writing
		//---------------------------------------------------------------------------
		public void
	      Write(
	         string            errorMsg,
	         Exception         exception,
	         bool              showStackTrace
	      )
	   {
			Write( Level.Default,
			       "",
			       errorMsg,
			       exception,
			       Severity.Error,
			       showStackTrace );
	   }

	   
		//---------------------------------------------------------------------------
		// Write - Adds app specific message to exception before writing
		//---------------------------------------------------------------------------
		public void
	      Write(
	         Level             level,
	         string            errorMsg,
	         Exception         exception,
	         bool              showStackTrace
	      )
	   {
			Write( level,
			       "",
			       errorMsg,
			       exception,
			       Severity.Error,
			       showStackTrace );
	   }


		//---------------------------------------------------------------------------
		// Write - Adds app specific message to exception before writing
		//---------------------------------------------------------------------------
		public void
	      Write(
	         string            errorMsg,
	         Exception         exception,
	         Severity          severity,
	         bool              showStackTrace
	      )
	   {
			Write( Level.Default,
			       "",
			       errorMsg,
			       exception,
			       severity,
			       showStackTrace );
	   }
	   
		//---------------------------------------------------------------------------
		// Write - Adds app specific message to exception before writing
		//---------------------------------------------------------------------------
		public void
	      Write(
	         Level             level,
	         string            errorMsg,
	         Exception         exception,
	         Severity          severity,
	         bool              showStackTrace
	      )
	   {
			Write( level,
			       "",
			       errorMsg,
			       exception,
			       severity,
			       showStackTrace );
	   }

	   
	   #endregion
	   
	   #region two strings, exceptions, severity and optional stack trace

		//---------------------------------------------------------------------------
		// Write - Adds app specific message to exception before writing
		//---------------------------------------------------------------------------
		public void
	      Write(
	         string            description,
	         string            errorMsg,
	         Exception         exception,
	         bool              showStackTrace
	      )
	   {
			Write( Level.Default,
			       description,
			       errorMsg,
			       exception,
			       Severity.Error,
			       showStackTrace );
	   }

	   
		//---------------------------------------------------------------------------
		// Write - Adds app specific message to exception before writing
		//---------------------------------------------------------------------------
		public void
	      Write(
	         Level             level,
	         string            description,
	         string            errorMsg,
	         Exception         exception,
	         bool              showStackTrace
	      )
	   {
			Write( level,
			       description,
			       errorMsg,
			       exception,
			       Severity.Error,
			       showStackTrace );
	   }


		//---------------------------------------------------------------------------
		// Write - Adds app specific message to exception before writing
		//---------------------------------------------------------------------------
		public void
	      Write(
	         string            description,
	         string            errorMsg,
	         Exception         exception,
	         Severity          severity,
	         bool              showStackTrace
	      )
	   {
			Write( Level.Default,
			       description,
			       errorMsg,
			       exception,
			       severity,
			       false );
	   }
	   
	   #endregion
	   
	   
	   #region Write - actual string builder called by many of above!

		//---------------------------------------------------------------------------
		// Write - Adds app specific message to exception before writing
		//---------------------------------------------------------------------------
		public void
	      Write(
	         Level             level,
	         string            description,
	         string            errorMsg,
	         Exception         exception,
	         Severity          severity,
	         bool              showStackTrace
	      )
	   {
		   if ( level > ErrorLevel ) return;
		   
			StringBuilder builder = new StringBuilder();
			
			builder.Append( description );
			
			if ( errorMsg != "" )
			{
			   builder.AppendFormat( "\n{0}{1}",
			                         CoreConstants.ErrorPrefix,
			                         errorMsg );
         }
                               
			builder.AppendFormat( "\n{0}{1}",
			                      CoreConstants.ExceptionPrefix,
			                      exception.Message );

			if ( level > Level.Default )
			{
			   if ( exception.TargetSite != null )
			   {
			      builder.AppendFormat( "\n{0}{1}",	
			                           CoreConstants.DetailsPrefix,
			                           exception.TargetSite );
			   }
			}
			
			if (showStackTrace)
			{
				builder.Append("\nStack Trace:\n"+ exception.StackTrace);
			}
			
			Write( level,
			       builder.ToString(),
			       severity);	
		}
		
		#endregion

      #region WriteAlert

      public void WriteAlert(Alert alert, Severity severity)
      {
         string oldSource = eventLog.Source ;
         if(alert == null)
            return ;
         try
         {
            if (alert.MessageBody != null && !alert.MessageBody.Equals("")) 
            {

               if (logToEventLog) 
               {
                  eventLog.Source = CoreConstants.EventLogSource_Alerting ;
                  if (severity == Severity.Informational) 
                  {
                     eventLog.WriteEntry( alert.MessageBody, EventLogEntryType.Information,
                        alert.EventType) ;
                  } 
                  else if (severity == Severity.Warning) 
                  {
                     eventLog.WriteEntry( alert.MessageBody, EventLogEntryType.Warning,
                        alert.EventType) ;
                  } 
                  else if (severity == Severity.Error) 
                  {
                     eventLog.WriteEntry( alert.MessageBody, EventLogEntryType.Error,
                        alert.EventType) ;
                  }
               }
            }
         }
         catch
         {
            // We do nothing here, just don't want to shut down the app when the event log is full
         }
         finally
         {
            eventLog.Source = oldSource ;
         }
      }

      public void WriteAlert(StatusAlert alert, Severity severity)
      {
         string oldSource = eventLog.Source;
         if (alert == null)
            return;
         try
         {
            if (alert.MessageBody != null && !alert.MessageBody.Equals(""))
            {

               if (logToEventLog)
               {
                  eventLog.Source = CoreConstants.EventLogSource_Alerting;
                  if (severity == Severity.Informational)
                  {
                     eventLog.WriteEntry(alert.MessageBody, EventLogEntryType.Information, (int)alert.EventType);
                  }
                  else if (severity == Severity.Warning)
                  {
                     eventLog.WriteEntry(alert.MessageBody, EventLogEntryType.Warning, (int)alert.EventType);
                  }
                  else if (severity == Severity.Error)
                  {
                     eventLog.WriteEntry(alert.MessageBody, EventLogEntryType.Error, (int)alert.EventType);
                  }
               }
            }
         }
         catch
         {
            // We do nothing here, just don't want to shut down the app when the event log is full
         }
         finally
         {
            eventLog.Source = oldSource;
         }
      }

      public void WriteAlert(DataAlert alert, Severity severity)
      {
         string oldSource = eventLog.Source;
         if (alert == null)
            return;
         try
         {
            if (alert.MessageBody != null && !alert.MessageBody.Equals(""))
            {

               if (logToEventLog)
               {
                  eventLog.Source = CoreConstants.EventLogSource_Alerting;
                  if (severity == Severity.Informational)
                  {
                     eventLog.WriteEntry(alert.MessageBody, EventLogEntryType.Information, (int)alert.EventType);
                  }
                  else if (severity == Severity.Warning)
                  {
                     eventLog.WriteEntry(alert.MessageBody, EventLogEntryType.Warning, (int)alert.EventType);
                  }
                  else if (severity == Severity.Error)
                  {
                     eventLog.WriteEntry(alert.MessageBody, EventLogEntryType.Error, (int)alert.EventType);
                  }
               }
            }
         }
         catch
         {
            // We do nothing here, just don't want to shut down the app when the event log is full
         }
         finally
         {
            eventLog.Source = oldSource;
         }
      }

      #endregion // WriteAlert
		
	}
}
