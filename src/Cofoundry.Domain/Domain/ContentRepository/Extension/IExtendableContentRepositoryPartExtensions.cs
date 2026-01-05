namespace Cofoundry.Domain.Extendable;

/// <summary>
/// Content repository part extension methods to allow casting to <see cref="IExtendableContentRepositoryPart"/>
/// which allows for modular extension via Cofoundry or plugins.
/// </summary>
public static class IExtendableContentRepositoryPartExtensions
{
    extension<TRepositoryPart>(TRepositoryPart contentRepository) where TRepositoryPart : IContentRepositoryPart
    {
        public IExtendableContentRepositoryPart AsExtendableContentRepositoryPart()
        {
            return CastToExtendableContentRepositoryPart(contentRepository);
        }
    }

    private static IExtendableContentRepositoryPart CastToExtendableContentRepositoryPart<T>(T contentRepository)
    {
        if (contentRepository is IExtendableContentRepositoryPart extendableRepository)
        {
            return extendableRepository;
        }

        throw new Exception($"An {nameof(IContentRepositoryPart)} implementation should also implement {nameof(IExtendableContentRepositoryPart)} to allow internal/plugin extendibility.");
    }
}
