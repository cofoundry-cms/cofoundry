create procedure Cofoundry.User_SoftDelete
	(
		@UserId int,
		@Pseudonym nvarchar(50),
		@DateNow datetime2
		)
	as
begin
	
	set nocount on;

		declare @EntityDefinitionCode char(6) = 'COFUSR'
		declare @UserAreaCode char(3)
		declare @OldUniqueUsername nvarchar(150)
		declare @EmailDomainId bigint
		declare @CanDeleteDomain bit = 0

		-- Gather any required data
		select @OldUniqueUsername = UniqueUsername, @EmailDomainId = EmailDomainId, @UserAreaCode = UserAreaCode
		from Cofoundry.[User] 
		where UserId = @UserId

		-- check for an orphaned email domain
		if (@EmailDomainId is not null and not exists(select * from Cofoundry.[User] where UserId <> @UserId and EmailDomainId = @EmailDomainId))
		begin
			set @CanDeleteDomain = 1
		end

		-- Mark the user record as deleted
		update Cofoundry.[User] 
		set 
			Username = @Pseudonym,
			UniqueUsername = @Pseudonym,
			FirstName = null,
			LastName = null,
			Email = null,
			UniqueEmail = null,
			EmailDomainId = null,
			[Password] = NewId(),
			DeletedDate = @DateNow,
			DeactivatedDate = IsNull(DeactivatedDate, @DateNow)
		where UserId = @UserId
		
		-- Clear out any optional dependencies. Log tables that contain IP references are kept 
		-- for auditing, but should be cleared out periodically by the users cleanup task

		exec Cofoundry.UnstructuredDataDependency_Delete @EntityDefinitionCode = @EntityDefinitionCode, @EntityId = @UserId

		-- Task data may contain PII, so clear this out
		update Cofoundry.AuthorizedTask 
		set InvalidatedDate = @DateNow, TaskData = null
		where UserId = @UserId

		update Cofoundry.UserAuthenticationFailLog
		set Username = @Pseudonym 
		where UserAreaCode = @UserAreaCode and Username = @OldUniqueUsername

		if (@CanDeleteDomain = 1)
		begin
			delete from Cofoundry.EmailDomain where EmailDomainId = @EmailDomainId
		end
end