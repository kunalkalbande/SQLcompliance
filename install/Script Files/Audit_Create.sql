-- This script is only for reference purposes
-- The actual code to do this is in the product itself.
--
CREATE DATABASE [REPLACEME_WITH_EVENTSDATABASE]
GO

ALTER DATABASE [REPLACEME_WITH_EVENTSDATABASE] SET RECOVERY SIMPLE
GO


USE [REPLACEME_WITH_EVENTSDATABASE]
GO

if exists (select * from dbo.sysobjects where name = 'Events' and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [Events] 
GO

CREATE TABLE [Events] (
	[startTime] [datetime] NOT NULL ,
	[checksum] [int] NOT NULL ,
	[eventId] [int] UNIQUE NOT NULL ,
	[eventType] [int] NULL ,
	[eventClass] [int] NULL ,
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
	[objectName] [nvarchar] (128) NULL ,
	[objectId] [int] NULL ,
	[permissions] [int] NULL ,
	[columnPermissions] [int] NULL ,
	[targetLoginName] [nvarchar] (128) NULL ,
	[targetUserName] [nvarchar] (128) NULL ,
	[roleName] [nvarchar] (128) NULL ,
	[ownerName] [nvarchar] (128) NULL ,
	[targetObject] [nvarchar] (512) NULL ,
	[details] [nvarchar] (512) NULL ,
	[eventCategory] [int] NOT NULL ,
	[hash] [int] NULL ,
	[alertLevel] [int] NULL ,
	[privilegedUser] [int] NULL,
	[fileName]         [nvarchar] (128) NULL ,
	[linkedServerName] [nvarchar] (128) NULL ,
	[parentName]       [nvarchar] (128) NULL ,
	[isSystem]         [int] NULL ,
	[sessionLoginName] [nvarchar] (128) NULL ,
	[providerName]     [nvarchar] (128) NULL,
    [appNameId]        [int] Null,
    [hostId]           [int] null,
    [loginId]          [int] null,
  endTime datetime null,
    startSequence bigint null, 
    endSequence bigint null
)
GO

CREATE UNIQUE CLUSTERED INDEX [IX_Events_eventId] ON [dbo].[Events]([eventId] DESC ) ON [PRIMARY]
GO

CREATE  INDEX [IX_Events_StartTime] ON [dbo].[Events]([startTime] DESC , [eventId] DESC ) ON [PRIMARY]
GO

CREATE  INDEX [IX_Events_eventCategory] ON [dbo].[Events]([eventCategory], [eventId] DESC ) ON [PRIMARY]
GO

CREATE  INDEX [IX_Events_eventType] ON [dbo].[Events]([eventType], [eventId] DESC ) ON [PRIMARY]
GO

CREATE  INDEX [IX_Events_databaseId] ON [dbo].[Events]([databaseId], [eventId] DESC ) ON [PRIMARY]
GO

CREATE  INDEX [IX_Events_privilegedUser] ON [dbo].[Events]([privilegedUser], [eventId] DESC ) ON [PRIMARY]
GO

CREATE  INDEX [IX_Events_appNameId] ON [dbo].[Events]([appNameId], [eventId] DESC ) ON [PRIMARY]
GO

CREATE  INDEX [IX_Events_hostId] ON [dbo].[Events]([hostId], [eventId] DESC ) ON [PRIMARY]
GO

CREATE  INDEX [IX_Events_loginId] ON [dbo].[Events]([loginId], [eventId] DESC ) ON [PRIMARY]
GO


if exists (select * from dbo.sysobjects where name = 'EventSQL' and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [EventSQL] 
GO

CREATE TABLE [EventSQL] (
    [eventId] [int] NOT NULL,
	[startTime] [datetime] NOT NULL ,
	[sqlText] [nvarchar](MAX) NOT NULL ,
	[hash] [int] NOT NULL DEFAULT (0)
)
GO

CREATE TABLE [Description] (
   [instance]     [nvarchar] (256) NULL,
   [databaseType] [nvarchar] (16)  NULL ,
   [state] [int] NULL,
   [sqlComplianceDbSchemaVersion] [int] NULL,
   [eventDbSchemaVersion] [int] NULL
)
GO

INSERT INTO [Description]
   ([instance],[databaseType],[sqlComplianceDbSchemaVersion],[eventDbSchemaVersion],[state])
VALUES ('REPLACEME_WITH_INSTANCE','Event',401,303,0)
GO

CREATE TABLE [Stats] ( 
   [dbId] [int] NOT NULL DEFAULT 0, 
   [category] [int] NOT NULL, 
   [date] [DateTime] NOT NULL, 
   [count] [int] NOT NULL DEFAULT 0, 
   [lastUpdated] [DateTime] NOT NULL,
   PRIMARY KEY CLUSTERED ( [dbId], [date] DESC, [category] ) ) ON [PRIMARY]
GO

CREATE TABLE [Applications] ( [name] [nvarchar] (128) NOT NULL, [id] [int] NOT NULL ) ON [PRIMARY]
GO

CREATE TABLE [Hosts] ( [name] [nvarchar] (128) NOT NULL, [id] [int] NOT NULL ) ON [PRIMARY]
GO

CREATE TABLE [Logins] ( [name] [nvarchar] (128) NOT NULL, [id] [int] NOT NULL ) ON [PRIMARY]
GO

-- New tables for each modified record
CREATE TABLE DataChanges (  startTime datetime not null,
						    eventSequence bigint not null,
						    spid int not null,
							databaseId int not null,
							actionType int not null,
                            schemaName nvarchar(128) not null,
							tableName nvarchar(	128) not null,
							recordNumber int not null,
							userName nvarchar(40) not null,
							changedColumns int not null,
							primaryKey nVarchar(4000),
							hashcode int not null ,
					    tableId int null,
            		dcId int IDENTITY (-2100000000, 1) NOT NULL,
            		eventId int null,
							primary key clustered (startTime, eventSequence, spid)  ) 
	on [PRIMARY]
go

-- The table that contains the before/after values.  One record is one column.
DECLARE @stmt nvarchar(2000)
SELECT @stmt = CASE WHEN SERVERPROPERTY('productversion') > '9' -- SQL 2005 or later
			THEN 
				'CREATE TABLE ColumnChanges ( startTime datetime not null,
							 eventSequence bigint not null,
							 spid int not null,
							 columnName nVarchar(128 )not null,
							 beforeValue nVarchar(max),
							 afterValue nVarchar(max),
							 hashcode int not null,
							 columnId int null, 
                    	 dcId int null ) 
	on [PRIMARY]'

            ELSE
				'CREATE TABLE ColumnChanges ( startTime datetime not null,
							 eventSequence bigint not null,
							 spid int not null,
							 columnName nVarchar(128 )not null,
							 beforeValue nVarchar(4000),
							 afterValue nVarchar(4000),
							 hashcode int not null,
							 columnId int null, 
                    	 dcId int null ) 
	on [PRIMARY]'
