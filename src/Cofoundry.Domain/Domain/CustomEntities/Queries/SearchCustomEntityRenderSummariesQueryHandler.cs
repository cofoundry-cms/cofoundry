using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class SearchCustomEntityRenderSummariesQueryHandler
        : IAsyncQueryHandler<SearchCustomEntityRenderSummariesQuery, PagedQueryResult<CustomEntityRenderSummary>>
        , IPermissionRestrictedQueryHandler<SearchCustomEntityRenderSummariesQuery, PagedQueryResult<CustomEntityRenderSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;
        private readonly ICustomEntityRenderSummaryMapper _customEntityRenderSummaryMapper;

        public SearchCustomEntityRenderSummariesQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            ICustomEntityRenderSummaryMapper customEntityRenderSummaryMapper,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _customEntityRenderSummaryMapper = customEntityRenderSummaryMapper;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        #endregion

        #region execution

        public async Task<PagedQueryResult<CustomEntityRenderSummary>> ExecuteAsync(SearchCustomEntityRenderSummariesQuery query, IExecutionContext executionContext)
        {
            var dbPagedResult = await GetQueryAsync(query, executionContext);
            var results = await _customEntityRenderSummaryMapper.MapAsync(dbPagedResult.Items, executionContext);

            return dbPagedResult.ChangeType(results);
        }

        private async Task<PagedQueryResult<CustomEntityVersion>> GetQueryAsync(SearchCustomEntityRenderSummariesQuery query, IExecutionContext executionContext)
        {
            var definitionQuery = new GetCustomEntityDefinitionSummaryByCodeQuery(query.CustomEntityDefinitionCode);
            var definition = await _queryExecutor.ExecuteAsync(definitionQuery, executionContext);
            EntityNotFoundException.ThrowIfNull(definition, query.CustomEntityDefinitionCode);

            var dbQuery = _dbContext
                .CustomEntityPublishStatusQueries
                .AsNoTracking()
                .Include(e => e.CustomEntityVersion)
                .ThenInclude(e => e.CustomEntity)
                .FilterByCustomEntityDefinitionCode(query.CustomEntityDefinitionCode)
                .FilterActive()
                .FilterByStatus(query.PublishStatus, executionContext.ExecutionDate);

            // Filter by locale 
            if (query.LocaleId > 0 && definition.HasLocale)
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
                            .ThenByDescendingWithSortDirection(e => e.CustomEntity.CreateDate, query.SortDirection);
                    }
                    else
                    {
                        dbQuery = dbQuery
                            .OrderByDescendingWithSortDirection(e => e.CustomEntity.CreateDate, query.SortDirection);
                    }
                    break;
                case CustomEntityQuerySortType.Title:
                    dbQuery = dbQuery
                        .OrderByWithSortDirection(e => e.CustomEntityVersion.Title, query.SortDirection);
                    break;
                case CustomEntityQuerySortType.CreateDate:
                    dbQuery = dbQuery
                        .OrderByDescendingWithSortDirection(e => e.CustomEntity.CreateDate, query.SortDirection);
                    break;
                case CustomEntityQuerySortType.PublishDate:
                    dbQuery = dbQuery
                        .OrderByDescendingWithSortDirection(e => e.CustomEntity.PublishDate.HasValue, query.SortDirection)
                        .ThenByDescendingWithSortDirection(e => e.CustomEntity.PublishDate, query.SortDirection)
                        .ThenByDescendingWithSortDirection(e => e.CustomEntity.CreateDate, query.SortDirection)
                        ;
                    break;
            }

            var dbPagedResult = await dbQuery.ToPagedResultAsync(query);
            // EF doesnt support includes after select so need to run this post execution
            var results = dbPagedResult
                .Items
                .Select(p => p.CustomEntityVersion);

            return dbPagedResult.ChangeType(results);
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
