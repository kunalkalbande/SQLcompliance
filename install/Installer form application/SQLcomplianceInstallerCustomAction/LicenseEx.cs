using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using BBS.License;
using Microsoft.Deployment.WindowsInstaller;

namespace SQLcomplianceInstallerCustomAction
{
    public static class BbsLicenseConstants
    {
        public const string LicenseTypeProduction = "Production";
        public const string LicenseTypeTrial = "Trial";
        public const string LicenseTypeEnterprise = "Enterprise";
        public const string LicenseNoExpirationDate = "None";
        public const string LicenseExpired = "License Expired";
        public const string CombinedLicenses = "Resulting Combined Licenses";
        public const string CombinedLicensesMultiExpirationDates = "Multiple Expiration Dates";
        public const string CombinedLicensesMultiTypes = "Mixed Enterprise and {0}";
        public const string LicenseCountNA = "Not Applicable";
        public const string LicenseCountUnlimited = "Unlimited";
        public const int ExpirationDayToWarnProduction = 45;
        public const int ExpirationDayToWarnTrial = 7;
    }

    public class BbsProductLicense
    {
        #region constructor

        public BbsProductLicense(string connectionString, string scope, int productId, string productVersion, Session session)
        {
            _session = session;
            LicenseLoaded = false;
            m_connectionString = connectionString;
            OrginalScopeString = scope;
            m_productID = productId;
            m_productVersion = new Version(productVersion);

            LoadLicenses();
            // Force combined license creation
            IsProductLicensed();
        }

        #endregion

        #region Queries

        // Add
        public const string NonQueryAddLicense = @"SQLcompliance.dbo.isp_sqlcm_addlicense";
        public const string ParamLicenseKey = "@licensekey";
        // Get
        public const string ViewLicenses = @"SELECT licenseid, licensekey FROM SQLcompliance.dbo.Licenses";
        // Remove
        public const string NonQueryRemoveLicense = @"SQLcompliance.dbo.isp_sqlcm_removelicense";
        public const string ParamLicenseId = "@licenseid";

        private enum SQLLicenseColumns
        {
            ColLicenseID,
            ColLicenseKey,
            ColCreatedTM,
            ColCreatedBy
        }

        #endregion

        #region Data Structs

        // Handy structure for passing information back to "Manage SQLcompliance Licenses" form
        public struct LicenseData
        {
            public int daysToExpire;
            public string daysToExpireStr;
            public string expirationDateStr;
            public string forStr;
            public bool isAboutToExpire;
            public bool isTrial;
            public string key;
            public int licenseRepositoryID;
            public LicenseState licState;
            public int numLicensedServers;
            public string numLicensedServersStr;
            public Version ProductVersion;
            public string typeStr;

            public override string ToString()
            {
                return key;
            }

            public void Initialize()
            {
                ProductVersion = new Version(1, 0);
                licState = LicenseState.InvalidKey;
                numLicensedServers = 0;
                isTrial = true;
                licenseRepositoryID = 0;
                key = string.Empty;
                numLicensedServersStr = string.Empty;
                daysToExpireStr = string.Empty;
                expirationDateStr = string.Empty;
                forStr = string.Empty;
                typeStr = string.Empty;
                daysToExpire = 0;
                isAboutToExpire = false;
            }
        }

        // License error codes, allows UI to provide more meaningful messages
        public enum LicenseState
        {
            Valid,
            InvalidKey,
            InvalidProductID,
            InvalidScope,
            InvalidExpired,
            InvalidMixedTypes,
            InvalidDuplicateLicense,
            InvalidProductVersion
        }

        #endregion

        #region Fields

        private readonly Version SUPPORTED_PREVIOUS_LICENSE_VERSION = new Version(5, 0);
        private Session _session;
        private LicenseData m_CombinedLicensedData;
        private readonly string m_connectionString;
        private readonly int m_productID;
        private readonly Version m_productVersion;

        #endregion

        #region Properties

        public string OrginalScopeString { get; private set; }

        public List<LicenseData> Licenses { get; private set; }

        public LicenseData CombinedLicense
        {
            get { return m_CombinedLicensedData; }
        }

        /// <summary>
        ///     Loaded just means that the query for the license data was successful. It does not mean that any rows were returned.
        /// </summary>
        public bool LicenseLoaded { get; set; }

