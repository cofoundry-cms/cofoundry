using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class AddMasterCofoundryUserCommandHandler
        : IAsyncCommandHandler<AddMasterCofoundryUserCommand>
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
            var settings = _queryExecutor.Get<InternalSettings>();
            if (settings.IsSetup)
            {
                throw new ValidationException("Site is already set up.");
            }

            if (await _dbContext
                .Users
                .FilterCanLogIn()
                .AnyAsync(u => u.Role.SpecialistRoleTypeCode == SpecialistRoleTypeCodes.SuperAdministrator))
            {
                throw new ValidationException("Cannot create a master user when master users already exist in the database.");
            }

            var role = await _dbContext
                .Roles
                .SingleOrDefaultAsync(r => r.SpecialistRoleTypeCode == SpecialistRoleTypeCodes.SuperAdministrator);
            EntityNotFoundException.ThrowIfNull(role, SpecialistRoleTypeCodes.SuperAdministrator);

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
            user.RequirePasswordChange = false;
            user.LastPasswordChangeDate = executionContext.ExecutionDate;
            user.CreateDate = executionContext.ExecutionDate;
            user.Role = superUserRole;

            var hashResult = _passwordCryptographyService.CreateHash(command.Password);
            user.Password = hashResult.Hash;
            user.PasswordEncryptionVersion = (int)hashResult.EncryptionVersion;

            user.UserArea = userArea;
            EntityNotFoundException.ThrowIfNull(user.UserArea, CofoundryAdminUserArea.AreaCode);

            _dbContext.Users.Add(user);

            return user;
        }

        #endregion
    }
}
