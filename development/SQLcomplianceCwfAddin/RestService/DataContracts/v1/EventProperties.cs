using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Events;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class EventProperties
    {
        [DataMember(Order = 0, Name = "eventId")]
        public int EventId { get; set; }

        [DataMember(Order = 1, Name = "eventType")]
        public int EventType { get; set; }

        [DataMember(Order = 2, Name = "eventCategory")]
        public int EventCategory { get; set; }

        [DataMember(Order = 3, Name = "targetObject")]
        public string TargetObject { get; set; }

        [DataMember(Order = 4, Name = "details")]
        public string Details { get; set; }

        [DataMember(Order = 5, Name = "hash")]
        public string Hash { get; set; }

        [DataMember(Order = 6, Name = "eventClass")]
        public int EventClass { get; set; }

        [DataMember(Order = 7, Name = "eventSubclass")]
        public int EventSubclass { get; set; }

        [DataMember(Order = 8, Name = "startTime")]
        public DateTime? StartTime { get; set; }

        [DataMember(Order = 9, Name = "spid")]
        public int SpId { get; set; }

        [DataMember(Order = 10, Name = "applicationName")]
        public string AppName { get; set; }

        [DataMember(Order = 11, Name = "hostName")]
        public string HostName { get; set; }

        [DataMember(Order = 11, Name = "serverName")]
        public string ServerName { get; set; }
        
        [DataMember(Order = 12, Name = "loginName")]
        public string LoginName { get; set; }

        [DataMember(Order = 13, Name = "success")]
        public int Success { get; set; }

        [DataMember(Order = 14, Name = "databaseName")]
        public string DatabaseName { get; set; }

        [DataMember(Order = 15, Name = "databaseId")]
        public int DatabaseId { get; set; }

        [DataMember(Order = 16, Name = "databaseUserName")]
        public string DatabaseUserName { get; set; }

        [DataMember(Order = 17, Name = "objectType")]
        public int ObjectType { get; set; }

        [DataMember(Order = 18, Name = "objectName")]
        public string ObjectName { get; set; }

        [DataMember(Order = 19, Name = "objectId")]
        public int ObjectId { get; set; }

        [DataMember(Order = 20, Name = "permissions")]
        public int Permissions { get; set; }

        [DataMember(Order = 21, Name = "columnPermissions")]
        public int ColumnPermissions { get; set; }

        [DataMember(Order = 22, Name = "targetLoginName")]
        public string TargetLoginName { get; set; }

        [DataMember(Order = 23, Name = "targetUserName")]
        public string TargetUserName { get; set; }

        [DataMember(Order = 24, Name = "roleName")]
        public string RoleName { get; set; }

        [DataMember(Order = 25, Name = "ownerName")]
        public string OwnerName { get; set; }

        [DataMember(Order = 26, Name = "alertLevel")]
        public int AlertLevel { get; set; }

        [DataMember(Order = 27, Name = "checkSum")]
        public string CheckSum { get; set; }

        [DataMember(Order = 28, Name = "privilegedUser")]
        public int PrivilegedUser { get; set; }

        [DataMember(Order = 29, Name = "name")]
        public string Name { get; set; }

        [DataMember(Order = 30, Name = "category")]
        public string Category { get; set; }

        [DataMember(Order = 31, Name = "fileName")]
        public string FileName { get; set; }

        [DataMember(Order = 32, Name = "linkedServerName")]
        public string LinkedServerName { get; set; }

        [DataMember(Order = 33, Name = "parentName")]
        public string ParentName { get; set; }

        [DataMember(Order = 34, Name = "isSystem")]
        public int IsSystem { get; set; }

        [DataMember(Order = 35, Name = "sessionLoginName")]
        public string SessionLoginName { get; set; }

        [DataMember(Order = 36, Name = "providerName")]
        public string ProviderName { get; set; }

        [DataMember(Order = 37, Name = "appNameId")]
        public string AppNameId { get; set; }

        [DataMember(Order = 38, Name = "hostId")]
        public string HostId { get; set; }

        [DataMember(Order = 39, Name = "loginId")]
        public string LoginId { get; set; }

        [DataMember(Order = 40, Name = "endTime")]
        public DateTime? EndTime { get; set; }

        [DataMember(Order = 41, Name = "startSequence")]
        public int StartSequence { get; set; }

        [DataMember(Order = 42, Name = "endSequence")]
        public int EndSequence { get; set; }

        [DataMember(Order = 43, Name = "sensitiveColumns")]
        public int SensitiveColumns { get; set; }

        [DataMember(Order = 44, Name = "sqlStatement")]
        public string SqlStatement { get; set; }

        [DataMember(Order = 45, Name = "columnsAffected")]
        public string ColumnsAffected { get; set; }

        [DataMember(Order = 46, Name = "rowsAffected")]
        public int RowsAffected { get; set; }

        [DataMember(Order = 47, Name = "sqlVersion")]
        public int SqlVersion { get; set; }

        [DataMember(Order = 48, Name = "sensitiveColumnList")]
        public List<string> SensitiveColumnList { get; set; }

        [DataMember(Order = 49, Name = "beforeAfterData")]
        public EventBeforeAfterData BeforeAfterData { get; set; }

        [DataMember(Order = 50, Name = "rowCounts")]
        public long? RowCounts { get; set; }
    }
}
