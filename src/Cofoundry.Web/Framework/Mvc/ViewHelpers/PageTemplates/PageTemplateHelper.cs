using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// UI helper for Page Template functionality such as defining region.
    /// </summary>
    /// <typeparam name="TModel">ViewModel type</typeparam>
    public class PageTemplateHelper<TModel>
        : IPageTemplateHelper<TModel>, IViewContextAware
        where TModel : IEditablePageViewModel
    {
        public ViewContext ViewContext { get; private set; }

        public TModel Model { get; private set; }

        public void Contextualize(ViewContext viewContext)
        {
            ViewContext = viewContext;

            if (viewContext.ViewData.Model is TModel model)
            {
                if (!(model is IEditablePageViewModel))
                {
                    throw new ArgumentException("Page templates must use a model that inherits from " + typeof(IEditablePageViewModel).Name);
                }
                Model = model;
            }
            else
            {
                throw new Exception("Model is not correct");
            }
        }

        /// <summary>
        /// Indictes where to render a region in the page template. 
        /// </summary>
        /// <param name="regionName">The name of the page template region. This must be unique in a page template.</param>
        /// <returns>IPageTemplateRegionTagBuilder to allow for method chaining.</returns>
        public IPageTemplateRegionTagBuilder Region(string regionName)
        {
            var factory = ViewContext.HttpContext.RequestServices.GetRequiredService<IPageTemplateRegionTagBuilderFactory>();
            var output = factory.Create(ViewContext, (IEditablePageViewModel)Model, regionName);

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
            if (description == null) throw new ArgumentNullException(nameof(description));

            // nothing is rendered here, this is just used as a convention for adding template meta data
            return HtmlString.Empty;
        }
    }
}
