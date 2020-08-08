using Cofoundry.Domain.CQS;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryWithElevatedPermissions 
        : ContentRepository
        , IContentRepositoryWithElevatedPermissions
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private readonly Lazy<Task<IExecutionContext>> _elevatedExecutionContextAsync;
        
        public ContentRepositoryWithElevatedPermissions(
            IServiceProvider serviceProvider,
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            IExecutionContextFactory executionContextFactory
            )
            : base(serviceProvider, queryExecutor, commandExecutor)
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;

            _elevatedExecutionContextAsync = new Lazy<Task<IExecutionContext>>(() =>
            {
                return executionContextFactory.CreateSystemUserExecutionContextAsync();
            });
        }

        /// <summary>
        /// Handles the asynchronous execution the specified query.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        public override async Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query)
        {
            var executionContext = await _elevatedExecutionContextAsync.Value;
            return await _queryExecutor.ExecuteAsync(query, executionContext);
        }

        /// <summary>
        /// Handles the execution of the specified command.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        public override async Task ExecuteCommandAsync(ICommand command)
        {
            var executionContext = await _elevatedExecutionContextAsync.Value;
            await _commandExecutor.ExecuteAsync(command, executionContext);
        }
    }
}
