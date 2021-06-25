--
-- Upgrade Event Databases and Archives
--

USE [SQLcompliance]
GO

IF (OBJECT_ID('sp_sqlcm_UpgradeAllEventDatabase') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_UpgradeAllEventDatabase
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_UpgradeAllEventDatabase](@databaseName nvarchar(128))
as
BEGIN
	DECLARE @databaseName2 nvarchar(256);
	DECLARE @sqlText nvarchar(4000) ;
	DECLARE @columnWidth nvarchar(24);
	DECLARE @columnWidthVar sql_variant;
	
	SELECT @columnWidthVar=SERVERPROPERTY('productversion');
	IF (SUBSTRING(CONVERT(nvarchar, @columnWidthVar),1,1)='8')
	   SET @columnWidth='4000';
	ELSE
	   SET @columnWidth='max';
	
	SET @databaseName2 = replace(@databaseName, char(39),char(39)+char(39)) ;
	
	-- 2.0 Upgrades
	SET @sqlText = N'USE [' + @databaseName2 + ']
IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name=''fileName'' and o.name=''Events'' and c.id=o.id)
ALTER TABLE [Events] ADD 
	[fileName] [nvarchar] (128) NULL ,
	[linkedServerName] [nvarchar] (128) NULL ,
	[parentName] [nvarchar] (128) NULL ,
	[isSystem] [int] NULL ,
	[sessionLoginName] [nvarchar] (128) NULL ,
	[providerName] [nvarchar] (128) NULL;';
	
	EXEC(@sqlText);
	
	-- 3.0 Upgrades
	SET @sqlText = N'USE [' + @databaseName2 + ']
IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name=''appNameId'' and o.name=''Events'' and c.id=o.id)
BEGIN
	ALTER TABLE [Events] ADD
		[appNameId] [int] NULL,
		[hostId] [int] NULL,
		[loginId] [int] NULL;
END';
	
	EXEC(@sqlText) ;	

	SET @sqlText = N'USE [' + @databaseName2 + ']
IF OBJECT_ID(''Stats'') IS NULL
BEGIN
CREATE TABLE [Stats] ( 
   [dbId] [int] NOT NULL DEFAULT 0, 
   [category] [int] NOT NULL, 
   [date] [DateTime] NOT NULL, 
   [count] [int] NOT NULL DEFAULT 0, 
   [lastUpdated] [DateTime] NOT NULL,
   CONSTRAINT [PK_Stats] PRIMARY KEY CLUSTERED ( [dbId], [date] DESC, [category] ) ) ON [PRIMARY];
END

IF OBJECT_ID(''Applications'') IS NULL
BEGIN
	CREATE TABLE [Applications] ( [name] [nvarchar] (128) NOT NULL, [id] int NOT NULL );
END

IF OBJECT_ID(''Hosts'') IS NULL
BEGIN
	CREATE TABLE [Hosts] ( [name] [nvarchar] (128) NOT NULL, [id] int NOT NULL );
END

IF OBJECT_ID(''Logins'') IS NULL
BEGIN
	CREATE TABLE [Logins] ( [name] [nvarchar] (128) NOT NULL, [id] int NOT NULL );
END';
	
	EXEC(@sqlText);
	
	-- 3.1 changes
	SET @sqlText = N'USE [' + @databaseName2 + ']
IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name=''startSequence'' and o.name=''Events'' and c.id=o.id)
	ALTER TABLE Events ADD endTime datetime null, startSequence bigint null, endSequence bigint null;


IF OBJECT_ID(''DataChanges'') IS NULL
BEGIN
	CREATE TABLE DataChanges (  startTime datetime not null,
		eventSequence bigint not null,
		spid int not null,
		databaseId int not null,
		actionType int not null,
        schemaName nvarchar(128) not null,
		tableName nvarchar(	128) not null,
		recordNumber int not null,
		userName nvarchar(40) not null,
		changedColumns int not null,
		primaryKey nVarchar(4000),
		hashcode int not null ,
		tableId int null,
		dcId int IDENTITY (-2100000000, 1) NOT NULL,
		eventId int null  ) 
	on [PRIMARY];

	GRANT SELECT ON [DataChanges] TO [guest];
