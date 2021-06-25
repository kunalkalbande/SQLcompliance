using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Triggers;

namespace Idera.SQLcompliance.Cluster
{
   public class ProgressBarHelper
   {
      enum FilenameParts
      {
         TrgLiteral,
         SQLcmLiteral,
         SchemaName,
         TableName
      }

      static private List<Instance> instanceList = new List<Instance>();

      public class Instance
      {
         private string name;
         private IDictionary<string, Database> databases = new Dictionary<string, Database>();

         public IDictionary<string, Database> Databases
         {
            get { return databases; }
            set { databases = value; }
         }

         public string Name
         {
            get { return name; }
            set { name = value; }
         }

         public void AddDatabase(Database database)
         {
            if (!databases.ContainsKey(database.Name))
               databases.Add(database.Name, database);
         }
      }

      public class Database
      {
         private string name;
         private IDictionary<string, Table> tables = new Dictionary<string, Table>();

         public IDictionary<string, Table> Tables
         {
            get { return tables; }
            set { tables = value; }
         }

         public string Name
         {
            get { return name; }
            set { name = value; }
         }
         public void AddTable(Table table)
         {
            if (!tables.ContainsKey(table.Name))
               tables.Add(table.Name, table);
         }
      }

      public class Table
      {
         public string Name;
         public string SchemaName;
         public string TriggerName;
      }

      public static void MoveTriggerAssemblies()
      {
         string connectionString = "";
         try
         {
            FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            List<ProgressBarHelper.Instance> instanceTriggerInfo = GetTriggerInfo(Path.Combine(fileInfo.DirectoryName, "Assemblies"));
            
            foreach (ProgressBarHelper.Instance instance in instanceTriggerInfo)
            {
               foreach (KeyValuePair<string, ProgressBarHelper.Database> kvpDatabase in instance.Databases)
               {
                  ProgressBarHelper.Database database = kvpDatabase.Value;

                  connectionString = CreateConnectionString(instance.Name.Replace('_', '\\'), database.Name);
                  using (SqlConnection conn = new SqlConnection(connectionString))
                  {
                     conn.Open();
                     foreach (KeyValuePair<string, ProgressBarHelper.Table> kvpTable in database.Tables)
                     {
                        ProgressBarHelper.Table table = kvpTable.Value;

                        DropTrigger(conn, table.SchemaName, table.TriggerName);
                        string assemblyName = String.Format("Trg_SQLcm_{0}_{1}", table.SchemaName, table.Name);
                        DeleteAssembly(instance.Name, database.Name, table.SchemaName, assemblyName);
                        DropAssembly(conn, assemblyName);
                     }
                     DropSharedAssembly(conn);
                     conn.Close();
                  }
               }
            }
         }
         catch (Exception e)
         {
            //Ignore all exceptions.  We will only try to remove the trigger if it exists.  Just log it and move on.
            StreamWriter sw = new StreamWriter("output.txt");
            sw.WriteLine("Unable to get or update trigger info.");
            sw.WriteLine(" ");
            sw.WriteLine(String.Format("Connection String:{0}", connectionString));
            sw.WriteLine(" ");
            sw.Write(e.ToString());
            sw.Close();
         }
      }

      private static string CreateConnectionString(string instanceName, string database)
      {
         if (String.IsNullOrEmpty(instanceName) || String.IsNullOrEmpty(database))
            return null;

         string connStr = String.Format("server={0};integrated security=SSPI;database={1};Connect Timeout=30;Application Name='{2}'",
                                        instanceName,
                                        database,
                                        CoreConstants.ManagementConsoleName);
         return connStr;
      }

      #region dropTriggerAssembly

      //we only need to drop the 3.2 version of the shared trigger core.
      private static void DropSharedAssembly(SqlConnection conn)
      {
         string stmt = "IF EXISTS (SELECT name FROM sys.assemblies where name = 'SQLcomplianceTriggerCore_1') DROP ASSEMBLY [SQLcomplianceTriggerCore_1];";

         using (SqlCommand cmd = new SqlCommand(stmt.ToString(), conn))
         {
            cmd.ExecuteNonQuery();
         }
      }

