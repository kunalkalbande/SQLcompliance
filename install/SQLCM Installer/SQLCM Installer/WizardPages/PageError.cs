using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLCM_Installer.WizardPages
{
    [WizardPageInfo(WizardPage.Error)]
    internal partial class PageError : WizardPageBase
    {
        public PageError(MainForm host)
            : base(host)
        {
            InitializeComponent();
        }

        internal override void OnNavigated(NavigationDirection direction)
        {
            if (Constants.FailedProduct == Products.Registration)
            {
                if (Constants.UserInstallType == InstallType.CMAndDashboard)
                {
                    labelErrorMessage.Text = "IDERA SQL Compliance Manager & IDERA Dashboard installation has completed. However, registration of SQL Compliance Manager to IDERA Dashboard has failed.\r\n\r\nYou will have to register SQL Compliance Manager manually to the IDERA Dashboard by going to Manage Products screen in IDERA Dashboard Administration.\r\n\r\nClick Finish to exit the installer.";
                }
                else
                {
                    labelErrorMessage.Text = "IDERA SQL Compliance Manager installation has completed. However, registration of SQL Compliance Manager to IDERA Dashboard has failed.\r\n\r\nYou will have to register SQL Compliance Manager manually to the IDERA Dashboard by going to Manage Products screen in IDERA Dashboard Administration.\r\n\r\nClick Finish to exit the installer.";
                }
            }
            else
            {
                labelErrorMessage.Text = labelErrorMessage.Text.Replace("{0}", Constants.ProductMap[Constants.FailedProduct]);
            }
        }
    }
}
