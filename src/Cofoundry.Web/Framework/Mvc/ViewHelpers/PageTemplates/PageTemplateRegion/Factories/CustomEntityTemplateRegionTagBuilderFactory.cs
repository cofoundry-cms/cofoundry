using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public CustomEntityTemplateRegionTagBuilderFactory(
            IPageBlockRenderer blockRenderer,
            IPageBlockTypeDataModelTypeFactory pageBlockTypeDataModelTypeFactory,
            IPageBlockTypeFileNameFormatter pageBlockTypeFileNameFormatter
            )
        {
            _blockRenderer = blockRenderer;
            _pageBlockTypeDataModelTypeFactory = pageBlockTypeDataModelTypeFactory;
            _pageBlockTypeFileNameFormatter = pageBlockTypeFileNameFormatter;
        }

        public ICustomEntityTemplateRegionTagBuilder<TModel> Create<TModel>(
            ViewContext viewContext,
            ICustomEntityPageViewModel<TModel> customEntityViewModel, 
            string regionName
            )
            where TModel : ICustomEntityPageDisplayModel
        {
            return new CustomEntityTemplateRegionTagBuilder<TModel>(_blockRenderer, _pageBlockTypeDataModelTypeFactory, _pageBlockTypeFileNameFormatter, viewContext, customEntityViewModel, regionName);
        }
    }
}
