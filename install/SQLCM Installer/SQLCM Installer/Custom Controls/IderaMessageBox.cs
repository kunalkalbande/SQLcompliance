using SQLCM_Installer;
using SQLCM_Installer.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLCM_Installer.Custom_Controls
{
    class IderaMessageBox : System.Windows.Forms.Form
    {
        Panel topPanel = new Panel();
        IderaButton OkButton = new IderaButton();
        IderaButton YesButton = new IderaButton();
        IderaButton NoButton = new IderaButton();
        PictureBox CloseButton = new PictureBox();
        IderaTitleLabel Header = new IderaTitleLabel();
        PictureBox HeaderImage = new PictureBox();
        Label MessageText = new Label();
        LinkLabel link = new LinkLabel();
        Panel BorderPanel = new Panel();
        private CustomDropShadow customDropShadow;
        private CustomBorderShadow customBorderShadow;
        public bool isYesClick = false;
        public bool isNoClick = false;
        #region Window Draggable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        public static System.Drawing.Point location;

        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        #endregion

        public IderaMessageBox()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IderaMessageBox));
            BorderPanel.Location = new Point(500, 195);
            BorderPanel.BackColor = Color.FromArgb(170, 170, 170);
            Header.Text = "Error";
            Header.AutoSize = true;
            Header.ForeColor = Color.White;
            HeaderImage.Image = Resources.error_icon;
            MessageText.ForeColor = Color.FromArgb(54, 54, 54);
            MessageText.Location = new Point(15, 84);
            MessageText.MaximumSize = new System.Drawing.Size(465, 0);
            MessageText.AutoSize = true;
            link.ForeColor = Color.FromArgb(54, 54, 54);
            link.LinkClicked += Link_LinkClicked;
            link.Location = new Point(15, 124);
            link.MaximumSize = new System.Drawing.Size(465, 0);
            link.AutoSize = true;
            link.Visible = false;
            this.BackColor = Color.White;
            CloseButton.Image = Resources.close_icon;
            CloseButton.SizeMode = PictureBoxSizeMode.AutoSize;
            OkButton.Text = "OK";
            OkButton.ForeColor = Color.FromArgb(72, 62, 47);
            YesButton.Text = "Yes";
            YesButton.ForeColor = Color.FromArgb(72, 62, 47);
            NoButton.Text = "No";
            NoButton.ForeColor = Color.FromArgb(72, 62, 47);
            CloseButton.Location = new Point(463, 20);
            HeaderImage.Location = new Point(0, 0);
            Header.Location = new Point(40, 12);
            OkButton.Size = new Size(53, 28);
            OkButton.Location = new Point(427, 147);
            YesButton.Size = new Size(53, 28);
            YesButton.Location = new Point(340, 147);
            NoButton.Size = new Size(53, 28);
            NoButton.Location = new Point(427, 147);
            topPanel.Location = new Point(0, 0);
            topPanel.Size = new Size(500, 54);
            topPanel.BackColor = Color.FromArgb(183, 175, 167);
            CloseButton.Click += CloseButton_Click;
            OkButton.Click += OkButton_Click;
            YesButton.Click += YesButton_Click;
            NoButton.Click += NoButton_Click;
            topPanel.MouseDown += topPanel_MouseDown;
            HeaderImage.MouseDown += topPanel_MouseDown;
            Header.MouseDown += topPanel_MouseDown;
            CloseButton.MouseEnter += CloseButton_MouseEnter;
            CloseButton.MouseLeave += CloseButton_MouseLeave;
            topPanel.Controls.Add(CloseButton);
            topPanel.Controls.Add(Header);
            topPanel.Controls.Add(HeaderImage);
            this.Size = new Size(500, 195);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Load += new System.EventHandler(this.IderaMessageBox_Load);
            this.Activated += new EventHandler(this.IderaMessageBox_Activated);
            this.Deactivate += new EventHandler(this.IderaMessageBox_Deactivated);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Controls.Add(topPanel);
            this.Controls.Add(OkButton);
            this.Controls.Add(MessageText);
            this.Controls.Add(YesButton);
            this.Controls.Add(NoButton);
            this.Controls.Add(link);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CloseButton)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void Link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://wiki.idera.com/display/SQLCM/Software+requirements");
        }

        private void CloseButton_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void CloseButton_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void topPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        public void Show(string message)
        {
            YesButton.Visible = false;
            NoButton.Visible = false;
            this.StartPosition = FormStartPosition.CenterParent;
            MessageText.Text = message;
            if (message.Length > 200)
            {
                int height = (message.Length - 200) / 2;
                this.Size = new Size(500, this.Size.Height + height);
                OkButton.Location = new Point(427, this.Size.Height - 50);
            }
            this.ShowDialog();
        }
        public void Show(string message, bool Ishyperlink)
        {
            YesButton.Visible = false;
            NoButton.Visible = false;
            this.StartPosition = FormStartPosition.CenterParent;
            MessageText.Text = message;
            link.Text = "Learn about SQL Compliance Manager software requirements";
            link.Visible = true;

            if (message.Length > 200)
            {
                int height = (message.Length - 200) / 2;
                this.Size = new Size(500, this.Size.Height + height);
                OkButton.Location = new Point(427, this.Size.Height - 50);
            }
            this.ShowDialog();
        }
        public void ShowYesNo(string message, string headermessage)
        {
            OkButton.Visible = false;
            Header.Text = headermessage;
            Header.Location = new Point(15, 17);
            Header.Size = new Size(77, 57);
            HeaderImage.Visible = false;
            this.StartPosition = FormStartPosition.CenterParent;
            MessageText.Text = message;
            if (message.Length > 200)
            {
                int height = (message.Length - 200) / 2;
                this.Size = new Size(500, this.Size.Height + height);
                YesButton.Location = new Point(340, this.Size.Height - 50);
                NoButton.Location = new Point(427, this.Size.Height - 50);
            }
            this.ShowDialog();
        }


        public void Show(string message, string headermessage)
        {
            this.StartPosition = FormStartPosition.CenterParent;
            MessageText.Text = message;
            Header.Text = headermessage;
            Header.Location = new Point(15, 17);
            HeaderImage.Visible = false;
            if (message.Length > 200)
            {
                int height = (message.Length - 200) / 2;
                this.Size = new Size(500, this.Size.Height + height);
                OkButton.Location = new Point(427, this.Size.Height - 50);
            }
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

        public void YesButton_Click(object sender, EventArgs e)
        {
            isYesClick = true;
            this.Close();
        }
        public void NoButton_Click(object sender, EventArgs e)
        {
            isNoClick = true;
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
