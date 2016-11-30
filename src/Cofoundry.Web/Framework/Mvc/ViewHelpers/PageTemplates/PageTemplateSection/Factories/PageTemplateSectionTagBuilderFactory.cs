using Cofoundry.Domain;
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
        private readonly IPageModuleDataModelTypeFactory _moduleDataModelTypeFactory;
        private readonly IPageModuleTypeFileNameFormatter _moduleTypeFileNameFormatter;

        public PageTemplateSectionTagBuilderFactory(
            IModuleRenderer moduleRenderer,
            IPageModuleDataModelTypeFactory moduleDataModelTypeFactory,
            IPageModuleTypeFileNameFormatter moduleTypeFileNameFormatter
            )
        {
            _moduleRenderer = moduleRenderer;
            _moduleDataModelTypeFactory = moduleDataModelTypeFactory;
            _moduleTypeFileNameFormatter = moduleTypeFileNameFormatter;
        }

        public IPageTemplateSectionTagBuilder Create(
            HtmlHelper htmlHelper,
            IEditablePageViewModel pageViewModel, 
            string sectionName
            )
        {
            return new PageTemplateSectionTagBuilder(
                _moduleRenderer, 
                _moduleDataModelTypeFactory,
                _moduleTypeFileNameFormatter,
                htmlHelper,
                pageViewModel,
                sectionName
                );
        }
    }
}
