using System.Globalization;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Web;

public class CustomEntityTemplateRegionTagBuilder<TModel> : ICustomEntityTemplateRegionTagBuilder<TModel>
    where TModel : ICustomEntityPageDisplayModel
{
    private readonly ICustomEntityPageViewModel<TModel> _customEntityViewModel;
    private readonly ViewContext _viewContext;
    private readonly IPageBlockRenderer _blockRenderer;
    private readonly IPageBlockTypeDataModelTypeFactory _pageBlockDataModelTypeFactory;
    private readonly IPageBlockTypeFileNameFormatter _blockTypeFileNameFormatter;
    private readonly IVisualEditorStateService _visualEditorStateService;
    private readonly ILogger<CustomEntityTemplateRegionTagBuilder<TModel>> _logger;

    public CustomEntityTemplateRegionTagBuilder(
        IPageBlockRenderer blockRenderer,
        IPageBlockTypeDataModelTypeFactory pageBlockDataModelTypeFactory,
        IPageBlockTypeFileNameFormatter pageBlockTypeFileNameFormatter,
        IVisualEditorStateService visualEditorStateService,
        ILogger<CustomEntityTemplateRegionTagBuilder<TModel>> logger,
        ViewContext viewContext,
        ICustomEntityPageViewModel<TModel> customEntityViewModel,
        string regionName
        )
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(regionName);
        ArgumentNullException.ThrowIfNull(customEntityViewModel);

        _blockRenderer = blockRenderer;
        _pageBlockDataModelTypeFactory = pageBlockDataModelTypeFactory;
        _blockTypeFileNameFormatter = pageBlockTypeFileNameFormatter;
        _visualEditorStateService = visualEditorStateService;
        _logger = logger;
        _regionName = regionName;
        _customEntityViewModel = customEntityViewModel;
        _viewContext = viewContext;
    }

    private readonly string _regionName;
    private string? _output;
    private string? _wrappingTagName;
    private bool _allowMultipleBlocks;
    private int? _emptyContentMinHeight;
    private Dictionary<string, string>? _additonalHtmlAttributes;
    private readonly HashSet<string> _permittedBlocks = [];

    /// <inheritdoc/>
    public ICustomEntityTemplateRegionTagBuilder<TModel> AllowMultipleBlocks()
    {
        _allowMultipleBlocks = true;
        return this;
    }

    /// <inheritdoc/>
    public ICustomEntityTemplateRegionTagBuilder<TModel> WrapWithTag(string tagName, object? htmlAttributes = null)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(tagName);

        _wrappingTagName = tagName;
        _additonalHtmlAttributes = TemplateRegionTagBuilderHelper.ParseHtmlAttributesFromAnonymousObject(htmlAttributes);

        return this;
    }

    /// <inheritdoc/>
    public ICustomEntityTemplateRegionTagBuilder<TModel> EmptyContentMinHeight(int minHeight)
    {
        _emptyContentMinHeight = minHeight;
        return this;
    }

    /// <inheritdoc/>
    public ICustomEntityTemplateRegionTagBuilder<TModel> AllowBlockType<TBlockType>()
        where TBlockType : IPageBlockTypeDataModel
    {
        AddBlockToAllowedTypes(typeof(TBlockType).Name);
        return this;
    }

    /// <inheritdoc/>
    public ICustomEntityTemplateRegionTagBuilder<TModel> AllowBlockType(string blockTypeName)
    {
        AddBlockToAllowedTypes(blockTypeName);
        return this;
    }

    /// <inheritdoc/>
    public ICustomEntityTemplateRegionTagBuilder<TModel> AllowBlockTypes(params string[] blockTypeNames)
    {
        foreach (var blockTypeName in blockTypeNames)
        {
            AddBlockToAllowedTypes(blockTypeName);
        }
        return this;
    }

    private void AddBlockToAllowedTypes(string blockTypeName)
    {
        // Get the file name if we haven't already got it, e.g. we could have the file name or 
        // full type name passed into here (we shouldn't be fussy)
        var fileName = _blockTypeFileNameFormatter.FormatFromDataModelName(blockTypeName);

        // Validate the model type, will throw exception if not implemented
        var blockType = _pageBlockDataModelTypeFactory.CreateByPageBlockTypeFileName(fileName);

        // Make sure we have the correct name casing
        var formattedBlockTypeName = _blockTypeFileNameFormatter.FormatFromDataModelType(blockType);

        _permittedBlocks.Add(formattedBlockTypeName);
    }

    /// <inheritdoc/>
    public async Task<ICustomEntityTemplateRegionTagBuilder<TModel>> InvokeAsync()
    {
        var region = _customEntityViewModel
            .CustomEntity
            .Regions
            .SingleOrDefault(s => s.Name == _regionName);

        if (region != null)
        {
            _output = await RenderRegion(region);
        }
        else
        {
            _logger.LogDebug("The custom entity region '{RegionName}' cannot be found in the database", _regionName);

            if (_customEntityViewModel.CustomEntity.WorkFlowStatus != WorkFlowStatus.Published)
            {
                _output = "<!-- The custom entity region " + _regionName + " cannot be found in the database -->";
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public void WriteTo(TextWriter writer, HtmlEncoder encoder)
    {
        if (_output == null)
        {
            _logger.LogWarning("Template region '{RegionName}' definition does not call " + nameof(InvokeAsync) + "().", _regionName);
        }
        else
        {
            writer.Write(_output);
        }
    }

    private async Task<string> RenderRegion(CustomEntityPageRegionRenderDetails pageRegion)
    {
        var regionAttributes = new Dictionary<string, string>();
        var visualEditorState = await _visualEditorStateService.GetCurrentAsync();
        var blocksHtml = await RenderBlocksToHtml(pageRegion, regionAttributes, visualEditorState);

        // If we're not in edit mode just return the blocks.
        if (visualEditorState.VisualEditorMode != VisualEditorMode.Edit)
        {
            if (_wrappingTagName != null)
            {
                return TemplateRegionTagBuilderHelper.WrapInTag(
                    blocksHtml,
                    _wrappingTagName,
                    _allowMultipleBlocks,
                    _additonalHtmlAttributes
                    );
            }

            return blocksHtml;
        }

        regionAttributes.Add("data-cms-page-template-region-id", pageRegion.PageTemplateRegionId.ToString(CultureInfo.InvariantCulture));
        regionAttributes.Add("data-cms-page-region-name", pageRegion.Name);
        regionAttributes.Add("data-cms-custom-entity-region", string.Empty);
        regionAttributes.Add("class", "cofoundry__sv-region");

        if (_permittedBlocks.Count != 0)
        {
            regionAttributes.Add("data-cms-page-region-permitted-block-types", string.Join(",", _permittedBlocks));
        }

        if (_allowMultipleBlocks)
        {
            regionAttributes.Add("data-cms-multi-block", "true");

            if (_emptyContentMinHeight.HasValue)
            {
                regionAttributes.Add("style", "min-height:" + _emptyContentMinHeight + "px");
            }
        }

        return TemplateRegionTagBuilderHelper.WrapInTag(
            blocksHtml,
            _wrappingTagName,
            _allowMultipleBlocks,
            _additonalHtmlAttributes,
            regionAttributes
            );
    }

    private async Task<string> RenderBlocksToHtml(
        CustomEntityPageRegionRenderDetails pageRegion,
        Dictionary<string, string> regionAttributes,
        VisualEditorState visualEditorState
        )
    {
        // No _permittedBlocks means any is allowed.
        var blocksToRender = pageRegion
            .Blocks
            .Where(m => _permittedBlocks.Count == 0 || _permittedBlocks.Contains(m.BlockType.FileName));

        var blockHtmlParts = new List<string>();

        foreach (var block in blocksToRender)
        {
            var renderedBlock = await _blockRenderer.RenderBlockAsync(_viewContext, _customEntityViewModel, block);
            blockHtmlParts.Add(renderedBlock);
        }

        var blocksHtml = string.Empty;

        if (blockHtmlParts.Count != 0)
        {
            if (!_allowMultipleBlocks)
            {
                // If for some reason another block has been added in error, make sure we only display one.
                blocksHtml = blockHtmlParts.Last();
            }
            else
            {
                blocksHtml = string.Join(string.Empty, blockHtmlParts);
            }
        }
        else
        {
            regionAttributes.Add("data-cms-page-region-empty", string.Empty);

            if (!_allowMultipleBlocks && visualEditorState.VisualEditorMode == VisualEditorMode.Edit)
            {
                // If there are no blocks and this is a single block region
                // add a placeholder element so we always have a menu
                blocksHtml = _blockRenderer.RenderPlaceholderBlock(_emptyContentMinHeight);
            }
        }

        return blocksHtml;
    }
}
