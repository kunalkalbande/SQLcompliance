USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_SelectFilteredActivityLogs]    Script Date: 11/3/2015 9:50:20 PM ******/



IF (OBJECT_ID('sp_sqlcm_SelectFilteredActivityLogs') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_SelectFilteredActivityLogs
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO


CREATE PROCEDURE [dbo].[sp_sqlcm_SelectFilteredActivityLogs]
@Instance nvarchar(256),
@DateFrom datetime,
@DateTo datetime,
@TimeFrom datetime,
@TimeTo datetime,
@Event nvarchar(Max),
@EventType int,
@Details nvarchar(1024),
@Page int,
@PageSize int,
@SortColumn nvarchar(100),
@SortOrder int
AS
BEGIN

DECLARE @Iterator INT;
DECLARE @TimeFromInSeconds bigint;
DECLARE @TimeToInSeconds bigint;

SET @TimeFromInSeconds = NULL
IF (@TimeFrom IS NOT NULL)
BEGIN
	SET @TimeFromInSeconds = DATEPART(hour, @TimeFrom) * 3600000 + DATEPART(minute,@TimeFrom) * 60000 + DATEPART(second,@TimeFrom) * 1000 + DATEPART(millisecond,@TimeFrom)		
END

SET @TimeToInSeconds = NULL

IF (@TimeTo IS NOT NULL)
BEGIN
	SET @TimeToInSeconds = DATEPART(hour, @TimeTo) * 3600000 + DATEPART(minute,@TimeTo) * 60000 + DATEPART(second,@TimeTo) * 1000 + DATEPART(millisecond,@TimeTo)
END


SET @Iterator = 1;
DECLARE @queryEvent nvarchar(max);
CREATE TABLE #OutputTable (
[id] int NOT NULL IDENTITY,
[eventId] int null,
[eventType] varchar(256) null,
[instance] nvarchar(256) null,
[instanceId] int,
[eventTime] datetime null,
[details] nvarchar(max) null
)

DECLARE @NumberOfRecords INT;
SET @NumberOfRecords = (SELECT COUNT(*) FROM AgentEvents)

DECLARE @query nvarchar(max);
SET @query = '';
	BEGIN 
		SET @query = 'INSERT INTO #OutputTable ([eventId], [eventType], [instance], [eventTime],[details])
SELECT 
a.[eventId], 
b.[Name],
a.[instance],
a.[eventTime],
a.[details]
FROM [SQLcompliance]..[AgentEvents] a, [SQLcompliance]..[AgentEventTypes] b
WHERE b.[eventId] = a.[eventType] 
AND (@DateFrom is NULL OR CONVERT(VARCHAR(10), @DateFrom, 112) <=  CONVERT(VARCHAR(10), a.eventTime, 112))
AND (@DateTo is NULL OR CONVERT(VARCHAR(10), @DateTo, 112) >= CONVERT(VARCHAR(10), a.eventTime, 112))
AND (@TimeFromInSeconds is NULL OR (DATEPART(hour, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), a.eventTime)) * 3600000 + 
DATEPART(minute,DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), a.eventTime)) * 60000 + 
DATEPART(second,DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), a.eventTime))*1000 + 
DATEPART(millisecond,DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), a.eventTime))) >= @TimeFromInSeconds)
AND (@TimeToInSeconds is NULL OR (DATEPART(hour, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), a.eventTime)) * 3600000 + 
DATEPART(minute,DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), a.eventTime)) * 60000 + 
DATEPART(second,DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), a.eventTime))*1000 + 
DATEPART(millisecond,DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), a.eventTime))) <= @TimeToInSeconds)';

DECLARE @subWhereCondition NVARCHAR(MAX);


------------------------------------------------------------------------------------------------------------
---SQLCM-137 Data entry fields such as Event, Details etc. are not supporting the use of wildcards  
------------------------------------------------------------------------------------------------------------
--Start

IF(@Instance IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (isnull(a.[instance],'''') LIKE ''' + REPLACE(@Instance,',',''' OR isnull(a.[instance],'''') LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END

IF(@Details IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (isnull(a.[details],'''') LIKE ''' + REPLACE(@Details,',',''' OR isnull(a.[details],'''') LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END


IF(@Event IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (b.[Name] LIKE ''' + REPLACE(@Event,',',''' OR b.[Name] LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END



EXEC sp_executesql @query, N'@EventType int, @TimeFromInSeconds bigint, @TimeToInSeconds bigint, @DateFrom datetime, @DateTo datetime, @Instance nvarchar(256)',  
@EventType,@TimeFromInSeconds, @TimeToInSeconds,@DateFrom, @DateTo, @Instance;
DECLARE @queryinstId nvarchar(max);
SET @queryinstId = '';
	BEGIN 
		SET @queryinstId = 'Update #OutputTable set instanceId = (select s.[srvId] from [Servers] s where s.instance COLLATE SQL_Latin1_General_CP1_CI_AS = #OutputTable.instance)';
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
	SET @PagingQuery = 'DECLARE PagingCursor CURSOR DYNAMIC READ_ONLY FOR SELECT eventId FROM #OutputTable ORDER BY ' + @SortColumn +' ' +  @SortOrderString + ' ' + '';
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
	SET @ResultQuery = 'SELECT ot.* FROM #OutputTable ot JOIN #tblPK temp ON ot.eventId = temp.PK ORDER BY ' + @SortColumn +' ' +  @SortOrderString + ' ' + '';
	EXEC sp_executesql @ResultQuery;
    SET @queryEvent = '
	SELECT distinct b.[Name] AS EventType FROM [SQLcompliance]..[AgentEvents] a, [SQLcompliance]..[AgentEventTypes] b
WHERE b.[eventId] = a.[eventType] '; 
     EXEC sp_executesql @queryEvent;
END
ELSE BEGIN
	SELECT * FROM #OutputTable;
	SET @queryEvent = '
	SELECT distinct b.[Name] AS EventType FROM [SQLcompliance]..[AgentEvents] a, [SQLcompliance]..[AgentEventTypes] b
WHERE b.[eventId] = a.[eventType] '; 
     EXEC sp_executesql @queryEvent;
END
 



GO

