namespace Cofoundry.Domain;

/// <summary>
/// Extension methods for <see cref="IContentRepository"/>.
/// </summary>
public static class IContentRepositoryExtensions
{
    extension(IContentRepository repository)
    {
        /// <summary>
        /// Execute queries or commands using the user context associated with the
        /// specified user area. This is useful when implementing multiple user areas
        /// whereby a client can be logged into multiple user accounts belonging to 
        /// different user areas. Use this to force execution to use the context
        /// of a specific user area rather than relying on the "ambient" or default.
        /// </summary>
        /// <typeparam name="TUserAreaDefinition">
        /// The user area to use when determining the signed in user to execute
        /// tasks with.
        /// </typeparam>
        public IContentRepository WithContext<TUserAreaDefinition>()
            where TUserAreaDefinition : IUserAreaDefinition
        {
            return (IContentRepository)IDomainRepositoryExtensions.WithContext<TUserAreaDefinition>(repository);
        }
    }
}
