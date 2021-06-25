namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_EventProperties
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
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.Label lblTime;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.Label label9;
            System.Windows.Forms.Label label10;
		 Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
         Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand4 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Property");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Value");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn9 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Property");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn10 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Value");
         Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand5 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Row #");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Primary Key");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Column");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Before Value");
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("After Value");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn11 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Row #");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn12 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Primary Key");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn13 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Column");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn14 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Before Value");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn15 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("After Value");
         Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
         Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand6 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Columns", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
         Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Row Count", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
         Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
         Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn16 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Columns");
         Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn17 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Row Count");
         this.panelBottom = new System.Windows.Forms.Panel();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.groupBox2 = new System.Windows.Forms.GroupBox();
         this.txtSQL = new System.Windows.Forms.RichTextBox();
         this.btnClose = new System.Windows.Forms.Button();
         this.btnCopy = new System.Windows.Forms.Button();
         this.panelTop = new System.Windows.Forms.Panel();
         this._groupBeforeAfter = new System.Windows.Forms.GroupBox();
         this.txtRowsAffected = new System.Windows.Forms.TextBox();
         this.txtColumnsAffected = new System.Windows.Forms.TextBox();
         this.grpError = new System.Windows.Forms.GroupBox();
         this.txtApplication = new System.Windows.Forms.TextBox();
         this.txtDetails = new System.Windows.Forms.TextBox();
         this.txtCategory = new System.Windows.Forms.TextBox();
         this.txtType = new System.Windows.Forms.TextBox();
         this.txtUser = new System.Windows.Forms.TextBox();
         this.txtDatabase = new System.Windows.Forms.TextBox();
         this.txtObject = new System.Windows.Forms.TextBox();
         this.txtTime = new System.Windows.Forms.TextBox();
         this.txtRowCounts = new System.Windows.Forms.TextBox(); 
         this.lblError = new System.Windows.Forms.Label();
         this.btnNext = new System.Windows.Forms.Button();
         this.btnPrevious = new System.Windows.Forms.Button();
         this._tabControl = new System.Windows.Forms.TabControl();
         this._tabGeneral = new System.Windows.Forms.TabPage();
         this._tabDetails = new System.Windows.Forms.TabPage();
         this._gridDetails = new Infragistics.Win.UltraWinGrid.UltraGrid();
         this._dsDetails = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
         this._tabBeforeAfter = new System.Windows.Forms.TabPage();
         this._gridBeforeAfter = new Infragistics.Win.UltraWinGrid.UltraGrid();
         this._dsBeforeAfter = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
         this._lblBeforeAfterNotAvailable = new System.Windows.Forms.Label();
         this._tabSensitiveColumns = new System.Windows.Forms.TabPage();
         this._gridSensitiveColumns = new Infragistics.Win.UltraWinGrid.UltraGrid();
         this._dsSensitiveColumns = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
         this.pnlError = new System.Windows.Forms.Panel();
         label2 = new System.Windows.Forms.Label();
         label4 = new System.Windows.Forms.Label();
         label7 = new System.Windows.Forms.Label();
         lblTime = new System.Windows.Forms.Label();
         label3 = new System.Windows.Forms.Label();
         label5 = new System.Windows.Forms.Label();
         label8 = new System.Windows.Forms.Label();
         label1 = new System.Windows.Forms.Label();
         label6 = new System.Windows.Forms.Label();
         label9 = new System.Windows.Forms.Label();
         label10 = new System.Windows.Forms.Label();
         this.panelBottom.SuspendLayout();
         this.groupBox1.SuspendLayout();
         this.panelTop.SuspendLayout();
         this._groupBeforeAfter.SuspendLayout();
         this.grpError.SuspendLayout();
         this._tabControl.SuspendLayout();
         this._tabGeneral.SuspendLayout();
         this._tabDetails.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._gridDetails)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._dsDetails)).BeginInit();
         this._tabBeforeAfter.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._gridBeforeAfter)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._dsBeforeAfter)).BeginInit();
         this._tabSensitiveColumns.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._gridSensitiveColumns)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._dsSensitiveColumns)).BeginInit();
            this.pnlError.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            label2.Location = new System.Drawing.Point(8, 92);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(64, 16);
            label2.TabIndex = 47;
            label2.Tag = "1";
            label2.Text = "Application:";
            // 
            // label4
            // 
            label4.Location = new System.Drawing.Point(8, 68);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(40, 16);
            label4.TabIndex = 45;
            label4.Tag = "1";
            label4.Text = "Type:";
            // 
            // label7
            // 
            label7.Location = new System.Drawing.Point(8, 44);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(56, 16);
            label7.TabIndex = 44;
            label7.Tag = "1";
            label7.Text = "Category:";
            // 
            // lblTime
            // 
            lblTime.Location = new System.Drawing.Point(8, 20);
            lblTime.Name = "lblTime";
            lblTime.Size = new System.Drawing.Size(44, 16);
            lblTime.TabIndex = 43;
            lblTime.Tag = "1";
            lblTime.Text = "Time:";
            // 
            // label3
            // 
            label3.Location = new System.Drawing.Point(264, 20);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(36, 16);
            label3.TabIndex = 38;
            label3.Tag = "1";
            label3.Text = "Login:";
            // 
            // label5
            // 
            label5.Location = new System.Drawing.Point(264, 44);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(56, 16);
            label5.TabIndex = 39;
            label5.Tag = "1";
            label5.Text = "Database:";
            // 
            // label8
            // 
            label8.Location = new System.Drawing.Point(264, 68);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(52, 16);
            label8.TabIndex = 40;
            label8.Tag = "1";
            label8.Text = "Target:";
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(264, 92);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(52, 16);
            label1.TabIndex = 41;
            label1.Tag = "1";
            label1.Text = "Details:";
            // 
            // label6
            // 
            label6.Location = new System.Drawing.Point(8, 49);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(106, 16);
            label6.TabIndex = 46;
            label6.Tag = "1";
            label6.Text = "Columns Affected:";
            // 
            // label9
            // 
            label9.Location = new System.Drawing.Point(8, 23);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(106, 16);
            label9.TabIndex = 48;
            label9.Tag = "1";
            label9.Text = "Rows Affected:";
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.groupBox2);
            this.panelBottom.Controls.Add(this.groupBox1);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBottom.Location = new System.Drawing.Point(3, 221);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(522, 171);
            this.panelBottom.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtSQL);
            this.groupBox1.Location = new System.Drawing.Point(9, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(504, 114);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SQL Statement";
            // 
            // txtSQL
            // 
            this.txtSQL.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSQL.Location = new System.Drawing.Point(12, 16);
            this.txtSQL.Name = "txtSQL";
            this.txtSQL.ReadOnly = true;
            this.txtSQL.Size = new System.Drawing.Size(481, 90);
            this.txtSQL.TabIndex = 0;
            this.txtSQL.TabStop = false;
            this.txtSQL.Text = "";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(548, 378);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(56, 24);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "&Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.copy;
            this.btnCopy.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCopy.Location = new System.Drawing.Point(548, 303);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(56, 24);
            this.btnCopy.TabIndex = 0;
            this.btnCopy.Text = "Co&py";
            this.btnCopy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this._groupBeforeAfter);
            this.panelTop.Controls.Add(this.grpError);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(3, 3);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(522, 218);
            this.panelTop.TabIndex = 0;
            // 
            // _groupBeforeAfter
            // 
            this._groupBeforeAfter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._groupBeforeAfter.Controls.Add(label9);
            this._groupBeforeAfter.Controls.Add(this.txtRowsAffected);
            this._groupBeforeAfter.Controls.Add(label6);
            this._groupBeforeAfter.Controls.Add(this.txtColumnsAffected);
            this._groupBeforeAfter.Location = new System.Drawing.Point(9, 133);
            this._groupBeforeAfter.Name = "_groupBeforeAfter";
            this._groupBeforeAfter.Size = new System.Drawing.Size(504, 79);
            this._groupBeforeAfter.TabIndex = 1;
            this._groupBeforeAfter.TabStop = false;
            this._groupBeforeAfter.Text = "Before-After Data Summary";
            // 
            // txtRowsAffected
            // 
            this.txtRowsAffected.Location = new System.Drawing.Point(120, 19);
            this.txtRowsAffected.Name = "txtRowsAffected";
            this.txtRowsAffected.ReadOnly = true;
            this.txtRowsAffected.Size = new System.Drawing.Size(132, 20);
            this.txtRowsAffected.TabIndex = 47;
            this.txtRowsAffected.TabStop = false;
            this.txtRowsAffected.Tag = "1";
            // 
            // txtColumnsAffected
            // 
            this.txtColumnsAffected.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtColumnsAffected.Location = new System.Drawing.Point(120, 45);
            this.txtColumnsAffected.Name = "txtColumnsAffected";
            this.txtColumnsAffected.ReadOnly = true;
            this.txtColumnsAffected.Size = new System.Drawing.Size(338, 20);
            this.txtColumnsAffected.TabIndex = 45;
            this.txtColumnsAffected.TabStop = false;
            this.txtColumnsAffected.Tag = "1";
            // 
            // grpError
            // 
            this.grpError.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpError.Controls.Add(label2);
            this.grpError.Controls.Add(this.txtApplication);
            this.grpError.Controls.Add(label4);
            this.grpError.Controls.Add(label7);
            this.grpError.Controls.Add(lblTime);
            this.grpError.Controls.Add(this.txtDetails);
            this.grpError.Controls.Add(label3);
            this.grpError.Controls.Add(label5);
            this.grpError.Controls.Add(label8);
            this.grpError.Controls.Add(this.txtCategory);
            this.grpError.Controls.Add(this.txtType);
            this.grpError.Controls.Add(this.txtUser);
            this.grpError.Controls.Add(this.txtDatabase);
            this.grpError.Controls.Add(this.txtObject);
            this.grpError.Controls.Add(this.txtTime);
            this.grpError.Controls.Add(label1);
            this.grpError.Location = new System.Drawing.Point(9, 8);
            this.grpError.Name = "grpError";
            this.grpError.Size = new System.Drawing.Size(504, 119);
            this.grpError.TabIndex = 0;
            this.grpError.TabStop = false;
            this.grpError.Text = "Event";
            // 
            // txtApplication
            // 
            this.txtApplication.Location = new System.Drawing.Point(76, 88);
            this.txtApplication.Name = "txtApplication";
            this.txtApplication.ReadOnly = true;
            this.txtApplication.Size = new System.Drawing.Size(176, 20);
            this.txtApplication.TabIndex = 46;
            this.txtApplication.TabStop = false;
            this.txtApplication.Tag = "1";
            // 
            // txtDetails
            // 
            this.txtDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDetails.Location = new System.Drawing.Point(320, 88);
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.ReadOnly = true;
            this.txtDetails.Size = new System.Drawing.Size(138, 20);
            this.txtDetails.TabIndex = 7;
            this.txtDetails.TabStop = false;
            this.txtDetails.Tag = "1";
            // 
            // txtCategory
            // 
            this.txtCategory.Location = new System.Drawing.Point(76, 40);
            this.txtCategory.Name = "txtCategory";
            this.txtCategory.ReadOnly = true;
            this.txtCategory.Size = new System.Drawing.Size(176, 20);
            this.txtCategory.TabIndex = 2;
            this.txtCategory.TabStop = false;
            this.txtCategory.Tag = "1";
            // 
            // txtType
            // 
            this.txtType.Location = new System.Drawing.Point(76, 64);
            this.txtType.Name = "txtType";
            this.txtType.ReadOnly = true;
            this.txtType.Size = new System.Drawing.Size(176, 20);
            this.txtType.TabIndex = 3;
            this.txtType.TabStop = false;
            this.txtType.Tag = "1";
            // 
            // txtUser
            // 
            this.txtUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUser.Location = new System.Drawing.Point(320, 16);
            this.txtUser.Name = "txtUser";
            this.txtUser.ReadOnly = true;
            this.txtUser.Size = new System.Drawing.Size(138, 20);
            this.txtUser.TabIndex = 4;
            this.txtUser.TabStop = false;
            this.txtUser.Tag = "1";
            // 
            // txtDatabase
            // 
            this.txtDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDatabase.Location = new System.Drawing.Point(320, 40);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.ReadOnly = true;
            this.txtDatabase.Size = new System.Drawing.Size(138, 20);
            this.txtDatabase.TabIndex = 5;
            this.txtDatabase.TabStop = false;
            this.txtDatabase.Tag = "1";
            // 
            // txtObject
            // 
            this.txtObject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtObject.Location = new System.Drawing.Point(320, 64);
            this.txtObject.Name = "txtObject";
            this.txtObject.ReadOnly = true;
            this.txtObject.Size = new System.Drawing.Size(138, 20);
            this.txtObject.TabIndex = 6;
            this.txtObject.TabStop = false;
            this.txtObject.Tag = "1";
            // 
            // txtTime
            // 
            this.txtTime.Location = new System.Drawing.Point(76, 16);
            this.txtTime.Name = "txtTime";
            this.txtTime.ReadOnly = true;
            this.txtTime.Size = new System.Drawing.Size(176, 20);
            this.txtTime.TabIndex = 1;
            this.txtTime.TabStop = false;
            this.txtTime.Tag = "1";
            // 
            // lblError
            // 
            this.lblError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblError.Location = new System.Drawing.Point(0, 0);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(542, 419);
            this.lblError.TabIndex = 42;
            this.lblError.Text = "Event";
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.DownArrow_161;
            this.btnNext.Location = new System.Drawing.Point(562, 59);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(28, 28);
            this.btnNext.TabIndex = 1;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrevious.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.UpArrow_161;
            this.btnPrevious.Location = new System.Drawing.Point(562, 25);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(28, 28);
            this.btnPrevious.TabIndex = 0;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // _tabControl
            // 
            this._tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tabControl.Controls.Add(this._tabGeneral);
            this._tabControl.Controls.Add(this._tabDetails);
            this._tabControl.Controls.Add(this._tabBeforeAfter);
            this._tabControl.Controls.Add(this._tabSensitiveColumns);
            this._tabControl.Location = new System.Drawing.Point(0, 0);
            this._tabControl.Name = "_tabControl";
            this._tabControl.SelectedIndex = 0;
            this._tabControl.Size = new System.Drawing.Size(536, 421);
            this._tabControl.TabIndex = 2;
            // 
            // _tabGeneral
            // 
            this._tabGeneral.Controls.Add(this.panelBottom);
            this._tabGeneral.Controls.Add(this.panelTop);
            this._tabGeneral.Location = new System.Drawing.Point(4, 22);
            this._tabGeneral.Name = "_tabGeneral";
            this._tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this._tabGeneral.Size = new System.Drawing.Size(528, 395);
            this._tabGeneral.TabIndex = 0;
            this._tabGeneral.Text = "General";
            this._tabGeneral.UseVisualStyleBackColor = true;
            // 
            // _tabDetails
            // 
			this._tabDetails.Controls.Add(this._gridDetails);
            this._tabDetails.Location = new System.Drawing.Point(4, 22);
            this._tabDetails.Name = "_tabDetails";
            this._tabDetails.Size = new System.Drawing.Size(528, 360);
            this._tabDetails.TabIndex = 1;
            this._tabDetails.Text = "Details";
            this._tabDetails.UseVisualStyleBackColor = true;
            // 
         // _gridDetails
         // 
         this._gridDetails.DataMember = "Band 0";
         this._gridDetails.DataSource = this._dsDetails;
         appearance14.BackColor = System.Drawing.SystemColors.Window;
         appearance14.BorderColor = System.Drawing.SystemColors.InactiveCaption;
         this._gridDetails.DisplayLayout.Appearance = appearance14;
         ultraGridColumn9.Header.VisiblePosition = 0;
         ultraGridColumn10.Header.VisiblePosition = 1;
         ultraGridBand4.Columns.AddRange(new object[] {
            ultraGridColumn9,
            ultraGridColumn10});
         this._gridDetails.DisplayLayout.BandsSerializer.Add(ultraGridBand4);
         this._gridDetails.Dock = System.Windows.Forms.DockStyle.Fill;
         this._gridDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._gridDetails.Location = new System.Drawing.Point(0, 0);
         this._gridDetails.Name = "_gridDetails";
         this._gridDetails.Size = new System.Drawing.Size(528, 360);
         this._gridDetails.TabIndex = 0;
         this._gridDetails.Text = "ultraGrid1";
         // 
         // _dsDetails
         // 
         this._dsDetails.Band.Columns.AddRange(new object[] {
            ultraDataColumn9,
            ultraDataColumn10});
         // 
         // _tabBeforeAfter
         // 
         this._tabBeforeAfter.Controls.Add(this._gridBeforeAfter);
         this._tabBeforeAfter.Controls.Add(this._lblBeforeAfterNotAvailable);
         this._tabBeforeAfter.Location = new System.Drawing.Point(4, 22);
         this._tabBeforeAfter.Name = "_tabBeforeAfter";
         this._tabBeforeAfter.Size = new System.Drawing.Size(528, 360);
         this._tabBeforeAfter.TabIndex = 2;
         this._tabBeforeAfter.Text = "Before-After Data";
         this._tabBeforeAfter.UseVisualStyleBackColor = true;
         // 
         // _gridBeforeAfter
         // 
         this._gridBeforeAfter.DataMember = "Band 0";
         this._gridBeforeAfter.DataSource = this._dsBeforeAfter;
         ultraGridColumn11.Header.VisiblePosition = 0;
         ultraGridColumn12.Header.VisiblePosition = 1;
         ultraGridColumn13.Header.VisiblePosition = 2;
         ultraGridColumn14.Header.VisiblePosition = 3;
         ultraGridColumn15.Header.VisiblePosition = 4;
         ultraGridBand5.Columns.AddRange(new object[] {
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn14,
            ultraGridColumn15});
         this._gridBeforeAfter.DisplayLayout.BandsSerializer.Add(ultraGridBand5);
         this._gridBeforeAfter.Dock = System.Windows.Forms.DockStyle.Fill;
         this._gridBeforeAfter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._gridBeforeAfter.Location = new System.Drawing.Point(0, 0);
         this._gridBeforeAfter.Name = "_gridBeforeAfter";
         this._gridBeforeAfter.Size = new System.Drawing.Size(528, 360);
         this._gridBeforeAfter.TabIndex = 2;
         this._gridBeforeAfter.Text = "ultraGrid1";
         // 
         // _dsBeforeAfter
         // 
         this._dsBeforeAfter.Band.Columns.AddRange(new object[] {
            ultraDataColumn11,
            ultraDataColumn12,
            ultraDataColumn13,
            ultraDataColumn14,
            ultraDataColumn15});
         // 
         // _lblBeforeAfterNotAvailable
         // 
         this._lblBeforeAfterNotAvailable.Location = new System.Drawing.Point(8, 14);
         this._lblBeforeAfterNotAvailable.Name = "_lblBeforeAfterNotAvailable";
         this._lblBeforeAfterNotAvailable.Size = new System.Drawing.Size(517, 89);
         this._lblBeforeAfterNotAvailable.TabIndex = 1;
         // 
         // _tabSensitiveColumns
         // 
         this._tabSensitiveColumns.Controls.Add(this._gridSensitiveColumns);
         this._tabSensitiveColumns.Location = new System.Drawing.Point(4, 22);
         this._tabSensitiveColumns.Name = "_tabSensitiveColumns";
         this._tabSensitiveColumns.Padding = new System.Windows.Forms.Padding(3);
         this._tabSensitiveColumns.Size = new System.Drawing.Size(528, 360);
         this._tabSensitiveColumns.TabIndex = 3;
         this._tabSensitiveColumns.Text = "Sensitive Columns";
         this._tabSensitiveColumns.UseVisualStyleBackColor = true;
         // 
         // _gridSensitiveColumns
         // 
         this._gridSensitiveColumns.DataSource = this._dsSensitiveColumns;
         appearance15.BackColor = System.Drawing.SystemColors.Window;
         appearance15.BorderColor = System.Drawing.SystemColors.InactiveCaption;
         this._gridSensitiveColumns.DisplayLayout.Appearance = appearance15;
         ultraGridColumn16.Header.Caption = "Column";         
         ultraGridColumn16.Header.VisiblePosition = 0;
         ultraGridColumn17.Header.Caption = "Row Count";
         ultraGridColumn17.Header.VisiblePosition = 1;
         ultraGridColumn17.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
         

         ultraGridBand6.Columns.AddRange(new object[] {
            ultraGridColumn16,ultraGridColumn17});
         this._gridSensitiveColumns.DisplayLayout.BandsSerializer.Add(ultraGridBand6);
         this._gridSensitiveColumns.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
         this._gridSensitiveColumns.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
         appearance16.BackColor = System.Drawing.SystemColors.ActiveBorder;
         appearance16.BackColor2 = System.Drawing.SystemColors.ControlDark;
         appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
         appearance16.BorderColor = System.Drawing.SystemColors.Window;
         this._gridSensitiveColumns.DisplayLayout.GroupByBox.Appearance = appearance16;
         appearance17.ForeColor = System.Drawing.SystemColors.GrayText;
         this._gridSensitiveColumns.DisplayLayout.GroupByBox.BandLabelAppearance = appearance17;
         this._gridSensitiveColumns.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
         appearance18.BackColor = System.Drawing.SystemColors.ControlLightLight;
         appearance18.BackColor2 = System.Drawing.SystemColors.Control;
         appearance18.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
         appearance18.ForeColor = System.Drawing.SystemColors.GrayText;
         this._gridSensitiveColumns.DisplayLayout.GroupByBox.PromptAppearance = appearance18;
         this._gridSensitiveColumns.DisplayLayout.MaxColScrollRegions = 1;
         this._gridSensitiveColumns.DisplayLayout.MaxRowScrollRegions = 1;
         appearance19.BackColor = System.Drawing.SystemColors.Window;
         appearance19.ForeColor = System.Drawing.SystemColors.ControlText;
         this._gridSensitiveColumns.DisplayLayout.Override.ActiveCellAppearance = appearance19;
         appearance20.BackColor = System.Drawing.SystemColors.Highlight;
         appearance20.ForeColor = System.Drawing.SystemColors.HighlightText;
         this._gridSensitiveColumns.DisplayLayout.Override.ActiveRowAppearance = appearance20;
         this._gridSensitiveColumns.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
         this._gridSensitiveColumns.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
         appearance21.BackColor = System.Drawing.SystemColors.Window;
         this._gridSensitiveColumns.DisplayLayout.Override.CardAreaAppearance = appearance21;
         appearance22.BorderColor = System.Drawing.Color.Silver;
         appearance22.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
         this._gridSensitiveColumns.DisplayLayout.Override.CellAppearance = appearance22;
         this._gridSensitiveColumns.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
         this._gridSensitiveColumns.DisplayLayout.Override.CellPadding = 0;
         appearance23.BackColor = System.Drawing.SystemColors.Control;
         appearance23.BackColor2 = System.Drawing.SystemColors.ControlDark;
         appearance23.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
         appearance23.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
         appearance23.BorderColor = System.Drawing.SystemColors.Window;
         this._gridSensitiveColumns.DisplayLayout.Override.GroupByRowAppearance = appearance23;
         appearance24.TextHAlignAsString = "Middle";
         this._gridSensitiveColumns.DisplayLayout.Override.HeaderAppearance = appearance24;
         this._gridSensitiveColumns.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
         this._gridSensitiveColumns.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
         appearance25.BackColor = System.Drawing.SystemColors.Window;
         appearance25.BorderColor = System.Drawing.Color.Silver;
         this._gridSensitiveColumns.DisplayLayout.Override.RowAppearance = appearance25;
         this._gridSensitiveColumns.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
         appearance26.BackColor = System.Drawing.SystemColors.ControlLight;
         this._gridSensitiveColumns.DisplayLayout.Override.TemplateAddRowAppearance = appearance26;
         this._gridSensitiveColumns.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
         this._gridSensitiveColumns.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
         this._gridSensitiveColumns.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
         this._gridSensitiveColumns.Dock = System.Windows.Forms.DockStyle.Fill;
         this._gridSensitiveColumns.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this._gridSensitiveColumns.Location = new System.Drawing.Point(3, 3);
         this._gridSensitiveColumns.Name = "_gridSensitiveColumns";
         this._gridSensitiveColumns.Size = new System.Drawing.Size(522, 354);
         this._gridSensitiveColumns.TabIndex = 0;
         this._gridSensitiveColumns.Text = "ultraGrid1";
         // 
         // _dsSensitiveColumns
         // 
         this._dsSensitiveColumns.Band.Columns.AddRange(new object[] {
            ultraDataColumn16,ultraDataColumn17});
         // 
         // pnlError
         // 
         this.pnlError.Controls.Add(this.lblError);
         this.pnlError.Dock = System.Windows.Forms.DockStyle.Left;
         this.pnlError.Location = new System.Drawing.Point(0, 0);
         this.pnlError.Name = "pnlError";
         this.pnlError.Size = new System.Drawing.Size(542, 384);
         this.pnlError.TabIndex = 3;
         this.pnlError.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(label10);
            this.groupBox2.Controls.Add(this.txtRowCounts);
            this.groupBox2.Location = new System.Drawing.Point(9, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(504, 47);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Row Count Summary";
            // 
            // label10
            // 
            label10.Location = new System.Drawing.Point(9, 19);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(106, 16);
            label10.TabIndex = 48;
            label10.Tag = "1";
            label10.Text = "Rows Affected:";
            // 
            // txtRowCounts
            // 
            this.txtRowCounts.Location = new System.Drawing.Point(121, 15);
            this.txtRowCounts.Name = "textBox1";
            this.txtRowCounts.ReadOnly = true;
            this.txtRowCounts.Size = new System.Drawing.Size(132, 20);
            this.txtRowCounts.TabIndex = 47;
            this.txtRowCounts.TabStop = false;
            this.txtRowCounts.Tag = "1";
            // 
            // Form_EventProperties
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(616, 419);
            this.Controls.Add(this._tabControl);
            this.Controls.Add(this.pnlError);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnPrevious);
            this.Controls.Add(this.btnNext);
            this.HelpButton = true;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "Form_EventProperties";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Event Properties";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_EventProperties_HelpRequested);
            this.panelBottom.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this._groupBeforeAfter.ResumeLayout(false);
            this._groupBeforeAfter.PerformLayout();
            this.grpError.ResumeLayout(false);
            this.grpError.PerformLayout();
            this._tabControl.ResumeLayout(false);
            this._tabGeneral.ResumeLayout(false);
