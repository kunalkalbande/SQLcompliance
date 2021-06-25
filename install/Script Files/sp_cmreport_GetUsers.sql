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
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetUsers' and xtype='P')
drop procedure [sp_cmreport_GetUsers]
GO


CREATE procedure sp_cmreport_GetUsers (
	@eventDatabase nvarchar(256),
	@databaseName nvarchar(256),
	@loginNameForUser nvarchar(MAX), 
	@roleNameForUser nvarchar(MAX),
	@userType nvarchar(20),
	@generatedFrom nvarchar(10) = NULL
   )
AS
BEGIN
DECLARE @serverId int;
DECLARE @stmt nvarchar(MAX);
DECLARE @filterForUserType nvarchar(64), @filterForLoginName nvarchar(MAX), @filterForRoleName nvarchar(MAX);

set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));
set @filterForUserType = '';

if(@userType = 'Privileged')
begin
	set @filterForUserType = ' AND usr.isPrivileged = 1 ';
end
if(@userType = 'Trusted')
begin
	set @filterForUserType = ' AND usr.isTrusted = 1 ';
end

if(@loginNameForUser = '<ALL>')
 begin
	SET @filterForLoginName = ' AND usr.loginName LIKE ''%''';
 end
else
 begin
	SET @loginNameForUser = dbo.fn_cmreport_CreateMultiString(@loginNameForUser);
	SET @filterForLoginName = ' AND usr.loginName IN (' + @loginNameForUser + ') ';
 end

 
if(@roleNameForUser = '<ALL>')
 begin
	SET @filterForRoleName = ' AND usr.roleName LIKE ''%''';
 end
else
 begin
	SET @roleNameForUser = dbo.fn_cmreport_CreateMultiString(@roleNameForUser);
	SET @filterForRoleName = ' AND usr.roleName IN (' + @roleNameForUser + ') ';
 end

