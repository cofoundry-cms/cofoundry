create procedure Cofoundry.UserLoginLog_Add
	(
		@UserId int,
		@IPAddress varchar(45),
		@DateTimeNow datetime2
		)
	as
begin
	insert into Cofoundry.UserLoginLog (UserId, IPAddress, LoginDate) values (@UserId, @IPAddress, @DateTimeNow)
end