using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   public partial class Form_AddStatus : Form
   {
      public string Message
      {
         get { return labelStatus.Text; }
         set { labelStatus.Text = value; }
      }

      public Form_AddStatus()
      {
         InitializeComponent();
      }
   }
}