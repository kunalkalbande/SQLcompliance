namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_RepositoryOptions
   {

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
         this.components = new System.ComponentModel.Container();
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_RepositoryOptions));
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this._tabControl = new System.Windows.Forms.TabControl();
         this._tabRecoveryModel = new System.Windows.Forms.TabPage();
         this.radioModel = new System.Windows.Forms.RadioButton();
         this.radioSimple = new System.Windows.Forms.RadioButton();
         this.label4 = new System.Windows.Forms.Label();
         this._tabIndexes = new System.Windows.Forms.TabPage();
         this._btnUpdateSchedule = new System.Windows.Forms.Button();
         this._btnUpdateIndexes = new System.Windows.Forms.Button();
         this._listDatabases = new System.Windows.Forms.ListView();
         this._colDatabase = new System.Windows.Forms.ColumnHeader();
         this._colType = new System.Windows.Forms.ColumnHeader();
         this._colStatus = new System.Windows.Forms.ColumnHeader();
         this._imgList = new System.Windows.Forms.ImageList(this.components);
         this._tabControl.SuspendLayout();
         this._tabRecoveryModel.SuspendLayout();
         this._tabIndexes.SuspendLayout();
         this.SuspendLayout();
         // 
         // btnOK
         // 
         this.btnOK.Location = new System.Drawing.Point(344, 268);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 11;
         this.btnOK.Text = "&OK";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(428, 268);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 12;
         this.btnCancel.Text = "&Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // _tabControl
         // 
         this._tabControl.Controls.Add(this._tabRecoveryModel);
         this._tabControl.Controls.Add(this._tabIndexes);
         this._tabControl.Location = new System.Drawing.Point(0, 0);
         this._tabControl.Name = "_tabControl";
         this._tabControl.SelectedIndex = 0;
         this._tabControl.Size = new System.Drawing.Size(512, 256);
         this._tabControl.TabIndex = 18;
         // 
         // _tabRecoveryModel
         // 
         this._tabRecoveryModel.Controls.Add(this.radioModel);
         this._tabRecoveryModel.Controls.Add(this.radioSimple);
         this._tabRecoveryModel.Controls.Add(this.label4);
         this._tabRecoveryModel.Location = new System.Drawing.Point(4, 22);
         this._tabRecoveryModel.Name = "_tabRecoveryModel";
         this._tabRecoveryModel.Size = new System.Drawing.Size(504, 230);
         this._tabRecoveryModel.TabIndex = 0;
         this._tabRecoveryModel.Text = "Recovery Model";
         this._tabRecoveryModel.UseVisualStyleBackColor = true;
         // 
         // radioModel
         // 
         this.radioModel.Location = new System.Drawing.Point(16, 100);
         this.radioModel.Name = "radioModel";
         this.radioModel.Size = new System.Drawing.Size(468, 24);
         this.radioModel.TabIndex = 20;
         this.radioModel.Text = "Create new databases with default recovery model (based on model database setting" +
             "s)";
         // 
         // radioSimple
         // 
         this.radioSimple.Checked = true;
         this.radioSimple.Location = new System.Drawing.Point(16, 76);
         this.radioSimple.Name = "radioSimple";
         this.radioSimple.Size = new System.Drawing.Size(328, 24);
         this.radioSimple.TabIndex = 19;
         this.radioSimple.TabStop = true;
         this.radioSimple.Text = "Create new databases using the simple recovery model";
         // 
         // label4
         // 
         this.label4.Location = new System.Drawing.Point(4, 4);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(468, 68);
         this.label4.TabIndex = 18;
         this.label4.Text = resources.GetString("label4.Text");
         // 
         // _tabIndexes
         // 
         this._tabIndexes.Controls.Add(this._btnUpdateSchedule);
         this._tabIndexes.Controls.Add(this._btnUpdateIndexes);
         this._tabIndexes.Controls.Add(this._listDatabases);
         this._tabIndexes.Location = new System.Drawing.Point(4, 22);
         this._tabIndexes.Name = "_tabIndexes";
         this._tabIndexes.Size = new System.Drawing.Size(504, 230);
         this._tabIndexes.TabIndex = 1;
         this._tabIndexes.Text = "Databases";
         this._tabIndexes.UseVisualStyleBackColor = true;
         // 
         // _btnUpdateSchedule
         // 
         this._btnUpdateSchedule.Location = new System.Drawing.Point(286, 196);
         this._btnUpdateSchedule.Name = "_btnUpdateSchedule";
         this._btnUpdateSchedule.Size = new System.Drawing.Size(100, 23);
         this._btnUpdateSchedule.TabIndex = 2;
         this._btnUpdateSchedule.Text = "Edit Schedule";
         this._btnUpdateSchedule.UseVisualStyleBackColor = true;
         this._btnUpdateSchedule.Click += new System.EventHandler(this.Click_btnUpdateSchedule);
         // 
         // _btnUpdateIndexes
         // 
         this._btnUpdateIndexes.Enabled = false;
         this._btnUpdateIndexes.Location = new System.Drawing.Point(392, 196);
         this._btnUpdateIndexes.Name = "_btnUpdateIndexes";
         this._btnUpdateIndexes.Size = new System.Drawing.Size(100, 23);
         this._btnUpdateIndexes.TabIndex = 1;
         this._btnUpdateIndexes.Text = "Update Now";
         this._btnUpdateIndexes.Click += new System.EventHandler(this.Click_btnUpdateIndexes);
         // 
         // _listDatabases
         // 
         this._listDatabases.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._colDatabase,
            this._colType,
            this._colStatus});
         this._listDatabases.FullRowSelect = true;
         this._listDatabases.Location = new System.Drawing.Point(0, 0);
         this._listDatabases.Name = "_listDatabases";
         this._listDatabases.Size = new System.Drawing.Size(504, 184);
         this._listDatabases.SmallImageList = this._imgList;
         this._listDatabases.Sorting = System.Windows.Forms.SortOrder.Ascending;
         this._listDatabases.TabIndex = 0;
         this._listDatabases.UseCompatibleStateImageBehavior = false;
         this._listDatabases.View = System.Windows.Forms.View.Details;
         this._listDatabases.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged_listDatabases);
         // 
         // _colDatabase
         // 
         this._colDatabase.Text = "Database Name";
         this._colDatabase.Width = 300;
         // 
         // _colType
         // 
         this._colType.Text = "Type";
         // 
         // _colStatus
         // 
         this._colStatus.Text = "Status";
         this._colStatus.Width = 120;
         // 
         // _imgList
         // 
         this._imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_imgList.ImageStream")));
         this._imgList.TransparentColor = System.Drawing.Color.Transparent;
         this._imgList.Images.SetKeyName(0, "");
         this._imgList.Images.SetKeyName(1, "");
         this._imgList.Images.SetKeyName(2, "");
         // 
         // Form_RepositoryOptions
         // 
         this.AcceptButton = this.btnOK;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(510, 300);
         this.Controls.Add(this._tabControl);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnOK);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_RepositoryOptions";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Configure Repository Databases";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_Defaults_HelpRequested);
         this._tabControl.ResumeLayout(false);
         this._tabRecoveryModel.ResumeLayout(false);
         this._tabIndexes.ResumeLayout(false);
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.TabControl _tabControl;
      private System.Windows.Forms.TabPage _tabRecoveryModel;
      private System.Windows.Forms.TabPage _tabIndexes;
      private System.Windows.Forms.RadioButton radioModel;
      private System.Windows.Forms.RadioButton radioSimple;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.ListView _listDatabases;
      private System.Windows.Forms.ColumnHeader _colDatabase;
      private System.Windows.Forms.ColumnHeader _colType;
      private System.Windows.Forms.ColumnHeader _colStatus;
      private System.Windows.Forms.ImageList _imgList;
      private System.Windows.Forms.Button _btnUpdateIndexes;
      private System.ComponentModel.IContainer components;
      private System.Windows.Forms.Button _btnUpdateSchedule;

	}
}