using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    public class ResetUserPasswordByUserIdCommandHandler 
        : ICommandHandler<ResetUserPasswordByUserIdCommand>
        , IIgnorePermissionCheckHandler
    {
        #region construstor

        private readonly CofoundryDbContext _dbContext;
        private readonly IResetUserPasswordCommandHelper _resetUserPasswordCommandHelper;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        
        public ResetUserPasswordByUserIdCommandHandler(
            CofoundryDbContext dbContext,
            IResetUserPasswordCommandHelper resetUserPasswordCommandHelper,
            IPermissionValidationService permissionValidationService,
            IUserAreaDefinitionRepository userAreaRepository
            )
        {
            _dbContext = dbContext;
            _resetUserPasswordCommandHelper = resetUserPasswordCommandHelper; 
            _permissionValidationService = permissionValidationService;
            _userAreaRepository = userAreaRepository;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(ResetUserPasswordByUserIdCommand command, IExecutionContext executionContext)
        {
            await _resetUserPasswordCommandHelper.ValidateCommandAsync(command, executionContext);
            var user = await QueryUser(command, executionContext).SingleOrDefaultAsync();
            ValidatePermissions(user, executionContext);
            await _resetUserPasswordCommandHelper.ResetPasswordAsync(user, command, executionContext);
        }

        #endregion

        #region private helpers

        private IQueryable<User> QueryUser(ResetUserPasswordByUserIdCommand command, IExecutionContext executionContext)
        {
            var user = _dbContext
                .Users
                .FilterById(command.UserId)
                .FilterCanLogIn();

            return user;
        }

        #endregion

        #region Permission

        public void ValidatePermissions(User user, IExecutionContext executionContext)
        {
            var userArea = _userAreaRepository.GetByCode(user.UserAreaCode);
            if (userArea is CofoundryAdminUserArea)
            {
                _permissionValidationService.EnforcePermission(new CofoundryUserUpdatePermission(), executionContext.UserContext);
            }
            else
            {
                _permissionValidationService.EnforcePermission(new NonCofoundryUserUpdatePermission(), executionContext.UserContext);
            }
        }

        #endregion
    }
}
