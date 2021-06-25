using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SQLcomplianceIndexUpgrade
{
    class RawSQL
    {
        static public int BuildIndexes(SqlConnection conn, int queryTimeoutDuration)
        {
            try
            {
                StringBuilder queryBuilder = new StringBuilder("");
                queryBuilder.Append("DECLARE @event_database nvarchar(max) ");
                queryBuilder.Append("DECLARE @strSQL nvarchar(max) ");
                queryBuilder.Append("DECLARE eventdb_cursor CURSOR FOR SELECT databaseName from[SQLcompliance]..[SystemDatabases] where databaseType = 'Event' ");
                queryBuilder.Append("OPEN eventdb_cursor ");
                queryBuilder.Append("FETCH NEXT FROM eventdb_cursor ");
                queryBuilder.Append("INTO @event_database ");
                queryBuilder.Append("WHILE @@FETCH_STATUS = 0 ");
                queryBuilder.Append("BEGIN ");
                queryBuilder.Append("Set @strSQL = ('USE ' + '[' + @event_database + '] ");

                /// Removed Code Part for SQLCM-6335
                //queryBuilder.Append("IF EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_eventId'') BEGIN ");
                //queryBuilder.Append("IF OBJECT_ID(''ChangeLog'') IS NULL CREATE UNIQUE CLUSTERED INDEX [IX_Events_eventId] ON [dbo].[Events]([eventId] ASC ) WITH (DROP_EXISTING = ON) ON [PRIMARY]; ");
                //queryBuilder.Append("ELSE CREATE CLUSTERED INDEX [IX_Events_eventId] ON [dbo].[Events]([eventId] ASC ) WITH (DROP_EXISTING = ON) ON [PRIMARY]; END ");
                //queryBuilder.Append("ELSE BEGIN IF OBJECT_ID(''ChangeLog'') IS NULL CREATE UNIQUE CLUSTERED INDEX [IX_Events_eventId] ON [dbo].[Events]([eventId] ASC ) ON [PRIMARY]; ");
                //queryBuilder.Append("ELSE CREATE CLUSTERED INDEX [IX_Events_eventId] ON [dbo].[Events]([eventId] ASC ) ON [PRIMARY]; END ");
                /// Removed Code Part for SQLCM-6335

                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_eventCategory'') CREATE INDEX [IX_Events_eventCategory] ON ['+@event_database+']..[Events]([eventCategory], [eventId] DESC ) ON [PRIMARY];");
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_eventType'') CREATE INDEX [IX_Events_eventType] ON ['+@event_database+']..[Events]([eventType], [eventId] DESC ) ON [PRIMARY]; ");
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_databaseId'') CREATE INDEX [IX_Events_databaseId] ON ['+@event_database+']..[Events]([databaseId], [eventId] DESC ) ON [PRIMARY]; ");
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_hostId'') CREATE  INDEX [IX_Events_hostId]  ON ['+@event_database+']..[Events]([hostId], [eventId] DESC ) ON [PRIMARY]; ");
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_loginId'') CREATE  INDEX [IX_Events_loginId]  ON ['+@event_database+']..[Events]([loginId], [eventId] DESC ) ON [PRIMARY]; ");
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_appNameId'') CREATE  INDEX [IX_Events_appNameId]  ON ['+@event_database+']..[Events]([appNameId], [eventId] DESC ) ON [PRIMARY]; ')");
                queryBuilder.Append("EXEC sp_executesql @strSQL; ");
                queryBuilder.Append("FETCH NEXT FROM eventdb_cursor INTO @event_database ");
                queryBuilder.Append("END; ");
                queryBuilder.Append("CLOSE eventdb_cursor; ");
                queryBuilder.Append("DEALLOCATE eventdb_cursor; ");
                string query = queryBuilder.ToString();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandTimeout = queryTimeoutDuration;
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Logger.Info("facing issue while updating index" + ex.Message);
                throw ex;
            }
        }

        static public int BuildOnlineIndexes(SqlConnection conn, int queryTimeoutDuration)
        {
            try
            {
                StringBuilder queryBuilder = new StringBuilder("");
                queryBuilder.Append("DECLARE @event_database nvarchar(max) ");
                queryBuilder.Append("DECLARE @strSQL nvarchar(max) ");
                queryBuilder.Append("DECLARE eventdb_cursor CURSOR FOR SELECT databaseName from[SQLcompliance]..[SystemDatabases] where databaseType = 'Event' ");
                queryBuilder.Append("OPEN eventdb_cursor ");
                queryBuilder.Append("FETCH NEXT FROM eventdb_cursor ");
                queryBuilder.Append("INTO @event_database ");
                queryBuilder.Append("WHILE @@FETCH_STATUS = 0 ");
                queryBuilder.Append("BEGIN ");
                queryBuilder.Append("Set @strSQL = ('USE ' + '[' + @event_database + '] ");

                /// Removed Code Part for SQLCM-6335
                //queryBuilder.Append("IF EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_eventId'') BEGIN ");
                //queryBuilder.Append("IF OBJECT_ID(''ChangeLog'') IS NULL CREATE UNIQUE CLUSTERED INDEX [IX_Events_eventId] ON [dbo].[Events]([eventId] ASC ) WITH (DROP_EXISTING = ON, ONLINE = ON) ON [PRIMARY]; ");
                //queryBuilder.Append("ELSE CREATE CLUSTERED INDEX [IX_Events_eventId] ON [dbo].[Events]([eventId] ASC ) WITH (DROP_EXISTING = ON, ONLINE = ON) ON [PRIMARY]; END ");
                //queryBuilder.Append("ELSE BEGIN IF OBJECT_ID(''ChangeLog'') IS NULL CREATE UNIQUE CLUSTERED INDEX [IX_Events_eventId] ON [dbo].[Events]([eventId] ASC ) WITH (ONLINE=ON) ON [PRIMARY]; ");
                //queryBuilder.Append("ELSE CREATE CLUSTERED INDEX [IX_Events_eventId] ON [dbo].[Events]([eventId] ASC ) WITH (ONLINE=ON) ON [PRIMARY]; END ");
                /// Removed Code Part for SQLCM-6335

                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_eventCategory'') CREATE INDEX [IX_Events_eventCategory] ON ['+@event_database+']..[Events]([eventCategory], [eventId] DESC ) WITH (ONLINE=ON)  ON [PRIMARY];");
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_eventType'') CREATE INDEX [IX_Events_eventType] ON ['+@event_database+']..[Events]([eventType], [eventId] DESC ) WITH (ONLINE=ON)  ON [PRIMARY]; ");
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_databaseId'') CREATE INDEX [IX_Events_databaseId] ON ['+@event_database+']..[Events]([databaseId], [eventId] DESC ) WITH (ONLINE=ON)  ON [PRIMARY]; ");
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_hostId'') CREATE  INDEX [IX_Events_hostId]  ON ['+@event_database+']..[Events]([hostId], [eventId] DESC ) WITH (ONLINE=ON)  ON [PRIMARY]; ");
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_loginId'') CREATE  INDEX [IX_Events_loginId]  ON ['+@event_database+']..[Events]([loginId], [eventId] DESC ) WITH (ONLINE=ON)  ON [PRIMARY]; ");
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_appNameId'') CREATE  INDEX [IX_Events_appNameId]  ON ['+@event_database+']..[Events]([appNameId], [eventId] DESC ) WITH (ONLINE=ON)  ON [PRIMARY];') ");
                queryBuilder.Append("EXEC sp_executesql @strSQL; ");
                queryBuilder.Append("FETCH NEXT FROM eventdb_cursor INTO @event_database ");
                queryBuilder.Append("END; ");
                queryBuilder.Append("CLOSE eventdb_cursor; ");
                queryBuilder.Append("DEALLOCATE eventdb_cursor; ");
                string query = queryBuilder.ToString();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandTimeout = queryTimeoutDuration;
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static public int CheckIndexes(SqlConnection conn, bool isUpgrade, int queryTimeoutDuration)
        {
            try
            {
                StringBuilder queryBuilder = new StringBuilder("");
                queryBuilder.Append("DECLARE @event_database nvarchar(max) ");
                queryBuilder.Append("DECLARE @strSQL nvarchar(2000) ");
                queryBuilder.Append("DECLARE @indexCount int = 0 ");
                queryBuilder.Append("DECLARE eventdb_cursor CURSOR FOR SELECT databaseName from[SQLcompliance]..[SystemDatabases] where databaseType = 'Event' ");
                queryBuilder.Append("OPEN eventdb_cursor ");
                queryBuilder.Append("FETCH NEXT FROM eventdb_cursor ");
                queryBuilder.Append("INTO @event_database ");
                queryBuilder.Append("WHILE @@FETCH_STATUS = 0 ");
                queryBuilder.Append("BEGIN ");
                queryBuilder.Append("BEGIN ");
                queryBuilder.Append("DECLARE @count int = 0; ");
                if (isUpgrade)
                    queryBuilder.Append("set @strSQL = 'USE ' + '[' + @event_database + '] SELECT  @count = Count(*) FROM sys.tables AS t INNER JOIN sys.indexes AS i ON t.object_id = i.object_id INNER JOIN sys.partitions AS p ON i.object_id = p.object_id AND i.index_id = p.index_id Where i.name in (''IX_Events_eventId'',''IX_Events_eventType'',''IX_Events_databaseId'',''IX_Events_appNameId'',''IX_Events_hostId'',''IX_Events_loginId'',''IX_Events_eventCategory'') and p.data_compression_desc = ''None'' and t.name = ''Events''' ");
                else
                    queryBuilder.Append("set @strSQL = 'USE ' + '[' + @event_database + '] SELECT  @count = Count(*) FROM sys.tables AS t INNER JOIN sys.indexes AS i ON t.object_id = i.object_id INNER JOIN sys.partitions AS p ON i.object_id = p.object_id AND i.index_id = p.index_id Where i.name in (''IX_Events_eventId'',''IX_Events_eventType'',''IX_Events_databaseId'',''IX_Events_appNameId'',''IX_Events_hostId'',''IX_Events_loginId'',''IX_Events_eventCategory'') and p.data_compression_desc = ''Page'' and t.name = ''Events''' ");
                queryBuilder.Append("EXEC sp_executesql @strSQL, N'@count varchar(max) output', @count output; ");
                queryBuilder.Append("SET @indexCount = @indexCount + @count; ");
                queryBuilder.Append("END ");
                queryBuilder.Append("FETCH NEXT FROM eventdb_cursor INTO @event_database ");
                queryBuilder.Append("END; ");
                queryBuilder.Append("Select @indexCount; ");
                queryBuilder.Append("CLOSE eventdb_cursor; ");
                queryBuilder.Append("DEALLOCATE eventdb_cursor; ");
                string query = queryBuilder.ToString();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    //cmd.CommandTimeout = 3600;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static public void UpgradeIndexesOnline(SqlConnection conn, int queryTimeoutDuration)
        {
            using (SqlTransaction transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    StringBuilder queryBuilder = new StringBuilder("");
                    queryBuilder.Append("DECLARE @event_database nvarchar(max) ");
                    queryBuilder.Append("DECLARE @indexname nvarchar(100) ");
                    queryBuilder.Append("DECLARE @data_compression_desc nvarchar(10) ");
                    queryBuilder.Append("declare @strSQL nvarchar(2000) ");
                    queryBuilder.Append("DECLARE eventdb_cursor CURSOR FOR SELECT databaseName from[SQLcompliance]..[SystemDatabases] where databaseType = 'Event' ");
                    queryBuilder.Append("OPEN eventdb_cursor ");
                    queryBuilder.Append("FETCH NEXT FROM eventdb_cursor ");
                    queryBuilder.Append("INTO @event_database ");
                    queryBuilder.Append("WHILE @@FETCH_STATUS = 0 ");
                    queryBuilder.Append("BEGIN ");
                    queryBuilder.Append("CREATE TABLE #Index_Temp ( indexname nvarchar(50), data_compression_desc nvarchar(10)) ");
                    queryBuilder.Append("set @strSQL = 'USE ' + '[' + @event_database + '] INSERT INTO #Index_Temp SELECT i.name AS indexname, p.data_compression_desc FROM sys.tables AS t INNER JOIN sys.indexes AS i ON t.object_id = i.object_id INNER JOIN sys.partitions AS p ON i.object_id = p.object_id AND i.index_id = p.index_id Where i.name in (''IX_Events_eventId'',''IX_Events_eventType'',''IX_Events_databaseId'',''IX_Events_appNameId'',''IX_Events_hostId'',''IX_Events_loginId'',''IX_Events_eventCategory'') and p.data_compression_desc = ''None'' and t.name = ''Events''' ");
                    queryBuilder.Append("EXEC sp_executesql @strSQL ");
                    queryBuilder.Append("DECLARE index_cursor CURSOR FOR Select indexname,data_compression_desc from #Index_Temp ");
                    queryBuilder.Append("OPEN index_cursor FETCH NEXT FROM index_cursor INTO @indexname, @data_compression_desc ");
                    queryBuilder.Append("WHILE @@FETCH_STATUS = 0 ");
                    queryBuilder.Append("BEGIN ");
                    queryBuilder.Append("EXEC ('ALTER​ ​INDEX​ ['+@indexname+']  ON​ ['+@event_database+']..[Events] REBUILD​ ​PARTITION​ ​=​ ​ALL WITH ​(​DATA_COMPRESSION​ ​=​ ​PAGE, ONLINE = ON​)') ");
                    queryBuilder.Append("FETCH NEXT FROM index_cursor ");
                    queryBuilder.Append("INTO @indexname, @data_compression_desc ");
                    queryBuilder.Append("END ");
                    queryBuilder.Append("CLOSE index_cursor ");
                    queryBuilder.Append("DEALLOCATE index_cursor ");
                    queryBuilder.Append("FETCH NEXT FROM eventdb_cursor ");
                    queryBuilder.Append("INTO @event_database ");
                    queryBuilder.Append("IF OBJECT_ID('tempdb..#Index_Temp') IS NOT NULL ");
                    queryBuilder.Append("DROP TABLE #Index_Temp ");
                    queryBuilder.Append("END ");
                    queryBuilder.Append("CLOSE eventdb_cursor ");
                    queryBuilder.Append("DEALLOCATE eventdb_cursor ");
                    string query = queryBuilder.ToString();
                    using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                    {
                        cmd.CommandTimeout = queryTimeoutDuration;
                        cmd.ExecuteScalar();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        static public void UpgradeIndexes(SqlConnection conn, int queryTimeoutDuration)
        {
            using (SqlTransaction transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    StringBuilder queryBuilder = new StringBuilder("");
                    queryBuilder.Append("DECLARE @event_database nvarchar(max) ");
                    queryBuilder.Append("DECLARE @indexname nvarchar(100) ");
                    queryBuilder.Append("DECLARE @data_compression_desc nvarchar(10) ");
                    queryBuilder.Append("declare @strSQL nvarchar(2000) ");
                    queryBuilder.Append("DECLARE eventdb_cursor CURSOR FOR SELECT databaseName from[SQLcompliance]..[SystemDatabases] where databaseType = 'Event' ");
                    queryBuilder.Append("OPEN eventdb_cursor ");
                    queryBuilder.Append("FETCH NEXT FROM eventdb_cursor ");
                    queryBuilder.Append("INTO @event_database ");
                    queryBuilder.Append("WHILE @@FETCH_STATUS = 0 ");
                    queryBuilder.Append("BEGIN ");
                    queryBuilder.Append("CREATE TABLE #Index_Temp ( indexname nvarchar(50), data_compression_desc nvarchar(10)) ");
                    queryBuilder.Append("set @strSQL = 'USE ' + '[' + @event_database + '] INSERT INTO #Index_Temp SELECT i.name AS indexname, p.data_compression_desc FROM sys.tables AS t INNER JOIN sys.indexes AS i ON t.object_id = i.object_id INNER JOIN sys.partitions AS p ON i.object_id = p.object_id AND i.index_id = p.index_id Where i.name in (''IX_Events_eventId'',''IX_Events_eventType'',''IX_Events_databaseId'',''IX_Events_appNameId'',''IX_Events_hostId'',''IX_Events_loginId'',''IX_Events_eventCategory'') and p.data_compression_desc = ''None'' and t.name = ''Events''' ");
                    queryBuilder.Append("EXEC sp_executesql @strSQL ");
                    queryBuilder.Append("DECLARE index_cursor CURSOR FOR Select indexname,data_compression_desc from #Index_Temp ");
                    queryBuilder.Append("OPEN index_cursor FETCH NEXT FROM index_cursor INTO @indexname, @data_compression_desc ");
                    queryBuilder.Append("WHILE @@FETCH_STATUS = 0 ");
                    queryBuilder.Append("BEGIN ");
                    queryBuilder.Append("EXEC ('ALTER​ ​INDEX​ ['+@indexname+']  ON​ ['+@event_database+']..[Events] REBUILD​ ​PARTITION​ ​=​ ​ALL WITH ​(​DATA_COMPRESSION​ ​=​ ​PAGE)') ");
                    queryBuilder.Append("FETCH NEXT FROM index_cursor ");
                    queryBuilder.Append("INTO @indexname, @data_compression_desc ");
                    queryBuilder.Append("END ");
                    queryBuilder.Append("CLOSE index_cursor ");
                    queryBuilder.Append("DEALLOCATE index_cursor ");
                    queryBuilder.Append("FETCH NEXT FROM eventdb_cursor ");
                    queryBuilder.Append("INTO @event_database ");
                    queryBuilder.Append("IF OBJECT_ID('tempdb..#Index_Temp') IS NOT NULL ");
                    queryBuilder.Append("DROP TABLE #Index_Temp ");
                    queryBuilder.Append("END ");
                    queryBuilder.Append("CLOSE eventdb_cursor ");
                    queryBuilder.Append("DEALLOCATE eventdb_cursor ");
                    string query = queryBuilder.ToString();
                    using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                    {
                        cmd.CommandTimeout = queryTimeoutDuration;
                        cmd.ExecuteScalar();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        static public void UpdateEventDatabaseIndexes(SqlConnection conn, int queryTimeoutDuration)
        {
            try
            {
                StringBuilder queryBuilder = new StringBuilder("");
                queryBuilder.Append("DECLARE @event_database nvarchar(max) ");
                queryBuilder.Append("DECLARE @strSQL nvarchar(max) ");
                queryBuilder.Append("DECLARE eventdb_cursor CURSOR FOR SELECT databaseName from[SQLcompliance]..[SystemDatabases] where databaseType = 'Event' ");
                queryBuilder.Append("OPEN eventdb_cursor ");
                queryBuilder.Append("FETCH NEXT FROM eventdb_cursor ");
                queryBuilder.Append("INTO @event_database ");
                queryBuilder.Append("WHILE @@FETCH_STATUS = 0 ");
                queryBuilder.Append("BEGIN ");
                queryBuilder.Append("Set @strSQL = ('USE ' + '[' + @event_database + '] ");
                queryBuilder.Append("IF EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_eventId'') DROP INDEX [IX_Events_eventId] ON ['+@event_database+']..[Events];");

                /// SQLCM-6352 
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_DataChanges_dcId_eventId'') CREATE  INDEX [IX_DataChanges_dcId_eventId]  ON ['+@event_database+']..[DataChanges]( [dcId] ASC, [eventId] ASC) ON [PRIMARY]; ");

                /// SQLCM-6333
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Hosts_NameId'') CREATE CLUSTERED INDEX [IX_Hosts_NameId] ON ['+@event_database+']..[Hosts] ([name] ASC, [id] ASC ); ");
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Applications_NameId'') CREATE CLUSTERED INDEX [IX_Applications_NameId] ON ['+@event_database+']..[Applications]([name] ASC, [id] ASC ); ");

                /// SQLCM-6335
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_StartTime_startSeq_eventId'') CREATE UNIQUE CLUSTERED INDEX [IX_Events_StartTime_startSeq_eventId] ON ['+@event_database+']..[Events]( [StartTime] ASC, [startSequence] ASC, [eventId] ASC ) WITH (DATA_COMPRESSION = PAGE); ");

                /// SQLCM-6341
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_BAD_guid_sSeq_inc_eType_eCat'') CREATE INDEX [IX_BAD_guid_sSeq_inc_eType_eCat] ON ['+@event_database+']..[Events]( [guid] ASC, [startSequence] ASC )INCLUDE( [eventType], [eventCategory] ); ");  //SQLCM-6341
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_BAD_guid_eventSequence'') CREATE INDEX [IX_BAD_guid_eventSequence] ON ['+@event_database+']..[DataChanges] ( [guid] ASC, [eventSequence] ASC, [eventId] ASC ); ");

                ///SQLCM-6356
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Stats_Category_date_lastUp'') CREATE INDEX [IX_Stats_Category_date_lastUp] ON ['+@event_database+']..[Stats] ([category],[date],[lastUpdated]) INCLUDE ([count]);')");

                queryBuilder.Append("EXEC sp_executesql @strSQL; ");
                queryBuilder.Append("FETCH NEXT FROM eventdb_cursor INTO @event_database ");
                queryBuilder.Append("END; ");
                queryBuilder.Append("CLOSE eventdb_cursor; ");
                queryBuilder.Append("DEALLOCATE eventdb_cursor; ");
                string query = queryBuilder.ToString();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandTimeout = queryTimeoutDuration;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static public void UpdateEventDatabaseOnlineIndexes(SqlConnection conn, int queryTimeoutDuration)
        {
            try
            {
                StringBuilder queryBuilder = new StringBuilder("");
                queryBuilder.Append("DECLARE @event_database nvarchar(max) ");
                queryBuilder.Append("DECLARE @strSQL nvarchar(max) ");
                queryBuilder.Append("DECLARE eventdb_cursor CURSOR FOR SELECT databaseName from[SQLcompliance]..[SystemDatabases] where databaseType = 'Event' ");
                queryBuilder.Append("OPEN eventdb_cursor ");
                queryBuilder.Append("FETCH NEXT FROM eventdb_cursor ");
                queryBuilder.Append("INTO @event_database ");
                queryBuilder.Append("WHILE @@FETCH_STATUS = 0 ");
                queryBuilder.Append("BEGIN ");
                queryBuilder.Append("Set @strSQL = ('USE ' + '[' + @event_database + '] ");
                queryBuilder.Append("IF EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_eventId'') DROP INDEX [IX_Events_eventId] ON ['+@event_database+']..[Events] WITH ( ONLINE = ON ); ");

                /// SQLCM-6352
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_DataChanges_dcId_eventId'') CREATE  INDEX [IX_DataChanges_dcId_eventId]  ON ['+@event_database+']..[DataChanges]( [dcId] ASC, [eventId] ASC) WITH (ONLINE=ON)  ON [PRIMARY]; ");

                /// SQLCM-6333
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Hosts_NameId'') CREATE CLUSTERED INDEX [IX_Hosts_NameId] ON ['+@event_database+']..[Hosts] ([name] ASC, [id] ASC ) WITH (ONLINE=ON); ");
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Applications_NameId'') CREATE CLUSTERED INDEX [IX_Applications_NameId] ON ['+@event_database+']..[Applications]([name] ASC, [id] ASC ) WITH (ONLINE=ON); ");

                /// SQLCM-6335
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_StartTime_startSeq_eventId'') CREATE UNIQUE CLUSTERED INDEX [IX_Events_StartTime_startSeq_eventId] ON ['+@event_database+']..[Events]( [startTime] ASC, [startSequence] ASC, [eventId] ASC ) WITH (DATA_COMPRESSION = PAGE); ");

                ///SQLCM-6341
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_BAD_guid_sSeq_inc_eType_eCat'') CREATE INDEX [IX_BAD_guid_sSeq_inc_eType_eCat] ON ['+@event_database+']..[Events]( [guid] ASC, [startSequence] ASC )INCLUDE( [eventType], [eventCategory] ) WITH (ONLINE=ON)  ON [PRIMARY]; ");
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_BAD_guid_eventSequence'') CREATE INDEX [IX_BAD_guid_eventSequence] ON ['+@event_database+']..[DataChanges] ( [guid] ASC, [eventSequence] ASC, [eventId] ASC ) WITH (ONLINE=ON)  ON [PRIMARY]; ");

                ///SQLCM-6356
                queryBuilder.Append("IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Stats_Category_date_lastUp'') CREATE INDEX [IX_Stats_Category_date_lastUp] ON ['+@event_database+']..[Stats] ([category],[date],[lastUpdated]) INCLUDE ([count]) WITH (ONLINE=ON)  ON [PRIMARY];') ");
                queryBuilder.Append("EXEC sp_executesql @strSQL; ");
                queryBuilder.Append("FETCH NEXT FROM eventdb_cursor INTO @event_database ");
                queryBuilder.Append("END; ");
                queryBuilder.Append("CLOSE eventdb_cursor; ");
                queryBuilder.Append("DEALLOCATE eventdb_cursor; ");
                string query = queryBuilder.ToString();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandTimeout = queryTimeoutDuration;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static public void UpdateIdentityDatatype(SqlConnection conn, int queryTimeoutDuration)
        {
            try
            {
                StringBuilder queryBuilder = new StringBuilder("");
                queryBuilder.Append("DECLARE @event_database nvarchar(max) ");
                queryBuilder.Append("DECLARE @strSQL nvarchar(max) ");
                queryBuilder.Append("DECLARE eventdb_cursor CURSOR FOR SELECT databaseName from [SQLcompliance]..[SystemDatabases] where databaseType = 'Event' ");
                queryBuilder.Append("OPEN eventdb_cursor ");
                queryBuilder.Append("FETCH NEXT FROM eventdb_cursor ");
                queryBuilder.Append("INTO @event_database ");
                queryBuilder.Append("WHILE @@FETCH_STATUS = 0 ");
                queryBuilder.Append("BEGIN ");
                queryBuilder.Append("Set @strSQL = ('USE ' + '[' + @event_database + '] ");
                queryBuilder.Append("IF((SELECT count(DATA_TYPE) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME IN( ''ColumnChanges'', ''DataChanges'') AND COLUMN_NAME IN(''ccId'',''dcId'') and DATA_TYPE = ''bigint'') <> 3) ");
                queryBuilder.Append("BEGIN ");
                queryBuilder.Append("IF EXISTS (SELECT name FROM sysindexes WHERE name = N''IX_ColumnChanges_dcId'') DROP INDEX [IX_ColumnChanges_dcId] ON ['+@event_database+']..[ColumnChanges]; ");
                queryBuilder.Append("IF EXISTS (SELECT name FROM sysindexes WHERE name = N''IX_ColumnChanges_FK'') DROP INDEX [IX_ColumnChanges_FK] ON ['+@event_database+']..[ColumnChanges]; ");
                queryBuilder.Append("IF EXISTS (SELECT name FROM sysindexes WHERE name = N''IX_DataChanges_startTime'') DROP INDEX [IX_DataChanges_startTime] ON ['+@event_database+']..[DataChanges]; ");
                queryBuilder.Append("IF EXISTS (SELECT name FROM sysindexes WHERE name = N''IX_DataChanges_eventId'') DROP INDEX [IX_DataChanges_eventId] ON ['+@event_database+']..[DataChanges]; ");
                queryBuilder.Append("IF EXISTS (SELECT name FROM sysindexes WHERE name = N''IX_ColumnChanges_columnId'') DROP INDEX [IX_ColumnChanges_columnId] ON ['+@event_database+']..[ColumnChanges]; ");
                queryBuilder.Append("IF EXISTS (SELECT name FROM sysindexes WHERE name = N''IX_DataChanges_tableId'') DROP INDEX [IX_DataChanges_tableId] ON ['+@event_database+']..[DataChanges]; ");
                queryBuilder.Append("IF EXISTS (SELECT name FROM sysindexes WHERE name = N''IX_DataChanges_dcId_eventId'') DROP INDEX [IX_DataChanges_dcId_eventId] ON ['+@event_database+']..[DataChanges]; ");
                queryBuilder.Append("IF EXISTS (SELECT name from sys.stats where name = N''ST_ColumnChanges'' and object_id = object_id(N''[dbo].[ColumnChanges]'')) DROP STATISTICS [ColumnChanges].[ST_ColumnChanges]; ");
                queryBuilder.Append("IF EXISTS (SELECT name from sys.stats where name = N''ST_DataChanges1'' and object_id = object_id(N''[dbo].[DataChanges]'')) DROP STATISTICS [DataChanges].[ST_DataChanges1]; ");
                queryBuilder.Append("IF EXISTS (SELECT name from sys.stats where name = N''ST_DataChanges2'' and object_id = object_id(N''[dbo].[DataChanges]'')) DROP STATISTICS [DataChanges].[ST_DataChanges2]; ");
                queryBuilder.Append("IF EXISTS (SELECT name from sys.stats where name = N''ST_DataChanges3'' and object_id = object_id(N''[dbo].[DataChanges]'')) DROP STATISTICS [DataChanges].[ST_DataChanges3]; ");
                queryBuilder.Append("IF EXISTS (SELECT name from sys.stats where name = N''ST_DataChanges4'' and object_id = object_id(N''[dbo].[DataChanges]'')) DROP STATISTICS [DataChanges].[ST_DataChanges4]; ");
                queryBuilder.Append("IF EXISTS (SELECT name from sys.stats where name = N''ST_DataChanges5'' and object_id = object_id(N''[dbo].[DataChanges]'')) DROP STATISTICS [DataChanges].[ST_DataChanges5]; ");

                queryBuilder.Append("Alter Table [ColumnChanges] Alter Column [ccId] bigint; ");
                queryBuilder.Append("Alter Table [ColumnChanges] Alter Column dcId bigint; ");
                queryBuilder.Append("Alter Table [DataChanges] Alter Column dcId bigint; ");

                queryBuilder.Append("CREATE NONCLUSTERED INDEX [IX_ColumnChanges_dcId] ON [dbo].[ColumnChanges] ([dcId] ASC) INCLUDE ( [startTime],[eventSequence],[spid],[columnName],[beforeValue],[afterValue],[hashcode]) ON [PRIMARY]; ");
                queryBuilder.Append("CREATE INDEX [IX_ColumnChanges_FK] ON ColumnChanges(startTime, eventSequence, spid ) ON [PRIMARY]; ");
                queryBuilder.Append("CREATE CLUSTERED INDEX [IX_DataChanges_startTime] ON [dbo].[DataChanges] ([startTime] ASC,	[eventSequence] ASC,[spid] ASC) ON [PRIMARY]; ");
                queryBuilder.Append("CREATE NONCLUSTERED INDEX [IX_DataChanges_eventId] ON [dbo].[DataChanges] ([eventId] ASC,	[recordNumber] ASC,[dcId] ASC)INCLUDE ( [startTime],[eventSequence],[spid],[databaseId],[actionType],[tableName],[userName],[changedColumns],[primaryKey],[hashcode],[totalChanges]) ON [PRIMARY]; ");
                queryBuilder.Append("CREATE CLUSTERED INDEX [IX_ColumnChanges_columnId] ON [dbo].[ColumnChanges] ([columnId] ASC) ON [PRIMARY]; ");
                queryBuilder.Append("CREATE NONCLUSTERED INDEX [IX_DataChanges_tableId] ON [dbo].[DataChanges] ([tableId] ASC,	[eventId] ASC,[recordNumber] ASC,[dcId] ASC)INCLUDE ( [startTime],[eventSequence],[spid],[databaseId],[actionType],[tableName],[userName],[changedColumns],[primaryKey],[hashcode],[totalChanges])  ON [PRIMARY]; ");
                queryBuilder.Append("CREATE STATISTICS [ST_ColumnChanges] ON [dbo].[ColumnChanges]([dcId], [columnId]); ");
                queryBuilder.Append("CREATE STATISTICS [ST_DataChanges1] ON [dbo].[DataChanges]([dcId], [recordNumber]); ");
                queryBuilder.Append("CREATE STATISTICS [ST_DataChanges2] ON [dbo].[DataChanges]([dcId], [eventId]); ");
                queryBuilder.Append("CREATE STATISTICS [ST_DataChanges3] ON [dbo].[DataChanges]([dcId], [tableId]); ");
                queryBuilder.Append("CREATE STATISTICS [ST_DataChanges4] ON [dbo].[DataChanges]([recordNumber], [eventId], [tableId]); ");
                queryBuilder.Append("CREATE STATISTICS [ST_DataChanges5] ON [dbo].[DataChanges]([eventId], [dcId], [tableId], [recordNumber]); ");
                queryBuilder.Append("END;') ");
                queryBuilder.Append("EXEC sp_executesql @strSQL; ");
                queryBuilder.Append("FETCH NEXT FROM eventdb_cursor INTO @event_database ");
                queryBuilder.Append("END; ");
                queryBuilder.Append("CLOSE eventdb_cursor; ");
                queryBuilder.Append("DEALLOCATE eventdb_cursor; ");
                string query = queryBuilder.ToString();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandTimeout = queryTimeoutDuration;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
