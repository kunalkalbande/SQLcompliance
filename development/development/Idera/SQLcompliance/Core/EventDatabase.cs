using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Idera.SQLcompliance.Core.Stats;

namespace Idera.SQLcompliance.Core
{

    //
    // NormalChainIntact - the database is not in archive or groom and the hash chain is being maintained.
    // Busy - the database IS in archive/groom
    // NormalChainBroken - the database bis not in archive/groom and the chain is no longer maintained
    // 
    // Before performing an integrity check against a database that does not have a maintained chain,
    //  you must rebuild the chain.
    public enum EventsDatabaseState
    {
        NormalChainIntact = 0,
        Busy = 1,
        NormalChainBroken = 2,
    }

    /// <summary>
    /// Summary description for EventDatabase.
    /// </summary>
    public class EventDatabase
    {
        #region Constants
        internal const int updateSize = 5000;

        #endregion

        #region Public Routines

        //-----------------------------------------------------------------------		
        // GetDatabaseName
        //-----------------------------------------------------------------------		
        static public string
           GetDatabaseName(
              string instance
           )
        {
            string databaseName = String.Format("{0}{1}",
                                                   CoreConstants.Repository_EventDBPrefix,
                                                   instance.Replace(@"\", @"_"));
            if (databaseName.Length > 128)
            {
                databaseName = databaseName.Substring(0, 128);
            }
            return databaseName;
        }

        //-----------------------------------------------------------------------		
        // Create - creates new events database for a server along
        //          with empty event tables
        //
        //          Return: Name of new database
        //-----------------------------------------------------------------------		
        static public string
           Create(
              string instance,
              string databaseName,
              int defaultAccess,
              SqlConnection conn
           )
        {
            // Create database
            try
            {
                CreateDatabase(databaseName, conn);
            }
            catch (SqlException sqlEx)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format("CreateDatabase SQLexception\nError Class: {0}\nError Number: {1}\nMessage {2}",
                                                        sqlEx.Number,
                                                        sqlEx.Class,
                                                        sqlEx.Message));
                throw;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format("CreateDatabase Exception\n\nMessage {0}",
                                         ex.Message));
                throw;
            }

            // Create tables & security
            try
            {
                // set recovery model
                SetRecoveryModel(databaseName, conn);

                // create database
                CreateEventsTable(databaseName, conn, false);
                CreateSQLTable(databaseName, conn);
                CreateDescriptionTable(instance, databaseName, conn);
                CreateIdLookUpTables(databaseName, conn);
                CreateStatsTable(databaseName, conn);
                CreateDataChangesTable(databaseName, conn, false);
                CreateColumnChangesTable(databaseName, conn, false);
                CreateDataChangeLinkProcs(databaseName, conn);
                CreateSensitiveColumnsTable(databaseName, conn, false);
                CreateInstanceDatabasesTable(databaseName, conn);
                BuildIndexes(databaseName, conn, false);
                Build30Indexes(conn, databaseName);
                Build31Indexes(conn, databaseName);
                Build35Indexes(conn, databaseName);
                BuildPageIndexes(conn, databaseName);

                // Add security
                SetDefaultSecurity(databaseName,
                                  defaultAccess,
                                  -1,
                                  false,
                                  conn);
                // Update SystemDatabase Table
                AddSystemDatabase(instance,
                                   databaseName,
                                   conn);
            }
            catch
            {
                DropDatabase(databaseName, conn);
                throw;
            }

            return databaseName;
        }

