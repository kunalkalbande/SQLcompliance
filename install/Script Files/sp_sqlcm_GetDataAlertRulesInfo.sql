USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetDataAlertRulesInfo]    Script Date: 12/11/2015 10:45:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

if ( object_id('sp_sqlcm_GetDataAlertRulesInfo') is not null ) 
   begin
         drop procedure sp_sqlcm_GetDataAlertRulesInfo
   end
GO


CREATE procedure [dbo].[sp_sqlcm_GetDataAlertRulesInfo]
@ConditionId int
as
BEGIN
 
 if(@ConditionId = 1)
 Begin
  SELECT [srvId], [name], [dbId] FROM  [SQLcompliance].[dbo].[Databases] WHERE auditSensitiveColumns = 1;
  SELECT [srvId]
      ,[dbId]
      ,[objectId]
      ,[schemaName]
      ,[schemaName]+'.'+[tableName] AS [tableName]
      ,[selectedColumns]
  FROM [SQLcompliance].[dbo].[SensitiveColumnTables];

  SELECT [srvId]
      ,[dbId]
      ,[objectId]
      ,[name]
  FROM [SQLcompliance].[dbo].[SensitiveColumnColumns];
END
ELSE BEGIN
SELECT [srvId], [name], [dbId] FROM  [SQLcompliance].[dbo].[Databases] WHERE auditDataChanges = 1;
  SELECT [srvId]
      ,[dbId]
      ,[objectId]
      ,[schemaName]
      ,[schemaName]+'.'+[tableName] AS [tableName]
      ,[selectedColumns]
  FROM [SQLcompliance].[dbo].[DataChangeTables];

  SELECT [srvId]
      ,[dbId]
      ,[objectId]
      ,[name]
  FROM [SQLcompliance].[dbo].[DataChangeColumns];
  END
END

GO

