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

CREATE procedure sp_cmreport_GetAgentHistory (
@instance nvarchar(256), 
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
@agent nvarchar(256) = NULL,
@event nvarchar(256) = NULL, 
@sortColumn nvarchar(256), 
@rowCount int )
as
declare @stmt nvarchar(2000);
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);
declare @orderByClause nvarchar(200);

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
	
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

	set @stmt = ' Select TOP ' + STR(@rowCount) + ' * FROM ( select t1.eventTime, t1.agentServer, t1.instance, t2.Name ''eventType'' ' +
		'from AgentEvents t1 LEFT OUTER JOIN AgentEventTypes t2 on t1.eventType=t2.eventId  ' +
		'where eventTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and eventTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') '

	if(@instance != '<ALL>')
		set @stmt = @stmt + ' and UPPER(t1.instance) = ''' + UPPER(@instance) + ''' '

	if(@agent <> NULL)
	begin
		set @agent = dbo.fn_cmreport_ProcessString(@agent);
		set @stmt = @stmt + ' and UPPER(t1.agentServer) like ''' + UPPER(@agent) + ''' '
	end

	if(@event <> NULL)
	begin
		set @event = dbo.fn_cmreport_ProcessString(@event);
		set @stmt = @stmt + ' and UPPER(t2.Name) like ''' + UPPER(@event) + ''' '
	end

	-- Descinding for time columns
	if(@sortColumn = 'date')
		set @orderByClause = ' ORDER BY temp.eventTime DESC';  
	else
		set @orderByClause = ' ORDER BY temp.agentServer' ;

	  set @tempTable = ') temp' ;
	  set @whereDate = ' WHERE CONVERT (Date, [temp].eventTime) <= CONVERT(Date, '''+@endDateStr+''') and Convert (Date, [temp].eventTime) >= CONVERT(Date, '''+@startDateStr+''') ';
	  if(@localEndTime > @localStartTime) 
	  set @whereTime = 'AND (CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].eventTime)) <= CONVERT(Time, '''+@localEndTime+''') AND CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].eventTime)) >= CONVERT(Time, '''+@localStartTime+'''))';
	  else
	  set @whereTime =
	  'AND 
	  (
	    (
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].eventTime)) <= CONVERT(Time, '''+@localEndTime+''') 
			AND 
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].eventTime)) >= CONVERT(Time, ''1900-01-01 00:00:00.0000000'')
	    )
		OR
		(
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].eventTime)) <= CONVERT(Time, ''1900-01-01 23:59:59.0000000'') 
			AND 
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].eventTime)) >= CONVERT(Time, '''+@localStartTime+''')
	    )
	  )'
	set @stmt = @stmt + @tempTable + @whereDate + @whereTime + @orderByClause;
	   
	
	EXEC(@stmt)
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- no more needed in SQLcm 4.5
-- grant execute on sp_cmreport_GetAgentHistory to public
-- GO