this._tabDetails.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this._gridDetails)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._dsDetails)).EndInit();
         this._tabBeforeAfter.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this._gridBeforeAfter)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._dsBeforeAfter)).EndInit();
         this._tabSensitiveColumns.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this._gridSensitiveColumns)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._dsSensitiveColumns)).EndInit();
         this.pnlError.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
         this.ResumeLayout(false);

      }
      #endregion

      private System.Windows.Forms.Panel panelBottom;
      private System.Windows.Forms.Button btnClose;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.Button btnCopy;
      private System.Windows.Forms.Panel panelTop;
      private System.Windows.Forms.GroupBox grpError;
      private System.Windows.Forms.TextBox txtDetails;
      private System.Windows.Forms.TextBox txtCategory;
      private System.Windows.Forms.TextBox txtType;
      private System.Windows.Forms.TextBox txtUser;
      private System.Windows.Forms.TextBox txtDatabase;
      private System.Windows.Forms.TextBox txtObject;
      private System.Windows.Forms.Button btnNext;
      private System.Windows.Forms.Button btnPrevious;
      private System.Windows.Forms.TextBox txtTime;
      private System.Windows.Forms.Label lblError;
      private System.Windows.Forms.RichTextBox txtSQL;
      private System.Windows.Forms.TextBox txtApplication;
      private System.Windows.Forms.TabControl _tabControl;
      private System.Windows.Forms.TabPage _tabGeneral;
      private System.Windows.Forms.TabPage _tabDetails;
      private System.Windows.Forms.TabPage _tabBeforeAfter;
      private System.Windows.Forms.GroupBox _groupBeforeAfter;
      private System.Windows.Forms.TextBox txtRowsAffected;
      private System.Windows.Forms.TextBox txtColumnsAffected;
      private System.Windows.Forms.Label _lblBeforeAfterNotAvailable;
      private Infragistics.Win.UltraWinGrid.UltraGrid _gridDetails;
      private Infragistics.Win.UltraWinDataSource.UltraDataSource _dsDetails;
      private System.ComponentModel.IContainer components;
      private Infragistics.Win.UltraWinGrid.UltraGrid _gridBeforeAfter;
      private Infragistics.Win.UltraWinDataSource.UltraDataSource _dsBeforeAfter;
      private System.Windows.Forms.TabPage _tabSensitiveColumns;
      private Infragistics.Win.UltraWinGrid.UltraGrid _gridSensitiveColumns;
      private Infragistics.Win.UltraWinDataSource.UltraDataSource _dsSensitiveColumns;
      private System.Windows.Forms.Panel pnlError;
      private System.Windows.Forms.GroupBox groupBox2;
      private System.Windows.Forms.TextBox txtRowCounts;
	}
}