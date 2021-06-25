using System;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Archives;

namespace SQLcomplianceCwfAddin.Helpers.Archives
{
    internal class ArchiveDbHelper
    {
        public static void
            UpdateArchiveProperties(
            SqlConnection conn,
            ArchiveUpdateRequest request
            )
        {
            using (conn)
            {
                string sql = "";
                SqlCommand cmd = null;

                // write record to SystemDatabases
                sql = String.Format("UPDATE {0}..{1} SET displayName={2},description={3} " +
                                    "WHERE instance={4} AND databaseName={5};",
                    CoreConstants.RepositoryDatabase,
                    CoreConstants.RepositorySystemDatabaseTable,
                    SQLHelpers.CreateSafeString(request.DisplayName),
                    SQLHelpers.CreateSafeString(request.Description),
                    SQLHelpers.CreateSafeString(request.Instance),
                    SQLHelpers.CreateSafeString(request.DatabaseName));
                cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                // update archive database
                if (request.NewDefaultAccess != request.OldDefaultAccess)
                {
                    EventDatabase.SetDefaultSecurity(request.DatabaseName,
                        request.NewDefaultAccess,
                        request.OldDefaultAccess,
                        true,
                        conn);
                }

                sql = String.Format("UPDATE {0}..{1} SET displayName={2},description={3},defaultAccess={4};",
                    SQLHelpers.CreateSafeDatabaseName(request.DatabaseName),
                    CoreConstants.RepositoryArchiveMetaTable,
                    SQLHelpers.CreateSafeString(request.DisplayName),
                    SQLHelpers.CreateSafeString(request.Description),
                    request.NewDefaultAccess);
                cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
