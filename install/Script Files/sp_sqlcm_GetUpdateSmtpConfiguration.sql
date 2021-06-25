USE [SQLcompliance]
GO

/****** Object:  StoredProcedure [dbo].[sp_sqlcm_GetUpdateSmtpConfiguration]    Script Date: 10/28/2015 9:09:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

IF (OBJECT_ID('sp_sqlcm_GetUpdateSmtpConfiguration') IS NOT NULL)
BEGIN
	DROP PROCEDURE sp_sqlcm_GetUpdateSmtpConfiguration
END
GO

Create PROCEDURE [dbo].[sp_sqlcm_GetUpdateSmtpConfiguration]
@SmtpServer nvarchar(255),
@SmtpPort int,
@SmtpAuthType int,
@SmtpSsl tinyint,
@SmtpUsername nvarchar(50),
@SmtpPassword nvarchar(50),
@SmtpSenderAddress nvarchar(50),
@SmtpSenderName nvarchar(50) 

AS
BEGIN

UPDATE [SQLcompliance]..[Configuration] 
    SET  
	  smtpServer = @SmtpServer,  
      smtpPort    =  @SmtpPort,
      smtpAuthType = @SmtpAuthType,
      smtpSsl    = @SmtpSsl,
      smtpUsername = @SmtpUsername,
      smtpPassword = @SmtpPassword,
      smtpSenderAddress =  @SmtpSenderAddress,
      smtpSenderName = @SmtpSenderName
End




GO

