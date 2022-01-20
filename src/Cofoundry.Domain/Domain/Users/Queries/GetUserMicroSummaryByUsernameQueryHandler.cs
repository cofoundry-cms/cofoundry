using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Finds a user with a specific username, returning <see langword="null"/> if the 
    /// user could not be found. A user always has a username, however it may just
    /// be a copy of the email address if the <see cref="IUserAreaDefinition.UseEmailAsUsername"/>
    /// setting is set to true.
    /// </summary>
    public class GetUserMicroSummaryByUsernameQueryHandler
        : IQueryHandler<GetUserMicroSummaryByUsernameQuery, UserMicroSummary>
        , IPermissionRestrictedQueryHandler<GetUserMicroSummaryByUsernameQuery, UserMicroSummary>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IUserMicroSummaryMapper _userMicroSummaryMapper;
        private readonly IUserDataFormatter _userDataFormatter;

        public GetUserMicroSummaryByUsernameQueryHandler(
            CofoundryDbContext dbContext,
            IUserMicroSummaryMapper userMicroSummaryMapper,
            IUserDataFormatter userDataFormatter
            )
        {
            _dbContext = dbContext;
            _userMicroSummaryMapper = userMicroSummaryMapper;
            _userDataFormatter = userDataFormatter;
        }

        public async Task<UserMicroSummary> ExecuteAsync(GetUserMicroSummaryByUsernameQuery query, IExecutionContext executionContext)
        {
            if (string.IsNullOrWhiteSpace(query.Username)) return null;

            var uniqueUsername = _userDataFormatter.UniquifyUsername(query.UserAreaCode, query.Username);
            if (uniqueUsername == null) return null;

            var dbResult = await _dbContext
                .Users
                .AsNoTracking()
                .FilterByUserArea(query.UserAreaCode)
                .Where(u => u.UniqueUsername == uniqueUsername)
                .SingleOrDefaultAsync();

            var user = _userMicroSummaryMapper.Map(dbResult);

            return user;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(GetUserMicroSummaryByUsernameQuery query)
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
