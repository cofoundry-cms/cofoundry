create procedure Cofoundry.FailedAuthticationAttempt_IsAttemptValid
	(
		@UserAreaCode char(3),
		@Username nvarchar(150),
		@IPAddress varchar(45),
		@DateTimeNow datetime2,
		@MaxIPAttempts int,
		@MaxUsernameAttempts int,
		@MaxIPAttemptsBoundaryInMinutes int,
		@MaxUsernameAttemptsBoundaryInMinutes int
		)
	as
begin
	
	set nocount on;

	declare @IPAttemptsAfter datetime = dateadd(minute, -@MaxIPAttemptsBoundaryInMinutes, @DateTimeNow)
	declare @UsernameAttemptsAfter datetime = dateadd(minute, -@MaxUsernameAttemptsBoundaryInMinutes, @DateTimeNow)
	declare @output int

	if (
		(select Count(*) from Cofoundry.FailedAuthenticationAttempt where UserAreaCode = @UserAreaCode and Username = @Username and AttemptDate > @UsernameAttemptsAfter) > @MaxUsernameAttempts
		) 
	begin
		select 0
		return;
	end

			
	if (
		(select Count(*) from Cofoundry.FailedAuthenticationAttempt where UserAreaCode = @UserAreaCode and IPAddress = @IPAddress and AttemptDate > @IPAttemptsAfter) > @MaxIPAttempts
		)
	begin
		select 0
		return;
	end

	select 1

end