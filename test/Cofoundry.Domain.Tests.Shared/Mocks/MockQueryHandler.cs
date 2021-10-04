using Cofoundry.Domain.CQS;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Tests.Shared.Mocks
{
    public class MockQueryHandler<TQuery, TResult>
        : IQueryHandler<TQuery, TResult>
        , IIgnorePermissionCheckHandler
        where TQuery : IQuery<TResult>
    {
        private readonly Func<TQuery, Task<TResult>> _queryDelegate;

        public MockQueryHandler(TResult result)
        {
            _queryDelegate = query => Task.FromResult(result);
        }

        public MockQueryHandler(Func<TQuery, TResult> queryDelegate)
        {
            _queryDelegate = query => Task.FromResult(queryDelegate(query));
        }

        public MockQueryHandler(Func<TQuery, Task<TResult>> asyncQueryDelegate)
        {
            _queryDelegate = asyncQueryDelegate;
        }

        public Task<TResult> ExecuteAsync(TQuery query, IExecutionContext executionContext)
        {
            return _queryDelegate.Invoke(query);
        }
    }
}
