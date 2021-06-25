using System ;
using System.ComponentModel ;
using System.Diagnostics ;
using System.Drawing ;
using System.Threading ;
using System.Windows.Forms ;
using Idera.Common.ReportsInstaller.View.HelperForms ;
using Idera.Common.ReportsInstaller.View.Wizard ;

namespace Idera.Common.ReportsInstaller.View
{
	public class InstallerGUI : WizardForm, IInstallerGUI
	{
		/// <summary>
		/// The reporting services folder to store reports in.
		/// </summary>
		private string folder;

		/// <summary>
		/// An instance of the about form.
		/// </summary>
		private AboutForm aboutForm;

		/// <summary>
		/// An instance of the SQLInstance form.
		/// </summary>
		private SQLInstanceForm sqlInstanceForm;

		/// <summary>
		/// An instance of the folder form.
		/// </summary>
		private FolderForm folderForm;

		/// <summary>
		/// The current panel in the wizard.
		/// </summary>
		private WizardPanel currentPanel;

		private string computerName = "";
		/// <summary>
		/// Retrieves the computer name in case it's "localhost".
		/// </summary>
		public string ComputerName
		{
			get
			{ 
				if (computerName.Equals("localhost"))
				{
					return Environment.MachineName;
				}
				else
				{
					return computerName;
				}
			}
		}

		/// <summary>
		/// The name of the product.
		/// </summary>
		private string productName;
		
		/// <summary>
		/// The abbreviated name of the product.
		/// </summary>
		private string productAbbrev;
		
		/// <summary>
		/// The copyright years for the product.
		/// </summary>
		private string copyrightYears;

		/// <summary>
		/// The image on the about form.
		/// </summary>
		private string aboutPanelImage;

		/// <summary>
		/// The image on the first panel.
		/// </summary>
		private string firstPanelImage;

		/// <summary>
		/// The image on the last panel.
		/// </summary>
		private string lastPanelImage;

		/// <summary>
		/// The icon in the form.
		/// </summary>
		private string icon;

		private string sqlServerName = "";
		/// <summary>
		/// Retrieves the server name in case it's "LOCAL" or ".".
		/// </summary>
		public string SqlServerName
		{
			get
			{ 
				if (sqlServerName.ToUpper().StartsWith( "(LOCAL)") || sqlServerName==".")
				{
					return Environment.MachineName;
				}
				else
				{
					return sqlServerName;
				}
			}
		}

		# region Registry Values
		/// <summary>
		/// The report server value stored in the registry.
		/// </summary>
		private string regReportServer;

		/// <summary>
		/// The report folder value stored in the registry.
		/// </summary>
		private string regReportFolder;

		/// <summary>
		/// The report server virtual directory value stored in the registry.
		/// </summary>
		private string regReportDirectory;

		/// <summary>
		/// The report manager virtual directory value stored in the registry.
		/// </summary>
		private string regReportManager;

		/// <summary>
		/// The repository sql server value stored in the registry.
		/// </summary>
		private string regSqlServer;

		/// <summary>
		/// The login name value stored in the registry.
		/// </summary>
		private string regLogin;

		/// <summary>
		/// The ssl checkbox value stored in the registry.
		/// </summary>
		private string regSsl;

		/// <summary>
		/// The url shortcut value stored in the registry.
		/// </summary>
		private string regShortcut;

		/// <summary>
		/// The report server value label stored in the registry.
		/// </summary>
		private string regReportServerName;

		/// <summary>
		/// The report folder value label stored in the registry.
		/// </summary>
		private string regReportFolderName;

		/// <summary>
		/// The report server virtual directory value label stored in the registry.
		/// </summary>
		private string regReportDirectoryName;

		/// <summary>
		/// The report manager virtual directory value label stored in the registry.
		/// </summary>
		private string regReportManagerName;

		/// <summary>
		/// The repository sql server value label stored in the registry.
		/// </summary>
		private string regSqlServerName;

		/// <summary>
		/// The login name value label stored in the registry.
		/// </summary>
		private string regLoginName;

		/// <summary>
		/// The ssl checkbox value label stored in the registry.
		/// </summary>
		private string regSslName;

		/// <summary>
		/// The url shortcut value label stored in the registry.
		/// </summary>
		private string regShortcutName;
		#endregion

		/// <summary>
		/// The ssl checkbox value label stored in the registry.
		/// </summary>
		private InstallState installState;

		/// <summary>
		/// The ssl checkbox value label stored in the registry.
		/// </summary>
		private InstallState modifyState;

		/// <summary>
		/// The ssl checkbox value label stored in the registry.
		/// </summary>
		private InstallState upgradeState;

		/// <summary>
		/// The ssl checkbox value label stored in the registry.
		/// </summary>
		private InstallState currentState;

		private IContainer components = null;
		private GroupBox groupProgress;
		private GroupBox groupSummary;
		private GroupBox groupReports;
		private GroupBox groupRepository;
		private GroupBox groupReportingServices;
		private MainMenu mainMenu;
		private MenuItem menuFile;
		private MenuItem menuExit;
		private MenuItem menuHelp;
		private MenuItem menuAbout;
		private PictureBox pictureIntroSplash;
		private Label labelWelcome;
		private GroupBox groupInstallation;
		private Label labelInstallation1;
		private RadioButton radioInstall;
		private RadioButton radioModify;
		private RadioButton radioUpgrade;
		private Label labelReportingService1;
		private Label labelReportingServiceComputer;
		private Label labelReportingServiceFolder;
		private Label labelReportingServiceDirectory;
		private TextBox textBoxReportingServiceComputer;
		private TextBox textBoxReportingServiceFolder;
		private TextBox textBoxReportingServiceDirectory;
		private Button buttonReportingServiceDefault;
		private Label labelReportingService2;
		private Label labelRepository1;
		private Label labelRepositoryServer;
		private TextBox textBoxRepositoryServer;
		private Button buttonRepositoryBrowse;
		private Label labelRepository2;
		private Label labelRepositoryLogin;
		private Label labelRepositoryPassword;
		private TextBox textBoxRepositoryLogin;
		private TextBox textBoxRepositoryPassword;
		private Button buttonReportsSelect;
		private Button buttonReportsUnselect;
		private Label labelReports1;
		private CheckedListBox checkedListReports;
		private Label labelSummary1;
		private Label labelSummary2;
		private Label labelSummaryWarning;
		private RichTextBox richTextProgress;
		private Label labelComplete1;
		private GroupBox groupComplete;
		private Label labelComplete2;
		private Label labelComplete3;
		private ListBox listBoxComplete;
		private PictureBox pictureBoxComplete;
		private WizardPanel panel7_Complete;
		private WizardPanel panel6_Progress;
		private WizardPanel panel5_Summary;
		private WizardPanel panel4_Reports;
		private WizardPanel panel3_Repository;
		private WizardPanel panel2_ReportingServices;
		private WizardPanel panel1_InstallationOptions;
		private Panel panelBackgroundInstallationOptions;
		private Panel panelBackgroundComplete;
		private WizardPanel panel4_2_Reports;
		private GroupBox groupReports2;
		private ListBox listBoxReports;
		private Label labelReports2;
		private CheckedListBox checkedListReports_2;
		private Label labelReports2_1;
		private Button buttonSelectAll_2;
		private Button buttonUnselectAll_2;
		private WizardPanel panel2_2_ReportingServices;
		private GroupBox groupReportingServices_2;
		private Label labelReportingService1_2;
		private Label labelReportingServicesFolder_2;
		private TextBox textBoxReportingServiceFolder_2;
		private Label labelReportingServiceNote_2;
		private Button buttonBrowse_2;
		private Label labelShortcut;
		private LinkLabel linkLabel;
		private Label labelReportingServiceManager;
		private TextBox textBoxReportingServiceManager;
		private Panel panelInstallationBackground1;
		private Panel panelPictureBoxBackground2;
		private CheckBox checkBoxReportingServiceSSL_1;

