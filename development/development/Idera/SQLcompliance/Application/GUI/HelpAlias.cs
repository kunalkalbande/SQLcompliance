using System;
using System.Diagnostics;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;

namespace Idera.SQLcompliance.Application.GUI
{
   /// <summary>
   /// This class contains Help context id string constants and methods to show help.
   /// </summary>
   public class HelpAlias
   {
       private const string WikiUrl = "http://wiki.idera.com/x/";

      private HelpAlias()
      {
      }

      public static void ShowHelp(Control control, string topic)
      {
         if (string.IsNullOrEmpty(topic))
             return;

         // Obsolete: From SQLcm 4.5 CHM help is depricated
         //IntPtr hWnd;
         //if (!Idera.WebHelp.WebHelpLauncher.TryShowWebHelp(topic, out hWnd))
         //{
         //   ShowHelpChm(control, topic);
         //}

          LaunchWebBrowser(string.Format("{0}{1}", WikiUrl, topic));
      }

      public static void ShowHelpChm(Control control,string topic)
      {
         string helpfilepath = AppDomain.CurrentDomain.BaseDirectory + SQLSECURECHMFILE;

         try
         {
            System.Windows.Forms.Help.ShowHelp(control,
                          helpfilepath,
                          HelpNavigator.Topic,
                          topic);
         }
         catch (Exception ex)
         {
            ErrorMessage.Show("SQL Compliance Manager Help",
                              String.Format(UIConstants.Error_CantLoadHelpFile,
                                            helpfilepath),
                              ex.Message);
         }
      }

      public static void LaunchWebBrowser(string url)
      {
         try
         {
            Process.Start(url);
         }
         catch
         {
         }
      }

      //---------------------
      // Help File Constants
      //---------------------
      public static string SQLSECURECHMFILE = "SQL Compliance Manager Help.chm";

       public static string SSHELP_TECHSUPP = "4QE1"; //@"About Idera.htm";
       public static string SSHELP_GETSTARTED = "CwI1"; //@"Getting Started.htm";

       public static string SSHELP_HowAuditingWorks = "QgI1"; //@"How Auditing Works.htm";
       public static string SSHELP_HowConsoleSecurityWorks = "ewI1"; //@"How Console Security Works.htm";
       public static string SSHELP_ReportingOnAuditData = "gwI1"; //@"Reporting on Audit Data.htm";
       public static string SSHELP_AuditingBestPractices = "XQI1"; //@"Reducing Audit Data Optimize Perf.htm";
       public static string SSHELP_MigrateCollectionServer = "PQM1"; //@"Migrate Collection Server.htm";
       public static string SSHELP_SensitiveColumnSearch = "HwAZBw"; //@"Sensitive Column Search window.htm";
       public static string SSHELP_SearchForHelp = "4AE1"; //@"Start Help.htm";
       public static string SSHELP_ViewContents = "4AE1"; //@"Start Help.htm";

      // stuff displayed in right side of main page
       public static string SSHELP_EventView = "sgI1"; //@"Windows\Audit Events Tab.htm";
       public static string SSHELP_LoginsView = "KAM1"; //@"Windows\SQL Logins Tab.htm";
       public static string SSHELP_ServerView = "IgM1"; //@"Windows\Registered SQL Servers Tab.htm";
       public static string SSHELP_ArchiveView = "sAI1"; //@"Windows\Archived Events Tab.htm";

       public static string SSHELP_EventFiltersView = "9wI1"; //@"Windows\Event Filters Tab.htm";
       public static string SSHELP_AlertRulesView = "qwI1"; //@"Windows\Alert Rules Tab.htm";
       public static string SSHELP_AlertView = "9gI1"; //@"Windows\Alerts Tab.htm";
       public static string SSHELP_StatusAlertView = "LgM1"; //@"Windows\Status Alerts Tab.htm";
       public static string SSHELP_DataAlertView = "3QI1"; //@"Windows\Data Alerts Tab.htm";

