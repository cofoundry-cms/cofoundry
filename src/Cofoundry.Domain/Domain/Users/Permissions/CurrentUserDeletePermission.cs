namespace Cofoundry.Domain
{
    public class CurrentUserDeletePermission : IEntityPermission
    {
        public CurrentUserDeletePermission()
        {
            EntityDefinition = new CurrentUserEntityDefinition();
            PermissionType = CommonPermissionTypes.Delete("Current User");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
