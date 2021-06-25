/****** RecommendedPermissions_Sql2012+.sql ******/
-- IDERA SQL compliance Manager Version 5.6.1.0
--
-- (c) Copyright 2004-2019 IDERA, Inc., All Rights Reserved.
-- SQL Compliance Manager, IDERA and the IDERA Logo are trademarks or registered trademarks
-- of IDERA or its subsidiaries in the United States and other jurisdictions.
--
-- Permissions Check Script for SQL Server 2012 and above
-- Grants required Permissions to allow using SQLCM for a non sysadmin user
-- Please follow the below steps to use this script for non sysadmin user
-- 1. Replace all instances of DOMAIN\USER with the user details.
-- 2. Execute the script using sysadmin user.
--
-- SQLCM 5.6- 566/740/4620/5280 (Non-Admin and Non-Sysadmin role) Permissions
--
USE [master]
GO
    DECLARE @UserSid VARBINARY(85);
	SELECT @UserSid = SUSER_SID('DOMAIN\USER')
	IF NOT EXISTS(SELECT * FROM sys.server_principals WHERE sid = @UserSid)
	BEGIN
	    CREATE LOGIN [DOMAIN\USER] FROM WINDOWS WITH DEFAULT_DATABASE=[master]
	END
GO
DECLARE @Command NVARCHAR(MAX);
SELECT @Command = '
USE [?]
BEGIN TRY
    DECLARE @UserSid VARBINARY(85);
    DECLARE @Name NVARCHAR(MAX);
    SELECT @UserSid = SUSER_SID(''DOMAIN\USER'')
    IF NOT EXISTS(SELECT * FROM sys.database_principals WHERE sid = @UserSid)
    BEGIN
	CREATE USER [DOMAIN\USER] FOR LOGIN [DOMAIN\USER]
    END
    SELECT @Name = name FROM sys.database_principals WHERE sid = @UserSid;
    IF @Name <> ''dbo''
    BEGIN
    	ALTER USER [DOMAIN\USER] WITH DEFAULT_SCHEMA=[dbo]
        ALTER ROLE [db_accessadmin] ADD MEMBER [DOMAIN\USER]
        ALTER ROLE [db_datareader] ADD MEMBER [DOMAIN\USER]
        ALTER ROLE [db_datawriter] ADD MEMBER [DOMAIN\USER]
        ALTER ROLE [db_ddladmin] ADD MEMBER [DOMAIN\USER]
        ALTER ROLE [db_owner] ADD MEMBER [DOMAIN\USER]
        ALTER ROLE [db_securityadmin] ADD MEMBER [DOMAIN\USER]
    END
END TRY
BEGIN CATCH
  -- Empty Catch
  PRINT ''?''
  PRINT ERROR_NUMBER()
  PRINT ERROR_MESSAGE();
END CATCH
'
EXEC sp_MSforeachdb @Command

USE [master]
-- Required for Server Role
ALTER SERVER ROLE [securityadmin] ADD MEMBER [DOMAIN\USER]
ALTER SERVER ROLE [dbcreator] ADD MEMBER [DOMAIN\USER]
-- Required at the server securables
GRANT ALTER ANY DATABASE TO [DOMAIN\USER]
GRANT ALTER ANY EVENT NOTIFICATION TO [DOMAIN\USER]
GRANT ALTER ANY EVENT SESSION TO [DOMAIN\USER]
GRANT ALTER ANY LOGIN TO [DOMAIN\USER]
GRANT ALTER ANY SERVER AUDIT TO [DOMAIN\USER]
GRANT ALTER ANY SERVER ROLE TO [DOMAIN\USER]
GRANT ALTER RESOURCES TO [DOMAIN\USER]
GRANT ALTER SERVER STATE TO [DOMAIN\USER]
GRANT ALTER SETTINGS TO [DOMAIN\USER]
GRANT ALTER TRACE TO [DOMAIN\USER]
GRANT CONNECT SQL TO [DOMAIN\USER]
GRANT CONTROL SERVER TO [DOMAIN\USER]
GRANT CREATE ANY DATABASE TO [DOMAIN\USER]
GRANT CREATE SERVER ROLE TO [DOMAIN\USER]
GRANT VIEW ANY DATABASE TO [DOMAIN\USER]
GRANT VIEW ANY DEFINITION TO [DOMAIN\USER]
GRANT VIEW SERVER STATE TO [DOMAIN\USER]
-- Required whenever new stored procedure is created in the master
GRANT EXECUTE TO [DOMAIN\USER]
GRANT ALTER TO [DOMAIN\USER]
GRANT DELETE TO [DOMAIN\USER]
GRANT INSERT TO [DOMAIN\USER]
GRANT SELECT TO [DOMAIN\USER]
GRANT CONTROL TO [DOMAIN\USER]
GRANT TAKE OWNERSHIP TO [DOMAIN\USER]
GO

