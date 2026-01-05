namespace Cofoundry.Domain.Extendable;

/// <summary>
/// Extension methods for <see cref="IDomainRepositoryTransactionManager"/>.
/// </summary>
public static class IDomainRepositoryTransactionManagerExtendableExtensions
{
    extension(IDomainRepositoryTransactionManager domainRepositoryTransactionManager)
    {
        public IExtendableDomainRepositoryTransactionManager AsExtendableDomainRepositoryTransactionManager()
        {
            if (domainRepositoryTransactionManager is IExtendableDomainRepositoryTransactionManager extendable)
            {
                return extendable;
            }

            throw new Exception($"An {nameof(IDomainRepositoryTransactionManager)} implementation should also implement {nameof(IExtendableDomainRepositoryTransactionManager)} to allow internal/plugin extendibility.");
        }
    }
}
