using System;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core;

namespace SQLcomplianceCwfAddin.Helpers.SQL
{
    public class SqlCmConfigurationHelper
    {
        public static SQLcomplianceConfiguration GetConfiguration(SqlConnection connection)
        {
            SQLcomplianceConfiguration config = null;

            try
            {
                config = new SQLcomplianceConfiguration();
                config.Read(connection);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(
                       ErrorLog.Level.Verbose,
                       String.Format("Failed to read SQL compliance Configuration with connection: {0}: ", connection),
                       ex,
                       ErrorLog.Severity.Warning);
            }
            
            return config;
        }
    }
}
