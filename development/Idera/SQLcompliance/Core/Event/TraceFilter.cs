using System;
using System.Diagnostics;
using System.Collections;
using System.Data;
using System.Text;

using Idera.SQLcompliance.Core.TraceProcessing;

namespace Idera.SQLcompliance.Core.Event
{


	/// <summary>
	/// Summary description for TraceFilter.
	/// </summary>
   //-----------------------------------------------------------------------
   //  TraceFilter class keeps trace filter settings and validates them.
   //-----------------------------------------------------------------------
   public class TraceFilter
   {
      public class FilterComparer : IComparer
      {
         public int Compare( object x, object y)
         {
            int rc = 0;
            try
            {
               rc = Comparer.DefaultInvariant.Compare( (int)(((TraceFilter)x).ColumnId), 
                                                         (int)(((TraceFilter)y).ColumnId) );
            }
            catch( Exception e )
            {
               ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                       "An error occurred comparing two TraceFilers.",
                                       e,
                                       true );
            }
            return rc;
         }
      }

      #region Private data members
      private TraceColumnId            columnId;
      private TraceFilterComparisonOp  compareOperator;
      private TraceFilterLogicalOp     logicalOperator;
      private TraceFilterType          type;
      private string                   stringValue;
      private int                      intValue;
      private string                   auditLogColumnId;
      private string                   auditLogCompareOperator;
      private string                   auditLogLogicOperator;
      #endregion

      #region Properties
      //-----------------------------------------------------------------------
      // Make these properties read-only so that the caller don't have a chance
      // to set incompetible comparison operators and columns.
      //-----------------------------------------------------------------------
      public TraceColumnId ColumnId
      {
         get { return columnId; }
      }

      public TraceFilterComparisonOp CompareOp
      {
         get { return compareOperator; }
      }

      public TraceFilterLogicalOp LogicalOp
      {
         get { return logicalOperator; }
      }

      public TraceFilterType Type
      {
         get { return type; }
      }

      public string AuditLogCompareOperator
      {
          get { return auditLogCompareOperator; }
      }

      public string AuditLogColumnId
      {
          get { return auditLogColumnId; }
      }

      public string AuditLogLogicOperator
      {
          get { return auditLogLogicOperator; }
      }
      #endregion

      #region Constructors

      public TraceFilter ()
      {
         // Initialize these to invalid values so we can check for errors later
         columnId = TraceColumnId.Unknown;
         compareOperator = TraceFilterComparisonOp.Unknown;
         logicalOperator = TraceFilterLogicalOp.Unknown;
         type = TraceFilterType.Unknown;
      }

      // For string type filters
      public TraceFilter (
         TraceColumnId              colid,
         TraceFilterComparisonOp    compOp,
         string                     value,
         TraceFilterLogicalOp       logicalOp
         ): base()
      {
         SetFilter( colid,
                    compOp,
                    value,
                    logicalOp );
      }

      // For integer type filters
      public TraceFilter (
         TraceColumnId                  colid,
         TraceFilterComparisonOp        compOp,
         int                            value,
         TraceFilterLogicalOp           logicalOp
         ) : base()
      {
         SetFilter( colid,
            compOp,
            value,
            logicalOp );
      }

      // For Audit Logs filters
      public TraceFilter(
         string colid,
         string compOp,
         int value,
         string logicalOp
         )
          : base()
      {
          type = TraceFilterType.integer;
          SetFilter(colid,
             compOp,
             value,
             logicalOp);
      }

      // For Audit Logs filters
      public TraceFilter(
         string colid,
         string compOp,
         string value,
         string logicalOp
         )
          : base()
      {
          type = TraceFilterType.nvarchar;
          SetFilter(colid,
             compOp,
             value,
             logicalOp);
      }

      //-----------------------------------------------------------------------
      // Set integer column filter for audit logs
      //-----------------------------------------------------------------------
      public void
         SetFilter(
            string colId,
            string compOp,
            int value,
            string logicalOp
         )
      {
          auditLogColumnId = colId;
          auditLogCompareOperator = compOp;
          auditLogLogicOperator = logicalOp;
          intValue = value;

      }
       //-----------------------------------------------------------------------
      // Set integer column filter for audit logs
      //-----------------------------------------------------------------------
      public void
         SetFilter(
            string colId,
            string compOp,
            string value,
            string logicalOp
         )
      {
          auditLogColumnId = colId;
          auditLogCompareOperator = compOp;
          auditLogLogicOperator = logicalOp;
          stringValue = value;

      }
      #endregion

      #region Private Static Methods
      private static void
         ValidateFilter (
            TraceColumnId             colid,
            TraceFilterComparisonOp   compOp,
            TraceFilterLogicalOp      logicalOp
         )
      {

         // Note that all these validation methods throws exceptions if
         // there is any invalid value.
         EventHelper.ValidateColumnId( colid );
         EventHelper.ValidateComparisonOp( compOp, 
                                               EventHelper.GetColumnType( colid ) );
         EventHelper.ValidateLogicalOp( logicalOp );
      }   
      
      #endregion

