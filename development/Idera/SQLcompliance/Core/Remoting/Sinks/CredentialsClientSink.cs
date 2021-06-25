using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using Idera.SQLcompliance.Core.Remoting.Sinks.Base;

namespace Idera.SQLcompliance.Core.Remoting.Sinks
{
    public class CredentialsClientSink : BaseCustomSink
    {
        private readonly RemotingUser _currentCredentials;

        public CredentialsClientSink(ClientSinkCreationData data)
        {
            _currentCredentials = new RemotingUser { Name = "RemotingUser", Password = "RemotingPassword" };
        }

        protected override void ProcessRequest(IMessage message, ITransportHeaders headers, ref Stream stream, ref object state)
        {
            message.Properties["Credentials"] = _currentCredentials;
        }
    }
}
