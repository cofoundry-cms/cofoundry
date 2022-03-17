using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryUserByEmailQueryBuilder
    : IContentRepositoryUserByEmailQueryBuilder
    , IExtendableContentRepositoryPart
{
    private readonly string _userAreaCode;
    private readonly string _emailAddress;

    public ContentRepositoryUserByEmailQueryBuilder(
        IExtendableContentRepository contentRepository,
        string userAreaCode,
        string emailAddress
        )
    {
        ExtendableContentRepository = contentRepository;
        _userAreaCode = userAreaCode;
        _emailAddress = emailAddress;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<UserMicroSummary> AsMicroSummary()
    {
        var query = new GetUserMicroSummaryByEmailQuery()
        {
            UserAreaCode = _userAreaCode,
            Email = _emailAddress
        };

        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
