using System;
using System.Collections;
using System.Data;

using Idera.SQLcompliance.Core.Event;

namespace Idera.SQLcompliance.Core.Stats
{
   public class Stats : IStats
   {
      #region Data members
      
      static Stats instance;
      
      DateTime date;
      static int      daysStatsCached;
      Hashtable instanceStats;
      static Hashtable dbLists = new Hashtable();
      static long keepInCacheTime;
      
      #endregion
      
      #region Constructors
      
      static Stats()
      {
         daysStatsCached = CoreConstants.DaysStatsCached;
         keepInCacheTime = 2 * 60 * 60 * 1000;
         instance = new Stats();
      }
      
      private Stats()
      {
         date = DateTime.Now;
         instanceStats = GetInstanceStats();
         //StatsDAL.PopulateStatsCategortiesTable();
      }
      
      #endregion
      
      #region Properties
      
      public static Stats Instance
      {
         get { return instance; }
      }
      
      public static int DaysStatsCached
      {
         get { return daysStatsCached; }
      }
      
      public DateTime Date
      {
         get { return date; }
         set { date = value; }
      }
      
      static public long KeepInCacheTime
      {
         get { return keepInCacheTime; }
         set { keepInCacheTime = value; }
      }
      
      #endregion
      
      #region Public methods
      
      public bool Update()
      {
         bool success = true;
         
         IDictionaryEnumerator enumerator = instanceStats.GetEnumerator();
         while( enumerator.MoveNext() )
         {
            if( !((InstanceStats)enumerator.Value).Update() )
            {
               success = false;
               break;
            }
         }
               
         
         return success;
      }
      
      public bool Update( string instanceName )
      {
         if( instanceStats.Contains( instanceName ) )
            return ((InstanceStats)instanceStats[instanceName]).Update();
         else 
            return false;
      }
      
      
      public InstanceStats GetStats( string instanceName )
      {
         
         if( instanceStats.Contains( instanceName ))
         {
            return (InstanceStats)instanceStats[instanceName];
         }
         else
         {
            DataRow row = StatsDAL.GetInstanceInfo( instanceName );
            if( row == null )
               return new InstanceStats( instanceName ); // not audited
            else 
               return new InstanceStats( row );
         }
         
      }
      
      public InstanceStats [] GetStats()
      {
         ArrayList list = new ArrayList();
         
         IDictionaryEnumerator enumerator = instanceStats.GetEnumerator();
         
         while( enumerator.MoveNext() )
            list.Add( enumerator.Value );
            
         return (InstanceStats [])list.ToArray( typeof(InstanceStats));
      }
      
      //
      // Recalculate the stats for an instance
      //
      public bool Recalculate( string instanceName )
      {  
         try
         {
            InstanceStats iStats = GetStats( instanceName );
            return iStats.Recalculate();
         }
         catch( Exception e )
         {
            string msg = String.Format( "An error occurred recalculating statistics for {0}.", instanceName );
            ErrorLog.Instance.Write( msg, e, ErrorLog.Severity.Error );
            return false;
         }
      }
      
      public bool Recalculate( string instanceName, string database )
      {
         try
         {
            InstanceStats iStats = new InstanceStats(instanceName);
            return iStats.Recalculate( database );
         }
         catch (Exception e)
         {
            string msg = String.Format("An error occurred recalculating statistics for {0}.", database);
            ErrorLog.Instance.Write(msg, e, ErrorLog.Severity.Error);
            return false;
         }
      }

      //
      // Recalculate stats for all instances
      //
      public bool Recalculate()
      {
         InstanceStats [] list = GetStats();
         bool success = true;
         
         for( int i = 0; i < list.Length; i++ )
         {
            if( !list[i].Recalculate())
               success = false;
         }
         
         return success;
      }
            
         
      
      #endregion
      
      #region Static public methods
      
