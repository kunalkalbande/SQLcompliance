CREATE DATABASE SQLcompliance
GO

USE SQLcompliance
GO

GRANT EXECUTE TO PUBLIC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[AgentEvents]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [AgentEvents]
GO

CREATE TABLE [AgentEvents] (
	[eventId] [int] IDENTITY (-2100000000, 1) NOT NULL ,
	[eventTime] [datetime] NULL ,
	[agentServer] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[instance] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[eventType] [smallint] NULL ,
	[details] [nvarchar](MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO

CREATE INDEX [IX_AgentEvents] ON [AgentEvents]([eventTime]) ON [PRIMARY]
GO

-- Changes to update Privileged users and Trusted Users at database level
if exists (select * from dbo.sysobjects where id = object_id(N'[RepositoryInfo]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [RepositoryInfo]
GO

CREATE TABLE [RepositoryInfo] (
	[Name] [nvarchar](64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Internal_Value] [int] NULL,
	[Character_Value] [nvarchar](MAX) NULL,
	CONSTRAINT [PK_Name] PRIMARY KEY CLUSTERED 
	(
		[Name]
	) ON [PRIMARY]
) ON [PRIMARY]
EXEC(' INSERT INTO  [SQLcompliance]..[RepositoryInfo] ([Name], [Internal_Value], [Character_Value]) VALUES (''SQLCM-5.6'', 1, ''Installed'')');
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[AgentEventTypes]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [AgentEventTypes]
GO

CREATE TABLE [AgentEventTypes] (
	[eventId] [int] NOT NULL ,
	[Name] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	 PRIMARY KEY  CLUSTERED 
	(
		[eventId]
	)  ON [PRIMARY] 
) ON [PRIMARY]
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[DatabaseObjects]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [DatabaseObjects]
GO

CREATE TABLE [DatabaseObjects] (
	[objectId] [int] IDENTITY (1, 1) NOT NULL ,
	[dbId] [int] NOT NULL ,
    [schemaName] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL default 'dbo',
	[name] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[objectType] [int] NOT NULL ,
	[id] [int] NOT NULL ,
	CONSTRAINT [PK_Tables] PRIMARY KEY  CLUSTERED 
	(
		[objectId]
	)  ON [PRIMARY] 
) ON [PRIMARY]
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[Databases]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [Databases]
GO

CREATE TABLE [Databases] (
	[dbId] [int] IDENTITY (1, 1) NOT NULL ,
	[srvId] [int] NOT NULL ,
	[srvInstance] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[sqlDatabaseId] [smallint] NULL ,
	[name] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[description] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[isSqlSecureDb] [tinyint] NULL DEFAULT (0),
	[isEnabled] [tinyint] NOT NULL DEFAULT (1),
	[timeCreated] [datetime] NULL ,
	[timeLastModified] [datetime] NULL ,
	[timeEnabledModified] [datetime] NULL ,
	[auditDDL] [tinyint] NULL DEFAULT (1),
	[auditSecurity] [tinyint] NULL DEFAULT (1),
	[auditAdmin] [tinyint] NULL DEFAULT (1),
	[auditDML] [tinyint] NULL DEFAULT (1),
	[auditSELECT] [tinyint] NULL DEFAULT (0),
	[auditFailures] [tinyint] NULL DEFAULT (0),
	[auditCaptureSQL] [tinyint] NULL DEFAULT (0),
	[auditExceptions] [tinyint] NULL DEFAULT (0),
	[auditDmlAll] [tinyint] NULL DEFAULT (1),
	[auditUserTables] [tinyint] NULL DEFAULT (1),
	[auditSystemTables] [tinyint] NULL DEFAULT (0),
	[auditStoredProcedures] [tinyint] NULL DEFAULT (0),
	[auditDmlOther] [tinyint] NULL DEFAULT (1),
	[auditBroker] [tinyint] NULL DEFAULT (0),
	[auditLogins] [tinyint] NULL DEFAULT (1),
	[auditUsersList] [nvarchar](MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[auditDataChanges] [tinyint] not null default (0),
	[auditSensitiveColumns] [tinyint] NOT NULL DEFAULT (0),
	[auditCaptureTrans] [tinyint] NULL DEFAULT (0),
	[pci] [tinyint] NOT NULL DEFAULT (0),
	[hipaa] [tinyint] NOT NULL DEFAULT (0),
	[isAlwaysOn] [tinyint] NULL DEFAULT (0),
	[isPrimary] [tinyint] NULL DEFAULT (0),
	[replicaServers] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[availGroupName] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[auditPrivUsersList] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
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
	[auditUserUDE] [tinyint] NULL DEFAULT (0),
	[auditUserCaptureDDL] [tinyint] NULL DEFAULT (0),
	[auditCaptureDDL] [tinyint] NULL DEFAULT (0),
	[disa] [tinyint] NOT NULL DEFAULT (0),
	[nerc] [tinyint] NOT NULL DEFAULT (0),
	[cis] [tinyint] NOT NULL DEFAULT (0),
	[sox] [tinyint] NOT NULL DEFAULT (0),
	[ferpa] [tinyint] NOT NULL DEFAULT (0),
	[auditLogouts] [tinyint] NULL DEFAULT (1),
	[auditUserLogouts] [tinyint] NULL DEFAULT (0), -- SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
	[gdpr] [tinyint] NOT NULL DEFAULT (0),
	[auditSensitiveColumnActivity] [tinyint] NOT NULL DEFAULT (0), -- SQLCM-5471 v5.6
	[auditSensitiveColumnActivityDataset] [tinyint] NOT NULL DEFAULT (0) 
	CONSTRAINT [PK_Databases] PRIMARY KEY  CLUSTERED 
	(
		[dbId]
	)  ON [PRIMARY] 
) ON [PRIMARY]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[EventTypes]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [EventTypes]
GO

CREATE TABLE [EventTypes] (
	[evtypeid] [int] NOT NULL ,
	[evcatid] [int] NULL CONSTRAINT [DF_EventTypes_evcatid] DEFAULT (2),
	[name] [nvarchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[category] [nvarchar] (16) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[is2005Only] [tinyint] NULL DEFAULT (0),
	[isExcludable] [tinyint] NULL DEFAULT (0)
	CONSTRAINT [PK_EventTypes] PRIMARY KEY  CLUSTERED 
	(
		[evtypeid]
	)  ON [PRIMARY] 
) ON [PRIMARY]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[Jobs]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [Jobs]
GO

CREATE TABLE [Jobs] (
	[jobId] [int] IDENTITY (-2100000000, 1) NOT NULL ,
	[instance] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[dateReceived] [datetime] NULL ,
	[state] [int] NULL CONSTRAINT [DF_Jobs_state] DEFAULT (0),
	[tempTablePrefix] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[compressedTraceFile] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[traceChecksum] [int] NULL ,
	[compressedAuditFile] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[auditChecksum] [int] NULL ,
	[privilegedUserTrace] [tinyint] NULL DEFAULT (0),
	[sqlSecureTrace] [tinyint] NULL DEFAULT (0),
	[aborting] [tinyint] NULL DEFAULT (0),
	CONSTRAINT [PK_Jobs] PRIMARY KEY  CLUSTERED 
	(
		[jobId]
	)  ON [PRIMARY] 
) ON [PRIMARY]
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[ObjectTypes]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [ObjectTypes]
GO

CREATE TABLE [ObjectTypes] (
	[objtypeId] [int] NOT NULL ,
	[name] [nvarchar] (32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	CONSTRAINT [PK_ObjectTypes] PRIMARY KEY  CLUSTERED 
	(
		[objtypeId]
	)  ON [PRIMARY] 
) ON [PRIMARY]
GO



if exists (select * from dbo.sysobjects where id = object_id(N'[ChangeLog]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [ChangeLog]
GO

CREATE TABLE [ChangeLog] (
	[logId] [int] IDENTITY (-2100000000, 1) NOT NULL ,
	[eventTime] [datetime] NOT NULL ,
	[logType] [int] NOT NULL ,
	[logUser] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[logSqlServer] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[logInfo] [nvarchar](MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO   

CREATE  INDEX [IX_ChangeLog] ON [ChangeLog]([eventTime]) ON [PRIMARY]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[ChangeLogEventTypes]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [ChangeLogEventTypes]
GO

CREATE TABLE [ChangeLogEventTypes] (
	[eventId] [int] NOT NULL ,
	[Name] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	CONSTRAINT [PK_LogEventTypes] PRIMARY KEY  CLUSTERED 
	(
		[eventId]
	)  ON [PRIMARY] 
) ON [PRIMARY]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[Servers]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [Servers]
GO

CREATE TABLE [Servers] (
	[srvId] [int] IDENTITY (1, 1) NOT NULL ,
	[instance] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[sqlVersion] [int] NULL ,
	[instanceServer] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[description] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
    [bias] [int] NULL DEFAULT(0),
    [standardBias] [int] NULL DEFAULT(0),
    [standardDate] [datetime] NULL,
    [daylightBias] [int] NULL DEFAULT(0),
    [daylightDate] [datetime] NULL,
	[isAuditedServer] [tinyint] NULL DEFAULT (1),
	[isDeployed] [tinyint] NULL DEFAULT (0),
	[isDeployedManually] [tinyint] NULL DEFAULT (0),
	[isUpdateRequested] [tinyint] NULL DEFAULT (0),
	[isRunning] [tinyint] NULL DEFAULT (0),
	[isCrippled] [tinyint] NULL DEFAULT (0),
	[isEnabled] [tinyint] NULL DEFAULT (1),
	[isSqlSecureDb] [tinyint] NULL DEFAULT (0),
	[isOnRepositoryHost] [tinyint] NULL DEFAULT (0),
	[timeCreated] [datetime] NULL ,
	[timeLastModified] [datetime] NULL ,
	[timeEnabledModified] [datetime] NULL ,
	[timeStartup] [datetime] NULL ,
	[timeShutdown] [datetime] NULL ,
	[timeLastAgentContact] [datetime] NULL ,
	[timeLastHeartbeat] [datetime] NULL ,
	[timeLastCollection] [datetime] NULL ,
    [timeLastArchive] [datetime] NULL,
    [lastArchiveResult] [int] NULL DEFAULT(0),
    [timeLastIntegrityCheck] [datetime] NULL,
    [lastIntegrityCheckResult] [int] NULL DEFAULT(0),
	[eventDatabase] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
    [defaultAccess] [int] NULL DEFAULT(2),
    [maxSqlLength] [int] NULL DEFAULT(512),
	[configVersion] [int] NULL DEFAULT (0),
	[lastKnownConfigVersion] [int] NULL DEFAULT (0),
	[lastConfigUpdate] [datetime] NULL ,
	[configUpdateRequested] [tinyint] NULL DEFAULT (0),
	[auditLogins] [tinyint] NULL DEFAULT (0),
	[auditFailedLogins] [tinyint] NULL DEFAULT (0),
	[auditDDL] [tinyint] NULL DEFAULT (1),
	[auditSecurity] [tinyint] NULL DEFAULT (1),
	[auditAdmin] [tinyint] NULL DEFAULT (0),
	[auditDML] [tinyint] NULL DEFAULT (0),
	[auditSELECT] [tinyint] NULL DEFAULT (0),
	[auditTrace] [tinyint] NULL DEFAULT (0),
	[auditExceptions] [tinyint] NULL DEFAULT (0),
	[auditFailures] [tinyint] NULL DEFAULT (0),
	[auditCaptureSQL] [tinyint] NULL DEFAULT (0),
	[auditUsers] [nvarchar] (512) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[auditUsersList] [nvarchar](MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[auditUserAll] [tinyint] NULL DEFAULT (0),
	[auditUserLogins] [tinyint] NULL DEFAULT (0),
	[auditUserFailedLogins] [tinyint] NULL DEFAULT (0),
	[auditUserDDL] [tinyint] NULL DEFAULT (1),
	[auditUserSecurity] [tinyint] NULL DEFAULT (1),
	[auditUserAdmin] [tinyint] NULL DEFAULT (1),
	[auditUserDML] [tinyint] NULL DEFAULT (0),
	[auditUserSELECT] [tinyint] NULL DEFAULT (0),
	[auditUserFailures] [tinyint] NULL DEFAULT (0),
	[auditUserCaptureSQL] [tinyint] NULL DEFAULT (0),
	[auditUserExceptions] [tinyint] NULL DEFAULT (0),
	[agentServer] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[agentPort] [int] NULL DEFAULT (5200),
	[agentVersion] [nvarchar] (43) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[agentServiceAccount] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[agentTraceDirectory] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[agentHeartbeatInterval] [int] NULL ,
	[agentCollectionInterval] [int] NULL ,
	[agentForceCollectionInterval] [int] NULL ,
	[agentLogLevel] [int] NULL DEFAULT (1),
	[agentMaxTraceSize] [int] NULL ,
	[agentMaxFolderSize] [int] NULL DEFAULT (-1),
	[agentMaxUnattendedTime] [int] NULL DEFAULT (-1),
	[agentTraceOptions] [int] NULL,
    [eventReviewNeeded] [tinyint] DEFAULT (0) NULL,
    [containsBadEvents] [tinyint] DEFAULT (0) NULL,
    [highWatermark] [int]  DEFAULT (-2100000000) NULL,
    [lowWatermark] [int] DEFAULT (-2100000000) NULL,
    [alertHighWatermark] [int]  DEFAULT (-2100000000) NULL,
    [auditDBCC] [tinyint] NULL DEFAULT(0),
	[auditSystemEvents] [tinyint] NULL DEFAULT (0),
	[auditUDE] [tinyint] NULL DEFAULT (0),
	[auditUserUDE] [tinyint] NULL DEFAULT (0),
	[agentDetectionInterval] [int]  NULL DEFAULT (60),
	[agentHealth] [bigint] DEFAULT(0) NULL,
	[agentTraceStartTimeout] [int] NULL,
	[auditUserCaptureTrans] [tinyint] NULL DEFAULT (0),
	[instancePort] [int] NULL,
	[versionName] [nvarchar] (100) NULL,
	[isSynchronized] [bit] NOT NULL DEFAULT (0),	
	[authentication_type] [int] not null default(0),
	[user_account] nvarchar(128) null,
	[password] nvarchar(256) null,
	[owner] [nvarchar](256) NULL,
	[location] [nvarchar](256) NULL,
	[comments] [nvarchar](max) NULL,
	[auditUserCaptureDDL] [tinyint] NULL DEFAULT (0),
	[isCluster] [tinyint] NULL,
	[isHadrEnabled] [tinyint] NULL,
	[auditUserExtendedEvents] [tinyint] NULL DEFAULT (0),
	[auditCaptureSQLXE] [tinyint] NULL DEFAULT (0),
	[isAuditLogEnabled] [tinyint] NULL DEFAULT (0),
	[auditLogouts] [tinyint] NULL DEFAULT (0),
	[auditUserLogouts] [tinyint] NULL DEFAULT (0), -- SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
	[auditTrustedUsersList] [nvarchar](MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[countDatabasesAuditingAllObjects] [int] NOT NULL DEFAULT (0), --SQLcm 5.6 (Aakash Prakash) - Fix for 4967	
	[userAppliedRegulations] [int] NOT NULL DEFAULT (0), --Fix for 5241
	[addNewDatabasesAutomatically] [tinyint] NOT NULL DEFAULT (0), -- SQLCM -541/4876/5259 v5.6
	[lastNewDatabasesAddTime] [datetime] NULL,
	[isCompressedFile] [tinyint] NULL DEFAULT (1),
	[isRowCountEnabled] [tinyint] NULL DEFAULT (0)
	CONSTRAINT [PK_Servers] PRIMARY KEY  CLUSTERED 
	(
		[instance]
	)  ON [PRIMARY] ,
	CONSTRAINT [IX_RegisteredServers] UNIQUE  NONCLUSTERED 
	(
		[srvId],
		[instance]
	)  ON [PRIMARY]
) ON [PRIMARY]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[SystemDatabases]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [SystemDatabases]
GO

CREATE TABLE [SystemDatabases] (
	[instance] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[databaseName] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[databaseType] [nvarchar] (8) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[dateCreated] [datetime] NULL ,
	[sqlDatabaseId] [smallint] NULL,
	[displayName] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[description] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	CONSTRAINT [PK_SystemDatabase] PRIMARY KEY  CLUSTERED 
	(
		[databaseName]
	)  ON [PRIMARY] 
) ON [PRIMARY]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[Configuration]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [Configuration]
GO

CREATE TABLE [Configuration] (
	[timeLastModified] [datetime] NULL ,
	[warnNoHeartbeatWarning] [tinyint] NULL ,
	[auditLogins] [tinyint] NULL  DEFAULT (0),
	[auditFailedLogins] [tinyint] NULL  DEFAULT (0),
	[auditDDL] [tinyint] NULL DEFAULT (1),
	[auditSecurity] [tinyint] NULL DEFAULT (1),
	[auditAdmin] [tinyint] NULL DEFAULT (1),
	[auditDML] [tinyint] NULL DEFAULT (0),
	[auditSELECT] [tinyint] NULL DEFAULT (0),
	[auditFailures] [tinyint] NULL DEFAULT (0),
	[licenseKey] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[auditDmlAll] [tinyint] NULL DEFAULT (0),
	[auditUserTables] [tinyint] NULL DEFAULT (0),
	[auditSystemTables] [tinyint] NULL DEFAULT (0),
	[auditStoredProcedures] [tinyint] NULL DEFAULT (0),
	[auditDmlOther] [tinyint] NULL DEFAULT (0),
	[auditUserAll] [tinyint] NULL DEFAULT (0),
	[auditUserLogins] [tinyint] NULL DEFAULT (0),
	[auditUserFailedLogins] [tinyint] NULL DEFAULT (0),
	[auditUserDDL] [tinyint] NULL DEFAULT (1),
	[auditUserSecurity] [tinyint] NULL DEFAULT (1),
	[auditUserAdmin] [tinyint] NULL DEFAULT (1),
	[auditUserDML] [tinyint] NULL DEFAULT (0),
	[auditUserSELECT] [tinyint] NULL DEFAULT (0),
	[auditUserFailures] [tinyint] NULL DEFAULT (0),
	[agentActivityLogLevel] [int] NULL DEFAULT (0),
	[serverPort] [int] NULL DEFAULT (5201),
	[agentPort] [int] NULL DEFAULT (5200),
	[server] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[sqlComplianceDbSchemaVersion] [int] NOT NULL DEFAULT (100),
	[eventsDbSchemaVersion][int] NOT NULL DEFAULT (100),
	[serverVersion] [nvarchar] (43) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[serverCoreVersion] [nvarchar] (43) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[archiveOn][int] NOT NULL DEFAULT (1),
	[archiveTimeZoneName][nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[archiveBias][int] NULL DEFAULT (0),
	[archiveStandardBias][int] NULL DEFAULT (0),
	[archiveStandardDate][datetime] NULL,
	[archiveDaylightBias][int] NULL DEFAULT (0),
	[archiveDaylightDate][datetime] NULL,
	[archiveInterval][int] NOT NULL DEFAULT (7),
	[archiveAge][int] NOT NULL DEFAULT (90),
	[archivePeriod][int] NOT NULL DEFAULT (3),
	[archivePrefix][nvarchar] (32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[archiveLastTime] [datetime] NULL ,
	[archiveNextTime] [datetime] NULL ,
	[groomLogAge][int] NOT NULL DEFAULT (60),
	[groomAlertAge][int] NOT NULL DEFAULT (60),
	[groomEventAllow][tinyint] NOT NULL DEFAULT (0),
	[groomEventAge][int] NOT NULL DEFAULT (90),
	[snapshotInterval] [int] NULL DEFAULT (0),
	[snapshotLastTime] [datetime] NULL ,
	[serverLastHeartbeatTime] [datetime] NULL ,
    [recoveryModel] [int] NULL DEFAULT (0),
    [reportingVersion] [int] NULL DEFAULT (100),
    [repositoryVersion] [int] NULL DEFAULT (100),
	[auditDBCC] [tinyint] NULL DEFAULT (0),
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
    [loginCacheSize] [int] NULL DEFAULT (1000),
	[archiveCheckIntegrity] [tinyint] NOT NULL DEFAULT (1),
	[collectionServerHeartbeatInterval] [int] NOT NULL DEFAULT (5),
	[indexStartTime] [DateTime] NULL,
	[indexDurationInSeconds] [int] NULL,
	[refactorTable] [tinyint] NULL DEFAULT (0),
	[archiveDatabaseFilesLocation] [nvarchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL, -- archive database files location
	[archiveScheduleType] [int] NULL DEFAULT (0), -- archive schedule type
	[archiveScheduleTime] datetime CONSTRAINT DF_Configuration_archiveScheduleTime DEFAULT '1-1-2014 1:30:00 AM', -- archive daily schedule time
	[archiveScheduleRepetition] [int] NULL DEFAULT(1), -- archive schedule repetition: week or month
	[archiveScheduleWeekDay]  [nvarchar] (7) COLLATE SQL_Latin1_General_CP1_CI_AS NULL DEFAULT('0000000'), -- archive schedule weekday
	[archiveScheduleDayOrWeekOfMonth] [int] NULL DEFAULT (1), -- archive schedule day of month or week of month
	[archiveScheduleExecutionPlan] [nvarchar] (MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL, --  archive schedule execution plan
	[archiveScheduleIsArchiveRunning] [tinyint] NULL DEFAULT (0), -- is scheduled archive running 
	[snmpServerAddress] [nvarchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL, -- SNMP server address
	[snmpPort] [int] NULL DEFAULT (162), -- SNMP server port
	[snmpCommunity] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL DEFAULT ('public'), -- SNMP community
	[alwaysOnRoleUpdateInterval] [int] NOT NULL DEFAULT (10),
	[auditLogouts] [tinyint] NULL  DEFAULT (0),
	[auditUserLogouts] [tinyint] NULL DEFAULT (0),	-- SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
) ON [PRIMARY]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[Reports]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [Reports] 
GO

CREATE TABLE [Reports] (
	[reportServer] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[serverVirtualDirectory] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[managerVirtualDirectory] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[port] [int] NULL ,
	[useSsl] [tinyint] NULL ,
	[userName] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[repository] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[targetDirectory] [nvarchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

-- Alerting Support

CREATE TABLE [dbo].[ActionResultStatusTypes] (
	[statusTypeId] [int] NOT NULL ,
	[name] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	CONSTRAINT [PK_ActionResultStatusTypes] PRIMARY KEY  CLUSTERED 
	(
		[statusTypeId]
	)  ON [PRIMARY] 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AlertTypes] (
	[alertTypeId] [int] NOT NULL ,
	[name] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	CONSTRAINT [PK_AlertTypes] PRIMARY KEY  CLUSTERED 
	(
		[alertTypeId]
	)  ON [PRIMARY] 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Alerts] (
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
	[computerName] [nvarchar](256) NULL ,
	[snmpTrapStatus] [tinyint] NOT NULL,
	[isDismissed] [bit] NULL,
	[alertEventType] [int] NOT NULL DEFAULT(0),
	CONSTRAINT [PK_Alerts] PRIMARY KEY  CLUSTERED 
	(
		[alertId]
	)  ON [PRIMARY] ,
	CONSTRAINT [FK_Alerts_AlertTypes] FOREIGN KEY 
	(
		[alertType]
	) REFERENCES [dbo].[AlertTypes] (
		[alertTypeId]
	)
) ON [PRIMARY]
GO

CREATE  INDEX [IX_Alerts_created] ON [dbo].[Alerts]([created] DESC, [alertId] DESC ) ON [PRIMARY]
GO
CREATE  INDEX [IX_Alerts_alertLevel] ON [dbo].[Alerts]([alertLevel], [alertId] DESC ) ON [PRIMARY]
GO
CREATE  INDEX [IX_Alerts_eventType] ON [dbo].[Alerts]([eventType], [alertId] DESC ) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AlertRules] (
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
	[snmpTrap] [tinyint] NOT NULL,
	[snmpServerAddress] [nvarchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL, -- SNMP server address
	[snmpPort] [int] NULL DEFAULT (162), -- SNMP server port
	[snmpCommunity] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL DEFAULT ('public'), -- SNMP community
	[isAlertRuleTimeFrameEnabled] [tinyint],
	[alertRuleTimeFrameStartTime] [time] (7),
	[alertRuleTimeFrameEndTime] [time] (7),
    [alertRuleTimeFrameDaysOfWeek] [nvarchar] (7) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [emailSummaryMessage] [tinyint]	,
    [summaryEmailFrequency] [int],
	[lastEmailSummarySendTime] [datetime],
	CONSTRAINT [PK_AlertRules] PRIMARY KEY  CLUSTERED 
	(
		[ruleId]
	)  ON [PRIMARY] ,
	CONSTRAINT [FK_AlertRules_AlertTypes] FOREIGN KEY 
	(
		[alertType]
	) REFERENCES [dbo].[AlertTypes] (
		[alertTypeId]
	)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AlertRuleConditions] (
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
	) REFERENCES [dbo].[AlertRules] (
		[ruleId]
	)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[StatusRuleTypes] (
   [StatusRuleId] [int] NOT NULL,
   [RuleName] [nvarchar](100) NOT NULL
   CONSTRAINT [PK_StatusRuleId] PRIMARY KEY  CLUSTERED 
   (
	   [StatusRuleId]
   )  
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DataRuleTypes] (
   [DataRuleId] [int] NOT NULL,
   [RuleName] [nvarchar](100) NOT NULL
   CONSTRAINT [PK_DataRuleId] PRIMARY KEY  CLUSTERED 
   (
	   [DataRuleId]
   )  
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[EventFilters] (
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
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[EventFilterConditions] (
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
) ON [PRIMARY]
GO

CREATE TABLE [StatsCategories] ( [category] [int] NOT NULL, [name] [nvarchar] (64) NOT NULL )
GO

CREATE TABLE [dbo].[ReportCard](
	[srvId] [int] NOT NULL,
	[statId] [int] NOT NULL,
	[warningThreshold] [int] NOT NULL,
	[errorThreshold] [int] NOT NULL,
	[period] [int] NOT NULL,
	[enabled] [tinyint] NOT NULL
) ON [PRIMARY]
GO

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
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Licenses] (
    [licenseid] INTEGER IDENTITY(1,1) NOT NULL,
    [licensekey] NVARCHAR(256) NOT NULL,
    [createdby] NVARCHAR(500) NOT NULL,
    [createdtm] DATETIME NOT NULL,
    CONSTRAINT [PK__applicationlicen__46E78A0C] PRIMARY KEY ([licenseid])
)
GO

-- the table that holds the tables being monitored for data changes accross all instances
CREATE TABLE DataChangeTables ( srvId int not null,
								dbId int not null,
								objectId int not null,
                        schemaName nVarchar(128) not null,
								tableName nVarchar(128) not null,
								rowLimit int not null default (20),
								selectedColumns tinyint not null default 1,
								CONSTRAINT [PK_DataChangeTables] PRIMARY KEY CLUSTERED (srvId, dbId, objectId )  )
	on [PRIMARY]
go

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
) ON [PRIMARY]
go

-- the table that holds the tables being monitored for sensitive column accesses accross all instances
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
go

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
go

CREATE TABLE [dbo].[Regulation](
	[regulationId] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar] (512) NOT NULL,
	[description] [nvarchar] (2048) NULL,
	CONSTRAINT [PK_Regulation] PRIMARY KEY CLUSTERED
	(
		[regulationId]
	)
)	ON [PRIMARY]
go

CREATE TABLE [dbo].[RegulationMap] (
	[regulationId] [int] NOT NULL,
	[section] [nvarchar] (256) NOT NULL,
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
go

CREATE TABLE [dbo].[Cwf](
	[CwfUrl] [nvarchar](255) NOT NULL,
	[CwfToken] [nvarchar](255) NOT NULL,
	CONSTRAINT [PK_Cwf] PRIMARY KEY CLUSTERED 
	(
		[CwfUrl] ASC
	)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[LoginAccounts](
	[Name] [nvarchar](128) NOT NULL,
	[SID] [varbinary](85) NULL,
	[WebApplicationAccess] [bit] NOT NULL CONSTRAINT [DF_Logins_IsHavingWebApplicationAccess]  DEFAULT ((1)),
 CONSTRAINT [PK_Logins] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[ViewSettings](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](256) NOT NULL,
	[ViewId] [nvarchar](256) NULL,
	[ViewName] [nvarchar](256) NULL,
	[Timeout] [int] NULL,
	[Filter] [nvarchar](MAX) NULL,
 CONSTRAINT [PK_ViewSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
CREATE TABLE [dbo].[UserSettings](
	[DashboardUserId] [int] NOT NULL,
	[Account] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](230) NULL,
	[SessionTimout] [int] NULL,
	[Subscribed] [bit] NOT NULL DEFAULT(0)
) ON [PRIMARY]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[WebRefreshDuration]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [WebRefreshDuration]
GO 
CREATE TABLE [dbo].[WebRefreshDuration]( 
[Id] [int] NOT NULL,
[RefreshDuration] [int] NOT NULL,
[isUpdated] [varchar](1) NOT NULL)
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[SensitiveColumnProfiler]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [SensitiveColumnProfiler]
GO 
CREATE TABLE [dbo].[SensitiveColumnProfiler](
 [profileName] [varchar](50) NOT NULL,
 [category] [varchar](50) NOT NULL,
 [searchStringName] [varchar](50) NOT NULL,
 [definition] [varchar](50) NOT NULL,
 [isStringChecked] [varchar](50) NOT NULL,
 [isProfileActive] [varchar](50) NOT NULL
) ON [PRIMARY]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[SensitiveColumnStrings]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [SensitiveColumnStrings]
GO 
CREATE TABLE [dbo].[SensitiveColumnStrings](
 [category] [varchar](50) NOT NULL,
 [searchStringName] [varchar](50) NOT NULL,
 [definition] [varchar](50) NOT NULL,
 [isStringChecked] [varchar](50) NOT NULL
) ON [PRIMARY]
GO

-- 5.1.4.1 Limiting User Access to Specific Reports --
CREATE TABLE [dbo].[AuditReports](
	[reportname] [nvarchar](max) NOT NULL,
	[rid] [int] IDENTITY(1,1) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[rid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
CREATE TABLE [dbo].[Logins](
	[name] [nvarchar](128) NOT NULL,
	[uid] [int] IDENTITY(1,1) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[ReportAccess](
	[rid] [int] NOT NULL,
	[uid] [int] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ReportAccess]  WITH CHECK ADD  CONSTRAINT [FK_AuditReport] FOREIGN KEY([rid])
REFERENCES [dbo].[AuditReports] ([rid])
GO
ALTER TABLE [dbo].[ReportAccess] CHECK CONSTRAINT [FK_AuditReport]
GO
ALTER TABLE [dbo].[ReportAccess]  WITH CHECK ADD  CONSTRAINT [FK_UserLogin] FOREIGN KEY([uid])
REFERENCES [dbo].[Logins] ([uid])
GO
ALTER TABLE [dbo].[ReportAccess] CHECK CONSTRAINT [FK_UserLogin]
GO
-- 5.1.4.1 Limiting User Access to Specific Reports --

Create TABLE [dbo].[Users] (
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[serverId] [int] NULL,
	[databaseId] [int] NULL,
	[isTrusted] [bit] NULL,
	[isPrivileged] [bit] NULL,
	[roleName] [nvarchar](256) NULL,
	[loginName] [nvarchar](256) NULL,
	[createdDate] [datetime] NULL,
	[isServerLevel] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [serverId]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [databaseId]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [isTrusted]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [isPrivileged]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (NULL) FOR [roleName]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (NULL) FOR [loginName]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [isServerLevel]
GO
EXECUTE sp_grantdbaccess 'guest'
GO
GRANT SELECT ON [AgentEvents] TO [public]
GO
GRANT SELECT ON [RepositoryInfo] TO [public]
GO
GRANT SELECT ON [AgentEventTypes] TO [public]
GO
GRANT SELECT ON [DatabaseObjects] TO [public]
GO
GRANT SELECT ON [Databases] TO [public]
GO
GRANT SELECT ON [EventTypes] TO [public]
GO
GRANT SELECT ON [Jobs] TO [public]
GO
GRANT SELECT ON [ObjectTypes] TO [public]
GO
GRANT SELECT ON [ChangeLog] TO [public]
GO
GRANT SELECT ON [ChangeLogEventTypes] TO [public]
GO
GRANT SELECT ON [Servers] TO [public]
GO
GRANT SELECT ON [SystemDatabases] TO [public]
GO
GRANT SELECT ON [Configuration] TO [public]
GO
GRANT SELECT ON [Reports] TO [public]
GO
GRANT SELECT ON [ActionResultStatusTypes] TO [public]
GO
GRANT SELECT ON [AlertTypes] TO [public]
GO
GRANT SELECT ON [AlertRules] TO [public]
GO
GRANT SELECT ON [AlertRuleConditions] TO [public]
GO
GRANT SELECT ON [Alerts] TO [public]
GO
GRANT SELECT ON [StatusRuleTypes] TO [public]
Go
GRANT SELECT ON [DataRuleTypes] TO [public]
Go
GRANT SELECT ON [EventFilterConditions] TO [public]
GO
GRANT SELECT ON [EventFilters] TO [public]
GO
GRANT SELECT ON [StatsCategories] TO [public]
GO
GRANT SELECT ON [ReportCard] TO [public]
GO
GRANT SELECT ON [ThresholdConfiguration] TO [public]
GO
GRANT SELECT ON [Licenses] TO [public]
GO
GRANT SELECT ON [DataChangeTables] TO [public]
GO
GRANT SELECT ON [DataChangeColumns] TO [public]
GO
GRANT SELECT ON [SensitiveColumnTables] TO [public]
GO
GRANT SELECT ON [SensitiveColumnColumns] TO [public]
GO
GRANT SELECT ON [Regulation] TO [public]
GO
GRANT SELECT ON [RegulationMap] TO [public]
GO
GRANT SELECT ON [LoginAccounts] TO [public]
GO
--SQLCM-5804: security flaw defect
--GRANT UPDATE ON [LoginAccounts] TO [public]
--GO
GRANT DELETE ON [LoginAccounts] TO [public]
GO
GRANT SELECT ON [WebRefreshDuration] TO [public]
GO
GRANT UPDATE ON [WebRefreshDuration] TO [public]
GO
GRANT DELETE ON [WebRefreshDuration] TO [public]
GO

GRANT SELECT ON [SensitiveColumnStrings] TO [public]
GO
--SQLCM-5804: security flaw defect
--GRANT UPDATE ON [SensitiveColumnStrings] TO [public]
--GO
GRANT DELETE ON [SensitiveColumnStrings] TO [public]
GO

GRANT SELECT ON [SensitiveColumnProfiler] TO [public]
GO
--SQLCM-5804: security flaw defect
--GRANT UPDATE ON [SensitiveColumnProfiler] TO [public]
--GO
GRANT DELETE ON [SensitiveColumnProfiler] TO [public]
GO
 
-- 5.1.4.1 Limiting User Access to Specific Reports
GRANT SELECT ON [Logins] TO [public]
GO

GRANT SELECT ON [AuditReports] TO [public]
GO

GRANT SELECT ON [ReportAccess] TO [public]
GO
GRANT SELECT ON [Users] TO [public]
GO

--5.1.4.1 Limiting User Access to Specific Reports -->

CREATE DATABASE [SQLcomplianceProcessing]
GO

ALTER DATABASE [SQLcomplianceProcessing] SET RECOVERY SIMPLE
GO

CREATE TABLE [SQLcomplianceProcessing]..[TraceTimes] (
	[instance] [nvarchar] (128) NULL ,
	[traceType] [int] NULL ,
	[startTime] [datetime] NULL ,
	[endTime] [datetime] NULL ,
	[updated] [datetime] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [SQLcomplianceProcessing]..[TraceStates] (
	[instance] [nvarchar] (128) NULL ,
	[traceType] [int] NULL ,
	[state] [int] NULL ,
	[updated] [datetime] NULL ,
	[flushed] [int] NULL ,
	[flushedEventId] [int] NULL ,
	[EventClass] [int] NULL ,
	[EventSubClass] [int] NULL ,
	[StartTime] [datetime] NULL ,
	[SPID] [int] NULL ,
	[ApplicationName] [nvarchar] (128)  NULL ,
	[HostName] [nvarchar] (128) NULL ,
	[ServerName] [nvarchar] (128) NULL ,
	[LoginName] [nvarchar] (128) NULL ,
	[Success] [int] NULL ,
	[DatabaseName] [nvarchar] (128) NULL ,
	[DatabaseID] [int] NULL ,
	[DBUserName] [nvarchar] (128) NULL ,
	[ObjectType] [int] NULL ,
	[ObjectName] [nvarchar] (128) NULL ,
	[ObjectID] [int] NULL ,
	[Permissions] [int] NULL ,
	[ColumnPermissions] [int] NULL ,
	[TargetLoginName] [nvarchar] (128) NULL ,
	[TargetUserName] [nvarchar] (128) NULL ,
	[RoleName] [nvarchar] (128) NULL ,
	[OwnerName] [nvarchar] (128) NULL ,
	[TextData] [nvarchar](MAX) NULL ,
    [NestLevel] [int] NULL,
	[FileName]         [nvarchar] (128) NULL ,
	[LinkedServerName] [nvarchar] (128) NULL ,
	[ParentName]       [nvarchar] (128) NULL ,
	[IsSystem]         [int] NULL ,
	[SessionLoginName] [nvarchar] (128) NULL ,
	[ProviderName]     [nvarchar] (128) NULL,
	[eventSequence]    [bigint] NULL,
	[RowCounts]		   [bigint] NULL,
	[guid]			   [nvarchar] (100) NULL

) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [SQLcomplianceProcessing]..[TraceLogins] (
	[instance] [nvarchar] (128) NULL ,
	[loginKey] [int] NULL ,
	[loginValue] [datetime] NULL
) ON [PRIMARY]
GO

USE SQLcompliance
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name]) 
	VALUES (1,'Heartbeat received')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (2,'Started agent')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (3,'Shutdown agent')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (4,'Update audit settings')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (5,'Received event data')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (6,'Registered new agent')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (7,'Unregistered agent')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (8,'Message received from unregistered instance')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (9,'Agent alert')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (10,'Agent error')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (11,'Agent warning')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (12,'First time update')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (13,'Agent suspended')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (14,'Agent resume')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (15,'Undeployed')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (16,'Deployed manually')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (17,'Incompatible SQL Server version error')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (18,'Audit trace stopped warning')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (19,'Audit trace closed warning')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (20,'Audit trace altered warning')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (1001,'Agent Warning')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (3001,'Agent Warning Resolution')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (2001,'Agent Configuration Error')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (4001,'Agent Configuration Resolution')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (2002,'Trace Directory Error')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (4002,'Trace Directory Resolution')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (2003,'SQL Trace Error')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (4003,'SQL Trace Resolution')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (2004,'Server Connection Error')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (4004,'Server Connection Resolution')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (2005,'Collection Service Connection Error')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (4005,'Collection Service Connection Resolution')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (2006,'CLR Error')
GO

INSERT INTO [AgentEventTypes] ([eventId],[Name])
	VALUES (4006,'CLR Resolution')
GO

INSERT INTO [WebRefreshDuration] ([Id],[RefreshDuration],[isUpdated])
	VALUES (1,30,0)
GO

INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Dates','Birth Date','%date%,%dob%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Dates','Generic Date','%dob%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Email','Email Address','%email%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Financial','Code','%code%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Financial','Credit Card Number','%credit%,%card%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Financial','Income','%income%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Financial','Pin Number','%pin%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Financial','Salary','%salary%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Financial','Tax','%tax%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Geographic','Address','%address%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Geographic','City','%city%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Geographic','Country','%country%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Geographic','County','%county%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Geographic','Parish','%parish%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Geographic','Post Code','%post%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Geographic','Precinct','%precinct%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Geographic','State','%state%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Geographic','Township','%township%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Geographic','Zip Code','%zip%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Medical','Beneficiary','%beneficiary%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Medical','Health','%health%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Medical','Patient','%patient%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Medical','Treatment','%treatment%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Names','First','%first%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Names','Last','%last%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Names','Name','%name%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Names','Name of emp','%name%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Numbers','Account','%account%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Numbers','Billing','%billing%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Numbers','Certificate','%certificate%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Numbers','Drivers License Number','%license%,%dln%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Numbers','License Plate','%license,%plate',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Numbers','Record Number','%record%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Numbers','Serial Number','%serial%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Numbers','Vehicle Identification Number','%vin%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Other','Authority','%authority%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Other','Nationality','%nationality%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Other','Sex','%sex%,%gender%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Social Security Number','Social Security Number','%social%,%ssn%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Telephone/Fax','Fax Number','%fax%,%facsimile%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Telephone/Fax','Telephone Number','%phone%,%cell%,%mobile%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Web/Network','IP Address','%ip%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Web/Network','Login','%login%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Web/Network','Password','%password%',0)
GO
INSERT INTO [SQLcompliance].[dbo].[SensitiveColumnStrings] ([category],[searchStringName],[definition],[isStringChecked])
	VALUES ('Web/Network','URL','%url%',0)
GO
  

-----------------------------------------------

INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (-1,-1,'Invalid','Invalid',0,0)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1,5,'Select','Select',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (2,4,'Update','DML',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (4,5,'References','Select',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (8,4,'Insert','DML',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (16,4,'Delete','DML',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (32,4,'Execute','DML',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (40,4,'Begin Transaction','DML',0,1)
Go
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (41,4,'Commit Transaction','DML',0,1)
Go
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (42,4,'Rollback Transaction','DML',0,1)
Go
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (43,4,'Save Transaction','DML',0,1)
Go
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (50,1,'Login','Login',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (51,1,'Login Failed','Login',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (52,1,'Server impersonation','Login',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (53,1,'Database impersonation','Login',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (54,3,'Disable login','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (55,3,'Enable login','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (62,6,'Backup','Admin',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (63,6,'Restore','Admin',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (64,6,'Backup database','Admin',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (65,6,'Backup log','Admin',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (66,6,'Backup Table','Admin',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (71,6,'Trace started','Trace',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (72,6,'Trace stopped','Trace',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (80,6,'DBCC','Admin',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (81,6,'DBCC - read','Admin',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (82,6,'DBCC - write','Admin',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (90,6,'Server operation','Admin',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (91,6,'Database operation','Admin',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (92,6,'Server alter trace','Admin',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (93,8,'Broker Conversation','Broker',1,0)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (94,8,'Broker Login','Broker',1,0)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (100,2,'Create','DDL',0,0)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (101,2,'Create index','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (102,2,'Create database','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (103,2,'Create user object','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (105,2,'Create DEFAULT','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (108,2,'Create stored procedure','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (109,2,'Create UDF','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (110,2,'Create rule','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (111,2,'Create repl filter stored proc','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (113,2,'Create trigger','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (114,2,'Create inline function','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (115,2,'Create table valued UDF','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (117,2,'Create user table','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (118,2,'Create view','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (119,2,'Create ext stored proc','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (122,2,'Create statistics','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (125,2,'Create Server Trigger','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (126,2,'Create CLR Trigger','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (127,2,'Create Full Text Catalog','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (128,2,'Create CLR Stored Procedure','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (129,2,'Create Schema','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (130,3,'Create Credential','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (131,2,'Create DDL Event','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (132,2,'Create Management Event','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (133,2,'Create Security Event','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (134,2,'Create User Event','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (135,2,'Create CLR Aggregate Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (136,2,'Create Partition Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (137,2,'Create Table Valued SQL Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (139,3,'Create Microsoft Windows Group','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (140,2,'Create Asymmetric Key','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (141,2,'Create Master Key','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (142,2,'Create Symmetric Key','DDL',1,0)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (143,3,'Create Asymmetric Key Login','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (144,3,'Create Certificate Login','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (145,3,'Create Role','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (146,3,'Create SQL Login','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (147,3,'Create Windows Login','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (148,2,'Create Remote Service Binding','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (149,2,'Create Event Notification On Database','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (150,2,'Create Event Notification','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (151,2,'Create Scalar SQL Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (152,2,'Create Event Notification On Object','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (153,2,'Create Synonym','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (154,2,'Create End Point','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (155,2,'Create Service Broker Service Queue','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (156,3,'Create Application Role','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (157,3,'Create Certificate','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (159,2,'Create Transact SQL Trigger','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (160,2,'Create Assembly','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (161,2,'Create CLR Scalar Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (162,2,'Create Partition Scheme','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (163,2,'Create User Object','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (164,2,'Create Service Broker Service Contract','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (165,2,'Create CLR Table Valued Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (167,2,'Create Service Broker Message Type','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (168,2,'Create Service Broker Route','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (169,3,'Create User','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (170,2,'Create Service Broker Service','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (172,2,'Create XML Schema','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (173,2,'Create Type','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (200,2,'Alter','DDL',0,0)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (201,2,'Alter index','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (202,2,'Alter database','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (203,2,'Alter user object','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (205,2,'Alter DEFAULT','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (208,2,'Alter stored procedure','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (209,2,'Alter UDF','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (210,2,'Alter rule','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (211,2,'Alter repl filter stored proc','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (212,2,'Alter system table','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (213,2,'Alter trigger','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (214,2,'Alter inline function','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (215,2,'Alter table valued UDF','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (217,2,'Alter user table','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (218,2,'Alter view','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (222,2,'Alter statistics','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (225,2,'Alter Server Trigger','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (226,2,'Alter CLR Trigger','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (227,2,'Alter Full Text Catalog','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (228,2,'Alter CLR Stored Procedure','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (229,2,'Alter Schema','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (230,3,'Alter Credential','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (235,2,'Alter CLR Aggregate Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (236,2,'Alter Partition Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (237,2,'Alter Table Valued SQL Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (240,2,'Alter Asymmetric Key','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (241,2,'Alter Master Key','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (242,2,'Alter Symmetric Key','DDL',1,0)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (245,3,'Alter Role','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (251,2,'Alter Scalar SQL Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (254,2,'Alter End Point','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (255,2,'Alter Service Broker Service Queue','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (256,3,'Alter Application Role','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (257,3,'Alter Certificate','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (259,2,'Alter Transact SQL Trigger','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (260,2,'Alter Assembly','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (261,2,'Alter CLR Scalar Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (262,2,'Alter Partition Scheme','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (263,2,'Alter User Object','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (265,2,'Alter CLR Table Valued Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (267,2,'Alter Service Broker Message Type','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (268,2,'Alter Service Broker Route','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (269,3,'Alter User','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (270,2,'Alter Service Broker Service','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (272,2,'Alter XML Schema','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (273,2,'Alter Type','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (300,2,'Drop','DDL',0,0)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (301,2,'Drop index','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (302,2,'Drop database','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (303,2,'Drop user object','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (305,2,'Drop DEFAULT','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (308,2,'Drop stored procedure','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (309,2,'Drop UDF','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (310,2,'Drop rule','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (311,2,'Drop repl filter stored proc','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (313,2,'Drop trigger','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (314,2,'Drop inline function','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (315,2,'Drop table valued UDF','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (317,2,'Drop user table','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (318,2,'Drop view','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (319,2,'Drop ext stored proc','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (322,2,'Drop statistics','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (325,2,'Drop Server Trigger','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (326,2,'Drop CLR Trigger','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (327,2,'Drop Full Text Catalog','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (328,2,'Drop CLR Stored Procedure','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (329,2,'Drop Schema','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (330,3,'Drop Credential','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (331,2,'Drop DDL Event','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (332,2,'Drop Management Event','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (333,2,'Drop Security Event','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (334,2,'Drop User Event','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (335,2,'Drop CLR Aggregate Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (336,2,'Drop Partition Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (337,2,'Drop Table Valued SQL Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (339,3,'Drop Microsoft Windows Group','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (340,2,'Drop Asymmetric Key','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (341,2,'Drop Master Key','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (342,2,'Drop Symmetric Key','DDL',1,0)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (343,3,'Drop Asymmetric Key Login','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (344,3,'Drop Certificate Login','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (345,3,'Drop Role','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (346,3,'Drop SQL Login','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (347,3,'Drop Windows Login','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (348,2,'Drop Remote Service Binding','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (349,2,'Drop Event Notification On Database','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (350,2,'Drop Event Notification','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (351,2,'Drop Scalar SQL Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (352,2,'Drop Event Notification On Object','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (353,2,'Drop Synonym','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (354,2,'Drop End Point','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (355,2,'Drop Service Broker Service Queue','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (356,3,'Drop Application Role','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (357,3,'Drop Certificate','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (359,2,'Drop Transact SQL Trigger','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (360,2,'Drop Assembly','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (361,2,'Drop CLR Scalar Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (362,2,'Drop Partition Scheme','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (363,2,'Drop User Object','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (364,2,'Drop Service Broker Service Contract','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (365,2,'Drop CLR Table Valued Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (367,2,'Drop Service Broker Message Type','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (368,2,'Drop Service Broker Route','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (369,3,'Drop User','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (370,2,'Drop Service Broker Service','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (372,2,'Drop XML Schema','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (373,2,'Drop Type','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (400,3,'Grant object Permission','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (500,3,'Deny object permission','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (600,3,'Revoke object permission','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (700,3,'Add database user','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (701,3,'Drop database user','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (702,3,'Grant database access','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (703,3,'Revoke database access','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (704,3,'Add login to server role','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (705,3,'Drop login from server role','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (706,3,'Add member to database role','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (707,3,'Drop member from database role','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (708,3,'Add database role','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (709,3,'Drop database role','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (710,3,'Add login','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (711,3,'Drop login','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (712,3,'App role change password','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (713,3,'Password change - self','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (714,3,'Password change','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (715,3,'Login change default database','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (716,3,'Login change default language','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (717,3,'Grant login','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (718,3,'Revoke login','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (719,3,'Deny login','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (720,3,'Server Object Change Owner','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (721,3,'Change Database Owner','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (722,3,'Database Object Change Owner','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (723,3,'Schema Object Change Owner','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (724,3,'Credential mapped to login','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (725,3,'Credential map dropped','Security',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1000,3,'Grant statement permission','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1100,3,'Deny statement permission','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1200,3,'Revoke statement permission','Security',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1600,2,'Access','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1700,2,'Transfer','DDL',0,0)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1703,2,'Transfer user object','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1705,2,'Transfer DEFAULT','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1708,2,'Transfer stored procedure','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1709,2,'Transfer UDF','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1710,2,'Transfer rule','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1711,2,'Transfer repl filter stored proc','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1712,2,'Transfer system table','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1714,2,'Transfer inline function','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1715,2,'Transfer table valued UDF','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1717,2,'Transfer user table','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1718,2,'Transfer view','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1719,2,'Transfer ext stored proc','DDL',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1728,2,'Transfer CLR Stored Procedure','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1735,2,'Transfer CLR Aggregate Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1736,2,'Transfer Partition Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1737,2,'Transfer Table Valued SQL Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1751,2,'Transfer Scalar SQL Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1753,2,'Transfer Synonym','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1755,2,'Transfer Service Broker Service Queue','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1761,2,'Transfer CLR Scalar Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1763,2,'Transfer User','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1765,2,'Transfer CLR Table Valued Function','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1769,2,'Transfer User','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1772,2,'Transfer XML Schema','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1773,2,'Transfer Type','DDL',1,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1800,9,'User Configurable 0','User Defined',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1801,9,'User Configurable 1','User Defined',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1802,9,'User Configurable 2','User Defined',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1803,9,'User Configurable 3','User Defined',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1804,9,'User Configurable 4','User Defined',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1805,9,'User Configurable 5','User Defined',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1806,9,'User Configurable 6','User Defined',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1807,9,'User Configurable 7','User Defined',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1808,9,'User Configurable 8','User Defined',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1809,9,'User Configurable 9','User Defined',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (1810,1,'Log out','Login',0,1)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (900001,4,'Encrypted','DML',0,0)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (999997,0,'Missing events','Integrity Check',0,0)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (999998,0,'Events inserted','Integrity Check',0,0)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (999999,0,'Event modified','Integrity Check',0,0)
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (138,3,'Create Server Role','Security',1,1) -- Server Role
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (338,3,'Drop Server Role','Security',1,1) -- Server Role
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (158,2,'Create Linked Server','DDL',1,1) -- Linked Server
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (358,2,'Delete Linked Server','DDL',1,1) -- Linked Server
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (258,2,'Edit Linked Server','DDL',1,1) -- Linked Server
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (233,2,'Alter Security Event','DDL',1,1) -- Linked Server
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (181,10,'Server Stop','Server',1,0) -- Server Stop
GO
INSERT INTO [EventTypes] ([evtypeid],[evcatid],[name],[category],[is2005Only],[isExcludable])	VALUES (182,10,'Server Start','Server',1,0) -- Server Start
GO

-----------------------------------------------

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (0,'')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (1,'INDEX')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (2,'DATABASE')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (3,'USER OBJECT')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (4,'CHECK')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (5,'DEFAULT')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (6,'FOREIGN KEY')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (7,'PRIMARY KEY')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (8,'STORED PROCEDURE')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (9,'USER-DEFINED FUNCTION')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (10,'RULE')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (11,'REPLICATION SP')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (12,'SYSTEM TABLE')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (13,'TRIGGER')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (14,'INLINE FUNCTION')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (15,'TABLE VALUED UDF')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (16,'UNIQUE')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (17,'USER TABLE')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (18,'VIEW')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (19,'EXTENDED SP')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (20,'AD-HOC QUERY')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (21,'PREPARED QUERY')
GO

INSERT INTO [ObjectTypes] ([objtypeId],[name])
	VALUES (22,'STATISTICS')
GO



INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8259,'CHECK')
GO                                                           
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8260,'DEFAULT')
GO                                                           
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8262,'FOREIGN KEY')
GO                                                           
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8272,'STORED PROCEDURE')
GO                                                           
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8274,'RULE')
GO                                                           
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8275,'SYSTEM TABLE')
GO                                                           
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8276,'SERVER TRIGGER')
GO                                                           
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8277,'USER TABLE')
GO                                                           
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8278,'VIEW')
GO                                                           
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (8280,'EXTENDED SP')
GO
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (16724,'CLR TRIGGER')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (16964,'DATABASE')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (16975,'OBJECT')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17222,'FULL TEXT CATALOG')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17232,'CLR STORED PROCEDURE')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17235,'SCHEMA')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17475,'CREDENTIAL')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17491,'DDL EVENT')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17741,'MGMT EVENT')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17747,'SECURITY EVENT')
GO
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17749,'USER EVENT')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17985,'CLR AGGREGATE FUNCTION')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (17993,'INLINE FUNCTION')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (18000,'PARTITION FUNCTION')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (18002,'REPL FILTER PROC')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (18004,'TABLE VALUED UDF')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (18259,'SERVER ROLE')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (18263,'MICROSOFT WINDOWS GROUP')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19265,'ASYMMETRIC KEY')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19277,'MASTER KEY')
GO
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19280,'PRIMARY KEY')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19283,'OBFUS KEY')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19521,'ASYMMETRIC KEY LOGIN')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19523,'CERTIFICATE LOGIN')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19538,'ROLE')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19539,'SQL LOGIN')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (19543,'WINDOWS LOGIN')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20034,'REMOTE SERVICE BINDING')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20036,'EVENT NOTIFICATION DATABASE')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20037,'EVENT NOTIFICATION')
GO
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20038,'SCALAR FUNCTION')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20047,'EVENT NOTIFICATION OBJECT')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20051,'SYNONYM')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20549,'ENDPOINT')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20801,'CACHED ADHOC QUERIES')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20816,'CACHED ADHOC QUERIES')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20819,'SERVICE BROKER QUEUE')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (20821,'UNIQUE')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21057,'APPLICATION ROLE')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21059,'CERTIFICATE')
GO
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21075,'SERVER')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21076,'TSQL TRIGGER')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21313,'ASSEMBLY')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21318,'CLR SCALAR FUNCTION')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21321,'INLINE SCALAR FUNCTION')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21328,'PARTITION SCHEME')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21333,'USER')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21571,'SERVICE BROKER CONTRACT')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21572,'DATABASE TRIGGER')
GO                                                            
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21574,'CLR TABLE FUNCTION')
GO
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21577,'INTERNAL TABLE')
GO
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21581,'SERVICE BROKER MSG TYPE')
GO
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21586,'SERVICE BROKER ROUTE')
GO
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21587,'STATISTICS')
GO
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21825,'USER')
GO
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21827,'USER')
GO
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21831,'USER')
GO
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21843,'USER')
GO
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (21847,'USER')
GO
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (22099,'SERVICE BROKER SERVICE')
GO
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (22601,'INDEX')
GO
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (22604,'CERTIFICATE LOGIN')
GO
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (22611,'XML SCHEMA')
GO
INSERT INTO [ObjectTypes] ([objtypeId],[name]) 	VALUES (22868,'TYPE')
GO


-----------------------------------------------

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (0,'Unknown event type')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (1,'Started Collection Server')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (2,'Shutdown Collection Server')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (3,'Updated license')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (11,'Server added')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (12,'Server removed')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (13,'Server modified')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (14,'Server disabled')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (15,'Server enabled')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (16,'Server deployed')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (17,'Server deployed manually')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (20,'Database added')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (21,'Database removed')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (22,'Database modified')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (23,'Database disabled')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (24,'Database enabled')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (31,'Login added')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (32,'Login deleted')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (33,'Login modified')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (36,'Audit snapshot')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (37,'Groom')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (38,'Archive')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (39,'Configure archive preferences')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (40,'Changed audit snapshot schedule')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (41,'Failed integrity check')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (42,'Changed agent properties')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (43,'Integrity check')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (44,'Events database deleted')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (45,'AutoArchive job')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (46,'Capture snapshot job')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (47,'Integrity Check job')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (48,'Archive properties modified')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (49,'Configure Repository')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (50,'Attach Archive')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (51,'Detach Archive')
GO

INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (52,'Login Filtering Changed')
GO
INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (53,'Alert Rule Added')
GO
INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (54,'Alert Rule Removed')
GO
INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (55,'Alert Rule Modified')
GO
INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (56,'Alert Rule Disabled')
GO
INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (57,'Alert Rule Enabled')
GO
INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (58,'Groom Alerts')
GO
INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (59,'Event Filter Added')
GO
INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (60,'Event Filter Removed')
GO
INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (61,'Event Filter Modified')
GO
INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (62,'Event Filter Disabled')
GO
INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (63,'Event Filter Enabled')
GO
INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
   VALUES (64, 'Re-Index Started')
GO
INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (65,'Re-Index Finished')
GO
INSERT INTO [ChangeLogEventTypes] ([eventId],[Name])
	VALUES (66,'Event Database Limit Exceeded')
GO


-----------------------------------------------

--Alerting
---------------------------
INSERT INTO AlertTypes (alertTypeId, name) VALUES (1, 'Audited SQL Server')
INSERT INTO AlertTypes (alertTypeId, name) VALUES (2, 'SQLcompliance Status');
INSERT INTO AlertTypes (alertTypeId, name) VALUES (3, 'Data Alert');
GO

---------------------------
INSERT INTO StatusRuleTypes (StatusRuleId, RuleName) VALUES (1, 'Agent trace directory reached size limit')
GO
INSERT INTO StatusRuleTypes (StatusRuleId, RuleName) VALUES (2, 'Collection Server trace directory reached size limit');
GO
INSERT INTO StatusRuleTypes (StatusRuleId, RuleName) VALUES (3, 'Agent heartbeat was not received');
GO
INSERT INTO StatusRuleTypes (StatusRuleId, RuleName) VALUES (4, 'Event database is too large');
GO
INSERT INTO StatusRuleTypes (StatusRuleId, RuleName) VALUES (5, 'Agent cannot connect to audited instance');
GO
	
INSERT INTO DataRuleTypes (DataRuleId, RuleName) VALUES (1, 'Sensitive Column Accessed');
GO
INSERT INTO DataRuleTypes (DataRuleId, RuleName) VALUES (2, 'Numeric Column Value Changed');
GO
INSERT INTO DataRuleTypes (DataRuleId, RuleName) VALUES (3, 'Column Value Changed');
GO
INSERT INTO DataRuleTypes (DataRuleId, RuleName) VALUES (4, 'Sensitive Column Accessed Via Dataset'); -- SQLCM-5470 v5.6
GO
--------------------------
INSERT INTO ActionResultStatusTypes (statusTypeId, name) VALUES (0, 'Success') 
GO
INSERT INTO ActionResultStatusTypes (statusTypeId, name) VALUES (1, 'Failure') 
GO
INSERT INTO ActionResultStatusTypes (statusTypeId, name) VALUES (2, 'Pending') 
GO
INSERT INTO ActionResultStatusTypes (statusTypeId, name) VALUES (3, 'Uninitialized') 
GO

--Stats categories
---------------------------
INSERT INTO StatsCategories (category, name) VALUES (0, 'Unknown')
GO
INSERT INTO StatsCategories (category, name) VALUES (1, 'Audited Instances')
GO
INSERT INTO StatsCategories (category, name) VALUES (2, 'Audited Databases')
GO
INSERT INTO StatsCategories (category, name) VALUES (3, 'Processed Events')
GO
INSERT INTO StatsCategories (category, name) VALUES (4, 'Alerts')
GO
INSERT INTO StatsCategories (category, name) VALUES (5, 'Privileged User Events')
GO
INSERT INTO StatsCategories (category, name) VALUES (6, 'Failed Logins')
GO
INSERT INTO StatsCategories (category, name) VALUES (7, 'User Defined Events')
GO
INSERT INTO StatsCategories (category, name) VALUES (8, 'Admin')
GO
INSERT INTO StatsCategories (category, name) VALUES (9, 'DDL')
GO
INSERT INTO StatsCategories (category, name) VALUES (10, 'Security')
GO
INSERT INTO StatsCategories (category, name) VALUES (11, 'DML')
GO
INSERT INTO StatsCategories (category, name) VALUES (12, 'Insert')
GO
INSERT INTO StatsCategories (category, name) VALUES (13, 'Update')
GO
INSERT INTO StatsCategories (category, name) VALUES (14, 'Delete')
GO
INSERT INTO StatsCategories (category, name) VALUES (15, 'Select')
GO
INSERT INTO StatsCategories (category, name) VALUES (16, 'Logins')
GO
INSERT INTO StatsCategories (category, name) VALUES (17, 'Agent Trace Directory Size')
GO
INSERT INTO StatsCategories (category, name) VALUES (18, 'Integrity Check')
GO
INSERT INTO StatsCategories (category, name) VALUES (19, 'Execute')
GO
INSERT INTO StatsCategories (category, name) VALUES (20, 'Event Received')
GO
INSERT INTO StatsCategories (category, name) VALUES (21, 'Event Processed')
GO
INSERT INTO StatsCategories (category, name) VALUES (22, 'Event Filtered')
GO

--Regulations
INSERT INTO Regulation (name, description) VALUES ('PCI DSS', 'The PCI DSS (Payment Card Industry Data Security Standard v3.2)
 is a set of comprehensive requirements developed by the PCI Security Standards Council which includes American Express, Discover 
 Financial Services, JCB International, MasterCard Worldwide and Visa Inc. to help standardize the broad adoption of consistent data 
 security measures on a global basis. In addition it also includes requirements for security management, policies, procedures, 
 network architecture, software design and other safeguard measures. This standard is intended to help organizations to proactively 
 secure customer data.  PCI data that resides on Microsoft SQL Server must adhere to these regulations. From an IT perspective, 
 security officers mandate that all user access to PCI data (including object changes), must be audited, stored and reported to 
 the security department.')
GO
INSERT INTO Regulation (name, description) VALUES ('HIPAA', 'The Health Insurance Portability and Accountability Act of 1996 (HIPAA) 
required the Secretary of the U.S. Department of Health and Human Services (HHS) to develop regulations protecting the privacy and 
security of certain health information. To fulfill this requirement, HHS published what are commonly known as the HIPAA Privacy Rule
 and the HIPAA Security Rule. The Privacy Rule, or Standards for Privacy of Individually Identifiable Health Information, establishes
  national standards for the protection of certain health information. The Security Standards for the Protection of Electronic Protected
   Health Information (the Security Rule) establish a national set of security standards for protecting certain health information that
    is held or transferred in electronic form. Furthermore, the Health Information Technology for Economic and Clinical Health Act
     (HITECH) contains specific incentives designed to accelerate the adoption of electronic health record systems among providers.')
GO

--RegulationMap
--PCI
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
(1, '8', 'Assigning a unique identification (ID) to each person with access ensures that each individual is uniquely accountable
 for his or her actions. When such accountability is in place, actions taken on critical data and systems are performed by, and 
 can be traced to, known and authorized users.', (2+4+8+16+256), (1+2+4+8+128+512))
GO
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
(1, '8.5.4', 'Immediately revoke access for any  terminated users', (4+16),	1)
GO
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
(1, '10.1', 'Establish a process for linking all access to system components (especially access done with administrative privileges
 such as root) to each individual user)', (2+16+256), 0)
GO
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
(1, '10.2', 'Implement automated audit trails for all system components to reconstruct the following events:\r\n10.2.1 - All 
individual user accesses to cardholder data\r\n10.2.2 - All actions taken by any individual with root or administrative privileges
\r\n10.2.3 - Access to all audit trails\r\n10.2.4 - Invalid logical access attempts\r\n10.2.5 - Use of identification and authentication
 mechanisms\r\n10.2.7 - Creation and deletions of system-level objects', (2+8+128), (2+8+512))
GO
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
(1, '10.3', 'Record at least the following audit trail entries for all system components for each event:\r\n10.3.1 - User identification
\r\n10.3.2 - Type of event\r\n10.3.3 - Date and time\r\n10.3.4 - Success or failure indication\r\n10.3.5 - Origination of event
\r\n10.3.6 - Identity or name of affected data, system component or resource', (2+256), (1+2+8+512))
GO

--HIPAA
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
(2, '164.306 (a,2)', 'Security Standards - Protect against any reasonably anticipated threats or hazards to the security or integrity of such information', (2+4+8+256), (8+512))
GO
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
(2, '164.308 (1,i)', 'Security Management Process - Implement policies and procedures to prevent, detect, contain and correct security violations', (2+4+8+256), 0)
GO
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
(2, '164.308 (B)', 'Risk Management - Implement security measures sufficient to reduce risks and vulnerabilities to a reasonable
 and appropriate level to comply with 164.306(a).', (2+4+8+256), 0)
GO
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
(2, '164.308 (D)', 'Information System Activity Review - (Required). Implement procedures to regularly review records of information
 system activity such as audit logs, access reports and security incident tracking reports.', (2+4+8+256), (1+2+4+8+512))
GO
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
(2, '164.308 (3,C)', 'Termination procedures - Implement procedures for terminating access to electronic protected health information
 when the employment of a workforce member ends or as required by determinations made as specified in paragraph (a) (3) (ii) (B) of this section.', 4, 1)
GO
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
(2, '164.308 (5,C)', 'Implementation Specifications - Log-in monitoring (Addressable). Procedures for monitoring log-in attempts and
 reporting discrepancies.', 2, 0)
GO
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
(2, '164.312 (b)', 'Technical Standard - Audit controls. Implement hardware, software, and/or procedural mechanisms that record and
 examine activity in information systems that contain or use electronic protected health information.', (2+4+8+16), (1+2+4+8+512))
GO
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
(2, '164.404 (a)(1)(2)', 'Security and Privacy  - General rule. A covered entity shall, following the discovery of a breach of 
unsecured protected health information, notify each individual whose unsecured protected health information has been, or is reasonably
 believed by the covered entity to have been, accessed, acquired, used, or disclosed as a result of such breach.', 0, 513)
GO
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
(2, '164.404 (c)(1) (A),(B)', 'Security and Privacy  - (c) Implementation specifications: Content of notification-  \r\n(1) Elements.
 The notification required by (a) of this section shall include, to the extent possible:\r\n(A) A brief description of what happened,
  including the date of the breach and the date of the discovery of the breach, if known;\r\n(B) A description of the types of unsecured
   protected health information that were involved in the breach (such as whether full name, social security number, date of birth,
    home address, account number, diagnosis, disability code, or other types of information.', 0, 512)
GO
INSERT INTO RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) VALUES 
(2, 'HITECH 13402 (a)(f), (1,2)', 'Notification In the Case of Breach  \r\n(a) In General.A covered entity that accesses, maintains,
 retains, modifies, records, stores, destroys, or otherwise holds, uses, or discloses unsecured protected health information (as defined
  in subsection (h)(1)) shall, in the case of a breach of such information that is discovered by the covered entity, notify each 
  individual whose unsecured protected health information has been, or is reasonably believed by the covered entity to have been,
   accessed, acquired, or disclosed as a result of such breach.\r\n(f)  Content of Notification.Regardless of the method by which
    notice is provided to individuals under this section, notice of a breach shall include, to the extent possible, the following:
    \r\n(1) A brief description of what happened, including the date of the breach and the date of the discovery of the breach, if
     known. \r\n(2) A description of the types of unsecured protected health information that were involved in the breach (such as
      full name, Social Security number, date of birth, home address, account number, or disability code).', 0, 512)
GO
INSERT INTO [Configuration] ([timeLastModified],[warnNoHeartbeatWarning],
                             [auditLogins],[auditFailedLogins],[auditDDL],[auditSecurity],[auditAdmin],[auditDBCC],
                             [auditDML],[auditSELECT],[auditFailures],[auditSystemEvents],
                             [auditDmlAll],[auditUserTables],[auditSystemTables],[auditStoredProcedures],[auditDmlOther],
                             [auditUserAll],
                             [auditUserLogins],[auditUserFailedLogins],[auditUserDDL],[auditUserSecurity],[auditUserAdmin],
                             [auditUserDML],[auditUserSELECT],[auditUserFailures],
                             [serverPort],[agentPort],[recoveryModel],
			                 			 [sqlComplianceDbSchemaVersion],[eventsDbSchemaVersion],
  			                 		 [archiveOn],[archiveInterval],[archiveAge],[archivePeriod],[archivePrefix],
			                 			 [groomLogAge],[groomAlertAge],[groomEventAllow],[groomEventAge],
	                         	 [archiveTimeZoneName],[server],
                             [reportingVersion],[repositoryVersion],
					         					 [loginCollapse],[loginTimespan],[loginCacheSize],[auditLogouts],[auditUserLogouts] -- SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
   )
	VALUES (null,0,
                0,1,1,1,1,0,
                0,0,0,0,
                0,1,0,0,0,
                0,
                0,1,1,1,1,
                1,1,0,
                5201,5200,0,
		        2402,1303,
                1,1,60,3,'SQLcmArchive',
                60,60,1,90,
                '(UTC) Universal Coordinated Time',HOST_NAME(),
                101,101,
                1,60,250,1,1 -- SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
   )
GO

DECLARE @serverName nvarchar(128)

SELECT @serverName = CONVERT(nvarchar(128), SERVERPROPERTY('ServerName'));

INSERT INTO [SystemDatabases] ([instance],[databaseName],[databaseType],[dateCreated])
	VALUES (@serverName,'SQLcompliance','System',GETUTCDATE())

INSERT INTO [SystemDatabases] ([instance],[databaseName],[databaseType],[dateCreated])
	VALUES (@serverName,'SQLcomplianceProcessing','System',GETUTCDATE())
GO

DECLARE @dbid smallint;

SET @dbid = (SELECT dbid FROM master..sysdatabases WHERE name='SQLcompliance');
UPDATE SQLcompliance..SystemDatabases set sqlDatabaseId=@dbid where databaseName='SQLcompliance';

SET @dbid = (SELECT dbid FROM master..sysdatabases WHERE name='SQLcomplianceProcessing');
UPDATE SQLcompliance..SystemDatabases set sqlDatabaseId=@dbid where databaseName='SQLcomplianceProcessing';

GO
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
		(2, 'HITECH 13402 (a)(f) (1,2)', 'Notification In the Case of Breach  \r\n(a) In General.A covered entity that accesses, maintains, 
		retains, modifies, records, stores, destroys, or otherwise holds, uses, or discloses unsecured protected health information (as defined 
		in subsection (h)(1)) shall, in the case of a breach of such information that is discovered by the covered entity, notify each individual 
		whose unsecured protected health information has been, or is reasonably believed by the covered entity to have been, accessed, acquired, 
		or disclosed as a result of such breach.\r\n(f)  Content of Notification.Regardless of the method by which notice is provided to individuals
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
INSERT INTO Regulation (name, description) Values 
	('General Data Protection Regulation (GDPR)','General Data Protection Regulation (GDPR)')
GO
if exists (select regulationId from Regulation where name like '%GDPR%')
Begin
	Declare @regId int	
	select @regId = regulationId from Regulation where name like '%GDPR%'	
	--GDPR
	insert into RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) values 
	(@regId, 'Article 5', 'Server Level-Privileged user -DDL, Server Level-Privileged User-DML, Database Lavel-Privileged User-DDL, Databasae Level-Privileged User-DML', (8+256), (2+8+2048))
	insert into RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) values 
	(@regId, 'Article 13 (1e)', 'Database Level-Sensitive Column Auditing, Databasae Level-Before After Data', 0, (512+1024))
	insert into RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) values 
	(@regId, 'Article 24', 'Server Level-Logins, Server Level-Failed Logins', (1+2), 0)
	insert into RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) values 
	(@regId, 'Article 25 (2)', 'Server Level-Logins, Database Level-Ssensitive Column Auditing, Database Level-Before After Data, Database Level-Privileged Logins', (1), (512+1024+2048))
	insert into RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) values 
	(@regId, 'Article 30 (1)', 'Database level-Sensitive Column Auditing, Database Level-Before Afterr Data Auditing', 0, (512+1024))
	insert into RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) values
	(@regId, 'Article 32', 'Database Level - Sensitive Column Auditing, Database Level - Before After Data', 0, (512+1024))
	insert into RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) values 
	(@regId, 'Article 33', ' Server level - Logins, Server Level - Failed Logins, Database Level - Privileged Users ', (1+2), 2048)
	insert into RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) values 
	(@regId, 'Article 35 (7)', ' Database Level - Sensitive Column Auditing', 0, 512)
	insert into RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) values 
	(@regId, 'Recital 39', 'Database Level - Sensitive Column Auditing, Database Level - Before After Data', 0, (512+1024))
	insert into RegulationMap (regulationId, section, sectionDescription, serverEvents, databaseEvents) values 
	(@regId, 'GDPRSection', 'GDPRSection', (1+2+4+8+16+32+64+256), (1+2+4+8++32++512+1024+2048))
END
GO

-------------------------------------------------------------------------------------------------------
--SQLCM-5467 [Default Audit Settings]
-------------------------------------------------------------------------------------------------------
USE [SQLcompliance]
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DefaultReportCard')
BEGIN
	CREATE TABLE [dbo].[DefaultReportCard](
		[statId] [int] NOT NULL,
		[warningThreshold] [int] NOT NULL,
		[errorThreshold] [int] NOT NULL,
		[period] [int] NOT NULL,
		[enabled] [bit] NOT NULL
	) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'IderaDefaultReportCard')
BEGIN
	CREATE TABLE [dbo].[IderaDefaultReportCard](
		[statId] [int] NOT NULL,
		[warningThreshold] [int] NOT NULL,
		[errorThreshold] [int] NOT NULL,
		[period] [int] NOT NULL,
		[enabled] [bit] NOT NULL
	) ON [PRIMARY]

	INSERT INTO [dbo].[IderaDefaultReportCard]([statId], [warningThreshold], [errorThreshold], [period], [enabled]) VALUES (4,100,150,4,0)
	INSERT INTO [dbo].[IderaDefaultReportCard]([statId], [warningThreshold], [errorThreshold], [period], [enabled]) VALUES (5,100,150,4,0)
	INSERT INTO [dbo].[IderaDefaultReportCard]([statId], [warningThreshold], [errorThreshold], [period], [enabled]) VALUES (6,100,150,4,0)
	INSERT INTO [dbo].[IderaDefaultReportCard]([statId], [warningThreshold], [errorThreshold], [period], [enabled]) VALUES (9,100,150,4,0)
	INSERT INTO [dbo].[IderaDefaultReportCard]([statId], [warningThreshold], [errorThreshold], [period], [enabled]) VALUES (10,100,150,4,0)
	INSERT INTO [dbo].[IderaDefaultReportCard]([statId], [warningThreshold], [errorThreshold], [period], [enabled]) VALUES (16,100,150,4,0)
	INSERT INTO [dbo].[IderaDefaultReportCard]([statId], [warningThreshold], [errorThreshold], [period], [enabled]) VALUES (21,100,150,4,0)
	INSERT INTO [dbo].[IderaDefaultReportCard]([statId], [warningThreshold], [errorThreshold], [period], [enabled]) VALUES (23,100,150,4,0)
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DefaultServerPropertise')
BEGIN
	CREATE TABLE [dbo].[DefaultServerPropertise](
		[auditLogins] [bit] NULL,
		[auditLogouts] [bit] NULL,
		[auditFailedLogins] [bit] NULL,
		[auditDDL] [bit] NULL,
		[auditSecurity] [bit] NULL,
		[auditAdmin] [bit] NULL,
		[auditUDE] [bit] NULL,
		[auditTrace] [bit] NULL,
		[auditCaptureSQLXE] [bit] NULL,
		[isAuditLogEnabled] [bit] NULL,
		[auditFailures] [tinyint] NULL,
		[auditUsersList] [nvarchar](max) NULL,
		[auditUserAll] [bit] NULL,
		[auditUserLogins] [bit] NULL,
		[auditUserLogouts] [bit] NULL,
		[auditUserFailedLogins] [bit] NULL,
		[auditUserDDL] [bit] NULL,
		[auditUserSecurity] [bit] NULL,
		[auditUserAdmin] [bit] NULL,
		[auditUserDML] [bit] NULL,
		[auditUserSELECT] [bit] NULL,
		[auditUserUDE] [bit] NULL,
		[auditUserFailures] [tinyint] NULL, 
		[auditUserCaptureSQL] [bit] NULL,
		[auditUserCaptureTrans] [bit] NULL,
		[auditUserCaptureDDL] [bit] NULL,
		[defaultAccess] [tinyint] NULL,
		[maxSqlLength] [int] NULL,
		[auditTrustedUsersList] [nvarchar](max) NULL)
		ON [PRIMARY]

		--[auditFailures] and [auditUserFailures] set to 0 to make success selected by default
		--[auditUserAll] set 0 to enable SelectedActivities
		INSERT INTO [dbo].[DefaultServerPropertise]([auditFailedLogins], [auditTrace], [auditFailures], [auditUserAll], [auditUserLogins], [auditUserLogouts], [auditUserFailedLogins], [auditUserDDL], [auditUserSecurity], [auditUserFailures], [defaultAccess],[maxSqlLength], [auditLogins], [auditLogouts], [auditDDL], [auditSecurity], [auditAdmin], [auditUDE], [auditCaptureSQLXE], [isAuditLogEnabled], [auditUsersList],	[auditUserAdmin],[auditUserDML], [auditUserSELECT], [auditUserUDE], [auditUserCaptureSQL], [auditUserCaptureTrans], [auditUserCaptureDDL], [auditTrustedUsersList])
		VALUES (1, 0, 0, 0, 1, 0, 1, 1, 1, 0, 2, 512, 0, 0, 0, 0, 0, 0, 1, 0, NULL, 0, 0, 0, 0, 0, 0, 0, NULL)
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'IderaDefaultServerPropertise')
BEGIN
	CREATE TABLE [dbo].[IderaDefaultServerPropertise](
		[auditLogins] [bit] NULL,
		[auditLogouts] [bit] NULL,
		[auditFailedLogins] [bit] NULL,
		[auditDDL] [bit] NULL,
		[auditSecurity] [bit] NULL,
		[auditAdmin] [bit] NULL,
		[auditUDE] [bit] NULL,
		[auditTrace] [bit] NULL,
		[auditCaptureSQLXE] [bit] NULL,
		[isAuditLogEnabled] [bit] NULL,
		[auditFailures] [tinyint] NULL,
		[auditUsersList] [nvarchar](max) NULL,
		[auditUserAll] [bit] NULL,
		[auditUserLogins] [bit] NULL,
		[auditUserLogouts] [bit] NULL,
		[auditUserFailedLogins] [bit] NULL,
		[auditUserDDL] [bit] NULL,
		[auditUserSecurity] [bit] NULL,
		[auditUserAdmin] [bit] NULL,
		[auditUserDML] [bit] NULL,
		[auditUserSELECT] [bit] NULL,
		[auditUserUDE] [bit] NULL,
		[auditUserFailures] [tinyint] NULL, 
		[auditUserCaptureSQL] [bit] NULL,
		[auditUserCaptureTrans] [bit] NULL,
		[auditUserCaptureDDL] [bit] NULL,
		[defaultAccess] [tinyint] NULL,
		[maxSqlLength] [int] NULL,
		[auditTrustedUsersList] [nvarchar](max) NULL)
		ON [PRIMARY]

		--[auditFailures] and [auditUserFailures] set to 0 to make success selected by default
		--[auditUserAll] set 0 to enable SelectedActivities
		INSERT INTO [dbo].[IderaDefaultServerPropertise]([auditFailedLogins], [auditTrace], [auditFailures], [auditUserAll], [auditUserLogins], [auditUserLogouts], [auditUserFailedLogins], [auditUserDDL], [auditUserSecurity], [auditUserFailures], [defaultAccess],[maxSqlLength], [auditLogins], [auditLogouts], [auditDDL], [auditSecurity], [auditAdmin], [auditUDE], [auditCaptureSQLXE], [isAuditLogEnabled], [auditUsersList],	[auditUserAdmin],[auditUserDML], [auditUserSELECT], [auditUserUDE], [auditUserCaptureSQL], [auditUserCaptureTrans], [auditUserCaptureDDL], [auditTrustedUsersList])
		VALUES (1, 0, 0, 0, 1, 0, 1, 1, 1, 0, 2, 512, 0, 0, 0, 0, 0, 0, 1, 0, NULL, 0, 0, 0, 0, 0, 0, 0, NULL)
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DefaultDatabasePropertise')
BEGIN
	CREATE TABLE [dbo].[DefaultDatabasePropertise](
		[auditDDL] [bit] NULL,
		[auditSecurity] [bit] NULL,
		[auditAdmin] [bit] NULL,
		[auditDML] [bit] NULL,
		[auditSELECT] [bit] NULL,
		[auditFailures] [tinyint] NULL,
		[auditCaptureSQL] [bit] NULL,
		[auditCaptureTrans] [bit] NULL,
		[auditCaptureDDL] [bit] NULL,
		[auditUsersList] [nvarchar](max) NULL,
		[auditPrivUsersList] [nvarchar](max) NULL,
		[auditUserAll] [bit] NULL,
		[auditUserLogins] [bit] NULL,
		[auditUserLogouts] [bit] NULL,
		[auditUserFailedLogins] [bit] NULL,
		[auditUserDDL] [bit] NULL,
		[auditUserSecurity] [bit] NULL,
		[auditUserAdmin] [bit] NULL,
		[auditUserDML] [bit] NULL,
		[auditUserSELECT] [bit] NULL,
		[auditUserUDE] [bit] NULL,
		[auditUserFailures] [tinyint] NULL,
		[auditUserCaptureSQL] [bit] NULL,
		[auditUserCaptureTrans] [bit] NULL,
		[auditUserCaptureDDL] [bit] NULL,)
		ON [PRIMARY]

		INSERT [dbo].[DefaultDatabasePropertise] ([auditDDL], [auditSecurity], [auditAdmin], [auditDML], [auditSELECT], [auditFailures],[auditCaptureSQL], [auditCaptureTrans], [auditCaptureDDL], [auditUsersList], [auditPrivUsersList], [auditUserAll], [auditUserLogins],[auditUserLogouts], [auditUserFailedLogins], [auditUserDDL], [auditUserSecurity], [auditUserAdmin], [auditUserDML], [auditUserSELECT],[auditUserUDE], [auditUserFailures], [auditUserCaptureSQL], [auditUserCaptureTrans], [auditUserCaptureDDL]) 
		VALUES (0, 0, 0, 1, 0, 1, 0, 0, 0, NULL, NULL, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0)
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'IderaDefaultDatabasePropertise')
BEGIN
	CREATE TABLE [dbo].[IderaDefaultDatabasePropertise](
		[auditDDL] [bit] NULL,
		[auditSecurity] [bit] NULL,
		[auditAdmin] [bit] NULL,
		[auditDML] [bit] NULL,
		[auditSELECT] [bit] NULL,
		[auditFailures] [tinyint] NULL,
		[auditCaptureSQL] [bit] NULL,
		[auditCaptureTrans] [bit] NULL,
		[auditCaptureDDL] [bit] NULL,
		[auditUsersList] [nvarchar](max) NULL,
		[auditPrivUsersList] [nvarchar](max) NULL,
		[auditUserAll] [bit] NULL,
		[auditUserLogins] [bit] NULL,
		[auditUserLogouts] [bit] NULL,
		[auditUserFailedLogins] [bit] NULL,
		[auditUserDDL] [bit] NULL,
		[auditUserSecurity] [bit] NULL,
		[auditUserAdmin] [bit] NULL,
		[auditUserDML] [bit] NULL,
		[auditUserSELECT] [bit] NULL,
		[auditUserUDE] [bit] NULL,
		[auditUserFailures] [tinyint] NULL,
		[auditUserCaptureSQL] [bit] NULL,
		[auditUserCaptureTrans] [bit] NULL,
		[auditUserCaptureDDL] [bit] NULL,)
		ON [PRIMARY]

		INSERT [dbo].[IderaDefaultDatabasePropertise] ([auditDDL], [auditSecurity], [auditAdmin], [auditDML], [auditSELECT], [auditFailures],[auditCaptureSQL], [auditCaptureTrans], [auditCaptureDDL], [auditUsersList], [auditPrivUsersList], [auditUserAll], [auditUserLogins],[auditUserLogouts], [auditUserFailedLogins], [auditUserDDL], [auditUserSecurity], [auditUserAdmin], [auditUserDML], [auditUserSELECT],[auditUserUDE], [auditUserFailures], [auditUserCaptureSQL], [auditUserCaptureTrans], [auditUserCaptureDDL]) 
		VALUES (0, 1, 0, 1, 0, 0, 0, 0, 0, NULL, NULL, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0)
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DefaultAuditSettingDialogFlags')
BEGIN
	CREATE TABLE [dbo].[DefaultAuditSettingDialogFlags](
		[flagName] [nvarchar](100) NOT NULL,
		[isSet] [bit] NOT NULL)
		ON [PRIMARY]

	INSERT INTO [dbo].[DefaultAuditSettingDialogFlags] ( flagName, isSet) VALUES  ('ALERT_SERVER_DEFAULT_AUDIT_SETTINGS', 1)
	INSERT INTO [dbo].[DefaultAuditSettingDialogFlags] ( flagName, isSet) VALUES  ('CONFIRM_SERVER_DEFAULT_AUDIT_SETTINGS', 1)
	INSERT INTO [dbo].[DefaultAuditSettingDialogFlags] ( flagName, isSet) VALUES  ('ALERT_DATABASE_DEFAULT_AUDIT_SETTINGS', 1)
	INSERT INTO [dbo].[DefaultAuditSettingDialogFlags] ( flagName, isSet) VALUES  ('CONFIRM_DATABASE_DEFAULT_AUDIT_SETTINGS', 1)
END
GO

 -- 5.7 Audit Reports Changes
 
USE [SQLcompliance]
GO
SET IDENTITY_INSERT [dbo].[AuditReports] ON 

INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Alert Activity - Events', 1)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Configuration Check', 2)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Alert Activity - Status', 3)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Alert Activity - Data', 4)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Application Activity', 5)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Backup and DBCC Activity', 6)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Object Activity', 7)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Database Schema Change History', 8)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Host Activity', 9)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Agent History', 10)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Audit Control Changes', 11)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Integrity Check', 12)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Change History (by object)', 13)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Change History (by user)', 14)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Permission Denied Activity', 15)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'User Login History', 16)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Login Creation History', 17)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Login Deletion History', 18)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'User Activity History', 19)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Server Login Activity Summary', 20)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Alert Rules', 21)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Application Activity Statistics', 22)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Daily Audit Activity Statistics', 23)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'DML Activity (Before-After)', 24)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Sensitive Column Activity', 25)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Regulation Guidelines', 26)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Table-Data Access by Rowcount', 27)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Regulatory Compliance Check', 28)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Privileged / Trusted Users', 29)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Sensitive Columns / BAD', 30)
INSERT [dbo].[AuditReports] ([reportname], [rid]) VALUES (N'Server Activity Report Card', 31)
SET IDENTITY_INSERT [dbo].[AuditReports] OFF

GRANT SELECT ON [DefaultServerPropertise] TO [public]
GO
GRANT SELECT ON [DefaultDatabasePropertise] TO [public]
GO
-------------------------------------------------------------------------------------------------------------------


