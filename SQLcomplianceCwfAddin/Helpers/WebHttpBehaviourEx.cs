using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using SQLcomplianceCwfAddin.Errors;

namespace SQLcomplianceCwfAddin.Helpers
{
    public class WebHttpBehaviorEx : WebHttpBehavior
    {
        protected override void AddServerErrorHandlers(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.ChannelDispatcher.ErrorHandlers.Clear();
            endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(new ErrorHandler());
        }
    }
}
