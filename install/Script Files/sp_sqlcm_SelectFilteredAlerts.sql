USE [SQLcompliance]
GO

IF (OBJECT_ID('sp_sqlcm_SelectFilteredAlerts') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_SelectFilteredAlerts
END
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_SelectFilteredAlerts]    Script Date: 3/19/2016 1:14:28 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO


CREATE PROCEDURE [dbo].[sp_sqlcm_SelectFilteredAlerts]
@AlertLevels nvarchar(255),
@Instance nvarchar(max),
@DateFrom datetime,
@DateTo datetime,
@TimeFrom datetime,
@TimeTo datetime,
@AlertType int,
@SourceRule nvarchar(1024),
@EventTypeName nvarchar(1024),
@Details nvarchar(1024),
@Page int,
@PageSize int,
@SortColumn nvarchar(100),
@SortOrder int
AS
BEGIN
DECLARE @QueryEvent nvarchar(max);
DECLARE @Iterator INT;
DECLARE @TimeFromInSeconds bigint
DECLARE @TimeToInSeconds bigint

SET @TimeFromInSeconds = NULL

IF (@TimeFrom IS NOT NULL)
BEGIN
	SET @TimeFromInSeconds = DATEPART(hour, @TimeFrom) * 3600000 + DATEPART(minute,@TimeFrom) * 60000 + DATEPART(second,@TimeFrom) * 1000 + DATEPART(millisecond,@TimeTo)
if(@TimeFromInSeconds > (18*3600000 + 30*60000))
		SET @TimeFromInSeconds = @TimeFromInSeconds -  (24*3600000)
END

SET @TimeToInSeconds = NULL

IF (@TimeTo IS NOT NULL)
BEGIN
	SET @TimeToInSeconds = DATEPART(hour, @TimeTo) * 3600000 + DATEPART(minute,@TimeTo) * 60000 + DATEPART(second,@TimeTo) * 1000 + DATEPART(millisecond,@TimeTo)
IF(@TimeToInSeconds > (18*3600000 + 30*60000))
		SET @TimeToInSeconds = @TimeToInSeconds - (24*3600000)
END

SET @Iterator = 1;

CREATE TABLE #TempServers (
 RowID int IDENTITY(1, 1), 
 srvId int,
 instance nvarchar(256),
 eventDatabase nvarchar(128),
)

CREATE TABLE #OutputTable (
[serverId] int null,
[alertId] int null,
[alertType] int null,
[instance] nvarchar(256) null,
[alertTime] datetime null,
[alertLevel] tinyint null,
[alertEventId] int null,
[alertRule] nvarchar(60) null,
[alertEventTypeId] int null,
[alertEventTypeName] nvarchar(64) null,
[alertEventDetail] nvarchar(512) null
)

INSERT INTO #TempServers (srvId, instance, eventDatabase)
	SELECT srvId, instance, eventDatabase
		FROM [SQLcompliance].[dbo].[Servers]
		WHERE @Instance IS NULL OR (SELECT instance FROM [SQLcompliance].[dbo].[Servers] WITH (NOLOCK) WHERE instance  like ''+@Instance+'') COLLATE DATABASE_DEFAULT IN ( select Value COLLATE DATABASE_DEFAULT from dbo.fn_sqlsm_Split(instance, ','))

DECLARE @NumberOfRecords INT;
SET @NumberOfRecords = (SELECT COUNT(*) FROM #TempServers)

DECLARE @query nvarchar(max);
SET @query = '';

WHILE (@Iterator <= @NumberOfRecords)
BEGIN
	DECLARE @EventDatabaseName nvarchar(128);
	DECLARE @ServerId int;
	DECLARE @CurrentInstance nvarchar(256);

	SELECT @EventDatabaseName = eventDatabase, @ServerId = srvId, @CurrentInstance = instance
	FROM #TempServers WHERE RowID = @Iterator
	IF (EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE ('[' + name + ']' = @EventDatabaseName OR name = @EventDatabaseName)))
	BEGIN 
		SET @query = 'INSERT INTO #OutputTable ([serverId], [alertId], [alertType], [instance], [alertTime], [alertLevel], [alertEventId], [alertRule], [alertEventTypeId], [alertEventTypeName], [alertEventDetail])
