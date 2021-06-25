using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Idera.SQLcompliance.Core.CWFDataContracts
{
    [Serializable]
    [DataContract]
    public class GlobalTag
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public List<string> Instances { get; set; }
    }
}
