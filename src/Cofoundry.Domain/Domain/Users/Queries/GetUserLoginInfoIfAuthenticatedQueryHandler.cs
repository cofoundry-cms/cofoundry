using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetUserLoginInfoIfAuthenticatedQueryHandler 
        : IQueryHandler<GetUserLoginInfoIfAuthenticatedQuery, UserLoginInfo>
        , IIgnorePermissionCheckHandler
    {
        private readonly UserAuthenticationHelper _userAuthenticationHelper;
        private readonly CofoundryDbContext _dbContext;

        public GetUserLoginInfoIfAuthenticatedQueryHandler(
            CofoundryDbContext dbContext,
            UserAuthenticationHelper userAuthenticationHelper
            )
        {
            _userAuthenticationHelper = userAuthenticationHelper;
            _dbContext = dbContext;
        }

        public UserLoginInfo Execute(GetUserLoginInfoIfAuthenticatedQuery query, IExecutionContext executionContext)
        {
            if (string.IsNullOrWhiteSpace(query.Username) || string.IsNullOrWhiteSpace(query.Password)) return null;

            var user = _dbContext
                .Users
                .AsNoTracking()
                .FirstOrDefault(u => u.Username == query.Username && !u.IsDeleted);
            
            if (_userAuthenticationHelper.IsPasswordCorrect(user, query.Password))
            {
                var result = Mapper.Map<UserLoginInfo>(user);
                return result;
            }

            return null;
        }
    }
}
