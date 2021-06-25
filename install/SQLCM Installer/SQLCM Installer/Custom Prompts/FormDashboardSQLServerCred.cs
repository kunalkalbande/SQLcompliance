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
    public partial class FormDashboardSQLServerCred : Form
    {
        private CustomDropShadow customDropShadow;
        private CustomBorderShadow customBorderShadow;
        private string errorMessage = string.Empty;

        public FormDashboardSQLServerCred()
        {
            InitializeComponent();
        }

        #region Window Draggable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        public static System.Drawing.Point location;

        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        #endregion

        private void FormDashboardSQLServerCred_Load(object sender, EventArgs e)
        {
            textUserName.Text = InstallProperties.DashboardSQLServerUsername;
            textPassword.Text = InstallProperties.DashboardSQLServerPassword;
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
        }

        private void FormDashboardSQLServerCred_Activated(object sender, EventArgs e)
        {
            if (customBorderShadow != null && customBorderShadow.ShadowColor != Color.FromArgb(24, 131, 215))
            {
                customDropShadow.ShadowBlur = 40;
                customDropShadow.RefreshShadow();

                customBorderShadow.ShadowColor = Color.FromArgb(24, 131, 215);
                customBorderShadow.RefreshShadow();
            }
        }

        private void FormDashboardSQLServerCred_Deactivated(object sender, EventArgs e)
        {
            if (customBorderShadow != null && customBorderShadow.ShadowColor != Color.Gray)
            {
                customDropShadow.ShadowBlur = 35;
                customDropShadow.RefreshShadow();

                customBorderShadow.ShadowColor = Color.Gray;
                customBorderShadow.RefreshShadow();
            }
        }

        private void pictureCloseIcon_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            HelperFunctions helperFunction = new HelperFunctions();
            if (string.IsNullOrEmpty(InstallProperties.DashboardSQLServerInstanceName)
                || string.IsNullOrEmpty(textUserName.Text)
                || string.IsNullOrEmpty(textPassword.Text))
            {
                pictureDashboardCredError.Visible = true;
                labelError.Visible = true;
                labelError.Text = "Login failed";
            }
            else if (!helperFunction.CheckServerConnection(true, InstallProperties.DashboardSQLServerInstanceName, textUserName.Text, textPassword.Text, out errorMessage))
            {
                pictureDashboardCredError.Visible = true;
                labelError.Visible = true;
                labelError.Text = "Login failed for user " + textUserName.Text;
            }
            else
            {
                pictureDashboardCredError.Visible = false;
                labelError.Visible = false;
                InstallProperties.IsDashboardSQLServerSQLAuth = true;
                InstallProperties.DashboardSQLServerUsername = textUserName.Text;
                InstallProperties.DashboardSQLServerPassword = textPassword.Text;
                this.Close();
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
