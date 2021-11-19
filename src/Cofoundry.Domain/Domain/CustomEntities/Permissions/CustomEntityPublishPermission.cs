namespace Cofoundry.Domain
{
    public class CustomEntityPublishPermission : ICustomEntityPermissionTemplate
    {
        /// <summary>
        /// Constructor used internally by AuthorizePermissionAttribute.
        /// </summary>
        public CustomEntityPublishPermission()
        {
            PermissionType = CreatePermissionType("Not Set");
        }

        public CustomEntityPublishPermission(ICustomEntityDefinition customEntityDefinition)
        {
            EntityDefinition = new CustomEntityDynamicEntityDefinition(customEntityDefinition);
            PermissionType = CreatePermissionType(customEntityDefinition.Name.ToLower());
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }

        public ICustomEntityPermissionTemplate CreateImplemention(ICustomEntityDefinition customEntityDefinition)
        {
            var implementedPermission = new CustomEntityPublishPermission(customEntityDefinition);
            return implementedPermission;
        }

        private static PermissionType CreatePermissionType(string customEntityName)
        {
            return new PermissionType("CMEPUB", "Publish", "Publish or unpublish a " + customEntityName);
        }
    }
}
