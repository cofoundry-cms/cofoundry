namespace Cofoundry.Web.Admin;

public class VisualEditorRouteLibrary : AngularModuleRouteLibrary
{
    public const string RoutePrefix = "visual-editor";
    private readonly AdminSettings _adminSettings;

    public VisualEditorRouteLibrary(AdminSettings adminSettings)
        : base(adminSettings, RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
    {
        _adminSettings = adminSettings;
    }

    public string VisualEditorToolbarViewPath()
    {
        return ResourcePrefix + "MVC/Views/Toolbar.cshtml";
    }
}