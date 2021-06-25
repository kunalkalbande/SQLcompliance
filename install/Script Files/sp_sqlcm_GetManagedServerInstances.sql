IF (OBJECT_ID('sp_sqlcm_GetManagedServerInstances') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetManagedServerInstances
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_GetManagedServerInstances]
	@InstanceIds AS nvarchar(max) = ''
as
begin
    if OBJECT_ID('tempdb.dbo.#instanceIdsTable') is not null 
		drop table #instanceIdsTable;

        select  Value
        into    #instanceIdsTable
        from    dbo.fn_sqlsm_Split(@InstanceIds, ',')
	
	select 
		srvId as Id,
		instance as Instance,
		agentServer as ServerHost
	from dbo.Servers WITH (NOLOCK)
	where srvId in (select Value from #instanceIdsTable)

end