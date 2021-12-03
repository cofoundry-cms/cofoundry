using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryUserByUsernameQueryBuilder
        : IContentRepositoryUserByUsernameQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly string _userAreaCode;
        private readonly string _username;

        public ContentRepositoryUserByUsernameQueryBuilder(
            IExtendableContentRepository contentRepository,
            string userAreaCode,
            string username
            )
        {
            ExtendableContentRepository = contentRepository;
            _userAreaCode = userAreaCode;
            _username = username;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<UserMicroSummary> AsMicroSummary()
        {
            var query = new GetUserMicroSummaryByUsernameQuery(_userAreaCode, _username);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
