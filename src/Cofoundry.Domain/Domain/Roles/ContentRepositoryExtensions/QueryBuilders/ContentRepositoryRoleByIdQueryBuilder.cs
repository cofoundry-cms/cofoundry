using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryRoleByIdQueryBuilder
        : IContentRepositoryRoleByIdQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly int? _roleId;

        public ContentRepositoryRoleByIdQueryBuilder(
            IExtendableContentRepository contentRepository,
            int? roleId
            )
        {
            ExtendableContentRepository = contentRepository;
            _roleId = roleId;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryQueryContext<RoleDetails> AsDetails()
        {
            var query = new GetRoleDetailsByIdQuery(_roleId);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
