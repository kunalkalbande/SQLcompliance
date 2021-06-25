using System ;
using System.Net ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_Connect.
	/// </summary>
	public partial class Form_Connect : Form
	{
		public Form_Connect()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         this.Icon = Resources.SQLcompliance_product_ico;
      }


      private void btnCancel_Click(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.Cancel;
         this.Close();
      }
      
      public string Server
      {
         get{ return txtServer.Text; }
         set{ txtServer.Text = value; }
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
      
         string server = txtServer.Text.Trim().ToUpper();
         if ( server == "." || server == "(LOCAL)" )
            server = Dns.GetHostName().ToUpper();
         txtServer.Text = server;
            
      
         this.DialogResult = DialogResult.OK;
         this.Close();
      }

      private void button1_Click(object sender, EventArgs e)
      {
         Form_SQLServerBrowse dlg = new Form_SQLServerBrowse(true);
         try
         {
            if (dlg.ShowDialog() == DialogResult.OK )
                  txtServer.Text = dlg.SelectedServer;
         }
         catch ( Exception ex )
         {
			   ErrorMessage.Show( UIConstants.Error_DMOLoadServers, ex.Message );
         }
      }

      #region Help
      //--------------------------------------------------------------------
      // Form_Connect_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_Connect_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
		   HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Form_Connect);
			hlpevent.Handled = true;
      }
      #endregion

      private void txtServer_TextChanged(object sender, EventArgs e)
      {
         if ( txtServer.Text.Trim().Length == 0 )
            btnOK.Enabled = false;
         else
            btnOK.Enabled = true;
      }
	}
}
