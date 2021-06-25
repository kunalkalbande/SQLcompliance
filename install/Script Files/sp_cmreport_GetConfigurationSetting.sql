
-- Idera SQL compliance Manager version 5.6
-- Last modification date: 10/31/2018
--
-- (c) Copyright 2004-2018 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetConfigurationSetting' and xtype='P')
drop procedure [sp_cmreport_GetConfigurationSetting]
GO

CREATE PROCEDURE sp_cmreport_GetConfigurationSetting
@flag bit,
@serverName int = null,
@dbName int = null,
@databaseName nvarchar(256) = null,
@auditSetting int = null,
@status int = null
AS
BEGIN

if(@databaseName IS NOT NULL)
BEGIN
set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName));
END

IF(@flag = 1) --database config
BEGIN
IF((@databaseName IS NOT NULL AND @databaseName = '%') OR (@dbName IS NOT NULL AND @dbName = 0))
BEGIN
	IF(@serverName = 0)
		begin
		SELECT db.srvId as srvId, instance, s.isDeployed as isDeployed, instanceServer, eventDatabase, db.sqlDatabaseId as sqlDatabaseId, db.[name] as [name], agentVersion,db.auditAdmin as auditAdmin,db.auditCaptureSQL as auditCaptureSQL,s.auditCaptureSQLXE as auditCaptureSQLXE,s.auditDBCC as auditDBCC,db.auditDDL as auditDDL,db.auditDML as auditDML,db.auditDmlAll as auditDmlAll,db.auditDmlOther as auditDmlOther,db.auditExceptions as auditExceptions,
		s.auditFailedLogins as auditFailedLogins,db.auditFailures as auditFailures,db.auditLogins as auditLogins,db.auditLogouts as auditLogouts,db.auditPrivUsersList as auditPrivUsersList,db.auditSecurity as auditSecurity,db.auditSELECT as auditSELECT,s.auditSystemEvents as auditSystemEvents,s.auditTrace as auditTrace
		,s.auditUDE as auditUDE,db.auditUserAdmin as auditUserAdmin,db.auditUserAll as auditUserAll,db.auditUserCaptureDDL as auditUserCaptureDDL,db.auditUserCaptureSQL as auditUserCaptureSQL,db.auditUserCaptureTrans as auditUserCaptureTrans,db.auditUserDDL as auditUserDDL,db.auditUserDML as auditUserDML
		,db.auditUserExceptions as auditUserExceptions,s.auditUserExtendedEvents as auditUserExtendedEvents,db.auditUserFailedLogins as auditUserFailedLogins,db.auditUserFailures as auditUserFailures,db.auditUserLogins as auditUserLogins,db.auditUserLogouts as auditUserLogouts,auditUsers as auditUsers,db.auditUserSecurity as auditUserSecurity ,db.auditUserSELECT as auditUserSELECT,db.auditUsersList as auditUsersList,
		db.auditUserTables as auditUserTables,db.auditUserUDE as auditUserUDE, s.isAuditLogEnabled as isAuditLogEnabled, db.auditCaptureTrans as auditCaptureTrans,
		db.auditCaptureDDL as auditCaptureDDL, db.auditSystemTables as auditSystemTables, auditStoredProcedures,
		db.auditBroker as auditBroker, db.auditDataChanges as auditDataChanges, db.auditSensitiveColumns as auditSensitiveColumns, null as auditTrustedUsersList, db.auditUserLogouts as auditUserLogouts
		from Servers s INNER JOIN Databases db on s.srvId = db.srvId
		ORDER BY s.instanceServer, db.[name]
		END
	ELSE
		BEGIN
		SELECT db.srvId as srvId, instance, s.isDeployed as isDeployed, instanceServer, eventDatabase, db.sqlDatabaseId as sqlDatabaseId, db.[name] as [name], agentVersion,db.auditAdmin as auditAdmin,db.auditCaptureSQL as auditCaptureSQL,s.auditCaptureSQLXE as auditCaptureSQLXE,s.auditDBCC as auditDBCC,db.auditDDL as auditDDL,db.auditDML as auditDML,db.auditDmlAll as auditDmlAll,db.auditDmlOther as auditDmlOther,db.auditExceptions as auditExceptions,
		s.auditFailedLogins as auditFailedLogins,db.auditFailures as auditFailures,db.auditLogins as auditLogins,db.auditLogouts as auditLogouts,db.auditPrivUsersList as auditPrivUsersList,db.auditSecurity as auditSecurity,db.auditSELECT as auditSELECT,s.auditSystemEvents as auditSystemEvents,s.auditTrace as auditTrace
		,s.auditUDE as auditUDE,db.auditUserAdmin as auditUserAdmin,db.auditUserAll as auditUserAll,db.auditUserCaptureDDL as auditUserCaptureDDL,db.auditUserCaptureSQL as auditUserCaptureSQL,db.auditUserCaptureTrans as auditUserCaptureTrans,db.auditUserDDL as auditUserDDL,db.auditUserDML as auditUserDML
		,db.auditUserExceptions as auditUserExceptions,s.auditUserExtendedEvents as auditUserExtendedEvents,db.auditUserFailedLogins as auditUserFailedLogins,db.auditUserFailures as auditUserFailures,db.auditUserLogins as auditUserLogins,db.auditUserLogouts as auditUserLogouts,auditUsers as auditUsers,db.auditUserSecurity as auditUserSecurity ,db.auditUserSELECT as auditUserSELECT,db.auditUsersList as auditUsersList,
		db.auditUserTables as auditUserTables,db.auditUserUDE as auditUserUDE, s.isAuditLogEnabled as isAuditLogEnabled, db.auditCaptureTrans as auditCaptureTrans,
		db.auditCaptureDDL as auditCaptureDDL, db.auditSystemTables as auditSystemTables, auditStoredProcedures,
		db.auditBroker as auditBroker, db.auditDataChanges as auditDataChanges, db.auditSensitiveColumns as auditSensitiveColumns, null as auditTrustedUsersList, db.auditUserLogouts as auditUserLogouts from Servers s INNER JOIN Databases db on s.srvId = db.srvId where db.srvId = @serverName
		ORDER BY db.[name]
		END
	END
