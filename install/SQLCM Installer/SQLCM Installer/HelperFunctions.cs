using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;

namespace SQLCM_Installer
{
    using System.Globalization;

    public class HelperFunctions
    {
        // SQLCM-5412 User name and password are displaying as plain text in the command
        #region Private Methods - Encryption
        private static string encryptKey = "SQLcomplianceAgentService"; // Key used to encode username/password for Agent service installer

        public static string EncryptString(string str)
        {
            return EncodeString(InternalEncryptString(str));
        }

        //-----------------------------------------------------------------------
        // InternalEncryptString
        //-----------------------------------------------------------------------
        private static string InternalEncryptString(string decStr)
        {
            if (decStr == null || decStr.Length == 0)
            {
                return decStr;
            }

            char[] encChars = new char[decStr.Length];
            char keyChar;

            // Decrypt the string      
            int j = 0; // key counter
            for (int i = 0; i < decStr.Length; i++)
            {
                keyChar = encryptKey[j++];

                if (decStr[i] != keyChar)
                {
                    encChars[i] = (char)(decStr[i] ^ keyChar);
                }
                else
                {
                    encChars[i] = decStr[i];
                }

                if (j == encryptKey.Length)
                {
                    j = 0;
                }
            }
            return new String(encChars);
        }


        private static string EncodeString(string s)
        {
            UnicodeEncoding encoder = new UnicodeEncoding(true, false);
            byte[] encodedBytes = encoder.GetBytes(s);
            StringBuilder retVal = new StringBuilder();

            for (int i = 0; i < encodedBytes.Length; i++)
                retVal.Append(encodedBytes[i].ToString("X2"));
            return retVal.ToString();
        }

        private string DecodeString(string s)
        {
            UnicodeEncoding encoder = new UnicodeEncoding(true, false);
            byte[] encodedBytes = new byte[s.Length / 2];
            char[] decodedChars = new char[encodedBytes.Length / 2];

            for (int i = 0; i < encodedBytes.Length; i++)
            {
                string sTemp = s.Substring(i * 2, 2);
                encodedBytes[i] = byte.Parse(s.Substring(i * 2, 2), NumberStyles.HexNumber);
            }

            encoder.GetDecoder().GetChars(encodedBytes, 0, encodedBytes.Length, decodedChars, 0);
            return new String(decodedChars);
        }

