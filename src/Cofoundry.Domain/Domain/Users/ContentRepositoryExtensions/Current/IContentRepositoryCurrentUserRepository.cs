namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries relating to the currently logged in user.
    /// </summary>
    public interface IContentRepositoryCurrentUserRepository
    {
        /// <summary>
        /// Retrieve the currently logged in user. If
        /// there are multiple users then this only applies to the
        /// UserArea set as the default schema.
        /// </summary>
        IContentRepositoryCurrentUserQueryBuilder Get();
    }
}
