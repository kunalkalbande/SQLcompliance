using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Status;
using Idera.SQLcompliance.Core.TimeZoneHelper;
using TimeZoneInfo = Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;

namespace Idera.SQLcompliance.Core.Agent
{
    /// <summary>
    /// Summary description for AgentConfiguration.
    /// </summary>
    [Serializable]
    public class AgentConfiguration : ISerializable
    {
        #region Constants

        private const string dateFieldSeparator = ":";
        private const string invariantDateFormat = "{0}:{1}:{2}:{3}:{4}:{5}"; // yyyy:MM:dd:hh:mm:ss

        #endregion

        #region Private Data members

        private Hashtable sqlServerStatusList;
        private string collectionServer;
        private int serverPort;
        private int agentPort;

        private int collectionInterval;
        private int forceCollectionInterval;
        private int traceStartTimeout;
        private int heartbeatInterval;
        private int detectionInterval;

        private bool isRunning;
        private bool isCrippled;
        private bool isEnabled;
        private bool isDeployed = true;
        private bool serverUpdated = false;
        private bool dbSchemaVersionChecked = false;


        private string agentVersion;
        private string agentName;
        private string agentServer;
        private string serviceAccount;
        private DateTime lastUpdateTime;      // not used anymore
        private DateTime lastCollectionTime;  // not used anymore
        private DateTime startupTime;         // not used anymore

        private int bias;
        private int standardBias;
        private DateTime standardDate;
        private int daylightBias;
        private DateTime daylightDate;

        private int configVersion;
        private int logLevel;
        private int sqlComplianceDbSchemaVersion = CoreConstants.RepositorySqlComplianceDbSchemaVersion;
        private int eventsDbSchemaVersion = CoreConstants.RepositoryEventsDbSchemaVersion;

        // Trace settings
        private int maxUnattendedTime = CoreConstants.Agent_Default_MaxUnattendedTime;
        private int maxFolderSize = CoreConstants.Agent_Default_MaxFolderSize;
        private int maxTraceSize;
        private TraceOption traceOptions;
        private string traceDirectory;
        private bool isCompressedFile;

        #endregion

        #region Internal fields

        // Versioin number for serializatioin
        internal int classVersion = CoreConstants.SerializationVersion;

        #endregion

        #region Properties

        public InstanceStatus[] InstanceStatusList
        {
            get
            {
                int i = 0;
                if (sqlServerStatusList == null)
                    return new InstanceStatus[0];

                InstanceStatus[] instanceStatusList = new InstanceStatus[sqlServerStatusList.Count];
                IDictionaryEnumerator enumerator = sqlServerStatusList.GetEnumerator();
                if (enumerator != null)
                {
                    while (enumerator.MoveNext())
                    {
                        instanceStatusList[i++] = (InstanceStatus)enumerator.Value;
                    }
                }
                else
                    instanceStatusList = new InstanceStatus[0];

                return instanceStatusList;
            }

            set
            {
                if (value == null)
                    return;

                foreach (InstanceStatus instanceStatus in value)
                    sqlServerStatusList.Add(instanceStatus.Instance, instanceStatus);
            }
        }

        public string Server
        {
            get { return collectionServer; }
            set { collectionServer = value; }
        }

        public int ServerPort
        {
            get { return serverPort; }
            set { serverPort = value; }
        }

        public string AgentServer
        {
            get { return agentServer; }
            set { agentServer = value; }
        }

        public int AgentPort
        {
            get { return agentPort; }
            set { agentPort = value; }
        }

        public string AgentName
        {
            get { return agentName; }
        }

        public string AgentVersion
        {
            get { return agentVersion; }
        }

        public bool IsRunning
        {
            get { return isRunning; }
            set { isRunning = value; }
        }

        public bool IsCrippled
        {
            get { return isCrippled; }
            set { isCrippled = value; }
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }

        public bool IsDeployed
        {
            get { return isDeployed; }
            set { isDeployed = value; }
        }

        public string ServiceAccount
        {
            get { return serviceAccount; }
            set { serviceAccount = value; }
        }

        public DateTime LastUpdateTime
        {
            get { return lastUpdateTime; }
            set { lastUpdateTime = value; }
        }

        public DateTime LastCollectionTime
        {
            get { return lastCollectionTime; }
            set { lastCollectionTime = value; }
        }

