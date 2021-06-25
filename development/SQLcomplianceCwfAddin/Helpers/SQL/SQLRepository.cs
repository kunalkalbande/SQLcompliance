using System.Collections.Generic;
using Idera.SQLcompliance.Core;
using System;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core.Agent;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ServerProperties;

namespace SQLcomplianceCwfAddin.Helpers.SQL
{
    /// <summary>
    /// This class was PARTIALLY COPIED from Idera.SQLcompliance.Application.GUI.SQL and should be moved to the SQLcomplianceCore in the future release
    /// </summary>
    public class SQLRepository
    {
        //--------------------------------------------------------------
        // IsSQLsecureOwnedDB - Checks if a database is a SQLsecure owned db
        //--------------------------------------------------------------
        static public bool IsSQLsecureOwnedDB(string dbName, SqlConnection connection)
        {
            bool retval = false;

            try
            {
                string selectQuery = String.Format("SELECT count(*) FROM {0} WHERE databaseName = {1}",
                                                   CoreConstants.RepositorySystemDatabaseTable,
                                                   SQLHelpers.CreateSafeString(dbName));

                SqlCommand cmd = new SqlCommand(selectQuery, connection);
                object obj = cmd.ExecuteScalar();
                int count;

                if (obj is DBNull)
                {
                    count = 0;
                }
                else
                {
                    count = (int)obj;
                }

                if (count != 0)
                {
                    retval = true;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(
                       ErrorLog.Level.Verbose,
                       "An error occurred reading the system database table in the Repository.",
                       ex,
                       ErrorLog.Severity.Warning);
            }

            return retval;
        }

        //----------------------------------------------------------------
        // GetStatus
        //----------------------------------------------------------------
        static public ServerStatus GetStatus(ServerRecord server, SqlConnection connection, out string opStatus, out string auditStatus)
        {
            opStatus = ServerStatusMessages.ServerStatus_OK;
            ServerStatus statusImage = ServerStatus.Ok;

            int repositorySqlVersion = SQLHelpers.GetSqlVersion(connection);

            if (!server.IsAuditedServer)
            {
                //-----------------------
                // Archive host
                //-----------------------
                statusImage = ServerStatus.Archive;
                opStatus = ServerStatusMessages.ServerStatus_NotAudited;
                auditStatus = "";
            }
            else
            {
                //----------------
                // Audited Server
                //----------------

                if (!server.IsDeployed)
                {
                    //--------------
                    // Not deployed
                    //--------------
                    statusImage = ServerStatus.Warning;
                    if (server.IsDeployedManually)
                        opStatus = ServerStatusMessages.ServerStatus_AwaitingManual;
                    else
                        opStatus = ServerStatusMessages.ServerStatus_NotDeployed;
                }
                else
                {
                    //-------------------------------------
                    // Deployed - so check for other stuff
                    //-------------------------------------
                    if (server.TimeLastAgentContact == DateTime.MinValue)
                    {
                        // Agent has never reported in
                        statusImage = ServerStatus.Warning;
                        opStatus = ServerStatusMessages.ServerStatus_Unknown;
                    }
                    else
                    {
                        // check for supported SQL Server Version 
                        // we cant support auditing 2005 from a repository hosted by 2000
                        if (server.SqlVersion > repositorySqlVersion)
                        {
                            statusImage = ServerStatus.Alert;
                            if (server.SqlVersion == 9)
                                opStatus = ServerStatusMessages.ServerStatus_2005NotSupported;
                            else if (server.SqlVersion == 10)
                                opStatus = ServerStatusMessages.ServerStatus_2008NotSupported;
                            else if (server.SqlVersion == 11)
                                opStatus = ServerStatusMessages.ServerStatus_2012NotSupported;
                            else if (server.SqlVersion == 12)
                                opStatus = ServerStatusMessages.ServerStatus_2014NotSupported;
                            else if (server.SqlVersion == 13)
                                opStatus = ServerStatusMessages.ServerStatus_2016NotSupported;
                            else if (server.SqlVersion == 14)
                                opStatus = ServerStatusMessages.ServerStatus_2017NotSupported;
                        }
                        else if (server.SqlVersion == 9 && repositorySqlVersion == 9
                                && server.AgentVersion.StartsWith("1."))
                        {
                            // We also can't support auditing 2005 with a 1.x agent
                            statusImage = ServerStatus.Alert;
                            opStatus = ServerStatusMessages.ServerStatus_AgentUpgradeRequired;
                        }
                        else if (server.SqlVersion == 10 && repositorySqlVersion == 10
                           && (server.AgentVersion.StartsWith("1.") || server.AgentVersion.StartsWith("2.")
                           || server.AgentVersion.StartsWith("3.0")))
                        {
                            // We also can't support auditing 2008 with a 1.x, 2.x, or 3.0 agent
                            statusImage = ServerStatus.Alert;
                            opStatus = ServerStatusMessages.ServerStatus_AgentUpgradeRequired;
                        }
                        else if (server.SqlVersion == 11 &&
                            repositorySqlVersion == 11 &&
                            (server.AgentVersion.StartsWith("1.") ||
                             server.AgentVersion.StartsWith("2.") ||
                             server.AgentVersion.StartsWith("3.0") ||
                             server.AgentVersion.StartsWith("3.1") ||
                             server.AgentVersion.StartsWith("3.2") ||
                             server.AgentVersion.StartsWith("3.3") ||
                             server.AgentVersion.StartsWith("3.5") ||
                             server.AgentVersion.StartsWith("3.6")))
                        {
                            //we can't support 2012 with anything less than 3.7
                            statusImage = ServerStatus.Alert;
                            opStatus = ServerStatusMessages.ServerStatus_AgentUpgradeRequired;
                        }
                        else
                        {
                            if (!server.IsRunning)
                            {
                                // Agent has reported in
                                statusImage = ServerStatus.Alert;
                                opStatus = ServerStatusMessages.ServerStatus_NotRunning;
                            }
                            else if (server.IsCrippled)
                            {
                                statusImage = ServerStatus.Alert;
                                opStatus = ServerStatusMessages.ServerStatus_Crippled;
                            }
                            else
                            {
                                DateTime noContactWarning = DateTime.UtcNow;
                                DateTime noContactError = DateTime.UtcNow;

                                noContactWarning = noContactWarning.AddMinutes(-2 * server.AgentHeartbeatInterval);
                                noContactError = noContactError.AddDays(-1);

                                if (DateTime.Compare(server.TimeLastAgentContact,
                                                    noContactError) < 0)
                                {
                                    opStatus = ServerStatusMessages.ServerStatus_VeryStale;
                                    statusImage = ServerStatus.Alert;
                                }
                                else if (DateTime.Compare(server.TimeLastAgentContact,
                                                         noContactWarning) < 0)
                                {
                                    opStatus = ServerStatusMessages.ServerStatus_Stale;
                                    statusImage = ServerStatus.Warning;
                                }
                            }
                        }
                    }
                }

                // Audit Status
                if (server.IsEnabled)
                    auditStatus = ServerStatusMessages.ServerStatus_Enabled;
                else
                {
                    statusImage = ServerStatus.Disabled;
                    auditStatus = ServerStatusMessages.ServerStatus_Disabled;
                }

                if (server.IsDeployed)
                {
                    if (server.ConfigUpdateRequested)
                    {
                        if (server.ConfigVersion == server.LastKnownConfigVersion)
                        {
                            ServerRecord oldServerState = server.Clone();
                            server.ConfigUpdateRequested = false;
                            server.Connection = connection;
                            server.Write(oldServerState);
                        }
                        else
                        {
                            auditStatus += ServerStatusMessages.ServerStatus_Requested;
                        }
                    }
                    else if (server.ConfigVersion != server.LastKnownConfigVersion)
                    {
                        auditStatus += ServerStatusMessages.ServerStatus_Pending;
                    }
                }
                else
                {
                    auditStatus = "None until deployed";
                }
            }
            // System Alerts
            if (server.AgentHealth != 0)
            {
                List<SystemAlertType> alerts = SystemAlert.GetAgentHealthDetails(server.AgentHealth);
                if (alerts.Count > 1)
                {
                    opStatus = String.Format("{0} Unresolved System Alerts", alerts.Count);
                }
                else if (alerts.Count == 1)
                {
                    switch (alerts[0])
                    {
                        case SystemAlertType.AgentWarning:
                            opStatus = "Agent Warning";
                            break;
                        case SystemAlertType.AgentConfigurationError:
                            opStatus = "Agent Configuration Error";
                            break;
                        case SystemAlertType.TraceDirectoryError:
                            opStatus = "Trace Directory Error";
                            break;
                        case SystemAlertType.SqlTraceError:
                            opStatus = "SQL Trace Error";
                            break;
                        case SystemAlertType.ServerConnectionError:
                            opStatus = "Server Connection Error";
                            break;
                        case SystemAlertType.CollectionServiceConnectionError:
                            opStatus = "Collection Service Connection Error";
                            break;
                        case SystemAlertType.ClrError:
                            opStatus = "CLR Error";
                            break;
                    }
                }
                else
                {
                    opStatus = "Unresolved System Alert";
                }
                auditStatus = ServerStatusMessages.AuditStatus_ViewActivityLog;
                statusImage = ServerStatus.Alert;
            }

            return statusImage;
        }
    }
}
