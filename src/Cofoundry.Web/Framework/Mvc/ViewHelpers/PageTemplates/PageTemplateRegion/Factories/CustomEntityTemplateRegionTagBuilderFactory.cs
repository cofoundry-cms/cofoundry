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
    /// Factory that enables ICustomEntityTemplateRegionTagBuilder implementation to be swapped out.
    /// </summary>
    /// <remarks>
    /// The factory is required because the HtmlHelper cannot be injected
    /// </remarks>
    public class CustomEntityTemplateRegionTagBuilderFactory : ICustomEntityTemplateRegionTagBuilderFactory
    {
        private readonly IPageBlockRenderer _blockRenderer;
        private readonly IPageBlockTypeDataModelTypeFactory _pageBlockTypeDataModelTypeFactory;
        private readonly IPageBlockTypeFileNameFormatter _pageBlockTypeFileNameFormatter;
        private readonly IVisualEditorStateService _visualEditorStateService;
        private readonly ILoggerFactory _loggerFactory;

        public CustomEntityTemplateRegionTagBuilderFactory(
            IPageBlockRenderer blockRenderer,
            IPageBlockTypeDataModelTypeFactory pageBlockTypeDataModelTypeFactory,
            IPageBlockTypeFileNameFormatter pageBlockTypeFileNameFormatter,
            IVisualEditorStateService visualEditorStateService,
            ILoggerFactory loggerFactory
            )
        {
            _blockRenderer = blockRenderer;
            _pageBlockTypeDataModelTypeFactory = pageBlockTypeDataModelTypeFactory;
            _pageBlockTypeFileNameFormatter = pageBlockTypeFileNameFormatter;
            _visualEditorStateService = visualEditorStateService;
            _loggerFactory = loggerFactory;
        }

        public ICustomEntityTemplateRegionTagBuilder<TModel> Create<TModel>(
            ViewContext viewContext,
            ICustomEntityPageViewModel<TModel> customEntityViewModel, 
            string regionName
            )
            where TModel : ICustomEntityPageDisplayModel
        {
            var logger = _loggerFactory.CreateLogger<CustomEntityTemplateRegionTagBuilder<TModel>>();

            return new CustomEntityTemplateRegionTagBuilder<TModel>(
                _blockRenderer, 
                _pageBlockTypeDataModelTypeFactory, 
                _pageBlockTypeFileNameFormatter, 
                _visualEditorStateService,
                logger,
                viewContext, 
                customEntityViewModel, 
                regionName);
        }
    }
}
