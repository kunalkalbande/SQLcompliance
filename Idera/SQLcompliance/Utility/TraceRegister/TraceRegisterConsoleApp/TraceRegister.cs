using System;
using System.Collections;
using System.IO;
using System.Data.SqlClient;


namespace Idera.SQLcompliance.Utility.TraceRegisterConsoleApp
{

   class TraceRegister
   {
      public SqlConnection sql_connection = null;
      public int m_goodFiles = 0;
      public int m_badFiles = 0;
      public int m_reprocessedFiles = 0;

      private String register_trace_directory = String.Empty;
      private String server_instance = String.Empty;
      private String collection_server_directory = String.Empty;

      public TraceRegister(String[] args)
      {
         if (args.Length > 0)
         {
            register_trace_directory = args[0];
         }
         else
         {
            Console.Write("Register trace directory is missing");
            System.Threading.Thread.Sleep(3000);
            Environment.Exit(0);
         }

         server_instance = RegisterUtils.getDefaultSQLServerInstance();
         collection_server_directory = RegisterUtils.GetCollectionServerTraceDirectory();
         Xceed.Compression.Formats.Licenser.LicenseKey = "SCN10DKKPZTUTWAU4AA";
      }

      //-----------------------------------------------------------------------------
      // OpenConnection
      //-----------------------------------------------------------------------------
      private void
         OpenConnection(
            string serverName
         )
      {
         string strConn = String.Format("server={0};" +
            "integrated security=SSPI;" +
            "Connect Timeout=30;" +
            "Application Name='SQLcompliance TraceRegister';",
            serverName);

         sql_connection = new SqlConnection(strConn);
         sql_connection.Open();
      }

      //-----------------------------------------------------------------------------
      // CloseConnection
      //-----------------------------------------------------------------------------
      public void
         CloseConnection()
      {
         if (sql_connection != null)
         {
            sql_connection.Close();
            sql_connection.Dispose();
            sql_connection = null;
         }
      }
      //--------------------------------------------------------
      // Register Trace, XE files and reprocess orphaned files moved from CollectionServerTraceFiles directory
      //--------------------------------------------------------
      public void Register()
      {
         try
         {
            if (!Directory.Exists(register_trace_directory))
            {
               Console.Write("Error: The specified trace directory does not exist.");
               return;
            }

            // Make sure they are not importing files already in collection server trace directory
            if (RegisterUtils.PathsMatch(register_trace_directory, collection_server_directory))
            {
               Console.Write("Error: You cannot register trace files already in the Collection Server trace directory.");
               return;
            }

            // check sql server connection (by opening connection)
            try
            {
               OpenConnection(server_instance);
            }
            catch (Exception ex)
            {
               Console.Write(String.Format("Error: Cant connect to SQL Server.\n\nError: {0}",
               ex.Message));
               return;
            }

            try
            {
               // see if SQlcompliance database exists
               string cmdstr = "select name from master..sysdatabases where name='SQLcompliance'";
               using (SqlCommand cmd = new SqlCommand(cmdstr, sql_connection))
               {
                  using (SqlDataReader reader = cmd.ExecuteReader())
                  {
                     if (!reader.Read())
                     {
                        throw new Exception("xxx");
                     }
                  }
               }
            }
            catch
            {
               Console.Write("Error: SQL Compliance Manager Repository database not found on the specified SQL Server.");
               return;
            }

            // validate that user has rights to write to repository
            if (!RegisterUtils.IsCurrentUserSysadmin(sql_connection))
            {
               Console.Write("Error: You do not have enough rights within SQL Compliance Manager to register trace files.");
               return;
            }

            //------------
            // Processing
            //------------
            m_goodFiles = 0;
            m_badFiles = 0;
            m_reprocessedFiles = 0;

            ProcessTraceFiles(register_trace_directory,
                               collection_server_directory);

            Console.Write(String.Format("Trace File Registration Complete\n" +
                            "================================\n" +
                            "Files registered:  {0}\n" +
                            "Files reprocessed: {1}\n" +
                            "Files with errors: {2}\n\n" +
                            "Note: All successfully registered files have been " +
                            "renamed to '<name>.extension_processed' to prevent duplicate " +
                            "registration. The reprocessed files have been moved to CollectionServerTrace directory.",
                            m_goodFiles,
                            m_reprocessedFiles,
                            m_badFiles));
         }
         finally
         {
            CloseConnection();
         }
      }


