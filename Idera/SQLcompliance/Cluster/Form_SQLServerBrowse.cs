using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;

using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using System.Data;
using System.Data.Sql;

namespace Idera.SQLcompliance.Cluster
{
    /// <summary>
    /// Summary description for Form_SQLServerBrowse.
    /// </summary>
    public class Form_SQLServerBrowse : System.Windows.Forms.Form
    {
        #region Properties

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBoxServers;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        string m_SelectedServer = "";
        public string SelectedServer
        {
            get { return m_SelectedServer; }
        }

        #endregion

        #region Constructor / Dispose

        //-----------------------------------------------------------------------
        // Constructor
        //-----------------------------------------------------------------------
        public Form_SQLServerBrowse()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            m_SelectedServer = "";
            LoadServers();

        }

        //-----------------------------------------------------------------------
        // Dispose - Clean up any resources being used.
        //-----------------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form_SQLServerBrowse));
            this.label1 = new System.Windows.Forms.Label();
            this.listBoxServers = new System.Windows.Forms.ListBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Select SQL Server:";
            // 
            // listBoxServers
            // 
            this.listBoxServers.Location = new System.Drawing.Point(12, 24);
            this.listBoxServers.Name = "listBoxServers";
            this.listBoxServers.Size = new System.Drawing.Size(292, 212);
            this.listBoxServers.TabIndex = 1;
            this.listBoxServers.SelectedIndexChanged += new System.EventHandler(this.listBoxServers_SelectedIndexChanged);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(312, 24);
            this.btnOK.Name = "btnOK";
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(312, 56);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Form_SQLServerBrowse
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(397, 252);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.listBoxServers);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_SQLServerBrowse";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select SQL Server";
            this.ResumeLayout(false);

        }
        #endregion

        #region Load list logic

        //-----------------------------------------------------------------------
        // LoadServers
        //-----------------------------------------------------------------------
        private bool
           LoadServers()
        {
            bool loaded = false;

            try
            {
                //SQLDMO.Application dmoapp = new SQLDMO.Application();
                //SQLDMO.NameList nameList = null;

                // nameList = dmoapp.ListAvailableSQLServers();

                //if ((nameList != null) && (nameList.Count > 0)) 
                //{
                //    IEnumerator nameListEnum = nameList.GetEnumerator();
                //    while (nameListEnum.MoveNext()) 
                //    {
                //       string instance = nameListEnum.Current.ToString();
                //   listBoxServers.Items.Add( instance.ToUpper()  );
                //    }
                //}

                SqlDataSourceEnumerator enumerator = SqlDataSourceEnumerator.Instance;
                using (DataTable tbl = enumerator.GetDataSources())
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        string server = row["ServerName"].ToString();
                        string instance = row["InstanceName"].ToString();
                        string fullName;

                        if (instance == null || instance.Length == 0)
                            fullName = server;
                        else
                            fullName = String.Format("{0}\\{1}", server, instance);

                        listBoxServers.Items.Add(fullName.ToUpper());
                    }
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
            return loaded;
        }

        #endregion

        #region Form Events

        //-----------------------------------------------------------------------
        // btnCancel_Click
        //-----------------------------------------------------------------------
        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        //-----------------------------------------------------------------------
        // btnOK_Click
        //-----------------------------------------------------------------------
        private void btnOK_Click(object sender, System.EventArgs e)
        {
            m_SelectedServer = listBoxServers.SelectedItem.ToString();
            this.Close();
        }

        #endregion

        private void listBoxServers_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (listBoxServers.SelectedItems.Count == 1)
                btnOK.Enabled = true;
            else
                btnOK.Enabled = false;
        }
    }
}
