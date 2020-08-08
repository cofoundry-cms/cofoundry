using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryPageBlockTypeRepository
            : IAdvancedContentRepositoryPageBlockTypeRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPageBlockTypeRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        #region queries

        public IContentRepositoryPageBlockTypeByIdQueryBuilder GetById(int pageBlockTypeId)
        {
            return new ContentRepositoryPageBlockTypeByIdQueryBuilder(ExtendableContentRepository, pageBlockTypeId);
        }

        public IContentRepositoryPageBlockTypeGetAllQueryBuilder GetAll()
        {
            return new ContentRepositoryPageBlockTypeGetAllQueryBuilder(ExtendableContentRepository);
        }

        #endregion
    }
}
