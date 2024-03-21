using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class CustomEntitySummaryMapper : ICustomEntitySummaryMapper
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IQueryExecutor _queryExecutor;
    private readonly ICustomEntityDataModelMapper _customEntityDataModelMapper;
    private readonly IAuditDataMapper _auditDataMapper;

    public CustomEntitySummaryMapper(
        CofoundryDbContext dbContext,
        IQueryExecutor queryExecutor,
        ICustomEntityDataModelMapper customEntityDataModelMapper,
        IAuditDataMapper auditDataMapper
        )
    {
        _dbContext = dbContext;
        _queryExecutor = queryExecutor;
        _customEntityDataModelMapper = customEntityDataModelMapper;
        _auditDataMapper = auditDataMapper;
    }

    public async Task<IReadOnlyCollection<CustomEntitySummary>> MapAsync(IReadOnlyCollection<CustomEntityPublishStatusQuery> dbStatusQueries, IExecutionContext executionContext)
    {
        var entities = new List<CustomEntitySummary>(dbStatusQueries.Count);
        var routingsQuery = new GetPageRoutingInfoByCustomEntityIdRangeQuery(dbStatusQueries.Select(e => e.CustomEntityId));
        var routings = await _queryExecutor.ExecuteAsync(routingsQuery, executionContext);

        Dictionary<int, ActiveLocale>? allLocales = null;
        var customEntityDefinitions = new Dictionary<string, CustomEntityDefinitionSummary>();
        var hasCheckedQueryValid = false;

        foreach (var dbCustomEntity in dbStatusQueries)
        {
            // Validate the input data
            if (!hasCheckedQueryValid)
            {
                ValidateQuery(dbCustomEntity);
            }

            hasCheckedQueryValid = true;

            // Easy mappings
            var entity = MapBasicProperties(dbCustomEntity);

            // Routing data (if any)
            var detailsRouting = FindRoutingData(routings, dbCustomEntity);

            if (detailsRouting != null)
            {
                EntityInvalidOperationException.ThrowIfNull(detailsRouting, detailsRouting.CustomEntityRouteRule);
                EntityInvalidOperationException.ThrowIfNull(detailsRouting, detailsRouting.CustomEntityRoute);

                entity.FullUrlPath = detailsRouting.CustomEntityRouteRule.MakeUrl(detailsRouting.PageRoute, detailsRouting.CustomEntityRoute);
                entity.HasPublishedVersion = detailsRouting.CustomEntityRoute.HasPublishedVersion;
            }

            // Locale data

            var localeId = dbCustomEntity.CustomEntity.LocaleId;
            if (localeId.HasValue && detailsRouting != null)
            {
                entity.Locale = detailsRouting.PageRoute.Locale;
                EntityNotFoundException.ThrowIfNull(entity.Locale, localeId.Value);
            }
            else if (localeId.HasValue)
            {
                // Lazy load locales, since they aren't always used
                allLocales ??= await GetLocalesAsync(executionContext);

                entity.Locale = allLocales.GetValueOrDefault(localeId.Value);
                EntityNotFoundException.ThrowIfNull(entity.Locale, localeId.Value);
            }

            entity.Model = _customEntityDataModelMapper.Map(dbCustomEntity.CustomEntity.CustomEntityDefinitionCode, dbCustomEntity.CustomEntityVersion.SerializedData);

            entities.Add(entity);
        }

        await EnsureHasPublishedVersionSet(entities);

        return entities;
    }

    private CustomEntitySummary MapBasicProperties(CustomEntityPublishStatusQuery dbStatusQuery)
    {
        var entity = new CustomEntitySummary()
        {
            AuditData = _auditDataMapper.MapUpdateAuditDataFromVersion(dbStatusQuery.CustomEntity, dbStatusQuery.CustomEntityVersion),
            CustomEntityDefinitionCode = dbStatusQuery.CustomEntity.CustomEntityDefinitionCode,
            CustomEntityId = dbStatusQuery.CustomEntityId,
            HasDraftVersion = dbStatusQuery.CustomEntityVersion.WorkFlowStatusId == (int)WorkFlowStatus.Draft,
            // note that if this is not a published version, we do further checks on this later in the process
            HasPublishedVersion = dbStatusQuery.CustomEntityVersion.WorkFlowStatusId == (int)WorkFlowStatus.Published,
            PublishStatus = PublishStatusMapper.FromCode(dbStatusQuery.CustomEntity.PublishStatusCode),
            PublishDate = dbStatusQuery.CustomEntity.PublishDate,
            LastPublishDate = dbStatusQuery.CustomEntity.LastPublishDate,
            Ordering = dbStatusQuery.CustomEntity.Ordering,
            Title = dbStatusQuery.CustomEntityVersion.Title,
            UrlSlug = dbStatusQuery.CustomEntity.UrlSlug
        };

        return entity;
    }

    /// <summary>
    /// There will only be routing data if there is a custom entity page
    /// associated with this entity type.
    /// </summary>
    private static PageRoutingInfo? FindRoutingData(IReadOnlyDictionary<int, IReadOnlyCollection<PageRoutingInfo>> routings, CustomEntityPublishStatusQuery dbStatusQuery)
    {
        PageRoutingInfo? detailsRouting = null;
        if (routings.ContainsKey(dbStatusQuery.CustomEntityId))
        {
            detailsRouting = routings[dbStatusQuery.CustomEntityId].FirstOrDefault(r => r.CustomEntityRouteRule != null);
        }

        return detailsRouting;
    }

    /// <summary>
    /// The mapper will set HasPublishedVersion if it knows for certain there is 
    /// one using the latest version record, but some entities may not be able to 
    /// determine this so we need to run a query to check.
    /// </summary>
    private async Task EnsureHasPublishedVersionSet(List<CustomEntitySummary> entities)
    {
        var entitiesWithUnconfirmedPublishRecord = entities
            .Where(e => !e.HasPublishedVersion)
            .ToDictionary(e => e.CustomEntityId);

        if (entitiesWithUnconfirmedPublishRecord.Count != 0)
        {
            var publishedEntityIds = await _dbContext
                .CustomEntityPublishStatusQueries
                .Where(q => q.PublishStatusQueryId == (int)PublishStatusQuery.Published
                    && entitiesWithUnconfirmedPublishRecord.Keys.Contains(q.CustomEntityId))
                .Select(q => q.CustomEntityId)
                .ToListAsync();

            foreach (var publishedEntityId in publishedEntityIds)
            {
                var entity = entitiesWithUnconfirmedPublishRecord.GetValueOrDefault(publishedEntityId);
                EntityNotFoundException.ThrowIfNull(entity, publishedEntityId);
                entity.HasPublishedVersion = true;
            }
        }
    }

    /// <summary>
    /// Validates any required object properties are included in the EF query result.
    /// </summary>
    private static void ValidateQuery(CustomEntityPublishStatusQuery dbStatusQuery)
    {
        if (dbStatusQuery.CustomEntity == null)
        {
            throw new ArgumentException("Invalid query: CustomEntityPublishStatusQuery.CustomEntity cannot be null. Have you included it in the query?");
        }

        if (dbStatusQuery.CustomEntityVersion == null)
        {
            throw new ArgumentException("Invalid query: CustomEntityPublishStatusQuery.CustomEntityVersion cannot be null. Have you included it in the query?");
        }
    }

    private async Task<Dictionary<int, ActiveLocale>> GetLocalesAsync(IExecutionContext executionContext)
    {
        var locales = await _queryExecutor.ExecuteAsync(new GetAllActiveLocalesQuery(), executionContext);
        return locales.ToDictionary(l => l.LocaleId);
    }
}
