using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Status;

namespace Idera.SQLcompliance.Core.Remoting
{
    public static class CoreRemoteObjectsProvider
    {
        public static void InitializeChannel()
        {
            RemoteObjectProviderBase.InitializeChannel();
        }

        public static AgentManager AgentManager(string server, int serverPort)
        {
            return RemoteObjectProviderBase.GetObject<AgentManager>(server, serverPort);
        }

        public static RemoteCollector RemoteCollector(string server, int serverPort)
        {
            return RemoteObjectProviderBase.GetObject<RemoteCollector>(server, serverPort);
        }

        public static AgentCommand AgentCommand(string agentServer, int agentPort)
        {
            return RemoteObjectProviderBase.GetObject<AgentCommand>(agentServer, agentPort);
        }

        public static RemoteAuditManager RemoteAuditManager(string server, int serverPort)
        {
            return RemoteObjectProviderBase.GetObject<RemoteAuditManager>(server, serverPort);
        }

        public static RemoteStatusLogger RemoteStatusLogger(string collectionServer, int collectionServerPort)
        {
            return RemoteObjectProviderBase.GetObject<RemoteStatusLogger>(collectionServer, collectionServerPort);
        }

        public static RemoteStatusLogger RemoteStatusLogger(string collectionServer, int collectionServerPort, string collectionServerName)
        {
            return RemoteObjectProviderBase.GetObject<RemoteStatusLogger>(collectionServer, collectionServerPort, collectionServerName);
        }
    }
}