      #region Public Methods
      //-----------------------------------------------------------------------
      // Set string column filter
      //-----------------------------------------------------------------------
      public void 
         SetFilter (
            TraceColumnId             colId,
            TraceFilterComparisonOp   compOp,
            string                    value,
            TraceFilterLogicalOp      logicalOp
         )
      {
         ValidateFilter( colId,
                         compOp,
                         logicalOp );

         type = EventHelper.GetColumnType ( colId );
         columnId = colId;
         compareOperator = compOp;
         logicalOperator = logicalOp;
         switch( (int)colId )
         {
            case (int)TraceColumnId.ObjectName:
            case (int)TraceColumnId.DatabaseName:
               stringValue = value;
               break;
            default:
               stringValue = value;
               break;
         }
      }

      //-----------------------------------------------------------------------
      // Set integer column filter
      //-----------------------------------------------------------------------
      public void 
         SetFilter (
            TraceColumnId            colId,
            TraceFilterComparisonOp  compOp,
            int                      value,
            TraceFilterLogicalOp     logicalOp
         )
      {
         ValidateFilter( colId,
                         compOp,
                         logicalOp );

         type = EventHelper.GetColumnType ( colId );
         columnId = colId;
         compareOperator = compOp;
         logicalOperator = logicalOp;
         intValue = value;

      }

      //-----------------------------------------------------------------------
      //  Get filter value
      //-----------------------------------------------------------------------
      public object GetValue()
      {
         if( type == TraceFilterType.nvarchar )
            return stringValue;
         else if( type == TraceFilterType.integer )
            return intValue;
         else
         {
            string msg = String.Format( "Filter type {0} is unknown or not supported",
                                        type );
            throw new InvalidFilterTypeException( msg );
         }
      }

      //-----------------------------------------------------------------------
      // Get filter integer value
      //-----------------------------------------------------------------------
      public int GetIntValue()
      {
         if( type != TraceFilterType.integer )
         {
            string msg = String.Format( "A type {0} value cannot be converted to an integer value",
                                        type );
            throw new InvalidFilterTypeException( msg );
         }
         return intValue;
      }

      //-----------------------------------------------------------------------
      // Get filter integer value
      //-----------------------------------------------------------------------
      public string GetTextValue()
      {
         if( type != TraceFilterType.nvarchar )
         {
            string msg = String.Format( "A type {0} value cannot be converted to an integer value",
               type );
            throw new InvalidFilterTypeException( msg );
         }
         return stringValue;
      }

      //----------------------------------------------------------
      // IsFilteredOut
      //----------------------------------------------------------
      public bool
         Equals (
         TraceFilter filter
         )
      {
         if( filter == null )
            return false;

         if( this.columnId == filter.ColumnId &&
             this.type == filter.Type &&
             this.compareOperator == filter.CompareOp )
             //this.logicalOperator == filter.LogicalOp )
         {
            if( type == TraceFilterType.integer )
               return this.GetIntValue() == filter.GetIntValue();
            else 
            {
               string v1 = this.GetTextValue();
               string v2 = filter.GetTextValue();
               return String.Equals( v1, v2 );
            }
         }

         return false;
      }
	   
      public static bool
         Equals (
         TraceFilter f1,
         TraceFilter f2
         )
      {
         if( f1 == null )
         {
            if( f2 == null )
               return true;
            else 
               return false;
         }
         else if( f2 == null )
         {
            return false;
         }

         if( f1.columnId == f2.ColumnId &&
             f1.type == f2.Type &&
             f1.compareOperator == f2.CompareOp &&
             f1.compareOperator == f2.CompareOp )
         {
            if( f1.Type == TraceFilterType.integer )
               return f1.GetValue() == f2.GetValue();
            else 
            {
               string v1 = f1.GetTextValue();
               string v2 = f2.GetTextValue();
               return String.Equals( v1, v2 );
            }
         }

         return false;
      }

      //----------------------------------------------------------
      // IsFilteredOut
      //----------------------------------------------------------
      public bool
         IsFilteredOut( int checkedValue )
      {
         switch( compareOperator )
         {
            case TraceFilterComparisonOp.Equal:
               return checkedValue != intValue;
            case TraceFilterComparisonOp.GreaterThanOrEqual:
               return checkedValue < intValue;
            case TraceFilterComparisonOp.GreaterThan:
               return checkedValue <= intValue;
            case TraceFilterComparisonOp.LessThanOrEqual:
               return checkedValue > intValue;
            case TraceFilterComparisonOp.LessThan:
               return checkedValue >= intValue;
            case TraceFilterComparisonOp.NotEqual:
               return checkedValue == intValue;
         }

         return false;
      }
 
      //----------------------------------------------------------
      // IsFilteredOut
      //----------------------------------------------------------
      public bool
         IsFilteredOut( string checkedValue )
      {
         switch( (int)compareOperator )
         {
            case (int)TraceFilterComparisonOp.Like:
               return !checkedValue.ToUpper().StartsWith( stringValue );
            case (int)TraceFilterComparisonOp.NotLike:
               return checkedValue.ToUpper().StartsWith( stringValue );
         }

         return false;
      }

      //----------------------------------------------------------
      // IsFilteredOut
      //----------------------------------------------------------
      public bool
         IsFilteredOut( object checkedValue )
      {
         if( checkedValue is int )
            return IsFilteredOut( (int)checkedValue );
         else if( checkedValue is string )
            return IsFilteredOut( (string)checkedValue );
         return false;
      }

