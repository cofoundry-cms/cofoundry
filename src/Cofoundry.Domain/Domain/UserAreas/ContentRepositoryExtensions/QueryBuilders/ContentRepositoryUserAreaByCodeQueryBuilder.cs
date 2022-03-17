using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryUserAreaByCodeQueryBuilder
    : IContentRepositoryUserAreaByCodeQueryBuilder
    , IExtendableContentRepositoryPart
{
    private readonly string _userAreaCode;

    public ContentRepositoryUserAreaByCodeQueryBuilder(
        IExtendableContentRepository contentRepository,
        string userAreaCode
        )
    {
        ExtendableContentRepository = contentRepository;
        _userAreaCode = userAreaCode;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<UserAreaMicroSummary> AsMicroSummary()
    {
        var query = new GetUserAreaMicroSummaryByCodeQuery(_userAreaCode);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
