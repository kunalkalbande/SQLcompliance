using System;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Appearance=Infragistics.Win.Appearance;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
   public static class GridHelper
   {
      private static Appearance _displayLayout ;
      private static Appearance _titleAppearance ;

      static GridHelper()
      {
         _displayLayout = new Appearance() ;
         _displayLayout.BackColor = SystemColors.Window ;
         _displayLayout.ForeColor = SystemColors.WindowText ;
         _titleAppearance = new Appearance() ;
         _titleAppearance.FontData.Bold = DefaultableBoolean.True ;
      }

      public static void ApplyDefaultSettings(UltraGrid grid)
      {
         grid.DisplayLayout.Appearance = _displayLayout;

//         // Office 2007 ISL extractions
//         grid.UseOsThemes = DefaultableBoolean.False;
//         grid.UseFlatMode = DefaultableBoolean.True;
//         grid.DisplayLayout.Appearance.BorderColor = Color.FromArgb(191, 197, 210);
//
//         // column headers
//         grid.DisplayLayout.Override.HeaderAppearance.BackColor = Color.FromArgb(246, 250, 251);
//         grid.DisplayLayout.Override.HeaderAppearance.ForeColor = Color.FromArgb(64, 94, 131);
//         grid.DisplayLayout.Override.HeaderAppearance.BorderColor = Color.FromArgb(191, 197, 210);
//         grid.DisplayLayout.Override.HeaderAppearance.ImageBackgroundStyle = ImageBackgroundStyle.Stretched;
//         grid.DisplayLayout.Override.HeaderAppearance.BackColor2 = Color.FromArgb(211, 219, 233);
//         grid.DisplayLayout.Override.HeaderAppearance.BackGradientStyle = GradientStyle.Vertical;
//         grid.DisplayLayout.Override.HeaderAppearance.ImageBackgroundStretchMargins = new ImageBackgroundStretchMargins(2, 2, 2, 2);
//
//         grid.DisplayLayout.Override.HotTrackHeaderAppearance.ImageBackground = Properties.Resources.GridColumnHeader_HotTracked_ImageBackground ;
//         grid.DisplayLayout.Override.HotTrackHeaderAppearance.BackColor = Color.Transparent;
//         grid.DisplayLayout.Override.HotTrackHeaderAppearance.BorderColor = Color.FromArgb(242, 149, 54);
//         grid.DisplayLayout.Override.HotTrackHeaderAppearance.ImageBackgroundStyle = ImageBackgroundStyle.Stretched;
//         grid.DisplayLayout.Override.HotTrackHeaderAppearance.BackGradientStyle = GradientStyle.None;
//         grid.DisplayLayout.Override.HotTrackHeaderAppearance.BackHatchStyle = BackHatchStyle.None;
//         grid.DisplayLayout.Override.HotTrackHeaderAppearance.ImageBackgroundStretchMargins = new ImageBackgroundStretchMargins(2, 2, 2, 2);
//
//         // Group-by Box
//         grid.DisplayLayout.GroupByBox.Appearance.BackColor = Color.White;
//         grid.DisplayLayout.GroupByBox.Appearance.BackGradientStyle = GradientStyle.None;
//         grid.DisplayLayout.GroupByBox.Appearance.BackHatchStyle = BackHatchStyle.None;
//
//         grid.DisplayLayout.GroupByBox.PromptAppearance.BackColor = Color.Transparent;
//         grid.DisplayLayout.GroupByBox.PromptAppearance.BackGradientStyle = GradientStyle.None ;
//         grid.DisplayLayout.GroupByBox.PromptAppearance.BackHatchStyle = BackHatchStyle.None;
//
//         // rows
//         grid.DisplayLayout.Override.RowAppearance.BackColor = Color.White;
//         grid.DisplayLayout.Override.RowAppearance.BackGradientStyle = GradientStyle.None;
//         grid.DisplayLayout.Override.RowAppearance.BackHatchStyle = BackHatchStyle.None;
//
//         grid.DisplayLayout.Override.ActiveRowAppearance.ImageBackgroundStyle = ImageBackgroundStyle.Stretched;
//         grid.DisplayLayout.Override.ActiveRowAppearance.ImageBackgroundStretchMargins = new ImageBackgroundStretchMargins(2, 2, 2, 2);
//         grid.DisplayLayout.Override.ActiveRowAppearance.ImageBackground = Properties.Resources.GridRow_Active_ImageBackground;
//
//         grid.DisplayLayout.Override.SelectedRowAppearance.ImageBackgroundStyle = ImageBackgroundStyle.Stretched;
//         grid.DisplayLayout.Override.SelectedRowAppearance.ForeColor = Color.FromArgb(0, 69, 209);
//         grid.DisplayLayout.Override.SelectedRowAppearance.ImageBackground = Properties.Resources.GridRow_Selected_ImageBackground;
//
//         // Scrollbars
//         grid.DisplayLayout.ScrollBarLook.TrackAppearanceHorizontal.ImageBackgroundStyle = ImageBackgroundStyle.Stretched;
//         grid.DisplayLayout.ScrollBarLook.TrackAppearanceHorizontal.ImageBackground = Properties.Resources.ScrollBarTrackHorizontal_Normal_ImageBackground;
//         grid.DisplayLayout.ScrollBarLook.TrackAppearanceVertical.ImageBackgroundStyle = ImageBackgroundStyle.Stretched;
//         grid.DisplayLayout.ScrollBarLook.TrackAppearanceVertical.ImageBackground = Properties.Resources.ScrollBarTrackVertical_Normal_ImageBackground;
//
//         grid.DisplayLayout.ScrollBarLook.ThumbAppearanceHorizontal.Image = Properties.Resources.ScrollBarThumbHorizontal_Normal_Image;
//         grid.DisplayLayout.ScrollBarLook.ThumbAppearanceHorizontal.ImageBackground = Properties.Resources.ScrollBarThumbHorizontal_Normal_ImageBackground;
//         grid.DisplayLayout.ScrollBarLook.ThumbAppearanceHorizontal.ImageBackgroundStyle = ImageBackgroundStyle.Stretched;
//         grid.DisplayLayout.ScrollBarLook.ThumbAppearanceHorizontal.ImageBackgroundStretchMargins = new ImageBackgroundStretchMargins(2, 2, 2, 2);
//
//         grid.DisplayLayout.ScrollBarLook.ThumbAppearanceVertical.Image = Properties.Resources.ScrollBarThumbVertical_Normal_Image;
//         grid.DisplayLayout.ScrollBarLook.ThumbAppearanceVertical.ImageBackground = Properties.Resources.ScrollBarThumbVertical_Normal_ImageBackground;
//         grid.DisplayLayout.ScrollBarLook.ThumbAppearanceVertical.ImageBackgroundStyle = ImageBackgroundStyle.Stretched;
//         grid.DisplayLayout.ScrollBarLook.ThumbAppearanceVertical.ImageBackgroundStretchMargins = new ImageBackgroundStretchMargins(2, 2, 2, 2);

         // Other mods
         grid.DisplayLayout.CaptionVisible = DefaultableBoolean.False;
         grid.DisplayLayout.MaxColScrollRegions = 1;
         grid.DisplayLayout.MaxRowScrollRegions = 1;
         grid.DisplayLayout.Override.AllowAddNew = AllowAddNew.No;
         grid.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
         grid.DisplayLayout.Override.AllowGroupBy = DefaultableBoolean.True;
         grid.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
         grid.DisplayLayout.Override.BorderStyleCell = UIElementBorderStyle.None;
         grid.DisplayLayout.Override.BorderStyleRow = UIElementBorderStyle.None;
         grid.DisplayLayout.Override.CellClickAction = CellClickAction.RowSelect;
         grid.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.SortMulti;
         grid.DisplayLayout.Override.RowSelectors = DefaultableBoolean.False;
         grid.DisplayLayout.Override.SelectTypeCell = SelectType.None;
         grid.DisplayLayout.Override.SelectTypeCol = SelectType.None;
         grid.DisplayLayout.Override.SelectTypeGroupByRow = SelectType.None;
         grid.DisplayLayout.Override.SelectTypeRow = SelectType.Single;
         grid.DisplayLayout.ScrollBounds = ScrollBounds.ScrollToFill;
         grid.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;
         grid.DisplayLayout.TabNavigation = TabNavigation.NextControl ;
         grid.DisplayLayout.ViewStyle = ViewStyle.SingleBand;
         grid.DisplayLayout.ViewStyleBand = ViewStyleBand.Horizontal;

         foreach (UltraGridBand b in grid.DisplayLayout.Bands)
         {
            foreach (UltraGridColumn c in b.Columns)
            {
               if(c.DataType == typeof(Bitmap))
                  c.AllowGroupBy = DefaultableBoolean.False ;

               if (c.Key == "Date")
               {
                  FormatDateColumn(c);
               }
               else if (c.Key == "Time")
               {
                  FormatTimeColumn(c);
               }
            }
         }
      }

      public static void ApplyEnterpriseSummarySettings(UltraGrid grid)
      {
         ApplyDefaultSettings(grid) ;
         grid.DisplayLayout.BorderStyle = UIElementBorderStyle.None ;
         grid.DisplayLayout.CaptionAppearance = _titleAppearance ;
         grid.DisplayLayout.CaptionVisible = DefaultableBoolean.True ;
         grid.DisplayLayout.Bands[0].Columns[0].LockedWidth = true ;
      }

      public static void ApplyRecentActivitySummarySettings(UltraGrid grid)
      {
         ApplyDefaultSettings(grid) ;
         grid.DisplayLayout.BorderStyle = UIElementBorderStyle.None ;
         grid.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns ;
      }

      public static void ApplyAdminSettings(UltraGrid grid)
      {
         ApplyDefaultSettings(grid) ;
         grid.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns ;
      }

      public static void FormatDateColumn(UltraGridColumn c)
      {
         Appearance a = new Appearance() ;
         a.TextHAlign = HAlign.Right ;
         c.CellAppearance = a ;
         c.Format = "d" ;
         c.GroupByEvaluator = new DateGroupBy() ;
         //c.GroupByMode = GroupByMode.Date ;
      }

      public static void FormatTimeColumn(UltraGridColumn c)
      {
         Appearance a = new Appearance();
         a.TextHAlign = HAlign.Right;
         c.CellAppearance = a;

         //To override this, just set it Format property on the control
         if (String.IsNullOrEmpty(c.Format))
         {
            c.Format = "T";
         }
         else
         {
            if (c.Format.Trim().Length == 0)
               c.Format = "T";
         }
//         c.GroupByMode = GroupByMode.Hour;
         c.GroupByEvaluator = new TimeGroupBy() ;
         c.HiddenWhenGroupBy = DefaultableBoolean.False ;
      }

      public static void AutoResizeColumns(UltraGrid grid)
      {
         foreach (UltraGridBand b in grid.DisplayLayout.Bands)
         {
            foreach (UltraGridColumn c in b.Columns)
            {
               c.PerformAutoResize(PerformAutoSizeType.AllRowsInBand) ;
            }
         }
      }

      public static void AutoResizeRelative(UltraGrid grid)
      {
         int totalWidth  = 0 ;
         int sizableWidth = 0 ;
         int gridWidth = grid.Width - SystemInformation.VerticalScrollBarWidth ;

         foreach (UltraGridBand b in grid.DisplayLayout.Bands)
         {
            foreach (UltraGridColumn c in b.Columns)
            {
               if(c.Hidden)
                  continue ;
               if(c.Key == "Date" || c.Key == "Time")
               {
                  c.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
               }else if(!c.LockedWidth)
                  sizableWidth += c.Width ;
               totalWidth += c.Width ;
            }
         }
         if(sizableWidth == 0 || totalWidth > gridWidth)
            return ;

         double modifier = (double)(sizableWidth + gridWidth - totalWidth)/(double)sizableWidth ;
         totalWidth = 0 ;
         UltraGridColumn lastColumn = null ;
         foreach (UltraGridBand b in grid.DisplayLayout.Bands)
         {
            foreach (UltraGridColumn c in b.Columns)
            {
               if (c.Hidden)
                  continue;
               if (c.Key == "Date" || c.Key == "Time" || c.LockedWidth)
               {
                  totalWidth += c.Width ;
                  continue;
               }
               c.Width = Convert.ToInt32(c.Width * modifier) ;
               totalWidth += c.Width ;
               lastColumn = c ;
            }
         }
         if(lastColumn != null)
            lastColumn.Width -= (totalWidth - gridWidth) ;
      }

      public static void SelectNextLeafRow(UltraGrid grid, int listIndex)
      {
         SelectNextLeafRow(grid, grid.Rows.GetRowWithListIndex(listIndex));
      }

      public static void SelectNextLeafRow(UltraGrid grid)
      {
         SelectNextLeafRow(grid, grid.Selected.Rows[0]);
      }

      private static void SelectNextLeafRow(UltraGrid grid, UltraGridRow selectedRow)
      {
         bool nextRow = false;
         UltraGridRow previous = null;

         foreach (UltraGridRow row in grid.Rows.GetAllNonGroupByRows())
         {
            if (nextRow)
            {
               previous.Selected = false;
               row.Selected = true;
               row.Activate();
               return;
            }
            if (row == selectedRow)
            {
               previous = row;
               nextRow = true;
            }
         }
      }

      public static void SelectPreviousLeafRow(UltraGrid grid, int listIndex)
      {
         SelectPreviousLeafRow(grid, grid.Rows.GetRowWithListIndex(listIndex));
      }

      public static void SelectPreviousLeafRow(UltraGrid grid)
      {
         SelectPreviousLeafRow(grid, grid.Selected.Rows[0]);
      }

      private static void SelectPreviousLeafRow(UltraGrid grid, UltraGridRow selectedRow)
      {
         UltraGridRow previous = null ;

         foreach (UltraGridRow row in grid.Rows.GetAllNonGroupByRows())
         {
            if (row == selectedRow)
            {
               if (previous != null)
               {
                  row.Selected = false;
                  previous.Selected = true;
                  previous.Activate();
               }
               return ;
            }
            previous = row ;
         }
      }
   }

   public class DateGroupBy : IGroupByEvaluatorEx
   {
      public int Compare(UltraGridCell cell1, UltraGridCell cell2)
      {
         DateTime dt1, dt2 ;
         dt1 = ((DateTime)cell1.Value).Date ;
         dt2 = ((DateTime)cell2.Value).Date;
         return DateTime.Compare(dt1, dt2) ;
      }

      public object GetGroupByValue(UltraGridGroupByRow groupByRow, UltraGridRow row)
      {
         if(row.Cells["Date"].Value is DateTime)
         {
            DateTime date = (DateTime)row.Cells["Date"].Value;
            return date.ToShortDateString();
         }
         else
         {
            return null;
         }
      }

      public bool DoesGroupContainRow(UltraGridGroupByRow groupByRow, UltraGridRow row)
      {
         DateTime date = (DateTime)row.Cells["Date"].Value;
         string cellValue =  date.ToShortDateString();

         // Do a case insensitive compare. 
         return 0 == string.Compare(groupByRow.Value.ToString(), cellValue, true);
      }
   }

   public class TimeGroupBy : IGroupByEvaluatorEx
   {
      private static DateTime ConvertDateTime(DateTime t)
      {
         DateTime retVal ;
         retVal = t.AddMinutes(-t.Minute);
         retVal = retVal.AddSeconds(-t.Second);
         return retVal ;
      }

      public int Compare(UltraGridCell cell1, UltraGridCell cell2)
      {
         DateTime dt1, dt2;
         dt1 = ConvertDateTime((DateTime)cell1.Value);
         dt2 = ConvertDateTime((DateTime)cell2.Value);
         return DateTime.Compare(dt1, dt2);
      }

      public object GetGroupByValue(UltraGridGroupByRow groupByRow, UltraGridRow row)
      {
         if(row.Cells["Time"].Value is DateTime)
         {
            DateTime date = ConvertDateTime((DateTime)row.Cells["Time"].Value);
            return date.ToShortDateString() + " " + date.ToShortTimeString();
         }
         else
         {
            return null;
         }
      }

      public bool DoesGroupContainRow(UltraGridGroupByRow groupByRow, UltraGridRow row)
      {
         DateTime date = ConvertDateTime((DateTime)row.Cells["Time"].Value);
         string cellValue = date.ToShortDateString() + " " + date.ToShortTimeString();

         // Do a case insensitive compare. 
         return 0 == string.Compare(groupByRow.Value.ToString(), cellValue, true);
      }
   }


}
