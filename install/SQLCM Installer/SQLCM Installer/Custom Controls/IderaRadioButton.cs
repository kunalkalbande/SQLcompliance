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
    class IderaRadioButton : Panel
    {
        RadioButton radio = new RadioButton();
        PictureBox customImage = new PictureBox();
        List<Control> allRadioButtons = new List<Control>();
        private Bitmap checkedImg = Resources.radiobuttoncheckedimage;
        private Bitmap uncheckedImg = Resources.radiobuttonimage;
        private bool isBoldFont = false;
        private bool _isDisabled = false;
        private bool hideRadio = false;

        public IderaRadioButton()
        {
            this.AutoSize = true;
            customImage.Image = Resources.radiobuttonimage;
            customImage.SizeMode = PictureBoxSizeMode.AutoSize;
            customImage.Location = new Point(0, 4);
            this.Controls.Add(radio);
            this.Controls.Add(customImage);
            customImage.BringToFront();
            this.radio.AutoSize = true;
            this.radio.ForeColor = Color.FromArgb(71, 62, 46);
            this.radio.CheckedChanged += radio_CheckedChanged;
            this.radio.MouseHover += radio_MouseHover;
            this.radio.MouseLeave += radio_MouseLeave;
            this.customImage.MouseHover += customImage_MouseHover;
            this.customImage.MouseLeave += customImage_MouseLeave;
            this.customImage.Click += customImage_Click;
        }

        public IderaRadioButton(bool boldFont)
        {
            this.AutoSize = true;
            customImage.Image = Resources.radiobuttonimage;
            customImage.SizeMode = PictureBoxSizeMode.AutoSize;
            customImage.Location = new Point(0, 4);
            this.Controls.Add(radio);
            this.Controls.Add(customImage);
            customImage.BringToFront();
            this.radio.AutoSize = true;
            this.radio.ForeColor = Color.FromArgb(71, 62, 46);
            this.radio.CheckedChanged += radio_CheckedChanged;
            this.radio.MouseHover += radio_MouseHover;
            this.radio.MouseLeave += radio_MouseLeave;
            this.customImage.MouseHover += customImage_MouseHover;
            this.customImage.MouseLeave += customImage_MouseLeave;
            this.customImage.Click += customImage_Click;
            isBoldFont = boldFont;
        }

        public void HideRadio(bool hide)
        {
            hideRadio = hide;
            if (hide)
            {
                customImage.Image = Resources.radioHidden;
            }
            else
            {
                if (radio.Checked)
                {
                    customImage.Image = Resources.radiobuttoncheckedimage;
                }
                else
                {
                    customImage.Image = Resources.radiobuttonimage;
                }
            }
        }

        public override string Text
        {
            get { return this.radio.Text; }
            set { this.radio.Text = value; }
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
                    if (this.radio.Checked)
                    {
                        if (!hideRadio)
                            this.customImage.Image = Resources.radioCheckedDisabled;
                    }
                    else
                    {
                        if (!hideRadio)
                            this.customImage.Image = Resources.radiodisabled;
                    }
                    this.radio.ForeColor = Color.FromArgb(146, 136, 117);
                    this.customImage.MouseHover -= customImage_MouseHover;
                    this.customImage.MouseLeave -= customImage_MouseLeave;
                    this.radio.MouseHover -= radio_MouseHover;
                    this.radio.MouseLeave -= radio_MouseLeave;
                    this.customImage.Click -= customImage_Click;
                    this.radio.CheckedChanged -= radio_CheckedChanged;
                    _isDisabled = true;
                }
                else
                {
                    if (this.radio.Checked)
                    {
                        if (!hideRadio)
                            this.customImage.Image = Resources.radiobuttoncheckedimage;
                    }
                    else
                    {
                        if (!hideRadio)
                            this.customImage.Image = Resources.radiobuttonimage;
                    }
                    this.radio.ForeColor = Color.FromArgb(71, 62, 46);
                    this.customImage.MouseHover += customImage_MouseHover;
                    this.customImage.MouseLeave += customImage_MouseLeave;
                    this.radio.MouseHover += radio_MouseHover;
                    this.radio.MouseLeave += radio_MouseLeave;
                    this.customImage.Click += customImage_Click;
                    this.radio.CheckedChanged += radio_CheckedChanged;
                    _isDisabled = false;
                }
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (isBoldFont)
            {
                if (Constants.SourceSansProBold != null)
                {
                    this.radio.Font = new System.Drawing.Font(Constants.SourceSansProBold, this.radio.Font.Size, FontStyle.Bold);
                }
            }
            else
            {
                if (Constants.SourceSansProRegular != null)
                {
                    this.radio.Font = new System.Drawing.Font(Constants.SourceSansProRegular, 10.5F, this.radio.Font.Style);
                }
            }
            foreach (Control control in this.Parent.Controls)
            {
                if (control is IderaRadioButton && !((IderaRadioButton)control).Equals(this))
                {
                    allRadioButtons.Add(control);
                }
            }
        }

        public bool Checked
        {
            get { return this.radio.Checked; }
            set { this.radio.Checked = value; }
        }

        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            base.OnClick(e);
            if (radio.Checked)
            {
                if (!hideRadio)
                    customImage.Image = checkedImg;
                foreach (Control c in allRadioButtons)
                {
                    ((IderaRadioButton)c).Checked = false;
                }
            }
            else
            {
                if (!hideRadio)
                    customImage.Image = uncheckedImg;
            }
        }

        private void radio_MouseHover(object sender, EventArgs e)
        {
            this.radio.ForeColor = Color.FromArgb(0, 96, 137);
            checkedImg = Resources.radiobtnhover;
            uncheckedImg = Resources.radiobtnhoverOff;
            if (this.radio.Checked)
            {
                if (!hideRadio)
                    this.customImage.Image = checkedImg;
            }
            else
            {
                if (!hideRadio)
                    customImage.Image = uncheckedImg;
            }
        }

        private void radio_MouseLeave(object sender, EventArgs e)
        {
            this.radio.ForeColor = Color.FromArgb(71, 62, 46);
            checkedImg = Resources.radiobuttoncheckedimage;
            uncheckedImg = Resources.radiobuttonimage;
            if (this.radio.Checked)
            {
                if (!hideRadio)
                    customImage.Image = checkedImg;
            }
            else
            {
                if (!hideRadio)
                    customImage.Image = uncheckedImg;
            }
        }

        private void customImage_MouseHover(object sender, EventArgs e)
        {
            checkedImg = Resources.radiobtnhover;
            uncheckedImg = Resources.radiobtnhoverOff;
            if (this.radio.Checked)
            {
                if (!hideRadio)
                    this.customImage.Image = checkedImg;
            }
            else
            {
                if (!hideRadio)
                    customImage.Image = uncheckedImg;
            }
            this.radio.ForeColor = Color.FromArgb(0, 96, 137);
        }

        private void customImage_MouseLeave(object sender, EventArgs e)
        {
            checkedImg = Resources.radiobuttoncheckedimage;
            uncheckedImg = Resources.radiobuttonimage;
            if (this.radio.Checked)
            {
                if (!hideRadio)
                    customImage.Image = checkedImg;
            }
            else
            {
                if (!hideRadio)
                    customImage.Image = uncheckedImg;
            }
            this.radio.ForeColor = Color.FromArgb(71, 62, 46);
        }

        private void customImage_Click(object sender, EventArgs e)
        {
            if (!this.radio.Checked)
            {
                this.radio.Checked = true;
            }
        }

        public void UpdateRadioImageLocation()
        {
            this.customImage.Location = new Point(0, 6);
        }
    }
}