ELSE
BEGIN
IF (@serverName = 0)
BEGIN
IF(@databaseName IS NOT NULL)
BEGIN
SELECT db.srvId as srvId, instance, s.isDeployed as isDeployed, instanceServer, eventDatabase, db.sqlDatabaseId as sqlDatabaseId, db.[name] as [name], agentVersion,db.auditAdmin as auditAdmin,db.auditCaptureSQL as auditCaptureSQL,s.auditCaptureSQLXE as auditCaptureSQLXE,s.auditDBCC as auditDBCC,db.auditDDL as auditDDL,db.auditDML as auditDML,db.auditDmlAll as auditDmlAll,db.auditDmlOther as auditDmlOther,db.auditExceptions as auditExceptions,
s.auditFailedLogins as auditFailedLogins,db.auditFailures as auditFailures,db.auditLogins as auditLogins,db.auditLogouts as auditLogouts,db.auditPrivUsersList as auditPrivUsersList,db.auditSecurity as auditSecurity,db.auditSELECT as auditSELECT,s.auditSystemEvents as auditSystemEvents,s.auditTrace as auditTrace
,s.auditUDE as auditUDE,db.auditUserAdmin as auditUserAdmin,db.auditUserAll as auditUserAll,db.auditUserCaptureDDL as auditUserCaptureDDL,db.auditUserCaptureSQL as auditUserCaptureSQL,db.auditUserCaptureTrans as auditUserCaptureTrans,db.auditUserDDL as auditUserDDL,db.auditUserDML as auditUserDML
,db.auditUserExceptions as auditUserExceptions,s.auditUserExtendedEvents as auditUserExtendedEvents,db.auditUserFailedLogins as auditUserFailedLogins,db.auditUserFailures as auditUserFailures,db.auditUserLogins as auditUserLogins,db.auditUserLogouts as auditUserLogouts,s.auditUsers as auditUsers,db.auditUserSecurity as auditUserSecurity ,db.auditUserSELECT as auditUserSELECT,db.auditUsersList as auditUsersList,
db.auditUserTables as auditUserTables,db.auditUserUDE as auditUserUDE, s.isAuditLogEnabled as isAuditLogEnabled, db.auditCaptureTrans as auditCaptureTrans,
db.auditCaptureDDL as auditCaptureDDL, db.auditSystemTables as auditSystemTables, auditStoredProcedures,
db.auditBroker as auditBroker, db.auditDataChanges as auditDataChanges, db.auditSensitiveColumns as auditSensitiveColumns, null as auditTrustedUsersList, db.auditUserLogouts as auditUserLogouts from Servers s INNER JOIN Databases db on s.srvId = db.srvId WHERE db.name like @databaseName
END
ELSE
BEGIN
SELECT db.srvId as srvId, instance, s.isDeployed as isDeployed, instanceServer, eventDatabase, db.sqlDatabaseId as sqlDatabaseId, db.[name] as [name], agentVersion,db.auditAdmin as auditAdmin,db.auditCaptureSQL as auditCaptureSQL,s.auditCaptureSQLXE as auditCaptureSQLXE,s.auditDBCC as auditDBCC,db.auditDDL as auditDDL,db.auditDML as auditDML,db.auditDmlAll as auditDmlAll,db.auditDmlOther as auditDmlOther,db.auditExceptions as auditExceptions,
s.auditFailedLogins as auditFailedLogins,db.auditFailures as auditFailures,db.auditLogins as auditLogins,db.auditLogouts as auditLogouts,db.auditPrivUsersList as auditPrivUsersList,db.auditSecurity as auditSecurity,db.auditSELECT as auditSELECT,s.auditSystemEvents as auditSystemEvents,s.auditTrace as auditTrace
,s.auditUDE as auditUDE,db.auditUserAdmin as auditUserAdmin,db.auditUserAll as auditUserAll,db.auditUserCaptureDDL as auditUserCaptureDDL,db.auditUserCaptureSQL as auditUserCaptureSQL,db.auditUserCaptureTrans as auditUserCaptureTrans,db.auditUserDDL as auditUserDDL,db.auditUserDML as auditUserDML
,db.auditUserExceptions as auditUserExceptions,s.auditUserExtendedEvents as auditUserExtendedEvents,db.auditUserFailedLogins as auditUserFailedLogins,db.auditUserFailures as auditUserFailures,db.auditUserLogins as auditUserLogins,db.auditUserLogouts as auditUserLogouts,s.auditUsers as auditUsers,db.auditUserSecurity as auditUserSecurity ,db.auditUserSELECT as auditUserSELECT,db.auditUsersList as auditUsersList,
db.auditUserTables as auditUserTables,db.auditUserUDE as auditUserUDE, s.isAuditLogEnabled as isAuditLogEnabled, db.auditCaptureTrans as auditCaptureTrans,
db.auditCaptureDDL as auditCaptureDDL, db.auditSystemTables as auditSystemTables, auditStoredProcedures,
db.auditBroker as auditBroker, db.auditDataChanges as auditDataChanges, db.auditSensitiveColumns as auditSensitiveColumns, null as auditTrustedUsersList, db.auditUserLogouts as auditUserLogouts from Servers s INNER JOIN Databases db on s.srvId = db.srvId WHERE db.sqlDatabaseId = @dbName
END
END
ELSE
BEGIN
IF(@databaseName IS NOT NULL)
BEGIN
SELECT db.srvId as srvId, instance, s.isDeployed as isDeployed, instanceServer, eventDatabase, db.sqlDatabaseId as sqlDatabaseId, db.[name] as [name], agentVersion,db.auditAdmin as auditAdmin,db.auditCaptureSQL as auditCaptureSQL,s.auditCaptureSQLXE as auditCaptureSQLXE,s.auditDBCC as auditDBCC,db.auditDDL as auditDDL,db.auditDML as auditDML,db.auditDmlAll as auditDmlAll,db.auditDmlOther as auditDmlOther,db.auditExceptions as auditExceptions,
s.auditFailedLogins as auditFailedLogins,db.auditFailures as auditFailures,db.auditLogins as auditLogins,db.auditLogouts as auditLogouts,db.auditPrivUsersList as auditPrivUsersList,db.auditSecurity as auditSecurity,db.auditSELECT as auditSELECT,s.auditSystemEvents as auditSystemEvents,s.auditTrace as auditTrace
,s.auditUDE as auditUDE,db.auditUserAdmin as auditUserAdmin,db.auditUserAll as auditUserAll,db.auditUserCaptureDDL as auditUserCaptureDDL,db.auditUserCaptureSQL as auditUserCaptureSQL,db.auditUserCaptureTrans as auditUserCaptureTrans,db.auditUserDDL as auditUserDDL,db.auditUserDML as auditUserDML
,db.auditUserExceptions as auditUserExceptions,s.auditUserExtendedEvents as auditUserExtendedEvents,db.auditUserFailedLogins as auditUserFailedLogins,db.auditUserFailures as auditUserFailures,db.auditUserLogins as auditUserLogins,db.auditUserLogouts as auditUserLogouts,s.auditUsers as auditUsers,db.auditUserSecurity as auditUserSecurity ,db.auditUserSELECT as auditUserSELECT,db.auditUsersList as auditUsersList,
db.auditUserTables as auditUserTables,db.auditUserUDE as auditUserUDE, s.isAuditLogEnabled as isAuditLogEnabled, db.auditCaptureTrans as auditCaptureTrans,
db.auditCaptureDDL as auditCaptureDDL, db.auditSystemTables as auditSystemTables, auditStoredProcedures,
db.auditBroker as auditBroker, db.auditDataChanges as auditDataChanges, db.auditSensitiveColumns as auditSensitiveColumns, null as auditTrustedUsersList, db.auditUserLogouts as auditUserLogouts from Servers s INNER JOIN Databases db on s.srvId = db.srvId WHERE db.name like @databaseName and db.srvId = @serverName
END
ELSE
BEGIN
SELECT db.srvId as srvId, instance, s.isDeployed as isDeployed, instanceServer, eventDatabase, db.sqlDatabaseId as sqlDatabaseId, db.[name] as [name], agentVersion,db.auditAdmin as auditAdmin,db.auditCaptureSQL as auditCaptureSQL,s.auditCaptureSQLXE as auditCaptureSQLXE,s.auditDBCC as auditDBCC,db.auditDDL as auditDDL,db.auditDML as auditDML,db.auditDmlAll as auditDmlAll,db.auditDmlOther as auditDmlOther,db.auditExceptions as auditExceptions,
s.auditFailedLogins as auditFailedLogins,db.auditFailures as auditFailures,db.auditLogins as auditLogins,db.auditLogouts as auditLogouts,db.auditPrivUsersList as auditPrivUsersList,db.auditSecurity as auditSecurity,db.auditSELECT as auditSELECT,s.auditSystemEvents as auditSystemEvents,s.auditTrace as auditTrace
,s.auditUDE as auditUDE,db.auditUserAdmin as auditUserAdmin,db.auditUserAll as auditUserAll,db.auditUserCaptureDDL as auditUserCaptureDDL,db.auditUserCaptureSQL as auditUserCaptureSQL,db.auditUserCaptureTrans as auditUserCaptureTrans,db.auditUserDDL as auditUserDDL,db.auditUserDML as auditUserDML
,db.auditUserExceptions as auditUserExceptions,s.auditUserExtendedEvents as auditUserExtendedEvents,db.auditUserFailedLogins as auditUserFailedLogins,db.auditUserFailures as auditUserFailures,db.auditUserLogins as auditUserLogins,db.auditUserLogouts as auditUserLogouts,s.auditUsers as auditUsers,db.auditUserSecurity as auditUserSecurity ,db.auditUserSELECT as auditUserSELECT,db.auditUsersList as auditUsersList,
db.auditUserTables as auditUserTables,db.auditUserUDE as auditUserUDE, s.isAuditLogEnabled as isAuditLogEnabled, db.auditCaptureTrans as auditCaptureTrans,
db.auditCaptureDDL as auditCaptureDDL, db.auditSystemTables as auditSystemTables, auditStoredProcedures,
db.auditBroker as auditBroker, db.auditDataChanges as auditDataChanges, db.auditSensitiveColumns as auditSensitiveColumns, null as auditTrustedUsersList, db.auditUserLogouts as auditUserLogouts from Servers s INNER JOIN Databases db on s.srvId = db.srvId WHERE db.sqlDatabaseId = @dbName and db.srvId = @serverName
END
END
END
END
ELSE
BEGIN --Server CONFIG
IF(@serverName = 0)
BEGIN
SELECT srvId, instance, isDeployed, instanceServer, eventDatabase, null as sqlDatabaseId, null as [name], agentVersion, auditAdmin, auditCaptureSQL, auditCaptureSQLXE, auditDBCC, auditDDL,auditDML,
null as auditDmlAll,null as auditDmlOther, auditExceptions, auditFailedLogins
,auditFailures,auditLogins,auditLogouts,null as auditPrivUsersList,auditSecurity, auditSELECT, auditSystemEvents, auditTrace, auditUDE, auditUserAdmin, auditUserAll
, auditUserCaptureDDL, auditUserCaptureSQL, auditUserCaptureTrans, auditUserDDL, auditUserDML, auditUserExceptions, auditUserExtendedEvents
, auditUserFailedLogins, auditUserFailures, auditUserLogins,auditUserLogouts, auditUsers, auditUserSecurity, auditUserSELECT, auditUsersList,null as auditUserTables, auditUserUDE
, isAuditLogEnabled, null as auditCaptureTrans, null as auditCaptureDDL, null as auditSystemTables, null as auditStoredProcedures,
null as auditBroker,null as auditDataChanges, null as auditSensitiveColumns, auditTrustedUsersList, auditUserLogouts  from Servers
END
ELSE
BEGIN
SELECT srvId, instance, isDeployed, instanceServer, eventDatabase, null as sqlDatabaseId, null as [name], agentVersion, auditAdmin, auditCaptureSQL, auditCaptureSQLXE, auditDBCC, auditDDL,auditDML,
null as auditDmlAll,null as auditDmlOther, auditExceptions, auditFailedLogins
,auditFailures,auditLogins,auditLogouts,null as auditPrivUsersList,auditSecurity, auditSELECT, auditSystemEvents, auditTrace, auditUDE, auditUserAdmin, auditUserAll
, auditUserCaptureDDL, auditUserCaptureSQL, auditUserCaptureTrans, auditUserDDL, auditUserDML, auditUserExceptions, auditUserExtendedEvents
, auditUserFailedLogins, auditUserFailures, auditUserLogins,auditUserLogouts, auditUsers, auditUserSecurity, auditUserSELECT, auditUsersList,null as auditUserTables, auditUserUDE
, isAuditLogEnabled, null as auditCaptureTrans, null as auditCaptureDDL, null as auditSystemTables, null as auditStoredProcedures,
null as auditBroker,null as auditDataChanges, null as auditSensitiveColumns, auditTrustedUsersList, auditUserLogouts 
from Servers WHERE srvId = @serverName
END
END
END


