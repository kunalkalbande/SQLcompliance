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
    class IderaMessageBoxWithOption : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Panel panelFooter;
        private System.Windows.Forms.PictureBox pictureCloseIcon;
        private IderaTitleLabel labelHeader;
        private IderaLabel labelMessage;
        private IderaButton cancelButton;
        private IderaButton finishButton;
        private System.Windows.Forms.Panel panelHeader;
        public bool isCancelClick = false;
        public bool isFinishClick = false;
        private CustomDropShadow customDropShadow;
        private CustomBorderShadow customBorderShadow;

        #region Window Draggable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        public static System.Drawing.Point location;

        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        #endregion

        public IderaMessageBoxWithOption()
        {
            this.panelHeader = new System.Windows.Forms.Panel();
            this.pictureCloseIcon = new System.Windows.Forms.PictureBox();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.labelHeader = new SQLCM_Installer.Custom_Controls.IderaTitleLabel();
            this.labelMessage = new SQLCM_Installer.Custom_Controls.IderaLabel();
            this.cancelButton = new SQLCM_Installer.Custom_Controls.IderaButton();
            this.finishButton = new SQLCM_Installer.Custom_Controls.IderaButton();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCloseIcon)).BeginInit();
            this.panelFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(175)))), ((int)(((byte)(167)))));
            this.panelHeader.Controls.Add(this.labelHeader);
            this.panelHeader.Controls.Add(this.pictureCloseIcon);
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(500, 55);
            this.panelHeader.TabIndex = 0;
            // 
            // pictureCloseIcon
            // 
            this.pictureCloseIcon.Image = global::SQLCM_Installer.Properties.Resources.close_icon;
            this.pictureCloseIcon.Location = new System.Drawing.Point(465, 20);
            this.pictureCloseIcon.Name = "pictureCloseIcon";
            this.pictureCloseIcon.Size = new System.Drawing.Size(17, 25);
            this.pictureCloseIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureCloseIcon.TabIndex = 0;
            this.pictureCloseIcon.TabStop = false;
            this.pictureCloseIcon.Click += pictureCloseIcon_Click;
            this.pictureCloseIcon.MouseHover += pictureCloseIcon_MouseHover;
            this.pictureCloseIcon.MouseLeave += pictureCloseIcon_MouseLeave;
            // 
            // panelFooter
            // 
            this.panelFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.panelFooter.Controls.Add(this.finishButton);
            this.panelFooter.Controls.Add(this.cancelButton);
            this.panelFooter.Location = new System.Drawing.Point(0, 228);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Size = new System.Drawing.Size(500, 60);
            this.panelFooter.TabIndex = 1;
            // 
            // labelHeader
            // 
            this.labelHeader.AutoSize = true;
            this.labelHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelHeader.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeader.ForeColor = System.Drawing.Color.White;
            this.labelHeader.Location = new System.Drawing.Point(14, 14);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(109, 25);
            this.labelHeader.TabIndex = 1;
            this.labelHeader.Text = "Cancel Install";
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.BackColor = System.Drawing.Color.Transparent;
            this.labelMessage.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.labelMessage.Location = new System.Drawing.Point(20, 74);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.MaximumSize = new System.Drawing.Size(475, 0);
            this.labelMessage.AutoSize = true;
            this.labelMessage.TabIndex = 2;
            this.labelMessage.Text = "ideraLabel2";
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.cancelButton.BorderColor = System.Drawing.Color.Transparent;
            this.cancelButton.BorderWidth = 2;
            this.cancelButton.ButtonText = "";
            this.cancelButton.EndColor = System.Drawing.Color.Yellow;
            this.cancelButton.FlatAppearance.BorderSize = 0;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.cancelButton.GradientAngle = 90;
            this.cancelButton.Location = new System.Drawing.Point(332, 16);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.ShowButtontext = true;
            this.cancelButton.Size = new System.Drawing.Size(66, 28);
            this.cancelButton.StartColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.TextLocation_X = 100;
            this.cancelButton.TextLocation_Y = 25;
            this.cancelButton.UseVisualStyleBackColor = false;
            this.cancelButton.Click += cancelButton_Click;
            // 
            // finishButton
            // 
            this.finishButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.finishButton.BorderColor = System.Drawing.Color.Transparent;
            this.finishButton.BorderWidth = 2;
            this.finishButton.ButtonText = "";
            this.finishButton.EndColor = System.Drawing.Color.Yellow;
            this.finishButton.FlatAppearance.BorderSize = 0;
            this.finishButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.finishButton.Font = new System.Drawing.Font("Source Sans Pro", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.finishButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(62)))), ((int)(((byte)(47)))));
            this.finishButton.GradientAngle = 90;
            this.finishButton.Location = new System.Drawing.Point(408, 16);
            this.finishButton.Name = "finishButton";
            this.finishButton.ShowButtontext = true;
            this.finishButton.Size = new System.Drawing.Size(66, 28);
            this.finishButton.StartColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(186)))), ((int)(((byte)(163)))));
            this.finishButton.TabIndex = 1;
            this.finishButton.Text = "Finish";
            this.finishButton.TextLocation_X = 100;
            this.finishButton.TextLocation_Y = 25;
            this.finishButton.UseVisualStyleBackColor = false;
            this.finishButton.Click += finishButton_Click;
            // 
            // IderaMessageBoxWithOption
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(500, 288);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.panelFooter);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "IderaMessageBoxWithOption";
            this.Load += new System.EventHandler(this.IderaMessageBoxWithOption_Load);
            this.Activated += new EventHandler(this.IderaMessageBoxWithOption_Activated);
            this.Deactivate += new EventHandler(this.IderaMessageBoxWithOption_Deactivated);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCloseIcon)).EndInit();
            this.panelFooter.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
            panelHeader.MouseDown += panelHeader_MouseDown;
            labelHeader.MouseDown += panelHeader_MouseDown;
        }

        private void panelHeader_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        public bool SetButtonText(string cancelButton, string finishButton, string labelHeaderText)
        {
            this.cancelButton.Text = cancelButton;
            this.finishButton.Text = finishButton;
            this.labelHeader.Text = labelHeaderText;
            return true;
        }

        public void Show(string message)
        {
            this.StartPosition = FormStartPosition.CenterParent;
            labelMessage.Text = message;
            this.ShowDialog();
        }

        public void Show(string message, bool isMainForm)
        {
            if (isMainForm)
            {
                this.StartPosition = FormStartPosition.CenterParent;
            }
            else
            {
                this.StartPosition = FormStartPosition.CenterScreen;
            }
            labelMessage.Text = message;
            this.ShowDialog();
        }

        private void IderaMessageBoxWithOption_Load(object sender, EventArgs e)
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

        private void IderaMessageBoxWithOption_Activated(object sender, EventArgs e)
        {
            if (customBorderShadow != null && customBorderShadow.ShadowColor != Color.FromArgb(24, 131, 215))
            {
                customDropShadow.ShadowBlur = 40;
                customDropShadow.RefreshShadow();

                customBorderShadow.ShadowColor = Color.FromArgb(24, 131, 215);
                customBorderShadow.RefreshShadow();
            }
        }

        private void IderaMessageBoxWithOption_Deactivated(object sender, EventArgs e)
        {
            if (customBorderShadow != null && customBorderShadow.ShadowColor != Color.Gray)
            {
                customDropShadow.ShadowBlur = 35;
                customDropShadow.RefreshShadow();

                customBorderShadow.ShadowColor = Color.Gray;
                customBorderShadow.RefreshShadow();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            isCancelClick = true;
            this.Close();
        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            isFinishClick = true;
            this.Close();
        }

        private void pictureCloseIcon_Click(object sender, EventArgs e)
        {
            isCancelClick = true;
            this.Close();
        }

        private void pictureCloseIcon_MouseHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void pictureCloseIcon_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (Constants.SourceSansProRegular != null)
            {
                labelMessage.Font = new System.Drawing.Font(Constants.SourceSansProRegular, 10.5F, this.Font.Style);
            }
        }
    }
}
