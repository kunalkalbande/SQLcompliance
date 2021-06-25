using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;

using Microsoft.CSharp;
using Microsoft.Win32;

using Idera.SQLcompliance.Core.Agent;
using System.Globalization;

namespace Idera.SQLcompliance.Core.Triggers
{
    public class TriggerHelpers
    {

        #region Constants

        internal const string SharedAssemblyName = "SQLcomplianceTriggerCore_1";
        internal const string DMLTriggerNameFormat = "Trg_SQLcm_{0}";
        internal const string DMLTriggerNameFormatEx = "{0}.Trg_SQLcm_{1}";
        internal const string DMLAssemblyNameFormat = "Trg_SQLcm_{0}";
        internal const string DMLAssemblyNameFormatEx = "Trg_SQLcm_{0}_{1}";

        public const string RelativeAssemblyPath = @".";

        #endregion

        #region Trigger Management

        internal static string
           GetTriggerName(string schema, string table)
        {
            if (schema != null && schema.Length > 0)
                return String.Format(DMLTriggerNameFormatEx, schema, table);
            else
                return String.Format(DMLTriggerNameFormat, table);
        }

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        internal static bool
           DoesTriggerExist(SqlConnection conn,
                             string schema,
                             string table)
        {
            if (schema == null || schema.Length == 0)
                schema = "dbo";
            string stmt = String.Format("SELECT count(t1.name) FROM sys.triggers t1, sys.tables t2 WHERE t1.name = '{0}' AND " +
                                         "t2.name = '{1}' AND t2.schema_id = schema_id('{2}' ) AND t1.parent_id = t2.object_id",
                                         GetTriggerName(table), table, schema);
            return ExecuteInt(conn, stmt) == 0 ? false : true;
        }

