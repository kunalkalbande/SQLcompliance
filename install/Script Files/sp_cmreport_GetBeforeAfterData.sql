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
	@tableName nvarchar(256) = NULL, 
	@loginName nvarchar(MAX), 
	@startDate datetime, 
	@endDate datetime,
	@StartTimeofDay nvarchar (1) = NULL,
	@EndTimeofDay nvarchar (1) = NULL,
	@hourOfStartTime nvarchar (40) = NULL, 
	@hourOfEndTime nvarchar (40) = NULL, 
	@minuteOfStartTime nvarchar (40) = NULL,
	@minuteOfEndTime nvarchar (40) = NULL, 
	@amPmOfStartTime nvarchar (40) = NULL, 	
	@amPmOfEndTime nvarchar (40) = NULL,  
	@schemaName nvarchar(256) = NULL, 
	@objectName nvarchar(256), 
	@columnName nvarchar(256) = NULL,
	@event nvarchar(256) = NULL,    
	@sortColumn nvarchar(1000),   
	@rowCount nvarchar(16),  
	@primaryKey nvarchar(512),
	@generatedFrom nvarchar(10) = NULL
	)  
as  
declare @stmt nvarchar(MAX), @stmtXE nvarchar(MAX),@columns nvarchar(2000), @fromClause nvarchar(500),@fromClauseXE nvarchar(500), @whereClause nvarchar(MAX), @orderByClause nvarchar(200)  
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
	
