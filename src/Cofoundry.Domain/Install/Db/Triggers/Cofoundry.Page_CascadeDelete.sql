create trigger Cofoundry.Page_CascadeDelete
   on  Cofoundry.[Page]
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	declare @DefinitionCode char(6) = 'COFPGE'
	
	-- Dependencies

    delete Cofoundry.PageGroupItem
	from Cofoundry.PageGroupItem e
	inner join deleted d on e.PageId = d.PageId

    delete Cofoundry.PageTag
	from Cofoundry.PageTag e
	inner join deleted d on e.PageId = d.PageId

    delete Cofoundry.PageVersion
	from Cofoundry.PageVersion e
	inner join deleted d on e.PageId = d.PageId

    delete Cofoundry.PageAccessRule
	from Cofoundry.PageAccessRule e
	inner join deleted d on e.PageId = d.PageId

    delete Cofoundry.PagePublishStatusQuery
	from Cofoundry.PagePublishStatusQuery e
	inner join deleted d on e.PageId = d.PageId

    delete Cofoundry.CustomEntityVersionPageBlock
	from Cofoundry.CustomEntityVersionPageBlock e
	inner join deleted d on e.PageId = d.PageId

	-- NB: related entity cascade constraints are enforced at the domain layer, so here we just need to clear everything
	delete from Cofoundry.UnstructuredDataDependency
	from Cofoundry.UnstructuredDataDependency e
	inner join deleted d on (e.RootEntityId = d.PageId and RootEntityDefinitionCode = @DefinitionCode) or (e.RelatedEntityId = d.PageId and RelatedEntityDefinitionCode = @DefinitionCode)

	-- Main Table
    delete Cofoundry.[Page]
	from Cofoundry.[Page] e
	inner join deleted d on e.PageId = d.PageId

end