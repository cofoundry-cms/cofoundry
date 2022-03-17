using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Domain.CQS.Internal;

/// <summary>
/// Factory to create the default QueryHandler instance
/// </summary>
public class QueryHandlerFactory : IQueryHandlerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public QueryHandlerFactory(
        IServiceProvider serviceProvider
        )
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Creates a new IAsyncQueryHandler instance with the specified type signature.
    /// </summary>
    public IQueryHandler<TQuery, TResult> CreateAsyncHandler<TQuery, TResult>() where TQuery : IQuery<TResult>
    {
        return _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
    }
}