        public DateTime StartupTime
        {
            get { return startupTime; }
            set { startupTime = value; }
        }

        public int ConfigVersion
        {
            get { return configVersion; }
            set { configVersion = value; }
        }

        public bool ServerUpdated
        {
            get { return serverUpdated; }
            set { serverUpdated = value; }
        }

        public int LogLevel
        {
            get { return logLevel; }
            set { logLevel = value; }
        }

        public int SqlComplianceDbSchemaVersion
        {
            get { return sqlComplianceDbSchemaVersion; }
        }

        public int EventsDbSchemaVersion
        {
            get { return eventsDbSchemaVersion; }
        }

        public bool DbSchemaVersionChecked
        {
            get { return dbSchemaVersionChecked; }
            set { dbSchemaVersionChecked = value; }
        }

        public int HeartbeatInterval
        {
            get { return heartbeatInterval; }
            set { heartbeatInterval = value; }
        }

        public bool IsCompressedFile
        {
            get { return isCompressedFile; }
            set { isCompressedFile = value; }
        }

        public int CollectionInterval
        {
            get { return collectionInterval; }
            set { collectionInterval = value; }
        }

        public int ForceCollectionInterval
        {
            get { return forceCollectionInterval; }
            set { forceCollectionInterval = value; }
        }

        public int TraceStartTimeout
        {
            get { return traceStartTimeout; }
            set { traceStartTimeout = value; }
        }

        public int DetectionInterval
        {
            get { return detectionInterval; }
            set { detectionInterval = value; }
        }

        public int MaxUnattendedTime
        {
            get { return maxUnattendedTime; }
            set { maxUnattendedTime = value; }
        }

        public int MaxFolderSize
        {
            get { return maxFolderSize; }
            set { maxFolderSize = value; }
        }

        public int MaxTraceSize
        {
            get { return maxTraceSize; }
            set { maxTraceSize = value; }
        }

        public TraceOption TraceOptions
        {
            get { return traceOptions; }
            set { traceOptions = value; }
        }

        public string TraceDirectory
        {
            get { return traceDirectory; }
            set { traceDirectory = value; }
        }

        public int Bias
        {
            get { return bias; }
            set { bias = value; }
        }
        public int StandardBias
        {
            get { return standardBias; }
            set { standardBias = value; }
        }
        public DateTime StandardDate
        {
            get { return standardDate; }
            set { standardDate = value; }
        }
        public int DaylightBias
        {
            get { return daylightBias; }
            set { daylightBias = value; }
        }
        public DateTime DaylightDate
        {
            get { return daylightDate; }
            set { daylightDate = value; }
        }


        #endregion

        #region Constructors

        public AgentConfiguration()
           : this(CoreConstants.Agent_Default_Server, CoreConstants.CollectionServerTcpPort) { }

        public AgentConfiguration(
           string collectionServer,
           int port
           )
        {
            agentName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name.ToString();
            agentVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
            string location = System.Reflection.Assembly.GetEntryAssembly().Location.ToString();
            FileInfo fileInfo = new FileInfo(location);

            traceDirectory = Path.Combine(fileInfo.DirectoryName,
                                           CoreConstants.Agent_Default_TraceDirectory);

            this.collectionServer = collectionServer;
            this.serverPort = port;

            agentServer = System.Net.Dns.GetHostName().ToUpper();

            // We don't expect to have a huge number of instances so use a small prime number
            sqlServerStatusList = new Hashtable(7);
            UpdateTimeZoneInfo();
            logLevel = 0;
        }


