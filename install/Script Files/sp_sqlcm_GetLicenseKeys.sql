USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetLicenseKeys]    Script Date: 4/26/2016 11:48:06 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('sp_sqlcm_GetLicenseKeys') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetLicenseKeys
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_GetLicenseKeys](
	@LicenseID int,
	@ReturnServerCount int output,
	@ReturnInstanceName nvarchar(128) output
)
AS
BEGIN
	DECLARE @e INT
	DECLARE @rmsc INT
	DECLARE @instanceName nvarchar(128)
	declare @listener nvarchar(1024)
	
	-- set return values to current number of registered servers and SQL instance
	SELECT @rmsc = COUNT([srvId]) FROM Servers (NOLOCK) WHERE isAuditedServer = 1  
	select @instanceName = CONVERT(nvarchar(128), serverproperty('servername'))
		
	IF (@LicenseID IS NULL) 
	BEGIN
		SELECT [licenseid],[licensekey],[createdtm] FROM [Licenses] (NOLOCK)
		ORDER BY [createdtm]
	END
	ELSE
	BEGIN
		SELECT [licenseid],[licensekey],[createdtm] FROM [Licenses] (NOLOCK)
				WHERE ([licenseid] = @LicenseID)
		ORDER BY [createdtm]
	END

	SELECT @e = @@error
	IF (@e = 0)
	BEGIN
		SELECT @ReturnServerCount = @rmsc
		--select @ReturnInstanceName = @listener
		-- irst use the listener
		if @ReturnInstanceName is null
			SELECT @ReturnInstanceName = @instanceName
	END

	RETURN @e
END
 


GO

