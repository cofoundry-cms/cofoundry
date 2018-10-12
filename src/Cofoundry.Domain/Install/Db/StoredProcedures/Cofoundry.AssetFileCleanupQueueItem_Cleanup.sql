create procedure Cofoundry.AssetFileCleanupQueueItem_Cleanup
	(
		@CompletedItemRetentionTimeInSeconds float,
		@DeadLetterRetentionTimeInSeconds float
		)
	as
begin
	
	set nocount on;

	declare @Datenow DateTime2(4) = GetUtcDate()

	delete from Cofoundry.AssetFileCleanupQueueItem
	where (CanRetry <> 0 and CompletedDate is not null and CompletedDate < DateAdd(second, -@CompletedItemRetentionTimeInSeconds, @Datenow))
	or (CanRetry = 0 and LastAttemptDate < DateAdd(second, -@DeadLetterRetentionTimeInSeconds, @Datenow))
end