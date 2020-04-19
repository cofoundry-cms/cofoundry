using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryPageDirectoryGetAllQueryBuilder
        : IAdvancedContentRepositoryPageDirectoryGetAllQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPageDirectoryGetAllQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<ICollection<PageDirectoryRoute>> AsRoutesAsync()
        {
            var query = new GetAllPageDirectoryRoutesQuery();
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<PageDirectoryNode> AsTreeAsync()
        {
            var query = new GetPageDirectoryTreeQuery();
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
