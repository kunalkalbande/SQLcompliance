using System;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace SQLcomplianceCwfAddin.Errors
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
            // SQLCM-3749 LM error fix
            if (error is LicenseManagerException)
            {
                string error_to_publish = error.Message;
                fault = Message.CreateMessage(version, "", error_to_publish, new DataContractJsonSerializer(error_to_publish.GetType()));

                // tell WCF to use JSON encoding rather than default XML
                fault.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Json));
                if (((LicenseManagerException)error).IsAuthenticationFailed)
                {
                    rmp.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                }
                else
                    rmp.StatusCode = System.Net.HttpStatusCode.ExpectationFailed;

                // put appropraite description here..
                rmp.StatusDescription = error.Message.Replace("\n", "&#10;").Replace("\r", "&#13;");
                // rmp.StatusDescription = error.Message + "  See fault object for more information.";
                fault.Properties.Add(HttpResponseMessageProperty.Name, rmp);
                return;
            }
            // SQLCM-3749 LM error fix
            if (error is FaultException)
            {
                var error_to_publish = new JsonExceptionWrapper(error);
                try
                {
                    // extract the our FaultContract object from the exception object.
                    var pInfo = error.GetType().GetProperty("Detail");
                    var detail = string.Empty;
                    if (pInfo != null)
                    {
                        detail = Convert.ToString(pInfo.GetGetMethod().Invoke(error, null));
                    }
                    else
                    {
                        detail = error.Message;
                    }


                    // create a fault message containing our FaultContract object
                    fault = Message.CreateMessage(version, "", error_to_publish,
                        new DataContractJsonSerializer(error_to_publish.GetType()));

                    // tell WCF to use JSON encoding rather than default XML
                    fault.Properties.Add(WebBodyFormatMessageProperty.Name,
                        new WebBodyFormatMessageProperty(WebContentFormat.Json));

                    rmp.StatusCode = HttpStatusCode.BadRequest;

                    // put appropraite description here..
                    rmp.StatusDescription = error.Message + "  See fault object for more information.";
                    fault.Properties.Add(HttpResponseMessageProperty.Name, rmp);
                    return;
                }
                catch
                {
                }
            }

            fault = Message.CreateMessage(version, "", new JsonExceptionWrapper(error),
                new DataContractJsonSerializer(typeof (JsonExceptionWrapper)));

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