using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Extendable;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// An <see cref="IDomainRepositoryExecutor"/> implementation that executes
    /// command and queries using a specific user context.
    /// </summary>
    /// <inheritdoc/>
    public class DomainRepositoryExecutorWithUserContext : IDomainRepositoryExecutor
    {
        private readonly IDomainRepositoryExecutor _innerDomainRepositoryExecutor;
        private readonly IExecutionContextFactory _executionContextFactory;
        private readonly IUserContext _userContext;

        public DomainRepositoryExecutorWithUserContext(
            IDomainRepositoryExecutor innerDomainRepositoryExecutor,
            IExecutionContextFactory executionContextFactory,
            IUserContext userContext
            )
        {
            _innerDomainRepositoryExecutor = innerDomainRepositoryExecutor;
            _executionContextFactory = executionContextFactory;
            _userContext = userContext;
        }

        public async Task ExecuteAsync(ICommand command, IExecutionContext executionContext)
        {
            var newExecutionContext = _executionContextFactory.Create(_userContext, executionContext);
            await _innerDomainRepositoryExecutor.ExecuteAsync(command, newExecutionContext);
        }

        public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, IExecutionContext executionContext)
        {
            var newExecutionContext = _executionContextFactory.Create(_userContext, executionContext);
            return await _innerDomainRepositoryExecutor.ExecuteAsync(query, newExecutionContext);
        }
    }
}