using System.Collections.Generic;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public  class ViewNameResponse
    {
        public ViewNameResponse()
        {
            viewNameTable = new List<ViewNameData>();
        }

        [DataMember]
        public List<ViewNameData> viewNameTable { get; set; }

        internal void Add(ViewNameData viewNameTableArg)
        {
            viewNameTable.Add(viewNameTableArg);
        }
    }
}
