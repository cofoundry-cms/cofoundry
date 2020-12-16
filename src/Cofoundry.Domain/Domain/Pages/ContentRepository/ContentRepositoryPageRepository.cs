using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryPageRepository
            : IContentRepositoryPageRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPageRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        #region queries
        
        public IContentRepositoryPageGetAllQueryBuilder GetAll()
        {
            return new ContentRepositoryPageGetAllQueryBuilder(ExtendableContentRepository);
        }

        public IContentRepositoryPageByIdQueryBuilder GetById(int pageId)
        {
            return new ContentRepositoryPageByIdQueryBuilder(ExtendableContentRepository, pageId);
        }

        public IContentRepositoryPageByIdRangeQueryBuilder GetByIdRange(IEnumerable<int> pageIds)
        {
            return new ContentRepositoryPageByIdRangeQueryBuilder(ExtendableContentRepository, pageIds);
        }

        public IContentRepositoryPageSearchQueryBuilder Search()
        {
            return new ContentRepositoryPageSearchQueryBuilder(ExtendableContentRepository);
        }

        public IContentRepositoryPageByPathQueryBuilder GetByPath()
        {
            return new ContentRepositoryPageByPathQueryBuilder(ExtendableContentRepository);
        }

        public IContentRepositoryPageByDirectoryIdQueryBuilder GetByDirectoryId(int directoryId)
        {
            return new ContentRepositoryPageByDirectoryIdQueryBuilder(ExtendableContentRepository, directoryId);
        }

        #endregion
    }
}
