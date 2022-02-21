using Cofoundry.Domain.Extendable;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class ContentRepositoryCurrentUserRepository
            : IAdvancedContentRepositoryCurrentUserRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryCurrentUserRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryCurrentUserQueryBuilder Get()
        {
            return new ContentRepositoryCurrentUserQueryBuilder(ExtendableContentRepository);
        }

        public IDomainRepositoryQueryMutator<IUserContext, bool> IsSignedIn()
        {
            var query = new GetCurrentUserContextQuery();

            return Get()
                .AsUserContext()
                .Map(u => u.IsSignedIn());
        }

        public Task UpdateAsync(UpdateCurrentUserCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task UpdateAsync(Action<UpdateCurrentUserCommand> commandPatcher)
        {
            return ExtendableContentRepository.PatchCommandAsync(commandPatcher);
        }

        public Task UpdatePasswordAsync(UpdateCurrentUserPasswordCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task DeleteAsync()
        {
            return ExtendableContentRepository.ExecuteCommandAsync(new DeleteCurrentUserCommand());
        }
    }
}