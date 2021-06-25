using Idera.SQLcompliance.Core.Agent;
using SQLcomplianceCwfAddin.Helpers;
using SQLcomplianceCwfAddin.Helpers.Agent;
using SQLcomplianceCwfAddin.Helpers.SQL;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AgentProperties;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.UpgradeAgent;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    public partial class RestService
    {
        public CheckAgentStatusResponse CheckAgentStatus(CheckAgentStatusRequest request)
        {
            using (_logger.InfoCall("CheckAgentStatus"))
            {
                var query = QueryBuilder.Instance.GetAgentConnectionDetails(request.Instance);
                var result = QueryExecutor.Instance.GetAgentConnectionDetails(query, GetConnection());

                bool status = AgentManagerHelper.Instance.PingAgent(result.AgentServer, result.AgentPort);
                using (var connection = GetConnection())
                {
                    ServerRecord.UpdateLastAgentContact(connection, request.Instance);
                }

                return new CheckAgentStatusResponse()
                {
                    IsActive = status,
                    AgentServer = result.AgentServer,
                };
            }
        }

        public UpgradeAgentResponse UpgradeAgent(UpgradeAgentRequest upgradeAgentRequest)
        {
            using (_logger.InfoCall("UpgradeAgent"))
            {
                using (var connection = GetConnection())
                {
                    UpgradeAgentResponse response = AgentManagerHelper.Instance.UpgradeAgent(upgradeAgentRequest, connection);
                    return response;
                }
            }
        }

        public AgentProperties GetAgentProperties(int sereverId)
        {
            using (_logger.InfoCall("GetAgentProperties"))
            {
                using (var connection = GetConnection())
                {
                    return AgentManagerHelper.Instance.GetAgentProperties(sereverId, connection);
                }
            }
        }

        public bool UpdateAgentProperties(AgentProperties agentProperties)
        {
            using (_logger.InfoCall("UpdateAgentProperties"))
            {
                using (var connection = GetConnection())
                {
                    return AgentManagerHelper.Instance.UpdateAgentProperties(agentProperties, connection);
                }
            }
        }
    }
}
