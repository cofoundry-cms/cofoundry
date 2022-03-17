using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryCurrentUserQueryBuilder
    : IContentRepositoryCurrentUserQueryBuilder
    , IExtendableContentRepositoryPart
{
    public ContentRepositoryCurrentUserQueryBuilder(
        IExtendableContentRepository contentRepository)
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<IUserContext> AsUserContext()
    {
        var query = new GetCurrentUserContextQuery();

        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public IDomainRepositoryQueryContext<UserMicroSummary> AsMicroSummary()
    {
        var query = new GetCurrentUserMicroSummaryQuery();

        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public IDomainRepositoryQueryContext<UserSummary> AsSummary()
    {
        var query = new GetCurrentUserSummaryQuery();

        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public IDomainRepositoryQueryContext<UserDetails> AsDetails()
    {
        var query = new GetCurrentUserDetailsQuery();

        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
