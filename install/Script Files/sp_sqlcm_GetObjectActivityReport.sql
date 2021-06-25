USE [SQLcompliance]
SET QUOTED_IDENTIFIER OFF 
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetObjectActivityReport]    Script Date: 6/7/2016 4:40:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

if exists (select * from dbo.sysobjects where name = N'sp_sqlcm_GetObjectActivityReport' and xtype='P')
drop procedure [sp_sqlcm_GetObjectActivityReport]

Go

CREATE procedure [dbo].[sp_sqlcm_GetObjectActivityReport] (
	@eventDatabaseAll nvarchar(256), 
	@databaseName nvarchar(256), 
	@loginName nvarchar(256),
	@objectName nvarchar(256), 
	@eventCategory int, 
	@startDate datetime, 
    @endDate datetime, 
	@privilegedUserOnly bit, 
	@showSqlText bit, 
	@sortColumn nvarchar(256), 
	@rowCount int)
as
declare @stmt nvarchar(4000)
declare @columns nvarchar(2000), @fromClause nvarchar(500), @whereClause nvarchar(1000), @orderByClause nvarchar(200)
declare @startDateStr nvarchar(40);
declare @endDateStr nvarchar(40);

    -- Process input
	set @startDateStr = dbo.fn_cmreport_GetDateString(@startDate);
	set @endDateStr = dbo.fn_cmreport_GetDateString(@endDate);

   -- prevents sql injection but set limitations on the database naming
   set @eventDatabaseAll = dbo.fn_cmreport_ProcessString(@eventDatabaseAll);
   set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));
   set @loginName = UPPER(dbo.fn_cmreport_ProcessString(@loginName));
   set @objectName = UPPER(dbo.fn_cmreport_ProcessString(@objectName));

	
	set @columns = 'select top ' + STR(@rowCount) + ' applicationName, details, databaseName, t2.name ''eventType'', hostName, ' +
	               'CASE WHEN sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0 ' +
                       'THEN loginName ' + 
                       'ELSE sessionLoginName ' + 
                  'END AS loginName, ' +
	               'targetObject, t1.startTime'
	               
	if (@showSqlText = 1)
		set @columns = @columns + ', t3.sqlText '
	else
		set @columns = @columns + ', '''' ''sqlText'' '

	set @whereClause = 'where success =  1 and UPPER(t1.targetObject) like ''' + @objectName + ''' and UPPER(databaseName) like ''' + @databaseName+ '''' +
		'AND (UPPER(sessionLoginName) LIKE ''' + @loginName + ''' OR UPPER(loginName) LIKE ''' + @loginName + ''') ' +
	   ' and t1.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''') and t1.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') ';
	
	if(@eventCategory <> -1)
	   set @whereClause = @whereClause + ' and eventCategory=' + STR(@eventCategory) + ' ';

	if (@privilegedUserOnly = 1)
	begin
		set @whereClause = @whereClause + ' and privilegedUser = 1 '
	end

	if(@sortColumn = 'date')
	   set @orderByClause = ' ORDER BY t1.startTime DESC';
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
		set @stmt = @tempColumns + @fromClause + @whereClause + @orderByClause
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
	set @stmt = @columns + @fromClause + @whereClause + @orderByClause
	exec(@stmt)
end

GO

