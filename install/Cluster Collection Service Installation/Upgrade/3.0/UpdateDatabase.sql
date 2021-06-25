 

---------- SP_CMREPORT_COMMON.SQL
USE SQLcompliance

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


-- *************************************************************
-- fn_cmreport_GetDateString
-- Gets a string representation of the supplied date
-- *************************************************************
if exists (select * from sysobjects where id = object_id(N'fn_cmreport_GetDateString') and xtype in (N'FN', N'IF', N'TF'))
	drop function [fn_cmreport_GetDateString]
GO

CREATE FUNCTION [fn_cmreport_GetDateString] (@inDate datetime)
RETURNS nvarchar(40)
AS
BEGIN
	RETURN CONVERT(nvarchar(40),@inDate);
END
GO

GRANT EXECUTE ON [fn_cmreport_GetDateString]   TO [public]
GO

-- *************************************************************
-- fn_cmreport_ProcessString
-- Remove injection threats from a string and replace * with % for SQL wildcards
-- *************************************************************
if exists (select * from sysobjects where id = object_id(N'fn_cmreport_ProcessString') and xtype in (N'FN', N'IF', N'TF'))
	drop function [fn_cmreport_ProcessString]
GO

CREATE FUNCTION [fn_cmreport_ProcessString] (@inString nvarchar(4000))
RETURNS nvarchar(4000)
AS
BEGIN
	DECLARE @retVal nvarchar(4000)
	select @retVal = replace(@inString, '*', '%')
	select @retVal = replace(@retVal, '--', '')
	select @retVal = replace(@retVal, ';', '')
	select @retVal = replace(@retVal, '''', '')
	RETURN @retVal ;
END
GO

GRANT EXECUTE ON [fn_cmreport_ProcessString]   TO [public]
GO

-- *************************************************************
-- sp_cmreport_GetEventDatabases
-- Get a list of eventdatabases and their corresponding description
-- *************************************************************
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetEventDatabases' and xtype='P')
drop procedure [sp_cmreport_GetEventDatabases]
GO

CREATE procedure sp_cmreport_GetEventDatabases 
as
   SELECT DISTINCT databaseName, instance + CASE WHEN UPPER(databaseType) LIKE 'EVENT' THEN '' ELSE ' - ' + displayName END AS description
      FROM SystemDatabases
      WHERE(UPPER(databaseType) IN ('ARCHIVE', 'EVENT'))
      ORDER BY description
GO

grant execute on sp_cmreport_GetEventDatabases to public
GO

-- *************************************************************
-- sp_cmreport_GetCategories
-- Get a list of categories and their ids
-- *************************************************************
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetCategories' and xtype='P')
drop procedure [sp_cmreport_GetCategories]
GO

CREATE procedure sp_cmreport_GetCategories 
as
   select -1 as id,'<ALL>' as category UNION select distinct evcatid as id,category from EventTypes where evcatid >= 0 and category <> 'Trace' order by category
GO

grant execute on sp_cmreport_GetCategories to public
GO

-- *************************************************************
-- sp_cmreport_GetInstances
-- Get a list of Instances and their ids
-- *************************************************************
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetInstances' and xtype='P')
drop procedure [sp_cmreport_GetInstances]
GO

CREATE procedure sp_cmreport_GetInstances 
as
select '<ALL>' as instance UNION select instance from Servers order by instance
GO

grant execute on sp_cmreport_GetInstances to public
GO

-- *************************************************************
-- sp_cmreport_GetDateTypes
-- Get a list of DateTypes and their ids
-- *************************************************************
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetDateTypes' and xtype='P')
drop procedure [sp_cmreport_GetDateTypes]
GO

CREATE procedure sp_cmreport_GetDateTypes 
as
   SELECT 'Custom' as DisplayName, '0' as DisplayValue UNION
   SELECT 'Yesterday', '1' UNION 
   SELECT 'Last Week', '2' UNION
   SELECT 'Last Month', '3' UNION
   SELECT 'Today', '4' UNION
   SELECT 'This Week', '5' UNION
   SELECT 'This Month', '6' UNION
   SELECT 'This Quarter', '7' UNION
   SELECT 'Last Quarter', '8' ORDER BY DisplayValue
GO

grant execute on sp_cmreport_GetDateTypes to public
GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

 
GO 

---------- SP_CMREPORT_GETAGENTHISTORY.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/252007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetAgentHistory' and xtype='P')
drop procedure [sp_cmreport_GetAgentHistory]
GO

CREATE procedure sp_cmreport_GetAgentHistory (@instance nvarchar(256), @startDate datetime, @endDate datetime, @sortColumn nvarchar(256), @rowCount int)
as
declare @stmt nvarchar(2000);
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);

	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

	set @stmt = 'SELECT TOP ' + STR(@rowCount) + ' t1.eventTime, t1.agentServer, t1.instance, t2.Name ''eventType'' ' +
		'from AgentEvents t1 LEFT OUTER JOIN AgentEventTypes t2 on t1.eventType=t2.eventId  ' +
		'where eventTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and eventTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') '

	if(@instance != '<ALL>')
		set @stmt = @stmt + ' and UPPER(t1.instance) = ''' + UPPER(@instance) + ''' '

	-- Descinding for time columns
	if(@sortColumn = 'date')
	   set @stmt = @stmt + ' ORDER BY t1.eventTime DESC';
	else
	   set @stmt = @stmt + ' ORDER BY t1.agentServer' ;
	   
	
	EXEC(@stmt)
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

grant execute on sp_cmreport_GetAgentHistory to public
GO
 
GO 

---------- SP_CMREPORT_GETALERTRULES.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetAlertRules' and xtype='P')
drop procedure [sp_cmreport_GetAlertRules]
GO

CREATE procedure sp_cmreport_GetAlertRules (
	@ruleName nvarchar(50), 
	@serverInstance nvarchar(50), 
	@alertLevel int, 
	@logMessage int, 
   @emailMessage int
)
as
begin

declare @string nvarchar(500)
declare @stmt nvarchar(2000)
declare @fromClause nvarchar(500)
declare @whereClause nvarchar(500)
declare @orderByClause nvarchar(200)

   -- prevents sql injection but set limitations on the database naming
   set @ruleName = dbo.fn_cmreport_ProcessString(@ruleName);
   set @serverInstance = UPPER(dbo.fn_cmreport_ProcessString(@serverInstance));

   set @string = 'SELECT name, alertLevel, targetInstances, logMessage, emailMessage FROM AlertRules ';

   set @whereClause = ' WHERE name LIKE (''' + @ruleName + ''') AND UPPER(targetInstances) LIKE (''' + @serverInstance + ''')';

   if(@alertLevel > 0)
      set @whereClause = @whereClause + ' AND alertLevel=' + STR(@alertLevel);
   if(@logMessage > -1)
   	set @whereClause = @whereClause + ' AND logMessage=' + STR(@logMessage);
   if(@emailMessage > -1)
   	set @whereClause = @whereClause + ' AND emailMessage = ' + STR(@emailMessage);

   set @orderByClause = ''

   set @stmt = @string + @whereClause + @orderByClause	

   EXEC(@stmt);
end
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

grant execute on sp_cmreport_GetAlertRules to public
GO
 
GO 

---------- SP_CMREPORT_GETALERTS.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetAlerts' and xtype='P')
drop procedure [sp_cmreport_GetAlerts]
GO

CREATE PROCEDURE sp_cmreport_GetAlerts( @eventDatabase nvarchar(256),
                                        @startDate datetime,
                                        @endDate datetime,
                                        @eventCategory int,
                                        @alertLevel int,
                                        @databaseName nvarchar(256),
                                        @objectName nvarchar(256),
                                        @privilegedUserOnly bit,
                                        @showSqlText bit,
                                        @sortColumn nvarchar(256),
                                        @rowCount int)
as
declare @stmt nvarchar(4000)
declare @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(1000), @orderByClause nvarchar(200)

declare @eventTable       nvarchar(256)
declare @eventSqlTable    nvarchar(256)
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);

    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

   -- prevents sql injection but set limitations on the database naming
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);
   set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));
   set @objectName = UPPER(dbo.fn_cmreport_ProcessString(@objectName));
	 
   -- table used in queries
	set @eventTable       = '[' + @eventDatabase + '].dbo.Events'
	set @eventSqlTable    = '[' + @eventDatabase + '].dbo.EventSQL'

	-- DEFINE COLUMNS FOR SELECT
	set @columns = 'SELECT TOP '+ STR(@rowCount) + ' ' +
   	'a.created ''alertTime'', ' +
   	'CASE a.alertLevel ' +
      	'WHEN 4 THEN ''Severe'' ' +
      	'WHEN 3 THEN ''High'' ' +
      	'WHEN 2 THEN ''Medium'' ' +
      	'WHEN 1 THEN ''Low'' ' +
      	'ELSE ''Unknown'' ' +
      	'END as alertLevel, ' +
   	'et.name ''eventType'', ' +
   	'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +
      	'THEN loginName ' + 
      	'ELSE sessionLoginName ' + 
      	'END AS loginName, ' +
   	'e.applicationName, ' +
   	'e.hostName, ' +
   	'e.databaseName, ' +
   	'e.targetObject, ' +
   	'e.details, ';
   	
   if(@showSqlText = 1)
      set @columns = @columns + 'es.sqlText ';
   else
      set @columns = @columns + ' '''' ''sqlText'' ';
   	
	-- Build FROM clause
	set @fromClause = 'FROM SQLcompliance..Alerts a LEFT OUTER JOIN ' + @eventTable + ' e ON a.alertEventId = e.eventId ';
	
	if (@showSqlText = 1)
	   set @fromClause = @fromClause + 'LEFT OUTER JOIN ' + @eventSqlTable + ' es ON e.eventId = es.eventId ' ;
	
	set @fromClause = @fromClause + 'LEFT OUTER JOIN SQLcompliance..EventTypes et ON a.eventType = et.evtypeid ' ;
	
	-- Build WHERE clause		
	set @whereClause = 'WHERE a.created >= CONVERT(DATETIME, ''' + @startDateStr + ''') ' +
   	'AND a.created <= CONVERT(DATETIME, ''' + @endDateStr + ''') ' +
   	'AND UPPER(databaseName) LIKE ''' + @databaseName + ''' ' +
   	'AND UPPER(e.targetObject) LIKE ''' + @objectName + ''' ';
	if( @eventCategory >= 0 )
	   set @whereClause = @whereClause + 'AND a.eventType=' + STR(@eventCategory)
	
	if ( @alertLevel <> 0 )
	   set @whereClause = @whereClause + ' AND a.alertLevel = ' + STR(@alertLevel)
	
	if (@privilegedUserOnly = 1)
	   set @whereClause = @whereClause + ' and privilegedUser = 1 '
	
	--set @whereClause = @whereClause + 'AND e.eventType = et.evtypeid '
	
	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY alertTime DESC';
	else
	   set @orderByClause = ' ORDER BY loginName' ;
	
	-- Build Complete SELECT statement
	set @stmt = @columns + ' ' + @fromClause + ' ' + @whereClause + ' ' + @orderByClause
	
	-- Execute SELECT
	EXEC(@stmt)
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


grant execute on sp_cmreport_GetAlerts to public
GO
 
GO 

---------- SP_CMREPORT_GETAPPLICATIONACTIVITY.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetApplicationActivity' and xtype='P')
drop procedure [sp_cmreport_GetApplicationActivity]
GO


CREATE procedure sp_cmreport_GetApplicationActivity (
	@eventDatabase nvarchar(256), 
	@databaseName nvarchar(256), 
	@applicationName nvarchar(256), 
	@eventCategory int, 
	@startDate datetime, 
   @endDate datetime, 
	@privilegedUserOnly bit, 
	@showSqlText bit, 
	@sortColumn nvarchar(256), 
	@rowCount int)
as
declare @stmt nvarchar(4000)
declare @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(1000), @orderByClause nvarchar(200)
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);

    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

   -- prevents sql injection but set limitations on the database naming
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);
   set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));
   set @applicationName = UPPER(dbo.fn_cmreport_ProcessString(@applicationName));

	
	set @columns = 'select top ' + STR(@rowCount) + ' applicationName, details, databaseName, t2.name ''eventType'', hostName, ' +
	               'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +
                       'THEN loginName ' + 
                       'ELSE sessionLoginName ' + 
                  'END AS loginName, ' +
	               'targetObject, t1.startTime'


   set @fromClause = 'FROM [' + @eventDatabase + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '
	if (@showSqlText = 1)
	begin
		set @columns = @columns + ', t3.sqlText '
		set @fromClause = @fromClause + 'LEFT OUTER JOIN [' + @eventDatabase + '].dbo.EventSQL t3 ON t1.eventId = t3.eventId '
	end
	else
	begin
		set @columns = @columns + ', '''' ''sqlText'' '
	end
	
	set @whereClause = 'where success =  1 and UPPER(t1.applicationName) like ''' + @applicationName + ''' and UPPER(databaseName) like ''' + @databaseName+ '''' +
	   ' and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') ';
	
	if(@eventCategory <> -1)
	   set @whereClause = @whereClause + ' and eventCategory=' + STR(@eventCategory) + ' ';

	if (@privilegedUserOnly = 1)
	begin
		set @whereClause = @whereClause + ' and privilegedUser = 1 '
	end

	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY t1.startTime DESC';
	else
	   set @orderByClause = ' ORDER BY loginName' ;
	
	set @stmt = @columns + @fromClause + @whereClause + @orderByClause

	EXEC(@stmt)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

grant execute on sp_cmreport_GetApplicationActivity to public
GO
 
GO 

---------- SP_CMREPORT_GETAPPLICATIONACTIVITYSUMMARY.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetApplicationActivitySummary' and xtype='P')
drop procedure [sp_cmreport_GetApplicationActivitySummary]
GO

CREATE PROC sp_cmreport_GetApplicationActivitySummary
(
	@eventDatabase nvarchar(256), 
	@databaseName nvarchar(256), 
	@applicationName nvarchar(256), 
	@eventCategory int, 
	@startDate datetime, 
   @endDate datetime, 
	@privilegedUserOnly bit, 
	@rowCount int)
AS
BEGIN
   -- Idera SQL compliance Manager Version 3.0
   --
   -- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
   -- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
   -- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.

-- Creates the temporary table to store the result of the procedure which returns 
-- the result from the original "get_application_activity" proceudre for us to 
-- work on the returned data
CREATE TABLE #ApplicationActivities
(
  ApplicationName	nvarchar (255)
, Details		nvarchar (300)
, DatabaseName		nvarchar (255)
, EventType		nvarchar (40)
, HostName		nvarchar (255)
, LoginName 		nvarchar (255)
, TargetObject		nvarchar (500)
, StartTime		DATETIME
, SqlText		nvarchar (10)
)


-- Gets the data from the Application Activity proc for, then, format
-- the data the way we need, because we don't want Databas, EventTypes,
-- Logins in separate columns, we want it to be a category for our
-- summarization.
INSERT #ApplicationActivities
EXEC sp_cmreport_GetApplicationActivity @eventDatabase
	,@databaseName
	,@applicationName
	,@eventCategory
	,@startDate
	,@endDate
	,@privilegedUserOnly
	,0
	,'date'
	,@rowCount

-- Now we develop the query to return the data we want devided by category.
SELECT  ApplicationName,'Grouped By Database' as 'Category', DatabaseName as 'Data', COUNT(DatabaseName) 'DataCount'
	FROM #ApplicationActivities
	--WHERE DatabaseName IS NOT NULL AND DatabaseName != ''
	GROUP BY
	ApplicationName, DatabaseName

UNION ALL
SELECT  ApplicationName, 'Grouped By EventType' as 'Category', EventType as 'Data', COUNT(EventType) 'DataCount'
	FROM #ApplicationActivities
	--WHERE EventType IS NOT NULL AND EventType != ''
	GROUP BY 
	ApplicationName, EventType
UNION ALL
SELECT  ApplicationName , 'Grouped By LoginName' as 'Category', LoginName as 'Data', COUNT(LoginName) 'DataCount'
	FROM #ApplicationActivities
	--WHERE LoginName IS NOT NULL AND LoginName != ''
	GROUP BY
	ApplicationName, LoginName
UNION ALL
SELECT  ApplicationName, 'Grouped By TargetObject' as 'Category', TargetObject as 'Data', COUNT(TargetObject) 'DataCount'
	FROM #ApplicationActivities
	--WHERE TargetObject IS NOT NULL AND TargetObject != ''
	GROUP BY ApplicationName, TargetObject

-- After using it, we now drop our temporary table to free memory space
DROP TABLE #ApplicationActivities

END
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

grant execute on sp_cmreport_GetApplicationActivitySummary to public
GO
 
GO 

---------- SP_CMREPORT_GETAUDITACTIVITYGRAPH.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetAuditActivityGraph' and xtype='P')
drop procedure [sp_cmreport_GetAuditActivityGraph]
GO

CREATE procedure sp_cmreport_GetAuditActivityGraph (
	@eventDatabase nvarchar(256), 
	@startDate datetime, 
	@endDate datetime, 
	@databaseName nvarchar(50)
)
as
begin

declare @stmt nvarchar(2000)
declare @selectClause nvarchar(2000)
declare @fromClause nvarchar(500)
declare @whereClause nvarchar(1000)
declare @groupbyClause nvarchar(500)
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);

    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

   -- prevents sql injection but set limitations on the database naming
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);
   set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));

set @selectClause = 'SELECT 
    DateAdd(Millisecond, -DatePart(MilliSecond, startTime), DateAdd(Second, -DatePart(Second, startTime), DateAdd(Minute, -DatePart(Minute, startTime),startTime)))  UTCCollectionDateTime,  
    COUNT(DateAdd(Millisecond, -DatePart(MilliSecond, startTime), DateAdd(Second, -DatePart(Second, startTime), DateAdd(Minute, -DatePart(Minute, startTime),startTime))))  DataCount'
set @fromClause = ' FROM [' + @eventDatabase + '].dbo.Events'
set @whereClause = ' WHERE startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') ';
if @databaseName <> '%'
    set @whereClause = @whereClause + ' AND  UPPER(databaseName) like ''' + @databaseName + ''''
set @groupbyClause = ' GROUP BY (DateAdd(Millisecond, -DatePart(MilliSecond, startTime), DateAdd(Second, -DatePart(Second, startTime), DateAdd(Minute, -DatePart(Minute, startTime),startTime)))) ORDER BY UTCCollectionDateTime'

set @stmt = @selectClause + @fromClause + @whereClause + @groupbyClause

EXEC(@stmt)
end
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

grant execute on sp_cmreport_GetAuditActivityGraph to public
go
 
GO 

---------- SP_CMREPORT_GETAUDITCONTROLCHANGES.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetAuditControlChanges' and xtype='P')
drop procedure [sp_cmreport_GetAuditControlChanges]
GO


CREATE procedure sp_cmreport_GetAuditControlChanges 
(
   @startDate datetime, 
   @endDate datetime, 
   @sortColumn nvarchar(256), 
   @rowCount int)
as
declare @stmt nvarchar(4000), @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(1000), @orderByClause nvarchar(200)
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);

   -- Idera SQL compliance Manager Version 3.0
   --
   -- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
   -- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
   -- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.

    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

	set @columns = 'select top '+ STR(@rowCount) +' eventTime, t2.Name, logUser, logSqlServer, logInfo  '
	set @fromClause = 'from ChangeLog t1 LEFT OUTER JOIN ChangeLogEventTypes t2 ON t1.logType = t2.eventId '
	set @whereClause = 'where eventTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and eventTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') ';

	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY eventTime DESC';
	else
	   set @orderByClause = ' ORDER BY logUser' ;
	
	set @stmt = @columns + @fromClause + @whereClause + @orderByClause

	EXEC(@stmt)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

grant execute on sp_cmreport_GetAuditControlChanges to public
GO
 
GO 

---------- SP_CMREPORT_GETBEFOREAFTERDATA.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetBeforeAfterData' and xtype='P')
drop procedure [sp_cmreport_GetBeforeAfterData]
GO


CREATE procedure sp_cmreport_GetBeforeAfterData (
   @eventDatabase nvarchar(256), 
   @databaseName nvarchar(256), 
   @loginName nvarchar(256), 
   @objectName nvarchar(256), 
   @schemaName nvarchar(256), 
   @startDate nvarchar(50), 
   @endDate nvarchar(50), 
   @sortColumn nvarchar(1000), 
   @rowCount nvarchar(16),
   @primaryKey nvarchar(512)
   )
as
declare @stmt nvarchar(4000), @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(1000), @orderByClause nvarchar(200)
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);

    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

	 -- prevents sql injection but set limitations on the database naming
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);
   set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));
   set @loginName = UPPER(dbo.fn_cmreport_ProcessString(@loginName));
   set @schemaName = UPPER(dbo.fn_cmreport_ProcessString(@schemaName));
   set @objectName = UPPER(dbo.fn_cmreport_ProcessString(@objectName));
   set @primaryKey = UPPER(dbo.fn_cmreport_ProcessString(@primaryKey));

	set @columns = 'select top '+ STR(@rowCount) +' e.applicationName, e.databaseName, ' +
	               'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +
                       'THEN loginName ' + 
                       'ELSE sessionLoginName ' + 
                  'END AS loginName, ' +
                  'CASE WHEN eventType=8 THEN ''Insert'' ' +
                  'WHEN eventType=2 THEN ''Update'' ' +
                  'WHEN eventType=16 THEN ''Delete'' ' +
                  'ELSE ''Unknown'' END AS eventTypeString, ' +
	               'e.startTime, e.targetObject, d.primaryKey,c.columnName,c.beforeValue,c.afterValue '
	               
   set @fromClause = 'FROM [' + @eventDatabase + '].dbo.Events as e ' + 
    'JOIN [' + @eventDatabase + '].dbo.DataChanges as d ON (d.startTime >= e.startTime AND d.startTime <= e.endTime ' +
    'AND d.eventSequence >= e.startSequence AND d.eventSequence <= e.endSequence) ' +
    'LEFT OUTER JOIN [' + @eventDatabase + '].dbo.ColumnChanges as c ON (c.startTime = d.startTime AND d.eventSequence = c.eventSequence)'
	
	set @whereClause = 'where UPPER(databaseName) like ''' + @databaseName+ ''' ' +
	                   'AND (UPPER(sessionLoginName) LIKE ''' + @loginName + ''' OR UPPER(loginName) LIKE ''' + @loginName + ''') ' +
					   'AND UPPER(schemaName) like ''' + @schemaName + ''' ' +
                      'AND UPPER(targetObject) like ''' + @objectName + ''' ' +
					  'AND UPPER(primaryKey) like ''' +'%'+ @primaryKey+'%' + ''' ' +
	                   'and eventType IN (8,2,16) ' +
	                   ' and e.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and e.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') '

	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY e.startTime DESC';
	else
	   set @orderByClause = ' ORDER BY loginName' ;
	
	set @stmt = @columns + @fromClause + @whereClause + @orderByClause

	EXEC(@stmt)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

grant execute on sp_cmreport_GetBeforeAfterData to public
GO
 
GO 

---------- SP_CMREPORT_GETHOSTACTIVITY.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetHostActivity' and xtype='P')
drop procedure [sp_cmreport_GetHostActivity]
GO


CREATE procedure sp_cmreport_GetHostActivity (
	@eventDatabase nvarchar(256), 
	@databaseName nvarchar(256), 
	@loginName nvarchar(256), 
	@hostName nvarchar(256), 
	@eventCategory int, 
	@startDate datetime, 
   @endDate datetime, 
	@privilegedUserOnly bit, 
	@showSqlText bit, 
	@sortColumn nvarchar(256), 
	@rowCount int)
as
declare @stmt nvarchar(4000)
declare @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(1000), @orderByClause nvarchar(200)
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);

   -- Idera SQL compliance Manager Version 3.0
   --
   -- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
   -- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
   -- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.

    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

	 -- prevents sql injection but set limitations on the database naming
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);
   set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));
   set @loginName = UPPER(dbo.fn_cmreport_ProcessString(@loginName));
   set @hostName = UPPER(dbo.fn_cmreport_ProcessString(@hostName));

	set @columns = 'select top ' + STR(@rowCount) + ' databaseName, applicationName, t2.name ''eventType'', hostName, details, ' +
	               'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +
                       'THEN loginName ' + 
                       'ELSE sessionLoginName ' + 
                  'END AS loginName, ' +
	               't1.startTime,targetObject'


   set @fromClause = 'FROM [' + @eventDatabase + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '
	if (@showSqlText = 1)
	begin
		set @columns = @columns + ', t3.sqlText '
		set @fromClause = @fromClause +    'LEFT OUTER JOIN [' + @eventDatabase + '].dbo.EventSQL t3 ON t1.eventId = t3.eventId '
	end
	else
	begin
		set @columns = @columns + ', '''' ''sqlText'' '
	end
	
	set @whereClause = 'where UPPER(t1.hostName) like ''' + @hostName + ''' and success = 1  and UPPER(databaseName) like ''' + @databaseName + 
	   ''' and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') ' +
      'AND (UPPER(sessionLoginName) LIKE ''' + @loginName + ''' OR UPPER(loginName) LIKE ''' + @loginName + ''') ';
	   

	if(@eventCategory <> -1)
	   set @whereClause = @whereClause + ' and eventCategory=' + STR(@eventCategory) + ' ';

	if (@privilegedUserOnly = 1)
	begin
		set @whereClause = @whereClause + ' and privilegedUser = 1 '
	end

	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY t1.startTime DESC';
	else
	   set @orderByClause = ' ORDER BY loginName' ;
	
	set @stmt = @columns + @fromClause + @whereClause + @orderByClause

	EXEC(@stmt)


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

grant execute on sp_cmreport_GetHostActivity to public
GO
 
GO 

---------- SP_CMREPORT_GETINTEGRITYCHECK.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetIntegrityCheck' and xtype='P')
drop procedure [sp_cmreport_GetIntegrityCheck]
GO


create procedure sp_cmreport_GetIntegrityCheck (
   @eventDatabase varchar(256), 
   @startDate datetime, 
   @endDate datetime, 
   @showSqlText bit, 
   @sortColumn nvarchar(256),
   @rowCount int)
as
declare @stmt nvarchar(4000)
declare @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(1000), @orderByClause nvarchar(200)
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);

   -- Idera SQL compliance Manager Version 3.0
   --
   -- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
   -- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
   -- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.

    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

	 -- prevents sql injection but set limitations on the database naming
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);


	set @columns = 'select top ' + STR(@rowCount)+ ' databaseName, applicationName, t2.name ''eventType'', hostName, details, ' +
	               'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +
                       'THEN loginName ' + 
                       'ELSE sessionLoginName ' + 
                  'END AS loginName, ' +
	               't1.startTime'


   set @fromClause = 'FROM [' + @eventDatabase + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '

	if (@showSqlText = 1)
	begin
		set @columns = @columns + ', t3.sqlText, targetObject '
		set @fromClause = @fromClause +    'LEFT OUTER JOIN [' + @eventDatabase + '].dbo.EventSQL t3 ON t1.eventId = t3.eventId ';
	end
	else
	begin
		set @columns = @columns + ', '''' ''sqlText'', targetObject '
	end

	set @whereClause = 'where eventCategory = 0 and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') '
	
	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY t1.startTime DESC';
	else
	   set @orderByClause = ' ORDER BY loginName' ;
	
	set @stmt = @columns + @fromClause + @whereClause + @orderByClause

	EXEC(@stmt)


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

