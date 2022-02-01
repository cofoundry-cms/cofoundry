using Cofoundry.Domain.Extendable;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryAuthorizedTaskRepository
            : IAdvancedContentRepositoryAuthorizedTaskRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryAuthorizedTaskRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public async Task<string> AddAsync(AddAuthorizedTaskCommand command)
        {
            await ExtendableContentRepository.ExecuteCommandAsync(command);
            return command.OutputToken;
        }

        public Task CompleteAsync(CompleteAuthorizedTaskCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task InvalidateBatchAsync(InvalidateAuthorizedTaskBatchCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public IDomainRepositoryQueryContext<AuthorizedTaskTokenValidationResult> ValidateAsync(ValidateAuthorizedTaskTokenQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
