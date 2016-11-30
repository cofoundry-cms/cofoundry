create trigger Cofoundry.PageTemplateSection_CascadeDelete
   on  Cofoundry.PageTemplateSection
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	
	-- Dependencies
    delete Cofoundry.PageVersionModule
	from Cofoundry.PageVersionModule e
	inner join deleted d on e.PageTemplateSectionId = d.PageTemplateSectionId

    delete Cofoundry.CustomEntityVersionPageModule
	from Cofoundry.CustomEntityVersionPageModule e
	inner join deleted d on e.PageTemplateSectionId = d.PageTemplateSectionId

	-- Main Table
    delete Cofoundry.PageTemplateSection
	from Cofoundry.PageTemplateSection e
	inner join deleted d on e.PageTemplateSectionId = d.PageTemplateSectionId

end