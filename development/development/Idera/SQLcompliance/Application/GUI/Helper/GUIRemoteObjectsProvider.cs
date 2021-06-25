using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Remoting;
using ManagementConsoleRequest = Idera.SQLcompliance.Core.Collector.ManagementConsoleRequest;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
    public static class GUIRemoteObjectsProvider
    {
        private static string CmServer { get { return CurrentServer(Globals.Repository.Connection); } }
        private static int CmPort { get { return Globals.SQLcomplianceConfig.ServerPort; } }

        public static void InitializeChannel()
        {
            RemoteObjectProviderBase.InitializeChannel();
        }

        public static ManagementConsoleRequest CollectorManagementConsoleRequest()
        {
            return RemoteObjectProviderBase.GetObject<ManagementConsoleRequest>(CmServer, CmPort);
        }

        public static Core.Agent.ManagementConsoleRequest AgentManagementConsoleRequest(string agentServer, int port)
        {
            return RemoteObjectProviderBase.GetObject<Core.Agent.ManagementConsoleRequest>(agentServer, port);
        }

        private static string GetCurrentServer()
        {
            string currentServer = "SELECT server " + "FROM SQLcompliance..Configuration";

            return currentServer;
        }

        public static string CurrentServer(SqlConnection conn)
        {
            string cmdString = "";
            string server = "";
            
            try
            {
                cmdString = GetCurrentServer();

                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            server = SQLHelpers.GetString(reader, 0);
                            Globals.SQLcomplianceConfig.Server = server;
                        }
                        else
                        {
                            server = Globals.SQLcomplianceConfig.Server;
                        }
                    }
                }
                return server;
            }
            catch 
            {
                server = Globals.SQLcomplianceConfig.Server;
                return server;
            }
        }

        public static AgentManager AgentManager()
        {
            AgentManager agentManager = RemoteObjectProviderBase.GetObject<AgentManager>(CmServer, CmPort);
            return agentManager;
        }
         
        public static RemoteCollector RemoteCollector()
        {
            return RemoteObjectProviderBase.GetObject<RemoteCollector>(CmServer, CmPort);
        }

        public static ServerManager ServerManager()
        {
            return RemoteObjectProviderBase.GetObject<ServerManager>(CmServer, CmPort);
        }

        public static AgentCommand AgentCommand(string agentServer, int agentPort)
        {
            return RemoteObjectProviderBase.GetObject<AgentCommand>(agentServer, agentPort);
        }

    }
}