grant execute on sp_cmreport_GetIntegrityCheck to public
GO
 
GO 

---------- SP_CMREPORT_GETLOGINCREATIONHISTORY.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetLoginCreationHistory' and xtype='P')
drop procedure [sp_cmreport_GetLoginCreationHistory]
GO

CREATE procedure sp_cmreport_GetLoginCreationHistory (
   @eventDatabase nvarchar(256), 
   @loginName nvarchar(256), 
   @startDate datetime, 
   @endDate datetime, 
   @sortColumn nvarchar(256), 
   @rowCount int)
as
declare @stmt nvarchar(4000), @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(1000), @orderByClause nvarchar(200), @eventTypeString nvarchar(200)
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);

   -- Idera SQL compliance Manager Version 3.0
   --
   -- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
   -- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
   -- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.

	-- set category type
	set @eventTypeString = '710,717,143,144,139,146,147,169'	

    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

	 -- prevents sql injection but set limitations on the database naming
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);
   set @loginName = UPPER(dbo.fn_cmreport_ProcessString(@loginName));

	set @columns = 'select top '+ STR(@rowCount) + ' applicationName, hostName, CASE eventType WHEN 717 THEN targetLoginName WHEN 710 then targetLoginName ELSE targetObject END as ''targetLoginName'', loginName, startTime '
	set @fromClause = 'from [' + @eventDatabase + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '
	set @whereClause = 'where success = 1 and eventCategory=3 and evtypeid in (' + @eventTypeString + ') and upper(t1.targetLoginName) like ''' + @loginName + ''' and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') ';

	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY t1.startTime DESC';
	else
	   set @orderByClause = ' ORDER BY loginName' ;
	
	set @stmt = @columns + @fromClause + @whereClause + @orderByClause

	EXEC(@stmt)

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

grant execute on sp_cmreport_GetLoginCreationHistory to public
GO
 
GO 

---------- SP_CMREPORT_GETLOGINDELETIONHISTORY.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetLoginDeletionHistory' and xtype='P')
drop procedure [sp_cmreport_GetLoginDeletionHistory]
GO


CREATE procedure sp_cmreport_GetLoginDeletionHistory (
   @eventDatabase nvarchar(256), 
   @loginName nvarchar(256), 
   @startDate datetime, 
   @endDate datetime, 
   @sortColumn nvarchar(256), 
   @rowCount int)
as
declare @stmt nvarchar(4000), @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(1000), @orderByClause nvarchar(200), @eventTypeString nvarchar(200)
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);

   -- Idera SQL compliance Manager Version 3.0
   --
   -- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
   -- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
   -- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.

	-- set category type
	set @eventTypeString = '711,718,346,347,343,344,339,369'	


	 -- prevents sql injection but set limitations on the database naming
    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

	 -- prevents sql injection but set limitations on the database naming
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);
   set @loginName = UPPER(dbo.fn_cmreport_ProcessString(@loginName));

	set @columns = 'select top '+ STR(@rowCount) +' applicationName, hostName, loginName, startTime, targetObject as targetLoginName '
	set @fromClause = 'from [' + @eventDatabase + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '
	set @whereClause = 'where success = 1 and eventCategory=3 and evtypeid in (' + @eventTypeString + ') and upper(t1.targetLoginName) like ''' + @loginName + ''' and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') ';

	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY t1.startTime DESC';
	else
	   set @orderByClause = ' ORDER BY loginName' ;
	
	set @stmt = @columns + @fromClause + @whereClause + @orderByClause

	EXEC(@stmt)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

grant execute on sp_cmreport_GetLoginDeletionHistory to public
GO
 
GO 

---------- SP_CMREPORT_GETLOGINHISTORY.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetLoginHistory' and xtype='P')
drop procedure [sp_cmreport_GetLoginHistory]
GO


CREATE procedure sp_cmreport_GetLoginHistory (
   @eventDatabase nvarchar(256), 
   @loginStatus int, 
   @loginName nvarchar(256), 
   @startDate datetime, 
   @endDate datetime, 
   @sortColumn nvarchar(256), 
   @rowCount int
   )
as
declare @stmt nvarchar(4000), @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(1000), @orderByClause nvarchar(200), @eventTypeString nvarchar(200)
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);

	if (@loginStatus = 1)
		set @eventTypeString = '50'
	else if (@loginStatus = 2)
		set @eventTypeString = '51'
	else
		set @eventTypeString = '50, 51'	

    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

	 -- prevents sql injection but set limitations on the database naming
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);
   set @loginName = UPPER(dbo.fn_cmreport_ProcessString(@loginName));

	set @columns = 'SELECT TOP '+ STR(@rowCount) +' applicationName, t2.name ''eventType'', hostName, ' +
	               'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +
                       'THEN loginName ELSE sessionLoginName END AS loginName, ' +
	               'startTime '
	               
   set @fromClause = 'FROM [' + @eventDatabase + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '
	
	set @whereClause = 'WHERE evtypeid in (' + @eventTypeString + ') ' +
	                   'AND eventCategory=1 ' +
	                   'AND (UPPER(sessionLoginName) LIKE ''' + @loginName + ''' OR UPPER(loginName) LIKE ''' + @loginName + ''') ' +
	                   ' and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') '

	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY t1.startTime DESC';
	else
	   set @orderByClause = ' ORDER BY loginName' ;
			
	set @stmt = @columns + @fromClause + @whereClause + @orderByClause

	EXEC(@stmt)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

grant execute on sp_cmreport_GetLoginHistory to public
GO
 
GO 

---------- SP_CMREPORT_GETMASSDATAACTIVITY.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetMassDataActivity' and xtype='P')
drop procedure [sp_cmreport_GetMassDataActivity]
GO


CREATE procedure sp_cmreport_GetMassDataActivity (
   @eventDatabase nvarchar(256), 
   @startDate datetime, 
   @endDate datetime, 
   @privilegedUserOnly bit, 
   @showSqlText bit, 
   @sortColumn nvarchar(256), 
   @rowCount int)
as
declare @stmt nvarchar(4000)
declare @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(1000), @orderByClause nvarchar(200), @category nvarchar(50), @eventTypeString nvarchar(200)
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);
	
   -- Idera SQL compliance Manager Version 3.0
   --
   -- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
   -- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
   -- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.

	-- set category type
	set @eventTypeString = '62, 63, 64, 65, 80, 81, 82'

    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

	 -- prevents sql injection but set limitations on the database naming
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);

	set @columns = 'select top '+ STR(@rowCount) +' applicationName, databaseName, t2.name ''eventType'', hostName, ' +
	               'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +
                       'THEN loginName ' + 
                       'ELSE sessionLoginName ' + 
                  'END AS loginName, ' +
	               'targetObject, t1.startTime'

   set @fromClause = 'FROM [' + @eventDatabase + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '
	if (@showSqlText = 1)
	begin
		set @columns = @columns + ', t3.sqlText '
		set @fromClause = @fromClause + 'LEFT OUTER JOIN [' + @eventDatabase + '].dbo.EventSQL t3 ON t1.eventId = t3.eventId '
	end
	else
	begin
		set @columns = @columns + ', '''' ''sqlText'' '
	end
	
	set @whereClause = 'where success = 1 and eventCategory=6 and evtypeid in (' + @eventTypeString + ') and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') ';
	
	if (@privilegedUserOnly = 1)
	begin
		set @whereClause = @whereClause + ' and privilegedUser = 1 '
	end

	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY t1.startTime DESC';
	else
	   set @orderByClause = ' ORDER BY loginName' ;
	
	set @stmt = @columns + @fromClause + @whereClause + @orderByClause

	EXEC(@stmt)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



