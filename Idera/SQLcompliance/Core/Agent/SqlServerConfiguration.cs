using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLcompliance.Core.Agent
{    
    class SqlServerConfiguration
    {
        public int OsVersionMajor { get; set; }

        public int OsVersionMinor { get; set; }

        public int SpNumber { get; set; }

        public int SqlVersion { get; set; }

        public string SqlVersionBuild { get; set; }

        public bool IsMSDE { get; set; } 
    }
}
