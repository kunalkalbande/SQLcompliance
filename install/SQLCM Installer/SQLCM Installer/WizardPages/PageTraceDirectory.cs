using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SQLCM_Installer.WizardPages
{
    [WizardPageInfo(WizardPage.TraceDirectory)]
    internal partial class PageTraceDirectory : WizardPageBase
    {
        public PageTraceDirectory(MainForm host)
            : base(host)
        {
            InitializeComponent();
        }

        internal override void Initialize()
        {
            txtAgentTraceDirectory.Text = InstallProperties.CMInstallDir + "\\" + "AgentTraceFiles";
            txtCollectionTraceDirectory.Text = InstallProperties.CMInstallDir + "\\" + "CollectionServerTraceFiles";
            IsInitialized = true;
        }

        internal override void OnNavigated(NavigationDirection direction)
        {
            if (Constants.UserInstallType == InstallType.AgentAndDashboard || Constants.UserInstallType == InstallType.AgentOnly)
            {
                panelAgentTrace.Visible = true;
                panelCollectionTrace.Visible = false;

                panelAgentTrace.Location = new Point(0, 64);
                labelSubHeader.Text = "Where do you want the " + Constants.ProductMap[Products.Agent] + " Trace Directory?";
            }
            else
            {
                panelAgentTrace.Visible = false;
                panelCollectionTrace.Visible = true;

                panelCollectionTrace.Location = new Point(0, 64);
                labelSubHeader.Text = "Where do you want the SQL Compliance Manager Collection Trace Directory?";
            }
        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            string traceDirectory;
            string rootUserDrive;

            errorMessage = null;
            invalidControl = null;
            if (panelCollectionTrace.Visible)
            {
                traceDirectory = txtCollectionTraceDirectory.Text;
                Constants.SetDirectoryList(txtCollectionTraceDirectory.Name, traceDirectory);
                if (traceDirectory.Equals(String.Empty))
                {
                    errorMessage = Constants.DirectoryPathError;
                    return false;
                }
                rootUserDrive = Path.GetPathRoot(traceDirectory);
                if (!Directory.Exists(rootUserDrive))
                {
                    errorMessage = Constants.CollectionTraceDirError;
                    return false;
                }
                else
                {
                    try
                    {
                        Directory.CreateDirectory(Path.Combine(traceDirectory, "TempDir"));
                        Directory.Delete(Path.Combine(traceDirectory, "TempDir"));
                    }
                    catch
                    {
                        errorMessage = Constants.CollectionTraceDirError;
                        return false;
                    }
                }
                InstallProperties.CollectionTraceDirectory = traceDirectory;
                InstallProperties.AgentTraceDirectory = InstallProperties.CMInstallDir + "\\" + "AgentTraceFiles";
            }
            else
            {
                traceDirectory = txtAgentTraceDirectory.Text;
                Constants.SetDirectoryList(txtAgentTraceDirectory.Name, traceDirectory);
                if (traceDirectory.Equals(String.Empty))
                {
                    errorMessage = Constants.DirectoryPathError;
                    return false;
                }
                rootUserDrive = Path.GetPathRoot(traceDirectory);
                if (!Directory.Exists(rootUserDrive))
                {
                    errorMessage = Constants.AgentTraceDirError;
                    return false;
                }
                else
                {
                    try
                    {
                        Directory.CreateDirectory(Path.Combine(traceDirectory, "TempDir"));
                        Directory.Delete(Path.Combine(traceDirectory, "TempDir"));
                    }
                    catch
                    {
                        errorMessage = Constants.AgentTraceDirError;
                        return false;
                    }
                }
                InstallProperties.AgentTraceDirectory = traceDirectory;
            }
            return true;
        }

        private void agentBrowseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            DialogResult result = folderDlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                txtAgentTraceDirectory.Text = folderDlg.SelectedPath;
                Environment.SpecialFolder root = folderDlg.RootFolder;
            }
        }

        private void collectionBrowseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            DialogResult result = folderDlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                txtCollectionTraceDirectory.Text = folderDlg.SelectedPath;
                Environment.SpecialFolder root = folderDlg.RootFolder;
            }
        }
    }
}
