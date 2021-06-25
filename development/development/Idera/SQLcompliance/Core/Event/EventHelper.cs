using System;



namespace Idera.SQLcompliance.Core.Event
{
	/// <summary>
	/// Summary description for EvenHelper.
	/// </summary>
	public class EventHelper
	{
      #region Class constants

      public static int TraceColumnCount = 65;

      #region Static Configuration Settings
      public static TraceColumnId[] TraceCommonColumns = {
                                                            TraceColumnId.StartTime,
                                                            TraceColumnId.SQLSecurityLoginName,
                                                            TraceColumnId.DatabaseUserName,
                                                            TraceColumnId.Success,
                                                            TraceColumnId.SPID,
                                                            TraceColumnId.ObjectType,
                                                            TraceColumnId.ObjectName,
                                                            TraceColumnId.ApplicationName,
                                                            TraceColumnId.ClientHostName,
                                                            TraceColumnId.NestLevel,
                                                            TraceColumnId.EndTime
                                                         };

      public static TraceColumnId[] TraceCommon2005Columns = {
                                                                TraceColumnId.FileName,
                                                                TraceColumnId.LinkedServerName,
                                                                TraceColumnId.ParentName,
                                                                TraceColumnId.IsSystem,
                                                                TraceColumnId.SessionLoginName,
                                                                TraceColumnId.EventSequence
                                                             };

      public static TraceColumnId[] TraceServerColumns = {
                                                            TraceColumnId.ObjectID,
                                                            TraceColumnId.EventSubClass,
                                                            TraceColumnId.TargetLoginName,
                                                            TraceColumnId.TargetRoleName
                                                         };

      public static TraceColumnId[] TraceDBColumns = {
                                                        TraceColumnId.SQLSecurityLoginName,
                                                        TraceColumnId.DatabaseName,
                                                        TraceColumnId.DatabaseUserName,
                                                        TraceColumnId.Permissions,
                                                        TraceColumnId.ObjectID,
                                                        TraceColumnId.ObjectOwner,
                                                        TraceColumnId.EventSubClass,
                                                        TraceColumnId.TargetLoginName,
                                                        TraceColumnId.TargetRoleName
                                                     };

      #endregion

      #endregion

      #region Private Constructors

      private EventHelper(){}

      #endregion

      #region Public Static Methods
      //-------------------------------------------------------------------------------------
      // Validate event ID.  Throws an exception when the event ID is invalid
      //-------------------------------------------------------------------------------------
      public static void 
         ValidateEventId (
         TraceEventId eventId
         )
      {
         if( eventId < TraceEventId.RpcCompleted ||                  // The first valid event ID
            eventId > TraceEventId.AuditDatabaseObjectAccess )   // The last valid event ID
         {
            string msg = String.Format( "{0} is an invalid event ID", eventId );
            throw new InvalidEventIdException( msg );
         }
      }

      //-------------------------------------------------------------------------------------
      // Validate event ID.  This version returns false for invalid event IDs.
      //-------------------------------------------------------------------------------------
      public static bool 
         IsValidEventId (
         TraceEventId eventId
         )
      {
         if( eventId >= TraceEventId.RpcCompleted &&                  // The first valid event ID
            eventId <= TraceEventId.AuditObjectDerivedPermission )   // The last valid event ID
            return true;
         else
            return false;
      }

      //-------------------------------------------------------------------------------------
      // Validate trace column ID
      //-------------------------------------------------------------------------------------
      public static void 
         ValidateColumnId (
         TraceColumnId colId
         )
      {
         if (!IsValidColumnId( colId ))        // The last valid column ID
         {
         
            string msg = String.Format( "{0} is an invalid data column ID", colId );
            throw new InvalidColumnIdException( msg );
         }
      }

      //-------------------------------------------------------------------------------------
      // Validate trace column ID
      //-------------------------------------------------------------------------------------
      public static bool 
         IsValidColumnId (
         TraceColumnId colId
         )
      {
         if (colId < TraceColumnId.TextData ||                  // The first valid column ID
            colId > TraceColumnId.SessionLoginName )
         {
            if( colId != TraceColumnId.SQLcmTableName )
               return false;
         }
         return true;
      }

      //-------------------------------------------------------------------------------------
      // Validate comparison operator
      //-------------------------------------------------------------------------------------
      public static void
         ValidateComparisonOp (
         TraceFilterComparisonOp compOp
         )
      {
         if (!IsValidComparisonOp( compOp ))
         {
            string msg = String.Format( "{0} is an invalid comparison operator", compOp );
            throw new InvalidComparisonOpException( msg );
         }
      }

      //-------------------------------------------------------------------------------------
      // Validate comparison operator
      //-------------------------------------------------------------------------------------
      public static bool
         IsValidComparisonOp (
         TraceFilterComparisonOp compOp
         )
      {
         if (compOp < TraceFilterComparisonOp.Equal ||
            compOp > TraceFilterComparisonOp.NotLike)
            return false;
         return true;
      }

