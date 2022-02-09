using System;

namespace Cofoundry.Domain.Extendable
{
    public static class IExtendableContentRepositoryExtensions
    {
        /// <summary>
        /// Casts the repository instance as <see cref="IExtendableContentRepository"/> to
        /// provide access to hidden functionality intended for extending a repository with
        /// bespoke features. These are advanced feature intended to be used for extension only 
        /// e.g. internally by Cofoundry, in plugins or custom extensions
        /// </summary>
        public static IExtendableContentRepository AsExtendableContentRepository(this IDomainRepository contentRepository)
        {
            return CastToExtendableContentRepository(contentRepository);
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
}
