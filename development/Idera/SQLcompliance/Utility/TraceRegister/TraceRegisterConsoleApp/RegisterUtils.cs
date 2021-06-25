using System;
using Microsoft.Win32;
using System.Data.SqlClient;

namespace Idera.SQLcompliance.Utility.TraceRegisterConsoleApp
{
   public static class RegisterUtils
   {
      public const string CollectionService_RegKey = @"Software\Idera\SQLcompliance\CollectionService";
      public const string CollectionService_RegKey_CM = @"Software\Idera\SQLCM\CollectionService";
      public const string CollectionService_RegVal_TraceDir = "TraceDirectory";

      //--------------------------------------------------------
      // Gets the Collection server trace directory from registry key
      //--------------------------------------------------------
      public static string GetCollectionServerTraceDirectory()
      {
         string traceDir = "";
         RegistryKey rk = null;
         RegistryKey rks = null;

         try
         {
            rk = Registry.LocalMachine;
            rks = rk.CreateSubKey(CollectionService_RegKey);
            traceDir = (string)rks.GetValue(CollectionService_RegVal_TraceDir, "");
            if (traceDir == "")
            {
               rks = rk.CreateSubKey(CollectionService_RegKey_CM);
               traceDir = (string)rks.GetValue(CollectionService_RegVal_TraceDir, "");
            }
         }
         catch
         {
            traceDir = "";
         }
         finally
         {
            if (rks != null) rks.Close();
            if (rk != null) rk.Close();
         }

         return traceDir;
      }

      //--------------------------------------------------------
      // Gets the SQL server instance name from registry key
      //--------------------------------------------------------
      public static String getDefaultSQLServerInstance()
      {
         String SQLServerInstance = String.Empty;

         try
         {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(CollectionService_RegKey_CM))
            {
               if (key != null)
               {
                  SQLServerInstance = (String)key.GetValue("ServerInstance");
               }
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine(String.Format("Error: Could not get SQL Server Instance Name: {0}", ex.Message));
         }
         return SQLServerInstance;
      }

      //--------------------------------------------------------
      // Verifies that they are not files already in target path
      //--------------------------------------------------------
      public static bool
       PathsMatch(
          string path1,
          string path2
       )
      {
         string p1 = path1.Trim().ToUpper();
         string p2 = path2.Trim().ToUpper();

         if (!p1.EndsWith(@"\")) p1 += @"\";
         if (!p2.EndsWith(@"\")) p2 += @"\";

         return (p1 == p2);
      }

      //--------------------------------------------------------
      // Verifies if current user is Sys admin
      //--------------------------------------------------------
      static public bool
        IsCurrentUserSysadmin(
           SqlConnection conn
        )
      {
         bool isSysadmin = false;

         try
         {
            string cmdStr = "SELECT IS_SRVROLEMEMBER ('sysadmin')";
            using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
            {
               int sysadmin;
               object obj = cmd.ExecuteScalar();
               if (obj is System.DBNull)
               {
                  sysadmin = 0;
               }
               else
               {
                  sysadmin = (int)obj;
               }

               if (sysadmin == 1)
               {
                  isSysadmin = true;
               }
            }
         }
         catch
         {
            // if it fails, you dont have right to do it so you are not a sysadmin
         }

         return isSysadmin;
      }
   }
}
