if ( object_id('sp_sqlcm_GetCmLicenses') is not null ) 
   begin
         drop procedure sp_sqlcm_GetCmLicenses
   end
GO

create procedure [dbo].[sp_sqlcm_GetCmLicenses]
as 
      begin
		 select
			[licenseid] as 'Id',
			[licensekey] as 'Key',
			[createdby] as 'CreatedBy',
			[createdtm] as 'CreatedTime'
		 from
			[SQLcompliance]..[Licenses];
 
		 select
			count(*) as auditedServersCount
		 from
			[SQLcompliance]..[Servers] WITH (NOLOCK)
		 where
			isAuditedServer = 1
			and isEnabled = 1
      end
