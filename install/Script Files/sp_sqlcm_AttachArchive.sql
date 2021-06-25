IF (OBJECT_ID('sp_sqlcm_AttachArchive') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_AttachArchive
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_AttachArchive]
@Archive nvarchar(128)
AS BEGIN
	BEGIN TRANSACTION;
	BEGIN TRY
		DECLARE @SqlDatabaseId INT;
		SELECT @SqlDatabaseId = [dbid] FROM [master]..[sysdatabases] WHERE [name]=@Archive;

		IF @SqlDatabaseId IS NULL BEGIN
			RAISERROR (N'Database: %s not found.', 10, 1, @Archive); 
		END ELSE
		BEGIN
			CREATE TABLE #Temp_Description(
				[instance] [nvarchar](256) NULL,
				[displayName] [nvarchar](512) NULL,
				[description] [nvarchar](256) NULL,
				[databaseType] [nvarchar](16) NULL,
				[startDate] [datetime] NULL,
				[endDate] [datetime] NULL,
				[archiveBias] [int] NULL,
				[archiveStandardBias] [int] NULL,
				[archiveStandardDate] [datetime] NULL,
				[archiveDaylightBias] [int] NULL,
				[archiveDaylightDate] [datetime] NULL,
				[archiveTimeZoneName] [nvarchar](128) NULL,
				[sqlComplianceDbSchemaVersion] [int] NULL,
				[eventDbSchemaVersion] [int] NULL,
				[bias] [int] NULL,
				[standardBias] [int] NULL,
				[standardDate] [datetime] NULL,
				[daylightBias] [int] NULL,
				[daylightDate] [datetime] NULL,
				[defaultAccess] [int] NULL,
				[eventReviewNeeded] [tinyint] NULL,
				[containsBadEvents] [tinyint] NULL,
				[state] [int] NULL,
				[timeLastIntegrityCheck] [datetime] NULL,
				[lastIntegrityCheckResult] [int] NULL,
				[highWatermark] [int] NULL,
				[lowWatermark] [int] NULL);

			DECLARE @query NVARCHAR(MAX);
			SET @query = 'INSERT INTO #Temp_Description
           ([instance]
           ,[displayName]
           ,[description]
           ,[databaseType]
           ,[startDate]
           ,[endDate]
           ,[archiveBias]
           ,[archiveStandardBias]
           ,[archiveStandardDate]
           ,[archiveDaylightBias]
           ,[archiveDaylightDate]
           ,[archiveTimeZoneName]
           ,[sqlComplianceDbSchemaVersion]
           ,[eventDbSchemaVersion]
           ,[bias]
           ,[standardBias]
           ,[standardDate]
           ,[daylightBias]
           ,[daylightDate]
           ,[defaultAccess]
           ,[eventReviewNeeded]
           ,[containsBadEvents]
           ,[state]
           ,[timeLastIntegrityCheck]
           ,[lastIntegrityCheckResult]
           ,[highWatermark]
           ,[lowWatermark])
			SELECT TOP 1 instance, displayName,description,databaseType
           ,startDate,endDate,archiveBias, archiveStandardBias, archiveStandardDate
           ,archiveDaylightBias,archiveDaylightDate,archiveTimeZoneName
           ,sqlComplianceDbSchemaVersion,eventDbSchemaVersion,bias
           ,standardBias,standardDate,daylightBias,daylightDate
           ,defaultAccess,eventReviewNeeded,containsBadEvents,state
           ,timeLastIntegrityCheck,lastIntegrityCheckResult,highWatermark,lowWatermark FROM [' + @Archive + ']..[Description]';
		   
			EXEC sp_executesql @query;
			DECLARE @archiveInstance NVARCHAR(256);
			SET @archiveInstance = (SELECT TOP 1 instance FROM #Temp_Description);


			INSERT INTO [SQLcompliance]..[SystemDatabases] (sqlDatabaseId,databaseName,instance,displayName,[description],databaseType) 
				SELECT TOP 1 @SqlDatabaseId, @Archive, @archiveInstance, displayName, [description], 'Archive' FROM #Temp_Description;

			
			IF NOT EXISTS (SELECT TOP 1 srvId FROM [SQLcompliance]..[Servers] WITH (NOLOCK) WHERE instance = @archiveInstance)
			BEGIN 
				INSERT INTO [dbo].[Servers]
           ([instance]
           ,[description]
           ,[bias]
           ,[standardBias]
           ,[standardDate]
           ,[daylightBias]
           ,[daylightDate]
           ,[isAuditedServer])
SELECT @archiveInstance,N'Archive server - This instance is not audited by this installation of SQL compliance manager.'
,bias ,standardBias,standardDate,daylightBias,daylightDate,0 FROM #Temp_Description
			END
		END
	END TRY
BEGIN CATCH 
			IF @@TRANCOUNT > 0
				ROLLBACK TRANSACTION;
	END CATCH
	IF @@TRANCOUNT > 0
		COMMIT TRANSACTION;
END