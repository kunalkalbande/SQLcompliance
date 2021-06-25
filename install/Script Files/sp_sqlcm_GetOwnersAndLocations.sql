IF (OBJECT_ID('sp_sqlcm_GetOwnersAndLocations') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetOwnersAndLocations
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_GetOwnersAndLocations]
as
begin

	select 
		[owner] as [Owner]
	from dbo.Servers where [owner] is not null
		group by [owner]

	select 
		location as Location
	from dbo.Servers where location is not null
		group by location

end
