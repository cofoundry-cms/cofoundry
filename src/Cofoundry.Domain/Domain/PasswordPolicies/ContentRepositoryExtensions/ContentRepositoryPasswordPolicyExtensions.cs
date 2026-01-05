using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Content repository extension methods for password policies.
/// </summary>
public static class ContentRepositoryPasswordPolicyExtensions
{
    extension(IAdvancedContentRepositoryUserAreaRepository contentRepository)
    {
        /// <summary>
        /// Queries relating to user area password policies.
        /// </summary>
        public IAdvancedContentRepositoryPasswordPolicyRepository PasswordPolicies()
        {
            var extendableContentRepository = contentRepository.AsExtendableContentRepositoryPart().ExtendableContentRepository;
            return new ContentRepositoryPasswordPolicyRepository(extendableContentRepository);
        }
    }
}
