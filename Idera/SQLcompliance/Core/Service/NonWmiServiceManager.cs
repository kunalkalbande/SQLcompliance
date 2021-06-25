using System;
using System.Text;
using System.IO;
using System.Management;
using System.Security.Principal;
using System.Data.SqlClient ;
using System.ServiceProcess;
using System.Reflection;

using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Security;
using Idera.SQLcompliance.Core.Collector ;

namespace Idera.SQLcompliance.Core.Service
{
	/// <summary>
	/// Control class for Collection Server service.
	/// </summary>
	public sealed class NonWmiServiceManager
	{
		#region Class-level variables

		private string           m_computer;     		                // The remote server that we're managing
		private string           m_serviceName;   		             // The service name 
		
		#endregion

		#region Constructor

      //-----------------------------------------------------------------------
      // Constructor - Instantiate the service manager.
      //-----------------------------------------------------------------------
		public
		   NonWmiServiceManager(
            string targetComputer,
            string targetService
		   )
		{
         int index = targetComputer.IndexOf("\\") ;
         if(index == -1)
			   this.m_computer    = targetComputer;
         else
            this.m_computer = targetComputer.Substring(0, index) ;
			this.m_serviceName = targetService;
		}		

		#endregion
		
		#region  Server Control Routines - Start/Stop

      //-----------------------------------------------------------------------
      // Start - Start the service on a remote server.
      //-----------------------------------------------------------------------
		public bool
		   Start()
		{
		   bool started = false;

         try
         {		   
			   if (ServiceInstalled())
			   {
               ServiceController sc  = new ServiceController();
               sc.ServiceName = m_serviceName;
               sc.MachineName = m_computer;

               if (sc.Status == ServiceControllerStatus.Stopped)
               {
                  // Start the service if the current status is stopped.
                  try 
                  {
                     // Start the service, and wait until its status is "Running".
                     sc.Start();
                     sc.WaitForStatus(ServiceControllerStatus.Running);
                     
      			      started = true;
                  }
                  catch (SystemException ex)
                  {
                     // Get the _remoteStackTraceString of the Exception class
                     FieldInfo remoteStackTraceString = typeof(Exception).GetField( "_remoteStackTraceString",
                                                                                    BindingFlags.Instance | BindingFlags.NonPublic );
                     // Set the InnerException._remoteStackTraceString to the current InnerException.StackTrace
                     remoteStackTraceString.SetValue( ex.InnerException,
                                                      ex.InnerException.StackTrace + Environment.NewLine );
                     // Throw the new exception
                     throw ex.InnerException;
                  }
               }
               else if (sc.Status == ServiceControllerStatus.StopPending )
               {
                  throw new Exception("Another instance of the service is in the process of shutting down");
               }
               else if (sc.Status == ServiceControllerStatus.Running )
               {
    			      started = true;
                  //throw new Exception("The service is already running.");
               }
               else if (sc.Status == ServiceControllerStatus.StartPending )
               {
    			      started = true;
                  //throw new Exception("The service is already in the process of starting.");
               }
            }
         }
			catch (System.Runtime.InteropServices.COMException COMe)
			{
				// Server is not responding
				throw new CoreException(CoreConstants.Exception_ServerNotResponding, COMe);
			}	
         
         return started;
		}	

