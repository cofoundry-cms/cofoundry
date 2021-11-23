create trigger Cofoundry.ImageAsset_CascadeDelete
   on  Cofoundry.ImageAsset
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return

	declare @DefinitionCode char(6) = 'COFIMG'
	
	-- Dependencies

	update Cofoundry.PageVersion
	set OpenGraphImageId = null
	from Cofoundry.PageVersion e
	inner join deleted d on e.OpenGraphImageId = d.ImageAssetId

    delete Cofoundry.ImageAssetGroupItem
	from Cofoundry.ImageAssetGroupItem e
	inner join deleted d on e.ImageAssetId = d.ImageAssetId

    delete Cofoundry.ImageAssetTag
	from Cofoundry.ImageAssetTag e
	inner join deleted d on e.ImageAssetId = d.ImageAssetId

	-- NB: related entity cascade constraints are enforced at the domain layer, so here we just need to clear everything
	delete from Cofoundry.UnstructuredDataDependency
	from Cofoundry.UnstructuredDataDependency e
	inner join deleted d on (e.RootEntityId = d.ImageAssetId and RootEntityDefinitionCode = @DefinitionCode) or (e.RelatedEntityId = d.ImageAssetId and RelatedEntityDefinitionCode = @DefinitionCode)
	
	-- Main Table
    delete Cofoundry.ImageAsset
	from Cofoundry.ImageAsset e
	inner join deleted d on e.ImageAssetId = d.ImageAssetId

end