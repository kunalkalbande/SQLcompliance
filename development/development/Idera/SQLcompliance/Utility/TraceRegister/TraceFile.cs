using System;
using System.Collections;
using System.IO;
using System.Data.SqlClient;


namespace Idera.SQLcompliance.Utility.TraceRegister
{
	/// <summary>
	/// Summary description for TraceFile.
	/// </summary>
	public class TraceFile
	{
		private TraceFile() {}
		
      //-----------------------------------------------------------------------------
      // Process - process a trace file; return a result either good or bad
      //
      // (1) Get instance name
      //     (a) validate it exists in repository
      //     (b) check for matching BIN file
      // (2) Check for existing job - fail if yes
      // (3) Get privileged user flag
      // (4) Compress file - get checksum
      // (5) Copy file to collection server trace directory
      //-----------------------------------------------------------------------------
      public static bool
         Process(
            SqlConnection     conn,
            string            sourceDir,
            string            sourceFile,
            string            targetDir,
            out string        status
         )
      {
         string   m_sourceTrcPath = "";
         string   m_sourceBinPath = "";
         int      m_trzChecksum   = 0;
         string   m_targetTrcPath = "";
         string   m_targetBinPath = "";
         int      m_bizChecksum   = 0;
         
         status = "";
         
         bool retval = false;
         TraceFileNameAttributes tfna;
         FileInfo fi = null;

         // Parse file name (instance, priv user(level=3) )           
         try
         {
            fi = new FileInfo( sourceFile );
            tfna = TraceFileNameAttributes.GetNameAttributes( fi );
         }
         catch ( Exception ex )
         {
            status = String.Format( "ERROR: {0}",
                                    ex.Message );
            return false;
         }
         
         // Check for matching bin file
         m_sourceBinPath = MakeFullPath( sourceDir,
                                         tfna.InstanceAlias,
                                         ".bin" );
         
         if ( ! File.Exists( m_sourceBinPath ) )
         {
            status = String.Format( "ERROR: No matching bin file ({0}.bin)", 
                                    tfna.InstanceAlias );
            return false;
         }
         
         // Check for valid server
         string m_serverName = GetServerName( conn,
                                              tfna.InstanceAlias );
         if ( m_serverName == "" )
         {
            status = "ERROR: The SQL Server for this trace file is not registered in the Repository"; 
            return false;
         }
         
         // make sourceFile be a full path
         m_sourceTrcPath = MakeFullPath( sourceDir, sourceFile );
         
         // Check for existing job
         if ( JobExists( conn, m_sourceTrcPath ) )
         {
            status = "ERROR: Trace file already registered with Collection Server."; 
            return false;
         }
         
         // Compress trace file - get checksum
         m_targetTrcPath = Path.ChangeExtension( sourceFile, "trz" );
         m_targetTrcPath = MakeFullPath( targetDir, m_targetTrcPath );
         m_trzChecksum = 0;
         
         try
         {
            XceedRoutines.CompressToDisk( m_sourceTrcPath, m_targetTrcPath );
            m_trzChecksum = XceedRoutines.CalculateChecksum(m_targetTrcPath);
         }
         catch (Exception ex )
         {
            status = ex.Message;
            return false;
         }

         // copy bin file from instancealias.bin to tracefilename.bin and compress???
         m_targetBinPath = Path.ChangeExtension( sourceFile, "biz" );
         m_targetBinPath = MakeFullPath( targetDir, m_targetBinPath );

         try
         {
            XceedRoutines.CompressToDisk( m_sourceBinPath, m_targetBinPath );
            m_bizChecksum = XceedRoutines.CalculateChecksum(m_targetBinPath);
         }
         catch (Exception ex )
         {
            status = ex.Message;
            return false;
         }
         
         // Create job
         try
         {
            CreateJob( conn,
                       m_targetTrcPath,
                       m_trzChecksum,
                       m_targetBinPath,
                       m_bizChecksum,
                       m_serverName,
                       ( tfna.AuditLevel == 3) );
         }
         catch ( Exception ex )
         {
            status = ex.Message;
            return false;
         }
         
         // if we made it here; we are done
         // rename file to trc_processed to prevent duplicate registration
         string newFileName = Path.ChangeExtension( m_sourceTrcPath, string.Format("{0}_processed", fi.Extension) );
         File.Move( m_sourceTrcPath, newFileName );
         
         retval = true;

         return retval;
      }

