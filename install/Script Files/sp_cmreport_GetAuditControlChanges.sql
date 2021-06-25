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


CREATE procedure sp_cmreport_GetAuditControlChanges (
	@eventDatabase nvarchar(256) = NULL, 
	@loginName nvarchar(Max) = NULL,
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
	@sortColumn nvarchar(256), 
	@event nvarchar(256) = NULL,
	@rowCount int,
	@generatedFrom nvarchar(10) = NULL )
as
declare @stmt nvarchar(MAX), @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(MAX), @orderByClause nvarchar(200)
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);
declare @Server nvarchar(256);

declare @concatedStartTime nvarchar(40);
declare @localStartTime nvarchar(40);
declare @concatedEndTime nvarchar(40);
declare @localEndTime nvarchar(40);
declare @tempTable nvarchar(10);
declare @whereDate nvarchar(500);
declare @whereTime nvarchar(1000);


   set @Server = @eventDatabase;
   if(@generatedFrom = 'Windows' and @loginName = '<ALL>')
   begin
	set @loginName = NULL;
   end
   set @loginName = LOWER(dbo.fn_cmreport_ProcessString(@loginName));
   SET @loginName = dbo.fn_cmreport_CreateMultiString(@loginName);
   
	---- Concating all local strings of START TIMES
	set @concatedStartTime = @hourOfStartTime+':'+@minuteOfStartTime+' '+@amPmOfStartTime
	set @localStartTime = CAST(@concatedStartTime AS DATETIME2)

	---- Concating all local strings of END TIMES
	set @concatedEndTime = @hourOfEndTime+':'+ @minuteOfEndTime+' '+ @amPmOfEndTime
	set @localEndTime = CAST(@concatedEndTime AS DATETIME2)

    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

	set @columns = ' Select TOP ' + STR(@rowCount) + ' * FROM ( select eventTime, t2.Name, logUser, logSqlServer, logInfo  '
	set @fromClause = 'from ChangeLog t1 LEFT OUTER JOIN ChangeLogEventTypes t2 ON t1.logType = t2.eventId '
	set @whereClause = 'where eventTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and eventTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') ';
	
	if(@Server <> NULL)
	 begin
		if(@Server <> '<ALL>')
			begin 
				set @whereClause = @whereClause + 'AND UPPER(logSqlServer) like ''' + (@Server) + ''' '
			end 
	 end
	 if(@event <> NULL)
			begin
				set @event = UPPER(dbo.fn_cmreport_ProcessString(@event));
				set @whereClause = @whereClause + 'AND UPPER(t2.Name) like ''' + (@event) + ''' '
	 end
	 	
	if(@loginName IS NOT NULL)
	begin
		set @whereClause = @whereClause + ' AND LOWER(logUser) IN (' + @loginName + ')'
	end
	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY eventTime DESC';
	else
	   set @orderByClause = ' ORDER BY logUser' ;

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

	set @stmt = @columns + @fromClause + @whereClause + @tempTable + @whereDate + @whereTime + @orderByClause;

	EXEC(@stmt)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- no more needed in SQLcm 4.5
-- grant execute on sp_cmreport_GetAuditControlChanges to public
-- GO
