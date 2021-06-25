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
if exists (select * from dbo.sysobjects where name = N'sp_sqlcm_GetUserSettings' and xtype='P')
drop procedure [sp_sqlcm_GetUserSettings]
GO


CREATE procedure sp_sqlcm_GetUserSettings (
	@DashboardUserId int = null)
as
begin
	select settings.DashboardUserId, settings.Account, settings.Email, settings.SessionTimout, settings.Subscribed
		 from [dbo].[UserSettings] settings
		 where 
			@DashboardUserId is null OR DashboardUserId = @DashboardUserId
end
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
