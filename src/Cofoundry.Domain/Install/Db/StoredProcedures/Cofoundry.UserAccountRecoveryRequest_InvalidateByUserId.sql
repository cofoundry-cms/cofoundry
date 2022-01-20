create procedure Cofoundry.UserAccountRecoveryRequest_InvalidateByUserId
	(
		@UserId int,
		@DateTimeNow datetime2
		)
	as
begin
	
	set nocount on;

	update Cofoundry.UserAccountRecoveryRequest
	set InvalidatedDate = @DateTimeNow
	where UserId = @UserId and CompletedDate is null
end