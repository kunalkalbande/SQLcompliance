using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Collector;
using SnmpSharpNet;
using Idera.SQLcompliance.Core.Sql;

namespace Idera.SQLcompliance.Core.Event
{

   /// <summary>
   /// Summary description for RemoteAuditManager.
   /// </summary>
   public class RemoteAuditManager : MarshalByRefObject
   {
      #region Instance variables	


      #endregion		

      #region Constructors
       /* Not needed.
      static RemoteAuditManager()
      {

      }
        * */
      #endregion

      #region Public/Internal Methods

      //-------------------------------------------------------------------------
      // GetCurrentAuditConfiguration
      //-------------------------------------------------------------------------
      /// <summary>
      /// 
      /// </summary>
      /// <param name="instance"></param>
      /// <returns></returns>
      public RemoteAuditConfiguration 
         GetCurrentAuditConfiguration (
            string               instance
         )
      {
         ServerRecord.UpdateLastAgentContact( instance );

         RemoteAuditConfiguration currentConfig = new RemoteAuditConfiguration();

         using( SqlConnection conn = GetConnection() )
         {
            try
            {
               // Get configuration record
               currentConfig = GetConfigurationFromDatabase( instance, conn );
               if (currentConfig.DBConfigs != null)
               {
                   int dbConfigCount = currentConfig.DBConfigs.Length;
                   for (int i = 0; i < dbConfigCount; i++)
                   {
                       if (currentConfig.DBConfigs[i].SensitiveColumns != null)
                       {
                           List<TableConfiguration> sensitiveTables = new List<TableConfiguration>();
                           List<string> tableNames = new List<string>();
                           foreach (TableConfiguration tableConfig in currentConfig.DBConfigs[i].SensitiveColumns)
                           {
                               foreach (string tableName in tableConfig.Name.Split(','))
                               {
                                   if (!tableNames.Contains(tableName))
                                   {
                                       TableConfiguration table = new TableConfiguration();
                                       table.Name = tableName;
                                       table.Schema = tableConfig.Schema;
                                       table.SrvId = tableConfig.SrvId;
                                       table.Id = tableConfig.Id;
                                       table.MaxRows = tableConfig.MaxRows;
                                       table.StructVersion = tableConfig.StructVersion;
                                       sensitiveTables.Add(table);
                                       tableNames.Add(tableName);
                                   }
                               }
                           }
                           currentConfig.DBConfigs[i].SensitiveColumns = sensitiveTables.ToArray();
                       }
                   }
               }
            }
            catch( Exception e)
            {
               // Errors and exception should be handled in GetConfigurationFromDatabase.
               // This is just a safty net.
               ErrorLog.Instance.Write( ErrorLog.Level.Default,
                                        "Error getting current configuration", 
                                        e,
                                        true);
            }
            finally
            {
               conn.Close();
            }
         }

         return currentConfig;
      }

      
      //-------------------------------------------------------------------------
      // GetLatestAuditVersion
      //-------------------------------------------------------------------------
      /// <summary>
      /// 
      /// </summary>
      /// <param name="instance"></param>
      /// <returns></returns>
      public int
         GetLatestAuditVersion ( string instance )
      {
         ServerRecord.UpdateLastAgentContact( instance );
         
         int version;

         try
         {
             using (SqlConnection conn = GetConnection())
             {
                 version = GetConfigVersion(instance, conn);
                 conn.Close();
             }
         }
         catch( Exception e )
         {
             ErrorLog.Instance.Write(ErrorLog.Level.Debug, e, ErrorLog.Severity.Error);
             throw;
         }

         return version;

      }

