using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.Validation;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    public class UpdateUserCommandHandler
        : ICommandHandler<UpdateUserCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly UserCommandPermissionsHelper _userCommandPermissionsHelper;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly IPermissionValidationService _permissionValidationService;

        public UpdateUserCommandHandler(
            IQueryExecutor queryExecutor,
            CofoundryDbContext dbContext,
            UserCommandPermissionsHelper userCommandPermissionsHelper,
            IUserAreaDefinitionRepository userAreaRepository,
            IPermissionValidationService permissionValidationService
            )
        {
            _queryExecutor = queryExecutor;
            _dbContext = dbContext;
            _userCommandPermissionsHelper = userCommandPermissionsHelper;
            _userAreaRepository = userAreaRepository;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region Execution

        public async Task ExecuteAsync(UpdateUserCommand command, IExecutionContext executionContext)
        {
            // Get User
            var user = await _dbContext
                .Users
                .FilterCanLogIn()
                .FilterById(command.UserId)
                .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(user, command.UserId);

            // Validate
            var userArea = _userAreaRepository.GetByCode(user.UserAreaCode);
            ValidatePermissions(userArea, executionContext);
            ValidateCommand(command, userArea);
            await ValidateIsUniqueAsync(command, userArea, executionContext);

            // Role
            if (command.RoleId != user.RoleId)
            {
                user.Role = await _userCommandPermissionsHelper.GetAndValidateNewRoleAsync(
                    command.RoleId,
                    user.RoleId,
                    user.UserAreaCode, 
                    executionContext
                    );
            }
            
            // Map
            Map(command, user, userArea);

            // Save
            await _dbContext.SaveChangesAsync();
        }

        private static void Map(UpdateUserCommand command, User user, IUserAreaDefinition userArea)
        {
            user.FirstName = command.FirstName?.Trim();
            user.LastName = command.LastName?.Trim();
            user.Email = command.Email?.Trim();

            if (userArea.UseEmailAsUsername)
            {
                user.Username = command.Email;
            }
            else
            {
                user.Username = command.Username?.Trim();
            }

            user.RequirePasswordChange = command.RequirePasswordChange;
        }

        private void ValidateCommand(UpdateUserCommand command, IUserAreaDefinition userArea)
        {
            // Email
            if (userArea.UseEmailAsUsername && string.IsNullOrEmpty(command.Email))
            {
                throw ValidationErrorException.CreateWithProperties("Email field is required.", "Email");
            }

            // Username
            if (!userArea.UseEmailAsUsername && string.IsNullOrWhiteSpace(command.Username))
            {
                throw ValidationErrorException.CreateWithProperties("Username field is required", "Username");
            }
        }

        private async Task ValidateIsUniqueAsync(
            UpdateUserCommand command, 
            IUserAreaDefinition userArea,
            IExecutionContext executionContext
            )
        {
            var query = new IsUsernameUniqueQuery()
            {
                UserId = command.UserId,
                UserAreaCode = userArea.UserAreaCode
            };

            if (userArea.UseEmailAsUsername)
            {
                query.Username = command.Email?.Trim();
            }
            else
            {
                query.Username = command.Username.Trim();
            }

            var isUnique = await _queryExecutor.ExecuteAsync(query, executionContext);

            if (!isUnique)
            {
                if (userArea.UseEmailAsUsername)
                {
                    throw ValidationErrorException.CreateWithProperties("This email is already registered", "Email");
                }
                else
                {
                    throw ValidationErrorException.CreateWithProperties("This username is already registered", "Username");
                }
            }
        }

        #endregion
        
        #region Permission

        public void ValidatePermissions(IUserAreaDefinition userArea, IExecutionContext executionContext)
        {
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
