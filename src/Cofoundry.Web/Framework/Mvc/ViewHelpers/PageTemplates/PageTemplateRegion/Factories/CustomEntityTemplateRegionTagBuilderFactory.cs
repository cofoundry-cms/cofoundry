using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="ICustomEntityTemplateRegionTagBuilderFactory"/>.
/// </summary>
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
