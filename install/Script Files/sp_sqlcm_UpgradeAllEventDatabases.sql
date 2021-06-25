USE [SQLcompliance]
GO
IF (OBJECT_ID('sp_sqlcm_UpgradeAllEventDatabases') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_UpgradeAllEventDatabases
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_UpgradeAllEventDatabases]
as
BEGIN
	-- Create a cursor to iterate through the tables
	DECLARE @dbname varchar(255)
	
	DECLARE db_name INSENSITIVE CURSOR FOR
	SELECT databaseName FROM [SQLcompliance]..SystemDatabases WHERE databaseType='Archive' OR databaseType='Event'
	FOR READ ONLY
	
	SET NOCOUNT ON
	OPEN db_name 
	FETCH db_name INTO @dbname 
	WHILE @@fetch_status = 0 
	BEGIN
		EXEC sp_sqlcm_UpgradeAllEventDatabase @dbname
		FETCH db_name INTO @dbname 
	END
	CLOSE db_name 
	DEALLOCATE db_name
END