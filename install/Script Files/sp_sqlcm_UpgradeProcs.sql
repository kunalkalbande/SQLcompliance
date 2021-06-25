---------- SP_SQLCM_UPGRADEPROCS.SQL
-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/252007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

-- Drop procedure before creation to handle upgrade case
IF (OBJECT_ID('sp_sqlcm_UpgradeRepository') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_UpgradeRepository
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_UpgradeRepository]
as
BEGIN
	SET QUOTED_IDENTIFIER OFF
	--------------------------------------------------------------------------------------------------------------
	-- 1.2 Changes
	--------------------------------------------------------------------------------------------------------------
	-- add column to hold reportingVersion if it doesnt already exist
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='reportingVersion' and o.name='Configuration' and c.id=o.id)
		ALTER TABLE Configuration ADD [reportingVersion] [int] NULL DEFAULT (100);
	
	-- add column to hold repositoryVersion if it doesnt already exist
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='repositoryVersion' and o.name='Configuration' and c.id=o.id)
		ALTER TABLE Configuration ADD [repositoryVersion] [int] NULL DEFAULT (100);
	
	-- START SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='auditLogouts' and o.name='Configuration' and c.id=o.id)
	begin
		ALTER TABLE Configuration ADD [auditLogouts] [tinyint] NULL  DEFAULT (0);
	    -- Use auditLogins value for auditLogouts for upgrade scenario
	    exec sp_executesql N'UPDATE [SQLcompliance]..[Configuration] SET [auditLogouts] = [auditLogins]'
	end
		
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='auditUserLogouts' and o.name='Configuration' and c.id=o.id)
	begin
		ALTER TABLE Configuration ADD [auditUserLogouts] [tinyint] NULL  DEFAULT (0);
		-- Use auditLogins value for auditUserLogouts for upgrade scenario
	    exec sp_executesql N'UPDATE [SQLcompliance]..[Configuration] SET [auditUserLogouts] = [auditUserLogins]'
	end
	-- STOP SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events

	-- Update Change Log Event Type Table
	if not exists (select Name from ChangeLogEventTypes where eventId=50 )
	BEGIN
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (50,'Attach Archive');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (51,'Detach Archive');
	END
		
	--------------------------------------------------------------------------------------------------------------
	-- 2.0  Changes
	--------------------------------------------------------------------------------------------------------------
	--SQLcomplianceProcessing
	---------------------------------------------------------------------------------------------------
	IF OBJECT_ID('[SQLcomplianceProcessing]..[TraceLogins]') IS NULL
		CREATE TABLE [SQLcomplianceProcessing]..[TraceLogins] (
			[instance] [nvarchar] (128) NULL ,
			[loginKey] [int] NULL ,
			[loginValue] [datetime] NULL
		) ON [PRIMARY];
		
	IF NOT EXISTS (SELECT c.name,c.id from [SQLcomplianceProcessing]..syscolumns as c,[SQLcomplianceProcessing]..sysobjects as o where c.name='FileName' and o.name='TraceStates' and c.id=o.id)		
		ALTER TABLE [SQLcomplianceProcessing]..[TraceStates] ADD
			[FileName]         [nvarchar] (128) NULL ,
			[LinkedServerName] [nvarchar] (128) NULL ,
			[ParentName]       [nvarchar] (128) NULL ,
			[IsSystem]         [int] NULL ,
			[SessionLoginName] [nvarchar] (128) NULL ,
			[ProviderName]     [nvarchar] (128) NULL;

	--SQLcompliance
	---------------------------------------------------------------------------------------------------
	
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='auditBroker' and o.name='Databases' and c.id=o.id)		
		ALTER TABLE [SQLcompliance]..[Databases] ADD
			[auditBroker] [tinyint] NULL DEFAULT (0),
			[auditLogins] [tinyint] NULL DEFAULT (1);

	-- 	START SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='auditLogouts' and o.name='Servers' and c.id=o.id)		
	BEGIN
		ALTER TABLE [SQLcompliance]..[Servers] ADD
			[auditLogouts] [tinyint] NULL DEFAULT (0);
	    -- Use auditLogins value for auditLogouts for upgrade scenario
	    exec sp_executesql N'UPDATE [SQLcompliance]..[Servers] SET [auditLogouts] = [auditLogins]'
	END

	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='auditUserLogouts' and o.name='Servers' and c.id=o.id)		
	BEGIN
		ALTER TABLE [SQLcompliance]..[Servers] ADD
			[auditUserLogouts] [tinyint] NULL DEFAULT (0);
	    -- Use auditLogins value for auditLogouts for upgrade scenario
	    exec sp_executesql N'UPDATE [SQLcompliance]..[Servers] SET [auditUserLogouts] = [auditUserLogins]'
	END
	
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='auditLogouts' and o.name='Databases' and c.id=o.id)		
	BEGIN
		ALTER TABLE [SQLcompliance]..[Databases] ADD
			[auditLogouts] [tinyint] NULL DEFAULT (0);
	    -- Use auditLogins value for auditLogouts for upgrade scenario
	    exec sp_executesql N'UPDATE [SQLcompliance]..[Databases] SET [auditLogouts] = [auditLogins]'
	END

	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='auditUserLogouts' and o.name='Databases' and c.id=o.id)		
	BEGIN
		ALTER TABLE [SQLcompliance]..[Databases] ADD
			[auditUserLogouts] [tinyint] NULL DEFAULT (0);
	    -- Use auditLogins value for auditLogouts for upgrade scenario
	    exec sp_executesql N'UPDATE [SQLcompliance]..[Databases] SET [auditUserLogouts] = [auditUserLogins]'
	END
	-- 	STOP SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
	
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='is2005Only' and o.name='EventTypes' and c.id=o.id)
	BEGIN
		ALTER TABLE [SQLcompliance]..[EventTypes] ADD
			[is2005Only] [tinyint] NULL DEFAULT (0),
			[isExcludable] [tinyint] NULL DEFAULT (0);

		EXEC('UPDATE [SQLcompliance]..[EventTypes] SET is2005Only=0');
	END

	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='alertHighWatermark' and o.name='Servers' and c.id=o.id)
	BEGIN
		ALTER TABLE [SQLcompliance]..[Servers] ADD
		  [alertHighWatermark] [int]  DEFAULT (-2100000000) NULL,
			[auditDBCC] [tinyint] NULL DEFAULT(0),
			[auditSystemEvents] [tinyint] NULL DEFAULT (0);
		
		EXEC('UPDATE [SQLcompliance]..[Servers] SET [alertHighWatermark]=[highWatermark]');
	END

	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='auditDBCC' and o.name='Configuration' and c.id=o.id)
		ALTER TABLE [SQLcompliance]..[Configuration] ADD
			[auditDBCC] [tinyint] NULL DEFAULT(0),
			[auditSystemEvents] [tinyint] NULL DEFAULT (0),
			[smtpServer] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			[smtpPort] [int] NULL ,
			[smtpAuthType] [int] NULL ,
			[smtpSsl] [tinyint] NULL ,
			[smtpUsername] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			[smtpPassword] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			[smtpSenderAddress] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			[smtpSenderName] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
		  [loginCollapse] [tinyint] NULL DEFAULT(0) ,
		  [loginTimespan] [int] NULL DEFAULT (60) ,
		  [loginCacheSize] [int] NULL DEFAULT (1000);

	-- Alerting Support
	IF OBJECT_ID('ActionResultStatusTypes') IS NULL
	BEGIN
		CREATE TABLE [SQLcompliance]..[ActionResultStatusTypes] (
			[statusTypeId] [int] NOT NULL ,
			[name] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			CONSTRAINT [PK_ActionResultStatusTypes] PRIMARY KEY  CLUSTERED 
			(
				[statusTypeId]
			)  ON [PRIMARY] 
		) ON [PRIMARY];		
		GRANT SELECT ON [ActionResultStatusTypes] TO [public];
		INSERT INTO ActionResultStatusTypes (statusTypeId, name) VALUES (0, 'Success'); 
		INSERT INTO ActionResultStatusTypes (statusTypeId, name) VALUES (1, 'Failure') ;
		INSERT INTO ActionResultStatusTypes (statusTypeId, name) VALUES (2, 'Pending') ;
		INSERT INTO ActionResultStatusTypes (statusTypeId, name) VALUES (3, 'Uninitialized');
	END

	IF OBJECT_ID('AlertTypes') IS NULL
	BEGIN
		CREATE TABLE [SQLcompliance]..[AlertTypes] (
			[alertTypeId] [int] NOT NULL ,
			[name] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			CONSTRAINT [PK_AlertTypes] PRIMARY KEY  CLUSTERED 
			(
				[alertTypeId]
			)  ON [PRIMARY] 
		) ON [PRIMARY];
		GRANT SELECT ON [AlertTypes] TO [public];
		INSERT INTO AlertTypes (alertTypeId, name) VALUES (1, 'Audited SQL Server');
	END
	
	IF OBJECT_ID('Alerts') IS NULL
	BEGIN
		CREATE TABLE [SQLcompliance]..[Alerts] (
			[alertId] [int] IDENTITY (-2100000000, 1) NOT NULL ,
			[alertType] [int] NOT NULL ,
			[alertRuleId] [int] NOT NULL ,
			[alertEventId] [int] NOT NULL ,
			[instance] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			[eventType] [int] NOT NULL ,
			[created] [datetime] NOT NULL CONSTRAINT [DF_Alerts_created] DEFAULT (getutcdate()),
			[alertLevel] [tinyint] NOT NULL ,
			[emailStatus] [tinyint] NOT NULL,
			[logStatus] [tinyint] NOT NULL,
			[message]  [nvarchar] (3000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			[ruleName] [nvarchar] (60) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			CONSTRAINT [PK_Alerts] PRIMARY KEY  CLUSTERED 
			(
				[alertId]
			)  ON [PRIMARY] ,
			CONSTRAINT [FK_Alerts_AlertTypes] FOREIGN KEY 
			(
				[alertType]
			) REFERENCES [SQLcompliance]..[AlertTypes] (
				[alertTypeId]
			)
		) ON [PRIMARY];
		GRANT SELECT ON [Alerts] TO [public];
		
		CREATE  INDEX [IX_Alerts_created] ON [dbo].[Alerts]([created] DESC, [alertId] DESC ) ON [PRIMARY];
		CREATE  INDEX [IX_Alerts_alertLevel] ON [dbo].[Alerts]([alertLevel], [alertId] DESC ) ON [PRIMARY];
		CREATE  INDEX [IX_Alerts_eventType] ON [dbo].[Alerts]([eventType], [alertId] DESC ) ON [PRIMARY];
	END
		
	IF OBJECT_ID('AlertRules') IS NULL
	BEGIN
		CREATE TABLE [SQLcompliance]..[AlertRules] (
			[ruleId] [int] IDENTITY (-2100000000, 1) NOT NULL ,
			[name] [nvarchar] (60) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			[description] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			[alertLevel] [tinyint] NOT NULL ,
			[alertType] [int] NOT NULL ,
			[targetInstances] [nvarchar] (640) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			[enabled] [tinyint] NOT NULL ,
			[message] [nvarchar] (2500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			[logMessage] [tinyint] NOT NULL,
			[emailMessage] [tinyint] NOT NULL,
			CONSTRAINT [PK_AlertRules] PRIMARY KEY  CLUSTERED 
			(
				[ruleId]
			)  ON [PRIMARY] ,
			CONSTRAINT [FK_AlertRules_AlertTypes] FOREIGN KEY 
			(
				[alertType]
			) REFERENCES [SQLcompliance]..[AlertTypes] (
				[alertTypeId]
			)
		) ON [PRIMARY];
		GRANT SELECT ON [AlertRules] TO [public];
	END

	IF OBJECT_ID('AlertRuleConditions') IS NULL
	BEGIN
		CREATE TABLE [SQLcompliance]..[AlertRuleConditions] (
			[conditionId] [int] IDENTITY (-2100000000, 1) NOT NULL ,
			[ruleId] [int] NOT NULL ,
			[fieldId] [int] NOT NULL ,
			[matchString] [nvarchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			CONSTRAINT [PK_AlertRuleConditions] PRIMARY KEY  CLUSTERED 
			(
				[conditionId]
			)  ON [PRIMARY] ,
			CONSTRAINT [FK_AlertRuleConditions_AlertRules] FOREIGN KEY 
			(
				[ruleId]
			) REFERENCES [SQLcompliance]..[AlertRules] (
				[ruleId]
			)
		) ON [PRIMARY];
		GRANT SELECT ON [AlertRuleConditions] TO [public];
	END
	
	IF NOT EXISTS(select Name from ChangeLogEventTypes where eventId=52 )
	BEGIN
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (52,'Login Filtering Changed');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (53,'Alert Rule Added');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (54,'Alert Rule Removed');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (55,'Alert Rule Modified');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (56,'Alert Rule Disabled');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (57,'Alert Rule Enabled');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (58,'Groom Alerts');
	END

	IF NOT EXISTS(select Name from AgentEventTypes where eventId=17 )
	BEGIN
		INSERT INTO [AgentEventTypes] ([eventId],[Name])	VALUES (17,'Incompatible SQL Server version error');
	END


	IF NOT EXISTS(select name from ObjectTypes where objtypeId=8259 )
	BEGIN
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8259,'CHECK');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8260,'DEFAULT');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8262,'FOREIGN KEY');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8272,'STORED PROCEDURE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8274,'RULE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8275,'SYSTEM TABLE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8276,'SERVER TRIGGER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8277,'USER TABLE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8278,'VIEW');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8280,'EXTENDED SP');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (16724,'CLR TRIGGER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (16964,'DATABASE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (16975,'OBJECT');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17222,'FULL TEXT CATALOG');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17232,'CLR STORED PROCEDURE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17235,'SCHEMA');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17475,'CREDENTIAL');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17491,'DDL EVENT');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17741,'MGMT EVENT');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17747,'SECURITY EVENT');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17749,'USER EVENT');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17985,'CLR AGGREGATE FUNCTION');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17993,'INLINE FUNCTION');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (18000,'PARTITION FUNCTION');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (18002,'REPL FILTER PROC');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (18004,'TABLE VALUED UDF');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (18259,'SERVER ROLE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (18263,'MICROSOFT WINDOWS GROUP');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19265,'ASYMMETRIC KEY');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19277,'MASTER KEY');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19280,'PRIMARY KEY');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19283,'OBFUS KEY');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19521,'ASYMMETRIC KEY LOGIN');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19523,'CERTIFICATE LOGIN');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19538,'ROLE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19539,'SQL LOGIN');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19543,'WINDOWS LOGIN');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20034,'REMOTE SERVICE BINDING');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20036,'EVENT NOTIFICATION DATABASE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20037,'EVENT NOTIFICATION');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20038,'SCALAR FUNCTION');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20047,'EVENT NOTIFICATION OBJECT');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20051,'SYNONYM');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20549,'ENDPOINT');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20801,'CACHED ADHOC QUERIES');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20816,'CACHED ADHOC QUERIES');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20819,'SERVICE BROKER QUEUE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20821,'UNIQUE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21057,'APPLICATION ROLE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21059,'CERTIFICATE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21075,'SERVER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21076,'TSQL TRIGGER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21313,'ASSEMBLY');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21318,'CLR SCALAR FUNCTION');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21321,'INLINE SCALAR FUNCTION');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21328,'PARTITION SCHEME');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21333,'USER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21571,'SERVICE BROKER CONTRACT');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21572,'DATABASE TRIGGER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21574,'CLR TABLE FUNCTION');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21577,'INTERNAL TABLE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21581,'SERVICE BROKER MSG TYPE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21586,'SERVICE BROKER ROUTE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21587,'STATISTICS');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21825,'USER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21827,'USER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21831,'USER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21843,'USER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21847,'USER');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (22099,'SERVICE BROKER SERVICE');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (22601,'INDEX');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (22604,'CERTIFICATE LOGIN');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (22611,'XML SCHEMA');
		INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (22868,'TYPE');
	END
	
	--------------------------------------------------------------------------------------------------------------
	-- 2.1 Changes
	--------------------------------------------------------------------------------------------------------------
	-- SQLcompliance database modifications
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='auditUDE' and o.name='Servers' and c.id=o.id)
		ALTER TABLE [SQLcompliance]..[Servers] ADD
			[auditUDE] [tinyint] NULL DEFAULT (0),
			[auditUserUDE] [tinyint] NULL DEFAULT (0),
			[agentDetectionInterval] [int] NULL DEFAULT (60);

	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='archiveCheckIntegrity' and o.name='Configuration' and c.id=o.id)
		ALTER TABLE [SQLcompliance]..[Configuration] ADD
			[archiveCheckIntegrity] [tinyint] NOT NULL DEFAULT (1);

	-- Event Filter Support
	IF OBJECT_ID('EventFilters') IS NULL
	BEGIN
		CREATE TABLE [SQLcompliance]..[EventFilters] (
			[filterId] [int] IDENTITY (-2100000000, 1) NOT NULL ,
			[name] [nvarchar] (60) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			[description] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			[eventType] [int] NOT NULL ,
			[targetInstances] [nvarchar] (640) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			[enabled] [tinyint] NOT NULL ,
			CONSTRAINT [PK_EventFilters] PRIMARY KEY  CLUSTERED 
			(
				[filterId]
			)  ON [PRIMARY] ,
		) ON [PRIMARY];
		GRANT SELECT ON [EventFilters] TO [public]
	END

	IF OBJECT_ID('EventFilterConditions') IS NULL
	BEGIN
		CREATE TABLE [SQLcompliance]..[EventFilterConditions] (
			[conditionId] [int] IDENTITY (-2100000000, 1) NOT NULL ,
			[filterId] [int] NOT NULL ,
			[fieldId] [int] NOT NULL ,
			[matchString] [nvarchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			CONSTRAINT [PK_EventFilterConditions] PRIMARY KEY  CLUSTERED 
			(
				[conditionId]
			)  ON [PRIMARY] ,
			CONSTRAINT [FK_EventFilterConditions_EventFilters] FOREIGN KEY 
			(
				[filterId]
			) REFERENCES [dbo].[EventFilters] (
				[filterId]
			)
		) ON [PRIMARY];
		GRANT SELECT ON [EventFilterConditions] TO [public]
	END
	
	IF NOT EXISTS(select Name from ChangeLogEventTypes where eventId=59 )
	BEGIN
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (59,'Event Filter Added');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (60,'Event Filter Removed');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (61,'Event Filter Modified');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (62,'Event Filter Disabled');
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (63,'Event Filter Enabled');
	END

	IF NOT EXISTS(select Name from AgentEventTypes where eventId=18 )
	BEGIN
		INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (18,'Audit trace stopped warning');
		INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (19,'Audit trace closed warning');
		INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (20,'Audit trace altered warning');
	END
	
	IF NOT EXISTS(select name from EventTypes where evtypeid=1800)
	BEGIN
		EXEC sp_sqlcm_UpdateEventTypes;
	END

	IF NOT EXISTS(select name from EventTypes where evtypeid=1810)
	BEGIN
		EXEC sp_sqlcm_UpdateEventTypes;
	END
	
	IF EXISTS(select name from EventTypes where evtypeid=1500)
	BEGIN
		EXEC sp_sqlcm_UpdateEventTypes;
	END
		
	--------------------------------------------------------------------------------------------------------------
	-- 3.0 Changes
	--------------------------------------------------------------------------------------------------------------
	-- Add trusted user column to the Databases table
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='auditUsersList' and o.name='Databases' and c.id=o.id)
		ALTER TABLE [Databases] ADD [auditUsersList] [nvarchar](MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL;

	-- Stats categories
	---------------------------
	IF OBJECT_ID('StatsCategories') IS NULL
	BEGIN
		CREATE TABLE [StatsCategories] ( [category] [int] NOT NULL, [name] [nvarchar] (64) NOT NULL );
		GRANT SELECT ON [StatsCategories] TO [public];
		INSERT INTO StatsCategories (category, name) VALUES (0, 'Unknown');
		INSERT INTO StatsCategories (category, name) VALUES (1, 'Audited Instances');
		INSERT INTO StatsCategories (category, name) VALUES (2, 'Audited Databases');
		INSERT INTO StatsCategories (category, name) VALUES (3, 'Processed Events');
		INSERT INTO StatsCategories (category, name) VALUES (4, 'Alerts');
		INSERT INTO StatsCategories (category, name) VALUES (5, 'Privileged User Events');
		INSERT INTO StatsCategories (category, name) VALUES (6, 'Failed Logins');
		INSERT INTO StatsCategories (category, name) VALUES (7, 'User Defined Events');
		INSERT INTO StatsCategories (category, name) VALUES (8, 'Admin');
		INSERT INTO StatsCategories (category, name) VALUES (9, 'DDL');
		INSERT INTO StatsCategories (category, name) VALUES (10, 'Security');
		INSERT INTO StatsCategories (category, name) VALUES (11, 'DML');
		INSERT INTO StatsCategories (category, name) VALUES (12, 'Insert');
		INSERT INTO StatsCategories (category, name) VALUES (13, 'Update');
		INSERT INTO StatsCategories (category, name) VALUES (14, 'Delete');
		INSERT INTO StatsCategories (category, name) VALUES (15, 'Select');
		INSERT INTO StatsCategories (category, name) VALUES (16, 'Logins');
		INSERT INTO StatsCategories (category, name) VALUES (17, 'Agent Trace Directory Size');
		INSERT INTO StatsCategories (category, name) VALUES (18, 'Integrity Check');
		INSERT INTO StatsCategories (category, name) VALUES (19, 'Execute');
		INSERT INTO StatsCategories (category, name) VALUES (20, 'Event Received');
		INSERT INTO StatsCategories (category, name) VALUES (21, 'Event Processed');
		INSERT INTO StatsCategories (category, name) VALUES (22, 'Event Filtered');		
	END
	
	IF OBJECT_ID('ReportCard') IS NULL
	BEGIN
		CREATE TABLE [dbo].[ReportCard](
			[srvId] [int] NOT NULL,
			[statId] [int] NOT NULL,
			[warningThreshold] [int] NOT NULL,
			[errorThreshold] [int] NOT NULL,
			[period] [int] NOT NULL,
			[enabled] [tinyint] NOT NULL
		) ON [PRIMARY];
		GRANT SELECT ON [ReportCard] TO [public];
	END
	
	-- Create the ThresholdConfiguration table in 5.3
	
	IF OBJECT_ID('ThresholdConfiguration') IS NULL
	BEGIN
		CREATE TABLE [dbo].[ThresholdConfiguration](
			[instance_name] [nvarchar](50) NULL,
			[sender_email] [nvarchar](50) NULL,
			[send_mail_permission] [tinyint] NULL,
			[snmp_permission] [tinyint] NULL,
			[logs_permission] [tinyint] NULL,
			[srvId] [int] NULL,
			[snmpAddress] [nvarchar](50) NULL,
			[snmpPort] [int] NULL,
			[snmpCommunity] [nvarchar](50) NULL,
			[severity] [int] NULL,
			[messageData] [nvarchar](MAX) NULL
		) ON [PRIMARY];
		GRANT SELECT ON [ThresholdConfiguration] TO [public];
	END
	
	IF NOT EXISTS (SELECT c.name,c.id from [SQLcompliance]..syscolumns as c,[SQLcompliance]..sysobjects as o where c.name='srvId' and o.name='ThresholdConfiguration' and c.id=o.id)		
	BEGIN	
		ALTER TABLE [SQLcompliance]..[ThresholdConfiguration] ADD
			[srvId] [int] NULL,
			[snmpAddress] [nvarchar](50) NULL,
			[snmpPort] [int] NULL,
			[snmpCommunity] [nvarchar](50) NULL,
			[severity] [int] NULL,
			[messageData] [nvarchar](MAX) NULL;
	END

	-- Create the licenses table
	IF OBJECT_ID('Licenses') IS NULL
	BEGIN
		CREATE TABLE [dbo].[Licenses] (
	    [licenseid] INTEGER IDENTITY(1,1) NOT NULL,
	    [licensekey] NVARCHAR(256) NOT NULL,
	    [createdby] NVARCHAR(500) NOT NULL,
	    [createdtm] DATETIME NOT NULL,
	    CONSTRAINT [PK__applicationlicen__46E78A0C] PRIMARY KEY ([licenseid])
		);
	END
	-- This was missed in the 3.0 upgrade path - just do it by default
	GRANT SELECT ON [Licenses] TO [public]

	--------------------------------------------------------------------------------------------------------------
	-- 3.1 Changes	
	--------------------------------------------------------------------------------------------------------------
	-- the table that holds the tables being monitored for data changes across all instances
	IF OBJECT_ID('DataChangeTables') IS NULL
	BEGIN
		CREATE TABLE DataChangeTables ( srvId int not null,
										dbId int not null,
										objectId int not null,
                                        schemaName nVarchar(128) not null,
										tableName nVarchar(128) not null,
										rowLimit int not null default (20),
										CONSTRAINT [PK_DataChangeTables] PRIMARY KEY CLUSTERED (srvId, dbId, objectId )  )
			on [PRIMARY];
		GRANT SELECT ON [DataChangeTables] TO [public];
	END

	-- Add a new column to the databases table for indicating whether there are tables monitored for data changes 
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='auditDataChanges' and o.name='Databases' and c.id=o.id)
		ALTER TABLE Databases ADD auditDataChanges tinyint not null default 0;

	-- Add a new column to the DatabaseObjects table for schema support
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='schemaName' and o.name='DatabaseObjects' and c.id=o.id)
	BEGIN
		ALTER TABLE DatabaseObjects ADD schemaName nvarchar(128) not null default 'dbo';
	END

	-- Update Reports Table
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='port' and o.name='Reports' and c.id=o.id)
	BEGIN
	   DROP TABLE [Reports];
      CREATE TABLE [Reports] (
      	[reportServer] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
      	[serverVirtualDirectory] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
      	[managerVirtualDirectory] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
      	[port] [int] NULL ,
      	[useSsl] [tinyint] NULL ,
      	[userName] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
      	[repository] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
      	[targetDirectory] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
      ) ON [PRIMARY];
		GRANT SELECT ON [Reports] TO [public];
	END
	
	IF NOT EXISTS (SELECT c.name,c.id from [SQLcomplianceProcessing]..syscolumns as c,[SQLcomplianceProcessing]..sysobjects as o where c.name='eventSequence' and o.name='TraceStates' and c.id=o.id)		
	ALTER TABLE [SQLcomplianceProcessing]..[TraceStates] ADD
		[eventSequence]         [bigint] NULL ;
		
	IF NOT EXISTS (SELECT c.name,c.id from [SQLcompliance]..syscolumns as c,[SQLcompliance]..sysobjects as o where c.name='details' and o.name='AgentEvents' and c.id=o.id)		
	ALTER TABLE [SQLcompliance]..[AgentEvents] ADD
   	[details] [nvarchar](MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL;
		
	IF NOT EXISTS (SELECT c.name,c.id from [SQLcompliance]..syscolumns as c,[SQLcompliance]..sysobjects as o where c.name='agentHealth' and o.name='Servers' and c.id=o.id)		
	ALTER TABLE [SQLcompliance]..[Servers] ADD
      [agentHealth] [bigint] DEFAULT(0) NULL;
      
	IF NOT EXISTS(select name from EventTypes where evtypeid=900001)
	BEGIN
   	INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (900001,4,'Encrypted','DML',0,0);
	END
	
	IF NOT EXISTS(select Name from AgentEventTypes where eventId=1001 )
	BEGIN
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (1001,'Agent Warning');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (3001,'Agent Warning Resolution');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (2001,'Agent Configuration Error');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (4001,'Agent Configuration Resolution');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (2002,'Trace Directory Error');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (4002,'Trace Directory Resolution');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (2003,'SQL Trace Error');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (4003,'SQL Trace Resolution');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (2004,'Server Connection Error');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (4004,'Server Connection Resolution');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (2005,'Collection Service Connection Error');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (4005,'Collection Service Connection Resolution');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (2006,'CLR Error');
      INSERT INTO [AgentEventTypes] ([eventId],[Name]) VALUES (4006,'CLR Resolution');
	END      
	
	UPDATE [SQLcompliance]..[EventTypes] SET [isExcludable]=1 WHERE evtypeid=139;
	UPDATE [SQLcompliance]..[EventTypes] SET [isExcludable]=1 WHERE evtypeid=339;
		
	UPDATE Configuration SET reportingVersion=103
	UPDATE Configuration SET repositoryVersion=100

	--------------------------------------------------------------------------------------------------------------
	-- 3.2 Changes
	--------------------------------------------------------------------------------------------------------------
	-- Main compliance database
	-- Add a new column to the DataChangeTables table for selected column auditing support
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='selectedColumns' and o.name='DataChangeTables' and c.id=o.id)
	BEGIN
		ALTER TABLE DataChangeTables ADD selectedColumns tinyint not null default 0;
	END

	-- Add a new table for selected column auditing support
	IF OBJECT_ID('DataChangeColumns') IS NULL
	BEGIN
		CREATE TABLE [dbo].[DataChangeColumns](
			[srvId] [int] NOT NULL,
			[dbId] [int] NOT NULL,
			[objectId] [int] NOT NULL,
			[name] [nvarchar](128) NOT NULL,
			CONSTRAINT [FK_DataChangeColumns_DataChangeTables] FOREIGN KEY 
			(
				[srvId],
				[dbId],
				[objectId]
			) REFERENCES [dbo].[DataChangeTables] (
				[srvId],
				[dbId],
				[objectId]
			)
		) ON [PRIMARY];

		GRANT SELECT ON [DataChangeColumns] TO [public];
	END
		
	-- Add a new alert type for supporting operational status alerts
	IF NOT EXISTS(select alertTypeId from AlertTypes where alertTypeId=2 )
	BEGIN
		INSERT INTO [AlertTypes] ([alertTypeId], [name]) VALUES (2, 'SQLcompliance Status');
	END
	
	-- add column to hold collection server heartbeat interval if it doesnt already exist
	if NOT EXISTS (SELECT c.name,c.id FROM syscolumns AS c,sysobjects AS o WHERE c.name='collectionServerHeartbeatInterval' and o.name='Configuration' and c.id=o.id)
	BEGIN
		ALTER TABLE Configuration ADD [collectionServerHeartbeatInterval] [int] NOT NULL DEFAULT (5);	
	END
	
	if NOT EXISTS (SELECT c.name,c.id FROM syscolumns AS c,sysobjects AS o WHERE c.name='computerName' and o.name='Alerts' and c.id=o.id)
	BEGIN
		ALTER TABLE Alerts ADD [computerName] [nvarchar](256) NULL;	
	END
	
	if NOT EXISTS (SELECT c.name,c.id FROM syscolumns AS c,sysobjects AS o WHERE c.name='indexStartTime' and o.name='Configuration' and c.id=o.id)
	BEGIN
		ALTER TABLE Configuration ADD [indexStartTime] [datetime] NULL,
									  [indexDurationInSeconds] [int] NULL;	
		
	END

	IF OBJECT_ID('StatusRuleTypes') IS NULL
	BEGIN
	   CREATE TABLE [dbo].[StatusRuleTypes](
	      [StatusRuleId] [int] NOT NULL,
	      [RuleName] [nvarchar](100) NOT NULL
	      CONSTRAINT [PK_StatusRuleId] PRIMARY KEY  CLUSTERED 
	      (
			   [StatusRuleId]
		   )  
		)ON [PRIMARY]
	   GRANT SELECT ON [StatusRuleTypes] TO [public]
	END

	IF NOT EXISTS(select RuleName from StatusRuleTypes where StatusRuleId=1)
	BEGIN
      INSERT INTO StatusRuleTypes (StatusRuleId, RuleName) VALUES (1, 'Agent trace directory reached size limit')
      INSERT INTO StatusRuleTypes (StatusRuleId, RuleName) VALUES (2, 'Collection Server trace directory reached size limit');
      INSERT INTO StatusRuleTypes (StatusRuleId, RuleName) VALUES (3, 'Agent heartbeat was not received');
      INSERT INTO StatusRuleTypes (StatusRuleId, RuleName) VALUES (4, 'Event database is too large');
      INSERT INTO StatusRuleTypes (StatusRuleId, RuleName) VALUES (5, 'Agent cannot connect to audited instance');
	END
	
	IF NOT EXISTS (SELECT Name FROM ChangeLogEventTypes WHERE eventId=64)
	BEGIN
      INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (64, 'Re-Index Started')
      INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (65, 'Re-Index Finished')
	END		

	IF NOT EXISTS (select Name from ChangeLogEventTypes where eventId=66 )
	BEGIN
		INSERT INTO [ChangeLogEventTypes] ([eventId],[Name]) VALUES (66,'Event Database Limit Exceeded');
	END		
	--------------------------------------------------------------------------------------------------------------
	-- 3.3 Changes
	--------------------------------------------------------------------------------------------------------------
	IF NOT EXISTS (SELECT c.name,c.id FROM syscolumns AS c,sysobjects AS o WHERE c.name='agentTraceStartTimeout' and o.name='Servers' and c.id=o.id)
	BEGIN
		ALTER TABLE Servers ADD [agentTraceStartTimeout] [int] NULL;	
	END
		
	--------------------------------------------------------------------------------------------------------------
	-- 3.5 Changes
	--------------------------------------------------------------------------------------------------------------
	IF OBJECT_ID('SensitiveColumnTables') IS NULL
	BEGIN
		CREATE TABLE SensitiveColumnTables ( 
			[srvId] [int] NOT NULL,
			[dbId] [int] NOT NULL,
			[objectId] [int] NOT NULL,
			[schemaName] [nvarchar] (128) NOT NULL,
			[tableName] [nvarchar] (128) NOT NULL,
			[selectedColumns] [tinyint] NOT NULL DEFAULT 1,
			CONSTRAINT [PK_SensitiveColumnTables] PRIMARY KEY CLUSTERED 
			(
				[srvId], 
				[dbId], 
				[objectId]
			)
		) ON [PRIMARY]

		CREATE TABLE [dbo].[SensitiveColumnColumns](
			[srvId] [int] NOT NULL,
			[dbId] [int] NOT NULL,
			[objectId] [int] NOT NULL,
			[name] [nvarchar](128) NOT NULL,
			[type] [nvarchar](128) NULL,
			[columnId] [int] NULL,
			CONSTRAINT [FK_SensitiveColumnColumns_SensitiveColumnTables] FOREIGN KEY 
			(
				[srvId],
				[dbId],
				[objectId]
			) REFERENCES [dbo].[SensitiveColumnTables] (
				[srvId],
				[dbId],
				[objectId]
			)
		) ON [PRIMARY]
		
		GRANT SELECT ON [SensitiveColumnTables] TO [public];
		GRANT SELECT ON [SensitiveColumnColumns] TO [public];
	END
	------------------------------------------------------------------------------------------------------------------------------------------------
	-- Sensitive Column Changes
	------------------------------------------------------------------------------------------------------------------------------------------------
	If NOT EXISTS (SELECT c.name,c.id FROM syscolumns AS c,sysobjects AS o WHERE c.name='type' and o.name='SensitiveColumnColumns' and c.id=o.id)
	BEGIN
		ALTER TABLE SensitiveColumnColumns ADD [type] [nvarchar](128) NULL DEFAULT ('Individual') WITH VALUES;	
	END
	If NOT EXISTS (SELECT c.name,c.id FROM syscolumns AS c,sysobjects AS o WHERE c.name='columnId' and o.name='SensitiveColumnColumns' and c.id=o.id)
	BEGIN
		ALTER TABLE SensitiveColumnColumns ADD [columnId] [int] NULL;	
	END	
    	-------------------------------------------------------------------------------------------------------------------------------------------------
	
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='auditSensitiveColumns' and o.name='Databases' and c.id=o.id)
	BEGIN
		ALTER TABLE Databases ADD auditSensitiveColumns tinyint NOT NULL DEFAULT 0,
								  auditCaptureTrans tinyint NULL DEFAULT 0;
								  
	END	
	
	IF NOT EXISTS (SELECT c.name,c.id FROM syscolumns AS c,sysobjects AS o WHERE c.name='auditUserCaptureTrans' and o.name='Servers' and c.id=o.id)
	BEGIN
		ALTER TABLE Servers ADD [auditUserCaptureTrans] [tinyint] NULL DEFAULT (0);	
	END
	
	
	IF NOT EXISTS(select name from EventTypes where evtypeid=40)
	BEGIN
		INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (40,4,'Begin Transaction','DML',0,1)
		INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (41,4,'Commit Transaction','DML',0,1)
		INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (42,4,'Rollback Transaction','DML',0,1)
		INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (43,4,'Save Transaction','DML',0,1)
	END
		
	--------------------------------------------------------------------------------------------------------------
	-- 3.6 Changes
	--------------------------------------------------------------------------------------------------------------	
	IF OBJECT_ID('DataRuleTypes') IS NULL
	BEGIN
		CREATE TABLE [dbo].[DataRuleTypes] (
		   [DataRuleId] [int] NOT NULL,
		   [RuleName] [nvarchar](100) NOT NULL
		   CONSTRAINT [PK_DataRuleId] PRIMARY KEY  CLUSTERED 
		   (
			   [DataRuleId]
		   )  
		) ON [PRIMARY]
		GRANT SELECT ON [DataRuleTypes] TO [public]
	END
	
	IF NOT EXISTS(select RuleName from DataRuleTypes where DataRuleId=1)
	BEGIN
      INSERT INTO DataRuleTypes (DataRuleId, RuleName) VALUES (1, 'Sensitive Column Accessed')
      INSERT INTO DataRuleTypes (DataRuleId, RuleName) VALUES (2, 'Numeric Column Value Changed');
    END		
    
	-- Add a new alert type for supporting operational status alerts
	IF NOT EXISTS(select alertTypeId from AlertTypes where alertTypeId=3 )
	BEGIN
		INSERT INTO [AlertTypes] ([alertTypeId], [name]) VALUES (3, 'Data Alert');
	END
	
	IF NOT EXISTS (SELECT c.name,c.id FROM syscolumns AS c,sysobjects AS o WHERE c.name='refactorTable' and o.name='Configuration' and c.id=o.id)
	BEGIN
		DECLARE @config INT
		SET @config = (SELECT [sqlComplianceDbSchemaVersion] FROM Configuration)

		--this should work ok since there is only one row in the configuration table, 
		IF (@config = 802 or @config = 803)
			ALTER TABLE Configuration ADD [refactorTable] [tinyint] NOT NULL DEFAULT (1);	
		else
			ALTER TABLE Configuration ADD [refactorTable] [tinyint] NULL DEFAULT (0);	
	END
		
	--------------------------------------------------------------------------------------------------------------
	-- 4.0 Changes
	--------------------------------------------------------------------------------------------------------------	
    IF OBJECT_ID ('Regulation') IS NULL
    BEGIN
		CREATE TABLE [dbo].[Regulation](
			[regulationId] [int] IDENTITY(1,1) NOT NULL,
			[name] [nvarchar] (512) NOT NULL,
			[description] [nvarchar] (2048) NULL,
			CONSTRAINT [PK_Regulation] PRIMARY KEY CLUSTERED
			(
				[regulationId]
			)
		)	ON [PRIMARY]
		GRANT SELECT ON [Regulation] TO [public]
		
		INSERT INTO Regulation (name, description) VALUES ('PCI DSS', 'The PCI DSS (Payment Card Industry Data Security Standard v3.0) is a set 
		of comprehensive requirements developed by the PCI Security Standards Council which includes American Express, Discover Financial Services, JCB 
		International, MasterCard Worldwide and Visa Inc. to help standardize the broad adoption of consistent data security measures on a global basis. 
		In addition it also includes requirements for security management, policies, procedures, network architecture, software design and other safeguard 
		measures. This standard is intended to help organizations to proactively secure customer data.  PCI data that resides on Microsoft SQL Server must 
		adhere to these regulations. From an IT perspective, security officers mandate that all user access to PCI data (including object changes), must 
		be audited, stored and reported to the security department.')
		INSERT INTO Regulation (name, description) VALUES ('HIPAA', 'The Health Insurance Portability and Accountability Act of 1996 (HIPAA) required 
		the Secretary of the U.S. Department of Health and Human Services (HHS) to develop regulations protecting the privacy and security of certain 
		health information. To fulfill this requirement, HHS published what are commonly known as the HIPAA Privacy Rule and the HIPAA Security Rule. 
		The Privacy Rule, or Standards for Privacy of Individually Identifiable Health Information, establishes national standards for the protection 
		of certain health information. The Security Standards for the Protection of Electronic Protected Health Information (the Security Rule) establish 
		a national set of security standards for protecting certain health information that is held or transferred in electronic form. Furthermore, 
		the Health Information Technology for Economic and Clinical Health Act (HITECH) contains specific incentives designed to accelerate the adoption 
		of electronic health record systems among providers.')
	END
	
	IF OBJECT_ID ('RegulationMap') IS NULL
	BEGIN
		CREATE TABLE [dbo].[RegulationMap] (
			[regulationMapId] [int] IDENTITY(1,1) NOT NULL,
			[regulationId] [int] NOT NULL,
			[section] [nvarchar] (max) NOT NULL,
			[sectionDescription] [nvarchar] (max) NULL,
			[serverEvents] [int] NOT NULL,
			[databaseEvents] [int] NOT NULL,
			CONSTRAINT [PK_RegulationMap] PRIMARY KEY CLUSTERED
			(
				[regulationMapId]
			),
			CONSTRAINT [FK_RegulationMap_Regulation] FOREIGN KEY 
			(
				[regulationId]
			)
			REFERENCES [dbo].[Regulation] 
			(
				[regulationId]
			)
		)	ON [PRIMARY]
		GRANT SELECT ON [RegulationMap] TO [public]
		
		--PCI
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(1, '8', 'Assigning a unique identification (ID) to each person with access ensures that each individual is uniquely accountable for 
		his or her actions. When such accountability is in place, actions taken on critical data and systems are performed by, and can be 
		traced to, known and authorized users.', (2+4+8+16+256), (1+2+4+8+128+512))
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(1, '8.5.4', 'Immediately revoke access for any  terminated users', (4+16),	1)
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(1, '10.1', 'Establish a process for linking all access to system components (especially access done with administrative privileges 
		such as root) to each individual user)', (2+16+256), 0)
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(1, '10.2', 'Implement automated audit trails for all system components to reconstruct the following events:\r\n10.2.1 - All 
		individual user accesses to cardholder data\r\n10.2.2 - All actions taken by any individual with root or administrative privileges
		\r\n10.2.3 - Access to all audit trails\r\n10.2.4 - Invalid logical access attempts\r\n10.2.5 - Use of identification and authentication
		 mechanisms\r\n10.2.7 - Creation and deletions of system-level objects', (2+8+128), (2+8+512))
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(1, '10.3', 'Record at least the following audit trail entries for all system components for each event:\r\n10.3.1 - User identification
		\r\n10.3.2 - Type of event\r\n10.3.3 - Date and time\r\n10.3.4 - Success or failure indication\r\n10.3.5 - Origination of event\r\n
		10.3.6 - Identity or name of affected data, system component or resource', (2+256), (1+2+8+512))

		--HIPAA
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, '164.306 (a,2)', 'Security Standards - Protect against any reasonably anticipated threats or hazards to the security or integrity 
		of such information', (2+4+8+256), (8+512))
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, '164.308 (1,i)', 'Security Management Process - Implement policies and procedures to prevent, detect, contain and correct security violations', (2+4+8+256), 0)
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, '164.308 (B)', 'Risk Management - Implement security measures sufficient to reduce risks and vulnerabilities to a reasonable and appropriate 
		level to comply with 164.306(a).', (2+4+8+256), 0)
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, '164.308 (D)', 'Information System Activity Review - (Required). Implement procedures to regularly review records of information 
		system activity such as audit logs, access reports and security incident tracking reports.', (2+4+8+256), (1+2+4+8+512))
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, '164.308 (3,C)', 'Termination procedures - Implement procedures for terminating access to electronic protected health information 
		when the employment of a workforce member ends or as required by determinations made as specified in paragraph (a) (3) (ii) (B) of this section.', 4, 1)
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, '164.308 (5,C)', 'Implementation Specifications - Log-in monitoring (Addressable). Procedures for monitoring log-in attempts and reporting discrepancies.', 2, 0)
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, '164.312 (b)', 'Technical Standard - Audit controls. Implement hardware, software, and/or procedural mechanisms that record and 
		examine activity in information systems that contain or use electronic protected health information.', (2+4+8+16), (1+2+4+8+512))
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, '164.404 (a)(1)(2)', 'Security and Privacy  - General rule. A covered entity shall, following the discovery of a breach of unsecured 
		protected health information, notify each individual whose unsecured protected health information has been, or is reasonably believed by
		 the covered entity to have been, accessed, acquired, used, or disclosed as a result of such breach.', 0, 513)
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, '164.404 (c)(1) (A),(B)', 'Security and Privacy  - (c) Implementation specifications: Content of notification-  \r\n(1) Elements. 
		The notification required by (a) of this section shall include, to the extent possible:\r\n(A) A brief description of what happened, 
		including the date of the breach and the date of the discovery of the breach, if known;\r\n(B) A description of the types of unsecured
		 protected health information that were involved in the breach (such as whether full name, social security number, date of birth, home
		  address, account number, diagnosis, disability code, or other types of information.', 0, 512)
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, 'HITECH 13402 (a)(f) (1,2)', 'Notification In the Case of Breach  \r\n(a) In General.—A covered entity that accesses, maintains, 
		retains, modifies, records, stores, destroys, or otherwise holds, uses, or discloses unsecured protected health information (as defined 
		in subsection (h)(1)) shall, in the case of a breach of such information that is discovered by the covered entity, notify each individual 
		whose unsecured protected health information has been, or is reasonably believed by the covered entity to have been, accessed, acquired, 
		or disclosed as a result of such breach.\r\n(f)  Content of Notification.—Regardless of the method by which notice is provided to individuals
		 under this section, notice of a breach shall include, to the extent possible, the following:\r\n(1) A brief description of what happened, 
		 including the date of the breach and the date of the discovery of the breach, if known. \r\n(2) A description of the types of unsecured 
		 protected health information that were involved in the breach (such as full name, Social Security number, date of birth, home address, 
		 account number, or disability code).', 0, 512)		
    END
    
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='pci' and o.name='Databases' and c.id=o.id)
	BEGIN
		ALTER TABLE Databases ADD [pci] [tinyint] NOT NULL DEFAULT (0),
								  [hipaa] [tinyint] NOT NULL DEFAULT (0);
	END	
	
    
	--------------------------------------------------------------------------------------------------------------
	-- 4.2 Changes
	--------------------------------------------------------------------------------------------------------------	
    	--sp_cmreport_GetBeforeAfterData changed
    
	--******  update the repository schema version after applying all of the changes  ******
	
	--------------------------------------------------------------------------------------------------------------
	--4.4 Changes
	--------------------------------------------------------------------------------------------------------------

	-- add column to hold archiveDatabaseFilesLocation if it doesnt already exist
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='archiveDatabaseFilesLocation' and o.name='Configuration' and c.id=o.id)
		ALTER TABLE Configuration ADD [archiveDatabaseFilesLocation] [nvarchar](255) NULL;
	
	-- add column to hold archiveScheduleType if it doesnt already exist
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='archiveScheduleType' and o.name='Configuration' and c.id=o.id)
		ALTER TABLE Configuration ADD [archiveScheduleType] [int] NULL DEFAULT (0);
	
	-- add column to hold archiveScheduleTime if it doesnt already exist
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='archiveScheduleTime' and o.name='Configuration' and c.id=o.id)
		ALTER TABLE Configuration ADD [archiveScheduleTime] datetime CONSTRAINT DF_Configuration_archiveScheduleTime DEFAULT '1-1-2014 1:30:00 AM';
	
	-- add column to hold archiveScheduleRepetition if it doesnt already exist
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='archiveScheduleRepetition' and o.name='Configuration' and c.id=o.id)
		ALTER TABLE Configuration ADD [archiveScheduleRepetition] [int] NULL DEFAULT (1);
	
	-- add column to hold archiveScheduleWeekDay if it doesnt already exist
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='archiveScheduleWeekDay' and o.name='Configuration' and c.id=o.id)
		ALTER TABLE Configuration ADD [archiveScheduleWeekDay] [nvarchar](7) NULL DEFAULT('0000000');
	
	-- add column to hold archiveScheduleDayOrWeekOfMonth if it doesnt already exist
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='archiveScheduleDayOrWeekOfMonth' and o.name='Configuration' and c.id=o.id)
		ALTER TABLE Configuration ADD [archiveScheduleDayOrWeekOfMonth] [int] NULL DEFAULT (1);
	
	-- add column to hold archiveScheduleExecutionPlan if it doesnt already exist
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='archiveScheduleExecutionPlan' and o.name='Configuration' and c.id=o.id)
		ALTER TABLE Configuration ADD [archiveScheduleExecutionPlan] [nvarchar] (MAX) NULL;
	
	-- add column to hold archiveScheduleIsArchiveRunning if it doesnt already exist
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='archiveScheduleIsArchiveRunning' and o.name='Configuration' and c.id=o.id)
		ALTER TABLE Configuration ADD [archiveScheduleIsArchiveRunning] [tinyint] NULL DEFAULT (0);
	
	-- add column to hold snmpTrap if it doesnt already exist
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='snmpTrap' and o.name='AlertRules' and c.id=o.id)
		ALTER TABLE AlertRules ADD [snmpTrap] tinyint NULL;
	
	-- add column to hold SNMP server Address if it doesnt already exist
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='snmpServerAddress' and o.name='Configuration' and c.id=o.id)
		ALTER TABLE Configuration ADD [snmpServerAddress] [nvarchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL;

	-- add column to hold SNMP server port if it doesnt already exist
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='snmpPort' and o.name='Configuration' and c.id=o.id)
		ALTER TABLE Configuration ADD [snmpPort] [int] NULL DEFAULT (162);

	-- add column to hold SNMP community if it doesnt already exist
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='snmpCommunity' and o.name='Configuration' and c.id=o.id)
		ALTER TABLE Configuration ADD [snmpCommunity] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL DEFAULT ('public');

	-- add column to hold SNMP server Address if it doesnt already exist for respective specific alert rule
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='snmpServerAddress' and o.name='AlertRules' and c.id=o.id)
		ALTER TABLE AlertRules ADD [snmpServerAddress] [nvarchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL;

	-- add column to hold SNMP server port if it doesnt already exist for respective specific alert rule
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='snmpPort' and o.name='AlertRules' and c.id=o.id)
		ALTER TABLE AlertRules ADD [snmpPort] [int] NULL DEFAULT (162);

	-- add column to hold SNMP community if it doesnt already exist for respective specific alert rule
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='snmpCommunity' and o.name='AlertRules' and c.id=o.id)
		ALTER TABLE AlertRules ADD [snmpCommunity] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL DEFAULT ('public');

	-- add column to hold snmpTrapStatus in alerts if it doesnt already exist
	if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='snmpTrapStatus' and o.name='Alerts' and c.id=o.id)
		ALTER TABLE Alerts ADD [snmpTrapStatus] [tinyint] NOT NULL DEFAULT(0);
	
	--------------------------------------------------------------------------------------------------------------
	--4.5 Changes
	--------------------------------------------------------------------------------------------------------------
	IF ((SELECT COUNT(name) FROM sys.tables WHERE name = 'Regulation') = 1)
	BEGIN
		UPDATE [dbo].[Regulation] 
		SET [description] = 'The PCI DSS (Payment Card Industry Data Security Standard v3.0) is a set of comprehensive requirements developed by the PCI Security Standards Council which includes American Express, Discover Financial Services, JCB International, MasterCard Worldwide and Visa Inc. to help standardize the broad adoption of consistent data security measures on a global basis. In addition it also includes requirements for security management, policies, procedures, network architecture, software design and other safeguard measures. This standard is intended to help organizations to proactively secure customer data. PCI data that resides on Microsoft SQL Server must adhere to these regulations. From an IT perspective, security officers mandate that all user access to PCI data (including object changes), must be audited, stored and reported to the security department.' 
		WHERE [name] = 'PCI DSS';
	END
	
	--******  update the repository schema version after applying all of the changes  ******
		
	--------------------------------------------------------------------------------------------------------------
	--5.0 Changes
	--------------------------------------------------------------------------------------------------------------
	IF (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Databases' AND COLUMN_NAME IN ('isAlwaysOn', 'isPrimary', 'replicaServers', 'availGroupName')) <> 4
	BEGIN
		ALTER TABLE [SQLcompliance]..[Databases] 
		ADD [isAlwaysOn] [tinyint] NULL DEFAULT (0),
		    [isPrimary] [tinyint] NULL DEFAULT (0),
		    [replicaServers] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
		    [availGroupName] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL;
			
		-- need to separate SQL batch, cannot call ALTER and UPDATE simultaneously
		-- using EXEC for this
		EXEC('UPDATE [SQLcompliance]..[Databases] 
			  SET isAlwaysOn = 0, 
				  isPrimary = 0, 
				  replicaServers = '''', 
				  availGroupName = '''';')
	END
			  
	IF (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Databases' AND COLUMN_NAME IN ('auditPrivUsersList', 'auditUserAll', 'auditUserLogins', 'auditUserFailedLogins', 'auditUserDDL', 'auditUserSecurity', 'auditUserAdmin', 'auditUserDML', 'auditUserSELECT', 'auditUserFailures', 'auditUserCaptureSQL', 'auditUserCaptureTrans', 'auditUserExceptions', 'auditUserUDE')) <> 14
	BEGIN
		ALTER TABLE [SQLcompliance]..[Databases] 
		ADD	[auditPrivUsersList] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			[auditUserAll] [tinyint] NULL DEFAULT (0),
			[auditUserLogins] [tinyint] NULL DEFAULT (0),
			[auditUserFailedLogins] [tinyint] NULL DEFAULT (0),
			[auditUserDDL] [tinyint] NULL DEFAULT (0),
			[auditUserSecurity] [tinyint] NULL DEFAULT (0),
			[auditUserAdmin] [tinyint] NULL DEFAULT (0),
			[auditUserDML] [tinyint] NULL DEFAULT (0),
			[auditUserSELECT] [tinyint] NULL DEFAULT (0),
			[auditUserFailures] [tinyint] NULL DEFAULT (0),
			[auditUserCaptureSQL] [tinyint] NULL DEFAULT (0),
			[auditUserCaptureTrans] [tinyint] NULL DEFAULT (0),
			[auditUserExceptions] [tinyint] NULL DEFAULT (0),
			[auditUserUDE] [tinyint] NULL DEFAULT (0);
				  
	END

	IF NOT EXISTS(select RuleName from DataRuleTypes where DataRuleId=3)
	BEGIN
      INSERT INTO DataRuleTypes (DataRuleId, RuleName) VALUES (3, 'Column Value Changed')
    END	
	
	if NOT EXISTS (SELECT c.name,c.id FROM syscolumns AS c,sysobjects AS o WHERE c.name='alwaysOnRoleUpdateInterval' and o.name='Configuration' and c.id=o.id)
	BEGIN
		ALTER TABLE Configuration ADD [alwaysOnRoleUpdateInterval] [int] NOT NULL DEFAULT (10);	
	END
	
	IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Cwf')
	BEGIN
		CREATE TABLE [dbo].[Cwf](
			[CwfUrl] [nvarchar](255) NOT NULL,
			[CwfToken] [nvarchar](255) NOT NULL,
			CONSTRAINT [PK_Cwf] PRIMARY KEY CLUSTERED 
			(
				[CwfUrl] ASC
			)
		) ON [PRIMARY]
	END
	
	IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'LoginAccounts')
	BEGIN
		CREATE TABLE [dbo].[LoginAccounts](
			[Name] [nvarchar](128) NOT NULL,
			[SID] [varbinary](85) NULL,
			[WebApplicationAccess] [bit] NOT NULL CONSTRAINT [DF_Logins_IsHavingWebApplicationAccess]  DEFAULT ((1)),
		CONSTRAINT [PK_Logins] PRIMARY KEY CLUSTERED 
		(
			[Name] ASC
		)
		) ON [PRIMARY]
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
	  
	--5.5 Regulation Guideline
	
	IF NOT EXISTS (SELECT c.name,c.id from syscolumns as c,sysobjects as o where c.name='disa' and o.name='Databases' and c.id=o.id)
	BEGIN
		ALTER TABLE Databases ADD [disa] [tinyint] NOT NULL DEFAULT (0),
								  [nerc] [tinyint] NOT NULL DEFAULT (0),
								  [cis] [tinyint] NOT NULL DEFAULT (0),
								  [sox] [tinyint] NOT NULL DEFAULT (0),
								  [ferpa] [tinyint] NOT NULL DEFAULT (0);
	END	


-------------------------------------------------INSATANCE SYNCHRONIZATION-------------------------------------------------
if not exists (select c.name,c.id from syscolumns as c,sysobjects as o where c.name='isSynchronized' and o.name='Servers' and c.id=o.id)
BEGIN
	ALTER TABLE [SQLcompliance]..[Servers] ADD [isSynchronized] [bit] NOT NULL DEFAULT (0);
END
		
-------------------------------------------------INSATANCE SYNCHRONIZATION-------------------------------------------------

-----------------------------------------5.5-----------------------------------
IF OBJECT_ID ('RegulationMap') IS NOT NULL
BEGIN 
  DROP Table RegulationMap;
END
IF OBJECT_ID ('Regulation') IS NOT NULL
BEGIN 
  DROP Table Regulation;
END
IF OBJECT_ID ('Regulation') IS NULL
    BEGIN
		CREATE TABLE [dbo].[Regulation](
			[regulationId] [int] IDENTITY(1,1) NOT NULL,
			[name] [nvarchar] (512) NOT NULL,
			[description] [nvarchar] (2048) NULL,
			CONSTRAINT [PK_Regulation] PRIMARY KEY CLUSTERED
			(
				[regulationId]
			)
		)	ON [PRIMARY]
		GRANT SELECT ON [Regulation] TO [public]
		INSERT INTO Regulation (name, description) Values ('Payment Card Data Security Standard (PCI DSS)', 'The PCI DSS (Payment Card Industry Data Security Standard v3.0)
 is a set of comprehensive requirements developed by the PCI Security Standards Council which includes American Express, Discover 
 Financial Services, JCB International, MasterCard Worldwide and Visa Inc. to help standardize the broad adoption of consistent data 
 security measures on a global basis. In addition it also includes requirements for security management, policies, procedures, 
 network architecture, software design and other safeguard measures. This standard is intended to help organizations to proactively 
 secure customer data.  PCI data that resides on Microsoft SQL Server must adhere to these regulations. From an IT perspective, 
 security officers mandate that all user access to PCI data (including object changes), must be audited, stored and reported to 
 the security department.')
		INSERT INTO Regulation (name, description) Values ('Health Insurance Portability and Accountability Act (HIPAA)','The Health Insurance Portability and Accountability Act of 1996 (HIPAA) 
		required the Secretary of the U.S. Department of Health and Human Services (HHS) to develop regulations protecting the privacy and 
		security of certain health information. To fulfill this requirement, HHS published what are commonly known as the HIPAA Privacy Rule
		 and the HIPAA Security Rule. The Privacy Rule, or Standards for Privacy of Individually Identifiable Health Information, establishes
		  national standards for the protection of certain health information. The Security Standards for the Protection of Electronic Protected
		   Health Information (the Security Rule) establish a national set of security standards for protecting certain health information that
			is held or transferred in electronic form. Furthermore, the Health Information Technology for Economic and Clinical Health Act
			 (HITECH) contains specific incentives designed to accelerate the adoption of electronic health record systems among providers.')
		INSERT INTO Regulation (name, description) Values ('Defense Information Security Agency (DISA STIG)','Defense Information Security Agency and National Institute
		of Standards and Technology (NIST), agency under the Department of
		Commerce, provide guidance on information security for various branches
		of the US government. Security Technical Implementation Guides
		(STIGs) address the security rules, specific queries for checks, and
		remediation. DISA STIG does require an on-going audit.')
		INSERT INTO Regulation (name, description) Values ('North American Electric Reliability Corporation (NERC)','The North American Electric Reliability Corporation (NERC)
		promotes the reliability and adequacy of bulk power transmission
		systems. NERC published the Critical Infrastructure Protection (CIP),
		which provide guidance on securing SQL Server. NERC has an ongoing
		auditing requirement to ensure that the business data is only accessed by
		the appropriate parties.')
		INSERT INTO Regulation (name, description) Values ('Center for Internet Security (CIS)','Center for Internet Security (CIS) provides security guidance
		to protect against online threats. The CIS Controls and Benchmarks
		standards outlines best practices for securing data.')
		INSERT INTO Regulation (name, description) Values ('Sarbanes-Oxley Act (SOX)','Sarbanes-Oxley Act of 2002 (SOX) is concerned with how
		internal processes are managed and implemented, specifically focusing 
		on auditing trails and separations of duties. SOX 404 has an ongoing 
		audit requirement to ensure that business data is only accessed by the
		appropriate parties.')
		INSERT INTO Regulation (name, description) Values ('Family Educational Rights and Privacy Act (FERPA)','Family Educational Rights and Privacy Act (FERPA) - US
		Department of Education sets and administers FERPA, a privacy law
		designed to protect student educational records, including PII information.
		Students and their parents have a right to access their education records.
		FERPA applies to all federally funded institutions that receive funds from
		the US Department of Education. FERPA ensures the confidentiality,
		integrity and accuracy of personal student information.')
	END
IF OBJECT_ID ('RegulationMap') IS NULL
	BEGIN
		CREATE TABLE [dbo].[RegulationMap] (
			[regulationId] [int] NOT NULL,
			[section] [nvarchar] (max) NOT NULL,
			[sectionDescription] [nvarchar] (max) NULL,
			[serverEvents] [int] NOT NULL,
			[databaseEvents] [int] NOT NULL
			CONSTRAINT [FK_RegulationMap_Regulation] FOREIGN KEY 
			(
				[regulationId]
			) 
			REFERENCES [dbo].[Regulation] 
			(
				[regulationId]
			)
		)	ON [PRIMARY]
		GRANT SELECT ON [RegulationMap] TO [public]
		--PCI		
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(1, 'PCI DSS V3.2', 'Record at least the following audit trail entries for all system components for each event:\r\n10.3.1 - User identification
		\r\n10.3.2 - Type of event\r\n10.3.3 - Date and time\r\n10.3.4 - Success or failure indication\r\n10.3.5 - Origination of event\r\n
		10.3.6 - Identity or name of affected data, system component or resource', (831), (3727))
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(1, '8', 'Assigning a unique identification (ID) to each person with access ensures that each individual is uniquely accountable
		 for his or her actions. When such accountability is in place, actions taken on critical data and systems are performed by, and 
		 can be traced to, known and authorized users.', (2+4+8+16+256), (1+2+4+8+128+512))
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(1, '8.5.4', 'Immediately revoke access for any  terminated users', (4+16),	1)
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(1, '10.1', 'Establish a process for linking all access to system components (especially access done with administrative privileges
		 such as root) to each individual user)', (2+16+256), 0)
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(1, '10.2', 'Implement automated audit trails for all system components to reconstruct the following events:\r\n10.2.1 - All 
		individual user accesses to cardholder data\r\n10.2.2 - All actions taken by any individual with root or administrative privileges
		\r\n10.2.3 - Access to all audit trails\r\n10.2.4 - Invalid logical access attempts\r\n10.2.5 - Use of identification and authentication
		 mechanisms\r\n10.2.7 - Creation and deletions of system-level objects', (2+8+128), (2+8+512))
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(1, '10.3', 'Record at least the following audit trail entries for all system components for each event:\r\n10.3.1 - User identification
		\r\n10.3.2 - Type of event\r\n10.3.3 - Date and time\r\n10.3.4 - Success or failure indication\r\n10.3.5 - Origination of event
		\r\n10.3.6 - Identity or name of affected data, system component or resource', (2+256), (1+2+8+512))
		--HIPAA
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, '164.306 (a,2)', 'Security Standards - Protect against any reasonably anticipated threats or hazards to the security or integrity 
		of such information', (2+4+8+256), (8+512))
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, '164.308 (1,i)', 'Security Management Process - Implement policies and procedures to prevent, detect, contain and correct security violations', (2+4+8+256), 0)
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, '164.308 (B)', 'Risk Management - Implement security measures sufficient to reduce risks and vulnerabilities to a reasonable and appropriate 
		level to comply with 164.306(a).', (2+4+8+256), 0)
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, '164.308 (D)', 'Information System Activity Review - (Required). Implement procedures to regularly review records of information 
		system activity such as audit logs, access reports and security incident tracking reports.', (2+4+8+256), (1+2+4+8+512))
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, '164.308 (3,C)', 'Termination procedures - Implement procedures for terminating access to electronic protected health information 
		when the employment of a workforce member ends or as required by determinations made as specified in paragraph (a) (3) (ii) (B) of this section.', 4, 1)
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, '164.308 (5,C)', 'Implementation Specifications - Log-in monitoring (Addressable). Procedures for monitoring log-in attempts and reporting discrepancies.', 2, 0)
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, '164.312 (b)', 'Technical Standard - Audit controls. Implement hardware, software, and/or procedural mechanisms that record and 
		examine activity in information systems that contain or use electronic protected health information.', (2+4+8+16), (1+2+4+8+512))
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, '164.404 (a)(1)(2)', 'Security and Privacy  - General rule. A covered entity shall, following the discovery of a breach of unsecured 
		protected health information, notify each individual whose unsecured protected health information has been, or is reasonably believed by
		 the covered entity to have been, accessed, acquired, used, or disclosed as a result of such breach.', 0, 513)
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, '164.404 (c)(1) (A),(B)', 'Security and Privacy  - (c) Implementation specifications: Content of notification-  \r\n(1) Elements. 
		The notification required by (a) of this section shall include, to the extent possible:\r\n(A) A brief description of what happened, 
		including the date of the breach and the date of the discovery of the breach, if known;\r\n(B) A description of the types of unsecured
		 protected health information that were involved in the breach (such as whether full name, social security number, date of birth, home
		  address, account number, diagnosis, disability code, or other types of information.', 0, 512)
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(2, 'HITECH 13402 (a)(f) (1,2)', 'Notification In the Case of Breach  \r\n(a) In General.—A covered entity that accesses, maintains, 
		retains, modifies, records, stores, destroys, or otherwise holds, uses, or discloses unsecured protected health information (as defined 
		in subsection (h)(1)) shall, in the case of a breach of such information that is discovered by the covered entity, notify each individual 
		whose unsecured protected health information has been, or is reasonably believed by the covered entity to have been, accessed, acquired, 
		or disclosed as a result of such breach.\r\n(f)  Content of Notification.—Regardless of the method by which notice is provided to individuals
		 under this section, notice of a breach shall include, to the extent possible, the following:\r\n(1) A brief description of what happened, 
		 including the date of the breach and the date of the discovery of the breach, if known. \r\n(2) A description of the types of unsecured 
		 protected health information that were involved in the breach (such as full name, Social Security number, date of birth, home address, 
		 account number, or disability code).', 0, 512)	

		 --DISASTIG
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES
		(3, 'DISA 2016 Instance
SQL6-D0-004300,
SQL6-D0-004500, 
SQL6-D0-004700, 
SQL6-D0-004800, 
SQL6-D0-005500, 
SQL6-D0-005900, 
SQL6-D0-006000, 
SQL6-D0-006100, 
SQL6-D0-006200, 
SQL6-D0-006300, 
SQL6-D0-006400, 
SQL6-D0-010700, 
SQL6-D0-010800,
SQL6-D0-011100,
SQL6-D0-011200
', 'DISA Trace Audit 2016 - Instance', 807, 0)

		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(3, 'DISA 2014 Instance
SQL4-00-011900,
SQL4-00-012000,
SQL4-00-012100,
SQL4-00-012200,
SQL4-00-012300,
SQL4-00-037600,
SQL4-00-037900,
SQL4-00-037500,
SQL4-00-037600,
SQL4-00-037900,
SQL4-00-038000,
SQL4-00-011200,
SQL4-00-036200,
SQL4-00-036300,
SQL4-00-038100,
SQL4-00-034000', 'DISA Trace Audit 2014 EventIDs', 807, 0)

		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(3, 'DISA 2014 Instance 0
SQL4-00-034000', 'DISA Audit Configuration', 4, 1)
		 
		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(3, 'DISA 2012 Instance
SQL2-00-012400,
SQL2-00-009700,
SQL2-00-011800,
SQL2-00-011900,
SQL2-00-011400,
SQL2-00-012000,
SQL2-00-012100,
SQL2-00-012200,
SQL2-00-012300,
SQL2-00-014700,
SQL2-00-002300', 'DISA Trace Audit EventIDs (2012)', 807, 0)

		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(3, 'DISA 2016 Database', 'DISA Trace Audit 2016 - Database', 0, 3601)

		INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(3, 'DISA 2012 Database
SQL2-00-011200, DISA
2014 Database
SQL4-00-011200', 'DISA Trace Audit Rule', 0, 3595)

--NERC
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(4, 'CIP-007-6 4.1', 'Login Audit Level,
SQL Server Audit is
configured for
Logins', 807, 3595)	

--CIS
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(5, 'CIS 2012, 2014 5.4', 'SQL Server Audit is
configured for
Login', 515, 0)	

--SOX
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(6, 'SOX 404 Auditing', 'Assessment of
internal controls', 871, 3627)	

INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(6, 'SOX 404 CDC', 'Implement Change
Data Capture', 0, 1536)	

--FERPA
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(7, 'S99.35', 'Audit or Evaluation
Exception', 839, 0)	
		
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
		(7, 'S99.35, S99.35 (b)(1)', 'Audit or Evaluation
Exception
Protection of PII
from Education
Records', 0, 571)	
		 
END

----------------------------------------5.5.End Region-------------------------		
		
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


-------------------------------------------------------------------------------------------------------------------

	--******  update the repository schema version after applying all of the changes  ******
	UPDATE [SQLcompliance]..[Configuration] SET [sqlComplianceDbSchemaVersion]=2402,[eventsDbSchemaVersion ]=1303;        
        
END

GO
Grant Select ON [DefaultServerPropertise] TO [public]
GO
Grant Select ON [DefaultDatabasePropertise] TO [public]
GO