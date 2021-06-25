using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using PluginCommon;
using SQLcomplianceCwfAddin.Helpers;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    public partial class RestService
    {
        public void SetViewSettings(ViewSettings settings)
        {
            using (_logger.InfoCall("SetViewSettings"))
            {
                using (var connection = GetConnection())
                {
                    IPrincipal prinicpal = GetPrincipalFromRequest();
                    ConnectionCredentials connectionCredentials = GetConnectionCredentials(prinicpal);

                    string query = QueryBuilder.Instance.SetViewSetting();
                    QueryExecutor.Instance.SetViewSettings(connection, query, connectionCredentials.ConnectionUser, settings);
                }
            }
        }

        public ViewSettings GetViewSettings(string viewName)
        {
            using (_logger.InfoCall("GetViewSettings"))
            {
                IPrincipal prinicpal = GetPrincipalFromRequest();
                ConnectionCredentials connectionCredentials = GetConnectionCredentials(prinicpal);

                string query = QueryBuilder.Instance.GetViewSettings(connectionCredentials.ConnectionUser, viewName);
                return QueryExecutor.Instance.GetViewSettings(GetConnection(), query);
            }
        }

        public ViewNameResponse GetViewName(string viewId)
        {
            using (_logger.InfoCall("GetViewName"))
            {
                using (var connection = GetConnection())
                {
                    var result = QueryExecutor.Instance.GetViewName(GetConnection(), viewId);
                    return result;
                }
            }
        }
    }
}
