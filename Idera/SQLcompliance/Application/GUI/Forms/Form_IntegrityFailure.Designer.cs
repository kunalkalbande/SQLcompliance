namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_IntegrityFailure
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
         Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("IntegrityEvents", -1);
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Problem");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Category");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Event");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Time");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Login");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Database");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TargetObject");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Details");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Problem");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Category");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Event");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Time");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Login");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Database");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn7 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("TargetObject");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn8 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Details");
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_IntegrityFailure));
         this.btnDontRepair = new System.Windows.Forms.Button();
         this.btnRepair = new System.Windows.Forms.Button();
         this.grpProblems = new System.Windows.Forms.GroupBox();
         this._gridEvents = new Infragistics.Win.UltraWinGrid.UltraGrid();
         this._dsEvents = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
         this.groupBox2 = new System.Windows.Forms.GroupBox();
         this.lblModifiedSensitiveColumns = new System.Windows.Forms.Label();
         this.lblModifiedColumnChanges = new System.Windows.Forms.Label();
         this.lblModifiedDataChanges = new System.Windows.Forms.Label();
         this.lblModifiedEvents = new System.Windows.Forms.Label();
         this.label7 = new System.Windows.Forms.Label();
         this.label8 = new System.Windows.Forms.Label();
         this.label9 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.groupOperationCantContinue = new System.Windows.Forms.GroupBox();
         this.labelOperation = new System.Windows.Forms.Label();
         this.pictureBox1 = new System.Windows.Forms.PictureBox();
         this.linkMoreHelp = new System.Windows.Forms.LinkLabel();
         this.groupDeletions = new System.Windows.Forms.GroupBox();
         this.lblSensitiveColumnGaps = new System.Windows.Forms.Label();
         this.lblColumnChangeGaps = new System.Windows.Forms.Label();
         this.lblDataChangeGaps = new System.Windows.Forms.Label();
         this.lblEventGaps = new System.Windows.Forms.Label();
         this.label3 = new System.Windows.Forms.Label();
         this.label5 = new System.Windows.Forms.Label();
         this.label4 = new System.Windows.Forms.Label();
         this.groupBox3 = new System.Windows.Forms.GroupBox();
         this.lblAddedEvents = new System.Windows.Forms.Label();
         this.label13 = new System.Windows.Forms.Label();
         this.grpProblems.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._gridEvents)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._dsEvents)).BeginInit();
         this.groupBox2.SuspendLayout();
         this.groupOperationCantContinue.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
         this.groupDeletions.SuspendLayout();
         this.groupBox3.SuspendLayout();
         this.SuspendLayout();
         // 
         // btnDontRepair
         // 
         this.btnDontRepair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnDontRepair.DialogResult = System.Windows.Forms.DialogResult.No;
         this.btnDontRepair.Location = new System.Drawing.Point(524, 403);
         this.btnDontRepair.Name = "btnDontRepair";
         this.btnDontRepair.Size = new System.Drawing.Size(116, 23);
         this.btnDontRepair.TabIndex = 0;
         this.btnDontRepair.Text = "&Don\'t Mark Events";
         this.btnDontRepair.Click += new System.EventHandler(this.btnDontRepair_Click);
         // 
         // btnRepair
         // 
         this.btnRepair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnRepair.DialogResult = System.Windows.Forms.DialogResult.Yes;
         this.btnRepair.Location = new System.Drawing.Point(400, 403);
         this.btnRepair.Name = "btnRepair";
         this.btnRepair.Size = new System.Drawing.Size(116, 23);
         this.btnRepair.TabIndex = 1;
         this.btnRepair.Text = "&Mark Events";
         this.btnRepair.Click += new System.EventHandler(this.btnRepair_Click);
         // 
         // grpProblems
         // 
         this.grpProblems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.grpProblems.Controls.Add(this._gridEvents);
         this.grpProblems.Location = new System.Drawing.Point(8, 204);
         this.grpProblems.Name = "grpProblems";
         this.grpProblems.Size = new System.Drawing.Size(636, 184);
         this.grpProblems.TabIndex = 2;
         this.grpProblems.TabStop = false;
         this.grpProblems.Text = "Details (only first 100 compromised events shown)";
         // 
         // _gridEvents
         // 
         this._gridEvents.DataMember = "IntegrityEvents";
         this._gridEvents.DataSource = this._dsEvents;
         ultraGridColumn1.Header.VisiblePosition = 0;
         ultraGridColumn2.Header.VisiblePosition = 1;
         ultraGridColumn3.Header.VisiblePosition = 2;
         ultraGridColumn4.Format = "G";
         ultraGridColumn4.Header.VisiblePosition = 3;
         ultraGridColumn5.Header.VisiblePosition = 4;
         ultraGridColumn6.Header.VisiblePosition = 5;
         ultraGridColumn7.Header.Caption = "Target Object";
         ultraGridColumn7.Header.VisiblePosition = 6;
         ultraGridColumn8.Header.VisiblePosition = 7;
         ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8});
         this._gridEvents.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
         this._gridEvents.Dock = System.Windows.Forms.DockStyle.Fill;
         this._gridEvents.Location = new System.Drawing.Point(3, 16);
         this._gridEvents.Name = "_gridEvents";
         this._gridEvents.Size = new System.Drawing.Size(630, 165);
         this._gridEvents.TabIndex = 0;
         this._gridEvents.Text = "ultraGrid1";
         // 
         // _dsEvents
         // 
         this._dsEvents.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5,
            ultraDataColumn6,
            ultraDataColumn7,
            ultraDataColumn8});
         this._dsEvents.Band.Key = "IntegrityEvents";
         // 
         // groupBox2
         // 
         this.groupBox2.Controls.Add(this.lblModifiedSensitiveColumns);
         this.groupBox2.Controls.Add(this.lblModifiedColumnChanges);
         this.groupBox2.Controls.Add(this.lblModifiedDataChanges);
         this.groupBox2.Controls.Add(this.lblModifiedEvents);
         this.groupBox2.Controls.Add(this.label7);
         this.groupBox2.Controls.Add(this.label8);
         this.groupBox2.Controls.Add(this.label9);
         this.groupBox2.Controls.Add(this.label2);
         this.groupBox2.Location = new System.Drawing.Point(8, 58);
         this.groupBox2.Name = "groupBox2";
         this.groupBox2.Size = new System.Drawing.Size(303, 95);
         this.groupBox2.TabIndex = 3;
         this.groupBox2.TabStop = false;
         this.groupBox2.Text = "Modifications";
         // 
         // lblModifiedSensitiveColumns
         // 
         this.lblModifiedSensitiveColumns.AutoSize = true;
         this.lblModifiedSensitiveColumns.Location = new System.Drawing.Point(129, 73);
         this.lblModifiedSensitiveColumns.Name = "lblModifiedSensitiveColumns";
         this.lblModifiedSensitiveColumns.Size = new System.Drawing.Size(13, 13);
         this.lblModifiedSensitiveColumns.TabIndex = 12;
         this.lblModifiedSensitiveColumns.Text = "0";
         // 
         // lblModifiedColumnChanges
         // 
         this.lblModifiedColumnChanges.AutoSize = true;
         this.lblModifiedColumnChanges.Location = new System.Drawing.Point(129, 55);
         this.lblModifiedColumnChanges.Name = "lblModifiedColumnChanges";
         this.lblModifiedColumnChanges.Size = new System.Drawing.Size(13, 13);
         this.lblModifiedColumnChanges.TabIndex = 11;
         this.lblModifiedColumnChanges.Text = "0";
         // 
         // lblModifiedDataChanges
         // 
         this.lblModifiedDataChanges.AutoSize = true;
         this.lblModifiedDataChanges.Location = new System.Drawing.Point(129, 37);
         this.lblModifiedDataChanges.Name = "lblModifiedDataChanges";
         this.lblModifiedDataChanges.Size = new System.Drawing.Size(13, 13);
         this.lblModifiedDataChanges.TabIndex = 10;
         this.lblModifiedDataChanges.Text = "0";
         // 
         // lblModifiedEvents
         // 
         this.lblModifiedEvents.AutoSize = true;
         this.lblModifiedEvents.Location = new System.Drawing.Point(129, 19);
         this.lblModifiedEvents.Name = "lblModifiedEvents";
         this.lblModifiedEvents.Size = new System.Drawing.Size(13, 13);
         this.lblModifiedEvents.TabIndex = 9;
         this.lblModifiedEvents.Text = "0";
         // 
         // label7
         // 
         this.label7.AutoSize = true;
         this.label7.Location = new System.Drawing.Point(12, 72);
         this.label7.Name = "label7";
         this.label7.Size = new System.Drawing.Size(115, 13);
         this.label7.TabIndex = 8;
         this.label7.Text = "Sensitive Column data:";
         // 
         // label8
         // 
         this.label8.AutoSize = true;
         this.label8.Location = new System.Drawing.Point(19, 54);
         this.label8.Name = "label8";
         this.label8.Size = new System.Drawing.Size(108, 13);
         this.label8.TabIndex = 7;
         this.label8.Text = "Before-After columns:";
         // 
         // label9
         // 
         this.label9.AutoSize = true;
         this.label9.Location = new System.Drawing.Point(37, 36);
         this.label9.Name = "label9";
         this.label9.Size = new System.Drawing.Size(90, 13);
         this.label9.TabIndex = 6;
         this.label9.Text = "Before-After data:";
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(84, 18);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(43, 13);
         this.label2.TabIndex = 1;
         this.label2.Text = "Events:";
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(84, 18);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(43, 13);
         this.label1.TabIndex = 0;
         this.label1.Text = "Events:";
         // 
         // groupOperationCantContinue
         // 
         this.groupOperationCantContinue.Controls.Add(this.labelOperation);
         this.groupOperationCantContinue.Controls.Add(this.pictureBox1);
         this.groupOperationCantContinue.Location = new System.Drawing.Point(309, 58);
         this.groupOperationCantContinue.Name = "groupOperationCantContinue";
         this.groupOperationCantContinue.Size = new System.Drawing.Size(333, 92);
         this.groupOperationCantContinue.TabIndex = 5;
         this.groupOperationCantContinue.TabStop = false;
         // 
         // labelOperation
         // 
         this.labelOperation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.labelOperation.Location = new System.Drawing.Point(72, 28);
         this.labelOperation.Name = "labelOperation";
         this.labelOperation.Size = new System.Drawing.Size(248, 52);
         this.labelOperation.TabIndex = 1;
         this.labelOperation.Text = " of the audit data for this SQL Server cannot be performed until the compromised " +
             "events in its event database are marked.";
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
         // linkMoreHelp
         // 
         this.linkMoreHelp.LinkArea = new System.Windows.Forms.LinkArea(291, 303);
         this.linkMoreHelp.Location = new System.Drawing.Point(8, 8);
         this.linkMoreHelp.Name = "linkMoreHelp";
         this.linkMoreHelp.Size = new System.Drawing.Size(624, 48);
         this.linkMoreHelp.TabIndex = 6;
         this.linkMoreHelp.TabStop = true;
         this.linkMoreHelp.Text = resources.GetString("linkMoreHelp.Text");
         this.linkMoreHelp.UseCompatibleTextRendering = true;
         this.linkMoreHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkMoreHelp_LinkClicked);
         // 
         // groupDeletions
         // 
         this.groupDeletions.Controls.Add(this.lblSensitiveColumnGaps);
         this.groupDeletions.Controls.Add(this.lblColumnChangeGaps);
         this.groupDeletions.Controls.Add(this.lblDataChangeGaps);
         this.groupDeletions.Controls.Add(this.lblEventGaps);
         this.groupDeletions.Controls.Add(this.label3);
         this.groupDeletions.Controls.Add(this.label5);
         this.groupDeletions.Controls.Add(this.label4);
         this.groupDeletions.Controls.Add(this.label1);
         this.groupDeletions.Location = new System.Drawing.Point(319, 58);
         this.groupDeletions.Name = "groupDeletions";
         this.groupDeletions.Size = new System.Drawing.Size(313, 95);
         this.groupDeletions.TabIndex = 7;
         this.groupDeletions.TabStop = false;
         this.groupDeletions.Text = "Deletions";
         // 
         // lblSensitiveColumnGaps
         // 
         this.lblSensitiveColumnGaps.AutoSize = true;
         this.lblSensitiveColumnGaps.Location = new System.Drawing.Point(129, 73);
         this.lblSensitiveColumnGaps.Name = "lblSensitiveColumnGaps";
         this.lblSensitiveColumnGaps.Size = new System.Drawing.Size(13, 13);
         this.lblSensitiveColumnGaps.TabIndex = 16;
         this.lblSensitiveColumnGaps.Text = "0";
         // 
         // lblColumnChangeGaps
         // 
         this.lblColumnChangeGaps.AutoSize = true;
         this.lblColumnChangeGaps.Location = new System.Drawing.Point(129, 55);
         this.lblColumnChangeGaps.Name = "lblColumnChangeGaps";
         this.lblColumnChangeGaps.Size = new System.Drawing.Size(13, 13);
         this.lblColumnChangeGaps.TabIndex = 15;
         this.lblColumnChangeGaps.Text = "0";
         // 
         // lblDataChangeGaps
         // 
         this.lblDataChangeGaps.AutoSize = true;
         this.lblDataChangeGaps.Location = new System.Drawing.Point(129, 37);
         this.lblDataChangeGaps.Name = "lblDataChangeGaps";
         this.lblDataChangeGaps.Size = new System.Drawing.Size(13, 13);
         this.lblDataChangeGaps.TabIndex = 14;
         this.lblDataChangeGaps.Text = "0";
         // 
         // lblEventGaps
         // 
         this.lblEventGaps.AutoSize = true;
         this.lblEventGaps.Location = new System.Drawing.Point(129, 19);
         this.lblEventGaps.Name = "lblEventGaps";
         this.lblEventGaps.Size = new System.Drawing.Size(13, 13);
         this.lblEventGaps.TabIndex = 13;
         this.lblEventGaps.Text = "0";
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(12, 72);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(115, 13);
         this.label3.TabIndex = 2;
         this.label3.Text = "Sensitive Column data:";
         // 
         // label5
         // 
         this.label5.AutoSize = true;
         this.label5.Location = new System.Drawing.Point(19, 54);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(108, 13);
         this.label5.TabIndex = 1;
         this.label5.Text = "Before-After columns:";
         // 
         // label4
         // 
         this.label4.AutoSize = true;
         this.label4.Location = new System.Drawing.Point(37, 36);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(90, 13);
         this.label4.TabIndex = 0;
         this.label4.Text = "Before-After data:";
         // 
         // groupBox3
         // 
         this.groupBox3.Controls.Add(this.lblAddedEvents);
         this.groupBox3.Controls.Add(this.label13);
         this.groupBox3.Location = new System.Drawing.Point(12, 157);
         this.groupBox3.Name = "groupBox3";
         this.groupBox3.Size = new System.Drawing.Size(620, 39);
         this.groupBox3.TabIndex = 8;
         this.groupBox3.TabStop = false;
         this.groupBox3.Text = "Inserts";
         // 
         // lblAddedEvents
         // 
         this.lblAddedEvents.AutoSize = true;
         this.lblAddedEvents.Location = new System.Drawing.Point(74, 16);
         this.lblAddedEvents.Name = "lblAddedEvents";
         this.lblAddedEvents.Size = new System.Drawing.Size(13, 13);
         this.lblAddedEvents.TabIndex = 6;
         this.lblAddedEvents.Text = "0";
         // 
         // label13
         // 
         this.label13.AutoSize = true;
         this.label13.Location = new System.Drawing.Point(6, 16);
         this.label13.Name = "label13";
         this.label13.Size = new System.Drawing.Size(43, 13);
         this.label13.TabIndex = 2;
         this.label13.Text = "Events:";
         // 
         // Form_IntegrityFailure
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(652, 433);
         this.Controls.Add(this.groupDeletions);
         this.Controls.Add(this.groupBox3);
         this.Controls.Add(this.linkMoreHelp);
         this.Controls.Add(this.groupOperationCantContinue);
         this.Controls.Add(this.groupBox2);
         this.Controls.Add(this.grpProblems);
         this.Controls.Add(this.btnRepair);
         this.Controls.Add(this.btnDontRepair);
         this.HelpButton = true;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.MinimumSize = new System.Drawing.Size(460, 300);
         this.Name = "Form_IntegrityFailure";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Integrity Check Results ";
         this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_Defaults_HelpRequested);
         this.grpProblems.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this._gridEvents)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._dsEvents)).EndInit();
         this.groupBox2.ResumeLayout(false);
         this.groupBox2.PerformLayout();
         this.groupOperationCantContinue.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
         this.groupDeletions.ResumeLayout(false);
         this.groupDeletions.PerformLayout();
         this.groupBox3.ResumeLayout(false);
         this.groupBox3.PerformLayout();
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.GroupBox groupBox2;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Button btnDontRepair;
      private System.Windows.Forms.Button btnRepair;
      private System.Windows.Forms.GroupBox grpProblems;
      private System.Windows.Forms.PictureBox pictureBox1;
      private System.Windows.Forms.GroupBox groupOperationCantContinue;
      private System.Windows.Forms.Label labelOperation;
      private System.Windows.Forms.LinkLabel linkMoreHelp;
      private Infragistics.Win.UltraWinGrid.UltraGrid _gridEvents;
      private Infragistics.Win.UltraWinDataSource.UltraDataSource _dsEvents;
      private System.ComponentModel.IContainer components;
      private System.Windows.Forms.GroupBox groupDeletions;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.Label label8;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.GroupBox groupBox3;
      private System.Windows.Forms.Label label13;
      private System.Windows.Forms.Label lblAddedEvents;
      private System.Windows.Forms.Label lblModifiedSensitiveColumns;
      private System.Windows.Forms.Label lblModifiedColumnChanges;
      private System.Windows.Forms.Label lblModifiedDataChanges;
      private System.Windows.Forms.Label lblModifiedEvents;
      private System.Windows.Forms.Label lblSensitiveColumnGaps;
      private System.Windows.Forms.Label lblColumnChangeGaps;
      private System.Windows.Forms.Label lblDataChangeGaps;
      private System.Windows.Forms.Label lblEventGaps;
   }
}