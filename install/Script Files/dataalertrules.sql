USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetDataAlertRulesInfo]    Script Date: 11/1/2015 1:14:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_GetDataAlertRulesInfo') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetDataAlertRulesInfo
END
GO


CREATE procedure [dbo].[sp_sqlcm_GetDataAlertRulesInfo]
@ServerId int
as
begin

  SELECT [name], [dbid] FROM  [SQLcompliance].[dbo].[Databases] WHERE [srvId] = @ServerId ;
  SELECT [srvId]
      ,[dbId]
      ,[objectId]
      ,[schemaName]
      ,[tableName]
      ,[selectedColumns]
  FROM [SQLcompliance].[dbo].[SensitiveColumnTables] WHERE [srvId] = @ServerId ;

  SELECT [srvId]
      ,[dbId]
      ,[objectId]
      ,[name]
  FROM [SQLcompliance].[dbo].[SensitiveColumnColumns];
end



GO

