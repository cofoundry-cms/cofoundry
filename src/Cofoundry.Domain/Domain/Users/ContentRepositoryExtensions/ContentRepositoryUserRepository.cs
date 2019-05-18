using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryUserRepository
            : IContentRepositoryUserRepository
            , IAdvancedContentRepositoryUserRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryUserRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        #region queries

        public IContentRepositoryCurrentUserQueryBuilder GetCurrent()
        {
            return new ContentRepositoryCurrentUserQueryBuilder(ExtendableContentRepository);
        }

        public IContentRepositoryUserByIdQueryBuilder GetById(int userId)
        {
            return new ContentRepositoryUserByIdQueryBuilder(ExtendableContentRepository, userId);
        }

        public IContentRepositoryUserSearchQueryBuilder Search()
        {
            return new ContentRepositoryUserSearchQueryBuilder(ExtendableContentRepository);
        }

        public Task<bool> IsUsernameUniqueAsync(IsUsernameUniqueQuery query)
        {
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        #endregion

        #region commands

        public Task AddAsync(AddUserCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task AddCofoundryUserAsync(AddCofoundryUserCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task UpdateUserAsync(UpdateUserCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task DeleteUserAsync(int userId)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(new DeleteUserCommand(userId));
        }

        public Task UpdateCurrentUserAccountAsync(UpdateCurrentUserAccountCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task UpdateCurrentUserPasswordAsync(UpdateCurrentUserPasswordCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        #endregion
    }
}
