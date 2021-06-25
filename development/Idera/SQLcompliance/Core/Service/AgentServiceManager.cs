using System;
using System.Globalization;
using System.IO;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Security;
using Microsoft.Win32;

namespace Idera.SQLcompliance.Core.Service
{
   /// <summary>
   /// Control class for Agent service.
   /// </summary>
   public sealed class AgentServiceManager
   {
      #region Class-level variables

      private string remoteServer; // The remote server that we're managing
      private CpuFamily cpuFamily ;
      private ManagementScope scope; // The WMI management scope
      private static string encryptKey = "SQLcomplianceAgentService"; // Key used to encode username/password for Agent service installer

      private string windowsDirectory; // The Windows directory on the remote server
      private WindowsIdentity localAdminIdentity; // The WindowsIdentity for the local administrator
      private WindowsImpersonationContext windowsImpersonationContext; // The Windows impersonation context

      #endregion

      #region Properties

      public CpuFamily CPUFamily
      {
         get { return cpuFamily; }
      }

      #endregion



      public enum CpuFamily
      {
         Unsupported = 0,
         x86 = 1,
         ia64 = 2,
         x64 = 3
      } ;

      #region Constructor

      //-----------------------------------------------------------------------
      // Constructor - Instantiate the Agent service manager.
      //-----------------------------------------------------------------------
      public
         AgentServiceManager(
         string localAdminUsername,
         string localAdminPassword,
         string targetServer,
         string remoteAdminUsername,
         string remoteAdminPassword
         )
      {
         bool isLocal = false;

         // We cannot specify login credentials if we are running WMI locally
         if (Dns.GetHostName().ToUpper() == targetServer.ToUpper() ||
             targetServer == "." || targetServer.ToUpper() == "LOCALHOST")
            isLocal = true;

         this.remoteServer = targetServer;

         string manPath = string.Format(@"\\{0}\root\CIMV2",
                                        remoteServer);

         ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                 "ManagementPath",
                                 manPath);

         try
         {
            // Set the default WMI management path
            ManagementPath managementPath = new ManagementPath(manPath);

            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "ManagementPath created");


            // Setup the default WMI management scope
            scope = new ManagementScope();
            scope.Path = managementPath;

            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "ManagementScope created");

            if (localAdminUsername != null)
            {
               // Get the current Windows identity for the local admin
               localAdminIdentity
                  = AuthenticationHelper.GetCurrentIdentity(localAdminUsername,
                                                            localAdminPassword);

               // Try to use these credentials for remote WMI
               if (remoteAdminUsername == null && !isLocal)
               {
                  scope.Options.Username = localAdminUsername;
                  scope.Options.Password = localAdminPassword;
               }
            }

