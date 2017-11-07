using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;
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
            var dbPagedResult = await RunQueryAsync(query, definition, executionContext);
            var mappedResult = await _customEntitySummaryMapper.MapAsync(dbPagedResult.Items, executionContext);

            return dbPagedResult.ChangeType(mappedResult);
        }

        private Task<PagedQueryResult<CustomEntityPublishStatusQuery>> RunQueryAsync(SearchCustomEntitySummariesQuery query, CustomEntityDefinitionSummary definition, IExecutionContext executionContext)
        {
            var dbQuery = _dbContext
                .CustomEntityPublishStatusQueries
                .AsNoTracking()
                .Include(v => v.CustomEntityVersion)
                .ThenInclude(v => v.Creator)
                .Include(v => v.CustomEntity)
                .ThenInclude(c => c.Creator)
                .Include(v => v.CustomEntity)
                .ThenInclude(c => c.Locale)
                .FilterByActive()
                .FilterByStatus(PublishStatusQuery.Latest, executionContext.ExecutionDate)
                .FilterByCustomEntityDefinitionCode(query.CustomEntityDefinitionCode);

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
                    .Where(e => e.CustomEntityVersion.Title.Contains(query.Text) || e.CustomEntityVersion.SerializedData.Contains(query.Text))
                    .OrderByDescending(e => e.CustomEntityVersion.Title == query.Text)
                    .ThenByDescending(e => e.CustomEntityVersion.Title.Contains(query.Text));
            }
            else if (definition.Ordering != CustomEntityOrdering.None)
            {
                dbQuery = dbQuery
                    .OrderBy(e => e.CustomEntity.Locale.IETFLanguageTag)
                    .ThenBy(e => !e.CustomEntity.Ordering.HasValue)
                    .ThenBy(e => e.CustomEntity.Ordering)
                    .ThenBy(e => e.CustomEntityVersion.Title);
            }
            else
            {
                dbQuery = dbQuery
                    .OrderBy(e => e.CustomEntity.Locale.IETFLanguageTag)
                    .ThenBy(e => e.CustomEntityVersion.Title);
            }
            
            var dbPagedResult = dbQuery.ToPagedResultAsync(query);

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
