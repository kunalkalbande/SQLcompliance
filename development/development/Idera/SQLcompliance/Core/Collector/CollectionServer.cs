using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core.Agent ;
using Idera.SQLcompliance.Core.Remoting;
using Idera.SQLcompliance.Core.Rules.Alerts ;
using Idera.SQLcompliance.Core.Rules.Filters;
using Idera.SQLcompliance.Core.Scripting;
using Idera.SQLcompliance.Core.Service;
using Microsoft.Win32;

using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Beta;
using Idera.SQLcompliance.Core.Cwf;
using Idera.SQLcompliance.Core.Security;
using Idera.SQLcompliance.Core.Status;
using Idera.SQLcompliance.Core.TraceProcessing;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Licensing;
using Idera.SQLcompliance.Core.TimeZoneHelper;
using TimeZoneInfo = Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;
using System.Net.Security;
using System.Runtime.Serialization.Formatters;

namespace Idera.SQLcompliance.Core.Collector
{
    using System.Collections.Generic;
    using System.Linq;

    using Idera.SQLcompliance.Core.Settings;

    /// <summary>
	/// Summary description for CollectionServer.
	/// </summary>
    public class CollectionServer : PermissionsCheckBase
	{
      #region Singleton constructors and properties

        /// <summary>
        /// The singleton instance of this class
        /// </summary>
        private static CollectionServer instance;
		
		/// <summary>
		/// Static constructor to create singleton instance
		/// </summary>
		static CollectionServer()
		{
			instance = new CollectionServer();
		}

		/// <summary>
		/// Private constructor to prevent code from creating additional instances of singleton
		/// </summary>
		private CollectionServer()
		{			
		}

		/// <summary>
		/// Internal property to provide access to singleton instance
		/// </summary>
		public static CollectionServer Instance 
      {
			get { return instance; }
		}		
		#endregion		
		
      #region Private Data Members
		private int             agentPort  = CoreConstants.AgentServerTcpPort;
      private int             serverPort = CoreConstants.CollectionServerTcpPort;
		private ErrorLog.Level  logLevel   = ErrorLog.Level.Default;
      private int             jobPoolThreads = -1;
      private object          startStopLock = new Object();
      private bool            channelRegistered = false;
      private bool            jobPoolStarted    = false;
      private TraceJobPool    jobPool ;//   = new TraceJobPool();
      
      private Dictionary<string, RepositoryInfo> _repositoryInformationMap = new Dictionary<string, RepositoryInfo>(); 
      internal  TraceJobPool    JobPool
      {
         get { return jobPool; }
      }

      // Event Filtering
      private FiltersConfiguration _filteringConfig ;

      // Alerting
      private AlertingConfiguration _alertingConfig ;
      public Thread alertingJobThread = null ;
      private bool            alertingJobPoolStarted    = false;
      private AlertingJobPool    alertingJobPool ;

      // For heartbeat messages and collection server
      private  TcpServerChannel tcpChannel;

      #endregion

      #region Properties

      public  static int             activityLogLevel = 0;
      public  static int             jobsLogLevel = 0;
      public  static string          traceDirectory   = "";
      private static string          serverInstance   = CoreConstants.RepositoryServerDefault;
      private static ServerJobs      serverJobs       = new ServerJobs();
      
      public bool            aborting = false;
      
      public Thread          jobPoolThread    = null;
		
      public static string ServerInstance
      {
         get { return serverInstance; }
      }

      #endregion
      
      #region Enumerations
      
      internal enum SQLServerStatus
      {
         Online = 0,
         Offline = 1,
         RepositoryDatabaseOffline = 2
      }
      
      #endregion
      
      #region Public Methods
      //--------------------------------------------------------------------------
      // Start - Read configuration, kick off heartbeat thread, register
      //             log gatherers, start processing
      //--------------------------------------------------------------------------
      public void
         Start()
      {
		 ErrorLog.Instance.LogToConsole = true;
         ErrorLog.Instance.Write( ErrorLog.Level.Debug, "CollectionServer::Start - Enter" );
         
         lock ( startStopLock ) // prevent stop from being run until start is done
         {
            aborting = false;
            
            ErrorLog.Instance.Write( ErrorLog.Level.UltraDebug, "CollectionServer::Start - Acquired Lock" );
            
            Repository rep = new Repository();
            ErrorLog.Instance.LogToEventLog  = true;

            // Do this simply to force cleanup of event sources.
            //  We bounce around on event source for alerting.  We do not want to perform
            //  cleanup everytime we change source.  This cleanup is done because we use the
            //  mini-deployment and if users install our application in different locations
            //  at different times, our EventSources become corrupt.  Therefore on startup, 
            //  we set the sources and force them to be re-registered.
            ErrorLog.Instance.EventLogSource = CoreConstants.EventLogSource_Alerting ;
            ErrorLog.Instance.EventLogSource = CoreConstants.EventLogSource_CollectionService;
            ErrorLog.Instance.ErrorLevel     = logLevel;

            try
            {
               CoreHelpers.TurnOffCustomRemotingErrors() ;         
               // Check for local admin privileges
               if ( !SkipAdminCheck() && ! SecurityHelper.IsLocalAdmin() )
               {
                     // SQLCM 5.6- 566/740/4620/5280 (Non-Admin and Non-Sysadmin role) Permissions
                     // Allow Non Admins for SQLCM
			         // throw new Exception(CoreConstants.Exception_ServiceAccountNotLocalAdmin);
			   }
               
               // Read Server Preferences from Registry
               LoadPreferences(); 
               ErrorLog.Instance.ErrorLevel     = logLevel;

                    // initialize CWF helper
                   // if (!CwfHelper.Instance.IsInitialized)
                    //    CwfHelper.Instance.Initialize(Repository.ServerInstance);

                    // Validate trace directory
                    CoreHelpers.ValidateTraceDirectory( traceDirectory );

               if( GetSqlServerStatus( ErrorLog.Level.Default ) != SQLServerStatus.Online )
               {
                  // SQL Server hosting the repository or the repository database is not online.
                  // Start a thread to wait until it is online and finish the initialization.
                  Thread t = new Thread( new ThreadStart( WaitForSQLServer ) );
                  t.Start();
               }
               else
               {
                  StartUpInitialization();
                  ActivateInactiveAgents();
               }              
               ErrorLog.Instance.Write( "Service started successfully." );
            }         
            catch ( Exception ex )
            {
               HandleServiceStartupException(ex);
            }
            finally
            {
               rep.CloseConnection();
               ErrorLog.Instance.Write( ErrorLog.Level.Debug, "CollectionServer::Start - Leave" );
            }
         }
      }
            
      /// <summary>
      /// Stop this service.
      /// </summary>
      public  void Stop()
      {
         ErrorLog.Instance.Write( ErrorLog.Level.Debug, "CollectionServer::Stop - Enter" );
      
         aborting = true;  // signal that we want to stop
         
         // TODO: Delete this later - debugging purposes only
		   ErrorLog.Instance.Write( ErrorLog.Level.Verbose, "ServiceStop called" );
      
         lock ( startStopLock ) // prevent stop from being run until start is done
         {
            ErrorLog.Instance.Write( ErrorLog.Level.UltraDebug, "CollectionServer::Stop - Acquired Lock" );
            
            if (channelRegistered)
            {
               try
               {
                  ChannelServices.UnregisterChannel( tcpChannel );
               }
               catch {} // ignore exceptions during shutdown
                
            }
            
            // Signal the jobPool to stop all works and persist any work left
            if ( jobPoolStarted )
            {
               try
               {
                  jobPool.Stop();
               }
               catch {} // ignore exceptions during shutdown
               jobPool = null;
            }

            // Signal the alertingJobPool to stop all works and persist any work left
            if ( alertingJobPoolStarted )
            {
               try
               {
                  alertingJobPool.Stop();
               }
               catch {} // ignore exceptions during shutdown
               alertingJobPool = null;
            }
            
            // Start server jobs
            try
            {
               serverJobs.StopAllTimers();
            }
            catch {}

            // Log server shutdown to activity log                                  
            Repository rep = null;
            try
            {
               rep = new Repository();
               rep.OpenConnection();
               LogRecord.WriteLog( rep.connection,
                                   LogType.ServerStop,
                                   "",
                                   String.Format("Computer: {0}",System.Net.Dns.GetHostName()) );
               ErrorLog.Instance.Write( "Service stopped successfully." );
            }
            catch( Exception )
            {
               // dont do anything on failure during shutdown
            }
            finally
            {
               if( rep != null )
               rep.CloseConnection();
               ErrorLog.Instance.Write( ErrorLog.Level.Debug, "CollectionServer::Stop - Leave" );
            }
         }
      }

