using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

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
            HtmlHelper htmlHelper,
            IEditablePageViewModel pageViewModel,
            string sectionName
            );
    }
}
