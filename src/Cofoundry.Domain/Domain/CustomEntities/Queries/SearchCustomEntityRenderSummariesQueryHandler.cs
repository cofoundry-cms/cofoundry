using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    public class SearchCustomEntityRenderSummariesQueryHandler
        : IQueryHandler<SearchCustomEntityRenderSummariesQuery, PagedQueryResult<CustomEntityRenderSummary>>
        , IPermissionRestrictedQueryHandler<SearchCustomEntityRenderSummariesQuery, PagedQueryResult<CustomEntityRenderSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;
        private readonly ICustomEntityRenderSummaryMapper _customEntityRenderSummaryMapper;

        public SearchCustomEntityRenderSummariesQueryHandler(
            CofoundryDbContext dbContext,
            ICustomEntityRenderSummaryMapper customEntityRenderSummaryMapper,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            _dbContext = dbContext;
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
            var definition = _customEntityDefinitionRepository.GetByCode(query.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, query.CustomEntityDefinitionCode);

            var dbQuery = _dbContext
                .CustomEntityPublishStatusQueries
                .AsNoTracking()
                .Include(c => c.CustomEntityVersion)
                .ThenInclude(c => c.CustomEntity)
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

            var dbPagedResult = await dbQuery
                .SortBy(definition, query.SortBy, query.SortDirection)
                .Select(p => p.CustomEntityVersion)
                .ToPagedResultAsync(query);

            return dbPagedResult;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(SearchCustomEntityRenderSummariesQuery query)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, query.CustomEntityDefinitionCode);

            yield return new CustomEntityReadPermission(definition);
        }

        #endregion
    }
}
