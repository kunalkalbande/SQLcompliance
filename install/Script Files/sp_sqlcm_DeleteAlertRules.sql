USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_DeleteAlertRules]    Script Date: 11/20/2015 3:42:33 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_DeleteAlertRules') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_DeleteAlertRules
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_DeleteAlertRules]
@RuleId int

AS
BEGIN

DELETE FROM [SQLcompliance]..[AlertRuleConditions] WHERE ruleId=@RuleId;
DELETE FROM [SQLcompliance]..[AlertRules] WHERE ruleId=@RuleId;

END
GO