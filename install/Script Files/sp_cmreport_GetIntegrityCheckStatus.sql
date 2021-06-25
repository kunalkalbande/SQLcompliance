USE SQLcompliance

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


-- Drop procedure before creation to handle upgrade case
if exists (select * from dbo.sysobjects where name = N'sp_cmreport_GetIntegrityCheckStatus' and xtype='P')
drop procedure [sp_cmreport_GetIntegrityCheckStatus]
GO

CREATE PROCEDURE [dbo].[sp_cmreport_GetIntegrityCheckStatus](
	@eventDatabase nvarchar(256)
	)
AS
BEGIN

	IF(@eventDatabase = '<ALL>')
	BEGIN
		SELECT 'N/A' as timeLastIntegrityCheck, 'N/A' as lastIntegrityCheckResult;
	END
	ELSE
	BEGIN
		SELECT
			Servers.timeLastIntegrityCheck,
		CASE 
			WHEN Servers.lastIntegrityCheckResult = 0 AND Servers.timeLastIntegrityCheck IS NOT NULL 
				THEN 'Success'
			WHEN Servers.lastIntegrityCheckResult = 0 AND Servers.timeLastIntegrityCheck IS NULL 
				THEN 'Not Run'
		  ELSE 
			'Failed'
		END AS lastIntegrityCheckResult		 
    	FROM
		  Servers
		WHERE
		  Servers.eventDatabase = @eventDatabase
	END
END