        //--------------------------------------
        // Custom deserialization constructor
        //--------------------------------------
        protected AgentConfiguration(
           SerializationInfo info,
           StreamingContext context)
        {
            try
            {
                try
                {
                    classVersion = info.GetInt32("classVersion");
                }
                catch
                {
                    // There is no class version prior to V 2.0
                    classVersion = 0;
                }

                // 1.1 and 1.2 agent fields
                sqlServerStatusList = info.GetValue("sqlServerStatusList",
                                                                typeof(Hashtable)) as Hashtable;
                collectionServer = info.GetString("collectionServer");
                serverPort = info.GetInt32("serverPort");
                agentPort = info.GetInt32("agentPort");

                collectionInterval = info.GetInt32("collectionInterval");
                forceCollectionInterval = info.GetInt32("forceCollectionInterval");
                heartbeatInterval = info.GetInt32("heartbeatInterval");

                isRunning = info.GetBoolean("isRunning");
                isCrippled = info.GetBoolean("isCrippled");
                isEnabled = info.GetBoolean("isEnabled");
                isDeployed = info.GetBoolean("isDeployed");
                serverUpdated = info.GetBoolean("serverUpdated");
                dbSchemaVersionChecked = info.GetBoolean("dbSchemaVersionChecked");


                agentVersion = info.GetString("agentVersion");
                agentName = info.GetString("agentName");
                agentServer = info.GetString("agentServer");
                serviceAccount = info.GetString("serviceAccount");

                bias = info.GetInt32("bias");
                standardBias = info.GetInt32("standardBias");
                daylightBias = info.GetInt32("daylightBias");

                configVersion = info.GetInt32("configVersion");
                logLevel = info.GetInt32("logLevel");
                sqlComplianceDbSchemaVersion = info.GetInt32("sqlComplianceDbSchemaVersion");
                eventsDbSchemaVersion = info.GetInt32("eventsDbSchemaVersion");

                maxUnattendedTime = info.GetInt32("maxUnattendedTime");
                maxFolderSize = info.GetInt32("maxFolderSize");
                maxTraceSize = info.GetInt32("maxTraceSize");
                traceOptions = (TraceOption)info.GetValue("traceOptions", typeof(TraceOption));
                traceDirectory = info.GetString("traceDirectory");

                // Version 2.0 agent fields
                if (classVersion >= CoreConstants.SerializationVersion_20)
                {
                    // Deserialize V 2.0 fields here
                    daylightDate = GetDateTimeFromString(info.GetString("daylightDateString"));
                    standardDate = GetDateTimeFromString(info.GetString("standardDateString"));
                    lastUpdateTime = new DateTime(info.GetInt64("lastUpdateTime"));      // not used anymore
                    lastCollectionTime = new DateTime(info.GetInt64("lastCollectionTime"));  // not used anymore
                    startupTime = new DateTime(info.GetInt64("startupTime"));         // not used anymore
                }
                else
                {
                    // special handling for 1.* fields
                    daylightDate = info.GetDateTime("daylightDate");
                    standardDate = info.GetDateTime("standardDate");
                    lastUpdateTime = info.GetDateTime("lastUpdateTime");      // not used anymore
                    lastCollectionTime = info.GetDateTime("lastCollectionTime");  // not used anymore
                    startupTime = info.GetDateTime("startupTime");         // not used anymore
                                                                           // Agent is 1.1 or 1.2, assign default values for fields added since 2.0
                }

                if (classVersion >= CoreConstants.SerializationVersion_21)
                    detectionInterval = info.GetInt32("detectionInterval");

                if (classVersion >= CoreConstants.SerializationVersion_33)
                {
                    traceStartTimeout = info.GetInt32("traceStartTimeout");
                }
                else
                {
                    traceStartTimeout = CoreConstants.Agent_Default_TraceStartTimeout;
                }
                Debug.WriteLine(String.Format("{0} deserializaed", this.GetType()));
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowDeserializationException(e, this.GetType());
            }

        }

        // Special handling for deserializing DateTime which is not local time
        private DateTime GetDateTimeFromString(string s)
        {
            string[] parts = s.Split(dateFieldSeparator.ToCharArray());
            return new DateTime(Convert.ToInt32(parts[0]),
                                 Convert.ToInt32(parts[1]),
                                 Convert.ToInt32(parts[2]),
                                 Convert.ToInt32(parts[3]),
                                 Convert.ToInt32(parts[4]),
                                 Convert.ToInt32(parts[5]),
                                 0);
        }

        private string GetDateTimeString(DateTime d)
        {
            return String.Format(invariantDateFormat, d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second);
        }

        #endregion

        #region Public Methods

        public void
           AddInstanceStatus(
              InstanceStatus instanceStatus
           )
        {
            try
            {
                if (instanceStatus != null)
                {
                    //instanceStatus.IsClustered = SQLcomplianceAgent.Instance.IsClustered;
                    sqlServerStatusList.Add(instanceStatus.Instance, instanceStatus);
                }
            }
            catch (ArgumentException)
            {
                // Ignore duplicate key error
            }
        }

