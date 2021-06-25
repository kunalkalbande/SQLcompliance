using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;

using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Remoting;
using Idera.SQLcompliance.Core.Templates.AuditSettings;
using Idera.SQLcompliance.Core.Templates.IOHandlers;

namespace Idera.SQLcompliance.Core.Templates
{
   public class AuditTemplate : Template
   {
   
      #region Data members
      
      ServerAuditConfig _serverConfig= null;// = new ServerAuditConfig();
      Dictionary<string, DBAuditConfig> _dbConfigs = new Dictionary<string, DBAuditConfig>( );
      UserAuditConfig _userConfig = null;
      //SQLCM-5581, 5582
      UserAuditConfig _trusteduserConfig = null;
      //Start - Requirement 4.1.4.1
      InstanceTemplate instanceTemplate = new InstanceTemplate();
      //End - Requirement 4.1.4.1
      #endregion
      
      #region Constructors
      
      public AuditTemplate()
      {
      }

      public AuditTemplate(string filename)
      {
         Load<AuditTemplate>(filename);
      }

      #endregion 
      
      #region Properties
      
      [XmlElement("ServerLevelAuditConfig")]
      public ServerAuditConfig ServerLevelConfig
      {
         get
         {
            return _serverConfig;
         }
         set
         {
            /*
            if ( value == null )
            {
               _serverConfig.Clear();
               return;
            }
            */
            _serverConfig = value;
         }
      }
      
      [XmlElement( "DatabaseAuditConfig")]
      public DBAuditConfig [] DbLevelConfigs
      {
         get
         {
            if( _dbConfigs.Count == 0 )
               return null;
            List<DBAuditConfig> configs = new List<DBAuditConfig>( _dbConfigs.Count );
            configs.AddRange( _dbConfigs.Values );
            return configs.ToArray();
         }
         set
         {
            _dbConfigs.Clear();
            if (value == null)
            {
               return;
            }
            if (InstanceTemplate.dbDetails != null && InstanceTemplate.dbDetails.Capacity > 0)
                InstanceTemplate.dbDetails.Clear();

            foreach ( DBAuditConfig config in value )
            {
               try
               {
                  _dbConfigs.Add( config.Database,
                                  config );
                  //Start - Requirement 4.1.4.1
                  InstanceTemplate.dbDetails.Add(config.Database);
                  //End - Requirement 4.1.4.1
               }
               catch
               {
               }
            }
         }
      }
     
      [XmlElement( "PrivilegedUserAuditConfig")]
      public UserAuditConfig PrivUserConfig
      {
         get
         {
            return _userConfig;
         }
         set
         {
            _userConfig = value;
         }
      }

      //SQLCM-5581, 5582
      [XmlElement("TrustedUserAuditConfig")]
      public UserAuditConfig TrustedUserConfig
      {
          get
          {
              return _trusteduserConfig;
          }
          set
          {
              _trusteduserConfig = value;
          }
      }
      
      #endregion
      
      #region Public Methods

      /// <summary>
      /// Apply the Server settings contained in this template to the supplied ServerRecord.
      /// </summary>
      /// <param name="server">target server record to apply the settings to</param>
      /// <param name="replace">true to replace the existing server settings, false to add to them</param>
      /// <returns>true</returns>
      public bool ApplyServerSettings(ServerRecord server, bool replace)
      {
         if (replace)
            server.ResetServerAuditSettings();

         if (_serverConfig != null)
         {
            // Apply the template to the server reccord.
            foreach (AuditCategory cat in _serverConfig.Categories)
               UpdateServerRecord(cat, server);
            if (replace)
            { 
               server.AuditAccessCheck = (AccessCheckFilter)_serverConfig.AccessCheckFilter;
               server.AuditTrustedUsersList = _trusteduserConfig.UserList.ToString();
            }
            else
            {
               server.AuditAccessCheck = CombineFilters(_serverConfig.AccessCheckFilter, server.AuditAccessCheck);
               UserList trustedUserList = new UserList();
               trustedUserList.LoadFromString(server.AuditTrustedUsersList);
               foreach(Login login in _trusteduserConfig.UserList.Logins)
                  trustedUserList.AddLogin(login) ;
               server.AuditTrustedUsersList = trustedUserList.ToString();
            }
         }

         return true ;
      }

      /// <summary>
      /// Apply the priv user settings in this template to the supplied server record
      /// </summary>
      /// <param name="server">target server record to apply the settings to</param>
      /// <param name="replace">true to replace the existing priv user settings, false to add to them</param>
      /// <returns></returns>
      public bool ApplyServerUserSettings(ServerRecord server, bool replace)
      {
         if (replace)
            server.ResetUserAuditSettings();

         if (_userConfig != null)
         {
            // Apple priv user settings
            foreach (AuditCategory category in _userConfig.Categories)
            {
                UpdateUserRecord(category, server);
            }

            if (replace)
            {
               server.AuditUsersList = _userConfig.UserList.ToString();
               server.AuditUserAccessCheck = (AccessCheckFilter)_userConfig.AccessCheckFilter;
               server.AuditUserCaptureSQL = _userConfig.KeepSQL;
               server.AuditUserCaptureTrans = _userConfig.CaptureTrans;
               server.AuditUserCaptureDDL = _userConfig.CaptureDDL;
            }
            else
            {
               UserList ul1 = new UserList() ;
               ul1.LoadFromString(server.AuditUsersList);
               foreach(Login l in _userConfig.UserList.Logins)
                  ul1.AddLogin(l) ;
               foreach (ServerRole r in _userConfig.UserList.ServerRoles)
                  ul1.AddServerRole(r) ;
               server.AuditUsersList = ul1.ToString();
               server.AuditUserAccessCheck = CombineFilters(_userConfig.AccessCheckFilter, server.AuditAccessCheck);

               if (_userConfig.KeepSQL)
               {
                  server.AuditUserCaptureSQL = true;
               }

               if (_userConfig.CaptureTrans)
               {
                  server.AuditUserCaptureTrans = true;
            }

               if (_userConfig.CaptureDDL)
               {
                   server.AuditUserCaptureDDL = true;
               }
                  
            }
         }

         return true;
      }

