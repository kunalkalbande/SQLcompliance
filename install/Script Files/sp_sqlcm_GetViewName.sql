USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetViewName]    Script Date: 12/18/2015 11:45:06 PM ******/
SET ANSI_NULLS ON
GO
IF (OBJECT_ID('sp_sqlcm_GetViewName') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetViewName
END
GO
SET QUOTED_IDENTIFIER OFF
GO


CREATE PROCEDURE [dbo].[sp_sqlcm_GetViewName](@ViewId nvarchar(256))
AS
SELECT ViewName FROM [SQLcompliance]..[ViewSettings] Where ViewId = @ViewId;


GO

