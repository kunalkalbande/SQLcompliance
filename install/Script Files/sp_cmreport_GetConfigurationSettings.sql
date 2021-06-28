
-- Idera SQL compliance Manager version 5.6
-- Last modification date: 11/25/2018
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
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetConfigurationSettings' and xtype='P')
drop procedure [sp_cmreport_GetConfigurationSettings]
GO

CREATE PROCEDURE sp_cmreport_GetConfigurationSettings
@serverName NVARCHAR(256) = '<ALL>',
@dbName NVARCHAR(128) = '<ALL>',
@getServerSettings BIT = 1
AS
BEGIN
	
	DECLARE @queryStr NVARCHAR(2048)

	SET @queryStr = 'SELECT '

	--Server Level Settings
	IF(@getServerSettings = 1)
	BEGIN

		SET @queryStr = @queryStr + "instance, auditLogins, auditLogouts, auditFailedLogins, auditDDL, 
										auditAdmin, auditSecurity, auditUDE, auditCaptureSQLXE, isAuditLogEnabled, 
										auditFailures, (auditLogins + auditFailedLogins * 2 + auditSecurity * 4 + auditDDL * 8 + 
										auditAdmin * 16 + auditUDE * 32 + (CASE WHEN auditFailures = 0 THEN 64 WHEN auditFailures = 2 
										THEN 128 ELSE 0 END) + (CASE WHEN LEN(auditUsersList) <> 0 THEN 1 ELSE 0 END) * 256 + auditLogouts * 512) AS userAppliedServerSettings, 
										(CASE WHEN EXISTS(SELECT * FROM Databases WHERE Databases.srvId = Servers.srvId AND Databases.auditLogins <> Servers.auditLogins) THEN 1 ELSE 0 END) AS variableLogins,
										(CASE WHEN EXISTS(SELECT * FROM Databases WHERE Databases.srvId = Servers.srvId AND Databases.auditLogouts <> Servers.auditLogouts) THEN 1 ELSE 0 END) AS variableLogouts,
										(CASE WHEN EXISTS(SELECT * FROM Databases WHERE Databases.srvId = Servers.srvId AND Databases.auditFailures <> Servers.auditFailures) THEN 1 ELSE 0 END) AS variableFailures,
										(CASE WHEN EXISTS(SELECT * FROM Databases WHERE Databases.srvId = Servers.srvId AND Databases.auditDDL <> Servers.auditDDL) THEN 1 ELSE 0 END) AS variableDDL, 
										(CASE WHEN EXISTS(SELECT * FROM Databases WHERE Databases.srvId = Servers.srvId AND Databases.auditAdmin <> Servers.auditAdmin) THEN 1 ELSE 0 END) AS variableAdmin,
										(CASE WHEN EXISTS(SELECT * FROM Databases WHERE Databases.srvId = Servers.srvId AND Databases.auditSecurity <> Servers.auditSecurity) THEN 1 ELSE 0 END) AS variableSecurity,
										userAppliedRegulations
										FROM Servers"
		
		IF(@serverName <> '<ALL>')
		BEGIN
			SET @serverName = "'" + @serverName + "'"
		 	SET @queryStr = @queryStr + ' WHERE instance = ' + @serverName
		END
	END
	--Database Level Settings
	ELSE
	BEGIN

		SET @queryStr = @queryStr + "srvInstance, name, auditDDL, auditSecurity, auditAdmin, auditDML, 
										auditSELECT, auditFailures, auditCaptureSQL, auditCaptureTrans, 
										auditCaptureDDL, (auditSecurity + auditDDL * 2 + auditAdmin * 4 + 
										auditDML * 8 + auditSELECT * 16 + (CASE WHEN auditFailures = 0 
										THEN 32 WHEN auditFailures = 2 THEN 64 ELSE 0 END) + auditCaptureSQL * 128 
										+ auditCaptureDDL * 256 + auditSensitiveColumns * 512 + 
										auditDataChanges * 1024 + (CASE WHEN LEN(auditPrivUsersList) <> 0 THEN 1 ELSE 0 END) * 2048) 
										AS userAppliedDatabaseSettings,
										pci, hipaa, disa, nerc, cis, sox, ferpa, gdpr 
										FROM Databases"
		
		IF(@dbName = '<Server Only>')
		BEGIN
			SET @queryStr = @queryStr + ' WHERE srvId = -1000' --Return no database settings records when asked for only server level records
		END
		ELSE IF(@dbName <> '<ALL>')
		BEGIN

			SET @dbName = "'" + @dbName + "'"
			
			IF(@serverName = '<ALL>')
			BEGIN
				SET @queryStr = @queryStr + ' WHERE name = ' + @dbName
			END
			ELSE
			BEGIN
				SET @serverName = "'" + @serverName + "'"
				SET @queryStr = @queryStr + ' WHERE srvInstance = ' + @serverName + ' AND name = ' + @dbName
			END
		END
		IF(@serverName <> '<ALL>' and @dbName = '<ALL>') 
		BEGIN
		   
		         SET @serverName = "'" + @serverName + "'"
				 SET @queryStr = @queryStr + ' WHERE srvInstance = ' + @serverName 
		END
	END

    --Return server/database settings
    EXECUTE(@queryStr) 

END


