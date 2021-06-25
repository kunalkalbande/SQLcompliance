using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Idera.SQLcompliance.Core.Cwf
{
    public class LoginAccount
    {
        #region members

        private static readonly string GetLoginSql;
        private static readonly string SetLoginSql;
        private static readonly string DeleteLoginSql;
        private bool _isSet;

        #endregion

        #region constructor \ destructor

        static LoginAccount()
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine(" SELECT TOP 1 ");
            sqlBuilder.AppendLine(" 	[WebApplicationAccess]");
            sqlBuilder.AppendLine(" FROM [SQLcompliance]..[LoginAccounts]");
            sqlBuilder.AppendLine(" WHERE [Name] = {0}");
            GetLoginSql = sqlBuilder.ToString();
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" IF (SELECT COUNT(*) FROM [SQLcompliance]..[LoginAccounts] WHERE [Name] = {0}) = 1");
            sqlBuilder.AppendLine(" BEGIN");
            sqlBuilder.AppendLine(" 	UPDATE [SQLcompliance]..[LoginAccounts]");
            sqlBuilder.AppendLine(" 	SET [WebApplicationAccess] = {1}");
            sqlBuilder.AppendLine(" 	WHERE [Name] = {0}");
            sqlBuilder.AppendLine(" END");
            sqlBuilder.AppendLine(" ELSE");
            sqlBuilder.AppendLine(" BEGIN");
            sqlBuilder.AppendLine(" 	INSERT INTO [SQLcompliance]..[LoginAccounts]");
            sqlBuilder.AppendLine("            ([Name]");
            sqlBuilder.AppendLine("            ,[WebApplicationAccess])");
            sqlBuilder.AppendLine("      VALUES");
            sqlBuilder.AppendLine("            ({0}");
            sqlBuilder.AppendLine("            ,{1})");
            sqlBuilder.AppendLine(" END");
            SetLoginSql = sqlBuilder.ToString();
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" DELETE FROM [SQLcompliance]..[LoginAccounts] WHERE [Name] = {0}");
            DeleteLoginSql = sqlBuilder.ToString();
            sqlBuilder.Clear();
        }

        /// <summary>
        /// Initialize login account object and populate its data from repository.
        /// </summary>
        /// <param name="name"></param>
        public LoginAccount(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(name);

            Name = name;
            Get();
        }

        #endregion

        #region properties

        public string Name { get; private set; }

        public bool WebApplicationAccess { get; set; }

        #endregion

        private SqlConnection GetRepositoryConnection()
        {
            if (string.IsNullOrEmpty(Repository.ServerInstance))
                throw new Exception("Repository server instance not initialized.");

            var repository = new Repository();
            repository.OpenConnection();
            return repository.connection;
        }

        public void Get()
        {
            using (var connection = GetRepositoryConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = string.Format(GetLoginSql, SQLHelpers.CreateSafeString(Name));
                    WebApplicationAccess = Convert.ToBoolean(command.ExecuteScalar());
                }
            }
        }

        public bool Set()
        {
            using (var connection = GetRepositoryConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    _isSet = true;
                    command.CommandType = CommandType.Text;
                    command.CommandText = string.Format(SetLoginSql, SQLHelpers.CreateSafeString(Name), WebApplicationAccess ? 1 : 0);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete()
        {
            using (var connection = GetRepositoryConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    _isSet = true;
                    command.CommandType = CommandType.Text;
                    command.CommandText = string.Format(DeleteLoginSql, SQLHelpers.CreateSafeString(Name));

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