        #endregion

        #region Public Methods

        // Check to see if SQLcompliance is licensed
        // Returns the combined license results and list of all licenses;
        public bool IsProductLicensed()
        {
            var bInitialized = false;
            var bLicensed = false;
            m_CombinedLicensedData = new LicenseData();
            m_CombinedLicensedData.Initialize();

            foreach (var licData in Licenses)
            {
                if (!bInitialized)
                {
                    bLicensed = true;
                    m_CombinedLicensedData = licData;
                    bInitialized = true;
                }
                else
                {
                    m_CombinedLicensedData.key = BbsLicenseConstants.CombinedLicenses;

                    // production licenses of v5 and above
                    if (!licData.isTrial && licData.licState == LicenseState.Valid && licData.ProductVersion.Major >= 5)
                    {
                        if (licData.numLicensedServers == -1)
                        {
                            m_CombinedLicensedData.numLicensedServers = -1;
                            m_CombinedLicensedData.numLicensedServersStr =
                                CountToString(m_CombinedLicensedData.numLicensedServers);
                        }
                        else if (m_CombinedLicensedData.numLicensedServers != -1)
                        {
                            m_CombinedLicensedData.numLicensedServers += licData.numLicensedServers;
                            m_CombinedLicensedData.numLicensedServersStr =
                                CountToString(m_CombinedLicensedData.numLicensedServers);
                        }
                    }

                    // trial licenses of any version
                    else if (licData.isTrial && licData.licState == LicenseState.Valid)
                    {
                        if (licData.numLicensedServers == -1)
                        {
                            m_CombinedLicensedData.numLicensedServers = -1;
                            m_CombinedLicensedData.numLicensedServersStr =
                                CountToString(m_CombinedLicensedData.numLicensedServers);
                        }
                        else if (m_CombinedLicensedData.numLicensedServers != -1)
                        {
                            m_CombinedLicensedData.numLicensedServers += licData.numLicensedServers;
                            m_CombinedLicensedData.numLicensedServersStr =
                                CountToString(m_CombinedLicensedData.numLicensedServers);
                        }
                    }

                    if (m_CombinedLicensedData.expirationDateStr != licData.expirationDateStr)
                    {
                        if (licData.isAboutToExpire)
                        {
                            m_CombinedLicensedData.isAboutToExpire = true;
                        }
                        m_CombinedLicensedData.expirationDateStr =
                            BbsLicenseConstants.CombinedLicensesMultiExpirationDates;
                        m_CombinedLicensedData.daysToExpireStr =
                            BbsLicenseConstants.CombinedLicensesMultiExpirationDates;
                    }
                    if (m_CombinedLicensedData.forStr != licData.forStr)
                    {
                        m_CombinedLicensedData.forStr = string.Format(BbsLicenseConstants.CombinedLicensesMultiTypes,
                            OrginalScopeString);
                    }
                }
            }

            if (!bLicensed)
            {
                m_CombinedLicensedData.Initialize();
            }

            return bLicensed;
        }

        // Remove all license from the repository
        public void RemoveAllLicenses()
        {
            foreach (var licData in Licenses)
            {
                RemoveLicenseBatchMode(licData.licenseRepositoryID);
            }

            ReLoadLicenses();
        }

        public void RemoveLicense(int id)
        {
            try
            {
                // Open connection to repository and add server.
                using (var connection = new SqlConnection(m_connectionString))
                {
                    // Open the connection.
                    connection.Open();

                    // Setup remove job params.
                    var paramLicenseId = new SqlParameter(ParamLicenseId, id);

                    ExecuteNonQuery(connection, CommandType.StoredProcedure,
                        NonQueryRemoveLicense, paramLicenseId);
                }
            }
            catch (Exception ex)
            {
                CustomActions.WriteLog(_session, "Error: {0}\r\nStack Trace: {1}", ex.Message, ex.StackTrace);
            }

            ReLoadLicenses();
        }

