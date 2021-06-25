using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SQLCM_Installer.Custom_Controls;

namespace SQLCM_Installer.WizardPages
{
    [WizardPageInfo(WizardPage.AgentCollectionServer)]
    internal partial class PageAgentCollectionServer : WizardPageBase
    {
        public PageAgentCollectionServer(MainForm host)
            : base(host)
        {
            InitializeComponent();
        }

        internal override void Initialize()
        {
            IsInitialized = true;
        }

        internal override bool DoAction(out string errorMessage, out Control invalidControl)
        {
            errorMessage = null;
            invalidControl = null;
            ServiceStatus status = ServiceStatus.NotSpecified;
            if(txtAgentServerName.Text == "")
            {
                errorMessage = "Please provide a server name";
                return false;
            }

            this.Cursor = Cursors.WaitCursor;
            HelperFunctions helperFunction = new HelperFunctions();
            if (helperFunction.CheckServiceStatus(txtAgentServerName.Text, Constants.CollectionServiceName, out status))
            {
                this.Cursor = Cursors.Default;
                InstallProperties.AgentCollectionServer = txtAgentServerName.Text;
                return true;
            }
            else
            {
                switch (status)
                { 
                    case ServiceStatus.NotFound:
                        errorMessage = Constants.CollectionServiceError;
                        break;
                    case ServiceStatus.NotReachable:
                        errorMessage = Constants.CollectionServiceMachineError;
                        break;
                    case ServiceStatus.NotRunning:
                        errorMessage = Constants.CollectionServiceError;
                        break;
                    default:
                        errorMessage = Constants.CollectionServiceMachineError;
                        break;
                }

                IderaMessageBoxWithOption messageBox = new IderaMessageBoxWithOption();
                messageBox.SetButtonText("Yes", "No", "Confirm");
                errorMessage = errorMessage.Replace("%MachinePlaceholder%", txtAgentServerName.Text);
                messageBox.Show(errorMessage, false);

                if (messageBox.isCancelClick)
                {
                    this.Cursor = Cursors.Default;
                    errorMessage = string.Empty;
                    return false;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                    InstallProperties.AgentCollectionServer = txtAgentServerName.Text;
                    return true;
                }
            }
        }
    }
}
