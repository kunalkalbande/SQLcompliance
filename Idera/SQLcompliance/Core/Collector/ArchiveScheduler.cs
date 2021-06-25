using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Net.Sockets;
using System.Security.Principal;
using System.Threading;
using Idera.SQLcompliance.Core.Remoting;

namespace Idera.SQLcompliance.Core.Collector
{
    /// <summary>
    /// This class handles scheduled archive process execution.
    /// To have only one instance of the class all the time, it is singleton.
    /// It invokes background archiving process at timed schedules. 
    /// </summary>
    public class ArchiveScheduler
    {
        #region members

        private static ArchiveScheduler _instance;
        private Thread _schedulerThread;
        private volatile DateTime[] _executionPlanDates;
        private DateTime _nextExecutionSchedule = DateTime.MinValue;
        private volatile bool _isRunning;
        
        #endregion

        private ArchiveScheduler(){ }

        #region properties

        public static ArchiveScheduler Instance
        {
            get { return _instance ?? (_instance = new ArchiveScheduler()); }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        public static bool IsScheduledArchiveRunning
        {
            get { return GetIsScheduledArchiveRunning(); }
        }

        #endregion

        #region public methods

        /// <summary>
        /// This method starts scheduler.
        /// </summary>
        public void Start(bool generateSchedulePlan)
        {
            _isRunning = true;
            SQLcomplianceConfiguration configuration = SQLcomplianceConfiguration.GetConfiguration();

            // get saved execution plan from repositary
            if (!generateSchedulePlan)
                _executionPlanDates = SQLcomplianceConfiguration.GetArchiveSchedulerExecutionPlan();
            else
            {
                _executionPlanDates = GenerateSchedulePlan(configuration);
                
                // failed to generate new execution plan
                // this may be due to no schedule set
                if (_executionPlanDates == null || _executionPlanDates.Length == 0)
                {
                    _isRunning = false;
                    return;
                }
            }

            DateTime currentDay = DateTime.Now;

            _nextExecutionSchedule = PickNextExecutionSchedule(_executionPlanDates, currentDay);

            // this may be due to execution has completed on all plan dates
            // no more dates left for execution
            // generate new schedule
            if (_nextExecutionSchedule == DateTime.MinValue)
            {
                _executionPlanDates = GenerateSchedulePlan(configuration);

                // failed to generate new execution plan
                // this may be due to no schedule set
                if (_executionPlanDates == null || _executionPlanDates.Length == 0)
                {
                    _isRunning = false;
                    return;
                }
            }

            // start scheduler thread
            _schedulerThread = new Thread(TriggerArchiveProcess);
            _schedulerThread.IsBackground = true;
            _schedulerThread.Priority = ThreadPriority.BelowNormal;
            _schedulerThread.Start();
        }

        /// <summary>
        /// This method stops scheduler.
        /// </summary>
        public void Stop()
        {
            _isRunning = false;

            // signal scheduler thread to stop
            if (_schedulerThread != null && _schedulerThread.IsAlive)
                _schedulerThread.Abort();
        }

        /// <summary>
        /// This method restarts scheduler.
        /// </summary>
        public void Reatart(bool generateSchedulePlan)
        {
            Stop();
            Start(generateSchedulePlan);
        }

        #endregion

        #region private methods

        /// <summary>
        /// Read settings to know status of scheduled archive running.
        /// </summary>
        /// <returns>True if scheduled archiving is running.</returns>
        private static bool GetIsScheduledArchiveRunning()
        {
            bool isScheduledArchiveRunning = false;
            string sql = String.Format("SELECT archiveScheduleIsArchiveRunning FROM {0}..{1}",
                                        CoreConstants.RepositoryDatabase,
                                        CoreConstants.RepositoryConfigurationTable);

            Repository repository = new Repository();
            try
            {
                repository.OpenConnection();

                using (SqlCommand cmd = new SqlCommand(sql, repository.connection))
                {
                    object valueReturned = cmd.ExecuteScalar();
                    if (valueReturned != null)
                        isScheduledArchiveRunning = Convert.ToInt32(valueReturned) == 1;
                }
            }
            finally
            {
                repository.CloseConnection();
            }

            return isScheduledArchiveRunning;
        }

        /// <summary>
        /// Save status of scheduled archiving running in settings.
        /// </summary>
        private void SetIsScheduledArchiveRunning(bool isRunning)
        {
            string sql = String.Format("UPDATE {0}..{1}  SET archiveScheduleIsArchiveRunning = {2}",
                                        CoreConstants.RepositoryDatabase,
                                        CoreConstants.RepositoryConfigurationTable,
                                        isRunning ? 1 : 0);

            Repository repository = new Repository();
            try
            {
                repository.OpenConnection();

                using (SqlCommand cmd = new SqlCommand(sql, repository.connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                repository.CloseConnection();
            }
        }

        /// <summary>
        /// This method generates archive scheduler execution plan dates for one year taking today as starting date.
        /// </summary>
        /// <param name="configuration">Configurations to read scheduler settings.</param>
        /// <returns></returns>
        private DateTime[] GenerateSchedulePlan(SQLcomplianceConfiguration configuration)
        {
            DateTime[] executionPlanDates = GenerateSchedulePlan(configuration, DateTime.Now);

            // failed to generate new execution plan
            // this may be due to no schedule set
            if (executionPlanDates == null || executionPlanDates.Length == 0)
            {
                SQLcomplianceConfiguration.RemoveArchiveScheduleExecutionPlan();
                return null;
            }

            // save execution plan
            SQLcomplianceConfiguration.SetArchiveSchedulerExecutionPlan(executionPlanDates);

            return executionPlanDates;
        }

        /// <summary>
        /// This method generates archive scheduler execution plan dates for one year.
        /// </summary>
        /// <param name="configuration">Configurations to read scheduler settings.</param>
        /// <param name="plantStartsFrom">Starting date of execution plan.</param>
        /// <returns></returns>
        private DateTime[] GenerateSchedulePlan(SQLcomplianceConfiguration configuration, DateTime plantStartsFrom)
        {
            DateTime currentDay = DateTime.Now;
            Calendar calendar = CultureInfo.CurrentCulture.Calendar;

            DateTime planStartTime = plantStartsFrom;
            DateTime planEndTime = planStartTime.AddYears(1);

            List<DateTime> schedulePlan = new List<DateTime>();

            
            switch (configuration.ArchiveSchedule)
            {
                    // daily calculations
                case SQLcomplianceConfiguration.ArchiveScheduleType.Daily:
                {
                    DateTime nextSchedule = planStartTime;
                    while (nextSchedule.Date <= planEndTime.Date)
                    {
                        // skip adding today's schedule if schedule time has already passed.
                        if (nextSchedule.Date == currentDay.Date && currentDay.TimeOfDay > configuration.ArchiveScheduleTime.TimeOfDay)
                            goto SKIP_ADDING_SCHEDULE;

                        schedulePlan.Add(new DateTime(nextSchedule.Year,
                                                      nextSchedule.Month,
                                                      nextSchedule.Day,
                                                      configuration.ArchiveScheduleTime.Hour,
                                                      configuration.ArchiveScheduleTime.Minute,
                                                      configuration.ArchiveScheduleTime.Second));
                    SKIP_ADDING_SCHEDULE:
                        nextSchedule = nextSchedule.AddDays(1);
                    }
                }
                break;

                    // weekly calculations
                case SQLcomplianceConfiguration.ArchiveScheduleType.Weekly:
                {
                    DateTime nextSchedule = planStartTime;
                    while (nextSchedule.Date <= planEndTime.Date)
                    {
                        DateTime weekStart;
                        DateTime weekEnd;

                        switch (nextSchedule.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                weekStart = nextSchedule;
                                weekEnd = nextSchedule.AddDays(6);
                                break;
                            case DayOfWeek.Monday:
                                weekStart = nextSchedule.Subtract(new TimeSpan(1, 0, 0, 0));
                                weekEnd = nextSchedule.AddDays(5);
                                break;
                            case DayOfWeek.Tuesday:
                                weekStart = nextSchedule.Subtract(new TimeSpan(2, 0, 0, 0));
                                weekEnd = nextSchedule.AddDays(4);
                                break;
                            case DayOfWeek.Wednesday:
                                weekStart = nextSchedule.Subtract(new TimeSpan(3, 0, 0, 0));
                                weekEnd = nextSchedule.AddDays(3);
                                break;
                            case DayOfWeek.Thursday:
                                weekStart = nextSchedule.Subtract(new TimeSpan(4, 0, 0, 0));
                                weekEnd = nextSchedule.AddDays(2);
                                break;
                            case DayOfWeek.Friday:
                                weekStart = nextSchedule.Subtract(new TimeSpan(5, 0, 0, 0));
                                weekEnd = nextSchedule.AddDays(1);
                                break;
                            default: // saturday
                                weekStart = nextSchedule.Subtract(new TimeSpan(6, 0, 0, 0));
                                weekEnd = nextSchedule;
                                break;
                        }

                        DateTime nextScheduleWeekDay = weekStart;
                        while (nextScheduleWeekDay.Date <= weekEnd)
                        {
                            bool isWeekdaySelected = configuration.ArchiveScheduleWeekDays[(int) nextScheduleWeekDay.DayOfWeek];

                            // skip adding schedule if date has already passed
                            if (isWeekdaySelected && nextScheduleWeekDay.Date >= currentDay.Date)
                            {
                                // skip adding today's schedule if schedule time has already passed.
                                if (nextScheduleWeekDay.Date == currentDay.Date && currentDay.TimeOfDay > configuration.ArchiveScheduleTime.TimeOfDay)
                                    goto SKIP_ADDING_WEEKDAY_SCHEDULE;

                                schedulePlan.Add(new DateTime(nextScheduleWeekDay.Year,
                                                              nextScheduleWeekDay.Month,
                                                              nextScheduleWeekDay.Day,
                                                              configuration.ArchiveScheduleTime.Hour,
                                                              configuration.ArchiveScheduleTime.Minute,
                                                              configuration.ArchiveScheduleTime.Second));
                            }

                        SKIP_ADDING_WEEKDAY_SCHEDULE:
                            nextScheduleWeekDay = nextScheduleWeekDay.AddDays(1);
                        }

                        // calculate next date of recurring week
                        nextSchedule = calendar.AddWeeks(nextSchedule, configuration.ArchiveScheduleRepetition);
                    }
                }
                break;

                    // monthly date wise calculation
                case SQLcomplianceConfiguration.ArchiveScheduleType.MonthlyDateWise:
                {
                    DateTime nextSchedule = planStartTime;
                    while (nextSchedule.Date <= planEndTime.Date)
                    {
                        // add date if it comes in next recurring month
                        if (configuration.ArchiveScheduleDayOrWeekOfMonth <= calendar.GetDaysInMonth(nextSchedule.Year, nextSchedule.Month))
                        {
                            DateTime nextScheduleMonthDay = new DateTime(nextSchedule.Year,
                                                                         nextSchedule.Month,
                                                                         configuration.ArchiveScheduleDayOrWeekOfMonth,
                                                                         configuration.ArchiveScheduleTime.Hour,
                                                                         configuration.ArchiveScheduleTime.Minute,
                                                                         configuration.ArchiveScheduleTime.Second);

                             // skip adding schedule if date has already passed
                            if (nextScheduleMonthDay.Date >= currentDay.Date)
                            {
                                // skip adding today's schedule if schedule time has already passed.
                                if (nextScheduleMonthDay.Date == currentDay.Date && currentDay.TimeOfDay > configuration.ArchiveScheduleTime.TimeOfDay)
                                    goto SKIP_ADDING_MONTHDAY_SCHEDULE;

                                schedulePlan.Add(nextScheduleMonthDay);
                            }
                        }

                    SKIP_ADDING_MONTHDAY_SCHEDULE:
                        // calculate next date of recurring month
                        nextSchedule = calendar.AddMonths(nextSchedule, configuration.ArchiveScheduleRepetition);
                    }
                }
                break;

                    // monthly weekday wise calculations
                case SQLcomplianceConfiguration.ArchiveScheduleType.MonthlyWeekdayWise:
                {
                    // get day of week scheduled for archiving
                    DayOfWeek scheduledDayOfWeek = 0;
                    for (int index = 0; index < configuration.ArchiveScheduleWeekDays.Length; index += 1)
                    {
                        if (configuration.ArchiveScheduleWeekDays[index])
                        {
                            scheduledDayOfWeek = (DayOfWeek)index;
                            break;
                        }
                    }

                    DateTime nextSchedule = planStartTime;
                    while (nextSchedule.Date <= planEndTime.Date)
                    {
                        DateTime firstDateOfMonth = new DateTime(nextSchedule.Year, nextSchedule.Month, 1);

                        // find first occurance of schedule weekday
                        while (firstDateOfMonth.DayOfWeek != scheduledDayOfWeek)
                            firstDateOfMonth = firstDateOfMonth.AddDays(1);

                        // days to add = weeks to add * 7
                        int daysToAdd = (configuration.ArchiveScheduleDayOrWeekOfMonth - 1)*7;
                        DateTime scheduleWeekDate = firstDateOfMonth.AddDays(daysToAdd);

                        // add schedule time to calculated schedule date
                        scheduleWeekDate = new DateTime(scheduleWeekDate.Year,
                                                        scheduleWeekDate.Month,
                                                        scheduleWeekDate.Day,
                                                        configuration.ArchiveScheduleTime.Hour,
                                                        configuration.ArchiveScheduleTime.Minute,
                                                        configuration.ArchiveScheduleTime.Second);

                        // skip adding schedule if date has already passed
                        if (scheduleWeekDate.Date >= currentDay.Date)
                        {
                            // skip adding date if it doesnot fall in month being processed
                            // ex: fifth sunday in February will never come
                            if (scheduleWeekDate.Month != nextSchedule.Month)
                                goto SKIP_ADDING_MONTHDAY_WEEKDAY_SCHEDULE;

                            // skip adding today's schedule if schedule time has already passed.
                            if (scheduleWeekDate.Date == currentDay.Date && currentDay.TimeOfDay > configuration.ArchiveScheduleTime.TimeOfDay)
                                goto SKIP_ADDING_MONTHDAY_WEEKDAY_SCHEDULE;
                            
                            schedulePlan.Add(scheduleWeekDate);
                        }

                    SKIP_ADDING_MONTHDAY_WEEKDAY_SCHEDULE:

                        // calculate next date of recurring month
                        nextSchedule = calendar.AddMonths(nextSchedule, configuration.ArchiveScheduleRepetition);
                    }
                }
                break;

            }

            return schedulePlan.ToArray();
        }

        /// <summary>
        /// This method selects nearest execution schedule DateTime from provided execution plan.
        /// </summary>
        /// <param name="executionPlanDates">List of execution plan dates.</param>
        /// <param name="referenceDate">DateTime to take as reference for nearest execution schedule calculation.</param>
        /// <returns></returns>
        private DateTime PickNextExecutionSchedule(DateTime[] executionPlanDates, DateTime referenceDate)
        {
            DateTime nextExecutionSchedule = DateTime.MinValue;
            DateTime currentDay = DateTime.Now;

            // pick nearest execution plan DateTime
            for (int index = 0; index < executionPlanDates.Length; index += 1)
            {
                // pick first date equal to reference date or greter
                if (executionPlanDates[index].Date >= referenceDate.Date)
                {
                    nextExecutionSchedule = executionPlanDates[index];

                    // skip referenceDate's schedule if its today and schedule time has already passed.
                    if (nextExecutionSchedule.Date == currentDay.Date && nextExecutionSchedule.TimeOfDay < currentDay.TimeOfDay)
                    {
                        nextExecutionSchedule = DateTime.MinValue;
                        continue;
                    }

                    break;
                }
            }

            return nextExecutionSchedule;
        }

        private void TriggerArchiveProcess()
        {
            try
            {
                _nextExecutionSchedule = PickNextExecutionSchedule(_executionPlanDates, DateTime.Now);
                while (_isRunning)
                {
                    // sleep till next execution schedule
                    TimeSpan sleepFor = _nextExecutionSchedule - DateTime.Now;

                    // request archiving when wait time is smaller than 5 milliseconds (threshold)
                    // execution thread will wait till its completion
                    if (sleepFor.TotalMilliseconds < 5)
                    {
                        RequestSynchronousArchive(SQLcomplianceConfiguration.GetConfiguration(), _nextExecutionSchedule);
                        _nextExecutionSchedule = PickNextExecutionSchedule(_executionPlanDates, DateTime.Now);
                    }
                    else
                    {
                        // Bug: Due to .NET framework bug, we can't sleep for time (milliseconds) more than max value of integer
                        // Fix: when timespan (milliseconds) exceeds max value of integer, sleep till max value of integer
                        if (sleepFor.TotalMilliseconds > Int32.MaxValue)
                            sleepFor = new TimeSpan(0, 0, 0, 0, Int32.MaxValue);

                        Thread.Sleep(sleepFor);
                    }
                }
            }
            catch (ThreadAbortException abortException)
            {
                // reset states to be on safe side
                _isRunning = false;

                // do nothing
                // we intentionally aborted
            }
        }

        private void RequestSynchronousArchive(SQLcomplianceConfiguration configuration, DateTime archiveTime)
        {
            string errMsg1 = string.Empty;
            string errMsg2 = string.Empty;

            Repository repository = new Repository();
            repository.OpenConnection();

            try
            {
                // set archive running
                SetIsScheduledArchiveRunning(true);

                RemoteCollector srv = CoreRemoteObjectsProvider.RemoteCollector(configuration.Server, configuration.ServerPort);
                
                // fake impersonation
                WindowsIdentity id = WindowsIdentity.GetCurrent();

                ErrorLog.Instance.Write(ErrorLog.Level.Debug, string.Format("Running scheduled archive. Archive Time: {0}", archiveTime.ToString(CultureInfo.CurrentUICulture)), ErrorLog.Severity.Informational);

                ArchiveSettings settings = new ArchiveSettings();
                settings.TargetInstance = string.Empty;
                settings.User = id.Name;
                settings.Background = false; // wait for archiving process to complete
                srv.Archive(settings);

                // set archive not running
                SetIsScheduledArchiveRunning(false);
            }
            catch (SocketException socketEx)
            {
                if (socketEx.ErrorCode == 10061)
                {
                    errMsg1 = String.Format("The {0} on {1} cannot be reached. The {0} service may be down or a network error is preventing the Management Console from contacting the {0}. Your request may not be processed at this time.",
                                             "Collection Service",
                                             configuration.Server);
                }
                else
                {
                    errMsg1 = "An error occurred performing the archive operation.";
                    errMsg2 = socketEx.Message;
                }

                ErrorLog.Instance.Write(ErrorLog.Level.Debug, string.Format("{0}{1}{2}", errMsg1, Environment.NewLine, errMsg2), socketEx, true);
                SetIsScheduledArchiveRunning(false);
            }
            catch (Exception ex)
            {
                errMsg1 = "An error occurred performing the archive operation.";

                ErrorLog.Instance.Write(ErrorLog.Level.Debug, errMsg1, ex, true);
                SetIsScheduledArchiveRunning(false);
            }
        }

        #endregion

    }
}
