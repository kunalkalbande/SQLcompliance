using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLcompliance.Core.Settings
{
    public class AuditDBSettings
    {
        public string Name { get; set; }
        public int AuditCaptureDDL { get; set; }
        public int AuditUserCaptureDDL { get; set; }
    }
}
