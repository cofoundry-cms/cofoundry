namespace Cofoundry.Domain;

public static class IContentRepositoryExtensions
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
    /// <param name="repository">
    /// Repository instance to extend.
    /// </param>
    public static IContentRepository WithContext<TUserAreaDefinition>(this IContentRepository repository)
        where TUserAreaDefinition : IUserAreaDefinition
    {
        return (IContentRepository)IDomainRepositoryExtensions.WithContext<TUserAreaDefinition>(repository);
    }
}
