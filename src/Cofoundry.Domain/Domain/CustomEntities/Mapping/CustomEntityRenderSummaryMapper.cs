using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to CustomEntityRenderSummary objects.
    /// </summary>
    public class CustomEntityRenderSummaryMapper : ICustomEntityRenderSummaryMapper
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityDataModelMapper _customEntityDataModelMapper;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

        public CustomEntityRenderSummaryMapper(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            ICustomEntityDataModelMapper customEntityDataModelMapper,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _customEntityDataModelMapper = customEntityDataModelMapper;
            _queryExecutor = queryExecutor;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        #endregion

        /// <summary>
        /// Maps an EF CustomEntityVersion record from the db into a CustomEntityRenderSummary 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbResult">CustomEntityVersion record from the database.</param>
        /// <param name="executionContext">Context to run any sub queries under.</param>
        public async Task<CustomEntityRenderSummary> MapAsync(
            CustomEntityVersion dbResult,
            IExecutionContext executionContext
            )
        {
            if (dbResult == null) return null;

            var routingQuery = GetPageRoutingQuery(dbResult);
            var routing = await _queryExecutor.ExecuteAsync(routingQuery, executionContext);

            ActiveLocale locale = null;
            if (dbResult.CustomEntity.LocaleId.HasValue)
            {
                var getLocaleQuery = new GetActiveLocaleByIdQuery(dbResult.CustomEntity.LocaleId.Value);
                locale = await _queryExecutor.ExecuteAsync(getLocaleQuery, executionContext);
            }

            return MapSingle(dbResult, routing, locale);
        }

        /// <summary>
        /// Maps a collection of EF CustomEntityVersion record from the db into CustomEntityRenderSummary 
        /// objects.
        /// </summary>
        /// <param name="dbResult">CustomEntityVersion records from the database.</param>
        /// <param name="executionContext">Context to run any sub queries under.</param>
        public async Task<ICollection<CustomEntityRenderSummary>> MapAsync(
            ICollection<CustomEntityVersion> dbResults,
            IExecutionContext executionContext
            )
        {
            var routingsQuery = GetPageRoutingQuery(dbResults);
            var allRoutings = await _queryExecutor.ExecuteAsync(routingsQuery, executionContext);
            var allLocales = await _queryExecutor.ExecuteAsync(new GetAllActiveLocalesQuery(), executionContext);

            return Map(dbResults, allRoutings, allLocales);
        }

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

        private ICollection<CustomEntityRenderSummary> Map(
            ICollection<CustomEntityVersion> dbResults,
            IDictionary<int, ICollection<PageRoutingInfo>> allRoutings,
            ICollection<ActiveLocale> allLocalesAsEnumerable
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

                entity.PageUrls = MapPageRoutings(allRoutings.GetOrDefault(dbResult.CustomEntityId), dbResult);

                results.Add(entity);
            }

            return results;
        }

        private CustomEntityRenderSummary MapSingle(
            CustomEntityVersion dbResult,
            ICollection<PageRoutingInfo> allRoutings,
            ActiveLocale locale
            )
        {
            var entity = MapCore(dbResult);
            entity.Locale = locale;
            entity.PageUrls = MapPageRoutings(allRoutings, dbResult);

            return entity;
        }

        private CustomEntityRenderSummary MapCore(CustomEntityVersion dbResult)
        {
            var entity = new CustomEntityRenderSummary()
            {
                CreateDate = DbDateTimeMapper.AsUtc(dbResult.CreateDate),
                CustomEntityDefinitionCode = dbResult.CustomEntity.CustomEntityDefinitionCode,
                CustomEntityId = dbResult.CustomEntityId,
                CustomEntityVersionId = dbResult.CustomEntityVersionId,
                Ordering = dbResult.CustomEntity.Ordering,
                Title = dbResult.Title,
                UrlSlug = dbResult.CustomEntity.UrlSlug,
                WorkFlowStatus = (WorkFlowStatus)dbResult.WorkFlowStatusId,
                PublishDate = DbDateTimeMapper.AsUtc(dbResult.CustomEntity.PublishDate)
            };

            entity.PublishStatus = PublishStatusMapper.FromCode(dbResult.CustomEntity.PublishStatusCode);
            entity.Model = _customEntityDataModelMapper.Map(dbResult.CustomEntity.CustomEntityDefinitionCode, dbResult.SerializedData);

            return entity;
        }

        private ICollection<string> MapPageRoutings(
            ICollection<PageRoutingInfo> allRoutings, 
            CustomEntityVersion dbResult)
        {
            if (allRoutings == null) return Array.Empty<string>();

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
