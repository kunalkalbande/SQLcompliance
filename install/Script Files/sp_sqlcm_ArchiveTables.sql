USE [SQLcompliance]
GO
/****** Object:  StoredProcedure [dbo].[sp_sqlcm_ArchiveTables]    Script Date: 2/19/2018 11:19:03 AM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF (OBJECT_ID('sp_sqlcm_ArchiveTables') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_ArchiveTables
END
GO

create procedure [dbo].[sp_sqlcm_ArchiveTables] (
	@InstanceDatabase varchar(255),
	@days as int,
	@type varchar(255),
	@archive_year as varchar(max),
	@new_path varchar(max)
	)
	
as
declare @dbName as varchar(200)
declare @noOfDays as varchar(200)
declare @sql nvarchar(max)
declare @description nvarchar(max)
declare @serverName nvarchar(max)
declare @year as varchar(max)
declare @new_mdf_file nvarchar(max)
declare @new_ldf_file nvarchar(max)
declare @db_id as varchar(50)
declare @db_name  as nvarchar(max)
declare @new_mdf_path as nvarchar(max)
declare @new_ldf_path as nvarchar(max)
declare @instanceName as nvarchar(max)
declare @month as nvarchar(max)
declare @months as nvarchar(50)
declare @archiveDbName as nvarchar(100)
begin


set @noOfDays=convert(varchar(10),DATEADD(DAY, -@days, getdate()),120)

set @month=MONTH(GETDATE()) 
set @year= YEAR(GETDATE())
set @InstanceDatabase= UPPER(@InstanceDatabase)
set @instanceName=@InstanceDatabase
set @InstanceDatabase=replace(@InstanceDatabase,'\','_')

set @type = UPPER(@type)
if(@days > 0)
BEGIN
if(@type in ('YEAR','Q1','Q2','Q3','Q4','JAN','FEB','MAR','APR','MAY','JUN','JUL','AUG','SEP','OCT','NOV','DEC'))
BEGIN
if(@type="JAN")
BEGIN
set @months="01"
END
if(@type="FEB")
BEGIN
set @months="02"
END
if(@type="MAR")
BEGIN
set @months="03"
END
if(@type="APR")
BEGIN
set @months="04"
END
if(@type="MAY")
BEGIN
set @months="05"
END
if(@type="JUN")
BEGIN
set @months="06"
END
if(@type="JUL")
BEGIN
set @months="07"
END
if(@type="AUG")
BEGIN
set @months="08"
END
if(@type="SEP")
BEGIN
set @months="09"
END
if(@type="OCT")
BEGIN
set @months="10"
END
if(@type="NOV")
BEGIN
set @months="11"
END
if(@type="DEC")
BEGIN
set @months="12"
END

if(@type in ('JAN','FEB','MAR','APR','MAY','JUN','JUL','AUG','SEP','OCT','NOV','DEC'))
BEGIN
	set @type=UPPER(LEFT(@type,1))+LOWER(SUBSTRING(@type,2,LEN(@type)))
END
if(@type in ('Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'))
BEGIN
	set @archiveDbName ='SQLcmArchive_'+@InstanceDatabase+'_'+@archive_year+'-'+@months+' ('+@type+')'
END

IF(@type in ('Q1','Q2','Q3','Q4'))
BEGIN		
set @archiveDbName ='SQLcmArchive_'+@InstanceDatabase+'_'+@archive_year+'_'+@type
END

IF(@type in ('YEAR'))
BEGIN		
set @archiveDbName ='SQLcmArchive_'+@InstanceDatabase+'_'+@archive_year
END
If not exists (SELECT * FROM sys.databases where name=@archiveDbName)
Begin

	if @new_path = NULL
		BEGIN
			set @sql=''
			set @sql = 'create database ' + QUOTENAME(@archiveDbName)
			exec (@sql)
			--set @archiveDbName ='SQLcmArchive_'+@InstanceDatabase+'_'+@archive_year+'-'+@months+' ('+@type+')'
		END
	ELSE
	BEGIN
			set @sql=''
			set @new_mdf_file=@archiveDbName+'Data.mdf';
			set @new_ldf_file=@archiveDbName+'Log.ldf';
			set @new_mdf_path=@new_path+'\'+@new_mdf_file
			set @new_ldf_path=@new_path+'\'+@new_ldf_file
			EXEC sp_detach_db @db_name;
			set @sql='CREATE DATABASE '+ QUOTENAME(@archiveDbName)+'   
					ON PRIMARY (NAME='''+@new_mdf_file+''', FILENAME= '''+@new_mdf_path+''')'+'   
					 LOG ON (NAME='''+@new_ldf_file+''', FILENAME= '''+@new_ldf_path+''')' 
			exec(@sql)
	END
end

if (OBJECT_ID( '['+@archiveDbName+'].[dbo].[Tables]' ) IS NULL )
begin
set @dbName=' Create table ['+@archiveDbName+'].[dbo].[Tables] '
Set @sql=''
set @sql='( [schemaName] [nvarchar] (128) NOT NULL,[name] [nvarchar] (128) NOT NULL,[id] [int] NOT NULL )'
Set @sql=@dbName + char(13) + @sql
	exec (@sql)
end
begin transaction t1
begin try
set @description = ''''+ @archive_year+' '+@type+' Events for SQL Server ' +@instanceName+''''
	set @db_id = DB_ID(@archiveDbName)
if not exists (select * from SQLcompliance.dbo.SystemDatabases where databaseName = @archiveDbName)
begin
	if(@type in ('JAN','FEB','MAR','APR','MAY','JUN','JUL','AUG','SEP','OCT','NOV','DEC'))
	BEGIN
		set @type=UPPER(LEFT(@type,1))+LOWER(SUBSTRING(@type,2,LEN(@type)))
	END
	if(@type in ('Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'))
	BEGIN
		set @sql=''
		set @sql='INSERT INTO SQLcompliance.dbo.SystemDatabases (instance, databaseName, databaseType, dateCreated, sqlDatabaseId, displayName,description) 
		values('''+@instanceName+''','+''''+@archiveDbName+''', ''Archive'','''+convert(varchar(35), getdate(), 120)+''','''+ @db_id +''','''
		+@archive_year+'-'+@months+' ('+@type+')'','+@description+')'
		exec(@sql)
	end
	IF(@type in ('Q1','Q2','Q3','Q4'))
	BEGIN		
		set @sql=''
		set @sql='INSERT INTO SQLcompliance.dbo.SystemDatabases (instance, databaseName, databaseType, dateCreated, sqlDatabaseId, displayName,description) 
		values('''+@instanceName+''','+''''+@archiveDbName+''', ''Archive'','''+convert(varchar(35), getdate(), 120)+''','''+ @db_id +''','''
		+@archive_year+' '+@type+''','+@description+')'
		exec(@sql)
	END
	IF(@type in ('YEAR'))
	BEGIN		
		set @sql=''
		set @sql='INSERT INTO SQLcompliance.dbo.SystemDatabases (instance, databaseName, databaseType, dateCreated, sqlDatabaseId, displayName,description) 
		values('''+@instanceName+''','+''''+@archiveDbName+''', ''Archive'','''+convert(varchar(35), getdate(), 120)+''','''+ @db_id +''','''
		+@archive_year+''','+@description+')'
		exec(@sql)
	END
end
	print 'success'
	commit transaction t1
end try

begin catch
           rollback transaction t1
end catch

END
END
end
 

