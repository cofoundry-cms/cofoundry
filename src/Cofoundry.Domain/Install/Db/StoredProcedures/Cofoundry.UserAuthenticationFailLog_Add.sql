create procedure Cofoundry.UserAuthenticationFailLog_Add
	(
		@UserAreaCode char(3),
		@Username nvarchar(150),
		@IPAddress varchar(45),
		@DateNow datetime2
		)
	as
begin
	
	set nocount on;
	
	declare @IPAddressId bigint
	exec Cofoundry.IPAddress_AddIfNotExists @Address = @IPAddress, @DateNow = @DateNow, @IPAddressId = @IPAddressId output

	insert into Cofoundry.UserAuthenticationFailLog (UserAreaCode, Username, IPAddressId, CreateDate) values (@UserAreaCode, @Username, @IPAddressId, @DateNow)
end