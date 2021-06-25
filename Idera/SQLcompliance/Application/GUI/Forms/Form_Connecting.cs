using System.Threading ;
using System.Windows.Forms ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_Connecting.
	/// </summary>
	public partial class Form_Connecting : Form
	{
		public Form_Connecting( string serverName )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			m_server = serverName;
			labelServer.Text = serverName;
         this.Invalidate();
			
         loadThread = new Thread(new ThreadStart(DoConnect));
         loadThread.Name = "Connecting";
         loadThread.Start();
         
      } 
      
      Thread         loadThread       = null;
      private string m_server = "";
      
      private void DoConnect()
      {
         this.Invalidate();
         
         try
         {
            this.Cursor = Cursors.WaitCursor;
            bool bConnected = Globals.Repository.OpenConnection( m_server );
            
            if ( bConnected )
               DialogResult = DialogResult.OK;
            else
               DialogResult = DialogResult.Cancel;
         }
         catch {}
         finally
         {         
            this.Cursor = Cursors.Default;
            this.Close();
         }
		}


	}
}
