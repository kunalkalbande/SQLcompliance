exec master.dbo.sp_sqlcm_RenamedSQLcomplianceProcessing;
declare @deleteRenameSP nvarchar(max);
 begin  
 set @deleteRenameSP= 'DROP PROCEDURE [dbo].[sp_sqlcm_RenamedSQLcomplianceProcessing]'
 exec (@deleteRenameSP);
 end


