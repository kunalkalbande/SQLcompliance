/***Start SQLCm 5.4***/
/*Requirement 4.1.4.1*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [Serializable]
    [DataContract(Name = "ServerDetails")]
    public class ServerDetails
    {
        [DataMember(Name = "serverId")]
        public int serverId { get; set; }

        [DataMember(Name = "serverName")]
        public String serverName { get; set; }
    }
}

/***End SQLCm 5.4***/