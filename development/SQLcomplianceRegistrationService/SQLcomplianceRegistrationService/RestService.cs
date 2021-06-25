using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceProcess;
using System.Text;
using SQLcomplianceRegistrationService.Properties;
using TracerX;

namespace SQLcomplianceRegistrationService
{
    public partial class RestService : ServiceBase
    {
        private WebServiceHost _busHost;
        private static readonly Logger LogX = Logger.GetLogger("SQLcomplianceRegistrationServiceLogger");
        public RestService()
        {
            InitializeComponent();
            if (!System.Diagnostics.EventLog.SourceExists("SQLcomplianceRegistrationSVC"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "SQLcomplianceRegistrationSVC", "SQLcomplianceRegistrationServiceLog");
            }
            eventLog1.Source = "SQLcomplianceRegistrationSVC";
            eventLog1.Log = "SQLcomplianceRegistrationServiceLog";
        }

        protected void LoadProperties()
        {
            try
            {
                Properties.Settings.Default.Reload();

                Constants.CoreServicesUrl = Properties.Settings.Default.CoreServicesUrl;
                Constants.DashboardHost = Constants.CoreServicesUrl.Split('/')[2].Split(':')[0];
                Constants.DashboardPort = Constants.CoreServicesUrl.Split('/')[2].Split(':')[1];
                Constants.ServicePort = Properties.Settings.Default.ServicePort;
                Constants.IsRegistered = Properties.Settings.Default.IsRegistered;
                Constants.IsTaggable = Convert.ToInt32(Properties.Settings.Default.IsTaggingEnabled);
                Constants.ProductId = Properties.Settings.Default.ProductId;             
                Constants.SQLcmRepository = Properties.Settings.Default.SQLcmRepository;
                RegistryHelper.SetValueInRegistry("ServiceHost", Environment.MachineName);
                RegistryHelper.SetValueInRegistry("ServicePort", Constants.ServicePort);

                System.Management.SelectQuery sQuery = new System.Management.SelectQuery(string.Format("select startname from Win32_Service where name = 'SQLcomplianceRegistrationService'")); // where name = '{0}'", "MCShield.exe"));
                using (System.Management.ManagementObjectSearcher mgmtSearcher = new System.Management.ManagementObjectSearcher(sQuery))
                {
                    foreach (System.Management.ManagementObject service in mgmtSearcher.Get())
                    {
                        Constants.ServiceAdminUser = service["startname"].ToString();
                        string[] loginDetail = Constants.ServiceAdminUser.Split('\\');
                        if (loginDetail[0] == ".")
                            Constants.ServiceAdminUser = Environment.MachineName + "\\" + loginDetail[1];
                    }
                }

                RegistryHelper.SetValueInRegistry("Administrator", Constants.ServiceAdminUser);
            }
            catch (Exception e)
            {
                LogX.ErrorFormat("Error While Loading Properties: {0}", e.Message);
                
            }
            
        }
        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("In OnStart");
            LogX.Info("In Start Method");
            LoadProperties();
            StartWebServices("http://localhost:" + Settings.Default.ServicePort + "/SQLCM/");
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("In OnStop");
            LogX.Info("In Stop Method");
        }

        protected override void OnShutdown()
        {
            eventLog1.WriteEntry("In OnShutdown");
            LogX.Info("In Shutdown Method Method");
        }

        internal void RunFromConsole()
        {

            Console.WriteLine("\nService is running.  Press Ctrl-C to stop service.");
            OnStart(null);

            Console.ReadKey();

            OnStop();

        }

        void StartWebServices(string urlAuthority)
        {

            _busHost = new WebServiceHost(typeof(SQLcmProduct), new Uri(urlAuthority + ""));
            var busBinding = new WebHttpBinding(WebHttpSecurityMode.TransportCredentialOnly);
            var busEndpoint = _busHost.AddServiceEndpoint(typeof(IProduct), busBinding, urlAuthority + "");
            busEndpoint.Behaviors.Add(new SQLcomplianceRegistrationService.Errors.ErrorHandlerBehavior());
            busEndpoint.Behaviors.Add(new SQLcomplianceRegistrationService.Helpers.WebHttpBehaviorEx { HelpEnabled = true });
            _busHost.Open();
            LogX.InfoFormat("Core services URL: {0}", Constants.CoreServicesUrl);
        }
    }
}
