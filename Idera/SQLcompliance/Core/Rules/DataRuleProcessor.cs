using System;
using System.Collections ;
using System.Collections.Generic;
using System.Data ;
using Idera.SQLcompliance.Core.Event ;
using Idera.SQLcompliance.Core.Rules.Alerts ;
using Idera.SQLcompliance.Core.TraceProcessing ;
using System.Data.SqlClient;

namespace Idera.SQLcompliance.Core.Rules
{
   public class DataRuleProcessor
   {
      private string _name;
      private string _instance;
      private List<DataAlertRule> _dataRules;

      public DataRuleProcessor(string sName, string targetInstance, List<DataAlertRule> dataRules)
      {
         _name = String.Format("{0} for {1}", sName, targetInstance);
         _instance = targetInstance;
         _dataRules = dataRules;
      }

      public List<DataAlert> GenerateSensitiveColumnAlerts(string sourceInstance, List<SensitiveColumnEvent> scEvents, string serverDbName,  string connectionString)
      {
         List<DataAlert> dataAlerts = new List<DataAlert>();
         List<int> eventIds = new List<int>();

         foreach (SensitiveColumnEvent scEvent in scEvents)
         {
            foreach (DataAlertRule dataRule in _dataRules)
            {
               bool bConditionMatches = true;
               int conditionIndex = 0;
               EventCondition[] eventConditions;

               if (!dataRule.Enabled)
                  continue;

               if (!dataRule.IsRuleValid)
                  continue;

               if (dataRule.DataType == DataRuleType.SensitiveColumn || dataRule.DataType == DataRuleType.SensitiveColumnViaDataset) //SQLCM -5470 v5.6
               {

                   if (dataRule.TargetsInstance(_instance))
                   {
                       bConditionMatches = dataRule.MatchesDatabase(scEvent.dataseName);

                       if (bConditionMatches)
                           bConditionMatches = dataRule.MatchesTable(scEvent.objectName);

                       if (bConditionMatches)
                       {
                           if (dataRule.DataType == DataRuleType.SensitiveColumn)  //SQLCM -5470 v5.6
                               bConditionMatches = dataRule.MatchesColumn(scEvent.columnName);
                           else
                           {
                               bConditionMatches = dataRule.MatchesDataSetColumn(scEvent.columnName);  //SQLCM -5470 v5.6
                           }
                       }
                       if (bConditionMatches)
                       {
                           eventConditions = dataRule.Conditions;
                           while (conditionIndex < eventConditions.Length && bConditionMatches)
                           {
                               if (eventConditions[conditionIndex].FieldId == (int)AlertableEventFields.rowCount && !String.IsNullOrEmpty(eventConditions[conditionIndex].IntegerTimeFrame))
                               {
                                   if (scEvent.rowCounts > 0 || (eventConditions[conditionIndex].CbOprtr == "<" || eventConditions[conditionIndex].CbOprtr == "<=") || (eventConditions[conditionIndex].CbOprtr == ">=" && eventConditions[conditionIndex].IntegerRowcount == "0") || (eventConditions[conditionIndex].CbOprtr == "=" && eventConditions[conditionIndex].IntegerRowcount == "0"))
                                       bConditionMatches = CheckScRowCountForMatch(scEvent, dataRule, conditionIndex, connectionString, serverDbName);
                                   else
                                       bConditionMatches = false;
                               }
                               else
                               {
                                   bConditionMatches = CheckForMatch(eventConditions[conditionIndex], scEvent);
                               }
                               conditionIndex++;
                           }
                       }
                       if (bConditionMatches)
                       {
                           if (eventIds.BinarySearch(scEvent.eventId) < 0)
                           {
                               dataAlerts.Add(GenerateAlert(dataRule, (ColumnLevelEvent)scEvent));
                               eventIds.Add(scEvent.eventId);
                           }
                       }
                   }
               }
            }
         }
         return dataAlerts;
      }

      private bool CheckScRowCountForMatch(SensitiveColumnEvent scEvent, DataAlertRule dataRule, int conditionIndex, string connectionString, string serverDbName)
      {
          if (serverDbName == null)
              throw new ArgumentNullException("serverDbName");
          if (connectionString == null)
              throw new ArgumentNullException("connectionString");

          using (SqlConnection connection = new SqlConnection(connectionString))
          {
              connection.Open();
              return CheckScRowCountForMatch(scEvent, dataRule, conditionIndex, connection, serverDbName);
          }
      }

