USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetChangeProperties]    Script Date: 11/20/2015 12:01:42 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_GetChangeProperties') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetChangeProperties
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_GetChangeProperties]
@EventId int,
@Instance nvarchar(256),
@DateFrom datetime,
@DateTo datetime,
@TimeFrom datetime,
@TimeTo datetime,
@LogType int,
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
[id] int not null IDENTITY,
[logId] int null,
[eventTime] datetime not null,
[logType] varchar(256) not null,
[logUser] nvarchar(256) not null,
[logSqlServer] nvarchar(256) null,
[logInfo] nvarchar(max) null
)

DECLARE @NumberOfRecords INT;
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
a.[logInfo]
FROM [SQLcompliance]..[ChangeLog] a ,[SQLcompliance]..[ChangeLogEventTypes] b
WHERE b.[eventId] = a.[logType] AND a.[logId] = @EventId
AND (@DateFrom is NULL OR CONVERT(VARCHAR(10), @DateFrom, 112) <=  CONVERT(VARCHAR(10), a.eventTime, 112))
AND (@DateTo is NULL OR CONVERT(VARCHAR(10), @DateTo, 112) >=  CONVERT(VARCHAR(10), a.eventTime, 112))
AND (@TimeFrom is NULL OR CONVERT(VARCHAR(8), @TimeFrom, 108) <=  CONVERT(VARCHAR(8), a.eventTime, 108))
AND (@TimeTo is NULL OR CONVERT(VARCHAR(8), @TimeTo, 108) >=  CONVERT(VARCHAR(8), a.eventTime, 108))';

DECLARE @subWhereCondition NVARCHAR(MAX);

IF(@Details IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (a.[logInfo] LIKE ''' + REPLACE(@Details,',',''' OR a.[logInfo] LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END


EXEC sp_executesql @query, N'@LogType int, @EventId int, @TimeFrom datetime, @TimeTo datetime, @DateFrom datetime, @DateTo datetime, @Instance nvarchar(256)',  
@LogType, @EventId,@TimeFrom, @TimeTo, @DateFrom, @DateTo, @Instance;

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
END
ELSE BEGIN
	SELECT * FROM #OutputTable
END
 





GO

