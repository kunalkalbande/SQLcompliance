using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using Idera.SQLcompliance.Core.Remoting.Sinks.Base;

namespace Idera.SQLcompliance.Core.Remoting.Sinks
{
    public class CredentialsServerSink : BaseCustomSink
    {
        private readonly IRemotingAuthorizationManager _authorizationManager;

        public CredentialsServerSink(SinkCreationData data)
        {
            _authorizationManager = new RemotingAuthorizationManager();
        }

        protected override void ProcessRequest(IMessage message, ITransportHeaders headers, ref Stream stream, ref object state)
        {
            var credentials = message.Properties["Credentials"] as RemotingUser;
            if (!_authorizationManager.IsValid(credentials))
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Always, "Incoming request with incorrect credentials. Access denied.");
                throw new RemotingException("Invalid credentials.");
            }
        }
    }
}
