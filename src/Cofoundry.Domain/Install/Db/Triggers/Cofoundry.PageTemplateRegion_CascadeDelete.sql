create trigger Cofoundry.PageTemplateRegion_CascadeDelete
   on  Cofoundry.PageTemplateRegion
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	
	-- Dependencies
    delete Cofoundry.PageVersionBlock
	from Cofoundry.PageVersionBlock e
	inner join deleted d on e.PageTemplateRegionId = d.PageTemplateRegionId

    delete Cofoundry.CustomEntityVersionPageBlock
	from Cofoundry.CustomEntityVersionPageBlock e
	inner join deleted d on e.PageTemplateRegionId = d.PageTemplateRegionId

	-- Main Table
    delete Cofoundry.PageTemplateRegion
	from Cofoundry.PageTemplateRegion e
	inner join deleted d on e.PageTemplateRegionId = d.PageTemplateRegionId

end