      // dialogs

      // Agent Properties
       public static string SSHELP_Form_AgentProperties_General = "KgM1"; //@"Windows\SQLcompliance Agent Properties General.htm";
       public static string SSHELP_Form_AgentProperties_Servers = "KwM1"; //@"Windows\SQLcompliance Agent Properties Servers.htm";
       public static string SSHELP_Form_AgentProperties_Deploy = "KQM1"; //@"Windows\SQLcompliance Agent Properties Deployment.htm";
       public static string SSHELP_Form_AgentProperties_Trace = "LAM1"; //@"Windows\SQLcompliance Agent Properties Trace Options.htm";

      // Agent Trace Directory
       public static string SSHELP_Form_AgentTraceDirectory = "LQM1"; //@"Windows\SQLcompliance Agent Trace.htm";

      // Agent Deploy
       public static string SSHELP_Form_ServerDeploy_Account = "3gI1"; //@"Windows\Deploy Agent Wizard Service Account.htm";
       public static string SSHELP_Form_ServerDeploy_Trace = "4AI1"; //@"Windows\Deploy Agent Wizard Trace Directory.htm";
       public static string SSHELP_Form_ServerDeploy_Summary = "3wI1"; //@"Windows\Deploy Agent Wizard Summary.htm";

      // Archive Now
       public static string SSHELP_Form_Archive = "rQI1"; //@"Windows\Archive Preferences Window.htm";

      // Import Archive
       public static string SSHELP_Form_ArchiveImport = "sQI1"; //@"Windows\Attach Archive Database Window.htm";

      // Archive Schedule Options
       public static string SSHELP_Form_ArchiveOptions = "rQI1"; //@"Windows\Archive Preferences Window.htm";

      // Archive Properties
       public static string SSHELP_Form_ArchiveProperties_General = "rwI1"; //@"Windows\Archive Properties General.htm";
       public static string SSHELP_Form_ArchiveProperties_Permissions = "rgI1"; //@"Windows\Archive Properties Default Permissions.htm";

      // Change Log Properties
       public static string SSHELP_Form_ChangeLogProperties = "vAI1"; //@"Windows\Change Log Properties Window.htm";

      // Activity Log Properties
       public static string SSHELP_Form_ActivityLogProperties = "pAI1"; //@"Windows\Activity Log Properties Window.htm";

      // Connect		
       public static string SSHELP_Form_Connect = "2gI1"; //@"Windows\Connect to Repository Window.htm";

      // Database Properties
       public static string SSHELP_Form_DatabaseProperties_General = "uAI1"; //@"Windows\Database Properties General.htm";
       public static string SSHELP_Form_DatabaseProperties_Activities = "tQI1"; //@"Windows\Database Properties Activities.htm";
       public static string SSHELP_Form_DatabaseProperties_Objects = "twI1"; //@"Windows\Database Properties DML Select.htm";
       public static string SSHELP_Form_DatabaseProperties_TrustedUsers = "ugI1"; //@"Windows\Database Properties Trusted Users.htm";
       public static string SSHELP_Form_DatabaseProperties_BeforeAfterData = "tgI1"; //@"Windows\Database Properties BAD.htm";
       public static string SSHELP_Form_DatabaseProperties_SensitiveColumns = "uQI1"; //@"Windows\Database Properties Sensitive Columns.htm";

      // Event Properties
       public static string SSHELP_Form_EventProperties_General = "_gI1"; //@"Windows\Event Properties Window General.htm";
       public static string SSHELP_Form_EventProperties_Details = "_QI1"; //@"Windows\Event Properties Window Details.htm";
       public static string SSHELP_Form_EventProperties_Data = "_AI1"; //@"Windows\Event Properties Window Data.htm";

      // Groom
       public static string SSHELP_Form_Groom = "-wI1"; //@"Windows\Groom Audit Data Now Window.htm";

