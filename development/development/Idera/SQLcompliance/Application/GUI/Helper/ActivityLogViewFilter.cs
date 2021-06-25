using System;
using System.Text;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.TimeZoneHelper;
using TimeZoneInfo = Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
   public class ActivityLogViewFilter
   {
      #region Properties

      private DateFilterType _dateLimitType  ;
      private int _nDays ;
      private DateTime _startDate  ;
      private DateTime _endDate  ;
      private int _typeId ;
      private string _targetServer;

      #endregion

      #region Constructors

      public ActivityLogViewFilter()
      {
         _dateLimitType = DateFilterType.NumberDays ;
         _nDays = 30 ;
         _startDate = DateTime.Today ;
         _endDate = DateTime.Today ;
         _typeId = -1 ;
      }


      #endregion

      #region Properties

      public DateFilterType DateLimitType
      {
         get { return _dateLimitType ; }
         set { _dateLimitType = value ; }
      }

      public int Days
      {
         get { return _nDays ; }
         set { _nDays = value ; }
      }

      public DateTime StartDate
      {
         get { return _startDate ; }
         set { _startDate = value ; }
      }

      public DateTime EndDate
      {
         get { return _endDate ; }
         set { _endDate = value ; }
      }

      public int EventTypeId
      {
         get { return _typeId ; }
         set { _typeId = value ; }
      }

      public string TargetServer
      {
         get { return _targetServer ; }
         set { _targetServer = value ; }
      }

      #endregion

      #region Filter String

      //-----------------------------------------------------------------------
      // GetWhereClause
      //-----------------------------------------------------------------------
      public string GetWhereClause(TimeZoneInfo timeZoneInfo)
      {
         StringBuilder whereClause = new StringBuilder();

         if (_dateLimitType == DateFilterType.NumberDays)
         {
            DateTime end = DateTime.UtcNow;
            DateTime start = end.Subtract(new TimeSpan(_nDays, 0, 0, 0));

            whereClause.AppendFormat("eventTime >= {0} AND eventTime <= {1}",
               SQLHelpers.CreateSafeDateTime(start),
               SQLHelpers.CreateSafeDateTime(end));
         }
         else if (_dateLimitType == DateFilterType.DateRange)
         {
            DateTime start = _startDate;
            DateTime end = _endDate;

            // convert to utc
            start = TimeZoneInfo.ToUniversalTime(timeZoneInfo, start);
            end = TimeZoneInfo.ToUniversalTime(timeZoneInfo, end);

            whereClause.AppendFormat("eventTime >= {0} AND eventTime <= {1}",
                                     SQLHelpers.CreateSafeDateTime(start),
                                     SQLHelpers.CreateSafeDateTime(end));
         }
         else if (_dateLimitType == DateFilterType.Today)
         {
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Now;

            // convert to utc
            start = TimeZoneInfo.ToUniversalTime(timeZoneInfo, start);
            end = TimeZoneInfo.ToUniversalTime(timeZoneInfo, end);

            whereClause.AppendFormat("eventTime >= {0} AND eventTime <= {1}",
                                     SQLHelpers.CreateSafeDateTime(start),
                                     SQLHelpers.CreateSafeDateTime(end));
         }

         if (_typeId != -1)
         {
            if (whereClause.Length != 0) whereClause.Append(" AND ");
            whereClause.AppendFormat("logType={0}", _typeId);
         }

         if (_targetServer != null && _targetServer.Length > 0)
         {
            if (whereClause.Length != 0) whereClause.Append(" AND ");
            whereClause.Append(String.Format("logSqlServer={0}", SQLHelpers.CreateSafeString(_targetServer)));
         }

         return whereClause.ToString();
      }

      #endregion

      public ActivityLogViewFilter Clone()
      {
         ActivityLogViewFilter retVal = (ActivityLogViewFilter)MemberwiseClone();

         return retVal;
      }   }
}
