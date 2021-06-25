using System;
using System.Collections ;

namespace Idera.SQLcompliance.Core.Event
{
   /// <summary>
   /// List of SQL Server object types
   /// </summary>
   public enum DBObjectType : int
   {
      Unknown               = -1,
      Index                 = 1,
      Database              = 2,
      UserObject            = 3,
      CheckConstraint       = 4,
      DefaultConstraint     = 5,
      ForeignKeyConstraint  = 6,
      PrimaryKeyConstraint  = 7,
      StoredProcedure       = 8,
      UDF                   = 9,
      Rule                  = 10,
      ReplicationFilterSP   = 11,
      SystemTable           = 12,
      Trigger               = 13,
      InlineFunction        = 14,
      TableValuedUDF        = 15,
      UniqueConstraint      = 16,
      UserTable             = 17,
      View                  = 18,
      ExtendStoredProcedure = 19,
      AdHocQuery            = 20,
      PreparedQuery         = 21,
      Statistics            = 22,

      // SQL Server 2005 Object Types
      CheckConstraint_2005                = 8259, 
      DefaultConstraint_2005              = 8260, 
      ForeignKeyConstraint_2005           = 8262, 
      StoredProcedure_2005                = 8272, 
      Rule_2005                           = 8274, 
      SystemTable_2005                    = 8275, 
      TriggerOnServer_2005                = 8276, 
      UserTable_2005                      = 8277, 
      View_2005                           = 8278, 
      ExtendedStoredProcedure_2005        = 8280, 
      CLRTrigger_2005                     = 16724,
      Database_2005                       = 16964,
      Object_2005                         = 16975,
      FullTextCatalog_2005                = 17222,
      CLRStoredProcedure_2005             = 17232,
      Schema_2005                         = 17235,
      Credential_2005                     = 17475,
      DDLEvent_2005                       = 17491,
      ManagementEvent_2005                = 17741,
      SecurityEvent_2005                  = 17747,
      UserEvent_2005                      = 17749,
      CLRAggregateFunction_2005           = 17985,
      InlineTableValuedSQLFunction_2005   = 17993,
      PartitionFunction_2005              = 18000,
      ReplicationFilterProcedure_2005     = 18002,
      TableValuedSQLFunction_2005         = 18004,
      ServerRole_2005                     = 18259,
      MicrosoftWindowsGroup_2005          = 18263,
      AsymmetricKey_2005                  = 19265,
      MasterKey_2005                      = 19277,
      PrimaryKey_2005                     = 19280,
      ObfusKey_2005                       = 19283,
      AsymmetricKeyLogin_2005             = 19521,
      CertificateLogin1_2005              = 19523,
      Role_2005                           = 19538,
      SQLLogin_2005                       = 19539,
      WindowsLogin_2005                   = 19543,
      RemoteServiceBinding_2005           = 20034,
      EventNotificationOnDatabase_2005    = 20036,
      EventNotification_2005              = 20037,
      ScalarSQLFunction_2005              = 20038,
      EventNotificationOnObject_2005      = 20047,
      Synonym_2005                        = 20051,
      EndPoint_2005                       = 20549,
      CachedAdhocQueries_2005             = 20801,
      CachedAdhocQueries2_2005            = 20816,
      ServiceBrokerServiceQueue_2005      = 20819,
      UniqueConstraint_2005               = 20821,
      ApplicationRole_2005                = 21057,
      Certificate_2005                    = 21059,
      Server_2005                         = 21075,
      TransactSQLTrigger_2005             = 21076,
      Assembly_2005                       = 21313,
      CLRScalarFunction_2005              = 21318,
      InlineScalarSQLFunction_2005        = 21321,
      PartitionScheme_2005                = 21328,
      User1_2005                          = 21333,
      ServiceBrokerServiceContract_2005   = 21571,
      TriggerOnDatabase_2005              = 21572,
      CLRTableValuedFunction_2005         = 21574,
      InternalTable_2005                  = 21577,
      ServiceBrokerMessageType_2005       = 21581,
      ServiceBrokerRoute_2005             = 21586,
      Statistics_2005                     = 21587,
      User2_2005                          = 21825,
      User3_2005                          = 21827,
      User4_2005                          = 21831,
      User5_2005                          = 21843,
      User6_2005                          = 21847,
      ServiceBrokerService_2005           = 22099,
      Index_2005                          = 22601,
      CertificateLogin2_2005              = 22604,
      XMLSchema_2005                      = 22611,
      Type_2005                           = 22868
   }

