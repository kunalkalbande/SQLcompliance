using Idera.SQLcompliance.Core.Security;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Idera.SQLcompliance.Core.Configuration
{
    public sealed class SqlConnectionInfo : ISerializable
    {

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("timeout", connectionTimeout);
            info.AddValue("application", applicationName);
            info.AddValue("databaseName", databaseName);
            info.AddValue("instanceName", instanceName);
            info.AddValue("userName", userName);
            info.AddValue("password", EncryptedPassword);
            info.AddValue("integratedSecurity", useIntegratedSecurity);
            info.AddValue("allowAsynchronousCommands", allowAsynchronousCommands);
            info.AddValue("encryptData", encryptData);
            info.AddValue("trustServerCert", trustServerCert);
        }

        [AuditableAttribute(false)]
        public string EncryptedPassword
        {
            get
            {
                // this will produce a different value every time
                return Cipher.EncryptPassword(CipherInstanceName, password);
            }

            set { password = Cipher.DecryptPassword(CipherInstanceName, value); }
        }

        #region constants

        private const string CipherInstanceName = "Idera.SQLcm.Common";
        private const string DefaultApplicationName = "SQL Compliance Manager";
        private const string DefaultDatabaseName = "master";

        #endregion

        #region fields

        private static int defaultConnectionTimeout = 60;

        private int connectionTimeout = DefaultConnectionTimeout;
        private string applicationName = DefaultApplicationName;
        private string instanceName;
        private string databaseName = DefaultDatabaseName;
        private bool useIntegratedSecurity = true;
        private string userName;
        private string password;
        private bool allowAsynchronousCommands = true;
        private bool encryptData;
        private bool trustServerCert;

        #endregion

        #region constructors

        public SqlConnectionInfo()
        {
        }

        private SqlConnectionInfo(SerializationInfo info, StreamingContext context)
        {

            connectionTimeout = info.GetInt32("timeout");
            applicationName = info.GetString("application");
            databaseName = info.GetString("databaseName");
            instanceName = info.GetString("instanceName");
            userName = info.GetString("userName");
            //EncryptedPassword = info.GetString("password");
            useIntegratedSecurity = info.GetBoolean("integratedSecurity");
            allowAsynchronousCommands = info.GetBoolean("allowAsynchronousCommands");
            encryptData = info.GetBoolean("encryptData");
            trustServerCert = info.GetBoolean("trustServerCert");
        }

        /// <summary>
        /// Gets the connection string used when the connection is established with the instance of SQL Server.
        /// </summary>
        [AuditableAttribute(false)]
        public string ConnectionString
        {
            get { return GetConnectionString(); }
        }

        /// <summary>
        /// Initializes a new instance of the ServerConnection class using
        /// the specified server name and default Windows Integrated security.
        /// </summary>
        /// <param name="instanceName">The instanceName.</param>
        public SqlConnectionInfo(string instanceName)
        {
            InstanceName = instanceName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SqlConnectionInfo"/> class.
        /// </summary>
        /// <param name="instanceName">The instance name.</param>
        /// <param name="databaseName">The database name.</param>
        public SqlConnectionInfo(string instanceName, string databaseName)
            : this(instanceName)
        {
            DatabaseName = databaseName;
        }

        /// <summary>
        /// Initializes a new instance of the ServerConnection class using
        /// the specified computer name and sets up SQL Server authentication using
        /// the specifed username and password.
        /// </summary>
        /// <param name="instanceName">The instance name.</param>
        /// <param name="userName">The username.</param>
        /// <param name="password">The password to use for SQL Server authentication.</param>
        public SqlConnectionInfo(string instanceName, string userName, string password)
            : this(instanceName)
        {
            useIntegratedSecurity = false;
            UserName = userName;
            Password = password;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SqlConnectionInfo"/> class.
        /// </summary>
        /// <param name="instanceName">The instance name.</param>
        /// <param name="databaseName">The database name.</param>
        /// <param name="userName">The username.</param>
        /// <param name="password">The password.</param>
        public SqlConnectionInfo(string instanceName, string databaseName, string userName, string password)
            : this(instanceName, userName, password)
        {
            DatabaseName = databaseName;
        }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        [AuditableAttribute(false)]
        public string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        /// <summary>
        /// Gets or sets the number of seconds before a connection times out.
        /// </summary>
        [Auditable(false)]
        public int ConnectionTimeout
        {
            get { return connectionTimeout; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("connectionTimeout");

                connectionTimeout = value;
            }
        }


        #endregion


        #region properties

        /// <summary>
        /// Gets or sets a string that represents the login name to use for
        /// SQL Server authentication mode.
        /// </summary>
        [Auditable("Changed user name to")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// Gets or sets the password used to establish a connection.
        /// </summary>
        /// <value>The password.</value>
        [AuditableAttribute(true, true)]
        public string Password
        {
            set { password = value; }
            get { return password; }
        }

        public static int DefaultConnectionTimeout
        {
            get { return defaultConnectionTimeout; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("DefaultConnectionTimeout");

                defaultConnectionTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets whether Windows integrated security is used.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use integrated security]; otherwise, <c>false</c>.
        /// </value>
        [Auditable("Windows Integrated Authentication")]
        public bool UseIntegratedSecurity
        {
            get { return useIntegratedSecurity; }
            set { useIntegratedSecurity = value; }
        }

        /// <summary>
        /// Gets or sets the default database name.
        /// </summary>
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }
        
        /// <summary>
        /// Gets or sets the name of the instance of SQL Server.
        /// </summary>
        /// <value>The instanceName.</value>
        public string InstanceName
        {
            get { return instanceName; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("instanceName");
                instanceName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use asynchronous processing].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use asynchronous processing]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowAsynchronousCommands
        {
            get { return allowAsynchronousCommands; }
            set { allowAsynchronousCommands = value; }
        }

        /// <summary>
        /// Get or set a value that causes the client network provider to use SSL 
        /// when connecting to a server.
        /// </summary>
        [Auditable("Encrypted Connection with SSL")]
        public bool EncryptData
        {
            get { return encryptData; }
            set { encryptData = value; }
        }

        /// <summary>
        /// Get or set a value that causes the client network provider to skip (trust) 
        /// the certificate used when an SSL connection is negotiated between the client
        /// and the server.  Setting this to true will allow use of self-signed certificates.
        /// </summary>
        [Auditable("Trust Server Certificate Encryption")]
        public bool TrustServerCertificate
        {
            get { return trustServerCert; }
            set { trustServerCert = value; }
        }

        #endregion

        #region methods

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public SqlConnectionInfo Clone()
        {
            SqlConnectionInfo clone = new SqlConnectionInfo(InstanceName);
            clone.ApplicationName = ApplicationName;
            clone.DatabaseName = DatabaseName;
            clone.ConnectionTimeout = ConnectionTimeout;
            clone.AllowAsynchronousCommands = AllowAsynchronousCommands;
            clone.EncryptData = EncryptData;
            clone.TrustServerCertificate = TrustServerCertificate;

            if (!UseIntegratedSecurity)
            {
                clone.UseIntegratedSecurity = UseIntegratedSecurity;
                clone.UserName = UserName;
                clone.Password = Password;
            }

            return clone;
        }

        public SqlConnection GetConnection(string applicationName)
        {
            this.applicationName = applicationName;
            return new SqlConnection(GetConnectionString());
        }

        private string GetConnectionString()
        {
            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder();

            // Application name
            connectionString.ApplicationName = ApplicationName;

            // Server name
            connectionString.DataSource = InstanceName;

            // Database name
            connectionString.InitialCatalog = DatabaseName;

            // Authentication information
            if (UseIntegratedSecurity)
            {
                connectionString.IntegratedSecurity = true;
            }
            else
            {
                connectionString.UserID = UserName;
                connectionString.Password = Password;
            }

            // Connection Timeout
            connectionString.ConnectTimeout = ConnectionTimeout;

            //Asynchronous Command Support
            connectionString.AsynchronousProcessing = AllowAsynchronousCommands;

            //SSL Support
            connectionString.Encrypt = encryptData;
            if (encryptData)
            {
                connectionString.TrustServerCertificate = trustServerCert;
            }

            return connectionString.ConnectionString;
        }


        #endregion
        
    }
}


/// <summary>
/// Custom Attribute indicate which porperty needs a new name and indicate if this porperty is auditable
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Property)]
public class AuditableAttribute : Attribute
{

