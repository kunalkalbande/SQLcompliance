USE SQLcompliance

-- Drop procedure before creation to handle upgrade case
IF (OBJECT_ID('sp_sqlcm_DismissAlertsGroupForInstance') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_DismissAlertsGroupForInstance
END
GO

create procedure [dbo].[sp_sqlcm_DismissAlertsGroupForInstance] 
		@InstanceId int = null,
		@AlertType int = null,
		@AlertLevel int = null

as 
       begin
             declare @instanceName as nvarchar(256) 

             if ( @InstanceId is not null ) 
                select
                    @instanceName = instance
                from
                    dbo.Servers
                where
                    srvId = @InstanceId;


			UPDATE [SQLcompliance]..[Alerts]
				SET isDismissed = 1
             where
                isnull([isDismissed], 0) <> 1
                and (
                      @InstanceId is null
					  or @InstanceId = -1
                      or instance = @instanceName
                    )
				and alertType = @AlertType 
				and alertLevel = @AlertLevel
       end
GO


