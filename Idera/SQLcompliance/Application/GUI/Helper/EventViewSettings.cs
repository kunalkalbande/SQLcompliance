using System.Collections.Generic;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
   public class EventViewSettings
   {
      private string _name;
      private EventViewFilter _filter;
      private List<GridColumnSettings> _columns;
      private bool _showGroupBy;
      private GridViewMode _viewMode;

      public string Name
      {
         get { return _name ; }
         set { _name = value ; }
      }

      public EventViewFilter Filter
      {
         get { return _filter ; }
         set { _filter = value ; }
      }

      public List<GridColumnSettings> Columns
      {
         get { return _columns ; }
         set { _columns = value ; }
      }

      public bool ShowGroupBy
      {
         get { return _showGroupBy ; }
         set { _showGroupBy = value ; }
      }


      public GridViewMode ViewMode
      {
         get { return _viewMode; }
         set { _viewMode = value; }
      }

      public EventViewSettings Clone()
      {
         EventViewSettings retVal = (EventViewSettings)MemberwiseClone();
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

   public enum GridViewMode
   {
      Hierarchical,
      Flat
   };
}
