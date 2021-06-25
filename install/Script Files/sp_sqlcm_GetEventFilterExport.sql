USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetEventFilterExport]    Script Date: 10/17/2015 7:29:55 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_GetEventFilterExport') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetEventFilterExport
END
GO

CREATE procedure [dbo].[sp_sqlcm_GetEventFilterExport] (@filterId int)
as
BEGIN
SELECT filterId,name,description,eventType,targetInstances,enabled FROM SQLcompliance..EventFilters  WHERE filterId=@filterId;
SELECT conditionId, fieldId, matchString FROM SQLcompliance..EventFilterConditions WHERE filterId=@filterId;
DECLARE @MatchString NVARCHAR(MAX);
SET @MatchString = (SELECT top 1 matchString FROM SQLcompliance..EventFilterConditions WHERE  filterId=@filterId and fieldId=0)
if @MatchString like '%)%'
SELECT name, category from dbo.EventTypes where evtypeid= RIGHT(@MatchString, CHARINDEX (')' ,REVERSE(@MatchString))-1);
else
SELECT name='', category='';
END
GO

