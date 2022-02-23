namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to update general settings.
    /// </summary>
    public class GeneralSettingsUpdatePermission : IEntityPermission
    {
        public GeneralSettingsUpdatePermission()
        {
            EntityDefinition = new SettingsEntityDefinition();
            PermissionType = new PermissionType("GENUPD", "Update General", "Update General Settings");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}