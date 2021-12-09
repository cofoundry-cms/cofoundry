namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving user data for a unique database id.
    /// </summary>
    public interface IContentRepositoryUserAreaByCodeQueryBuilder
    {
        /// <summary>
        /// The <see cref="UserMicroSummary"/> is a minimal projection of user data 
        /// that is quick to load.
        /// </summary>
        IDomainRepositoryQueryContext<UserAreaMicroSummary> AsMicroSummary();
    }
}
