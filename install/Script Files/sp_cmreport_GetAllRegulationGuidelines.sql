
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
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetAllRegulationGuidelines' and xtype='P')
drop procedure [sp_cmreport_GetAllRegulationGuidelines]
GO

CREATE procedure sp_cmreport_GetAllRegulationGuidelines
as
begin
	
	SELECT 0 as regulationId, "<ALL>" as name
		UNION
	SELECT regulationId, name FROM Regulation

end