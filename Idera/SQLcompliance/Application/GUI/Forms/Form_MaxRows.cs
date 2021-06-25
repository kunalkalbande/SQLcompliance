using System;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Properties;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   public partial class Form_MaxRows : Form
   {
      public Form_MaxRows(int rows)
      {
         InitializeComponent();
         Icon = Resources.SQLcompliance_product_ico;

         if (rows == -1)
            _comboMaxRows.SelectedItem = "All";
         else
            _comboMaxRows.SelectedItem = rows.ToString();
      }

      public int MaximumRows
      {
         get 
         {
            return GetMaxRows(_comboMaxRows.SelectedItem.ToString());
         }
      }

      public static int GetMaxRows(string s)
      {
         if (String.IsNullOrEmpty(s))
            return 10;

         if (s.Equals("All"))
            return -1;
         else
         {
            try
            {
               return Int32.Parse(s);
            }
            catch (Exception)
            {
               // Default
               return 10;
            }
         }
      }

      public static string GetMaxRowString(int i)
      {
         if (i < 0)
            return "All";
         else
            return i.ToString();
      }
   }
}