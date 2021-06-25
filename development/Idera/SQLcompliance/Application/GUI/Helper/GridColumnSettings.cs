using System.Collections.Generic;
using Infragistics.Win.UltraWinGrid ;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
   public class GridColumnSettings
   {
      private string _key;
      private int _visiblePosition;
      private bool _groupBy;
      private int _sortPosition;
      private bool _descending;
      private int _width;
      private int _band;

      public GridColumnSettings()
      {
         _key = "";
         _visiblePosition = -1;
         _groupBy = false;
         _sortPosition = -1;
         _descending = false;
         _width = -1;
         _band = 0;
      }

      public string Key
      {
         get { return _key ; }
         set { _key = value ; }
      }

      public int VisiblePosition
      {
         get { return _visiblePosition ; }
         set { _visiblePosition = value ; }
      }

      public bool GroupBy
      {
         get { return _groupBy ; }
         set { _groupBy = value ; }
      }

      public bool Descending
      {
         get { return _descending ; }
         set { _descending = value ; }
      }

      public int SortPosition
      {
         get { return _sortPosition ; }
         set { _sortPosition = value ; }
      }

      public int ColumnWidth
      {
         get { return _width ; }
         set { _width = value ; }
      }


      public int Band
      {
         get { return _band; }
         set { _band = value; }
      }

      public GridColumnSettings Clone()
      {
         GridColumnSettings retVal = (GridColumnSettings)MemberwiseClone();
         return retVal;
      }

      public static List<GridColumnSettings> ExtractColumnSettings(UltraGrid grid)
      {
         Dictionary<string, GridColumnSettings> table = new Dictionary<string, GridColumnSettings>();

         for (int bandNumber = 0; bandNumber < grid.DisplayLayout.Bands.Count; bandNumber++)
         {
            foreach (UltraGridColumn column in grid.DisplayLayout.Bands[bandNumber].Columns)
            {
               GridColumnSettings item = new GridColumnSettings();
               item.Key = column.Key;
               if (column.Hidden && !column.IsGroupByColumn)
                  item.VisiblePosition = -1;
               else
                  item.VisiblePosition = column.Header.VisiblePosition;
               item.SortPosition = -1;
               item.ColumnWidth = column.Width;
               item.GroupBy = column.IsGroupByColumn;
               item.Band = bandNumber;
               table.Add(item.Key + bandNumber, item);
            }
            for (int i = 0; i < grid.DisplayLayout.Bands[bandNumber].SortedColumns.Count; i++)
            {
               UltraGridColumn column = grid.DisplayLayout.Bands[bandNumber].SortedColumns[i];
               GridColumnSettings settings = table[column.Key + bandNumber];
               settings.SortPosition = i;
               settings.Descending = column.SortIndicator == SortIndicator.Descending;
            }
         }

         return new List<GridColumnSettings>(table.Values) ;
      }

      public static void ApplyColumnSettings(UltraGrid grid, List<GridColumnSettings> settings)
      {
         int[] sortCount = new int[grid.DisplayLayout.Bands.Count] ;
         GridColumnSettings[,] sorted = new GridColumnSettings[grid.DisplayLayout.Bands.Count,settings.Count];

         for (int i = 0; i < grid.DisplayLayout.Bands.Count; i++)
            sortCount[i] = 0;

         for(int i = 0 ; i < settings.Count; i++)
         {
            GridColumnSettings current = settings[i] ;
            // Preventive check to populate columns that are present
            if (!grid.DisplayLayout.Bands[current.Band].Columns.Contains(current.Key))
            { 
                continue;
            }
            UltraGridColumn column = grid.DisplayLayout.Bands[current.Band].Columns[current.Key];
            column.Hidden = (current.VisiblePosition == -1);
            if (!column.Hidden)
               column.Header.VisiblePosition = current.VisiblePosition;
            if (current.SortPosition != -1)
            {
               sortCount[current.Band]++;
               sorted[current.Band, current.SortPosition] = current;
            }
            if (current.ColumnWidth != -1)
               column.Width = current.ColumnWidth;
         }
         for (int bandNumber = 0; bandNumber < grid.DisplayLayout.Bands.Count; bandNumber++)
         {
            grid.DisplayLayout.Bands[bandNumber].SortedColumns.Clear();
            for (int columnNumber = 0; columnNumber < sortCount[bandNumber]; columnNumber++)
            {
               GridColumnSettings setting = sorted[bandNumber,columnNumber];
               UltraGridColumn column = grid.DisplayLayout.Bands[bandNumber].Columns[setting.Key];
               grid.DisplayLayout.Bands[bandNumber].SortedColumns.Add(column, setting.Descending, setting.GroupBy);
            }
         }
      }
   }
}
