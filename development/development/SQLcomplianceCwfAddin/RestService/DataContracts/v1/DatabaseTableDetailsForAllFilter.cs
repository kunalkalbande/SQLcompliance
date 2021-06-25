using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
     [DataContract]
    public class DatabaseTableDetailsForAllFilter
    {
       

            [DataMember(Order = 0, Name = "databaseList")]
            public List<string> DatabaseList { get; set; }

            [DataMember(Order = 1, Name = "serverId")]
            public int ServerId { get; set; }
           
            [DataMember(Order = 2, Name = "profileName")]
            public string ProfileName { get; set; }
    

        
    }
}
