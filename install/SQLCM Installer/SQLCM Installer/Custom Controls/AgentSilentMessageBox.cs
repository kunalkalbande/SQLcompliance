using SQLCM_Installer;
using SQLCM_Installer.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLCM_Installer.Custom_Controls
{
    class AgentSilentMessageBox : System.Windows.Forms.Form
    {
        Panel topPanel = new Panel();
        IderaButton OkButton = new IderaButton();
        PictureBox CloseButton = new PictureBox();
        IderaTitleLabel Header = new IderaTitleLabel();
        PictureBox HeaderImage = new PictureBox();
        LinkLabel MessageText = new LinkLabel();
        Panel BorderPanel = new Panel();
        private CustomDropShadow customDropShadow;
        private CustomBorderShadow customBorderShadow;


        public AgentSilentMessageBox()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IderaMessageBox));
            BorderPanel.Location = new Point(500, 195);
            BorderPanel.BackColor = Color.FromArgb(170, 170, 170);
            Header.Text = "Error";
            Header.AutoSize = true;
            Header.ForeColor = Color.White;
            HeaderImage.Image = Resources.error_icon;
            MessageText.ForeColor = Color.FromArgb(54, 54, 54);
            MessageText.Location = new Point(30, 84);
            MessageText.MaximumSize = new System.Drawing.Size(400, 0);
            if (Environment.Is64BitOperatingSystem)
            {
                MessageText.LinkArea = new System.Windows.Forms.LinkArea(163, 4);
            }
            else
            {
                MessageText.LinkArea = new System.Windows.Forms.LinkArea(159, 4);
            }
            MessageText.AutoSize = true;
            this.MessageText.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.MessageText_LinkClicked);
            this.BackColor = Color.White;
            CloseButton.Image = Resources.close_icon;
            CloseButton.SizeMode = PictureBoxSizeMode.AutoSize;
            OkButton.Text = "OK";
            OkButton.ForeColor = Color.FromArgb(72, 62, 47);
            this.Size = new Size(500, 195);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Load += new System.EventHandler(this.IderaMessageBox_Load);
            this.Activated += new EventHandler(this.IderaMessageBox_Activated);
            this.Deactivate += new EventHandler(this.IderaMessageBox_Deactivated);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Controls.Add(topPanel);
            this.Controls.Add(OkButton);
            topPanel.Controls.Add(CloseButton);
            topPanel.Controls.Add(Header);
            topPanel.Controls.Add(HeaderImage);
            this.Controls.Add(MessageText);
            CloseButton.Location = new Point(463, 20);
            HeaderImage.Location = new Point(20, 0);
            Header.Location = new Point(57, 12);
            OkButton.Size = new Size(53, 28);
            OkButton.Location = new Point(427, 147);
            topPanel.Location = new Point(0, 0);
            topPanel.Size = new Size(500, 54);
            topPanel.BackColor = Color.FromArgb(183, 175, 167);
            CloseButton.Click += CloseButton_Click;
            OkButton.Click += OkButton_Click;
        }

        private void MessageText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://wiki.idera.com/display/SQLCM/Perform+a+silent+installation+of+the+SQLCM+Agent");
        }

        public void Show(string message)
        {
            this.StartPosition = FormStartPosition.CenterParent;
            MessageText.Text = message;
            if (message.Length > 200)
            {
                this.Size = new Size(500, message.Length);
                OkButton.Location = new Point(427, message.Length - 50);
            }
            this.ShowDialog();
        }

        public void Show(string message, string headermessage)
        {
            this.StartPosition = FormStartPosition.CenterParent;
            MessageText.Text = message;
            Header.Text = headermessage;
            Header.Location = new Point(20, 17);
            HeaderImage.Visible = false;
            this.ShowDialog();
        }

        private void IderaMessageBox_Load(object sender, EventArgs e)
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

        private void IderaMessageBox_Activated(object sender, EventArgs e)
        {
            if (customBorderShadow != null && customBorderShadow.ShadowColor != Color.FromArgb(24, 131, 215))
            {
                customDropShadow.ShadowBlur = 40;
                customDropShadow.RefreshShadow();

                customBorderShadow.ShadowColor = Color.FromArgb(24, 131, 215);
                customBorderShadow.RefreshShadow();
            }
        }

        private void IderaMessageBox_Deactivated(object sender, EventArgs e)
        {
            if (customBorderShadow != null && customBorderShadow.ShadowColor != Color.Gray)
            {
                customDropShadow.ShadowBlur = 35;
                customDropShadow.RefreshShadow();

                customBorderShadow.ShadowColor = Color.Gray;
                customBorderShadow.RefreshShadow();
            }
        }

        public void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void OkButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (Constants.SourceSansProRegular != null)
            {
                MessageText.Font = new System.Drawing.Font(Constants.SourceSansProRegular, 10.5F, this.Font.Style);
            }
        }
    }
}
