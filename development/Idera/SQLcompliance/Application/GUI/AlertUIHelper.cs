using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Rules;
using Idera.SQLcompliance.Core.Rules.Alerts ;
using System.Reflection;
using System.ComponentModel;

namespace Idera.SQLcompliance.Application.GUI
{
	/// <summary>
	/// Summary description for AlertUIHelper.
	/// </summary>
	public class AlertUIHelper
	{
		#region Member Variables

		#endregion

		#region Properties

		#endregion

		#region Construction/Destruction

      private AlertUIHelper()
      {
         // This is just a static member helper class
      }
		#endregion

      public static EventCondition ExtractVerbCondition(AlertRule rule)
      {
         if (rule == null)
            throw new ArgumentNullException("rule") ; 

         foreach(EventCondition condition in rule.Conditions)
         {
            if(condition.FieldId == (int)AlertableEventFields.eventType && condition.IsValid && condition.Inclusive)
            {
               return condition ;
            }
            else if(condition.FieldId == (int)AlertableEventFields.eventCategory && condition.IsValid)
            {
               return condition ;
            }         
         }
         return null ;
      }

      private static string CreateColumnString(DataAlertRule rule, out LinkString link)
      {
         string column;
         string columnString;
         link = new LinkString();

         if (!String.IsNullOrEmpty(rule.Column))
         {
            if (String.Equals("<ALL>", rule.Column))
            {
               columnString = " on any column ";
               link.Append(" on any column ");
            }
            else
            {
               column = RtfEscape(rule.Column);
               columnString = String.Format(@" on column \ul\cf1 {0}\cf0\ulnone  ", column);
               link.Append(" on column ");
               link.AppendLink(column, "Column");
               link.Append("  ");
            }
         }
         else
         {
            column = "specified column";
            columnString = String.Format(@"in \ul\cf2 {0}\cf0\ulnone  ", column);
            link.Append("on ");
            link.AppendLink(column, "Column");
            link.Append("  ");
         }
         return columnString;
      }

      private static string CreateTableString(DataAlertRule rule, out LinkString link)
      {
         string table;
         string tableString;
         link = new LinkString();

         if (!String.IsNullOrEmpty(rule.FullTableName))
         {
            if (String.Equals("<ALL>", rule.FullTableName))
            {
               tableString = " in any table ";
               link.Append(" in any table ");
            }
            else
            {
               table = RtfEscape(rule.FullTableName);
               tableString = String.Format(@"in table \ul\cf1 {0}\cf0\ulnone  \line  ", table);
               link.Append("in table ");
               link.AppendLink(table, "Table");
               link.Append("  ");
            }
         }
         else
         {
            table = "specified table";
            tableString = String.Format(@"in a \ul\cf2 {0}\cf0\ulnone  \line  ", table);
            link.Append("in a ");
            link.AppendLink(table, "Table");
            link.Append("  ");
         }
         return tableString;
      }

      private static string CreateDatabaseString(DataAlertRule rule, out LinkString link)
      {
         string database;
         string databaseString;
         link = new LinkString();

         if (!String.IsNullOrEmpty(rule.Database))
         {
            if (String.Equals("<ALL>", rule.Database))
            {
               databaseString = " in any database ";
               link.Append(" in any database ");
            }
            else
            {
               database = RtfEscape(rule.Database);
               databaseString = string.Format(@"in database \ul\cf1 {0}\cf0\ulnone  ", database);
               link.Append("in database ");
               link.AppendLink(database, "Database");
               link.Append("  ");
            }
         }
         else
         {
            database = "specified database";
            databaseString = string.Format(@"in a \ul\cf2 {0}\cf0\ulnone  ", database);
            link.Append("in a ");
            link.AppendLink(database, "Database");
            link.Append("  ");
         }
         return databaseString;
      }

      private static string CreateInstanceString(DataAlertRule rule, out LinkString link)
      {
         string instanceString;
         string instance;
         link = new LinkString();

         if (!String.IsNullOrEmpty(rule.Instance))
         {
            if (String.Equals("<ALL>", rule.Instance))
            {
               instanceString = String.Format("on any SQL Server");
               link.Append("on any SQL Server");
            }
            else
            {
               instance = RtfEscape(rule.Instance);
               instanceString = String.Format(@"in instance \ul\cf1 {0}\cf0\ulnone    ", instance);
               link.Append("in instance ");
               link.AppendLink(instance, "Instance");
            }
         }
         else
         {
            instance = "specified instance";
            instanceString = String.Format(@"in a \ul\cf2 {0}\cf0\ulnone  \line  ", instance);
            link.Append("in a ");
            link.AppendLink(instance, "Instance");
            link.Append("  ");
         }
         return instanceString;
      }