      public static StatsCategory GetStatsCategory( TraceEventType type )
      {
         switch( type )
         {
               // Unknown
            case TraceEventType.InvalidType:
               return StatsCategory.Unknown;

               // Select
            case TraceEventType.SELECT:
            case TraceEventType.REFERENCES:
            case TraceEventType.AccessBase:
               return StatsCategory.Select;

               // Update
            case TraceEventType.UPDATE:
               return StatsCategory.Update;

               // INSERT
            case TraceEventType.INSERT:
               return StatsCategory.Insert;

               // DELETE
            case TraceEventType.DELETE:
               return StatsCategory.Delete;

               // Execute
            case TraceEventType.EXECUTE:
               return StatsCategory.Execute;

            case TraceEventType.OpenBase:
            case TraceEventType.TransferBase:
            case TraceEventType.BeginTran:
            case TraceEventType.CommitTran:
            case TraceEventType.RollbackTran:
            case TraceEventType.SaveTran:
               return StatsCategory.DML;
                    //start sqlcm 5.6 - 5363
                case TraceEventType.Logout://removed logout from login category
                    return StatsCategory.Logout;
                //end sqlcm 5.6 - 5363
                // Logins
                case TraceEventType.ServerImpersonation:
            case TraceEventType.DatabaseImersonation:
            case TraceEventType.Login:
            
            case TraceEventType.BrokerConversation:
            case TraceEventType.BrokerLogin:
               return StatsCategory.Logins;

               // FailedLogin
            case TraceEventType.LoginFailed:
               return StatsCategory.FailedLogin;

               // Security
            case TraceEventType.DisableLogin:
            case TraceEventType.EnableLogin:
            case TraceEventType.GrantObjectGdrBase:
            case TraceEventType.DenyObjectGdrBase:
            case TraceEventType.RevokeObjectGdrBase:
            case TraceEventType.AddDatabaseUser:
            case TraceEventType.DropDatabaseUser:
            case TraceEventType.GrantDatabaseAccess:
            case TraceEventType.RevokeDatabaseAccess:
            case TraceEventType.AddLogintoServerRole:
            case TraceEventType.DropLoginfromServerRole:
            case TraceEventType.AddMembertoDatabaseRole:
            case TraceEventType.DropMembertoDatabaseRole:
            case TraceEventType.AddDatabaseRole:
            case TraceEventType.DropDatabaseRole:
            case TraceEventType.AddLogin:
            case TraceEventType.DropLogin:
            case TraceEventType.AppRoleChangePassword:
            case TraceEventType.PasswordChangeSelf:
            case TraceEventType.PasswordChange:
            case TraceEventType.LoginChangePropertyDB:
            case TraceEventType.LoginChangePropertyLanguage:
            case TraceEventType.GrantLogin:
            case TraceEventType.RevokeLogin:
            case TraceEventType.DenyLogin:
            case TraceEventType.ServerObjectChangeOwner:
            case TraceEventType.ChangeDatabaseOwner:
            case TraceEventType.DatabaseObjectChangeOwner:
            case TraceEventType.SchemaObjectChangeOwner:
            case TraceEventType.CredentialMapped:
            case TraceEventType.CredentialMapDropped:

            case TraceEventType.GrantStmtBase:
            case TraceEventType.GrantCREATEDATABASE:
            case TraceEventType.GrantCREATETABLE:
            case TraceEventType.GrantCREATEPROCEDURE:
            case TraceEventType.GrantCREATEVIEW:
            case TraceEventType.GrantCREATERULE:
            case TraceEventType.GrantCREATEDEFAULT:
            case TraceEventType.GrantBACKUPDATABASE:
            case TraceEventType.GrantBACKUPLOG:
            case TraceEventType.GrantCREATEFUNCTION:

            case TraceEventType.DenyStmtBase:
            case TraceEventType.DenyCREATEDATABASE:
            case TraceEventType.DenyCREATETABLE:
            case TraceEventType.DenyCREATEPROCEDURE:
            case TraceEventType.DenyCREATEVIEW:
            case TraceEventType.DenyCREATERULE:
            case TraceEventType.DenyCREATEDEFAULT:
            case TraceEventType.DenyBACKUPDATABASE:
            case TraceEventType.DenyBACKUPLOG:
            case TraceEventType.DenyCREATEFUNCTION:

            case TraceEventType.RevokeStmtBase:
            case TraceEventType.RevokeCREATEDATABASE:
            case TraceEventType.RevokeCREATETABLE:
            case TraceEventType.RevokeCREATEPROCEDURE:
            case TraceEventType.RevokeCREATEVIEW:
            case TraceEventType.RevokeCREATERULE:
            case TraceEventType.RevokeCREATEDEFAULT:
            case TraceEventType.RevokeBACKUPDATABASE:
            case TraceEventType.RevokeBACKUPLOG:
            case TraceEventType.RevokeCREATEFUNCTION:

            case TraceEventType.CreateServerRole:
            case TraceEventType.DropServerRole:
               return StatsCategory.Security;

               // DDL
            case TraceEventType.CreateBase:
            case TraceEventType.CreateIndex:
            case TraceEventType.CreateDatabase:
            case TraceEventType.CreateUserObject:
            case TraceEventType.CreateCHECK:
            case TraceEventType.CreateDEFAULT:
            case TraceEventType.CreateFOREIGNKEY:
            case TraceEventType.CreatePRIMARYKEY:
            case TraceEventType.CreateStoredProcedure:
            case TraceEventType.CreateUDF:
            case TraceEventType.CreateRule:
            case TraceEventType.CreateReplFilterStoredProc:
            case TraceEventType.CreateSystemTable:
            case TraceEventType.CreateTrigger:
            case TraceEventType.CreateInlineFunction:
            case TraceEventType.CreateTableValuedUDF:
            case TraceEventType.CreateUNIQUE:
            case TraceEventType.CreateUserTable:
            case TraceEventType.CreateView:
            case TraceEventType.CreateExtStoredProc:
            case TraceEventType.CreateAdHocQuery:
            case TraceEventType.CreatePreparedQuery:
            case TraceEventType.CreateStatistics:
            case TraceEventType.CreateLinkedServer:
            case TraceEventType.DeleteLinkedServer:
               return StatsCategory.DDL;

               // Admin
            case TraceEventType.DBCC:
            case TraceEventType.DBCC_read:
            case TraceEventType.DBCC_write:
            case TraceEventType.AuditStarted:
            case TraceEventType.AuditStopped:
            case TraceEventType.ServerOperation:
            case TraceEventType.DatabaseOperation:
            case TraceEventType.ServerAlterTrace:
            case TraceEventType.Backup:
            case TraceEventType.Restore:
            case TraceEventType.BackupDatabase:
            case TraceEventType.BackupLog:
            case TraceEventType.BackupTable:
            case TraceEventType.LoadBase:
            case TraceEventType.DumpBase:
               return StatsCategory.Admin;

            case TraceEventType.UserDefinedEvent0:
            case TraceEventType.UserDefinedEvent1:
            case TraceEventType.UserDefinedEvent2:
            case TraceEventType.UserDefinedEvent3:
            case TraceEventType.UserDefinedEvent4:
            case TraceEventType.UserDefinedEvent5:
            case TraceEventType.UserDefinedEvent6:
            case TraceEventType.UserDefinedEvent7:
            case TraceEventType.UserDefinedEvent8:
            case TraceEventType.UserDefinedEvent9:
               return StatsCategory.UserDefinedEvents;

            case TraceEventType.DummyEvent:
            case TraceEventType.MissingEvents:
            case TraceEventType.InsertedEvent:
            case TraceEventType.ModifiedEvent:
               return StatsCategory.IntegrityCheck;

            default:
               if ( IsDDLEvent( type ) )
               {
                  if ( IsSecurityDDLEvent( type ) )
                  {
                     return StatsCategory.Security;
                  }
                  else
                  {
                     return StatsCategory.DDL;
                  }
               }

               return StatsCategory.Unknown;
            

         }
      }

