namespace Idera.SQLcompliance.Application.GUI.Forms
{
   partial class Form_ArchiveOptions
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
          this.groupBox5 = new System.Windows.Forms.GroupBox();
          this.btnBrowseArchiveDBFilesDirectory = new System.Windows.Forms.Button();
          this.txtArchiveDBFilesDirectory = new System.Windows.Forms.TextBox();
          this.label5 = new System.Windows.Forms.Label();
          this.textBox1 = new System.Windows.Forms.TextBox();
          this.label16 = new System.Windows.Forms.Label();
          this.textPrefix = new System.Windows.Forms.TextBox();
          this.label15 = new System.Windows.Forms.Label();
          this.comboPeriod = new System.Windows.Forms.ComboBox();
          this.label14 = new System.Windows.Forms.Label();
          this.groupBox1 = new System.Windows.Forms.GroupBox();
          this._comboSkipIntegrity = new System.Windows.Forms.ComboBox();
          this._lblSkipIntegrity = new System.Windows.Forms.Label();
          this.label3 = new System.Windows.Forms.Label();
          this.comboTimeZone = new System.Windows.Forms.ComboBox();
          this.label2 = new System.Windows.Forms.Label();
          this.label1 = new System.Windows.Forms.Label();
          this.label9 = new System.Windows.Forms.Label();
          this.label4 = new System.Windows.Forms.Label();
          this.linkLblHelpBestPractices = new System.Windows.Forms.LinkLabel();
          this.groupBox2 = new System.Windows.Forms.GroupBox();
          this.updnWeekRepetition = new System.Windows.Forms.NumericUpDown();
          this.label13 = new System.Windows.Forms.Label();
          this.dttmMonthlyScheduleTime = new System.Windows.Forms.DateTimePicker();
          this.pnlWeekDays = new System.Windows.Forms.Panel();
          this.chkSaturday = new System.Windows.Forms.CheckBox();
          this.chkFriday = new System.Windows.Forms.CheckBox();
          this.chkThursday = new System.Windows.Forms.CheckBox();
          this.chkWednesday = new System.Windows.Forms.CheckBox();
          this.chkTuesday = new System.Windows.Forms.CheckBox();
          this.chkMonday = new System.Windows.Forms.CheckBox();
          this.chkSunday = new System.Windows.Forms.CheckBox();
          this.pnlMonthlySchedule = new System.Windows.Forms.Panel();
          this.updnMonthRepetition2 = new System.Windows.Forms.NumericUpDown();
          this.updnMonthRepetition1 = new System.Windows.Forms.NumericUpDown();
          this.updnMonthDay = new System.Windows.Forms.NumericUpDown();
          this.label12 = new System.Windows.Forms.Label();
          this.label11 = new System.Windows.Forms.Label();
          this.label10 = new System.Windows.Forms.Label();
          this.label8 = new System.Windows.Forms.Label();
          this.cboWeekday = new System.Windows.Forms.ComboBox();
          this.cboMonthWeek = new System.Windows.Forms.ComboBox();
          this.radSchedule_WeekdayWise = new System.Windows.Forms.RadioButton();
          this.radSchedule_DateWise = new System.Windows.Forms.RadioButton();
          this.radMonthlySchedule = new System.Windows.Forms.RadioButton();
          this.label7 = new System.Windows.Forms.Label();
          this.dttmWeeklyScheduleTime = new System.Windows.Forms.DateTimePicker();
          this.label6 = new System.Windows.Forms.Label();
          this.radWeeklySchedule = new System.Windows.Forms.RadioButton();
          this.dttmDailyScheduleTime = new System.Windows.Forms.DateTimePicker();
          this.radDailySchedule = new System.Windows.Forms.RadioButton();
          this.radNoSchedule = new System.Windows.Forms.RadioButton();
          this.textAge = new Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox();
          this.groupBox5.SuspendLayout();
          this.groupBox1.SuspendLayout();
          this.groupBox2.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this.updnWeekRepetition)).BeginInit();
          this.pnlWeekDays.SuspendLayout();
          this.pnlMonthlySchedule.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this.updnMonthRepetition2)).BeginInit();
          ((System.ComponentModel.ISupportInitialize)(this.updnMonthRepetition1)).BeginInit();
          ((System.ComponentModel.ISupportInitialize)(this.updnMonthDay)).BeginInit();
          this.SuspendLayout();
          // 
          // btnOK
          // 
          this.btnOK.Location = new System.Drawing.Point(375, 524);
          this.btnOK.Name = "btnOK";
          this.btnOK.Size = new System.Drawing.Size(75, 23);
          this.btnOK.TabIndex = 11;
          this.btnOK.Text = "&OK";
          this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
          // 
          // btnCancel
          // 
          this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
          this.btnCancel.Location = new System.Drawing.Point(456, 524);
          this.btnCancel.Name = "btnCancel";
          this.btnCancel.Size = new System.Drawing.Size(75, 23);
          this.btnCancel.TabIndex = 12;
          this.btnCancel.Text = "&Cancel";
          this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
          // 
          // groupBox5
          // 
          this.groupBox5.Controls.Add(this.btnBrowseArchiveDBFilesDirectory);
          this.groupBox5.Controls.Add(this.txtArchiveDBFilesDirectory);
          this.groupBox5.Controls.Add(this.label5);
          this.groupBox5.Controls.Add(this.textBox1);
          this.groupBox5.Controls.Add(this.label16);
          this.groupBox5.Controls.Add(this.textPrefix);
          this.groupBox5.Controls.Add(this.label15);
          this.groupBox5.Controls.Add(this.comboPeriod);
          this.groupBox5.Controls.Add(this.label14);
          this.groupBox5.Location = new System.Drawing.Point(12, 400);
          this.groupBox5.Name = "groupBox5";
          this.groupBox5.Size = new System.Drawing.Size(520, 118);
          this.groupBox5.TabIndex = 13;
          this.groupBox5.TabStop = false;
          this.groupBox5.Text = "Archive Database Creation";
          // 
          // btnBrowseArchiveDBFilesDirectory
          // 
          this.btnBrowseArchiveDBFilesDirectory.Location = new System.Drawing.Point(439, 89);
          this.btnBrowseArchiveDBFilesDirectory.Name = "btnBrowseArchiveDBFilesDirectory";
          this.btnBrowseArchiveDBFilesDirectory.Size = new System.Drawing.Size(75, 23);
          this.btnBrowseArchiveDBFilesDirectory.TabIndex = 13;
          this.btnBrowseArchiveDBFilesDirectory.Text = "Browse";
          this.btnBrowseArchiveDBFilesDirectory.UseVisualStyleBackColor = true;
          this.btnBrowseArchiveDBFilesDirectory.Click += new System.EventHandler(this.btnBrowseArchiveDBFilesDirectory_Click);
          // 
          // txtArchiveDBFilesDirectory
          // 
          this.txtArchiveDBFilesDirectory.Location = new System.Drawing.Point(185, 91);
          this.txtArchiveDBFilesDirectory.Name = "txtArchiveDBFilesDirectory";
          this.txtArchiveDBFilesDirectory.Size = new System.Drawing.Size(248, 20);
          this.txtArchiveDBFilesDirectory.TabIndex = 12;
          this.txtArchiveDBFilesDirectory.DoubleClick += new System.EventHandler(this.txtArchiveDBFilesDirectory_DoubleClick);
          // 
          // label5
          // 
          this.label5.Location = new System.Drawing.Point(16, 94);
          this.label5.Name = "label5";
          this.label5.Size = new System.Drawing.Size(163, 20);
          this.label5.TabIndex = 11;
          this.label5.Text = "Archive Database Files Location:";
          // 
          // textBox1
          // 
          this.textBox1.Location = new System.Drawing.Point(272, 48);
          this.textBox1.Name = "textBox1";
          this.textBox1.ReadOnly = true;
          this.textBox1.Size = new System.Drawing.Size(148, 20);
          this.textBox1.TabIndex = 5;
          this.textBox1.TabStop = false;
          this.textBox1.Text = "_{instancename}_{timeperiod}";
          // 
          // label16
          // 
          this.label16.Location = new System.Drawing.Point(152, 72);
          this.label16.Name = "label16";
          this.label16.Size = new System.Drawing.Size(288, 16);
          this.label16.TabIndex = 4;
          this.label16.Text = "(Example: SQLcmArchive_IDERA_2005_Q1 )";
          // 
          // textPrefix
          // 
          this.textPrefix.Location = new System.Drawing.Point(148, 48);
          this.textPrefix.MaxLength = 32;
          this.textPrefix.Name = "textPrefix";
          this.textPrefix.Size = new System.Drawing.Size(124, 20);
          this.textPrefix.TabIndex = 10;
          this.textPrefix.Text = "SQLcmArchive";
          this.textPrefix.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textPrefix_KeyPress);
          // 
          // label15
          // 
          this.label15.Location = new System.Drawing.Point(16, 52);
          this.label15.Name = "label15";
          this.label15.Size = new System.Drawing.Size(128, 20);
          this.label15.TabIndex = 9;
          this.label15.Text = "Archive database prefix:";
          // 
          // comboPeriod
          // 
          this.comboPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
          this.comboPeriod.Items.AddRange(new object[] {
            "Quarter",
            "Month",
            "Year"});
          this.comboPeriod.Location = new System.Drawing.Point(220, 20);
          this.comboPeriod.Name = "comboPeriod";
          this.comboPeriod.Size = new System.Drawing.Size(72, 21);
          this.comboPeriod.TabIndex = 8;
          // 
          // label14
          // 
          this.label14.Location = new System.Drawing.Point(16, 24);
          this.label14.Name = "label14";
          this.label14.Size = new System.Drawing.Size(212, 16);
          this.label14.TabIndex = 7;
          this.label14.Text = "Create a new archive database for each ";
          // 
          // groupBox1
          // 
          this.groupBox1.Controls.Add(this._comboSkipIntegrity);
          this.groupBox1.Controls.Add(this._lblSkipIntegrity);
          this.groupBox1.Controls.Add(this.label3);
          this.groupBox1.Controls.Add(this.comboTimeZone);
          this.groupBox1.Controls.Add(this.label2);
          this.groupBox1.Controls.Add(this.textAge);
          this.groupBox1.Controls.Add(this.label1);
          this.groupBox1.Controls.Add(this.label9);
          this.groupBox1.Location = new System.Drawing.Point(12, 40);
          this.groupBox1.Name = "groupBox1";
          this.groupBox1.Size = new System.Drawing.Size(520, 144);
          this.groupBox1.TabIndex = 14;
          this.groupBox1.TabStop = false;
          this.groupBox1.Text = "Archive Options";
          // 
          // _comboSkipIntegrity
          // 
          this._comboSkipIntegrity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
          this._comboSkipIntegrity.Items.AddRange(new object[] {
            "No",
            "Yes"});
          this._comboSkipIntegrity.Location = new System.Drawing.Point(132, 44);
          this._comboSkipIntegrity.Name = "_comboSkipIntegrity";
          this._comboSkipIntegrity.Size = new System.Drawing.Size(72, 21);
          this._comboSkipIntegrity.TabIndex = 18;
          // 
          // _lblSkipIntegrity
          // 
          this._lblSkipIntegrity.Location = new System.Drawing.Point(8, 47);
          this._lblSkipIntegrity.Name = "_lblSkipIntegrity";
          this._lblSkipIntegrity.Size = new System.Drawing.Size(116, 14);
          this._lblSkipIntegrity.TabIndex = 17;
          this._lblSkipIntegrity.Text = "Skip Integrity Check";
          // 
          // label3
          // 
          this.label3.Location = new System.Drawing.Point(132, 96);
          this.label3.Name = "label3";
          this.label3.Size = new System.Drawing.Size(344, 44);
          this.label3.TabIndex = 16;
          this.label3.Text = "Note: The Collection Server uses the specified time zone to define local midnight" +
              " time and create a new archive database for each audited SQL Server instance.";
          // 
          // comboTimeZone
          // 
          this.comboTimeZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
          this.comboTimeZone.Location = new System.Drawing.Point(132, 72);
          this.comboTimeZone.Name = "comboTimeZone";
          this.comboTimeZone.Size = new System.Drawing.Size(348, 21);
          this.comboTimeZone.Sorted = true;
          this.comboTimeZone.TabIndex = 15;
          // 
          // label2
          // 
          this.label2.Location = new System.Drawing.Point(8, 80);
          this.label2.Name = "label2";
          this.label2.Size = new System.Drawing.Size(128, 16);
          this.label2.TabIndex = 14;
          this.label2.Text = "Archive Time Zone:";
          // 
          // label1
          // 
          this.label1.Location = new System.Drawing.Point(8, 22);
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size(124, 14);
          this.label1.TabIndex = 7;
          this.label1.Text = "Move events older than";
          // 
          // label9
          // 
          this.label9.Location = new System.Drawing.Point(176, 22);
          this.label9.Name = "label9";
          this.label9.Size = new System.Drawing.Size(152, 16);
          this.label9.TabIndex = 6;
          this.label9.Text = "days to an archive database.";
          // 
          // label4
          // 
          this.label4.Location = new System.Drawing.Point(12, 8);
          this.label4.Name = "label4";
          this.label4.Size = new System.Drawing.Size(520, 32);
          this.label4.TabIndex = 15;
          this.label4.Text = "Archiving moves the collected audit data from the event database to an archive da" +
              "tabase for each registered SQL Server you audit.";
          // 
          // linkLblHelpBestPractices
          // 
          this.linkLblHelpBestPractices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
          this.linkLblHelpBestPractices.AutoSize = true;
          this.linkLblHelpBestPractices.LinkArea = new System.Windows.Forms.LinkArea(0, 99);
          this.linkLblHelpBestPractices.Location = new System.Drawing.Point(125, 40);
          this.linkLblHelpBestPractices.Name = "linkLblHelpBestPractices";
          this.linkLblHelpBestPractices.Size = new System.Drawing.Size(289, 17);
          this.linkLblHelpBestPractices.TabIndex = 39;
          this.linkLblHelpBestPractices.TabStop = true;
          this.linkLblHelpBestPractices.Text = "Learn how to optimize performance with archive settings.";
          this.linkLblHelpBestPractices.UseCompatibleTextRendering = true;
          this.linkLblHelpBestPractices.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLblHelpBestPractices_LinkClicked);
          // 
          // groupBox2
          // 
          this.groupBox2.Controls.Add(this.updnWeekRepetition);
          this.groupBox2.Controls.Add(this.label13);
          this.groupBox2.Controls.Add(this.dttmMonthlyScheduleTime);
          this.groupBox2.Controls.Add(this.pnlWeekDays);
          this.groupBox2.Controls.Add(this.pnlMonthlySchedule);
          this.groupBox2.Controls.Add(this.radMonthlySchedule);
          this.groupBox2.Controls.Add(this.label7);
          this.groupBox2.Controls.Add(this.dttmWeeklyScheduleTime);
          this.groupBox2.Controls.Add(this.label6);
          this.groupBox2.Controls.Add(this.radWeeklySchedule);
          this.groupBox2.Controls.Add(this.dttmDailyScheduleTime);
          this.groupBox2.Controls.Add(this.radDailySchedule);
          this.groupBox2.Controls.Add(this.radNoSchedule);
          this.groupBox2.Location = new System.Drawing.Point(12, 190);
          this.groupBox2.Name = "groupBox2";
          this.groupBox2.Size = new System.Drawing.Size(520, 204);
          this.groupBox2.TabIndex = 40;
          this.groupBox2.TabStop = false;
          this.groupBox2.Text = "Archive Schedule";
          // 
          // updnWeekRepetition
          // 
          this.updnWeekRepetition.Location = new System.Drawing.Point(65, 72);
          this.updnWeekRepetition.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
          this.updnWeekRepetition.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
          this.updnWeekRepetition.Name = "updnWeekRepetition";
          this.updnWeekRepetition.Size = new System.Drawing.Size(35, 20);
          this.updnWeekRepetition.TabIndex = 25;
          this.updnWeekRepetition.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
          // 
          // label13
          // 
          this.label13.AutoSize = true;
          this.label13.Location = new System.Drawing.Point(30, 182);
          this.label13.Name = "label13";
          this.label13.Size = new System.Drawing.Size(16, 13);
          this.label13.TabIndex = 24;
          this.label13.Text = "at";
          // 
          // dttmMonthlyScheduleTime
          // 
          this.dttmMonthlyScheduleTime.CustomFormat = "hh:mm:ss tt";
          this.dttmMonthlyScheduleTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
          this.dttmMonthlyScheduleTime.Location = new System.Drawing.Point(48, 179);
          this.dttmMonthlyScheduleTime.Name = "dttmMonthlyScheduleTime";
          this.dttmMonthlyScheduleTime.ShowUpDown = true;
          this.dttmMonthlyScheduleTime.Size = new System.Drawing.Size(98, 20);
          this.dttmMonthlyScheduleTime.TabIndex = 23;
          // 
          // pnlWeekDays
          // 
          this.pnlWeekDays.Controls.Add(this.chkSaturday);
          this.pnlWeekDays.Controls.Add(this.chkFriday);
          this.pnlWeekDays.Controls.Add(this.chkThursday);
          this.pnlWeekDays.Controls.Add(this.chkWednesday);
          this.pnlWeekDays.Controls.Add(this.chkTuesday);
          this.pnlWeekDays.Controls.Add(this.chkMonday);
          this.pnlWeekDays.Controls.Add(this.chkSunday);
          this.pnlWeekDays.Location = new System.Drawing.Point(172, 68);
          this.pnlWeekDays.Name = "pnlWeekDays";
          this.pnlWeekDays.Size = new System.Drawing.Size(342, 27);
          this.pnlWeekDays.TabIndex = 22;
          // 
          // chkSaturday
          // 
          this.chkSaturday.AutoSize = true;
          this.chkSaturday.Location = new System.Drawing.Point(299, 6);
          this.chkSaturday.Name = "chkSaturday";
          this.chkSaturday.Size = new System.Drawing.Size(42, 17);
          this.chkSaturday.TabIndex = 24;
          this.chkSaturday.Text = "Sat";
          this.chkSaturday.UseVisualStyleBackColor = true;
          // 
          // chkFriday
          // 
          this.chkFriday.AutoSize = true;
          this.chkFriday.Location = new System.Drawing.Point(256, 6);
          this.chkFriday.Name = "chkFriday";
          this.chkFriday.Size = new System.Drawing.Size(37, 17);
          this.chkFriday.TabIndex = 23;
          this.chkFriday.Text = "Fri";
          this.chkFriday.UseVisualStyleBackColor = true;
          // 
          // chkThursday
          // 
          this.chkThursday.AutoSize = true;
          this.chkThursday.Location = new System.Drawing.Point(205, 6);
          this.chkThursday.Name = "chkThursday";
          this.chkThursday.Size = new System.Drawing.Size(45, 17);
          this.chkThursday.TabIndex = 22;
          this.chkThursday.Text = "Thu";
          this.chkThursday.UseVisualStyleBackColor = true;
          // 
          // chkWednesday
          // 
          this.chkWednesday.AutoSize = true;
          this.chkWednesday.Location = new System.Drawing.Point(153, 6);
          this.chkWednesday.Name = "chkWednesday";
          this.chkWednesday.Size = new System.Drawing.Size(49, 17);
          this.chkWednesday.TabIndex = 21;
          this.chkWednesday.Text = "Wed";
          this.chkWednesday.UseVisualStyleBackColor = true;
          // 
          // chkTuesday
          // 
          this.chkTuesday.AutoSize = true;
          this.chkTuesday.Location = new System.Drawing.Point(102, 6);
          this.chkTuesday.Name = "chkTuesday";
          this.chkTuesday.Size = new System.Drawing.Size(45, 17);
          this.chkTuesday.TabIndex = 20;
          this.chkTuesday.Text = "Tue";
          this.chkTuesday.UseVisualStyleBackColor = true;
          // 
          // chkMonday
          // 
          this.chkMonday.AutoSize = true;
          this.chkMonday.Location = new System.Drawing.Point(50, 6);
          this.chkMonday.Name = "chkMonday";
          this.chkMonday.Size = new System.Drawing.Size(47, 17);
          this.chkMonday.TabIndex = 19;
          this.chkMonday.Text = "Mon";
          this.chkMonday.UseVisualStyleBackColor = true;
          // 
          // chkSunday
          // 
          this.chkSunday.AutoSize = true;
          this.chkSunday.Location = new System.Drawing.Point(4, 6);
          this.chkSunday.Name = "chkSunday";
          this.chkSunday.Size = new System.Drawing.Size(45, 17);
          this.chkSunday.TabIndex = 18;
          this.chkSunday.Text = "Sun";
          this.chkSunday.UseVisualStyleBackColor = true;
          // 
          // pnlMonthlySchedule
          // 
          this.pnlMonthlySchedule.Controls.Add(this.updnMonthRepetition2);
          this.pnlMonthlySchedule.Controls.Add(this.updnMonthRepetition1);
          this.pnlMonthlySchedule.Controls.Add(this.updnMonthDay);
          this.pnlMonthlySchedule.Controls.Add(this.label12);
          this.pnlMonthlySchedule.Controls.Add(this.label11);
          this.pnlMonthlySchedule.Controls.Add(this.label10);
          this.pnlMonthlySchedule.Controls.Add(this.label8);
          this.pnlMonthlySchedule.Controls.Add(this.cboWeekday);
          this.pnlMonthlySchedule.Controls.Add(this.cboMonthWeek);
          this.pnlMonthlySchedule.Controls.Add(this.radSchedule_WeekdayWise);
          this.pnlMonthlySchedule.Controls.Add(this.radSchedule_DateWise);
          this.pnlMonthlySchedule.Location = new System.Drawing.Point(72, 124);
          this.pnlMonthlySchedule.Name = "pnlMonthlySchedule";
          this.pnlMonthlySchedule.Size = new System.Drawing.Size(439, 56);
          this.pnlMonthlySchedule.TabIndex = 21;
          // 
          // updnMonthRepetition2
          // 
          this.updnMonthRepetition2.Location = new System.Drawing.Point(313, 29);
          this.updnMonthRepetition2.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
          this.updnMonthRepetition2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
          this.updnMonthRepetition2.Name = "updnMonthRepetition2";
          this.updnMonthRepetition2.Size = new System.Drawing.Size(35, 20);
          this.updnMonthRepetition2.TabIndex = 34;
          this.updnMonthRepetition2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
          // 
          // updnMonthRepetition1
          // 
          this.updnMonthRepetition1.Location = new System.Drawing.Point(143, 3);
          this.updnMonthRepetition1.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
          this.updnMonthRepetition1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
          this.updnMonthRepetition1.Name = "updnMonthRepetition1";
          this.updnMonthRepetition1.Size = new System.Drawing.Size(35, 20);
          this.updnMonthRepetition1.TabIndex = 33;
          this.updnMonthRepetition1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
          // 
          // updnMonthDay
          // 
          this.updnMonthDay.Location = new System.Drawing.Point(53, 2);
          this.updnMonthDay.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
          this.updnMonthDay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
          this.updnMonthDay.Name = "updnMonthDay";
          this.updnMonthDay.Size = new System.Drawing.Size(35, 20);
          this.updnMonthDay.TabIndex = 32;
          this.updnMonthDay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
          // 
          // label12
          // 
          this.label12.AutoSize = true;
          this.label12.Location = new System.Drawing.Point(184, 5);
          this.label12.Name = "label12";
          this.label12.Size = new System.Drawing.Size(47, 13);
          this.label12.TabIndex = 31;
          this.label12.Text = "month(s)";
          // 
          // label11
          // 
          this.label11.AutoSize = true;
          this.label11.Location = new System.Drawing.Point(95, 5);
          this.label11.Name = "label11";
          this.label11.Size = new System.Drawing.Size(45, 13);
          this.label11.TabIndex = 29;
          this.label11.Text = "of every";
          // 
          // label10
          // 
          this.label10.AutoSize = true;
          this.label10.Location = new System.Drawing.Point(354, 31);
          this.label10.Name = "label10";
          this.label10.Size = new System.Drawing.Size(47, 13);
          this.label10.TabIndex = 28;
          this.label10.Text = "month(s)";
          // 
          // label8
          // 
          this.label8.AutoSize = true;
          this.label8.Location = new System.Drawing.Point(262, 31);
          this.label8.Name = "label8";
          this.label8.Size = new System.Drawing.Size(45, 13);
          this.label8.TabIndex = 26;
          this.label8.Text = "of every";
          // 
          // cboWeekday
          // 
          this.cboWeekday.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
          this.cboWeekday.Items.AddRange(new object[] {
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday"});
          this.cboWeekday.Location = new System.Drawing.Point(131, 28);
          this.cboWeekday.Name = "cboWeekday";
          this.cboWeekday.Size = new System.Drawing.Size(125, 21);
          this.cboWeekday.TabIndex = 25;
          // 
          // cboMonthWeek
          // 
          this.cboMonthWeek.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
          this.cboMonthWeek.Items.AddRange(new object[] {
            "First",
            "Second",
            "Third",
            "Fourth",
            "Fifth"});
          this.cboMonthWeek.Location = new System.Drawing.Point(53, 28);
          this.cboMonthWeek.Name = "cboMonthWeek";
          this.cboMonthWeek.Size = new System.Drawing.Size(72, 21);
          this.cboMonthWeek.TabIndex = 24;
          // 
          // radSchedule_WeekdayWise
          // 
          this.radSchedule_WeekdayWise.AutoSize = true;
          this.radSchedule_WeekdayWise.Location = new System.Drawing.Point(3, 29);
          this.radSchedule_WeekdayWise.Name = "radSchedule_WeekdayWise";
          this.radSchedule_WeekdayWise.Size = new System.Drawing.Size(44, 17);
          this.radSchedule_WeekdayWise.TabIndex = 23;
          this.radSchedule_WeekdayWise.Text = "The";
          this.radSchedule_WeekdayWise.UseVisualStyleBackColor = true;
          this.radSchedule_WeekdayWise.CheckedChanged += new System.EventHandler(this.MonthlySchedule_CheckChanged);
          // 
          // radSchedule_DateWise
          // 
          this.radSchedule_DateWise.AutoSize = true;
          this.radSchedule_DateWise.Location = new System.Drawing.Point(3, 3);
          this.radSchedule_DateWise.Name = "radSchedule_DateWise";
          this.radSchedule_DateWise.Size = new System.Drawing.Size(44, 17);
          this.radSchedule_DateWise.TabIndex = 21;
          this.radSchedule_DateWise.Text = "Day";
          this.radSchedule_DateWise.UseVisualStyleBackColor = true;
          this.radSchedule_DateWise.CheckedChanged += new System.EventHandler(this.MonthlySchedule_CheckChanged);
          // 
          // radMonthlySchedule
          // 
          this.radMonthlySchedule.AutoSize = true;
          this.radMonthlySchedule.Location = new System.Drawing.Point(11, 127);
          this.radMonthlySchedule.Name = "radMonthlySchedule";
          this.radMonthlySchedule.Size = new System.Drawing.Size(65, 17);
          this.radMonthlySchedule.TabIndex = 20;
          this.radMonthlySchedule.TabStop = true;
          this.radMonthlySchedule.Text = "Monthly:";
          this.radMonthlySchedule.UseVisualStyleBackColor = true;
          this.radMonthlySchedule.CheckedChanged += new System.EventHandler(this.Schedule_CheckedChanged);
          // 
          // label7
          // 
          this.label7.AutoSize = true;
          this.label7.Location = new System.Drawing.Point(30, 101);
          this.label7.Name = "label7";
          this.label7.Size = new System.Drawing.Size(16, 13);
          this.label7.TabIndex = 19;
          this.label7.Text = "at";
          // 
          // dttmWeeklyScheduleTime
          // 
          this.dttmWeeklyScheduleTime.CustomFormat = "hh:mm:ss tt";
          this.dttmWeeklyScheduleTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
          this.dttmWeeklyScheduleTime.Location = new System.Drawing.Point(48, 98);
          this.dttmWeeklyScheduleTime.Name = "dttmWeeklyScheduleTime";
          this.dttmWeeklyScheduleTime.ShowUpDown = true;
          this.dttmWeeklyScheduleTime.Size = new System.Drawing.Size(98, 20);
          this.dttmWeeklyScheduleTime.TabIndex = 18;
          // 
          // label6
          // 
          this.label6.AutoSize = true;
          this.label6.Location = new System.Drawing.Point(106, 75);
          this.label6.Name = "label6";
          this.label6.Size = new System.Drawing.Size(62, 13);
          this.label6.TabIndex = 10;
          this.label6.Text = "week(s) on:";
          // 
          // radWeeklySchedule
          // 
          this.radWeeklySchedule.AutoSize = true;
          this.radWeeklySchedule.Location = new System.Drawing.Point(11, 73);
          this.radWeeklySchedule.Name = "radWeeklySchedule";
          this.radWeeklySchedule.Size = new System.Drawing.Size(55, 17);
          this.radWeeklySchedule.TabIndex = 3;
          this.radWeeklySchedule.TabStop = true;
          this.radWeeklySchedule.Text = "Every ";
          this.radWeeklySchedule.UseVisualStyleBackColor = true;
          this.radWeeklySchedule.CheckedChanged += new System.EventHandler(this.Schedule_CheckedChanged);
          // 
          // dttmDailyScheduleTime
          // 
          this.dttmDailyScheduleTime.CustomFormat = "hh:mm:ss tt";
          this.dttmDailyScheduleTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
          this.dttmDailyScheduleTime.Location = new System.Drawing.Point(106, 46);
          this.dttmDailyScheduleTime.Name = "dttmDailyScheduleTime";
          this.dttmDailyScheduleTime.ShowUpDown = true;
          this.dttmDailyScheduleTime.Size = new System.Drawing.Size(98, 20);
          this.dttmDailyScheduleTime.TabIndex = 2;
          // 
          // radDailySchedule
          // 
          this.radDailySchedule.AutoSize = true;
          this.radDailySchedule.Location = new System.Drawing.Point(11, 46);
          this.radDailySchedule.Name = "radDailySchedule";
          this.radDailySchedule.Size = new System.Drawing.Size(89, 17);
          this.radDailySchedule.TabIndex = 1;
          this.radDailySchedule.TabStop = true;
          this.radDailySchedule.Text = "Once Daily at";
          this.radDailySchedule.UseVisualStyleBackColor = true;
          this.radDailySchedule.CheckedChanged += new System.EventHandler(this.Schedule_CheckedChanged);
          // 
          // radNoSchedule
          // 
          this.radNoSchedule.AutoSize = true;
          this.radNoSchedule.Location = new System.Drawing.Point(11, 19);
          this.radNoSchedule.Name = "radNoSchedule";
          this.radNoSchedule.Size = new System.Drawing.Size(87, 17);
          this.radNoSchedule.TabIndex = 0;
          this.radNoSchedule.TabStop = true;
          this.radNoSchedule.Text = "No Schedule";
          this.radNoSchedule.UseVisualStyleBackColor = true;
          this.radNoSchedule.CheckedChanged += new System.EventHandler(this.Schedule_CheckedChanged);
          // 
          // textAge
          // 
          this.textAge.Location = new System.Drawing.Point(132, 16);
          this.textAge.MaxLength = 3;
          this.textAge.Name = "textAge";
          this.textAge.Size = new System.Drawing.Size(36, 20);
          this.textAge.TabIndex = 8;
          this.textAge.Text = " ";
          // 
          // Form_ArchiveOptions
          // 
          this.AcceptButton = this.btnOK;
          this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
          this.CancelButton = this.btnCancel;
          this.ClientSize = new System.Drawing.Size(539, 559);
          this.Controls.Add(this.linkLblHelpBestPractices);
          this.Controls.Add(this.groupBox2);
          this.Controls.Add(this.groupBox1);
          this.Controls.Add(this.groupBox5);
          this.Controls.Add(this.btnCancel);
          this.Controls.Add(this.btnOK);
          this.Controls.Add(this.label4);
          this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
          this.HelpButton = true;
          this.MaximizeBox = false;
          this.MinimizeBox = false;
          this.Name = "Form_ArchiveOptions";
          this.ShowInTaskbar = false;
          this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
          this.Text = "Archive Preferences";
          this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_Defaults_HelpRequested);
          this.groupBox5.ResumeLayout(false);
          this.groupBox5.PerformLayout();
          this.groupBox1.ResumeLayout(false);
          this.groupBox1.PerformLayout();
          this.groupBox2.ResumeLayout(false);
          this.groupBox2.PerformLayout();
          ((System.ComponentModel.ISupportInitialize)(this.updnWeekRepetition)).EndInit();
          this.pnlWeekDays.ResumeLayout(false);
          this.pnlWeekDays.PerformLayout();
          this.pnlMonthlySchedule.ResumeLayout(false);
          this.pnlMonthlySchedule.PerformLayout();
          ((System.ComponentModel.ISupportInitialize)(this.updnMonthRepetition2)).EndInit();
          ((System.ComponentModel.ISupportInitialize)(this.updnMonthRepetition1)).EndInit();
          ((System.ComponentModel.ISupportInitialize)(this.updnMonthDay)).EndInit();
          this.ResumeLayout(false);
          this.PerformLayout();

      }
      #endregion

      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.GroupBox groupBox5;
      private System.Windows.Forms.Label label16;
      private System.Windows.Forms.TextBox textPrefix;
      private System.Windows.Forms.Label label15;
      private System.Windows.Forms.Label label14;
      private System.Windows.Forms.TextBox textBox1;
      private System.Windows.Forms.ComboBox comboPeriod;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.Label label1;
      private Idera.SQLcompliance.Application.GUI.Controls.NumericTextBox textAge;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.ComboBox comboTimeZone;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label _lblSkipIntegrity;
      private System.Windows.Forms.ComboBox _comboSkipIntegrity;
      private System.Windows.Forms.LinkLabel linkLblHelpBestPractices;
       private System.Windows.Forms.Button btnBrowseArchiveDBFilesDirectory;
       private System.Windows.Forms.TextBox txtArchiveDBFilesDirectory;
       private System.Windows.Forms.Label label5;
       private System.Windows.Forms.GroupBox groupBox2;
       private System.Windows.Forms.DateTimePicker dttmDailyScheduleTime;
       private System.Windows.Forms.RadioButton radDailySchedule;
       private System.Windows.Forms.RadioButton radNoSchedule;
       private System.Windows.Forms.RadioButton radWeeklySchedule;
       private System.Windows.Forms.Label label6;
       private System.Windows.Forms.Panel pnlMonthlySchedule;
       private System.Windows.Forms.RadioButton radMonthlySchedule;
       private System.Windows.Forms.Label label7;
       private System.Windows.Forms.DateTimePicker dttmWeeklyScheduleTime;
       private System.Windows.Forms.Panel pnlWeekDays;
       private System.Windows.Forms.CheckBox chkSaturday;
       private System.Windows.Forms.CheckBox chkFriday;
       private System.Windows.Forms.CheckBox chkThursday;
       private System.Windows.Forms.CheckBox chkWednesday;
       private System.Windows.Forms.CheckBox chkTuesday;
       private System.Windows.Forms.CheckBox chkMonday;
       private System.Windows.Forms.CheckBox chkSunday;
       private System.Windows.Forms.ComboBox cboMonthWeek;
       private System.Windows.Forms.RadioButton radSchedule_WeekdayWise;
       private System.Windows.Forms.RadioButton radSchedule_DateWise;
       private System.Windows.Forms.Label label8;
       private System.Windows.Forms.ComboBox cboWeekday;
       private System.Windows.Forms.Label label10;
       private System.Windows.Forms.Label label12;
       private System.Windows.Forms.Label label11;
       private System.Windows.Forms.Label label13;
       private System.Windows.Forms.DateTimePicker dttmMonthlyScheduleTime;
       private System.Windows.Forms.NumericUpDown updnWeekRepetition;
       private System.Windows.Forms.NumericUpDown updnMonthDay;
       private System.Windows.Forms.NumericUpDown updnMonthRepetition2;
       private System.Windows.Forms.NumericUpDown updnMonthRepetition1;
   }
}