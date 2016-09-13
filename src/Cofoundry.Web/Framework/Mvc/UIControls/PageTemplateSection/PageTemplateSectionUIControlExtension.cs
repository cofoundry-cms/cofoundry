using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public static class PageTemplateSectionUIControlExtension
    {
        /// <summary>
        /// Indictes where to render a template section in the page template. 
        /// </summary>
        /// <param name="sectionName">The name of the page template section. This must be unique in a page template.</param>
        /// <returns>PageTemplateSectionTagOutput to allow for method chaining.</returns>
        public static IPageTemplateSectionTagBuilder PageTemplateSection<TModel>(this IUIViewHelper<TModel> helper, string sectionName)
            where TModel : IEditablePageViewModel
        {
            var factory = IckyDependencyResolution.ResolveFromMvcContext<IPageTemplateSectionTagBuilderFactory>();
            var output = factory.Create(helper.HtmlHelper, helper.Model, sectionName);

            return output;
        }

        public static ICustomEntityTemplateSectionTagBuilder<TModel> CustomEntitySection<TModel>(this IUIViewHelper<CustomEntityDetailsPageViewModel<TModel>> helper, string sectionName)
            where TModel : ICustomEntityDetailsDisplayViewModel
        {
            var factory = IckyDependencyResolution.ResolveFromMvcContext<ICustomEntityTemplateSectionTagBuilderFactory>();
            var output = factory.Create(helper.HtmlHelper, helper.Model, sectionName);

            return output;
        }
        
    }
}
