USE [SQLcompliance]
GO
/****** Object:  StoredProcedure [dbo].[sp_sqlcm_ArchiveEvents]    Script Date: 9/21/2016 3:07:06 PM ******/
/****** These stored procedures are not called from anywhere in the code, but these are standalone procedures to help with archiving. ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF (OBJECT_ID('sp_sqlcm_ArchiveEvents') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_ArchiveEvents
END
GO
Create procedure [dbo].[sp_sqlcm_ArchiveEvents] (
	@InstanceDatabase varchar(255),
	@days as int,
	@type varchar(255),
	@archive_year as varchar(max),
	@new_path varchar(max)
	)
	
as
declare @dbName as varchar(200)
declare @todayDate as varchar(200)
declare @noOfDays as varchar(200)
declare @sql nvarchar(max)
declare @serverName nvarchar(max)
declare @sql1 nvarchar(max)
declare @year as varchar(max)
declare @startDate as varchar(200)
declare @endDate as varchar(200)
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
begin

set @quarter =RIGHT(@type, 1)
set @currentQuarter =DATEPART(QUARTER, GETDATE())

set @noOfDays=convert(varchar(10),DATEADD(DAY, -@days, getdate()),120)

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
	
end

else if(@type="Q1" and @days < 89)
Begin
	set @first=1
	set @last=3
	set @currenYear=CONVERT(int,@archive_year)
end

else if(@type="Q2" and @days < 91)
Begin
	set @first=4
	set @last=6
	set @currenYear=CONVERT(int,@archive_year)
end
else if(@type="Q3" and @days < 92)
Begin
	set @first=7
	set @last=9
	set @currenYear=CONVERT(int,@archive_year)
end

else if(@type="Q4" and @days < 92)
Begin
	set @first=10
	set @last=12
	set @currenYear=CONVERT(int,@archive_year)
end

else if(@type="JAN" and @days < 31)
Begin
		
	set @currenMonth=1
	set @currenYear=CONVERT(int,@archive_year)

end

else if(@type="FEB" and @days < 28)
Begin
	set @currenMonth=2
	set @currenYear=CONVERT(int,@archive_year)

end

else if(@type="MAR" and @days < 31)
Begin
	set @currenMonth=3
	set @currenYear=CONVERT(int,@archive_year)
	
end

else if(@type="APR" and @days < 30)
Begin
	set @currenMonth=4
	set @currenYear=CONVERT(int,@archive_year)
	
end

else if(@type="MAY" and @days < 31)
Begin
	set @currenMonth=5
	set @currenYear=CONVERT(int,@archive_year)
	
end

else if(@type="JUN" and @days < 30)
Begin
	set @currenMonth=6
	set @currenYear=CONVERT(int,@archive_year)

end

else if(@type="JUL" and @days < 31)
Begin
	set @currenMonth=7
	set @currenYear=CONVERT(int,@archive_year)

end

else if(@type="AUG" and @days < 31)
Begin
	set @currenMonth=8
	set @currenYear=CONVERT(int,@archive_year)
	
end
else if (@type="SEP" and @days < 30)
Begin
	set @currenMonth=9
	set @currenYear=CONVERT(int,@archive_year)
	
end
else if(@type="OCT" and @days < 31)
Begin
	set @currenMonth=10
	set @currenYear=CONVERT(int,@archive_year)

end
else if(@type="NOV" and @days < 30)
Begin
	set @currenMonth=11
	set @currenYear=CONVERT(int,@archive_year)

end
else if(@type="DEC" and @days < 31)
Begin
	set @currenMonth=12
	set @currenYear=CONVERT(int,@archive_year)

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

if (OBJECT_ID( '['+@archiveDbName+'].[dbo].[Events]' ) IS NULL )
begin
set @dbName=' Create table ['+@archiveDbName+'].[dbo].[Events] '
Set @sql=''
set @sql='
	(  	[startTime] [datetime] NOT NULL ,
          [checksum] [int] NOT NULL,
          [eventId] [int] NOT NULL ,
          [eventType] [int] NOT NULL ,
          [eventClass] [int] NOT NULL ,
          [eventSubclass] [int] NULL ,
          [spid] [int] NULL ,
          [applicationName] [nvarchar] (128) NULL ,
          [hostName] [nvarchar] (128) NULL ,
          [serverName] [nvarchar] (128) NULL ,
          [loginName] [nvarchar] (128) NULL ,
          [success] [int] NULL ,
          [databaseName] [nvarchar] (128) NULL ,
          [databaseId] [int] NULL ,
          [dbUserName] [nvarchar] (128) NULL ,
          [objectType] [int] NULL ,
          [objectName] [nvarchar] (512) NULL ,
          [objectId] [int] NULL ,
          [permissions] [int] NULL ,
          [columnPermissions] [int] NULL ,
          [targetLoginName] [nvarchar] (128) NULL ,
          [targetUserName] [nvarchar] (128) NULL ,
          [roleName] [nvarchar] (128) NULL ,
          [ownerName] [nvarchar] (128) NULL ,
          [targetObject] [nvarchar] (512) NULL ,  
          [details] [nvarchar] (512) NULL ,
          [eventCategory] [int] NOT NULL,
          [hash] [int] NOT NULL,
          [alertLevel] [int] NOT NULL,
          [privilegedUser] [int] NOT NULL,
          [fileName]         [nvarchar] (128) NULL ,
          [linkedServerName] [nvarchar] (128) NULL ,
          [parentName]       [nvarchar] (128) NULL ,
          [isSystem]         [int] NULL ,
          [sessionLoginName] [nvarchar] (128) NULL ,
          [providerName]     [nvarchar] (128) NULL, 
		  [appNameId] [int] NULL, 
		  [hostId] [int] NULL, 
		  [loginId] [int] NULL, 
		  [endTime] [datetime] NULL, 
		  [startSequence] [bigint] NULL,
		  [endSequence] [bigint] NULL,
		  [rowCounts] [bigint] NULL,
		  [guid] [nvarchar](100) NULL 
                             )'
	Set @sql=@dbName + char(13) + @sql
	exec (@sql)
end
begin transaction t1
begin try
set @todayDate = convert(varchar(35), getdate(), 120)


set @noOfDays=convert(varchar(35),DATEADD(DAY, -@days, getdate()), 120)

set @sql=''
set @dbName=''
set @serverName='[SQLcompliance_'+@InstanceDatabase+'].[dbo].[Events]'
set @sql='INSERT INTO ['+@archiveDbName+'].[dbo].[Events] '
set @sql=@sql + '([startTime],[checksum],[eventId],[eventType],[eventClass],
         [eventSubclass],[spid],[applicationName],[hostName],[serverName],
         [loginName],[success],[databaseName],[databaseId],[dbUserName],
         [objectType],[objectName],[objectId],[permissions],[columnPermissions],
         [targetLoginName],[targetUserName],[roleName],[ownerName],[targetObject],
         [details],[eventCategory],[hash],[alertLevel],[privilegedUser], 
         [fileName], [linkedServerName], [parentName], [isSystem], 
         [sessionLoginName], [providerName], [appNameId], [hostId], [loginId], 
         [endTime], [startSequence], [endSequence],[rowCounts],[guid]) SELECT [startTime],[checksum],[eventId],[eventType],[eventClass],
         [eventSubclass],[spid],[applicationName],[hostName],[serverName],
         [loginName],[success],[databaseName],[databaseId],[dbUserName],
         [objectType],[objectName],[objectId],[permissions],[columnPermissions],
         [targetLoginName],[targetUserName],[roleName],[ownerName],[targetObject],
         [details],[eventCategory],[hash],[alertLevel],[privilegedUser], 
         [fileName], [linkedServerName], [parentName], [isSystem], 
         [sessionLoginName], [providerName], [appNameId], [hostId], [loginId], 
         [endTime], [startSequence], [endSequence],[rowCounts],[guid] FROM '
set @type=UPPER(@type)
if(@type in ('JAN','FEB','MAR','APR','MAY','JUN','JUL','AUG','SEP','OCT','NOV','DEC'))
begin

		if(@currentMonth = @type)
		begin

		set @sql =@sql + @serverName +' where month(startTime)= '+CONVERT(varchar(10), @currenMonth) +' and Day(startTime)<= '+CONVERT(varchar(10), @remainDay) +' and Year(startTime) <= '+CONVERT(varchar(10), @currenYear)

		end
		else
		begin
		
		set @sql =@sql + @serverName +' where month(startTime) = '+CONVERT(varchar(10), @currenMonth) +' and Year( startTime )<= '+CONVERT(varchar(10), @currenYear)
		
		end
end
else if(@type in ('Q1','Q2','Q3','Q4'))
begin
		set @remianDays= ''''+CONVERT(varchar(11), cast (DATEADD(day, -@days, GETDATE()) AS datetime))+''''
		if(@quarter =@currentQuarter )
		begin

		set @sql =@sql + @serverName +' where month(startTime) >= '+CONVERT(varchar(10), @first) +' and month(startTime) <= '+CONVERT(varchar(10), @last) 
		set @sql =@sql +' and CONVERT(varchar(10), cast ([startTime] as datetime)) <=' 
		set @sql =@sql+ @remianDays +' and Year(startTime) <= '
		set @sql =@sql +CONVERT(varchar(10), @currenYear)
		

		end
		else
		begin

		set @sql =@sql + @serverName +' where month(startTime) >= '+CONVERT(varchar(10), @first) +' and month(startTime) <= '+CONVERT(varchar(10), @last) +' and Year( startTime ) <= '+CONVERT(varchar(10), @currenYear)
		
		
		end
end

else if(@type = 'YEAR')
	begin
	set @remianDays= ''''+CONVERT(varchar(11), cast (DATEADD(day, -@days, GETDATE()) AS datetime))+''''
	if(convert(varchar(10),@currenYear) = @currentYear)
		begin
		set @sql =@sql + @serverName +' where Year( startTime ) <= '+CONVERT(varchar(10), @currenYear)
		set @sql =@sql +' and CONVERT(varchar(10), cast ([startTime] as datetime)) <= '+ @remianDays
		
		end
	else
		begin
				set @sql =@sql + @serverName +' where Year( startTime ) <= '+CONVERT(varchar(10), @currenYear)
				
				exec(@sql)
		end
	end
exec (@sql)

if(@type in ('JAN','FEB','MAR','APR','MAY','JUN','JUL','AUG','SEP','OCT','NOV','DEC'))
begin
	if(@currentMonth = @type)
		begin
			set @sql=''
			set @sql ='DELETE FROM '+ '[SQLcompliance_'+@InstanceDatabase+'].[dbo].[Events] where month(startTime)= '+CONVERT(varchar(10), @currenMonth) +' and Day(startTime)<= '+CONVERT(varchar(10), @remainDay) +' and Year(startTime) <= '+CONVERT(varchar(10), @currenYear)
			exec(@sql)	
		end
	else
		begin
			set @sql=''
			set @sql ='DELETE FROM '+ '[SQLcompliance_'+@InstanceDatabase+'].[dbo].[Events] where month(startTime) = '+CONVERT(varchar(10), @currenMonth) +' and Year( startTime )<= '+CONVERT(varchar(10), @currenYear)
			exec(@sql)	
		end
	
end

else if(@type in ('Q1','Q2','Q3','Q4'))
begin
	set @remianDays= ''''+CONVERT(varchar(11), cast (DATEADD(day, -@days, GETDATE()) AS datetime))+''''
		if(@quarter =@currentQuarter )
		begin
			set @sql=''
			set @sql ='DELETE FROM '+ '[SQLcompliance_'+@InstanceDatabase+'].[dbo].[Events]'
			set @sql =@sql +' where month(startTime) >= '+CONVERT(varchar(10), @first) +' and month(startTime) <= '+CONVERT(varchar(10), @last) 
			set @sql =@sql +' and CONVERT(varchar(10), cast ([startTime] as datetime)) <=' 
			set @sql =@sql+ @remianDays +' and Year(startTime) <= '
			set @sql =@sql +CONVERT(varchar(10), @currenYear)
			

			exec(@sql)
		end
		else
		begin
			set @sql=''
			set @sql ='DELETE FROM '+ '[SQLcompliance_'+@InstanceDatabase+'].[dbo].[Events]'
			set @sql =@sql +' where month(startTime) >= '+CONVERT(varchar(10), @first) +' and month(startTime) <= '+CONVERT(varchar(10), @last) +' and Year( startTime ) <= '+CONVERT(varchar(10), @currenYear)
			
			exec(@sql)
		end
end

else if(@type = 'YEAR')
	begin
	set @remianDays= ''''+CONVERT(varchar(11), cast (DATEADD(day, -@days, GETDATE()) AS datetime))+''''
	if(convert(varchar(10),@currenYear) = @currentYear)
		begin
			set @sql=''
			set @sql ='DELETE FROM '+ '[SQLcompliance_'+@InstanceDatabase+'].[dbo].[Events]'
			set @sql =@sql +' where Year( startTime ) <= '+CONVERT(varchar(10), @currenYear)
			set @sql =@sql +' and CONVERT(varchar(10), cast ([startTime] as datetime)) <= '+ @remianDays
			
			exec(@sql)
		end
	else
	begin
			set @sql=''
			set @sql ='DELETE FROM '+ '[SQLcompliance_'+@InstanceDatabase+'].[dbo].[Events]'
			set @sql =@sql+' where Year( startTime ) <= '+CONVERT(varchar(10), @currenYear)
			
			exec(@sql)
	end
	end

	set @sql=''
	set @sql= 'UPDATE [SQLcompliance].[dbo].[Servers] set timeLastArchive ='''+convert(varchar(35), getdate(), 120)+''' where instance='''+@instanceName+''''
	exec(@sql)
	
	------------ UPDATE lowWatermark in [SQLcompliance].[dbo].[Servers] table ------------
	set @sql = ''
	set @sql = 'DECLARE @eventCount bigint '
	set @sql = @sql+'DECLARE @minEventId int '
	set @sql = @sql+'DECLARE @highWatermark int '
	set @sql = @sql+'SET @eventCount = (SELECT count(eventId) FROM [SQLcompliance_'+@InstanceDatabase+'].[dbo].[Events]) '
	set @sql = @sql+'SET @minEventId = (SELECT min(eventId) FROM  [SQLcompliance_'+@InstanceDatabase+'].[dbo].[Events] where eventId >= '
	set @sql = @sql+'					(SELECT lowWatermark from [SQLcompliance].[dbo].[Servers] '
	set @sql = @sql+'						where instance = '''+@instanceName+''')) '
	set @sql = @sql+'IF(@minEventId IS NULL AND @eventCount > 0) '
	set @sql = @sql+'	SET @minEventId = (SELECT min(eventId) FROM [SQLcompliance_'+@InstanceDatabase+'].[dbo].[Events]) '
	set @sql = @sql+'IF(@minEventId IS NULL) '
	set @sql = @sql+'BEGIN '
	set @sql = @sql+'SET @highWatermark = (SELECT highWatermark from [SQLcompliance].[dbo].[Servers] '
	set @sql = @sql+'						where instance = '''+@instanceName+''') '
	set @sql = @sql+'UPDATE SQLcompliance..Servers SET lowWatermark= @highWatermark ,highWatermark= @highWatermark,alertHighWatermark = @highWatermark WHERE instance = '''+@instanceName+''' '
	set @sql = @sql+'END '
	set @sql = @sql+'ELSE '
	set @sql = @sql+'UPDATE SQLcompliance..Servers SET lowWatermark = (@minEventId-1) WHERE instance = '''+@instanceName+''''
	exec(@sql)
	-------------------------------------------------------------------------------------
	

	set @db_id = DB_ID(@archiveDbName)
if not exists (select * from SQLcompliance.dbo.SystemDatabases where databaseName = @archiveDbName)
begin
	if(@type in ('JAN','FEB','MAR','APR','MAY','JUN','JUL','AUG','SEP','OCT','NOV','DEC'))
	BEGIN
		set @type=UPPER(LEFT(@type,1))+LOWER(SUBSTRING(@type,2,LEN(@type)))
		print @type
	END
	if(@type in ('Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'))
	BEGIN
		set @sql=''
		set @sql='INSERT INTO SQLcompliance.dbo.SystemDatabases (instance, databaseName, databaseType, dateCreated, sqlDatabaseId, displayName,description) 
		values('''+@instanceName+''','+''''+@archiveDbName+''', ''Archive'','''+convert(varchar(35), getdate(), 120)+''','''+ @db_id +''','''
		+@archive_year+'-'+@months+' ('+@type+')'','''+ @archive_year+' '+@type+' Events for SQL Server ' +@instanceName+''')'
		
		exec(@sql)
	end
	IF(@type in ('Q1','Q2','Q3','Q4'))
	BEGIN		
		set @sql=''
		set @sql='INSERT INTO SQLcompliance.dbo.SystemDatabases (instance, databaseName, databaseType, dateCreated, sqlDatabaseId, displayName,description) 
		values('''+@instanceName+''','+''''+@archiveDbName+''', ''Archive'','''+convert(varchar(35), getdate(), 120)+''','''+ @db_id +''','''
		+@archive_year+' '+@type+''','''+ @archive_year+' '+@type+' Events for SQL Server ' +@instanceName+''')'
		
		exec(@sql)
	END
	IF(@type in ('YEAR'))
	BEGIN		
		set @sql=''
		set @sql='INSERT INTO SQLcompliance.dbo.SystemDatabases (instance, databaseName, databaseType, dateCreated, sqlDatabaseId, displayName,description) 
		values('''+@instanceName+''','+''''+@archiveDbName+''', ''Archive'','''+convert(varchar(35), getdate(), 120)+''','''+ @db_id +''','''
		+@archive_year+''','''+ @archive_year+' '+@type+' Events for SQL Server ' +@instanceName+''')'
		
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

