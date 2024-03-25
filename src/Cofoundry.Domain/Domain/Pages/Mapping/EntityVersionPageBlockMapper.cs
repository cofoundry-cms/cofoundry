using Cofoundry.Domain.Data;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IEntityVersionPageBlockMapper"/>.
/// </summary>
public class EntityVersionPageBlockMapper : IEntityVersionPageBlockMapper
{
    private readonly IPageVersionBlockModelMapper _pageVersionBlockModelMapper;
    private readonly ILogger<EntityVersionPageBlockMapper> _logger;

    public EntityVersionPageBlockMapper(
        IPageVersionBlockModelMapper pageVersionBlockModelMapper,
        ILogger<EntityVersionPageBlockMapper> logger
        )
    {
        _pageVersionBlockModelMapper = pageVersionBlockModelMapper;
        _logger = logger;
    }

    private class MappedPageBlock
    {
        public required IEntityVersionPageBlock PageBlock { get; set; }
        public required IPageBlockTypeDisplayModel DisplayModel { get; set; }
        public required PageBlockTypeSummary BlockType { get; set; }
    }

    /// <inheritdoc/>
    public async Task MapRegionsAsync<TBlockRenderDetails>(
        IEnumerable<IEntityVersionPageBlock> dbBlocks,
        IEnumerable<IEntityRegionRenderDetails<TBlockRenderDetails>> regions,
        IReadOnlyCollection<PageBlockTypeSummary> allBlockTypes,
        PublishStatusQuery publishStatus,
        IExecutionContext executionContext
        )
        where TBlockRenderDetails : IEntityVersionPageBlockRenderDetails, new()
    {
        var mappedBlocks = await ToBlockMappingDataAsync(dbBlocks, allBlockTypes, publishStatus, executionContext);

        // Map Regions

        foreach (var region in regions)
        {
            var regionMappedBlocks = mappedBlocks
                .Where(m => m.PageBlock.PageTemplateRegionId == region.PageTemplateRegionId)
                .OrderBy(m => m.PageBlock.Ordering);

            region.Blocks = ToBlockRenderDetails<TBlockRenderDetails>(regionMappedBlocks).ToArray();
        }
    }

    /// <inheritdoc/>
    public PageBlockTypeTemplateSummary? GetCustomTemplate(IEntityVersionPageBlock pageBlock, PageBlockTypeSummary blockType)
    {
        ArgumentNullException.ThrowIfNull(pageBlock);
        ArgumentNullException.ThrowIfNull(blockType);

        if (!pageBlock.PageBlockTypeTemplateId.HasValue)
        {
            return null;
        }

        var template = blockType
            .Templates
            .FirstOrDefault(t => t.PageBlockTypeTemplateId == pageBlock.PageBlockTypeTemplateId);

        if (template == null)
        {
            _logger.LogDebug("The block template with id {PageBlockTypeTemplateId} could not be found for {PageBlockType} {VersionBlockId}", pageBlock.PageBlockTypeTemplateId, pageBlock.GetType().Name, pageBlock.GetVersionBlockId());
        }

        return template;
    }

    private async Task<ICollection<MappedPageBlock>> ToBlockMappingDataAsync(
        IEnumerable<IEntityVersionPageBlock> entityBlocks,
        IEnumerable<PageBlockTypeSummary> blockTypes,
        PublishStatusQuery publishStatus,
        IExecutionContext executionContext
        )
    {
        var mappedBlocks = new List<MappedPageBlock>();

        foreach (var group in entityBlocks.GroupBy(m => m.PageBlockTypeId))
        {
            var blockType = blockTypes.SingleOrDefault(t => t.PageBlockTypeId == group.Key);

            // If missing e.g. archived, skip
            if (blockType == null)
            {
                continue;
            }

            var mapperOutput = await _pageVersionBlockModelMapper.MapDisplayModelAsync(
                blockType.FileName,
                group,
                publishStatus,
                executionContext
                );

            foreach (var block in group)
            {
                var versionBlockId = block.GetVersionBlockId();

                if (mapperOutput.ContainsKey(versionBlockId) && mapperOutput[versionBlockId] != null)
                {
                    var mappedBlock = new MappedPageBlock()
                    {
                        PageBlock = block,
                        BlockType = blockType,
                        DisplayModel = mapperOutput[versionBlockId]
                    };

                    mappedBlocks.Add(mappedBlock);
                }
            }
        }

        return mappedBlocks;
    }

    private IEnumerable<TBlockRenderDetails> ToBlockRenderDetails<TBlockRenderDetails>(IEnumerable<MappedPageBlock> dbBlocks)
        where TBlockRenderDetails : IEntityVersionPageBlockRenderDetails, new()
    {
        var index = 0;
        var size = dbBlocks.Count();

        foreach (var dbBlock in dbBlocks)
        {
            var block = new TBlockRenderDetails();

            block.EntityVersionPageBlockId = dbBlock.PageBlock.GetVersionBlockId();
            block.BlockType = dbBlock.BlockType;
            block.DisplayModel = dbBlock.DisplayModel;
            block.Template = GetCustomTemplate(dbBlock.PageBlock, dbBlock.BlockType);

            // Add any list context information.
            if (block.DisplayModel is IListablePageBlockTypeDisplayModel displayData)
            {
                displayData.ListContext = new ListablePageBlockRenderContext()
                {
                    Index = index,
                    NumBlocks = size
                };

                index++;
            }

            yield return block;
        }
    }
}
