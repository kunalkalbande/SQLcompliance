using System;
using System.Collections ;
using System.Collections.Specialized ;
using System.Data.SqlClient ;
using System.Diagnostics ;
using System.IO ;
using System.ServiceProcess ;
using System.Text ;
using System.Threading ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Agent ;
using Microsoft.Win32 ;

namespace Idera.SQLcompliance.Cluster
{
	/// <summary>
	/// Summary description for VSInstaller.
	/// </summary>
	public class VSInstaller
	{
		#region Member Variables

		private static string _agentExe = "SQLcomplianceAgent.exe" ;
		private static string _virtualServerKey = "VirtualServers" ;
      private static string _versionValue = "Version" ;
		private static string _vskServicNameValue = "ServiceName" ;
		private static string _vskFullInstanceNameValue = "FullInstanceName" ;
		//private static string _vskPortValue = "Port" ;
		private static int _timeoutSeconds = 10 ;

		#endregion

		#region Properties

		#endregion

		#region Construction/Destruction

		public VSInstaller()
		{
		}

		#endregion

		//
		// AddRegistryinformation()
		//  VirtualServer server - the virtual server to add agent registry keys for
		//
		// This funtion will add the VirtualServer subkey (used for enumeration and such)
		//  and the agent subkey (used for agent configuration parameters) for the supplied
		//  VirtualServer object.  
		//
		// Returns: N/A - Propogates internal exceptions that are thrown
		//  
		private static void AddRegistryInformation(VirtualServer server)
		{
			RegistryKey vsKey = null, vsSubKey = null, agentKey = null, agentSubKey = null ;

			if(server == null)
				throw new ArgumentNullException("server") ;
			try
			{
				// Virtual Server Key
				vsKey = Registry.LocalMachine.OpenSubKey(String.Format(@"SOFTWARE\Idera\SQLCM\{0}", _virtualServerKey), true) ;
				vsSubKey = vsKey.CreateSubKey(server.ServerName) ;
				//if(server.PortNumber != CoreConstants.AgentServerTcpPort)
				//	vsSubKey.SetValue(_vskPortValue, server.PortNumber) ;
				vsSubKey.SetValue(_vskServicNameValue, server.ServiceName) ;
				vsSubKey.SetValue(_vskFullInstanceNameValue, server.FullInstanceName) ;

				// Agent Key
                agentKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Idera\SQLCM", true);
				agentSubKey = agentKey.CreateSubKey(server.ServiceName) ;
				agentSubKey.SetValue(CoreConstants.Agent_RegVal_Server, server.CollectionServer) ;
				string[] instances = new string[1] ;
				instances[0] = server.FullInstanceName ;
				agentSubKey.SetValue(CoreConstants.Agent_RegVal_Instances, instances) ;
				if(server.PortNumber != CoreConstants.AgentServerTcpPort)
					agentSubKey.SetValue(CoreConstants.Agent_RegVal_AgentPort, server.PortNumber) ;
				agentSubKey.SetValue(CoreConstants.Agent_RegVal_TraceDirectory, server.TraceDirectory) ;
            agentSubKey.SetValue(CoreConstants.Agent_RegVal_AssemblyRootDirectory, server.TriggerAssemblyDirectory);
            string[] instancesWithPort = new string[1];
            if (server.InstanceWithPort != null)
            { instancesWithPort[0] = server.InstanceWithPort; }
            if (instancesWithPort[0] != null && instancesWithPort[0] != string.Empty)
            {
                agentSubKey.SetValue("InstancesWithPort", instancesWithPort);
            }
			}
			catch(Exception e)
			{
				try
				{
					// Last ditch cleanup effort
					if(vsSubKey != null)
					{
						vsKey.DeleteSubKeyTree(server.ServerName) ;
						vsSubKey = null ;
					}
					if(agentSubKey != null)
					{
						agentKey.DeleteSubKeyTree(server.ServiceName) ;
						agentSubKey = null ;
					}
				}
				catch{}

				throw e ;
			}
			finally
			{
				if(vsSubKey != null)
					vsSubKey.Close() ;
				if(vsKey != null)
					vsKey.Close() ;
				if(agentSubKey != null)
					agentSubKey.Close() ;
				if(agentKey != null)
					agentKey.Close() ;
			}
		}