		private IModelAccessAdapter modelAdapter;
		/// <summary>
		/// Gives the view access to certain methods in the model.
		/// </summary>
		public IModelAccessAdapter ModelAdapter
		{
			get
			{
				return modelAdapter;
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="_modelAdapter">Gives the view access to certain methods in the model.</param>
		/// <param name="header">The header of this GUI.</param>
		/// <param name="_productName">The name of the product.</param>
		/// <param name="_productAbbrev">The abbreviated name of the product.</param>
		/// <param name="_copyrightYears">The copyright years for the product.</param>
		/// <param name="_folder">The name of the reports folder.</param>
		/// <param name="_aboutPanelImage">The image on the about form.</param>
		/// <param name="_firstPanelImage">The image on the first panel.</param>
		/// <param name="_lastPanelImage">The image on the last panel.</param>
		/// <param name="_icon">The icon in the form.</param>
		public InstallerGUI(IModelAccessAdapter _modelAdapter, string header,
			string _productName, string _productAbbrev, string _copyrightYears,
			string _folder, string _aboutPanelImage, string _firstPanelImage,
			string _lastPanelImage, string _icon)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			modelAdapter = _modelAdapter;
			this.Text = header;
			aboutForm = new AboutForm();
			sqlInstanceForm = new SQLInstanceForm();
			folderForm = new FolderForm();
			productName = _productName;
			productAbbrev = _productAbbrev;
			copyrightYears = _copyrightYears;
			folder = _folder;
			aboutPanelImage = _aboutPanelImage;
			firstPanelImage = _firstPanelImage;
			lastPanelImage = _lastPanelImage;
			icon = _icon;

			regReportServerName = "ReportServer";
			regReportFolderName = "ReportFolder";
			regReportDirectoryName = "ReportDirectory";
			regReportManagerName = "ReportManager";
			regSqlServerName = "SqlServer";
			regLoginName = "Login";
			regSslName = "SslCheckbox";
			regShortcutName = "Shortcut";

			regReportServer = "localhost";
			regReportFolder = folder;
			regReportDirectory = "reportServer";
			regReportManager = "reports";
			regSqlServer = "localhost";
			regLogin = "";
			regSsl = ((Boolean)false).ToString();
			regShortcut = "";

			installState = new InstallState();
			modifyState = new InstallState();
			upgradeState = new InstallState();
			currentState = installState;

			// initialize text fields with the name of the product
			this.labelComplete2.Text = "You have successfully completed the "+productAbbrev+" reports installation wizard.";
			this.groupRepository.Text = productName+" Repository";
			this.labelRepository1.Text = "Specify which SQL Server hosts the "+ productName +" Repository:";
			this.labelReportingService1.Text = "Specify the Report Server computer and folder that host the " +
				productName + " reports.";
			this.labelInstallation1.Text = "The following installation options allow you to select which " +
				productName + " reports you want to install, configure the root report folder, and deploy rep" +
				"orts to Microsoft Reporting Services.  Select the appropriate installation optio" +
				"n:";
			this.labelWelcome.Text = "Welcome to the Idera "+ productAbbrev +" Reports Installer";
			this.labelReportingService1_2.Text = "Specify the Report Server computer and folder that host the " +
				productName + " reports.";
			this.labelRepository2.Text = "Specify the Windows credentials the Report Server will use to connect to the " +
				productAbbrev + " Repository database.  The specified account should have permission to" +
				" execute stored procedures on Repository databases.";
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(InstallerGUI));
			this.panel7_Complete = new Idera.Common.ReportsInstaller.View.Wizard.WizardPanel();
			this.panelPictureBoxBackground2 = new System.Windows.Forms.Panel();
			this.panelBackgroundComplete = new System.Windows.Forms.Panel();
			this.groupComplete = new System.Windows.Forms.GroupBox();
			this.linkLabel = new System.Windows.Forms.LinkLabel();
			this.labelShortcut = new System.Windows.Forms.Label();
			this.listBoxComplete = new System.Windows.Forms.ListBox();
			this.labelComplete3 = new System.Windows.Forms.Label();
			this.labelComplete2 = new System.Windows.Forms.Label();
			this.labelComplete1 = new System.Windows.Forms.Label();
			this.pictureBoxComplete = new System.Windows.Forms.PictureBox();
			this.panel6_Progress = new Idera.Common.ReportsInstaller.View.Wizard.WizardPanel();
			this.groupProgress = new System.Windows.Forms.GroupBox();
			this.richTextProgress = new System.Windows.Forms.RichTextBox();
			this.panel5_Summary = new Idera.Common.ReportsInstaller.View.Wizard.WizardPanel();
			this.groupSummary = new System.Windows.Forms.GroupBox();
			this.labelSummaryWarning = new System.Windows.Forms.Label();
			this.labelSummary2 = new System.Windows.Forms.Label();
			this.labelSummary1 = new System.Windows.Forms.Label();
			this.panel4_Reports = new Idera.Common.ReportsInstaller.View.Wizard.WizardPanel();
			this.groupReports = new System.Windows.Forms.GroupBox();
			this.checkedListReports = new System.Windows.Forms.CheckedListBox();
			this.labelReports1 = new System.Windows.Forms.Label();
			this.buttonReportsUnselect = new System.Windows.Forms.Button();
			this.buttonReportsSelect = new System.Windows.Forms.Button();
			this.panel3_Repository = new Idera.Common.ReportsInstaller.View.Wizard.WizardPanel();
			this.groupRepository = new System.Windows.Forms.GroupBox();
			this.textBoxRepositoryPassword = new System.Windows.Forms.TextBox();
			this.textBoxRepositoryLogin = new System.Windows.Forms.TextBox();
			this.labelRepositoryPassword = new System.Windows.Forms.Label();
			this.labelRepositoryLogin = new System.Windows.Forms.Label();
			this.labelRepository2 = new System.Windows.Forms.Label();
			this.buttonRepositoryBrowse = new System.Windows.Forms.Button();
			this.textBoxRepositoryServer = new System.Windows.Forms.TextBox();
			this.labelRepositoryServer = new System.Windows.Forms.Label();
			this.labelRepository1 = new System.Windows.Forms.Label();
			this.panel2_ReportingServices = new Idera.Common.ReportsInstaller.View.Wizard.WizardPanel();
			this.groupReportingServices = new System.Windows.Forms.GroupBox();
			this.checkBoxReportingServiceSSL_1 = new System.Windows.Forms.CheckBox();
			this.textBoxReportingServiceManager = new System.Windows.Forms.TextBox();
			this.labelReportingServiceManager = new System.Windows.Forms.Label();
			this.labelReportingService2 = new System.Windows.Forms.Label();
			this.buttonReportingServiceDefault = new System.Windows.Forms.Button();
			this.textBoxReportingServiceDirectory = new System.Windows.Forms.TextBox();
			this.textBoxReportingServiceFolder = new System.Windows.Forms.TextBox();
			this.textBoxReportingServiceComputer = new System.Windows.Forms.TextBox();
			this.labelReportingServiceDirectory = new System.Windows.Forms.Label();
			this.labelReportingServiceFolder = new System.Windows.Forms.Label();
			this.labelReportingServiceComputer = new System.Windows.Forms.Label();
			this.labelReportingService1 = new System.Windows.Forms.Label();
			this.panel1_InstallationOptions = new Idera.Common.ReportsInstaller.View.Wizard.WizardPanel();
			this.panelInstallationBackground1 = new System.Windows.Forms.Panel();
			this.panelBackgroundInstallationOptions = new System.Windows.Forms.Panel();
			this.groupInstallation = new System.Windows.Forms.GroupBox();
			this.radioUpgrade = new System.Windows.Forms.RadioButton();
			this.radioModify = new System.Windows.Forms.RadioButton();
			this.radioInstall = new System.Windows.Forms.RadioButton();
			this.labelInstallation1 = new System.Windows.Forms.Label();
			this.labelWelcome = new System.Windows.Forms.Label();
			this.pictureIntroSplash = new System.Windows.Forms.PictureBox();
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.menuFile = new System.Windows.Forms.MenuItem();
			this.menuExit = new System.Windows.Forms.MenuItem();
			this.menuHelp = new System.Windows.Forms.MenuItem();
			this.menuAbout = new System.Windows.Forms.MenuItem();
			this.panel4_2_Reports = new Idera.Common.ReportsInstaller.View.Wizard.WizardPanel();
			this.groupReports2 = new System.Windows.Forms.GroupBox();
			this.buttonUnselectAll_2 = new System.Windows.Forms.Button();
			this.buttonSelectAll_2 = new System.Windows.Forms.Button();
			this.labelReports2_1 = new System.Windows.Forms.Label();
			this.checkedListReports_2 = new System.Windows.Forms.CheckedListBox();
			this.listBoxReports = new System.Windows.Forms.ListBox();
			this.labelReports2 = new System.Windows.Forms.Label();
			this.panel2_2_ReportingServices = new Idera.Common.ReportsInstaller.View.Wizard.WizardPanel();
			this.groupReportingServices_2 = new System.Windows.Forms.GroupBox();
			this.buttonBrowse_2 = new System.Windows.Forms.Button();
			this.labelReportingServiceNote_2 = new System.Windows.Forms.Label();
			this.textBoxReportingServiceFolder_2 = new System.Windows.Forms.TextBox();
			this.labelReportingServicesFolder_2 = new System.Windows.Forms.Label();
			this.labelReportingService1_2 = new System.Windows.Forms.Label();
			this.panel7_Complete.SuspendLayout();
			this.panelBackgroundComplete.SuspendLayout();
			this.groupComplete.SuspendLayout();
			this.panel6_Progress.SuspendLayout();
			this.groupProgress.SuspendLayout();
			this.panel5_Summary.SuspendLayout();
			this.groupSummary.SuspendLayout();
			this.panel4_Reports.SuspendLayout();
			this.groupReports.SuspendLayout();
			this.panel3_Repository.SuspendLayout();
			this.groupRepository.SuspendLayout();
			this.panel2_ReportingServices.SuspendLayout();
			this.groupReportingServices.SuspendLayout();
			this.panel1_InstallationOptions.SuspendLayout();
			this.panelBackgroundInstallationOptions.SuspendLayout();
			this.groupInstallation.SuspendLayout();
			this.panel4_2_Reports.SuspendLayout();
			this.groupReports2.SuspendLayout();
			this.panel2_2_ReportingServices.SuspendLayout();
			this.groupReportingServices_2.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonPrev
			// 
			this.buttonPrev.Location = new System.Drawing.Point(208, 336);
			this.buttonPrev.Name = "buttonPrev";
			// 
			// buttonNext
			// 
			this.buttonNext.Location = new System.Drawing.Point(280, 336);
			this.buttonNext.Name = "buttonNext";
			// 
			// buttonFinish
			// 
			this.buttonFinish.Location = new System.Drawing.Point(384, 336);
			this.buttonFinish.Name = "buttonFinish";
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(472, 336);
			this.buttonCancel.Name = "buttonCancel";
			// 
			// panel7_Complete
			// 
			this.panel7_Complete.BackColor = System.Drawing.Color.White;
			this.panel7_Complete.Controls.Add(this.panelPictureBoxBackground2);
			this.panel7_Complete.Controls.Add(this.panelBackgroundComplete);
			this.panel7_Complete.Controls.Add(this.labelComplete1);
			this.panel7_Complete.Controls.Add(this.pictureBoxComplete);
			this.panel7_Complete.Location = new System.Drawing.Point(0, 0);
			this.panel7_Complete.Name = "panel7_Complete";
			this.panel7_Complete.NextPanelEvent = null;
			this.panel7_Complete.PreviousPanelEvent = null;
			this.panel7_Complete.Size = new System.Drawing.Size(560, 336);
			this.panel7_Complete.TabIndex = 17;
			// 
			// panelPictureBoxBackground2
			// 
			this.panelPictureBoxBackground2.BackColor = System.Drawing.SystemColors.Control;
			this.panelPictureBoxBackground2.Location = new System.Drawing.Point(-16, 328);
			this.panelPictureBoxBackground2.Name = "panelPictureBoxBackground2";
			this.panelPictureBoxBackground2.Size = new System.Drawing.Size(200, 8);
			this.panelPictureBoxBackground2.TabIndex = 3;
			// 
			// panelBackgroundComplete
			// 
			this.panelBackgroundComplete.BackColor = System.Drawing.SystemColors.Control;
			this.panelBackgroundComplete.Controls.Add(this.groupComplete);
			this.panelBackgroundComplete.Location = new System.Drawing.Point(184, 48);
			this.panelBackgroundComplete.Name = "panelBackgroundComplete";
			this.panelBackgroundComplete.Size = new System.Drawing.Size(376, 288);
			this.panelBackgroundComplete.TabIndex = 2;
			// 
			// groupComplete
			// 
			this.groupComplete.BackColor = System.Drawing.SystemColors.Control;
			this.groupComplete.Controls.Add(this.linkLabel);
			this.groupComplete.Controls.Add(this.labelShortcut);
			this.groupComplete.Controls.Add(this.listBoxComplete);
			this.groupComplete.Controls.Add(this.labelComplete3);
			this.groupComplete.Controls.Add(this.labelComplete2);
			this.groupComplete.Location = new System.Drawing.Point(8, 8);
			this.groupComplete.Name = "groupComplete";
			this.groupComplete.Size = new System.Drawing.Size(360, 272);
			this.groupComplete.TabIndex = 2;
			this.groupComplete.TabStop = false;
			// 
			// linkLabel
			// 
			this.linkLabel.Location = new System.Drawing.Point(136, 248);
			this.linkLabel.Name = "linkLabel";
			this.linkLabel.Size = new System.Drawing.Size(216, 16);
			this.linkLabel.TabIndex = 4;
			this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// labelShortcut
			// 
			this.labelShortcut.Location = new System.Drawing.Point(16, 248);
			this.labelShortcut.Name = "labelShortcut";
			this.labelShortcut.Size = new System.Drawing.Size(120, 16);
			this.labelShortcut.TabIndex = 3;
			this.labelShortcut.Text = "Reports are hosted at:";
			// 
			// listBoxComplete
			// 
			this.listBoxComplete.HorizontalScrollbar = true;
			this.listBoxComplete.Location = new System.Drawing.Point(8, 88);
			this.listBoxComplete.Name = "listBoxComplete";
			this.listBoxComplete.Size = new System.Drawing.Size(344, 147);
			this.listBoxComplete.TabIndex = 2;
			// 
			// labelComplete3
			// 
			this.labelComplete3.Location = new System.Drawing.Point(8, 56);
			this.labelComplete3.Name = "labelComplete3";
			this.labelComplete3.Size = new System.Drawing.Size(216, 23);
			this.labelComplete3.TabIndex = 1;
			this.labelComplete3.Text = "You have installed the following reports:";
			// 
			// labelComplete2
			// 
			this.labelComplete2.Location = new System.Drawing.Point(8, 16);
			this.labelComplete2.Name = "labelComplete2";
			this.labelComplete2.Size = new System.Drawing.Size(320, 32);
			this.labelComplete2.TabIndex = 0;
			this.labelComplete2.Text = "You have successfully completed the SQL compliance reports installation wizard.";
			// 
			// labelComplete1
			// 
			this.labelComplete1.BackColor = System.Drawing.Color.White;
			this.labelComplete1.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelComplete1.Location = new System.Drawing.Point(192, 8);
			this.labelComplete1.Name = "labelComplete1";
			this.labelComplete1.Size = new System.Drawing.Size(232, 32);
			this.labelComplete1.TabIndex = 1;
			this.labelComplete1.Text = "Installation Complete";
			// 
			// pictureBoxComplete
			// 
			this.pictureBoxComplete.BackColor = System.Drawing.Color.White;
			this.pictureBoxComplete.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureBoxComplete.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxComplete.Image")));
			this.pictureBoxComplete.Location = new System.Drawing.Point(0, 0);
			this.pictureBoxComplete.Name = "pictureBoxComplete";
			this.pictureBoxComplete.Size = new System.Drawing.Size(184, 336);
			this.pictureBoxComplete.TabIndex = 0;
			this.pictureBoxComplete.TabStop = false;
			// 
			// panel6_Progress
			// 
			this.panel6_Progress.BackColor = System.Drawing.SystemColors.Control;
			this.panel6_Progress.Controls.Add(this.groupProgress);
			this.panel6_Progress.Location = new System.Drawing.Point(0, 0);
			this.panel6_Progress.Name = "panel6_Progress";
			this.panel6_Progress.NextPanelEvent = null;
			this.panel6_Progress.PreviousPanelEvent = null;
			this.panel6_Progress.Size = new System.Drawing.Size(560, 336);
			this.panel6_Progress.TabIndex = 16;
			// 
			// groupProgress
			// 
			this.groupProgress.BackColor = System.Drawing.SystemColors.Control;
			this.groupProgress.Controls.Add(this.richTextProgress);
			this.groupProgress.Location = new System.Drawing.Point(8, 8);
			this.groupProgress.Name = "groupProgress";
			this.groupProgress.Size = new System.Drawing.Size(544, 320);
			this.groupProgress.TabIndex = 0;
			this.groupProgress.TabStop = false;
			this.groupProgress.Text = "Report Installation Progress";
			// 
			// richTextProgress
			// 
			this.richTextProgress.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			this.richTextProgress.Location = new System.Drawing.Point(8, 24);
			this.richTextProgress.Name = "richTextProgress";
			this.richTextProgress.Size = new System.Drawing.Size(528, 288);
			this.richTextProgress.TabIndex = 0;
			this.richTextProgress.Text = "";
			// 
			// panel5_Summary
			// 
			this.panel5_Summary.BackColor = System.Drawing.SystemColors.Control;
			this.panel5_Summary.Controls.Add(this.groupSummary);
			this.panel5_Summary.Location = new System.Drawing.Point(0, 0);
			this.panel5_Summary.Name = "panel5_Summary";
			this.panel5_Summary.NextPanelEvent = null;
			this.panel5_Summary.PreviousPanelEvent = null;
			this.panel5_Summary.Size = new System.Drawing.Size(560, 336);
			this.panel5_Summary.TabIndex = 15;
			// 
			// groupSummary
			// 
			this.groupSummary.BackColor = System.Drawing.SystemColors.Control;
			this.groupSummary.Controls.Add(this.labelSummaryWarning);
			this.groupSummary.Controls.Add(this.labelSummary2);
			this.groupSummary.Controls.Add(this.labelSummary1);
			this.groupSummary.Location = new System.Drawing.Point(8, 8);
			this.groupSummary.Name = "groupSummary";
			this.groupSummary.Size = new System.Drawing.Size(544, 320);
			this.groupSummary.TabIndex = 0;
			this.groupSummary.TabStop = false;
			this.groupSummary.Text = "Summary";
			// 
			// labelSummaryWarning
			// 
			this.labelSummaryWarning.BackColor = System.Drawing.SystemColors.Control;
			this.labelSummaryWarning.Location = new System.Drawing.Point(48, 272);
			this.labelSummaryWarning.Name = "labelSummaryWarning";
			this.labelSummaryWarning.Size = new System.Drawing.Size(464, 32);
			this.labelSummaryWarning.TabIndex = 2;
			this.labelSummaryWarning.Text = "Warning: The upgrade process will overwrite existing reports.  If you have made c" +
				"hanges to the deployed reports, you should backup the modified reports before ap" +
				"plying the upgrade.";
			// 
			// labelSummary2
			// 
			this.labelSummary2.BackColor = System.Drawing.SystemColors.Control;
			this.labelSummary2.Location = new System.Drawing.Point(16, 56);
			this.labelSummary2.Name = "labelSummary2";
			this.labelSummary2.Size = new System.Drawing.Size(136, 23);
			this.labelSummary2.TabIndex = 1;
			this.labelSummary2.Text = "Click \'Finish\' to continue.";
			// 
			// labelSummary1
			// 
			this.labelSummary1.BackColor = System.Drawing.SystemColors.Control;
			this.labelSummary1.Location = new System.Drawing.Point(16, 24);
			this.labelSummary1.Name = "labelSummary1";
			this.labelSummary1.Size = new System.Drawing.Size(416, 23);
			this.labelSummary1.TabIndex = 0;
			this.labelSummary1.Text = "You have finished entering the data necessary to complete the reports installatio" +
				"n.";
			// 
			// panel4_Reports
			// 
			this.panel4_Reports.BackColor = System.Drawing.SystemColors.Control;
			this.panel4_Reports.Controls.Add(this.groupReports);
			this.panel4_Reports.Location = new System.Drawing.Point(0, 0);
			this.panel4_Reports.Name = "panel4_Reports";
			this.panel4_Reports.NextPanelEvent = null;
			this.panel4_Reports.PreviousPanelEvent = null;
			this.panel4_Reports.Size = new System.Drawing.Size(560, 336);
			this.panel4_Reports.TabIndex = 14;
			// 
			// groupReports
			// 
			this.groupReports.BackColor = System.Drawing.SystemColors.Control;
			this.groupReports.Controls.Add(this.checkedListReports);
			this.groupReports.Controls.Add(this.labelReports1);
			this.groupReports.Controls.Add(this.buttonReportsUnselect);
			this.groupReports.Controls.Add(this.buttonReportsSelect);
			this.groupReports.Location = new System.Drawing.Point(8, 8);
			this.groupReports.Name = "groupReports";
			this.groupReports.Size = new System.Drawing.Size(544, 320);
			this.groupReports.TabIndex = 0;
			this.groupReports.TabStop = false;
			this.groupReports.Text = "Reports";
			// 
			// checkedListReports
			// 
			this.checkedListReports.HorizontalScrollbar = true;
			this.checkedListReports.Location = new System.Drawing.Point(16, 48);
			this.checkedListReports.Name = "checkedListReports";
			this.checkedListReports.Size = new System.Drawing.Size(520, 229);
			this.checkedListReports.TabIndex = 4;
			// 
			// labelReports1
			// 
			this.labelReports1.BackColor = System.Drawing.SystemColors.Control;
			this.labelReports1.Location = new System.Drawing.Point(16, 24);
			this.labelReports1.Name = "labelReports1";
			this.labelReports1.Size = new System.Drawing.Size(232, 23);
			this.labelReports1.TabIndex = 2;
			this.labelReports1.Text = "Select which reports you would like to install:";
			// 
			// buttonReportsUnselect
			// 
			this.buttonReportsUnselect.BackColor = System.Drawing.SystemColors.Control;
			this.buttonReportsUnselect.Location = new System.Drawing.Point(280, 288);
			this.buttonReportsUnselect.Name = "buttonReportsUnselect";
			this.buttonReportsUnselect.TabIndex = 1;
			this.buttonReportsUnselect.Text = "&Unselect All";
			this.buttonReportsUnselect.Click += new System.EventHandler(this.buttonReportsUnselect_Click);
			// 
			// buttonReportsSelect
			// 
			this.buttonReportsSelect.BackColor = System.Drawing.SystemColors.Control;
			this.buttonReportsSelect.Location = new System.Drawing.Point(192, 288);
			this.buttonReportsSelect.Name = "buttonReportsSelect";
			this.buttonReportsSelect.TabIndex = 0;
			this.buttonReportsSelect.Text = "&Select All";
			this.buttonReportsSelect.Click += new System.EventHandler(this.buttonReportsSelect_Click);
			// 
			// panel3_Repository
			// 
			this.panel3_Repository.BackColor = System.Drawing.SystemColors.Control;
			this.panel3_Repository.Controls.Add(this.groupRepository);
			this.panel3_Repository.Location = new System.Drawing.Point(0, 0);
			this.panel3_Repository.Name = "panel3_Repository";
			this.panel3_Repository.NextPanelEvent = null;
			this.panel3_Repository.PreviousPanelEvent = null;
			this.panel3_Repository.Size = new System.Drawing.Size(560, 336);
			this.panel3_Repository.TabIndex = 13;
			// 
			// groupRepository
			// 
			this.groupRepository.BackColor = System.Drawing.SystemColors.Control;
			this.groupRepository.Controls.Add(this.textBoxRepositoryPassword);
			this.groupRepository.Controls.Add(this.textBoxRepositoryLogin);
			this.groupRepository.Controls.Add(this.labelRepositoryPassword);
			this.groupRepository.Controls.Add(this.labelRepositoryLogin);
			this.groupRepository.Controls.Add(this.labelRepository2);
			this.groupRepository.Controls.Add(this.buttonRepositoryBrowse);
			this.groupRepository.Controls.Add(this.textBoxRepositoryServer);
			this.groupRepository.Controls.Add(this.labelRepositoryServer);
			this.groupRepository.Controls.Add(this.labelRepository1);
			this.groupRepository.Location = new System.Drawing.Point(8, 8);
			this.groupRepository.Name = "groupRepository";
			this.groupRepository.Size = new System.Drawing.Size(544, 320);
			this.groupRepository.TabIndex = 0;
			this.groupRepository.TabStop = false;
			this.groupRepository.Text = "SQL Compliance Manager Repository";
			// 
			// textBoxRepositoryPassword
			// 
			this.textBoxRepositoryPassword.Location = new System.Drawing.Point(176, 200);
			this.textBoxRepositoryPassword.Name = "textBoxRepositoryPassword";
			this.textBoxRepositoryPassword.PasswordChar = '*';
			this.textBoxRepositoryPassword.Size = new System.Drawing.Size(192, 20);
			this.textBoxRepositoryPassword.TabIndex = 8;
			this.textBoxRepositoryPassword.Text = "";
			// 
			// textBoxRepositoryLogin
			// 
			this.textBoxRepositoryLogin.Location = new System.Drawing.Point(176, 160);
			this.textBoxRepositoryLogin.Name = "textBoxRepositoryLogin";
			this.textBoxRepositoryLogin.Size = new System.Drawing.Size(192, 20);
			this.textBoxRepositoryLogin.TabIndex = 7;
			this.textBoxRepositoryLogin.Text = "";
			// 
			// labelRepositoryPassword
			// 
			this.labelRepositoryPassword.BackColor = System.Drawing.SystemColors.Control;
			this.labelRepositoryPassword.Location = new System.Drawing.Point(48, 200);
			this.labelRepositoryPassword.Name = "labelRepositoryPassword";
			this.labelRepositoryPassword.Size = new System.Drawing.Size(64, 23);
			this.labelRepositoryPassword.TabIndex = 6;
			this.labelRepositoryPassword.Text = "Password:";
			// 
			// labelRepositoryLogin
			// 
			this.labelRepositoryLogin.BackColor = System.Drawing.SystemColors.Control;
			this.labelRepositoryLogin.Location = new System.Drawing.Point(48, 160);
			this.labelRepositoryLogin.Name = "labelRepositoryLogin";
			this.labelRepositoryLogin.Size = new System.Drawing.Size(128, 23);
			this.labelRepositoryLogin.TabIndex = 5;
			this.labelRepositoryLogin.Text = "Login ID (domain\\user}:";
			// 
			// labelRepository2
			// 
			this.labelRepository2.BackColor = System.Drawing.SystemColors.Control;
			this.labelRepository2.Location = new System.Drawing.Point(16, 104);
			this.labelRepository2.Name = "labelRepository2";
			this.labelRepository2.Size = new System.Drawing.Size(512, 40);
			this.labelRepository2.TabIndex = 4;
			this.labelRepository2.Text = "Specify the Windows credentials the Report Server will use to connect to the SQL " +
				"compliance Repository database.  The specified account should have permission to" +
				" execute stored procedures on Repository databases.";
			// 
			// buttonRepositoryBrowse
			// 
			this.buttonRepositoryBrowse.BackColor = System.Drawing.SystemColors.Control;
			this.buttonRepositoryBrowse.Location = new System.Drawing.Point(328, 64);
			this.buttonRepositoryBrowse.Name = "buttonRepositoryBrowse";
			this.buttonRepositoryBrowse.Size = new System.Drawing.Size(75, 20);
			this.buttonRepositoryBrowse.TabIndex = 3;
			this.buttonRepositoryBrowse.Text = "&Browse...";
			this.buttonRepositoryBrowse.Click += new System.EventHandler(this.buttonRepositoryBrowse_Click);
			// 
			// textBoxRepositoryServer
			// 
			this.textBoxRepositoryServer.Location = new System.Drawing.Point(120, 64);
			this.textBoxRepositoryServer.Name = "textBoxRepositoryServer";
			this.textBoxRepositoryServer.Size = new System.Drawing.Size(200, 20);
			this.textBoxRepositoryServer.TabIndex = 2;
			this.textBoxRepositoryServer.Text = "";
			// 
			// labelRepositoryServer
			// 
			this.labelRepositoryServer.BackColor = System.Drawing.SystemColors.Control;
			this.labelRepositoryServer.Location = new System.Drawing.Point(48, 64);
			this.labelRepositoryServer.Name = "labelRepositoryServer";
			this.labelRepositoryServer.Size = new System.Drawing.Size(72, 23);
			this.labelRepositoryServer.TabIndex = 1;
			this.labelRepositoryServer.Text = "SQL Server:";
			// 
			// labelRepository1
			// 
			this.labelRepository1.BackColor = System.Drawing.SystemColors.Control;
			this.labelRepository1.Location = new System.Drawing.Point(16, 24);
			this.labelRepository1.Name = "labelRepository1";
			this.labelRepository1.Size = new System.Drawing.Size(520, 23);
			this.labelRepository1.TabIndex = 0;
			this.labelRepository1.Text = "Specify which SQL Server hosts the SQL Compliance Manager Repository:";
			// 
			// panel2_ReportingServices
			// 
			this.panel2_ReportingServices.BackColor = System.Drawing.SystemColors.Control;
			this.panel2_ReportingServices.Controls.Add(this.groupReportingServices);
			this.panel2_ReportingServices.Location = new System.Drawing.Point(0, 0);
			this.panel2_ReportingServices.Name = "panel2_ReportingServices";
			this.panel2_ReportingServices.NextPanelEvent = null;
			this.panel2_ReportingServices.PreviousPanelEvent = null;
			this.panel2_ReportingServices.Size = new System.Drawing.Size(560, 336);
			this.panel2_ReportingServices.TabIndex = 12;
			// 
			// groupReportingServices
			// 
			this.groupReportingServices.BackColor = System.Drawing.SystemColors.Control;
			this.groupReportingServices.Controls.Add(this.checkBoxReportingServiceSSL_1);
			this.groupReportingServices.Controls.Add(this.textBoxReportingServiceManager);
			this.groupReportingServices.Controls.Add(this.labelReportingServiceManager);
			this.groupReportingServices.Controls.Add(this.labelReportingService2);
			this.groupReportingServices.Controls.Add(this.buttonReportingServiceDefault);
			this.groupReportingServices.Controls.Add(this.textBoxReportingServiceDirectory);
			this.groupReportingServices.Controls.Add(this.textBoxReportingServiceFolder);
			this.groupReportingServices.Controls.Add(this.textBoxReportingServiceComputer);
			this.groupReportingServices.Controls.Add(this.labelReportingServiceDirectory);
			this.groupReportingServices.Controls.Add(this.labelReportingServiceFolder);
			this.groupReportingServices.Controls.Add(this.labelReportingServiceComputer);
			this.groupReportingServices.Controls.Add(this.labelReportingService1);
			this.groupReportingServices.Location = new System.Drawing.Point(8, 8);
			this.groupReportingServices.Name = "groupReportingServices";
			this.groupReportingServices.Size = new System.Drawing.Size(544, 320);
			this.groupReportingServices.TabIndex = 0;
			this.groupReportingServices.TabStop = false;
			this.groupReportingServices.Text = "Reporting Services";
			// 
			// checkBoxReportingServiceSSL_1
			// 
			this.checkBoxReportingServiceSSL_1.Location = new System.Drawing.Point(440, 72);
			this.checkBoxReportingServiceSSL_1.Name = "checkBoxReportingServiceSSL_1";
			this.checkBoxReportingServiceSSL_1.Size = new System.Drawing.Size(48, 24);
			this.checkBoxReportingServiceSSL_1.TabIndex = 11;
			this.checkBoxReportingServiceSSL_1.Text = "SSL";
			// 
			// textBoxReportingServiceManager
			// 
			this.textBoxReportingServiceManager.Location = new System.Drawing.Point(208, 136);
			this.textBoxReportingServiceManager.Name = "textBoxReportingServiceManager";
			this.textBoxReportingServiceManager.Size = new System.Drawing.Size(264, 20);
			this.textBoxReportingServiceManager.TabIndex = 10;
			this.textBoxReportingServiceManager.Text = "";
			// 
			// labelReportingServiceManager
			// 
			this.labelReportingServiceManager.Location = new System.Drawing.Point(40, 136);
			this.labelReportingServiceManager.Name = "labelReportingServiceManager";
			this.labelReportingServiceManager.Size = new System.Drawing.Size(176, 23);
			this.labelReportingServiceManager.TabIndex = 9;
			this.labelReportingServiceManager.Text = "Report &Manager Virtual Directory:";
			// 
			// labelReportingService2
			// 
			this.labelReportingService2.BackColor = System.Drawing.SystemColors.Control;
			this.labelReportingService2.Location = new System.Drawing.Point(16, 256);
			this.labelReportingService2.Name = "labelReportingService2";
			this.labelReportingService2.Size = new System.Drawing.Size(520, 32);
			this.labelReportingService2.TabIndex = 8;
			this.labelReportingService2.Text = "Note: To successfully deploy reports using this utility, your logon account must " +
				"have Content Manager rights on the Report Server.  For more information, see the" +
				" Reporting Services Books Online.";
			// 
			// buttonReportingServiceDefault
			// 
			this.buttonReportingServiceDefault.BackColor = System.Drawing.SystemColors.Control;
			this.buttonReportingServiceDefault.Location = new System.Drawing.Point(288, 200);
			this.buttonReportingServiceDefault.Name = "buttonReportingServiceDefault";
			this.buttonReportingServiceDefault.Size = new System.Drawing.Size(104, 23);
			this.buttonReportingServiceDefault.TabIndex = 7;
			this.buttonReportingServiceDefault.Text = "Restore Defaults";
			this.buttonReportingServiceDefault.Click += new System.EventHandler(this.buttonReportingServiceDefault_Click);
			// 
			// textBoxReportingServiceDirectory
			// 
			this.textBoxReportingServiceDirectory.Location = new System.Drawing.Point(208, 104);
			this.textBoxReportingServiceDirectory.Name = "textBoxReportingServiceDirectory";
			this.textBoxReportingServiceDirectory.Size = new System.Drawing.Size(264, 20);
			this.textBoxReportingServiceDirectory.TabIndex = 6;
			this.textBoxReportingServiceDirectory.Text = "";
			// 
			// textBoxReportingServiceFolder
			// 
			this.textBoxReportingServiceFolder.Location = new System.Drawing.Point(208, 168);
			this.textBoxReportingServiceFolder.Name = "textBoxReportingServiceFolder";
			this.textBoxReportingServiceFolder.Size = new System.Drawing.Size(264, 20);
			this.textBoxReportingServiceFolder.TabIndex = 5;
			this.textBoxReportingServiceFolder.Text = "";
			// 
			// textBoxReportingServiceComputer
			// 
			this.textBoxReportingServiceComputer.Location = new System.Drawing.Point(208, 72);
			this.textBoxReportingServiceComputer.Name = "textBoxReportingServiceComputer";
			this.textBoxReportingServiceComputer.Size = new System.Drawing.Size(224, 20);
			this.textBoxReportingServiceComputer.TabIndex = 4;
			this.textBoxReportingServiceComputer.Text = "";
			// 
			// labelReportingServiceDirectory
			// 
			this.labelReportingServiceDirectory.BackColor = System.Drawing.SystemColors.Control;
			this.labelReportingServiceDirectory.Location = new System.Drawing.Point(40, 104);
			this.labelReportingServiceDirectory.Name = "labelReportingServiceDirectory";
			this.labelReportingServiceDirectory.Size = new System.Drawing.Size(168, 23);
			this.labelReportingServiceDirectory.TabIndex = 3;
			this.labelReportingServiceDirectory.Text = "Report Server Virtual &Directory:";
			// 
			// labelReportingServiceFolder
			// 
			this.labelReportingServiceFolder.BackColor = System.Drawing.SystemColors.Control;
			this.labelReportingServiceFolder.Location = new System.Drawing.Point(40, 168);
			this.labelReportingServiceFolder.Name = "labelReportingServiceFolder";
			this.labelReportingServiceFolder.Size = new System.Drawing.Size(152, 23);
			this.labelReportingServiceFolder.TabIndex = 2;
			this.labelReportingServiceFolder.Text = "Report Server &Folder Name:";
			// 
			// labelReportingServiceComputer
			// 
			this.labelReportingServiceComputer.BackColor = System.Drawing.SystemColors.Control;
			this.labelReportingServiceComputer.Location = new System.Drawing.Point(40, 72);
			this.labelReportingServiceComputer.Name = "labelReportingServiceComputer";
			this.labelReportingServiceComputer.Size = new System.Drawing.Size(136, 23);
			this.labelReportingServiceComputer.TabIndex = 1;
			this.labelReportingServiceComputer.Text = "Report Server &Host Site:";
			// 
			// labelReportingService1
			// 
			this.labelReportingService1.BackColor = System.Drawing.SystemColors.Control;
			this.labelReportingService1.Location = new System.Drawing.Point(16, 24);
			this.labelReportingService1.Name = "labelReportingService1";
			this.labelReportingService1.Size = new System.Drawing.Size(520, 40);
			this.labelReportingService1.TabIndex = 0;
			this.labelReportingService1.Text = "Specify the Report Server computer and folder that host the SQL compliance manage" +
				"r reports.";
			// 
			// panel1_InstallationOptions
			// 
			this.panel1_InstallationOptions.BackColor = System.Drawing.Color.White;
			this.panel1_InstallationOptions.Controls.Add(this.panelInstallationBackground1);
			this.panel1_InstallationOptions.Controls.Add(this.panelBackgroundInstallationOptions);
			this.panel1_InstallationOptions.Controls.Add(this.labelWelcome);
			this.panel1_InstallationOptions.Controls.Add(this.pictureIntroSplash);
			this.panel1_InstallationOptions.Location = new System.Drawing.Point(0, 0);
			this.panel1_InstallationOptions.Name = "panel1_InstallationOptions";
			this.panel1_InstallationOptions.NextPanelEvent = null;
			this.panel1_InstallationOptions.PreviousPanelEvent = null;
			this.panel1_InstallationOptions.Size = new System.Drawing.Size(560, 336);
			this.panel1_InstallationOptions.TabIndex = 11;
			// 
			// panelInstallationBackground1
			// 
			this.panelInstallationBackground1.BackColor = System.Drawing.SystemColors.Control;
			this.panelInstallationBackground1.Location = new System.Drawing.Point(-16, 328);
			this.panelInstallationBackground1.Name = "panelInstallationBackground1";
			this.panelInstallationBackground1.Size = new System.Drawing.Size(200, 8);
			this.panelInstallationBackground1.TabIndex = 3;
			// 
			// panelBackgroundInstallationOptions
			// 
			this.panelBackgroundInstallationOptions.BackColor = System.Drawing.SystemColors.Control;
			this.panelBackgroundInstallationOptions.Controls.Add(this.groupInstallation);
			this.panelBackgroundInstallationOptions.Location = new System.Drawing.Point(184, 72);
			this.panelBackgroundInstallationOptions.Name = "panelBackgroundInstallationOptions";
			this.panelBackgroundInstallationOptions.Size = new System.Drawing.Size(376, 264);
			this.panelBackgroundInstallationOptions.TabIndex = 2;
			// 
			// groupInstallation
			// 
			this.groupInstallation.BackColor = System.Drawing.SystemColors.Control;
			this.groupInstallation.Controls.Add(this.radioUpgrade);
			this.groupInstallation.Controls.Add(this.radioModify);
			this.groupInstallation.Controls.Add(this.radioInstall);
			this.groupInstallation.Controls.Add(this.labelInstallation1);
			this.groupInstallation.Location = new System.Drawing.Point(8, 8);
			this.groupInstallation.Name = "groupInstallation";
			this.groupInstallation.Size = new System.Drawing.Size(360, 248);
			this.groupInstallation.TabIndex = 2;
			this.groupInstallation.TabStop = false;
			this.groupInstallation.Text = "Installation Options";
			// 
			// radioUpgrade
			// 
			this.radioUpgrade.Location = new System.Drawing.Point(16, 160);
			this.radioUpgrade.Name = "radioUpgrade";
			this.radioUpgrade.Size = new System.Drawing.Size(328, 32);
			this.radioUpgrade.TabIndex = 3;
			this.radioUpgrade.Text = "&Upgrade - Add reports and deploy updated reports to an existing installation";
			this.radioUpgrade.CheckedChanged += new System.EventHandler(this.radioUpgrade_CheckedChanged);
			// 
			// radioModify
			// 
			this.radioModify.Location = new System.Drawing.Point(16, 128);
			this.radioModify.Name = "radioModify";
			this.radioModify.Size = new System.Drawing.Size(272, 24);
			this.radioModify.TabIndex = 2;
			this.radioModify.Text = "&Modify - Change data source and add reports";
			this.radioModify.CheckedChanged += new System.EventHandler(this.radioModify_CheckedChanged);
			// 
			// radioInstall
			// 
			this.radioInstall.Checked = true;
			this.radioInstall.Location = new System.Drawing.Point(16, 88);
			this.radioInstall.Name = "radioInstall";
			this.radioInstall.Size = new System.Drawing.Size(312, 32);
			this.radioInstall.TabIndex = 1;
			this.radioInstall.TabStop = true;
			this.radioInstall.Text = "&Install - Create a new root folder, create a new data source, and deploy reports" +
				"";
			this.radioInstall.CheckedChanged += new System.EventHandler(this.radioInstall_CheckedChanged);
			// 
			// labelInstallation1
			// 
			this.labelInstallation1.Location = new System.Drawing.Point(8, 24);
			this.labelInstallation1.Name = "labelInstallation1";
			this.labelInstallation1.Size = new System.Drawing.Size(344, 56);
			this.labelInstallation1.TabIndex = 0;
			this.labelInstallation1.Text = "The following installation options allow you to select which SQL compliance manag" +
				"er reports you want to install, configure the root report folder, and deploy rep" +
				"orts to Microsoft Reporting Services.  Select the appropriate installation optio" +
				"n:";
			// 
			// labelWelcome
			// 
			this.labelWelcome.BackColor = System.Drawing.Color.White;
			this.labelWelcome.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelWelcome.Location = new System.Drawing.Point(192, 8);
			this.labelWelcome.Name = "labelWelcome";
			this.labelWelcome.Size = new System.Drawing.Size(352, 56);
			this.labelWelcome.TabIndex = 1;
			this.labelWelcome.Text = "Welcome to the Idera SQLcompliance Reports Installer";
			// 
			// pictureIntroSplash
			// 
			this.pictureIntroSplash.BackColor = System.Drawing.Color.White;
			this.pictureIntroSplash.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureIntroSplash.Image = ((System.Drawing.Image)(resources.GetObject("pictureIntroSplash.Image")));
			this.pictureIntroSplash.Location = new System.Drawing.Point(0, 0);
			this.pictureIntroSplash.Name = "pictureIntroSplash";
			this.pictureIntroSplash.Size = new System.Drawing.Size(184, 336);
			this.pictureIntroSplash.TabIndex = 0;
			this.pictureIntroSplash.TabStop = false;
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuFile,
																					 this.menuHelp});
			// 
			// menuFile
			// 
			this.menuFile.Index = 0;
			this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuExit});
			this.menuFile.Text = "&File";
			// 
			// menuExit
			// 
			this.menuExit.Index = 0;
			this.menuExit.Text = "&Exit";
			this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
			// 
			// menuHelp
			// 
			this.menuHelp.Index = 1;
			this.menuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuAbout});
			this.menuHelp.Text = "&Help";
			// 
			// menuAbout
			// 
			this.menuAbout.Index = 0;
			this.menuAbout.Text = "&About Idera SQLcompliance Reports Installer...";
			this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
			// 
			// panel4_2_Reports
			// 
			this.panel4_2_Reports.Controls.Add(this.groupReports2);
			this.panel4_2_Reports.Location = new System.Drawing.Point(0, 0);
			this.panel4_2_Reports.Name = "panel4_2_Reports";
			this.panel4_2_Reports.NextPanelEvent = null;
			this.panel4_2_Reports.PreviousPanelEvent = null;
			this.panel4_2_Reports.Size = new System.Drawing.Size(560, 336);
			this.panel4_2_Reports.TabIndex = 18;
			// 
			// groupReports2
			// 
			this.groupReports2.Controls.Add(this.buttonUnselectAll_2);
			this.groupReports2.Controls.Add(this.buttonSelectAll_2);
			this.groupReports2.Controls.Add(this.labelReports2_1);
			this.groupReports2.Controls.Add(this.checkedListReports_2);
			this.groupReports2.Controls.Add(this.listBoxReports);
			this.groupReports2.Controls.Add(this.labelReports2);
			this.groupReports2.Location = new System.Drawing.Point(8, 8);
			this.groupReports2.Name = "groupReports2";
			this.groupReports2.Size = new System.Drawing.Size(544, 320);
			this.groupReports2.TabIndex = 0;
			this.groupReports2.TabStop = false;
			this.groupReports2.Text = "Reports";
			// 
			// buttonUnselectAll_2
			// 
			this.buttonUnselectAll_2.Location = new System.Drawing.Point(136, 288);
			this.buttonUnselectAll_2.Name = "buttonUnselectAll_2";
			this.buttonUnselectAll_2.TabIndex = 11;
			this.buttonUnselectAll_2.Text = "Unselect All";
			this.buttonUnselectAll_2.Click += new System.EventHandler(this.buttonUnselectAll_2_Click);
			// 
			// buttonSelectAll_2
			// 
			this.buttonSelectAll_2.Location = new System.Drawing.Point(48, 288);
			this.buttonSelectAll_2.Name = "buttonSelectAll_2";
			this.buttonSelectAll_2.TabIndex = 10;
			this.buttonSelectAll_2.Text = "Select All";
			this.buttonSelectAll_2.Click += new System.EventHandler(this.buttonSelectAll_2_Click);
			// 
			// labelReports2_1
			// 
			this.labelReports2_1.Location = new System.Drawing.Point(16, 32);
			this.labelReports2_1.Name = "labelReports2_1";
			this.labelReports2_1.Size = new System.Drawing.Size(232, 23);
			this.labelReports2_1.TabIndex = 9;
			this.labelReports2_1.Text = "Select which reports you would like to install:";
			// 
			// checkedListReports_2
			// 
			this.checkedListReports_2.HorizontalScrollbar = true;
			this.checkedListReports_2.Location = new System.Drawing.Point(16, 56);
			this.checkedListReports_2.Name = "checkedListReports_2";
			this.checkedListReports_2.Size = new System.Drawing.Size(224, 229);
			this.checkedListReports_2.TabIndex = 8;
			// 
			// listBoxReports
			// 
			this.listBoxReports.HorizontalScrollbar = true;
			this.listBoxReports.Location = new System.Drawing.Point(280, 56);
			this.listBoxReports.Name = "listBoxReports";
			this.listBoxReports.Size = new System.Drawing.Size(232, 225);
			this.listBoxReports.TabIndex = 7;
			// 
			// labelReports2
			// 
			this.labelReports2.BackColor = System.Drawing.SystemColors.Control;
			this.labelReports2.Location = new System.Drawing.Point(280, 32);
			this.labelReports2.Name = "labelReports2";
			this.labelReports2.Size = new System.Drawing.Size(224, 23);
			this.labelReports2.TabIndex = 6;
			this.labelReports2.Text = "Reports already installed on this computer:";
			// 
			// panel2_2_ReportingServices
			// 
			this.panel2_2_ReportingServices.Controls.Add(this.groupReportingServices_2);
			this.panel2_2_ReportingServices.Location = new System.Drawing.Point(0, 0);
			this.panel2_2_ReportingServices.Name = "panel2_2_ReportingServices";
			this.panel2_2_ReportingServices.NextPanelEvent = null;
			this.panel2_2_ReportingServices.PreviousPanelEvent = null;
			this.panel2_2_ReportingServices.Size = new System.Drawing.Size(560, 336);
			this.panel2_2_ReportingServices.TabIndex = 19;
			// 
			// groupReportingServices_2
			// 
			this.groupReportingServices_2.Controls.Add(this.buttonBrowse_2);
			this.groupReportingServices_2.Controls.Add(this.labelReportingServiceNote_2);
			this.groupReportingServices_2.Controls.Add(this.textBoxReportingServiceFolder_2);
			this.groupReportingServices_2.Controls.Add(this.labelReportingServicesFolder_2);
			this.groupReportingServices_2.Controls.Add(this.labelReportingService1_2);
			this.groupReportingServices_2.Location = new System.Drawing.Point(8, 8);
			this.groupReportingServices_2.Name = "groupReportingServices_2";
			this.groupReportingServices_2.Size = new System.Drawing.Size(544, 320);
			this.groupReportingServices_2.TabIndex = 0;
			this.groupReportingServices_2.TabStop = false;
			this.groupReportingServices_2.Text = "Reporting Services";
			// 
			// buttonBrowse_2
			// 
			this.buttonBrowse_2.Location = new System.Drawing.Point(400, 104);
			this.buttonBrowse_2.Name = "buttonBrowse_2";
			this.buttonBrowse_2.TabIndex = 9;
			this.buttonBrowse_2.Text = "Browse";
			this.buttonBrowse_2.Click += new System.EventHandler(this.buttonBrowse_2_Click);
			// 
			// labelReportingServiceNote_2
			// 
			this.labelReportingServiceNote_2.Location = new System.Drawing.Point(16, 256);
			this.labelReportingServiceNote_2.Name = "labelReportingServiceNote_2";
			this.labelReportingServiceNote_2.Size = new System.Drawing.Size(520, 32);
			this.labelReportingServiceNote_2.TabIndex = 8;
			this.labelReportingServiceNote_2.Text = "Note: To successfully deploy reports using this utility, your logon account must " +
				"have Content Manager rights on the Report Server. For more information, see the " +
				"Reporting Services Books Online. ";
			// 
			// textBoxReportingServiceFolder_2
			// 
			this.textBoxReportingServiceFolder_2.Location = new System.Drawing.Point(208, 104);
			this.textBoxReportingServiceFolder_2.Name = "textBoxReportingServiceFolder_2";
			this.textBoxReportingServiceFolder_2.Size = new System.Drawing.Size(184, 20);
			this.textBoxReportingServiceFolder_2.TabIndex = 5;
			this.textBoxReportingServiceFolder_2.Text = "";
			// 
			// labelReportingServicesFolder_2
			// 
			this.labelReportingServicesFolder_2.Location = new System.Drawing.Point(40, 104);
			this.labelReportingServicesFolder_2.Name = "labelReportingServicesFolder_2";
			this.labelReportingServicesFolder_2.Size = new System.Drawing.Size(152, 23);
			this.labelReportingServicesFolder_2.TabIndex = 2;
			this.labelReportingServicesFolder_2.Text = "Report Server &Folder Name:";
			// 
			// labelReportingService1_2
			// 
			this.labelReportingService1_2.Location = new System.Drawing.Point(16, 24);
			this.labelReportingService1_2.Name = "labelReportingService1_2";
			this.labelReportingService1_2.Size = new System.Drawing.Size(520, 40);
			this.labelReportingService1_2.TabIndex = 0;
			this.labelReportingService1_2.Text = "Specify the Report Server computer and folder that host the SQL compliance manage" +
				"r reports.";
			// 
			// InstallerGUI
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(554, 363);
			this.Controls.Add(this.panel6_Progress);
			this.Controls.Add(this.panel5_Summary);
			this.Controls.Add(this.panel4_2_Reports);
			this.Controls.Add(this.panel4_Reports);
			this.Controls.Add(this.panel3_Repository);
			this.Controls.Add(this.panel2_2_ReportingServices);
			this.Controls.Add(this.panel2_ReportingServices);
			this.Controls.Add(this.panel1_InstallationOptions);
			this.Controls.Add(this.panel7_Complete);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Menu = this.mainMenu;
			this.MinimizeBox = false;
			this.Name = "InstallerGUI";
			this.Text = "Idera SQLcompliance Reports Installer";
			this.Load += new System.EventHandler(this.InstallerGUI_Load);
			this.Controls.SetChildIndex(this.panel7_Complete, 0);
			this.Controls.SetChildIndex(this.panel1_InstallationOptions, 0);
			this.Controls.SetChildIndex(this.panel2_ReportingServices, 0);
			this.Controls.SetChildIndex(this.panel2_2_ReportingServices, 0);
			this.Controls.SetChildIndex(this.panel3_Repository, 0);
			this.Controls.SetChildIndex(this.panel4_Reports, 0);
			this.Controls.SetChildIndex(this.panel4_2_Reports, 0);
			this.Controls.SetChildIndex(this.panel5_Summary, 0);
			this.Controls.SetChildIndex(this.panel6_Progress, 0);
			this.Controls.SetChildIndex(this.buttonPrev, 0);
			this.Controls.SetChildIndex(this.buttonNext, 0);
			this.Controls.SetChildIndex(this.buttonFinish, 0);
			this.Controls.SetChildIndex(this.buttonCancel, 0);
			this.panel7_Complete.ResumeLayout(false);
			this.panelBackgroundComplete.ResumeLayout(false);
			this.groupComplete.ResumeLayout(false);
			this.panel6_Progress.ResumeLayout(false);
			this.groupProgress.ResumeLayout(false);
			this.panel5_Summary.ResumeLayout(false);
			this.groupSummary.ResumeLayout(false);
			this.panel4_Reports.ResumeLayout(false);
			this.groupReports.ResumeLayout(false);
			this.panel3_Repository.ResumeLayout(false);
			this.groupRepository.ResumeLayout(false);
			this.panel2_ReportingServices.ResumeLayout(false);
			this.groupReportingServices.ResumeLayout(false);
			this.panel1_InstallationOptions.ResumeLayout(false);
			this.panelBackgroundInstallationOptions.ResumeLayout(false);
			this.groupInstallation.ResumeLayout(false);
			this.panel4_2_Reports.ResumeLayout(false);
			this.groupReports2.ResumeLayout(false);
			this.panel2_2_ReportingServices.ResumeLayout(false);
			this.groupReportingServices_2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Starts the GUI
		/// </summary>
		public void Run()
		{
			Application.Run(this);
		}

		private delegate void DisplayMessageBoxDelegate(string message, string titleBar);
		/// <summary>
		/// Displays a message box with the specified text and specified
		/// title bar text.
		/// </summary>
		/// <param name="message">The text that appears in the message box.</param>
		/// <param name="titleBar">The text that appears in the title bar
		/// of the message box.</param>
		public void DisplayMessageBox(string message, string titleBar)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new DisplayMessageBoxDelegate(DisplayMessageBox),
					new object[] {message, titleBar});
				return;
			}
			MessageBox.Show(this, message, this.Text + ": " + titleBar);
		}

		/// <summary>
		/// Displays a message box with the specified text and specified
		/// title bar text.  Also includes YesNo buttons.
		/// </summary>
		/// <param name="message">The text that appears in the message box.</param>
		/// <param name="titleBar">The text that appears in the title bar
		/// of the message box.</param>
		/// <returns>True if Yes is chosen; False if No is chosen</returns>
		public bool DisplayMessageBoxYesNo(string message, string titleBar)
		{
			DialogResult choice = MessageBox.Show(this, message, this.Text + ": " + 
				titleBar, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (choice == DialogResult.Yes)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private delegate void UpdateInstallerLogDelegate(string message);
		/// <summary>
		/// Adds text to the installer log (richTextProgress).
		/// </summary>
		/// <param name="message">The text that appears in the rich text box 
		/// acting as an installer log.</param>
		public void UpdateInstallerLog(string message)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new UpdateInstallerLogDelegate(UpdateInstallerLog),
					new object[] {message});
				return;
			}
			richTextProgress.AppendText(message);
		}

		private delegate void AddToReportsListDelegate(string data);
		/// <summary>
		/// Adds a report to the checked list box (checkedListReports).
		/// </summary>
		/// <param name="data">The item added to the checked list box</param>
		public void AddToReportsList(string data)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new AddToReportsListDelegate(AddToReportsList),
					new object[] {data});
				return;
			}
			checkedListReports.Items.Add(data, true);
		}

		private delegate void AddToReportsList_2Delegate(string data);
		/// <summary>
		/// Adds a report to the checked list box (checkedListReports_@).
		/// </summary>
		/// <param name="data">The item added to the checked list box</param>
		public void AddToReportsList_2(string data)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new AddToReportsList_2Delegate(AddToReportsList_2),
					new object[] {data});
				return;
			}
			checkedListReports_2.Items.Add(data, true);
		}

		private delegate void AddToAlreadyInstalledListDelegate(string data);
		/// <summary>
		/// Adds a report to the already installed list box (listBoxReports).
		/// </summary>
		/// <param name="data">The item added to the list box</param>
		public void AddToAlreadyInstalledList(string data)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new AddToAlreadyInstalledListDelegate(AddToAlreadyInstalledList),
					new object[] {data});
				return;
			}
			listBoxReports.Items.Add(data);
		}

		private delegate void AddToInstalledListDelegate(string data);
		/// <summary>
		/// Adds a report to the completed list box (listBoxComplete).
		/// </summary>
		/// <param name="data">The item added to the list box</param>
		public void AddToInstalledList(string data)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new AddToInstalledListDelegate(AddToInstalledList),
					new object[] {data});
				return;
			}
			listBoxComplete.Items.Add(data);
		}

		/// <summary>
		/// Adds an item to the list box in the SQL Instance Form.
		/// </summary>
		/// <param name="data">The item being added</param>
		public void AddItemToSQLInstanceList(string data)
		{
			sqlInstanceForm.AddItemToSQLInstanceList(data);
		}

		/// <summary>
		/// Adds an item to the list box in the folder Form.
		/// </summary>
		/// <param name="data">The item being added</param>
		public void AddItemToFolderList(string data)
		{
			folderForm.AddItemToFolderList(data);
		}

		private void InstallerGUI_Load(object sender, EventArgs e)
		{
			if (modelAdapter.DoesReportsXMLFileExist() == false)
			{
				Application.Exit();
			}
			else
			{
				try
				{
					pictureIntroSplash.Image = Image.FromFile(firstPanelImage);
					pictureBoxComplete.Image = Image.FromFile(lastPanelImage);
					this.Icon = new Icon(icon);
				}
				catch (Exception ex)
				{
				}

				checkedListReports.Items.Clear();
				modelAdapter.ReportDescriptor();

				regReportServer = modelAdapter.ReadPreferences(regReportServerName, regReportServer);
				regReportFolder = modelAdapter.ReadPreferences(regReportFolderName, regReportFolder);
				regReportDirectory = modelAdapter.ReadPreferences(regReportDirectoryName, regReportDirectory);
				regReportManager = modelAdapter.ReadPreferences(regReportManagerName, regReportManager);
				regSqlServer = modelAdapter.ReadPreferences(regSqlServerName, regSqlServer);
				regLogin = modelAdapter.ReadPreferences(regLoginName, regLogin);
				regSsl = modelAdapter.ReadPreferences(regSslName, regSsl);
				regShortcut = modelAdapter.ReadPreferences(regShortcutName, regShortcut);

				textBoxReportingServiceComputer.Text = regReportServer;
				textBoxReportingServiceFolder.Text = regReportFolder;
				textBoxReportingServiceDirectory.Text = regReportDirectory;
				textBoxReportingServiceManager.Text = regReportManager;
				textBoxRepositoryServer.Text = regSqlServer;
				textBoxRepositoryLogin.Text = regLogin;
				checkBoxReportingServiceSSL_1.Checked = bool.Parse(regSsl);
				linkLabel.Text = regShortcut;

				textBoxReportingServiceFolder_2.Text = regReportFolder;

				computerName = textBoxReportingServiceComputer.Text;
				sqlServerName = textBoxRepositoryServer.Text;

				panel1_InstallationOptions.PreviousPanelEvent = new PreviousPanelEventHandler(Panel1_InstallationOptions_Previous);
				panel1_InstallationOptions.NextPanelEvent = new NextPanelEventHandler(Panel1_InstallationOptions_Next);
				panel2_ReportingServices.PreviousPanelEvent = new PreviousPanelEventHandler(Panel2_ReportingServices_Previous);
				panel2_ReportingServices.NextPanelEvent = new NextPanelEventHandler(Panel2_ReportingServices_Next);
				panel2_2_ReportingServices.PreviousPanelEvent = new PreviousPanelEventHandler(Panel2_2_ReportingServices_Previous);
				panel2_2_ReportingServices.NextPanelEvent = new NextPanelEventHandler(Panel2_2_ReportingServices_Next);
				panel3_Repository.PreviousPanelEvent = new PreviousPanelEventHandler(Panel3_Repository_Previous);
				panel3_Repository.NextPanelEvent = new NextPanelEventHandler(Panel3_Repository_Next);
				panel4_Reports.PreviousPanelEvent = new PreviousPanelEventHandler(Panel4_Reports_Previous);
				panel4_Reports.NextPanelEvent = new NextPanelEventHandler(Panel4_Reports_Next);
				panel4_2_Reports.PreviousPanelEvent = new PreviousPanelEventHandler(Panel4_2_Reports_Previous);
				panel4_2_Reports.NextPanelEvent = new NextPanelEventHandler(Panel4_2_Reports_Next);
				panel5_Summary.PreviousPanelEvent = new PreviousPanelEventHandler(Panel5_Summary_Previous);
				panel5_Summary.NextPanelEvent = new NextPanelEventHandler(Panel5_Summary_Next);
				panel6_Progress.PreviousPanelEvent = new PreviousPanelEventHandler(Panel6_Progress_Previous);
				panel6_Progress.NextPanelEvent = new NextPanelEventHandler(Panel6_Progress_Next);
				panel7_Complete.PreviousPanelEvent = new PreviousPanelEventHandler(Panel7_Complete_Previous);
				panel7_Complete.NextPanelEvent = new NextPanelEventHandler(Panel7_Complete_Next);

				panel1_InstallationOptions.Enabled = true;
				panel2_ReportingServices.Enabled = false;
				panel3_Repository.Enabled = false;
				panel4_Reports.Enabled = false;
				panel5_Summary.Enabled = false;
				panel6_Progress.Enabled = false;
				panel7_Complete.Enabled = false;

				buttonPrev.Enabled = false;
				buttonNext.Enabled = true;
				buttonFinish.Enabled = false;
				buttonCancel.Enabled = true;
				this.AcceptButton = buttonNext;

				panel1_InstallationOptions.BringToFront();
				currentPanel = panel1_InstallationOptions;
				if ( radioInstall.Checked )
				{
					radioInstall.Focus();
				}
				else if ( radioModify.Checked )
				{
					radioModify.Focus();
				}
				else if ( radioUpgrade.Checked )
				{
					radioUpgrade.Focus();
				}

				installState.InstallationEvent = new InstallationEventHandler(DoInstall_Install);
				modifyState.InstallationEvent = new InstallationEventHandler(DoInstall_Modify);
				upgradeState.InstallationEvent = new InstallationEventHandler(DoInstall_Upgrade);

				aboutForm.initialize(this.Text, modelAdapter.GetVersion(), copyrightYears, productName,
					aboutPanelImage, icon);
				sqlInstanceForm.initialize(icon);
				folderForm.initialize(icon);
				sqlInstanceForm.OkButtonEvent = new SQLInstanceFormEventHandler(SQLInstanceChosen);
				folderForm.OkButtonEvent = new FolderFormEventHandler(FolderChosen);
			}
		}

		private void SQLInstanceChosen()
		{
			textBoxRepositoryServer.Text = sqlInstanceForm.SelectedInstanceInBox();
		}

		private void FolderChosen()
		{
			textBoxReportingServiceFolder_2.Text = folderForm.SelectedInstanceInBox();
		}

		protected override void buttonPrev_Click(object sender, EventArgs e)
		{
//			this.Cursor = Cursors.WaitCursor;
			currentPanel.PreviousPanel();
//			this.Cursor = Cursors.Default;
		}

		protected override void buttonNext_Click(object sender, EventArgs e)
		{
//			this.Cursor = Cursors.WaitCursor;
			currentPanel.NextPanel();
//			this.Cursor = Cursors.Default;
		}

		protected override void buttonFinish_Click(object sender, EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
//			base.GoToNextPanel(currentPanel, panel6_Progress);

			currentPanel.Enabled = false;
			panel6_Progress.Enabled = true;
			panel6_Progress.BringToFront();

			currentPanel = panel6_Progress;
			buttonPrev.Enabled = false;
			buttonFinish.Enabled = false;
			buttonPrev.Visible = false;
			buttonNext.Visible = false;
			buttonFinish.Visible = false;
			buttonCancel.Enabled = false;
			this.AcceptButton = null;
			richTextProgress.Text = "";

//			DoInstall();

//			this.Cursor = Cursors.Default;

			Thread t = new Thread(new ThreadStart(currentState.Install));
			t.Start();
//			currentState.Install();

//			this.Cursor = Cursors.Default;
		}

		protected override void buttonCancel_Click(object sender, EventArgs e)
		{
			if (buttonCancel.Text != "&Close")
			{
				bool choice = DisplayMessageBoxYesNo("Are you sure you want to exit the Reports Installer?",
					"Closing Installer");
				if (choice)
				{
					Application.Exit();
				}
			}
			else
			{
				Application.Exit();
			}
		}

		#region Wizard Panel Event Handling

		public void Panel1_InstallationOptions_Previous()
		{
		}

		public void Panel1_InstallationOptions_Next()
		{
			this.Cursor = Cursors.WaitCursor;
			buttonPrev.Enabled = true;
			if (radioModify.Checked || radioUpgrade.Checked)
			{
				base.GoToNextPanel(currentPanel, panel2_2_ReportingServices);
				currentPanel = panel2_2_ReportingServices;
				textBoxReportingServiceFolder_2.Focus();
			}
			else
			{
				base.GoToNextPanel(currentPanel, panel2_ReportingServices);
				currentPanel = panel2_ReportingServices;
				textBoxReportingServiceComputer.Focus();
			}
			this.Cursor = Cursors.Default;
		}

		public void Panel2_ReportingServices_Previous()
		{
			this.Cursor = Cursors.WaitCursor;
			buttonPrev.Enabled = false;
			base.GoToPreviousPanel(currentPanel, panel1_InstallationOptions);
			currentPanel = panel1_InstallationOptions;
			if ( radioInstall.Checked )
			{
				radioInstall.Focus();
			}
			else if ( radioModify.Checked )
			{
				radioModify.Focus();
			}
			else if ( radioUpgrade.Checked )
			{
				radioUpgrade.Focus();
			}
			this.Cursor = Cursors.Default;
		}

		public void Panel2_ReportingServices_Next()
		{
			this.Cursor = Cursors.WaitCursor;
			if (textBoxReportingServiceComputer.Text.Length == 0)
			{
				DisplayMessageBox("Report Server Computer is a required field.", "Empty Field!");
			}
			else if (textBoxReportingServiceFolder.Text.Length == 0)
			{
				DisplayMessageBox("Report Server Folder is a required field.", "Empty Field!");
			}
			else if (textBoxReportingServiceDirectory.Text.Length == 0)
			{
				DisplayMessageBox("Report Server Virtual Directory is a required field.", "Empty Field!");
			}
			else if (textBoxReportingServiceManager.Text.Length == 0)
			{
				DisplayMessageBox("Report Manager Virtual Directory is a required field.", "Empty Field!");
			}
			else
			{
				Thread t = new Thread(new ThreadStart(Panel2_ReportingServices_Next_Helper));
				t.Start();
				/*
				if (checkBoxReportingServiceSSL_1.Checked)
				{
					modelAdapter.SetSslProxy();
				}
				else
				{
					modelAdapter.SetNormalProxy();
				}
				computerName = modelAdapter.ParseHostSite(textBoxReportingServiceComputer.Text);
				if (modelAdapter.TestReportManager(ComputerName, textBoxReportingServiceManager.Text))
				{
					if (modelAdapter.HasAppropriatePermissions(ComputerName, textBoxReportingServiceDirectory.Text))
					{
						base.GoToNextPanel(currentPanel, panel3_Repository);
						currentPanel = panel3_Repository;
						textBoxRepositoryServer.Focus();
					}
				}
				*/
			}
		}

		private void Panel2_ReportingServices_Next_Helper()
		{
			if (checkBoxReportingServiceSSL_1.Checked)
			{
				modelAdapter.SetSslProxy();
			}
			else
			{
				modelAdapter.SetNormalProxy();
			}
			computerName = modelAdapter.ParseHostSite(textBoxReportingServiceComputer.Text);
			if (modelAdapter.TestHostComputer(ComputerName))
			{
				if (modelAdapter.TestReportManager(ComputerName, textBoxReportingServiceManager.Text))
				{
					if (modelAdapter.TestReportServer(ComputerName, textBoxReportingServiceDirectory.Text))
					{
						if (modelAdapter.HasAppropriatePermissions(ComputerName, textBoxReportingServiceDirectory.Text))
						{
							base.GoToNextPanel(currentPanel, panel3_Repository);
							currentPanel = panel3_Repository;
							textBoxRepositoryServer.Focus();
						}
					}
				}
			}
			this.Cursor = Cursors.Default;
		}

		public void Panel2_2_ReportingServices_Previous()
		{
			this.Cursor = Cursors.WaitCursor;
			buttonPrev.Enabled = false;
			base.GoToPreviousPanel(currentPanel, panel1_InstallationOptions);
			currentPanel = panel1_InstallationOptions;
			if ( radioInstall.Checked )
			{
				radioInstall.Focus();
			}
			else if ( radioModify.Checked )
			{
				radioModify.Focus();
			}
			else if ( radioUpgrade.Checked )
			{
				radioUpgrade.Focus();
			}
			this.Cursor = Cursors.Default;
		}

		public void Panel2_2_ReportingServices_Next()
		{
			this.Cursor = Cursors.WaitCursor;
			if (textBoxReportingServiceFolder_2.Text.Length == 0)
			{
				DisplayMessageBox("Report Server Folder is a required field.", "Empty Field!");
			}
			else
			{
				Thread t = new Thread(new ThreadStart(Panel2_2_ReportingServices_Next_Helper));
				t.Start();
/*				if (Boolean.Parse(regSsl))
				{
					modelAdapter.SetSslProxy();
				}
				else
				{
					modelAdapter.SetNormalProxy();
				}
				computerName = modelAdapter.ParseHostSite(regReportServer);
				if (modelAdapter.TestReportManager(ComputerName, regReportManager))
				{
					if (modelAdapter.HasAppropriatePermissions(ComputerName, regReportDirectory))
					{
						if (radioUpgrade.Checked)
						{
							if (modelAdapter.DoesFolderExist(ComputerName, regReportDirectory, textBoxReportingServiceFolder_2.Text) == false)
							{
								DisplayMessageBox("The specified Report Server folder does not exist. An upgrade cannot be performed if the Report Server folder does not exist.  To create a folder, use the install option.", "Error!");
							}
							else
							{
								checkedListReports_2.Items.Clear();
								listBoxReports.Items.Clear();
								modelAdapter.ReportsFoundInFolder(ComputerName, regReportDirectory, textBoxReportingServiceFolder_2.Text);
								base.GoToNextPanel(currentPanel, panel4_2_Reports);
								currentPanel = panel4_2_Reports;
								checkedListReports_2.Focus();
							}
						}
							// radioModify.Checked
						else
						{
							if (modelAdapter.DoesFolderExist(ComputerName, regReportDirectory, textBoxReportingServiceFolder_2.Text) == false)
							{
								DisplayMessageBox("The specified Report Server folder does not exist. You can only modify an existing Reports Server folder.  To create a folder, use the install option.", "Error!");
							}
							else
							{
								base.GoToNextPanel(currentPanel, panel3_Repository);
								currentPanel = panel3_Repository;
								textBoxRepositoryServer.Focus();
							}
						}
					}
				}*/
			}
		}

		private void Panel2_2_ReportingServices_Next_Helper()
		{
			if (Boolean.Parse(regSsl))
			{
				modelAdapter.SetSslProxy();
			}
			else
			{
				modelAdapter.SetNormalProxy();
			}
			computerName = modelAdapter.ParseHostSite(regReportServer);
			if (modelAdapter.TestReportManager(ComputerName, regReportManager))
			{
				if (modelAdapter.HasAppropriatePermissions(ComputerName, regReportDirectory))
				{
					if (radioUpgrade.Checked)
					{
						if (modelAdapter.DoesFolderExist(ComputerName, regReportDirectory, textBoxReportingServiceFolder_2.Text) == false)
						{
							DisplayMessageBox("The specified Report Server folder does not exist. An upgrade cannot be performed if the Report Server folder does not exist.  To create a folder, use the install option.", "Error!");
						}
						else
						{
							checkedListReports_2.Items.Clear();
							listBoxReports.Items.Clear();
							modelAdapter.ReportsFoundInFolder(ComputerName, regReportDirectory, textBoxReportingServiceFolder_2.Text);
							base.GoToNextPanel(currentPanel, panel4_2_Reports);
							currentPanel = panel4_2_Reports;
							checkedListReports_2.Focus();
						}
					}
						// radioModify.Checked
					else
					{
						if (modelAdapter.DoesFolderExist(ComputerName, regReportDirectory, textBoxReportingServiceFolder_2.Text) == false)
						{
							DisplayMessageBox("The specified Report Server folder does not exist. You can only modify an existing Reports Server folder.  To create a folder, use the install option.", "Error!");
						}
						else
						{
							base.GoToNextPanel(currentPanel, panel3_Repository);
							currentPanel = panel3_Repository;
							textBoxRepositoryServer.Focus();
						}
					}
				}
			}
			this.Cursor = Cursors.Default;
		}

		public void Panel3_Repository_Previous()
		{
			this.Cursor = Cursors.WaitCursor;
			if (radioModify.Checked)
			{
				base.GoToPreviousPanel(currentPanel, panel2_2_ReportingServices);
				currentPanel = panel2_2_ReportingServices;
				textBoxReportingServiceFolder_2.Focus();
			}
			else
			{
				base.GoToPreviousPanel(currentPanel, panel2_ReportingServices);
				currentPanel = panel2_ReportingServices;
				textBoxReportingServiceComputer.Focus();
			}
			this.Cursor = Cursors.Default;
		}

		public void Panel3_Repository_Next()
		{
			this.Cursor = Cursors.WaitCursor;
			sqlServerName = textBoxRepositoryServer.Text;
			if(textBoxRepositoryServer.Text.Length == 0)
			{
				DisplayMessageBox("SQL Server instance is a required field.", "Error!");
			}
			else if(textBoxRepositoryLogin.Text.Length == 0)
			{
				DisplayMessageBox("Logon ID is a required field.", "Error!");
			}
			else if(textBoxRepositoryPassword.Text.Length == 0)
			{
				DisplayMessageBox("Password is a required field.", "Error!");
			}
			else
			{
				Thread t = new Thread(new ThreadStart(Panel3_Repository_Next_Helper));
				t.Start();
			}
/*			else if (modelAdapter.ValidateAccountName(textBoxRepositoryLogin.Text) == false)
			{
				DisplayMessageBox("Logon ID must be a windows account specified in the form of 'domain\\user'.", "Error!");
			}
			else if(modelAdapter.TestConnection(SqlServerName) == false)
			{
			}
			else if(modelAdapter.ConfirmPassword(textBoxRepositoryLogin.Text, textBoxRepositoryPassword.Text) == false)
			{
			}
			else
			{
				if (radioModify.Checked)
				{
					checkedListReports_2.Items.Clear();
					listBoxReports.Items.Clear();
					modelAdapter.ReportsFoundInFolder(ComputerName, regReportDirectory, textBoxReportingServiceFolder_2.Text);
					base.GoToNextPanel(currentPanel, panel4_2_Reports);
					currentPanel = panel4_2_Reports;
					checkedListReports_2.Focus();
				}
				else
				{
					base.GoToNextPanel(currentPanel, panel4_Reports);
					currentPanel = panel4_Reports;
					checkedListReports.Focus();
				}
			}*/
		}

		public void Panel3_Repository_Next_Helper()
		{
			if (modelAdapter.ValidateAccountName(textBoxRepositoryLogin.Text) == false)
			{
				DisplayMessageBox("Logon ID must be a windows account specified in the form of 'domain\\user'.", "Error!");
			}
			else if(modelAdapter.TestConnection(SqlServerName) == false)
			{
			}
			else if(modelAdapter.ConfirmPassword(textBoxRepositoryLogin.Text, textBoxRepositoryPassword.Text) == false)
			{
			}
			else
			{
				if (radioModify.Checked)
				{
					checkedListReports_2.Items.Clear();
					listBoxReports.Items.Clear();
					modelAdapter.ReportsFoundInFolder(ComputerName, regReportDirectory, textBoxReportingServiceFolder_2.Text);
					base.GoToNextPanel(currentPanel, panel4_2_Reports);
					currentPanel = panel4_2_Reports;
					checkedListReports_2.Focus();
				}
				else
				{
					modelAdapter.AvailableReports();
					base.GoToNextPanel(currentPanel, panel4_Reports);
					currentPanel = panel4_Reports;
					checkedListReports.Focus();
				}
			}
			this.Cursor = Cursors.Default;
		}

		public void Panel4_Reports_Previous()
		{
			this.Cursor = Cursors.WaitCursor;
			base.GoToPreviousPanel(currentPanel, panel3_Repository);
			currentPanel = panel3_Repository;
			textBoxRepositoryServer.Focus();
			this.Cursor = Cursors.Default;
		}

		public void Panel4_Reports_Next()
		{
			this.Cursor = Cursors.WaitCursor;
			if  ( checkedListReports.CheckedItems.Count == 0 )
			{
				DisplayMessageBox("At least one report must be selected.", "Error!");
			}
			else
			{
				base.GoToNextPanel(currentPanel, panel5_Summary);
				currentPanel = panel5_Summary;
				if ( radioUpgrade.Checked )
				{
					labelSummary1.Text = "You have specified the required configuration settings to complete this upgrade. ";
					labelSummaryWarning.Visible = true;
				}
				else
				{
					labelSummary1.Text = "You have specified the required configuration settings to complete this installation. ";
					labelSummaryWarning.Visible = false;
				}
				buttonNext.Enabled = false;
				buttonFinish.Enabled = true;
				this.AcceptButton = buttonFinish;
				buttonFinish.Focus();
			}
			this.Cursor = Cursors.Default;
		}

		public void Panel4_2_Reports_Previous()
		{
			this.Cursor = Cursors.WaitCursor;
			if (radioUpgrade.Checked)
			{
				base.GoToPreviousPanel(currentPanel, panel2_2_ReportingServices);
				currentPanel = panel2_2_ReportingServices;
				textBoxReportingServiceFolder_2.Focus();
			}
			else
			{
				base.GoToPreviousPanel(currentPanel, panel3_Repository);
				currentPanel = panel3_Repository;
				textBoxRepositoryServer.Focus();
			}
			this.Cursor = Cursors.Default;
		}

		public void Panel4_2_Reports_Next()
		{
			this.Cursor = Cursors.WaitCursor;
			if  (checkedListReports_2.CheckedItems.Count == 0)
			{
				DisplayMessageBox("At least one report must be selected.", "Error!");
			}
			else
			{
				base.GoToNextPanel(currentPanel, panel5_Summary);
				currentPanel = panel5_Summary;
				if ( radioUpgrade.Checked )
				{
					labelSummary1.Text = "You have specified the required configuration settings to complete this upgrade. ";
					labelSummaryWarning.Visible = true;
				}
				else
				{
					labelSummary1.Text = "You have specified the required configuration settings to complete this installation. ";
					labelSummaryWarning.Visible = false;
				}
				buttonNext.Enabled = false;
				buttonFinish.Enabled = true;
				this.AcceptButton = buttonFinish;
				buttonFinish.Focus();
			}
			this.Cursor = Cursors.Default;
		}

		public void Panel5_Summary_Previous()
		{
			this.Cursor = Cursors.WaitCursor;
			if (radioModify.Checked || radioUpgrade.Checked)
			{
				base.GoToPreviousPanel(currentPanel, panel4_2_Reports);
				currentPanel = panel4_2_Reports;
				buttonNext.Enabled = true;
				buttonFinish.Enabled = false;
				this.AcceptButton = buttonNext;
				checkedListReports_2.Focus();
			}
			else
			{
				base.GoToPreviousPanel(currentPanel, panel4_Reports);
				currentPanel = panel4_Reports;
				buttonNext.Enabled = true;
				buttonFinish.Enabled = false;
				this.AcceptButton = buttonNext;
				checkedListReports.Focus();
			}
			this.Cursor = Cursors.Default;
		}

		public void Panel5_Summary_Next()
		{
		}

		public void Panel6_Progress_Previous()
		{
			this.Cursor = Cursors.WaitCursor;
			base.GoToPreviousPanel(currentPanel, panel5_Summary);
			currentPanel = panel5_Summary;
			buttonFinish.Enabled = true;
			this.AcceptButton = buttonFinish;
			this.Cursor = Cursors.Default;
		}

		public void Panel6_Progress_Next()
		{
		}

		public void Panel7_Complete_Previous()
		{
		}

		public void Panel7_Complete_Next()
		{
		}

		#endregion