   /// <summary>
   /// IDBObject interface: for storing database object information.
   /// </summary>
   public interface IDBObject
   {
      DBObjectType    Type     { get; set;}
      int             DBId     { get; set; }
      int             ObjectId { get; set; }
   }

   //--------------------------------------------------------------------------
   /// <summary>
   /// DBObject struct for storing database object information.
   /// </summary>
   //--------------------------------------------------------------------------
   public struct DBObject : IDBObject
   {
      #region Protected Fields

      private DBObjectType        type;
      private int                 objectId;
      private int                 dbId;

      #endregion

      #region Properties

      public DBObjectType Type
      {
         get
         {
            return type;
         }
         set
         {
            type = value;
         }
      }

      public int DBId
      {
         get { return dbId; }
         set { dbId = value; }
      }

      public int ObjectId
      {
         get { return objectId; }
         set { objectId = value; }
      }

      #endregion

      #region Constructor


      public DBObject ( 
         int            dbId,
         int            objectId,
         DBObjectType   type )
      {
         this.dbId = dbId;
         this.objectId = objectId;
         this.type = type;
      }

      #endregion

      #region Public Static Methods
      //-------------------------------------------------------------------------
      // Valid SQL Server type range is from 1 to 22
      //-------------------------------------------------------------------------
      public static bool IsValidType ( DBObjectType type )
      {
         switch( (int)type )
         {
            case (int)DBObjectType.Index:
            case (int)DBObjectType.Database:
            case (int)DBObjectType.UserObject:
            case (int)DBObjectType.CheckConstraint:
            case (int)DBObjectType.DefaultConstraint:
            case (int)DBObjectType.ForeignKeyConstraint:
            case (int)DBObjectType.PrimaryKeyConstraint:
            case (int)DBObjectType.StoredProcedure:
            case (int)DBObjectType.UDF:
            case (int)DBObjectType.Rule:
            case (int)DBObjectType.ReplicationFilterSP:
            case (int)DBObjectType.SystemTable :
            case (int)DBObjectType.Trigger:
            case (int)DBObjectType.InlineFunction :
            case (int)DBObjectType.TableValuedUDF:
            case (int)DBObjectType.UniqueConstraint:
            case (int)DBObjectType.UserTable:
            case (int)DBObjectType.View :
            case (int)DBObjectType.ExtendStoredProcedure:
            case (int)DBObjectType.AdHocQuery :
            case (int)DBObjectType.PreparedQuery:
            case (int)DBObjectType.Statistics:
            
               // SQL Server 2005 Object Types
            case (int)DBObjectType.CheckConstraint_2005:
            case (int)DBObjectType.DefaultConstraint_2005 :
            case (int)DBObjectType.ForeignKeyConstraint_2005: 
            case (int)DBObjectType.StoredProcedure_2005:
            case (int)DBObjectType.Rule_2005:
            case (int)DBObjectType.SystemTable_2005:
            case (int)DBObjectType.TriggerOnServer_2005: 
            case (int)DBObjectType.UserTable_2005: 
            case (int)DBObjectType.View_2005 :
            case (int)DBObjectType.ExtendedStoredProcedure_2005: 
            case (int)DBObjectType.CLRTrigger_2005:
            case (int)DBObjectType.Database_2005:
            case (int)DBObjectType.Object_2005:
            case (int)DBObjectType.FullTextCatalog_2005:
            case (int)DBObjectType.CLRStoredProcedure_2005:
            case (int)DBObjectType.Schema_2005:
            case (int)DBObjectType.Credential_2005:
            case (int)DBObjectType.DDLEvent_2005:
            case (int)DBObjectType.ManagementEvent_2005:
            case (int)DBObjectType.SecurityEvent_2005:
            case (int)DBObjectType.UserEvent_2005:
            case (int)DBObjectType.CLRAggregateFunction_2005:
            case (int)DBObjectType.InlineTableValuedSQLFunction_2005:
            case (int)DBObjectType.PartitionFunction_2005:
            case (int)DBObjectType.ReplicationFilterProcedure_2005:
            case (int)DBObjectType.TableValuedSQLFunction_2005:
            case (int)DBObjectType.ServerRole_2005:
            case (int)DBObjectType.MicrosoftWindowsGroup_2005:
            case (int)DBObjectType.AsymmetricKey_2005:
            case (int)DBObjectType.MasterKey_2005:
            case (int)DBObjectType.PrimaryKey_2005:
            case (int)DBObjectType.ObfusKey_2005:
            case (int)DBObjectType.AsymmetricKeyLogin_2005:
            case (int)DBObjectType.CertificateLogin1_2005:
            case (int)DBObjectType.Role_2005:
            case (int)DBObjectType.SQLLogin_2005:
            case (int)DBObjectType.WindowsLogin_2005:
            case (int)DBObjectType.RemoteServiceBinding_2005:
            case (int)DBObjectType.EventNotificationOnDatabase_2005:
            case (int)DBObjectType.EventNotification_2005:
            case (int)DBObjectType.ScalarSQLFunction_2005:
            case (int)DBObjectType.EventNotificationOnObject_2005:
            case (int)DBObjectType.Synonym_2005:
            case (int)DBObjectType.EndPoint_2005:
            case (int)DBObjectType.CachedAdhocQueries_2005:
            case (int)DBObjectType.CachedAdhocQueries2_2005:
            case (int)DBObjectType.ServiceBrokerServiceQueue_2005:
            case (int)DBObjectType.UniqueConstraint_2005:
            case (int)DBObjectType.ApplicationRole_2005:
            case (int)DBObjectType.Certificate_2005:
            case (int)DBObjectType.Server_2005:
            case (int)DBObjectType.TransactSQLTrigger_2005:
            case (int)DBObjectType.Assembly_2005:
            case (int)DBObjectType.CLRScalarFunction_2005:
            case (int)DBObjectType.InlineScalarSQLFunction_2005:
            case (int)DBObjectType.PartitionScheme_2005:
            case (int)DBObjectType.User1_2005:
            case (int)DBObjectType.ServiceBrokerServiceContract_2005:
            case (int)DBObjectType.TriggerOnDatabase_2005:
            case (int)DBObjectType.CLRTableValuedFunction_2005:
            case (int)DBObjectType.InternalTable_2005:
            case (int)DBObjectType.ServiceBrokerMessageType_2005:
            case (int)DBObjectType.ServiceBrokerRoute_2005:
            case (int)DBObjectType.Statistics_2005:
            case (int)DBObjectType.User2_2005 :
            case (int)DBObjectType.User3_2005:
            case (int)DBObjectType.User4_2005:
            case (int)DBObjectType.User5_2005:
            case (int)DBObjectType.User6_2005:
            case (int)DBObjectType.ServiceBrokerService_2005:
            case (int)DBObjectType.Index_2005:
            case (int)DBObjectType.CertificateLogin2_2005:
            case (int)DBObjectType.XMLSchema_2005:
            case (int)DBObjectType.Type_2005:
               return true;
            default:
               return false;
         }

      }

