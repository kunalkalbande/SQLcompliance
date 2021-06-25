using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using Idera.SQLcompliance.Core.Remoting;
using Idera.SQLcompliance.Core.Rules.Filters;
using PluginAddInView;
using PluginCommon;
using SQLcomplianceCwfAddin.Errors;
using SQLcomplianceCwfAddin.Helpers;
using SQLcomplianceCwfAddin.RestService.ServiceContracts.v1;
using TracerX;
using System.Threading;

namespace SQLcomplianceCwfAddin.RestService
{
    public class RestServiceHost
    {
        #region members

        private HostObject _host;
        private readonly Dictionary<string, WebServiceHost> _webServiceHosts;
        private readonly Logger _logger;

        private static RestServiceHost _instance;

        #endregion

        private RestServiceHost()
        {
            _logger = Logger.GetLogger("RestServiceHost");
            _logger.BinaryFileTraceLevel = TraceLevel.Info;

            _webServiceHosts = new Dictionary<string, WebServiceHost>();
        }

        #region properties

        public static RestServiceHost Instance
        {
            get { return _instance ?? (_instance = new RestServiceHost()); }
        }

        #endregion

        internal void StartRestService(string productId, string urlAuthority)
        {
            _logger.Info("Start Rest Service LM");
            try
            {
                Thread.Sleep(10000);
                StartWebService(productId, urlAuthority);
            }
            catch (Exception e) 
            {
                _logger.Info(e.Message);
            }
            _logger.Info("End Rest Service LM");
        }

        internal void StopRestService(string productId)
        {
            StopWebService(productId);
        }

        internal void InitializeHost(HostObject host)
        {
            if (host == null) { _logger.Info("Check Host Null"); }
            _logger.Info("Setting host object for REST service.");
            _host = host;
        }

        private void StopWebService(string productId)
        {
            using (_logger.InfoCall("StopWebService"))
            {
                WebServiceHost restServiceHost;
                if (!_webServiceHosts.TryGetValue(productId, out restServiceHost))
                {
                    _logger.ErrorFormat("Failed to get web service host for Product ID: {0}", productId);
                    return;
                }

                if (restServiceHost != null &&
                    (restServiceHost.State == CommunicationState.Opened ||
                     restServiceHost.State == CommunicationState.Opening))
                    restServiceHost.Close();
            }
        }

        private void StartWebService(string productId, string urlAuthority)
        {
            _logger.Info("Entered StartWebService");
            using (_logger.InfoCall("StartWebService"))
            {
                try
                {
                    _logger.InfoFormat("Product ID: {0}\r\nURL Authority: {1}", productId, urlAuthority);

                    var restService = new ServiceContracts.v1.RestService(_host);
                    var restServiceHost = new WebServiceHost(restService, new Uri(urlAuthority));
                    _logger.Info("Created service object and web service host object.");

                    var sqlcmBinding = new WebHttpBinding(WebHttpSecurityMode.TransportCredentialOnly);
                    var sqlcmEndpoint = restServiceHost.AddServiceEndpoint(typeof (IRestService), sqlcmBinding, urlAuthority);
                    sqlcmEndpoint.Behaviors.Add(new ErrorHandlerBehavior());
                    sqlcmEndpoint.Behaviors.Add(new WebHttpBehaviorEx() { HelpEnabled = true });
                    restServiceHost.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;
                    _logger.Info("Created binding and service endpoint.");

                    restServiceHost.Open();
                    _logger.Info("Opened service host.");

                    _webServiceHosts.Add(productId, restServiceHost);
                    _logger.Info("Added service host to service hosts list.");

                    PushWidgetsToDashboard(urlAuthority);
                    _logger.Info("Pushed product widgwts to dashboard.");

                    CoreRemoteObjectsProvider.InitializeChannel();
                    InitializeProductSpecificData();
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormat("Failed to start service host.\r\nMessage: {0}\r\nStack Trace: {1}", 
                                        ex.Message,
                                        ex.StackTrace);
                }
            }
        }

        private static void InitializeProductSpecificData()
        {
            var filters = new FiltersConfiguration();
            filters.InitializeEventFields();
        }

