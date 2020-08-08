using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryWithCustomExecutionContext
        : ContentRepository
        , IContentRepositoryWithCustomExecutionContext
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private IExecutionContext _executionContext = null;

        public ContentRepositoryWithCustomExecutionContext(
            IServiceProvider serviceProvider,
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor
            )
            : base(serviceProvider, queryExecutor, commandExecutor)
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
        }

        /// <summary>
        /// Sets the execution context for any queries or commands
        /// chained of this instance. Typically used to impersonate
        /// a user or elevate permissions.
        /// </summary>
        /// <param name="executionContext">
        /// The execution context instance to use. May pass null to reset 
        /// this instance and use the default.
        /// </param>
        public virtual void SetExecutionContext(IExecutionContext executionContext)
        {
            _executionContext = executionContext;
        }

        /// <summary>
        /// Handles the asynchronous execution the specified query.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        public override Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query)
        {
            return _queryExecutor.ExecuteAsync(query, _executionContext);
        }

        /// <summary>
        /// Handles the execution of the specified command.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        public override Task ExecuteCommandAsync(ICommand command)
        {
            return _commandExecutor.ExecuteAsync(command, _executionContext);
        }
    }
}
