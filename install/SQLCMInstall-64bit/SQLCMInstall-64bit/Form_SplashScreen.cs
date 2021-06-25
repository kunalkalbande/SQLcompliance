using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SQLCMInstall_64bit
{
    /// <summary>
    /// Summary description for SplashScreen.
    /// </summary>
    public partial class SplashScreen : Form
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public SplashScreen()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();

            // I am hard coding these to match EXACT image dimensions so that IDE doesn't change em.
            this.ClientSize = new Size(632, 406);
        }
    }
}
