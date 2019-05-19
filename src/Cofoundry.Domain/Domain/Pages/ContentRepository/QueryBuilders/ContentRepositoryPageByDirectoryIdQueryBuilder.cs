using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryPageByDirectoryIdQueryBuilder
        : IContentRepositoryPageByDirectoryIdQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly int _pageDirectoryId;

        public ContentRepositoryPageByDirectoryIdQueryBuilder(
            IExtendableContentRepository contentRepository,
            int pageDirectoryId
            )
        {
            ExtendableContentRepository = contentRepository;
            _pageDirectoryId = pageDirectoryId;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<ICollection<PageRoute>> AsPageRoutesAsync()
        {
            var query = new GetPageRoutesByPageDirectoryIdQuery(_pageDirectoryId);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