grant execute on sp_cmreport_GetMassDataActivity to public
GO
 
GO 

---------- SP_CMREPORT_GETOBJECTACTIVITY.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetObjectActivity' and xtype='P')
drop procedure [sp_cmreport_GetObjectActivity]
GO


CREATE procedure sp_cmreport_GetObjectActivity (
	@eventDatabase nvarchar(256), 
	@databaseName nvarchar(256), 
	@objectName nvarchar(256), 
	@eventCategory int, 
	@startDate datetime, 
   @endDate datetime, 
	@privilegedUserOnly bit, 
	@showSqlText bit, 
	@sortColumn nvarchar(256), 
	@rowCount int)
as
declare @stmt nvarchar(4000)
declare @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(1000), @orderByClause nvarchar(200)
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);

    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

   -- prevents sql injection but set limitations on the database naming
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);
   set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));
   set @objectName = UPPER(dbo.fn_cmreport_ProcessString(@objectName));

	
	set @columns = 'select top ' + STR(@rowCount) + ' applicationName, details, databaseName, t2.name ''eventType'', hostName, ' +
	               'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +
                       'THEN loginName ' + 
                       'ELSE sessionLoginName ' + 
                  'END AS loginName, ' +
	               'targetObject, t1.startTime'


   set @fromClause = 'FROM [' + @eventDatabase + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '
	if (@showSqlText = 1)
	begin
		set @columns = @columns + ', t3.sqlText '
		set @fromClause = @fromClause + 'LEFT OUTER JOIN [' + @eventDatabase + '].dbo.EventSQL t3 ON t1.eventId = t3.eventId '
	end
	else
	begin
		set @columns = @columns + ', '''' ''sqlText'' '
	end
	
	set @whereClause = 'where success =  1 and UPPER(t1.targetObject) like ''' + @objectName + ''' and UPPER(databaseName) like ''' + @databaseName+ '''' +
	   ' and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') ';
	
	if(@eventCategory <> -1)
	   set @whereClause = @whereClause + ' and eventCategory=' + STR(@eventCategory) + ' ';

	if (@privilegedUserOnly = 1)
	begin
		set @whereClause = @whereClause + ' and privilegedUser = 1 '
	end

	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY t1.startTime DESC';
	else
	   set @orderByClause = ' ORDER BY loginName' ;

	set @stmt = @columns + @fromClause + @whereClause + @orderByClause

	EXEC(@stmt)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


grant execute on sp_cmreport_GetObjectActivity to public
GO
 
GO 

---------- SP_CMREPORT_GETOBJECTPRIVCHANGES.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetObjectPrivChanges' and xtype='P')
drop procedure [sp_cmreport_GetObjectPrivChanges]
GO


CREATE procedure sp_cmreport_GetObjectPrivChanges (
   @eventDatabase nvarchar(256), 
   @databaseName nvarchar(256), 
   @objectName nvarchar(256), 
   @startDate datetime, 
   @endDate datetime, 
   @privilegedUserOnly bit, 
   @sortColumn nvarchar(256),
   @rowCount int)
as
declare @stmt nvarchar(4000), @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(1000), @orderByClause nvarchar(200)
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);

   -- Idera SQL compliance Manager Version 3.0
   --
   -- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
   -- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
   -- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.

    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

	 -- prevents sql injection but set limitations on the database naming
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);
   set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));
   set @objectName = UPPER(dbo.fn_cmreport_ProcessString(@objectName));

	set @columns = 'SELECT TOP '+ STR(@rowCount) +' applicationName, databaseName, t2.name ''eventType'', hostName, ' +
	               'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +
                       'THEN loginName ' + 
                       'ELSE sessionLoginName ' + 
                  'END AS loginName, ' +
	               'targetObject, startTime, details '
	
	set @fromClause = 'from [' + @eventDatabase + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '
	
	set @whereClause = 'where success = 1 and UPPER(databaseName) like ''' + @databaseName+ ''' and UPPER(targetObject) like ''' + @objectName + ''' and eventCategory=3 and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') ';

	if (@privilegedUserOnly = 1)
	begin
		set @whereClause = @whereClause + ' and privilegedUser = 1 '
	end

	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY t1.startTime DESC';
	else
	   set @orderByClause = ' ORDER BY loginName' ;
	
	set @stmt = @columns + @fromClause + @whereClause + @orderByClause

	EXEC(@stmt)
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

grant execute on sp_cmreport_GetObjectPrivChanges to public
GO
 
GO 

---------- SP_CMREPORT_GETPERMISSIONDENIED.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetPermissionDenied' and xtype='P')
drop procedure [sp_cmreport_GetPermissionDenied]
GO

CREATE  procedure sp_cmreport_GetPermissionDenied (
	@eventDatabase nvarchar(256), 
	@databaseName nvarchar(256), 
	@loginName nvarchar(256), 
	@eventCategory int, 
	@startDate datetime, 
	@endDate datetime, 
	@privilegedUserOnly bit, 
	@showSqlText bit, 
	@sortColumn nvarchar(256), 
	@rowCount int)
as
declare @stmt nvarchar(1000)
declare @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(1000), @orderByClause nvarchar(200), @category nvarchar(200)
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);

   -- Idera SQL compliance Manager Version 3.0
   --
   -- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
   -- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
   -- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.

    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

	 -- prevents sql injection but set limitations on the database naming
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);
   set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));
   set @loginName = UPPER(dbo.fn_cmreport_ProcessString(@loginName));

	set @columns = 'SELECT TOP ' +STR(@rowCount) + ' applicationName, databaseName, t2.name ''eventType'', hostName, details, ' +
	               'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +
                       'THEN UPPER(loginName) ' + 
                       'ELSE UPPER(sessionLoginName) ' + 
                  'END AS loginName, ' +
	               'targetObject, t1.startTime'

   set @fromClause = 'FROM [' + @eventDatabase + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '
	if (@showSqlText = 1)
	begin
		set @columns = @columns + ', t3.sqlText '
		set @fromClause = @fromClause + 'LEFT OUTER JOIN [' + @eventDatabase + '].dbo.EventSQL t3 ON t1.eventId = t3.eventId '
	end
	else
	begin
		set @columns = @columns + ', '''' ''sqlText'' '
	end

	set @whereClause = 'WHERE success = 0 ' +
	                   'AND UPPER(databaseName) like ''' + @databaseName+ ''' ' +
	                   'AND (UPPER(sessionLoginName) LIKE ''' + @loginName + ''' OR UPPER(loginName) LIKE ''' + @loginName + ''') ' +
	                   ' and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') '

	if(@eventCategory <> -1)
	   set @whereClause = @whereClause + ' and eventCategory=' + STR(@eventCategory) + ' ';

	if (@privilegedUserOnly = 1)
	begin
		set @whereClause = @whereClause + ' and privilegedUser = 1 '
	end

	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY t1.startTime DESC';
	else
	   set @orderByClause = ' ORDER BY loginName' ;
	
	set @stmt = @columns + @fromClause + @whereClause + @orderByClause

	EXEC(@stmt)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

grant execute on sp_cmreport_GetPermissionDenied to public
GO
 
GO 

---------- SP_CMREPORT_GETSCHEMACHANGEHISTORY.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetSchemaChangeHistory' and xtype='P')
drop procedure [sp_cmreport_GetSchemaChangeHistory]
GO


CREATE procedure sp_cmreport_GetSchemaChangeHistory (
   @eventDatabase nvarchar(256), 
   @databaseName nvarchar(256), 
   @loginName nvarchar(256), 
   @startDate datetime, 
   @endDate datetime, 
   @privilegedUserOnly bit, 
   @showSqlText bit, 
   @sortColumn nvarchar(256), 
   @rowCount int
   )
as
declare @stmt nvarchar(4000)
declare @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(1000), @orderByClause nvarchar(200)
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);

    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

	 -- prevents sql injection but set limitations on the database naming
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);
   set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));
   set @loginName = UPPER(dbo.fn_cmreport_ProcessString(@loginName));

	-- only one event database and only if it exists and available
	set @columns = 'select  top '+ STR(@rowCount) + ' databaseName, applicationName, t2.name ''eventType'', hostName, ' +
	               'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +
                       'THEN loginName ' + 
                       'ELSE sessionLoginName ' + 
                  'END AS loginName, ' +
	               't1.startTime, ' +
	               't1.targetObject '

   set @fromClause = 'FROM [' + @eventDatabase + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '

	if (@showSqlText = 1)
	begin
		set @columns = @columns + ', t3.sqlText '
		set @fromClause = @fromClause + 'LEFT OUTER JOIN [' + @eventDatabase + '].dbo.EventSQL t3 ON t1.eventId = t3.eventId '
	end
	else
	begin
		set @columns = @columns + ', '''' ''sqlText'' '
	end

   set @whereClause = 'WHERE UPPER(databaseName) like ''' + @databaseName + ''' ' +
	                   'AND (UPPER(sessionLoginName) LIKE ''' + @loginName + ''' OR UPPER(loginName) LIKE ''' + @loginName + ''') ' +
                      ' and eventCategory=2' +
	                   ' and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') '

	if (@privilegedUserOnly = 1)
	begin
		set @whereClause = @whereClause + ' and privilegedUser = 1 '
	end

	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY t1.startTime DESC';
	else
	   set @orderByClause = ' ORDER BY loginName' ;

	set @stmt = @columns + @fromClause + @whereClause + @orderByClause

	EXEC(@stmt)
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

grant execute on sp_cmreport_GetSchemaChangeHistory to public
GO
 
GO 

---------- SP_CMREPORT_GETSERVERLOGINACTIVITY.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetServerLoginActivity' and xtype='P')
drop procedure [sp_cmreport_GetServerLoginActivity]
GO


CREATE procedure sp_cmreport_GetServerLoginActivity (
	@eventDatabase nvarchar(256), 
	@loginStatus int, 
	@startDate datetime, 
   @endDate datetime, 
	@sortColumn nvarchar(256),
	@rowCount int)
as
	-- Creates the temporary table to store the result of the procedure which returns 
   -- the result from the original "get_application_activity" proceudre for us to 
   -- work on the returned data
   CREATE TABLE #UserLoginHistory
   (
      ApplicationName	NVARCHAR (255),
      EventType		int,
      LoginName		NVARCHAR (255),
      StartTime		DATETIME
   )


   -- Gets the data from the Application Activity proc for, then, format
   -- the data the way we need, because we don't want Databas, EventTypes,
   -- Logins in separate columns, we want it to be a category for our
   -- summarization.

   INSERT #UserLoginHistory EXEC sp_cmreport_GetServerLoginActivityHelper
   	@eventDatabase, 
	   @loginStatus, 
	   @startDate, 
      @endDate, 
	   @sortColumn,
	   @rowCount;

   -- Now we develop the query to return the data we want devided by category.
   SELECT  LoginName, EventType, ApplicationName, COUNT(StartTime) as DataCount
   	FROM #UserLoginHistory
   	GROUP BY
   	LoginName, EventType, ApplicationName

   -- After using it, we now drop our temporary table to free memory space
   DROP TABLE #UserLoginHistory
GO


-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetServerLoginActivityHelper' and xtype='P')
drop procedure [sp_cmreport_GetServerLoginActivityHelper]
GO


CREATE procedure sp_cmreport_GetServerLoginActivityHelper (
	@eventDatabase nvarchar(256), 
	@loginStatus int, 
	@startDate datetime, 
   @endDate datetime, 
	@sortColumn nvarchar(256),
	@rowCount int)
as
declare @string nvarchar(4000), @columns nvarchar(1000), @fromClause nvarchar(500), @whereClause nvarchar(2000), @orderByClause nvarchar(200);
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);

    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

   -- prevents sql injection but set limitations on the database naming
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);


	set @columns = 'select top ' + STR(@rowCount) + ' applicationName, eventType, ' +
	               'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +
                       'THEN loginName ' + 
                       'ELSE sessionLoginName ' + 
                  'END AS loginName, ' +
                  'startTime ';
                  
   set @fromClause = 'FROM [' + @eventDatabase + '].dbo.Events  ';
	
	set @whereClause = 'where startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') ';
	
	if (@loginStatus = 1)
		set @whereClause = @whereClause + ' and eventType=50 ';
	else if (@loginStatus = 2)
		set @whereClause = @whereClause + ' and eventType=51 ';
	else
	   set @whereClause = @whereClause + ' and (eventType=50 or eventType=51) ';

	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY startTime DESC';
	else
	   set @orderByClause = ' ORDER BY loginName' ;
	
	set @string = @columns + @fromClause + @whereClause + @orderByClause ;

   print @string;
   EXEC(@string);

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

grant execute on sp_cmreport_GetServerLoginActivity to public
GO

grant execute on sp_cmreport_GetServerLoginActivityHelper to public
GO
 
GO 

---------- SP_CMREPORT_GETUSERACTIVITY.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetUserActivity' and xtype='P')
drop procedure [sp_cmreport_GetUserActivity]
GO


CREATE procedure sp_cmreport_GetUserActivity (
	@eventDatabase nvarchar(256), 
	@databaseName nvarchar(256), 
	@loginName nvarchar(256), 
	@eventCategory int, 
	@startDate datetime, 
	@endDate datetime, 
	@privilegedUserOnly bit, 
	@showSqlText bit, 
	@sortColumn nvarchar(256), 
	@rowCount int)
as
declare @stmt nvarchar(4000)
declare @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(1000), @orderByClause nvarchar(200)
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);

    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

	 -- prevents sql injection but set limitations on the database naming
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);
   set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));
   set @loginName = UPPER(dbo.fn_cmreport_ProcessString(@loginName));


	set @columns = 'SELECT TOP '+ STR(@rowCount) +' applicationName, databaseName, t2.name ''eventType'', hostName, details, ' +
	               'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +
                       'THEN loginName ' + 
                       'ELSE sessionLoginName ' +
                  'END AS loginName, ' +
	               'targetObject, t1.startTime'

   set @fromClause = 'FROM [' + @eventDatabase + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '

	if (@showSqlText = 1)
	begin
		set @columns = @columns + ', t3.sqlText '
		set @fromClause = @fromClause + 'LEFT OUTER JOIN [' + @eventDatabase + '].dbo.EventSQL t3 ON t1.eventId = t3.eventId '
	end
	else
	begin
		set @columns = @columns + ', '''' ''sqlText'' '
	end
	
	set @whereClause = 'WHERE success = 1 ' +
	                   'AND UPPER(databaseName) like ''' + @databaseName+ ''' ' +
	                   'AND (UPPER(sessionLoginName) LIKE ''' + @loginName + ''' OR UPPER(loginName) LIKE ''' + @loginName + ''') ' +
	                   ' and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') '


	if(@eventCategory <> -1)
	   set @whereClause = @whereClause + ' and eventCategory=' + STR(@eventCategory) + ' ';

	if (@privilegedUserOnly = 1)
	begin
		set @whereClause = @whereClause + ' and privilegedUser = 1 '
	end

	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY t1.startTime DESC';
	else
	   set @orderByClause = ' ORDER BY loginName' ;

	set @stmt = @columns + @fromClause + @whereClause + @orderByClause

	EXEC(@stmt)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

grant execute on sp_cmreport_GetUserActivity to public
GO
 
GO 

---------- SP_CMREPORT_GETUSERCHANGES.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetUserChanges' and xtype='P')
drop procedure [sp_cmreport_GetUserChanges]
GO


CREATE procedure sp_cmreport_GetUserChanges (
   @eventDatabase nvarchar(256), 
   @databaseName nvarchar(256), 
   @loginName nvarchar(1000), 
   @startDate nvarchar(50), 
   @endDate nvarchar(50), 
   @sortColumn nvarchar(1000), 
   @rowCount nvarchar(16)
   )
as
declare @stmt nvarchar(4000), @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(1000), @orderByClause nvarchar(200)
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);

    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

	 -- prevents sql injection but set limitations on the database naming
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);
   set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));
   set @loginName = UPPER(dbo.fn_cmreport_ProcessString(@loginName));

	set @columns = 'select top '+ STR(@rowCount) +' applicationName, databaseName, t2.name ''eventType'', details, hostName, ' +
	               'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +
                       'THEN loginName ' + 
                       'ELSE sessionLoginName ' + 
                  'END AS loginName, ' +
	               'startTime, targetObject '
	               
   set @fromClause = 'FROM [' + @eventDatabase + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '
	
	set @whereClause = 'where UPPER(databaseName) like ''' + @databaseName+ ''' ' +
	                   'AND (UPPER(sessionLoginName) LIKE ''' + @loginName + ''' OR UPPER(loginName) LIKE ''' + @loginName + ''') ' +
	                   'and eventCategory=3 ' +
	                   ' and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') '

	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY t1.startTime DESC';
	else
	   set @orderByClause = ' ORDER BY loginName' ;
	
	set @stmt = @columns + @fromClause + @whereClause + @orderByClause

	EXEC(@stmt)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

grant execute on sp_cmreport_GetUserChanges to public
GO
 
GO 

---------- SP_ISP_SQLCM_ADDLICENSE.SQL
USE SQLcompliance
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[isp_sqlcm_addlicense]'))
drop procedure [dbo].[isp_sqlcm_addlicense]
GO

CREATE procedure [dbo].[isp_sqlcm_addlicense] (@licensekey nvarchar(256))
as

   -- (c) Copyright 2004-2006 Idera, a division of BBS Technologies, Inc., all rights reserved.
   -- SQLsecure, Idera and the Idera Logo are trademarks or registered trademarks
   -- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
   --
   -- Description :
   --              Insert a single license
   -- 	           
	
	declare @result int
	declare @err int
	declare @errmsg nvarchar(500)
	declare @ans int

	-- Get application program name
	declare @programname nvarchar(128)
	select @programname = program_name from master..sysprocesses where spid= @@spid and sid = SUSER_SID(SYSTEM_USER)

	declare @connectionname nvarchar(128)
	set @connectionname = NULL
		
	if (@licensekey IS NULL)
	begin
		set @errmsg = N'License key cannot be null.'
		RAISERROR (@errmsg, 16, 1)
		return -1
	end 
	
	declare @mxlen int

	if (LEN(@licensekey) > 256)
	begin
		set @errmsg = N'License key cannot be longer than 256 characters.'
		RAISERROR (@errmsg, 16, 1)
		return -1
	end
	
	BEGIN TRAN
	
		insert into Licenses (licensekey, createdby, createdtm) values (@licensekey, SYSTEM_USER, GETUTCDATE())

		select @err = @@error

		if @err <> 0
		begin
			set @errmsg = 'Error: Failed to insert a new license key to Licenses table with licence ' + @licensekey
			RAISERROR (@errmsg, 16, 1)
			ROLLBACK TRAN
			return -1
		end

	COMMIT TRAN

	exec('select max(licenseid) from Licenses')
	
GO
 
GO 

---------- SP_ISP_SQLCM_REMOVELICENSE.SQL
USE SQLcompliance
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[isp_sqlcm_removelicense]'))
drop procedure [dbo].[isp_sqlcm_removelicense]
GO

CREATE procedure [dbo].[isp_sqlcm_removelicense] (@licenseid int)
as

   -- (c) Copyright 2004-2006 Idera, a division of BBS Technologies, Inc., all rights reserved.
   -- SQLsecure, Idera and the Idera Logo are trademarks or registered trademarks
   -- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
   --
   -- Description :
   --              Remove a single license with license id
   -- 	           
	
	declare @result int
	declare @err int
	declare @errmsg nvarchar(500)
	declare @ans int

	-- Get application program name
	declare @programname nvarchar(128)
	select @programname = program_name from master..sysprocesses where spid= @@spid and sid = SUSER_SID(SYSTEM_USER)

	declare @connectionname nvarchar(128)
	set @connectionname = NULL
		
	BEGIN TRAN
	
		delete from Licenses where licenseid = @licenseid

		select @err = @@error

		if @err <> 0
		begin
			set @errmsg = 'Error: Failed to remove a license key with license id ' + CONVERT(NVARCHAR(256), @licenseid)
			RAISERROR (@errmsg, 16, 1)
			ROLLBACK TRAN
			return -1
		end
	COMMIT TRAN	
GO
 
GO 

---------- SP_SQLCM_UPGRADEPROCS.SQL
---------- SP_SQLCM_UPGRADEPROCS.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/252007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

-- Drop procedure before creation to handle upgrade case
IF (OBJECT_ID('sp_sqlcm_UpgradeRepository') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_UpgradeRepository
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_UpgradeRepository]
as
BEGIN
	SET QUOTED_IDENTIFIER OFF
	-- 1.2 Changes
	-- add column to hold reportingVersion if it doesnt already exist
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='reportingVersion' and o.name='Configuration' and c.id=o.id)
		ALTER TABLE Configuration ADD [reportingVersion] [int] NULL DEFAULT (100);
	
	-- Start SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='auditLogouts' and o.name='Configuration' and c.id=o.id)
	begin
		ALTER TABLE Configuration ADD [auditLogouts] [tinyint] NULL  DEFAULT (0);
	    -- Use auditLogins value for auditLogouts for upgrade scenario
	    exec sp_executesql N'UPDATE [SQLcompliance]..[Configuration] SET [auditLogouts] = [auditLogins]'
	end
		
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='auditUserLogouts' and o.name='Configuration' and c.id=o.id)
	begin
		ALTER TABLE Configuration ADD [auditUserLogouts] [tinyint] NULL  DEFAULT (0);
		-- Use auditLogins value for auditUserLogouts for upgrade scenario
	    exec sp_executesql N'UPDATE [SQLcompliance]..[Configuration] SET [auditUserLogouts] = [auditUserLogins]'
	end
	-- Stop SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events

	-- add column to hold repositoryVersion if it doesnt already exist
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='repositoryVersion' and o.name='Configuration' and c.id=o.id)
		ALTER TABLE Configuration ADD [repositoryVersion] [int] NULL DEFAULT (100);
		
	-- Update Change Log Event Type Table
	if not exists (select Name from ChangeLogEventTypes where eventId=50 )
	BEGIN
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (50,'Attach Archive');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (51,'Detach Archive');
	END
		

	-- 2.0  Changes
	--SQLcompliance.Processing
	---------------------------------------------------------------------------------------------------
	IF OBJECT_ID('[SQLcompliance.Processing]..[TraceLogins]') IS NULL
		CREATE TABLE [SQLcompliance.Processing]..[TraceLogins] (
			[instance] [nvarchar] (128) NULL ,
			[loginKey] [int] NULL ,
			[loginValue] [datetime] NULL
		) ON [PRIMARY];
		
	IF NOT EXISTS (SELECT c.name,c.id from [SQLcompliance.Processing]..syscolumns as c,[SQLcompliance.Processing]..sysobjects as o where c.name='FileName' and o.name='TraceStates' and c.id=o.id)		
		ALTER TABLE [SQLcompliance.Processing]..[TraceStates] ADD
			[FileName]         [nvarchar] (128) NULL ,
			[LinkedServerName] [nvarchar] (128) NULL ,
			[ParentName]       [nvarchar] (128) NULL ,
			[IsSystem]         [int] NULL ,
			[SessionLoginName] [nvarchar] (128) NULL ,
			[ProviderName]     [nvarchar] (128) NULL;

	--SQLcompliance
	---------------------------------------------------------------------------------------------------
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='auditBroker' and o.name='Databases' and c.id=o.id)		
		ALTER TABLE [SQLcompliance]..[Databases] ADD
			[auditBroker] [tinyint] NULL DEFAULT (0),
			[auditLogins] [tinyint] NULL DEFAULT (1);

	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='is2005Only' and o.name='EventTypes' and c.id=o.id)
	BEGIN
		ALTER TABLE [SQLcompliance]..[EventTypes] ADD
			[is2005Only] [tinyint] NULL DEFAULT (0),
			[isExcludable] [tinyint] NULL DEFAULT (0);

		EXEC('UPDATE [SQLcompliance]..[EventTypes] SET is2005Only=0');
	END

	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='alertHighWatermark' and o.name='Servers' and c.id=o.id)
	BEGIN
		ALTER TABLE [SQLcompliance]..[Servers] ADD
		  [alertHighWatermark] [int]  DEFAULT (-2100000000) NULL,
			[auditDBCC] [tinyint] NULL DEFAULT(0),
			[auditSystemEvents] [tinyint] NULL DEFAULT (0);
		
		EXEC('UPDATE [SQLcompliance]..[Servers] SET [alertHighWatermark]=[highWatermark]');
	END

	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='auditDBCC' and o.name='Configuration' and c.id=o.id)
		ALTER TABLE [SQLcompliance]..[Configuration] ADD
			[auditDBCC] [tinyint] NULL DEFAULT(0),
			[auditSystemEvents] [tinyint] NULL DEFAULT (0),
			[smtpServer] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			[smtpPort] [int] NULL ,
			[smtpAuthType] [int] NULL ,
			[smtpSsl] [tinyint] NULL ,
			[smtpUsername] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			[smtpPassword] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			[smtpSenderAddress] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			[smtpSenderName] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
		  [loginCollapse] [tinyint] NULL DEFAULT(0) ,
		  [loginTimespan] [int] NULL DEFAULT (60) ,
		  [loginCacheSize] [int] NULL DEFAULT (1000);

	-- Alerting Support
	IF OBJECT_ID('ActionResultStatusTypes') IS NULL
	BEGIN
		CREATE TABLE [SQLcompliance]..[ActionResultStatusTypes] (
			[statusTypeId] [int] NOT NULL ,
			[name] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			CONSTRAINT [PK_ActionResultStatusTypes] PRIMARY KEY  CLUSTERED 
			(
				[statusTypeId]
			)  ON [PRIMARY] 
		) ON [PRIMARY];		
		GRANT SELECT ON [ActionResultStatusTypes] TO [public];
		INSERT INTO ActionResultStatusTypes (statusTypeId, name) VALUES (0, 'Success'); 
		INSERT INTO ActionResultStatusTypes (statusTypeId, name) VALUES (1, 'Failure') ;
		INSERT INTO ActionResultStatusTypes (statusTypeId, name) VALUES (2, 'Pending') ;
		INSERT INTO ActionResultStatusTypes (statusTypeId, name) VALUES (3, 'Uninitialized');
	END

	IF OBJECT_ID('AlertTypes') IS NULL
	BEGIN
		CREATE TABLE [SQLcompliance]..[AlertTypes] (
			[alertTypeId] [int] NOT NULL ,
			[name] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			CONSTRAINT [PK_AlertTypes] PRIMARY KEY  CLUSTERED 
			(
				[alertTypeId]
			)  ON [PRIMARY] 
		) ON [PRIMARY];
		GRANT SELECT ON [AlertTypes] TO [public];
		INSERT INTO AlertTypes (alertTypeId, name) VALUES (1, 'Audited SQL Server');
	END
	
	IF OBJECT_ID('Alerts') IS NULL
	BEGIN
		CREATE TABLE [SQLcompliance]..[Alerts] (
			[alertId] [int] IDENTITY (-2100000000, 1) NOT NULL ,
			[alertType] [int] NOT NULL ,
			[alertRuleId] [int] NOT NULL ,
			[alertEventId] [int] NOT NULL ,
			[instance] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			[eventType] [int] NOT NULL ,
			[created] [datetime] NOT NULL CONSTRAINT [DF_Alerts_created] DEFAULT (getutcdate()),
			[alertLevel] [tinyint] NOT NULL ,
			[emailStatus] [tinyint] NOT NULL,
			[logStatus] [tinyint] NOT NULL,
			[message]  [nvarchar] (3000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			[ruleName] [nvarchar] (60) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			CONSTRAINT [PK_Alerts] PRIMARY KEY  CLUSTERED 
			(
				[alertId]
			)  ON [PRIMARY] ,
			CONSTRAINT [FK_Alerts_AlertTypes] FOREIGN KEY 
			(
				[alertType]
			) REFERENCES [SQLcompliance]..[AlertTypes] (
				[alertTypeId]
			)
		) ON [PRIMARY];
		GRANT SELECT ON [Alerts] TO [public];
		
		CREATE  INDEX [IX_Alerts_created] ON [dbo].[Alerts]([created] DESC, [alertId] DESC ) ON [PRIMARY];
		CREATE  INDEX [IX_Alerts_alertLevel] ON [dbo].[Alerts]([alertLevel], [alertId] DESC ) ON [PRIMARY];
		CREATE  INDEX [IX_Alerts_eventType] ON [dbo].[Alerts]([eventType], [alertId] DESC ) ON [PRIMARY];
	END
		
	IF OBJECT_ID('AlertRules') IS NULL
	BEGIN
		CREATE TABLE [SQLcompliance]..[AlertRules] (
			[ruleId] [int] IDENTITY (-2100000000, 1) NOT NULL ,
			[name] [nvarchar] (60) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			[description] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			[alertLevel] [tinyint] NOT NULL ,
			[alertType] [int] NOT NULL ,
			[targetInstances] [nvarchar] (640) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			[enabled] [tinyint] NOT NULL ,
			[message] [nvarchar] (2500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			[logMessage] [tinyint] NOT NULL,
			[emailMessage] [tinyint] NOT NULL,
			CONSTRAINT [PK_AlertRules] PRIMARY KEY  CLUSTERED 
			(
				[ruleId]
			)  ON [PRIMARY] ,
			CONSTRAINT [FK_AlertRules_AlertTypes] FOREIGN KEY 
			(
				[alertType]
			) REFERENCES [SQLcompliance]..[AlertTypes] (
				[alertTypeId]
			)
		) ON [PRIMARY];
		GRANT SELECT ON [AlertRules] TO [public];
	END

	IF OBJECT_ID('AlertRuleConditions') IS NULL
	BEGIN
		CREATE TABLE [SQLcompliance]..[AlertRuleConditions] (
			[conditionId] [int] IDENTITY (-2100000000, 1) NOT NULL ,
			[ruleId] [int] NOT NULL ,
			[fieldId] [int] NOT NULL ,
			[matchString] [nvarchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			CONSTRAINT [PK_AlertRuleConditions] PRIMARY KEY  CLUSTERED 
			(
				[conditionId]
			)  ON [PRIMARY] ,
			CONSTRAINT [FK_AlertRuleConditions_AlertRules] FOREIGN KEY 
			(
				[ruleId]
			) REFERENCES [SQLcompliance]..[AlertRules] (
				[ruleId]
			)
		) ON [PRIMARY];
		GRANT SELECT ON [AlertRuleConditions] TO [public];
	END
	
	IF NOT EXISTS(select Name from ChangeLogEventTypes where eventId=52 )
	BEGIN
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (52,'Login Filtering Changed');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (53,'Alert Rule Added');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (54,'Alert Rule Removed');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (55,'Alert Rule Modified');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (56,'Alert Rule Disabled');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (57,'Alert Rule Enabled');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (58,'Groom Alerts');
	END

	IF NOT EXISTS(select Name from AgentEventTypes where eventId=17 )
	BEGIN
		INSERT INTO [AgentEventTypes] ([eventId],[Name])	VALUES (17,'Incompatible SQL Server version error');
	END


	IF NOT EXISTS(select name from ObjectTypes where objtypeId=8259 )
	BEGIN
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8259,'CHECK');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8260,'DEFAULT');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8262,'FOREIGN KEY');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8272,'STORED PROCEDURE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8274,'RULE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8275,'SYSTEM TABLE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8276,'SERVER TRIGGER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8277,'USER TABLE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8278,'VIEW');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8280,'EXTENDED SP');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (16724,'CLR TRIGGER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (16964,'DATABASE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (16975,'OBJECT');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17222,'FULL TEXT CATALOG');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17232,'CLR STORED PROCEDURE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17235,'SCHEMA');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17475,'CREDENTIAL');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17491,'DDL EVENT');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17741,'MGMT EVENT');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17747,'SECURITY EVENT');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17749,'USER EVENT');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17985,'CLR AGGREGATE FUNCTION');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17993,'INLINE FUNCTION');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (18000,'PARTITION FUNCTION');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (18002,'REPL FILTER PROC');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (18004,'TABLE VALUED UDF');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (18259,'SERVER ROLE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (18263,'MICROSOFT WINDOWS GROUP');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19265,'ASYMMETRIC KEY');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19277,'MASTER KEY');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19280,'PRIMARY KEY');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19283,'OBFUS KEY');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19521,'ASYMMETRIC KEY LOGIN');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19523,'CERTIFICATE LOGIN');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19538,'ROLE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19539,'SQL LOGIN');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19543,'WINDOWS LOGIN');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20034,'REMOTE SERVICE BINDING');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20036,'EVENT NOTIFICATION DATABASE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20037,'EVENT NOTIFICATION');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20038,'SCALAR FUNCTION');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20047,'EVENT NOTIFICATION OBJECT');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20051,'SYNONYM');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20549,'ENDPOINT');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20801,'CACHED ADHOC QUERIES');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20816,'CACHED ADHOC QUERIES');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20819,'SERVICE BROKER QUEUE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20821,'UNIQUE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21057,'APPLICATION ROLE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21059,'CERTIFICATE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21075,'SERVER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21076,'TSQL TRIGGER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21313,'ASSEMBLY');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21318,'CLR SCALAR FUNCTION');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21321,'INLINE SCALAR FUNCTION');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21328,'PARTITION SCHEME');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21333,'USER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21571,'SERVICE BROKER CONTRACT');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21572,'DATABASE TRIGGER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21574,'CLR TABLE FUNCTION');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21577,'INTERNAL TABLE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21581,'SERVICE BROKER MSG TYPE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21586,'SERVICE BROKER ROUTE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21587,'STATISTICS');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21825,'USER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21827,'USER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21831,'USER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21843,'USER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21847,'USER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (22099,'SERVICE BROKER SERVICE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (22601,'INDEX');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (22604,'CERTIFICATE LOGIN');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (22611,'XML SCHEMA');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (22868,'TYPE');
	END
	
	-- 2.1 Changes
	-- SQLcompliance database modifications
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='auditUDE' and o.name='Servers' and c.id=o.id)
		ALTER TABLE [SQLcompliance]..[Servers] ADD
			[auditUDE] [tinyint] NULL DEFAULT (0),
			[auditUserUDE] [tinyint] NULL DEFAULT (0),
			[agentDetectionInterval] [int] NULL DEFAULT (60);

	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='archiveCheckIntegrity' and o.name='Configuration' and c.id=o.id)
		ALTER TABLE [SQLcompliance]..[Configuration] ADD
			[archiveCheckIntegrity] [tinyint] NOT NULL DEFAULT (1);

	-- Event Filter Support
	IF OBJECT_ID('EventFilters') IS NULL
	BEGIN
		CREATE TABLE [SQLcompliance]..[EventFilters] (
			[filterId] [int] IDENTITY (-2100000000, 1) NOT NULL ,
			[name] [nvarchar] (60) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			[description] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			[eventType] [int] NOT NULL ,
			[targetInstances] [nvarchar] (640) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			[enabled] [tinyint] NOT NULL ,
			CONSTRAINT [PK_EventFilters] PRIMARY KEY  CLUSTERED 
			(
				[filterId]
			)  ON [PRIMARY] ,
		) ON [PRIMARY];
		GRANT SELECT ON [EventFilters] TO [public]
	END

	IF OBJECT_ID('EventFilterConditions') IS NULL
	BEGIN
		CREATE TABLE [SQLcompliance]..[EventFilterConditions] (
			[conditionId] [int] IDENTITY (-2100000000, 1) NOT NULL ,
			[filterId] [int] NOT NULL ,
			[fieldId] [int] NOT NULL ,
			[matchString] [nvarchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			CONSTRAINT [PK_EventFilterConditions] PRIMARY KEY  CLUSTERED 
			(
				[conditionId]
			)  ON [PRIMARY] ,
			CONSTRAINT [FK_EventFilterConditions_EventFilters] FOREIGN KEY 
			(
				[filterId]
			) REFERENCES [dbo].[EventFilters] (
				[filterId]
			)
		) ON [PRIMARY];
		GRANT SELECT ON [EventFilterConditions] TO [public]
	END
	
	IF NOT EXISTS(select Name from ChangeLogEventTypes where eventId=59 )
	BEGIN
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (59,'Event Filter Added');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (60,'Event Filter Removed');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (61,'Event Filter Modified');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (62,'Event Filter Disabled');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (63,'Event Filter Enabled');
	END

	IF NOT EXISTS (select Name from ChangeLogEventTypes where eventId=66 )
	BEGIN
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (66,'Event Database Limit Exceeded');
	END
	
	IF NOT EXISTS(select Name from AgentEventTypes where eventId=18 )
	BEGIN
		INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (18,'Audit trace stopped warning');
		INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (19,'Audit trace closed warning');
		INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (20,'Audit trace altered warning');
	END
	
	IF NOT EXISTS(select name from EventTypes where evtypeid=1800)
	BEGIN
		EXEC sp_sqlcm_UpdateEventTypes;
	END
	
	IF NOT EXISTS(select name from EventTypes where evtypeid=1810)
	BEGIN
		EXEC sp_sqlcm_UpdateEventTypes;
	END

	IF EXISTS(select name from EventTypes where evtypeid=1500)
	BEGIN
		EXEC sp_sqlcm_UpdateEventTypes;
	END
	
	-- 3.0 Changes
	-- Add trusted user column to the Databases table
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='auditUsersList' and o.name='Databases' and c.id=o.id)
		ALTER TABLE [Databases] ADD [auditUsersList] [nvarchar](MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL;

	-- Stats categories
	---------------------------
	IF OBJECT_ID('StatsCategories') IS NULL
	BEGIN
		CREATE TABLE [StatsCategories] ( [category] [int] NOT NULL, [name] [nvarchar] (64) NOT NULL );
		GRANT SELECT ON [StatsCategories] TO [public];
		INSERT INTO StatsCategories (category, name) VALUES (0, 'Unknown');
		INSERT INTO StatsCategories (category, name) VALUES (1, 'Audited Instances');
		INSERT INTO StatsCategories (category, name) VALUES (2, 'Audited Databases');
		INSERT INTO StatsCategories (category, name) VALUES (3, 'Processed Events');
		INSERT INTO StatsCategories (category, name) VALUES (4, 'Alerts');
		INSERT INTO StatsCategories (category, name) VALUES (5, 'Privileged User Events');
		INSERT INTO StatsCategories (category, name) VALUES (6, 'Failed Logins');
		INSERT INTO StatsCategories (category, name) VALUES (7, 'User Defined Events');
		INSERT INTO StatsCategories (category, name) VALUES (8, 'Admin');
		INSERT INTO StatsCategories (category, name) VALUES (9, 'DDL');
		INSERT INTO StatsCategories (category, name) VALUES (10, 'Security');
		INSERT INTO StatsCategories (category, name) VALUES (11, 'DML');
		INSERT INTO StatsCategories (category, name) VALUES (12, 'Insert');
		INSERT INTO StatsCategories (category, name) VALUES (13, 'Update');
		INSERT INTO StatsCategories (category, name) VALUES (14, 'Delete');
		INSERT INTO StatsCategories (category, name) VALUES (15, 'Select');
		INSERT INTO StatsCategories (category, name) VALUES (16, 'Logins');
		INSERT INTO StatsCategories (category, name) VALUES (17, 'Agent Trace Directory Size');
		INSERT INTO StatsCategories (category, name) VALUES (18, 'Integrity Check');
		INSERT INTO StatsCategories (category, name) VALUES (19, 'Execute');
		INSERT INTO StatsCategories (category, name) VALUES (20, 'Event Received');
		INSERT INTO StatsCategories (category, name) VALUES (21, 'Event Processed');
		INSERT INTO StatsCategories (category, name) VALUES (22, 'Event Filtered');		
	END
	
	IF OBJECT_ID('ReportCard') IS NULL
	BEGIN
		CREATE TABLE [dbo].[ReportCard](
			[srvId] [int] NOT NULL,
			[statId] [int] NOT NULL,
			[warningThreshold] [int] NOT NULL,
			[errorThreshold] [int] NOT NULL,
			[period] [int] NOT NULL,
			[enabled] [tinyint] NOT NULL
		) ON [PRIMARY];
		GRANT SELECT ON [ReportCard] TO [public];
	END

	-- Create the licenses table
	IF OBJECT_ID('Licenses') IS NULL
	BEGIN
		CREATE TABLE [dbo].[Licenses] (
	    [licenseid] INTEGER IDENTITY(1,1) NOT NULL,
	    [licensekey] NVARCHAR(256) NOT NULL,
	    [createdby] NVARCHAR(500) NOT NULL,
	    [createdtm] DATETIME NOT NULL,
	    CONSTRAINT [PK__applicationlicen__46E78A0C] PRIMARY KEY ([licenseid])
		);
	END
	-- This was missed in the 3.0 upgrade path - just do it by default
	GRANT SELECT ON [Licenses] TO [public]

	-- 3.1 Changes	
	-- the table that holds the tables being monitored for data changes accross all instances
	IF OBJECT_ID('DataChangeTables') IS NULL
	BEGIN
		CREATE TABLE DataChangeTables ( srvId int not null,
										dbId int not null,
										objectId int not null,
                                        schemaName nVarchar(128) not null,
										tableName nVarchar(128) not null,
										rowLimit int not null default (20),
										CONSTRAINT [PK_DataChangeTables] PRIMARY KEY CLUSTERED (srvId, dbId, objectId )  )
			on [PRIMARY];
		GRANT SELECT ON [DataChangeTables] TO [public];
	END

	-- Add a new column to the databases table for indicating whether there are tables monitored for data changes 
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='auditDataChanges' and o.name='Databases' and c.id=o.id)
		ALTER TABLE Databases ADD auditDataChanges tinyint not null default 0;

	-- Add a new column to the DatabaseObjects table for schema support
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='schemaName' and o.name='DatabaseObjects' and c.id=o.id)
	BEGIN
		ALTER TABLE DatabaseObjects ADD schemaName nvarchar(128) not null default 'dbo';
	END

	-- Update Reports Table
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='port' and o.name='Reports' and c.id=o.id)
	BEGIN
	   DROP TABLE [Reports];
      CREATE TABLE [Reports] (
      	[reportServer] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
      	[serverVirtualDirectory] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
      	[managerVirtualDirectory] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
      	[port] [int] NULL ,
      	[useSsl] [tinyint] NULL ,
      	[userName] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
      	[repository] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
      	[targetDirectory] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
      ) ON [PRIMARY];
		GRANT SELECT ON [Reports] TO [public];
	END
	
	IF NOT EXISTS (SELECT c.name,c.id from [SQLcompliance.Processing]..syscolumns as c,[SQLcompliance.Processing]..sysobjects as o where c.name='eventSequence' and o.name='TraceStates' and c.id=o.id)		
	ALTER TABLE [SQLcompliance.Processing]..[TraceStates] ADD
		[eventSequence]         [bigint] NULL ;
		
	IF NOT EXISTS (SELECT c.name,c.id from [SQLcompliance]..syscolumns as c,[SQLcompliance]..sysobjects as o where c.name='details' and o.name='AgentEvents' and c.id=o.id)		
	ALTER TABLE [SQLcompliance]..[AgentEvents] ADD
   	[details] [nvarchar](MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL;
		
	IF NOT EXISTS (SELECT c.name,c.id from [SQLcompliance]..syscolumns as c,[SQLcompliance]..sysobjects as o where c.name='agentHealth' and o.name='Servers' and c.id=o.id)		
	ALTER TABLE [SQLcompliance]..[Servers] ADD
      [agentHealth] [bigint] DEFAULT(0) NULL;
      
	IF NOT EXISTS(select name from EventTypes where evtypeid=900001)
	BEGIN
   	INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (900001,4,'Encrypted','DML',0,0);
	END
	
	IF NOT EXISTS(select Name from AgentEventTypes where eventId=1001 )
	BEGIN
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (1001,'Agent Warning');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (3001,'Agent Warning Resolution');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (2001,'Agent Configuration Error');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (4001,'Agent Configuration Resolution');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (2002,'Trace Directory Error');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (4002,'Trace Directory Resolution');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (2003,'SQL Trace Error');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (4003,'SQL Trace Resolution');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (2004,'Server Connection Error');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (4004,'Server Connection Resolution');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (2005,'Collection Service Connection Error');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (4005,'Collection Service Connection Resolution');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (2006,'CLR Error');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (4006,'CLR Resolution');
	END      
	
	UPDATE [SQLcompliance]..[EventTypes] SET [isExcludable]=1 WHERE evtypeid=139;
	UPDATE [SQLcompliance]..[EventTypes] SET [isExcludable]=1 WHERE evtypeid=339;

	IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Alerts_created')
		UPDATE [SQLcompliance]..[Configuration] SET [sqlComplianceDbSchemaVersion]=502,[eventsDbSchemaVersion ]=402;
	ELSE
		UPDATE [SQLcompliance]..[Configuration] SET [sqlComplianceDbSchemaVersion]=501,[eventsDbSchemaVersion ]=402;
		
	UPDATE Configuration SET reportingVersion=103
	UPDATE Configuration SET repositoryVersion=100
	
END
GO


--
-- Upgrade Event Databases and Archives
--
IF (OBJECT_ID('sp_sqlcm_UpgradeEventDatabase') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_UpgradeEventDatabase
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_UpgradeEventDatabase](@databaseName nvarchar(128))
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
END';

	EXEC(@sqlText);
	
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

	
	SET @sqlText = N'USE [' + @databaseName2 + ']
	GRANT SELECT ON [Stats] TO [guest];
	GRANT SELECT ON [Logins] TO [guest];
	GRANT SELECT ON [Applications] TO [guest];
	GRANT SELECT ON [Hosts] TO [guest];';
	
	EXEC(@sqlText);
	


	-- Update Schema Version	
	SET @sqlText = N'USE [' + @databaseName2 + ']
IF EXISTS (SELECT name FROM sysindexes WHERE name = ''IX_Events_appNameId'')
	UPDATE [Description] SET [eventDbSchemaVersion]=402;
ELSE
	UPDATE [Description] SET [eventDbSchemaVersion]=401;';
	
	EXEC(@sqlText);
END
GO

IF (OBJECT_ID('sp_sqlcm_UpgradeAllEventDatabases') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_UpgradeAllEventDatabases
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_UpgradeAllEventDatabases]
as
BEGIN
	-- Create a cursor to iterate through the tables
	DECLARE @dbname varchar(255)
	
	DECLARE db_name INSENSITIVE CURSOR FOR
	SELECT databaseName FROM [SQLcompliance]..SystemDatabases WHERE databaseType='Archive' OR databaseType='Event'
	FOR READ ONLY
	
	SET NOCOUNT ON
	OPEN db_name 
	FETCH db_name INTO @dbname 
	WHILE @@fetch_status = 0 
	BEGIN
		EXEC sp_sqlcm_UpgradeEventDatabase @dbname
		FETCH db_name INTO @dbname 
	END
	CLOSE db_name 
	DEALLOCATE db_name
END
GO

IF (OBJECT_ID('sp_sqlcm_UpdateEventTypes') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_UpdateEventTypes
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_UpdateEventTypes]
as
BEGIN
	DELETE FROM [EventTypes];
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (-1,-1,''Invalid'',''Invalid'',0,0)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1,5,''Select'',''Select'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (2,4,''Update'',''DML'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (4,5,''References'',''Select'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (8,4,''Insert'',''DML'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (16,4,''Delete'',''DML'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (32,4,''Execute'',''DML'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (50,1,''Login'',''Login'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (51,1,''Login Failed'',''Login'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (52,1,''Server impersonation'',''Login'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (53,1,''Database impersonation'',''Login'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (54,3,''Disable login'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (55,3,''Enable login'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (62,6,''Backup'',''Admin'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (63,6,''Restore'',''Admin'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (64,6,''Backup database'',''Admin'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (65,6,''Backup log'',''Admin'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (66,6,''Backup Table'',''Admin'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (71,6,''Trace started'',''Trace'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (72,6,''Trace stopped'',''Trace'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (80,6,''DBCC'',''Admin'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (81,6,''DBCC - read'',''Admin'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (82,6,''DBCC - write'',''Admin'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (90,6,''Server operation'',''Admin'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (91,6,''Database operation'',''Admin'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (92,6,''Server alter trace'',''Admin'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (93,8,''Broker Conversation'',''Broker'',1,0)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (94,8,''Broker Login'',''Broker'',1,0)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (100,2,''Create'',''DDL'',0,0)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (101,2,''Create index'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (102,2,''Create database'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (103,2,''Create user object'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (105,2,''Create DEFAULT'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (108,2,''Create stored procedure'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (109,2,''Create UDF'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (110,2,''Create rule'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (111,2,''Create repl filter stored proc'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (113,2,''Create trigger'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (114,2,''Create inline function'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (115,2,''Create table valued UDF'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (117,2,''Create user table'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (118,2,''Create view'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (119,2,''Create ext stored proc'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (122,2,''Create statistics'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (125,2,''Create Server Trigger'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (126,2,''Create CLR Trigger'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (127,2,''Create Full Text Catalog'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (128,2,''Create CLR Stored Procedure'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (129,2,''Create Schema'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (130,3,''Create Credential'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (131,2,''Create DDL Event'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (132,2,''Create Management Event'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (133,2,''Create Security Event'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (134,2,''Create User Event'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (135,2,''Create CLR Aggregate Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (136,2,''Create Partition Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (137,2,''Create Table Valued SQL Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (139,3,''Create Microsoft Windows Group'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (140,2,''Create Asymmetric Key'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (141,2,''Create Master Key'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (142,2,''Create Symmetric Key'',''DDL'',1,0)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (143,3,''Create Asymmetric Key Login'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (144,3,''Create Certificate Login'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (145,3,''Create Role'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (146,3,''Create SQL Login'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (147,3,''Create Windows Login'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (148,2,''Create Remote Service Binding'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (149,2,''Create Event Notification On Database'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (150,2,''Create Event Notification'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (151,2,''Create Scalar SQL Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (152,2,''Create Event Notification On Object'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (153,2,''Create Synonym'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (154,2,''Create End Point'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (155,2,''Create Service Broker Service Queue'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (156,3,''Create Application Role'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (157,3,''Create Certificate'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (159,2,''Create Transact SQL Trigger'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (160,2,''Create Assembly'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (161,2,''Create CLR Scalar Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (162,2,''Create Partition Scheme'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (163,2,''Create User Object'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (164,2,''Create Service Broker Service Contract'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (165,2,''Create CLR Table Valued Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (167,2,''Create Service Broker Message Type'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (168,2,''Create Service Broker Route'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (169,3,''Create User'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (170,2,''Create Service Broker Service'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (172,2,''Create XML Schema'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (173,2,''Create Type'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (200,2,''Alter'',''DDL'',0,0)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (201,2,''Alter index'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (202,2,''Alter database'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (203,2,''Alter user object'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (205,2,''Alter DEFAULT'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (208,2,''Alter stored procedure'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (209,2,''Alter UDF'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (210,2,''Alter rule'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (211,2,''Alter repl filter stored proc'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (212,2,''Alter system table'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (213,2,''Alter trigger'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (214,2,''Alter inline function'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (215,2,''Alter table valued UDF'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (217,2,''Alter user table'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (218,2,''Alter view'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (222,2,''Alter statistics'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (225,2,''Alter Server Trigger'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (226,2,''Alter CLR Trigger'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (227,2,''Alter Full Text Catalog'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (228,2,''Alter CLR Stored Procedure'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (229,2,''Alter Schema'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (230,3,''Alter Credential'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (235,2,''Alter CLR Aggregate Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (236,2,''Alter Partition Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (237,2,''Alter Table Valued SQL Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (240,2,''Alter Asymmetric Key'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (241,2,''Alter Master Key'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (242,2,''Alter Symmetric Key'',''DDL'',1,0)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (245,3,''Alter Role'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (251,2,''Alter Scalar SQL Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (254,2,''Alter End Point'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (255,2,''Alter Service Broker Service Queue'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (256,3,''Alter Application Role'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (257,3,''Alter Certificate'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (259,2,''Alter Transact SQL Trigger'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (260,2,''Alter Assembly'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (261,2,''Alter CLR Scalar Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (262,2,''Alter Partition Scheme'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (263,2,''Alter User Object'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (265,2,''Alter CLR Table Valued Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (267,2,''Alter Service Broker Message Type'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (268,2,''Alter Service Broker Route'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (269,3,''Alter User'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (270,2,''Alter Service Broker Service'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (272,2,''Alter XML Schema'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (273,2,''Alter Type'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (300,2,''Drop'',''DDL'',0,0)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (301,2,''Drop index'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (302,2,''Drop database'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (303,2,''Drop user object'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (305,2,''Drop DEFAULT'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (308,2,''Drop stored procedure'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (309,2,''Drop UDF'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (310,2,''Drop rule'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (311,2,''Drop repl filter stored proc'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (313,2,''Drop trigger'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (314,2,''Drop inline function'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (315,2,''Drop table valued UDF'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (317,2,''Drop user table'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (318,2,''Drop view'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (319,2,''Drop ext stored proc'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (322,2,''Drop statistics'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (325,2,''Drop Server Trigger'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (326,2,''Drop CLR Trigger'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (327,2,''Drop Full Text Catalog'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (328,2,''Drop CLR Stored Procedure'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (329,2,''Drop Schema'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (330,3,''Drop Credential'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (331,2,''Drop DDL Event'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (332,2,''Drop Management Event'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (333,2,''Drop Security Event'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (334,2,''Drop User Event'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (335,2,''Drop CLR Aggregate Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (336,2,''Drop Partition Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (337,2,''Drop Table Valued SQL Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (339,3,''Drop Microsoft Windows Group'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (340,2,''Drop Asymmetric Key'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (341,2,''Drop Master Key'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (342,2,''Drop Symmetric Key'',''DDL'',1,0)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (343,3,''Drop Asymmetric Key Login'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (344,3,''Drop Certificate Login'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (345,3,''Drop Role'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (346,3,''Drop SQL Login'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (347,3,''Drop Windows Login'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (348,2,''Drop Remote Service Binding'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (349,2,''Drop Event Notification On Database'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (350,2,''Drop Event Notification'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (351,2,''Drop Scalar SQL Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (352,2,''Drop Event Notification On Object'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (353,2,''Drop Synonym'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (354,2,''Drop End Point'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (355,2,''Drop Service Broker Service Queue'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (356,3,''Drop Application Role'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (357,3,''Drop Certificate'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (359,2,''Drop Transact SQL Trigger'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (360,2,''Drop Assembly'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (361,2,''Drop CLR Scalar Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (362,2,''Drop Partition Scheme'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (363,2,''Drop User Object'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (364,2,''Drop Service Broker Service Contract'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (365,2,''Drop CLR Table Valued Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (367,2,''Drop Service Broker Message Type'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (368,2,''Drop Service Broker Route'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (369,3,''Drop User'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (370,2,''Drop Service Broker Service'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (372,2,''Drop XML Schema'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (373,2,''Drop Type'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (400,3,''Grant object Permission'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (500,3,''Deny object permission'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (600,3,''Revoke object permission'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (700,3,''Add database user'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (701,3,''Drop database user'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (702,3,''Grant database access'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (703,3,''Revoke database access'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (704,3,''Add login to server role'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (705,3,''Drop login from server role'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (706,3,''Add member to database role'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (707,3,''Drop member from database role'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (708,3,''Add database role'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (709,3,''Drop database role'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (710,3,''Add login'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (711,3,''Drop login'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (712,3,''App role change password'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (713,3,''Password change - self'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (714,3,''Password change'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (715,3,''Login change default database'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (716,3,''Login change default language'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (717,3,''Grant login'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (718,3,''Revoke login'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (719,3,''Deny login'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (720,3,''Server Object Change Owner'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (721,3,''Change Database Owner'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (722,3,''Database Object Change Owner'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (723,3,''Schema Object Change Owner'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (724,3,''Credential mapped to login'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (725,3,''Credential map dropped'',''Security'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1000,3,''Grant statement permission'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1100,3,''Deny statement permission'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1200,3,''Revoke statement permission'',''Security'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1600,2,''Access'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1700,2,''Transfer'',''DDL'',0,0)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1703,2,''Transfer user object'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1705,2,''Transfer DEFAULT'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1708,2,''Transfer stored procedure'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1709,2,''Transfer UDF'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1710,2,''Transfer rule'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1711,2,''Transfer repl filter stored proc'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1712,2,''Transfer system table'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1714,2,''Transfer inline function'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1715,2,''Transfer table valued UDF'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1717,2,''Transfer user table'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1718,2,''Transfer view'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1719,2,''Transfer ext stored proc'',''DDL'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1728,2,''Transfer CLR Stored Procedure'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1735,2,''Transfer CLR Aggregate Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1736,2,''Transfer Partition Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1737,2,''Transfer Table Valued SQL Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1751,2,''Transfer Scalar SQL Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1753,2,''Transfer Synonym'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1755,2,''Transfer Service Broker Service Queue'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1761,2,''Transfer CLR Scalar Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1763,2,''Transfer User'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1765,2,''Transfer CLR Table Valued Function'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1769,2,''Transfer User'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1772,2,''Transfer XML Schema'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1773,2,''Transfer Type'',''DDL'',1,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1800,9,''User Configurable 0'',''User Defined'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1801,9,''User Configurable 1'',''User Defined'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1802,9,''User Configurable 2'',''User Defined'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1803,9,''User Configurable 3'',''User Defined'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1804,9,''User Configurable 4'',''User Defined'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1805,9,''User Configurable 5'',''User Defined'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1806,9,''User Configurable 6'',''User Defined'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1807,9,''User Configurable 7'',''User Defined'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1808,9,''User Configurable 8'',''User Defined'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1809,9,''User Configurable 9'',''User Defined'',0,1)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (900001,4,''Encrypted'',''DML'',0,0)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (999997,0,''Missing events'',''Integrity Check'',0,0)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (999998,0,''Events inserted'',''Integrity Check'',0,0)');
	EXEC(N'INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (999999,0,''Event modified'',''Integrity Check'',0,0)');
END
GO
 
GO 



USE SQLcompliance


exec sp_sqlcm_UpgradeRepository
exec sp_sqlcm_UpgradeAllEventDatabases


