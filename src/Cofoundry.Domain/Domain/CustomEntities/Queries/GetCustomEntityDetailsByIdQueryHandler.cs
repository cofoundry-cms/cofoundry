using Cofoundry.Domain.Data;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Query for detailed information on a custom entity and it's latest version. This 
/// query is primarily used in the admin area because it is not version-specific
/// and the CustomEntityDetails projection includes audit data and other additional 
/// information that should normally be hidden from a customer facing app.
/// </summary>
public class GetCustomEntityDetailsByIdQueryHandler
    : IQueryHandler<GetCustomEntityDetailsByIdQuery, CustomEntityDetails?>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IQueryExecutor _queryExecutor;
    private readonly ICustomEntityDataModelMapper _customEntityDataModelMapper;
    private readonly IPageVersionBlockModelMapper _pageVersionBlockModelMapper;
    private readonly IEntityVersionPageBlockMapper _entityVersionPageBlockMapper;
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly IAuditDataMapper _auditDataMapper;
    private readonly ILogger<GetCustomEntityDetailsByIdQueryHandler> _logger;

    public GetCustomEntityDetailsByIdQueryHandler(
        CofoundryDbContext dbContext,
        IQueryExecutor queryExecutor,
        ICustomEntityDataModelMapper customEntityDataModelMapper,
        IPageVersionBlockModelMapper pageVersionBlockModelMapper,
        IEntityVersionPageBlockMapper entityVersionPageBlockMapper,
        IPermissionValidationService permissionValidationService,
        IAuditDataMapper auditDataMapper,
        ILogger<GetCustomEntityDetailsByIdQueryHandler> logger
        )
    {
        _dbContext = dbContext;
        _queryExecutor = queryExecutor;
        _customEntityDataModelMapper = customEntityDataModelMapper;
        _pageVersionBlockModelMapper = pageVersionBlockModelMapper;
        _entityVersionPageBlockMapper = entityVersionPageBlockMapper;
        _permissionValidationService = permissionValidationService;
        _auditDataMapper = auditDataMapper;
        _logger = logger;
    }

    public async Task<CustomEntityDetails?> ExecuteAsync(GetCustomEntityDetailsByIdQuery query, IExecutionContext executionContext)
    {
        var customEntityVersion = await QueryAsync(query.CustomEntityId);
        if (customEntityVersion == null)
        {
            return null;
        }

        _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(customEntityVersion.CustomEntity.CustomEntityDefinitionCode, executionContext.UserContext);

        return await MapAsync(query, customEntityVersion, executionContext);
    }

    private async Task<CustomEntityDetails?> MapAsync(
        GetCustomEntityDetailsByIdQuery query,
        CustomEntityVersion dbVersion,
        IExecutionContext executionContext
        )
    {
        if (dbVersion == null)
        {
            return null;
        }

        var entity = MapInitialData(dbVersion);

        if (entity.LatestVersion.WorkFlowStatus == WorkFlowStatus.Published)
        {
            entity.HasPublishedVersion = true;
        }
        else
        {
            entity.HasPublishedVersion = await _dbContext
                    .CustomEntityVersions
                    .AnyAsync(v => v.CustomEntityId == query.CustomEntityId && v.WorkFlowStatusId == (int)WorkFlowStatus.Published);
        }

        if (dbVersion.CustomEntity.LocaleId.HasValue)
        {
            var getLocaleQuery = new GetActiveLocaleByIdQuery(dbVersion.CustomEntity.LocaleId.Value);
            entity.Locale = await _queryExecutor.ExecuteAsync(getLocaleQuery, executionContext);
        }

        // Custom Mapping
        entity.LatestVersion.Model = _customEntityDataModelMapper.Map(
            dbVersion.CustomEntity.CustomEntityDefinitionCode,
            dbVersion.SerializedData
            );

        await MapPages(dbVersion, entity, executionContext);

        return entity;
    }

    private CustomEntityDetails MapInitialData(CustomEntityVersion dbVersion)
    {
        var entity = new CustomEntityDetails()
        {
            CustomEntityId = dbVersion.CustomEntity.CustomEntityId,
            UrlSlug = dbVersion.CustomEntity.UrlSlug,
            PublishStatus = PublishStatusMapper.FromCode(dbVersion.CustomEntity.PublishStatusCode),
            PublishDate = dbVersion.CustomEntity.PublishDate,
            LastPublishDate = dbVersion.CustomEntity.LastPublishDate,
        };

        entity.AuditData = _auditDataMapper.MapCreateAuditData(dbVersion.CustomEntity);
        entity.LatestVersion = new CustomEntityVersionDetails()
        {
            CustomEntityVersionId = dbVersion.CustomEntityVersionId,
            Title = dbVersion.Title,
            DisplayVersion = dbVersion.DisplayVersion,
            WorkFlowStatus = (WorkFlowStatus)dbVersion.WorkFlowStatusId
        };

        entity.LatestVersion.AuditData = _auditDataMapper.MapCreateAuditData(dbVersion);
        entity.HasDraftVersion = entity.LatestVersion.WorkFlowStatus == WorkFlowStatus.Draft;

        return entity;
    }

    private async Task MapPages(CustomEntityVersion dbVersion, CustomEntityDetails entity, IExecutionContext executionContext)
    {
        var pages = new List<CustomEntityPage>();
        entity.LatestVersion.Pages = pages;

        var routingsQuery = new GetPageRoutingInfoByCustomEntityIdQuery(dbVersion.CustomEntityId);
        var routings = (await _queryExecutor.ExecuteAsync(routingsQuery, executionContext))
            .Where(r => r.CustomEntityRouteRule != null);

        if (!routings.Any())
        {
            return;
        }

        // Map templates

        var pageTemplateIds = routings
            .Select(r => new
            {
                r.PageRoute.PageId,
                VersionRoute = r.PageRoute.Versions.GetVersionRouting(PublishStatusQuery.Latest)
            })
            .Where(r => r.VersionRoute != null && r.VersionRoute.HasCustomEntityRegions)
            .ToDictionary(k => k.PageId, v => v.VersionRoute!.PageTemplateId);

        var allTemplateIds = pageTemplateIds
            .Select(r => r.Value)
            .ToArray();

        var allTemplateRegions = await _dbContext
            .PageTemplateRegions
            .AsNoTracking()
            .Where(s => allTemplateIds.Contains(s.PageTemplateId) && s.IsCustomEntityRegion)
            .ToListAsync();

        var allPageBlockTypes = await _queryExecutor.ExecuteAsync(new GetAllPageBlockTypeSummariesQuery(), executionContext);

        foreach (var routing in routings)
        {
            EntityInvalidOperationException.ThrowIfNull(routing, routing.CustomEntityRouteRule);
            EntityInvalidOperationException.ThrowIfNull(routing, routing.CustomEntityRoute);

            var page = new CustomEntityPage();
            pages.Add(page);
            page.FullUrlPath = routing.CustomEntityRouteRule.MakeUrl(routing.PageRoute, routing.CustomEntityRoute);
            page.PageRoute = routing.PageRoute;

            // Map Regions

            var templateId = pageTemplateIds.GetValueOrDefault(routing.PageRoute.PageId);
            page.Regions = allTemplateRegions
                .Where(s => s.PageTemplateId == templateId)
                .OrderBy(s => s.UpdateDate)
                .Select(s => new CustomEntityPageRegionDetails()
                {
                    Name = s.Name,
                    PageTemplateRegionId = s.PageTemplateRegionId
                })
                .ToList();

            // Map Blocks

            foreach (var region in page.Regions)
            {
                region.Blocks = dbVersion
                    .CustomEntityVersionPageBlocks
                    .AsQueryable()
                    .FilterActive()
                    .Where(m => m.PageId == routing.PageRoute.PageId && m.PageTemplateRegionId == region.PageTemplateRegionId)
                    .OrderBy(m => m.Ordering)
                    .Select(m => MapBlock(m, allPageBlockTypes))
                    .WhereNotNull()
                    .ToArray();
            }
        }

        // Map default full path

        entity.FullUrlPath = pages
            .OrderByDescending(p => p.PageRoute.Locale == null)
            .Select(p => p.FullUrlPath)
            .First();
    }

    private CustomEntityVersionPageBlockDetails? MapBlock(CustomEntityVersionPageBlock dbBlock, IReadOnlyCollection<PageBlockTypeSummary> allPageBlockTypes)
    {
        var blockType = allPageBlockTypes.SingleOrDefault(t => t.PageBlockTypeId == dbBlock.PageBlockTypeId);

        if (blockType == null)
        {
            _logger.LogDebug("Could not find page block type with id of {PageBlockTypeId}", dbBlock.PageBlockTypeId);
            return null;
        }

        var block = new CustomEntityVersionPageBlockDetails
        {
            BlockType = blockType,
            DataModel = _pageVersionBlockModelMapper.MapDataModel(blockType.FileName, dbBlock),
            CustomEntityVersionPageBlockId = dbBlock.CustomEntityVersionPageBlockId,
            Template = _entityVersionPageBlockMapper.GetCustomTemplate(dbBlock, blockType)
        };

        return block;
    }

    private Task<CustomEntityVersion?> QueryAsync(int id)
    {
        return _dbContext
            .CustomEntityVersions
            .Include(v => v.CustomEntityVersionPageBlocks)
            .ThenInclude(e => e.PageBlockType)
            .Include(v => v.CustomEntity)
            .ThenInclude(e => e.Creator)
            .Include(v => v.Creator)
            .AsNoTracking()
            .Where(v => v.CustomEntityId == id && (v.CustomEntity.LocaleId == null || v.CustomEntity.Locale!.IsActive))
            .OrderByLatest()
            .FirstOrDefaultAsync();
    }
}
