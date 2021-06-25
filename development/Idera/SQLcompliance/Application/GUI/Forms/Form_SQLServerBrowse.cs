using System ;
using System.Data ;
using System.Data.Sql ;
using System.Net ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Core.Agent ;
using System.Threading;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_SQLServerBrowse.
	/// </summary>
	public partial class Form_SQLServerBrowse : Form
	{
        private volatile bool _includeAuditedServers;
        private Thread _loaderThread;
        private string m_SelectedServer = string.Empty;

      #region Constructor / Dispose

      //-----------------------------------------------------------------------
      // Constructor
      //-----------------------------------------------------------------------
		public Form_SQLServerBrowse(bool includeAuditedServers)
		{
            _includeAuditedServers = includeAuditedServers;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         this.Icon = Resources.SQLcompliance_product_ico;

         _loaderThread = new Thread(LoadServers);
         _loaderThread.IsBackground = true;

         lblLoading.Visible = false;
         progLoading.Visible = false;
		    pnlLoading.Visible = false;
		}

		#endregion

        #region Properties

        public string SelectedServer
        {
            get { return m_SelectedServer; }
        }

        #endregion


      #region Load list logic

        private void LoadServers()
        {
            // save cancel state
            bool cancelEnabled = btnCancel.Enabled;
            btnCancel.Enabled = false;

            // loading logic
            try
            {

                SqlDataSourceEnumerator enumerator = SqlDataSourceEnumerator.Instance;
                using (DataTable tbl = enumerator.GetDataSources())
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        string server = row["ServerName"].ToString();
                        string instance = row["InstanceName"].ToString();
                        bool audited = true;

                        string fullName;

                        if (instance == null || instance.Length == 0)
                            fullName = server;
                        else
                            fullName = String.Format("{0}\\{1}", server, instance);

                        if (!_includeAuditedServers)
                        {
                            if (fullName.ToUpper() == "(LOCAL)")
                            {
                                fullName = Dns.GetHostName().ToUpper();
                            }
                            audited = ServerRecord.ServerIsAudited(fullName.ToUpper(),
                                Globals.Repository.Connection);
                        }

                        if (_includeAuditedServers || !audited)
                        {
                            listBoxServers.Items.Add(fullName.ToUpper());
                        }
                    }
                }
            }
            catch (ThreadAbortException threadAbortException)
            {
                // do nothing
            }
   		catch (Exception  ex) 
			{
			   ErrorMessage.Show( UIConstants.Error_DMOLoadServers, ex.Message );
			}
			finally
			{
               // set controls to normal state
			   this.Cursor = Cursors.Default;

			   btnCancel.Enabled = cancelEnabled; // set cancel state

               lblLoading.Visible = false;
               progLoading.Visible = false;
			    pnlLoading.Visible = false;
			}
		}
		
		#endregion
		
		#region Form Events

      //-----------------------------------------------------------------------
      // btnCancel_Click
      //-----------------------------------------------------------------------
      private void btnCancel_Click(object sender, EventArgs e)
      {
          if (_loaderThread.IsAlive)
              _loaderThread.Abort();

         this.Close();
      }

      //-----------------------------------------------------------------------
      // btnOK_Click
      //-----------------------------------------------------------------------
      private void btnOK_Click(object sender, EventArgs e)
      {
         m_SelectedServer = listBoxServers.SelectedItem.ToString();
         this.Close();
      }

        private void Form_SQLServerBrowse_Shown(object sender, EventArgs e)
        {
            // set controls to loading state
            lblLoading.Visible = true;
            progLoading.Visible = true;
            pnlLoading.Visible = true;

            Cursor = Cursors.WaitCursor;

            System.Windows.Forms.Application.DoEvents();

            _loaderThread.Start();
        }
      
      #endregion

      #region Help
      //--------------------------------------------------------------------
      // Form_SQLServerBrowse_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_SQLServerBrowse_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
		   HelpAlias.ShowHelp(this,HelpAlias.SSHELP_Form_SQLServerBrowse);
			hlpevent.Handled = true;
      }
      #endregion

      private void listBoxServers_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (listBoxServers.SelectedItems.Count == 1 )
            btnOK.Enabled = true;
         else
            btnOK.Enabled = false;
      }
    
	}
}