//		private delegate void DoInstall_InstallDelegate();
		private void DoInstall_Install()
		{
//			if (this.InvokeRequired)
//			{
//				this.Invoke(new DoInstall_InstallDelegate(DoInstall_Install),
//					new object[] {});
//				return;
//			}
			//---------------
			// Create Folder
			//---------------
			UpdateInstallerLog("");
			UpdateInstallerLog("Start install"+"\r\n");
			bool createFolder = modelAdapter.CreateFolder(ComputerName,
				textBoxReportingServiceDirectory.Text,
				textBoxReportingServiceFolder.Text);
			if (createFolder)
			{
				//--------------------
				// Create Data Source
				//--------------------
				bool createDataSource = modelAdapter.CreateDataSource(ComputerName,
					textBoxReportingServiceDirectory.Text,
					textBoxReportingServiceFolder.Text,
					SqlServerName, textBoxRepositoryLogin.Text,
					textBoxRepositoryPassword.Text, radioModify.Checked);
				if (createDataSource)
				{
					//-----------------
					// Publish Reports
					//-----------------
					UpdateInstallerLog("Publish Reports\r\n");
					bool deployReports ;
					foreach (string val in checkedListReports.CheckedItems)
					{
						deployReports = modelAdapter.DeployReport(ComputerName, 
							textBoxReportingServiceDirectory.Text, 
							textBoxReportingServiceFolder.Text, val);
						if (!deployReports)
						{
							buttonPrev.Visible = true;
							buttonNext.Visible = true;
							buttonFinish.Visible = true;
							buttonPrev.Enabled = true;
							buttonCancel.Enabled = true;
							this.AcceptButton = buttonPrev;
							this.Cursor = Cursors.Default;
							return;
						}
					}

					//-----------------
					// Create Shortcut
					//-----------------
					linkLabel.Text = modelAdapter.CreateShortcutLink(ComputerName, textBoxReportingServiceManager.Text);
					modelAdapter.CreateShortcut(ComputerName, textBoxReportingServiceManager.Text);

					//--------------------------
					// Create Server and Folder
					//--------------------------
					modelAdapter.InsertServerAndFolder(SqlServerName, ComputerName,
						textBoxReportingServiceFolder.Text);

					modelAdapter.WritePreferences(regReportServerName, textBoxReportingServiceComputer.Text);
					modelAdapter.WritePreferences(regReportFolderName, textBoxReportingServiceFolder.Text);
					modelAdapter.WritePreferences(regReportDirectoryName, textBoxReportingServiceDirectory.Text);
					modelAdapter.WritePreferences(regReportManagerName, textBoxReportingServiceManager.Text);
					modelAdapter.WritePreferences(regSqlServerName, textBoxRepositoryServer.Text);
					modelAdapter.WritePreferences(regLoginName, textBoxRepositoryLogin.Text);
					modelAdapter.WritePreferences(regSslName, ((Boolean)checkBoxReportingServiceSSL_1.Checked).ToString());
					modelAdapter.WritePreferences(regShortcutName, linkLabel.Text);

					buttonCancel.Enabled = true;
					buttonCancel.Text = "&Close";
					base.GoToNextPanel(currentPanel, panel7_Complete);
					currentPanel = panel7_Complete;
					this.AcceptButton = buttonCancel;
					this.Cursor = Cursors.Default;
				}
				else
				{
					buttonPrev.Visible = true;
					buttonNext.Visible = true;
					buttonFinish.Visible = true;
					buttonPrev.Enabled = true;
					buttonCancel.Enabled = true;
					this.AcceptButton = buttonPrev;
					this.Cursor = Cursors.Default;
					return;
				}
			}
			else
			{
				buttonPrev.Visible = true;
				buttonNext.Visible = true;
				buttonFinish.Visible = true;
				buttonPrev.Enabled = true;
				buttonCancel.Enabled = true;
				this.AcceptButton = buttonPrev;
				this.Cursor = Cursors.Default;
				return;
			}
		}

