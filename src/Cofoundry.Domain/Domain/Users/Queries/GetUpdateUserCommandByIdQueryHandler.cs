using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    public class GetUpdateUserCommandByIdQueryHandler 
        : IQueryHandler<GetUpdateCommandByIdQuery<UpdateUserCommand>, UpdateUserCommand>
        , ILoggedInPermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetUpdateUserCommandByIdQueryHandler(
            CofoundryDbContext dbContext,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _permissionValidationService = permissionValidationService;
        }

        public async Task<UpdateUserCommand> ExecuteAsync(GetUpdateCommandByIdQuery<UpdateUserCommand> query, IExecutionContext executionContext)
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

            var user = new UpdateUserCommand()
            {
                Email = dbUser.Email,
                FirstName = dbUser.FirstName,
                IsEmailConfirmed = dbUser.IsEmailConfirmed,
                LastName = dbUser.LastName,
                RequirePasswordChange = dbUser.RequirePasswordChange,
                RoleId = dbUser.RoleId,
                UserId = dbUser.RoleId,
                Username = dbUser.Username
            };

            return user;
        }
    }
}
