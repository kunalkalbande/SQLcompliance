using System;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace SQLcomplianceRegistrationService.Errors
{
    internal class ErrorHandler : IErrorHandler
    {
        private readonly TracerX.Logger _logger = TracerX.Logger.GetLogger("ErrorHandler");

        public bool HandleError(Exception x)
        {
            using (_logger.ErrorCall(string.Format("HandleError({0})", null == x ? "null exception!" : x.Message)))
            {
                _logger.Error("Exception:", x);
                return true;
            }
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
                var rmp = new HttpResponseMessageProperty();               

                fault.Properties.Add(WebBodyFormatMessageProperty.Name,
                    new WebBodyFormatMessageProperty(WebContentFormat.Json));

                // return custom error code.
                rmp = new HttpResponseMessageProperty();
                rmp.StatusCode = HttpStatusCode.InternalServerError;

                // put appropraite description here..
                rmp.StatusDescription = error.Message.Replace("\n", "&#10;").Replace("\r", "&#13;");

                fault.Properties.Add(HttpResponseMessageProperty.Name, rmp);
        }
    }
}