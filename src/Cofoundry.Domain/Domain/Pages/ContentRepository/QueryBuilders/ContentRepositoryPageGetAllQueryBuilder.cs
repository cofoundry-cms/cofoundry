using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryPageGetAllQueryBuilder
        : IContentRepositoryPageGetAllQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPageGetAllQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<ICollection<PageRoute>> AsRoutesAsync()
        {
            var query = new GetAllPageRoutesQuery();
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
