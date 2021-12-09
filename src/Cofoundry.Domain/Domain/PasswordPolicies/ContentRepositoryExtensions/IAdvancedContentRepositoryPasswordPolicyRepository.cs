namespace Cofoundry.Domain
{
    /// <summary>
    /// <see cref="IAdvancedContentRepositoryUserAreaRepository"/> extension for user area password policies.
    /// </summary>
    public interface IAdvancedContentRepositoryPasswordPolicyRepository
    {
        /// <summary>
        /// Gets the password policy for a user area.
        /// </summary>
        /// <param name="userAreaCode">
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area to query.
        /// </param>
        IContentRepositoryPasswordPolicyByCodeQueryBuilder GetByCode(string userAreaCode);
    }
}
