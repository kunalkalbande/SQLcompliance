using System;
using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Idera.SQLcompliance.Core;
using Microsoft.Deployment.WindowsInstaller;
using BBS.License;
using System.Data.OleDb;

namespace SQLcomplianceInstallerCustomAction
{
    public static class CustomActions
    {
        #region declares

        private const string SupportDirectory = "SUPPORTDIR";
        private const string LicenseKeyOld = "LICENSE_OLD_KEY";
        private const string LicenseKeyNew = "LICENSE_NEW_KEY";
        private const string LicenseNewAdd = "LICENSE_NEW_ADD";
        private const string LicenseOldRemove = "LICENSE_OLD_REMOVE";
        private const string InstancesMonitered = "LICENSES_MONITERED";
        private const string InstancesTotal = "LICENSES_TOTAL";
        private const string SqlServer = "REPOSITORY";
        private const string SqlServerAuthentication = "IS_SQLSERVER_AUTHENTICATION";
        private const string SqlServerUser = "IS_SQLSERVER_USERNAME";
        private const string SqlServerPassword = "IS_SQLSERVER_PASSWORD";

        #endregion

        [CustomAction]
        public static ActionResult GetAndValidateOldLicense(Session session)
        {
            // Point to support files directory as all reference assemblies are deployed by InstallShield temporarily there.
            // Also we are using x86 license4net.dll, x64 is not working in installer.
            Environment.CurrentDirectory = session[SupportDirectory];

            WriteLog(session, "Getting and validating old license from repository...");

            var combinedLicense = CreateCombinedLicense(session);
            if (combinedLicense != null)
            {
                session[InstancesTotal] = combinedLicense.CombinedLicense.numLicensedServersStr;
                session[InstancesMonitered] = GetMoniteredServerCount(session).ToString();
            }

            WriteLog(session, "Old license validation complete.");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ValidateNewLicense(Session session)
        {
            WriteLog(session, "Validating new license key...");

            var newLicenseKey = session[LicenseKeyNew];

            WriteLog(session, AddNewLicense(session, newLicenseKey) ?
                    "New license key marked for addition to repository." :
                    "Failed to add new license key.");

            WriteLog(session, "New license key validation complete.");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult CopyOldLicenseToClipboard(Session session)
        {
            WriteLog(session, "Copying old license to clipboard...");

            try
            {
                var oldLicenseDetails = new StringBuilder();
                oldLicenseDetails.AppendFormat("License Keys: {0}\r\n", session[LicenseKeyOld]);
                oldLicenseDetails.AppendFormat("Total Licenses: {0}\r\n", session[InstancesTotal]);
                oldLicenseDetails.AppendFormat("Monitered Instances: {0}\r\n", session[InstancesMonitered]);

                // we are using native method to copy to clipboard as Clipboard.SetText() don't work here
                Native.OpenClipboard(IntPtr.Zero);
                Native.EmptyClipboard();
                var ptr = Marshal.StringToHGlobalUni(oldLicenseDetails.ToString());
                Native.SetClipboardData(13, ptr);
                Native.CloseClipboard();
                Marshal.FreeHGlobal(ptr);

                ShowMessage(false, "Old license details copied to clipboard.");
            }
            catch (Exception ex)
            {
                WriteLog(session, "Failed to copy license information to clipboard.\r\nError: {0}\r\nStack Trace: {1}", ex.Message, ex.StackTrace);
                ShowMessage(true, "Failed to copy license information to clipboard.");   
            }

            WriteLog(session, "Old license copied to clipboard.");
            return ActionResult.Success;
        }

        #region private methods

        private static void ShowMessage(bool isError, string messageFormat, params object[] parameters)
        {
            MessageBox.Show(
                string.Format(messageFormat, parameters), 
                "SQL Compliance Manager", 
                MessageBoxButtons.OK,
                isError ? MessageBoxIcon.Error : MessageBoxIcon.Information);
        }

        internal static void WriteLog(Session session, string messageFormat, params object[] parameters)
        {
            session.Log(messageFormat, parameters);
        }

        static string GetConnectionString(Session session)
        {
            var connectionStringBuilder = new OleDbConnectionStringBuilder();
            connectionStringBuilder.DataSource = session[SqlServer];
            connectionStringBuilder.Provider = "SQLNCLI11";
            connectionStringBuilder.Add("Database", "master");

            if (session[SqlServerAuthentication].Equals("0"))
            {
                connectionStringBuilder.Add("Trusted_Connection", "yes");

                WriteLog(session, "Connection String: {0}", connectionStringBuilder.ConnectionString);
                return connectionStringBuilder.ConnectionString;
            }
            else
            {
                connectionStringBuilder.Add("Uid", session[SqlServerUser]);
                connectionStringBuilder.Add("Pwd", session[SqlServerPassword]);

                WriteLog(session, "Connection String: {0}", connectionStringBuilder.ConnectionString);
                return connectionStringBuilder.ConnectionString;
            }
        }
		
        private static bool AddNewLicense(Session session, string licenseKey)
        {
            session[LicenseOldRemove] = "0";
            session[LicenseNewAdd] = "0";

            if (string.IsNullOrEmpty(licenseKey))
            {
                ShowMessage(true, "License key not entered.");
                return false;
            }

            var isLicenseValid = true;
            BbsProductLicense.LicenseState licState;

            var combinedLicense = CreateCombinedLicense(session);
            if (!combinedLicense.IsLicenseStringValid(licenseKey, out licState))
            {
                string errorMessage;
                switch (licState)
                {
                    case BbsProductLicense.LicenseState.InvalidKey:
                        errorMessage = string.Format(CoreConstants.LicenseInvalid, licenseKey);
                        break;

                    case BbsProductLicense.LicenseState.InvalidExpired:
                        errorMessage = string.Format(CoreConstants.LicenseExpired);
                        break;

                    case BbsProductLicense.LicenseState.InvalidProductID:
                        errorMessage = string.Format(CoreConstants.LicenseInvalidProductID);
                        break;

                    case BbsProductLicense.LicenseState.InvalidProductVersion:
                        errorMessage = string.Format(CoreConstants.LicenseInvalidProductVersion);
                        break;

                    case BbsProductLicense.LicenseState.InvalidScope:
                        errorMessage = string.Format(CoreConstants.LicenseInvalidRepository, GetScope(session));
                        break;

                    case BbsProductLicense.LicenseState.InvalidDuplicateLicense:
                        // we don't want to check for duplicate license
                        // if duplicate license is entered, just accept it
                        goto SKIP_DUPLICATE_MESSAGE_CHECK;
                        /*errorMessage = string.Format(CoreConstants.LicenseInvalidDuplicate);*/
                        break;

                    default:
                        errorMessage = string.Format(CoreConstants.LicenseInvalid, licenseKey);
                        break;
                }
                isLicenseValid = false;
                ShowMessage(true, errorMessage);
            }

            SKIP_DUPLICATE_MESSAGE_CHECK:
            if (!isLicenseValid)
                return false;

            if (!combinedLicense.CombinedLicense.isTrial)
            {
                if (combinedLicense.IsLicenseStringTrial(licenseKey))
                {
                    isLicenseValid = false;
                    ShowMessage(true, CoreConstants.CantAddTrialToPermamentLicense);
                }
            }

            if (!isLicenseValid || !IsAllInstancesCoveredByLicense(session, licenseKey))
            {
                return false;
            }

            if (combinedLicense.CombinedLicense.licState != BbsProductLicense.LicenseState.Valid ||
                combinedLicense.CombinedLicense.isTrial)
            {
                WriteLog(session, "Set flag to remove all existing license keys.");
                session[LicenseOldRemove] = "1";
            }

            WriteLog(session, "Set flag to add new license key.");
            session[LicenseNewAdd] = "1";
            return true;
        }

        private static bool IsAllInstancesCoveredByLicense(Session session, string newLicense)
        {
            var lic = new BBSLic();
            lic.LoadKeyString(newLicense);
            if (lic.Limit1 == -1) return true;
            if (lic.Limit1 >= GetMoniteredServerCount(session)) return true;
            ShowMessage(true, CoreConstants.LicenseCantCoverAllInstances);
            return false;
        }

        private static string GetScope(Session session)
        {
            // fix SQL server instance name in case of local instance
            var scope = session[SqlServer].ToLower();
            if (scope.StartsWith("(local)"))
                scope = scope.Replace("(local)", Environment.MachineName);
            else if (scope.StartsWith("."))
                scope = scope.Replace(".", Environment.MachineName);

            return scope;
        }

        private static BbsProductLicense CreateCombinedLicense(Session session)
        {
            WriteLog(session, "Creating combined license from keys in repository...");
            var licenseKeysBuilder = new StringBuilder();
            BbsProductLicense license = null;

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var versionString = string.Format("{0}.{1}", version.Major, version.Minor);

            try
            {
                license = new BbsProductLicense(GetConnectionString(session), GetScope(session), 1400, versionString, session);
                foreach (var licenseData in license.Licenses)
                    licenseKeysBuilder.AppendFormat("{0};", licenseData.key);

                WriteLog(session, "Total {0} license keys found in repository.", license.Licenses.Count);
            }
            catch (OleDbException ex)
            {
                WriteLog(session, "Failed to create combined license from keys in repository.\r\nError: {0}\r\nStack Trace: {1}", ex.Message, ex.StackTrace);
                licenseKeysBuilder.Remove(0, licenseKeysBuilder.Length);
                ShowMessage(true, "Failed to get license keys from repository.");
            }

            session[LicenseKeyOld] = licenseKeysBuilder.ToString();
            return license;
        }

        private static int GetMoniteredServerCount(Session session)
        {
            WriteLog(session, "Getting count of monitered servers from repository...");
            var moniteredServers = 0;

            using (var connection = new OleDbConnection(GetConnectionString(session)))
            {
                try
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT COUNT(*) AS 'MoniteredServers' FROM [SQLcompliance]..[Servers]";
                        moniteredServers = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
                catch (OleDbException ex)
                {
                    WriteLog(session, "Failed to get monitered sercvers count from repository.\r\nError: {0}\r\nStack Trace: {1}", ex.Message, ex.StackTrace);
                    ShowMessage(true, "Failed to get monitered sercvers count from repository.");
                    moniteredServers = 0;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                        connection.Close();

                    WriteLog(session, "Total {0} monitered servers found in repository.", moniteredServers);
                }
            }

            session[InstancesMonitered] = moniteredServers.ToString();
            return moniteredServers;
        }

        #endregion
    }
}
