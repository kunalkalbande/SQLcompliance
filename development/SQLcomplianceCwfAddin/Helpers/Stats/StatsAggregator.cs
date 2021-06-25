using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Stats;
using SQLcomplianceCwfAddin.Helpers.SQL;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Stats;
using TimeZoneInfo = Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;

namespace SQLcomplianceCwfAddin.Helpers.Stats
{
    public static class StatsAggregator
    {
        public static List<KeyValuePair<RestStatsCategory, List<RestStatsData>>> GetStatsDataInternal(List<ServerRecord> servers, int? dbId, int days, IEnumerable<RestStatsCategory> categories, SqlConnection connection)
        {
            var stats = new List<KeyValuePair<RestStatsCategory, List<RestStatsData>>>();

            DateTime endDate = DateTime.UtcNow;
            DateTime startDate = endDate.AddDays(-31);

            foreach (var category in categories)
            {
                var statsCategory = (StatsCategory)Enum.ToObject(typeof(StatsCategory), category);
                var coreStats = StatsExtractor.GetEnterpriseStatistics(connection, statsCategory, servers, startDate, endDate);
                //SCM-416 null pointer exception handled here
                if (servers.Count != 0)
                {
                    List<RestStatsData> statsDataList = GetStatsDataListForCategory(category, days, startDate, endDate, coreStats, servers[0].SrvId, connection);  //SCM-4
                    stats.Add(new KeyValuePair<RestStatsCategory, List<RestStatsData>>(category, statsDataList));
                }
            }

            return stats;
        }

        public static List<KeyValuePair<RestStatsCategory, List<RestStatsData>>> GetDatabaseActivityStatsDataInternal(int? serverId, int? dbId, int days, IEnumerable<RestStatsCategory> categories, SqlConnection connection)
        {
            var stats = new List<KeyValuePair<RestStatsCategory, List<RestStatsData>>>();

            DateTime utcEndDate = DateTime.UtcNow;
            DateTime utcStartDate = utcEndDate.AddDays(-days);
            utcStartDate = GetCorrectedStartDateTime(utcStartDate, days);


            var serverRecord = SqlCmRecordReader.GetServerRecord(serverId.Value, connection);
            var databaseRecord = SqlCmRecordReader.GetDatabaseRecord(dbId.Value, connection);

            foreach (var category in categories)
            {
                var statsCategory = (StatsCategory)Enum.ToObject(typeof(StatsCategory), category);
                var coreStats = StatsExtractor.GetDatabaseStatistics(connection, serverRecord, databaseRecord, statsCategory, utcStartDate, utcEndDate);
                List<RestStatsData> statsDataList = GetStatsDataListForCategory(category, days, utcStartDate, utcEndDate, coreStats, serverId.Value, connection); //SCM-4
                stats.Add(new KeyValuePair<RestStatsCategory, List<RestStatsData>>(category, statsDataList));
            }

            return stats;
        }

        private static DateTime GetDateTimeWithZeroMinuteAndSeconds(DateTime time)
        {
            var second = 0;
            var millisecond = 0;
            var minute = 0;
            var hourTime = new DateTime(time.Year, time.Month, time.Day, time.Hour, minute, second, millisecond, time.Kind);
            return hourTime;
        }

        private static DateTime GetCorrectedStartDateTime(DateTime utcStartDate, int days)
        {
            // ChartFx compacts on date/time boundaries from 12:00.  Because of this,
            //  we ensure that our start time is on an hour marker for the daily chart
            DateTime startDate = TimeZoneInfo.CurrentTimeZone.ToLocalTime(utcStartDate);

            switch (days)
            {
                case 1:
                    startDate = startDate.AddMinutes(-startDate.Minute);
                    break;
                case 7:
                    startDate = startDate.AddMinutes(-startDate.Minute);
                    startDate = startDate.AddHours(-startDate.Hour % 6); // put hours at 0,6,12, or 18
                    break;
                case 30:
                    startDate = startDate.Date;
                    break;
            }

            return TimeZoneInfo.CurrentTimeZone.ToUniversalTime(startDate);
        }

        private static List<StatsDataPoint> GetStatsDataPointListForChart(StatsDataSeries stats, int days)
        {
            var dayPoints = new List<StatsDataPoint>();

            DateTime utcEndDate = DateTime.UtcNow;
            DateTime utcStartDate = utcEndDate;

            switch (days)
            {
                case 1:
                    utcStartDate = utcEndDate.AddHours(-24);
                    break;
                case 7:
                    utcStartDate = utcEndDate.AddDays(-7);
                    break;
                case 30:
                    utcStartDate = utcEndDate.AddDays(-30);
                    break;
            }

            utcStartDate = GetCorrectedStartDateTime(utcStartDate, days);

            var compressedData = stats.ExtractRange(utcStartDate, utcEndDate, TimeZoneInfo.CurrentTimeZone);

            var firstHourTime = compressedData.Points[0].Time;

            var lastPoint = compressedData.Points[compressedData.Points.Count - 1];

            var startTime = GetDateTimeWithZeroMinuteAndSeconds(firstHourTime);
            var endTime = GetDateTimeWithZeroMinuteAndSeconds(lastPoint.Time);

            var intervalIn15minutes = 1;// 4 * 15 minutes

            switch (days)
            {
                case 1:
                    intervalIn15minutes = 4;// 1 hour
                    break;
                case 7:
                    intervalIn15minutes = 24;// 6 hour
                    break;
                case 30:
                    intervalIn15minutes = 96;// 24 hour
                    break;
            }

            var newCompressedData = compressedData.CompressSeriesWithSum(startTime, endTime, intervalIn15minutes);

            foreach (var point in newCompressedData.Points)
            {
                var updatedPoint = point.Clone();
                dayPoints.Add(updatedPoint);
            }

            return dayPoints;
        }

        private static List<RestStatsData> GetStatsDataListForCategory(RestStatsCategory category, int days, DateTime startDate, DateTime endDate, StatsDataSeries coreStats, int srvId, SqlConnection connection) //SCM-4
        {

            var statsDataList = new List<RestStatsData>();
            // SCM-4 Start
            int criticalThresholdValue = 0;
            int warningThresholdValue = 0;
            try
            {
                criticalThresholdValue = ReportCardRecord.GetReportCardEntry(connection, srvId, (int)category).CriticalThreshold;
                warningThresholdValue = ReportCardRecord.GetReportCardEntry(connection, srvId, (int)category).WarningThreshold;
            }
            catch(Exception ex){}
            // SCM-4 End
            List<StatsDataPoint> coreStatsPoints = GetStatsDataPointListForChart(coreStats, days);

            if (coreStatsPoints != null)
            {
                foreach (var point in coreStatsPoints)
                {
                    var statsDate = new RestStatsData { Date = point.Time, LastUpdated = point.Time, Count = Convert.ToInt32(point.Value), Category = category, CategoryName = "Category Name", CriticalThreshold = criticalThresholdValue, WarningThreshold = warningThresholdValue }; //SCM-4 
                    statsDataList.Add(statsDate);
                }
            }

            return statsDataList;
        }
    }
}
