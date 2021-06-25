using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections;
using System.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Idera.SQLcompliance.Core.Licensing;
using Idera.SQLcompliance.Core.Remoting;
using Idera.SQLcompliance.Core.Templates.AuditSettings;
using Idera.SQLcompliance.Core.Service;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Status;
using Idera.SQLcompliance.Core.Templates;
using Idera.SQLcompliance.Core.Templates.AuditTemplates;
using Idera.SQLcompliance.Core.Templates.IOHandlers;
using Idera.SQLcompliance.Core.Scripting;
using Idera.SQLcompliance.Core.TraceProcessing;


namespace Idera.SQLcompliance.Core.Collector
{
   [Serializable]
   public class RegisterServerArgs : ISerializable
   {
      public string Instance;
      public string Server;
      public string ConfigFile;
      public string User;
      public bool DeleteExisting;

      public RegisterServerArgs()
      {
         Instance = string.Empty;
         ConfigFile = string.Empty;
         User = String.Empty;
         DeleteExisting = false;
      }

      #region Serialization Code
      public RegisterServerArgs(SerializationInfo info, StreamingContext context)
      {
         try
         {
            Instance = info.GetString("Instance");
            Server = info.GetString("Server");
            ConfigFile = info.GetString("ConfigFile");
            User = info.GetString("User");
            DeleteExisting = info.GetBoolean("DeleteExisting");
         }
         catch (Exception e)
         {
            SerializationHelper.ThrowDeserializationException(e, GetType());
         }
      }

      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         try
         {
            info.AddValue("Instance", Instance);
            info.AddValue("Server", Server);
            info.AddValue("ConfigFile", ConfigFile);
            info.AddValue("User", User);
            info.AddValue("DeleteExisting", DeleteExisting);
         }
         catch( Exception e )
         {
            SerializationHelper.ThrowSerializationException( e, GetType());
         }
      }
      #endregion
   }

   [Serializable]
   public class RegisterDatabaseArgs : ISerializable
   {
      public string Instance;
      public string Database;
      public string ConfigFile;
      public string Regulation;
      public string User;
      public int SqlDBId;

      public RegisterDatabaseArgs()
      {
         Instance = string.Empty;
         Database = string.Empty;
         ConfigFile = string.Empty;
         Regulation = string.Empty;
         User = string.Empty;
         SqlDBId = -2;
      }

      #region Serialization Code
      public RegisterDatabaseArgs(SerializationInfo info, StreamingContext context)
      {
         try
         {
            Instance = info.GetString("Instance");
            Database = info.GetString("Database");
            ConfigFile = info.GetString("ConfigFile");
            Regulation = info.GetString("Regulation");
            User = info.GetString("User");
            SqlDBId = info.GetInt32("SqlDBId");
         }
         catch (Exception e)
         {
            SerializationHelper.ThrowDeserializationException(e, GetType());
         }
      }

      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         try
         {
            info.AddValue("Instance", Instance);
            info.AddValue("Database", Database);
            info.AddValue("ConfigFile", ConfigFile);
            info.AddValue("Regulation", Regulation);
            info.AddValue("User", User);
            info.AddValue("SqlDBId", SqlDBId);
         }
         catch (Exception e)
         {
            SerializationHelper.ThrowSerializationException(e, GetType());
         }
      }
      #endregion
   }
   
   /// <summary>
	/// Summary description for RemoteCollector.
	/// </summary>
	public class RemoteCollector : MarshalByRefObject
	{

      #region Delegates
	   
	   public delegate void ReportStatus( string stage, int percentDone );

      #endregion

      private bool _alreadyDeployed = false;
      private bool _repositoryComputer = false;
      private bool _convertingNonAudited = false;
      private ServerRecord _existingAuditedServer = null;
      private ArrayList _databaseList = new ArrayList();

      #region Public methods

      #region Server Management

      public void UpdateIndexes(string databaseName)
      {
         Repository rep = new Repository() ;
         rep.OpenConnection() ;

         try
         {
            SystemDatabaseRecord record = SystemDatabaseRecord.Read(rep.connection, databaseName) ;
            if(record == null)
               throw new Exception(String.Format("UpdateIndexes:  No SystemDatabase named {0}", databaseName)) ;

            if(String.Equals(record.DatabaseType, "System"))
            {
               // Only SQLcompliance database
               if(!String.Equals(databaseName, CoreConstants.RepositoryDatabase))
                  return ;
               Repository.BuildIndexes(rep.connection) ;
            }
            else
            {
               int version = EventDatabase.GetDatabaseSchemaVersion(rep.connection, databaseName) ;

               // The schema version must be at least 50X
               if ((version / 100) < 5)
                  throw new Exception(String.Format("UpdateIndexes:  Unable to upgrade indexes for {0}.  Schema version is incorrect:  {1}", databaseName, version));

               EventDatabase.UpdateIndexes(rep.connection, databaseName) ;
            }
         }
         finally
         {
            rep.CloseConnection() ;
         }
      }

      public void UpdateAllIndexes()
      {
         String errorString = "" ;
         Repository rep = new Repository() ;
         rep.OpenConnection() ;

         try
         {
            SystemDatabaseRecord[] records = SystemDatabaseRecord.Read(rep.connection) ;
            foreach(SystemDatabaseRecord record in records)
            {
               try
               {
                  if(String.Equals(record.DatabaseType, "System"))
                  {
                     // Only SQLcompliance database
                     if(!String.Equals(record.DatabaseName, CoreConstants.RepositoryDatabase))
                        continue ;
                     Repository.BuildIndexes(rep.connection) ;
                  }
                  else
                  {
                     int version = EventDatabase.GetDatabaseSchemaVersion(rep.connection, record.DatabaseName) ;

                     // The schema version must be at least 50X
                     if ((version / 100) < 5)
                        errorString += String.Format("UpdateIndexes:  Unable to upgrade indexes for {0}.  Schema version is incorrect:  {1}", record.DatabaseName, version);
                     EventDatabase.UpdateIndexes(rep.connection, record.DatabaseName) ;
                  }
               }
               catch(Exception e)
               {
                  errorString += String.Format("Exception:  {0} {1}\n", e.Message, e.StackTrace) ;
               }
            }
         }
         finally
         {
            rep.CloseConnection() ;
            if(errorString.Length > 0)
               throw new Exception(errorString) ;
         }
      }

	   
      public string GetRepositoryInstance()
      {
         return CollectionServer.ServerInstance ;
      }

      public string GetServerSettings()
      {
         return CollectionServer.Instance.GetServerSettings() ;
      }


      //------------------------------------------------------------		
      // GetStatus
      //
      // Called from GUI to get server variables
      //------------------------------------------------------------		
      public bool
         GetStatus(
            out string outTraceDirectory,
            out int    outLogLevel,
            out int    outActivityLogLevel
         )
      {
         outTraceDirectory   = CollectionServer.traceDirectory;
         outLogLevel         = (int)ErrorLog.Instance.ErrorLevel;
         outActivityLogLevel = CollectionServer.activityLogLevel;
         
         return true;
      }

      //------------------------------------------------------------		
      // PingCollectionServer
      //
      // Tests whether collection service is available and ready to receive actions.
      // Callers should wrap this call in a try/catch block; an exception will be 
      // thrown if the remotable object cannot be reached.
      //
      // Retval: Always returns true; if an exception is thrown,
      //         the collection service is not available</returns>
      //------------------------------------------------------------		
      public bool PingCollectionServer()
      {
         // throw exception if server cant receive requests right now (usually due to shutting down)
         CollectionServer.Instance.CheckServerStatus();         
         
         // Just testing
         return true;
      }
		
      public bool AllowCaptureSql()
      {
         return CoreConstants.AllowCaptureSql ;
      }
		
      #endregion
	   
      #region Archive
      //------------------------------------------------------------		
      // ArchiveNow
      //
      // Request to do an archive now instead of waiting for next
      // AutoArchive. User can request one instance or all
      //
      //    instanceName="" indicates do All
      //
      // This request kicks off background thread to do archive
      //
      // Only one archive request allowed at any given time
      //------------------------------------------------------------
      public CMCommandResult Archive(ArchiveSettings settings)
      {
         Thread archiveThread = new Thread(new ThreadStart(settings.StartArchive)) ;
         archiveThread.Name = "ArchiveJob" ;

         // throw exception if server cant receive requests right now (usually due to shutting down)
         CollectionServer.Instance.CheckServerStatus();         

         archiveThread.Start() ;
         if(!settings.Background)
            archiveThread.Join() ;
         return settings.ArchiveResults ;
      }

      //------------------------------------------------------------		
      // UpdateArchiveList
      //------------------------------------------------------------
      public void
         UpdateArchiveList()
      {
         ArchiveJob.UpdateArchiveList();
      }
      
      #endregion
	   
      #region Groom
	   
      //------------------------------------------------------------		
      // Groom
      //
      // Request grooming of one server instance
      //------------------------------------------------------------		
      public CMCommandResult Groom(string instanceName, int groomAgeInDays, IntegrityCheckAction icAction)
      {
         // throw exception if server cant receive requests right now (usually due to shutting down)
         CollectionServer.Instance.CheckServerStatus();         

         Grooming g = new Grooming();
         return g.Groom(instanceName, groomAgeInDays, icAction);
      }
	   
      //------------------------------------------------------------		
      // Groom
      //
      // Request grooming of one server instance
      //------------------------------------------------------------		
      public CMCommandResult Groom(string instanceName, int groomAgeInDays, IntegrityCheckAction icAction, int batchSize )
      {
         // throw exception if server cant receive requests right now (usually due to shutting down)
         CollectionServer.Instance.CheckServerStatus();         

         Grooming g = new Grooming();
         return g.Groom(instanceName, groomAgeInDays, icAction, batchSize);
      }
	  
      //------------------------------------------------------------		
      // GroomAll
      //
      // Request grooming of all server instances
      //------------------------------------------------------------		
      public CMCommandResult GroomAll(int groomAgeInDays, IntegrityCheckAction icAction)
      {
         // throw exception if server cant receive requests right now (usually due to shutting down)
         CollectionServer.Instance.CheckServerStatus();         

         Grooming g = new Grooming();
         return g.GroomAll(groomAgeInDays, icAction);
      }
	   
      //------------------------------------------------------------		
      // GroomAll
      //
      // Request grooming of all server instances
      //------------------------------------------------------------		
      public CMCommandResult GroomAll(int groomAgeInDays, IntegrityCheckAction icAction, int batchSize )
      {
         // throw exception if server cant receive requests right now (usually due to shutting down)
         CollectionServer.Instance.CheckServerStatus();         

         Grooming g = new Grooming();
         return g.GroomAll(groomAgeInDays, icAction, batchSize);
      }
	   
      #endregion
	   
      #region Integrity Check
      
      //------------------------------------------------------------		
      // IntegrityCheck
      //------------------------------------------------------------		
      public bool 
         CheckIntegrity(
            bool  fixProblems
         )
      {
         // throw exception if server cant receive requests right now (usually due to shutting down)
         CollectionServer.Instance.CheckServerStatus();         
         
         IntegrityChecker checker = new IntegrityChecker();
         
         return checker.CheckIntegrity( fixProblems );
      }

      public CheckResult 
         CheckIntegrity(
            string instance,
            bool   fixProblems
         )
      {
         // throw exception if server cant receive requests right now (usually due to shutting down)
         CollectionServer.Instance.CheckServerStatus();         
         
         IntegrityChecker checker = new IntegrityChecker();
         
         return checker.CheckIntegrity( instance, fixProblems );
      }

      public CheckResult 
         CheckIntegrity(
            string               instance,
            string               inDatabase,
            bool                 inIsArchive,
            bool                 fixProblems,
            out EventRecord[]    badRecords,
            out int[]            badRecordTypes
         )
      {
         // throw exception if server cant receive requests right now (usually due to shutting down)
         CollectionServer.Instance.CheckServerStatus();         

         IntegrityChecker checker = new IntegrityChecker();
         
         return checker.CheckIntegrity( instance,
                                        inDatabase,
                                        inIsArchive,
                                        fixProblems,
                                        out badRecords,
                                        out badRecordTypes);
      }
      
      #endregion
	   
      #region Trace Collection and Management
      //------------------------------------------------------------------------
      // SendFile
      //
      // Handle status event from agent
      //
      //  retval - Does agent need to update its audit settings
      //------------------------------------------------------------------------
      /// <summary>
      /// 
      /// </summary>
      /// <param name="file"></param>
      public void
         SendFile(
            RemoteFile file
         )
      {
         bool filesAndJobPersisted = false;
         string localTraceFileName = null;
         string localAuditFileName = null;

         // throw exception if server cant receive requests right now (usually due to shutting down)
         CollectionServer.Instance.CheckServerStatus();         
         
         // make sure instance registered with Collector Service - it not an exception is thrown back to agent
         CheckForValidInstance( file.Instance );
         
         ServerRecord.UpdateLastAgentContact( file.Instance );
         
         ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                  String.Format( "SendFile (Start) - Instance: {0} File: {1}", file.Instance, file.TraceFilename ) );

         
         try
         {
            // Skip empty files
            if( file.Content.Length != 0 )
            {
               // if we have file exists and trace job exist,
               //    we have seen these before; return success to agent and do nothing
               // if we have files but no trace job
               //    delete files and start from scratch
               bool fileExists = File.Exists( Path.Combine( CollectionServer.traceDirectory, file.TraceFilename ) )
                              || File.Exists( Path.Combine( CollectionServer.traceDirectory, file.AuditFileName ) );
                              
               // does trace job for this file exist?
               int traceJobState = GetTraceJobState( file.Instance,
                                                     Path.Combine( CollectionServer.traceDirectory, file.TraceFilename ) );
               if ( fileExists )
               {
                  if (    traceJobState != -1   // no job
                       && traceJobState !=  4 ) // failed job
                  {
                     // agent is trying to send twice but we already have everything we need - return as 
                     // success and let agent clean itself up better this time
                     return;
                  }
                  else
                  {
                     string compressed;
                     string uncompressed;
                     
                     // (1) Files but no traceJob - must be aborted SendFile - clean up and start from scratch
                     // (2) We have files and a job that failed in processing (state=4) so lets retry with new files
                     compressed   = Path.Combine( CollectionServer.traceDirectory, file.TraceFilename );
                     if (file.TraceFilename.StartsWith("XE"))
                     {
                         uncompressed = Path.ChangeExtension(compressed, ".xel");
                     }
                     else if (file.TraceFilename.StartsWith("AL"))
                     {
                         uncompressed = Path.ChangeExtension(compressed, ".sqlaudit");
                     }
                     else
                     {
                         uncompressed = Path.ChangeExtension(compressed, ".trc");
                     }
                     try { File.Delete( compressed ); } catch {}
                     try { File.Delete( uncompressed ); } catch {}
                     
                     compressed   = Path.Combine( CollectionServer.traceDirectory, file.AuditFileName );
                     uncompressed = Path.ChangeExtension( compressed, ".bin" );
                     try { File.Delete( compressed ); } catch {}
                     try { File.Delete( uncompressed ); } catch {}
                     
                     DeleteTraceJob( file.Instance, Path.Combine( CollectionServer.traceDirectory, file.TraceFilename ) );
                  }
               }
               else
               {
                  if ( traceJobState != -1 )
                  {
                     // no files but we have a job - we are never in this case unless bad things have happened
                     // now we have a chance to recover by redoing job - kill existing job and retry
                     // just wipe out job and recreate
                     DeleteTraceJob( file.Instance, Path.Combine( CollectionServer.traceDirectory, file.TraceFilename ) );
                  }
               }

               // Serialize the trace and audit files
               localTraceFileName = TraceFile.PersistToDisk( CollectionServer.traceDirectory, 
                                                             file.TraceFilename, 
                                                             file.Content );
               localAuditFileName = TraceFile.PersistToDisk( CollectionServer.traceDirectory, 
                                                             file.AuditFileName, 
                                                             file.AuditContent );

               // Call trace server to start new trace processing job.
               TraceServer.Instance.GetAgentTrace( file.Instance, 
                                                   localTraceFileName, 
                                                   localAuditFileName, 
                                                   file.IsPrivilegedUserTrace, 
                                                   file.IsSqlSecureTrace );
               filesAndJobPersisted = true;
               
               ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                        String.Format( "SendFile (Complete) - Instance: {0} File: {1}", file.Instance, localTraceFileName ) );
               
            }
         }
         catch( IOException ie )
         {
               // some other I/O problem.  Log this and throw the exception back to the agent
               ErrorLog.Instance.Write( ErrorLog.Level.Default, ie,  true);	
               throw ie;

         }
         catch (Exception e)
         {	
            // Need to log this on the collection service side
            ErrorLog.Instance.Write( ErrorLog.Level.Default, 
                                     e.Message,
                                     e, 
                                     ErrorLog.Severity.Warning, 
                                     true);	

            // Throw it back to the agent.
            throw e;
         }
         finally
         {
            // Error occurred.  Rollback.
            if( ! filesAndJobPersisted )
            {
               if( localTraceFileName != null )
               {
                  ErrorLog.Instance.Write( ErrorLog.Level.Debug, 
                     "Error occurred persisting files.  Deleting " + localTraceFileName );	
                  try { File.Delete( localTraceFileName ); } catch {}
                  
                  // in case we create a job but things went wrong - kill i tnow
                  DeleteTraceJob( file.Instance, localTraceFileName );
               }

               if( localAuditFileName != null )
               {
                  ErrorLog.Instance.Write( ErrorLog.Level.Debug, 
                     "Error occurred persisting files.  Deleting " + localAuditFileName );	
                  try { File.Delete( localAuditFileName ); } catch {}
               }
            }
         }
      }			

      //------------------------------------------------------------------------------------
      // CheckFileStatus : check trace and bin file status and check if there is already
      // a trace job created.
      //------------------------------------------------------------------------------------
      internal static void	CheckFileStatus(
         string instance,
         string filename,
         string binFilename )
      {
         ErrorLog.Instance.Write(ErrorLog.Level.Debug,
            String.Format("Remote collector: checking trace file {0} status...", filename));
         // if we have file exists and trace job exist,
         //    we have seen these before; return success to agent and do nothing
         // if we have files but no trace job
         //    delete files and start from scratch
         bool fileExists = File.Exists(Path.Combine(CollectionServer.traceDirectory, filename))
                        || File.Exists(Path.Combine(CollectionServer.traceDirectory, binFilename ));

         // does trace job for this file exist?
         int traceJobState = GetTraceJobState( instance,
                                               Path.Combine(CollectionServer.traceDirectory, filename));
         if (fileExists)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
               String.Format("Dupliacte trace file {0} found.  Checking trace job state...", filename));
         
            if (traceJobState != -1   // no job
                 && traceJobState != (int)TraceJobInfo.State.UnrecoverableError) // job encountered an unrecoverable error
            {
               // agent is trying to send twice but we already have everything we need - return as 
               // success and let agent clean itself up better this time
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                  String.Format( "Dupliacte trace file {0} and a job is already created in the job queue.", filename ) );
               return;
            }
            else
            {
               string compressed;
               string uncompressed;
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                  String.Format("No valid trace job associated with trace file {0}.  Removing existing trace file...", filename));

               // (1) Files but no traceJob - must be aborted SendFile - clean up and start from scratch
               // (2) We have files and a job that failed in processing (state=4) so lets retry with new files
               compressed = Path.Combine(CollectionServer.traceDirectory, filename);
               if (filename.StartsWith("XE"))
               {
                   uncompressed = Path.ChangeExtension(compressed, ".xel");
               }
               else if (filename.StartsWith("AL"))
               {
                   uncompressed = Path.ChangeExtension(compressed, ".sqlaudit");
               }
               else
               {
                   uncompressed = Path.ChangeExtension(compressed, ".trc");
               }
               DeleteFile( compressed );
               DeleteFile( uncompressed ); 

               compressed = Path.Combine(CollectionServer.traceDirectory, binFilename);
               uncompressed = Path.ChangeExtension(compressed, ".bin");
               DeleteFile(compressed);
               DeleteFile(uncompressed); 

               DeleteTraceJob( instance, Path.Combine(CollectionServer.traceDirectory, filename));
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                  String.Format("Trace file clean up finished.  Resuming trace file transfer.", filename));
            }
         }
         else
         {
            if (traceJobState != -1)
            {
               // no files but we have a job - we are never in this case unless bad things have happened
               // now we have a chance to recover by redoing job - kill existing job and retry
               // just wipe out job and recreate
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                  String.Format("Invalid trace job found for trace file {0}.  Deleting up trace job.", filename));
               DeleteTraceJob(instance, Path.Combine(CollectionServer.traceDirectory, filename));
            }
         }
      }

      internal static int
         GetTraceJobState(
            string   instance,
            string   traceFile )
      {
         int state = -1;
         Repository rep = new Repository();
         
         try
         {
            rep.OpenConnection();
            
            string sql = String.Format( "SELECT top 1 state from {0}..{1} WHERE instance={2} and compressedTraceFile LIKE {3}",
                                        CoreConstants.RepositoryDatabase,
                                        CoreConstants.RepositoryJobsTable,
                                        SQLHelpers.CreateSafeString(instance),
                                        SQLHelpers.CreateSafeString(traceFile + "%") );
            
            using ( SqlCommand cmd = new SqlCommand( sql, rep.connection ) )
            {
				   object obj = cmd.ExecuteScalar();
				   if( (obj is DBNull) || (obj == null))
					   state = -1;
				   else
					   state = (int)obj;
            }
         }
         catch {}
         finally
         {
            rep.CloseConnection();
         }
         
         return state;
      }
      
      internal static void
         DeleteTraceJob(
            string   instance,
            string   traceFile )
      {
         Repository rep = new Repository();
         
         try
         {
            rep.OpenConnection();
            
            string sql = String.Format( "DELETE FROM {0}..{1} WHERE instance={2} and compressedTraceFile LIKE {3}",
                                        CoreConstants.RepositoryDatabase,
                                        CoreConstants.RepositoryJobsTable,
                                        SQLHelpers.CreateSafeString(instance),
                                        SQLHelpers.CreateSafeString(traceFile + "%") );
            
            using ( SqlCommand cmd = new SqlCommand( sql, rep.connection ) )
            {
				   cmd.ExecuteNonQuery();
            }
         }
         catch
         {}
         finally
         {
            rep.CloseConnection();
         }
      }
      
      #endregion

      #region RegisterInstance

      public CMCommandResult RegisterInstance(RegisterServerArgs args)
      {
         CMCommandResult retVal = new CMCommandResult();
         SQLcomplianceConfiguration cmConfig = new SQLcomplianceConfiguration();
         Repository rep = null;
         ServerRecord server = new ServerRecord();
         StringBuilder builder = new StringBuilder();
         string error = String.Empty;

         try
         {
            rep = new Repository();
            rep.OpenConnection();
            cmConfig.Read(rep.connection);

            if (!LicenseHelper.LicenseAllowsMoreInstances(rep.connection, cmConfig))
            {
               builder.AppendFormat("Registration failed for {0}", args.Instance);
               builder.Append("\n  You have already reached the maximum number of registered SQL Servers allowed by the current license. Please contact IDERA to purchase additional licenses.");
               retVal.AddResultString(builder.ToString());
               retVal.ResultCode = ResultCode.Error;
               return retVal;
            }

            //see if we are going to audit an instance on the same machine as the repository.
            _repositoryComputer = args.Server == GetInstanceHost(cmConfig.Server.ToUpper());

            if (CreateServer(args, rep.connection, ref server, ref error))
            {
               if (_alreadyDeployed)
               {
                  if (!Activate(args.Instance, cmConfig))
                  {
                     // Activate first - if cant reach an existing agent then we cant 
                     // do anything - mark as not deployed/running.  Activate requires the
                     //  entry exist in the db to work.
                     server.IsRunning = false;
                     server.IsDeployed = false;
                  }
                  else
                  {
                     server.IsRunning = true;
                     server.IsDeployed = true;
                  }
                  ServerRecord.SetIsFlags(server.Instance,
                                          server.IsDeployed,
                                          server.IsDeployedManually,
                                          server.IsRunning,
                                          server.IsCrippled,
                                          rep.connection);
               }

               // Force a heartbeat for status
               PingAgent(server);
               System.Threading.Thread.Sleep(2000); //sleep for two seconds to wait for the ping to complete.  No, this isn't the best solution.
            }
            else
            {
               if (String.IsNullOrEmpty(error))
                  retVal.AddResultString(String.Format("Unable to add the instance {0} for auditing because the instance is already being audited or The Repository was not able to be updated.", args.Instance));
               else
                  retVal.AddResultString(String.Format("Unable to add the instance {0} for auditing.  {1}", args.Instance, error));
               retVal.ResultCode = ResultCode.Error;
            }
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write("An error occurred reading configuration record to perform license check.", ex);
         }
         finally
         {
            if(rep != null)
               rep.CloseConnection();
         }
         return retVal;
      }

      private string GetInstanceHost(string instance)
      {
         string host = "";

         int pos = instance.IndexOf(@"\");
         if (pos > 0)
         {
            host = instance.Substring(0, pos);
         }
         else
         {
            host = instance;
         }

         // expand local reference
         if (host == "" || host == ".")
         {
            host = System.Net.Dns.GetHostName().ToUpper();
         }
         return host;
      }

      private void PingAgent(ServerRecord server)
      {
         try
         {
            // ping agent
            AgentCommand agentCmd = CoreRemoteObjectsProvider.AgentCommand(server.AgentServer, server.AgentPort);
            agentCmd.Ping();
         }
         catch (Exception)
         {
         }
      }

      private bool CreateServer(RegisterServerArgs args, SqlConnection conn, ref ServerRecord server, ref string error)
      {
         bool retval = true;
         bool compatiableSchema = false;

         // Create events database
         string eventsDatabase;

         try
         {
            //this will only return false if the server is already being audited
            server = CreateServerRecord(args, conn, ref error);

            if (server == null)
               return false;

            eventsDatabase = EventDatabase.GetDatabaseName(server.Instance);

            if (!DoesDatabaseExist(args.Instance, conn, out compatiableSchema))
            {
               // database doesnt already exist
               EventDatabase.Create(server.Instance,
                                    eventsDatabase,
                                    server.DefaultAccess,
                                    conn);
            }
            else
            {
               if (args.DeleteExisting)
               {
                  EventDatabase.InitializeExistingEventDatabase(server.Instance,
                                                                 eventsDatabase,
                                                                 server.DefaultAccess,
                                                                 conn);
                  // reset watermarks
                  server.LowWatermark = -2100000000;
                  server.HighWatermark = -2100000000;
               }
               else
               {
                  // Upgrade existing database to latest version if needed
                  if (!EventDatabase.IsCompatibleSchema(eventsDatabase, conn))
                     EventDatabase.UpgradeEventDatabase(conn, eventsDatabase);

                  int schemaVersion = EventDatabase.GetDatabaseSchemaVersion(conn, eventsDatabase);

                  if (schemaVersion != CoreConstants.RepositoryEventsDbSchemaVersion)
                  {
                     CollectionServer.Instance.SetReindexFlag(true);
                  }

                  // set watermarks to first and last record in existing database
                  int lowWatermark;
                  int highWatermark;

                  EventDatabase.GetWatermarks(eventsDatabase,
                                               out lowWatermark,
                                               out highWatermark,
                                               conn);

                  server.LowWatermark = lowWatermark;

                  if (server.LowWatermark != -2100000000)
                     server.LowWatermark--;
                  server.HighWatermark = highWatermark;
               }

               // Update SystemDatabase Table
               EventDatabase.AddSystemDatabase(server.Instance, eventsDatabase, conn);
            }
            server.EventDatabase = eventsDatabase;
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write("An error occurred creating the events database for this instance. The registration of this server cannot be done at this time.", ex);
            return false;
         }

         // write server record
         if (_convertingNonAudited)
         {
            if (!server.Write(_existingAuditedServer))
               retval = false;
         }
         else
         {
            if (!server.Create(null))
               retval = false;
         }

         if (retval)
         {
            string snapshot = Snapshot.ServerSnapshot(conn, server, false);

            // bump server version number so that agent will synch up	      
            ServerRecord.IncrementServerConfigVersion(conn, server.SrvId);
            
            // Log update
            LogRecord.WriteLog(conn, LogType.NewServer, server.Instance, snapshot, args.User);
            AgentStatusMsg.LogStatus(server.AgentServer, server.Instance, AgentStatusMsg.MsgType.Registered, conn);
         }
         return retval;
      }

      private bool DoesDatabaseExist(string instance, SqlConnection conn, out bool compatibleSchema)
      {
         bool exists;
         compatibleSchema = false;

         string dbName = EventDatabase.GetDatabaseName(instance.ToUpper());
         exists = EventDatabase.DatabaseExists(dbName, conn);

         if (exists)
         {
            // check schema for compatability - equal or upgradeable
            compatibleSchema = EventDatabase.IsUpgradeableSchema(dbName, conn);
         }
         return exists;
      }

      private ServerRecord CreateServerRecord(RegisterServerArgs args, SqlConnection conn, ref string error)
      {
         bool alreadyDeployedManually = false;

         ICollection serverList = ServerRecord.GetServers(conn, false);

         if ((serverList != null) && (serverList.Count != 0))
         {
            foreach (ServerRecord config in serverList)
            {
               if (config.IsAuditedServer)
               {
                  if (config.Instance.ToUpper() == args.Instance.ToUpper())
                  {
                     return null;
                  }

                  // some possible states depend on state of already
                  // audited instances on same computer				      
                  if (config.InstanceServer.ToUpper() == args.Server.ToUpper())
                  {
                     if (_existingAuditedServer == null)
                     {
                        _existingAuditedServer = config;
                     }
                     if (config.IsDeployed)
                     {
                        _alreadyDeployed = true;
                        alreadyDeployedManually = config.IsDeployedManually;
                     }
                  }
               }
               else
               {
                  if (config.Instance.ToUpper() == args.Instance.ToUpper())
                  {
                     _existingAuditedServer = config;
                     _convertingNonAudited = true;
                  }
               }
            }
         }

         ServerRecord server = new ServerRecord();
         server.Connection = conn;

         // General
         server.Instance = args.Instance;
         server.isClustered = false;
         server.InstanceServer = args.Server;
         server.AgentServer = server.InstanceServer;
         server.Description = String.Empty;
         server.IsEnabled = true;
         server.DefaultAccess = 2;
         server.ConfigVersion = 1;
         server.LastKnownConfigVersion = 0;
         server.LastConfigUpdate = DateTime.MinValue;
         server.IsAuditedServer = true;
         server.IsOnRepositoryHost = _repositoryComputer;

         // Agent Settings, We only allow manaul deployment via the CLI
         server.AgentServiceAccount = String.Empty; 
         server.AgentTraceDirectory = String.Empty;

         //the repository comptuer is a special case.
         if (_repositoryComputer)
         {
            server.IsDeployed = true;
            server.IsDeployedManually = false;
            _alreadyDeployed = true;
         }
         else
         {
            if (!_alreadyDeployed)
            {
               server.IsDeployed = false;
               server.IsDeployedManually = true;
            }
            else
            {
               server.IsDeployed = true;
               server.IsDeployedManually = alreadyDeployedManually;
            }
         }

         //If there is no config file, use the default audit settings.
         if (String.IsNullOrEmpty(args.ConfigFile))
         {
                // Audit Settings		
            server.AuditLogins = false;
            server.AuditLogouts = false;
            server.AuditFailedLogins = true;
            server.AuditDDL = true;
            server.AuditAdmin = true;
            server.AuditSecurity = true;
            server.AuditUDE = false;
            server.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
            server.AuditExceptions = false;
            server.AuditUsersList = String.Empty;
            server.AuditTrustedUsersList = String.Empty; //v5.6 SQLCM-5373
            server.AuditUserAll = false;
            server.AuditUserLogins = true;
            server.AuditUserLogouts = true;
            server.AuditUserFailedLogins = true;
            server.AuditUserDDL = true;
            server.AuditUserSecurity = true;
            server.AuditUserAdmin = true;
            server.AuditUserDML = false;
            server.AuditUserSELECT = false;
            server.AuditUserUDE = false;
            server.AuditUserAccessCheck = AccessCheckFilter.SuccessOnly;
            server.AuditUserCaptureSQL = false;
            server.AuditUserCaptureTrans = false;
            server.AuditUserExceptions = false;
            server.AuditUserCaptureDDL = false;
         }
         else
         {
            if (!ConfigureAuditSettings(args.ConfigFile, ref server, ref error))
               return null;
         }
         server.LowWatermark = -2100000000;
         server.HighWatermark = -2100000000;

         // copy agent properties from existing audited instances   
         if (_existingAuditedServer != null)
         {
            server.IsRunning = _existingAuditedServer.IsRunning;
            server.IsCrippled = _existingAuditedServer.IsCrippled;
            server.InsertAgentProperties = true;
            server.AgentServer = _existingAuditedServer.AgentServer;
            server.AgentPort = _existingAuditedServer.AgentPort;
            server.AgentServiceAccount = _existingAuditedServer.AgentServiceAccount;
            server.AgentTraceDirectory = _existingAuditedServer.AgentTraceDirectory;
            server.AgentCollectionInterval = _existingAuditedServer.AgentCollectionInterval;
            server.AgentForceCollectionInterval = _existingAuditedServer.AgentForceCollectionInterval;
            server.AgentHeartbeatInterval = _existingAuditedServer.AgentHeartbeatInterval;
            server.AgentLogLevel = _existingAuditedServer.AgentLogLevel;
            server.AgentMaxFolderSize = _existingAuditedServer.AgentMaxFolderSize;
            server.AgentMaxTraceSize = _existingAuditedServer.AgentMaxTraceSize;
            server.AgentMaxUnattendedTime = _existingAuditedServer.AgentMaxUnattendedTime;
            server.AgentTraceOptions = _existingAuditedServer.AgentTraceOptions;
            server.AgentVersion = _existingAuditedServer.AgentVersion;
            server.TimeLastHeartbeat = _existingAuditedServer.TimeLastHeartbeat;
         }

         if (_convertingNonAudited)
         {
            server.SrvId = ServerRecord.GetServerId(conn, server.Instance);
         }
         return server;
      }

      private bool ConfigureAuditSettings(string config, ref ServerRecord server, ref string error)
      {
         try
         {
            AuditTemplate auditSettings;
            InstanceTemplate tmp;
            XMLHandler<InstanceTemplate> reader = new XMLHandler<InstanceTemplate>();
            tmp = reader.Read(config);
            auditSettings = tmp.AuditTemplate;

            if (auditSettings != null)
            {
               if (auditSettings.ApplyServerSettings(server, true))
                  return auditSettings.ApplyServerUserSettings(server, true);
            }
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose, 
                                     String.Format("An error occurred reading the XML template file {0}.",
                                                    config),
                                     e,
                                     true);
            error = String.Format("Unable to process the config file {0}. {1}", config, e.Message);
         }
         return false;
      }

      private bool Activate(string instance, SQLcomplianceConfiguration config)
      {
         bool activated = false;

         try
         {
            // Make sure the service is started
            string agentServer = instance;

            if (agentServer.IndexOf("\\") != -1)
               agentServer = agentServer.Substring(0, agentServer.IndexOf("\\"));

            try
            {
               // This will fail across untrusted domains/workgroups.  However, it is
               //  not a fatal error, so we silently catch and move along.  The true
               //  point of failure will be in the following AgentManager.Activate().
               AgentServiceManager serviceManager = new AgentServiceManager(null, null, agentServer, null, null);
               serviceManager.Start();
            }
            catch (Exception) { }

            AgentManager.GetAgentCommand(instance).Activate(instance);
            ServerRecord.UpdateLastAgentContact(instance);
            activated = true;
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, ex, true);
         }
         return activated;
      }

      #endregion

      #region RegisterDatabase
      public CMCommandResult RegisterDatabase(RegisterDatabaseArgs args)
      {
         CMCommandResult retVal = new CMCommandResult();
         SQLcomplianceConfiguration cmConfig = new SQLcomplianceConfiguration();
         Repository rep = null;
         string errorMsg = string.Empty;
         ServerRecord server = new ServerRecord();
         StringBuilder builder = new StringBuilder();
         DatabaseRecord db = new DatabaseRecord(); 

         try
         {
            rep = new Repository();
            rep.OpenConnection();
            cmConfig.Read(rep.connection);
            server.Connection = rep.connection;

            if (!server.Read(args.Instance))
            {
               builder.AppendFormat("Unable to register database {0}.  Instance {1} is not registered with SQLCompliance Manager.", args.Database, args.Instance);
               retVal.ResultCode = ResultCode.Error;
            }
            else
            {

               if (!LoadDatabases(server, cmConfig, rep.connection))
               {
                  builder.AppendFormat("Unable to load the list of databases from {0}. Make sure the SQLcompliance agent has been deployed to this audited instance and is running.", server.Instance);
                  retVal.ResultCode = ResultCode.Error;
               }
               else
               {
                  if (!DatabaseExistsOnInstance(args))
                  {
                     builder.AppendFormat("Unable to add the specified database {0}.  It does not exist on the audited instance or is already an audited database. \r\n NOTE: The database name is case sensitive.", args.Database);
                     retVal.ResultCode = ResultCode.Error;
                  }
                  else
                  {
                     string error = String.Empty;

                     Dictionary<string, DBO> auditedTables =new Dictionary<string,DBO>();
                     List<SensitiveColumnTableRecord> sensitiveColumnTables = new List<SensitiveColumnTableRecord>();

                     db = CreateDatabaseRecord(args, rep.connection, cmConfig, server, ref error, auditedTables, sensitiveColumnTables);

                     if (db == null || !WriteDatabaseRecord(db, rep.connection, args.User))
                     {
                        builder.AppendFormat("Unable to create the database record. {0}", error);
                        retVal.ResultCode = ResultCode.Error;
                     }
                     else
                     {
                        retVal.ResultCode = ResultCode.Success;

                        //apply the user tables and sensitive column tables if they are not empty.
                        if (auditedTables.Count > 0)
                        {
                           List<DBO> newTables = new List<DBO>(auditedTables.Values);

                           if (!DBO.UpdateUserTables(rep.connection, newTables, 0, db.DbId, null, true))
                           {
                              builder.Append("Unable to add the list of filtered user tables.");
                              retVal.ResultCode = ResultCode.Error;
                           }
                        }

                        if (sensitiveColumnTables.Count > 0 && retVal.ResultCode == ResultCode.Success)
                        {
                           List<SensitiveColumnTableRecord> scRecords = new List<SensitiveColumnTableRecord>(sensitiveColumnTables);
                           
                           if (!SensitiveColumnTableRecord.CreateUserTables(rep.connection, scRecords, server.SrvId, db.DbId, null))
                           {
                              builder.Append("Unable to add the list of Sensitive Column tables.");
                              retVal.ResultCode = ResultCode.Error;
                           }
                        }
                     }
                  }
               }
            }
         }
         catch (Exception ex)
         {
            errorMsg = ex.Message;
         }
         finally
         {

            if (retVal.ResultCode != ResultCode.Success)
            {
               if (!String.IsNullOrEmpty(errorMsg))
               {
                  builder.AppendFormat("\r\n{0}", errorMsg);
               }
            }
            if (rep != null)
               rep.CloseConnection();
         }
         if (builder.Length > 0)
            retVal.AddResultString(builder.ToString());
         return retVal;
      }

      private bool DatabaseExistsOnInstance(RegisterDatabaseArgs args)
      {
         foreach (RawDatabaseObject db in _databaseList)
         {
            if (db.name == args.Database)
            {
               args.SqlDBId = db.dbid;
               return true;
            }
         }
         return false;
      }

      private bool LoadDatabases(ServerRecord server, SQLcomplianceConfiguration cmConfig, SqlConnection conn)
      {
         bool loaded = false;

         ICollection dbList = null;

         // load database list via agent (if deployed)
         if (server.IsDeployed && server.IsRunning)
         {
             string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), cmConfig.Server, cmConfig.ServerPort);
            try
            {
               AgentManager manager = CoreRemoteObjectsProvider.AgentManager(cmConfig.Server, cmConfig.ServerPort);
               dbList = manager.GetRawUserDatabases(server.Instance);
            }
            catch (Exception ex)
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                        String.Format("LoadDatabases: URL: {0} Instance {1}", url, server.Instance), ex, ErrorLog.Severity.Warning);
               dbList = null;
            }
         }

         if ((dbList != null))
         {
            if ((dbList.Count != 0))
            {
               foreach (RawDatabaseObject db in dbList)
               {
                  // only load if this database doesnt already exist in our other DB
                  // potentially slow but we are using selects from two sql servers
                  DatabaseRecord dbrec = new DatabaseRecord();
                  dbrec.Connection = conn;

                  if (!dbrec.Read(server.Instance, db.name) &&
                     !(server.IsSqlSecureDb && IsSQLsecureOwnedDB(db.name, conn)))
                  {
                     _databaseList.Add(db);
                  }
               }
            }
            loaded = true;
         }

         if (loaded)
         {
            dbList = null;
  
            // load database list via agent (if deployed)
            if (server.IsDeployed && server.IsRunning)
            {
               try
               {
                  AgentManager manager = CoreRemoteObjectsProvider.AgentManager(cmConfig.Server, cmConfig.ServerPort);
                  dbList = manager.GetRawSystemDatabases(server.Instance);
               }
               catch (Exception )
               {
                  dbList = null;
               }
            }
            
			   if (dbList != null)  
			   {
               if (dbList.Count != 0)
               {
                  foreach (RawDatabaseObject db in dbList)
                  {
                     // only load if this database doesnt already exist in our other DB
                     // potentially slow but we are using selects from two sql servers
                     DatabaseRecord dbrec = new DatabaseRecord();
                     dbrec.Connection = conn;

                     if (!dbrec.Read(server.Instance, db.name))
                     {
                        _databaseList.Add(db);
                     }
                  }
               }
            }
         }
         return loaded;
      }

      private bool IsSQLsecureOwnedDB(string dbName, SqlConnection conn)
      {
         bool retval = false;

         try
         {
            string selectQuery = String.Format("SELECT count(*) FROM {0} WHERE databaseName = {1}",
                                               CoreConstants.RepositorySystemDatabaseTable,
                                               SQLHelpers.CreateSafeString(dbName));

            SqlCommand cmd = new SqlCommand(selectQuery, conn);
            int count;

            object obj = cmd.ExecuteScalar();
            if (obj is DBNull)
               count = 0;
            else
               count = (int)obj;


            if (count != 0)
               retval = true;
         }
         catch
         {
         }

         return retval;
      }

      private DatabaseRecord CreateDatabaseRecord(RegisterDatabaseArgs args, 
                                                  SqlConnection conn, 
                                                  SQLcomplianceConfiguration cmConfig, 
                                                  ServerRecord server,
                                                  ref string error,
                                                  Dictionary<string, DBO> auditedTables,
                                                  List<SensitiveColumnTableRecord> sensitiveColumnTables)
      {
         DatabaseRecord db = new DatabaseRecord();
         db.Connection = conn;

         // General
         db.SrvId = server == null ? -1 : server.SrvId;
         db.SrvInstance = args.Instance;
         db.Name = args.Database;
         db.SqlDatabaseId = args.SqlDBId;
         db.Description = "";
         db.IsEnabled = true;
         db.IsSqlSecureDb = false;

         if (!String.IsNullOrEmpty(args.ConfigFile))
         {
            if (!ConfigureDatabaseSettings(args.ConfigFile, db, server, ref error, auditedTables, sensitiveColumnTables))
               return null;
         }
         else if (!String.IsNullOrEmpty(args.Regulation))
         {
            if (!ApplyDatabaseRegulation(db, args.Regulation, conn, ref error))
               return null;

            db.AuditDmlAll = true;
            db.AuditUserTables = cmConfig.AuditUserTables;
            db.AuditSystemTables = cmConfig.AuditSystemTables;
            db.AuditStoredProcedures = cmConfig.AuditStoredProcedures;
            db.AuditDmlOther = cmConfig.AuditDmlOther;
         }
         else
         {
            // Audit Settings		
            db.AuditDDL = true;
            db.AuditSecurity = true;
            db.AuditAdmin = true;
            db.AuditDML = false;
            db.AuditSELECT = false;
            db.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
            db.AuditCaptureSQL = false;
            db.AuditCaptureTrans = false;
            db.AuditExceptions = false;
            db.AuditDmlAll = true;
            db.AuditUserTables = cmConfig.AuditUserTables;
            db.AuditSystemTables = cmConfig.AuditSystemTables;
            db.AuditStoredProcedures = cmConfig.AuditStoredProcedures;
            db.AuditDmlOther = cmConfig.AuditDmlOther;
         }

         if (sensitiveColumnTables.Count > 0)
            db.AuditSensitiveColumns = true;
         return db;
      }

      private bool ConfigureDatabaseSettings(string config, 
                                             DatabaseRecord db, 
                                             ServerRecord server, 
                                             ref string error,
                                             Dictionary<string, DBO> auditedTables,
                                             List<SensitiveColumnTableRecord> sensitiveColumnTables)
      {
         bool retVal = false;
         AuditTemplate auditSettings;
         InstanceTemplate tmp;
         XMLHandler<InstanceTemplate> reader = new XMLHandler<InstanceTemplate>();
         Dictionary<string, DataChangeTableRecord> dataChangeTables = new Dictionary<string,DataChangeTableRecord>();

         try
         {
            tmp = reader.Read(config);
            auditSettings = tmp.AuditTemplate;

            if (auditSettings != null)
            {
               if (!auditSettings.ApplyDatabaseSetting(server, db, auditedTables,
                                                          dataChangeTables,
                                                          sensitiveColumnTables,
                                                          true, "localhost", 5201))  //the collection server and port will not be used because the tables, BAD tables and SC tables are empty.
               {
                  error = "Unable to apply the databse config file.";
               }
               else
               {
                  retVal = true;
               }
            }
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                     String.Format("An error occurred reading the XML template file {0}.",
                                                    config),
                                     e,
                                     true);
            error = String.Format("Unable to process the config file {0}. {1}", config, e.Message);
         }
         return retVal;
      }

      private bool ApplyDatabaseRegulation(DatabaseRecord db, string regulationArg, SqlConnection conn, ref string error)
      {
         RegulationSettings settings;
         DatabaseRecord tempDb = new DatabaseRecord();
         Dictionary<int, RegulationSettings> regulationSettings = new Dictionary<int, RegulationSettings>();
         bool retVal = true;
         string[] regulations = regulationArg.Split(',');
         bool hipaa = false;
         bool pci = false;
         bool disa = false;
         bool nerc = false;
         bool cis = false;
         bool sox = false;
         bool ferpa = false;

         //read the regulation info;
         try
         {
            regulationSettings = RegulationDAL.LoadRegulationCategories(conn);
         }
         catch (Exception e)
         {
            error = String.Format("Unable to load the regulation information for the repository. {0}", e.Message);
            return false;
         }

         //we have already validate the contents of the regulation list so start processing it.
         foreach (string regulation in regulations)
         {
            // apply PCI
            if (regulation.ToUpper() == "PCI")
            {
               pci = true;

               if (regulationSettings.TryGetValue((int)Regulation.RegulationType.PCI, out settings))
               {
                  tempDb.AuditDDL =       ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition);
                  tempDb.AuditSecurity =  ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges);
                  tempDb.AuditAdmin =     ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity);
                  tempDb.AuditDML =       ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification);
                  tempDb.AuditSELECT =    ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select);

                  if ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess)
                     tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                  else if ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess)
                     tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                  else
                     tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                  tempDb.AuditCaptureSQL = CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText);
                  tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions);
                  tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns);
               }
               else
                  retVal = false;
            }

            // apply HIPAA
            // the OR against the server settings is done because the user can select more than one template.  When more than one template is 
            // selected, the options are combined togeher
            if (regulation.ToUpper() == "HIPAA")
            {
               hipaa = true;

               if (regulationSettings.TryGetValue((int)Regulation.RegulationType.HIPAA, out settings))
               {
                  tempDb.AuditDDL = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) || tempDb.AuditDDL;
                  tempDb.AuditSecurity = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) || tempDb.AuditSecurity;
                  tempDb.AuditAdmin = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) || tempDb.AuditAdmin;
                  tempDb.AuditDML = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) || tempDb.AuditDML;
                  tempDb.AuditSELECT = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select) || tempDb.AuditSELECT;

                  if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                     tempDb.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                  else if (((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) || (tempDb.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                     tempDb.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                  else
                     tempDb.AuditAccessCheck = AccessCheckFilter.NoFilter;

                  tempDb.AuditCaptureSQL = (CoreConstants.AllowCaptureSql && ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)) || tempDb.AuditCaptureSQL;
                  tempDb.AuditCaptureTrans = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions) || tempDb.AuditCaptureTrans;
                  tempDb.AuditSensitiveColumns = ((settings.DatabaseCategories & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) || tempDb.AuditSensitiveColumns;
               }
               else
                  retVal = false;
            }
         }

         //return the database setttings
         db.HIPAA = hipaa;
         db.PCI = pci;
         db.DISA = disa;
         db.NERC = nerc;
         db.CIS = cis;
         db.SOX = sox;
         db.FERPA = ferpa;
         db.AuditDDL = tempDb.AuditDDL;
         db.AuditSecurity = tempDb.AuditSecurity;
         db.AuditAdmin = tempDb.AuditAdmin;
         db.AuditDML = tempDb.AuditDML;
         db.AuditSELECT = tempDb.AuditSELECT;
         db.AuditAccessCheck = tempDb.AuditAccessCheck;
         db.AuditCaptureSQL = tempDb.AuditCaptureSQL;
         db.AuditCaptureTrans = tempDb.AuditCaptureTrans;
         db.AuditSensitiveColumns = tempDb.AuditSensitiveColumns;
         return retVal;
      }

      private bool WriteDatabaseRecord(DatabaseRecord db, SqlConnection conn, string user)
      {
         bool retVal = false;

         try
         {
            retVal = db.Create(null);
         }
         finally
         {
            if (retVal)
            {
               // Register change to server and perform audit log				 
               ServerRecord.IncrementServerConfigVersion(conn, db.SrvId);
               LogRecord.WriteLog(conn,
                                  LogType.NewDatabase,
                                  db.SrvInstance,
                                  Snapshot.DatabaseSnapshot(conn, db.DbId, db.Name, true),
                                  user);
            }
         }
         return retVal;
      }

      #endregion

      #endregion

      #region Private methods
      //--------------------------------------------------------------------------------------
      // DeleteFile - Deletes an existing file
      //--------------------------------------------------------------------------------------
      private static void DeleteFile( string filename )
      {
         try
         {
            if ( File.Exists( filename ) )
               File.Delete( filename );
         }
         catch ( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                     String.Format( "An error occurred deleting {0}.",
                                                    filename ),
                                     e,
                                     ErrorLog.Severity.Warning,
                                     true );
         }
      }

	   //--------------------------------------------------------------------------------------
      // CheckInstance - Makes sure that incoming agent messages are from registered instance
      //--------------------------------------------------------------------------------------
      internal static void
         CheckForValidInstance(
            string            instanceName
         ) 
      {
         bool   serverExists = false;
         string sql = "";
         
         Repository rep = new Repository();
         
         try
         {
            rep.OpenConnection();
         
            sql = String.Format( "SELECT count(*) FROM {0}..{1} WHERE instance={2} AND isAuditedServer=1",
                                 SQLHelpers.CreateSafeDatabaseName(CoreConstants.RepositoryDatabase),
                                 CoreConstants.RepositoryServerTable,
                                 SQLHelpers.CreateSafeString(instanceName) );
            using ( SqlCommand cmd = new SqlCommand( sql, rep.connection ) )
            {
               int count;
				   object obj = cmd.ExecuteScalar();
				   if( obj is DBNull )
					   count = 0;
				   else
					   count = (int)obj;
               
               if ( count != 0 )
               {
                  serverExists = true;
               }
            }
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Default,
                                     instanceName,
                                     sql,
                                     ex );
         }
         finally
         {
            rep.CloseConnection();
         }
         
         if ( ! serverExists )
         {
            string s = String.Format( CoreConstants.Exception_RejectedAgentRequest, instanceName);
            
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                     s,
                                     sql,
                                     ErrorLog.Severity.Warning );
            throw new Exception( s );
         }
      }
	   
      
      #endregion
	   
   }
   
   [Serializable]
   public class StatusEventArgs
   {
      public int percentDone;
   }
   
   public class RemoteOpStatus : MarshalByRefObject
	{
	   string operation;
	   int percentDone;
	   string stage;
	   string message;
	   
	   public RemoteOpStatus( )
	   {
	      operation = "";
	      percentDone = 0;
	      stage = "Initialization";
	   }
	   
	   public string Operation
	   {
	      get { return operation; }
	      set { operation = value; }
	   }
	   
	   public int PercentDone
	   {
	      get { return percentDone; }
	      set { percentDone = value; }
	   }
	   
	   public string Stage
	   {
	      get { return stage; }
	      set { stage = value; }
	   }
	   
	   public string Message
	   {
	      get { return message; }
	      set { message = value; }
	   }
	}
   

}