END

IF OBJECT_ID(''ColumnChanges'') IS NULL
BEGIN
	CREATE TABLE ColumnChanges ( startTime datetime not null,
		eventSequence bigint not null,
		spid int not null,
		columnName nVarchar(128 )not null,
		beforeValue nVarchar(' + @columnWidth + '),
		afterValue nVarchar(' + @columnWidth + '),
		hashcode int not null,
		columnId int null,
		dcId int null ) 
	on [PRIMARY];
	
	GRANT SELECT ON [ColumnChanges] TO [guest];
END
IF OBJECT_ID(''Databases'') IS NULL
BEGIN
	CREATE TABLE [dbo].[Databases](
	[srvId] [int] NOT NULL,
	[databaseName] [nvarchar](128) NOT NULL,
	[dbId] [smallint] NOT NULL
) ON [PRIMARY]
END';

	EXEC(@sqlText);
	
	-- 5.5 Upgrades
	SET @sqlText = N'USE [' + @databaseName2 + ']
IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name=''rowCounts'' and o.name=''Events'' and c.id=o.id)
BEGIN
	ALTER TABLE [Events] ADD
	[rowCounts] [bigint] NULL,
	[guid] [nvarchar] (100) NULL;
END';
	
	EXEC(@sqlText) ;
	
	IF @columnWidth = 'max'
	BEGIN
	SET @sqlText = N'USE [' + @databaseName2 + ']
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_ColumnChanges_dcId'') 
CREATE INDEX [IX_ColumnChanges_dcId] ON [dbo].[ColumnChanges] ([dcId] ASC) INCLUDE ( [startTime],[eventSequence],[spid],[columnName],[beforeValue],[afterValue],[hashcode]) ON [PRIMARY];

IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_ColumnChanges_FK'') 
CREATE INDEX [IX_ColumnChanges_FK] ON [dbo].[ColumnChanges](startTime, eventSequence, spid ) ON [PRIMARY];

IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_DataChanges_startTime'')
CREATE CLUSTERED INDEX [IX_DataChanges_startTime] ON [dbo].[DataChanges] ([startTime] ASC,	[eventSequence] ASC,[spid] ASC) ON [PRIMARY];

IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_DataChanges_eventId'') 
CREATE INDEX [IX_DataChanges_eventId] ON [dbo].[DataChanges] ([eventId] ASC,	[recordNumber] ASC,[dcId] ASC)INCLUDE ( [startTime],[eventSequence],[spid],[databaseId],[actionType],[tableName],[userName],[changedColumns],[primaryKey],[hashcode]) ON [PRIMARY];

IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_ColumnChanges_columnId'') 
CREATE CLUSTERED INDEX [IX_ColumnChanges_columnId] ON [dbo].[ColumnChanges] ([columnId] ASC) ON [PRIMARY];

IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_DataChanges_tableId'') 
CREATE INDEX [IX_DataChanges_tableId] ON [dbo].[DataChanges] ([tableId] ASC,	[eventId] ASC,[recordNumber] ASC,[dcId] ASC)INCLUDE ( [startTime],[eventSequence],[spid],[databaseId],[actionType],[tableName],[userName],[changedColumns],[primaryKey],[hashcode])  ON [PRIMARY];

IF NOT EXISTS (SELECT name from sys.stats where name = N''ST_ColumnChanges'' and object_id = object_id(N''[dbo].[ColumnChanges]''))
CREATE STATISTICS [ST_ColumnChanges] ON [dbo].[ColumnChanges]([dcId], [columnId]);

IF NOT EXISTS (SELECT name from sys.stats where name = N''ST_DataChanges1'' and object_id = object_id(N''[dbo].[DataChanges]''))
CREATE STATISTICS [ST_DataChanges1] ON [dbo].[DataChanges]([dcId], [recordNumber]);

