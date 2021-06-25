USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_InsertEventFilter]    Script Date: 10/17/2015 7:33:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_InsertEventFilter') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_InsertEventFilter
END
GO

CREATE procedure [dbo].[sp_sqlcm_InsertEventFilter]  (@StrQuery NVARCHAR(MAX))
AS

EXEC(@StrQuery)

GO

