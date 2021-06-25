-- Idera SQL compliance Manager version 3.0
-- Last modification date: 1/16/2018
--
-- (c) Copyright 2004-2018 IDERA, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, IDERA and the IDERA Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetRowCount' and xtype='P')
drop procedure [sp_cmreport_GetRowCount]
GO
CREATE procedure [dbo].[sp_cmreport_GetRowCount] (
	@eventDatabaseAbove2005 nvarchar(256), 
	@databaseNameAll nvarchar(256) = null,   
	@databaseName nvarchar(256) = null,  
	@tableName nvarchar(256),     
	@loginNameAbove2005 nvarchar(MAX),   
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
	@columnName nvarchar(256),  
	@applicationName nvarchar(256) = NULL,
	@event nvarchar(256) = NULL,  
	@showSqlText bit,
	@privilegedUsers nvarchar(256),
	@status int = null,
	@rowCountThreshold int,
	@generatedFrom nvarchar(10) = NULL
 )	
	as  
 declare @stmt nvarchar(4000)    
 declare @columns nvarchar(2000), @fromClause nvarchar(max), @whereClause nvarchar(1000), @orderByClause nvarchar(200)    
	declare @startDateStr nvarchar(40);  
	declare @endDateStr nvarchar(40); 
	declare @concatedStartTime nvarchar(40);
	declare @localStartTime nvarchar(40);
	declare @concatedEndTime nvarchar(40);
	declare @localEndTime nvarchar(40);
	declare @whereDate nvarchar(500);
	declare @whereTime nvarchar(1000);

	if (object_id(N'tempdb..#rowCountsData') IS NOT NULL)  
	drop table #rowCountsData  
     
	create table #rowCountsData (  
	roleName nvarchar(128),   
	spid int,   
	targetObject nvarchar(128),   
	serverName nvarchar(128),  
	parentName nvarchar(256),  	
	startTime dateTime,  
	databaseName nvarchar(256),  
	applicationName nvarchar(256),  
	loginName nvarchar(256),  
	rowCounts nvarchar(256),  
	eventTypeString nvarchar(256),  
	columnName nvarchar(256),  
	sqlText nvarchar(max))   

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
			  
	if (@eventDatabaseAbove2005 = '<ALL>')  
	begin  
	declare @eventDatabase nvarchar(128),  
	@description nvarchar(512),  
	@length int,  
	@tempColumns nvarchar(2000)  
     
	if (object_id(N'tempdb..#loginInfo') IS NOT NULL)  
	drop table #loginInfo  
     
	create table #loginInfo (  
	roleName nvarchar(128),   
	spid int,   
	targetObject nvarchar(128),   
	serverName nvarchar(128),  
	parentName nvarchar(256),  	
	startTime dateTime,  
	databaseName nvarchar(256),  
	applicationName nvarchar(256),  
	loginName nvarchar(256),  
	rowCounts nvarchar(256),  
	eventTypeString nvarchar(256),  
	columnName nvarchar(256),  
	sqlText nvarchar(max))   

	set @loginNameAbove2005 = UPPER(dbo.fn_cmreport_ProcessString(@loginNameAbove2005));
	if (@generatedFrom = 'Windows')
	begin
		SET @loginNameAbove2005 = dbo.fn_cmreport_CreateMultiString(@loginNameAbove2005);
	end	
  
	DECLARE eventDatabases CURSOR FOR   
	SELECT DISTINCT SD.databaseName, SD.instance + CASE WHEN UPPER(databaseType) LIKE 'EVENT' THEN '' ELSE ' - ' + displayName END AS description  
	FROM SQLcompliance..SystemDatabases as SD left join Servers as S on (SD.instance = S.instance)  
	WHERE(UPPER(databaseType) IN ('ARCHIVE', 'EVENT')) AND S.sqlVersion >= 10 -- Version is 9 for SQL server 2005  
	ORDER BY description;  
	OPEN eventDatabases;  
	FETCH eventDatabases INTO @eventDatabase, @description  
   
	while @@Fetch_Status = 0  
	begin  
   
  
	set @privilegedUsers = UPPER(dbo.fn_cmreport_ProcessString(@privilegedUsers));  
	set @columnName = UPPER(dbo.fn_cmreport_ProcessString(@columnName));  
	set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);  
	if(@databaseName IS NOT NULL)
	BEGIN
		set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));  
	END
	ELSE
	BEGIN
		set @databaseNameAll = UPPER(dbo.fn_cmreport_ProcessString(@databaseNameAll));
	END
	set @tableName = UPPER(dbo.fn_cmreport_ProcessString(@tableName));  
   
	set @columns =   
	-- Changing the select mechanism due to request on SQLCM - 6136 and SQLCM - 6137
	'select distinct   
	et.roleName, et.spid, et.targetObject, et.serverName,et.parentName,  
	et.startTime, et.databaseName, et.applicationName,  
	et.loginName,
	CASE WHEN et.rowCounts IS NULL  
	THEN ''N/A''  
	ELSE convert(varchar,et.rowCounts)  
	END AS rowCounts, evt.category AS eventTypeString,  
	CASE WHEN sc.columnName IS not NULL  
	then sc.columnName  
	when dc.columnName is not null  
	then dc.columnName  
	else ''''  
	end as columnName'  
  
	set @fromClause = '  
	FROM [' + @eventDatabase + '].dbo.Events as et'  
   
	if (@showSqlText = 0)  
	begin  
	set @columns = @columns + ', CASE WHEN tx.sqlText IS NULL  
	then ''''  
	else ''''  
	end as sqlText'  
		set @fromClause = @fromClause + '  LEFT JOIN [' + @eventDatabase + '].dbo.EventSQL tx ON tx.eventId = et.eventId'  
	end  
	else  
	begin  
		set @columns = @columns + ', tx.sqlText'  
		set @fromClause = @fromClause + '  LEFT JOIN [' + @eventDatabase + '].dbo.EventSQL tx ON tx.eventId = et.eventId'  
	end
	  
	set @whereClause = '  WHERE success in (1,0) '  
	
	if (@rowCountThreshold = 0)  
	begin  
		set @whereClause = @whereClause + '  AND (et.rowCounts >='+STR(@rowCountThreshold)+' OR et.rowCounts is NULL) '  
	end 
	else  
	begin  
		set @whereClause = @whereClause + '  AND et.rowCounts >=' + STR(@rowCountThreshold) + ' '  
	end  
	if(@databaseName IS NOT NULL)
	BEGIN
		set @whereClause = @whereClause + '  AND UPPER(databaseName) like ''' + @databaseName+ '''  AND UPPER(targetObject) like ''' + @tableName + ''' '   
	END
	ELSE
	BEGIN
		set @whereClause = @whereClause + '  AND UPPER(databaseName) like ''' + @databaseNameAll+ '''  AND UPPER(targetObject) like ''' + @tableName + ''' '  
	END
	if (@columnName <> '%')  
	begin     
		set @whereClause = @whereClause + '  AND (UPPER(sc.columnName) like ''' + @columnName + ''' OR UPPER(dc.columnName) like ''' + @columnName + ''')'  
		set @fromClause = @fromClause + '  LEFT JOIN [' + @eventDatabase + '].dbo.SensitiveColumns sc ON sc.eventId = et.eventId  
	LEFT JOIN (Select d1c.eventId, d1c.dcId,cc.columnName from [' + @eventDatabase + '].dbo.DataChanges d1c  
	INNER JOIN [' + @eventDatabase + '].dbo.ColumnChanges cc on d1c.dcId = cc.dcId  
	UNION  
  SELECT DD1.dcId,eventId, '''' as columnName from [' + @eventDatabase + '].dbo.DataChanges DD1 left join [' + @eventDatabase + '].dbo.ColumnChanges CC1 on DD1.dcId=CC1.dcId where CC1.dcId = NULL) dc ON (dc.eventId = et.eventId)'    
		set @fromClause = @fromClause + ' JOIN EventTypes evt on(evt.evtypeid = et.eventType)'  
	end  
	else  
	begin  
		set @fromClause = @fromClause + '  LEFT JOIN [' + @eventDatabase + '].dbo.SensitiveColumns sc ON sc.eventId = et.eventId  
	LEFT JOIN (Select d1c.eventId, d1c.dcId,cc.columnName from [' + @eventDatabase + '].dbo.DataChanges d1c  
	INNER JOIN [' + @eventDatabase + '].dbo.ColumnChanges cc on d1c.dcId = cc.dcId  
	UNION  
 SELECT DD1.dcId,eventId, '''' as columnName from [' + @eventDatabase + '].dbo.DataChanges DD1 left join [' + @eventDatabase + '].dbo.ColumnChanges CC1 on DD1.dcId=CC1.dcId where CC1.dcId = NULL) dc ON (dc.eventId = et.eventId)'    
		set @fromClause = @fromClause + ' join EventTypes evt on(evt.evtypeid = et.eventType)'  
	end  

	if (@privilegedUsers = '%')  
	begin  
		set @whereClause = @whereClause + '  AND (privilegedUser = 1 ) '  
	end  
	else if (@privilegedUsers <> '' AND @privilegedUsers <> '%')  
	begin  
	set @whereClause = @whereClause + '  AND (privilegedUser = 1 ' + 'AND (UPPER(loginName) LIKE ''' + @privilegedUsers + ''')) '  
	end 
	 
	set @whereClause = @whereClause + '  AND et.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''')  AND et.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') '  
	
	if (@generatedFrom = 'Windows' AND @loginNameAbove2005 <> '%') 
	begin
		-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
		--set @whereClause = @whereClause + ' AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) IN (' + @loginNameAbove2005 + ') OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) IN (' + @loginNameAbove2005 + '))'
		set @whereClause = @whereClause + ' AND UPPER(loginName) IN (' + @loginNameAbove2005 + ')'
	end
	else
	begin
		-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
		--set @whereClause = @whereClause + '	AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) like ''' + @loginNameAbove2005 + ''' OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) like ''' + @loginNameAbove2005 + ''')'
  set @whereClause = @whereClause + ' AND UPPER(loginName) like ''' + @loginNameAbove2005 + ''''  
	end
  	
	-- 5.2.4.5		
		if(@schemaName <> NULL)
			begin
				set @schemaName = UPPER(dbo.fn_cmreport_ProcessString(@schemaName));
				set @whereClause = @whereClause + 'AND UPPER(et.parentName) like ''' + (@schemaName) + ''' '
			end			

		
		if(@applicationName <> NULL)
			begin
				set @applicationName = UPPER(dbo.fn_cmreport_ProcessString(@applicationName));
				set @whereClause = @whereClause + 'AND UPPER(et.applicationName) like ''' + (@applicationName) + ''' '
			end

		if(@event <> NULL)
			begin
				set @event = UPPER(dbo.fn_cmreport_ProcessString(@event));
				set @whereClause = @whereClause + 'AND UPPER(evt.category) like ''' + (@event) + ''' '
		end
		-- 5.2.4.5
	set @orderByClause = '  ORDER BY startTime DESC' ;  

	if (@hourOfStartTime=NULL)
	begin
	set @stmt = @columns + @fromClause + @whereClause + @orderByClause;
	end
	else
	begin
	set @whereDate = ' WHERE CONVERT (Date, startTime) <= CONVERT(Date, '''+@endDateStr+''') and Convert (Date, startTime) >= CONVERT(Date, '''+@startDateStr+''') ';
	if(@localEndTime > @localStartTime)
	set @whereTime = 'AND (CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), startTime)) <= CONVERT(Time, '''+@localEndTime+''') AND CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), startTime)) >= CONVERT(Time, '''+@localStartTime+'''))'; 
	else
	set @whereTime =
	  'AND 
	  (
	    (
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), startTime)) <= CONVERT(Time, '''+@localEndTime+''') 
			AND 
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), startTime)) >= CONVERT(Time, ''1900-01-01 00:00:00.0000000'')
	    )
		OR
		(
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), startTime)) <= CONVERT(Time, ''1900-01-01 23:59:59.0000000'') 
			AND 
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), startTime)) >= CONVERT(Time, '''+@localStartTime+''')
	    )
	  )'

	  set @stmt = @columns + @fromClause + @whereClause

	  insert into #rowCountsData exec (@stmt);

	
	set @stmt = 'Select * FROM #rowCountsData ' + @whereDate + @whereTime + @orderByClause;
	end
    
	insert into #loginInfo exec (@stmt)   
   
	fetch eventDatabases into @eventDatabase, @description  
	end  
   
	close eventDatabases    
	deallocate eventDatabases  
   
	select * from #loginInfo  
	end  
	else  
	begin  
    
  
	set @privilegedUsers = UPPER(dbo.fn_cmreport_ProcessString(@privilegedUsers));  
	set @columnName = UPPER(dbo.fn_cmreport_ProcessString(@columnName));  
	set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabaseAbove2005);  
	if(@databaseName IS NOT NULL)
	BEGIN
		set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));  
	END
	ELSE
	BEGIN
		set @databaseNameAll = UPPER(dbo.fn_cmreport_ProcessString(@databaseNameAll));
	END
	set @loginNameAbove2005 = UPPER(dbo.fn_cmreport_ProcessString(@loginNameAbove2005));  
	set @tableName = UPPER(dbo.fn_cmreport_ProcessString(@tableName));  
	
	if (@generatedFrom = 'Windows')
	begin
		SET @loginNameAbove2005 = dbo.fn_cmreport_CreateMultiString(@loginNameAbove2005);
	end
	set @columns =   
	-- Changing the select mechanism due to request on SQLCM - 6136 and SQLCM - 6137
	'select distinct   
	et.roleName, et.spid, et.targetObject, et.serverName,et.parentName,  
	et.startTime, et.databaseName, et.applicationName,  
	et.loginName,
	CASE WHEN et.rowCounts IS NULL  
	THEN ''N/A''  
	ELSE convert(varchar,et.rowCounts)  
	END AS rowCounts,  
	evt.category AS eventTypeString,  
	CASE WHEN sc.columnName IS NOT NULL  
	then sc.columnName  
	when dc.columnName is not null  
	then dc.columnName  
	else ''''  
	end as columnName'  
  
	set @fromClause = '  FROM [' + @eventDatabase + '].dbo.Events as et'  
   
	if (@showSqlText = 0)  
	begin  
	set @columns = @columns + ', CASE WHEN tx.sqlText IS NULL  
	then ''''  
	else ''''  
	end as sqlText'  
		set @fromClause = @fromClause + '  LEFT JOIN [' + @eventDatabase + '].dbo.EventSQL tx ON tx.eventId = et.eventId'  
	end  
	else  
	begin  
		set @columns = @columns + ', tx.sqlText'  
		set @fromClause = @fromClause + '  LEFT JOIN [' + @eventDatabase + '].dbo.EventSQL tx ON tx.eventId = et.eventId'  
	end  
	set @whereClause = '  
	WHERE success in (1,0) '  
	if (@rowCountThreshold = 0)  
	begin  
		set @whereClause = @whereClause + '  AND (et.rowCounts >='+STR(@rowCountThreshold)+' OR et.rowCounts is NULL) '  
	end  
	else  
	begin  
		set @whereClause = @whereClause + '  AND et.rowCounts >=' + STR(@rowCountThreshold) + ' '  
	end 
	if(@databaseName IS NOT NULL)
	BEGIN
		set @whereClause = @whereClause + '  AND UPPER(databaseName) like ''' + @databaseName+ '''  AND UPPER(targetObject) like ''' + @tableName + ''' '
	END
	ELSE
	BEGIN
		set @whereClause = @whereClause + '  AND UPPER(databaseName) like ''' + @databaseNameAll+ '''  AND UPPER(targetObject) like ''' + @tableName + ''' '
	END  
		if (@columnName <> '%')  
		begin  
			set @whereClause = @whereClause + '  AND (UPPER(sc.columnName) like ''' + @columnName + ''' OR UPPER(dc.columnName) like ''' + @columnName + ''')'  
			set @fromClause = @fromClause + '  LEFT JOIN [' + @eventDatabase + '].dbo.SensitiveColumns sc ON sc.eventId = et.eventId  
	LEFT JOIN (Select d1c.eventId, d1c.dcId,cc.columnName from [' + @eventDatabase + '].dbo.DataChanges d1c  
	INNER JOIN [' + @eventDatabase + '].dbo.ColumnChanges cc on d1c.dcId = cc.dcId  
	UNION  
	SELECT DD1.dcId,eventId, '''' as columnName from [' + @eventDatabase + '].dbo.DataChanges DD1 left join [' + @eventDatabase + 	'].dbo.ColumnChanges CC1 on DD1.dcId=CC1.dcId where CC1.dcId = NULL) dc ON (dc.eventId = et.eventId)'    
	set @fromClause = @fromClause + ' join EventTypes evt on(evt.evtypeid = et.eventType)'  
	end  
	else  
	begin  
	set @fromClause = @fromClause + '  
	LEFT JOIN [' + @eventDatabase + '].dbo.SensitiveColumns sc ON sc.eventId = et.eventId  
	LEFT JOIN (Select d1c.eventId, d1c.dcId,cc.columnName from [' + @eventDatabase + '].dbo.DataChanges d1c  
	INNER JOIN [' + @eventDatabase + '].dbo.ColumnChanges cc on d1c.dcId = cc.dcId  
	UNION  
	SELECT DD1.dcId,eventId, '''' as columnName from [' + @eventDatabase + '].dbo.DataChanges DD1 left join [' + @eventDatabase + '].dbo.ColumnChanges CC1 on DD1.dcId=CC1.dcId where CC1.dcId = NULL) dc ON (dc.eventId = et.eventId)'    
	set @fromClause = @fromClause + ' join EventTypes evt on(evt.evtypeid = et.eventType)'  
	end  

	-- 5.2.4.5		
		if(@schemaName <> NULL)
			begin
				set @schemaName = UPPER(dbo.fn_cmreport_ProcessString(@schemaName));
				set @whereClause = @whereClause + 'AND UPPER(et.parentName) like ''' + (@schemaName) + ''' '
			end			

		
		if(@applicationName <> NULL)
			begin
				set @applicationName = UPPER(dbo.fn_cmreport_ProcessString(@applicationName));
				set @whereClause = @whereClause + 'AND UPPER(et.applicationName) like ''' + (@applicationName) + ''' '
			end

		if(@event <> NULL)
			begin
				set @event = UPPER(dbo.fn_cmreport_ProcessString(@event));
				set @whereClause = @whereClause + 'AND UPPER(evt.category) like ''' + (@event) + ''' '
		end
		-- 5.2.4.5

	if (@privilegedUsers = '%')  
	begin  
		set @whereClause = @whereClause + '  AND (privilegedUser = 1 ) '  
	end  
	else if (@privilegedUsers <> '' AND @privilegedUsers <> '%')  
	begin  
		set @whereClause = @whereClause + '  AND (privilegedUser = 1 ' + 'AND (UPPER(loginName) LIKE ''' + @privilegedUsers + ''')) '  
	end

	set @whereClause = @whereClause + '  AND et.startTime >= CONVERT(DATETIME, ''' + @startDateStr + ''')  AND et.startTime <= CONVERT(DATETIME, ''' + @endDateStr + ''') '  
	
	if (@generatedFrom = 'Windows' AND @loginNameAbove2005 <> '%') 
	begin
		-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
		--set @whereClause = @whereClause + ' AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) IN (' + @loginNameAbove2005 + ') OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) IN (' + @loginNameAbove2005 + '))'
		set @whereClause = @whereClause + ' AND UPPER(loginName) IN (' + @loginNameAbove2005 + ')'
	end
	else
	begin
		-- Changing the filter mechanism due to request on SQLCM - 6136 and SQLCM - 6137
		--set @whereClause = @whereClause + '	AND ((sessionLoginName IS NULL OR DATALENGTH(sessionLoginName) = 0) AND UPPER(loginName) like ''' + @loginNameAbove2005 + ''' OR (sessionLoginName IS NOT NULL OR DATALENGTH(sessionLoginName) > 0) AND UPPER(sessionLoginName) like ''' + @loginNameAbove2005 + ''')'
  set @whereClause = @whereClause + ' AND UPPER(loginName) like ''' + @loginNameAbove2005 + ''''  
	end
	
   	set @orderByClause = '  ORDER BY startTime DESC' ;  

	if (@hourOfStartTime=NULL)
	begin
	set @stmt = @columns + @fromClause + @whereClause + @orderByClause;
	end
	else
	begin
	set @whereDate = ' WHERE CONVERT (Date, startTime) <= CONVERT(Date, '''+@endDateStr+''') and Convert (Date, startTime) >= CONVERT(Date, '''+@startDateStr+''') ';
	if(@localEndTime > @localStartTime)
	set @whereTime = 'AND (CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), startTime)) <= CONVERT(Time, '''+@localEndTime+''') AND CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), startTime)) >= CONVERT(Time, '''+@localStartTime+'''))'; 
	else
	set @whereTime =
	  'AND 
	  (
	    (
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), startTime)) <= CONVERT(Time, '''+@localEndTime+''') 
			AND 
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), startTime)) >= CONVERT(Time, ''1900-01-01 00:00:00.0000000'')
	    )
		OR
		(
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), startTime)) <= CONVERT(Time, ''1900-01-01 23:59:59.0000000'') 
			AND 
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), startTime)) >= CONVERT(Time, '''+@localStartTime+''')
	    )
	  )'

	set @stmt = @columns + @fromClause + @whereClause

	  insert into #rowCountsData exec (@stmt);

	
	set @stmt = 'Select * FROM #rowCountsData ' + @whereDate + @whereTime + @orderByClause;
	end
	
	EXEC(@stmt)  
	end 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO