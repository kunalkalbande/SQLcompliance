USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetEnableAlertRules]    Script Date: 11/20/2015 3:44:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_GetEnableAlertRules') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetEnableAlertRules
END
GO

CREATE PROCEDURE [dbo].[sp_sqlcm_GetEnableAlertRules]
@Enable bit,
@RuleId int

AS
BEGIN

UPDATE SQLcompliance..AlertRules SET enabled=@Enable WHERE ruleId = @RuleId

END

GO

