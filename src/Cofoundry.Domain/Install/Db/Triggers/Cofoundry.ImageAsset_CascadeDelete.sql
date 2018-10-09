create trigger Cofoundry.ImageAsset_CascadeDelete
   on  Cofoundry.ImageAsset
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	
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

	-- Main Table
    delete Cofoundry.ImageAsset
	from Cofoundry.ImageAsset e
	inner join deleted d on e.ImageAssetId = d.ImageAssetId

end