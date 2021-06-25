using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

using Idera.SQLcompliance.Core;

namespace Idera.SQLcompliance.Core.TraceProcessing
{
	/// <summary>
	/// Summary description for TraceCleanup.
	/// </summary>
	internal class TraceCleanup
	{
		public TraceCleanup()
		{
		}
		
      //-----------------------------------------------------------------------		
      // KillAllTempTables
      //-----------------------------------------------------------------------		
		static public void
		   KillAllTempTables(
		   )
		{
		   Repository        readRep = new Repository();
		   Repository        writeRep = new Repository();
		   string            sqlText;
		   
		   try
		   {
		      readRep.OpenConnection(CoreConstants.RepositoryTempDatabase);
		      writeRep.OpenConnection(CoreConstants.RepositoryTempDatabase);
		      
		      sqlText = "SELECT * from sysobjects where xtype='U'";
		      using ( SqlCommand sqlCmd = new SqlCommand( sqlText, readRep.connection ) )
		      {
			      sqlCmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
		         using ( SqlDataReader reader = sqlCmd.ExecuteReader() )
		         {
                  if( reader == null )
                     return;

		            while ( reader.Read() )
		            {
		               string tableName= "";
      		         
		               try
		               {
		                  tableName = reader.GetString(0);
      		            
		                  // dont delete our working tables
		                  if ( tableName != CoreConstants.RepositoryTemp_StatesTable 
		                        && tableName != CoreConstants.RepositoryTemp_TimesTable
		                        && tableName != CoreConstants.RepositoryTemp_DupTable )
		                  {
		                     sqlText = String.Format( "DROP TABLE {0}", tableName );
		                     using ( SqlCommand sqlCmd2 = new SqlCommand( sqlText, writeRep.connection ) )
		                     {
               			      sqlCmd2.CommandTimeout = CoreConstants.sqlcommandTimeout;
		                        sqlCmd2.ExecuteNonQuery();
		                     }
		                  }
		               }
		               catch( Exception)
		               {
		                  // no big deal if things fail, we clean up tables before we process anyway
		                  // this is just trying to clean up
		               }
		            }
		         }
		      }
		   }
		   catch( Exception ex)
		   {
		      throw ex;
		   }
		   finally
		   {
		      readRep.CloseConnection();
		      writeRep.CloseConnection();
		   }
		}
		
      //-----------------------------------------------------------------------		
      // KillInstanceJobs - Kill all jobs associated with instance
      //-----------------------------------------------------------------------		
		static public void
		   KillInstanceJobs(
		      string            instanceName
		   )
		{
		   Repository rep = new Repository();
		   
		   try
		   {
		      rep.OpenConnection();
		      string sqlText = String.Format( "DELETE FROM {0} " +
                                            "WHERE instance={1}",
		                                      CoreConstants.RepositoryJobsTable,
		                                      SQLHelpers.CreateSafeString(instanceName) );
		      using ( SqlCommand cmd = new SqlCommand( sqlText, rep.connection ) )
		      {
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
		         cmd.ExecuteNonQuery();
		      }
		   }
		   catch( Exception ex )
		   {
		      throw ex;
		   }
		   finally
		   {
		      rep.CloseConnection();
		   }
		}
		