SELECT @ServerId,
a.[alertId], 
a.[alertType],
a.[instance],
a.[created],
a.[alertLevel],
a.[alertEventId],
a.[ruleName],
a.[eventType],
et.[name],
e.[details]
FROM [SQLcompliance]..[Alerts] a
INNER JOIN [SQLcompliance]..[Servers] s ON s.[instance] = a.[instance] 
LEFT JOIN [' + @EventDatabaseName + ']..[Events] e ON e.eventId = a.alertEventId 
LEFT JOIN [SQLcompliance]..[EventTypes] et ON et.[evtypeid] = a.[eventType]
WHERE a.isDismissed = 0 OR a.isDismissed IS NULL AND a.instance = @CurrentInstance
AND (@AlertType IS NULL OR @AlertType = 4 OR a.[alertType] = @AlertType) 
AND (@DateFrom is NULL OR CONVERT(VARCHAR(10), @DateFrom, 112) <=  CONVERT(VARCHAR(10), a.created, 112))
AND (@DateTo is NULL OR CONVERT(VARCHAR(10), @DateTo, 112) >=  CONVERT(VARCHAR(10), a.created, 112))
AND (@TimeFromInSeconds is NULL OR (DATEPART(hour, a.created) * 3600000 + DATEPART(minute,a.created) * 60000 + DATEPART(second,a.created)*1000 + DATEPART(millisecond,a.created)) >= @TimeFromInSeconds)
AND (@TimeToInSeconds is NULL OR (DATEPART(hour, a.created) * 3600000 + DATEPART(minute,a.created) * 60000 + DATEPART(second,a.created)*1000 + DATEPART(millisecond,a.created)) <= @TimeToInSeconds)';

DECLARE @subWhereCondition NVARCHAR(MAX);
IF (@AlertLevels IS NOT NULL)
BEGIN
	SET @query = @query + ' AND a.[alertLevel] in (' + @AlertLevels +')';
END

IF(@SourceRule IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (isnull(a.[ruleName],'''') LIKE ''' + REPLACE(@SourceRule,',',''' OR isnull(a.[ruleName],'''') LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END

IF(@EventTypeName IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (et.name LIKE ''' + REPLACE(@EventTypeName,',',''' OR et.name LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END

IF(@Details IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (isnull(e.[details],'''') LIKE ''' + REPLACE(@Details,',',''' OR isnull(e.[details],'''') LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END


EXEC sp_executesql @query, N'@ServerId int, @AlertType int, @TimeFromInSeconds bigint, @TimeToInSeconds bigint, @DateFrom datetime, @DateTo datetime, @CurrentInstance nvarchar(256)',  
@ServerId,  @AlertType,   @TimeFromInSeconds, @TimeToInSeconds, @DateFrom, @DateTo, @CurrentInstance;

	END
	SET @Iterator = @Iterator + 1;
END

SELECT COUNT(*) FROM #OutputTable

SELECT alertLevel, COUNT(*) as alertCount FROM #OutputTable Group by alertLevel

SELECT 
(SELECT COUNT(*) FROM (SELECT DISTINCT (instance) FROM #OutputTable WHERE alertLevel = 1) low) as low,
(SELECT COUNT(*) FROM (SELECT DISTINCT (instance) FROM #OutputTable WHERE alertLevel = 2) med) as med,
(SELECT COUNT(*) FROM (SELECT DISTINCT (instance) FROM #OutputTable WHERE alertLevel = 3) high) as high,
(SELECT COUNT(*) FROM (SELECT DISTINCT (instance) FROM #OutputTable WHERE alertLevel = 4) severe) as severe

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
	SET @PagingQuery = 'DECLARE PagingCursor CURSOR DYNAMIC READ_ONLY FOR SELECT alertId FROM #OutputTable ORDER BY ' + @SortColumn +' ' +  @SortOrderString + ' ' + '';
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
	SET @ResultQuery = 'SELECT ot.* FROM #OutputTable ot JOIN #tblPK temp ON ot.alertId = temp.PK ORDER BY ' + @SortColumn +' ' +  @SortOrderString + ' ' + '';
	EXEC sp_executesql @ResultQuery;
	Set  @QueryEvent = ' select distinct et.[name] as EventType FROM [SQLcompliance]..[Alerts] a INNER JOIN [SQLcompliance]..[Servers] s ON s.[instance] = a.[instance] 
						LEFT JOIN [' + @EventDatabaseName + ']..[Events] e ON e.eventId = a.alertEventId 
						LEFT JOIN [SQLcompliance]..[EventTypes] et ON et.[evtypeid] = a.[eventType] where isnull(et.[name],'''')!= ''''';
	EXEC sp_executesql @QueryEvent;
END
ELSE BEGIN
	SELECT * FROM #OutputTable;
	Set  @QueryEvent = ' select distinct et.[name] as EventType FROM [SQLcompliance]..[Alerts] a INNER JOIN [SQLcompliance]..[Servers] s ON s.[instance] = a.[instance] 
						LEFT JOIN [' + @EventDatabaseName + ']..[Events] e ON e.eventId = a.alertEventId 
						LEFT JOIN [SQLcompliance]..[EventTypes] et ON et.[evtypeid] = a.[eventType] where isnull(et.[name],'''')!= ''''';
	EXEC sp_executesql @QueryEvent;
END
END
 
 

GO

