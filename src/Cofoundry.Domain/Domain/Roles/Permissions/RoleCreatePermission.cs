namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to create new roles.
    /// </summary>
    public class RoleCreatePermission : IEntityPermission
    {
        public RoleCreatePermission()
        {
            EntityDefinition = new RoleEntityDefinition();
            PermissionType = CommonPermissionTypes.Create("Roles");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}