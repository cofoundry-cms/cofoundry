using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Determines if an email address is unique within a user area. Email
    /// addresses must be unique per user area and can therefore appear in multiple
    /// user areas.
    /// </summary>
    public class IsUserEmailAddressUniqueQueryHandler
        : IQueryHandler<IsUserEmailAddressUniqueQuery, bool>
        , IPermissionRestrictedQueryHandler<IsUserEmailAddressUniqueQuery, bool>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IUserDataFormatter _userDataFormatter;

        public IsUserEmailAddressUniqueQueryHandler(
            CofoundryDbContext dbContext,
            IUserDataFormatter userDataFormatter
            )
        {
            _dbContext = dbContext;
            _userDataFormatter = userDataFormatter;
        }

        public async Task<bool> ExecuteAsync(IsUserEmailAddressUniqueQuery query, IExecutionContext executionContext)
        {
            var uniqueEmailAddress = _userDataFormatter.UniquifyEmail(query.UserAreaCode, query.Email);
            if (string.IsNullOrWhiteSpace(uniqueEmailAddress)) return true;

            var exists = await _dbContext
                .Users
                .AsNoTracking()
                .FilterActive()
                .FilterByUserArea(query.UserAreaCode)
                .Where(u => u.UserId != query.UserId && u.UniqueEmail == uniqueEmailAddress)
                .AnyAsync();

            return !exists;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(IsUserEmailAddressUniqueQuery query)
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
