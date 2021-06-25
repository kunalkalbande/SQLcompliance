using System.Drawing;
using System.Windows.Forms;

namespace CwfAddinInstaller
{
    internal class  WizardPageBase: UserControl
    {
        #region declares

        protected readonly FormInstaller Host;

        #endregion

        #region constructor \ desctructor

        protected WizardPageBase()
        {
            Size = new Size(314, 214);
            MaximumSize = new Size(314, 214);
            Margin = new Padding(0);
            Padding = new Padding(0);
            BackColor = Color.White;

#if !DEBUG
            Dock = DockStyle.Fill;
#endif
        }

        protected WizardPageBase(FormInstaller host): this()
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
    }
}
