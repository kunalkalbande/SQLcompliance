namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_ServerDeploy
	{
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.Container components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
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

      #region Windows Form Designer generated code
      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_ServerDeploy));
         this.panelButtons = new System.Windows.Forms.Panel();
         this.btnCancel = new System.Windows.Forms.Button();
         this.btnFinish = new System.Windows.Forms.Button();
         this.btnNext = new System.Windows.Forms.Button();
         this.btnBack = new System.Windows.Forms.Button();
         this.panelLeft = new System.Windows.Forms.Panel();
         this.pictureBox1 = new System.Windows.Forms.PictureBox();
         this.panelAgentService = new System.Windows.Forms.Panel();
         this.panelAgentProperties = new System.Windows.Forms.Panel();
         this.label6 = new System.Windows.Forms.Label();
         this.groupBox2 = new System.Windows.Forms.GroupBox();
         this.label1 = new System.Windows.Forms.Label();
         this.textServicePasswordConfirm = new System.Windows.Forms.TextBox();
         this.textServicePassword = new System.Windows.Forms.TextBox();
         this.label2 = new System.Windows.Forms.Label();
         this.textServiceAccount = new System.Windows.Forms.TextBox();
         this.label3 = new System.Windows.Forms.Label();
         this.labelAgentDescription = new System.Windows.Forms.Label();
         this.labelAgentTitle = new System.Windows.Forms.Label();
         this.panelSummary = new System.Windows.Forms.Panel();
         this.label8 = new System.Windows.Forms.Label();
         this.label9 = new System.Windows.Forms.Label();
         this.panel4 = new System.Windows.Forms.Panel();
         this.textSummaryDirectory = new System.Windows.Forms.TextBox();
         this.textSummaryAccount = new System.Windows.Forms.TextBox();
         this.textSummaryComputer = new System.Windows.Forms.TextBox();
         this.label5 = new System.Windows.Forms.Label();
         this.label4 = new System.Windows.Forms.Label();
         this.lblInstance = new System.Windows.Forms.Label();
         this.label11 = new System.Windows.Forms.Label();
         this.label10 = new System.Windows.Forms.Label();
         this.panelAgentTrace = new System.Windows.Forms.Panel();
         this.panel7 = new System.Windows.Forms.Panel();
         this.label13 = new System.Windows.Forms.Label();
         this.label12 = new System.Windows.Forms.Label();
         this.txtTraceDirectory = new System.Windows.Forms.TextBox();
         this.radioSpecifyTrace = new System.Windows.Forms.RadioButton();
         this.radioDefaultTrace = new System.Windows.Forms.RadioButton();
         this.labeAgentTraceDescription = new System.Windows.Forms.Label();
         this.labelAgentTraceTitle = new System.Windows.Forms.Label();
         this.panelButtons.SuspendLayout();
         this.panelLeft.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
         this.panelAgentService.SuspendLayout();
         this.panelAgentProperties.SuspendLayout();
         this.groupBox2.SuspendLayout();
         this.panelSummary.SuspendLayout();
         this.panel4.SuspendLayout();
         this.panelAgentTrace.SuspendLayout();
         this.panel7.SuspendLayout();
         this.SuspendLayout();
         // 
         // panelButtons
         // 
         this.panelButtons.Controls.Add(this.btnCancel);
         this.panelButtons.Controls.Add(this.btnFinish);
         this.panelButtons.Controls.Add(this.btnNext);
         this.panelButtons.Controls.Add(this.btnBack);
         this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.panelButtons.Location = new System.Drawing.Point(0, 334);
         this.panelButtons.Name = "panelButtons";
         this.panelButtons.Size = new System.Drawing.Size(558, 38);
         this.panelButtons.TabIndex = 9;
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
         this.panelLeft.TabIndex = 10;
         // 
         // pictureBox1
         // 
         this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
         this.pictureBox1.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.Wizard_Server;
         this.pictureBox1.Location = new System.Drawing.Point(0, 0);
         this.pictureBox1.Name = "pictureBox1";
         this.pictureBox1.Size = new System.Drawing.Size(112, 336);
         this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
         this.pictureBox1.TabIndex = 0;
         this.pictureBox1.TabStop = false;
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
         this.panelAgentService.TabIndex = 11;
         // 
         // panelAgentProperties
         // 
         this.panelAgentProperties.BackColor = System.Drawing.SystemColors.Control;
         this.panelAgentProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panelAgentProperties.Controls.Add(this.label6);
         this.panelAgentProperties.Controls.Add(this.groupBox2);
         this.panelAgentProperties.Location = new System.Drawing.Point(0, 60);
         this.panelAgentProperties.Name = "panelAgentProperties";
         this.panelAgentProperties.Size = new System.Drawing.Size(448, 276);
         this.panelAgentProperties.TabIndex = 53;
         // 
         // label6
         // 
         this.label6.Location = new System.Drawing.Point(36, 128);
         this.label6.Name = "label6";
         this.label6.Size = new System.Drawing.Size(368, 56);
         this.label6.TabIndex = 52;
         this.label6.Text = resources.GetString("label6.Text");
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
         this.groupBox2.Size = new System.Drawing.Size(396, 104);
         this.groupBox2.TabIndex = 51;
         this.groupBox2.TabStop = false;
         this.groupBox2.Text = "SQLcompliance Agent service account:";
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
         this.textServicePasswordConfirm.MaxLength = 255;
         this.textServicePasswordConfirm.Name = "textServicePasswordConfirm";
         this.textServicePasswordConfirm.PasswordChar = '*';
         this.textServicePasswordConfirm.Size = new System.Drawing.Size(223, 20);
         this.textServicePasswordConfirm.TabIndex = 48;
         // 
         // textServicePassword
         // 
         this.textServicePassword.Location = new System.Drawing.Point(156, 45);
         this.textServicePassword.MaxLength = 255;
         this.textServicePassword.Name = "textServicePassword";
         this.textServicePassword.PasswordChar = '*';
         this.textServicePassword.Size = new System.Drawing.Size(223, 20);
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
         this.labelAgentDescription.Text = "Specify the SQLcompliance Agent service options. This account needs to be given S" +
             "QL Server Administrator privileges on the registered SQL Server.";
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
         this.panelSummary.TabIndex = 27;
         // 
         // label8
         // 
         this.label8.Location = new System.Drawing.Point(14, 24);
         this.label8.Name = "label8";
         this.label8.Size = new System.Drawing.Size(420, 28);
         this.label8.TabIndex = 15;
         this.label8.Text = "Ready to deploy the SQLcompliance Agent";
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
         this.panel4.Controls.Add(this.textSummaryDirectory);
         this.panel4.Controls.Add(this.textSummaryAccount);
         this.panel4.Controls.Add(this.textSummaryComputer);
         this.panel4.Controls.Add(this.label5);
         this.panel4.Controls.Add(this.label4);
         this.panel4.Controls.Add(this.lblInstance);
         this.panel4.Controls.Add(this.label11);
         this.panel4.Controls.Add(this.label10);
         this.panel4.Location = new System.Drawing.Point(0, 60);
         this.panel4.Name = "panel4";
         this.panel4.Size = new System.Drawing.Size(448, 276);
         this.panel4.TabIndex = 16;
         // 
         // textSummaryDirectory
         // 
         this.textSummaryDirectory.Location = new System.Drawing.Point(120, 100);
         this.textSummaryDirectory.Name = "textSummaryDirectory";
         this.textSummaryDirectory.ReadOnly = true;
         this.textSummaryDirectory.Size = new System.Drawing.Size(284, 20);
         this.textSummaryDirectory.TabIndex = 7;
         this.textSummaryDirectory.Text = "Computer";
         // 
         // textSummaryAccount
         // 
         this.textSummaryAccount.Location = new System.Drawing.Point(120, 76);
         this.textSummaryAccount.Name = "textSummaryAccount";
         this.textSummaryAccount.ReadOnly = true;
         this.textSummaryAccount.Size = new System.Drawing.Size(284, 20);
         this.textSummaryAccount.TabIndex = 6;
         this.textSummaryAccount.Text = "Account";
         // 
         // textSummaryComputer
         // 
         this.textSummaryComputer.Location = new System.Drawing.Point(120, 52);
         this.textSummaryComputer.Name = "textSummaryComputer";
         this.textSummaryComputer.ReadOnly = true;
         this.textSummaryComputer.Size = new System.Drawing.Size(284, 20);
         this.textSummaryComputer.TabIndex = 5;
         this.textSummaryComputer.Text = "Computer";
         // 
         // label5
         // 
         this.label5.Location = new System.Drawing.Point(28, 104);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(88, 16);
         this.label5.TabIndex = 4;
         this.label5.Text = "Trace Directory:";
         // 
         // label4
         // 
         this.label4.Location = new System.Drawing.Point(28, 80);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(92, 16);
         this.label4.TabIndex = 3;
         this.label4.Text = "Service Account:";
         // 
         // lblInstance
         // 
         this.lblInstance.Location = new System.Drawing.Point(28, 56);
         this.lblInstance.Name = "lblInstance";
         this.lblInstance.Size = new System.Drawing.Size(60, 16);
         this.lblInstance.TabIndex = 2;
         this.lblInstance.Text = "Computer:";
         // 
         // label11
         // 
         this.label11.Location = new System.Drawing.Point(12, 12);
         this.label11.Name = "label11";
         this.label11.Size = new System.Drawing.Size(420, 28);
         this.label11.TabIndex = 1;
         this.label11.Text = "You are ready to deploy the SQLcompliance Agent to the computer hosting this SQL " +
             "Server instance and begin auditing using the following deployment options:";
         // 
         // label10
         // 
         this.label10.Location = new System.Drawing.Point(12, 136);
         this.label10.Name = "label10";
         this.label10.Size = new System.Drawing.Size(412, 44);
         this.label10.TabIndex = 0;
         this.label10.Text = "Click Finish to begin the deployment process. This may take several minutes as it" +
             " installs the SQLcompliance Agent on the remote computer.";
         // 
         // panelAgentTrace
         // 
         this.panelAgentTrace.BackColor = System.Drawing.SystemColors.ControlLightLight;
         this.panelAgentTrace.Controls.Add(this.panel7);
         this.panelAgentTrace.Controls.Add(this.labeAgentTraceDescription);
         this.panelAgentTrace.Controls.Add(this.labelAgentTraceTitle);
         this.panelAgentTrace.Location = new System.Drawing.Point(111, 0);
         this.panelAgentTrace.Name = "panelAgentTrace";
         this.panelAgentTrace.Size = new System.Drawing.Size(448, 336);
         this.panelAgentTrace.TabIndex = 29;
         // 
         // panel7
         // 
         this.panel7.BackColor = System.Drawing.SystemColors.Control;
         this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.panel7.Controls.Add(this.label13);
         this.panel7.Controls.Add(this.label12);
         this.panel7.Controls.Add(this.txtTraceDirectory);
         this.panel7.Controls.Add(this.radioSpecifyTrace);
         this.panel7.Controls.Add(this.radioDefaultTrace);
         this.panel7.Location = new System.Drawing.Point(0, 60);
         this.panel7.Name = "panel7";
         this.panel7.Size = new System.Drawing.Size(448, 276);
         this.panel7.TabIndex = 53;
         // 
         // label13
         // 
         this.label13.Location = new System.Drawing.Point(12, 12);
         this.label13.Name = "label13";
         this.label13.Size = new System.Drawing.Size(424, 40);
         this.label13.TabIndex = 4;
         this.label13.Text = resources.GetString("label13.Text");
         // 
         // label12
         // 
         this.label12.Location = new System.Drawing.Point(48, 164);
         this.label12.Name = "label12";
         this.label12.Size = new System.Drawing.Size(368, 24);
         this.label12.TabIndex = 3;
         this.label12.Text = "Note:  This directory will be created during the agent installation process.";
         // 
         // txtTraceDirectory
         // 
         this.txtTraceDirectory.Enabled = false;
         this.txtTraceDirectory.Location = new System.Drawing.Point(48, 136);
         this.txtTraceDirectory.MaxLength = 255;
         this.txtTraceDirectory.Name = "txtTraceDirectory";
         this.txtTraceDirectory.Size = new System.Drawing.Size(348, 20);
         this.txtTraceDirectory.TabIndex = 2;
         // 
         // radioSpecifyTrace
         // 
         this.radioSpecifyTrace.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioSpecifyTrace.Location = new System.Drawing.Point(32, 116);
         this.radioSpecifyTrace.Name = "radioSpecifyTrace";
         this.radioSpecifyTrace.Size = new System.Drawing.Size(364, 20);
         this.radioSpecifyTrace.TabIndex = 1;
         this.radioSpecifyTrace.Text = "Specify alternate trace directory:";
         this.radioSpecifyTrace.CheckedChanged += new System.EventHandler(this.radioSpecifyTrace_CheckedChanged);
         // 
         // radioDefaultTrace
         // 
         this.radioDefaultTrace.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioDefaultTrace.Checked = true;
         this.radioDefaultTrace.Location = new System.Drawing.Point(32, 64);
         this.radioDefaultTrace.Name = "radioDefaultTrace";
         this.radioDefaultTrace.Size = new System.Drawing.Size(380, 40);
         this.radioDefaultTrace.TabIndex = 0;
         this.radioDefaultTrace.TabStop = true;
         this.radioDefaultTrace.Text = "Use default trace directory - By default, the SQLcompliance Agent will store coll" +
             "ected audit data in a protected subdirectory of the SQLcompliance Agent installa" +
             "tion directory.";
         this.radioDefaultTrace.CheckedChanged += new System.EventHandler(this.radioSpecifyTrace_CheckedChanged);
         // 
         // labeAgentTraceDescription
         // 
         this.labeAgentTraceDescription.Location = new System.Drawing.Point(14, 24);
         this.labeAgentTraceDescription.Name = "labeAgentTraceDescription";
         this.labeAgentTraceDescription.Size = new System.Drawing.Size(420, 28);
         this.labeAgentTraceDescription.TabIndex = 52;
         this.labeAgentTraceDescription.Text = "Specify directory for temporary storage of audit data";
         // 
         // labelAgentTraceTitle
         // 
         this.labelAgentTraceTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.labelAgentTraceTitle.Location = new System.Drawing.Point(14, 8);
         this.labelAgentTraceTitle.Name = "labelAgentTraceTitle";
         this.labelAgentTraceTitle.Size = new System.Drawing.Size(384, 16);
         this.labelAgentTraceTitle.TabIndex = 51;
         this.labelAgentTraceTitle.Text = "SQLcompliance Agent Trace Directory";
         // 
         // Form_ServerDeploy
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(558, 372);
         this.Controls.Add(this.panelLeft);
         this.Controls.Add(this.panelButtons);
         this.Controls.Add(this.panelAgentTrace);
         this.Controls.Add(this.panelAgentService);
         this.Controls.Add(this.panelSummary);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_ServerDeploy";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Deploy SQLcompliance Agent";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_ServerDeploy_HelpRequested);
         this.panelButtons.ResumeLayout(false);
         this.panelLeft.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
         this.panelAgentService.ResumeLayout(false);
         this.panelAgentProperties.ResumeLayout(false);
         this.groupBox2.ResumeLayout(false);
         this.groupBox2.PerformLayout();
         this.panelSummary.ResumeLayout(false);
         this.panel4.ResumeLayout(false);
         this.panel4.PerformLayout();
         this.panelAgentTrace.ResumeLayout(false);
         this.panel7.ResumeLayout(false);
         this.panel7.PerformLayout();
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.Button btnFinish;
      private System.Windows.Forms.Button btnNext;
      private System.Windows.Forms.Button btnBack;
      private System.Windows.Forms.Panel panelAgentService;
      private System.Windows.Forms.Panel panelButtons;
      private System.Windows.Forms.Panel panelLeft;
      private System.Windows.Forms.Label labelAgentDescription;
      private System.Windows.Forms.Label labelAgentTitle;
      private System.Windows.Forms.Panel panelAgentProperties;
      private System.Windows.Forms.GroupBox groupBox2;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox textServicePasswordConfirm;
      private System.Windows.Forms.TextBox textServicePassword;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.TextBox textServiceAccount;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.PictureBox pictureBox1;
      private System.Windows.Forms.Panel panelSummary;
      private System.Windows.Forms.Label label8;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.Panel panel4;
      private System.Windows.Forms.Label label10;
      private System.Windows.Forms.Label label11;
      private System.Windows.Forms.Label lblInstance;
      private System.Windows.Forms.Panel panelAgentTrace;
      private System.Windows.Forms.Panel panel7;
      private System.Windows.Forms.Label labeAgentTraceDescription;
      private System.Windows.Forms.Label labelAgentTraceTitle;
      private System.Windows.Forms.RadioButton radioDefaultTrace;
      private System.Windows.Forms.Label label12;
      private System.Windows.Forms.RadioButton radioSpecifyTrace;
      private System.Windows.Forms.TextBox txtTraceDirectory;
      private System.Windows.Forms.Label label13;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.TextBox textSummaryComputer;
      private System.Windows.Forms.TextBox textSummaryAccount;
      private System.Windows.Forms.TextBox textSummaryDirectory;
      private System.Windows.Forms.Label label6;
	}
}