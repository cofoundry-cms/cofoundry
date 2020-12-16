using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
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

        public IDomainRepositoryQueryContext<bool> IsUsernameUnique(IsUsernameUniqueQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        #endregion

        #region commands

        public async Task<int> AddAsync(AddUserCommand command)
        {
            await ExtendableContentRepository.ExecuteCommandAsync(command);
            return command.OutputUserId;
        }

        public async Task<int> AddCofoundryUserAsync(AddCofoundryUserCommand command)
        {
            await ExtendableContentRepository.ExecuteCommandAsync(command);
            return command.OutputUserId;
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
