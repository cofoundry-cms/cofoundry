using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
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

        #region queries

        public IContentRepositoryRoleByIdQueryBuilder GetById(int? roleId)
        {
            return new ContentRepositoryRoleByIdQueryBuilder(ExtendableContentRepository, roleId);
        }

        public IContentRepositoryRoleByCodeQueryBuilder GetByCode(string roleCode)
        {
            return new ContentRepositoryRoleByCodeQueryBuilder(ExtendableContentRepository, roleCode);
        }

        IContentRepositoryRoleSearchQueryBuilder IAdvancedContentRepositoryRoleRepository.Search()
        {
            return new ContentRepositoryRoleSearchQueryBuilder(ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<bool> IsRoleTitleUnique(IsRoleTitleUniqueQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        #endregion

        #region commands

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

        #endregion

        #region child entities

        public IAdvancedContentRepositoryPermissionsRepository Permissions()
        {
            return new ContentRepositoryPermissionsRepository(ExtendableContentRepository);
        }

        #endregion
    }
}
