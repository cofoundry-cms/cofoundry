namespace Cofoundry.Web;

/// <summary>
/// Registers the default locations for page template view files
/// </summary>
public class DefaultPageTemplateViewLocationRegistration : IPageTemplateViewLocationRegistration
{
    public IEnumerable<string> GetPathPrefixes()
    {
        yield return "/Cofoundry/PageTemplates";
        yield return "/Views/PageTemplates";
    }
}
