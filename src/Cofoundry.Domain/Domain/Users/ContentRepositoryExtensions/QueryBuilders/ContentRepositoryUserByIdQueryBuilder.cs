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

        public Task<UserMicroSummary> AsMicroSummaryAsync()
        {
            var query = new GetUserMicroSummaryByIdQuery(_userId);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<UserDetails> AsDetailsAsync()
        {
            var query = new GetUserDetailsByIdQuery(_userId);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
