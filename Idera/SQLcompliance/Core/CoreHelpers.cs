using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Management;
using System.Reflection;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;

namespace Idera.SQLcompliance.Core
{
	/// <summary>
	/// Summary description for CoreHelpers.
	/// </summary>
	public class CoreHelpers
	{
        //private const string PROGRAM_FILE = @"C:\Program";

        #region Member Variables

        #endregion

        #region Properties

        //Fix for 5241
        public static int currentServerId = -1;
        public static int currentDatabaseId = -1;

		#endregion

		#region Construction/Destruction

		public CoreHelpers()
		{
		}

		#endregion

      public static void TurnOffCustomRemotingErrors() 
      {

         //Get the assembly that hosts System.Runtime.Remoting.RemoteConfiguration
         System.Reflection.Assembly remoting = System.Reflection.Assembly.GetAssembly(typeof(System.Runtime.Remoting.RemotingConfiguration));

         //Next get the internal static RemotingConfigHandler class
         Type remotingConfigHandler = remoting.GetType("System.Runtime.Remoting.RemotingConfigHandler");

         //Get the CustomErrorsModes enum (used to set the _errorMode)
         Type customErrorsModes = remoting.GetType("System.Runtime.Remoting.CustomErrorsModes");

         //Get the (non public static) error mode field
         FieldInfo errorMode = remotingConfigHandler.GetField("_errorMode", BindingFlags.Static | BindingFlags.NonPublic);

         //Get the "off" member of the enum
         FieldInfo mode = customErrorsModes.GetField("Off");

         //Set the error mode value to off
         errorMode.SetValue(null, mode.GetValue(null));

      }
      //-------------------------------------------------------------------------
      // ValidateTraceDirectory
      //-------------------------------------------------------------------------
      public static void
         ValidateTraceDirectory(
            string traceDirectory
         )
      {
         DirectoryInfo di = new DirectoryInfo(traceDirectory);

         if (di.FullName.Length > 180)
         {
            Exception e = new Exception(CoreConstants.Exception_TraceDirectoryNameTooLong);
            throw e;
         }

         // Create the directory if necessary
         if (!di.Exists)
         {
            //di.Create();
            int rc = InstallUtil.CreateDirAndGiveFullControl(di.FullName, "S-1-5-32-544");
            if (rc != 0)
            {
               string errmsg = String.Format("Cannot create specified trace directory.\n\nSystem Error: {0}", rc);
               throw new Exception(errmsg);
            }
         }

         CheckDirectoryPermissions(di.FullName);
      }

      //-------------------------------------------------------------------------
      // CheckDirectoryPermissions
      //-------------------------------------------------------------------------
      private static void
         CheckDirectoryPermissions(
            string directory
         )
      {
         // Test if agent can write to the directory
          string fileName  = String.Format("SQLCM{0}.tmp",DateTime.Now.ToString("yyyyMMdd-HHmmssfff").Replace("-",""));
          string testFile = Path.Combine(directory, fileName);

         using (FileStream fs = File.Create(testFile)) { }
         File.Delete(testFile);
      }

      public static string GetTableNameKey(string schema, string table)
      {
         if (schema == null || schema.Length == 0)
            return table;
         else
            return String.Format("{0}.{1}", schema, table);
      }

      public static void GetTableNameFromKey(string key, out string schema, out string table)
      {
         string[] parts = key.Split(".".ToCharArray());
         if (parts.Length > 1)
         {
            schema = parts[0];
            table = parts[1];
         }
         else
         {
            schema = "";
            table = key;
         }
      }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static string GetServiceAccount(string serviceName)
        {
            string logonName = string.Empty;
            SelectQuery query = new SelectQuery(string.Format("SELECT name, startname FROM Win32_Service WHERE name = '{0}'", serviceName));
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementBaseObject managementObject in searcher.Get())
                {
                    ManagementObject service = (ManagementObject)managementObject;
                    logonName = service["startname"].ToString();
                    break; // we need to find logon name for first available service
                }
            }

