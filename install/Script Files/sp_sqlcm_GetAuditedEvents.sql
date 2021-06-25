USE [SQLcompliance]
GO

IF (OBJECT_ID('sp_sqlcm_GetAuditedEvents') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetAuditedEvents
END
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetAuditedEvents]    Script Date: 3/19/2016 1:18:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_sqlcm_GetAuditedEvents]
 @EventDatabaseName nvarchar(128),
 @DatabaseName nvarchar(1024),
 @DatabaseId int,
 @Category nvarchar(1024),
 @EventCategoryId int,
 @EventType nvarchar(1024),
 @LoginName nvarchar(1024),
 @TargetObject nvarchar(1024),
 @TimeFrom datetime,
 @TimeTo datetime,
 @DateFrom datetime,
 @DateTo datetime,
 @Details nvarchar(1024),
 @Page int,
 @PageSize int,
 @SortColumn nvarchar(100),
	 @SortOrder int,
	@SpidFrom int,
	@SpidTo int,
	@Application nvarchar(1024),
	@Host nvarchar(1024),
	@Server nvarchar(1024),
	@AccessCheck nvarchar(1024),
	@DatabaseUser nvarchar(1024),
	@Object nvarchar(1024),
	@TargetLogin nvarchar(1024),
	@TargetUser nvarchar(1024),
	@Role nvarchar(1024),
	@Owner nvarchar(1024),
	@PrivilegedUser nvarchar(1024),
	@SessionLogin nvarchar(1024),
	@AuditedUpdatesFrom int,
	@AuditedUpdatesTo int,
	@PrimaryKey nvarchar(1024),
	@TableId int,
	@Table nvarchar(1024),
	@ColumnId int,
	@Column nvarchar(1024),
	@BeforeValue nvarchar(1024),
	@AfterValue nvarchar(1024),
	@ColumnsUpdatedFrom int,
	@ColumnsUpdatedTo int,
	@Schema nvarchar(1024)
AS
BEGIN
CREATE TABLE #OutputTable (
[Id] int null,
[databaseId] int null,
[EventDatabaseName] nvarchar(128) null,
[databaseName] nvarchar(128) null,
[EventTypeId] int null,
[CategoryId] int null,
[Category] nvarchar(16) null,
[EventType] nvarchar(64) null,
[Time] datetime null,
[Details] nvarchar(512) null,
[loginName] nvarchar(128) null,
[targetObject] nvarchar(512) null,
[Spid] int null,
[Application] nvarchar(128) null,
[Host] nvarchar(128) null,
[Server] nvarchar(128) null,
[AccessCheck] int null,
[DatabaseUser] nvarchar(128) null,
[Object] nvarchar(512) null,
[TargetLogin] nvarchar(128) null,
[TargetUser] nvarchar(128) null,
[Role] nvarchar(128) null,
[Owner] nvarchar(128) null,
[PrivilegedUser] int null,
[SessionLogin] nvarchar(128) null,
[AuditedUpdates] int null,
[PrimaryKey] nvarchar(4000) null,
[Table] nvarchar(128) null,
[Column] nvarchar(128) null,
[BeforeValue] nvarchar(max) null,
[AfterValue] nvarchar(max) null,
[ColumnsUpdated] int null,
[Schema] nvarchar(128) null
)
DECLARE @queryEvent nvarchar(max);
DECLARE @ActualDbName AS NVARCHAR(128);
DECLARE @TimeFromInSeconds bigint
DECLARE @TimeToInSeconds bigint

SET @TimeFromInSeconds = NULL

IF (@TimeFrom IS NOT NULL)
BEGIN
	SET @TimeFromInSeconds = DATEPART(hour, @TimeFrom) * 3600000 + DATEPART(minute,@TimeFrom) * 60000 + DATEPART(second,@TimeFrom) * 1000 + DATEPART(millisecond,@TimeTo)
END

SET @TimeToInSeconds = NULL

IF (@TimeTo IS NOT NULL)
BEGIN
	SET @TimeToInSeconds = DATEPART(hour, @TimeTo) * 3600000 + DATEPART(minute,@TimeTo) * 60000 + DATEPART(second,@TimeTo) * 1000 + DATEPART(millisecond,@TimeTo)
END

IF (@Owner IS NOT NULL)
BEGIN
 SET @Owner = LOWER(@Owner);
END