        // Add validated license to repository. 
        public void AddLicense(string licenseString)
        {
            LicenseState licState;

            if (IsLicenseStringValid(licenseString, out licState))
            {
                try
                {
                    // Open connection to repository and add server.
                    using (var connection = new SqlConnection(m_connectionString))
                    {
                        // Open the connection.
                        connection.Open();

                        // Setup remove job params.
                        var paramLicenseKey = new SqlParameter(ParamLicenseKey, licenseString);

                        ExecuteNonQuery(connection, CommandType.StoredProcedure,
                            NonQueryAddLicense, paramLicenseKey);
                    }
                }
                catch (Exception ex)
                {
                    CustomActions.WriteLog(_session, "Error: {0}\r\nStack Trace: {1}", ex.Message, ex.StackTrace);
                }
            }
        }

        // Check if the given string is a valid trial license
        // UI calls this before accepting license string
        public bool IsLicenseStringTrial(string licenseStr)
        {
            var bTrial = false;

            if (!string.IsNullOrEmpty(licenseStr))
            {
                var lic = new BBSLic();
                var rc = lic.LoadKeyString(licenseStr);
                if (rc == LicErr.OK)
                {
                    var licState = IsLicenseValid(lic, licenseStr);
                    if (licState == LicenseState.Valid)
                    {
                        bTrial = lic.IsTrial;
                    }
                }
            }

            return bTrial;
        }

        // Check if the given string is a valid license
        // UI calls this before accepting license string
        public bool IsLicenseStringValid(string license, out LicenseState licState)
        {
            var bValid = false;
            licState = LicenseState.InvalidKey;

            if (!string.IsNullOrEmpty(license))
            {
                var lic = new BBSLic();
                var rc = lic.LoadKeyString(license);

                if (rc == LicErr.OK)
                {
                    // check for hacked license
                    if (!IsLicenseReasonable(lic)) return false;

                    licState = IsLicenseValid(lic, license);
                    bValid = licState == LicenseState.Valid ? true : false;
                }
            }

            return bValid;
        }

        //---------------------------------------------------------------------
        // IsLicenseReasonable - Our license key checksum is not solid so
        //                       you can change characters in the key and
        //                       still have a valid license. This could allow
        //                       a customer to bump up their license cound.
        //                       However the changes always create unresonable 
        //                       licenses like 1000s of seats. To avoid
        //                       problems of upgrading license DLL we just are
        //                       putting in a reasonableness check in the products
        //---------------------------------------------------------------------
        private bool IsLicenseReasonable(BBSLic license)
        {
            // Trials only valid for 0-90 days
            if (license.IsTrial)
            {
                if (license.IsPermanent) return false;
                if (license.DaysToExpiration < 0) return false;
                if (license.DaysToExpiration > 90) return false;
            }
            else // Purchase license only valid for 0-400 days or unlimited
            {
                if (license.DaysToExpiration < 0) return false;
                if (license.DaysToExpiration > 400 && !license.IsPermanent) return false;
            }

            // License only good for up to 500 licenses
            if (license.Limit1 < BBSLic.Unlimited) return false;
            if (license.Limit1 > 500) return false;
            if (license.Limit2 < -2 || license.Limit2 > 1)
                return false; // some products code limit 2 as 1 instead of unlimited

            return true;
        }

        // Generate a trial license
        public string GenerateTrialLicense()
        {
            var lic = new BBSLic();

            lic.IsTrial = true;
            lic.KeyID = 0;
            lic.DaysToExpiration = 14;
            lic.ProductID = (short) m_productID;
            lic.SetScopeHash(OrginalScopeString);
            lic.Limit1 = 5;
            lic.ProductVersion = m_productVersion;

            // generate new trial license based on current version
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            var v2 = new Version(v.Major, v.Minor);
            lic.ProductVersion = v2;

            var key = lic.GetKeyString(PW());

            return key;
        }

        public bool IsLicneseGoodForServerCount(int serverCnt)
        {
            var isOK = false;
            if (m_CombinedLicensedData.numLicensedServers == -1)
            {
                isOK = true;
            }
            else
            {
                isOK = (m_CombinedLicensedData.numLicensedServers >= serverCnt);
            }

            return isOK;
        }

        #endregion

        #region Helpers

        private static byte[] PW()
        {
            var currentProcess = Process.GetCurrentProcess();
            var data = currentProcess.MachineName + currentProcess.Id;
            return BBSLic.GetHash(data);
        }


