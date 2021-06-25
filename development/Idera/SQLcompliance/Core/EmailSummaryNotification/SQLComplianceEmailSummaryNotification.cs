using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mail;

namespace Idera.SQLcompliance.Core.EmailSummaryNotification
{
    public class SQLComplianceEmailSummaryNotification
    {

        private object startStopLock = new Object();
        static string processOcurrenceTime = "";

        #region Singleton constructors and properties

        /// <summary>
        /// The singleton instance of this class
        /// </summary>
        private static SQLComplianceEmailSummaryNotification instance;

        /// <summary>
        /// Static constructor to create singleton instance
        /// </summary>
        static SQLComplianceEmailSummaryNotification()
        {
            instance = new SQLComplianceEmailSummaryNotification();
        }

        /// <summary>
        /// Private constructor to prevent code from creating additional instances of singleton
        /// </summary>
        private SQLComplianceEmailSummaryNotification()
        {
        }

        /// <summary>
        /// Internal property to provide access to singleton instance
        /// </summary>
        public static SQLComplianceEmailSummaryNotification Instance
        {
            get { return instance; }
        }
        #endregion


        public void Start()
        {
            lock (startStopLock) // prevent stop from being run until start is done
            {
                String SQLCMSqlServerName = FetchSQLCMSQLServerNameFromRegistry();
                processOcurrenceTime = DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss");
                if (SQLCMSqlServerName != null)
                {
                    Logger.Initialize("SQLcomplianceEmailSummaryTrigger", 5, 1000000);
                    Repository rep = new Repository();
                    List<AlertRule> alertRules = new List<AlertRule>();
                    Dictionary<int, List<AlertMessage>> alerts = new Dictionary<int, List<AlertMessage>>();
                    MailConfiguration config = new MailConfiguration();

                    Repository.ServerInstance = SQLCMSqlServerName;
                    rep.OpenConnection();

                    alertRules = GetEventAlertRules(rep.connection);
                    if (alertRules.Count > 0)
                        alertRules = GetApplicableRules(alertRules);
                    if (alertRules.Count > 0)
                    {
                        alerts = GetAlerts(alertRules, rep.connection);
                        if (alerts.Count > 0)
                        {
                            config = GetConfiguration(rep.connection);
                            SendMailAndUpdateTime(config, alerts, rep.connection);
                        }
                    }
                    rep.CloseConnection();
                }
            }
        }

        private static string FetchSQLCMSQLServerNameFromRegistry()
        {
            RegistryKey rk = null;
            RegistryKey rk2 = null;
            string serverInstance = null;

            try
            {
                rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32));
                rk2 = rk.OpenSubKey(CoreConstants.SQLcompliance_RegKey);

