using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using TracerX;

namespace SQLcomplianceRegistrationService
{
    static class Program
    {
        private static readonly Logger Log = null;
        private static readonly bool _logFileOpened = InitLogging();
        
        static Program()
        {
            if (_logFileOpened) Log = Logger.GetLogger("SQLcomplianceRegistrationService.Program");
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (!_logFileOpened)
                return;
            Log.Info("Init SQLcomplianceRegistrationService()");
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new RestService() 
            };
            if(Constants.IsDeveloperMode)
                ((RestService)ServicesToRun[0]).RunFromConsole();
            else
                ServiceBase.Run(ServicesToRun);
        }
        private static bool InitLogging()
        {
            try
            {
                // It's best to account most threads.
                Thread.CurrentThread.Name = "Main";

                // Load TracerX configuration from app config.
                Logger.Xml.Configure();

                // Setup event logging.
                //Logger.EventLogging.EventLog = new EventLog("Application", ".", Common.Constants.ProductName);
                //Logger.EventLogging.FormatString = "{msg}";
                //Logger root = Logger.GetLogger("Root"); // all other loggers inherit from Root.
                //root.EventLogTraceLevel = TracerX.TraceLevel.Debug; 

                // Open the log file.
                return (Logger.FileLogging.Open());
            }
            catch { return (false); }
        }
    }
}
