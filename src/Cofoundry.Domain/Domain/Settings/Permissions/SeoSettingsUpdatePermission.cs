namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to update SEO settings.
    /// </summary>
    public class SeoSettingsUpdatePermission : IEntityPermission
    {
        public SeoSettingsUpdatePermission()
        {
            EntityDefinition = new SettingsEntityDefinition();
            PermissionType = new PermissionType("SEOUPD", "Update SEO", "Update SEO Settings");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}