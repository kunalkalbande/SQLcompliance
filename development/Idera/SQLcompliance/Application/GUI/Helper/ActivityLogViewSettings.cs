using System.Collections.Generic;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
   public class ActivityLogViewSettings
   {
      private string _name;
      private ActivityLogViewFilter _filter;
      private List<GridColumnSettings> _columns;
      private bool _showGroupBy;


      public ActivityLogViewFilter Filter
      {
         get { return _filter; }
         set { _filter = value; }
      }

      public List<GridColumnSettings> Columns
      {
         get { return _columns; }
         set { _columns = value; }
      }

      public string Name
      {
         get { return _name; }
         set { _name = value; }
      }

      public bool ShowGroupBy
      {
         get { return _showGroupBy; }
         set { _showGroupBy = value; }
      }

      public ActivityLogViewSettings Clone()
      {
         ActivityLogViewSettings retVal = (ActivityLogViewSettings)MemberwiseClone();
         retVal._filter = _filter.Clone();
         retVal._columns = new List<GridColumnSettings>();
         foreach (GridColumnSettings settings in _columns)
            retVal._columns.Add(settings.Clone());
         return retVal;
      }

      public override string ToString()
      {
         return _name;
      }
   }
}
