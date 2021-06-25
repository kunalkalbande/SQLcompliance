using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.ServiceModel.Web;
using PluginCommon;
using SQLcomplianceCwfAddin.Helpers;
using CWFDataContracts = Idera.SQLcompliance.Core.CWFDataContracts;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    public partial class RestService
    {
        public AuditedDatabase GetAuditedDatabase(int id, int serverId)
        {
            using (_logger.InfoCall("GetAuditedDatabase"))
            {
                var query = QueryBuilder.Instance.GetDatabase(id, serverId);
                var result = QueryExecutor.Instance.GetAuditedDatabase(GetConnection(), query);
                return result;
            }
        }

        public List<string> GetDatabaseByServerName(string serverName)
        {
            using (_logger.InfoCall("GetDatabaseByServerName"))
            {
                _logger.Info("Parameter Server Name: " + serverName);
                string query = "[SQLcompliance]..[sp_cmreport_GetDatabasesByServerName]";
                List<string> result = QueryExecutor.Instance.GetRCCData(GetConnection(), query, serverName);
                _logger.Info("Total Audited Databases: " + result.Count);
                return result;
            }
        }

        public List<string> GetAllAuditSettings()
        {
            using (_logger.InfoCall("GetAllAuditSettings"))
            {
                string query = "[SQLcompliance]..[sp_cmreport_GetAllAuditSettings]";
                List<string> result = QueryExecutor.Instance.GetRCCData(GetConnection(), query);
                _logger.Info("Total Audit Settings: " + result.Count);
                return result;
            }
        }

        public List<string> GetAllRegulationGuidelines()
        {
            using (_logger.InfoCall("GetAllRegulationGuidelines"))
            {
                string query = "[SQLcompliance]..[sp_cmreport_GetAllRegulationGuidelines]";
                List<string> result = QueryExecutor.Instance.GetRCCData(GetConnection(), query);
                _logger.Info("Total Regulation Guidelines: " + result.Count);
                return result;
            }
        }

        public bool PushAlertsToCwfDashboard(List<Alert> alerts)
        {
            using (_logger.InfoCall("PushAlertsToCwfDashboard"))
            {
                if (alerts == null || alerts.Count == 0) return true;

                var prinicpal = GetPrincipalFromRequest();
                var productId = GetProductIdFromrequest(prinicpal);
                _dashboardHost.SynchronizeAlerts(productId, new Alerts(alerts), prinicpal);

                return true;
            }
        }

        public void PushInstancesToCwfDashboard()
        {
            using (_logger.InfoCall("PushInstancesToCwfDashboard"))
            {
                var prinicpal = GetPrincipalFromRequest();
                var productId = GetProductIdFromrequest(prinicpal);

                new InstanceSynchronizationHelper(prinicpal, GetConnection(), productId, _dashboardHost, _logger).SynchronizeAsync();
            }
        }

        private bool UpdateUser(CWFDataContracts.User userToUpdate, 
                                Users cwfUsers, 
                                IPrincipal principal, 
                                int productId, 
                                int userRoleId, 
                                bool isEnabled)
        {
            using (_logger.InfoCall("UpdateUser"))
            {
                foreach (var cwfUser in cwfUsers)
                {
                    if (!cwfUser.Account.Equals(userToUpdate.Account, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    try
                    {
                        var permissionsList = new EditPermissions();
                        var permission = new EditPermission()
                        {
                            RoleID = userRoleId,
                            ProductID = productId
                        };
                        permissionsList.Add(permission);

                        var updatedInfo = new EditUser();
                        updatedInfo.Account = cwfUser.Account;
                        updatedInfo.IsEnabled = isEnabled;
                        updatedInfo.Roles = permissionsList;

                        _dashboardHost.EditUser(updatedInfo, cwfUser.Id.ToString(), principal);
                        _logger.InfoFormat("User {0} updated.", userToUpdate.Account);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.InfoFormat("Failed to update user {0}. Error: {1}", userToUpdate.Account, ex);
                        return false;
                    }
                }

                return false;
            }
            
        }

        public bool SyncUsersWithCwf(List<CWFDataContracts.User> users)
        {
            using (_logger.InfoCall("SyncUsersWithCwf"))
            {
                var prinicpal = GetPrincipalFromRequest();
                var productId = GetProductIdFromrequest(prinicpal);

                int prodId;
                int.TryParse(productId, out prodId);

                // get proper role ID
                var availableRoles = _dashboardHost.GetRoles(prinicpal);
                var userRoleId = 0;
                foreach (var role in availableRoles)
                {
                    if (role.RoleName.Equals("ProductUser", StringComparison.OrdinalIgnoreCase))
                        userRoleId = role.RoleID;
                }

                // get all existing users
                var cwfUsers = _dashboardHost.GetUsers(prinicpal);
                var success = true;

                foreach (var user in users)
                {
                    // update user permissions if it exists
                    if (UpdateUser(user, cwfUsers, prinicpal, prodId, userRoleId, user.IsEnabled))
                        continue;

                    var productUser = new CreateUser();
                    productUser.Account = user.Account;
                    productUser.SID = user.Sid;
                    productUser.UserType = user.UserType;

                    try
                    {
                        var newUser = _dashboardHost.CreateUser(productUser, prinicpal);
                        if (userRoleId <= 0) 
                            continue;

                        var permissionsList = new EditPermissions();
                        var permission = new EditPermission()
                        {
                            RoleID = userRoleId, 
                            ProductID = prodId
                        };
                        permissionsList.Add(permission);

                        var updatedInfo = new EditUser();
                        updatedInfo.Account = newUser.Account;
                        updatedInfo.IsEnabled = user.IsEnabled;
                        updatedInfo.Roles = permissionsList;

                        _dashboardHost.EditUser(updatedInfo, newUser.Id.ToString(), prinicpal);
                    }
                    catch (WebFaultException exWebFault)
                    {
                        _logger.WarnFormat("Creation of the user {0} failed.\r\nError Details: {1}\r\nHttp Status Code: {2}\r\n{3}", user.Account, exWebFault.Message, exWebFault.StatusCode, (exWebFault.InnerException != null ? exWebFault.InnerException.Message : string.Empty));
                        success = false;
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorFormat("Creation of the user {0} failed.\r\nError Details: {1}\r\n{2}", user.Account, ex.Message, (ex.InnerException != null ? ex.InnerException.Message : string.Empty));
                        success = false;
                    }
                }
                return success;
            }
        }
    }
}
