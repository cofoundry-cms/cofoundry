create procedure Cofoundry.AuthorizedTask_Cleanup
	(
		@RetentionPeriodInSeconds float,
		@Datenow datetime2
		)
	as
begin
	
	set nocount on;

	delete from Cofoundry.AuthorizedTask
	where (CompletedDate is not null and CompletedDate < DateAdd(second, -@RetentionPeriodInSeconds, @Datenow))
	or (ExpiryDate is not null and ExpiryDate < DateAdd(second, -@RetentionPeriodInSeconds, @Datenow))
	or (InvalidatedDate is not null and InvalidatedDate < DateAdd(second, -@RetentionPeriodInSeconds, @Datenow))
end