                serverInstance = (string)rk2.GetValue("SQLcmSQLServerName");
            }
            catch (Exception)
            {
            }
            finally
            {
                if (rk != null) rk.Close();
                if (rk2 != null) rk2.Close();
            }
            if (serverInstance != null)
                return serverInstance;
            else
                return null;
        }

        private static List<AlertRule> GetEventAlertRules(SqlConnection conn)
        {
            List<AlertRule> rules = new List<AlertRule>();
            int column = 0;

            try
            {
                string selectCmd = "SELECT ruleId,emailSummaryMessage,summaryEmailFrequency,lastEmailSummarySendTime from [SQLcompliance]..[AlertRules] where alertType = 1 and emailSummaryMessage = 1";
                if (conn != null)
                {
                    using (SqlCommand cmd = new SqlCommand(selectCmd, conn))
                    {
                        cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AlertRule rule = new AlertRule();
                                column = 0;

                                rule.RuleId = reader.GetInt32(column++);
                                if (!reader.IsDBNull(column))
                                    rule.HasEmailSummaryMessage = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
                                else
                                    rule.HasEmailSummaryMessage = false;
                                if (!reader.IsDBNull(column))
                                    rule.SummaryEmailFrequency = reader.GetInt32(column++);
                                else
                                    rule.SummaryEmailFrequency = 0;
                                if (!reader.IsDBNull(column))
                                    rule.LastEmailSummarySendTime = reader.GetDateTime(column++);
                                else
                                    rule.LastEmailSummarySendTime = DateTime.Now;

                                if (rule.HasEmailSummaryMessage)
                                    rules.Add(rule);
                            }
                        }
                    }
                }
                Logger.Info("Successfully fetched alert rules");
            }
            catch (Exception ex)
            {
                Logger.Info("Error reading alerts rules due to " + ex.Message);
            }

            return rules;
        }

        private static List<AlertRule> GetApplicableRules(List<AlertRule> rules)
        {
            List<AlertRule> applicableRules = new List<AlertRule>();
            DateTime currentTime = DateTime.Now;

            foreach (AlertRule rule in rules)
            {
                DateTime alertRuleDateTime = rule.LastEmailSummarySendTime;
                DateTime newAlertRuleDateTime = alertRuleDateTime.AddMinutes(rule.SummaryEmailFrequency);

                if (DateTime.Compare(currentTime, newAlertRuleDateTime) > 0)
                    applicableRules.Add(rule);
            }
            Logger.Info("Successfully calculated applicable rules");
            return applicableRules;
        }

        private static Dictionary<int, List<AlertMessage>> GetAlerts(List<AlertRule> rules, SqlConnection conn)
        {
            Dictionary<int, List<AlertMessage>> alertsDict = new Dictionary<int, List<AlertMessage>>();
            int column = 0;
            string message = "";

            foreach (AlertRule rule in rules)
            {
                List<AlertMessage> alertMessages = new List<AlertMessage>();
                try
                {
                    string selectCmd = "SELECT message, created from [SQLcompliance]..[Alerts] where alertRuleId = " + rule.RuleId + "and created >= '" + rule.LastEmailSummarySendTime.ToUniversalTime().ToString("MM-dd-yyyy HH:mm:ss") + "'";
                    if (conn != null)
                    {
                        using (SqlCommand cmd = new SqlCommand(selectCmd, conn))
                        {
                            cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    AlertMessage alertMessage = new AlertMessage();
                                    column = 0;

                                    message = reader.GetString(column++);
                                    try
                                    {
                                        Hashtable map = ParseString(message);
                                        alertMessage.Title = (string)map["title"] + " at " + reader.GetDateTime(column).ToLocalTime();
                                        alertMessage.Body = (string)map["body"];
                                        string temp = (string)map["recipients"];
                                        if (temp != null && temp.Trim().Length > 0)
                                            alertMessage.Recipients = (string)map["recipients"];
                                        else
                                            alertMessage.Recipients = "";
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Info("Some error occurred while fetching alerts from database");
                                    }
                                    alertMessages.Add(alertMessage);
                                }
                                if (alertMessages.Count > 0)
                                    alertsDict.Add(rule.RuleId, alertMessages);
                            }
                        }
                    }
                    Logger.Info("Successfully fetched alerts from database");
                }
                catch (Exception ex)
                {
                    string msg = String.Format("Error reading alerts.\n\nError: {0}",
                       ex.Message);
                    throw new Exception(msg);
                }
            }
            return alertsDict;
        }

        private static Hashtable ParseString(string s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            Hashtable retVal = new Hashtable();
            string active = s;
            int index = s.IndexOf("(");

            try
            {
                while (index != -1)
                {
                    string sKey, sValue;
                    int length;

                    sKey = active.Substring(0, index);
                    active = active.Substring(index + 1);
                    index = active.IndexOf(")");
                    length = Int32.Parse(active.Substring(0, index));
                    active = active.Substring(index + 1);
                    sValue = active.Substring(0, length);
                    active = active.Substring(length);
                    retVal.Add(sKey, sValue);
                    index = active.IndexOf("(");
                }
            }
            catch (Exception e)
            {
                Logger.Info("Improperly formed KeyValue string " + e.Message);
            }
            if (active.Length > 0)
                Logger.Info("Improperly formed KeyValue string.");
            return retVal;
        }

        private static MailConfiguration GetConfiguration(SqlConnection conn)
        {
            MailConfiguration config = new MailConfiguration();
            int column = 0;
            try
            {
                string selectCmd = "SELECT smtpServer, smtpSenderAddress, smtpPort, smtpSsl, smtpAuthType, smtpUsername, smtpPassword from [SQLcompliance]..[Configuration]";
                if (conn != null)
                {
                    using (SqlCommand cmd = new SqlCommand(selectCmd, conn))
                    {
                        cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                column = 0;

                                config.Server = reader.GetString(column++);
                                config.SenderAddress = reader.GetString(column++);
                                config.Port = reader.GetInt32(column++);
                                config.UseSSL = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));
                                config.Authentication = reader.GetInt32(column++);
                                config.Username = reader.GetString(column++);
                                config.Password = reader.GetString(column++);
                            }
                        }
                    }
                }
                Logger.Info("Successfully fetched configuration from database");
            }
            catch (Exception ex)
            {
                Logger.Info("Error reading configuration rules due to " + ex.Message);
            }
            return config;
        }

        private static void SendMailAndUpdateTime(MailConfiguration config, Dictionary<int, List<AlertMessage>> alerts, SqlConnection conn)
        {
            StringBuilder emailBody;
            string emailRecipient;
            int messageCount;
            int fileNum;
            List<string> filesList = new List<string>();
            string filePath = "";
            foreach (KeyValuePair<int, List<AlertMessage>> alert in alerts)
            {
                messageCount = 0;
                fileNum = 1;
                emailBody = new StringBuilder();
                emailRecipient = "";
                foreach (AlertMessage messages in alert.Value)
                {
                    emailBody.Append("Title- " + messages.Title + "\n");
                    emailBody.Append("Message- " + messages.Body + "\n\n");
                    emailRecipient = messages.Recipients;
                    messageCount++;
                    if(messageCount == 100000)
                    {
                        filePath = Path.Combine(Path.GetTempPath(),"Alerts_" + DateTime.Now.ToString().Replace(" ", "_").Replace(":", "_") + "_" + fileNum + ".txt");
                        filesList.Add(filePath);
                        if (!File.Exists(filePath))
                        {
                            using (StreamWriter sw = File.CreateText(filePath))
                            {
                                sw.WriteLine(emailBody.ToString());
                            }
                        }
                        emailBody = new StringBuilder();
                        fileNum++;
                        messageCount = 0;
                    }
                }
                if (messageCount < 100000)
                {
                    filePath = Path.Combine(Path.GetTempPath(), "Alerts_" + DateTime.Now.ToString().Replace(" ", "_").Replace(":", "_") + "_" + fileNum + ".txt");
                    filesList.Add(filePath);
                    if (!File.Exists(filePath))
                    {
                        using (StreamWriter sw = File.CreateText(filePath))
                        {
                            sw.WriteLine(emailBody.ToString());
                        }
                    }
                }
                if (PerformSmtpAction(config, filesList, emailRecipient))
                {
                    UpdateEmailSummarySendTime(alert.Key, conn);
                }
            }
        }

        private static bool PerformSmtpAction(MailConfiguration config, List<string> attachmentFiles, string recipient)
        {
            MailMessage msg;
            MailAttachment mailAttachment;
            bool retVal = false;

            foreach (string fileName in attachmentFiles)
            {
                msg = new MailMessage();

                msg.BodyFormat = MailFormat.Text;
                msg.From = config.SenderAddress;
                msg.Subject = "SQL Compliance Event Alerts Summary Notification";
                msg.Body = "Please find the alert notifications in the attachment.";
                msg.To = String.Join(";", recipient);
                msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtsperver"] = config.Server;
                msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"] = config.Port;
                msg.Fields["http://schemas.microsoft.com/cdo/configuration/sendusing"] = 2;
                msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"] = config.UseSSL.ToString();
                msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"] = (int)config.Authentication;
                if (config.Authentication == 1)
                {
                    msg.Fields["http://schemas.microsoft.com/cdo/configuration/sendusername"] = config.Username;
                    msg.Fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"] = config.Password;
                }

                mailAttachment = new MailAttachment(fileName);
                msg.Attachments.Add(mailAttachment);

                try
                {
                    SmtpMail.SmtpServer = config.Server;
                    SmtpMail.Send(msg);
                    Logger.Info("Successfully sent email to " + recipient);
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                    retVal = true;
                }
                catch (Exception e)
                {
                    Logger.Info("Email summary notification could not be sent to " + recipient + " due to " + e.Message);
                    retVal = false;
                }
            }
            return retVal;
        }

        private static void UpdateEmailSummarySendTime(int alertRuleId, SqlConnection conn)
        {
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = "Update [SQLcompliance]..[AlertRules] set lastEmailSummarySendTime = '" + processOcurrenceTime + "' where ruleId = " + alertRuleId;
                    command.ExecuteNonQuery();
                }
                Logger.Info("Successfully updated AlertRules table with lastEmailSummarySendTime");
            }
            catch (Exception ex)
            {
                Logger.Info("Error updating email summary send time for ruleId " + alertRuleId + " due to " + ex.Message);
            }
        }
    }
}