        //-----------------------------------------------------------------------------
        // Reprocess - reprocess zipped files by registering them into jobs table and copyng them into CollectionServerTraceFiles    
        //-----------------------------------------------------------------------------
        public static bool  Reprocess(
            SqlConnection conn,
            string sourceDir,
            string sourceFile,
            string targetDir,
            out string status
         )
        {
            string m_sourceTrzPath = "";
            string m_sourceBizPath = "";
            int m_trzChecksum = 0;
            string m_targetTrzPath = "";
            string m_targetBizPath = "";
            int m_bizChecksum = 0;

            status = "";

            bool retval = false;
            TraceFileNameAttributes tfna;

            // Parse file name (instance, priv user(level=3) )          
            try
            {
                FileInfo fi = new FileInfo(sourceFile);
                tfna = TraceFileNameAttributes.GetNameAttributes(fi);
            }
            catch (Exception ex)
            {
                status = String.Format("ERROR: {0}",
                                        ex.Message);
                return false;
            }

            m_sourceBizPath = MakeFullPath(sourceDir, tfna.FileName, "." + tfna.BinZippedExtension);

            if (!File.Exists(m_sourceBizPath))
            {
                status = String.Format("ERROR: No matching file ({0}.{1})",
                                       tfna.InstanceAlias, tfna.BinZippedExtension);
               return false;
            }           

            // Check for valid server
            string m_serverName = GetServerName(conn,
                                                 tfna.InstanceAlias);
            if (m_serverName == "")
            {
                status = "ERROR: The SQL Server for this trace file is not registered in the Repository";
                return false;
            }

            // make sourceFile be a full path
            m_sourceTrzPath = MakeFullPath(sourceDir, sourceFile);

            // Check for existing job
            if (JobExists(conn, m_sourceTrzPath))
            {
                status = "ERROR: Trace file already registered with Collection Server.";
                return false;
            }

            // Prepare target paths for zipped trace and bin files
            m_targetTrzPath = Path.ChangeExtension(sourceFile, tfna.TrcZippedExtension);
            m_targetTrzPath = MakeFullPath(targetDir, m_targetTrzPath);           

            m_targetBizPath = Path.ChangeExtension(sourceFile, tfna.BinZippedExtension);
            m_targetBizPath = MakeFullPath(targetDir, m_targetBizPath);

            //Move trace and bin zipped files to target path (CollectionServerTraceFiles)
            try
            {
                File.Move(m_sourceTrzPath, m_targetTrzPath);
                m_trzChecksum = XceedRoutines.CalculateChecksum(m_targetTrzPath);
                File.Move(m_sourceBizPath, m_targetBizPath);
                m_bizChecksum = XceedRoutines.CalculateChecksum(m_targetBizPath);

            }
            catch (Exception ex)
            {
                status = ex.Message;
                return false;
            }
          
            // Create job for the processed files
            try
            {
                CreateJob(conn,
                           m_targetTrzPath,
                           m_trzChecksum,
                           m_targetBizPath,
                           m_bizChecksum,
                           m_serverName,
                           (tfna.AuditLevel == 3));
            }
            catch (Exception ex)
            {
                status = ex.Message;
                return false;
            }

            retval = true;

            return retval;
        }

        #region Handle server name lookup

        static private bool        m_listLoaded = false;
      static private ICollection m_serverList = null;