            //Getting user name from recovered domain user name with format 'username@domain'
            if (!logonName.Contains("\\") && logonName.Contains("@"))
            {
               try
               {
                  var userAccount = new WindowsIdentity(logonName);
                  logonName = userAccount.Name;
               }
               catch (Exception e)
               {
                  ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                          String.Format("Failed to get standard user name for domain user", logonName),
                                          e,
                                          true);
               }
            }

            if (logonName.Equals("NT AUTHORITY\\NetworkService")) 
            {
               logonName = "NT AUTHORITY\\Network Service";
            }
            
            return logonName;
        }

        public static bool CheckAndGrantDirectoryAccessPermissions(string directory, string userAccount, out Exception exception)
        {
            bool hasAclEntry = false;
            bool recheckAclEntry = true;
            exception = null;

            // form proper user account name
            if (userAccount.StartsWith(".\\"))
                userAccount = userAccount.Replace(".\\", string.Format("{0}\\", Environment.MachineName));

            try
            {
            RE_CHECK_ACL_ENTRY:

                // get directory access control
                DirectorySecurity security = Directory.GetAccessControl(directory);

                // get ACL
                AuthorizationRuleCollection accessControlList = security.GetAccessRules(true, true, typeof(NTAccount));
                foreach (FileSystemAccessRule accessControlEntry in accessControlList)
                {
                    // user account ACL entry found
                    if (userAccount.Equals(accessControlEntry.IdentityReference.Value,
                                           StringComparison.InvariantCultureIgnoreCase))
                    {
                        // permission allowed
                        if (accessControlEntry.AccessControlType == AccessControlType.Allow)
                        {
                            // has read, wite and delete rights
                            if ((accessControlEntry.FileSystemRights & FileSystemRights.Read) == FileSystemRights.Read &&
                                (accessControlEntry.FileSystemRights & FileSystemRights.Write) == FileSystemRights.Write &&
                                (accessControlEntry.FileSystemRights & FileSystemRights.Delete) == FileSystemRights.Delete)
                                hasAclEntry = true;
                        }

                        break;
                    }
                }

                if (!hasAclEntry)
                {
                    // try to grant permission
                    FileSystemAccessRule rule = new FileSystemAccessRule(userAccount, FileSystemRights.Read | FileSystemRights.Write | FileSystemRights.Delete, AccessControlType.Allow);
                    security.AddAccessRule(rule);

                    Directory.SetAccessControl(directory, security);

                    // recheck for ACL entry
                    if (recheckAclEntry)
                    {
                        recheckAclEntry = false;
                        goto RE_CHECK_ACL_ENTRY;
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return hasAclEntry;
        }

	    public static SecurityException IsRegistryKeyReadable(string registryKey)
	    {
            try
            {
                RegistryPermission permission = new RegistryPermission(RegistryPermissionAccess.Read, registryKey);
                permission.Demand();
            }
            catch (SecurityException ex)
            {
                return ex;
            }

	        return null;
	    }

        public static Exception IsDirectoryWritable(string path)
        {
            try
            {
                using (FileStream fs = File.Create(Path.Combine(path, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose)) { }
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public static bool IsDirectoryWritableBySql(string directory, string connectionString, out Exception exception)
        {
            bool isDirectoryWritable = true;
            exception = null;

            string permissionCheckFile = Path.Combine(directory, "SQLCM");
            string createFileSql = string.Format("EXEC @RetCode=sp_trace_create @TraceId=@TraceID OUT,@options=2, @tracefile=@TraceFileName, @maxfilesize=@MaxSize, @stoptime=@StopTime;");

            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine("  DECLARE @RetCode int");
            sqlBuilder.AppendLine("  DECLARE @TraceID int");
            sqlBuilder.AppendLine("  DECLARE @TraceFileName nvarchar(128)");
            sqlBuilder.AppendLine("  DECLARE @MaxSize bigint");
            sqlBuilder.AppendLine("  DECLARE @StopTime datetime");
            sqlBuilder.AppendLine("  DECLARE @FileDate varchar(32)");
            sqlBuilder.AppendLine("  DECLARE @return_code INT");
            sqlBuilder.AppendLine("  DECLARE @TraceInfoTable table ( [num] [int], [TraceID] [int], [FileName] [nvarchar] (245), [Options] [int], [MaxSize] [bigint], [StopTime] [DateTime], [Status] [int])");
            sqlBuilder.AppendLine();
            sqlBuilder.AppendLine("BEGIN");
            sqlBuilder.AppendLine("  	-- create file");
            sqlBuilder.AppendLine();
            sqlBuilder.AppendLine("SET @FileDate = CONVERT(CHAR(8),GETDATE(),112) + REPLACE(CONVERT(varchar(15),GETDATE(),114),':','')");
            sqlBuilder.AppendLine("SET @TraceFileName = " + "'" + permissionCheckFile + "'" + " + @FileDate;");
            sqlBuilder.AppendLine(createFileSql);
            sqlBuilder.AppendLine();
            sqlBuilder.AppendLine("IF( @RetCode != 0 )");
            sqlBuilder.AppendLine("BEGIN");
            sqlBuilder.AppendLine(" SELECT 'Failed to create file.'");
            sqlBuilder.AppendLine("END");
            sqlBuilder.AppendLine("ELSE");
            sqlBuilder.AppendLine(" exec sp_trace_setstatus @TraceID, 2");
            sqlBuilder.AppendLine();
            sqlBuilder.AppendLine("END");

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = sqlBuilder.ToString();
                command.CommandType = CommandType.Text;
                command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                object result = command.ExecuteScalar();
                if (result != null && !string.IsNullOrEmpty(result.ToString()))
                {
                    isDirectoryWritable = false;
                    exception = new Exception(result.ToString());
                }
            }
            catch (Exception ex)
            {
                isDirectoryWritable = false;
                exception = ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();

                DeletePermissionCheckFile(directory);
            }

	        return isDirectoryWritable;
	    }
        //SQLCM-5471 - To handle select sensitive columns from views
        public static bool IsObjectView(string databaseName, string objectName, string instanceName)
        {
            string connectionString = String.Format("server={0};" +
                                                    "integrated security=SSPI;" +
                                                    "Connect Timeout=30;" +
                                                    "Application Name='{1}';" +
                                                    "database={2};",
                                                    instanceName,
                                                    CoreConstants.DefaultSqlApplicationName,
                                                    databaseName);

            bool isObjectView = false;
            string query = string.Format(@"USE {0} SELECT TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME like '{1}'", databaseName, objectName);
            Repository readRepository = new Repository();
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                //readRepository.OpenConnection(databaseName);
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string objectType = reader.GetString(0);
                            if (objectType.Equals("VIEW"))
                                isObjectView = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                       "Failed to retrive the Object Type",
                                       e,
                                       true);
            }
            finally
            {
                //readRepository.CloseConnection();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return isObjectView;
        }
        //public static string GetViewDefinition(string databaseName, string objectName)
        //{
        //    string viewDefinition = string.Empty;
        //    string query = string.Format(@"SELECT OBJECT_DEFINITION (OBJECT_ID('{0}')) AS ObjectDefinition; ", objectName);
        //    Repository readRepository = new Repository();
        //    try
        //    {
        //        readRepository.OpenConnection(databaseName);
        //        using (SqlCommand cmd = new SqlCommand(query, readRepository.connection))
        //        {
        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    viewDefinition = reader.GetString(0);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        ErrorLog.Instance.Write(ErrorLog.Level.Debug,
        //                               "Failed to retrive the Object Type",
        //                               e,
        //                               true);
        //    }
        //    finally
        //    {
        //        readRepository.CloseConnection();
        //    }
        //    return viewDefinition;
        //}

        public static IList<KeyValuePair<string, string>> GetColumnsUsedInView(string databaseName, string objectName, string instanceName)
        {
            string connectionString = String.Format("server={0};" +
                                                    "integrated security=SSPI;" +
                                                    "Connect Timeout=30;" +
                                                    "Application Name='{1}';" +
                                                    "database={2};",
                                                    instanceName,
                                                    CoreConstants.DefaultSqlApplicationName,
                                                    databaseName);

            List<KeyValuePair<string, string>> columnsList = new List<KeyValuePair<string, string>>();
            //SQLCM-5674 v5.6
            string query = string.Format(@"USE {0} SELECT distinct 
	                                                VIEW_NAME,
                                                  TABLE_SCHEMA,
                                                  TABLE_NAME,
                                                  COLUMN_NAME
                                                FROM   INFORMATION_SCHEMA.VIEW_COLUMN_USAGE
                                                WHERE  VIEW_NAME = '{1}'
                                                ORDER BY
                                                   VIEW_NAME
                                                  ,TABLE_SCHEMA
                                                  ,TABLE_NAME
                                                  ,COLUMN_NAME", databaseName, objectName);
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                //readRepository.OpenConnection(databaseName);
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columnsList.Add(new KeyValuePair<string, string>(reader.GetString(2), reader.GetString(3)));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                       "Failed to retrive the Object Type",
                                       e,
                                       true);
            }
            finally
            {
                //readRepository.CloseConnection();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return columnsList;
        }
        //SQLCM-5471 - To handle select sensitive columns from views - END
        ///
        /// Remove Permission Check file.
        ///
        private static void DeletePermissionCheckFile(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            if (directoryInfo != null)
            {
                FileInfo[] fileList = directoryInfo.GetFiles("SQLCM*");

                if (fileList != null && fileList.Length > 0)
                {
                    foreach (FileInfo file in fileList)
                    {
                        try
                        {
                            File.Delete(file.FullName);
                        }
                        catch (Exception ex)
                        {
                            // Log the error
                            ErrorLog.Instance.Write(string.Format("Can't delete '{0}' file due to the following error: {1}", file.FullName, ex), ErrorLog.Severity.Error);
                        }
                    }
                }
            }
        }
    }
}
