create trigger Cofoundry.PageTemplate_CascadeDelete
   on  Cofoundry.PageTemplate
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	
	-- Dependencies
    delete Cofoundry.PageTemplateRegion
	from Cofoundry.PageTemplateRegion e
	inner join deleted d on e.PageTemplateId = d.PageTemplateId

	-- Main Table
    delete Cofoundry.PageTemplate
	from Cofoundry.PageTemplate e
	inner join deleted d on e.PageTemplateId = d.PageTemplateId

end