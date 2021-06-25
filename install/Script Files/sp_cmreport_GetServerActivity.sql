USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_cmreport_GetServerActivity]    Script Date: 9/19/2019 8:14:25 PM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetServerActivity' and xtype='P')
drop procedure [sp_cmreport_GetServerActivity]
GO


CREATE procedure [dbo].[sp_cmreport_GetServerActivity] (  
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
 @serverActivityCategory nvarchar(10) =NULL,
 @eventCategory int,
 @event nvarchar(256) = NULL,
 @sortColumn nvarchar(256),
 @rowCount int
 )
as  
declare @stmt nvarchar(4000)  
declare @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(1000), @orderByClause nvarchar(200)  
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
		if (object_id(N'tempdb..#applicationActivity') IS NOT NULL)
			drop table #applicationActivity
		create table #applicationActivity (
			serverName nvarchar (512),
			databaseName nvarchar(128), 		
			eventCategory nvarchar(128),	
			eventType nvarchar(64),
			startTime datetime,
			details nvarchar(512),	
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
	     set @eventDatabaseName = dbo.fn_cmreport_ProcessString(@eventDatabaseName);  
	     set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));  	     
		 set @columns = 'Select TOP ' + STR(@rowCount) + ' * FROM ( select serverName, databaseName, t2.category ''eventCategory'', t2.name ''eventType'', t1.startTime, details '						 
		 set @fromClause = 'FROM [' + @eventDatabaseName + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '		   
		 set @whereClause = 'where success IN(1,0) and UPPER(databaseName) like ''' + @databaseName+ '''' +  
			' and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') ';  
		 if(@eventCategory <> -1)  
			set @whereClause = @whereClause + ' and eventCategory=' + STR(@eventCategory) + ' ';  

		if(@event <> NULL)
			begin
				set @event = UPPER(dbo.fn_cmreport_ProcessString(@event));
				set @whereClause = @whereClause + 'AND UPPER(t2.name) like ''' + (@event) + ''' '
		end

		 if(@sortColumn = 'date')  
			set @orderByClause = ' ORDER BY temp.startTime DESC';

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
   
 set @columns = 'Select TOP ' + STR(@rowCount) + ' * FROM ( select serverName, databaseName, t2.category ''eventCategory'', t2.name ''eventType'', t1.startTime, details '   
 set @fromClause = 'FROM [' + @eventDatabase + '].dbo.Events t1 LEFT OUTER JOIN EventTypes t2 ON t1.eventType = t2.evtypeid '   
 set @whereClause = 'where success IN (1,0) and UPPER(databaseName) like ''' + @databaseName+ '''' +  
    ' and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') ';  
   
 if(@eventCategory <> -1)  
    set @whereClause = @whereClause + ' and eventCategory=' + STR(@eventCategory) + ' ';  
    

if(@event <> NULL)
	begin
		set @event = UPPER(dbo.fn_cmreport_ProcessString(@event));
		set @whereClause = @whereClause + 'AND UPPER(t2.name) like ''' + (@event) + ''' '
end
  
 if(@sortColumn = 'date')  
    set @orderByClause = ' ORDER BY temp.startTime DESC'; 

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

