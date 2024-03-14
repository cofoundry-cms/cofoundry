namespace Cofoundry.Web.Admin;

public class AdminMenuViewModel
{
    public IReadOnlyCollection<AdminModule> ManageSiteModules { get; set; } = Array.Empty<AdminModule>();

    public IReadOnlyCollection<AdminModule> SettingsModules { get; set; } = Array.Empty<AdminModule>();

    public AdminModuleMenuCategory? SelectedCategory { get; set; }

    public AdminModule? SelectedModule { get; set; }
}
