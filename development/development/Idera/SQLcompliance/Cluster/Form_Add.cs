using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Security;

namespace Idera.SQLcompliance.Cluster
{
	/// <summary>
	/// Summary description for Form_Add.
	/// </summary>
	public class Form_Add : System.Windows.Forms.Form
	{
		#region Window Properties	
		private System.Windows.Forms.Panel panelButtons;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnFinish;
		private System.Windows.Forms.Button btnNext;
		private System.Windows.Forms.Button btnBack;
		private System.Windows.Forms.Panel panelLeft;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Panel panelAgentTrace;
		private System.Windows.Forms.Panel panel7;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox txtTriggerAssemblyDirectory;
		private System.Windows.Forms.Label labeAgentTraceDescription;
		private System.Windows.Forms.Label labelAgentTraceTitle;
		private System.Windows.Forms.Panel panelAgentService;
		private System.Windows.Forms.Panel panelAgentProperties;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textServicePasswordConfirm;
		private System.Windows.Forms.TextBox textServicePassword;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textServiceAccount;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label labelAgentDescription;
		private System.Windows.Forms.Label labelAgentTitle;
		private System.Windows.Forms.Panel panelGeneralProperties;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.TextBox textSQLServer;
		private System.Windows.Forms.Label labelSQLServer;
		private System.Windows.Forms.Label lblGeneralDescription;
		private System.Windows.Forms.Label lblGeneralTitle;
		private System.Windows.Forms.Panel panelSummary;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Label lblInstance;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Panel panelCollectionServer;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Panel panelGeneral;
		private System.Windows.Forms.TextBox txtCollectionServer;
      private System.Windows.Forms.Panel panelAgentTrigger;
      private System.Windows.Forms.Panel panel3;
      private System.Windows.Forms.Label label12;
      private System.Windows.Forms.Label label17;
      private System.Windows.Forms.Label label18;
      private System.Windows.Forms.TextBox txtTraceDirectory;
      private System.Windows.Forms.Label label19;
      private System.Windows.Forms.Label label20;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		public Form_Add()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.panelGeneral.Enabled = false;
			this.panelCollectionServer.Enabled = false;
			this.panelAgentService.Enabled = false;
			this.panelAgentTrace.Enabled = false;
         this.panelAgentTrigger.Enabled = false;
			this.panelSummary.Enabled = false;
         
