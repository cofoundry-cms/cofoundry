create trigger Cofoundry.PageDirectory_CascadeDelete
   on  Cofoundry.PageDirectory
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	declare @DefinitionCode char(6) = 'COFDIR'
	declare @PageDirectoryToDelete table (PageDirectoryId int);

	with cteExpandedDirectories as (
		select PageDirectoryId as RootDirectoryId, PageDirectoryId from Cofoundry.PageDirectory
		union all
		select ed.RootDirectoryId, pd.PageDirectoryId 
		from cteExpandedDirectories ed
		inner join Cofoundry.PageDirectory pd on pd.ParentPageDirectoryId = ed.PageDirectoryId
	)
	insert into @PageDirectoryToDelete
	select distinct ed.PageDirectoryId
	from cteExpandedDirectories ed
	inner join deleted d on d.PageDirectoryId = ed.RootDirectoryId
	where ParentPageDirectoryId is not null -- ensure root dir is never deleted

	-- Dependencies
	
	-- NB: related entity cascade constraints are enforced at the domain layer, so here we just need to clear everything
	delete from Cofoundry.UnstructuredDataDependency
	from Cofoundry.UnstructuredDataDependency e
	inner join @PageDirectoryToDelete d on (e.RootEntityId = d.PageDirectoryId and RootEntityDefinitionCode = @DefinitionCode) or (e.RelatedEntityId = d.PageDirectoryId and RelatedEntityDefinitionCode = @DefinitionCode)
	

    delete Cofoundry.[Page]
	from Cofoundry.[Page] e
	inner join @PageDirectoryToDelete d on e.PageDirectoryId = d.PageDirectoryId

    delete Cofoundry.PageDirectoryAccessRule
	from Cofoundry.PageDirectoryAccessRule e
	inner join @PageDirectoryToDelete d on e.PageDirectoryId = d.PageDirectoryId

    delete Cofoundry.PageDirectoryPath
	from Cofoundry.PageDirectoryPath e
	inner join @PageDirectoryToDelete d on e.PageDirectoryId = d.PageDirectoryId

    delete Cofoundry.PageDirectoryClosure
	from Cofoundry.PageDirectoryClosure e
	inner join @PageDirectoryToDelete d on e.DescendantPageDirectoryId = d.PageDirectoryId or e.AncestorPageDirectoryId = d.PageDirectoryId

    delete Cofoundry.PageDirectoryLocale
	from Cofoundry.PageDirectoryLocale e
	inner join @PageDirectoryToDelete d on e.PageDirectoryId = d.PageDirectoryId

    delete Cofoundry.PageDirectory
	from Cofoundry.PageDirectory e
	inner join @PageDirectoryToDelete d on e.ParentPageDirectoryId = d.PageDirectoryId

	-- Main Table
    delete Cofoundry.PageDirectory
	from Cofoundry.PageDirectory e
	inner join @PageDirectoryToDelete d on e.PageDirectoryId = d.PageDirectoryId

end