//		private delegate void DoInstall_ModifyDelegate();
		private void DoInstall_Modify()
		{
//			if (this.InvokeRequired)
//			{
//				this.Invoke(new DoInstall_ModifyDelegate(DoInstall_Modify),
//					new object[] {});
//				return;
//			}

			//--------------------
			// Create Data Source
			//--------------------
			bool createDataSource = modelAdapter.CreateDataSource(ComputerName,
				regReportDirectory, textBoxReportingServiceFolder_2.Text,
				SqlServerName, textBoxRepositoryLogin.Text,
				textBoxRepositoryPassword.Text, radioModify.Checked);
			if (createDataSource)
			{
				//-----------------
				// Publish Reports
				//-----------------
				UpdateInstallerLog("Publish Reports\r\n");
				bool deployReports ;
				foreach (string val in checkedListReports_2.CheckedItems)
				{
					deployReports = modelAdapter.DeployReport(ComputerName, 
						regReportDirectory, textBoxReportingServiceFolder_2.Text, val);
					if (!deployReports)
					{
						buttonPrev.Visible = true;
						buttonNext.Visible = true;
						buttonFinish.Visible = true;
						buttonPrev.Enabled = true;
						buttonCancel.Enabled = true;
						this.AcceptButton = buttonPrev;
						this.Cursor = Cursors.Default;
						return;
					}
				}

				//--------------------------
				// Create Server and Folder
				//--------------------------
				modelAdapter.InsertServerAndFolder(SqlServerName, ComputerName,
					textBoxReportingServiceFolder_2.Text);

				modelAdapter.WritePreferences(regReportFolderName, textBoxReportingServiceFolder_2.Text);
				modelAdapter.WritePreferences(regSqlServerName, textBoxRepositoryServer.Text);
				modelAdapter.WritePreferences(regLoginName, textBoxRepositoryLogin.Text);

				buttonCancel.Enabled = true;
				buttonCancel.Text = "&Close";
				base.GoToNextPanel(currentPanel, panel7_Complete);
				currentPanel = panel7_Complete;
				this.AcceptButton = buttonCancel;
				this.Cursor = Cursors.Default;
			}
			else
			{
				buttonPrev.Visible = true;
				buttonNext.Visible = true;
				buttonFinish.Visible = true;
				buttonPrev.Enabled = true;
				buttonCancel.Enabled = true;
				this.AcceptButton = buttonPrev;
				this.Cursor = Cursors.Default;
			}
		}

