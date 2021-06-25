-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetAllLogins' and xtype='P')
drop procedure [sp_cmreport_GetAllLogins]
GO


CREATE procedure [dbo].[sp_cmreport_GetAllLogins] (
 @eventDatabaseName NVARCHAR(256),
   @requestedFrom nvarchar(16) = NULL
   )
AS
BEGIN
	DECLARE @query NVARCHAR(MAX);
	if (object_id(N'tempdb..#loginInfo') IS NOT NULL)
			drop table #loginInfo			
		create table #loginInfo (
			loginName nvarchar(MAX)) 
	if (@eventDatabaseName = '<ALL>' OR @eventDatabaseName = '<All>')
	begin
		declare @eventDatabase nvarchar(128),
			@description nvarchar(512),
			@length int,
			@tempColumns nvarchar(2000)

		DECLARE eventDatabases CURSOR FOR 
		SELECT DISTINCT databaseName, instance + CASE WHEN UPPER(databaseType) LIKE 'EVENT' THEN '' ELSE ' - ' + displayName END AS description
		  FROM SQLcompliance..SystemDatabases
		  WHERE(UPPER(databaseType) IN ('ARCHIVE', 'EVENT'))
		  ORDER BY description;
		OPEN eventDatabases;
		FETCH eventDatabases INTO @eventDatabase, @description
	
		while @@Fetch_Status = 0
		begin
			set @eventDatabase = dbo.fn_cmreport_ProcessString(@eventDatabase);
			set @query = 'SELECT DISTINCT name as loginName FROM ['+@eventDatabase+'].[dbo].Logins'
		
			insert into #loginInfo exec (@query) 
	
		fetch eventDatabases into @eventDatabase, @description
		end	
		close eventDatabases  
		deallocate eventDatabases
	end
	else
	begin
		if(@requestedFrom = 'Windows')
		begin
			SET @query = 'SELECT ''<ALL>'' as loginName UNION SELECT DISTINCT name as loginName FROM ['+@eventDatabaseName+'].[dbo].Logins';
			insert into #loginInfo exec (@query) 
	end
	else
	begin
		SET @query = 'SELECT DISTINCT name as loginName FROM ['+@eventDatabaseName+'].[dbo].Logins';
		insert into #loginInfo exec (@query) 
	end
END
if(@requestedFrom = 'Windows')
	begin
		select '<ALL>' as loginName UNION select distinct loginName COLLATE sql_latin1_general_cp1_ci_as as loginName from #loginInfo where loginName IS NOT NULL and loginName <> ''
	end
else
	begin
		select distinct loginName COLLATE sql_latin1_general_cp1_ci_as as loginName from #loginInfo where loginName IS NOT NULL and loginName <> ''
end
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

