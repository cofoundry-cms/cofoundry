namespace Cofoundry.Web.Admin;

/// <summary>
/// This is a little helper to make it easier to reference views by
/// full path without having to go through the view locator. This means
/// we don't have to register the admin view path with the view engine 
/// so we can avoid potential conflicts.
/// </summary>
internal static class ViewPathFormatter
{
    public static string View(string controllerName, string viewName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(controllerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(viewName);

        var viewPath = $"~{RouteConstants.InternalModuleResourcePathPrefix}{controllerName}/MVC/Views/{viewName}.cshtml";
        return viewPath;
    }
}
