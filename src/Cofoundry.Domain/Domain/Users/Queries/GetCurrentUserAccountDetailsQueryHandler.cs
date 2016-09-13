using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using AutoMapper.QueryableExtensions;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCurrentUserAccountDetailsQueryHandler 
        : IAsyncQueryHandler<GetCurrentUserAccountDetailsQuery, UserAccountDetails>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;

        public GetCurrentUserAccountDetailsQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        public Task<UserAccountDetails> ExecuteAsync(GetCurrentUserAccountDetailsQuery query, IExecutionContext executionContext)
        {
            if (!executionContext.UserContext.UserId.HasValue) return null;

            var user = _dbContext
                .Users
                .FilterById(executionContext.UserContext.UserId.Value)
                .ProjectTo<UserAccountDetails>()
                .SingleOrDefaultAsync();

            return user;
        }
    }
}
