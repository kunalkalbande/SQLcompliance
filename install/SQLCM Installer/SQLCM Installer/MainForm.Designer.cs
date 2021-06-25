using SQLCM_Installer.Custom_Controls;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
namespace SQLCM_Installer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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

        private void panelHeaderPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void pictureCloseIcon_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void pictureCloseIcon_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(MainForm));
            this.panelHeaderPanel = new Panel();
            this.labelHeaderImage = new IderaTitleLabel();
            this.pictureCloseIcon = new PictureBox();
            this.pictureHeaderImage = new PictureBox();
            this.panelSetupTypeSidePanel = new Panel();
            this.panelMainSidePanel = new Panel();
            this.labelLeftNavFirst = new IderaHeaderLabel();
            this.labelLeftNavSecond = new IderaHeaderLabel();
            this.labelLeftNavThird = new IderaHeaderLabel();
            this.labelLeftNavFourth = new IderaHeaderLabel();
            this.labelLeftNavFifth = new IderaHeaderLabel();
            this.labelLeftNavSixth = new IderaHeaderLabel();
            this.labelLeftNavSeventh = new IderaHeaderLabel();
            this.labelLeftNavEighth = new IderaHeaderLabel();
            this.labelLeftNavNineth = new IderaHeaderLabel();
            this.labelLeftNavTenth = new IderaHeaderLabel();
            this.pictureOne = new PictureBox();
            this.pictureTwo = new PictureBox();
            this.pictureThree = new PictureBox();
            this.pictureFour = new PictureBox();
            this.pictureFive = new PictureBox();
            this.pictureSix = new PictureBox();
            this.pictureSeven = new PictureBox();
            this.pictureEight = new PictureBox();
            this.pictureNine = new PictureBox();
            this.pictureTen = new PictureBox();
            this.panelSidePanel = new Panel();
            this.pictureLogo = new PictureBox();
            this.wizardPanel = new Panel();
            this.panelBottomPanel = new Panel();
            this.testConnectionsButton = new IderaButton();
            this.labelInstallationhelp = new IderaLabel();
            this.pictureInstallationHelp = new PictureBox();
            this.backButton = new IderaButton();
            this.cancelButton = new IderaButton();
            this.nextButton = new IderaButton();
            this.panelHeaderPanel.SuspendLayout();
            ((ISupportInitialize)(this.pictureCloseIcon)).BeginInit();
            ((ISupportInitialize)(this.pictureHeaderImage)).BeginInit();
            this.panelSetupTypeSidePanel.SuspendLayout();
            this.panelMainSidePanel.SuspendLayout();
            ((ISupportInitialize)(this.pictureTen)).BeginInit();
            ((ISupportInitialize)(this.pictureNine)).BeginInit();
            ((ISupportInitialize)(this.pictureSeven)).BeginInit();
            ((ISupportInitialize)(this.pictureEight)).BeginInit();
            ((ISupportInitialize)(this.pictureOne)).BeginInit();
            ((ISupportInitialize)(this.pictureSix)).BeginInit();
            ((ISupportInitialize)(this.pictureTwo)).BeginInit();
            ((ISupportInitialize)(this.pictureFive)).BeginInit();
            ((ISupportInitialize)(this.pictureThree)).BeginInit();
            ((ISupportInitialize)(this.pictureFour)).BeginInit();
            this.panelSidePanel.SuspendLayout();
            ((ISupportInitialize)(this.pictureLogo)).BeginInit();
            this.panelBottomPanel.SuspendLayout();
            ((ISupportInitialize)(this.pictureInstallationHelp)).BeginInit();
            this.SuspendLayout();
            // 
            // panelHeaderPanel
            // 
            this.panelHeaderPanel.BackColor = Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(175)))), ((int)(((byte)(167)))));
            this.panelHeaderPanel.Controls.Add(this.labelHeaderImage);
            this.panelHeaderPanel.Controls.Add(this.pictureCloseIcon);
            this.panelHeaderPanel.Controls.Add(this.pictureHeaderImage);
            this.panelHeaderPanel.Location = new Point(0, 0);
            this.panelHeaderPanel.Name = "panelHeaderPanel";
            this.panelHeaderPanel.Size = new Size(750, 54);
            this.panelHeaderPanel.TabIndex = 8;
            this.panelHeaderPanel.MouseDown += new MouseEventHandler(this.panelHeaderPanel_MouseDown);
            // 
            // labelHeaderImage
            // 
            this.labelHeaderImage.AutoSize = true;
            this.labelHeaderImage.BackColor = Color.Transparent;
            this.labelHeaderImage.ForeColor = Color.White;
            this.labelHeaderImage.Location = new Point(46, 13);
            this.labelHeaderImage.Name = "labelHeaderImage";
            this.labelHeaderImage.Size = new Size(233, 13);
            this.labelHeaderImage.TabIndex = 2;
            this.labelHeaderImage.Text = "IDERA SQL Compliance Manager v 5.9 Installer";
            this.labelHeaderImage.MouseDown += new MouseEventHandler(this.panelHeaderPanel_MouseDown);
            // 
            // pictureCloseIcon
            // 
            this.pictureCloseIcon.Image = global::SQLCM_Installer.Properties.Resources.close_icon;
            this.pictureCloseIcon.Location = new Point(720, 19);
            this.pictureCloseIcon.Name = "pictureCloseIcon";
            this.pictureCloseIcon.Size = new Size(16, 16);
            this.pictureCloseIcon.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureCloseIcon.TabIndex = 0;
            this.pictureCloseIcon.TabStop = false;
            this.pictureCloseIcon.Click += new EventHandler(this.pictureCloseIcon_Click);
            this.pictureCloseIcon.MouseEnter += new EventHandler(this.pictureCloseIcon_MouseEnter);
            this.pictureCloseIcon.MouseLeave += new EventHandler(this.pictureCloseIcon_MouseLeave);
            // 
            // pictureHeaderImage
            // 
            this.pictureHeaderImage.Image = global::SQLCM_Installer.Properties.Resources.headericon;
            this.pictureHeaderImage.Location = new Point(20, 18);
            this.pictureHeaderImage.Name = "pictureHeaderImage";
            this.pictureHeaderImage.Size = new Size(16, 16);
            this.pictureHeaderImage.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureHeaderImage.TabIndex = 1;
            this.pictureHeaderImage.TabStop = false;
            this.pictureHeaderImage.MouseDown += new MouseEventHandler(this.panelHeaderPanel_MouseDown);
            // 
            // panelSetupTypeSidePanel
            // 
            this.panelSetupTypeSidePanel.BackColor = Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(238)))), ((int)(((byte)(239)))));
            this.panelSetupTypeSidePanel.Controls.Add(this.panelMainSidePanel);
            this.panelSetupTypeSidePanel.Location = new Point(0, 0);
            this.panelSetupTypeSidePanel.Name = "panelSetupTypeSidePanel";
            this.panelSetupTypeSidePanel.Size = new Size(200, 486);
            this.panelSetupTypeSidePanel.TabIndex = 1;
            this.panelSetupTypeSidePanel.Visible = false;
            // 
            // panelMainSidePanel
            // 
            this.panelMainSidePanel.BackColor = Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(238)))), ((int)(((byte)(239)))));
            this.panelMainSidePanel.Controls.Add(this.labelLeftNavTenth);
            this.panelMainSidePanel.Controls.Add(this.labelLeftNavNineth);
            this.panelMainSidePanel.Controls.Add(this.pictureTen);
            this.panelMainSidePanel.Controls.Add(this.pictureNine);
            this.panelMainSidePanel.Controls.Add(this.labelLeftNavSeventh);
            this.panelMainSidePanel.Controls.Add(this.labelLeftNavEighth);
            this.panelMainSidePanel.Controls.Add(this.pictureSeven);
            this.panelMainSidePanel.Controls.Add(this.pictureEight);
            this.panelMainSidePanel.Controls.Add(this.labelLeftNavSecond);
            this.panelMainSidePanel.Controls.Add(this.labelLeftNavThird);
            this.panelMainSidePanel.Controls.Add(this.labelLeftNavFourth);
            this.panelMainSidePanel.Controls.Add(this.labelLeftNavFifth);
            this.panelMainSidePanel.Controls.Add(this.labelLeftNavSixth);
            this.panelMainSidePanel.Controls.Add(this.labelLeftNavFirst);
            this.panelMainSidePanel.Controls.Add(this.pictureOne);
            this.panelMainSidePanel.Controls.Add(this.pictureSix);
            this.panelMainSidePanel.Controls.Add(this.pictureTwo);
            this.panelMainSidePanel.Controls.Add(this.pictureFive);
            this.panelMainSidePanel.Controls.Add(this.pictureThree);
            this.panelMainSidePanel.Controls.Add(this.pictureFour);
            this.panelMainSidePanel.Location = new Point(0, 0);
            this.panelMainSidePanel.Name = "panelMainSidePanel";
            this.panelMainSidePanel.Size = new Size(200, 486);
            this.panelMainSidePanel.TabIndex = 0;
            this.panelMainSidePanel.Visible = false;
            // 
            // ideraHeaderLabel2
            // 
            this.labelLeftNavTenth.AutoSize = true;
            this.labelLeftNavTenth.BackColor = Color.Transparent;
            this.labelLeftNavTenth.Font = new Font("Source Sans Pro Semibold", 10.5F);
            this.labelLeftNavTenth.ForeColor = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.labelLeftNavTenth.Location = new Point(50, 381);
            this.labelLeftNavTenth.Name = "10";
            this.labelLeftNavTenth.Size = new Size(43, 18);
            this.labelLeftNavTenth.TabIndex = 28;
            this.labelLeftNavTenth.Text = "Tenth";
            // 
            // ideraHeaderLabel1
            // 
            this.labelLeftNavNineth.AutoSize = true;
            this.labelLeftNavNineth.BackColor = Color.Transparent;
            this.labelLeftNavNineth.Font = new Font("Source Sans Pro Semibold", 10.5F);
            this.labelLeftNavNineth.ForeColor = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.labelLeftNavNineth.Location = new Point(50, 341);
            this.labelLeftNavNineth.Name = "9";
            this.labelLeftNavNineth.Size = new Size(49, 18);
            this.labelLeftNavNineth.TabIndex = 27;
            this.labelLeftNavNineth.Text = "Nineth";
            // 
            // labelLeftNavSeventh
            // 
            this.labelLeftNavSeventh.AutoSize = true;
            this.labelLeftNavSeventh.BackColor = Color.Transparent;
            this.labelLeftNavSeventh.Font = new Font("Source Sans Pro Semibold", 10.5F);
            this.labelLeftNavSeventh.ForeColor = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.labelLeftNavSeventh.Location = new Point(50, 261);
            this.labelLeftNavSeventh.Name = "7";
            this.labelLeftNavSeventh.Size = new Size(58, 18);
            this.labelLeftNavSeventh.TabIndex = 24;
            this.labelLeftNavSeventh.Text = "Seventh";
            // 
            // labelLeftNavEighth
            // 
            this.labelLeftNavEighth.AutoSize = true;
            this.labelLeftNavEighth.BackColor = Color.Transparent;
            this.labelLeftNavEighth.Font = new Font("Source Sans Pro Semibold", 10.5F);
            this.labelLeftNavEighth.ForeColor = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.labelLeftNavEighth.Location = new Point(50, 301);
            this.labelLeftNavEighth.Name = "8";
            this.labelLeftNavEighth.Size = new Size(47, 18);
            this.labelLeftNavEighth.TabIndex = 23;
            this.labelLeftNavEighth.Text = "Eighth";
            // 
            // labelLeftNavSecond
            // 
            this.labelLeftNavSecond.AutoSize = true;
            this.labelLeftNavSecond.BackColor = Color.Transparent;
            this.labelLeftNavSecond.Font = new Font("Source Sans Pro Semibold", 10.5F);
            this.labelLeftNavSecond.ForeColor = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.labelLeftNavSecond.Location = new Point(50, 61);
            this.labelLeftNavSecond.Name = "2";
            this.labelLeftNavSecond.Size = new Size(53, 18);
            this.labelLeftNavSecond.TabIndex = 19;
            this.labelLeftNavSecond.Text = "Second";
            // 
            // labelLeftNavThird
            // 
            this.labelLeftNavThird.AutoSize = true;
            this.labelLeftNavThird.BackColor = Color.Transparent;
            this.labelLeftNavThird.Font = new Font("Source Sans Pro Semibold", 10.5F);
            this.labelLeftNavThird.ForeColor = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.labelLeftNavThird.Location = new Point(50, 101);
            this.labelLeftNavThird.Name = "3";
            this.labelLeftNavThird.Size = new Size(41, 18);
            this.labelLeftNavThird.TabIndex = 18;
            this.labelLeftNavThird.Text = "Third";
            // 
            // labelLeftNavFourth
            // 
            this.labelLeftNavFourth.AutoSize = true;
            this.labelLeftNavFourth.BackColor = Color.Transparent;
            this.labelLeftNavFourth.Font = new Font("Source Sans Pro Semibold", 10.5F);
            this.labelLeftNavFourth.ForeColor = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.labelLeftNavFourth.Location = new Point(50, 141);
            this.labelLeftNavFourth.Name = "4";
            this.labelLeftNavFourth.Size = new Size(49, 18);
            this.labelLeftNavFourth.TabIndex = 20;
            this.labelLeftNavFourth.Text = "Fourth";
            // 
            // labelLeftNavFifth
            // 
            this.labelLeftNavFifth.AutoSize = true;
            this.labelLeftNavFifth.BackColor = Color.Transparent;
            this.labelLeftNavFifth.Font = new Font("Source Sans Pro Semibold", 10.5F);
            this.labelLeftNavFifth.ForeColor = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.labelLeftNavFifth.Location = new Point(50, 181);
            this.labelLeftNavFifth.Name = "5";
            this.labelLeftNavFifth.Size = new Size(36, 18);
            this.labelLeftNavFifth.TabIndex = 17;
            this.labelLeftNavFifth.Text = "Fifth";
            // 
            // labelLeftNavSixth
            // 
            this.labelLeftNavSixth.AutoSize = true;
            this.labelLeftNavSixth.BackColor = Color.Transparent;
            this.labelLeftNavSixth.Font = new Font("Source Sans Pro Semibold", 10.5F);
            this.labelLeftNavSixth.ForeColor = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.labelLeftNavSixth.Location = new Point(50, 221);
            this.labelLeftNavSixth.Name = "6";
            this.labelLeftNavSixth.Size = new Size(40, 18);
            this.labelLeftNavSixth.TabIndex = 16;
            this.labelLeftNavSixth.Text = "Sixth";
            // 
            // labelLeftNavFirst
            // 
            this.labelLeftNavFirst.AutoSize = true;
            this.labelLeftNavFirst.BackColor = Color.Transparent;
            this.labelLeftNavFirst.Font = new Font("Source Sans Pro Semibold", 10.5F);
            this.labelLeftNavFirst.ForeColor = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(219)))));
            this.labelLeftNavFirst.Location = new Point(50, 21);
            this.labelLeftNavFirst.Name = "1";
            this.labelLeftNavFirst.Size = new Size(35, 18);
            this.labelLeftNavFirst.TabIndex = 0;
            this.labelLeftNavFirst.Text = "First";
            // 
            // pictureOne
            // 
            this.pictureOne.Image = global::SQLCM_Installer.Properties.Resources._1on;
            this.pictureOne.Location = new Point(20, 20);
            this.pictureOne.Name = "1";
            this.pictureOne.Size = new Size(20, 20);
            this.pictureOne.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureOne.TabIndex = 1;
            this.pictureOne.TabStop = false;
            // 
            // pictureSix
            // 
            this.pictureSix.Image = global::SQLCM_Installer.Properties.Resources._6on;
            this.pictureSix.Location = new Point(20, 220);
            this.pictureSix.Name = "6";
            this.pictureSix.Size = new Size(20, 20);
            this.pictureSix.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureSix.TabIndex = 7;
            this.pictureSix.TabStop = false;
            // 
            // pictureTwo
            // 
            this.pictureTwo.Image = global::SQLCM_Installer.Properties.Resources._2on;
            this.pictureTwo.Location = new Point(20, 60);
            this.pictureTwo.Name = "2";
            this.pictureTwo.Size = new Size(20, 20);
            this.pictureTwo.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureTwo.TabIndex = 2;
            this.pictureTwo.TabStop = false;
            // 
            // pictureFive
            // 
            this.pictureFive.Image = global::SQLCM_Installer.Properties.Resources._5on;
            this.pictureFive.Location = new Point(20, 180);
            this.pictureFive.Name = "5";
            this.pictureFive.Size = new Size(20, 20);
            this.pictureFive.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureFive.TabIndex = 6;
            this.pictureFive.TabStop = false;
            // 
            // pictureThree
            // 
            this.pictureThree.Image = global::SQLCM_Installer.Properties.Resources._3on;
            this.pictureThree.Location = new Point(20, 100);
            this.pictureThree.Name = "3";
            this.pictureThree.Size = new Size(20, 20);
            this.pictureThree.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureThree.TabIndex = 3;
            this.pictureThree.TabStop = false;
            // 
            // pictureFour
            // 
            this.pictureFour.Image = global::SQLCM_Installer.Properties.Resources._4on;
            this.pictureFour.Location = new Point(20, 140);
            this.pictureFour.Name = "4";
            this.pictureFour.Size = new Size(20, 20);
            this.pictureFour.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureFour.TabIndex = 4;
            this.pictureFour.TabStop = false;
            // 
            // pictureSeven
            // 
            this.pictureSeven.Image = global::SQLCM_Installer.Properties.Resources._7on;
            this.pictureSeven.Location = new Point(20, 261);
            this.pictureSeven.Name = "7";
            this.pictureSeven.Size = new Size(20, 20);
            this.pictureSeven.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureSeven.TabIndex = 22;
            this.pictureSeven.TabStop = false;
            // 
            // pictureEight
            // 
            this.pictureEight.Image = global::SQLCM_Installer.Properties.Resources._8on;
            this.pictureEight.Location = new Point(20, 301);
            this.pictureEight.Name = "8";
            this.pictureEight.Size = new Size(20, 20);
            this.pictureEight.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureEight.TabIndex = 21;
            this.pictureEight.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureTen.Image = global::SQLCM_Installer.Properties.Resources._10on;
            this.pictureTen.Location = new Point(20, 381);
            this.pictureTen.Name = "10";
            this.pictureTen.Size = new Size(20, 20);
            this.pictureTen.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureTen.TabIndex = 26;
            this.pictureTen.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureNine.Image = global::SQLCM_Installer.Properties.Resources._9on;
            this.pictureNine.Location = new Point(20, 341);
            this.pictureNine.Name = "9";
            this.pictureNine.Size = new Size(20, 20);
            this.pictureNine.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureNine.TabIndex = 25;
            this.pictureNine.TabStop = false;
            // 
            // panelSidePanel
            // 
            this.panelSidePanel.BackColor = Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(102)))), ((int)(((byte)(91)))));
            this.panelSidePanel.Controls.Add(this.panelSetupTypeSidePanel);
            this.panelSidePanel.Controls.Add(this.pictureLogo);
            this.panelSidePanel.Location = new Point(0, 54);
            this.panelSidePanel.Name = "panelSidePanel";
            this.panelSidePanel.Size = new Size(200, 486);
            this.panelSidePanel.TabIndex = 9;
            // 
            // pictureLogo
            // 
            this.pictureLogo.Image = global::SQLCM_Installer.Properties.Resources.idera_logo;
            this.pictureLogo.Location = new Point(27, 430);
            this.pictureLogo.Name = "pictureLogo";
            this.pictureLogo.Size = new Size(146, 16);
            this.pictureLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureLogo.TabIndex = 0;
            this.pictureLogo.TabStop = false;
            // 
            // wizardPanel
            // 
            this.wizardPanel.Location = new Point(200, 54);
            this.wizardPanel.Name = "wizardPanel";
            this.wizardPanel.Size = new Size(550, 486);
            this.wizardPanel.TabIndex = 11;
            // 
            // panelBottomPanel
            // 
            this.panelBottomPanel.BackColor = Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panelBottomPanel.Controls.Add(this.testConnectionsButton);
            this.panelBottomPanel.Controls.Add(this.labelInstallationhelp);
            this.panelBottomPanel.Controls.Add(this.pictureInstallationHelp);
            this.panelBottomPanel.Controls.Add(this.backButton);
            this.panelBottomPanel.Controls.Add(this.cancelButton);
            this.panelBottomPanel.Controls.Add(this.nextButton);
            this.panelBottomPanel.Location = new Point(0, 540);
            this.panelBottomPanel.Name = "panelBottomPanel";
            this.panelBottomPanel.Size = new Size(750, 60);
            this.panelBottomPanel.TabIndex = 7;
            // 
            // testConnectionsButton
            // 
            this.testConnectionsButton.BackColor = Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.testConnectionsButton.BorderColor = Color.Transparent;
            this.testConnectionsButton.BorderWidth = 2;
            this.testConnectionsButton.ButtonText = "";
            this.testConnectionsButton.Disabled = false;
            this.testConnectionsButton.EndColor = Color.Yellow;
            this.testConnectionsButton.FlatAppearance.BorderSize = 0;
            this.testConnectionsButton.FlatStyle = FlatStyle.Flat;
            this.testConnectionsButton.Font = new Font("Source Sans Pro", 10.5F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.testConnectionsButton.ForeColor = Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.testConnectionsButton.GradientAngle = 90;
            this.testConnectionsButton.Location = new Point(219, 16);
            this.testConnectionsButton.Name = "testConnectionsButton";
            this.testConnectionsButton.ShowButtontext = true;
            this.testConnectionsButton.Size = new Size(141, 28);
            this.testConnectionsButton.StartColor = Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.testConnectionsButton.TabIndex = 5;
            this.testConnectionsButton.Text = "Test Connections";
            this.testConnectionsButton.TextLocation_X = 100;
            this.testConnectionsButton.TextLocation_Y = 25;
            this.testConnectionsButton.UseVisualStyleBackColor = false;
            this.testConnectionsButton.Visible = false;
            this.testConnectionsButton.Click += new EventHandler(this.testConnectionsButton_Click);
            // 
            // labelInstallationhelp
            // 
            this.labelInstallationhelp.AutoSize = true;
            this.labelInstallationhelp.BackColor = Color.Transparent;
            this.labelInstallationhelp.Font = new Font("Source Sans Pro", 10.5F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.labelInstallationhelp.ForeColor = Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelInstallationhelp.Location = new Point(44, 21);
            this.labelInstallationhelp.Name = "labelInstallationhelp";
            this.labelInstallationhelp.Size = new Size(109, 18);
            this.labelInstallationhelp.TabIndex = 4;
            this.labelInstallationhelp.Text = "Installation Help";
            this.labelInstallationhelp.MouseEnter += new EventHandler(this.labelInstallationhelp_MouseEnter);
            this.labelInstallationhelp.MouseLeave += new EventHandler(this.labelInstallationhelp_MouseLeave);
            this.labelInstallationhelp.Click += new EventHandler(this.labelInstallationhelp_Click);
            // 
            // pictureInstallationHelp
            // 
            this.pictureInstallationHelp.Image = global::SQLCM_Installer.Properties.Resources.installationhelpimage;
            this.pictureInstallationHelp.Location = new Point(20, 22);
            this.pictureInstallationHelp.Name = "pictureInstallationHelp";
            this.pictureInstallationHelp.Size = new Size(16, 16);
            this.pictureInstallationHelp.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureInstallationHelp.TabIndex = 3;
            this.pictureInstallationHelp.TabStop = false;
            // 
            // backButton
            // 
            this.backButton.BackColor = Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.backButton.BorderColor = Color.Transparent;
            this.backButton.BorderWidth = 2;
            this.backButton.ButtonText = "";
            this.backButton.Disabled = false;
            this.backButton.EndColor = Color.Yellow;
            this.backButton.FlatAppearance.BorderSize = 0;
            this.backButton.FlatStyle = FlatStyle.Flat;
            this.backButton.Font = new Font("Source Sans Pro", 10.5F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.backButton.ForeColor = Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.backButton.GradientAngle = 90;
            this.backButton.Location = new Point(512, 16);
            this.backButton.Name = "backButton";
            this.backButton.ShowButtontext = true;
            this.backButton.Size = new Size(66, 28);
            this.backButton.StartColor = Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.backButton.TabIndex = 2;
            this.backButton.Text = "Back";
            this.backButton.TextLocation_X = 100;
            this.backButton.TextLocation_Y = 25;
            this.backButton.UseVisualStyleBackColor = false;
            this.backButton.Click += new EventHandler(this.backButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.cancelButton.BorderColor = Color.Transparent;
            this.cancelButton.BorderWidth = 2;
            this.cancelButton.ButtonText = "";
            this.cancelButton.Disabled = false;
            this.cancelButton.EndColor = Color.Yellow;
            this.cancelButton.FlatAppearance.BorderSize = 0;
            this.cancelButton.FlatStyle = FlatStyle.Flat;
            this.cancelButton.Font = new Font("Source Sans Pro", 10.5F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.ForeColor = Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.cancelButton.GradientAngle = 90;
            this.cancelButton.Location = new Point(664, 16);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.ShowButtontext = true;
            this.cancelButton.Size = new Size(66, 28);
            this.cancelButton.StartColor = Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.TextLocation_X = 100;
            this.cancelButton.TextLocation_Y = 25;
            this.cancelButton.UseVisualStyleBackColor = false;
            this.cancelButton.Click += new EventHandler(this.cancelButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.BackColor = Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.nextButton.BorderColor = Color.Transparent;
            this.nextButton.BorderWidth = 2;
            this.nextButton.ButtonText = "";
            this.nextButton.Disabled = false;
            this.nextButton.EndColor = Color.Yellow;
            this.nextButton.FlatAppearance.BorderSize = 0;
            this.nextButton.FlatStyle = FlatStyle.Flat;
            this.nextButton.Font = new Font("Source Sans Pro", 10.5F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.nextButton.ForeColor = Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.nextButton.GradientAngle = 90;
            this.nextButton.Location = new Point(588, 16);
            this.nextButton.Name = "nextButton";
            this.nextButton.ShowButtontext = true;
            this.nextButton.Size = new Size(66, 28);
            this.nextButton.StartColor = Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.nextButton.TabIndex = 0;
            this.nextButton.Text = "Next";
            this.nextButton.TextLocation_X = 100;
            this.nextButton.TextLocation_Y = 25;
            this.nextButton.UseVisualStyleBackColor = false;
            this.nextButton.Click += new EventHandler(this.nextButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(750, 600);
            this.Controls.Add(this.panelSidePanel);
            this.Controls.Add(this.panelBottomPanel);
            this.Controls.Add(this.wizardPanel);
            this.Controls.Add(this.panelHeaderPanel);
            this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "IDERA SQL Compliance Manager";
            this.Activated += new EventHandler(this.MainForm_Activated);
            this.Deactivate += new EventHandler(this.MainForm_Deactivated);
            this.Load += new EventHandler(this.MainForm_Load);
            this.panelHeaderPanel.ResumeLayout(false);
            this.panelHeaderPanel.PerformLayout();
            ((ISupportInitialize)(this.pictureCloseIcon)).EndInit();
            ((ISupportInitialize)(this.pictureHeaderImage)).EndInit();
            this.panelSetupTypeSidePanel.ResumeLayout(false);
            this.panelMainSidePanel.ResumeLayout(false);
            this.panelMainSidePanel.PerformLayout();
            ((ISupportInitialize)(this.pictureTen)).EndInit();
            ((ISupportInitialize)(this.pictureNine)).EndInit();
            ((ISupportInitialize)(this.pictureSeven)).EndInit();
            ((ISupportInitialize)(this.pictureEight)).EndInit();
            ((ISupportInitialize)(this.pictureOne)).EndInit();
            ((ISupportInitialize)(this.pictureSix)).EndInit();
            ((ISupportInitialize)(this.pictureTwo)).EndInit();
            ((ISupportInitialize)(this.pictureFive)).EndInit();
            ((ISupportInitialize)(this.pictureThree)).EndInit();
            ((ISupportInitialize)(this.pictureFour)).EndInit();
            this.panelSidePanel.ResumeLayout(false);
            ((ISupportInitialize)(this.pictureLogo)).EndInit();
            this.panelBottomPanel.ResumeLayout(false);
            this.panelBottomPanel.PerformLayout();
            ((ISupportInitialize)(this.pictureInstallationHelp)).EndInit();
            this.ResumeLayout(false);

        }

        private void labelInstallationhelp_Click(object sender, EventArgs e)
        {
            Process.Start(@"http://wiki.idera.com/x/GgI1");
        }

        private void labelInstallationhelp_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void labelInstallationhelp_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        #endregion

        private Panel panelHeaderPanel;
        private PictureBox pictureHeaderImage;
        private PictureBox pictureCloseIcon;
        private Panel panelSidePanel;
        private PictureBox pictureLogo;
        private Panel wizardPanel;
        private Panel panelBottomPanel;
        private IderaButton backButton;
        private IderaButton cancelButton;
        private IderaButton nextButton;
        private PictureBox pictureInstallationHelp;
        private IderaLabel labelInstallationhelp;
        private Panel panelSetupTypeSidePanel;
        private Panel panelMainSidePanel;
        private PictureBox pictureSix;
        private PictureBox pictureFive;
        private PictureBox pictureFour;
        private PictureBox pictureThree;
        private PictureBox pictureTwo;
        private PictureBox pictureOne;
        private PictureBox pictureSeven;
        private PictureBox pictureEight;
        private IderaTitleLabel labelHeaderImage;
        private IderaButton testConnectionsButton;
        private IderaHeaderLabel labelLeftNavFirst;
        private IderaHeaderLabel labelLeftNavSecond;
        private IderaHeaderLabel labelLeftNavThird;
        private IderaHeaderLabel labelLeftNavFourth;
        private IderaHeaderLabel labelLeftNavFifth;
        private IderaHeaderLabel labelLeftNavSixth;
        private IderaHeaderLabel labelLeftNavSeventh;
        private IderaHeaderLabel labelLeftNavEighth;
        private IderaHeaderLabel labelLeftNavTenth;
        private IderaHeaderLabel labelLeftNavNineth;
        private PictureBox pictureTen;
        private PictureBox pictureNine;
    }
}