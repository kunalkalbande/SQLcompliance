using System;
using System.Collections;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Idera.SQLcompliance.Core.Event
{
   #region Enums
   
   public enum TraceEventCategory : int
   {
      InvalidCategory               = -1,
      Corrupted                     = 0,
      Login                         = 1,
      DDL                           = 2,
      Security                      = 3,
      DML                           = 4,
      SELECT                        = 5,
      Admin                         = 6,
      Audit                         = 7,
      Broker                        = 8,
      UserDefined                   = 9,
       Server = 10
   }
   
   public enum TraceEventType : int
   {
      InvalidType                            = -1,
      SELECT                                 =  1,
      UPDATE                                 =  2,
      REFERENCES                             =  4,
      INSERT                                 =  8,
      DELETE                                 =  16,
      EXECUTE                                =  32,
      BeginTran                              =  40,
      CommitTran                             =  41,
      RollbackTran                           =  42,
      SaveTran                               =  43,
      Login                                  =  50,
      LoginFailed                            =  51,
      ServerImpersonation                    =  52,
      DatabaseImersonation                   =  53,
      DisableLogin                           =  54,
      EnableLogin                            =  55,
      Backup                                 =  62,
      Restore                                =  63,
      BackupDatabase                         =  64,
      BackupLog                              =  65,
      BackupTable                            =  66,
      AuditStarted                           =  71,
      AuditStopped                           =  72,
      DBCC                                   =  80,
      DBCC_read                              =  81,
      DBCC_write                             =  82,
      ServerOperation                        =  90,
      DatabaseOperation                      =  91,
      ServerAlterTrace                       =  92,
      BrokerConversation                     =  93,
      BrokerLogin                            =  94,
      CreateBase                             =  100, /* Note that CREATE should be saved through 299 */
      CreateIndex                            =  101,
      CreateDatabase                         =  102,
      CreateUserObject                       =  103,
      CreateCHECK                            =  104,
      CreateDEFAULT                          =  105,
      CreateFOREIGNKEY                       =  106,
      CreatePRIMARYKEY                       =  107,
      CreateStoredProcedure                  =  108,
      CreateUDF                              =  109,
      CreateRule                             =  110,
      CreateReplFilterStoredProc             =  111,
      CreateSystemTable                      =  112,
      CreateTrigger                          =  113,
      CreateInlineFunction                   =  114,
      CreateTableValuedUDF                   =  115,
      CreateUNIQUE                           =  116,
      CreateUserTable                        =  117,
      CreateView                             =  118,
      CreateExtStoredProc                    =  119,
      CreateAdHocQuery                       =  120,
      CreatePreparedQuery                    =  121,
      CreateStatistics                       =  122,
      AlterBase                              =  200, /* Note that ALTER should be saved through 299 */
        DropBase                               =  300, /* Note that DROP should be saved through 299 */
                                                     /* New SQL Server 2005 verb spaces are below*/
      GrantObjectGdrBase                     =  400,
      DenyObjectGdrBase                      =  500,
      RevokeObjectGdrBase                    =  600,
      AddDatabaseUser                        =  700,
      DropDatabaseUser                       =  701,
      GrantDatabaseAccess                    =  702,
      RevokeDatabaseAccess                   =  703,
      AddLogintoServerRole                   =  704,
      DropLoginfromServerRole                =  705,
      AddMembertoDatabaseRole                =  706,
      DropMembertoDatabaseRole               =  707,
      AddDatabaseRole                        =  708,
      DropDatabaseRole                       =  709,
      AddLogin                               =  710,
      DropLogin                              =  711,
      AppRoleChangePassword                  =  712,
      PasswordChangeSelf                     =  713,
      PasswordChange                         =  714,
      LoginChangePropertyDB                  =  715,
      LoginChangePropertyLanguage            =  716,
      GrantLogin                             =  717,
      RevokeLogin                            =  718,
      DenyLogin                              =  719,
      ServerObjectChangeOwner                =  720,
      ChangeDatabaseOwner                    =  721,
      DatabaseObjectChangeOwner              =  722,
      SchemaObjectChangeOwner                =  723,
      CredentialMapped                       =  724,
      CredentialMapDropped                   =  725,
      
      GrantStmtBase                          =  1000,
      GrantCREATEDATABASE                    =  1001,
      GrantCREATETABLE                       =  1002,
      GrantCREATEPROCEDURE                   =  1004,
      GrantCREATEVIEW                        =  1008,
      GrantCREATERULE                        =  1016,
      GrantCREATEDEFAULT                     =  1032,
      GrantBACKUPDATABASE                    =  1064,
      GrantBACKUPLOG                         =  1028,
      GrantCREATEFUNCTION                    =  1012,
      
      DenyStmtBase                           =  1100,
      DenyCREATEDATABASE                     =  1101,
      DenyCREATETABLE                        =  1102,
      DenyCREATEPROCEDURE                    =  1104,
      DenyCREATEVIEW                         =  1108,
      DenyCREATERULE                         =  1116,
      DenyCREATEDEFAULT                      =  1132,
      DenyBACKUPDATABASE                     =  1164,
      DenyBACKUPLOG                          =  1128,
      DenyCREATEFUNCTION                     =  1112,
      
      RevokeStmtBase                         =  1200,
      RevokeCREATEDATABASE                   =  1201,
      RevokeCREATETABLE                      =  1202,
      RevokeCREATEPROCEDURE                  =  1204,
      RevokeCREATEVIEW                       =  1208,
      RevokeCREATERULE                       =  1216,
      RevokeCREATEDEFAULT                    =  1232,
      RevokeBACKUPDATABASE                   =  1264,
      RevokeBACKUPLOG                        =  1228,
      RevokeCREATEFUNCTION                   =  1212,
      
      DumpBase                               =  1300,
      OpenBase                               =  1400,
      LoadBase                               =  1500,
      AccessBase                             =  1600,
      TransferBase                           =  1700,

      UserDefinedEvent0                      =  1800,
      UserDefinedEvent1                      =  1801,
      UserDefinedEvent2                      =  1802,
      UserDefinedEvent3                      =  1803,
      UserDefinedEvent4                      =  1804,
      UserDefinedEvent5                      =  1805,
      UserDefinedEvent6                      =  1806,
      UserDefinedEvent7                      =  1807,
      UserDefinedEvent8                      =  1808,
      UserDefinedEvent9                      =  1809,

      Logout                                 =  1810,

      EncryptedDML                           =  900001,
      DummyEvent                             =  999996,
      MissingEvents                          =  999997,
      InsertedEvent                          =  999998,
      ModifiedEvent                          =  999999,

      CreateServerRole                       =  138,
      DropServerRole                         =  338,

      CreateLinkedServer                     =  158,
      DeleteLinkedServer                     =  358,

       ServerStart = 182,
       ServerStop = 181,
       AlterUserTable = 217,  // SQLCM-3040 Create and drop index events are recorded as 'Alter User Table' event type
        DropIndex = 301
    }

    #endregion

    //----------------------------------------------------------------------------
    // Class: EventRecord
    //----------------------------------------------------------------------------
    [Serializable]
   public class EventRecord : ISerializable
	{
      #region Constants
      private const int MaxObjNameLen         = 128;
      private const int MaxTargetObjNameLen   = 512;
      private const int MaxDetailLen          = 512;
      
      #endregion

		#region Properties

      //------------------------------------------------------
      // V 1.1 and 1.2 fields
      //------------------------------------------------------

      // calculated
      public TraceEventType          eventType;
      public TraceEventCategory      eventCategory;
      
      public string                  eventTypeString;
      public string                  eventCategoryString;
      public string                  privilegedUsers;
      
      public string                  targetObject   = "";
      public string                  details        = "";
      public int                     checksum       = 0;
      public int                     hash           = 0;
      public int                     privilegedUser = 0;
      public int                     alertLevel     = 0;
      public int                     eventId        = 0;
      public long                    rowcount       = 0;

      // from original trace file      
      public int                     eventClass;
      public int                     eventSubclass;
      public DateTime                startTime;
      public int                     spid;
      public string                  applicationName;
      public string                  hostName;
      public string                  serverName;
      public string                  loginName;
      public int                     success;
      public string                  databaseName;
      public int                     databaseId;
      public string                  dbUserName;
      public int                     objectType;
      public string                  objectName;
      public int                     objectId;
      public int                     permissions;
      public int                     columnPermissions;
      public string                  targetLoginName;
      public string                  targetUserName;
      public string                  roleName;
      public string                  ownerName;

      // private properties
      public SqlConnection           connection = null;
      string                         serverDB   = "";
      
      //------------------------------------------------------
      // V 2.0 fields
      //------------------------------------------------------
      internal int                   classVersion     = CoreConstants.SerializationVersion;

      // sqlserver 2005
      public string                  fileName         = "";
      public string                  linkedServerName = "";
      public string                  parentName       = "";
      public int                     isSystem         = 0;
      public string                  sessionLoginName = "";
      public string                  providerName     = "";

      //------------------------------------------------------
      // V 3.0 fields
      //------------------------------------------------------
      public int appNameId = -2100000000;
      public int hostId = -2100000000;
      public int loginId = -2100000000;

      //------------------------------------------------------
      // V 3.1 fields
      //------------------------------------------------------
      public long startSequence = -1;
      public long endSequence = -1 ;
      public DateTime endTime = CoreConstants.InvalidDateTimeValue;

      // V 5.5 field
      public long? rowCounts = null;

      // This property retrieves the user that is ultimately responsible for causing
      //  this event to happen.  Therefore, in impersonation cases, the sessionLogin
      //  will be returned.
      public string ResponsibleLogin
      {
         get
         {
            if(sessionLoginName != null && sessionLoginName.Trim().Length > 0)
               return sessionLoginName ;
            else
               return loginName ;
         }
      }

		#endregion

	   #region Constructor
	   
	   //-------------------------------------------------------------------------
	   // EventRecord - Constructor
	   //-------------------------------------------------------------------------
		public
		   EventRecord(
		      SqlConnection     inConnection,
		      string            inServerDB
		   )
		{
		   connection  = inConnection;
		   serverDB    = inServerDB;
		}
		
		public
		   EventRecord()
		{
		}

      public EventRecord(
         SerializationInfo    info,
         StreamingContext      context )
      {
         // V 1.1 and 1.2 fields
         // calculated
         eventType = (TraceEventType)info.GetValue( "eventType", typeof(TraceEventType) );
         eventCategory = (TraceEventCategory)info.GetValue( "eventCategory", typeof(TraceEventCategory) );
      
         eventTypeString = info.GetString( "eventTypeString" );
         eventCategoryString = info.GetString( "eventCategoryString" );
      
         targetObject = info.GetString( "targetObject" );
         details = info.GetString( "details" );
         checksum = info.GetInt32( "checksum" );
         hash = info.GetInt32( "hash" );
         privilegedUser = info.GetInt32( "privilegedUser" );
         alertLevel = info.GetInt32( "alertLevel" );
         eventId = info.GetInt32( "eventId" );

         // from original trace file      
         eventClass = info.GetInt32( "eventClass" );
         eventSubclass = info.GetInt32( "eventSubclass" );
         spid = info.GetInt32( "spid" );
         applicationName = info.GetString( "applicationName" );
         hostName = info.GetString( "hostName" );
         serverName = info.GetString( "serverName" );
         loginName = info.GetString( "loginName" );
         success = info.GetInt32( "success" );
         databaseName = info.GetString( "databaseName" );
         databaseId = info.GetInt32( "databaseId" );
         dbUserName = info.GetString( "dbUserName" );
         objectType = info.GetInt32( "objectType" );
         objectName = info.GetString( "objectName" );
         objectId = info.GetInt32( "objectId" );
         permissions = info.GetInt32( "permissions" );
         columnPermissions = info.GetInt32( "columnPermissions" );
         targetLoginName = info.GetString( "targetLoginName" );
         targetUserName = info.GetString( "targetUserName" );
         roleName = info.GetString( "roleName" );
         ownerName = info.GetString( "ownerName" );
         if (privilegedUser != 0)
         {
             privilegedUsers = loginName;
         }

         // private properties
         connection = null;
         serverDB = info.GetString( "serverDB" );

         // V 2.0 fields
         try
         {
            classVersion = info.GetInt32( "classVersion" );
         }
         catch
         {
            classVersion = 0;
         }

         if( classVersion >= CoreConstants.SerializationVersion_20 )
         {
            // DateTime serialization is changed since 2.0
            startTime = new DateTime( info.GetInt64( "startTicks" ) );
            // sqlserver 2005 and V 2.0 fields
            fileName = info.GetString( "fileName" );
            linkedServerName = info.GetString( "linkedServerName" );
            parentName = info.GetString( "parentName" );
            isSystem = info.GetInt32( "isSystem" );
            sessionLoginName = info.GetString( "sessionLoginName" );
            providerName = info.GetString( "providerName" );
         }
         else
         {
            startTime = info.GetDateTime( "startTime" );
            // Provide default values for new fields added since 2.0
         }
         
         if( classVersion >= CoreConstants.SerializationVersion_30 )
         {
            appNameId = info.GetInt32( "appNameId" );
            hostId = info.GetInt32( "hostId" );
            loginId = info.GetInt32( "loginId" );
         }

         if (classVersion >= CoreConstants.SerializationVersion_31)
         {
            endTime = info.GetDateTime("endTime");
            startSequence = info.GetInt64( "startSequence");
            endSequence = info.GetInt64("endSequence");
         }
         if (classVersion >= CoreConstants.SerializationVersion_55)
         {
             Object rowCountData = info.GetValue("rowCounts", typeof(long));
             if (rowCountData != null)
             {
                 rowCounts = (long)rowCountData;
             }
             else
             {
                 rowCounts = null;
             }
         }
      }
		
		#endregion

		#region LastError
		
	   static string           errMsg   = "";
	   static public  string   GetLastError() { return errMsg; } 
	   
	   #endregion
		
      #region Public Read Methods

	   //-----------------------------------------------------------------------------
	   // Read - reads event record
	   //-----------------------------------------------------------------------------
	   public bool
	      Read(
	         int               inEventType,
	         DateTime          inStartTime,
	         int               inSpid
         )
	   {
	      return Read( connection,
	                   serverDB,
	                   inEventType,
	                   inStartTime,
	                   inSpid );
      }
	   //-----------------------------------------------------------------------------
	   // Read - reads event record
	   //-----------------------------------------------------------------------------
	   public bool
	      Read(
            SqlConnection     inConnection,
            string            inServerDB,
	         int               inEventType,
	         DateTime          inStartTime,
	         int               inSpid
         )
	   {
         string where = String.Format( "e.eventType={0} AND e.startTime={1} AND e.spid={2}",
	                                    inEventType,
			                              SQLHelpers.CreateSafeDateTimeString(inStartTime),
			                              inSpid );
			return InternalRead( inConnection,
			                     inServerDB,
			                     where );
      }
      
	   //-----------------------------------------------------------------------------
	   // Read - reads event record
	   //-----------------------------------------------------------------------------
	   public bool
	      Read(
	         int               inEventId
         )
	   {
	      bool           retval = false;
	      
	      try
	      {
              string cmdstr = string.Empty;

                string where = String.Format("eventId={0}", inEventId);

                cmdstr = GetSelectSQL(serverDB,
                                       where,
                                       "");
                using (SqlCommand cmd = new SqlCommand(cmdstr, connection))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Load(reader);
                            retval = true;
                        }
                        else
                        {
                            errMsg = CoreConstants.Error_NoEventDataAvailable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return retval;
        }


	   //-----------------------------------------------------------------------------
	   // GetEventRecords  
	   //-----------------------------------------------------------------------------
      static public ICollection
         GetEventRecords(
            SqlConnection     inConnection,
            string            inServerDB,
            string            whereClause
         )
      {
         IList eventList;
         
			try
			{
			   string cmdStr = GetSelectSQL( inServerDB,
			                                 whereClause,
			                                 "" );
			   
			   using ( SqlCommand    cmd    = new SqlCommand( cmdStr,
			                                                  inConnection ) )
            {			                                                  
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
				      eventList = new ArrayList();

			         while ( reader.Read() )
                  {
                     EventRecord evRec = new EventRecord();
                     evRec.Load( reader );

                     // Add to list               
                     eventList.Add( evRec );
                  }
               }
            }
			}
			catch( Exception ex )
			{
			   errMsg = ex.Message;
				eventList = new ArrayList();  // return an empty array
			}
			
			return eventList;
	   }
	   
	   #endregion
	   
      #region Public Insert Methods
	   
	   //-----------------------------------------------------------------------------
	   // Insert - Insert record into events table
	   //-----------------------------------------------------------------------------
	   public bool
	      Insert(
	         SqlConnection     inConnection,
	         string            inDatabaseName
	      )
	   {
	      return Insert( inConnection, inDatabaseName, null );
	   }

	   //-----------------------------------------------------------------------------
	   // Insert - Insert record into events table
	   //-----------------------------------------------------------------------------
	   public bool
	      Insert(
	         SqlConnection     inConnection,
	         string            inDatabaseName,
	         SqlTransaction    inTransaction
	      )
	   {
	      bool inserted ;
	      string sqlCmd = GetInsertSQL(inDatabaseName);
   	   
	      using ( SqlCommand cmd = new SqlCommand( sqlCmd, 
                                                  inConnection ) )
         {
	         cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      
            if ( inTransaction != null )                                            
               cmd.Transaction = inTransaction;
            
            cmd.ExecuteNonQuery();
            inserted = true;
         }
         return inserted;
	   }
	   
      
      #endregion

      #region Update Methods
      public bool
         Update()
      {
         bool   retval = true;

         try
			{
			   string cmdStr = GetUpdateSQL( serverDB );
			   
			   using ( SqlCommand    cmd    = new SqlCommand( cmdStr, connection ) ) 
			   {
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      
               cmd.ExecuteNonQuery();
            }
			}
			catch( Exception e )
			{
            errMsg = e.Message;
            retval = false;
			}
         return retval;
      }
      #endregion
      
      #region Private Methods

	   //-----------------------------------------------------------------------------
	   // InternalRead
	   //-----------------------------------------------------------------------------
	   private bool
	      InternalRead(
            SqlConnection     inConnection,
            string            inServerDB,
	         string            where
	      )
	   {
	      bool           retval = false;
	      
	      try
	      {
			   string cmdstr = GetSelectSQL( inServerDB,
			                                 where,
			                                 "" );

			   using ( SqlCommand    cmd    = new SqlCommand( cmdstr, inConnection ) )
			   {
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
			         if ( reader.Read() )
                  {
                     Load( reader );
                     retval = true;
                  }
                  else
                  {
                     errMsg = CoreConstants.Error_NoEventDataAvailable;
                  }
               }
            }
	      }
	      catch (Exception ex )
	      {
	        errMsg = ex.Message;
			}
	   
	      return retval;
	   }
     
      //-------------------------------------------------------------
      // Load
      //--------------------------------------------------------------
      public void
         Load(
            SqlDataReader reader
         )
      {
         int col = 0;

         eventId           = SQLHelpers.GetInt32( reader,    col++);
         
         try
         {
            eventType         = (TraceEventType)SQLHelpers.GetInt32( reader,     col++);
            eventCategory     = (TraceEventCategory)SQLHelpers.GetInt32( reader, col++);
         }
         catch (Exception)
         {
            eventType     = TraceEventType.InvalidType;
            eventCategory = TraceEventCategory.InvalidCategory;
         }
         
         targetObject      = SQLHelpers.GetString( reader,   col++);
         details           = SQLHelpers.GetString( reader,   col++);
         hash              = SQLHelpers.GetInt32( reader,    col++);
         eventClass        = SQLHelpers.GetInt32( reader,    col++);
         eventSubclass     = SQLHelpers.GetInt32( reader,    col++);
         startTime         = SQLHelpers.GetDateTime( reader, col++);
         spid              = SQLHelpers.GetInt32( reader,    col++);
         applicationName   = SQLHelpers.GetString( reader,   col++);
         hostName          = SQLHelpers.GetString( reader,   col++);
         serverName        = SQLHelpers.GetString( reader,   col++);
         loginName         = SQLHelpers.GetString( reader,   col++);
         success           = SQLHelpers.GetInt32( reader,    col++);
         databaseName      = SQLHelpers.GetString( reader,   col++);
         databaseId        = SQLHelpers.GetInt32( reader,    col++);
         dbUserName        = SQLHelpers.GetString( reader,   col++);
         objectType        = SQLHelpers.GetInt32( reader,    col++);
         objectName        = SQLHelpers.GetString( reader,   col++);
         objectId          = SQLHelpers.GetInt32( reader,    col++);
         permissions       = SQLHelpers.GetInt32( reader,    col++);
         columnPermissions = SQLHelpers.GetInt32( reader,    col++);
         targetLoginName   = SQLHelpers.GetString( reader,   col++);
         targetUserName    = SQLHelpers.GetString( reader,   col++);
         roleName          = SQLHelpers.GetString( reader,   col++);
         ownerName         = SQLHelpers.GetString( reader,   col++);
         alertLevel        = SQLHelpers.GetInt32( reader,    col++);
         checksum          = SQLHelpers.GetInt32( reader,    col++);
         privilegedUser    = SQLHelpers.GetInt32( reader,    col++);
         if (privilegedUser != 0)
         {
             privilegedUsers = loginName;
         }
         
         eventTypeString     = SQLHelpers.GetString( reader, col++);
         eventCategoryString = SQLHelpers.GetString( reader, col++);

         // SQL Server 2005 and 3.0 columns - they wont be in old databases  
            
         try
         {    
            fileName          = SQLHelpers.GetString( reader,   col++);
            linkedServerName  = SQLHelpers.GetString( reader,   col++);
            parentName        = SQLHelpers.GetString( reader,   col++);
            isSystem          = SQLHelpers.GetInt32( reader,    col++);
            sessionLoginName  = SQLHelpers.GetString( reader,   col++);
            providerName      = SQLHelpers.GetString( reader,   col++);
            
            // 3.0 columns
            appNameId = SQLHelpers.GetInt32( reader,    col++);
            hostId = SQLHelpers.GetInt32(reader, col++);
            loginId = SQLHelpers.GetInt32( reader,    col++);

            // 3.1 columns
            endTime = SQLHelpers.GetDateTime(reader, col++);
            startSequence = SQLHelpers.GetLong(reader, col++, -1);
            endSequence = SQLHelpers.GetLong( reader, col++, -1);
            if (reader.FieldCount > col)
            {
                rowCounts = SQLHelpers.GetRowCounts(reader, col++);
            }
         }
         catch 
         {
            // exception thrown if no SQL Server 2005 columns so we just initialize to 0
            fileName          = "";
            linkedServerName  = "";
            parentName        = "";
            isSystem          = 0;
            sessionLoginName  = "";
            providerName      = "";
         }
      }
      
      #endregion

      #region GetHashCode overload for event checksum calculation

      //-------------------------------------------------------------
      // GetHashCode - Used a checksum in has calculation
      //--------------------------------------------------------------
      public override int
         GetHashCode()
      {
         int x;
         
         x = eventClass;
         x += (eventSubclass<<2);

         int eCat = (int)eventCategory;
         if ( eCat!=0)               x += (eCat<<4);
         if ( databaseId!=0)         x += (databaseId<<6);
         if ( objectId!=0)           x += (objectId<<8);
         if ( permissions!=0)        x += (permissions<<10);
         if ( objectType!=0)         x += (objectType<<12);
         int eType = (int)eventType;
         if ( eType!=0)              x += (eType<<14);
         if ( privilegedUser!=0)     x += (privilegedUser<<16);
         if ( columnPermissions!=0)  x += (columnPermissions<<17);
         if ( alertLevel !=0)        x += (alertLevel<<18);
         if ( startSequence != 0 )   x += ((int)startSequence <<19);
         if( endSequence != 0 )      x += ((int)endSequence<<20);      

         if( endTime != DateTime.MaxValue && endTime != DateTime.MinValue ) x+= ((int)endTime.Ticks <<21);

         if ( applicationName!=null && applicationName!="" ) x ^= NativeMethods.GetHashCode(applicationName);
         if ( hostName!=null && hostName!="")                x ^= (NativeMethods.GetHashCode(hostName)<<1);
         if ( loginName!=null && loginName!="")              x ^= (NativeMethods.GetHashCode(loginName)<<2);
         if ( databaseName!=null && databaseName!="")        x ^= (NativeMethods.GetHashCode(databaseName)<<3);
         if ( dbUserName!=null && dbUserName!="")            x ^= (NativeMethods.GetHashCode(dbUserName)<<4);
         if ( objectName!=null && objectName!="")            x ^= (NativeMethods.GetHashCode(objectName)<<5);
         if ( targetLoginName!=null && targetLoginName!="")  x ^= (NativeMethods.GetHashCode(targetLoginName)<<6);
         if ( targetUserName!=null && targetUserName!="")    x ^= (NativeMethods.GetHashCode(targetUserName)<<7);
         if ( roleName!=null && roleName!="")                x ^= (NativeMethods.GetHashCode(roleName)<<8);
         if ( ownerName!=null && ownerName!="")              x ^= (NativeMethods.GetHashCode(ownerName)<<9);
         if ( serverName!=null && serverName!="")            x ^= (NativeMethods.GetHashCode(serverName)<<10);

         // new fields in checksum         
         if ( targetObject!=null && targetObject!="")        x ^= (NativeMethods.GetHashCode(targetObject)<<11);
         if ( details!=null && details!="")                  x ^= (NativeMethods.GetHashCode(details)<<12);
         x ^= (startTime.GetHashCode()<<13);

         if ( fileName!=null         && fileName!="")          x ^= (NativeMethods.GetHashCode(fileName)<<14);
         if ( linkedServerName!=null && linkedServerName!="")  x ^= (NativeMethods.GetHashCode(linkedServerName)<<15);
         if ( parentName!=null       && parentName!="")        x ^= (NativeMethods.GetHashCode(parentName)<<16);
         if ( isSystem!=0)                                     x ^= (isSystem<<17);
         if ( sessionLoginName!=null && sessionLoginName!="")  x ^= (NativeMethods.GetHashCode(sessionLoginName)<<18);
         if ( providerName!=null     && providerName!="")      x ^= (NativeMethods.GetHashCode(providerName)<<19);
         
         x ^= (success<<23);
         
         //new field in checksum
         if (rowCounts != null)
         {
             if (rowCounts == 0)
                 x ^= (NativeMethods.GetHashCode("ZERO") << 22);
             else
                 x += ((int)rowCounts << 22);
         }
         return x;
      }

      
      #endregion
      
      #region SQL
      
      //-------------------------------------------------------------
      // GetSelectSQL
      //--------------------------------------------------------------
      private static string
         GetSelectSQL(       
           string             serverDBName,
           string             strWhere,
           string             strOrder
         )
      {
         return GetSelectSQL( serverDBName,
                              CoreConstants.RepositoryEventsTable,
                              strWhere,
                              strOrder );
      }

       
        //-------------------------------------------------------------
        // GetSelectSQL
        //--------------------------------------------------------------
        public static string
           GetSelectSQL(
             string serverDBName,
             string tableName,
             string strWhere,
             string strOrder
           )
        {
            return GetSelectSQL(serverDBName,
                                 tableName,
                                 strWhere,
                                 strOrder,
                                 false);
        }

        
        //-------------------------------------------------------------
        // GetSelectSQL
        //--------------------------------------------------------------
        public static string
           GetSelectSQL(
             string serverDBName,
             string tableName,
             string strWhere,
             string strOrder,
             bool includeId
           )
        {
            string idString;
            string typeName;
            string category;
            string fromClause;
            string whereClause;

            if (includeId)
            {
                idString = ",e.eventId ";
                typeName = ",''Type Name'' ";
                category = ",''Category'' ";
                fromClause = " FROM {0}..{1} AS e";
                whereClause = " WHERE 1=1{2}{3}";
            }
            else
            {
                idString = "";
                typeName = ",t.name";
                category = ",t.category";
                fromClause = " FROM {0}..{1} AS e LEFT OUTER JOIN {5}..{6} t ON e.eventType=t.evtypeid ";
                whereClause = " WHERE 1=1{2}{3}";
            }

            string tmp = @"declare @RowCount bit =0
                        if(SELECT COUNT(*) FROM {0}.INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = '{1}' AND COLUMN_NAME = 'rowCounts')>0
                        set @RowCount = 1

                        declare @query nvarchar (max)" +
                          "set @query='SELECT " +
                             "e.eventId" +
                             ",e.eventType" +
                             ",e.eventCategory" +
                             ",e.targetObject" +
                             ",e.details" +
                             ",e.hash" +
                             ",e.eventClass" +
                             ",e.eventSubclass" +
                             ",e.startTime" +
                             ",e.spid" +
                             ",e.applicationName" +
                             ",e.hostName" +
                             ",e.serverName" +
                             ",e.loginName" +
                             ",e.success" +
                             ",e.databaseName" +
                             ",e.databaseId" +
                             ",e.dbUserName" +
                             ",e.objectType" +
                             ",e.objectName" +
                             ",e.objectId" +
                             ",e.permissions" +
                             ",e.columnPermissions" +
                             ",e.targetLoginName" +
                             ",e.targetUserName" +
                             ",e.roleName" +
                             ",e.ownerName" +
                             ",e.alertLevel" +
                             ",e.checksum" +
                             ",e.privilegedUser" +
                             typeName +
                             category +
                             ",e.fileName" +
                             ",e.linkedServerName" +
                             ",e.parentName" +
                             ",e.isSystem" +
                             ",e.sessionLoginName" +
                             ",e.providerName" +
                             ",e.appNameId" +
                             ",e.hostId" +
                             ",e.loginId" +
                             ",e.endTime" +
                             ",e.startSequence" +
                             ",e.endSequence" +
                             idString + "' " +
                             "if(@RowCount = 1) " +
                             "set @query = @query + ',e.rowCounts ' " +
                             "else set @query = @query + ', NULL as rowCounts '" +
                             "set @query = @query + '" +
                             fromClause +
                             whereClause + // where
                             " {4};'" +
                             "exec (@query)";  // order

          return string.Format(tmp,
                                SQLHelpers.CreateSafeDatabaseName(serverDBName),
                                tableName,
                                (strWhere != "") ? " AND " : "",
                                strWhere,
                                strOrder,
                                CoreConstants.RepositoryDatabase,
                                CoreConstants.RepositoryEventTypesTable);
      }
      //-------------------------------------------------------------
      // GetInsertSQL
      //--------------------------------------------------------------
      private string
         GetInsertSQL(       
           string             serverDBName
         )
      {
         string tmp = "INSERT INTO {0}..{1} (" +
                           "eventType" +
                           ",eventCategory" +
                           ",targetObject" +
                           ",details" +
                           ",hash" +
                           ",eventClass" +
                           ",eventSubclass" +
                           ",startTime" +
                           ",spid" +
                           ",applicationName" +
                           ",hostName" +
                           ",serverName" +
                           ",loginName" +
                           ",success" +
                           ",databaseName" +
                           ",databaseId" +
                           ",dbUserName" +
                           ",objectType" +
                           ",objectName" +
                           ",objectId" +
                           ",permissions" +
                           ",columnPermissions" +
                           ",targetLoginName" +
                           ",targetUserName" +
                           ",roleName" +
                           ",ownerName" +
                           ",alertLevel" +
                           ",checksum" +
                           ",privilegedUser" +
                           ",eventId" +
                           ",fileName" +
                           ",linkedServerName" +
                           ",parentName" +
                           ",isSystem" +
                           ",sessionLoginName" +
                           ",providerName" +
                           ",appNameId" +
                           ",hostId" +
                           ",loginId" +
                           ",endTime" +
                           ",startSequence" +
                           ",endSequence"+
                           ",rowCounts"+
                           ") VALUES ({2},{3},{4},{5},{6},{7},{8},{9},{10}," +
			                            "{11},{12},{13},{14},{15},{16},{17},{18},{19},"+
                                    "{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31}," +
                                    "{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44});";
         return string.Format( tmp,
                               SQLHelpers.CreateSafeDatabaseName(serverDBName),
                               CoreConstants.RepositoryEventsTable,
                               (int)eventType,
                               (int)eventCategory,
                               SQLHelpers.CreateSafeString(targetObject, MaxTargetObjNameLen),
                               SQLHelpers.CreateSafeString(details, MaxDetailLen),
                               hash,
                               eventClass,
                               eventSubclass,
                               SQLHelpers.CreateSafeDateTimeString(startTime),
                               spid,
                               SQLHelpers.CreateSafeString(applicationName, MaxObjNameLen),
                               SQLHelpers.CreateSafeString(hostName, MaxObjNameLen),
                               SQLHelpers.CreateSafeString(serverName, MaxObjNameLen),
                               SQLHelpers.CreateSafeString(loginName, MaxObjNameLen),
                               success,
                               SQLHelpers.CreateSafeString(databaseName, MaxObjNameLen),
                               databaseId,
                               SQLHelpers.CreateSafeString(dbUserName, MaxObjNameLen),
                               objectType,
                               SQLHelpers.CreateSafeString(objectName, MaxObjNameLen),
                               objectId,
                               permissions,
                               columnPermissions,
                               SQLHelpers.CreateSafeString(targetLoginName, MaxObjNameLen),
                               SQLHelpers.CreateSafeString(targetUserName, MaxObjNameLen),
                               SQLHelpers.CreateSafeString(roleName, MaxObjNameLen),
                               SQLHelpers.CreateSafeString(ownerName, MaxObjNameLen),
                               alertLevel,
                               checksum,
                               privilegedUser,
                              eventId,
                              SQLHelpers.CreateSafeString(fileName, MaxObjNameLen),
                              SQLHelpers.CreateSafeString(linkedServerName, MaxObjNameLen),
                              SQLHelpers.CreateSafeString(parentName, MaxObjNameLen),
                              isSystem,
                              SQLHelpers.CreateSafeString(sessionLoginName, MaxObjNameLen),
                              SQLHelpers.CreateSafeString(providerName, MaxObjNameLen),
                              appNameId,
                              hostId,
                              loginId,
                              SQLHelpers.CreateSafeDateTime(endTime),
                              startSequence,
                              endSequence,
                              rowCounts.HasValue? rowCounts.ToString() : "NULL");
      }

      private string
         GetUpdateSQL(
            string     serverDBName
         )
      {
         string tmp = "UPDATE {0}..{1} SET " +
                           "eventType = {2}" +
                           ",eventCategory = {3}" +
                           ",targetObject = {4}" +
                           ",details = {5}" +
                           ",hash = {6}" +
                           ",eventClass = {7}" +
                           ",eventSubclass = (8)" +
                           ",startTime = {9}" +
                           ",spid = {10}" +
                           ",applicationName = {11}" +
                           ",hostName = {12}" +
                           ",serverName = {13}" +
                           ",loginName = {14}" +
                           ",success = {15}" +
                           ",databaseName = {16}" +
                           ",databaseId = {17}" +
                           ",dbUserName = {18}" +
                           ",objectType = {19}" +
                           ",objectName = {20}" +
                           ",objectId = {21}" +
                           ",permissions = {22}" +
                           ",columnPermissions = {23}" +
                           ",targetLoginName = {24}" +
                           ",targetUserName = {25}" +
                           ",roleName = {26}" +
                           ",ownerName = {27}" +
                           ",alertLevel = {28}" +
                           ",checksum = {29}" +
                           ",privilegedUser = {30}" +
                           ",fileName = {31}" +
                           ",linkedServerName = {32}" +
                           ",parentName = {33}" +
                           ",isSystem = {34}" +
                           ",sessionLoginName = {35}" +
                           ",providerName = {36}" +
                           ",appNameId = {37}" +
                           ",hostId = {38}" +
                           ",loginId = {39}" +
                           ",endTime= {40}" +
                           ",startSequence = {41}" +
                           ",endSequence = {42}" +
                           ",rowCounts = {43}" +
                           " WHERE eventId = {44};";
         return string.Format( tmp,
                               SQLHelpers.CreateSafeDatabaseName(serverDBName),
                               CoreConstants.RepositoryEventsTable,
                               (int)eventType,
                               (int)eventCategory,
                               SQLHelpers.CreateSafeString(targetObject),
                               SQLHelpers.CreateSafeString(details),
                               hash,
                               eventClass,
                               eventSubclass,
                               SQLHelpers.CreateSafeDateTimeString(startTime),
                               spid,
                               SQLHelpers.CreateSafeString(applicationName),
                               SQLHelpers.CreateSafeString(hostName),
                               SQLHelpers.CreateSafeString(serverName),
                               SQLHelpers.CreateSafeString(loginName),
                               success,
                               SQLHelpers.CreateSafeString(databaseName),
                               databaseId,
                               SQLHelpers.CreateSafeString(dbUserName),
                               objectType,
                               SQLHelpers.CreateSafeString(objectName),
                               objectId,
                               permissions,
                               columnPermissions,
                               SQLHelpers.CreateSafeString(targetLoginName),
                               SQLHelpers.CreateSafeString(targetUserName),
                               SQLHelpers.CreateSafeString(roleName),
                               SQLHelpers.CreateSafeString(ownerName),
                               alertLevel,
                               checksum,
                               privilegedUser,
                              SQLHelpers.CreateSafeString(fileName),
                              SQLHelpers.CreateSafeString(linkedServerName),
                              SQLHelpers.CreateSafeString(parentName),
                              isSystem,
                              SQLHelpers.CreateSafeString(sessionLoginName),
                              SQLHelpers.CreateSafeString(providerName),
                              appNameId,
                              hostId,
                              loginId,
                              endTime,
                              SQLHelpers.CreateSafeDateTime(endTime),
                              startSequence,
                              endSequence,
                              rowCounts.HasValue ? rowCounts.ToString() : "NULL",
                              eventId );
      }

      #endregion
      
      #region Bulk Copy stuff

      #if bulkcopyt
      //-------------------------------------------------------------
      // GetBulkCopyRow
      //--------------------------------------------------------------
      public string
         GetBulkCopyRow()       
      {
         lock (bulk)
         {
            bulk.Length = 0;

            bulk.Append(SQLHelpers.CreateSafeDateTimeString(startTime,false));
            bulk.Append("\0\0");
            bulk.Append(checksum);
            bulk.Append("\0\0");
            bulk.Append(eventId);
            bulk.Append("\0\0");
            bulk.Append((int)eventType);
            bulk.Append("\0\0");
            bulk.Append(eventClass);
            bulk.Append("\0\0");
            bulk.Append(eventSubclass);
            bulk.Append("\0\0");
            bulk.Append(spid);
            bulk.Append("\0\0");
            bulk.Append(applicationName);
            bulk.Append("\0\0");
            bulk.Append(hostName);
            bulk.Append("\0\0");
            bulk.Append(serverName);
            bulk.Append("\0\0");
            bulk.Append(loginName);
            bulk.Append("\0\0");
            bulk.Append(success);
            bulk.Append("\0\0");
            bulk.Append(databaseName);
            bulk.Append("\0\0");
            bulk.Append(databaseId);
            bulk.Append("\0\0");
            bulk.Append(dbUserName);
            bulk.Append("\0\0");
            bulk.Append(objectType);
            bulk.Append("\0\0");
            bulk.Append(objectName);
            bulk.Append("\0\0");
            bulk.Append(objectId);
            bulk.Append("\0\0");
            bulk.Append(permissions);
            bulk.Append("\0\0");
            bulk.Append(columnPermissions);
            bulk.Append("\0\0");
            bulk.Append(targetLoginName);
            bulk.Append("\0\0");
            bulk.Append(targetUserName);
            bulk.Append("\0\0");
            bulk.Append(roleName);
            bulk.Append("\0\0");
            bulk.Append(ownerName);
            bulk.Append("\0\0");
            bulk.Append(targetObject);
            bulk.Append("\0\0");
            bulk.Append(details);
            bulk.Append("\0\0");
            bulk.Append((int)eventCategory);
            bulk.Append("\0\0");
            bulk.Append(hash);
            bulk.Append("\0\0");
            bulk.Append(alertLevel);
            bulk.Append("\0\0");
            bulk.Append(privilegedUser);
            bulk.Append("\0\0\r\n");
            
            return bulk.ToString();
         }
      }
      #endif
      
      #endregion

      #region ISerializable member implementation
      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         try
         {
            //---------------------------------------------------
            // V 1.* fields
            //---------------------------------------------------
            // calculated
            info.AddValue( "eventType", eventType );
            info.AddValue( "eventCategory", eventCategory );
         
            info.AddValue( "eventTypeString", eventTypeString );
            info.AddValue( "eventCategoryString", eventCategoryString );
         
            info.AddValue( "targetObject", targetObject );
            info.AddValue( "details", details );
            info.AddValue( "checksum", checksum );
            info.AddValue( "hash", hash );
            info.AddValue( "privilegedUser", privilegedUser );
            if (privilegedUser != 0)
            {
               info.AddValue( "privilegedUsers", loginName);
            }
            info.AddValue( "alertLevel", alertLevel );
            info.AddValue( "eventId", eventId );

            // from original trace file      
            info.AddValue( "eventClass", eventClass );
            info.AddValue( "eventSubclass", eventSubclass );
            info.AddValue( "spid", spid );
            info.AddValue( "applicationName", applicationName );
            info.AddValue( "hostName", hostName );
            info.AddValue( "serverName", serverName );
            info.AddValue( "loginName", loginName );
            info.AddValue( "success", success );
            info.AddValue( "databaseName", databaseName );
            info.AddValue( "databaseId", databaseId );
            info.AddValue( "dbUserName", dbUserName );
            info.AddValue( "objectType", objectType );
            info.AddValue( "objectName", objectName );
            info.AddValue( "objectId", objectId );
            info.AddValue( "permissions", permissions );
            info.AddValue( "columnPermissions", columnPermissions );
            info.AddValue( "targetLoginName", targetLoginName );
            info.AddValue( "targetUserName", targetUserName );
            info.AddValue( "roleName", roleName );
            info.AddValue( "ownerName", ownerName );

             // private properties
            info.AddValue( "connection", null );
            info.AddValue( "serverDB", serverDB );

           //---------------------------------------------------
            // V 2.0 fields
            //---------------------------------------------------

            if( classVersion < CoreConstants.SerializationVersion_20 )
            {
               // Special handling for 1.* fields
               info.AddValue( "startTime", startTime );
               return;
            }

            // startTime serialization is changed in 2.0
            // Note: startTime is always UTC
            info.AddValue( "startTicks", startTime.Ticks );

            info.AddValue( "classVersion", classVersion );

            // sqlserver 2005
            info.AddValue( "fileName", fileName );
            info.AddValue( "linkedServerName", linkedServerName );
            info.AddValue( "parentName", parentName );
            info.AddValue( "isSystem", isSystem );
            info.AddValue( "sessionLoginName", sessionLoginName );
            info.AddValue( "providerName", providerName );

            //---------------------------------------------------
            // V 3.0 fields
            //---------------------------------------------------
            info.AddValue( "appNameId", appNameId );
            info.AddValue( "hostId", hostId );
            info.AddValue( "loginId", loginId );

            // V 3.1 fields
            info.AddValue("endTime", endTime);
            info.AddValue("startSequence", startSequence);
            info.AddValue("endSequence", endSequence);

            if (classVersion >= CoreConstants.SerializationVersion_55)
            {
                //V5.5
                info.AddValue("rowCounts", rowCounts);
            }

            Debug.WriteLine( String.Format("{0} serialized.",this.GetType()));
         }
         catch( Exception e )
         {
            SerializationHelper.ThrowSerializationException( e, this.GetType());
         }

      }
      #endregion
	}
}
