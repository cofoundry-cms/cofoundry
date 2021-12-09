using System;

namespace Cofoundry.Domain.Extendable
{
    public static class IExtendableContentRepositoryPartExtensions
    {
        public static IExtendableContentRepositoryPart AsExtendableContentRepositoryPart<TRepositoryPart>(this TRepositoryPart contentRepository)
            where TRepositoryPart : IContentRepositoryPart
        {
            return CastToExtendableContentRepositoryPart(contentRepository);
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
}