//		private delegate void DoInstall_UpgradeDelegate();
		private void DoInstall_Upgrade()
		{
//			if (this.InvokeRequired)
//			{
//				this.Invoke(new DoInstall_UpgradeDelegate(DoInstall_Upgrade),
//					new object[] {});
//				return;
//			}
			UpdateInstallerLog("");
			UpdateInstallerLog("Start upgrade"+"\r\n");

			//-----------------
			// Publish Reports
			//-----------------
			UpdateInstallerLog("Publish Reports\r\n");
			bool deployReports ;
			foreach (string val in checkedListReports_2.CheckedItems)
			{
				deployReports = modelAdapter.DeployReport(ComputerName, 
					regReportDirectory, textBoxReportingServiceFolder_2.Text, val);
				if (!deployReports)
				{
					buttonPrev.Visible = true;
					buttonNext.Visible = true;
					buttonFinish.Visible = true;
					buttonPrev.Enabled = true;
					buttonCancel.Enabled = true;
					this.AcceptButton = buttonPrev;
					this.Cursor = Cursors.Default;
					return;
				}
			}

			//--------------------------
			// Create Server and Folder
			//--------------------------
			modelAdapter.InsertServerAndFolder(regSqlServer, ComputerName,
				textBoxReportingServiceFolder_2.Text);

			modelAdapter.WritePreferences(regReportFolderName, textBoxReportingServiceFolder_2.Text);

			buttonCancel.Enabled = true;
			buttonCancel.Text = "&Close";
			base.GoToNextPanel(currentPanel, panel7_Complete);
			currentPanel = panel7_Complete;
			this.AcceptButton = buttonCancel;
			this.Cursor = Cursors.Default;
		}

