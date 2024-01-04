using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Domain.CQS.Internal;

/// <summary>
/// Default implementation of <see cref="IQueryHandlerFactory"/>.
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

    /// <inheritdoc/>
    public IQueryHandler<TQuery, TResult> CreateAsyncHandler<TQuery, TResult>() where TQuery : IQuery<TResult>
    {
        return _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
    }
}
