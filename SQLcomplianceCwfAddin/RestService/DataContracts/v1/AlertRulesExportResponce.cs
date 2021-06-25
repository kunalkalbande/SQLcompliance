using System.Collections.Generic;
using System.Runtime.Serialization;
namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
    [DataContract]
    public class AlertRulesExportResponse
    {

        public AlertRulesExportResponse()
        {
            AlertRules = new List<AlertRulesExportData>();
            AlertRulesCondition = new List<AlertRulesExportConditionData>();
            AlertRulesCategory = new List<AlertRulesExportCategoryData>();
        }


        [DataMember]
        public List<AlertRulesExportData> AlertRules { get; set; }

        internal void Add(AlertRulesExportData alertRules)
        {
            AlertRules.Add(alertRules);
        }

        [DataMember]
        public List<AlertRulesExportConditionData> AlertRulesCondition { get; set; }

        internal void Add(AlertRulesExportConditionData alertRulesCondition)
        {
            AlertRulesCondition.Add(alertRulesCondition);
        }

        [DataMember]
        public List<AlertRulesExportCategoryData> AlertRulesCategory { get; set; }

        internal void Add(AlertRulesExportCategoryData alertRulesCategory)
        {
            AlertRulesCategory.Add(alertRulesCategory);
        }
    }
}
