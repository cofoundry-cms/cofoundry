using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryGetAllPermissionsQueryBuilder
        : IAdvancedContentRepositoryGetAllPermissionsQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryGetAllPermissionsQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryQueryContext<ICollection<IPermission>> AsIPermission()
        {
            var query = new GetAllPermissionsQuery();
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
