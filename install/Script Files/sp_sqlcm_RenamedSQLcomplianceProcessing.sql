USE [master]
GO
/****** Object:  StoredProcedure [dbo].[sp_sqlcm_RenamedSQLcomplianceProcessing]    Script Date: 4/6/2016 12:18:42 PM
Create By: Abhay/Sonu
 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_RenamedSQLcomplianceProcessing') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_RenamedSQLcomplianceProcessing
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_RenamedSQLcomplianceProcessing]
AS BEGIN
DECLARE @sql1 varchar(5000)
DECLARE @sql varchar (5000)
DECLARE @final varchar (5000)
DECLARE @multiUser varchar(5000)
Declare @SetMultiuser varchar (5000)
Declare @dbName varchar (200)
set @sql1=(select name from [master]..[sysdatabases] where name='SQLcomplianceProcessing' OR name='SQLcompliance.Processing')
set @sql='USE [' + @sql1 + ']'
set @dbName= 'SQLcomplianceProcessing'
set @multiUser = 'ALTER DATABASE [' + @sql1 + '] set SINGLE_USER WITH ROLLBACK IMMEDIATE;'
set @SetMultiuser= 'ALTER DATABASE [' + @dbName + '] set MULTI_USER;'
exec (@sql)
set @final= 'ALTER DATABASE [' +  @sql1 + '] MODIFY NAME = [SQLcomplianceProcessing];'
IF EXISTS(SELECT * FROM [master]..[sysdatabases] WHERE name=@sql1 )  
BEGIN
exec (@multiUser);
exec (@final);
exec (@SetMultiuser);
END
END