      public static DBObjectType GetTypeFromId( int id )
      {
         switch( id )
         {
            case 0: return DBObjectType.Index;
            case 1: return DBObjectType.Database;
            case 2: return DBObjectType.UserObject;
            case 3: return DBObjectType.CheckConstraint;
            case 4: return DBObjectType.DefaultConstraint;
            case 5: return DBObjectType.ForeignKeyConstraint;
            case 6: return DBObjectType.PrimaryKeyConstraint;
            case 7: return DBObjectType.StoredProcedure;
            case 8: return DBObjectType.UDF;
            case 9: return DBObjectType.Rule;
            case 10: return DBObjectType.ReplicationFilterSP;
            case 11: return DBObjectType.SystemTable ;
            case 12: return DBObjectType.Trigger;
            case 13: return DBObjectType.InlineFunction ;
            case 14: return DBObjectType.TableValuedUDF;
            case 15: return DBObjectType.UniqueConstraint;
            case 16: return DBObjectType.UserTable;
            case 17: return DBObjectType.View ;
            case 18: return DBObjectType.ExtendStoredProcedure;
            case 19: return DBObjectType.AdHocQuery ;
            case 20: return DBObjectType.PreparedQuery;
            case 21: return DBObjectType.Statistics;
            
               // SQL Server 2005 Object Types
            case 22: return DBObjectType.CheckConstraint_2005;
            case 23: return DBObjectType.DefaultConstraint_2005 ;
            case 24: return DBObjectType.ForeignKeyConstraint_2005;
            case 25: return DBObjectType.StoredProcedure_2005;
            case 26: return DBObjectType.Rule_2005;
            case 27: return DBObjectType.SystemTable_2005;
            case 28: return DBObjectType.TriggerOnServer_2005;
            case 29: return DBObjectType.UserTable_2005;
            case 30: return DBObjectType.View_2005 ;
            case 31: return DBObjectType.ExtendedStoredProcedure_2005;
            case 32: return DBObjectType.CLRTrigger_2005;
            case 33: return DBObjectType.Database_2005;
            case 34: return DBObjectType.Object_2005;
            case 35: return DBObjectType.FullTextCatalog_2005;
            case 36: return DBObjectType.CLRStoredProcedure_2005;
            case 37: return DBObjectType.Schema_2005;
            case 38: return DBObjectType.Credential_2005;
            case 39: return DBObjectType.DDLEvent_2005;
            case 40: return DBObjectType.ManagementEvent_2005;
            case 41: return DBObjectType.SecurityEvent_2005;
            case 42: return DBObjectType.UserEvent_2005;
            case 43: return DBObjectType.CLRAggregateFunction_2005;
            case 44: return DBObjectType.InlineTableValuedSQLFunction_2005;
            case 45: return DBObjectType.PartitionFunction_2005;
            case 46: return DBObjectType.ReplicationFilterProcedure_2005;
            case 47: return DBObjectType.TableValuedSQLFunction_2005;
            case 48: return DBObjectType.ServerRole_2005;
            case 49: return DBObjectType.MicrosoftWindowsGroup_2005;
            case 50: return DBObjectType.AsymmetricKey_2005;
            case 51: return DBObjectType.MasterKey_2005;
            case 52: return DBObjectType.PrimaryKey_2005;
            case 53: return DBObjectType.ObfusKey_2005;
            case 54: return DBObjectType.AsymmetricKeyLogin_2005;
            case 55: return DBObjectType.CertificateLogin1_2005;
            case 56: return DBObjectType.Role_2005;
            case 57: return DBObjectType.SQLLogin_2005;
            case 58: return DBObjectType.WindowsLogin_2005;
            case 59: return DBObjectType.RemoteServiceBinding_2005;
            case 60: return DBObjectType.EventNotificationOnDatabase_2005;
            case 61: return DBObjectType.EventNotification_2005;
            case 62: return DBObjectType.ScalarSQLFunction_2005;
            case 63: return DBObjectType.EventNotificationOnObject_2005;
            case 64: return DBObjectType.Synonym_2005;
            case 65: return DBObjectType.EndPoint_2005;
            case 66: return DBObjectType.CachedAdhocQueries_2005;
            case 67: return DBObjectType.CachedAdhocQueries2_2005;
            case 68: return DBObjectType.ServiceBrokerServiceQueue_2005;
            case 69: return DBObjectType.UniqueConstraint_2005;
            case 70: return DBObjectType.ApplicationRole_2005;
            case 71: return DBObjectType.Certificate_2005;
            case 72: return DBObjectType.Server_2005;
            case 73: return DBObjectType.TransactSQLTrigger_2005;
            case 74: return DBObjectType.Assembly_2005;
            case 75: return DBObjectType.CLRScalarFunction_2005;
            case 76: return DBObjectType.InlineScalarSQLFunction_2005;
            case 77: return DBObjectType.PartitionScheme_2005;
            case 78: return DBObjectType.User1_2005;
            case 79: return DBObjectType.ServiceBrokerServiceContract_2005;
            case 80: return DBObjectType.TriggerOnDatabase_2005;
            case 81: return DBObjectType.CLRTableValuedFunction_2005;
            case 82: return DBObjectType.InternalTable_2005;
            case 83: return DBObjectType.ServiceBrokerMessageType_2005;
            case 84: return DBObjectType.ServiceBrokerRoute_2005;
            case 85: return DBObjectType.Statistics_2005;
            case 86: return DBObjectType.User2_2005 ;
            case 87: return DBObjectType.User3_2005;
            case 88: return DBObjectType.User4_2005;
            case 89: return DBObjectType.User5_2005;
            case 90: return DBObjectType.User6_2005;
            case 91: return DBObjectType.ServiceBrokerService_2005;
            case 92: return DBObjectType.Index_2005;
            case 93: return DBObjectType.CertificateLogin2_2005;
            case 94: return DBObjectType.XMLSchema_2005;
            case 95: return DBObjectType.Type_2005;
            default:
               throw( new ArgumentException( String.Format( "Invalid object type ID {0}", id),
                  "id" ) );
         }
      }

