create procedure Cofoundry.AuthorizedTask_Invalidate
	(
		@UserId int,
		@AuthorizedTaskTypeCodes nvarchar(max),
		@DateTimeNow datetime2
		)
	as
begin
	
	set nocount on;

	if (IsNull(@AuthorizedTaskTypeCodes, '') = '')
	begin
		update Cofoundry.AuthorizedTask
		set InvalidatedDate = @DateTimeNow
		where UserId = @UserId and CompletedDate is null
	end
	else begin
		update Cofoundry.AuthorizedTask
		set InvalidatedDate = @DateTimeNow
		from Cofoundry.AuthorizedTask t
		inner join Cofoundry.StringListToTbl(@AuthorizedTaskTypeCodes, default) c on t.AuthorizedTaskTypeCode = c.[value]
		where UserId = @UserId and CompletedDate is null
	end
end