if(@eventDatabase = "<ALL>")
begin

	if (object_id(N'tempdb..#users') IS NOT NULL)
		drop table #users
	create table #users (
		ServerName nvarchar(256),
		DatabaseName nvarchar(128),
		IsPrivileged bit,
		Name nvarchar(256),
		IsServerLevel bit
		) 

	DECLARE serverCursor CURSOR FOR 
	SELECT srvId FROM Servers
	OPEN serverCursor;
	FETCH serverCursor INTO @serverId
	while @@Fetch_Status = 0
	begin

		SET @stmt = 'SELECT srv.instance AS ServerName, db.name AS DatabaseName, usr.isPrivileged AS IsPrivileged, usr.roleName AS Name, usr.isServerLevel AS IsServerLevel 
			FROM Users usr JOIN Servers srv on srv.srvId = usr.serverId 
			JOIN Databases db on srv.srvId = db.srvId
			WHERE srv.srvId = ' + STR(@serverId) + ' AND db.name LIKE ''' + @databaseName + ''' AND usr.isTrusted = 1 AND usr.roleName IS NOT NULL ' + @filterForUserType + @filterForRoleName +'
			GROUP BY instance, name, isPrivileged, roleName, isServerLevel

			UNION

			SELECT srv.instance AS ServerName, db.name AS DatabaseName, usr.isPrivileged AS IsPrivileged, usr.roleName AS Name, usr.isServerLevel AS IsServerLevel
			FROM Users usr JOIN Databases db on db.dbId = usr.databaseId 
			JOIN Servers srv on srv.srvId = db.srvId
			WHERE srv.srvId = ' + STR(@serverId) + ' AND db.name LIKE ''' + @databaseName + ''' AND usr.isTrusted = 1 AND usr.roleName IS NOT NULL ' + @filterForUserType + @filterForRoleName +' AND usr.roleName NOT IN (SELECT roleName FROM Users WHERE isServerLevel = 1 AND isTrusted = 1 AND serverId = srv.srvId AND roleName IS NOT NULL)
			GROUP BY instance, name, isPrivileged, roleName, isServerLevel

			UNION

			SELECT srv.instance AS ServerName, db.name AS DatabaseName, usr.isPrivileged AS IsPrivileged, usr.loginName AS LoginName , usr.isServerLevel AS IsServerLevel
			FROM Users usr JOIN Servers srv on srv.srvId = usr.serverId 
			JOIN Databases db on srv.srvId = db.srvId
			WHERE srv.srvId = ' + STR(@serverId) + ' AND db.name LIKE ''' + @databaseName + ''' AND usr.isTrusted = 1 AND usr.loginName IS NOT NULL ' + @filterForUserType + @filterForLoginName +'
			GROUP BY instance, name, isPrivileged, loginName, isServerLevel

			UNION

			SELECT srv.instance AS ServerName, db.name AS DatabaseName, usr.isPrivileged AS IsPrivileged, usr.loginName AS LoginName , usr.isServerLevel AS IsServerLevel
			FROM Users usr JOIN Databases db on db.dbId = usr.databaseId 
			JOIN Servers srv on srv.srvId = db.srvId
			WHERE srv.srvId = ' + STR(@serverId) + ' AND db.name LIKE ''' + @databaseName + ''' AND usr.isTrusted = 1 AND usr.loginName IS NOT NULL ' + @filterForUserType + @filterForLoginName +' AND usr.loginName NOT IN (SELECT loginName FROM Users WHERE isServerLevel = 1 AND isTrusted = 1 AND serverId = srv.srvId AND loginName IS NOT NULL)
			GROUP BY instance, name, isPrivileged, loginName, isServerLevel

			UNION

			SELECT srv.instance AS ServerName, db.name AS DatabaseName, 1 AS IsPrivileged, usr.roleName AS Name, usr.isServerLevel AS IsServerLevel
			FROM Users usr JOIN Servers srv on srv.srvId = usr.serverId 
			JOIN Databases db on srv.srvId = db.srvId
			WHERE srv.srvId = ' + STR(@serverId) + ' AND db.name LIKE ''' + @databaseName + ''' AND usr.isPrivileged = 1 AND usr.roleName IS NOT NULL ' + @filterForUserType + @filterForRoleName +'
			GROUP BY instance, name, isPrivileged, roleName, isServerLevel

			UNION

			SELECT srv.instance AS ServerName, db.name AS DatabaseName, 1 AS IsPrivileged, usr.roleName AS Name, usr.isServerLevel AS IsServerLevel
			FROM Users usr JOIN Databases db on db.dbId = usr.databaseId 
			JOIN Servers srv on srv.srvId = db.srvId
			WHERE srv.srvId = ' + STR(@serverId) + ' AND db.name LIKE ''' + @databaseName + ''' AND usr.isPrivileged = 1 AND usr.roleName IS NOT NULL ' + @filterForUserType + @filterForRoleName +' AND usr.roleName NOT IN (SELECT roleName FROM Users WHERE isServerLevel = 1 AND isPrivileged = 1 AND serverId = srv.srvId AND roleName IS NOT NULL)
			GROUP BY instance, name, isPrivileged, roleName, isServerLevel

			UNION

			SELECT srv.instance AS ServerName, db.name AS DatabaseName, 1 AS IsPrivileged, usr.loginName AS LoginName , usr.isServerLevel AS IsServerLevel
			FROM Users usr JOIN Servers srv on srv.srvId = usr.serverId 
			JOIN Databases db on srv.srvId = db.srvId
			WHERE srv.srvId = ' + STR(@serverId) + ' AND db.name LIKE ''' + @databaseName + ''' AND usr.isPrivileged = 1 AND usr.loginName IS NOT NULL ' + @filterForUserType + @filterForLoginName +'
			GROUP BY instance, name, isPrivileged, loginName, isServerLevel

			UNION

			SELECT srv.instance AS ServerName, db.name AS DatabaseName, 1 AS IsPrivileged, usr.loginName AS LoginName , usr.isServerLevel AS IsServerLevel
			FROM Users usr JOIN Databases db on db.dbId = usr.databaseId 
			JOIN Servers srv on srv.srvId = db.srvId
			WHERE srv.srvId = ' + STR(@serverId) + ' AND db.name LIKE ''' + @databaseName + ''' AND usr.isPrivileged = 1 AND usr.loginName IS NOT NULL ' + @filterForUserType + @filterForLoginName +' AND usr.loginName NOT IN (SELECT loginName FROM Users WHERE isServerLevel = 1 AND isPrivileged = 1 AND serverId = srv.srvId AND loginName IS NOT NULL)
			GROUP BY instance, name, isPrivileged, loginName, isServerLevel';

		insert into #users exec (@stmt) 
		fetch serverCursor into @serverId
	end
	close serverCursor
	deallocate serverCursor
	select * from #users
