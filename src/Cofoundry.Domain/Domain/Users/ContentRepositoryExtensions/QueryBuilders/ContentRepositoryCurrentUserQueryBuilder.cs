using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
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

        public IContentRepositoryQueryContext<UserMicroSummary> AsMicroSummary()
        {
            var query = new GetCurrentUserMicroSummaryQuery();
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<UserSummary> AsSummary()
        {
            var query = new GetCurrentUserSummaryQuery();
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<UserDetails> AsDetails()
        {
            var query = new GetCurrentUserDetailsQuery();
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