      //--------------------------------------------------------
      // ProcessTraceFiles
      //--------------------------------------------------------
      private void
         ProcessTraceFiles(
            string sourceDir,
            string targetDir
         )
      {
         try
         {
            DirectoryInfo traceDirectoryInfo = new DirectoryInfo(sourceDir);
            FileInfo[] fileInfoListTRC = traceDirectoryInfo.GetFiles("*.trc");
            FileInfo[] fileInfoListXEL = traceDirectoryInfo.GetFiles("*.xel");

            ArrayList fileInfoList = new ArrayList();
            fileInfoList.AddRange(fileInfoListTRC);
            fileInfoList.AddRange(fileInfoListXEL);

            if (fileInfoList == null)
            {
               Console.Write("**** ERROR: No trace files found in the specified source trace file directory.");
               return;
            }

            ArrayList tmpFileList = new ArrayList(fileInfoList);
            tmpFileList.Sort(new FileInfoComparer());


            foreach (FileInfo file in tmpFileList)
            {
               string status;

               // check for .TRC extension since GetFiles works funny for 3 character extensions
               string ext = file.Extension.ToUpper();
               if (ext == ".TRC" || ext == ".XEL")
               {
                  if (TraceFile.Process(sql_connection,
                                          sourceDir,
                                          file.Name,
                                          collection_server_directory,
                                          out status))
                  {
                     m_goodFiles++;
                     Console.Write(String.Format("Trace File: {0} - Registered", file.Name));
                  }
                  else
                  {
                     m_badFiles++;
                     Console.Write(String.Format("Trace File: {0} - Failed\n\n***** ERROR: {1}",
                                                         file.Name,
                                                         status));
                  }
               }
            }

            //TODO Call this method from other process
            //Call reprocess method in order to move orphaned files and register them into jobs table
            this.ReprocessFiles(sourceDir, targetDir);

            if (m_goodFiles == 0 && m_badFiles == 0 && m_reprocessedFiles == 0)
            {
               Console.Write("**** ERROR: No files to process found in the specified file directory.");
            }
         }
         catch (Exception ex)
         {
            Console.Write(String.Format("***** ERROR PROCESSING FILES: {0}",
                                                  ex.Message));
         }
      }

      //--------------------------------------------------------
      // Reprocess orphaned files 
      //--------------------------------------------------------
      private void
         ReprocessFiles(
            string sourceDir,
            string targetDir
         )
      {
         try
         {

            DirectoryInfo traceDirectoryInfo = new DirectoryInfo(sourceDir);

            FileInfo[] fileInfoListTRZ = traceDirectoryInfo.GetFiles("*.trz");
            FileInfo[] fileInfoListTR7Z = traceDirectoryInfo.GetFiles("*.tr7z");

            ArrayList fileInfoList = new ArrayList();
            fileInfoList.AddRange(fileInfoListTRZ);
            fileInfoList.AddRange(fileInfoListTR7Z);

            if (fileInfoList.Count == 0)
            {

               return;
            }

            ArrayList tmpFileList = new ArrayList(fileInfoList);
            tmpFileList.Sort(new FileInfoComparer());


            foreach (FileInfo file in tmpFileList)
            {

               string status;

               // check for files with .TRZ or TR7Z extensions
               string ext = file.Extension.ToUpper();
               if (ext == ".TRZ" || ext == ".TR7Z")
               {
                  if (TraceFile.Reprocess(sql_connection,
                                          sourceDir,
                                          file.Name,
                                          collection_server_directory,
                                          out status))
                  {
                     m_reprocessedFiles++;
                     Console.Write(String.Format("File: {0} - Registered",
                                                         file.Name));
                  }
                  else
                  {
                     m_badFiles++;
                     Console.Write(String.Format("File: {0} - Failed\n\n***** ERROR: {1}",
                                                         file.Name,
                                                         status));
                  }

               }
            }
         }
         catch (Exception ex)
         {
            Console.Write(String.Format("***** ERROR REPROCESSING FILES: {0}",
                                                  ex.Message));
         }
      }
   }
}
