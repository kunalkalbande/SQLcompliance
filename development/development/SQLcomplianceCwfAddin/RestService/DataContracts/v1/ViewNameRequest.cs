using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "ViewNameData")]
    public class ViewNameData
    {
        [DataMember(Order = 1, Name = "viewName")]
        public string ViewName { get; set; }

    }
}

