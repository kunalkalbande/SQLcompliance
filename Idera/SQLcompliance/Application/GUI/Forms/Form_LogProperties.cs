using System ;
using System.Text ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Controls ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_LogProperties.
	/// </summary>
	public partial class Form_LogProperties : Form
	{

      private LogRecord       logRec     = null;
	   private BaseControl     parentView = null;
	   private int             logId      = -1;

		public Form_LogProperties(
            BaseControl inParentView,
		      int                inLogId
         )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;

		   parentView = inParentView;
         logId      = inLogId;
         
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
         int newId = parentView.Previous();
         logId     = newId;
         LoadLogRecord();
      }

      //--------------------------------------------------------------------
      // btnNext_Click
      //--------------------------------------------------------------------
      private void btnNext_Click(object sender, EventArgs e)
      {
         int newId = parentView.Next();
         logId     = newId;
         LoadLogRecord();
      }
      
      //--------------------------------------------------------------------
      // LoadLogRecord
      //--------------------------------------------------------------------
      private void LoadLogRecord()
      {
         Cursor = Cursors.WaitCursor;
         
         string details = "";
         bool readFailed = false;
         
         if ( logId == -1 )
         {
            details = UIConstants.Prop_NotDataRow;
            readFailed = true;
         }
         else
         {
            logRec = new LogRecord();
            
            try
            {
               logRec.Read( Globals.Repository.Connection, logId );


               DateTime local    = logRec.eventTime;
               txtTime.Text      = local.ToLocalTime().ToString() ;
               txtUser.Text      = logRec.logUser;
               txtType.Text      = logRec.logTypeString;
               txtSQLServer.Text = logRec.logSqlServer;

               textDetails.Text   = logRec.logInfo;
            }
            catch ( Exception ex)
            {
               details = ex.Message;
               readFailed = true;
            }
         }
         
         if ( readFailed )
         {
            txtTime.Text      = "";
            txtUser.Text      = "";
            txtSQLServer.Text = "";
            txtType.Text      = "";
            
            textDetails.Text = details;
         }
         
         Cursor = Cursors.Default;
      }

      #region Help
      //--------------------------------------------------------------------
      // Form_LogProperties_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_LogProperties_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
		   HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Form_ChangeLogProperties);
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
         s.Append ("User:\t\t"         + txtUser.Text      + "\r\n");
         s.Append ("SQL Server:\t\t"   + txtSQLServer.Text + "\r\n");
         
         s.Append ("Details:\r\n");
         s.Append( textDetails.Text );
         
         Clipboard.SetDataObject( s.ToString() );
      }
	}
}
