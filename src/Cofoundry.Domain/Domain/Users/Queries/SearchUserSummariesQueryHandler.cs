using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using AutoMapper.QueryableExtensions;

namespace Cofoundry.Domain
{
    public class SearchUserSummariesQueryHandler 
        : IAsyncQueryHandler<SearchUserSummariesQuery, PagedQueryResult<UserSummary>>
        , IPermissionRestrictedQueryHandler<SearchUserSummariesQuery, PagedQueryResult<UserSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;

        public SearchUserSummariesQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region execution

        public async Task<PagedQueryResult<UserSummary>> ExecuteAsync(SearchUserSummariesQuery query, IExecutionContext executionContext)
        {
            var result = await CreateQuery(query).ToPagedResultAsync(query);

            return result;
        }

        private IQueryable<UserSummary> CreateQuery(SearchUserSummariesQuery query)
        {
            var dbQuery = _dbContext
                .Users
                .AsNoTracking()
                .Where(p => !p.IsDeleted && p.UserAreaCode == query.UserAreaCode);

            if (!string.IsNullOrEmpty(query.Email))
            {
                dbQuery = dbQuery.Where(u => u.Email.Contains(query.Email));
            }

            // Filter by name
            if (!string.IsNullOrEmpty(query.Name))
            {
                var names = query.Name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string name in names)
                {
                    // See http://stackoverflow.com/a/7288269/486434 for why this is copied into a new variable
                    string localName = name;

                    dbQuery = dbQuery.Where(u => u.FirstName.Contains(localName) || u.LastName.Contains(localName));
                }

                // Order by exact matches first
                dbQuery = dbQuery
                    .OrderByDescending(u => names.Contains(u.FirstName) && names.Contains(u.LastName))
                    .ThenByDescending(u => names.Contains(u.FirstName) || names.Contains(u.LastName));
            }
            else
            {
                dbQuery = dbQuery.OrderBy(u => u.LastName);
            }

            return dbQuery.ProjectTo<UserSummary>();
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(SearchUserSummariesQuery query)
        {
            if (query.UserAreaCode == CofoundryAdminUserArea.AreaCode)
            {
                yield return new CofoundryUserReadPermission();
            }
            else
            {
                yield return new NonCofoundryUserReadPermission();
            }
        }

        #endregion
    }
}
