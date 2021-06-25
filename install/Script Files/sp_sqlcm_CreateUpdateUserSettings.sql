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
if exists (select * from dbo.sysobjects where name = N'sp_sqlcm_CreateUpdateUserSettings' and xtype='P')
drop procedure [sp_sqlcm_CreateUpdateUserSettings]
GO


CREATE procedure sp_sqlcm_CreateUpdateUserSettings (
	@Account nvarchar(255),
	@DashboardUserId Int,
	@Email NVarChar(230) = null,
	@SessionTimout Int,
	@Subscribed Bit
	)
as
begin
	if exists(select 1 from [dbo].[UserSettings] where DashboardUserId = @DashboardUserId)
		UPDATE [dbo].[UserSettings]
		   SET [Account] = @Account
			  ,[Email] = @Email
			  ,[SessionTimout] = @SessionTimout
			  ,[Subscribed] = @Subscribed
		 WHERE DashboardUserId = @DashboardUserId
	else
		INSERT INTO [dbo].[UserSettings]
				   ([DashboardUserId], [Account], [Email], [SessionTimout], [Subscribed])
			 VALUES
				   (@DashboardUserId, @Account, @Email, @SessionTimout, @Subscribed)
		        
end
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
