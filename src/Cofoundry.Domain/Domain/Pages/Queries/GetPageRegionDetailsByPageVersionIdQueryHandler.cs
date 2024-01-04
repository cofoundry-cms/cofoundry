using Cofoundry.Domain.Data;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Returns a collection of content managed regions with
/// block data for a specific version of a page.
/// </summary>
public class GetPageRegionDetailsByPageVersionIdQueryHandler
    : IQueryHandler<GetPageRegionDetailsByPageVersionIdQuery, IReadOnlyCollection<PageRegionDetails>>
    , IPermissionRestrictedQueryHandler<GetPageRegionDetailsByPageVersionIdQuery, IReadOnlyCollection<PageRegionDetails>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IQueryExecutor _queryExecutor;
    private readonly IPageVersionBlockModelMapper _pageVersionBlockModelMapper;
    private readonly IEntityVersionPageBlockMapper _entityVersionPageBlockMapper;
    private readonly ILogger<GetPageRegionDetailsByPageVersionIdQueryHandler> _logger;

    public GetPageRegionDetailsByPageVersionIdQueryHandler(
        CofoundryDbContext dbContext,
        IQueryExecutor queryExecutor,
        IPageVersionBlockModelMapper pageVersionBlockModelMapper,
        IEntityVersionPageBlockMapper entityVersionPageBlockMapper,
        ILogger<GetPageRegionDetailsByPageVersionIdQueryHandler> logger
        )
    {
        _dbContext = dbContext;
        _queryExecutor = queryExecutor;
        _pageVersionBlockModelMapper = pageVersionBlockModelMapper;
        _entityVersionPageBlockMapper = entityVersionPageBlockMapper;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<PageRegionDetails>> ExecuteAsync(GetPageRegionDetailsByPageVersionIdQuery query, IExecutionContext executionContext)
    {
        var regions = await GetRegionsAsync(query);
        var dbPageBlocks = await GetPageVersionBlocksAsync(query);
        var allBlockTypes = await _queryExecutor.ExecuteAsync(new GetAllPageBlockTypeSummariesQuery(), executionContext);

        MapRegions(regions, dbPageBlocks, allBlockTypes);

        return regions;
    }

    private void MapRegions(
        IReadOnlyCollection<PageRegionDetails> regions,
        IReadOnlyCollection<PageVersionBlock> dbPageBlocks,
        IReadOnlyCollection<PageBlockTypeSummary> allBlockTypes
        )
    {
        foreach (var region in regions)
        {
            var regionMappedBlocks = dbPageBlocks
                .Where(m => m.PageTemplateRegionId == region.PageTemplateRegionId)
                .OrderBy(m => m.Ordering)
                .Select(m => MapPageBlock(m, allBlockTypes));

            region.Blocks = regionMappedBlocks.ToArray();
        }
    }

    private async Task<IReadOnlyCollection<PageVersionBlock>> GetPageVersionBlocksAsync(GetPageRegionDetailsByPageVersionIdQuery query)
    {
        var dbPageVersionBlocks = await _dbContext
            .PageVersionBlocks
            .AsNoTracking()
            .FilterActive()
            .Where(m => m.PageVersionId == query.PageVersionId)
            .ToArrayAsync();

        return dbPageVersionBlocks;
    }

    private PageVersionBlockDetails MapPageBlock(PageVersionBlock dbBlock, IReadOnlyCollection<PageBlockTypeSummary> allBlockTypes)
    {
        var blockType = allBlockTypes.SingleOrDefault(t => t.PageBlockTypeId == dbBlock.PageBlockTypeId);
        EntityNotFoundException.ThrowIfNull(blockType, dbBlock.PageBlockTypeId);

        var block = new PageVersionBlockDetails();
        block.BlockType = blockType;
        block.DataModel = _pageVersionBlockModelMapper.MapDataModel(blockType.FileName, dbBlock);
        block.PageVersionBlockId = dbBlock.PageVersionBlockId;
        block.Template = _entityVersionPageBlockMapper.GetCustomTemplate(dbBlock, blockType);

        return block;
    }

    private async Task<IReadOnlyCollection<PageRegionDetails>> GetRegionsAsync(GetPageRegionDetailsByPageVersionIdQuery query)
    {
        var dbRegions = await _dbContext
            .PageVersions
            .AsNoTracking()
            .FilterActive()
            .FilterByPageVersionId(query.PageVersionId)
            .SelectMany(v => v.PageTemplate.PageTemplateRegions)
            .Where(s => !s.IsCustomEntityRegion)
            .OrderBy(s => s.UpdateDate)
            .Select(s => new PageRegionDetails()
            {
                PageTemplateRegionId = s.PageTemplateRegionId,
                Name = s.Name
            })
            .ToArrayAsync();

        return dbRegions;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageRegionDetailsByPageVersionIdQuery query)
    {
        yield return new PageReadPermission();
    }
}