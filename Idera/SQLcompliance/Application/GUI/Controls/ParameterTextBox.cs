using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI
{
   public partial class ParameterTextBox : UserControl
   {
      #region Data members
      
      bool _isNumeric = false;
      
      #endregion
      
      #region Properties
      
      public string Parameter
      {
         get
         {
            return _lbl.Text;
         }
         set
         {
            _lbl.Text = value;
         }
      }
      
      public string Value
      {
         get
         {
            return _txtBox.Text;
         }
         set
         {
            _txtBox.Text = value;
         }
      }
      
      public bool IsNumber
      {
         get
         {
            return _isNumeric;
         }
         
         set
         {
            _isNumeric = value;
         }
      }
      
      #endregion
      public ParameterTextBox()
      {
         InitializeComponent();
      }
   }
}
