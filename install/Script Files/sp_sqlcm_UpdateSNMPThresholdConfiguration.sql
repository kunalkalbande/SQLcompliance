USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_UpdateSNMPThresholdConfiguration]    Script Date: 12/31/2015 12:50:48 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_UpdateSNMPThresholdConfiguration') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_UpdateSNMPThresholdConfiguration
END
GO


CREATE PROCEDURE [dbo].[sp_sqlcm_UpdateSNMPThresholdConfiguration]
@InstanceName nvarchar(255),
@SenderEmail nvarchar(255),
@SendMailPermission tinyint,
@SnmpPermission tinyint,
@LogsPermission tinyint,
@SnmpServerAddress nvarchar(255),
@Port int,
@Community nvarchar(255),
@Severity tinyint,
@SrvId int,
@MessageData nvarchar(MAX)


AS
BEGIN
	DECLARE @count int;

SET @count = (SELECT COUNT(*) FROM SQLcompliance..ThresholdConfiguration WHERE instance_name = @InstanceName)

IF (@count > 0)
	BEGIN
			UPDATE SQLcompliance..ThresholdConfiguration SET sender_email =@SenderEmail , send_mail_permission = @SendMailPermission, snmp_permission = @SnmpPermission,logs_permission = @LogsPermission, srvId=@SrvId, snmpAddress=@SnmpServerAddress, snmpPort=@Port, snmpCommunity=@Community, severity=@Severity, messageData=@MessageData WHERE instance_name = @InstanceName
	END
ELSE
	BEGIN
		INSERT INTO SQLcompliance..ThresholdConfiguration VALUES( @InstanceName , @SenderEmail , @SendMailPermission, @SnmpPermission, @LogsPermission, @SrvId, @SnmpServerAddress, @Port, @Community, @Severity, @MessageData)
	END
END

