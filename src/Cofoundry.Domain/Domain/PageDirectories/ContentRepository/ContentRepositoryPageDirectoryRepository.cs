using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryPageDirectoryRepository
            : IContentRepositoryPageDirectoryRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPageDirectoryRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        #region queries

        public IContentRepositoryPageDirectoryByIdQueryBuilder GetById(int imageAssetId)
        {
            return new ContentRepositoryPageDirectoryByIdQueryBuilder(ExtendableContentRepository, imageAssetId);
        }

        public IContentRepositoryPageDirectoryGetAllQueryBuilder GetAll()
        {
            return new ContentRepositoryPageDirectoryGetAllQueryBuilder(ExtendableContentRepository);
        }

        #endregion
    }
}
