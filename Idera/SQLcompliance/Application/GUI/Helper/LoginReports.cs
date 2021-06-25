using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
    public class LoginReports
    {
        public string reportName;
        public int uid;
        public bool chked = true;
        public bool originalChked = true;

        override public string ToString() { return reportName; }
    }
}
