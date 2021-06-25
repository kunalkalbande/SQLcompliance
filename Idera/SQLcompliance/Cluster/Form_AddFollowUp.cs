using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Cluster
{
	/// <summary>
	/// Summary description for Form_AddFollowUp.
	/// </summary>
	public class Form_AddFollowUp : System.Windows.Forms.Form
	{
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.LinkLabel linkTellMeMore;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label5;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form_AddFollowUp()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form_AddFollowUp));
			this.btnOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.linkTellMeMore = new System.Windows.Forms.LinkLabel();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnOK.Location = new System.Drawing.Point(179, 152);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "&OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(420, 40);
			this.label1.TabIndex = 1;
			this.label1.Text = "You have successfully registered the SQLcompliance Agent service on one node in y" +
				"our SQL Server cluster. To complete this process, you will need to do the follow" +
				"ing:";
			// 
			// linkTellMeMore
			// 
			this.linkTellMeMore.Location = new System.Drawing.Point(41, 120);
			this.linkTellMeMore.Name = "linkTellMeMore";
			this.linkTellMeMore.Size = new System.Drawing.Size(350, 16);
			this.linkTellMeMore.TabIndex = 3;
			this.linkTellMeMore.TabStop = true;
			this.linkTellMeMore.Text = "Tell me more about configuring auditing in a clustered environment...";
			this.linkTellMeMore.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkTellMeMore_LinkClicked);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(44, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(360, 28);
			this.label2.TabIndex = 4;
			this.label2.Text = "Repeat the cluster support setup process on each node in the cluster. ";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(24, 56);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(24, 20);
			this.label3.TabIndex = 5;
			this.label3.Text = "(1)";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(24, 80);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(24, 20);
			this.label4.TabIndex = 6;
			this.label4.Text = "(2)";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(44, 80);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(348, 28);
			this.label5.TabIndex = 7;
			this.label5.Text = "Register the SQLcompliance Agent service as a shared resource in the Microsoft Cl" +
				"uster Administrator.";
			// 
			// Form_AddFollowUp
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnOK;
			this.ClientSize = new System.Drawing.Size(432, 184);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.linkTellMeMore);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.label3);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Form_AddFollowUp";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add SQLcompliance Agent Service";
			this.ResumeLayout(false);

		}
		#endregion

      private void btnOK_Click(object sender, System.EventArgs e)
      {
         Close();
      }

      private void linkTellMeMore_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
      {
         HelpAlias.ShowHelp(this, HelpAlias.CLUSTERHELP_AuditVirtualInstance);
      }
	}
}