      // Integrity Check
       public static string SSHELP_Form_IntegrityCheck = "vgI1"; //@"Windows\Check Repository Integrity.htm";

      // Integrity Check REsults
       public static string SSHELP_Form_IntegrityCheckResults = "BQM1"; //@"Windows\Integrity Check Results Window.htm";


      // License Management
       public static string SSHELP_Form_LicenseManagement = "CQM1"; //@"Windows\Manage Lincenses Window.htm";

      // Login New
       public static string SSHELP_Form_LoginNew_General = "GAM1"; //@"Windows\New SQL Server Login Wizard Authentication.htm";
       public static string SSHELP_Form_LoginNew_Access = "FwM1"; //@"Windows\New SQL Server Login Wizard Permissions.htm";
       public static string SSHELP_Form_LoginNew_Summary = "GQM1"; //@"Windows\New SQL Server Login Wizard Summary.htm";

      // Login Properties
       public static string SSHELP_Form_LoginProperties_General = "CAM1"; //@"Windows\Login Properties Window General.htm";
       public static string SSHELP_Form_LoginProperties_Database = "BwM1"; //@"Windows\Login Properties Window Database.htm";

      // Login Filtering
       public static string SSHELP_Form_LoginFiltering = "BgM1"; //@"Windows\Login Filtering Options Window.htm";

      // Preferences (Customize)
       public static string SSHELP_Form_Preferences = "3AI1"; //@"Windows\Console Preferences Event Views.htm";
       public static string SSHELP_Alerts_Console_Preferences = "2wI1"; //@"Windows\Console Preferences Alert Views.htm";

      // Priv User Add
       public static string SSHELP_Form_PrivUser = "pgI1"; //@"Windows\Add Privileged Users Window.htm";

      // Repository Database Options
       public static string SSHELP_Form_RepositoryOptions_RecoveryModel = "2AI1"; //@"Windows\Configure Repository Recovery Model.htm";
       public static string SSHELP_Form_RepositoryOptions_Databases = "1wI1"; //@"Windows\Configure Repository Databases.htm";

      // Server/Database Registration Wizard
       public static string SSHELP_Form_ConfigWizard_ServerActivities = "zwI1"; //@"Windows\Configuration Wizard Server Audit Settings.htm";
       public static string SSHELP_Form_ConfigWizard_PrivUsers = "ywI1"; //@"Windows\Configuration Wizard Privileged Users.htm";
       public static string SSHELP_Form_ConfigWizard_PrivUserSettings = "zAI1"; //@"Windows\Configuration Wizard Privileged Users Activity.htm";
       public static string SSHELP_Form_ConfigWizard_Deploy = "0QI1"; //@"Windows\Configuration Wizard SQLcompliance Agent Deployment.htm";
       public static string SSHELP_Form_ConfigWizard_Account = "0gI1"; //@"Windows\Configuration Wizard Agent Service Account.htm";
       public static string SSHELP_Form_ConfigWizard_Trace = "0wI1"; //@"Windows\Configuration Wizard Trace Directory.htm";
       public static string SSHELP_Form_ConfigWizard_Permissions = "xQI1"; //@"Windows\Configuration Wizard Default Permissions.htm";
       public static string SSHELP_Form_ConfigWizard_Summary = "1AI1"; //@"Windows\Configuration Wizard Summary.htm";
       public static string SSHELP_Form_ConfigWizard_Error = "ygI1"; //@"Windows\Configuration Wizard License Limit.htm";
       public static string SSHELP_Form_ConfigWizard_ExistingDatabase = "yAI1"; //@"Windows\Configuration Wizard Existing Audit Data.htm";
       public static string SSHELP_Form_ConfigWizard_IncompatibleDatabase = "yQI1"; //@"Windows\Configuration Wizard Incomplete Database.htm";
       public static string SSHELP_Form_ConfigWizard_IsCluster = "0AI1"; //@"Windows\Configuration Wizard SQLServer Cluster.htm";
       public static string SSHELP_Form_ConfigWizard_AddServer = "wQI1"; //@"Windows\Configuration Wizard Add Server.htm";
       public static string SSHELP_Form_ConfigWizard_AuditLevel = "wwI1"; //@"Windows\Configuration Wizard Audit Level.htm";
       public static string SSHELP_Form_ConfigWizard_DMLFilters = "xgI1"; //@"Windows\Configuration Wizard DML and SELECT.htm";
       public static string SSHELP_Form_ConfigWizard_DatabaseActivities = "xAI1"; //@"Windows\Configuration Wizard Database Audit Settings.htm";
       public static string SSHELP_Form_ConfigWizard_AddDatabase = "wAI1"; //@"Windows\Configuration Wizard Add Databases.htm";
       public static string SSHELP_Form_ConfigWizard_TrustedUsers = "1QI1"; //@"Windows\Configuration Wizard Trusted Users.htm";
       public static string SSHELP_Form_ConfigWizard_ServerTrustedUsers = "1QI1"; //@"Windows\Configuration Wizard Server Level Trusted Users.htm";
       public static string SSHELP_Form_ConfigWizard_ApplyRegulation = "wgI1"; //@"Windows\Configuration Wizard Apply Regulation.htm";
       public static string SSHELP_Form_ConfigWizard_RegulationInfo = "xwI1"; //@"Windows\Configuration Wizard Enforce Regulation Guidelines.htm";
       public static string SSHELP_Form_ConfigWizard_SensitiveColumns = "zgI1"; //@"Windows\Configuration Wizard Sensitive Columns.htm";


