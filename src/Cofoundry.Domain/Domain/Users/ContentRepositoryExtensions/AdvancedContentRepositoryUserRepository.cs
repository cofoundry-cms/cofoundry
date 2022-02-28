using Cofoundry.Domain.Extendable;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class AdvancedContentRepositoryUserRepository
            : IAdvancedContentRepositoryUserRepository
            , IExtendableContentRepositoryPart
    {
        public AdvancedContentRepositoryUserRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryUserByIdQueryBuilder GetById(int userId)
        {
            return new ContentRepositoryUserByIdQueryBuilder(ExtendableContentRepository, userId);
        }

        public IContentRepositoryUserByEmailQueryBuilder GetByEmail(string userAreaCode, string emailAddress)
        {
            return new ContentRepositoryUserByEmailQueryBuilder(ExtendableContentRepository, userAreaCode, emailAddress);
        }

        public IContentRepositoryUserByEmailQueryBuilder GetByEmail<TUserArea>(string emailAddress) where TUserArea : IUserAreaDefinition
        {
            var userArea = GetUserAreaByType<TUserArea>();
            return new ContentRepositoryUserByEmailQueryBuilder(ExtendableContentRepository, userArea.UserAreaCode, emailAddress);
        }

        public IContentRepositoryUserByUsernameQueryBuilder GetByUsername(string userAreaCode, string username)
        {
            return new ContentRepositoryUserByUsernameQueryBuilder(ExtendableContentRepository, userAreaCode, username);
        }

        public IContentRepositoryUserByUsernameQueryBuilder GetByUsername<TUserArea>(string username) where TUserArea : IUserAreaDefinition
        {
            var userArea = GetUserAreaByType<TUserArea>();
            return new ContentRepositoryUserByUsernameQueryBuilder(ExtendableContentRepository, userArea.UserAreaCode, username);
        }

        public IContentRepositoryUserSearchQueryBuilder Search()
        {
            return new ContentRepositoryUserSearchQueryBuilder(ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<bool> IsUsernameUnique(IsUsernameUniqueQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<ValidationQueryResult> ValidateUsername(ValidateUsernameQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<bool> IsEmailAddressUnique(IsUserEmailAddressUniqueQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<ValidationQueryResult> ValidateEmailAddress(ValidateUserEmailAddressQuery query)
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

        public Task UpdateAsync(int userId, Action<UpdateUserCommand> commandPatcher)
        {
            return ExtendableContentRepository.PatchCommandAsync(userId, commandPatcher);
        }

        public Task DeleteAsync(int userId)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(new DeleteUserCommand(userId));
        }

        public Task ResetPasswordAsync(int userId)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(new ResetUserPasswordCommand()
            {
                UserId = userId
            });
        }

        public Task UpdatePasswordByCredentialsAsync(UpdateUserPasswordByCredentialsCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public IAdvancedContentRepositoryCurrentUserRepository Current()
        {
            return new ContentRepositoryCurrentUserRepository(ExtendableContentRepository);
        }

        public IAdvancedContentRepositoryUserAccountRecoveryRepository AccountRecovery()
        {
            return new ContentRepositoryUserAccountRecoveryRepository(ExtendableContentRepository);
        }

        public IAdvancedContentRepositoryUserAccountVerificationRepository AccountVerification()
        {
            return new ContentRepositoryUserAccountVerificationRepository(ExtendableContentRepository);
        }

        public IAdvancedContentRepositoryUserAuthenticationRepository Authentication()
        {
            return new ContentRepositoryUserAuthenticationRepository(ExtendableContentRepository);
        }

        private IUserAreaDefinition GetUserAreaByType<TUserArea>() where TUserArea : IUserAreaDefinition
        {
            var userAreaDefinitionRepository = ExtendableContentRepository.ServiceProvider.GetRequiredService<IUserAreaDefinitionRepository>();
            return userAreaDefinitionRepository.GetRequired<TUserArea>();
        }
    }
}