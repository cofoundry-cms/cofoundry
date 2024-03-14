namespace Cofoundry.Web.Admin;

public class VisualEditorRouteLibrary : AngularModuleRouteLibrary
{
    public const string RoutePrefix = "visual-editor";

    public VisualEditorRouteLibrary(AdminSettings adminSettings)
        : base(adminSettings, RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
    {
    }

    public string VisualEditorToolbarViewPath()
    {
        return ResourcePrefix + "MVC/Views/Toolbar.cshtml";
    }
}