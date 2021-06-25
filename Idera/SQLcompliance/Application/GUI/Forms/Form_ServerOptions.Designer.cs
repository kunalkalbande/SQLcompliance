namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_ServerOptions
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

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
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.groupBox2 = new System.Windows.Forms.GroupBox();
         this.heartbeatIntervalText = new System.Windows.Forms.NumericUpDown();
         this.label1 = new System.Windows.Forms.Label();
         this.comboLogLevel = new System.Windows.Forms.ComboBox();
         this.grpService = new System.Windows.Forms.GroupBox();
         this.btnStop = new System.Windows.Forms.Button();
         this.btnStart = new System.Windows.Forms.Button();
         this.textServerLastHeartbeatTime = new System.Windows.Forms.TextBox();
         this.label6 = new System.Windows.Forms.Label();
         this.textActivityLogLevel = new System.Windows.Forms.TextBox();
         this.label18 = new System.Windows.Forms.Label();
         this.label10 = new System.Windows.Forms.Label();
         this.textTraceDirectory = new System.Windows.Forms.TextBox();
         this.label4 = new System.Windows.Forms.Label();
         this.groupServerDown = new System.Windows.Forms.GroupBox();
         this.btnRefreshStatus = new System.Windows.Forms.Button();
         this.lblBigStatus = new System.Windows.Forms.Label();
         this.imgStatus = new System.Windows.Forms.PictureBox();
         this.textServerMachine = new System.Windows.Forms.TextBox();
         this.label5 = new System.Windows.Forms.Label();
         this.textServerStatus = new System.Windows.Forms.TextBox();
         this.label17 = new System.Windows.Forms.Label();
         this.textServerPort = new System.Windows.Forms.TextBox();
         this.textServerCoreVersion = new System.Windows.Forms.TextBox();
         this.textServerVersion = new System.Windows.Forms.TextBox();
         this.label3 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.label13 = new System.Windows.Forms.Label();
         this.groupBox2.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.heartbeatIntervalText)).BeginInit();
         this.grpService.SuspendLayout();
         this.groupServerDown.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.imgStatus)).BeginInit();
         this.SuspendLayout();
         // 
         // btnOK
         // 
         this.btnOK.Location = new System.Drawing.Point(405, 286);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 50;
         this.btnOK.Text = "&OK";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(489, 286);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 51;
         this.btnCancel.Text = "&Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // groupBox2
         // 
         this.groupBox2.Controls.Add(this.heartbeatIntervalText);
         this.groupBox2.Controls.Add(this.label1);
         this.groupBox2.Controls.Add(this.comboLogLevel);
         this.groupBox2.Controls.Add(this.grpService);
         this.groupBox2.Controls.Add(this.textServerLastHeartbeatTime);
         this.groupBox2.Controls.Add(this.label6);
         this.groupBox2.Controls.Add(this.textActivityLogLevel);
         this.groupBox2.Controls.Add(this.label18);
         this.groupBox2.Controls.Add(this.label10);
         this.groupBox2.Controls.Add(this.textTraceDirectory);
         this.groupBox2.Controls.Add(this.label4);
         this.groupBox2.Controls.Add(this.groupServerDown);
         this.groupBox2.Controls.Add(this.textServerMachine);
         this.groupBox2.Controls.Add(this.label5);
         this.groupBox2.Controls.Add(this.textServerStatus);
         this.groupBox2.Controls.Add(this.label17);
         this.groupBox2.Controls.Add(this.textServerPort);
         this.groupBox2.Controls.Add(this.textServerCoreVersion);
         this.groupBox2.Controls.Add(this.textServerVersion);
         this.groupBox2.Controls.Add(this.label3);
         this.groupBox2.Controls.Add(this.label2);
         this.groupBox2.Controls.Add(this.label13);
         this.groupBox2.Location = new System.Drawing.Point(8, 8);
         this.groupBox2.Name = "groupBox2";
         this.groupBox2.Size = new System.Drawing.Size(556, 272);
         this.groupBox2.TabIndex = 52;
         this.groupBox2.TabStop = false;
         this.groupBox2.Text = "Collection Server";
         // 
         // heartbeatIntervalText
         // 
         this.heartbeatIntervalText.Location = new System.Drawing.Point(200, 214);
         this.heartbeatIntervalText.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
         this.heartbeatIntervalText.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
         this.heartbeatIntervalText.Name = "heartbeatIntervalText";
         this.heartbeatIntervalText.Size = new System.Drawing.Size(84, 20);
         this.heartbeatIntervalText.TabIndex = 48;
         this.heartbeatIntervalText.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(12, 216);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(119, 13);
         this.label1.TabIndex = 45;
         this.label1.Text = "Heartbeat interval (min):";
         // 
         // comboLogLevel
         // 
         this.comboLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.comboLogLevel.Items.AddRange(new object[] {
            "Silent",
            "Normal",
            "Verbose",
            "Debug"});
         this.comboLogLevel.Location = new System.Drawing.Point(200, 188);
         this.comboLogLevel.Name = "comboLogLevel";
         this.comboLogLevel.Size = new System.Drawing.Size(84, 21);
         this.comboLogLevel.TabIndex = 44;
         // 
         // grpService
         // 
         this.grpService.Controls.Add(this.btnStop);
         this.grpService.Controls.Add(this.btnStart);
         this.grpService.Location = new System.Drawing.Point(296, 152);
         this.grpService.Name = "grpService";
         this.grpService.Size = new System.Drawing.Size(248, 52);
         this.grpService.TabIndex = 43;
         this.grpService.TabStop = false;
         this.grpService.Text = "Collection Server Service Control";
         // 
         // btnStop
         // 
         this.btnStop.Enabled = false;
         this.btnStop.Location = new System.Drawing.Point(128, 20);
         this.btnStop.Name = "btnStop";
         this.btnStop.Size = new System.Drawing.Size(110, 23);
         this.btnStop.TabIndex = 47;
         this.btnStop.Text = "Sto&p Service";
         this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
         // 
         // btnStart
         // 
         this.btnStart.Enabled = false;
         this.btnStart.Location = new System.Drawing.Point(10, 20);
         this.btnStart.Name = "btnStart";
         this.btnStart.Size = new System.Drawing.Size(110, 23);
         this.btnStart.TabIndex = 46;
         this.btnStart.Text = "&Start Service";
         this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
         // 
         // textServerLastHeartbeatTime
         // 
         this.textServerLastHeartbeatTime.Location = new System.Drawing.Point(128, 140);
         this.textServerLastHeartbeatTime.Name = "textServerLastHeartbeatTime";
         this.textServerLastHeartbeatTime.ReadOnly = true;
         this.textServerLastHeartbeatTime.Size = new System.Drawing.Size(156, 20);
         this.textServerLastHeartbeatTime.TabIndex = 42;
         this.textServerLastHeartbeatTime.TabStop = false;
         this.textServerLastHeartbeatTime.Text = "12/121/2005 12:56PM";
         // 
         // label6
         // 
         this.label6.Location = new System.Drawing.Point(12, 144);
         this.label6.Name = "label6";
         this.label6.Size = new System.Drawing.Size(116, 14);
         this.label6.TabIndex = 41;
         this.label6.Text = "Last server heartbeat:";
         // 
         // textActivityLogLevel
         // 
         this.textActivityLogLevel.Location = new System.Drawing.Point(220, 164);
         this.textActivityLogLevel.Name = "textActivityLogLevel";
         this.textActivityLogLevel.ReadOnly = true;
         this.textActivityLogLevel.Size = new System.Drawing.Size(64, 20);
         this.textActivityLogLevel.TabIndex = 40;
         this.textActivityLogLevel.TabStop = false;
         // 
         // label18
         // 
         this.label18.Location = new System.Drawing.Point(12, 168);
         this.label18.Name = "label18";
         this.label18.Size = new System.Drawing.Size(216, 16);
         this.label18.TabIndex = 38;
         this.label18.Text = "SQLcompliance Agent Activity Log Level:";
         // 
         // label10
         // 
         this.label10.Location = new System.Drawing.Point(12, 190);
         this.label10.Name = "label10";
         this.label10.Size = new System.Drawing.Size(148, 16);
         this.label10.TabIndex = 37;
         this.label10.Text = "Collection Server Log Level:";
         // 
         // textTraceDirectory
         // 
         this.textTraceDirectory.Location = new System.Drawing.Point(124, 239);
         this.textTraceDirectory.Name = "textTraceDirectory";
         this.textTraceDirectory.ReadOnly = true;
         this.textTraceDirectory.Size = new System.Drawing.Size(420, 20);
         this.textTraceDirectory.TabIndex = 33;
         this.textTraceDirectory.TabStop = false;
         // 
         // label4
         // 
         this.label4.Location = new System.Drawing.Point(12, 242);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(104, 16);
         this.label4.TabIndex = 32;
         this.label4.Text = "Trace file directory:";
         // 
         // groupServerDown
         // 
         this.groupServerDown.Controls.Add(this.btnRefreshStatus);
         this.groupServerDown.Controls.Add(this.lblBigStatus);
         this.groupServerDown.Controls.Add(this.imgStatus);
         this.groupServerDown.Location = new System.Drawing.Point(296, 14);
         this.groupServerDown.Name = "groupServerDown";
         this.groupServerDown.Size = new System.Drawing.Size(248, 130);
         this.groupServerDown.TabIndex = 29;
         this.groupServerDown.TabStop = false;
         // 
         // btnRefreshStatus
         // 
         this.btnRefreshStatus.Location = new System.Drawing.Point(128, 100);
         this.btnRefreshStatus.Name = "btnRefreshStatus";
         this.btnRefreshStatus.Size = new System.Drawing.Size(112, 23);
         this.btnRefreshStatus.TabIndex = 44;
         this.btnRefreshStatus.Text = "&Refresh Status";
         this.btnRefreshStatus.Click += new System.EventHandler(this.btnRefreshStatus_Click);
         // 
         // lblBigStatus
         // 
         this.lblBigStatus.Location = new System.Drawing.Point(80, 12);
         this.lblBigStatus.Name = "lblBigStatus";
         this.lblBigStatus.Size = new System.Drawing.Size(156, 80);
         this.lblBigStatus.TabIndex = 43;
         this.lblBigStatus.Text = "The Collection Server is not operational or is inaccessible. When the server is n" +
             "on-operational, no events can be collected from audited SQL Servers.";
         this.lblBigStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // imgStatus
         // 
         this.imgStatus.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.StatusError_48;
         this.imgStatus.Location = new System.Drawing.Point(16, 24);
         this.imgStatus.Name = "imgStatus";
         this.imgStatus.Size = new System.Drawing.Size(48, 48);
         this.imgStatus.TabIndex = 42;
         this.imgStatus.TabStop = false;
         // 
         // textServerMachine
         // 
         this.textServerMachine.Location = new System.Drawing.Point(128, 44);
         this.textServerMachine.Name = "textServerMachine";
         this.textServerMachine.ReadOnly = true;
         this.textServerMachine.Size = new System.Drawing.Size(156, 20);
         this.textServerMachine.TabIndex = 28;
         this.textServerMachine.TabStop = false;
         // 
         // label5
         // 
         this.label5.Location = new System.Drawing.Point(12, 48);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(64, 14);
         this.label5.TabIndex = 27;
         this.label5.Text = "Machine:";
         // 
         // textServerStatus
         // 
         this.textServerStatus.Location = new System.Drawing.Point(128, 20);
         this.textServerStatus.Name = "textServerStatus";
         this.textServerStatus.ReadOnly = true;
         this.textServerStatus.Size = new System.Drawing.Size(156, 20);
         this.textServerStatus.TabIndex = 26;
         this.textServerStatus.TabStop = false;
         this.textServerStatus.Text = "Pending";
         // 
         // label17
         // 
         this.label17.Location = new System.Drawing.Point(12, 24);
         this.label17.Name = "label17";
         this.label17.Size = new System.Drawing.Size(76, 16);
         this.label17.TabIndex = 25;
         this.label17.Text = "Status:";
         // 
         // textServerPort
         // 
         this.textServerPort.Location = new System.Drawing.Point(128, 68);
         this.textServerPort.Name = "textServerPort";
         this.textServerPort.ReadOnly = true;
         this.textServerPort.Size = new System.Drawing.Size(156, 20);
         this.textServerPort.TabIndex = 22;
         this.textServerPort.TabStop = false;
         // 
         // textServerCoreVersion
         // 
         this.textServerCoreVersion.Location = new System.Drawing.Point(128, 116);
         this.textServerCoreVersion.Name = "textServerCoreVersion";
         this.textServerCoreVersion.ReadOnly = true;
         this.textServerCoreVersion.Size = new System.Drawing.Size(156, 20);
         this.textServerCoreVersion.TabIndex = 21;
         this.textServerCoreVersion.TabStop = false;
         // 
         // textServerVersion
         // 
         this.textServerVersion.Location = new System.Drawing.Point(128, 92);
         this.textServerVersion.Name = "textServerVersion";
         this.textServerVersion.ReadOnly = true;
         this.textServerVersion.Size = new System.Drawing.Size(156, 20);
         this.textServerVersion.TabIndex = 20;
         this.textServerVersion.TabStop = false;
         // 
         // label3
         // 
         this.label3.Location = new System.Drawing.Point(12, 120);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(110, 14);
         this.label3.TabIndex = 19;
         this.label3.Text = "Core version:";
         // 
         // label2
         // 
         this.label2.Location = new System.Drawing.Point(12, 96);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(82, 14);
         this.label2.TabIndex = 18;
         this.label2.Text = "Version:";
         // 
         // label13
         // 
         this.label13.Location = new System.Drawing.Point(12, 72);
         this.label13.Name = "label13";
         this.label13.Size = new System.Drawing.Size(64, 14);
         this.label13.TabIndex = 17;
         this.label13.Text = "Port:";
         // 
         // Form_ServerOptions
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(570, 319);
         this.Controls.Add(this.groupBox2);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_ServerOptions";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Collection Server Status";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_Defaults_HelpRequested);
         this.groupBox2.ResumeLayout(false);
         this.groupBox2.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.heartbeatIntervalText)).EndInit();
         this.grpService.ResumeLayout(false);
         this.groupServerDown.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.imgStatus)).EndInit();
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.GroupBox groupBox2;
      private System.Windows.Forms.TextBox textServerLastHeartbeatTime;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.TextBox textActivityLogLevel;
      private System.Windows.Forms.Label label18;
      private System.Windows.Forms.Label label10;
      private System.Windows.Forms.TextBox textTraceDirectory;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.GroupBox groupServerDown;
      private System.Windows.Forms.Button btnRefreshStatus;
      private System.Windows.Forms.Label lblBigStatus;
      private System.Windows.Forms.PictureBox imgStatus;
      private System.Windows.Forms.TextBox textServerMachine;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.TextBox textServerStatus;
      private System.Windows.Forms.Label label17;
      private System.Windows.Forms.TextBox textServerPort;
      private System.Windows.Forms.TextBox textServerCoreVersion;
      private System.Windows.Forms.TextBox textServerVersion;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label13;
      private System.Windows.Forms.GroupBox grpService;
      private System.Windows.Forms.Button btnStop;
      private System.Windows.Forms.Button btnStart;
      private System.Windows.Forms.ComboBox comboLogLevel;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.NumericUpDown heartbeatIntervalText;
	}
}