create procedure Cofoundry.Users_Cleanup
	(
		@UserAreaCode char(3),
		@AuthenticationLogRetentionPeriodInSeconds float,
		@AuthenticationFailedLogRetentionPeriodInSeconds float,
		@Datenow datetime2
		)
	as
begin
	
	set nocount on;

	if (@AuthenticationLogRetentionPeriodInSeconds >= 0)
	begin
		delete Cofoundry.UserAuthenticationLog
		from Cofoundry.UserAuthenticationLog l
		inner join Cofoundry.[User] u on l.UserId = u.UserId
		where u.UserAreaCode = @UserAreaCode and l.CreateDate < DateAdd(second, -@AuthenticationLogRetentionPeriodInSeconds, @Datenow)
	end
	
	if (@AuthenticationFailedLogRetentionPeriodInSeconds >= 0)
	begin
		delete from Cofoundry.UserAuthenticationFailLog
		where UserAreaCode = @UserAreaCode and CreateDate < DateAdd(second, -@AuthenticationFailedLogRetentionPeriodInSeconds, @Datenow)
	end
end