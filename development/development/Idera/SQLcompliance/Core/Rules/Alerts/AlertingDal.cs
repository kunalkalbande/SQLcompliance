using System;
using System.Collections ;
using System.Collections.Generic;
using System.Data ;
using System.Data.SqlClient ;
using System.Data.SqlTypes ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Event ;
using SnmpSharpNet;

namespace Idera.SQLcompliance.Core.Rules.Alerts
{
	/// <summary>
	/// Summary description for AlertingDal.
	/// </summary>
	public class AlertingDal
	{
		#region Member Variables

      private const string SELECT_AlertRulesForServer = "SELECT ruleId,name,description,targetInstances,enabled FROM {0}..{1} where targetInstances='<ALL>' or targetInstances='{2}' or targetInstances LIKE '{2},%' or targetInstances LIKE '%,{2}' or targetInstances LIKE '%,{2},%' ";

        private const string SELECT_AlertRule = "SELECT ruleId,name,description,alertLevel,alertType,targetInstances,enabled,message,logMessage,emailMessage,snmpTrap,snmpServerAddress,snmpPort,snmpCommunity,isAlertRuleTimeFrameEnabled,alertRuleTimeFrameStartTime,alertRuleTimeFrameEndTime,alertRuleTimeFrameDaysOfWeek,emailSummaryMessage,summaryEmailFrequency FROM {0}..{1} WHERE alertType = 1";
        private const string SELECT_StatusAlertRule = "SELECT r.ruleId,r.name,r.description,r.alertLevel,r.alertType,r.targetInstances,r.enabled,r.message,r.logMessage,r.emailMessage,t.RuleName,c.fieldId as 'statusRuleType',c.conditionId,c.matchString as 'thresholdValue', r.snmpTrap, r.snmpServerAddress, r.snmpPort, r.snmpCommunity FROM {0}..{1} r JOIN {0}..{2} c on r.ruleId = c.ruleId JOIN {0}..{3} t on c.fieldId = t.StatusRuleId WHERE r.alertType = 2";
        private const string SELECT_DataAlertRule = "SELECT r.ruleId,r.name,r.description,r.alertLevel,r.alertType,r.targetInstances,r.enabled,r.message,r.logMessage,r.emailMessage,t.RuleName,c.fieldId as 'dataRuleType',c.conditionId,c.matchString as 'comparison', r.snmpTrap, r.snmpServerAddress, r.snmpPort, r.snmpCommunity FROM {0}..{1} r JOIN {0}..{2} c on r.ruleId = c.ruleId JOIN {0}..{3} t on c.fieldId = t.DataRuleId WHERE r.alertType = 3";
      private const string SELECT_StatusRuleNames = "SELECT RuleName from {0}..{1}";
      private const string SELECT_DataRuleNames = "SELECT RuleName from {0}..{1} ";//order by RuleName";
		private const string SELECT_AlertRuleId = "SELECT ruleId,name,description,alertLevel,alertType,targetInstances,enabled,message,logMessage,emailMessage, snmpTrap FROM {0}..{1} WHERE ruleId={2}" ;
        private const string INSERT_AlertRule = "INSERT INTO {0}..{1} (name,description,alertLevel,alertType,targetInstances,enabled,message,logMessage,emailMessage,snmpTrap,snmpServerAddress,snmpPort,snmpCommunity) VALUES({2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}) SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY]";
        private const string INSERT_EventAlertRule = "INSERT INTO {0}..{1} (name,description,alertLevel,alertType,targetInstances,enabled,message,logMessage,emailMessage,snmpTrap,snmpServerAddress,snmpPort,snmpCommunity,isAlertRuleTimeFrameEnabled,alertRuleTimeFrameStartTime,alertRuleTimeFrameEndTime,alertRuleTimeFrameDaysOfWeek,emailSummaryMessage,summaryEmailFrequency, lastEmailSummarySendTime) VALUES({2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14},{15},{16},{17},{18},{19},{20},{21}) SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY]";
        private const string DELETE_AlertRule = "DELETE FROM {0}..{1} WHERE ruleId={2}" ;
        private const string UPDATE_AlertRule = "UPDATE {0}..{1} SET name={2},description={3},alertLevel={4},alertType={5},targetInstances={6},enabled={7},message={8},logMessage={9},emailMessage={10}, snmpTrap = {12}, snmpServerAddress = {13}, snmpPort = {14}, snmpCommunity = {15} WHERE ruleId={11}";
        private const string UPDATE_EventAlertRule = "UPDATE {0}..{1} SET name={2},description={3},alertLevel={4},alertType={5},targetInstances={6},enabled={7},message={8},logMessage={9},emailMessage={10}, snmpTrap = {12}, snmpServerAddress = {13}, snmpPort = {14}, snmpCommunity = {15}, isAlertRuleTimeFrameEnabled = {16}, alertRuleTimeFrameStartTime = {17}, alertRuleTimeFrameEndTime = {18}, alertRuleTimeFrameDaysOfWeek = {19}, emailSummaryMessage = {20}, summaryEmailFrequency = {21} WHERE ruleId={11}";
        private const string UPDATE_OldEventAlertRule = "UPDATE {0}..{1} SET name={2},description={3},alertLevel={4},alertType={5},targetInstances={6},enabled={7},message={8},logMessage={9},emailMessage={10}, snmpTrap = {12}, snmpServerAddress = {13}, snmpPort = {14}, snmpCommunity = {15}, isAlertRuleTimeFrameEnabled = {16}, alertRuleTimeFrameStartTime = {17}, alertRuleTimeFrameEndTime = {18}, alertRuleTimeFrameDaysOfWeek = {19}, emailSummaryMessage = {20}, summaryEmailFrequency = {21}, lastEmailSummarySendTime = {22} WHERE ruleId={11}";
        private const string UPDATE_AlertRuleEnable = "UPDATE {0}..{1} SET enabled={2} WHERE ruleId={3}";

		private const string SELECT_AlertCondition = "SELECT conditionId, fieldId, matchString FROM {0}..{1} WHERE ruleId={2}" ;
      private const string SELECT_AlertThreshold = "SELECT conditionId, matchString, fieldId FROM {0}..{1} WHERE ruleId={2}";
		private const string INSERT_AlertCondition = "INSERT INTO {0}..{1} (ruleId, fieldId, matchString) VALUES ({2}, {3}, {4}) SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY]" ;
      private const string INSERT_StatusAlertThreshold = "INSERT INTO {0}..{1} (ruleId, fieldId, matchString) VALUES ({2}, {3}, {4}) SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY]";
		private const string DELETE_AlertCondition = "DELETE FROM {0}..{1} WHERE conditionId={2}" ;
		private const string DELETE_AlertConditions = "DELETE FROM {0}..{1} WHERE ruleId={2}" ;
		private const string UPDATE_AlertCondition = "UPDATE {0}..{1} SET matchString={2} WHERE conditionId={3}" ;
      private const string UPDATE_StatusAlertThreshold = "UPDATE {0}..{1} SET fieldId={2}, matchString={3} where conditionId={4}";

		private const string SELECT_Alert = "SELECT alertId,alertType,alertRuleId,alertEventId,instance,eventType,created,alertLevel,emailStatus,logStatus,message,ruleName,snmpTrapStatus FROM {0}..{1}" ;
      private const string SELECT_RecentAlerts = "SELECT TOP {0} alertId,alertType,alertRuleId,alertEventId,instance,eventType,created,alertLevel,emailStatus,logStatus,message,ruleName, snmpTrapStatus FROM {1}..{2} ORDER BY created DESC" ;
      private const string SELECT_AlertId = "SELECT alertId,alertType,alertRuleId,alertEventId,instance,eventType,created,alertLevel,emailStatus,logStatus,message,ruleName, snmpTrapStatus FROM {0}..{1} WHERE alertId={2}" ;
		private const string INSERT_Alert = "INSERT INTO {0}..{1} (alertType,alertRuleId,alertEventId,instance,eventType,alertLevel,emailStatus,logStatus,message,ruleName, snmpTrapStatus) VALUES({2},{3},{4},{5},{6},{7},{8},{9},{10},{11}, {12}) SELECT alertId,created FROM {0}..{1} WHERE alertId=SCOPE_IDENTITY()" ;
      private const string INSERT_StatusAlert = "INSERT INTO {0}..{1} (alertType,alertRuleId,alertEventId,instance,computerName,eventType,alertLevel,emailStatus,logStatus,message,ruleName, snmpTrapStatus) VALUES({2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}, {13}) SELECT alertId,created FROM {0}..{1} WHERE alertId=SCOPE_IDENTITY()";
      private const string INSERT_DataAlert = "INSERT INTO {0}..{1} (alertType,alertRuleId,alertEventId,instance,eventType,alertLevel,emailStatus,logStatus,message,ruleName, snmpTrapStatus, alertEventType) VALUES({2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}, {13}) SELECT alertId,created FROM {0}..{1} WHERE alertId=SCOPE_IDENTITY()";
      private const string DELETE_Alert = "DELETE FROM {0}..{1} WHERE alertId={2}";
      private const string DELETE_AlertsTime = "DELETE {0}..{1} FROM (SELECT TOP 1000 alertId FROM {0}..{1} WHERE created<{2}) AS t WHERE {0}..{1}.alertId=t.alertId" ;
      private const string DELETE_AlertsTimeInstance = "DELETE {0}..{1} FROM (SELECT TOP 1000 alertId FROM {0}..{1} WHERE instance={2} AND created<{3}) AS t WHERE {0}..{1}.alertId=t.alertId" ;
      private const string UPDATE_Alert = "UPDATE {0}..{1} SET emailStatus={2},logStatus={3}, snmpTrapStatus = {5} WHERE alertId={4}" ;

		private const string SELECT_SmtpConfig = "SELECT smtpServer,smtpPort,smtpAuthType,smtpSsl,smtpUsername,smtpPassword,smtpSenderAddress,smtpSenderName FROM {0}..{1}" ;
		private const string UPDATE_SmtpConfig = "UPDATE {0}..{1} SET smtpServer={2},smtpPort={3},smtpAuthType={4},smtpSsl={5},smtpUsername={6},smtpPassword={7},smtpSenderAddress={8},smtpSenderName={9}" ;

        private const string SELECT_SnmpConfig = "SELECT snmpServerAddress, snmpPort, snmpCommunity FROM {0}..{1}";
        private const string UPDATE_SnmpConfig = "UPDATE {0}..{1} SET snmpServerAddress = {2}, snmpPort = {3}, snmpCommunity = {4}";

		private const string SELECT_AlertCount = "SELECT COUNT(alertId) FROM {0}..{1}" ;
      private const string SELECT_AlertLevelCount = "SELECT COUNT(alertId),alertLevel FROM {0}..{1} {2} GROUP BY alertLevel";

      private const string SELECT_AlertableServers = "SELECT instance, eventDatabase, highWatermark, alertHighWatermark FROM {0}..{1} WHERE isAuditedServer=1 AND (highWatermark > alertHighWatermark OR (highWatermark < lowWatermark AND alertHighWatermark > lowWatermark)) ";
      private const string Update_AlertsWatermark = "UPDATE {0}..{1} SET alertHighWatermark={2} WHERE instance={3}" ;

      private const string SELECT_AuditEvents = "SELECT TOP {0} startTime,checksum,eventId,eventType,eventClass,eventSubclass,spid,applicationName,hostName,serverName,loginName,success,databaseName,databaseId,dbUserName,objectType,objectName,objectId,permissions,columnPermissions,targetLoginName,targetUserName,roleName,ownerName,targetObject,details,eventCategory,hash,alertLevel,privilegedUser,rowCounts FROM {1}..{2} WHERE ({3} > {4} AND (eventId > {3} OR eventId <= {4})) OR ({3} < {4} AND eventId > {3} AND eventId <= {4}) ORDER BY eventId ASC";
      private const string SELECT_SCAuditEvents = "SELECT TOP {0} e.eventId, e.startTime,e.serverName,e.databaseName,e.targetObject,sc.columnName,e.hostName,e.loginName, e.applicationName, e.rowCounts, e.eventType FROM {1}..{2} e join {1}..{3} sc on e.eventId = sc.eventId WHERE ({4} > {5} AND (e.eventId > {4} OR e.eventId <= {5})) OR ({4} < {5} AND e.eventId > {4} AND e.eventId <= {5}) ORDER BY eventId ASC";
      private const string SELECT_BADAuditEvents = "SELECT TOP {0} dc.eventId,dc.startTime,sd.srvInstance,sd.name,dc.tableName,cc.columnName,cc.beforeValue,cc.afterValue,e.hostName,dc.userName, e.applicationName, e.rowCounts , e.eventType FROM {2}..{3} dc join {2}..{4} cc on dc.dcId = cc.dcId join {5}..{6} sd on dc.databaseId = sd.sqlDatabaseId and '['+ sd.srvInstance+']' = '{1}' join {2}..{7} e on dc.eventId = e.eventId ORDER BY dc.eventId DESC";

		private static Hashtable _eventFields ;


		#endregion

		#region Properties

      public static Hashtable SqlServerAlertableFields
      {
         set { _eventFields = value ; }
         get { return _eventFields ; }
      }

		#endregion

		#region Construction/Destruction

      public static List<AlertRule> SelectAlertRulesForServer(SqlConnection connection, string serverName)
      {
         List<AlertRule> retVal = new List<AlertRule>();
         int column = 0;

         ValidateConnection(connection);

         // Get the Rule objects
         string cmdStr = String.Format(SELECT_AlertRulesForServer, CoreConstants.RepositoryDatabase,
            CoreConstants.RepositoryAlertRulesTable, serverName);
         using (SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using (SqlDataReader reader = command.ExecuteReader())
            {
               while (reader.Read())
               {
                  AlertRule rule = new AlertRule();
                  column = 0;

                  rule.Id = reader.GetInt32(column++);
                  rule.Name = reader.GetString(column++);
                  rule.Description = reader.GetString(column++);
                  rule.TargetInstanceList = reader.GetString(column++);
                  rule.Enabled = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));

                  // We only return filters that are enabled
                  if (rule.Enabled)
                     retVal.Add(rule);
               }
            }
         }