      //-------------------------------------------------------------------------------------
      // Validate comparison operator and the type of the column it is applying to
      //-------------------------------------------------------------------------------------
      public static void
         ValidateComparisonOp (
         TraceFilterComparisonOp   compOp,
         TraceFilterType           type
         )
      {
         if( compOp < TraceFilterComparisonOp.Equal ||
            compOp > TraceFilterComparisonOp.NotLike )
         {
            string msg = String.Format( "{0} is an invalid comparison operator", compOp );
            throw new InvalidComparisonOpException( msg );
         }

         if( type != TraceFilterType.nvarchar &&
            type != TraceFilterType.integer )
         {
            string msg = String.Format( "Filter type {0} is invalid or not supported", type );
            throw new InvalidFilterTypeException( msg );
         }

         if( type == TraceFilterType.integer && 
            ( compOp != TraceFilterComparisonOp.Equal &&
            compOp != TraceFilterComparisonOp.GreaterThan &&
            compOp != TraceFilterComparisonOp.GreaterThanOrEqual &&
            compOp != TraceFilterComparisonOp.LessThan &&
            compOp != TraceFilterComparisonOp.LessThanOrEqual &&
            compOp != TraceFilterComparisonOp.NotEqual ) )
         {
            string msg = String.Format( "Comparison operator '{0}' cannot be used to set integer type filter", 
               compOp.ToString() );
            throw new IncompetibleComparisonOpException( msg );
         }
         else if( type == TraceFilterType.nvarchar &&
            ( compOp != TraceFilterComparisonOp.Like &&
            compOp != TraceFilterComparisonOp.NotLike ))
         {
            string msg = String.Format( "Comparison operator '{0}' cannot be used to set string type filter", 
               compOp.ToString() );
            throw new IncompetibleComparisonOpException( msg );
         }

      }

      //-------------------------------------------------------------------------------------
      // Validate comparison operator and the type of the column it is applying to
      //-------------------------------------------------------------------------------------
      public static bool
         IsValidComparisonOp (
         TraceFilterComparisonOp   compOp,
         TraceFilterType           type
         )
      {
         if( compOp < TraceFilterComparisonOp.Equal ||
            compOp > TraceFilterComparisonOp.NotLike )
            return false;

         if( type != TraceFilterType.nvarchar &&
            type != TraceFilterType.integer )
            return false;

         if( type == TraceFilterType.integer && 
            ( compOp == TraceFilterComparisonOp.Equal ||
            compOp == TraceFilterComparisonOp.GreaterThan ||
            compOp == TraceFilterComparisonOp.GreaterThanOrEqual ||
            compOp == TraceFilterComparisonOp.LessThan ||
            compOp == TraceFilterComparisonOp.LessThanOrEqual ||
            compOp == TraceFilterComparisonOp.NotEqual ) )
            return true;
         else if( type == TraceFilterType.nvarchar &&
            ( compOp == TraceFilterComparisonOp.Like ||
            compOp == TraceFilterComparisonOp.NotLike ))
            return true;

         return false;
      }


      //-------------------------------------------------------------------------------------
      // Validate Logical operator
      //-------------------------------------------------------------------------------------
      public static void
         ValidateLogicalOp (
         TraceFilterLogicalOp logicalOp
         )
      {
         if( logicalOp != TraceFilterLogicalOp.AND &&
            logicalOp != TraceFilterLogicalOp.OR )
         {
            string msg = String.Format( "Logical operator {0} is invalid or not supported", logicalOp );
            throw new InvalidLogicalOpException( msg );
         }
      }

      //-------------------------------------------------------------------------------------
      // Validate Logical operator
      //-------------------------------------------------------------------------------------
      public static bool
         IsValidLogicalOp (
         TraceFilterLogicalOp logicalOp
         )
      {
         if( logicalOp == TraceFilterLogicalOp.AND ||
            logicalOp == TraceFilterLogicalOp.OR )
            return true;
         else
            return false;
      }

      //-------------------------------------------------------------------------------------
      // Returns column type
      //-------------------------------------------------------------------------------------
      public static TraceFilterType
         GetColumnType (
         TraceColumnId colId
         )
      {
         switch( colId )
         {
            case  TraceColumnId.TextData:
            case  TraceColumnId.BinaryData:
            case  TraceColumnId.NTUserName: 
            case  TraceColumnId.NTDomainName: 
            case  TraceColumnId.ClientHostName:
            case  TraceColumnId.ApplicationName:
            case  TraceColumnId.SQLSecurityLoginName:
            case  TraceColumnId.StartTime:
            case  TraceColumnId.EndTime:
            case  TraceColumnId.ServerName:
            case  TraceColumnId.ObjectName:
            case  TraceColumnId.DatabaseName:
            case  TraceColumnId.FileName:
            case  TraceColumnId.ObjectOwner:
            case  TraceColumnId.TargetRoleName:
            case  TraceColumnId.TargetUserName:
            case  TraceColumnId.DatabaseUserName:
            case  TraceColumnId.TargetLoginName:
            case  TraceColumnId.ParentName:
            case TraceColumnId.SessionLoginName:
            case TraceColumnId.SQLcmTableName:
               return TraceFilterType.nvarchar;

            case  TraceColumnId.ColumnPermissionsSet:
            case  TraceColumnId.LoginSID:
            case  TraceColumnId.TargetLoginSID:
            case  TraceColumnId.EventClass:
            case  TraceColumnId.ObjectType:
            case  TraceColumnId.NestLevel:
            case  TraceColumnId.State:
            case  TraceColumnId.Error:
            case  TraceColumnId.Mode:
            case  TraceColumnId.Handle:
            case  TraceColumnId.Reads:
            case  TraceColumnId.Writes:
            case  TraceColumnId.CPU :
            case  TraceColumnId.Permissions:
            case  TraceColumnId.Severity:
            case  TraceColumnId.EventSubClass:
            case  TraceColumnId.ObjectID:
            case  TraceColumnId.Success:
            case  TraceColumnId.IndexID:
            case  TraceColumnId.IntegerData:
            case  TraceColumnId.DatabaseID:
            case  TraceColumnId.TransactionID: 
            case  TraceColumnId.ClientProcessID:
            case  TraceColumnId.SPID:
            case  TraceColumnId.Duration:
            case  TraceColumnId.IsSystem:
            case  TraceColumnId.RowCounts:
               return TraceFilterType.integer;

            case  TraceColumnId.Reserved:  
            default:
               return TraceFilterType.Unknown;
         }
      }


      #endregion
	}
}
