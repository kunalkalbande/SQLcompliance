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
        private static bool IsDev = true;
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
            if (IsDev)
            {
                ((AlertsNotification)ServicesToRun[0]).RunFromConsole();
            }
            else
            {
              
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