      public static int GetIdFromType( DBObjectType type )
      {
         switch( (int)type )
         {
            case (int)DBObjectType.Index:
               return 0;
            case (int)DBObjectType.Database:
               return 1;
            case (int)DBObjectType.UserObject:
               return 2;
            case (int)DBObjectType.CheckConstraint:
               return 3;
            case (int)DBObjectType.DefaultConstraint:
               return 4;
            case (int)DBObjectType.ForeignKeyConstraint:
               return 5;
            case (int)DBObjectType.PrimaryKeyConstraint:
               return 6;
            case (int)DBObjectType.StoredProcedure:
               return 7;
            case (int)DBObjectType.UDF:
               return 8;
            case (int)DBObjectType.Rule:
               return 9;
            case (int)DBObjectType.ReplicationFilterSP:
               return 10;
            case (int)DBObjectType.SystemTable :
               return 11;
            case (int)DBObjectType.Trigger:
               return 12;
            case (int)DBObjectType.InlineFunction :
               return 13;
            case (int)DBObjectType.TableValuedUDF:
               return 14;
            case (int)DBObjectType.UniqueConstraint:
               return 15;
            case (int)DBObjectType.UserTable:
               return 16;
            case (int)DBObjectType.View :
               return 17;
            case (int)DBObjectType.ExtendStoredProcedure:
               return 18;
            case (int)DBObjectType.AdHocQuery :
               return 19;
            case (int)DBObjectType.PreparedQuery:
               return 20;
            case (int)DBObjectType.Statistics:
               return 21;

               // SQL Server 2005 Object Types
            case (int)DBObjectType.CheckConstraint_2005:
               return 22;
            case (int)DBObjectType.DefaultConstraint_2005 :
               return 23;
            case (int)DBObjectType.ForeignKeyConstraint_2005: 
               return 24;
            case (int)DBObjectType.StoredProcedure_2005:
               return 25;
            case (int)DBObjectType.Rule_2005:
               return 26;
            case (int)DBObjectType.SystemTable_2005:
               return 27;
            case (int)DBObjectType.TriggerOnServer_2005: 
               return 28;
            case (int)DBObjectType.UserTable_2005: 
               return 29;
            case (int)DBObjectType.View_2005 :
               return 30;
            case (int)DBObjectType.ExtendedStoredProcedure_2005: 
               return 31;
            case (int)DBObjectType.CLRTrigger_2005:
               return 32;
            case (int)DBObjectType.Database_2005:
               return 33;
            case (int)DBObjectType.Object_2005:
               return 34;
            case (int)DBObjectType.FullTextCatalog_2005:
               return 35;
            case (int)DBObjectType.CLRStoredProcedure_2005:
               return 36;
            case (int)DBObjectType.Schema_2005:
               return 37;
            case (int)DBObjectType.Credential_2005:
               return 38;
            case (int)DBObjectType.DDLEvent_2005:
               return 39;
            case (int)DBObjectType.ManagementEvent_2005:
               return 40;
            case (int)DBObjectType.SecurityEvent_2005:
               return 41;
            case (int)DBObjectType.UserEvent_2005:
               return 42;
            case (int)DBObjectType.CLRAggregateFunction_2005:
               return 43;
            case (int)DBObjectType.InlineTableValuedSQLFunction_2005:
               return 44;
            case (int)DBObjectType.PartitionFunction_2005:
               return 45;
            case (int)DBObjectType.ReplicationFilterProcedure_2005:
               return 46;
            case (int)DBObjectType.TableValuedSQLFunction_2005:
               return 47;
            case (int)DBObjectType.ServerRole_2005:
               return 48;
            case (int)DBObjectType.MicrosoftWindowsGroup_2005:
               return 49;
            case (int)DBObjectType.AsymmetricKey_2005:
               return 50;
            case (int)DBObjectType.MasterKey_2005:
               return 51;
            case (int)DBObjectType.PrimaryKey_2005:
               return 52;
            case (int)DBObjectType.ObfusKey_2005:
               return 53;
            case (int)DBObjectType.AsymmetricKeyLogin_2005:
               return 54;
            case (int)DBObjectType.CertificateLogin1_2005:
               return 55;
            case (int)DBObjectType.Role_2005:
               return 56;
            case (int)DBObjectType.SQLLogin_2005:
               return 57;
            case (int)DBObjectType.WindowsLogin_2005:
               return 58;
            case (int)DBObjectType.RemoteServiceBinding_2005:
               return 59;
            case (int)DBObjectType.EventNotificationOnDatabase_2005:
               return 60;
            case (int)DBObjectType.EventNotification_2005:
               return 61;
            case (int)DBObjectType.ScalarSQLFunction_2005:
               return 62;
            case (int)DBObjectType.EventNotificationOnObject_2005:
               return 63;
            case (int)DBObjectType.Synonym_2005:
               return 64;
            case (int)DBObjectType.EndPoint_2005:
               return 65;
            case (int)DBObjectType.CachedAdhocQueries_2005:
               return 66;
            case (int)DBObjectType.CachedAdhocQueries2_2005:
               return 67;
            case (int)DBObjectType.ServiceBrokerServiceQueue_2005:
               return 68;
            case (int)DBObjectType.UniqueConstraint_2005:
               return 69;
            case (int)DBObjectType.ApplicationRole_2005:
               return 70;
            case (int)DBObjectType.Certificate_2005:
               return 71;
            case (int)DBObjectType.Server_2005:
               return 72;
            case (int)DBObjectType.TransactSQLTrigger_2005:
               return 73;
            case (int)DBObjectType.Assembly_2005:
               return 74;
            case (int)DBObjectType.CLRScalarFunction_2005:
               return 75;
            case (int)DBObjectType.InlineScalarSQLFunction_2005:
               return 76;
            case (int)DBObjectType.PartitionScheme_2005:
               return 77;
            case (int)DBObjectType.User1_2005:
               return 78;
            case (int)DBObjectType.ServiceBrokerServiceContract_2005:
               return 79;
            case (int)DBObjectType.TriggerOnDatabase_2005:
               return 80;
            case (int)DBObjectType.CLRTableValuedFunction_2005:
               return 81;
            case (int)DBObjectType.InternalTable_2005:
               return 82;
            case (int)DBObjectType.ServiceBrokerMessageType_2005:
               return 83;
            case (int)DBObjectType.ServiceBrokerRoute_2005:
               return 84;
            case (int)DBObjectType.Statistics_2005:
               return 85;
            case (int)DBObjectType.User2_2005 :
               return 86;
            case (int)DBObjectType.User3_2005:
               return 87;
            case (int)DBObjectType.User4_2005:
               return 88;
            case (int)DBObjectType.User5_2005:
               return 89;
            case (int)DBObjectType.User6_2005:
               return 90;
            case (int)DBObjectType.ServiceBrokerService_2005:
               return 91;
            case (int)DBObjectType.Index_2005:
               return 92;
            case (int)DBObjectType.CertificateLogin2_2005:
               return 93;
            case (int)DBObjectType.XMLSchema_2005:
               return 94;
            case (int)DBObjectType.Type_2005:
               return 95;
            default:
               throw( new ArgumentException( String.Format( "Invalid object type {0}", (int)type),
                  "type" ) );

         }

      }