      private bool CheckScRowCountForMatch(SensitiveColumnEvent scEvent, DataAlertRule dataRule, int conditionIndex, SqlConnection connection, string serverDbName)
      {
          DateTime startTime = scEvent.startTime;
          EventCondition[] eventConditions = dataRule.Conditions;
          startTime = startTime.AddHours(-Double.Parse(eventConditions[conditionIndex].IntegerTimeFrame));
          ValidateConnection(connection);
          string cmdStr = "SELECT SUM(rc.rowCounts) FROM (SELECT DISTINCT e.eventId, e.rowCounts FROM {0}..{1} e join {0}..{2} sc on e.eventId = sc.eventId join {0}..{3} t on t.id = sc.tableId  WHERE e.startTime >= '{4}' AND e.eventId <= {5} {6}) as rc";
          string whereClause = ParseConditions(eventConditions, eventConditions[conditionIndex].RuleId, "e");
          
          if (dataRule.Instance != "<ALL>")
          {
              whereClause = String.Format(whereClause + " AND e.serverName = '{0}'", dataRule.Instance);
          }
          if (dataRule.Database != "<ALL>")
          {
              whereClause = String.Format(whereClause + " AND e.databaseName = '{0}'", dataRule.Database);
          }
          if (dataRule.Table != "<ALL>")
          {
              whereClause = String.Format(whereClause + " AND t.name = '{0}'", dataRule.Table);
          }
          if (dataRule.Column != "<ALL>")
          {
              whereClause = String.Format(whereClause + " AND sc.columnName = '{0}'", dataRule.Column);
          }
          
          cmdStr = String.Format(cmdStr, 
                                 SQLHelpers.CreateSafeDatabaseName(serverDbName), 
                                 CoreConstants.RepositoryEventsTable, 
                                 CoreConstants.RepositorySensitiveColumnsTable, 
                                 CoreConstants.RepositoryTablesTable, 
                                 startTime, 
                                 scEvent.eventId,
                                 whereClause);
     
          using (SqlCommand command = new SqlCommand(cmdStr, connection))
          {
              using (SqlDataReader reader = command.ExecuteReader())
              {
                  while (reader.Read())
                  {
                      if (reader.GetValue(0) != DBNull.Value)
                          return CheckForMatch(eventConditions[conditionIndex], (object)reader.GetValue(0));
                  }
              }
          }
          return false;
      }
      private bool CheckBadRowCountForMatch(BeforeAfterEvent badEvent, DataAlertRule dataRule, int conditionIndex, string connectionString, string serverDbName)
      {
          if (serverDbName == null)
              throw new ArgumentNullException("serverDbName");
          if (connectionString == null)
              throw new ArgumentNullException("connectionString");

          using (SqlConnection connection = new SqlConnection(connectionString))
          {
              connection.Open();
              return CheckBadRowCountForMatch(badEvent, dataRule, conditionIndex, connection, serverDbName);
          }
       }

      private bool CheckBadRowCountForMatch(BeforeAfterEvent badEvent, DataAlertRule dataRule, int conditionIndex, SqlConnection connection, string serverDbName)
      {
          DateTime startTime = badEvent.startTime;
          EventCondition[] eventConditions = dataRule.Conditions;
          startTime = startTime.AddHours(-Double.Parse(eventConditions[conditionIndex].IntegerTimeFrame));
          ValidateConnection(connection);
          string cmdStr = "SELECT SUM(rc.rowCounts) FROM (SELECT DISTINCT e.eventId, e.rowCounts FROM {0}..{1} dc join {0}..{2} cc on dc.dcId = cc.dcId join {0}..{3} e on dc.eventId = e.eventId WHERE e.startTime >= '{4}' AND e.eventId <= {5} {6}) as rc";
          string whereClause = ParseConditions(eventConditions, eventConditions[conditionIndex].RuleId, "e");

          if (dataRule.Instance != "<ALL>")
          {
              whereClause = String.Format(whereClause + " AND e.serverName = '{0}'", dataRule.Instance);
          }
          if (dataRule.Database != "<ALL>")
          {
              whereClause = String.Format(whereClause + " AND e.databaseName = '{0}'", dataRule.Database);
          }
          if (dataRule.Table != "<ALL>")
          {
              whereClause = String.Format(whereClause + " AND dc.tableName = '{0}'", dataRule.Table);
          }
          if (dataRule.Column != "<ALL>")
          {
              whereClause = String.Format(whereClause + " AND cc.columnName = '{0}'", dataRule.Column);
          }
                    
          cmdStr = String.Format(cmdStr,
                                 SQLHelpers.CreateSafeDatabaseName(serverDbName),
                                 CoreConstants.RepositoryDataChangesTable,
                                 CoreConstants.RepositoryColumnChangesTable,
                                 CoreConstants.RepositoryEventsTable,
                                 startTime,
                                 badEvent.eventId,
                                 whereClause);
         
          //cmdStr = String.Format("SELECT SUM(rc.rowCounts) FROM ({0}) as rc", cmdStr); 

          using (SqlCommand command = new SqlCommand(cmdStr, connection))
          {
              using (SqlDataReader reader = command.ExecuteReader())
              {
                  while (reader.Read())
                  {
                      if (reader.GetValue(0) != DBNull.Value)
                          return CheckForMatch(eventConditions[conditionIndex], (object)reader.GetValue(0));
                  }
              }
          }
          return false;
      }

