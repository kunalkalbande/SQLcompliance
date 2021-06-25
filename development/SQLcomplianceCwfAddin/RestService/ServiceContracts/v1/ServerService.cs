using System.Collections.Generic;
using SQLcomplianceCwfAddin.Helpers;
using SQLcomplianceCwfAddin.Helpers.Agent;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddServer;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.RemoveServers;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ServerProperties;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    public partial class RestService
    {
        public List<AuditedServerInfo> GetAuditedInstances()
        {
            using (_logger.InfoCall("GetAuditedInstances"))
            {
                using (var connection = GetConnection())
                {
                bool auditServersOnly = false;
                var serverProperties = AuditServerHelper.Instance.GetAuditedInstances(auditServersOnly, connection);
                return serverProperties;
            } 
        }
        }

        public List<string> GetAllNotRegisteredInstanceNameList()
        {
            using (_logger.InfoCall("GetAllNotRegisteredInstanceNameList"))
            {
                using (var connection = GetConnection())
                {
                var serverProperties = AuditServerHelper.Instance.GetAllNotRegisteredInstanceNameList(connection);
                return serverProperties;
            }            
        }
        }

        public List<AddServerStatus> AddServers(List<AuditServerSettings> serverSettingsList)
        {
            using (_logger.InfoCall("AddServers"))
            {
                using (var connection = GetConnection())
                {
                    var result = AuditServerHelper.Instance.AddServers(serverSettingsList, connection);
                    PushInstancesToCwfDashboard();
                    return result;
                }
            }
        }

        public List<AddServerStatus> ImportServerInstances(ImportInstanceListRequest importInstanceListRequest)
        {
            using (_logger.InfoCall("ImportServerInstances"))
            {
                using (var connection = GetConnection())
                {
                    return AuditServerHelper.Instance.ImportServerInstances(importInstanceListRequest.InstanceList, connection);
                }
            }
        }

        public List<RemoveServerStatus> RemoveServers(RemoveServersRequest removeServersRequest)
        {
            using (_logger.InfoCall("RemoveServers"))
            {
                using (var connection = GetConnection())
                {
                return AuditServerHelper.Instance.RemoveServers(removeServersRequest, connection);
            }
        }
        }

        public void EnableAuditingForServers(EnableAuditForServers servers)
        {
            using (_logger.InfoCall("EnableAuditingForServer"))
            {
                using (var connection = GetConnection())
                {
                AuditServerHelper.Instance.EnableAuditingForServers(servers, connection);
            }
        }
        }

        public AuditServerProperties GetAuditServerProperties(int serverId)
        {
            using (_logger.InfoCall("GetAuditServerProperties"))
            {
                using (var connection = GetConnection())
                {
                var serverProperties = AuditServerHelper.Instance.GetAuditServerProperties(serverId, connection);
                return serverProperties;
            }
        }
        }

        public AgentDeploymentProperties GetAgentDeploymentPropertiesForInstance(InstanceRequest instanceRequest)
        {
            using (_logger.InfoCall("GetAgentDeploymentPropertiesForInstance"))
            {
                using (var connection = GetConnection())
                {
                    var agentProperties = AuditServerHelper.Instance.GetAgentDeploymentPropertiesForInstance(instanceRequest.Instance, connection);
                    return agentProperties;
                }
            }
        }

        public InstanceAvailableResponse IsInstanceAvailable(InstanceRequest instanceRequest)
        {
            using (_logger.InfoCall("IsInstanceAvailable"))
            {
                using (var connection = GetConnection())
                {
                    var instanceResponse = AuditServerHelper.Instance.IsInstanceAvailable(instanceRequest.Instance,
                        connection);
                    return instanceResponse;
                }
            }            
        }

        public bool UpdateAuditServerProperties(AuditServerProperties serverProperties)
        {
            using (_logger.InfoCall("UpdateAuditServerProperties"))
            {
                using (var connection = GetConnection())
                {
                return AuditServerHelper.Instance.UpdateAuditServerProperties(serverProperties, connection);
            }               
        }
        }

        public bool ValidateCredentials(Credentials credentials)
        {
            using (_logger.InfoCall("ValidateCredentials"))
            {
                return AuditServerHelper.Instance.ValidateCredentials(credentials);
            }              
        }

        public string UpdateAuditConfigurationForServer(UpdateAuditConfigurationRequest request)
        {
            using (_logger.InfoCall("UpdateAuditConfigurationForServer"))
            {
                using (var connection = GetConnection())
                {
                    var serverProperties = AgentManagerHelper.Instance.UpdateAuditConfiguration(request.ServerId, connection); 
                    return serverProperties;
                }
            }
        }

        public InstanceRegisteredStatus CheckInstanceRegisteredStatus(CheckInstanceRegisteredRequest request)
        {
            using (_logger.InfoCall("CheckInstanceRegisteredStatus"))
            {
                using (var connection = GetConnection())
                {
                    return AuditServerHelper.Instance.CheckInstanceRegisteredStatus(request, connection);
                }
            }   
        }

        public List<string> GetAllAuditedInstances()
        {
            using (_logger.InfoCall("GetAllAuditedInstances"))
            {
                using (var connection = GetConnection())
                {
                    List<string> result = AuditServerHelper.Instance.GetAllAuditedInstances(connection);
                    _logger.Info("Total Audited Instances: " + result.Count);
                    return result;
                }
            }
        }
    }
}
