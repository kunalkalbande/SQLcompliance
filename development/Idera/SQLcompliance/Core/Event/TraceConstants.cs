using System.Runtime.Serialization;

namespace Idera.SQLcompliance.Core.Event
{  
   #region Trace Constants

   // Options for creating a trace file
    [DataContract]
   public enum TraceOption : int
   {
        [EnumMember]
      Unknown          = -1,

        [EnumMember]
      ProduceRowSet    = 1,

        [EnumMember]
      RollOver         = 2,

        [EnumMember]
      ShutDownOnError  = 4,

        [EnumMember]
      BlackBox         = 8
   }


   //  Properties for a trace
   public enum TraceInfoProperty : int
   {
      Unknown     = -1,
      Options     = 1,
      FileName    = 2,
      MaxSize     = 3,
      StopTime    = 4,
      Status      = 5
   }

   public enum TraceCategory : int
   {
      ServerTrace                 = 1,
      DBSecurity                  = 2,
      DMLwithSELECT               = 3,
      DMLwithDetails              = 4,
      DML                         = 5,
      SELECTwithDetails           = 6,
      SELECT                      = 7,
      DataChange                  = 8,
      SensitiveColumn             = 9,
      SensitiveColumnwithSelect   = 10,
      Transactions                = 11, // This has to be it own trace so we can always filter on DBid.  Trace combination means there will not always be a DML trace for every audited database.
      ServerStartStop             = 12,
      DBPrivilegedUsers           = 13,
      DataChangeWithDetails       = 14
   }

   public enum TraceLevel : int
   {
      Server        = 1,
      Database      = 2,
      User          = 3,
      Table         = 4,
      Object        = 5

   }

   public enum TraceFilterResult : int
   {
      NotApplicable    = 0,
      Matched          = 1,
      FilteredOut      = 2
   }
   
   #endregion

	/// <summary>
	/// Summary description for TraceConstants.
	/// </summary>
	public class TraceConstants
	{
      #region Public Constants

      public static TraceEventId [] ServerEvents =    {
                                                          TraceEventId.Login,
                                                          TraceEventId.Logout,
                                                          TraceEventId.LoginFailed,
                                                          TraceEventId.AuditLoginGDR,
                                                          TraceEventId.AuditAddLoginToServer,  // To Server Role
                                                          TraceEventId.AuditAddLogin,
                                                          TraceEventId.AuditLoginChangePassword,
                                                          TraceEventId.AuditLoginChange,
                                                          TraceEventId.AuditChangeAudit,
                                                          TraceEventId.UserEvent0,
                                                          TraceEventId.UserEvent1,
                                                          TraceEventId.UserEvent2,
                                                          TraceEventId.UserEvent3,
                                                          TraceEventId.UserEvent4,
                                                          TraceEventId.UserEvent5,
                                                          TraceEventId.UserEvent6,
                                                          TraceEventId.UserEvent7,
                                                          TraceEventId.UserEvent8,
                                                          TraceEventId.UserEvent9,
                                                          TraceEventId.Exception
                                                      };

      public static TraceEventId [] DBSecurityEvents = {
                                                          TraceEventId.AuditObjectGDR,               // 103
                                                          TraceEventId.AuditAddDbUser,               // 109
                                                          TraceEventId.AuditAddDropRole,             // 111
                                                          TraceEventId.AuditAddMember,               // 110
                                                          TraceEventId.AuditStatementGDR,            // 102
                                                          TraceEventId.AuditObjectDerivedPermission, // 118
                                                          TraceEventId.AuditStatementPermission,     // 113
                                                          TraceEventId.AuditBackupRestore,           // 115
                                                          TraceEventId.AuditDbcc,                    // 116
                                                          TraceEventId.AppRolePassChange             // 112
                                                       };

      public static TraceEventId [] DMLEvents = {
                                             TraceEventId.AuditObjectPermission,    //114
                                             TraceEventId.Transaction
                                          };

      public static TraceEventId [] DDLEvents = {
                                          };

      public static TraceEventId [] SecurityChangesEvents = {
                                                      };

      public static TraceEventId [] LoginsEvents = {
                                                   };

      public static TraceEventId [] FailedLoginEvents = {
                                                        };

      public static TraceEventId [] AdminEvents = {
                                                  };

      public static TraceEventId [] ErrorEvents = {
                                                  };

      public static TraceEventId [] AuditChangesEvents = {
                                                         };



      #endregion
      #region Private Constructor
		private TraceConstants()
		{
		}
      #endregion
      
      #region Public methods
      