      /// <summary>
      /// Apply the priv user settings in this template to the supplied server record
      /// </summary>
      /// <param name="server">target server record to apply the settings to</param>
      /// <param name="replace">true to replace the existing priv user settings, false to add to them</param>
      /// <returns></returns>
      public bool ApplyDatabasePrivilegedUserSettings(string databaseTemplateName, DatabaseRecord database, bool replace)
      {
          if (!_dbConfigs.ContainsKey(databaseTemplateName))
          {
              return false;
          }

          DBAuditConfig dbConfig = _dbConfigs[databaseTemplateName];

          if (replace)
          {
              database.ResetPrivilegedUsersAuditSettings();
          }

          UserAuditConfig privUserConfig = dbConfig.PrivUserConfig;

          if (privUserConfig != null)
          {
              // Apple priv user settings
              foreach (AuditCategory category in privUserConfig.Categories)
              {
                  UpdateDatabaseRecordForUserSettings(category, database);
              }

              if (replace)
              {
                  database.AuditPrivUsersList = privUserConfig.UserList.ToString();
                  database.AuditUserAccessCheck = (AccessCheckFilter)privUserConfig.AccessCheckFilter;
                  database.AuditUserCaptureSQL = privUserConfig.KeepSQL;
                  database.AuditUserCaptureTrans = privUserConfig.CaptureTrans;
                  database.AuditUserCaptureDDL = privUserConfig.CaptureDDL;
              }
              else
              {
                  UserList userList = new UserList();
                  userList.LoadFromString(database.AuditPrivUsersList);

                  foreach (Login l in privUserConfig.UserList.Logins)
                  {
                      userList.AddLogin(l);
                  }
                      
                  foreach (ServerRole r in privUserConfig.UserList.ServerRoles)
                  {
                      userList.AddServerRole(r);
                  }
                      
              database.AuditPrivUsersList = userList.ToString();
              database.AuditUserAccessCheck = CombineFilters(privUserConfig.AccessCheckFilter, database.AuditUserAccessCheck);

                  if (privUserConfig.KeepSQL)
                  {
                      database.AuditUserCaptureSQL = true;
                  }

                  if (privUserConfig.CaptureTrans)
                  {
                      database.AuditUserCaptureTrans = true;
                  }

                  if (privUserConfig.CaptureDDL)
                  {
                      database.AuditUserCaptureDDL = true;
                  }
              }
         }

         return true;
      }

       //This is for checking DML Selevt Filter from XML file
      public bool CheckDMLSelectFilter(string databaseTemplateName)
      {
          if (!_dbConfigs.ContainsKey(databaseTemplateName))
              return false;
          DBAuditConfig config = _dbConfigs[databaseTemplateName];
          if (!config.AuditAllUserTables)
          {
              return false;
          }
          return true;
      }

      public bool CheckAuditUserDML(string databaseTemplateName)
      {
          if (!_dbConfigs.ContainsKey(databaseTemplateName))
              return false;
          DBAuditConfig config = _dbConfigs[databaseTemplateName];
          foreach (AuditCategory cat in config.Categories)
          {
              switch (cat)
              {
                  case AuditCategory.DML:
                      return true;                      
              }
          }
          return false;
      }



        //This is only called from the CLI when the name of the database in the config doesn't matter.
        public bool ApplyDatabaseSetting(ServerRecord server, DatabaseRecord database,
                                         Dictionary<string, DBO> auditedTables,
                                         Dictionary<string, DataChangeTableRecord> dataChangeTables,
                                         List<SensitiveColumnTableRecord> sensitiveColumnTables,
                                         bool replace, string collectionServer, int port)
        {
            string dbName = null;

         //just get the name of the first dbconfig in the collection and use that one.
         foreach (KeyValuePair<string, DBAuditConfig> kvp in _dbConfigs)
         {
            dbName = kvp.Key;
            break;
         }

         //empty does not count because there could be a config that has an empty db name.
         if (dbName == null)
            return false;

         return ApplyDatabaseSettings(dbName, server, database, auditedTables, dataChangeTables, sensitiveColumnTables, replace, collectionServer, port);
      }

