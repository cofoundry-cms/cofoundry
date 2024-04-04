namespace Cofoundry.Domain.CQS.Internal;

/// <summary>
/// Factory to create <see cref="IQueryHandler{TQuery, TResult}"/> instances. This factory allows 
/// you to override or wrap the existing <see cref="IQueryHandler{TQuery, TResult}"/> implementation
/// </summary>
public interface IQueryHandlerFactory
{
    /// <summary>
    /// Creates a new <see cref="IQueryHandler{TQuery, TResult}"/> instance with the specified type signature.
    /// </summary>
    IQueryHandler<TQuery, TResult> CreateAsyncHandler<TQuery, TResult>() where TQuery : IQuery<TResult>;
}
