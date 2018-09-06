using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
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
        private readonly IVisualEditorStateService _visualEditorStateService;
        private readonly ILoggerFactory _loggerFactory;

        public PageTemplateRegionTagBuilderFactory(
            IPageBlockRenderer pageBlockRenderer,
            IPageBlockTypeDataModelTypeFactory pageBlockTypeDataModelTypeFactory,
            IPageBlockTypeFileNameFormatter pageBlockTypeFileNameFormatter,
            IVisualEditorStateService visualEditorStateService,
            ILoggerFactory loggerFactory
            )
        {
            _pageBlockRenderer = pageBlockRenderer;
            _pageBlockTypeDataModelTypeFactory = pageBlockTypeDataModelTypeFactory;
            _pageBlockTypeFileNameFormatter = pageBlockTypeFileNameFormatter;
            _visualEditorStateService = visualEditorStateService;
            _loggerFactory = loggerFactory;
        }

        public IPageTemplateRegionTagBuilder Create(
            ViewContext viewContext,
            IEditablePageViewModel pageViewModel, 
            string regionName
            )
        {
            var logger = _loggerFactory.CreateLogger<PageTemplateRegionTagBuilder>();

            return new PageTemplateRegionTagBuilder(
                _pageBlockRenderer, 
                _pageBlockTypeDataModelTypeFactory,
                _pageBlockTypeFileNameFormatter,
                _visualEditorStateService,
                logger,
                viewContext,
                pageViewModel,
                regionName
                );
        }
    }
}
