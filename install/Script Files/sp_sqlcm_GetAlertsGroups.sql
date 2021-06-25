if ( object_id('sp_sqlcm_GetAlertsGroups') is not null ) 
   begin
         drop procedure sp_sqlcm_GetAlertsGroups
   end
GO

create procedure [dbo].[sp_sqlcm_GetAlertsGroups] @InstanceId int = null
as 
       begin
             declare @instanceName as nvarchar(256) 

             if ( @InstanceId is not null ) 
                select
                    @instanceName = instance
                from
                    dbo.Servers WITH (NOLOCK)
                where
                    srvId = @InstanceId;

             select
                [alertType],
                [alertLevel],
                count(*) as alertsCount
             from
                [SQLcompliance].[dbo].[Alerts]
             where
                isnull([isDismissed], 0) <> 1
                and (
                      @InstanceId is null
                      or instance = @instanceName
                    )
             group by
                [alertType],
                [alertLevel]

       end