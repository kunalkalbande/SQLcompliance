USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetAlertRulesExport]    Script Date: 3/18/2016 9:20:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (OBJECT_ID('sp_sqlcm_GetAlertRulesExport') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetAlertRulesExport
END
GO

CREATE procedure [dbo].[sp_sqlcm_GetAlertRulesExport] (@ruleId int)
as
BEGIN

SELECT [ruleId]	
      ,[name]
      ,[description]
      ,[alertLevel]
      ,[alertType]
      ,[targetInstances]
      ,[enabled]
      ,[message]
      ,[logMessage]
      ,[emailMessage]
      ,[snmpTrap]
      ,[snmpServerAddress]
      ,[snmpPort]
      ,[snmpCommunity] FROM SQLcompliance..[AlertRules]  WHERE ruleId=@ruleId;
SELECT conditionId, fieldId, matchString FROM [SQLcompliance]..[AlertRuleConditions] WHERE ruleId=@ruleId;
DECLARE @MatchString NVARCHAR(MAX);
IF EXISTS (SELECT top 1 matchString FROM [SQLcompliance]..[AlertRuleConditions] WHERE  ruleId=@ruleId and fieldId=1)
BEGIN
SET @MatchString = (SELECT top 1 matchString FROM [SQLcompliance]..[AlertRuleConditions] WHERE  ruleId=@ruleId and fieldId=1 order by conditionId desc)
END
ELSE
BEGIN
SET @MatchString = (SELECT top 1 matchString FROM [SQLcompliance]..[AlertRuleConditions] WHERE  ruleId=@ruleId and fieldId=0 order by conditionId desc)
END
if @MatchString like '%)%'
BEGIN
if @MatchString like '%,%'
SELECT name, category from dbo.EventTypes where evtypeid= RIGHT(@MatchString, CHARINDEX (',' ,REVERSE(@MatchString))-1);
else
SELECT name, category from dbo.EventTypes where evtypeid= RIGHT(@MatchString, CHARINDEX (')' ,REVERSE(@MatchString))-1);
END
else
SELECT name, category from dbo.EventTypes where evtypeid=@MatchString;
END

GO

GO

