using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
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

        public IContentRepositoryQueryContext<UserMicroSummary> AsMicroSummary()
        {
            var query = new GetUserMicroSummaryByIdQuery(_userId);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<UserDetails> AsDetails()
        {
            var query = new GetUserDetailsByIdQuery(_userId);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
