using System;

namespace Idera.SQLcompliance.Cluster
{
	/// <summary>
	/// Summary description for Constants.
	/// </summary>
	public class Constants
	{
		private Constants()
		{
		}
		
      public static string Error_InvalidServerName         = "The SQL Server instance name is not in a legal format. Enter a name in the format 'virtualServer\\instance'. Instances local to this computer may not be registerd using the Cluster Management UI. Only clustered SQL Servers may be registered.";
      public static string Error_InvalidServiceAccountName = "Enter a service account name in the form of 'domain\\user'.";
      public static string Error_MismatchedPasswords       = "Password fields don't match.";
	   public static string Error_InvalidDomainCredentials  = "The domain account credentials supplied could not be verified." ;
      public static string Error_InvalidTraceDirectory     = "The trace directory must be a valid local directory path on the SQLcompliance Agent Computer, may not include relative pathing, and must be 180 characters or less.";
      public static string Error_InvalidAssemblyDirectory = "The CLR Trigger Assembly directory must be a valid local directory path on the SQLcompliance Agent Computer, may not include relative pathing, and must be 180 characters or less.";
      public static string Error_NotLocalInstance = "You may not register the local SQL Server instance. Please select a clustered SQL Server instance.";
      public static string Error_InvalidComputerName       = "Enter a valid non-blank computer name.";
		public static string Error_ServerAlreadyInstalled = "The clustered SQL Server instance is already being monitored by an agent." ;
		public static string Error_ServerNotClusteredYesNo = "The selected server does not appear to be clustered.  Do you wish to proceed anyway?" ;
		public static string Error_UnableToContactServerYesNo = "Unable to contact the specified instance to verify that the configuration is supported.  Do you wish to proceed with agent installation?" ;
		public static string Error_InvalidCollectionServerName = "The collection server must be a valid NetBIOS or DNS name." ;
		public static string Error_UnableToLoadServerInformation = "Unable to access registry information for virtual server {0}." ;
		
		public static string Error_ServerOSComboNotSupported = "The combination of the specified SQL Server and the host operating system is not supported. The SQLcompliance Agent cannot be added for this instance.  Do you wish to proceed with agent installation?";
		
		public static string Error_DMOLoadServers            = "An error occurred trying to load the list of SQL Servers available on your network.  SQL Server service pack 2 is required for this ability.  Alternately, you can manually enter the instance name in the text box.\n\nError: {0}";

		
		public static string Help_ClusterHelpFile = "Auditing Virtual SQL Servers.pdf";
	}
}
