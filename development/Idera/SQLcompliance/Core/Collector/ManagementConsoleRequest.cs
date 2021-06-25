using System;

namespace Idera.SQLcompliance.Core.Collector
{
    public class ManagementConsoleRequest : MarshalByRefObject
    {
        public string HasRoghtsToRepository()
        {
            return CollectionServer.Instance.HasRoghtsToRepository();
        }

        public string HasRightToReadRegistry()
        {
            return CollectionServer.Instance.HasRightToReadRegistry();
        }

        public string HasPermissionToTraceDirectory()
        {
            return CollectionServer.Instance.HasPermissionToTraceDirectory();
        }

        public string SqlServerHasPermissionToTraceDirectory(string instanceName)
        {
            return CollectionServer.Instance.SqlServerHasPermissionToTraceDirectory(instanceName);
        }
    }
}
