using Microsoft.Win32;
using SQLcomplianceCwfAddin.RestService;
using System;
using System.Windows.Forms;

namespace CwfAddinInstaller.WizardPages
{
    [WizardPageInfo("Let’s register SQL Compliance Manager to \r\nIDERA Dashboard", "Enter the display name to be used. Display Name will be displayed as an identifier for the SQL Compliance Manager installation. For example, SQLCM-Prod, SQLCM-North…", WizardPage.Install)]
    internal partial class PageInstall : WizardPageBase
    {
        private readonly Version m_installerVersion = new Version(RestServiceConstants.ProductVersion);
        public static string SQLCMSQLServerInstanceName = string.Empty;

        public PageInstall(FormInstaller host): base(host)
        {
            InitializeComponent();
            Host.OnLogAppended += Host_OnLogAppended;
        }

        ~PageInstall()
        {
            Host.OnLogAppended -= Host_OnLogAppended;
        }

        internal override void OnNavigated(NavigationDirection direction)
        {
            txtLog.Clear();
            Host.lblPageTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Host.lblPageDescription.Location = new System.Drawing.Point(173, 45);
            Host.lblPageDescription.BringToFront();

            SQLCMSQLServerInstanceName = GetSQLCMSQLServerNameFromRegistry();
            Host.CwfHelper.RepositoryInstance = SQLCMSQLServerInstanceName;
            if (SQLCMSQLServerInstanceName == "")
            {
                Host.NavigateToPage(NavigationDirection.Finish);
            }

            progInstall.Visible = false;
            if (direction != NavigationDirection.Next)
                return;

            try
            {
                var upgradingProducts = Host.CwfHelper.GetUpdatingRegisteredSqlCmProductsWithLowerVersionThen(m_installerVersion);
                cboInstances.DataSource = null;
                string repositoryInstance = Host.CwfHelper.RepositoryInstance.ToUpper();
                repositoryInstance = repositoryInstance.Replace(".", Environment.MachineName).Replace("(LOCAL)", Environment.MachineName);
                if (upgradingProducts.Count > 0)
                {
                    foreach (var upgradeProduct in upgradingProducts)
                    {
                        if ((!Host.CwfHelper.IsDashboardOnRemoteHost
                            && upgradeProduct.Location.ToUpper().Replace(".", Environment.MachineName)
                            .Replace("(LOCAL)", Environment.MachineName).StartsWith(repositoryInstance))
                            || upgradeProduct.Location.ToUpper().StartsWith(repositoryInstance))
                        {
                            cboInstances.DataSource = upgradingProducts;
                            cboInstances.DisplayMember = "InstanceName";
                            cboInstances.DropDownStyle = ComboBoxStyle.DropDownList;
                            cboInstances.SelectedItem = upgradeProduct;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Host.Log("An error occurred while detecting SQLCM CWF AddIn to be upgraded. \r\nError Message: {0}\r\nStack Trace: {1}", ex.Message, ex.StackTrace);
            }

            //Host.NavigateToPage(NavigationDirection.Next);
        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            errorMessage = null;
            invalidControl = null;

            if (!Host.IsInputValid(cboInstances))
            {
                errorMessage = "Display name for product installation not entered.";
                invalidControl = cboInstances;
                MakeProgressBarInvisible();
                return false;
            }

            var instanceName = cboInstances.Text.Trim();
            var allRegistredSqlCmProducts = Host.CwfHelper.GetRegisteredSqlCmProducts();
            string repositoryInstance = Host.CwfHelper.RepositoryInstance.ToUpper();
            repositoryInstance = repositoryInstance.Replace(".", Environment.MachineName).Replace("(LOCAL)", Environment.MachineName);

            foreach (var product in allRegistredSqlCmProducts)
            {
                string location = product.Location.Split(';')[0].ToUpper();
                if (!Host.CwfHelper.IsDashboardOnRemoteHost)
                    location = location.Replace(".", Environment.MachineName).Replace("(LOCAL)", Environment.MachineName);
                if (product.InstanceName.ToUpper().Equals(instanceName.ToUpper()))
                {
                    var productVersion = new Version(product.Version);
                    if (location.StartsWith(repositoryInstance) && m_installerVersion > productVersion)
                    {
                        Host.CwfHelper.ProductsToBeUpgraded = new Products();
                        Host.CwfHelper.ProductsToBeUpgraded.Add(product);
                        break;
                    }
                    else if (!location.StartsWith(repositoryInstance))
                    {
                        errorMessage = string.Format(
                            "Another product is already registered with '{0}' display name. Please specify another display name.",
                            instanceName);
                        MakeProgressBarInvisible();
                        return false;
                    }
                    else
                    {
                        errorMessage = string.Format(
                            "Display name for SQL CM product installation has been already used by registered SQL CM product with upper or the same version {0}. The installer version is {1}. Please specify another display name or choose it from drop down list to upgrade existing one.",
                            product.Version, RestServiceConstants.ProductVersion);
                        MakeProgressBarInvisible();
                        return false;
                    }
                }
            }

            for (var index = 0; index < instanceName.Length; index++)
            {
                if (char.IsLetterOrDigit(instanceName, index))
                    continue;

                if (instanceName[index].Equals('-'))
                    continue;

                errorMessage = "Display name for product installation is having invalid characters. It can only contain letters, numbers or hyphen(-) character.";
                invalidControl = cboInstances;
                MakeProgressBarInvisible();
                return false;
            }

            Host.CwfHelper.CwfProductInstance = instanceName;
            Host.Log("Product Instance Entered: {0}", Host.CwfHelper.CwfProductInstance);
            Host.Log("Upgrade all Instances: {0}", Host.CwfHelper.UpgradeAllProductInstances);

            bool result;

            progInstall.Visible = true;
            lblDisplayName.Visible = false;
            cboInstances.Visible = false;
            this.Refresh();

            try
            {
                Host.AllowNext(false);
                Host.AllowCancel(false);
                result = Host.CwfHelper.RegisterProduct();
            }
            catch (Exception ex)
            {
                result = false;
                Host.AllowNext(true);
                Host.AllowCancel(true);
                Host.ChangeNextButtonText("Retry");
                Host.ChangeCancelButtonText("Exit");
                lblDisplayName.Visible = true;
                cboInstances.Visible = true;
                MakeProgressBarInvisible();
                errorMessage = "An error occurred while registering SQL Compliance Manager to the IDERA Dashboard.";
                Host.Log("An error occured while configuring SQLCM CWF AddIn.\r\nError Message: {0}\r\nStack Trace: {1}", ex.Message, ex.StackTrace);
            }

            return result;
        }

        private void Host_OnLogAppended(string message, params object[] parameters)
        {
            txtLog.AppendText(string.Format(message, parameters));
            txtLog.AppendText(Environment.NewLine);
        }

        private void MakeProgressBarInvisible()
        {
            progInstall.Visible = false;
            this.Refresh();
        }

        private string GetSQLCMSQLServerNameFromRegistry()
        {
            string CollectionServerName = string.Empty;
            RegistryKey view;
            if (Environment.Is64BitOperatingSystem)
            {
                view = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            }
            else
            {
                view = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            }
            try
            {
                using (RegistryKey cmCollectionKey = view.OpenSubKey(@"SOFTWARE\Idera\SQLCM\CollectionService", false))
                {
                    object SQLServerName = cmCollectionKey.GetValue("ServerInstance");
                    if (SQLServerName != null)
                    {
                        CollectionServerName = SQLServerName.ToString();
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return CollectionServerName;
        }
    }
}
