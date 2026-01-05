using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Web;

/// <summary>
/// Extension methods for <see cref="IPageTemplateHelper{T}"/>.
/// </summary>
public static class PageTemplateHelperExtensions
{
    extension<TModel>(IPageTemplateHelper<ICustomEntityPageViewModel<TModel>> helper) where TModel : ICustomEntityPageDisplayModel
    {
        /// <summary>
        /// Indictes where to render a region in a custom entity page template. 
        /// </summary>
        /// <param name="regionName">The name of the region. This must be unique in a page template.</param>
        /// <returns>ICustomEntityTemplateRegionTagBuilder to allow for method chaining.</returns>
        public ICustomEntityTemplateRegionTagBuilder<TModel> CustomEntityRegion(string regionName)
        {
            var factory = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<ICustomEntityTemplateRegionTagBuilderFactory>();
            var output = factory.Create(helper.ViewContext, helper.Model, regionName);

            return output;
        }
    }
}
