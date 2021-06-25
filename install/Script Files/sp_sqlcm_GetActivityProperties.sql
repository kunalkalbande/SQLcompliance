USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetActivityProperties]    Script Date: 11/20/2015 12:02:05 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_GetActivityProperties') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetActivityProperties
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_GetActivityProperties]
@EventId int,
@Instance nvarchar(256),
@DateFrom datetime,
@DateTo datetime,
@TimeFrom datetime,
@TimeTo datetime,
@EventType int,
@Details nvarchar(1024),
@Page int,
@PageSize int,
@SortColumn nvarchar(100),
@SortOrder int
AS
BEGIN

DECLARE @Iterator INT;
SET @Iterator = 1;

CREATE TABLE #OutputTable (
[id] int NOT NULL IDENTITY,
[eventId] int null,
[eventType] varchar(256) null,
[instance] nvarchar(256) null,
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
WHERE b.[eventId] = a.[eventType] AND a.[eventId] = @EventId';

DECLARE @subWhereCondition NVARCHAR(MAX);

IF(@Details IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (a.[details] LIKE ''' + REPLACE(@Details,',',''' OR a.[details] LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END


EXEC sp_executesql @query, N'@EventType int,@EventId int',  
@EventType,@EventId;

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
END
ELSE BEGIN
	SELECT * FROM #OutputTable
END

GO

