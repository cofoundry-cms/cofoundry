using Cofoundry.Domain.Data;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Returns data for a specific block in a page version by it's id. Because
/// the mapped display model may contain other versioned entities, you can 
/// optionally pass down a PublishStatusQuery to use in the mapping process.
/// </summary>
public class GetPageVersionBlockRenderDetailsByIdQueryHandler
    : IQueryHandler<GetPageVersionBlockRenderDetailsByIdQuery, PageVersionBlockRenderDetails?>
    , IPermissionRestrictedQueryHandler<GetPageVersionBlockRenderDetailsByIdQuery, PageVersionBlockRenderDetails?>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IQueryExecutor _queryExecutor;
    private readonly IPageVersionBlockModelMapper _pageVersionBlockModelMapper;
    private readonly ILogger<GetPageVersionBlockRenderDetailsByIdQueryHandler> _logger;

    public GetPageVersionBlockRenderDetailsByIdQueryHandler(
        CofoundryDbContext dbContext,
        IQueryExecutor queryExecutor,
        IPageVersionBlockModelMapper pageVersionBlockModelMapper,
        ILogger<GetPageVersionBlockRenderDetailsByIdQueryHandler> logger
        )
    {
        _dbContext = dbContext;
        _queryExecutor = queryExecutor;
        _pageVersionBlockModelMapper = pageVersionBlockModelMapper;
        _logger = logger;
    }

    public async Task<PageVersionBlockRenderDetails?> ExecuteAsync(GetPageVersionBlockRenderDetailsByIdQuery query, IExecutionContext executionContext)
    {
        var dbResult = await QueryBlock(query.PageVersionBlockId)
            .Select(b => new
            {
                PageBlock = b,
                BlockTypeFileName = b.PageBlockType.FileName
            })
            .SingleOrDefaultAsync();

        if (dbResult == null)
        {
            return null;
        }

        var result = await MapAsync(
            dbResult.PageBlock,
            dbResult.BlockTypeFileName,
            query.PublishStatus,
            executionContext
            );

        if (result == null)
        {
            return null;
        }

        // Add any list context information.
        var displayData = result.DisplayModel as IListablePageBlockTypeDisplayModel;

        if (displayData != null)
        {
            var blocks = await GetOrderedBlockIds(dbResult.PageBlock).ToListAsync();

            displayData.ListContext = new ListablePageBlockRenderContext()
            {
                Index = blocks.IndexOf(result.PageVersionBlockId),
                NumBlocks = blocks.Count
            };
        }

        return result;
    }

    private IQueryable<int> GetOrderedBlockIds(PageVersionBlock pageVersionBlock)
    {
        return _dbContext
            .PageVersionBlocks
            .AsNoTracking()
            .FilterActive()
            .Where(m => m.PageVersionId == pageVersionBlock.PageVersionId && m.PageTemplateRegionId == pageVersionBlock.PageTemplateRegionId)
            .OrderBy(m => m.Ordering)
            .Select(m => m.PageVersionBlockId);
    }

    private async Task<PageVersionBlockRenderDetails?> MapAsync(
        PageVersionBlock pageVersionBlock,
        string blockTypeFileName,
        PublishStatusQuery publishStatus,
        IExecutionContext executionContext
        )
    {
        var blockTypeQuery = new GetPageBlockTypeSummaryByIdQuery(pageVersionBlock.PageBlockTypeId);
        var blockType = await _queryExecutor.ExecuteAsync(blockTypeQuery, executionContext);
        EntityNotFoundException.ThrowIfNull(blockType, pageVersionBlock.PageBlockTypeId);

        var displayModel = await _pageVersionBlockModelMapper.MapDisplayModelAsync(
            blockTypeFileName,
            pageVersionBlock,
            publishStatus,
            executionContext
            );

        if (displayModel == null)
        {
            return null;
        }

        var result = new PageVersionBlockRenderDetails
        {
            PageVersionBlockId = pageVersionBlock.PageVersionBlockId,
            BlockType = blockType,
            DisplayModel = displayModel
        };

        result.Template = GetCustomTemplate(pageVersionBlock, blockType);

        return result;
    }

    private IQueryable<PageVersionBlock> QueryBlock(int pageVersionBlockId)
    {
        return _dbContext
            .PageVersionBlocks
            .AsNoTracking()
            .FilterActive()
            .Where(m => m.PageVersionBlockId == pageVersionBlockId);
    }

    private PageBlockTypeTemplateSummary? GetCustomTemplate(PageVersionBlock versionBlock, PageBlockTypeSummary blockType)
    {
        if (!versionBlock.PageBlockTypeTemplateId.HasValue)
        {
            return null;
        }

        var template = blockType
            .Templates
            .FirstOrDefault(t => t.PageBlockTypeTemplateId == versionBlock.PageBlockTypeTemplateId);

        if (template == null)
        {
            _logger.LogWarning("The page block type template with id {PageBlockTypeTemplateId} could not be found for page version block {PageVersionBlockId}", versionBlock.PageBlockTypeTemplateId, versionBlock.PageVersionBlockId);
        }

        return template;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageVersionBlockRenderDetailsByIdQuery query)
    {
        yield return new PageReadPermission();
    }
}