      public static string GenerateRuleDescription(DataAlertRule rule, out LinkString link)
      {
         StringBuilder builderCodes = new StringBuilder(@"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 Microsoft Sans Serif;}}{\colortbl ;\red0\green0\blue255;\red255\green0\blue0;\red0\green128\blue0;}\viewkind4\uc1\pard\f0\fs17 ");

         link = new LinkString();

         builderCodes.AppendFormat(@" Generate a \ul\cf1 {0}\cf0\ulnone  alert\line  ", RtfEscape(rule.Level.ToString()));
         link.Append(" Generate a ");
         link.AppendLink(rule.Level.ToString(), "AlertLevel");
         link.Append(" alert");
         link.Append("   ");

         switch (rule.DataType)
         {
            case DataRuleType.SensitiveColumn:
               builderCodes.AppendFormat(@"when a \b Sensitive Column is accessed \b0 \line  ");
               link.Append("when a Sensitive Column is accessed  ");
               break;
            //SQLCM -5470 v5.6
             case DataRuleType.SensitiveColumnViaDataset:
                 builderCodes.AppendFormat(@"when a \b Sensitive Column Via Dataset is accessed \b0 \line  ");
                 link.Append("when a Sensitive Column Via Dataset is accessed  ");
               break;
            case DataRuleType.ColumnValueChanged:
               builderCodes.AppendFormat(@"when \b the value of a numeric column \b0 \line  ");
               link.Append("when the value of a numeric column  ");

               string comparison = String.Format("{0} {1} ", GetEnumDescription(rule.Comparison.Operator), rule.Comparison.Value.ToString());
               builderCodes.AppendFormat(@"is \ul\cf1 {0}\cf0\ulnone  \line  ", RtfEscape(comparison));
               link.Append("is ");
               link.AppendLink(comparison, "Comparison");
               link.Append("  ");
               break;

            case DataRuleType.ColumnValueChangedBad:
                 builderCodes.AppendFormat(@"when \b the value of a column changes \b0 \line  ");
                 link.Append("when the value of a column changes  ");
                 break;
         }

         bool addedColumn = false;
         bool addedTable = false;
         bool addedDatabase = false;
         string columnString;
         string tableString;
         string databaseString;
         string instanceString;
         LinkString columnLink;
         LinkString tableLink;
         LinkString databaseLink;
         LinkString instanceLink;

         columnString = CreateColumnString(rule, out columnLink);
         tableString = CreateTableString(rule, out tableLink);
         databaseString = CreateDatabaseString(rule, out databaseLink);
         instanceString = CreateInstanceString(rule, out instanceLink);

         //add the column
         if (!String.Equals("<ALL>", rule.Column))
         {
            builderCodes.Append(columnString);
            AppendLink(columnLink, link);
            addedColumn = true;
         }

         //add the table
         if (!String.Equals("<ALL>", rule.Table))
         {
            if (!addedColumn)
            {
               builderCodes.Append(columnString);
               AppendLink(columnLink, link);
            }
            builderCodes.Append(tableString);
            AppendLink(tableLink, link);
            addedTable = true;
         }

         //add the database
         if (!String.Equals("<ALL>", rule.Database))
         {
            if (!addedTable)
            {
               builderCodes.Append(tableString);
               AppendLink(tableLink, link);
            }
            builderCodes.Append(databaseString);
            AppendLink(databaseLink, link);
            addedDatabase = true;
         }

         //add the instance
         if (!addedDatabase && !String.Equals("<ALL>", rule.Instance))
         {
            builderCodes.Append(databaseString);
            AppendLink(databaseLink, link);
         }
         builderCodes.Append(instanceString);
         AppendLink(instanceLink, link);
         GenerateFiltersString(rule, builderCodes, link);
         GenerateActionString(rule, builderCodes, link);

         // This helps with selection of the last character
         builderCodes.Append("  ");
         link.Append("  ");

         builderCodes.Append(@"\par}");
         return builderCodes.ToString();
      }

      private static void AppendLink(LinkString linkInserted, LinkString link)
      {
         foreach (LinkSegment segment in linkInserted.Segments)
            link.Append(segment);
      }

