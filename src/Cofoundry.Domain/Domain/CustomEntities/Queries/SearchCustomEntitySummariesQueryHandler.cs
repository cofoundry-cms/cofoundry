using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Cofoundry.Domain
{
    public class SearchCustomEntitySummariesQueryHandler 
        : IAsyncQueryHandler<SearchCustomEntitySummariesQuery, PagedQueryResult<CustomEntitySummary>>
        , IPermissionRestrictedQueryHandler<SearchCustomEntitySummariesQuery, PagedQueryResult<CustomEntitySummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;
        private readonly ICustomEntityCodeDefinitionRepository _customEntityDefinitionRepository;

        public SearchCustomEntitySummariesQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
            ICustomEntityCodeDefinitionRepository customEntityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        #endregion

        #region execution

        public async Task<PagedQueryResult<CustomEntitySummary>> ExecuteAsync(SearchCustomEntitySummariesQuery query, IExecutionContext executionContext)
        {
            var definition = _queryExecutor.GetById<CustomEntityDefinitionSummary>(query.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, query.CustomEntityDefinitionCode);

            // Get Query
            var dbQuery = GetQuery(query, definition);

            // Execute Query
            var dbPagedResult = dbQuery.ToPagedResult(query);

            var routingsQuery = new GetPageRoutingInfoByCustomEntityIdRangeQuery(query.CustomEntityDefinitionCode, dbPagedResult.Items.Select(e => e.CustomEntityId));
            var routings = await _queryExecutor.ExecuteAsync(routingsQuery);
            var allLocales = (await _queryExecutor.GetAllAsync<ActiveLocale>()).ToDictionary(l => l.LocaleId);

            // Map Items
            var entities = new List<CustomEntitySummary>(dbPagedResult.Items.Length);
            foreach (var dbVersion in dbPagedResult.Items)
            {
                PageRoutingInfo detailsRouting = null;

                if (routings.ContainsKey(dbVersion.CustomEntityId))
                {
                    detailsRouting = routings[dbVersion.CustomEntityId].FirstOrDefault(r => r.CustomEntityRouteRule != null);
                }

                var entity = Mapper.Map<CustomEntitySummary>(dbVersion);

                if (dbVersion.LocaleId.HasValue)
                {
                    entity.Locale = allLocales.GetOrDefault(dbVersion.LocaleId.Value);
                    EntityNotFoundException.ThrowIfNull(entity.Locale, dbVersion.LocaleId.Value);
                }

                if (detailsRouting != null)
                {
                    entity.FullPath = detailsRouting.CustomEntityRouteRule.MakeUrl(detailsRouting.PageRoute, detailsRouting.CustomEntityRoute);
                }

                entity.Model = (ICustomEntityVersionDataModel)_dbUnstructuredDataSerializer.Deserialize(dbVersion.SerializedData, definition.DataModelType);

                entity.AuditData.UpdateDate = dbVersion.VersionAuditData.CreateDate;
                entity.AuditData.Updater = dbVersion.VersionAuditData.Creator;
                entities.Add(entity);
            }

            return dbPagedResult.ChangeType(entities);
        }

        private IQueryable<CustomEntitySummaryQueryModel> GetQuery(SearchCustomEntitySummariesQuery query, CustomEntityDefinitionSummary definition)
        {
            var dbQuery = _dbContext
                .CustomEntityVersions
                .AsNoTracking()
                .Where(e => e.CustomEntity.CustomEntityDefinitionCode == query.CustomEntityDefinitionCode)
                .Where(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft || v.WorkFlowStatusId == (int)WorkFlowStatus.Published)
                .GroupBy(e => e.CustomEntityId, (key, g) => g.OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft).FirstOrDefault());

            // Filter by locale 
            if (query.LocaleId > 0)
            {
                dbQuery = dbQuery.Where(p => p.CustomEntity.LocaleId == query.LocaleId);
            }
            else if (query.InterpretNullLocaleAsNone)
            {
                dbQuery = dbQuery.Where(p => !p.CustomEntity.LocaleId.HasValue);
            }

            if (!string.IsNullOrWhiteSpace(query.Text))
            {
                dbQuery = dbQuery
                    .Where(e => e.Title.Contains(query.Text) || e.SerializedData.Contains(query.Text))
                    .OrderByDescending(e => e.Title == query.Text)
                    .ThenByDescending(e => e.Title.Contains(query.Text));
            }
            else if (definition.Ordering != CustomEntityOrdering.None)
            {
                dbQuery = dbQuery
                    .OrderBy(e => e.CustomEntity.Locale.IETFLanguageTag)
                    .ThenBy(e => !e.CustomEntity.Ordering.HasValue)
                    .ThenBy(e => e.CustomEntity.Ordering)
                    .ThenBy(e => e.Title);
            }
            else
            {
                dbQuery = dbQuery
                    .OrderBy(e => e.CustomEntity.Locale.IETFLanguageTag)
                    .ThenBy(e => e.Title);
            }

            return dbQuery.ProjectTo<CustomEntitySummaryQueryModel>();
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(SearchCustomEntitySummariesQuery query)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.CustomEntityDefinitionCode);
            yield return new CustomEntityReadPermission(definition);
        }

        #endregion
    }
}
