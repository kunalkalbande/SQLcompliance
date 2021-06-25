IF (OBJECT_ID('sp_sqlcm_DetachArchive') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_DetachArchive
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_DetachArchive]
@Instance nvarchar(256),
@DatabaseName nvarchar(128)
AS BEGIN
	BEGIN TRANSACTION;
	
	BEGIN TRY
		
		DELETE FROM [SQLcompliance]..[SystemDatabases] WHERE databaseName = @DatabaseName and databaseType='Archive'
		DECLARE @LoadedArchivesCount int;
		DECLARE @IsServerAudited tinyint;
		
		SET @IsServerAudited = (SELECT isAuditedServer FROM [SQLcompliance]..[Servers] WHERE instance=@Instance);
		SET @LoadedArchivesCount = (SELECT count(*) FROM [SQLcompliance]..[SystemDatabases] WHERE instance=@Instance AND databaseType='Archive')
		IF @LoadedArchivesCount = 0 AND @IsServerAudited = 0
		BEGIN
			DELETE FROM [SQLcompliance]..[Servers] WHERE instance=@Instance;
		END
	
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;
	END CATCH
	IF @@TRANCOUNT > 0
		COMMIT TRANSACTION;
END