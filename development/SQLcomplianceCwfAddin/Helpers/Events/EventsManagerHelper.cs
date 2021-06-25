using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Event;
using SQLcomplianceCwfAddin.Helpers.SQL;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Events;

namespace SQLcomplianceCwfAddin.Helpers.Events
{
    public class EventsManagerHelper : Singleton<EventsManagerHelper>
    {
        public EventProperties GetEventProperties(EventPropertiesRequest request, SqlConnection connection)
        {
            string eventDatabase = request.EventDatabase;

            if (string.IsNullOrEmpty(eventDatabase))
            {
                eventDatabase = QueryExecutor.Instance.GetEventDatabaseName(connection.ConnectionString, request.ServerId);
                request.EventDatabase = eventDatabase;
            }

            string query = QueryBuilder.Instance.GetEventProperties(eventDatabase, request.EventId);
            var response = QueryExecutor.Instance.GetEventProperties(connection, query, eventDatabase);
            var eventRecord = SqlCmRecordReader.GetEventRecord(request.EventId, eventDatabase, connection);

            if (eventRecord != null)
            {
                LoadSensitiveColumns(eventRecord, request, response, connection);
                LoadBeforeAfterData(eventRecord, request, response, connection);
            }

            return response;
        }

        private void LoadSensitiveColumns(EventRecord eventRecord, EventPropertiesRequest request, EventProperties response, SqlConnection connection)
        {
            var columns = SensitiveColumnRecord.GetSensitiveColumnRecords(connection, request.EventDatabase, eventRecord);

            if (columns.Count > 0)
            {
                response.SensitiveColumnList = new List<string>(columns.Count);

                foreach (var column in columns)
                {
                    response.SensitiveColumnList.Add(column.columnName);
                }
            }
        }

        private void LoadBeforeAfterData(EventRecord eventRecord, EventPropertiesRequest request, EventProperties response, SqlConnection connection)
        {
            response.BeforeAfterData = new EventBeforeAfterData();
            var server = SqlCmRecordReader.GetServerRecord(request.ServerId, connection);
            // SQL Server 2005,2008
            if (server.SqlVersion < 9)
            {
                response.BeforeAfterData.StatusMessage = CoreConstants.Feature_BeforeAfterNotAvailable;
                return;
            }

            string columnAffected = string.Empty;
            string rowsAffected = string.Empty;

            try
            {
                if (eventRecord.eventCategory != TraceEventCategory.DML)
                {
                    columnAffected = "Not Applicable";
                    rowsAffected = "Not Applicable";
                    return;
                }

                if (eventRecord.startSequence == -1 && eventRecord.endSequence == -1)
                {
                    columnAffected = "Not Available";
                    rowsAffected = "Not Available";
                    return;
                }

                var dcRecords = DataChangeRecord.GetDataChangeRecords(connection, request.EventDatabase, eventRecord);
                response.BeforeAfterData.RowsAffectedStatusMessage = dcRecords.Count.ToString();

                if (dcRecords.Count == 0)
                {
                    columnAffected = "None";
                    return;
                }

                int index = 1;
                var columnNames = new List<string>();

                response.BeforeAfterData.BeforeAfterValueList = new List<BeforeAfterValue>();
                foreach (DataChangeRecord rec in dcRecords)
                {
                    List<ColumnChangeRecord> ccRecords = ColumnChangeRecord.GetColumnChangeRecords(connection, request.EventDatabase, rec);

                    foreach (ColumnChangeRecord change in ccRecords)
                    {
                        var beforeAfterValue = new BeforeAfterValue
                        {
                            RowNumber = index,
                            PrimaryKey = rec.primaryKey,
                            Column = change.columnName,
                            BeforeValue = change.beforeValue,
                            AfterValue = change.afterValue,
                        };

                        response.BeforeAfterData.BeforeAfterValueList.Add(beforeAfterValue);

                        if (!columnNames.Contains(change.columnName))
                        {
                            columnNames.Add(change.columnName);
                        }
                    }
                    columnAffected = string.Join(",", columnNames.ToArray());
                    index++;
                }

                if (response.BeforeAfterData.BeforeAfterValueList.Count > 0)
                {
                    response.BeforeAfterData.IsAvailable = true;
                }
                else
                {
                    response.BeforeAfterData.BeforeAfterValueList = null;
                    columnAffected = "None";
                }
            }
            catch (Exception e)
            {
                columnAffected = "Error loading Before-After data";
                rowsAffected = "Error loading Before-After data";
                ErrorLog.Instance.Write("Error loading Before-After data", e);
            }
            finally
            {
                response.BeforeAfterData.ColumnsAffectedStatusMessage = columnAffected;
                response.BeforeAfterData.RowsAffectedStatusMessage = rowsAffected;
            }
        }
    }
}