      // Server Options
       public static string SSHELP_Form_ServerOptions = "vwI1"; //@"Windows\Collection Server Status Window.htm";

      // Server properties
       public static string SSHELP_Form_ServerProperties_General = "IAM1"; //@"Windows\Registered SQL Server Properties General.htm";
       public static string SSHELP_Form_ServerProperties_Activities = "HgM1"; //@"Windows\Registered SQL Server Properties Audited Activities.htm";
       public static string SSHELP_Form_ServerProperties_Advanced = "HQM1"; //@"Windows\Registered SQL Server Properties Advanced.htm";
       public static string SSHELP_Form_ServerProperties_Thresholds = "HwM1"; //@"Windows\Registered SQL Server Properties Auditing Thresholds.htm";
       public static string SSHELP_Form_ServerProperties_PrivUsers = "IQM1"; //@"Windows\Registered SQL Server Properties Privileged User.htm";

      // Snapshot capture
       public static string SSHELP_Form_Snapshot = "uwI1"; //@"Windows\Capture Audit Snapshot Window.htm";

      // Snapshot Options
       public static string SSHELP_Form_SnapshotOptions = "tAI1"; //@"Windows\Audit Snapshot Preferences.htm";

      // SQL Server Browse
       public static string SSHELP_Form_SQLServerBrowse = "IwM1"; //@"Windows\Select SQL Server Window.htm";

      // Add user tables
       public static string SSHELP_Form_TableAdd_BAD = "pwI1"; //@"Windows\Add User Tables Window BAD.htm";
       public static string SSHELP_Form_TableAdd_DML = "qAI1"; //@"Windows\Add User Tables Window DML.htm";
       public static string SSHELP_Form_TableAdd_SC = "qQI1"; //@"Windows\Add User Tables Window SC.htm";

      // Alerting Help
       public static string SSHELP_AlertingGeneral = "ZgI1"; //@"How Alerting Works.htm";

