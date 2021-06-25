using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports
{
    class ConfigurationCheckProcedureResponse
    {
        
            public ConfigurationCheckProcedureResponse()
            {
                ConfigurationCheck = new List<ConfigurationCheckProcedureData>();
            }

            public List<ConfigurationCheckProcedureData> ConfigurationCheck { get; set; }

            internal void Add(ConfigurationCheckProcedureData configurationCheckProcedureData)
            {
                ConfigurationCheck.Add(configurationCheckProcedureData);
            }
        
    }
}
