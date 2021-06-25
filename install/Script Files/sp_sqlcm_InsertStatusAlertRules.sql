USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_InsertStatusAlertRules]    Script Date: 10/17/2015 10:52:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_InsertStatusAlertRules') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_InsertStatusAlertRules
END
GO

CREATE procedure [dbo].[sp_sqlcm_InsertStatusAlertRules]  (@StrQuery NVARCHAR(MAX))
AS

EXEC(@StrQuery)
GO

