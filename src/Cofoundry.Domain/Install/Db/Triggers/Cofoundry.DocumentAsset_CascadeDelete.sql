create trigger Cofoundry.DocumentAsset_CascadeDelete
   on  Cofoundry.DocumentAsset
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	
	-- Dependencies
    delete Cofoundry.DocumentAssetTag
	from Cofoundry.DocumentAssetTag e
	inner join deleted d on e.DocumentAssetId = d.DocumentAssetId

    delete Cofoundry.DocumentAssetGroupItem
	from Cofoundry.DocumentAssetGroupItem e
	inner join deleted d on e.DocumentAssetId = d.DocumentAssetId

	-- Main Table
    delete Cofoundry.DocumentAsset
	from Cofoundry.DocumentAsset e
	inner join deleted d on e.DocumentAssetId = d.DocumentAssetId

end