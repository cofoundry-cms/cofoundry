create procedure Cofoundry.PagePublishStatusQuery_Update
	(
		@PageId int
	)
	as
begin
	
	set nocount on;
			
	-- PublishStatusQuery.Published

	with CTE_LatestPublishedPageVersions as
	(
	   select v.PageId, v.PageVersionId,
			 row_number() over (partition by v.PageId order by v.CreateDate desc) as RowNumber
	   from Cofoundry.PageVersion v
	   inner join Cofoundry.[Page] p on p.PageId = v.PageId
	   where v.PageId = @PageId and WorkFlowStatusId = 4 and p.PublishStatusCode = 'P'
	)

	merge into Cofoundry.PagePublishStatusQuery as t
	using (
			select PageId, PageVersionId 
			from CTE_LatestPublishedPageVersions
			where RowNumber = 1
			) as s
		on t.PageId = s.PageId
		and t.PublishStatusQueryId = 0
		when matched then
			update set PageVersionId = s.PageVersionId
		when not matched by target then 
			insert (PageId, PublishStatusQueryId, PageVersionId)
			values(PageId, 0, PageVersionId)
		when not matched by source and t.PublishStatusQueryId = 0 and t.PageId = @PageId then
			delete;
	
	-- PublishStatusQuery.Latest

	with CTE_LatestPageVersions as
	(
		select v.PageId, v.PageVersionId,
				row_number() over (partition by v.PageId order by v.WorkFlowStatusId, v.CreateDate desc) as RowNumber
		from Cofoundry.PageVersion v
		inner join Cofoundry.[Page] p on p.PageId = v.PageId
		where v.PageId = @PageId
	)
	merge into Cofoundry.PagePublishStatusQuery as t
	using (
			select PageId, PageVersionId
			from CTE_LatestPageVersions
			where RowNumber = 1
			) as s
		on t.PageId = s.PageId
		and t.PublishStatusQueryId = 1
		when matched then
			update set PageVersionId = s.PageVersionId
		when not matched by target then 
			insert (PageId, PublishStatusQueryId, PageVersionId)
			values(PageId, 1, PageVersionId)
		when not matched by source and t.PublishStatusQueryId = 1 and t.PageId = @PageId then
			delete;

	-- PublishStatusQuery.Draft

	with CTE_LatestDraftPageVersions as
	(
	   select v.PageId, v.PageVersionId,
			 row_number() over (partition by v.PageId order by v.CreateDate desc) as RowNumber
	   from Cofoundry.PageVersion v
	   inner join Cofoundry.[Page] p on p.PageId = v.PageId
	   where v.PageId = @PageId and WorkFlowStatusId = 1
	)
	merge into Cofoundry.PagePublishStatusQuery as t
	using (
			select PageId, PageVersionId 
			from CTE_LatestDraftPageVersions
			where RowNumber = 1
			) as s
		on t.PageId = s.PageId
		and t.PublishStatusQueryId = 2
		when matched then
			update set PageVersionId = s.PageVersionId
		when not matched by target then 
			insert (PageId, PublishStatusQueryId, PageVersionId)
			values(PageId, 2, PageVersionId)
		when not matched by source and t.PublishStatusQueryId = 2 and t.PageId = @PageId then
			delete;
	
	-- PublishStatusQuery.PreferPublished
	
	with CTE_PreferPublishedPageVersions as
	(
	   select v.PageId, v.PageVersionId,
			 row_number() over (partition by v.PageId order by v.WorkFlowStatusId desc, v.CreateDate desc) as RowNumber
	   from Cofoundry.PageVersion v
	   inner join Cofoundry.[Page] p on p.PageId = v.PageId
	   where v.PageId = @PageId and WorkFlowStatusId in (1, 4)
	)

	merge into Cofoundry.PagePublishStatusQuery as t
	using (
			select PageId, PageVersionId 
			from CTE_PreferPublishedPageVersions
			where RowNumber = 1
			) as s
		on t.PageId = s.PageId
		and t.PublishStatusQueryId = 3
		when matched then
			update set PageVersionId = s.PageVersionId
		when not matched by target then 
			insert (PageId, PublishStatusQueryId, PageVersionId)
			values(PageId, 3, PageVersionId)
		when not matched by source and t.PublishStatusQueryId = 3 and t.PageId = @PageId then
			delete;

end