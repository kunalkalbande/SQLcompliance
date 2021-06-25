USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetEventEnable]    Script Date: 10/17/2015 7:37:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_GetEventEnable') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetEventEnable
END
GO

CREATE procedure [dbo].[sp_sqlcm_GetEventEnable]
@Enable bit,
@EventId int

AS
BEGIN

UPDATE SQLcompliance..EventFilters SET enabled=@Enable WHERE filterId = @EventId

END

GO