      //----------------------------------------------------------
      // Apply
      //----------------------------------------------------------
      public bool
         Apply( TraceEventId eventId, object checkedValue )
      {
         if( !IsApplicableFilter( eventId ) )
            return true;

         if( checkedValue is int )
            return Apply( (int)checkedValue );
         else if( checkedValue is System.Int64 )
            return Apply( (System.Int64)checkedValue );
         else if( checkedValue is string )
         {
            if( columnId == TraceColumnId.ObjectName &&
                (string)checkedValue == "" )
               return true;
            return Apply( (string)checkedValue );
         }
         else // null and unknow filter type handling
            //return Apply( checkedValue );
            return true;

      }

      //----------------------------------------------------------
      // IsFilteredOut
      //----------------------------------------------------------
      public bool
         Apply( int checkedValue )
      {
         switch( compareOperator )
         {
            case TraceFilterComparisonOp.Equal:
               return checkedValue == intValue;
            case TraceFilterComparisonOp.GreaterThanOrEqual:
               return checkedValue >= intValue;
            case TraceFilterComparisonOp.GreaterThan:
               return checkedValue > intValue;
            case TraceFilterComparisonOp.LessThanOrEqual:
               return checkedValue <= intValue;
            case TraceFilterComparisonOp.LessThan:
               return checkedValue < intValue;
            case TraceFilterComparisonOp.NotEqual:
               return checkedValue != intValue;
         }

         return true;
      }

      //-------------------------------------------------------------------
      // Apply - This function is added for SQL Server 2005.  Permissions
      //         column is changed to type BigInt.  This is the only
      //         column with BigInt.  Agent and server still treats this
      //         column as Int32 at this time.
      //-------------------------------------------------------------------
      public bool
         Apply( System.Int64 checkedValue )
      {
         switch( compareOperator )
         {
            case TraceFilterComparisonOp.Equal:
                 return checkedValue == intValue;  // Note: we don't have Int64 filters at this time
            case TraceFilterComparisonOp.GreaterThanOrEqual:
               return checkedValue >= intValue;
            case TraceFilterComparisonOp.GreaterThan:
               return checkedValue > intValue;
            case TraceFilterComparisonOp.LessThanOrEqual:
               return checkedValue <= intValue;
            case TraceFilterComparisonOp.LessThan:
               return checkedValue < intValue;
            case TraceFilterComparisonOp.NotEqual:
               return checkedValue != intValue;
         }

         return true;
      }
      
      //----------------------------------------------------------
      // Apply
      //----------------------------------------------------------
      public bool
         Apply( string checkedValue )
      {
         switch( compareOperator )
         {
            case TraceFilterComparisonOp.Like:
               switch( (int)columnId )
               {
                  case (int)TraceColumnId.ObjectName:
                  case (int)TraceColumnId.SQLcmTableName:
                     return checkedValue == stringValue;
                  default:
                     return checkedValue.ToUpper().StartsWith( stringValue.ToUpper() );
               }
            case TraceFilterComparisonOp.NotLike:
               switch( (int)columnId )
               {
                  case (int)TraceColumnId.ObjectName:
                  case (int)TraceColumnId.SQLcmTableName:
                     return checkedValue != stringValue;
                  default:
                     return !checkedValue.ToUpper().StartsWith( stringValue.ToUpper() );
               }
         }

         return true;
      }

      public bool
         Apply( object checkedValue )
      {
         switch( compareOperator )
         {
            case TraceFilterComparisonOp.Equal:
               return false;  // Note: we don't have Int64 filters at this time
            case TraceFilterComparisonOp.GreaterThanOrEqual:
               return false;
            case TraceFilterComparisonOp.GreaterThan:
               return false;
            case TraceFilterComparisonOp.LessThanOrEqual:
               return false;
            case TraceFilterComparisonOp.LessThan:
               return false;
            case TraceFilterComparisonOp.NotEqual:
               return true;
            case TraceFilterComparisonOp.Like:
               return false;
            case TraceFilterComparisonOp.NotLike:
               return true;
         }

         return true;
      }
      
      //----------------------------------------------------------
      // CreateFilterSets
      //----------------------------------------------------------
      public static ArrayList
         CreateFilterSets (
         TraceFilter [] filters )
      {
         if( filters == null || filters.Length == 0 )
            return null;

         Hashtable tmpSets = new Hashtable();
         ArrayList filterList;

         for( int i = 0; i < filters.Length; i++ )
         {
            if( !tmpSets.ContainsKey( filters[i].ColumnId ))
            {
               filterList = new ArrayList();
               filterList.Add( filters[i] );
               tmpSets.Add( filters[i].ColumnId, filterList );
            }
            else
            {
               filterList = (ArrayList)tmpSets[filters[i].ColumnId];
               filterList.Add( filters[i] );
            }
         }

         ArrayList finalSets = new ArrayList();

         IDictionaryEnumerator enumerator = tmpSets.GetEnumerator();

         TraceFilter [] tmpArray;
         while( enumerator.MoveNext() )
         {
            tmpArray = (TraceFilter [])((ArrayList)(enumerator.Value)).ToArray(typeof( TraceFilter ));
            finalSets.Add( tmpArray );
         }
         return finalSets;
      }