      public static bool IsDDLEvent(TraceEventType type)
      {
         if( ( type >= TraceEventType.CreateBase && type < TraceEventType.CreateBase + 100 ) ||
             ( type >= TraceEventType.DropBase && type < TraceEventType.DropBase + 100 ) ||
             ( type >= TraceEventType.AlterBase && type < TraceEventType.AlterBase + 100 ) )
            return true;
         else 
            return false;
      }
      
      // check whether the DDL event is for security objects such as logins
      public static bool IsSecurityDDLEvent(TraceEventType type)
      {
         int objectType;
         if ( type >= TraceEventType.CreateBase && type < TraceEventType.CreateBase + 100 )
            objectType = type - TraceEventType.CreateBase;
         else if ( type >= TraceEventType.DropBase && type < TraceEventType.DropBase + 100 )
            objectType = type - TraceEventType.DropBase;

         else if ( type >= TraceEventType.AlterBase &&
                   type < TraceEventType.AlterBase + 100 )
            objectType = type - TraceEventType.AlterBase;
         else
            return false;
            
         switch ( objectType )
         {
            case 38: // server role
            case 39: // windows groups
            case 43: // asym key login
            case 45: // application role
            case 46: // SQL login
            case 47: // Windows Login
            case 56: // application role 2005
               return true;
            default:
               return false;
         }
      }

