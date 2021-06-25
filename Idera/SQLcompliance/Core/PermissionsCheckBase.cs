using System;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core.Agent;

namespace Idera.SQLcompliance.Core
{
    public class PermissionsCheckBase
    {
        protected string GetSqlServiceNameOfInstance(string instanceName)
        {
            using (var connection = new SqlConnection(SQLcomplianceAgent.CreateConnectionString(instanceName, null)))
            {
                try
                {
                    connection.Open();
                    return RawSQL.GetServiceNameOfInstance(instanceName, connection);
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Always, string.Format("PermissionsCheckBase.GetSqlServiceNameOfInstance Failed for instance '{0}'.", instanceName), ex);
                }
            }

            return string.Empty;
        }

        protected string GetWindowsServiceNameOfSqlInstance(string instanceName)
        {
            if (instanceName.Contains(","))
            {
               instanceName = instanceName.Split(',')[0];
            }

            string[] instanceParts = instanceName.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);

            string instancePart = string.Empty;

            switch (instanceParts.Length)
            {
                // default service name
                case 1:
                    instancePart = GetSqlServiceNameOfInstance(instanceName);
                    break;

                // service with instance name
                case 2:
                    instancePart = instanceParts[1];
                    break;
            }

            //We avoid adding MSSQL$ prefix for default issues
            if (instancePart.ToUpper() != "MSSQLSERVER")
            {
                instancePart = string.Format("MSSQL${0}", instancePart);
            }

            return instancePart;
        }
    }
}
