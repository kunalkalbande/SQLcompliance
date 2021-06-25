IF (OBJECT_ID('sp_sqlcm_GetAlertsByDatabases') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetAlertsByDatabases
END
GO

USE [SQLcompliance]
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_GetAlertsByDatabases]
@EventDateFrom DateTime
AS
BEGIN
	--DECLARE @dbFieldId INT;
	--SET @dbFieldId = 5;

	SELECT a.[alertType] as RuleType, 
		at.[name] as RuleTypeName,
		a.alertLevel,
		a.ruleName,
		a.instance,
		a.[created],
		a.[eventType] as EventTypeId,
		et.category
	FROM [SQLcompliance]..[AlertRuleConditions] ars
		INNER JOIN [SQLcompliance]..[Alerts] a ON a.[alertRuleId] = ars.[ruleId] AND a.[alertType]=ars.[fieldId]
		INNER JOIN [SQLcompliance]..[AlertTypes] at on at.alertTypeId = a.alertType
		INNER JOIN [SQLcompliance]..[EventTypes] et on a.[eventType] = et.evtypeid
	WHERE (@EventDateFrom IS NULL OR @EventDateFrom <= a.[created])
END
GO
