using System;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.UserSettings
{
    [Serializable]
    [DataContract(Name = "UserSettingsModel")]
    public class UserSettingsModel
    {
        [DataMember(Order = 1, Name = "dashboardUserId")]
        public int DashboardUserId { get; set; }

        [DataMember(Order = 2, Name = "account")]
        public string Account { get; set; }

        [DataMember(Order = 3, Name = "email")]
        public string Email { get; set; }

        [DataMember(Order = 4, Name = "sessionTimout")]
        public int? SessionTimout { get; set; }

        [DataMember(Order = 5, Name = "subscribed")]
        public bool Subscribed { get; set; }
    }
}
