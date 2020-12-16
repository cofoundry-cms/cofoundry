using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryPermissionsRepository
            : IAdvancedContentRepositoryPermissionsRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPermissionsRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        #region queries

        public IAdvancedContentRepositoryGetAllPermissionsQueryBuilder GetAll()
        {
            return new ContentRepositoryGetAllPermissionsQueryBuilder(ExtendableContentRepository);
        }
                
        #endregion
    }
}
