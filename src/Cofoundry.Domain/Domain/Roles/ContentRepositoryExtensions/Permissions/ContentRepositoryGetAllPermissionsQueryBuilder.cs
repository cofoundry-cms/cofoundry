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

        public Task<ICollection<IPermission>> AsIPermissionAsync()
        {
            var query = new GetAllPermissionsQuery();
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
