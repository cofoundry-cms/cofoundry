using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryCustomEntityDefinitionsRepository
            : IContentRepositoryCustomEntityDefinitionsRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryCustomEntityDefinitionsRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        #region queries

        public IContentRepositoryCustomEntityDefinitionGetAllQueryBuilder GetAll()
        {
            return new ContentRepositoryCustomEntityDefinitionGetAllQueryBuilder(ExtendableContentRepository);
        }

        public IContentRepositoryCustomEntityDefinitionByCodeQueryBuilder GetByCode(string customEntityDefinitioncode)
        {
            return new ContentRepositoryCustomEntityDefinitionByCodeQueryBuilder(ExtendableContentRepository, customEntityDefinitioncode);
        }

        #endregion
    }
}
