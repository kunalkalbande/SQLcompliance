using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class DeleteUserSettingsRequest
    {
        [DataMember(Name = "dashbloardUserIds", Order = 1)]
        public List<int> DashbloardUserIds { get; set; }
    }
}