IF NOT EXISTS (SELECT name from sys.stats where name = N''ST_DataChanges2'' and object_id = object_id(N''[dbo].[DataChanges]''))
CREATE STATISTICS [ST_DataChanges2] ON [dbo].[DataChanges]([dcId], [eventId]);

IF NOT EXISTS (SELECT name from sys.stats where name = N''ST_DataChanges3'' and object_id = object_id(N''[dbo].[DataChanges]''))
CREATE STATISTICS [ST_DataChanges3] ON [dbo].[DataChanges]([dcId], [tableId]);

IF NOT EXISTS (SELECT name from sys.stats where name = N''ST_DataChanges4'' and object_id = object_id(N''[dbo].[DataChanges]''))
CREATE STATISTICS [ST_DataChanges4] ON [dbo].[DataChanges]([recordNumber], [eventId], [tableId]);

IF NOT EXISTS (SELECT name from sys.stats where name = N''ST_DataChanges5'' and object_id = object_id(N''[dbo].[DataChanges]''))
CREATE STATISTICS [ST_DataChanges5] ON [dbo].[DataChanges]([eventId], [dcId], [tableId], [recordNumber]);';

	EXEC(@sqlText);
   END;
   
   -- Date Change Linking Stored Procedures
	SET @sqlText = N'USE [' + @databaseName2 + ']
if exists (select * from dbo.sysobjects where name = N''p_LinkDataChangeRecords'' and xtype=''P'')
drop procedure [p_LinkDataChangeRecords];
if exists (select * from dbo.sysobjects where name = N''p_LinkAllDataChangeRecords'' and xtype=''P'')
drop procedure [p_LinkAllDataChangeRecords];';
   EXEC(@sqlText);
   
   
	SET @sqlText = N'USE [' + @databaseName2 + ']
DECLARE @myProc nvarchar(2000);
SET @myProc=N''CREATE PROCEDURE p_LinkDataChangeRecords (@startTime DateTime, @endTime DateTime) AS BEGIN
UPDATE t1 set t1.eventId=t2.eventId FROM DataChanges t1 INNER JOIN 
(SELECT * from Events where eventCategory=4 AND startTime >= @startTime AND startTime <= @endTime) t2 
             ON (t1.startTime >= t2.startTime AND t1.startTime <= t2.endTime AND 
             t1.eventSequence >= t2.startSequence AND t1.eventSequence <= t2.endSequence AND t1.spid = t2.spid) 
            WHERE t1.eventId IS NULL AND t1.startTime >= @startTime AND t1.startTime <= @endTime

UPDATE t1 set t1.dcId=t2.dcId FROM ColumnChanges t1 INNER JOIN DataChanges t2 
             ON (t1.startTime = t2.startTime AND t1.eventSequence = t2.eventSequence AND t1.spid = t2.spid) 
             WHERE t1.dcId IS NULL AND t1.startTime >= @startTime AND t1.startTime <= @endTime
END;''
EXEC(@myProc);';
   EXEC(@sqlText);

	SET @sqlText = N'USE [' + @databaseName2 + ']
DECLARE @myProc nvarchar(2000);
SET @myProc=N''CREATE PROCEDURE p_LinkAllDataChangeRecords AS BEGIN
UPDATE t1 set t1.eventId=t2.eventId FROM DataChanges t1 INNER JOIN 
(SELECT * from Events where eventCategory=4) t2 
             ON (t1.startTime >= t2.startTime AND t1.startTime <= t2.endTime AND 
             t1.eventSequence >= t2.startSequence AND t1.eventSequence <= t2.endSequence AND t1.spid = t2.spid) 
            WHERE t1.eventId IS NULL 

UPDATE t1 set t1.dcId=t2.dcId FROM ColumnChanges t1 INNER JOIN DataChanges t2 
             ON (t1.startTime = t2.startTime AND t1.eventSequence = t2.eventSequence AND t1.spid = t2.spid) 
             WHERE t1.dcId IS NULL 
