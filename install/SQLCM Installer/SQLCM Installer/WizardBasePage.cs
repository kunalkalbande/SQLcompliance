using System.Drawing;
using System.Windows.Forms;

namespace SQLCM_Installer
{
    internal class WizardPageBase : UserControl
    {
        #region declares

        protected readonly MainForm Host;

        #endregion

        #region constructor \ desctructor

        protected WizardPageBase()
        {
            Size = new Size(550, 486);
            MaximumSize = new Size(550, 486);
            Margin = new Padding(0);
            Padding = new Padding(0);
            BackColor = Color.White;

#if !DEBUG
            Dock = DockStyle.Fill;
#endif
        }

        protected WizardPageBase(MainForm host)
            : this()
        {
            Host = host;
        }

        #endregion

        internal bool IsInitialized { get; set; }

        internal virtual void Initialize() { }

        internal virtual void OnNavigated(NavigationDirection direction) { }

        internal virtual bool DoAction(out string errorMessage, out Control invalidControl)
        {
            errorMessage = null;
            invalidControl = null;
            return true;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // WizardPageBase
            // 
            this.Name = "WizardPageBase";
            this.Size = new System.Drawing.Size(550, 486);
            this.ResumeLayout(false);

        }
    }
}
