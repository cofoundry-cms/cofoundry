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
using Microsoft.EntityFrameworkCore;

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
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

        public SearchCustomEntitySummariesQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
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
            var definition = await _queryExecutor.GetByIdAsync<CustomEntityDefinitionSummary>(query.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, query.CustomEntityDefinitionCode);

            // Get Main Query
            var dbPagedResult = await GetQueryAsync(query, definition);
            
            var routingsQuery = new GetPageRoutingInfoByCustomEntityIdRangeQuery(dbPagedResult.Items.Select(e => e.CustomEntityId));
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

                entity.Model = (ICustomEntityDataModel)_dbUnstructuredDataSerializer.Deserialize(dbVersion.SerializedData, definition.DataModelType);

                entity.AuditData.UpdateDate = dbVersion.VersionAuditData.CreateDate;
                entity.AuditData.Updater = dbVersion.VersionAuditData.Creator;
                entities.Add(entity);
            }

            return dbPagedResult.ChangeType(entities);
        }

        private async Task<PagedQueryResult<CustomEntitySummaryQueryModel>> GetQueryAsync(SearchCustomEntitySummariesQuery query, CustomEntityDefinitionSummary definition)
        {
            var dbQuery = (await _dbContext
                .CustomEntityVersions
                .AsNoTracking()
                .Include(v => v.Creator)
                .Include(v => v.CustomEntity)
                .ThenInclude(c => c.Creator)
                .Include(v => v.CustomEntity)
                .ThenInclude(c => c.Locale)
                .Where(e => e.CustomEntity.CustomEntityDefinitionCode == query.CustomEntityDefinitionCode)
                .Where(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft || v.WorkFlowStatusId == (int)WorkFlowStatus.Published)
                .GroupBy(e => e.CustomEntityId, (key, g) => g.OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft).FirstOrDefault())
                // TODO: EF Core: To make this work we must execute the full query before sorting & paging
                .ToListAsync())
                .AsQueryable();

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
                    .OrderBy(e => e.CustomEntity.Locale != null ? e.CustomEntity.Locale.IETFLanguageTag : string.Empty)
                    //.OrderBy(e => e.CustomEntity.Locale.IETFLanguageTag)
                    .ThenBy(e => !e.CustomEntity.Ordering.HasValue)
                    .ThenBy(e => e.CustomEntity.Ordering)
                    .ThenBy(e => e.Title);
            }
            else
            {
                dbQuery = dbQuery
                    // TODO: EF Core: Put this sorting back in when EF core supports it
                    //.OrderBy(e => e.CustomEntity.Locale.IETFLanguageTag)
                    .OrderBy(e => e.CustomEntity.Locale != null ? e.CustomEntity.Locale.IETFLanguageTag : string.Empty)
                    .ThenBy(e => e.Title);
            }

            var projected = dbQuery.ProjectTo<CustomEntitySummaryQueryModel>();
            // TODO: could be async when EF core supports it
            var dbPagedResult = projected.ToPagedResult(query); 

            return dbPagedResult;
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
