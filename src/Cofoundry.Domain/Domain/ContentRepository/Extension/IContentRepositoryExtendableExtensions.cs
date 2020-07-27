using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Extendable
{
    public static class IContentRepositoryExtendableExtensions
    {
        public static IExtendableContentRepository AsExtendableContentRepository(this IDomainRepository contentRepository)
        {
            return CastToExtendableContentRepository(contentRepository);
        }

        public static IExtendableContentRepository AsExtendableContentRepository(this IContentRepository contentRepository)
        {
            return CastToExtendableContentRepository(contentRepository);
        }

        public static IExtendableContentRepository AsExtendableContentRepository(this IAdvancedContentRepository contentRepository)
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