      private static void GenerateFiltersString(DataAlertRule rule, StringBuilder builderCodes, LinkString link)
      {
          bool hasWhere = false;
          string tempString;
          StringBuilder tempBuilder = new StringBuilder(128);
          foreach (EventCondition condition in rule.Conditions)
          {
              // Skipping these conditions as these field Ids are reserved for Data alert rule
              if (condition.FieldId == (int)AlertableEventFields.eventCategory 
                  || condition.FieldId == (int)AlertableEventFields.applicationName 
                  || condition.FieldId == (int)AlertableEventFields.loginName
                  || condition.FieldId == (int)AlertableEventFields.success)
                  continue;
              if (!hasWhere)
              {
                  builderCodes.Append(@"\line   where ");
                  link.Append(@"   where ");
                  hasWhere = true;
              }
              else
              {
                  builderCodes.Append(@"\line   and ");
                  link.Append("   and ");
              }

              switch (condition.ConditionType)
              {
                  case MatchType.String:
                      if (condition.FieldId == (int)AlertableEventFields.rowCount)
                      {
                          tempString = "";
                          builderCodes.Append("Row Count is ");
                          link.Append("Row Count is ");

                          tempString = condition.CbOprtr + " " + condition.IntegerRowcount;
                          string tempString_TimeFrame = condition.IntegerTimeFrame;


                          if (string.IsNullOrEmpty(condition.IntegerRowcount))
                          {
                              builderCodes.Append(@"\ul\cf2Specify Row Count Threshold\cf0\ulnone");
                              link.AppendLink("Specify Row Count Threshold", condition);

                          }


                          else
                          {
                              tempString = link.AppendLink(tempString, condition);
                              builderCodes.AppendFormat(@"\ul\cf1 {0}\cf0\ulnone ", RtfEscape(tempString));
                              if (!string.IsNullOrEmpty(condition.IntegerTimeFrame))
                              {
                                  builderCodes.Append(" over the time period of ");
                                  link.Append(" over the time period of ");
                                  builderCodes.AppendFormat(@"\ul\cf1 {0}\cf0\ulnone ", RtfEscape(tempString_TimeFrame));
                                  link.AppendLink(tempString_TimeFrame, condition);
                                  builderCodes.Append(" Hours ");
                                  link.Append(" Hours ");
                              }
                          }
                          break;
                      }
                      if (condition.Inclusive)
                      {
                          builderCodes.AppendFormat("{0} like ", RtfEscape(condition.TargetEventField.DisplayName));
                          link.Append(String.Format("{0} like ", condition.TargetEventField.DisplayName));
                      }
                      else
                      {
                          builderCodes.AppendFormat("{0} not like ", RtfEscape(condition.TargetEventField.DisplayName));
                          link.Append(String.Format("{0} not like ", condition.TargetEventField.DisplayName));
                      }
                      if (condition.IsValid)
                      {
                          tempBuilder.Remove(0, tempBuilder.Length);
                          for (int i = 0; i < condition.StringArray.Length; i++)
                          {
                              tempBuilder.AppendFormat("'{0}'", condition.StringArray[i]);
                              if (i < condition.StringArray.Length - 1)
                              {
                                  if (condition.Inclusive)
                                      tempBuilder.Append(" or ");
                                  else
                                      tempBuilder.Append(" and ");
                              }

                          }
                          tempString = link.AppendLink(tempBuilder.ToString(), condition);
                          builderCodes.AppendFormat(@"\ul\cf1 {0}\cf0\ulnone ", RtfEscape(tempString));
                      }
                      else
                      {
                          builderCodes.Append(@"\ul\cf2specified words\cf0\ulnone ");
                          link.AppendLink("specified words", condition);
                      }
                      break;
              }
          }
      }

      /// <summary>
      /// 
      /// </summary>
      private static void GenerateActionString(DataAlertRule rule, StringBuilder builderCodes, LinkString link)
      {
         if (rule.HasEmailAction || rule.HasLogAction || rule.HasSnmpTrapAction)
         {
            builderCodes.Append(@"\line Send an \ul\cf1alert message\cf0\ulnone  to\line   ");
            link.Append(" Send an ");
            link.AppendLink("alert message", "AlertMessage");
            link.Append(" to   ");
         }

         if (rule.HasEmailAction)
         {
            if (rule.Recipients.Length == 0)
            {
               builderCodes.Append(@"\ul\cf2specified addresses\cf0\ulnone");
               link.AppendLink("specified addresses", "Recipients");
            }
            else
            {
               StringBuilder builder = new StringBuilder("");

               for (int i = 0; i < rule.Recipients.Length; i++)
               {
                  builder.Append(rule.Recipients[i]);
                  if (i != rule.Recipients.Length - 1)
                     builder.Append(", ");
               }
               string realString = link.AppendLink(builder.ToString(), "Recipients");
               builderCodes.AppendFormat(@"\ul\cf1 {0}\cf0\ulnone", RtfEscape(realString));
            }
            if (rule.HasLogAction || rule.HasSnmpTrapAction)
            {
               builderCodes.Append(@"\line   and ");
               link.Append("   and ");
            }
         }

         if (rule.HasLogAction)
         {
            if (!rule.HasLogAction)
               builderCodes.Append(@"\line   ");
            EventLogEntryType evType = EventLogEntryType.Error;

            switch (rule.LogEntryType)
            {
               case ErrorLog.Severity.Informational:
                  {
                     evType = EventLogEntryType.Information;
                     break;
                  }
               case ErrorLog.Severity.Warning:
                  {
                     evType = EventLogEntryType.Warning;
                     break;
                  }
               case ErrorLog.Severity.Error:
                  {
                     evType = EventLogEntryType.Error;
                     break;
                  }
            }
            builderCodes.AppendFormat(@"Windows Event Log entry of type \ul\cf1 {0}\cf0\ulnone ", RtfEscape(evType.ToString()));
            link.Append("Windows Event Log entry of type ");
            link.AppendLink(evType.ToString(), "LogEntryType");

            if (rule.HasSnmpTrapAction)
            {
                builderCodes.Append(@"\line   and ");
                link.Append("   and ");
            }
         }

         if (rule.HasSnmpTrapAction)
         {
             builderCodes.Append(@"specified \ul\cf1network management console\cf0\ulnone");
             link.Append("specified ");
             link.AppendLink("network management console", "SnmpConfigurations");
         }
      }