		//
		// RemoveRegistryInformation()
		//  VirtualServer server - the server to remove registry information for.
		//
		// This function removes the VirtualServer subkey and the Agent subkey for the supplied
		//  VirtualServer object.
		//
		// Returns - N/A
		//
		private static void RemoveRegistryInformation(VirtualServer server)
		{
			if(server == null)
				throw new ArgumentNullException("server") ;

			RegistryKey vsKey = null, agentKey = null ;

			try
			{
				// Virtual Server Key
				vsKey = Registry.LocalMachine.OpenSubKey(String.Format(@"SOFTWARE\Idera\SQLCM\{0}", _virtualServerKey), true) ;
				vsKey.DeleteSubKeyTree(server.ServerName) ;

				// Agent Key
                agentKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Idera\SQLCM", true);
				agentKey.DeleteSubKeyTree(server.ServiceName) ;
			}
			finally
			{
				if(vsKey != null)
					vsKey.Close() ;
				if(agentKey != null)
					agentKey.Close() ;
			}
		}

		//
		// RegisterService()
		//  VirtualServer server - the server to register and agent for
		//
		// This function registers a Service with windows to monitor the sql server running
		//  on the supplied VirtualServer.  InstallUtil.exe is how we register services.
		//
		// Returns:  true on success, false otherwise
		//
		private static bool RegisterService(VirtualServer server)
		{
			if(server == null)
				throw new ArgumentNullException("server") ;

			ProcessStartInfo pInfo = new ProcessStartInfo() ;

			FileInfo currentModule = new FileInfo(Process.GetCurrentProcess().MainModule.FileName) ;
         FileInfo installUtil = new FileInfo(Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "InstallUtil.exe"));

			pInfo.CreateNoWindow = true ;
			pInfo.UseShellExecute = false ;
			pInfo.WorkingDirectory = currentModule.DirectoryName ;
			pInfo.FileName = installUtil.FullName ;
			pInfo.Arguments = String.Format("/i /ServiceName=\"{0}\" /DisplayName=\"{1}\" /Username=\"{2}\" /Password=\"{3}\" /TraceDirectory=\"{4}\" /LogFile= {5}",
				EscapeQuotedDosArgument(server.ServiceName), EscapeQuotedDosArgument(server.ServiceName), 
            EscapeQuotedDosArgument(server.ServiceUsername), EscapeQuotedDosArgument(server.ServicePassword), 
            EscapeQuotedDosArgument(server.TraceDirectory), _agentExe) ;
			Process p = Process.Start(pInfo) ;

			p.WaitForExit() ;
			if(p.ExitCode == 0)
				return true ;
			else
				return false ;
		}

		//
		// GetService()
		//  string sName - the name of the service to retrieve
		//
		// This function returns the Servicecontroller object for the specified service.
		//  If no service is found, null is returned.
		//
		internal static ServiceController GetService(string sName)
		{
			try
			{
				ServiceController[] services = ServiceController.GetServices() ;
				foreach(ServiceController service in services)
				{
					if(service.ServiceName.Equals(sName))
						return service ;
				}
			}
			catch
			{
			}
			return null ;
		}

		//
		// UnregisterService()
		//  VirtualServer server - the server to unregister an agent for
		//
		// This funtion uses InstallUtil.exe to remove the registration for the service setup
		//  to monitor the supplied VirtualServer
		//
		// Returns:  true on success, false otherwise
		//
		private static bool UnregisterService(VirtualServer server)
		{
			if(server == null)
				throw new ArgumentNullException("server") ;
			ProcessStartInfo pInfo = new ProcessStartInfo() ;

			ServiceController controller = GetService(server.ServiceName) ;
			// No Service exists by that name - already gone.
			if(controller == null)
				return true ;

			try
			{
				// If the service is running, stop it, we are the active node and we need to remove
				//  installed traces and stored procedurse.  This is done via the registry key.
				if(controller.Status != ServiceControllerStatus.Stopped)
					controller.Stop() ;

				//  We must wait for the serivce to be fully stopped before attempting to remove it.
				int tries = _timeoutSeconds ;
				while(tries > 0)
				{
					controller.Refresh() ;
					if(controller.Status == ServiceControllerStatus.Stopped)
						break ;
					else
						Thread.Sleep(1000) ;
					tries-- ;
				}
			}
			catch{}

			// Execute InstallUtil.exe to remove the service registration
			FileInfo currentModule = new FileInfo(Process.GetCurrentProcess().MainModule.FileName) ;
         FileInfo installUtil = new FileInfo(Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "InstallUtil.exe"));

