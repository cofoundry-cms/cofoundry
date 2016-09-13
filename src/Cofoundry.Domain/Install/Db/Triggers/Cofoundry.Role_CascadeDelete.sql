create trigger Cofoundry.Role_CascadeDelete
   on  Cofoundry.[Role]
   instead of delete
AS 
begin
	set nocount on;
	if not exists (select * from deleted) return
	
	-- Dependencies
    delete Cofoundry.RolePermission
	from Cofoundry.RolePermission e
	inner join deleted d on e.RoleId = d.RoleId

	-- Main Table
    delete Cofoundry.[Role]
	from Cofoundry.[Role] e
	inner join deleted d on e.RoleId = d.RoleId

end