USE [SQLcompliance]
GO
IF (OBJECT_ID('sp_sqlcm_UpdateManagedInstancesCredentials') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_UpdateManagedInstancesCredentials
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_UpdateManagedInstancesCredentials]
	@AccountType int,
	@Password [nvarchar](256),
	@UserName [nvarchar](128),
	@InstanceIds AS nvarchar(max) = ''
as
begin
    if OBJECT_ID('tempdb.dbo.#instanceIdsTableToUpdate') is not null 
		drop table #instanceIdsTableToUpdate;

        select  Value
        into    #instanceIdsTableToUpdate
        from    dbo.fn_sqlsm_Split(@InstanceIds, ',')
	
	update dbo.Servers
	set 
		authentication_type = @AccountType,
		user_account = @UserName,
		[password] = @Password
	where srvId in (select Value from #instanceIdsTableToUpdate)

end