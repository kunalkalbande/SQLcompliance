IF (OBJECT_ID('sp_sqlcm_GetInstancesForSynchronization') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetInstancesForSynchronization
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_GetInstancesForSynchronization]
AS
BEGIN

SELECT 
	  [srvId]
      ,[instance]
      ,[sqlVersion]
      ,[description]
      ,[timeLastCollection]
      ,[versionName]
      ,'' as [owner]
      ,'' as [location]
      ,'' as [comments]
	  ,[timeCreated]
  FROM [SQLcompliance]..[Servers]
  WHERE [isSynchronized] = 0
END