      //-----------------------------------------------------------------------
      // Create sp_trace_setfilter call for the SQLsecure MOASP stored procedure
      // TODO: Move this to stored procedure builder?
      // TODO: Provide a static version so it can be used without instantiating
      //       a new instance.
      //-----------------------------------------------------------------------
      public string CreateSPStatement( )
      {
         // check for invalid filter settings
         ValidateFilter( columnId,
                         compareOperator,
                         logicalOperator );

         string statement = null;

         if( type == TraceFilterType.nvarchar )
         {
            statement = String.Format( 
               "EXEC sp_trace_setfilter @traceId, {1}, {2}, {3}, {4}" ,
               columnId,
               logicalOperator,
               compareOperator,
               "'" + stringValue + "'" 
               );
         }
         else if ( type == TraceFilterType.integer )
         {
            statement = String.Format( 
               "EXEC sp_trace_setfilter @traceId, {1}, {2}, {3}, {4}" ,
               columnId,
               logicalOperator,
               compareOperator,
               intValue 
               );
         }
         else
         {
            // throw an exception to tell the type is not supported or not implemented
            string msg = String.Format( "Filter type {0} is not supported by this method",type );
            throw new InvalidFilterTypeException( msg );
         }

         return statement;
        
      }

      //-----------------------------------------------------------------------
      // Get filter integer value of Text
      //-----------------------------------------------------------------------
      /// <summary>
      /// This method will return integer value of action_id
      /// 5.5 Audit Logs
      /// </summary>
      /// <returns></returns>
      public int GetIntValueOfActionId()
      {
          if (type != TraceFilterType.nvarchar)
          {
              string msg = String.Format("A type {0} value cannot be converted to an nvarchar value",
                                          type);
              throw new InvalidFilterTypeException(msg);
          }
          Char[] charArr = stringValue.ToUpper().ToCharArray();
          int intOfText = 0;
          int charCount = charArr.Length;
          for (int i = 0; i < charCount && i < 4; i++)
          {
              intOfText = Convert.ToInt32(charArr[i]) * (int)Math.Pow(2, 8 * i) + intOfText;
          }

          while (charCount * 8 <= 24)
          {
              intOfText = Convert.ToInt32(' ') * (int)Math.Pow(2, charCount * 8) + intOfText;
              charCount++;
          }
          return intOfText;
      }      

      #region Event and Filter Matching

