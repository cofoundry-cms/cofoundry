using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Seaches users based on simple filter criteria and returns a paged result. 
    /// </summary>
    public class SearchUserSummariesQueryHandler 
        : IQueryHandler<SearchUserSummariesQuery, PagedQueryResult<UserSummary>>
        , IPermissionRestrictedQueryHandler<SearchUserSummariesQuery, PagedQueryResult<UserSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IUserSummaryMapper _userSummaryMapper;

        public SearchUserSummariesQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IUserSummaryMapper userSummaryMapper
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _userSummaryMapper = userSummaryMapper;
        }

        #endregion

        #region execution

        public async Task<PagedQueryResult<UserSummary>> ExecuteAsync(SearchUserSummariesQuery query, IExecutionContext executionContext)
        {
            var dbPagedResult = await CreateQuery(query).ToPagedResultAsync(query);
            var mappedResult = dbPagedResult
                .Items
                .Select(_userSummaryMapper.Map);

            return dbPagedResult.ChangeType(mappedResult);
        }

        private IQueryable<User> CreateQuery(SearchUserSummariesQuery query)
        {
            var dbQuery = _dbContext
                .Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.Creator)
                .FilterCanLogIn()
                .Where(p => p.UserAreaCode == query.UserAreaCode);

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

            return dbQuery;
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
