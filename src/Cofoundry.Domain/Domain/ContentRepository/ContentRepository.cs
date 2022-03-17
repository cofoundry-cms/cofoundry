using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class ContentRepository
    : IContentRepository
    , IAdvancedContentRepository
    , IExtendableContentRepository
{
    private IDomainRepositoryExecutor _domainRepositoryExecutor;

    public ContentRepository(
        IServiceProvider serviceProvider,
        IDomainRepositoryExecutor domainRepositoryExecutor
        )
    {
        ServiceProvider = serviceProvider;
        _domainRepositoryExecutor = domainRepositoryExecutor;
    }

    public virtual IServiceProvider ServiceProvider { get; private set; }

    public virtual Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query)
    {
        return _domainRepositoryExecutor.ExecuteAsync(query, null);
    }

    public virtual Task ExecuteCommandAsync(ICommand command)
    {
        return _domainRepositoryExecutor.ExecuteAsync(command, null);
    }

    public IExtendableContentRepository WithExecutor(Func<IDomainRepositoryExecutor, IDomainRepositoryExecutor> domainRepositoryExecutorFactory)
    {
        var newRepository = new ContentRepository(ServiceProvider, _domainRepositoryExecutor);
        newRepository.DecorateExecutor(domainRepositoryExecutorFactory);

        return newRepository;
    }

    private void DecorateExecutor(Func<IDomainRepositoryExecutor, IDomainRepositoryExecutor> domainRepositoryExecutorFactory)
    {
        if (domainRepositoryExecutorFactory == null) throw new ArgumentNullException(nameof(domainRepositoryExecutorFactory));

        var newExecutor = domainRepositoryExecutorFactory.Invoke(_domainRepositoryExecutor);
        if (newExecutor == null) throw new InvalidOperationException(nameof(domainRepositoryExecutorFactory) + " did not return an instance.");

        _domainRepositoryExecutor = newExecutor;
    }
}
