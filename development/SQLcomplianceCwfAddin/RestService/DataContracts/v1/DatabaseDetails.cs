/***Start SQLCm 5.4***/
/*Requirement 4.1.4.1*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "DatabaseDetails")]
    public class DatabaseDetails
    {
        [DataMember(Name = "dbId")]
        public int dbId { get; set; }

        [DataMember(Name = "srvId")]
        public int srvId { get; set; }

        [DataMember(Name = "dbName")]
        public String dbName { get; set; }
    }
}

/***End SQLCm 5.4***/