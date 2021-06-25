using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Idera.SQLcompliance.Core
{
    class XEventHelper
    {
        private static Dictionary<string, Assembly> _assemblies = new Dictionary<string, Assembly>();

        public static Assembly  loadAssembly(string dll)
        {            
            Assembly assembly;
            if (_assemblies.TryGetValue(dll, out assembly))
                return assembly;
            string sqlPath = String.Empty;            
            try
            {
                int version = 0;
                using (RegistryKey sqlServerKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server"))
                {
                    foreach (string subKeyName in sqlServerKey.GetSubKeyNames())
                    {
                        if (subKeyName.Equals("130") || subKeyName.Equals("120")
                            || subKeyName.Equals("110") || subKeyName.Equals("100") || subKeyName.Equals("90"))
                        {
                            int temp = 0;
                            int.TryParse(subKeyName, out temp);
                            if (version < temp)
                            {
                                version = temp;
                            }
                        }
                    }
                    if (version != 0)
                    sqlPath = sqlServerKey.OpenSubKey(version.ToString()).GetValue("VerSpecificRootDir").ToString();
                }
                sqlPath = sqlPath.Substring(0, sqlPath.IndexOf(version.ToString()));
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("Error: Unable to get SQL Server directory. Exception: " + e.Message);
            }
            string start_search_path;
            string end_search_path;
            start_search_path = @"C:\Program Files\Microsoft SQL Server\";
            if (sqlPath != String.Empty && !sqlPath.Contains("Program Files (x86)"))
                start_search_path = sqlPath;
            end_search_path = @"Shared\";
            assembly = loadXEventLinqAssembly(start_search_path, end_search_path, dll);
            if (assembly != null)
                return assembly;
            start_search_path = @"C:\Windows\Microsoft.NET\assembly\GAC_64\" + dll.Substring(0,dll.IndexOf(".dll"));
            end_search_path = String.Empty;
            assembly = loadXEventLinqAssembly(start_search_path, end_search_path, dll);
            if (assembly != null)
                return assembly;

            start_search_path = @"C:\Windows\Microsoft.NET\assembly\GAC_32\" + dll.Substring(0, dll.IndexOf(".dll"));
            end_search_path = String.Empty;
            assembly = loadXEventLinqAssembly(start_search_path, end_search_path, dll);
            if (assembly != null)
                return assembly;
            return null;
        }

        public static Assembly loadXEventLinqAssembly(string start_search_path, string end_search_path, string dll)
        {
            Assembly assembly;
            if (!Directory.Exists(start_search_path))
                return null;
            DirectoryInfo start_dir = new DirectoryInfo(start_search_path);
            DirectoryInfo[] SQLDirs = start_dir.GetDirectories();
            foreach (DirectoryInfo d in SQLDirs)
            {
                string dll_path = Path.Combine(Path.Combine( d.FullName,end_search_path),dll);

                if (File.Exists(dll_path))
                {
                    try
                    {
                        assembly = Assembly.LoadFrom(dll_path);
                        _assemblies.Add(dll, assembly);
						return assembly;
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.Write("'"+dll+"' failed to load. Exception: "+ex.Message);
                    }
                }
            }
            return null;
        }

        public static Assembly XEventDependencyHandler(object sender, ResolveEventArgs args)
        {
            if (args.Name.IndexOf("Microsoft.SqlServer.XE.Core") > -1)
            {
                return loadAssembly("Microsoft.SqlServer.XE.Core.dll");
            }
            return null;
        }
    }
}
