using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryCustomEntityRepository
            : IContentRepositoryCustomEntityRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryCustomEntityRepository(
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

        #region child entities

        /// <summary>
        /// Custom entity definitions are used to define the identity and
        /// behavior of a custom entity type. This includes meta data such
        /// as the name and description, but also the configuration of
        /// features such as whether the identity can contain a locale
        /// and whether versioning (i.e. auto-publish) is enabled.
        /// </summary>
        public IContentRepositoryCustomEntityDefinitionsRepository Definitions()
        {
            return new ContentRepositoryCustomEntityDefinitionsRepository(ExtendableContentRepository);
        }

        #endregion
    }
}
