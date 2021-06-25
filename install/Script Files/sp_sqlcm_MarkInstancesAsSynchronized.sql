IF (OBJECT_ID('sp_sqlcm_MarkInstancesAsSynchronized') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_MarkInstancesAsSynchronized
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_MarkInstancesAsSynchronized]
	@InstancesToMarkIds AS nvarchar(max) = ''
AS
BEGIN

        if OBJECT_ID('tempdb.dbo.#instancesToMarkIds') is not null 
            drop table #instancesToMarkIds;

        select  Value
        into    #instancesToMarkIds
        from    dbo.fn_sqlsm_Split(@InstancesToMarkIds, ',')

		update [SQLcompliance]..[Servers]
		set [isSynchronized] = 1
		where [SQLcompliance]..[Servers].[srvId] in (select Value from #instancesToMarkIds)
END