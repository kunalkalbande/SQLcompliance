using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Idera.SQLcompliance.Core.Agent;


namespace Idera.SQLcompliance.Core.Status
{
	/// <summary>
	/// Summary description for StatusCache.
	/// </summary>
	public class StatusCache
	{
	   private static bool empty;
	   private static FileStream stream;
      private static BinaryFormatter formatter = new BinaryFormatter();
	   private static object lockObject = new object();
	   private static StatusCache instance;
	   private static string cacheFileName;
	   
      #region constructors
	   
	   static StatusCache()
	   {
   	   instance = new StatusCache();
	   }
	   
		private StatusCache()
		{
		   Init();
		}
	   
      #endregion
	   
      #region Properties
	   
	   public static StatusCache Instance
	   {
	      get { return instance; }
	   }
	   
	   public bool IsEmpty
	   {
	      get { return empty; }
	   }
	   
      #endregion
	   
      #region Public Methods
	   
      public void Save( AgentStatusMsg msg )
      {
         switch( msg.Type )
         {
            // cache these types only
            case AgentStatusMsg.MsgType.Startup :
            case AgentStatusMsg.MsgType.Shutdown :
            case AgentStatusMsg.MsgType.Update :
            case AgentStatusMsg.MsgType.UnknownInstance:
            case AgentStatusMsg.MsgType.TraceAltered :
            case AgentStatusMsg.MsgType.TraceStopped :
            case AgentStatusMsg.MsgType.TraceClosed :
               break;
            default:
               return;
         }
         
         lock( lockObject )
         {
            stream = GetCacheFile();
            stream.Seek(0, SeekOrigin.End);
            formatter.Serialize(stream, false );
            formatter.Serialize(stream, msg);
            empty = false;
	         ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Status message cached.");
         }
         

      }
	   
	   public void Flush()
	   {
	      lock( lockObject )
	      {
	         if( empty )
	            return;
	         try 
	         {
	            stream = GetCacheFile();
	            stream.Flush();
	            stream.Seek(0, SeekOrigin.Begin);
	         
	            long start;
	            long end;
	            AgentStatusMsg msg;
	            RemoteStatusLogger remoteLogger;
	            bool sent;
					
	            // Get the singleton remote status logger
	            remoteLogger = StatusLogger.GetRemoteLogger( SQLcomplianceAgent.Instance.Server,
	                                                         SQLcomplianceAgent.Instance.ServerPort);

	            // Log status remotely
	         
	            while( stream.Position < stream.Length )
	            {
	               try
	               {
	                  start = stream.Position;
	                  sent = (bool) formatter.Deserialize(stream);
	                  msg = (AgentStatusMsg)formatter.Deserialize(stream);
	                  msg.classVersion = CoreConstants.SerializationVersion;
	                  msg.cached = true;
	                  end = stream.Position;
	            
	                  if( sent )
	                     continue;
	            
	                  try
	                  {
	                     // send the status message
	                     remoteLogger.SendStatusEx( msg );
	                  }
	                  catch( Exception re )
	                  {
	                     ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Error sending cached status to collection server.", re);
	                     stream.Seek(0, SeekOrigin.End);
	                     return;
	                  }
	            
	                  // mark the message as sent so we won't resend it in case of errors
	                  stream.Seek(start, SeekOrigin.Begin);
	                  formatter.Serialize(stream, true);
	                  stream.Seek(end, SeekOrigin.Begin);
	               }
	               catch( Exception ex)
	               {
	                  // Cache file is corrupted somehow.
	                  ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Error processinging cached status.", ex);
	                  break;
	               }
	            
	            }
	         
	            empty = true;
	            stream.Close();
	            File.Delete(cacheFileName);
	            stream = null;
	            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Status cache flushed.");
	         }
	         catch( Exception e )
	         {
	            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Error processing status cache file.", e);
	            if( stream != null )	stream.Seek(0, SeekOrigin.End);

	         }
	      }
	      
	   }
	   
	   
      #endregion
	   
      #region Private Methods
	   
	   private void Init()
	   {
	      cacheFileName = Path.GetTempPath() + @"\" + CoreConstants.Agent_StatusCache_FileName;
	      if( File.Exists(cacheFileName) )
	      {
	         stream = GetCacheFile();
	         stream.Seek(0, SeekOrigin.End);
	         empty = false;
	      }
	      else
	      {
	         stream = null;
	         empty = true;
	      }
	   }
	   
	   private FileStream GetCacheFile()
	   {
	      if( stream == null )
	         stream = new FileStream( cacheFileName,
						                   FileMode.OpenOrCreate, 
						                   FileAccess.ReadWrite,
						                   FileShare.Read );
	      return stream;

	   }
	   
      #endregion
	}
   
   
}
