create procedure Cofoundry.CustomEntityPublishStatusQuery_Update
	(
		@CustomEntityId int
	)
	as
begin
	
	set nocount on;
			
	-- PublishStatusQuery.Published
	
	with CTE_LatestPublishedCustomEntityVersions as
	(
	   select v.CustomEntityId, v.CustomEntityVersionId,
			 row_number() over (partition by v.CustomEntityId order by v.CreateDate desc) as RowNumber
	   from Cofoundry.CustomEntityVersion v
	   inner join Cofoundry.CustomEntity e on e.CustomEntityId = v.CustomEntityId and e.PublishStatusCode = 'P'
	   where v.CustomEntityId = @CustomEntityId and WorkFlowStatusId = 4
	)

	merge into Cofoundry.CustomEntityPublishStatusQuery as t
	using (
			select CustomEntityId, CustomEntityVersionId 
			from CTE_LatestPublishedCustomEntityVersions
			where RowNumber = 1
			) as s
		on t.CustomEntityId = s.CustomEntityId
		and t.PublishStatusQueryId = 0
		when matched then
			update set CustomEntityVersionId = s.CustomEntityVersionId
		when not matched by target then 
			insert (CustomEntityId, PublishStatusQueryId, CustomEntityVersionId)
			values(CustomEntityId, 0, CustomEntityVersionId)
		when not matched by source and t.PublishStatusQueryId = 0 and t.CustomEntityId = @CustomEntityId then
			delete;
	
	-- PublishStatusQuery.Latest

	with CTE_LatestCustomEntityVersions as
	(
		select CustomEntityId, CustomEntityVersionId,
			row_number() over (partition by CustomEntityId order by WorkFlowStatusId, CreateDate desc) as RowNumber
		from Cofoundry.CustomEntityVersion
		where CustomEntityId = @CustomEntityId
	)
	merge into Cofoundry.CustomEntityPublishStatusQuery as t
	using (
			select CustomEntityId, CustomEntityVersionId
			from CTE_LatestCustomEntityVersions
			where RowNumber = 1
			) as s
		on t.CustomEntityId = s.CustomEntityId
		and t.PublishStatusQueryId = 1
		when matched then
			update set CustomEntityVersionId = s.CustomEntityVersionId
		when not matched by target then 
			insert (CustomEntityId, PublishStatusQueryId, CustomEntityVersionId)
			values(CustomEntityId, 1, CustomEntityVersionId)
		when not matched by source and t.PublishStatusQueryId = 1 and t.CustomEntityId = @CustomEntityId then
			delete;

	-- PublishStatusQuery.Draft

	with CTE_LatestDraftCustomEntityVersions as
	(
	   select CustomEntityId, CustomEntityVersionId,
			 row_number() over (partition by CustomEntityId order by CreateDate desc) as RowNumber
	   from Cofoundry.CustomEntityVersion
	   where CustomEntityId = @CustomEntityId and WorkFlowStatusId = 1
	)
	merge into Cofoundry.CustomEntityPublishStatusQuery as t
	using (
			select CustomEntityId, CustomEntityVersionId 
			from CTE_LatestDraftCustomEntityVersions
			where RowNumber = 1
			) as s
		on t.CustomEntityId = s.CustomEntityId
		and t.PublishStatusQueryId = 2
		when matched then
			update set CustomEntityVersionId = s.CustomEntityVersionId
		when not matched by target then 
			insert (CustomEntityId, PublishStatusQueryId, CustomEntityVersionId)
			values(CustomEntityId, 2, CustomEntityVersionId)
		when not matched by source and t.PublishStatusQueryId = 2 and t.CustomEntityId = @CustomEntityId then
			delete;
	
	-- PublishStatusQuery.PreferPublished
	
	with CTE_PreferPublishedCustomEntityVersions as
	(
	   select CustomEntityId, CustomEntityVersionId,
			 row_number() over (partition by CustomEntityId order by WorkFlowStatusId desc, CreateDate desc) as RowNumber
	   from Cofoundry.CustomEntityVersion
	   where CustomEntityId = @CustomEntityId and  WorkFlowStatusId in (1, 4)
	)

	merge into Cofoundry.CustomEntityPublishStatusQuery as t
	using (
			select CustomEntityId, CustomEntityVersionId 
			from CTE_PreferPublishedCustomEntityVersions
			where RowNumber = 1
			) as s
		on t.CustomEntityId = s.CustomEntityId
		and t.PublishStatusQueryId = 3
		when matched then
			update set CustomEntityVersionId = s.CustomEntityVersionId
		when not matched by target then 
			insert (CustomEntityId, PublishStatusQueryId, CustomEntityVersionId)
			values(CustomEntityId, 3, CustomEntityVersionId)
		when not matched by source and t.PublishStatusQueryId = 3 and t.CustomEntityId = @CustomEntityId then
			delete;

end