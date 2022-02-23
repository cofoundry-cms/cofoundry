namespace Cofoundry.Domain
{
    /// <summary>
    /// Read access to a custom entity. Read access is required in order
    /// to include any other custom entity permissions.
    /// </summary>
    public class CustomEntityReadPermission : ICustomEntityPermissionTemplate
    {
        /// <summary>
        /// Constructor used internally by AuthorizePermissionAttribute.
        /// </summary>
        public CustomEntityReadPermission()
        {
            PermissionType = CommonPermissionTypes.Read("Not Set");
        }

        public CustomEntityReadPermission(ICustomEntityDefinition customEntityDefinition)
        {
            EntityDefinition = new CustomEntityDynamicEntityDefinition(customEntityDefinition);
            PermissionType = CommonPermissionTypes.Read(customEntityDefinition.NamePlural);
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }

        public ICustomEntityPermissionTemplate CreateImplemention(ICustomEntityDefinition customEntityDefinition)
        {
            var implementedPermission = new CustomEntityReadPermission(customEntityDefinition);
            return implementedPermission;
        }
    }
}