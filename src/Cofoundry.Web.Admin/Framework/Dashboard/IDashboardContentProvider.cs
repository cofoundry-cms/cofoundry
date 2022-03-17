using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web.Admin;

/// <summary>
/// Used to get the html content for the admin panel dashboard.
/// </summary>
public interface IDashboardContentProvider
{
    /// <summary>
    /// Returns the html content to be injected into the page for the admin 
    /// panel dashboard.
    /// </summary>
    Task<IHtmlContent> GetAsync();
}
