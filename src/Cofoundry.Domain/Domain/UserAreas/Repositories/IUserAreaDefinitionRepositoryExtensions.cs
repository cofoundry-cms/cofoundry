namespace Cofoundry.Domain
{
    public static class IUserAreaDefinitionRepositoryExtensions
    {
        /// <summary>
        /// Determins if a user area exists with the specified code.
        /// </summary>
        /// <param name="userAreaCode">The unique 3 character code that identifies the user area definition.</param>
        /// <returns><see langword="true"/> if the user area exists; otherwise <see langword="false"/>.</returns>
        public static bool Exists(this IUserAreaDefinitionRepository userAreaDefinitionRepository, string userAreaCode)
        {
            return !string.IsNullOrEmpty(userAreaCode) && userAreaDefinitionRepository.GetByCode(userAreaCode) != null;
        }
    }
}