END;''
EXEC(@myProc);';
	EXEC(@sqlText);

   
	
	SET @sqlText = N'USE [' + @databaseName2 + ']
IF OBJECT_ID(''Tables'') IS NULL
BEGIN
	CREATE TABLE [Tables] ( [schemaName] [nvarchar] (128) NOT NULL,[name] [nvarchar] (128) NOT NULL, [id] [int] NOT NULL ) ON [PRIMARY];
	GRANT SELECT ON [dbo].[Tables] TO [guest];
END

IF OBJECT_ID(''Columns'') IS NULL
BEGIN
	CREATE TABLE [Columns] ( [name] [nvarchar] (128) NOT NULL, [id] [int] NOT NULL ) ON [PRIMARY];
	GRANT SELECT ON [dbo].[Columns] TO [guest];
END';

	EXEC(@sqlText);
	
	-- 3.2 changes
	SET @sqlText = N'USE [' + @databaseName2 + ']
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name=''totalChanges'' and o.name=''DataChanges'' and c.id=o.id)
	BEGIN
		ALTER TABLE [DataChanges] ADD [totalChanges] [int] NULL ;
	END';
		
	EXEC(@sqlText);

	-- 5.5 changes
	SET @sqlText = N'USE [' + @databaseName2 + ']
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name=''guid'' and o.name=''DataChanges'' and c.id=o.id)
	BEGIN
		ALTER TABLE [DataChanges] ADD [guid] [nvarchar] (100) NULL ;
	END';
		
	EXEC(@sqlText);
	
	SET @sqlText = N'USE [' + @databaseName2 + ']
	GRANT SELECT ON [Stats] TO [guest];
	GRANT SELECT ON [Logins] TO [guest];
	GRANT SELECT ON [Applications] TO [guest];
	GRANT SELECT ON [Hosts] TO [guest];';
	
	EXEC(@sqlText);

	--3.5 changes
	SET @sqlText = N'USE [' + @databaseName2 + ']
	IF OBJECT_ID(''SensitiveColumns'') IS NULL
	BEGIN
		CREATE TABLE SensitiveColumns (
			[startTime] DATETIME NOT NULL,
			[eventId] INT NOT NULL,
			[columnName] nvarchar(128) NOT NULL,
			[hashcode] [INT] NOT NULL,
			[tableId] [INT] NULL,
			[columnId] [INT] NULL)

		GRANT SELECT ON [SensitiveColumns] TO [guest];
	END';

	EXEC(@sqlText);
	
	SET @sqlText = N'USE [' + @databaseName2 + ']
		IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_SensitiveColumns_eventId'') 
		CREATE CLUSTERED INDEX [IX_SensitiveColumns_eventId] ON [dbo].[SensitiveColumns] ([eventId] ASC) ON [PRIMARY];

		IF NOT EXISTS (SELECT name from sys.stats where name = N''ST_SensitiveColumns'' and object_id = object_id(N''[dbo].[SensitiveColumns]''))
		CREATE STATISTICS [ST_SensitiveColumns] ON [dbo].[SensitiveColumns]([eventId]);';
	EXEC(@sqlText);	

	--3.6 changes	
	SET @sqlText = N'USE [' + @databaseName2 + ']
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name=''ccId'' and o.name=''ColumnChanges'' and c.id=o.id)
	BEGIN
		ALTER TABLE [ColumnChanges] ADD ccId int IDENTITY (-2100000000, 1) NOT NULL
	END

	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name=''scId'' and o.name=''SensitiveColumns'' and c.id=o.id)
	BEGIN
		ALTER TABLE [SensitiveColumns] ADD scId int IDENTITY (-2100000000, 1) NOT NULL
	END';
	EXEC(@sqlText);

	-- Update Schema Version	
	SET @sqlText = N'USE [' + @databaseName2 + ']
IF EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_appNameId'')
	UPDATE [Description] SET [eventDbSchemaVersion]=702;
ELSE
	UPDATE [Description] SET [eventDbSchemaVersion]=701;';
	
	EXEC(@sqlText);
END