USE [SQLcompliance]
GO
/****** Object:  StoredProcedure [dbo].[sp_sqlcm_ArchiveParentsProc]    Script Date: 9/21/2016 2:26:10 PM ******/
/****** These stored procedures are not called from anywhere in the code, but these are standalone procedures to help with archiving. ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_ArchiveParentsProc') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_ArchiveParentsProc
END
GO

Create procedure [dbo].[sp_sqlcm_ArchiveParentsProc] (
	@InstanceDatabase varchar(255),
	@days as int,
	@type varchar(255),
	@archive_year as varchar(max),
	@new_path varchar(max)
	)
	
as
declare @dbName as varchar(200)

declare @sql nvarchar(max)
declare @serverName nvarchar(max)
declare @sql1 nvarchar(max)

begin
	begin
	EXEC 	[dbo].[sp_sqlcm_ArchiveEventSQL]
			@InstanceDatabase = @InstanceDatabase,
			@days = @days,
			@type=@type,
			@archive_year =@archive_year,
			@new_path= @new_path

	end

	begin
	EXEC 	[dbo].[sp_sqlcm_ArchiveStats]
			@InstanceDatabase = @InstanceDatabase,
			@days = @days,
			@type=@type,
			@archive_year =@archive_year,
			@new_path= @new_path

	end

	begin
	EXEC 	[dbo].[sp_sqlcm_ArchiveDataChanges]
			@InstanceDatabase = @InstanceDatabase,
			@days = @days,
			@type=@type,
			@archive_year =@archive_year,
			@new_path= @new_path

	end

	begin
	EXEC 	[dbo].[sp_sqlcm_ArchiveChangeLog]
			@InstanceDatabase = @InstanceDatabase,
			@days = @days,
			@type=@type,
			@archive_year =@archive_year,
			@new_path= @new_path

	end

	begin
	EXEC 	[dbo].[sp_sqlcm_ArchiveEvents]
			@InstanceDatabase = @InstanceDatabase,
			@days = @days,
			@type=@type,
			@archive_year =@archive_year,
			@new_path= @new_path

	end

	begin
	EXEC 	[dbo].[sp_sqlcm_ArchiveSensitiveColumns]
			@InstanceDatabase = @InstanceDatabase,
			@days = @days,
			@type=@type,
			@archive_year =@archive_year,
			@new_path= @new_path

	end

	begin
	EXEC 	[dbo].[sp_sqlcm_ArchiveColumnChanges]
			@InstanceDatabase = @InstanceDatabase,
			@days = @days,
			@type=@type,
			@archive_year =@archive_year,
			@new_path= @new_path

	end

	begin
	EXEC 	[dbo].[sp_sqlcm_ArchiveDescriptions]
			@InstanceDatabase = @InstanceDatabase,
			@days = @days,
			@type=@type,
			@archive_year =@archive_year,
			@new_path= @new_path

	end

	begin
	EXEC 	[dbo].[sp_sqlcm_ArchiveColumns]
			@InstanceDatabase = @InstanceDatabase,
			@days = @days,
			@type=@type,
			@archive_year =@archive_year,
			@new_path= @new_path

	end
	
	begin
	EXEC 	[dbo].[sp_sqlcm_ArchiveLogins]
			@InstanceDatabase = @InstanceDatabase,
			@days = @days,
			@type=@type,
			@archive_year =@archive_year,
			@new_path= @new_path

	end

	begin
	EXEC 	[dbo].[sp_sqlcm_ArchiveTables]
			@InstanceDatabase = @InstanceDatabase,
			@days = @days,
			@type=@type,
			@archive_year =@archive_year,
			@new_path= @new_path

	end

	begin
	EXEC 	[dbo].[sp_sqlcm_ArchiveApplications]
			@InstanceDatabase = @InstanceDatabase,
			@days = @days,
			@type=@type,
			@archive_year =@archive_year,
			@new_path= @new_path

	end

	begin
	EXEC 	[dbo].[sp_sqlcm_ArchiveHosts]
			@InstanceDatabase = @InstanceDatabase,
			@days = @days,
			@type=@type,
			@archive_year =@archive_year,
			@new_path= @new_path

	end

end
