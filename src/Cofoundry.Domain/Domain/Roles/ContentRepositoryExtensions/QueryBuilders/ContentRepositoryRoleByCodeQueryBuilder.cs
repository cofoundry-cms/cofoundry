using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryRoleByCodeQueryBuilder
        : IContentRepositoryRoleByCodeQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly string _roleCodeId;

        public ContentRepositoryRoleByCodeQueryBuilder(
            IExtendableContentRepository contentRepository,
            string roleCode
            )
        {
            ExtendableContentRepository = contentRepository;
            _roleCodeId = roleCode;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<RoleDetails> AsDetailsAsync()
        {
            var query = new GetRoleDetailsByRoleCodeQuery(_roleCodeId);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