SET @ActualDbName = NULL;
IF(@DatabaseId IS NOT NULL)
	SELECT @ActualDbName = name FROM [SQLcompliance]..Databases d  WHERE d.[dbId] = @DatabaseId

declare @serverId as int
select @serverId = srvId from [SQLcompliance]..Servers
where eventDatabase = @EventDatabaseName
	
DECLARE @query nvarchar(max);
SET @query = '
;WITH CalculationResults(EventId, [AuditedUpdates], [ColumnsUpdated])
 AS
 (
	SELECT
		e.eventId as EventId,
		(SELECT Top 1 ISNULL(dc2.totalChanges, 0) from [' + @EventDatabaseName + ']..DataChanges dc2 where dc2.recordNumber = 0 and dc2.eventId = d.eventId and dc2.eventId IS NOT NULL) as [AuditedUpdates],
		(case 
			when e.eventCategory = 5 then (select count(sc.columnName) from [' + @EventDatabaseName + ']..SensitiveColumns sc where e.eventId = sc.eventId) 
			else (select 0)
		end) as [ColumnsUpdated]
	FROM [' + @EventDatabaseName + ']..[Events] e

	LEFT OUTER JOIN [' + @EventDatabaseName + ']..DataChanges as d ON (e.eventId = d.eventId AND d.primaryKey IS NOT NULL)
	where e.eventCategory <> 0 AND e.eventCategory <> 10
 )

INSERT INTO #OutputTable ([Id], [databaseId], [databaseName], [EventDatabaseName], [EventTypeId], [CategoryId], [Category], [EventType], [Time], [Details], [loginName], [targetObject], [Spid], [Application], [Host], [Server], [AccessCheck], [DatabaseUser], [Object], [TargetLogin], [TargetUser], [Role], [Owner], [PrivilegedUser], [SessionLogin], [AuditedUpdates], [PrimaryKey], [Table], [Column], [BeforeValue], [AfterValue], [ColumnsUpdated], [Schema])
 SELECT TOP 5000 e.[eventId] AS Id,
isnull(cDb.[dbId], 0) as databaseId,
e.databaseName,
@EventDatabaseName as EventDatabaseName,
e.eventType as EventTypeId,
e.eventCategory as CategoryId,
et.category AS Category,
et.name AS EventType,
e.startTime AS Time,
e.details AS Details,
e.loginName,
e.targetObject,
e.spid as Spid,
e.applicationName as [Application],
e.hostName as [Host],
e.serverName as [Server],
e.success as [AccessCheck],
e.dbUserName as [DatabaseUser],
e.objectName as [Object],
e.targetLoginName as [TargetLogin],
e.targetUserName as [TargetUser],
e.roleName as [Role],
e.ownerName as [Owner],
e.privilegedUser as [PrivilegedUser],
e.sessionLoginName as [SessionLogin],
cr.[AuditedUpdates],
d.primaryKey as [PrimaryKey],
d.tableName as [Table],
c.columnName as [Column]
,c.beforeValue as [BeforeValue],
c.afterValue as [AfterValue],
cr.[ColumnsUpdated],
e.parentName
FROM [' + @EventDatabaseName + ']..[Events] e
LEFT JOIN [SQLcompliance]..[EventTypes] et ON et.evtypeid = e.eventType
LEFT JOIN [SQLcompliance]..[Databases] cDb on cDb.sqlDatabaseId = e.databaseId  and cDb.srvId = ' + convert(nvarchar, @serverId) + '

LEFT OUTER JOIN [' + @EventDatabaseName + ']..DataChanges as d ON (e.eventId = d.eventId AND d.primaryKey IS NOT NULL)
LEFT OUTER JOIN [' + @EventDatabaseName + ']..ColumnChanges as c ON (d.dcId = c.dcId) 

LEFT OUTER JOIN CalculationResults as cr on e.eventId = cr.EventId

