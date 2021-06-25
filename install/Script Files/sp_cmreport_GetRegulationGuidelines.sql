-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2011 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'p_cmreport_GetRegulationGuidelines' and xtype='P')
drop procedure [p_cmreport_GetRegulationGuidelines]
GO

CREATE procedure p_cmreport_GetRegulationGuidelines
as
DECLARE @stmt nvarchar(4000)

set @stmt = 'select rm.regulationId, r.name, section, sectionDescription, serverEvents, databaseEvents from SQLcompliance..RegulationMap rm
			 JOIN SQLcompliance..Regulation r on r.regulationId = rm.regulationId and rm.section NOT LIKE (''GDPRSection'')'

EXEC (@stmt)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- no more needed in SQLcm 4.5
-- grant execute on p_cmreport_GetRegulationGuidelines to public
-- GO