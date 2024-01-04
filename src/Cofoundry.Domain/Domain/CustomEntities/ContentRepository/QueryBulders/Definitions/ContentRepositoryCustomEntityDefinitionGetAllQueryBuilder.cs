using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryCustomEntityDefinitionGetAllQueryBuilder
    : IContentRepositoryCustomEntityDefinitionGetAllQueryBuilder
    , IExtendableContentRepositoryPart
{
    public ContentRepositoryCustomEntityDefinitionGetAllQueryBuilder(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<IReadOnlyCollection<CustomEntityDefinitionMicroSummary>> AsMicroSummaries()
    {
        var query = new GetAllCustomEntityDefinitionMicroSummariesQuery();
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public IDomainRepositoryQueryContext<IReadOnlyCollection<CustomEntityDefinitionSummary>> AsSummaries()
    {
        var query = new GetAllCustomEntityDefinitionSummariesQuery();
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
