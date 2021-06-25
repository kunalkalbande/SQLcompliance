namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_InstanceList
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
         this._lblDirections = new System.Windows.Forms.Label();
         this._listBoxServers = new System.Windows.Forms.ListBox();
         this._btnOk = new System.Windows.Forms.Button();
         this._btnCancel = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // _lblDirections
         // 
         this._lblDirections.Location = new System.Drawing.Point(8, 8);
         this._lblDirections.Name = "_lblDirections";
         this._lblDirections.Size = new System.Drawing.Size(240, 16);
         this._lblDirections.TabIndex = 0;
         this._lblDirections.Text = "&Select the SQL Servers monitored by this alert:";
         // 
         // _listBoxServers
         // 
         this._listBoxServers.Location = new System.Drawing.Point(8, 28);
         this._listBoxServers.Name = "_listBoxServers";
         this._listBoxServers.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
         this._listBoxServers.Size = new System.Drawing.Size(232, 160);
         this._listBoxServers.TabIndex = 1;
         // 
         // _btnOk
         // 
         this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
         this._btnOk.Location = new System.Drawing.Point(248, 28);
         this._btnOk.Name = "_btnOk";
         this._btnOk.Size = new System.Drawing.Size(75, 23);
         this._btnOk.TabIndex = 2;
         this._btnOk.Text = "&OK";
         // 
         // _btnCancel
         // 
         this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this._btnCancel.Location = new System.Drawing.Point(248, 56);
         this._btnCancel.Name = "_btnCancel";
         this._btnCancel.Size = new System.Drawing.Size(75, 23);
         this._btnCancel.TabIndex = 3;
         this._btnCancel.Text = "&Cancel";
         // 
         // Form_InstanceList
         // 
         this.AcceptButton = this._btnOk;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this._btnCancel;
         this.ClientSize = new System.Drawing.Size(330, 196);
         this.Controls.Add(this._btnCancel);
         this.Controls.Add(this._btnOk);
         this.Controls.Add(this._listBoxServers);
         this.Controls.Add(this._lblDirections);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_InstanceList";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Specify SQL Servers";
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Label _lblDirections;
      private System.Windows.Forms.ListBox _listBoxServers;
      private System.Windows.Forms.Button _btnOk;
      private System.Windows.Forms.Button _btnCancel;
	}
}