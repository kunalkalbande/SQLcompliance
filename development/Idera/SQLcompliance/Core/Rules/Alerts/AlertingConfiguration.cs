using System;
using System.Collections ;
using System.Collections.Generic;
using Idera.SQLcompliance.Core.TimeZoneHelper ;
using TimeZoneInfo = Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;

namespace Idera.SQLcompliance.Core.Rules.Alerts
{

   public enum AlertableEventFields
   {
      eventType,
      eventCategory,
      applicationName,
      loginName,
      success,
      databaseName,
      objectName,
      privilegedUser,
      objectType,
      serverName,
      hostName,
      sessionLoginName,
      columnName,
      privilegedUsers,
      rowCount,
      dataRuleApplicationName,
      dataRuleLoginName
   } ;
	/// <summary>
	/// Summary description for AlertingConfiguration.
	/// </summary>
	public class AlertingConfiguration
	{
		#region Member Variables

		public const int NULL_ID = -2100000001;

      public static string AlertMessageTitle = "$AlertLevel$ Alert on $ServerName$ instance" ;
      public static string AlertMessageBody = "$EventType$ event occurred on $ServerName$ instance at $EventTime$ by $Login$." ;

      public static string StatusAlertMessageTitle = "$AlertLevel$ Status Alert";
      public static string StatusAlertMessageBody = "An event of \"$AlertTypeName$\" occurred on Computer:$ComputerName$ Instance:$ServerName$ at $AlertTime$";

      public static string DataAlertMessageTitle = "$AlertLevel$ Data Alert";
      public static string DataAlertMessageBody = "An event of \"$AlertType$\" occurred on $ServerName$ $EventTime$";

      private string _hostInstance;
		private string _connectionString ;

		private Hashtable _sqlServerFields ;

      private Dictionary<int, CMEventCategory> _sqlServerCategories;

		private Hashtable _sqlServerEvents ;

		private MacroDefinition[] _sqlServerMacros ;
      private MacroDefinition[] _statusMacros ;
      private MacroDefinition[] _dataMacros;

		private SmtpConfiguration _smtpSettings ;
		private static Hashtable _timeZoneLookup ;

	    private SNMPConfiguration _snmpConfiguration;

	    #endregion

		#region Properties

		public CMEventCategory[] SqlServerCategories
		{
			get 
			{ 
				CMEventCategory[] retVal = new CMEventCategory[_sqlServerCategories.Values.Count] ;
				_sqlServerCategories.Values.CopyTo(retVal, 0) ;
				return retVal ;
			}
		}


		public MacroDefinition[] SqlServerMacros
		{
			get { return _sqlServerMacros ; }
		}

      public MacroDefinition[] DataMacros
      {
         get { return _dataMacros; }
      }

      public MacroDefinition[] StatusMacros
      {
         get { return _statusMacros ; }
      }

      public Hashtable SqlServerEventMap
		{
			get { return _sqlServerEvents ; }
		}
      
		public string ConnectionString
		{
			get { return _connectionString ; }
		}

		public string HostInstance
		{
			get { return _hostInstance ; }
		}

		public SmtpConfiguration SmtpSettings
		{
			get { return _smtpSettings ; }
			set { _smtpSettings = value ; }
		}

	    public SNMPConfiguration SnmpConfiguration
	    {
	        get { return _snmpConfiguration; }
            set { _snmpConfiguration = value; }
	    }

		#endregion

		#region Construction/Destruction

		static AlertingConfiguration()
		{
			_timeZoneLookup = new Hashtable() ;
			TimeZoneInfo[] results ;

			results = TimeZoneInfo.GetSystemTimeZones() ;
			foreach(TimeZoneInfo tzi in results)
				_timeZoneLookup[tzi.TimeZoneStruct.StandardName] = tzi ;
		}

		public AlertingConfiguration()
		{
         _sqlServerCategories = new Dictionary<int, CMEventCategory>();
			_sqlServerFields = new Hashtable() ;
			_sqlServerEvents = new Hashtable() ;
			_smtpSettings = new SmtpConfiguration() ;

			BuildMacros() ;
		}

		#endregion

