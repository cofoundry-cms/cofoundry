using Cofoundry.Domain.Extendable;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryRoleRepository
            : IContentRepositoryRoleRepository
            , IAdvancedContentRepositoryRoleRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryRoleRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryRoleByIdQueryBuilder GetById(int? roleId)
        {
            return new ContentRepositoryRoleByIdQueryBuilder(ExtendableContentRepository, roleId);
        }

        public IContentRepositoryRoleByCodeQueryBuilder GetByCode(string roleCode)
        {
            return new ContentRepositoryRoleByCodeQueryBuilder(ExtendableContentRepository, roleCode);
        }

        public IContentRepositoryRoleByIdRangeQueryBuilder GetByIdRange(IEnumerable<int> roleIds)
        {
            return new ContentRepositoryRoleByIdRangeQueryBuilder(ExtendableContentRepository, roleIds);
        }

        IContentRepositoryRoleSearchQueryBuilder IAdvancedContentRepositoryRoleRepository.Search()
        {
            return new ContentRepositoryRoleSearchQueryBuilder(ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<bool> IsRoleTitleUnique(IsRoleTitleUniqueQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public async Task<int> AddAsync(AddRoleCommand command)
        {
            await ExtendableContentRepository.ExecuteCommandAsync(command);
            return command.OutputRoleId;
        }

        public Task UpdateRoleAsync(UpdateRoleCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task DeleteRoleAsync(int roleId)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(new DeleteRoleCommand(roleId));
        }

        public Task RegisterPermissionsAndRoles(RegisterPermissionsAndRolesCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public IAdvancedContentRepositoryPermissionsRepository Permissions()
        {
            return new ContentRepositoryPermissionsRepository(ExtendableContentRepository);
        }
    }
}
