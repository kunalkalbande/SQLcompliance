using SQLCM_Installer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLCM_Installer.Custom_Prompts
{
    public partial class FormTestConnection : Form
    {
        private CustomDropShadow customDropShadow;
        private CustomBorderShadow customBorderShadow;
        public string errorMessage = string.Empty;
        private bool _showCMMessage = false;
        private bool _showDashboardMessage = false;

        #region Window Draggable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        public static System.Drawing.Point location;

        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        #endregion

        public FormTestConnection()
        {
            InitializeComponent();
        }

        public void SetMessageVisibility(bool cmValue, bool dashboardValue)
        {
            _showCMMessage = cmValue;
            _showDashboardMessage = dashboardValue;
        }

        private void FormTestConnection_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                customDropShadow = new CustomDropShadow(this)
                {
                    ShadowBlur = 40,
                    ShadowSpread = -30,
                    ShadowColor = Color.Black

                };
                customDropShadow.RefreshShadow();

                customBorderShadow = new CustomBorderShadow(this)
                {
                    ShadowBlur = 0,
                    ShadowSpread = 1,
                    ShadowColor = Color.FromArgb(24, 131, 215)

                };
                customBorderShadow.RefreshShadow();
            }

            HelperFunctions helperFunction = new HelperFunctions();
            bool cmPass = false;
            bool dashboardPass = false;
            bool cmInstall = false;
            bool dashboardInstall = false;

            if (Constants.UserInstallType == InstallType.CMOnly || Constants.UserInstallType == InstallType.AgentOnly ||
                Constants.UserInstallType == InstallType.CMAndDashboard || (Constants.UserInstallType == InstallType.AgentAndDashboard && _showCMMessage))
            {
                cmInstall = true;
                if (string.IsNullOrEmpty(InstallProperties.CMSQLServerInstanceName)
                    || (InstallProperties.IsCMSQLServerSQLAuth && string.IsNullOrEmpty(InstallProperties.CMSQLServerUsername))
                    || (InstallProperties.IsCMSQLServerSQLAuth && string.IsNullOrEmpty(InstallProperties.CMSQLServerPassword)))
                {
                    cmPass = false;
                }
                else if (helperFunction.CheckServerConnection(InstallProperties.IsCMSQLServerSQLAuth, InstallProperties.CMSQLServerInstanceName, InstallProperties.CMSQLServerUsername, InstallProperties.CMSQLServerPassword, out errorMessage))
                {
                    cmPass = true;
                }
                else
                {
                    cmPass = false;
                }
            }

            if ((Constants.UserInstallType == InstallType.DashboardOnly ||
                Constants.UserInstallType == InstallType.CMAndDashboard || 
                (Constants.UserInstallType == InstallType.AgentAndDashboard && _showDashboardMessage) ||
                Constants.UserInstallType == InstallType.ConsoleAndDashboard) && !InstallProperties.IsUpgradeRadioSelection)
            {
                dashboardInstall = true;
                if (string.IsNullOrEmpty(InstallProperties.DashboardSQLServerInstanceName)
                    || (InstallProperties.IsDashboardSQLServerSQLAuth && string.IsNullOrEmpty(InstallProperties.DashboardSQLServerPassword))
                    || (InstallProperties.IsDashboardSQLServerSQLAuth && string.IsNullOrEmpty(InstallProperties.DashboardSQLServerUsername)))
                {
                    dashboardPass = false;
                }
                else if (helperFunction.CheckServerConnection(InstallProperties.IsDashboardSQLServerSQLAuth, InstallProperties.DashboardSQLServerInstanceName, InstallProperties.DashboardSQLServerUsername, InstallProperties.DashboardSQLServerPassword, out errorMessage))
                {
                    dashboardPass = true;
                }
                else
                {
                    dashboardPass = false;
                }
            }

            SetControls(cmInstall, dashboardInstall, cmPass, dashboardPass);
        }

        private void FormTestConnection_Activated(object sender, EventArgs e)
        {
            if (customBorderShadow != null && customBorderShadow.ShadowColor != Color.FromArgb(24, 131, 215))
            {
                customDropShadow.ShadowBlur = 40;
                customDropShadow.RefreshShadow();

                customBorderShadow.ShadowColor = Color.FromArgb(24, 131, 215);
                customBorderShadow.RefreshShadow();
            }
        }

        private void FormTestConnection_Deactivated(object sender, EventArgs e)
        {
            if (customBorderShadow != null && customBorderShadow.ShadowColor != Color.Gray)
            {
                customDropShadow.ShadowBlur = 35;
                customDropShadow.RefreshShadow();

                customBorderShadow.ShadowColor = Color.Gray;
                customBorderShadow.RefreshShadow();
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureCloseIcon_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void SetControls(bool cmInstall, bool dashboardInstall, bool CMPass, bool DashboardPass)
        {
            if (cmInstall)
            {
                this.labelDashboardCredOkorError.Location = new System.Drawing.Point(47, 119);
                this.pictureDashboardCredOkorError.Location = new System.Drawing.Point(20, 122);

                if (CMPass)
                {
                    this.labelCMCredOkorError.Visible = true;
                    this.pictureCMCredOkorError.Image = Resources.okicon;
                    this.labelCMCredOkorError.Text = Constants.CMSQLServerConnectionOK;
                    this.labelCMCredOkorError.ForeColor = Color.FromArgb(72, 62, 47);
                }
                else if (!CMPass)
                {
                    this.labelCMCredOkorError.Visible = true;
                    this.pictureCMCredOkorError.Image = Resources.criticalicon;
                    this.labelCMCredOkorError.Text = Constants.CMSQLServerConnectionError;
                    this.labelCMCredOkorError.ForeColor = Color.FromArgb(204, 0, 0);
                }
            }
            else
            {
                this.labelCMCredOkorError.Visible = false;
                this.labelDashboardCredOkorError.Location = new Point(47, 74);
                this.pictureDashboardCredOkorError.Location = new Point(20, 77);
            }

            if (dashboardInstall)
            {
                if (DashboardPass)
                {
                    this.labelDashboardCredOkorError.Visible = true;
                    this.pictureDashboardCredOkorError.Image = Resources.okicon;
                    this.labelDashboardCredOkorError.Text = Constants.DashboardSQLServerConnectionOK;
                    this.labelDashboardCredOkorError.ForeColor = Color.FromArgb(72, 62, 47);
                }

                else if (!DashboardPass)
                {
                    this.labelDashboardCredOkorError.Visible = true;
                    this.pictureDashboardCredOkorError.Image = Resources.criticalicon;
                    this.labelDashboardCredOkorError.Text = Constants.DashboardSQLServerConnectionError;
                    this.labelDashboardCredOkorError.ForeColor = Color.FromArgb(204, 0, 0);
                }
            }
            else
            {
                this.labelDashboardCredOkorError.Visible = false;
            }
        }

        private void panelHeader_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void pictureCloseIcon_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void pictureCloseIcon_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }
    }
}