      private bool CheckForMatch(EventCondition condition, SensitiveColumnEvent scEvent)
      { 
          bool retVal = true;
          if(condition.FieldId == (int)AlertableEventFields.dataRuleLoginName)
          {
              retVal = CheckForMatch(condition, scEvent.loginName);
          }
          else if(condition.FieldId == (int)AlertableEventFields.dataRuleApplicationName)
          {
              retVal = CheckForMatch(condition, scEvent.applicationName);
          }
          else if (condition.FieldId == (int)AlertableEventFields.rowCount && String.IsNullOrEmpty(condition.IntegerTimeFrame))
          {
              retVal = CheckForMatch(condition, scEvent.rowCounts);
          }
          return retVal;
      }

      private bool CheckForMatch(EventCondition condition,  BeforeAfterEvent badEvent)
      {
          bool retVal = true;
          if (condition.FieldId == (int)AlertableEventFields.dataRuleLoginName)
          {
              retVal = CheckForMatch(condition, badEvent.loginName);
          }
          else if (condition.FieldId == (int)AlertableEventFields.dataRuleApplicationName)
          {
              retVal = CheckForMatch(condition, badEvent.applicationName);
          }
          else if (condition.FieldId == (int)AlertableEventFields.rowCount && String.IsNullOrEmpty(condition.IntegerTimeFrame))
          {
              retVal = CheckForMatch(condition, badEvent.rowCounts);
          }
          return retVal;
      }
      private bool CheckForMatch(EventCondition condition, object o)
      {
          bool retVal = false;

          if (condition.FieldId == (int)AlertableEventFields.rowCount)
          {
              if (!(o is Int64))
                  retVal = false;
              else
                  retVal = condition.MatchRowCount((Int64)o);
          }
          else
              switch (condition.TargetEventField.DataFormat)
              {
                  case MatchType.String:
                      if (!(o is String))
                          retVal = false;
                      else
                          retVal = condition.Matches(o as String);
                      break;
                  case MatchType.Bool:
                      if (!(o is Boolean))
                          retVal = false;
                      else
                          retVal = condition.Matches((Boolean)o);
                      break;
                  case MatchType.Integer:
                      if (!(o is Int32))
                          retVal = false;
                      else
                          retVal = condition.Matches((Int32)o);
                      break;
              }
          return retVal;
      }

