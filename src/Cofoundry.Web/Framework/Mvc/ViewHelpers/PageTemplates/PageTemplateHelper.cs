using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// UI helper for Page Template functionality such as defining sections.
    /// </summary>
    /// <typeparam name="TModel">ViewModel type</typeparam>
    public class PageTemplateHelper<TModel> : IPageTemplateHelper<TModel>
            where TModel : IEditablePageViewModel
    {
        public PageTemplateHelper(
            ViewContext viewContext,
            TModel model
            )
        {
            ViewContext = viewContext;
            Model = model;
        }

        public ViewContext ViewContext { get; private set; }
        public TModel Model { get; private set; }

        /// <summary>
        /// Indictes where to render a template section in the page template. 
        /// </summary>
        /// <param name="sectionName">The name of the page template section. This must be unique in a page template.</param>
        /// <returns>PageTemplateSectionTagOutput to allow for method chaining.</returns>
        public IPageTemplateSectionTagBuilder Section(string sectionName)
        {
            var factory = IckyDependencyResolution.ResolveFromMvcContext<IPageTemplateSectionTagBuilderFactory>();
            var output = factory.Create(HtmlHelper, Model, sectionName);

            return output;
        }

        /// <summary>
        /// Sets the description assigned to the template in the
        /// administration UI. Use this to tell users what the template 
        /// should be used for.
        /// </summary>
        /// <param name="description">A plain text description about this template</param>
        public IHtmlContent UseDescription(string description)
        {
            // nothing is rendered here, this is just used as a convention for adding template meta data
            return HtmlString.Empty;
        }
    }
}
