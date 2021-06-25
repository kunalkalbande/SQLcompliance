using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "DataAlertRulesDetail")]
    public class DataAlertRulesTableDetail
    {
        [DataMember(Name = "srvId")]
        public int SrvId { get; set; }

        [DataMember(Name = "dbId")]
        public int DbId { get; set; }

        [DataMember(Name = "objectId")]
        public int ObjectId { get; set; }

        [DataMember(Name = "schemaName")]
        public string SchemaName { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "selectedColumn")]
        public int SelectedColumn { get; set; }
    }

    [Serializable]
    [DataContract(Name = "DataAlertRulesDBDetail")]
    public class DataAlertRulesDBDetail
    {
        [DataMember(Name = "srvId")]
        public int SrvId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "dbId")]
        public int DbId { get; set; }
    }

    [Serializable]
    [DataContract(Name = "DataAlertRulesColumnDetail")]
    public class DataAlertRulesColumnDetail
    {
        [DataMember(Name = "srvId")]
        public int SrvId { get; set; }

        [DataMember(Name = "dbId")]
        public int DbId { get; set; }

        [DataMember(Name = "objectId")]
        public int ObjectId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    [Serializable]
    [DataContract(Name = "DataAlertRulesServerId")]
    public class DataAlertRulesServerId
    {
        [DataMember(Order = 0, Name = "srvId")]
        public int SrvId { get; set; }

        [DataMember(Order = 0, Name = "conditionId")]
        public int ConditionId { get; set; }
    }
}