      public static string GetEnumDescription(Enum value)
      {
         FieldInfo fi = value.GetType().GetField(value.ToString());

         DescriptionAttribute[] attributes =
             (DescriptionAttribute[])fi.GetCustomAttributes(
             typeof(DescriptionAttribute),
             false);

         if (attributes != null &&
             attributes.Length > 0)
            return attributes[0].Description;
         else
            return value.ToString();
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="ruleType"></param>
      /// <returns></returns>
      public static string GenerateRuleDescription(StatusAlertRule rule, out LinkString link)
      {
         StringBuilder builderCodes = new StringBuilder(@"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 Microsoft Sans Serif;}}{\colortbl ;\red0\green0\blue255;\red255\green0\blue0;\red0\green128\blue0;}\viewkind4\uc1\pard\f0\fs17 ");

         link = new LinkString(80);

         builderCodes.AppendFormat(@" Generate a \ul\cf1 {0}\cf0\ulnone  alert\line  ", RtfEscape(rule.Level.ToString()));
         link.Append(" Generate a ");
         link.AppendLink(rule.Level.ToString(), "AlertLevel");
         link.Append(" alert");
         link.Append("   ");

         string lineTwo = "";
         string firstPart = "";
         string middlePart = "";
         string lastPart = "";
         bool hasThreshold = false;

         switch (rule.StatusType)
         {
            case StatusRuleType.NoHeartbeats:
               lineTwo = "Collection Server";
               firstPart = "indicates no heartbeat has been received from the SQLcompliance Agent";
               break;
            case StatusRuleType.SqlServerDown:
               lineTwo = "Agent";
               firstPart = "indicates the SQLcompliance Agent cannot connect to a SQL Server";
               break;
            case StatusRuleType.RepositoryTooBig:
               lineTwo = "event database";
               firstPart = "meets or exceeds the event database size limit of ";
               middlePart = String.Format("{0} GB ", rule.Threshold.Value.ToString());
               hasThreshold = true;
               break;
            case StatusRuleType.TraceDirFullAgent:
               lineTwo = "Agent trace directory";
               firstPart = "meets or exceeds";
               middlePart = String.Format("{0}% ", rule.Threshold.Value.ToString());
               lastPart = "of the trace directory size limit";
               hasThreshold = true;
               break;
            case StatusRuleType.TraceDirFullCollect:
               lineTwo = "Collection Server trace directory";
               firstPart = "meets or exceeds the trace directory size limit of";
               middlePart = String.Format("{0} GB ", rule.Threshold.Value.ToString());
               hasThreshold = true;
               break;
         }
         builderCodes.AppendFormat(@"when the \b {0} status \b0 \line  ", RtfEscape(lineTwo));
         link.Append("when the ");
         link.Append(lineTwo);
         link.Append(" status ");
         link.Append("  ");

         if (hasThreshold)
         {
            builderCodes.AppendFormat(@"{0} \ul\cf1 {1}\cf0\ulnone {2} \line  ", RtfEscape(firstPart), RtfEscape(middlePart), RtfEscape(lastPart));
            link.Append(firstPart);
            link.AppendLink(middlePart, "Threshold");
         }
         else
         {
            builderCodes.AppendFormat(@"{0} {1} {2} \line  ", RtfEscape(firstPart), RtfEscape(middlePart), RtfEscape(lastPart));
            link.Append(firstPart);
            link.Append(middlePart);
         }
         link.Append(lastPart);
         link.Append("  ");
         GenerateActionString(rule, builderCodes, link);

         // This helps with selection of the last character
         builderCodes.Append("  ");
         link.Append("  ");

         builderCodes.Append(@"\par}");
         return builderCodes.ToString();
      }

      /// <summary>
      /// 
      /// </summary>
      private static void GenerateActionString(StatusAlertRule rule, StringBuilder builderCodes, LinkString link)
      {
         if (rule.HasEmailAction || rule.HasLogAction || rule.HasSnmpTrapAction)
         {
            builderCodes.Append(@" Send an \ul\cf1alert message\cf0\ulnone  to\line   ");
            link.Append(" Send an ");
            link.AppendLink("alert message", "AlertMessage");
            link.Append(" to   ");
         }

         if (rule.HasEmailAction)
         {
            if (rule.Recipients.Length == 0)
            {
               builderCodes.Append(@"\ul\cf2specified addresses\cf0\ulnone");
               link.AppendLink("specified addresses", "Recipients");
            }
            else
            {
               StringBuilder builder = new StringBuilder("");

               for (int i = 0; i < rule.Recipients.Length; i++)
               {
                  builder.Append(rule.Recipients[i]);
                  if (i != rule.Recipients.Length - 1)
                     builder.Append(", ");
               }
               string realString = link.AppendLink(builder.ToString(), "Recipients");
               builderCodes.AppendFormat(@"\ul\cf1 {0}\cf0\ulnone", RtfEscape(realString));
            }
            if (rule.HasLogAction || rule.HasSnmpTrapAction)
            {
               builderCodes.Append(@"\line   and ");
               link.Append("   and ");
            }
         }

         if (rule.HasLogAction)
         {
            if (!rule.HasLogAction)
               builderCodes.Append(@"\line   ");
            EventLogEntryType evType = EventLogEntryType.Error;

            switch (rule.LogEntryType)
            {
               case ErrorLog.Severity.Informational:
                  {
                     evType = EventLogEntryType.Information;
                     break;
                  }
               case ErrorLog.Severity.Warning:
                  {
                     evType = EventLogEntryType.Warning;
                     break;
                  }
               case ErrorLog.Severity.Error:
                  {
                     evType = EventLogEntryType.Error;
                     break;
                  }
            }
            builderCodes.AppendFormat(@"Windows Event Log entry of type \ul\cf1 {0}\cf0\ulnone ", RtfEscape(evType.ToString()));
            link.Append("Windows Event Log entry of type ");
            link.AppendLink(evType.ToString(), "LogEntryType");

            if (rule.HasSnmpTrapAction)
            {
                builderCodes.Append(@"\line   and ");
                link.Append("   and ");
            }
         }

          if (rule.HasSnmpTrapAction)
          {
              builderCodes.Append(@"specified \ul\cf1network management console\cf0\ulnone");
              link.Append("specified ");
              link.AppendLink("network management console", "SnmpConfigurations");
          }
      } 

      public static string GenerateRuleDescription(AlertRule rule, AlertingConfiguration config, out LinkString link)
      {
         StringBuilder builderCodes = new StringBuilder(@"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 Microsoft Sans Serif;}}{\colortbl ;\red0\green0\blue255;\red255\green0\blue0;\red0\green128\blue0;}\viewkind4\uc1\pard\f0\fs17 ");

         link = new LinkString(80) ;

         builderCodes.AppendFormat(@"Generate a \ul\cf1 {0}\cf0\ulnone  alert\line   ", RtfEscape(rule.Level.ToString())) ;
         link.Append("Generate a ") ;
         link.AppendLink(rule.Level.ToString(), "AlertLevel") ;
         link.Append(" alert") ;
         link.Append("   ") ;

         // Handle the verb condition
         EventCondition verbCondition = null ;
         string verbConditionString = "ALL" ;
         bool verbLinkable = false ;

         foreach(EventCondition condition in rule.Conditions)
         {
            if(verbCondition == null && condition.Inclusive && condition.FieldId == (int)AlertableEventFields.eventType && condition.IsValid)
            {
               verbCondition = condition ;
               CMEventType evType = config.LookupEventType(condition.IntegerArray[0], EventType.SqlServer) ;

               verbConditionString = evType != null ? evType.Name : "N/A" ;
               verbLinkable = true ;
            }
            else if(condition.FieldId == (int)AlertableEventFields.eventCategory)
            {
               verbCondition = condition ;
               if(verbCondition.IntegerArray[0] == 1)
               {
                  verbConditionString = "Login Activity" ;
               }
               else if(verbCondition.IntegerArray[0] == 2)
               {
                  verbConditionString = "Data Definition (DDL)" ;
               }
               else if(verbCondition.IntegerArray[0] == 3)
               {
                  verbConditionString = "Security" ;
               }
               else if(verbCondition.IntegerArray[0] == 4)
               {
                  verbConditionString = "Data Manipulation (DML)" ;
               }
               else if(verbCondition.IntegerArray[0] == 6)
               {
                  verbConditionString = "Administrative" ;
               }
               else if(verbCondition.IntegerArray[0] == 9)
               {
                  verbConditionString = "User Defined" ;
               }
               else
               {
                  verbConditionString = "N/A" ;
               }
            }
         }

         if(verbLinkable)
            builderCodes.AppendFormat(@"for \ul\cf1 {0}\cf0\ulnone  events\line  ", RtfEscape(verbConditionString)) ;
         else
            builderCodes.AppendFormat(@"for \b {0}\b0  events\line  ", RtfEscape(verbConditionString)) ;
         link.Append("for ") ;
         if(verbLinkable)
            link.AppendLink(verbConditionString, verbCondition) ;
         else
            link.Append(verbConditionString) ;
         link.Append(" events") ;
         link.Append("  ") ;

         GenerateScopeString(rule, builderCodes, link) ;
         GenerateFiltersString(rule, builderCodes, link, config) ;
         GenerateActionString(rule, builderCodes, link) ;

         // This helps with selection of the last character (need pointNext)
         builderCodes.Append("  ") ;
         link.Append("  ") ;

			builderCodes.Append(@"\par}") ;
			return builderCodes.ToString() ;
		}

      private static void GenerateScopeString(AlertRule rule, StringBuilder builderCodes, LinkString link)
      {
         EventCondition dbLevel = null, objectLevel = null ;

         foreach(EventCondition condition in rule.Conditions)
         {
            if(condition.FieldId == (int)AlertableEventFields.databaseName)
               dbLevel = condition ;
            else if(condition.FieldId == (int)AlertableEventFields.objectName)
               objectLevel = condition ;
         }

         builderCodes.Append(" on ") ;
         link.Append(" on ") ;

         // If null, we match any instance (ALL)
         if(rule.TargetInstances.Length == 1  && String.Equals("<ALL>", rule.TargetInstances[0]))
         {
            builderCodes.Append("any SQL Server ") ;
            link.Append("any SQL Server ") ;
         }
         else
         {
            //  In this case, the user hasn't specified target instances
            if(rule.TargetInstances.Length == 0)
            {
               builderCodes.Append(@"\ul\cf2specified SQL Servers\cf0\ulnone ") ;
               link.AppendLink("specified SQL Servers", "TargetInstances") ;
            }
            else
            {
               if(rule.TargetInstances.Length > 1)
               {
                  builderCodes.Append("SQL Servers ") ;
                  link.Append("SQL Servers ") ;
               }
               else
               {
                  builderCodes.Append("SQL Server ") ;
                  link.Append("SQL Server ") ;
               }
               string sInstances = rule.TargetInstanceList ;
               sInstances = link.AppendLink(sInstances, "TargetInstances") ;
               builderCodes.AppendFormat(@"\ul\cf1 {0}\cf0\ulnone ", RtfEscape(sInstances)) ;
            }
         }

         if(dbLevel != null)
         {
            builderCodes.Append(@"\line   and ") ;
            link.Append("   and ") ;
            if(dbLevel.StringArray == null || dbLevel.StringArray.Length == 0)
            {
               builderCodes.Append(@"\ul\cf2specified databases\cf0\ulnone ") ;
               link.AppendLink("specified databases", dbLevel) ;
            }
            else
            {
               string sDatabases = String.Join(",", dbLevel.StringArray) ;

               if(dbLevel.Inclusive)
               {
                  if(dbLevel.StringArray.Length == 1)
                  {
                     builderCodes.Append("database ") ;
                     link.Append("database ") ;
                  }
                  else
                  {
                     builderCodes.Append("databases ") ;
                     link.Append("databases ") ;
                  }
               }
               else
               {
                  builderCodes.Append("all databases except ") ;
                  link.Append("all databases except ") ;
               }
               sDatabases = link.AppendLink(sDatabases, dbLevel) ;
               builderCodes.AppendFormat(@"\ul\cf1 {0}\cf0\ulnone ", RtfEscape(sDatabases)) ;
            }
         }

         if(objectLevel != null)
         {
            builderCodes.Append(@"\line   and ") ;
            link.Append("   and ") ;
            if(objectLevel.StringArray == null || objectLevel.StringArray.Length == 0)
            {
               builderCodes.Append(@"\ul\cf2specified objects\cf0\ulnone ") ;
               link.AppendLink("specified objects", objectLevel) ;
            }
            else
            {
               string sObjects = String.Join(",", objectLevel.StringArray) ;

               if(objectLevel.Inclusive)
               {
                  if(objectLevel.StringArray.Length == 1)
                  {
                     builderCodes.Append("object ") ;
                     link.Append("object ") ;
                  }
                  else
                  {
                     builderCodes.Append("objects ") ;
                     link.Append("objects ") ;
                  }
               }
               else
               {
                  builderCodes.Append("all objects except ") ;
                  link.Append("all objects except ") ;
               }
               sObjects = link.AppendLink(sObjects, objectLevel) ;
               builderCodes.AppendFormat(@"\ul\cf1 {0}\cf0\ulnone ", RtfEscape(sObjects)) ;
            }
         }
      }

      private static void GenerateFiltersString(AlertRule rule, StringBuilder builderCodes, LinkString link, AlertingConfiguration config)
      {
         bool hasWhere = false ;
         bool hasCategory = false ;
         string tempString ;
         StringBuilder tempBuilder = new StringBuilder(128);

         // We always have at last a type or category condition
         //if(rule.Conditions.Length == 1)
            //return ;

         foreach(EventCondition condition in rule.Conditions)
         {
            if(condition.FieldId == (int)AlertableEventFields.eventCategory)
               hasCategory = true ;
         }

         foreach(EventCondition condition in rule.Conditions)
         {
            // These fields are used for conditions other than filter
            if(condition.FieldId == (int)AlertableEventFields.objectName)
               continue ;
            if(condition.FieldId == (int)AlertableEventFields.databaseName)
               continue ;
            if(condition.FieldId == (int)AlertableEventFields.eventCategory)
               continue ;
            if(!hasCategory && condition.FieldId == (int)AlertableEventFields.eventType && condition.Inclusive)
               continue ;

            if(!hasWhere)
            {
               builderCodes.Append(@"\line   where ") ;
               link.Append(@"   where ") ;
               hasWhere = true ;
            }
            else
            {
               builderCodes.Append(@"\line   and ") ;
               link.Append("   and ") ;
            }

            switch(condition.ConditionType)
            {
               case MatchType.Bool:
                  builderCodes.AppendFormat(@"{0} is \ul\cf1 {1}\cf0\ulnone ", RtfEscape(condition.TargetEventField.DisplayName), RtfEscape(condition.BooleanValue.ToString())) ;
                  link.Append(String.Format("{0} is ", condition.TargetEventField.DisplayName)) ;
                  link.AppendLink(condition.BooleanValue.ToString(), condition) ;
                  break ;
               case MatchType.Integer:
                  if(condition.FieldId == (int)AlertableEventFields.eventType && !condition.Inclusive)
                  {
                     builderCodes.Append("event type not ") ;
                     link.Append("event type not ") ;
                     tempString = "" ;
                     for(int i = 0 ; i < condition.IntegerArray.Length ; i++)
                     {
                        CMEventType evType = config.LookupEventType(condition.IntegerArray[i], EventType.SqlServer) ;
                        tempString += evType.Name ;
                        if(i < condition.IntegerArray.Length - 1)
                           tempString += "," ;
                     }
                     if(tempString.Length == 0)
                     {
                        builderCodes.Append(@"\ul\cf2select event types\cf0\ulnone") ;
                        link.AppendLink("select event types", condition) ;
                     }
                     else
                     {
                        tempString = link.AppendLink(tempString, condition) ;
                        builderCodes.AppendFormat(@"\ul\cf1 {0}\cf0\ulnone", RtfEscape(tempString)) ;
                     }
                  }
                  break ;
               case MatchType.String:
                  if (condition.FieldId == (int)AlertableEventFields.rowCount)
                  {
                      tempString = "";
                      builderCodes.Append("Row Count is ");
                      link.Append("Row Count is ");

                      tempString = condition.CbOprtr + " " + condition.IntegerRowcount + " ";
                      string tempString_TimeFrame = condition.IntegerTimeFrame;
                      
                      
                          if (string.IsNullOrEmpty(condition.IntegerRowcount))
                          {
                              builderCodes.Append(@"\ul\cf2Specify Row Count Threshold\cf0\ulnone");
                              link.AppendLink("Specify Row Count Threshold", condition);

                          }
                      

                      else
                      {
                          tempString = link.AppendLink(tempString, condition);
                          builderCodes.AppendFormat(@"\ul\cf1 {0}\cf0\ulnone ", RtfEscape(tempString));
                          if (!string.IsNullOrEmpty(condition.IntegerTimeFrame))
                          {
                              builderCodes.Append(" over the time period of ");
                              link.Append(" over the time period of ");
                              builderCodes.AppendFormat(@"\ul\cf1 {0}\cf0\ulnone ", RtfEscape(tempString_TimeFrame));
                              link.AppendLink(tempString_TimeFrame, condition);
                              builderCodes.Append(" Hours ");
                              link.Append(" Hours ");
                          }
                      } 
                      break;
                  }
                  if(condition.Inclusive)
                  {
                     builderCodes.AppendFormat("{0} like ", RtfEscape(condition.TargetEventField.DisplayName)) ;
                     link.Append(String.Format("{0} like ", condition.TargetEventField.DisplayName)) ;
                  }
                  else
                  {
                     builderCodes.AppendFormat("{0} not like ", RtfEscape(condition.TargetEventField.DisplayName)) ;
                     link.Append(String.Format("{0} not like ", condition.TargetEventField.DisplayName)) ;
                  }
                  if(condition.IsValid)
                  {
                     tempBuilder.Remove(0, tempBuilder.Length) ;
                     for(int i = 0 ; i < condition.StringArray.Length ; i++)
                     {
                        tempBuilder.AppendFormat("'{0}'", condition.StringArray[i]);
                        if(i < condition.StringArray.Length - 1)
                        {
                           if(condition.Inclusive)
                              tempBuilder.Append(" or ");
                           else
                              tempBuilder.Append(" and ");
                        }
							
                     }
                     tempString = link.AppendLink(tempBuilder.ToString(), condition) ;
                     builderCodes.AppendFormat(@"\ul\cf1 {0}\cf0\ulnone ", RtfEscape(tempString)) ;
                  }
                  else
                  {
                     builderCodes.Append(@"\ul\cf2specified words\cf0\ulnone ") ;
                     link.AppendLink("specified words", condition) ;
                  }
                  break ;
            }                        
         }
      }

      private static void GenerateActionString(AlertRule rule, StringBuilder builderCodes, LinkString link)
		{
         if(rule.HasEmailAction || rule.HasLogAction || rule.HasSnmpTrapAction || rule.HasEmailSummaryAction)
         {
            builderCodes.Append(@"\line Send an \ul\cf1alert message\cf0\ulnone  to\line   ") ;
            link.Append(" Send an ") ;
            link.AppendLink("alert message", "AlertMessage") ;
            link.Append(" to   ") ;
         }

         if(rule.HasEmailAction || rule.HasEmailSummaryAction)
         {
            if(rule.Recipients.Length == 0)
            {
               builderCodes.Append(@"\ul\cf2specified addresses\cf0\ulnone") ;
               link.AppendLink("specified addresses", "Recipients") ;
            }
            else
            {
               StringBuilder builder = new StringBuilder("");

               for(int i = 0 ; i < rule.Recipients.Length ; i++)
               {
                  builder.Append(rule.Recipients[i]) ;
                  if(i != rule.Recipients.Length - 1)
                     builder.Append(", ") ;
               }
               string realString = link.AppendLink(builder.ToString(), "Recipients") ;
               builderCodes.AppendFormat(@"\ul\cf1 {0}\cf0\ulnone", RtfEscape(realString)) ;
            }
            if(rule.HasLogAction || rule.HasSnmpTrapAction)
            {
               builderCodes.Append(@"\line   and ") ;
               link.Append("   and ") ;
            }
         }

         if(rule.HasLogAction)
         {
            if(!rule.HasLogAction)
               builderCodes.Append(@"\line   ") ;
            EventLogEntryType evType  = EventLogEntryType.Error ;

            switch(rule.LogEntryType)
            {
               case ErrorLog.Severity.Informational:
                  evType = EventLogEntryType.Information ;
                  break ;
               case ErrorLog.Severity.Warning:
                  evType = EventLogEntryType.Warning ;
                  break ;
               case ErrorLog.Severity.Error:
                  evType = EventLogEntryType.Error ;
                  break ;
            }

            builderCodes.AppendFormat(@"Windows Event Log entry of type \ul\cf1 {0}\cf0\ulnone ", RtfEscape(evType.ToString())) ;
            link.Append("Windows Event Log entry of type ") ;
            link.AppendLink(evType.ToString(), "LogEntryType") ;

            if (rule.HasSnmpTrapAction)
            {
                builderCodes.Append(@"\line   and ");
                link.Append("   and ");
            }
         }

         if (rule.HasSnmpTrapAction)
         {
             builderCodes.Append(@"specified \ul\cf1network management console\cf0\ulnone");
             link.Append("specified ");
             link.AppendLink("network management console", "SnmpConfigurations");
         }
		}

		private static string RtfEscape(string s)
		{
			ASCIIEncoding encoder = new ASCIIEncoding() ;
			StringBuilder retVal = new StringBuilder() ;

			byte[] myBytes = encoder.GetBytes(s) ;
			for(int i = 0 ; i < myBytes.Length ; i++)
			{
				byte b = myBytes[i] ;
				if((b >= 0 && b < 0x20) ||
					(b >= 0x80 && b <= 0xFF) ||
					b == 0x5C || b == 0x7B || b == 0x7D)
				{
					retVal.AppendFormat("\\'{0:X2}",  b) ;
				}
				else
					retVal.Append(s[i]) ;
			}
			return retVal.ToString(); 
		}
	}

	public class EnumComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			if(x == null)
			{
				if(y == null)
					return 0 ;
				else 
					return -1 ;
			}
				
			if(x is Enum)
			{
				Int32 intValue = (int)x ;
				if(y is Enum)
					return intValue.CompareTo((int)y) ;
				else
					return -1 ;
			}
			else
			{
				return -1 ;
			}
		}
	}
}
