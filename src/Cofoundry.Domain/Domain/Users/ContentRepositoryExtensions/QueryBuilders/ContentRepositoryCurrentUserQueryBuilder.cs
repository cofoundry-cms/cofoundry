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

        public Task<UserMicroSummary> AsMicroSummaryAsync()
        {
            var query = new GetCurrentUserMicroSummaryQuery();
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<UserSummary> AsSummaryAsync()
        {
            var query = new GetCurrentUserSummaryQuery();
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<UserDetails> AsDetailsAsync()
        {
            var query = new GetCurrentUserDetailsQuery();
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