			currentPage = pageGeneral;
			SetCurrentPage();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Add));
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnFinish = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panelAgentTrigger = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.txtTriggerAssemblyDirectory = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.labeAgentTraceDescription = new System.Windows.Forms.Label();
            this.labelAgentTraceTitle = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.txtTraceDirectory = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.panelAgentTrace = new System.Windows.Forms.Panel();
            this.panelAgentService = new System.Windows.Forms.Panel();
            this.panelAgentProperties = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textServicePasswordConfirm = new System.Windows.Forms.TextBox();
            this.textServicePassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textServiceAccount = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.labelAgentDescription = new System.Windows.Forms.Label();
            this.labelAgentTitle = new System.Windows.Forms.Label();
            this.panelGeneral = new System.Windows.Forms.Panel();
            this.panelGeneralProperties = new System.Windows.Forms.Panel();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.textSQLServer = new System.Windows.Forms.TextBox();
            this.labelSQLServer = new System.Windows.Forms.Label();
            this.lblGeneralDescription = new System.Windows.Forms.Label();
            this.lblGeneralTitle = new System.Windows.Forms.Label();
            this.panelSummary = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblInstance = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.panelCollectionServer = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCollectionServer = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.panelButtons.SuspendLayout();
            this.panelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelAgentTrigger.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panelAgentTrace.SuspendLayout();
            this.panelAgentService.SuspendLayout();
            this.panelAgentProperties.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panelGeneral.SuspendLayout();
            this.panelGeneralProperties.SuspendLayout();
            this.panelSummary.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panelCollectionServer.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.btnCancel);
            this.panelButtons.Controls.Add(this.btnFinish);
            this.panelButtons.Controls.Add(this.btnNext);
            this.panelButtons.Controls.Add(this.btnBack);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 332);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(557, 38);
            this.panelButtons.TabIndex = 10;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(490, 10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(62, 20);
            this.btnCancel.TabIndex = 203;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnFinish
            // 
            this.btnFinish.Enabled = false;
            this.btnFinish.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnFinish.Location = new System.Drawing.Point(420, 10);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new System.Drawing.Size(62, 20);
            this.btnFinish.TabIndex = 202;
            this.btnFinish.Text = "&Finish";
            this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
            // 
            // btnNext
            // 
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnNext.Location = new System.Drawing.Point(350, 10);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(62, 20);
            this.btnNext.TabIndex = 201;
            this.btnNext.Text = "&Next >";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnBack
            // 
            this.btnBack.Enabled = false;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnBack.Location = new System.Drawing.Point(280, 10);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(62, 20);
            this.btnBack.TabIndex = 200;
            this.btnBack.Text = "< &Back";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.panelLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLeft.Controls.Add(this.pictureBox1);
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(112, 336);
            this.panelLeft.TabIndex = 11;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(112, 336);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // panelAgentTrigger
            // 
            this.panelAgentTrigger.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panelAgentTrigger.Controls.Add(this.panel7);
            this.panelAgentTrigger.Controls.Add(this.labeAgentTraceDescription);
            this.panelAgentTrigger.Controls.Add(this.labelAgentTraceTitle);
            this.panelAgentTrigger.Location = new System.Drawing.Point(111, 0);
            this.panelAgentTrigger.Name = "panelAgentTrigger";
            this.panelAgentTrigger.Size = new System.Drawing.Size(448, 336);
            this.panelAgentTrigger.TabIndex = 30;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.SystemColors.Control;
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Controls.Add(this.txtTriggerAssemblyDirectory);
            this.panel7.Controls.Add(this.label16);
            this.panel7.Controls.Add(this.label14);
            this.panel7.Controls.Add(this.label13);
            this.panel7.Location = new System.Drawing.Point(0, 60);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(448, 276);
            this.panel7.TabIndex = 53;
            // 
            // txtTriggerAssemblyDirectory
            // 
            this.txtTriggerAssemblyDirectory.Location = new System.Drawing.Point(119, 89);
            this.txtTriggerAssemblyDirectory.MaxLength = 255;
            this.txtTriggerAssemblyDirectory.Name = "txtTriggerAssemblyDirectory";
            this.txtTriggerAssemblyDirectory.Size = new System.Drawing.Size(304, 20);
            this.txtTriggerAssemblyDirectory.TabIndex = 2;
            this.txtTriggerAssemblyDirectory.TextChanged += new System.EventHandler(this.txtTriggerAssebmlyDirectory_TextChanged);
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(16, 129);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(420, 31);
            this.label16.TabIndex = 7;
            this.label16.Text = "Note: This directory will be created by the SQLcompliance Agent when before-after" +
    " auditing is enabled.";
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(12, 92);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(148, 23);
            this.label14.TabIndex = 6;
            this.label14.Text = "Assembly Directory:";
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(8, 12);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(432, 68);
            this.label13.TabIndex = 4;
            this.label13.Text = resources.GetString("label13.Text");
            // 
            // labeAgentTraceDescription
            // 
            this.labeAgentTraceDescription.Location = new System.Drawing.Point(14, 24);
            this.labeAgentTraceDescription.Name = "labeAgentTraceDescription";
            this.labeAgentTraceDescription.Size = new System.Drawing.Size(420, 28);
            this.labeAgentTraceDescription.TabIndex = 52;
            this.labeAgentTraceDescription.Text = "Specify where the SQL Server assemblies for the CLR trigger should be stored";
            // 
            // labelAgentTraceTitle
            // 
            this.labelAgentTraceTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAgentTraceTitle.Location = new System.Drawing.Point(14, 8);
            this.labelAgentTraceTitle.Name = "labelAgentTraceTitle";
            this.labelAgentTraceTitle.Size = new System.Drawing.Size(384, 16);
            this.labelAgentTraceTitle.TabIndex = 51;
            this.labelAgentTraceTitle.Text = "CLR Trigger Location";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Control;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.label12);
            this.panel3.Controls.Add(this.label17);
            this.panel3.Controls.Add(this.label18);
            this.panel3.Controls.Add(this.txtTraceDirectory);
            this.panel3.Location = new System.Drawing.Point(0, 60);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(448, 276);
            this.panel3.TabIndex = 53;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(16, 124);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(420, 36);
            this.label12.TabIndex = 7;
            this.label12.Text = "Note: This directory will be created by the SQLcompliance Agent when Before and A" +
    "fter data auditing enabled.";
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(24, 80);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(88, 23);
            this.label17.TabIndex = 6;
            this.label17.Text = "Trace Directory:";
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(8, 12);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(432, 56);
            this.label18.TabIndex = 4;
            this.label18.Text = resources.GetString("label18.Text");
            // 
            // txtTraceDirectory
            // 
            this.txtTraceDirectory.Location = new System.Drawing.Point(112, 77);
            this.txtTraceDirectory.MaxLength = 255;
            this.txtTraceDirectory.Name = "txtTraceDirectory";
            this.txtTraceDirectory.Size = new System.Drawing.Size(304, 20);
            this.txtTraceDirectory.TabIndex = 2;
            this.txtTraceDirectory.TextChanged += new System.EventHandler(this.txtTraceDirectory_TextChanged);
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(14, 24);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(420, 28);
            this.label19.TabIndex = 52;
            this.label19.Text = "Specify directory for temporary storage of audit data";
            // 
            // label20
            // 
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(14, 8);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(384, 16);
            this.label20.TabIndex = 51;
            this.label20.Text = "SQLcompliance Agent Trace Directory";
            // 
            // panelAgentTrace
            // 
            this.panelAgentTrace.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panelAgentTrace.Controls.Add(this.panel3);
            this.panelAgentTrace.Controls.Add(this.label19);
            this.panelAgentTrace.Controls.Add(this.label20);
            this.panelAgentTrace.Location = new System.Drawing.Point(111, 0);
            this.panelAgentTrace.Name = "panelAgentTrace";
            this.panelAgentTrace.Size = new System.Drawing.Size(448, 336);
            this.panelAgentTrace.TabIndex = 54;
            // 
            // panelAgentService
            // 
            this.panelAgentService.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panelAgentService.Controls.Add(this.panelAgentProperties);
            this.panelAgentService.Controls.Add(this.labelAgentDescription);
            this.panelAgentService.Controls.Add(this.labelAgentTitle);
            this.panelAgentService.Location = new System.Drawing.Point(111, 0);
            this.panelAgentService.Name = "panelAgentService";
            this.panelAgentService.Size = new System.Drawing.Size(448, 336);
            this.panelAgentService.TabIndex = 31;
            // 
            // panelAgentProperties
            // 
            this.panelAgentProperties.BackColor = System.Drawing.SystemColors.Control;
            this.panelAgentProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelAgentProperties.Controls.Add(this.label15);
            this.panelAgentProperties.Controls.Add(this.groupBox2);
            this.panelAgentProperties.Location = new System.Drawing.Point(0, 60);
            this.panelAgentProperties.Name = "panelAgentProperties";
            this.panelAgentProperties.Size = new System.Drawing.Size(448, 276);
            this.panelAgentProperties.TabIndex = 53;
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(24, 124);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(364, 56);
            this.label15.TabIndex = 53;
            this.label15.Text = resources.GetString("label15.Text");
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textServicePasswordConfirm);
            this.groupBox2.Controls.Add(this.textServicePassword);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.textServiceAccount);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(387, 104);
            this.groupBox2.TabIndex = 51;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "SQLcompliance Agent Service Account:";
            // 
            // label1
            // 
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Location = new System.Drawing.Point(10, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 15);
            this.label1.TabIndex = 47;
            this.label1.Text = "C&onfirm password:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // textServicePasswordConfirm
            // 
            this.textServicePasswordConfirm.Location = new System.Drawing.Point(156, 69);
            this.textServicePasswordConfirm.MaxLength = 64;
            this.textServicePasswordConfirm.Name = "textServicePasswordConfirm";
            this.textServicePasswordConfirm.PasswordChar = '*';
            this.textServicePasswordConfirm.Size = new System.Drawing.Size(224, 20);
            this.textServicePasswordConfirm.TabIndex = 48;
            // 
            // textServicePassword
            // 
            this.textServicePassword.Location = new System.Drawing.Point(156, 45);
            this.textServicePassword.MaxLength = 64;
            this.textServicePassword.Name = "textServicePassword";
            this.textServicePassword.PasswordChar = '*';
            this.textServicePassword.Size = new System.Drawing.Size(224, 20);
            this.textServicePassword.TabIndex = 46;
            // 
            // label2
            // 
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Location = new System.Drawing.Point(10, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 15);
            this.label2.TabIndex = 45;
            this.label2.Text = "&Password :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // textServiceAccount
            // 
            this.textServiceAccount.Location = new System.Drawing.Point(156, 21);
            this.textServiceAccount.MaxLength = 255;
            this.textServiceAccount.Name = "textServiceAccount";
            this.textServiceAccount.Size = new System.Drawing.Size(223, 20);
            this.textServiceAccount.TabIndex = 44;
            this.textServiceAccount.TextChanged += new System.EventHandler(this.textServiceAccount_TextChanged);
            // 
            // label3
            // 
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label3.Location = new System.Drawing.Point(10, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(142, 17);
            this.label3.TabIndex = 43;
            this.label3.Text = "&Login account (domain\\user):";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelAgentDescription
            // 
            this.labelAgentDescription.Location = new System.Drawing.Point(14, 24);
            this.labelAgentDescription.Name = "labelAgentDescription";
            this.labelAgentDescription.Size = new System.Drawing.Size(420, 28);
            this.labelAgentDescription.TabIndex = 52;
            this.labelAgentDescription.Text = "Specify the service options. This account needs to be given SQL Server Administra" +
    "tor privileges on the associated SQL Server.";
            // 
            // labelAgentTitle
            // 
            this.labelAgentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAgentTitle.Location = new System.Drawing.Point(14, 8);
            this.labelAgentTitle.Name = "labelAgentTitle";
            this.labelAgentTitle.Size = new System.Drawing.Size(384, 16);
            this.labelAgentTitle.TabIndex = 51;
            this.labelAgentTitle.Text = "SQLcompliance Agent Service Account";
            // 
            // panelGeneral
            // 
            this.panelGeneral.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panelGeneral.Controls.Add(this.panelGeneralProperties);
            this.panelGeneral.Controls.Add(this.lblGeneralDescription);
            this.panelGeneral.Controls.Add(this.lblGeneralTitle);
            this.panelGeneral.Location = new System.Drawing.Point(111, 0);
            this.panelGeneral.Name = "panelGeneral";
            this.panelGeneral.Size = new System.Drawing.Size(448, 336);
            this.panelGeneral.TabIndex = 32;
            // 
            // panelGeneralProperties
            // 
            this.panelGeneralProperties.BackColor = System.Drawing.SystemColors.Control;
            this.panelGeneralProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelGeneralProperties.Controls.Add(this.btnBrowse);
            this.panelGeneralProperties.Controls.Add(this.textSQLServer);
            this.panelGeneralProperties.Controls.Add(this.labelSQLServer);
            this.panelGeneralProperties.Location = new System.Drawing.Point(0, 60);
            this.panelGeneralProperties.Name = "panelGeneralProperties";
            this.panelGeneralProperties.Size = new System.Drawing.Size(448, 276);
            this.panelGeneralProperties.TabIndex = 16;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(412, 8);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(23, 20);
            this.btnBrowse.TabIndex = 16;
            this.btnBrowse.Text = ".&..";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // textSQLServer
            // 
            this.textSQLServer.Location = new System.Drawing.Point(88, 8);
            this.textSQLServer.MaxLength = 255;
            this.textSQLServer.Name = "textSQLServer";
            this.textSQLServer.Size = new System.Drawing.Size(316, 20);
            this.textSQLServer.TabIndex = 15;
            this.textSQLServer.TextChanged += new System.EventHandler(this.textSQLServer_TextChanged);
            // 
            // labelSQLServer
            // 
            this.labelSQLServer.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelSQLServer.Location = new System.Drawing.Point(12, 12);
            this.labelSQLServer.Name = "labelSQLServer";
            this.labelSQLServer.Size = new System.Drawing.Size(70, 15);
            this.labelSQLServer.TabIndex = 14;
            this.labelSQLServer.Text = "&SQL Server:";
            this.labelSQLServer.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblGeneralDescription
            // 
            this.lblGeneralDescription.Location = new System.Drawing.Point(14, 24);
            this.lblGeneralDescription.Name = "lblGeneralDescription";
            this.lblGeneralDescription.Size = new System.Drawing.Size(420, 28);
            this.lblGeneralDescription.TabIndex = 15;
            this.lblGeneralDescription.Text = "Specify the virtual SQL Server instance that this SQLcompliance Agent will audit." +
    "";
            // 
            // lblGeneralTitle
            // 
            this.lblGeneralTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGeneralTitle.Location = new System.Drawing.Point(14, 8);
            this.lblGeneralTitle.Name = "lblGeneralTitle";
            this.lblGeneralTitle.Size = new System.Drawing.Size(384, 16);
            this.lblGeneralTitle.TabIndex = 14;
            this.lblGeneralTitle.Text = "General";
            // 
            // panelSummary
            // 
            this.panelSummary.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panelSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSummary.Controls.Add(this.label8);
            this.panelSummary.Controls.Add(this.label9);
            this.panelSummary.Controls.Add(this.panel4);
            this.panelSummary.Location = new System.Drawing.Point(111, 0);
            this.panelSummary.Name = "panelSummary";
            this.panelSummary.Size = new System.Drawing.Size(448, 336);
            this.panelSummary.TabIndex = 33;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(14, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(420, 28);
            this.label8.TabIndex = 15;
            this.label8.Text = "Ready to add SQLcompliance Agent";
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(14, 8);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(384, 16);
            this.label9.TabIndex = 14;
            this.label9.Text = "Summary";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.Control;
            this.panel4.Controls.Add(this.lblInstance);
            this.panel4.Controls.Add(this.label11);
            this.panel4.Controls.Add(this.label10);
            this.panel4.Location = new System.Drawing.Point(0, 60);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(448, 276);
            this.panel4.TabIndex = 16;
            // 
            // lblInstance
            // 
            this.lblInstance.Location = new System.Drawing.Point(44, 48);
            this.lblInstance.Name = "lblInstance";
            this.lblInstance.Size = new System.Drawing.Size(384, 36);
            this.lblInstance.TabIndex = 2;
            this.lblInstance.Text = "txtSummaryInstance";
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(12, 12);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(420, 28);
            this.label11.TabIndex = 1;
            this.label11.Text = "You have finished entering the data necessary to install the SQLcompliance Agent " +
    "service necessary to audit this virtual SQL Server:";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(12, 96);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(412, 64);
            this.label10.TabIndex = 0;
            this.label10.Text = resources.GetString("label10.Text");
            // 
            // panelCollectionServer
            // 
            this.panelCollectionServer.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panelCollectionServer.Controls.Add(this.panel2);
            this.panelCollectionServer.Controls.Add(this.label6);
            this.panelCollectionServer.Controls.Add(this.label7);
            this.panelCollectionServer.Location = new System.Drawing.Point(111, 0);
            this.panelCollectionServer.Name = "panelCollectionServer";
            this.panelCollectionServer.Size = new System.Drawing.Size(448, 336);
            this.panelCollectionServer.TabIndex = 34;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.txtCollectionServer);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Location = new System.Drawing.Point(0, 60);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(448, 276);
            this.panel2.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(404, 44);
            this.label4.TabIndex = 16;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // txtCollectionServer
            // 
            this.txtCollectionServer.Location = new System.Drawing.Point(168, 68);
            this.txtCollectionServer.MaxLength = 255;
            this.txtCollectionServer.Name = "txtCollectionServer";
            this.txtCollectionServer.Size = new System.Drawing.Size(224, 20);
            this.txtCollectionServer.TabIndex = 15;
            this.txtCollectionServer.TextChanged += new System.EventHandler(this.txtCollectionServer_TextChanged);
            // 
            // label5
            // 
            this.label5.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label5.Location = new System.Drawing.Point(24, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(136, 15);
            this.label5.TabIndex = 14;
            this.label5.Text = "&Collection Server computer:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(14, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(420, 28);
            this.label6.TabIndex = 15;
            this.label6.Text = "Specify the SQL Server to register with SQL Compliance Manager. Once a SQL Server" +
    " is registered, you can begin auditing database activity on the server. ";
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(14, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(384, 16);
            this.label7.TabIndex = 14;
            this.label7.Text = "Collection Server";
            // 
            // Form_Add
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(557, 370);
            this.Controls.Add(this.panelAgentTrace);
            this.Controls.Add(this.panelAgentTrigger);
            this.Controls.Add(this.panelSummary);
            this.Controls.Add(this.panelGeneral);
            this.Controls.Add(this.panelCollectionServer);
            this.Controls.Add(this.panelAgentService);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_Add";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add SQLcompliance Agent Service";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_Add_HelpRequested);
            this.panelButtons.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelAgentTrigger.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panelAgentTrace.ResumeLayout(false);
            this.panelAgentService.ResumeLayout(false);
            this.panelAgentProperties.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panelGeneral.ResumeLayout(false);
            this.panelGeneralProperties.ResumeLayout(false);
            this.panelGeneralProperties.PerformLayout();
            this.panelSummary.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panelCollectionServer.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

      }
		#endregion

		#region General Properties

		private string m_instance ;
		private string m_computer ;
        private string instanceName;
        private int? instancePort;
        private string instanceWithPort;
        private const string COMMA_CHARACTER = ",";

		public VirtualServer VirtualServer
		{
			get 
			{
				VirtualServer retVal = new VirtualServer() ;

				retVal.ServerName = m_computer ;
				retVal.InstanceName = m_instance ;
				retVal.CollectionServer = this.txtCollectionServer.Text ;
				retVal.ServiceUsername = this.textServiceAccount.Text ;
				retVal.ServicePassword = this.textServicePassword.Text ;
				retVal.TraceDirectory = this.txtTraceDirectory.Text ;
                retVal.TriggerAssemblyDirectory = this.txtTriggerAssemblyDirectory.Text;
                retVal.InstancePort = instancePort;
                retVal.InstanceWithPort = instanceWithPort;
				return retVal ;
			}
		}
      
		#endregion

		#region Page Navigation
      
		//--------------------------------------------------------------------
		// btnNext_Click - Next button; move forward a page in the wizard
		//--------------------------------------------------------------------
		private void btnNext_Click(object sender, System.EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor ;
			if ( ValidatePage( currentPage ) )
			{
				ChangeWizardPage ( WizardAction.Next );
			}
			this.Cursor = Cursors.Default ;
		}

		//--------------------------------------------------------------------
		// btnBack_Click - Back button; move back a page in the wizard
		//--------------------------------------------------------------------
		private void btnBack_Click(object sender, System.EventArgs e)
		{
			ChangeWizardPage( WizardAction.Prev );
		}
      

		//--------------------------------
		// Wizard State Machine Constants
		//--------------------------------
		int numPages    = 6;
		int currentPage = pageGeneral;
		Panel currentPanel = null;
		enum WizardAction
		{
			Next = 0,
				Prev = 1
				};
	   
		private const int pageGeneral              = 0;
		private const int pageCollectionServer     = 1;
		private const int pageAgentService         = 2;
		private const int pageAgentTrace           = 3;
      private const int pageAgentTrigger         = 4;
		private const int pageSummary              = 5;

		//--------------------------------------------------------------------
		// ChangeWizardPage - Move forward or backwards in the wizard
		//--------------------------------------------------------------------
		private void ChangeWizardPage( WizardAction direction )
		{
			// Change Page
			if ( direction == WizardAction.Next )
			{
				if ( currentPage < (numPages-1) )
				{
					currentPage ++;
				}
			}
			else // Prev
			{
				if ( currentPage > 0 )
				{
					currentPage --;
				}
			}
         
			SetCurrentPage();
		}
      
		//--------------------------------------------------------------------
		// SetCurrentPage - Make sure the current page is visible and buttons 
		//                  are enabled/disabled appropriately
		//--------------------------------------------------------------------
		private void SetCurrentPage()
		{
			Panel oldPanel = currentPanel;
            
			if ( currentPage == pageGeneral )
			{
				currentPanel = this.panelGeneral;
				SetButtonState( false,   /* back   */
					( textSQLServer.Text.Trim() != "" ),    /* next   */
					false ); /* finish */
			}
			else if ( currentPage == pageCollectionServer )
			{
				currentPanel = this.panelCollectionServer;
				SetButtonState( true,   /* back   */
					( txtCollectionServer.Text.Trim() != "" ),    /* next  */
					false ); /* finish */
			}
			else if ( currentPage == pageAgentService )
			{
				currentPanel = this.panelAgentService;
				SetButtonState( true,   /* back   */
					( textServiceAccount.Text.Trim() != "" ),    /* next  */
					false); /* finish */
			}
			else if ( currentPage == pageAgentTrace )
			{
				currentPanel = this.panelAgentTrace;
				SetButtonState( true,   /* back   */
					( txtTraceDirectory.Text.Trim() != "" ),    /* next  */
					false); /* finish */
			}
         else if (currentPage == pageAgentTrigger)
         {
            currentPanel = this.panelAgentTrigger;
            SetButtonState(true, //back
               (txtTriggerAssemblyDirectory.Text.Trim() != ""), 
               false); //finish
         }
         else if (currentPage == pageSummary)
         {
            currentPanel = this.panelSummary;
            lblInstance.Text = textSQLServer.Text;
            SetButtonState(true,   /* back   */
               false,  /* next   */
               true); /* finish */
         }
         else
         {
            //internal error
         }

			if ( (currentPanel != null) && (currentPanel!=oldPanel) )
			{  
				if ( oldPanel != null )
					oldPanel.Enabled = false;
         
				currentPanel.Enabled = true;       
				currentPanel.BringToFront();
            
				// set focus
				if ( currentPage == pageGeneral )
				{
					textSQLServer.Focus();
				}
				else if ( currentPage == pageCollectionServer )
				{
					txtCollectionServer.Focus();
				}
				else if ( currentPage == pageAgentService )
				{
					textServiceAccount.Focus();
				}
				else if ( currentPage == pageAgentTrace )
				{
					txtTraceDirectory.Focus();
				}
            else if (currentPage == pageAgentTrigger)
            {
               txtTriggerAssemblyDirectory.Focus();
            }
            else if (currentPage == pageSummary)
            {
               btnFinish.Focus();
            }
			}
         
			if ( btnNext.Enabled )
			{
				this.AcceptButton = btnNext;
			}
			else if ( btnFinish.Enabled )
			{
				this.AcceptButton = btnFinish;
			}
		}
      
		//--------------------------------------------------------------------
		// SetButtonState - Set back,next, finish enabled state
		//--------------------------------------------------------------------
		private void SetButtonState( bool back, bool next, bool finish )
		{
			btnBack.Enabled   = back;
			btnNext.Enabled   = next;
			btnFinish.Enabled = finish;
		}
      
		#endregion

		#region Validation Methods

		//--------------------------------------------------------------------
		// ValidatePage - Simple validation done as users switches pages with
		//                back and next. More extensive validation is done
		//                after Finish is pressed.
		//--------------------------------------------------------------------
		private bool
			ValidatePage(
			int               page
			)
		{
			if ( page == pageGeneral )
			{
				// validate server name
				if ( ! ValidateServerName() )
				{
					MessageBox.Show(this, Constants.Error_InvalidServerName, this.Text);
					return false;
				}
				else
				{
					if(IsServerInstalled())
					{
						MessageBox.Show(this, Constants.Error_ServerAlreadyInstalled, this.Text);
						return false;
					}
					else
					{
					   bool connectionFailed = false;
					   
					   //make sure SQL Server is clustered
						try
						{
                            if(!VSInstaller.IsClustered(instanceName))
							{
								if(MessageBox.Show(this, Constants.Error_ServerNotClusteredYesNo, this.Text, MessageBoxButtons.YesNo) == DialogResult.No)
								{
									return false ;
								}
							}
						}
						catch(Exception e)
						{
						   connectionFailed = true;
							if(MessageBox.Show(this, String.Format("{0} {1}", e.Message, Constants.Error_UnableToContactServerYesNo), this.Text, MessageBoxButtons.YesNo) == DialogResult.No)
							{
								return false ;
							}
						}
						
						// make sure SQL Server/OS combo is supported
						if ( ! connectionFailed )
						{
						   try
						   {
                        if ( ! VSInstaller.ValidateSqlServerOSCombo( instanceName ) )
                        {
						         DialogResult choice = MessageBox.Show( this,
						                                                Constants.Error_ServerOSComboNotSupported,
						                                                this.Text,
						                                                MessageBoxButtons.YesNo );
						         if ( choice == DialogResult.No )
						         {
						            return false;
						         }
                        }
						   }
						   catch(Exception e)
						   {
							   if(MessageBox.Show( this,
							                       String.Format("{0} {1}", e.Message, Constants.Error_UnableToContactServerYesNo),
							                       this.Text,
							                       MessageBoxButtons.YesNo) == DialogResult.No)
							   {
								   return false ;
							   }
						   }
                  }
					}
				}

				// Check for duplicates               
				// TODO: see if server is already registered
				//       compare against existing registered instances
				//       compare virtual server since you can only have one 
				//       instance per virtual server
			}
         
			if ( page == pageCollectionServer ) // collectionserver
			{
				// check for non blank - valid comptuer name
				if(!IsValidComputerName(txtCollectionServer.Text))
				{
					MessageBox.Show(this, Constants.Error_InvalidCollectionServerName, this.Text);
					return false;
				}
			}

			if ( page == pageAgentService )  // agent service account
			{
				if ( ! ValidateAccountName() )
				{
					MessageBox.Show(this, Constants.Error_InvalidServiceAccountName, this.Text);
					return false;
				}
               
				if ( textServicePassword.Text != textServicePasswordConfirm.Text )
				{
					MessageBox.Show(this, Constants.Error_MismatchedPasswords, this.Text);
					return false;
				}
            
				if(InstallUtil.VerifyPassword(textServiceAccount.Text, textServicePassword.Text) != 0)
				{
					MessageBox.Show(this, Constants.Error_InvalidDomainCredentials, this.Text) ;
					return false ;
				}
				
				// grant logon as service right
				int retval = InstallUtil.GiveLogonAsServicePriv ( textServiceAccount.Text ); 	
            if ( retval != 0 )
            {
					MessageBox.Show( this,
					                 String.Format("Unable to grant 'Logon as Service' right to {0}.\r\nError code: {1}",
					                               textServiceAccount.Text,
					                               retval ),
					                 this.Text) ;
               return false;					                 
            }                                         				
			}
         
			if ( page == pageAgentTrace ) // trace directory
			{
				if ( ! ValidatePath( txtTraceDirectory.Text ) )
				{
					MessageBox.Show(this, Constants.Error_InvalidTraceDirectory, this.Text);
					return false;
				}
			}

         if (page == pageAgentTrigger)
         {
            if (!ValidatePath(txtTriggerAssemblyDirectory.Text))
            {
               MessageBox.Show(this, Constants.Error_InvalidAssemblyDirectory, this.Text);
               return false;
            }
         }
			return true;
		}
      
		//--------------------------------------------------------------------
		// ValidateServerName
		//
		//     Check server form
		//     Set member variables: m_computer and m_instance
		//
		//     Cases
		//     ----------------------------------------
		//     xxxxx   - computer="xxxx",  instance = xxxx
		//     .       - computer="",  instance = local
		//     ./xxxx  - computer="",  instance = xxxx
		//     (local) - computer="",  instance = local
		//     hhh/zzz - computer=hhh, instance = zzzz
		//
		//  Note: For cluster support the name must contain virtual server
		//        and instance
		//--------------------------------------------------------------------
		private bool ValidateServerName()
		{
			string localhost = "(LOCAL)";

            string server = GetServerFromSqlServerConnectionString(textSQLServer.Text).Trim().ToUpper();            
            if (textSQLServer.Text.IndexOf(',') != -1)
            { instancePort = GetPortFromSqlServerConnectionString(textSQLServer.Text);
            instanceWithPort = string.Join(",", server, instancePort); }                        
            if (instancePort != null) { instanceName = string.Join(",", server, instancePort); }
            else { instanceName = server; }
			textSQLServer.Text = server;
         
			int pos = server.IndexOf(@"\");
			if ( pos == - 1)
			{
				m_computer = server ;
				m_instance = "" ;
			}
			else if ( pos == 0)
			{
				return false;
			}
			else // pos > 0; we have xxx/yyy
			{
				m_computer = server.Substring(0,pos);
				m_instance = server.Substring(pos+1);
			}
			if (m_computer == "." || m_computer == localhost)
			{
				return false;
			}

			if(!IsValidComputerName(m_computer))
				return false ;
         
			// if we got here, instance name is at least a valid format          
			return true;
		}

		/// <summary>
		/// Verify that the supplied instance name is not already installed
		/// </summary>
		/// <returns></returns>
		private bool IsServerInstalled()
		{
			VirtualServer server = new VirtualServer() ;
			server.ServerName = m_computer ;
			server.InstanceName = m_instance ;
			return VSInstaller.VirtualServerInstalled(server) ;
		}

		private bool IsValidComputerName(string name)
		{
			return IsValidDnsName(name) || IsValidNetBiosName(name) ;
		}

		/// <summary>
		/// http://support.microsoft.com/kb/q188997/
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private bool IsValidNetBiosName(string name)
		{
			if(name == null || name.Length == 0 || name.Length > 15)
				return false ;

			foreach(char c in name.ToCharArray())
			{
				if(!(Char.IsLetterOrDigit(c) ||
					c == '!' || c == '@' ||
					c == '#' || c == '$' ||
					c == '%' || c == '^' ||
					c == '&' || c == '(' ||
					c == ')' || c == '-' ||
					c == '_' || c == '\'' ||
					c == '{' || c == '}' ||
					c == '.' || c == '~'))
					return false ;
			}
			return true; 
		}

		private bool IsValidDnsName(string name)
		{
			if(name == null || name.Length == 0)
				return false ;

			foreach(char c in name.ToCharArray())
			{
				if(!(Char.IsLetterOrDigit(c) ||
					c == '-' || c == '.'))
					return false ;
			}
			return true; 
		}
      
      
		//--------------------------------------------------------------------
		// ValidateAccountName
		//
		// simple validation
		// -----------------
		// form domain\account
		// neither domain or account can be blank
		//--------------------------------------------------------------------
		private bool ValidateAccountName()
		{
			string domain = "";
			string account = "";
         
			string tmp = textServiceAccount.Text.Trim();
         
			int pos = tmp.IndexOf(@"\");
			if ( pos <= 0 )
			{
				return false;
			}
			else
			{
				domain  = tmp.Substring(0,pos);
				account = tmp.Substring(pos+1);
            
				if ((domain == "") || (account == "" ))
					return false;
			}
         
			return true;
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
      
		#endregion

		private void textServiceAccount_TextChanged(object sender, System.EventArgs e)
		{
			btnNext.Enabled = textServiceAccount.Text.Trim() != "";
			if ( btnNext.Enabled )
			{
				this.AcceptButton = btnNext;
			}
		}

		private void txtTraceDirectory_TextChanged(object sender, System.EventArgs e)
		{
         btnNext.Enabled = txtTraceDirectory.Text.Trim() != "";
			if ( btnNext.Enabled )
			{
				this.AcceptButton = btnNext;
			}
		}

      private void txtTriggerAssebmlyDirectory_TextChanged(object sender, System.EventArgs e)
      {
         btnNext.Enabled = txtTriggerAssemblyDirectory.Text.Trim() != "";
         if (btnNext.Enabled)
         {
            this.AcceptButton = btnNext;
         }
      }

		private void txtCollectionServer_TextChanged(object sender, System.EventArgs e)
		{
			btnNext.Enabled = txtCollectionServer.Text.Trim() != "";
			if ( btnNext.Enabled )
			{
				this.AcceptButton = btnNext;
			}
		}

		private void textSQLServer_TextChanged(object sender, System.EventArgs e)
		{
			btnNext.Enabled = textSQLServer.Text.Trim() != "";
			if ( btnNext.Enabled )
			{
				this.AcceptButton = btnNext;
			}
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

        private string GetServerFromSqlServerConnectionString(string sqlServerConnectionString)
        {
            string server = GetStringTillCharacter(sqlServerConnectionString, COMMA_CHARACTER);
            return server.Trim().ToUpper();
        }        

        private int? GetPortFromSqlServerConnectionString(string sqlServerConnectionString)
        {
            int? instancePort = null;
            int parsedValue = 0;
            string port = GetStringAfterCharacter(sqlServerConnectionString, COMMA_CHARACTER).Trim();
            if (int.TryParse(port, out parsedValue))
            {
                instancePort = parsedValue;
            }
            return instancePort;
        }

        private string GetStringTillCharacter(string text, string character)
        {

            if (text.Contains(character))
            {
                return text.Substring(0, text.IndexOf(COMMA_CHARACTER));
            }

            return text;
        }

        private string GetStringAfterCharacter(string text, string character)
        {

            if (text.Contains(character))
            {
                return text.Substring(text.IndexOf(character) + 1).Trim();
            }

            return text;
        }
      
		#region Finish and Save

		//--------------------------------------------------------------------
		// btnFinishClick - Validate input and then create the new
		//                  registered SQL Server
		//--------------------------------------------------------------------
		private void btnFinish_Click(object sender, System.EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor ;
			// Validate Data
			for ( int i=0; i<numPages; i++ )
			{
				if ( ! ValidatePage(i) )
				{
					currentPage = i;
					SetCurrentPage();
					return;
				}
			}
			this.Cursor = Cursors.Default ;
         
			// Save

			//m_instance = textSQLServer.Text;
         
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
		#endregion	

      //--------------------------------------------------------------------
      // btnBrowse_Click - Browse for SQL Servers on the network to select
      //                   one for registration
      //--------------------------------------------------------------------
      private void btnBrowse_Click(object sender, System.EventArgs e)
      {
         
         try
         {
			 Form_SQLServerBrowse dlg = new Form_SQLServerBrowse();
			 
			 if ( DialogResult.OK == dlg.ShowDialog() )
            {
                textSQLServer.Text = dlg.SelectedServer.ToString();   
            }
         }
         catch (Exception ex )
         {
			   MessageBox.Show( this,
			                    String.Format( Constants.Error_DMOLoadServers, ex.Message ),
			                    this.Text,
			                    MessageBoxButtons.OK,
			                    MessageBoxIcon.Error );
         }
      }

      private void Form_Add_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
      {
         string topic = "" ;

         switch(currentPage)
         {
            case 0:
               topic = HelpAlias.CLUSTERHELP_Form_General ;
               break ;
            case 1:
               topic = HelpAlias.CLUSTERHELP_Form_CollectionServer ;
               break ;
            case 2:
               topic = HelpAlias.CLUSTERHELP_Form_AgentServiceAccount ;
               break ;
            case 3:
               topic = HelpAlias.CLUSTERHELP_Form_TraceDirectory ;
               break ;
            case 4:
               topic = HelpAlias.CLUSTERHELP_FORM_AssemblyDirectory;
               break;
            case 5:
               topic = HelpAlias.CLUSTERHELP_Form_Summary ;
               break ;
         }
         if(topic.Length > 0)
         {
            HelpAlias.ShowHelp(this, topic) ;
            hlpevent.Handled = true ;
         }
         /*
         if ( inHelpRequested ) return;
         inHelpRequested = true;
         Cursor = Cursors.WaitCursor;
         
         Help.ShowHelp( this, Constants.Help_ClusterHelpFile );
         
         Cursor = Cursors.Default;
         inHelpRequested = false;*/
      }

	}
}
