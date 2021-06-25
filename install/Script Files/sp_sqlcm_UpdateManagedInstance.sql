USE [SQLcompliance]
GO
if ( object_id('sp_sqlcm_UpdateManagedInstance') is not null ) 
   begin
         drop procedure sp_sqlcm_UpdateManagedInstance
   end
GO

create procedure [dbo].[sp_sqlcm_UpdateManagedInstance]
       @Id int,
       @CollectionInterval int,
       @KeepDataFor int,
       @Comments nvarchar(max),
       @Location [nvarchar](256),
       @Owner [nvarchar](256),
       @AccountType int,
       @Password [nvarchar](256),
       @UserName [nvarchar](128)
as 
       begin

		begin transaction [UpdateInstance]
             update
                dbo.Servers
             set
                authentication_type = @AccountType,
                [password] = @Password,
                user_account = @UserName,
                comments = @Comments,
                location = @Location,
                [owner] = @Owner
             where
                srvId = @Id

				declare @agentServer as nvarchar(256);
				select @agentServer = agentServer from dbo.Servers where srvId = @Id;

				update
					dbo.Servers
				set 
					agentCollectionInterval = @CollectionInterval              
				where agentServer = @agentServer

				update
					dbo.Configuration
				set 
					groomEventAge = @KeepDataFor

		commit transaction [UpdateInstance]
       end