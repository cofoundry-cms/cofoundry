using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain;

public class DomainRepositoryQueryContext<TResult> : IDomainRepositoryQueryContext<TResult>
{
    public DomainRepositoryQueryContext(
        IQuery<TResult> query,
        IExtendableContentRepository extendableRepository
        )
    {
        Query = query;
        ExtendableContentRepository = extendableRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IQuery<TResult> Query { get; }

    public async Task<TResult> ExecuteAsync()
    {
        var result = await ExtendableContentRepository.ExecuteQueryAsync(Query);

        return result;
    }
}
