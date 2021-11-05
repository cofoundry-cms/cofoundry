-- =============================================
-- Regenerates the PageDirectoryClosure table, upserting any 
-- missing or out of date values. After the update is completed
-- the PageDirectoryPath table is also updated, because the path
-- data is dependent on the data in the closure table.
-- 
-- This procedure should be called whenever page directories
-- are added, or the heirachy is updated by changing 
-- ParentDirectoryIds. Although the procedure does handle
-- deletions, it's not necessary to run it for deletions as this 
-- is also handled by the page directory cascade 
-- delete trigger. 
-- =============================================
create procedure Cofoundry.PageDirectoryClosure_Update
	as
begin
	
	set nocount on;

	with DirectoryCTE as 
	(
		select 
			PageDirectoryId as AncestorPageDirectoryId, 
			PageDirectoryId as DescendantPageDirectoryId, 
			0 as Distance

		from Cofoundry.PageDirectory
	 
		union all

		select
			cte.AncestorPageDirectoryId,
			d.PageDirectoryId as DescendantPageDirectoryId,
			cte.Distance + 1 AS Distance
		from Cofoundry.PageDirectory as d
		inner join DirectoryCTE AS cte on d.ParentPageDirectoryId = cte.DescendantPageDirectoryId
	)
	merge into Cofoundry.PageDirectoryClosure as t
	using (
		select AncestorPageDirectoryId, DescendantPageDirectoryId, Distance
		from DirectoryCTE
		) as s
	on t.AncestorPageDirectoryId = s.AncestorPageDirectoryId and t.DescendantPageDirectoryId = s.DescendantPageDirectoryId
	when matched and t.Distance <> s.Distance then 
		update set Distance = s.Distance
	when not matched by target then
		insert (AncestorPageDirectoryId, DescendantPageDirectoryId, Distance)
		values (AncestorPageDirectoryId, DescendantPageDirectoryId, Distance)
	when not matched by source then
		delete;

	-- PageDirectoryPath is dependent on the closure table, so update it
	exec Cofoundry.PageDirectoryPath_Update;

end