		public void Initialize(string hostInstance)
		{
			_hostInstance = hostInstance ;
         _connectionString = String.Format( "server={0};" +
            "database={1};" +
            "integrated security=SSPI;" +
            "Connect Timeout=30;" + 
            "Application Name='{2}';",
            hostInstance,
            CoreConstants.RepositoryDatabase,
            CoreConstants.ManagementConsoleName );

         BuildEventFields() ;

         // Sql Server Event Types
         _sqlServerCategories = RulesDal.SelectEventCategories(_connectionString) ;
         foreach(CMEventCategory category in _sqlServerCategories.Values)
         {
            foreach(CMEventType evType in category.EventTypes)
            {
               _sqlServerEvents.Add(evType.TypeId, evType) ;
            }
         }
         _smtpSettings = AlertingDal.SelectSmtpConfiguration(_connectionString) ;
		    _snmpConfiguration = AlertingDal.SelectSnmpConfiguration(_connectionString);
		}

      private void BuildEventFields()
      {
         EventField field ;
         
         field = new EventField() ;
         field.Id = (int)AlertableEventFields.eventType ;
         field.DisplayName = "Type" ;
         field.ColumnName = "eventType" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.Integer ;
         _sqlServerFields[field.Id] = field ;

         field = new EventField() ;
         field.Id = (int)AlertableEventFields.eventCategory ;
         field.DisplayName = "Category" ;
         field.ColumnName = "eventCategory" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.Integer ;
         _sqlServerFields[field.Id] = field ;

         field = new EventField() ;
         field.Id = (int)AlertableEventFields.applicationName ;
         field.DisplayName = "Application Name" ;
         field.ColumnName = "applicationName" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.String ;
         _sqlServerFields[field.Id] = field ;

         field = new EventField();
         field.Id = (int)AlertableEventFields.privilegedUsers;
         field.DisplayName = "Privileged Users";
         field.ColumnName = "privilegedUsers";
         field.RuleType = EventType.SqlServer;
         field.DataFormat = MatchType.String;
         _sqlServerFields[field.Id] = field;

         field = new EventField() ;
         field.Id = (int)AlertableEventFields.loginName ;
         field.DisplayName = "Login Name" ;
         field.ColumnName = "loginName" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.String ;
         _sqlServerFields[field.Id] = field ;

         field = new EventField() ;
         field.Id = (int)AlertableEventFields.success ;
         field.DisplayName = "Access Check Passed" ;
         field.ColumnName = "success" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.Bool ;
         _sqlServerFields[field.Id] = field ;

         field = new EventField() ;
         field.Id = (int)AlertableEventFields.databaseName ;
         field.DisplayName = "Database" ;
         field.ColumnName = "databaseName" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.String ;
         _sqlServerFields[field.Id] = field ;

         field = new EventField() ;
         field.Id = (int)AlertableEventFields.objectName ;
         field.DisplayName = "Object Name" ;
         field.ColumnName = "objectName" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.String ;
         _sqlServerFields[field.Id] = field ;

         field = new EventField();
         field.Id = (int)AlertableEventFields.hostName;
         field.DisplayName = "Hostname";
         field.ColumnName = "hostName";
         field.RuleType = EventType.SqlServer;
         field.DataFormat = MatchType.String;
         _sqlServerFields[field.Id] = field;

         field = new EventField();
         field.Id = (int)AlertableEventFields.objectType ;
         field.DisplayName = "Object Type" ;
         field.ColumnName = "objectType" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.Integer ;
         _sqlServerFields[field.Id] = field ;

         field = new EventField() ;
         field.Id = (int)AlertableEventFields.privilegedUser ;
         field.DisplayName = "Privileged User" ;
         field.ColumnName = "privilegedUser" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.Bool ;
         _sqlServerFields[field.Id] = field ;

         field = new EventField() ;
         field.Id = (int)AlertableEventFields.serverName ;
         field.DisplayName = "SQL Server" ;
         field.ColumnName = "serverName" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.String ;
	  	_sqlServerFields[field.Id] = field ;
		
		 field = new EventField();
         field.Id = (int)AlertableEventFields.rowCount;
         field.DisplayName = "Row Count";
         field.ColumnName = "rowcount";
         field.RuleType = EventType.SqlServer;
         field.DataFormat = MatchType.String;
         _sqlServerFields[field.Id] = field;

         field = new EventField();
         field.Id = (int)AlertableEventFields.dataRuleApplicationName;
         field.DisplayName = "Application Name";
         field.ColumnName = "applicationName";
         field.RuleType = EventType.SqlServer;
         field.DataFormat = MatchType.String;
         _sqlServerFields[field.Id] = field;

         field = new EventField();
         field.Id = (int)AlertableEventFields.dataRuleLoginName;
         field.DisplayName = "Login Name";
         field.ColumnName = "loginName";
         field.RuleType = EventType.SqlServer;
         field.DataFormat = MatchType.String;
         _sqlServerFields[field.Id] = field;

         AlertingDal.SqlServerAlertableFields = _sqlServerFields ;
      }

