using Idera.SQLcompliance.Core.Agent;
using SQLcomplianceCwfAddin.Helpers.Agent;

namespace SQLcomplianceCwfAddin.Helpers
{
    public class CheckSupportsHelper
    {
        // Returns true if the agent is able to support BeforeAfter data collection (3.1 and beyond)
        public static bool SupportsBeforeAfter(ServerRecord server)
        {
            if (server == null ||
                string.IsNullOrEmpty(server.AgentVersion) ||
                server.AgentVersion.StartsWith(Agents.VERSION_1) ||
                server.AgentVersion.StartsWith(Agents.VERSION_2) ||
                server.AgentVersion.StartsWith(Agents.VERSION_3))
            {
                return false;
            }

            return true;
        }

        public static bool SupportsSchemas(ServerRecord server)
        {
            if (server == null ||
                server.SqlVersion < 9)
            {
                return false;
            }

            return SupportsBeforeAfter(server);
        }

        // Returns true if the agent is able to support trusted users (3.0 and beyond)
        public static bool SupportsTrustedUsers(ServerRecord server)
        {
            if (server == null ||
                string.IsNullOrEmpty(server.AgentVersion) ||
                server.AgentVersion.StartsWith(Agents.VERSION_1) ||
                server.AgentVersion.StartsWith(Agents.VERSION_2))
            {
                return false;
            }

            return true;
        }

        public static bool SupportsSensitiveColumns(ServerRecord server)
        {
            if (server == null ||
                string.IsNullOrEmpty(server.AgentVersion) ||
                server.AgentVersion.StartsWith(Agents.VERSION_1) ||
                server.AgentVersion.StartsWith(Agents.VERSION_2) ||
                server.AgentVersion.StartsWith(Agents.VERSION_3) ||
                server.AgentVersion.StartsWith(Agents.VERSION_3_1) ||
                server.AgentVersion.StartsWith(Agents.VERSION_3_2) ||
                server.AgentVersion.StartsWith(Agents.VERSION_3_3))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
