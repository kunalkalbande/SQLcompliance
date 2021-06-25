using System;
using System.Diagnostics;
using System.Text;
using Idera.SQLcompliance.Core;
using Microsoft.Win32;
using TimeZoneInfo = Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
    public enum DateFilterType : int
    {
        Unlimited = 0,
        NumberDays = 1,
        DateRange = 2,
        Today = 3
    }

    public class EventViewFilter
    {
        #region Properties

        private DateFilterType _dateLimitType;
        private int _nDays;
        private DateTime _startDate;
        private DateTime _endDate;
        private bool _showPrivUsersOnly;
        private bool _dbLevelPrivUser;
        private bool _archiveFilter;
        private int _categoryId;
        private int _typeId;
        private int? _applicationId;
        private int? _hostId;
        private int? _loginId;
        private int? _tableId;
        private int? _columnId;

        #endregion

        #region Constructors

        public EventViewFilter()
        {
            _dateLimitType = DateFilterType.Today;
            _nDays = 1;
            _startDate = DateTime.Today;
            _endDate = DateTime.Today;
            _showPrivUsersOnly = false;
            _dbLevelPrivUser = false;

            _archiveFilter = false;
            _categoryId = -1;
            _typeId = -1;
            _applicationId = null;
            _hostId = null;
            _loginId = null;
            _tableId = null;
            _columnId = null;
        }

        public EventViewFilter(bool inArchive)
        {
            _archiveFilter = inArchive;
            _dateLimitType = DateFilterType.Unlimited;
            _nDays = 90;
            _startDate = DateTime.Today;
            _endDate = DateTime.Today;
        }

        #endregion

        #region Properties

        public DateFilterType DateLimitType
        {
            get { return _dateLimitType; }
            set { _dateLimitType = value; }
        }

        public int Days
        {
            get { return _nDays; }
            set { _nDays = value; }
        }

        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }

        public bool ShowPrivUsersOnly
        {
            get { return _showPrivUsersOnly; }
            set { _showPrivUsersOnly = value; }
        }
        public bool DBLevelPrivUser
        {
            get { return _dbLevelPrivUser; }
            set { _dbLevelPrivUser = value; }
        }

        public bool IsArchiveFilter
        {
            get { return _archiveFilter; }
            set { _archiveFilter = value; }
        }

        public int EventCategoryId
        {
            get { return _categoryId; }
            set { _categoryId = value; }
        }

        public int EventTypeId
        {
            get { return _typeId; }
            set { _typeId = value; }
        }

        public int? ApplicationId
        {
            get { return _applicationId; }
            set { _applicationId = value; }
        }

        public int? HostId
        {
            get { return _hostId; }
            set { _hostId = value; }
        }

        public int? LoginId
        {
            get { return _loginId; }
            set { _loginId = value; }
        }

        public int? TableId
        {
            get { return _tableId; }
            set { _tableId = value; }
        }

        public int? ColumnId
        {
            get { return _columnId; }
            set { _columnId = value; }
        }

        #endregion

        #region Filter String

        //-----------------------------------------------------------------------
        // GetWhereClause
        //  Assumed table prefixes:
        //  Events - e
        //  DataChanges - d
        //  ColumnChanges - c
        //-----------------------------------------------------------------------
        public string GetWhereClause(string databaseName, TimeZoneInfo timeZoneInfo)
        {
            StringBuilder whereClause = new StringBuilder();

            if (_dateLimitType == DateFilterType.NumberDays)
            {
                DateTime end = DateTime.UtcNow;
                DateTime start = end.Subtract(new TimeSpan(_nDays, 0, 0, 0));

                whereClause.AppendFormat("e.startTime >= {0} AND e.startTime <= {1}",
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

                whereClause.AppendFormat("e.startTime >= {0} AND e.startTime <= {1}",
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

                whereClause.AppendFormat("e.startTime >= {0} AND e.startTime <= {1}",
                                         SQLHelpers.CreateSafeDateTime(start),
                                         SQLHelpers.CreateSafeDateTime(end));
            }

            if (String.IsNullOrEmpty(databaseName) == false)
            {
                if (whereClause.Length != 0) whereClause.Append(" AND ");
                //Fix for SQLCM-5789, Added collate option to compare
                whereClause.AppendFormat("e.databaseName like '{0}' COLLATE Latin1_General_CI_AS", databaseName);
            }

            if (_typeId != -1)
            {
                if (whereClause.Length != 0) whereClause.Append(" AND ");
                whereClause.AppendFormat("e.eventType={0}", _typeId);
            }
            else if (_categoryId != -1)
            {
                if (whereClause.Length != 0) whereClause.Append(" AND ");
                whereClause.AppendFormat("e.eventCategory={0}", _categoryId);
            }

            if (_showPrivUsersOnly)
            {
                if (whereClause.Length != 0) whereClause.Append(" AND ");
                whereClause.Append("e.privilegedUser=1");
            }
            //else
            //{
            //    if (whereClause.Length != 0) whereClause.Append(" AND ");
            //    whereClause.Append("e.privilegedUser=0");
            //}

            if (_applicationId != null)
            {
                if (whereClause.Length != 0) whereClause.Append(" AND ");
                whereClause.Append(String.Format("e.appNameId={0}", _applicationId));
            }

            if (_hostId != null)
            {
                if (whereClause.Length != 0) whereClause.Append(" AND ");
                whereClause.Append(String.Format("e.hostId={0}", _hostId));
            }

            if (_loginId != null)
            {
                if (whereClause.Length != 0) whereClause.Append(" AND ");
                whereClause.Append(String.Format("e.loginId={0}", _loginId));
            }

            if (IsBeforeAfterEnabled() && _tableId != null)
            {
                if (whereClause.Length != 0) whereClause.Append(" AND ");
                whereClause.Append(String.Format("d.tableId={0}", _tableId));
            }

            if (IsBeforeAfterEnabled() && _columnId != null)
            {
                if (whereClause.Length != 0) whereClause.Append(" AND ");
                whereClause.Append(String.Format("c.columnId={0}", _columnId));
            }


            return whereClause.ToString();
        }

        public bool IsBeforeAfterEnabled()
        {
            bool enabled = false;
            // All categories - BA enabled
            if (_categoryId == -1)
                enabled = true;
            // DML and not Execute - BA Enabled
            if (_categoryId == 4 && _typeId != 32)
                enabled = true;
            return enabled;
        }

        #endregion

        #region Registry Persistence

        //-----------------------------------------------------------------------
        // ReadFromRegistry
        //-----------------------------------------------------------------------
        public void ReadFromRegistry(RegistryKey parentKey, string filterKey)
        {
            RegistryKey rks = null;

            try
            {
                int tmp;

                int defaultType;
                int defaultDays;

                if (_archiveFilter)
                {
                    defaultType = 0;
                    defaultDays = 90;
                }
                else
                {
                    defaultType = 3;
                    defaultDays = 1;
                }


                rks = parentKey.CreateSubKey(filterKey);

                tmp = (int)rks.GetValue(UIConstants.RegVal_LimitType, defaultType);
                _dateLimitType = (DateFilterType)tmp;
                _nDays = (int)rks.GetValue(UIConstants.RegVal_nDays, defaultDays);

                string dt;
                dt = (string)rks.GetValue(UIConstants.RegVal_StartDate, DateTime.Now.ToString());
                _startDate = Convert.ToDateTime(dt);
                dt = (string)rks.GetValue(UIConstants.RegVal_EndDate, DateTime.Now.ToString());
                _endDate = Convert.ToDateTime(dt);

                _showPrivUsersOnly = ((int)rks.GetValue(UIConstants.RegVal_PrivUsers, 0) != 0);

                _categoryId = (int)rks.GetValue(UIConstants.RegVal_CategoryId, -1);
                _typeId = (int)rks.GetValue(UIConstants.RegVal_TypeId, -1);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                // if we have trouble reading, just set the whole things to default values
                _dateLimitType = (_archiveFilter) ? DateFilterType.Unlimited : DateFilterType.Today;
                _nDays = 1;
                _startDate = DateTime.Now;
                _endDate = DateTime.Now;
                _categoryId = -1;
                _typeId = -1;
            }
            finally
            {
                if (rks != null) rks.Close();
            }
        }

        #endregion

        public EventViewFilter Clone()
        {
            EventViewFilter retVal = (EventViewFilter)MemberwiseClone();

            return retVal;
        }
    }
}