using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class ContentRepositoryPageAccessByPageIdQueryBuilder
        : IAdvancedContentRepositoryPageAccessByPageIdQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly int _pageId;

        public ContentRepositoryPageAccessByPageIdQueryBuilder(
            IExtendableContentRepository contentRepository,
            int pageId
            )
        {
            ExtendableContentRepository = contentRepository;
            _pageId = pageId;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }


        public IDomainRepositoryQueryContext<PageAccessRuleSetDetails> AsDetails()
        {
            var query = new GetPageAccessRuleSetDetailsByPageIdQuery(_pageId);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
