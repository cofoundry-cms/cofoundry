using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public interface IPageTemplateHelper<out TModel>
        where TModel : IEditablePageViewModel
    {
        ViewContext ViewContext { get; }

        TModel Model { get; }

        /// <summary>
        /// Indictes where to render a region in the page template. 
        /// </summary>
        /// <param name="regionName">The name of the page template region. This must be unique in a page template.</param>
        /// <returns>IPageTemplateRegionTagBuilder to allow for method chaining.</returns>
        IPageTemplateRegionTagBuilder Region(string regionName);

        /// <summary>
        /// Sets the description assigned to the template in the
        /// administration UI. Use this to tell users what the template 
        /// should be used for.
        /// </summary>
        /// <param name="description">A plain text description about this template</param>
        IHtmlContent UseDescription(string description);
    }
}
