using Idera.SQLcompliance.Core.EmailSummaryNotification;
using System.ServiceProcess;
using System.Timers;

namespace SQLComplianceAlertsService
{
    public partial class AlertsNotification : ServiceBase
    {
        private static SQLComplianceEmailSummaryNotification emailSummaryNotification = SQLComplianceEmailSummaryNotification.Instance;

        public AlertsNotification()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Timer _timer = new Timer(2 * 60 * 1000); // every 2 minutes
            _timer.Elapsed += new ElapsedEventHandler(OnTimer);
            _timer.Start();
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            emailSummaryNotification.Start();
        }

        protected override void OnStop()
        {
        }

        private void StartAlertNotifications()
        {
            emailSummaryNotification.Start();
        }
    }
}
