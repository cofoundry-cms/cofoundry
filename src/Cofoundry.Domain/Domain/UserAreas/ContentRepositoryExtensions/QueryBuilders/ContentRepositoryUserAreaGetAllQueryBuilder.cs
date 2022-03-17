using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryUserAreaGetAllQueryBuilder
    : IContentRepositoryUserAreaGetAllQueryBuilder
    , IExtendableContentRepositoryPart
{
    public ContentRepositoryUserAreaGetAllQueryBuilder(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<ICollection<UserAreaMicroSummary>> AsMicroSummaries()
    {
        var query = new GetAllUserAreaMicroSummariesQuery();
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
