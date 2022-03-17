using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Common mapping functionality for PageSummaries
/// </summary>
public class PageSummaryMapper : IPageSummaryMapper
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IQueryExecutor _queryExecutor;
    private readonly IAuditDataMapper _auditDataMapper;
    private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

    public PageSummaryMapper(
        CofoundryDbContext dbContext,
        IQueryExecutor queryExecutor,
        IAuditDataMapper auditDataMapper,
        ICustomEntityDefinitionRepository customEntityDefinitionRepository
        )
    {
        _dbContext = dbContext;
        _queryExecutor = queryExecutor;
        _auditDataMapper = auditDataMapper;
        _customEntityDefinitionRepository = customEntityDefinitionRepository;
    }

    /// <summary>
    /// Finishes off bulk mapping of tags and page routes in a PageSummary object
    /// </summary>
    public virtual async Task<List<PageSummary>> MapAsync(ICollection<Page> dbPages, IExecutionContext executionContext)
    {
        var ids = dbPages
            .Select(p => p.PageId)
            .ToArray();

        var routes = await _queryExecutor.ExecuteAsync(new GetPageRoutesByIdRangeQuery(ids), executionContext);

        var pageTags = await _dbContext
            .PageTags
            .AsNoTracking()
            .Where(p => ids.Contains(p.PageId))
            .Select(t => new
            {
                PageId = t.PageId,
                Tag = t.Tag.TagText
            })
            .ToListAsync();

        var pages = new List<PageSummary>(ids.Length);

        foreach (var dbPage in dbPages)
        {
            var pageRoute = routes.GetOrDefault(dbPage.PageId);
            EntityNotFoundException.ThrowIfNull(pageRoute, dbPage.PageId);

            var page = new PageSummary()
            {
                FullUrlPath = pageRoute.FullUrlPath,
                HasDraftVersion = pageRoute.HasDraftVersion,
                HasPublishedVersion = pageRoute.HasPublishedVersion,
                PublishDate = pageRoute.PublishDate,
                LastPublishDate = pageRoute.LastPublishDate,
                PublishStatus = pageRoute.PublishStatus,
                Locale = pageRoute.Locale,
                PageId = pageRoute.PageId,
                PageType = pageRoute.PageType,
                Title = pageRoute.Title,
                UrlPath = pageRoute.UrlPath
            };

            page.AuditData = _auditDataMapper.MapCreateAuditData(dbPage);

            if (!string.IsNullOrWhiteSpace(dbPage.CustomEntityDefinitionCode))
            {
                var customEntityDefinition = _customEntityDefinitionRepository.GetByCode(dbPage.CustomEntityDefinitionCode);
                // Graciously fall-back if the definition has been removed, since this is typically just for display purposes
                page.CustomEntityName = customEntityDefinition?.Name ?? "Definition Missing";
            }

            page.Tags = pageTags
                .Where(t => t.PageId == page.PageId)
                .Select(t => t.Tag)
                .ToArray();

            pages.Add(page);
        }

        return pages;
    }
}
