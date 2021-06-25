using System;
using System.Collections.Generic;
using System.Net.Configuration;
using System.Text;
using Idera.SQLcompliance.Core;
using SQLcomplianceCwfAddin.Helpers;
using SQLcomplianceCwfAddin.Helpers.Archives;
using SQLcomplianceCwfAddin.Helpers.Server;
using SQLcomplianceCwfAddin.Helpers.SQL;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Archives;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    public partial class RestService
    {
        public IEnumerable<ArchiveRecord> GetArchivesList(ArchiveInstanceRequest archveInstanceRequest)
        {
            using (_logger.InfoCall("GetArchivesList"))
            {
                var query = QueryBuilder.Instance.GetArchivesList();
                var result = QueryExecutor.Instance.GetArchiveList(GetConnection(), query, archveInstanceRequest.Instance);
                return result;
            }
        }

        public ArchiveProperties GetArchiveProperties(ArchivePropertiesRequest archive)
        {
            using (_logger.InfoCall("GetArchiveProperties"))
            {
                var query = QueryBuilder.Instance.GetArchiveProperties(archive.Archive);
                var result = QueryExecutor.Instance.GetArchiveProperties(GetConnection(), query, archive.Archive);
                return result;
            }
        }

        public void UpdateArchiveSettings(ArchiveUpdateRequest request)
        {
            using (_logger.InfoCall("UpdateArchiveSettings"))
            {
                ArchiveDbHelper.UpdateArchiveProperties(GetConnection(), request);
            }
        }

        public void DetachArchive(DetachArchiveRequest request)
        {
            using (_logger.InfoCall("UpdateArchiveSettings"))
            {
                var prinicpal = GetPrincipalFromRequest();
                var connectionCredentials = GetConnectionCredentials(prinicpal);
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.DetachArchive();
                    QueryExecutor.Instance.DetachArchive(connection, query, request);
                    string snapshot = String.Format("Detach archive database: {0}\r\n\r\n", request.DatabaseName);

                    LogRecord.WriteLog(connection, LogType.DetachArchive,
                                        request.Instance, snapshot
                                       ,connectionCredentials.ConnectionUser);
                }
            }
        }

        public bool CheckNeedIndexUpdates(ArchivePropertiesRequest archive)
        {
            using (_logger.InfoCall("CheckNeedIndexUpdates"))
            {
                var properties = GetArchiveProperties(archive);
                return properties.Schema != CoreConstants.RepositoryEventsDbSchemaVersion;
            }
        }

        public bool IsIndexStartTimeIsValid()
        {
            using (_logger.InfoCall("IsIndexStartTimeIsValid"))
            {
                using (var connection = GetConnection())
                {
                    SQLcomplianceConfiguration configuration = SqlCmConfigurationHelper.GetConfiguration(connection);
                    return configuration.IndexStartTime != DateTime.MinValue;
                }
            }
        }

        public void ApplyReindex(DatabaseReindexRequest request)
        {
            using (_logger.InfoCall("ApplyReindex"))
            {
                SQLcomplianceConfiguration configuration = SqlCmConfigurationHelper.GetConfiguration(GetConnection());

                configuration.IndexStartTime = request.IndexStartTime;
                configuration.IndexDuration = new TimeSpan(request.IndexDurationHours, request.IndexDurationMinutes, 0);
                using (var connection = GetConnection())
                {
                    configuration.Write(connection);
                }

                ServerManagerHelper.SetReindexFlag(true, configuration);
            }
        }

        public void AttachArchive(string archive)
        {
            using (_logger.InfoCall("ApplyReindex"))
            {
                var properties = GetArchiveProperties(new ArchivePropertiesRequest { Archive = archive });
                using (var connection = GetConnection())
                {
                    var query = QueryBuilder.Instance.AttachArchive();
                    QueryExecutor.Instance.AttachArchive(connection, query, archive);

                    // Build snapshot
                    StringBuilder snap = new StringBuilder(1024);

                    snap.AppendFormat("Attach archive database: {0}\r\n\r\n", properties.DatabaseName);
                    snap.AppendFormat("Display name: {0}\r\n", properties.DisplayName);
                    snap.AppendFormat("Description: {0}\r\n", properties.Description);

                    string snapshot = snap.ToString();

                    LogRecord.WriteLog(connection,
                        LogType.AttachArchive,
                        properties.Instance,
                        snapshot);
                }
            }
        }
    }
}
