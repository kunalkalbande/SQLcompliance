USE [SQLcompliance]
GO
/****** Object:  StoredProcedure [dbo].[sp_cmreport_GetAlertRules]    Script Date: 29-08-2019 20:08:50 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetAlertRules' and xtype='P')
drop procedure [sp_cmreport_GetAlertRules]
GO

CREATE procedure [dbo].[sp_cmreport_GetAlertRules] (@Server nvarchar(256) = NULL,
	@ruleName nvarchar(50),
    @ruleType int,
	@logMessage int, 
	@alertLevel int, 
    @emailMessage int
)
as
begin

declare @string nvarchar(500)
declare @stmt nvarchar(2000)
declare @fromClause nvarchar(500)
declare @whereClause nvarchar(500)
declare @orderByClause nvarchar(200)

   -- prevents sql injection but set limitations on the database naming
   set @string = 'SELECT name, alertLevel, targetInstances, logMessage, emailMessage FROM AlertRules ';

   if(@Server <> NULL)
   begin
		if(@Server = '<ALL>' OR @Server = '<All>')
		begin
			SET @Server = '*';
		end
		set @Server = UPPER(dbo.fn_cmreport_ProcessString(@Server));
		set @whereClause = ' WHERE UPPER(targetInstances) LIKE (''' + @Server + ''')';
	end

   if(@ruleName <> NULL)
   begin
		set @ruleName = UPPER(dbo.fn_cmreport_ProcessString(@ruleName));
		set @whereClause = @whereClause + ' AND name LIKE (''' + @ruleName + ''')';
	end

   if(@alertLevel > 0)
      set @whereClause = @whereClause + ' AND alertLevel=' + STR(@alertLevel);
   if(@logMessage > -1)
   	set @whereClause = @whereClause + ' AND logMessage=' + STR(@logMessage);
   if(@emailMessage > -1)
   	set @whereClause = @whereClause + ' AND emailMessage = ' + STR(@emailMessage);
   	
   if (@ruleType > 0)
      set @whereClause = @whereClause + ' AND alertType = ' + STR(@ruleType);

   set @orderByClause = ''

   set @stmt = @string + @whereClause + @orderByClause	

   EXEC(@stmt);
end
