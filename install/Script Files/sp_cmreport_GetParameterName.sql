
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
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetParameterName' and xtype='P')
drop procedure [sp_cmreport_GetParameterName]
GO

CREATE procedure sp_cmreport_GetParameterName
@flag bit = null,
@serverName int,
@dbName int = null,
@databaseName nvarchar(256) = null,
@status int = null,
@auditSetting int
as
begin

If OBJECT_ID('tempdb..#AuditSettings', 'U') is not null
drop table #AuditSettings
CREATE TABLE #AuditSettings
(
	SettingId int Identity(0, 1) ,
	SettingName varchar(100)        
);


Insert into #AuditSettings values ('All');																			--SettingId: 0
Insert into #AuditSettings values ('Logins');																		--SettingId: 1
Insert into #AuditSettings values ('Logouts');																		--SettingId: 2
Insert into #AuditSettings values ('Failed Logins');																--SettingId: 3
Insert into #AuditSettings values ('Security Changes (e.g.  GRANT, REVOKE, LOGIN CHANGE PWD)');						--SettingId: 4
Insert into #AuditSettings values ('Administrative Actions (e.g. DBCC)');											--SettingId: 5
Insert into #AuditSettings values ('Database Definition (DDL) (e.g.  CREATE or DROP DATABASE)');					--SettingId: 6
Insert into #AuditSettings values ('Database Modification (DML)');     												--SettingId: 7
Insert into #AuditSettings values ('Database SELECT operations');													--SettingId: 8
Insert into #AuditSettings values ('User Defined Events (custom SQL Server event type)');							--SettingId: 9
Insert into #AuditSettings values ('Passed');																		--SettingId: 10
Insert into #AuditSettings values ('Failed');																		--SettingId: 11
Insert into #AuditSettings values ('Capture DML and SELECT Activities via');										--SettingId: 12
Insert into #AuditSettings values ('Capture SQL statements for DML and SELECT activities');							--SettingId: 13
Insert into #AuditSettings values ('Capture Transaction Status for DML Activity');									--SettingId: 14
Insert into #AuditSettings values ('Capture SQL statements for DDL and Security Changes');							--SettingId: 15
Insert into #AuditSettings values ('Privileged User Auditing');														--SettingId: 16
Insert into #AuditSettings values ('DML/SELECT Filters');															--SettingId: 17
Insert into #AuditSettings values ('Audit User Tables');															--SettingId: 18
Insert into #AuditSettings values ('Audit System Tables');															--SettingId: 19
Insert into #AuditSettings values ('Audit Stored Procedures');														--SettingId: 20
Insert into #AuditSettings values ('Audit other objects (views, indexes, etc.)');									--SettingId: 21
Insert into #AuditSettings values ('Before/After Data');															--SettingId: 22
Insert into #AuditSettings values ('Sensitive columns');															--SettingId: 23
Insert into #AuditSettings values ('Trusted Users');																--SettingId: 24


declare @param table
(
	servername varchar(50) not null,
	databasename varchar(100) not null,
	settingname varchar(100) not null
);

declare @instance varchar(50)
declare @name varchar(100)
declare @SettingName varchar(100)

select @instance = instance from Servers where srvId = @serverName
select @SettingName = SettingName from #AuditSettings where SettingId = @auditSetting

insert into @param (servername, databasename, settingname)
values
( case when (@serverName = 0) then 'All' else @instance end,
case when (@databaseName = '*') then 'All' else @databaseName end,
@SettingName
)
If OBJECT_ID('tempdb..#AuditSettings', 'U') is not null
drop table #AuditSettings

select servername, databasename, settingname from @param

end