         foreach (AlertRule rule in retVal)
         {
            // Get the associated conditions
            ArrayList conditions = SelectAlertConditions(rule, connection);
            foreach (EventCondition condition in conditions)
               rule.AddCondition(condition);
         }
         return retVal;
      }

      /*
		public static void LoadEventFields(string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				LoadEventFields(connection) ;
			}
		}

		public static void LoadEventFields(SqlConnection connection)
		{
			if(connection == null)
				throw new ArgumentNullException("connection") ;

			_eventFields = new EventFieldMap() ;

			ValidateConnection(connection) ;
			EventFieldCollection fields = SelectEventFields(connection) ;
			foreach(EventField field in fields)
				_eventFields[field.Id] = field ;
		}*/

		#endregion

		#region General

		/// <summary>
		/// Verifies that the connection object is not null and that the connection is
		/// open.
		/// </summary>
		/// <param name="connection"></param>
		/// <exception cref="ArgumentNullException">Thrown when connection is null</exception>
		/// <exception cref="Exception">Thrown when the connection is not open.</exception>
		private static void ValidateConnection(SqlConnection connection)
		{
			if(connection == null)
				throw new ArgumentNullException("connection") ;
			if(connection.State != ConnectionState.Open)
				throw new Exception("SqlConnection object is not opened.") ;
		}

		#endregion // General

      #region Instance Servers

      public static AlertingJobInfo[] SelectAlertableServers(string connectionString)
      {
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;
         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return SelectAlertableServers(connection) ;
         }
      }

      public static AlertingJobInfo[] SelectAlertableServers(SqlConnection connection)
      {
         ArrayList list = new ArrayList();
         string cmdStr = String.Format(SELECT_AlertableServers, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryServerTable) ;
         ValidateConnection(connection) ;
         using(SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using(SqlDataReader reader = command.ExecuteReader())
            {
               while(reader.Read())
               {
                  AlertingJobInfo info = new AlertingJobInfo();
                  
                  info.TargetInstance = reader.GetString(0) ;
                  info.ServerDbName = reader.GetString(1) ;
                  info.HighWatermark = reader.GetInt32(2) ;
                  info.AlertHighWatermark = reader.GetInt32(3) ;

                  list.Add(info) ;
               }
               AlertingJobInfo[] retVal = new AlertingJobInfo[list.Count] ;
               for(int i = 0 ; i < list.Count ; i ++)
                  retVal[i] = (AlertingJobInfo)list[i] ;
               return retVal ;
            }
         }
      }

      public static int UpdateAlertHighWatermark(int watermark, string targetInstance, string connectionString)
      {
         if(targetInstance == null)
            throw new ArgumentNullException("targetInstance") ;
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return UpdateAlertHighWatermark(watermark, targetInstance, connection) ;
         }
      }

      public static int UpdateAlertHighWatermark(int watermark, string targetInstance, SqlConnection connection)
      {
         if(targetInstance == null)
            throw new ArgumentNullException("targetInstance") ;
         ValidateConnection(connection) ;

         using(SqlCommand command = new SqlCommand())
         {
            command.Connection = connection ;
            command.CommandText = String.Format(Update_AlertsWatermark, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryServerTable, watermark, SQLHelpers.CreateSafeString(targetInstance)) ;
            return command.ExecuteNonQuery() ;
         }
      }

		#endregion

		#region EventFields

      /*
		public static EventFieldCollection SelectEventFields(string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return SelectEventFields(connection) ;
			}
		}

		public static EventFieldCollection SelectEventFields(SqlConnection connection)
		{
			EventFieldCollection retVal = new EventFieldCollection() ;
			int column = 0 ;

			ValidateConnection(connection) ;
         string cmdStr = String.Format(SELECT_EventField, CoreConstants.RepositoryDatabase, 
            CoreConstants.RepositoryEventFieldMapTable) ;
			using(SqlCommand command = new SqlCommand(cmdStr, connection))
			{
				using(SqlDataReader reader = command.ExecuteReader())
				{
					while(reader.Read())
					{
						EventField field = new EventField() ;
						column = 0 ;

						field.Id = reader.GetInt32(column++) ;
						field.DataFormat = (MatchType)reader.GetInt32(column++) ;
						field.RuleType = (AlertType)reader.GetInt32(column++) ;
						field.ColumnName = reader.GetString(column++) ;
						field.DisplayName = reader.GetString(column++) ;
						field.Description = reader.GetString(column++) ;
						retVal.Add(field) ;
					}
				}
			}
			return retVal ;
		}*/

		#endregion // EventFields

		#region Alert Rules

		/// <summary>
		/// Retrieves all alert rules stored in the database.
		/// </summary>
		/// <param name="connectionString">valid connection string</param>
		/// <returns>A collection of alert rules</returns>
		/// <exception cref="ArgumentNullException">When connection string is null</exception>
		/// <exception cref="ArgumentException">When the supplied connection string is invalid</exception>
      public static List<AlertRule> SelectAlertRules(string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return SelectAlertRules(connection) ;
			}
		}

      public static List<AlertRule> SelectAlertRules(string connectionString, string request)
      {
          if (connectionString == null)
              throw new ArgumentNullException("connectionString");

          using (SqlConnection connection = new SqlConnection(connectionString))
          {
              connection.Open();
              return SelectAlertRules(connection,request);
          }
      }

		/// <summary>
		/// Retrieves all alert rules stored in the database.
		/// </summary>
		/// <param name="connection">a valid, open connection</param>
		/// <returns>A collection of alert rules</returns>
		/// <exception cref="ArgumentNullException">Thrown when connection is null</exception>
		/// <exception cref="Exception">Thrown when the connection is not open.</exception>
      public static List<AlertRule> SelectAlertRules(SqlConnection connection)
		{
         List<AlertRule> retVal = new List<AlertRule>();
			int column ;

			ValidateConnection(connection) ;

			// Get the Rule objects
         string cmdStr = String.Format(SELECT_AlertRule, CoreConstants.RepositoryDatabase,
            CoreConstants.RepositoryAlertRulesTable) ;
			using(SqlCommand command = new SqlCommand(cmdStr, connection))
			{
				using(SqlDataReader reader = command.ExecuteReader())
				{
					while(reader.Read())
					{
						AlertRule rule = new AlertRule() ;
						column = 0 ;

						rule.Id = reader.GetInt32(column++) ;
						rule.Name = reader.GetString(column++) ;
						rule.Description = reader.GetString(column++) ;
						rule.Level = (AlertLevel)reader.GetByte(column++) ;
						rule.RuleType = (EventType)reader.GetInt32(column++) ;
						rule.TargetInstanceList = reader.GetString(column++) ;
						rule.Enabled = !SqlByte.Zero.Equals(reader.GetSqlByte(column++)) ;
                  rule.MessageData = reader.GetString(column++) ;
                  rule.HasLogAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++)) ;
                  rule.HasEmailAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++)) ;
                  rule.HasSnmpTrapAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));

                        // read SNMP configuration
                        rule.SnmpConfiguration = new SNMPConfiguration();
					    rule.SnmpConfiguration.ReceiverAddress = SQLHelpers.GetString(reader, column++);
					    rule.SnmpConfiguration.ReceiverPort = SQLHelpers.GetInt32(reader, column++);
					    rule.SnmpConfiguration.Community = SQLHelpers.GetString(reader, column++);

                        if(!reader.IsDBNull(column))
                        {
                            rule.IsAlertRuleTimeFrameEnabled = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
                        }
                        else
                        {
                            column++;
                            rule.IsAlertRuleTimeFrameEnabled = false;
                        }
                        rule.AlertRuleTimeFrameStartTime = reader["alertRuleTimeFrameStartTime"].ToString();
                        column++;
                        rule.AlertRuleTimeFrameEndTime = reader["alertRuleTimeFrameEndTime"].ToString();
                        column++;
                        rule.AlertRuleTimeFrameDaysOfWeek = SQLHelpers.GetString(reader, column++);
                        if (!reader.IsDBNull(column))
                        {
                            rule.HasEmailSummaryAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
                        }
                        else
                        {
                            column++;
                            rule.HasEmailSummaryAction = false;
                        }
                        rule.SummaryEmailFrequency = SQLHelpers.GetInt32(reader, column);

                        retVal.Add(rule) ;
					}
				}
			}

			foreach(AlertRule rule in retVal)
			{
				// Get the associated conditions
				ArrayList conditions = SelectAlertConditions(rule, connection) ;
				foreach(EventCondition condition in conditions)
					rule.AddCondition(condition) ;
			}
			return retVal ;
		}

      public static List<AlertRule> SelectAlertRules(SqlConnection connection, string request)
      {
          List<AlertRule> retVal = new List<AlertRule>();
          int column;

          ValidateConnection(connection);

          // Get the Rule objects
          string cmdStr = String.Format(SELECT_AlertRule + request, CoreConstants.RepositoryDatabase,
             CoreConstants.RepositoryAlertRulesTable);
          using (SqlCommand command = new SqlCommand(cmdStr, connection))
          {
              using (SqlDataReader reader = command.ExecuteReader())
              {
                  while (reader.Read())
                  {
                      AlertRule rule = new AlertRule();
                      column = 0;

                      rule.Id = reader.GetInt32(column++);
                      rule.Name = reader.GetString(column++);
                      rule.Description = reader.GetString(column++);
                      rule.Level = (AlertLevel)reader.GetByte(column++);
                      rule.RuleType = (EventType)reader.GetInt32(column++);
                      rule.TargetInstanceList = reader.GetString(column++);
                      rule.Enabled = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
                      rule.MessageData = reader.GetString(column++);
                      rule.HasLogAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
                      rule.HasEmailAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
                      rule.HasSnmpTrapAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));

                      // read SNMP configuration
                      rule.SnmpConfiguration = new SNMPConfiguration();
                      rule.SnmpConfiguration.ReceiverAddress = SQLHelpers.GetString(reader, column++);
                      rule.SnmpConfiguration.ReceiverPort = SQLHelpers.GetInt32(reader, column++);
                      rule.SnmpConfiguration.Community = SQLHelpers.GetString(reader, column);

                      retVal.Add(rule);
                  }
              }
          }

          foreach (AlertRule rule in retVal)
          {
              // Get the associated conditions
              ArrayList conditions = SelectAlertConditions(rule, connection);
              foreach (EventCondition condition in conditions)
                  rule.AddCondition(condition);
          }
          return retVal;
      }

      public static List<StatusAlertRule> SelectStatusAlertRules(string connectionString)
      {
         if (connectionString == null)
            throw new ArgumentNullException("connectionString");

         using (SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open();
            return SelectStatusAlertRules(connection);
         }
      }

      public static List<StatusAlertRule> SelectStatusAlertRules(string connectionString, string request)
      {
          if (connectionString == null)
              throw new ArgumentNullException("connectionString");

          using (SqlConnection connection = new SqlConnection(connectionString))
          {
              connection.Open();
              return SelectStatusAlertRules(connection, request);
          }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="connection"></param>
      /// <returns></returns>
      public static List<StatusAlertRule> SelectStatusAlertRules(SqlConnection connection)
		{
         List<StatusAlertRule> rules = new List<StatusAlertRule>();
			int column ;

			ValidateConnection(connection) ;
         string cmdStr = String.Format(SELECT_StatusAlertRule, CoreConstants.RepositoryDatabase, 
                                                               CoreConstants.RepositoryAlertRulesTable, 
                                                               CoreConstants.RepositoryAlertRuleConditionsTable, 
                                                               CoreConstants.StatusRuleTypesTable);

			using(SqlCommand command = new SqlCommand(cmdStr, connection))
			{
				using(SqlDataReader reader = command.ExecuteReader())
				{
					while(reader.Read())
					{
                  StatusAlertRule rule = new StatusAlertRule();
                  StatusAlertThreshold threshold = new StatusAlertThreshold();
						column = 0 ;

						rule.Id = reader.GetInt32(column++);
						rule.Name = reader.GetString(column++);
						rule.Description = reader.GetString(column++);
						rule.Level = (AlertLevel)reader.GetByte(column++);
						rule.RuleType = (EventType)reader.GetInt32(column++);
						rule.TargetInstanceList = reader.GetString(column++);
						rule.Enabled = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
                  rule.MessageData = reader.GetString(column++);
                  rule.HasLogAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
                  rule.HasEmailAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
                  rule.StatusRuleTypeName = reader.GetString(column++);
                  rule.StatusType = (StatusRuleType)reader.GetInt32(column++);
                  threshold.Id = reader.GetInt32(column++);
                  threshold.Value = Convert.ToInt32(reader.GetString(column++));
                  rule.HasSnmpTrapAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));

                        // read SNMP configurations
                        rule.SnmpConfiguration = new SNMPConfiguration();
					    rule.SnmpConfiguration.ReceiverAddress = SQLHelpers.GetString(reader, column++);
					    rule.SnmpConfiguration.ReceiverPort = SQLHelpers.GetInt32(reader, column++);
					    rule.SnmpConfiguration.Community = SQLHelpers.GetString(reader, column);

                        rule.Threshold = threshold;
                  
                  rules.Add(rule);
					}
				}
			}
			return rules;
		}

      static List<StatusAlertRule> SelectStatusAlertRules(SqlConnection connection, string request)
      {
          List<StatusAlertRule> rules = new List<StatusAlertRule>();
          int column;
          request = request.Replace("ruleId", "r.ruleId");
          ValidateConnection(connection);
          string cmdStr = String.Format(SELECT_StatusAlertRule + request, CoreConstants.RepositoryDatabase,
                                                                CoreConstants.RepositoryAlertRulesTable,
                                                                CoreConstants.RepositoryAlertRuleConditionsTable,
                                                                CoreConstants.StatusRuleTypesTable);

          using (SqlCommand command = new SqlCommand(cmdStr, connection))
          {
              using (SqlDataReader reader = command.ExecuteReader())
              {
                  while (reader.Read())
                  {
                      StatusAlertRule rule = new StatusAlertRule();
                      StatusAlertThreshold threshold = new StatusAlertThreshold();
                      column = 0;

                      rule.Id = reader.GetInt32(column++);
                      rule.Name = reader.GetString(column++);
                      rule.Description = reader.GetString(column++);
                      rule.Level = (AlertLevel)reader.GetByte(column++);
                      rule.RuleType = (EventType)reader.GetInt32(column++);
                      rule.TargetInstanceList = reader.GetString(column++);
                      rule.Enabled = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
                      rule.MessageData = reader.GetString(column++);
                      rule.HasLogAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
                      rule.HasEmailAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
                      rule.StatusRuleTypeName = reader.GetString(column++);
                      rule.StatusType = (StatusRuleType)reader.GetInt32(column++);
                      threshold.Id = reader.GetInt32(column++);
                      threshold.Value = Convert.ToInt32(reader.GetString(column++));
                      rule.HasSnmpTrapAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));

                      // read SNMP configurations
                      rule.SnmpConfiguration = new SNMPConfiguration();
                      rule.SnmpConfiguration.ReceiverAddress = SQLHelpers.GetString(reader, column++);
                      rule.SnmpConfiguration.ReceiverPort = SQLHelpers.GetInt32(reader, column++);
                      rule.SnmpConfiguration.Community = SQLHelpers.GetString(reader, column);

                      rule.Threshold = threshold;

                      rules.Add(rule);
                  }
              }
          }
          return rules;
      }

      public static List<DataAlertRule> SelectDataAlertRules(string connectionString)
      {
         if (connectionString == null)
            throw new ArgumentNullException("connectionString");

         using (SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open();
            return SelectDataAlertRules(connection);
         }
      }

      public static List<DataAlertRule> SelectDataAlertRules(string connectionString, string request)
      {
          if (connectionString == null)
              throw new ArgumentNullException("connectionString");

          using (SqlConnection connection = new SqlConnection(connectionString))
          {
              connection.Open();
              return SelectDataAlertRules(connection, request);
          }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="connection"></param>
      /// <returns></returns>
      public static List<DataAlertRule> SelectDataAlertRules(SqlConnection connection)
      {
         List<DataAlertRule> rules = new List<DataAlertRule>();
         int column;

         ValidateConnection(connection);
         string cmdStr = String.Format(SELECT_DataAlertRule,   CoreConstants.RepositoryDatabase,
                                                               CoreConstants.RepositoryAlertRulesTable,
                                                               CoreConstants.RepositoryAlertRuleConditionsTable,
                                                               CoreConstants.DataRuleTypesTable);

         using (SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using (SqlDataReader reader = command.ExecuteReader())
            {
               while (reader.Read())
               {
                  DataAlertRule rule = new DataAlertRule();
                  column = 0;

                  rule.Id = reader.GetInt32(column++);
                  rule.Name = reader.GetString(column++);
                  rule.Description = reader.GetString(column++);
                  rule.Level = (AlertLevel)reader.GetByte(column++);
                  rule.RuleType = (EventType)reader.GetInt32(column++);
                  rule.TargetInstanceList = reader.GetString(column++);
                  rule.Enabled = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
                  rule.MessageData = reader.GetString(column++);
                  rule.HasLogAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
                  rule.HasEmailAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
                  rule.DataRuleTypeName = reader.GetString(column++);
                  rule.DataType = (DataRuleType)reader.GetInt32(column++);
                  rule.Comparison.Id = reader.GetInt32(column++);
                  rule.InsertComparison(reader.GetString(column++));
                  rule.HasSnmpTrapAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));

                  // read SNMP configurations
                  rule.SnmpConfiguration = new SNMPConfiguration();
                  rule.SnmpConfiguration.ReceiverAddress = SQLHelpers.GetString(reader, column++);
                  rule.SnmpConfiguration.ReceiverPort = SQLHelpers.GetInt32(reader, column++);
                  rule.SnmpConfiguration.Community = SQLHelpers.GetString(reader, column);

                  rules.Add(rule);
               }
            }
         }
         foreach (DataAlertRule rule in rules)
         {
             // Get the associated conditions
             ArrayList conditions = SelectAlertConditions(rule, connection);
             foreach (EventCondition condition in conditions)
                 rule.AddCondition(condition);
         }
         return rules;
      }

      public static List<DataAlertRule> SelectDataAlertRules(SqlConnection connection, string request)
      {
          List<DataAlertRule> rules = new List<DataAlertRule>();
          int column;
          request = request.Replace("ruleId","r.ruleId");
          ValidateConnection(connection);
          string cmdStr = String.Format(SELECT_DataAlertRule + request, CoreConstants.RepositoryDatabase,
                                                                CoreConstants.RepositoryAlertRulesTable,
                                                                CoreConstants.RepositoryAlertRuleConditionsTable,
                                                                CoreConstants.DataRuleTypesTable);

          using (SqlCommand command = new SqlCommand(cmdStr, connection))
          {
              using (SqlDataReader reader = command.ExecuteReader())
              {
                  while (reader.Read())
                  {
                      DataAlertRule rule = new DataAlertRule();
                      column = 0;

                      rule.Id = reader.GetInt32(column++);
                      rule.Name = reader.GetString(column++);
                      rule.Description = reader.GetString(column++);
                      rule.Level = (AlertLevel)reader.GetByte(column++);
                      rule.RuleType = (EventType)reader.GetInt32(column++);
                      rule.TargetInstanceList = reader.GetString(column++);
                      rule.Enabled = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
                      rule.MessageData = reader.GetString(column++);
                      rule.HasLogAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
                      rule.HasEmailAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
                      rule.DataRuleTypeName = reader.GetString(column++);
                      rule.DataType = (DataRuleType)reader.GetInt32(column++);
                      rule.Comparison.Id = reader.GetInt32(column++);
                      rule.InsertComparison(reader.GetString(column++));
                      rule.HasSnmpTrapAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));

                      // read SNMP configurations
                      rule.SnmpConfiguration = new SNMPConfiguration();
                      rule.SnmpConfiguration.ReceiverAddress = SQLHelpers.GetString(reader, column++);
                      rule.SnmpConfiguration.ReceiverPort = SQLHelpers.GetInt32(reader, column++);
                      rule.SnmpConfiguration.Community = SQLHelpers.GetString(reader, column);

                      rules.Add(rule);
                  }
              }
          }
          foreach (DataAlertRule rule in rules)
          {
              // Get the associated conditions
              ArrayList conditions = SelectAlertConditions(rule, connection);
              foreach (EventCondition condition in conditions)
                  rule.AddCondition(condition);
          }
          return rules;
      }

      public static List<string> SelectDataRuleNames(string connectionString)
      {
         if (connectionString == null)
            throw new ArgumentNullException("connectionString");

         using (SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open();
            return SelectDataRuleNames(connection);
         }
      }

      public static List<string> SelectDataRuleNames(SqlConnection connection)
      {
         List<string> names = new List<string>();

         ValidateConnection(connection);
         string cmdStr = String.Format(SELECT_DataRuleNames, CoreConstants.RepositoryDatabase, CoreConstants.DataRuleTypesTable);

         using (SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using (SqlDataReader reader = command.ExecuteReader())
            {
               while (reader.Read())
               {
                  names.Add(reader.GetString(0));
               }
            }
         }
         return names;
      }

      public static List<string> SelectStatusRuleNames(string connectionString)
      {
         if (connectionString == null)
            throw new ArgumentNullException("connectionString");

         using (SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open();
            return SelectStatusRuleNames(connection);
         }
      }

      public static List<string> SelectStatusRuleNames(SqlConnection connection)
      {
         List<string> names = new List<string>();

         ValidateConnection(connection);
         string cmdStr = String.Format(SELECT_StatusRuleNames, CoreConstants.RepositoryDatabase, CoreConstants.StatusRuleTypesTable);

         using (SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using (SqlDataReader reader = command.ExecuteReader())
            {
               while (reader.Read())
               {
                  names.Add(reader.GetString(0));
               }
            }
         }
         return names;
		}
      
      /// <summary>
		/// Retrieves a single alert rule from the database, as specifed by the supplied
		/// alert rule ID.
		/// </summary>
		/// <param name="id">rule id to fetch</param>
		/// <param name="connectionString">valid connection string</param>
		/// <returns>alert rule with the supplied id, if it exists.  If it doesn't exist, null is returned</returns>
		public static AlertRule SelectAlertRuleById(int id, string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return SelectAlertRuleById(id, connection) ;
			}
		}

		/// <summary>
		/// Retrieves a single alert rule from the database, as specifed by the supplied
		/// alert rule ID.
		/// </summary>
		/// <param name="id">rule id to fetch</param>
		/// <param name="connection">valid, open connection</param>
		/// <returns>alert rule with the supplied id, if it exists.  If it doesn't exist, null is returned</returns>
		public static AlertRule SelectAlertRuleById(int id, SqlConnection connection)
		{
			AlertRule rule = null ;

			ValidateConnection(connection) ;
         string cmdStr = String.Format(SELECT_AlertRuleId, CoreConstants.RepositoryDatabase,
            CoreConstants.RepositoryAlertRulesTable, id) ;
			using(SqlCommand command = new SqlCommand(cmdStr, connection))
			{
				using(SqlDataReader reader = command.ExecuteReader())
				{
					if(reader.HasRows)
					{
						rule = new AlertRule() ;

						reader.Read() ;
						int column = 0 ;

						rule.Id = reader.GetInt32(column++) ;
						rule.Name = reader.GetString(column++) ;
						rule.Description = reader.GetString(column++) ;
						rule.Level = (AlertLevel)reader.GetByte(column++) ;
						rule.RuleType = (EventType)reader.GetInt32(column++) ;
						rule.TargetInstanceList = reader.GetString(column++) ;
						rule.Enabled = !SqlByte.Zero.Equals(reader.GetSqlByte(column++)) ;
                  rule.MessageData = reader.GetString(column++) ;
                  rule.HasLogAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++)) ;
                  rule.HasEmailAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++)) ;
                  rule.HasSnmpTrapAction = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
					}
				}
			}
			if(rule == null)
				return null ;

			// Conditions
			ArrayList conditions = SelectAlertConditions(rule, connection) ;
			foreach(EventCondition condition in conditions)
				rule.AddCondition(condition) ;
			return rule; 
		}

		public static bool InsertAlertRule(AlertRule rule, string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;
			if(rule == null)
				throw new ArgumentNullException("rule") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return InsertAlertRule(rule, connection) ;
			}
		}

      /// <summary>
      /// 
      /// </summary>
      /// <param name="rule"></param>
      /// <param name="connection"></param>
      /// <returns></returns>
		public static bool InsertAlertRule(AlertRule rule, SqlConnection connection)
		{
			if(rule == null)
				throw new ArgumentNullException("rule") ;
			ValidateConnection(connection) ;

			using(SqlTransaction trans = connection.BeginTransaction())
			{
				using(SqlCommand command = new SqlCommand())
				{
                    #region SNMP Settings

                    string snmpReceiverAddress = string.Empty;
                    string snmpCommunity = string.Empty;
                    int snmpReceiverPort = 0;
                    if (rule.SnmpConfiguration != null)
                    {
                        snmpReceiverAddress = rule.SnmpConfiguration.ReceiverAddress;
                        snmpCommunity = rule.SnmpConfiguration.Community;
                        snmpReceiverPort = rule.SnmpConfiguration.ReceiverPort;

                    }

                    #endregion

					command.Transaction = trans ;
					command.Connection = connection ;
					command.CommandText = String.Format(INSERT_EventAlertRule, CoreConstants.RepositoryDatabase,
                  CoreConstants.RepositoryAlertRulesTable, SQLHelpers.CreateSafeString(rule.Name, 60), 
                  SQLHelpers.CreateSafeString(rule.Description, 200), (int)rule.Level, (int)rule.RuleType, 
                  SQLHelpers.CreateSafeString(rule.TargetInstanceList), rule.Enabled ? 1 : 0,
                  SQLHelpers.CreateSafeString(rule.MessageData), rule.HasLogAction ? 1 : 0,
                  rule.HasEmailAction ? 1 : 0,
                  rule.HasSnmpTrapAction ? 1 : 0,
                  SQLHelpers.CreateSafeString(snmpReceiverAddress),
                  snmpReceiverPort,
                  SQLHelpers.CreateSafeString(snmpCommunity),
                  rule.IsAlertRuleTimeFrameEnabled ? 1 : 0,
                  SQLHelpers.CreateSafeString(rule.AlertRuleTimeFrameStartTime),
                  SQLHelpers.CreateSafeString(rule.AlertRuleTimeFrameEndTime),
                  SQLHelpers.CreateSafeString(rule.AlertRuleTimeFrameDaysOfWeek),
                  rule.HasEmailSummaryAction ? 1 : 0,
                  rule.SummaryEmailFrequency,
                  SQLHelpers.CreateSafeDateTimeString(DateTime.Now));

					object o = command.ExecuteScalar() ;
					if(!(o is Decimal))
					{
						trans.Rollback() ;
						return false ;
					}
					rule.Id = Decimal.ToInt32((Decimal)o) ;

					// Conditions
					foreach(EventCondition condition in rule.Conditions)
					{
						if(!InsertAlertCondition(condition, connection, trans))
						{
							trans.Rollback() ;
							return false ;
						}
					}
					trans.Commit() ;
					return true ;
				}
			}
		}

      /// <summary>
      /// 
      /// </summary>
      /// <param name="rule"></param>
      /// <param name="connectionString"></param>
      /// <returns></returns>
      public static bool InsertAlertRule(StatusAlertRule rule, string connectionString)
      {
         if (connectionString == null)
            throw new ArgumentNullException("connectionString");
         if (rule == null)
            throw new ArgumentNullException("rule");

         using (SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open();
            return InsertAlertRule(rule, connection);
         }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="rule"></param>
      /// <param name="connection"></param>
      /// <returns></returns>
      public static bool InsertAlertRule(StatusAlertRule rule, SqlConnection connection)
      {
         if (rule == null)
            throw new ArgumentNullException("rule");
         ValidateConnection(connection);

         using (SqlTransaction trans = connection.BeginTransaction())
         {
            using (SqlCommand command = new SqlCommand())
            {
                #region SNMP Settings

                string snmpReceiverAddress = string.Empty;
                string snmpCommunity = string.Empty;
                int snmpReceiverPort = 0;
                if (rule.SnmpConfiguration != null)
                {
                    snmpReceiverAddress = rule.SnmpConfiguration.ReceiverAddress;
                    snmpCommunity = rule.SnmpConfiguration.Community;
                    snmpReceiverPort = rule.SnmpConfiguration.ReceiverPort;

                }

                #endregion

                command.Transaction = trans;
               command.Connection = connection;
               command.CommandText = String.Format(INSERT_AlertRule, 
                                     CoreConstants.RepositoryDatabase,
                                     CoreConstants.RepositoryAlertRulesTable, 
                                     SQLHelpers.CreateSafeString(rule.Name, 60),
                                     SQLHelpers.CreateSafeString(rule.Description, 200), 
                                     (int)rule.Level, 
                                     (int)rule.RuleType,
                                     SQLHelpers.CreateSafeString(rule.TargetInstanceList), 
                                     rule.Enabled ? 1 : 0,
                                     SQLHelpers.CreateSafeString(rule.MessageData), 
                                     rule.HasLogAction ? 1 : 0,
                                     rule.HasEmailAction ? 1 : 0,
                                     rule.HasSnmpTrapAction ? 1 : 0,
                                     SQLHelpers.CreateSafeString(snmpReceiverAddress),
                                     snmpReceiverPort,
                                     SQLHelpers.CreateSafeString(snmpCommunity));
               object o = command.ExecuteScalar();

               if (!(o is Decimal))
               {
                  trans.Rollback();
                  return false;
               }
               rule.Id = Decimal.ToInt32((Decimal)o);

               // Threshold
               if (InsertStatusAlertThreshold(rule, connection, trans) == false)
               {
                  trans.Rollback();
                  return false;
               }
               trans.Commit();
               return true;
            }
         }
      }

      public static bool InsertAlertRule(DataAlertRule rule, string connectionString)
      {
         if (connectionString == null)
            throw new ArgumentNullException("connectionString");
         if (rule == null)
            throw new ArgumentNullException("rule");

         using (SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open();
            return InsertAlertRule(rule, connection);
         }
      }

      public static bool InsertAlertRule(DataAlertRule rule, SqlConnection connection)
      {
         if (rule == null)
            throw new ArgumentNullException("rule");
         ValidateConnection(connection);
         //int ruleDataTypeId = SelectDataRuleDataTypeId(rule.DataRuleTypeName, connection);
         using (SqlTransaction trans = connection.BeginTransaction())
         {
            using (SqlCommand command = new SqlCommand())
            {
                #region SNMP Settings

                string snmpReceiverAddress = string.Empty;
                string snmpCommunity = string.Empty;
                int snmpReceiverPort = 0;
                if (rule.SnmpConfiguration != null)
                {
                    snmpReceiverAddress = rule.SnmpConfiguration.ReceiverAddress;
                    snmpCommunity = rule.SnmpConfiguration.Community;
                    snmpReceiverPort = rule.SnmpConfiguration.ReceiverPort;

                }

                #endregion

               command.Transaction = trans;
               command.Connection = connection;
               command.CommandText = String.Format(INSERT_AlertRule, CoreConstants.RepositoryDatabase,
                  CoreConstants.RepositoryAlertRulesTable, SQLHelpers.CreateSafeString(rule.Name, 60),
                  SQLHelpers.CreateSafeString(rule.Description, 200), (int)rule.Level, (int)rule.RuleType,
                  SQLHelpers.CreateSafeString(rule.TargetInstanceList), rule.Enabled ? 1 : 0,
                  SQLHelpers.CreateSafeString(rule.MessageData), rule.HasLogAction ? 1 : 0,
                  rule.HasEmailAction ? 1 : 0,
                  rule.HasSnmpTrapAction ? 1 : 0,
                  SQLHelpers.CreateSafeString(snmpReceiverAddress),
                  snmpReceiverPort,
                  SQLHelpers.CreateSafeString(snmpCommunity));
               object o = command.ExecuteScalar();

               if (!(o is Decimal))
               {
                  trans.Rollback();
                  return false;
               }
               rule.Id = Decimal.ToInt32((Decimal)o);

               // Threshold
               if (InsertDataAlertComparison(rule, connection, trans) == false)
               {
                  trans.Rollback();
                  return false;
               }
               // Conditions
               foreach (EventCondition condition in rule.Conditions)
               {
                   // Skipping these conditions as these field Ids are reserved for Data alert rule
                   if (condition.FieldId == (int)AlertableEventFields.applicationName
                       || condition.FieldId == (int)AlertableEventFields.eventCategory
                       || condition.FieldId == (int)AlertableEventFields.loginName
                       || condition.FieldId == (int)AlertableEventFields.success)
                       continue;
                   if (!InsertAlertCondition(condition, connection, trans))
                   {
                       trans.Rollback();
                       return false;
                   }
               }

               trans.Commit();
               return true;
            }
         }
      }

		public static int DeleteAlertRule(EventRule rule, string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;
			if(rule == null)
				throw new ArgumentNullException("rule") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return DeleteAlertRule(rule.Id, connection) ;
			}
		}

      public static int DeleteAlertRule(EventRule rule, SqlConnection connection)
		{
			if(rule == null)
				throw new ArgumentNullException("rule") ;
			ValidateConnection(connection) ;
			return DeleteAlertRule(rule.Id, connection) ;
		}

		public static int DeleteAlertRule(int ruleId, string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return DeleteAlertRule(ruleId, connection) ;
			}
		}

		public static int DeleteAlertRule(int ruleId, SqlConnection connection)
		{
			int retVal = 0 ;
			ValidateConnection(connection) ;
			using(SqlTransaction trans = connection.BeginTransaction())
			{
				try
				{
					//DeleteAlertActions(ruleId, connection, trans) ;
					DeleteAlertConditions(ruleId, connection, trans) ;
					using(SqlCommand command = new SqlCommand())
					{
						command.Transaction = trans ;
						command.Connection = connection ;
						command.CommandText = String.Format(DELETE_AlertRule, CoreConstants.RepositoryDatabase,
                     CoreConstants.RepositoryAlertRulesTable, ruleId) ;

						retVal = command.ExecuteNonQuery() ;
						trans.Commit() ;
					}
				}
				catch
				{
					trans.Rollback() ;
				}
			}
			return retVal ;
		}

      public static int UpdateAlertRule(DataAlertRule rule, string connectionString)
      {
         if (rule == null)
            throw new ArgumentNullException("rule");
         if (connectionString == null)
            throw new ArgumentNullException("connectionString");

         using (SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open();
            return UpdateAlertRule(rule, connection);
         }
      }

      public static int UpdateAlertRule(DataAlertRule rule, SqlConnection connection)
      {
         bool bDone = false;
         int retVal;

         if (rule == null)
            throw new ArgumentNullException("rule");
         ValidateConnection(connection);

         using (SqlTransaction trans = connection.BeginTransaction())
         {
            try
            {
               using (SqlCommand command = new SqlCommand())
               {
                  command.Transaction = trans;
                  command.Connection = connection;
                  command.CommandText = String.Format(UPDATE_AlertRule, CoreConstants.RepositoryDatabase,
                     CoreConstants.RepositoryAlertRulesTable, SQLHelpers.CreateSafeString(rule.Name, 60),
                     SQLHelpers.CreateSafeString(rule.Description), (int)rule.Level, (int)rule.RuleType,
                     SQLHelpers.CreateSafeString(rule.TargetInstanceList), rule.Enabled ? 1 : 0,
                     SQLHelpers.CreateSafeString(rule.MessageData), rule.HasLogAction ? 1 : 0,
                     rule.HasEmailAction ? 1 : 0, rule.Id,
                     rule.HasSnmpTrapAction ? 1 : 0,
                     SQLHelpers.CreateSafeString(rule.SnmpConfiguration.ReceiverAddress),
                     rule.SnmpConfiguration.ReceiverPort,
                     SQLHelpers.CreateSafeString(rule.SnmpConfiguration.Community));
                  retVal = command.ExecuteNonQuery();

                  if (retVal == 0)
                     return 0;
                  foreach (EventCondition condition in rule.Conditions)
                  {
                      if (condition.Dirty)
                      {
                          if (condition.Id != AlertingConfiguration.NULL_ID)
                          {
                              if (UpdateAlertCondition(condition, connection, trans) != 1)
                                  return 0;
                          }
                          else
                          {
                              if (!InsertAlertCondition(condition, connection, trans))
                                  return 0;
                          }
                      }
                  }
                  foreach (EventCondition condition in rule.RemovedConditions)
                  {
                      if (DeleteAlertCondition(condition, connection, trans) != 1)
                          return 0;
                  }

                  if (UpdateComparison(rule, connection, trans) == 0)
                     return 0;
               }
               bDone = true;
            }
            finally
            {
               if (!bDone)
                  trans.Rollback();
               else
                  trans.Commit();
            }
         }
         return retVal;
      }

      public static int UpdateAlertRule(StatusAlertRule rule, string connectionString)
      {
         if (rule == null)
            throw new ArgumentNullException("rule");
         if (connectionString == null)
            throw new ArgumentNullException("connectionString");

         using (SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open();
            return UpdateAlertRule(rule, connection);
         }
      }

      public static int UpdateAlertRule(StatusAlertRule rule, SqlConnection connection)
      {
         bool bDone = false;
         int retVal;

         if (rule == null)
            throw new ArgumentNullException("rule");
         ValidateConnection(connection);

         using (SqlTransaction trans = connection.BeginTransaction())
         {
            try
            {
               using (SqlCommand command = new SqlCommand())
               {
                  command.Transaction = trans;
                  command.Connection = connection;
                  command.CommandText = String.Format(UPDATE_AlertRule, CoreConstants.RepositoryDatabase,
                     CoreConstants.RepositoryAlertRulesTable, SQLHelpers.CreateSafeString(rule.Name, 60),
                     SQLHelpers.CreateSafeString(rule.Description), (int)rule.Level, (int)rule.RuleType,
                     SQLHelpers.CreateSafeString(rule.TargetInstanceList), rule.Enabled ? 1 : 0,
                     SQLHelpers.CreateSafeString(rule.MessageData), rule.HasLogAction ? 1 : 0,
                     rule.HasEmailAction ? 1 : 0, rule.Id,
                     rule.HasSnmpTrapAction ? 1 : 0,
                     SQLHelpers.CreateSafeString(rule.SnmpConfiguration.ReceiverAddress),
                     rule.SnmpConfiguration.ReceiverPort,
                     SQLHelpers.CreateSafeString(rule.SnmpConfiguration.Community));
                  retVal = command.ExecuteNonQuery();

                  if (retVal == 0)
                     return 0;

                  if (UpdateThreshold(rule, connection, trans) == 0)
                     return 0;
               }
               bDone = true;
            }
            finally
            {
               if (!bDone)
                  trans.Rollback();
               else
                  trans.Commit();
            }
         }
         return retVal;
      }

		public static int UpdateAlertRule(AlertRule rule, string connectionString)
		{
			if(rule == null)
				throw new ArgumentNullException("rule") ;
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return UpdateAlertRule(rule, connection) ;
			}
		}

		public static int UpdateAlertRule(AlertRule rule, SqlConnection connection)
		{
			bool bDone = false ;
			int retVal ;
			if(rule == null)
				throw new ArgumentNullException("rule") ;
			ValidateConnection(connection) ;
			using(SqlTransaction trans = connection.BeginTransaction())
			{
				try
				{
					using(SqlCommand command = new SqlCommand())
					{
						command.Transaction = trans ;
						command.Connection = connection ;
                        if(rule.LastEmailSummarySendTime == "UpdateOldRule")
						    command.CommandText = String.Format(UPDATE_OldEventAlertRule, CoreConstants.RepositoryDatabase,
                         CoreConstants.RepositoryAlertRulesTable, SQLHelpers.CreateSafeString(rule.Name, 60), 
                         SQLHelpers.CreateSafeString(rule.Description), (int)rule.Level, (int)rule.RuleType, 
                         SQLHelpers.CreateSafeString(rule.TargetInstanceList), rule.Enabled ? 1 : 0,
                         SQLHelpers.CreateSafeString(rule.MessageData), rule.HasLogAction ? 1 : 0,
                         rule.HasEmailAction ? 1 : 0, rule.Id,
                         rule.HasSnmpTrapAction ? 1 : 0,
                         SQLHelpers.CreateSafeString(rule.SnmpConfiguration.ReceiverAddress),
                         rule.SnmpConfiguration.ReceiverPort,
                         SQLHelpers.CreateSafeString(rule.SnmpConfiguration.Community),
                         rule.IsAlertRuleTimeFrameEnabled ? 1 : 0,
                        SQLHelpers.CreateSafeString(rule.AlertRuleTimeFrameStartTime),
                        SQLHelpers.CreateSafeString(rule.AlertRuleTimeFrameEndTime),
                        SQLHelpers.CreateSafeString(rule.AlertRuleTimeFrameDaysOfWeek),
                        rule.HasEmailSummaryAction ? 1 : 0,
                        rule.SummaryEmailFrequency,
                        SQLHelpers.CreateSafeDateTimeString(DateTime.Now));
                        else
                            command.CommandText = String.Format(UPDATE_EventAlertRule, CoreConstants.RepositoryDatabase,
                     CoreConstants.RepositoryAlertRulesTable, SQLHelpers.CreateSafeString(rule.Name, 60),
                     SQLHelpers.CreateSafeString(rule.Description), (int)rule.Level, (int)rule.RuleType,
                     SQLHelpers.CreateSafeString(rule.TargetInstanceList), rule.Enabled ? 1 : 0,
                     SQLHelpers.CreateSafeString(rule.MessageData), rule.HasLogAction ? 1 : 0,
                     rule.HasEmailAction ? 1 : 0, rule.Id,
                     rule.HasSnmpTrapAction ? 1 : 0,
                     SQLHelpers.CreateSafeString(rule.SnmpConfiguration.ReceiverAddress),
                     rule.SnmpConfiguration.ReceiverPort,
                     SQLHelpers.CreateSafeString(rule.SnmpConfiguration.Community),
                     rule.IsAlertRuleTimeFrameEnabled ? 1 : 0,
                    SQLHelpers.CreateSafeString(rule.AlertRuleTimeFrameStartTime),
                    SQLHelpers.CreateSafeString(rule.AlertRuleTimeFrameEndTime),
                    SQLHelpers.CreateSafeString(rule.AlertRuleTimeFrameDaysOfWeek),
                    rule.HasEmailSummaryAction ? 1 : 0,
                    rule.SummaryEmailFrequency);
                        retVal = command.ExecuteNonQuery() ;
						if(retVal == 0)
							return 0 ;
						foreach(EventCondition condition in rule.Conditions)
						{
							if(condition.Dirty)
							{
								if(condition.Id != AlertingConfiguration.NULL_ID)
								{
									if(UpdateAlertCondition(condition, connection, trans) != 1)
										return 0 ;
								}
								else
								{
									if(!InsertAlertCondition(condition, connection, trans))
										return 0 ;
								}
							}
						}
						foreach(EventCondition condition in rule.RemovedConditions)
						{
							if(DeleteAlertCondition(condition, connection, trans) != 1)
								return 0 ;
						}
               }
					bDone = true ;
				}
				finally
				{
					if(!bDone)
						trans.Rollback() ;
					else
						trans.Commit() ;
				}
			}
			return retVal ;
		}

      public static int UpdateAlertRuleEnabled(EventRule rule, string connectionString)
		{
			if(rule == null)
				throw new ArgumentNullException("rule") ;
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return UpdateAlertRuleEnabled(rule, connection) ;
			}
		}

      public static int UpdateAlertRuleEnabled(EventRule rule, SqlConnection connection)
      {
         if (rule == null)
            throw new ArgumentNullException("rule");
         ValidateConnection(connection);

         using (SqlCommand command = new SqlCommand())
         {
            command.Connection = connection;
            command.CommandText = String.Format(UPDATE_AlertRuleEnable, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryAlertRulesTable, rule.Enabled ? 1 : 0, rule.Id);
            return command.ExecuteNonQuery();
         }
      }

      private static int UpdateThreshold(StatusAlertRule rule, SqlConnection connection, SqlTransaction trans)
      {
         if (rule == null)
            throw new ArgumentNullException("rule");
         ValidateConnection(connection);

         using (SqlCommand command = new SqlCommand())
         {
            if (trans != null)
               command.Transaction = trans;
            command.Connection = connection;
            command.CommandText = String.Format(UPDATE_StatusAlertThreshold, CoreConstants.RepositoryDatabase,
                                                                             CoreConstants.RepositoryAlertRuleConditionsTable,
                                                                             (int)rule.StatusType,
                                                                             rule.Threshold.Value.ToString(),
                                                                             rule.Threshold.Id);
            return command.ExecuteNonQuery();
         }
      }

      private static int UpdateComparison(DataAlertRule rule, SqlConnection connection, SqlTransaction trans)
      {
         if (rule == null)
            throw new ArgumentNullException("rule");
         ValidateConnection(connection);

         using (SqlCommand command = new SqlCommand())
         {
            if (trans != null)
               command.Transaction = trans;
            command.Connection = connection;
            command.CommandText = String.Format(UPDATE_StatusAlertThreshold, CoreConstants.RepositoryDatabase,
                                                                             CoreConstants.RepositoryAlertRuleConditionsTable,
                                                                             (int)rule.DataType,
                                                                             SQLHelpers.CreateSafeString(rule.GetStringForComparisonInsert()),
                                                                             rule.Comparison.Id);
            return command.ExecuteNonQuery();
         }
      }
      
		#endregion // Alert Rules

		#region Conditions

		public static ArrayList SelectAlertConditions(AlertRule parentRule, string connectionString)
		{
			if(parentRule == null)
				throw new ArgumentNullException("parentRule") ;
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return SelectAlertConditions(parentRule, connection) ;
			}
		}

   	public static ArrayList SelectAlertConditions(AlertRule parentRule, SqlConnection connection)
		{
			ArrayList retVal = new ArrayList() ;
			int column ;

			if(parentRule == null)
				throw new ArgumentNullException("parentRule") ;
			ValidateConnection(connection) ;

			if(_eventFields == null)
            throw new CoreException("Alertable fields have not been initialized.") ;

         string cmdStr = String.Format(SELECT_AlertCondition, CoreConstants.RepositoryDatabase,
            CoreConstants.RepositoryAlertRuleConditionsTable, parentRule.Id) ;
			using(SqlCommand command = new SqlCommand(cmdStr, connection))
			{
				using(SqlDataReader reader = command.ExecuteReader())
				{
					while(reader.Read())
					{
						EventCondition condition = new EventCondition() ;
						column = 0 ;
						condition.Id = reader.GetInt32(column++) ;
						condition.TargetEventField = (EventField)_eventFields[reader.GetInt32(column++)] ;
						condition.MatchString = reader.GetString(column++) ;
						condition.ParentRule = parentRule ; 
						retVal.Add(condition) ;
					}
				}
			}
			return retVal ;
		}

    public static ArrayList SelectAlertConditions(DataAlertRule parentRule, string connectionString)
    {
        if (parentRule == null)
            throw new ArgumentNullException("parentRule");
        if (connectionString == null)
            throw new ArgumentNullException("connectionString");

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            return SelectAlertConditions(parentRule, connection);
        }
    }
    public static ArrayList SelectAlertConditions(DataAlertRule parentRule, SqlConnection connection)
    {
        ArrayList retVal = new ArrayList();
        int column;

        if (parentRule == null)
            throw new ArgumentNullException("parentRule");
        ValidateConnection(connection);

        if (_eventFields == null)
            throw new CoreException("Alertable fields have not been initialized.");

        string cmdStr = String.Format(SELECT_AlertCondition, CoreConstants.RepositoryDatabase,
           CoreConstants.RepositoryAlertRuleConditionsTable, parentRule.Id);
        using (SqlCommand command = new SqlCommand(cmdStr, connection))
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    EventCondition condition = new EventCondition();
                    column = 0;
                    condition.Id = reader.GetInt32(column++);
                    condition.TargetEventField = (EventField)_eventFields[reader.GetInt32(column++)];
                    condition.MatchString = reader.GetString(column++);
                    condition.ParentRule = parentRule;
                    retVal.Add(condition);
                }
            }
        }
        return retVal;
    }

      public static bool InsertStatusAlertThreshold(StatusAlertRule rule, SqlConnection connection, SqlTransaction trans)
      {
         if (rule == null)
            throw new ArgumentNullException("status alert rule");
         ValidateConnection(connection);
         using (SqlCommand command = new SqlCommand())
         {
            if (trans != null)
               command.Transaction = trans;
            command.Connection = connection;
            command.CommandText = String.Format(INSERT_StatusAlertThreshold,  CoreConstants.RepositoryDatabase,
                                                                              CoreConstants.RepositoryAlertRuleConditionsTable,
                                                                              rule.Id,
                                                                              (int)rule.StatusType,
                                                                              rule.Threshold.Value.ToString());
            object o = command.ExecuteScalar();
            
            if (o is Decimal)
            {
               rule.Threshold.Id = Decimal.ToInt32((Decimal)o);
               return true;
            }
         }
         return false;
      }

      public static bool InsertDataAlertComparison(DataAlertRule rule, SqlConnection connection, SqlTransaction trans)
      {
         if (rule == null)
            throw new ArgumentNullException("status alert rule");
         ValidateConnection(connection);
         
         using (SqlCommand command = new SqlCommand())
         {

            if (trans != null)
               command.Transaction = trans;
            command.Connection = connection;
            command.CommandText = String.Format(INSERT_StatusAlertThreshold,  CoreConstants.RepositoryDatabase,
                                                                              CoreConstants.RepositoryAlertRuleConditionsTable,
                                                                              rule.Id,
                                                                              (int)rule.DataType,
                                                                              SQLHelpers.CreateSafeString(rule.GetStringForComparisonInsert()));
            object o = command.ExecuteScalar();
            
            if (o is Decimal)
            {
               rule.Comparison.Id = Decimal.ToInt32((Decimal)o);
               return true;
            }
         }
         return false;
      }

		public static bool InsertAlertCondition(EventCondition condition, string connectionString)
		{
			if(condition == null)
				throw new ArgumentNullException("condition") ;
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return InsertAlertCondition(condition, connection) ;
			}
		}

		public static bool InsertAlertCondition(EventCondition condition, SqlConnection connection)
		{
			if(condition == null)
				throw new ArgumentNullException("condition") ;
			ValidateConnection(connection) ;
			return InsertAlertCondition(condition, connection, null) ;
		}

		private static bool InsertAlertCondition(EventCondition condition, SqlConnection connection, SqlTransaction trans)
		{
			if(condition == null)
				throw new ArgumentNullException("condition") ;
			ValidateConnection(connection) ;
			using(SqlCommand command = new SqlCommand())
			{
				if(trans != null)
					command.Transaction = trans ;
				command.Connection = connection ;
				command.CommandText = String.Format(INSERT_AlertCondition, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryAlertRuleConditionsTable, condition.RuleId, condition.FieldId, 
               SQLHelpers.CreateSafeString(condition.MatchString)) ;
				object o = command.ExecuteScalar() ;
				if(o is Decimal)
				{
					condition.Id = Decimal.ToInt32((Decimal)o) ;
					return true ;
				}
			}
			return false ;
		}


		public static int DeleteAlertCondition(EventCondition condition, string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;
			if(condition == null)
				throw new ArgumentNullException("condition") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return DeleteAlertCondition(condition, connection) ;
			}
		}

		public static int DeleteAlertCondition(EventCondition condition, SqlConnection connection)
		{
			if(condition == null)
				throw new ArgumentNullException("condition") ;
			ValidateConnection(connection) ;
			return DeleteAlertCondition(condition, connection, null) ;
		}

		private static int DeleteAlertCondition(EventCondition condition, SqlConnection connection, SqlTransaction trans)
		{
			if(condition == null)
				throw new ArgumentNullException("condition") ;
			ValidateConnection(connection) ;
			using(SqlCommand command = new SqlCommand())
			{
				if(trans != null)
					command.Transaction = trans ;
				command.Connection = connection ;
				command.CommandText = String.Format(DELETE_AlertCondition, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryAlertRuleConditionsTable, condition.Id) ;

				return command.ExecuteNonQuery() ;
			}
		}

		public static int DeleteAlertConditions(AlertRule rule, string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;
			if(rule == null)
				throw new ArgumentNullException("rule") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return DeleteAlertConditions(rule.Id, connection) ;
			}
		}

		public static int DeleteAlertConditions(AlertRule rule, SqlConnection connection)
		{
			if(rule == null)
				throw new ArgumentNullException("rule") ;
			ValidateConnection(connection) ;
			return DeleteAlertConditions(rule.Id, connection) ;
		}

		public static int DeleteAlertConditions(int ruleId, string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return DeleteAlertConditions(ruleId, connection) ;
			}
		}

		public static int DeleteAlertConditions(int ruleId, SqlConnection connection)
		{
			ValidateConnection(connection) ;
			return DeleteAlertConditions(ruleId, connection, null) ;
		}

		private static int DeleteAlertConditions(int ruleId, SqlConnection connection, SqlTransaction trans)
		{
			ValidateConnection(connection) ;
			using(SqlCommand command = new SqlCommand())
			{
				if(trans != null)
					command.Transaction = trans ;
				command.Connection = connection ;
				command.CommandText = String.Format(DELETE_AlertConditions, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryAlertRuleConditionsTable, ruleId) ;

				return command.ExecuteNonQuery() ;
			}
		}

		public static int UpdateAlertCondition(EventCondition condition, string connectionString)
		{
			if(condition == null)
				throw new ArgumentNullException("condition") ;
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return UpdateAlertCondition(condition, connection) ;
			}
		}

		public static int UpdateAlertCondition(EventCondition condition, SqlConnection connection)
		{
			if(condition == null)
				throw new ArgumentNullException("condition") ;
			ValidateConnection(connection) ;
			return UpdateAlertCondition(condition, connection, null) ;
		}

		private static int UpdateAlertCondition(EventCondition condition, SqlConnection connection, SqlTransaction trans)
		{
			if(condition == null)
				throw new ArgumentNullException("condition") ;
			ValidateConnection(connection) ;

			using(SqlCommand command = new SqlCommand())
			{
				if(trans != null)
					command.Transaction = trans ;
				command.Connection = connection ;
				command.CommandText = String.Format(UPDATE_AlertCondition, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryAlertRuleConditionsTable, SQLHelpers.CreateSafeString(condition.MatchString), 
               condition.Id) ;
				return command.ExecuteNonQuery() ;
			}
		}

		#endregion // Conditions

		#region Alerts

		public static int SelectAlertCount(string whereClause, string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return SelectAlertCount(whereClause, connection) ;
			}
		}

		public static int SelectAlertCount(string whereClause, SqlConnection connection)
		{
			ValidateConnection(connection) ;
			string queryString = String.Format(SELECT_AlertCount, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryAlertsTable) ;
			if(whereClause != null && whereClause.Length > 0)
				queryString += String.Format(" WHERE {0}", whereClause) ;

			using(SqlCommand command = new SqlCommand(queryString, connection))
			{
				return (int)command.ExecuteScalar() ;
			}
		}

      public static Dictionary<AlertLevel, int> SelectAlertLevelCount(string whereClause, string connectionString)
      {
         if (connectionString == null)
            throw new ArgumentNullException("connectionString");

         using (SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open();
            return SelectAlertLevelCount(whereClause, connection);
         }
      }

      public static Dictionary<AlertLevel, int> SelectAlertLevelCount(string whereClause, SqlConnection connection)
      {
         ValidateConnection(connection);
         Dictionary<AlertLevel, int> retVal = new Dictionary<AlertLevel, int> () ;
         if (whereClause != null && whereClause.Length > 0)
            whereClause = String.Format(" WHERE {0}", whereClause);
         else
            whereClause = "" ;
         string queryString = String.Format(SELECT_AlertLevelCount, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryAlertsTable, whereClause);

         using (SqlCommand command = new SqlCommand(queryString, connection))
         {
            using (SqlDataReader reader = command.ExecuteReader())
            {
               while (reader.Read())
               {
                  int count ;
                  AlertLevel level ;
                  int column = 0;

                  count = reader.GetInt32(column++) ;
                  level = (AlertLevel)reader.GetByte(column++);
                  retVal.Add(level, count);
               }
            }
         }
         return retVal ;
      }

      public static List<Alert> SelectAlerts(string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return SelectAlerts(connection) ;
			}
		}

		public static List<Alert> SelectAlerts(SqlConnection connection)
		{
			List<Alert> retVal = new List<Alert>() ;
			int column ;

			ValidateConnection(connection) ;

         string cmdStr = String.Format(SELECT_Alert, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryAlertsTable) ;
			using(SqlCommand command = new SqlCommand(cmdStr, connection))
			{
				using(SqlDataReader reader = command.ExecuteReader())
				{
					while(reader.Read())
					{
						Alert alert = new Alert() ;
						column = 0 ;

						alert.Id = reader.GetInt32(column++) ;
						alert.AlertType = (EventType)reader.GetInt32(column++) ;
						alert.ParentRuleId = reader.GetInt32(column++) ;
						alert.EventId = reader.GetInt32(column++) ;
						alert.Instance = reader.GetString(column++) ;
						alert.EventType = reader.GetInt32(column++) ;
						alert.Created = reader.GetDateTime(column++) ;
						alert.Level = (AlertLevel)reader.GetByte(column++) ;
                  alert.EmailStatus = (NotificationStatus)reader.GetByte(column++) ;
                  alert.LogStatus = (NotificationStatus)reader.GetByte(column++) ;
                  alert.MessageData = reader.GetString(column++) ;
                  alert.RuleName = reader.GetString(column++) ;
					    alert.SnmpTrapStatus = (NotificationStatus) reader.GetByte(column++);
						retVal.Add(alert) ;
					}
				}
			}

			return retVal ;
		}

      public static List<Alert> SelectMostRecentAlerts(int count, string connectionString)
      {
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return SelectMostRecentAlerts(count, connection) ;
         }
      }

      public static List<Alert> SelectMostRecentAlerts(int count, SqlConnection connection)
      {
         List<Alert> retVal = new List<Alert>() ;
         int column ;

         if(count < 1)
            throw new ArgumentException("count", "count cannot be less than zero.") ;

         ValidateConnection(connection) ;

         string cmdStr = String.Format(SELECT_RecentAlerts, count, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryAlertsTable) ;
         using(SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using(SqlDataReader reader = command.ExecuteReader())
            {
               while(reader.Read())
               {
                  Alert alert = new Alert() ;
                  column = 0 ;

                  alert.Id = reader.GetInt32(column++) ;
                  alert.AlertType = (EventType)reader.GetInt32(column++) ;
                  alert.ParentRuleId = reader.GetInt32(column++) ;
                  alert.EventId = reader.GetInt32(column++) ;
                  alert.Instance = reader.GetString(column++) ;
                  alert.EventType = reader.GetInt32(column++) ;
                  alert.Created = reader.GetDateTime(column++) ;
                  alert.Level = (AlertLevel)reader.GetByte(column++) ;
                  alert.EmailStatus = (NotificationStatus)reader.GetByte(column++) ;
                  alert.LogStatus = (NotificationStatus)reader.GetByte(column++) ;
                  alert.MessageData = reader.GetString(column++) ;
                  alert.RuleName = reader.GetString(column++) ;
                  alert.SnmpTrapStatus = (NotificationStatus)reader.GetByte(column++);
                  retVal.Add(alert) ;
               }
            }
         }

         return retVal ;
      }
      
      public static List<Alert> SelectAlerts(string sQuery, string connectionString)
		{
			if(sQuery == null)
				throw new ArgumentNullException("sQuery") ;
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return SelectAlerts(sQuery, connection) ;
			}
		}

		public static List<Alert> SelectAlerts(string sQuery, SqlConnection connection)
		{
			List<Alert> retVal = new List<Alert>() ;
			int column ;

			if(sQuery == null)
				throw new ArgumentNullException("sQuery") ;
			ValidateConnection(connection) ;

			using(SqlCommand command = new SqlCommand(sQuery, connection))
			{
				using(SqlDataReader reader = command.ExecuteReader())
				{
					while(reader.Read())
					{
						Alert alert = new Alert() ;
						column = 0 ;

						alert.Id = reader.GetInt32(column++) ;
						alert.AlertType = (EventType)reader.GetInt32(column++) ;
						alert.ParentRuleId = reader.GetInt32(column++) ;
						alert.EventId = reader.GetInt32(column++) ;
						alert.Instance = reader.GetString(column++) ;
						alert.EventType = reader.GetInt32(column++) ;
						alert.Created = reader.GetDateTime(column++) ;
						alert.Level = (AlertLevel)reader.GetByte(column++) ;
                  alert.EmailStatus = (NotificationStatus)reader.GetByte(column++) ;
                  alert.LogStatus = (NotificationStatus)reader.GetByte(column++) ;
                  alert.MessageData = reader.GetString(column++) ;
                  alert.RuleName = reader.GetString(column++) ;
                  retVal.Add(alert) ;
					}
				}
			}

			return retVal ;
		}

      public static List<DataAlert> SelectDataAlerts(string sQuery, string connectionString)
      {
         if (sQuery == null)
            throw new ArgumentNullException("sQuery");
         if (connectionString == null)
            throw new ArgumentNullException("connectionString");

         using (SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open();
            return SelectDataAlerts(sQuery, connection);
         }
      }

      public static List<DataAlert> SelectDataAlerts(string sQuery, SqlConnection connection)
      {
         List<DataAlert> retVal = new List<DataAlert>();
         int column;

         if (sQuery == null)
            throw new ArgumentNullException("sQuery");
         ValidateConnection(connection);

         using (SqlCommand command = new SqlCommand(sQuery, connection))
         {
            using (SqlDataReader reader = command.ExecuteReader())
            {
               while (reader.Read())
               {
                  DataAlert alert = new DataAlert();
                  column = 0;

                  alert.Id = reader.GetInt32(column++);
                  alert.EventType = (DataRuleType)reader.GetInt32(column++);
                  alert.ParentRuleId = reader.GetInt32(column++);
                  alert.EventId = reader.GetInt32(column++);
                  alert.Instance = reader.GetString(column++);
                  alert.AlertType = (EventType)reader.GetInt32(column++);
                  alert.Created = reader.GetDateTime(column++);
                  alert.Level = (AlertLevel)reader.GetByte(column++);
                  alert.EmailStatus = (NotificationStatus)reader.GetByte(column++);
                  alert.LogStatus = (NotificationStatus)reader.GetByte(column++);
                  alert.MessageData = reader.GetString(column++);
                  alert.RuleName = reader.GetString(column++);
                  int alertEventId = reader.GetInt32(column++);
                  TraceEventType eventT = (TraceEventType)alertEventId;
                  alert.AlertEventTypeString = eventT.ToString();
                  alert.AlertEventType = alertEventId;
                  retVal.Add(alert);
               }
            }
         }

         return retVal;
      }

      public static List<StatusAlert> SelectStatusAlerts(string sQuery, string connectionString)
      {
         if (sQuery == null)
            throw new ArgumentNullException("sQuery");
         if (connectionString == null)
            throw new ArgumentNullException("connectionString");

         using (SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open();
            return SelectStatusAlerts(sQuery, connection);
         }
      }

      public static List<StatusAlert> SelectStatusAlerts(string sQuery, SqlConnection connection)
      {
         List<StatusAlert> retVal = new List<StatusAlert>();
         int column;

         if (sQuery == null)
            throw new ArgumentNullException("sQuery");
         ValidateConnection(connection);

         using (SqlCommand command = new SqlCommand(sQuery, connection))
         {
            using (SqlDataReader reader = command.ExecuteReader())
            {
               while (reader.Read())
               {
                  StatusAlert alert = new StatusAlert();
                  column = 0;

                  alert.Id = reader.GetInt32(column++);
                  alert.ParentRuleId = reader.GetInt32(column++);
                  alert.Created = reader.GetDateTime(column++);
                  alert.Level = (AlertLevel)reader.GetByte(column++);
                  alert.EmailStatus = (NotificationStatus)reader.GetByte(column++);
                  alert.LogStatus = (NotificationStatus)reader.GetByte(column++);
                  alert.MessageData = reader.GetString(column++);

                  if (reader.IsDBNull(column))
                  {
                     alert.Instance = "N/A";
                     column++;
                  }
                  else
                  {
                     alert.Instance = reader.GetString(column++);
                  }

                  if (reader.IsDBNull(column))
                  {
                     alert.ComputerName = "N/A";
                     column++;
                  }
                  else
                  {
                     alert.ComputerName = reader.GetString(column++);
                  }
                  alert.AlertType = (EventType)reader.GetInt32(column++);
                  alert.RuleName = reader.GetString(column++);
                  alert.StatusRuleTypeName = reader.GetString(column++);
                  retVal.Add(alert);
               }
            }
         }
         return retVal;
      }

		public static Alert SelectAlertById(int id, string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return SelectAlertById(id, connection) ;
			}
		}

		public static Alert SelectAlertById(int id, SqlConnection connection)
		{
			Alert alert = null ;

			ValidateConnection(connection) ;
         string cmdStr = String.Format(SELECT_AlertId, CoreConstants.RepositoryDatabase,
            CoreConstants.RepositoryAlertsTable, id) ;
			using(SqlCommand command = new SqlCommand(cmdStr, connection))
			{
				using(SqlDataReader reader = command.ExecuteReader())
				{
					if(reader.HasRows)
					{
						alert = new Alert() ;

						reader.Read() ;
						int column = 0 ;

						alert.Id = reader.GetInt32(column++) ;
						alert.AlertType = (EventType)reader.GetInt32(column++) ;
						alert.ParentRuleId = reader.GetInt32(column++) ;
						alert.EventId = reader.GetInt32(column++) ;
						alert.Instance = reader.GetString(column++) ;
						alert.EventType = reader.GetInt32(column++) ;
						alert.Created = reader.GetDateTime(column++) ;
						alert.Level = (AlertLevel)reader.GetByte(column++) ;
                  alert.EmailStatus = (NotificationStatus)reader.GetByte(column++) ;
                  alert.LogStatus = (NotificationStatus)reader.GetByte(column++) ;
                  alert.MessageData = reader.GetString(column++) ;
                  alert.RuleName = reader.GetString(column++) ;
                  alert.SnmpTrapStatus = (NotificationStatus)reader.GetByte(column++);
               }
				}
			}
			return alert ; 
		}

		public static bool InsertAlert(Alert alert, string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;
			if(alert == null)
				throw new ArgumentNullException("alert") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return InsertAlert(alert, connection) ;
			}
		}

		public static bool InsertAlert(Alert alert, SqlConnection connection)
		{
			if(alert == null)
				throw new ArgumentNullException("alert") ;
			ValidateConnection(connection) ;

			using(SqlTransaction trans = connection.BeginTransaction())
			{
				using(SqlCommand command = new SqlCommand())
				{
               string trimmedAlertMessage = alert.MessageData;
               if (trimmedAlertMessage.Length > 2999)
                  trimmedAlertMessage = trimmedAlertMessage.Substring(0, 2999);
					command.Transaction = trans ;
					command.Connection = connection ;
					command.CommandText = String.Format(INSERT_Alert, CoreConstants.RepositoryDatabase,
                  CoreConstants.RepositoryAlertsTable, (int)alert.AlertType, alert.ParentRuleId, 
                  alert.EventId, SQLHelpers.CreateSafeString(alert.Instance), alert.EventType, 
                  (int)alert.Level, (int)alert.EmailStatus, (int)alert.LogStatus,
                  SQLHelpers.CreateSafeString(trimmedAlertMessage), SQLHelpers.CreateSafeString(alert.RuleName), (int)alert.SnmpTrapStatus);
					using(SqlDataReader reader = command.ExecuteReader())
					{
						if(!reader.HasRows)
						{
							trans.Rollback() ;
							return false ;
						}
						reader.Read() ;
						alert.Id = reader.GetInt32(0) ;
						alert.Created = reader.GetDateTime(1) ;

                  ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                          String.Format("Alert: alert {0} is created at {1}.",
                                                         alert.Id, DateTime.Now));
               }
				}
				trans.Commit() ;
			}
			return true ;
		}

      public static bool InsertAlert(StatusAlert alert, string connectionString)
      {
         if (connectionString == null)
            throw new ArgumentNullException("connectionString");
         if (alert == null)
            throw new ArgumentNullException("alert");

         using (SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open();
            return InsertAlert(alert, connection);
         }
      }

      public static bool InsertAlert(StatusAlert alert, SqlConnection connection)
      {
         if (alert == null)
            throw new ArgumentNullException("alert");
         ValidateConnection(connection);

         using (SqlTransaction trans = connection.BeginTransaction())
         {
            using (SqlCommand command = new SqlCommand())
            {
               string trimmedAlertMessage = alert.MessageData;
               if (trimmedAlertMessage.Length > 2999)
                  trimmedAlertMessage = trimmedAlertMessage.Substring(0, 2999);
               command.Transaction = trans;
               command.Connection = connection;
               command.CommandText = String.Format(INSERT_StatusAlert, CoreConstants.RepositoryDatabase,
                                                   CoreConstants.RepositoryAlertsTable, (int)alert.AlertType, alert.ParentRuleId,
                                                   (int)alert.EventType, SQLHelpers.CreateSafeString(alert.Instance), 
                                                   SQLHelpers.CreateSafeString(alert.ComputerName), (int)alert.EventType,
                                                   (int)alert.Level, (int)alert.EmailStatus, (int)alert.LogStatus,
                                                   SQLHelpers.CreateSafeString(trimmedAlertMessage), SQLHelpers.CreateSafeString(alert.RuleName), (int)alert.SnmpTrapStatus);
               using (SqlDataReader reader = command.ExecuteReader())
               {
                  if (!reader.HasRows)
                  {
                     trans.Rollback();
                     return false;
                  }
                  reader.Read();
                  alert.Id = reader.GetInt32(0);
                  alert.Created = reader.GetDateTime(1);
                  ErrorLog.Instance.Write(ErrorLog.Level.Debug, String.Format("Alert: Status alert {0} is created at {1}.", alert.Id, DateTime.Now));
               }
            }
            trans.Commit();
         }
         return true;
      }

      public static bool InsertAlert(DataAlert alert, string connectionString)
      {
         if (connectionString == null)
            throw new ArgumentNullException("connectionString");
         if (alert == null)
            throw new ArgumentNullException("alert");

         using (SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open();
            return InsertAlert(alert, connection);
         }
      }

      public static bool InsertAlert(DataAlert alert, SqlConnection connection)
      {
         if (alert == null)
            throw new ArgumentNullException("alert");
         ValidateConnection(connection);

         using (SqlTransaction trans = connection.BeginTransaction())
         {
            using (SqlCommand command = new SqlCommand())
            {
               string trimmedAlertMessage = alert.MessageData;
               if (trimmedAlertMessage.Length > 2999)
                  trimmedAlertMessage = trimmedAlertMessage.Substring(0, 2999);
               command.Transaction = trans;
               command.Connection = connection;
               command.CommandText = String.Format(INSERT_DataAlert, CoreConstants.RepositoryDatabase,
                                                                     CoreConstants.RepositoryAlertsTable, 
                                                                     (int)alert.AlertType, 
                                                                     alert.ParentRuleId,
                                                                     (int)alert.EventId, 
                                                                     SQLHelpers.CreateSafeString(alert.Instance),
                                                                     (int)alert.EventType,
                                                                     (int)alert.Level, 
                                                                     (int)alert.EmailStatus, 
                                                                     (int)alert.LogStatus,
                                                                     SQLHelpers.CreateSafeString(trimmedAlertMessage), 
                                                                     SQLHelpers.CreateSafeString(alert.RuleName),
                                                                     (int)alert.SnmpTrapStatus, (int)alert.AlertEventType);
               using (SqlDataReader reader = command.ExecuteReader())
               {
                  if (!reader.HasRows)
                  {
                     trans.Rollback();
                     return false;
                  }
                  reader.Read();
                  alert.Id = reader.GetInt32(0);
                  alert.Created = reader.GetDateTime(1);
                  ErrorLog.Instance.Write(ErrorLog.Level.Debug, String.Format("Alert: Data alert {0} is created at {1}.", alert.Id, DateTime.Now));
               }
            }
            trans.Commit();
         }
         return true;
      }

      public static int DeleteAlert(Alert alert, string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;
			if(alert == null)
				throw new ArgumentNullException("alert") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return DeleteAlert(alert.Id, connection) ;
			}
		}

		public static int DeleteAlert(Alert alert, SqlConnection connection)
		{
			if(alert == null)
				throw new ArgumentNullException("alert") ;
			ValidateConnection(connection) ;
			return DeleteAlert(alert.Id, connection) ;
		}

		public static int DeleteAlert(int alertId, string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return DeleteAlert(alertId, connection) ;
			}
		}

		public static int DeleteAlert(int alertId, SqlConnection connection)
		{
			ValidateConnection(connection) ;
			using(SqlCommand command = new SqlCommand())
			{
				command.Connection = connection ;
				command.CommandText = String.Format(DELETE_Alert, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryAlertsTable, alertId) ;

				return command.ExecuteNonQuery() ;
			}
		}

      public static int DeleteAlerts(string instance, DateTime createdBefore, string connectionString)
      {
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return DeleteAlerts(instance, createdBefore, connection) ;
         }
      }

      public static int DeleteAlerts(string instance, DateTime createdBefore, SqlConnection connection)
      {
         ValidateConnection(connection) ;
         int totalRemoved = 0 ;
         using(SqlCommand command = new SqlCommand())
         {
            int removed ;
            command.Connection = connection ;
            command.CommandText = String.Format(DELETE_AlertsTimeInstance, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryAlertsTable, SQLHelpers.CreateSafeString(instance),
               SQLHelpers.CreateSafeDateTime(createdBefore)) ;
            removed = command.ExecuteNonQuery() ;
            while(removed > 0)
            {
               totalRemoved += removed ;
               removed = command.ExecuteNonQuery() ;
            }

            return totalRemoved ;
         }
      }
      
      public static int DeleteAlerts(DateTime createdBefore, string connectionString)
      {
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return DeleteAlerts(createdBefore, connection) ;
         }
      }

      public static int DeleteAlerts(DateTime createdBefore, SqlConnection connection)
      {
         int totalRemoved = 0 ;
         ValidateConnection(connection) ;
         using(SqlCommand command = new SqlCommand())
         {
            int removed ;
            command.Connection = connection ;
            command.CommandText = String.Format(DELETE_AlertsTime, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryAlertsTable, SQLHelpers.CreateSafeDateTime(createdBefore)) ;

            removed = command.ExecuteNonQuery() ;
            while(removed > 0)
            {
               totalRemoved += removed ;
               removed = command.ExecuteNonQuery() ;
            }

            return totalRemoved ;
         }
      }

      public static int UpdateAlert(Alert alert, string connectionString)
      {
         if(alert == null)
            throw new ArgumentNullException("alert") ;
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return UpdateAlert(alert, connection) ;
         }
      }

      public static int UpdateAlert(Alert alert, SqlConnection connection)
      {
         if(alert == null)
            throw new ArgumentNullException("alert") ;
         ValidateConnection(connection) ;

         using(SqlCommand command = new SqlCommand())
         {
            command.Connection = connection ;
            command.CommandText = String.Format(UPDATE_Alert, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryAlertsTable, (int)alert.EmailStatus, (int)alert.LogStatus,
               alert.Id, (int)alert.SnmpTrapStatus) ;
            return command.ExecuteNonQuery() ;
         }
      }

      public static int UpdateAlert(StatusAlert alert, SqlConnection connection)
      {
         if(alert == null)
            throw new ArgumentNullException("alert") ;
         ValidateConnection(connection) ;

         using(SqlCommand command = new SqlCommand())
         {
            command.Connection = connection ;
            command.CommandText = String.Format(UPDATE_Alert, CoreConstants.RepositoryDatabase,
                                                CoreConstants.RepositoryAlertsTable, (int)alert.EmailStatus, (int)alert.LogStatus,
                                                alert.Id, (int)alert.SnmpTrapStatus) ;
            return command.ExecuteNonQuery();
         }
      }
      
      public static int UpdateAlert(DataAlert alert, SqlConnection connection)
      {
         if(alert == null)
            throw new ArgumentNullException("alert") ;
         ValidateConnection(connection) ;

         using(SqlCommand command = new SqlCommand())
         {
            command.Connection = connection ;
            command.CommandText = String.Format(UPDATE_Alert, CoreConstants.RepositoryDatabase,
                                                CoreConstants.RepositoryAlertsTable, (int)alert.EmailStatus, (int)alert.LogStatus,
                                                alert.Id, (int)alert.SnmpTrapStatus) ;
            return command.ExecuteNonQuery();
         }
      }
      #endregion // Alerts

      /*
		#region Action Results

		public static ActionResultCollection SelectActionResults(Alert alert, string connectionString)
		{
			if(alert == null)
				throw new ArgumentNullException("alert") ;
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return SelectActionResults(alert, connection) ;
			}
		}

		public static ActionResultCollection SelectActionResults(Alert alert, SqlConnection connection)
		{
			ActionResultCollection retVal = new ActionResultCollection() ;
			int column = 0 ;

			if(alert == null)
				throw new ArgumentNullException("alert") ;
			ValidateConnection(connection) ;

         string cmdStr = String.Format(SELECT_ActionResult, CoreConstants.RepositoryDatabase,
            CoreConstants.RepositoryActionResultsTable, alert.Id) ;
			using(SqlCommand command = new SqlCommand(cmdStr, connection))
			{
				using(SqlDataReader reader = command.ExecuteReader())
				{
					while(reader.Read())
					{
						ActionResult result = new ActionResult() ;
						column = 0 ;

						result.Id = reader.GetInt32(column++) ;
						result.AlertId = reader.GetInt32(column++) ;
						result.ActionId = reader.GetInt32(column++) ;
						result.Status = (ActionResultStatus)reader.GetInt32(column++) ;
						result.ActionData = reader.GetString(column++) ;
						result.ResultData = reader.GetString(column++) ;
						retVal.Add(result) ;
					}
				}
			}

			return retVal ;
		}

		public static bool InsertActionResult(ActionResult result, string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;
			if(result == null)
				throw new ArgumentNullException("result") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return InsertActionResult(result, connection) ;
			}
		}

		public static bool InsertActionResult(ActionResult result, SqlConnection connection)
		{
			if(result == null)
				throw new ArgumentNullException("result") ;
			ValidateConnection(connection) ;

			return InsertActionResult(result, connection, null) ;
		}

		public static bool InsertActionResult(ActionResult result, SqlConnection connection, SqlTransaction trans)
		{
			if(result == null)
				throw new ArgumentNullException("result") ;
			ValidateConnection(connection) ;

			using(SqlCommand command = new SqlCommand())
			{
				if(trans != null)
					command.Transaction = trans ;
				command.Connection = connection ;
				command.CommandText = String.Format(INSERT_ActionResult, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryActionResultsTable, result.AlertId, result.ActionId, (int)result.Status, 
               SQLHelpers.CreateSafeString(result.ActionData), SQLHelpers.CreateSafeString(result.ResultData)) ;
				object o = command.ExecuteScalar() ;
				if(o is Decimal)
				{
					result.Id = Decimal.ToInt32((Decimal)o) ;
					return true ;
				}
			}
			return false ;
		}

		public static int DeleteActionResult(ActionResult result, string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;
			if(result == null)
				throw new ArgumentNullException("result") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return DeleteActionResult(result.Id, connection) ;
			}
		}

		public static int DeleteActionResult(ActionResult result, SqlConnection connection)
		{
			if(result == null)
				throw new ArgumentNullException("result") ;
			ValidateConnection(connection) ;
			return DeleteActionResult(result.Id, connection) ;
		}

		public static int DeleteActionResult(int resultId, string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return DeleteActionResult(resultId, connection) ;
			}
		}

		public static int DeleteActionResult(int resultId, SqlConnection connection)
		{
			ValidateConnection(connection) ;
			using(SqlCommand command = new SqlCommand())
			{
				command.Connection = connection ;
				command.CommandText = String.Format(DELETE_ActionResult, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryActionResultsTable, resultId) ;

				return command.ExecuteNonQuery() ;
			}
		}

		public static int DeleteActionResults(Alert alert, string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;
			if(alert == null)
				throw new ArgumentNullException("alert") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return DeleteActionResults(alert.Id, connection) ;
			}
		}

		public static int DeleteActionResults(Alert alert, SqlConnection connection)
		{
			if(alert == null)
				throw new ArgumentNullException("alert") ;
			ValidateConnection(connection) ;
			return DeleteActionResults(alert.Id, connection) ;
		}

		public static int DeleteActionResults(int alertId, string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return DeleteActionResults(alertId, connection) ;
			}
		}

		public static int DeleteActionResults(int alertId, SqlConnection connection)
		{
			ValidateConnection(connection) ;
			return DeleteActionResults(alertId, connection, null) ;
		}

		private static int DeleteActionResults(int alertId, SqlConnection connection, SqlTransaction trans)
		{
			ValidateConnection(connection) ;
			using(SqlCommand command = new SqlCommand())
			{
				if(trans != null)
					command.Transaction = trans ;
				command.Connection = connection ;
				command.CommandText = String.Format(DELETE_ActionResults, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryActionResultsTable, alertId) ;

				return command.ExecuteNonQuery() ;
			}
		}

		public static int UpdateActionResult(ActionResult result, string connectionString)
		{
			if(result == null)
				throw new ArgumentNullException("result") ;
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return UpdateActionResult(result, connection) ;
			}
		}

		public static int UpdateActionResult(ActionResult result, SqlConnection connection)
		{
			if(result == null)
				throw new ArgumentNullException("result") ;
			ValidateConnection(connection) ;

			using(SqlCommand command = new SqlCommand())
			{
				command.Connection = connection ;
				command.CommandText = String.Format(UPDATE_ActionResult, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryActionResultsTable, (int)result.Status, SQLHelpers.CreateSafeString(result.ActionData), 
               SQLHelpers.CreateSafeString(result.ResultData), result.Id) ;
				return command.ExecuteNonQuery() ;
			}
		}

		public static int UpdateActionResultStatus(ActionResult result, string connectionString)
		{
			if(result == null)
				throw new ArgumentNullException("result") ;
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return UpdateActionResultStatus(result, connection) ;
			}
		}

		public static int UpdateActionResultStatus(ActionResult result, SqlConnection connection)
		{
			if(result == null)
				throw new ArgumentNullException("result") ;
			ValidateConnection(connection) ;

			using(SqlCommand command = new SqlCommand())
			{
				command.Connection = connection ;
				command.CommandText = String.Format(UPDATE_ActionResultStatus, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryActionResultsTable, (int)result.Status, result.Id) ;
				return command.ExecuteNonQuery() ;
			}
		}

		public static int UpdateActionResultData(ActionResult result, string connectionString)
		{
			if(result == null)
				throw new ArgumentNullException("result") ;
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return UpdateActionResultData(result, connection) ;
			}
		}

		public static int UpdateActionResultData(ActionResult result, SqlConnection connection)
		{
			if(result == null)
				throw new ArgumentNullException("result") ;
			ValidateConnection(connection) ;

			using(SqlCommand command = new SqlCommand())
			{
				command.Connection = connection ;
				command.CommandText = String.Format(UPDATE_ActionResultData, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryActionResultsTable, SQLHelpers.CreateSafeString(result.ActionData), 
               SQLHelpers.CreateSafeString(result.ResultData), result.Id) ;
				return command.ExecuteNonQuery() ;
			}
		}

		#endregion // Action Results*/

		#region Alerting Configuration

	    public static SNMPConfiguration SelectSnmpConfiguration(string connectionString)
	    {
	        if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

	        using (SqlConnection connection = new SqlConnection(connectionString))
	        {
	            connection.Open();
	            return SelectSnmpConfiguration(connection);
	        }
	    }

	    public static SNMPConfiguration SelectSnmpConfiguration(SqlConnection connection)
	    {
            ValidateConnection(connection);

	        string sql = string.Format(SELECT_SnmpConfig, 
                                       CoreConstants.RepositoryDatabase,
	                                   CoreConstants.RepositoryConfigurationTable);

	        using (SqlCommand command = new SqlCommand(sql, connection))
	        {
	            using (SqlDataReader reader = command.ExecuteReader())
	            {
	                if (reader.Read())
	                {
                        SNMPConfiguration result = new SNMPConfiguration();

	                    int columnIndex = 0;

	                    result.ReceiverAddress = SQLHelpers.GetString(reader, columnIndex++);                      
	                    result.ReceiverPort = SQLHelpers.GetInt32(reader, columnIndex++);
	                    result.Community = SQLHelpers.GetString(reader, columnIndex);

                        return result;
	                }
	            }
	        }

	        return null;
	    }

	    public static int UpdateSnmpConfiguration(SNMPConfiguration configuration, string connectionString)
	    {
	        if (configuration == null)
                throw new ArgumentNullException("configuration");
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

	        using (SqlConnection connection = new SqlConnection(connectionString))
	        {
                connection.Open();
	            return UpdateSnmpConfiguration(configuration, connection);
	        }
	    }

	    public static int UpdateSnmpConfiguration(SNMPConfiguration configuration, SqlConnection connection)
	    {
	        if (configuration == null)
                throw new ArgumentNullException("configuration");

            ValidateConnection(connection);

            string sql = string.Format(UPDATE_SnmpConfig,
                                           CoreConstants.RepositoryDatabase,
                                           CoreConstants.RepositoryConfigurationTable,
                                           SQLHelpers.CreateSafeString(configuration.ReceiverAddress),
                                           configuration.ReceiverPort,
                                           SQLHelpers.CreateSafeString(configuration.Community));

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                command.Connection = connection;

                return command.ExecuteNonQuery();
            }   
	    }

		public static SmtpConfiguration SelectSmtpConfiguration(string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;
			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return SelectSmtpConfiguration(connection) ;
			}
		}

		public static SmtpConfiguration SelectSmtpConfiguration(SqlConnection connection)
		{
			SmtpConfiguration retVal = new SmtpConfiguration() ;
			ValidateConnection(connection) ;

			// Get the Rule objects
         string cmdStr = String.Format(SELECT_SmtpConfig, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryConfigurationTable) ;
			using(SqlCommand command = new SqlCommand(cmdStr, connection))
			{
				using(SqlDataReader reader = command.ExecuteReader())
				{
					if(reader.Read())
					{
						int column = 0 ;

						retVal.Server = SQLHelpers.GetString(reader, column++) ;
						if(!reader.IsDBNull(column))
							retVal.Port = reader.GetInt32(column++) ;
						else
							column++ ;
						if(!reader.IsDBNull(column))
							retVal.Authentication = (SmtpAuthProtocol)reader.GetInt32(column++) ;
						else
							column++ ;
						if(!reader.IsDBNull(column))
							retVal.UseSsl = !SqlByte.Zero.Equals(reader.GetSqlByte(column++)) ;
						else
							column++ ;
						retVal.Username = SQLHelpers.GetString(reader, column++) ;
						retVal.Password = SQLHelpers.GetString(reader, column++) ;
						retVal.SenderAddress = SQLHelpers.GetString(reader, column++) ;
						retVal.SenderName = SQLHelpers.GetString(reader, column++) ;

						return retVal ;
					}
				}
			}
			return null ;
		}

		public static int UpdateSmtpConfiguration(SmtpConfiguration config, string connectionString)
		{
			if(config == null)
				throw new ArgumentNullException("config") ;
			if(connectionString == null)
				throw new ArgumentNullException("connectionString") ;

			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open() ;
				return UpdateSmtpConfiguration(config, connection) ;
			}
		}

		public static int UpdateSmtpConfiguration(SmtpConfiguration config, SqlConnection connection)
		{
			if(config == null)
				throw new ArgumentNullException("config") ;
			ValidateConnection(connection) ;

			using(SqlCommand command = new SqlCommand())
			{
				command.Connection = connection ;
				command.CommandText = String.Format(UPDATE_SmtpConfig, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryConfigurationTable, SQLHelpers.CreateSafeString(config.Server), 
               config.Port, (int)config.Authentication, config.UseSsl ? 1 : 0, 
               SQLHelpers.CreateSafeString(config.Username), SQLHelpers.CreateSafeString(config.Password), 
               SQLHelpers.CreateSafeString(config.SenderAddress), SQLHelpers.CreateSafeString(config.SenderName)) ;
				return command.ExecuteNonQuery() ;
			}
		}

		#endregion // Alerting Configuration

      #region Audit Events

      public static List<SensitiveColumnEvent> SelectSCAuditEvents(int count,int alertHighWatermark, int highWatermark, string serverDbName, string connectionString)
      {
         if(count < 1)
            throw new ArgumentException("count", "count must be greater than zero.") ;
         if(serverDbName == null)
            throw new ArgumentNullException("serverDbName") ;
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return SelectSCAuditEvents(count, alertHighWatermark, highWatermark, serverDbName, connection);
         }
      }

      public static List<SensitiveColumnEvent> SelectSCAuditEvents(int count, int lowWatermark, int highWatermark, string serverDbName, SqlConnection connection)
      {
         if(serverDbName == null)
            throw new ArgumentNullException("serverDbName") ;
         ValidateConnection(connection) ;
         List<SensitiveColumnEvent> scEvents = new List<SensitiveColumnEvent>();

         string cmdStr = String.Format(SELECT_SCAuditEvents, 
                                       count, 
                                       SQLHelpers.CreateSafeDatabaseName(serverDbName), 
                                       CoreConstants.RepositoryEventsTable, 
                                       CoreConstants.RepositorySensitiveColumnsTable,
                                       lowWatermark,
                                       highWatermark) ;
         using(SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using(SqlDataReader reader = command.ExecuteReader())
            {
               SensitiveColumnEvent scEvent;
               int column ;

               while(reader.Read())
               {
                  scEvent = new SensitiveColumnEvent();
                  scEvent.Load(reader);
                  scEvents.Add(scEvent);
               }
            }
         }
         return scEvents;      
      }

      public static List<BeforeAfterEvent> SelectBADAuditEvents(int count, string serverName, string serverDbName, string connectionString)
      {
         if(count < 1)
            throw new ArgumentException("count", "count must be greater than zero.") ;
         if(serverDbName == null)
            throw new ArgumentNullException("serverDbName") ;
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return SelectBADAuditEvents(count, serverName, serverDbName, connection);
         }
      }

      public static List<BeforeAfterEvent> SelectBADAuditEvents(int count, string serverName, string serverDbName, SqlConnection connection)
      {
         if(serverDbName == null)
            throw new ArgumentNullException("serverDbName");
         ValidateConnection(connection) ;
         List<BeforeAfterEvent> badEvents = new List<BeforeAfterEvent>();

         string cmdStr = String.Format(SELECT_BADAuditEvents, 
                                       count, 
                                       SQLHelpers.CreateSafeDatabaseName(serverName),
                                       SQLHelpers.CreateSafeDatabaseName(serverDbName), 
                                       CoreConstants.RepositoryDataChangesTable,
                                       CoreConstants.RepositoryColumnChangesTable,
                                       CoreConstants.RepositoryDatabase,
                                       CoreConstants.RepositoryDatabaseTable,
                                       CoreConstants.RepositoryEventsTable
                                       );
         using(SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using(SqlDataReader reader = command.ExecuteReader())
            {
               BeforeAfterEvent badEvent;

               while(reader.Read())
               {
                  badEvent = new BeforeAfterEvent();
                  badEvent.Load(reader);
                  badEvents.Add(badEvent);
               }
            }
         }
         return badEvents;      
      }

      public static EventRecord[] SelectAuditEvents(int count, int lowWatermark, int highWatermark, string serverDbName, string connectionString)
      {
         if(count < 1)
            throw new ArgumentException("count", "count must be greater than zero.") ;
         if(serverDbName == null)
            throw new ArgumentNullException("serverDbName") ;
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return SelectAuditEvents(count, lowWatermark, highWatermark, serverDbName, connection);
         }
      }

      public static EventRecord[] SelectAuditEvents(int count, int lowWatermark, int highWatermark, string serverDbName, SqlConnection connection)      {
         if(serverDbName == null)
            throw new ArgumentNullException("serverDbName") ;
         ValidateConnection(connection) ;

         ArrayList records = new ArrayList() ;

         string cmdStr = String.Format(SELECT_AuditEvents, count, SQLHelpers.CreateSafeDatabaseName(serverDbName),
            CoreConstants.RepositoryEventsTable, lowWatermark, highWatermark);
         using(SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using(SqlDataReader reader = command.ExecuteReader())
            {
               EventRecord record ;
               int column ;

               while(reader.Read())
               {
                  record = new EventRecord() ;
                  column = 0 ;

                  if(!reader.IsDBNull(column))
                     record.startTime = reader.GetDateTime(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.checksum = reader.GetInt32(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.eventId = reader.GetInt32(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.eventType = (TraceEventType)reader.GetInt32(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.eventClass = reader.GetInt32(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.eventSubclass = reader.GetInt32(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.spid = reader.GetInt32(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.applicationName = reader.GetString(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.hostName = reader.GetString(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.serverName = reader.GetString(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.loginName = reader.GetString(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.success = reader.GetInt32(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.databaseName = reader.GetString(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.databaseId = reader.GetInt32(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.dbUserName = reader.GetString(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.objectType = reader.GetInt32(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.objectName = reader.GetString(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.objectId = reader.GetInt32(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.permissions = reader.GetInt32(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.columnPermissions = reader.GetInt32(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.targetLoginName = reader.GetString(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.targetUserName = reader.GetString(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.roleName = reader.GetString(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.ownerName = reader.GetString(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.targetObject = reader.GetString(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.details = reader.GetString(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.eventCategory = (TraceEventCategory)reader.GetInt32(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.hash = reader.GetInt32(column++) ;
                  else
                     column++ ;
                  if(!reader.IsDBNull(column))
                     record.alertLevel = reader.GetInt32(column++) ;
                  else
                     column++ ;
                  if (!reader.IsDBNull(column))
                  {
                      record.privilegedUser = reader.GetInt32(column++);
                      if (record.privilegedUser != 0)
                      record.privilegedUsers = record.loginName;
                  }
                  if (!reader.IsDBNull(column))
                  {
                      record.rowcount = reader.GetInt64(column++);
                  }
                  else
                     column++ ;
                  records.Add(record) ;
               }
            }
         }
         EventRecord[] retVal = new EventRecord[records.Count] ;
         for(int i = 0 ; i < records.Count ; i++)
            retVal[i] = (EventRecord)records[i] ;

         return retVal ;      
      }

      #endregion // Audit Events
	}
}
