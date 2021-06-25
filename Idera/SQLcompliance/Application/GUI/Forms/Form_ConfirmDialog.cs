using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_ConfirmDialog : Form
    {
        
        public Form_ConfirmDialog()
        {
            InitializeComponent();
        }

        public void setText(string text)
        {
            this.label1.Text = text;
        }

        public void setFormWidth(int width) 
        {
            this.Width = width;
        }

        public void setFormHeight(int height)
        {
            this.Height = height;
        }

    }
}
