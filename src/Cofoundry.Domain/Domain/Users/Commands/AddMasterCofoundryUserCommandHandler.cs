using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Creates the initial super admin account for the site as part
    /// of the setup process. This cannot be run once the site is set up
    /// because by design it has to forgo permission checks.
    /// </summary>
    public class AddMasterCofoundryUserCommandHandler
        : ICommandHandler<AddMasterCofoundryUserCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPasswordCryptographyService _passwordCryptographyService;
        private readonly IEmailAddressNormalizer _emailAddressNormalizer;

        public AddMasterCofoundryUserCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPasswordCryptographyService passwordCryptographyService,
            IEmailAddressNormalizer emailAddressNormalizer
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _passwordCryptographyService = passwordCryptographyService;
            _emailAddressNormalizer = emailAddressNormalizer;
        }

        public async Task ExecuteAsync(AddMasterCofoundryUserCommand command, IExecutionContext executionContext)
        {
            await ValidateIsNotSetupAsync(executionContext);

            var role = await _dbContext
                .Roles
                .SingleOrDefaultAsync(r => r.RoleCode == SuperAdminRole.SuperAdminRoleCode);
            EntityNotFoundException.ThrowIfNull(role, SuperAdminRole.SuperAdminRoleCode);

            var userArea = await _dbContext
                .UserAreas
                .SingleOrDefaultAsync(a => a.UserAreaCode == CofoundryAdminUserArea.AreaCode);
            EntityNotFoundException.ThrowIfNull(userArea, CofoundryAdminUserArea.AreaCode);

            Normalize(command);
            var user = MapUser(command, executionContext, role, userArea);

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            command.OutputUserId = user.UserId;
        }

        private async Task ValidateIsNotSetupAsync(IExecutionContext executionContext)
        {
            var settings = await _queryExecutor.ExecuteAsync(new GetSettingsQuery<InternalSettings>(), executionContext);

            if (settings.IsSetup)
            {
                throw new ValidationException("Site is already set up.");
            }

            if (await _dbContext
                .Users
                .FilterCanLogIn()
                .AnyAsync(u => u.Role.RoleCode == SuperAdminRole.SuperAdminRoleCode))
            {
                throw new ValidationException("Cannot create a master user when master users already exist in the database.");
            }
        }

        private void Normalize(AddMasterCofoundryUserCommand command)
        {
            command.FirstName = command?.FirstName.Trim();
            command.LastName = command?.LastName.Trim();
            command.Email = _emailAddressNormalizer.Normalize(command.Email);
        }

        private User MapUser(AddMasterCofoundryUserCommand command, IExecutionContext executionContext, Role superUserRole, UserArea userArea)
        {
            var user = new User();
            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.Username = command.Email;
            user.Email = command.Email;
            user.RequirePasswordChange = command.RequirePasswordChange;
            user.LastPasswordChangeDate = executionContext.ExecutionDate;
            user.CreateDate = executionContext.ExecutionDate;
            user.Role = superUserRole;

            var hashResult = _passwordCryptographyService.CreateHash(command.Password);
            user.Password = hashResult.Hash;
            user.PasswordHashVersion = hashResult.HashVersion;
            user.UserArea = userArea;

            return user;
        }
    }
}