      //-------------------------------------------------------------------------------------------------
      // GetConfigurationFromDatabase
      //-------------------------------------------------------------------------------------------------
      /// <summary>
      /// 
      /// </summary>
      /// <param name="instance"></param>
      /// <param name="conn"></param>
      /// <returns></returns>
      static internal RemoteAuditConfiguration
         GetConfigurationFromDatabase(
            string instance,
            SqlConnection conn
         )
      {
         List<AuditCategory> categories = new List<AuditCategory>();
         RemoteAuditConfiguration remoteConfig = new RemoteAuditConfiguration();
         string userListXML = null;
        //v5.6 SQLCM-5373
        string trustedUserListXML = null;
         bool auditAllUserActivities = false;
         bool excludeSystemEvents = false;
         int columnIdx = 0;
         int sqlVersion = 9;
         string agentVersion = "1.1";
            try
            {
                string commandText = String.Format("SELECT "
                   + "auditLogins, " //0
                   + "auditFailedLogins,"
                   + "auditDDL,"
                   + "auditSecurity,"
                   + "auditAdmin, "
                   + "auditSystemEvents, "
                   + "auditUDE, "
                   + "auditFailures, "
                   + "auditCaptureSQL, "
                   + "auditExceptions, "
                   + "configVersion, "
                   + "timeLastModified, "
                   + "auditUsersList, "
                   + "auditUserAll, "
                   + "isEnabled, "
                   + "isSqlSecureDb, "
                   + "sqlVersion, "
                   + "agentVersion, "
                   + "auditUserCaptureDDL, "
                   + "auditUserExtendedEvents, "
                   + "auditCaptureSQLXE,"
                   + "isAuditLogEnabled, "
                   + "auditLogouts," // SQLCM-5375-6.1.4.3-Capture Logout Events
                   + "auditTrustedUsersList " //v5.6 SQLCM-5373
                   + " FROM {0} "
                   + "WHERE instance = {1}",
                   CoreConstants.RepositoryServerTable,
                   SQLHelpers.CreateSafeString(instance));

                using (SqlCommand command = new SqlCommand(commandText, conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (!reader.IsDBNull(columnIdx))
                                if ((int)reader.GetSqlByte(columnIdx) == 1)
                                    categories.Add(AuditCategory.Logins);
                            columnIdx++;

                            if (!reader.IsDBNull(columnIdx))
                                if ((int)reader.GetSqlByte(columnIdx) == 1)
                                    categories.Add(AuditCategory.FailedLogins);
                            columnIdx++;

                            if (!reader.IsDBNull(columnIdx))
                                if ((int)reader.GetSqlByte(columnIdx) == 1)
                                    categories.Add(AuditCategory.DDL);
                            columnIdx++;

                            if (!reader.IsDBNull(columnIdx))
                                if ((int)reader.GetSqlByte(columnIdx) == 1)
                                    categories.Add(AuditCategory.Security);
                            columnIdx++;

                            if (!reader.IsDBNull(columnIdx))
                                if ((int)reader.GetSqlByte(columnIdx) == 1)
                                    categories.Add(AuditCategory.Admin);
                            columnIdx++;

                            if (!reader.IsDBNull(columnIdx))
                            {
                                /* Disable for 2.0 release
                                if( (int)reader.GetSqlByte(columnIdx) == 1 )
                                   categories.Add( AuditCategory.SystemEvents );
                                else
                                   excludeSystemEvents = true;
                                   */
                            }

                            columnIdx++;

                            if (!reader.IsDBNull(columnIdx))
                                if ((int)reader.GetSqlByte(columnIdx) == 1)
                                    categories.Add(AuditCategory.UDC);
                            columnIdx++;

                            if (ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Debug)
                            {
                                StringBuilder msg = new StringBuilder();
                                msg.AppendFormat("Audited categories for {0}: ", instance);
                                foreach (AuditCategory category in categories)
                                    msg.AppendFormat("{0}, ", category);
                                ErrorLog.Instance.Write(msg.ToString());
                            }

                            if (!reader.IsDBNull(columnIdx))
                            {
                                remoteConfig.AccessCheck = (int)reader.GetSqlByte(columnIdx);
                            }
                            columnIdx++;

                            /*
                            if( !reader.IsDBNull(columnIdx) )
                               if( reader.GetSqlByte(columnIdx) == 1 )
                                  remoteConfig.CaptureDetails = true;
                                  */
                            columnIdx++;

                            if (!reader.IsDBNull(columnIdx))
                                if ((int)reader.GetSqlByte(columnIdx) == 1)
                                    remoteConfig.Exceptions = true;
                            columnIdx++;

                            if (!reader.IsDBNull(columnIdx))
                                remoteConfig.Version = reader.GetInt32(columnIdx);
                            columnIdx++;

                            if (!reader.IsDBNull(columnIdx))
                            {
                                DateTime lastModifiedTime = reader.GetDateTime(columnIdx);
                                if (lastModifiedTime > remoteConfig.LastModifiedTime)
                                    remoteConfig.LastModifiedTime = lastModifiedTime;
                            }
                            columnIdx++;

                            if (!reader.IsDBNull(columnIdx))
                                userListXML = (string)reader.GetSqlString(columnIdx);
                            columnIdx++;

                            if (userListXML != null && !reader.IsDBNull(columnIdx))
                                if ((int)reader.GetSqlByte(columnIdx) == 1)
                                    auditAllUserActivities = true;
                            columnIdx++;

                            if (!reader.IsDBNull(columnIdx))
                                if ((int)reader.GetSqlByte(columnIdx) == 1)
                                    remoteConfig.IsEnabled = true;
                            columnIdx++;

                            if (!reader.IsDBNull(columnIdx))
                                if ((int)reader.GetSqlByte(columnIdx) == 1)
                                    remoteConfig.IsSQLsecure = true;
                            columnIdx++;

                            if (!reader.IsDBNull(columnIdx))       // 
                                sqlVersion = (reader.GetInt32(columnIdx) % 1000);
                            columnIdx++;

                            if (!reader.IsDBNull(columnIdx))
                                agentVersion = (string)reader.GetSqlString(columnIdx);

                            columnIdx++;

                            if (!reader.IsDBNull(columnIdx) &&
                                (int)reader.GetSqlByte(columnIdx) == 1)
                            {
                                remoteConfig.CaptureDDL = true;
                            }

                            columnIdx++;
                            //5.4_4.1.1_Extended Events
                            if (!reader.IsDBNull(columnIdx) &&
                               (int)reader.GetSqlByte(columnIdx) == 1)
                            {
                                remoteConfig.CaptureDetailsXE = true;
                            }

                            columnIdx++;
                            //5.4_4.1.1_Extended Events
                            if (!reader.IsDBNull(columnIdx) &&
                               (int)reader.GetSqlByte(columnIdx) == 1)
                            {
                                remoteConfig.AuditCaptureSQLXE = true;
                            }

                            columnIdx++;
                            //5.5 Audit Logs
                            if (!reader.IsDBNull(columnIdx) &&
                               (int)reader.GetSqlByte(columnIdx) == 1)
                            {
                                remoteConfig.IsAuditLogEnabled = true;
                            }

                            // SQLCM-5375-6.1.4.3-Capture Logout Events
                            columnIdx++;
                            if (!reader.IsDBNull(columnIdx) && (int)reader.GetSqlByte(columnIdx) == 1)
                            {
                                categories.Add(AuditCategory.Logouts);
                            }

                            columnIdx++;
                            //v5.6 SQLCM-5373
                            if (!reader.IsDBNull(columnIdx))
                                trustedUserListXML = (string)reader.GetSqlString(columnIdx);

                            // Add Categories in the end to include Logouts
                            remoteConfig.Categories = categories.ToArray();

                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                         CoreConstants.Exception_ErrorLoadingConfigurationForInstance + instance,
                                         e,
                                         true);
            }
            remoteConfig.DBConfigs = GetDBAuditConfigurationsFromDatabase(instance, conn, remoteConfig, sqlVersion, excludeSystemEvents);

            //v5.6 SQLCM-5373
            if (trustedUserListXML != null && trustedUserListXML.Length > 0)
            {
                try
                {
                    UserList userList = new UserList(trustedUserListXML);
                    remoteConfig.ServerTrustedUsersServerRoles = userList.ServerRoleIdList;
                    remoteConfig.ServerTrustedUsers = userList.UserNames;
                    remoteConfig.ServerTrustedUsersData = trustedUserListXML;

                }
                catch (Exception exp)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Default,"An error occurred loading trusted user audit settings",
                                            exp,true);
                }
            }

