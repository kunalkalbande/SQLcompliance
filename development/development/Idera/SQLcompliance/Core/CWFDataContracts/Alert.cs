using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLcompliance.Core.CWFDataContracts
{
    public class Alert
    {
        public string AlertCategory { get; set; }
        
        public string Database { get; set; }
        
        public string Instance { get; set; }
        
        public DateTime LastActiveTime { get; set; }
        
        public string Metric { get; set; }
        
        public int ProductId { get; set; }
        
        public string Severity { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public string Summary { get; set; }
        
        public string Table { get; set; }
        
        public string Value { get; set; }
    }
}
