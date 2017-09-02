create procedure Cofoundry.UserLoginLog_Add
	(
		@UserId int,
		@IPAddress varchar(45),
		@DateTimeNow datetime2
		)
	as
begin
	
	set nocount on;

	insert into Cofoundry.UserLoginLog (UserId, IPAddress, LoginDate) values (@UserId, @IPAddress, @DateTimeNow)
end