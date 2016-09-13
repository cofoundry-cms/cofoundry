using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;
using System.Data.Entity;

namespace Cofoundry.Domain
{
    public class GetUpdateCurrentUserAccountCommandByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<UpdateCurrentUserAccountCommand>, UpdateCurrentUserAccountCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetUpdateCurrentUserAccountCommandByIdQueryHandler(
            CofoundryDbContext dbContext,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _permissionValidationService = permissionValidationService;
        }

        public async Task<UpdateCurrentUserAccountCommand> ExecuteAsync(GetByIdQuery<UpdateCurrentUserAccountCommand> query, IExecutionContext executionContext)
        {
            if (!executionContext.UserContext.UserId.HasValue) return null;

            var user = await _dbContext
                .Users
                .AsNoTracking()
                .FilterActive()
                .FilterById(executionContext.UserContext.UserId.Value)
                .ProjectTo<UpdateCurrentUserAccountCommand>()
                .SingleOrDefaultAsync();

            return user;
        }
    }
}
