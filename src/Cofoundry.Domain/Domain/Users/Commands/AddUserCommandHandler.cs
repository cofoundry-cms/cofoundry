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
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPasswordCryptographyService _passwordCryptographyService;
        private readonly UserCommandPermissionsHelper _userCommandPermissionsHelper;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly IEmailAddressNormalizer _emailAddressNormalizer;

        public AddUserCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPasswordCryptographyService passwordCryptographyService,
            UserCommandPermissionsHelper userCommandPermissionsHelper,
            IUserAreaDefinitionRepository userAreaRepository,
            IEmailAddressNormalizer emailAddressNormalizer
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _passwordCryptographyService = passwordCryptographyService;
            _userCommandPermissionsHelper = userCommandPermissionsHelper;
            _userAreaRepository = userAreaRepository;
            _emailAddressNormalizer = emailAddressNormalizer;
        }

        public async Task ExecuteAsync(AddUserCommand command, IExecutionContext executionContext)
        {
            Normalize(command);

            var userArea = _userAreaRepository.GetRequiredByCode(command.UserAreaCode);
            var dbUserArea = await GetUserAreaAsync(userArea);

            ValidateCommand(command, userArea);
            await ValidateIsUniqueAsync(command, userArea, executionContext);
            var newRole = await GetAndValidateRoleAsync(command, executionContext);

            await _userCommandPermissionsHelper.ValidateNewRoleAsync(newRole, null, command.UserAreaCode, executionContext);

            var user = MapAndAddUser(command, executionContext, newRole, userArea, dbUserArea);
            await _dbContext.SaveChangesAsync();

            command.OutputUserId = user.UserId;
        }

        private void Normalize(AddUserCommand command)
        {
            command.FirstName = command.FirstName?.Trim();
            command.LastName = command.LastName?.Trim();
            command.Username = command.Username?.Trim();
            command.Email = _emailAddressNormalizer.Normalize(command.Email);
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
                throw ValidationErrorException.CreateWithProperties("Password field is required", nameof(command.Password));
            }
            else if (!userArea.AllowPasswordLogin && !isPasswordEmpty)
            {
                throw ValidationErrorException.CreateWithProperties("Password field should be empty because the specified user area does not use passwords", nameof(command.Password));
            }

            // Email
            if (userArea.UseEmailAsUsername && string.IsNullOrEmpty(command.Email))
            {
                throw ValidationErrorException.CreateWithProperties("Email field is required.", "Email");
            }

            // Username
            if (userArea.UseEmailAsUsername && !string.IsNullOrEmpty(command.Username))
            {
                throw ValidationErrorException.CreateWithProperties("Username field should be empty becuase the specified user area uses the email as the username.", nameof(command.Username));
            }
            else if (!userArea.UseEmailAsUsername && string.IsNullOrWhiteSpace(command.Username))
            {
                throw ValidationErrorException.CreateWithProperties("Username field is required", nameof(command.Username));
            }
        }

        private async Task ValidateIsUniqueAsync(AddUserCommand command, IUserAreaDefinition userArea, IExecutionContext executionContext)
        {
            var query = new IsUsernameUniqueQuery()
            {
                Username = userArea.UseEmailAsUsername ? command.Email : command.Username,
                UserAreaCode = command.UserAreaCode
            };
            var isUnique = await _queryExecutor.ExecuteAsync(query, executionContext);

            if (isUnique) return;

            if (userArea.UseEmailAsUsername)
            {
                throw ValidationErrorException.CreateWithProperties("This email is already registered", "Email");
            }
            else
            {
                throw ValidationErrorException.CreateWithProperties("This username is already registered", "Username");
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

        private User MapAndAddUser(
            AddUserCommand command, 
            IExecutionContext executionContext, 
            Role role,
            IUserAreaDefinition userArea, 
            UserArea dbUserArea
            )
        {
            var user = new User()
            {
                Username = userArea.UseEmailAsUsername ? command.Email : command.Username,
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                RequirePasswordChange = command.RequirePasswordChange,
                LastPasswordChangeDate = executionContext.ExecutionDate,
                CreateDate = executionContext.ExecutionDate,
                Role = role,
                UserArea = dbUserArea,
                CreatorId = executionContext.UserContext.UserId,
            };

            if (userArea.AllowPasswordLogin)
            {
                var hashResult = _passwordCryptographyService.CreateHash(command.Password);
                user.Password = hashResult.Hash;
                user.PasswordHashVersion = hashResult.HashVersion;
            }

            _dbContext.Users.Add(user);

            return user;
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
