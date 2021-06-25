using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "environmentObjectType")]
    public enum EnvironmentObjectType : byte
    {
        [EnumMember(Value = "root")]
        Root,

        [EnumMember(Value = "server")]
        Server,

        [EnumMember(Value = "database")]
        Database,

        [EnumMember(Value = "table")]
        Table,

        [EnumMember(Value = "column")]
        Column
    }

    [Serializable]
    [DataContract(Name = "environmentObject")]
    public class EnvironmentObject
    {
        [DataMember(Order = 1, Name = "id")]
        public int Id { get; set; }

        [DataMember(Order = 2, Name = "systemId", EmitDefaultValue = false)]
        [Description("This is the internal ID of object assigned by system. ex: SQL server database ID, table ID, etc.")]
        public int SyetemId { get; set; }

        [DataMember(Order = 3, Name = "parentId")]
        public int ParentId { get; set; }

        [DataMember(Order = 4, Name = "type")]
        public EnvironmentObjectType Type { get; set; }

        [DataMember(Order = 5, Name = "name")]
        public string Name { get; set; }

        [DataMember(Order = 6, Name = "description")]
        public string Description { get; set; }

        [DataMember(Order = 7, Name = "isEnabled")]
        public bool IsEnabled { get; set; }
    }
}
