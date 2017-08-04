using Cofoundry.Domain;
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
    public class PageTemplateRegionTagBuilderFactory : IPageTemplateRegionTagBuilderFactory
    {
        private readonly IPageBlockRenderer _pageBlockRenderer;
        private readonly IPageBlockTypeDataModelTypeFactory _pageBlockTypeDataModelTypeFactory;
        private readonly IPageBlockTypeFileNameFormatter _pageBlockTypeFileNameFormatter;

        public PageTemplateRegionTagBuilderFactory(
            IPageBlockRenderer pageBlockRenderer,
            IPageBlockTypeDataModelTypeFactory pageBlockTypeDataModelTypeFactory,
            IPageBlockTypeFileNameFormatter pageBlockTypeFileNameFormatter
            )
        {
            _pageBlockRenderer = pageBlockRenderer;
            _pageBlockTypeDataModelTypeFactory = pageBlockTypeDataModelTypeFactory;
            _pageBlockTypeFileNameFormatter = pageBlockTypeFileNameFormatter;
        }

        public IPageTemplateRegionTagBuilder Create(
            ViewContext viewContext,
            IEditablePageViewModel pageViewModel, 
            string regionName
            )
        {
            return new PageTemplateRegionTagBuilder(
                _pageBlockRenderer, 
                _pageBlockTypeDataModelTypeFactory,
                _pageBlockTypeFileNameFormatter,
                viewContext,
                pageViewModel,
                regionName
                );
        }
    }
}