      private static void DropTrigger(SqlConnection conn, string schema, string triggerName)
      {
         StringBuilder stmt = new StringBuilder();
         stmt.AppendFormat("IF EXISTS( SELECT t1.name FROM sys.triggers t1, sys.tables t2 WHERE t1.name = '{0}' AND " +
                           "t2.schema_id = schema_id('{1}' ) AND t1.parent_id = t2.object_id )",
                           triggerName,
                           schema);
         stmt.AppendFormat("DROP TRIGGER [{0}].[{1}];", schema, triggerName);

         using (SqlCommand cmd = new SqlCommand(stmt.ToString(), conn))
         {
            cmd.ExecuteNonQuery();
         }
      }

      private static void DeleteAssembly(string instance, string database, string schema, string triggerName)
      {
         string filename = GetFullName(instance, database, schema, triggerName);
         FileInfo fi = new FileInfo(filename);
         
         if (fi.Exists)
            File.Delete(filename);
      }

      private static void DropAssembly(SqlConnection conn, string assemblyName)
      {
         string stmt = String.Format("IF EXISTS (SELECT name FROM sys.assemblies where name = '{0}') DROP ASSEMBLY [{0}];", assemblyName);
         using (SqlCommand cmd = new SqlCommand(stmt.ToString(), conn))
         {
            cmd.ExecuteNonQuery();
         }
      }

      private static string GetFullName(string instance, string database, string schema, string tableName)
      {
         FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
         string directoryName = fileInfo.DirectoryName;

         string dllPath = Path.Combine(directoryName, "Assemblies");

         dllPath = Path.Combine(dllPath, GetSafeInstancename(instance));
         dllPath = Path.Combine(dllPath, database);

         if (schema != null && schema.Length > 0)
         {
            dllPath = Path.Combine(dllPath, schema);
         }
         return Path.Combine(dllPath, tableName + ".dll");
      }

      private static string GetSafeInstancename(string instance)
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

      #region GetTriggerInformation

      private static List<Instance> GetTriggerInfo(string path)
      {
         DirectoryInfo assemblies = new DirectoryInfo(path);
         DirectoryInfo[] instances = assemblies.GetDirectories();
         Instance instanceObj = null;

         foreach (DirectoryInfo instance in instances)
         {
            instanceObj = new Instance();
            instanceObj.Name = instance.Name;
            instanceObj.Name.Replace('_', '\\');
            ProcessDatabases(instanceObj, instance.FullName);

            if (instanceList == null)
               instanceList = new List<Instance>();
            instanceList.Add(instanceObj);
         }
         return instanceList;
      }

      private static void ProcessDatabases(Instance instanceObj, string path)
      {
         DirectoryInfo instances = new DirectoryInfo(path);
         DirectoryInfo[] databases = instances.GetDirectories();
         Database databaseObj = null;

         foreach (DirectoryInfo database in databases)
         {
            databaseObj = new Database();
            databaseObj.Name = database.Name;
            ProcessSchema(databaseObj, database.FullName);
            instanceObj.AddDatabase(databaseObj);
         }
      }

      private static void ProcessSchema(Database databaseObj, string path)
      {
         DirectoryInfo databases = new DirectoryInfo(path);
         DirectoryInfo[] schemas = databases.GetDirectories();

         foreach (DirectoryInfo schema in schemas)
         {
            ProcessTables(databaseObj, schema.Name, schema.FullName);
         }
      }

      private static void ProcessTables(Database databaseObj, string schema, string path)
      {
         DirectoryInfo schemas = new DirectoryInfo(path);
         FileInfo[] triggers = schemas.GetFiles("Trg_SQLcm_*");

         foreach (FileInfo trigger in triggers)
         {
            Table table = null;
            string[] parts = trigger.Name.Split('_');

            if ((parts != null) && (parts.Length > 0))
            {
               try
               {
                  //remove .dll from the table name.
                  int index = parts[(int)FilenameParts.TableName].LastIndexOf(".dll");
                  parts[(int)FilenameParts.TableName] = parts[(int)FilenameParts.TableName].Substring(0, index);
                  table = new Table();
                  table.TriggerName = String.Format("{0}_{1}_{2}", parts[(int)FilenameParts.TrgLiteral], parts[(int)FilenameParts.SQLcmLiteral], parts[(int)FilenameParts.TableName]);
                  table.Name = parts[(int)FilenameParts.TableName];
                  table.SchemaName = schema;
                  databaseObj.AddTable(table);
               }
               catch
               {
                  //just ignore any errors. It isn't possible to recover from them anyway.
               }
            }
         }
      }

      #endregion

   }
}