end
else
begin
	SET @serverId = (SELECT srvId FROM Servers WHERE eventDatabase = @eventDatabase);

	SET @stmt = 'SELECT srv.instance AS ServerName, db.name AS DatabaseName, usr.isPrivileged AS IsPrivileged, usr.roleName AS Name, usr.isServerLevel AS IsServerLevel 
			FROM Users usr JOIN Servers srv on srv.srvId = usr.serverId 
			JOIN Databases db on srv.srvId = db.srvId
			WHERE srv.srvId = ' + STR(@serverId) + ' AND db.name LIKE ''' + @databaseName + ''' AND usr.isTrusted = 1 AND usr.roleName IS NOT NULL ' + @filterForUserType + @filterForRoleName +'
			GROUP BY instance, name, isPrivileged, roleName, isServerLevel

			UNION

			SELECT srv.instance AS ServerName, db.name AS DatabaseName, usr.isPrivileged AS IsPrivileged, usr.roleName AS Name, usr.isServerLevel AS IsServerLevel
			FROM Users usr JOIN Databases db on db.dbId = usr.databaseId 
			JOIN Servers srv on srv.srvId = db.srvId
			WHERE srv.srvId = ' + STR(@serverId) + ' AND db.name LIKE ''' + @databaseName + ''' AND usr.isTrusted = 1 AND usr.roleName IS NOT NULL ' + @filterForUserType + @filterForRoleName +' AND usr.roleName NOT IN (SELECT roleName FROM Users WHERE isServerLevel = 1 AND isTrusted = 1 AND serverId = srv.srvId AND roleName IS NOT NULL)
			GROUP BY instance, name, isPrivileged, roleName, isServerLevel

			UNION

			SELECT srv.instance AS ServerName, db.name AS DatabaseName, usr.isPrivileged AS IsPrivileged, usr.loginName AS LoginName , usr.isServerLevel AS IsServerLevel
			FROM Users usr JOIN Servers srv on srv.srvId = usr.serverId 
			JOIN Databases db on srv.srvId = db.srvId
			WHERE srv.srvId = ' + STR(@serverId) + ' AND db.name LIKE ''' + @databaseName + ''' AND usr.isTrusted = 1 AND usr.loginName IS NOT NULL ' + @filterForUserType + @filterForLoginName +'
			GROUP BY instance, name, isPrivileged, loginName, isServerLevel

			UNION

			SELECT srv.instance AS ServerName, db.name AS DatabaseName, usr.isPrivileged AS IsPrivileged, usr.loginName AS LoginName , usr.isServerLevel AS IsServerLevel
			FROM Users usr JOIN Databases db on db.dbId = usr.databaseId 
			JOIN Servers srv on srv.srvId = db.srvId
			WHERE srv.srvId = ' + STR(@serverId) + ' AND db.name LIKE ''' + @databaseName + ''' AND usr.isTrusted = 1 AND usr.loginName IS NOT NULL ' + @filterForUserType + @filterForLoginName +' AND usr.loginName NOT IN (SELECT loginName FROM Users WHERE isServerLevel = 1 AND isTrusted = 1 AND serverId = srv.srvId AND loginName IS NOT NULL)
			GROUP BY instance, name, isPrivileged, loginName, isServerLevel

			UNION

			SELECT srv.instance AS ServerName, db.name AS DatabaseName, 1 AS IsPrivileged, usr.roleName AS Name, usr.isServerLevel AS IsServerLevel
			FROM Users usr JOIN Servers srv on srv.srvId = usr.serverId 
			JOIN Databases db on srv.srvId = db.srvId
			WHERE srv.srvId = ' + STR(@serverId) + ' AND db.name LIKE ''' + @databaseName + ''' AND usr.isPrivileged = 1 AND usr.roleName IS NOT NULL ' + @filterForUserType + @filterForRoleName +'
			GROUP BY instance, name, isPrivileged, roleName, isServerLevel

			UNION

			SELECT srv.instance AS ServerName, db.name AS DatabaseName, 1 AS IsPrivileged, usr.roleName AS Name, usr.isServerLevel AS IsServerLevel
			FROM Users usr JOIN Databases db on db.dbId = usr.databaseId 
			JOIN Servers srv on srv.srvId = db.srvId
			WHERE srv.srvId = ' + STR(@serverId) + ' AND db.name LIKE ''' + @databaseName + ''' AND usr.isPrivileged = 1 AND usr.roleName IS NOT NULL ' + @filterForUserType + @filterForRoleName +' AND usr.roleName NOT IN (SELECT roleName FROM Users WHERE isServerLevel = 1 AND isPrivileged = 1 AND serverId = srv.srvId AND roleName IS NOT NULL)
			GROUP BY instance, name, isPrivileged, roleName, isServerLevel

			UNION

			SELECT srv.instance AS ServerName, db.name AS DatabaseName, 1 AS IsPrivileged, usr.loginName AS LoginName , usr.isServerLevel AS IsServerLevel
			FROM Users usr JOIN Servers srv on srv.srvId = usr.serverId 
			JOIN Databases db on srv.srvId = db.srvId
			WHERE srv.srvId = ' + STR(@serverId) + ' AND db.name LIKE ''' + @databaseName + ''' AND usr.isPrivileged = 1 AND usr.loginName IS NOT NULL ' + @filterForUserType + @filterForLoginName +'
			GROUP BY instance, name, isPrivileged, loginName, isServerLevel

			UNION

			SELECT srv.instance AS ServerName, db.name AS DatabaseName, 1 AS IsPrivileged, usr.loginName AS LoginName , usr.isServerLevel AS IsServerLevel
			FROM Users usr JOIN Databases db on db.dbId = usr.databaseId 
			JOIN Servers srv on srv.srvId = db.srvId
			WHERE srv.srvId = ' + STR(@serverId) + ' AND db.name LIKE ''' + @databaseName + ''' AND usr.isPrivileged = 1 AND usr.loginName IS NOT NULL ' + @filterForUserType + @filterForLoginName +' AND usr.loginName NOT IN (SELECT loginName FROM Users WHERE isServerLevel = 1 AND isPrivileged = 1 AND serverId = srv.srvId AND loginName IS NOT NULL)
			GROUP BY instance, name, isPrivileged, loginName, isServerLevel';

	EXEC(@stmt)
end
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- no more needed in SQLcm 4.5
-- grant execute on sp_cmreport_GetLoginHistory to public
-- GO