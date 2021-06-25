using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   public partial class Form_IndexSchedule : Form
   {
      private DateTime indexStartTime = DateTime.MinValue;
      private TimeSpan indexDuration = new TimeSpan();

      public DateTime IndexStartTime
      {
         get { return indexStartTime; }
         set 
         {
            if (value != DateTime.MinValue)
            {
               indexStartTime = value;
               indexStartTimeCombo.DateTime = indexStartTime;
            }
         }
      }

      public TimeSpan IndexDuration
      {
         get { return indexDuration; }
         set 
         { 
            indexDuration = value;
            indexDurationCombo.Time = indexDuration;
            if (indexDurationCombo.Time == TimeSpan.Zero)
               disableScheduleCheckbox.Checked = true;
         }
      }

      public Form_IndexSchedule()
      {
         InitializeComponent();
      }

      private void IDOK_Click(object sender, EventArgs e)
      {
         indexDuration = indexDurationCombo.Time;
         indexStartTime = indexStartTimeCombo.DateTime;
         this.Close();
      }

      private void IndexDurationCombo_ValueChanged(object sender, EventArgs e)
      {
         if (indexDurationCombo.Time != TimeSpan.Zero)
         {
            IDOK.Enabled = true;
            disableScheduleCheckbox.Checked = false;
         }
         else
         {
            disableScheduleCheckbox.Checked = true;
            IDOK.Enabled = true;
         }
      }

      private void LinkClick_HelpLink(object sender, LinkLabelLinkClickedEventArgs e)
      {
         HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Index_Schedule_Information);
      }

      private void Form_IndexSchedule_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Index_Schedule_Information);
      }

      private void disableScheduleCheckbox_CheckedChanged(object sender, EventArgs e)
      {
         if (disableScheduleCheckbox.Checked)
         {
            IDOK.Enabled = true;
            indexDurationCombo.Time = TimeSpan.Zero;
         }
         else
         {
            if (indexDurationCombo.Time != TimeSpan.Zero)
            {
               IDOK.Enabled = true;
            }
            else
            {
               IDOK.Enabled = false;
            }
         }
      }
   }
}