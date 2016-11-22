using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;

namespace Cofoundry.Domain
{
    public class GetUpdateUserCommandByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<UpdateUserCommand>, UpdateUserCommand>
        , ILoggedInPermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetUpdateUserCommandByIdQueryHandler(
            CofoundryDbContext dbContext,
            IMapper mapper,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _permissionValidationService = permissionValidationService;
        }

        public async Task<UpdateUserCommand> ExecuteAsync(GetByIdQuery<UpdateUserCommand> query, IExecutionContext executionContext)
        {
            var dbUser = await _dbContext
                .Users
                .AsNoTracking()
                .FilterCanLogIn()
                .FilterById(query.Id)
                .SingleOrDefaultAsync();

            if (dbUser == null) return null;

            if (dbUser.UserAreaCode == CofoundryAdminUserArea.AreaCode)
            {
                _permissionValidationService.EnforceCurrentUserOrHasPermission<CofoundryUserReadPermission>(query.Id, executionContext.UserContext);
            }
            else
            {
                _permissionValidationService.EnforceCurrentUserOrHasPermission<NonCofoundryUserReadPermission>(query.Id, executionContext.UserContext);
            }

            var user = _mapper.Map<UpdateUserCommand>(dbUser);

            return user;
        }
    }
}