      public static bool IsDatabae( int type )
      {
         switch( type )
         {
            case (int)DBObjectType.Database:
            case (int)DBObjectType.Database_2005:
               return true;
            default:
               return false;
         }
      }

      public static bool IsUserTable( int type )
      {
         switch( type )
         {
            case (int)DBObjectType.UserTable:
            case (int)DBObjectType.UserTable_2005:
               return true;
            default:
               return false;
         }
      }

      public static DBObjectType GetUserTableTypeValue( int sqlVersion )
      {
         switch( sqlVersion )
         {
            case 8:  // SQL 2000
               return DBObjectType.UserTable;
            case 9:  // SQL 2005
            case 10: // SQL 2008
            case 11: // SQL 2012
            case 12: // SQL 2014
            case 13: // SQL 2016  Date 7/15/2016 4.1.7. Support SQL Server 2016
			case 14: // SQL 2017
               return DBObjectType.UserTable_2005;
            default:
               return DBObjectType.UserTable;
         }
      }

      public static DBObjectType GetSystemTableTypeValue( int sqlVersion )
      {
         switch( sqlVersion )
         {
            case 8:  // SQL 2000
               return DBObjectType.SystemTable;
            case 9:  // SQL 2005
            case 10: // SQL 2008
            case 11: // SQL 2012
            case 12: // SQL 2014
            case 13: // SQL 2016  Date 7/15/2016 4.1.7. Support SQL Server 2016
			case 14: // SQL 2017 
               return DBObjectType.SystemTable_2005;
            default:
               return DBObjectType.SystemTable;
         }
      }

