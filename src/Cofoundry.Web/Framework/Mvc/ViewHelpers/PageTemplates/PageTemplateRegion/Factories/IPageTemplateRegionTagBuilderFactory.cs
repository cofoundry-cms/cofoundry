using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Factory that enables IPageTemplateRegionTagBuilder implementation to be swapped out.
    /// </summary>
    /// <remarks>
    /// The factory is required because the HtmlHelper cannot be injected
    /// </remarks>
    public interface IPageTemplateRegionTagBuilderFactory
    {
        IPageTemplateRegionTagBuilder Create(
            ViewContext viewContext,
            IEditablePageViewModel pageViewModel,
            string regionName
            );
    }
}
