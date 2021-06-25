namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_AgentTraceDirectory
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_AgentTraceDirectory));
         this.groupBox7 = new System.Windows.Forms.GroupBox();
         this.textAgentTraceDirectory = new System.Windows.Forms.TextBox();
         this.label14 = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.groupBox7.SuspendLayout();
         this.SuspendLayout();
         // 
         // groupBox7
         // 
         this.groupBox7.Controls.Add(this.textAgentTraceDirectory);
         this.groupBox7.Controls.Add(this.label14);
         this.groupBox7.Location = new System.Drawing.Point(8, 68);
         this.groupBox7.Name = "groupBox7";
         this.groupBox7.Size = new System.Drawing.Size(488, 52);
         this.groupBox7.TabIndex = 91;
         this.groupBox7.TabStop = false;
         this.groupBox7.Text = "SQLcompliance Agent Trace Directory";
         // 
         // textAgentTraceDirectory
         // 
         this.textAgentTraceDirectory.Location = new System.Drawing.Point(96, 20);
         this.textAgentTraceDirectory.MaxLength = 255;
         this.textAgentTraceDirectory.Name = "textAgentTraceDirectory";
         this.textAgentTraceDirectory.Size = new System.Drawing.Size(384, 20);
         this.textAgentTraceDirectory.TabIndex = 92;
         this.textAgentTraceDirectory.TextChanged += new System.EventHandler(this.textAgentTraceDirectory_TextChanged);
         // 
         // label14
         // 
         this.label14.Location = new System.Drawing.Point(8, 24);
         this.label14.Name = "label14";
         this.label14.Size = new System.Drawing.Size(88, 16);
         this.label14.TabIndex = 91;
         this.label14.Text = "Trace directory:";
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(8, 8);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(500, 56);
         this.label1.TabIndex = 92;
         this.label1.Text = resources.GetString("label1.Text");
         // 
         // btnOK
         // 
         this.btnOK.Location = new System.Drawing.Point(340, 132);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 93;
         this.btnOK.Text = "&OK";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(420, 132);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 94;
         this.btnCancel.Text = "Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // Form_AgentTraceDirectory
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(506, 164);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.groupBox7);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_AgentTraceDirectory";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "SQLcompliance Agent Trace Directory";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_AgentTraceDirectory_HelpRequested);
         this.groupBox7.ResumeLayout(false);
         this.groupBox7.PerformLayout();
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.GroupBox groupBox7;
      private System.Windows.Forms.TextBox textAgentTraceDirectory;
      private System.Windows.Forms.Label label14;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;

	}
}