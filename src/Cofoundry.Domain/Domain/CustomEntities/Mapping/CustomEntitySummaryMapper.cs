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
    /// <summary>
    /// Simple mapper for mapping to CustomEntityVersion objects.
    /// </summary>
    public class CustomEntitySummaryMapper : ICustomEntitySummaryMapper
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;
        private readonly IAuditDataMapper _auditDataMapper;

        public CustomEntitySummaryMapper(
            IQueryExecutor queryExecutor,
            IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
            IAuditDataMapper auditDataMapper
            )
        {
            _queryExecutor = queryExecutor;
            _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
            _auditDataMapper = auditDataMapper;
        }

        /// <summary>
        /// Maps a collection of EF CustomEntityVersion records from the db into CustomEntitySummary 
        /// objects.
        /// </summary>
        /// <param name="dbStatusQueries">Collection of versions to map.</param>
        public async Task<List<CustomEntitySummary>> MapAsync(ICollection<CustomEntityPublishStatusQuery> dbStatusQueries, IExecutionContext executionContext)
        {
            var entities = new List<CustomEntitySummary>(dbStatusQueries.Count);
            var routingsQuery = new GetPageRoutingInfoByCustomEntityIdRangeQuery(dbStatusQueries.Select(e => e.CustomEntityId));
            var routings = await _queryExecutor.ExecuteAsync(routingsQuery, executionContext);

            Dictionary<int, ActiveLocale> allLocales = null;
            Dictionary<string, CustomEntityDefinitionSummary> customEntityDefinitions = new Dictionary<string, CustomEntityDefinitionSummary>();
            var hasCheckedQueryValid = false;

            foreach (var dbStatusQuery in dbStatusQueries)
            {
                // Validate the input data
                if (!hasCheckedQueryValid) ValidateQuery(dbStatusQuery);
                hasCheckedQueryValid = true;
                
                // Easy mappings
                var entity = new CustomEntitySummary()
                {
                    AuditData = _auditDataMapper.MapUpdateAuditDataCreatorData(dbStatusQuery.CustomEntity),
                    CustomEntityDefinitionCode = dbStatusQuery.CustomEntity.CustomEntityDefinitionCode,
                    CustomEntityId = dbStatusQuery.CustomEntityId,
                    HasDraft = dbStatusQuery.CustomEntityVersion.WorkFlowStatusId == (int)WorkFlowStatus.Draft,
                    PublishStatus = PublishStatusMapper.FromCode(dbStatusQuery.CustomEntity.PublishStatusCode),
                    PublishDate = dbStatusQuery.CustomEntity.PublishDate,
                    Ordering = dbStatusQuery.CustomEntity.Ordering,
                    Title = dbStatusQuery.CustomEntityVersion.Title,
                    UrlSlug = dbStatusQuery.CustomEntity.UrlSlug
                };

                entity.IsPublished = entity.PublishStatus == PublishStatus.Published && entity.PublishDate <= executionContext.ExecutionDate;
                _auditDataMapper.MapUpdateAuditDataUpdaterData(entity.AuditData, dbStatusQuery.CustomEntityVersion);


                // Routing data (if any)

                PageRoutingInfo detailsRouting = null;
                if (routings.ContainsKey(dbStatusQuery.CustomEntityId))
                {
                    detailsRouting = routings[dbStatusQuery.CustomEntityId].FirstOrDefault(r => r.CustomEntityRouteRule != null);
                    entity.FullPath = detailsRouting?.CustomEntityRouteRule?.MakeUrl(detailsRouting.PageRoute, detailsRouting.CustomEntityRoute);
                }

                // Locale data

                var localeId = dbStatusQuery.CustomEntity.LocaleId;
                if (localeId.HasValue && detailsRouting != null)
                {
                    entity.Locale = detailsRouting.PageRoute.Locale;
                    EntityNotFoundException.ThrowIfNull(entity.Locale, localeId.Value);
                }
                else if (localeId.HasValue)
                {
                    // Lazy load locales, since they aren't always used
                    if (allLocales == null)
                    {
                        allLocales = await GetLocales();
                    }

                    entity.Locale = allLocales.GetOrDefault(localeId.Value);
                    EntityNotFoundException.ThrowIfNull(entity.Locale, localeId.Value);
                }

                // Parse model data
                var definition = customEntityDefinitions.GetOrDefault(dbStatusQuery.CustomEntity.CustomEntityDefinitionCode);
                if (definition == null)
                {
                    // Load and cache definitions
                    definition = await _queryExecutor.GetByIdAsync<CustomEntityDefinitionSummary>(dbStatusQuery.CustomEntity.CustomEntityDefinitionCode, executionContext);
                    EntityNotFoundException.ThrowIfNull(definition, definition.CustomEntityDefinitionCode);
                    customEntityDefinitions.Add(dbStatusQuery.CustomEntity.CustomEntityDefinitionCode, definition);
                }

                entity.Model = (ICustomEntityDataModel)_dbUnstructuredDataSerializer.Deserialize(dbStatusQuery.CustomEntityVersion.SerializedData, definition.DataModelType);

                entities.Add(entity);
            }

            return entities;
        }

        private void ValidateQuery(CustomEntityPublishStatusQuery dbStatusQuery)
        {
            if (dbStatusQuery.CustomEntity == null)
            {
                throw new ArgumentException("Invalid query: CustomEntityVersion.CustomEntity cannot be null. Have you included it in the query?");
            }
        }

        private async System.Threading.Tasks.Task<Dictionary<int, ActiveLocale>> GetLocales()
        {
            return (await _queryExecutor.GetAllAsync<ActiveLocale>()).ToDictionary(l => l.LocaleId);
        }
    }
}
