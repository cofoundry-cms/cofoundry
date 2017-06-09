using AutoMapper;
using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityMapper : ICustomEntityRenderSummaryMapper
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly CustomEntityDataModelMapper _customEntityDataModelMapper;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

        public CustomEntityMapper(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            CustomEntityDataModelMapper customEntityDataModelMapper,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _customEntityDataModelMapper = customEntityDataModelMapper;
            _queryExecutor = queryExecutor;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        #endregion

        #region ICustomEntityRenderSummaryMapper Implementation

        public async Task<IEnumerable<CustomEntityRenderSummary>> MapSummariesAsync(
            ICollection<CustomEntityVersion> dbResults,
            IExecutionContext executionContext
            )
        {
            var routingsQuery = GetPageRoutingQuery(dbResults);
            var allRoutings = await _queryExecutor.ExecuteAsync(routingsQuery, executionContext);
            var allLocales = await _queryExecutor.GetAllAsync<ActiveLocale>(executionContext);

            return Map(dbResults, allRoutings, allLocales);
        }

        public async Task<CustomEntityRenderSummary> MapSummaryAsync(
            CustomEntityVersion dbResult,
            IExecutionContext executionContext
            )
        {
            var routingQuery = GetPageRoutingQuery(dbResult);
            var routing = await _queryExecutor.ExecuteAsync(routingQuery, executionContext);

            ActiveLocale locale = null;
            if (dbResult.CustomEntity.LocaleId.HasValue)
            {
                locale = await _queryExecutor.GetByIdAsync<ActiveLocale>(dbResult.CustomEntity.LocaleId.Value, executionContext);
            }

            return MapSingle(dbResult, routing, locale);
        }

        #endregion

        #region helpers

        private static GetPageRoutingInfoByCustomEntityIdRangeQuery GetPageRoutingQuery(ICollection<CustomEntityVersion> dbResults)
        {
            return new GetPageRoutingInfoByCustomEntityIdRangeQuery(dbResults.Select(e => e.CustomEntityId));
        }

        private static GetPageRoutingInfoByCustomEntityIdQuery GetPageRoutingQuery(CustomEntityVersion dbResult)
        {
            if (dbResult == null) throw new ArgumentNullException(nameof(dbResult));
            if (dbResult.CustomEntity == null) throw new ArgumentNullException(nameof(dbResult.CustomEntity));

            return new GetPageRoutingInfoByCustomEntityIdQuery(dbResult.CustomEntityId);
        }

        private IEnumerable<CustomEntityRenderSummary> Map(
            ICollection<CustomEntityVersion> dbResults,
            IDictionary<int, IEnumerable<PageRoutingInfo>> allRoutings,
            IEnumerable<ActiveLocale> allLocalesAsEnumerable
            )
        {
            var results = new List<CustomEntityRenderSummary>(dbResults.Count);
            var allLocales = allLocalesAsEnumerable.ToDictionary(l => l.LocaleId);

            foreach (var dbResult in dbResults)
            {
                var entity = Mapper.Map<CustomEntityRenderSummary>(dbResult);

                if (dbResult.CustomEntity.LocaleId.HasValue)
                {
                    entity.Locale = allLocales.GetOrDefault(dbResult.CustomEntity.LocaleId.Value);
                    EntityNotFoundException.ThrowIfNull(entity.Locale, dbResult.CustomEntity.LocaleId.Value);
                }

                entity.DetailsPageUrls = MapPageRoutings(allRoutings.GetOrDefault(dbResult.CustomEntityId), dbResult);
                entity.Model = _customEntityDataModelMapper.Map(dbResult.CustomEntity.CustomEntityDefinitionCode, dbResult.SerializedData);

                results.Add(entity);
            }

            return results;
        }

        private CustomEntityRenderSummary MapSingle(
            CustomEntityVersion dbResult,
            IEnumerable<PageRoutingInfo> allRoutings,
            ActiveLocale locale
            )
        {
            var entity = Mapper.Map<CustomEntityRenderSummary>(dbResult);
            entity.Locale = locale;
            entity.Model = _customEntityDataModelMapper.Map(dbResult.CustomEntity.CustomEntityDefinitionCode, dbResult.SerializedData);
            entity.DetailsPageUrls = MapPageRoutings(allRoutings, dbResult);

            return entity;
        }

        private IEnumerable<string> MapPageRoutings(
            IEnumerable<PageRoutingInfo> allRoutings, 
            CustomEntityVersion dbResult)
        {
            if (allRoutings == null) return Enumerable.Empty<string>();

            var urls = new List<string>(allRoutings.Count());

            foreach (var detailsRouting in allRoutings
                .Where(r => r.CustomEntityRouteRule != null))
            {
                var detailsUrl = detailsRouting
                    .CustomEntityRouteRule
                    .MakeUrl(detailsRouting.PageRoute, detailsRouting.CustomEntityRoute);

                urls.Add(detailsUrl);
            }

            return urls;
        }

        #endregion
    }
}
