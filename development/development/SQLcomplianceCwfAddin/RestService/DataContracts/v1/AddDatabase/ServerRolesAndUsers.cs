using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase
{
    [DataContract(Name = "serverRolesAndUsers")]
    public class ServerRolesAndUsers
    {
        [DataMember(Order = 1, Name = "userList")]
        public List<ServerLogin> UserList;

        [DataMember(Order = 2, Name = "roleList")]
        public List<ServerRole> RoleList;
    }
}
