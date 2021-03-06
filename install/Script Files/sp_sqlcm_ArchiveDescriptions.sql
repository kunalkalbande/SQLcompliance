USE [SQLcompliance]
GO
/****** Object:  StoredProcedure [dbo].[sp_sqlcm_ArchiveDescriptions]    Script Date: 4/21/2018 10:32:01 AM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_ArchiveDescriptions') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_ArchiveDescriptions
END
GO

CREATE procedure [dbo].[sp_sqlcm_ArchiveDescriptions] (
	@InstanceDatabase varchar(255),
	@days as int,
	@type varchar(255),
	@archive_year as varchar(max),
	@new_path varchar(max)
	)
	
as
declare @dbName as varchar(200)
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
declare @currentDay as nvarchar(max)
declare @currentMonth as varchar(max)
declare @quarter as nvarchar(50)
declare @currentQuarter as nvarchar(50)
declare @monthDays as nvarchar(50)
declare @remianDays as nvarchar(50)
declare @months as nvarchar(50)
declare @archiveDbName as nvarchar(100)
declare @currenMonth as int
declare @currenYear as int
declare @first as int
declare @last as int
declare @remainDay as int
declare @currentYear as nvarchar(10)
declare @startTime as varchar(max)
declare @endTime as varchar(max)
declare @sqlText varchar(max)
begin

set @quarter =RIGHT(@type, 1)
set @currentQuarter =DATEPART(QUARTER, GETDATE())


set @currentMonth =DATENAME(month, GETDATE())
set @currentYear=DATENAME(year, GETDATE())
set @month=MONTH(GETDATE()) 
set @currentDay=DAY(GETDATE())
set @currentMonth =SUBSTRING(UPPER(@currentMonth), 1,3)
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


if(@type="YEAR" and @days < 365)
Begin
	set @last=12	
	set @currenYear=CONVERT(int,@archive_year)
	set @startTime ='''1/1/'+@archive_year+' 12:00:00'''

	
end

else if(@type="Q1" and @days < 89)
Begin
	set @first=1
	set @last=3
	set @currenYear=CONVERT(int,@archive_year)
	set @startTime ='''1/1/'+@archive_year+' 12:00:00'''

end

else if(@type="Q2" and @days < 91)
Begin
	set @first=4
	set @last=6
	set @currenYear=CONVERT(int,@archive_year)
	set @startTime ='''4/1/'+@archive_year+' 12:00:00'''
end
else if(@type="Q3" and @days < 92)
Begin
	set @first=7
	set @last=9
	set @currenYear=CONVERT(int,@archive_year)
	set @startTime ='''7/1/'+@archive_year+' 12:00:00'''

end

else if(@type="Q4" and @days < 92)
Begin
	set @first=10
	set @last=12
	set @currenYear=CONVERT(int,@archive_year)
	set @startTime ='''10/1/'+@archive_year+' 12:00:00'''

end

else if(@type="JAN" and @days < 31)
Begin
		
	set @currenMonth=1
	set @currenYear=CONVERT(int,@archive_year)
	set @startTime ='''1/1/'+@archive_year+' 12:00:00'''
	
end

else if(@type="FEB" and @days < 28)
Begin
	set @currenMonth=2
	set @currenYear=CONVERT(int,@archive_year)
	set @startTime ='''2/1/'+@archive_year+' 12:00:00'''
end

else if(@type="MAR" and @days < 31)
Begin
	set @currenMonth=3
	set @currenYear=CONVERT(int,@archive_year)
	set @startTime ='''3/1/'+@archive_year+' 12:00:00'''

end

else if(@type="APR" and @days < 30)
Begin
	set @currenMonth=4
	set @currenYear=CONVERT(int,@archive_year)
	set @startTime ='''4/1/'+@archive_year+' 12:00:00'''
end

else if(@type="MAY" and @days < 31)
Begin
	set @currenMonth=5
	set @currenYear=CONVERT(int,@archive_year)
	set @startTime ='''5/1/'+@archive_year+' 12:00:00'''
end

else if(@type="JUN" and @days < 30)
Begin
	set @currenMonth=6
	set @currenYear=CONVERT(int,@archive_year)
	set @startTime ='''6/1/'+@archive_year+' 12:00:00'''

end

else if(@type="JUL" and @days < 31)
Begin
	set @currenMonth=7
	set @currenYear=CONVERT(int,@archive_year)
	set @startTime ='''7/1/'+@archive_year+' 12:00:00'''

end

else if(@type="AUG" and @days < 31)
Begin
	set @currenMonth=8
	set @currenYear=CONVERT(int,@archive_year)
	set @startTime ='''8/1/'+@archive_year+' 12:00:00'''
end
else if (@type="SEP" and @days < 30)
Begin
	set @currenMonth=9
	set @currenYear=CONVERT(int,@archive_year)
	set @startTime ='''9/1/'+@archive_year+' 12:00:00'''
end
else if(@type="OCT" and @days < 31)
Begin
	set @currenMonth=10
	set @currenYear=CONVERT(int,@archive_year)
	set @startTime ='''10/1/'+@archive_year+' 12:00:00'''
end
else if(@type="NOV" and @days < 30)
Begin
	set @currenMonth=11
	set @currenYear=CONVERT(int,@archive_year)
	set @startTime ='''11/1/'+@archive_year+' 12:00:00'''
end
else if(@type="DEC" and @days < 31)
Begin
	set @currenMonth=12
	set @currenYear=CONVERT(int,@archive_year)
	set @startTime ='''12/1/'+@archive_year+' 12:00:00'''
end


if(@currentMonth =@type)
Begin
	if(@year <= @archive_year)
		BEGIN

					set @remianDays =CONVERT(INT, @currentDay)-CONVERT(INT, @days) 
					set @remianDays =CONVERT(INT, @remianDays)
					set @remainDay=CONVERT(int, @remianDays) 

			 END

end

if(@quarter =@currentQuarter )
BEGIN

					set @remianDays =CONVERT(INT, @currentDay)-CONVERT(INT, @days) 
					set @remianDays =CONVERT(INT, @remianDays)

					set @remainDay=CONVERT(int, @remianDays) 


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

if (OBJECT_ID( '['+@archiveDbName+'].[dbo].[Description]' ) IS NULL )
begin
--set @descTableName ='['+@archiveDbName+'].[dbo].[Descriptions]'
set @dbName=' Create table ['+@archiveDbName+'].[dbo].[Description] '
Set @sql=''
set @sql='
	([instance] [nvarchar] (256),
	[displayName] [nvarchar] (512),
	[description] [nvarchar] (256),
	[databaseType] [nvarchar] (16) NULL,
	[startDate] [datetime],
	[endDate] [datetime], 
	[archiveBias] [int],
	[archiveStandardBias] [int],
	[archiveStandardDate] [datetime],
	[archiveDaylightBias] [int], 
	[archiveDaylightDate] [datetime],
	[archiveTimeZoneName] [nvarchar] (128), 
	[sqlComplianceDbSchemaVersion] [int], 
	[eventDbSchemaVersion] [int], 
	[bias] [int], 
	[standardBias] [int], 
	[standardDate] [datetime],
	[daylightBias] [int], 
	[daylightDate] [datetime],
	[defaultAccess] [int], 
	[eventReviewNeeded] [tinyint], 
	[containsBadEvents] [tinyint], 
	[state] [int] NULL, 
	[timeLastIntegrityCheck] 
	[datetime],
	[lastIntegrityCheckResult] [int], 
	[highWatermark] [int], 
	[lowWatermark] [int] )'
	Set @sql=@dbName + char(13) + @sql
	exec (@sql)
end
begin transaction t1
begin try


set @endTime = ''''+CONVERT(VARCHAR(20), GETDATE(), 101)+' '+CONVERT(VARCHAR(20), GETDATE(), 108)+''''
set @description = ''''+ @archive_year+' '+@type+' Events for SQL Server ' +@instanceName+''''
set @sql=''
set @dbName=''
set @serverName='[SQLcompliance_'+@InstanceDatabase+'].[dbo].[Description]'
set @sql='INSERT INTO ['+@archiveDbName+'].[dbo].[Description] '
set @sql=@sql + '([instance],[bias],[standardBias],[standardDate],[daylightBias],[daylightDate],[defaultAccess])
		         SELECT [instance],[bias],[standardBias],[standardDate],[daylightBias],[daylightDate],[defaultAccess] 
				 from [SQLcompliance].[dbo].[Servers] where instance = '''''+@instanceName+'''''' 
set @type=UPPER(@type)
set @sqlText = 'if ((Select count(instance) from ['+@archiveDbName+'].[dbo].[Description] where instance ='''+@instanceName+''') = 0)
begin
exec ('''+@sql+''')
end'

exec(@sqlText)
set @sql=''
set @sql ='Update ['+@archiveDbName+'].[dbo].[Description] set [databaseType] = ''Archive'', [startDate] = '+@startTime+',
 [endDate]='+@endTime+', [state] = ''0'',[archiveBias] =''0'', [archiveStandardBias] =''0'', [archiveDaylightBias] =''0'', [description] ='+@description
	exec(@sql)


set @sql =''
set @sql = 'UPDATE ['+@archiveDbName+'].[dbo].[Description] set eventDbSchemaVersion = t2.eventDbSchemaVersion, sqlComplianceDbSchemaVersion = t2.sqlComplianceDbSchemaVersion 
			from ['+@archiveDbName+'].[dbo].[Description] t1 JOIN '+@serverName+' t2 ON t1.instance =t2.instance'
	exec(@sql)

if (OBJECT_ID( '['+@archiveDbName+'].[dbo].[Events]' ) IS NOT NULL )
begin	
set @sql =''
set @sql = 'UPDATE ['+@archiveDbName+'].[dbo].[Description] set lowWatermark = (SELECT MIN(eventId)-1 FROM ['+@archiveDbName+'].[dbo].[Events]), 
			highWatermark = (SELECT MAX(eventId) FROM ['+@archiveDbName+'].[dbo].[Events])' 			
	exec(@sql)
end

	set @sql=''
	set @sql= 'UPDATE [SQLcompliance].[dbo].[Servers] set timeLastArchive ='''+convert(varchar(35), getdate(), 120)+''' where instance='''+@instanceName+''''
	exec(@sql)
	

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
 

