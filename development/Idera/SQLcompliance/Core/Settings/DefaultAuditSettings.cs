using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Idera.SQLcompliance.Core.Settings
{
    public class DefaultAuditSettings
    {
        public static void RestoreDefaultAuditFlags(SqlConnection conn)
        {
            try
            {
                string sql = String.Format("UPDATE {0}..{1} SET isSet = 1",
                       CoreConstants.RepositoryDatabase,
                       CoreConstants.RepositoryDefaultAuditSettingDialogFlags);
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                //log ex
            }
        }
    }
}
