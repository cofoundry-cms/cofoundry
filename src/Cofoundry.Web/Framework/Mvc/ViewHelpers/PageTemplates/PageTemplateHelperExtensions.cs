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
        /// Indictes where to render a section in a custom entity details page template. 
        /// </summary>
        /// <typeparam name="TModel">Custom entity display model type.</typeparam>
        /// <param name="helper">IPageTemplateHelper to extend.</param>
        /// <param name="sectionName">The name of the section. This must be unique in a page template.</param>
        /// <returns>ICustomEntityTemplateSectionTagBuilder to allow for method chaining.</returns>
        public static ICustomEntityTemplateSectionTagBuilder<TModel> CustomEntitySection<TModel>(
            this IPageTemplateHelper<ICustomEntityDetailsPageViewModel<TModel>> helper, string sectionName)
            where TModel : ICustomEntityDetailsDisplayViewModel
        {
            var factory = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<ICustomEntityTemplateSectionTagBuilderFactory>();
            var output = factory.Create(helper.ViewContext, helper.Model, sectionName);

            return output;
        }
    }
}
