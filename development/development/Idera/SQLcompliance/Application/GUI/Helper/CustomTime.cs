using System;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
   public class CustomTime
   {
      private const int _lruCount = 3 ;  // number of custom items to save in the combo
      private const int _fixedCount = 5 ;  // number of fixed items in a combo
      private DateTime?[] _startDateLru;
      private DateTime?[] _endDateLru;

      public CustomTime()
      {
         _startDateLru = new DateTime?[_lruCount];
         _endDateLru = new DateTime?[_lruCount];
      }

      public DateTime?[] StartTime
      {
         get { return _startDateLru; }
      }

      public DateTime?[] EndTime
      {
         get { return _endDateLru; }
      }

      public void AddCustomRange(ComboBoxTool combo, DateTime start, DateTime end)
      {
         for (int i = _lruCount - 1; i > 0; i--)
         {
            _startDateLru[i] = _startDateLru[i - 1];
            _endDateLru[i] = _endDateLru[i - 1];
         }
         _startDateLru[0] = start;
         _endDateLru[0] = end;

         int itemsToRemove = combo.ValueList.ValueListItems.Count - _fixedCount;
         for (int i = 0; i < _lruCount; i++)
         {
            if (_startDateLru[i] != null)
            {
               string dateStr = String.Format("{0} {2} - {1} {3}", _startDateLru[i].Value.ToShortDateString(), _endDateLru[i].Value.ToShortDateString(),
                  _startDateLru[i].Value.ToShortTimeString(), _endDateLru[i].Value.ToShortTimeString());
               combo.ValueList.ValueListItems.Add(dateStr, dateStr);
            }
         }
         while (itemsToRemove > 0)
         {
            combo.ValueList.ValueListItems.RemoveAt(_fixedCount);
            itemsToRemove--;
         }
      }

      public bool GetCustomTime(ComboBoxTool combo, int lastIndex)
      {
         Form_TimeSpan frm = new Form_TimeSpan();
         if(frm.ShowDialog() == DialogResult.OK)
         {
            AddCustomRange(combo, frm.StartDate, frm.EndDate);
            combo.SelectedIndex = _fixedCount;
            return true ;
         }
         else
         {
            combo.SelectedIndex = lastIndex;
            return false ;
         }
      }
   }
}
