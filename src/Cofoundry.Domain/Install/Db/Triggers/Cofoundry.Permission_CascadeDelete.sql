create trigger Cofoundry.Permission_CascadeDelete
   on  Cofoundry.[Permission]
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	
	-- Dependencies
    delete Cofoundry.RolePermission
	from Cofoundry.RolePermission e
	inner join deleted d on e.PermissionId = d.PermissionId

	-- Main Table
    delete Cofoundry.[Permission]
	from Cofoundry.[Permission] e
	inner join deleted d on e.PermissionId = d.PermissionId

end