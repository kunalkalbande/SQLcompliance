using System;
using System.Collections.Generic;

namespace Idera.SQLcompliance.Core.Agent
{
    public class ManagementConsoleRequest : MarshalByRefObject
    {
        public string HasRightsToSqlServer(string instanceName)
        {
            return SQLcomplianceAgent.Instance.HasRightsToSqlServer(instanceName);
        }

        public string HasRightToReadRegistry()
        {
            return SQLcomplianceAgent.Instance.HasRightToReadRegistry();
        }

        public string HasPermissionToTraceDirectory()
        {
            return SQLcomplianceAgent.Instance.HasPermissionToTraceDirectory();
        }

        public string SqlServerHasPermissionToTraceDirectory(string instanceName)
        {
            return SQLcomplianceAgent.Instance.SqlServerHasPermissionToTraceDirectory(instanceName);
        }

        public bool isRowCountEnabled()
        {
            return SQLcomplianceAgent.Instance.isRowCountEnabled();
        }
    }
}
