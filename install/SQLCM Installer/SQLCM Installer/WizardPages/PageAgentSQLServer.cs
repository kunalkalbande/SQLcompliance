using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using SQLCM_Installer.Custom_Prompts;

namespace SQLCM_Installer.WizardPages
{
    [WizardPageInfo(WizardPage.AgentSQLServer)]
    internal partial class PageAgentSQLServer : WizardPageBase
    {
        List<string> localSQLServerInstanceList;
        public PageAgentSQLServer(MainForm host)
            : base(host)
        {
            InitializeComponent();
        }

        internal override void Initialize()
        {
            IsInitialized = true;
        }

        internal override void OnNavigated(NavigationDirection direction)
        {
            labelWindowsUserName.Text = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            GetLocalSQLServerInstanceList();
            dropdownSQLServerInstance.RemoveItems();
            dropdownSQLServerInstance.SetItems(localSQLServerInstanceList);

            if (string.IsNullOrEmpty(InstallProperties.CMSQLServerInstanceName))
            {
                dropdownSQLServerInstance.SetDefaultItem(localSQLServerInstanceList[0]);
                InstallProperties.CMSQLServerInstanceName = dropdownSQLServerInstance.SelectedItem;
            }
            else
            {
                dropdownSQLServerInstance.SetDefaultItem(InstallProperties.CMSQLServerInstanceName);
            }
        }

        private void dropdownSQLServerInstance_Leave(object sender, EventArgs e)
        {
            InstallProperties.CMSQLServerInstanceName = dropdownSQLServerInstance.SelectedItem;
        }

        private void linkLabelChangeCred_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FormCMSQLServerCred formCMSQLServerCred = new FormCMSQLServerCred();
            formCMSQLServerCred.ShowDialog();
            if (InstallProperties.CMSQLServerUsername.Equals(string.Empty) && InstallProperties.CMSQLServerPassword.Equals(string.Empty))
            {
                labelWindowsUserName.Visible = false;
                radioWindowsAuth.Checked = true;
                radioSQLServerAuth.Checked = false;
            }
            else
            {
                labelSQLUserName.Visible = true;
                labelSQLUserName.Text = InstallProperties.CMSQLServerUsername;
                linkLabelChangeCred.Visible = true;
                labelWindowsUserName.Visible = false;
            }
        }

        private void radioSQLServerAuth_Click(object sender, EventArgs e)
        {
            if (radioSQLServerAuth.Checked)
            {
                FormCMSQLServerCred formCMSQLServerCred = new FormCMSQLServerCred();
                formCMSQLServerCred.ShowDialog();
                if (InstallProperties.CMSQLServerUsername.Equals(string.Empty) && InstallProperties.CMSQLServerPassword.Equals(string.Empty))
                {
                    labelWindowsUserName.Visible = false;
                    radioWindowsAuth.Checked = true;
                    radioSQLServerAuth.Checked = false;
                }
                else
                {
                    labelSQLUserName.Visible = true;
                    labelSQLUserName.Text = InstallProperties.CMSQLServerUsername;
                    linkLabelChangeCred.Visible = true;
                    labelWindowsUserName.Visible = false;
                }
            }
            else
            {
                labelWindowsUserName.Visible = true;
                labelSQLUserName.Visible = false;
                linkLabelChangeCred.Visible = false;
                InstallProperties.IsCMSQLServerSQLAuth = false;
                InstallProperties.CMSQLServerUsername = "";
                InstallProperties.CMSQLServerPassword = "";
            }
        }

        public void GetLocalSQLServerInstanceList()
        {
            localSQLServerInstanceList = new List<string>();
            string fullSQLServerInstance = "";

            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey instanceKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);
                if (instanceKey != null)
                {
                    foreach (var instanceName in instanceKey.GetValueNames())
                    {
                        if (!instanceName.Equals("MSSQLSERVER"))
                        {
                            fullSQLServerInstance = Environment.MachineName + "\\" + instanceName;
                        }
                        else
                        {
                            fullSQLServerInstance = Environment.MachineName;
                        }
                        localSQLServerInstanceList.Add(fullSQLServerInstance);
                    }
                }
            }
        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            errorMessage = null;
            invalidControl = null;

            this.Cursor = Cursors.WaitCursor;

            InstallProperties.CMSQLServerInstanceName = dropdownSQLServerInstance.SelectedItem;

            if (!ValidateAgentInstanceCred(out errorMessage))
            {
                this.Cursor = Cursors.Default;
                return false;
            }
            HelperFunctions helperFunction = new HelperFunctions();
            if (!helperFunction.GetSQLServerServiceAccount(InstallProperties.IsCMSQLServerSQLAuth, InstallProperties.CMSQLServerInstanceName, InstallProperties.CMSQLServerUsername, InstallProperties.CMSQLServerPassword, out errorMessage))
            {
                this.Cursor = Cursors.Default;
                return false;
            }

            this.Cursor = Cursors.Default;
            return true;
        }

        private bool ValidateAgentInstanceCred(out string errorMessage)
        {
            HelperFunctions helperFunction = new HelperFunctions();
            if (!helperFunction.CheckServerConnection(InstallProperties.IsCMSQLServerSQLAuth, InstallProperties.CMSQLServerInstanceName, InstallProperties.CMSQLServerUsername, InstallProperties.CMSQLServerPassword, out errorMessage))
            {
                errorMessage = "Connection failed for " + Constants.ProductMap[Products.Compliance] + " Repository. Please check Server Instance and Authentication.";
                return false;
            }
            return true;
        }
    }
}