END
EXEC( @stmt)
GO

CREATE TABLE [Tables] ( [schemaName] nvarchar(128) NOT NULL, 
                        [name] [nvarchar] (128) NOT NULL, 
                        [id] [int] NOT NULL ) ON [PRIMARY]
GO

CREATE TABLE [Columns] ( [name] [nvarchar] (128) NOT NULL, [id] [int] NOT NULL ) ON [PRIMARY]
GO


CREATE  INDEX [IX_ColumnChanges_FK] ON ColumnChanges(startTime, eventSequence, spid ) ON [PRIMARY]
GO


EXECUTE sp_grantdbaccess 'guest'
GO

GRANT SELECT ON [dbo].[Events] TO [guest]
GO

GRANT SELECT ON [dbo].[EventSQL] TO [guest]
GO

GRANT SELECT ON [dbo].[Description] TO [guest]
GO

GRANT SELECT ON [dbo].[Stats] TO [guest]
GO

GRANT SELECT ON [dbo].[Applications] TO [guest]
GO

GRANT SELECT ON [dbo].[Hosts] TO [guest]
GO

GRANT SELECT ON [dbo].[Logins] TO [guest]
GO

GRANT SELECT ON [dbo].[DataChanges] TO [guest]
GO

GRANT SELECT ON [dbo].[ColumnChanges] TO [guest]
GO

GRANT SELECT ON [dbo].[Tables] TO [guest]
GO

GRANT SELECT ON [dbo].[Columns] TO [guest]
GO



