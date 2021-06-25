USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_SelectFilteredChangeLogs]    Script Date: 11/3/2015 9:50:02 PM ******/

IF (OBJECT_ID('sp_sqlcm_SelectFilteredChangeLogs') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_SelectFilteredChangeLogs
END
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO




CREATE PROCEDURE [dbo].[sp_sqlcm_SelectFilteredChangeLogs]
@Instance nvarchar(256),
@Event nvarchar(256),
@DateFrom datetime,
@DateTo datetime,
@TimeFrom datetime,
@TimeTo datetime,
@LogType int,
@User nvarchar(max),
@Detail nvarchar(max),
@Page int,
@PageSize int,
@SortColumn nvarchar(100),
@SortOrder int
AS
BEGIN

DECLARE @Iterator INT;
SET @Iterator = 1;
DECLARE @queryEvent nvarchar(max);
CREATE TABLE #OutputTable (
[id] int not null IDENTITY,
[logId] int null,
[eventTime] datetime not null,
[logType] varchar(256) not null,
[logUser] nvarchar(256) not null,
[logSqlServer] nvarchar(256) null,
[instanceId] int,
[logInfo] nvarchar(max) null
)

DECLARE @NumberOfRecords INT;

DECLARE @TimeFromInSeconds bigint;
DECLARE @TimeToInSeconds bigint;

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
SET @NumberOfRecords = (SELECT COUNT(*) FROM AgentEvents)

DECLARE @query nvarchar(max);
SET @query = '';
   BEGIN
		SET @query = 'INSERT INTO #OutputTable ([logId], [eventTime] , [logType], [logUser], [logSqlServer],[logInfo])
SELECT 
a.[logId], 
a.[eventTime],
b.[Name],
a.[logUser],
a.[logSqlServer],
ltrim(rtrim(a.[logInfo]))
FROM [SQLcompliance]..[ChangeLog] a ,[SQLcompliance]..[ChangeLogEventTypes] b
WHERE b.[eventId] = a.[logType] 
AND (@DateFrom is NULL OR CONVERT(VARCHAR(10), @DateFrom, 112) <=  CONVERT(VARCHAR(10), a.eventTime, 112))
AND (@DateTo is NULL OR CONVERT(VARCHAR(10), @DateTo, 112) >=  CONVERT(VARCHAR(10), a.eventTime, 112))
AND (@TimeFromInSeconds is NULL OR (DATEPART(hour, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), a.eventTime)) * 3600000 + 
DATEPART(minute,DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), a.eventTime)) * 60000 + 
DATEPART(second,DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), a.eventTime))*1000 + 
DATEPART(millisecond,DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), a.eventTime))) >= @TimeFromInSeconds)
AND (@TimeToInSeconds is NULL OR (DATEPART(hour, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), a.eventTime)) * 3600000 + 
DATEPART(minute,DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), a.eventTime)) * 60000 + 
DATEPART(second,DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), a.eventTime))*1000 + 
DATEPART(millisecond,DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), a.eventTime))) <= @TimeToInSeconds)'

DECLARE @subWhereCondition NVARCHAR(MAX);

IF(@Instance IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (isnull(a.[logSqlServer],'''') LIKE ''' + REPLACE(@Instance,',',''' OR isnull(a.[logSqlServer],'''') LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END


IF(@User IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (isnull(a.[logUser],'''') LIKE ''' + REPLACE(@User,',',''' OR isnull(a.[logUser],'''') LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END
IF(@Detail IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (isnull(ltrim(rtrim(replace(replace(replace(replace(replace(replace(a.[logInfo],char(9),''''),char(10),'' ''),char(13),''''),''  '','' |''),''| '',''''),''|'',''''))),'''') LIKE ''' + REPLACE(@Detail,',',''' OR isnull(ltrim(rtrim(replace(replace(replace(replace(replace(replace(a.[logInfo],char(9),''''),char(10),'' ''),char(13),''''),''  '','' |''),''| '',''''),''|'',''''))),'''') LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END

IF(@Event IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (b.[Name] LIKE ''' + REPLACE(@Event,',',''' OR b.[Name] LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END


EXEC sp_executesql @query, N'@LogType int, @TimeFrom datetime,@TimeFromInSeconds bigint, @TimeToInSeconds bigint, @TimeTo datetime, @DateFrom datetime, @DateTo datetime, @Instance nvarchar(256)',  
@LogType,@TimeFrom,@TimeFromInSeconds, @TimeToInSeconds, @TimeTo, @DateFrom, @DateTo, @Instance;

DECLARE @queryinstId nvarchar(max);
SET @queryinstId = '';
	BEGIN 
		SET @queryinstId = 'Update #OutputTable set instanceId = (select s.[srvId] from [Servers] s where s.instance COLLATE SQL_Latin1_General_CP1_CI_AS = #OutputTable.logSqlServer)';
	END

EXEC sp_executesql @queryinstId;
	END

	SET @Iterator = @Iterator + 1;
END

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
	SET @PagingQuery = 'DECLARE PagingCursor CURSOR DYNAMIC READ_ONLY FOR SELECT logId FROM #OutputTable ORDER BY ' + @SortColumn +' ' +  @SortOrderString + ' ' + '';
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
	SET @ResultQuery = 'SELECT ot.* FROM #OutputTable ot JOIN #tblPK temp ON ot.logId = temp.PK ORDER BY ' + @SortColumn +' ' +  @SortOrderString + ' ' + '';
	EXEC sp_executesql @ResultQuery;

	SET @queryEvent = '
	SELECT distinct b.[Name] AS EventTypeFROM [SQLcompliance]..[ChangeLog] a ,[SQLcompliance]..[ChangeLogEventTypes] b WHERE b.[eventId] = a.[logType] '; 
     EXEC sp_executesql @queryEvent;
END
ELSE BEGIN
	SELECT * FROM #OutputTable;

	SET @queryEvent = '
	SELECT distinct b.[Name] AS EventType FROM [SQLcompliance]..[ChangeLog] a ,[SQLcompliance]..[ChangeLogEventTypes] b
WHERE b.[eventId] = a.[logType] '; 
     EXEC sp_executesql @queryEvent;
END
 




GO

