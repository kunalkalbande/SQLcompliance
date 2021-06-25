using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace Idera.SQLcompliance.Cluster
{
	/// <summary>
	/// Summary description for Form_Properties.
	/// </summary>
	public class Form_Properties : System.Windows.Forms.Form
	{
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox textSqlServer;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		
		
		private string m_oldCollectionServer = "";
      private System.Windows.Forms.TextBox textCollectionServer;
      private System.Windows.Forms.TextBox textTraceDirectory;
		private string m_oldTraceDirectory   = "";
      private string m_oldAssemblyDirectory = "";
      private TextBox textAssemblyDirectory;
		private System.Windows.Forms.TextBox _txtReplicatedRegistryKey;
		private System.Windows.Forms.TextBox _txtServiceName;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
      private System.Windows.Forms.Button btnCopyService;
      private System.Windows.Forms.Button btnCopyRegistry;
      private Label label6;
		private VirtualServer m_server; 

		public Form_Properties(VirtualServer server, bool allowEdit)
		{
			m_server = server ;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// TODO: read old values; store in old properties
			
			textSqlServer.Text = server.FullInstanceName ;
			m_oldCollectionServer = server.CollectionServer;
			textCollectionServer.Text = m_oldCollectionServer ;
			m_oldTraceDirectory = server.TraceDirectory;
			textTraceDirectory.Text = m_oldTraceDirectory ;
         m_oldAssemblyDirectory = server.TriggerAssemblyDirectory;
         textAssemblyDirectory.Text = m_oldAssemblyDirectory;
			_txtServiceName.Text = server.ServiceName ;
			_txtReplicatedRegistryKey.Text = String.Format(@"SOFTWARE\Idera\SQLCM\{0}", server.ServiceName) ;

			if(!allowEdit)
			{
				btnOK.Visible = false ;
				btnCancel.Text = "OK" ;
				textCollectionServer.ReadOnly = true ;
				textTraceDirectory.ReadOnly = true ;
            textAssemblyDirectory.ReadOnly = true;
			}else
				btnOK.Focus() ;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Properties));
this.label1 = new System.Windows.Forms.Label();
this.textSqlServer = new System.Windows.Forms.TextBox();
this.textCollectionServer = new System.Windows.Forms.TextBox();
this.label2 = new System.Windows.Forms.Label();
this.textTraceDirectory = new System.Windows.Forms.TextBox();
this.label3 = new System.Windows.Forms.Label();
this.btnOK = new System.Windows.Forms.Button();
this.btnCancel = new System.Windows.Forms.Button();
this._txtReplicatedRegistryKey = new System.Windows.Forms.TextBox();
this._txtServiceName = new System.Windows.Forms.TextBox();
this.label4 = new System.Windows.Forms.Label();
this.label5 = new System.Windows.Forms.Label();
this.btnCopyService = new System.Windows.Forms.Button();
this.btnCopyRegistry = new System.Windows.Forms.Button();
this.label6 = new System.Windows.Forms.Label();
this.textAssemblyDirectory = new System.Windows.Forms.TextBox();
this.SuspendLayout();
// 
// label1
// 
this.label1.Location = new System.Drawing.Point(9, 12);
this.label1.Name = "label1";
this.label1.Size = new System.Drawing.Size(76, 16);
this.label1.TabIndex = 0;
this.label1.Text = "SQL Server:";
// 
// textSqlServer
// 
this.textSqlServer.Location = new System.Drawing.Point(173, 8);
this.textSqlServer.Name = "textSqlServer";
this.textSqlServer.ReadOnly = true;
this.textSqlServer.Size = new System.Drawing.Size(300, 20);
this.textSqlServer.TabIndex = 1;
this.textSqlServer.TabStop = false;
this.textSqlServer.Text = "txtInstance";
// 
// textCollectionServer
// 
this.textCollectionServer.Location = new System.Drawing.Point(173, 36);
this.textCollectionServer.Name = "textCollectionServer";
this.textCollectionServer.Size = new System.Drawing.Size(300, 20);
this.textCollectionServer.TabIndex = 3;
// 
// label2
// 
this.label2.Location = new System.Drawing.Point(9, 40);
this.label2.Name = "label2";
this.label2.Size = new System.Drawing.Size(152, 16);
this.label2.TabIndex = 2;
this.label2.Text = "Collection Server Computer:";
// 
// textTraceDirectory
// 
this.textTraceDirectory.Location = new System.Drawing.Point(173, 64);
this.textTraceDirectory.Name = "textTraceDirectory";
this.textTraceDirectory.Size = new System.Drawing.Size(300, 20);
this.textTraceDirectory.TabIndex = 5;
// 
// label3
// 
this.label3.Location = new System.Drawing.Point(9, 68);
this.label3.Name = "label3";
this.label3.Size = new System.Drawing.Size(144, 16);
this.label3.TabIndex = 4;
this.label3.Text = "Agent Trace Directory:";
// 
// btnOK
// 
this.btnOK.Location = new System.Drawing.Point(317, 183);
this.btnOK.Name = "btnOK";
this.btnOK.Size = new System.Drawing.Size(75, 23);
this.btnOK.TabIndex = 14;
this.btnOK.Text = "&OK";
this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
// 
// btnCancel
// 
this.btnCancel.Location = new System.Drawing.Point(397, 183);
this.btnCancel.Name = "btnCancel";
this.btnCancel.Size = new System.Drawing.Size(75, 23);
this.btnCancel.TabIndex = 15;
this.btnCancel.Text = "&Cancel";
this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
// 
// _txtReplicatedRegistryKey
// 
this._txtReplicatedRegistryKey.Location = new System.Drawing.Point(173, 151);
this._txtReplicatedRegistryKey.Name = "_txtReplicatedRegistryKey";
this._txtReplicatedRegistryKey.ReadOnly = true;
this._txtReplicatedRegistryKey.Size = new System.Drawing.Size(300, 20);
this._txtReplicatedRegistryKey.TabIndex = 12;
// 
// _txtServiceName
// 
this._txtServiceName.Location = new System.Drawing.Point(173, 123);
this._txtServiceName.Name = "_txtServiceName";
this._txtServiceName.ReadOnly = true;
this._txtServiceName.Size = new System.Drawing.Size(300, 20);
this._txtServiceName.TabIndex = 9;
// 
// label4
// 
this.label4.Location = new System.Drawing.Point(9, 155);
this.label4.Name = "label4";
this.label4.Size = new System.Drawing.Size(144, 16);
this.label4.TabIndex = 11;
this.label4.Text = "Replicated Registry Key:";
// 
// label5
// 
this.label5.Location = new System.Drawing.Point(9, 127);
this.label5.Name = "label5";
this.label5.Size = new System.Drawing.Size(152, 16);
this.label5.TabIndex = 8;
this.label5.Text = "Agent Service Name:";
// 
// btnCopyService
// 
this.btnCopyService.Image = ((System.Drawing.Image)(resources.GetObject("btnCopyService.Image")));
this.btnCopyService.Location = new System.Drawing.Point(481, 123);
this.btnCopyService.Name = "btnCopyService";
this.btnCopyService.Size = new System.Drawing.Size(28, 20);
this.btnCopyService.TabIndex = 10;
this.btnCopyService.Click += new System.EventHandler(this.btnCopyService_Click);
// 
// btnCopyRegistry
// 
this.btnCopyRegistry.Image = ((System.Drawing.Image)(resources.GetObject("btnCopyRegistry.Image")));
this.btnCopyRegistry.Location = new System.Drawing.Point(481, 151);
this.btnCopyRegistry.Name = "btnCopyRegistry";
this.btnCopyRegistry.Size = new System.Drawing.Size(28, 20);
this.btnCopyRegistry.TabIndex = 13;
this.btnCopyRegistry.Click += new System.EventHandler(this.btnCopyRegistry_Click);
// 
// label6
// 
this.label6.AutoSize = true;
this.label6.Location = new System.Drawing.Point(9, 97);
this.label6.Name = "label6";
this.label6.Size = new System.Drawing.Size(159, 13);
this.label6.TabIndex = 6;
this.label6.Text = "CLR Trigger Assembly Directory:";
// 
// textAssemblyDirectory
// 
this.textAssemblyDirectory.Location = new System.Drawing.Point(173, 94);
this.textAssemblyDirectory.Name = "textAssemblyDirectory";
this.textAssemblyDirectory.Size = new System.Drawing.Size(300, 20);
this.textAssemblyDirectory.TabIndex = 7;
// 
// Form_Properties
// 
this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
this.ClientSize = new System.Drawing.Size(519, 219);
this.Controls.Add(this.textAssemblyDirectory);
this.Controls.Add(this.label6);
this.Controls.Add(this.btnCopyRegistry);
this.Controls.Add(this.btnCopyService);
this.Controls.Add(this._txtReplicatedRegistryKey);
this.Controls.Add(this._txtServiceName);
this.Controls.Add(this.textTraceDirectory);
this.Controls.Add(this.textCollectionServer);
this.Controls.Add(this.textSqlServer);
this.Controls.Add(this.label4);
this.Controls.Add(this.label5);
this.Controls.Add(this.btnCancel);
this.Controls.Add(this.btnOK);
this.Controls.Add(this.label3);
this.Controls.Add(this.label2);
this.Controls.Add(this.label1);
this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
this.HelpButton = true;
this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
this.MaximizeBox = false;
this.MinimizeBox = false;
this.Name = "Form_Properties";
this.ShowInTaskbar = false;
this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
this.Text = "SQLcompliance Agent Details";
this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_Properties_HelpRequested);
this.ResumeLayout(false);
this.PerformLayout();

      }
		#endregion

      private void btnCancel_Click(object sender, System.EventArgs e)
      {
         this.Close();
      }

      private void btnOK_Click(object sender, System.EventArgs e)
      {
         bool dirty = false;
         
         // Validate
         if ( m_oldCollectionServer != textCollectionServer.Text )
         {
            dirty = true;
		      if ( textCollectionServer.Text.Trim().Length == 0 )
            {
               MessageBox.Show(this, Constants.Error_InvalidComputerName, this.Text);
               return;
            }
         }
         if ( m_oldTraceDirectory != textTraceDirectory.Text )
         {
            dirty = true;
		      if ( ! ValidatePath( textTraceDirectory.Text ) )
            {
               MessageBox.Show(this, Constants.Error_InvalidTraceDirectory, this.Text);
               return;
            }
         }

         if (m_oldAssemblyDirectory != textAssemblyDirectory.Text)
         {
            dirty = true;

            if (!ValidatePath(textAssemblyDirectory.Text))
            {
               MessageBox.Show(this, Constants.Error_InvalidTraceDirectory, this.Text);
               return;
            }
         }
         
         if ( dirty )
         {
			 m_server.CollectionServer = textCollectionServer.Text ;
			 m_server.TraceDirectory = textTraceDirectory.Text ;
          m_server.TriggerAssemblyDirectory = textAssemblyDirectory.Text;

			 try
			 {
				 VSInstaller.UpdateServer(m_server) ;
			 }
			 catch(Exception ex)
			 {
				 MessageBox.Show(this, String.Format("Failed to update agent properties: {0}", ex.Message), this.Text) ;
			 }
            // TODO
            // save values
            // restart agent service
         }
                  
         this.Close();
      }
      
      //--------------------------------------------------------------------
      // Validate Path
      //--------------------------------------------------------------------
		private bool
		   ValidatePath(
		      string      filepath
		   )
		{
		   // make sure defined and a local path
		   if ( filepath.Length<3) return false;
			if(filepath.Length > 180) return false ;
		   if ( filepath[1] != ':' ) return false;
		   if ( filepath[2] != '\\' ) return false;
			if(filepath.IndexOf("..") != -1) return false ;
		   
         try
         {
            if ( ! Path.IsPathRooted(filepath) )
               return false;

            //This will check for aall invalid filename characters.
            Path.GetFullPath(filepath);
         }
         catch (Exception)
         {
            return false;
         }
		   return true;
		}

      private void Form_Properties_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
      {
         HelpAlias.ShowHelp(this, HelpAlias.CLUSTERHELP_Form_Properties) ;
         hlpevent.Handled = true ;
         /*
         if ( inHelpRequested ) return;
         inHelpRequested = true;
         Cursor = Cursors.WaitCursor;
         
         Help.ShowHelp( this, Constants.Help_ClusterHelpFile );
         
         Cursor = Cursors.Default;
         inHelpRequested = false;
         */
      }

      private void btnCopyService_Click(object sender, System.EventArgs e)
      {
         Clipboard.SetDataObject( _txtServiceName.Text );
      }

      private void btnCopyRegistry_Click(object sender, System.EventArgs e)
      {
         Clipboard.SetDataObject( _txtReplicatedRegistryKey.Text );
      }
      
	}
}
