namespace Cofoundry.Domain;

/// <summary>
/// Content repository extension methods for user area definitions.
/// </summary>
public static class IUserAreaDefinitionRepositoryExtensions
{
    extension(IUserAreaDefinitionRepository userAreaDefinitionRepository)
    {
        /// <summary>
        /// Determins if a user area exists with the specified code.
        /// </summary>
        /// <param name="userAreaCode">The unique 3 character code that identifies the user area definition.</param>
        /// <returns><see langword="true"/> if the user area exists; otherwise <see langword="false"/>.</returns>
        public bool Exists(string? userAreaCode)
        {
            return !string.IsNullOrEmpty(userAreaCode) && userAreaDefinitionRepository.GetByCode(userAreaCode) != null;
        }
    }
}
