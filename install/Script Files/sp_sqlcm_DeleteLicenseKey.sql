USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_DeleteLicenseKey]    Script Date: 4/26/2016 11:47:42 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('sp_sqlcm_DeleteLicenseKey') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_DeleteLicenseKey
END
GO
CREATE PROCEDURE [dbo].[sp_sqlcm_DeleteLicenseKey](
	@LicenseKey nvarchar(255)
)
AS
BEGIN
	DECLARE @e INT

	if (@LicenseKey IS NULL)
	BEGIN
		-- delete em all
		DELETE FROM [Licenses]
	END
	ELSE
	BEGIN
		-- delete a specific key
		DELETE FROM [Licenses] WHERE ([licensekey] = @LicenseKey)
	END


	SELECT @e = @@error
	RETURN @e
END
GO

