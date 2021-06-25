using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace CwfAddinInstaller.WizardPages
{
    [WizardPageInfo("Welcome to IDERA SQL Compliance Manager and Dashboard Setup", "", WizardPage.Welcome)]
    internal partial class PageWelcome : WizardPageBase
    {
        public PageWelcome(FormInstaller host)
            : base(host)
        {
            Host.SetHeader();
            InitializeComponent();
        }

        internal override void Initialize()
        {
            Host.Log("{0} - Initialized.", WizardPage.Welcome);
            IsInitialized = true;
        }

        private void PageWelcome_Load(object sender, System.EventArgs e)
        {
        }

        private void lnkIderaWiki_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://wiki.idera.com/display/SQLCM/How+to+install+the+IDERA+Dashboard+and+SQL+Compliance+Manager");
        }

    }
}
