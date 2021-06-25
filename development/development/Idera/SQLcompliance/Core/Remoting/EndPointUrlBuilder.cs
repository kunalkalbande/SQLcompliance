using System;

namespace Idera.SQLcompliance.Core.Remoting
{
    public static class EndPointUrlBuilder
    {
        private const string TcpUrlPattern = "tcp://{0}:{1}/{2}";

        public static string GetUrl(Type type, string server, int port)
        {
            return string.Format(TcpUrlPattern, server, port, type.Name);
        }

        public static string GetUrl(string typeName, string server, int port)
        {
            return string.Format(TcpUrlPattern, server, port, typeName);
        }
    }
}
