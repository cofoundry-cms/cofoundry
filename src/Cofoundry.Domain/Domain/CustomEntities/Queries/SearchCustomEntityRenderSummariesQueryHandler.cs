using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System.Data.Entity;

namespace Cofoundry.Domain
{
    public class SearchCustomEntityRenderSummariesQueryHandler
        : IQueryHandler<SearchCustomEntityRenderSummariesQuery, PagedQueryResult<CustomEntityRenderSummary>>
        , IAsyncQueryHandler<SearchCustomEntityRenderSummariesQuery, PagedQueryResult<CustomEntityRenderSummary>>
        , IPermissionRestrictedQueryHandler<SearchCustomEntityRenderSummariesQuery, PagedQueryResult<CustomEntityRenderSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICustomEntityCodeDefinitionRepository _customEntityDefinitionRepository;
        private readonly ICustomEntityRenderSummaryMapper _customEntityRenderSummaryMapper;

        public SearchCustomEntityRenderSummariesQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            ICustomEntityRenderSummaryMapper customEntityRenderSummaryMapper,
            ICustomEntityCodeDefinitionRepository customEntityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _customEntityRenderSummaryMapper = customEntityRenderSummaryMapper;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        #endregion

        #region execution

        public PagedQueryResult<CustomEntityRenderSummary> Execute(SearchCustomEntityRenderSummariesQuery query, IExecutionContext executionContext)
        {
            var dbQuery = GetQuery(query);
            var dbPagedResult = dbQuery.ToPagedResult(query);
            var results = _customEntityRenderSummaryMapper.MapSummaries(dbPagedResult.Items, executionContext);

            return dbPagedResult.ChangeType(results);
        }

        public async Task<PagedQueryResult<CustomEntityRenderSummary>> ExecuteAsync(SearchCustomEntityRenderSummariesQuery query, IExecutionContext executionContext)
        {
            var dbQuery = GetQuery(query);
            var dbPagedResult = await dbQuery.ToPagedResultAsync(query);
            var results = await _customEntityRenderSummaryMapper.MapSummariesAsync(dbPagedResult.Items, executionContext);

            return dbPagedResult.ChangeType(results);
        }
        
        private IQueryable<CustomEntityVersion> GetQuery(SearchCustomEntityRenderSummariesQuery query)
        {
            var definition = _queryExecutor.GetById<CustomEntityDefinitionSummary>(query.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, query.CustomEntityDefinitionCode);

            var dbQuery = _dbContext
                .CustomEntityVersions
                .AsNoTracking()
                .Where(e => e.CustomEntity.CustomEntityDefinitionCode == query.CustomEntityDefinitionCode)
                .Where(v => v.WorkFlowStatusId == (int)Domain.WorkFlowStatus.Draft || v.WorkFlowStatusId == (int)Domain.WorkFlowStatus.Published)
                .GroupBy(e => e.CustomEntityId, (key, g) => g.OrderByDescending(v => v.WorkFlowStatusId == (int)Domain.WorkFlowStatus.Draft).FirstOrDefault())
                .Include(e => e.CustomEntity);

            // Filter by locale 
            if (query.LocaleId > 0)
            {
                dbQuery = dbQuery.Where(p => p.CustomEntity.LocaleId == query.LocaleId);
            }
            else
            {
                dbQuery = dbQuery.Where(p => !p.CustomEntity.LocaleId.HasValue);
            }

            switch (query.SortBy)
            {
                case CustomEntityQuerySortType.Default:
                case CustomEntityQuerySortType.Natural:
                    if (definition.Ordering != CustomEntityOrdering.None)
                    {
                        dbQuery = dbQuery
                            .OrderByWithSortDirection(e => !e.CustomEntity.Ordering.HasValue, query.SortDirection)
                            .ThenByWithSortDirection(e => e.CustomEntity.Ordering, query.SortDirection)
                            .ThenByDescendingWithSortDirection(e => e.CreateDate, query.SortDirection);
                    }
                    else
                    {
                        dbQuery = dbQuery
                            .OrderByDescendingWithSortDirection(e => e.CreateDate, query.SortDirection);
                    }
                    break;
                case CustomEntityQuerySortType.Title:
                    dbQuery = dbQuery
                        .OrderByWithSortDirection(e => e.Title, query.SortDirection);
                    break;
                case CustomEntityQuerySortType.CreateDate:
                    dbQuery = dbQuery
                        .OrderByDescendingWithSortDirection(e => e.CreateDate, query.SortDirection);
                    break;
            }

            return dbQuery;
        }
        
        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(SearchCustomEntityRenderSummariesQuery query)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.CustomEntityDefinitionCode);
            yield return new CustomEntityReadPermission(definition);
        }

        #endregion
    }
}
