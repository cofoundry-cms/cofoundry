namespace Cofoundry.Domain;

public static class IContentRepositoryExtensions
{
    /// <summary>
    /// Uses the specified <paramref name="userContext"/> to build a new <see cref="IExecutionContext"/>
    /// to run queries or commands under. Typically this is used to impersonate a user or 
    /// elevate permissions.
    /// </summary>
    /// <param name="userContext">
    /// The <see cref="IUserContext"/> to build into a new <see cref="IExecutionContext"/>.
    /// </param>
    public static IContentRepository WithContext<TUserAreaDefinition>(this IContentRepository repository)
        where TUserAreaDefinition : IUserAreaDefinition
    {
        return (IContentRepository)IDomainRepositoryExtensions.WithContext<TUserAreaDefinition>(repository);
    }
}
