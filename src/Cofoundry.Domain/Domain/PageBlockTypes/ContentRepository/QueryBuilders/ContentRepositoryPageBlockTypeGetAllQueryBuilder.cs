using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryPageBlockTypeGetAllQueryBuilder
        : IContentRepositoryPageBlockTypeGetAllQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPageBlockTypeGetAllQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<ICollection<PageBlockTypeSummary>> AsSummariesAsync()
        {
            var query = new GetAllPageBlockTypeSummariesQuery();
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