    private bool propetySensitive = false;

    /// <summary>
    /// Create a Auditable Attribute to rename the porperties and set IsAuditable true
    /// </summary>
    /// <param name="reName">New name for the porperty</param>
    public AuditableAttribute(string reName)
    {
        this.NewPropertyName = reName;
        this.IsAuditable = true;
        this.propetySensitive = false;
    }

    /// <summary>
    /// Create a Auditable Attribute to rename the porperty and set IsAuditable 
    /// </summary>
    /// <param name="reName">New name for the properties</param>
    /// <param name="propertySensitive">Indicate if the porperty is Insensitive</param>
    public AuditableAttribute(string reName, bool propertySensitive)
    {
        this.NewPropertyName = reName;
        this.IsAuditable = true;
        this.propetySensitive = propertySensitive;
    }

    /// <summary>
    /// Create a Auditable Attribute indicated if IsAuditable and set the null the new name
    /// </summary>
    /// <param name="isAuditable">Indicate if property is auditable</param>
    public AuditableAttribute(bool isAuditable)
    {
        this.NewPropertyName = null;
        this.IsAuditable = isAuditable;
        this.propetySensitive = false;
    }


    /// <summary>
    /// Create a Auditable Attribute indicated if IsAuditable and is sensitive also set the null the new name
    /// </summary>
    /// <param name="isAuditable">Indicate if property is auditable</param>
    /// <param name="propertySensitive">Indicate if property is Sensitive to show in log</param>
    public AuditableAttribute(bool isAuditable, bool propertySensitive)
    {
        this.NewPropertyName = null;
        this.IsAuditable = isAuditable;
        this.propetySensitive = propertySensitive;
    }

    /// <summary>
    /// The new property name
    /// </summary>
    public string NewPropertyName { get; set; }

    /// <summary>
    /// True to find the diference in PropertiesComparer and false to ignore the property
    /// </summary>
    public bool IsAuditable { get; set; }

    /// <summary>
    /// True to find if sensitive in PropertiesComparer and false to not log
    /// </summary>
    public bool IsPropetySensitive
    {
        get { return propetySensitive; }
    }

}