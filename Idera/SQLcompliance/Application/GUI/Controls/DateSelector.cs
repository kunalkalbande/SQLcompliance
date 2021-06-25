using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinEditors;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
   public partial class DateSelector : UserControl
   {
      #region Properties
      
      public string ParameterName
      {
         get
         {
            return _label.Text;
         }
         set
         {
            _label.Text = value;
         }
      }
      
      public UltraDateTimeEditor Editor
      {
         get
         {
            return _dateEditor;
         }
      }
      
      public string SelectedDateString
      {
         get
         {
            return _dateEditor.Text;
         }
         set
         {
            _dateEditor.Text = value;
         }
      }            

      #endregion
      
      public DateSelector()
      {
         InitializeComponent();
      }
      
      
      
   }
}
