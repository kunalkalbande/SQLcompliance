using System ;
using System.ComponentModel ;
using System.Drawing ;
using System.IO ;
using System.Windows.Forms ;
using File=System.IO.File ;

namespace Idera.Common.ReportsInstaller.View.HelperForms
{
	/// <summary>
	/// Summary description for AboutForm.
	/// </summary>
	public class AboutForm : Form
	{
		private PictureBox pictureBoxAbout;
		private Label labelVersion;
		private Label labelCopyright;
		private Button buttonClose;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public AboutForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AboutForm));
			this.pictureBoxAbout = new System.Windows.Forms.PictureBox();
			this.labelVersion = new System.Windows.Forms.Label();
			this.labelCopyright = new System.Windows.Forms.Label();
			this.buttonClose = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// pictureBoxAbout
			// 
			this.pictureBoxAbout.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxAbout.Image")));
			this.pictureBoxAbout.Location = new System.Drawing.Point(8, 16);
			this.pictureBoxAbout.Name = "pictureBoxAbout";
			this.pictureBoxAbout.Size = new System.Drawing.Size(184, 64);
			this.pictureBoxAbout.TabIndex = 0;
			this.pictureBoxAbout.TabStop = false;
			// 
			// labelVersion
			// 
			this.labelVersion.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelVersion.Location = new System.Drawing.Point(208, 16);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(312, 23);
			this.labelVersion.TabIndex = 1;
			this.labelVersion.Text = "<productAbbrev> Reports Installer 1.0.300.1234";
			this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelCopyright
			// 
			this.labelCopyright.Location = new System.Drawing.Point(200, 48);
			this.labelCopyright.Name = "labelCopyright";
			this.labelCopyright.Size = new System.Drawing.Size(344, 64);
			this.labelCopyright.TabIndex = 2;
			this.labelCopyright.Text = @"© Copyright <copyrightYears> Idera, a division of BBS Technologies, Inc., all rights reserved. <productName>, Idera and the Idera Logo are trademarks or registered trademarks of BBS Technologies or its subsidiaries in the United States and other jurisdictions.";
			// 
			// buttonClose
			// 
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonClose.Location = new System.Drawing.Point(456, 112);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.TabIndex = 3;
			this.buttonClose.Text = "&Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// AboutForm
			// 
			this.AcceptButton = this.buttonClose;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonClose;
			this.ClientSize = new System.Drawing.Size(538, 142);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.labelCopyright);
			this.Controls.Add(this.labelVersion);
			this.Controls.Add(this.pictureBoxAbout);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About <header>";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private delegate void initializeDelegate(string header, string version, string copyrightYears,
			string productName, string aboutPanelImage, string icon);
		public void initialize(string header, string version, string copyrightYears,
			string productName, string aboutPanelImage, string icon)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new initializeDelegate(initialize), new object[] {header,
				version, copyrightYears, productName, aboutPanelImage, icon});
				return;
			}
			this.Text = "About " + header;
			this.labelVersion.Text = header+ " " + version;
			this.labelCopyright.Text = @"© Copyright "+copyrightYears+" Idera, a division of BBS Technologies, Inc., all rights reserved. " +productName+ ", Idera and the Idera Logo are trademarks or registered trademarks of BBS Technologies or its subsidiaries in the United States and other jurisdictions.";
			if (File.Exists(aboutPanelImage)) 
			{
				try
				{
					this.pictureBoxAbout.Image = Image.FromFile(aboutPanelImage);
				}
				catch (Exception ex)
				{
				}
			}
			if (File.Exists(icon))
			{
				try
				{
					this.Icon = new Icon(icon);
				}
				catch (Exception ex)
				{
				}
			}
		}

	}
}
