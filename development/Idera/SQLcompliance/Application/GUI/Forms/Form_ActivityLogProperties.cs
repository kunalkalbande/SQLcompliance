using System;
using System.Text;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Controls;
using Idera.SQLcompliance.Application.GUI.Properties;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   public partial class Form_ActivityLogProperties : Form
   {
      private ActivityLogRow _record = null;
      private ActivityLogView _parent = null;

      public Form_ActivityLogProperties(ActivityLogView inParentView,ActivityLogRow record)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

		   _parent = inParentView;
         _record = record;
         LoadLogRecord();
		}


      //--------------------------------------------------------------------
      // btnClose_Click
      //--------------------------------------------------------------------
      private void btnClose_Click(object sender, EventArgs e)
      {
         this.Close();
      }

      //--------------------------------------------------------------------
      // btnPrevious_Click
      //--------------------------------------------------------------------
      private void btnPrevious_Click(object sender, EventArgs e)
      {
         _record = _parent.PreviousRecord();
         LoadLogRecord();
      }

      //--------------------------------------------------------------------
      // btnNext_Click
      //--------------------------------------------------------------------
      private void btnNext_Click(object sender, EventArgs e)
      {
         _record = _parent.NextRecord();
         LoadLogRecord();
      }
      
      //--------------------------------------------------------------------
      // LoadLogRecord
      //--------------------------------------------------------------------
      private void LoadLogRecord()
      {
         DateTime local    = _record.StartTime;
         txtTime.Text      = local.ToLocalTime().ToString() ;
         string sEventType = Globals.Repository.LookupAgentLogEventType(_record.EventTypeId);
         txtType.Text = sEventType == null ? _record.EventTypeId.ToString() : sEventType;
         txtSQLServer.Text = _record.Instance;
         if (_record.Details != null)
            textDetails.Text = _record.Details.Replace("\n", "\r\n");
         else
            textDetails.Text = "";
      }

      #region Help
      //--------------------------------------------------------------------
      // Form_LogProperties_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_LogProperties_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_ActivityLogProperties);
			hlpevent.Handled = true;
      }
      #endregion

      //--------------------------------------------------------------------
      // btnCopy_Click
      //--------------------------------------------------------------------
      private void btnCopy_Click(object sender, EventArgs e)
      {
         StringBuilder s = new StringBuilder("",1024);
         
         s.Append ("Event time:\t"     + txtTime.Text      + "\r\n");
         s.Append ("Event type:\t"     + txtType.Text      + "\r\n");
         s.Append ("SQL Server:\t\t"   + txtSQLServer.Text + "\r\n");
         
         s.Append ("Details:\r\n");
         s.Append( textDetails.Text );
         
         Clipboard.SetDataObject( s.ToString() );
      }
   }
}