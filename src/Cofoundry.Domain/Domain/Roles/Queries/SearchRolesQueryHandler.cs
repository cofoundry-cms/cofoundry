using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using AutoMapper.QueryableExtensions;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Seaches roles based on simple filter criteria and returns a paged result. 
    /// </summary>
    public class SearchRolesQueryHandler 
        : IAsyncQueryHandler<SearchRolesQuery, PagedQueryResult<RoleMicroSummary>>
        , IPermissionRestrictedQueryHandler<SearchRolesQuery, PagedQueryResult<RoleMicroSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;

        public SearchRolesQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region execution

        public async Task<PagedQueryResult<RoleMicroSummary>> ExecuteAsync(SearchRolesQuery query, IExecutionContext executionContext)
        {
            CheckRolePermission(query, executionContext);
            var result = await CreateQuery(query).ToPagedResultAsync(query);

            return result;
        }

        #endregion

        #region helpers

        private IQueryable<RoleMicroSummary> CreateQuery(SearchRolesQuery query)
        {
            var dbQuery = _dbContext
                .Roles
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(query.UserAreaCode))
            {
                dbQuery = dbQuery
                    .Where(r => r.UserAreaCode == query.UserAreaCode);
            }

            if (query.ExcludeAnonymous)
            {
                dbQuery = dbQuery
                    .Where(r => r.SpecialistRoleTypeCode != SpecialistRoleTypeCodes.Anonymous);
            }

            if (!string.IsNullOrEmpty(query.Text))
            {
                var text = query.Text.Trim();
                dbQuery = dbQuery.Where(r => r.Title.Contains(text))
                    .OrderByDescending(r => r.Title.StartsWith(text))
                    .ThenByDescending(r => r.Title);
            }
            else
            {
                dbQuery = dbQuery
                    .OrderBy(r => r.UserArea.Name)
                    .ThenBy(r => r.Title);
            }

            return dbQuery
                .ProjectTo<RoleMicroSummary>();
        }

        private void CheckRolePermission(SearchRolesQuery query, IExecutionContext executionContext)
        {

        }


        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(SearchRolesQuery command)
        {
            yield return new RoleReadPermission();
        }

        #endregion
    }
}
