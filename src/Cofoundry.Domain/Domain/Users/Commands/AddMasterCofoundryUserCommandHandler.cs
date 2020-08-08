using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

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
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPasswordCryptographyService _passwordCryptographyService;

        public AddMasterCofoundryUserCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPasswordCryptographyService passwordCryptographyService
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _passwordCryptographyService = passwordCryptographyService;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(AddMasterCofoundryUserCommand command, IExecutionContext executionContext)
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

            var role = await _dbContext
                .Roles
                .SingleOrDefaultAsync(r => r.RoleCode == SuperAdminRole.SuperAdminRoleCode);
            EntityNotFoundException.ThrowIfNull(role, SuperAdminRole.SuperAdminRoleCode);

            var userArea = await _dbContext
                .UserAreas
                .SingleOrDefaultAsync(a => a.UserAreaCode == CofoundryAdminUserArea.AreaCode);

            var user = MapAndAddUser(command, executionContext, role, userArea);

            await _dbContext.SaveChangesAsync();

            command.OutputUserId = user.UserId;
        }

        #endregion

        #region private helpers

        private User MapAndAddUser(AddMasterCofoundryUserCommand command, IExecutionContext executionContext, Role superUserRole, UserArea userArea)
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
            EntityNotFoundException.ThrowIfNull(user.UserArea, CofoundryAdminUserArea.AreaCode);

            _dbContext.Users.Add(user);

            return user;
        }

        #endregion
    }
}
