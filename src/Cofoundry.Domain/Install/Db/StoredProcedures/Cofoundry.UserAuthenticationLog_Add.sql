create procedure Cofoundry.UserAuthenticationLog_Add
	(
		@UserId int,
		@IPAddress varchar(45),
		@DateNow datetime2
		)
	as
begin
	
	set nocount on;

	declare @IPAddressId bigint
	exec Cofoundry.IPAddress_AddIfNotExists @Address = @IPAddress, @DateNow = @DateNow, @IPAddressId = @IPAddressId output

	insert into Cofoundry.UserAuthenticationLog (UserId, IPAddressId, CreateDate) values (@UserId, @IPAddressId, @DateNow)
end