        //-----------------------------------------------------------------------		
        // SetRecoveryModel
        //-----------------------------------------------------------------------		
        static public void
           SetRecoveryModel(
              string databaseName,
              SqlConnection conn
           )
        {
            int recoveryModel = SQLcomplianceConfiguration.GetRecoveryModel();

            if (recoveryModel == 0)
            {
                string sql = String.Format("ALTER DATABASE {0} SET RECOVERY SIMPLE",
                                            SQLHelpers.CreateSafeDatabaseName(databaseName));
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //-----------------------------------------------------------------------		
        // DatabaseExists
        //-----------------------------------------------------------------------		
        static public bool
           DatabaseExists(
              string dbName,
              SqlConnection conn
           )
        {
            bool exists = false;

            try
            {
                string existsSql =
                   String.Format("select name from master..sysdatabases where name={0}",
                                  SQLHelpers.CreateSafeString(dbName));
                using (SqlCommand sqlcmd = new SqlCommand(existsSql, conn))
                {
                    sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    using (SqlDataReader reader = sqlcmd.ExecuteReader())
                    {
                        if (reader.Read())
                            exists = true;
                    }
                }
            }
            catch
            { }

            return exists;
        }

        //-----------------------------------------------------------------------		
        // IsCompatibleSchema
        //
        // Two flavors
        // (1) Is compatible - No difference in 100's digit of version
        // (2) Is upgradeable - is old version less then existing
        //-----------------------------------------------------------------------		
        static public bool IsCompatibleSchema(string dbName, SqlConnection conn)
        {
            string sql = String.Format("SELECT eventDbSchemaVersion FROM {0}.dbo.{1} ",
                                        SQLHelpers.CreateSafeDatabaseName(dbName),
                                        CoreConstants.RepositoryMetaTable);

            int evSchema = 0;
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        evSchema = SQLHelpers.GetInt32(reader, 0);
                    }
                }
            }
            return IsCompatibleSchema(evSchema);
        }

        static public bool IsCompatibleSchema(int eventsSchemaVersion)
        {
            if (eventsSchemaVersion / 100 == CoreConstants.RepositoryEventsDbSchemaVersion / 100)
                return true;
            else
                return false;
        }

        static public bool IsUpgradeableSchema(string dbName, SqlConnection conn)
        {
            try
            {
                string sql = String.Format("SELECT eventDbSchemaVersion FROM {0}.dbo.{1} ",
                                            SQLHelpers.CreateSafeDatabaseName(dbName),
                                            CoreConstants.RepositoryMetaTable);

                int evSchema = 0;
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            evSchema = SQLHelpers.GetInt32(reader, 0);
                        }
                    }
                }

                if (evSchema <= CoreConstants.RepositoryEventsDbSchemaVersion)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        //-----------------------------------------------------------------------		
        // InitializeExistingEventDatabase
        //-----------------------------------------------------------------------		
        static public void
           InitializeExistingEventDatabase(
              string instance,
              string databaseName,
              int defaultAccess,
              SqlConnection conn
           )
        {
            // Create tables & security
            try
            {
                // drop existing tables
                DropTable(databaseName, CoreConstants.RepositoryEventsTable, conn);
                DropTable(databaseName, CoreConstants.RepositoryEventSqlTable, conn);
                DropTable(databaseName, CoreConstants.RepositoryMetaTable, conn);
                DropTable(databaseName, CoreConstants.RepositoryApplicationsTable, conn);
                DropTable(databaseName, CoreConstants.RepositoryHostsTable, conn);
                DropTable(databaseName, CoreConstants.RepositoryLoginsTable, conn);
                DropTable(databaseName, StatsDAL.StatsTable, conn);
                DropTable(databaseName, CoreConstants.InstanceDatabasesTable, conn);
                DropTable(databaseName, CoreConstants.RepositoryColumnChangesTable, conn);
                DropTable(databaseName, CoreConstants.RepositoryDataChangesTable, conn);
                DropTable(databaseName, CoreConstants.RepositorySensitiveColumnsTable, conn);

                // create database
                CreateEventsTable(databaseName, conn, false);
                CreateSQLTable(databaseName, conn);
                CreateDescriptionTable(instance, databaseName, conn);
                CreateStatsTable(databaseName, conn);
                CreateIdLookUpTables(databaseName, conn);
                CreateDataChangesTable(databaseName, conn, false);
                CreateColumnChangesTable(databaseName, conn, false);
                CreateDataChangeLinkProcs(databaseName, conn);
                CreateSensitiveColumnsTable(databaseName, conn, false);
                CreateInstanceDatabasesTable(databaseName, conn);
                BuildIndexes(databaseName, conn, false);
                Build30Indexes(conn, databaseName);
                Build31Indexes(conn, databaseName);
                Build35Indexes(conn, databaseName);
                BuildPageIndexes(conn, databaseName);
                // Add security
                SetDefaultSecurity(databaseName,
                                    defaultAccess,
                                    -1,
                                    false,
                                    conn);
            }
            catch
            {
                throw;
            }

            return;
        }

        //-----------------------------------------------------------------------		
        // GetWatermarks
        //-----------------------------------------------------------------------		
        static public void
           GetWatermarks(
              string databaseName,
              out int lowWatermark,
              out int highWatermark,
              SqlConnection conn
           )
        {
            lowWatermark = -2100000000;
            highWatermark = -2100000000;

            string sql = String.Format("SELECT MIN(eventId), MAX(eventId) FROM {0}..{1}",
                                        SQLHelpers.CreateSafeDatabaseName(databaseName),
                                        CoreConstants.RepositoryEventsTable);
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            lowWatermark = reader.GetInt32(0);
                        }
                        else
                        {
                            lowWatermark = -2100000000;
                        }

                        if (!reader.IsDBNull(1))
                        {
                            highWatermark = reader.GetInt32(1);
                        }
                        else
                        {
                            highWatermark = -2100000000;
                        }
                    }
                }
            }
        }


        static internal void UpdateIdTableEx(SqlConnection conn, string database, string tableName, string schema, string name, int id)
        {
            try
            {
                String stmt =
                   String.Format("IF NOT EXISTS (SELECT schemaName, name FROM {0}..{1} WHERE schemaName = {2} AND name = {3}) " +
                                  "INSERT INTO {0}..{1} ( schemaName, name, id ) VALUES ({2}, {3}, {4})",
                                  SQLHelpers.CreateSafeDatabaseName(database),
                                  tableName,
                                  SQLHelpers.CreateSafeString(schema),
                                  SQLHelpers.CreateSafeString(name),
                                  id);
                using (SqlCommand cmd = new SqlCommand(stmt, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "An error occurred updating ID look up table " +
                                         tableName + ".",
                                         e,
                                         ErrorLog.Severity.Warning);
                throw;
            }
        }

        static internal void UpdateIdTable(SqlConnection conn, string database, string tableName, string name, int id)
        {
            try
            {
                String stmt =
                   String.Format("IF NOT EXISTS (SELECT name FROM {0}..{1} WHERE name = {2}) " +
                                  "INSERT INTO {0}..{1} ( name, id ) VALUES ({2}, {3})",
                                  SQLHelpers.CreateSafeDatabaseName(database),
                                  tableName,
                                  SQLHelpers.CreateSafeString(name),
                                  id);
                using (SqlCommand cmd = new SqlCommand(stmt, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "An error occurred updating ID look up table " +
                                         tableName + ".",
                                         e,
                                         ErrorLog.Severity.Warning);
                throw;
            }
        }

        #endregion

        #region Upgrade Logic

        static public void UpgradeEventDatabase(SqlConnection conn, string databaseName)
        {
            string alter = String.Format("EXEC {0}..{1} {2}",
               CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryStoredProcUpgradeEventDatabase,
               SQLHelpers.CreateSafeDatabaseName(databaseName));
            using (SqlCommand cmd = new SqlCommand(alter, conn))
            {
                cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                cmd.ExecuteNonQuery();
            }
        }

        //
        // This function updates indexes that were added to tables after they had already existed within the product.
        //  If a new table is added with corresponding indexes, it does not need to be in this function
        //  This exists so users can control when to update large tables with our updated indexes.
        static public void UpdateIndexes(SqlConnection conn, string databaseName)
        {
            // Get old schema version
            int oldVersion = GetDatabaseSchemaVersion(conn, databaseName);
            StringBuilder indexBatch = new StringBuilder();

            //X03 is the version that has the fixed indexes.
            if (((oldVersion - 3) % 100) == 0)
            {
                return;
            }

            //the Event database schema must be at least 50X
            if ((oldVersion / 100) < 5)
            {
                throw new Exception(String.Format("UpdateIndexes:  Unable to upgrade indexes for {0}.  Schema version is incorrect:  {1}", databaseName, oldVersion));
            }

            // We have a X01 version schema, meaning original indexes.  Drop them outright.
            if (((oldVersion - 1) % 100) == 0)
            {
                // These share the name of our current indexes - however, the case on PrivUser and EventId indexes is different.
                indexBatch.Append("IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_StartTime') DROP INDEX Events.IX_Events_StartTime;");
                indexBatch.Append("IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_PrivilegedUser') DROP INDEX Events.IX_Events_PrivilegedUser;");
                indexBatch.Append("IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_EventId') DROP INDEX Events.IX_Events_EventId;");
            }

            //SQLCM-6207 SQLCM 5.8 Moved to Index Upgrade Utility "SQLcomplianceIndexUpgrade"

            //Fix the clustered index on the Events table.
            //indexBatch.Append("IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_eventId') BEGIN ");

            //if (SQLHelpers.GetSqlVersion(conn) < 9)
            //{
            //    //For SQL Server 2000
            //    indexBatch.Append("IF OBJECT_ID('ChangeLog') IS NULL CREATE UNIQUE CLUSTERED INDEX [IX_Events_eventId] ON [dbo].[Events]([eventId] ASC ) WITH DROP_EXISTING ON [PRIMARY];");
            //    indexBatch.Append("ELSE CREATE CLUSTERED INDEX [IX_Events_eventId] ON [dbo].[Events]([eventId] ASC ) WITH DROP_EXISTING ON [PRIMARY]; END ");
            //}
            //else
            //{
            //    //For SQL Server 2005 and later
            //    indexBatch.Append("IF OBJECT_ID('ChangeLog') IS NULL CREATE UNIQUE CLUSTERED INDEX [IX_Events_eventId] ON [dbo].[Events]([eventId] ASC ) WITH (DROP_EXISTING = ON) ON [PRIMARY];");
            //    indexBatch.Append("ELSE CREATE CLUSTERED INDEX [IX_Events_eventId] ON [dbo].[Events]([eventId] ASC ) WITH (DROP_EXISTING = ON) ON [PRIMARY]; END ");
            //}
            //indexBatch.Append("ELSE BEGIN IF OBJECT_ID('ChangeLog') IS NULL CREATE UNIQUE CLUSTERED INDEX [IX_Events_eventId] ON [dbo].[Events]([eventId] ASC ) ON [PRIMARY];");
            //indexBatch.Append("ELSE CREATE CLUSTERED INDEX [IX_Events_eventId] ON [dbo].[Events]([eventId] ASC ) ON [PRIMARY]; END ");

            // Create our indexes if they don't already exist
            //indexBatch.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_eventCategory') CREATE INDEX [IX_Events_eventCategory] ON [dbo].[Events]([eventCategory], [eventId] DESC ) ON [PRIMARY];");
            //indexBatch.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_eventType') CREATE INDEX [IX_Events_eventType] ON [dbo].[Events]([eventType], [eventId] DESC ) ON [PRIMARY];");
            //indexBatch.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_databaseId') CREATE INDEX [IX_Events_databaseId] ON [dbo].[Events]([databaseId], [eventId] DESC ) ON [PRIMARY];");
            //indexBatch.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_appNameId') CREATE  INDEX [IX_Events_appNameId] ON [dbo].[Events]([appNameId], [eventId] DESC ) ON [PRIMARY];");
            //indexBatch.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_hostId') CREATE  INDEX [IX_Events_hostId] ON [dbo].[Events]([hostId], [eventId] DESC ) ON [PRIMARY];");
            //indexBatch.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_loginId') CREATE  INDEX [IX_Events_loginId] ON [dbo].[Events]([loginId], [eventId] DESC ) ON [PRIMARY];");


            indexBatch.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_StartTime') CREATE INDEX [IX_Events_StartTime] ON [dbo].[Events]([startTime] DESC , [eventId] DESC ) ON [PRIMARY];");
            indexBatch.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_privilegedUser') CREATE INDEX [IX_Events_privilegedUser] ON [dbo].[Events]([privilegedUser], [eventId] DESC ) ON [PRIMARY];");

            // EventSQL indexes
            indexBatch.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_EventSQL_StartTime') CREATE INDEX [IX_EventSQL_StartTime] ON [dbo].[EventSQL]([startTime] DESC , [eventId] DESC ) ON [PRIMARY];");
            indexBatch.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_EventSQL_eventId') CREATE INDEX [IX_EventSQL_eventId] ON [dbo].[EventSQL]([eventId] DESC ) ON [PRIMARY];");

            //fix the indexes on the DataChanges table 2005 and greater.
            if (SQLHelpers.GetSqlVersion(conn) >= 9)
            {
                indexBatch.Append("IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_DataChanges_eventId') CREATE INDEX [IX_DataChanges_eventId] ON [dbo].[DataChanges] ([eventId] ASC, [recordNumber] ASC,[dcId] ASC)INCLUDE ( [startTime],[eventSequence],[spid],[databaseId],[actionType],[tableName],[userName],[changedColumns],[primaryKey],[hashcode],[totalChanges]) WITH (DROP_EXISTING = ON) ON [PRIMARY];");
                indexBatch.Append("ELSE CREATE INDEX [IX_DataChanges_eventId] ON [dbo].[DataChanges] ([eventId] ASC,	[recordNumber] ASC,[dcId] ASC)INCLUDE ( [startTime],[eventSequence],[spid],[databaseId],[actionType],[tableName],[userName],[changedColumns],[primaryKey],[hashcode],[totalChanges]) ON [PRIMARY];");
                indexBatch.Append("IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_DataChanges_tableId') CREATE INDEX [IX_DataChanges_tableId] ON [dbo].[DataChanges] ([tableId] ASC,	[eventId] ASC,[recordNumber] ASC,[dcId] ASC)INCLUDE ( [startTime],[eventSequence],[spid],[databaseId],[actionType],[tableName],[userName],[changedColumns],[primaryKey],[hashcode],[totalChanges]) WITH (DROP_EXISTING = ON) ON [PRIMARY];");
                indexBatch.Append("ELSE CREATE INDEX [IX_DataChanges_tableId] ON [dbo].[DataChanges] ([tableId] ASC,	[eventId] ASC,[recordNumber] ASC,[dcId] ASC)INCLUDE ( [startTime],[eventSequence],[spid],[databaseId],[actionType],[tableName],[userName],[changedColumns],[primaryKey],[hashcode],[totalChanges]) ON [PRIMARY];");
            }
            string msg = String.Format("Re-Indexing for {0} starting.", databaseName);
            LogRecord.WriteLog(conn, LogType.ReIndexStarted, "", msg);

            string currentDb = conn.Database;
            conn.ChangeDatabase(databaseName);

            try
            {
                using (SqlCommand cmd = new SqlCommand(indexBatch.ToString(), conn))
                {
                    // Infinite time
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                conn.ChangeDatabase(currentDb);
            }
            msg = String.Format("Re-Indexing for {0} finished.", databaseName);
            LogRecord.WriteLog(conn, LogType.ReIndexEnded, "", msg);

            UpdateSchemaVersion(conn, databaseName, CoreConstants.RepositoryEventsDbSchemaVersion);
        }

        //	   //----------------------------------------------------------------------
        //      // UpgradeEventDatabaseSchema - upgrades the schema of an events
        //      //                              database or archive database
        //      //
        //      //  Plan: Add new routines for each new version so an upgrade will be 
        //      //        be a succession of smaller upgrades
        //      //----------------------------------------------------------------------
        //      static public void UpgradeEventDatabase(SqlConnection conn, string databaseName, bool isArchive)
        //      {
        //         // We do not do index upgrades by default.  This requires verification from the user
        //         UpgradeEventDatabase(conn, databaseName, isArchive, 301) ;
        //      }
        //
        //      static public void UpgradeEventDatabase(SqlConnection conn, string databaseName, bool isArchive, int toVersion)
        //      {
        //         switch( toVersion )
        //         {
        //            // We only support these versions
        //            case 106:
        //            case 201:
        //            case 202:
        //            case 301:
        //            case 302:
        //            case 303:
        //               break;
        //            default:
        //               throw new Exception(
        //                  String.Format(
        //                     "Error:  Attempting to upgrade Events database {0} to an unsupported version:  {1}",
        //                     databaseName,
        //                     toVersion ) );
        //         }
        //         
        //         // Get old schema version
        //         int oldVersion = GetDatabaseSchemaVersion( conn, databaseName );
        //
        //         // We cannot revert to prior versions.
        //         if(oldVersion > toVersion)
        //         {
        //            throw new Exception(String.Format("Error:  Events database {0} cannot be upgraded to a prior schema version.",
        //               databaseName)) ;
        //         }
        //
        //         try
        //         {
        //            while(oldVersion < toVersion)
        //            {
        //               switch(oldVersion)
        //               {
        //                  case 106:
        //                     oldVersion = 201 ;
        //                     UpgradeTo201(conn, databaseName) ;
        //                     break ;
        //                  case 201:
        //                     switch( toVersion )
        //                     {
        //                        case 202:
        //                        case 302:
        //                        case 303:
        //                           oldVersion = 202;
        //                           UpgradeTo202( conn, databaseName, isArchive );
        //                           break;
        //                        case 301:
        //                           // ingore the 202 indexes
        //                           oldVersion = 301;
        //                           UpgradeTo30x( conn, databaseName, 301 );
        //                           break;
        //                     }
        //                     break ;
        //                  case 202:
        //                     // There is no upgrade path from 202 to 301 since the difference between
        //                     // 301 and 302 is that 301 doesn't have the 202 indexes.
        //                     oldVersion = 302;
        //                     UpgradeTo30x( conn, databaseName, 302 );
        //                     break;
        //                  case 301:
        //                     oldVersion = 302;
        //                     BuildIndexes( databaseName, conn, isArchive );
        //                     UpdateSchemaVersion(conn, databaseName, 302);
        //                     break;    
        //                  case 302:
        //                     oldVersion = 303 ;
        //                     Build30Indexes(conn, databaseName) ;
        //                     UpdateSchemaVersion(conn, databaseName, 303);
        //                     break;
        //                  case 303:
        //                     break ;
        //               }
        //            }
        //         }
        //         catch(Exception e)
        //         {
        //            string msg = String.Format( "Error upgrading events database {0} to version {1}.\n\nError: {2}",
        //               databaseName,
        //               oldVersion,
        //               e.Message );
        //            throw new Exception( msg );
        //         }
        //      }

        //      static private void UpgradeTo201(SqlConnection conn, string databaseName)
        //      {
        //         string alter = String.Format( 
        //            "ALTER TABLE {0}..{1} " + 
        //            "ADD 	 [fileName]         [nvarchar] (128) NULL," +
        //            "[linkedServerName] [nvarchar] (128) NULL, " +
        //            "[parentName]       [nvarchar] (128) NULL, " +
        //            "[isSystem]         [int]            NULL, " +
        //            "[sessionLoginName] [nvarchar] (128) NULL, " +
        //            "[providerName]     [nvarchar] (128) NULL  ",
        //            SQLHelpers.CreateSafeDatabaseName(databaseName),
        //            CoreConstants.RepositoryEventsTable );
        //         using ( SqlCommand cmd = new SqlCommand( alter, conn ) )
        //         {
        //            cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
        //            cmd.ExecuteNonQuery();
        //         }
        //               
        //         UpdateSchemaVersion( conn, databaseName, 201);
        //      }
        //
        //      //
        //      // UpgradeTo202
        //      //
        //      //  This function upgrades the event schema version to 202.  201 -> 202 is a pure
        //      //  index upgrade.  This can be a costly operation, therefore the user should
        //      //  verify that this is a desired operation.
        //      //
        //      static private void UpgradeTo202(SqlConnection conn, string databaseName, bool isArchive)
        //      {
        //         try
        //         {
        //            DropIndexes(conn,databaseName) ;
        //            BuildIndexes(databaseName, conn, isArchive) ;
        //            UpdateSchemaVersion( conn, databaseName, 202);
        //         }
        //         catch(Exception e)
        //         {
        //            throw new Exception(String.Format("UpgradeTo202 failed:  {0}", e.Message), e) ;
        //         }
        //      }
        //
        //      // UpgradeTo30x
        //      // 
        //      // This function upgrades the event schema version either from 201 to 301 or 202 to 302.
        //      // The difference between 301 and 302 is 301 doesn't have the 202 index upgrade.
        //      //
        //      static private void UpgradeTo30x(SqlConnection conn, string databaseName, int toVersion)
        //      {
        //         string stmt =  String.Format( "ALTER TABLE {0} " +
        //                                    "ADD 	 [appNameId]  [int] NULL," +
        //                                    "[hostId]         [int]            NULL, " +
        //                                    "[loginId]        [int]            NULL ",
        //                                    CoreConstants.RepositoryEventsTable);
        //         string msg = stmt;
        //         
        //         try
        //         {
        //            string sDatabase = conn.Database;
        //            conn.ChangeDatabase(databaseName);
        //            using (SqlCommand cmd = new SqlCommand(stmt, conn))
        //            {
        //               msg = cmd.CommandText;
        //               // Infinite time
        //               cmd.CommandTimeout = 0;
        //               cmd.ExecuteNonQuery();
        //            }
        //            conn.ChangeDatabase(sDatabase);
        //            
        //            if(toVersion == 303)
        //               Build30Indexes( conn, databaseName );
        //            
        //            // 3.0 tables
        //            msg = "Create ID look up table";
        //            CreateIdLookUpTables( databaseName, conn );
        //            msg = "Create Stats table";
        //            CreateStatsTable( databaseName, conn );
        //            
        //            msg = "Update schema version";
        //            UpdateSchemaVersion(conn, databaseName, toVersion);
        //         }
        //         catch (Exception e)
        //         {
        //            throw new Exception(String.Format("Upgrade to 30x failed on {0}", msg), e);
        //         }
        //      }

        //
        // Build 3.0 indexes for the new columns
        //
        // Events table got three new columns: appNameId, hostId and loginId.  Build indexes on
        // these columns to increase the performance on event view filtering and search.
        //
        static internal void Build30Indexes(SqlConnection conn, string databaseName)
        {
            List<string> stmts = new List<string>();
            string msg = "";
            // 3.0 Indexes
            stmts.Add(
               "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_appNameId') DROP INDEX Events.IX_Events_appNameId");
            stmts.Add(
               "CREATE  INDEX [IX_Events_appNameId] ON [dbo].[Events]([appNameId], [eventId] DESC ) ON [PRIMARY]");
            stmts.Add(
               "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_hostId') DROP INDEX Events.IX_Events_hostId");
            stmts.Add(
               "CREATE  INDEX [IX_Events_hostId] ON [dbo].[Events]([hostId], [eventId] DESC ) ON [PRIMARY]");
            stmts.Add(
               "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_loginId') DROP INDEX Events.IX_Events_loginId");
            stmts.Add(
               "CREATE  INDEX [IX_Events_loginId] ON [dbo].[Events]([loginId], [eventId] DESC ) ON [PRIMARY]");


            try
            {
                string sDatabase = conn.Database;
                conn.ChangeDatabase(databaseName);
                for (int i = 0; i < stmts.Count; i++)
                {
                    using (SqlCommand cmd = new SqlCommand(stmts[i], conn))
                    {
                        // Infinite time
                        msg = stmts[i];
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                    }
                }
                conn.ChangeDatabase(sDatabase);
            }
            catch (Exception e)
            {
                throw new Exception(
                   String.Format("Build 3.0 indexes failed on {0}", msg), e);
            }
        }

        //
        // Build 3.0 indexes for the new columns
        //
        // Events table got three new columns: appNameId, hostId and loginId.  Build indexes on
        // these columns to increase the performance on event view filtering and search.
        //
        static internal void Build31Indexes(SqlConnection conn, string databaseName)
        {
            List<string> stmts = new List<string>();
            string msg = "";

            // Only create indexes on 2005 or greater
            if (SQLHelpers.GetSqlVersion(conn) < 9)
                return;

            // Data Change Indexes
            stmts.Add("IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ColumnChanges_dcId') DROP INDEX ColumnChanges.IX_ColumnChanges_dcId");
            stmts.Add("IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ColumnChanges_FK') DROP INDEX ColumnChanges.IX_ColumnChanges_FK");
            stmts.Add("IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_DataChanges_startTime') DROP INDEX DataChanges.IX_DataChanges_startTime");
            stmts.Add("IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_DataChanges_eventId') DROP INDEX DataChanges.IX_DataChanges_eventId");
            stmts.Add("IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ColumnChanges_columnId') DROP INDEX ColumnChanges.IX_ColumnChanges_columnId");
            stmts.Add("IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_DataChanges_tableId') DROP INDEX DataChanges.IX_DataChanges_tableId");
            stmts.Add("IF EXISTS (SELECT name from sys.stats where name = N'ST_ColumnChanges' and object_id = object_id(N'[dbo].[ColumnChanges]')) DROP STATISTICS [dbo].[ColumnChanges].[ST_ColumnChanges]");
            stmts.Add("IF EXISTS (SELECT name from sys.stats where name = N'ST_DataChanges1' and object_id = object_id(N'[dbo].[DataChanges]')) DROP STATISTICS [dbo].[DataChanges].[ST_DataChanges1]");
            stmts.Add("IF EXISTS (SELECT name from sys.stats where name = N'ST_DataChanges2' and object_id = object_id(N'[dbo].[DataChanges]')) DROP STATISTICS [dbo].[DataChanges].[ST_DataChanges2]");
            stmts.Add("IF EXISTS (SELECT name from sys.stats where name = N'ST_DataChanges3' and object_id = object_id(N'[dbo].[DataChanges]')) DROP STATISTICS [dbo].[DataChanges].[ST_DataChanges3]");
            stmts.Add("IF EXISTS (SELECT name from sys.stats where name = N'ST_DataChanges4' and object_id = object_id(N'[dbo].[DataChanges]')) DROP STATISTICS [dbo].[DataChanges].[ST_DataChanges4]");
            stmts.Add("IF EXISTS (SELECT name from sys.stats where name = N'ST_DataChanges5' and object_id = object_id(N'[dbo].[DataChanges]')) DROP STATISTICS [dbo].[DataChanges].[ST_DataChanges5]");

            stmts.Add("Alter Table ColumnChanges Alter Column ccId bigint");
            stmts.Add("Alter Table ColumnChanges Alter Column dcId bigint");
            stmts.Add("Alter Table DataChanges Alter Column dcId bigint");           

            stmts.Add("CREATE NONCLUSTERED INDEX [IX_ColumnChanges_dcId] ON [dbo].[ColumnChanges] ([dcId] ASC) INCLUDE ( [startTime],[eventSequence],[spid],[columnName],[beforeValue],[afterValue],[hashcode]) ON [PRIMARY]");           
            stmts.Add("CREATE INDEX [IX_ColumnChanges_FK] ON ColumnChanges(startTime, eventSequence, spid ) ON [PRIMARY]");            
            stmts.Add("CREATE CLUSTERED INDEX [IX_DataChanges_startTime] ON [dbo].[DataChanges] ([startTime] ASC,	[eventSequence] ASC,[spid] ASC) ON [PRIMARY]");            
            stmts.Add("CREATE NONCLUSTERED INDEX [IX_DataChanges_eventId] ON [dbo].[DataChanges] ([eventId] ASC,	[recordNumber] ASC,[dcId] ASC)INCLUDE ( [startTime],[eventSequence],[spid],[databaseId],[actionType],[tableName],[userName],[changedColumns],[primaryKey],[hashcode],[totalChanges]) ON [PRIMARY]");  
            stmts.Add("CREATE CLUSTERED INDEX [IX_ColumnChanges_columnId] ON [dbo].[ColumnChanges] ([columnId] ASC) ON [PRIMARY]");            
            stmts.Add("CREATE NONCLUSTERED INDEX [IX_DataChanges_tableId] ON [dbo].[DataChanges] ([tableId] ASC,	[eventId] ASC,[recordNumber] ASC,[dcId] ASC)INCLUDE ( [startTime],[eventSequence],[spid],[databaseId],[actionType],[tableName],[userName],[changedColumns],[primaryKey],[hashcode],[totalChanges])  ON [PRIMARY]");            
            stmts.Add("CREATE STATISTICS [ST_ColumnChanges] ON [dbo].[ColumnChanges]([dcId], [columnId])");            
            stmts.Add("CREATE STATISTICS [ST_DataChanges1] ON [dbo].[DataChanges]([dcId], [recordNumber])");            
            stmts.Add("CREATE STATISTICS [ST_DataChanges2] ON [dbo].[DataChanges]([dcId], [eventId])");            
            stmts.Add("CREATE STATISTICS [ST_DataChanges3] ON [dbo].[DataChanges]([dcId], [tableId])");            
            stmts.Add("CREATE STATISTICS [ST_DataChanges4] ON [dbo].[DataChanges]([recordNumber], [eventId], [tableId])");            
            stmts.Add("CREATE STATISTICS [ST_DataChanges5] ON [dbo].[DataChanges]([eventId], [dcId], [tableId], [recordNumber])");

            try
            {
                string sDatabase = conn.Database;
                conn.ChangeDatabase(databaseName);
                for (int i = 0; i < stmts.Count; i++)
                {
                    using (SqlCommand cmd = new SqlCommand(stmts[i], conn))
                    {
                        // Infinite time
                        msg = stmts[i];
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                    }
                }
                conn.ChangeDatabase(sDatabase);
            }
            catch (Exception e)
            {
                throw new Exception(
                   String.Format("Build DataChange indexes failed on {0}", msg), e);
            }
        }

        static internal void Build35Indexes(SqlConnection conn, string databaseName)
        {
            List<string> stmts = new List<string>();
            string msg = "";

            // Only create indexes on 2005 or greater
            if (SQLHelpers.GetSqlVersion(conn) < 9)
                return;

            // Sensitive Column Indexes
            stmts.Add("IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_SensitiveColumns_eventId') DROP INDEX SensitiveColumns.IX_ColumnChanges_dcId");
            stmts.Add("CREATE CLUSTERED INDEX [IX_SensitiveColumns_eventId] ON [dbo].[SensitiveColumns] ([eventId] ASC) ON [PRIMARY]");
            stmts.Add("IF EXISTS (SELECT name from sys.stats where name = N'ST_SensitiveColumns' and object_id = object_id(N'[dbo].[SensitiveColumns]')) DROP STATISTICS [dbo].[SensitiveColumns].[ST_SensitiveColumns]");
            stmts.Add("CREATE STATISTICS [ST_SensitiveColumns] ON [dbo].[SensitiveColumns]([eventId])");

            try
            {
                string sDatabase = conn.Database;
                conn.ChangeDatabase(databaseName);
                for (int i = 0; i < stmts.Count; i++)
                {
                    using (SqlCommand cmd = new SqlCommand(stmts[i], conn))
                    {
                        // Infinite time
                        msg = stmts[i];
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                    }
                }
                conn.ChangeDatabase(sDatabase);
            }
            catch (Exception e)
            {
                throw new Exception(
                   String.Format("Build DataChange indexes failed on {0}", msg), e);
            }
        }      //
               // PopulateLookUpIds
               //
               // After upgrading the schema an 20x EventDatabase to 30x, use this function to populate
               // appNameId, hostId and loginId.  This is optional and need to prompt user before the
               // process.
               //
        static internal void PopulateLookUpIds(SqlConnection conn, string databaseName)
        {
            string sDb = conn.Database;
            try
            {
                CreateIdLookUpTables(databaseName, conn);
                conn.ChangeDatabase(databaseName);
                /*
                PopulateLookUpIdsHelper(conn, databaseName, "applicationName", "appNameId", CoreConstants.RepositoryApplicationsTable);
                PopulateLookUpIdsHelper(conn, databaseName, "hostName", "hostId", CoreConstants.RepositoryHostsTable);
                PopulateLookUpIdsHelper(conn, databaseName, "loginName", "loginId", CoreConstants.RepositoryLoginsTable);
                 */
                Dictionary<string, int> appIdCache = GetLookUpIDList(conn, databaseName, "applicationName", "appNameId", CoreConstants.RepositoryApplicationsTable);
                Dictionary<string, int> hostIdCache = GetLookUpIDList(conn, databaseName, "hostName", "hostId", CoreConstants.RepositoryHostsTable);
                Dictionary<string, int> loginIdCache = GetLookUpIDList(conn, databaseName, "loginName", "loginId", CoreConstants.RepositoryLoginsTable);
                PopulateLookUpIds(conn, databaseName, appIdCache, hostIdCache, loginIdCache);

            }
            finally
            {
                conn.ChangeDatabase(sDb);
            }
        }

        static private void PopulateLookUpIds(
                             SqlConnection conn,
                             string databaseName,
                             IDictionary<string, int> appIdCache,
                             IDictionary<string, int> hostIdCache,
                             IDictionary<string, int> loginIdCache)
        {

            try
            {
                string query =
                   "SELECT eventId, applicationName, hostName, loginName FROM Events ORDER BY eventId";
                using (SqlCommand readCmd = new SqlCommand(query, conn))
                {
                    readCmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    using (SqlDataReader reader = readCmd.ExecuteReader())
                    {
                        using (SqlConnection writeConn = new SqlConnection(conn.ConnectionString))
                        {
                            writeConn.Open();
                            writeConn.ChangeDatabase(databaseName);
                            while (reader.Read())
                            {
                                int eventId;
                                string app;
                                string host;
                                string login;

                                eventId = reader.GetInt32(0);
                                app = reader.GetString(1);
                                host = reader.GetString(2);
                                login = reader.GetString(3);

                                string stmt =
                                   String.Format(
                                      "UPDATE Events SET appNameId = {0}, hostId = {1}, loginId = {2} WHERE eventId = {3}",
                                      appIdCache[app],
                                      hostIdCache[host],
                                      loginIdCache[login],
                                      eventId);
                                using (SqlCommand cmd = new SqlCommand(stmt, writeConn))
                                {
                                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                   String.Format("Populating IDs failed.  Error: {0}.",
                                  ex.Message));
            }

        }

        static private Dictionary<string, int> GetExistingLookUpIDs(SqlConnection conn, string databaseName, string lookupTable)
        {
            Dictionary<string, int> lookupCache = new Dictionary<string, int>();
            string baseQuery = "SELECT name,id FROM {0}..{1} ORDER BY name ASC";
            string query;

            query = String.Format(baseQuery,
               SQLHelpers.CreateSafeDatabaseName(databaseName), lookupTable);
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = SQLHelpers.GetString(reader, 0);
                        int id = SQLHelpers.GetInt32(reader, 1);
                        lookupCache.Add(name, id);
                    }
                }
            }
            return lookupCache;
        }

        static private Dictionary<string, int> GetLookUpIDList(SqlConnection conn, string databaseName, string strColumn, string idColumn, string lookupTable)
        {
            Dictionary<string, int> lookupCache = GetExistingLookUpIDs(conn, databaseName, lookupTable);
            try
            {
                CreateIdLookUpTables(databaseName, conn);

                string query =
                   String.Format("SELECT DISTINCT {0} FROM {1} WHERE {2} IS NULL",
                                  strColumn,
                                  CoreConstants.RepositoryEventsTable,
                                  idColumn);

                // Load the cache
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name;
                            int id;

                            name = reader.GetString(0);
                            id = NativeMethods.GetHashCode(name);

                            if (!lookupCache.ContainsKey(name))
                            {
                                lookupCache.Add(name, id);
                            }
                        }
                    }
                }

                foreach (string name in lookupCache.Keys)
                {
                    UpdateIdTable(conn, databaseName, lookupTable, name, lookupCache[name]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                   String.Format("Calculating IDs failed.  Error: {0}.",
                                  ex.Message));
            }

            return lookupCache;
        }

        static private void PopulateLookUpIdsHelper(SqlConnection conn, string databaseName, string strColumn, string idColumn, string lookupTable)
        {
            string sDb = conn.Database;
            string message = "create ID look up tables.";

            Dictionary<string, int> lookupCache = new Dictionary<string, int>();
            try
            {
                CreateIdLookUpTables(databaseName, conn);
                conn.ChangeDatabase(databaseName);

                message = "loading ID cache";
                string query = String.Format("SELECT DISTINCT {0} FROM {1} WHERE {2} IS NULL",
                                             strColumn, CoreConstants.RepositoryEventsTable, idColumn);

                // Load the cache
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name;
                            int id;

                            name = reader.GetString(0);
                            id = NativeMethods.GetHashCode(name);

                            if (!lookupCache.ContainsKey(name))
                                lookupCache.Add(name, id);
                        }
                    }
                }
                foreach (string name in lookupCache.Keys)
                {
                    int id = lookupCache[name];
                    message = "Setting IDs in Events";
                    query = String.Format("UPDATE TOP( {0} ) {1} SET {2}={3} WHERE {2} is NULL AND {4}={5}",
                       updateSize,
                       CoreConstants.RepositoryEventsTable,
                       idColumn,
                       id,
                       strColumn,
                       SQLHelpers.CreateSafeString(name));
                    int rows = updateSize;

                    while (rows >= updateSize)
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                            rows = cmd.ExecuteNonQuery();
                        }
                    }
                    message = "Setting ID in lookup table";
                    UpdateIdTable(conn, databaseName, lookupTable, name, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                   String.Format("Populating IDs failed on {0}.  Error: {1}.", message, ex.Message));
            }
            finally
            {
                conn.ChangeDatabase(sDb);
            }

        }


        //
        // DropIndexes
        //
        // This function drops any indexes that previously existed in SQLcm.  During an index upgrade,
        //  this function should be called prior to installing the new indexes.
        //  
        static private void DropIndexes(SqlConnection conn, string databaseName)
        {
            string[] stmts = new string[3];
            // 1.1 Indexes
            stmts[0] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_StartTime') DROP INDEX Events.IX_Events_StartTime";
            stmts[1] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_PrivilegedUser') DROP INDEX Events.IX_Events_PrivilegedUser";
            stmts[2] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_EventId') DROP INDEX Events.IX_Events_EventId";

            int i = 0;
            try
            {
                string sDatabase = conn.Database;
                conn.ChangeDatabase(databaseName);
                for (i = 0; i < stmts.Length; i++)
                {
                    using (SqlCommand cmd = new SqlCommand(stmts[i], conn))
                    {
                        // Infinite time
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                    }
                }
                conn.ChangeDatabase(sDatabase);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Drop Indexes failed on {0}", stmts[i]), e);
            }
        }

        // 
        // BuildIndexes()
        //
        // This function installs the currently supported indexes on an Events database.  It will
        //  drop these if they currently exist.  Any index upgrade should essentially call this function.
        //
        static internal void BuildIndexes(string databaseName, SqlConnection conn, bool isArchive)
        {
            string[] stmts = new string[30];
            // 2.1 Indexes
            stmts[0] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_eventId') DROP INDEX Events.IX_Events_eventId";
            stmts[1] = String.Format("CREATE {0} CLUSTERED INDEX [IX_Events_eventId] ON [dbo].[Events]([eventId] ASC ) ON [PRIMARY]", isArchive ? "" : "UNIQUE");
            stmts[2] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_StartTime') DROP INDEX Events.IX_Events_StartTime";
            stmts[3] = "CREATE INDEX [IX_Events_StartTime] ON [dbo].[Events]([startTime] DESC , [eventId] DESC ) ON [PRIMARY]";
            stmts[4] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_eventCategory') DROP INDEX Events.IX_Events_eventCategory";
            stmts[5] = "CREATE INDEX [IX_Events_eventCategory] ON [dbo].[Events]([eventCategory], [eventId] DESC ) ON [PRIMARY]";
            stmts[6] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_eventType') DROP INDEX Events.IX_Events_eventType";
            stmts[7] = "CREATE INDEX [IX_Events_eventType] ON [dbo].[Events]([eventType], [eventId] DESC ) ON [PRIMARY]";
            stmts[8] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_databaseId') DROP INDEX Events.IX_Events_databaseId";
            stmts[9] = "CREATE INDEX [IX_Events_databaseId] ON [dbo].[Events]([databaseId], [eventId] DESC ) ON [PRIMARY]";
            stmts[10] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_privilegedUser') DROP INDEX Events.IX_Events_privilegedUser";
            stmts[11] = "CREATE INDEX [IX_Events_privilegedUser] ON [dbo].[Events]([privilegedUser], [eventId] DESC ) ON [PRIMARY]";
            // EventSQL indexes
            stmts[12] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_EventSQL_StartTime') DROP INDEX EventSQL.IX_EventSQL_StartTime";
            stmts[13] = "CREATE INDEX [IX_EventSQL_StartTime] ON [dbo].[EventSQL]([startTime] DESC , [eventId] ASC ) ON [PRIMARY]";
            stmts[14] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_EventSQL_eventId') DROP INDEX EventSQL.IX_EventSQL_eventId";
            stmts[15] = "CREATE INDEX [IX_EventSQL_eventId] ON [dbo].[EventSQL]([eventId] DESC ) ON [PRIMARY]";
            stmts[16] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_eventId') DROP INDEX Events.IX_Events_eventId";
            stmts[17] = "CREATE UNIQUE CLUSTERED INDEX [IX_Events_StartTime_startSeq_eventId] ON [dbo].[Events]( [startTime] ASC, [startSequence] ASC, [eventId] ASC ) WITH (DATA_COMPRESSION = PAGE)";
            stmts[18] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_BAD_guid_sSeq_inc_eType_eCat') DROP INDEX Events.IX_BAD_guid_sSeq_inc_eType_eCat";
            stmts[19] = "CREATE INDEX [IX_BAD_guid_sSeq_inc_eType_eCat] ON [dbo].[Events]( [guid] ASC, [startSequence] ASC )INCLUDE( [eventType], [eventCategory] )";
            stmts[20] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Stats_Category_date_lastUp') DROP INDEX Events.IX_Stats_Category_date_lastUp";
            stmts[21] = "CREATE INDEX [IX_Stats_Category_date_lastUp] ON [dbo].[Stats] ([category],[date],[lastUpdated]) INCLUDE ([count])  ON [PRIMARY]";
            stmts[22] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_BAD_guid_eventSequence') DROP INDEX Events.IX_BAD_guid_eventSequence";
            stmts[23] = "CREATE INDEX [IX_BAD_guid_eventSequence] ON [dbo].[DataChanges] ( [guid] ASC, [eventSequence] ASC, [eventId] ASC )  ON [PRIMARY]";

            stmts[24] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_DataChanges_dcId_eventId') DROP INDEX DataChanges.IX_DataChanges_dcId_eventId";
            stmts[25] = "CREATE  INDEX [IX_DataChanges_dcId_eventId]  ON [dbo].[DataChanges]( [dcId] ASC, [eventId] ASC)  ON [PRIMARY]";

            /// SQLCM-6333
            stmts[26] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Hosts_NameId') DROP INDEX Events.IX_Hosts_NameId";
            stmts[27] = "CREATE CLUSTERED INDEX [IX_Hosts_NameId] ON [dbo].[Hosts]([name] ASC, [id] ASC )";
            stmts[28] = "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Applications_NameId') DROP INDEX Events.IX_Applications_NameId";
            stmts[29] = "CREATE CLUSTERED INDEX [IX_Applications_NameId] ON [dbo].[Applications]([name] ASC, [id] ASC )";
            /// SQLCM-6333

            int i = 0;
            try
            {
                string sDatabase = conn.Database;
                conn.ChangeDatabase(databaseName);
                for (i = 0; i < stmts.Length; i++)
                {
                    using (SqlCommand cmd = new SqlCommand(stmts[i], conn))
                    {
                        // Infinite time
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                    }
                }
                conn.ChangeDatabase(sDatabase);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("BuildIndexes failed on {0}", stmts[i]), e);
            }
        }

        static internal void BuildPageIndexes(SqlConnection conn, string databaseName)
        {
            String edition = string.Empty;
            int repositoryVersion = SQLHelpers.GetSqlVersion(conn);
            if (repositoryVersion >= 11)
            {
                //SQLCM:6269 Check for SQL Server Enterprise Edition for 2012 and 2014(Data Compression is not supported except Enterprise Edition)
                ArrayList values = SQLHelpers.GetServerProperties(conn, "Edition");
                if (values != null)
                {
                    edition = Convert.ToString(values[0]);
                    if (repositoryVersion <= 12 && (!edition.Contains("Enterprise")))
                    {
                        return;
                    }
                }

                string[] stmts = new string[7];
                int i = 0;
                try
                {
                    stmts = CreateIndexStatement();
                    string sDatabase = conn.Database;
                    conn.ChangeDatabase(databaseName);
                    for (i = 0; i < stmts.Length; i++)
                    {
                        using (SqlCommand cmd = new SqlCommand(stmts[i], conn))
                        {
                            // Infinite time
                            cmd.CommandTimeout = 0;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    conn.ChangeDatabase(sDatabase);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Build Indexes failed on {0}", stmts[i]), ex);                    
                }
            }
        }

        static public string[] CreateIndexStatement()
        {
            return new string[7]{"IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_eventId') ALTER​ ​INDEX​ IX_Events_eventId  ON​ dbo​.​Events REBUILD​ ​PARTITION​ ​=​ ​ALL WITH ​(​DATA_COMPRESSION​ ​=​ ​PAGE​)",
            "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_eventCategory') ALTER​ ​INDEX​ IX_Events_eventCategory  ON​ dbo​.​Events REBUILD​ ​PARTITION​ ​=​ ​ALL WITH ​(​DATA_COMPRESSION​ ​=​ ​PAGE)",
            "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_eventType') ALTER​ ​INDEX​ IX_Events_eventType  ON​ dbo​.​Events REBUILD​ ​PARTITION​ ​=​ ​ALL WITH ​(​DATA_COMPRESSION​ ​=​ ​PAGE​)",
            "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_databaseId') ALTER​ ​INDEX​ IX_Events_databaseId  ON​ dbo​.​Events REBUILD​ ​PARTITION​ ​=​ ​ALL WITH ​(​DATA_COMPRESSION​ ​=​ ​PAGE​)",
            "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_appNameId') ALTER​ ​INDEX​  IX_Events_appNameId  ON​ dbo​.​Events  REBUILD​ ​PARTITION​ ​=​ ​ALL  WITH ​(​DATA_COMPRESSION​ ​=​ ​PAGE)",
            "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_hostId') ALTER​ ​INDEX​  IX_Events_hostId  ON​ dbo​.​Events  REBUILD​ ​PARTITION​ ​=​ ​ALL  WITH ​(​DATA_COMPRESSION​ ​=​ ​PAGE)",
            "IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Events_loginId') ALTER​ ​INDEX​  IX_Events_loginId  ON​ dbo​.​Events  REBUILD​ ​PARTITION​ ​=​ ​ALL  WITH ​(​DATA_COMPRESSION​ ​=​ ​PAGE)" };
        }

        //----------------------------------------------------------------------
        // GetDatabaseSchemaVersion
        //----------------------------------------------------------------------
        static public int
           GetDatabaseSchemaVersion(
           SqlConnection conn,
           string databaseName
           )
        {
            int schemaVersion;

            try
            {
                string selectCmd = String.Format("SELECT eventDbSchemaVersion FROM {0}..{1} ",
                   SQLHelpers.CreateSafeDatabaseName(databaseName),
                   CoreConstants.RepositoryMetaTable);
                using (SqlCommand cmd = new SqlCommand(selectCmd, conn))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            schemaVersion = SQLHelpers.GetInt32(reader, 0);
                        }
                        else
                        {
                            throw new Exception("Missing Description table.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = String.Format("Error reading Events Database version information.\n\nError: {0}",
                   ex.Message);
                throw new Exception(msg);
            }

            return schemaVersion;
        }

        //----------------------------------------------------------------------
        // UpdateSchemaVersion - updates eventSchemaVersion
        //----------------------------------------------------------------------
        static public void UpdateSchemaVersion(SqlConnection conn, string databaseName, int newVersion)
        {
            string upgradeVersion = String.Format("UPDATE {0}..{1} SET eventDbSchemaVersion={2}",
               SQLHelpers.CreateSafeDatabaseName(databaseName),
               CoreConstants.RepositoryMetaTable,
               newVersion);
            using (SqlCommand cmd = new SqlCommand(upgradeVersion, conn))
            {
                cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region  Low level - Update SQLsecure Configuration Tables

        //-----------------------------------------------------------------------		
        // AddSystemDatabase
        //-----------------------------------------------------------------------		
        static public void
           AddSystemDatabase(
              string instance,
              string dbName,
              SqlConnection conn
           )
        {
            AddSystemDatabase(instance,
                               dbName,
                               false,
                               "",
                               "",
                               conn);
        }

        static public void
           AddSystemDatabase(
              string instance,
              string dbName,
              bool archive,
              string displayName,
              string description,
              SqlConnection conn
           )
        {
            string cmdStr = "";
            Int16 sqlDatabaseId = 0;

            try
            {
                // get database id            
                cmdStr = String.Format("SELECT dbid from master..sysdatabases where name={0};",
                                        SQLHelpers.CreateSafeString(dbName));

                using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    object obj = cmd.ExecuteScalar();
                    if (obj is DBNull)
                    {
                        sqlDatabaseId = 0;
                    }
                    else
                    {
                        sqlDatabaseId = (Int16)obj;
                    }
                }

                // write record to system databases
                cmdStr = String.Format("INSERT INTO {0}..{1} " +
                                        "(" +
                                           "instance,databaseName,databaseType," +
                                           "dateCreated,sqlDatabaseId,displayName,description" +
                                        ") " +
                                        "VALUES (" +
                                           "{2},{3},'{4}'," +
                                           "GETUTCDATE(),{5},{6},{7}" +
                                        ")",
                                        CoreConstants.RepositoryDatabase,
                                        CoreConstants.RepositorySystemDatabaseTable,
                                        SQLHelpers.CreateSafeString(instance),
                                        SQLHelpers.CreateSafeString(dbName),
                                        (archive) ? CoreConstants.DatabaseType_Archive
                                                  : CoreConstants.DatabaseType_Events,
                                        sqlDatabaseId,
                                        SQLHelpers.CreateSafeString(displayName),
                                        SQLHelpers.CreateSafeString(description));

                using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException sqlEx)
            {
                // if database already exists here, then we are just recovering from an old delete bug
                // we will just use the entry that exists
                if (sqlEx.Number == CoreConstants.SqlErrorCode_DuplicateEntry)
                {
                    try
                    {
                        if (!archive)
                        {
                            // write record to system databases
                            cmdStr = String.Format("UPDATE {0}..{1} SET " +
                                                    "instance={2}," +
                                                    "databaseType='Event'," +
                                                    "dateCreated=GETUTCDATE()," +
                                                    "sqlDatabaseId={3} " +
                                                    "WHERE databaseName={4}",
                                                    CoreConstants.RepositoryDatabase,
                                                    CoreConstants.RepositorySystemDatabaseTable,
                                                    SQLHelpers.CreateSafeString(instance),
                                                    sqlDatabaseId,
                                                    SQLHelpers.CreateSafeString(dbName));
                            using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                            {
                                cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                                 "AddSystemDatabase",
                                                 cmdStr,
                                                 ex);
                        throw;
                    }
                }
                else
                {
                    throw sqlEx;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Low level - Create Database and tables

        //-----------------------------------------------------------------------		
        // CreateDatabase
        //-----------------------------------------------------------------------		
        static internal void
           CreateDatabase(
              string dbName,
              SqlConnection conn
           )
        {
            try
            {
                string createSql = String.Format("CREATE DATABASE {0}",
                                                  SQLHelpers.CreateSafeDatabaseName(dbName));
                using (SqlCommand sqlcmd = new SqlCommand(createSql, conn))
                {
                    sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    sqlcmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "CreateDatabase",
                                         String.Format("An error occurred creating {0}", dbName),
                                         ex);
                throw ex;
            }
        }

        /// <summary>
        /// This method creates a database at specified physical location.
        /// </summary>
        /// <param name="dbName">Name of database to create.</param>
        /// <param name="conn">An open connection to use for query execution.</param>
        /// <param name="databaseFilesLocation">Location (path) where database files have to be created.</param>
        static internal void CreateDatabase(string dbName,
                                            SqlConnection conn,
                                            string databaseFilesLocation)
        {

            try
            {
                String dataFilePath = System.IO.Path.Combine(databaseFilesLocation, string.Format("{0}Data.mdf", dbName));
                String logFilePath = System.IO.Path.Combine(databaseFilesLocation, string.Format("{0}Log.ldf", dbName));

                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.AppendFormat(" CREATE DATABASE {0}", SQLHelpers.CreateSafeDatabaseName(dbName));
                sqlBuilder.AppendFormat(" ON PRIMARY (NAME = '{0}_Data', FILENAME = '{1}')", dbName, dataFilePath);
                sqlBuilder.AppendFormat(" LOG ON (NAME = '{0}_Log', FILENAME = '{1}')", dbName, logFilePath);

                using (SqlCommand sqlcmd = new SqlCommand(sqlBuilder.ToString(), conn))
                {
                    sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    sqlcmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                        "CreateDatabase",
                                        String.Format("An error occurred creating {0}", dbName),
                                        ex,
                                        ErrorLog.Severity.Error);
                throw ex;
            }
        }

        //-----------------------------------------------------------------------		
        // DropDatabase
        //-----------------------------------------------------------------------		
        static internal void
           DropDatabase(
              string dbName,
              SqlConnection conn
           )
        {
            try
            {
                string createSql = String.Format("DROP DATABASE {0}",
                                                 SQLHelpers.CreateSafeDatabaseName(dbName));
                using (SqlCommand sqlcmd = new SqlCommand(createSql, conn))
                {
                    sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    sqlcmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                // this is a cleanup routine - we dont care so catch and throw away
            }
        }


        //-----------------------------------------------------------------------		
        // DropTable
        //-----------------------------------------------------------------------		
        static private void
           DropTable(
              string database,
              string table,
              SqlConnection conn
           )
        {
            try
            {
                string dropSql = String.Format("DROP TABLE {0}..{1} ",
                                                SQLHelpers.CreateSafeDatabaseName(database),
                                                table);

                using (SqlCommand sqlcmd = new SqlCommand(dropSql, conn))
                {
                    sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    sqlcmd.ExecuteNonQuery();
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number != 3701) // ignore table doesnt exist errors
                {
                    throw sqlEx;
                }
            }
        }

        //-----------------------------------------------------------------------		
        // CreateEventsTable - Set archiveFlag to false
        //
        //-----------------------------------------------------------------------		
        static internal void
             CreateEventsTable(
                string databaseName,
              SqlConnection conn,
              bool archiveDatabase
             )
        {
            try
            {
                string createTmp = "CREATE TABLE {0}..[Events] (" +
                                   "	[startTime] [datetime] NOT NULL ," +
                                   "	[checksum] [int] NOT NULL," +
                                   "	[eventId] [int] {1} NOT NULL ," +
                                   "	[eventType] [int] NOT NULL ," +
                                   "	[eventClass] [int] NOT NULL ," +
                                   "	[eventSubclass] [int] NULL ," +
                                   "	[spid] [int] NULL ," +
                                   "	[applicationName] [nvarchar] (128) NULL ," +
                                   "	[hostName] [nvarchar] (128) NULL ," +
                                   "	[serverName] [nvarchar] (128) NULL ," +
                                   "	[loginName] [nvarchar] (128) NULL ," +
                                   "	[success] [int] NULL ," +
                                   "	[databaseName] [nvarchar] (128) NULL ," +
                                   "	[databaseId] [int] NULL ," +
                                   "	[dbUserName] [nvarchar] (128) NULL ," +
                                   "	[objectType] [int] NULL ," +
                                   "	[objectName] [nvarchar] (512) NULL ," +
                                   "	[objectId] [int] NULL ," +
                                   "	[permissions] [int] NULL ," +
                                   "	[columnPermissions] [int] NULL ," +
                                   "	[targetLoginName] [nvarchar] (128) NULL ," +
                                   "	[targetUserName] [nvarchar] (128) NULL ," +
                                   "	[roleName] [nvarchar] (128) NULL ," +
                                   "	[ownerName] [nvarchar] (128) NULL ," +
                                   "	[targetObject] [nvarchar] (512) NULL ," +  // db + . + owner + . + tablename
                                   "	[details] [nvarchar] (512) NULL ," +
                                   "	[eventCategory] [int] NOT NULL," +
                                   "	[hash] [int] NOT NULL," +
                                   "	[alertLevel] [int] NOT NULL," +
                                   "	[privilegedUser] [int] NOT NULL," +
                                   "	[fileName]         [nvarchar] (128) NULL ," +
                                   "	[linkedServerName] [nvarchar] (128) NULL ," +
                                   "	[parentName]       [nvarchar] (128) NULL ," +
                                   "	[isSystem]         [int] NULL ," +
                                   "	[sessionLoginName] [nvarchar] (128) NULL ," +
                                   "	[providerName]     [nvarchar] (128) NULL, " +
                                   "    [appNameId] [int] NULL, " +
                                   "    [hostId] [int] NULL, " +
                                   "    [loginId] [int] NULL, " +
                                   "    [endTime] [datetime] NULL, " +
                                   "    [startSequence] [bigint] NULL, " +
                                   "    [endSequence] [bigint] NULL, " +
                                   "    [rowCounts] [bigint] NULL, " +
                                   "    [guid] [nvarchar] (100) NULL" +
                                   ")";
                string createSql = String.Format(createTmp,
                                                  SQLHelpers.CreateSafeDatabaseName(databaseName),
                                                  (archiveDatabase) ? "" : "UNIQUE");

                using (SqlCommand sqlcmd = new SqlCommand(createSql, conn))
                {
                    sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    sqlcmd.ExecuteNonQuery();
                }


                //add index -no need to be UNIQUE on archive since we just bulk copy in
                //string indexSql = String.Format("CREATE {1} INDEX [IX_Events_EventId] " +
                //                        "ON {0}..[Events]([eventId]);",
                //                        SQLHelpers.CreateSafeDatabaseName(databaseName),
                //                        (archiveDatabase) ? "" : "UNIQUE");
                //using (SqlCommand sqlcmd = new SqlCommand(indexSql, conn))
                //{
                //    sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                //    sqlcmd.ExecuteNonQuery();
                //}
                // add index
                //BuildIndexes(conn, databaseName, archiveDatabase) ;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("CreateEventsTable",
                                         String.Format("An error occurred creating the events table for {0}", databaseName),
                                         ex);
                throw ex;
            }
        }

        //-----------------------------------------------------------------------		
        // CreateSQLTable
        //
        // Note: If this changes, you must change the script in setup as well
        //-----------------------------------------------------------------------		
        static internal void
             CreateSQLTable(
                string databaseName,
              SqlConnection conn
             )
        {
            try
            {
                string createTmp = "CREATE TABLE {0}..[EventSQL] (" +
                                   "[eventId] [int] NOT NULL" +
                                   ",[startTime] [datetime] NOT NULL" +
                                    ",[sqlText] [nvarchar](MAX) NOT NULL" +
                                    ",[hash] [int] NOT NULL DEFAULT (0)" +
                                    ");";
                string createSql = String.Format(createTmp,
                                                 SQLHelpers.CreateSafeDatabaseName(databaseName));

                using (SqlCommand sqlcmd = new SqlCommand(createSql, conn))
                {
                    sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    sqlcmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("CreateSQLTable",
                                         String.Format("An error occurred creating the eventsSQL table for {0}", databaseName),
                                         ex);
                throw ex;
            }
        }

        //---------------------------------------------------------------
        // CreateDescriptionTable
        //---------------------------------------------------------------
        private static void
           CreateDescriptionTable(
                string instance,
                string databaseName,
              SqlConnection conn
           )
        {
            string sql = "";

            try
            {
                sql = String.Format("CREATE TABLE {0}.dbo.{1} ( " +
                                     "[instance] [nvarchar] (256)," +
                                     "[databaseType] [nvarchar] (16) NULL, " +
                                     "[state] [int] NULL, " +
                                     "[sqlComplianceDbSchemaVersion] [int] NULL, " +
                                     "[eventDbSchemaVersion] [int] NULL ) ",
                                     SQLHelpers.CreateSafeDatabaseName(databaseName),
                                     CoreConstants.RepositoryArchiveMetaTable);


                using (SqlCommand sqlcmd = new SqlCommand(sql, conn))
                {
                    sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    sqlcmd.ExecuteNonQuery();
                }

                sql = String.Format("INSERT INTO {0}.dbo.{1} " +
                                     "(instance,databaseType,sqlComplianceDbSchemaVersion,eventDbSchemaVersion) " +
                                     "VALUES ({2},'{3}',{4},{5})",
                                     SQLHelpers.CreateSafeDatabaseName(databaseName),
                                     CoreConstants.RepositoryMetaTable,
                                     SQLHelpers.CreateSafeString(instance),
                                     CoreConstants.DatabaseType_Events,
                                     CoreConstants.RepositorySqlComplianceDbSchemaVersion,
                                     CoreConstants.RepositoryEventsDbSchemaVersion);

                using (SqlCommand sqlcmd = new SqlCommand(sql, conn))
                {
                    sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    sqlcmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred creating the Description table for {0}", databaseName),
                                         sql,
                                         ex);
                throw ex;
            }
        }

        //---------------------------------------------------------------
        // CreateStatsTable
        //---------------------------------------------------------------
        static internal void
           CreateStatsTable(
              string databaseName,
              SqlConnection conn
           )
        {
            string sql = "";

            try
            {
                StatsDAL.CreateStatsTable(conn, databaseName);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred creating the stats table for {0}", databaseName),
                                         sql,
                                         ex);
                throw;
            }
        }

        static internal void
           CreateIdLookUpTables(string databaseName,
                                 SqlConnection conn)
        {
            CreateIdLookUpTable(databaseName, conn, CoreConstants.RepositoryApplicationsTable);
            CreateIdLookUpTable(databaseName, conn, CoreConstants.RepositoryHostsTable);
            CreateIdLookUpTable(databaseName, conn, CoreConstants.RepositoryLoginsTable);
            CreateIdLookUpTable(databaseName, conn, CoreConstants.RepositoryTablesTable);
            CreateIdLookUpTable(databaseName, conn, CoreConstants.RepositoryColumnsTable);
        }

        static internal void
          CreateIdLookUpTable(
             string databaseName,
             SqlConnection conn,
             string tableName
          )
        {
            string sDb = conn.Database;
            try
            {
                string createSql = String.Format("IF NOT EXISTS( SELECT name FROM sysobjects WHERE name = '{0}' AND xtype = 'U') " +
                                                  "CREATE TABLE [{0}] ( {1}" +
                                                  "[name] [nvarchar] (128) NOT NULL" +
                                                  ",[id] [int] NOT NULL )",
                                                  tableName,
                                                  tableName == CoreConstants.RepositoryTablesTable ? "[schemaName] [nvarchar] (128) NOT NULL," : "");

                conn.ChangeDatabase(databaseName);
                using (SqlCommand sqlcmd = new SqlCommand(createSql, conn))
                {
                    sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    sqlcmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("CreateSQLTable",
                                         String.Format("An error occurred creating the {0} table for {1}", tableName, databaseName),
                                         ex);
                throw ex;
            }
            finally
            {
                conn.ChangeDatabase(sDb);
            }
        }

        static internal void CreateDataChangesTable(string databaseName, SqlConnection conn, bool archiveDatabase)
        {
            try
            {
                string createTmp = "CREATE TABLE {0}..[DataChanges] (" +
                            " [startTime] [datetime] NOT NULL," +
                            " [eventSequence] [bigint] NOT NULL," +
                            " [spid] [int] NOT NULL," +
                            " [databaseId] [int] NOT NULL," +
                            " [actionType] [int] NOT NULL," +
                            " [schemaName] [nvarchar](128) NOT NULL," +
                            " [tableName] [nvarchar](128) NOT NULL," +
                            " [recordNumber] [int] NOT NULL," +
                            " [userName] [nvarchar](128) NOT NULL," +
                            " [changedColumns] [int] NOT NULL," +
                            " [primaryKey] [nvarchar](4000)," +
                            " [hashcode] [int] NOT NULL ," +
                            " [tableId] [int] null," +
                            " [dcId] [bigint] {1} NULL," +
                            " [eventId] [int] NULL," +
                            " [totalChanges] [int] NULL," +
                            " [guid] [nvarchar] (100) NULL" +
                            ")";
                string createSql = String.Format(createTmp, SQLHelpers.CreateSafeDatabaseName(databaseName),
                   archiveDatabase ? "" : "IDENTITY (-9223372036854775807, 1) NOT "); // SQLCM - 6327 Upated to bigint(-9223372036854775808) from int(-2100000000) in fresh installation.

                using (SqlCommand sqlcmd = new SqlCommand(createSql, conn))
                {
                    sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    sqlcmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("CreateDataChangesTable",
                                         String.Format("An error occurred creating the DataChanges table for {0}", databaseName),
                                         ex);
                throw ex;
            }
        }

        static internal void CreateColumnChangesTable(string databaseName, SqlConnection conn, bool archiveDatabase)
        {
            try
            {
                string size = "4000";
                if (SQLHelpers.GetSqlVersion(conn) >= 9)
                    size = "max";
                string createTmp = "CREATE TABLE {0}..[ColumnChanges] (" +
                            " [startTime] [datetime] NOT NULL," +
                            " [eventSequence] [bigint] NOT NULL," +
                            " [spid] [int] NOT NULL," +
                                   " [columnName] [nvarchar](128)not null," +
                                   " [beforeValue] nvarchar({1})," +
                                   " [afterValue] nvarchar({1})," +
                                   " [hashcode] int not null, " +
                            " [columnId] int null, " +
                            " [dcId] [bigint] NULL, " +
                            " [ccId] [bigint] {2} NULL" +
                            ")";
                string createSql = String.Format(createTmp,
                                                 SQLHelpers.CreateSafeDatabaseName(databaseName),
                                                 size,
                                                 archiveDatabase ? "" : "IDENTITY (-9223372036854775807, 1) NOT "); // SQLCM - 6327 Upated to bigint(-9223372036854775808) from int(-2100000000) in fresh installation.

                using (SqlCommand sqlcmd = new SqlCommand(createSql, conn))
                {
                    sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    sqlcmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("CreateColumnChangesTable",
                                         String.Format("An error occurred creating the ColumnChanges table for {0}", databaseName),
                                         ex);
                throw ex;
            }
        }

        static internal void CreateDataChangeLinkProcs(string databaseName, SqlConnection conn)
        {
            string originalDb = conn.Database;
            try
            {
                conn.ChangeDatabase(databaseName);
                string dropProcs = "if exists (select * from dbo.sysobjects where name = N'p_LinkDataChangeRecords' and xtype='P') " +
                   "drop procedure [p_LinkDataChangeRecords]; " +
                   "if exists (select * from dbo.sysobjects where name = N'p_LinkAllDataChangeRecords' and xtype='P') " +
                   "drop procedure [p_LinkAllDataChangeRecords]; ";
                string createProc1 = "CREATE PROCEDURE p_LinkDataChangeRecords(@startTime DateTime, @endTime DateTime) AS BEGIN " +
                   "UPDATE t1 set t1.eventId=t2.eventId FROM DataChanges t1 INNER JOIN " +
                   "(SELECT * from Events where eventCategory=4 AND startTime >= @startTime AND startTime <= @endTime) t2 " +
                   "ON (t1.startTime >= t2.startTime AND t1.startTime <= t2.endTime AND " +
                   "t1.eventSequence >= t2.startSequence AND t1.eventSequence <= t2.endSequence AND t1.spid = t2.spid) " +
                   "WHERE t1.eventId IS NULL AND t1.startTime >= @startTime AND t1.startTime <= @endTime " +
                   "UPDATE t1 set t1.dcId=t2.dcId FROM ColumnChanges t1 INNER JOIN DataChanges t2 " +
                   "ON (t1.startTime = t2.startTime AND t1.eventSequence = t2.eventSequence AND t1.spid = t2.spid) " +
                   "WHERE t1.dcId IS NULL AND t1.startTime >= @startTime AND t1.startTime <= @endTime END ";
                string createProc2 = "CREATE PROCEDURE p_LinkAllDataChangeRecords AS BEGIN " +
                   "UPDATE t1 set t1.eventId=t2.eventId FROM DataChanges t1 INNER JOIN " +
                   "(SELECT * from Events where eventCategory=4) t2 " +
                   "ON (t1.startTime >= t2.startTime AND t1.startTime <= t2.endTime AND " +
                   "t1.eventSequence >= t2.startSequence AND t1.eventSequence <= t2.endSequence AND t1.spid = t2.spid) " +
                   "WHERE t1.eventId IS NULL " +
                   "UPDATE t1 set t1.dcId=t2.dcId FROM ColumnChanges t1 INNER JOIN DataChanges t2 " +
                   "ON (t1.startTime = t2.startTime AND t1.eventSequence = t2.eventSequence AND t1.spid = t2.spid) " +
                   "WHERE t1.dcId IS NULL END";

                using (SqlCommand sqlcmd = new SqlCommand(dropProcs, conn))
                {
                    sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    sqlcmd.ExecuteNonQuery();
                }
                using (SqlCommand sqlcmd = new SqlCommand(createProc1, conn))
                {
                    sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    sqlcmd.ExecuteNonQuery();
                }
                using (SqlCommand sqlcmd = new SqlCommand(createProc2, conn))
                {
                    sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    sqlcmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("CreateDataChangeLinkProcs",
                                         String.Format("An error occurred creating the ColumnChanges table for {0}", databaseName),
                                         ex);
                throw ex;
            }
            finally
            {
                conn.ChangeDatabase(originalDb);
            }
        }

        static internal void CreateSensitiveColumnsTable(string databaseName, SqlConnection conn, bool archiveDatabase)
        {
            try
            {
                StringBuilder createSql = new StringBuilder();
                createSql.AppendFormat("CREATE TABLE {0}..[{1}] (", SQLHelpers.CreateSafeDatabaseName(databaseName), CoreConstants.RepositorySensitiveColumnsTable);
                createSql.Append(" [startTime] DATETIME NOT NULL,");
                createSql.Append(" [eventId] [INT] NOT NULL,");
                createSql.Append(" [columnName] [nvarchar](128) NOT NULL,");
                createSql.Append(" [hashcode] [INT] NOT NULL,");
                createSql.Append(" [tableId] [INT] NULL,");
                createSql.Append(" [columnId] [INT] NULL,");
                createSql.AppendFormat(" [scId] [int] {0} NULL)", archiveDatabase ? "" : "IDENTITY (-2100000000, 1) NOT ");

                using (SqlCommand sqlcmd = new SqlCommand(createSql.ToString(), conn))
                {
                    sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    sqlcmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("CreateSensitiveColumnsTable",
                                         String.Format("An error occurred creating the {0} table for {1}", CoreConstants.RepositorySensitiveColumnsTable, databaseName),
                                         ex);
                throw ex;
            }
        }

        static internal void CreateInstanceDatabasesTable(string databaseName, SqlConnection conn)
        {
            try
            {
                StringBuilder createSql = new StringBuilder();
                createSql.AppendFormat("CREATE TABLE {0}..[{1}] (", SQLHelpers.CreateSafeDatabaseName(databaseName), CoreConstants.InstanceDatabasesTable);
                createSql.Append(" [srvId] [int] NOT NULL,");
                createSql.Append(" [databaseName] [nvarchar](128) NOT NULL,");
                createSql.Append(" [dbId] [smallint] NOT NULL)");
                using (SqlCommand sqlcmd = new SqlCommand(createSql.ToString(), conn))
                {
                    sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    sqlcmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("CreateInstanceDatabasesTable",
                                         String.Format("An error occurred creating the {0} table for {1}", CoreConstants.InstanceDatabasesTable, databaseName),
                                         ex);
                throw ex;
            }
        }
        #endregion

        #region Archive in progress flags

        //-----------------------------------------------------------------------		
        // SetArchiveInProgress
        //-----------------------------------------------------------------------		
        static public void SetDatabaseState(string database, EventsDatabaseState state)
        {
            string cmdStr = "";
            Repository rep = new Repository();

            try
            {
                rep.OpenConnection();

                // write record to system databases
                cmdStr = String.Format("UPDATE {0}..{1} SET state={2}",
                                        SQLHelpers.CreateSafeDatabaseName(database),
                                        CoreConstants.RepositoryMetaTable,
                                        (int)state);
                using (SqlCommand cmd = new SqlCommand(cmdStr, rep.connection))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "SetArchiveInProgress",
                                         cmdStr,
                                         ex);
                throw ex;
            }
            finally
            {
                rep.CloseConnection();
            }
        }

        //-----------------------------------------------------------------------		
        // GetArchiveInProgress
        //-----------------------------------------------------------------------		
        static public EventsDatabaseState GetDatabaseState(string database)
        {
            EventsDatabaseState state;
            string cmdStr = "";

            Repository rep = new Repository();

            try
            {
                rep.OpenConnection();

                // write record to system databases
                cmdStr = String.Format("SELECT state from {0}..{1}",
                                        SQLHelpers.CreateSafeDatabaseName(database),
                                        CoreConstants.RepositoryMetaTable);
                using (SqlCommand cmd = new SqlCommand(cmdStr, rep.connection))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    object obj = cmd.ExecuteScalar();
                    if (obj is DBNull)
                        state = EventsDatabaseState.NormalChainIntact;
                    else
                        state = (EventsDatabaseState)obj;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "GetArchiveInProgress",
                                         cmdStr,
                                         ex);
                throw ex;
            }
            finally
            {
                rep.CloseConnection();
            }

            return state;
        }

        #endregion

        #region Low level - Database security

        //-----------------------------------------------------------------------		
        // SetDefaultSecurity
        //
        // Sets up guest login and public role permissions for event databases
        //
        // Note: If this changes, you must change the script in setup as well
        //-----------------------------------------------------------------------		
        static public void
             SetDefaultSecurity(
                string databaseName,
                int defaultAccess,
                int oldDefaultAccess,   // old value or -1 for new
                bool archive,
              SqlConnection conn
             )
        {
            bool useWorked = false;

            try
            {
                UseDatabase(databaseName,
                                           conn);
                useWorked = true;

                // no longer needed in SQLcm 4.5
                /*
               try
               {
                  // add guest login since we have a default access
                  string guestSql = "EXECUTE sp_grantdbaccess 'guest'" ;
                using ( SqlCommand sqlcmd = new SqlCommand( guestSql, conn ) )
                {
                     sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                   sqlcmd.ExecuteNonQuery();
                }
             }
             catch ( SqlException sqlEx )
             {
                // 15023 - guest already exists (ignore)
                if ( sqlEx.Number != 15023 ) throw sqlEx;
             }
                 */

                // set up public access to tables
                // if it was granted before but not now - revoke

                // Events
                string grantSql = "";
                if (defaultAccess != 0)
                {
                    // no longer needed in SQLcm4.5
                    //grantSql = "GRANT SELECT ON [Events] TO [guest]";
                }
                else if (oldDefaultAccess != 0)
                {
                    grantSql = "DENY SELECT ON [Events] TO [guest]";
                }

                if (grantSql != "")
                {
                    using (SqlCommand sqlcmd = new SqlCommand(grantSql, conn))
                    {
                        sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        sqlcmd.ExecuteNonQuery();
                    }
                }

                // EventSQL
                grantSql = "";
                if (defaultAccess == 2)
                {
                    // no longer needed in SQLcm4.5
                    // grantSql = "GRANT SELECT ON [EventSQL] TO [guest]";
                }
                else if (oldDefaultAccess == 2 || oldDefaultAccess == -1)
                {
                    grantSql = "DENY SELECT ON [EventSQL] TO [guest]";
                }

                if (grantSql != "")
                {
                    using (SqlCommand sqlcmd = new SqlCommand(grantSql, conn))
                    {
                        sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        sqlcmd.ExecuteNonQuery();
                    }
                }

                // Stats
                grantSql = "";
                if (defaultAccess != 0)
                {
                    // no longer needed in SQLcm4.5
                    // grantSql = "GRANT SELECT ON [Stats] TO [guest]";
                }
                else if (oldDefaultAccess != 0)
                {
                    grantSql = "DENY SELECT ON [Stats] TO [guest]";
                }

                if (grantSql != "")
                {
                    using (SqlCommand sqlcmd = new SqlCommand(grantSql, conn))
                    {
                        sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        sqlcmd.ExecuteNonQuery();
                    }
                }

                // Applications
                grantSql = "";
                if (defaultAccess != 0)
                {
                    // no longer needed in SQLcm4.5
                    // grantSql = "GRANT SELECT ON [Applications] TO [guest]";
                }
                else if (oldDefaultAccess != 0)
                {
                    grantSql = "DENY SELECT ON [Applications] TO [guest]";
                }

                if (grantSql != "")
                {
                    using (SqlCommand sqlcmd = new SqlCommand(grantSql, conn))
                    {
                        sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        sqlcmd.ExecuteNonQuery();
                    }
                }

                // Hosts
                grantSql = "";
                if (defaultAccess != 0)
                {
                    // no longer needed in SQLcm4.5
                    // grantSql = "GRANT SELECT ON [Hosts] TO [guest]";
                }
                else if (oldDefaultAccess != 0)
                {
                    grantSql = "DENY SELECT ON [Hosts] TO [guest]";
                }

                if (grantSql != "")
                {
                    using (SqlCommand sqlcmd = new SqlCommand(grantSql, conn))
                    {
                        sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        sqlcmd.ExecuteNonQuery();
                    }
                }

                // Logins
                grantSql = "";
                if (defaultAccess != 0)
                {
                    // no longer needed in SQLcm4.5
                    // grantSql = "GRANT SELECT ON [Logins] TO [guest]";
                }
                else if (oldDefaultAccess != 0)
                {
                    grantSql = "DENY SELECT ON [Logins] TO [guest]";
                }

                if (grantSql != "")
                {
                    using (SqlCommand sqlcmd = new SqlCommand(grantSql, conn))
                    {
                        sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        sqlcmd.ExecuteNonQuery();
                    }
                }

                // Tables
                grantSql = "";
                if (defaultAccess != 0)
                {
                    // no longer needed in SQLcm4.5
                    // grantSql = "GRANT SELECT ON [Tables] TO [guest]";
                }
                else if (oldDefaultAccess != 0)
                {
                    grantSql = "DENY SELECT ON [Tables] TO [guest]";
                }

                if (grantSql != "")
                {
                    using (SqlCommand sqlcmd = new SqlCommand(grantSql, conn))
                    {
                        sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        sqlcmd.ExecuteNonQuery();
                    }
                }

                // Columns
                grantSql = "";
                if (defaultAccess != 0)
                {
                    // no longer needed in SQLcm4.5
                    // grantSql = "GRANT SELECT ON [Columns] TO [guest]";
                }
                else if (oldDefaultAccess != 0)
                {
                    grantSql = "DENY SELECT ON [Columns] TO [guest]";
                }

                if (grantSql != "")
                {
                    using (SqlCommand sqlcmd = new SqlCommand(grantSql, conn))
                    {
                        sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        sqlcmd.ExecuteNonQuery();
                    }
                }

                // DataChanges
                grantSql = "";
                if (defaultAccess != 0)
                {
                    // no longer needed in SQLcm4.5
                    // grantSql = "GRANT SELECT ON [DataChanges] TO [guest]";
                }
                else if (oldDefaultAccess != 0)
                {
                    grantSql = "DENY SELECT ON [DataChanges] TO [guest]";
                }

                if (grantSql != "")
                {
                    using (SqlCommand sqlcmd = new SqlCommand(grantSql, conn))
                    {
                        sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        sqlcmd.ExecuteNonQuery();
                    }
                }
                // ColumnChanges
                grantSql = "";
                if (defaultAccess != 0)
                {
                    // no longer needed in SQLcm4.5
                    // grantSql = "GRANT SELECT ON [ColumnChanges] TO [guest]";
                }
                else if (oldDefaultAccess != 0)
                {
                    grantSql = "DENY SELECT ON [ColumnChanges] TO [guest]";
                }

                if (grantSql != "")
                {
                    using (SqlCommand sqlcmd = new SqlCommand(grantSql, conn))
                    {
                        sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        sqlcmd.ExecuteNonQuery();
                    }
                }

                // SensitiveColumns
                grantSql = "";
                if (defaultAccess != 0)
                {
                    // no longer needed in SQLcm4.5
                    // grantSql = "GRANT SELECT ON [SensitiveColumns] TO [guest]";
                }
                else if (oldDefaultAccess != 0)
                {
                    grantSql = "DENY SELECT ON [SensitiveColumns] TO [guest]";
                }

                if (grantSql != "")
                {
                    using (SqlCommand sqlcmd = new SqlCommand(grantSql, conn))
                    {
                        sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        sqlcmd.ExecuteNonQuery();
                    }
                }

                // no longer needed in SQLcm4.5
                /*
                // Description
                grantSql = "GRANT SELECT ON [Description] TO [guest]";
                using ( SqlCommand sqlcmd = new SqlCommand( grantSql, conn ) )
                {
                     sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                   sqlcmd.ExecuteNonQuery();
                }

                grantSql = "GRANT SELECT ON [Description] TO [public]";
                using ( SqlCommand sqlcmd = new SqlCommand( grantSql, conn ) )
                {
                     sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                   sqlcmd.ExecuteNonQuery();
                }

                // change log               
                if ( archive && oldDefaultAccess!= 1)
                {
                   grantSql = "GRANT SELECT ON [ChangeLog] TO [guest]";
                   using ( SqlCommand sqlcmd = new SqlCommand( grantSql, conn ) )
                   {
                        sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                      sqlcmd.ExecuteNonQuery();
                   }
                }
                */
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "SetDefaultSecurity",
                                         String.Format("An error occurred setting the security for {0}", databaseName),
                                         ex);
                throw;
            }
            finally
            {
                if (useWorked)
                {
                    UseDatabase(CoreConstants.RepositoryDatabase,
                                               conn);
                }
            }
        }

        static public void
           UseDatabase(
              string databaseName,
              SqlConnection conn
           )
        {
            string str = String.Format("USE {0};",
                                        SQLHelpers.CreateSafeDatabaseName(databaseName));
            using (SqlCommand cmd = new SqlCommand(str, conn))
            {
                cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        //----------------------------------------------------------------------
        // GetLoginNames
        //----------------------------------------------------------------------
        public List<string>
          GetLoginNames(
          SqlConnection conn
            )
        {
            List<string> loginNames = new List<string>();

            try
            {
                string selectCmd = "SELECT distinct([Name]) from [SQLcompliance]..[LoginAccounts]";
                if (conn != null)
                {
                    using (SqlCommand cmd = new SqlCommand(selectCmd, conn))
                    {
                        cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                string name = SQLHelpers.GetString(reader, 0);
                                loginNames.Add(name);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = String.Format("Error reading login names information.\n\nError: {0}",
                   ex.Message);
                throw new Exception(msg);
            }

            return loginNames;
        }

        //
        //GetAccessType
        //
        public List<string>
          GetAccessType(
          SqlConnection conn, string userName, string databaseName
            )
        {
            List<string> loginNames = new List<string>();
            string connectionDatabase = string.Empty;
            try
            {
                string selectCmd = String.Format("USE {0}; select OBJECT_NAME(major_id) " +
                                    "from sys.database_permissions P " +
                                    "JOIN sys.tables T ON P.major_id = T.object_id " +
                                    "JOIN sysusers U ON U.uid = P.grantee_principal_id " +
                                    "where U.name ='{1}' and state_desc='GRANT'", SQLHelpers.CreateSafeDatabaseName(databaseName), userName);
                if (conn != null)
                {
                    connectionDatabase = conn.Database;
                    using (SqlCommand cmd = new SqlCommand(selectCmd, conn))
                    {
                        cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    string name = SQLHelpers.GetString(reader, 0);
                                    loginNames.Add(name);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = String.Format("Error reading login names information.\n\nError: {0}",
                   ex.Message);
                throw new Exception(msg);
            }
            finally
            {
                if (conn != null)
                {
                    conn.ChangeDatabase(connectionDatabase);
                }
            }

            return loginNames;
        }

        //----------------------------------------------------------------------
        // defaulAccess
        //----------------------------------------------------------------------
        public int
          GetDefaultAccess(
          SqlConnection conn,
            String instance
            )
        {
            int defaultAccess = 0;

            try
            {
                string selectCmd = String.Format("SELECT defaultAccess from [SQLcompliance].[dbo].[Servers] where instance = '{0}'", instance);
                if (conn != null && !string.IsNullOrEmpty(instance))
                {
                    using (SqlCommand cmd = new SqlCommand(selectCmd, conn))
                    {
                        cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                defaultAccess = SQLHelpers.GetInt32(reader, 0);
                            }
                            else
                            {
                                throw new Exception("Missing Login table.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = String.Format("Error reading login names information.\n\nError: {0}",
                   ex.Message);
                throw new Exception(msg);
            }

            return defaultAccess;
        }

        public bool IsAdded(string userName, string databaseName, SqlConnection conn)
        {
            string sql;
            bool result = false;
            int added = 0;
            try
            {
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(databaseName) && conn != null)
                {
                    sql = String.Format("use {0};SELECT count(name) from sys.sysusers where name ={1}",
                                         SQLHelpers.CreateSafeDatabaseName(databaseName),
                                         SQLHelpers.CreateSafeString(userName));
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                added = SQLHelpers.GetInt32(reader, 0);
                            }
                        }
                    }
                    if (added != 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool IsOwner(string userName, string databaseName, SqlConnection conn)
        {
            string sql;
            bool result = false;
            string user = userName;
            string name = "";
            try
            {
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(databaseName) && conn != null)
                {
                    sql = String.Format("select suser_sname(owner_sid) from sys.databases where name ='{0}'",
                                         databaseName);
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                name = SQLHelpers.GetString(reader, 0);
                            }
                        }
                    }
                    if (name != "" && string.Equals(name, user, StringComparison.OrdinalIgnoreCase))
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }



        public void AddUser(string dbName, string userName, SqlConnection conn)
        {
            try
            {
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(dbName) && conn != null)
                {
                    string createSql = String.Format("USE {0}; CREATE USER {1}",
                                                      SQLHelpers.CreateSafeDatabaseName(dbName),
                                                      SQLHelpers.CreateSafeDatabaseName(userName));
                    using (SqlCommand sqlcmd = new SqlCommand(createSql, conn))
                    {
                        sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        sqlcmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "add user",
                                         String.Format("An error occurred adding {1} in {0}", dbName, userName),
                                         ex);
                return;
            }

        }


        public void AddTablePermissions(
          string userName,
          string databaseName,
          string tableName,
          SqlConnection conn
       )
        {
            string sql;

            try
            {
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(databaseName) && !string.IsNullOrEmpty(tableName) && conn != null)
                {
                    sql = String.Format("use {0};GRANT SELECT ON {1} TO [{2}]",
                                         SQLHelpers.CreateSafeDatabaseName(databaseName),
                                         tableName,
                                         userName);
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //-------------------------------------------------------------
        // LoadInstances
        //--------------------------------------------------------------
        public List<string> LoadInstances(SqlConnection conn)
        {

            List<string> listInstances = new List<string>();

            try
            {
                if (conn != null)
                {
                    string cmdstr = String.Format("SELECT distinct(instance) " +
                                                         "FROM {0} " +
                                                         "WHERE databaseType!='System' ",
                                                   CoreConstants.RepositorySystemDatabaseTable);

                    using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                    {
                        cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string instanceName = SQLHelpers.GetString(reader, 0);
                                listInstances.Add(instanceName);
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listInstances;
        }

        //-------------------------------------------------------------
        // LoadDatabases
        //--------------------------------------------------------------
        public List<string> LoadDatabases(SqlConnection conn, string instance)
        {

            List<string> listDatabases = new List<string>();


            try
            {
                if (conn != null)
                {
                    string cmdstr = String.Format("USE SQLcompliance; SELECT databaseName, instance " +
                                                         "FROM {0} " +
                                                         "WHERE instance = '{1}' and databaseType!='System' " +
                                                         "ORDER by databaseName ASC",
                                                   CoreConstants.RepositorySystemDatabaseTable, instance);

                    using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                    {
                        cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string databaseName = SQLHelpers.GetString(reader, 0);
                                listDatabases.Add(databaseName);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return listDatabases;
        }
        /// <summary>
        /// Get Default Access
        /// </summary>

        public int GetDefaultAccess(string instance, SqlConnection conn)
        {
            int defaultAccess = 0;

            try
            {
                if (conn != null && !string.IsNullOrEmpty(instance))
                {
                    string selectCmd = String.Format("SELECT defaultAccess from [SQLcompliance].[dbo].[Servers] where instance = {0}", "'" + instance + "'");

                    using (SqlCommand cmd = new SqlCommand(selectCmd, conn))
                    {
                        cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                defaultAccess = SQLHelpers.GetInt32(reader, 0);
                            }
                            else
                            {
                                throw new Exception("Missing defaultAccess.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = String.Format("Error reading default access information.\n\nError: {0}",
                   ex.Message);
                throw new Exception(msg);
            }

            return defaultAccess;
        }

        public void ApplyAccess(int defaultAccess, string name, string dbName, SqlConnection conn, string instanceDbName = null, bool archive = false)
        {
            string connectionDatabase = string.Empty;
            if (conn != null)
            {
                connectionDatabase = conn.Database;
            }
            try
            {
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(dbName) && conn != null)
                {
                    if (defaultAccess != 0 || archive)
                    {
                        if (EventDatabase.DatabaseExists(dbName, conn))
                        {
                            if (!IsAdded(name, dbName, conn))
                            {
                                if (!IsOwner(name, dbName, conn))
                                {
                                    if (instanceDbName == null || IsAdded(name, instanceDbName, conn))
                                    {
                                        AddUser(dbName, name, conn);

                                        ApplyDataReaderRole(dbName, name, conn);
                                        if (defaultAccess != 0)
                                        {
                                            AddTablePermissions(name, dbName, "Events", conn);
                                            //BAD data
                                            AddTablePermissions(name, dbName, "DataChanges", conn);
                                            AddTablePermissions(name, dbName, "ColumnChanges", conn);
                                            //BAD filters
                                            AddTablePermissions(name, dbName, "Columns", conn);
                                            AddTablePermissions(name, dbName, "Tables", conn);
                                            //event view filters
                                            AddTablePermissions(name, dbName, "Applications", conn);
                                            AddTablePermissions(name, dbName, "Logins", conn);
                                            AddTablePermissions(name, dbName, "Hosts", conn);
                                            //Server and DB Summary charts.
                                            AddTablePermissions(name, dbName, "Stats", conn);
                                        }
                                        if (defaultAccess == 2)
                                        {
                                            AddTablePermissions(name, dbName, "EventSQL", conn);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.ChangeDatabase(connectionDatabase);
                }
            }
        }

        public static void UpdateManuallyDeployed(string instanceName, string value, SqlConnection conn)
        {
            try
            {

                string createSql = String.Format("USE {0}; Update {1} set isDeployedManually= '{2}' where instance='{3}' AND isDeployedManually ='1'",
                                                      CoreConstants.RepositoryDatabase,
                                                      CoreConstants.RepositoryServerTable,
                                                      value,
                                                      instanceName);
                using (SqlCommand sqlcmd = new SqlCommand(createSql, conn))
                {
                    sqlcmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    sqlcmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "update server",
                                         String.Format("An error occurred"),
                                         ex);
                return;
            }

        }

        public void ApplyDataReaderRole(string dbName, string userName, SqlConnection conn)
        {
            try
            {
                if (dbName != null && userName != null)
                {
                    string createSql = String.Format("USE {0}; ALTER ROLE db_datareader ADD MEMBER {1} ",
                                                          SQLHelpers.CreateSafeDatabaseName(dbName),
                                                          SQLHelpers.CreateSafeDatabaseName(userName));

                    using (SqlCommand cmd = new SqlCommand(createSql, conn))
                    {
                        cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "add datareader role",
                                         String.Format("An error occurred adding {0} in {1}", dbName, userName),
                                         ex);
                return;
            }

        }



    }
}
