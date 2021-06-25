using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Idera.SQLcompliance.Core.Event
{
   #region RemoteAuditConfiguration
	/// <summary>
	/// Summary description for RemoteAuditConfiguration.
	/// </summary>
	[Serializable]
	public struct RemoteAuditConfiguration : ISerializable
	{
      #region Fields

      public BaseAuditConfiguration          BaseConfig;      
      public int []                          ServerRoles;
      public string []                       Users;  // May contain user groups
      public string []                       AuditedUsers;  // User list after explosion
      public string []                       PrivUsers;  // User list from audited server roles
      public DBRemoteAuditConfiguration []   DBConfigs;  // For individual dababases settings
      public UserRemoteAuditConfiguration    UserConfig; // For privileged users settings
      public bool                            IsSQLsecure;
      public bool                            IsEnabled;
      public int []                          sqlsecureDBIds;
      public DateTime                        LastModifiedTime;
      public string[] ServerTrustedUsers; // Trusted Users at Server level.
      public int[] ServerTrustedUsersServerRoles;
      public string ServerTrustedUsersData;

      #endregion

      #region Properties

      public AuditCategory []                Categories
      {
         get { return BaseConfig.Categories; }
         set { BaseConfig.Categories = value; }
      }

      public int []                     AuditObjects
      {
         get { return BaseConfig.AuditObjects; }
         set { BaseConfig.AuditObjects = value; }
      }


      public bool                            CaptureDetails
      {
         get { return BaseConfig.CaptureDetails; }
         set { BaseConfig.CaptureDetails = value; }
      }

      //5.4_4.1.1_Extended Events
      public bool CaptureDetailsXE
      {
          get { return BaseConfig.CaptureDetailsXE; }
          set { BaseConfig.CaptureDetailsXE = value; }
      }

      //5.4_4.1.1_Extended Events
      public bool AuditCaptureSQLXE
      {
          get { return BaseConfig.AuditCaptureSQLXE; }
          set { BaseConfig.AuditCaptureSQLXE = value; }
      }

      public bool CaptureTransactions
      {
         get { return BaseConfig.CaptureTransactions; }
         set { BaseConfig.CaptureTransactions = value; }
      }

      public bool CaptureDDL
      {
          get
          {
              return BaseConfig.CaptureDDL;
          }

          set
          {
              BaseConfig.CaptureDDL = value;
          }
      }

      public bool                            Exceptions
      {
         get { return BaseConfig.Exceptions; }
         set { BaseConfig.Exceptions = value; }
      }
      
      public int                             Version
      {
         get { return BaseConfig.Version; }
         set { BaseConfig.Version = value; }
      }

      public DBObjectType []                 ObjectTypes
      {
         get { return BaseConfig.ObjectTypes; }
         set { BaseConfig.ObjectTypes = value; }
      }

      internal int StructVersion
      {
         get { return BaseConfig.structVersion; }
         set 
         { 
            BaseConfig.structVersion = value; 
            UserConfig.StructVersion = value;

            if( DBConfigs != null && DBConfigs.Length > 0 )
            {
               //Set the DBconfig version
               for( int i = 0; i < DBConfigs.Length; i++ )
               {
                  DBConfigs[i].StructVersion = value;
               }
            }
         }
      }

      public int AccessCheck
      {
         get { return BaseConfig.AccessCheck; }
         set 
         { 
            BaseConfig.AccessCheck = value;
         }
      }

      //5.5 Audit Logs
      public bool IsAuditLogEnabled 
      {
          get { return BaseConfig.IsAuditLogEnabled; }
          set { BaseConfig.IsAuditLogEnabled = value; }
      }

      #endregion

      #region Deserializatioin constructor

      public RemoteAuditConfiguration(
         SerializationInfo    info,
         StreamingContext     context )
      {
         BaseConfig = new BaseAuditConfiguration();
         ServerRoles = null;
         Users = null;
         AuditedUsers = null;
         PrivUsers = null;
         DBConfigs = new DBRemoteAuditConfiguration[0];
         UserConfig = new UserRemoteAuditConfiguration();
         IsSQLsecure = false;
         IsEnabled = false;
         sqlsecureDBIds = null;
         LastModifiedTime = DateTime.MinValue;
        //v5.6 SQLCM-5373
        ServerTrustedUsers = null;
        ServerTrustedUsersServerRoles = null;
        ServerTrustedUsersData = null;

         try
         {
            BaseConfig = (BaseAuditConfiguration)info.GetValue( "BaseConfig", typeof(BaseAuditConfiguration));      
            ServerRoles = (int [])info.GetValue( "ServerRoles", typeof(int []));
            Users = (string [])info.GetValue( "Users", typeof(string []));  // May contain user groups
            AuditedUsers = (string [])info.GetValue( "AuditedUsers", typeof(string []));  // User list after explosion
            PrivUsers = (string [])info.GetValue( "PrivUsers", typeof(string []));  // User list from audited server roles
            DBConfigs = (DBRemoteAuditConfiguration [])info.GetValue( "DBConfigs", typeof(DBRemoteAuditConfiguration []));  // For individual dababases settings
            UserConfig = (UserRemoteAuditConfiguration)info.GetValue( "UserConfig", typeof(UserRemoteAuditConfiguration)); // For privileged users settings
            IsSQLsecure = info.GetBoolean("IsSQLsecure");
            IsEnabled = info.GetBoolean("IsEnabled");
            sqlsecureDBIds = (int [])info.GetValue( "sqlsecureDBIds", typeof(int []));
            
            //v5.6 SQLCM-5373
            if (BaseConfig.structVersion >= CoreConstants.SerializationVersion_56)
            {
                ServerTrustedUsers = (string[])info.GetValue("ServerTrustedUsers", typeof(string[]));
                ServerTrustedUsersServerRoles = (int[])info.GetValue("ServerTrustedUsersServerRoles", typeof(int[]));
            }

            if( BaseConfig.structVersion >= CoreConstants.SerializationVersion_20 )
            {
               LastModifiedTime = new DateTime( info.GetInt64("LastModifiedTicks"));
               // Deserialize V 2.0 fields here
            }
            else
            {
               LastModifiedTime = info.GetDateTime("LastModifiedTime");
            }
         }
         catch( Exception e )
         {
            SerializationHelper.ThrowDeserializationException( e, typeof(RemoteAuditConfiguration));
         }
      }

      #endregion

      #region ISerializable members

      // Required custom serialization method
      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         try
         {
            info.AddValue( "BaseConfig", BaseConfig);      
            info.AddValue( "ServerRoles", ServerRoles);
            info.AddValue( "Users", Users);  // May contain user groups
            info.AddValue( "AuditedUsers", AuditedUsers);  // User list after explosion
            info.AddValue( "PrivUsers", PrivUsers);  // User list from audited server roles
            info.AddValue( "DBConfigs", DBConfigs);  // For individual dababases settings
            info.AddValue( "UserConfig", UserConfig); // For privileged users settings
            info.AddValue("IsSQLsecure", IsSQLsecure);
            info.AddValue("IsEnabled", IsEnabled);
            info.AddValue( "sqlsecureDBIds", sqlsecureDBIds);
            
            //v5.6 SQLCM-5373
            if (BaseConfig.structVersion >= CoreConstants.SerializationVersion_56)
            { 
                info.AddValue("ServerTrustedUsers", ServerTrustedUsers);
                info.AddValue("ServerTrustedUsersServerRoles", ServerTrustedUsersServerRoles);
            }

            if( BaseConfig.structVersion >= CoreConstants.SerializationVersion_20 )
            {
               info.AddValue( "LastModifiedTicks", LastModifiedTime.Ticks );
               // Add V 2.0 fields here
            }
            else
            {
               info.AddValue("LastModifiedTime", LastModifiedTime);
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


   #endregion

   #region BaseAuditConfiguration

   [Serializable]
   public struct BaseAuditConfiguration : ISerializable
   {
      #region Fields

      // V 1.* fields
      public AuditCategory []                Categories;
      public int []                          AuditObjects;
      public bool                            CaptureDetails;
      public bool                            CaptureTransactions;
      public bool                            CaptureDDL;
      //public bool                            Failures;  // removed since 2.1
      public bool                            Exceptions;
      public int                             Version;
      public DBObjectType []                 ObjectTypes;

       //5.4  DMLwithSelect SQL Capture By XE
      public bool                            CaptureDetailsXE;
       //5.4  DMLwithSelect SQL Capture By XE
      public bool                            AuditCaptureSQLXE;

      //5.5 Audit Logs
      public bool                            IsAuditLogEnabled;

      // V 2.0 fields
      internal int                           structVersion;

      // V 2.1 fields
      public int                             AccessCheck;  // replacing Failures field

      public int AuditSensitiveColumnActivity; // SQLCM-5471 - v5.6
      public int AuditSensitiveColumnActivityDataset; // SQLCM-5471 - v5.6
      #endregion

      #region Deserializatioin constructor

      public BaseAuditConfiguration(
         SerializationInfo    info,
         StreamingContext     context )
      {
         Categories = null;
         AuditObjects = null;
         CaptureDetails = false;
         CaptureTransactions = false;
         CaptureDDL = false;
         Exceptions = false;
         Version = 0;
         ObjectTypes = null;
         structVersion = CoreConstants.SerializationVersion;
         AccessCheck = (int)AccessCheckFilter.SuccessOnly;

          //5.4_4.1.1_Extended Events
         CaptureDetailsXE = false;
         AuditCaptureSQLXE = false;
         IsAuditLogEnabled = false;
         AuditSensitiveColumnActivity = (int)SensitiveColumnActivity.SelectOnly; // SQLCM-5471 v5.6
         AuditSensitiveColumnActivityDataset = (int)SensitiveColumnActivity.SelectOnly; // SQLCM-5471 v5.6
         try
         {
            // V 1.* fields
            Categories = (AuditCategory[])info.GetValue( "Categories", typeof(AuditCategory[]));
            AuditObjects = (int [])info.GetValue( "AuditObjects", typeof(int []));
            CaptureDetails = info.GetBoolean( "CaptureDetails");
            Exceptions = info.GetBoolean( "Exceptions");
            Version = info.GetInt32( "Version");
            ObjectTypes = (DBObjectType [])info.GetValue( "ObjectTypes", typeof(DBObjectType []));

            // V 2.0 fields
            try
            {
               structVersion = info.GetInt32( "structVersion" );
            }
            catch
            {
               structVersion = 0;
            }

            if( structVersion >= CoreConstants.SerializationVersion_21 )
            {
               AccessCheck = info.GetInt32( "AccessCheck" );
            }
            else if( structVersion == 0 )
            {
               AccessCheck = (int) AccessCheckFilter.NoFilter;
            }
            else
            {
               if( info.GetBoolean( "Failures" ) )
                  AccessCheck = (int)AccessCheckFilter.NoFilter;
               else
                  AccessCheck = (int)AccessCheckFilter.SuccessOnly;
            }

            if (structVersion >= CoreConstants.SerializationVersion_35)
            {
               CaptureTransactions = info.GetBoolean("CaptureTransactions");
            }
            else
            {
               CaptureTransactions = false;
            }
            
            if (structVersion >= CoreConstants.SerializationVersion_51)
            {
                CaptureDDL = info.GetBoolean("CaptureDDL");
            }
            else
            {
                CaptureDDL = false;
            }
            if (structVersion >= CoreConstants.SerializationVersion_54)
            {
                CaptureDetailsXE = info.GetBoolean("CaptureDetailsXE");  //5.4_4.1.1_Extended Events
                AuditCaptureSQLXE = info.GetBoolean("AuditCaptureSQLXE");  //5.4_4.1.1_Extended Events
            }
            else
            {
                CaptureDetailsXE = false;
                AuditCaptureSQLXE = false;
            }

            //5.4 Audit Logs
            if (structVersion >= CoreConstants.SerializationVersion_55)
            {
                IsAuditLogEnabled = info.GetBoolean("IsAuditLogEnabled");
            }
            else
            {
                IsAuditLogEnabled = false;
            }

            // SQLCM-5471 v5.6
            if (structVersion >= CoreConstants.SerializationVersion_56)
            {

                AuditSensitiveColumnActivity = info.GetInt32("AuditSensitiveColumnActivity");
                AuditSensitiveColumnActivityDataset = info.GetInt32("AuditSensitiveColumnActivityDataset");
                    
            }
            else
            {
                AuditSensitiveColumnActivity = 0;
                AuditSensitiveColumnActivityDataset = 0;
            }

         }
         catch( Exception e )
         {
            SerializationHelper.ThrowDeserializationException( e, this.GetType());
         }
      }

      #endregion

      #region ISerializable members

      // Required custom serialization method
      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         try
         {
            // V 1.* fields
            info.AddValue("Categories", Categories, typeof(object []) );
            info.AddValue("AuditObjects", AuditObjects, typeof(int []) );
            info.AddValue("CaptureDetails", CaptureDetails );
            info.AddValue("Exceptions", Exceptions );
            info.AddValue("Version", Version );
            info.AddValue("ObjectTypes", ObjectTypes, typeof(object []) );

            // V 2.0 fields
            if( structVersion < CoreConstants.SerializationVersion_20 )
               return;

            info.AddValue( "structVersion", structVersion );

            // V 2.1 fields
            if( structVersion >= CoreConstants.SerializationVersion_21 )
               info.AddValue( "AccessCheck", AccessCheck );
            else
               info.AddValue("Failures", !(AccessCheck == (int)AccessCheckFilter.SuccessOnly) );

            if (structVersion >= CoreConstants.SerializationVersion_35)
            {
               info.AddValue("CaptureTransactions", CaptureTransactions);
            }
            else
            {
               CaptureTransactions = false;
            }

            if (structVersion >= CoreConstants.SerializationVersion_51)
            {
                info.AddValue("CaptureDDL", CaptureDDL);
            }
            else
            {
                CaptureDDL = false;
            }
            if (structVersion >= CoreConstants.SerializationVersion_54)
            {
                info.AddValue("CaptureDetailsXE", CaptureDetailsXE);//5.4_4.1.1_Extended Events
                info.AddValue("AuditCaptureSQLXE", AuditCaptureSQLXE);//5.4_4.1.1_Extended Events
            }
            else
            {
                CaptureDetailsXE = false;
                AuditCaptureSQLXE = false;
            }

            //5.5 Audit Logs
            if (structVersion >= CoreConstants.SerializationVersion_55)
            {
                info.AddValue("IsAuditLogEnabled", IsAuditLogEnabled);
            }

            // SQLCM-5471 v5.6
            if (structVersion >= CoreConstants.SerializationVersion_56)
            {
                info.AddValue("AuditSensitiveColumnActivity", AuditSensitiveColumnActivity);
                info.AddValue("AuditSensitiveColumnActivityDataset", AuditSensitiveColumnActivityDataset);
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


   #endregion

   #region UserRemoteAuditConfiguration

   [Serializable]
   public struct UserRemoteAuditConfiguration : ISerializable
   {
      #region Fields

      public BaseAuditConfiguration          BaseConfig;  
      public DBRemoteAuditConfiguration []   dbConfigs;  // For individual dababases settings

      #endregion

      #region Properties

      public AuditCategory []                Categories
      {
         get { return BaseConfig.Categories; }
         set { BaseConfig.Categories = value; }
      }

      public int []                          AuditObjects
      {
         get { return BaseConfig.AuditObjects; }
         set { BaseConfig.AuditObjects = value; }
      }


      public bool                            CaptureDetails
      {
         get { return BaseConfig.CaptureDetails; }
         set { BaseConfig.CaptureDetails = value; }
      }

      public bool CaptureTransactions
      {
         get { return BaseConfig.CaptureTransactions; }
         set { BaseConfig.CaptureTransactions = value; }
      }

      public bool CaptureDDL
      {
          get 
          { 
              return BaseConfig.CaptureDDL; 
          }

          set 
          { 
              BaseConfig.CaptureDDL = value; 
          }
      }

      public bool                            Exceptions
      {
         get { return BaseConfig.Exceptions; }
         set { BaseConfig.Exceptions = value; }
      }
      
      public int                             Version
      {
         get { return BaseConfig.Version; }
         set { BaseConfig.Version = value; }
      }

      public DBObjectType []                 ObjectTypes
      {
         get { return BaseConfig.ObjectTypes; }
         set { BaseConfig.ObjectTypes = value; }
      }

       //5.4_4.1.1_Extended Events
      public bool CaptureDetailsXE
      {
          get { return BaseConfig.CaptureDetailsXE; }
          set { BaseConfig.CaptureDetailsXE = value; }
      }

      //5.4_4.1.1_Extended Events
      public bool AuditCaptureSQLXE
      {
          get { return BaseConfig.AuditCaptureSQLXE; }
          set { BaseConfig.AuditCaptureSQLXE = value; }
      }

      internal int StructVersion
      {
         get { return BaseConfig.structVersion; }
         set 
         { 
            BaseConfig.structVersion = value;
            if( dbConfigs != null && dbConfigs.Length > 0 )
            {
               for( int i = 0; i < dbConfigs.Length; i++ )
               {
                  dbConfigs[i].StructVersion = value;
               }
            }
         }
      }

      public int AccessCheck
      {
         get { return BaseConfig.AccessCheck; }
         set 
         { 
            BaseConfig.AccessCheck = value;
         }
      }

      #endregion

      #region Deserializatioin constructor

      public UserRemoteAuditConfiguration(
         SerializationInfo    info,
         StreamingContext     context )
      {
         BaseConfig = new BaseAuditConfiguration();
         dbConfigs = new DBRemoteAuditConfiguration[0];

         try
         {
            BaseConfig = (BaseAuditConfiguration)info.GetValue( "BaseConfig", typeof(BaseAuditConfiguration));
            dbConfigs = (DBRemoteAuditConfiguration [])info.GetValue( "dbConfigs", typeof(DBRemoteAuditConfiguration));
         }
         catch( Exception e )
         {
            SerializationHelper.ThrowDeserializationException( e, this.GetType());
         }
      }

      #endregion

      #region ISerializable members

      // Required custom serialization method
      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         try
         {
            info.AddValue( "BaseConfig", BaseConfig );
            info.AddValue( "dbConfigs", dbConfigs );
            Debug.WriteLine( String.Format("{0} serialized.",this.GetType()));
         }
         catch( Exception e )
         {
            SerializationHelper.ThrowSerializationException( e, this.GetType());
         }

      }
      #endregion
   }


   #endregion

   #region DBRemoteAuditConfiguration

   [Serializable]
   public struct DBRemoteAuditConfiguration : ISerializable
   {
      #region Fields

      public int                             DbId;
      public string                          dbName;
      public BaseAuditConfiguration          BaseConfig;  
      public int []                          ServerRoles;
      public string []                       Users;
      public string []                       TrustedUsers;
      public TableConfiguration[]            DataChangeTables;
      public TableConfiguration[]            SensitiveColumns;
      public int []                          PrivServerRoles;
      public string[]                        PrivUsers;
      public AuditCategory[]                 UserCategories;
      public bool                            UserCaptureSql;
      public bool                            UserCaptureTran;
      public bool                            UserCaptureDDL;
      public bool                            UserExceptions;
      public int                             UserAccessCheck;

      #endregion

      #region Properties

      public AuditCategory []                Categories
      {
         get { return BaseConfig.Categories; }
         set { BaseConfig.Categories = value; }
      }

      public int []                          AuditObjects
      {
         get { return BaseConfig.AuditObjects; }
         set { BaseConfig.AuditObjects = value; }
      }


      public bool                            CaptureDetails
      {
         get { return BaseConfig.CaptureDetails; }
         set { BaseConfig.CaptureDetails = value; }
      }

      public bool                            CaptureTransactions
      {
         get { return BaseConfig.CaptureTransactions; }
         set { BaseConfig.CaptureTransactions = value; }
      }

      public bool CaptureDDL
      {
          get
          {
              return BaseConfig.CaptureDDL;
          }

          set
          {
              BaseConfig.CaptureDDL = value;
          }
      }

      public bool                            Exceptions
      {
         get { return BaseConfig.Exceptions; }
         set { BaseConfig.Exceptions = value; }
      }

      public int                             Version
      {
         get { return BaseConfig.Version; }
         set { BaseConfig.Version = value; }
      }

      public DBObjectType []                 ObjectTypes
      {
         get { return BaseConfig.ObjectTypes; }
         set { BaseConfig.ObjectTypes = value; }
      }

      internal int StructVersion
      {
         get { return BaseConfig.structVersion; }
         set 
         { 
            BaseConfig.structVersion = value;

            if (DataChangeTables != null)
            {
               if (DataChangeTables.Length > 0)
               {
                  //set the Table config version
                  for (int i = 0; i < DataChangeTables.Length; i++)
                  {
                     DataChangeTables[i].StructVersion = value;
                  }
               }
            }

            if (SensitiveColumns != null)
            {
               if (SensitiveColumns.Length > 0)
               {
                  //set the Table config version
                  for (int i = 0; i < SensitiveColumns.Length; i++)
                  {
                     SensitiveColumns[i].StructVersion = value;
                  }
               }
            }
         }
      }
      

      public int AccessCheck
      {
         get { return BaseConfig.AccessCheck; }
         set 
         { 
            BaseConfig.AccessCheck = value;
         }
      }
      // SQLCM-5471 v5.6 Add Activity to Senstitive columns
      public int AuditSensitiveColumnActivity
      {
          get { return BaseConfig.AuditSensitiveColumnActivity; }
          set
          {
              BaseConfig.AuditSensitiveColumnActivity = value;
          }
      }
      public int AuditSensitiveColumnActivityDataset
      {
          get { return BaseConfig.AuditSensitiveColumnActivityDataset; }
          set
          {
              BaseConfig.AuditSensitiveColumnActivityDataset = value;
          }
      }

      #endregion

      #region Deserializatioin constructor

      public DBRemoteAuditConfiguration(
         SerializationInfo    info,
         StreamingContext     context )
      {
         DbId = 0;
         dbName = null;
         BaseConfig = new BaseAuditConfiguration();
         ServerRoles = null;
         Users = null;
         TrustedUsers = null;
         DataChangeTables = null;
         SensitiveColumns = null;
         PrivServerRoles = null;
         PrivUsers = null;
         UserCategories = null;
         UserCaptureSql = false;
         UserCaptureTran = false;
         UserCaptureDDL = false;
         UserExceptions = false;
         UserAccessCheck = (int)AccessCheckFilter.SuccessOnly;


         try
         {
            DbId = info.GetInt32( "DbId" );
            dbName = info.GetString( "dbName" );
            BaseConfig = (BaseAuditConfiguration)info.GetValue( "BaseConfig", typeof(BaseAuditConfiguration));
            ServerRoles = (int [])info.GetValue( "ServerRoles", typeof(int []));
            Users = (string [])info.GetValue( "Users", typeof(string []));
            
            if( BaseConfig.structVersion >= CoreConstants.SerializationVersion_30 )
            {
               // Deserialize V 3.0 and later fields here
               
               TrustedUsers = (string[])info.GetValue( "TrustedUsers", typeof(string[]));
                  
            }
            
            if( BaseConfig.structVersion >= CoreConstants.SerializationVersion_31 )
            {
               // V 3.1 fields
               DataChangeTables = (TableConfiguration[])info.GetValue("DataChangeTables", typeof(TableConfiguration[]));
            }

            if (BaseConfig.structVersion >= CoreConstants.SerializationVersion_35)
            {
               // V 3.5 fields
               SensitiveColumns = (TableConfiguration[])info.GetValue("SensitiveColumns", typeof(TableConfiguration[]));
            }

            if (BaseConfig.structVersion >= CoreConstants.SerializationVersion_50)
            {
                // V 5.0 fields
                PrivServerRoles = (int[])info.GetValue("PrivServerRoles", typeof(int[]));
                PrivUsers = (string[])info.GetValue("PrivUsers", typeof(string[]));
                UserCategories = (AuditCategory[])info.GetValue("UserCategories", typeof(AuditCategory[]));
                UserCaptureSql = info.GetBoolean("UserCaptureSql");
                UserCaptureTran = info.GetBoolean("UserCaptureTran");
                UserExceptions = info.GetBoolean("UserExceptions");
                UserAccessCheck = info.GetInt32("UserAccessCheck");
            }

            if (BaseConfig.structVersion >= CoreConstants.SerializationVersion_51)
            {
                UserCaptureDDL = info.GetBoolean("UserCaptureDDL");
            }

         }
         catch( Exception e )
         {
            SerializationHelper.ThrowDeserializationException( e, typeof(DBAuditConfiguration));
         }
      }

      #endregion

      #region ISerializable members

      // Required custom serialization method
      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         try
         {
            info.AddValue( "DbId", DbId );
            info.AddValue( "dbName", dbName );
            info.AddValue( "BaseConfig", BaseConfig );
            info.AddValue( "ServerRoles", ServerRoles );
            info.AddValue( "Users", Users );
            

            if( BaseConfig.structVersion >= CoreConstants.SerializationVersion_30 )
            {
               // Add V 3.0 and later fields here.
               info.AddValue( "TrustedUsers", TrustedUsers );
            }
            
            if (BaseConfig.structVersion >= CoreConstants.SerializationVersion_31)
            {
               // Add V 3.1 fields here.
               info.AddValue("DataChangeTables", DataChangeTables);
            }

            if (BaseConfig.structVersion >= CoreConstants.SerializationVersion_35)
            {
               // Add V 3.5 fields here.
               info.AddValue("SensitiveColumns", SensitiveColumns);
            }

            if (BaseConfig.structVersion >= CoreConstants.SerializationVersion_50)
            {
                //Add V 5.0 fields here.
                info.AddValue("PrivServerRoles", PrivServerRoles);
                info.AddValue("PrivUsers", PrivUsers);
                info.AddValue("UserCategories", UserCategories);
                info.AddValue("UserCaptureSql", UserCaptureSql);
                info.AddValue("UserCaptureTran", UserCaptureTran);
                info.AddValue("UserExceptions", UserExceptions);
                info.AddValue("UserAccessCheck", UserAccessCheck);
            }

            if (BaseConfig.structVersion >= CoreConstants.SerializationVersion_51)
            {
                info.AddValue("UserCaptureDDL", UserCaptureDDL);
            }

            Debug.WriteLine(String.Format("{0} serialized.", this.GetType()));
         }
         catch( Exception e )
         {
            SerializationHelper.ThrowSerializationException( e, this.GetType());
         }

      }

      #endregion
   }


   #endregion
   
   #region Monitored Data Change Table Configuration

   [Serializable]
   public struct TableConfiguration : ISerializable
   {
      #region Fields
      public int SrvId;
      public int DbId;
      public int Id;
      public string Schema;
      public string Name;
      public int MaxRows;
      public int StructVersion;
      public string [] Columns;
      public string Type;
      #endregion

      #region Public Operators & Methods
      public static bool operator ==(
         TableConfiguration lhs,
         TableConfiguration rhs)
      {
         if (lhs.SrvId != rhs.SrvId ||
             lhs.DbId != rhs.DbId ||
             lhs.Id != rhs.Id ||
             lhs.Schema != rhs.Schema ||
             lhs.Name != rhs.Name ||
             lhs.Type !=rhs.Type)
            return false;
         else
            return true;
      }

      public static bool operator !=(
         TableConfiguration lhs,
         TableConfiguration rhs)
      {
         return !(lhs == rhs);
      }

      public bool Changed(TableConfiguration config)
      {
         if (MaxRows != config.MaxRows ||
             Columns.Length != config.Columns.Length)
            return true;

         if (StructVersion > CoreConstants.SerializationVersion_31 &&
             config.StructVersion > CoreConstants.SerializationVersion_31)
         {
            for (int i = 0; i < Columns.Length; i++)
            {
               if (Columns[i] != config.Columns[i])
                  return true;
            }
         }
         return false;
      }

      #endregion


      #region Deserializatioin constructor

      public TableConfiguration(
         SerializationInfo info,
         StreamingContext context)
      {
         SrvId = 0;
         DbId = 0;
         Id = 0;
         Schema = "";
         Name = "";
         MaxRows = 0;
         Type = "";
         StructVersion = CoreConstants.SerializationVersion;
         Columns = new string[0];

         try
         {
            SrvId = info.GetInt32( "SrvId" );
            DbId = info.GetInt32("DbId");
            Id = info.GetInt32("Id" ); 
            Schema = info.GetString( "Schema" );
            Name = info.GetString("Name");
            MaxRows = info.GetInt32( "MaxRows" );
            StructVersion = info.GetInt32("StructVersion");

            if (StructVersion > CoreConstants.SerializationVersion_31)
            {
               Columns = (string[])info.GetValue("Columns", typeof(string[]));
            }

         }
         catch (Exception e)
         {
            SerializationHelper.ThrowDeserializationException(e, typeof(TableConfiguration));
         }
      }

      #endregion

      #region ISerializable members

      // Required custom serialization method
      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         try
         {
            info.AddValue( "SrvId", SrvId );
            info.AddValue("DbId", DbId);
            info.AddValue( "Id", Id );
            info.AddValue( "Schema", Schema ); 
            info.AddValue("Name", Name);
            info.AddValue("MaxRows", MaxRows);
            info.AddValue("StructVersion", StructVersion );
            if (StructVersion > CoreConstants.SerializationVersion_31)
               info.AddValue("Columns", Columns);

            Debug.WriteLine(String.Format("{0} serialized.", this.GetType()));
         }
         catch (Exception e)
         {
            SerializationHelper.ThrowSerializationException(e, this.GetType());
         }

      }
      
      public string GetFullName()
      {
         return CoreHelpers.GetTableNameKey( Schema, Name );
      }

      #endregion
   }
   #endregion


}
