-- =============================================
-- Regenerates the PageDirectoryPath table, upserting any missing
-- or out of date values. This only needs to be called when the UrlPath
-- of a page directory changes because in most other scenarios either the
-- Cofoundry.PageDirectoryClosure_Update stored procedure makes the same 
-- changes or for deletions the cascade delete trigger handles any necessary
-- deletions.
-- =============================================
create procedure Cofoundry.PageDirectoryPath_Update
	as
begin
	
	set nocount on;

	merge into Cofoundry.PageDirectoryPath as t
	using (
		select 
			c.DescendantPageDirectoryId as PageDirectoryId, 
			IsNull((
				select Stuff((
					select N'/' + d.UrlPath 
					from Cofoundry.PageDirectoryClosure c2
					inner join Cofoundry.PageDirectory d on c2.AncestorPageDirectoryId = d.PageDirectoryId
					where c2.DescendantPageDirectoryId = c.DescendantPageDirectoryId and d.ParentPageDirectoryId is not null
					order by c2.Distance desc
					for xml path('')
				) ,1 ,1, N'')
			), '') as FullUrlPath,
			Max(c.Distance) as Depth
		from Cofoundry.PageDirectoryClosure c
		group by c.DescendantPageDirectoryId
	) as s
	on t.PageDirectoryId = s.PageDirectoryId
	when matched and t.FullUrlPath <> s.FullUrlPath or t.Depth <> s.Depth  then 
		update set FullUrlPath = s.FullUrlPath, Depth = s.Depth
	when not matched by target then
		insert (PageDirectoryId, FullUrlPath, Depth)
		values (PageDirectoryId, FullUrlPath, Depth)
	when not matched by source then
		delete;

end