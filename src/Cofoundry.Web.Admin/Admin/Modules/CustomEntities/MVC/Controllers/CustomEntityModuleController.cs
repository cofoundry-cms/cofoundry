using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    public class CustomEntityModuleController : BaseAdminMvcController
    {
        private static readonly Dictionary<string, string> EmptyTerms = new Dictionary<string, string>();

        public ActionResult Index()
        {
            var definition = RouteData.DataTokens["Definition"] as ICustomEntityDefinition;

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

            if (definition is IOrderableCustomEntityDefinition)
            {
                options.Ordering = ((IOrderableCustomEntityDefinition)definition).Ordering;
            }
            
            var viewPath = ViewPathFormatter.View("CustomEntities", nameof(Index));
            return View(viewPath, options);
        }
    }
}