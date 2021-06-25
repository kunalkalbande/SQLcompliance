IF (OBJECT_ID('sp_sqlcm_GetManagedInstance') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetManagedInstance
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_GetManagedInstance]
 @id int
as
begin
	declare @groomEventAge as int;
	(select top(1) @groomEventAge = groomEventAge from dbo.Configuration)

	select 
		srvId as Id,
		instance as InstanceName,
		authentication_type as AccountType,
		user_account as UserName,
		[owner] as [Owner],
		location as Location,
		comments as Comments,
		agentCollectionInterval as CollectionInterval,
		@groomEventAge as KeepDataFor,
		[password] as [Password]
	from dbo.Servers WITH (NOLOCK)
	where srvId = @id

end
