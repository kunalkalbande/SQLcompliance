using SQLCM_Installer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLCM_Installer.Custom_Prompts
{
    public partial class FormEulaBox : Form
    {
        private CustomDropShadow customDropShadow;
        private CustomBorderShadow customBorderShadow;

        public FormEulaBox()
        {
            InitializeComponent();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closeButton_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void FormEulaBox_Load(object sender, EventArgs e)
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
        }

        private void FormEulaBox_Activated(object sender, EventArgs e)
        {
            if (customBorderShadow != null && customBorderShadow.ShadowColor != Color.FromArgb(24, 131, 215))
            {
                customDropShadow.ShadowBlur = 40;
                customDropShadow.RefreshShadow();

                customBorderShadow.ShadowColor = Color.FromArgb(24, 131, 215);
                customBorderShadow.RefreshShadow();
            }
        }

        private void FormEulaBox_Deactivated(object sender, EventArgs e)
        {
            if (customBorderShadow != null && customBorderShadow.ShadowColor != Color.Gray)
            {
                customDropShadow.ShadowBlur = 35;
                customDropShadow.RefreshShadow();

                customBorderShadow.ShadowColor = Color.Gray;
                customBorderShadow.RefreshShadow();
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }
    }
}
