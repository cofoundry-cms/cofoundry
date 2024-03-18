﻿using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

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

        if (string.IsNullOrEmpty(command.OutputToken))
        {
            throw new InvalidOperationException($"{nameof(command.OutputToken)} should not be empty after executing {nameof(AddAuthorizedTaskCommand)}");
        }

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
