using System;
using System.IO;
using System.Collections.Generic;
using SQLcomplianceCwfAddin.Helpers;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ChangeLogs;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    public partial class RestService
    {
        public FilteredChangeLogsViewResponce GetFilteredChangeLogs(FilteredChangeLogsViewRequest request)
        {

            Console.Out.WriteLine("inside Change Logs");
            using (_logger.InfoCall("GetFilteredChangeLogs"))
            {
                var query = QueryBuilder.Instance.GetFilteredChangeLogs();
                var result = QueryExecutor.Instance.GetFilteredChangeLogs(GetConnection(), query, request);

                return result;
            }
        }

        public ServerChangeLogs GetChangeProperties(FilteredChangeLogsViewRequest request)
        {
            using (_logger.InfoCall("GetChangeProperties"))
            {
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.GetChangeProperties();
                    var result = QueryExecutor.Instance.GetChangeProperties(GetConnection(), query, request);

                    return result;
                }
            }
        }
    }
}

