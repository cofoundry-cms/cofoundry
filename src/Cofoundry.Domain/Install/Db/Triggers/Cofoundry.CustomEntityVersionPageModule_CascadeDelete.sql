create trigger Cofoundry.CustomEntityVersionPageModule_CascadeDelete
   on  Cofoundry.CustomEntityVersionPageModule
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	
	declare @DefinitionCode char(6) = 'COFCEM'

	-- Dependencies

	delete from Cofoundry.UnstructuredDataDependency
	from Cofoundry.UnstructuredDataDependency e
	inner join deleted d on e.RootEntityId = d.CustomEntityVersionPageModuleId and RootEntityDefinitionCode = @DefinitionCode

	-- Main Table
    delete Cofoundry.CustomEntityVersionPageModule
	from Cofoundry.CustomEntityVersionPageModule e
	inner join deleted d on e.CustomEntityVersionPageModuleId = d.CustomEntityVersionPageModuleId

end