         if (userListXML != null && userListXML.Length > 0)
         {
            try
            {
               UserList userList = new UserList(userListXML);
               remoteConfig.ServerRoles = userList.ServerRoleIdList;
               remoteConfig.Users = userList.UserNames;
               if ((remoteConfig.ServerRoles != null &&
                     remoteConfig.ServerRoles.Length > 0) ||
                     (remoteConfig.Users != null &&
                     remoteConfig.Users.Length > 0))
               {
                  remoteConfig.UserConfig = new UserRemoteAuditConfiguration();
                  remoteConfig.UserConfig = GetUserAuditConfigurtionFromDatabase(instance,
                     conn,
                     auditAllUserActivities,
                     excludeSystemEvents);
               }
            }
            catch (Exception exp)
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Default,
                  // TODO: move string to CoreConstants
                                        "An error occurred loading privileged user audit settings",
                                        exp,
                                        true);
            }
         }

         // get SQLsecure databases to be excluded
         if (remoteConfig.IsSQLsecure)
         {
            remoteConfig.sqlsecureDBIds = GetSQLsecureDBIds(instance, conn);
         }

         // assign serialization version number so it can be deserialized by
         // different versions of agents.
         remoteConfig.StructVersion = GetAgentClassVersion(agentVersion);

         return remoteConfig;
      }

      //------------------------------------------------------------------------------
      // At first glance, this method and the ones it calls seem like a poor design,
      // but I cannot imagine a scenario where more than two or three database ids will
      // be changing.  When dealing with small sets of data, it doesn't really matter.
      //
      public bool UpdateDBIds(Hashtable auditedServerDbIds, string instance)
      {
         string commandText;
         int column = 0;
         int sqlDbId = -1;
         string dbName = "";
         int newSqlDbId;
         bool updated = false;
         Dictionary<string, int> dbs = new Dictionary<string,int>();

         commandText = String.Format("SELECT name, sqlDatabaseId from {0}..{1} where srvInstance = '{2}'",  CoreConstants.RepositoryDatabase,
                                                                                                            CoreConstants.RepositoryDatabaseTable,
                                                                                                            instance);
         try
         {
            using (SqlConnection conn = GetConnection())
            {
               using (SqlCommand cmd = new SqlCommand(commandText, conn))
               {
                  cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

                  using (SqlDataReader reader = cmd.ExecuteReader())
                  {
                     if (reader.Read())
                     {
                        column = 0;

                        if (reader.IsDBNull(column) == false)
                           dbName = reader.GetString(column++);

                        if (reader.IsDBNull(column) == false)
                           sqlDbId = (int)reader.GetInt16(column++);

                        dbs.Add(dbName, sqlDbId);
                     }
                  }
               }

               foreach (KeyValuePair<string, int> kvp in dbs)
               {
                  dbName = (string)kvp.Key;

                  if (auditedServerDbIds.ContainsKey(dbName))
                  {
                     newSqlDbId = (int)auditedServerDbIds[dbName];
                     sqlDbId = (int)kvp.Value;

                     if (newSqlDbId != sqlDbId)
                     {
                        UpdateDBId(newSqlDbId, dbName, instance, conn);
                        updated = true;
                     }
                  }
                  else
                  {
                     //we are auditing a database that is no longer on the audited instance.
                     //set the database ID in the repository to -2. there will not be any databases with that ID (as of SQL Server 2012)
                     UpdateDBId(-2, dbName, instance, conn);
                  }
               }
            }
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write(String.Format("Unable to update the database ID Error: {0}", e.ToString()), ErrorLog.Severity.Error);
         }
         return updated;
      }

      private void UpdateDBId(int newSqlId, string dbName, string instance, SqlConnection conn)
      {
         string commandText;

         commandText = String.Format("UPDATE {0}..{1} SET sqlDatabaseId={2} where name='{3}' AND srvInstance = '{4}'",  CoreConstants.RepositoryDatabase,
                                                                                                                        CoreConstants.RepositoryDatabaseTable,
                                                                                                                        newSqlId,
                                                                                                                        dbName,
                                                                                                                        instance);
         using (SqlCommand cmd = new SqlCommand(commandText, conn))
         {
            cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
            cmd.ExecuteNonQuery();
         }
      }

      #endregion

      #region Private Methods

      //-------------------------------------------------------------------------------------------------
      // GetConnection
      //-------------------------------------------------------------------------------------------------
      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      static private SqlConnection 
         GetConnection()
      {
         SqlConnection conn;
			
         try
         {
            conn = new SqlConnection( String.Format( "server={0};database={1};integrated security=SSPI;Connect Timeout=30;Application Name='{2}';",
               CollectionServer.ServerInstance,
               CoreConstants.RepositoryDatabase,
               CoreConstants.DefaultSqlApplicationName ) );
            conn.Open();
   	      
         }
         catch (Exception e)
         {
            // Database is not available
            ErrorLog.Instance.Write( String.Format( CoreConstants.Exception_RepositoryNotAvailable,
               e.Message ),
               ErrorLog.Severity.Error);
            throw;
         }

	      
         return conn;
      }

      //-------------------------------------------------------------------------------------------------
      // GetConfigVersion
      //-------------------------------------------------------------------------------------------------
      /// <summary>
      /// 
      /// </summary>
      /// <param name="instance"></param>
      /// <param name="conn"></param>
      /// <returns></returns>
      private int
         GetConfigVersion(
         string           instance,
         SqlConnection    conn
         )
      {
         int configVersion    = 0;
         
         try
         {
            string cmdStr = String.Format( "SELECT configVersion FROM {0} where instance={1}",
               CoreConstants.RepositoryServerTable,
               SQLHelpers.CreateSafeString(instance) );
            using ( SqlCommand cmd = new SqlCommand( cmdStr, conn ) )
            {
				   object obj = cmd.ExecuteScalar();
				   if( obj is DBNull )
					   configVersion = 0;
				   else
					   configVersion = (int)obj;
               
            }
         }
         catch (Exception ex )
         {
            ErrorLog.Instance.Write( String.Format( CoreConstants.Exception_Format_CouldntReadServerRecord,
               instance,
               ex ),
               ErrorLog.Severity.Error);
         }

         return configVersion;
      }

      //-------------------------------------------------------------------------------------------------
      // GetGlobalSetting
      //-------------------------------------------------------------------------------------------------
      /*
      private RemoteAuditConfiguration
         GetGlobalSetting(
         SqlConnection conn
         )
      {
         RemoteAuditConfiguration remoteConfig = new RemoteAuditConfiguration();
         int columnIdx = 0;
         ArrayList categories     = new ArrayList();

         try
         {
            string commandText = String.Format( "SELECT "
               + "auditLogins, " //0
               + "auditFailedLogins,"
               + "auditDDL,"
               + "auditSecurity,"
               + "auditAdmin,"
               + "auditDML," // 5
               + "auditSELECT, "
               + "auditFailures,  "
               + "timeLastModified "
               + " FROM {0} ",
               CoreConstants.RepositoryConfigurationTable );

            using ( SqlCommand command = new SqlCommand( commandText, conn ) )
            {
               using ( SqlDataReader reader = command.ExecuteReader() )
               {
                  if ( reader.Read() )
                  {
                     if( !reader.IsDBNull(columnIdx) )               
                        if( (int)reader.GetSqlByte(columnIdx) == 1 )
                           categories.Add( AuditCategory.Logins );
                     columnIdx++;

                     if( !reader.IsDBNull(columnIdx) )
                        if( (int)reader.GetSqlByte(columnIdx) == 1 )
                           categories.Add( AuditCategory.FailedLogins );
                     columnIdx++;

                     if( !reader.IsDBNull(columnIdx) )
                        if( (int)reader.GetSqlByte(columnIdx) == 1 )
                           categories.Add( AuditCategory.DDL );
                     columnIdx++;

                     if( !reader.IsDBNull(columnIdx) )
                        if( (int)reader.GetSqlByte(3) == 1 )
                           categories.Add( AuditCategory.Security );
                     columnIdx++;

                     if( !reader.IsDBNull(columnIdx) )
                        if( (int)reader.GetSqlByte(columnIdx) == 1 )
                           categories.Add( AuditCategory.Admin );              
                     columnIdx++;

                     if( !reader.IsDBNull(columnIdx) )                   
                        if( (int)reader.GetSqlByte(columnIdx) == 1 )
                           categories.Add( AuditCategory.DML );
                     columnIdx++;

                     if( !reader.IsDBNull(columnIdx) )
                        if( (int)reader.GetSqlByte(columnIdx) == 1 )
                           categories.Add( AuditCategory.SELECT );
                     columnIdx++;

                      
                     remoteConfig.Categories = (AuditCategory [])categories.ToArray( typeof(AuditCategory));


                     if( !reader.IsDBNull(columnIdx) )
                     {
                        remoteConfig.AccessCheck = (int)reader.GetSqlByte(columnIdx);
                     }
                     columnIdx++;

                     if( !reader.IsDBNull(columnIdx) ) 
                        remoteConfig.LastModifiedTime = reader.GetDateTime(columnIdx);
                     columnIdx++;

                     remoteConfig.StructVersion = CoreConstants.SerializationVersion;

                  }
               }
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Default,
                                     "An error occurred reading gloable audit settings from repository configuration table",
                                     e,
                                     true );
         }
         return remoteConfig;
      }
      */


      static private int GetAgentClassVersion( string agentVersion )
      {
         string [] parts = agentVersion.Split( ".".ToCharArray());

         int major = Convert.ToInt32( parts[0]);
         int minor = Convert.ToInt32(parts[1]);
         if( major <= 1 )
            return 0;  // Version 1.* agent
         else if( major >= 2 ) // serialization version is implemented since 2.0
         {
            switch( major )
            {
               case 2:  // Version 2.* agents
                  switch (minor)
                  {
                     case 0:
                        return CoreConstants.SerializationVersion_20;
                     case 1:
                        return CoreConstants.SerializationVersion_21;
                     default:
                        // Need to add new agent version
                        throw new ApplicationException("Unknown new 2.* serialiazation version error.");
                  }
                  
               case 3:  // V 3.* agents
                  switch( minor )
                  {
                     case 0:
                        return CoreConstants.SerializationVersion_30;
                     case 1:
                        return CoreConstants.SerializationVersion_31;
                     case 2:
                        return CoreConstants.SerializationVersion_32;
                     case 3:
                        return CoreConstants.SerializationVersion_33;
                     case 5:
                        return CoreConstants.SerializationVersion_35;
                     case 6:
                        return CoreConstants.SerializationVersion_36;
                     case 7:
                        return CoreConstants.SerializationVersion_37;
                     default:
                        // Need to add new agent version
                        throw new ApplicationException("Unknown new 3.* serialization version error.");
                  }
               case 4: // V 4.* agents
                  switch(minor)
                  {
                     case 0:
                        return CoreConstants.SerializationVersion_40;
                     case 2:
                        return CoreConstants.SerializationVersion_42;
                     case 3:
                        return CoreConstants.SerializationVersion_43;
                      case 4:
                          return CoreConstants.SerializationVersion_44;
                      case 5:
                          return CoreConstants.SerializationVersion_45;
                      default:
                        throw new ApplicationException("Unknown new 4.* serialization version error.");
                  }
                case 5: // v5.* agents
                  switch (minor)
                  {
                      case 0:
                          return CoreConstants.SerializationVersion_50;
                      case 1:
                          return CoreConstants.SerializationVersion_51;
                      case 3:
                          return CoreConstants.SerializationVersion_53;
					  case 4:
                      	  return CoreConstants.SerializationVersion_54;
                      case 5:
                          return CoreConstants.SerializationVersion_55;// Indicate Serialization Version
                      case 6:
                          return CoreConstants.SerializationVersion_56;
                      case 7:
                          return CoreConstants.SerializationVersion_57;
                      case 8:
                          return CoreConstants.SerializationVersion_58;
                      case 9:
                          return CoreConstants.SerializationVersion_59; // Serialization Version change for each release
                            default:
                          throw new ApplicationException("Unknown new 5.* serialization version error.");
                  }

               default:
                  // Need to add new agent version
                  throw new ApplicationException("Unknown new serialization version error.");
                  
            }
         }
         else
            return 0;  // unknown
      }


        static private byte GetByte(SqlDataReader reader, string columnName, byte defaultValueIfNull)
        {
            int columnId = reader.GetOrdinal(columnName);
            var sqlByte = reader.GetSqlByte(columnId);
            return sqlByte.IsNull ? defaultValueIfNull : sqlByte.Value;
        }

      //-------------------------------------------------------------------------------------------------
      // GetDBAuditConfigurationsFromDatabase
      //-------------------------------------------------------------------------------------------------
      /// <summary>
      /// 
      /// </summary>
      /// <param name="instance"></param>
      /// <param name="conn"></param>
      static private DBRemoteAuditConfiguration []
         GetDBAuditConfigurationsFromDatabase( 
            string                   instance, 
            SqlConnection            conn,
            RemoteAuditConfiguration serverConfig,
            int                      sqlVersion,
            bool                     excludeSystemEvents )
      {
         DBRemoteAuditConfiguration [] configList = null;

         try
         {
            string commandText = String.Format( "SELECT {0}.isEnabled, "  //0
               + "{0}.sqlDatabaseId, "
               + "{0}.auditDDL, " 
               + "{0}.auditSecurity, " 
               + "{0}.auditAdmin, "
               + "{0}.auditDML, "  // 5 
               + "{0}.auditSELECT, " 
               + "{0}.auditBroker, "
               + "{0}.auditLogins, "
               + "{0}.auditFailures, " 
               + "{0}.auditCaptureSQL, "  //10
               + "{0}.auditExceptions, " 
               + "{0}.auditDmlAll, " 
               + "{0}.auditUserTables, " 
               + "{0}.auditSystemTables, " 
               + "{0}.auditStoredProcedures, "  //15
               + "{0}.auditDmlOther, "
               + "{0}.auditUsersList, "
               + "{0}.auditDataChanges, "  // 18 : new since 3.1 
               + "{0}.auditSensitiveColumns, " // 19 : new since 3.5
               + "{0}.auditCaptureTrans, " // 20 : new since 3.5
               + "{0}.auditPrivUsersList, "
               + "{0}.auditUserAll, "
               + "{0}.auditUserLogins, "
               + "{0}.auditUserFailedLogins, "
               + "{0}.auditUserDDL, "
               + "{0}.auditUserSecurity, "
               + "{0}.auditUserAdmin, "
               + "{0}.auditUserDML, "
               + "{0}.auditUserSELECT, "
               + "{0}.auditUserFailures, " // 30
               + "{0}.auditUserCaptureSQL, "
               + "{0}.auditUserCaptureTrans, "
               + "{0}.auditUserCaptureDDL, "
               + "{0}.auditUserExceptions, "
               + "{0}.auditUserUDE, "
               + "{0}.auditCaptureDDL, "
               + "{0}.srvId, "
               + "{0}.dbId, " // dbId and name must be the last two columns of this result set
               + "{0}.name, "
               + "{0}.auditLogouts, "  // SQLCM-5375-6.1.4.3-Capture Logout Events
               + "{0}.auditUserLogouts, " // SQLCM-5375-6.1.4.3-Capture Logout Events
               + "{0}.auditSensitiveColumnActivity,  " // column 42 // SQLCM-5471 v5.6 Add Activity to Senstitive columns
               + "{0}.auditSensitiveColumnActivityDataset  " // column 43 // SQLCM-5471 v5.6 Add Activity to Senstitive columns
               + " FROM {0}, {1} "
               + " WHERE {0}.srvId = {1}.srvId "
               + " AND {1}.instance = {2} and {0}.isEnabled = 1 ",
               CoreConstants.RepositoryDatabaseTable,
               CoreConstants.RepositoryServerTable,
               SQLHelpers.CreateSafeString(instance) );

            using ( SqlCommand command = new SqlCommand( commandText, conn ) )
            {
               using ( SqlDataReader reader = command.ExecuteReader() )
               {
                  configList = GetDBAuditConfigurationFromReader( conn, reader, serverConfig, sqlVersion, excludeSystemEvents  );
               }
            }
         }
         catch( Exception e )
         {
            string msg = String.Format( CoreConstants.Exception_Format_ErrorLoadingDBAuditConfigurationsFromDatabase,
                                                    instance );
            ErrorLog.Instance.Write( ErrorLog.Level.Default,
                                     msg,
                                     e,
                                     true );
            InternalAlert.Raise( msg );
         }

         return configList;

      }

      //-------------------------------------------------------------------------------------------------
      // GetDBAuditConfigurationFromReader
      //-------------------------------------------------------------------------------------------------
      /// <summary>
      /// 
      /// </summary>
      /// <param name="reader"></param>
      /// <returns></returns>
      static private DBRemoteAuditConfiguration []
         GetDBAuditConfigurationFromReader( 
            SqlConnection             conn,
            SqlDataReader             reader,
            RemoteAuditConfiguration  serverConfig,
            int                       sqlVersion,
            bool                      excludeSystemEvents )
      {
         if( reader == null )
         {
            return null;
         }

         List<DBRemoteAuditConfiguration> dbList = new List<DBRemoteAuditConfiguration>();

         while ( reader.Read() )
         {

            
         List<AuditCategory> categories = new List<AuditCategory>();
         List<DBObjectType> objectTypes = new List<DBObjectType>();
         int       columnIdx;
         bool      auditAllObjectTypes;
         int       dbId;

         DBRemoteAuditConfiguration config;
         List<AuditCategory> userCategories = new List<AuditCategory>();
         string pUsers = null;
         bool auditAllUserActivities = false;

            columnIdx = 0;

            // Check if audit is enable for this database
            if( reader.IsDBNull(columnIdx) )
               continue;

            if((int)reader.GetSqlByte(columnIdx) == 0 )
               continue;

            columnIdx++;
            config = new DBRemoteAuditConfiguration();

            // Column 1
            // Database ID
            config.DbId = reader.GetInt16(columnIdx);
            columnIdx++;

            // Update Field Count for adding two new items in the end
            if( !reader.IsDBNull(reader.FieldCount - 5) )
               config.dbName = (string)reader.GetSqlString(reader.FieldCount - 5);

            // DDL

            // Column 2
            if( !reader.IsDBNull(columnIdx) )
               if( (int)reader.GetSqlByte(columnIdx) == 1 )
                  categories.Add( AuditCategory.DDL );
            columnIdx++;

            
            // Column 3
            if( !reader.IsDBNull(columnIdx) )
               if( (int)reader.GetSqlByte(columnIdx) == 1 )
                  categories.Add( AuditCategory.Security );
            columnIdx++;


            // Column 4
            if( !reader.IsDBNull(columnIdx) )
                if ((int)reader.GetSqlByte(columnIdx) == 1)
                  categories.Add( AuditCategory.Admin );  
            columnIdx++;


            // Column 5
            if( !reader.IsDBNull(columnIdx) )
            {
               if( (int)reader.GetSqlByte(columnIdx) == 1 )
               {
                  categories.Add( AuditCategory.DML );
               }
            }
            columnIdx++;


            // Column 6
            if( !reader.IsDBNull(columnIdx) )
            {
               if( (int)reader.GetSqlByte(columnIdx) == 1 )
               {
                  categories.Add( AuditCategory.SELECT );
               }
            }
            columnIdx++;

            // 7
            /* Removed from 2.0 features
            if( !reader.IsDBNull(columnIdx) )
            {
               if( (int)reader.GetSqlByte(columnIdx) == 1 )
               {
                  categories.Add( AuditCategory.Broker );
               }
            }
            */
            columnIdx++;


            
            if( !reader.IsDBNull(columnIdx) )
            {
               if( (int)reader.GetSqlByte(columnIdx) == 1 )
               {
                  categories.Add( AuditCategory.Logins );
               }
            }
            columnIdx++;




            if( categories.Count == 0 )
               continue;

            /*
            if( !excludeSystemEvents )
               categories.Add( AuditCategory.SystemEvents );
               */

         

            if( !reader.IsDBNull(columnIdx) )
            {
               config.AccessCheck = (int)reader.GetSqlByte(columnIdx);
            }
            columnIdx++;
            
            if( !reader.IsDBNull(columnIdx) )
               if( (int)reader.GetSqlByte(columnIdx) == 1 )
                  config.CaptureDetails = true;
            columnIdx++;

            // Override CaptureSql based on registry settings
            if (!CoreConstants.AllowCaptureSql)
               config.CaptureDetails = false;

            if( !reader.IsDBNull(columnIdx) )
               if( (int)reader.GetSqlByte(columnIdx) == 1 )
                  config.Exceptions = true;
            columnIdx++;

            objectTypes.Clear();
            auditAllObjectTypes = false;
            if( !reader.IsDBNull(columnIdx) )
               if( (int)reader.GetSqlByte(columnIdx) == 1 )
                  auditAllObjectTypes = true;
            columnIdx++;

            // Update Field Count for adding two new items in the end
            dbId = reader.GetInt32( reader.FieldCount - 6 );
            if( !auditAllObjectTypes )
            {
               if( !reader.IsDBNull(columnIdx) )
               {
                  switch( (int)reader.GetSqlByte(columnIdx))
                  {
                     case 1:  // All user tables
                        objectTypes.Add( DBObject.GetUserTableTypeValue( sqlVersion ) );
                        break;
                     case 2:  // Selected user tables
                        objectTypes.Add( DBObject.GetUserTableTypeValue( sqlVersion ) );
                        config.AuditObjects = GetAuditedUserTables( dbId, conn.ConnectionString ) ; // SQLcompliance DB Id
                        break;
                     default:
                        break;
                  }
               }
               columnIdx++;


               if( !reader.IsDBNull(columnIdx) )
               {
                  if( (int)reader.GetSqlByte(columnIdx) == 1 )
                  {
                     objectTypes.Add( DBObject.GetSystemTableTypeValue( sqlVersion ) );
                  }
               }
               columnIdx++;

               if( !reader.IsDBNull(columnIdx) )
               {
                  if( (int)reader.GetSqlByte(columnIdx) == 1 )
                  {
                     DBObjectType [] spTypes = DBObject.GetSPTypeValues( sqlVersion );
                     objectTypes.AddRange( spTypes );
                  }
               }
               columnIdx++;

               if( !reader.IsDBNull(columnIdx) )
               {
                  if( (int)reader.GetSqlByte(columnIdx) == 1 )
                  {
                     DBObjectType [] dmlOtherTypes = DBObject.GetDmlOtherTypeValues( sqlVersion );
                     objectTypes.AddRange(dmlOtherTypes);
                  }
               }
               columnIdx++;
            }
            else
            {
               columnIdx += 4; // skip object types columns
            }


            if( objectTypes.Count > 0  && !auditAllObjectTypes )
               config.ObjectTypes = objectTypes.ToArray();
            else
               config.ObjectTypes = null;

            if( !reader.IsDBNull( columnIdx ) )
            {
               string users = reader.GetString( columnIdx );
               UserList list = new UserList( users );
               config.ServerRoles = list.ServerRoleIdList;
               config.Users = list.UserNames;
            }
            
            // column 18 DataChanges flag
            columnIdx++;
            if( !reader.IsDBNull( columnIdx ) )
            {
                //start Sqlcm 5.6-5564 (changed the number in GetInt32 function from 3 to 5 to accomodate two newly added columns)
               if ((int)reader.GetSqlByte(columnIdx) == 1)
                  config.DataChangeTables =
                     GetDataChangeTables( reader.GetInt32( reader.FieldCount - 7 ), // Server ID
                                          dbId,
                                          conn.ConnectionString,
                                          config.AuditObjects );
                //end Sqlcm 5.6-5564
            }
            // column 19 Sensitive Columns flag
            columnIdx++;
            if (!reader.IsDBNull(columnIdx))
            {
               if ((int)reader.GetSqlByte(columnIdx) == 1)
                  // Update Field Count for adding two new items in the end
                  config.SensitiveColumns = GetSensitiveColumnTables(reader.GetInt32(reader.FieldCount - 7), //Server ID
                                                                     dbId,
                                                                     conn.ConnectionString,
                                                                     config.AuditObjects);
            }

            //column 20 Capture Transactions flag
            columnIdx++;
            if (!reader.IsDBNull(columnIdx))
               if ((int)reader.GetSqlByte(columnIdx) == 1)
                  config.CaptureTransactions = true;

            //column 21 Privileged User list
              columnIdx++;
              if (!reader.IsDBNull(columnIdx))
              {
                  pUsers = reader.GetString(columnIdx);
                  UserList list = new UserList(pUsers);
                  config.PrivServerRoles = list.ServerRoleIdList;
                  config.PrivUsers = list.UserNames;
              }

              columnIdx++;
              if (pUsers != null && !reader.IsDBNull(columnIdx))
                  if ((int) reader.GetSqlByte(columnIdx) == 1)
                      auditAllUserActivities = true;
                     
              // Column 23
              columnIdx++;
              if (!reader.IsDBNull(columnIdx ))
              {
                  if ((int)reader.GetSqlByte(columnIdx) == 1)
                  {
                      userCategories.Add(AuditCategory.Logins);
                  }
              }
              //column24
              columnIdx++;
              if (!reader.IsDBNull(columnIdx))
              {
                  if ((int)reader.GetSqlByte(columnIdx) == 1)
                  {
                      userCategories.Add(AuditCategory.FailedLogins);
                  }
              }
              //column25
              columnIdx++;

              if (!reader.IsDBNull(columnIdx))
                  if ((int)reader.GetSqlByte(columnIdx) == 1)
                      userCategories.Add(AuditCategory.DDL);

              //column26
              columnIdx++;

              if (!reader.IsDBNull(columnIdx))
                  if ((int)reader.GetSqlByte(columnIdx) == 1)
                      userCategories.Add(AuditCategory.Security);
              //column27
              columnIdx++;

              if (!reader.IsDBNull(columnIdx))
                  if ((int)reader.GetSqlByte(columnIdx) == 1)
                      userCategories.Add(AuditCategory.Admin);
              columnIdx++;

              if (!reader.IsDBNull(columnIdx))
              {
                  if ((int)reader.GetSqlByte(columnIdx) == 1)
                  {
                      userCategories.Add(AuditCategory.DML);
                  }
              }
              columnIdx++;

              if (!reader.IsDBNull(columnIdx))
              {
                  if ((int)reader.GetSqlByte(columnIdx) == 1)
                  {
                      userCategories.Add(AuditCategory.SELECT);
                  }
              }
             
              if (userCategories.Count > 0)
              {
                  // next time move this into other method and write and use methods like GetByte(SqlDataReader reader, string columnName, byte defaultValueIfNull)
                  //old snow leopard
              columnIdx++;
              if (!reader.IsDBNull(columnIdx))
              {
                  config.UserAccessCheck = (int)reader.GetSqlByte(columnIdx);
              }
              columnIdx++;

              if (!reader.IsDBNull(columnIdx))
                  if ((int)reader.GetSqlByte(columnIdx) == 1)
                      config.UserCaptureSql = true;
              columnIdx++;

              // Override CaptureSql based on registry settings
              if (!CoreConstants.AllowCaptureSql)
                  config.UserCaptureSql = false;

              if (!reader.IsDBNull(columnIdx))
                  if ((int)reader.GetSqlByte(columnIdx) == 1)
                      config.UserCaptureTran = true;
              columnIdx++;

              if (!reader.IsDBNull(columnIdx))
                  if ((int)reader.GetSqlByte(columnIdx) == 1)
                      config.UserCaptureDDL = true;
              columnIdx++;

              if (!reader.IsDBNull(columnIdx))
                  if ((int)reader.GetSqlByte(columnIdx) == 1)
                      config.UserExceptions = true;

              columnIdx++;

              if (!reader.IsDBNull(columnIdx))
              {
                  if ((int)reader.GetSqlByte(columnIdx) == 1)
                  {
                      userCategories.Add(AuditCategory.UDC);
                  }
              }
              }


              if (GetByte(reader, DatabaseTableColumns.auditCaptureDDL, 0) == 1)
              {
                  config.CaptureDDL = true;
              }

              columnIdx++;
              
              // SQLCM-5375-6.1.4.3-Capture Logout Events
              if (GetByte(reader, DatabaseTableColumns.auditLogouts, 0) == 1)
              {
                  categories.Add(AuditCategory.Logouts);
              }             

              // SQLCM-5375-6.1.4.3-Capture Logout Events
              if (GetByte(reader, DatabaseTableColumns.auditUserLogouts, 0) == 1)
              {
                  userCategories.Add(AuditCategory.Logouts);
              }             

              // Adding User Categories and Categories in the end
              config.UserCategories = userCategories.ToArray();
              config.Categories = categories.ToArray();

              if (auditAllUserActivities)
              {
                  UserRemoteAuditConfiguration userConfig = AddAllAuditCategories(excludeSystemEvents);
                  config.UserCategories = userConfig.Categories;
                  config.UserCaptureSql = userConfig.CaptureDetails;
                  config.UserCaptureTran = userConfig.CaptureTransactions;
                  config.UserCaptureDDL = userConfig.CaptureDDL;
                  config.UserExceptions = userConfig.Exceptions;
                  config.UserAccessCheck = userConfig.AccessCheck;
              }
              // SQLCM-5471 v5.6 Add Activity to Senstitive columns
              if (GetByte(reader, "auditSensitiveColumnActivity", 0) >= 0)
              {
                  config.AuditSensitiveColumnActivity = (int)GetByte(reader, "auditSensitiveColumnActivity", 0);
              }
              if (GetByte(reader, "auditSensitiveColumnActivityDataset", 0) >= 0)
              {
                  config.AuditSensitiveColumnActivityDataset = (int)GetByte(reader, "auditSensitiveColumnActivityDataset", 0);
              } 

              dbList.Add( config );

         }

         return dbList.ToArray();
      }

      //-------------------------------------------------------------------------------------------------
      // GetUserAuditConfigurtionFromDatabase
      //-------------------------------------------------------------------------------------------------
      /// <summary>
      /// 
      /// </summary>
      /// <param name="instance"></param>
      /// <param name="conn"></param>
      /// <returns></returns>
      static private UserRemoteAuditConfiguration 
         GetUserAuditConfigurtionFromDatabase (
            string instance, 
            SqlConnection conn,
            bool          allActivities,
            bool          excludeSystemEvents )
      {
         if( allActivities )
         {
            return AddAllAuditCategories( excludeSystemEvents );
         }
         else
         {
            return GetUserAuditConfigurationsFromDatabase( conn, instance, excludeSystemEvents );
         }

      }

      //-------------------------------------------------------------------------------------------------
      // GetSQLsecureDBNames
      //-------------------------------------------------------------------------------------------------
      /*
      /// <summary>
      /// 
      /// </summary>
      /// <param name="instance"></param>
      /// <param name="conn"></param>
      /// <returns></returns>
      private string []
         GetSQLsecureDBNames (
         string instance,
         SqlConnection conn )
      {
         ArrayList                  sqlsecureDBs   = null;

         try
         {

            string commandText = String.Format( "SELECT databaseName FROM {0} "
                                                + "WHERE instance = {1}",
                                                CoreConstants.RepositorySystemDatabaseTable,
                                                SQLHelpers.CreateSafeString(instance) );

            using ( SqlCommand command = new SqlCommand( commandText, conn ) )
            {
               using ( SqlDataReader reader = command.ExecuteReader() )
               {
                  if( reader != null )
                  {
                     sqlsecureDBs = new ArrayList();

                     while( reader.Read() )
                     {
                        if( !reader.IsDBNull(0) )
                           sqlsecureDBs.Add( reader.GetString(0) );
                     }
                  }
               }
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Default, 
                                     CoreConstants.Exception_CannotReadSystemDatabaseTable, 
                                     e,
                                     ErrorLog.Severity.Error );
         }

         return (string [] )sqlsecureDBs.ToArray( typeof(string) );

      }
      */
      
      
      //-------------------------------------------------------------------------------------------------
      // GetSQLsecureDBIds
      //-------------------------------------------------------------------------------------------------
      /// <summary>
      /// 
      /// </summary>
      /// <param name="instance"></param>
      /// <param name="conn"></param>
      /// <returns></returns>
      static private int []
         GetSQLsecureDBIds (
            string instance,
            SqlConnection conn )
      {
         List<int>                  sqlsecureDBs   = new List<int>();

         try
         {

            string commandText = String.Format( "SELECT sqlDatabaseId FROM {0} WHERE instance = {1}",
                                                CoreConstants.RepositorySystemDatabaseTable,
                                                SQLHelpers.CreateSafeString(instance) );

            using ( SqlCommand command = new SqlCommand( commandText, conn ) )
            {
               using (SqlDataReader reader = command.ExecuteReader() )
               {
                  if( reader != null )
                  {

                     while( reader.Read() )
                     {
                        if( !reader.IsDBNull(0) )
                           sqlsecureDBs.Add( reader.GetInt16(0) );
                     }
                  }
               }
            }
         }
         catch( Exception e )
         {
            string msg = String.Format( CoreConstants.Exception_ErrorGettingSQLcomplianceDBIds, instance );
            ErrorLog.Instance.Write( ErrorLog.Level.Default,
                                     msg,
                                     e,
                                     true);
            InternalAlert.Raise( msg );
         }

         return sqlsecureDBs.ToArray( );

      }
      

      //-------------------------------------------------------------------------------------------------
      // SetLastConfigUpdate
      //-------------------------------------------------------------------------------------------------
      /*
      /// <summary>
      /// 
      /// </summary>
      /// <param name="instance"></param>
      /// <param name="conn"></param>
      private void
         SetLastConfigUpdate(
            string           instance,
            SqlConnection    conn
         )
      {
         try
         {
            string cmdStr = String.Format( "UPDATE {0} SET lastConfigUpdate=GETUTCDATE() where instance={1}",
                                           CoreConstants.RepositoryServerTable,
                                           SQLHelpers.CreateSafeString(instance) );
            using ( SqlCommand cmd = new SqlCommand( cmdStr, conn ) )
            {
               cmd.ExecuteNonQuery();
            }
         }
         catch (Exception ex )
         {
            ErrorLog.Instance.Write( String.Format( CoreConstants.Exception_CouldntUpdateServerRecord, ex.Message ),
                                     instance,
                                     ErrorLog.Severity.Error);
         }
      }
      */

      //---------------------------------------------------------------------
      // GetUserIDArray
      //---------------------------------------------------------------------
      /*
      /// <summary>
      /// Create an integer user ID array from a comma separated string list
      /// </summary>
      /// <param name="userIDList"></param>
      /// <returns></returns>
      private int []
         GetUserIDArray (
         string userIDList )
      {
         int [] idList = null;
         try
         {
            if( userIDList == null ||
               userIDList.Length == 0 )
               return null;

            string [] ids = userIDList.Split( ",".ToCharArray() );
            idList = new int [ ids.Length ];
            int i = 0;

            foreach( string id in ids )
               idList[i++] = int.Parse( id );
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Default,
                                     "Error parsing user ID list.",
                                     e,
                                     true );
         }
         return idList;

      }
      */

      //---------------------------------------------------------------------
      // AddAllAuditCategories
      //---------------------------------------------------------------------
      /// <summary>
      /// 
      /// </summary>
      static private UserRemoteAuditConfiguration
         AddAllAuditCategories ( bool excludeSystemEvents )
      {
         UserRemoteAuditConfiguration config = new UserRemoteAuditConfiguration();
         ArrayList categories = new ArrayList();

         categories.Add( AuditCategory.Admin );
         categories.Add( AuditCategory.Audit );
         categories.Add( AuditCategory.DDL );
         categories.Add( AuditCategory.DML );
         categories.Add( AuditCategory.FailedLogins );
         categories.Add( AuditCategory.Logins);
         categories.Add( AuditCategory.Logouts);
         categories.Add( AuditCategory.Security );
         categories.Add( AuditCategory.SELECT );
         categories.Add( AuditCategory.UDC );
         /*
         if( !excludeSystemEvents )
            categories.Add( AuditCategory.SystemEvents );
            */

         config.Categories = (AuditCategory [] )categories.ToArray( typeof(AuditCategory) );

         config.AccessCheck = (int)AccessCheckFilter.NoFilter;
         config.CaptureDetails = CoreConstants.AllowCaptureSql;
         config.CaptureTransactions = true;
         config.CaptureDDL = true;
         config.Exceptions = true;
         config.CaptureDetailsXE = true;

         return config;
      }
            
      //---------------------------------------------------------------------
      // GetUserAuditConfigurationsFromDatabase
      //---------------------------------------------------------------------
      static private UserRemoteAuditConfiguration
         GetUserAuditConfigurationsFromDatabase (
            SqlConnection                conn,
            string                       instance,
            bool                         excludeSystemEvents
         )
      {
         SqlDataReader reader = null;
         ArrayList categories = new ArrayList();
         UserRemoteAuditConfiguration config = new UserRemoteAuditConfiguration();

         try
         {

            string commandText = String.Format( "SELECT " 
                                                + "auditUserLogins, "
                                                + "auditUserFailedLogins, "
                                                + "auditUserDDL, " 
                                                + "auditUserSecurity, " 
                                                + "auditUserAdmin, "
                                                + "auditUserDML, " 
                                                + "auditUserSELECT, " 
                                                + "auditUserUDE, "
                                                + "auditUserFailures, " 
                                                + "auditUserCaptureSQL,  " 
                                                + "auditUserCaptureTrans, "
                                                + "auditUserExceptions,  "
                                                + "auditUserCaptureDDL,  "
                                                + "auditUserExtendedEvents, "
                                                + "auditUserLogouts "  // SQLCM-5375-6.1.4.3-Capture Logout Events
                                                + "FROM {0} "
                                                + "WHERE instance = {1} ",
                                                CoreConstants.RepositoryServerTable,
                                                SQLHelpers.CreateSafeString(instance) );

            using ( SqlCommand command = new SqlCommand( commandText, conn ) )
            {
               reader = command.ExecuteReader();
            }
         }
         catch( Exception e )
         {
            string msg = String.Format( CoreConstants.Exception_Format_ErrorLoadingUserAuditConfigurationsFromDatabase,
                              instance );
            ErrorLog.Instance.Write( ErrorLog.Level.Default,
                                     msg,
                                     e,
                                     ErrorLog.Severity.Error );
            InternalAlert.Raise( msg );
            if( reader != null )
               reader.Close();
            return config;
         }

         try
         {
            if( reader.Read() )
            {
               if( !reader.IsDBNull(0) )
                  if( reader.GetByte(0) == 1 )
                     categories.Add( AuditCategory.Logins );

               if( !reader.IsDBNull(1) )
                  if( reader.GetByte(1) == 1 )
                     categories.Add( AuditCategory.FailedLogins );

               if( !reader.IsDBNull(2) )
                  if( reader.GetByte(2) == 1 )
                     categories.Add( AuditCategory.DDL );

               if( !reader.IsDBNull(3) )
                  if( reader.GetByte(3) == 1 )
                     categories.Add( AuditCategory.Security );

               if( !reader.IsDBNull(4) )
                  if( reader.GetByte(4) == 1 )
                     categories.Add( AuditCategory.Admin );

               if( !reader.IsDBNull(5) )
                  if( reader.GetByte(5) == 1 )
                     categories.Add( AuditCategory.DML );

               if( !reader.IsDBNull(6) )
                  if( reader.GetByte(6) == 1 )
                     categories.Add( AuditCategory.SELECT );

               if( !reader.IsDBNull(7) )
                  if( reader.GetByte(7) == 1 )
                     categories.Add( AuditCategory.UDC );

               /* Disabled for 2.0 
               if( !excludeSystemEvents )
                  categories.Add( AuditCategory.SystemEvents );
                  */

              
               if( !reader.IsDBNull(8) )
               {
                  config.AccessCheck = (int)reader.GetSqlByte(8);
               }

               if( !reader.IsDBNull(9) )
                  if( reader.GetByte(9) == 1 )
                     config.CaptureDetails = true;
               if (!CoreConstants.AllowCaptureSql)
                  config.CaptureDetails = false;

               if (!reader.IsDBNull(10))
                  if (reader.GetByte(10) == 1)
                     config.CaptureTransactions = true;

               if( !reader.IsDBNull(11) )
                  if( reader.GetByte(11) == 1 )
                     config.Exceptions = true;

               if (!reader.IsDBNull(12))
                   if (reader.GetByte(12) == 1)
                       config.CaptureDDL = true;

                //5.4_4.1.1_Extended Events
               if (!reader.IsDBNull(13))
                   if (reader.GetByte(13) == 1)
                       config.CaptureDetailsXE = true;

               // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
               if( !reader.IsDBNull(14) )
                  if( reader.GetByte(14) == 1 )
                     categories.Add( AuditCategory.Logouts );

            } 
             //SQLCM-5568 - V5.6 Logout event not captured when user server level privileged user settings
             config.Categories = (AuditCategory[])categories.ToArray(typeof(AuditCategory));

         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Default,
               String.Format( CoreConstants.Exception_Format_ErrorLoadingUserAuditConfigurationsFromDataReader,
               instance ),
               e,
               ErrorLog.Severity.Error );
         }

         if( reader != null )
            reader.Close();

         return config;

      }

      /// <summary>
      /// Get audited user tables for a specified database
      /// </summary>
      /// <param name="sqlsecureDbId">SQLsecure database ID.</param>
      /// <returns>int []</returns>
      static public int []
         GetAuditedUserTables (
            int sqlsecureDbId, string connString  )
      {
         List <int> objectList = new List<int>();

         using( SqlConnection conn = new SqlConnection( connString) )
         {
            try
            {
               conn.Open();

               // Note that we don't have valid object types in this table.  Treat everything as a table for now.
               string query = String.Format( "SELECT id from {0} where dbId = {1}",
                                             CoreConstants.RepositoryDatabaseObjectsTable,
                                             sqlsecureDbId );
               using ( SqlCommand command = new SqlCommand( query, conn ) )
               {
                  using ( SqlDataReader reader = command.ExecuteReader() )
                  {
                     if( reader == null )
                        return null;
               
                     while( reader.Read() )
                     {
                        if( !reader.IsDBNull(0) )
                        {
                           objectList.Add( reader.GetInt32(0) );
                        }
                     }
                  }
               }
            }
            catch( Exception e )
            {
               string msg = String.Format( CoreConstants.Exception_ErrorReadingAuditedUserTables,
                                           CoreConstants.RepositoryDatabaseObjectsTable,
                                           sqlsecureDbId );
               ErrorLog.Instance.Write( ErrorLog.Level.Default,
                                        msg,
                                        e,
                                        true );
               throw;
            }
            finally
            {
               conn.Close();
            }
         }
      
         return objectList.ToArray( );

      }

      static internal string[]
         GetAuditedTableNames(
            int sqlsecureDbId)
      {
         List<string> tables;

         using (SqlConnection conn = GetConnection())
         {
            try
            {

               // Note that we don't have valid object types in this table.  Treat everything as a table for now.
               string query = String.Format("SELECT name,schemaName from {0} where dbId = {1}",
                                             CoreConstants.RepositoryDatabaseObjectsTable,
                                             sqlsecureDbId);
               using (SqlCommand command = new SqlCommand(query, conn))
               {
                  using (SqlDataReader reader = command.ExecuteReader())
                  {
                     if (reader == null)
                        return null;

                     tables = new List<string>();

                     while (reader.Read())
                     {
                        if (!reader.IsDBNull(0))
                        {
                           tables.Add(String.Format("{0}.{1}", reader.GetString(1), reader.GetString(0)));
                        }
                     }
                  }
               }
            }
            catch (Exception e)
            {
               string msg = String.Format(CoreConstants.Exception_ErrorReadingAuditedUserTables,
                                           CoreConstants.RepositoryDatabaseObjectsTable,
                                           sqlsecureDbId);
               ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                        msg,
                                        e,
                                        true);
               throw;
            }
            finally
            {
               if (conn != null) conn.Close();
            }
         }

         return tables.ToArray();

      }

      public static TableConfiguration[]
         GetDataChangeTables( int serverId, // Server ID
                              int dbId,
                              string connStr,
                              int [] auditedObjects )
      {
         List<TableConfiguration> tables = new List<TableConfiguration>();
         using (SqlConnection conn = new SqlConnection(connStr))
         {
            conn.Open();
            TableConfiguration config;

            // Use DataChangeTablesRecrod to retrieve the list of monitored tables.
            List<DataChangeTableRecord> records = DataChangeTableRecord.GetAuditedTables(conn, 
                                                                                         serverId, 
                                                                                         dbId, 
                                                                                         auditedObjects);
            foreach (DataChangeTableRecord record in records)
            {
               config = new TableConfiguration();
               config.DbId = record.DbId;
               config.SrvId = record.SrvId;
               config.MaxRows = record.RowLimit;
               config.Schema = record.SchemaName;
               config.Name = record.TableName;
               config.Columns = record.Columns;

               tables.Add(config);
            }
         }
         
         return tables.ToArray();
      }

      public static TableConfiguration[] GetSensitiveColumnTables(int serverId, 
                                                              int dbId,
                                                              string connStr,  
                                                              int [] auditedObjects)
      {
         List<TableConfiguration> tables = new List<TableConfiguration>();

         using (SqlConnection conn = new SqlConnection(connStr))
         {
            conn.Open();
            TableConfiguration config;

            // Use DataChangeTablesRecrod to retrieve the list of monitored tables.
            List<SensitiveColumnTableRecord> records = SensitiveColumnTableRecord.GetAuditedTables(conn, 
                                                                                                   serverId, 
                                                                                                   dbId, 
                                                                                                   auditedObjects);
            if (records != null)
            {
            foreach (SensitiveColumnTableRecord record in records)
            {
               config = new TableConfiguration();
               config.DbId = record.DbId;
               config.SrvId = record.SrvId;
               config.Schema = record.SchemaName;
               config.Name = record.TableName;
               config.Columns = record.Columns;
               config.Type = record.Type;
               tables.Add(config);
            }
         }
         }
         return tables.ToArray();
      }
      #endregion

	}
}