       public static string SSHELP_NewAlertWizard_Filters = "DgM1"; //@"Windows\New Alert Rule Wizard Event Filters.htm";
        public static string SSHELP_NewAlertWizard_AlertRuleTimeFrame = "HCjrBw";
        public static string SSHELP_NewAlertWizard_Actions = "DwM1"; //@"Windows\New Alert Rule Wizard Actions.htm";
       public static string SSHELP_NewAlertWizard_Finish = "EAM1"; //@"Windows\New Alert Rule Wizard Finish.htm";
       public static string SSHELP_NewAlertWizard_EventType = "EQM1"; //@"Windows\New Alert Rule Wizard Event Type.htm";
       public static string SSHELP_NewAlertWizard_ObjectType = "EgM1"; //@"Windows\New Alert Rule Wizard Object Type.htm";
       public static string SSHELP_EditAlertWizard_Filters = "6gI1"; //@"Windows\Edit Alert Rule Wizard Event Filters.htm";
       public static string SSHELP_EditAlertWizard_Actions = "6wI1"; //@"Windows\Edit Alert Rule Wizard Alert Actions.htm";
       public static string SSHELP_EditAlertWizard_Finish = "7AI1"; //@"Windows\Edit Alert Rule Wizard Finish.htm";
       public static string SSHELP_EditAlertWizard_EventType = "7QI1"; //@"Windows\Edit Alert Rule Wizard Event Type.htm";
       public static string SSHELP_EditAlertWizard_ObjectType = "7gI1"; //@"Windows\Edit Alert Rule Wizard Object Type.htm";
       public static string SSHELP_AlertWizard_StringSearch = "JgM1"; //@"Windows\Specify Alert Criteria Windows.htm";
       public static string SSHELP_Alerts_Groom = "-gI1"; //@"Windows\Groom Alerts Now Window.htm";
       public static string SSHELP_Alerts_Distribution_List = "JQM1"; //@"Windows\Specify Addresses Window.htm";
       public static string SSHELP_Alert_Message_Template = "qgI1"; //@"Windows\Alert Message Template Window.htm";

       public static string SSHELP_NewStatusAlertWizard_Type = "HAM1"; //@"Windows\New Status Alert Wizard Type.htm";
      public static string SSHELP_NewStatusAlertWizard_Action = "GgM1"; //@"Windows\New Status Alert Wizard Action.htm";
       public static string SSHELP_NewStatusAlertWizard_Finish = "GwM1"; //@"Windows\New Status Alert Wizard Finish.htm";

       public static string SSHELP_NewDataAlertWizard_Type = "CwM1"; //@"Windows\New Data Alert Rule Wizard Type.htm";
       public static string SSHELP_NewDataAlertWizard_Object = "DQM1"; //@"Windows\New Data Alert Rule Wizard Object.htm";
       public static string SSHELP_NewDataAlertWizard_Filters = "DgM1"; //@"Windows\New Data Alert Rule Wizard Event Filters.htm";
       public static string SSHELP_NewDataAlertWizard_Action = "CgM1"; //@"Windows\New Data Alert Rule Wizard Actions.htm";
       public static string SSHELP_NewDataAlertWizard_Finish = "DAM1"; //@"Windows\New Data Alert Rule Wizard Finish.htm";

       public static string SSHELP_EditDataAlertWizard_Type = "5wI1"; //@"Windows\Edit Data Alert Rule Wizard Type.htm";
       public static string SSHELP_EditDataAlertWizard_Object = "6QI1"; //@"Windows\Edit Data Alert Rule Wizard Object.htm";
       public static string SSHELP_EditDataAlertWizard_Filters = "6gI1"; //@"Windows\Edit Data Alert Rule Wizard Event Filters.htm";
       public static string SSHELP_EditDataAlertWizard_Action = "5gI1"; //@"Windows\Edit Data Alert Rule Wizard Actions.htm";
       public static string SSHELP_EditDataAlertWizard_Finish = "6AI1"; //@"Windows\Edit Data Alert Rule Wizard Finish.htm";

