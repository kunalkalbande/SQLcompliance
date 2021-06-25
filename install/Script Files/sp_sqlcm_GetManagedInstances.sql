IF (OBJECT_ID('sp_sqlcm_GetManagedInstances') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetManagedInstances
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_GetManagedInstances]
 @Page int = 1,
 @PageSize int = 10,
 @SortColumn nvarchar(100) = 'instance',
 @SortOrder int = 1
as
BEGIN
	declare @ResultQuery as nvarchar(max);

	if(@Page <> 0 and @Page is not null)
		set @Page = @Page - 1;

	declare @from as int
	set @from = @Page * @PageSize;
	declare @to as int;
	set @to = @from + @PageSize;

	DECLARE @SortOrderString nvarchar(4);
	IF (@SortOrder = 1)
		SET @SortOrderString = 'ASC';
	ELSE 
		SET @SortOrderString = 'DESC';

	set @ResultQuery = 
	'
	declare @tempServers table
	(
		Id int,
		InstanceName nvarchar(256),
		AccountType nvarchar(256),
		UserName nvarchar(256),
		Row int
	);

		insert into @tempServers(Id, InstanceName, AccountType, UserName, Row)
		select 
			srvId as Id,
			instance as InstanceName,
			authentication_type as AccountType,
			user_account as UserName,
			ROW_NUMBER() OVER(ORDER BY ' + @SortColumn + ' ' + @SortOrderString + ') AS Row
		from dbo.Servers

		select Id, InstanceName, AccountType, UserName from @tempServers
		where 
			[Row] > ' + cast(@from as nvarchar(10)) + '
			and [Row] <= ' + cast(@to as nvarchar(10));
	EXEC sp_executesql @ResultQuery;

	select count(*) as ManagedInstances from dbo.Servers WITH (NOLOCK)

END