WHERE 
e.eventCategory <> 0 AND e.eventCategory <> 10
AND (@DatabaseId IS NULL OR e.databaseName = @ActualDbName)
AND (@EventCategoryId IS NULL OR e.eventCategory = @EventCategoryId)
AND (@DateFrom is NULL OR CONVERT(VARCHAR(10), @DateFrom, 112) <=  CONVERT(VARCHAR(10), e.startTime, 112))
AND (@DateTo is NULL OR CONVERT(VARCHAR(10), @DateTo, 112) >=  CONVERT(VARCHAR(10), e.startTime, 112))
AND (@TimeFromInSeconds is NULL OR (DATEPART(hour, e.startTime) * 3600000 + DATEPART(minute,e.startTime) * 60000 + DATEPART(second,e.startTime)*1000 + DATEPART(millisecond,e.startTime)) >= @TimeFromInSeconds)
AND (@TimeToInSeconds is NULL OR (DATEPART(hour, e.startTime) * 3600000 + DATEPART(minute,e.startTime) * 60000 + DATEPART(second,e.startTime)*1000 + DATEPART(millisecond,e.startTime)) <= @TimeToInSeconds)';

DECLARE @subWhereCondition NVARCHAR(MAX);
IF(@DatabaseName IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (e.databaseName LIKE ''' + REPLACE(@DatabaseName,',',''' OR e.databaseName LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END
IF(@Category IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (et.category LIKE ''' + REPLACE(@Category,',',''' OR et.category LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END
IF(@EventType IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (et.name LIKE ''' + REPLACE(@EventType,',',''' OR et.name LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END
IF(@Details IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (e.details LIKE ''' + REPLACE(@Details,',',''' OR e.details LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END
IF(@LoginName IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (e.loginName LIKE ''' + REPLACE(@LoginName,',',''' OR e.loginName LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END
IF(@TargetObject IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (e.targetObject LIKE ''' + REPLACE(@TargetObject,',',''' OR e.targetObject LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END

----------------------------------------- Details ------------------------------------------
IF(@SpidFrom IS NOT NULL)
BEGIN 
	SET @query = @query + ' AND (e.spid >=' + CONVERT(nvarchar(50), @SpidFrom) + ')';
END

IF(@SpidTo IS NOT NULL)
BEGIN 
	SET @query = @query + ' AND (e.spid <=' + CONVERT(nvarchar(50), @SpidTo) + ')';
END

IF(@Application IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (e.applicationName LIKE ''' + REPLACE(@Application,',',''' OR e.applicationName LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END
IF(@Host IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (e.hostName LIKE ''' + REPLACE(@Host,',',''' OR e.hostName LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END
IF(@Server IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (e.serverName LIKE ''' + REPLACE(@Server,',',''' OR e.serverName LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END
IF(@AccessCheck IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (e.success IN (' + @AccessCheck + '))';
	SET @query = @query + @subWhereCondition;
END
IF(@DatabaseUser IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (e.dbUserName LIKE ''' + REPLACE(@DatabaseUser,',',''' OR e.dbUserName LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END
IF(@Object IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (e.objectName LIKE ''' + REPLACE(@Object,',',''' OR e.objectName LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END
IF(@TargetLogin IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (e.targetLoginName LIKE ''' + REPLACE(@TargetLogin,',',''' OR e.targetLoginName LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END
IF(@TargetUser IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (e.targetUserName LIKE ''' + REPLACE(@TargetUser,',',''' OR e.targetUserName LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END
IF(@Role IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (e.roleName LIKE ''' + REPLACE(@Role,',',''' OR e.roleName LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END
IF(@Owner IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (e.ownerName LIKE ''' + REPLACE(@Owner,',',''' OR e.ownerName LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END
IF(@PrivilegedUser IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (e.privilegedUser IN (' + @PrivilegedUser + '))';
	SET @query = @query + @subWhereCondition;
END
IF(@SessionLogin IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (e.sessionLoginName LIKE ''' + REPLACE(@SessionLogin,',',''' OR e.sessionLoginName LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END

IF(@AuditedUpdatesFrom IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (cr.[AuditedUpdates] >= ' + convert(nvarchar, @AuditedUpdatesFrom) + ')'; 
	SET @query = @query + @subWhereCondition;
END
IF(@AuditedUpdatesTo IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (cr.[AuditedUpdates] < ' + convert(nvarchar, @AuditedUpdatesTo) + ')'; 
	SET @query = @query + @subWhereCondition;
END

IF(@PrimaryKey IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (d.primaryKey LIKE ''' + REPLACE(@PrimaryKey,',',''' OR d.primaryKey LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END

IF(@TableId IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (d.tableId LIKE ''' + REPLACE(@TableId,',',''' OR d.tableId LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END

IF(@Table IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (d.tableName LIKE ''' + REPLACE(@Table,',',''' OR d.tableName LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END

IF(@ColumnId IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (c.columnId LIKE ''' + REPLACE(@ColumnId,',',''' OR c.columnId LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END

IF(@Column IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (c.columnName LIKE ''' + REPLACE(@Column,',',''' OR c.columnName LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END
IF(@BeforeValue IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (c.beforeValue LIKE ''' + REPLACE(@BeforeValue,',',''' OR c.beforeValue LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END
IF(@AfterValue IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (c.afterValue LIKE ''' + REPLACE(@AfterValue,',',''' OR c.afterValue LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
END

IF(@ColumnsUpdatedFrom IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (cr.[ColumnsUpdated] >= ' + convert(nvarchar, @ColumnsUpdatedFrom) + ')';
	SET @query = @query + @subWhereCondition;
end
IF(@ColumnsUpdatedTo IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (cr.[ColumnsUpdated] < ' + convert(nvarchar, @ColumnsUpdatedTo) + ')';
	SET @query = @query + @subWhereCondition;
end

IF(@Schema IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (e.parentName LIKE ''' + REPLACE(@Schema,',',''' OR e.parentName LIKE ''') + ''')';
	SET @query = @query + @subWhereCondition;
end
 
 set @query = @query + ' ORDER BY e.startTime DESC, e.startSequence DESC, e.eventId DESC'

EXEC sp_executesql @query, N'@EventDatabaseName nvarchar(128), @DatabaseId int, @TimeFromInSeconds bigint, @TimeToInSeconds bigint, @DateFrom datetime, @DateTo datetime, @EventCategoryId int, @ActualDbName nvarchar(128)',  
@EventDatabaseName, @DatabaseId, @TimeFromInSeconds, @TimeToInSeconds, @DateFrom, @DateTo, @EventCategoryId, @ActualDbName;

SELECT COUNT(*) FROM #OutputTable

IF (@Page IS NOT NULL AND @PageSize IS NOT NULL AND @SortColumn IS NOT NULL AND @SortOrder IS NOT NULL)
BEGIN
	DECLARE @SortOrderString nvarchar(4);
	IF (@SortOrder = 1)
		SET @SortOrderString = 'ASC';
	ELSE SET @SortOrderString = 'DESC';

	DECLARE @StartRow INT;
	SET @StartRow = (@Page - 1) * @PageSize + 1;
		
	DECLARE @PK int;
	CREATE TABLE #tblPK (
		PK int NOT NULL PRIMARY KEY
	)
	
	DECLARE @PagingQuery NVARCHAR(MAX);
	SET @PagingQuery = 'DECLARE PagingCursor CURSOR DYNAMIC READ_ONLY FOR SELECT Id FROM (SELECT DISTINCT Id, [' + @SortColumn +'] FROM #OutputTable) as result ORDER BY result.[' + @SortColumn +'] ' +  @SortOrderString + '' + '';

	EXEC sp_executesql @PagingQuery;
	
	OPEN PagingCursor FETCH RELATIVE @StartRow FROM PagingCursor INTO @PK
	
	WHILE @PageSize > 0 AND @@FETCH_STATUS = 0
	BEGIN
		INSERT #tblPK(PK) VALUES(@PK)
		FETCH NEXT FROM PagingCursor INTO @PK
		SET @PageSize = @PageSize - 1
	END

	CLOSE PagingCursor
	DEALLOCATE PagingCursor
	
	DECLARE @ResultQuery nvarchar(max);
	SET @ResultQuery = 'SELECT DISTINCT ot.* FROM #OutputTable ot JOIN #tblPK temp ON ot.Id = temp.PK ORDER BY [' + @SortColumn +'] ' +  @SortOrderString + ' ' + '';
	EXEC sp_executesql @ResultQuery;
    SET @queryEvent = '
	SELECT distinct et.name AS EventType FROM [' + @EventDatabaseName + ']..[Events] e
    LEFT JOIN [SQLcompliance]..[EventTypes] et ON et.evtypeid = e.eventType'; 
     EXEC sp_executesql @queryEvent;
END
ELSE BEGIN
	SELECT * FROM #OutputTable;
	SET @queryEvent = '
	SELECT distinct et.name AS EventType FROM [' + @EventDatabaseName + ']..[Events] e
    LEFT JOIN [SQLcompliance]..[EventTypes] et ON et.evtypeid = e.eventType'; 
    EXEC sp_executesql @queryEvent;;
END
END 
 

GO