      // Event Filters Help
       public static string SSHELP_NewEventFilterWizard_EventSource = "FAM1"; //@"Windows\New Event Filter Wizard Event Source.htm";
       public static string SSHELP_NewEventFilterWizard_Finish = "EwM1"; //@"Windows\New Event Filter Wizard Finish.htm";
       public static string SSHELP_NewEventFilterWizard_EventType = "FQM1"; //@"Windows\New Event Filter Wizard Event Type.htm";
       public static string SSHELP_NewEventFilterWizard_ObjectType = "FgM1"; //@"Windows\New Event Filter Wizard Object Type.htm";
       public static string SSHELP_EditEventFilterWizard_EventSource = "8QI1"; //@"Windows\Edit Event Filter Wizard Event Source.htm";
       public static string SSHELP_EditEventFilterWizard_Finish = "7wI1"; //@"Windows\Edit Event Filter Wizard Finish.htm";
       public static string SSHELP_EditEventFilterWizard_EventType = "8gI1"; //@"Windows\Edit Event Filter Wizard Event Type.htm";
       public static string SSHELP_EditEventFilterWizard_ObjectType = "8AI1"; //@"Windows\Edit Event Filter Wizard Event Object.htm";
       public static string SSHELP_EventFilterWizard_StringSearch = "JwM1"; //@"Windows\Specify Event Filter Criteria.htm";

       public static string SSHELP_SMTP_Settings = "1gI1"; //@"Windows\Configure Email Settings Window.htm";
       public static string SSHELP_Index_Update_Information = "LwM1"; //@"Windows\Update Indexes Window.htm";
       public static string SSHELP_Index_Schedule_Information = "JAM1"; //@"Windows\Set Maintenance Schedule Window.htm";

       public static string SSHELP_ChangeLogView = "vQI1"; //@"Windows\Change Log Tab.htm";
       public static string SSHELP_ActivityLogView = "pQI1"; //@"Windows\Activity Log Tab.htm";
       public static string SSHELP_ReportsView = "swI1"; //@"Windows\Audit Reports.htm";

       public static string SSHELP_EnterpriseSummary = "_wI1"; //@"Windows\Explore Activity Audited SQL Servers Summary.htm";
       public static string SSHELP_InstanceSummary = "-QI1"; //@"Windows\Explore Activity Instance.htm";
       public static string SSHELP_DatabaseSummary = "-AI1"; //@"Windows\Explore Activity Database.htm";

       public static string SSHELP_ImportAuditSettings_SelectFile = "AQM1"; //@"Windows\Import Audit Settings Select.htm";
       public static string SSHELP_ImportAuditSettings_TargetServers = "BAM1"; //@"Windows\Import Audit Settings Target Servers.htm";
       public static string SSHELP_ImportAuditSettings_TargetDatabases = "AwM1"; //@"Windows\Import Audit Settings Target Databases.htm";
       public static string SSHELP_ImportAuditSettings_Summary = "AgM1"; //@"Windows\Import Audit Settings Summary.htm";
       public static string SSHELP_ImportAuditSettings_SelectSettings = "AAM1"; //@"Windows\Import Audit Settings Import.htm";

      public static string SSHELP_DeployReportsWizard_Welcome = "5QI1"; //@"Windows\Deploy Reports Wizard Welcome.htm";
       public static string SSHELP_DeployReportsWizard_Location = "4gI1"; //@"Windows\Deploy Reports Wizard Location.htm";
       public static string SSHELP_DeployReportsWizard_Connect = "4QI1"; //@"Windows\Deploy Reports Wizard Connect.htm";
       public static string SSHELP_DeployReportsWizard_Repository = "4wI1"; //@"Windows\Deploy Reports Wizard Repository.htm";
       public static string SSHELP_DeployReportsWizard_Summary = "5AI1"; //@"Windows\Deploy Reports Wizard Summary.htm";

       public static string SSHELP_AuditVirtualInstance = "QgM1"; //@"Audit Virtual Instance.htm";

       public static string SSHELP_PermissionsCheck = "IINi";
       public static string SSHELP_PermissionsCheckFailed = "KINi";
   }
}