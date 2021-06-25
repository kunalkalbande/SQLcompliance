using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{
    [Serializable]
    [DataContract(Name = "RegulatoryComplianceData")]
    public class RegulatoryComplianceData
    {

        [DataMember(Order = 0, Name = "ServerName")]
        public string ServerName { get; set; }

        [DataMember(Order = 1, Name = "DatabaseName")]
        public string DatabaseName { get; set; }

        [DataMember(Order = 2, Name = "IsDatabase")]
        public bool IsDatabase { get; set; }

        [DataMember(Order = 3, Name = "RowList")]
        public List<RegulatoryComplianceRowData> RowList { get; set; }

        [DataMember(Order = 4, Name = "ShowCIS")]
        public bool ShowCIS { get; set; }

        [DataMember(Order = 5, Name = "ShowDISASTIG")]
        public bool ShowDISASTIG { get; set; }

        [DataMember(Order = 6, Name = "ShowFERPA")]
        public bool ShowFERPA { get; set; }

        [DataMember(Order = 7, Name = "ShowGDPR")]
        public bool ShowGDPR { get; set; }

        [DataMember(Order = 8, Name = "ShowHIPAA")]
        public bool ShowHIPAA { get; set; }

        [DataMember(Order = 9, Name = "ShowNERC")]
        public bool ShowNERC { get; set; }

        [DataMember(Order = 10, Name = "ShowPCIDSS")]
        public bool ShowPCIDSS { get; set; }

        [DataMember(Order = 11, Name = "ShowSOX")]
        public bool ShowSOX { get; set; }

    }
}
