using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// A workflow non-specifc search of custom entities which returns basic
    /// custom entity information with workflow status and model data for the
    /// latest version. Designed to be used in the admin panel and not in a 
    /// version-sensitive context sach as a public webpage.
    /// </summary>
    public class SearchCustomEntitySummariesQueryHandler 
        : IQueryHandler<SearchCustomEntitySummariesQuery, PagedQueryResult<CustomEntitySummary>>
        , IPermissionRestrictedQueryHandler<SearchCustomEntitySummariesQuery, PagedQueryResult<CustomEntitySummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntitySummaryMapper _customEntitySummaryMapper;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

        public SearchCustomEntitySummariesQueryHandler(
            CofoundryDbContext dbContext,
            ICustomEntitySummaryMapper customEntitySummaryMapper,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _customEntitySummaryMapper = customEntitySummaryMapper;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        #endregion

        public async Task<PagedQueryResult<CustomEntitySummary>> ExecuteAsync(SearchCustomEntitySummariesQuery query, IExecutionContext executionContext)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, query.CustomEntityDefinitionCode);

            // Get Main Query
            var dbPagedResult = await RunQueryAsync(query, definition, executionContext);
            var mappedResult = await _customEntitySummaryMapper.MapAsync(dbPagedResult.Items, executionContext);

            return dbPagedResult.ChangeType(mappedResult);
        }

        private Task<PagedQueryResult<CustomEntityPublishStatusQuery>> RunQueryAsync(
            SearchCustomEntitySummariesQuery query, 
            ICustomEntityDefinition definition, 
            IExecutionContext executionContext
            )
        {
            var dbQuery = _dbContext
                .CustomEntityPublishStatusQueries
                .AsNoTracking()
                .Include(v => v.CustomEntityVersion)
                .ThenInclude(v => v.Creator)
                .Include(v => v.CustomEntity)
                .ThenInclude(c => c.Creator)
                .FilterActive()
                .FilterByDate(c => c.CustomEntity.CreateDate, query.CreatedAfter, query.CreatedBefore)
                .FilterByStatus(PublishStatusQuery.Latest, executionContext.ExecutionDate)
                .FilterByCustomEntityDefinitionCode(query.CustomEntityDefinitionCode);

            // Filter by locale 
            if (query.LocaleId > 0 && definition.HasLocale)
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
            else
            {
                dbQuery = dbQuery
                    .SortBy(definition, CustomEntityQuerySortType.Default);
            }
            
            var dbPagedResult = dbQuery.ToPagedResultAsync(query);

            return dbPagedResult;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(SearchCustomEntitySummariesQuery query)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, query.CustomEntityDefinitionCode);

            yield return new CustomEntityReadPermission(definition);
        }

        #endregion
    }
}
