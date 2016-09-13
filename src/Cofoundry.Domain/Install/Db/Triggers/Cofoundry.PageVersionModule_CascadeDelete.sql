create trigger Cofoundry.PageVersionModule_CascadeDelete
   on  Cofoundry.PageVersionModule
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	
	declare @DefinitionCode char(6) = 'COFPGM'

	-- Dependencies

	delete from Cofoundry.UnstructuredDataDependency
	from Cofoundry.UnstructuredDataDependency e
	inner join deleted d on e.RootEntityId = d.PageVersionModuleId and RootEntityDefinitionCode = @DefinitionCode

	-- Main Table
    delete Cofoundry.PageVersionModule
	from Cofoundry.PageVersionModule e
	inner join deleted d on e.PageVersionModuleId = d.PageVersionModuleId

end