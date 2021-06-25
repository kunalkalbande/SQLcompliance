using SQLCM_Installer.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLCM_Installer.Custom_Controls
{
    class IderaToolTip : System.Windows.Forms.UserControl
    {
        Label text = new Label();
        PictureBox blacktriangle = new PictureBox();

        public IderaToolTip()
        {
            blacktriangle.Size = new Size(29, 20);
            blacktriangle.Image = Resources.icon;
            blacktriangle.Location = new Point(85, 80);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
            this.Size = new Size(220, 100);
            text.Text = "";
            text.MaximumSize = new Size(220, 80);
            text.MinimumSize = new Size(220, 80);
            text.AutoSize = true;
            text.Font = new System.Drawing.Font("Source Sans Pro", 10.5F);
            text.BackColor = Color.FromArgb(34, 29, 22);
            text.ForeColor = Color.White;
            text.Padding = new Padding(5);
            this.Controls.Add(text);
            this.Controls.Add(blacktriangle);
            this.Visible = false;
        }

        public bool SetText(string tooltip)
        {
            this.text.Text = tooltip;
            return true;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (Constants.SourceSansProRegular != null)
            {
                this.text.Font = new System.Drawing.Font(Constants.SourceSansProRegular, 9F, FontStyle.Regular);
            }
        }
    }
}
