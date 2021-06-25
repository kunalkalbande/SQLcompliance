using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLcompliance.Core.Rules.Alerts;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   public partial class Form_ChangeThreshold : Form
   {
      StatusAlertRule _rule;

      public Form_ChangeThreshold(StatusAlertRule rule)
      {
         InitializeComponent();

         if (rule != null)
         {
            _rule = rule;
         }
         else
         {
            _rule = new StatusAlertRule();
         }
         Initialize();
      }

      public int Threshold
      {
         get { return Convert.ToInt32(this.thresholdNumeric.Value); }
      }

      private void Initialize()
      {
         switch (_rule.StatusType)
         {
            case StatusRuleType.TraceDirFullAgent:
               {
                  this.thresholdDescLabel.Text = "Max percentage of trace directory used:";
                  this.thresholdNumeric.DecimalPlaces = 0;
                  this.thresholdNumeric.Minimum = 1;
                  this.thresholdNumeric.Maximum = 100;
                  this.ID_OK.Location = new Point(ID_OK.Location.X + 42, ID_OK.Location.Y);
                  this.ID_CANCEL.Location = new Point(ID_CANCEL.Location.X + 42, ID_CANCEL.Location.Y);
                  this.thresholdNumeric.Location = new Point(thresholdNumeric.Location.X + 40, this.thresholdNumeric.Location.Y);
                  this.Size = new Size(this.Size.Width + 39, this.Size.Height);

                  if (_rule.Threshold.Value != 0)
                  {
                     this.thresholdNumeric.Value = _rule.Threshold.Value;
                  }
                  else
                  {
                     this.thresholdNumeric.Value = StatusAlertRule.DefaultAgentTrcDirPercentFull;
                  }
                  break;
               }
            case StatusRuleType.TraceDirFullCollect:
               {
                  this.thresholdDescLabel.Text = "Trace File Directory Size (GB):";
                  this.thresholdNumeric.DecimalPlaces = 0;
                  this.thresholdNumeric.Minimum = 1;
                  this.thresholdNumeric.Maximum = 10000;

                  if (_rule.Threshold.Value != 0)
                  {
                     this.thresholdNumeric.Value = _rule.Threshold.Value;
                  }
                  else
                  {
                     this.thresholdNumeric.Value = StatusAlertRule.DefaultCollectSvrTrcDirSize;
                  }
                  break;
               }
            case StatusRuleType.RepositoryTooBig:
               {
                  this.thresholdDescLabel.Text = "Database Size:";
                  this.thresholdNumeric.DecimalPlaces = 0;
                  this.thresholdNumeric.Minimum = 1;
                  this.thresholdNumeric.Maximum = 10000;
                  this.ID_OK.Location = new Point(ID_OK.Location.X - 20, ID_OK.Location.Y);
                  this.ID_CANCEL.Location = new Point(ID_CANCEL.Location.X - 20, ID_CANCEL.Location.Y);
                  this.thresholdDescLabel.Location = new Point(this.thresholdDescLabel.Location.X + 30, this.thresholdDescLabel.Location.Y);
                  this.thresholdNumeric.Location = new Point(ID_CANCEL.Location.X, this.thresholdNumeric.Location.Y);
                  this.Size = new Size(this.Size.Width - 10, this.Size.Height);

                  if (_rule.Threshold.Value != 0)
                  {
                     this.thresholdNumeric.Value = _rule.Threshold.Value;
                  }
                  else
                  {
                     this.thresholdNumeric.Value = StatusAlertRule.DefaultRepositorySize;
                  }
                  break;
               }
         }
      }
   }
}