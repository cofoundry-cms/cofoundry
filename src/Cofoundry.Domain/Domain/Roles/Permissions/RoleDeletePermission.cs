namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to delete a role.
    /// </summary>
    public class RoleDeletePermission : IEntityPermission
    {
        public RoleDeletePermission()
        {
            EntityDefinition = new RoleEntityDefinition();
            PermissionType = CommonPermissionTypes.Delete("Roles");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}