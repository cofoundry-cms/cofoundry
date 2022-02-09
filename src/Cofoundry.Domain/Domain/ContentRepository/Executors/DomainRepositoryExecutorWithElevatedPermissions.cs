using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Extendable;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// An <see cref="IDomainRepositoryExecutor"/> implementation that executes
    /// command and queries using the system account, effectively ignoring
    /// permission checks.
    /// </summary>
    /// <inheritdoc/>
    public class DomainRepositoryExecutorWithElevatedPermissions : IDomainRepositoryExecutor
    {
        private readonly IExecutionContextFactory _executionContextFactory;
        private readonly IDomainRepositoryExecutor _innerDomainRepositoryExecutor;

        public DomainRepositoryExecutorWithElevatedPermissions(
            IDomainRepositoryExecutor innerDomainRepositoryExecutor,
            IExecutionContextFactory executionContextFactory
            )
        {
            _executionContextFactory = executionContextFactory;
            _innerDomainRepositoryExecutor = innerDomainRepositoryExecutor;
        }

        public async Task ExecuteAsync(ICommand command, IExecutionContext executionContext)
        {
            var newExecutionContext = await _executionContextFactory.CreateSystemUserExecutionContextAsync(executionContext);
            await _innerDomainRepositoryExecutor.ExecuteAsync(command, newExecutionContext);
        }

        public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, IExecutionContext executionContext)
        {
            var newExecutionContext = await _executionContextFactory.CreateSystemUserExecutionContextAsync(executionContext);

            return await _innerDomainRepositoryExecutor.ExecuteAsync(query, newExecutionContext);
        }
    }
}