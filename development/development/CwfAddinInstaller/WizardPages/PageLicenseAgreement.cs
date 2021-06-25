using System;
using System.Windows.Forms;
using CwfAddinInstaller.Properties;
using SQLcomplianceCwfAddin.RestService;

namespace CwfAddinInstaller.WizardPages
{
    [WizardPageInfo("End-User License Agreement", "Please read the following license agreement carefully.", WizardPage.LicenseAgreement)]
    internal partial class PageLicenseAgreement : WizardPageBase
    {
        public PageLicenseAgreement(FormInstaller host): base(host)
        {
            InitializeComponent();
        }

        internal override void Initialize()
        {
            txtAgreement.Rtf = RestServiceConstants.InstallerType.Equals("Trial") ? 
                Resources.IderaTrialSoftwareLicenseAgreement :
                Resources.IderaSoftwareLicenseAgreement;
            Host.Log("{0} - Initialized.", WizardPage.LicenseAgreement);
            IsInitialized = true;
        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            errorMessage = null;
            invalidControl = null;

            Host.Log("License Agreement Accepted: {0}", chkIAccept.Checked);
            if (chkIAccept.Checked)
                return true;

            errorMessage = "The terms in the License Agreement not accepted.";
            invalidControl = chkIAccept;
            return false;
        }

        internal override void OnNavigated(NavigationDirection direction)
        {
            Host.AllowNext(chkIAccept.Checked);
        }

        private void chkIAccept_CheckedChanged(object sender, EventArgs e)
        {
            Host.AllowNext(chkIAccept.Checked);
        }
    }
}
