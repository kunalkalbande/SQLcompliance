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

if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetApplicationActivitySummary' and xtype='P')
drop procedure [sp_cmreport_GetApplicationActivitySummary]
GO

CREATE PROC sp_cmreport_GetApplicationActivitySummary(
	@eventDatabase nvarchar(256), 
	@databaseName nvarchar(256), 
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
	@applicationName nvarchar(256), 
	@eventCategory int, 
	@privilegedUserOnly bit, 
	@rowCount int )
AS
BEGIN
declare @stmt nvarchar(4000)
declare @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(1000)  
declare @startDateStr nvarchar(40);  
declare @endDateStr nvarchar(40);
declare @concatedStartTime nvarchar(40);
declare @localStartTime nvarchar(40);
declare @concatedEndTime nvarchar(40);
declare @localEndTime nvarchar(40);
declare @tempTable nvarchar(10);
declare @whereDate nvarchar(500);
declare @whereTime nvarchar(1000);
   -- Idera SQL compliance Manager Version 3.0
   --
   -- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
   -- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
   -- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
   
	---- Concating all local strings of START TIMES
	set @concatedStartTime = @hourOfStartTime+':'+@minuteOfStartTime+' '+@amPmOfStartTime
	set @localStartTime = CAST(@concatedStartTime AS DATETIME2)

	---- Concating all local strings of END TIMES
	set @concatedEndTime = @hourOfEndTime+':'+ @minuteOfEndTime+' '+ @amPmOfEndTime
	set @localEndTime = CAST(@concatedEndTime AS DATETIME2)
	
	-- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);
   
