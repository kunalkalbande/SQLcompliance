using System;
using System.Windows.Forms;
using SQLcomplianceCwfAddin.RestService;

namespace CwfAddinInstaller.WizardPages
{
    [WizardPageInfo("Display Name for Product Installation", "Enter name of the instance to be used.", WizardPage.InstanceName)]
    internal partial class PageInstanceName : WizardPageBase
    {
        private readonly Version m_installerVersion = new Version(RestServiceConstants.ProductVersion);

        public PageInstanceName(FormInstaller host): base(host)
        {
            InitializeComponent();
        }

        internal override void Initialize()
        {
            Host.Log("{0} - Initialized.", WizardPage.InstanceName);
            IsInitialized = true;
        }

        internal override void OnNavigated(NavigationDirection direction)
        {
            if (direction != NavigationDirection.Next)
            {
                return;
            }

            try
            {
                var upgradingProducts = Host.CwfHelper.GetUpdatingRegisteredSqlCmProductsWithLowerVersionThen(m_installerVersion);
                cboInstances.DataSource = null;
                string repositoryInstance = Host.CwfHelper.RepositoryInstance.ToUpper();                
                repositoryInstance = repositoryInstance.Replace(".", Environment.MachineName).Replace("(LOCAL)", Environment.MachineName);
               if (upgradingProducts.Count > 0)
                {
                    foreach(var upgradeProduct in upgradingProducts)
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
        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            errorMessage = null;
            invalidControl = null;

            if (!Host.IsInputValid(cboInstances))
            {
                errorMessage = "Display name for product installation not entered.";
                invalidControl = cboInstances;
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
                    location = location.Replace(".",Environment.MachineName).Replace("(LOCAL)",Environment.MachineName);                
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
                        return false;
                    }
                    else
                    {
                        errorMessage = string.Format(
                            "Display name for SQL CM product installation has been already used by registered SQL CM product with upper or the same version {0}. The installer version is {1}. Please specify another display name or choose it from drop down list to upgrade existing one.",
                            product.Version, RestServiceConstants.ProductVersion);
                        return false;
                    }
                }
            }
            //else
            //{
            //    Host.CwfHelper.ProductsToBeUpgraded = new Products();
            //    Host.CwfHelper.ProductsToBeUpgraded.Add(cboInstances.SelectedValue as Product);
            //}

            for (var index = 0; index < instanceName.Length; index++)
            {
                if (char.IsLetterOrDigit(instanceName, index))
                    continue;

                if (instanceName[index].Equals('-'))
                    continue;

                errorMessage = "Display name for product installation is having invalid characters. It can only contain letters, numbers or hyphen(-) character.";
                invalidControl = cboInstances;
                return false;
            }

            Host.CwfHelper.CwfProductInstance = instanceName;
            Host.CwfHelper.UpgradeAllProductInstances = chkUpgradeAll.Checked;
            Host.Log("Product Instance Entered: {0}", Host.CwfHelper.CwfProductInstance);
            Host.Log("Upgrade all Instances: {0}", Host.CwfHelper.UpgradeAllProductInstances);
            return true;
        }
    }
}
