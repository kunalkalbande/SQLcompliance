using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddServer
{
    [DataContract(Name = "maintainDatabase")]
    public enum ExistingAuditData
    {
        //Keep the previously collected audit data and use the existing database. 
        //If the events database is from a previous version of SQL Compliance Manager, 
        //it will be automatically upgraded.
        [EnumMember(Value = "keepExisting")]
        Keep,

        /// Delete the previously collected audit data but use the existing database. 
        /// This option will reinitialize the existing database. 
        /// All audit data in the database will be permanently deleted.
        [EnumMember(Value = "delete")]
        Delete,
    }
}
