using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class ContentRepositoryPasswordPolicyRepository
        : IAdvancedContentRepositoryPasswordPolicyRepository
        , IExtendableContentRepositoryPart
{
    public ContentRepositoryPasswordPolicyRepository(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IContentRepositoryPasswordPolicyByCodeQueryBuilder GetByCode(string userAreaCode)
    {
        return new ContentRepositoryPasswordPolicyByCodeQueryBuilder(ExtendableContentRepository, userAreaCode);
        throw new System.NotImplementedException();
    }
}