        public bool ApplyDatabaseSettings(string databaseTemplateName, ServerRecord server,
           DatabaseRecord database, Dictionary<string, DBO> auditedTables,
           Dictionary<string, DataChangeTableRecord> dataChangeTables, List<SensitiveColumnTableRecord> sensitiveColumnTables, bool replace, string collectionServer, int port)
        {
            if (!_dbConfigs.ContainsKey(databaseTemplateName))
                return false;
            DBAuditConfig config = _dbConfigs[databaseTemplateName];
            // Update the database record
            UpdateDbRecord(database, config, replace);

         // Handle tables if necessary
         if (database.AuditUserTables == 2 || auditedTables.Count > 0)
         {
            Dictionary<string, DBO> allTables = GetTables(collectionServer, server.Instance, port, database.Name);

            if (replace)
               auditedTables.Clear();
            foreach (string tableName in config.AuditedTables)
            {
               if (allTables.ContainsKey(tableName) && !auditedTables.ContainsKey(tableName))
                  auditedTables.Add(tableName, allTables[tableName]);
            }
            // Special case - if we don't match any tables, turn table auditing off
            if (auditedTables.Count == 0)
               database.AuditUserTables = 0;
         }

         // Handle Data Change Tables
         if (replace)
            dataChangeTables.Clear();

         if (config.DataChangeTables != null && config.DataChangeTables.Length > 0)
         {
            Dictionary<string, DBO> allTables = GetTables(collectionServer, server.Instance, port, database.Name);
            foreach (DataChangeTableConfig tableConfig in config.DataChangeTables)
            {
               if (allTables.ContainsKey(tableConfig.TableName) && !dataChangeTables.ContainsKey(tableConfig.TableName))
               {
                  DataChangeTableRecord tableRecord = new DataChangeTableRecord() ;
                  string schema = "";
                  string table = "";
                  CoreHelpers.GetTableNameFromKey( tableConfig.TableName, out schema, out table );
                  tableRecord.SchemaName = schema;
                  tableRecord.TableName = table;
                  tableRecord.RowLimit = tableConfig.RowCount;
                  foreach (string col in tableConfig.Columns)
                  {
                     tableRecord.AddColumn(col);
                  }
                  tableRecord.ObjectId = allTables[tableConfig.TableName].Id;
                  dataChangeTables.Add(tableConfig.TableName, tableRecord);
               }
            }
         }

         if (replace)
            sensitiveColumnTables.Clear();

            if (config.SensitiveColumnTables != null && config.SensitiveColumnTables.Length > 0)
            {
                Dictionary<string, DBO> allTables = GetTables(collectionServer, server.Instance, port, database.Name);
                foreach (SensitiveColumnTableConfig tableConfig in config.SensitiveColumnTables)
                {
                    bool foundTable = true;
                    Char delimiter = ',', schemaDelimiter = '.';
                    string[] schemaName = tableConfig.TableName.Split(schemaDelimiter);
                    string[] tableNames = tableConfig.TableName.Split(delimiter);
                    foreach (string tableName in tableNames)
                    {
                        string tName = tableName;
                        if (!tName.StartsWith("" + schemaName[0] + ""))
                        {
                            tName = schemaName[0] + "." + tName;
                        }
                        if (!allTables.ContainsKey(tName))
                        {
                            foundTable = false;
                            break;
                        }
                    }
                    if (foundTable && (sensitiveColumnTables.Find(i => i.FullTableName == tableConfig.TableName && tableConfig.Type == CoreConstants.IndividualColumnType) == null))
                    {
                        SensitiveColumnTableRecord tableRecord = new SensitiveColumnTableRecord();
                        string schema = "";
                        string table = "";
                        CoreHelpers.GetTableNameFromKey(tableConfig.TableName, out schema, out table);
                        tableRecord.SchemaName = schema;
                        tableRecord.TableName = table;
                        tableRecord.Type = tableConfig.Type;
                        foreach (string col in tableConfig.Columns)
                        {
                            tableRecord.AddColumn(col);
                        }
                        if (tableConfig.Type == CoreConstants.IndividualColumnType)
                        {
                            tableRecord.ObjectId = allTables[tableConfig.TableName].Id;
                        }
                        else
                        {
                            tableRecord.ObjectId = 0;
                        }
                        sensitiveColumnTables.Add(tableRecord);
                    }

                    else if (foundTable && (sensitiveColumnTables.Find(i => i.FullTableName == tableConfig.TableName && tableConfig.Type == CoreConstants.IndividualColumnType) != null))
                    {
                        SensitiveColumnTableRecord sensitiveColumnTableRecord = new SensitiveColumnTableRecord();
                        string schema = "";
                        string table = "";
                        CoreHelpers.GetTableNameFromKey(tableConfig.TableName, out schema, out table);
                        sensitiveColumnTableRecord.SchemaName = schema;
                        sensitiveColumnTableRecord.TableName = table;
                        sensitiveColumnTableRecord.Type = tableConfig.Type;
                        foreach (string col in tableConfig.Columns)
                        {
                            sensitiveColumnTableRecord.AddColumn(col);
                        }
                        if (tableConfig.Type == CoreConstants.IndividualColumnType)
                        {
                            sensitiveColumnTableRecord.ObjectId = allTables[tableConfig.TableName].ObjectId;
                        }
                        else
                        {
                            sensitiveColumnTableRecord.ObjectId = 0;
                        }
                        if (tableConfig.TableName != null)
                        {
                            bool isAlreadyAdded = false;
                            bool isChangeRequired = false;
                            List<SensitiveColumnTableRecord> newRecordList = new List<SensitiveColumnTableRecord>();
                            foreach (SensitiveColumnTableRecord s in sensitiveColumnTables)
                            {
                                if (s.SchemaName == sensitiveColumnTableRecord.SchemaName &&
                                    s.TableName == sensitiveColumnTableRecord.TableName &&
                                    s.Type == sensitiveColumnTableRecord.Type &&
                                    s.Columns[0] == "All Columns")
                                {
                                    isAlreadyAdded = true;
                                    break;
                                }
                                else if (s.SchemaName == sensitiveColumnTableRecord.SchemaName &&
                                    s.TableName == sensitiveColumnTableRecord.TableName &&
                                    s.Type == sensitiveColumnTableRecord.Type && 
                                    sensitiveColumnTableRecord.Columns[0] == "All Columns")
                                {
                                    isChangeRequired = true;
                                    sensitiveColumnTableRecord.ObjectId = s.ObjectId;
                                    break;
                                }
                                else if (s.SchemaName == sensitiveColumnTableRecord.SchemaName &&
                                    s.TableName == sensitiveColumnTableRecord.TableName &&
                                    s.Type == sensitiveColumnTableRecord.Type)
                                {
                                    isChangeRequired = true;
                                    sensitiveColumnTableRecord.ObjectId = s.ObjectId;
                                    List<string> existingColumns = new List<string>(sensitiveColumnTableRecord.Columns);
                                    foreach (string column in s.Columns)
                                    {
                                        if (!existingColumns.Contains(column))
                                        {
                                            sensitiveColumnTableRecord.AddColumn(column);
                                        }
                                    }
                                    break;
                                }

                            }
                            if (!isAlreadyAdded && !isChangeRequired)
                            {
                                sensitiveColumnTables.Add(sensitiveColumnTableRecord);
                            }
                            else if(isChangeRequired)
                            {
                                sensitiveColumnTables.Remove(sensitiveColumnTables.Find(i => i.TableName == sensitiveColumnTableRecord.TableName
                                    && i.SchemaName == sensitiveColumnTableRecord.SchemaName
                                    && i.Type == sensitiveColumnTableRecord.Type));
                                sensitiveColumnTables.Add(sensitiveColumnTableRecord);
                            }
                        }
                    } 

                }
            }
            return true;
        }

