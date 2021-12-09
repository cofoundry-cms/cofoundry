using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class ContentRepositoryPasswordPolicyByCodeQueryBuilder
        : IContentRepositoryPasswordPolicyByCodeQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly string _userAreaCode;

        public ContentRepositoryPasswordPolicyByCodeQueryBuilder(
            IExtendableContentRepository contentRepository,
            string userAreaCode
            )
        {
            ExtendableContentRepository = contentRepository;
            _userAreaCode = userAreaCode;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<PasswordPolicyDescription> AsDescription()
        {
            var query = new GetPasswordPolicyDescriptionByUserAreaCodeQuery(_userAreaCode);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
