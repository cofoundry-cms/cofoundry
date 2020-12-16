using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    public class UpdateUserPasswordByUserIdCommandHandler
        : ICommandHandler<UpdateUserPasswordByUserIdCommand>
        , IIgnorePermissionCheckHandler
    {
        #region construstor

        private readonly CofoundryDbContext _dbContext;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly IPasswordUpdateCommandHelper _passwordUpdateCommandHelper;

        public UpdateUserPasswordByUserIdCommandHandler(
            CofoundryDbContext dbContext,
            IUserAreaDefinitionRepository userAreaRepository,
            IPasswordUpdateCommandHelper passwordUpdateCommandHelper
            )
        {
            _dbContext = dbContext;
            _userAreaRepository = userAreaRepository;
            _passwordUpdateCommandHelper = passwordUpdateCommandHelper;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(UpdateUserPasswordByUserIdCommand command, IExecutionContext executionContext)
        {
            var user = await GetUser(command.UserId);
            EntityNotFoundException.ThrowIfNull(user, command.UserId);

            var userArea = _userAreaRepository.GetByCode(user.UserAreaCode);
            _passwordUpdateCommandHelper.ValidateUserArea(userArea);
            _passwordUpdateCommandHelper.ValidatePermissions(userArea, executionContext);

            _passwordUpdateCommandHelper.UpdatePassword(command.NewPassword, user, executionContext);

            await _dbContext.SaveChangesAsync();
        }


        #endregion
        

        private Task<User> GetUser(int userId)
        {
            return _dbContext
                .Users
                .FilterById(userId)
                .FilterCanLogIn()
                .SingleOrDefaultAsync();
        }
       
    }
}
