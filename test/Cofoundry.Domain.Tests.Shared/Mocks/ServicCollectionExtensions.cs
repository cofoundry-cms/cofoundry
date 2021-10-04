using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Tests.Shared.Mocks
{
    /// <summary>
    /// Extensions to help with mocking and altering behaviour when setting up tests.
    /// </summary>
    public static class ServicCollectionExtensions
    {
        /// <summary>
        /// Overrides a query handler to return a mock result.
        /// </summary>
        /// <typeparam name="TQuery">Query type parameter of the <see cref="IQueryHandler{,}"/> to override e.g. GetPageDetailsByIdQuery.</typeparam>
        /// <typeparam name="TResult">Query result parameter of the <see cref="IQueryHandler{,}"/> to override e.g. PageDetails.</typeparam>
        /// <param name="serviceCollection"><see cref="IServiceCollection"/> to alter.</param>
        /// <param name="result">The result to return from the mock handler.</param>
        public static IServiceCollection MockHandler<TQuery, TResult>(this IServiceCollection serviceCollection, TResult result)
            where TQuery : IQuery<TResult>
        {
            return serviceCollection.AddTransient<IAsyncQueryHandler<TQuery, TResult>>(s => new MockQueryHandler<TQuery, TResult>(result));
        }

        /// <summary>
        /// Overrides a query handler to return a mock result, determined by executing the 
        /// <paramref name="queryDelegate"/>.
        /// </summary>
        /// <typeparam name="TQuery">Query type parameter of the <see cref="IQueryHandler{,}"/> to override e.g. GetPageDetailsByIdQuery.</typeparam>
        /// <typeparam name="TResult">Query result parameter of the <see cref="IQueryHandler{,}"/> to override e.g. PageDetails.</typeparam>
        /// <param name="serviceCollection"><see cref="IServiceCollection"/> to alter.</param>
        /// <param name="queryDelegate">A delegate task to run to determine the result of the query handler.</param>
        public static IServiceCollection MockHandler<TQuery, TResult>(this IServiceCollection serviceCollection, Func<TQuery, TResult> queryDelegate)
            where TQuery : IQuery<TResult>
        {
            return serviceCollection.AddTransient<IAsyncQueryHandler<TQuery, TResult>>(s => new MockQueryHandler<TQuery, TResult>(queryDelegate));
        }

        /// <summary>
        /// Overrides a query handler to return a mock result, determined by executing the 
        /// <paramref name="asyncQueryDelegate"/>.
        /// </summary>
        /// <typeparam name="TQuery">Query type parameter of the <see cref="IQueryHandler{,}"/> to override e.g. GetPageDetailsByIdQuery.</typeparam>
        /// <typeparam name="TResult">Query result parameter of the <see cref="IQueryHandler{,}"/> to override e.g. PageDetails.</typeparam>
        /// <param name="serviceCollection"><see cref="IServiceCollection"/> to alter.</param>
        /// <param name="asyncQueryDelegate">A delegate task to run to determine the result of the query handler.</param>
        public static IServiceCollection MockHandler<TQuery, TResult>(this IServiceCollection serviceCollection, Func<TQuery, Task<TResult>> asyncQueryDelegate)
            where TQuery : IQuery<TResult>
        {
            return serviceCollection.AddTransient<IAsyncQueryHandler<TQuery, TResult>>(s => new MockQueryHandler<TQuery, TResult>(asyncQueryDelegate));
        }

        /// <summary>
        /// Turns on the developer exception page so that you can check
        /// the details of the exception thrown.
        /// </summary>
        public static IServiceCollection TurnOnDeveloperExceptionPage(this IServiceCollection serviceCollection)
        {
            return serviceCollection.Configure<DebugSettings>(c => c.DeveloperExceptionPageMode = DeveloperExceptionPageMode.On);
        }
    }
}