        //-----------------------------------------------------------------------------------------------------
        // GetTriggerStatus: check the status of a trigger
        //   returns true if trigger exists
        //           disabled: indicates whether the trigger is disabled.
        //           renamed: whether the base table is renamed.
        //-----------------------------------------------------------------------------------------------------
        internal static bool
           GetTriggerStatus(SqlConnection conn,
                             string schema,
                             string table,
                             out bool disabled,
                             out bool renamed,
                             out string newName)
        {
            if (schema == null || schema.Length == 0)
                schema = "dbo";
            string stmt = String.Format("SELECT t1.is_disabled disabled, t2.name tableName" +
                                         " FROM sys.triggers t1, sys.tables t2 WHERE t1.name = '{0}' AND " +
                                         " t2.schema_id = schema_id('{1}' ) AND t1.parent_id = t2.object_id",
                                         GetTriggerName(table), schema);
            using (SqlDataReader reader = ExecuteReader(conn, stmt))
            {
                if (reader.Read())
                {
                    disabled = reader.GetBoolean(0);
                    newName = SQLHelpers.GetString(reader, 1);
                    renamed = newName != table;
                    return true;
                }
                else
                {
                    disabled = false;
                    renamed = false;
                    newName = table;
                    return false;
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        internal static bool
           CreateCLRDMLTrigger(SqlConnection conn,
                                string table,
                                string triggerName,
                                string method)
        {
            return CreateCLRDMLTrigger(conn, table, triggerName, method, false);
        }

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        internal static bool
           CreateCLRDMLTrigger(SqlConnection conn,
                                string table,
                                string triggerName,
                                string method,
                                bool recreate)
        {
            return CreateCLRDMLTrigger(conn, null, table, triggerName, method, recreate);
        }

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        internal static bool
           CreateCLRDMLTrigger(SqlConnection conn,
                                string schema,
                                string table,
                                string triggerName,
                                string method)
        {
            return CreateCLRDMLTrigger(conn, schema, table, triggerName, method, true);
        }

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        internal static bool
           CreateCLRDMLTrigger(SqlConnection conn,
                                string schema,
                                string table,
                                string triggerName,
                                string method,
                                bool recreate)
        {
            if (schema == null || schema.Length == 0)
                schema = "dbo";

            try
            {
                StringBuilder stmt = new StringBuilder();
                if (recreate)
                {
                    DropTrigger(conn, schema, triggerName);
                }
                stmt.AppendFormat("CREATE TRIGGER [{0}].[{1}] ON [{0}].[{2}] " +
                                             "FOR INSERT, UPDATE, DELETE " +
                                             "AS EXTERNAL NAME {3}",
                                             schema,
                                             triggerName,
                                             table,
                                             method);

                ExecuteNonQuery(conn, stmt.ToString());
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         String.Format(
                                            "Trigger {0}.{1} is created on {0}.{2}.",
                                            schema,
                                            triggerName,
                                            table));
                return true;
            }
            catch (SqlException se)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   String.Format("An error occurred creating trigger: [{0}].[{1}]",
                                  schema,
                                  triggerName),
                   se.Message,
                   se,
                   ErrorLog.Severity.Warning,
                   true);
                if (se.Number != 2714 || recreate)
                    // ingnore trigger already exists error
                    throw;
                else
                    return true;
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   String.Format("An error occurred creating trigger: [{0}].[{1}]",
                                  schema,
                                  triggerName),
                   e.Message,
                   e,
                   true);
                throw;
            }
        }

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        internal static void EnableTrigger(SqlConnection conn,
                              string schema,
                              string trigger,
                              string table)
        {
            string schemaName = "";

            if (schema != null && schema.Length > 0)
                schemaName = String.Format("[{0}].", schema);
            string stmt =
               String.Format("ENABLE TRIGGER {0}[{1}] ON {0}[{2}]",
                              schemaName,
                              trigger,
                              table);
            try
            {
                ExecuteNonQuery(conn, stmt);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format(
                                            "TriggerHelpers: An error occurred enabling trigger {0}[{1}] ON {0}[{2}]. Error: {3}.",
                                            schemaName,
                                            trigger,
                                            table,
                                            e.Message),
                                         ErrorLog.Severity.Warning);
                throw;
            }
        }

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        internal static void DisableTrigger(SqlConnection conn,
                              string schema,
                              string trigger,
                              string table)
        {
            string schemaName = "";

            if (schema != null && schema.Length > 0)
                schemaName = String.Format("[{0}].", schema);
            string stmt =
               String.Format("DISABLE TRIGGER {0}[{1}] ON {0}[{2}]",
                              schemaName,
                              trigger,
                              table);
            try
            {
                ExecuteNonQuery(conn, stmt);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format(
                                            "TriggerHelpers: An error occurred disabling trigger {0}[{1}] ON {0}[{2}]. Error: {3}.",
                                            schemaName,
                                            trigger,
                                            table,
                                            e.Message),
                                         ErrorLog.Severity.Warning);
                throw;
            }
        }

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        internal static bool
           DropTrigger(SqlConnection conn,
                        string trigger)
        {

            return DropObject(conn, null, "TRIGGER", trigger, null);
        }

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        internal static bool
           DropTrigger(SqlConnection conn,
                        string schema,
                        string trigger)
        {
            bool success = true;
            try
            {
                StringBuilder stmt = new StringBuilder();
                stmt.AppendFormat(
                   "IF EXISTS( SELECT t1.name FROM sys.triggers t1, sys.tables t2 WHERE t1.name = '{0}' AND " +
                      "t2.schema_id = schema_id('{1}' ) AND t1.parent_id = t2.object_id )",
                   trigger,
                   schema);
                stmt.AppendFormat("DROP TRIGGER [{0}].[{1}];", schema, trigger);
                ExecuteNonQuery(conn, stmt.ToString());
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         String.Format(
                                            "Trigger {0}.{1} is dropped.",
                                            schema ?? "dbo", trigger));
            }
            catch (Exception e)
            {
                string msg = String.Format("An error occurred dropping trigger {0}.{1}.",
                                            schema ?? "dbo",
                                            trigger);
                ErrorLog.Instance.Write(msg, e, ErrorLog.Severity.Warning, true);
                return false;
            }
            return success;
        }

        internal static string
           GetTriggerName(string table)
        {
            return GetTriggerName(null, table);
        }

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        public static DMLTriggerInfo[]
           GetTriggers(string instance,
                          string database)
        {
            using (SqlConnection conn = new SqlConnection(SQLcomplianceAgent.CreateConnectionString(instance, database)))
            {
                return GetTriggers(conn, database);
            }
        }

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        public static DMLTriggerInfo[]
           GetTriggers(SqlConnection conn,
                          string database)
        {
            List<DMLTriggerInfo> triggers = new List<DMLTriggerInfo>();
            try
            {
                string query =
                   "SELECT tr.name name, s.name schemaName, t.name tableName, tr.modify_date, tr.is_disabled" +
                   " FROM sys.triggers tr, sys.tables t, sys.schemas s" +
                   " WHERE tr.name like 'Trg_SQLcm_%' AND tr.type = 'TA' AND tr.parent_id = t.object_id AND t.schema_id = s.schema_id";
                using (DataTable table = ExecuteTable(conn, query))
                {
                    foreach (DataRow row in table.Rows)
                    {
                        DMLTriggerInfo info = new DMLTriggerInfo(conn.DataSource, database, row);
                        triggers.Add(info);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                   String.Format("An error occurred getting SQLcompliance triggers.  Instance = {0}.  Database = {1}.",
                                  conn.DataSource, database),
                   e,
                   true);
                throw;
            }

            return triggers.ToArray();
        }

        #endregion

        #region Assembly Management

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        internal static string
          GetAssemblyName(string schema, string table)
        {
            if (schema != null && schema.Length > 0)
                return String.Format(DMLAssemblyNameFormatEx, schema, table);
            else
                return String.Format(DMLAssemblyNameFormat, table);
        }

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        internal static string GetFullShardAssemblyName()
        {
            FileInfo fInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string filePath = Path.Combine(fInfo.DirectoryName, RelativeAssemblyPath);
            return Path.Combine(filePath, SharedAssemblyName + ".dll");
        }

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        internal static string GetFullShardAssemblyNameEx()
        {
            FileInfo fInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string filePath = Path.Combine(fInfo.DirectoryName, RelativeAssemblyPath);
            return Path.Combine(filePath, SharedAssemblyName + "Ex.dll");
        }

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        internal static bool
           DoesAssemblyExist(SqlConnection conn,
                              string assemblyName)
        {
            string stmt =
               String.Format("SELECT count(name) FROM sys.assemblies where name = '{0}'",
                              assemblyName);
            return ExecuteInt(conn, stmt) == 0 ? false : true;
        }

        internal static bool
           CreateAssembly(SqlConnection conn,
                           string assemblyName,
                           string fileName)
        {
            return CreateAssembly(conn, assemblyName, fileName, false);
        }

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        internal static bool
           CreateAssembly(SqlConnection conn,
                           string assemblyName,
                           string fileName,
                           bool recreate)
        {
            StringBuilder stmt = new StringBuilder();
            try
            {
                if (!String.IsNullOrEmpty(conn.ServerVersion) && Int32.Parse(conn.ServerVersion.Split('.')[0]) > 13)
                {
                    string sql = "DECLARE @asmBin varbinary(max)={0}; DECLARE @hash varbinary(64); Declare @existing_hash varbinary(64); SELECT @hash = HASHBYTES('SHA2_512', @asmBin);" +
                                 " set @existing_hash = null;" +
                                 " select @existing_hash = hash FROM sys.trusted_assemblies where description = '{1}' " +
                                 " IF @existing_hash is not null" +
                                 " BEGIN" +
                                 " EXEC sp_drop_trusted_assembly @hash = @existing_hash;" +
                                 " EXEC sys.sp_add_trusted_assembly @hash = @hash, @description = [{1}];" +
                                 " END" +
                                 " ELSE" +
                                 " BEGIN" +
                                 " EXEC sys.sp_add_trusted_assembly @hash = @hash, @description = [{1}];" +
                                 " END ";
                    string hexString = GetHexString(fileName);
                    string dbName = conn.Database;
                    string description = dbName + "_" + assemblyName;
                    sql = String.Format(sql,
                                        hexString,
                                        description);
                    stmt.Append(sql);
                }
                if (recreate)
                    stmt.AppendFormat(
                       "IF EXISTS (SELECT name FROM sys.assemblies where name = '{0}') DROP ASSEMBLY [{0}];",
                       assemblyName);
                else
                    stmt.AppendFormat(
                       "IF NOT EXISTS (SELECT name FROM sys.assemblies where name = '{0}') ",
                       assemblyName);

                stmt.AppendFormat(
                   "CREATE ASSEMBLY [{0}] FROM '{1}' WITH PERMISSION_SET = SAFE",
                   assemblyName,
                   fileName);
                return ExecuteNonQuery(conn, stmt.ToString());
            }
            catch (Exception e)
            {
                StringBuilder msg = new StringBuilder();
                msg.AppendFormat("An error occurred creating assembly {0}.\n\r", assemblyName);
                msg.AppendFormat("Instance: {0}.  Database: {1}.  File: {2}.",
                                  conn.DataSource, conn.Database, fileName);

                ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg.ToString(), e, ErrorLog.Severity.Warning, true);
                throw;
            }
        }

        internal static string GetHexString(string assemblyPath)
        {
            if (!Path.IsPathRooted(assemblyPath))
                assemblyPath = Path.Combine(Environment.CurrentDirectory, assemblyPath);

            StringBuilder builder = new StringBuilder();
            builder.Append("0x");

            using (FileStream stream = new FileStream(assemblyPath,
                  FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int currentByte = stream.ReadByte();
                while (currentByte > -1)
                {
                    builder.Append(currentByte.ToString("X2", CultureInfo.InvariantCulture));
                    currentByte = stream.ReadByte();
                }
            }

            return builder.ToString();
        }
        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        internal static bool
           DropAssembly(SqlConnection conn,
                         string assembly)
        {
            try
            {
                string stmt =
                   String.Format(
                      "IF EXISTS (SELECT name FROM sys.assemblies where name = '{0}') DROP ASSEMBLY [{0}];",
                      assembly);
                ExecuteNonQuery(conn, stmt);
                return true;
            }
            catch (Exception e)
            {
                StringBuilder msg = new StringBuilder();
                msg.AppendFormat("An error occurred dropping assembly {0}.\n\r", assembly);
                msg.AppendFormat("Instance: {0}.  Database: {1}.",
                                  conn.DataSource, conn.Database);

                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         msg.ToString(),
                                         e,
                                         ErrorLog.Severity.Warning, true);
                throw;
            }
        }

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        public static List<string>
           GetAssemblies(string instance, string database)
        {
            List<string> assemblies;
            List<DateTime> modifiedDates;
            GetAssemblies(instance, database, out assemblies, out modifiedDates);
            return assemblies;
        }


        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        internal static void
           GetAssemblies(string instance,
                          string database,
                          out List<string> assemblies,
                          out List<DateTime> modifiedDates)
        {
            try
            {
                assemblies = new List<string>();
                modifiedDates = new List<DateTime>();
                using (SqlConnection conn = GetConnection(instance, database))
                {
                    string query =
                       "SELECT name, modify_date FROM sys.assemblies WHERE name like 'Trg_SQLcm_%' OR name = 'SQLcomplianceTriggerCore'";
                    using (SqlDataReader reader = ExecuteReader(conn, query))
                    {
                        while (reader.Read())
                        {
                            assemblies.Add(reader.GetString(0));
                            modifiedDates.Add(reader.GetDateTime(1));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(
                   String.Format("An error occurred getting SQLcompliance assemblies.  Instance = {0}.  Database = {1}.",
                                   instance, database),
                   e,
                   true);
                throw;
            }
        }

        #endregion

        #region Schema and Tables

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        internal static bool DoesSchemaExist(SqlConnection conn,
                                              string schemaName)
        {
            string stmt =
               String.Format("SELECT count(name) FROM sys.schemas where name = '{0}'",
                              schemaName);
            return ExecuteInt(conn, stmt) == 0 ? false : true;
        }

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        internal static bool DoesTableExist(SqlConnection conn,
                                              string schemaName,
                                              string tableName)
        {
            if (schemaName == null || schemaName.Length == 0)
                schemaName = "dbo";
            string stmt =
               String.Format("SELECT count(t.name) FROM sys.tables t where t.name = '{0}' and t.schema_id = schema_id('{1}')",
                              tableName, schemaName);
            return ExecuteInt(conn, stmt) == 0 ? false : true;
        }

        #endregion

        #region Code Generation

        //----------------------------------------------------------------------------------
        // GenerateDMLTriggerAssembly
        //----------------------------------------------------------------------------------
        internal static bool
           GenerateDMLTriggerAssembly(string filename,
                                       string schema,
                                       string table,
                                       string[] pkCols,
                                       int rowLimit)
        {
            var providerOptions = new Dictionary<string, string>();
            //providerOptions.Add("CompilerVersion", "v2.0");
            providerOptions.Add("CompilerVersion", ComplilerVersion());
            CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);
            StringBuilder source = new StringBuilder(2000);
            bool success = true;

            if (schema != null && schema.Length > 0)
                table = String.Format("{0}.{1}", schema, table);

            // usings
            source.Append("using System;");
            source.Append("using Microsoft.SqlServer.Server;");

            // Declare the class
            source.Append("public class CLRTriggers {");

            // members
            // Embed primary key columns
            source.Append("private static readonly string[] pkCols = { ");
            foreach (string col in pkCols)
            {
                source.Append(String.Format("\"{0}\", ", col));
            }
            source.Append("};");
            source.AppendFormat("private static readonly string table =  \"{0}\";", table);
            source.AppendFormat("private static readonly int rowLimit = {0};", rowLimit);

            // Trigger method
            source.Append("   public static void DMLTrigger() {");
            source.Append("      try {");
            source.Append("         TriggerCore core = new TriggerCore();");
            source.Append("         TriggerCore.GenerateTraceEvents( table, pkCols, rowLimit);");
            source.Append("      }");
            source.Append("      catch (Exception e) { SqlContext.Pipe.Send( \"Triggere error: \" + e.Message );} }");

            // Entry point
            source.Append("   public static void Main(string[] args) {}");

            source.Append("}");


            String[] referenceAssemblies = { "System.dll", "System.Data.dll", GetFullShardAssemblyName() };
            CompilerParameters parameters = new CompilerParameters(referenceAssemblies, filename);

            parameters.GenerateExecutable = true;
            CompilerResults cr = provider.CompileAssemblyFromSource(parameters, source.ToString());
            if (cr.NativeCompilerReturnValue != 0)
            {
                StringBuilder errors = new StringBuilder("Error occurred generating DML trigger assembly for ");
                errors.AppendFormat("{0}.{1}:\n\r", schema, table);
                for (int i = 1; i <= cr.Errors.Count; i++)
                {
                    errors.AppendFormat("[{0}]: {1}\n\r", i, cr.Errors[i - 1]);
                }
                ErrorLog.Instance.Write(errors.ToString(), ErrorLog.Severity.Warning);
                success = false;
            }

            return success;
        }
        //----------------------------------------------------------------------------------
        // GenerateDMLTriggerAssembly
        //----------------------------------------------------------------------------------
        internal static bool
           GenerateDMLTriggerAssembly(string filename,
                                       string schema,
                                       string table,
                                       string[] pkCols,
                                       int rowLimit,
                                       string[] columns)
        {

            var providerOptions = new Dictionary<string, string>();
            //providerOptions.Add("CompilerVersion", "v2.0");
            providerOptions.Add("CompilerVersion", ComplilerVersion());
            CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);
            StringBuilder source = new StringBuilder(2000);
            bool success = true;

            if (schema != null && schema.Length > 0)
                table = String.Format("{0}.{1}", schema, table);

            // usings
            source.Append("using System;");
            source.Append("using Microsoft.SqlServer.Server;");

            // Declare the class
            source.Append("public class CLRTriggers {");

            // members
            // Embed primary key columns
            source.Append("private static readonly string[] pkCols = { ");
            foreach (string col in pkCols)
            {
                source.Append(String.Format("\"{0}\", ", col));
            }
            source.Append("};");
            source.AppendFormat("private static readonly string table =  \"{0}\";", table);
            // Embed selected audit columns
            source.Append("private static readonly string[] auditedColumns = { ");
            foreach (string col in columns)
            {
                source.Append(String.Format("\"{0}\", ", col));
            }
            source.Append("};");
            source.AppendFormat("private static readonly int rowLimit = {0};", rowLimit);

            // Trigger method
            source.Append("   public static void DMLTrigger() {");
            source.Append("      try {");
            source.Append("         TriggerCore core = new TriggerCore();");
            source.Append("         TriggerCore.GenerateTraceEvents( table, pkCols, auditedColumns, rowLimit);");
            source.Append("      }");
            source.Append("      catch (Exception e) { SqlContext.Pipe.Send( \"Triggere error: \" + e.Message );} }");

            // Entry point
            source.Append("   public static void Main(string[] args) {}");

            source.Append("}");


            String[] referenceAssemblies = { "System.dll", "System.Data.dll", GetFullShardAssemblyName() };
            CompilerParameters parameters = new CompilerParameters(referenceAssemblies, filename);

            parameters.GenerateExecutable = true;
            CompilerResults cr = provider.CompileAssemblyFromSource(parameters, source.ToString());
            if (cr.NativeCompilerReturnValue != 0)
            {
                StringBuilder errors = new StringBuilder("Error occurred generating DML trigger assembly for ");
                errors.AppendFormat("{0}.{1}:\n\r", schema, table);
                for (int i = 1; i <= cr.Errors.Count; i++)
                {
                    errors.AppendFormat("[{0}]: {1}\n\r", i, cr.Errors[i - 1]);
                }
                ErrorLog.Instance.Write(errors.ToString(), ErrorLog.Severity.Warning);
                success = false;
            }

            return success;
        }

        public static string ComplilerVersion()
        {
            string ComplilerVersion = string.Empty;
            using (RegistryKey ndpKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").
                OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
            {
                foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                {
                    if (versionKeyName.StartsWith("v"))
                    {
                        string Framework = versionKeyName.Remove(0, 1);
                        if (Framework.StartsWith("3.5"))
                        {
                            return ComplilerVersion = "v2.0";
                        }
                        if (Framework.StartsWith("4"))
                        {
                            return ComplilerVersion = "v4.0";
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(ComplilerVersion))
            {
                return "v2.0";
            }
            else
            {
                return ComplilerVersion;
            }
        }
        /* CodeDome version
      *
        internal static bool 
        GenerateDMLTriggerAssembly( string name,
                                    string [] pkCols,
                                    int rowLimit )
     {
        /* CodeDome 
        string[] pkCols; // Primary key columns
        string fullName = ""; // Full name of generated dll
        Stream strm = File.Open( name, FileMode.CreateNew );
        StreamWriter writer = new StreamWriter( strm );

        CSharpCodeProvider provider = new CSharpCodeProvider();
        ICodeGenerator generator = provider.CreateGenerator( writer );
        CodeGeneratorOptions cgOptions = new CodeGeneratorOptions();


        // Usings
        CodeSnippetCompileUnit cu1 = new CodeSnippetCompileUnit( "using System" );
        CodeSnippetCompileUnit cu2 = new CodeSnippetCompileUnit("using System.IO");
        generator.GenerateCodeFromCompileUnit( cu1, writer, cgOptions );
        generator.GenerateCodeFromCompileUnit( cu2, writer, cgOptions );
        writer.WriteLine();

        // Declare class
        CodeTypeDeclaration declaration = new CodeTypeDeclaration( );
        declaration.IsClass = true;
        declaration.Name = "SQLCmTrigger";
        declaration.TypeAttributes = TypeAttributes.Public;

        // Class members
        writer.WriteLine();
        CodeMemberField primaryKeyCols = new CodeMemberField( "string []", "pkCols" );
      */

        #endregion

        #region Utilities

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        public static SqlConnection GetConnection(string instance, string database)
        {
            SQLInstance si = SQLcomplianceAgent.Instance.GetSQLInstance(instance);
            return si.GetConnection(database);
        }

        internal static string GetSaveInstancename(string instance)
        {
            string safeInstanceName;

            int index = instance.IndexOf(@"\");
            if (index > 0)
            {
                safeInstanceName = instance.Substring(0, index)
                                   + "_"
                                   +
                                   instance.Substring(index + 1, instance.Length - index - 1);
            }
            else
                safeInstanceName = instance;
            return safeInstanceName;
        }


        #endregion

        #region DAL

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        private static bool ExecuteNonQuery(SqlConnection conn,
                                             string stmt)
        {
            using (SqlCommand cmd = new SqlCommand(stmt, conn))
            {
                cmd.ExecuteNonQuery();
                return true;
            }
        }


        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        public static SqlDataReader ExecuteReader(SqlConnection conn,
                                             string stmt)
        {
            using (SqlCommand cmd = new SqlCommand(stmt, conn))
            {
                return cmd.ExecuteReader();
            }
        }

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        public static DataTable ExecuteTable(SqlConnection conn,
                                             string stmt)
        {
            SqlDataAdapter da = new SqlDataAdapter(stmt,
                                                  conn);
            da.SelectCommand.CommandTimeout = CoreConstants.sqlcommandTimeout;
            DataTable table = new DataTable();
            da.Fill(table);

            return table;

        }

        public static int ExecuteInt(SqlConnection conn, string stmt)
        {
            using (SqlCommand cmd = new SqlCommand(stmt, conn))
            {
                return (int)cmd.ExecuteScalar();
            }
        }

        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        public static bool DropObject(SqlConnection conn, string type, string name, string options)
        {
            return DropObject(conn, type, null, name, options);
        }


        //-----------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------
        public static bool DropObject(SqlConnection conn, string type, string schema, string name, string options)
        {
            string schemaName = "";

            if (schema != null && schema.Length > 0)
                schemaName = String.Format("[{0}].", schema);
            string stmt =
               String.Format("DROP {0} {1}[{2}] {3}",
                              type,
                              schemaName,
                              name,
                              options ?? "");
            try
            {
                return ExecuteNonQuery(conn, stmt);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         String.Format(
                                            "TriggerHelpers: An error occurred dropping {0} {1}. {2}.",
                                            name,
                                            type,
                                            e.Message),
                                         ErrorLog.Severity.Warning);
                throw;
            }
        }
        #endregion
    }
}