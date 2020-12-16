using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    public class ResetUserPasswordByUsernameCommandHandler 
        : ICommandHandler<ResetUserPasswordByUsernameCommand>
        , IIgnorePermissionCheckHandler
    {
        #region construstor
        
        private readonly CofoundryDbContext _dbContext;
        private readonly IResetUserPasswordCommandHelper _resetUserPasswordCommandHelper;

        public ResetUserPasswordByUsernameCommandHandler(
            CofoundryDbContext dbContext,
            IResetUserPasswordCommandHelper resetUserPasswordCommandHelper
            )
        {
            _dbContext = dbContext;
            _resetUserPasswordCommandHelper = resetUserPasswordCommandHelper; 
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(ResetUserPasswordByUsernameCommand command, IExecutionContext executionContext)
        {
            await _resetUserPasswordCommandHelper.ValidateCommandAsync(command, executionContext);
            var user = await QueryUser(command, executionContext).SingleOrDefaultAsync();
            if (user == null) return;
            await _resetUserPasswordCommandHelper.ResetPasswordAsync(user, command, executionContext);
        }

        #endregion

        #region private helpers

        private IQueryable<User> QueryUser(ResetUserPasswordByUsernameCommand command, IExecutionContext executionContext)
        {
            var user = _dbContext
                .Users
                .FilterByUserArea(command.UserAreaCode)
                .FilterCanLogIn()
                .Where(u => u.Username == command.Username.Trim());

            return user;
        }

        #endregion
    }
}
