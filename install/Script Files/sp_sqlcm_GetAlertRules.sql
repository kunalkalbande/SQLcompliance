USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetAlertRules]    Script Date: 11/20/2015 2:06:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_GetAlertRules') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetAlertRules
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_GetAlertRules]
@TargetInstance nvarchar(256),
@Name nvarchar(60),
@AlertLevels nvarchar(255),
@AlertType int,
@LogMessage nvarchar (255),
@EmailMessage nvarchar (255),
@SnmpTrap nvarchar(255),
@Email nvarchar(255),
@EventLog nvarchar(255),
@Rule nvarchar(255),
@RuleType nvarchar(255),
@Server nvarchar(255),
@Page int,
@PageSize int,
@SortColumn nvarchar(100),
@SortOrder int

AS
BEGIN

CREATE TABLE #OutputTable (
[ruleId] int not null,
[name] nvarchar(60) null,
[alertLevel] tinyint null,
[alertType] int null,
[targetInstances] nvarchar(256) not null,
[logMessage] tinyint not null,
[emailMessage] tinyint not null,
[snmpTrap] tinyint not null,
[enabled] bit not null,
[instanceId] int null,
[ruleValidation] int
)
DECLARE @NumberOfRecords INT;
SET @NumberOfRecords = (SELECT COUNT(*) as count FROM AlertRules)

DECLARE @query nvarchar(max);
SET @query = '';
   BEGIN
		SET @query = 'INSERT INTO #OutputTable ([ruleId],[name],[alertLevel],[alertType],[targetInstances],[logMessage],[emailMessage],[snmpTrap],[enabled],[ruleValidation])
SELECT 
a.[ruleId], 
a.[name],
a.[alertLevel],
a.[alertType],
a.[targetInstances],
a.[logMessage],
a.[emailMessage],
a.[snmpTrap],
a.[enabled],
(SELECT COUNT(matchString) FROM [SQLcompliance]..[AlertRuleConditions] b WHERE (a.[targetInstances]='''' and b.ruleId=a.ruleId) OR (b.ruleId=a.ruleId and (matchString ='''' OR matchString = ''include(1)0value(0)''  OR matchString like "blanks(1)0count(1)0%" 
OR matchString like ''%rowcount(0)%''OR (a.[alertType]= 3 AND matchString like ''%(0)%'' AND matchString not like ''%timeframe(0)%'')))) AS ruleValidation
FROM [SQLcompliance]..[AlertRules] a where 1=1 ';

DECLARE @subWhereCondition NVARCHAR(MAX);
IF (@AlertLevels IS NOT NULL)
BEGIN
	SET @query = @query + ' AND a.[alertLevel] in (' + @AlertLevels +')';
END

IF(@Email IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (a.[emailMessage] LIKE ''' + REPLACE(@Email,',',''' OR a.[emailMessage] LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END

IF(@EventLog IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (a.[logMessage] LIKE ''' + REPLACE(@EventLog,',',''' OR a.[logMessage] LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END

IF(@Rule IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (a.[name] LIKE ''' + REPLACE(@Rule,',',''' OR a.[name] LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END

IF(@RuleType IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (a.[alertType] LIKE ''' + REPLACE(@RuleType,',',''' OR a.[alertType] LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END

IF(@Server IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (a.[targetInstances] LIKE ''' + REPLACE(@Server,',',''' OR a.[targetInstances] LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END

IF(@SnmpTrap IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND (a.[snmpTrap] LIKE ''' + REPLACE(@SnmpTrap,',',''' OR a.[snmpTrap] LIKE ''') + ''')'
	SET @query = @query + @subWhereCondition;
END

EXEC sp_executesql @query, N'@AlertType int, @Instance nvarchar(256)',  
@AlertType, @TargetInstance;
	END
END

DECLARE @queryinstId nvarchar(max);
SET @queryinstId = '';
	BEGIN 
		SET @queryinstId = 'Update #OutputTable set instanceId = (select s.[srvId] from [Servers] s where s.instance COLLATE SQL_Latin1_General_CP1_CI_AS = #OutputTable.targetInstances)';
	END

EXEC sp_executesql @queryinstId;

SELECT COUNT(*) FROM #OutputTable

SELECT alertType, COUNT(*) as alertCount FROM #OutputTable Group by alertType

SELECT 
(SELECT COUNT(*) FROM (SELECT DISTINCT (ruleId) FROM #OutputTable WHERE alertType = 1) event) as event,
(SELECT COUNT(*) FROM (SELECT DISTINCT (ruleId) FROM #OutputTable WHERE alertType = 2) status) as status,
(SELECT COUNT(*) FROM (SELECT DISTINCT (ruleId) FROM #OutputTable WHERE alertType = 3) data) as data

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
	SET @PagingQuery = 'DECLARE PagingCursor CURSOR DYNAMIC READ_ONLY FOR SELECT ruleId FROM #OutputTable ORDER BY ' + @SortColumn +' ' +  @SortOrderString + ' ' + '';
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
	DECLARE @queryEvent nvarchar(max);
	SET @queryEvent = 'SELECT distinct name AS EventType FROM [SQLcompliance]..[AlertRules]'; 
     EXEC sp_executesql @queryEvent;
END
ELSE BEGIN
	SELECT * FROM #OutputTable	
	SET @queryEvent = 'SELECT distinct name AS EventType FROM [SQLcompliance]..[AlertRules]'; 
     EXEC sp_executesql @queryEvent;
END
GO