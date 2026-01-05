namespace Cofoundry.Domain.Extendable;

/// <summary>
/// Content repository extension methods to allow casting to <see cref="IExtendableContentRepository"/>
/// which allows for modular extension via Cofoundry or plugins.
/// </summary>
public static class IExtendableContentRepositoryExtensions
{
    extension(IDomainRepository contentRepository)
    {
        /// <summary>
        /// Casts the repository instance as <see cref="IExtendableContentRepository"/> to
        /// provide access to hidden functionality intended for extending a repository with
        /// bespoke features. These are advanced feature intended to be used for extension only 
        /// e.g. internally by Cofoundry, in plugins or custom extensions
        /// </summary>
        public IExtendableContentRepository AsExtendableContentRepository()
        {
            return CastToExtendableContentRepository(contentRepository);
        }
    }

    private static IExtendableContentRepository CastToExtendableContentRepository<T>(T contentRepository)
    {
        if (contentRepository is IExtendableContentRepository extendableRepository)
        {
            return extendableRepository;
        }

        throw new Exception($"An {nameof(IContentRepository)} implementation should also implement {nameof(IExtendableContentRepository)} to allow internal/plugin extendibility.");
    }
}
