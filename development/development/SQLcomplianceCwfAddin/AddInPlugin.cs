using System.AddIn;
using PluginAddInView;
using SQLcomplianceCwfAddin.RestService;
using TracerX;

namespace SQLcomplianceCwfAddin
{
    [AddIn(RestServiceConstants.ProductName, Version = RestServiceConstants.ProductVersion)]
    public class AddInPlugin: IPlugin
    {
        private readonly Logger _logger;
        
        public AddInPlugin()
        {
            Logger.DefaultBinaryFile.Name = "SQLcomplianceCwfAddin.tx1";
            Logger.DefaultBinaryFile.Directory = "%EXEDIR%\\Logs";
            Logger.DefaultBinaryFile.MaxSizeMb = 10;
            Logger.DefaultBinaryFile.Archives = 3;
            Logger.DefaultBinaryFile.CircularStartSizeKb = 250;
            Logger.DefaultBinaryFile.CircularStartDelaySeconds = 90;
            Logger.DefaultBinaryFile.AddToListOfRecentlyCreatedFiles = true;
            Logger.DefaultBinaryFile.Open();

            _logger = Logger.GetLogger("AddInPlugin");
            _logger.BinaryFileTraceLevel = TraceLevel.Info;
        }

        #region AddIn methods

        public void Initialize(HostObject hostObj)
        {
            using (_logger.InfoCall("Initialize"))
            {
                 RestServiceHost.Instance.InitializeHost(hostObj);
                 if (RestServiceHost.Instance == null) { _logger.Info("Rest ServiceHost Null Check1"); }
            }
        }  

        public void StartREST(string productId, string urlAuthority)
        {
            using (_logger.InfoCall("StartREST"))
            {
                _logger.InfoFormat("Starting REST service.\r\nProduct ID: {0}\r\nURL Authority: {1}", productId, urlAuthority);
                if (RestServiceHost.Instance == null) { _logger.Info("Rest ServiceHost Null Check"); }
                RestServiceHost.Instance.StartRestService(productId, urlAuthority);
            }
        }

        public void StopREST(string productId)
        {
            using (_logger.InfoCall("StopREST"))
            {
                _logger.InfoFormat("Stopping REST service.\r\nProduct ID: {0}", productId);
                RestServiceHost.Instance.StopRestService(productId);
            }
        }

        public string getRESTUrl(string productId)
        {
            using (_logger.InfoCall("getRESTUrl"))
            {
                _logger.InfoFormat("REST service URL: {0}", RestServiceConstants.RestUrl);
                return RestServiceConstants.RestUrl;
            }
        }

        public string getWebUrl(string productId)
        {
            using (_logger.InfoCall("getWebUrl"))
            {
                _logger.InfoFormat("Web service URL: {0}", RestServiceConstants.WebUrl);
                return RestServiceConstants.WebUrl;
            }
        }

        #endregion
    }
}
