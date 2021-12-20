using Cofoundry.Domain.Extendable;
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

        public IContentRepositoryCurrentUserQueryBuilder GetCurrent()
        {
            return new ContentRepositoryCurrentUserQueryBuilder(ExtendableContentRepository);
        }

        public IContentRepositoryUserByIdQueryBuilder GetById(int userId)
        {
            return new ContentRepositoryUserByIdQueryBuilder(ExtendableContentRepository, userId);
        }

        public IContentRepositoryUserByEmailQueryBuilder GetByEmail(string userAreaCode, string emailAddress)
        {
            return new ContentRepositoryUserByEmailQueryBuilder(ExtendableContentRepository, userAreaCode, emailAddress);
        }

        public IContentRepositoryUserByUsernameQueryBuilder GetByUsername(string userAreaCode, string username)
        {
            return new ContentRepositoryUserByUsernameQueryBuilder(ExtendableContentRepository, userAreaCode, username);
        }

        public IContentRepositoryUserSearchQueryBuilder Search()
        {
            return new ContentRepositoryUserSearchQueryBuilder(ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<bool> IsUsernameUnique(IsUsernameUniqueQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<bool> IsEmailUnique(IsEmailUniqueQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public async Task<int> AddAsync(AddUserCommand command)
        {
            await ExtendableContentRepository.ExecuteCommandAsync(command);
            return command.OutputUserId;
        }

        public async Task<int> AddWithTemporaryPasswordAsync(AddUserWithTemporaryPasswordCommand command)
        {
            await ExtendableContentRepository.ExecuteCommandAsync(command);
            return command.OutputUserId;
        }

        public Task UpdateAsync(UpdateUserCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task DeleteAsync(int userId)
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

        public Task ResetPasswordAsync(int userId)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(new ResetUserPasswordCommand()
            {
                UserId = userId
            });
        }

        public IAdvancedContentRepositoryUserPasswordResetRequestsRepository PasswordResetRequests()
        {
            return new ContentRepositoryUserPasswordResetRequestsRepository(ExtendableContentRepository);
        }
    }
}
