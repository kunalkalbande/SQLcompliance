using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Idera.SQLcompliance.Core
{
   public enum SystemAlertType
   {
      AgentWarning = 1001,
      AgentWarningResolution = 3001,
      AgentConfigurationError = 2001,
      AgentConfigurationResolution = 4001,
      TraceDirectoryError = 2002,
      TraceDirectoryResolution = 4002,
      SqlTraceError = 2003,
      SqlTraceResolution = 4003,
      ServerConnectionError = 2004,
      ServerConnectionResolution = 4004,
      CollectionServiceConnectionError = 2005,
      CollectionServiceConnectionResolution = 4005,
      ClrError = 2006,
      ClrResolution = 4006
   };


   [Serializable()]
   public class SystemAlert
   {
      private SystemAlertType _alertType;
      private DateTime _whenOccurred ;
      private string _details;
      private string _instance;

      public SystemAlert(SystemAlertType alertType, DateTime occurred, string instance, string details)
		{
         _alertType = alertType;
         _whenOccurred = occurred;
         _details = details;
         _instance = instance;
		}

      public SystemAlertType AlertType
      {
         get { return _alertType; }
         set { _alertType = value; }
      }

      public DateTime WhenOccurred
      {
         get { return _whenOccurred; }
         set { _whenOccurred = value; }
      }

      public string Details
      {
         get { return _details; }
         set { _details = value; }
      }

      public string Instance
      {
         get { return _instance; }
         set { _instance = value; }
      }

      public bool StoreAlert(SqlConnection conn)
      {
         return StoreAlert( conn, true );
      }

      public bool StoreAlert(SqlConnection conn, bool allErrorsResolved)
      {
         try
         {
            string cmdStr = String.Format("INSERT INTO {0} (eventTime, instance,eventType,details) VALUES ({1},{2},{3},{4})",
                                          CoreConstants.RepositoryAgentEventTable,
                                          SQLHelpers.CreateSafeDateTime(_whenOccurred),
                                          SQLHelpers.CreateSafeString(_instance),
                                          (int)_alertType,
                                          SQLHelpers.CreateSafeString(_details));

            using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
            {
               cmd.ExecuteNonQuery();
            }
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write("Unable to store System Alerts.", e);
            return false;
         }
         return SetAgentHealthFlag(conn, allErrorsResolved);
      }
      
      //-----------------------------------------------------------------------------
      // SetAgentHealthFlag: set agentHealth column value in the Servers table.
      // AgentHealth: 0 - healthy
      //              >0 - Unresolved system alerts
      //-----------------------------------------------------------------------------
      public bool SetAgentHealthFlag(SqlConnection conn, bool allErrorsResolved)
      {
         try
         {
            int intType = (int)_alertType;
            UInt64 mask;
            int state = intType % 1000;
            int resolved = intType / 1000;
            bool setErrorFlag = true;
            string op;
            switch( resolved )
            {
               case 1: // warning
                  mask = 0x01;
                  op = "|";
                  break;
               case 3: // warning resolved
                  op = "&";
                  mask = 0xFFFFFFFFFFFFFFFe;
                  setErrorFlag = allErrorsResolved;
                  break;
               case 2: // errors
                  op = "|";
                  mask = (UInt64)(0x0000000000000001 << state);
                  break;
               case 4: // error resolved
                  op = "&";
                  mask = (UInt64)(~(0x0000000000000001 << state));
                  setErrorFlag = allErrorsResolved;  // clear the error flag if all errors are resolved
                  break;
               default:
                  op = "|";
                  mask = 0;
                  break;
            }
            
            if( setErrorFlag )
            {
               string cmdStr =
                  String.Format(
                     "UPDATE {0} SET agentHealth = agentHealth{1}0x{2} WHERE instance = {3} ",
                     CoreConstants.RepositoryServerTable,
                     op,
                     mask.ToString( "X" ),
                     SQLHelpers.CreateSafeString( _instance ) );

               using ( SqlCommand cmd = new SqlCommand( cmdStr, conn ) )
               {
                  cmd.ExecuteNonQuery();
               }
            }
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write("Unable to update agentHealth flag.", e);
            return false;
         }
         return true;
         
      }

      public static List<SystemAlertType> GetAgentHealthDetails(UInt64 agentHealth)
      {
         List<SystemAlertType> retVal = new List<SystemAlertType>();

         if ((agentHealth & 0x0000000000000001ul) != 0)
            retVal.Add(SystemAlertType.AgentWarning);
         if ((agentHealth & 0x0000000000000002ul) != 0)
            retVal.Add(SystemAlertType.AgentConfigurationError);
         if ((agentHealth & 0x0000000000000004ul) != 0)
            retVal.Add(SystemAlertType.TraceDirectoryError);
         if ((agentHealth & 0x0000000000000008ul) != 0)
            retVal.Add(SystemAlertType.SqlTraceError);
         if ((agentHealth & 0x0000000000000010ul) != 0)
            retVal.Add(SystemAlertType.ServerConnectionError);
         if ((agentHealth & 0x0000000000000020ul) != 0)
            retVal.Add(SystemAlertType.CollectionServiceConnectionError);
         if ((agentHealth & 0x0000000000000040ul) != 0)
            retVal.Add(SystemAlertType.ClrError);

         return retVal;
      }
      
      public static bool ResetAgentHealthFlag(SqlConnection conn, string instance)
      {
         try
         {
            string cmdStr =
               String.Format(
                  "UPDATE {0} SET agentHealth = 0 WHERE instance = {1} ",
                  CoreConstants.RepositoryServerTable,
                  SQLHelpers.CreateSafeString(instance));

            using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
            {
               cmd.ExecuteNonQuery();
            }
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write("Unable to reset agentHealth flag.", e);
            return false;
         }
         return true;
      }
   }
}
