using Cofoundry.Domain.Data;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Returns data for a specific custom entity page block by it's id. Because
/// the mapped display model may contain other versioned entities, you can 
/// optionally pass down a PublishStatusQuery to use in the mapping process.
/// </summary>
public class GetCustomEntityVersionPageBlockRenderDetailsByIdQueryHandler
    : IQueryHandler<GetCustomEntityVersionPageBlockRenderDetailsByIdQuery, CustomEntityVersionPageBlockRenderDetails?>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IQueryExecutor _queryExecutor;
    private readonly IPageVersionBlockModelMapper _pageVersionBlockModelMapper;
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly ILogger<GetCustomEntityVersionPageBlockRenderDetailsByIdQueryHandler> _logger;

    public GetCustomEntityVersionPageBlockRenderDetailsByIdQueryHandler(
        CofoundryDbContext dbContext,
        IQueryExecutor queryExecutor,
        IPageVersionBlockModelMapper pageVersionBlockModelMapper,
        IPermissionValidationService permissionValidationService,
        ILogger<GetCustomEntityVersionPageBlockRenderDetailsByIdQueryHandler> logger
        )
    {
        _dbContext = dbContext;
        _queryExecutor = queryExecutor;
        _pageVersionBlockModelMapper = pageVersionBlockModelMapper;
        _permissionValidationService = permissionValidationService;
        _logger = logger;
    }

    public async Task<CustomEntityVersionPageBlockRenderDetails?> ExecuteAsync(GetCustomEntityVersionPageBlockRenderDetailsByIdQuery query, IExecutionContext executionContext)
    {
        var dbResult = await QueryBlock(query.CustomEntityVersionPageBlockId)
            .Select(b => new
            {
                PageBlock = b,
                BlockTypeFileName = b.PageBlockType.FileName,
                b.CustomEntityVersion.CustomEntity.CustomEntityDefinitionCode
            })
            .SingleOrDefaultAsync();

        if (dbResult == null)
        {
            return null;
        }

        var result = await MapAsync(
            dbResult.PageBlock,
            dbResult.BlockTypeFileName,
            dbResult.CustomEntityDefinitionCode,
            query.PublishStatus,
            executionContext
            );

        if (result == null)
        {
            return null;
        }

        // Add any list context information.

        if (result.DisplayModel is IListablePageBlockTypeDisplayModel displayData)
        {
            var blocks = await GetOrderedBlockIdsAsync(dbResult.PageBlock);

            displayData.ListContext = new ListablePageBlockRenderContext()
            {
                Index = blocks.IndexOf(result.CustomEntityVersionPageBlockId),
                NumBlocks = blocks.Count
            };
        }

        return result;
    }

    private async Task<List<int>> GetOrderedBlockIdsAsync(CustomEntityVersionPageBlock versionBlock)
    {
        return await _dbContext
            .CustomEntityVersionPageBlocks
            .AsNoTracking()
            .FilterActive()
            .Where(m => m.CustomEntityVersionId == versionBlock.CustomEntityVersionId
                && m.PageTemplateRegionId == versionBlock.PageTemplateRegionId
                && m.PageId == versionBlock.PageId)
            .OrderBy(m => m.Ordering)
            .Select(m => m.CustomEntityVersionPageBlockId)
            .ToListAsync();
    }

    private async Task<CustomEntityVersionPageBlockRenderDetails?> MapAsync(
        CustomEntityVersionPageBlock versionBlock,
        string blockTypeFileName,
        string customEntityDefinitionCode,
        PublishStatusQuery publishStatus,
        IExecutionContext executionContext
        )
    {
        _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(customEntityDefinitionCode, executionContext.UserContext);

        var blockTypeQuery = new GetPageBlockTypeSummaryByIdQuery(versionBlock.PageBlockTypeId);
        var blockType = await _queryExecutor.ExecuteAsync(blockTypeQuery, executionContext);
        EntityNotFoundException.ThrowIfNull(blockType, versionBlock.PageBlockTypeId);

        var displayModel = await _pageVersionBlockModelMapper.MapDisplayModelAsync(
            blockTypeFileName,
            versionBlock,
            publishStatus,
            executionContext
            );

        if (displayModel == null)
        {
            return null;
        }

        var result = new CustomEntityVersionPageBlockRenderDetails
        {
            CustomEntityVersionPageBlockId = versionBlock.CustomEntityVersionPageBlockId,
            BlockType = blockType,
            DisplayModel = displayModel
        };

        result.Template = GetCustomTemplate(versionBlock, blockType);

        return result;
    }

    private IQueryable<CustomEntityVersionPageBlock> QueryBlock(int customEntityVersionPageBlockId)
    {
        return _dbContext
            .CustomEntityVersionPageBlocks
            .AsNoTracking()
            .FilterActive()
            .Where(m => m.CustomEntityVersionPageBlockId == customEntityVersionPageBlockId);
    }

    public PageBlockTypeTemplateSummary? GetCustomTemplate(CustomEntityVersionPageBlock versionBlock, PageBlockTypeSummary blockType)
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
            _logger.LogWarning("The page block type template with id {PageBlockTypeTemplateId} could not be found for custom entity block {CustomEntityVersionPageBlockId}", versionBlock.PageBlockTypeTemplateId, versionBlock.CustomEntityVersionPageBlockId);
        }

        return template;
    }
}