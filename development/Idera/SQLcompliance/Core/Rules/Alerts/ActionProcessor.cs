using System;
using System.Collections.Generic;
using System.Data.SqlClient ;
using System.Text ;
using System.Web.Mail ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Event ;
using SnmpSharpNet;

namespace Idera.SQLcompliance.Core.Rules.Alerts
{
	/// <summary>
	/// Summary description for ActionProcessor.
	/// </summary>
	public class ActionProcessor
	{
		#region Member Variables

		private AlertingConfiguration _configuration ;

		#endregion

		#region Properties

		#endregion

		#region Construction/Destruction

		public ActionProcessor(AlertingConfiguration config)
		{
			_configuration = config ;
		}

		#endregion

      #region AlertActions

      public void PerformActions(List<Alert> alerts)
		{
         using(SqlConnection connection = new SqlConnection(_configuration.ConnectionString))
         {
            bool bChanged = false ;
            connection.Open() ;
            foreach(Alert alert in alerts)
            {
               bChanged = false ;
               if(alert.EmailStatus == NotificationStatus.Pending)
               {
                  PerformSmtpAction(alert, _configuration.SmtpSettings) ;
                  bChanged = true; 
               }

               if(alert.LogStatus == NotificationStatus.Pending)
               {
                  PerformEventLogAction(alert) ;
                  bChanged = true ;
               }

                if (alert.SnmpTrapStatus == NotificationStatus.Pending)
                {
                    PerformSnmpAction(alert);
                    bChanged = true;
                }

               if(bChanged)
                  AlertingDal.UpdateAlert(alert, connection) ;
            }
         }
		}