      //-----------------------------------------------------------------------
      // Stop - Stop the service on a remote server.
      //-----------------------------------------------------------------------
		public bool
		   Stop()
		{
		   bool stopped = false;

         try
         {		   
			   if (ServiceInstalled())
			   {
               ServiceController sc  = new ServiceController();
               sc.ServiceName = m_serviceName;
               sc.MachineName = m_computer;

               if (sc.Status == ServiceControllerStatus.Running)
               {
                  // Stop the service if the current status is running.
                  try 
                  {
                     // Start the service, and wait until its status is "Running".
                     sc.Stop();
                     sc.WaitForStatus(ServiceControllerStatus.Stopped);
                     
      			      stopped = true;
                  }
                  catch (SystemException ex)
                  {
                     // Get the _remoteStackTraceString of the Exception class
                     FieldInfo remoteStackTraceString = typeof(Exception).GetField( "_remoteStackTraceString",
                                                                                    BindingFlags.Instance | BindingFlags.NonPublic );
                     // Set the InnerException._remoteStackTraceString to the current InnerException.StackTrace
                     remoteStackTraceString.SetValue( ex.InnerException,
                                                      ex.InnerException.StackTrace + Environment.NewLine );
                     // Throw the new exception
                     throw ex.InnerException;
                  }
               }
               else if (sc.Status == ServiceControllerStatus.Stopped )
               {
                  stopped = true;
                  //throw new Exception("The service is already stopped.);
               }
               else if (sc.Status == ServiceControllerStatus.StopPending )
               {
                  stopped = true;
                  //throw new Exception("The service is already in the process of shutting down");
               }
               else if (sc.Status == ServiceControllerStatus.StartPending )
               {
                  throw new Exception("The service is in the process of starting.");
               }
            }
         }
			catch (System.Runtime.InteropServices.COMException COMe)
			{
				// Server is not responding
				throw new CoreException(CoreConstants.Exception_ServerNotResponding, COMe);
			}	
         
         return stopped;
		}	
		
		#endregion

		#region Public Method - Get ServiceStatus

      //-----------------------------------------------------------------------
      // GetServiceStatus  - Check to see if the service is running
      //-----------------------------------------------------------------------
		public ServiceControllerStatus
		   GetServiceStatus()
		{
		   ServiceControllerStatus status;
		   
			try
			{
            ServiceController sc  = new ServiceController();
            sc.ServiceName = CoreConstants.CollectionServiceName;
            sc.ServiceName = m_serviceName;
            sc.MachineName = m_computer;

            // checking Status propery causes service status to be queried
            status = sc.Status;
         }
         catch ( Exception ex )
         {
            // probably due to not being installed
            
            // Get the _remoteStackTraceString of the Exception class
            FieldInfo remoteStackTraceString = typeof(Exception).GetField( "_remoteStackTraceString",
                                                                           BindingFlags.Instance | BindingFlags.NonPublic );
            // Set the InnerException._remoteStackTraceString to the current InnerException.StackTrace
            remoteStackTraceString.SetValue( ex.InnerException,
                                             ex.InnerException.StackTrace + Environment.NewLine );
            // Throw the new exception
            throw ex.InnerException;
         }
         
         return status;
		}
		
		#endregion

		#region Private methods - ServiceInstalled
      //-----------------------------------------------------------------------
      // ServiceInstalled  - Check to see if the service is installed.
      //-----------------------------------------------------------------------
		private bool
		   ServiceInstalled()
		{
		   bool isInstalled = false;
		   
			try
			{
            ServiceController sc  = new ServiceController();
            sc.ServiceName = CoreConstants.CollectionServiceName;
            sc.ServiceName = m_serviceName;
            sc.MachineName = m_computer;

            // querying status causes connection to remote service
            // if cant reach or service not installed an exception is thrown
            ServiceControllerStatus statusHolder = sc.Status;
            
            isInstalled = true;
         }
         catch ( Exception ex )
         {
            if ( ex.InnerException.Message.IndexOf("not exist") != -1 )
            {
               isInstalled = false;
            }
            else
            {
               // Get the _remoteStackTraceString of the Exception class
               FieldInfo remoteStackTraceString = typeof(Exception).GetField( "_remoteStackTraceString",
                                                                              BindingFlags.Instance | BindingFlags.NonPublic );
               // Set the InnerException._remoteStackTraceString to the current InnerException.StackTrace
               remoteStackTraceString.SetValue( ex.InnerException,
                                                ex.InnerException.StackTrace + Environment.NewLine );
               // Throw the new exception
               throw ex.InnerException;
            }
         }
         return isInstalled;
		}
		
		#endregion
	}
}
