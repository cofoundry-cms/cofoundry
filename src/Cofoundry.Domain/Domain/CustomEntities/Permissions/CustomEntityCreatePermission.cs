namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to create new custom entities.
    /// </summary>
    public class CustomEntityCreatePermission : ICustomEntityPermissionTemplate
    {
        /// <summary>
        /// Constructor used internally by AuthorizePermissionAttribute.
        /// </summary>
        public CustomEntityCreatePermission()
        {
            PermissionType = CommonPermissionTypes.Create("Not Set");
        }

        public CustomEntityCreatePermission(ICustomEntityDefinition customEntityDefinition)
        {
            EntityDefinition = new CustomEntityDynamicEntityDefinition(customEntityDefinition);
            PermissionType = CommonPermissionTypes.Create(customEntityDefinition.NamePlural);
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }

        public ICustomEntityPermissionTemplate CreateImplemention(ICustomEntityDefinition customEntityDefinition)
        {
            var implementedPermission = new CustomEntityCreatePermission(customEntityDefinition);
            return implementedPermission;
        }
    }
}