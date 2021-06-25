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
    class IderaCheckBox : System.Windows.Forms.Panel
    {
        CheckBox checkBox = new CheckBox();
        PictureBox customImage = new PictureBox();
        private Bitmap checkedImg = Resources.checkboxcheckedimage;
        private Bitmap uncheckedImg = Resources.checkboximage;
        private bool isBoldFont = false;
        private bool _isDisabled = false;

        public IderaCheckBox()
        {
            this.AutoSize = true;
            customImage.Image = Resources.checkboximage;
            customImage.SizeMode = PictureBoxSizeMode.AutoSize;
            customImage.Location = new Point(0, 4);
            this.Size = new Size(250, 25);
            this.Controls.Add(checkBox);
            this.Controls.Add(customImage);
            customImage.BringToFront();
            this.checkBox.AutoSize = true;
            this.checkBox.ForeColor = Color.FromArgb(71, 62, 46);
            this.checkBox.CheckedChanged += checkBox_CheckedChanged;
            this.customImage.Click += customImage_Click;
            this.checkBox.MouseHover += IderaCheckBox_MouseHover;
            this.checkBox.MouseLeave += checkBox_MouseLeave;
            this.customImage.DoubleClick += customImage_Click;
            this.customImage.MouseHover += customImage_MouseHover;
            this.customImage.MouseLeave += customImage_MouseLeave;
        }

        public IderaCheckBox(bool boldFont)
        {
            this.AutoSize = true;
            customImage.Image = Resources.checkboximage;
            customImage.SizeMode = PictureBoxSizeMode.AutoSize;
            customImage.Location = new Point(0, 4);
            this.Size = new Size(250, 25);
            this.Controls.Add(checkBox);
            this.Controls.Add(customImage);
            customImage.BringToFront();
            this.checkBox.AutoSize = true;
            this.checkBox.ForeColor = Color.FromArgb(71, 62, 46);
            this.checkBox.CheckedChanged += checkBox_CheckedChanged;
            this.customImage.Click += customImage_Click;
            this.checkBox.MouseHover += IderaCheckBox_MouseHover;
            this.checkBox.MouseLeave += checkBox_MouseLeave;
            this.customImage.DoubleClick += customImage_Click;
            this.customImage.MouseHover += customImage_MouseHover;
            this.customImage.MouseLeave += customImage_MouseLeave;
            this.isBoldFont = boldFont;
        }

        public override string Text
        {
            get { return this.checkBox.Text; }
            set { this.checkBox.Text = value; }
        }

        public bool Disabled
        {
            get
            {
                return _isDisabled;
            }
            set
            {
                if (value)
                {
                    if (this.checkBox.Checked)
                    {
                        this.customImage.Image = Resources.checboxdisabled;
                    }
                    else
                    {
                        this.customImage.Image = Resources.checkboxDisabledUncheck;
                    }
                    this.checkBox.ForeColor = Color.FromArgb(146, 136, 117);
                    this.customImage.MouseHover -= customImage_MouseHover;
                    this.customImage.MouseLeave -= customImage_MouseLeave;
                    this.checkBox.MouseHover -= IderaCheckBox_MouseHover;
                    this.checkBox.MouseLeave -= checkBox_MouseLeave;
                    this.customImage.Click -= customImage_Click;
                    this.checkBox.CheckedChanged -= checkBox_CheckedChanged;
                    this.checkBox.AutoCheck = false;
                    _isDisabled = true;
                }
                else
                {
                    if (this.checkBox.Checked)
                    {
                        this.customImage.Image = Resources.checkboxcheckedimage;
                    }
                    else
                    {
                        this.customImage.Image = Resources.checkboximage;
                    }
                    this.checkBox.ForeColor = Color.FromArgb(71, 62, 46);
                    this.customImage.MouseHover += customImage_MouseHover;
                    this.customImage.MouseLeave += customImage_MouseLeave;
                    this.checkBox.MouseHover += IderaCheckBox_MouseHover;
                    this.checkBox.MouseLeave += checkBox_MouseLeave;
                    this.customImage.Click += customImage_Click;
                    this.checkBox.CheckedChanged += checkBox_CheckedChanged;
                    this.checkBox.AutoCheck = true;
                    _isDisabled = false;
                }
            }
        }

        public bool Checked
        {
            get { return this.checkBox.Checked; }
            set { this.checkBox.Checked = value; }
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            base.OnClick(e);
            if (checkBox.Checked)
            {
                customImage.Image = checkedImg;
            }
            else
            {
                customImage.Image = uncheckedImg;
            }
        }

        private void IderaCheckBox_MouseHover(object sender, EventArgs e)
        {
            this.checkBox.ForeColor = Color.FromArgb(0, 96, 137);
            checkedImg = Resources.chkboxhover;
            uncheckedImg = Resources.chkboxhoverOff;
            if (this.checkBox.Checked)
            {
                this.customImage.Image = checkedImg;
            }
            else
            {
                customImage.Image = uncheckedImg;
            }
        }

        private void checkBox_MouseLeave(object sender, EventArgs e)
        {
            this.checkBox.ForeColor = Color.FromArgb(71, 62, 46);
            checkedImg = Resources.checkboxcheckedimage;
            uncheckedImg = Resources.checkboximage;
            if (this.checkBox.Checked)
            {
                customImage.Image = checkedImg;
            }
            else
            {
                customImage.Image = uncheckedImg;
            }
        }

        private void customImage_Click(object sender, EventArgs e)
        {
            checkBox.Checked = !checkBox.Checked;
        }

        private void customImage_MouseHover(object sender, EventArgs e)
        {
            checkedImg = Resources.chkboxhover;
            uncheckedImg = Resources.chkboxhoverOff;
            if (this.checkBox.Checked)
            {
                this.customImage.Image = checkedImg;
            }
            else
            {
                customImage.Image = uncheckedImg;
            }
            this.checkBox.ForeColor = Color.FromArgb(0, 96, 137);
        }

        private void customImage_MouseLeave(object sender, EventArgs e)
        {
            this.checkBox.ForeColor = Color.FromArgb(71, 62, 46);
            checkedImg = Resources.checkboxcheckedimage;
            uncheckedImg = Resources.checkboximage;
            if (this.checkBox.Checked)
            {
                customImage.Image = checkedImg;
            }
            else
            {
                customImage.Image = uncheckedImg;
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (isBoldFont)
            {
                if (Constants.SourceSansProBold != null)
                {
                    this.checkBox.Font = new System.Drawing.Font(Constants.SourceSansProBold, 10.5F, FontStyle.Bold);
                }
            }
            else
            {
                if (Constants.SourceSansProRegular != null)
                {
                    this.checkBox.Font = new System.Drawing.Font(Constants.SourceSansProRegular, 10.5F, this.checkBox.Font.Style);
                }
            }
        }
    }
}
