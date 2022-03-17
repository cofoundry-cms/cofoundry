namespace Cofoundry.Domain.Tests.Integration.Bootstrap;

public class PageTemplateViewLocationRegistration : IPageTemplateViewLocationRegistration
{
    public IEnumerable<string> GetPathPrefixes()
    {
        yield return "Shared/SeedData/PageTemplates";
    }
}
