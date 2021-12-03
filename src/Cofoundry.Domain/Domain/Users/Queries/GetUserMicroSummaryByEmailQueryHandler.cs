using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Finds a user with a specific email address returning <see langword="null"/> 
    /// if the user could not be found. Note that if the user area does not use email 
    /// addresses as the username then the email field is optional and may be empty.
    /// </summary>
    public class GetUserMicroSummaryByEmailQueryHandler
        : IQueryHandler<GetUserMicroSummaryByEmailQuery, UserMicroSummary>
        , IPermissionRestrictedQueryHandler<GetUserMicroSummaryByEmailQuery, UserMicroSummary>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IUserMicroSummaryMapper _userMicroSummaryMapper;
        private readonly IUserDataFormatter _userDataFormatter;

        public GetUserMicroSummaryByEmailQueryHandler(
            CofoundryDbContext dbContext,
            IUserMicroSummaryMapper userMicroSummaryMapper,
            IUserDataFormatter userDataFormatter
            )
        {
            _dbContext = dbContext;
            _userMicroSummaryMapper = userMicroSummaryMapper;
            _userDataFormatter = userDataFormatter;
        }

        public async Task<UserMicroSummary> ExecuteAsync(GetUserMicroSummaryByEmailQuery query, IExecutionContext executionContext)
        {
            if (string.IsNullOrWhiteSpace(query.Email)) return null;

            var email = _userDataFormatter.NormalizeEmail(query.UserAreaCode, query.Email);
            if (email == null) return null;

            var dbResult = await _dbContext
                .Users
                .AsNoTracking()
                .FilterByUserArea(query.UserAreaCode)
                .Where(u => u.Email == email)
                .SingleOrDefaultAsync();

            var user = _userMicroSummaryMapper.Map(dbResult);
            
            return user;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(GetUserMicroSummaryByEmailQuery query)
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
    }
}