      public void StartAlerting()
      {
         alertingJobPoolStarted = false ;
         try
         {
            // Alerting can NOT shutdown the collection server
            _alertingConfig = new AlertingConfiguration();
            _alertingConfig.Initialize(serverInstance) ;
            alertingJobPool = new AlertingJobPool(_alertingConfig) ;
            ThreadStart alertPoolDelegate = new ThreadStart(StartAlertingPool);
            alertingJobThread = new Thread(alertPoolDelegate) ;
            alertingJobThread.Name = "Alerting" ;
            alertingJobThread.Start() ;
            alertingJobPoolStarted = true ;
         }
         catch(Exception e)
         {
            alertingJobPool = null ;
            alertingJobThread = null ;
            ErrorLog.Instance.Write( CoreConstants.Exception_AlertingStartup, e);
         }
      }

      public void LoadAlertRules()
      {
         if(alertingJobPool != null)
            alertingJobPool.LoadAlertRules() ;
      }

      public void LoadAlertingConfiguration()
      {
         // Other components reference this configuration, so we need to copy
         //  the updates.
         if(_alertingConfig != null)
         {
            // Currently, we only do this for email settings
            AlertingConfiguration newConfig = new AlertingConfiguration(); 
            newConfig.Initialize(serverInstance) ;
            _alertingConfig.SmtpSettings = newConfig.SmtpSettings ;
            _alertingConfig.SnmpConfiguration = newConfig.SnmpConfiguration;
         }
      }

      public void LoadEventFilters()
      {
         if(jobPool != null)
            jobPool.LoadEventFilters() ;
      }

      public void StartTraceProcessing()
      {
         _filteringConfig = new FiltersConfiguration() ;
         _filteringConfig.Initialize(serverInstance) ;
         jobPoolStarted = false ;
         // Start up trace job pool
         jobPool = new TraceJobPool(_filteringConfig) ;
         jobPool.NumberThreads = jobPoolThreads;
         // some cleanup at startup to kill old remnants lying around
         jobPool.Initialize();
         ThreadStart jobPoolDelegate = new ThreadStart(StartJobPool);
         jobPoolThread = new Thread(jobPoolDelegate);
         jobPoolThread.Name = "JobPool";
         jobPoolThread.Start();
         jobPoolStarted = true;
      }

	    

      #endregion

      #region Permissions Check

