//Start SQLCm-5.4 
//Requirement - 4.1.3.1

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "SensitiveTableDetails")]
    public class SensitiveColumnTableDetails
    {
        [DataMember(Name = "dbId")]
        public int DbId { get; set; }

        [DataMember(Name = "tblId")]
        public int TblId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "selected")]
        public bool Selected { get; set; }
    }

    [Serializable]
    [DataContract(Name = "SensitiveDatabaseDetails")]
    public class SensitiveColumnDatabaseDetails
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "dbId")]
        public int DbId { get; set; }

        [DataMember(Name = "srvId")]
        public int SrvId { get; set; }

        [DataMember(Name = "selected")]
        public bool Selected { get; set; }
    }

    [Serializable]
    [DataContract(Name = "SensitiveColumnDetails")]
    public class SensitiveColumnColumnDetail
    {
        [DataMember(Name = "dbId")]
        public int DbId { get; set; }

        [DataMember(Name = "tblId")]
        public int TblId { get; set; }

        [DataMember(Name = "colId")]
        public int ColId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "selected")]
        public bool Selected { get; set; }
    }
}

//End SQLCm-5.4 
