using System ;
using System.Text ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Status ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_SnapshotOptions.
	/// </summary>
	public partial class Form_SnapshotOptions : Form
	{
      private string oldSnapshot;
      private int    oldInterval;

		public Form_SnapshotOptions()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

			Cursor = Cursors.WaitCursor;

         int      snapshotInterval;
         DateTime snapshotLastTime;
         
         Snapshot.GetSnapshotSettings( Globals.Repository.Connection,
                                       out snapshotInterval,
                                       out snapshotLastTime );
         if ( snapshotInterval <= 0 )
         {
            radioDoNotCapture.Checked = true;
            textInterval.Text = "30";
         }
         else
         {
            radioSnapshotInterval.Checked = true;
            textInterval.Text = snapshotInterval.ToString();
         }
         
         oldSnapshot = CreateSnapshot( snapshotInterval );
         oldInterval = snapshotInterval;
         
         //------------------------------------------------------
         // Make controls read only unless user has admin access
         //------------------------------------------------------
         if ( ! Globals.isAdmin  )
         {
            foreach ( Control ctrl in this.Controls )
            {
               ctrl.Enabled = false;
            }

            // change buttons
            btnOK.Visible = false;
            btnCancel.Text = "Close";
            btnCancel.Enabled = true;
            this.AcceptButton = btnCancel;
         }

			Cursor = Cursors.Default;
		}


      private void btnCancel_Click(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.Cancel;
         this.Close();
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         // validate values
         int snapshotInterval = 0;
         if ( radioSnapshotInterval.Checked )
         {
            snapshotInterval = UIUtils.TextToInt(textInterval.Text);
            
            if ( snapshotInterval < 1 || snapshotInterval > 999 )
            {
               ErrorMessage.Show( this.Text, UIConstants.Error_IllegalSnapshotPeriod );
               return;
            }
         }
         
         if ( oldInterval != snapshotInterval )
         {
            Snapshot.UpdateSnapshotInterval( Globals.Repository.Connection,
                                             snapshotInterval );
                                             
			   string newSnapshot = CreateSnapshot( snapshotInterval);
   			
			   StringBuilder log = new StringBuilder(1024);
   			
			   log.Append("Audit Snapshot Schedule Changed\r\n\r\n");
			   log.Append("Old Setting\r\n");
			   log.Append(oldSnapshot);
			   log.Append("\r\n");
			   log.Append("New Setting\r\n");
			   log.Append(newSnapshot);
         
            LogRecord.WriteLog( Globals.Repository.Connection,
                                 LogType.AuditSnapshotSchedule,
                                 log.ToString() );
         }
         
         this.DialogResult = DialogResult.OK;
         this.Close();
      }
      
      private string
         CreateSnapshot(
            int   interval
         )
      {
			StringBuilder log = new StringBuilder(1024);
			
         if ( interval == 0 )
         {
            log.Append( "Audit snapshots will not be captured on a scheduled basis.");
         }
         else
         {
            log.AppendFormat( "Audit snapshots will be captured every {0} days.", interval);
         }
         
         return log.ToString();
      }
      

      #region Help
      //--------------------------------------------------------------------
      // Form_Defaults_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_Defaults_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
		   HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Form_SnapshotOptions);
			hlpevent.Handled = true;
      }
      #endregion

      private void radioSnapshotInterval_CheckedChanged(object sender, EventArgs e)
      {
         textInterval.Enabled = (radioSnapshotInterval.Checked);
      }
	}
}
