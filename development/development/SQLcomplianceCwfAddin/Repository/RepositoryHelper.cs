using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using BBS.License;
using SQLcomplianceCwfAddin.Errors;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Service;
using Idera.SQLcompliance.Core.Licensing;
using System.Data.SqlTypes;
using SQLcomplianceCwfAddin.Helpers;
using Idera.SQLcompliance.Core.Configuration;
using SQLcomplianceCwfAddin.RestService.ServiceContracts.v1;

namespace SQLcomplianceCwfAddin.Repository
{
    internal static class RepositoryHelper
    {
        #region License
        //public List<SQLcomplianceCwfAddin.RestService.ServiceContracts.v1.CheckedKey> CheckedKeys = new List<SQLcomplianceCwfAddin.RestService.ServiceContracts.v1.CheckedKey>();


        public static LicenseSummary GetLicenseKeys(SqlConnectionInfo connectionInfo)
        {

            List<string> keys = new List<string>();
            int registeredServers = -1;
            string repositoryInstance = null;

            // Use default connection if null was passed.
            connectionInfo = CheckConnectionInfo(connectionInfo);
            

            using (SqlConnection connection = connectionInfo.GetConnection(CoreConstants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlCommand command = SqlHelper.CreateCommand(connection, "[dbo].[sp_sqlcm_GetLicenseKeys]"))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        int fieldId = reader.GetOrdinal("LicenseKey");
                        while (reader.Read())
                        {
                            string key = reader.GetString(fieldId);
                            keys.Add(key);
                        }
                    } // using reader

                    SqlParameter rsc = command.Parameters["@ReturnServerCount"];
                    SqlInt32 sqlValue = (SqlInt32)rsc.SqlValue;
                    if (!sqlValue.IsNull)
                        registeredServers = sqlValue.Value;

                    SqlParameter repository = command.Parameters["@ReturnInstanceName"];
                    SqlString strValue = (SqlString)repository.SqlValue;
                    repositoryInstance = strValue.Value;
                } // using command
            }

