using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
   [ToolboxItem(true)]
   public partial class RegulationEntry : Idera.SQLcompliance.Application.GUI.Controls.BaseControl
   {
      public RegulationEntry()
      {
         InitializeComponent();
      }

      [Category("Appearance")]
      public string Section
      {
         get { return labelSection.Text; }
         set { labelSection.Text = value; }
      }

      [Category("Appearance")]
      public string ServerEvents
      {
         get { return labelServerEvents.Text; }
         set { labelServerEvents.Text = value.Replace("\\\\", "\\"); }
      }

      [Category("Appearance")]
      public string DatabaseEvents
      {
         get { return labelDatabaseEvents.Text; }
         set { labelDatabaseEvents.Text = value; }
      }

      [Category("Appearance")]
      public Color FillColor
      {
         get { return panelServerEvents.FillColor; }
         set
         {
            panelServerEvents.FillColor = value;
            panelDatabaseEvents.FillColor = value;
         }
      }

      [Category("Appearance")]
      public Color FillColor2
      {
         get { return panelServerEvents.FillColor2; }
         set
         {
            panelServerEvents.FillColor2 = value;
            panelDatabaseEvents.FillColor2 = value;
         }
      }
   }
}

