create trigger Cofoundry.Role_CascadeDelete
   on  Cofoundry.[Role]
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return

	declare @DefinitionCode char(6) = 'COFROL'
	
	-- Dependencies
    delete Cofoundry.RolePermission
	from Cofoundry.RolePermission e
	inner join deleted d on e.RoleId = d.RoleId

    delete Cofoundry.PageDirectoryAccessRule
	from Cofoundry.PageDirectoryAccessRule e
	inner join deleted d on e.RoleId = d.RoleId

	-- NB: related entity cascade constraints are enforced at the domain layer, so here we just need to clear everything
	delete from Cofoundry.UnstructuredDataDependency
	from Cofoundry.UnstructuredDataDependency e
	inner join deleted d on (e.RootEntityId = d.RoleId and RootEntityDefinitionCode = @DefinitionCode) or (e.RelatedEntityId = d.RoleId and RelatedEntityDefinitionCode = @DefinitionCode)

	-- Main Table
    delete Cofoundry.[Role]
	from Cofoundry.[Role] e
	inner join deleted d on e.RoleId = d.RoleId

end