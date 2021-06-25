using System;
using System.IO;
using System.Collections.Specialized ;
using System.Data.SqlClient ;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ServiceProcess ;
using System.Windows.Forms;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Idera.SQLcompliance.Core;

namespace Idera.SQLcompliance.Cluster
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form_Main : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnProperties;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.ListView listServers;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuLaunchPDF;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuHelpAbout;
		private System.Windows.Forms.MenuItem menuFileExit;
      private System.Windows.Forms.Label _lblVersion;
      private System.Windows.Forms.TextBox _tbVersion;
      private IContainer components;

		public Form_Main()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Check for local admin privileges
			if ( ! IsLocalAdmin() )
			{
				MessageBox.Show( "You do not have sufficient permissions to configure " +
					"the cluster support for SQL Compliance Manager. This " +
					"application requires that you are a member of the " +
					"BUILTIN\\Administrators group.",
					this.Text,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error );
				Application.Exit();
			}
		}
		
		//-----------------------------------------------------------------------
		// IsLocalAdmin
		//-----------------------------------------------------------------------
		public static bool
			IsLocalAdmin()
		{
			bool isAdmin = false;

			try
			{
				AppDomain.CurrentDomain.SetPrincipalPolicy(System.Security.Principal.PrincipalPolicy.WindowsPrincipal);
				WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
				WindowsIdentity identity = (WindowsIdentity)principal.Identity;
				isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
			}
			catch {}
		   
			return isAdmin;
		}
		

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
         this.components = new System.ComponentModel.Container();
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Main));
         this.label1 = new System.Windows.Forms.Label();
         this.btnAdd = new System.Windows.Forms.Button();
         this.btnProperties = new System.Windows.Forms.Button();
         this.btnRemove = new System.Windows.Forms.Button();
         this.listServers = new System.Windows.Forms.ListView();
         this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
         this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
         this.menuItem1 = new System.Windows.Forms.MenuItem();
         this.menuFileExit = new System.Windows.Forms.MenuItem();
         this.menuItem2 = new System.Windows.Forms.MenuItem();
         this.menuLaunchPDF = new System.Windows.Forms.MenuItem();
         this.menuItem4 = new System.Windows.Forms.MenuItem();
         this.menuHelpAbout = new System.Windows.Forms.MenuItem();
         this._lblVersion = new System.Windows.Forms.Label();
         this._tbVersion = new System.Windows.Forms.TextBox();
         this.SuspendLayout();
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(8, 8);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(300, 16);
         this.label1.TabIndex = 1;
         this.label1.Text = "Virtual SQL Servers with a SQLcompliance Agent service:";
         // 
         // btnAdd
         // 
         this.btnAdd.Location = new System.Drawing.Point(324, 24);
         this.btnAdd.Name = "btnAdd";
         this.btnAdd.Size = new System.Drawing.Size(96, 23);
         this.btnAdd.TabIndex = 2;
         this.btnAdd.Text = "&Add Service";
         this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
         // 
         // btnProperties
         // 
         this.btnProperties.Enabled = false;
         this.btnProperties.Location = new System.Drawing.Point(324, 56);
         this.btnProperties.Name = "btnProperties";
         this.btnProperties.Size = new System.Drawing.Size(96, 23);
         this.btnProperties.TabIndex = 3;
         this.btnProperties.Text = "&Properties...";
         this.btnProperties.Click += new System.EventHandler(this.btnProperties_Click);
         // 
         // btnRemove
         // 
         this.btnRemove.Enabled = false;
         this.btnRemove.Location = new System.Drawing.Point(324, 88);
         this.btnRemove.Name = "btnRemove";
         this.btnRemove.Size = new System.Drawing.Size(96, 23);
         this.btnRemove.TabIndex = 4;
         this.btnRemove.Text = "&Remove Service";
         this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
         // 
         // listServers
         // 
         this.listServers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
         this.listServers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
         this.listServers.HideSelection = false;
         this.listServers.Location = new System.Drawing.Point(8, 24);
         this.listServers.MultiSelect = false;
         this.listServers.Name = "listServers";
         this.listServers.Size = new System.Drawing.Size(304, 232);
         this.listServers.Sorting = System.Windows.Forms.SortOrder.Ascending;
         this.listServers.TabIndex = 7;
         this.listServers.UseCompatibleStateImageBehavior = false;
         this.listServers.View = System.Windows.Forms.View.List;
         this.listServers.SelectedIndexChanged += new System.EventHandler(this.listServers_SelectedIndexChanged);
         // 
         // columnHeader1
         // 
         this.columnHeader1.Text = "SQL Server";
         this.columnHeader1.Width = 280;
         // 
         // mainMenu1
         // 
         this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2});
         // 
         // menuItem1
         // 
         this.menuItem1.Index = 0;
         this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuFileExit});
         this.menuItem1.Text = "&File";
         // 
         // menuFileExit
         // 
         this.menuFileExit.Index = 0;
         this.menuFileExit.Text = "E&xit";
         this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
         // 
         // menuItem2
         // 
         this.menuItem2.Index = 1;
         this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuLaunchPDF,
            this.menuItem4,
            this.menuHelpAbout});
         this.menuItem2.Text = "&Help";
         // 
         // menuLaunchPDF
         // 
         this.menuLaunchPDF.Index = 0;
         this.menuLaunchPDF.Text = "&Help on Auditing Virtual SQL Servers...";
         this.menuLaunchPDF.Click += new System.EventHandler(this.menuLaunchPDF_Click);
         // 
         // menuItem4
         // 
         this.menuItem4.Index = 1;
         this.menuItem4.Text = "-";
         // 
         // menuHelpAbout
         // 
         this.menuHelpAbout.Index = 2;
         this.menuHelpAbout.Text = "&About Cluster Configuration Console...";
         this.menuHelpAbout.Click += new System.EventHandler(this.menuHelpAbout_Click);
         // 
         // _lblVersion
         // 
         this._lblVersion.Location = new System.Drawing.Point(8, 264);
         this._lblVersion.Name = "_lblVersion";
         this._lblVersion.Size = new System.Drawing.Size(164, 20);
         this._lblVersion.TabIndex = 8;
         this._lblVersion.Text = "SQLcompliance Agent Version:";
         this._lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // _tbVersion
         // 
         this._tbVersion.Location = new System.Drawing.Point(176, 264);
         this._tbVersion.Name = "_tbVersion";
         this._tbVersion.ReadOnly = true;
         this._tbVersion.Size = new System.Drawing.Size(136, 20);
         this._tbVersion.TabIndex = 9;
         // 
         // Form_Main
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(430, 291);
         this.Controls.Add(this._tbVersion);
         this.Controls.Add(this._lblVersion);
         this.Controls.Add(this.listServers);
         this.Controls.Add(this.btnRemove);
         this.Controls.Add(this.btnProperties);
         this.Controls.Add(this.btnAdd);
         this.Controls.Add(this.label1);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.Menu = this.mainMenu1;
         this.MinimizeBox = false;
         this.Name = "Form_Main";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "Cluster Configuration Console";
         this.Load += new System.EventHandler(this.Form_Main_Load);
         this.Shown += new System.EventHandler(this.Form_Main_Shown);
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_Main_HelpRequested);
         this.ResumeLayout(false);
         this.PerformLayout();

      }
		#endregion

		/// <summary>
		/// This handler is called for unhandled exceptions thrown from all non-main threads in SQLsafe
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if(e.ExceptionObject is Exception)
			{
				Exception ex = (Exception)e.ExceptionObject ;
				Idera.SQLcompliance.Core.ErrorLog.Instance.Write((Exception)e.ExceptionObject, true);
				string errorString = String.Format("Message:  {0}\r\n\r\nStack Trace:  {1}", ex.Message, ex.StackTrace) ;
				Form_UnhandledError errorBox = new Form_UnhandledError(errorString) ;
				errorBox.ShowDialog() ;
			}
			else
			{
				Idera.SQLcompliance.Core.ErrorLog.Instance.Write(e.ExceptionObject.ToString());
				string errorString = String.Format("Message:  {0}", e.ExceptionObject.ToString()) ;
				Form_UnhandledError errorBox = new Form_UnhandledError(errorString) ;
				errorBox.ShowDialog() ;
			}
		}

		/// <summary>
		/// This handler is called for unhandled exceptions thrown from the main thread in SQLsafe
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			Idera.SQLcompliance.Core.ErrorLog.Instance.Write(e.Exception, true);
			string errorString = String.Format("Message:  {0}\r\n\r\nStack Trace:  {1}", e.Exception.Message, e.Exception.StackTrace) ;
			Form_UnhandledError errorBox = new Form_UnhandledError(errorString) ;
			errorBox.ShowDialog() ;
		}
		
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			Application.Run(new Form_Main());
		}

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			Form_Add frm = new Form_Add();
			DialogResult choice = frm.ShowDialog();
			
			if ( choice == DialogResult.OK )
			{
				this.Cursor = Cursors.WaitCursor ;
				   
				// create service   
				try
				{
					VSInstaller.InstallVirtualServerAgent(frm.VirtualServer) ;
					
					// Show post registration instruction dialog
					Form_AddFollowUp followUpFrm = new Form_AddFollowUp();
					followUpFrm.ShowDialog();
					
					listServers.Items.Add(frm.VirtualServer.FullInstanceName );
				}
				catch(Exception ex)
				{
					// Error
					MessageBox.Show(this, String.Format("Unable to Add the Agent Service: {0}", ex.Message), this.Text) ;
				}
			}
			RefreshInstanceList() ;
			
			this.Cursor = Cursors.Default; 
		}

		private void btnProperties_Click(object sender, System.EventArgs e)
		{
			if(listServers.SelectedItems.Count > 0)
			{
				VirtualServer server = listServers.SelectedItems[0].Tag as VirtualServer ;
				Form_Properties propForm = new Form_Properties(server, false);

				propForm.ShowDialog(this) ;
			}
		}

		private bool IsActiveAgentOnNode(VirtualServer server)
		{
			ServiceController controller = VSInstaller.GetService(server.ServiceName) ;
			if(controller == null)
				return false ;
			else
				return controller.Status == ServiceControllerStatus.Running ;
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			bool removeTraces = false ;
			if ( listServers.SelectedItems.Count > 0 )
			{
				int ndx = listServers.SelectedIndices[0] ;
				VirtualServer server = VSInstaller.LoadServerFromRegistry(listServers.Items[ndx].Text) ;

				// On a corrupt registry, we bail
				if(server == null)
				{
					MessageBox.Show(this, String.Format(Constants.Error_UnableToLoadServerInformation, listServers.Items[ndx].Text),
						this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error) ;
					this.Close() ;
				}

				if(MessageBox.Show(this, String.Format("Are you sure you want to remove the agent for {0}?", server.FullInstanceName),
					this.Text, MessageBoxButtons.YesNo) == DialogResult.No)
					return ;

				if(MessageBox.Show(this, String.Format("Do you wish to stop collecting SQL Compliance Manager audit data for {0}?", server.FullInstanceName),
					this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
					removeTraces = true ;

				this.Cursor = Cursors.WaitCursor ;
				if(removeTraces)
				{
					VSInstaller.DropAuditingSupport(server.FullInstanceName) ;
				}
				try
				{
					VSInstaller.RemoveVirtualServerAgent(server.FullInstanceName) ;
				}
				catch(Exception ex)
				{
					MessageBox.Show(this, String.Format("Unable to remove the agent: {0}", ex.Message), this.Text) ;
					this.Cursor = Cursors.Default; 
					return ;
				}
				listServers.Items[ndx].Remove();

				if (listServers.Items.Count != 0)
				{
					if ( ndx >= listServers.Items.Count )
					{
						listServers.Items[listServers.Items.Count-1].Selected = true;
					}
					else
						listServers.Items[ndx].Selected = true;
				}
				this.Cursor = Cursors.Default; 
			}
			RefreshInstanceList() ;
		}

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void listServers_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if ( listServers.SelectedItems.Count != 0 )
			{
				btnProperties.Enabled = true;
				btnRemove.Enabled = true;
			}
			else
			{
				btnProperties.Enabled = false;
				btnRemove.Enabled = false;
			}
		}

		private void Form_Main_Load(object sender, System.EventArgs e)
      {
         RefreshInstanceList();
         _tbVersion.Text = VSInstaller.GetAgentVersion();
      }

		private void RefreshInstanceList()
		{
			listServers.BeginUpdate() ;
			listServers.Items.Clear() ;
			ArrayList instances = VSInstaller.GetInstalledVirtualInstances() ;
			if(instances == null)
			{
				MessageBox.Show(this, "SQLcompliance Cluster registry entries are corrupt.  Please reinstall SQLcompliance Cluster Support Manager.", this.Text) ;
				Close() ;
			}
			foreach(VirtualServer server in instances)
			{
				ListViewItem item = new ListViewItem(server.FullInstanceName) ;
				item.Tag = server ;
				listServers.Items.Add(item) ;
			}

			listServers.EndUpdate() ;
		}

		private void menuFileExit_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void menuLaunchPDF_Click(object sender, System.EventArgs e)
		{
			Cursor = Cursors.WaitCursor;
         HelpAlias.ShowHelp(this, HelpAlias.CLUSTERHELP_Form_Main);
			Cursor = Cursors.Default;
		}

		private void menuHelpAbout_Click(object sender, System.EventArgs e)
		{
			Form_About frm = new Form_About();
			frm.ShowDialog();
		}
      
		private void Form_Main_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
		{
         HelpAlias.ShowHelp(this, HelpAlias.CLUSTERHELP_Form_Main) ;
         hlpevent.Handled = true ;
		}

      private void Form_Main_Shown(object sender, EventArgs e)
      {
         bool displayForm = false;
         Form_SetAssemblyDirectory form = new Form_SetAssemblyDirectory();
         ArrayList instances = VSInstaller.GetInstalledVirtualInstances();

         //no work to be done, so bail.
         if (instances == null)
            return;

         foreach (VirtualServer server in instances)
         {
            if (String.IsNullOrEmpty(server.TriggerAssemblyDirectory))
            {
               displayForm = true;
               form.AddServer(server);
            }
         }

         if (displayForm)
         {
            if (form.ShowDialog(this) == DialogResult.OK)
            {
               Dictionary<string, string> paths = new Dictionary<string, string>();

               foreach (VirtualServer virtualServer in form.VirtualServers)
               {
                  VSInstaller.UpdateServer(virtualServer);
                  paths.Add(virtualServer.ServerName, virtualServer.TriggerAssemblyDirectory);
               }
               ClusterAgentUpgrade agentUpgrade = new ClusterAgentUpgrade();
               agentUpgrade.AssemblyDirectories = paths;
               SetAgentUpgradeInfo(agentUpgrade);

               RefreshInstanceList();

               Form_ProgressBar progress = new Form_ProgressBar();
               progress.ShowDialog(this);
            }
         }
      }

      private void SetAgentUpgradeInfo(ClusterAgentUpgrade agentUpgrade)
      {
         try
         {
            FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string path = Path.Combine(fileInfo.DirectoryName, CoreConstants.ClusterAgentUpgradeFilename);
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, agentUpgrade);
            stream.Close();
         }
         catch (Exception e)
         {
            Idera.SQLcompliance.Core.ErrorLog.Instance.Write("Error serializing agent upgrade info.", e, true);
         }
      }
	}
}
