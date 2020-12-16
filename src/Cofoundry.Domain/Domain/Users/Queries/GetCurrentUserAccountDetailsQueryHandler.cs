using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Gets a UserAccountDetails object representing the currently logged in 
    /// user. If the user is not logged in then null is returned.
    /// </summary>
    public class GetCurrentUserAccountDetailsQueryHandler 
        : IQueryHandler<GetCurrentUserAccountDetailsQuery, UserAccountDetails>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IUserAccountDetailsMapper _userAccountDetailsMapper;

        public GetCurrentUserAccountDetailsQueryHandler(
            CofoundryDbContext dbContext,
            IUserAccountDetailsMapper userAccountDetailsMapper
            )
        {
            _dbContext = dbContext;
            _userAccountDetailsMapper = userAccountDetailsMapper;
        }

        public async Task<UserAccountDetails> ExecuteAsync(GetCurrentUserAccountDetailsQuery query, IExecutionContext executionContext)
        {
            if (!executionContext.UserContext.UserId.HasValue) return null;

            var dbResult = await _dbContext
                .Users
                .AsNoTracking()
                .Include(u => u.Creator)
                .FilterById(executionContext.UserContext.UserId.Value)
                .FilterCanLogIn()
                .SingleOrDefaultAsync();

            var user = _userAccountDetailsMapper.Map(dbResult);

            return user;
        }
    }
}
