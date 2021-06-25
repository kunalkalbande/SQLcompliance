using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Rules.Alerts;

namespace Idera.SQLcompliance.Core.Rules
{
   public class StatusRuleProcessor
   {

      public StatusRuleProcessor()
      {
      }

      public List<StatusAlert> GenerateAlerts(SqlConnection connection, List<StatusAlertRule> rules)
      {
         return (GenerateAlerts(connection, rules, false, 0, 0, 0));
      }

      public List<StatusAlert> GenerateAlerts(SqlConnection connection, 
                                              List<StatusAlertRule> rules, 
                                              bool agentAlerts, 
                                              StatusRuleType agentAlertType, 
                                              long folderSize, 
                                              long maxFolderSize)
      {
         return GenerateAlerts(connection,
                               rules,
                               agentAlerts,
                               agentAlertType,
                               folderSize,
                               maxFolderSize,
                               "");
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="connection"></param>
      /// <param name="rules"></param>
      /// <returns></returns>
      public List<StatusAlert> GenerateAlerts(SqlConnection connection, 
                                              List<StatusAlertRule> rules, 
                                              bool agentAlerts, 
                                              StatusRuleType agentAlertType, 
                                              long folderSize, 
                                              long maxFolderSize,
                                              string instance)
      {
         List<StatusAlert> alerts = new List<StatusAlert>();
         List<AgentHeartBeat> agentHeartbeats = new List<AgentHeartBeat>();
         Dictionary<string, long> repoSizes = new Dictionary<string, long>();
         long size = 0;

         foreach (StatusAlertRule rule in rules)
         {
            // don't try to process a disabled rule.
            if (rule.Enabled == false)
               continue;

            switch (rule.StatusType)
            {
               case StatusRuleType.NoHeartbeats:
                  {
                     if (agentAlerts)
                        continue;

                     try
                     {
                        if (agentHeartbeats.Count <= 0)
                        {
                           GetAgentHeartbeats(connection, agentHeartbeats);
                        }
                        foreach (AgentHeartBeat agentHeartbeat in agentHeartbeats)
                        {
                           // if now minus the interval is still greater than the last heartbeat, create an alert.
                           if (agentHeartbeat.LastHeartbeat < DateTime.UtcNow.Subtract(new TimeSpan(0, agentHeartbeat.HeartbeatInterval, 0)))
                           {
                              alerts.Add(GenerateAlert(rule, agentHeartbeat, agentHeartbeat.Agent));
                           }
                        }
                     }
                     catch (Exception e)
                     {
                        ErrorLog.Instance.Write(ErrorLog.Level.Always, String.Format("Unable to process the No Heartbeat Status Alert. Error:{0}", e.ToString()), ErrorLog.Severity.Error);
                     }
                     break;
                  }
               case StatusRuleType.RepositoryTooBig:
                  {
                     if (agentAlerts)
                        continue;
                     try
                     {
                        GetRepositorySizes(connection, repoSizes);

                        foreach (KeyValuePair<string, long> kvp in repoSizes)
                        {
                           //The threshold value for the repository sizes signifies GB.  Ex:  user enters 20 - this is 20GB
                           // 20 must be converted to 20 * 1024 * 1024 * 1024
                           if (kvp.Value > (rule.Threshold.Value * CoreConstants.OneGigabyte))
                           {
                              alerts.Add(GenerateAlert(rule, kvp.Value, kvp.Key));
                           }
                        }
                     }
                     catch (Exception e)
                     {
                        ErrorLog.Instance.Write(ErrorLog.Level.Always, String.Format("Unable to process the Repository Size Status Alert. Error:{0}", e.ToString()), ErrorLog.Severity.Error);
                     }
                     break;
                  }
               case StatusRuleType.TraceDirFullCollect:
                  {
                     if (agentAlerts)
                        continue;

                     try
                     {
                        size = GetTraceDirSize();

                        if (size > (rule.Threshold.Value * CoreConstants.OneGigabyte))
                        {
                           string server;
                           int index = CollectionServer.ServerInstance.IndexOf('\\');

                           if (index == -1)
                              server = CollectionServer.ServerInstance;
                           else
                              server = CollectionServer.ServerInstance.Substring(0, index);

                           alerts.Add(GenerateAlert(rule, size, server));
                        }
                     }
                     catch (Exception e)
                     {
                        ErrorLog.Instance.Write(ErrorLog.Level.Always, String.Format("Unable to process the Collection Server Trace Directory Status Alert. Error:{0}", e.ToString()), ErrorLog.Severity.Error);
                     }
                     break;
                  }
               case StatusRuleType.TraceDirFullAgent:
                  {
                     if (agentAlerts && agentAlertType == StatusRuleType.TraceDirFullAgent)
                     {
                        try
                        {
                           long percentFull = rule.Threshold.Value;
                           float actualPercentFull = (float)folderSize / (float)maxFolderSize;

                           if ((actualPercentFull > 1) ||
                               ((actualPercentFull * 100) > percentFull))
                           {
                              alerts.Add(GenerateAlert(rule, folderSize, instance));
                           }
                        }
                        catch (Exception e)
                        {
                           ErrorLog.Instance.Write(ErrorLog.Level.Always, String.Format("Unable to process the Agent Trace Directory Status Alert. Error:{0}", e.ToString()), ErrorLog.Severity.Error);
                        }
                     }
                     else
                     {
                        continue;
                     }
                     break;
                  }
               case StatusRuleType.SqlServerDown:
                  {
                     if (agentAlerts && agentAlertType == StatusRuleType.SqlServerDown)
                     {
                        try
                        {
                           alerts.Add(GenerateAlert(rule, null, instance));
                        }
                        catch (Exception e)
                        {
                           ErrorLog.Instance.Write("Unable to process the Unable to communicate to Audited Instance Status Alert. Error:{0}", e.ToString());
                        }
                     }
                     else
                     {
                        continue;
                     }
                     break;
                  }
            }
         }
         return (alerts);
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="rule"></param>
      /// <param name="size"></param>
      /// <returns></returns>
      private StatusAlert GenerateAlert(StatusAlertRule rule, object alertInfo, string instance)
      {
         StatusAlert alert = new StatusAlert();

         alert.AlertType = EventType.Status;
         alert.ParentRule = rule;
         alert.Instance = instance;
         alert.Level = rule.Level;
         alert.Threshold = rule.Threshold;
         alert.EventType = rule.StatusType;
         alert.MessageData = rule.MessageData;
         if (rule.HasLogAction)
            alert.LogStatus = NotificationStatus.Pending;
         if (rule.HasEmailAction)
            alert.EmailStatus = NotificationStatus.Pending;
          if (rule.HasSnmpTrapAction)
              alert.SnmpTrapStatus = NotificationStatus.Pending;
         alert.RuleName = rule.Name;
         alert.ParentRuleId = rule.Id;
         alert.StatusData = alertInfo;
         alert.StatusRuleTypeName = rule.StatusRuleTypeName;

         switch (rule.StatusType)
         {
            case StatusRuleType.NoHeartbeats:
            case StatusRuleType.RepositoryTooBig:
            case StatusRuleType.SqlServerDown:
               {
                  string server;
                  int index = instance.IndexOf('\\');

                  if (index == -1)
                     server = instance;
                  else
                     server = instance.Substring(0, index);
                  alert.ComputerName = server;
                  alert.Instance = instance;
                  break;
               }
            case StatusRuleType.TraceDirFullAgent:
            case StatusRuleType.TraceDirFullCollect:
               {
                  alert.ComputerName = instance;
                  alert.Instance = "N/A";
                  break;
               }
         }
         ErrorLog.Instance.Write(ErrorLog.Level.Debug,String.Format("An alert is generated for:\n" +
                                                                   "Rule: {0}\n" +
                                                                   "Type: {1}\n" +
                                                                   "Time: {2}\n",
                                                                   rule.Name,
                                                                   rule.StatusType,
                                                                   DateTime.Now.ToShortTimeString()));
         return alert;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="source"></param>
      /// <param name="alert"></param>
      /// <returns></returns>
      public string ExpandMacros(string source, StatusAlert alert)
      {
         StringBuilder builder = new StringBuilder(source);
         string stringSize;

         builder.Replace("$AlertType$", alert.AlertType.ToString());
         builder.Replace("$AlertTime$", alert.Created.ToString());
         builder.Replace("$AlertLevel$", alert.Level.ToString());

         switch (alert.EventType)
         {
            case StatusRuleType.TraceDirFullAgent:
               {
                  stringSize = String.Format("{0}%", alert.Threshold.Value.ToString());
                  builder.Replace("$ThresholdValue$", stringSize);
                  break;
               }
            case StatusRuleType.RepositoryTooBig:
            case StatusRuleType.TraceDirFullCollect:
               {
                  stringSize = String.Format("{0}GB", alert.Threshold.Value.ToString());
                  builder.Replace("$ThresholdValue$", stringSize);
                  break;
               }
            default:
               {
                  builder.Replace("$ThresholdValue$", "");
                  break;
               }
         }
         builder.Replace("$AlertTypeName$", alert.StatusRuleTypeName);
         builder.Replace("$ServerName$", alert.Instance);
         builder.Replace("$ComputerName$", alert.ComputerName);

         if (alert.StatusData != null)
         {
            if (alert.StatusData is AgentHeartBeat)
            {
               AgentHeartBeat heartBeat = (AgentHeartBeat)alert.StatusData;
               builder.Replace("$ActualValue$", heartBeat.LastHeartbeat.ToLocalTime().ToString());
            }
            else if (alert.StatusData is long)
            {
               long size = (long)alert.StatusData;
               string sizeString = String.Format("{0:F}GB", (float)size / (float)CoreConstants.OneGigabyte);
               builder.Replace("$ActualValue$", sizeString);
            }
         }
         else
         {
            builder.Replace("$ActualValue$", "");
         }
         return builder.ToString();
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="connection"></param>
      /// <param name="agentHeartbeats"></param>
      private void GetAgentHeartbeats(SqlConnection connection, List<AgentHeartBeat> agentHeartbeats)
      {
         int column;
         string cmdStr = String.Format(
             "SELECT timeLastHeartbeat, instance, agentHeartbeatInterval FROM {0}..{1} WHERE isAuditedServer=1 and isEnabled=1", 
             CoreConstants.RepositoryDatabase,
             CoreConstants.RepositoryServerTable);
         using (SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using (SqlDataReader reader = command.ExecuteReader())
            {
               while (reader.Read())
               {
                  column = 0;

                  //if the Last Heartbeat is null, the agent is never reported (probably not deployed)
                  if (reader.IsDBNull(column))
                     continue;

                  AgentHeartBeat heartBeat = new AgentHeartBeat();
                  heartBeat.LastHeartbeat = reader.GetDateTime(column++);
                  heartBeat.Agent = reader.GetString(column++);
                  heartBeat.HeartbeatInterval = reader.GetInt32(column++);
                  agentHeartbeats.Add(heartBeat);
               }
            }
         }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      private long GetTraceDirSize()
      {
         long totalDirSize = 0;

         DirectoryInfo directory = new DirectoryInfo(CollectionServer.traceDirectory);
         FileInfo[] files = directory.GetFiles();

         foreach (FileInfo fileInfo in files)
         {
            totalDirSize += fileInfo.Length;
         }
         return totalDirSize;
      }

      /// <summary>
      /// 
      /// </summary>
      private void GetRepositorySizes(SqlConnection connection, Dictionary<string, long> repoSizes)
      {
         Dictionary<string, string> repoNames = new Dictionary<string, string>();
         int column;
         long size = 0;
         string cmdStr;

         cmdStr = String.Format("select instance, eventDatabase from [{0}]..{1} where isAuditedServer = 1", CoreConstants.RepositoryDatabase, CoreConstants.RepositoryServerTable);
         
         using (SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using (SqlDataReader reader = command.ExecuteReader())
            {
               while (reader.Read())
               {
                  column = 0;
                  repoNames.Add(reader.GetString(column++), reader.GetString(column++));
               }
            }
         }

         string databaseName;

         //foreach of the event databases, get the size.
         foreach (KeyValuePair<string, string> kvp in repoNames)
         {
            databaseName = kvp.Value.Trim();

            //skip archives
            if (String.IsNullOrEmpty(databaseName))
               continue;
            cmdStr = String.Format("select isnull(sum(convert(bigint,size) * 8), 0) from [{0}]..sysfiles (nolock)", databaseName);

            using (SqlCommand command = new SqlCommand(cmdStr, connection))
            {
               using (SqlDataReader reader = command.ExecuteReader())
               {
                  while (reader.Read())
                  {
                     column = 0;
                     if (reader.IsDBNull(column) == false)
                     {
                        size = reader.GetInt64(column++);

                        //Convert the size to byets.  The value returned from SQL server is in KB
                        repoSizes.Add(kvp.Key, size * 1024);
                     }
                  }
               }
            }
         }
      }
   }
}
