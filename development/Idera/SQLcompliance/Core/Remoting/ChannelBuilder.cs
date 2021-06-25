using System;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Idera.SQLcompliance.Core.Remoting.Sinks;
using Idera.SQLcompliance.Core.Remoting.Sinks.Base;

namespace Idera.SQLcompliance.Core.Remoting
{
    public class ChannelBuilder
    {
        private const string CustomSinkTypePropertyName = "customSinkType";
        private const string NamePropertyName = "name";
        private const string PortPropertyName = "port";

        public static TcpServerChannel GetRegisteredServerChannel(string name, int port)
        {
            var props = new Hashtable();
            props[NamePropertyName] = name;
            props[PortPropertyName] = port;

            return GetRegisteredServerChannel(props);
        }

        public static TcpServerChannel GetRegisteredServerChannel(IDictionary props)
        {
            LogServerChannelRegistration(props);

            var tcpChannel = CreateServerChannel(props);
            try
            {
                ChannelServices.RegisterChannel(tcpChannel, false);
            }
            catch (Exception e)
            {
                LogServerChannelRegistrationFail(props, e);
            }
            return tcpChannel;
        }

        public static void RegisterClientChannel()
        {
            LocClientChannelRegistration();

            var channel = CreateClientChannel();
            try
            {
                ChannelServices.RegisterChannel(channel, false);
            }
            catch (Exception e)
            {
                LogClientChannelRegistrationFail(e);
            }
        }

        private static void LogServerChannelRegistrationFail(IDictionary props, Exception e)
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Always,
                String.Format("Fail to register server channel {0} on {1} port. {2}", props[NamePropertyName],
                    props[PortPropertyName], e.Message),
                ErrorLog.Severity.Error);
        }

        private static void LogServerChannelRegistration(IDictionary props)
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Always,
                String.Format("Registering server channel {0} on {1} port", props[NamePropertyName], props[PortPropertyName]),
                ErrorLog.Severity.Informational);
        }

        private static TcpServerChannel CreateServerChannel(IDictionary props)
        {
            props[CustomSinkTypePropertyName] = GetServerAuthSincType();

            var serverProvider = new BinaryServerFormatterSinkProvider
            {
                Next = new CustomServerSinkProvider(props, new object[0])
            };

            return new TcpServerChannel(props, serverProvider);
        }

        private static void LogClientChannelRegistrationFail(Exception e)
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Always,
                String.Format("Fail to register client channel. {0}", e.Message),
                ErrorLog.Severity.Error);
        }

        private static void LocClientChannelRegistration()
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Always, "Registering client channel.", ErrorLog.Severity.Informational);
        }

        private static TcpClientChannel CreateClientChannel()
        {
            var props = new Hashtable();
            props[CustomSinkTypePropertyName] = GetClientAuthSincType();
            //props.Add("tokenImpersonationLevel", "impersonation");

            var clientProvider = new CustomClientSinkProvider(props, new object[0])
            {
                Next = new BinaryClientFormatterSinkProvider()
            };

            return new TcpClientChannel(props, clientProvider);
        }

        private static string GetClientAuthSincType()
        {
            return string.Format("{0}, {1}", typeof(CredentialsClientSink).FullName, typeof(CredentialsClientSink).Assembly.GetName().Name);
        }

        private static string GetServerAuthSincType()
        {
            return string.Format("{0}, {1}", typeof(CredentialsServerSink).FullName, typeof(CredentialsServerSink).Assembly.GetName().Name);
        }
    }
}
