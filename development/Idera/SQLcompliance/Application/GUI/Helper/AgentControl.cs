using System;
using System.Text;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

using Microsoft.Win32;
//using Microsoft.Samples.Runtime.Remoting.Security;

using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Service;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
	/// <summary>
	/// Contains static utility methods used by SQLsecure applications.
	/// </summary>
	/// <remarks>Constructor is private, so this class cannot be instantiated.</remarks>
	public sealed class AgentControl
	{		

		#region Constructor (private)

		private AgentControl()
		{}

		#endregion

		#region Start / Stop

      //-----------------------------------------------------------------------
      // RemoteStartAgentService
      //-----------------------------------------------------------------------
		public static bool
		   RemoteStartAgentService(
		      string            localAdmin,
		      string            localPassword,
            string            targetServer,
		      string            remoteAdmin,
		      string            remotePassword
 		   )
		{
		   bool started = false;
		   
			// Instantiate the SQLsecure service manager
			AgentServiceManager agentServiceManager
			   = new AgentServiceManager( localAdmin,
		                                  localPassword,
                                        targetServer,
		                                  remoteAdmin,
		                                  remotePassword );

			try
			{
				// Attempt to start the SQLsecure service
				started = agentServiceManager.Start();
				
				if ( started ) ServerRecord.UpdateLastAgentContact(true, targetServer);
			}
			catch (System.Runtime.InteropServices.COMException COMe)
			{
				// Server is not responding
				throw new CoreException(CoreConstants.Exception_ServerNotResponding, COMe);
			}	
			
			return started;
		}

      //-----------------------------------------------------------------------
      // RemoteStopAgentService
      //-----------------------------------------------------------------------
		public static bool
		   RemoteStopAgentService(
		      string            localAdmin,
		      string            localPassword,
            string            targetServer,
		      string            remoteAdmin,
		      string            remotePassword
 		   )
		{
		   bool stopped = false;
		   
			// Instantiate the Agent service manager
			AgentServiceManager agentServiceManager
			   = new AgentServiceManager( localAdmin,
		                                 localPassword,
                                       targetServer,
		                                 remoteAdmin,
		                                 remotePassword );

			try
			{
				// Attempt to stop the SQLsecure service
				stopped = agentServiceManager.Stop();
				
				if ( stopped ) ServerRecord.UpdateLastAgentContact(true, targetServer);
			}
			catch (System.Runtime.InteropServices.COMException COMe)
			{
				// Server is not responding
				throw new CoreException(CoreConstants.Exception_ServerNotResponding, COMe);
			}	
			
			return stopped;
		}
		
		#endregion
		
		#region Install / Upgrade / Uninstall

      //-----------------------------------------------------------------------
      // RemoteInstallAgentService
      //-----------------------------------------------------------------------
		public static void
		   RemoteInstallAgentService(
		      string localAdmin,
		      string localPassword,
            string targetServer,
			string            remoteAdmin,
			string            remotePassword,
			string serviceAccount,
		      string servicePassword,
		      string instance,
		      string traceDirectory,
		      string collectionServer
		   )
		{
			// Instantiate the SQLsecure service manager
			AgentServiceManager agentServiceManager
			   = new AgentServiceManager( localAdmin,
		                                 localPassword,
                                       targetServer,
		                                 remoteAdmin,
		                                 remotePassword );
			try
			{
				agentServiceManager.Install( serviceAccount,
				                             servicePassword,
				                             instance,
				                             traceDirectory,
				                             targetServer,
				                             collectionServer );
			}
			catch (System.Runtime.InteropServices.COMException COMe)
			{
				// Server is not responding
				throw new CoreException(CoreConstants.Exception_ServerNotResponding, COMe);
			}	
		}

      //-----------------------------------------------------------------------
      // RemoteUpgradeAgentService
      //-----------------------------------------------------------------------
		public static void RemoteMajorUpgradeAgentService(
            string            localAdmin,
            string            localPassword,
            string            targetServer,
            string            remoteAdmin,
            string            remotePassword,
            string serviceAccount,
            string servicePassword)
		{
			// Instantiate the SQLsecure service manager
			AgentServiceManager agentServiceManager
			   = new AgentServiceManager( localAdmin,
		                                  localPassword,
                                        targetServer,
		                                  remoteAdmin,
		                                  remotePassword );

			try
			{
				// Upgrade the SQLsecure service
				agentServiceManager.MajorUpgrade(serviceAccount, servicePassword) ;
			}
			catch (System.Runtime.InteropServices.COMException COMe)
			{
				// Server is not responding
				throw new CoreException(CoreConstants.Exception_ServerNotResponding, COMe);
			}	
		}

      //-----------------------------------------------------------------------
      // RemoteUpgradeAgentService
      //-----------------------------------------------------------------------
      public static void
         RemoteMinorUpgradeAgentService(
         string            localAdmin,
         string            localPassword,
         string            targetServer,
         string            remoteAdmin,
         string            remotePassword,
         string            collectionServer
         )
      {
         // Instantiate the SQLsecure service manager
         AgentServiceManager agentServiceManager
            = new AgentServiceManager( localAdmin,
            localPassword,
            targetServer,
            remoteAdmin,
            remotePassword );

         try
         {
            // Upgrade the SQLsecure service
             agentServiceManager.MinorUpgrade(collectionServer);
         }
         catch (System.Runtime.InteropServices.COMException COMe)
         {
            // Server is not responding
            throw new CoreException(CoreConstants.Exception_ServerNotResponding, COMe);
         }	
      }

      //-----------------------------------------------------------------------
      // RemoteUninstallAgentService
      //-----------------------------------------------------------------------
		public static void
		   RemoteUninstallAgentService(
		      string            localAdmin,
		      string            localPassword,
            string            targetServer,
			string            remoteAdmin,
			string            remotePassword
			)
		{
			// Instantiate the SQLsecure service manager
			AgentServiceManager agentServiceManager
			   = new AgentServiceManager( localAdmin,
		                                  localPassword,
                                        targetServer,
		                                  remoteAdmin,
		                                  remotePassword );

			// Uninstall the service (which will stop it)
			agentServiceManager.Uninstall();				
		}

		#endregion

	}
}