      //-----------------------------------------------------------------------		
      // KillFinishedJobs - If the server is rudely shut down at just the right time
      //                    you will have jobs that are marked as finished (state=5)
      //                    but there files still linger since they were never properly
      //                    cleaned up - this routine hunts them down and deletes them
      //-----------------------------------------------------------------------		
		static public void
		   KillFinishedJobs()
		{
         string sqlText = "";
		   Repository rep = new Repository();
		   
		   try
		   {
		      string compressedTrace = "";
		      string compressedAudit = "";
		      string uncompressedTrace = "";
		      string uncompressedAudit = "";
		      
		      rep.OpenConnection();

   		   // delete files
		      sqlText = String.Format( "SELECT compressedTraceFile,compressedAuditFile FROM {0} WHERE state=5",
		                              CoreConstants.RepositoryJobsTable );
		      using ( SqlCommand cmd = new SqlCommand( sqlText, rep.connection ) )
		      {
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      using ( SqlDataReader reader = cmd.ExecuteReader() )
			      {
			         while ( reader.Read() )
			         {
			            compressedTrace = SQLHelpers.GetString(reader,0);
						 int i = compressedTrace.IndexOf('*') ;
						 if(i != -1)
							 compressedTrace = compressedTrace.Substring(0, i) ;
			            compressedAudit = SQLHelpers.GetString(reader,1);

                        if (compressedTrace.Contains("XE"))
                        {
                            uncompressedTrace = Path.ChangeExtension(compressedTrace, ".xel");
                        }
                        else
                        {
                            uncompressedTrace = Path.ChangeExtension(compressedTrace, ".trc");
                        }
                     uncompressedAudit = Path.ChangeExtension( compressedAudit, ".bin");
                     
                     // delete files
                     try { File.Delete(compressedTrace);   } catch {}
                     try { File.Delete(compressedAudit);   } catch {}
                     try { File.Delete(uncompressedTrace); } catch {}
                     try { File.Delete(uncompressedAudit); } catch {}
			         }
			      }
		      }
		      
		      // delete jobs
		      sqlText = String.Format( "DELETE FROM {0} WHERE state=5", CoreConstants.RepositoryJobsTable );
		      using ( SqlCommand cmd = new SqlCommand( sqlText, rep.connection ) )
		      {
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
		         cmd.ExecuteNonQuery();
		      }
		   }
		   catch( Exception ex )
		   {
		      throw ex;
		   }
		   finally
		   {
		      rep.CloseConnection();
		   }
		}


		
      //-----------------------------------------------------------------------		
      // ResetTraceJobs - any job that is not complete (state=4) we reset to 
      //                  new (state=0) so it will be processed again
      //-----------------------------------------------------------------------		
		static public void
		   ResetTraceJobs()
		{
		   Repository rep = new Repository();
		   
		   try
		   {
		      rep.OpenConnection();
		      string sqlText = String.Format( "UPDATE {0} SET state=0, aborting = 0" +
                                            "FROM (SELECT * FROM {0} where state<4 or state = 6) AS j " +
                                            "WHERE {0}.jobId = j.jobId",
		                                      CoreConstants.RepositoryJobsTable );
		      using ( SqlCommand cmd = new SqlCommand( sqlText, rep.connection ) )
		      {
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
		         cmd.ExecuteNonQuery();
		      }
		      
		      // Remove all jobs from the queue since there was a problem with the SQL Server.
		      // The jobs will be re-queued in the TraceJobsPool.
		      TraceJobInfo.ResetJobQueue();
		   }
		   catch( Exception ex )
		   {
		      throw ex;
		   }
		   finally
		   {
		      rep.CloseConnection();
		   }
		}

      //-----------------------------------------------------------------------		
      // KillOrphanedTraceFiles - kills any compressed files not associated 
      //                          with a job in the table
      //-----------------------------------------------------------------------		
		static public void
		   KillOrphanedTraceFiles()
		{
		   // TODO: Kill orphaned trace files
		   
		   // for each file in directory
		   //    is file part of a job?
		   //    if not delete it
		}
		
      //-----------------------------------------------------------------------		
      // KillDecompressedFiles - Kills all .trc and .bin files in the trace
      //                         directory
      //-----------------------------------------------------------------------		
		static public void
		   KillDecompressedFiles(
		      string traceDir
		   )
		{
	      ErrorLog.Instance.Write( ErrorLog.Level.Debug, "TraceJobPool::KillDecompressedFiles - Enter" );
         string[] dirs = null;
         try
         {
            CoreHelpers.ValidateTraceDirectory(  traceDir );
            dirs = Directory.GetFiles( traceDir, "*.*" );
         }
         catch( Exception ex)
		   {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     CoreConstants.Debug_CantDeleteFile,
                                     ex,
                                     ErrorLog.Severity.Warning);
            throw;
		   }
            List<string> list = new List<string>();
           foreach (string fileName in dirs) 
         {
            string ext = Path.GetExtension(fileName);
                if (ext == ".bi7z" || ext == ".tr7z")
                {
                    list.Add(fileName);
                }
            
         }
            string[] compressedFileName = list.ToArray();
            foreach (string fileName in dirs)
            {
                string ext = Path.GetExtension(fileName);
               
                if (ext == ".xel" || ext == ".bin" || ext == ".trc")
                {
                    string fileNames = Path.GetFileNameWithoutExtension(fileName);
                    foreach (string file in compressedFileName)
                    {
                       
                        try
                        {
                            if (file.Contains(fileNames)) {
                                File.Delete(fileName);
                            }
                            ErrorLog.Instance.Write(ErrorLog.Level.Debug, String.Format("Deleted {0}", fileName));
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                     CoreConstants.Debug_CantDeleteFile,
                                                     ex,
                                                     ErrorLog.Severity.Warning);
                        }
                    }
                }
            }
                ErrorLog.Instance.Write( ErrorLog.Level.Debug, "TraceJobPool::KillDecompressedFiles - Leave" );
		}
	}
}
