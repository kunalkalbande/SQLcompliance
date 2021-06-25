using System;
using System.Drawing;
using ChartFX.WinForms;
using ChartFX.WinForms.Adornments;
using ChartFX.WinForms.DataProviders;
using Idera.SQLcompliance.Core.Stats;
using Idera.SQLcompliance.Core.TimeZoneHelper;
using System.Collections.Generic;
using TimeZoneInfo = Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
   public class ChartHelper
   {
      private static CompactFormulaHandler _dayCompactor = new CompactFormulaHandler(GenericSumFormula);
      private static CompactFormulaHandler _weekCompactor = new CompactFormulaHandler(GenericSumFormula);
      private static CompactFormulaHandler _monthCompactor = new CompactFormulaHandler(GenericSumFormula);


      public static CompactFormulaHandler DayCompactor
      {
         get { return _dayCompactor; }
      }

      public static CompactFormulaHandler WeekCompactor
      {
         get { return _weekCompactor; }
      }

      public static CompactFormulaHandler MonthCompactor
      {
         get { return _monthCompactor; }
      }

      public static void InitializeChart(Chart chart)
      {
         SolidBackground solidBackground1 = new SolidBackground();
         solidBackground1.AssemblyName = "ChartFX.WinForms.Adornments";
         //CompactFormulaHandler handler = new CompactFormulaHandler(SumFormula);
         //chart.Data.Y.CompactFormula = handler;

         chart.Background = solidBackground1;
         chart.Border = new SimpleBorder(SimpleBorderType.None, Color.White);
         chart.AllSeries.MarkerShape = MarkerShape.None;
         chart.AxisX.ForceZero = false;
         chart.Highlight.Dimmed = false;
         chart.Highlight.Enabled = false;
         chart.LegendBox.Visible = false;
         chart.PlotAreaMargin.Right = 15;
         chart.PlotAreaMargin.Top = 5;
         chart.PlotAreaColor = Color.FromArgb(240, 240, 240);
         chart.ContextMenus = false ;
         chart.AllSeries.MarkerShape = MarkerShape.Marble ;
         chart.AllSeries.MarkerSize = 1 ;
         chart.Data.Clear() ;
      }

      public static void ShowMonthChart(StatsDataSeries stats, Chart chart)
      {
         DateTime utcEndDate = DateTime.UtcNow;
         DateTime utcStartDate = utcEndDate.AddDays(-30);
         // ChartFx compacts on date/time boundaires from 12:00.  Because of this,
         //  we ensure that our start time is on an hour marker for the daily chart
         DateTime startDate = TimeZoneInfo.CurrentTimeZone.ToLocalTime(utcStartDate);
         startDate = startDate.Date; // we only want the day
         utcStartDate = TimeZoneInfo.CurrentTimeZone.ToUniversalTime(startDate);

         StatsDataSeries compressedData = stats.ExtractRange(utcStartDate, utcEndDate, TimeZoneInfo.CurrentTimeZone);
         chart.Data.Y.CompactFormula = _monthCompactor;
         chart.DataSource = new ListProvider(compressedData.PointsEx);
         // When compacting date/time, the value provided is day-based.  The following is telling
         //  ChartFx to compact data daily
         chart.Data.Compact(1);
         chart.AxisX.LabelsFormat.CustomFormat = "M/dd";
         chart.AxisX.Step = 5;
         chart.AxisX.MinorStep = 1;
         chart.RecalculateScale();
      }

      public static void ShowWeekChart(StatsDataSeries stats, Chart chart)
      {
         DateTime utcEndDate = DateTime.UtcNow;
         DateTime utcStartDate = utcEndDate.AddDays(-7);
         // ChartFx compacts on date/time boundaires from 12:00.  Because of this,
         //  we ensure that our start time is on an hour marker for the daily chart
         DateTime startDate = TimeZoneInfo.CurrentTimeZone.ToLocalTime(utcStartDate);
         startDate = startDate.AddMinutes(-startDate.Minute); // dump the minutes
         startDate = startDate.AddHours(-startDate.Hour % 6); // put hours at 0,6,12, or 18
         utcStartDate = TimeZoneInfo.CurrentTimeZone.ToUniversalTime(startDate);

         StatsDataSeries compressedData = stats.ExtractRange(utcStartDate, utcEndDate, TimeZoneInfo.CurrentTimeZone);
         chart.Data.Y.CompactFormula = _weekCompactor;
         chart.DataSource = new ListProvider(compressedData.PointsEx);
         // When compacting date/time, the value provided is day-based.  The following is telling
         //  ChartFx to compact data every 6 hours
         chart.Data.Compact(6.0 / 24.0);
         chart.AxisX.LabelsFormat.CustomFormat = "M/dd\nhh:mm tt";
         chart.RecalculateScale();
         chart.AxisX.Step = 42.0 / 24.0;
         chart.AxisX.MinorStep = 6.0 / 24.0;
      }

      public static void ShowDayChart(StatsDataSeries stats, Chart chart)
      {
         DateTime utcEndDate = DateTime.UtcNow;
         DateTime utcStartDate = utcEndDate.AddHours(-24);
         // ChartFx compacts on date/time boundaires from 12:00.  Because of this,
         //  we ensure that our start time is on an hour marker for the daily chart
         DateTime startDate = TimeZoneInfo.CurrentTimeZone.ToLocalTime(utcStartDate);
         startDate = startDate.AddMinutes(-startDate.Minute);
         utcStartDate = TimeZoneInfo.CurrentTimeZone.ToUniversalTime(startDate);

         StatsDataSeries compressedData = stats.ExtractRange(utcStartDate, utcEndDate, TimeZoneInfo.CurrentTimeZone);
         chart.Data.Y.CompactFormula = _dayCompactor;
         chart.DataSource = new ListProvider(compressedData.PointsEx);
         // When compacting date/time, the value provided is day-based.  The following is telling
         //  ChartFx to compact data hourly
         chart.Data.Compact(1.0 / 24.0);
         chart.AxisX.LabelsFormat.CustomFormat = "M/dd\nhh:mm tt";
         chart.RecalculateScale();
         chart.AxisX.Step = 6.0 / 24.0;
         chart.AxisX.MinorStep = 1.0 / 24.0;
//                  AxisSection section = new AxisSection(DateTime.UtcNow.AddHours(-7).ToOADate(), DateTime.UtcNow.AddHours(-6).ToOADate(), Color.Red);
//                  chart.AxisX.Sections.Add(section) ;
//                  section.Visible = true ;
//                  AxisSection section2 = new AxisSection(DateTime.UtcNow.AddHours(-18).ToOADate(), DateTime.UtcNow.AddHours(-16).ToOADate(), Color.Yellow);
//                  chart.AxisX.Sections.Add(section2);
//                  section2.Visible = true;
      }

      public static void ShowAxisSections(Chart chart, List<AxisRange> ranges)
      {
         chart.AxisX.Sections.Clear() ;

         foreach(AxisRange range in ranges)
         {
            AxisSection section = null ;
            double start = TimeZoneInfo.CurrentTimeZone.ToLocalTime(range.StartTime).ToOADate();
            double end = TimeZoneInfo.CurrentTimeZone.ToLocalTime(range.EndTime).ToOADate();
            if (range.Level == 1)
               section = new AxisSection(start, end, Color.FromArgb(255, 255, 123));
            else if(range.Level == 2)
               section = new AxisSection(start, end, Color.FromArgb(255,170,148));

            if(section != null)
            {
               DateTime startTime = TimeZoneInfo.CurrentTimeZone.ToLocalTime(range.StartTime);
               DateTime endTime = TimeZoneInfo.CurrentTimeZone.ToLocalTime(range.EndTime);
               chart.AxisX.Sections.Add(section);
               section.Text = String.Format("{0} threshold exceeded from {1} to {2}",
                  range.Level == 1 ? "Warning" : "Critical", startTime, endTime) ;
               section.Visible = true;
            }
         }
      }

      public static void ShowAxisSections(Chart chart, List<AxisRangePoint> points, DateTime start, DateTime end)
      {
         List<AxisSection> sections = ExtractSections(points, start, end) ;
         chart.AxisX.Sections.Clear() ;
         foreach(AxisSection section in sections)
         {
            chart.AxisX.Sections.Add(section);
            section.Visible = true;
         }
      }

      private static List<AxisSection> ExtractSections(List<AxisRangePoint> points, DateTime start, DateTime end)
      {
         List<AxisSection> retVal = new List<AxisSection> () ;
         double sectionStart = double.MinValue ;
         double sectionLevel = 0 ;

         foreach(AxisRangePoint pt in points)
         {
            // Is the point in our desired range?
            if(pt.Time >= start && pt.Time <= end)
            {
               // Are we starting the first section?
               if(sectionStart == double.MinValue && pt.Value > 0)
               {
                  sectionStart = pt.Time.ToOADate();
                  sectionLevel = pt.Value ;
               }
               else if(sectionLevel != pt.Value)
               {
                  // We need to end our previous section and possibly start a new one
                  if(sectionLevel == 1)
                     retVal.Add(new AxisSection(sectionStart, pt.Time.ToOADate(), Color.Yellow)) ;
                  else if(sectionLevel == 2)
                     retVal.Add(new AxisSection(sectionStart, pt.Time.ToOADate(), Color.Red)) ;

                  // start a new one if necessary
                  if(pt.Value > 0)
                  {
                     sectionStart = pt.Time.ToOADate() ;
                     sectionLevel = pt.Value ;
                  }else
                  {
                     sectionStart = double.MinValue ;
                     sectionLevel = 0 ;
                  }
               }
            }
         }
         // Finally, close the last section if we are in one.
         if(sectionStart != double.MinValue)
         {
            AxisRangePoint lastPoint = points[points.Count - 1] ;
            if (sectionLevel == 1)
               retVal.Add(new AxisSection(sectionStart, lastPoint.Time.AddMinutes(5).ToOADate(), Color.Yellow));
            else if (sectionLevel == 2)
               retVal.Add(new AxisSection(sectionStart, lastPoint.Time.AddMinutes(5).ToOADate(), Color.Red));
         }
         return retVal ;
      }

      //
      // You must do the undo compact to avoid an overflow exception.
      //
      public static void ShowEmptyChart(Chart chart)
      {
         chart.BeginUpdate() ;
         if(chart.Data != null)
         {
            chart.Data.UndoCompact() ;
            chart.Data.Clear() ;
         }
         chart.EndUpdate() ;
      }

      //
      // We have different sum formulas because we want to extrapolate for ranges that
      //  do not have a full set of values (this happens on the boundaries of the graphy).
      //  Our graphs calculate the start boundary to avoid this; however, it is unavoidable
      //  on the end boundary

      //This code is the handler for the delegate.
      public static double DaySumFormula( IDataArray data, int seriesIndex, int startPoint, int endPoint )
      {
         // Day sum expects 4 points per compaction (hour)
        double sum = 0;
        int numPoints = endPoint - startPoint + 1;
                
        for( int i = startPoint; i <= endPoint; i++ )
            sum += data[ seriesIndex, i ];

         if (numPoints < 4)
         {
            return sum / (double)numPoints * 4.0;
         }
         else
            return sum;
      }

      public static double WeekSumFormula(IDataArray data, int seriesIndex, int startPoint, int endPoint)
      {
         // Week sum expects 24 points per compaction (6 hours)
         double sum = 0;
         int numPoints = endPoint - startPoint + 1;

         for (int i = startPoint; i <= endPoint; i++)
            sum += data[seriesIndex, i];

         if (numPoints < 24)
         {
            return sum / (double)numPoints * 24.0;
         }
         else
            return sum;
      }

      public static double MonthSumFormula(IDataArray data, int seriesIndex, int startPoint, int endPoint)
      {
         // Month sum expects 96 points per compaction (day)
         double sum = 0;
         int numPoints = endPoint - startPoint + 1;

         for (int i = startPoint; i <= endPoint; i++)
            sum += data[seriesIndex, i];

         if (numPoints < 96)
         {
            return sum / (double)numPoints * 96.0;
         }
         else
            return sum;
      }

      public static double GenericSumFormula(IDataArray data, int seriesIndex, int startPoint, int endPoint)
      {
         double sum = 0;

         for (int i = startPoint; i <= endPoint; i++)
            sum += data[seriesIndex, i];

         return sum;
      }
   }
}
