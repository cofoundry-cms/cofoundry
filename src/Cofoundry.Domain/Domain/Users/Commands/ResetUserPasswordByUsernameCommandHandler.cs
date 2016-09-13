using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;

namespace Cofoundry.Domain
{
    public class ResetUserPasswordByUsernameCommandHandler 
        : ICommandHandler<ResetUserPasswordByUsernameCommand>
        , IAsyncCommandHandler<ResetUserPasswordByUsernameCommand>
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

        public void Execute(ResetUserPasswordByUsernameCommand command, IExecutionContext executionContext)
        {
            _resetUserPasswordCommandHelper.ValidateCommand(command, executionContext);
            var user = QueryUser(command, executionContext).SingleOrDefault();
            if (user == null) return;
            _resetUserPasswordCommandHelper.ResetPassword(user, command, executionContext);
        }

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
                .Where(u => u.Username == command.Username.Trim());

            return user;
        }

        #endregion
    }
}
