using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "CategoryRequest")]
    public class CategoryRequest
    {
        [DataMember(Order = 1, Name = "category")]
        public string Category { get; set; }
    }

    [Serializable]
    [DataContract(Name = "CategoryrData")]
    public class CategoryData
    {
        [DataMember(Order = 1, Name = "name")]
        public string Name { get; set; }

        [DataMember(Order = 2, Name = "evtypeid")]
        public string EvTypeId { get; set; }

    }
}
