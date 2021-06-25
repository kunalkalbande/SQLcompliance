using System;
using Idera.SQLcompliance.Core.Agent;
using TimeZoneInfo = Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;

namespace SQLcomplianceCwfAddin.Helpers
{
    public class DateTimeHelper
    {
        public static DateTime? GetNullableDateTime(DateTime dateTime)
        {
            return dateTime == DateTime.MinValue ? null : (DateTime?) dateTime;
        }

        public static DateTime? GetLocalTimeOfCurrentTimeZone(DateTime? dateTime)
        {
            DateTime? localTime = null;

            if (dateTime.HasValue)
            {
                localTime = TimeZoneInfo.CurrentTimeZone.ToLocalTime(dateTime.Value);
            }

            return localTime;
        }

        public static DateTime? GetNullableLocalTimeOfCurrentTimeZone(DateTime dateTime)
            {
            var time = GetNullableDateTime(dateTime);
            var localTime = GetLocalTimeOfCurrentTimeZone(time);
            return localTime;
        }

        public static DateTime? GetUtcTime(DateTime? localTime)
        {
            DateTime? utcTime = null;

            if (localTime.HasValue)
            {
                utcTime = TimeZoneInfo.CurrentTimeZone.ToUniversalTime(localTime.Value);
            }

            return utcTime;
        }

        public static string GetUTCCultureFreeDateTime(DateTime localTime)
        {
            DateTime dt = TimeZone.CurrentTimeZone.ToUniversalTime(localTime);
            return dt.ToString("s");
        }
    }
}
