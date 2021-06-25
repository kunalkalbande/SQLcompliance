-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
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
if exists (select * from dbo.sysobjects where name = N'p_cmreport_GetSensitiveColumns' and xtype='P')
drop procedure [p_cmreport_GetSensitiveColumns]
GO
CREATE procedure p_cmreport_GetSensitiveColumns (  
	@eventDatabase nvarchar(256),   
	@databaseName nvarchar(256),    
	@tableName nvarchar(256),  
	@loginName nvarchar(MAX),   
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
	@columnName nvarchar(256),
	@applicationName nvarchar(256) = NULL, 
	@event nvarchar(256) = NULL,    
	@showSqlText bit,   
	@sortColumn nvarchar(1000),   
	@rowCount nvarchar(16),
	@generatedFrom nvarchar(10) = NULL
   )  
as  
declare @stmt nvarchar(MAX), @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(MAX), @orderByClause nvarchar(200)  
declare @startDateStr nvarchar(40);  
declare @endDateStr nvarchar(40);  
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
			
		if (object_id(N'tempdb..#sensitiveColumns') IS NOT NULL)
			drop table #sensitiveColumns
			
		create table #sensitiveColumns (
			applicationName nvarchar(128), 
			databaseName nvarchar(128),
			parentName nvarchar(128), 
			loginName nvarchar(128),
			eventTypeString nvarchar(64),
			startTime datetime, 
			tableName nvarchar(512),
			columnName nvarchar(128),
			eventId int,
			sqlText nvarchar(MAX)
			) 

		set @loginName = UPPER(dbo.fn_cmreport_ProcessString(@loginName));
		if (@generatedFrom = 'Windows')
		begin
			SET @loginName = dbo.fn_cmreport_CreateMultiString(@loginName);
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

		 -- prevents sql injection but set limitations on the database naming
	   set @eventDatabaseName = dbo.fn_cmreport_ProcessString(@eventDatabaseName);
	   set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));
	   set @tableName = UPPER(dbo.fn_cmreport_ProcessString(@tableName));

	   set @columns = 'Select TOP ' + STR(@rowCount) + ' * FROM ( select e.applicationName, e.databaseName, e.parentName, ' +
					  -- Changing the select mechanism due to request on SQLCM - 6136 and SQLCM - 6137
					  -- 'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +
						 --  'THEN loginName ' + 
						 --  'ELSE sessionLoginName ' + 
					  --'END AS loginName, ' +
					  'e.loginName, ' +  
					  'CASE WHEN eventType=1 THEN ''Select'' ' +
							'WHEN eventType=2 THEN ''Update'' '+
							'WHEN eventType=8 THEN ''Insert'' '+
							'WHEN eventType=16 THEN ''Delete'' '+
							'WHEN eventType=217 THEN ''Alter user Table'' '+
							'WHEN eventType=317 THEN ''Drop user Table'' '+
					  'ELSE ''Unknown'' END AS eventTypeString, ' +
					   'e.startTime, e.targetObject as ''tableName'', sc.columnName, e.eventId '
	               
	               
	   set @fromClause = 'FROM [' + @eventDatabaseName + '].dbo.Events as e ' + 
		'JOIN [' + @eventDatabaseName + '].dbo.SensitiveColumns as sc ON e.eventId = sc.eventId '
    
		if (@showSqlText = 1)
		begin
			set @columns = @columns + ', es.sqlText '
			set @fromClause = @fromClause +    'LEFT OUTER JOIN [' + @eventDatabaseName + '].dbo.EventSQL es ON e.eventId = es.eventId '
		end
		else
		begin
			set @columns = @columns + ', '''' ''sqlText'' '
		end

		 if (@generatedFrom = 'Windows' AND @loginName <> '%')
		begin
		-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
 		set @whereClause = 'where UPPER(databaseName) like ''' + @databaseName+ ''' ' +
	                   -- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
					   --'AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) IN (' + @loginName + ') OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) IN (' + @loginName + ')) ' +
                       ' AND UPPER(loginName) IN (' + @loginName + ') ' + 
						   'AND UPPER(targetObject) like ''' + @tableName + ''' ' +
	                   ' and e.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and e.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') '
		end 
		else
		begin
			-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
 			set @whereClause = 'where UPPER(databaseName) like ''' + @databaseName+ ''' ' +
	                   -- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
					   --'AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) like ''' + @loginName + ''' OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) like ''' + @loginName + ''') ' +
                      	'AND UPPER(loginName) like ''' + @loginName + ''' ' + 
						   'AND UPPER(targetObject) like ''' + @tableName + ''' ' +
	                   --'and eventType = 1 ' +
	                   ' and e.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and e.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') '
		end
	-- 5.2.4.5
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

		if(@columnName <> NULL)
			begin
				set @columnName = UPPER(dbo.fn_cmreport_ProcessString(@columnName));
				set @whereClause = @whereClause + 'AND UPPER(sc.columnName) like ''' + (@columnName) + ''' '
			end		
	-- 5.2.4.5
	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY temp.startTime DESC';
	else
	   set @orderByClause = ' ORDER BY loginName' ;

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
	
	-- 5.2.4.5
	if(@event <> NULL)
			begin
				set @event = UPPER(dbo.fn_cmreport_ProcessString(@event));
				set @stmt =  'Select * from (' + @stmt + ') as sensitiveColumn where UPPER(sensitiveColumn.eventTypeString) like ''' + UPPER(@event) + ''' '
		end
	-- 5.2.4.5
	insert into #sensitiveColumns exec (@stmt) 
	
	fetch eventDatabases into @eventDatabaseName, @description
	end
	
	close eventDatabases  
	deallocate eventDatabases
	
	select * from #sensitiveColumns
	end
	else
	begin
	   
  
  -- prevents sql injection but set limitations on the database naming  
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);  
   set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));  
   set @loginName = UPPER(dbo.fn_cmreport_ProcessString(@loginName));  
   set @tableName = UPPER(dbo.fn_cmreport_ProcessString(@tableName));    
   
   if (@generatedFrom = 'Windows')
	begin
   		SET @loginName = dbo.fn_cmreport_CreateMultiString(@loginName);
	end
 set @columns = 'Select TOP ' + STR(@rowCount) + ' * FROM ( select e.applicationName, e.databaseName, e.parentName,' +  
				-- Changing the select mechanism due to request on SQLCM - 6136 and SQLCM - 6137
                --'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +  
                --       'THEN loginName ' +   
                --       'ELSE sessionLoginName ' +   
                --  'END AS loginName, ' +  
				'e.loginName, ' + 
                  'CASE WHEN eventType=1 THEN ''Select'' ' +  
      'WHEN eventType=2 THEN ''Update'' '+  
      'WHEN eventType=8 THEN ''Insert'' '+  
      'WHEN eventType=16 THEN ''Delete'' '+  
      'WHEN eventType=217 THEN ''Alter user Table'' '+  
      'WHEN eventType=317 THEN ''Drop user Table'' '+  
                  'ELSE ''Unknown'' END AS eventTypeString, ' +  
                'e.startTime, e.targetObject as ''tableName'', sc.columnName, e.eventId '  
                  
                  
   set @fromClause = 'FROM [' + @eventDatabase + '].dbo.Events as e ' + 'JOIN [' + @eventDatabase + '].dbo.SensitiveColumns as sc ON e.eventId = sc.eventId '  
      
 if (@showSqlText = 1)  
 begin  
  set @columns = @columns + ', es.sqlText '  
  set @fromClause = @fromClause +    'LEFT OUTER JOIN [' + @eventDatabase + '].dbo.EventSQL es ON e.eventId = es.eventId '  
 end  
 else  
 begin  
  set @columns = @columns + ', '''' ''sqlText'' '  
 end  
  
 if (@generatedFrom = 'Windows' AND @loginName <> '%')
	begin
	-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
 	set @whereClause = 'where UPPER(databaseName) like ''' + @databaseName+ ''' ' +
	                   --'AND (UPPER(sessionLoginName) IN (' + @loginName + ') OR UPPER(loginName) IN (' + @loginName + ')) ' +
					   ' AND UPPER(loginName) IN (' + @loginName + ') ' + 
                      	   'AND UPPER(targetObject) like ''' + @tableName + ''' ' +
	                   --'and eventType = 1 ' +
	                   ' and e.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and e.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') '
	end 
 else
	begin
	-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
 	set @whereClause = 'where UPPER(databaseName) like ''' + @databaseName+ ''' ' +
	                   --'AND (UPPER(sessionLoginName) LIKE ''' + @loginName + ''' OR UPPER(loginName) LIKE ''' + @loginName + ''') ' +
					   '	AND UPPER(loginName) like ''' + @loginName + ''' ' + 
                      	   'AND UPPER(targetObject) like ''' + @tableName + ''' ' +
	                   --'and eventType = 1 ' +
	                   ' and e.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and e.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') '
	end
	
	-- 5.2.4.5
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

		if(@columnName <> NULL)
			begin
				set @columnName = UPPER(dbo.fn_cmreport_ProcessString(@columnName));
				set @whereClause = @whereClause + 'AND UPPER(sc.columnName) like ''' + (@columnName) + ''' '
			end

		-- 5.2.4.5

	if(@sortColumn = 'date')  
		set @orderByClause = ' ORDER BY temp.startTime DESC';  
	else  
		set @orderByClause = ' ORDER BY loginName' ; 
		
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
	-- 5.2.4.5
	if(@event <> NULL)
			begin
				set @event = UPPER(dbo.fn_cmreport_ProcessString(@event));
				set @stmt =  'Select * from (' + @stmt + ') as sensitiveColumn where UPPER(sensitiveColumn.eventTypeString) like ''' + UPPER(@event) + ''' '
		end
	-- 5.2.4.5

 EXEC(@stmt)  
end
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- no more needed in SQLcm 4.5
--grant execute on sp_cmreport_GetBeforeAfterData to public
--GO