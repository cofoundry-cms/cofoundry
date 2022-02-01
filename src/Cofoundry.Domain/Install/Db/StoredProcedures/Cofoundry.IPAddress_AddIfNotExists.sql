create procedure Cofoundry.IPAddress_AddIfNotExists
	(
		@Address varchar(45), 
		@DateNow datetime2, 
		@IPAddressId bigint output
	)
	as
begin
	
	set nocount on;
	
	select @IPAddressId = IPAddressId 
	from Cofoundry.IPAddress
	where [Address] = @Address

	if (@IPAddressId is null)
	begin
		insert into Cofoundry.IPAddress (
			[Address],
			CreateDate
		) values (
			@Address,
			@DateNow
		)

		set @IPAddressId = SCOPE_IDENTITY()
	end
end