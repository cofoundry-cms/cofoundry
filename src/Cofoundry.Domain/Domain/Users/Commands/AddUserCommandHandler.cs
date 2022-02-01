using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System;
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
        private readonly IPasswordPolicyService _newPasswordValidationService;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ISecurityStampGenerator _securityStampGenerator;

        public AddUserCommandHandler(
            CofoundryDbContext dbContext,
            IPasswordCryptographyService passwordCryptographyService,
            UserCommandPermissionsHelper userCommandPermissionsHelper,
            IUserAreaDefinitionRepository userAreaRepository,
            IUserUpdateCommandHelper userUpdateCommandHelper,
            IPasswordPolicyService newPasswordValidationService,
            ITransactionScopeManager transactionScopeFactory,
            IMessageAggregator messageAggregator,
            ISecurityStampGenerator securityStampGenerator
            )
        {
            _dbContext = dbContext;
            _passwordCryptographyService = passwordCryptographyService;
            _userCommandPermissionsHelper = userCommandPermissionsHelper;
            _userAreaRepository = userAreaRepository;
            _userUpdateCommandHelper = userUpdateCommandHelper;
            _newPasswordValidationService = newPasswordValidationService;
            _transactionScopeFactory = transactionScopeFactory;
            _messageAggregator = messageAggregator;
            _securityStampGenerator = securityStampGenerator;
        }

        public async Task ExecuteAsync(AddUserCommand command, IExecutionContext executionContext)
        {
            var userArea = _userAreaRepository.GetRequiredByCode(command.UserAreaCode);
            var dbUserArea = await GetUserAreaAsync(userArea);
            var role = await GetAndValidateRoleAsync(command, executionContext);

            var user = new User()
            {
                FirstName = command.FirstName?.Trim(),
                LastName = command.LastName?.Trim(),
                RequirePasswordChange = command.RequirePasswordChange,
                LastPasswordChangeDate = executionContext.ExecutionDate,
                AccountVerifiedDate = command.IsAccountVerified ? executionContext.ExecutionDate : (DateTime?)null,
                CreateDate = executionContext.ExecutionDate,
                Role = role,
                UserArea = dbUserArea,
                CreatorId = executionContext.UserContext.UserId,
                SecurityStamp = _securityStampGenerator.Generate()
            };

            await _userUpdateCommandHelper.UpdateEmailAndUsernameAsync(command.Email, command.Username, user, executionContext);
            await ValidatePasswordAsync(userArea, user, command, executionContext);
            SetPassword(user, command, userArea);

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            await _transactionScopeFactory.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(userArea, user));

            command.OutputUserId = user.UserId;
        }

        private async Task OnTransactionComplete(IUserAreaDefinition userArea, User user)
        {
            await _messageAggregator.PublishAsync(new UserAddedMessage()
            {
                UserAreaCode = userArea.UserAreaCode,
                UserId = user.UserId
            });
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

        private async Task ValidatePasswordAsync(
            IUserAreaDefinition userArea, 
            User user, 
            AddUserCommand command,
            IExecutionContext executionContext
            )
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
            else if (!userArea.AllowPasswordLogin)
            {
                return;
            }

            var context = NewPasswordValidationContext.MapFromUser(user);
            context.Password = command.Password;
            context.PropertyName = nameof(command.Password);
            context.ExecutionContext = executionContext;

            await _newPasswordValidationService.ValidateAsync(context);
        }

        private void SetPassword(User user, AddUserCommand command, IUserAreaDefinition userArea)
        {
            if (userArea.AllowPasswordLogin)
            {
                var hashResult = _passwordCryptographyService.CreateHash(command.Password);
                user.Password = hashResult.Hash;
                user.PasswordHashVersion = hashResult.HashVersion;
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
            if (command.UserAreaCode == CofoundryAdminUserArea.Code)
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
