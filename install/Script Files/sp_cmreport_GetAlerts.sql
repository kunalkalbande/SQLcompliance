-- Idera SQL compliance Manager version 3.3
-- Last modification date: 8/10/2011
--
-- (c) Copyright 2004-2011 Idera, a division of BBS Technologies, Inc., all rights reserved.
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
					@databaseName nvarchar(256), 
					@loginName nvarchar(MAX) = NULL,
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
					@schemaName nvarchar(256) = NUll,
					@objectName nvarchar(256), 
					@applicationName nvarchar(256) = NULL,
					@hostName nvarchar(256) = NULL,
                    @eventCategory int,  
					@event nvarchar(256) = NULL,
					@showSqlText bit, 
                    @privilegedUserOnly bit,                                          
                    @alertLevel int, 
                    @sortColumn nvarchar(256),  
                    @rowCount int,
					@generatedFrom nvarchar(10) = NULL )
					 
as  
declare @stmt nvarchar(MAX)  
declare @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(MAX), @orderByClause nvarchar(200)  
  
declare @eventTable       nvarchar(256)  
declare @eventSqlTable    nvarchar(256)  
declare @startDateStr nvarchar(40);  
declare @endDateStr nvarchar(40);  
declare @instanceName nvarchar(256); 
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

if (@eventDatabase = '<ALL>')
	begin
	declare @eventDatabaseName nvarchar(128),
			@description nvarchar(512),
			@length int,
			@tempColumns nvarchar(2000)
		if (object_id(N'tempdb..#alerts') IS NOT NULL)
			drop table #alerts
		create table #alerts (
			alertTime datetime,
			alertLevel nvarchar(64),
			eventType nvarchar(64),
			loginName nvarchar(128),
			applicationName nvarchar(128), 
			hostName nvarchar(128), 
			parentName nvarchar(256),
			databaseName nvarchar(128),
			targetObject nvarchar(128),
			details nvarchar(512),
			sqlText nvarchar(max)
			)
			
			if(@loginName <> NULL)
			begin				
				set @loginName = UPPER(dbo.fn_cmreport_ProcessString(@loginName));
				set @loginName = dbo.fn_cmreport_CreateMultiString(@loginName);
			end

	DECLARE eventDatabases CURSOR FOR 
	   SELECT DISTINCT SD.databaseName, SD.instance + CASE WHEN UPPER(databaseType) LIKE 'EVENT' THEN '' ELSE ' - ' + displayName END AS description
		  FROM SQLcompliance..SystemDatabases as SD left join Servers as S on (SD.instance = S.instance)
		  WHERE(UPPER(databaseType) IN ('ARCHIVE', 'EVENT'))
		  ORDER BY description;
    OPEN eventDatabases;
	FETCH eventDatabases INTO @eventDatabaseName, @description
	while @@Fetch_Status = 0
	begin
		set @instanceName = coalesce((select instance from SQLcompliance..SystemDatabases where databaseName = @eventDatabaseName), '')   
		set @eventDatabaseName = dbo.fn_cmreport_ProcessString(@eventDatabaseName);  
		set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));  
		set @objectName = UPPER(dbo.fn_cmreport_ProcessString(@objectName));
		set @eventTable       = '[' + @eventDatabaseName + '].dbo.Events'  
		set @eventSqlTable    = '[' + @eventDatabaseName + '].dbo.EventSQL'  
		set @columns = ' Select TOP ' + STR(@rowCount) + ' * FROM ( select ' +
		'a.created ''alertTime'', ' +  
		'CASE a.alertLevel ' +  
		   'WHEN 4 THEN ''Severe'' ' +  
		   'WHEN 3 THEN ''High'' ' +  
		   'WHEN 2 THEN ''Medium'' ' +  
		   'WHEN 1 THEN ''Low'' ' +  
		   'ELSE ''Unknown'' ' +  
		   'END as alertLevel, ' +  
		'et.name ''eventType'', ' +  
		-- Changing the select mechanism due to request on SQLCM - 6136 and SQLCM - 6137
		--'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +  
		--   'THEN loginName ' +   
		--   'ELSE sessionLoginName ' +   
		--   'END AS loginName, ' +  
		   'e.loginName, ' +  
		'e.applicationName, ' +  
		'e.hostName, e.parentName,' +  
		'e.databaseName, ' +  
		'e.targetObject, ' +  
		'e.details, ';  
		if(@showSqlText = 1)  
		  set @columns = @columns + 'es.sqlText ';  
		else  
		  set @columns = @columns + ' '''' ''sqlText'' ';  
		set @fromClause = 'FROM SQLcompliance..Alerts a LEFT OUTER JOIN ' + @eventTable + ' e ON a.alertEventId = e.eventId ';  
		if (@showSqlText = 1)  
			set @fromClause = @fromClause + 'LEFT OUTER JOIN ' + @eventSqlTable + ' es ON e.eventId = es.eventId ' ;  
		set @fromClause = @fromClause + 'LEFT OUTER JOIN SQLcompliance..EventTypes et ON a.eventType = et.evtypeid ' ;  
		set @whereClause = 'WHERE a.created >= CONVERT(DATETIME, ''' + @startDateStr + ''') ' +  
		'AND a.created <= CONVERT(DATETIME, ''' + @endDateStr + ''') ' +  
		'AND a.alertType = 1' +  
		'AND a.instance = ''' + @instanceName + ''' ' +  
		'AND UPPER(databaseName) LIKE ''' + @databaseName + ''' ' +  
		'AND UPPER(e.targetObject) LIKE ''' + @objectName + ''' ';  
		if( @eventCategory >= 0 )  
			set @whereClause = @whereClause + 'AND a.eventType=' + STR(@eventCategory)  
		if ( @alertLevel <> 0 )  
			set @whereClause = @whereClause + ' AND a.alertLevel = ' + STR(@alertLevel)  
		if (@privilegedUserOnly = 1)  
			set @whereClause = @whereClause + ' and privilegedUser = 1 '  

		-- 5.2.4.5
		if (@loginName <> NULL)      
			begin    
				if(@loginName = '<ALL>' or @loginName = '<All>' or @loginName = '%')
					begin
						-- Changing the select mechanism due to request on SQLCM - 6136 and SQLCM - 6137
						--set @whereClause = @whereClause + '	AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) like ''' + @loginName + ''' OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) like ''' + @loginName + ''')'
						set @whereClause = @whereClause + '	AND UPPER(loginName) like ''' + @loginName + ''''
					end
				else
					begin
						-- Changing the select mechanism due to request on SQLCM - 6136 and SQLCM - 6137
						-- set @whereClause = @whereClause + ' AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) IN (' + @loginName + ') OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) IN (' + @loginName + '))'
						set @whereClause = @whereClause + ' AND UPPER(loginName) IN (' + @loginName + ')'
					end
			end

		if(@schemaName <> NULL)
			begin
				set @schemaName = UPPER(dbo.fn_cmreport_ProcessString(@schemaName));
				set @whereClause = @whereClause + 'AND UPPER(e.parentName) like ''' + (@schemaName) + ''' '
			end			

		if(@applicationName <> NULL)
			begin
				set @applicationName = UPPER(dbo.fn_cmreport_ProcessString(@applicationName));
				set @whereClause = @whereClause + 'AND UPPER(e.applicationName) like ''' + (@applicationName) + ''' '
			end

		if(@hostName <> NULL)
			begin
				set @hostName = UPPER(dbo.fn_cmreport_ProcessString(@hostName));
				set @whereClause = @whereClause + 'AND UPPER(e.hostName) like ''' + (@hostName) + ''' '
			end

		if(@event <> NULL)
			begin
				set @event = UPPER(dbo.fn_cmreport_ProcessString(@event));
				set @whereClause = @whereClause + 'AND UPPER(et.name) like ''' + (@event) + ''' '
		end
		-- 5.2.4.5

		if(@sortColumn = 'date')  
			set @orderByClause = ' ORDER BY alertTime DESC';  
		else  
			set @orderByClause = ' ORDER BY loginName' ;  
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
		set @stmt = @columns + ' ' + @fromClause + ' ' + @whereClause + ' ' + @tempTable + @whereDate + @whereTime + @orderByClause
		insert into #alerts exec (@stmt) 
		fetch eventDatabases into @eventDatabaseName, @description
		end
		close eventDatabases  
		deallocate eventDatabases
		select * from #alerts
	end
	else
	begin
  
	--get the instance name  
	set @instanceName = coalesce((select instance from SQLcompliance..SystemDatabases where databaseName = @eventDatabase), '')   
	  
   -- prevents sql injection but set limitations on the database naming  
    set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);  
	set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));  
	set @objectName = UPPER(dbo.fn_cmreport_ProcessString(@objectName));  

   -- table used in queries  
    set @eventTable       = '[' + @eventDatabase + '].dbo.Events'  
    set @eventSqlTable    = '[' + @eventDatabase + '].dbo.EventSQL'  
  
 -- DEFINE COLUMNS FOR SELECT  
	set @columns = 'Select TOP ' + STR(@rowCount) + ' * FROM ( select ' +
    'a.created ''alertTime'', ' +  
    'CASE a.alertLevel ' +  
       'WHEN 4 THEN ''Severe'' ' +  
       'WHEN 3 THEN ''High'' ' +  
       'WHEN 2 THEN ''Medium'' ' +  
       'WHEN 1 THEN ''Low'' ' +  
       'ELSE ''Unknown'' ' +  
       'END as alertLevel, ' +  
    'et.name ''eventType'', ' +  
	-- Changing the select mechanism due to request on SQLCM - 6136 and SQLCM - 6137
	--'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +  
	--	   'THEN loginName ' +   
	--	   'ELSE sessionLoginName ' +   
	--	   'END AS loginName, ' +  
    'e.loginName, ' + 
    'e.applicationName, ' +  
    'e.hostName, e.parentName,' +  
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
    'AND a.alertType = 1' +  
    'AND a.instance = ''' + @instanceName + ''' ' +  
    'AND UPPER(databaseName) LIKE ''' + @databaseName + ''' ' +  
    'AND UPPER(e.targetObject) LIKE ''' + @objectName + ''' ';  
    if( @eventCategory >= 0 )  
		set @whereClause = @whereClause + 'AND a.eventType=' + STR(@eventCategory)  
   
	if ( @alertLevel <> 0 )  
		set @whereClause = @whereClause + ' AND a.alertLevel = ' + STR(@alertLevel)  
	   
	if (@privilegedUserOnly = 1)  
		set @whereClause = @whereClause + ' and privilegedUser = 1 '  
		
	-- 5.2.4.5
	if (@loginName IS NOT NULL) 
		set @loginName = UPPER(dbo.fn_cmreport_ProcessString(@loginName)); 
		begin    
			if(@loginName = '<ALL>' or @loginName = '<All>')
				begin
					set @loginName = dbo.fn_cmreport_CreateMultiString(@loginName);  
					-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
					-- set @whereClause = @whereClause + '	AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) like ''' + @loginName + ''' OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) like ''' + @loginName + ''')'
					set @whereClause = @whereClause + '	AND UPPER(loginName) like ''' + @loginName + ''''
				end
			else
				begin
					set @loginName = dbo.fn_cmreport_CreateMultiString(@loginName);  
					-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
					-- set @whereClause = @whereClause + ' AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) IN (' + @loginName + ') OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) IN (' + @loginName + '))'
					set @whereClause = @whereClause + ' AND UPPER(loginName) IN (' + @loginName + ')'
				end
		end

	if(@schemaName <> NULL)
		begin
			set @schemaName = UPPER(dbo.fn_cmreport_ProcessString(@schemaName));
			set @whereClause = @whereClause + 'AND UPPER(e.parentName) like ''' + (@schemaName) + ''' '
		end
		

	if(@applicationName <> NULL)
		begin
			set @applicationName = UPPER(dbo.fn_cmreport_ProcessString(@applicationName));
			set @whereClause = @whereClause + 'AND UPPER(e.applicationName) like ''' + (@applicationName) + ''' '
		end

	if(@hostName <> NULL)
		begin
			set @hostName = UPPER(dbo.fn_cmreport_ProcessString(@hostName));
			set @whereClause = @whereClause + 'AND UPPER(e.hostName) like ''' + (@hostName) + ''' '
		end

	if(@event <> NULL)
		begin
			set @event = UPPER(dbo.fn_cmreport_ProcessString(@event));
			set @whereClause = @whereClause + 'AND UPPER(et.name) like ''' + (@event) + ''' '
		end
	-- 5.2.4.5
	   
 --set @whereClause = @whereClause + 'AND e.eventType = et.evtypeid '  
   
	if(@sortColumn = 'date')  
		set @orderByClause = ' ORDER BY alertTime DESC';  
	else  
		set @orderByClause = ' ORDER BY loginName' ;  
   
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
end	
	GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- no more needed in SQLcm 4.5
-- grant execute on sp_cmreport_GetAlerts to public
-- GO
