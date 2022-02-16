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
        private readonly CofoundryDbContext _dbContext;
        private readonly IUserSummaryMapper _userSummaryMapper;
        private readonly IUserDataFormatter _userDataFormatter;

        public SearchUserSummariesQueryHandler(
            CofoundryDbContext dbContext,
            IUserSummaryMapper userSummaryMapper,
            IUserDataFormatter userDataFormatter
            )
        {
            _dbContext = dbContext;
            _userSummaryMapper = userSummaryMapper;
            _userDataFormatter = userDataFormatter;
        }

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
                .FilterNotDeleted()
                .FilterNotSystemAccount()
                .Where(p => p.UserAreaCode == query.UserAreaCode);

            if (query.AccountStatus != UserAccountStatusFilter.Any)
            {
                var isActive = query.AccountStatus == UserAccountStatusFilter.Deactivated;
                dbQuery = dbQuery.Where(u => u.DeactivatedDate.HasValue != isActive);
            }

            if (!string.IsNullOrEmpty(query.Email))
            {
                var uniqueEmail = FormatEmailForSearch(query);
                dbQuery = dbQuery.Where(u => u.UniqueEmail.Contains(uniqueEmail));
            }

            if (!string.IsNullOrEmpty(query.Username))
            {
                var uniqueUsername = _userDataFormatter.UniquifyUsername(query.UserAreaCode, query.Username);
                dbQuery = dbQuery.Where(u => u.UniqueUsername.Contains(uniqueUsername));
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

        private string FormatEmailForSearch(SearchUserSummariesQuery query)
        {
            const string PLACEHOLDER_DOMAIN = "@example.com";

            var email = query.Email;
            var isPartialEmail = !email.Contains("@");

            if (isPartialEmail)
            {
                // we can only assume it's the local part of the email
                email = query.Email + PLACEHOLDER_DOMAIN;
            }

            var uniqueEmail = _userDataFormatter.UniquifyEmail(query.UserAreaCode, email);

            if (uniqueEmail == null)
            {
                // if it could not be parsed, just stick with the original
                uniqueEmail = query.Email;
            }
            else if (isPartialEmail)
            {
                uniqueEmail = email.Replace(PLACEHOLDER_DOMAIN, string.Empty);
            }

            return uniqueEmail;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(SearchUserSummariesQuery query)
        {
            if (query.UserAreaCode == CofoundryAdminUserArea.Code)
            {
                yield return new CofoundryUserReadPermission();
            }
            else
            {
                yield return new NonCofoundryUserReadPermission();
            }
        }
    }
}