		public static void PerformEventLogAction(Alert alert)
		{
			try
			{
			   ErrorLog.Severity severity = (ErrorLog.Severity)alert.EventLogSeverity ;
				ErrorLog.Instance.WriteAlert(alert, severity) ;
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                    String.Format("Alert: Event for alert {0} on {1} written to event log.",
                                     alert.Id, alert.Instance));
            alert.LogStatus = NotificationStatus.Complete;
			}
			catch( Exception e )
			{
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                     String.Format("Alert: An error occurred when writting an event for alert {0} on {1}.  Exception: {2}",
                                                   alert.Id, alert.Instance, e),
                                     ErrorLog.Severity.Warning);
            alert.LogStatus = NotificationStatus.Failed;
         }
		}

		public static void PerformSmtpAction(Alert alert, SmtpConfiguration config)
		{
			MailMessage msg ;

			msg = new MailMessage() ;

			msg.BodyFormat = MailFormat.Text ;
			msg.From = config.SenderAddress ;
			msg.Subject = alert.MessageTitle ;
			msg.Body = alert.MessageBody ;
			msg.To = String.Join(";", alert.Recipients) ;
			msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtsperver"] = config.Server ;
			msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"] = config.Port ;
			msg.Fields["http://schemas.microsoft.com/cdo/configuration/sendusing"]  = 2 ;
			msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"] = config.UseSsl.ToString() ;
			msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"] = (int)config.Authentication;
			if(config.Authentication == SmtpAuthProtocol.Basic)
			{
				msg.Fields["http://schemas.microsoft.com/cdo/configuration/sendusername"] = config.Username ;
				msg.Fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"] = config.Password ;
			}

			try
			{
				SmtpMail.SmtpServer = config.Server;
				SmtpMail.Send(msg);
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                    String.Format("Alert: Email for alert {0} on {1} sent",
                                     alert.Id, alert.Instance));
            alert.EmailStatus = NotificationStatus.Complete ;
         }
			catch(Exception e)
			{
            string errorString ;
				while(e != null)
				{
					errorString = e.Message ;
					e = e.InnerException ;
               ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                        String.Format("Alert: An error occurred when sending an email for alert {0} on {1}.  Exception: {2}",
                                                      alert.Id, alert.Instance, errorString),
                                        ErrorLog.Severity.Warning);
            }
            alert.EmailStatus = NotificationStatus.Failed ;
			}
      }

        public static void PerformSnmpAction(Alert alert)
        {
            List<KeyValuePair<SNMPHelper.SqlCmVariable, string>> variableBindings = new List<KeyValuePair<SNMPHelper.SqlCmVariable, string>>();
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.AlertType, alert.AlertType.ToString("G")));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.Instance, alert.Instance));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.Created, alert.Created.ToString("G")));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.Level, alert.Level.ToString("G")));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.EventType, alert.EventType.ToString()));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.Message, alert.MessageBody));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.RuleName, alert.RuleName));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.MessageTitle, alert.MessageTitle));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.Severity, alert.EventLogSeverity.ToString("G")));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.ComputerName, string.Empty));

            VbCollection variableBindingCollection = SNMPHelper.CreateVariableBindings(variableBindings);

            try
            {
                SNMPHelper.SendSnmpTrap(variableBindingCollection, alert.ParentRule.SnmpConfiguration, false);
                alert.SnmpTrapStatus = NotificationStatus.Complete;
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                        String.Format("Alert: An error occurred when sending SNMP trap for alert {0} on {1}.  Exception: {2}", alert.Id, alert.Instance, e),
                                        ErrorLog.Severity.Warning);
                alert.SnmpTrapStatus = NotificationStatus.Failed;
            }
        }

      #endregion AlertActions 

      #region StatusAlertActions

      public void PerformActions(List<StatusAlert> alerts, SqlConnection connection)
      {
         bool bChanged = false;

         foreach (StatusAlert alert in alerts)
         {
            bChanged = false;

            if (alert.EmailStatus == NotificationStatus.Pending)
            {
               PerformSmtpAction(alert, _configuration.SmtpSettings);
               bChanged = true;
            }

            if (alert.LogStatus == NotificationStatus.Pending)
            {
               PerformEventLogAction(alert);
               bChanged = true;
            }
             if (alert.SnmpTrapStatus == NotificationStatus.Pending)
             {
                 PerformSnmpAction(alert);
                 bChanged = true;
             }

            if (bChanged)
            {
               AlertingDal.UpdateAlert(alert, connection);
            }
         }
      }

      public static void PerformEventLogAction(StatusAlert alert)
      {
         try
         {
            ErrorLog.Severity severity = (ErrorLog.Severity)alert.EventLogSeverity;
            ErrorLog.Instance.WriteAlert(alert, severity);
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                    String.Format("Alert: Event for alert {0} on {1} written to event log.",
                                     alert.Id, alert.Instance));
            alert.LogStatus = NotificationStatus.Complete;
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                     String.Format("Alert: An error occurred when writting an event for alert {0} on {1}.  Exception: {2}",
                                                   alert.Id, alert.Instance, e),
                                     ErrorLog.Severity.Warning);
            alert.LogStatus = NotificationStatus.Failed;
         }
      }

      public static void PerformSmtpAction(StatusAlert alert, SmtpConfiguration config)
      {
         MailMessage msg;

         msg = new MailMessage();

         msg.BodyFormat = MailFormat.Text;
         msg.From = config.SenderAddress;
         msg.Subject = alert.MessageTitle;
         msg.Body = alert.MessageBody;
         msg.To = String.Join(";", alert.Recipients);
         msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtsperver"] = config.Server;
         msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"] = config.Port;
         msg.Fields["http://schemas.microsoft.com/cdo/configuration/sendusing"] = 2;
         msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"] = config.UseSsl.ToString();
         msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"] = (int)config.Authentication;
         if (config.Authentication == SmtpAuthProtocol.Basic)
         {
            msg.Fields["http://schemas.microsoft.com/cdo/configuration/sendusername"] = config.Username;
            msg.Fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"] = config.Password;
         }

         try
         {
            SmtpMail.SmtpServer = config.Server;
            SmtpMail.Send(msg);
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                    String.Format("Alert: Email for alert {0} on {1} sent",
                                     alert.Id, alert.Instance));
            alert.EmailStatus = NotificationStatus.Complete;
         }
         catch (Exception e)
         {
            string errorString;
            while (e != null)
            {
               errorString = e.Message;
               e = e.InnerException;
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                        String.Format("Alert: An error occurred when sending an email for alert {0} on {1}.  Exception: {2}",
                                                      alert.Id, alert.Instance, errorString),
                                        ErrorLog.Severity.Warning);
            }
            alert.EmailStatus = NotificationStatus.Failed;
         }
      }

	    public static void PerformSnmpAction(StatusAlert alert)
	    {
            List<KeyValuePair<SNMPHelper.SqlCmVariable, string>> variableBindings = new List<KeyValuePair<SNMPHelper.SqlCmVariable, string>>();
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.AlertType, alert.AlertType.ToString("G")));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.Instance, alert.Instance));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.Created, alert.Created.ToString("G")));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.Level, alert.Level.ToString("G")));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.EventType, alert.EventType.ToString("G")));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.Message, alert.MessageBody));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.RuleName, alert.RuleName));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.MessageTitle, alert.MessageTitle));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.Severity, alert.EventLogSeverity.ToString("G")));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.ComputerName, alert.ComputerName));

            VbCollection variableBindingCollection = SNMPHelper.CreateVariableBindings(variableBindings);
	        try
	        {
                SNMPHelper.SendSnmpTrap(variableBindingCollection, alert.ParentRule.SnmpConfiguration, false);
                alert.SnmpTrapStatus = NotificationStatus.Complete;
	        }
	        catch (Exception e)
	        {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                        String.Format("Alert: An error occurred when sending SNMP trap for alert {0} on {1}.  Exception: {2}", alert.Id, alert.Instance, e),
                                        ErrorLog.Severity.Warning);
                alert.SnmpTrapStatus = NotificationStatus.Failed;
	        }
	    }

      #endregion StatusAlertActions

      #region DataAlertActions

      public void PerformActions(List<DataAlert> alerts)
      {
         using (SqlConnection connection = new SqlConnection(_configuration.ConnectionString))
         {
            bool bChanged = false;
            connection.Open();

            foreach (DataAlert alert in alerts)
            {
               bChanged = false;

               if (alert.EmailStatus == NotificationStatus.Pending)
               {
                  PerformSmtpAction(alert, _configuration.SmtpSettings);
                  bChanged = true;
               }

               if (alert.LogStatus == NotificationStatus.Pending)
               {
                  PerformEventLogAction(alert);
                  bChanged = true;
               }

                if (alert.SnmpTrapStatus == NotificationStatus.Pending)
                {
                    PerformSnmpAction(alert);
                    bChanged = true;
                }

               if (bChanged)
               {
                  AlertingDal.UpdateAlert(alert, connection);
               }
            }
         }
      }

      public static void PerformEventLogAction(DataAlert alert)
      {
         try
         {
            ErrorLog.Severity severity = (ErrorLog.Severity)alert.EventLogSeverity;
            ErrorLog.Instance.WriteAlert(alert, severity);
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                    String.Format("Alert: Event for alert {0} on {1} written to event log.",
                                     alert.Id, alert.Instance));
            alert.LogStatus = NotificationStatus.Complete;
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                     String.Format("Alert: An error occurred when writting an event for alert {0} on {1}.  Exception: {2}",
                                                   alert.Id, alert.Instance, e),
                                     ErrorLog.Severity.Warning);
            alert.LogStatus = NotificationStatus.Failed;
         }
      }

      public static void PerformSmtpAction(DataAlert alert, SmtpConfiguration config)
      {
         MailMessage msg;

         msg = new MailMessage();

         msg.BodyFormat = MailFormat.Text;
         msg.From = config.SenderAddress;
         msg.Subject = alert.MessageTitle;
         msg.Body = alert.MessageBody;
         msg.To = String.Join(";", alert.Recipients);
         msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtsperver"] = config.Server;
         msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"] = config.Port;
         msg.Fields["http://schemas.microsoft.com/cdo/configuration/sendusing"] = 2;
         msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"] = config.UseSsl.ToString();
         msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"] = (int)config.Authentication;
         if (config.Authentication == SmtpAuthProtocol.Basic)
         {
            msg.Fields["http://schemas.microsoft.com/cdo/configuration/sendusername"] = config.Username;
            msg.Fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"] = config.Password;
         }

         try
         {
            SmtpMail.SmtpServer = config.Server;
            SmtpMail.Send(msg);
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                    String.Format("Alert: Email for alert {0} on {1} sent",
                                     alert.Id, alert.Instance));
            alert.EmailStatus = NotificationStatus.Complete;
         }
         catch (Exception e)
         {
            string errorString;
            while (e != null)
            {
               errorString = e.Message;
               e = e.InnerException;
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                        String.Format("Alert: An error occurred when sending an email for alert {0} on {1}.  Exception: {2}",
                                                      alert.Id, alert.Instance, errorString),
                                        ErrorLog.Severity.Warning);
            }
            alert.EmailStatus = NotificationStatus.Failed;
         }
      }

        public static void PerformSnmpAction(DataAlert alert)
        {
            List<KeyValuePair<SNMPHelper.SqlCmVariable, string>> variableBindings = new List<KeyValuePair<SNMPHelper.SqlCmVariable, string>>();
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.AlertType, alert.AlertType.ToString("G")));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.Instance, alert.Instance));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.Created, alert.Created.ToString("G")));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.Level, alert.Level.ToString("G")));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.EventType, alert.EventType.ToString("G")));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.Message, alert.MessageBody));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.RuleName, alert.RuleName));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.MessageTitle, alert.MessageTitle));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.Severity, alert.EventLogSeverity.ToString("G")));
            variableBindings.Add(new KeyValuePair<SNMPHelper.SqlCmVariable, string>(SNMPHelper.SqlCmVariable.ComputerName, alert.ComputerName));

            VbCollection variableBindingCollection = SNMPHelper.CreateVariableBindings(variableBindings);

            try
            {
                SNMPHelper.SendSnmpTrap(variableBindingCollection, alert.ParentRule.SnmpConfiguration, false);
                alert.SnmpTrapStatus = NotificationStatus.Complete;
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                        String.Format("Alert: An error occurred when sending SNMP trap for alert {0} on {1}.  Exception: {2}", alert.Id, alert.Instance, e),
                                        ErrorLog.Severity.Warning);
                alert.SnmpTrapStatus = NotificationStatus.Failed;
            }
        }

      #endregion DataAlertActions

	    public static bool PerformSnmpTrapTest(SNMPConfiguration configuration, out string errorMessage)
	    {
	        errorMessage = string.Empty;

	        try
	        {
                SNMPHelper.SendSnmpTrap(null, configuration, true);
	            return true;
	        }
	        catch (Exception ex)
	        {
                while (ex != null)
                {
                    errorMessage = ex.Message;
                    ex = ex.InnerException;
                }
                return false;
	        }
	    }

      public static bool PerformSmtpTest(string recipient, string subject, string body, SmtpConfiguration config, out string errorString)
      {
         errorString = "" ;
         MailMessage msg ;

         msg = new MailMessage() ;

         msg.BodyFormat = MailFormat.Text ;
         msg.From = config.SenderAddress ;
         msg.Subject = subject ;
         msg.Body = body ;
         msg.To = recipient ;
         msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtsperver"] = config.Server ;
         msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"] = config.Port ;
         msg.Fields["http://schemas.microsoft.com/cdo/configuration/sendusing"]  = 2 ;
         msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"] = config.UseSsl.ToString() ;
         msg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"] = (int)config.Authentication;
         if(config.Authentication == SmtpAuthProtocol.Basic)
         {
            msg.Fields["http://schemas.microsoft.com/cdo/configuration/sendusername"] = config.Username ;
            msg.Fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"] = config.Password ;
         }

         try
         {
            SmtpMail.SmtpServer = config.Server;
            SmtpMail.Send(msg);
            return true ;
         }
         catch(Exception e)
         {
            while(e != null)
            {
               errorString = e.Message ;
               e = e.InnerException ;
            }
            return false ;
         }
      }
	}
}
