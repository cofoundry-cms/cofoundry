using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryCurrentUserQueryBuilder 
        : IContentRepositoryCurrentUserQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryCurrentUserQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

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
}
