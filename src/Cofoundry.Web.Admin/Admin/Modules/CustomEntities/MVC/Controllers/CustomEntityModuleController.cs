using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin;

public class CustomEntityModuleController : BaseAdminMvcController
{
    private static readonly Dictionary<string, string> EmptyTerms = [];

    public ActionResult Index()
    {
        var definition = RouteData.DataTokens["Definition"] as ICustomEntityDefinition;
        if (definition == null)
        {
            throw new Exception($"RouteData.DataTokens[\"Definition\"] is null or not castable to ICustomEntityDefinition.");
        }

        var options = new CustomEntityModuleOptions()
        {
            CustomEntityDefinitionCode = definition.CustomEntityDefinitionCode,
            ForceUrlSlugUniqueness = definition.ForceUrlSlugUniqueness,
            HasLocale = definition.HasLocale,
            AutoGenerateUrlSlug = definition.AutoGenerateUrlSlug,
            AutoPublish = definition.AutoPublish,
            Name = definition.NamePlural,
            NameSingular = definition.Name,
            Terms = definition.GetTerms()
        };

        if (definition is IOrderableCustomEntityDefinition orderableCustomEntityDefinition)
        {
            options.Ordering = orderableCustomEntityDefinition.Ordering;
        }

        var viewPath = ViewPathFormatter.View("CustomEntities", nameof(Index));
        return View(viewPath, options);
    }
}
