using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;

namespace Cofoundry.Web;

/// <inheritdoc/>
public class CustomEntityTemplateRegionTagBuilder<TModel> : ICustomEntityTemplateRegionTagBuilder<TModel>
    where TModel : ICustomEntityPageDisplayModel
{
    const string DEFAULT_TAG = "div";
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
        if (regionName == null) throw new ArgumentNullException(nameof(regionName));
        if (string.IsNullOrWhiteSpace(regionName)) throw new ArgumentEmptyException(nameof(regionName));
        if (customEntityViewModel == null) throw new ArgumentNullException(nameof(customEntityViewModel));

        _blockRenderer = blockRenderer;
        _pageBlockDataModelTypeFactory = pageBlockDataModelTypeFactory;
        _blockTypeFileNameFormatter = pageBlockTypeFileNameFormatter;
        _visualEditorStateService = visualEditorStateService;
        _logger = logger;
        _regionName = regionName;
        _customEntityViewModel = customEntityViewModel;
        _viewContext = viewContext;
    }

    private string _output = null;
    private string _regionName;
    private string _wrappingTagName = null;
    private bool _allowMultipleBlocks = false;
    private int? _emptyContentMinHeight = null;
    private Dictionary<string, string> _additonalHtmlAttributes = null;
    private readonly HashSet<string> _permittedBlocks = new HashSet<string>();

    public ICustomEntityTemplateRegionTagBuilder<TModel> AllowMultipleBlocks()
    {
        _allowMultipleBlocks = true;
        return this;
    }

    public ICustomEntityTemplateRegionTagBuilder<TModel> WrapWithTag(string tagName, object htmlAttributes = null)
    {
        if (tagName == null) throw new ArgumentNullException(nameof(tagName));
        if (string.IsNullOrWhiteSpace(tagName)) throw new ArgumentEmptyException(nameof(tagName));

        _wrappingTagName = tagName;
        _additonalHtmlAttributes = TemplateRegionTagBuilderHelper.ParseHtmlAttributesFromAnonymousObject(htmlAttributes);

        return this;
    }

    public ICustomEntityTemplateRegionTagBuilder<TModel> EmptyContentMinHeight(int minHeight)
    {
        _emptyContentMinHeight = minHeight;
        return this;
    }

    public ICustomEntityTemplateRegionTagBuilder<TModel> AllowBlockType<TBlockType>() where TBlockType : IPageBlockTypeDataModel
    {
        AddBlockToAllowedTypes(typeof(TBlockType).Name);
        return this;
    }

    public ICustomEntityTemplateRegionTagBuilder<TModel> AllowBlockType(string blockTypeName)
    {
        AddBlockToAllowedTypes(blockTypeName);
        return this;
    }

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

        regionAttributes.Add("data-cms-page-template-region-id", pageRegion.PageTemplateRegionId.ToString());
        regionAttributes.Add("data-cms-page-region-name", pageRegion.Name);
        regionAttributes.Add("data-cms-custom-entity-region", string.Empty);
        regionAttributes.Add("class", "cofoundry__sv-region");

        if (_permittedBlocks.Any())
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

        string blocksHtml = string.Empty;

        if (blockHtmlParts.Any())
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