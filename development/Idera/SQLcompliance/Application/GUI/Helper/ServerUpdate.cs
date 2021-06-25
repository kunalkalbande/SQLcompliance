using System;
using System.Data.SqlClient;

using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Application.GUI.SQL;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
	/// <summary>
	/// Summary description for ServerUpdate.
	/// </summary>
	public class ServerUpdate
	{
		public ServerUpdate()
		{
			//
			// TODO: Add constructor logic here
			//
		}

      #region RegisterChange 
      
      //-----------------------------------------------------------------------
      // RegisterChange
      //-----------------------------------------------------------------------
		public static void
		   RegisterChange(
		      int               serverId,
		      LogType           logType,
		      string            serverName
		   )
      {
		   RegisterChange( serverId, logType, serverName, "", null );
      }
      		      
      //-----------------------------------------------------------------------
      // RegisterChange
      //-----------------------------------------------------------------------
		public static void
		   RegisterChange(
		      int               serverId,
		      LogType           logType,
		      string            serverName,
		      SqlTransaction    transaction
		   )
      {
		   RegisterChange( serverId, logType, serverName, "", transaction );
      }
      		      
      //-----------------------------------------------------------------------
      // RegisterChange
      //-----------------------------------------------------------------------
		public static void
		   RegisterChange(
		      int               serverId,
		      LogType           logType,
		      string            serverName,
		      string            info
		   )
      {
		   RegisterChange( serverId, logType, serverName, info, null );
      }
      
      #endregion

      #region LogChange 
      
      //-----------------------------------------------------------------------
      // LogChange
      //-----------------------------------------------------------------------
		public static void
		   LogChange(
		      LogType           logType,
		      string            serverName
		   )
      {
		   LogChange( logType, serverName, "", null );
      }
      		      
      //-----------------------------------------------------------------------
      // LogChange
      //-----------------------------------------------------------------------
		public static void
		   LogChange(
		      LogType           logType,
		      string            serverName,
		      SqlTransaction    transaction
		   )
      {
		   LogChange( logType, serverName, "", transaction );
      }
      		      
      //-----------------------------------------------------------------------
      // LogChange
      //-----------------------------------------------------------------------
		public static void
		   LogChange(
		      LogType           logType,
		      string            serverName,
		      string            info
		   )
      {
		   LogChange( logType, serverName, info, null );
      }
      
      #endregion
      
      #region RegisterChange/LogChange - Real Thing
      		      
      //-----------------------------------------------------------------------
      // RegisterChange
      //-----------------------------------------------------------------------
		public static void
		   RegisterChange(
		      int               serverId,
		      LogType           logType,
		      string            serverName,
		      string            info, 
		      SqlTransaction    transaction
		   )
		{
            // bump server version number so that agent will synch up	      
	         ServerRecord.IncrementServerConfigVersion( Globals.Repository.Connection,
	                                                    serverId );
      	   
	         // Log update
	         LogRecord.WriteLog( Globals.Repository.Connection,
                                logType,
                                serverName,
                                info );
                                 
           // inform agent of change?                                 
      }		   
      
      //-----------------------------------------------------------------------
      // LogChange
      //-----------------------------------------------------------------------
		public static void
		   LogChange(
		      LogType           logType,
		      string            serverName,
		      string            info, 
		      SqlTransaction    transaction
		   )
		{
	         // Log update
	         LogRecord.WriteLog( Globals.Repository.Connection,
                                logType,
                                serverName,
                                info );
                                 
           // inform agent of change?                                 
      }		   
      
      
      #endregion
      
	}
}
