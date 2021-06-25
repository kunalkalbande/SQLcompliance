using System;
using Idera.SQLcompliance.Core.Remoting;

namespace SQLcomplianceCwfAddin.Helpers
{
    public class ProxyObjecHelper
    {
        public static T CreateProxyObject<T>(string server, int port)
        {
            string objectName = string.Empty;

            try
            {
                objectName = typeof(T).Name;
                var proxyObject = RemoteObjectProviderBase.GetObject<T>(server, port);
                return proxyObject;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    string.Format("Failed to create {0} proxy object: \n Server: {1}, \n Server Port: {2}.\n Error: {3}",
                                    objectName,
                                    server, 
                                    port, 
                                    ex));
            }
        }
    }
}
