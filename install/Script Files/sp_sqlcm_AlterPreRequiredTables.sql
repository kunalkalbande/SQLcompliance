USE [SQLcompliance]
-- add column to for Save and View Drop down list
IF NOT EXISTS (SELECT c.name from syscolumns as c where c.name='ViewName')
 BEGIN
  ALTER TABLE ViewSettings ADD [ViewName] [nvarchar](256) NULL;
 END


IF Exists (select name from Databases where name='SQLcomplianceProcessing' OR name='SQLcompliance.Processing')
BEGIN
update Databases SET name='SQLcomplianceProcessing' where name='SQLcomplianceProcessing' OR name='SQLcompliance.Processing'
END 
IF Exists (select databaseName from SystemDatabases where databaseName='SQLcomplianceProcessing' OR databaseName='SQLcompliance.Processing')
BEGIN
update SystemDatabases SET databaseName='SQLcomplianceProcessing' where databaseName='SQLcomplianceProcessing' OR databaseName='SQLcompliance.Processing'
END


--------------------------------------------------------------------------------------------------------------
	--5.1 Changes
--------------------------------------------------------------------------------------------------------------

	IF (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Servers' AND COLUMN_NAME IN ('instancePort')) <> 1
		BEGIN
			ALTER TABLE [SQLcompliance]..[Servers] 
			ADD	[instancePort] [int] NULL;
		END
	
	
	IF (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Servers' AND COLUMN_NAME IN ('versionName')) <> 1
	BEGIN
		ALTER TABLE [SQLcompliance]..[Servers] 
		ADD	[versionName] [nvarchar](100) NULL;
	END
	
IF (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Servers' AND COLUMN_NAME IN ('isCluster')) <> 1
BEGIN
	ALTER TABLE [SQLcompliance]..[Servers] 
	ADD	[isCluster] [tinyint] NULL;
END
IF (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Servers' AND COLUMN_NAME IN ('isHadrEnabled')) <> 1
BEGIN
	ALTER TABLE [SQLcompliance]..[Servers] 
	ADD	[isHadrEnabled] [tinyint] NULL;
END

IF (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Servers' AND COLUMN_NAME IN ('auditUserExtendedEvents')) <> 1
BEGIN
	ALTER TABLE [SQLcompliance]..[Servers] 
	ADD	[auditUserExtendedEvents] [tinyint] NULL;
END

IF (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Servers' AND COLUMN_NAME IN ('auditCaptureSQLXE')) <> 1
BEGIN
	ALTER TABLE [SQLcompliance]..[Servers] 
	ADD	[auditCaptureSQLXE] [tinyint] NULL;
END

--5.5 Added 'isAuditLogEnabled' column for Audit Logs
IF (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Servers' AND COLUMN_NAME IN ('isAuditLogEnabled')) <> 1
BEGIN
	ALTER TABLE [SQLcompliance]..[Servers] 
	ADD	[isAuditLogEnabled] [tinyint] NULL;
END

IF NOT EXISTS (SELECT c.name,c.id FROM syscolumns AS c,sysobjects AS o WHERE c.name='isDismissed' and o.name='Alerts' and c.id=o.id)
BEGIN
	ALTER TABLE Alerts ADD [isDismissed] [bit] NULL;	
END
	
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ViewSettings')
BEGIN
CREATE TABLE [dbo].[ViewSettings](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](256) NOT NULL,
	[ViewId] [nvarchar](256) NULL,
	[Timeout] [int] NULL,
	[Filter] [nvarchar](MAX) NULL,
 CONSTRAINT [PK_ViewSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

       IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserSettings')
       BEGIN
		CREATE TABLE [dbo].[UserSettings](
			[DashboardUserId] [int] NOT NULL,
			[Account] [nvarchar](255) NOT NULL,
			[Email] [nvarchar](230) NULL,
			[SessionTimout] [int] NULL,
			[Subscribed] [bit] NOT NULL DEFAULT(0)
		) ON [PRIMARY]
		END
	  
	IF NOT EXISTS (SELECT c.name,c.id FROM syscolumns AS c,sysobjects AS o WHERE c.name='authentication_type' and o.name='Servers' and c.id=o.id)
	BEGIN
		ALTER TABLE Servers ADD [authentication_type] int not null default(0);	
	end

	IF NOT EXISTS (SELECT c.name,c.id FROM syscolumns AS c,sysobjects AS o WHERE c.name='user_account' and o.name='Servers' and c.id=o.id)
	BEGIN
		ALTER TABLE Servers ADD [user_account] nvarchar(128) null;	
	end

	IF NOT EXISTS (SELECT c.name,c.id FROM syscolumns AS c,sysobjects AS o WHERE c.name='password' and o.name='Servers' and c.id=o.id)
	BEGIN
		ALTER TABLE Servers ADD [password] nvarchar(256) null;	
	END
	
	IF NOT EXISTS (SELECT c.name,c.id FROM syscolumns AS c,sysobjects AS o WHERE c.name='owner' and o.name='Servers' and c.id=o.id)
	BEGIN
		ALTER TABLE Servers ADD [owner] [nvarchar](256) NULL;	
	end

	IF NOT EXISTS (SELECT c.name,c.id FROM syscolumns AS c,sysobjects AS o WHERE c.name='location' and o.name='Servers' and c.id=o.id)
	BEGIN	
		ALTER TABLE Servers ADD [location] [nvarchar](256) NULL;	
	end

	IF NOT EXISTS (SELECT c.name,c.id FROM syscolumns AS c,sysobjects AS o WHERE c.name='comments' and o.name='Servers' and c.id=o.id)
	BEGIN	
		ALTER TABLE Servers ADD [comments] [nvarchar](max) NULL;	
	end
	  
	IF NOT EXISTS (SELECT c.name,c.id FROM syscolumns AS c,sysobjects AS o WHERE c.name='auditUserCaptureDDL' and o.name='Servers' and c.id=o.id)
	BEGIN	
		ALTER TABLE Servers ADD [auditUserCaptureDDL] [tinyint] NULL;	
	end

	IF NOT EXISTS (SELECT c.name,c.id FROM syscolumns AS c,sysobjects AS o WHERE c.name='auditUserCaptureDDL' and o.name='Databases' and c.id=o.id)
	BEGIN	
		ALTER TABLE Databases ADD [auditUserCaptureDDL] [tinyint] NULL;	
	end

	IF NOT EXISTS (SELECT c.name,c.id FROM syscolumns AS c,sysobjects AS o WHERE c.name='auditCaptureDDL' and o.name='Databases' and c.id=o.id)
	BEGIN	
		ALTER TABLE Databases ADD [auditCaptureDDL] [tinyint] NULL;	
	end
	  

-------------------------------------------------INSATANCE SYNCHRONIZATION-------------------------------------------------
if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='isSynchronized' and o.name='Servers' and c.id=o.id)
BEGIN
	ALTER TABLE [SQLcompliance]..[Servers] ADD [isSynchronized] [bit] NOT NULL DEFAULT (0);
END
		
-------------------------------------------------INSATANCE SYNCHRONIZATION-------------------------------------------------
EXEC('
UPDATE [SQLcompliance]..[Databases] set [availGroupName] = '''' WHERE [availGroupName] IS NULL
UPDATE [SQLcompliance]..[Databases] set [auditUsersList] = '''' WHERE [auditUsersList] IS NULL
UPDATE [SQLcompliance]..[Databases] set [auditCaptureTrans] = 0 WHERE [auditCaptureTrans] IS NULL  
UPDATE [SQLcompliance]..[Databases] set [auditPrivUsersList] = '''' WHERE [auditPrivUsersList] IS NULL
UPDATE [SQLcompliance]..[Databases] set [auditUserAll] = 0 WHERE [auditUserAll] IS NULL
UPDATE [SQLcompliance]..[Databases] set [auditUserLogins] = 1 WHERE [auditUserLogins] IS NULL
UPDATE [SQLcompliance]..[Databases] set [auditUserFailedLogins] = 1 WHERE [auditUserFailedLogins] IS NULL
UPDATE [SQLcompliance]..[Databases] set [auditUserDDL] = 1 WHERE [auditUserDDL] IS NULL
UPDATE [SQLcompliance]..[Databases] set [auditUserSecurity] = 1 WHERE [auditUserSecurity] IS NULL
UPDATE [SQLcompliance]..[Databases] set [auditUserAdmin] = 1 WHERE [auditUserAdmin] IS NULL
UPDATE [SQLcompliance]..[Databases] set [auditUserDML] = 0 WHERE [auditUserDML] IS NULL 
UPDATE [SQLcompliance]..[Databases] set [auditUserSELECT] = 0 WHERE [auditUserSELECT] IS NULL 
UPDATE [SQLcompliance]..[Databases] set [auditUserFailures] = 1 WHERE [auditUserFailures] IS NULL 
UPDATE [SQLcompliance]..[Databases] set [auditUserCaptureSQL] = 0 WHERE [auditUserCaptureSQL] IS NULL 
UPDATE [SQLcompliance]..[Databases] set [auditUserCaptureTrans] = 0 WHERE [auditUserCaptureTrans] IS NULL
UPDATE [SQLcompliance]..[Databases] set [auditUserExceptions] = 0 WHERE [auditUserExceptions] IS NULL
UPDATE [SQLcompliance]..[Databases] set [auditUserUDE] = 0 WHERE [auditUserUDE] IS NULL')

GO 


-- END add column script for drop down lists