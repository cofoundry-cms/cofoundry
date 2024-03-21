using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class CustomEntityRenderSummaryMapper : ICustomEntityRenderSummaryMapper
{
    private readonly ICustomEntityDataModelMapper _customEntityDataModelMapper;
    private readonly IQueryExecutor _queryExecutor;

    public CustomEntityRenderSummaryMapper(
        IQueryExecutor queryExecutor,
        ICustomEntityDataModelMapper customEntityDataModelMapper
        )
    {
        _customEntityDataModelMapper = customEntityDataModelMapper;
        _queryExecutor = queryExecutor;
    }

    public async Task<CustomEntityRenderSummary?> MapAsync(
        CustomEntityVersion? dbResult,
        IExecutionContext executionContext
        )
    {
        if (dbResult == null)
        {
            return null;
        }

        var routingQuery = GetPageRoutingQuery(dbResult);
        var routing = await _queryExecutor.ExecuteAsync(routingQuery, executionContext);

        ActiveLocale? locale = null;
        if (dbResult.CustomEntity.LocaleId.HasValue)
        {
            var getLocaleQuery = new GetActiveLocaleByIdQuery(dbResult.CustomEntity.LocaleId.Value);
            locale = await _queryExecutor.ExecuteAsync(getLocaleQuery, executionContext);
        }

        return MapSingle(dbResult, routing, locale);
    }

    public async Task<IReadOnlyCollection<CustomEntityRenderSummary>> MapAsync(
        IReadOnlyCollection<CustomEntityVersion> dbResults,
        IExecutionContext executionContext
        )
    {
        var routingsQuery = GetPageRoutingQuery(dbResults);
        var allRoutings = await _queryExecutor.ExecuteAsync(routingsQuery, executionContext);
        var allLocales = await _queryExecutor.ExecuteAsync(new GetAllActiveLocalesQuery(), executionContext);

        return Map(dbResults, allRoutings, allLocales);
    }

    private static GetPageRoutingInfoByCustomEntityIdRangeQuery GetPageRoutingQuery(IReadOnlyCollection<CustomEntityVersion> dbResults)
    {
        return new GetPageRoutingInfoByCustomEntityIdRangeQuery(dbResults.Select(e => e.CustomEntityId));
    }

    private static GetPageRoutingInfoByCustomEntityIdQuery GetPageRoutingQuery(CustomEntityVersion dbResult)
    {
        ArgumentNullException.ThrowIfNull(dbResult);
        ArgumentNullException.ThrowIfNull(dbResult.CustomEntity);

        return new GetPageRoutingInfoByCustomEntityIdQuery(dbResult.CustomEntityId);
    }

    private IReadOnlyCollection<CustomEntityRenderSummary> Map(
        IReadOnlyCollection<CustomEntityVersion> dbResults,
        IReadOnlyDictionary<int, IReadOnlyCollection<PageRoutingInfo>> allRoutings,
        IReadOnlyCollection<ActiveLocale> allLocalesAsEnumerable
        )
    {
        var results = new List<CustomEntityRenderSummary>(dbResults.Count);
        var allLocales = allLocalesAsEnumerable.ToDictionary(l => l.LocaleId);

        foreach (var dbResult in dbResults)
        {
            var entity = MapCore(dbResult);

            if (dbResult.CustomEntity.LocaleId.HasValue)
            {
                entity.Locale = allLocales.GetOrDefault(dbResult.CustomEntity.LocaleId.Value);
                EntityNotFoundException.ThrowIfNull(entity.Locale, dbResult.CustomEntity.LocaleId.Value);
            }

            entity.PageUrls = MapPageRoutings(allRoutings.GetValueOrDefault(dbResult.CustomEntityId));

            results.Add(entity);
        }

        return results;
    }

    private CustomEntityRenderSummary MapSingle(
        CustomEntityVersion dbResult,
        IReadOnlyCollection<PageRoutingInfo> allRoutings,
        ActiveLocale? locale
        )
    {
        var entity = MapCore(dbResult);
        entity.Locale = locale;
        entity.PageUrls = MapPageRoutings(allRoutings);

        return entity;
    }

    private CustomEntityRenderSummary MapCore(CustomEntityVersion dbResult)
    {
        var entity = new CustomEntityRenderSummary()
        {
            CreateDate = dbResult.CreateDate,
            CustomEntityDefinitionCode = dbResult.CustomEntity.CustomEntityDefinitionCode,
            CustomEntityId = dbResult.CustomEntityId,
            CustomEntityVersionId = dbResult.CustomEntityVersionId,
            Ordering = dbResult.CustomEntity.Ordering,
            Title = dbResult.Title,
            UrlSlug = dbResult.CustomEntity.UrlSlug,
            WorkFlowStatus = (WorkFlowStatus)dbResult.WorkFlowStatusId,
            PublishDate = dbResult.CustomEntity.PublishDate,
            LastPublishDate = dbResult.CustomEntity.LastPublishDate
        };

        entity.PublishStatus = PublishStatusMapper.FromCode(dbResult.CustomEntity.PublishStatusCode);
        entity.Model = _customEntityDataModelMapper.Map(dbResult.CustomEntity.CustomEntityDefinitionCode, dbResult.SerializedData);

        return entity;
    }

    private static IReadOnlyCollection<string> MapPageRoutings(
        IReadOnlyCollection<PageRoutingInfo>? allRoutings
        )
    {
        if (allRoutings == null)
        {
            return Array.Empty<string>();
        }

        var urls = new List<string>(allRoutings.Count);

        foreach (var detailsRouting in allRoutings)
        {
            if (detailsRouting.CustomEntityRouteRule != null)
            {
                EntityInvalidOperationException.ThrowIfNull(detailsRouting, detailsRouting.CustomEntityRoute);

                var detailsUrl = detailsRouting
                    .CustomEntityRouteRule
                    .MakeUrl(detailsRouting.PageRoute, detailsRouting.CustomEntityRoute);

                urls.Add(detailsUrl);
            }
        }

        return urls;
    }
}