      //----------------------------------------------------------
      // IsApplicableFilter
      //----------------------------------------------------------
      public bool 
         IsApplicableFilter (
            TraceEventId eventId )
      {
         bool isApplicable = false;

         switch( (int)eventId )
         {
               // Login Events
            case (int)TraceEventId.Login:
               isApplicable = isLoginEventFilter(  );          
               break;

            case (int)TraceEventId.Logout:
               isApplicable = isLogoutEventFilter();
               break;

            case (int)TraceEventId.LoginFailed:   
               isApplicable = isLoginFailedEventFilter();    
               break;
	         
               // DDL
            case (int)TraceEventId.AuditObjectDerivedPermission:
               isApplicable = isAuditObjectDerivedPermissionEventFilter();
               break;
            case (int)TraceEventId.AuditStatementPermission:
               isApplicable = isAuditStatementPermissionEventFilter();
               break;
            case (int)TraceEventId.ObjectCreated:
               isApplicable = isObjectCreatedEventFilter();
               break;
            case (int)TraceEventId.AuditDatabaseManagement:
               isApplicable = isAuditDatabaseManagementEventFilter();
               break;
            case (int)TraceEventId.AuditDatabaseObjectManagement:
               isApplicable = isAuditDatabaseObjectManagementEventFilter();
               break;
            case (int)TraceEventId.AuditSchemaObjectManagement:
               isApplicable = isAuditSchemaObjectManagementEventFilter();
               break;
            case (int)TraceEventId.AuditServerObjectManagement:
               isApplicable = isAuditServerObjectManagementEventFilter();
               break;
	            
               // Security Events
            case (int)TraceEventId.AuditObjectGDR:
               isApplicable = isAuditObjectGdrEventFilter();
               break;
            case (int)TraceEventId.AuditStatementGDR:
               isApplicable = isAuditStatementGdrEventFilter();
               break;
            case (int)TraceEventId.AuditLoginGDR:
               isApplicable = isAuditLoginGdrEventFilter();
               break;
            case (int)TraceEventId.AuditLoginChange:
               isApplicable = isAuditLoginChangeEventFilter();
               break;
            case (int)TraceEventId.AuditLoginChangePassword:
               isApplicable = isAuditLoginChangePasswordEventFilter();
               break;
            case (int)TraceEventId.AuditAddLogin:
               isApplicable = isAuditAddLoginEventFilter();
               break;
            case (int)TraceEventId.AuditAddLoginToServer:
               isApplicable = isAuditAddLoginToServerEventFilter();
               break;
            case (int)TraceEventId.AuditAddDbUser:
               isApplicable = isAuditAddDbUserEventFilter();
               break;
            case (int)TraceEventId.AuditAddMember:
               isApplicable = isAuditAddMemberEventFilter();
               break;
            case (int)TraceEventId.AuditAddDropRole:
               isApplicable = isAuditAddRoleEventFilter();
               break;
            case (int)TraceEventId.AppRolePassChange:
               isApplicable = isAppRolePassChangeEventFilter();
               break;
            case (int)TraceEventId.AuditDatabasePrincipalManagement:
               isApplicable = isAuditDatabasePrincipalManagementEventFilter();
               break;
            case (int)TraceEventId.AuditServerPrincipalImpersonation:
               isApplicable = isAuditServerPrincipalImpersonationEventFilter();
               break;
            case (int)TraceEventId.AuditDatabasePrincipalImpersonation:
               isApplicable = isAuditDatabasePrincipalImpersonationEventFilter();
               break;
            case (int)TraceEventId.AuditServerObjectTakeOwnership:
               isApplicable = isAuditServerObjectTakeOwnershipEventFilter();
               break;
            case (int)TraceEventId.AuditDatabaseObjectTakeOwnership:
               isApplicable = isAuditDatabaseObjectTakeOwnershipEventFilter();
               break;
            case (int)TraceEventId.AuditChangeDatabaseOwner:
               isApplicable = isAuditChangeDatabaseOwnerEventFilter();
               break;
            case (int)TraceEventId.AuditSchemaObjectTakeOwnership:
               isApplicable = isAuditSchemaObjectTakeOwnershipEventFilter();
               break;
            case (int)TraceEventId.AuditServerScopeGDR:
               isApplicable = isAuditServerScopeGdrEventFilter();
               break;
            case (int)TraceEventId.AuditServerObjectGDR:
               isApplicable = isAuditServerObjectGdrEventFilter();
               break;
            case (int)TraceEventId.AuditDatabaseObjectGDR:
               isApplicable = isAuditDatabaseObjectGdrEventFilter();
               break;
            case (int)TraceEventId.AuditServerPrincipalManagement:
               isApplicable = isAuditServerPrincipalManagementEventFilter();
               break;

            /*
            // Broker
            case (int)TraceEventId.AuditBrokerConversation:
               isApplicable = isAuditBrokerConversationEventFilter();
               break;
            case (int)TraceEventId.AuditBrokerLogin:
               isApplicable = isAuditBrokerLoginEventFilter();
               break;
            */

            // Admin
            case (int)TraceEventId.AuditServerOperation:
               isApplicable = isAuditServerOperationEventFilter();
               break;
            case (int)TraceEventId.AuditBackupRestore:
               isApplicable = isAuditBackupRestoreEventFilter();
               break;
            case (int)TraceEventId.AuditDbcc:
               isApplicable = isAuditDbccEventFilter();
               break;
            case (int)TraceEventId.AuditChangeAudit:
               isApplicable = isAuditChangeAuditEventFilter();
               break;
            case (int)TraceEventId.AuditServerAlterTrace:
               isApplicable = isAuditServerAlterTraceEventFilter();
               break;
            case (int)TraceEventId.AuditDatabaseOperation:
               isApplicable = isAuditDatabaseOperationEventFilter();
               break;

	            
               // DML    
            case (int)TraceEventId.AuditObjectPermission:
               isApplicable = isAuditObjectPermissionEventFilter();
               break;
            case (int)TraceEventId.AuditDatabaseObjectAccess:
               isApplicable = isAuditDatabaseObjectAccessEventFilter();
               break;
            case (int)TraceEventId.Transaction:
               isApplicable = isAuditTransactionEventFilter();
               break;
               // Other   
            case (int)TraceEventId.SqlStmtCompleted:
               isApplicable = isSqlStmtCompletedEventFilter();
               break;
            case (int)TraceEventId.SqlStmtStarting:
               isApplicable = isSqlStmtStartingEventFilter();
               break;
            default:
               isApplicable = false;
               break;
         }

         return isApplicable;
      }

      //---------------------------------------------------------------------------------
      // RowMatchesEventFilter - Does the current row deserver to be collected?
      //
      // Note: Filtering of SQLsecure internal actions is done earlier during
      //       during row processing (See IsSQLsecureAction() for details )
      //---------------------------------------------------------------------------------
      internal static bool
         RowMatchesEventFilter( TraceJob job, DataRow row )
      {
           
            TraceJobInfo jobInfo    = job.jobInfo;
         int          eventClass = job.GetRowInt32( row, TraceJob.ndxEventClass );
         int          type       = jobInfo.traceType;
            
            if ( jobInfo.traceEvents.Count == 0 )
            return true;

         if( !jobInfo.traceEvents.Contains( eventClass ) )
         {
            if( CoreConstants.LogFilteredOutEvents )
               ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                       String.Format( "Event {1} is filtered out. \nTrace type {0}: Event {1} is not being audited.",
                                                      type,
                                                      eventClass ));
            return false;
         }

         type %= 10;




