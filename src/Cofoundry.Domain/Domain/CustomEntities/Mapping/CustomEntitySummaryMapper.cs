using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to CustomEntitySummary objects.
    /// </summary>
    public class CustomEntitySummaryMapper : ICustomEntitySummaryMapper
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;
        private readonly IAuditDataMapper _auditDataMapper;

        public CustomEntitySummaryMapper(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
            IAuditDataMapper auditDataMapper
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
            _auditDataMapper = auditDataMapper;
        }

        /// <summary>
        /// Maps a collection of EF CustomEntityPublishStatusQuery records from the db 
        /// into CustomEntitySummary objects. The records must include data for the the 
        /// CustomEntity, CustomEntityVersion, CustomEntity.Creator and CustomEntityVersion.Creator 
        /// properties.
        /// </summary>
        /// <param name="dbCustomEntities">Collection of CustomEntityPublishStatusQuery records to map.</param>
        /// <param name="executionContext">Execution context to pass down when executing child queries.</param>
        public async Task<List<CustomEntitySummary>> MapAsync(ICollection<CustomEntityPublishStatusQuery> dbCustomEntities, IExecutionContext executionContext)
        {
            var entities = new List<CustomEntitySummary>(dbCustomEntities.Count);
            var routingsQuery = new GetPageRoutingInfoByCustomEntityIdRangeQuery(dbCustomEntities.Select(e => e.CustomEntityId));
            var routings = await _queryExecutor.ExecuteAsync(routingsQuery, executionContext);

            Dictionary<int, ActiveLocale> allLocales = null;
            var customEntityDefinitions = new Dictionary<string, CustomEntityDefinitionSummary>();
            var hasCheckedQueryValid = false;

            foreach (var dbCustomEntity in dbCustomEntities)
            {
                // Validate the input data
                if (!hasCheckedQueryValid) ValidateQuery(dbCustomEntity);
                hasCheckedQueryValid = true;

                // Easy mappings
                var entity = MapBasicProperties(dbCustomEntity);

                // Routing data (if any)
                var detailsRouting = FindRoutingData(routings, dbCustomEntity);

                if (detailsRouting != null)
                {
                    entity.FullPath = detailsRouting.CustomEntityRouteRule.MakeUrl(detailsRouting.PageRoute, detailsRouting.CustomEntityRoute);
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
                    if (allLocales == null)
                    {
                        allLocales = await GetLocalesAsync(executionContext);
                    }

                    entity.Locale = allLocales.GetOrDefault(localeId.Value);
                    EntityNotFoundException.ThrowIfNull(entity.Locale, localeId.Value);
                }

                // Parse model data
                var definition = customEntityDefinitions.GetOrDefault(dbCustomEntity.CustomEntity.CustomEntityDefinitionCode);
                if (definition == null)
                {
                    // Load and cache definitions
                    var definitionQuery = new GetCustomEntityDefinitionSummaryByCodeQuery(dbCustomEntity.CustomEntity.CustomEntityDefinitionCode);
                    definition = await _queryExecutor.ExecuteAsync(definitionQuery, executionContext);

                    EntityNotFoundException.ThrowIfNull(definition, definition.CustomEntityDefinitionCode);
                    customEntityDefinitions.Add(dbCustomEntity.CustomEntity.CustomEntityDefinitionCode, definition);
                }

                entity.Model = (ICustomEntityDataModel)_dbUnstructuredDataSerializer.Deserialize(dbCustomEntity.CustomEntityVersion.SerializedData, definition.DataModelType);

                entities.Add(entity);
            }

            await EnsureHasPublishedVersionSet(entities);

            return entities;
        }

        private CustomEntitySummary MapBasicProperties(CustomEntityPublishStatusQuery dbStatusQuery)
        {
            var entity = new CustomEntitySummary()
            {
                AuditData = _auditDataMapper.MapUpdateAuditDataCreatorData(dbStatusQuery.CustomEntity),
                CustomEntityDefinitionCode = dbStatusQuery.CustomEntity.CustomEntityDefinitionCode,
                CustomEntityId = dbStatusQuery.CustomEntityId,
                HasDraftVersion = dbStatusQuery.CustomEntityVersion.WorkFlowStatusId == (int)WorkFlowStatus.Draft,
                // note that if this is not a published version, we do further checks on this later in the process
                HasPublishedVersion = dbStatusQuery.CustomEntityVersion.WorkFlowStatusId == (int)WorkFlowStatus.Published,
                PublishStatus = PublishStatusMapper.FromCode(dbStatusQuery.CustomEntity.PublishStatusCode),
                PublishDate = DbDateTimeMapper.AsUtc(dbStatusQuery.CustomEntity.PublishDate),
                Ordering = dbStatusQuery.CustomEntity.Ordering,
                Title = dbStatusQuery.CustomEntityVersion.Title,
                UrlSlug = dbStatusQuery.CustomEntity.UrlSlug
            };

            _auditDataMapper.MapUpdateAuditDataUpdaterData(entity.AuditData, dbStatusQuery.CustomEntityVersion);
            return entity;
        }

        /// <summary>
        /// There will only be routing data if there is a custom entity page
        /// associated with this entity type.
        /// </summary>
        private static PageRoutingInfo FindRoutingData(IDictionary<int, ICollection<PageRoutingInfo>> routings, CustomEntityPublishStatusQuery dbStatusQuery)
        {
            PageRoutingInfo detailsRouting = null;
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

            if (entitiesWithUnconfirmedPublishRecord.Any())
            {
                var publishedEntityIds = await _dbContext
                    .CustomEntityPublishStatusQueries
                    .Where(q => q.PublishStatusQueryId == (int)PublishStatusQuery.Published
                        && entitiesWithUnconfirmedPublishRecord.Keys.Contains(q.CustomEntityId))
                    .Select(q => q.CustomEntityId)
                    .ToListAsync();

                foreach (var publishedEntityId in publishedEntityIds)
                {
                    var entity = entitiesWithUnconfirmedPublishRecord.GetOrDefault(publishedEntityId);
                    EntityNotFoundException.ThrowIfNull(entity, publishedEntityId);
                    entity.HasPublishedVersion = true;
                }
            }
        }

        /// <summary>
        /// Validates any required object properties are included in the EF query result.
        /// </summary>
        private void ValidateQuery(CustomEntityPublishStatusQuery dbStatusQuery)
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
}
