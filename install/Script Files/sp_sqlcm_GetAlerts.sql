if ( object_id('sp_sqlcm_GetAlerts') is not null ) 
   begin
         drop procedure sp_sqlcm_GetAlerts
   end
GO

create procedure [dbo].[sp_sqlcm_GetAlerts]
       @AlertType int,
       @AlertLevel int,
       @PageSize int,
       @Page int,
       @InstanceId int = null
as 
       begin
---------------------------------------------- Cleaning up ----------------------------------------------------------------
	IF OBJECT_ID('tempdb..#TempServers') IS NOT NULL DROP TABLE #TempServers
	IF OBJECT_ID('tempdb..#OutputTable') IS NOT NULL DROP TABLE #OutputTable
---------------------------------------------- Declarations ---------------------------------------------------------------
	DECLARE @Iterator INT;
	SET @Iterator = 1;

	CREATE TABLE #TempServers (
	 RowID int IDENTITY(1, 1), 
	 srvId int,
	 instance nvarchar(256),
	 eventDatabase nvarchar(128),
	)

	CREATE TABLE #OutputTable (
	[serverId] int null,
	[alertId] int null,
	[alertType] int null,
	[instance] nvarchar(256) null,
	[alertTime] datetime null,
	[alertLevel] tinyint null,
	[alertEventId] int null,
	[alertRule] nvarchar(60) null,
	[alertEventTypeId] int null,
	[alertEventTypeName] nvarchar(64) null,
	[alertEventDetail] nvarchar(512) null
	)
---------------------------------------------- Getting servers -----------------------------------------------------------
	INSERT INTO #TempServers (srvId, instance, eventDatabase)
		SELECT srvId, instance, eventDatabase
			FROM [SQLcompliance].[dbo].[Servers]
                where
				@InstanceId is null or srvId = @InstanceId
                    
---------------------------------------------- Collecting events from  event databases -----------------------------------
	DECLARE @NumberOfRecords INT;
	SET @NumberOfRecords = (SELECT COUNT(*) FROM #TempServers)

	DECLARE @query nvarchar(max);
	SET @query = '';

		WHILE (@Iterator <= @NumberOfRecords)
		BEGIN
			DECLARE @EventDatabaseName nvarchar(128);
			DECLARE @ServerId int;
			DECLARE @CurrentInstance nvarchar(256);

			SELECT @EventDatabaseName = eventDatabase, @ServerId = srvId, @CurrentInstance = instance
			FROM #TempServers WHERE RowID = @Iterator
			IF (EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE ('[' + name + ']' = @EventDatabaseName OR name = @EventDatabaseName)))
			BEGIN 
				SET @query = 'INSERT INTO #OutputTable ([serverId], [alertId], [alertType], [instance], [alertTime], [alertLevel], [alertEventId], [alertRule], [alertEventTypeId], [alertEventTypeName], [alertEventDetail])
									SELECT @ServerId,
                            a.[alertId],
                            a.[alertType],
                            a.[instance],
                            a.[created],
                            a.[alertLevel],
									a.[alertEventId],
                            a.[ruleName],
									a.[eventType],
									et.[name],
									e.[details]
									FROM [SQLcompliance]..[Alerts] a
									INNER JOIN [SQLcompliance]..[Servers] s ON s.[instance] = a.[instance] 
									LEFT JOIN [' + @EventDatabaseName + ']..[Events] e ON e.eventId = a.alertEventId 
									LEFT JOIN [SQLcompliance]..[EventTypes] et ON et.[evtypeid] = a.[eventType]
									WHERE a.isDismissed = 0 OR a.isDismissed IS NULL AND a.instance = @CurrentInstance
									AND (a.[alertType] = @AlertType) 
									AND (a.[alertLevel] = @AlertLevel) ';

				EXEC sp_executesql @query, N'@ServerId int, @AlertType int, @AlertLevel int, @CurrentInstance nvarchar(256)',  
				@ServerId,  @AlertType, @AlertLevel, @CurrentInstance;

			END
		SET @Iterator = @Iterator + 1;
	end
---------------------------------------------- Calculating paginnation ---------------------------------------------------
	declare @from as int;
	declare @to as int;
	if(@Page>0) set @Page = @Page - 1;
	set @from = @Page * @PageSize;
	set @to = @from + @PageSize;

---------------------------------------------- Gettting result ------------------------------------------------------------
	;with alertsWithPagination([serverId], [alertId], [alertType], [instance], [alertTime], [alertLevel], [alertEventId], [alertRule], [alertEventTypeId], [alertEventTypeName], [alertEventDetail], [row])
	as
	(
		select *, row_number() over ( order by [alertTime] desc ) as [row]
		from #OutputTable
                         )
	select [serverId], [alertId], [alertType], [instance], [alertTime], [alertLevel], [alertEventId], [alertRule], [alertEventTypeId], [alertEventTypeName], [alertEventDetail]
		from alertsWithPagination
                  where
		[row] > @from and [row] <= @to
---------------------------------------------- Cleaning up ----------------------------------------------------------------
	IF OBJECT_ID('tempdb..#TempServers') IS NOT NULL DROP TABLE #TempServers
	IF OBJECT_ID('tempdb..#OutputTable') IS NOT NULL DROP TABLE #OutputTable

       end
