using Cofoundry.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public static class PageTemplateHelperExtensions
    {
        /// <summary>
        /// Indictes where to render a region in a custom entity page template. 
        /// </summary>
        /// <typeparam name="TModel">Custom entity display model type.</typeparam>
        /// <param name="helper">IPageTemplateHelper to extend.</param>
        /// <param name="regionName">The name of the region. This must be unique in a page template.</param>
        /// <returns>ICustomEntityTemplateRegionTagBuilder to allow for method chaining.</returns>
        public static ICustomEntityTemplateRegionTagBuilder<TModel> CustomEntityRegion<TModel>(
            this IPageTemplateHelper<ICustomEntityPageViewModel<TModel>> helper, string regionName)
            where TModel : ICustomEntityPageDisplayModel
        {
            var factory = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<ICustomEntityTemplateRegionTagBuilderFactory>();
            var output = factory.Create(helper.ViewContext, helper.Model, regionName);

            return output;
        }
    }
}
