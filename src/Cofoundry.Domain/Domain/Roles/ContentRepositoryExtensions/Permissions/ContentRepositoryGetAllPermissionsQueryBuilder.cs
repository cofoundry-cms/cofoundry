using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
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

        public IDomainRepositoryQueryContext<ICollection<IPermission>> AsIPermission()
        {
            var query = new GetAllPermissionsQuery();
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
