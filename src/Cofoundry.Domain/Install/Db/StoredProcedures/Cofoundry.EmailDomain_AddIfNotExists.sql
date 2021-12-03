create procedure Cofoundry.EmailDomain_AddIfNotExists
	(
		@Name varchar(255), 
		@UniqueName nvarchar(max), 
		@DateNow datetime2, 
		@EmailDomainId int output
	)
	as
begin
	
	set nocount on;
	
	if (@UniqueName is null) throw 50000, '@UniqueName should not be null', 1;

	declare @NameHash binary(32) = HashBytes('SHA2_256', @UniqueName);
	
	select @EmailDomainId = EmailDomainId 
	from Cofoundry.EmailDomain
	where NameHash = @NameHash

	if (@EmailDomainId is null)
	begin
		insert into Cofoundry.EmailDomain (
			[Name],
			NameHash,
			CreateDate
		) values (
			@Name,
			@NameHash,
			@DateNow
		)

		set @EmailDomainId = SCOPE_IDENTITY()
	end
end