      public static bool IsServerLevelEvent( TraceEventType type )
      {
         switch( type )
         {
            case TraceEventType.ServerImpersonation:
            case TraceEventType.DatabaseImersonation:
            case TraceEventType.Login:
            case TraceEventType.Logout:
            case TraceEventType.BrokerConversation:
            case TraceEventType.BrokerLogin:

            // FailedLogin
            case TraceEventType.LoginFailed:

            // Security
            case TraceEventType.DisableLogin:
            case TraceEventType.EnableLogin:
            case TraceEventType.AddLogintoServerRole:
            case TraceEventType.DropLoginfromServerRole:
            case TraceEventType.AddLogin:
            case TraceEventType.DropLogin:
            case TraceEventType.AppRoleChangePassword:
            case TraceEventType.PasswordChangeSelf:
            case TraceEventType.PasswordChange:
            case TraceEventType.LoginChangePropertyDB:
            case TraceEventType.LoginChangePropertyLanguage:
            case TraceEventType.GrantLogin:
            case TraceEventType.RevokeLogin:
            case TraceEventType.DenyLogin:
            case TraceEventType.ServerObjectChangeOwner:
            case TraceEventType.CredentialMapped:
            case TraceEventType.CredentialMapDropped:

            // GDR
            case TraceEventType.GrantCREATEDATABASE:
            case TraceEventType.DenyCREATEDATABASE:
            case TraceEventType.RevokeCREATEDATABASE:

            // DDL
            case TraceEventType.CreateDatabase:
            case TraceEventType.CreateLinkedServer:
            case TraceEventType.DeleteLinkedServer:

            // Admin
            case TraceEventType.AuditStarted:
            case TraceEventType.AuditStopped:
            case TraceEventType.ServerOperation:
            case TraceEventType.ServerAlterTrace:

            case TraceEventType.UserDefinedEvent0:
            case TraceEventType.UserDefinedEvent1:
            case TraceEventType.UserDefinedEvent2:
            case TraceEventType.UserDefinedEvent3:
            case TraceEventType.UserDefinedEvent4:
            case TraceEventType.UserDefinedEvent5:
            case TraceEventType.UserDefinedEvent6:
            case TraceEventType.UserDefinedEvent7:
            case TraceEventType.UserDefinedEvent8:
            case TraceEventType.UserDefinedEvent9:

            case TraceEventType.DummyEvent:
            case TraceEventType.MissingEvents:
            case TraceEventType.InsertedEvent:
            case TraceEventType.ModifiedEvent:
               return true;

            default:
               if( ( type - TraceEventType.CreateBase) == 2 || // Create/Alter/Drop database
                   ( type - TraceEventType.AlterBase ) == 2 ||
                   ( type - TraceEventType.DropBase ) == 2 )
                   return true;
               return false;
         }
      }
     
