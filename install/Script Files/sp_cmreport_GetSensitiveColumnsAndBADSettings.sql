-- Idera SQL compliance Manager version 3.0
-- Last modification date: 4/25/2007
--
-- (c) Copyright 2004-2007 Idera, a division of BBS Technologies, Inc., all rights reserved.
-- SQL compliance manager, Idera and the Idera Logo are trademarks or registered trademarks
-- of BBS Technologies or its subsidiaries in the United States and other jurisdictions.
USE SQLcompliance

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetSensitiveColumnsAndBADSettings' and xtype='P')
drop procedure sp_cmreport_GetSensitiveColumnsAndBADSettings
GO

CREATE PROC [dbo].[sp_cmreport_GetSensitiveColumnsAndBADSettings](
	@Server nvarchar(256), 
	@databaseName nvarchar(256), 
	@tableName nvarchar(256),
	@schemaName nvarchar(256),
	@columnName nvarchar(256),  
	@dataType nvarchar(25), 
	@rowCount int)
AS
declare @stmt nvarchar(4000)
declare @output nvarchar(4000)
declare @column1 nvarchar(2000), @column2 nvarchar(2000)
declare @whereClause nvarchar(2000)
begin

if (object_id(N'tempdb..#sensitiveAndBad') IS NOT NULL)
		drop table #sensitiveAndBad
	create table #sensitiveAndBad (
		[Servers] nvarchar(256),
		[Databases] nvarchar(256),
		[DbId] nvarchar(256),
		[AuditType] nvarchar(25),
		[SchemaName] nvarchar(256),
		[Tables] nvarchar(max),
		[Columns] nvarchar(max),
		[Types] nvarchar(256),
		[ObjectId] int,
		[ColumnId] int,
		[RowLimit] nvarchar(5)
		) 
if (object_id(N'tempdb..#sensitiveAndBadOutput') IS NOT NULL)
		drop table #sensitiveAndBadOutput
	create table #sensitiveAndBadOutput (
		[Servers] nvarchar(256),
		[Databases] nvarchar(256),
		[AuditType] nvarchar(25),
		[SchemaName] nvarchar(256),
		[Tables] nvarchar(max),
		[Columns] nvarchar(max),
		[Types] nvarchar(256),
		[RowLimit] nvarchar(5),
		[ColumnId] int
		) 

