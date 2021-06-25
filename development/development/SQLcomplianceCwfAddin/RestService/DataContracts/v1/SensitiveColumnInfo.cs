//Start SQLCm-5.4 
//Requirement - 4.1.3.1

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class SensitiveColumnInfo
    {
        public SensitiveColumnInfo()
        {
            sensitiveTable = new List<SensitiveColumnTableDetails>();
            sensitiveColumn = new List<SensitiveColumnColumnDetail>();
            sensitiveDatabase = new List<SensitiveColumnDatabaseDetails>();
        }

        [DataMember]
        public List<SensitiveColumnTableDetails> sensitiveTable { get; set; }

        internal void Add(SensitiveColumnTableDetails sensitiveTableArg)
        {
            sensitiveTable.Add(sensitiveTableArg);
        }

        internal void Remove(int index)
        {
            sensitiveTable.RemoveAt(index);
        }

        [DataMember]
        public List<SensitiveColumnColumnDetail> sensitiveColumn { get; set; }

        internal void Add(SensitiveColumnColumnDetail sensitiveColumnArg)
        {
            sensitiveColumn.Add(sensitiveColumnArg);
        }

        [DataMember]
        public List<SensitiveColumnDatabaseDetails> sensitiveDatabase { get; set; }

        internal void Add(SensitiveColumnDatabaseDetails sensitiveDatabaseArg)
        {
            sensitiveDatabase.Add(sensitiveDatabaseArg);
        }

        [DataMember(Name = "validFile")]
        public bool validFile { get; set; }

    }
}

//End SQLCm-5.4
