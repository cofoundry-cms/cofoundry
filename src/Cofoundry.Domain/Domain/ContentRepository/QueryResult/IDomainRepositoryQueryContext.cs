namespace Cofoundry.Domain;

/// <summary>
/// Represents a repository query that is ready to execute.
/// </summary>
/// <typeparam name="TQueryResult">The type of the query result.</typeparam>
public interface IDomainRepositoryQueryContext<TQueryResult>
    : IDomainRepositoryQueryMutator<TQueryResult, TQueryResult>
{
}
