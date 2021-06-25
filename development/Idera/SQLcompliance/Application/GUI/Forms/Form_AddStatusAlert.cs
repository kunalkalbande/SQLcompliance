using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Rules.Alerts;
using System.Diagnostics;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   public partial class Form_AddStatusAlert : Form
   {
      readonly string _traceDirFullAgentDesc = "The SQLcompliance Agent trace directory on the audited SQL Server computer reached the specified percentage of its size limit.";
      readonly string _traceDirFullCollectDesc = "The Collection Server trace directory reached the specified size limit.";
      readonly string _noHeartbeatsDesc = "The Collection Server has not received a heartbeat from one of the deployed SQLcompliance Agents.";
      readonly string _repositoryTooBigDesc = "An event database for an audited SQL Server instance reached the specified maximum size.";
      readonly string _sqlServerDownDesc = "The SQLcompliance Agent cannot connect to one of the audited SQL Server instances.";

      private Graphics _rtfGraphics ;
      private StatusAlertRule _rule;
      private LinkString _link;
      private List<string> _ruleNames = new List<string>();
      private AlertingConfiguration _configuration = new AlertingConfiguration();

      enum WizardPage
      {
         StartPage,
         AlertActions,
         FinishPage
      }

      public StatusAlertRule StatusRule
      {
         get { return this._rule; }
      }

      public string RuleText
      {
         get { return this.alertRuleRtfTextbox.Text; }
      }

      public List<string> RuleNames
      {
         set { this._ruleNames = value; }
      }

      private WizardPage currentPage = WizardPage.StartPage;

      /// <summary>
      /// 
      /// </summary>
      public Form_AddStatusAlert(StatusAlertRule rule, List<string> ruleNames, bool fromTemplate)
      {
         InitializeComponent();

         if (rule == null)
         {
            this.Text = "New Status Alert Rule";
            this._rule = new StatusAlertRule();
            this._rule.Enabled = true;
            this._rule.StatusType = StatusRuleType.TraceDirFullAgent;
            this._rule.Level = AlertLevel.Medium;
            this._rule.Threshold.Value = 50;
         }
         else
         {
            this._rule = rule;

            if (fromTemplate)
               this.Text = "New Status Alert Rule";
            else
               this.Text = "Edit Status Alert Rule";
         }
         this._ruleNames = ruleNames;
         Initialize();
      }

      private void Initialize()
      {
         this._rtfGraphics = this.alertRuleRtfTextbox.CreateGraphics();

         this.ruleDescriptionTextbox.Text = this._rule.Description;
         this.ruleNameTextbox.Text = this._rule.Name;
         this.alertLevelCombo.SelectedIndex = ((int)this._rule.Level) - 1;
         this.eventLogCheckbox.Checked = this._rule.HasLogAction;
         this.emailCheckbox.Checked = this._rule.HasEmailAction;
          snmpTrapCheckbox.Checked = _rule.HasSnmpTrapAction;
         this.ruleEnabledCheckbox.Checked = this._rule.Enabled;

         InitRuleCombo();
         SetSelectedRuleType();
         SetDescriptionText();
         ShowPage();
      }

      /// <summary>
      /// 
      /// </summary>
      private void InitRuleCombo()
      {
         ValueListItem item = null;
         int index = 1;
         
         foreach (string name in _ruleNames)
         {
            item = new ValueListItem((StatusRuleType)index++, name);
            ruleCombo.Items.Add(item);
         }
      }

      /// <summary>
      /// 
      /// </summary>
      private void ShowPage()
      {
         addOpAlertWizard.Tabs[(int)currentPage].Selected = true;
         UpdateView();
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void previousButton_Click(object sender, EventArgs e)
      {
         currentPage--;
         ShowPage();
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void nextButton_Click(object sender, EventArgs e)
      {
         //increment to the next page.
         currentPage++;

         if (currentPage > WizardPage.FinishPage)
         {
            if (SaveRule())
            {
               this.DialogResult = DialogResult.OK;
               this.Close();
            }
            else
            {
               currentPage--;
            }
         }
         else
         {
            ShowPage();
         }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      private bool SaveRule()
      {
         this._rule.Name = this.ruleNameTextbox.Text;
         this._rule.Description = this.ruleDescriptionTextbox.Text;
         this._rule.Enabled = this.ruleEnabledCheckbox.Checked;
         return true;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void buttonCancel_Click(object sender, EventArgs e)
      {
         this.Close();
      }

      /// <summary>
      /// 
      /// </summary>
      void UpdateView()
      {
         switch (currentPage)
         {
            case WizardPage.StartPage:
               {
                  this.previousButton.Enabled = false;
                  this.nextButton.Enabled = true;
                  this.cancelButton.Enabled = true;
                  this.titleLabel.Text = "Status Alert Type";
                  this.descLabel.Text = "Select the status alert type that you want to monitor.";
                  UpdateRuleText();
                  break;
               }
            case WizardPage.AlertActions:
               {
                  this.previousButton.Enabled = true;
                  this.nextButton.Enabled = true;
                  this.nextButton.Text = "Next";
                  this.cancelButton.Enabled = true;
                  this.titleLabel.Text = "Alert Actions";
                  this.descLabel.Text = "Select the action to be taken when a SQL Compliance Manager status matches this rule.";
                  UpdateRuleText();
                  break;
               }
            case WizardPage.FinishPage:
               {
                  this.previousButton.Enabled = true;
                  this.nextButton.Enabled = true;
                  this.nextButton.Text = "Finish";
                  this.cancelButton.Enabled = true;
                  this.titleLabel.Text = "Finish Status Alert Rule";
                  this.descLabel.Text = "Specify the name for this rule and select the categorization level for the alert. Also, \r\nselect whether to enable this rule now.";
                  UpdateRuleText();
                  break;
               }
            default:
               {
                  this.previousButton.Enabled = false;
                  this.nextButton.Enabled = false;
                  this.cancelButton.Enabled = false;
                  break;
               }
         }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="s"></param>
      /// <returns></returns>
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

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void RuleCombo_SelectedIndexChanged(object sender, EventArgs e)
      {
         ValueListItem selectedItem = (ValueListItem)((ComboBox)sender).SelectedItem;

         //if this is the case, the combo is just being initialized.
         if ((StatusRuleType)selectedItem.DataValue == _rule.StatusType)
            return;

         if (selectedItem != null)
         {
            this._rule.StatusType = (StatusRuleType)selectedItem.DataValue;
            SetDescriptionText();

            switch (_rule.StatusType)
            {
               case StatusRuleType.RepositoryTooBig:
                  {
                     this._rule.Threshold.Value = StatusAlertRule.DefaultRepositorySize;
                     break;
                  }
               case StatusRuleType.TraceDirFullAgent:
                  {
                     this._rule.Threshold.Value = StatusAlertRule.DefaultAgentTrcDirPercentFull;
                     break;
                  }
               case StatusRuleType.TraceDirFullCollect:
                  {
                     this._rule.Threshold.Value = StatusAlertRule.DefaultCollectSvrTrcDirSize;
                     break;
                  }
               default:
                  {
                     this._rule.Threshold.Value = 0;
                     break;
                  }
            }
            UpdateRuleText();
         }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="ruleType"></param>
      private void SetDescriptionText()
      {
         switch (_rule.StatusType)
         {
            case StatusRuleType.NoHeartbeats:
               {
                  this.ruleDescription.Text = this._noHeartbeatsDesc;
                  break;
               }
            case StatusRuleType.RepositoryTooBig:
               {
                  this.ruleDescription.Text = this._repositoryTooBigDesc;
                  break;
               }
            case StatusRuleType.SqlServerDown:
               {
                  this.ruleDescription.Text = this._sqlServerDownDesc;
                  break;
               }
            case StatusRuleType.TraceDirFullAgent:
               {
                  this.ruleDescription.Text = this._traceDirFullAgentDesc;
                  break;
               }
            case StatusRuleType.TraceDirFullCollect:
               {
                  this.ruleDescription.Text = this._traceDirFullCollectDesc;
                  break;
               }
         }
      }

      /// <summary>
      /// 
      /// </summary>
      private void HideFilters()
      {
         switch (this._rule.StatusType)
         {
            case StatusRuleType.TraceDirFullAgent:
               {
                  this.dbTooBigPanel.Visible = false;
                  this.trcDirAgentPanel.Visible = true;
                  this.trcDirCollectPanel.Visible = false;
                  break;
               }
            case StatusRuleType.TraceDirFullCollect:
               {
                  this.dbTooBigPanel.Visible = false;
                  this.trcDirAgentPanel.Visible = false;
                  this.trcDirCollectPanel.Visible = true;
                  break;
               }
            case StatusRuleType.RepositoryTooBig:
               {
                  this.dbTooBigPanel.Visible = true;
                  this.trcDirAgentPanel.Visible = false;
                  this.trcDirCollectPanel.Visible = false;
                  break;
               }
            default:
               {
                  this.dbTooBigPanel.Visible = false;
                  this.trcDirAgentPanel.Visible = false;
                  this.trcDirCollectPanel.Visible = false;
                  break;
               }
         }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void AlertRuleRtf_MouseMove(object sender, MouseEventArgs e)
      {
         if (!Globals.isAdmin)
            return;

         LinkSegment seg = this._link.LinkHitTest(e.X, e.Y, this.alertRuleRtfTextbox, this._rtfGraphics);
         
         if (seg != null && seg.Tag != null)
         {
            alertRuleRtfTextbox.Cursor = Cursors.Hand;
            Cursor.Current = Cursors.Hand;
         }
         else
         {
            alertRuleRtfTextbox.Cursor = Cursors.Default;
            Cursor.Current = Cursors.Default;
         }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void AlertRuleRtf_MouseDown(object sender, MouseEventArgs e)
      {
         LinkSegment link = _link.LinkHitTest(e.X, e.Y, sender as RichTextBox, _rtfGraphics);

         if (link == null || link.Tag == null || e.Button != MouseButtons.Left || !Globals.isAdmin)
            return;
         else if (link.Tag is string)
         {
            string tagString = (string)link.Tag;

            if (String.Equals(tagString, "AlertLevel"))
            {
               Form_AlertLevel frmLevel = new Form_AlertLevel();
               frmLevel.Level = this._rule.Level;
               if (frmLevel.ShowDialog(this) == DialogResult.OK)
               {
                  this._rule.Level = frmLevel.Level;
                  this.alertLevelCombo.SelectedIndex = ((int)_rule.Level) - 1;
                  UpdateRuleText();
               }
            }
            else if (String.Equals(tagString, "Threshold"))
            {
               Form_ChangeThreshold frmThreshold = new Form_ChangeThreshold(_rule);

               if (frmThreshold.ShowDialog(this) == DialogResult.OK)
               {
                  this._rule.Threshold.Value = frmThreshold.Threshold;
                  UpdateRuleText();
               }
            }
            else if (String.Equals(tagString, "AlertMessage"))
            {
               Form_EmailTemplate options;

               options = new Form_EmailTemplate(_configuration.StatusMacros);
               options.Subject = _rule.MessageTitle;
               options.Message = _rule.MessageBody;
               if (options.ShowDialog(this) == DialogResult.OK)
               {
                  _rule.MessageTitle = options.Subject;
                  _rule.MessageBody = options.Message;
               }
            }
            else if (String.Equals(tagString, "Recipients"))
            {
               Form_EmailList emailForm = new Form_EmailList();
               emailForm.EmailArray = _rule.Recipients;
               if (emailForm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
               {
                  _rule.Recipients = emailForm.EmailArray;
                  UpdateRuleText();
               }
            }
            else if (String.Equals(tagString, "LogEntryType"))
            {
               Form_EventLogType logTypeForm = new Form_EventLogType();

               logTypeForm.EntryType = _rule.LogEntryType;
               if (logTypeForm.ShowDialog(this) == DialogResult.OK)
               {
                  _rule.LogEntryType = logTypeForm.EntryType;
                  UpdateRuleText();
               }
            }
            else if (String.Equals(tagString, "SnmpConfigurations"))
            {
                // set default SNMP configurations if missing
                if (_rule.SnmpConfiguration == null)
                    _rule.SnmpConfiguration = AlertingDal.SelectSnmpConfiguration(Globals.Repository.Connection.ConnectionString);

                Form_AlertingOptions_Snmp frmSnmpConfigurations = new Form_AlertingOptions_Snmp(_rule.SnmpConfiguration);
                if (frmSnmpConfigurations.ShowDialog(this) == DialogResult.OK)
                {
                    _rule.SnmpConfiguration = frmSnmpConfigurations.SnmpConfiguration;
                    UpdateRuleText();
                }
            }
         }
      }

      /// <summary>
      /// 
      /// </summary>
      private void SetSelectedRuleType()
      {
         foreach (ValueListItem item in ruleCombo.Items)
         {
            if ((StatusRuleType)item.DataValue == _rule.StatusType)
            {
               ruleCombo.SelectedItem = item;
               break;
            }
         }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void EventLogCheckbox_CheckChanged(object sender, EventArgs e)
      {
         _rule.HasLogAction = this.eventLogCheckbox.Checked;
         UpdateRuleText();
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void EmailCheckbox_CheckChanged(object sender, EventArgs e)
      {
         _rule.HasEmailAction = this.emailCheckbox.Checked;
         UpdateRuleText();
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void SnmpTrapCheckbox_CheckChanged(object sender, EventArgs e)
      {
          _rule.HasSnmpTrapAction = this.snmpTrapCheckbox.Checked;
          UpdateRuleText();
      }

      /// <summary>
      /// 
      /// </summary>
      private void UpdateRuleText()
      {
         this.alertRuleRtfTextbox.Rtf = AlertUIHelper.GenerateRuleDescription(_rule, out _link);
      }

      private void AlertLevelCombo_SelectedIndexChanged(object sender, EventArgs e)
      {
         _rule.Level = (AlertLevel)(this.alertLevelCombo.SelectedIndex + 1);
         UpdateRuleText();
      }

      private void Form_AddStatusAlert_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
      {
         string helpTopic = "";

         switch (currentPage)
         {
            case WizardPage.StartPage:
               helpTopic = HelpAlias.SSHELP_NewStatusAlertWizard_Type;
               break;
            case WizardPage.AlertActions:
               helpTopic = HelpAlias.SSHELP_NewStatusAlertWizard_Action;
               break;
            case WizardPage.FinishPage:
               helpTopic = HelpAlias.SSHELP_NewStatusAlertWizard_Finish;
               break;
         }

         if (helpTopic != "") 
              HelpAlias.ShowHelp(this, helpTopic);
         hlpevent.Handled = true;
      }
   }
}