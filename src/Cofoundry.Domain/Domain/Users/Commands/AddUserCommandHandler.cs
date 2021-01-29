using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;
using Cofoundry.Core.Mail;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// A generic user creation command for use with Cofoundry users and
    /// other non-Cofoundry users. Does not send any email notifications.
    /// </summary>
    public class AddUserCommandHandler 
        : ICommandHandler<AddUserCommand>
        , IPermissionRestrictedCommandHandler<AddUserCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPasswordCryptographyService _passwordCryptographyService;
        private readonly IMailService _mailService;
        private readonly UserCommandPermissionsHelper _userCommandPermissionsHelper;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        
        public AddUserCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPasswordCryptographyService passwordCryptographyService,
            IMailService mailService,
            UserCommandPermissionsHelper userCommandPermissionsHelper,
            IPermissionValidationService permissionValidationService,
            IUserAreaDefinitionRepository userAreaRepository
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _passwordCryptographyService = passwordCryptographyService;
            _mailService = mailService;
            _userCommandPermissionsHelper = userCommandPermissionsHelper;
            _permissionValidationService = permissionValidationService;
            _userAreaRepository = userAreaRepository;
        }

        #endregion

        public async Task ExecuteAsync(AddUserCommand command, IExecutionContext executionContext)
        {
            var userArea = _userAreaRepository.GetByCode(command.UserAreaCode);
            var dbUserArea = await QueryUserArea(userArea).SingleOrDefaultAsync();
            dbUserArea = AddUserAreaIfNotExists(userArea, dbUserArea);

            ValidateCommand(command, userArea);
            var isUnique = await _queryExecutor.ExecuteAsync(GetUniqueQuery(command, userArea), executionContext);
            ValidateIsUnique(isUnique, userArea);
            var newRole = await GetNewRole(command, executionContext);

            await _userCommandPermissionsHelper.ValidateNewRoleAsync(newRole, null, command.UserAreaCode, executionContext);

            var user = MapAndAddUser(command, executionContext, newRole, userArea, dbUserArea);
            await _dbContext.SaveChangesAsync();

            command.OutputUserId = user.UserId;
        }

        private async Task<Role> GetNewRole(AddUserCommand command, IExecutionContext executionContext)
        {
            Role role;

            if (command.RoleId.HasValue)
            {
                role = await _dbContext
                  .Roles
                  .FilterById(command.RoleId.Value)
                  .SingleOrDefaultAsync();

                EntityNotFoundException.ThrowIfNull(role, command.RoleId.Value);
            }
            else
            {
                role = await _dbContext
                  .Roles
                  .FilterByRoleCode(command.RoleCode)
                  .SingleOrDefaultAsync();

                EntityNotFoundException.ThrowIfNull(role, command.RoleCode);
            }

            await _userCommandPermissionsHelper.ValidateNewRoleAsync(role, null, command.UserAreaCode, executionContext);

            return role;
        }

        /// <summary>
        /// Perform some additional command validation that we can't do using data 
        /// annotations.
        /// </summary>
        private void ValidateCommand(AddUserCommand command, IUserAreaDefinition userArea)
        {
            // Password
            var isPasswordEmpty = string.IsNullOrWhiteSpace(command.Password);

            if (userArea.AllowPasswordLogin && isPasswordEmpty)
            {
                throw ValidationErrorException.CreateWithProperties("Password field is required", "Password");
            }
            else if (!userArea.AllowPasswordLogin && !isPasswordEmpty)
            {
                throw ValidationErrorException.CreateWithProperties("Password field should be empty because the specified user area does not use passwords", "Password");
            }

            // Email
            if (userArea.UseEmailAsUsername && string.IsNullOrEmpty(command.Email))
            {
                throw ValidationErrorException.CreateWithProperties("Email field is required.", "Email");
            }

            // Username
            if (userArea.UseEmailAsUsername && !string.IsNullOrEmpty(command.Username))
            {
                throw ValidationErrorException.CreateWithProperties("Username field should be empty becuase the specified user area uses the email as the username.", "Password");
            }
            else if (!userArea.UseEmailAsUsername && string.IsNullOrWhiteSpace(command.Username))
            {
                throw ValidationErrorException.CreateWithProperties("Username field is required", "Username");
            }
        }

        private IQueryable<UserArea> QueryUserArea(IUserAreaDefinition userArea)
        {
            return _dbContext
                .UserAreas
                .Where(a => a.UserAreaCode == userArea.UserAreaCode);
        }

        private UserArea AddUserAreaIfNotExists(IUserAreaDefinition userArea, UserArea dbUserArea)
        {
            if (dbUserArea == null)
            {
                dbUserArea = new UserArea();
                dbUserArea.UserAreaCode = userArea.UserAreaCode;
                dbUserArea.Name = userArea.Name;
                _dbContext.UserAreas.Add(dbUserArea);
            }

            return dbUserArea;
        }

        private User MapAndAddUser(AddUserCommand command, IExecutionContext executionContext, Role role, IUserAreaDefinition userArea, UserArea dbUserArea)
        {
            var user = new User();
            user.FirstName = command.FirstName?.Trim();
            user.LastName = command.LastName?.Trim();
            user.Email = command.Email?.Trim();
            user.RequirePasswordChange = command.RequirePasswordChange;
            user.LastPasswordChangeDate = executionContext.ExecutionDate;
            user.CreateDate = executionContext.ExecutionDate;
            user.Role = role;
            user.UserArea = dbUserArea;

            if (userArea.AllowPasswordLogin)
            {
                var hashResult = _passwordCryptographyService.CreateHash(command.Password);
                user.Password = hashResult.Hash;
                user.PasswordHashVersion = hashResult.HashVersion;
            }

            if (userArea.UseEmailAsUsername)
            {
                user.Username = command.Email?.Trim();
            }
            else
            {
                user.Username = command.Username?.Trim();
            }

            _dbContext.Users.Add(user);

            return user;
        }

        private void ValidateIsUnique(bool isUnique, IUserAreaDefinition userArea)
        {
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

        private IsUsernameUniqueQuery GetUniqueQuery(AddUserCommand command, IUserAreaDefinition userArea)
        {
            var query = new IsUsernameUniqueQuery();

            if (userArea.UseEmailAsUsername)
            {
                query.Username = command.Email?.Trim();
            }
            else
            {
                query.Username = command.Username?.Trim();
            }
            query.UserAreaCode = command.UserAreaCode;

            return query;
        }

        #region Permission

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

        #endregion
    }
}
