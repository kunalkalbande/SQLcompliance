USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetSNMPThresholdConfiguration]    Script Date: 3/7/2016 12:31:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_GetSNMPThresholdConfiguration') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetSNMPThresholdConfiguration
END
GO


CREATE PROCEDURE [dbo].[sp_sqlcm_GetSNMPThresholdConfiguration]
@InstanceName nvarchar(255)

AS
BEGIN

SELECT sender_email, logs_permission, send_mail_permission, snmpCommunity, snmp_permission,snmpPort,snmpAddress,severity,messageData
from SQLcompliance..ThresholdConfiguration where instance_name=@InstanceName;


END


GO