            if (!jobInfo.privilegedUserTrace) // not privileged user trace
            {
                if (!job.IsDatabaseEvents(row))
                {
                    if (CoreConstants.LogFilteredOutEvents)
                        ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                String.Format("Event {1} is filtered out. \nTrace type {0}: Event {1} will be captured in the previleged user trace.",
                                                               type,
                                                               eventClass));
                    return false;
                }

            }
            else if (!job.IsServerPrivEvent(row))// Check priv user trace filters
            {
                if (CoreConstants.LogFilteredOutEvents)
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                            String.Format("Event {1} is filtered out. \nTrace type {0}: Event {1} is filtered out from priv user trace.",
                                                           type,
                                                           eventClass));
                return false;
            }

         if( ( jobInfo.filterSets == null ||
               jobInfo.filterSets.Count == 0 ) &&
             ( eventClass != (int)TraceEventId.AuditObjectDerivedPermission &&
               eventClass != (int)TraceEventId.AuditStatementPermission ))
            return true;

         bool matched = false;
         try
         {
            if( jobInfo.filterSets != null )
            {
               foreach( TraceFilter [] filters in jobInfo.filterSets )
               {
                  matched = false;
                  for( int i = 0; i < filters.Length; i++ )
                  {
                     if (filters[i].columnId == TraceColumnId.Success && jobInfo.traceFile.ToLower().EndsWith(".xel"))
                     {
                        //Skiping access check filter because Success column is not available with Extended Events
                        matched = true;
                        break;
                     }
                     if (eventClass == (int)TraceEventId.AuditObjectPermission)
                     {                         
                        if (filters[i].ColumnId == TraceColumnId.SQLcmTableName && job.GetRowPermissions(row, TraceJob.ndxPermissions) == (int)TracePermissions.Execute)
                        {
                           //the object name for execute is the stored proc, It should not be compared
                           //against the table name.
                           matched = true;
                           break;
                        }
                     }

                     if (filters[i].Apply((TraceEventId)eventClass, 
                        GetFilteredValue( filters[i].ColumnId, job, row, eventClass ) ) )
                     { 
                        matched = true;
                        // No need to check logical operator.  The filters grouped together
                        // should always ORed together.
                        break;
                     }
                     else
                     {
                        // Check exclusion filters
                        if( filters[i].CompareOp == TraceFilterComparisonOp.NotEqual ||
                           filters[i].CompareOp == TraceFilterComparisonOp.NotLike )
                        {
                           if( CoreConstants.LogFilteredOutEvents )
                              ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                                      String.Format( "Event {1} is filtered out by exclusion filter. \nTrace type {0}",
                                                                     type,
                                                                     eventClass ));

                           return false;
                        }
                     }    
                  }

                  if( !matched )
                  {
                     if( CoreConstants.LogFilteredOutEvents )
                        ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                                String.Format( "Event {1} does not match column filter {2}. \nTrace type {0}",
                                                               type,
                                                               eventClass,
                                                               (int)filters[0].ColumnId));
                     break;
                  }
               }
            }
            else
            {
               // This is a special case to cover 113 and 118 in server traces.
               matched = true;
            }
         }
         catch( Exception e )
         {
            // TODO: ignore all the exceptions.  This is for testing only.
            ErrorLog.Instance.Write( ErrorLog.Level.UltraDebug,
                                     "Error matching an event with filters.",
                                     e,
                                     true );
            matched = true;
         }
         if( matched )
         {
             if(!jobInfo.traceFile.Contains("xel"))
            matched = CheckFinalFilter( type, eventClass, job, row );
            if( !matched && CoreConstants.LogFilteredOutEvents)
               ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                       String.Format( "Event {1} is filtered out by final filter. \nTrace type {0}",
                                                      type,
                                                      eventClass ));

         }

         return matched;
      }

      //---------------------------------------------------------------------------------
      // GetFilteredValue - Get the value of the specified column ID.  
      //---------------------------------------------------------------------------------
      private static object
         GetFilteredValue( 
            TraceColumnId columnId,
            TraceJob      job,
            DataRow       row,
            int           eventClass
         )
      {
         string stringValue = "";
         int    intValue    = 0;
         System.Int64 int64Value = 0;

         switch( columnId )
         {
            case TraceColumnId.ObjectName:
               if( DBObject.IsUserTable(job.GetRowInt32( row, TraceJob.ndxObjectType )) )
                  stringValue = job.GetRowString( row, TraceJob.ndxObjectName );
               if( stringValue == "" )
                  break;
               return stringValue;
            case TraceColumnId.ApplicationName:
               stringValue = job.GetRowString( row, TraceJob.ndxApplicationName );
               if( stringValue == "" )
                  break;
               return stringValue;
            case TraceColumnId.ObjectType:
               intValue = job.GetRowInt32( row, TraceJob.ndxObjectType );
               if( intValue == 0 )
                  break;
               return intValue;
            case TraceColumnId.DatabaseID:
               intValue = job.GetRowInt32( row, TraceJob.ndxDatabaseId );
               if( intValue == 0 )
                  break;
               return intValue;
            case TraceColumnId.ObjectID:
               intValue = job.GetRowInt32( row, TraceJob.ndxObjectId );
               if( intValue == 0 )
                  break;
               return intValue;
            case TraceColumnId.Permissions:
               if ( job.jobInfo.isSqlServer2005 )
               {
                  int64Value = job.GetRowInt64( row, TraceJob.ndxPermissions );
                  if( int64Value == 0 ) break;
                  return int64Value;
               }
               else
               {
                  intValue = job.GetRowInt32( row, TraceJob.ndxPermissions );
                  if( intValue == 0 )
                     break;
                  return intValue;
               }
            case TraceColumnId.TargetLoginName:
               stringValue = job.GetRowString( row, TraceJob.ndxTargetLoginName );
               if( stringValue == "" )
                  break;
               return stringValue;
            case TraceColumnId.ObjectOwner:
               stringValue = job.GetRowString( row, TraceJob.ndxOwnerName );
               if( stringValue == "" )
                  break;
               return stringValue;
            case TraceColumnId.EventSubClass:
               intValue = job.GetRowInt32( row, TraceJob.ndxEventSubclass );
               if( intValue == 0 )
                  break;
               return intValue;
            case TraceColumnId.Success:
               intValue = job.GetRowInt32( row, TraceJob.ndxSuccess );
               if( intValue == 0 && eventClass == (int)TraceEventId.LoginFailed )
                  return 1;
               return intValue;
            case TraceColumnId.DatabaseName:
               stringValue = job.GetRowString( row, TraceJob.ndxDatabaseName );
               if( stringValue == "" )
                  break;
               return stringValue;
            case TraceColumnId.ClientHostName:
               stringValue = job.GetRowString( row, TraceJob.ndxHostName );
               if( stringValue == "" )
                  break;
               return stringValue;
            case TraceColumnId.ServerName:
               stringValue = job.GetRowString( row, TraceJob.ndxServerName );
               if( stringValue == "" )
                  break;
               return stringValue;
            case TraceColumnId.DatabaseUserName:
               stringValue = job.GetRowString( row, TraceJob.ndxDbUserName );
               if( stringValue == "" )
                  break;
               return stringValue;
            case TraceColumnId.SQLcmTableName:
               string schema = job.jobInfo.isSqlServer2005 
                             ? job.GetRowString( row, TraceJob.ndxParentName ) 
                             : "dbo";
               string table = job.GetRowString( row, TraceJob.ndxObjectName );
               stringValue = CoreHelpers.GetTableNameKey( schema, table );
               return stringValue;
            default:
               return new object();
         }
         return new object();
      }

      //---------------------------------------------------------------------------------
      // CheckFinalFilter
      //---------------------------------------------------------------------------------
      private static bool
         CheckFinalFilter(
            int      type,
            int      eventClass,
            TraceJob job,
            DataRow  row
         )
      {
         bool match = true;
         int objType;

         switch( eventClass )
         {
               // Login Events
            case (int)TraceEventId.Login:
               break;
            case (int)TraceEventId.Logout:
               break;
            case (int)TraceEventId.LoginFailed:   
               break;
	         
               // DDL
            case (int)TraceEventId.AuditObjectDerivedPermission:
               // Filter out create database from DB security trace
               objType = job.GetRowInt32( row, TraceJob.ndxObjectType );
               if( type == 2 && DBObject.IsDatabae(objType)  )
                  match = false;
               else if( type == 1                                && 
                  !DBObject.IsDatabae(objType) )
                  match = false;
               break;
            case (int)TraceEventId.AuditStatementPermission:
               // Filter out create database from DB security trace
               objType = job.GetRowInt32( row, TraceJob.ndxObjectType );
               if( type == 2 )
               {
                  if( DBObject.IsDatabae(objType) )
                     match = false;
               }
               else if( type == 1                               && 
                  !DBObject.IsDatabae(objType) )
                  match = false;
               break;

            case (int)TraceEventId.AuditDatabaseManagement:
               int subClass = job.GetRowInt32( row, TraceJob.ndxEventSubclass );
               if (subClass == 1 || subClass == 2 || subClass == 3) // check for create/alter/drop operations
               {
                  objType = job.GetRowInt32( row, TraceJob.ndxObjectType );
                  if( type == 2 && DBObject.IsDatabae( objType)) // filter out Database operations from db security traces
                     match = false;
                  else if (type == 1 &&           // remove events with objects other than Databases from server traces
                     !DBObject.IsDatabae(objType))
                        match = false;
               }
               else if( type != 2 ) // not db security trace
                  match = false;
               break;

            case (int)TraceEventId.ObjectCreated:
               break;
	            
               // Security Events
            case (int)TraceEventId.AuditObjectGDR:
               break;
            case (int)TraceEventId.AuditStatementGDR:
               {
                  string dbName = job.GetRowString( row, TraceJob.ndxDatabaseName );
                  if( (type == 3 ) ||               // priv user
                      (type == 2 && job.jobInfo.databaseNames.Contains( dbName.ToUpper() )) ) // database level: filter based on name
                     match = true;
                  else
                     match = false;
               }
               break;
            case (int)TraceEventId.AuditLoginGDR:
               break;
            case (int)TraceEventId.AuditLoginChange:
               break;
            case (int)TraceEventId.AuditLoginChangePassword:
               break;
            case (int)TraceEventId.AuditAddLogin:
               break;
            case (int)TraceEventId.AuditAddLoginToServer:
               break;
            case (int)TraceEventId.AuditAddDbUser:
               break;
            case (int)TraceEventId.AuditAddMember:
               break;
            case (int)TraceEventId.AuditAddDropRole:
               break;
            case (int)TraceEventId.AppRolePassChange:
               break;
	            
               // DML  && SELECT
            case (int)TraceEventId.AuditObjectPermission:
               // Remove before/after end events
               if (job.jobInfo.isSqlServer2005 
                   && job.GetRowPermissions(row, TraceJob.ndxPermissions) == 16 )    
               {
                  if ( job.GetRowString( row, TraceJob.ndxParentName ) ==
                       CoreConstants.Agent_BeforeAfter_SchemaName
                       &&
                       job.GetRowString( row, TraceJob.ndxObjectName ) ==
                       CoreConstants.Agent_BeforeAfter_TableName )
                     return false;
               }
               break;
	         
               // Other   
            case (int)TraceEventId.AuditBackupRestore:
               break;
            case (int)TraceEventId.AuditDbcc:
               if( job.jobInfo.traceSequence > 1 )
                  match = false;
               else
               {
                  string dbName = job.GetRowString( row, TraceJob.ndxDatabaseName );
                  if( (type == 3 ) ||               // priv user - any dbcc event
                      (type == 2 && job.jobInfo.databaseNames.Contains( dbName.ToUpper() )) || // database level: filter based on name
                      (type == 1 && dbName == "" )) // Server level - let empty db name events pass through
                     match = true;
                  else
                     match = false;
               }
               break;
            case (int)TraceEventId.AuditChangeAudit:
               break;
            case (int)TraceEventId.SqlStmtCompleted:
                match = false;
              break;
            case (int)TraceEventId.SqlStmtStarting:
               match = false;
               break;
            case (int)TraceEventId.Exception:
               match = false;
               break;
            default:
               break;
         }
         return match;
      }

      #endregion

      #endregion

      #region private methods

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isLoginEventFilter ()
      {
         if( TraceColumnId.ApplicationName == columnId )
            return true;

         return false;
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
        isLogoutEventFilter()
      {
          if (TraceColumnId.ApplicationName == columnId)
              return true;

          return false;
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isLoginFailedEventFilter ()
      {
         if( TraceColumnId.ApplicationName == columnId )
            return true;

         return false;
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditObjectDerivedPermissionEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.DatabaseID:
            //case TraceColumnId.ObjectType:
            //case TraceColumnId.ObjectOwner:
            case TraceColumnId.ObjectName:
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditStatementPermissionEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.DatabaseID:
            //case TraceColumnId.Permissions:
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;
         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditObjectGdrEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.DatabaseID:
            //case TraceColumnId.ObjectType:
            //case TraceColumnId.ObjectOwner:
            //case TraceColumnId.ObjectName:
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditStatementGdrEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.DatabaseID:
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditLoginGdrEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditLoginChangeEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditLoginChangePasswordEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditAddLoginEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditAddLoginToServerEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditAddDbUserEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.DatabaseID:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditAddMemberEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.DatabaseID:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditAddRoleEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.DatabaseID:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAppRolePassChangeEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.DatabaseID:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditObjectPermissionEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.DatabaseID:
            case TraceColumnId.ObjectType:
            case TraceColumnId.ObjectName:
            case TraceColumnId.ObjectOwner:
            case TraceColumnId.ObjectID:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
            case TraceColumnId.Permissions:
            case TraceColumnId.SQLcmTableName:
               return true;
            default:
               return false;

         }
      }

      private bool isAuditTransactionEventFilter()
      {
         switch (columnId)
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.DatabaseID:
            case TraceColumnId.ObjectName:
            case TraceColumnId.ApplicationName:
               return true;
            default:
               return false;
         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditBackupRestoreEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.DatabaseID:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditDbccEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditChangeAuditEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;
         }
      }

      
      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isObjectCreatedEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.DatabaseID:
            case TraceColumnId.ObjectType:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isSqlStmtStartingEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.DatabaseID:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isSqlStmtCompletedEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.DatabaseID:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }


      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditDatabaseManagementEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditDatabaseObjectManagementEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.DatabaseID:
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditDatabasePrincipalManagementEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.DatabaseID:
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditSchemaObjectManagementEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.DatabaseID:
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.ObjectType:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditServerPrincipalImpersonationEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditDatabasePrincipalImpersonationEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.DatabaseID:
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditServerObjectTakeOwnershipEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditDatabaseObjectTakeOwnershipEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.DatabaseID:
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditChangeDatabaseOwnerEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditSchemaObjectTakeOwnershipEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.DatabaseID:
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditBrokerConversationEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.DatabaseID:
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditBrokerLoginEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.DatabaseID:
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditServerScopeGdrEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditServerObjectGdrEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditDatabaseObjectGdrEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.DatabaseID:
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditServerOperationEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditServerAlterTraceEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditServerObjectManagementEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditServerPrincipalManagementEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditDatabaseOperationEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.DatabaseID:
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }

      //----------------------------------------------------------
      // 
      //----------------------------------------------------------
      private bool
         isAuditDatabaseObjectAccessEventFilter ()
      {
         switch( columnId )
         {
            case TraceColumnId.DatabaseID:
            case TraceColumnId.SQLSecurityLoginName:
            case TraceColumnId.ApplicationName:
            case TraceColumnId.Success:
               return true;
            default:
               return false;

         }
      }
      #endregion
   }
}
