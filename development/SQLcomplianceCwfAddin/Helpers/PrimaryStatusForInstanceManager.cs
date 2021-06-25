using System;
using System.Collections.Concurrent;
using System.Threading;
using TracerX;

namespace SQLcomplianceCwfAddin.Helpers
{
    public class PrimaryStatusForInstanceManager
    {
        private readonly Logger _logger;

        private static PrimaryStatusForInstanceManager _instance;
        private static readonly ConcurrentDictionary<string, Timer> OfflineServers = new ConcurrentDictionary<string, Timer>();
        private static readonly ConcurrentDictionary<string, int> ProblematicServers = new ConcurrentDictionary<string, int>();

        private const int RunIntervalMinutes = 5;
        private const int MaxRunIntervalMinutes = 360;
        private static readonly TimeSpan NewerRepeat = TimeSpan.FromMilliseconds(-1);

        private PrimaryStatusForInstanceManager()
        {
            _logger = Logger.GetLogger("PrimaryStatusForInstanceManager");
        }

        public static PrimaryStatusForInstanceManager Instance
        {
            get { return _instance ?? (_instance = new PrimaryStatusForInstanceManager()); }
        }

        public bool IsStatusCanBeUpdated(string server)
        {
            return !OfflineServers.ContainsKey(server);
        }

        public void NoteServerIsOffline(string server)
        {
            try
            {
                OfflineServers.AddOrUpdate(server, CreateTimer(s => RemoveStatus(server), server),
                    (sk, oldTimer) =>
                    {
                        oldTimer.Change(TimeSpan.FromMinutes(CalculateRunInterval(server)), NewerRepeat);
                        return oldTimer;
                    });
                IncreaseServerProblematicLevel(server);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }


        public void NoteServerIsOnline(string instance)
        {
            DecreaseServerProblematicLevel(instance);
        }

        private Timer CreateTimer(TimerCallback callback, string server)
        {
            var runIntervalMinutesForLevel = CalculateRunInterval(server);
            if (MaxRunIntervalMinutes < runIntervalMinutesForLevel) runIntervalMinutesForLevel = MaxRunIntervalMinutes;

            return new Timer(callback, null, TimeSpan.FromMinutes(runIntervalMinutesForLevel), NewerRepeat);
        }

        private static double CalculateRunInterval(string server)
        {
            int serverProblematicLevel;
            if (!ProblematicServers.TryGetValue(server, out serverProblematicLevel))
            {
                serverProblematicLevel = 1;
            }

            var runIntervalMinutesForLevel = RunIntervalMinutes * serverProblematicLevel;
            return runIntervalMinutesForLevel;
        }

        private void IncreaseServerProblematicLevel(string server)
        {
            ProblematicServers.AddOrUpdate(server, 1, (s, oldLevel) => oldLevel < 100 ? oldLevel + 1 : 100);
        }

        private void DecreaseServerProblematicLevel(string server)
        {
            ProblematicServers.AddOrUpdate(server, 1, (s, oldLevel) => oldLevel > 0 ? oldLevel - 1 : 0);
        }

        private void RemoveStatus(string server)
        {
            try
            {
                Timer timerValue;
                if (OfflineServers.TryRemove(server, out timerValue))
                {
                    timerValue.Dispose();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }
    }
}
