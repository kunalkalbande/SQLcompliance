
-- Idera SQL compliance Manager version 5.6
-- Last modification date: 11/24/2018
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
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetDatabasesByServerName' and xtype='P')
drop procedure [sp_cmreport_GetDatabasesByServerName]
GO

CREATE procedure sp_cmreport_GetDatabasesByServerName
@serverName nvarchar(256)
as
begin

	IF(LTRIM(RTRIM(@serverName)) = '<ALL>')
		SELECT "<ALL>" AS name
			UNION
		SELECT "<Server Only>" AS name
			UNION
		SELECT DISTINCT(name) AS name FROM Databases
	ELSE
		SELECT "<ALL>" AS name
			UNION
		SELECT "<Server Only>" AS name
			UNION
		SELECT name FROM Databases WHERE UPPER(@serverName) = UPPER(srvInstance)

end