/***Start SQLCm 5.4***/
/*Requirement 4.1.4.1*/

using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "AllImportSettingDetails")]
    public class AllImportSettingDetails
    {
        public AllImportSettingDetails()
        {
            ServerDetails = new List<ServerDetails>();
            dbDetails = new List<DatabaseDetails>();
            userdbServerComboSelection = new HashSet<TargetDatabaseDetail>();
            auditedActivities = new AuditActivity();
            serverAuditedActivities = new AuditActivity();
        }

        [DataMember(Name = "validFile")]
        public Boolean validFile { get; set; }

        [DataMember(Name = "dbDetails")]
        public List<String> allDBNames { get; set; }

        [DataMember(Name = "privUserConfig")]
        public String privUserConfig { get; set; }

        [DataMember(Name = "ServerLevelConfig")]
        public String ServerLevelConfig { get; set; }

        [DataMember(Name = "Database")]
        public String Database { get; set; }

        [DataMember(Name = "MatchDBNames")]
        public String MatchDBNames { get; set; }

        [DataMember(Name = "DatabasePrivUser")]
        public String DatabasePrivUser { get; set; }

        [DataMember(Name = "ServerDetails")]
        public List<ServerDetails> ServerDetails { get; set; }

        [DataMember(Name = "DatabaseDetails")]
        public List<DatabaseDetails> dbDetails { get; set; }

        [DataMember(Name = "xmlData")]
        public String xmlData { get; set; }
        [DataMember(Name = "auditedActivities")]
        public AuditActivity auditedActivities { get; set; }
        [DataMember(Name = "serverAuditedActivities")]
        public AuditActivity serverAuditedActivities { get; set; }

        [DataMember(Name = "userCheckServer")]
        public Boolean userCheckServer { get; set; }

        [DataMember(Name = "userCheckServerPrivilage")]
        public Boolean userCheckServerPrivilage { get; set; }

        [DataMember(Name = "userCheckDatabase")]
        public Boolean userCheckDatabase { get; set; }

        [DataMember(Name = "usercheckDatabasePrivilage")]
        public Boolean usercheckDatabasePrivilage { get; set; }

        [DataMember(Name = "overwriteSelection")]
        public Boolean overwriteSelection { get; set; }

        [DataMember(Name = "usermatchdbNameSelection")]
        public Boolean usermatchdbNameSelection { get; set; }

        [DataMember(Name = "userdbSelection")]
        public HashSet<DatabaseDetails> userdbSelection { get; set; }

        [DataMember(Name = "userServerSelection")]
        public HashSet<ServerDetails> userServerSelection { get; set; }

        [DataMember(Name = "userdbServerComboSelection")]
        public HashSet<TargetDatabaseDetail> userdbServerComboSelection { get; set; }

        internal void Add(DatabaseDetails dbdetails)
        {
            dbDetails.Add(dbdetails);    
        }
        internal void Add(ServerDetails serverDetails)
        {
            ServerDetails.Add(serverDetails);
        }
       
    }
}

/***End SQLCm 5.4***/