        // Returns display string for the scope
        // License is validated when class is instanciated.
        // Since license is valid then scope is either enterprise or our repository
        private string GetLicenseScopeStr(BBSLic bbsLic)
        {
            var scope = string.Empty;
            if (bbsLic != null)
            {
                if (bbsLic.IsEnterprise)
                {
                    scope = BbsLicenseConstants.LicenseTypeEnterprise;
                }
                else
                {
                    scope = OrginalScopeString;
                }
            }
            return scope;
        }

        // Returns display string for type (Production or Trial)
        private string GetLicenseTypeStr(BBSLic bbsLic)
        {
            var type = string.Empty;
            if (bbsLic != null)
            {
                if (bbsLic.IsTrial)
                {
                    type = BbsLicenseConstants.LicenseTypeTrial;
                }
                else
                {
                    type = BbsLicenseConstants.LicenseTypeProduction;
                }
            }
            return type;
        }

        // Returns display string for expiration date (None if no expiration)
        private string GetLicenseExpirationDateStr(BBSLic bbsLic)
        {
            var date = string.Empty;
            if (bbsLic != null)
            {
                if (bbsLic.IsPermanent)
                {
                    date = BbsLicenseConstants.LicenseNoExpirationDate;
                }
                else
                {
                    date = bbsLic.ExpirationDate.ToShortDateString();
                }
            }
            return date;
        }

        // Returns display string for days to expiration or None if no expiration date
        private string GetLicenseDaysToExpirationStr(BBSLic bbsLic)
        {
            var days = string.Empty;
            if (bbsLic != null)
            {
                if (bbsLic.IsPermanent)
                {
                    days = BbsLicenseConstants.LicenseNoExpirationDate;
                }
                else if (bbsLic.IsExpired)
                {
                    days = BbsLicenseConstants.LicenseExpired;
                }
                else
                {
                    days = bbsLic.DaysToExpiration.ToString();
                }
            }
            return days;
        }

        // Returns display string for number of licensed servers
        private string GetLicenseCountStr(BBSLic bbsLic)
        {
            var count = string.Empty;
            if (bbsLic != null)
            {
                count = CountToString(bbsLic.Limit1);
            }
            return count;
        }


        private int GetLicenseCount(BBSLic bbsLic)
        {
            var count = 0;
            if (bbsLic != null)
            {
                count = bbsLic.Limit1;
            }
            return count;
        }

        private bool IsLicensePermament(BBSLic bbsLic)
        {
            var isPermament = false;
            if (bbsLic != null)
            {
                isPermament = bbsLic.IsPermanent;
            }
            return isPermament;
        }

        private bool IsLicenseTrial(BBSLic bbsLic)
        {
            var isTrial = false;
            if (bbsLic != null)
            {
                isTrial = bbsLic.IsTrial;
            }
            return isTrial;
        }

        private void ReLoadLicenses()
        {
            Licenses.Clear();
            LicenseLoaded = false;
            LoadLicenses();
        }

        // Get all licenses from repository
        private void LoadLicenses()
        {
            Licenses = new List<LicenseData>();
            try
            {
                using (var connection = new SqlConnection(m_connectionString))
                {
                    // Open the connection.
                    connection.Open();

                    //Get Schedule
                    var cmd = new SqlCommand(ViewLicenses, connection);
                    cmd.CommandType = CommandType.Text;

                    var da = new SqlDataAdapter(cmd);
                    var ds = new DataSet();
                    da.Fill(ds);
                    LicenseLoaded = true;

                    for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        var dr = ds.Tables[0].Rows[i];
                        var id = (int) dr[(int) SQLLicenseColumns.ColLicenseID];
                        var key = (string) dr[(int) SQLLicenseColumns.ColLicenseKey];

                        LicenseData licData;
                        FillLicenseData(key, out licData);
                        licData.licenseRepositoryID = id;
                        Licenses.Add(licData);
                    }
                }
            }

            catch (Exception ex)
            {
                LicenseLoaded = false;
                CustomActions.WriteLog(_session, "Error: {0}\r\nStack Trace: {1}", ex.Message, ex.StackTrace);
            }
        }

