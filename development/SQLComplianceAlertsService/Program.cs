using Idera.SQLcompliance.Core.EmailSummaryNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SQLComplianceAlertsService
{
    static class Program
    {

        private static SQLComplianceEmailSummaryNotification emailSummaryNotification;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
            new AlertsNotification()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