      internal string HasRoghtsToRepository()
      {
          bool checkFailed = false;
          CheckServerStatus();

          string serviceAccount = CoreHelpers.GetServiceAccount(CoreConstants.CollectionServiceName).Replace("\\", "\\\\");

          StringBuilder resolutionStepsBuilder = new StringBuilder();
          resolutionStepsBuilder.AppendFormat("SQLcompliance Collection Service is running under service account '{0}' ", serviceAccount);
          resolutionStepsBuilder.AppendFormat("and is using repository database '{0}'. \\line ", CoreConstants.RepositoryDatabase);
          resolutionStepsBuilder.Append("Please manually verify that this account has 'sa' access to SQL server and repository database or sufficient privileges to access the SQL Server. \\line ");

          SQLServerStatus serverStatus = GetSqlServerStatus(ErrorLog.Level.UltraDebug);
          switch (serverStatus)
          {
              case SQLServerStatus.Offline:
                  checkFailed = true;
                  resolutionStepsBuilder.Append("ERROR: SQL Server is offline. Please verify that SQL server is running and connectable. \\line ");
                  ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck1, "SQL Server is offline. Please verify that SQL server is running and connectable.", ErrorLog.Severity.Informational);
                  break;
              case SQLServerStatus.RepositoryDatabaseOffline:
                  checkFailed = true;
                  resolutionStepsBuilder.Append("ERROR: SQL compliance repository database is offline. Check that repository database exists on SQL server and is connectable. \\line ");
                  ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck1, string.Format("SQL compliance repository database is offline. Check that repository database '{0}' exists on SQL server and is connectable.", CoreConstants.RepositoryDatabase), ErrorLog.Severity.Informational);
                  break;
              case SQLServerStatus.Online:
                  Repository repository = new Repository();
                  repository.OpenConnection(CoreConstants.RepositoryDatabase);
                  if (!RawSQL.IsCurrentUserSysadmin(repository.connection))
                  {
                        // SQLCM 5.6- 566/740/4620/5280 (Non-Admin and Non-Sysadmin role) Permissions
                        // Check sufficient permissions for non sysadmin users
                        if (!RawSQL.HasSufficientPermissions(repository.connection))
                        {
                            checkFailed = true;
                            resolutionStepsBuilder.Append("ERROR: Current user is not having 'sa' rights or the sufficient privileges to SQL Server. \\line ");
                            ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck1, string.Format("Current user '{0}' is not having 'sa' rights or the sufficient privileges to SQL Server.", serviceAccount), ErrorLog.Severity.Informational);
                        }
                        else
                        {
                            resolutionStepsBuilder.Append("Current user has the sufficient privileges to SQL Server and is not having 'sa' rights. \\line ");
                            ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck1, string.Format("Current user '{0}' has the sufficient privileges to SQL Server and is not having 'sa' rights.", serviceAccount), ErrorLog.Severity.Informational);
                        }
                    }
                  repository.CloseConnection();
                  break;
          }

          return checkFailed ? resolutionStepsBuilder.ToString() : string.Empty;
      }

      internal string HasRightToReadRegistry()
      {
          CheckServerStatus();

          string serviceAccount = CoreHelpers.GetServiceAccount(CoreConstants.CollectionServiceName).Replace("\\", "\\\\");

          StringBuilder resolutionStepsBuilder = new StringBuilder();
          resolutionStepsBuilder.AppendFormat("SQLcompliance Collection Service is running under service account '{0}'. \\line ", serviceAccount);
          resolutionStepsBuilder.Append("An error occured while requesting read access to registry key 'HKEY_LOCAL_MACHINE\\\\Software\\\\Idera\\\\SQLCM'. \\line ");
          resolutionStepsBuilder.Append("Please visit the following link for instructions to manually view/edit the registry: http://wiki.idera.com/x/QIRi \\line ");

          // SQLCM 5.6- 566/740/4620/5280 (Non-Admin and Non-Sysadmin role) Permissions
          // Check sufficient permissions for non admin users
          var exceptionOccured = false;
          Exception ex = CoreHelpers.IsRegistryKeyReadable(@"HKEY_LOCAL_MACHINE\Software\Idera\SQLCM");
          if (ex != null)
          {
              resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line ", ex.Message.Replace("\\", "\\\\"));
              ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck2, "An error occured while requesting read access to registry key 'HKEY_LOCAL_MACHINE\\Software\\Idera\\SQLCM'.", ErrorLog.Severity.Informational);
              exceptionOccured = true;
          }

          // Check sufficient permissions registry for non admin users only
          if (!SecurityHelper.IsLocalAdmin())
          {

              ex = CoreHelpers.IsRegistryKeyReadable(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\Eventlog");
              if (ex != null)
              {
                  resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line ", ex.Message.Replace("\\", "\\\\"));
                  ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck2, "An error occured while requesting read access to registry key 'HKEY_LOCAL_MACHINE\\System\\CurrentControlSet\\Services\\Eventlog'.", ErrorLog.Severity.Informational);
                  exceptionOccured = true;
              }

              ex = CoreHelpers.IsRegistryKeyReadable(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\Eventlog");
              if (ex != null)
              {
                  resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line ", ex.Message.Replace("\\", "\\\\"));
                  ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck2, "An error occured while requesting read access to registry key 'HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\EventLog\\Security'.", ErrorLog.Severity.Informational);
                  exceptionOccured = true;
              }
          }
          return exceptionOccured ? resolutionStepsBuilder.ToString() : string.Empty;
      }

      internal string HasPermissionToTraceDirectory()
      {
          CheckServerStatus();

          string serviceAccount = CoreHelpers.GetServiceAccount(CoreConstants.CollectionServiceName).Replace("\\", "\\\\");

          StringBuilder resolutionStepsBuilder = new StringBuilder();
          resolutionStepsBuilder.AppendFormat("SQLcompliance Collection Service is running under service account '{0}' ", serviceAccount);
          resolutionStepsBuilder.AppendFormat("and trace directory path is '{0}'. \\line Please manually create new trace directory and provide read - write access to it for account '{1}'. \\line ", traceDirectory.Replace("\\", "\\\\"), serviceAccount);

          Exception ex = CoreHelpers.IsDirectoryWritable(traceDirectory);
          if (ex == null)
              return string.Empty;

          resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line ", ex.Message.Replace("\\","\\\\"));
          ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck3, string.Format("No permissions to collection trace directory: {1}{0}Service account: {2}", Environment.NewLine, traceDirectory, serviceAccount), ErrorLog.Severity.Informational);
          return resolutionStepsBuilder.ToString();
      }

        internal string SqlServerHasPermissionToTraceDirectory(string instanceName)
        {
            if (string.IsNullOrEmpty(instanceName))
                return "SQL server instance name not provided.";

            CheckServerStatus();

            StringBuilder resolutionStepsBuilder = new StringBuilder();
            resolutionStepsBuilder.AppendFormat("Path of SQLcompliance Collection trace directory is '{0}' ", traceDirectory.Replace("\\", "\\\\"));
            resolutionStepsBuilder.AppendFormat("and SQL server instance name is '{0}'. \\line ", instanceName.Replace("\\", "\\\\"));

            if (traceDirectory.Length > 128)
            {
                resolutionStepsBuilder.Append("ERROR: Length of trace directory should be less than or equal to 128 characters. Select different trace directory. \\line ");
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck8, string.Format("Length of trace directory '{0}' should be less than or equal to 128 characters.", traceDirectory), ErrorLog.Severity.Informational);
                return resolutionStepsBuilder.ToString();
            }

            // find SQL server service name
            string serviceName = GetWindowsServiceNameOfSqlInstance(instanceName);

            if (string.IsNullOrEmpty(serviceName))
            {
                resolutionStepsBuilder.Append("ERROR: Failed to determine SQL server service name. \\line ");
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck8, "Failed to determine SQL server service name.", ErrorLog.Severity.Informational);
                return resolutionStepsBuilder.ToString();
            }

            string serviceLogonAccount = CoreHelpers.GetServiceAccount(serviceName);
            bool checkPassed = true;

            // SPECIAL CASE: 
            // Both collection server and agents are on different machine.
            // Checking audited SQL server permissions on collection server trace directory doesnot makes any point.
            // This is because both are different machines and a service can't have access to directory on other machine (unless directory is a network share).
            // We are marking this check as passed.
            if (string.IsNullOrEmpty(serviceLogonAccount))
                return string.Empty;

            resolutionStepsBuilder.AppendFormat("SQL server service name is '{0}' and service account is '{1}'. \\line ", serviceName, serviceLogonAccount.Replace("\\", "\\\\"));
            resolutionStepsBuilder.Append("Please manually verify that SQL server's service account has read-write access to trace directory. \\line ");
            resolutionStepsBuilder.Append("Due to security restrictions, the security on this folder will not allow us to process the files properly. Specifying \"modify\" access on the directory for \"everyone\" should fix this issue. \\line ");

            #region SQL server file write & delete check

            string connectionString = String.Format("server={0};" +
                                                    "integrated security=SSPI;" +
                                                    "Connect Timeout=30;" +
                                                    "Application Name='{1}';",
                                                    instanceName,
                                                    ";database=master",
                                                    CoreConstants.DefaultSqlApplicationName);

            Exception exception;
            bool result = CoreHelpers.IsDirectoryWritableBySql(traceDirectory, connectionString, out exception);
            if (!result)
            {
                checkPassed = false;
                if (exception != null)
                {
                    resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line", exception.Message.Replace("\\","\\\\"));
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck8, exception.Message, ErrorLog.Severity.Informational);
                }
                else
                {
                    resolutionStepsBuilder.Append("ERROR: SQL server service account is not having permissions to read and write trace directory. \\line ");
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck8, "SQL server service account is not having permissions to read and write trace directory.", ErrorLog.Severity.Informational);
                }
            }

            // don't check further
            if (!checkPassed)
                return resolutionStepsBuilder.ToString();

            #endregion

            #region service logon account ACL entry check

            result = CoreHelpers.CheckAndGrantDirectoryAccessPermissions(traceDirectory, serviceLogonAccount, out exception);
            if (!result)
            {
                checkPassed = false;
                if (exception != null)
                {
                    resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line", exception.Message.Replace("\\","\\\\"));
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck8, exception.Message, ErrorLog.Severity.Informational);
                }
                else
                {
                    resolutionStepsBuilder.Append("ERROR: SQL server service account is not having permissions to read and write trace directory. \\line ");
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck8, "SQL server service account is not having permissions to read and write trace directory.", ErrorLog.Severity.Informational);
                }

            }

            #endregion

            return checkPassed ? string.Empty : resolutionStepsBuilder.ToString();
        }

      #endregion

      #region Private Methods

      private void WaitForSQLServer()
      {
         try
         {
            while ( GetSqlServerStatus( ErrorLog.Level.Verbose ) != SQLServerStatus.Online )
            {
               for( int i = 0; i < 6; i++ ) // retry every 30 seconds
               {
                  if ( aborting )
                     return;
                  Thread.Sleep( 5000 );
               }
               if( aborting )
                  return;
            }
            ErrorLog.Instance.Write( "SQL Server and main repository database are available now.  Trace collection and processing resumed." );
            StartUpInitialization();
         }
         catch( Exception ex )
         {
            ErrorLog.Instance.Write( "An error occurred in startup recovery thread.", ex, ErrorLog.Severity.Error );
            throw;
         }
      }

      private void StartUpInitialization()
      {
         Repository rep = new Repository();
         
         try
         {
            rep.OpenConnection();
            
            // Check for appropriate SQL privileges
            CheckSQLPrivileges( rep.connection );
            if ( aborting ) return;

            // Check for supported repository schema
            CheckRepositorySchema( rep.connection );
            if ( aborting ) return;

            // Check auto archive time zone to see if it needs initialization
            CheckAutoArchiveTimeZone( rep.connection );
            if ( aborting ) return;

            // Check license at startup and then once a day
            LicenseManager.StartupCheck( serverInstance );
            LicenseManager.CheckLicense();
            if ( aborting ) return;

            // Read Repository Information and perform operations based on the version
            HandleRepositoryInformation(rep);
            if (aborting) return;

            // clear in progress flags for background jobs - integrity and archive
            ClearInProgressFlags( rep.connection );

            // Start all background job timers
            serverJobs.StartAllTimers(rep.connection);
            if ( aborting ) return;

            StartTraceProcessing();
            if ( aborting ) return;

            // Start the alerting job pool
            StartAlerting();
            
            // give jobpool some time to spin up before getting agent requests
            Thread.Sleep( 1000 );
            if ( aborting ) return;

            /* Commenting the Secure Implementation due to Agent Issues
            // Create the composite channel that will encapsulate all of our client channels (enables hybrid secure/unsecure TCP)
            RemotingCompositeChannel cc = new RemotingCompositeChannel(CoreConstants.CollectionClientName, UriComparisonMode.WildcardMatch);

            // Add the secure TCP channel
            Hashtable tcpChannelProps = new Hashtable();
            tcpChannelProps["name"] = CoreConstants.CollectionClientName;
            tcpChannelProps["secure"] = true;
            tcpChannelProps["protectionLevel"] = ProtectionLevel.EncryptAndSign;
            tcpChannelProps["tokenImpersonationLevel"] = TokenImpersonationLevel.Impersonation;
            TcpClientChannel secureChannel = new TcpClientChannel(tcpChannelProps, new BinaryClientFormatterSinkProvider());
            secureChannel.IsSecured = true;
            cc.Add(@"^tcp:\/\/.*(?<!\+Open)$", secureChannel);

            // Register the channel
            ChannelServices.RegisterChannel(cc, false);
             * */

            // Remoting all the remotable objects
            RegisterRemoteObjects();
            channelRegistered = true;

            if (aborting) 
               return;
            MirrorAllowCaptureSql(rep.connection);
            SQLcomplianceConfiguration.WriteServer( rep.connection,
                                                    System.Net.Dns.GetHostName(),
                                                    System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(),
                                                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            string msg = String.Format( "Computer: {0}", System.Net.Dns.GetHostName());
            LogRecord.WriteLog( rep.connection, LogType.ServerStartup, "", msg );

            // start automatic archive scheduler
            if (!ArchiveScheduler.Instance.IsRunning)
                ArchiveScheduler.Instance.Start(false);
         }
         catch( Exception ex)
         {
            HandleServiceStartupException(ex);
         }
         finally
         {
            rep.CloseConnection();
         }
      }

      /// <summary>
      /// Handle Upgrade Scenarios to be handled during service start based on the Repository Information
      /// </summary>
      /// <param name="rep">used for connection</param>
      private void HandleRepositoryInformation(Repository rep)
	  {
          try
          {
              // Read Repository Information Map
              this._repositoryInformationMap = SQLcomplianceConfiguration.GetRepositoryInfo(rep.connection);

              // Validate if SqlCm-5.6 Upgrade already handled 
              if (this._repositoryInformationMap != null && !this._repositoryInformationMap.ContainsKey(CoreConstants.RepositoryInfo_SqlCm5_6))
              {
                  // Get Servers
                  var servers = ServerRecord.GetServers(rep.connection, false);
                  if (servers != null)
                  {
                      // Update Database Records
                      UpdateServerRecords(servers, rep);
                  }
                  // Update Repository Information table to prevent running update
                  SQLcomplianceConfiguration.SetRepositoryInfo(CoreConstants.RepositoryInfo_SqlCm5_6);
              }
          }
          catch (Exception exception)
          {

              ErrorLog.Instance.Write(CoreConstants.Exception_ServerStartup, CoreConstants.RepositoryInfoErrorMsg + exception);
          }
	  }

      /// <summary>
      /// Update Server Records based on repository info table
      /// </summary>
      /// <param name="servers">list of servers</param>
      /// <param name="rep">used for connection</param>
	  private static void UpdateServerRecords(List<ServerRecord> servers, Repository rep)
	  {
	      foreach (var serverRecord in servers)
	      {
	          UserList serverPrivilegedUserList = null;
	          UserList serverTrustedUserList = null;

	          var isServerPrivilegeUserEmpty = string.IsNullOrEmpty(serverRecord.AuditUsersList);
	          var isServerTrustedUserEmpty = string.IsNullOrEmpty(serverRecord.AuditTrustedUsersList);

	          // Ignore if not privilege users set at the server level
	          if (isServerPrivilegeUserEmpty && isServerTrustedUserEmpty)
	          {
	              continue;
	          }

	          // Read Databases
	          var databases = DatabaseRecord.GetDatabases(rep.connection, serverRecord.SrvId);
	          if (databases == null)
	          {
	              continue;
	          }

	          // Update the Server Level Users
	          if (!isServerPrivilegeUserEmpty)
	          {
	              serverPrivilegedUserList = new UserList(serverRecord.AuditUsersList);
	          }
	          if (!isServerTrustedUserEmpty)
	          {
	              serverTrustedUserList = new UserList(serverRecord.AuditTrustedUsersList);
	          }

	          foreach (var databaseRecordOriginal in databases)
	          {
	              var isDbPrivilegeUserEmpty = string.IsNullOrEmpty(databaseRecordOriginal.AuditPrivUsersList);
	              var isDbTrustedUserEmpty = string.IsNullOrEmpty(databaseRecordOriginal.AuditUsersList);

                  var databaseRecord = databaseRecordOriginal.Clone();
	              databaseRecord.Connection = rep.connection;

                  var isDbRecordUpdated = UpdateDbRecord(
	                  databaseRecord,
	                  serverPrivilegedUserList,
	                  isServerPrivilegeUserEmpty,
	                  isDbPrivilegeUserEmpty,
	                  serverTrustedUserList,
	                  isServerTrustedUserEmpty,
	                  isDbTrustedUserEmpty);

	              if (isDbRecordUpdated)
	              {
	                  SaveDbRecord(databaseRecord, databaseRecordOriginal, rep);
	              }
	          }
	      }
	  }

	    /// <summary>
	    /// Update Database Record Privileged or Trusted users based on values in the server
	    /// </summary>
	    /// <param name="databaseRecord">Database Record to be updated</param>
	    /// <param name="serverPrivilegedUserList">List of Server Privileged Users</param>
	    /// <param name="isServerPrivilegeUserEmpty">True if no privilege users in the server level</param>
	    /// <param name="isDbPrivilegeUserEmpty">True if no privilege users in the database level</param>
	    /// <param name="serverTrustedUserList">List of Server Trusted Users</param>
	    /// <param name="isServerTrustedUserEmpty">True if no trusted users in the server level</param>
	    /// <param name="isDbTrustedUserEmpty">True if no trusted users in the database level</param>
	    /// <returns>True if any record is updated</returns>
	    /// <remarks>
	    /// Keeps the hierarchy of the servers and database intact
	    /// Upgrade scenarios that are not feasible on the SQL end are ideally performed here
	    /// e.g. use of the Privilege/Trusted Users List serialized and stored as VARBINARY
	    /// </remarks>
	    private static bool UpdateDbRecord(DatabaseRecord databaseRecord, UserList serverPrivilegedUserList, bool isServerPrivilegeUserEmpty, bool isDbPrivilegeUserEmpty, UserList serverTrustedUserList, bool isServerTrustedUserEmpty, bool isDbTrustedUserEmpty)
	  {
	      var isDbRecordUpdated = false;

          if (!isDbPrivilegeUserEmpty && !isServerPrivilegeUserEmpty)
	      {
              // Update privilege users
	          var dbPrivilegedUserList = new UserList(databaseRecord.AuditPrivUsersList);
	          foreach (var serverPrivilegedLogin in serverPrivilegedUserList.Logins)
	          {
                  // Use LINQ for comparison if login already exists
	              if (dbPrivilegedUserList.Logins.Any(dbLogin => dbLogin.Name.Equals(serverPrivilegedLogin.Name)))
	              {
	                  continue;
	              }
	              dbPrivilegedUserList.AddLogin(serverPrivilegedLogin.Name, serverPrivilegedLogin.Sid);
	              isDbRecordUpdated = true;
	          }

	          foreach (var serverPrivilegedSr in serverPrivilegedUserList.ServerRoles)
	          {
	              // Use LINQ for comparison if login already exists
	              // SQLCM-5868: Roles added to default server settings gets added twice at database level
	              if (dbPrivilegedUserList.ServerRoles.Any(dbLogin => dbLogin.CompareName(serverPrivilegedSr)))
	              {
	                  continue;
	              }
	              dbPrivilegedUserList.AddServerRole(
	                  serverPrivilegedSr.Name,
	                  serverPrivilegedSr.FullName,
	                  serverPrivilegedSr.Id);
	              isDbRecordUpdated = true;
	          }
	          databaseRecord.AuditPrivUsersList = dbPrivilegedUserList.ToString();
	      }
          else if (isDbPrivilegeUserEmpty && !isServerPrivilegeUserEmpty)  // Handle the empty case
          {
              // SQLCM-5721- Assert that server and the database settings follows hierarchy after the upgrade
              databaseRecord.AuditPrivUsersList = serverPrivilegedUserList.ToString();
              isDbRecordUpdated = true;
          }

          // Update Trusted users
          if (!isDbTrustedUserEmpty && !isServerTrustedUserEmpty)
	      {
	          var dbTrustedUserList = new UserList(databaseRecord.AuditUsersList);
	          foreach (var serverTrustedLogin in serverTrustedUserList.Logins)
	          {
	              if (dbTrustedUserList.Logins.Any(dbLogin => dbLogin.Name.Equals(serverTrustedLogin.Name)))
	              {
	                  continue;
	              }
	              dbTrustedUserList.AddLogin(serverTrustedLogin.Name, serverTrustedLogin.Sid);
	              isDbRecordUpdated = true;
	          }
	          foreach (var serverTrustedSr in serverTrustedUserList.ServerRoles)
	          {
	              // SQLCM-5868: Roles added to default server settings gets added twice at database level
	              if (dbTrustedUserList.ServerRoles.Any(dbLogin => dbLogin.CompareName(serverTrustedSr)))
	              {
	                  continue;
	              }
	              dbTrustedUserList.AddServerRole(serverTrustedSr.Name, serverTrustedSr.FullName, serverTrustedSr.Id);
	              isDbRecordUpdated = true;
	          }
	          databaseRecord.AuditUsersList = dbTrustedUserList.ToString();
	      }
	      else if (isDbTrustedUserEmpty && !isServerTrustedUserEmpty)  // Handle the empty case
	      {
              // SQLCM-5721- Assert that server and the database settings follows hierarchy after the upgrade
	          databaseRecord.AuditUsersList = serverTrustedUserList.ToString();
	          isDbRecordUpdated = true;
	      }
          return isDbRecordUpdated;
	  }
      
      /// <summary>
      /// Save Database Records if old and the new records are not the same
      /// </summary>
      /// <param name="databaseRecord">database record to be updated</param>
      /// <param name="databaseRecordOriginal">original database record</param>
	  /// <param name="rep">used for connection</param>
      private static void SaveDbRecord(DatabaseRecord databaseRecord, DatabaseRecord databaseRecordOriginal, Repository rep)
	  {
	      // Execute Update SQL in a transaction
	      using (var transaction = rep.connection.BeginTransaction())
	      {
	          var errorMsg = string.Empty;
	          try
	          {
	              //---------------------------------------
	              // Write Database Properties if necessary
	              //---------------------------------------
	              if (DatabaseRecord.Match(databaseRecord, databaseRecordOriginal))
	              {
	                  return;
	              }
	              if (databaseRecord.Write(databaseRecordOriginal, transaction))
	              {
	                  return;
	              }

	              errorMsg = DatabaseRecord.GetLastError();
	              throw (new Exception(errorMsg));
	          }
	          catch (Exception ex)
	          {
	              errorMsg = ex.Message;
	          }
	          finally
	          {
	              //-----------------------------------------------------------
	              // Cleanup - Close transaction, update server, display error
	              //-----------------------------------------------------------
	              transaction.Commit();

	              string changeLog = Snapshot.DatabaseChangeLog(
	                  rep.connection,
	                  databaseRecordOriginal,
	                  databaseRecord,
	                  string.Empty,
	                  string.Empty,
	                  string.Empty);

	              // Register change to server and perform audit log		
	              // bump server version number so that agent will synch up	      
	              ServerRecord.IncrementServerConfigVersion(rep.connection, databaseRecordOriginal.SrvId);

	              // Log update
	              LogRecord.WriteLog(rep.connection, LogType.ModifyDatabase, databaseRecordOriginal.SrvInstance, changeLog);

	              if (!string.IsNullOrEmpty(errorMsg))
	              {
	                  ErrorLog.Instance.Write(CoreConstants.Exception_ServerStartup, CoreConstants.DbUpdateErrorMsg + errorMsg);
	              }
	          }
	      }
	  }

	  //
      // Check SQL Server and main database status.
      //
      private SQLServerStatus GetSqlServerStatus(ErrorLog.Level level)
      {
         Repository rep = new Repository();

         // Test connection
         try
         {
            rep.OpenConnection("");
         }
         catch (SqlException sqlEx)
         {
            ErrorLog.Instance.Write(level, CoreConstants.Exception_SQLServerStartup, sqlEx, ErrorLog.Severity.Warning);
            return SQLServerStatus.Offline;
         }
         CheckMainDatabases( rep.connection );
         
         // test the repository database
         try
         {
            SqlConnection conn = rep.connection;
            conn.ChangeDatabase(CoreConstants.RepositoryDatabase);
         }
         catch (SqlException sqlEx)
         {
            ErrorLog.Instance.Write(level, CoreConstants.Exception_MainDatabaseStartup, sqlEx, ErrorLog.Severity.Warning);
            return SQLServerStatus.RepositoryDatabaseOffline;
         }
         rep.CloseConnection();
         return SQLServerStatus.Online;
      }
      
      private void HandleServiceStartupException( Exception ex )
      {
         ErrorLog.Instance.Write( CoreConstants.Exception_ServerStartup, ex );
         System.Diagnostics.Process p = System.Diagnostics.Process.GetCurrentProcess();
         p.Kill();
      }

      private void StartJobPool()
      {
         jobPool.Start();
      }

      private void StartAlertingPool()
      {
         alertingJobPool.Start() ;
      }
 
      //-----------------------------------------------------------------------		
      // LoadPreferences
      //-----------------------------------------------------------------------		
      private void LoadPreferences( )
      {
         RegistryKey rk  = null;
         RegistryKey rks = null;
         
         try
         {
            rk  = Registry.LocalMachine;
            rks = rk.CreateSubKey(CoreConstants.CollectionService_RegKey);
            
	         serverPort     = (int)rks.GetValue( CoreConstants.CollectionService_RegVal_ServerPort, CoreConstants.CollectionServerTcpPort );
	         agentPort      = (int)rks.GetValue( CoreConstants.CollectionService_RegVal_AgentPort, CoreConstants.AgentServerTcpPort );
            
            // log level
	         int lvl        = (int)rks.GetValue( CoreConstants.CollectionService_RegVal_LogLevel, 1 );
            if ( lvl < 0 ) lvl = 1;
            if ( lvl > 4 ) lvl = 4;
            logLevel = (ErrorLog.Level)lvl;
            
            // activity log level
	         activityLogLevel = (int)rks.GetValue( CoreConstants.CollectionService_RegVal_ActivityLogLevel, 0 );
	         if ( activityLogLevel != 0 ) activityLogLevel = 1;

            // jobs log level
            jobsLogLevel = (int)rks.GetValue(CoreConstants.CollectionService_RegVal_JobsLogLevel, 0);
            if (jobsLogLevel != 0) jobsLogLevel = 1;

            // timeout value for SqlCommand (note that is not a default - it has to be
            // explicitly set on your SqlCommand objects before execution
	         CoreConstants.sqlcommandTimeout = (int)rks.GetValue( CoreConstants.CollectionService_RegVal_SqlCommandTimeout, CoreConstants.CollectionService_DefaultSqlCommandTimeout );
	         if ( CoreConstants.sqlcommandTimeout < 30) CoreConstants.sqlcommandTimeout = CoreConstants.CollectionService_DefaultSqlCommandTimeout;
	         
	         // filter SQLcompliance events
	         CoreConstants.filterAgentEvents  = ((int)rks.GetValue( CoreConstants.CollectionService_RegVal_FilterAgentEvents,  1 ) == 1 ) ? true : false;
	         CoreConstants.filterGUIEvents    = ((int)rks.GetValue( CoreConstants.CollectionService_RegVal_FilterGUIEvents,    1 ) == 1 ) ? true : false;
	         CoreConstants.filterServerEvents = ((int)rks.GetValue( CoreConstants.CollectionService_RegVal_FilterServerEvents, 1 ) == 1 ) ? true : false;
            CoreConstants.AllowCaptureSql = ((int)rks.GetValue( CoreConstants.CollectionService_RegVal_AllowCaptureSql, 1 ) == 1 ) ? true : false;

            CoreConstants.LogSQLParsingErrors = ((int)rks.GetValue(CoreConstants.CollectionService_RegVal_LogSQLErrors, 1) == 1) ? true : false;

            // alert processing
            CoreConstants.alertingMaxEventsToProcess = (int)rks.GetValue(CoreConstants.CollectionService_RegVal_AlertingMaxEventsToProcess, CoreConstants.CollectionService_DefaultAlertingMaxEventsToProcess);

	         // diagnstic code - done delete TRC files
	         CoreConstants.DontDeleteNonDMLTraces   = ((int)rks.GetValue( CoreConstants.CollectionService_RegVal_DontDeleteNonDMLTraces, 0 )   == 1 ) ? true : false;
	         CoreConstants.DontDeleteDMLTraces      = ((int)rks.GetValue( CoreConstants.CollectionService_RegVal_DontDeleteDMLTraces, 0 )       == 1 ) ? true : false;
  
            // log filtered out events
            CoreConstants.LogFilteredOutEvents = ((int)rks.GetValue( CoreConstants.CollectionService_RegVal_LogFilteredOutEvents, 0 ) == 1 ) ? true : false;

            // EventRule and AlertRule optimization
            CoreConstants.optimizeRules = ((int)rks.GetValue(CoreConstants.CollectionService_RegVal_OptimizeRules, 1) == 1) ? true : false ;
            
            // Archive and Groom batch sizes
            CoreConstants.archiveBatchSize =((int)rks.GetValue(CoreConstants.CollectionService_RegVal_ArchiveBatchSize, CoreConstants.ArchiveBatchSize));
            CoreConstants.groomBatchSize =((int) rks.GetValue(CoreConstants.CollectionService_RegVal_GroomBatchSize, CoreConstants.GroomBatchSize));
            
            // number of threads in job pool
	         int t = (int)rks.GetValue( CoreConstants.CollectionService_RegVal_JobPoolThreads, -1 );

	         if ( t != -1 )
	         {
               if (t < 1)
               {
                  t = 1;
               }

               if (t > 100)
               {
                  t = 100;
               }
               jobPoolThreads = t;
            }
	         traceDirectory = (string)rks.GetValue( CoreConstants.CollectionService_RegVal_TraceDir, "" );
            serverInstance = (string)rks.GetValue( CoreConstants.CollectionService_RegVal_ServerInstance, CoreConstants.RepositoryServerDefault );	
            Repository.ServerInstance = serverInstance;
            
            // Days stats records should be cached in memory
            CoreConstants.DaysStatsCached = (int)rks.GetValue( CoreConstants.CollectionService_RegVal_DaysStatsCached, CoreConstants.DefaultDaysStatsCached );
            CoreConstants.ParseForUpdateStats = ((int)rks.GetValue(CoreConstants.CollectionService_RegVal_ParseForUpdateStats,1) >0) ? true : false;
            CoreConstants.LinkDataChangeRecords = ((int)rks.GetValue(CoreConstants.CollectionService_RegVal_LinkDataChange, 1) > 0) ? true : false;

            SetReindexFlag(true);
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Default,
                                     CoreConstants.Exception_ErrorLoadingPreferences,
                                     ex.Message,
                                     ErrorLog.Severity.Warning );

            // Abort startup if we cant read the preferences
            throw new Exception (CoreConstants.Exception_ErrorLoadingPreferences,ex);
         }
         finally
         {
            string msg = GetServerSettings() ;
            ErrorLog.Instance.Write( ErrorLog.Level.Default, msg );

            if (rks != null)
            {
               rks.Close();
            }

            if (rk != null)
            {
               rk.Close();
            }
         }

         // validate preferences         
         if ( traceDirectory == "" )
         {
            string msg = String.Format(CoreConstants.Alert_InvalidTraceDirectory, traceDirectory);
            InternalAlert.Raise( msg );
            throw new Exception (msg);
         }	                                                        
         else
         {
	         // truncate trailing backslash - setup sometimes puts one on and it causes problems!
	         int len = traceDirectory.Length;

	         if ( traceDirectory[len-1]=='\\' ||
	              traceDirectory[len-1]=='/' )
	         {
	            traceDirectory = traceDirectory.Substring(0,len-1);
	         }
         }
      }

      public void SetReindexFlag(bool reindex)
      {
         int value;
         if (reindex)
            value = 1;
         else
            value = 0;

         //Set the flag to zero because we have fixed the indexes on all event databases.
         RegistryKey rk = null;
         RegistryKey rks = null;

         try
         {
            rk = Registry.LocalMachine;
            rks = rk.CreateSubKey(CoreConstants.CollectionService_RegKey);
            rks.SetValue(CoreConstants.CollectionService_RegVal_CheckEventIndexes, value);
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Default, "Unable to set the reindex flag.", ex.Message, ErrorLog.Severity.Warning);
         }
         finally
         {
            if (rks != null)
            {
               rks.Close();
            }

            if (rk != null)
            {
               rk.Close();
            }
         }   
      }

      public void SetLogLevel(int level)
      {
         RegistryKey rk = null;
         RegistryKey rks = null;

         try
         {
            rk = Registry.LocalMachine;
            rks = rk.CreateSubKey(CoreConstants.CollectionService_RegKey);
            rks.SetValue(CoreConstants.CollectionService_RegVal_LogLevel, level);
         }
         finally
         {
            if (rks != null) rks.Close();
            if (rk != null) rk.Close();
         }
      }
      
      public void ReloadSomePreferences()
      {
         RegistryKey rk  = null;
         RegistryKey rks = null;
         
         try
         {
            rk  = Registry.LocalMachine;
            rks = rk.CreateSubKey(CoreConstants.CollectionService_RegKey);
            
            // log level
	         int lvl = (int)rks.GetValue( CoreConstants.CollectionService_RegVal_LogLevel, 1 );

            if (lvl < 0)
            { 
               lvl = 1; 
            }

            if (lvl > 4)
            {
               lvl = 4;
            }
            logLevel = (ErrorLog.Level)lvl;

            // jobs log level
            jobsLogLevel = (int)rks.GetValue(CoreConstants.CollectionService_RegVal_JobsLogLevel, 0);
            if (jobsLogLevel != 0) jobsLogLevel = 1;

            ErrorLog.Instance.ErrorLevel = logLevel;
	         
	         // diagnostic code - done delete TRC files
	         CoreConstants.DontDeleteNonDMLTraces   = ((int)rks.GetValue( CoreConstants.CollectionService_RegVal_DontDeleteNonDMLTraces, 0 )   == 1 ) ? true : false;
	         CoreConstants.DontDeleteDMLTraces      = ((int)rks.GetValue( CoreConstants.CollectionService_RegVal_DontDeleteDMLTraces, 0 )       == 1 ) ? true : false;

            // alert processing may need to be adjusted on the fly if it gets way behind
            CoreConstants.alertingMaxEventsToProcess = (int)rks.GetValue(CoreConstants.CollectionService_RegVal_AlertingMaxEventsToProcess, CoreConstants.CollectionService_DefaultAlertingMaxEventsToProcess);
         }
         catch 
         {
         }
         finally
         {
            if ( rks != null )rks.Close();
            if ( rk != null )rk.Close();
         }
      }

      private void
         RegisterRemoteObjects()
      {
         try
         {
            //--------------------------------------------
            // Register Port for agent to server messages
            //--------------------------------------------
             tcpChannel = ChannelBuilder.GetRegisteredServerChannel(typeof(CollectionServer).Name, serverPort);
            /* Commenting the Secure Implementation due to Agent Issues
            // Register the secure TCP channel
            BinaryServerFormatterSinkProvider formatterProvider = new BinaryServerFormatterSinkProvider();
            formatterProvider.TypeFilterLevel = TypeFilterLevel.Full;
             
            Hashtable tcpChannelProps = new Hashtable();
            tcpChannelProps["machineName"] = Environment.MachineName;
            tcpChannelProps["name"] = typeof(CollectionServer).Name;
            tcpChannelProps["port"] = serverPort;
            tcpChannelProps["secure"] = true;
            tcpChannelProps["protectionLevel"] = ProtectionLevel.EncryptAndSign;
            tcpChannelProps["impersonate"] = false; // This must be false, because we'll impersonate manually 
            tcpChannel = new TcpServerChannel(tcpChannelProps, formatterProvider);
            ChannelServices.RegisterChannel(tcpChannel, true);
             * */
              
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                    String.Format( "Server Channel Opened: {0}:{1}",
                                    CoreConstants.CollectionServerName,
                                    serverPort), 
                                    ErrorLog.Severity.Informational );
			
            // Status Message Handler
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteStatusLogger), 
                                                               CoreConstants.CollectionServerName,
                                                               WellKnownObjectMode.SingleCall );
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                     String.Format( "{0} is registered as a remotable object.  URL: {1}",
                                     CoreConstants.CollectionServerName,
                                     tcpChannel.GetUrlsForUri( CoreConstants.CollectionServerName )[0] ));

            // Remoting RemoteCollector
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteCollector),
                                                               typeof(RemoteCollector).Name,
                                                               WellKnownObjectMode.SingleCall );
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                    String.Format( "{0} is registered as a remotable object.  URL: {1}",
                                    typeof(RemoteCollector).Name,
                                    tcpChannel.GetUrlsForUri( typeof(RemoteCollector).Name )[0] ));

            // Remoting RemoteAuditManager
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteAuditManager),
                                                               typeof(RemoteAuditManager).Name,
                                                               WellKnownObjectMode.SingleCall );
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                    String.Format( "{0} is registered as a remotable object.  URL: {1}",
                                    typeof(RemoteAuditManager).Name,
                                    tcpChannel.GetUrlsForUri( typeof(RemoteAuditManager).Name )[0] ));

            // Remoting AgentManager
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(AgentManager),
                                                               typeof(AgentManager).Name,
                                                               WellKnownObjectMode.SingleCall );
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                    String.Format( "{0} is registered as a remotable object.  URL: {1}",
                                    typeof(AgentManager), 
                                    tcpChannel.GetUrlsForUri( typeof(AgentManager).Name )[0] ));

            // Remoting ServerManager
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ServerManager),
                                                               typeof(ServerManager).Name,
                                                               WellKnownObjectMode.SingleCall );

            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                    String.Format( "{0} is registered as a remotable object.  URL: {1}",
                                    typeof(ServerManager), 
                                    tcpChannel.GetUrlsForUri( typeof(ServerManager).Name )[0] ));
               
            // Register client activated obje FileReceiver
            RemotingConfiguration.RegisterActivatedServiceType( typeof(FileReceiver) );



             // management console requests
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ManagementConsoleRequest),
                                                               typeof(ManagementConsoleRequest).Name,
                                                               WellKnownObjectMode.SingleCall);
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                   String.Format("{0} is registered as a remotable object.  URL: {1}",
                                   typeof(ManagementConsoleRequest),
                                   tcpChannel.GetUrlsForUri(typeof(ManagementConsoleRequest).Name)[0]));
                        
            // Get registered service types
            ActivatedServiceTypeEntry [] types = RemotingConfiguration.GetRegisteredActivatedServiceTypes();

            for( int i = 0; i < types.Length; i ++ )
            {
               ErrorLog.Instance.Write( ErrorLog.Level.Debug, String.Format("{0} is registered.", types[i].ObjectType) );
            }
            RemotingConfiguration.ApplicationName = "SQLcompliance";
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                    String.Format("{0} is registered as a client activated remotable object.  Url: {1}.  ApplicationName: {2}.",
                                    typeof(FileReceiver),
                                    tcpChannel.GetUrlsForUri(typeof(FileReceiver).Name)[0],
                                    RemotingConfiguration.ApplicationName));
         }
         catch( Exception e )
         {
            // Serious problem.
            // TODO: add a constant string and replace the errorlog with the right error message.
            ErrorLog.Instance.Write( e, true );
            // rethrow the exception so the service can be stopped.
            throw e;
         }
      }

      //-----------------------------------------------------------------------
      // CheckAutoArchiveTimeZone - it this has never been set set
      //                            to local timezone
      //-----------------------------------------------------------------------
      private void CheckAutoArchiveTimeZone(SqlConnection conn)
      {
         SQLcomplianceConfiguration config = new SQLcomplianceConfiguration();
         config.Read(conn);
         
         if ( config.ArchiveTimeZoneName == "" )
         {
            TimeZoneInfo tzi = TimeZoneInfo.CurrentTimeZone;
            config.ArchiveTimeZoneName = tzi.Name;
            config.ArchiveBias         = tzi.TimeZoneStruct.Bias;
            config.ArchiveStandardBias = tzi.TimeZoneStruct.StandardBias;
            config.ArchiveStandardDate = SystemTime.ToTimeZoneDateTime(tzi.TimeZoneStruct.StandardDate);
            config.ArchiveDaylightBias = tzi.TimeZoneStruct.DaylightBias;
            config.ArchiveDaylightDate = SystemTime.ToTimeZoneDateTime(tzi.TimeZoneStruct.DaylightDate);
            config.Write(conn);
         }
      }

      //-----------------------------------------------------------------------
      // CheckSQLPrivileges - (1) Make sure service account has necessary privs in
      //                          repository SQL Server
      //-----------------------------------------------------------------------
      private void CheckSQLPrivileges(SqlConnection conn)
      {
         if (!RawSQL.IsCurrentUserSysadmin(conn))
			{
            ErrorLog.Instance.Write( CoreConstants.Exception_ServiceAccountNotSysadmin,
                                     SQLcomplianceConfiguration.GetLastError(),
                                     ErrorLog.Severity.Error );
                // SQLCM 5.6- 566/740/4620/5280 (Non-Admin and Non-Sysadmin role) Permissions
                if (!RawSQL.HasSufficientPermissions(conn))
                {
                    ErrorLog.Instance.Write(CoreConstants.Exception_ServiceAccountNotSysadmin,
                                     SQLcomplianceConfiguration.GetLastError(),
                                     ErrorLog.Severity.Error);
                    throw new Exception(CoreConstants.Exception_ServiceAccountNotSysadmin);
                }
            }
      }
      
      //-----------------------------------------------------------------------
      // CheckRepositorySchema - Compare code database schema version against
      //                         that in the database to make sure we are
      //                         compatible - in future versions we can add
      //                         code here to upgrade a database if we want
      //-----------------------------------------------------------------------
      private void CheckRepositorySchema( SqlConnection conn )
      {
         if(!SQLcomplianceConfiguration.IsCompatibleSchema( conn ))
 	      {
            ErrorLog.Instance.Write( CoreConstants.Exception_UnsupportedRepositoryVersion, ErrorLog.Severity.Error );
			   throw new Exception(CoreConstants.Exception_UnsupportedRepositoryVersion);
 	      }
      }
      
      //-----------------------------------------------------------------------
      // CheckMainDatabases - check for SQLcompliance and SQLcompliance.Processing
      //-----------------------------------------------------------------------
      private void CheckMainDatabases( SqlConnection conn )
      {
         
         if (!EventDatabase.DatabaseExists(CoreConstants.RepositoryDatabase, conn) ||
             !EventDatabase.DatabaseExists(CoreConstants.RepositoryTempDatabase, conn))
         {
            throw new Exception (CoreConstants.Exception_MissingSQLcomplianceDatabase);
         }
      }
      //---------------------------------------------------------------
      // ClearInProgressFlags - if collection server is shut
      //    down while archive or integrity check is in progress,
      //    state will be left at in progress - change these to incomplete
      //---------------------------------------------------------------
      internal void ClearInProgressFlags( SqlConnection conn )
      {
         ArrayList databases = new ArrayList();
         
         try
         {
            // Servers Table
            string sql2 = String.Format( "UPDATE {0}..{1} SET lastIntegrityCheckResult={2} WHERE lastIntegrityCheckResult={3}",
                                         CoreConstants.RepositoryDatabase,
                                         CoreConstants.RepositoryServerTable,
                                         CoreConstants.IntegrityCheck_Incomplete,
                                         CoreConstants.IntegrityCheck_InProgress );
            using ( SqlCommand cmd = new SqlCommand( sql2,conn ) )
            {
               cmd.ExecuteNonQuery();
            }                                                    
            sql2 = String.Format("UPDATE {0}..{1} SET lastArchiveResult={2} WHERE lastArchiveResult={3}",
                                 CoreConstants.RepositoryDatabase,
                                 CoreConstants.RepositoryServerTable,
                                 CoreConstants.Archive_Incomplete,
                                 CoreConstants.Archive_InProgress );
            using ( SqlCommand cmd = new SqlCommand( sql2,conn ) )
            {
               cmd.ExecuteNonQuery();
            }                                                    
            
            // Archive Database
            string query = ArchiveJob.GetArchiveDatabasesStatement();

            using (SqlCommand command = new SqlCommand(query, conn))
            {
	            command.CommandTimeout = CoreConstants.sqlcommandTimeout;
	            
               using (SqlDataReader reader = command.ExecuteReader())
               {
                  if (reader != null)
                  {
                     while (reader.Read())
                     {
                        string db = reader.GetString(0);

                        if (db != "") 
                        {
                           databases.Add(db); 
                        }
                     }
                  }
               }

               for( int i = 0; i < databases.Count; i++ )
               {
                  try
                  {
                     string sql = String.Format( "UPDATE {0}..{1} SET lastIntegrityCheckResult={2} WHERE lastIntegrityCheckResult={3}",
                                                SQLHelpers.CreateSafeDatabaseName((string)databases[i]),
                                                CoreConstants.RepositoryArchiveMetaTable,
                                                CoreConstants.IntegrityCheck_Incomplete,
                                                CoreConstants.IntegrityCheck_InProgress );
                     using ( SqlCommand cmd = new SqlCommand( sql,conn ) )
                     {
                        cmd.ExecuteNonQuery();
                     }
                  }
                  catch {}
               }
            }
         }
         catch( Exception e ) 
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                    "An error updating in progress integrity check flags for archive databases.",
                                    e,
                                    true);
         }
      }
      
      internal void CheckServerStatus()
      {
         if ( aborting )
         {
            // server shutting down 
            throw new Exception( CoreConstants.Exception_ServerShuttingDown );
         }
      }

      //---------------------------------------------------------------
      // SkipAdminCheck
      //---------------------------------------------------------------
      public bool SkipAdminCheck()
      {
         RegistryKey   rk              = null;
         RegistryKey   rks             = null;
         bool          skipAdminCheck  = false;

         try
         {
            rk = Registry.LocalMachine;
            rks = rk.OpenSubKey(CoreConstants.CollectionService_RegKey);
            int val = (int)rks.GetValue( CoreConstants.CollectionService_RegVal_SkipAdminCheck, 0 );
            skipAdminCheck = (val == 1) ? true : false;
         }
         catch
         {
            ErrorLog.Instance.Write( "Error accessing SQLcompliance Collection Service registry - Check that the service account has appropriate permissions for reading the registry.",
                                     ErrorLog.Severity.Error );
         }
         finally
         {
            if( rks != null ) rks.Close();
            if( rk != null )  rk.Close();
         }
         return skipAdminCheck;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      public string GetServerSettings()
      {
         string retVal = String.Format(CoreConstants.Info_ServerSettings,
                                       System.Reflection.Assembly.GetEntryAssembly().GetName().Name,
                                       System.Reflection.Assembly.GetEntryAssembly().GetName().Version,
                                       System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
                                       System.Reflection.Assembly.GetExecutingAssembly().GetName().Version,
                                       serverInstance,
                                       traceDirectory,
                                       serverPort,
                                       agentPort,
                                       logLevel);
         if (BetaHelper.IsBeta)
         {
            retVal += "\n\n";
            retVal += BetaHelper.Title_Beta;
         }
         return retVal ;
      }

      //
      // MirrorAllowCaptureSql()
      //
      //  The option to disable AllowCaptureSql is managed by the collection service in the
      //  registry.  We mirror this value to the repository in case the collection service is
      //  not available to consoles.
      //  0 - CaptureSql allowed, CaptureSql Off
      //  1 - CaptureSql allowed, CaptureSql On
      //  2 - CaptureSql not allowed
      //
      private void MirrorAllowCaptureSql( SqlConnection conn )
      {
         int rows = 0 ;
         
         try
         {
            string query ;

            // Handle this for Database Settings
            if(CoreConstants.AllowCaptureSql)
            {
               // Set any 2 values from previous disallow to 0
               query = String.Format("UPDATE {0}..{1} SET auditCaptureSQL=0 WHERE auditCaptureSQL=2",
                                     CoreConstants.RepositoryDatabase,
                                     CoreConstants.RepositoryDatabaseTable) ;
            }
            else
            {
               // Set them all to 2
               query = String.Format("UPDATE {0}..{1} SET auditCaptureSQL=2",
                                     CoreConstants.RepositoryDatabase,
                                     CoreConstants.RepositoryDatabaseTable) ;
            }
            
            using ( SqlCommand cmd = new SqlCommand(query,conn ) )
            {
               rows += cmd.ExecuteNonQuery();
            }                                                    

            //  Handle it for priv users
            if(CoreConstants.AllowCaptureSql)
            {
               // Set any 2 values from previous disallow to 0
               query = String.Format("UPDATE {0}..{1} SET auditUserCaptureSQL=0 WHERE auditUserCaptureSQL=2",
                                     CoreConstants.RepositoryDatabase,
                                     CoreConstants.RepositoryServerTable) ;
            }
            else
            {
               // Set them all to 2
               query = String.Format("UPDATE {0}..{1} SET auditUserCaptureSQL=2",
                                     CoreConstants.RepositoryDatabase,
                                     CoreConstants.RepositoryServerTable) ;
            }
            using ( SqlCommand cmd = new SqlCommand(query,conn ) )
            {
               rows += cmd.ExecuteNonQuery();
            }              
           
            // modification to mirroring should not occur frequently, so we do a lazy update
            //  of all server audit setting versions.  This forces agents to update.
            if (rows > 0)
            {
               ServerRecord.IncrementServerConfigVersionAll(conn);
            }
         }
         catch( Exception e ) 
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug, "An error updating AuditCaptureSQL status.", e, true );
         }
      }

	    private void ActivateInactiveAgents()
	    {
	        RegistryKey collectionServerKey = null;

	        try
	        {
                // open collection server key
	            collectionServerKey = Registry.LocalMachine.OpenSubKey(CoreConstants.CollectionService_RegKey, true);
	            if (collectionServerKey == null)
	                return;

                // read flag set by installer to check if repository has been preserved during install
                object isRepositoryUpgarde = collectionServerKey.GetValue(CoreConstants.CollectionService_RegVal_IsRepositoryPreserved);
	            if (isRepositoryUpgarde == null)
	                return;

                // open cnnection to repository
                Repository repository = new Repository();
	            if (!repository.OpenConnection( CoreConstants.RepositoryDatabase, false))
                    return;

                // try to activate all SQL server audited instances stored in repository on agents
	            bool allInstancesActivated = true;
	            foreach (ServerRecord server in ServerRecord.GetServers(repository.connection, true, false))
	            {
	                string collectionServer = server.Server.Equals("(local)", StringComparison.InvariantCultureIgnoreCase) ||
	                                          server.Server.Equals(".", StringComparison.InvariantCultureIgnoreCase)
	                                          ? Environment.MachineName
	                                          : server.Server;
	                int collectionServerPort = server.ServerPort;

	                if (!ActivateSqlServerInstance(server.Instance, 
                                                   server.AgentTraceDirectory,
	                                               collectionServer,
                                                   collectionServerPort))
	                    allInstancesActivated = false;
	            }

                // close connection to repository
                if (repository.connection.State == ConnectionState.Open)
                    repository.CloseConnection();

                // on successful activation, delete flag
                if (allInstancesActivated)
                    collectionServerKey.DeleteValue(CoreConstants.CollectionService_RegVal_IsRepositoryPreserved);
	        }
	        catch
	        {
                // do nothing
	        }
	        finally
	        {
	            if (collectionServerKey != null)
                    collectionServerKey.Close();
	        }
	    }

        private bool ActivateSqlServerInstance(string monitoredInstance, string agentTraceDirectory, string collectionServer, int collectionServerPort)
        {
            bool isActivated = false;

            try
            {
                // url to register with agent
                AgentManager manager = CoreRemoteObjectsProvider.AgentManager(collectionServer, collectionServerPort);
                manager.Activate(monitoredInstance, collectionServer, agentTraceDirectory);
                isActivated = true;
            }
            catch (Exception ex)
            {
                // we will do nothing if activation attempt fails
            }

            return isActivated;
        }

      #endregion 
   }
}