USE [master]
IF NOT EXISTS(SELECT 1 FROM sysobjects WHERE name = 'sp_SQLcompliance_StartUp' AND type = 'P') 
BEGIN
    DECLARE @Command NVARCHAR(MAX);
    SELECT @Command = 'CREATE PROC [dbo].[sp_SQLcompliance_StartUp]
    AS
    BEGIN
		PRINT ''SQLCM: Stored Procedure Blanked to keep Sysadmin permissions and registered on startup intact'';
    END'
	EXEC SP_EXECUTESQL @Command
END
IF OBJECTPROPERTY ( OBJECT_ID('sp_SQLcompliance_StartUp'),'ExecIsStartup') <> 1
BEGIN
    -- RegisterStartupSP: Register sp_SQLcompliance_StartUp Stored Procedure to run on every startup
	-- Dropped occasionally and dependency on sp_SQLcompliance_Audit responsible for traces
	EXEC sp_procoption sp_SQLcompliance_StartUp, 'startup', true
END

--
-- Defines Permissions Check to be used for a non-sysadmin user by SQLCompliance Permissions Check
USE [master]
-- Used by the Permissions Check GUI to check for permissions for Non Sysadmin Users
IF (OBJECT_ID('sp_SQLcompliance_PermissionsCheck') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_SQLcompliance_PermissionsCheck
END
GO
CREATE PROC [dbo].[sp_SQLcompliance_PermissionsCheck]
AS
BEGIN
	-- sysadmin role
    IF(IS_SRVROLEMEMBER('sysadmin') = 1)
	BEGIN
		SELECT 1;
		RETURN;
	END

	DECLARE @UserName NVARCHAR(MAX);
	SELECT  @UserName = SUSER_NAME(SUSER_ID());
	-- non-sysadmin role member
    DECLARE @UserSid VARBINARY(85);
	SELECT @UserSid = SUSER_SID(@UserName)
	IF NOT EXISTS(SELECT * FROM sys.server_principals WHERE sid = @UserSid)
	BEGIN
		SELECT 0;
		RETURN;
	END

    DECLARE @Command NVARCHAR(MAX);
    DECLARE @ErrorTable TABLE 
    (
        HasPermissions BIT
    );
    SELECT @Command = '
BEGIN TRY
    USE [?]
    DECLARE @UserSid VARBINARY(85);
    DECLARE @PermissionsError NVARCHAR(MAX);
    DECLARE @Name NVARCHAR(MAX);
    SELECT @UserSid = SUSER_SID('''+@UserName+''')
    SELECT @Name = name FROM sys.database_principals WHERE sid = @UserSid;
    IF NOT EXISTS(SELECT 1 FROM sys.database_principals WHERE sid = @UserSid) OR SCHEMA_NAME() <> ''dbo'' OR NOT EXISTS(SELECT 1 FROM sys.database_role_members AS dRo JOIN sys.database_principals AS dPrinc ON dRo.member_principal_id = dPrinc.principal_id JOIN sys.database_principals AS dRole ON dRo.role_principal_id = dRole.principal_id WHERE dRole.name IN(''db_datareader'', ''db_owner'') AND dPrinc.name = @Name)	
    BEGIN
	SELECT 0;
	RETURN
    END
END TRY
BEGIN CATCH
    SELECT 0;
END CATCH
';
    --  Read into the Error Table
    INSERT INTO @ErrorTable
    EXEC sp_MSforeachdb @Command

	IF EXISTS(SELECT 1 FROM @ErrorTable WHERE HasPermissions = 0)
	BEGIN
		SELECT 0;
		RETURN;
	END
	
	DELETE FROM @ErrorTable;

	SELECT @Command = '
BEGIN TRY
    USE [?]
	DECLARE @Db NVARCHAR(MAX);
	SET @Db = ''?'';    
	DECLARE @Cnt INT;
	IF @Db = ''SQLcompliance'' OR @Db = ''SQLcomplianceProcessing'' OR CharIndex(''SQLcompliance_'', @Db) = 1 OR @Db = ''master''
    BEGIN
		USE [?]
		SELECT @Cnt = COUNT(*) FROM sys.database_role_members AS dRo JOIN sys.database_principals AS dPrinc ON dRo.member_principal_id = dPrinc.principal_id JOIN sys.database_principals AS dRole ON dRo.role_principal_id = dRole.principal_id WHERE dPrinc.name = '''+@UserName+''' AND dRole.name IN(''db_datareader'', ''db_owner'', ''db_accessadmin'', ''db_securityadmin'', ''db_ddladmin'', ''db_datareader'', ''db_datawriter'')
		IF @Cnt <> 6
		BEGIN
			SELECT 0;
			RETURN;
		END
		IF @Db = ''master''
	    BEGIN
	    	IF EXISTS(SELECT 1 FROM sysobjects WHERE name = ''sp_SQLcompliance_StartUp'' AND type = ''P'') AND 
	    		OBJECTPROPERTY ( OBJECT_ID(''sp_SQLcompliance_StartUp''),''ExecIsStartup'') <> 1
	    	BEGIN
	    		SELECT 0;
	    		RETURN;
	    	END
	    END
    END
	ELSE IF @Db = ''msdb'' OR @Db = ''tempdb''
	BEGIN
		IF NOT EXISTS(SELECT 1 FROM sys.database_role_members AS dRo JOIN sys.database_principals AS dPrinc ON dRo.member_principal_id = dPrinc.principal_id JOIN sys.database_principals AS dRole ON dRo.role_principal_id = dRole.principal_id WHERE dPrinc.name = '''+@UserName+''' AND dRole.name  = ''db_datawriter'')
		BEGIN
			SELECT 0;
		END
	END
END TRY
BEGIN CATCH
	SELECT 0;
END CATCH
';
    --  Read into the Error Table
    INSERT INTO @ErrorTable
    EXEC sp_MSforeachdb @Command

	IF EXISTS(SELECT 1 FROM @ErrorTable WHERE HasPermissions = 0)
	BEGIN
		SELECT 0;
		RETURN;
	END

	DELETE FROM @ErrorTable;

	IF IS_SRVROLEMEMBER('securityadmin') <> 1 OR IS_SRVROLEMEMBER('dbcreator') <> 1
	BEGIN
		SELECT 0;
		RETURN;
	END

	DECLARE @Cnt INT;
	SELECT @Cnt = COUNT(*) FROM sys.fn_my_permissions(NULL,NULL) where permission_name IN('ALTER ANY DATABASE', 'ALTER ANY EVENT NOTIFICATION', 'ALTER ANY EVENT SESSION', 'ALTER ANY LOGIN', 'ALTER ANY SERVER AUDIT', 'ALTER ANY SERVER ROLE', 'ALTER RESOURCES', 'ALTER SERVER STATE', 'ALTER SETTINGS', 'ALTER TRACE', 'CONNECT SQL', 'CONTROL SERVER', 'CREATE ANY DATABASE', 'CREATE SERVER ROLE', 'VIEW ANY DATABASE', 'VIEW ANY DEFINITION', 'VIEW SERVER STATE');
	IF(@Cnt <> 17)
	BEGIN
		SELECT 0;
		RETURN;
	END
	
	SELECT @Cnt = COUNT(*) FROM sys.fn_my_permissions(NULL,'DATABASE') where permission_name IN('EXECUTE', 'ALTER', 'DELETE', 'INSERT', 'SELECT', 'CONTROL', 'TAKE OWNERSHIP');
	IF(@Cnt <> 7)
	BEGIN
		SELECT 0;
		RETURN;
	END

	SELECT 1
END

GO
GRANT EXECUTE ON [sp_SQLcompliance_PermissionsCheck] TO [public];
GO

--
-- Run in the Repository to define the Repository Startup method to perform sysadmin actions
--

USE [master]
-- Changes to update Privileged users and Trusted Users at database level
IF OBJECT_ID('SqlCmStartupDllChecker') IS NULL
BEGIN
    CREATE TABLE [SqlCmStartupDllChecker](
    	[id] [bigint] IDENTITY(1,1) NOT NULL,
    	[DllName] [NVARCHAR](MAX) NOT NULL,
    	[DllVersion] [INT] NOT NULL
    ) ON [PRIMARY]
	GRANT SELECT ON [SqlCmStartupDllChecker] TO [public];
END

GO

IF (OBJECT_ID('sp_sqlcm_LinkDllCheck') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_LinkDllCheck
END
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_LinkDllCheck]    Script Date: 3/19/2016 1:14:28 PM ******/
CREATE PROCEDURE [dbo].[sp_sqlcm_LinkDllCheck]( @dllFile nvarchar(50), @withSelect bit)
as
-- To allow advanced options to be changed.  
EXEC sp_configure 'show advanced options', 1;
-- To update the currently configured value for advanced options.  
RECONFIGURE; 
-- To enable the feature.  
EXEC sp_configure 'xp_cmdshell', 1;
-- To update the currently configured value for this feature.  
RECONFIGURE;  
DECLARE @sqlServerPath nvarchar(255) 
DECLARE @VersionNumber nvarchar(3) 
DECLARE @FileDirectory nvarchar(255)
DECLARE @Count int 
DECLARE @RowCount int
DECLARE @getVersionQuery NVARCHAR(255) 
DECLARE @Version NVARCHAR(20)
DECLARE @GacPath NVARCHAR(200)
DECLARE @GacDirectory nvarchar(200)
CREATE TABLE #files (ID int IDENTITY, fileList varchar(100))
CREATE TABLE #SubFiles (ID int IDENTITY, fileList varchar(100))
CREATE TABLE #TempTable (ID int IDENTITY, [version] nvarchar(20))
SET @sqlServerPath = 'EMPTY'
SET @VersionNumber = 0
SET @Count = 1
EXEC master..xp_regread
'HKEY_LOCAL_MACHINE',
'SOFTWARE\Microsoft\Microsoft SQL Server\130',
'VerSpecificRootDir',
@sqlServerPath OUTPUT

IF(@sqlServerPath like 'EMPTY')
EXEC master..xp_regread
'HKEY_LOCAL_MACHINE',
'SOFTWARE\Microsoft\Microsoft SQL Server\120',
'VerSpecificRootDir',
@sqlServerPath OUTPUT
ELSE IF(@VersionNumber like '0')
SET @VersionNumber = '130'

if(@sqlServerPath like 'EMPTY')
EXEC master..xp_regread
'HKEY_LOCAL_MACHINE',
'SOFTWARE\Microsoft\Microsoft SQL Server\110',
'VerSpecificRootDir',
@sqlServerPath OUTPUT
ELSE IF(@VersionNumber like '0')
SET @VersionNumber = '120'

if(@sqlServerPath like 'EMPTY')
EXEC master..xp_regread
'HKEY_LOCAL_MACHINE',
'SOFTWARE\Microsoft\Microsoft SQL Server\100',
'VerSpecificRootDir',
@sqlServerPath OUTPUT
ELSE IF(@VersionNumber like '0')
SET @VersionNumber = '110'

if(@sqlServerPath like 'EMPTY')
EXEC master..xp_regread
'HKEY_LOCAL_MACHINE',
'SOFTWARE\Microsoft\Microsoft SQL Server\90',
'VerSpecificRootDir',
@sqlServerPath OUTPUT
ELSE IF(@VersionNumber like '0')
SET @VersionNumber = '100'

IF(@sqlServerPath not like 'EMPTY')
BEGIN
	IF(@VersionNumber like '0')
		SET @VersionNumber = '90'
print @sqlServerPath
print @VersionNumber
		SET @sqlServerPath = SUBSTRING(@sqlServerPath,1,CHARINDEX(@VersionNumber,@sqlServerPath)-1)
END
IF(@sqlServerPath like 'EMPTY' OR @sqlServerPath = '')
SET @sqlServerPath = 'C:\Program Files\Microsoft SQL Server\'
declare @directory nvarchar(200)
SET @directory = 'dir "'+ @sqlServerPath+ '" /b'
insert into #files execute xp_cmdshell @directory
SET @RowCount = (SELECT COUNT(*) FROM #files)
WHILE(@RowCount >= @Count)
BEGIN
IF((SELECT COUNT(*) FROM #SubFiles)>0)
BEGIN
DELETE FROM #SubFiles
DBCC CHECKIDENT ('#SubFiles', RESEED, 0)
END
SET @FileDirectory = (SELECT TOP 1 fileList FROM #files WHERE ID = @Count)
DECLARE @SubDirectory NVARCHAR(200)
SET @SubDirectory =  'dir "'+ @sqlServerPath+ @FileDirectory+ '\Shared" /b'
INSERT INTO #SubFiles EXECUTE xp_cmdshell @SubDirectory
IF ((SELECT COUNT (*) FROM #SubFiles WHERE fileList = @dllFile) >0)
BEGIN
SET @getVersionQuery = 'WMIC DATAFILE WHERE name="'+@sqlServerPath+ @FileDirectory+'\Shared\'+@dllFile+'" get Version'
SET @getVersionQuery = REPLACE(@getVersionQuery,'\','\\')
INSERT INTO #TempTable EXEC xp_cmdshell @getVersionQuery
SET @Version = ( SELECT TOP 1 [version] FROM #TempTable where ID = 2)
if(@Version IS NOT NULL AND @Version <> '')
BEGIN
SET @Version = SUBSTRING(@Version,CHARINDEX('.',@Version)+1,LEN(@Version))
SET @Version = SUBSTRING(@Version,1,CHARINDEX('.',@Version)-1)
BREAK;
END
print @Version
END
SET @Count = @Count + 1
END
IF(@Version IS NULL OR @Version = 0)
BEGIN
SET @Count = 1
SET @GacPath = 'C:\Windows\Microsoft.NET\assembly\GAC_64\'+SUBSTRING(@dllFile,0,CHARINDEX('dll',@dllFile))+'\'
SET @GacDirectory = 'dir "'+ @GacPath+ '" /b'
DECLARE @GacSubDirectory nvarchar(200)
DECLARE @GacFileDirectory nvarchar(200)
IF((SELECT COUNT(*) FROM #files)>0)
BEGIN
DELETE FROM #files
DBCC CHECKIDENT ('#files', RESEED, 0)
END
INSERT INTO #files EXECUTE xp_cmdshell @GacDirectory
SET @RowCount = (SELECT COUNT(*) FROM #files)
WHILE(@Count <= @RowCount)
BEGIN
IF((SELECT COUNT(*) FROM #SubFiles)>0)
BEGIN
DELETE FROM #SubFiles
DBCC CHECKIDENT ('#SubFiles', RESEED, 0)
END
SET @GacFileDirectory = (SELECT TOP 1 fileList FROM #files WHERE ID = @Count)
SET @GacSubDirectory = 'dir "'+ @GacPath+ @GacFileDirectory+ '" /b'
INSERT INTO #SubFiles EXECUTE xp_cmdshell @GacSubDirectory
IF ((SELECT COUNT (*) FROM #SubFiles WHERE fileList = @dllFile) > 0)
BEGIN
SET @getVersionQuery = 'WMIC DATAFILE WHERE name="'+ @GacPath + @GacFileDirectory+'\'+ @dllFile+'" get Version'
SET @getVersionQuery = REPLACE(@getVersionQuery,'\','\\')
IF((SELECT COUNT(*) FROM #TempTable)>0)
BEGIN
DELETE FROM #TempTable
DBCC CHECKIDENT ('#TempTable', RESEED, 0)
END
INSERT INTO #TempTable EXEC xp_cmdshell @getVersionQuery
SET @Version = ( SELECT TOP 1 [version] FROM #TempTable where ID = 2)
if(@Version IS NOT NULL AND @Version <> '')
BEGIN
SET @Version = SUBSTRING(@Version,CHARINDEX('.',@Version)+1,LEN(@Version))
SET @Version = SUBSTRING(@Version,1,CHARINDEX('.',@Version)-1)
BREAK;
END
print @Version
END
SET @Count = @Count + 1
END
END
IF(@Version IS NULL OR @Version = 0)
BEGIN
SET @Count = 1 
SET @GacPath = 'C:\Windows\Microsoft.NET\assembly\GAC_32\'+SUBSTRING(@dllFile,0,CHARINDEX('dll',@dllFile))+'\'
IF((SELECT COUNT(*) FROM #files)>0)
BEGIN
DELETE FROM #files
DBCC CHECKIDENT ('#files', RESEED, 0)
END
INSERT INTO #files EXECUTE xp_cmdshell @GacDirectory
SET @RowCount = (SELECT COUNT(*) FROM #files)
WHILE(@Count <= @RowCount)
BEGIN
IF((SELECT COUNT(*) FROM #SubFiles)>0)
BEGIN
DELETE FROM #SubFiles
DBCC CHECKIDENT ('#SubFiles', RESEED, 0)
END
SET @GacFileDirectory = (SELECT TOP 1 fileList FROM #files WHERE ID = @Count)
SET @GacSubDirectory = 'dir "'+ @GacPath+ @GacFileDirectory+ '" /b'
INSERT INTO #SubFiles EXECUTE xp_cmdshell @GacSubDirectory
IF ((SELECT COUNT (*) FROM #SubFiles WHERE fileList = @dllFile) > 0)
BEGIN
SET @getVersionQuery = 'WMIC DATAFILE WHERE name="'+ @GacPath + @GacFileDirectory+'\'+ @dllFile+'" get Version'
SET @getVersionQuery = REPLACE(@getVersionQuery,'\','\\')
IF((SELECT COUNT(*) FROM #TempTable)>0)
BEGIN
DELETE FROM #TempTable
DBCC CHECKIDENT ('#TempTable', RESEED, 0)
END
INSERT INTO #TempTable EXEC xp_cmdshell @getVersionQuery
SET @Version = ( SELECT TOP 1 [version] FROM #TempTable where ID = 2)
if(@Version IS NOT NULL AND @Version <> '')
BEGIN
SET @Version = SUBSTRING(@Version,CHARINDEX('.',@Version)+1,LEN(@Version))
SET @Version = SUBSTRING(@Version,1,CHARINDEX('.',@Version)-1)
BREAK;
END
print @Version
END
SET @Count = @Count + 1
END
END
IF(@Version IS NOT NULL)
BEGIN
    -- Update the Dll Value in the [SqlCmStartupDllChecker] table
    UPDATE [SqlCmStartupDllChecker] SET [DllVersion]=CAST(@Version AS INT) where [DllName]= @dllFile
    IF @@ROWCOUNT=0
       INSERT INTO [SqlCmStartupDllChecker]([DllName], [DllVersion]) VALUES(@dllFile, CAST(@Version AS INT));
    
	IF @withSelect = 1
    BEGIN
        SELECT CAST(@Version AS INT) as [version]
    END
END
ELSE 
    IF @withSelect = 1
    BEGIN
        SELECT 0 as [version]
    END
GO

GO

IF (OBJECT_ID('sp_SQLcompliance_Repository_StartUp') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_SQLcompliance_Repository_StartUp
END
GO

/****** Object:  StoredProcedure [dbo].[sp_SQLcompliance_Repository_StartUp]    Script Date: 23/11/2018 13:27:25 ******/
-- IDERA SQL compliance Manager Version 5.6.1.0
--
-- (c) Copyright 2004-2018 IDERA, Inc., All Rights Reserved.
-- SQL Compliance Manager, IDERA and the IDERA Logo are trademarks or registered trademarks
-- of IDERA or its subsidiaries in the United States and other jurisdictions.

CREATE PROC [dbo].[sp_SQLcompliance_Repository_StartUp]
AS
BEGIN 
  SET NOCOUNT ON
  BEGIN TRY
  EXEC sp_sqlcm_LinkDllCheck 'Microsoft.SqlServer.XE.Core.dll', 0
  EXEC sp_sqlcm_LinkDllCheck 'Microsoft.SqlServer.XEvent.Linq.dll', 0
  END TRY
  BEGIN CATCH
  -- Suppress exception if any
  END CATCH
END
GO
-- check the "ExecIsStartup" property of an SP you can determine if the SP is set up for autoexecution
IF OBJECTPROPERTY ( object_id('sp_SQLcompliance_Repository_StartUp'),'ExecIsStartup') <> 1
BEGIN
    -- RegisterStartupSP: Register sp_SQLcompliance_StartUp Stored Procedure to run on every startup
    -- Dropped occasionally and dependency on sp_SQLcompliance_Audit responsible for traces
    EXEC sp_procoption [sp_SQLcompliance_Repository_StartUp], 'startup', true
	EXEC [sp_SQLcompliance_Repository_StartUp]
END
GO
