using System.Collections.Generic;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class DataAlertRulesInfo
    {

        public DataAlertRulesInfo()
        {
            sensitiveTable = new List<DataAlertRulesTableDetail>();
            sensitiveColumn = new List<DataAlertRulesColumnDetail>();
            sensitiveDatabase = new List<DataAlertRulesDBDetail>();
        }


        [DataMember]
        public List<DataAlertRulesTableDetail> sensitiveTable { get; set; }

        internal void Add(DataAlertRulesTableDetail sensitiveTableArg)
        {
            sensitiveTable.Add(sensitiveTableArg);
        }

        [DataMember]
        public List<DataAlertRulesColumnDetail> sensitiveColumn { get; set; }

        internal void Add(DataAlertRulesColumnDetail sensitiveColumnArg)
        {
            sensitiveColumn.Add(sensitiveColumnArg);
        }

        [DataMember]
        public List<DataAlertRulesDBDetail> sensitiveDatabase { get; set; }

        internal void Add(DataAlertRulesDBDetail sensitiveDatabaseArg)
        {
            sensitiveDatabase.Add(sensitiveDatabaseArg);
        }
    }
}
