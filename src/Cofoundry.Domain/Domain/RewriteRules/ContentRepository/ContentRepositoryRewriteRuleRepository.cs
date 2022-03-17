using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryRewriteRuleRepository
        : IAdvancedContentRepositoryRewriteRuleRepository
        , IExtendableContentRepositoryPart
{
    public ContentRepositoryRewriteRuleRepository(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IContentRepositoryRewriteRuleByPathQueryBuilder GetByPath(string path)
    {
        return new ContentRepositoryRewriteRuleByPathQueryBuilder(ExtendableContentRepository, path);
    }

    public IContentRepositoryRewriteRuleGetAllQueryBuilder GetAll()
    {
        return new ContentRepositoryRewriteRuleGetAllQueryBuilder(ExtendableContentRepository);
    }

    public async Task<int> AddAsync(AddRedirectRuleCommand command)
    {
        await ExtendableContentRepository.ExecuteCommandAsync(command);
        return command.OutputRedirectRuleId;
    }
}