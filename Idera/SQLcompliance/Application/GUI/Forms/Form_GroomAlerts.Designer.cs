namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_GroomAlerts
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
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.label1 = new System.Windows.Forms.Label();
         this.pictureBox1 = new System.Windows.Forms.PictureBox();
         this.groupBox2 = new System.Windows.Forms.GroupBox();
         this.textAge = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
         this.label9 = new System.Windows.Forms.Label();
         this.label8 = new System.Windows.Forms.Label();
         this.groupBox3 = new System.Windows.Forms.GroupBox();
         this.comboServer = new System.Windows.Forms.ComboBox();
         this.radioServer = new System.Windows.Forms.RadioButton();
         this.radioAll = new System.Windows.Forms.RadioButton();
         this.groupBox1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
         this.groupBox2.SuspendLayout();
         this.groupBox3.SuspendLayout();
         this.SuspendLayout();
         // 
         // btnOK
         // 
         this.btnOK.Location = new System.Drawing.Point(276, 200);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(68, 23);
         this.btnOK.TabIndex = 2;
         this.btnOK.Text = "&OK";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(348, 200);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(64, 23);
         this.btnCancel.TabIndex = 3;
         this.btnCancel.Text = "&Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.label1);
         this.groupBox1.Controls.Add(this.pictureBox1);
         this.groupBox1.Location = new System.Drawing.Point(12, 144);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(252, 80);
         this.groupBox1.TabIndex = 4;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "Warning";
         // 
         // label1
         // 
         this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label1.Location = new System.Drawing.Point(72, 16);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(176, 36);
         this.label1.TabIndex = 1;
         this.label1.Text = "Grooming will permanently delete alerts on the selected SQL Server instances.";
         // 
         // pictureBox1
         // 
         this.pictureBox1.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.StatusWarning_48;
         this.pictureBox1.Location = new System.Drawing.Point(12, 24);
         this.pictureBox1.Name = "pictureBox1";
         this.pictureBox1.Size = new System.Drawing.Size(48, 48);
         this.pictureBox1.TabIndex = 0;
         this.pictureBox1.TabStop = false;
         // 
         // groupBox2
         // 
         this.groupBox2.Controls.Add(this.textAge);
         this.groupBox2.Controls.Add(this.label9);
         this.groupBox2.Controls.Add(this.label8);
         this.groupBox2.Location = new System.Drawing.Point(12, 88);
         this.groupBox2.Name = "groupBox2";
         this.groupBox2.Size = new System.Drawing.Size(400, 48);
         this.groupBox2.TabIndex = 1;
         this.groupBox2.TabStop = false;
         this.groupBox2.Text = "Grooming Options";
         // 
         // textAge
         // 
         this.textAge.Location = new System.Drawing.Point(192, 16);
         this.textAge.MaxLength = 3;
         this.textAge.Name = "textAge";
         this.textAge.Size = new System.Drawing.Size(32, 20);
         this.textAge.TabIndex = 21;
         this.textAge.Text = "30";
         // 
         // label9
         // 
         this.label9.Location = new System.Drawing.Point(228, 20);
         this.label9.Name = "label9";
         this.label9.Size = new System.Drawing.Size(144, 16);
         this.label9.TabIndex = 1;
         this.label9.Text = "&days from the Repository.";
         // 
         // label8
         // 
         this.label8.Location = new System.Drawing.Point(8, 20);
         this.label8.Name = "label8";
         this.label8.Size = new System.Drawing.Size(188, 16);
         this.label8.TabIndex = 0;
         this.label8.Text = "Permanently delete alerts older than";
         // 
         // groupBox3
         // 
         this.groupBox3.Controls.Add(this.comboServer);
         this.groupBox3.Controls.Add(this.radioServer);
         this.groupBox3.Controls.Add(this.radioAll);
         this.groupBox3.Location = new System.Drawing.Point(12, 8);
         this.groupBox3.Name = "groupBox3";
         this.groupBox3.Size = new System.Drawing.Size(400, 72);
         this.groupBox3.TabIndex = 0;
         this.groupBox3.TabStop = false;
         this.groupBox3.Text = "SQL Servers";
         // 
         // comboServer
         // 
         this.comboServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.comboServer.Enabled = false;
         this.comboServer.Location = new System.Drawing.Point(160, 40);
         this.comboServer.Name = "comboServer";
         this.comboServer.Size = new System.Drawing.Size(228, 21);
         this.comboServer.TabIndex = 2;
         // 
         // radioServer
         // 
         this.radioServer.Location = new System.Drawing.Point(12, 40);
         this.radioServer.Name = "radioServer";
         this.radioServer.Size = new System.Drawing.Size(152, 24);
         this.radioServer.TabIndex = 1;
         this.radioServer.Text = "G&room alerts on";
         this.radioServer.CheckedChanged += new System.EventHandler(this.radioServer_CheckedChanged);
         // 
         // radioAll
         // 
         this.radioAll.Checked = true;
         this.radioAll.Location = new System.Drawing.Point(12, 16);
         this.radioAll.Name = "radioAll";
         this.radioAll.Size = new System.Drawing.Size(324, 24);
         this.radioAll.TabIndex = 0;
         this.radioAll.TabStop = true;
         this.radioAll.Text = "&Groom alerts on all registered SQL Servers.";
         this.radioAll.CheckedChanged += new System.EventHandler(this.radioServer_CheckedChanged);
         // 
         // Form_GroomAlerts
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(422, 232);
         this.Controls.Add(this.groupBox3);
         this.Controls.Add(this.groupBox2);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_GroomAlerts";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Groom Alerts Now";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_Defaults_HelpRequested);
         this.groupBox1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
         this.groupBox2.ResumeLayout(false);
         this.groupBox2.PerformLayout();
         this.groupBox3.ResumeLayout(false);
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.PictureBox pictureBox1;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.GroupBox groupBox2;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.Label label8;
   }
}