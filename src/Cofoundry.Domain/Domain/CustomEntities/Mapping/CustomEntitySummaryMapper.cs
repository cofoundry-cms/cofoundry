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
        /// Maps a collection of EF CustomEntityVersion records from the db into a CustomEntitySummary 
        /// objects.
        /// </summary>
        /// <param name="dbVersions">Collection of vewrsions to map.</param>
        public async Task<List<CustomEntitySummary>> MapAsync(ICollection<CustomEntityVersion> dbVersions, IExecutionContext executionContext)
        {
            var entities = new List<CustomEntitySummary>(dbVersions.Count);
            var routingsQuery = new GetPageRoutingInfoByCustomEntityIdRangeQuery(dbVersions.Select(e => e.CustomEntityId));
            var routings = await _queryExecutor.ExecuteAsync(routingsQuery, executionContext);

            Dictionary<int, ActiveLocale> allLocales = null;
            Dictionary<string, CustomEntityDefinitionSummary> customEntityDefinitions = new Dictionary<string, CustomEntityDefinitionSummary>();
            var hasCheckedQueryValid = false;

            foreach (var dbVersion in dbVersions)
            {
                // Validate the input data
                if (!hasCheckedQueryValid) ValidateQuery(dbVersion);
                hasCheckedQueryValid = true;
                
                // Easy mappings
                var entity = new CustomEntitySummary()
                {
                    AuditData = _auditDataMapper.MapUpdateAuditDataCreatorData(dbVersion.CustomEntity),
                    CustomEntityDefinitionCode = dbVersion.CustomEntity.CustomEntityDefinitionCode,
                    CustomEntityId = dbVersion.CustomEntityId,
                    HasDraft = dbVersion.WorkFlowStatusId == (int)WorkFlowStatus.Draft,
                    IsPublished = dbVersion.WorkFlowStatusId == (int)WorkFlowStatus.Published,
                    Ordering = dbVersion.CustomEntity.Ordering,
                    Title = dbVersion.Title,
                    UrlSlug = dbVersion.CustomEntity.UrlSlug
                };

                // We need to do an additional check here to see if a previous version was published
                // TODO: This need optimizing!
                if (!entity.IsPublished)
                {
                    entity.IsPublished = dbVersion.CustomEntity.CustomEntityVersions.Any(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Published);
                }
                _auditDataMapper.MapUpdateAuditDataUpdaterData(entity.AuditData, dbVersion);


                // Routing data (if any)

                PageRoutingInfo detailsRouting = null;
                if (routings.ContainsKey(dbVersion.CustomEntityId))
                {
                    detailsRouting = routings[dbVersion.CustomEntityId].FirstOrDefault(r => r.CustomEntityRouteRule != null);
                    entity.FullPath = detailsRouting?.CustomEntityRouteRule?.MakeUrl(detailsRouting.PageRoute, detailsRouting.CustomEntityRoute);
                }

                // Locale data

                var localeId = dbVersion.CustomEntity.LocaleId;
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
                var definition = customEntityDefinitions.GetOrDefault(dbVersion.CustomEntity.CustomEntityDefinitionCode);
                if (definition == null)
                {
                    // Load and cache definitions
                    definition = await _queryExecutor.GetByIdAsync<CustomEntityDefinitionSummary>(dbVersion.CustomEntity.CustomEntityDefinitionCode, executionContext);
                    EntityNotFoundException.ThrowIfNull(definition, definition.CustomEntityDefinitionCode);
                    customEntityDefinitions.Add(dbVersion.CustomEntity.CustomEntityDefinitionCode, definition);
                }

                entity.Model = (ICustomEntityDataModel)_dbUnstructuredDataSerializer.Deserialize(dbVersion.SerializedData, definition.DataModelType);

                entities.Add(entity);
            }

            return entities;
        }

        private void ValidateQuery(CustomEntityVersion dbVersion)
        {
            if (dbVersion.CustomEntity == null)
            {
                throw new ArgumentException("Invalid query: CustomEntityVersion.CustomEntity cannot be null. Have you included it in the query?");
            }
            if (dbVersion.CustomEntity.CustomEntityVersions.Count == 0)
            {
                throw new ArgumentException("Invalid query: CustomEntityVersion.CustomEntityVersions cannot be empty. Have you included it in the query?");
            }
        }

        private async System.Threading.Tasks.Task<Dictionary<int, ActiveLocale>> GetLocales()
        {
            return (await _queryExecutor.GetAllAsync<ActiveLocale>()).ToDictionary(l => l.LocaleId);
        }
    }
}
