using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    public static class PageTemplateHelperExtensions
    {
        /// <summary>
        /// Indictes where to render a section in a custom entity details page template. 
        /// </summary>
        /// <typeparam name="TModel">View model of the page</typeparam>
        /// <param name="helper">IPageTemplateHelper to extend</param>
        /// <param name="sectionName">The name of the section. This must be unique in a page template.</param>
        /// <returns>ICustomEntityTemplateSectionTagBuilder to allow for method chaining.</returns>
        public static ICustomEntityTemplateSectionTagBuilder<TModel> CustomEntitySection<TModel>(
            this IPageTemplateHelper<CustomEntityDetailsPageViewModel<TModel>> helper, string sectionName)
            where TModel : ICustomEntityDetailsDisplayViewModel
        {
            var factory = IckyDependencyResolution.ResolveFromMvcContext<ICustomEntityTemplateSectionTagBuilderFactory>();
            var output = factory.Create(helper.HtmlHelper, helper.Model, sectionName);

            return output;
        }
    }
}
