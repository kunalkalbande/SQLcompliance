using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace SQLCM_Installer.Custom_Controls
{
    public class IderaButton : System.Windows.Forms.Button
    {
        private Color color1 = Color.FromArgb(196, 186, 163);
        private Color color2 = Color.Yellow;
        private Color m_hovercolor1 = Color.Turquoise;
        private Color m_hovercolor2 = Color.DarkSlateGray;
        private int angle = 90;
        private int textX = 100;
        private int textY = 25;
        public String text = "";
        public Color buttonborder_1 = Color.FromArgb(220, 220, 220);
        public Color buttonborder_2 = Color.FromArgb(150, 150, 150);
        public Boolean showButtonText = true;
        public int borderWidth = 2;
        public Color borderColor = Color.Transparent;
        private bool _disabled = false;
        

        public String ButtonText
        {
            get { return text; }
            set { text = value; Invalidate(); }
        }

        public int BorderWidth
        {
            get { return borderWidth; }
            set { borderWidth = value; Invalidate(); }
        }

        public bool Disabled
        {
            get { return _disabled;}
            set
            {
                if (value)
                {
                    this.ForeColor = Color.FromArgb(146, 136, 117);
                    this.BackColor = Color.FromArgb(196, 186, 163);
                    this.Cursor = Cursors.No;
                    this.TabStop = false;
                }
                else
                {
                    this.ForeColor =Color.FromArgb(72, 62, 47);
                    this.TabStop = true;
                }
                _disabled = value;
            }
        }

        void SetBorderColor(Color bdrColor)
        {
            int red = bdrColor.R - 40;
            int green = bdrColor.G - 40;
            int blue = bdrColor.B - 40;
            if (red < 0)
                red = 0;
            if (green < 0)
                green = 0;
            if (blue < 0)
                blue = 0;

            buttonborder_1 = Color.FromArgb(red, green, blue);
            buttonborder_2 = bdrColor;
        }

        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                if (borderColor == Color.Transparent)
                {
                    buttonborder_1 = Color.FromArgb(220, 220, 220);
                    buttonborder_2 = Color.FromArgb(150, 150, 150);
                }
                else
                {
                    SetBorderColor(borderColor);
                }
            }
        }

        public Color StartColor
        {
            get { return color1; }
            set { color1 = value; Invalidate(); }
        }
        public Color EndColor
        {
            get { return color2; }
            set { color2 = value; Invalidate(); }
        }

        public int GradientAngle
        {
            get { return angle; }
            set { angle = value; Invalidate(); }
        }

        public int TextLocation_X
        {
            get { return textX; }
            set { textX = value; Invalidate(); }
        }
        public int TextLocation_Y
        {
            get { return textY; }
            set { textY = value; Invalidate(); }
        }

        public Boolean ShowButtontext
        {
            get { return showButtonText; }
            set { showButtonText = value; Invalidate(); }
        }

        public IderaButton()
        {
            this.Size = new Size(100, 40);
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.BackColor = Color.FromArgb(196, 186, 163);
            this.Text = text;
            this.ForeColor = Color.FromArgb(72, 62, 47);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (Constants.SourceSansProBold != null)
            {
                this.Font = new System.Drawing.Font(Constants.SourceSansProBold, 10.5F, FontStyle.Bold);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (Disabled)
                return;
            base.OnMouseEnter(e);
            this.BackColor = Color.FromArgb(0, 96, 137);
            this.Cursor = Cursors.Hand;
            this.ForeColor = Color.White;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (Disabled)
                return;
            base.OnMouseLeave(e);
            this.BackColor = Color.FromArgb(196, 186, 163);
            this.Cursor = Cursors.Arrow;
            this.ForeColor = Color.FromArgb(72, 62, 47);
        }

        protected override void OnClick(EventArgs e)
        {
            if (Disabled)
                return;
            base.OnClick(e);
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            if (Disabled)
                return;
            base.OnDoubleClick(e);
        }
    }
}