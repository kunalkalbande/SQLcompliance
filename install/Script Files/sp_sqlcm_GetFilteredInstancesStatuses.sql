USE [SQLcompliance]
GO

IF (OBJECT_ID('sp_sqlcm_GetFilteredInstancesStatuses') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetFilteredInstancesStatuses
END
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetFilteredInstancesStatuses]    Script Date: 3/19/2016 1:20:03 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO



CREATE PROCEDURE [dbo].[sp_sqlcm_GetFilteredInstancesStatuses]
@InstanceName nvarchar(max),
@VersionName nvarchar(max),
@IsAudited bit,
@IsEnabled nvarchar(max),
@LastAgentContactFrom datetime,
@LastAgentContactTo datetime,
@AuditedDatabaseCountFrom int,
@AuditedDatabaseCountTo int,
@InstanceId int,
@EventsDateFrom datetime
AS
BEGIN
DECLARE @Iterator INT;
SET @Iterator = 1;

CREATE TABLE #TempServers (
 RowID int IDENTITY(1, 1), 
 srvId int,
 instance nvarchar(256),
 eventDatabase nvarchar(128),
);

INSERT INTO #TempServers (srvId, instance, eventDatabase)
	SELECT srvId, instance, eventDatabase
		FROM [SQLcompliance].[dbo].[Servers] WITH (NOLOCK) WHERE ((@InstanceId IS NULL OR @InstanceId = srvId));
		
DECLARE @NumberOfRecords INT;
SET @NumberOfRecords = (SELECT COUNT(*) FROM #TempServers)

CREATE TABLE #OutputTable(
	[srvId] [int]  NOT NULL,
	[instance] [nvarchar](256) NOT NULL,
	[sqlVersion] [int] NULL,
	[sqlVersionName] [nvarchar](100) NULL,
	[description] [nvarchar](256) NULL,
	[bias] [int] NULL,
	[standardBias] [int] NULL,
	[standardDate] [datetime] NULL,
	[daylightBias] [int] NULL,
	[daylightDate] [datetime] NULL,
	[isAuditedServer] [tinyint] NULL,
	[isDeployed] [tinyint] NULL,
	[isDeployedManually] [tinyint] NULL,
	[isUpdateRequested] [tinyint] NULL,
	[isRunning] [tinyint] NULL,
	[isCrippled] [tinyint] NULL,
	[isEnabled] [tinyint] NULL,
	[isSqlSecureDb] [tinyint] NULL,
	[isOnRepositoryHost] [tinyint] NULL,
	[timeCreated] [datetime] NULL,
	[timeLastModified] [datetime] NULL,
	[timeEnabledModified] [datetime] NULL,
	[timeStartup] [datetime] NULL,
	[timeShutdown] [datetime] NULL,
	[timeLastAgentContact] [datetime] NULL,
	[timeLastHeartbeat] [datetime] NULL,
	[timeLastCollection] [datetime] NULL,
	[timeLastArchive] [datetime] NULL,
	[lastArchiveResult] [int] NULL,
	[timeLastIntegrityCheck] [datetime] NULL,
	[lastIntegrityCheckResult] [int] NULL,
	[eventDatabase] [nvarchar](128) NULL,
	[defaultAccess] [int] NULL,
	[maxSqlLength] [int] NULL,
	[configVersion] [int] NULL,
	[lastKnownConfigVersion] [int] NULL,
	[lastConfigUpdate] [datetime] NULL,
	[configUpdateRequested] [tinyint] NULL,
	[auditLogins] [tinyint] NULL,
	[auditFailedLogins] [tinyint] NULL,
	[auditDDL] [tinyint] NULL,
	[auditSecurity] [tinyint] NULL,
	[auditAdmin] [tinyint] NULL,
	[auditDML] [tinyint] NULL,
	[auditSELECT] [tinyint] NULL,
	[auditTrace] [tinyint] NULL,
	[auditExceptions] [tinyint] NULL,
	[auditFailures] [tinyint] NULL,
	[auditCaptureSQL] [tinyint] NULL,
	[auditUsers] [nvarchar](512) NULL,
	[auditUsersList] [nvarchar](max) NULL,
	[auditUserAll] [tinyint] NULL,
	[auditUserLogins] [tinyint] NULL,
	[auditUserFailedLogins] [tinyint] NULL,
	[auditUserDDL] [tinyint] NULL,
	[auditUserSecurity] [tinyint] NULL,
	[auditUserAdmin] [tinyint] NULL,
	[auditUserDML] [tinyint] NULL,
	[auditUserSELECT] [tinyint] NULL,
	[auditUserFailures] [tinyint] NULL,
	[auditUserCaptureSQL] [tinyint] NULL,
	[auditUserExceptions] [tinyint] NULL,
	[agentServer] [nvarchar](256) NULL,
	[agentPort] [int] NULL,
	[agentVersion] [nvarchar](43) NULL,
	[agentServiceAccount] [nvarchar](256) NULL,
	[agentTraceDirectory] [nvarchar](256) NULL,
	[agentHeartbeatInterval] [int] NULL,
	[agentCollectionInterval] [int] NULL,
	[agentForceCollectionInterval] [int] NULL,
	[agentLogLevel] [int] NULL,
	[agentMaxTraceSize] [int] NULL,
	[agentMaxFolderSize] [int] NULL,
	[agentMaxUnattendedTime] [int] NULL,
	[agentTraceOptions] [int] NULL,
	[eventReviewNeeded] [tinyint] NULL,
	[containsBadEvents] [tinyint] NULL,
	[highWatermark] [int] NULL,
	[lowWatermark] [int] NULL,
	[alertHighWatermark] [int] NULL,
	[auditDBCC] [tinyint] NULL,
	[auditSystemEvents] [tinyint] NULL,
	[auditUDE] [tinyint] NULL,
	[auditUserUDE] [tinyint] NULL,
	[agentDetectionInterval] [int] NULL,
	[agentHealth] [bigint] NULL,
	[agentTraceStartTimeout] [int] NULL,
	[auditUserCaptureTrans] [tinyint] NULL,
	[auditedDbCount] [int] NULL,
	[collectedEventCount] [int] NULL,
	[lowAlerts] [int] NULL,
	[mediumAlerts] [int] NULL,
	[highAlerts] [int] NULL,
	[criticalAlerts] [int] NULL,
	[eventFilters] [nvarchar] (max) NULL,
	[recentAlertCount] [int] NULL
	);

