IF (OBJECT_ID('sp_sqlcm_GetEventsFilters') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetEventsFilters
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_GetEventsFilters]
 @ServerId int
AS
BEGIN

DECLARE @InstanceName nvarchar(256);
SET @InstanceName = (SELECT TOP 1 instance FROM [SQLcompliance]..[Servers] WITH (NOLOCK) WHERE srvId = @ServerId)

DECLARE @query nvarchar(max);
SET @query = 'SELECT ef.filterId,name,description,eventType,targetInstances,enabled, efc.conditionId, efc.fieldId, efc.matchString FROM [SQLcompliance]..[EventFilters] ef
INNER JOIN [SQLcompliance]..[EventFilterConditions] efc 
ON efc.filterId = ef.filterId
WHERE targetInstances=''<ALL>'' or targetInstances='''+ @InstanceName +''' or targetInstances LIKE ''' + @InstanceName +',%'' or targetInstances LIKE ''%,' + @InstanceName+ ''' or targetInstances LIKE ''%,' + @InstanceName+',%''';

EXEC sp_executesql @query

END