      //-----------------------------------------------------------------------------
      // GetServerName - walk server list to check for instance alias
      //-----------------------------------------------------------------------------
      private static string
         GetServerName(
            SqlConnection              conn,
            string                     instanceAlias
         )
      {
         string serverName = "";
         
         if ( ! m_listLoaded )
         {
            m_serverList = GetServers(conn);
            m_listLoaded = true;
         }
         
         if ( m_serverList != null && m_serverList.Count != 0)
         {
            foreach ( string nm in m_serverList )
            {
               if ( instanceAlias == NativeMethods.GetHashCode(nm).ToString())
               {
                  serverName = nm;
                  break;
               }
            }
         }
         
         return serverName;
      }

     
      //-----------------------------------------------------------------------------
      // GetServers
      //-----------------------------------------------------------------------------
      static public ICollection
         GetServers(
            SqlConnection     conn
         )
      {
         IList serverList     = null;
   		
         try
         {
            string cmdstr = "SELECT instance from SQLcompliance..Servers where isAuditedServer=1";
			                     
            using ( SqlCommand    cmd    = new SqlCommand( cmdstr, conn ) )
            {
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
                  serverList = new ArrayList();

                  while ( reader.Read() )
                  {
                     string serverName = reader.GetString(0);

                     serverList.Add( serverName );
                  }
               }
            }
         }
         catch
         {
            serverList = new ArrayList();  // return an empty array
         }
			
         return serverList;
      }
      
      #endregion
      
      #region Job Stuff

      //-----------------------------------------------------------------------------
      // JobExists
      //-----------------------------------------------------------------------------
      static private bool
         JobExists(
            SqlConnection     conn,
            string            traceFileName
         )
      {
         bool jobExists = false;
         
         string cmdstr = String.Format( "SELECT jobId from SQLcompliance..Jobs " +
                                        "WHERE compressedTraceFile={0}",
                                        SQLHelpers.CreateSafeString(traceFileName) );
                                        
         using ( SqlCommand    cmd    = new SqlCommand( cmdstr, conn ) )
         {
            using ( SqlDataReader reader = cmd.ExecuteReader() )
            {
               if ( reader.Read() )
               {
                  jobExists = true;
               }
            }
         }

         return jobExists;
      }
      
      //-----------------------------------------------------------------------------
      // CreateJob
      //-----------------------------------------------------------------------------
      static private void
         CreateJob(
            SqlConnection     conn,
            string            traceFile,
            int               traceChecksum,
            string            auditFile,
            int               auditChecksum,
            string            instance,
            bool              privUserTrace
         )
      {
         try
         {
            string sqlTmp = "INSERT INTO SQLcompliance..Jobs "+
               "(" +
               "instance" +
               ",state" +
               ",dateReceived" +
               ",compressedTraceFile" +
               ",traceChecksum" +
               ",compressedAuditFile" +
               ",auditChecksum" +
               ",privilegedUserTrace" +
               ",sqlSecureTrace" +
               ") VALUES ({0},0, GETUTCDATE(),{1},{2},{3},{4},{5},0);";
   			                     
            string cmdStr = String.Format( sqlTmp,
               SQLHelpers.CreateSafeString(instance),
               SQLHelpers.CreateSafeString(traceFile),
               traceChecksum,
               SQLHelpers.CreateSafeString(auditFile),
               auditChecksum,
               privUserTrace ? 1 : 0 );
   		                     
            using ( SqlCommand cmd = new SqlCommand( cmdStr, conn ) )
            {
               //cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
               cmd.ExecuteNonQuery();
            }
         }
         catch (Exception ex )
         {
            throw new Exception( String.Format( "Error creating job - {0}",
                                                ex.Message ) );
         }
      }
      
      #endregion
      
      #region Path Routines

      //-----------------------------------------------------------------------------
      // MakeFullPath
      //-----------------------------------------------------------------------------
      static private string
         MakeFullPath(
            string   dir,
            string   fileName
         )
      {
         return MakeFullPath( dir, fileName, "" );
      }
      
      static private string
         MakeFullPath(
            string   dir,
            string   fileName,
            string   extension
         )
      {
         return String.Format( "{0}{1}{2}{3}",
                               dir,
                               dir.EndsWith( @"\" ) ? "" : @"\",
                               fileName,
                               extension );
      }
      
      #endregion
	}
}
