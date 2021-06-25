using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Idera.SQLcompliance.Core.Reports;

using Infragistics.Win;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
   public partial class ParameterComboBox : UserControl
   {
      #region Properties
      
      public string ParameterName
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
      
      public ReportParameterValue [] Values
      {
         get
         {
            List<ReportParameterValue> list = new List<ReportParameterValue>( _comboEditor.Items.Count );
            for( int i = 0; i < _comboEditor.Items.Count; i++ )
               list.Add( (ReportParameterValue)_comboEditor.Items[i].DataValue);
            return list.ToArray();
         }
         set
         {
            _comboEditor.Items.Clear();
            if ( value == null )
               return;

            for ( int i = 0; i < value.Length; i++ )
               _comboEditor.Items.Add( value[i], value[i].Label );
         }
      }
      
      public ReportParameterValue SelectedValue
      {
         get
         {
            if ( _comboEditor.SelectedIndex == -1 )
               return null;
            return (ReportParameterValue) _comboEditor.SelectedItem.DataValue;
         }
         
         set
         {
            if( _comboEditor.Items.Count == 0 )
            {
               _comboEditor.SelectedIndex = -1;
               return;
            }
            _comboEditor.SelectedIndex = _comboEditor.FindStringExact(value.Label);
         }
      }
      
      public int SelectedIndex
      {
         get
         {
            return _comboEditor.SelectedIndex;
         }
         set
         {
            if( value < 0 || value > _comboEditor.Items.Count )
               _comboEditor.SelectedIndex = -1;
            else
               _comboEditor.SelectedIndex = value;
         }
      }

      #endregion
      
      public ParameterComboBox()
      {
         InitializeComponent();
      }
      
      #region Public methods
      
      public void SetDefaultValue( DefaultValue value )
      {
         for( int i = 0; i < _comboEditor.Items.Count; i++ )
         {
            if (
               ( (ReportParameterValue) _comboEditor.Items[i].DataValue ).IsDefault( value ) )
            {
               _comboEditor.SelectedIndex = i;
               return;
            }
         }
      }

      #endregion
   }
}
