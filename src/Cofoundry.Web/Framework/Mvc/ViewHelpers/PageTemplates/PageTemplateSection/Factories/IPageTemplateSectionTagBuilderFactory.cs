using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Factory that enables IPageTemplateSectionTagBuilder implementation to be swapped out.
    /// </summary>
    /// <remarks>
    /// The factory is required because the HtmlHelper cannot be injected
    /// </remarks>
    public interface IPageTemplateSectionTagBuilderFactory
    {
        IPageTemplateSectionTagBuilder Create(
            ViewContext viewContext,
            IEditablePageViewModel pageViewModel,
            string sectionName
            );
    }
}
