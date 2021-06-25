using System;

namespace CwfAddinInstaller
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class WizardPageInfo: Attribute
    {
        #region constructor \ destructor

        public WizardPageInfo(string title, string description, WizardPage page)
        {
            Title = title;
            Description = description;
            Page = page;
        }

        ~WizardPageInfo()
        {
            Title = null;
            Description = null;
            Page = WizardPage.NotSpecified;
        }

        #endregion

        #region properties

        public string Title { get; private set; }

        public string Description { get; private set; }

        public WizardPage Page { get; private set; }

        #endregion
    }
}
