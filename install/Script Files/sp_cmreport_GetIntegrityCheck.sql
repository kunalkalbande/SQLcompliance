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
CREATE procedure [dbo].[sp_cmreport_GetIntegrityCheck] (  
	@eventDatabase nvarchar(256),   
	@databaseName nvarchar(256) = NULL,   
	@loginName nvarchar(MAX) = NULL, 
	@fromConsole bit=0,
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
	@objectName nvarchar(256) = NULL,
	@applicationName nvarchar(256) = NULL,   
	@hostName nvarchar(256) = NULL,
	@event nvarchar(256) = NULL,  
	@showSqlText bit,   
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

	---- Concating all local strings of START TIMES
	set @concatedStartTime = @hourOfStartTime+':'+@minuteOfStartTime+' '+@amPmOfStartTime
	set @localStartTime = CAST(@concatedStartTime AS DATETIME2)

	---- Concating all local strings of END TIMES
	set @concatedEndTime = @hourOfEndTime+':'+ @minuteOfEndTime+' '+ @amPmOfEndTime
	set @localEndTime = CAST(@concatedEndTime AS DATETIME2)
	
	-- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);
  
   -- Idera SQL compliance Manager Version 3.0  
   --  
   -- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.  
   -- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks  
   -- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.  
  
if (@eventDatabase = '<ALL>')
	begin
	declare @eventDatabaseName nvarchar(128),
			@description nvarchar(512),
			@length int,
			@tempColumns nvarchar(2000)
			
		if (object_id(N'tempdb..#integrityCheck') IS NOT NULL)
			drop table #integrityCheck
			
		create table #integrityCheck (
			databaseName nvarchar(128), 
			applicationName nvarchar(128), 
			eventType nvarchar(64),
			hostName nvarchar(128), 
			details nvarchar(512),
			parentName nvarchar(256),
			loginName nvarchar(128),
			startTime datetime,
			sqlText nvarchar(max),
			targetObject nvarchar(128)
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
		 if(@schemaName = '')
			begin
				set @schemaName = NULL
			end 
  
		  -- prevents sql injection but set limitations on the database naming  
		   set @eventDatabaseName = dbo.fn_cmreport_ProcessString(@eventDatabaseName);  
		   set @schemaName = UPPER(dbo.fn_cmreport_ProcessString(@schemaName));  
  
  
		 set @columns = 'Select TOP ' + STR(@rowCount) + ' * FROM ( select databaseName, applicationName, t2.name ''eventType'', hostName, details,t1.parentName,' +  
						-- Changing the select mechanism due to request on SQLCM - 6136 and SQLCM - 6137
						--'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +  
						--	   'THEN loginName ' +   
						--	   'ELSE sessionLoginName ' +   
						--  'END AS loginName, ' +
						't1.loginName, ' +    
						't1.startTime'  
  
  
		   set @fromClause = 'FROM [' + @eventDatabaseName + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '  
  
		 if (@showSqlText = 1)  
		 begin  
		  set @columns = @columns + ', t3.sqlText, targetObject '  
		  set @fromClause = @fromClause +    'LEFT OUTER JOIN [' + @eventDatabaseName + '].dbo.EventSQL t3 ON t1.eventId = t3.eventId ';  
		 end  
		 else  
		 begin  
		  set @columns = @columns + ', '''' ''sqlText'', targetObject '  
		 end  
  
		 set @whereClause = 'where eventCategory = 0 and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + 
		 @endDateStr + ''') '  
		 
		 -- 5.2.4.5
		if (@loginName <> NULL)      
			begin    
				if(@loginName = '<ALL>' or @loginName = '<All>' or  @loginName = '%')
					begin
						-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
						--set @whereClause = @whereClause + '	AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) like ''' + @loginName + ''' OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) like ''' + @loginName + ''')'
						set @whereClause = @whereClause + '	AND UPPER(loginName) like ''' + @loginName + ''''
					end
				else
					begin
					-- Added new code for SQLCM-6276
					if(@fromConsole=0)
					begin
					declare @c  int,@c1 int,@loginname1 nvarchar(max)
					set @loginName1=@loginName
					Exec [sp_cmreport_GetAllLogins_Count]  @eventDatabaseName,@c output
					SELECT @c1=SUM(len(@loginName1) - len(replace(@loginName1, ',', '')) +1) 
					if(@c=@c1)
					begin
						set @whereClause = @whereClause
						end
						else
						begin
						set @whereClause = @whereClause + ' AND UPPER(loginName) IN (' + @loginName + ')'
						end
					end
					else
					begin
						-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
						--set @whereClause = @whereClause + ' AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) IN (' + @loginName + ') OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) IN (' + @loginName + '))'
						set @whereClause = @whereClause + ' AND UPPER(loginName) IN (' + @loginName + ')'
						end
					end
			end

		if(@databaseName <> NULL)
			begin
				set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));
				set @whereClause = @whereClause + 'AND UPPER(databaseName) like ''' + (@databaseName) + ''' '
			end

		if(@schemaName <> NULL)
			begin
				set @schemaName = UPPER(dbo.fn_cmreport_ProcessString(@schemaName));
				set @whereClause = @whereClause + 'AND UPPER(t1.parentName) like ''' + (@schemaName) + ''' '
			end		
			
		if(@objectName <> NULL)
			begin
				set @objectName = UPPER(dbo.fn_cmreport_ProcessString(@objectName));
				set @whereClause = @whereClause + 'AND UPPER(targetObject) like ''' + (@objectName) + ''' '
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
		
		insert into #integrityCheck exec (@stmt) 
	
		fetch eventDatabases into @eventDatabaseName, @description
		end
	
		close eventDatabases  
		deallocate eventDatabases
	
		select * from #integrityCheck
	end
	else
	begin

 if(@schemaName = '')
	begin
		set @schemaName = NULL
	end 
  
  -- prevents sql injection but set limitations on the database naming  
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);  
   set @schemaName = UPPER(dbo.fn_cmreport_ProcessString(@schemaName));  
  
  
 set @columns = 'Select TOP ' + STR(@rowCount) + ' * FROM ( select databaseName, applicationName, t2.name ''eventType'', hostName, details,t1.parentName,' +  
                -- Changing the select mechanism due to request on SQLCM - 6136 and SQLCM - 6137
				--'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +  
                --       'THEN loginName ' +   
                --       'ELSE sessionLoginName ' +   
                --  'END AS loginName, ' +
				't1.loginName, ' +    
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
  
 set @whereClause = 'where eventCategory = 0 and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + 
 @endDateStr + ''') '  
 
 -- 5.2.4.5
		if (@loginName <> NULL) 
			set @loginName = UPPER(dbo.fn_cmreport_ProcessString(@loginName));     
			begin    
				if(@loginName = '<ALL>' or @loginName = '<All>')
					begin
						set @loginName = dbo.fn_cmreport_CreateMultiString(@loginName);
						-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
						--set @whereClause = @whereClause + '	AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) like ''' + @loginName + ''' OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) like ''' + @loginName + ''')'
						set @whereClause = @whereClause + '	AND UPPER(loginName) like ''' + @loginName + ''''
					end
				else
					begin
					 set @loginName = dbo.fn_cmreport_CreateMultiString(@loginName);
					 -- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
					 --set @whereClause = @whereClause + ' AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) IN (' + @loginName + ') OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) IN (' + @loginName + '))'
					 set @whereClause = @whereClause + ' AND UPPER(loginName) IN (' + @loginName + ')'
					end
			end

		if(@databaseName <> NULL)
			begin
				set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));
				set @whereClause = @whereClause + 'AND UPPER(databaseName) like ''' + (@databaseName) + ''' '
			end

		if(@schemaName <> NULL)
			begin
				set @schemaName = UPPER(dbo.fn_cmreport_ProcessString(@schemaName));
				set @whereClause = @whereClause + 'AND UPPER(t1.parentName) like ''' + (@schemaName) + ''' '
			end		
			
		if(@objectName <> NULL)
			begin
				set @objectName = UPPER(dbo.fn_cmreport_ProcessString(@objectName));
				set @whereClause = @whereClause + 'AND UPPER(targetObject) like ''' + (@objectName) + ''' '
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
  
 EXEC(@stmt)  
 end
GO

-- no more needed in SQLcm 4.5
-- grant execute on sp_cmreport_GetIntegrityCheck to public
-- GO

