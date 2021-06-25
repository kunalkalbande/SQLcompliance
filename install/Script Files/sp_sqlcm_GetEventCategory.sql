IF (OBJECT_ID('sp_sqlcm_GetEventCategory') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetEventCategory
END
GO
USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetEventCategory]    Script Date: 11/4/2015 4:48:17 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO


Create procedure [dbo].[sp_sqlcm_GetEventCategory] (@category nvarchar(256))
as
DECLARE @stmt nvarchar(4000)

BEGIN
set @stmt = 'Select name,evtypeid from dbo.EventTypes where category = ''' + REPLACE( @category,'_',' ')  + '''  and isExcludable = 1';

END

EXEC (@stmt)
GO

