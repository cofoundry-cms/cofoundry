namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to update the UrlSlug and locale of a custom entity which often forms
    /// the identity of the entity and can form part of the URL when used in
    /// custom entity pages.
    /// </summary>
    public class CustomEntityUpdateUrlPermission : ICustomEntityPermissionTemplate
    {
        /// <summary>
        /// Constructor used internally by AuthorizePermissionAttribute.
        /// </summary>
        public CustomEntityUpdateUrlPermission()
        {
            PermissionType = CreatePermissionType("Not Set");
        }

        public CustomEntityUpdateUrlPermission(ICustomEntityDefinition customEntityDefinition)
        {
            EntityDefinition = new CustomEntityDynamicEntityDefinition(customEntityDefinition);
            PermissionType = new PermissionType("UPDURL", "Update custom entity Url", "Update the url of a " + customEntityDefinition.Name);
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }

        public ICustomEntityPermissionTemplate CreateImplemention(ICustomEntityDefinition customEntityDefinition)
        {
            var implementedPermission = new CustomEntityUpdateUrlPermission(customEntityDefinition);
            return implementedPermission;
        }

        private static PermissionType CreatePermissionType(string customEntityName)
        {
            return new PermissionType("UPDURL", "Update custom entity Url", "Update the url of a " + customEntityName);
        }
    }
}