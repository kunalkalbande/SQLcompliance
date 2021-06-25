using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;


namespace Idera.SQLcompliance.Core.Event
{
   /// <summary>
   /// Summary description for DBAuditConfiguration.
   /// This class is for setting database level audit configurations and
   /// generating event trace configurations.
   /// </summary>
   public class DBAuditConfiguration : AuditConfiguration
   {
      #region Private Data Members

      protected int                         dbId;
      protected ServerAuditConfiguration    serverConfig;
      protected bool[]                      auditObjectTypes;
      protected Hashtable                   dbIdList;
      protected Hashtable                   auditTableList;
      protected List<TableConfiguration>    dataChangeTables = new List<TableConfiguration>();
      protected List<TableConfiguration>    sensitiveColumns = new List<TableConfiguration>();
      protected string                      name;
      protected int[]                       privServerRoles;
      protected string[]                    privUsers;
      protected AuditCategory[]             userCategories;
      protected bool                        userCaptureSql;
      protected bool                        userCaptureTran;
      protected bool                        userExceptions;
      protected int                         userAccessCheck;
      protected Hashtable                   dbNameList; //5.5 Audit Logs

      #endregion

      #region Properties

       public int[] PrivServerRoles
       {
           get { return privServerRoles; }
           set { privServerRoles = value; }
       }

       public string[] PrivUsers
       {
           get { return privUsers; }
           set { privUsers = value; }
       }

       public AuditCategory[] UserCategories
       {
           get { return userCategories; }
           set { userCategories = value; }
       }

       public bool UserCaptureSql
       {
           get { return userCaptureSql; }
           set { userCaptureSql = value; }
       }

       public bool UserCaptureTran
       {
           get { return userCaptureTran; }
           set { userCaptureTran = value; }
       }

       public bool UserExceptions
       {
           get { return userExceptions; }
           set { userExceptions = value; }
       }

       public int UserAccessCheck
       {
           get { return userAccessCheck; }
           set { userAccessCheck = value; }
       }

       public int DBId
      {
         get { return dbId; }
         set 
         { 
            dbId = value; 
            AddDBId( value );
         }
      }

      public ServerAuditConfiguration ServerConfiguration
      {
         get { return serverConfig; }
         set { serverConfig = value; }
      }

      public int [] DBIdList
      {
         get
         {
            int[] idList = new int[ dbIdList.Count ];

            int idx = 0;
            IDictionaryEnumerator enumerator = dbIdList.GetEnumerator();
            while( enumerator.MoveNext() )
               idList[idx++] = (int)enumerator.Value;

            return idList;
         }
      }
      
      public string Name
      {
         get
         {
            return name;
         }
         
         set
         {
            name = value;
         }
      }

      /// <summary>
      /// 5.5 Audit Logs
      /// </summary>
      public string[] DBNameList
      {
          get
          {
              string[] nameList = new string[dbNameList.Count];

              int index = 0;
              IDictionaryEnumerator enumerator = dbNameList.GetEnumerator();
              while (enumerator.MoveNext())
                  nameList[index++] = (string)enumerator.Value;

              return nameList;
          }
      }

      //5.5 Audit Log
      /// <summary>
      /// AddDBName
      /// </summary>
      /// <param name="name"></param>
      public void AddDBName(string name)
      {
          try
          {
              dbNameList.Add(name, name);
          }
          catch (ArgumentException)
          {
              // Same name in the table, ignore it
          }
      }

      public void ClearDBNameList()
      {
          try
          {
              dbNameList.Clear();
          }
          catch { }
      }


      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      public DBObjectType[]
         AuditObjectTypeList
      {
         get
         {
            ArrayList typeList = new ArrayList();
            for( int i = 0; i < auditObjectTypes.Length; i++ )
            {
               if( auditObjectTypes[i] )
                  typeList.Add( DBObject.GetTypeFromId( i ) );
            }

            return (DBObjectType [])typeList.ToArray(typeof(DBObjectType));
         }
      }

      /// <summary>
      /// GetAuditTableList: get an arry of tables being audited
      /// </summary>
      /// <returns></returns>
      public int[] 
         AuditTableList
      {
         get
         {
            int[] tableList = new int[ auditTableList.Count ];

            IDictionaryEnumerator enumerator = auditTableList.GetEnumerator();
            int idx = 0;

            while( enumerator.MoveNext() )
            {
               tableList[idx++] = (int)enumerator.Value;
            }

            return tableList;
         }
      }

      public TableConfiguration[] DataChangeTables
      {
         get { return dataChangeTables.ToArray(); }
         set
         {
            dataChangeTables.Clear();
            if ( value != null )
               foreach (TableConfiguration table in value)
                  dataChangeTables.Add( table );
         }
      }

      public TableConfiguration[] SensitiveColumns
      {
         get { return sensitiveColumns.ToArray(); }
         set
         {
            sensitiveColumns.Clear();
            if (value != null)
               foreach (TableConfiguration table in value)
                  sensitiveColumns.Add(table);
         }
      }

      #endregion
      
      #region Constructors


