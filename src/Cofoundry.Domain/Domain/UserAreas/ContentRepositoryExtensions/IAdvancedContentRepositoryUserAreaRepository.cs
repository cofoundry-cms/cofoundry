namespace Cofoundry.Domain;

/// <summary>
/// IAdvancedContentRespository extension root for user areas.
/// </summary>
public interface IAdvancedContentRepositoryUserAreaRepository : IContentRepositoryPart
{
    /// <summary>
    /// Query a user area by it's unique <see cref="IUserAreaDefinition.UserAreaCode"/>.
    /// If the definition does not exist then <see langword="null"/> is returned.
    /// </summary>
    /// <param name="userAreaCode">
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area to query.
    /// </param>
    IContentRepositoryUserAreaByCodeQueryBuilder GetByCode(string userAreaCode);

    /// <summary>
    /// Query all user areas registered with the system.
    /// </summary>
    IContentRepositoryUserAreaGetAllQueryBuilder GetAll();
}