      public static bool IsDatabaseLevelEvent( TraceEventType type )
      {
         switch( type )
         {
            // DML and SELECT
            case TraceEventType.SELECT:                            
            case TraceEventType.UPDATE:                            
            case TraceEventType.REFERENCES:                        
            case TraceEventType.INSERT:                            
            case TraceEventType.DELETE:                            
            case TraceEventType.EXECUTE:
            case TraceEventType.BeginTran:
            case TraceEventType.CommitTran:
            case TraceEventType.RollbackTran:
            case TraceEventType.SaveTran:
               return true;

            // Login
            case TraceEventType.Login:                             
            case TraceEventType.Logout:                             
            case TraceEventType.LoginFailed:                       
            case TraceEventType.ServerImpersonation:               
            case TraceEventType.DatabaseImersonation:              
            case TraceEventType.DisableLogin:                      
            case TraceEventType.EnableLogin:                       
               return false;
               
            // Backup and Restore
            case TraceEventType.Backup:                            
            case TraceEventType.Restore:                           
            case TraceEventType.BackupDatabase:                    
            case TraceEventType.BackupLog:                         
            case TraceEventType.BackupTable:                       
               return true;
               
            // Trace events
            case TraceEventType.AuditStarted:
            case TraceEventType.ServerAlterTrace:
            case TraceEventType.AuditStopped:                      
               return false;
               
            case TraceEventType.DBCC:                              
            case TraceEventType.DBCC_read:                         
            case TraceEventType.DBCC_write:                        
               return true;
               
            case TraceEventType.ServerOperation:
               return false;
               
            case TraceEventType.DatabaseOperation:                 
               return true;
               
            case TraceEventType.BrokerConversation:                
            case TraceEventType.BrokerLogin:
               return true;
               
            // DDL: Create                       
            case TraceEventType.CreateBase:                        
            case TraceEventType.CreateIndex:                       
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
               return true;
               

            case TraceEventType.GrantObjectGdrBase:                
            case TraceEventType.DenyObjectGdrBase:                 
            case TraceEventType.RevokeObjectGdrBase:               
            case TraceEventType.AddDatabaseUser:                   
            case TraceEventType.DropDatabaseUser:                  
            case TraceEventType.GrantDatabaseAccess:               
            case TraceEventType.RevokeDatabaseAccess:             
               return true;

            case TraceEventType.CreateDatabase:
            case TraceEventType.AddLogintoServerRole:              
            case TraceEventType.DropLoginfromServerRole: 
            case TraceEventType.CreateServerRole:
            case TraceEventType.DropServerRole:
               return false;
               
            case TraceEventType.AddMembertoDatabaseRole:           
            case TraceEventType.DropMembertoDatabaseRole:          
            case TraceEventType.AddDatabaseRole:                   
            case TraceEventType.DropDatabaseRole:
            case TraceEventType.AppRoleChangePassword:
               return true;

            case TraceEventType.AddLogin:                          
            case TraceEventType.DropLogin:                         
            case TraceEventType.PasswordChangeSelf:                
            case TraceEventType.PasswordChange:                    
            case TraceEventType.LoginChangePropertyDB:             
            case TraceEventType.LoginChangePropertyLanguage:       
            case TraceEventType.GrantLogin:                        
            case TraceEventType.RevokeLogin:                       
            case TraceEventType.DenyLogin:                         
            case TraceEventType.ServerObjectChangeOwner:           
               return false;

            case TraceEventType.ChangeDatabaseOwner:               
            case TraceEventType.DatabaseObjectChangeOwner:         
            case TraceEventType.SchemaObjectChangeOwner:           
            case TraceEventType.CredentialMapped:                  
            case TraceEventType.CredentialMapDropped:              
               return true;

            // GDR statement
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
               return true;

            case TraceEventType.DumpBase:                          
            case TraceEventType.OpenBase:                          
            case TraceEventType.LoadBase:                          
            case TraceEventType.AccessBase:                        
            case TraceEventType.TransferBase:                      
               return true;


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
               return false;

             case TraceEventType.CreateLinkedServer:
             case TraceEventType.DeleteLinkedServer:
                 return false;


            case TraceEventType.DummyEvent:                        
            case TraceEventType.MissingEvents:                     
            case TraceEventType.InsertedEvent:                     
            case TraceEventType.ModifiedEvent:                    
               return true;
               
            default:
               if( type >= TraceEventType.AlterBase &&
                   type < TraceEventType.AlterBase + 100 )
               {
                  return true;
               }
               else if( type >= TraceEventType.DropBase &&
                        type < TraceEventType.DropBase + 100 )
               {
                  return true;
               }
               else
                  return false;
         } 
         #endregion        
      }

	}
}
