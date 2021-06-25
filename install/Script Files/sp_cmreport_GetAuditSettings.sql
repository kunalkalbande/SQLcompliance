USE [SQLcompliance]
GO
/****** Object:  StoredProcedure [dbo].[sp_cmreport_GetAuditSettings]    Script Date: 11/23/2018 12:02:51 AM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetAuditSettings' and xtype='P')
drop procedure [sp_cmreport_GetAuditSettings]
GO

CREATE procedure [dbo].[sp_cmreport_GetAuditSettings]
as
BEGIN
If OBJECT_ID('tempdb..#AuditSettings', 'U') is not null
drop table #AuditSettings
CREATE TABLE #AuditSettings
(
	SettingId int Identity(0, 1) ,
	SettingName varchar(100),
	SettingOrder int
);

Insert into #AuditSettings values ('All', 0);																			--SettingId: 0
Insert into #AuditSettings values ('Logins', 1);																		--SettingId: 1
Insert into #AuditSettings values ('Logouts', 2);																		--SettingId: 2
Insert into #AuditSettings values ('Failed Logins', 3);																	--SettingId: 3
Insert into #AuditSettings values ('Security Changes (e.g.  GRANT, REVOKE, LOGIN CHANGE PWD)', 4);						--SettingId: 4
Insert into #AuditSettings values ('Administrative Actions (e.g. DBCC)', 5);											--SettingId: 5
Insert into #AuditSettings values ('Database Definition (DDL) (e.g.  CREATE or DROP DATABASE)', 6);						--SettingId: 6
Insert into #AuditSettings values ('Database Modification (DML)', 7);     												--SettingId: 7
Insert into #AuditSettings values ('Database SELECT operations', 8);													--SettingId: 8
Insert into #AuditSettings values ('User Defined Events (custom SQL Server event type)', 9);							--SettingId: 9
Insert into #AuditSettings values ('Passed', 10);																		--SettingId: 10
Insert into #AuditSettings values ('Failed', 11);																		--SettingId: 11
Insert into #AuditSettings values ('Capture DML and SELECT Activities via', 12);										--SettingId: 12
Insert into #AuditSettings values ('Capture SQL statements for DML and SELECT activities', 13);							--SettingId: 13
Insert into #AuditSettings values ('Capture Transaction Status for DML Activity', 14);									--SettingId: 14
Insert into #AuditSettings values ('Capture SQL statements for DDL and Security Changes', 15);							--SettingId: 15
Insert into #AuditSettings values ('Privileged User Auditing', 16);														--SettingId: 16
Insert into #AuditSettings values ('DML/SELECT Filters', 17);															--SettingId: 17
Insert into #AuditSettings values ('Audit User Tables', 18);															--SettingId: 18
Insert into #AuditSettings values ('Audit System Tables', 19);															--SettingId: 19
Insert into #AuditSettings values ('Audit Stored Procedures', 20);														--SettingId: 20
Insert into #AuditSettings values ('Audit other objects (views, indexes, etc.)', 21);									--SettingId: 21
Insert into #AuditSettings values ('Before/After Data', 22);															--SettingId: 22
Insert into #AuditSettings values ('Sensitive columns', 23);															--SettingId: 23
Insert into #AuditSettings values ('Trusted Users', 24);																--SettingId: 24

select SettingId, SettingName from #AuditSettings order by SettingOrder

If OBJECT_ID('tempdb..#AuditSettings', 'U') is not null
drop table #AuditSettings

END
 