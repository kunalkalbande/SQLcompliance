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
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetStatusAlerts' and xtype='P')
drop procedure [sp_cmreport_GetStatusAlerts]
GO

CREATE PROCEDURE sp_cmreport_GetStatusAlerts(
	@Server nvarchar(256) = NULL,
	@startDate datetime, 
	@endDate datetime,
	@StartTimeofDay nvarchar (1) = NULL,
	@EndTimeofDay nvarchar (1) = NULL,
	@hourOfStartTime nvarchar (40), 
	@hourOfEndTime nvarchar (40), 
	@minuteOfStartTime nvarchar (40),
	@minuteOfEndTime nvarchar (40), 
	@amPmOfStartTime nvarchar (40), 	
	@amPmOfEndTime nvarchar (40),
	@alertLevel int,
	@sortColumn nvarchar(256),
	@rowCount int 
	)
as
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);
declare @stmt nvarchar(4000)
declare @columns nvarchar(2000)
declare @fromClause nvarchar(500)
declare @whereClause nvarchar(1000)
declare @orderByClause nvarchar(200)
declare @concatedStartTime nvarchar(40);
declare @localStartTime nvarchar(40);
declare @concatedEndTime nvarchar(40);
declare @localEndTime nvarchar(40);
declare @tempTable nvarchar(10);
declare @whereDate nvarchar(500);
declare @whereTime nvarchar(1000);

	---- Concating all local strings of START TIMES
	set @concatedStartTime = @hourOfStartTime+':'+@minuteOfStartTime+' '+@amPmOfStartTime
	set @localStartTime = CAST(@concatedStartTime AS DATETIME2)

	---- Concating all local strings of END TIMES
	set @concatedEndTime = @hourOfEndTime+':'+ @minuteOfEndTime+' '+ @amPmOfEndTime
	set @localEndTime = CAST(@concatedEndTime AS DATETIME2)
	
	-- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

	-- DEFINE COLUMNS FOR SELECT
	set @columns = 'Select TOP ' + STR(@rowCount) + ' * FROM ( select ' +
   	'a.created as ''alertTime'', ' +
   	'CASE a.alertLevel ' +
      	'WHEN 4 THEN ''Severe'' ' +
      	'WHEN 3 THEN ''High'' ' +
      	'WHEN 2 THEN ''Medium'' ' +
      	'WHEN 1 THEN ''Low'' ' +
      	'ELSE ''Unknown'' ' +
      	'END as alertLevel, ' +
      	'a.ruleName as ''RuleName'', ' +
      	'a.instance, ' +
      	'a.computerName, ' + 
      	't.RuleName as ''SourceRule'' ';

	-- Build FROM clause
	set @fromClause = 'FROM SQLcompliance..Alerts a JOIN SQLcompliance..StatusRuleTypes t ON a.alertEventId = t.StatusRuleId ';
	
	-- Build WHERE clause		
	set @whereClause = 'WHERE a.created >= CONVERT(DATETIME, ''' + @startDateStr + ''') ' +
   	'AND a.created <= CONVERT(DATETIME, ''' + @endDateStr + ''') ' +
   	'AND a.alertType = 2';

	if(@Server <> NULL)
	 begin
		if(@Server <> '<ALL>' AND @Server <> '<All>')
			set @whereClause = @whereClause + ' AND a.instance = ''' + @Server + ''''
	 end
	
	if ( @alertLevel <> 0 )
	   set @whereClause = @whereClause + ' AND a.alertLevel = ' + STR(@alertLevel)
	
    set @orderByClause = ' ORDER BY alertTime DESC';
	   	  set @tempTable = ') temp' ;
	  set @whereDate = ' WHERE CONVERT (Date, [temp].alertTime) <= CONVERT(Date, '''+@endDateStr+''') and Convert (Date, [temp].alertTime) >= CONVERT(Date, '''+@startDateStr+''') ';
	  if(@localEndTime > @localStartTime)
	  set @whereTime = 'AND (CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].alertTime)) <= CONVERT(Time, '''+@localEndTime+''') AND CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].alertTime)) >= CONVERT(Time, '''+@localStartTime+'''))';
	else
	set @whereTime =
	  'AND 
	  (
	    (
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].alertTime)) <= CONVERT(Time, '''+@localEndTime+''') 
			AND 
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].alertTime)) >= CONVERT(Time, ''1900-01-01 00:00:00.0000000'')
	    )
		OR
		(
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].alertTime)) <= CONVERT(Time, ''1900-01-01 23:59:59.0000000'') 
			AND 
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].alertTime)) >= CONVERT(Time, '''+@localStartTime+''')
	    )
	  )'
	-- Build Complete SELECT statement
	set @stmt = @columns + ' ' + @fromClause + ' ' + @whereClause + ' ' + @tempTable + @whereDate + @whereTime + @orderByClause
	
	-- Execute SELECT
	EXEC(@stmt)
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- no more needed in SQLcm 4.5
-- grant execute on sp_cmreport_GetStatusAlerts to public
-- GO
