namespace Cofoundry.Domain;

/// <summary>
/// Queries for retrieving user data for a unique database id.
/// </summary>
public interface IContentRepositoryUserByIdQueryBuilder
{
    /// <summary>
    /// The <see cref="UserMicroSummary"/> is a minimal projection of user data 
    /// that is quick to load.
    /// </summary>
    IDomainRepositoryQueryContext<UserMicroSummary> AsMicroSummary();

    /// <summary>
    /// The <see cref="UserDetails"/> projection is a full representation of a user, containing 
    /// all properties including role and permission data. If the user is not logged 
    /// in then <see langword="null"/> is returned.
    /// </summary>
    IDomainRepositoryQueryContext<UserDetails> AsDetails();
}
