using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TracerX;

namespace SQLcomplianceCwfAddin.Helpers
{
    public static class SQLComplianceLogHelper
    {
        public static Logger GetSQLcmLogger(string logClient)
        {
            return Logger.GetLogger(logClient);
        }
    }
}