      public DBAuditConfiguration (
         int dbId
         )
          : this(dbId, false, false, false, false, false, AccessCheckFilter.NoFilter, false, false, true, false, false, false, false, false, AccessCheckFilter.NoFilter, false, false, true, SensitiveColumnActivity.SelectOnly, SensitiveColumnActivity.SelectOnly) // SQLCM-5471 v5.6 Add Activity to Senstitive columns
      {}

      public DBAuditConfiguration (
         int  dbId,
         bool ddl,
         bool dml,
         bool security,
         bool select,
         bool admin,
         AccessCheckFilter accessCheck,
         bool captureDetails,
         bool captureTransactions,
         bool exceptions,
         bool userDDL,
         bool userDML,
         bool userSecurity,
         bool userSelect,
         bool userAdmin,
         AccessCheckFilter userAccessCheck,
         bool userCaptureDetails,
         bool userCaptureTransactions,
         bool userExceptions,
          SensitiveColumnActivity auditSensitiveColumnActivity, // SQLCM-5471 v5.6 Add Activity to Senstitive columns
          SensitiveColumnActivity auditSensitiveColumnActivityDataset // SQLCM-5471 v5.6 Add Activity to Senstitive columns
         ) 
      {
         AuditDDL = ddl;
         AuditDML = dml;
         AuditSecurity = security;
         AuditSELECT = select;
         AuditAdmin = admin;
         AuditAccessCheck = accessCheck;
         AuditCaptureDetails = captureDetails;
         AuditCaptureTransactions = captureTransactions;
         AuditExceptions = exceptions;

          AuditUserDDL = userDDL;
          AuditUserDML = userDML;
          AuditUserSecurity = userSecurity;
          AuditUserSELECT = userSelect;
          AuditUserAdmin = userAdmin;
          AuditUserCaptureSql = userCaptureDetails;
          AuditUserCaptureTransactions = userCaptureTransactions;
          AuditUserExceptions = userExceptions;
          AuditUserAccessCheck = userAccessCheck;
          AuditSensitiveColumnActivity = auditSensitiveColumnActivity;// SQLCM-5471 v5.6 Add Activity to Senstitive columns
          AuditSensitiveColumnActivityDataset = auditSensitiveColumnActivityDataset;// SQLCM-5471 v5.6 Add Activity to Senstitive columns

         this.dbId = dbId;
         auditTableList = new Hashtable();
         dbIdList = new Hashtable();
         auditObjectTypes = new bool[96] ; // number of types available
         dbNameList = new Hashtable();

      }

      #endregion

      #region Public Methods
      
      /// <summary>
      /// AuditTable
      /// </summary>
      public void 
         AuditTable
         (
            int    tableId,
            bool   add   // true = add, false = remove
         )
      {

         if( add )
         {
            try
            {
               auditTableList.Add( tableId, tableId );
            }
            catch ( ArgumentException )
            {
               // already in there.  Ignore it.
            }
         }
         else
         {
            auditTableList.Remove( tableId );
         }
      }


      
      /// <summary>
      /// AuditObjectType
      /// </summary>
      /// <param name="type"></param>
      /// <param name="enable"></param>
      public void
         AuditObjectType (
            DBObjectType type,
            bool         enable
         )
      {
         if( !DBObject.IsValidType( type ) )
            throw( new ArgumentException( String.Format( "Invalid object type {0}", (int)type),
               "type" ) );
         try
         {
            auditObjectTypes[DBObject.GetIdFromType(type)] = enable;
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( String.Format( "Invalid object type ID: {0}.  There are {1} valid types.", DBObject.GetIdFromType(type), auditObjectTypes.Length ),
               e,
               true );
         }
      }

      /// <summary>
      /// AddDBId
      /// </summary>
      /// <param name="id"></param>
      public void AddDBId( int id )
      {
         try
         {
            dbIdList.Add( id, id );
         }
         catch( ArgumentException )
         {
            // Same id in the table, ignore it
         }
      }

      public void ClearDBIdList()
      {
         try
         {
            dbIdList.Clear();
         }
         catch{}
      }


      //-----------------------------------------------------------------
      // AddAuditedTables
      //-----------------------------------------------------------------
      /// <summary>
      /// AddAuditedTables
      /// </summary>
      /// <param name="newTables"></param>
      public void 
         AddAuditedTables (
            int [] newTables
         )
      {
         if( newTables == null ||
            newTables.Length == 0 )
            return;

         for( int i = 0; i < newTables.Length; i++ )
            AuditTable( newTables[i], true );
      }

      //-----------------------------------------------------------------
      // AddAuditedObjects
      //-----------------------------------------------------------------
      /// <summary>
      /// Add an array of audited objects to the configuration.
      /// </summary>
      /// <param name="newObjects"></param>
      public void 
         AddAuditedObjects (
            int [] newObjects
         )
      {
         if( newObjects == null ||
            newObjects.Length == 0 )
            return;

         for( int i = 0; i < newObjects.Length; i++ )
            AddAuditedObject( newObjects[i] );
      }

      #endregion

   }

}
