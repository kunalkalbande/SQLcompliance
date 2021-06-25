using Idera.SQLcompliance.Application.GUI.SQL;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Remoting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
    public class DefaultAuditSettingsHelper
    {
        public static DatabaseRecord GetDBAuditSettings(string serverName)
        {
            string queryDefaultSettings = String.Format("select * from {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDefaultDatabaseSettings);
            string serverDefaultSettings = String.Format("select * from {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.DefaultServerPropertise);

            UserList srvDefaultTrustedUserList, srvDefaultPrivilegedUserList;
            UserList defaultTrustedUserList, defaultPrivilegedUserList;
            var databaseRecord = new DatabaseRecord();
            try
            {
                #region read server default settings
                using (SqlCommand srvCommand = new SqlCommand(serverDefaultSettings, Globals.Repository.Connection))
                {
                    using (SqlDataReader srvReader = srvCommand.ExecuteReader())
                    {
                        srvReader.Read();
                        srvDefaultPrivilegedUserList = new UserList(SQLHelpers.GetString(srvReader, "auditUsersList"));
                        srvDefaultTrustedUserList = new UserList(SQLHelpers.GetString(srvReader, "auditTrustedUsersList"));

                        databaseRecord.AuditDDL = SQLHelpers.GetBool(srvReader, "auditDDL");
                        databaseRecord.AuditSecurity = SQLHelpers.GetBool(srvReader, "auditSecurity");
                        databaseRecord.AuditAdmin = SQLHelpers.GetBool(srvReader, "auditAdmin");
                        databaseRecord.AuditAccessCheck = (AccessCheckFilter)SQLHelpers.GetByteToInt(srvReader, "auditFailures");
                        databaseRecord.AuditUserAll = SQLHelpers.GetBool(srvReader, "auditUserAll");
                        databaseRecord.AuditUserLogins = SQLHelpers.GetBool(srvReader, "auditUserLogins");
                        databaseRecord.AuditUserLogouts = SQLHelpers.GetBool(srvReader, "auditUserLogouts");
                        databaseRecord.AuditUserFailedLogins = SQLHelpers.GetBool(srvReader, "auditUserFailedLogins");
                        databaseRecord.AuditUserDDL = SQLHelpers.GetBool(srvReader, "auditUserDDL");
                        databaseRecord.AuditUserSecurity = SQLHelpers.GetBool(srvReader, "auditUserSecurity");
                        databaseRecord.AuditUserAdmin = SQLHelpers.GetBool(srvReader, "auditUserAdmin");
                        databaseRecord.AuditUserDML = SQLHelpers.GetBool(srvReader, "auditUserDML");
                        databaseRecord.AuditUserSELECT = SQLHelpers.GetBool(srvReader, "auditUserSELECT");
                        databaseRecord.AuditUserUDE = SQLHelpers.GetBool(srvReader, "auditUserUDE");
                        databaseRecord.AuditUserAccessCheck = (AccessCheckFilter)SQLHelpers.GetByteToInt(srvReader, "auditUserFailures");
                        databaseRecord.AuditUserCaptureSQL = SQLHelpers.GetBool(srvReader, "auditUserCaptureSQL");
                        databaseRecord.AuditUserCaptureTrans = SQLHelpers.GetBool(srvReader, "auditUserCaptureTrans");
                        databaseRecord.AuditUserCaptureDDL = SQLHelpers.GetBool(srvReader, "auditUserCaptureDDL");
                        srvReader.Close();
                    }
                }
                #endregion

                #region read actual server settings
                ServerRecord actualServerSettings = new ServerRecord();
                actualServerSettings.Connection = Globals.Repository.Connection;
                actualServerSettings.Read(serverName);
                #endregion

                #region read db default settings
                using (SqlCommand command = new SqlCommand(queryDefaultSettings, Globals.Repository.Connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();

                        defaultTrustedUserList = new UserList(SQLHelpers.GetString(reader, "auditUsersList"));
                        defaultPrivilegedUserList = new UserList(SQLHelpers.GetString(reader, "auditPrivUsersList"));

                        var finalAuditFailures = SQLHelpers.GetByteToInt(reader, "auditFailures");

                        var finalAuditUserFailures = SQLHelpers.GetByteToInt(reader, "auditUserFailures");
                        if ((int)actualServerSettings.AuditUserAccessCheck != 1)
                            finalAuditUserFailures = (int)actualServerSettings.AuditUserAccessCheck;
                        else if ((int)databaseRecord.AuditUserAccessCheck != 1)
                            finalAuditUserFailures = (int)databaseRecord.AuditUserAccessCheck; //set from server default above

                        databaseRecord.AuditDDL = SQLHelpers.GetBool(reader, "auditDDL");
                        databaseRecord.AuditSecurity = SQLHelpers.GetBool(reader, "auditSecurity");
                        databaseRecord.AuditAdmin = SQLHelpers.GetBool(reader, "auditAdmin");
                        databaseRecord.AuditDML = SQLHelpers.GetBool(reader, "auditDML");
                        databaseRecord.AuditSELECT = SQLHelpers.GetBool(reader, "auditSELECT");
                        databaseRecord.AuditAccessCheck = (AccessCheckFilter)finalAuditFailures;
                        databaseRecord.AuditCaptureSQL = SQLHelpers.GetBool(reader, "auditCaptureSQL");
                        databaseRecord.AuditCaptureTrans = SQLHelpers.GetBool(reader, "auditCaptureTrans");
                        databaseRecord.AuditCaptureDDL = SQLHelpers.GetBool(reader, "auditCaptureDDL");
                        databaseRecord.AuditUserAll = databaseRecord.AuditUserAll || actualServerSettings.AuditUserAll || SQLHelpers.GetBool(reader, "auditUserAll"); ;
                        databaseRecord.AuditUserLogins = databaseRecord.AuditUserLogins || actualServerSettings.AuditUserLogins || SQLHelpers.GetBool(reader, "auditUserLogins");
                        databaseRecord.AuditUserLogouts = databaseRecord.AuditUserLogouts || actualServerSettings.AuditUserLogouts || SQLHelpers.GetBool(reader, "auditUserLogouts");
                        databaseRecord.AuditUserFailedLogins = databaseRecord.AuditUserFailedLogins || actualServerSettings.AuditUserFailedLogins || SQLHelpers.GetBool(reader, "auditUserFailedLogins");
                        databaseRecord.AuditUserDDL = databaseRecord.AuditUserDDL || actualServerSettings.AuditUserDDL || SQLHelpers.GetBool(reader, "auditUserDDL");
                        databaseRecord.AuditUserSecurity = databaseRecord.AuditUserSecurity || actualServerSettings.AuditUserSecurity || SQLHelpers.GetBool(reader, "auditUserSecurity");
                        databaseRecord.AuditUserAdmin = databaseRecord.AuditUserAdmin || actualServerSettings.AuditUserAdmin || SQLHelpers.GetBool(reader, "auditUserAdmin");
                        databaseRecord.AuditUserDML = databaseRecord.AuditUserDML || actualServerSettings.AuditUserDML || SQLHelpers.GetBool(reader, "auditUserDML");
                        databaseRecord.AuditUserSELECT = databaseRecord.AuditUserSELECT || actualServerSettings.AuditUserSELECT || SQLHelpers.GetBool(reader, "auditUserSELECT");
                        databaseRecord.AuditUserUDE = databaseRecord.AuditUserUDE || actualServerSettings.AuditUserUDE || SQLHelpers.GetBool(reader, "auditUserUDE");
                        databaseRecord.AuditUserAccessCheck = (AccessCheckFilter)finalAuditUserFailures;
                        databaseRecord.AuditUserCaptureSQL = databaseRecord.AuditUserCaptureSQL || actualServerSettings.AuditUserCaptureSQL || SQLHelpers.GetBool(reader, "auditUserCaptureSQL");
                        databaseRecord.AuditUserCaptureTrans = databaseRecord.AuditUserCaptureTrans || actualServerSettings.AuditUserCaptureTrans || SQLHelpers.GetBool(reader, "auditUserCaptureTrans");
                        databaseRecord.AuditUserCaptureDDL = databaseRecord.AuditUserCaptureDDL || actualServerSettings.AuditUserCaptureDDL || SQLHelpers.GetBool(reader, "auditUserCaptureDDL");
                    }
                }
                #endregion

                #region merge trusted and pri users
                //getting roles and logins for the server
                ICollection roleList = null, loginList = null;
                SQLDirect sqlServer = null;

                //read the available logins and roles from server
                if (actualServerSettings.IsDeployed && actualServerSettings.IsRunning)
                {
                    string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
                    try
                    {
                        AgentManager manager = GUIRemoteObjectsProvider.AgentManager();


                        roleList = manager.GetRawServerRoles(serverName);

                        loginList = manager.GetRawServerLogins(serverName);

                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                                 String.Format("LoadRoles or Logins: URL: {0} Instance {1}", url, serverName),
                                                 ex,
                                                 ErrorLog.Severity.Warning);
                    }
                }

                if (roleList == null && loginList == null)
                {
                    sqlServer = new SQLDirect();
                    if (sqlServer.OpenConnection(serverName))
                    {
                        roleList = RawSQL.GetServerRoles(sqlServer.Connection);
                        loginList = RawSQL.GetServerLogins(sqlServer.Connection);
                    }
                }

                if (sqlServer != null)
                {
                    sqlServer.CloseConnection();
                }

                //actul servers users
                var actualServerPrivUsers = new UserList(actualServerSettings.AuditUsersList);
                var actualServerTrustedUsers = new UserList(actualServerSettings.AuditTrustedUsersList);

                UserList trustedUserList = new UserList();
                UserList privilegedUserList = new UserList();
                // SQLCM-5849: Handle case-insensitive trusted/privilege user for default settings
                if ((roleList != null) && (roleList.Count != 0))
                {
                    foreach (RawRoleObject role in roleList)
                    {
                        foreach (ServerRole serverRole in defaultTrustedUserList.ServerRoles)
                        {
                            // SQLCM-5868: Roles added to default server settings gets added twice at database level
                            if (serverRole.CompareName(role))
                            {
                                trustedUserList.AddServerRole(role);
                                break;
                            }
                        }
                        foreach (ServerRole serverRole in defaultPrivilegedUserList.ServerRoles)
                        {
                        	// SQLCM-5868: Roles added to default server settings gets added twice at database level
                            if (serverRole.CompareName(role))
                            {
                                privilegedUserList.AddServerRole(role);
                                break;
                            }
                        }

                        foreach (var trustedServerRole in srvDefaultTrustedUserList.ServerRoles)
                        {
                            var rawRole = (RawRoleObject)role;
                            if (trustedServerRole.CompareName(rawRole))
                            {
                                trustedUserList.AddServerRole(rawRole);
                                break;
                            }
                        }

                        foreach (var trustedServerRole in actualServerTrustedUsers.ServerRoles)
                        {
                            var rawRole = (RawRoleObject)role;
                            if (trustedServerRole.CompareName(rawRole))
                            {
                                trustedUserList.AddServerRole(rawRole);
                                break;
                            }
                        }
                    }

                }

                // SQLCM-5849: Handle case-insensitive trusted/privilege user for default settings
                if ((loginList != null) && (loginList.Count != 0))
                {
                    foreach (RawLoginObject login in loginList)
                    {
                        foreach (Login defaultLogin in defaultTrustedUserList.Logins)
                        {
                            if (login.name.Equals(defaultLogin.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                trustedUserList.AddLogin(login);
                                break;
                            }
                        }
                        foreach (Login defaultLogin in defaultPrivilegedUserList.Logins)
                        {
                            if (login.name.Equals(defaultLogin.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                privilegedUserList.AddLogin(login);
                                break;
                            }
                        }

                        foreach (var trustedLogin in srvDefaultTrustedUserList.Logins)
                        {
                            if (login.name.Equals(trustedLogin.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                trustedUserList.AddLogin(login);
                                break;
                            }
                        }

                        foreach (var trustedLogin in actualServerTrustedUsers.Logins)
                        {
                            if (login.name.Equals(trustedLogin.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                trustedUserList.AddLogin(login);
                                break;
                            }
                        }
                    }
                }

                databaseRecord.AuditUsersList = (trustedUserList.Logins.Length > 0 || trustedUserList.ServerRoles.Length > 0) ?
                    trustedUserList.ToString() : string.Empty;
                databaseRecord.AuditPrivUsersList = (privilegedUserList.Logins.Length > 0 || privilegedUserList.ServerRoles.Length > 0) ?
                    privilegedUserList.ToString() : string.Empty;
                #endregion
            }
            catch(Exception ex)
            {
                return null;
            }
            return databaseRecord;
        }
    }
}
