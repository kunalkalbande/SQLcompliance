using Idera.SQLcompliance.Application.GUI.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_DisplayServerDefaultAuditSettings : Form
    {
        public Form_DisplayServerDefaultAuditSettings(string selection, string defaultSettings)
        {
            InitializeComponent();
            this.Icon = Resources.SQLcompliance_product_ico;
            label1.Text += selection;
            StringBuilder builderCodes = new StringBuilder(@"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 Microsoft Sans Serif;}}\viewkind4\uc1\pard\f0\fs17 ");

            foreach (var settings in defaultSettings.Split('!'))
            {
                var strings = settings.Split(':');
                if (strings != null && strings.Count() == 2)
                    builderCodes.AppendFormat(@"\b {0} : \b0 {1}. \line\line", Regex.Escape(strings[0]), Regex.Escape(strings[1]));
            }

            builderCodes.Append(@"\par}");
            richTextBox1.Rtf = builderCodes.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
