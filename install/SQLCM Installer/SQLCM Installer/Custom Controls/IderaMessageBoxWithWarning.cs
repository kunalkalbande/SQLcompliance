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
    public class IderaMessageBoxWithWarning : System.Windows.Forms.Form
    {
        Panel topPanel = new Panel();
        IderaButton DeleteButton = new IderaButton();
        IderaButton OkButton = new IderaButton(); //as Preserve button working
        IderaButton ChangeButton = new IderaButton();
        IderaButton exitButton = new IderaButton();
        PictureBox CloseButton = new PictureBox();
        IderaTitleLabel Header = new IderaTitleLabel();
        PictureBox HeaderImage = new PictureBox();
        Label MessageText = new Label();
        Label MessageOneText = new Label();
        Label MessageTwoText = new Label();
        Label MessageThreeText = new Label();
        Label DeleteText = new Label();
        Label PreserveText = new Label();
        Label ChangeText = new Label();
        Panel BorderPanel = new Panel();
        private CustomDropShadow customDropShadow;
        private CustomBorderShadow customBorderShadow;
        public bool isDeleteClick = false;
        public bool isChangeClick = false;
        public bool isOkClick = false;
        public bool isExitClick = false;
        #region Window Draggable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        public static System.Drawing.Point location;

        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        #endregion

        public IderaMessageBoxWithWarning()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IderaMessageBox));
            BorderPanel.Location = new Point(470, 195);
            BorderPanel.BackColor = Color.FromArgb(170, 170, 170);
            Header.Text = "Repository Warning";
            Header.AutoSize = true;
            Header.ForeColor = Color.White;
            HeaderImage.Image = Resources.infoicon;
            HeaderImage.Location = new Point(0, 0);
            topPanel.Location = new Point(0, 0);
            topPanel.Size = new Size(588, 54);
            topPanel.BackColor = Color.FromArgb(183, 175, 167);
            Header.Location = new Point(40, 12);
            MessageText.ForeColor = Color.FromArgb(54, 54, 54);
            MessageText.Location = new Point(15, 84);
            MessageText.MaximumSize = new System.Drawing.Size(470, 0);
            MessageText.AutoSize = true;
            MessageText.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            MessageOneText.ForeColor = Color.FromArgb(54, 54, 54);
            MessageOneText.Location = new Point(15, 110);//27
            MessageOneText.MaximumSize = new System.Drawing.Size(471, 0);
            MessageOneText.AutoSize = true;

            MessageTwoText.ForeColor = Color.FromArgb(54, 54, 54);
            MessageTwoText.Location = new Point(15, 180);
            MessageTwoText.MaximumSize = new System.Drawing.Size(471, 0);
            MessageTwoText.AutoSize = true;

            MessageThreeText.ForeColor = Color.FromArgb(54, 54, 54);
            MessageThreeText.Location = new Point(15, 230);
            MessageThreeText.MaximumSize = new System.Drawing.Size(471, 0);
            MessageThreeText.AutoSize = true;

            DeleteText.ForeColor = Color.FromArgb(54, 54, 54);
            DeleteText.Location = new Point(481, 146);//new Point(437, 110);
            DeleteText.Text = "Delete.";
            DeleteText.Font = new System.Drawing.Font("Source Sans Pro", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            PreserveText.ForeColor = Color.FromArgb(54, 54, 54);
            PreserveText.Location = new Point(483, 216);
            PreserveText.Text = "Preserve.";
            PreserveText.Font = new System.Drawing.Font("Source Sans Pro", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            ChangeText.ForeColor = Color.FromArgb(54, 54, 54);
            ChangeText.Location = new Point(310, 254);
            ChangeText.Text = "Change.";
            ChangeText.Font = new System.Drawing.Font("Source Sans Pro", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));


            this.BackColor = Color.White;
            CloseButton.Image = Resources.close_icon;
            CloseButton.SizeMode = PictureBoxSizeMode.AutoSize;

            DeleteButton.Text = "Delete";
            DeleteButton.ForeColor = Color.FromArgb(72, 62, 47);
            DeleteButton.Size = new Size(73, 28); //53
            DeleteButton.Location = new Point(127, 280);

            DeleteButton.Click += DeleteButton_Click;

            OkButton.Text = "Preserve";
            OkButton.ForeColor = Color.FromArgb(72, 62, 47);
            CloseButton.Location = new Point(503, 20);
            OkButton.Size = new Size(73, 28);
            OkButton.Location = new Point(217, 280);
            CloseButton.Click += CloseButton_Click;
            OkButton.Click += OkButton_Click;

            ChangeButton.Text = "Change";
            ChangeButton.ForeColor = Color.FromArgb(72, 62, 47);
            ChangeButton.Size = new Size(73, 28); //53
            ChangeButton.Location = new Point(307, 280);
            ChangeButton.Click += ChangeButton_Click;

            exitButton.Text = "Cancel";
            exitButton.ForeColor = Color.FromArgb(72, 62, 47);
            exitButton.Size = new Size(73, 28);//53
            exitButton.Location = new Point(397, 280);
            exitButton.Click += exitButton_Click;

            topPanel.MouseDown += topPanel_MouseDown;
            HeaderImage.MouseDown += topPanel_MouseDown;
            Header.MouseDown += topPanel_MouseDown;
            CloseButton.MouseEnter += CloseButton_MouseEnter;
            CloseButton.MouseLeave += CloseButton_MouseLeave;
            topPanel.Controls.Add(CloseButton);
            topPanel.Controls.Add(Header);
            topPanel.Controls.Add(HeaderImage);
            this.Size = new Size(470, 320);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Load += new System.EventHandler(this.IderaMessageBox_Load);
            this.Activated += new EventHandler(this.IderaMessageBox_Activated);
            this.Deactivate += new EventHandler(this.IderaMessageBox_Deactivated);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Controls.Add(topPanel);
            this.Controls.Add(DeleteButton);
            this.Controls.Add(OkButton);
            this.Controls.Add(ChangeButton);
            this.Controls.Add(exitButton);
            this.Controls.Add(MessageText);
            this.Controls.Add(MessageOneText);
            this.Controls.Add(MessageTwoText);
            this.Controls.Add(MessageThreeText);
            this.Controls.Add(DeleteText);
            this.Controls.Add(PreserveText);
            this.Controls.Add(ChangeText);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CloseButton)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
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

        public void Show()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            HeaderImage.Visible = false;
            MessageText.Text = "WARNING: The repository database already exists on the specified SQL server instance.";
            MessageOneText.Text = "" + Environment.NewLine + Environment.NewLine + Environment.NewLine + "If you want to remove the repository database for this instance and continue the setup, click" + Environment.NewLine + "We recommend backing up the repository database before performing the deletion. " + Environment.NewLine + "(SQLcompliance and SQLcomplianceProcessing)";
            //DeleteText.Location = new Point(437, 122);
            MessageTwoText.Text = "" + Environment.NewLine + Environment.NewLine + Environment.NewLine + "If you want to keep the existing repository database for this instance and continue setup, click";
            MessageThreeText.Text = "" + Environment.NewLine + Environment.NewLine + "If you want to specify a different SQL server instance, click";
            if (MessageOneText.Text.Length > 200)
            {
                int height = (MessageOneText.Text.Length - 200) / 2;
                this.Size = new Size(555, this.Size.Height + height);
                DeleteButton.Location = new Point(127, this.Size.Height - 50);
                OkButton.Location = new Point(217, this.Size.Height - 50);
                ChangeButton.Location = new Point(307, this.Size.Height - 50);
                exitButton.Location = new Point(397, this.Size.Height - 50);
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
            if (message.Length > 1000)
            {
                int height = (message.Length - 1000) / 2;
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
            this.isChangeClick = true;
            this.Close();
        }

        public void OkButton_Click(object sender, EventArgs e)
        {
            this.isOkClick = true;
            this.Close();
        }
        public void DeleteButton_Click(object sender, EventArgs e)
        {
            this.isDeleteClick = true;
            this.Close();
        }
        public void ChangeButton_Click(object sender, EventArgs e)
        {
            this.isChangeClick = true;
            this.Close();
        }
        public void exitButton_Click(object sender, EventArgs e)
        {
            this.isExitClick = true;
            this.Close();
        }
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (Constants.SourceSansProRegular != null)
            {
                //MessageText.Font = new System.Drawing.Font(Constants.SourceSansProRegular, 10.5F, this.Font.Style);
                MessageText.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            }
        }
    }
}
