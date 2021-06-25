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
	@applicationName nvarchar(256) = NULL,
	@loginStatus int,
	@sortColumn nvarchar(256),
	@rowCount int,
	@generatedFrom nvarchar(10) = NULL
	)
as
declare @stmt nvarchar(MAX)
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

if (@eventDatabase = '<ALL>')
	begin
	declare @eventDatabaseName nvarchar(128),
			@description nvarchar(512),
			@length int,
			@tempColumns nvarchar(2000)
			
		if (object_id(N'tempdb..#serverLoginHistory') IS NOT NULL)
			drop table #serverLoginHistory
			
		create table #serverLoginHistory (
			LoginName nvarchar(128), 
			EventType nvarchar(64), 
			ApplicationName nvarchar(128),
			DataCount int
			) 

	DECLARE eventDatabases CURSOR FOR 
	   SELECT DISTINCT SD.databaseName, SD.instance + CASE WHEN UPPER(databaseType) LIKE 'EVENT' THEN '' ELSE ' - ' + displayName END AS description
		  FROM SQLcompliance..SystemDatabases as SD left join Servers as S on (SD.instance = S.instance)
		  WHERE(UPPER(databaseType) IN ('ARCHIVE', 'EVENT'))
		  ORDER BY description;
    OPEN eventDatabases;
	FETCH eventDatabases INTO @eventDatabaseName, @description
	
	while @@Fetch_Status = 0
	begin

		-- Gets the data from the Application Activity proc for, then, format
	   -- the data the way we need, because we don't want Databas, EventTypes,
	   -- Logins in separate columns, we want it to be a category for our
	   -- summarization.

		INSERT #UserLoginHistory EXEC sp_cmreport_GetServerLoginActivityHelper
   		   @eventDatabaseName, 
		   @loginStatus, 
		   @startDate, 
		   @endDate, 
		   @hourOfStartTime,
		   @minuteOfStartTime,
		   @amPmOfStartTime, 
		   @hourOfEndTime, 
		   @minuteOfEndTime, 
		   @amPmOfEndTime,
		   @sortColumn,
		   @loginName,
		   @applicationName,
		   @rowCount,		   
		   @generatedFrom;  

		   set @stmt = 'SELECT  LoginName, EventType, ApplicationName, COUNT(StartTime) as DataCount FROM #UserLoginHistory GROUP BY LoginName, EventType, ApplicationName';

	   -- Now we develop the query to return the data we want devided by category.
	   insert into #serverLoginHistory EXEC(@stmt) 
	
		fetch eventDatabases into @eventDatabaseName, @description
		end
	
		close eventDatabases  
		deallocate eventDatabases
	
		select * from #serverLoginHistory

	end
else
begin

   -- Gets the data from the Application Activity proc for, then, format
   -- the data the way we need, because we don't want Databas, EventTypes,
   -- Logins in separate columns, we want it to be a category for our
   -- summarization.

   INSERT #UserLoginHistory EXEC sp_cmreport_GetServerLoginActivityHelper
   	   @eventDatabase, 
	   @loginStatus, 
	   @startDate, 
       @endDate, 
	   @hourOfStartTime,
	   @minuteOfStartTime,
	   @amPmOfStartTime, 
	   @hourOfEndTime, 
	   @minuteOfEndTime, 
	   @amPmOfEndTime,
	   @sortColumn,
	   @loginName,
	   @applicationName,
	   @rowCount,		   
	   @generatedFrom; 

   -- Now we develop the query to return the data we want devided by category.
   SELECT  LoginName, EventType, ApplicationName, COUNT(StartTime) as DataCount
   	FROM #UserLoginHistory
   	GROUP BY
   	LoginName, EventType, ApplicationName

   -- After using it, we now drop our temporary table to free memory space
   DROP TABLE #UserLoginHistory
   end
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
	@hourOfStartTime nvarchar (40), 
	@minuteOfStartTime nvarchar (40),
	@amPmOfStartTime nvarchar (40), 
	@hourOfEndTime nvarchar (40), 
	@minuteOfEndTime nvarchar (40), 
	@amPmOfEndTime nvarchar (40),
	@sortColumn nvarchar(256),
	@loginName nvarchar(max) = NULL,
	@applicationName nvarchar(256) = NULL,
	@rowCount int,
	@generatedFrom nvarchar(10) = NULL 
	)
as
declare @string nvarchar(4000), @columns nvarchar(1000), @fromClause nvarchar(500), @whereClause nvarchar(MAX), @orderByClause nvarchar(200);
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

   -- prevents sql injection but set limitations on the database naming
   set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);

   set @loginName = UPPER(dbo.fn_cmreport_ProcessString(@loginName)); 

	if (@generatedFrom = 'Windows')
	begin
		SET @loginName = dbo.fn_cmreport_CreateMultiString(@loginName);
	end

	set @columns = 'Select TOP ' + STR(@rowCount) + ' * FROM ( select applicationName, eventType, ' +
					-- Changing the select mechanism due to request on SQLCM - 6136 and SQLCM - 6137
	               --'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +
                --       'THEN loginName ' + 
                --       'ELSE sessionLoginName ' + 
                --  'END AS loginName, ' +
				'loginName, ' +  
                  'startTime ';
                  
   set @fromClause = 'FROM [' + @eventDatabase + '].dbo.Events  ';
	
	set @whereClause = 'where startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') ';
	
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
	
	set @string = @columns + @fromClause + @whereClause + @tempTable + @whereDate + @whereTime + @orderByClause ;

   print @string;
   EXEC(@string);

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO