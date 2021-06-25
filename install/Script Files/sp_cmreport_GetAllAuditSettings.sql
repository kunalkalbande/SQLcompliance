
-- Idera SQL compliance Manager version 5.6
-- Last modification date: 11/24/2018
--
-- (c) Copyright 2004-2018 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE [SQLcompliance]
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET XACT_ABORT ON;
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetAllAuditSettings' and xtype='P')
drop procedure [sp_cmreport_GetAllAuditSettings]
GO

CREATE procedure sp_cmreport_GetAllAuditSettings
as
begin
	
	CREATE TABLE #AuditSettings
	(
		SettingId int Identity(0, 1),
		SettingName nvarchar(100)        
	);

	INSERT INTO #AuditSettings values ('<ALL>');																		--SettingId: 0
	INSERT INTO #AuditSettings values ('Logins');																		--SettingId: 1
	INSERT INTO #AuditSettings values ('Logouts');																		--SettingId: 2
	INSERT INTO #AuditSettings values ('Failed Logins');																--SettingId: 3
	INSERT INTO #AuditSettings values ('Security Changes');																--SettingId: 4
	INSERT INTO #AuditSettings values ('Administrative Actions');													    --SettingId: 5
	INSERT INTO #AuditSettings values ('Database Definition (DDL)');													--SettingId: 6
	INSERT INTO #AuditSettings values ('Database Modification (DML)');													--SettingId: 7
	INSERT INTO #AuditSettings values ('Database SELECT operations');													--SettingId: 8
	INSERT INTO #AuditSettings values ('User Defined Events');															--SettingId: 9
	INSERT INTO #AuditSettings values ('Access check filter');															--SettingId: 10
	INSERT INTO #AuditSettings values ('Capture DML and SELECT Activities via');									    --SettingId: 11
	INSERT INTO #AuditSettings values ('Capture SQL statements for DML and SELECT activities');							--SettingId: 12
	INSERT INTO #AuditSettings values ('Capture Transaction Status for DML Activity');									--SettingId: 13
	INSERT INTO #AuditSettings values ('Capture SQL statements for DDL and Security Changes');							--SettingId: 14

	SELECT SettingId, SettingName FROM #AuditSettings

	If OBJECT_ID('tempdb..#AuditSettings', 'U') IS NOT NULL
		DROP TABLE #AuditSettings

END
 
