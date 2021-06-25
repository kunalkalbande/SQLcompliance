USE SQLcompliance

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


-- *************************************************************
-- fn_cmreport_GetDateString
-- Gets a string representation of the supplied date
-- *************************************************************
if exists (select * from sysobjects where id = object_id(N'fn_cmreport_GetDateString') and xtype in (N'FN', N'IF', N'TF'))
	drop function [fn_cmreport_GetDateString]
GO

CREATE FUNCTION [fn_cmreport_GetDateString] (@inDate datetime)
RETURNS nvarchar(40)
AS
BEGIN
	RETURN CONVERT(nvarchar(40),@inDate, 109);
END
GO

GRANT EXECUTE ON [fn_cmreport_GetDateString]   TO [public]
GO

-- *************************************************************
-- fn_cmreport_ProcessString
-- Remove injection threats from a string and replace * with % for SQL wildcards
-- *************************************************************
if exists (select * from sysobjects where id = object_id(N'fn_cmreport_ProcessString') and xtype in (N'FN', N'IF', N'TF'))
	drop function [fn_cmreport_ProcessString]
GO

CREATE FUNCTION [fn_cmreport_ProcessString] (@inString nvarchar(4000))
RETURNS nvarchar(4000)
AS
BEGIN
	DECLARE @retVal nvarchar(4000)
	select @retVal = replace(@inString, '*', '%')
	select @retVal = replace(@retVal, '--', '')
	select @retVal = replace(@retVal, ';', '')
	select @retVal = replace(@retVal, '''', '')
	RETURN @retVal ;
END
GO

GRANT EXECUTE ON [fn_cmreport_ProcessString]   TO [public]
GO

-- *************************************************************
if exists (select * from sysobjects where id = object_id(N'fn_cmreport_CreateMultiString') and xtype in (N'FN', N'IF', N'TF'))
	drop function [fn_cmreport_CreateMultiString]
GO
CREATE FUNCTION [fn_cmreport_CreateMultiString] (@inString nvarchar(MAX))
RETURNS nvarchar(MAX)
AS
BEGIN
	declare @outString nvarchar(MAX);
	SET @outString = '';
	if(@inString = '<ALL>' OR @inString = '<all>')
	begin
		SET @outString = '%';
	end
	else
	begin
		DECLARE @name NVARCHAR(255);
		DECLARE @pos INT;

		WHILE CHARINDEX(',', @inString) > 0
		BEGIN
			SELECT @pos  = CHARINDEX(',', @inString);
			SELECT @name = SUBSTRING(@inString, 1, @pos-1);

			SET @outString = @outString + '''' +@name+ '''' + ',';
			SELECT @inString = SUBSTRING(@inString, @pos+1, LEN(@inString)-@pos)
		END
		SET @outString = @outString + '''' +@inString+ '''';
	end
	RETURN @outString;
END
GO
GRANT EXECUTE ON [fn_cmreport_CreateMultiString]   TO [public]
GO
-- sp_cmreport_GetEventDatabasesAbove2005
-- Get a list of eventdatabases which are greater than SQL server 2005 and their corresponding description
-- *************************************************************
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetEventDatabasesAbove2005' and xtype='P')
drop procedure [sp_cmreport_GetEventDatabasesAbove2005]
GO

CREATE procedure [dbo].[sp_cmreport_GetEventDatabasesAbove2005] (@includeAll bit = 1)
AS
IF (@includeAll = 1)
	BEGIN 
		SELECT '<ALL>' as databaseName, '<ALL>' as description 
		UNION
		SELECT DISTINCT SD.databaseName, SD.instance + CASE WHEN UPPER(databaseType) LIKE 'EVENT' THEN '' ELSE ' - ' + displayName END AS description
			FROM SystemDatabases as SD left join Servers as S on (SD.instance = S.instance)
			WHERE(UPPER(databaseType) IN ('ARCHIVE', 'EVENT')) AND S.sqlVersion >= 10 -- Version is 9 for SQL server 2005
			ORDER BY description
	END
ELSE
   BEGIN
	   SELECT '<ALL>' as databaseName, '<ALL>' as description 
	   UNION
	   SELECT DISTINCT databaseName, instance + CASE WHEN UPPER(databaseType) LIKE 'EVENT' THEN '' ELSE ' - ' + displayName END AS description
		  FROM SystemDatabases
		  WHERE(UPPER(databaseType) IN ('ARCHIVE', 'EVENT'))
		  ORDER BY description
   END
GO

-- *************************************************************
-- sp_cmreport_GetEventDatabases
-- Get a list of eventdatabases and their corresponding description
-- *************************************************************
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetEventDatabases' and xtype='P')
drop procedure [sp_cmreport_GetEventDatabases]
GO

CREATE procedure sp_cmreport_GetEventDatabases (@includeAll bit = 1)
as
   IF (@includeAll = 1)
   BEGIN
	   SELECT '<ALL>' as databaseName, '<ALL>' as description 
	   UNION
	   SELECT DISTINCT databaseName, instance + CASE WHEN UPPER(databaseType) LIKE 'EVENT' THEN '' ELSE ' - ' + displayName END AS description
		  FROM SystemDatabases
		  WHERE(UPPER(databaseType) IN ('ARCHIVE', 'EVENT'))
		  ORDER BY description
   END
   ELSE
   BEGIN
	   SELECT DISTINCT databaseName, instance + CASE WHEN UPPER(databaseType) LIKE 'EVENT' THEN '' ELSE ' - ' + displayName END AS description
		  FROM SystemDatabases
		  WHERE(UPPER(databaseType) IN ('ARCHIVE', 'EVENT'))
		  ORDER BY description
   END
GO

-- no more needed in SQLcm 4.5
-- grant execute on sp_cmreport_GetEventDatabases to public
-- GO

-- *************************************************************
-- sp_cmreport_GetCategories
-- Get a list of categories and their ids
-- *************************************************************
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetCategories' and xtype='P')
drop procedure [sp_cmreport_GetCategories]
GO

CREATE procedure sp_cmreport_GetCategories 
as
   select -1 as id,'<ALL>' as category UNION select distinct evcatid as id,category from EventTypes where evcatid >= 0 and category <> 'Trace' order by category
GO

-- no more needed in SQLcm 4.5
-- grant execute on sp_cmreport_GetCategories to public
-- GO

-- *************************************************************
-- sp_cmreport_GetInstances
-- Get a list of Instances and their ids
-- *************************************************************
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetInstances' and xtype='P')
drop procedure [sp_cmreport_GetInstances]
GO

CREATE procedure sp_cmreport_GetInstances 
as
select '<ALL>' as instance UNION select instance from Servers order by instance
GO

-- no more needed in SQLcm 4.5
-- grant execute on sp_cmreport_GetInstances to public
-- GO

-- *************************************************************
-- sp_cmreport_GetDateTypes
-- Get a list of DateTypes and their ids
-- *************************************************************
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetDateTypes' and xtype='P')
drop procedure [sp_cmreport_GetDateTypes]
GO

CREATE procedure sp_cmreport_GetDateTypes 
as
   SELECT 'Custom' as DisplayName, '0' as DisplayValue UNION
   SELECT 'Yesterday', '1' UNION 
   SELECT 'Last Week', '2' UNION
   SELECT 'Last Month', '3' UNION
   SELECT 'Today', '4' UNION
   SELECT 'This Week', '5' UNION
   SELECT 'This Month', '6' UNION
   SELECT 'This Quarter', '7' UNION
   SELECT 'Last Quarter', '8' ORDER BY DisplayValue
GO

-- no more needed in SQLcm 4.5
-- grant execute on sp_cmreport_GetDateTypes to public
-- GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


