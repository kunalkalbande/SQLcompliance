using System.Windows.Forms;

namespace CwfAddinInstaller.WizardPages
{
    [WizardPageInfo("Ready to Install SQLCM CWF Add-In", "", WizardPage.ReadyToInstall)]
    internal partial class PageReadyToInstall : WizardPageBase
    {
        public PageReadyToInstall(FormInstaller host): base(host)
        {
            InitializeComponent();
        }

        internal override void Initialize()
        {
            Host.Log("{0} - Initialized.", WizardPage.ReadyToInstall);
            IsInitialized = true;
        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            errorMessage = null;
            invalidControl = null;

            Host.Log("Yes we are ready to configure.");
            return true;
        }
    }
}
