namespace Cofoundry.Web.Admin;

public class CustomEntitiesRouteLibrary : AngularModuleRouteLibrary
{
    public const string RoutePrefix = "custom-entities";
    private readonly AdminSettings _adminSettings;

    public CustomEntitiesRouteLibrary(AdminSettings adminSettings)
        : base(adminSettings, RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
    {
        _adminSettings = adminSettings;
    }

    public string List(CustomEntityDefinitionSummary definition)
    {
        return GetCustomEntityRoute(definition?.NamePlural);
    }

    public string List(ICustomEntityDefinition definition)
    {
        return GetCustomEntityRoute(definition?.NamePlural);
    }

    public string New(CustomEntityDefinitionSummary definition)
    {
        if (definition == null)
        {
            return string.Empty;
        }

        return List(definition) + "new";
    }

    public string Details(CustomEntityDefinitionSummary definition, int id)
    {
        if (definition == null)
        {
            return string.Empty;
        }

        return List(definition) + id.ToString(CultureInfo.InvariantCulture);
    }

    private string GetCustomEntityRoute(string? namePlural, string? route = null)
    {
        if (namePlural == null)
        {
            return string.Empty;
        }

        return "/" + _adminSettings.DirectoryName + "/" + SlugFormatter.ToSlug(namePlural) + "#/" + route;
    }
}