            return LicenseSummary.SummarizeKeys(registeredServers, repositoryInstance, keys);
        }

        public static void SetAuditableEntity(LicenseKeyOperation keyOp, string key, string totalKeys)
        {
            //AuditableEntity auditableEntity = GetAuditableEntity();
            //auditableEntity.AddMetadataProperty("License Key", key);
            //auditableEntity.AddMetadataProperty("Total number of servers that license supports", totalKeys == "-1" ? "Unlimited" : totalKeys);
            //auditableEntity.AddMetadataProperty("License operations", keyOp.ToString());

            //AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
            //AuditingEngine.SetAuxiliarData("LicenseEntity", auditableEntity);
        }

        public static BBSLic LoadKeyString(string key)
        {
            BBSLic lic = new BBSLic();
            LicErr licErr = lic.LoadKeyString(key);

            switch (licErr)
            {
                case LicErr.OK:
                    return lic;
                case LicErr.FutureKey:
                    throw new LicenseManagerException(string.Format("The specified key has a creation date of {0}.  Keys with future creation dates are not allowed.\n\nKey: {1}", lic.CreationDate, key));
                default:
                    //Log.Info("BBSLic failed to parse license key.  Error: " + licErr);
                    throw new LicenseManagerException("The specified license key is invalid.");
            }
        }

        public static void AddOrReplaceKey(BBSLic lic, string key)
        {
            //using (Log.InfoCall())
            {

                LicenseKeyOperation keyOp = LicenseKeyOperation.Add;
                //var connection = GetConnection();
                var licSummary = RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo);
                //LicenseSummary licSummary = new LicenseSummary();
                List<SQLcomplianceCwfAddin.Helpers.CheckedKey> keyList = new List<SQLcomplianceCwfAddin.Helpers.CheckedKey>();
                foreach (SQLcomplianceCwfAddin.Helpers.CheckedKey item in licSummary.CheckedKeys)
                {
                    keyList.Add(item);
                }
                if (lic.IsTrial && !licSummary.IsTrial)
                {
                    throw new SQLcomplianceCwfAddin.Errors.LicenseManagerException("A trial key cannot be entered when a production key exists.");
                }

                if (keyList.Count > 0 && (lic.IsTrial || !lic.IsPermanent))
                {
                    // There can only be one such key.
                    keyOp = LicenseKeyOperation.Replace;
                }

                // Check for duplicate or incompatible keys.
                foreach (SQLcomplianceCwfAddin.Helpers.CheckedKey item in keyList)
                {
                    SQLcomplianceCwfAddin.Helpers.CheckedKey listObject = item;
                    if (LicenseSummary.KeysAreEqual(key, listObject.KeyString))
                    {
                        // duplicate keys
                        throw new SQLcomplianceCwfAddin.Errors.LicenseManagerException("The specified key is the same as an existing key.");
                    }
                    else if (listObject.KeyObject == null || listObject.KeyObject.IsTrial || !listObject.KeyObject.IsPermanent)
                    {
                        keyOp = LicenseKeyOperation.Replace;
                    }
                }

                // If this key must replace the others, confirm that the user wants to do that. 


                try
                {
                    //to audit licenses replace
                    SetAuditableEntity(keyOp, key, lic.Limit1.ToString());

                    // Send the new license to the server.
                    //Log.Info("KeyOp is " + keyOp);
                    LicenseSummary license = SetLicenseKeys(keyOp, new string[] { key });
                    //Log.Info(keyOp, " operation succeeded");
                    //ShowLicense(license);
                    //textBox1.Text = string.Empty;

                    /*if (!lic.IsTrial)
                    {
                        //We need to mark the desktop client machine as well as the management service machine
                        RegistryKey rk = null;
                        RegistryKey rks = null;

                        rk = Registry.LocalMachine;
                        rks = rk.CreateSubKey(@"Software\Idera\SQLdm");
                        rks.SetValue("ConfigInfo", 1, RegistryValueKind.DWord);

                        if (rks != null)
                            rks.Close();
                        rks = null;

                        if (rk != null)
                            rk.Close();
                        rk = null;
                    }*/

                }
                catch
                {
                    throw;
                }
            }
        }
        #endregion

        private static object _lock = new object();

        public static void AddLicenseKeys(SqlTransaction transaction, IEnumerable<string> addList)
        {
            // using (LOG.DebugCall())
            {
                // See what errors exist before and after adding the new keys.
                // We will add the new keys if doing so doesn't add a new error.

                List<string> keys = new List<string>();
                //  LicenseSummary summary1 = GetLicenseSummary(transaction, transaction.Connection, keys);
                var summary1 = RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo);
                //LicenseSummary summary1 = new LicenseSummary();
                keys.AddRange(addList);
                LicenseSummary summary2 = LicenseSummary.SummarizeKeys(summary1.MonitoredServers, summary1.Repository, keys);

                // See if we found a different bad key string.
                if (summary2.BadKey != null &&
                    (summary1.BadKey == null || !object.ReferenceEquals(summary1.BadKey.KeyString, summary2.BadKey.KeyString)))
                {
                    throw new SQLcomplianceCwfAddin.Errors.LicenseManagerException(string.Format("Can't add key '" + summary2.BadKey.KeyString + "'.  " + summary2.BadKey.Comment));
                    //throw new ServiceException("Can't add key '" + summary2.BadKey.KeyString + "'.  " + summary2.BadKey.Comment);
                }
                else
                {
                    AddKeysUnchecked(transaction, transaction.Connection, addList);
                }
            }
        }

        public static void AddKeysUnchecked(SqlTransaction transaction, SqlConnection connection, IEnumerable<string> keyList)
        {
            using (SqlCommand command = SqlHelper.CreateCommand(connection, "[dbo].[sp_sqlcm_AddLicenseKey]"))
            {
                command.Transaction = transaction;
                foreach (string key in keyList)
                {
                    if (key != null)
                    {
                        SqlHelper.AssignParameterValues(command.Parameters, key, DBNull.Value);
                        command.ExecuteNonQuery();
                        // don't really care about returned id
                    }
                }
            }
        }

        public static void RemoveLicenseKeys(SqlTransaction transaction, IEnumerable<string> removeKeys)
        {
            //using (LOG.DebugCall())
            //{
            List<string> currentKeys = new List<string>();
            var summary = RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo);
            //LicenseSummary summary = new LicenseSummary();
            
            foreach (var item in summary.CheckedKeys)
            {
                currentKeys.Add(item.KeyString);
            }
            // If we already have a violation, skip checking.
            if (summary.Status == LicenseStatus.OK)
            {
                //LOG.Debug("No violation with current keys.");
                // There is currently no violation.
                // See if removing the specified keys will cause a violation.
                // First determine what the new list will be after removing
                // the specified keys.  We use exact string matching here
                // because the stored proc that deletes keys also works that way.
                foreach (string toRemove in removeKeys) currentKeys.Remove(toRemove);



                summary = LicenseSummary.SummarizeKeys(summary.MonitoredServers, summary.Repository, currentKeys);

                switch (summary.Status)
                {
                    case LicenseStatus.CountExceeded:
                        throw new SQLcomplianceCwfAddin.Errors.LicenseManagerException("Removing the specified key(s) would make the number of licensed servers less than the number of monitored servers.");
                    case LicenseStatus.Expired:
                        throw new SQLcomplianceCwfAddin.Errors.LicenseManagerException("Removing the specified key(s) would result in an expired license.");
                    case LicenseStatus.NoValidKeys:
                        throw new SQLcomplianceCwfAddin.Errors.LicenseManagerException("Removing the specified key(s) would leave no valid keys.");
                }
            }

            // If we get this far, remove the keys.
            using (SqlCommand command = SqlHelper.CreateCommand(transaction.Connection, "[dbo].[sp_sqlcm_DeleteLicenseKey]"))
            {
                command.Transaction = transaction;
                foreach (string key in removeKeys)
                {
                    SqlHelper.AssignParameterValues(command.Parameters, key);
                    command.ExecuteNonQuery();
                    // don't really care about returned id
                }
            }
            //}
        }

        public static void ReplaceLicenseKeys(SqlTransaction transaction, IEnumerable<string> keyList)
        {
            //using (LOG.DebugCall())
            {
                if (keyList == null)
                    throw new SQLcomplianceCwfAddin.Errors.LicenseManagerException("The list of keys is null.");

                // We really need the keys in a List<string>.
                List<string> newKeys = keyList as List<string>;
                if (newKeys == null) newKeys = new List<string>(keyList);

                if (newKeys.Count == 0)
                    throw new SQLcomplianceCwfAddin.Errors.LicenseManagerException("The list of keys is empty.");

                var summary = RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo);
                //LicenseSummary summary = new LicenseSummary();
                
                if (summary.BadKey != null)
                {
                    // All new keys must be completely valid.
                    throw new SQLcomplianceCwfAddin.Errors.LicenseManagerException(string.Format("Can't add key '" + summary.BadKey.KeyString + "'.  " + summary.BadKey.Comment));
                }
                else
                {
                    switch (summary.Status)
                    {
                        case LicenseStatus.OK:
                            // delete all the license keys
                            SqlHelper.ExecuteNonQuery(transaction, "[dbo].[sp_sqlcm_DeleteLicenseKey]", DBNull.Value);
                            // add all keys in the newKeys to the database
                            AddKeysUnchecked(transaction, transaction.Connection, newKeys);
                            break;
                        case LicenseStatus.CountExceeded:
                            throw new SQLcomplianceCwfAddin.Errors.LicenseManagerException("The number of currently monitored servers is greater than the number allowed by the specified key(s).");
                        //throw new ServiceException("The number of currently monitored servers is greater than the number allowed by the specified key(s).");
                        default:
                            // Other conditions should be impossible due to earlier check of summary.BadKey.
                            throw new SQLcomplianceCwfAddin.Errors.LicenseManagerException("Internal error in license check.");
                        //throw new ServiceException("Internal error in license check.");
                    }
                }
            }
        }

        public static LicenseSummary SetLicenseKeys(LicenseKeyOperation operation, IEnumerable<string> keyList)
        {
            //using (LOG.DebugCall())
            {
                //LOG.Debug("operation = ", operation);

                using (SqlConnection connection = RestServiceConfiguration.SQLConnectInfo.GetConnection(CoreConstants.DesktopClientConnectionStringApplicationName))
                {

                    connection.Open();
                    lock (_lock)
                    {
                        SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                        try
                        {
                            switch (operation)
                            {
                                case LicenseKeyOperation.Add:
                                    AddLicenseKeys(transaction, keyList);
                                    break;
                                case LicenseKeyOperation.Remove:
                                    RemoveLicenseKeys(transaction, keyList);
                                    break;
                                case LicenseKeyOperation.Replace:
                                    ReplaceLicenseKeys(transaction, keyList);
                                    break;
                            }

                            var newLicense = RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo);

                            transaction.Commit();

                            return (LicenseSummary)newLicense;
                        }
                        //catch (ServiceException e)
                        //{
                        //    //LOG.Error("ServiceException caught in SetLicenseKeys: ", e);
                        //    transaction.Rollback();
                        //    throw;
                        //}
                        catch (Exception e)
                        {
                            //LOG.Error("Exception caught in SetLicenseKeys: ", e);
                            transaction.Rollback();
                            throw;
                            //throw new ServiceException(e, Status.ErrorUnknown);
                        }
                    }
                }
            }
        }

        private static SqlConnectionInfo CheckConnectionInfo(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                // Use RestServiceConfiguration.SQLConnectInfo.
                if (RestServiceConfiguration.SQLConnectInfo == null)
                {
                    throw new ArgumentNullException("connectionInfo");
                }
                else
                {
                    connectionInfo = RestServiceConfiguration.SQLConnectInfo;
                }
            }
            return connectionInfo;
        }
    }
}