        private void PushWidgetsToDashboard(string urlAuthority)
        {
            using (_logger.InfoCall("PushWidgetsToDashboard"))
            {
                var parts = urlAuthority.Split('/');
                var productId = parts[parts.Length - 1];
                var widgets = GetInstallableWidgets(productId);
                if (widgets.Count > 0)
                {
                    _host.AddWidgets(productId, widgets, null);
                }
            }
        }

        private Widgets GetDashboardWidgets(string prodID)
        {
            var allDashboardWidgets = new DashboardWidgets();
            Widgets widgetsList = new Widgets();
            allDashboardWidgets = _host.GetWidgets(new PluginCommon.Authentication.SimplePrincipal(null));
            foreach (DashboardWidget widget in allDashboardWidgets)
            {
                if (widget.Product.Id.ToString() == prodID)
                {
                    Widget currentWidget = new Widget();
                    currentWidget.Collapsed = widget.Collapsed;
                    currentWidget.DataURI = widget.DataURI;
                    currentWidget.DefaultViews = widget.DefaultViews;
                    currentWidget.Description = widget.Description;
                    currentWidget.Name = widget.Name;
                    currentWidget.NavigationLink = widget.NavigationLink;
                    currentWidget.PackageURI = widget.PackageURI;
                    currentWidget.Settings = widget.Settings;
                    currentWidget.Type = widget.Type;
                    currentWidget.Version = widget.Version;

                    widgetsList.Add(currentWidget);
                }
            }
            return widgetsList;
        }

        private Widgets GetInstallableWidgets(string prodId)
        {
            var installerWidgets = GetWidgets();
            var dashboardWidgets = GetDashboardWidgets(prodId);
            var installtableWidgets = new Widgets();

            foreach (Widget widget in installerWidgets)
            {
                if (dashboardWidgets.Find(i => i.Name == widget.Name
                    && i.Type == widget.Type
                    && i.PackageURI == widget.PackageURI
                    && i.DataURI == widget.DataURI
                    && i.Description == widget.Description) == null)
                {
                    installtableWidgets.Add(widget);
                }
            }

            return installtableWidgets;
        }
        private Widgets GetWidgets()
        {
            using (_logger.InfoCall("GetWidgets"))
            {
                _logger.Info("Preparing widgets to push...");
                var widgets = new Widgets();

                var widgetSettings = new Dictionary<string, string>
                {
                    {"Limit", "10"}
                };
                var widget = new Widget
                {
                    Name = "SQLCM ENVIRONMENT ALERTS",
                    Type = "Alert Status",
                    NavigationLink = "/sqlcm/{InstanceName}/",
                    PackageURI = "/sqlcm/widgets/environment-alerts-widget.zul",
                    DataURI = "/GetEnvironmentDetailsForInstancesAndDatabases",
                    Description = "Provides an Overview of the total alerts.",
                    Version = RestServiceConstants.ProductVersion,
                    Settings = widgetSettings,
                    DefaultViews = "Overview"
                };
                
                
                widgets.Add(widget);
                
                var widgetReportCard = new Widget
                {
                    Name = "SQLCM | Enterprise Activity Report Card",
                    Type = "Report Card",
                    NavigationLink = "/sqlcm/{InstanceName}/",
                    PackageURI = "/sqlcm/widgets/instance-detail-widget.zul",
                    DataURI = "/GetStatsData",
                    Description = "Provides an Overview of the Report Card.",
                    Version = RestServiceConstants.ProductVersion,
                    Settings = widgetSettings,
                    DefaultViews = "Overview"
                };


                widgets.Add(widgetReportCard);

                var widgetAuditInstance = new Widget
                {
                    Name = "SQLCM | Audited Instances",
                    Type = "Audited Instances",
                    NavigationLink = "/sqlcm/{InstanceName}/",
                    PackageURI = "/sqlcm/widgets/sqlcm-audited-instances-widget.zul",
                    DataURI = "/GetFilteredAuditedInstancesStatus",
                    Description = "Provides an Overview of the audited instances.",
                    Version = RestServiceConstants.ProductVersion,
                    Settings = widgetSettings,
                    DefaultViews = "Overview"
                };
                widgets.Add(widgetAuditInstance);
                _logger.InfoFormat("{0}. {1}",widgets.Count, widget.Name);

                _logger.InfoFormat("Total {0} widgets prepared to push.", widgets.Count);
                return widgets;
            }
            
        }
    }
}