         pInfo.CreateNoWindow = true ;
			pInfo.UseShellExecute = false ;
			pInfo.WorkingDirectory = currentModule.DirectoryName ;
			pInfo.FileName = installUtil.FullName ;
			pInfo.Arguments = String.Format("/u /ServiceName=\"{0}\" /LogFile= {1}",
				server.ServiceName, _agentExe) ;
			Process p = Process.Start(pInfo) ;

			p.WaitForExit() ;

			if(p.ExitCode != 0)
				return false ;

			return true ;
		}

		//
		// CreateAddRemoveProgramEntry()
		//  VirtualServer server - the server to create an add/remove program entry for.
		//
		// This function creates an add/remove program entry for the service monitoring the
		//  supplied VirtualServer.
		//
		private static void CreateAddRemoveProgramEntry(VirtualServer server)
		{
			RegistryKey addRemoveKey = null, addRemoveSubKey = null ;

			if(server == null)
				throw new ArgumentNullException("server") ;
			try
			{
				FileInfo currentModule = new FileInfo(Process.GetCurrentProcess().MainModule.FileName) ;

				// Add/Remove Programs Key
				addRemoveKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true) ;
				addRemoveSubKey = addRemoveKey.CreateSubKey(server.ServiceName) ;
				addRemoveSubKey.SetValue("DisplayName", server.ServiceName) ;
				addRemoveSubKey.SetValue("UninstallString", String.Format("{0} remove {1}", Path.Combine(currentModule.DirectoryName, "VSInstallCmd.exe"), server.ServerName)) ;
			}
			catch(Exception e)
			{
				try
				{
					if(addRemoveSubKey != null)
					{
						addRemoveKey.DeleteSubKeyTree(server.ServiceName) ;
						addRemoveSubKey = null ;
					}
				}
				catch{}

				throw e ;
			}
			finally
			{
				if(addRemoveSubKey != null)
					addRemoveSubKey.Close() ;
				if(addRemoveKey != null)
					addRemoveKey.Close() ;
			}			
		}

		//
		// CreateAddRemoveProgramEntry()
		//  VirtualServer server - the server to remove an add/remove program entry for.
		//
		// This function removes the add/remove program entry for the service monitoring the
		//  supplied VirtualServer.
		//
		private static void DeleteAddRemoveProgramEntry(VirtualServer server)
		{
			if(server == null)
				throw new ArgumentNullException("server") ;
			RegistryKey addRemoveKey = null ;

			try
			{
				// Add/Remove Programs Entry
				addRemoveKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true) ;
				addRemoveKey.DeleteSubKeyTree(server.ServiceName) ;
			}
			catch
			{
			}
			finally
			{
				if(addRemoveKey != null)
					addRemoveKey.Close() ;
			}
		}

		//
		// InstallVirtualServerAgent()
		//  VirtualServer server - the server to install an agent for
		//
		// This is the top-level installations function.  I handles creating the appropriate
		//  registry information and registering the service with windows.
		//
		public static void InstallVirtualServerAgent(VirtualServer server)
		{
			if(server == null)
				throw new ArgumentNullException("server") ;

			AddRegistryInformation(server) ;
			try
			{
				RegisterService(server) ;
			}catch(Exception e)
			{
				RemoveRegistryInformation(server) ;
				throw e ;
			}
			//CreateAddRemoveProgramEntry(server) ;
		}

		//
		// RemoveAllVirtualServers()
		//
		// This function removes all registered agents services for virtual servers.
		//
		public static void RemoveAllVirtualServers()
		{
			RegistryKey vsKey = null ;

			try
			{
				// Virtual Server Key
				vsKey = Registry.LocalMachine.OpenSubKey(String.Format(@"SOFTWARE\Idera\SQLCM\{0}", _virtualServerKey), true) ;
				foreach(string serverName in vsKey.GetSubKeyNames())
				{
					Console.WriteLine("Server {0}", serverName) ;
					RemoveVirtualServerAgent(serverName) ;
				}
			}
			catch
			{
			}
			finally
			{
				if(vsKey != null)
					vsKey.Close() ;
			}
		}

		//
		// RemoveVirtualServerAgent
		//  string virtualServerName - the name of the virtual server to remove an agent for
		//   NOTE:  The virtualServerName can be the virtual server name or full instance name.
		//
		// This function will remove the agent service that is monitoring the supplied virtual server.
		//  This function will unregister the service and remove any residual registry information.
		//
		public static void RemoveVirtualServerAgent(string virtualServerName)
		{
			if(virtualServerName == null)
				throw new ArgumentNullException("virtualServerName") ;

			VirtualServer server = new VirtualServer() ;
			// We tolerate full instance names also
			int index = virtualServerName.IndexOf('\\') ;
			if(index == -1)
				server.ServerName = virtualServerName ;
			else
				server.ServerName = virtualServerName.Substring(0,index) ;

			if(!UnregisterService(server))
				throw new Exception(String.Format("Unable to remove service: {0}", server.ServiceName)) ;

			RemoveRegistryInformation(server) ;
			//DeleteAddRemoveProgramEntry(server) ;
		}

		/// <summary>
		/// This function retrieves the installed virtual instances from the registry
		/// and returns an arraylist of VirtualServer objects.
		/// </summary>
		/// <returns></returns>
		public static ArrayList GetInstalledVirtualInstances()
		{
			ArrayList retVal = new ArrayList() ;
			RegistryKey vsKey = null ;

			try
			{
				// Virtual Server Key
				vsKey = Registry.LocalMachine.OpenSubKey(String.Format(@"SOFTWARE\Idera\SQLCM\{0}", _virtualServerKey)) ;
				/*
				if(vsKey == null)
				{
					// I'm not sure this is a good idea.  replication of registry opens interesting doors here.
					if(!RebuildVirtualServers())
						return null ;
					else
						vsKey = Registry.LocalMachine.OpenSubKey(String.Format(@"SOFTWARE\Idera\SQLCM\{0}", _virtualServerKey)) ;
				}*/
				foreach(string subKeyName in vsKey.GetSubKeyNames())
				{
					using(RegistryKey subKey = vsKey.OpenSubKey(subKeyName))
					{
						string sFullName = (string)subKey.GetValue(_vskFullInstanceNameValue) ;
						VirtualServer server = new VirtualServer() ;
						server.FullInstanceName = sFullName ;
						using(RegistryKey agentSubKey = Registry.LocalMachine.OpenSubKey(String.Format(@"SOFTWARE\Idera\SQLCM\{0}", server.ServiceName)))
						{
							server.CollectionServer = (string)agentSubKey.GetValue(CoreConstants.Agent_RegVal_Server) ;
							server.TraceDirectory = (string)agentSubKey.GetValue(CoreConstants.Agent_RegVal_TraceDirectory) ;
                     server.TriggerAssemblyDirectory = (string)agentSubKey.GetValue(CoreConstants.Agent_RegVal_AssemblyRootDirectory);
							retVal.Add(server) ;
						}
					}
				}

				return retVal ;
			}
			catch
			{
				// Invalid Registry Entry
				return null ;
			}
			finally
			{
				if(vsKey != null)
					vsKey.Close() ;
			}
		}

		//
		// VirtualServerInstalled()
		//  VirtualServer server - the server to check for
		//
		// This function checks to see if a virtual server has an agent installed to monitor it.
		//
		// Returns: true if an agent is installed for the virtual server, false otherwise.
		//
		public static bool VirtualServerInstalled(VirtualServer server)
		{
			RegistryKey vsKey = null ;

			try
			{
				// Virtual Server Key
				vsKey = Registry.LocalMachine.OpenSubKey(String.Format(@"SOFTWARE\Idera\SQLCM\{0}", _virtualServerKey)) ;
				using(RegistryKey vsSubKey = vsKey.OpenSubKey(server.ServerName))
				{
					if(vsSubKey != null)
						return true ;
					else
						return false ;
				}
			}
			catch
			{
				// Invalid Registry Entry
				return false ;
			}
			finally
			{
				if(vsKey != null)
					vsKey.Close() ;
			}
		}

		//
		// UpdateServer()
		//  VirtualServer server - the virtual server to update configuration for
		//
		// This function allows you to update the TraceDirectory and CollectionServer
		//  for an agent monitoring a virtual server.
		//
		public static void UpdateServer(VirtualServer server)
		{
			RegistryKey subKey = null ;

			try
			{
				// Virtual Server Key
				subKey = Registry.LocalMachine.OpenSubKey(String.Format(@"SOFTWARE\Idera\SQLCM\{0}", server.ServiceName), true) ;
				subKey.SetValue(CoreConstants.Agent_RegVal_TraceDirectory, server.TraceDirectory) ;
				subKey.SetValue(CoreConstants.Agent_RegVal_Server, server.CollectionServer) ;
            subKey.SetValue(CoreConstants.Agent_RegVal_AssemblyRootDirectory, server.TriggerAssemblyDirectory);
				subKey.Close() ;
				subKey = null ;
			}
			finally
			{
				if(subKey != null)
					subKey.Close() ;
			}
		}

		//
		// IsClustered()
		//  string fullInstanceName - the name of the instance to query for cluster property
		//
		// This function queries a SQL server instance directly for the IsClustered propertly.
		//  It assumes the use of a trusted connection to the SQL Server, meaning the executing
		//  user must have permission to query the server.
		//
		// Returns:  true if clustered, false otherwise.
		//
		public static bool IsClustered(string fullInstanceName)
		{
			using(SqlConnection connection = new SqlConnection(String.Format("Server={0};Trusted_Connection=True;", fullInstanceName)))
			{
				connection.Open() ;
				using(SqlCommand command = new SqlCommand("SELECT SERVERPROPERTY('IsClustered')"))
				{
					command.Connection = connection ;
					int result = (int)command.ExecuteScalar() ;
					if(result == 0)
						return false ;
					else
						return true ;
				}
			}
		}
		
      //
      // ValidateSqlServerOSCombo()
      // Makes sure we are at least SQL Server 2000
	   public static bool
	      ValidateSqlServerOSCombo(
	         string            instance
	      )
	   {
		   bool retVal = false ;
			using (SqlCommand myCommand = new SqlCommand()) 
			{
				string strConn = String.Format( "server={0};" +
					"integrated security=SSPI;" +
					"Connect Timeout=30;" + 
					"Application Name='{1}';",
					instance ,
					CoreConstants.ManagementConsoleName );

				using(myCommand.Connection = new SqlConnection(strConn))
				{
					myCommand.Connection.Open() ;
					myCommand.CommandText = "SELECT @@VERSION" ;
					myCommand.CommandType = System.Data.CommandType.Text ;

					string versionString = (string)myCommand.ExecuteScalar() ;
					retVal =  IsValidSqlServerOSCombo(versionString) ;
				}
			}
		   return retVal ;
	   }

	   /// <summary>
	   /// Check if the SQL server is a valid SQL Server/OS combination.
	   /// </summary>		
	   /// <param name="versionString">>String returned by @@VERSION</param>
	   /// <returns>true if supported; false otherwise</returns>
	   private static bool IsValidSqlServerOSCombo(string versionString) 
	   {
		   // Get the SQL server full version numbers
		   string sqlVersionMajor;	
		   string sqlVersionBuild;					
		   int iStart, iEnd;
		   
			iStart = versionString.IndexOf("- ") ;
			if(iStart == -1)
				return false ;
			iStart += 2 ;
			iEnd = versionString.IndexOf(".", iStart) ;
			if(iEnd == -1)
				return false ;
			sqlVersionMajor = versionString.Substring(iStart, iEnd - iStart) ;
			iStart = iEnd + 1 ;
			iEnd = versionString.IndexOf(".", iStart) ;
			if(iEnd == -1)
				return false ;
			iStart = iEnd + 1 ;
			iEnd = versionString.IndexOf(" ", iStart) ;
			if(iEnd == -1)
				return false ;
			sqlVersionBuild = versionString.Substring(iStart, iEnd - iStart) ;

			// We don't support MSDE
			if (versionString.IndexOf(CoreConstants.VersionString_MSDE70) != -1
				|| versionString.IndexOf(CoreConstants.VersionString_MSDE2000) != -1) 
			{
				return false;
			}
		
			// We don't support < SQL Server 2000
			if (int.Parse(sqlVersionMajor) < 8) 
			{
				return false;
			}
			
			// Get the OS version from the SQL version string
			int osVersionMajor;
			int osVersionMinor;
			int spNumber = 0 ;
            string osVersion = "";
            if (versionString.Contains(CoreConstants.OSVersionNT))
            {
                osVersion = osVersion + versionString.Substring(versionString.IndexOf(CoreConstants.OSVersionNT));
                osVersion = osVersion.Substring(CoreConstants.OSVersionNT.Length);
                osVersion = osVersion.Substring(0, osVersion.IndexOf(" "));
            }
            else
            {
                osVersion = osVersion + (versionString.Substring(0, versionString.IndexOf(" <X64>")));
                osVersion = osVersion.Substring(osVersion.LastIndexOf(" ") + 1);
            }
			// Parse the major/minor OS version (X.Y)
            osVersionMajor = int.Parse(osVersion.Substring(0, osVersion.LastIndexOf(".")));
            osVersionMinor = int.Parse(osVersion.Substring(osVersion.LastIndexOf(".") + 1));
            
			iStart = versionString.IndexOf("Service Pack ") ;
			if(iStart != -1)
				spNumber = int.Parse(versionString.Substring(iStart + 13, 1)) ;


			// We don't support Windows 2003 Server and < SQL 2000 service pack 3a
			if (osVersionMajor >= 5 && osVersionMinor > 1  // Win2003 Server
				&& int.Parse(sqlVersionMajor) == 8 && int.Parse(sqlVersionBuild) < CoreConstants.VersionBuild_SQL2000SP3) 
			{
				return false;
			}
			
			// We don't support Windows 2000 below service pack 2
			if(osVersionMajor == 5 && osVersionMinor == 0 && spNumber < 2)
			{
				return false ;
			}
			
		   // If we got here, we passed the tests
		   return true;
	   }	

		//
		// RebuildVirtualServers()
		//
		// This function parses the registry in an attempt to rebuild the VirtualServers key.
		//  It's probably not a good idea to do this...
		//
		public static bool RebuildVirtualServers()
		{
			try
			{
				RegistryKey vsKey, complianceKey, agentKey, vsSubKey ;

				vsKey = Registry.LocalMachine.CreateSubKey(String.Format(@"SOFTWARE\Idera\SQLCM\{0}", _virtualServerKey)) ;
                complianceKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Idera\SQLCM");
				foreach(string keyName in complianceKey.GetSubKeyNames())
				{
					if(keyName.ToUpper().StartsWith("SQLCOMPLIANCEAGENT$"))
					{
						agentKey = complianceKey.OpenSubKey(keyName) ;

						// Virtual Server Key
						vsSubKey = vsKey.CreateSubKey(keyName.Substring(19)) ;
						vsSubKey.SetValue(_vskFullInstanceNameValue, ((string[])agentKey.GetValue("INSTANCES"))[0]) ;
						vsSubKey.SetValue(_vskServicNameValue, keyName) ;
						vsSubKey.Close() ;
						agentKey.Close() ;
					}
				}
				complianceKey.Close() ;
				vsKey.Close() ;
				return true ;
			}
			catch
			{
				return false ;
			}
		}

		//
		// DropAutidingSupport()
		//  string fullInstanceName - the full instance name to drop auditing support for
		//
		// This function stops all SQL compliance traces that are running on the supplied instance.
		//  It also drops the stored procedures used to manage the traces.  It is assumed
		//  that a trusted connection can be opened to the instance
		//
		public static bool DropAuditingSupport(string fullInstanceName)
		{
			try
			{
				using( SqlConnection conn = new SqlConnection(String.Format("Server={0};Trusted_Connection=True;", fullInstanceName)))
				{
					conn.Open();
					// Stop all traces
					try
					{
						SPHelper.StopAllTraces( conn );
					}
					catch(SqlException e)
					{
						// eat message if SP is gone already
						if(e.Number != 2812)
							throw e ;
					}

					// Drop all stored procedures
					SPHelper.DropStartupSP( conn );
					SPHelper.DropAuditSP( conn );
					conn.Close();
					return true; 
				}
			}
			catch
			{
				return false ;
			}
		}

		//
		// LoadServerFromRegistry()
		//  string virtualServerName
		//
		// This function loads the VirtualServer information from the registry.  Cluster servers
		//  are keyed by virtual server name, however, many functions need the full instance name
		//  to do their job.  This function provides the VirtualServer object, which contains such
		//  information about the clustered instance.
		//
		// Returns: the VirtualServer object on success, null otherwise
		//
		public static VirtualServer LoadServerFromRegistry(string virtualServerName)
		{
			VirtualServer retVal = null ;
			int index = virtualServerName.IndexOf("\\") ;

			if(index != -1)
				virtualServerName = virtualServerName.Substring(0, index) ;

			try
			{
				// Virtual Server Key
				using(RegistryKey vsKey = Registry.LocalMachine.OpenSubKey(String.Format(@"SOFTWARE\Idera\SQLCM\{0}\{1}", _virtualServerKey, virtualServerName)))
				{
					string sFullName = (string)vsKey.GetValue(_vskFullInstanceNameValue) ;
					retVal = new VirtualServer() ;
					retVal.FullInstanceName = sFullName ;
					using(RegistryKey agentSubKey = Registry.LocalMachine.OpenSubKey(String.Format(@"SOFTWARE\Idera\SQLCM\{0}", retVal.ServiceName)))
					{
						retVal.CollectionServer = (string)agentSubKey.GetValue(CoreConstants.Agent_RegVal_Server) ;
						retVal.TraceDirectory = (string)agentSubKey.GetValue(CoreConstants.Agent_RegVal_TraceDirectory) ;
                  retVal.TriggerAssemblyDirectory = (string)agentSubKey.GetValue(CoreConstants.Agent_RegVal_AssemblyRootDirectory);
					}
				}

				return retVal ;
			}
			catch
			{
				// Invalid Registry Entry
				return null ;
			}
		}

      public static string GetAgentVersion()
      {
         string retVal = null ;

         try
         {
            // Virtual Server Key
            using(RegistryKey vsKey = Registry.LocalMachine.OpenSubKey(String.Format(@"SOFTWARE\Idera\SQLCM\{0}", _virtualServerKey)))
            {
               retVal = (string)vsKey.GetValue(_versionValue) ;
            }
         }
         catch
         {
            // Invalid Registry Entry (gobble)
         }
         if(retVal == null)
            return "Unavailable" ;
         else
            return retVal ;
      }

      private static string EscapeQuotedDosArgument(string s)
      {
         StringBuilder retVal = new StringBuilder() ;
         int slashCount = 0 ;

         if(s == null)
            return "" ;

         foreach(char c in s)
         {
            switch(c)
            {
               case '\"':
                  // For quotes ending a slash, we must double the prior slash count and add one
                  if(slashCount > 0)
                     retVal.Append('\\', slashCount) ;
                  retVal.Append('\\') ;
                  retVal.Append('\"') ;
                  slashCount = 0 ;
                  break ;
               case '\\':
                  slashCount++ ;
                  retVal.Append(c) ;
                  break ;
               default:
                  slashCount = 0 ;
                  retVal.Append(c) ;
                  break ;
            }
         }
         if(slashCount > 0)
         {
            // For strings ending with slash, we must double the quote count
            retVal.Append('\\', slashCount) ;
         }
         return retVal.ToString(); 
      }
	}
}
