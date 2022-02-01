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
        /// Overrides the registered command handler to run a mock handler instead.
        /// </summary>
        /// <typeparam name="TCommand">Command type parameter of the <see cref="ICommandHandler{TCommand}"/> to override e.g. AddPageCommand.</typeparam>
        /// <param name="serviceCollection"><see cref="IServiceCollection"/> to alter.</param>
        /// <param name="handlerDelegate">A delegate task to run to in place of the registered command handler.</param>
        public static IServiceCollection MockHandler<TCommand>(this IServiceCollection serviceCollection, Action<TCommand> handlerDelegate)
            where TCommand : ICommand
        {
            return serviceCollection.AddTransient<ICommandHandler<TCommand>>(s => new MockCommandHandler<TCommand>(handlerDelegate));
        }

        /// <summary>
        /// Overrides the registered command handler to run a mock handler instead.
        /// </summary>
        /// <typeparam name="TCommand">Command type parameter of the <see cref="ICommandHandler{TCommand}"/> to override e.g. AddPageCommand.</typeparam>
        /// <param name="serviceCollection"><see cref="IServiceCollection"/> to alter.</param>
        /// <param name="asyncHandlerDelegate">A delegate task to run to in place of the registered command handler.</param>
        public static IServiceCollection MockHandler<TCommand>(this IServiceCollection serviceCollection, Func<TCommand, Task> asyncHandlerDelegate)
            where TCommand : ICommand
        {
            return serviceCollection.AddTransient<ICommandHandler<TCommand>>(s => new MockCommandHandler<TCommand>(asyncHandlerDelegate));
        }

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
            return serviceCollection.AddTransient<IQueryHandler<TQuery, TResult>>(s => new MockQueryHandler<TQuery, TResult>(result));
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
            return serviceCollection.AddTransient<IQueryHandler<TQuery, TResult>>(s => new MockQueryHandler<TQuery, TResult>(queryDelegate));
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
            return serviceCollection.AddTransient<IQueryHandler<TQuery, TResult>>(s => new MockQueryHandler<TQuery, TResult>(asyncQueryDelegate));
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
