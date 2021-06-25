using System.Data.SqlClient;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;

namespace SQLcomplianceCwfAddin.Helpers
{
    /// <summary>
    /// This class was BASED ON class from Idera.SQLcompliance.Application.GUI.Helper and should be moved to the SQLcomplianceCore in the future release
    /// </summary>
    public class ServerUpdate
    {
        #region RegisterChange

        //-----------------------------------------------------------------------
        // RegisterChange
        //-----------------------------------------------------------------------
        public static void
           RegisterChange(
              int serverId,
              LogType logType,
              string serverName
           )
        {
            RegisterChange(serverId, logType, serverName, "", null);
        }

        //-----------------------------------------------------------------------
        // RegisterChange
        //-----------------------------------------------------------------------
        public static void
           RegisterChange(
              int serverId,
              LogType logType,
              string serverName,
              SqlTransaction transaction,
              SqlConnection connection
           )
        {
            RegisterChange(serverId, logType, serverName, "", transaction, connection);
        }

        //-----------------------------------------------------------------------
        // RegisterChange
        //-----------------------------------------------------------------------
        public static void
           RegisterChange(
              int serverId,
              LogType logType,
              string serverName,
              string info,
              SqlConnection connection
           )
        {
            RegisterChange(serverId, logType, serverName, info, null, connection);
        }

        #endregion

        #region LogChange

        //-----------------------------------------------------------------------
        // LogChange
        //-----------------------------------------------------------------------
        public static void
           LogChange(
              LogType logType,
              string serverName
           )
        {
            LogChange(logType, serverName, "", null);
        }

        //-----------------------------------------------------------------------
        // LogChange
        //-----------------------------------------------------------------------
        public static void
           LogChange(
              LogType logType,
              string serverName,
              SqlTransaction transaction,
              SqlConnection connection
           )
        {
            LogChange(logType, serverName, "", transaction, connection);
        }

        //-----------------------------------------------------------------------
        // LogChange
        //-----------------------------------------------------------------------
        public static void
           LogChange(
              LogType logType,
              string serverName,
              string info,
              SqlConnection connection
           )
        {
            LogChange(logType, serverName, info, null, connection);
        }

        #endregion

        #region RegisterChange/LogChange - Real Thing

        //-----------------------------------------------------------------------
        // RegisterChange
        //-----------------------------------------------------------------------
        public static void
           RegisterChange(
              int serverId,
              LogType logType,
              string serverName,
              string info,
              SqlTransaction transaction,
              SqlConnection connection
           )
        {
            // bump server version number so that agent will synch up	      
            ServerRecord.IncrementServerConfigVersion(connection,
                                                       serverId);

            // Log update
            LogRecord.WriteLog(connection,
                               logType,
                               serverName,
                               info);

            // inform agent of change?                                 
        }

        //-----------------------------------------------------------------------
        // LogChange
        //-----------------------------------------------------------------------
        public static void
           LogChange(
              LogType logType,
              string serverName,
              string info,
              SqlTransaction transaction,
              SqlConnection connection
           )
        {
            // Log update
            LogRecord.WriteLog(connection,
                               logType,
                               serverName,
                               info);

            // inform agent of change?                                 
        }


        #endregion
    }
}
