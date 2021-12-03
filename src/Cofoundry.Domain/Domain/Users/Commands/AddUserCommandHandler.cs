using Cofoundry.Core;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// A basic user creation command that adds data only and does not 
    /// send any email notifications.
    /// </summary>
    public class AddUserCommandHandler
        : ICommandHandler<AddUserCommand>
        , IPermissionRestrictedCommandHandler<AddUserCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IPasswordCryptographyService _passwordCryptographyService;
        private readonly UserCommandPermissionsHelper _userCommandPermissionsHelper;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly IUserUpdateCommandHelper _userUpdateCommandHelper;

        public AddUserCommandHandler(
            CofoundryDbContext dbContext,
            IPasswordCryptographyService passwordCryptographyService,
            UserCommandPermissionsHelper userCommandPermissionsHelper,
            IUserAreaDefinitionRepository userAreaRepository,
            IUserUpdateCommandHelper userUpdateCommandHelper
            )
        {
            _dbContext = dbContext;
            _passwordCryptographyService = passwordCryptographyService;
            _userCommandPermissionsHelper = userCommandPermissionsHelper;
            _userAreaRepository = userAreaRepository;
            _userUpdateCommandHelper = userUpdateCommandHelper;
        }

        public async Task ExecuteAsync(AddUserCommand command, IExecutionContext executionContext)
        {
            var userArea = _userAreaRepository.GetRequiredByCode(command.UserAreaCode);
            var dbUserArea = await GetUserAreaAsync(userArea);
            ValidatePassword(userArea, command);
            var role = await GetAndValidateRoleAsync(command, executionContext);

            var user = new User()
            {
                FirstName = command.FirstName?.Trim(),
                LastName = command.LastName?.Trim(),
                RequirePasswordChange = command.RequirePasswordChange,
                LastPasswordChangeDate = executionContext.ExecutionDate,
                CreateDate = executionContext.ExecutionDate,
                Role = role,
                UserArea = dbUserArea,
                CreatorId = executionContext.UserContext.UserId,
            };

            await _userUpdateCommandHelper.UpdateEmailAndUsernameAsync(command.Email, command.Username, user, executionContext);

            if (userArea.AllowPasswordLogin)
            {
                var hashResult = _passwordCryptographyService.CreateHash(command.Password);
                user.Password = hashResult.Hash;
                user.PasswordHashVersion = hashResult.HashVersion;
            }

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            command.OutputUserId = user.UserId;
        }

        private async Task<UserArea> GetUserAreaAsync(IUserAreaDefinition userArea)
        {
            var dbUserArea = await _dbContext
                .UserAreas
                .Where(a => a.UserAreaCode == userArea.UserAreaCode)
                .SingleOrDefaultAsync();

            if (dbUserArea == null)
            {
                dbUserArea = new UserArea();
                dbUserArea.UserAreaCode = userArea.UserAreaCode;
                dbUserArea.Name = userArea.Name;
                _dbContext.UserAreas.Add(dbUserArea);
            }

            return dbUserArea;
        }

        private void ValidatePassword(IUserAreaDefinition userArea, AddUserCommand command)
        {
            var isPasswordEmpty = string.IsNullOrWhiteSpace(command.Password);

            if (userArea.AllowPasswordLogin && isPasswordEmpty)
            {
                throw ValidationErrorException.CreateWithProperties("Password field is required", nameof(command.Password));
            }
            else if (!userArea.AllowPasswordLogin && !isPasswordEmpty)
            {
                throw ValidationErrorException.CreateWithProperties("Password field should be empty because the specified user area does not use passwords", nameof(command.Password));
            }
        }

        private async Task<Role> GetAndValidateRoleAsync(AddUserCommand command, IExecutionContext executionContext)
        {
            var role = await _dbContext
                  .Roles
                  .FilterByIdOrCode(command.RoleId, command.RoleCode)
                  .SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(role, command.RoleId?.ToString() ?? command.RoleCode);

            await _userCommandPermissionsHelper.ValidateNewRoleAsync(role, null, command.UserAreaCode, executionContext);

            return role;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(AddUserCommand command)
        {
            if (command.UserAreaCode == CofoundryAdminUserArea.AreaCode)
            {
                yield return new CofoundryUserCreatePermission();
            }
            else
            {
                yield return new NonCofoundryUserCreatePermission();
            }
        }
    }
}
