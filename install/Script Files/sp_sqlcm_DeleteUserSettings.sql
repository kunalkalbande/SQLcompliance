-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_sqlcm_DeleteUserSettings' and xtype='P')
drop procedure [sp_sqlcm_DeleteUserSettings]
GO


create procedure sp_sqlcm_DeleteUserSettings (
	@DashboardUserIds AS nvarchar(max) = '')
as
begin
        if OBJECT_ID('tempdb.dbo.#dashboardUserIdsTable') is not null 
            drop table #dashboardUserIdsTable;

        select  Value
        into    #dashboardUserIdsTable
        from    dbo.fn_sqlsm_Split(@DashboardUserIds, ',')

	delete from [dbo].[UserSettings]
    where DashboardUserId in (select Value from #dashboardUserIdsTable)


end
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