if(@eventDatabase = "<ALL>")
begin
-- Creates the temporary table to store the result of the procedure which returns 
-- the result from the original "get_application_activity" proceudre for us to 
-- work on the returned data
CREATE TABLE #ApplicationActivities(
  ApplicationName	nvarchar (255),
  Details		nvarchar (300),
  DatabaseName		nvarchar (255),
  EventType		nvarchar (40),
  HostName		nvarchar (255),
  ParentName nvarchar(256),
  LoginName 		nvarchar (255),
  TargetObject		nvarchar (500),
  StartTime		DATETIME
)
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
			parentName nvarchar (256), 
			loginName nvarchar(128),
			targetObject nvarchar(512),
			startTime datetime
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
 
         -- Logins in separate columns, we want it to be a category for our
		 set @eventDatabaseName = dbo.fn_cmreport_ProcessString(@eventDatabaseName);  
		 set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));  
		 set @applicationName = UPPER(dbo.fn_cmreport_ProcessString(@applicationName));
   
		 set @columns = 'Select TOP ' + STR(@rowCount) + ' * FROM ( select applicationName, details, databaseName, t2.name ''eventType'', hostName, parentName, ' +  
						-- Changing the select mechanism due to request on SQLCM - 6136 and SQLCM - 6137
						--'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +  
						--	   'THEN loginName ' +   
						--	   'ELSE sessionLoginName ' +   
						--  'END AS loginName, ' +  
						't1.loginName, ' +  
						'targetObject, t1.startTime'  
		 set @fromClause = ' FROM [' + @eventDatabaseName + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '   
		 set @whereClause = 'where success =  1 and UPPER(t1.applicationName) like ''' + @applicationName + ''' and UPPER(databaseName) like ''' + @databaseName+ '''' +  
			' and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') ';  
		 
		 -- 5.2.4.5 -- 
		 if(@schemaName <> NULL)
			begin
				set @schemaName = UPPER(dbo.fn_cmreport_ProcessString(@schemaName));
				set @whereClause = @whereClause + 'AND UPPER(parentName) like ''' + (@schemaName) + ''' '
			end	
         -- 5.2.4.5 --

		 if(@eventCategory <> -1)  
			set @whereClause = @whereClause + ' and eventCategory=' + STR(@eventCategory) + ' ';  
  
		 if (@privilegedUserOnly = 1)  
		 begin  
		  set @whereClause = @whereClause + ' and privilegedUser = 1 '  
		 end  

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
		 set @stmt = @columns + @fromClause + @whereClause + @tempTable + @whereDate + @whereTime; 

		INSERT #ApplicationActivities EXEC (@stmt)

		fetch eventDatabases into @eventDatabaseName, @description
		end
		close eventDatabases  
		deallocate eventDatabases

-- Now we develop the query to return the data we want devided by category.
SELECT  ApplicationName,'Grouped By Database' as 'Category', DatabaseName as 'Data', COUNT(DatabaseName) 'DataCount'
	FROM #ApplicationActivities
	--WHERE DatabaseName IS NOT NULL AND DatabaseName != ''
	GROUP BY
	ApplicationName, DatabaseName

UNION ALL
SELECT  ApplicationName, 'Grouped By EventType' as 'Category', EventType as 'Data', COUNT(EventType) 'DataCount'
	FROM #ApplicationActivities
	--WHERE EventType IS NOT NULL AND EventType != ''
	GROUP BY 
	ApplicationName, EventType
UNION ALL
SELECT  ApplicationName , 'Grouped By LoginName' as 'Category', LoginName as 'Data', COUNT(LoginName) 'DataCount'
	FROM #ApplicationActivities
	--WHERE LoginName IS NOT NULL AND LoginName != ''
	GROUP BY
	ApplicationName, LoginName
UNION ALL
SELECT  ApplicationName, 'Grouped By TargetObject' as 'Category', TargetObject as 'Data', COUNT(TargetObject) 'DataCount'
	FROM #ApplicationActivities
	--WHERE TargetObject IS NOT NULL AND TargetObject != ''
	GROUP BY ApplicationName, TargetObject

-- After using it, we now drop our temporary table to free memory space
DROP TABLE #ApplicationActivities
	end
	else
	begin
	CREATE TABLE #ApplicationActivities2(
		applicationName	nvarchar (255),
		details nvarchar (300),
		databaseName nvarchar (255),
		eventType nvarchar (40),
		hostName nvarchar (255),
		parentName nvarchar(256),
		loginName nvarchar (255),
		targetObject nvarchar (500),
		startTime DATETIME
	)
 
	 set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);  
	 set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));  
	 set @applicationName = UPPER(dbo.fn_cmreport_ProcessString(@applicationName));
	 set @columns = 'Select TOP ' + STR(@rowCount) + ' * FROM ( select applicationName, details, databaseName, t2.name ''eventType'', hostName, parentName, ' +  
					-- Changing the select mechanism due to request on SQLCM - 6136 and SQLCM - 6137
					--'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +  
					--	   'THEN loginName ' +   
					--	   'ELSE sessionLoginName ' +   
					--  'END AS loginName, ' +  
					't1.loginName, ' +  
					'targetObject, t1.startTime'  
	   set @fromClause = ' FROM [' + @eventDatabase + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '   
	 set @whereClause = 'where success =  1 and UPPER(t1.applicationName) like ''' + @applicationName + ''' and UPPER(databaseName) like ''' + @databaseName+ '''' +  
		' and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') ';  

	 -- 5.2.4.5 -- 
	 if(@schemaName <> NULL)
		begin
			set @schemaName = UPPER(dbo.fn_cmreport_ProcessString(@schemaName));
			set @whereClause = @whereClause + 'AND UPPER(parentName) like ''' + (@schemaName) + ''' '
		end	
     -- 5.2.4.5 --

	 if(@eventCategory <> -1)  
		set @whereClause = @whereClause + ' and eventCategory=' + STR(@eventCategory) + ' ';  
	 if (@privilegedUserOnly = 1)  
	 begin  
	  set @whereClause = @whereClause + ' and privilegedUser = 1 '  
	 end  

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
	 set @stmt = @columns + @fromClause + @whereClause + @tempTable + @whereDate + @whereTime;
	INSERT #ApplicationActivities2 EXEC (@stmt)
	SELECT  applicationName,'Grouped By Database' as 'Category', databaseName as 'Data', COUNT(databaseName) 'DataCount'
		FROM #ApplicationActivities2
		GROUP BY
		applicationName, databaseName
	UNION ALL
	SELECT  applicationName, 'Grouped By EventType' as 'Category', eventType as 'Data', COUNT(eventType) 'DataCount'
		FROM #ApplicationActivities2
		GROUP BY 
		applicationName, eventType
	UNION ALL
	SELECT  applicationName , 'Grouped By LoginName' as 'Category', loginName as 'Data', COUNT(loginName) 'DataCount'
		FROM #ApplicationActivities2
		GROUP BY
		applicationName, loginName
	UNION ALL
	SELECT  applicationName, 'Grouped By TargetObject' as 'Category', targetObject as 'Data', COUNT(targetObject) 'DataCount'
		FROM #ApplicationActivities2
		GROUP BY applicationName, targetObject
	DROP TABLE #ApplicationActivities2
end

END
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- no more needed in SQLcm 4.5
-- grant execute on sp_cmreport_GetApplicationActivitySummary to public
-- GO