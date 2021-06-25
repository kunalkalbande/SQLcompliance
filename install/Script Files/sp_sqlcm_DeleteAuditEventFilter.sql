USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_DeleteAuditEventFilter]    Script Date: 10/17/2015 7:33:30 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_DeleteAuditEventFilter') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_DeleteAuditEventFilter
END
GO


CREATE PROCEDURE [dbo].[sp_sqlcm_DeleteAuditEventFilter] (@filterid as int)
AS
BEGIN
	DELETE FROM [SQLcompliance].[dbo].[EventFilterConditions] WHERE filterId=@filterid;
	DELETE FROM [SQLcompliance].[dbo].[EventFilters] WHERE filterId=@filterid;
END

GO