        // Remove specified license from repository, but don't refresh.
        // Refresh is done by calling code when all are removed.
        private void RemoveLicenseBatchMode(int id)
        {
            try
            {
                // Open connection to repository and add server.
                using (var connection = new SqlConnection(m_connectionString))
                {
                    // Open the connection.
                    connection.Open();

                    // Setup remove job params.
                    var paramLicenseId = new SqlParameter(ParamLicenseId, id);

                    ExecuteNonQuery(connection, CommandType.StoredProcedure,
                        NonQueryRemoveLicense, paramLicenseId);
                }
            }
            catch (Exception ex)
            {
                CustomActions.WriteLog(_session, "Error: {0}\r\nStack Trace: {1}", ex.Message, ex.StackTrace);
            }
        }

        // Fill the LicenseData structure with information about this license key
        private void FillLicenseData(string licenseKey, out LicenseData licData)
        {
            licData = new LicenseData();
            if (!string.IsNullOrEmpty(licenseKey))
            {
                BBSLic bbsLic;
                licData.Initialize();
                licData.licState = LoadAndValidateLicense(licenseKey, out bbsLic);
                if (bbsLic != null)
                {
                    licData.ProductVersion = bbsLic.ProductVersion;
                    licData.isTrial = IsLicenseTrial(bbsLic);
                    licData.key = licenseKey;
                    licData.numLicensedServers = GetLicenseCount(bbsLic);
                    licData.numLicensedServersStr = CountToString(licData.numLicensedServers);
                    licData.licenseRepositoryID = 0;
                    licData.forStr = GetLicenseScopeStr(bbsLic);
                    licData.typeStr = GetLicenseTypeStr(bbsLic);
                    licData.expirationDateStr = GetLicenseExpirationDateStr(bbsLic);
                    licData.daysToExpireStr = GetLicenseDaysToExpirationStr(bbsLic);
                    licData.daysToExpire = bbsLic.DaysToExpiration;
                    if (licData.typeStr == BbsLicenseConstants.LicenseTypeProduction &&
                        licData.daysToExpire <= BbsLicenseConstants.ExpirationDayToWarnProduction ||
                        licData.typeStr == BbsLicenseConstants.LicenseTypeTrial &&
                        licData.daysToExpire <= BbsLicenseConstants.ExpirationDayToWarnTrial)
                    {
                        licData.isAboutToExpire = true;
                    }
                    else
                    {
                        licData.isAboutToExpire = false;
                    }
                }
            }
        }

        // Checks if license is valid for SQLcompliance
        // Checks ProductID, Version, Scope, Expiration, Is Duplicate
        private LicenseState IsLicenseValid(BBSLic lic, string licenseKey)
        {
            var licState = LicenseState.InvalidKey;

            if (lic != null)
            {
                while (licState == LicenseState.InvalidKey)
                {
                    licState = LicenseState.Valid;

                    // Is the product ID for SQLcompliance
                    if (!IsLicenseProductIDValid(lic))
                    {
                        licState = LicenseState.InvalidProductID;
                        break;
                    }
                    // Is this for correct version
                    if (!IsLicenseVersionValid(lic))
                    {
                        licState = LicenseState.InvalidProductVersion;
                        break;
                    }
                    // Is it registered for this repository or enterprise
                    if (!IsLicenseScopeValid(lic))
                    {
                        licState = LicenseState.InvalidScope;
                        break;
                    }
                    // Is license expired
                    if (lic.IsExpired)
                    {
                        licState = LicenseState.InvalidExpired;
                        break;
                    }
                    // Does it already exist
                    var bDuplicate = false;
                    foreach (var licData2 in Licenses)
                    {
                        if (licData2.key == licenseKey)
                        {
                            bDuplicate = true;
                            break;
                        }
                    }
                    if (bDuplicate)
                    {
                        licState = LicenseState.InvalidDuplicateLicense;
                    }
                }
            }

            return licState;
        }

        // Is the scope hash valid for our repository
        private bool IsLicenseScopeValid(BBSLic lic)
        {
            var bValid = false;
            if (lic != null)
            {
                if (lic.IsEnterprise || lic.CheckScopeHash(OrginalScopeString))
                {
                    bValid = true;
                }
            }
            return bValid;
        }

        // Is the ProductID valid for SQLcompliance
        private bool IsLicenseProductIDValid(BBSLic lic)
        {
            var bValid = false;
            if (lic != null)
            {
                if (lic.ProductID == m_productID)
                {
                    bValid = true;
                }
            }
            return bValid;
        }

