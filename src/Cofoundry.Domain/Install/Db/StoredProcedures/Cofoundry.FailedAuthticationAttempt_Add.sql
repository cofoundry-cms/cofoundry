create procedure Cofoundry.FailedAuthticationAttempt_Add
	(
		@UserAreaCode char(3),
		@Username nvarchar(150),
		@IPAddress varchar(45),
		@DateTimeNow datetime2
		)
	as
begin
	
	set nocount on;

	insert into Cofoundry.FailedAuthenticationAttempt (UserAreaCode, Username, IPAddress, AttemptDate) values (@UserAreaCode, @Username, @IPAddress, @DateTimeNow)
end