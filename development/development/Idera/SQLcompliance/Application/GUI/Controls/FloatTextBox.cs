using System;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
    public class FloatTextBox : TextBox
    {
        private System.ComponentModel.Container components = null;
        public FloatTextBox()
        {
            InitializeComponent();
        }
        
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            int index = this.Text.IndexOf('.');
            if ((!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && (e.KeyChar != '.')) || (index > -1 && (e.KeyChar == '.' || (index + 4 == this.Text.Length && this.SelectionStart > index )) && e.KeyChar != (char)8))
            {
                e.Handled = true; // input is not passed on to the control(TextBox) 
            }
            base.OnKeyPress(e);
        }
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
    }
}
