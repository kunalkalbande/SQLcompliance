
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.Properties;
using Idera.SQLcompliance.Application.GUI.ReportService2005;
using Idera.SQLcompliance.Core.Reports;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   /// <summary>
   /// Summary description for Form_DeployReportsWizard.
   /// </summary>
   public partial class Form_DeployReportsWizard : Form
   {
      private static readonly string[] _wizardTitles = new string[] { "Deploy Reports",
                                                                       "Connect to Reporting Services",
                                                                       "SQL Compliance Manager Repository",
                                                                       "Report Deployment Location", 
                                                                       "Summary", 
                                                                       "Deployment Status"};
      private static readonly string[] _wizardDescriptions = new string[] { "Deploy reports to Reporting Services 2005 or later.", 
                                                                             "Specify the Report Server computer for hosting the SQL Compliance Manager reports.",
                                                                             "Specify the location and credentials for the SQL Compliance Manager Repository.",
                                                                             "Specify the location for the deployed reports.",
                                                                             "Review the deployment configuration and select next to initiate deployment.",
                                                                             ""} ;

      private static readonly string _reportWebService = "ReportService2005.asmx";

      private int _pageIndex ;
      private int _pageCount ;
      private ReportingService2005 _rs;
      private ReportsRecord _drd;
      private List<CMReport> _reports;
      private bool _hasErrors;

      private const string PRIVILEGED_TRUSTED_USERS_REPORT = "Privileged / Trusted Users";
      private const string SENSITIVE_COLUMNS_BAD_REPORT = "Sensitive Columns / BAD";
      public Form_DeployReportsWizard()
      {
         InitializeComponent();

         this.Icon = Resources.SQLcompliance_product_ico;

         _pageIndex = 0 ;
         _pageCount = 6 ;

         _lblTitle.Text = _wizardTitles[_pageIndex] ;
         _lblDescription.Text = _wizardDescriptions[_pageIndex] ;

         _drd = ReportsRecord.Read(Globals.Repository.Connection);

         Scatter();
         LoadReports() ;
         // We deploy one set of reports and 8 versions of linked reports
         _progressBar.Minimum = 0;
         _progressBar.Maximum = _reports.Count * 9; 
      }

      private void LoadReports()
      {
         FileInfo fInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
         string filePath = Path.Combine(fInfo.DirectoryName, CMReport.RelativeReportsPath);
         _reports = ReportXmlHelper.LoadReportListFromXmlFile(Path.Combine(filePath, CMReport.RDLXmlFileName));
      }

      private void VerifyConnection()
      {
         try
         {
            Gather();
            _rs = new ReportingService2005();
            _rs.UseDefaultCredentials = true;
            _rs.Url = String.Format("{0}/{1}", _drd.GetReportServerUrl(), _reportWebService);
            _rs.GetPermissions("/");
         }
         catch (Exception)
         {
            _rs = null;
            throw;
         }
      }

      private void ShowPage(int pageNumber)
      {
         if(pageNumber < 0 || pageNumber >= _pageCount)
         {
            throw new Exception("Invalid page number"); 
         }
         if(pageNumber == _pageIndex)
            return ;

         if(pageNumber == 0)
         {
            _btnBack.Enabled = false ;
            _btnNext.Enabled = true ;
            _btnFinish.Enabled = false ;
         }
         else if(pageNumber == (_pageCount - 1))
         {
            _btnBack.Enabled = false;
            _btnFinish.Enabled = false;
            _btnNext.Enabled = false;
         }
         else
         {
            _btnBack.Enabled = true ;
            _btnNext.Enabled = true ;
            _btnFinish.Enabled = false ;
         }

         _lblTitle.Text = _wizardTitles[pageNumber] ;
         _lblDescription.Text = _wizardDescriptions[pageNumber] ;

         switch(pageNumber)
         {
            case 0:
               _pnlIntro.Show();
               break ;
            case 1:
               _pnlReportServer.Show();
               break;
            case 2:
               _pnlRepository.Show();
               break;
            case 3:
               _pnlSelectReports.Show();
               break; 
            case 4:
               BuildSummary();
               _pnlSummary.Show();
               break;
            case 5:
               _pnlStatus.Show();
               break;
         }

         switch(_pageIndex)
         {
            case 0:
               _pnlIntro.Hide();
               break ;
            case 1:
               _pnlReportServer.Hide();
               break;
            case 2:
               _pnlRepository.Hide();
               break;
            case 3:
               _pnlSelectReports.Hide();
               break; 
            case 4:
               _pnlSummary.Hide();
               // Only deploy if they hit next.
               if(_pageIndex < pageNumber)
                  DeployReports();
               break;
            case 5:
               _pnlStatus.Hide() ;
               break ;
         }
         _pageIndex = pageNumber ;
         if (_pageIndex == (_pageCount - 1))
         {
            AcceptButton = _btnFinish;
            _btnFinish.Focus();
         }
         else
         {
            AcceptButton = _btnNext;
            _btnNext.Focus();
         }
      }

      private bool ValidatePage(int page)
      {
         Cursor = Cursors.WaitCursor;
         try
         {
            switch (page)
            {
               case 0:
                  return true;
               case 1:
                  return ValidateReportServerPage();
               case 2:
                  return ValidateRepositoryPage();
               case 3:
                  return ValidateTargetFolder();
               case 4:
                  return true;
            }
            return false;
         }
         finally
         {
            Cursor = Cursors.Default;
         }
      }

      private bool ValidateReportServerPage()
      {
         try
         {
            if (_tbReportServer.Text.Trim().Length == 0)
            {
               MessageBox.Show(this, "Please specify a report server.", "Report Server Missing");
               return false;
            }
            if (_tbPort.Text.Trim().Length == 0)
            {
               MessageBox.Show(this, "Please specify a port number.", "Port Number Missing");
               _cbAdvancedConnectionOptions.Checked = true;
               return false;
            }
            if (_tbRSVirtualDirectory.Text.Trim().Length == 0)
            {
               MessageBox.Show(this, "Please specify a virtual directory.", "Virtual Directory Missing");
               _cbAdvancedConnectionOptions.Checked = true;
               return false;
            }
            VerifyConnection();
            return true;
         }
         catch (Exception e)
         {
            string errorMessage = String.Format("Unable to connect to report server: {0}\r\n{1}", _drd.GetReportServerUrl(), e.Message);
            MessageBox.Show(this, errorMessage, "Connection Failed");
            return false;
         }
      }

      private bool ValidateRepositoryPage()
      {
         try
         {
            if (_tbRepository.Text.Trim().Length == 0)
            {
               MessageBox.Show(this, "Please specify the Repository server instance.", "Repository Server Missing");
               return false;
            }
            if (_tbLogin.Text.Trim().Length == 0)
            {
               MessageBox.Show(this, "Please specify a login.", "Login Missing");
               return false;
            }
            if (!_tbPassword.Text.Equals(_tbConfirmPassword.Text))
            {
               MessageBox.Show(this, "The passwords do not match.", "Password Mismatch");
               return false;
            }
            return VerifyRepositoryConnection() && ValidateLogin();
         }
         catch (Exception e)
         {
            string errorMessage = String.Format("Unable to connect to Repository: {0}\r\n{1}", _drd.Repository, e.Message);
            MessageBox.Show(this, errorMessage, "Connection Failed");
            return false;
         }
      }

      private bool ValidateTargetFolder()
      {
         if (_tbTargetFolder.Text.Trim().Length == 0)
         {
            MessageBox.Show(this, "Please specify a target folder for the reports.", "Target Folder Missing");
            return false;
         }
         return true;
      }

      private bool VerifyRepositoryConnection()
      {
         string connStr = String.Format("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=SQLcompliance;Data Source={0}", _tbRepository.Text) ;
         try
         {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
               conn.Open();
            }
            return true;
         }
         catch (Exception e)
         {
            string errorMessage = String.Format("Unable to connect to Repository: {0}\r\n{1}", _tbRepository.Text, e.Message);
            MessageBox.Show(this, errorMessage, "Connection Failed");
            return false;
         }
      }

      private bool ValidateLogin()
      {
         return true;
      }

      #region Events

      private void Click_btnBack(object sender, EventArgs e)
      {
         ShowPage(_pageIndex - 1) ;
      }

      private void Click_btnFinish(object sender, EventArgs e)
      {
      }

      private void Click_btnCancel(object sender, EventArgs e)
      {
         if (_bgWorker.IsBusy)
            _bgWorker.CancelAsync();
      }

      private void Click_btnNext(object sender, EventArgs e)
      {
         if(ValidatePage(_pageIndex))
            ShowPage(_pageIndex + 1) ;
      }

      #endregion // Events

      private void Form_DeployReportsWizard_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         string helpTopic;

         if (_pageIndex == 0)
            helpTopic = HelpAlias.SSHELP_DeployReportsWizard_Welcome;
         else if (_pageIndex == 1)
            helpTopic = HelpAlias.SSHELP_DeployReportsWizard_Connect;
         else if (_pageIndex == 2)
            helpTopic = HelpAlias.SSHELP_DeployReportsWizard_Repository;
         else if (_pageIndex == 3)
            helpTopic = HelpAlias.SSHELP_DeployReportsWizard_Location;
         else if (_pageIndex == 4)
            helpTopic = HelpAlias.SSHELP_DeployReportsWizard_Summary;
         else
            return;

         HelpAlias.ShowHelp(this, helpTopic);
         hlpevent.Handled = true;
      }

      private void CheckedChanged_cbAdvancedConnectionOptions(object sender, EventArgs e)
      {
         _groupReportServerSettings.Visible = _cbAdvancedConnectionOptions.Checked;
         _groupReportManager.Visible = _cbAdvancedConnectionOptions.Checked;
      }

      private void Scatter()
      {
         _cbOverwriteExisting.Checked = _drd.OverwriteExisting;
         _tbPort.Text = _drd.Port.ToString();
         _tbReportServer.Text = _drd.ReportServer;
         _tbRepository.Text = _drd.Repository;
         _tbTargetFolder.Text = _drd.TargetDirectory;
         _tbLogin.Text = _drd.UserName;
         _cbUseSsl.Checked = _drd.UseSsl;
         _tbRMVirtualDirectory.Text = _drd.ReportManagerDirectory;
         _tbRSVirtualDirectory.Text = _drd.ReportServerDirectory;
         if (_drd.IsAdvancedConnection())
         {
            _cbAdvancedConnectionOptions.Checked = true;
         }
      }

      private void Gather()
      {
         _drd.OverwriteExisting = _cbOverwriteExisting.Checked;
         _drd.Password = _tbPassword.Text;
         _drd.Port = Int32.Parse(_tbPort.Text);
         _drd.ReportServer = _tbReportServer.Text;
         _drd.Repository = _tbRepository.Text;
         _drd.TargetDirectory = _tbTargetFolder.Text;
         _drd.UserName = _tbLogin.Text;
         _drd.UseSsl = _cbUseSsl.Checked;
         _drd.ReportServerDirectory = _tbRSVirtualDirectory.Text;
         _drd.ReportManagerDirectory = _tbRMVirtualDirectory.Text;
      }

      private void DeployReports()
      {
         Gather();

         _btnBack.Enabled = false;
         _progressBar.Visible = true;
         _lblFinalStatusMessage.Visible = false;
         _linkFinalLocation.Visible = false;
         _progressBar.Value = 0;
         _bgWorker.RunWorkerAsync();
         Cursor = Cursors.WaitCursor;
      }

      private void DeployLinkedReports(List<CMReport> reports, string parentReportsLocation,
         string linkedReportsLocation, int dateTypeValue)
      {
         string targetLocation = CreateTargetLocation(linkedReportsLocation);
         foreach (CMReport report in reports)
         {
            string newReport = linkedReportsLocation + "/" + report.Name;

            if (_bgWorker.CancellationPending)
               return;

            // Skip this report when deploying linked - it does not have dates
            if (report.FileName.Equals("SQLcmReportAlertRules.rdl"))
               continue;

            // See if we should over
            if (ItemExists(newReport, ItemTypeEnum.LinkedReport))
            {
               if (_drd.OverwriteExisting)
               {
                  _rs.DeleteItem(newReport);
               }
               else
               {
                  _bgWorker.ReportProgress(1, String.Format("Skipping linked report: {0}...", linkedReportsLocation + "/" + report.Name));
                  continue;
               }
            }
            // Create the report
            _bgWorker.ReportProgress(1, String.Format("Deploying linked report: {0}...", linkedReportsLocation + "/" + report.Name));
            _rs.CreateLinkedReport(report.Name, targetLocation,
               parentReportsLocation + "/" + report.Name, null);

            // Fix the parameters
            ReportParameter[] parameters = _rs.GetReportParameters(newReport, null, false, null, null);

            foreach (ReportParameter parameter in parameters)
            {
               if (parameter.Name == "dateType")
               {
                  parameter.DefaultValues[0] = dateTypeValue.ToString();
               }
               else if (parameter.Name == "startDate")
               {
                  parameter.PromptUser = false;
                  parameter.PromptUserSpecified = true;
               }
               else if (parameter.Name == "endDate")
               {
                  parameter.PromptUser = false;
                  parameter.PromptUserSpecified = true;
               }
            }
            _rs.SetReportParameters(newReport, parameters);
         }
      }

      private void DeployReport(string targetLocation, CMReport report)
      {
         byte[] reportDef;
         Property prop = new Property();
         prop.Name = "Description";
         prop.Value = report.Description;

         if (report.Name == PRIVILEGED_TRUSTED_USERS_REPORT || report.Name == SENSITIVE_COLUMNS_BAD_REPORT)
         {
            report.Name = report.Name.Replace(@"/", "and");
         }
         // Don't overwrite if exists
         if (ItemExists(targetLocation + "/" + report.Name, ItemTypeEnum.Report) && !_drd.OverwriteExisting)
         {
            _bgWorker.ReportProgress(1, String.Format("Skipping report: {0} - Report already exists.", report.Name));
            return;
         }else
            _bgWorker.ReportProgress(1, String.Format("Deploying report: {0}...", report.Name));
         using (FileStream stream = File.OpenRead(report.FileName))
         {
            reportDef = new Byte[stream.Length];
            stream.Read(reportDef, 0, (int) stream.Length);
         }


         _rs.CreateReport(report.Name, targetLocation, _drd.OverwriteExisting,
            reportDef, new Property[] { prop });
      }

      private string CreateTargetLocation(string folderName)
      {
         string[] folders = folderName.Split(new char[] { '/', '\\' });

         string currentLocation = "/";
         foreach (string fld in folders)
         {
            string folder = fld.Trim();
            if (folder.Length == 0)
               continue;

            string tmpLocation;
            if(currentLocation.Length == 1)
               tmpLocation = currentLocation + folder ;
            else
               tmpLocation = currentLocation + "/" + folder;
            if (!ItemExists(tmpLocation, ItemTypeEnum.Folder))
            {
               _rs.CreateFolder(folder, currentLocation, null);
            }
            if (currentLocation.Length == 1)
               currentLocation += folder;
            else
               currentLocation += "/" + folder;
         }

         return currentLocation;
      }

      private void CreateDataSource(string location)
      {
         Property prop = new Property();
         prop.Name = "Description";
         prop.Value = "Data source for SQL Compliance Manager Reports.";

         // Define the data source definition.
         DataSourceDefinition definition = new DataSourceDefinition();
         definition.CredentialRetrieval = CredentialRetrievalEnum.Store;
         definition.ConnectString = "data source=" + _drd.Repository + ";initial catalog=SQLcompliance";
         definition.Enabled = true;
         definition.EnabledSpecified = true;
         definition.Extension = "SQL";
         definition.UserName = _drd.UserName;
         definition.Password = _drd.Password;
         definition.Prompt = null;
         definition.WindowsCredentials = true;

         _rs.CreateDataSource("SQL Compliance Manager Data Source", location, true, definition, new Property[] { prop });
      }

      private bool ItemExists(string item, ItemTypeEnum itemType)
      {
         ItemTypeEnum retType = _rs.GetItemType(item);
         return (itemType == retType);
      }

      private void Form_DeployReportsWizard_FormClosing(object sender, FormClosingEventArgs e)
      {
         if (DialogResult == DialogResult.OK && _pageIndex < _pageCount - 1)
            e.Cancel = true;
         else
         {
            if (_bgWorker.IsBusy)
               _bgWorker.CancelAsync();
         }
      }

      private void DoWork_bgWorker(object sender, System.ComponentModel.DoWorkEventArgs e)
      {
         try
         {
            _hasErrors = false;
            string targetLocation = CreateTargetLocation(_drd.TargetDirectory);
            string reportLocation = CreateTargetLocation(targetLocation + "/Anytime");
            CreateDataSource(reportLocation);
            foreach (CMReport report in _reports)
            {
               try
               {
                  if (_bgWorker.CancellationPending) return;
                  DeployReport(reportLocation, report);
               }
               catch (Exception ex)
               {
                  _bgWorker.ReportProgress(1, ex);
               }
            }

            CreateTargetLocation(targetLocation + "/Daily");
            DeployLinkedReports(_reports, reportLocation, targetLocation + "/Daily/Yesterday", 1);
            DeployLinkedReports(_reports, reportLocation, targetLocation + "/Daily/Today", 4);
            CreateTargetLocation(targetLocation + "/Weekly");
            DeployLinkedReports(_reports, reportLocation, targetLocation + "/Weekly/Last Week", 2);
            DeployLinkedReports(_reports, reportLocation, targetLocation + "/Weekly/This Week", 5);
            CreateTargetLocation(targetLocation + "/Monthly");
            DeployLinkedReports(_reports, reportLocation, targetLocation + "/Monthly/Last Month", 3);
            DeployLinkedReports(_reports, reportLocation, targetLocation + "/Monthly/This Month", 6);
            CreateTargetLocation(targetLocation + "/Quarterly");
            DeployLinkedReports(_reports, reportLocation, targetLocation + "/Quarterly/Last Quarter", 8);
            DeployLinkedReports(_reports, reportLocation, targetLocation + "/Quarterly/This Quarter", 7);
         }
         catch (Exception ex1)
         {
            _bgWorker.ReportProgress(0, ex1);
         }
      }

      private void ProgressChanged_bgWorker(object sender, System.ComponentModel.ProgressChangedEventArgs e)
      {
         string message;
         if (e.UserState is Exception)
         {
            Exception ex = (Exception)e.UserState;
            message = "Error:  " + ex.Message;
            _hasErrors = true;
         }
         else
         {
            message = e.UserState.ToString();
         }
         _tbStatus.Text += String.Format("{0}\r\n", message);
         _tbStatus.SelectionStart = _tbStatus.Text.Length;
         _tbStatus.SelectionLength = 0;
         _tbStatus.ScrollToCaret();
         _progressBar.Value++;
      }

      private void RunWorkerCompleted_bgWorker(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
      {
         Cursor = Cursors.Default;
         if (e.Cancelled)
         {
            _btnBack.Enabled = true;
            _lblFinalStatusMessage.Text = "You have cancelled deployment of the SQL Compliance Manager reports.";
         }
         else
         {
            _btnBack.Visible = false;
            _btnFinish.Visible = false;
            _btnNext.Visible = false;
            _btnCancel.Text = "OK";
            if (_hasErrors)
               _lblFinalStatusMessage.Text = "Errors occurred during deployment.  Please check the log for more information.";
            else
               _lblFinalStatusMessage.Text = "You have successfully deployed the SQL Compliance Manager reports.";
            _linkFinalLocation.Text = "View Deployed Reports" ;
            _linkFinalLocation.Visible = true;
            _linkFinalLocation.Links[0].LinkData = String.Format("{0}", _drd.GetReportManagerUrl(true));
            _drd.Write(Globals.Repository.Connection);
         }
         _lblFinalStatusMessage.Visible = true;
         _progressBar.Visible = false;
      }

      private void LinkClicked_linkFinalLocation(object sender, LinkLabelLinkClickedEventArgs e)
      {
         HelpAlias.LaunchWebBrowser(e.Link.LinkData.ToString());
      }

      private void BuildSummary()
      {
         _lblSumReportServer.Text = _tbReportServer.Text;
         _lblSumPort.Text = _tbPort.Text;
         _lblSumUseSSL.Text = _cbUseSsl.Checked.ToString();
         _lblSumRSDirectory.Text = _tbRSVirtualDirectory.Text;
         _lblSumRMDirectory.Text = _tbRMVirtualDirectory.Text;
         _lblSumRepository.Text = _tbRepository.Text;
         _lblSumRepositoryLogin.Text = _tbLogin.Text;
         _lblSumTargetFolder.Text = _tbTargetFolder.Text;
         _lblSumOverwrite.Text = _cbOverwriteExisting.Checked.ToString();
      }
   }
}