if (@eventDatabase = '<ALL>')
	begin
	declare @eventDatabaseName nvarchar(128),
			@description nvarchar(512),
			@length int,
			@tempColumns nvarchar(2000)
			
		if (object_id(N'tempdb..#beforeAfterData') IS NOT NULL)
			drop table #beforeAfterData
			
		create table #beforeAfterData (
			applicationName nvarchar(128), 
			databaseName nvarchar(128), 
			loginName nvarchar(128),
			eventTypeString nvarchar(128),
			startTime datetime,
			parentName nvarchar(256),
			targetObject nvarchar(128),
			primaryKey nvarchar(4000),
			columnName nvarchar(128),
			beforeValue nvarchar(MAX),
			afterValue nvarchar(MAX)
			)

		if(@loginName <> NULL)
			set @loginName = UPPER(dbo.fn_cmreport_ProcessString(@loginName)); 

		if (@loginName <> NULL and @generatedFrom = 'Windows')
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
		   set @objectName = UPPER(dbo.fn_cmreport_ProcessString(@objectName));  
		   set @primaryKey = UPPER(dbo.fn_cmreport_ProcessString(@primaryKey)); 
 

		 set @columns = 'Select * FROM ( select top '+ STR(@rowCount) +' e.applicationName, e.databaseName, ' +  
						-- Changing the select mechanism due to request on SQLCM - 6136 and SQLCM - 6137
						--'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +  
						--	   'THEN loginName ' +   
						--	   'ELSE sessionLoginName ' +   
						--  'END AS loginName, ' +  
						'e.loginName, ' +  
						  'CASE WHEN eventType=8 THEN ''Insert'' ' +  
						  'WHEN eventType=2 THEN ''Update'' ' +  
						  'WHEN eventType=16 THEN ''Delete'' ' +  
						  'ELSE ''Unknown'' END AS eventTypeString, ' +  
						'e.startTime, e.parentName, e.targetObject, d.primaryKey,c.columnName,c.beforeValue,c.afterValue '  
                  
		   set @fromClause = 'FROM [' + @eventDatabaseName + '].dbo.Events as e ' +   
			'JOIN [' + @eventDatabaseName + '].dbo.DataChanges as d ON (d.startTime >= e.startTime AND d.startTime <= e.endTime ' +  
			'AND d.eventSequence >= e.startSequence AND d.eventSequence <= e.endSequence AND e.guid=NULL) ' +  
			'LEFT OUTER JOIN [' + @eventDatabaseName + '].dbo.ColumnChanges as c ON (c.startTime = d.startTime AND d.eventSequence = c.eventSequence)' 
			 
			set @fromClauseXE='FROM [' + @eventDatabaseName + '].dbo.Events as e ' +   
			'JOIN [' + @eventDatabaseName + '].dbo.DataChanges as d ON (d.startTime <= e.startTime AND d.startTime >= e.endTime ' +  
			'AND d.eventSequence <= e.startSequence AND d.eventSequence >= e.endSequence AND d.guid=e.guid) ' +  
			'LEFT OUTER JOIN [' + @eventDatabaseName + '].dbo.ColumnChanges as c ON (c.startTime = d.startTime AND d.eventSequence = c.eventSequence)'  

		
		 if (@generatedFrom = 'Windows' AND @loginName <> '%')
			begin
				-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
				set @whereClause = 'where UPPER(databaseName) like ''' + @databaseName+ ''' ' +
								--'AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) IN (' + @loginName + ') OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) IN (' + @loginName + ')) ' +
								'AND UPPER(loginName) IN (' + @loginName + ') ' + 
								'AND UPPER(targetObject) like ''' + @objectName + ''' ' +
								'AND UPPER(primaryKey) like ''' +'%'+ @primaryKey+'%' + ''' ' +
								'and eventType IN (8,2,16) ' +
								'AND recordNumber<>0 ' +
								' and e.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and e.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') '
			end
			else
			begin
				-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
				set @whereClause = 'where UPPER(databaseName) like ''' + @databaseName+ ''' ' +
								--'AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) like ''' + @loginName + ''' OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) like ''' + @loginName + ''') ' +
								'AND UPPER(loginName) like ''' + @loginName + ''' ' + 
								'AND UPPER(targetObject) like ''' + @objectName + ''' ' +
								'AND UPPER(primaryKey) like ''' +'%'+ @primaryKey+'%' + ''' ' +
								'and eventType IN (8,2,16) ' +
								'AND recordNumber<>0 ' +
								' and e.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and e.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') '
			end
		-- 5.2.4.5
		 if(@schemaName <> NULL)
			begin
				set @schemaName = UPPER(dbo.fn_cmreport_ProcessString(@schemaName));
				set @whereClause = @whereClause + 'AND UPPER(e.parentName) like ''' + (@schemaName) + ''' '
			end			

		if(@columnName <> NULL)
			begin
				set @columnName = UPPER(dbo.fn_cmreport_ProcessString(@columnName));
				set @whereClause = @whereClause + 'AND UPPER(c.columnName) like ''' + (@columnName) + ''' '
			end

		if(@tableName <> NULL)
			begin
				set @tableName = UPPER(dbo.fn_cmreport_ProcessString(@tableName));
				set @whereClause = @whereClause + 'AND UPPER(e.targetObject) like ''' + (@tableName) + ''' '
			end
		-- 5.2.4.5

		 if(@sortColumn = 'date')  
			set @orderByClause = ' ORDER BY e.startTime DESC';  
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
if (@hourOfStartTime <> NULL)
	begin
	  set @stmt = @columns + @fromClause + @whereClause + @orderByClause	
	  set @stmtXE=	@columns + @fromClauseXE + @whereClause + @orderByClause
		if(@event <> NULL)
				begin
					set @event = UPPER(dbo.fn_cmreport_ProcessString(@event));
					set @stmt =  'Select * from (' + @stmt + ') as beforeAfter where UPPER(beforeAfter.eventTypeString) like ''' + UPPER(@event) + ''' '
					set @stmtXE= 'Select * from (' + @stmtXE + ') as beforeAfter where UPPER(beforeAfter.eventTypeString) like ''' + UPPER(@event) + ''' '
				end
	  set @stmt = @stmt + @tempTable + @whereDate + @whereTime
	   set @stmtXE=	@stmtXE + @tempTable + @whereDate + @whereTime
	  end
else
	  begin
	  set @stmt = @columns + @fromClause + @whereClause + @tempTable + @orderByClause;
	  set @stmtXE=	@columns + @fromClauseXE + @whereClause+ @tempTable + @orderByClause
	  if(@event <> NULL)
				begin
					set @event = UPPER(dbo.fn_cmreport_ProcessString(@event));
					set @stmt =  'Select * from (' + @stmt + ') as beforeAfter where UPPER(beforeAfter.eventTypeString) like ''' + UPPER(@event) + ''' '
					set @stmtXE =  'Select * from (' + @stmtXE + ') as beforeAfter where UPPER(beforeAfter.eventTypeString) like ''' + UPPER(@event) + ''' '
			    end
      end
		-- 5.2.4.5
		  insert into #beforeAfterData exec (@stmt) 
		  insert into #beforeAfterData exec (@stmtXE) 

		 fetch eventDatabases into @eventDatabaseName, @description
		 end
	
		 close eventDatabases  
		 deallocate eventDatabases
	
		 select * from #beforeAfterData
	end
	else
	begin 
  
  	
		if (object_id(N'tempdb..#beforeAfterDataXE') IS NOT NULL)
			drop table #beforeAfterDataXE
			
		create table #beforeAfterDataXE (
			applicationName nvarchar(128), 
			databaseName nvarchar(128), 
			loginName nvarchar(128),
			eventTypeString nvarchar(128),
			startTime datetime,
			parentName nvarchar(256),
			targetObject nvarchar(128),
			primaryKey nvarchar(4000),
			columnName nvarchar(128),
			beforeValue nvarchar(MAX),
			afterValue nvarchar(MAX)
			)
  -- prevents sql injection but set limitations on the database naming  
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);  
   set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));  
   set @loginName = UPPER(dbo.fn_cmreport_ProcessString(@loginName));  
   set @objectName = UPPER(dbo.fn_cmreport_ProcessString(@objectName));  
   set @primaryKey = UPPER(dbo.fn_cmreport_ProcessString(@primaryKey));  
  if (@generatedFrom = 'Windows')
   begin
   	SET @loginName = dbo.fn_cmreport_CreateMultiString(@loginName);
   end
 

 set @columns = 'Select * FROM ( select top '+ STR(@rowCount) +' e.applicationName, e.databaseName, ' +  
                -- Changing the select mechanism due to request on SQLCM - 6136 and SQLCM - 6137
				--'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +  
                --       'THEN loginName ' +   
                --       'ELSE sessionLoginName ' +   
                --  'END AS loginName, ' +  
				'e.loginName, ' +  
                  'CASE WHEN eventType=8 THEN ''Insert'' ' +  
                  'WHEN eventType=2 THEN ''Update'' ' +  
                  'WHEN eventType=16 THEN ''Delete'' ' +  
                  'ELSE ''Unknown'' END AS eventTypeString, ' +  
                'e.startTime, e.parentName, e.targetObject, d.primaryKey,c.columnName,c.beforeValue,c.afterValue '  
                  
   set @fromClause = 'FROM [' + @eventDatabase + '].dbo.Events as e ' +   
    'JOIN [' + @eventDatabase + '].dbo.DataChanges as d ON (d.startTime >= e.startTime AND d.startTime <= e.endTime ' +  
    'AND d.eventSequence >= e.startSequence AND d.eventSequence <= e.endSequence AND e.guid=NULL) ' +  
    'LEFT OUTER JOIN [' + @eventDatabase + '].dbo.ColumnChanges as c ON (c.startTime = d.startTime AND d.eventSequence = c.eventSequence)'  

    set @fromClauseXE = 'FROM [' + @eventDatabase + '].dbo.Events as e ' +   
    'JOIN [' + @eventDatabase + '].dbo.DataChanges as d ON (d.startTime <= e.startTime AND d.startTime >= e.endTime ' +  
    'AND d.eventSequence <= e.startSequence AND d.eventSequence >= e.endSequence AND d.guid=e.guid) ' +  
    'LEFT OUTER JOIN [' + @eventDatabase + '].dbo.ColumnChanges as c ON (c.startTime = d.startTime AND d.eventSequence = c.eventSequence)'  
   
 if (@generatedFrom = 'Windows' AND @loginName <> '%')
	begin
		-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
		set @whereClause = 'where UPPER(databaseName) like ''' + @databaseName+ ''' ' +
						--'AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) IN (' + @loginName + ') OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) IN (' + @loginName + ')) ' +
						'AND UPPER(loginName) IN (' + @loginName + ') ' + 
						'AND UPPER(targetObject) like ''' + @objectName + ''' ' +
						'AND UPPER(primaryKey) like ''' +'%'+ @primaryKey+'%' + ''' ' +
						'and eventType IN (8,2,16) ' +
						'AND recordNumber<>0 ' +
						' and e.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and e.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') '
	end
	else
	begin
		-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
		set @whereClause = 'where UPPER(databaseName) like ''' + @databaseName+ ''' ' +
						--'AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) like ''' + @loginName + ''' OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) like ''' + @loginName + ''') ' +
						'AND UPPER(loginName) like ''' + @loginName + ''' ' + 
						'AND UPPER(targetObject) like ''' + @objectName + ''' ' +
						'AND UPPER(primaryKey) like ''' +'%'+ @primaryKey+'%' + ''' ' +
						'and eventType IN (8,2,16) ' +
						'AND recordNumber<>0 ' +
						' and e.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and e.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') '
	end
	-- 5.2.4.5
		 if(@schemaName <> NULL)
			begin
				set @schemaName = UPPER(dbo.fn_cmreport_ProcessString(@schemaName));
				set @whereClause = @whereClause + 'AND UPPER(e.parentName) like ''' + (@schemaName) + ''' '
			end			

		if(@columnName <> NULL)
			begin
				set @columnName = UPPER(dbo.fn_cmreport_ProcessString(@columnName));
				set @whereClause = @whereClause + 'AND UPPER(c.columnName) like ''' + (@columnName) + ''' '
			end

		if(@tableName <> NULL)
			begin
				set @tableName = UPPER(dbo.fn_cmreport_ProcessString(@tableName));
				set @whereClause = @whereClause + 'AND UPPER(e.targetObject) like ''' + (@tableName) + ''' '
			end
		-- 5.2.4.5
 if(@sortColumn = 'date')  
    set @orderByClause = ' ORDER BY e.startTime DESC';  
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
if (@hourOfStartTime <> NULL)
	   begin
	  set @stmt = @columns + @fromClause + @whereClause + @orderByClause
	  set @stmtXE = @columns + @fromClauseXE + @whereClause + @orderByClause	

		if(@event <> NULL)
				begin
					set @event = UPPER(dbo.fn_cmreport_ProcessString(@event));
					set @stmt =  'Select * from (' + @stmt + ') as beforeAfter where UPPER(beforeAfter.eventTypeString) like ''' + UPPER(@event) + ''' '
					set @stmtXE =  'Select * from (' + @stmtXE + ') as beforeAfter where UPPER(beforeAfter.eventTypeString) like ''' + UPPER(@event) + ''' '
			    end
	  set @stmt = @stmt + @tempTable + @whereDate + @whereTime
	  set @stmtXE = @stmtXE + @tempTable + @whereDate + @whereTime
	  end
else
	  begin
	if(@sortColumn = 'date')  
    set @orderByClause = ' ORDER BY temp.startTime DESC';  
	else  
    set @orderByClause = ' ORDER BY loginName' ;
	  set @stmt = @columns + @fromClause + @whereClause + @tempTable + @orderByClause;
	  set @stmtXE = @columns + @fromClauseXE + @whereClause + @tempTable + @orderByClause;
	  if(@event <> NULL)
				begin
					set @event = UPPER(dbo.fn_cmreport_ProcessString(@event));
					set @stmt =  'Select * from (' + @stmt + ') as beforeAfter where UPPER(beforeAfter.eventTypeString) like ''' + UPPER(@event) + ''' '
					set @stmtXE = 'Select * from (' + @stmtXE + ') as beforeAfter where UPPER(beforeAfter.eventTypeString) like ''' + UPPER(@event) + ''' '
			    end
      end


		-- 5.2.4.5
		insert into #beforeAfterDataXE exec (@stmt) 
		insert into #beforeAfterDataXE exec (@stmtXE) 

		 select * from #beforeAfterDataXE
 
end
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- no more needed in SQLcm 4.5
-- grant execute on sp_cmreport_GetBeforeAfterData to public
-- GO