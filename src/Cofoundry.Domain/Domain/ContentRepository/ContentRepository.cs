using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepository 
        : IContentRepository
        , IAdvancedContentRepository
        , IExtendableContentRepository
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;

        public ContentRepository(
            IServiceProvider serviceProvider,
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor
            )
        {
            ServiceProvider = serviceProvider;
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
        }

        /// <summary>
        /// Service provider instance to be used for extension only
        /// i.e. by internal Cofoundry or plugins. Access this by casting
        /// to the IExtendableContentRepository interface.
        /// </summary>
        public virtual IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Handles the asynchronous execution the specified query.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        public virtual Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query)
        {
            return _queryExecutor.ExecuteAsync(query);
        }

        /// <summary>
        /// Handles the execution of the specified command.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        public virtual Task ExecuteCommandAsync(ICommand command)
        {
            return _commandExecutor.ExecuteAsync(command);
        }
    }
}
