namespace Idera.SQLcompliance.Application.GUI.Forms
{
    partial class Form_SensitiveColumnsProfiles
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Category", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Search String Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Definition");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Select");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OriginalSelect");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Category");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Search String Name");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Definition");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Select");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("OriginalSelect");
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            this.ultraGridProfiles = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraDataSourceProfiles = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this.ubtnClose = new Infragistics.Win.Misc.UltraButton();
            this.ubtnDeleteProfile = new Infragistics.Win.Misc.UltraButton();
            this.ubtnOpenProfile = new Infragistics.Win.Misc.UltraButton();
            this.ubtnSaveNewProfile = new Infragistics.Win.Misc.UltraButton();
            this.ubtnSaveProfile = new Infragistics.Win.Misc.UltraButton();
            this.ulblActiveSearchProfile = new Infragistics.Win.Misc.UltraLabel();
            this.uteActiveSearchProfile = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.llToggleSelection = new System.Windows.Forms.LinkLabel();
            this.ubtnEditString = new Infragistics.Win.Misc.UltraButton();
            this.ubtnNewString = new Infragistics.Win.Misc.UltraButton();
            this.ubtnDeleteString = new Infragistics.Win.Misc.UltraButton();
            this.ubtnSaveString = new Infragistics.Win.Misc.UltraButton();
            this.ubtnDiscardString = new Infragistics.Win.Misc.UltraButton();
            this.ultraComboEditorCategory = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraPanelEditString = new Infragistics.Win.Misc.UltraPanel();
            this.ugbString = new Infragistics.Win.Misc.UltraGroupBox();
            this.ulblDefinition = new Infragistics.Win.Misc.UltraLabel();
            this.ultraTextEditorSearchStringName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ulblSearchStringName = new Infragistics.Win.Misc.UltraLabel();
            this.ultraTextEditorDefinition = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ulblCategory = new Infragistics.Win.Misc.UltraLabel();
            this.ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            this.ultraPanel2 = new Infragistics.Win.Misc.UltraPanel();
            this.ultraPanel3 = new Infragistics.Win.Misc.UltraPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.llHelp = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridProfiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSourceProfiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteActiveSearchProfile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraComboEditorCategory)).BeginInit();
            this.ultraPanelEditString.ClientArea.SuspendLayout();
            this.ultraPanelEditString.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugbString)).BeginInit();
            this.ugbString.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditorSearchStringName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditorDefinition)).BeginInit();
            this.ultraPanel1.ClientArea.SuspendLayout();
            this.ultraPanel1.SuspendLayout();
            this.ultraPanel2.ClientArea.SuspendLayout();
            this.ultraPanel2.SuspendLayout();
            this.ultraPanel3.ClientArea.SuspendLayout();
            this.ultraPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGridProfiles
            // 
            this.ultraGridProfiles.DataSource = this.ultraDataSourceProfiles;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ultraGridProfiles.DisplayLayout.Appearance = appearance1;
            this.ultraGridProfiles.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 193;
            ultraGridColumn2.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 242;
            ultraGridColumn3.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            appearance2.TextHAlignAsString = "Center";
            ultraGridColumn3.CellAppearance = appearance2;
            appearance3.TextHAlignAsString = "Center";
            ultraGridColumn3.Header.Appearance = appearance3;
            ultraGridColumn3.Header.TextOrientation = new Infragistics.Win.TextOrientationInfo(0, Infragistics.Win.TextFlowDirection.Horizontal);
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 206;
            ultraGridColumn4.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.Edit;
            appearance4.TextHAlignAsString = "Center";
            ultraGridColumn4.Header.Appearance = appearance4;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Width = 99;
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn5.Hidden = true;
            ultraGridColumn5.Width = 87;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5});
            this.ultraGridProfiles.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.ultraGridProfiles.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ultraGridProfiles.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance5.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance5.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance5.BorderColor = System.Drawing.SystemColors.Window;
            this.ultraGridProfiles.DisplayLayout.GroupByBox.Appearance = appearance5;
            appearance6.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ultraGridProfiles.DisplayLayout.GroupByBox.BandLabelAppearance = appearance6;
            this.ultraGridProfiles.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance7.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance7.BackColor2 = System.Drawing.SystemColors.Control;
            appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance7.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ultraGridProfiles.DisplayLayout.GroupByBox.PromptAppearance = appearance7;
            this.ultraGridProfiles.DisplayLayout.MaxColScrollRegions = 1;
            this.ultraGridProfiles.DisplayLayout.MaxRowScrollRegions = 1;
            appearance8.BackColor = System.Drawing.SystemColors.Window;
            appearance8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ultraGridProfiles.DisplayLayout.Override.ActiveCellAppearance = appearance8;
            appearance9.BackColor = System.Drawing.SystemColors.Highlight;
            appearance9.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.ultraGridProfiles.DisplayLayout.Override.ActiveRowAppearance = appearance9;
            this.ultraGridProfiles.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.ultraGridProfiles.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance10.BackColor = System.Drawing.SystemColors.Window;
            this.ultraGridProfiles.DisplayLayout.Override.CardAreaAppearance = appearance10;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            appearance11.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ultraGridProfiles.DisplayLayout.Override.CellAppearance = appearance11;
            this.ultraGridProfiles.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.ultraGridProfiles.DisplayLayout.Override.CellPadding = 0;
            appearance12.BackColor = System.Drawing.SystemColors.Control;
            appearance12.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance12.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance12.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance12.BorderColor = System.Drawing.SystemColors.Window;
            this.ultraGridProfiles.DisplayLayout.Override.GroupByRowAppearance = appearance12;
            appearance13.TextHAlignAsString = "Left";
            this.ultraGridProfiles.DisplayLayout.Override.HeaderAppearance = appearance13;
            this.ultraGridProfiles.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ultraGridProfiles.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance14.BackColor = System.Drawing.SystemColors.Window;
            appearance14.BorderColor = System.Drawing.Color.Silver;
            this.ultraGridProfiles.DisplayLayout.Override.RowAppearance = appearance14;
            this.ultraGridProfiles.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.ultraGridProfiles.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.ultraGridProfiles.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.ultraGridProfiles.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance15.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ultraGridProfiles.DisplayLayout.Override.TemplateAddRowAppearance = appearance15;
            this.ultraGridProfiles.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ultraGridProfiles.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.ultraGridProfiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGridProfiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraGridProfiles.Location = new System.Drawing.Point(14, 49);
            this.ultraGridProfiles.Name = "ultraGridProfiles";
            this.ultraGridProfiles.Size = new System.Drawing.Size(742, 312);
            this.ultraGridProfiles.TabIndex = 1;
            this.ultraGridProfiles.Text = "ultraGrid1";
            this.ultraGridProfiles.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultraGridProfiles_AfterCellUpdate);
            this.ultraGridProfiles.AfterRowActivate += new System.EventHandler(this.ultraGridProfiles_AfterRowActivate);
            this.ultraGridProfiles.CellChange += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultraGridProfiles_CellChange);
            // 
            // ultraDataSourceProfiles
            // 
            this.ultraDataSourceProfiles.AllowAdd = false;
            this.ultraDataSourceProfiles.AllowDelete = false;
            ultraDataColumn1.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn2.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn3.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            ultraDataColumn4.DataType = typeof(bool);
            ultraDataColumn4.DefaultValue = false;
            ultraDataColumn4.ReadOnly = Infragistics.Win.DefaultableBoolean.False;
            ultraDataColumn5.DataType = typeof(bool);
            ultraDataColumn5.DefaultValue = false;
            ultraDataColumn5.ReadOnly = Infragistics.Win.DefaultableBoolean.True;
            this.ultraDataSourceProfiles.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5});
            // 
            // ubtnClose
            // 
            this.ubtnClose.Location = new System.Drawing.Point(14, 14);
            this.ubtnClose.Margin = new System.Windows.Forms.Padding(14, 14, 7, 14);
            this.ubtnClose.Name = "ubtnClose";
            this.ubtnClose.Size = new System.Drawing.Size(114, 23);
            this.ubtnClose.TabIndex = 0;
            this.ubtnClose.Text = "Close";
            this.ubtnClose.Click += new System.EventHandler(this.ubtnClose_Click);
            // 
            // ubtnDeleteProfile
            // 
            this.ubtnDeleteProfile.Location = new System.Drawing.Point(142, 14);
            this.ubtnDeleteProfile.Margin = new System.Windows.Forms.Padding(7);
            this.ubtnDeleteProfile.Name = "ubtnDeleteProfile";
            this.ubtnDeleteProfile.Size = new System.Drawing.Size(114, 23);
            this.ubtnDeleteProfile.TabIndex = 1;
            this.ubtnDeleteProfile.Text = "Delete Profile";
            this.ubtnDeleteProfile.Click += new System.EventHandler(this.ubtnDeleteProfile_Click);
            // 
            // ubtnOpenProfile
            // 
            this.ubtnOpenProfile.Location = new System.Drawing.Point(270, 14);
            this.ubtnOpenProfile.Margin = new System.Windows.Forms.Padding(7);
            this.ubtnOpenProfile.Name = "ubtnOpenProfile";
            this.ubtnOpenProfile.Size = new System.Drawing.Size(114, 23);
            this.ubtnOpenProfile.TabIndex = 2;
            this.ubtnOpenProfile.Text = "Open Profile";
            this.ubtnOpenProfile.Click += new System.EventHandler(this.ubtnOpenProfile_Click);
            // 
            // ubtnSaveNewProfile
            // 
            this.ubtnSaveNewProfile.Location = new System.Drawing.Point(398, 14);
            this.ubtnSaveNewProfile.Margin = new System.Windows.Forms.Padding(7);
            this.ubtnSaveNewProfile.Name = "ubtnSaveNewProfile";
            this.ubtnSaveNewProfile.Size = new System.Drawing.Size(114, 23);
            this.ubtnSaveNewProfile.TabIndex = 3;
            this.ubtnSaveNewProfile.Text = "Save New Profile";
            this.ubtnSaveNewProfile.Click += new System.EventHandler(this.ubtnSaveNewProfile_Click);
            // 
            // ubtnSaveProfile
            // 
            this.ubtnSaveProfile.Location = new System.Drawing.Point(526, 14);
            this.ubtnSaveProfile.Margin = new System.Windows.Forms.Padding(7);
            this.ubtnSaveProfile.Name = "ubtnSaveProfile";
            this.ubtnSaveProfile.Size = new System.Drawing.Size(114, 23);
            this.ubtnSaveProfile.TabIndex = 4;
            this.ubtnSaveProfile.Text = "Save Profile";
            this.ubtnSaveProfile.Click += new System.EventHandler(this.ubtnSaveProfile_Click);
            // 
            // ulblActiveSearchProfile
            // 
            appearance16.TextVAlignAsString = "Middle";
            this.ulblActiveSearchProfile.Appearance = appearance16;
            this.ulblActiveSearchProfile.Location = new System.Drawing.Point(12, 16);
            this.ulblActiveSearchProfile.Name = "ulblActiveSearchProfile";
            this.ulblActiveSearchProfile.Size = new System.Drawing.Size(116, 19);
            this.ulblActiveSearchProfile.TabIndex = 6;
            this.ulblActiveSearchProfile.Text = "Active Search Profile";
            // 
            // uteActiveSearchProfile
            // 
            this.uteActiveSearchProfile.Location = new System.Drawing.Point(122, 14);
            this.uteActiveSearchProfile.Margin = new System.Windows.Forms.Padding(14);
            this.uteActiveSearchProfile.Name = "uteActiveSearchProfile";
            this.uteActiveSearchProfile.ReadOnly = true;
            this.uteActiveSearchProfile.Size = new System.Drawing.Size(194, 21);
            this.uteActiveSearchProfile.TabIndex = 7;
            // 
            // llToggleSelection
            // 
            this.llToggleSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llToggleSelection.AutoSize = true;
            this.llToggleSelection.Location = new System.Drawing.Point(667, 18);
            this.llToggleSelection.Name = "llToggleSelection";
            this.llToggleSelection.Size = new System.Drawing.Size(51, 13);
            this.llToggleSelection.TabIndex = 0;
            this.llToggleSelection.TabStop = true;
            this.llToggleSelection.Text = "Select All";
            this.llToggleSelection.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llToggleSelection_LinkClicked);
            // 
            // ubtnEditString
            // 
            this.ubtnEditString.Location = new System.Drawing.Point(17, 49);
            this.ubtnEditString.Margin = new System.Windows.Forms.Padding(14, 14, 14, 7);
            this.ubtnEditString.Name = "ubtnEditString";
            this.ubtnEditString.Size = new System.Drawing.Size(87, 23);
            this.ubtnEditString.TabIndex = 1;
            this.ubtnEditString.Text = "Edit String";
            this.ubtnEditString.Click += new System.EventHandler(this.ubtnEditString_Click);
            // 
            // ubtnNewString
            // 
            this.ubtnNewString.Location = new System.Drawing.Point(17, 86);
            this.ubtnNewString.Margin = new System.Windows.Forms.Padding(7);
            this.ubtnNewString.Name = "ubtnNewString";
            this.ubtnNewString.Size = new System.Drawing.Size(87, 23);
            this.ubtnNewString.TabIndex = 2;
            this.ubtnNewString.Text = "New String";
            this.ubtnNewString.Click += new System.EventHandler(this.ubtnNewString_Click);
            // 
            // ubtnDeleteString
            // 
            this.ubtnDeleteString.Location = new System.Drawing.Point(17, 123);
            this.ubtnDeleteString.Margin = new System.Windows.Forms.Padding(7);
            this.ubtnDeleteString.Name = "ubtnDeleteString";
            this.ubtnDeleteString.Size = new System.Drawing.Size(87, 23);
            this.ubtnDeleteString.TabIndex = 3;
            this.ubtnDeleteString.Text = "Delete String";
            this.ubtnDeleteString.Click += new System.EventHandler(this.ubtnDeleteString_Click);
            // 
            // ubtnSaveString
            // 
            this.ubtnSaveString.Location = new System.Drawing.Point(17, 160);
            this.ubtnSaveString.Margin = new System.Windows.Forms.Padding(7);
            this.ubtnSaveString.Name = "ubtnSaveString";
            this.ubtnSaveString.Size = new System.Drawing.Size(87, 23);
            this.ubtnSaveString.TabIndex = 4;
            this.ubtnSaveString.Text = "Save String";
            this.ubtnSaveString.Click += new System.EventHandler(this.ubtnSaveString_Click);
            // 
            // ubtnDiscardString
            // 
            this.ubtnDiscardString.Location = new System.Drawing.Point(17, 197);
            this.ubtnDiscardString.Margin = new System.Windows.Forms.Padding(7);
            this.ubtnDiscardString.Name = "ubtnDiscardString";
            this.ubtnDiscardString.Size = new System.Drawing.Size(87, 23);
            this.ubtnDiscardString.TabIndex = 5;
            this.ubtnDiscardString.Text = "Discard";
            this.ubtnDiscardString.Click += new System.EventHandler(this.ubtnDiscardString_Click);
            // 
            // ultraComboEditorCategory
            // 
            this.ultraComboEditorCategory.Location = new System.Drawing.Point(24, 53);
            this.ultraComboEditorCategory.Margin = new System.Windows.Forms.Padding(14);
            this.ultraComboEditorCategory.Name = "ultraComboEditorCategory";
            this.ultraComboEditorCategory.Size = new System.Drawing.Size(376, 21);
            this.ultraComboEditorCategory.TabIndex = 1;
            this.ultraComboEditorCategory.TextChanged += new System.EventHandler(this.ultraComboEditorCategory_TextChanged);
            // 
            // ultraPanelEditString
            // 
            // 
            // ultraPanelEditString.ClientArea
            // 
            this.ultraPanelEditString.ClientArea.Controls.Add(this.ugbString);
            this.ultraPanelEditString.Location = new System.Drawing.Point(199, 65);
            this.ultraPanelEditString.Name = "ultraPanelEditString";
            this.ultraPanelEditString.Size = new System.Drawing.Size(417, 246);
            this.ultraPanelEditString.TabIndex = 16;
            this.ultraPanelEditString.Visible = false;
            // 
            // ugbString
            // 
            this.ugbString.Controls.Add(this.ultraComboEditorCategory);
            this.ugbString.Controls.Add(this.ulblDefinition);
            this.ugbString.Controls.Add(this.ultraTextEditorSearchStringName);
            this.ugbString.Controls.Add(this.ulblSearchStringName);
            this.ugbString.Controls.Add(this.ultraTextEditorDefinition);
            this.ugbString.Controls.Add(this.ulblCategory);
            this.ugbString.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugbString.Location = new System.Drawing.Point(0, 0);
            this.ugbString.Name = "ugbString";
            this.ugbString.Size = new System.Drawing.Size(417, 246);
            this.ugbString.TabIndex = 0;
            this.ugbString.Text = "ultraGroupBox1";
            // 
            // ulblDefinition
            // 
            this.ulblDefinition.Location = new System.Drawing.Point(24, 158);
            this.ulblDefinition.Margin = new System.Windows.Forms.Padding(14, 14, 14, 3);
            this.ulblDefinition.Name = "ulblDefinition";
            this.ulblDefinition.Size = new System.Drawing.Size(100, 23);
            this.ulblDefinition.TabIndex = 4;
            this.ulblDefinition.Text = "Definition";
            // 
            // ultraTextEditorSearchStringName
            // 
            this.ultraTextEditorSearchStringName.Location = new System.Drawing.Point(24, 120);
            this.ultraTextEditorSearchStringName.Margin = new System.Windows.Forms.Padding(14);
            this.ultraTextEditorSearchStringName.Name = "ultraTextEditorSearchStringName";
            this.ultraTextEditorSearchStringName.Size = new System.Drawing.Size(376, 21);
            this.ultraTextEditorSearchStringName.TabIndex = 3;
            this.ultraTextEditorSearchStringName.TextChanged += new System.EventHandler(this.ultraTextEditorSearchStringName_TextChanged);
            // 
            // ulblSearchStringName
            // 
            this.ulblSearchStringName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ulblSearchStringName.Location = new System.Drawing.Point(24, 91);
            this.ulblSearchStringName.Margin = new System.Windows.Forms.Padding(14, 14, 14, 3);
            this.ulblSearchStringName.Name = "ulblSearchStringName";
            this.ulblSearchStringName.Size = new System.Drawing.Size(144, 23);
            this.ulblSearchStringName.TabIndex = 2;
            this.ulblSearchStringName.Text = "Search String Name";
            // 
            // ultraTextEditorDefinition
            // 
            this.errorProvider.SetIconPadding(this.ultraTextEditorDefinition, -20);
            this.ultraTextEditorDefinition.Location = new System.Drawing.Point(24, 187);
            this.ultraTextEditorDefinition.Margin = new System.Windows.Forms.Padding(14);
            this.ultraTextEditorDefinition.Name = "ultraTextEditorDefinition";
            this.ultraTextEditorDefinition.Size = new System.Drawing.Size(376, 21);
            this.ultraTextEditorDefinition.TabIndex = 5;
            this.ultraTextEditorDefinition.TextChanged += new System.EventHandler(this.ultraTextEditorDefinition_TextChanged);
            // 
            // ulblCategory
            // 
            this.ulblCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ulblCategory.Location = new System.Drawing.Point(24, 24);
            this.ulblCategory.Margin = new System.Windows.Forms.Padding(14, 14, 14, 3);
            this.ulblCategory.Name = "ulblCategory";
            this.ulblCategory.Size = new System.Drawing.Size(100, 23);
            this.ulblCategory.TabIndex = 0;
            this.ulblCategory.Text = "Category";
            // 
            // ultraPanel1
            // 
            // 
            // ultraPanel1.ClientArea
            // 
            this.ultraPanel1.ClientArea.Controls.Add(this.llHelp);
            this.ultraPanel1.ClientArea.Controls.Add(this.ubtnNewString);
            this.ultraPanel1.ClientArea.Controls.Add(this.ubtnDeleteString);
            this.ultraPanel1.ClientArea.Controls.Add(this.ubtnSaveString);
            this.ultraPanel1.ClientArea.Controls.Add(this.ubtnEditString);
            this.ultraPanel1.ClientArea.Controls.Add(this.ubtnDiscardString);
            this.ultraPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.ultraPanel1.Location = new System.Drawing.Point(756, 0);
            this.ultraPanel1.Name = "ultraPanel1";
            this.ultraPanel1.Size = new System.Drawing.Size(118, 412);
            this.ultraPanel1.TabIndex = 3;
            // 
            // ultraPanel2
            // 
            // 
            // ultraPanel2.ClientArea
            // 
            this.ultraPanel2.ClientArea.Controls.Add(this.ubtnClose);
            this.ultraPanel2.ClientArea.Controls.Add(this.ubtnDeleteProfile);
            this.ultraPanel2.ClientArea.Controls.Add(this.ubtnOpenProfile);
            this.ultraPanel2.ClientArea.Controls.Add(this.ubtnSaveNewProfile);
            this.ultraPanel2.ClientArea.Controls.Add(this.ubtnSaveProfile);
            this.ultraPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ultraPanel2.Location = new System.Drawing.Point(0, 361);
            this.ultraPanel2.Name = "ultraPanel2";
            this.ultraPanel2.Size = new System.Drawing.Size(756, 51);
            this.ultraPanel2.TabIndex = 2;
            // 
            // ultraPanel3
            // 
            // 
            // ultraPanel3.ClientArea
            // 
            this.ultraPanel3.ClientArea.Controls.Add(this.uteActiveSearchProfile);
            this.ultraPanel3.ClientArea.Controls.Add(this.ulblActiveSearchProfile);
            this.ultraPanel3.ClientArea.Controls.Add(this.llToggleSelection);
            this.ultraPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraPanel3.Location = new System.Drawing.Point(0, 0);
            this.ultraPanel3.Name = "ultraPanel3";
            this.ultraPanel3.Size = new System.Drawing.Size(756, 49);
            this.ultraPanel3.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 49);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(14, 312);
            this.panel1.TabIndex = 1;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // llHelp
            // 
            this.llHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llHelp.Location = new System.Drawing.Point(17, 18);
            this.llHelp.Name = "llHelp";
            this.llHelp.Size = new System.Drawing.Size(87, 13);
            this.llHelp.TabIndex = 0;
            this.llHelp.TabStop = true;
            this.llHelp.Text = "Settings Help";
            this.llHelp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.llHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llHelp_LinkClicked);
            // 
            // Form_SensitiveColumnsProfiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 412);
            this.Controls.Add(this.ultraPanelEditString);
            this.Controls.Add(this.ultraGridProfiles);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ultraPanel3);
            this.Controls.Add(this.ultraPanel2);
            this.Controls.Add(this.ultraPanel1);
            this.MinimumSize = new System.Drawing.Size(780, 360);
            this.Name = "Form_SensitiveColumnsProfiles";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Column Search Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_SensitiveColumnsProfiles_FormClosing);
            this.Load += new System.EventHandler(this.Form_SensitiveColumnsProfiles_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridProfiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSourceProfiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteActiveSearchProfile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraComboEditorCategory)).EndInit();
            this.ultraPanelEditString.ClientArea.ResumeLayout(false);
            this.ultraPanelEditString.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ugbString)).EndInit();
            this.ugbString.ResumeLayout(false);
            this.ugbString.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditorSearchStringName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditorDefinition)).EndInit();
            this.ultraPanel1.ClientArea.ResumeLayout(false);
            this.ultraPanel1.ResumeLayout(false);
            this.ultraPanel2.ClientArea.ResumeLayout(false);
            this.ultraPanel2.ResumeLayout(false);
            this.ultraPanel3.ClientArea.ResumeLayout(false);
            this.ultraPanel3.ClientArea.PerformLayout();
            this.ultraPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinGrid.UltraGrid ultraGridProfiles;
        private Infragistics.Win.UltraWinDataSource.UltraDataSource ultraDataSourceProfiles;
        private Infragistics.Win.Misc.UltraButton ubtnClose;
        private Infragistics.Win.Misc.UltraButton ubtnDeleteProfile;
        private Infragistics.Win.Misc.UltraButton ubtnOpenProfile;
        private Infragistics.Win.Misc.UltraButton ubtnSaveNewProfile;
        private Infragistics.Win.Misc.UltraButton ubtnSaveProfile;
        private Infragistics.Win.Misc.UltraLabel ulblActiveSearchProfile;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor uteActiveSearchProfile;
        private System.Windows.Forms.LinkLabel llToggleSelection;
        private Infragistics.Win.Misc.UltraButton ubtnEditString;
        private Infragistics.Win.Misc.UltraButton ubtnNewString;
        private Infragistics.Win.Misc.UltraButton ubtnDeleteString;
        private Infragistics.Win.Misc.UltraButton ubtnSaveString;
        private Infragistics.Win.Misc.UltraButton ubtnDiscardString;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor ultraComboEditorCategory;
        private Infragistics.Win.Misc.UltraPanel ultraPanelEditString;
        private Infragistics.Win.Misc.UltraLabel ulblDefinition;
        private Infragistics.Win.Misc.UltraLabel ulblSearchStringName;
        private Infragistics.Win.Misc.UltraLabel ulblCategory;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor ultraTextEditorDefinition;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor ultraTextEditorSearchStringName;
        private Infragistics.Win.Misc.UltraPanel ultraPanel1;
        private Infragistics.Win.Misc.UltraPanel ultraPanel2;
        private Infragistics.Win.Misc.UltraPanel ultraPanel3;
        private System.Windows.Forms.Panel panel1;
        private Infragistics.Win.Misc.UltraGroupBox ugbString;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.LinkLabel llHelp;
    }
}