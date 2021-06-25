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
if exists (select * from dbo.sysobjects where name = N'p_cmreport_GetInstanceRegulationGuidelines' and xtype='P')
drop procedure [p_cmreport_GetInstanceRegulationGuidelines]
GO

CREATE procedure p_cmreport_GetInstanceRegulationGuidelines (@instance nvarchar(256))
as
DECLARE @stmt nvarchar(4000)

if (@instance = '<ALL>')
BEGIN
set @stmt = 'select s.instance, s.srvId, db.name, db.pci, db.hipaa, db.disa, db.nerc, db.cis, db.sox, db.ferpa, db.gdpr from Servers s
			 join Databases db on s.srvId = db.srvId;'
END
ELSE
BEGIN
set @stmt = 'select s.instance, s.srvId, db.name, db.pci, db.hipaa, db.disa, db.nerc, db.cis, db.sox, db.ferpa, db.gdpr from Servers s
			join Databases db on s.srvId = db.srvId
			where UPPER(s.instance) = ''' + UPPER(@instance) + ''' ';
END

EXEC (@stmt)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- no more needed in SQLcm 4.5
-- grant execute on p_cmreport_GetInstanceRegulationGuidelines to public
-- GO