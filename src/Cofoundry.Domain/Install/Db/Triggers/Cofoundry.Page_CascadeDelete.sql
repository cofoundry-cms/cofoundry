create trigger Cofoundry.Page_CascadeDelete
   on  Cofoundry.[Page]
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	declare @DefinitionCode char(6) = 'COFPGE'
	
	-- Dependencies
	delete from Cofoundry.UnstructuredDataDependency
	from Cofoundry.UnstructuredDataDependency e
	inner join deleted d on e.RootEntityId = d.PageId and RootEntityDefinitionCode = @DefinitionCode

    delete Cofoundry.PageGroupItem
	from Cofoundry.PageGroupItem e
	inner join deleted d on e.PageId = d.PageId

    delete Cofoundry.PageTag
	from Cofoundry.PageTag e
	inner join deleted d on e.PageId = d.PageId

    delete Cofoundry.PageVersion
	from Cofoundry.PageVersion e
	inner join deleted d on e.PageId = d.PageId

    delete Cofoundry.PagePublishStatusQuery
	from Cofoundry.PagePublishStatusQuery e
	inner join deleted d on e.PageId = d.PageId

    delete Cofoundry.CustomEntityVersionPageBlock
	from Cofoundry.CustomEntityVersionPageBlock e
	inner join deleted d on e.PageId = d.PageId

	-- Main Table
    delete Cofoundry.[Page]
	from Cofoundry.[Page] e
	inner join deleted d on e.PageId = d.PageId

end