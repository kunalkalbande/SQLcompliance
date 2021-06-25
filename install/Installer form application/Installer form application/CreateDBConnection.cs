using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Installer_form_application
{
    class CreateDBConnection
    {
        public SqlConnection GetConnection(string database, string instanceName)
        {
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(CreateConnectionString(instanceName, database));
                conn.Open();
            }
            catch (Exception e)
            {
                string msg = String.Format("Connection Error", instanceName, e.Message);
                //ConnectionFailedException ce = new ConnectionFailedException(msg, e);
                //SetConnectionState( false, ce );
                //throw ce;
            }

            return conn;
        }

        public static string CreateConnectionString(string database, string instanceName)
        {
            if (instanceName == null)
                return null;

            string connStr =
               String.Format(
                  "server={0};integrated security=SSPI{1}{2};Connect Timeout=30;Application Name='{3}'",
                  instanceName,
                  database == null || database.Length == 0 ? "" : ";database=",
                  database == null || database.Length == 0 ? "" : database,
                  "SQLcompliânce");
            return connStr;
        }
    }
}
