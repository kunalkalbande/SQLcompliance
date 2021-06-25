using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.Data.SqlClient;


using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.TraceProcessing;

namespace Idera.SQLcompliance.Core.Collector
{
   /// <summary>
   /// Singleton remote object to handle buffers of db activity
   ///  TODO: Not a singleton object - write all the code for TraceServer
   ///  sent from remote agent
   ///
   /// Runs in SQLsecure collector service process space.
   /// </summary>
   public class TraceServer : System.MarshalByRefObject
   {

      private static TraceServer instance;
      private static string      traceDirectory;

      #region Property

      public static TraceServer Instance
      {
         get { return instance; }
      }

      public static string TraceDirectory
      {
         get { return traceDirectory; }
      }

      #endregion

      #region Constructor

		static TraceServer()
		{
         instance = new TraceServer();
         traceDirectory = GetTraceDirectory();
		}

      #endregion
		
      #region Public Methods

		public bool
		   GetAgentTrace(
		      string                instance,
		      string                compressedTraceFile,
		      string                compressedAuditFile,
		      bool                  privilegedUserTrace,
		      bool                  sqlSecureTrace )
		{
		   // Make sure trace directory exists - alert if it doesnt but recreate
		   CheckTraceDirectory();
		   
		   // Input - server, compressedTraceFile, compressedAuditFile
		   int traceChecksum = TraceFile.CalculateChecksum( compressedTraceFile );
         int auditChecksum = TraceFile.CalculateChecksum( compressedAuditFile );

         // create trace processing job entry		    
		   TraceJobInfo jobInfo = new TraceJobInfo();
		   jobInfo.compressedTraceFile = compressedTraceFile;
		   jobInfo.traceChecksum       = traceChecksum;
		   jobInfo.compressedAuditFile = compressedAuditFile;
		   jobInfo.auditChecksum       = auditChecksum;
		   jobInfo.instance            = instance;
		   jobInfo.privilegedUserTrace = privilegedUserTrace;
		   jobInfo.sqlSecureTrace      = sqlSecureTrace;

         // insert trace job		   
		   try
		   {
		      jobInfo.Insert();
		   }
		   catch ( Exception ex )
		   {
		      // log on server side and throw exception back to agent
		      ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
		                               CoreConstants.Exception_ErrorReceivingTrace,
		                               ex );
            throw ex;		                               
		   }

         // signal to pool that job is available - its ok if this throws an exception
         // we will pick up job later on restart or in an idle period
		   try
		   {
		      TraceJobPool.SignalNewTraceAvailable();
		   }
		   catch ( Exception ex )
		   {
		      // log on server side and throw exception back to agent
		      ErrorLog.Instance.Write( ErrorLog.Level.Debug,
		                               "TraceJobPool.SignalNewTraceAvailable",
		                               ex,
		                               ErrorLog.Severity.Informational );
		   }
		   
		   // update record to show last time we received something for this instance
         Repository rep = new Repository();
         
         try
         {
            rep.OpenConnection();
            
            string cmdStr = String.Format( "UPDATE {0} SET " +
			                                  "timeLastCollection = GETUTCDATE() " +
			                                  "where instance = '{1}'",
			                                  CoreConstants.RepositoryServerTable,
			                                  instance );
            using ( SqlCommand cmd = new SqlCommand( cmdStr, rep.connection ) )
            {
			      cmd.ExecuteNonQuery();
			   }
			}
			catch (Exception ex)
			{
			   // no big deal if we fail here - but log for our own debugging purposes
			   ErrorLog.Instance.Write( ErrorLog.Level.Debug,
			                            "Error updating servers table with last collection time",
			                            ex.Message );
			}
			finally
			{
			   rep.CloseConnection();
			}
	
		   return false;
		}

      #endregion

      #region Private Methods

      //-----------------------------------------------------------------------		
      // CheckTraceDirectory - Make sure directory to store traces still exists
      //-----------------------------------------------------------------------		
		private void
		   CheckTraceDirectory()
		{
		    if ( ! Directory.Exists( traceDirectory ) )
		    {
            InternalAlert.Raise( CoreConstants.Alert_DeletedTraceDirectory );
            Directory.CreateDirectory( traceDirectory );
            // TODO: Lockdown trace directory with ACLs                                 
         }
		}

      private static string
         GetTraceDirectory( )
      {
         RegistryKey rk  = null;
         RegistryKey rks = null;
         string directory = null;
         
         try
         {
            rk  = Registry.LocalMachine;
            rks = rk.CreateSubKey(CoreConstants.CollectionService_RegKey);
            
            directory = (string)rks.GetValue( CoreConstants.CollectionService_RegVal_TraceDir,
               "" );
	                                                        
         }
         catch (Exception)
         {
         }
         finally
         {
            if ( rks != null )rks.Close();
            if ( rk != null )rk.Close();
         }


         // validate preferences         
         if ( directory == "" )
         {
            string msg = String.Format( CoreConstants.Alert_InvalidTraceDirectory,
               directory  );
            InternalAlert.Raise( msg );
            throw new Exception (msg);
         }	  
         return directory;                                          
      }


      #endregion
	}
}
