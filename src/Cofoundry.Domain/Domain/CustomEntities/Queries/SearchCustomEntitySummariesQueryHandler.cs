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
        private readonly ICustomEntitySummaryMapper _customEntitySummaryMapper;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

        public SearchCustomEntitySummariesQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            ICustomEntitySummaryMapper customEntitySummaryMapper,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _customEntitySummaryMapper = customEntitySummaryMapper;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        #endregion

        #region execution

        public async Task<PagedQueryResult<CustomEntitySummary>> ExecuteAsync(SearchCustomEntitySummariesQuery query, IExecutionContext executionContext)
        {
            var definition = await _queryExecutor.GetByIdAsync<CustomEntityDefinitionSummary>(query.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, query.CustomEntityDefinitionCode);

            // Get Main Query
            var dbPagedResult = await RunQueryAsync(query, definition);
            var mappedResult = await _customEntitySummaryMapper.MapAsync(dbPagedResult.Items, executionContext);

            return dbPagedResult.ChangeType(mappedResult);
        }

        private async Task<PagedQueryResult<CustomEntityVersion>> RunQueryAsync(SearchCustomEntitySummariesQuery query, CustomEntityDefinitionSummary definition)
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

            // TODO: could be async when EF core supports it
            var dbPagedResult = dbQuery.ToPagedResult(query);

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
