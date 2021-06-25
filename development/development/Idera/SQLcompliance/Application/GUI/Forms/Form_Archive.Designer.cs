namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_Archive
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Archive));
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.label4 = new System.Windows.Forms.Label();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.comboServer = new System.Windows.Forms.ComboBox();
         this.radioServer = new System.Windows.Forms.RadioButton();
         this.radioAll = new System.Windows.Forms.RadioButton();
         this._lblCurrentRight = new System.Windows.Forms.Label();
         this.btnShowAutoArchivePreferences = new System.Windows.Forms.Button();
         this._btnScript = new System.Windows.Forms.Button();
         this._lblIntegrityRight = new System.Windows.Forms.Label();
         this._lblCurentLeft = new System.Windows.Forms.Label();
         this._lblIntegrityLeft = new System.Windows.Forms.Label();
         this.groupBox1.SuspendLayout();
         this.SuspendLayout();
         // 
         // btnOK
         // 
         this.btnOK.Location = new System.Drawing.Point(336, 176);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 0;
         this.btnOK.Text = "&OK";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(416, 176);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 1;
         this.btnCancel.Text = "&Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // label4
         // 
         this.label4.Location = new System.Drawing.Point(8, 8);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(484, 44);
         this.label4.TabIndex = 16;
         this.label4.Text = resources.GetString("label4.Text");
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.comboServer);
         this.groupBox1.Controls.Add(this.radioServer);
         this.groupBox1.Controls.Add(this.radioAll);
         this.groupBox1.Location = new System.Drawing.Point(8, 52);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(480, 76);
         this.groupBox1.TabIndex = 17;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "Select SQL Servers to Archive";
         // 
         // comboServer
         // 
         this.comboServer.Enabled = false;
         this.comboServer.Location = new System.Drawing.Point(140, 44);
         this.comboServer.Name = "comboServer";
         this.comboServer.Size = new System.Drawing.Size(320, 21);
         this.comboServer.TabIndex = 7;
         // 
         // radioServer
         // 
         this.radioServer.Location = new System.Drawing.Point(8, 44);
         this.radioServer.Name = "radioServer";
         this.radioServer.Size = new System.Drawing.Size(132, 24);
         this.radioServer.TabIndex = 6;
         this.radioServer.Text = "Archive audit data for ";
         this.radioServer.CheckedChanged += new System.EventHandler(this.radioServer_CheckedChanged);
         // 
         // radioAll
         // 
         this.radioAll.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
         this.radioAll.Checked = true;
         this.radioAll.Location = new System.Drawing.Point(8, 20);
         this.radioAll.Name = "radioAll";
         this.radioAll.Size = new System.Drawing.Size(444, 16);
         this.radioAll.TabIndex = 5;
         this.radioAll.TabStop = true;
         this.radioAll.Text = "Archive events for all registered SQL Servers now.";
         this.radioAll.CheckedChanged += new System.EventHandler(this.radioServer_CheckedChanged);
         // 
         // _lblCurrentRight
         // 
         this._lblCurrentRight.Location = new System.Drawing.Point(136, 132);
         this._lblCurrentRight.Name = "_lblCurrentRight";
         this._lblCurrentRight.Size = new System.Drawing.Size(340, 16);
         this._lblCurrentRight.TabIndex = 18;
         this._lblCurrentRight.Text = "Current Archive Setting: Move events older then 90 days to an archive database.";
         // 
         // btnShowAutoArchivePreferences
         // 
         this.btnShowAutoArchivePreferences.Location = new System.Drawing.Point(12, 176);
         this.btnShowAutoArchivePreferences.Name = "btnShowAutoArchivePreferences";
         this.btnShowAutoArchivePreferences.Size = new System.Drawing.Size(116, 23);
         this.btnShowAutoArchivePreferences.TabIndex = 19;
         this.btnShowAutoArchivePreferences.Text = "Archive Preferences";
         this.btnShowAutoArchivePreferences.Click += new System.EventHandler(this.btnShowAutoArchivePreferences_Click);
         // 
         // _btnScript
         // 
         this._btnScript.Location = new System.Drawing.Point(144, 176);
         this._btnScript.Name = "_btnScript";
         this._btnScript.Size = new System.Drawing.Size(92, 23);
         this._btnScript.TabIndex = 20;
         this._btnScript.Text = "Generate Script";
         this._btnScript.Click += new System.EventHandler(this.Click_btnScript);
         // 
         // _lblIntegrityRight
         // 
         this._lblIntegrityRight.Location = new System.Drawing.Point(136, 152);
         this._lblIntegrityRight.Name = "_lblIntegrityRight";
         this._lblIntegrityRight.Size = new System.Drawing.Size(268, 16);
         this._lblIntegrityRight.TabIndex = 21;
         this._lblIntegrityRight.Text = "Skip Integrity Check:  Yes";
         // 
         // _lblCurentLeft
         // 
         this._lblCurentLeft.Location = new System.Drawing.Point(8, 132);
         this._lblCurentLeft.Name = "_lblCurentLeft";
         this._lblCurentLeft.Size = new System.Drawing.Size(124, 16);
         this._lblCurentLeft.TabIndex = 22;
         this._lblCurentLeft.Text = "Current Archive Setting:";
         // 
         // _lblIntegrityLeft
         // 
         this._lblIntegrityLeft.Location = new System.Drawing.Point(8, 152);
         this._lblIntegrityLeft.Name = "_lblIntegrityLeft";
         this._lblIntegrityLeft.Size = new System.Drawing.Size(124, 16);
         this._lblIntegrityLeft.TabIndex = 23;
         this._lblIntegrityLeft.Text = "Skip Integrity Check:";
         // 
         // Form_Archive
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(498, 208);
         this.Controls.Add(this._lblIntegrityLeft);
         this.Controls.Add(this._lblCurentLeft);
         this.Controls.Add(this._lblIntegrityRight);
         this.Controls.Add(this._btnScript);
         this.Controls.Add(this.btnShowAutoArchivePreferences);
         this.Controls.Add(this._lblCurrentRight);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_Archive";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Archive Audit Data Now";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_Defaults_HelpRequested);
         this.groupBox1.ResumeLayout(false);
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.ComboBox comboServer;
      private System.Windows.Forms.RadioButton radioServer;
      private System.Windows.Forms.RadioButton radioAll;
      private System.Windows.Forms.Button btnShowAutoArchivePreferences;
      private System.Windows.Forms.Button _btnScript;
      private System.Windows.Forms.Label _lblCurrentRight;
      private System.Windows.Forms.Label _lblCurentLeft;
      private System.Windows.Forms.Label _lblIntegrityLeft;
      private System.Windows.Forms.Label _lblIntegrityRight;

	}
}