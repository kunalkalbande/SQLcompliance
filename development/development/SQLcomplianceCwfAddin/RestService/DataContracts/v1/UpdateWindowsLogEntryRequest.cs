using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "UpdateWindowsLogEntryRequest")]
    public class UpdateWindowsLogEntryRequest
    {
        [DataMember(Order = 1, Name = "logType")]
        public string LogType { get; set; }
    }
}
