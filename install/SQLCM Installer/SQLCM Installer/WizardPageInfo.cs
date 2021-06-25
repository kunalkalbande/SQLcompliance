using System;

namespace SQLCM_Installer
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class WizardPageInfo : Attribute
    {
        #region constructor \ destructor

        public WizardPageInfo( WizardPage page)
        {
            Page = page;
        }

        ~WizardPageInfo()
        {
            Page = WizardPage.NotSpecified;
        }

        #endregion

        #region properties

        public WizardPage Page { get; private set; }

        #endregion
    }
}
