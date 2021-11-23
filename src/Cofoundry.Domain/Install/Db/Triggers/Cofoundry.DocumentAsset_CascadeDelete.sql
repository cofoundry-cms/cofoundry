create trigger Cofoundry.DocumentAsset_CascadeDelete
   on  Cofoundry.DocumentAsset
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	
	declare @DefinitionCode char(6) = 'COFDOC'

	-- Dependencies

    delete Cofoundry.DocumentAssetTag
	from Cofoundry.DocumentAssetTag e
	inner join deleted d on e.DocumentAssetId = d.DocumentAssetId

    delete Cofoundry.DocumentAssetGroupItem
	from Cofoundry.DocumentAssetGroupItem e
	inner join deleted d on e.DocumentAssetId = d.DocumentAssetId

	-- NB: related entity cascade constraints are enforced at the domain layer, so here we just need to clear everything
	delete from Cofoundry.UnstructuredDataDependency
	from Cofoundry.UnstructuredDataDependency e
	inner join deleted d on (e.RootEntityId = d.DocumentAssetId and RootEntityDefinitionCode = @DefinitionCode) or (e.RelatedEntityId = d.DocumentAssetId and RelatedEntityDefinitionCode = @DefinitionCode)
	
	-- Main Table
    delete Cofoundry.DocumentAsset
	from Cofoundry.DocumentAsset e
	inner join deleted d on e.DocumentAssetId = d.DocumentAssetId

end