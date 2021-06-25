using System;

using Idera.SQLcompliance.Core;

namespace Idera.SQLcompliance.Core.Event
{
	/// <summary>
	/// Summary description for TraceHelper.
	/// </summary>
	public class TraceHelper
	{
		private TraceHelper()
		{
		}

      #region Public Static Methods

      /*
      /// <summary>
      /// CreateDBTraceConfiguration: Convert the database base level audit 
      /// configuration to a SQL Server event trace setting.  
      /// </summary>
      /// <returns></returns>
      public static DBTraceConfiguration 
         CreateDBTraceConfiguration( DBAuditConfiguration auditConfig )
      {
         return ConvertToDBTraceConfig( auditConfig, null );
      }

      /// <summary>
      /// CreateDBTraceConfiguration: Convert the database level audit 
      /// configuration to a SQL Server event trace setting.  The events 
      /// enabled at this level should be removed from the server level by 
      /// adding filters to the server level configuration.
      /// </summary>
      /// <returns></returns>
      public static DBTraceConfiguration 
         CreateDBTraceConfiguration( 
         DBAuditConfiguration auditConfig,
         ServerTraceConfiguration serverTraceConfig )
      {
         return ConvertToDBTraceConfig( auditConfig, serverTraceConfig );
      }

      */

      #endregion

      #region Private Static Methods


      /// <summary>
      /// 
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      public static bool
         IsServerLevelOnlyEvent (
            TraceEventId id
         )
      {

         switch( id )
         {                                                          
            case TraceEventId.Login:
            case TraceEventId.Logout:
            case TraceEventId.LoginFailed:
            case TraceEventId.AuditLoginGDR:
            case TraceEventId.AuditAddLoginToServer:  // To Server Role
            case TraceEventId.AuditAddLogin:
            case TraceEventId.AuditLoginChangePassword:
            case TraceEventId.AuditLoginChange:
            case TraceEventId.AuditChangeAudit:
            case TraceEventId.Exception:
               return true;

            case TraceEventId.AuditObjectGDR:               // 103
            case TraceEventId.AuditAddDbUser:               // 109
            case TraceEventId.AuditAddDropRole:             // 111
            case TraceEventId.AuditAddMember:               // 110
            case TraceEventId.AuditStatementGDR:            // 102
            case TraceEventId.AuditObjectDerivedPermission: // 118
            case TraceEventId.AuditStatementPermission:     // 113
            case TraceEventId.AuditBackupRestore:           // 115
            case TraceEventId.AuditDbcc:                    // 116
            case TraceEventId.AppRolePassChange:            // 112
               return false;

            default:
               throw new CoreException( String.Format( CoreConstants.Exception_Format_InvalidTraceEventID, id ) );
         }

      }

      #endregion
	}
}
