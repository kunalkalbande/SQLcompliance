using System.Collections.Generic;
using Idera.SQLcompliance.Core;
using SQLcomplianceCwfAddin.Helpers;
using SQLcomplianceCwfAddin.Helpers.Agent;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.UserSettings;
using Idera.SQLcompliance.Core.Collector;
using System;
using SQLcomplianceCwfAddin.Helpers.SQL;
using Idera.SQLcompliance.Core.Remoting;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    public partial class RestService
    {
        public UserSettingsModel GetUserSettings(int dashboardUserId)
        {
            using (_logger.InfoCall("GetUserSettings"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.GetUserSettings();
                    var result = QueryExecutor.Instance.GetUserSettings(connection, query, dashboardUserId);
                    return result;
                }
            }
        }

        public IEnumerable<UserSettingsModel> GetAllUserSettings()
        {
            using (_logger.InfoCall("GetAllUserSettings"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.GetUserSettings();
                    var result = QueryExecutor.Instance.GetUserSettings(connection, query);
                    return result;
                }
            }
        }

        public void CreateUpdateUserSettings(UserSettingsModel userSettings)
        {
            using (_logger.InfoCall("CreateUpdateUserSettings"))
            {
                using (var connection = GetConnection())
                {
                    Validator.Instance.AssertIsValid(userSettings);
                    var query = QueryBuilder.Instance.CreateUpdateUserSettings();
                    QueryExecutor.Instance.CreateUpdateUserSettings(connection, query, userSettings);
                    AgentManagerHelper.Instance.UpdateAuditSettingsForAuditUsers(connection);
                }
            }
        }

        public void DeleteUserSettings(DeleteUserSettingsRequest deleteUserSettingsRequest)
        {
            using (_logger.InfoCall("DeleteUserSettings"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.DeleteUserSettings();
                    QueryExecutor.Instance.DeleteUserSettings(connection, query, deleteUserSettingsRequest);
                    AgentManagerHelper.Instance.UpdateAuditSettingsForAuditUsers(connection);
                }
            }
        }
        //SQLCM 5.4 SCM-9 Start
        public string SetRefreshDuration(string refreshDuration)
        {
            using (_logger.InfoCall("SetRefreshDuration"))
            {
                var result = "0";
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.CreateRefreshDuration(refreshDuration);
                    result= QueryExecutor.Instance.CreateRefreshDuration(connection, query, refreshDuration);
                    AgentManagerHelper.Instance.UpdateAuditSettingsForAuditUsers(connection);
                    return result;
                }
            }

        }

        public string GetRefreshDuration()
        {
            using (_logger.InfoCall("GetRefreshDuration"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.GetRefreshDuration();

                    var result = QueryExecutor.Instance.GetRefreshDuration(connection, query);
                    PushInstancesToCwfDashboard();
                    return result;

                }
            }
        }    //SQLCM 5.4 SCM-9 Start
    }
}