            if (remoteAdminUsername != null && !isLocal)
            {
               // Set the management scope security (on the remote server)
               scope.Options.Username = remoteAdminUsername;
               scope.Options.Password = remoteAdminPassword;
            }
            cpuFamily = CpuFamily.Unsupported ;
            cpuFamily = GetWindowsCpuFamily() ;
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                    "AgentServiceManager: Could not create and initialize remote scope",
                                    ex,
                                    ErrorLog.Severity.Informational);
            scope = null;
            if (ex is System.UnauthorizedAccessException)
            {
               throw new RemoteSecurityException(CoreConstants.Exception_WMIAccessDeniedRemote);
            }
         }
      }

      #endregion

      #region Public static methods

      public static CpuFamily GetCPUFamily( string targetServer )
      {
         AgentServiceManager manager = new AgentServiceManager( null, null, targetServer, null, null );
         return manager.CPUFamily;
      }


      #endregion

      #region Service Install/Uninstall Routines

      //-----------------------------------------------------------------------
      // Install - Install the Agent service on a remote server.
      //-----------------------------------------------------------------------
      public void
         Install(
         string agentServiceUsername,
         string agentServicePassword,
         string agentInstance,
         string agentTraceDirectory,
         string agentHostServer,
         string collectionServer
         )
      {
         ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AgentServerManager::Install");

         try
         {
            if (!SQLcomplianceAgent.ValidSqlServerOSCombo(agentInstance))
               throw new CoreException(CoreConstants.Exception_InvalidSQLServerOSCombo);
         }
         catch (Exception ex)
         {
            if (ex is CoreException)
               throw ex;
         }

         ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AgentServerManager::Impersonate.Enter");
         ImpersonateAdminUser();
         ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AgentServerManager::Impersonate.Exit");

         try
         {
            if (! ServiceInstalled())
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AgentServerManager::GetRemoteDirectory.Enter");

               // Get the remote Windows directory for the server
               GetRemoteWindowsDirectory();

               ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AgentServerManager::GetRemoteDirectory.Exit");
               ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AgentServerManager::CopyFiles.Enter");

               // Copy the Agent service installer to the remote system
               CopyAgentServiceInstaller();

               ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AgentServerManager::CopyFiles.Exit");

               // Execute the Agent service installer on the remote system
               InstallAgentService(agentServiceUsername,
                                   agentServicePassword,
                                   agentInstance,
                                   agentTraceDirectory,
                                   agentHostServer,
                                   collectionServer);
            }
            else
            {
               // Agent service is already installed
               UndoImpersonation();
               AgentCommand myCommand = AgentManager.GetAgentCommand(agentInstance);
               string agentServer = myCommand.GetServerName();

               // TODO:  Using GetHostName() is not correct.  We need to use the collection
               //  server name.
               if (agentServer == null || agentServer.Trim().Length == 0)
               {
                  throw new CoreException("The remote agent does not have a target collection server.");
               }
               else if (String.Compare(collectionServer, agentServer.Trim(), true) != 0)
               {
                  throw new CoreException("The remote agent is configured for a different collection server.");
               }
               myCommand.Activate(agentInstance);
            }
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write("AgentServerManager::Install", ex);
            throw ex;
         }
         finally
         {
            UndoImpersonation();
         }
      }

      //-----------------------------------------------------------------------
      // Upgrade - Upgrade the Agent service on a remote server.
      //-----------------------------------------------------------------------
      public void MinorUpgrade(string collectionServer)
      {
         ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AgentServerManager::Upgrade");

         ImpersonateAdminUser();

         try
         {
            if (! ServiceInstalled())
            {
               // Service is not installed -- give error back to user
               // and return
               throw new CoreException(CoreConstants.Exception_CantUpgradeIfNotInstalled);
            }
            else
            {
               // Get the remote Windows directory for the server
               GetRemoteWindowsDirectory();

               // Copy the Agent service installer to the remote system
               CopyAgentServiceInstaller();

               // Execute the Agent service installer on the remote system
               MinorUpgradeAgentService(collectionServer);
            }
         }
         finally
         {
            UndoImpersonation();
         }
      }

      //-----------------------------------------------------------------------
      // Upgrade - Upgrade the Agent service on a remote server.
      //-----------------------------------------------------------------------
      public void MajorUpgrade(string username, string password)
      {
         ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AgentServerManager::Upgrade");

         ImpersonateAdminUser();

         try
         {
            if (! ServiceInstalled())
            {
               // Service is not installed -- give error back to user
               // and return
               throw new CoreException(CoreConstants.Exception_CantUpgradeIfNotInstalled);
            }
            else
            {
               // Get the remote Windows directory for the server
               GetRemoteWindowsDirectory();

               // Copy the Agent service installer to the remote system
               CopyAgentServiceInstaller();

               // Execute the Agent service installer on the remote system
               MajorUpgradeAgentService(username, password);
            }
         }
         finally
         {
            UndoImpersonation();
         }
      }

      //-----------------------------------------------------------------------
      // Uninstall - Uninstall the Agent service on a remote server.
      //-----------------------------------------------------------------------
      public void
         Uninstall()
      {
         ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AgentServerManager::Uninstall");

         ImpersonateAdminUser();

         try
         {
            // Get the remote Windows directory for the server
            GetRemoteWindowsDirectory();

            // Run the uninstaller for the Agent service
            UninstallAgentService();

            // Check if the service is really uninstalled
            if (ServiceInstalled())
            {
               throw new CoreException(CoreConstants.Exception_ErrorUninstallingAgentService);
            }
         }
         finally
         {
            UndoImpersonation();
         }
      }

      #endregion

      #region  WMI Server Control Routines - Start/Stop

      //-----------------------------------------------------------------------
      // Start - Start the Agent service on a remote server.
      //-----------------------------------------------------------------------
      public bool
         Start()
      {
         ImpersonateAdminUser();

         try
         {
            if (ServiceInstalled())
            {
               ManagementObject service =
                  new ManagementObject(string.Format("Win32_Service='{0}'",
                                                     CoreConstants.AgentServiceName));
               service.Scope = scope;

               UInt32 retval = (UInt32) service.InvokeMethod("StartService", null);
               if (retval == 0)
                  return true;
               else
               {
                  throw new Exception(ServiceError.GetMessageText(retval));
               }
            }
            else
            {
               // Agent service is not installed
               return false;
            }
         }
         finally
         {
            UndoImpersonation();
         }
      }

      //-----------------------------------------------------------------------
      // Stop - Stop the Agent service on a remote server.
      //-----------------------------------------------------------------------
      public bool
         Stop()
      {
         ImpersonateAdminUser();

         try
         {
            if (ServiceInstalled())
            {
               ManagementObject service =
                  new ManagementObject(string.Format("Win32_Service='{0}'",
                                                     CoreConstants.AgentServiceName));
               service.Scope = scope;
               UInt32 retval = (UInt32) service.InvokeMethod("StopService", null);
               if (retval == 0)
                  return true;
               else
               {
                  throw new Exception(ServiceError.GetMessageText(retval));
               }
            }
            else
            {
               // Agent service is not installed
               return false;
            }
         }
         finally
         {
            UndoImpersonation();
         }
      }

      #endregion

      #region Private methods - ServiceInstalled, various MSI execs

      //-----------------------------------------------------------------------
      // ServiceInstalled  - Check to see if the Agent service is installed.
      //-----------------------------------------------------------------------
      private bool
         ServiceInstalled()
      {
         try
         {
            if (scope == null)
            {
               throw new RemoteSecurityException(CoreConstants.Exception_WMIAccessDeniedRemote);
            }
            string svcPath = string.Format("Win32_Service='{0}'", CoreConstants.AgentServiceName);
            ManagementObject service =
               new ManagementObject(svcPath);
            service.Scope = scope;
            service.InvokeMethod("InterrogateService", null);
            return true; // Service is installed
         }
         catch (ManagementException me)
         {
            if (me.ErrorCode == ManagementStatus.AccessDenied)
            {
               throw new RemoteSecurityException(CoreConstants.Exception_WMIAccessDeniedRemote);
            }
            else if (me.ErrorCode == ManagementStatus.NotFound)
            {
               return false; // Service is not installed
            }
            else
            {
               throw;
            }
         }
         catch (UnauthorizedAccessException uae)
         {
            // These are actually remote security problems.
            throw new RemoteSecurityException(CoreConstants.Exception_WMIAccessDeniedLocal, uae);
            //throw new LocalSecurityException(CoreConstants.Exception_WMIAccessDeniedLocal, uae);
         }
      }

      //-----------------------------------------------------------------------
      /// InstallAgentService - Launch the MSI to install
      //-----------------------------------------------------------------------
      private void
         InstallAgentService(
         string username,
         string password,
         string instance,
         string traceDirectory,
         string hostServer,
         string collectionServer
         )
      {
         StringBuilder optionsBuilder = new StringBuilder();

         // TODO: Encrypt the username and password
         string encUsername;
         string encPassword;

         if (username != null && username != "")
         {
            encUsername = InternalEncryptString(username);
            encUsername = EncodeString(encUsername);
            optionsBuilder.AppendFormat("SERVICEACCOUNT=\"{0}\"",
                                        encUsername);
         }
         if (password != null && password != "")
         {
            encPassword = InternalEncryptString(password);
            encPassword = EncodeString(encPassword);
            optionsBuilder.AppendFormat(" SERVICEPASSWORD=\"{0}\"",
                                        encPassword);
         }
         if (instance != null && instance != "")
         {
            optionsBuilder.AppendFormat(" INSTANCE=\"{0}\"",
                                        instance);
         }
         if (traceDirectory != null && traceDirectory != "")
         {
            optionsBuilder.AppendFormat(" TRACE_DIRECTORY=\"{0}\"",
                                        traceDirectory);
         }
         if (collectionServer != null && collectionServer != "")
         {
            optionsBuilder.AppendFormat(" COLLECT_SERVER=\"{0}\"",
                                        collectionServer);
         }


         string options = optionsBuilder.ToString();

         ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                 String.Format("msi options for {0}", BuildInstallerTargetLocalFilename()),
                                 options);

         try
         {
            if (scope == null) // skip WMI iff we couldnt initialize it. straight to manual mode
               throw new Exception("Skipping WMI");

            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AgentServerManager::InstallAgentService - MSI");

            // Try to use the WMI Win32_Product class
            ManagementClass process = new ManagementClass("Win32_Product");
            process.Scope = scope;

            ManagementBaseObject inParams = process.GetMethodParameters("Install");
            inParams["PackageLocation"] = BuildInstallerTargetLocalFilename();
            inParams["AllUsers"] = true;
            if (options != null)
            {
               inParams["Options"] = options;
            }

            ManagementBaseObject outParams;
            outParams = process.InvokeMethod("Install", inParams, null);
            uint returnValue = (uint) outParams["returnValue"];

            if (returnValue == 1613)
            {
               // Improper msiexec.exe version
               throw new CoreException(CoreConstants.Exception_ErrorWrongMsiVersion);
            }
            else if (returnValue == 3010)
            {
               // Reboot Required
               throw new CoreException(CoreConstants.Exception_ErrorRebootRequired);
            }
            else if (returnValue != 0)
            {
               // Other errors
               throw new CoreException(CoreConstants.Exception_ErrorInstallingAgentService);
            }
         }
         catch (Exception e)
         {
            // If we have a CoreException, the MSI failed, so just continue and throw the exception.
            if (e is CoreException)
               throw e;
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AgentServerManager::InstallAgentService - W2K");

            // Target is probably Windows Server 2000, which doesn't have default WMI Win32_Product support,
            // so try to create a process manually
            ManagementClass process = new ManagementClass("Win32_Process");
            process.Scope = scope;

            ManagementBaseObject inParams = process.GetMethodParameters("Create");
            inParams["CommandLine"] = CoreConstants.AgentServiceInstallerMsiexecInstall
                                      + BuildInstallerTargetLocalFilename();
            if (options != null)
            {
               inParams["CommandLine"] += " AllUsers=1 " + options;
            }

            ManagementBaseObject outParams = process.InvokeMethod("Create", inParams, null);
            uint returnValue = (uint) outParams["returnValue"];
            uint processId = (uint) outParams["processId"];
            if (returnValue != 0)
            {
               throw new CoreException(CoreConstants.Exception_ErrorInstallingAgentService);
            }

            WaitForRemoteMsiexecToComplete(processId);
            if (!ServiceInstalled())
            {
               throw new CoreException(CoreConstants.Exception_ErrorInstallingAgentService);
            }
         }
      }

      private void MajorUpgradeAgentService(string username, string password)
      {
         StringBuilder optionsBuilder = new StringBuilder();

         // TODO: Encrypt the username and password
         string encUsername;
         string encPassword;

         if (username != null && username != "")
         {
            encUsername = InternalEncryptString(username);
            encUsername = EncodeString(encUsername);
            optionsBuilder.AppendFormat("SERVICEACCOUNT=\"{0}\"",
                                        encUsername);
         }
         if (password != null && password != "")
         {
            encPassword = InternalEncryptString(password);
            encPassword = EncodeString(encPassword);
            optionsBuilder.AppendFormat(" SERVICEPASSWORD=\"{0}\"",
                                        encPassword);
         }
         optionsBuilder.Append(" IS_MAJOR_UPGRADE=\"Yes\"");         

         string options = optionsBuilder.ToString();

         ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                 String.Format("msi options for {0}", BuildInstallerTargetLocalFilename()),
                                 options);

         try
         {
            if (scope == null) // skip WMI iff we couldnt initialize it. straight to manual mode
               throw new Exception("Skipping WMI");

            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AgentServerManager::InstallAgentService - MSI");

            // Try to use the WMI Win32_Product class
            ManagementClass process = new ManagementClass("Win32_Product");
            process.Scope = scope;

            ManagementBaseObject inParams = process.GetMethodParameters("Install");
            inParams["PackageLocation"] = BuildInstallerTargetLocalFilename();
            inParams["AllUsers"] = true;
            if (options != null)
            {
               inParams["Options"] = options;
            }

            ManagementBaseObject outParams;
            process.InvokeMethod("StopService", null);
            outParams = process.InvokeMethod("Install", inParams, null);
            uint returnValue = (uint) outParams["returnValue"];

            if (returnValue == 1613)
            {
               // Improper msiexec.exe version
               throw new CoreException(CoreConstants.Exception_ErrorWrongMsiVersion);
            }
            else if (returnValue == 3010)
            {
               // Reboot Required
               throw new CoreException(CoreConstants.Exception_ErrorRebootRequired);
            }
            else if (returnValue != 0)
            {
               // Other errors
               throw new CoreException(CoreConstants.Exception_ErrorInstallingAgentService);
            }
         }
         catch (Exception e)
         {
            // If we have a CoreException, the MSI failed, so just continue and throw the exception.
            if (e is CoreException)
               throw e;
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AgentServerManager::InstallAgentService - W2K");

            // Target is probably Windows Server 2000, which doesn't have default WMI Win32_Product support,
            // so try to create a process manually
            ManagementClass process = new ManagementClass("Win32_Process");
            process.Scope = scope;

            ManagementBaseObject inParams = process.GetMethodParameters("Create");
            inParams["CommandLine"] = CoreConstants.AgentServiceInstallerMsiexecInstall
                                      + BuildInstallerTargetLocalFilename();
            if (options != null)
            {
               inParams["CommandLine"] += " AllUsers=1 " + options;
            }

            ManagementBaseObject outParams = process.InvokeMethod("Create", inParams, null);
            uint returnValue = (uint) outParams["returnValue"];
            uint processId = (uint) outParams["processId"];
            if (returnValue != 0)
            {
               throw new CoreException(CoreConstants.Exception_ErrorInstallingAgentService);
            }

            WaitForRemoteMsiexecToComplete(processId);
            if (!ServiceInstalled())
            {
               throw new CoreException(CoreConstants.Exception_ErrorInstallingAgentService);
            }
         }
      }

      //-----------------------------------------------------------------------
      // UpgradeAgentService - Launch the MSI installer to upgrade
      //-----------------------------------------------------------------------
      private void MinorUpgradeAgentService(string collectionServer)
      {
         try
         {
            if (scope == null) // skip WMI iff we couldnt initialize it. straight to manual mode
               throw new Exception("Skipping WMI");

            // Try to use the WMI Win32_Product class
            ManagementClass process = new ManagementClass("Win32_Product");
            process.Scope = scope;

            ManagementBaseObject inParams = process.GetMethodParameters("Install");
            inParams["PackageLocation"] = BuildInstallerTargetLocalFilename();
            inParams["Options"] = CoreConstants.AgentServiceInstallerMsiexecUpgradeSuffix;
            if (collectionServer != null && collectionServer != "")
            {
                inParams["Options"] += String.Format(CoreConstants.AgentMinorUpgradeColletion, collectionServer);
            }
            ManagementBaseObject outParams;
            outParams = process.InvokeMethod("Install", inParams, null);
            uint returnValue = (uint) outParams["returnValue"];
            if (returnValue != 0)
            {
               throw new CoreException(CoreConstants.Exception_ErrorUpgradingAgentService);
            }
         }
         catch
         {
            // Target is probably Windows Server 2000, which doesn't have default WMI Win32_Product support,
            // so try to create a process manually
            ManagementClass process = new ManagementClass("Win32_Process");
            process.Scope = scope;

            ManagementBaseObject inParams = process.GetMethodParameters("Create");
            inParams["CommandLine"] =
               CoreConstants.AgentServiceInstallerMsiexecUpgradePrefix +
               BuildInstallerTargetLocalFilename() +
               CoreConstants.AgentServiceInstallerMsiexecUpgradeSuffix;

            ManagementBaseObject outParams = process.InvokeMethod("Create", inParams, null);
            uint returnValue = (uint) outParams["returnValue"];
            uint processId = (uint) outParams["processId"];
            if (returnValue != 0)
            {
               throw new CoreException(CoreConstants.Exception_ErrorUpgradingAgentService);
            }

            WaitForRemoteMsiexecToComplete(processId);
         }
      }

      //--------------------------------------------------------------------------
      // UninstallAgentService - Uninstall the Agent service from remote server
      //--------------------------------------------------------------------------
      private void
         UninstallAgentService()
      {
         try
         {
            if (scope == null) // skip WMI iff we couldnt initialize it. straight to manual mode
               throw new Exception("Skipping WMI");

            string installedAgenetServiceName = AgentServiceMSIInstalledName();
            // Try to use the WMI Win32_Product class
            SelectQuery query = new SelectQuery(String.Format("SELECT * FROM Win32_Product WHERE Name = '{0}'", installedAgenetServiceName));
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query, null);
            foreach (ManagementObject product in searcher.Get())
            {
               product.Scope = scope;
               ManagementBaseObject outParams = product.InvokeMethod("Uninstall", null, null);
               uint returnValue = (uint) outParams["returnValue"];
               if (returnValue != 0)
               {
                  throw new CoreException(CoreConstants.Exception_ErrorUninstallingAgentService);
               }
            }
         }
         catch (Exception e)
         {
            if (e is CoreException)
               throw e;

            // Target is probably Windows Server 2000, which doesn't have default WMI Win32_Product support
            ManagementClass process = new ManagementClass("Win32_Process");
            process.Scope = scope;

            ManagementBaseObject inParams = process.GetMethodParameters("Create");
            inParams["CommandLine"] = CoreConstants.AgentServiceInstallerMsiexecUninstall
                                      + BuildInstallerTargetLocalFilename();

            ManagementBaseObject outParams = process.InvokeMethod("Create", inParams, null);
            uint returnValue = (uint) outParams["returnValue"];
            uint processId = (uint) outParams["processId"];
            if (returnValue != 0)
            {
               throw new CoreException(CoreConstants.Exception_ErrorUninstallingAgentService);
            }
            WaitForRemoteMsiexecToComplete(processId);
         }

         // Now that we've uninstalled, remove the installer
         DeleteAgentServiceInstaller();
      }

      //-----------------------------------------------------------------------
      // WaitForRemoteMsiexecToComplete
      //-----------------------------------------------------------------------
      private void
         WaitForRemoteMsiexecToComplete(
         uint processId
         )
      {
         // The installer/uninstaller is running, so need to monitor the process until it is complete
         while (true)
         {
            SelectQuery query =
               new SelectQuery("SELECT Name FROM Win32_Process WHERE ProcessId = " + processId);

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query, null);

            if (searcher.Get().Count == 0)
            {
               break;
            }
            Thread.Sleep(1500);
         }
      }

      /*
		//
		//  VerifyMsiexecVersion
		//
		// This function checks the msiexec.exe version on the remote machine represented
		//  by the management scope.  If the version is greater than or equal to the supplied version, 
		//  true is returned, unless exact is specified true.  If exact is true, true is returned
		//  only if the version is an exact match.
		//
		private bool VerifyMsiexecVersion(int major, int minor, bool exact)
		{
			// Try to use the WMI Win32_FileSpecification class
			ManagementObject obj = null ;

			// msiexec.exe should be in c:\windows\system32 or c:\winnt\system32
			try
			{
				obj = new ManagementObject(@"CIM_DataFile.Name='c:\windows\system32\msiexec.exe'");
				obj.Scope = scope ;
				obj.Get() ;
			}
			catch(Exception e)
			{
				try
				{
					obj = new ManagementObject(@"CIM_DataFile.Name='c:\winnt\system32\msiexec.exe'");
					obj.Scope = scope ;
					obj.Get() ;
				}
				catch(Exception e2)
				{
					return false ;
				}
			}

			try
			{
				// Get the version property and compare to the supplied arguments
				string s = (string)obj["Version"] ;
				string[] verStrings = s.Split(new char[] {'.'}) ;

				if(verStrings.Length < 2)
					return false ;

				int remoteMajor, remoteMinor ;

				remoteMajor = Int32.Parse(verStrings[0]) ;
				remoteMinor = Int32.Parse(verStrings[1]) ;

				if(exact)
					return remoteMajor == major && remoteMinor == minor; 

				if(remoteMajor > major || 
					(remoteMajor == major && remoteMinor >= minor))
					return true ;
			}
			catch(Exception e)
			{
			}
			return false ;
		}*/

      #endregion

      #region Private methods - Copy/delete MSI files remotely

      //-----------------------------------------------------------------------
      // GetRemoteWindowsDirectory - Gets the Windows (ADMIN$ share) directory
      //                             on the remote server
      //-----------------------------------------------------------------------
      private void
         GetRemoteWindowsDirectory()
      {
         ImpersonateAdminUser();

         try
         {
            SelectQuery query =
               new SelectQuery("SELECT WindowsDirectory FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            foreach (ManagementObject obj in searcher.Get())
            {
               windowsDirectory = (string) obj.GetPropertyValue("WindowsDirectory");
            }
         }
         catch (ManagementException me)
         {
            if (me.ErrorCode == ManagementStatus.AccessDenied)
            {
               throw new RemoteSecurityException(CoreConstants.Exception_WMIAccessDeniedRemote);
            }
            else
            {
               throw;
            }
         }
         catch (UnauthorizedAccessException uae)
         {
            // These are actually remote security problems.
            throw new RemoteSecurityException(CoreConstants.Exception_WMIAccessDeniedLocal, uae);
            //throw new LocalSecurityException(CoreConstants.Exception_WMIAccessDeniedLocal, uae);
         }
         finally
         {
            UndoImpersonation();
         }
      }

      /// <summary>
      /// Gets the true CPU family of a host.  Does not return Windows edition installed (32bit/64bit).
      /// </summary>
      /// <returns>The CPU family.</returns>
      private void GetCpuFamily()
      {
         try
         {
            SelectQuery query =
               new SelectQuery("select Architecture from Win32_Processor");

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
            {
               foreach (ManagementObject result in searcher.Get())
               {
                  switch (int.Parse(result["Architecture"].ToString()))
                  {
                     case 0:
                        cpuFamily = CpuFamily.x86;
                        break ;
                     case 6:
                        cpuFamily = CpuFamily.ia64;
                        break ;
                     case 9:
                        cpuFamily = CpuFamily.x64;
                        break ;
                     default:
                        cpuFamily = CpuFamily.Unsupported;
                        break ;
                  }
               }
            }
         }
         catch (COMException e)
         {
            throw new InvalidOperationException(string.Format("Could not connect to {0}.", remoteServer), e);
         }
      }

      /// <summary>
      /// Gets the true CPU family of a host.  Return Windows edition installed (32bit/64bit).
      /// </summary>
      /// <returns>The CPU family.</returns>
      private CpuFamily GetWindowsCpuFamily()
      {
         try
         {
            SelectQuery query = new SelectQuery("select VariableValue from Win32_Environment WHERE Name = 'PROCESSOR_ARCHITECTURE' AND UserName = '<SYSTEM>'");

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
            {
               foreach (ManagementObject result in searcher.Get())
               {
                  object test = result["VariableValue"];
                  string environmentString = (test != null) ? test.ToString() : string.Empty;

                  if (string.Compare(environmentString, "x86", true) == 0)
                  {
                     return CpuFamily.x86;
                  }
                  else if (string.Compare(environmentString, "ia64", true) == 0)
                  {
                     return CpuFamily.ia64;
                  }
                  else if (string.Compare(environmentString, "amd64", true) == 0)
                  {
                     return CpuFamily.x64;
                  }
                  else if (string.Compare(environmentString, "em64t", true) == 0)
                  {
                     return CpuFamily.x64;
                  }
                  else
                  {
                     return CpuFamily.Unsupported;
                  }
               }
            }
         }
         catch (COMException e)
         {
            throw new InvalidOperationException(string.Format("Could not connect to {0}.", remoteServer), e);
         }

         return CpuFamily.Unsupported;
      }


      private string GetSqlConsoleApplicationDirectory()
      {
          try
          {
              var sqlComplianceKey = Registry.LocalMachine.OpenSubKey(CoreConstants.SQLcompliance_RegKey);
              if (sqlComplianceKey != null)
              {
                  string path = (string)sqlComplianceKey.GetValue(CoreConstants.SQLcompliance_RegKey_Path);
                  return path;
              }
          }
          catch(Exception ex)
          {
              ErrorLog.Instance.Write(ErrorLog.Level.Always, string.Format("Failed read SQL Compliance path from registry due to the error: {0}", ex.ToString()));
          }
          return AppDomain.CurrentDomain.BaseDirectory;
      }


      //-----------------------------------------------------------------------
      // CopyAgentServiceInstaller - Copy the Agent service installer MSI
      //                              file to the remote server.
      //-----------------------------------------------------------------------
      private void CopyAgentServiceInstaller()
      {
         string installerFilename = GetSqlConsoleApplicationDirectory() + AgentMsiName();
         File.Copy(installerFilename,
                   BuildInstallerTargetRemoteFilename(),
                   true);

         // There is an InstallShield 11.5 bug on x64 where the InstallScript runtime only installs
         //  if executed from a wrapper exe.  If we are x64, copy the exe and run it before anything
         //  else is done.
         if (cpuFamily == CpuFamily.x64)
         {
            try
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AgentServerManager::CopyAgentServicex64PreInstaller.Enter");
               CopyAgentServicex64PreInstaller();
               ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AgentServerManager::CopyAgentServicex64PreInstaller.Exit");
            }
            catch (Exception e)
            {
               // Eat this excpetion 
               ErrorLog.Instance.Write(ErrorLog.Level.Debug, e);
            }

            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AgentServerManager::Executex64PreInstaller.Enter");
            Executex64PreInstaller();
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "AgentServerManager::Executex64PreInstaller.Exit");
         }


      }

      /// <summary>
      /// Copies a no-op installer exe which serves only to get the installshield runtime installed.
      /// This is only necessary (currentl) for x64 machines due to an InstallShield bug.
      /// </summary>
      private void CopyAgentServicex64PreInstaller()
      {
          string installerFilename = GetSqlConsoleApplicationDirectory() + CoreConstants.AgentServicex64PreInstallerFilename;
         File.Copy(installerFilename, Buildx64PreInstallerTargetRemoteFilename(), true);
      }

      /// <summary>
      /// Constructs the x64 pre-installer remote filename
      /// </summary>
      /// <returns></returns>
      private string Buildx64PreInstallerTargetRemoteFilename()
      {
         StringBuilder builder = new StringBuilder();
         builder.Append(@"\\");
         builder.Append(remoteServer);
         builder.Append(CoreConstants.AgentServiceInstallerRemoteShare);
         builder.Append(CoreConstants.AgentServicex64PreInstallerFilename);
         return builder.ToString();
      }

      /// <summary>
      /// Constructs the target filename of the x64 pre-installer on the remote server
      /// </summary>
      /// <returns></returns>
      private string Buildx64PreInstallerTargetLocalFilename()
      {
         StringBuilder builder = new StringBuilder();
         builder.Append(windowsDirectory);
         builder.Append(@"\");
         builder.Append(CoreConstants.AgentServicex64PreInstallerFilename);
         return builder.ToString();
      }

      /// <summary>
      /// Execute the x64 PreInstaller on the remote server (only for x64 machines, due to an InstallShield bug)
      /// </summary>
      private void Executex64PreInstaller()
      {
         ManagementClass process = null;
         ManagementBaseObject inParams = null;
         ManagementBaseObject outParams = null;

         try
         {
            process = new ManagementClass("Win32_Process");
            process.Scope = scope;

            inParams = process.GetMethodParameters("Create");
            inParams["CommandLine"] = Buildx64PreInstallerTargetLocalFilename() + " " + CoreConstants.AgentServicex64PreInstallerCommand;

            outParams = process.InvokeMethod("Create", inParams, null);

            object returnValueObj = outParams["returnValue"];
            uint returnValue = (uint)returnValueObj;

            if (returnValue != 0)
            {
               throw new CoreException(String.Format(CoreConstants.Exception_x64PreInstallerExitCode, returnValue));
            }

            if (outParams["processId"] != null)
            {
               object processIdObj = outParams["processId"];
               uint processId = (uint)processIdObj;

               WaitForRemoteMsiexecToComplete(processId);
            }
         }
         finally
         {
            if (process != null)
               process.Dispose();

            if (inParams != null)
               inParams.Dispose();

            if (outParams != null)
               outParams.Dispose();
         }
      }

      private string AgentMsiName()
      {
         switch (cpuFamily)
         {
            case CpuFamily.x86:
               return CoreConstants.AgentServiceInstallerFilename_Win32;
            case CpuFamily.ia64:
               return CoreConstants.AgentServiceInstallerFilename_ia64;
            case CpuFamily.x64:
               return CoreConstants.AgentServiceInstallerFilename_x64;
            default:
               throw new Exception("Cannot deploy agent to unknown architecture.");
         }
      }


      private string AgentServiceMSIInstalledName()
      {
          switch (cpuFamily)
          {
              case CpuFamily.x64:
                  return CoreConstants.AgentServicexMSIInstalledName_x64;
              case CpuFamily.ia64:
                  return CoreConstants.AgentServicexMSIInstalledName_ia64;
              case CpuFamily.x86:
                  return CoreConstants.AgentServiceMSIInstalledName;
              default:
                  throw new Exception("Cannot deploy agent to unknown architecture.");
          }
      }

      //-----------------------------------------------------------------------
      // DeleteAgentServiceInstaller - Remove the Agent service installer
      //                                from the remote server.
      //-----------------------------------------------------------------------
      private void
         DeleteAgentServiceInstaller()
      {
         File.Delete(BuildInstallerTargetRemoteFilename());
      }

      //-----------------------------------------------------------------------
      /// BuildInstallerTargetLocalFilename - Construct the Agent service
      //                                      installer filename on the remote
      //                                      server
      //-----------------------------------------------------------------------
      private string BuildInstallerTargetLocalFilename()
      {
         StringBuilder builder = new StringBuilder();
         builder.Append(windowsDirectory);
         builder.Append(@"\");
         builder.Append(AgentMsiName());
         return builder.ToString();
      }

      //-----------------------------------------------------------------------
      // BuildInstallerTargetRemoteFilename - Construct the Agent service
      //                                      installer filename on the remote
      //                                      server
      //-----------------------------------------------------------------------
      private string BuildInstallerTargetRemoteFilename()
      {
         StringBuilder builder = new StringBuilder();
         builder.Append(@"\\");
         builder.Append(remoteServer);
         builder.Append(CoreConstants.AgentServiceInstallerRemoteShare);
         builder.Append(AgentMsiName());
         return builder.ToString();
      }

      #endregion

      #region Private Methods - Impersonation

      //-----------------------------------------------------------------------
      // ImpersonateAdminUser - Impersonate the Windows administrative user.
      //-----------------------------------------------------------------------
      private void
         ImpersonateAdminUser()
      {
         if (localAdminIdentity != null)
         {
            try
            {
               windowsImpersonationContext = localAdminIdentity.Impersonate();
            }
            catch (Exception e)
            {
               throw new CoreException(CoreConstants.Exception_CouldNotImpersonateUser, e);
            }
         }
      }

      //-----------------------------------------------------------------------
      // UndoImpersonation - Undo the impersonation of the administrative user.
      //-----------------------------------------------------------------------
      private void
         UndoImpersonation()
      {
         if (localAdminIdentity != null && windowsImpersonationContext != null)
         {
            windowsImpersonationContext.Undo();
         }
      }

      #endregion

      #region Private Methods - Encryption

      public static string EncryptString(string str)
      {
         return EncodeString(InternalEncryptString(str));
      }

      //-----------------------------------------------------------------------
      // InternalEncryptString
      //-----------------------------------------------------------------------
      private static string InternalEncryptString(string decStr)
      {
         if (decStr == null || decStr.Length == 0)
         {
            return decStr;
         }

         char[] encChars = new char[decStr.Length];
         char keyChar;

         // Decrypt the string      
         int j = 0; // key counter
         for (int i = 0; i < decStr.Length; i++)
         {
            keyChar = encryptKey[j++];

            if (decStr[i] != keyChar)
            {
               encChars[i] = (char) (decStr[i] ^ keyChar);
            }
            else
            {
               encChars[i] = decStr[i];
            }

            if (j == encryptKey.Length)
            {
               j = 0;
            }
         }
         return new String(encChars);
      }


      private static string EncodeString(string s)
      {
         UnicodeEncoding encoder = new UnicodeEncoding(true, false);
         byte[] encodedBytes = encoder.GetBytes(s);
         StringBuilder retVal = new StringBuilder();

         for (int i = 0; i < encodedBytes.Length; i++)
            retVal.Append(encodedBytes[i].ToString("X2"));
         return retVal.ToString();
      }

      private string DecodeString(string s)
      {
         UnicodeEncoding encoder = new UnicodeEncoding(true, false);
         byte[] encodedBytes = new byte[s.Length/2];
         char[] decodedChars = new char[encodedBytes.Length/2];

         for (int i = 0; i < encodedBytes.Length; i++)
         {
            string sTemp = s.Substring(i*2, 2);
            encodedBytes[i] = byte.Parse(s.Substring(i*2, 2), NumberStyles.HexNumber);
         }

         encoder.GetDecoder().GetChars(encodedBytes, 0, encodedBytes.Length, decodedChars, 0);
         return new String(decodedChars);
      }

      #endregion
   }
}
