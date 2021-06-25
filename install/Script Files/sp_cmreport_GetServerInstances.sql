
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
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetServerInstances' and xtype='P')
drop procedure [sp_cmreport_GetServerInstances]
GO
CREATE procedure sp_cmreport_GetServerInstances
as
begin
select 0 as srvId,  'All' as instance 
Union
select srvId, instance from Servers order by instance
end