set @column1 = 'Select distinct s.[instance] as Servers, 
				d.[name] as Databases,
				d.[dbId] as DbId,
				''Sensitive Column'' as AuditType,
				sct.[schemaName] as SchemaName, 
				sct.[tableName] as Tables, 
				''All Columns'' as Columns, 
				''Individual'' as Types, 
				sct.objectId as ObjectId, 
				'''' as ColumnId,
				'''' as RowLimit 
					from [Servers] s JOIN [Databases]d ON s.srvId = d.srvId 
					JOIN [SensitiveColumnTables] sct ON d.dbId = sct.dbId 
					Where sct.[selectedColumns] = 0
				UNION
				Select distinct s.[instance] as Servers, 
				d.[name] as Databases,
				d.[dbId] as DbId,
				''Sensitive Column'' as AuditType, 
				sct.[schemaName] as SchemaName, 
				sct.[tableName] as Tables, 
				scc.[name] as Columns, 
				scc.type as Types,
				sct.objectId as ObjectId, 
				scc.columnId as ColumnId,
				'''' as RowLimit 
					from [Servers] s JOIN [Databases]d ON s.srvId = d.srvId 
					JOIN [SensitiveColumnTables] sct ON d.dbId = sct.dbId 
					JOIN [SensitiveColumnColumns] scc ON sct.objectId = scc.objectId and scc.dbId = sct.dbId order by ColumnId'

set @column2 = 'Select distinct s.[instance] as Servers, 
				d.[name] as Databases,
				d.[dbId] as DbId,
				''Before After Data'' as AuditType, 
				dct.[schemaName] as SchemaName, 
				dct.[tableName] as Tables, 
				''All Columns'' as Columns,
				'''' as Types,
				dct.objectId as ObjectId, 
				'''' as ColumnId,
				dct.[rowLimit] as RowLimit 
				from [Servers] s JOIN [Databases]d ON s.srvId = d.srvId 
					JOIN [DataChangeTables] dct ON d.dbId = dct.dbId 
					where dct.[selectedColumns] = 0
				UNION
				Select distinct s.[instance] as Servers, 
				d.[name] as Databases,
				d.[dbId] as DbId,
				''Before After Data'' as AuditType, 
				dct.[schemaName] as SchemaName, 
				dct.[tableName] as Tables, 
				dcc.[name] as Columns,
				'''' as Types,
				dct.objectId as ObjectId, 
				'''' as ColumnId,
				dct.[rowLimit] as RowLimit 
				from [Servers] s JOIN [Databases]d ON s.srvId = d.srvId 
					JOIN [DataChangeTables] dct ON d.dbId = dct.dbId 
					JOIN [DataChangeColumns] dcc ON dct.objectId = dcc.objectId and dcc.dbId = dct.dbId order by ColumnId'

	insert into #sensitiveAndBad exec (@column1);
	insert into #sensitiveAndBad exec (@column2);
	
	--select * from #sensitiveAndBad

	set @stmt = 'Select distinct t.Servers, 
				t.Databases, 
				t.AuditType,  
				t.SchemaName,
				STUFF((SELECT distinct '', '' + t1.Tables 
				from #sensitiveAndBad t1
				 where t.ColumnId = t1.ColumnId and AuditType = ''Sensitive Column''
				    FOR XML PATH(''''), TYPE
				    ).value(''.'', ''NVARCHAR(MAX)'') 
				,1,2,'''') Tables,
				STUFF((SELECT distinct '', '' + t1.Columns
				 from #sensitiveAndBad t1
				 where t.ColumnId = t1.ColumnId and t.DbId = t1.DbId and AuditType = ''Sensitive Column''
				    FOR XML PATH(''''), TYPE
				    ).value(''.'', ''NVARCHAR(MAX)'') 
				,1,2,'''') Columns , 
				t.Types , 
				t.RowLimit ,
				t.ColumnId
				from #sensitiveAndBad t  where Columns <> ''All Columns'' and AuditType = ''Sensitive Column'''

				set @stmt = @stmt + 
				' UNION ALL  
				 Select [Servers], [Databases], [AuditType], [SchemaName], [Tables], [Columns], [Types], [RowLimit], [ColumnId] from #sensitiveAndBad where Columns = ''All Columns'' and AuditType = ''Sensitive Column'''

				 set @stmt = @stmt + 
				 ' UNION ALL 
				 Select distinct t.Servers, 
				 t.Databases, 
				 t.AuditType,  
				 t.SchemaName,
				 t.Tables,
				 STUFF((SELECT distinct '', '' + t1.Columns
				 from #sensitiveAndBad t1
				 where t.ObjectId = t1.ObjectId and t.DbId = t1.DbId and AuditType = ''Before After Data''
				    FOR XML PATH(''''), TYPE
				    ).value(''.'', ''NVARCHAR(MAX)'') 
				 ,1,2,'''') Columns , 
				 t.Types , 
				 t.RowLimit ,
				 t.ColumnId
				 from #sensitiveAndBad t  where Columns <> ''All Columns'' and AuditType = ''Before After Data'''

				 set @stmt = @stmt + 
				' UNION ALL  
				 Select [Servers], [Databases], [AuditType], [SchemaName], [Tables], [Columns], [Types], [RowLimit], [ColumnId] from #sensitiveAndBad where Columns = ''All Columns'' and AuditType = ''Before After Data'''
	
	Insert into #sensitiveAndBadOutput exec(@stmt);
	set @whereClause = '';
	if(@databaseName IS NOT NULL)
		begin
			set @databaseName = UPPER(dbo.fn_cmreport_ProcessString(@databaseName)); 
			set @whereClause = 'where UPPER(#sensitiveAndBadOutput.Databases) like ''' + UPPER(@databaseName) + ''' '
		end

	if(@Server IS NOT NULL AND @Server <> '<ALL>')
		begin
			set @Server = UPPER(dbo.fn_cmreport_ProcessString(@Server)); 
			set @whereClause = @whereClause + 'AND UPPER(#sensitiveAndBadOutput.Servers) like ''' + UPPER(@Server) + ''' '
		end

	if(@schemaName IS NOT NULL)
		begin
			set @schemaName = UPPER(dbo.fn_cmreport_ProcessString(@schemaName)); 
			set @whereClause = @whereClause + 'AND UPPER(#sensitiveAndBadOutput.SchemaName) like ''' + UPPER(@schemaName) + ''' '
		end	

	if(@columnName IS NOT NULL)
		begin
			set @columnName = UPPER(dbo.fn_cmreport_ProcessString('*' + @columnName + '*')); 
			set @whereClause = @whereClause + 'AND UPPER(#sensitiveAndBadOutput.Columns) like ''' + UPPER(@columnName) + ''' '
		end		

	if(@tableName IS NOT NULL)
		begin
			set @tableName = UPPER(dbo.fn_cmreport_ProcessString(@tableName)); 
			set @whereClause = @whereClause + 'AND UPPER(#sensitiveAndBadOutput.Tables) like ''' + UPPER(@tableName) + ''' '
		end	

	if(@dataType IS NOT NULL AND @dataType <> 'Both')
		begin
			set @dataType = UPPER(dbo.fn_cmreport_ProcessString(@dataType)); 
			set @whereClause = @whereClause + 'AND UPPER(#sensitiveAndBadOutput.AuditType) like ''' + UPPER(@dataType) + ''' '
		end	
end

	set @output = 'Select * from #sensitiveAndBadOutput ' + @whereClause 
	exec(@output);
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- no more needed in SQLcm 4.5
-- grant execute on sp_cmreport_GetApplicationActivitySummary to public
-- GO