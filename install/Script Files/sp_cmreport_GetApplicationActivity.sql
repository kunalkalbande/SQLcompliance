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
 @loginName nvarchar(Max) = NULL,   
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
 @objectName nvarchar(256) = NULL,
 @applicationName nvarchar(256), 
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

if (@eventDatabase = '<ALL>')
	begin
	declare @eventDatabaseName nvarchar(128),
			@description nvarchar(512),
			@length int,
			@tempColumns nvarchar(2000)
		if (object_id(N'tempdb..#applicationActivity') IS NOT NULL)
			drop table #applicationActivity
		create table #applicationActivity (
			applicationName nvarchar(256),  
			details nvarchar(512),
			databaseName nvarchar(128), 
			eventType nvarchar(64),
			hostName nvarchar(128), 
			loginName nvarchar(128),
			targetObject nvarchar(512),
			startTime datetime,
			parentName nvarchar(256),
			sqlText nvarchar(MAX)
			) 
			
		if (@loginName <> NULL) 
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
	     set @eventDatabaseName = dbo.fn_cmreport_ProcessString(@eventDatabaseName);  
	     set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));  
	     set @applicationName = UPPER(dbo.fn_cmreport_ProcessString(@applicationName)); 
		 set @columns = 'Select TOP ' + STR(@rowCount) + ' * FROM ( select applicationName, details, databaseName, t2.name ''eventType'', hostName, ' +  
						-- Changing the select mechanism due to request on SQLCM - 6136 and SQLCM - 6137
						--'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +  
						--	   'THEN loginName ' +   
						--	   'ELSE sessionLoginName ' +   
						--  'END AS loginName, ' +  
						't1.loginName, ' +  
						'targetObject, t1.startTime, t1.parentName'  
		   set @fromClause = 'FROM [' + @eventDatabaseName + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '  
		 if (@showSqlText = 1)  
		 begin  
		  set @columns = @columns + ',(CASE WHEN LEN(t3.sqlText) > 32763  THEN SUBSTRING(t3.sqlText, 1, 32763) + '' ...'' ELSE t3.sqlText END) AS sqlText '  
		  set @fromClause = @fromClause + 'LEFT OUTER JOIN [' + @eventDatabaseName + '].dbo.EventSQL t3 ON t1.eventId = t3.eventId '  
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
		 
		-- 5.2.4.5
		if (@loginName <> NULL)
			begin    
				if(@loginName = '<ALL>' or @loginName = '<All>' or @loginName = '%')
					begin
						-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
						--set @whereClause = @whereClause + '	AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) like ''' + @loginName + ''' OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) like ''' + @loginName + ''')'
						set @whereClause = @whereClause + '	AND UPPER(loginName) like ''' + @loginName + ''''
					end
				else
					begin
						-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
						--set @whereClause = @whereClause + ' AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) IN (' + @loginName + ') OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) IN (' + @loginName + '))'
						set @whereClause = @whereClause + ' AND UPPER(loginName) IN (' + @loginName + ')'
					end
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
				set @whereClause = @whereClause + 'AND UPPER(t1.applicationName) like ''' + (@applicationName) + ''' '
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
			set @orderByClause = ' ORDER BY [temp].startTime DESC';  
		 else  
			set @orderByClause = ' ORDER BY loginName' ;  
	  set @tempTable = ') temp' ;

	if (@hourOfStartTime=NULL)
	begin
	set @stmt = @columns + @fromClause + @whereClause + @tempTable + @orderByClause;
	end
	else
	begin
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
	end

		insert into #applicationActivity exec (@stmt) 
		fetch eventDatabases into @eventDatabaseName, @description
		end
		close eventDatabases  
		deallocate eventDatabases
		select * from #applicationActivity
	end
	else
	begin 
  
   -- prevents sql injection but set limitations on the database naming  
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);  
   set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));  
   set @applicationName = UPPER(dbo.fn_cmreport_ProcessString(@applicationName)); 

   if (@loginName <> NULL) 
   begin
		set @loginName = UPPER(dbo.fn_cmreport_ProcessString(@loginName)); 
		set @loginName = dbo.fn_cmreport_CreateMultiString(@loginName);    
   end
   
 set @columns = 'Select TOP ' + STR(@rowCount) + ' * FROM ( select applicationName, details, databaseName, t2.name ''eventType'', hostName, ' +  
				-- Changing the select mechanism due to request on SQLCM - 6136 and SQLCM - 6137
                --'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +  
                --       'THEN loginName ' +   
                --       'ELSE sessionLoginName ' +   
                --  'END AS loginName, ' +
				't1.loginName, ' +    
                'targetObject, t1.startTime, t1.parentName'  
  
  
   set @fromClause = 'FROM [' + @eventDatabase + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '  
 if (@showSqlText = 1)  
 begin  
  set @columns = @columns + ',(CASE WHEN LEN(t3.sqlText) > 32763  THEN SUBSTRING(t3.sqlText, 1, 32763) + '' ...'' ELSE t3.sqlText END) AS sqlText '  
    
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
 
 -- 5.2.4.5
		if (@loginName <> NULL)      
			begin    
				if(@loginName = '<ALL>' or @loginName = '<All>' or @loginName = '%')
					begin
						-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
						--set @whereClause = @whereClause + '	AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) like ''' + @loginName + ''' OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) like ''' + @loginName + ''')'
						set @whereClause = @whereClause + '	AND UPPER(loginName) like ''' + @loginName + ''''
					end
				else
					begin
						-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
						--set @whereClause = @whereClause + ' AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) IN (' + @loginName + ') OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) IN (' + @loginName + '))'
						set @whereClause = @whereClause + ' AND UPPER(loginName) IN (' + @loginName + ')'
					end
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
				set @whereClause = @whereClause + 'AND UPPER(t1.applicationName) like ''' + (@applicationName) + ''' '
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
    set @orderByClause = ' ORDER BY [temp].startTime DESC';  
 else  
    set @orderByClause = ' ORDER BY loginName' ;  

	  set @tempTable = ') temp' ;
	if (@hourOfStartTime=NULL)
	begin
	set @stmt = @columns + @fromClause + @whereClause + @tempTable + @orderByClause;
	end
	else
	begin
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
	end
 
 EXEC(@stmt)
 
end
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- no more needed in SQLcm 4.5
-- grant execute on sp_cmreport_GetApplicationActivity to public
-- GO