        public void
           RemoveInstanceStatus(
              string instanceName
           )
        {
            sqlServerStatusList.Remove(instanceName);
        }

        public InstanceStatus
           GetInstanceStatus(
              string instanceName
           )
        {
            return (InstanceStatus)sqlServerStatusList[instanceName];
        }

        public void
           UpdateTimeZoneInfo()
        {
            TimeZoneInfo tzi = TimeZoneInfo.CurrentTimeZone;
            bias = tzi.TimeZoneStruct.Bias;
            standardBias = tzi.TimeZoneStruct.StandardBias;
            standardDate = SystemTime.ToTimeZoneDateTime(tzi.TimeZoneStruct.StandardDate);
            daylightBias = tzi.TimeZoneStruct.DaylightBias;
            daylightDate = SystemTime.ToTimeZoneDateTime(tzi.TimeZoneStruct.DaylightDate);
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(
           SerializationInfo info,
           StreamingContext context)
        {
            try
            {
                // This class is sent from the agent to the server.  We don't care about
                // backward compatibility during serialization.  Note that 1.1 and 1.2 
                // assemblies doesn't have this field.
                info.AddValue("classVersion", classVersion);

                // 1.1 and 1.2 fields
                info.AddValue("sqlServerStatusList", sqlServerStatusList);
                info.AddValue("collectionServer", collectionServer);
                info.AddValue("serverPort", serverPort);
                info.AddValue("agentPort", agentPort);

                info.AddValue("collectionInterval", collectionInterval);
                info.AddValue("forceCollectionInterval", forceCollectionInterval);
                info.AddValue("heartbeatInterval", heartbeatInterval);

                info.AddValue("isRunning", isRunning);
                info.AddValue("isCrippled", isCrippled);
                info.AddValue("isEnabled", isEnabled);
                info.AddValue("isDeployed", isDeployed);
                info.AddValue("serverUpdated", serverUpdated);
                info.AddValue("dbSchemaVersionChecked", dbSchemaVersionChecked);

                info.AddValue("agentVersion", agentVersion);
                info.AddValue("agentName", agentName);
                info.AddValue("agentServer", agentServer);
                info.AddValue("serviceAccount", serviceAccount);

                info.AddValue("bias", bias);
                info.AddValue("standardBias", standardBias);
                info.AddValue("daylightBias", daylightBias);

                info.AddValue("configVersion", configVersion);
                info.AddValue("logLevel", logLevel);
                info.AddValue("sqlComplianceDbSchemaVersion", sqlComplianceDbSchemaVersion);
                info.AddValue("eventsDbSchemaVersion", eventsDbSchemaVersion);

                info.AddValue("maxUnattendedTime", maxUnattendedTime);
                info.AddValue("maxFolderSize", maxFolderSize);
                info.AddValue("maxTraceSize", maxTraceSize);
                info.AddValue("traceOptions", traceOptions);
                info.AddValue("traceDirectory", traceDirectory);

                // 2.0 fields
                // None so far.
                if (classVersion >= CoreConstants.SerializationVersion_20)
                {
                    // special handling for DateTime
                    info.AddValue("standardDateString", GetDateTimeString(standardDate));
                    info.AddValue("daylightDateString", GetDateTimeString(daylightDate));
                    info.AddValue("lastUpdateTime", lastUpdateTime.Ticks);
                    info.AddValue("lastCollectionTime", lastCollectionTime.Ticks);
                    info.AddValue("startupTime", startupTime.Ticks);
                }
                else
                {
                    info.AddValue("standardDate", standardDate);
                    info.AddValue("daylightDate", daylightDate);
                    info.AddValue("lastUpdateTime", lastUpdateTime);
                    info.AddValue("lastCollectionTime", lastCollectionTime);
                    info.AddValue("startupTime", startupTime);
                }

                if (classVersion >= CoreConstants.SerializationVersion_21)
                    info.AddValue("detectionInterval", detectionInterval);

                if (classVersion >= CoreConstants.SerializationVersion_33)
                {
                    info.AddValue("traceStartTimeout", traceStartTimeout);
                }
                Debug.WriteLine(String.Format("{0} serialized.", this.GetType()));
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowSerializationException(e, this.GetType());
            }

        }

        #endregion

    }
}
