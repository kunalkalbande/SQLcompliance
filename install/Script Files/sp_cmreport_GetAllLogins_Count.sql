/****** Object:  StoredProcedure [dbo].[sp_cmreport_GetAllLogins_Count]    Script Date: 07-07-2021 12:23:45 ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO




CREATE procedure [dbo].[sp_cmreport_GetAllLogins_Count] (
 @eventDatabaseName NVARCHAR(256),
 @Count int output
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
		SET @query = 'SELECT DISTINCT name as loginName FROM ['+@eventDatabaseName+'].[dbo].Logins';
		insert into #loginInfo exec (@query) 
		
end
if (object_id(N'tempdb..#loginInfo1') IS NOT NULL)
			drop table #loginInfo1			
		create table #loginInfo1 (
			loginName nvarchar(MAX)) 
			insert into #loginInfo1 
select distinct loginName COLLATE sql_latin1_general_cp1_ci_as as loginName from #loginInfo where loginName IS NOT NULL and loginName <> ''
select  @Count=count(loginName) from #loginInfo1  
END

GO
