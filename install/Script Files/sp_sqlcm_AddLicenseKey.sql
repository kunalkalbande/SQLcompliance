USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_AddLicenseKey]    Script Date: 4/26/2016 2:19:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (OBJECT_ID('sp_sqlcm_AddLicenseKey') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_AddLicenseKey
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_AddLicenseKey](
	@LicenseKey nvarchar(255),
	@ReturnLicenseID uniqueidentifier output
)
as

	declare @result int
	declare @License nvarchar(255)
	declare @err int
	declare @errmsg nvarchar(500)
	declare @ans int

	-- Get application program name
	declare @programname nvarchar(128)
	select @programname = program_name from master..sysprocesses where spid= @@spid and sid = SUSER_SID(SYSTEM_USER)

	declare @connectionname nvarchar(128)
	set @License=@LicenseKey
	set @connectionname = NULL
		
	if (@License IS NULL)
	begin
		set @errmsg = N'License key cannot be null.'
		RAISERROR (@errmsg, 16, 1)
		return -1
	end 
	
	declare @mxlen int

	if (LEN(@License) > 256)
	begin
		set @errmsg = N'License key cannot be longer than 256 characters.'
		RAISERROR (@errmsg, 16, 1)
		return -1
	end
	
	BEGIN TRAN
	
		insert into Licenses (licensekey, createdby, createdtm) values (@License, SYSTEM_USER, GETUTCDATE())

		select @err = @@error

		if @err <> 0
		begin
			set @errmsg = 'Error: Failed to insert a new license key to Licenses table with licence ' + @License
			RAISERROR (@errmsg, 16, 1)
			ROLLBACK TRAN
			return -1
		end

	COMMIT TRAN

	exec('select max(licenseid) from Licenses')
	

--as 
--begin
--	declare @e int
--	declare @id uniqueidentifier

--	select @id = NEWID()

--	INSERT INTO [Licenses]
--           ([LicenseKey]
--           ,[createdtm])
--     VALUES
--           (@LicenseKey,
--           GETUTCDATE())

--	select @e = @@error

--	IF (@e = 0)
--	begin
--		select @ReturnLicenseID = @id 
--	end

--	return @e
--end

GO