WHILE (@Iterator <= @NumberOfRecords)
BEGIN
	DECLARE @EventDatabaseName nvarchar(128);
	DECLARE @ServerId int;
	DECLARE @CurrentInstance nvarchar(256);

	SELECT @EventDatabaseName = eventDatabase, @ServerId = srvId, @CurrentInstance = instance
	FROM #TempServers WHERE RowID = @Iterator
	
	DECLARE @processedEventCount int;
	DECLARE @RecentAlertCount int;

	DECLARE @query nvarchar(max);

	If exists (SELECT * FROM sys.databases where name=' [' +@EventDatabaseName +']')
	BEGIN
	SET @query = 'select 
					@EventCount = count(eventId) 
			      from 	[' +@EventDatabaseName +']..[Events] 
				  where eventType != 181 and eventType != 182';


	IF(@EventsDateFrom IS NOT NULL)
	BEGIN 
		SET @query = @query + ' and (startTime >= ''' +  CONVERT(nvarchar, @EventsDateFrom) + ''')';
	END				  
	print @query
	EXECUTE sp_executesql @query, N'@EventCount int OUTPUT', @EventCount = @processedEventCount OUT
	END
	If exists (SELECT * FROM sys.databases where name=' [' +@EventDatabaseName +']')
	BEGIN
	SET @query = 'SELECT @AlertCount = sum(count) FROM [' +@EventDatabaseName +']..Stats WHERE date >= @DateFrom and category = 4';
	EXECUTE sp_executesql @query ,N'@DateFrom datetime, @AlertCount int OUTPUT', @DateFrom = @EventsDateFrom, @AlertCount = @RecentAlertCount OUT
	END
	DECLARE @eventFiltersForInstance  nvarchar(max)
	SET @eventFiltersForInstance = NULL
	SELECT @eventFiltersForInstance = Coalesce(@eventFiltersForInstance + ', ', '') + name FROM [SQLcompliance]..[EventFilters] WHERE [targetInstances] LIKE '%'+ @CurrentInstance + '%' AND [enabled] = 1;

	
INSERT INTO #OutputTable
           ([srvId]
		   ,[instance]
           ,[sqlVersion]
		   ,[sqlVersionName]
           ,[description]
           ,[bias]
           ,[standardBias]
           ,[standardDate]
           ,[daylightBias]
           ,[daylightDate]
           ,[isAuditedServer]
           ,[isDeployed]
           ,[isDeployedManually]
           ,[isUpdateRequested]
           ,[isRunning]
           ,[isCrippled]
           ,[isEnabled]
           ,[isSqlSecureDb]
           ,[isOnRepositoryHost]
           ,[timeCreated]
           ,[timeLastModified]
           ,[timeEnabledModified]
           ,[timeStartup]
           ,[timeShutdown]
           ,[timeLastAgentContact]
           ,[timeLastHeartbeat]
           ,[timeLastCollection]
           ,[timeLastArchive]
           ,[lastArchiveResult]
           ,[timeLastIntegrityCheck]
           ,[lastIntegrityCheckResult]
           ,[eventDatabase]
           ,[defaultAccess]
           ,[maxSqlLength]
           ,[configVersion]
           ,[lastKnownConfigVersion]
           ,[lastConfigUpdate]
           ,[configUpdateRequested]
           ,[auditLogins]
           ,[auditFailedLogins]
           ,[auditDDL]
           ,[auditSecurity]
           ,[auditAdmin]
           ,[auditDML]
           ,[auditSELECT]
           ,[auditTrace]
           ,[auditExceptions]
           ,[auditFailures]
           ,[auditCaptureSQL]
           ,[auditUsers]
           ,[auditUsersList]
           ,[auditUserAll]
           ,[auditUserLogins]
           ,[auditUserFailedLogins]
           ,[auditUserDDL]
           ,[auditUserSecurity]
           ,[auditUserAdmin]
           ,[auditUserDML]
           ,[auditUserSELECT]
           ,[auditUserFailures]
           ,[auditUserCaptureSQL]
           ,[auditUserExceptions]
           ,[agentServer]
           ,[agentPort]
           ,[agentVersion]
           ,[agentServiceAccount]
           ,[agentTraceDirectory]
           ,[agentHeartbeatInterval]
           ,[agentCollectionInterval]
           ,[agentForceCollectionInterval]
           ,[agentLogLevel]
           ,[agentMaxTraceSize]
           ,[agentMaxFolderSize]
           ,[agentMaxUnattendedTime]
           ,[agentTraceOptions]
           ,[eventReviewNeeded]
           ,[containsBadEvents]
           ,[highWatermark]
           ,[lowWatermark]
           ,[alertHighWatermark]
           ,[auditDBCC]
           ,[auditSystemEvents]
           ,[auditUDE]
           ,[auditUserUDE]
           ,[agentDetectionInterval]
           ,[agentHealth]
           ,[agentTraceStartTimeout]
           ,[auditUserCaptureTrans]
		   ,[auditedDbCount]
		   ,[collectedEventCount]
		   ,[lowAlerts]
		   ,[mediumAlerts]
		   ,[highAlerts]
	       ,[criticalAlerts]
		   ,[eventFilters]
		   ,[recentAlertCount]
		   )
SELECT [srvId]
	 ,[instance]
	 ,[sqlVersion]
	  ,(CASE 
			WHEN [versionName] IS NOT NULL AND [versionName] <> '' THEN [versionName]
			WHEN [sqlVersion] = 8  THEN N'Microsoft SQL Server 2000'
			WHEN [sqlVersion] = 9  THEN N'Microsoft SQL Server 2005'
			WHEN [sqlVersion] = 10 THEN N'Microsoft SQL Server 2008'
			WHEN [sqlVersion] = 11 THEN N'Microsoft SQL Server 2012'
			WHEN [sqlVersion] = 12 THEN N'Microsoft SQL Server 2014'
			WHEN [sqlVersion] = 13 THEN N'Microsoft SQL Server 2016'
			WHEN [sqlVersion] = 14 THEN N'Microsoft SQL Server 2017'
	     END) AS [versionName]
	 ,[description]
	  ,[bias]
	  ,[standardBias]
	  ,[standardDate]
	  ,[daylightBias]
	  ,[daylightDate]
	  ,[isAuditedServer]
      ,[isDeployed]
      ,[isDeployedManually]
      ,[isUpdateRequested]
      ,[isRunning]
      ,[isCrippled]
      ,[isEnabled]
      ,[isSqlSecureDb]
      ,[isOnRepositoryHost]
      ,[timeCreated]
      ,[timeLastModified]
      ,[timeEnabledModified]
      ,[timeStartup]
      ,[timeShutdown]
      ,[timeLastAgentContact]
      ,[timeLastHeartbeat]
      ,[timeLastCollection]
      ,[timeLastArchive]
      ,[lastArchiveResult]
      ,[timeLastIntegrityCheck]
      ,[lastIntegrityCheckResult]
      ,[eventDatabase]
      ,[defaultAccess]
      ,[maxSqlLength]
      ,[configVersion]
      ,[lastKnownConfigVersion]
      ,[lastConfigUpdate]
      ,[configUpdateRequested]
      ,[auditLogins]
      ,[auditFailedLogins]
      ,[auditDDL]
      ,[auditSecurity]
      ,[auditAdmin]
      ,[auditDML]
      ,[auditSELECT]
      ,[auditTrace]
      ,[auditExceptions]
      ,[auditFailures]
      ,[auditCaptureSQL]
      ,[auditUsers]
      ,[auditUsersList]
      ,[auditUserAll]
      ,[auditUserLogins]
      ,[auditUserFailedLogins]
      ,[auditUserDDL]
      ,[auditUserSecurity]
      ,[auditUserAdmin]
      ,[auditUserDML]
      ,[auditUserSELECT]
      ,[auditUserFailures]
      ,[auditUserCaptureSQL]
      ,[auditUserExceptions]
      ,[agentServer]
      ,[agentPort]
      ,[agentVersion]
      ,[agentServiceAccount]
      ,[agentTraceDirectory]
      ,[agentHeartbeatInterval]
      ,[agentCollectionInterval]
      ,[agentForceCollectionInterval]
      ,[agentLogLevel]
      ,[agentMaxTraceSize]
      ,[agentMaxFolderSize]
      ,[agentMaxUnattendedTime]
      ,[agentTraceOptions]
      ,[eventReviewNeeded]
      ,[containsBadEvents]
      ,[highWatermark]
      ,[lowWatermark]
      ,[alertHighWatermark]
      ,[auditDBCC]
      ,[auditSystemEvents]
      ,[auditUDE]
      ,[auditUserUDE]
      ,[agentDetectionInterval]
      ,[agentHealth]
      ,[agentTraceStartTimeout]
      ,[auditUserCaptureTrans]
	  ,(SELECT COUNT(*) FROM [SQLcompliance]..[Databases] d WHERE d.[srvId] = s.[srvId] AND d.[isEnabled] = 1)
	  ,@processedEventCount
	  ,(SELECT COUNT(*) FROM [SQLcompliance]..[Alerts] WHERE instance = @CurrentInstance AND alertLevel = 1 AND (isDismissed = 0 OR isDismissed IS NULL) AND (@EventsDateFrom IS NULL OR created >= @EventsDateFrom))
	  ,(SELECT COUNT(*) FROM [SQLcompliance]..[Alerts] WHERE instance = @CurrentInstance AND alertLevel = 2 AND (isDismissed = 0 OR isDismissed IS NULL) AND (@EventsDateFrom IS NULL OR created >= @EventsDateFrom))
	  ,(SELECT COUNT(*) FROM [SQLcompliance]..[Alerts] WHERE instance = @CurrentInstance AND alertLevel = 3 AND (isDismissed = 0 OR isDismissed IS NULL) AND (@EventsDateFrom IS NULL OR created >= @EventsDateFrom))
	  ,(SELECT COUNT(*) FROM [SQLcompliance]..[Alerts] WHERE instance = @CurrentInstance AND alertLevel = 4 AND (isDismissed = 0 OR isDismissed IS NULL) AND (@EventsDateFrom IS NULL OR created >= @EventsDateFrom))
	  ,@eventFiltersForInstance
	  ,@RecentAlertCount
	  FROM [SQLcompliance]..[Servers] s
	  WHERE s.[srvId] = @ServerId
	  SET @Iterator = @Iterator + 1;
END	


DECLARE @query11 nvarchar(max);
SET @query11 = '';
   BEGIN
		SET @query11 = 'Select 
* FROM #OutputTable where 1=1 ';

DECLARE @subWhereCondition NVARCHAR(MAX);
IF (@InstanceName IS NOT NULL)
BEGIN
	SET @query11 = @query11 + ' AND ([instance] LIKE ''' + REPLACE(@InstanceName,',',''' OR [instance] LIKE ''') + ''')'
END

IF(@VersionName IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND ([sqlVersionName] LIKE ''' + REPLACE(@VersionName,',',''' OR [sqlVersionName] LIKE ''') + ''')'
	SET @query11 = @query11 + @subWhereCondition;
END

IF(@IsAudited IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND ([isAuditedServer] = @IsAudited)'
	SET @query11 = @query11 + @subWhereCondition;
END

IF(@IsEnabled IS NOT NULL)
BEGIN 
	SET @subWhereCondition = 'AND ([isEnabled] IN ( select data from dbo.fn_SplitBitAsTable('''+@IsEnabled+''', '','') ))'
	SET @query11 = @query11 + @subWhereCondition;
END

IF(@LastAgentContactFrom IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND ([timeLastAgentContact] >cast('''+CONVERT(varchar(23), @LastAgentContactFrom, 121) +''' as datetime))'
	SET @query11 = @query11 + @subWhereCondition;
END

IF(@LastAgentContactTo IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND ([timeLastAgentContact] <cast('''+CONVERT(varchar(23), @LastAgentContactTo, 121) +''' as datetime))'
	SET @query11 = @query11 + @subWhereCondition;
END

IF(@AuditedDatabaseCountFrom IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND ([auditedDbCount] >='+ cast(@AuditedDatabaseCountFrom as varchar(5))+')'
	SET @query11 = @query11 + @subWhereCondition;
END

IF(@AuditedDatabaseCountTo IS NOT NULL)
BEGIN 
	SET @subWhereCondition = ' AND ([auditedDbCount] <='+ cast( @AuditedDatabaseCountTo as varchar(5))+')'
	SET @query11 = @query11 + @subWhereCondition;
END

EXEC sp_executesql @query11
END
END


GO