      #endregion
      
      #region Internal Methods

      static internal Hashtable GetDbList(string inst)
      {
         if( dbLists.Contains( inst ) )
            return (Hashtable)dbLists[inst];
         else
         {
            Hashtable list = GetDbNameList( inst );
            dbLists.Add( inst, list );
            return list;
         }
      }
      
      internal void UpdateAlertCount( string instanceName, int count )
      {
         try
         {
            InstanceStats iStats = GetStats( instanceName );
            InstanceStats newStats = iStats.GetNewInstanceStats();
            newStats.Add( StatsCategory.Alerts, DateTime.UtcNow, count );
            iStats.Update( newStats );
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     "An error occurred updating the alerts stats.",
                                     e,
                                     true );
         }
      }
      
      internal void UpdateIntegrityCheckCount( string database, int count )
      {
         try
         {
            StatsDAL.UpdateStatsRecord( database, StatsDAL.SvrStatsDbId, StatsCategory.IntegrityCheck, DateTime.UtcNow, count ); 
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "An error occurred updating the integrity check stats for " + database,
                                     e,
                                     true);
         }
      }

      #endregion
      
      #region Private Methods
            
      Hashtable GetInstanceStats()
      {
         Hashtable records = new Hashtable();
         
         try
         {
            using( DataTable table = StatsDAL.GetInstanceInfo() )
            {
               InitDbLists();
               
               InstanceStats newStats;
               for( int i = 0; i < table.Rows.Count; i++ )
               {
                  newStats = new InstanceStats( table.Rows[i] );
                  records.Add( newStats.Instance, newStats );
               }
            }
         }
         catch( Exception e)
         {
            ErrorLog.Instance.Write("An error occurred while initializing stats cache.",
                                     e,
                                     ErrorLog.Severity.Error,
                                     true);
            throw e;
         }
               
         return records;
      }
      
      static void InitDbLists()
      {
         Hashtable lists = dbLists;
         Hashtable dbList;
         string inst;
         //int instId;
         string dbName;
         int dbId;

         using (DataTable table = StatsDAL.GetDbLists())
         {
            for( int i = 0; i < table.Rows.Count; i++ )
            {
               GetDatabaseTableRowValue( table.Rows[i], out inst, out dbName, out dbId );
            
               if( !lists.Contains( inst ) )
               {
                  dbList = new Hashtable();
                  lists.Add( inst, dbList );
               }
               else 
                  dbList = (Hashtable)lists[inst];
            
               dbList.Add( dbId, dbName );          
              
            }
         }            
      }
      
      static Hashtable GetDbNameList( string inst )
      {
         string name;
         string dbName;
         int    dbId;
         Hashtable list = new Hashtable();
         
         using( DataTable table = StatsDAL.GetDbList( inst ) )
         {
            for( int i = 0; i < table.Rows.Count; i++ )
            {
               GetDatabaseTableRowValue( table.Rows[i], out name, out dbName, out dbId );
               list.Add( dbId, dbName );
            }
         }
         
         return list;
      }
      
      static void GetDatabaseTableRowValue( DataRow row, out string instanceName, out string dbName, out int dbId )
      {
         instanceName = StatsDAL.GetStringValue(row, StatsDAL.ColDTInstance);
         //instId = StatsDAL.GetIntValue( row, StatsDAL.ColDTSrvId );
         dbName = StatsDAL.GetStringValue(row, StatsDAL.ColDTName);
         dbId = StatsDAL.GetIntValue(row, StatsDAL.ColDTDbId);
      }
      #endregion
   }
   
}