		private void BuildMacros()
		{
            MacroDefinition def;

            _sqlServerMacros = new MacroDefinition[18];
            _statusMacros = new MacroDefinition[8];
            _dataMacros = new MacroDefinition[14];
            int alertIndex = 0;
            int statusAlertIndex = 0;
            int dataAlertIndex = 0;

			// General Alert Fields

			def = new MacroDefinition() ;
			def.Name = "Event Id" ;
			def.Description = "Event Id" ;
			def.Value = "$EventId$" ;
         _sqlServerMacros[alertIndex++] = def;
         _dataMacros[dataAlertIndex++] = def;

			def = new MacroDefinition() ;
			def.Name = "Alert Type" ;
			def.Description = "Alert Type" ;
			def.Value = "$AlertType$" ;
			_sqlServerMacros[alertIndex++] = def ;
         _statusMacros[statusAlertIndex++] = def;
         _dataMacros[dataAlertIndex++] = def;

			def = new MacroDefinition() ;
			def.Name = "Alert Time" ;
			def.Description = "Alert Time" ;
			def.Value = "$AlertTime$" ;
			_sqlServerMacros[alertIndex++] = def ;
         _statusMacros[statusAlertIndex++] = def;
         _dataMacros[dataAlertIndex++] = def;

			def = new MacroDefinition() ;
			def.Name = "Alert Level" ;
			def.Description = "Alert Level" ;
			def.Value = "$AlertLevel$" ;
			_sqlServerMacros[alertIndex++] = def ;
         _statusMacros[statusAlertIndex++] = def;
         _dataMacros[dataAlertIndex++] = def;

         def = new MacroDefinition();
         def.Name = "Threshold Value";
         def.Description = "Threshold Value";
         def.Value = "$ThresholdValue$";
         _statusMacros[statusAlertIndex++] = def;

         def = new MacroDefinition();
         def.Name = "Actual Value";
         def.Description = "Actual Value";
         def.Value = "$ActualValue$";
         _statusMacros[statusAlertIndex++] = def;

         def = new MacroDefinition();
         def.Name = "Alert Type Name";
         def.Description = "Alert Type Name";
         def.Value = "$AlertTypeName$";
         _statusMacros[statusAlertIndex++] = def;

         def = new MacroDefinition();
         def.Name = "Computer Name";
         def.Description = "Computer Name";
         def.Value = "$ComputerName$";
         _statusMacros[statusAlertIndex++] = def;
         
         // Sql Server Events
			def = new MacroDefinition() ;
			def.Name = "Event Time" ;
			def.Description = "Event Time" ;
			def.Value = "$EventTime$" ;
			_sqlServerMacros[alertIndex++] = def ;
         _dataMacros[dataAlertIndex++] = def;

			def = new MacroDefinition() ;
			def.Name = "Event Type" ;
			def.Description = "Event Type" ;
			def.Value = "$EventType$" ;
			_sqlServerMacros[alertIndex++] = def ;
         _dataMacros[dataAlertIndex++] = def;

			def = new MacroDefinition() ;
			def.Name = "Application Name" ;
			def.Description = "Application Name" ;
			def.Value = "$ApplicationName$" ;
			_sqlServerMacros[alertIndex++] = def ;

			def = new MacroDefinition() ;
			def.Name = "Host Name" ;
			def.Description = "Host Name" ;
			def.Value = "$HostName$" ;
			_sqlServerMacros[alertIndex++] = def ;
            _dataMacros[dataAlertIndex++] = def;

			def = new MacroDefinition() ;
			def.Name = "Server Name" ;
			def.Description = "Server Name" ;
			def.Value = "$ServerName$" ;
			_sqlServerMacros[alertIndex++] = def ;
         _statusMacros[statusAlertIndex++] = def;
         _dataMacros[dataAlertIndex++] = def;

			def = new MacroDefinition() ;
			def.Name = "Login" ;
			def.Description = "Login" ;
			def.Value = "$Login$" ;
			_sqlServerMacros[alertIndex++] = def ;
            _dataMacros[dataAlertIndex++] = def;

			def = new MacroDefinition() ;
			def.Name = "Success" ;
			def.Description = "Success" ;
			def.Value = "$Success$" ;
			_sqlServerMacros[alertIndex++] = def ;

			def = new MacroDefinition() ;
			def.Name = "Database Name" ;
			def.Description = "Database Name" ;
			def.Value = "$DatabaseName$" ;
			_sqlServerMacros[alertIndex++] = def ;
         _dataMacros[dataAlertIndex++] = def;
         
			def = new MacroDefinition() ;
			def.Name = "Object Name" ;
			def.Description = "Object Name" ;
			def.Value = "$ObjectName$" ;
			_sqlServerMacros[alertIndex++] = def ;

         def = new MacroDefinition();
         def.Name = "Table Name";
         def.Description = "Table Name";
         def.Value = "$TableName$";
         _dataMacros[dataAlertIndex++] = def;

			def = new MacroDefinition() ;
			def.Name = "Privileged User" ;
			def.Description = "Privileged User" ;
			def.Value = "$PrivilegedUser$" ;
			_sqlServerMacros[alertIndex++] = def ;

         def = new MacroDefinition() ;
         def.Name = "Target Login" ;
         def.Description = "Target Login" ;
         def.Value = "$TargetLogin$" ;
         _sqlServerMacros[alertIndex++] = def ;

         def = new MacroDefinition() ;
         def.Name = "Target User" ;
         def.Description = "Target User" ;
         def.Value = "$TargetUser$" ;
         _sqlServerMacros[alertIndex++] = def ;

         def = new MacroDefinition() ;
         def.Name = "Target Object" ;
         def.Description = "Target Object" ;
         def.Value = "$TargetObject$" ;
         _sqlServerMacros[alertIndex++] = def ;

         def = new MacroDefinition();
         def.Name = "Column Name";
         def.Description = "Column Name";
         def.Value = "$ColumnName$";
         _dataMacros[dataAlertIndex++] = def;

         def = new MacroDefinition();
         def.Name = "SQL Text";
         def.Description = "SQL text";
         def.Value = "$SQLText$";
         _sqlServerMacros[alertIndex++] = def;

         def = new MacroDefinition();
         def.Name = "After Data Value";
         def.Description = "After Data Value";
         def.Value = "$AfterDataValue$";
         _dataMacros[dataAlertIndex++] = def;

         def = new MacroDefinition();
         def.Name = "Before Data Value";
         def.Description = "Before Data Value";
         def.Value = "$BeforeDataValue$";
         _dataMacros[dataAlertIndex++] = def;
         
 		}

		public static TimeZoneHelper.TimeZoneInfo GetTimeZone(string standardName)
		{
			return _timeZoneLookup[standardName] as TimeZoneHelper.TimeZoneInfo ;
		}

		public CMEventType LookupEventType(int id, EventType alertType)
		{
			switch(alertType)
			{
				case EventType.SqlServer:
               return (CMEventType)_sqlServerEvents[id] ;
				default:
					return null; 
			}
		}

      public EventField LookupAlertableEventField(AlertableEventFields fieldId)
      {
         try
         {
            return (EventField)_sqlServerFields[(int)fieldId] ;
         }
         catch(Exception)
         {
            return null ;
         }
      }

      public CMEventCategory LookupCategory(int categoryId)
      {
         return _sqlServerCategories[categoryId] ;
      }
	}
}
