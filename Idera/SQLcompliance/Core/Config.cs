#if NOTHING_DEFINED

using System;
using System.Collections;
using System.Text;

namespace Idera.SQLcompliance.Core
{
	/// <summary>
	/// The status object used for agent status messages
	/// </summary>
	public class Config
	{
		
		#region Enums
		#endregion

		#region Properties

      #region General Properties
      
      // agentServer - Machine on which agent is running
      // server      - Machine on which collection server is running
      // serverPort  - Port for agent => server communications
      // agentPort   - Port for server => communications
      // logLevel    - logging level

		private string agentServer;
		public string AgentServer
		{
			get { return agentServer;  }
			set { agentServer = value; }
		}
		
      private string server;
      public string  Server
		{
			get { return server;  }
			set { server = value; }
		}

      private int serverPort;
      public int  ServerPort
		{
			get { return serverPort;  }
			set { serverPort = value; }
		}
		
      private int agentPort;
      public int  AgentPort
		{
			get { return agentPort;  }
			set { agentPort = value; }
		}
		
      private int logLevel;
      public int  LogLevel
		{
			get { return logLevel;  }
			set { logLevel = value; }
		}
		
		private string remotingURL;
		public string  RemotingURL
		{
		   get {return remotingURL; }
		   set { remotingURL = value; }
		}
		
		#endregion
		
		#region HeartbeatStatus properties

		// int     agentVersion
		// int     metadataVersion
		// bool    auditingEnabled;
		// bool    startedSuccessfully;
		// int     sqlServerVersionMajor;
		// string  sqlServerInstanceName;

      private string  agentVersion;
		/// <summary>
		/// agentVersion
		/// </summary>
		public string AgentVersion
		{
			get { return agentVersion;  }
			set { agentVersion = value; }
		}
		
      private int  metadataVersion;
		/// <summary>
		/// metadataVersion
		/// </summary>
		public int MetadataVersion
		{
			get {	return metadataVersion;  }
			set {	metadataVersion = value; }
		}

      private int  startedSuccessfully;
      /// <summary>
      /// StartedSuccessfully
      /// </summary>
      public int StartedSuccessfully
      {
         get {	return startedSuccessfully;  }
         set {	startedSuccessfully = value;}
      }
		
      private int  auditingEnabled;
		/// <summary>
		/// auditingEnabled
		/// </int>
		public int AuditingEnabled
		{
			get {	return auditingEnabled;  }
			set {	auditingEnabled = value;}
		}
		
		private int sqlServerVersionMajor;
		/// <summary>
		/// The major version of the target SQL Server.  
		/// This property will be set in BackupCore.DoBackupOrRestore.
		/// </summary>
		internal int SqlServerVersionMajor
		{
			get { return sqlServerVersionMajor;  }
			set {	sqlServerVersionMajor = value; }
		}
		
		private string sqlServerInstanceName;
		/// <summary>
		/// SQL Server instance name
		/// </summary>
		/// <remarks>Can be null for local instance</remarks>
		public string SqlServerInstanceName
		{
			get {	return sqlServerInstanceName;	}
			set {	if (value != null)
					   sqlServerInstanceName = value.ToUpper();
				   else
					   sqlServerInstanceName = null;
 				 }				
		}
		
		#endregion
		
		#region ErrorStatus properties
		private int    errorNumber;
		/// <summary>
		/// errorNumber
		/// </summary>
		internal int ErrorNumber
		{
			get { return errorNumber;  }
			set {	errorNumber = value; }
		}
		#endregion

		#endregion
		
		#region Constructors

		public Config()
		{
			agentServer     = System.Net.Dns.GetHostName();
			
         server          = CoreConstants.LogSpy_Default_Server;
         serverPort      = CoreConstants.CollectionServerTcpPort;
         agentPort       = CoreConstants.LogSpyServerTcpPort;
         logLevel        = 1;
			
		   agentVersion        = "1.1";
		   metadataVersion     = 1;
		   
		   auditingEnabled     = 0;
		   startedSuccessfully = 0;
		   
		   errorNumber         = 0;											
		}

		#endregion

		#region Public methods
		#endregion

		#region Internal methods

		/// <summary>
		/// Validate the Config object; i.e. make sure that parameters set are valid
		/// </summary>
		internal void ValidateConfiguration()
		{			
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			//builder.Append("SqlCommandMechanismUsed: " + this.SqlCommandMechanismUsed + "\n");

         return builder.ToString();
		}

		#endregion

		#region Static methods
		#endregion
		
	}

}


#endif
