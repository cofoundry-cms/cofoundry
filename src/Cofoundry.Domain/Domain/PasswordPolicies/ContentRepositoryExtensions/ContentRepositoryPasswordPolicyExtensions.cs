using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

public static class ContentRepositoryPasswordPolicyExtensions
{
    /// <summary>
    /// Queries relating to user area password policies.
    /// </summary>
    public static IAdvancedContentRepositoryPasswordPolicyRepository PasswordPolicies(this IAdvancedContentRepositoryUserAreaRepository contentRepository)
    {
        var extendableContentRepository = contentRepository.AsExtendableContentRepositoryPart().ExtendableContentRepository;
        return new ContentRepositoryPasswordPolicyRepository(extendableContentRepository);
    }
}