        #endregion
        public bool CheckServerConnection(bool isSQLAuthentication, string repositoryName, string userName, string password, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (var connection = new SqlConnection())
                {
                    connection.ConnectionString = isSQLAuthentication
                        ? string.Format("Server={0};Database=master;User Id={1};Password={2};", repositoryName, userName, password)
                        : string.Format("Server={0};Database=master;Integrated Security=SSPI;", repositoryName);
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = string.Format("SELECT COUNT(*) AS 'MasterDatabaseCount'  FROM sys.databases WHERE name = 'master'");
                        var result = command.ExecuteScalar();
                        if (result == DBNull.Value)
                            return false;

                        return Convert.ToInt32(result) >= 1;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return false;
        }

        public bool IsSQLComplianceDBAvialable(bool isSQLAuthentication, string repositoryName, string userName, string password, out string errorMessage)
        {
            errorMessage = string.Empty;
            bool complianceExists = false;
            bool processignExists = false;
            try
            {
                using (var connection = new SqlConnection())
                {
                    connection.ConnectionString = isSQLAuthentication
                        ? string.Format("Server={0};Database=master;User Id={1};Password={2};", repositoryName, userName, password)
                        : string.Format("Server={0};Database=master;Integrated Security=SSPI;", repositoryName);
                    connection.Open();

                    // check database exists
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = string.Format("select COUNT(name) from sys.databases where name = 'SQLcompliance'");
                        Int32 count = (Int32)command.ExecuteScalar();
                        if (count > 0)
                        {
                            complianceExists = true;
                        }
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = string.Format("select COUNT(name) from sys.databases where name = 'SQLcomplianceProcessing'");
                        Int32 count = (Int32)command.ExecuteScalar();
                        if (count > 0)
                        {
                            processignExists = true;
                        }
                    }

                    if ((processignExists && !complianceExists) || (!processignExists && complianceExists))
                    {
                        return true;
                    }

                    // check repository version
                    if (complianceExists && processignExists)
                    {
                        connection.ChangeDatabase("SQLcompliance");
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandType = CommandType.Text;
                            command.CommandText = string.Format("SELECT sqlComplianceDbSchemaVersion FROM Configuration");
                            object dbSchemaVersion = command.ExecuteScalar();
                            if (dbSchemaVersion != null)
                            {
                                if ((Int32)dbSchemaVersion > Constants.RepositoryDBSchemaVersion)
                                {
                                    return true;
                                }
                                else
                                {
                                    InstallProperties.UpgradeSchema = true;
                                    //return false;
                                    return true;
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return complianceExists;
            }

            return false;
        }

        public bool IsSQLComplianceDBCheck(bool isSQLAuthentication, string repositoryName, string userName, string password, out string errorMessage)
        {
            errorMessage = string.Empty;
            bool complianceExists = false;
            bool processignExists = false;
            string errorMessageOut = "";
            try
            {
                using (var connection = new SqlConnection())
                {
                    connection.ConnectionString = isSQLAuthentication
                        ? string.Format("Server={0};Database=master;User Id={1};Password={2};", repositoryName, userName, password)
                        : string.Format("Server={0};Database=master;Integrated Security=SSPI;", repositoryName);
                    connection.Open();


                    // check database exists
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = string.Format("select COUNT(name) from sys.databases where name = 'SQLcompliance'");
                        Int32 count = (Int32)command.ExecuteScalar();
                        if (count > 0)
                        {
                            complianceExists = true;
                        }
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = string.Format("select COUNT(name) from sys.databases where name = 'SQLcomplianceProcessing'");
                        Int32 count = (Int32)command.ExecuteScalar();
                        if (count > 0)
                        {
                            processignExists = true;
                        }
                    }

                    if ((processignExists && !complianceExists) || (!processignExists && complianceExists))
                    {
                        IsSQLComplianceDBRemove(isSQLAuthentication, repositoryName, userName, password);
                        return true;
                    }

                    // check repository version
                    if (complianceExists && processignExists)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return complianceExists;
            }

            return false;
        }

        public string IsSQLComplianceDBRemove(bool isSQLAuthentication, string repositoryName, string userName, string password)
        {
            string errorMessage = string.Empty;
            bool complianceExists = false;
            bool processignExists = false;
            try
            {
                using (var connection = new SqlConnection())
                {
                    connection.ConnectionString = isSQLAuthentication
                        ? string.Format("Server={0};Database=master;User Id={1};Password={2};", repositoryName, userName, password)
                        : string.Format("Server={0};Database=master;Integrated Security=SSPI;", repositoryName);
                    connection.Open();

                    // check database exists
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = string.Format("select COUNT(name) from sys.databases where name = 'SQLcompliance'");
                        Int32 count = (Int32)command.ExecuteScalar();
                        if (count > 0)
                        {
                            complianceExists = true;
                        }
                    }
                    if (complianceExists)
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandType = CommandType.Text;
                            command.CommandText = string.Format("DROP DATABASE SQLcompliance;");
                            Int32 count = (Int32)command.ExecuteNonQuery();
                        }
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = string.Format("select COUNT(name) from sys.databases where name = 'SQLcomplianceProcessing'");
                        Int32 count = (Int32)command.ExecuteScalar();
                        if (count > 0)
                        {
                            processignExists = true;
                        }
                    }
                    if (processignExists)
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandType = CommandType.Text;
                            command.CommandText = string.Format("DROP DATABASE SQLcomplianceProcessing;");
                            Int32 count = (Int32)command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return errorMessage;
        }
        public bool GetSQLServerServiceAccount(bool isSQLAuthentication, string repositoryName, string userName, string password, out string errorMessage)
        {
            errorMessage = string.Empty;
            string OnlyInstanceName = "";
            int version = 0;
            if (InstallProperties.CMSQLServerInstanceName.Contains("\\"))
            {
                OnlyInstanceName = InstallProperties.CMSQLServerInstanceName.Split('\\')[1];
            }
            else
            {
                OnlyInstanceName = "MSSQLSERVER";
            }
            try
            {
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = isSQLAuthentication
                        ? string.Format("Server={0};Database=master;User Id={1};Password={2};", repositoryName, userName, password)
                        : string.Format("Server={0};Database=master;Integrated Security=SSPI;", repositoryName);
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = string.Format("SELECT SERVERPROPERTY('PRODUCTVERSION');");
                        var scalar = command.ExecuteScalar();
                        if (scalar != null && scalar != DBNull.Value)
                        {
                            int.TryParse(scalar.ToString().Split('.')[0], out version);
                        }
                    }

                    if (version <= 10)
                    {
                        RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
                        using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
                        {
                            try
                            {
                                string instanceSQLServerKey;
                                if (OnlyInstanceName != "MSSQLSERVER")
                                {
                                    instanceSQLServerKey = @"SYSTEM\CurrentControlSet\Services\" + "MSSQL$" + OnlyInstanceName;
                                }
                                else
                                {
                                    instanceSQLServerKey = @"SYSTEM\CurrentControlSet\Services\" + "MSSQLSERVER";
                                }
                                RegistryKey servicesKey = hklm.OpenSubKey(instanceSQLServerKey, false);
                                if (servicesKey != null)
                                {
                                    InstallProperties.SQLServerUserName = servicesKey.GetValue("ObjectName").ToString().Replace(".", Environment.MachineName);
                                    if (InstallProperties.SQLServerUserName == "LocalSystem" || InstallProperties.SQLServerUserName == "LocalService")
                                    {
                                        InstallProperties.SQLServerUserName = InstallProperties.SQLServerUserName.Replace("Local", "NT AUTHORITY\\");
                                    }

                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            catch (Exception ex)
                            {
                                errorMessage = ex.Message;
                            }
                        }
                    }
                    else
                    {
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandType = CommandType.Text;
                            command.CommandText = string.Format("SELECT DSS.service_account FROM sys.dm_server_services AS DSS WHERE DSS.servicename = 'SQL Server ({0})';", OnlyInstanceName);
                            var result = command.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                            {
                                if (result.ToString().StartsWith("."))
                                {
                                    var regex = new Regex(Regex.Escape("."));
                                    InstallProperties.SQLServerUserName = regex.Replace(result.ToString(), Environment.MachineName, 1);
                                }

                                if (InstallProperties.SQLServerUserName == "LocalSystem" || InstallProperties.SQLServerUserName == "LocalService")
                                {
                                    InstallProperties.SQLServerUserName = InstallProperties.SQLServerUserName.Replace("Local", "NT AUTHORITY\\");
                                }

                                if (InstallProperties.SQLServerUserName == null)
                                {
                                    InstallProperties.SQLServerUserName = result.ToString();
                                }

                                return true;
                            }
                            else
                            {
                                errorMessage = "Unable to fetch SQL Server service account details.";
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return false;
        }

        public bool CheckServiceStatus(string machineName, string serviceName, out ServiceStatus status)
        {
            status = ServiceStatus.NotSpecified;
            try
            {
                ServiceController collectionService = new ServiceController(serviceName, machineName);
                ServiceControllerStatus serviceStatus = collectionService.Status;
                if (serviceStatus.Equals(ServiceControllerStatus.Stopped)
                    || serviceStatus.Equals(ServiceControllerStatus.StopPending)
                    || serviceStatus.Equals(ServiceControllerStatus.Paused)
                    || serviceStatus.Equals(ServiceControllerStatus.PausePending))
                {
                    status = ServiceStatus.NotRunning;
                    return false;
                }
                else
                {
                    status = ServiceStatus.OK;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message != null && ex.Message.Contains("was not found on computer"))
                {
                    status = ServiceStatus.NotFound;
                }
                else
                {
                    status = ServiceStatus.NotReachable;
                }
                return false;
            }

            return true;
        }

        public bool CheckLocalInstalledProduct(out Products installedProduct)
        {
            ServiceController[] services = null;
            try
            {
                bool collectionFound = false;
                services = ServiceController.GetServices(Environment.MachineName);
                if (services.FirstOrDefault(s => s.ServiceName == Constants.CollectionServiceName) != null)
                {
                    collectionFound = true;
                }

                if (services.FirstOrDefault(s => s.ServiceName == Constants.AgentServiceName) != null && !(InstallProperties.isSilentAgentInstalled))
                {
                    if (!collectionFound)
                    {
                        installedProduct = Products.Agent;
                        return true;
                    }
                    else
                    {
                        installedProduct = Products.Compliance;
                        return true;
                    }
                }
                else
                {
                    if (collectionFound)
                    {
                        installedProduct = Products.Compliance;
                        InstallProperties.Clustered = true;
                        return true;
                    }
                    else
                    {
                        installedProduct = Products.Console;
                        return true;
                    }
                }

            }
            finally
            {
                if (services != null)
                {
                    foreach (ServiceController controller in services)
                    {
                        controller.Dispose();
                    }
                }
            }
        }

        public bool ReadDataFromRegistry(RegistryHive baseKey, string subKeyPath, string valueToCheck, out string outputValue)
        {
            outputValue = string.Empty;
            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey baseRegistryKey = RegistryKey.OpenBaseKey(baseKey, registryView))
            {
                RegistryKey instanceKey = baseRegistryKey.OpenSubKey(subKeyPath, false);
                if (instanceKey != null)
                {
                    if (instanceKey.GetValue(valueToCheck) != null)
                    {
                        outputValue = instanceKey.GetValue(valueToCheck).ToString();
                        return true;
                    }
                }
            }

            return false;
        }

        public bool GetCWFDetailsFromDatabase(bool isSQLAuthentication, string repositoryName, string userName, string password, out string cwfURL, out string cwfToken, out string errorMessage)
        {
            errorMessage = string.Empty;
            cwfURL = string.Empty;
            cwfToken = string.Empty;

            try
            {
                using (var connection = new SqlConnection())
                {
                    connection.ConnectionString = isSQLAuthentication
                        ? string.Format("Server={0};Database=master;User Id={1};Password={2};", repositoryName, userName, password)
                        : string.Format("Server={0};Database=master;Integrated Security=SSPI;", repositoryName);
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = string.Format("SELECT TOP(1) [{0}], [{1}] FROM {2}", Constants.CWFURLRegKey, Constants.CWFTokenRegKey, Constants.CWFTableFullName);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cwfURL = reader.GetString(0);
                                cwfToken = reader.GetString(1);
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return false;
        }

        public void SetFolderAccess(bool throwRequired, string directory, string type, out string errorMessage)
        {
            errorMessage = string.Empty;
            bool currentUserAccess = false;
            bool serviceUserAccess = false;
            bool sqlServerUserAccess = false;

            try
            {
                var currentUserDirectoryInfo = new DirectoryInfo(directory);
                var currentUserDirectorySecurity = currentUserDirectoryInfo.GetAccessControl();
                AuthorizationRuleCollection authRuleCollection = currentUserDirectorySecurity.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
                var CurrentUserIdentity = WindowsIdentity.GetCurrent();

                if (authRuleCollection != null)
                {
                    for (int index = 0; index < authRuleCollection.Count; index++)
                    {
                        FileSystemAccessRule currentRule = (FileSystemAccessRule)authRuleCollection[index];
                        if (currentRule.IdentityReference.Value.Equals(CurrentUserIdentity.Name, StringComparison.OrdinalIgnoreCase)
                            && currentRule.AccessControlType == AccessControlType.Allow)
                        {
                            currentUserAccess = true;
                        }
                        if (!string.IsNullOrEmpty(InstallProperties.ServiceUserName)
                            && currentRule.IdentityReference.Value.Equals(InstallProperties.ServiceUserName, StringComparison.OrdinalIgnoreCase)
                            && currentRule.AccessControlType == AccessControlType.Allow)
                        {
                            serviceUserAccess = true;
                        }
                        if (!string.IsNullOrEmpty(InstallProperties.SQLServerUserName)
                            && currentRule.IdentityReference.Value.Equals(InstallProperties.SQLServerUserName, StringComparison.OrdinalIgnoreCase)
                            && currentRule.AccessControlType == AccessControlType.Allow)
                        {
                            sqlServerUserAccess = true;
                        }
                    }
                }

                try
                {
                    if (!currentUserAccess)
                    {
                        var currentUserfileSystemRule = new FileSystemAccessRule(CurrentUserIdentity.Name,
                                                                      FileSystemRights.FullControl,
                                                                      InheritanceFlags.ObjectInherit |
                                                                      InheritanceFlags.ContainerInherit,
                                                                      PropagationFlags.None,
                                                                      AccessControlType.Allow);

                        currentUserDirectorySecurity.AddAccessRule(currentUserfileSystemRule);
                    }
                }
                catch
                {
                    errorMessage = "An error was encountered while attempting to grant the current login account access to the IDERA SQL Compliance Manager " + type + ".";
                    throw;
                }

                try
                {
                    if (!string.IsNullOrEmpty(InstallProperties.ServiceUserName) && !serviceUserAccess)
                    {
                        var serviceUserfileSystemRule = new FileSystemAccessRule(InstallProperties.ServiceUserName,
                                                                      FileSystemRights.FullControl,
                                                                      InheritanceFlags.ObjectInherit |
                                                                      InheritanceFlags.ContainerInherit,
                                                                      PropagationFlags.None,
                                                                      AccessControlType.Allow);

                        currentUserDirectorySecurity.AddAccessRule(serviceUserfileSystemRule);
                    }
                }
                catch
                {
                    errorMessage = "An error was encountered while attempting to grant the IDERA SQL Compliance Manager service account access to the " + type + ".";
                    throw;
                }

                try
                {
                    if (!string.IsNullOrEmpty(InstallProperties.SQLServerUserName) && !sqlServerUserAccess)
                    {
                        var sqlServerServiceUserfileSystemRule = new FileSystemAccessRule(InstallProperties.SQLServerUserName,
                                                                      FileSystemRights.FullControl,
                                                                      InheritanceFlags.ObjectInherit |
                                                                      InheritanceFlags.ContainerInherit,
                                                                      PropagationFlags.None,
                                                                      AccessControlType.Allow);

                        currentUserDirectorySecurity.AddAccessRule(sqlServerServiceUserfileSystemRule);
                    }

                    if (!currentUserAccess || !serviceUserAccess || !sqlServerUserAccess)
                    {
                        currentUserDirectoryInfo.SetAccessControl(currentUserDirectorySecurity);
                    }
                }
                catch
                {
                    errorMessage = "An error was encountered while attempting to grant the SQL Server service account access to the IDERA SQL Compliance Manager " + type + ".";
                    throw;
                }
            }
            catch
            {
                if (throwRequired)
                {
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        errorMessage = "An error was encountered while attempting to get the access permissions of the IDERA SQL Compliance Manager " + type + ".";
                    }
                    throw;
                }
                else
                {
                    errorMessage += type + "\r\n";
                }
            }
        }
    }
}
