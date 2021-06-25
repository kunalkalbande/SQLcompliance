USE [SQLcompliance]
GO
/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetAuditedEventsFilter]    Script Date: 1/20/2016 6:33:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_GetAuditedEventsFilter') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetAuditedEventsFilter
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_GetAuditedEventsFilter]
@Description nvarchar(256),
@Filter nvarchar(256),
@Instance nvarchar(256),
@isEnabled nvarchar(256),

@TargetInstance nvarchar(256),
@Name nvarchar(60),
@EventType int,
@Page int,
@PageSize int,
@SortColumn nvarchar(100),
@SortOrder int

AS
BEGIN

CREATE TABLE #OutputTable (
[filterId] int not null,
[name] nvarchar(60) null,
[description] nvarchar(200) null,
[eventType] int null,
[targetInstances] nvarchar(640) not null,
[enabled] bit not null,
[instanceId] int null,
[validFilter] int null
)
DECLARE @NumberOfRecords INT;
SET @NumberOfRecords = (SELECT COUNT(*) as count FROM EventFilters)

DECLARE @query nvarchar(max);
SET @query = '';


   BEGIN
  
  SET @query = 'INSERT INTO #OutputTable ([filterId],[name],[description],[eventType],[targetInstances],[enabled],[validFilter])
SELECT 
a.[filterId], 
a.[name],
a.[description],
a.[eventType],
a.[targetInstances],
a.[enabled],
(CASE 
when(SELECT count(matchString) from [SQLcompliance]..[EventFilterConditions] o where o.filterId = a.filterId)>0 and a.targetInstances !='''' then (SELECT count(matchString) from [SQLcompliance]..[EventFilterConditions] o where o.filterId = a.filterId and (o.matchString ='''' OR o.matchString like ''blanks(1)0count(1)0%''))
else 1
end) as validFilter
FROM [SQLcompliance]..[EventFilters] a Where 1=1';


DECLARE @subWhereCondition NVARCHAR(MAX);
IF (@isEnabled IS NOT NULL)
BEGIN
	SET @query = @query + ' AND a.[enabled] in (' + @isEnabled +')';
END

IF(@Filter IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (a.[name] LIKE ''' + REPLACE(@Filter,',',''' OR a.[name] LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END

IF(@Description IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (a.[description] LIKE ''' + REPLACE(@Description,',',''' OR a.[description] LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END

IF(@Instance IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (a.[targetInstances] LIKE ''' + REPLACE(@Instance,',',''' OR a.[targetInstances] LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END




EXEC sp_executesql @query;
 END
END

DECLARE @queryinstId nvarchar(max);
SET @queryinstId = '';
	BEGIN 
		SET @queryinstId = 'Update #OutputTable set instanceId = (select s.[srvId] from [Servers] s where s.instance COLLATE SQL_Latin1_General_CP1_CI_AS = #OutputTable.targetInstances)';
	END

EXEC sp_executesql @queryinstId;

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
 SET @PagingQuery = 'DECLARE PagingCursor CURSOR DYNAMIC READ_ONLY FOR SELECT filterId FROM #OutputTable ORDER BY ' + @SortColumn +' ' +  @SortOrderString + ' ' + '';
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
 SET @ResultQuery = 'SELECT ot.* FROM #OutputTable ot JOIN #tblPK temp ON ot.ruleId = temp.PK ORDER BY ' + @SortColumn +' ' +  @SortOrderString + ' ' + '';
 EXEC sp_executesql @ResultQuery;

END
ELSE
BEGIN
select * from #OutputTable;
 END


GO

