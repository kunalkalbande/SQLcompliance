using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract(Name = "viewSettings")]
    public class ViewSettings
    {
        [DataMember(Name = "timeout", Order = 0)]
        public int? Timeout { get; set; }

        [DataMember(Name = "filter", Order = 1)]
        public string Filter { get; set; }

        [DataMember(Name = "viewId", Order = 2)]
        public string ViewId { get; set; }

        [DataMember(Name = "userId", Order = 3)]
        public string UserId { get; set; }

        [DataMember(Name = "viewName", Order = 4)]
        public string ViewName { get; set; }

    }
}
