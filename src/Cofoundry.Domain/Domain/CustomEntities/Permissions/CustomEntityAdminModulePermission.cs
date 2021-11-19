namespace Cofoundry.Domain
{
    public class CustomEntityAdminModulePermission : ICustomEntityPermissionTemplate
    {
        /// <summary>
        /// Constructor used internally by AuthorizePermissionAttribute.
        /// </summary>
        public CustomEntityAdminModulePermission()
        {
            PermissionType = CommonPermissionTypes.AdminModule("Not Set");
        }

        public CustomEntityAdminModulePermission(ICustomEntityDefinition customEntityDefinition)
        {
            EntityDefinition = new CustomEntityDynamicEntityDefinition(customEntityDefinition);
            PermissionType = CommonPermissionTypes.AdminModule(customEntityDefinition.NamePlural);
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }

        public ICustomEntityPermissionTemplate CreateImplemention(ICustomEntityDefinition customEntityDefinition)
        {
            var implementedPermission = new CustomEntityAdminModulePermission(customEntityDefinition);
            return implementedPermission;
        }
    }
}
