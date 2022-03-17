using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryPageBlockTypeByIdQueryBuilder
    : IContentRepositoryPageBlockTypeByIdQueryBuilder
    , IExtendableContentRepositoryPart
{
    private readonly int _pageBlockTypeId;

    public ContentRepositoryPageBlockTypeByIdQueryBuilder(
        IExtendableContentRepository contentRepository,
        int pageBlockTypeId
        )
    {
        ExtendableContentRepository = contentRepository;
        _pageBlockTypeId = pageBlockTypeId;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<PageBlockTypeSummary> AsSummary()
    {
        var query = new GetPageBlockTypeSummaryByIdQuery(_pageBlockTypeId);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public IDomainRepositoryQueryContext<PageBlockTypeDetails> AsDetails()
    {
        var query = new GetPageBlockTypeDetailsByIdQuery(_pageBlockTypeId);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
