create procedure Cofoundry.UserAuthenticationFailLog_IsAttemptValid
	(
		@UserAreaCode char(3),
		@Username nvarchar(150),
		@IPAddress varchar(45),
		@DateNow datetime2,
		@IPAddressRateLimitQuantity int,
		@IPAddressRateLimitWindowInSeconds int,
		@UsernameRateLimitQuantity int,
		@UsernameRateLimitWindowInSeconds int
		)
	as
begin
	
	set nocount on;

	declare @output int

	if (@UsernameRateLimitQuantity is not null and @UsernameRateLimitWindowInSeconds is not null)
	begin
		declare @UsernameAttemptsAfter datetime = dateadd(second, -@UsernameRateLimitWindowInSeconds, @DateNow)
		if (
			(select Count(*) from Cofoundry.UserAuthenticationFailLog where UserAreaCode = @UserAreaCode and Username = @Username and CreateDate > @UsernameAttemptsAfter) > @UsernameRateLimitQuantity
			) 
		begin
			select 0
			return;
		end
	end
	
	if (@IPAddressRateLimitQuantity is not null and @IPAddressRateLimitWindowInSeconds is not null)
	begin
		declare @IPAddressId bigint
		exec Cofoundry.IPAddress_AddIfNotExists @Address = @IPAddress, @DateNow = @DateNow, @IPAddressId = @IPAddressId output
		declare @IPAttemptsAfter datetime = dateadd(second, -@IPAddressRateLimitWindowInSeconds, @DateNow)

		if (
			(select Count(*) from Cofoundry.UserAuthenticationFailLog where UserAreaCode = @UserAreaCode and IPAddressId = @IPAddressId and CreateDate > @IPAttemptsAfter) > @IPAddressRateLimitQuantity
			)
		begin
			select 0
			return;
		end
	end

	select 1

end