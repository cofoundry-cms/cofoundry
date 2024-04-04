namespace Cofoundry.Web.Admin;

/// <summary>
/// A base class for module url/content routing information.
/// </summary>
public class ModuleRouteLibrary
{
    private const string CONTENT_FOLDER = "Content";
    private readonly AdminSettings _adminSettings;

    /// <summary>
    /// Constructor when you want to use the default resource path (i.e. your
    /// module is located in /cofoundry/admin/modules/{modulename}.
    /// </summary>
    /// <param name="adminSettings">
    /// Admin configuration settings typically resolved form the DI container.
    /// </param>
    /// <param name="routePrefix">
    /// The unique entity/folder name of your route to use when building 
    /// urls e.g. products, users, honeybagders.
    /// </param>
    public ModuleRouteLibrary(
        AdminSettings adminSettings,
        string routePrefix
        )
        : this(adminSettings, routePrefix, RouteConstants.ModuleResourcePathPrefix)
    {
    }

    /// <summary>
    /// Constructor to use when you want to provide a custom resource path prefix, for
    /// when your module is located in a non-standard directory (or for all internal 
    /// modules where you pass RouteConstants.InternalModuleResourcePathPrefix).
    /// </summary>
    /// <param name="adminSettings">
    /// Admin configuration settings typically resolved form the DI container.
    /// </param>
    /// <param name="routePrefix">
    /// The unique entity/folder name of your route to use when building 
    /// urls e.g. products, users, honeybagders.
    /// </param>
    /// <param name="resourcePathPrefix">The path prefix to your module e.g. '/admin/modules/'</param>
    public ModuleRouteLibrary(
        AdminSettings adminSettings,
        string routePrefix,
        string resourcePathPrefix
        )
    {
        ModuleFolderName = TextFormatter.Pascalize(routePrefix);
        ResourcePrefix = resourcePathPrefix + ModuleFolderName + "/";
        StaticResourcePrefix = RelativePathHelper.Combine(ResourcePrefix, CONTENT_FOLDER);
        UrlPrefix = routePrefix;

        _adminSettings = adminSettings;
    }

    /// <summary>
    /// The name of the folder that contains the module (typically the entity name). Used
    /// for formatting routes.
    /// </summary>
    public string ModuleFolderName { get; private set; }

    /// <summary>
    /// For constructing resource paths i.e. "Modules/Entity/"
    /// </summary>
    /// <remarks>
    /// Note that resource paths have to mimic the directory structure
    /// because it is based on the embedded resource name.
    /// </remarks>
    public string ResourcePrefix { get; private set; }

    /// <summary>
    /// Route for the static resource folder i.e. "/AdminModules/Entity/Content"
    /// </summary>
    public string StaticResourcePrefix { get; private set; }

    /// <summary>
    /// Prefix for constructing navigation urls i.e. "entity"
    /// </summary>
    public string UrlPrefix { get; private set; }

    public string StaticResource(string fileName)
    {
        return RelativePathHelper.Combine(_adminSettings.DirectoryName, StaticResourcePrefix, fileName);
    }

    public string StaticResourceFilePath(string fileName)
    {
        return RelativePathHelper.Combine(StaticResourcePrefix, fileName);
    }

    public string JsDirectory()
    {
        return RelativePathHelper.Combine(_adminSettings.DirectoryName, StaticResourcePrefix, "js");
    }

    public string JsFile(string fileName)
    {
        return RelativePathHelper.Combine(_adminSettings.DirectoryName, StaticResourcePrefix, "js", fileName + ".js");
    }

    public string CssDirectory()
    {
        return RelativePathHelper.Combine(_adminSettings.DirectoryName, StaticResourcePrefix, "css");
    }

    public string CssFile(string fileName)
    {
        return RelativePathHelper.Combine(_adminSettings.DirectoryName, StaticResourcePrefix, "css", fileName + ".css");
    }

    public string GetStaticResourceFilePath()
    {
        return StaticResourcePrefix;
    }

    public string GetStaticResourceUrlPath()
    {
        return RelativePathHelper.Combine(_adminSettings.DirectoryName, StaticResourcePrefix);
    }

    public string AngularRoute(string? path = null)
    {
        return "/" + _adminSettings.DirectoryName + "/" + UrlPrefix + "#/" + path;
    }

    public string MvcRoute(string? action = null, string? qs = null)
    {
        if (action == null)
        {
            return "/" + _adminSettings.DirectoryName + "/" + UrlPrefix + qs;
        }

        return "/" + _adminSettings.DirectoryName + "/" + UrlPrefix + "/" + action + qs;
    }
}
