using System;
using System.Net.Sockets;

//using Microsoft.Samples.Runtime.Remoting.Security;

using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Remoting;

namespace Idera.SQLcompliance.Core.Status
{
	/// <summary>
	/// The singleton instance of the status logger.
	/// Runs in the SQLsecure Agent process space.  
	/// Sends SQLsecure status messages to collection service
	/// </summary>
	internal class StatusLogger: IStatusLogger
	{
		#region Singleton constructors and properties

		/// <summary>
		/// The singleton instance of this class
		/// </summary>
		private static StatusLogger instance;
		private static object syncObj;

		/// <summary>
		/// Static constructor to create singleton instance
		/// </summary>
		static StatusLogger()
		{
			instance = new StatusLogger();
			syncObj = new object();
		}

		/// <summary>
		/// Private constructor to prevent code from creating additional instances of singleton
		/// </summary>
		private StatusLogger()
		{	
		}

		/// <summary>
		/// Internal property to provide access to singleton instance
		/// </summary>
		public static StatusLogger Instance {
			get {
				return instance;
			}
		}		

		#endregion		

		#region Delegates

		/// <summary>
		/// Delegate declaration for status logger
		/// </summary>
		public delegate  bool  LogStatusDelegate(AgentStatusMsg status);

		#endregion
		
		#region Public methods

		//-----------------------------------------------------------------------
      // SendStatus - Log a status event to remote collection service
		//-----------------------------------------------------------------------
		public bool []
		   SendStatus(
		      AgentStatusMsg       statusMsg
         )
      {
         bool [] updateNeeded = new bool[statusMsg.Config.InstanceStatusList.Length];
         bool [,] instanceStatus = SendStatusEx( statusMsg);

         if( instanceStatus == null )
         {
            // Something went wrong when sending the message.  
            // Initialize the bool array to false.
            for( int i = 0; i < updateNeeded.Length; i++ )
               updateNeeded[i] = false;
            return updateNeeded;
         }
            
         for( int i = 0; i < statusMsg.Config.InstanceStatusList.Length; i++ )
         {
            updateNeeded[i] = instanceStatus[i,0];
         }
			
			return updateNeeded;
		}

		//-----------------------------------------------------------------------
      // SendStatusEx - Log a status event to remote collection service
		//-----------------------------------------------------------------------
		public bool [,]
		   SendStatusEx(
		      AgentStatusMsg       statusMsg
         )
      {
         bool [,] updateNeeded = null;
         try
         {
            // Lock for the entire status logging operation
            lock (syncObj)
            {
               statusMsg.Config.UpdateTimeZoneInfo();
               RemoteStatusLogger remoteLogger;
					
               // Get the singleton remote status logger
               remoteLogger = GetRemoteLogger( statusMsg.Config.Server,
                  statusMsg.Config.ServerPort);

               // Log status remotely
               try
               {
                  updateNeeded = remoteLogger.SendStatusEx( statusMsg );
                  SQLcomplianceAgent.Instance.ReportCollectionServerConnectionResolution();
                  if (statusMsg.Type == AgentStatusMsg.MsgType.Startup ||
                     statusMsg.Type == AgentStatusMsg.MsgType.Heartbeat )
                     statusMsg.Config.DbSchemaVersionChecked = true;
                  StatusCache.Instance.Flush();
               }
               catch (Exception e)
               {
               
                  if( e.Message == CoreConstants.Exception_IncompatibleAgentRepositoryVersion )
                  {
                     throw new IncompatibleAgentRepositoryVersionException();
                  }

                  string msg = e.Message;
                  try
                  {
                     SocketException socketEx = (SocketException)e;
                     if ( socketEx.ErrorCode == 10061 )
                     {
                        msg = String.Format( CoreConstants.Exception_ServerNotAvailable,
                                             statusMsg.Config.Server );
                     }
                  }
                  catch {}

                  SQLcomplianceAgent.Instance.ReportCollectionServerConnectionError(e);                  
                  StatusCache.Instance.Save(statusMsg);
							
                  // The management service is not available - we dont really care - we will just handle later
               }		
               finally
               {
               }
            }
         }
         catch ( IncompatibleAgentRepositoryVersionException ie )
         {
            ErrorLog.Instance.Write( ie );
            throw ie;
         }
         catch (Exception e)
         {
            // We were probably called asynchronously, so nothing is around to catch the exception
            // but we still need to log it
            ErrorLog.Instance.Write( statusMsg.Type.ToString() + " status messaging error: "+ e.Message, ErrorLog.Severity.Warning);				
         }
			
			return updateNeeded;
		}

		//-----------------------------------------------------------------------
      // GetAuditSettings - Get updated copy of audit settigs from server
		//-----------------------------------------------------------------------
		public bool
		   GetAuditSettings(
		      AgentStatusMsg       statusMsg
         )
      {
         bool newCopy = false;
         
			try
			{
				// Lock for the entire status logging operation
				lock (syncObj)
				{
					RemoteStatusLogger remoteLogger;
					
					// Get the singleton remote status logger
					remoteLogger = GetRemoteLogger( statusMsg.Config.Server,
					                                statusMsg.Config.ServerPort);

					try
					{
						newCopy = remoteLogger.GetAuditSettings( statusMsg );
                  SQLcomplianceAgent.Instance.ReportCollectionServerConnectionResolution();
               }
					catch (Exception e)
					{
                  SQLcomplianceAgent.Instance.ReportCollectionServerConnectionError(e);							
						// The management service is not available - we dont really care - we will just handle later
					}		
				}
			}
			catch (Exception e)
			{
				// We were probably called asynchronously, so nothing is around to catch the exception
				// but we still need to log it
				ErrorLog.Instance.Write(e, ErrorLog.Severity.Warning);				
			}
			
			return newCopy;
		}



		#endregion

		#region Private methods
		
		internal static RemoteStatusLogger
		   GetRemoteLogger(
		      string               collectionServer,
		      int                  collectionServerPort
	      )
     {
         string remotingURL = EndPointUrlBuilder.GetUrl(CoreConstants.CollectionServerName, collectionServer, collectionServerPort);

         ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                  "Remoting " + remotingURL, 
                                  ErrorLog.Severity.Informational );
				                                 
			// Get the remote proxy for the logger singleton
         RemoteStatusLogger remoteLogger = CoreRemoteObjectsProvider.RemoteStatusLogger(collectionServer, collectionServerPort, CoreConstants.CollectionServerName);
			
			return remoteLogger;
		}

		#endregion

	}
}