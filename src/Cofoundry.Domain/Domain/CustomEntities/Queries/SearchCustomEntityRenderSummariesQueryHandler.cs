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
        private readonly CustomEntityDataModelMapper _customEntityDataModelMapper;
        private readonly ICustomEntityCodeDefinitionRepository _customEntityDefinitionRepository;

        public SearchCustomEntityRenderSummariesQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            CustomEntityDataModelMapper customEntityDataModelMapper,
            ICustomEntityCodeDefinitionRepository customEntityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _customEntityDataModelMapper = customEntityDataModelMapper;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        #endregion

        #region execution

        public PagedQueryResult<CustomEntityRenderSummary> Execute(SearchCustomEntityRenderSummariesQuery query, IExecutionContext executionContext)
        {
            var dbQuery = GetQuery(query);
            var dbPagedResult = dbQuery.ToPagedResult(query);
            var results = Map(dbPagedResult);

            return results;
        }

        public async Task<PagedQueryResult<CustomEntityRenderSummary>> ExecuteAsync(SearchCustomEntityRenderSummariesQuery query, IExecutionContext executionContext)
        {
            var dbQuery = GetQuery(query);
            var dbPagedResult = await dbQuery.ToPagedResultAsync(query);
            var results = Map(dbPagedResult);

            return results;
        }

        private PagedQueryResult<CustomEntityRenderSummary> Map(PagedQueryResult<CustomEntitySummaryQueryModel> dbPagedResult)
        {
            // Map Items
            var entities = new List<CustomEntityRenderSummary>(dbPagedResult.Items.Length);

            foreach (var dbVersion in dbPagedResult.Items)
            {
                var entity = Mapper.Map<CustomEntityRenderSummary>(dbVersion);
                entity.Model = _customEntityDataModelMapper.Map(dbVersion.CustomEntityDefinitionCode, dbVersion.SerializedData);
                entities.Add(entity);
            }

            // Change Result
            return dbPagedResult.ChangeType(entities);
        }

        private IQueryable<CustomEntitySummaryQueryModel> GetQuery(SearchCustomEntityRenderSummariesQuery query)
        {
            var definition = _queryExecutor.GetById<CustomEntityDefinitionSummary>(query.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, query.CustomEntityDefinitionCode);

            var dbQuery = _dbContext
                .CustomEntityVersions
                .AsNoTracking()
                .Where(e => e.CustomEntity.CustomEntityDefinitionCode == query.CustomEntityDefinitionCode)
                .Where(v => v.WorkFlowStatusId == (int)Domain.WorkFlowStatus.Draft || v.WorkFlowStatusId == (int)Domain.WorkFlowStatus.Published)
                .GroupBy(e => e.CustomEntityId, (key, g) => g.OrderByDescending(v => v.WorkFlowStatusId == (int)Domain.WorkFlowStatus.Draft).FirstOrDefault());

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

            return dbQuery.ProjectTo<CustomEntitySummaryQueryModel>();
        }

        private class CustomEntitySummaryQueryModel
        {
            public int CustomEntityId { get; set; }

            public string CustomEntityDefinitionCode { get; set; }

            public string Title { get; set; }

            public string UrlSlug { get; set; }

            /// <summary>
            /// The full path of the entity including directories and the locale. 
            /// </summary>
            public string FullPath { get; set; }

            /// <summary>
            /// Indicates if the entity has at least one published version and is currently
            /// viewable in the live site.
            /// </summary>
            public bool IsPublished { get; set; }

            /// <summary>
            /// Indicates whether there is a draft version of this entity available.
            /// </summary>
            public bool HasDraft { get; set; }

            public int? LocaleId { get; set; }

            public int? Ordering { get; set; }

            public string SerializedData { get; set; }

            public CreateAuditData AuditData { get; set; }

            public CreateAuditData VersionAuditData { get; set; }
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