      public static DBObjectType [] GetDmlOtherTypeValues( int sqlVersion )
      {
         ArrayList objectTypes = new ArrayList();
         switch( sqlVersion )
         {
            case 8:  // SQL 2000
               objectTypes.Add( DBObjectType.View );
               objectTypes.Add( DBObjectType.Index );
               objectTypes.Add( DBObjectType.Database );
               objectTypes.Add( DBObjectType.UserObject );
               objectTypes.Add( DBObjectType.CheckConstraint );
               objectTypes.Add( DBObjectType.DefaultConstraint );
               objectTypes.Add( DBObjectType.ForeignKeyConstraint );
               objectTypes.Add( DBObjectType.PrimaryKeyConstraint );
               objectTypes.Add( DBObjectType.UDF );
               objectTypes.Add( DBObjectType.Rule );
               objectTypes.Add( DBObjectType.ReplicationFilterSP );
               objectTypes.Add( DBObjectType.Trigger );
               objectTypes.Add( DBObjectType.InlineFunction );
               objectTypes.Add( DBObjectType.TableValuedUDF );
               objectTypes.Add( DBObjectType.UniqueConstraint );
               objectTypes.Add( DBObjectType.AdHocQuery );
               objectTypes.Add( DBObjectType.PreparedQuery );
               objectTypes.Add( DBObjectType.Statistics );
               break;
            case 9:  // SQL 2005 expand the number of types and we might need to rework this
            case 10: // SQL 2008
            case 11: // SQL 2012
            case 12: // SQL 2014
            case 13: // SQL 2016 Date 7/15/2016 4.1.7. Support SQL Server 2016
			case 14: // SQL 2017
               // object type selection.
               objectTypes.Add( DBObjectType.View_2005 );
               objectTypes.Add( DBObjectType.Index_2005 );
               objectTypes.Add( DBObjectType.Database_2005 );
               objectTypes.Add( DBObjectType.User1_2005 );
               objectTypes.Add( DBObjectType.User2_2005 );
               objectTypes.Add( DBObjectType.User3_2005 );
               objectTypes.Add( DBObjectType.User4_2005 );
               objectTypes.Add( DBObjectType.User5_2005 );
               objectTypes.Add( DBObjectType.User6_2005 );
               objectTypes.Add( DBObjectType.CheckConstraint_2005 );
               objectTypes.Add( DBObjectType.DefaultConstraint_2005 );
               objectTypes.Add( DBObjectType.ForeignKeyConstraint_2005 );
               objectTypes.Add( DBObjectType.PrimaryKey_2005 );
               objectTypes.Add( DBObjectType.ReplicationFilterProcedure_2005 );
               objectTypes.Add( DBObjectType.UniqueConstraint_2005 );
               objectTypes.Add( DBObjectType.Statistics_2005 );
               objectTypes.Add( DBObjectType.Rule_2005 );
               objectTypes.Add( DBObjectType.TriggerOnDatabase_2005 );
               objectTypes.Add( DBObjectType.TriggerOnServer_2005 );
               objectTypes.Add( DBObjectType.CLRTrigger_2005 );
               objectTypes.Add( DBObjectType.TransactSQLTrigger_2005 );
               objectTypes.Add( DBObjectType.CLRAggregateFunction_2005 );
               objectTypes.Add( DBObjectType.InlineTableValuedSQLFunction_2005 );
               objectTypes.Add( DBObjectType.PartitionFunction_2005 );
               objectTypes.Add( DBObjectType.TableValuedSQLFunction_2005 );
               objectTypes.Add( DBObjectType.ScalarSQLFunction_2005 );
               objectTypes.Add( DBObjectType.CachedAdhocQueries2_2005 );
               objectTypes.Add( DBObjectType.CachedAdhocQueries_2005 );
               objectTypes.Add( DBObjectType.CLRScalarFunction_2005 );
               objectTypes.Add( DBObjectType.InlineScalarSQLFunction_2005 );
               objectTypes.Add( DBObjectType.CLRTableValuedFunction_2005 );
               objectTypes.Add( DBObjectType.InternalTable_2005 );
               objectTypes.Add( DBObjectType.XMLSchema_2005 );
               objectTypes.Add( DBObjectType.PartitionScheme_2005 );
               objectTypes.Add( DBObjectType.Type_2005 );
               objectTypes.Add( DBObjectType.Object_2005 );
               objectTypes.Add( DBObjectType.FullTextCatalog_2005 );
               objectTypes.Add( DBObjectType.Schema_2005 );
               objectTypes.Add( DBObjectType.AsymmetricKey_2005 );
               objectTypes.Add( DBObjectType.MasterKey_2005 );
               objectTypes.Add( DBObjectType.Credential_2005 );
               objectTypes.Add( DBObjectType.DDLEvent_2005 );
               objectTypes.Add( DBObjectType.ManagementEvent_2005 );
               objectTypes.Add( DBObjectType.SecurityEvent_2005 );
               objectTypes.Add( DBObjectType.UserEvent_2005 );
               objectTypes.Add( DBObjectType.ServerRole_2005 );
               objectTypes.Add( DBObjectType.Role_2005 );
               objectTypes.Add( DBObjectType.EventNotification_2005 );
               objectTypes.Add( DBObjectType.EventNotificationOnDatabase_2005 );
               objectTypes.Add( DBObjectType.EventNotificationOnObject_2005 );
               objectTypes.Add( DBObjectType.Synonym_2005 );
               objectTypes.Add( DBObjectType.EndPoint_2005 );
               objectTypes.Add( DBObjectType.ServiceBrokerServiceQueue_2005 );
               objectTypes.Add( DBObjectType.Certificate_2005 );
               objectTypes.Add( DBObjectType.Assembly_2005 );
               objectTypes.Add( DBObjectType.ServiceBrokerServiceContract_2005 );
               objectTypes.Add( DBObjectType.ServiceBrokerRoute_2005 );
               objectTypes.Add( DBObjectType.ServiceBrokerMessageType_2005 );
               objectTypes.Add( DBObjectType.ServiceBrokerService_2005 );
               objectTypes.Add( DBObjectType.CertificateLogin1_2005 );
               objectTypes.Add( DBObjectType.CertificateLogin2_2005 );
               objectTypes.Add( DBObjectType.WindowsLogin_2005 );
               objectTypes.Add( DBObjectType.SQLLogin_2005 );
               objectTypes.Add( DBObjectType.AsymmetricKeyLogin_2005 );
               objectTypes.Add( DBObjectType.MicrosoftWindowsGroup_2005 );
               break;
         }
         return (DBObjectType [])objectTypes.ToArray( typeof(DBObjectType) );
      }

      public static DBObjectType [] GetSPTypeValues( int sqlVersion )
      {
         ArrayList objectTypes = new ArrayList();
         switch( sqlVersion )
         {
            case 9:  // SQL 2005
            case 10: // SQL 2008
            case 11: // SQL 2012
            case 12: // SQL 2014
            case 13: // SQL 2016 Date 7/15/2016 4.1.7. Support SQL Server 2016
			case 14: // SQL 2017
               objectTypes.Add(DBObjectType.StoredProcedure_2005);
               objectTypes.Add( DBObjectType.ExtendedStoredProcedure_2005 );
               objectTypes.Add( DBObjectType.CLRStoredProcedure_2005 );
               break;
            case 8:  // SQL 2000
            default:
               objectTypes.Add( DBObjectType.StoredProcedure );
               objectTypes.Add( DBObjectType.ExtendStoredProcedure );
               break;
         }
         return (DBObjectType [])objectTypes.ToArray( typeof(DBObjectType) );
      }
      #endregion

   }

}
