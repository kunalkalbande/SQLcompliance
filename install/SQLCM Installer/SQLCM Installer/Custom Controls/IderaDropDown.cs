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
    class IderaDropDown : System.Windows.Forms.Panel
    {
        ComboBox dropDown = new ComboBox();
        PictureBox customImage = new PictureBox();
        public Color HighlightColor { get; set; }
        private bool _isDisabled = false;
        private IderaLabel dropDownText= new IderaLabel();

        int height = 32;

        public IderaDropDown()
        {
            this.Size = new Size(287, 32);
            this.Font = new Font("Source Sans Pro", 10.5F);
            dropDownText.Visible = false;
            this.dropDownText.Location = new Point(2,2);
            customImage.Image = Resources.dropdownimage;
            customImage.SizeMode = PictureBoxSizeMode.AutoSize;
            customImage.Location = new Point(262, 1);
            customImage.BackColor = Color.Transparent;
            customImage.BorderStyle = BorderStyle.None;
            this.dropDown.Size = new Size(287, 32);
            this.dropDown.Font = new Font("Source Sans Pro", 10.5F);
            this.dropDown.ForeColor = Color.FromArgb(72, 62, 47);
            this.HighlightColor = Color.FromArgb(0, 96, 137);
            this.dropDown.DrawMode = DrawMode.OwnerDrawFixed;
            this.Controls.Add(dropDownText);
            this.Controls.Add(dropDown);
            this.Controls.Add(customImage);
            customImage.BringToFront();
            this.dropDown.DrawItem += new DrawItemEventHandler(ComboTheme_DrawItem);
            this.dropDown.Enter += dropdown_Enter;
            this.dropDown.Leave += dropDown_Leave;
            this.dropDown.Click += dropDown_Click;
            this.dropDown.TextChanged+=dropDown_TextChanged;
            this.customImage.Click += this.customImage_Click;
            this.height = this.Height;
        }

        private void dropDown_TextChanged(object sender, EventArgs e)
        {
            base.OnTextChanged(e);
        }

        public void SetItems(List<string> itemList)
        {
            foreach(string item in itemList)
            {
                this.dropDown.Items.Add(item);
            }
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
                    dropDownText.Text = dropDown.Text;
                    dropDownText.Visible = true;
                    this.customImage.Image = Resources.dropdownDisabled;
                    this.customImage.Location = new Point(260, -1);
                    this.Height = customImage.Height; 
                    this.dropDown.Visible = false;
                    this.BackColor = Color.FromArgb(146, 136, 117);
                    this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                    this.customImage.Enabled = !value;
                }
                else
                {
                    dropDown.Visible = true;
                    dropDownText.Visible = false;
                    this.customImage.Location = new Point(262, 1);
                    this.Height = height; 
                    this.BackColor = Color.White;
                    this.customImage.Image = Resources.dropdownimage;
                    this.BorderStyle = System.Windows.Forms.BorderStyle.None;
                    this.customImage.Enabled = !value;
                }
            }
        }

        public string SelectedItem
        {
            get { return this.dropDown.Text; }
        }

        public void SetDefaultItem(string item)
        {
            this.dropDown.Text = item;
            this.dropDownText.Text = item;
        }

        public void RemoveItems()
        {
            this.dropDown.Items.Clear();
        }

        protected void dropDown_Click(object sender, EventArgs e)
        {
            base.OnClick(e);
            this.dropDown.ForeColor = Color.FromArgb(0, 96, 137);
        }

        protected void dropdown_Enter(object sender, EventArgs e)
        {
            base.OnEnter(e);
            this.dropDown.ForeColor = Color.FromArgb(0, 96, 137);
        }

        protected void dropDown_Leave(object sender, EventArgs e)
        {
            base.OnLeave(e);
            this.dropDown.ForeColor = Color.FromArgb(72, 62, 47);
        }

        protected void customImage_Click(object sender, EventArgs e)
        {
            this.dropDown.DroppedDown = true;
        }

        public void ComboTheme_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox box = ((ComboBox)sender);
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(HighlightColor), e.Bounds);
                this.dropDown.ForeColor = Color.White;
            }

            else
            {
                e.Graphics.FillRectangle(new SolidBrush(box.BackColor), e.Bounds);
                this.dropDown.ForeColor = Color.FromArgb(0, 96, 137);
            }

            e.Graphics.DrawString(box.Items[e.Index].ToString(), e.Font, new SolidBrush(box.ForeColor), new Point(e.Bounds.X, e.Bounds.Y));
            e.DrawFocusRectangle();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
    }
}
