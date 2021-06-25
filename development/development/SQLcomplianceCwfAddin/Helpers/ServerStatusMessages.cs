
namespace SQLcomplianceCwfAddin.Helpers
{
    public static class ServerStatusMessages
    {
        public static string ServerStatus_OK = "Ok";
        public static string ServerStatus_AwaitingManual = "Awaiting manual deployment";
        public static string ServerStatus_Inactive = "Inactive";
        public static string ServerStatus_NotDeployed = "Agent not deployed";
        public static string ServerStatus_NotRunning = "Down";
        public static string ServerStatus_Crippled = "Error";
        public static string ServerStatus_Disabled = "Disabled";
        public static string ServerStatus_Enabled = "Enabled";
        public static string ServerStatus_NoAuditData = "No Audit Data Collected";
        public static string ServerStatus_Unknown = "Unknown";
        public static string ServerStatus_NotRegistered = "Not registered";
        public static string ServerStatus_LicenseExpired = "License Expired";
        public static string ServerStatus_NotAudited = "Archive server";
        public static string ServerStatus_Pending = "; Update pending";
        public static string ServerStatus_Requested = "; Update requested";
        public static string ServerStatus_Stale = "No recent heartbeat";
        public static string ServerStatus_VeryStale = "No contact in over a day";
        public static string ServerStatus_NoEventsDatabase = "Error creating database";
        public static string ServerStatus_Down = "Non-operational";
        public static string ServerStatus_ReviewNeeded = "Corrupted events; review needed";
        public static string ServerStatus_Initializing = "Initializing...";
        public static string ServerStatus_2005NotSupported = "SQL Server 2005 Unsupported";
        public static string ServerStatus_2008NotSupported = "SQL Server 2008 Unsupported";
        public static string ServerStatus_2012NotSupported = "SQL Server 2012 Unsupported";
        public static string ServerStatus_2014NotSupported = "SQL Server 2014 Unsupported";
        public static string ServerStatus_2016NotSupported = "SQL Server 2016 Unsupported";		
        public static string ServerStatus_2017NotSupported = "SQL Server 2017 Unsupported";
        public static string ServerStatus_AgentUpgradeRequired = "Agent Upgrade Required";
        public static string AuditStatus_ViewActivityLog = "View Activity Log";
        public static string ServerStatus_UnknownVerbose = "Collection Server status is unknown. The service is running but has not sent its regular heartbeat message in over an hour.";
        public static string ServerStatus_StartingVerbose = "Collection Server is starting. Status will show as non-operational until initialization complete.";
        public static string ServerStatus_OKVerbose = "Collection Server is up.";
        public static string ServerStatus_DownVerbose = "Collection Server is not operational or is inaccessible. When the server is non-operational, no events can be collected from audited SQL Servers.";

        // Status Descriptions
        public static string Status_Enabled = "Enabled";
        public static string Status_Disabled = "Disabled";
        public static string Status_Pending = "Update pending";
        public static string Status_Requested = "Update requested";
        public static string Status_Never = "Never";
        public static string Status_Current = "Current";
        public static string Status_ReportingOnly = "Not Audited";
        public static string Status_Unavailable = "Unavailable";
    }
}