        // Is the Product Version valid for SQLcompliance
        private bool IsLicenseVersionValid(BBSLic lic)
        {
            if (lic == null)
            {
                return false;
            }

            // we are accepting trial licenses
            if (lic.IsTrial)
            {
                return true;
            }

            return lic.ProductVersion >= SUPPORTED_PREVIOUS_LICENSE_VERSION;
        }

        private string CountToString(int count)
        {
            if (count == BBSLic.NotApplicable)
            {
                return BbsLicenseConstants.LicenseCountNA;
            }
            if (count == BBSLic.Unlimited)
            {
                return BbsLicenseConstants.LicenseCountUnlimited;
            }
            return count.ToString();
        }


        private LicenseState LoadAndValidateLicense(string license, out BBSLic bbsLic)
        {
            var licState = LicenseState.InvalidKey;
            bbsLic = new BBSLic();
            var rc = bbsLic.LoadKeyString(license);
            if (rc == LicErr.OK)
            {
                licState = IsLicenseValid(bbsLic, license);
            }

            return licState;
        }

        #region SQLHelper

        private int ExecuteNonQuery(
            SqlConnection connection,
            CommandType commandType,
            string commandText,
            params SqlParameter[] commandParameters
            )
        {
            Debug.Assert(connection != null);

            // Create a command and prepare it for execution
            var retval = 0;
            using (var cmd = new SqlCommand())
            {
                // Prepare and execute the command.
                try
                {
                    // Create the command object.
                    prepareCommand(cmd, connection, null, commandType, commandText, commandParameters);

                    // Execute the command
                    retval = cmd.ExecuteNonQuery();

                    // Detach the SqlParameters from the command object, so they can be used again
                    // Detach the SqlParameters from the command object, so they can be used again.
                    // HACK: There is a problem here, the output parameter values are fletched 
                    // when the reader is closed, so if the parameters are detached from the command
                    // then the SqlReader can´t set its values. 
                    // When this happen, the parameters can´t be used again in other command.
                    var canClear = true;
                    foreach (SqlParameter commandParameter in cmd.Parameters)
                    {
                        if (commandParameter.Direction != ParameterDirection.Input)
                            canClear = false;
                    }

                    if (canClear)
                    {
                        cmd.Parameters.Clear();
                    }
                }
                catch (SqlException ex)
                {
                    CustomActions.WriteLog(_session, "Error: {0}\r\nStack Trace: {1}", ex.Message, ex.StackTrace);
                }

                return retval;
            }
        }

        /// <summary>
        ///     This method opens (if necessary) and assigns a connection, transaction, command type and parameters
        ///     to the provided command
        /// </summary>
        /// <param name="command">The SqlCommand to be prepared</param>
        /// <param name="connection">A valid SqlConnection, on which to execute this command</param>
        /// <param name="transaction">A valid SqlTransaction, or 'null'</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">
        ///     An array of SqlParameters to be associated with the command or 'null' if no parameters
        ///     are required
        /// </param>
        private static void prepareCommand(
            SqlCommand command,
            SqlConnection connection,
            SqlTransaction transaction,
            CommandType commandType,
            string commandText,
            SqlParameter[] commandParameters
            )
        {
            Debug.Assert(command != null);
            Debug.Assert(commandText != null || commandText.Length != 0);
            Debug.Assert(connection.State == ConnectionState.Open);
            Debug.Assert((transaction == null) ? true : transaction.Connection != null);

            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = commandType;

            // Attach the command parameters if they are provided
            if (commandParameters != null)
            {
                attachParameters(command, commandParameters);
            }
        }

        /// <summary>
        ///     This method is used to attach array of SqlParameters to a SqlCommand.
        ///     This method will assign a value of DbNull to any parameter with a direction of
        ///     InputOutput and a value of null.
        ///     This behavior will prevent default values from being used, but
        ///     this will be the less common case than an intended pure output parameter (derived as InputOutput)
        ///     where the user provided no input value.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">An array of SqlParameters to be added to command</param>
        private static void attachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            Debug.Assert(command != null);

            if (commandParameters != null)
            {
                foreach (var p in commandParameters)
                {
                    if (p != null)
                    {
                        // Check for derived output value with no value assigned
                        if ((p.Direction == ParameterDirection.InputOutput ||
                             p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}