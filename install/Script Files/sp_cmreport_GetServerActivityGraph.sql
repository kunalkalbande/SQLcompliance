USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_cmreport_GetServerActivityGraph]    Script Date: 9/19/2019 8:14:42 PM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetServerActivityGraph' and xtype='P')
drop procedure [sp_cmreport_GetServerActivityGraph]
GO


CREATE procedure [dbo].[sp_cmreport_GetServerActivityGraph] (
	@eventDatabase nvarchar(256),  
	@databaseName nvarchar(50),
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
	@serverActivityCategory nvarchar(10) ,
	@eventCategory int=NULL,
	@event nvarchar(256) = NULL,
	@sortColumn nvarchar(256) = NULL,
	@rowCount int=NULL
)
as

declare @stmt nvarchar(2000)
declare @selectClause nvarchar(2000)
declare @fromClause nvarchar(500)
declare @whereClause nvarchar(1000)
declare @groupbyClause nvarchar(500)
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
			UTCCollectionDateTime datetime,
			DataCount int, 		
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

	set @selectClause = 'Select * FROM ( SELECT 
		DateAdd(Millisecond, -DatePart(MilliSecond, date), DateAdd(Second, -DatePart(Second, date), DateAdd(Minute, -DatePart(Minute, date),date)))  UTCCollectionDateTime,  
		SUM (count)  DataCount'
	set @fromClause = ' FROM [' + @eventDatabaseName + '].dbo.Stats'
	set @whereClause = ' WHERE date >= CONVERT(DATETIME, ''' + @startDateStr + ''') and date <= CONVERT(DATETIME, ''' + @endDateStr + ''') AND category = '''+ @serverActivityCategory +''' ';
	set @groupbyClause = ' GROUP BY (DateAdd(Millisecond, -DatePart(MilliSecond, date), DateAdd(Second, -DatePart(Second, date), DateAdd(Minute, -DatePart(Minute, date),date))))) temp';
    set @whereDate = ' WHERE CONVERT (Date, [temp].UTCCollectionDateTime) <= CONVERT(Date, '''+@endDateStr+''') and Convert (Date, [temp].UTCCollectionDateTime) >= CONVERT(Date, '''+@startDateStr+''') ';
	if(@localEndTime > @localStartTime)
	set @whereTime = 'AND (CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].UTCCollectionDateTime)) <= CONVERT(Time, '''+@localEndTime+''') AND CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].UTCCollectionDateTime)) >= CONVERT(Time, '''+@localStartTime+''')) ORDER BY UTCCollectionDateTime';
		else
	set @whereTime =
	  'AND 
	  (
	    (
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].UTCCollectionDateTime)) <= CONVERT(Time, '''+@localEndTime+''') 
			AND 
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].UTCCollectionDateTime)) >= CONVERT(Time, ''1900-01-01 00:00:00.0000000'')
	    )
		OR
		(
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].UTCCollectionDateTime)) <= CONVERT(Time, ''1900-01-01 23:59:59.0000000'') 
			AND 
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].UTCCollectionDateTime)) >= CONVERT(Time, '''+@localStartTime+''')
	    )
	  )'

	set @stmt = @selectClause + @fromClause + @whereClause + @groupbyClause + @whereDate + @whereTime;
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

set @selectClause = 'Select * FROM ( SELECT 
    DateAdd(Millisecond, -DatePart(MilliSecond, date), DateAdd(Second, -DatePart(Second, date), DateAdd(Minute, -DatePart(Minute, date),date)))  UTCCollectionDateTime,  
    SUM (count)  DataCount'
set @fromClause = ' FROM [' + @eventDatabase + '].dbo.Stats'
set @whereClause = ' WHERE date >= CONVERT(DATETIME, ''' + @startDateStr + ''') and date <= CONVERT(DATETIME, ''' + @endDateStr + ''') AND category = '''+ @serverActivityCategory +''' ';
	set @groupbyClause = ' GROUP BY (DateAdd(Millisecond, -DatePart(MilliSecond, date), DateAdd(Second, -DatePart(Second, date), DateAdd(Minute, -DatePart(Minute, date),date))))) temp';
    set @whereDate = ' WHERE CONVERT (Date, [temp].UTCCollectionDateTime) <= CONVERT(Date, '''+@endDateStr+''') and Convert (Date, [temp].UTCCollectionDateTime) >= CONVERT(Date, '''+@startDateStr+''') ';
	if(@localEndTime > @localStartTime)
	set @whereTime = 'AND (CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].UTCCollectionDateTime)) <= CONVERT(Time, '''+@localEndTime+''') AND CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].UTCCollectionDateTime)) >= CONVERT(Time, '''+@localStartTime+''')) ORDER BY UTCCollectionDateTime';
		else
	set @whereTime =
	  'AND 
	  (
	    (
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].UTCCollectionDateTime)) <= CONVERT(Time, '''+@localEndTime+''') 
			AND 
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].UTCCollectionDateTime)) >= CONVERT(Time, ''1900-01-01 00:00:00.0000000'')
	    )
		OR
		(
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].UTCCollectionDateTime)) <= CONVERT(Time, ''1900-01-01 23:59:59.0000000'') 
			AND 
			CONVERT (Time, DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [temp].UTCCollectionDateTime)) >= CONVERT(Time, '''+@localStartTime+''')
	    )
	  )'

set @stmt = @selectClause + @fromClause + @whereClause + @groupbyClause + @whereDate + @whereTime;

EXEC(@stmt)
end
GO

