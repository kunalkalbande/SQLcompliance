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
    public class IderaWarningMessageBox : System.Windows.Forms.Form
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

        public IderaWarningMessageBox()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IderaMessageBox));
            BorderPanel.Location = new Point(500, 195);
            BorderPanel.BackColor = Color.FromArgb(170, 170, 170);
            Header.Text = "Repository Warning";
            Header.AutoSize = true;
            Header.ForeColor = Color.White;
            HeaderImage.Image = Resources.infoicon;
            HeaderImage.Location = new Point(25, 25);
            Header.Location = new Point(44, 25);
            MessageText.ForeColor = Color.FromArgb(54, 54, 54);
            MessageText.Location = new Point(15, 84);
            MessageText.MaximumSize = new System.Drawing.Size(465, 0);
            MessageText.AutoSize = true;
            MessageText.Font = new Font(this.Font, FontStyle.Bold);

            MessageOneText.ForeColor = Color.FromArgb(54, 54, 54);
            MessageOneText.Location = new Point(27, 110);
            MessageOneText.MaximumSize = new System.Drawing.Size(460, 0);
            MessageOneText.AutoSize = true;

            MessageTwoText.ForeColor = Color.FromArgb(54, 54, 54);
            MessageTwoText.Location = new Point(27, 180);
            MessageTwoText.MaximumSize = new System.Drawing.Size(460, 0);
            MessageTwoText.AutoSize = true;

            MessageThreeText.ForeColor = Color.FromArgb(54, 54, 54);
            MessageThreeText.Location = new Point(27, 230);
            MessageThreeText.MaximumSize = new System.Drawing.Size(460, 0);
            MessageThreeText.AutoSize = true;

            DeleteText.ForeColor = Color.FromArgb(54, 54, 54);
            DeleteText.Location = new Point(444, 110);
            DeleteText.MaximumSize = new System.Drawing.Size(465, 0);
            DeleteText.Text = "Delete ,";
            DeleteText.Font = new Font(this.Font, FontStyle.Bold);
            DeleteText.AutoSize = true;

            PreserveText.ForeColor = Color.FromArgb(54, 54, 54);
            PreserveText.Location = new Point(379, 180);
            PreserveText.MaximumSize = new System.Drawing.Size(465, 0);
            PreserveText.Text = "Preserve";
            PreserveText.Font = new Font(this.Font, FontStyle.Bold);
            PreserveText.AutoSize = true;

            ChangeText.ForeColor = Color.FromArgb(54, 54, 54);
            ChangeText.Location = new Point(347, 230);
            ChangeText.MaximumSize = new System.Drawing.Size(465, 0);
            ChangeText.Text = "Change";
            ChangeText.Font = new Font(this.Font, FontStyle.Bold);
            ChangeText.AutoSize = true;

            this.BackColor = Color.White;
            CloseButton.Image = Resources.close_icon;
            CloseButton.SizeMode = PictureBoxSizeMode.AutoSize;

            DeleteButton.Text = "Delete";
            DeleteButton.ForeColor = Color.FromArgb(72, 62, 47);
            DeleteButton.Size = new Size(53, 28);
            DeleteButton.Location = new Point(127, 280);
            topPanel.Location = new Point(0, 0);
            topPanel.Size = new Size(588, 54);
            topPanel.BackColor = Color.FromArgb(183, 175, 167);
            DeleteButton.Click += DeleteButton_Click;

            OkButton.Text = "Preserve";
            OkButton.ForeColor = Color.FromArgb(72, 62, 47);
            CloseButton.Location = new Point(463, 20);
            OkButton.AutoSize = true;
            OkButton.Size = new Size(73, 28);
            OkButton.Location = new Point(197, 280);
            topPanel.Location = new Point(0, 0);
            topPanel.Size = new Size(588, 54);
            topPanel.BackColor = Color.FromArgb(183, 175, 167);
            CloseButton.Click += CloseButton_Click;
            OkButton.Click += OkButton_Click;

            ChangeButton.Text = "Change";
            ChangeButton.ForeColor = Color.FromArgb(72, 62, 47);
            ChangeButton.Size = new Size(53, 28);
            ChangeButton.Location = new Point(287, 280);
            topPanel.Location = new Point(0, 0);
            topPanel.Size = new Size(588, 54);
            topPanel.BackColor = Color.FromArgb(183, 175, 167);
            ChangeButton.Click += ChangeButton_Click;

            exitButton.Text = "Cancel";
            exitButton.ForeColor = Color.FromArgb(72, 62, 47);
            exitButton.Size = new Size(53, 28);
            exitButton.Location = new Point(357, 280);
            topPanel.Location = new Point(0, 0);
            topPanel.Size = new Size(588, 54);
            topPanel.BackColor = Color.FromArgb(183, 175, 167);
            exitButton.Click += exitButton_Click;

            topPanel.MouseDown += topPanel_MouseDown;
            HeaderImage.MouseDown += topPanel_MouseDown;
            Header.MouseDown += topPanel_MouseDown;
            CloseButton.MouseEnter += CloseButton_MouseEnter;
            CloseButton.MouseLeave += CloseButton_MouseLeave;
            topPanel.Controls.Add(CloseButton);
            topPanel.Controls.Add(Header);
            topPanel.Controls.Add(HeaderImage);
            this.Size = new Size(500, 320);
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
            //StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.Append("WARNING : The Repository already exists on the specified SQL Server instance");
            //stringBuilder.Append("" + Environment.NewLine + "");
            //stringBuilder.Append("" + Environment.NewLine + "");
            //stringBuilder.Append("" + Environment.NewLine + "");
            //stringBuilder.Append("To remove the Repository databases from this instance and continue setup, click Delete, " + Environment.NewLine + "we recommend backing up the Repository databases.");
            //stringBuilder.Append("" + Environment.NewLine + "");
            //stringBuilder.Append("(SQLcompliance and SQLcomplianceProcessing)");
            //stringBuilder.Append("" + Environment.NewLine + "");
            //stringBuilder.Append("" + Environment.NewLine + "");
            //stringBuilder.Append("" + Environment.NewLine + "");
            //stringBuilder.Append("" + Environment.NewLine + "");
            //stringBuilder.Append("To Keep the existing Repositroy database and continue setup, click Preserve.");
            //stringBuilder.Append("" + Environment.NewLine + "");
            //stringBuilder.Append("" + Environment.NewLine + "");
            //stringBuilder.Append("" + Environment.NewLine + "");
            //stringBuilder.Append("" + Environment.NewLine + "");
            //stringBuilder.Append("To go back and specify a different SQL Server instance, click Change.");
            //MessageText.Text = stringBuilder.ToString();
            MessageText.Text = "WARNING : The Repository already exists on the specified SQL Server instance";
            MessageOneText.Text = "To remove the Repository databases from this instance and continue setup, click  " + Environment.NewLine + " we recommend backing up the Repository databases. " + Environment.NewLine + " (SQLcompliance and SQLcomplianceProcessing)";
            MessageTwoText.Text = "To Keep the existing Repositroy database and continue setup, click ";
            MessageThreeText.Text = "To go back and specify a different SQL Server instance, click ";
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
                MessageText.Font = new System.Drawing.Font(Constants.SourceSansProRegular, 10.5F, this.Font.Style);
            }
        }
    }
}
