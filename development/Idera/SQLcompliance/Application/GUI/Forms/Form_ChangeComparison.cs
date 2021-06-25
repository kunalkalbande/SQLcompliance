using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLcompliance.Core.Rules.Alerts;
using Infragistics.Win;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   public partial class Form_ChangeComparison : Form
   {
      public DataAlertRule _rule;

      public ComparisonOperator Operator
      {
         get
         {
            ValueListItem selectedItem = (ValueListItem)operatorCombo.SelectedItem;

            return (ComparisonOperator)selectedItem.DataValue;
         }
      }

      public long Value
      {
         get { return (long)comparisonValue.Value; }
      }

      public Form_ChangeComparison(DataAlertRule rule)
      {
         InitializeComponent();

         _rule = rule;
         SetInstanceInfo();
         InitializeCombo();
         comparisonValue.Value = _rule.Comparison.Value;
      }

      private void SetInstanceInfo()
      {
         if (String.IsNullOrEmpty(_rule.Instance))
         {
            instanceText.Text = "Any SQL Server";
         }
         else
         {
            if (String.Equals("<ALL>", _rule.Instance))
               instanceText.Text = "Any SQL Server";
            else
               instanceText.Text = _rule.Instance;
         }

         if (String.IsNullOrEmpty(_rule.Database))
         {
            databaseText.Text = "Any Database";
         }
         else
         {
            if (String.Equals("<ALL>", _rule.Database))
               databaseText.Text = "Any Database";
            else
               databaseText.Text = _rule.Database;
         }

         if (String.IsNullOrEmpty(_rule.FullTableName))
         {
            tableText.Text = "Any Table";
         }
         else
         {
            if (String.Equals("<ALL>", _rule.FullTableName))
               tableText.Text = "Any Table";
            else
               tableText.Text = _rule.FullTableName;
         }

         if (String.IsNullOrEmpty(_rule.Column))
         {
            columnText.Text = "Any Column";
         }
         else
         {
            if (String.Equals("<ALL>", _rule.Column))
               columnText.Text = "Any Column";
            else
               columnText.Text = _rule.Column;
         }
      }

      private void InitializeCombo()
      {
         ValueListItem item = null;

         foreach (int val in Enum.GetValues(typeof(ComparisonOperator)))
         {
            item = new ValueListItem((ComparisonOperator)val, AlertUIHelper.GetEnumDescription((ComparisonOperator)val));
            operatorCombo.Items.Add(item);

            if (_rule.Comparison.Operator == (ComparisonOperator)val)
               operatorCombo.SelectedItem = item;
         }
      }

      private void operatorCombo_SelectedIndexChanged(object sender, EventArgs e)
      {
         ValueListItem selectedItem = (ValueListItem)operatorCombo.SelectedItem;

         if (selectedItem.DataValue == null)
            buttonOk.Enabled = false;
         else
            buttonOk.Enabled = true;
      }

      private void buttonOk_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.OK;
         Close();
      }
   }
}