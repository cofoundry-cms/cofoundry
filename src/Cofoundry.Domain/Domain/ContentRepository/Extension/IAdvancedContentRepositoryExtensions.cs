namespace Cofoundry.Domain;

public static class IAdvancedContentRepositoryExtensions
{
    /// <summary>
    /// Execute queries or commands using the user context associated with the
    /// specified user area. This is useful when implementing multiple user areas
    /// whereby a client can be signed into multiple user accounts belonging to 
    /// different user areas. Use this to force execution to use the context
    /// of a specific user area rather than relying on the "ambient" or default.
    /// </summary>
    /// <typeparam name="TUserAreaDefinition">
    /// The user area to use when determining the signed in user to execute
    /// tasks with.
    /// </typeparam>
    public static IAdvancedContentRepository WithContext<TUserAreaDefinition>(this IAdvancedContentRepository repository)
        where TUserAreaDefinition : IUserAreaDefinition
    {
        return (IAdvancedContentRepository)IDomainRepositoryExtensions.WithContext<TUserAreaDefinition>(repository);
    }
}
