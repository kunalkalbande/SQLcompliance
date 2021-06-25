using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Properties;
using Idera.SQLcompliance.Core.Rules.Alerts;
using Idera.SQLcompliance.Core.Rules;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_RowCounts : Form
    {
        public Form_RowCounts(int fieldId, bool forAlerts)
        {
            InitializeComponent();
            this.Icon = Resources.SQLcompliance_product_ico;
            this.StartPosition = FormStartPosition.CenterParent; 
        }

        private void _tbRowcount_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form_RowCounts_Load(object sender, EventArgs e)
        {

        }

        private void _gbRowcount_Enter(object sender, EventArgs e)
        {

        }

        private void _lblRowcount_Click_1(object sender, EventArgs e)
        {

        }

        private void _cbRowcount_SelectedIndexChanged_1(object sender, EventArgs e)
        {
          
        }

        private void _gbTimeframe_Enter(object sender, EventArgs e)
        {

        }

        private void _numtbRowcount_TextChanged_1(object sender, EventArgs e)
        {
   
        }

        private void _btnOk_Click(object sender, EventArgs e)
        {

        }

        public string CbOprtr
        {
            get
            { return _cbRowcount.Text.ToString(); }
            set
            {
                int index = (value == "<"?0:value == "<="?1:value == "="?2:value == ">="?4:3);
                _cbRowcount.SelectedIndex = index;
            }
        }
        public string IntegerRowcount
        {
            get { return _numtbRowcount.Text; }
            set { (_numtbRowcount.Text) = value; }
        }
        public string IntegerTimeFrame
        {
            get { return _numtbTimeframe.Text; }
            set { (_numtbTimeframe.Text) = value; }
        }

        private void _numtbTimeframe_TextChanged(object sender, EventArgs e)
        {

        }
        
    }
}
