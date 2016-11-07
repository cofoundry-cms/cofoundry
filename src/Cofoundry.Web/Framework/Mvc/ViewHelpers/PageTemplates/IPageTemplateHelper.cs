using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    public interface IPageTemplateHelper<TModel>
        where TModel : IEditablePageViewModel
    {
        HtmlHelper HtmlHelper { get; }
        TModel Model { get; }

        /// <summary>
        /// Indictes where to render a template section in the page template. 
        /// </summary>
        /// <param name="sectionName">The name of the page template section. This must be unique in a page template.</param>
        /// <returns>PageTemplateSectionTagOutput to allow for method chaining.</returns>
        IPageTemplateSectionTagBuilder Section(string sectionName);

        /// <summary>
        /// Sets the description assigned to the template in the
        /// administration UI. Use this to tell users what the template 
        /// should be used for.
        /// </summary>
        /// <param name="description">A plain text description about this template</param>
        IHtmlString UseTemplateDescription(string description);
    }
}
