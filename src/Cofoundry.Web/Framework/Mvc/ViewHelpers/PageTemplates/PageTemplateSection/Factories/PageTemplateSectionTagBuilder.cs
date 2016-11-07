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
    public class PageTemplateSectionTagBuilderFactory : IPageTemplateSectionTagBuilderFactory
    {
        private readonly IModuleRenderer _moduleRenderer;

        public PageTemplateSectionTagBuilderFactory(
            IModuleRenderer moduleRenderer)
        {
            _moduleRenderer = moduleRenderer;
        }

        public IPageTemplateSectionTagBuilder Create(
            HtmlHelper htmlHelper,
            IEditablePageViewModel pageViewModel, 
            string sectionName
            )
        {
            return new PageTemplateSectionTagBuilder(_moduleRenderer, htmlHelper, pageViewModel, sectionName);
        }
    }
}
