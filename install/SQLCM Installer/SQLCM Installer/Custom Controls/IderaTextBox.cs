using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLCM_Installer.Custom_Controls
{
    class IderaTextBox : System.Windows.Forms.Panel
    {
        private static Color backColor = System.Drawing.Color.White;
        private static Color _fore = System.Drawing.ColorTranslator.FromHtml("#483e2f");        
        private static Color borderColor = Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
        private static Color borderColorFocus = Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(213)))), ((int)(((byte)(229)))));
        private static Padding _margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
        private static Color _foreFocus = System.Drawing.ColorTranslator.FromHtml("#006089");
        private static string buttonText = String.Empty;
        public string WatermarkText = string.Empty;
        TextBox textBox = new TextBox();

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public override string Text
        {
            get{return textBox.Text;}
            set{textBox.Text = value;}
        }

        public char PasswordChar
        {
            get { return textBox.PasswordChar; }
            set { textBox.PasswordChar = value; }
        }

        public HorizontalAlignment TextAlign
        {
            get { return textBox.TextAlign; }
            set { textBox.TextAlign = value; }
        }

        private static Size _minSize = new System.Drawing.Size(50, 28);

        private bool _active;
        public IderaTextBox()
            : base()
        {
            base.BackColor = backColor;
            base.ForeColor = _fore;            
            base.Margin = _margin;
            base.MinimumSize = _minSize;
            base.Size = _minSize;
            base.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            base.Anchor = AnchorStyles.None;
            base.BorderStyle = BorderStyle.None;  
            textBox.BackColor = Color.White;
            textBox.BorderStyle = BorderStyle.None;
            textBox.Enter += textBox_Enter;
            textBox.Leave += textBox_Leave;
            textBox.MouseEnter += textBox_MouseEnter;
            textBox.MouseLeave += textBox_MouseLeave;
            textBox.LostFocus += textBox_LostFocus;
            textBox.GotFocus += textBox_GotFocus;
            textBox.Text = this.Text;
            textBox.Location = new Point(4, 5);
            base.Controls.Add(textBox);
        }

        public bool UseSystemPasswordChar
        {
            get { return this.textBox.UseSystemPasswordChar; }
            set { this.textBox.UseSystemPasswordChar = value; }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (Constants.SourceSansProRegular != null)
            {
                this.Font = new System.Drawing.Font(Constants.SourceSansProRegular, 14F, FontStyle.Regular, GraphicsUnit.Pixel);
                this.textBox.Font = this.Font;
            }

            if (this.Text == string.Empty)
            {
                this.Text = WatermarkText;
            }
        }

        protected override void OnEnter(System.EventArgs e)
        {
            base.OnEnter(e);
            if (this.Text == WatermarkText)
            {
                this.Text = string.Empty;
            }
            this.Cursor = Cursors.IBeam;
            _active = true;
            this.Invalidate();
        }

        protected override void OnLeave(System.EventArgs e)
        {
            base.OnLeave(e);
            this.Cursor = Cursors.Arrow;
            if (this.Text.Length == 0)
            {
                this.Text = WatermarkText;
            }
            if (!textBox.Focused)
            {
                _active = false;
                this.Invalidate();
            }
        }

        protected override void OnMouseEnter(System.EventArgs e)
        {
            base.OnMouseEnter(e);
            this.Cursor = Cursors.IBeam;
            _active = true;
            this.Invalidate();
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            base.OnMouseLeave(e);
            this.Cursor = Cursors.Arrow;
            if (!textBox.Focused)
            {
                _active = false;
                this.Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            _active = true;
            textBox.Select();
            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Rectangle r = new Rectangle(0, 0, this.ClientRectangle.Width -1, this.ClientRectangle.Height - 1);
            if (_active)
            {
                Pen p = new Pen(borderColorFocus, 1);
                e.Graphics.DrawRectangle(p, r);
                if (string.IsNullOrEmpty(WatermarkText) || this.textBox.Text != WatermarkText)
                {
                    textBox.ForeColor = _foreFocus;
                }
                p.Dispose();
            }
            else
            {
                Pen p = new Pen(borderColor, 1);
                e.Graphics.DrawRectangle(p, r);
                if (!string.IsNullOrEmpty(WatermarkText) && this.textBox.Text == WatermarkText)
                {
                    textBox.ForeColor = Color.FromArgb(146, 136, 117);
                }
                else
                {
                    textBox.ForeColor = _fore;
                }
                p.Dispose();
            }
           
        }

        private void textBox_Enter(object sender, EventArgs e)
        {
            OnEnter(e);
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            OnLeave(e);
        }

        private void textBox_MouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(e);
        }

        private void textBox_MouseLeave(object sender, EventArgs e)
        {
            OnMouseLeave(e);
        }

        private void textBox_LostFocus(object sender, EventArgs e)
        {
            _active = false;
            this.Invalidate();
        }

        private void textBox_GotFocus(object sender, EventArgs e)
        {
            _active = true;
            this.Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);                     
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.textBox.Width = base.Width - 7;
        }
    }
}
