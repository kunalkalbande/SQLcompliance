using System ;
using System.Diagnostics ;
using System.Text ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Rules ;
using Idera.SQLcompliance.Core.Rules.Alerts ;
using Microsoft.Win32 ;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
   public enum AlertSelectionType
   {
      SelectAll,
      SelectPastDays,
      SelectRanged,
      SelectToday
   } ;

   /// <summary>
   /// Summary description for AlertFilter.
   /// </summary>
   public class AlertViewFilter
   {
      #region Member Variables

      private AlertSelectionType _selectionType ;
      private int _days ;
      private DateTime _startDate ;
      private DateTime _endDate ;

      private bool _useType ;
      private EventType _alertType ;

      private bool _useLevel ;
      private AlertLevel _alertLevel ;

      private string _targetServer ;

      #endregion

      #region Properties

      public AlertSelectionType SelectionType
      {
         get { return _selectionType ; }
         set { _selectionType = value ; }
      }

      public int Days
      {
         get { return _days ; }
         set { _days = value ; }
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

      public bool UseType
      {
         get { return _useType ; }
         set { _useType = value ; }
      }

      public EventType AlertType
      {
         get { return _alertType ; }
         set { _alertType = value ; }
      }

      public bool UseLevel
      {
         get { return _useLevel ; }
         set { _useLevel = value ; }
      }

      public AlertLevel AlertLevel
      {
         get { return _alertLevel ; }
         set { _alertLevel = value ; }
      }

      public string TargetServer
      {
         get { return _targetServer ; }
         set { _targetServer = value ; }
      }

      #endregion

      #region Construction/Destruction

      public AlertViewFilter()
      {
         _selectionType = AlertSelectionType.SelectPastDays ;
         _days = 7 ;
         _startDate = DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0)) ;
         _endDate = DateTime.Now ;
         _useType = false ;
         _alertType = EventType.SqlServer ;
         _useLevel = false ;
         _alertLevel = AlertLevel.High ;
      }

      #endregion

      public string GetWhereClause(EventType type)
      {
         StringBuilder whereFilterClause = new StringBuilder(150) ;
         DateTime startDate, endDate ;

         switch(_selectionType)
         {
            case AlertSelectionType.SelectAll:
               break ;
            case AlertSelectionType.SelectPastDays:
               startDate = DateTime.Now.Date.Subtract(new TimeSpan(_days, 0, 0, 0)).ToUniversalTime() ;
               whereFilterClause.Append(" created >= CONVERT(DATETIME, '") ;
               whereFilterClause.AppendFormat("{0}-{1}-{2} {3}:{4}:{5}", startDate.Year, startDate.Month,
                                              startDate.Day, startDate.Hour, startDate.Minute, startDate.Second) ;
               whereFilterClause.Append("',120) ") ;
               break ;
            case AlertSelectionType.SelectRanged:
               startDate = _startDate.ToUniversalTime() ;
               endDate = _endDate.ToUniversalTime() ;
               whereFilterClause.Append(" created >= CONVERT(DATETIME, '") ;
               whereFilterClause.AppendFormat("{0}-{1}-{2} {3}:{4}:{5}", startDate.Year, startDate.Month,
                                              startDate.Day, startDate.Hour, startDate.Minute, startDate.Second) ;
               whereFilterClause.Append("',120) AND created <= CONVERT(DATETIME, '") ;
               whereFilterClause.AppendFormat("{0}-{1}-{2} {3}:{4}:{5}", endDate.Year, endDate.Month,
                                              endDate.Day, endDate.Hour, endDate.Minute, endDate.Second) ;
               whereFilterClause.Append("',120) ") ;
               break ;
            case AlertSelectionType.SelectToday:
               startDate = DateTime.Today.ToUniversalTime() ;
               endDate = DateTime.Now.ToUniversalTime() ;
               whereFilterClause.Append(" created >= CONVERT(DATETIME, '") ;
               whereFilterClause.AppendFormat("{0}-{1}-{2} {3}:{4}:{5}", startDate.Year, startDate.Month,
                                              startDate.Day, startDate.Hour, startDate.Minute, startDate.Second) ;
               whereFilterClause.Append("',120) AND created <= CONVERT(DATETIME, '") ;
               whereFilterClause.AppendFormat("{0}-{1}-{2} {3}:{4}:{5}", endDate.Year, endDate.Month,
                                              endDate.Day, endDate.Hour, endDate.Minute, endDate.Second) ;
               whereFilterClause.Append("',120) ") ;
               break ;
         }

         if(_useLevel)
         {
            if(whereFilterClause.Length > 0)
               whereFilterClause.Append(" AND ") ;
            whereFilterClause.AppendFormat("alertLevel={0}", (int)_alertLevel) ;
         }
         if(_useType)
         {
            if(whereFilterClause.Length > 0)
               whereFilterClause.Append(" AND ") ;
            whereFilterClause.AppendFormat("alertType={0}", (int)_alertType) ;
         }
         if(_targetServer != null && _targetServer.Length > 0)
         {
            if(whereFilterClause.Length > 0)
               whereFilterClause.Append(" AND ") ;
            whereFilterClause.AppendFormat("instance={0}", SQLHelpers.CreateSafeString(_targetServer)) ;
         }

         switch (type)
         {
            case EventType.SqlServer:
               if (whereFilterClause.Length > 0)
                  whereFilterClause.Append(" AND ");
               whereFilterClause.Append(" alertType = 1");
               break;
            case EventType.Status:
               if (whereFilterClause.Length > 0)
                  whereFilterClause.Append(" AND ");
               whereFilterClause.Append(" alertType = 2");
               break;
            case EventType.Data:
               if (whereFilterClause.Length > 0)
                  whereFilterClause.Append(" AND ");
               whereFilterClause.Append(" alertType = 3");
               break;
         };
         return whereFilterClause.ToString() ;
      }

      public string BuildFilterString()
      {
         StringBuilder builder = new StringBuilder(100) ;
         StringBuilder alertType = new StringBuilder(50) ;

         if(_useLevel)
            alertType.AppendFormat(" {0} level", _alertLevel) ;

         if(_useType)
            alertType.AppendFormat(" {0}", _alertType) ;

         switch(_selectionType)
         {
            case AlertSelectionType.SelectAll:
               builder.AppendFormat("Show all{0} alerts", alertType) ;
               break ;
            case AlertSelectionType.SelectPastDays:
               builder.AppendFormat("Show all{0} alerts from the past {1} days", alertType, _days) ;
               break ;
            case AlertSelectionType.SelectRanged:
               builder.AppendFormat("Show all{0} alerts from {1} {2} to {3} {4}", alertType,
                                    _startDate.ToShortDateString(), _startDate.ToShortTimeString(),
                                    _endDate.ToShortDateString(), _endDate.ToShortTimeString()) ;
               break ;
         }

         return builder.ToString() ;
      }

      public void WriteToRegistry(RegistryKey parentKey, string filterKey)
      {
         RegistryKey rks = null ;

         try
         {
            rks = parentKey.CreateSubKey(filterKey) ;

            rks.SetValue(UIConstants.RegVal_LimitType, (int)_selectionType) ;
            rks.SetValue(UIConstants.RegVal_nDays, _days) ;
            rks.SetValue(UIConstants.RegVal_StartDate, _startDate) ;
            rks.SetValue(UIConstants.RegVal_EndDate, _endDate) ;
            rks.SetValue(UIConstants.RegVal_AlertLevel, (_useLevel) ? 0 : (int)_alertLevel) ;
         }
         catch(Exception ex)
         {
            Debug.WriteLine(ex.Message) ;
         }
         finally
         {
            if(rks != null) rks.Close() ;
         }
      }

      public void ReadFromRegistry(RegistryKey parentKey, string filterKey)
      {
         RegistryKey rks = null ;

         try
         {
            int tmp ;

            rks = parentKey.CreateSubKey(filterKey) ;

            tmp = (int)rks.GetValue(UIConstants.RegVal_LimitType, (int)AlertSelectionType.SelectPastDays) ;
            _selectionType = (AlertSelectionType)tmp ;
            _days = (int)rks.GetValue(UIConstants.RegVal_nDays, 7) ;

            string dt ;
            dt = (string)rks.GetValue(UIConstants.RegVal_StartDate, DateTime.Now.ToString()) ;
            _startDate = Convert.ToDateTime(dt) ;
            dt = (string)rks.GetValue(UIConstants.RegVal_EndDate, DateTime.Now.ToString()) ;
            _endDate = Convert.ToDateTime(dt) ;

            tmp = (int)rks.GetValue(UIConstants.RegVal_PrivUsers, 0) ;
            if(tmp == 0)
               _useLevel = false ;
            else
            {
               _useLevel = true ;
               _alertLevel = (AlertLevel)tmp ;
            }
         }
         catch(Exception ex)
         {
            Debug.WriteLine(ex.Message) ;

            // if we have trouble reading, just set the whole things to default values
            _selectionType = AlertSelectionType.SelectPastDays ;
            _days = 7 ;
            _startDate = DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0)) ;
            _endDate = DateTime.Now ;
            _useLevel = false ;
         }
         finally
         {
            if(rks != null) rks.Close() ;
         }
      }

      public AlertViewFilter Clone()
      {
         AlertViewFilter retVal = (AlertViewFilter)MemberwiseClone();
         return retVal;
      }
   }
}