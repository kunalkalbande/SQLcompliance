USE [SQLcompliance]
GO

IF (OBJECT_ID('sp_sqlcm_LinkDllCheck') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_LinkDllCheck
END
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_LinkDllCheck]    Script Date: 3/19/2016 1:14:28 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO


CREATE PROCEDURE [dbo].[sp_sqlcm_LinkDllCheck]( @dllFile nvarchar(50))
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
SELECT CAST(@Version AS INT) as [version]
END
ELSE 
SELECT 0 as [version]
GO

