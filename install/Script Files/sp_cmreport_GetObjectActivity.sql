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
	@eventDatabaseAll nvarchar(256),   
	@databaseName nvarchar(256),
	@loginNameAll nvarchar(MAX) = NULL,  
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
	@schemaName nvarchar(256) = NULL,
	@objectName nvarchar(256),
	@applicationName nvarchar(256) = NULL, 
	@hostName nvarchar(256) = NULL,  
	@eventCategory int,
	@event nvarchar(256) = NULL,   
	@showSqlText bit,   
	@privilegedUserOnly bit,
	@sortColumn nvarchar(256),   
	@rowCount int
	)  
as  
declare @stmt nvarchar(MAX)  
declare @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(MAX), @orderByClause nvarchar(200)  
declare @startDateStr nvarchar(40);  
declare @endDateStr nvarchar(40);  
declare @concatedStartTime nvarchar(40);
declare @localStartTime nvarchar(40);
declare @concatedEndTime nvarchar(40);
declare @localEndTime nvarchar(40);
declare @tempTable nvarchar(10);
declare @whereDate nvarchar(500);
declare @whereTime nvarchar(1000);

if (@hourOfStartTime=NULL)
	BEGIN
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate)	;
	END
else

BEGIN
    
	---- Concating all local strings of START TIMES
	set @concatedStartTime = @hourOfStartTime+':'+@minuteOfStartTime+' '+@amPmOfStartTime
	set @localStartTime = CAST(@concatedStartTime AS DATETIME2)

	---- Concating all local strings of END TIMES
	set @concatedEndTime = @hourOfEndTime+':'+ @minuteOfEndTime+' '+ @amPmOfEndTime
	set @localEndTime = CAST(@concatedEndTime AS DATETIME2)
	-- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

	END  
  
  
    -- prevents sql injection but set limitations on the database naming  
    set @eventDatabaseAll = dbo.fn_cmreport_ProcessString(@eventDatabaseAll);  
    set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));  
    set @objectName = UPPER(dbo.fn_cmreport_ProcessString(@objectName));  
  
   
 set @columns = 'Select TOP ' + STR(@rowCount) + ' * FROM ( select applicationName, details, databaseName, t2.name ''eventType'', hostName,t1.parentName, ' +  
				-- Changing the select mechanism due to request on SQLCM - 6136 and SQLCM - 6137
                --'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +  
                --       'THEN loginName ' +   
                --       'ELSE sessionLoginName ' +   
                --  'END AS loginName, ' +  
				't1.loginName, ' + 
                'targetObject, t1.startTime'  
                  
 if (@showSqlText = 1)  
  set @columns = @columns + ', t3.sqlText '  
 else  
  set @columns = @columns + ', '''' ''sqlText'' '  
  
 set @whereClause = 'where success =  1 and UPPER(t1.targetObject) like ''' + @objectName + ''' and UPPER(databaseName) like ''' + @databaseName+ '''' +  
    ' and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') ';  
	
	-- 5.2.4.5
		if (@loginNameAll <> NULL) 
			set @loginNameAll = UPPER(dbo.fn_cmreport_ProcessString(@loginNameAll));     
			begin    
				if(@loginNameAll = '<ALL>' or @loginNameAll = '<All>')
					begin
						set @loginNameAll = dbo.fn_cmreport_CreateMultiString(@loginNameAll);
						-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
						--set @whereClause = @whereClause + '	AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) like ''' + @loginNameAll + ''' OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) like ''' + @loginNameAll + ''')'
						set @whereClause = @whereClause + '	AND UPPER(loginName) like ''' + @loginNameAll + ''''
					end
				else
					begin
						set @loginNameAll = dbo.fn_cmreport_CreateMultiString(@loginNameAll);
						-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
						--set @whereClause = @whereClause + ' AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) IN (' + @loginNameAll + ') OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) IN (' + @loginNameAll + '))'
						set @whereClause = @whereClause + ' AND UPPER(loginName) IN (' + @loginNameAll + ')'
					end
			end

		if(@schemaName <> NULL)
			begin
				set @schemaName = UPPER(dbo.fn_cmreport_ProcessString(@schemaName));
				set @whereClause = @whereClause + 'AND UPPER(t1.parentName) like ''' + (@schemaName) + ''' '
			end			

		if(@applicationName <> NULL)
			begin
				set @applicationName = UPPER(dbo.fn_cmreport_ProcessString(@applicationName));
				set @whereClause = @whereClause + 'AND UPPER(applicationName) like ''' + (@applicationName) + ''' '
			end

		if(@hostName <> NULL)
			begin
				set @hostName = UPPER(dbo.fn_cmreport_ProcessString(@hostName));
				set @whereClause = @whereClause + 'AND UPPER(hostName) like ''' + (@hostName) + ''' '
			end

		if(@event <> NULL)
			begin
				set @event = UPPER(dbo.fn_cmreport_ProcessString(@event));
				set @whereClause = @whereClause + 'AND UPPER(t2.name) like ''' + (@event) + ''' '
		end
		-- 5.2.4.5

	if(@eventCategory <> -1)  
	   set @whereClause = @whereClause + ' and eventCategory=' + STR(@eventCategory) + ' ';  
	 
	if (@privilegedUserOnly = 1)  
	begin  
	 set @whereClause = @whereClause + ' and privilegedUser = 1 '  
	end  

	if(@sortColumn = 'date')  
	   set @orderByClause = ' ORDER BY temp.startTime DESC';  
	else  
	   set @orderByClause = ' ORDER BY loginName' ;  
   
if (@eventDatabaseAll = '<ALL>')  
begin  
 declare @eventDatabase nvarchar(128),  
   @description nvarchar(512),  
   @length int,  
   @tempColumns nvarchar(2000)  
    
  if (object_id(N'tempdb..#cmObjectActivity') IS NOT NULL)     
   drop table #cmObjectActivity  
  
  create table #cmObjectActivity (  
   applicationName nvarchar(128),  
   details nvarchar(512),  
   databaseName nvarchar(128),  
   eventType nvarchar(64),  
   hostName nvarchar(128),  
   parentName nvarchar(256),     
   loginName nvarchar(128),  
   targetObject nvarchar(512),  
   startTime dateTime,  
   sqlText [nvarchar](MAX),  
   instanceName nvarchar(256))  
  
 DECLARE eventDatabases CURSOR FOR   
    SELECT DISTINCT databaseName, instance + CASE WHEN UPPER(databaseType) LIKE 'EVENT' THEN '' ELSE ' - ' + displayName END AS description  
    FROM SQLcompliance..SystemDatabases  
    WHERE(UPPER(databaseType) IN ('ARCHIVE', 'EVENT'))  
    ORDER BY description;  
    OPEN eventDatabases;  
 FETCH eventDatabases INTO @eventDatabase, @description  
   
 while @@Fetch_Status = 0  
 begin  
  set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);  
  set @fromClause = 'FROM [' + @eventDatabase + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '  
  
  if (@showSqlText = 1)  
   set @fromClause = @fromClause + 'LEFT OUTER JOIN [' + @eventDatabase + '].dbo.EventSQL t3 ON t1.eventId = t3.eventId '  
  
  set @tempColumns = @columns + ', ''' + @description + ''' as ''instanceName'' '  
  	  set @tempTable = ') temp' ;
	  set @whereDate = ' WHERE CONVERT (Date, [temp].startTime) <= CONVERT(Date, '''+@endDateStr+''') and Convert (Date, [temp].startTime) >= CONVERT(Date, '''+@startDateStr+''') ';
	  if(@localEndTime > @localStartTime)
	  set @whereTime = 'AND (CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].startTime)) <= CONVERT(Time, '''+@localEndTime+''') AND CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].startTime)) >= CONVERT(Time, '''+@localStartTime+'''))';
	else
	set @whereTime =
	  'AND 
	  (
	    (
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].startTime)) <= CONVERT(Time, '''+@localEndTime+''') 
			AND 
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].startTime)) >= CONVERT(Time, ''1900-01-01 00:00:00.0000000'')
	    )
		OR
		(
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].startTime)) <= CONVERT(Time, ''1900-01-01 23:59:59.0000000'') 
			AND 
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].startTime)) >= CONVERT(Time, '''+@localStartTime+''')
	    )
	  )'

  set @stmt = @tempColumns + @fromClause + @whereClause + @tempTable + @whereDate + @whereTime + @orderByClause ;
  insert into #cmObjectActivity exec (@stmt)   
  
 fetch eventDatabases into @eventDatabase, @description  
 end  
   
 close eventDatabases    
 deallocate eventDatabases  
   
 select * from #cmObjectActivity  
end  
else  
begin  
    set @fromClause = 'FROM [' + @eventDatabaseAll + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '  
  
 if (@showSqlText = 1)  
  set @fromClause = @fromClause + 'LEFT OUTER JOIN [' + @eventDatabaseAll + '].dbo.EventSQL t3 ON t1.eventId = t3.eventId '  
  
 set @columns = @columns + ', ''' + @eventDatabaseAll + ''' as ''instanceName'' ' 
  	  set @tempTable = ') temp' ;
	  set @whereDate = ' WHERE CONVERT (Date, [temp].startTime) <= CONVERT(Date, '''+@endDateStr+''') and Convert (Date, [temp].startTime) >= CONVERT(Date, '''+@startDateStr+''') ';
	  if(@localEndTime > @localStartTime)
	  set @whereTime = 'AND (CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].startTime)) <= CONVERT(Time, '''+@localEndTime+''') AND CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].startTime)) >= CONVERT(Time, '''+@localStartTime+'''))';
	else
	set @whereTime =
	  'AND 
	  (
	    (
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].startTime)) <= CONVERT(Time, '''+@localEndTime+''') 
			AND 
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].startTime)) >= CONVERT(Time, ''1900-01-01 00:00:00.0000000'')
	    )
		OR
		(
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].startTime)) <= CONVERT(Time, ''1900-01-01 23:59:59.0000000'') 
			AND 
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].startTime)) >= CONVERT(Time, '''+@localStartTime+''')
	    )
	  )'
 set @stmt = @columns + @fromClause + @whereClause + @tempTable + @whereDate + @whereTime + @orderByClause;
 exec(@stmt)  
end 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- no more needed in SQLcm 4.5
-- grant execute on sp_cmreport_GetObjectActivity to public
-- GO