      private string ParseConditions(EventCondition[] eventConditions, int ruleId, string alias)
      {
          string whereClauseString = "";
          int conditionIndex = 0;
          while (conditionIndex < eventConditions.Length)
          {
              EventCondition eventCondition = eventConditions[conditionIndex];

              string columnName = alias+"."+eventCondition.TargetEventField.ColumnName;
              
              if (eventCondition.RuleId == ruleId
                  &&
              eventCondition.FieldId != (int)AlertableEventFields.rowCount
                  &&
              eventCondition.FieldId != (int)AlertableEventFields.eventCategory)
              {
                  switch (eventCondition.TargetEventField.DataFormat)
                  {
                      case MatchType.String:
                          string[] condtionValues = eventCondition.StringArray;
                          if (condtionValues.Length > 0)
                              whereClauseString += " AND ";
                          if (condtionValues.Length == 1)
                          {
                              string condtionValue = eventCondition.StringArray[0];
                              if (condtionValue.Contains("*") || condtionValue.Contains("?") || condtionValue.Contains("#"))
                              {
                                  if (eventCondition.Inclusive)
                                      whereClauseString = String.Format(whereClauseString + " " + columnName + " {0}", GenarateWildCardsString(condtionValue));
                                  else
                                      whereClauseString = String.Format(whereClauseString + " " + columnName + " NOT {0}", GenarateWildCardsString(condtionValue));
                              }
                              else
                              {
                                  if (eventCondition.Inclusive)
                                  {
                                      whereClauseString = String.Format(whereClauseString + " " + columnName + " = '{0}'", eventCondition.StringArray[0]);
                                  }
                                  else
                                  {
                                      whereClauseString = String.Format(whereClauseString + " " + columnName + " <> '{0}'", eventCondition.StringArray[0]);
                                  }
                              }
                          }
                          else if (condtionValues.Length > 1)
                          {
                              string values = "";
                              string wildcardFilters = "";
                              whereClauseString += " (";
                              foreach (string value in condtionValues)
                              {
                                  if (value.Contains("*") || value.Contains("?") || value.Contains("#"))
                                  {
                                      if (!String.IsNullOrEmpty(wildcardFilters))
                                      {
                                          if (eventCondition.Inclusive)
                                              wildcardFilters += " OR ";
                                          else
                                              wildcardFilters += " AND ";
                                      }
                                          wildcardFilters += " " + columnName;
                                          if (!eventCondition.Inclusive)
                                              wildcardFilters += " NOT ";
                                          wildcardFilters += GenarateWildCardsString(value);
                                  }
                                  else
                                  {
                                      if (!String.IsNullOrEmpty(values))
                                      {
                                          values += ", ";
                                      }
                                      values = values + "'" + value + "'";

                                  }
                              }
                              if (!String.IsNullOrEmpty(values))
                              {
                                  whereClauseString += " " + columnName;
                                  if (eventCondition.Inclusive)
                                  {
                                      whereClauseString = String.Format(whereClauseString + " IN ({0})", values);
                                  }
                                  else
                                  {
                                      whereClauseString = String.Format(whereClauseString + " NOT IN ({0})", values);
                                  }
                              }
                              if (!String.IsNullOrEmpty(values) && !String.IsNullOrEmpty(wildcardFilters))
                              {                                  
                                  if (eventCondition.Inclusive)
                                      wildcardFilters += " OR ";
                                  else
                                      wildcardFilters += " AND ";
                              }
                              if (!String.IsNullOrEmpty(wildcardFilters))
                              {
                                  whereClauseString += wildcardFilters;
                              }
                              whereClauseString += ") ";
                          }
                          break;
                      case MatchType.Bool:
                          whereClauseString += " AND ";
                          if (eventCondition.BooleanValue)
                          {
                              whereClauseString = whereClauseString + columnName + " = 1";
                          }
                          else
                          {
                              whereClauseString = whereClauseString + columnName + " = 0";
                          }
                          break;
                      case MatchType.Integer:
                          if (eventCondition.IntegerArray.Length > 0)
                              whereClauseString += " AND ";
                          if (eventCondition.IntegerArray.Length == 1)
                          {
                              if (eventCondition.Inclusive)
                              {
                                  whereClauseString = String.Format(whereClauseString + columnName + " = {0}", eventCondition.IntegerArray[0]);
                              }
                              else
                              {
                                  whereClauseString = String.Format(whereClauseString + columnName + " <> {0}", eventCondition.IntegerArray[0]);
                              }
                          }
                          else if (eventCondition.IntegerArray.Length > 1)
                          {
                              string values = "";
                              foreach (int vaule in eventCondition.IntegerArray)
                              {
                                  if (!String.IsNullOrEmpty(values))
                                  {
                                      values += ",";
                                  }
                                  values = values + vaule;
                              }
                              if (eventCondition.Inclusive)
                              {
                                  whereClauseString = String.Format(whereClauseString + columnName + " IN ({0})", values);
                              }
                              else
                              {
                                  whereClauseString = String.Format(whereClauseString + columnName + " NOT IN ({0})", values);
                              }
                          }
                          break;
                  }
              }
              conditionIndex++;
          }
          return whereClauseString;
      }
      private string GenarateWildCardsString(string rawString)
      {
          string wildcardsFilters = "";
          wildcardsFilters += " LIKE '" + ConvertWildCardsString(rawString) + "'";
          return wildcardsFilters;
      }
      private string ConvertWildCardsString(String rawString)
      {
          string cmdStr = "";
          if (rawString == null)
              throw new ArgumentNullException("rawString");

          for (int i = 0; i < rawString.Length; i++)
          {
              char currentChar, nextChar;
              bool bLastChar;

              currentChar = rawString[i];
              if (rawString.Length - 1 == i)
              {
                  bLastChar = true;
                  nextChar = '\0';
              }
              else
              {
                  bLastChar = false;
                  nextChar = rawString[i + 1];
              }

              switch (currentChar)
              {
                  case '*':
                        if (bLastChar || (nextChar != '?' && nextChar != '#'))
                          cmdStr = cmdStr + "%";
                      else
                      {
                          if (nextChar == '#')
                              cmdStr += "%[0-9]%";
                          else if (nextChar == '?')
                              cmdStr += "%_%";
                          else
                              cmdStr += nextChar;
                          i++;
                      }
                      break;
                  case '?':
                      cmdStr += "_";
                      break;
                  case '#':
                      cmdStr += "[0-9]";
                      break;
                  default:
                      cmdStr = cmdStr + currentChar;
                      break;
              }
          }
          return cmdStr;
      }
      /// <summary>
      /// Verifies that the connection object is not null and that the connection is
      /// open.
      /// </summary>
      /// <param name="connection"></param>
      /// <exception cref="ArgumentNullException">Thrown when connection is null</exception>
      /// <exception cref="Exception">Thrown when the connection is not open.</exception>
      private static void ValidateConnection(SqlConnection connection)
      {
          if (connection == null)
              throw new ArgumentNullException("connection");
          if (connection.State != ConnectionState.Open)
              throw new Exception("SqlConnection object is not opened.");
      }
      public List<DataAlert> GenerateBADAlerts(string sourceInstance, List<BeforeAfterEvent> badEvents, string serverDbName,  string connectionString)
      {

          List<string> columns = new List<string>();
          List<string> beforeValue = new List<string>();
          List<string> afterValue = new List<string>();
          Dictionary<int, DataAlert> alerts = new Dictionary<int, DataAlert>();
          bool flag = true;
          foreach (BeforeAfterEvent badEvent in badEvents)
          {
             foreach (DataAlertRule dataRule in _dataRules)
             {
                bool bConditionMatches = true;
                int conditionIndex = 0;
                EventCondition[] eventConditions;
          
                if (!dataRule.Enabled)
                   continue;
          
                if (!(dataRule.DataType == DataRuleType.ColumnValueChanged 
                     || dataRule.DataType == DataRuleType.ColumnValueChangedBad
                    ))
                   continue;
          
                if (dataRule.TargetsInstance(_instance))
                {
                   bConditionMatches = dataRule.MatchesDatabase(badEvent.dataseName);
          
                   if (bConditionMatches)
                      bConditionMatches = dataRule.MatchesTable(badEvent.objectName);
          
                   if (bConditionMatches)
                      bConditionMatches = dataRule.MatchesColumn(badEvent.columnName);
          
                    if (bConditionMatches)
                    {
                        switch (dataRule.DataType)
                        {
                             case DataRuleType.ColumnValueChangedBad:
                                break;
                             case DataRuleType.ColumnValueChanged:
                                bConditionMatches = MatchDataValue(badEvent.afterValue, dataRule.Comparison);
                                break;
                        }
                    }
                    if (bConditionMatches)
                    {
                        eventConditions = dataRule.Conditions;
                        while (conditionIndex < eventConditions.Length && bConditionMatches)
                        {
                            if (eventConditions[conditionIndex].FieldId == (int)AlertableEventFields.rowCount && !String.IsNullOrEmpty(eventConditions[conditionIndex].IntegerTimeFrame))
                            {
                                bConditionMatches = CheckBadRowCountForMatch(badEvent, dataRule, conditionIndex, connectionString, serverDbName);
                            }
                            else
                            {
                                bConditionMatches = CheckForMatch(eventConditions[conditionIndex], badEvent);
                            }
                            conditionIndex++;
                        }
                    }
                      
                   if (bConditionMatches)
                   {
          
                       if (alerts.ContainsKey(badEvent.eventId))
                       {                         
                           if (!columns.Contains(badEvent.columnName))
                           {
                               ((BeforeAfterEvent)alerts[badEvent.eventId].EventData).beforeValue += "," + badEvent.beforeValue;
                               ((BeforeAfterEvent)alerts[badEvent.eventId].EventData).afterValue += "," + badEvent.afterValue;
                               columns.Add(badEvent.columnName);
                           }
                           else
                           {
                               if (flag)
                               {
                                   ((BeforeAfterEvent)alerts[badEvent.eventId].EventData).columnName = "[" + string.Join(",", columns) + "]";
                                   ((BeforeAfterEvent)alerts[badEvent.eventId].EventData).beforeValue = "[" + ((BeforeAfterEvent)alerts[badEvent.eventId].EventData).beforeValue + "]";
                                   ((BeforeAfterEvent)alerts[badEvent.eventId].EventData).afterValue = "[" + ((BeforeAfterEvent)alerts[badEvent.eventId].EventData).afterValue + "]";
                                   flag = false;
                               }
                               beforeValue.Add(badEvent.beforeValue);
                               afterValue.Add(badEvent.afterValue);
                           }
                           if (beforeValue.Count == columns.Count && !flag)
                           {
                               if (((BeforeAfterEvent)alerts[badEvent.eventId].EventData).beforeValue.EndsWith("]"))
                               {
                                   ((BeforeAfterEvent)alerts[badEvent.eventId].EventData).beforeValue += ",[" + string.Join(",", beforeValue) + "]";
                                   ((BeforeAfterEvent)alerts[badEvent.eventId].EventData).afterValue += ",[" + string.Join(",", afterValue) + "]";
                               }
                    
                               beforeValue.Clear();
                               afterValue.Clear();
                           }
                       }
                       else
                       {
                           alerts[badEvent.eventId] = GenerateAlert(dataRule, badEvent);
                           columns.Add(badEvent.columnName);
                       }                      
                   }
                }
             }
          }
          return new List<DataAlert>(alerts.Values);
      }

