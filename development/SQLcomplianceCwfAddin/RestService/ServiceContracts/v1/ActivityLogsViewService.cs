using System;
using System.IO;
using System.Collections.Generic;
using SQLcomplianceCwfAddin.Helpers;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ActivityLogs;
using TracerX;
using Idera.SQLcompliance.Core;
using SQLcomplianceCwfAddin.Repository;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    public partial class RestService
    {
        private static TracerX.Logger _logX = TracerX.Logger.GetLogger("WebService");
        private UserToken userToken = new UserToken();
        private static TracerX.Logger LOG = TracerX.Logger.GetLogger("ApplicationModel");

        public FilteredActivityLogsViewResponce GetFilteredActivityLogs(FilteredActivityLogsViewRequest request)
        {

            Console.Out.WriteLine("inside Activity Logs");
            using (_logger.InfoCall("GetFilteredActivityLogs"))
            {
                var query = QueryBuilder.Instance.GetFilteredActivityLogs();
                var result = QueryExecutor.Instance.GetFilteredActivityLogs(GetConnection(), query, request);

                return result;
            }
        }

        public ServerActivityLogs GetActivityProperties(FilteredActivityLogsViewRequest request)
        {
            using (_logger.InfoCall("GetActivityProperties"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.GetActivityProperties();
                    var result = QueryExecutor.Instance.GetActivityProperties(GetConnection(), query, request);

                    return result;
                }
            }
        }
    }
}