/*		private delegate void DoInstallDelegate();
		private void DoInstall()
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new DoInstallDelegate(DoInstall),
					new object[] {});
				return;
			}
			if (radioUpgrade.Checked)
			{
				UpdateInstallerLog("");
				UpdateInstallerLog("Start upgrade"+"\r\n");

				//-----------------
				// Publish Reports
				//-----------------
				UpdateInstallerLog("Publish Reports\r\n");
				bool deployReports ;
				foreach (string val in checkedListReports_2.CheckedItems)
				{
					deployReports = modelAdapter.DeployReport(ComputerName, 
						regReportDirectory, textBoxReportingServiceFolder_2.Text, val);
					if (!deployReports)
					{
						buttonPrev.Visible = true;
						buttonNext.Visible = true;
						buttonFinish.Visible = true;
						buttonPrev.Enabled = true;
						buttonCancel.Enabled = true;
						this.AcceptButton = buttonPrev;
						return;
					}
				}

				//--------------------------
				// Create Server and Folder
				//--------------------------
				modelAdapter.InsertServerAndFolder(regSqlServer, ComputerName,
					textBoxReportingServiceFolder_2.Text);

				modelAdapter.WritePreferences(regReportFolderName, textBoxReportingServiceFolder_2.Text);

				buttonCancel.Enabled = true;
				buttonCancel.Text = "&Close";
				base.GoToNextPanel(currentPanel, panel7_Complete);
				currentPanel = panel7_Complete;
				this.AcceptButton = buttonCancel;
			}
			else if (radioModify.Checked)
			{
				//--------------------
				// Create Data Source
				//--------------------
				bool createDataSource = modelAdapter.CreateDataSource(ComputerName,
					regReportDirectory, textBoxReportingServiceFolder_2.Text,
					SqlServerName, textBoxRepositoryLogin.Text,
					textBoxRepositoryPassword.Text, radioModify.Checked);
				if (createDataSource)
				{
					//-----------------
					// Publish Reports
					//-----------------
					UpdateInstallerLog("Publish Reports\r\n");
					bool deployReports ;
					foreach (string val in checkedListReports_2.CheckedItems)
					{
						deployReports = modelAdapter.DeployReport(ComputerName, 
							regReportDirectory, textBoxReportingServiceFolder_2.Text, val);
						if (!deployReports)
						{
							buttonPrev.Visible = true;
							buttonNext.Visible = true;
							buttonFinish.Visible = true;
							buttonPrev.Enabled = true;
							buttonCancel.Enabled = true;
							this.AcceptButton = buttonPrev;
							return;
						}
					}

					//--------------------------
					// Create Server and Folder
					//--------------------------
					modelAdapter.InsertServerAndFolder(SqlServerName, ComputerName,
						textBoxReportingServiceFolder_2.Text);

					modelAdapter.WritePreferences(regReportFolderName, textBoxReportingServiceFolder_2.Text);
					modelAdapter.WritePreferences(regSqlServerName, textBoxRepositoryServer.Text);
					modelAdapter.WritePreferences(regLoginName, textBoxRepositoryLogin.Text);

					buttonCancel.Enabled = true;
					buttonCancel.Text = "&Close";
					base.GoToNextPanel(currentPanel, panel7_Complete);
					currentPanel = panel7_Complete;
					this.AcceptButton = buttonCancel;
				}
			}
			else
			{
				//---------------
				// Create Folder
				//---------------
				UpdateInstallerLog("");
				UpdateInstallerLog("Start install"+"\r\n");
				bool createFolder = modelAdapter.CreateFolder(ComputerName,
					textBoxReportingServiceDirectory.Text,
					textBoxReportingServiceFolder.Text);
				if (createFolder)
				{
					//--------------------
					// Create Data Source
					//--------------------
					bool createDataSource = modelAdapter.CreateDataSource(ComputerName,
						textBoxReportingServiceDirectory.Text,
						textBoxReportingServiceFolder.Text,
						SqlServerName, textBoxRepositoryLogin.Text,
						textBoxRepositoryPassword.Text, radioModify.Checked);
					if (createDataSource)
					{
						//-----------------
						// Publish Reports
						//-----------------
						UpdateInstallerLog("Publish Reports\r\n");
						bool deployReports ;
						foreach (string val in checkedListReports.CheckedItems)
						{
							deployReports = modelAdapter.DeployReport(ComputerName, 
								textBoxReportingServiceDirectory.Text, 
								textBoxReportingServiceFolder.Text, val);
							if (!deployReports)
							{
								buttonPrev.Visible = true;
								buttonNext.Visible = true;
								buttonFinish.Visible = true;
								buttonPrev.Enabled = true;
								buttonCancel.Enabled = true;
								this.AcceptButton = buttonPrev;
								return;
							}
						}

						//-----------------
						// Create Shortcut
						//-----------------
						linkLabel.Text = modelAdapter.CreateShortcut(ComputerName, textBoxReportingServiceManager.Text);

						//--------------------------
						// Create Server and Folder
						//--------------------------
						modelAdapter.InsertServerAndFolder(SqlServerName, ComputerName,
							textBoxReportingServiceFolder.Text);

						modelAdapter.WritePreferences(regReportServerName, textBoxReportingServiceComputer.Text);
						modelAdapter.WritePreferences(regReportFolderName, textBoxReportingServiceFolder.Text);
						modelAdapter.WritePreferences(regReportDirectoryName, textBoxReportingServiceDirectory.Text);
						modelAdapter.WritePreferences(regReportManagerName, textBoxReportingServiceManager.Text);
						modelAdapter.WritePreferences(regSqlServerName, textBoxRepositoryServer.Text);
						modelAdapter.WritePreferences(regLoginName, textBoxRepositoryLogin.Text);
						modelAdapter.WritePreferences(regSslName, ((Boolean)checkBoxReportingServiceSSL_1.Checked).ToString());
						modelAdapter.WritePreferences(regShortcutName, linkLabel.Text);

						buttonCancel.Enabled = true;
						buttonCancel.Text = "&Close";
						base.GoToNextPanel(currentPanel, panel7_Complete);
						currentPanel = panel7_Complete;
						this.AcceptButton = buttonCancel;
					}
				}
			}
		}
*/
		private void menuExit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void menuAbout_Click(object sender, EventArgs e)
		{
			aboutForm.ShowDialog(this);
		}

		private void buttonReportingServiceDefault_Click(object sender, EventArgs e)
		{
			textBoxReportingServiceComputer.Text = "localhost";
			textBoxReportingServiceFolder.Text = folder;
			textBoxReportingServiceDirectory.Text = "reportServer";
			textBoxReportingServiceManager.Text = "reports";
			textBoxReportingServiceComputer.Focus();
		}

		private void buttonRepositoryBrowse_Click(object sender, EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			modelAdapter.ListAvailableServers();
			this.Cursor = Cursors.Default;
			sqlInstanceForm.ShowDialog(this);
		}

		private void buttonReportsSelect_Click(object sender, EventArgs e)
		{
			for ( int i=0; i < checkedListReports.Items.Count; i++ )
			{
				checkedListReports.SetItemChecked(i, true );
			}
		}

		private void buttonReportsUnselect_Click(object sender, EventArgs e)
		{
			for ( int i=0; i < checkedListReports.Items.Count; i++ )
			{
				checkedListReports.SetItemChecked(i, false);
			}
		}

		private void buttonBrowse_2_Click(object sender, EventArgs e)
		{
			computerName = modelAdapter.ParseHostSite(regReportServer);
			this.Cursor = Cursors.WaitCursor;
			if (Boolean.Parse(regSsl))
			{
				modelAdapter.SetSslProxy();
			}
			else
			{
				modelAdapter.SetNormalProxy();
			}
			modelAdapter.PopulateFolderList(ComputerName, regReportDirectory);
			this.Cursor = Cursors.Default;
			folderForm.ShowDialog(this);
		}

		private void buttonSelectAll_2_Click(object sender, EventArgs e)
		{
			for ( int i=0; i < checkedListReports_2.Items.Count; i++ )
			{
				checkedListReports_2.SetItemChecked(i, true);
			}
		}

		private void buttonUnselectAll_2_Click(object sender, EventArgs e)
		{
			for ( int i=0; i < checkedListReports_2.Items.Count; i++ )
			{
				checkedListReports_2.SetItemChecked(i, false);
			}
		}

		private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(linkLabel.Text);  
		}

		private void radioInstall_CheckedChanged(object sender, System.EventArgs e)
		{
			currentState = installState;
		}

		private void radioModify_CheckedChanged(object sender, System.EventArgs e)
		{
			currentState = modifyState;
		}

		private void radioUpgrade_CheckedChanged(object sender, System.EventArgs e)
		{
			currentState = upgradeState;
		}
	}
}