      //
      // Import an instance audit settings as a template
      //
      public override bool Import( string instance )
      {
         try
         {
            RemoteAuditConfiguration config;
            using( SqlConnection conn = OpenNewConnection() )
            {
               config = RemoteAuditManager.GetConfigurationFromDatabase( instance, conn );
            }
            
            if( _serverConfig == null )
               _serverConfig = new ServerAuditConfig();
               
            _serverConfig.Categories = config.Categories;
            _serverConfig.AccessCheckFilter =
               (AccessCheckFilterOption) config.AccessCheck;

            if(_trusteduserConfig == null )
              _trusteduserConfig = new UserAuditConfig();
            _trusteduserConfig.UserList = new UserList(config.ServerTrustedUsersData);

            foreach (DBRemoteAuditConfiguration dbConfig in config.DBConfigs)
               ImportDbConfig(instance, dbConfig);
            
            ImportUserConfig( instance, config );
                  
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     "An error occurred importing audit settings template.",
                                     e,
                                     true );
            throw;
         }
         return true;
      }

      //
      // Export an instance's settings to an XML template file
      //
      public override bool Export( string instance,
                                   string filename,
                                   bool overwrite )
      {
         bool success;
         try
         {
            if ( Import( instance ) )
            {
               success = SaveThis( filename,
                               overwrite );
            }
            else 
               success = false;
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     String.Format( "An error occurred exporting {0} to an XML template file.",GetType()),
                                     e,
                                     true );
            throw;
         }
         return success;
      }

      public override bool Load(string filename)
      {
         try
         {
            AuditTemplate tmp;
            XMLHandler<AuditTemplate> reader = new XMLHandler<AuditTemplate>();
            tmp = reader.Read(filename);
            
            _serverConfig = tmp.ServerLevelConfig;
            _dbConfigs.Clear();
            foreach( DBAuditConfig config in tmp.DbLevelConfigs )
            {
               _dbConfigs.Add( config.Database,
                               config );
            }
            _userConfig = tmp.PrivUserConfig;
            return true;
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                     String.Format("An error occurred reading the XML template file {0}.",
                                                    filename),
                                     e,
                                     true);
            return false;
         }
      }
      
      public override bool Save(string filename)
      {
         return SaveThis(filename, true);
      }

      #endregion

      #region Private Methods
      
       bool SaveThis( string filename,
                     bool overwrite )
       {
          FileInfo info = new FileInfo( filename );
          if ( info.Exists )
          {
             if ( !overwrite )
             {
                ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                         String.Format( TemplateFileExistsError,
                                                        filename ),
                                         ErrorLog.Severity.Warning );
                return false;
             }
             else
             {
                info.Delete();
             }
          }

          try
          {
             XMLHandler<AuditTemplate> writer = new XMLHandler<AuditTemplate>();
             writer.Write( filename,
                           this,
                           overwrite );
             return true;
          }
          catch ( Exception e )
          {
             ErrorLog.Instance.Write( ErrorLog.Level.Default,
                                      "An error occurred writing the XML template file.",
                                      e,
                                      true );
             return false;
          }
      }

      private void ImportUserConfig(string instance, RemoteAuditConfiguration config)
      {
         if( (config.ServerRoles == null || config.ServerRoles.Length == 0 ) &&
             (config.Users == null || config.Users.Length == 0 ) )
            return;
            
         _userConfig = new UserAuditConfig();
         using ( SqlConnection conn = OpenNewConnection() )
         {
            _userConfig.UserList = ConfigurationHelper.GetAuditedUserList( instance,
                                                                                 conn );
         }
         _userConfig.Categories = config.UserConfig.Categories;
         _userConfig.AccessCheckFilter = (AccessCheckFilterOption)config.UserConfig.AccessCheck;
         _userConfig.KeepSQL = config.UserConfig.CaptureDetails;
         _userConfig.CaptureTrans = config.UserConfig.CaptureTransactions;
         _userConfig.CaptureDDL = config.UserConfig.CaptureDDL;
         _userConfig.KeepSQLXE = config.UserConfig.CaptureDetailsXE;  //5.4_4.1.1_Extended Events
      }


      private void ImportUserConfigCwf(string instance, RemoteAuditConfiguration config, SqlConnection connection)
      {
          if ((config.ServerRoles == null || config.ServerRoles.Length == 0) &&
              (config.Users == null || config.Users.Length == 0))
              return;

          _userConfig = new UserAuditConfig();
          _userConfig.UserList = ConfigurationHelper.GetAuditedUserList(instance,connection);
          _userConfig.Categories = config.UserConfig.Categories;
          _userConfig.AccessCheckFilter = (AccessCheckFilterOption)config.UserConfig.AccessCheck;
          _userConfig.KeepSQL = config.UserConfig.CaptureDetails;
          _userConfig.CaptureTrans = config.UserConfig.CaptureTransactions;
          _userConfig.CaptureDDL = config.UserConfig.CaptureDDL;
          _userConfig.KeepSQLXE = config.UserConfig.CaptureDetailsXE;  //5.4_4.1.1_Extended Events
      }

      private List<DBObjectType> GetObjectTypes(DatabaseRecord database, string connectionString)
      {
          var objectTypes = new List<DBObjectType>();
          var server = ServerRecord.GetServerRecord(database.SrvId, connectionString);
          int sqlVersion = server.SqlVersion;

          if (!database.AuditDmlAll)
          {

              switch (database.AuditUserTables)
              {
                  case 1:  // All user tables
                      objectTypes.Add(DBObject.GetUserTableTypeValue(sqlVersion));
                      break;
                  case 2:  // Selected user tables
                      objectTypes.Add(DBObject.GetUserTableTypeValue(sqlVersion));
                      //config.AuditObjects = GetAuditedUserTables( dbId, conn.ConnectionString ) ; // SQLcompliance DB Id
                      break;
              }
          }

          if (database.AuditSystemTables)
          {
              objectTypes.Add(DBObject.GetSystemTableTypeValue(sqlVersion));
          }

          if (database.AuditStoredProcedures)
          {
              DBObjectType[] spTypes = DBObject.GetSPTypeValues(sqlVersion);
              objectTypes.AddRange(spTypes);
          }

          if (database.AuditDmlOther)
          {
              DBObjectType[] dmlOtherTypes = DBObject.GetDmlOtherTypeValues(sqlVersion);
              objectTypes.AddRange(dmlOtherTypes);
          }

          return objectTypes;
      }

      private int[] GetAuditObjects(DatabaseRecord database, string connectionString)
      {
          if (!database.AuditDmlAll &&
               database.AuditUserTables == 2) // Selected user tables
          {
              return RemoteAuditManager.GetAuditedUserTables(database.DbId, connectionString);
          }

          return null;
      }

      public void ExportDatabase(DatabaseRecord database, string connectionString)
      {
          var config = new DBAuditConfig();
          config.Database = database.Name;
          config.KeepSQL = database.AuditCaptureSQL;
          config.AuditTrans = database.AuditCaptureTrans;
          config.AuditDDL = database.AuditCaptureDDL;    
          config.AccessCheckFilter = (AccessCheckFilterOption) database.AuditAccessCheck; 

          if (!string.IsNullOrEmpty( database.AuditUsersList ))
          {
              config.TrustedUserList = new UserList(database.AuditUsersList);
          }

          if (!string.IsNullOrEmpty( database.AuditPrivUsersList ))
          {
              config.PrivUserConfig = GetPrevelegeDatabaseUserConfig(database);
          }

          config.Categories = database.GetAuditCategoryList().ToArray();


          var objectTypes = GetObjectTypes(database, connectionString);

          int[] auditObjects = new int[0];

          if (objectTypes.Count == 0)
          {
              config.AuditAllObjects = true;
          }
          else
          {
              config.AuditAllObjects = false;

              auditObjects = GetAuditObjects(database, connectionString);

              if (auditObjects == null || 
                  auditObjects.Length == 0)
              {
                  config.AuditAllUserTables = true;
              }
              else
              {
                  var server = ServerRecord.GetServerRecord(database.SrvId, connectionString);

                  config.AuditAllUserTables = false;
                  config.AuditedTables = ConfigurationHelper.GetAuditedTableNames(server.Instance, database.Name, connectionString);
              }

              foreach (DBObjectType type in objectTypes)
              {
                  switch (type)
                  {
                      case DBObjectType.UserTable:
                      case DBObjectType.UserTable_2005:
                          config.AddAuditedType(AuditedObjectTypes.UserTables);
                          break;

                      case DBObjectType.SystemTable:
                      case DBObjectType.SystemTable_2005:
                          config.AddAuditedType(AuditedObjectTypes.SystemTables);
                          break;

                      case DBObjectType.StoredProcedure_2005:
                      case DBObjectType.ExtendedStoredProcedure_2005:
                      case DBObjectType.CLRStoredProcedure_2005:
                      case DBObjectType.StoredProcedure:
                      case DBObjectType.ExtendStoredProcedure:
                          config.AddAuditedType(AuditedObjectTypes.StoredProcedures);
                          break;

                      default:
                          config.AddAuditedType(AuditedObjectTypes.AllOtherTypes);
                          break;
                  }
              }
          }

          var dataChangeTables = RemoteAuditManager.GetDataChangeTables(database.SrvId, database.DbId, connectionString, auditObjects);
          config.SetDataChangeTables(dataChangeTables);

          var sensitiveColumnTables = RemoteAuditManager.GetSensitiveColumnTables(database.SrvId, database.DbId, connectionString, auditObjects);
          config.SetSensitiveColumnTables(sensitiveColumnTables);

          _dbConfigs.Add(config.Database, config);
      }

      private void ImportDbConfig(string instance, DBRemoteAuditConfiguration dbConfig)
      {
         DBAuditConfig config;
         
         config = new DBAuditConfig();
         config.Database = dbConfig.dbName;
         config.KeepSQL = dbConfig.CaptureDetails;
         config.AuditTrans = dbConfig.CaptureTransactions;
         config.AuditDDL = dbConfig.CaptureDDL;
         config.AccessCheckFilter = (AccessCheckFilterOption) dbConfig.AccessCheck;
         if(( dbConfig.ServerRoles != null && dbConfig.ServerRoles.Length > 0 )|| 
            ( dbConfig.Users != null && dbConfig.Users.Length > 0 ))
         {
            config.TrustedUserList =
               ConfigurationHelper.GetTrustedUserList( instance, dbConfig.dbName, _connectionString );
         }

         if ( (dbConfig.PrivServerRoles != null && dbConfig.PrivServerRoles.Length > 0 ) ||
              (dbConfig.PrivUsers != null && dbConfig.PrivUsers.Length > 0))
         {
             config.PrivUserConfig = GetPrevelegeDatabaseUserConfig(instance,dbConfig);
         }
         
         config.Categories = dbConfig.Categories;
         config.SetDataChangeTables(dbConfig.DataChangeTables);
         config.SetSensitiveColumnTables(dbConfig.SensitiveColumns);

         if ( dbConfig.ObjectTypes == null || dbConfig.ObjectTypes.Length == 0 )
            config.AuditAllObjects = true;
         else
         {
            config.AuditAllObjects = false;
            if (dbConfig.AuditObjects == null || dbConfig.AuditObjects.Length == 0)
               config.AuditAllUserTables = true;
            else
            {
               config.AuditAllUserTables = false;
               config.AuditedTables = ConfigurationHelper.GetAuditedTableNames( instance, dbConfig.dbName, _connectionString ); 
            }

            foreach ( DBObjectType type in dbConfig.ObjectTypes )
            {
               switch ( type )
               {
                  case DBObjectType.UserTable:
                  case DBObjectType.UserTable_2005:
                     config.AddAuditedType( AuditedObjectTypes.UserTables );
                     break;

                  case DBObjectType.SystemTable:
                  case DBObjectType.SystemTable_2005:
                     config.AddAuditedType( AuditedObjectTypes.SystemTables );
                     break;

                  case DBObjectType.StoredProcedure_2005:
                  case DBObjectType.ExtendedStoredProcedure_2005:
                  case DBObjectType.CLRStoredProcedure_2005:
                  case DBObjectType.StoredProcedure:
                  case DBObjectType.ExtendStoredProcedure:
                     config.AddAuditedType( AuditedObjectTypes.StoredProcedures );
                     break;

                  default:
                     config.AddAuditedType( AuditedObjectTypes.AllOtherTypes );
                     break;
               }
            }
         }

         _dbConfigs.Add(config.Database, config);
      }

      private UserAuditConfig GetPrevelegeDatabaseUserConfig(string srvInstance, DBRemoteAuditConfiguration dbConfig)
      {

          UserAuditConfig userConfig = null;

          if ((dbConfig.PrivServerRoles == null || dbConfig.PrivServerRoles.Length == 0) &&
              (dbConfig.PrivUsers == null || dbConfig.PrivUsers.Length == 0))
          {
              return userConfig;
          }

          userConfig = new UserAuditConfig();

          using (SqlConnection conn = OpenNewConnection())
          {
              userConfig.UserList = ConfigurationHelper.GetDatabaseAuditedPrevelegedUserList(srvInstance, dbConfig.DbId, conn);
          }

          userConfig.Categories = dbConfig.UserCategories;
          userConfig.AccessCheckFilter = (AccessCheckFilterOption)dbConfig.UserAccessCheck;
          userConfig.KeepSQL = dbConfig.UserCaptureSql;
          userConfig.CaptureTrans = dbConfig.UserCaptureTran;
          userConfig.CaptureDDL = dbConfig.UserCaptureDDL;

          return userConfig;
      }
      
      private UserAuditConfig GetPrevelegeDatabaseUserConfig(DatabaseRecord database)
      {

          UserAuditConfig userConfig = null;

          if ( string.IsNullOrEmpty(database.AuditPrivUsersList))
          {
              return userConfig;
          }

          userConfig = new UserAuditConfig();
          userConfig.UserList = new UserList(database.AuditPrivUsersList);
          userConfig.Categories = database.GetUserAuditCategoryList().ToArray();
          

          if (database.AuditUserAll)
          {
              userConfig.KeepSQL = true;
              userConfig.CaptureTrans = true;
              userConfig.CaptureDDL = true;
              userConfig.AccessCheckFilter = AccessCheckFilterOption.CapturePassedCheckActions;
          }
          else
          {
          userConfig.AccessCheckFilter = (AccessCheckFilterOption)database.AuditUserAccessCheck;
          userConfig.KeepSQL = database.AuditUserCaptureSQL;
          userConfig.CaptureTrans = database.AuditUserCaptureTrans;
          userConfig.CaptureDDL = database.AuditUserCaptureDDL;
          }

          return userConfig;
      }
      
      private void UpdateServerRecord( AuditCategory cat, ServerRecord record)
      {
         switch ( cat )
         {
            // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
            case AuditCategory.Logouts:
             record.AuditLogouts = true;
             break;
            case AuditCategory.Logins:
                 record.AuditLogins = true;
                 break;
            case AuditCategory.DDL:
               record.AuditDDL = true;
               break;
            case AuditCategory.Security:
               record.AuditSecurity = true;
               break;
            case AuditCategory.Admin:
               record.AuditAdmin = true;
               break;
            case AuditCategory.FailedLogins:
               record.AuditFailedLogins = true;
               break;
            case AuditCategory.UDC: // User defined
               record.AuditUDE = true;
               break;
         }
      }

      // Performs an OR operation on the two filters
      private AccessCheckFilter CombineFilters(AccessCheckFilterOption importingFilter, AccessCheckFilter currentFilter)
      {
          switch(currentFilter)
          {
              case AccessCheckFilter.NoFilter:
              
                  switch(importingFilter)
         {
            case AccessCheckFilterOption.CapturePassedCheckActions:
                          return AccessCheckFilter.SuccessOnly;

            case AccessCheckFilterOption.CaptureFailedCheckActions:
                          return AccessCheckFilter.FailureOnly;
         }

                  break;

              case AccessCheckFilter.FailureOnly:

                  switch(importingFilter)
                  {
                      case AccessCheckFilterOption.CapturePassedCheckActions:
                          return AccessCheckFilter.SuccessOnly;
                  }

                  break;
          }

          return currentFilter;
      }

      private void UpdateUserRecord(AuditCategory category, ServerRecord record)
      {
         switch ( category )
         {
            case AuditCategory.Logins:
               record.AuditUserLogins = true;
               break;
            // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
            case AuditCategory.Logouts:
               record.AuditUserLogouts = true;
               break;
            case AuditCategory.DDL:
               record.AuditUserDDL = true;
               break;
            case AuditCategory.Security:
               record.AuditUserSecurity = true;
               break;
            case AuditCategory.Admin:
               record.AuditUserAdmin = true;
               break;
            case AuditCategory.FailedLogins:
               record.AuditUserFailedLogins = true;
               break;
            case AuditCategory.UDC: // User defined
               record.AuditUserUDE = true;
               break;
            case AuditCategory.SELECT:
               record.AuditUserSELECT = true;
               break;
            case AuditCategory.DML:
               record.AuditUserDML = true;
               break;
         }
      }

      private void UpdateDatabaseRecordForUserSettings(AuditCategory category, DatabaseRecord database)
      {
         switch ( category )
         {
            case AuditCategory.Logins:
               database.AuditUserLogins = true;
               break;
            // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
            case AuditCategory.Logouts:
               database.AuditUserLogouts = true;
               break;
            case AuditCategory.DDL:
               database.AuditUserDDL = true;
               break;
            case AuditCategory.Security:
               database.AuditUserSecurity = true;
               break;
            case AuditCategory.Admin:
               database.AuditUserAdmin = true;
               break;
            case AuditCategory.FailedLogins:
               database.AuditUserFailedLogins = true;
               break;
            case AuditCategory.UDC: // User defined
               database.AuditUserUDE = true;
               break;
            case AuditCategory.SELECT:
               database.AuditUserSELECT = true;
               break;
            case AuditCategory.DML:
               database.AuditUserDML = true;
               break;
         }
      }

      Dictionary<string, DBO> GetTables(string server, string instance, int port, string database)
      {
         Dictionary<string, DBO> tables = null;
         IList list = null;
         string url = "";
         try
         {
            try
            {
               url = String.Format("tcp://{0}:{1}/{2}",
                                    server,
                                    port,
                                    typeof(AgentManager).Name);
               AgentManager manager = GetAgentManager(server,
                                                       port);

               list = manager.GetRawTables(instance, database);
            }
            catch
            {
            }

            if (list == null)
               list = GetTablesDirect(instance, database);
            tables = new Dictionary<string, DBO>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
               RawTableObject table = (RawTableObject)list[i];
               tables.Add(table.FullTableName, new DBO(table));
            }
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                     String.Format("LoadTables: URL: {0} Instance {1} Database {2}",
                                                    url,
                                                    instance,
                                                    database),
                                     ex,
                                     ErrorLog.Severity.Warning);
            throw ;
         }
         return tables;
      }

      // Apply settings to an audited database
      private void UpdateDbRecord(DatabaseRecord record, DBAuditConfig config, bool replace)
      {
         if (replace)
            record.ResetAuditSettings() ;

         // Basic DB Settings
         foreach (AuditCategory cat in config.Categories)
         {
            switch (cat)
            {
               case AuditCategory.DDL:
                  record.AuditDDL = true;
                  break;
               case AuditCategory.Security:
                  record.AuditSecurity = true;
                  break;
               case AuditCategory.Admin:
                  record.AuditAdmin = true;
                  break;
               case AuditCategory.SELECT:
                  record.AuditSELECT = true;
                  break;
               case AuditCategory.DML:
                  record.AuditDML = true;
                  break;
               case AuditCategory.UDC:
                  record.AuditDataChanges = true;
                  break;

            }
         }
         if (replace)
         {
            record.AuditCaptureSQL = config.KeepSQL;
            record.AuditCaptureTrans = config.AuditTrans;
            record.AuditCaptureDDL = config.AuditDDL;
            record.AuditAccessCheck = (AccessCheckFilter)config.AccessCheckFilter;
         }
         else
         {
            record.AuditCaptureSQL |= config.KeepSQL;
            record.AuditCaptureTrans |= config.AuditTrans;
            record.AuditCaptureDDL |= config.AuditDDL;
            record.AuditAccessCheck = CombineFilters(config.AccessCheckFilter, record.AuditAccessCheck);
         }

         // DML/Select Filtering
         if (record.AuditDML || record.AuditSELECT)
         {
            if (record.AuditDmlAll || config.AuditAllObjects)
            {
               record.AuditDmlAll = true;
               record.AuditUserTables = 1;
               record.AuditSystemTables = false;
               record.AuditStoredProcedures = false;
               record.AuditDmlOther = false;
            }
            else
            {
               record.AuditDmlAll = false;
               if (config.AuditAllUserTables || record.AuditUserTables == 1)
                  record.AuditUserTables = 1;
               else if(record.AuditUserTables == 2 || (config.AuditedTables != null &&
                  config.AuditedTables.Length > 0))
                  record.AuditUserTables = 2 ;
               else
                  record.AuditUserTables = 0 ;

               foreach (AuditedObjectTypes type in config.AuditedTypes)
               {
                  switch (type)
                  {
                     case AuditedObjectTypes.SystemTables:
                        record.AuditSystemTables = true;
                        break;

                     case AuditedObjectTypes.StoredProcedures:
                        record.AuditStoredProcedures = true;
                        break;

                     case AuditedObjectTypes.AllOtherTypes:
                        record.AuditDmlOther = true;
                        break;
                  }
               }
            }
         }
         
         // Trusted Users
         UserList ul1 = new UserList();
         ul1.LoadFromString(record.AuditUsersList);
         if (config.TrustedUserList != null)
         {
            foreach(Login l in config.TrustedUserList.Logins)
               ul1.AddLogin(l) ;
            foreach(ServerRole r in config.TrustedUserList.ServerRoles)
               ul1.AddServerRole(r) ;
         }
         record.AuditUsersList = ul1.ToString();
      }

      AgentManager GetAgentManager( string server, int port )
      {
         try
         {
             return CoreRemoteObjectsProvider.AgentManager(server, port);
         }
         catch ( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     e,
                                     true );
            throw;
         }
      }
      
      public IList GetTablesDirect(string instance, string dbName)
      {
         IList tableList;

         try
         {
            string strConn = String.Format("server={0};" +
                                            "integrated security=SSPI;" +
                                            "Connect Timeout=30;" +
                                            "Application Name='{1}';",
                                            instance,
                                            CoreConstants.ManagementConsoleName);
                                            
            using( SqlConnection conn = new SqlConnection(strConn))
            {
               conn.Open();
               // Load Databases			   
               string cmdfmt = "SELECT name,id,xtype" +
                               " FROM [{0}]..sysobjects " +
                               " WHERE xtype='U'" +
                               " ORDER by name ASC";
               string cmdstr = String.Format( cmdfmt,
                                              dbName );

               using ( SqlCommand cmd = new SqlCommand( cmdstr,
                                                        conn ) )
               {
                  using ( SqlDataReader reader = cmd.ExecuteReader() )
                  {
                     tableList = new ArrayList();

                     while ( reader.Read() )
                     {
                        RawTableObject raw = new RawTableObject();
                        raw.TableName = reader.GetString( 0 );
                        raw.id = reader.GetInt32( 1 );
                        tableList.Add( raw );
                     }
                  }
               }
            }
         }
         catch
         {
            throw;
         }

         return tableList;
      }
      #endregion

      #region 5.4

      ///<Export Audit Setting Serever Details> 
      ///

      public bool ImportCwf(SqlConnection connection, string instance)
      {
          try
          {
              RemoteAuditConfiguration config;
              config = RemoteAuditManager.GetConfigurationFromDatabase(instance, connection);


              if (_serverConfig == null)
                  _serverConfig = new ServerAuditConfig();

              _serverConfig.Categories = config.Categories;
              _serverConfig.AccessCheckFilter =
                 (AccessCheckFilterOption)config.AccessCheck;

              foreach (DBRemoteAuditConfiguration dbConfig in config.DBConfigs)
                  ImportDbConfigCwf(instance, dbConfig, connection);

              ImportUserConfigCwf(instance, config, connection);

          }
          catch (Exception e)
          {
              ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                       "An error occurred importing audit settings template.",
                                       e,
                                       true);
              throw;
          }
          return true;
      }


      private void ImportDbConfigCwf(string instance, DBRemoteAuditConfiguration dbConfig, SqlConnection connection)
      {
          DBAuditConfig config;

          config = new DBAuditConfig();
          config.Database = dbConfig.dbName;
          config.KeepSQL = dbConfig.CaptureDetails;
          config.AuditTrans = dbConfig.CaptureTransactions;
          config.AuditDDL = dbConfig.CaptureDDL;
          config.AccessCheckFilter = (AccessCheckFilterOption)dbConfig.AccessCheck;
          if ((dbConfig.ServerRoles != null && dbConfig.ServerRoles.Length > 0) ||
             (dbConfig.Users != null && dbConfig.Users.Length > 0))
          {
              config.TrustedUserList =
                 ConfigurationHelper.GetTrustedUserList(instance, dbConfig.dbName, connection.ConnectionString);
          }

          if ((dbConfig.PrivServerRoles != null && dbConfig.PrivServerRoles.Length > 0) ||
               (dbConfig.PrivUsers != null && dbConfig.PrivUsers.Length > 0))
          {
              config.PrivUserConfig = GetPrevelegeDatabaseUserConfigCwf(instance, dbConfig,connection);
          }

          config.Categories = dbConfig.Categories;
          config.SetDataChangeTables(dbConfig.DataChangeTables);
          config.SetSensitiveColumnTables(dbConfig.SensitiveColumns);

          if (dbConfig.ObjectTypes == null || dbConfig.ObjectTypes.Length == 0)
              config.AuditAllObjects = true;
          else
          {
              config.AuditAllObjects = false;
              if (dbConfig.AuditObjects == null || dbConfig.AuditObjects.Length == 0)
                  config.AuditAllUserTables = true;
              else
              {
                  config.AuditAllUserTables = false;
                  config.AuditedTables = ConfigurationHelper.GetAuditedTableNames(instance, dbConfig.dbName, connection.ConnectionString);
              }

              foreach (DBObjectType type in dbConfig.ObjectTypes)
              {
                  switch (type)
                  {
                      case DBObjectType.UserTable:
                      case DBObjectType.UserTable_2005:
                          config.AddAuditedType(AuditedObjectTypes.UserTables);
                          break;

                      case DBObjectType.SystemTable:
                      case DBObjectType.SystemTable_2005:
                          config.AddAuditedType(AuditedObjectTypes.SystemTables);
                          break;

                      case DBObjectType.StoredProcedure_2005:
                      case DBObjectType.ExtendedStoredProcedure_2005:
                      case DBObjectType.CLRStoredProcedure_2005:
                      case DBObjectType.StoredProcedure:
                      case DBObjectType.ExtendStoredProcedure:
                          config.AddAuditedType(AuditedObjectTypes.StoredProcedures);
                          break;

                      default:
                          config.AddAuditedType(AuditedObjectTypes.AllOtherTypes);
                          break;
                  }
              }
          }

          _dbConfigs.Add(config.Database, config);
      }

      private UserAuditConfig GetPrevelegeDatabaseUserConfigCwf(string srvInstance, DBRemoteAuditConfiguration dbConfig,SqlConnection connection)
      {

          UserAuditConfig userConfig = null;

          if ((dbConfig.PrivServerRoles == null || dbConfig.PrivServerRoles.Length == 0) &&
              (dbConfig.PrivUsers == null || dbConfig.PrivUsers.Length == 0))
          {
              return userConfig;
          }

          userConfig = new UserAuditConfig();
          userConfig.UserList = ConfigurationHelper.GetDatabaseAuditedPrevelegedUserList(srvInstance, dbConfig.DbId, connection);
          
          userConfig.Categories = dbConfig.UserCategories;
          userConfig.AccessCheckFilter = (AccessCheckFilterOption)dbConfig.UserAccessCheck;
          userConfig.KeepSQL = dbConfig.UserCaptureSql;
          userConfig.CaptureTrans = dbConfig.UserCaptureTran;
          userConfig.CaptureDDL = dbConfig.UserCaptureDDL;

          return userConfig;
      }

      #endregion 5.4
   }
}
