using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using Cofoundry.Core.EntityFramework;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class SetupCofoundryCommandHandler
        : IAsyncCommandHandler<SetupCofoundryCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly ICommandExecutor _commandExecutor;
        private readonly IQueryExecutor _queryExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly UserContextMapper _userContextMapper;
        
        public SetupCofoundryCommandHandler(
            ICommandExecutor commandExecutor,
            IQueryExecutor queryExecutor,
            CofoundryDbContext dbContext,
            ITransactionScopeFactory transactionScopeFactory,
            UserContextMapper userContextMapper
            )
        {
            _commandExecutor = commandExecutor;
            _queryExecutor = queryExecutor;
            _dbContext = dbContext;
            _transactionScopeFactory = transactionScopeFactory;
            _userContextMapper = userContextMapper;
        }

        #endregion

        #region Execute

        public async Task ExecuteAsync(SetupCofoundryCommand command, IExecutionContext executionContext)
        {
            var settings = _queryExecutor.Get<InternalSettings>();
            if (settings.IsSetup)
            {
                throw new ApplicationException("Site is already set up.");
            }

            using (var scope = _transactionScopeFactory.Create())
            {
                var userId = await CreateAdminUser(command);
                var impersonatedUserContext = await GetImpersonatedUserContext(executionContext, userId);

                var settingsCommand = await _queryExecutor.GetAsync<UpdateGeneralSiteSettingsCommand>();
                settingsCommand.CompanyName = command.CompanyName;
                await _commandExecutor.ExecuteAsync(settingsCommand, impersonatedUserContext);

                // Create the root directory
                var wdCommand = new AddRootWebDirectoryCommand();
                await _commandExecutor.ExecuteAsync(wdCommand, impersonatedUserContext);

                // Setup Complete
                await _commandExecutor.ExecuteAsync(new MarkAsSetUpCommand(), impersonatedUserContext);

                scope.Complete();
            }
        }

        private async Task<ExecutionContext>  GetImpersonatedUserContext(IExecutionContext executionContext, int userId)
        {
            var dbUser = await _dbContext
                .Users
                .FilterByUserArea(CofoundryAdminUserArea.AreaCode)
                .FilterById(userId)
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(dbUser, userId);
            var impersonatedUserContext = _userContextMapper.Map(dbUser);

            var impersonatedExecutionContext = new ExecutionContext()
            {
                ExecutionDate = executionContext.ExecutionDate,
                UserContext = impersonatedUserContext
            };

            return impersonatedExecutionContext;
        }

        #endregion

        #region private helpers

        private async Task<int> CreateAdminUser(SetupCofoundryCommand command)
        {
            var newUserCommand = new AddMasterCofoundryUserCommand();
            newUserCommand.Email = command.UserEmail;
            newUserCommand.FirstName = command.UserFirstName;
            newUserCommand.LastName = command.UserLastName;
            newUserCommand.Password = command.UserPassword;
            await _commandExecutor.ExecuteAsync(newUserCommand);

            return newUserCommand.OutputUserId;
        }

        #endregion
    }
}
