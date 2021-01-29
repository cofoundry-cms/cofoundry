using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryUserByIdQueryBuilder
        : IContentRepositoryUserByIdQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly int _userId;

        public ContentRepositoryUserByIdQueryBuilder(
            IExtendableContentRepository contentRepository,
            int userId
            )
        {
            ExtendableContentRepository = contentRepository;
            _userId = userId;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<UserMicroSummary> AsMicroSummary()
        {
            var query = new GetUserMicroSummaryByIdQuery(_userId);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<UserDetails> AsDetails()
        {
            var query = new GetUserDetailsByIdQuery(_userId);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
