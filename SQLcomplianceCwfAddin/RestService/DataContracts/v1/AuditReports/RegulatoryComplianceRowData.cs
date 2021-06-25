using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{
    [Serializable]
    [DataContract(Name = "RegulatoryComplianceRowData")]
    public class RegulatoryComplianceRowData
    {

        [DataMember(Order = 0, Name = "FieldName")]
        public string FieldName { get; set; }

        [DataMember(Order = 1, Name = "IsHeader")]
        public bool IsHeader { get; set; }

        [DataMember(Order = 2, Name = "FieldType")]
        public int FieldType { get; set; }

        [DataMember(Order = 3, Name = "IsFieldNameRed")]
        public bool IsFieldNameRed { get; set; }

        [DataMember(Order = 4, Name = "IsCISRed")]
        public bool IsCISRed { get; set; }

        [DataMember(Order = 5, Name = "IsCISChecked")]
        public bool IsCISChecked { get; set; }

        [DataMember(Order = 6, Name = "IsDISASTIGRed")]
        public bool IsDISASTIGRed { get; set; }

        [DataMember(Order = 7, Name = "IsDISASTIGChecked")]
        public bool IsDISASTIGChecked { get; set; }

        [DataMember(Order = 8, Name = "IsFERPARed")]
        public bool IsFERPARed { get; set; }

        [DataMember(Order = 9, Name = "IsFERPAChecked")]
        public bool IsFERPAChecked { get; set; }

        [DataMember(Order = 10, Name = "IsGDPRRed")]
        public bool IsGDPRRed { get; set; }

        [DataMember(Order = 11, Name = "IsGDPRChecked")]
        public bool IsGDPRChecked { get; set; }

        [DataMember(Order = 12, Name = "IsHIPAARed")]
        public bool IsHIPAARed { get; set; }

        [DataMember(Order = 13, Name = "IsHIPAAChecked")]
        public bool IsHIPAAChecked { get; set; }

        [DataMember(Order = 14, Name = "IsNERCRed")]
        public bool IsNERCRed { get; set; }

        [DataMember(Order = 15, Name = "IsNERCChecked")]
        public bool IsNERCChecked { get; set; }

        [DataMember(Order = 16, Name = "IsPCIDSSRed")]
        public bool IsPCIDSSRed { get; set; }

        [DataMember(Order = 17, Name = "IsPCIDSSChecked")]
        public bool IsPCIDSSChecked { get; set; }

        [DataMember(Order = 18, Name = "IsSOXRed")]
        public bool IsSOXRed { get; set; }

        [DataMember(Order = 19, Name = "IsSOXChecked")]
        public bool IsSOXChecked { get; set; }

    }
}