      private bool MatchDataValue(string stringValue, DataAlertComparison comparison)
      {
         long value;

         try
         {
            value = Convert.ToInt64(stringValue);
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, String.Format("Unable to convert {0} to a long. Error: {1}", stringValue, e.ToString()));
            return false;
         }

         switch (comparison.Operator)
         {
            case ComparisonOperator.Equal:
               return value == comparison.Value;
            case ComparisonOperator.GreaterThan:
               return value > comparison.Value;
            case ComparisonOperator.GreaterThanEqual:
               return value >= comparison.Value;
            case ComparisonOperator.LessThan:
               return value < comparison.Value;
            case ComparisonOperator.LessThanEqual:
               return value <= comparison.Value;
            case ComparisonOperator.NotEqual:
               return value != comparison.Value;
            default:
               return false;
         };
      }

      private DataAlert GenerateAlert(DataAlertRule rule, ColumnLevelEvent scEvent)
      {
         DataAlert dataAlert = new DataAlert();

         dataAlert.AlertType = EventType.Data;
         dataAlert.ParentRule = rule;
         dataAlert.Instance = _instance;
         dataAlert.EventId = scEvent.eventId;
         dataAlert.Level = rule.Level;
         dataAlert.EventType = rule.DataType;
         dataAlert.Comparison = rule.Comparison;
         dataAlert.EventData = scEvent;
         dataAlert.MessageData = rule.MessageData;
         if (rule.HasLogAction)
            dataAlert.LogStatus = NotificationStatus.Pending;
         if (rule.HasEmailAction)
            dataAlert.EmailStatus = NotificationStatus.Pending;
         if (rule.HasSnmpTrapAction)
             dataAlert.SnmpTrapStatus = NotificationStatus.Pending;
         dataAlert.RuleName = rule.Name;
         dataAlert.ParentRuleId = rule.Id;
         dataAlert.DataRuleTypeName = rule.DataRuleTypeName;
         dataAlert.AlertEventType = scEvent.alertEventType;

         ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                  String.Format("A data alert is generated for instance {0}:\n" +
                                                 "Rule: {1}\n" +
                                                 "Type: {2}\n" +
                                                 "Event ID: {3}\n" +
                                                 "Event time{4}\n",
                                                 _instance,
                                                 rule.Name,
                                                 rule.DataRuleTypeName,
                                                 scEvent.eventId,
                                                 scEvent.startTime));

         return dataAlert;
      }
   }
}
