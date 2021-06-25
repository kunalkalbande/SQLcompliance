using System;

namespace Idera.SQLcompliance.Core.Remoting
{
    public static class RemoteObjectProviderBase
    {
        private static bool _isChannelInitialized;

        public static void InitializeChannel()
        {
            if (_isChannelInitialized) return;
            try
            {
                ChannelBuilder.RegisterClientChannel();
                _isChannelInitialized = true;
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Always,
                                     String.Format(CoreConstants.Exception_FailToInitializeRemotingChannel,
                                     e.Message),
                                     ErrorLog.Severity.Error);
            }
        }

        public static T GetObject<T>(string url)
        {
            InitializeChannel();
            return (T)Activator.GetObject(typeof(T), url);
        }

        public static T GetObject<T>(string server, int serverPort)
        {
            return GetObject<T>(EndPointUrlBuilder.GetUrl(typeof(T), server, serverPort));
        }

        public static T GetObject<T>(string server, int serverPort, string serverName)
        {
            return GetObject<T>(EndPointUrlBuilder.GetUrl(serverName, server, serverPort));
        }
    }
}
